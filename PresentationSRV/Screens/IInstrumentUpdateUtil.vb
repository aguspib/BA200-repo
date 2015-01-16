Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.Controls.UserControls
Imports System.Runtime.InteropServices 'WIN32
'Imports Biosystems.Ax00.Global.TO
'Imports Biosystems.Ax00.PresentationCOM
Imports System.IO
Imports System.Drawing.Printing
Imports System.Xml.Serialization

'Pendiente
'**************************


'Instrucciones necesarioas
'************************************
'RESET (start-end)
'LOADFACTORYADJ (start-end)
'FWUTIL


Public Class IInstrumentUpdateUtil
    Inherits PesentationLayer.BSAdjustmentBaseForm


#Region "Declarations"
    Private WithEvents myScreenDelegate As InstrumentUpdateUtilDelegate 'Screen's business delegate

    Private myAdjustmentsMasterDataDS As SRVAdjustmentsDS
    Private CurrentAdjustmentsDS As New SRVAdjustmentsDS 'readed from Analyzer

    Private InitialAdjustmentsText As String = "" 'readed adj from Analyzer

    Private ImportedBinaryFwFile() As Byte 'imported fw from file
    Private AnalyzerBinaryFwFile() As Byte 'Analyzer's current fw
    Private AnalyzerFirmwareVersion As String

    Private SelectedPage As UPDATE_PAGES = UPDATE_PAGES.ADJUSTMENTS

    ' Waiting process functionality
    Private TimeForWait As Integer = 0 'miliseconds to wait until END response of an instruction

    'action timer (used for waiting before sending consecutive next instruction)
    Private ActionTimerCallBack As System.Threading.TimerCallback
    Private WithEvents ActionTimer As System.Threading.Timer

    'wait timer (used for displaying the waiting progress bar)
    Private WaitTimerCallBack As System.Threading.TimerCallback
    Private WithEvents WaitTimer As System.Threading.Timer
    Private Delegate Sub UpdateProgressCallBack()
    Private Delegate Sub HideProgressBarCallBack()

    'simulation timer (used for displaying waiting progress bar in suimulation mode)
    Private SimulationTimerCallBack As System.Threading.TimerCallback
    Private WithEvents SimulationTimer As System.Threading.Timer

    'Printing
    Private myTextToPrint As String = ""
    Private myPageToPrint As String = ""
    Private myFontToPrint As New Font("Arial", 12)
    Private myPrintSettings As New PageSettings

    ' Language
    Private currentLanguage As String

    'Information
    Private SelectedInfoPanel As Panel
    Private SelectedAdjPanel As Panel
    Private SelectedInfoExpandButton As Panel

    Private EditionAllowedAttr As Boolean = False
    Private AdjustmentsEditionChangesMadeAttr As Boolean = False


    'FW Update Result 
    Private FirmwareUpdateResult As FW_GENERIC_RESULT = FW_GENERIC_RESULT.KO

    Private NeededUpdatesResponse As FWUpdateResponseTO 'object for knowing which FW elements need to be updated


#End Region

#Region "Constants"

#End Region

#Region "Flags"
    Private IsReadyToSendNext As Boolean = False 'ready for sending next consecutive instruction
    Private ManageTabPages As Boolean = False 'permission for changing tabs
#End Region

#Region "Constructor"
    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout(UPDATE_PAGES.ADJUSTMENTS)

    End Sub

    Sub New(ByVal pOnlyFirmwareUpdate As Boolean)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        MyClass.IsOnlyFirmwareUpdate = pOnlyFirmwareUpdate

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout(UPDATE_PAGES.FIRMWARE)


    End Sub
#End Region

#Region "Enumerates"

    'screen tab pages
    Private Enum UPDATE_PAGES
        _NONE
        ADJUSTMENTS
        FIRMWARE
    End Enum

#End Region

#Region "Properties"

    Private Property IsOnlyFirmwareUpdate() As Boolean
        Get
            Return IsOnlyFirmwareUpdateAttr
        End Get
        Set(ByVal value As Boolean)
            IsOnlyFirmwareUpdateAttr = value
            If Not value Then
                MyClass.SelectedPage = UPDATE_PAGES.ADJUSTMENTS
            Else
                MyClass.SelectedPage = UPDATE_PAGES.FIRMWARE
            End If
        End Set
    End Property
    Private IsOnlyFirmwareUpdateAttr As Boolean = False

    ''' <summary>
    ''' determines if the displayed adjustments are already saved
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsAlreadySaved() As Boolean
        Get
            Return IsAlreadySavedAttr
        End Get
        Set(ByVal value As Boolean)
            IsAlreadySavedAttr = value
        End Set
    End Property
    Private IsAlreadySavedAttr As Boolean = False


    ''' <summary>
    ''' determines if a printing work is being carried out
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsPrinting() As Boolean
        Get
            Return IsPrintingAttr
        End Get
        Set(ByVal value As Boolean)
            IsPrintingAttr = value
        End Set
    End Property
    Private IsPrintingAttr As Boolean = False


    ''' <summary>
    ''' determines if an action is being carried out
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsActionRequested() As Boolean
        Get
            Return IsActionRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If value Then
                MyBase.ActivateMDIMenusButtons(False)
            Else
                MyBase.ActivateMDIMenusButtons(True)
            End If

            MyClass.ManageTabPages = Not value


            IsActionRequestedAttr = value
        End Set
    End Property
    Private IsActionRequestedAttr As Boolean = False

    ''' <summary>
    ''' determines if restoring operation is requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsRestoringRequested() As Boolean
        Get
            Return IsRestoringRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If IsRestoringRequestedAttr <> value Then
                IsRestoringRequestedAttr = value
            End If
        End Set
    End Property
    Private IsRestoringRequestedAttr As Boolean = False

    ''' <summary>
    ''' determines if restoring to factory operation is requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsRestoringToFactoryRequested() As Boolean
        Get
            Return IsRestoringToFactoryRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If IsRestoringToFactoryRequestedAttr <> value Then
                IsRestoringToFactoryRequestedAttr = value
            End If
        End Set
    End Property
    Private IsRestoringToFactoryRequestedAttr As Boolean = False

    ''' <summary>
    ''' determines if current firmware reading operation is requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Property IsCurrentFwRequested() As Boolean
        Get
            Return IsCurrentFwRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            IsCurrentFwRequestedAttr = value
            If MyClass.SelectedPage = UPDATE_PAGES.FIRMWARE Then
                Me.BsFwUpdateButton.Enabled = Not value
            End If
        End Set
    End Property
    Private IsCurrentFwRequestedAttr As Boolean = False

    ''' <summary>
    ''' determines if firmware updating operation is requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Public Property IsFwUpdateRequested() As Boolean
        Get
            Return IsFwUpdateRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If IsFwUpdateRequestedAttr <> value Then
                IsFwUpdateRequestedAttr = value
                If value Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATING
                    MyClass.PrepareArea()
                    Me.BsFwUpdateButton.Enabled = False
                End If
                MyBase.myServiceMDI.MDIAnalyzerManager.IsFwUpdateInProcess = value
            End If
        End Set
    End Property
    Private IsFwUpdateRequestedAttr As Boolean = False


    Private ReadOnly Property IsFwFileReadyToSend() As Boolean
        Get
            Return (myScreenDelegate.IsFwFileCompatible AndAlso myScreenDelegate.FWFileBlocks.Count > 0 AndAlso myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None)
        End Get
    End Property

    'Editing
    Private Property AdjustmentsEditionMode() As Boolean
        Get
            Return MyClass.AdjustmentsEditionModeAttr
        End Get
        Set(ByVal value As Boolean)
            If Not EditionAllowed Then value = False
            If MyClass.AdjustmentsEditionModeAttr <> value Then
                MyClass.AdjustmentsEditionModeAttr = value
                If value Then
                    Me.BsAdjustmentsRTextBox.BackColor = Color.White
                    Me.BsAdjustmentsRTextBox.Focus()
                Else
                    MyClass.AdjustmentsEditionChangesMade = False
                    Me.BsAdjustmentsRTextBox.BackColor = Color.LightYellow
                    HideCaret(Me.BsAdjustmentsRTextBox.Handle)
                End If
            End If

            Me.BsAdjEditButton.Enabled = Not value
            Me.BsAdjUndoButton.Enabled = value

        End Set
    End Property
    Private AdjustmentsEditionModeAttr As Boolean = False

    Private Property AdjustmentsEditionChangesMade() As Boolean
        Get
            Return MyClass.AdjustmentsEditionChangesMadeAttr
        End Get
        Set(ByVal value As Boolean)
            If MyClass.AdjustmentsEditionChangesMadeAttr <> value Then
                MyClass.AdjustmentsEditionChangesMadeAttr = value
                Me.BsAdjEditButton.Enabled = Not value
                Me.BsAdjUndoButton.Enabled = value

            End If
        End Set
    End Property

    Private Property EditionAllowed() As Boolean
        Get
            Return MyClass.EditionAllowedAttr
        End Get
        Set(ByVal value As Boolean)
            MyClass.EditionAllowedAttr = value
            Me.BsAdjNewButton.Visible = value
            Me.BsAdjEditButton.Visible = value
            Me.BsAdjUndoButton.Visible = value
        End Set
    End Property

#End Region

#Region "Private Methods"

#Region "Common"

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: SGM 23/06/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim MLRD As New MultilanguageResourcesDelegate

            Me.BsAdjustmentsTab.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_ADJUTIL_TAB", currentLanguage)
            Me.BsFirmwareTab.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_TAB", currentLanguage)

            'adjustments
            Me.BsAdjInfoTitleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            Me.BsAdjustmentsTitleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_ADJUTIL_TAB", currentLanguage)

            'firmware
            Me.BsFirmwareInfoTitleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            Me.BsFwTitleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_TAB", currentLanguage)

            Me.BsFwCurrentVersionCaptionLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_VERSION1", currentLanguage)
            Me.BsFwOpenFileCaptionLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_OPEN", currentLanguage)
            Me.BsFwNewVersionCaptionLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_VERSION2", currentLanguage)
            Me.BsFwUpdateCaptionLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FWUTIL_UPDATE", currentLanguage)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjOpenFileButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_OPEN", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjSaveAsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_SAVE", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjBackupButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_BK", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjRestoreButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_RES", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjRestoreFactoryButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_FAC", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsFwUpdateButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FW_UPT", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))

            If MyClass.EditionAllowed Then
                MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_NEW", currentLanguage))
                MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_EDIT", currentLanguage))
                MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsAdjUndoButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ADJ_UNDO", currentLanguage))
            End If

            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: SGM 23/06/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Try

            MyBase.SetButtonImage(BsExitButton, "CANCEL")
            MyBase.SetButtonImage(BsAdjOpenFileButton, "OPEN")
            MyBase.SetButtonImage(BsAdjSaveAsButton, "SAVE")
            MyBase.SetButtonImage(BsAdjBackupButton, "READADJ")
            MyBase.SetButtonImage(BsAdjRestoreButton, "LOADADJ")
            MyBase.SetButtonImage(BsAdjRestoreFactoryButton, "FACTORY")
            MyBase.SetButtonImage(BsAdjPrintButton, "PRINT")
            MyBase.SetButtonImage(BsAdjNewButton, "ADD")
            MyBase.SetButtonImage(BsAdjEditButton, "EDIT")
            MyBase.SetButtonImage(BsAdjUndoButton, "UNDO")
            MyBase.SetButtonImage(BsFwOpenFileButton, "OPEN")
            MyBase.SetButtonImage(BsFwUpdateButton, "LOADADJ")


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enables all available buttons in the screen
    ''' </summary>
    ''' <remarks>SGM 23/06/2011</remarks>
    Private Sub EnableButtons()
        Try
            Me.BsAdjNewButton.Enabled = True
            Me.BsAdjOpenFileButton.Enabled = True
            Me.BsAdjSaveAsButton.Enabled = (MyClass.BsAdjustmentsRTextBox.Text.Length > 0)
            Me.BsAdjBackupButton.Enabled = True
            Me.BsAdjRestoreButton.Enabled = True
            Me.BsAdjRestoreFactoryButton.Enabled = True
            Me.BsAdjPrintButton.Enabled = Me.BsAdjustmentsRTextBox.Text.Length > 0

            Me.BsFwOpenFileButton.Enabled = True
            Me.BsFwUpdateButton.Enabled = MyClass.IsFwFileReadyToSend

            Me.BsExitButton.Enabled = Not MyClass.IsOnlyFirmwareUpdate


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".EnableButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>SGM 23/06/2011</remarks>
    Private Sub DefineScreenLayout(ByVal pPage As UPDATE_PAGES)
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                Select Case pPage
                    Case UPDATE_PAGES.ADJUSTMENTS
                        .AdjustmentPanel.Container = Me.BsAdjustmentsPanel
                        .InfoPanel.Container = Me.BsAdjInfoPanel
                        .InfoPanel.InfoXPS = BsInfoAdjXPSViewer


                    Case UPDATE_PAGES.FIRMWARE
                        .AdjustmentPanel.Container = Me.BsFirmwarePanel
                        .InfoPanel.Container = Me.BsFwInfoPanel
                        .InfoPanel.InfoXPS = BsInfoFwXPSViewer


                    Case Else
                        .AdjustmentPanel.Container = Nothing
                        .InfoPanel.Container = Nothing
                        .InfoPanel.InfoXPS = Nothing


                End Select


            End With
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#End Region

#Region "Adjustments"

    ''' <summary>
    ''' Loads Adjustments Master Data
    ''' </summary>
    ''' <remarks>Created by SGM 23/06/2011 </remarks>
    Private Sub LoadAdjustmentsMasterData()

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal = MyBase.myServiceMDI.MDIAnalyzerManager.LoadFwAdjustmentsMasterData(MyBase.SimulationMode)
            If Not myGlobal.HasError OrElse myGlobal.SetDatos IsNot Nothing Then
                myAdjustmentsMasterDataDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentsMasterData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentsMasterData", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Warns user that some changes have been made during Adjustments edition
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Function WarnIfAdjustmentsEditionChanges() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            If MyClass.AdjustmentsEditionChangesMade Then
                Dim res As DialogResult = MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_EDIT_ADJ_SAVE_ASK.ToString)
                'Dim res As DialogResult = MessageBox.Show("There are some changes in the Adjustments edition." & vbCrLf & "Do you want to save them before continue?", "", MessageBoxButtons.YesNoCancel)
                Select Case res
                    Case Windows.Forms.DialogResult.Yes
                        myGlobal = MyClass.SaveAdjustmentsFileAs()
                        If Not myGlobal.HasError Then
                            myGlobal.SetDatos = True
                        End If

                    Case Windows.Forms.DialogResult.No
                        myGlobal.SetDatos = True

                    Case Windows.Forms.DialogResult.Cancel
                        myGlobal.SetDatos = False

                End Select
            Else
                myGlobal.SetDatos = True
            End If

            MyClass.AdjustmentsEditionMode = Not CBool(myGlobal.SetDatos)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".WarnIfAdjustmentsEditionChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WarnIfAdjustmentsEditionChanges", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Validates the contents of the open Adjustments file
    ''' </summary>
    ''' <param name="pData"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Function ValidateAdjustmentsData(ByVal pData As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myFwAdjustmentsDelegate As New FwAdjustmentsDelegate(MyClass.myAdjustmentsMasterDataDS)
            Dim myAnalyzerModel As String = "A400"
            Dim myAnalyzerID As String = pData.Substring(pData.IndexOf("SN") + 2, 9)
            myGlobal = myFwAdjustmentsDelegate.ConvertReceivedDataToDS(pData, myAnalyzerID, "", New SRVAdjustmentsDS)
            If myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing Then
                Dim res As DialogResult = MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_WRONG_ADJ_ASK.ToString)
                'Dim ans As DialogResult = MessageBox.Show("The edited data is not valid." & vbCrLf & "Do you really want to continue anyway?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                If res = Windows.Forms.DialogResult.No Then
                    Return myGlobal
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateAdjustmentsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateAdjustmentsData", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Validates the compatibility of the Adjustments file with the Analyzer Model
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 23/06/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function CheckAdjustmentsCompatibility() As Boolean
        Dim myRes As Boolean = True
        Try
            If Not MyBase.SimulationMode Then
                'If MyClass.CurrentAdjustmentsDS.AnalyzerModel.ToUpper.Trim <> MyBase.myServiceMDI.AnalyzerModel.ToUpper.Trim Then
                If MyClass.CurrentAdjustmentsDS.AnalyzerModel.Trim <> MyBase.myServiceMDI.AnalyzerModel.Trim Then
                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_ADJ_WRONG_MODEL.ToString)
                    myRes = False
                    'ElseIf MyClass.CurrentAdjustmentsDS.FirmwareVersion.ToUpper.Trim <> MyBase.myServiceMDI.ActiveFwVersion.ToUpper.Trim Then
                ElseIf MyClass.CurrentAdjustmentsDS.FirmwareVersion.Trim <> MyBase.myServiceMDI.ActiveFwVersion.Trim Then
                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_ADJ_WRONG_FWVER.ToString)
                    myRes = False
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".CheckAdjustmentsCompatibility", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CheckAdjustmentsCompatibility", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myRes
    End Function


    ''' <summary>
    ''' Exports an Adjustments File in order to save as
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 28/11/2011</remarks>
    Private Function SaveAdjustmentsFileAs() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myTempPath As String = ""

        Try
            Dim myAdjustmentsDataToSave As String = Me.BSAdjHeaderRTextBox.Text & vbCrLf & Me.BsAdjustmentsRTextBox.Text 'SGM 03/10/2012
            'Dim myAdjustmentsDataToSave As String = Me.BsAdjustmentsRTextBox.Text

            'validate data
            myGlobal = MyClass.ValidateAdjustmentsData(myAdjustmentsDataToSave)

            If Not myGlobal.HasError Then
                With Me.SaveAdjFileDialog
                    .Title = "Save Adjustments As"
                    .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                    .Filter = "Adjustments files|*" & FwAdjustmentsDelegate.AdjustmentCryptedFileExtension
                    .CheckPathExists = True
                    .OverwritePrompt = True
                End With

                Dim res As DialogResult = Me.SaveAdjFileDialog.ShowDialog

                If res <> Windows.Forms.DialogResult.Cancel Then

                    MyBase.DisplayMessage("")

                    Dim myPath As String = Me.SaveAdjFileDialog.FileName
                    myTempPath = Me.SaveAdjFileDialog.FileName.Replace(FwAdjustmentsDelegate.AdjustmentCryptedFileExtension, FwAdjustmentsDelegate.AdjustmentDecryptedFileExtension)

                    myGlobal = MyClass.ExportAdjustmentsDataset(MyClass.CurrentAdjustmentsDS, myPath, myTempPath, True)
                    If Not myGlobal.HasError Then
                        Me.BsAdjOpenFileButton.Enabled = True
                        Me.BsAdjBackupButton.Enabled = True
                        Me.BsAdjSaveAsButton.Enabled = True
                        Me.BsAdjRestoreButton.Enabled = True
                        Me.BsAdjRestoreFactoryButton.Enabled = True
                        Me.BsAdjPrintButton.Enabled = True
                    End If

                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAdjustmentsFileAs ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAdjustmentsFileAs", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If Not myGlobal.HasError Then
            MyBase.ActivateMDIMenusButtons(True)
        End If

        Return myGlobal

    End Function

    ''' <summary>
    ''' Opens Adjustments file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Function OpenAdjustmentsFile() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myTempPath As String = ""

        Try
            With Me.OpenAdjFileDialog
                .Title = "Open Adjustments File"
                .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                .Filter = "Adjustments files|*" & FwAdjustmentsDelegate.AdjustmentCryptedFileExtension
                .CheckFileExists = True
                .Multiselect = False
                .FileName = ""
            End With

            Dim res As DialogResult = Me.OpenAdjFileDialog.ShowDialog

            If res <> Windows.Forms.DialogResult.Cancel Then

                MyBase.DisplayMessage("")

                MyClass.AdjustmentsEditionMode = False

                Dim path As String = Me.OpenAdjFileDialog.FileName


                'deserialize decrypted data
                Dim myUtility As New Utilities
                Dim myOpenAdjustmentsDS As New SRVAdjustmentsDS
                myTempPath = path.Replace(FwAdjustmentsDelegate.AdjustmentCryptedFileExtension, FwAdjustmentsDelegate.AdjustmentDecryptedFileExtension)

                myGlobal = MyClass.ImportAdjustmentsDataset(MyClass.CurrentAdjustmentsDS.GetType, path, myTempPath, True)

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    myOpenAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                    If myOpenAdjustmentsDS.srv_tfmwAdjustments.Rows.Count > 0 Then
                        'get the Analyzer Model, Firmware Version and readed datetime
                        Dim myRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
                        myRow = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                  In myOpenAdjustmentsDS.srv_tfmwAdjustments _
                                  Select a).First

                        'Dim myAnalyzerID As String = myRow.AnalyzerID
                        'Dim i As Integer = myAnalyzerID.IndexOf("_")
                        'If i >= 0 Then
                        '    myOpenAdjustmentsDS.AnalyzerID = myRow.AnalyzerID.Substring(i + 1)
                        'Else
                        '    myOpenAdjustmentsDS.AnalyzerID = myRow.AnalyzerID
                        'End If

                        myOpenAdjustmentsDS.AnalyzerModel = "A400"
                        myOpenAdjustmentsDS.AnalyzerID = myRow.AnalyzerID
                        myOpenAdjustmentsDS.FirmwareVersion = myRow.FwVersion
                        myOpenAdjustmentsDS.ReadedDatetime = New FileInfo(path).CreationTime

                        'get string data
                        Dim myStringData As String = ""
                        Dim myHeaderStringData As String = ""
                        Dim myAdjDelegate As New FwAdjustmentsDelegate(myOpenAdjustmentsDS)
                        myGlobal = myAdjDelegate.ConvertDSToString()
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                            myHeaderStringData = myAdjDelegate.MakeAdjustmentsFileHeader().Trim

                            Dim myContentsData As String = CStr(myGlobal.SetDatos)
                            myStringData = myHeaderStringData & myContentsData

                            MyClass.CurrentAdjustmentsDS = myOpenAdjustmentsDS

                            MyClass.AdjustmentsEditionMode = False

                            Me.BsAdjOpenFileButton.Enabled = True
                            Me.BsAdjBackupButton.Enabled = True
                            Me.BsAdjSaveAsButton.Enabled = True
                            Me.BsAdjRestoreButton.Enabled = True
                            Me.BsAdjRestoreFactoryButton.Enabled = True
                            Me.BsAdjPrintButton.Enabled = True

                            Me.BSAdjHeaderRTextBox.Text = myHeaderStringData 'SGM 03/10/2012
                            Me.BsAdjustmentsRTextBox.Text = myContentsData
                            MyClass.InitialAdjustmentsText = myStringData
                            Me.BsAdjustmentsRTextBox.Refresh()
                            MyBase.DisplayMessage(Messages.SRV_ADJ_FILE_OPENED.ToString)

                        End If
                    End If

                End If

                Application.DoEvents()

                'set editable in case of enough user privileges

            End If

            myGlobal.SetDatos = res

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".OpenAdjustmentsFile ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".OpenAdjustmentsFile", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If File.Exists(myTempPath) Then
            File.Delete(myTempPath)
        End If

        Return myGlobal

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAdjustmentsDS"></param>
    ''' <param name="pFilePath"></param>
    ''' <param name="pTempFilePath"></param>
    ''' <param name="pEncrypt"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Function ExportAdjustmentsDataset(ByVal pAdjustmentsDS As SRVAdjustmentsDS, ByVal pFilePath As String, ByVal pTempFilePath As String, ByVal pEncrypt As Boolean) As GlobalDataTO

        Dim resultData As New GlobalDataTO

        'Dim myGlobalbase As New GlobalBase

        Try
            'delete if previously exists
            If File.Exists(pTempFilePath) Then
                File.Delete(pTempFilePath)
            End If


            Dim FS As System.IO.FileStream

            If pEncrypt Then
                FS = System.IO.File.OpenWrite(pTempFilePath)
            Else
                FS = System.IO.File.OpenWrite(pFilePath)
            End If

            Dim serializer As New System.Xml.Serialization.XmlSerializer(pAdjustmentsDS.GetType)
            serializer.Serialize(FS, pAdjustmentsDS)

            FS.Close()
            FS.Dispose()

            If pEncrypt Then
                'encrypt the file
                If File.Exists(pTempFilePath) Then

                    Dim myUtil As New Utilities
                    resultData = myUtil.EncryptFile(pTempFilePath, pFilePath)

                Else
                    Throw New Exception(GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
                End If
            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ExportAdjustmentsDataset ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExportAdjustmentsDataset", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            'delete the temp file
            If File.Exists(pTempFilePath) Then
                File.Delete(pTempFilePath)
            End If
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAdjustmentsDatasetType"></param>
    ''' <param name="pFilePath"></param>
    ''' <param name="pTempFilePath"></param>
    ''' <param name="pDecrypt"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Function ImportAdjustmentsDataset(ByVal pAdjustmentsDatasetType As Type, ByVal pFilePath As String, ByVal pTempFilePath As String, ByVal pDecrypt As Boolean) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim myUtil As New Utilities
        ' 
        Dim FS As FileStream = Nothing

        Try

            Dim serializer As New XmlSerializer(pAdjustmentsDatasetType)

            If pDecrypt Then

                If File.Exists(pTempFilePath) Then
                    File.Delete(pTempFilePath)
                End If

                resultData = myUtil.DecryptFile(pFilePath, pTempFilePath)

                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    If File.Exists(pTempFilePath) Then
                        FS = File.OpenRead(pTempFilePath)
                        resultData.SetDatos = CType(serializer.Deserialize(FS), SRVAdjustmentsDS)
                        FS.Close()
                        FS.Dispose()

                        File.Delete(pTempFilePath)
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If
            Else
                'without decrypting
                FS = File.OpenRead(pFilePath)
                resultData.SetDatos = CType(serializer.Deserialize(FS), SRVAdjustmentsDS)
                FS.Close()
                FS.Dispose()
            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ImportAdjustmentsDataset ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ImportAdjustmentsDataset", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            If Not FS Is Nothing Then
                FS.Close()
                FS.Dispose()
            End If
            If File.Exists(pTempFilePath) Then
                File.Delete(pTempFilePath)
            End If
        End Try
        Return resultData
    End Function




#Region "Backup Adjustments"

    ''' <summary>
    ''' Request adjustments from analyzer
    ''' </summary>
    ''' <remarks>Created by SGM 24/06/2011</remarks>
    Private Function RequestAdjustFromInstrument() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.ActivateMDIMenusButtons(False)

            MyClass.AdjustmentsEditionMode = False

            Me.BsAdjNewButton.Enabled = False
            Me.BsAdjOpenFileButton.Enabled = False
            Me.BsAdjSaveAsButton.Enabled = False
            Me.BsAdjEditButton.Enabled = False
            Me.BsAdjUndoButton.Enabled = False
            Me.BsAdjBackupButton.Enabled = False
            Me.BsAdjRestoreButton.Enabled = False
            Me.BsAdjRestoreFactoryButton.Enabled = False
            Me.BsAdjPrintButton.Enabled = False

            Me.BSAdjHeaderRTextBox.Text = "" 'SGM 03/10/2012
            Me.BsAdjustmentsRTextBox.Text = ""

            ' Disable Area Buttons
            Me.BsExitButton.Enabled = False

            ' Reading Adjustments
            'If Not MyClass.myServiceMDI.AdjustmentsReaded Then
            ' Parent Reading Adjustments
            MyBase.ReadAdjustments()
            PrepareArea()

            ' Manage FwScripts must to be sent at load screen
            If MyBase.SimulationMode Then
                MyBase.myServiceMDI.SEND_INFO_STOP()
                'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                Me.Cursor = Cursors.WaitCursor

                myGlobal = MyBase.myServiceMDI.MDIAnalyzerManager.ReadFwAdjustmentsDS()
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    MyClass.StartProgress(4 * SimulationProcessTime)

                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyBase.myServiceMDI.SEND_INFO_START()

                    'MyBase.DisplaySimulationMessage("Adjustments successfully readed.")

                    MyClass.StopProgress()

                    PrepareArea()
                End If

            Else
                If Not myGlobal.HasError AndAlso MyClass.myAnalyzerManager.Connected Then
                    MyBase.myServiceMDI.SEND_INFO_STOP()
                    myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                End If
            End If


            'End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RequestAdjustFromInstrument ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestAdjustFromInstrument", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    '''' <summary>
    '''' Saves to an external file the obtained adjustments backup
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>Created by SGM 24/06/2011</remarks>
    'Private Function SaveAdjustmentsBackupFile() As GlobalDataTO

    '    Dim myGlobal As New GlobalDataTO

    '    Try
    '        'Dim myGlobalbase As New GlobalBase


    '        With Me.BackupFileDialog
    '            .Title = "Backup Adjustments"
    '            .InitialDirectory = Directory.GetDirectoryRoot(Application.StartupPath & GlobalBase.FwAdjustmentsPath)
    '            .Filter = "Text files|*.txt|All files|*.*"
    '            .CheckPathExists = True
    '            .OverwritePrompt = True
    '        End With

    '        Dim res As DialogResult = Me.BackupFileDialog.ShowDialog


    '        If res <> Windows.Forms.DialogResult.Cancel Then
    '            'export to a file
    '            Dim myPath As String = Me.BackupFileDialog.FileName
    '            myGlobal = MyBase.myAdjustmentsDelegate.ExportDSToFile(MyClass.myAnalyzerManager.ActiveAnalyzer, MyClass.myAnalyzerManager.ActiveFwVersion, myPath, True)

    '            If Not myGlobal.HasError Then
    '                MyClass.IsAlreadySaved = True
    '                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
    '            Else
    '                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.NOK, InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS.BACKUP_FILE_ERROR)
    '            End If
    '        End If


    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        MyBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAdjustmentsBackupFile ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".SaveAdjustmentsBackupFile", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try

    '    Return myGlobal

    'End Function

#End Region

#Region "Restore Adjustments"

    ''' <summary>
    ''' Requests to the Analyzer a restoring operation that can be from a new adjustments set or 
    ''' from the internal factory adjustments set
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 24/06/2011</remarks>
    Private Function RequestRestoreAction() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim AreCompatible As Boolean = True

        Try

            If MyClass.IsRestoringRequested Then
                Dim myAdjustmentsDataToRestore As String = Me.BSAdjHeaderRTextBox.Text & vbCrLf & Me.BsAdjustmentsRTextBox.Text 'SGM 03/10/2012
                'Dim myAdjustmentsDataToRestore As String = Me.BsAdjustmentsRTextBox.Text
                'validate data
                myGlobal = MyClass.ValidateAdjustmentsData(myAdjustmentsDataToRestore)

                If Not myGlobal.HasError And Not MyClass.IsRestoringToFactoryRequested Then
                    AreCompatible = MyClass.CheckAdjustmentsCompatibility()
                End If
            End If

            If Not myGlobal.HasError And AreCompatible Then
                MyBase.ActivateMDIMenusButtons(False)

                ' Configurate GUI Controls
                Me.BsAdjOpenFileButton.Enabled = False
                Me.BsAdjSaveAsButton.Enabled = False
                Me.BsAdjEditButton.Enabled = False
                Me.BsAdjUndoButton.Enabled = False
                Me.BsAdjBackupButton.Enabled = False
                Me.BsAdjRestoreButton.Enabled = False
                Me.BsAdjRestoreFactoryButton.Enabled = False
                Me.BsAdjPrintButton.Enabled = False

                ' Buttons Area
                Me.BsExitButton.Enabled = False


                If MyBase.SimulationMode Then

                    If MyClass.CurrentAdjustmentsDS IsNot Nothing Then
                        'update database
                        'save changes to the DB   SGM 20/09/2011
                        Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                        myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, MyClass.CurrentAdjustmentsDS)
                        'End SGM 20/09/2011

                        'update text file
                        MyClass.myAdjustmentsDelegate = New FwAdjustmentsDelegate(MyClass.CurrentAdjustmentsDS)
                        myGlobal = MyClass.myAdjustmentsDelegate.ExportDSToFile(MyClass.myAnalyzerManager.ActiveAnalyzer)

                    End If

                Else

                    If Not MyClass.IsRestoringToFactoryRequested Then
                        MyClass.RestoreAdjustments()
                    Else
                        MyClass.RestoreFactoryAdjustments()
                    End If

                End If

                '2-check if is inn STANDBY before sending
                If Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.STANDBY Then

                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_RESTOREADJ_MUST_STANDBY.ToString)
                    Dim res As DialogResult = MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_STANDBY_ASK.ToString)

                    'Dim res As DialogResult = MessageBox.Show("The Analyzer must be in Standby mode before restoring a set of adjustments" & _
                    '                                          vbCrLf & "Do you want to force analyzer to Standby mode?", MyBase.myServiceMDI.Text, _
                    '                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning)


                    If res = Windows.Forms.DialogResult.Yes Then
                        If MyBase.SimulationMode Then

                            MyBase.myServiceMDI.SEND_INFO_STOP()
                            MyBase.CurrentMode = ADJUSTMENT_MODES.STANDBY_DOING
                            MyClass.PrepareArea()

                            MyClass.SimulateWaiting(5000)

                            MyBase.myServiceMDI.SEND_INFO_START()
                            MyBase.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE
                            MyClass.PrepareArea()

                        Else
                            'restart info
                            If Not myGlobal.HasError AndAlso MyClass.myAnalyzerManager.Connected Then
                                MyBase.myServiceMDI.SEND_INFO_START()
                            End If

                            MyClass.StandbyAnalyzer()
                        End If

                    Else

                        MyBase.DisplayMessage(Messages.SRV_RESTOREADJ_CANCEL.ToString)
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()

                    End If


                End If

            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RequestRestoreAction ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestRestoreAction ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Restores to the Analyzer an adjustments set from an external file
    ''' </summary>
    ''' <remarks>Created by SGM 24/06/2011</remarks>
    Private Function RestoreAdjustments() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then

                MyBase.DisplayMessage(Messages.SRV_RESTORE_ADJUSTMENTS.ToString)

                Dim mydata As String = Me.BSAdjHeaderRTextBox.Text & vbCrLf & Me.BsAdjustmentsRTextBox.Text.Trim 'SGM 03/10/2012
                'Dim mydata As String = Me.BsAdjustmentsRTextBox.Text.Trim
                Dim myAnalyzerID As String = mydata.Substring(mydata.IndexOf("SN") + 2, 9)
                myGlobal = myAdjustmentsDelegate.ConvertReceivedDataToDS(mydata, "", "")
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    MyClass.CurrentAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myGlobal = myAdjustmentsDelegate.ConvertDSToString()
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            Dim myAdjToRestore As String = CStr(myGlobal.SetDatos)

                            ' XBC 29/05/2012 - correction singles
                            myAdjToRestore = myAdjToRestore.Replace(".", ",")

                            MyClass.myScreenDelegate.RestoringAdjustmentsTextAttr = myAdjToRestore

                            MyClass.ManageTabPages = False

                            'stop info
                            MyBase.myServiceMDI.SEND_INFO_STOP()

                            myGlobal = myScreenDelegate.SendRESTORE_ADJUSTMENTS()
                            If myGlobal.HasError Then
                                Throw New Exception(myGlobal.ErrorMessage)
                            End If
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RestoreAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RestoreAdjustments ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Restores to the Analyzer the internal adjustments factory set
    ''' </summary>
    ''' <remarks>Created by SGM 24/06/2011</remarks>
    Private Function RestoreFactoryAdjustments() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then

                MyBase.DisplayMessage(Messages.SRV_RESTORE_DEF_ADJUSTMENTS.ToString)

                'stop info
                MyBase.myServiceMDI.SEND_INFO_STOP()

                MyClass.myScreenDelegate.RestoringAdjustmentsTextAttr = ""
                myGlobal = myScreenDelegate.SendRESTORE_FACTORY_ADJUSTMENTS()
                If myGlobal.HasError Then
                    Throw New Exception(myGlobal.ErrorMessage)
                End If

                MyClass.ManageTabPages = False

            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RestoreFactoryAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RestoreFactoryAdjustments ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function



#End Region

#End Region

#Region "Update Firmware"

    ''' <summary>
    ''' Manages the visualization of the progress bar during Firmware Update initial process
    ''' </summary>
    ''' <remarks>Created by SGM 23/06/2012</remarks>
    Private Sub ManageFwUpdateInitialProgress()
        Try
            Me.BsWaitProgressBar.Refresh()

            Dim myValue As Integer
            Select Case myScreenDelegate.FWUpdateCurrentAction
                Case FwUpdateActions.None
                    myValue = 0
                    Me.BsWaitProgressBar.Visible = False

                Case FwUpdateActions.StartUpdate
                    myValue = CInt(10 * 100 / 100)

                Case FwUpdateActions.SendRepository
                    If myScreenDelegate.FWFileBlocks.Count > 0 Then
                        Dim BlockRate As Double = ((myScreenDelegate.FWFileSendingBlockIndex + 1) / (myScreenDelegate.FWFileBlocks.Count))
                        myValue = CInt((10 + 80 * BlockRate) * 100 / 100)
                    Else
                        myValue = CInt(40 * 100 / 100)
                    End If

                Case FwUpdateActions.QueryCRC32
                    myValue = CInt(90 * 100 / 100)

                Case FwUpdateActions.QueryNeeded
                    myValue = CInt(95 * 100 / 100)

            End Select

            Me.BsWaitProgressBar.Value = myValue
            Me.BsWaitProgressBar.Visible = (myScreenDelegate.FWUpdateCurrentAction <> FwUpdateActions.None)
            Me.BsWaitProgressBar.Refresh()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ManageFwUpdateInitialProgress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ManageFwUpdateInitialProgress ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' Attempts to start Fw Update process
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Function TryUpdateFirmware() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Me.SimulationMode Then
                myGlobal = MyClass.UpdateFirmware()
            Else
                If Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_UPDATEFW_MUST_SLEEP.ToString)
                    MyBase.DisplayMessage("")
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()
                Else
                    myGlobal = MyClass.UpdateFirmware()
                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".TryUpdateFirmware ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TryUpdateFirmware ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsFwUpdateRequested = False

        End Try
        Return myGlobal
    End Function


    ''' <summary>
    ''' Request to the Analyzer to update the Firmware
    ''' </summary>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Function UpdateFirmware() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.IsActionRequested = True
            MyClass.IsFwUpdateRequested = True

            MyBase.DisplayMessage(Messages.SRV_UPDATEFW_FIRMWARE.ToString)

            If MyBase.SimulationMode Then

                MyClass.SimulateFirmwareUpdate()

            Else
                ' XBC 19/09/2012 - When Analyzer is set to Sleep Connected attr is False so this condition no apply
                'If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                If Not myGlobal.HasError Then
                    ' XBC 19/09/2012

                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.StartUpdate 'UpdateCPU
                    MyClass.StopProgress()

                    myGlobal = myScreenDelegate.SendFWUTIL

                    MyClass.ManageFwUpdateInitialProgress()
                    MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_START")

                    MyBase.CreateLogActivity("Firmware Version " & MyClass.myScreenDelegate.FWFileHeaderVersion & " update started", Me.Name & ".PrepareFirmwareUpdatedMode ", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)

                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateFirmware ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateFirmware ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Simulates Firmware Update process
    ''' </summary>
    ''' <remarks>Created by SGM 23/06/2012</remarks>
    Private Sub SimulateFirmwareUpdate()
        Try
            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.StartUpdate
            MyClass.ManageFwUpdateInitialProgress()
            System.Threading.Thread.Sleep(SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.SendRepository
            For B As Integer = 1 To 30 Step 1
                Me.BsWaitProgressBar.Value += 1
                Me.BsWaitProgressBar.Refresh()
                System.Threading.Thread.Sleep(SimulationProcessTime)
            Next

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.QueryCRC32
            MyClass.ManageFwUpdateInitialProgress()
            System.Threading.Thread.Sleep(SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.QueryNeeded
            MyClass.ManageFwUpdateInitialProgress()
            System.Threading.Thread.Sleep(SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU
            MyClass.TimeForWait = 10 * SimulationProcessTime
            Me.BsWaitProgressBar.Value = 80
            System.Threading.Thread.Sleep(10 * SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdatePER
            MyClass.TimeForWait = 10 * SimulationProcessTime
            Me.BsWaitProgressBar.Value = 85
            System.Threading.Thread.Sleep(10 * SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateMAN
            MyClass.TimeForWait = 10 * SimulationProcessTime
            Me.BsWaitProgressBar.Value = 90
            System.Threading.Thread.Sleep(10 * SimulationProcessTime)

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateADJ
            MyClass.TimeForWait = 10 * SimulationProcessTime
            Me.BsWaitProgressBar.Value = 95
            System.Threading.Thread.Sleep(10 * SimulationProcessTime)

            MyClass.FirmwareUpdateResult = FW_GENERIC_RESULT.OK
            MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
            MyClass.PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SimulateFirmwareUpdate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulateFirmwareUpdate ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ''' <summary>
    ''' Imports from an external file a new Firmware
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Function OpenFirmwareFile() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            'Dim myGlobalbase As New GlobalBase

            With Me.OpenFwFileDialog
                .Title = Me.BsTabPagesControl.TabPages(1).Text ' "Update Firmware"
                .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                .Filter = "BAx00 Firmware files|*.BA4"
                .CheckFileExists = True
                .Multiselect = False
                .FileName = ""
            End With

            Dim res As DialogResult = Me.OpenFwFileDialog.ShowDialog

            If res <> Windows.Forms.DialogResult.Cancel Then

                Dim myFilePath As String = Me.OpenFwFileDialog.FileName

                myGlobal = myScreenDelegate.OpenFwFile(myFilePath)

                If Not myGlobal.HasError Then

                    Me.BsFileNameTextBox.Text = Me.OpenFwFileDialog.FileName
                    Me.BsFwNewVersionLabel.Text = myScreenDelegate.FWFileHeaderVersion

                Else

                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.FW_FILE_VERSION_NOT_VALID.ToString)
                    MyBase.DisplayMessage(Messages.FW_UPDATE_ERROR.ToString)

                End If

            Else

                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".OpenFirmwareFile ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".OpenFirmwareFile", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function





#End Region

#Region "Status Methods"

    ''' <summary>
    ''' Forces the Analyzer to reset
    ''' </summary>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Sub ResetAnalyzer()
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.DisplayMessage(Messages.SRV_RESET_ANALYZER.ToString)
            MyClass.ManageTabPages = False
            myGlobal = MyBase.myServiceMDI.SEND_RESET
            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ResetAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ResetAnalyzer ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Forces the Analyzer to Standby
    ''' </summary>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Sub StandbyAnalyzer()
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.DisplayMessage(Messages.SRV_STANDBY_DOING.ToString)
            MyClass.ManageTabPages = False

            'stop info
            MyBase.myServiceMDI.SEND_INFO_STOP()

            myGlobal = MyBase.myServiceMDI.SEND_STANDBY
            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StandbyAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StandbyAnalyzer ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Forces the Analyzer to Sleep
    ''' </summary>
    ''' <remarks>SGM 04/07/2011</remarks>
    Private Sub SleepAnalyzer()
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.DisplayMessage(Messages.SRV_SLEEP_DOING.ToString)
            MyClass.ManageTabPages = False

            'stop info
            MyBase.myServiceMDI.SEND_INFO_STOP()

            myGlobal = MyBase.myServiceMDI.SEND_SLEEP
            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SleepAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SleepAnalyzer ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Win32"
    <DllImport("user32.dll")> _
    Private Shared Function HideCaret(ByVal hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function ShowCaret(ByVal hWnd As IntPtr) As Boolean
    End Function

#End Region



#Region "Prepare Methods"


    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: SGM 23/06/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            'Me.BsAdjustButton.Visible = False
            'Me.BsTestButton.Visible = False
            'Me.BsCancelButton.Visible = False

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyClass.PrepareAdjustmentsReadingMode()
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                    MyClass.PrepareAdjustmentsReadedMode()

                Case ADJUSTMENT_MODES.SLEEP_DOING
                    MyClass.PrepareSleepDoingMode()

                Case ADJUSTMENT_MODES.SLEEP_DONE
                    MyClass.PrepareSleepDoneMode()

                Case ADJUSTMENT_MODES.STANDBY_DOING
                    MyClass.PrepareStandByDoingMode()

                Case ADJUSTMENT_MODES.STANDBY_DONE
                    MyClass.PrepareStandByDoneMode()

                Case ADJUSTMENT_MODES.SAVING
                    MyClass.PrepareAdjustmentsRestoringMode()

                Case ADJUSTMENT_MODES.SAVED
                    MyClass.PrepareAdjustmentsRestoredMode()

                Case ADJUSTMENT_MODES.ANALYZER_RESETING
                    MyClass.PrepareAnalyzerResetingMode()

                Case ADJUSTMENT_MODES.ANALYZER_IS_RESET
                    MyClass.PrepareAnalyzerIsResetMode()

                Case ADJUSTMENT_MODES.FW_READING
                    MyClass.PrepareFirmwareReadingMode()

                Case ADJUSTMENT_MODES.FW_READED
                    MyClass.PrepareFirmwareReadedMode()

                Case ADJUSTMENT_MODES.FW_UPDATING
                    MyClass.PrepareFirmwareUpdatingMode()

                Case ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                    MyClass.PrepareFwUtilReceivedMode()

                Case ADJUSTMENT_MODES.FW_UPDATED
                    MyClass.PrepareFirmwareUpdatedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

            'If Not MyBase.SimulationMode And Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
            '    MyClass.PrepareErrorMode()
            '    MyBase.DisplayMessage("")
            'End If

            Application.DoEvents()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loadede Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/06/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try
            ' Configurate GUI Controls
            Select Case MyClass.SelectedPage

                Case UPDATE_PAGES.ADJUSTMENTS

                    MyClass.IsRestoringRequested = False
                    MyClass.IsRestoringToFactoryRequested = False

                    Me.BsAdjOpenFileButton.Enabled = True
                    Me.BsAdjSaveAsButton.Enabled = False
                    Me.BsAdjEditButton.Enabled = False
                    Me.BsAdjUndoButton.Enabled = False

                    Me.BsAdjBackupButton.Enabled = True
                    Me.BsAdjRestoreButton.Enabled = True
                    Me.BsAdjRestoreFactoryButton.Enabled = True
                    Me.BsAdjPrintButton.Enabled = Me.BsAdjustmentsRTextBox.Text.Length > 0
                    MyBase.DisplayMessage(Messages.SRV_BACKUPRESTORE_READY.ToString)
                    Me.BsExitButton.Enabled = True

                Case UPDATE_PAGES.FIRMWARE

                    Me.BsAdjOpenFileButton.Enabled = False
                    Me.BsAdjSaveAsButton.Enabled = False
                    Me.BsAdjEditButton.Enabled = False
                    Me.BsAdjUndoButton.Enabled = False

                    MyClass.IsFwUpdateRequested = False
                    Me.BsFwOpenFileButton.Enabled = True
                    Me.BsFileNameTextBox.Enabled = True

                    If MyClass.IsFwFileReadyToSend Then
                        If Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                            MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_UPDATEFW_MUST_SLEEP.ToString)
                            MyBase.DisplayMessage("")
                        Else
                            MyBase.DisplayMessage(Messages.SRV_UPDATEFW_READY.ToString)
                            Me.BsFwUpdateButton.Enabled = True
                        End If
                    Else
                        If myScreenDelegate.FWFileHeaderVersion <> Nothing Then
                            MyBase.DisplayMessage(Messages.FW_FILE_VERSION_NOT_VALID.ToString)
                        End If
                        Me.BsFwUpdateButton.Enabled = False
                    End If


                    Me.BsExitButton.Enabled = True
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None

            End Select


            ' Buttons Area
            'Me.BsExitButton.Enabled = True

            MyClass.ManageTabPages = True

            MyClass.IsActionRequested = False

            'update MDI buttons and menus

            If myAnalyzerManager.IsFwSwCompatible Then
                MyBase.ActivateMDIMenusButtons(True, False, True)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments reading mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAdjustmentsReadingMode()
        Try
            ' Configurate GUI Controls
            Me.BsAdjNewButton.Enabled = False
            Me.BsCancelButton.Enabled = False
            Me.BsAdjOpenFileButton.Enabled = False
            Me.BsAdjSaveAsButton.Enabled = False
            Me.BsAdjBackupButton.Enabled = False
            Me.BsAdjRestoreButton.Enabled = False
            Me.BsAdjRestoreFactoryButton.Enabled = False
            Me.BsAdjPrintButton.Enabled = False

            ' Buttons Area
            Me.BsExitButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustmentsReadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustmentsReadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)

        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments restoring mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAdjustmentsRestoringMode()
        Try

            MyClass.IsActionRequested = True
            MyBase.DisplayMessage(Messages.SRV_RESTORE_ADJUSTMENTS.ToString)

            'Me.BsWaitProgressBar.Visible = True
            ''MyClass.StartWaitTimer()


            If MyBase.SimulationMode Then
                MyBase.myServiceMDI.SEND_INFO_STOP()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustmentsRestoringMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustmentsRestoringMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            'MyClass.EndWaitTimer()
            Me.BsWaitProgressBar.Visible = False
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments restored mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAdjustmentsRestoredMode()

        Dim myGlobal As New GlobalDataTO

        Try


            'MyClass.EndWaitTimer()
            Me.BsWaitProgressBar.Visible = False
            MyBase.DisplayMessage(Messages.SRV_ADJ_RESTORED.ToString)

            If MyClass.IsRestoringRequested Or MyClass.IsRestoringToFactoryRequested Then

                ' Configurate GUI Controls
                Me.BsAdjOpenFileButton.Enabled = False
                Me.BsAdjSaveAsButton.Enabled = False
                Me.BsAdjEditButton.Enabled = False
                Me.BsAdjUndoButton.Enabled = False
                Me.BsAdjBackupButton.Enabled = False
                Me.BsAdjRestoreButton.Enabled = False
                Me.BsAdjRestoreFactoryButton.Enabled = False
                Me.BsAdjPrintButton.Enabled = False
                Me.BsAdjNewButton.Enabled = False

                ' Buttons Area
                Me.BsExitButton.Enabled = False

                'Now the Application sends to the analyzer the Reset Instruction
                'Dim res As DialogResult = MessageBox.Show("Now the Analyzer must be reset. Do you want to reset right now?", MyBase.myServiceMDI.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                If Not MyClass.IsReadyToSendNext Then
                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_MUST_RESET.ToString)
                End If

                If MyBase.SimulationMode Then

                    MyBase.myServiceMDI.SEND_INFO_STOP()

                    'MyBase.DisplaySimulationMessage("Reseting the Analyzer...")

                    MyBase.CurrentMode = ADJUSTMENT_MODES.ANALYZER_RESETING
                    MyClass.PrepareArea()

                    MyClass.SimulateWaiting(4000)

                    MyBase.CurrentMode = ADJUSTMENT_MODES.ANALYZER_IS_RESET
                    MyClass.PrepareArea()

                    'MyBase.DisplaySimulationMessage("Analyzer successfully reset")
                Else

                    'PENDING TO DEFINE IF SLEEP IS NEEDED
                    MyBase.myServiceMDI.ActivateMenus(False, True)
                    MyBase.myServiceMDI.ActivateActionButtonBar(False, True)
                    MyBase.myServiceMDI.ControlBox = True
                    Me.Cursor = Cursors.Default
                    Me.BsAdjPrintButton.Enabled = True
                    Me.BsExitButton.Enabled = True
                    MyBase.CurrentMode = ADJUSTMENT_MODES._NONE

                    ''restart info
                    'MyBase.myServiceMDI.SEND_INFO_START()

                    'If Not MyClass.IsReadyToSendNext Then

                    '    MyClass.StartActionTimer()

                    'Else
                    '    MyClass.IsReadyToSendNext = False
                    '    MyClass.ResetAnalyzer()
                    'End If
                End If


            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustmentsRestoredMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustmentsRestoredMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Firmware updating mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareFirmwareUpdatingMode()
        Try

            MyClass.IsActionRequested = True
            MyBase.DisplayMessage(Messages.SRV_UPDATEFW_FIRMWARE.ToString)

            Me.BsFwOpenFileButton.Enabled = False
            Me.BsFwUpdateButton.Enabled = False
            Me.BsFileNameTextBox.Enabled = False
            Me.BsExitButton.Enabled = False

            MyBase.ActivateMDIMenusButtons(False)

            Me.BsWaitProgressBar.Visible = True

            Me.Refresh()


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFirmwareUpdatingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareFirmwareUpdatingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False
        End Try
    End Sub




    ''' <summary>
    ''' Prepare GUI for FWUTIL received mode
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 28/06/2011
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub PrepareFwUtilReceivedMode()

        Dim myGlobal As New GlobalDataTO

        Try

            'SGM 25/10/2012
            If Not MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                MyClass.StopCurrentOperation(ManagementAlarmTypes.NONE)
                Exit Sub
            End If

            'MyBase.DisplayMessage(Messages.SRV_FW_UPDATED.ToString)

            Dim myFWResponseData As FWUpdateResponseTO = MyBase.myServiceMDI.MDIAnalyzerManager.FWUpdateResponseData

            'Me.ProgressBar1.Visible = False

            If myFWResponseData IsNot Nothing AndAlso myFWResponseData.ActionType = myScreenDelegate.FWUpdateCurrentAction Then

                If myFWResponseData.ActionResult = FW_GENERIC_RESULT.OK Then

                    Select Case myFWResponseData.ActionType
                        Case FwUpdateActions.StartUpdate
                            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.SendRepository
                            myGlobal = myScreenDelegate.SendFWUTIL
                            MyClass.ManageFwUpdateInitialProgress()
                            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_SEND")

                        Case FwUpdateActions.SendRepository
                            If myScreenDelegate.FWFileSendingBlockIndex <= myScreenDelegate.FWFileBlocks.Count - 2 Then
                                myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.SendRepository
                                'MyBase.myServiceMDI.Text = myScreenDelegate.FWFileSendingBlockIndex.ToString
                            Else
                                myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.QueryCRC32
                                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            End If
                            myGlobal = myScreenDelegate.SendFWUTIL
                            MyClass.ManageFwUpdateInitialProgress()


                        Case FwUpdateActions.QueryCRC32
                            Dim myAnalyzerCRC32 As String = "0x" & myFWResponseData.FirmwareCRC.ToUpperBS.Trim    ' ToUpper.Trim
                            If myAnalyzerCRC32 <> myScreenDelegate.FWFileCRC32Hex Then
                                myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None
                                MyClass.FirmwareUpdateResult = FW_GENERIC_RESULT.KO
                                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.NOK)
                                MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
                                MyClass.PrepareArea()
                            Else
                                MyClass.NeededUpdatesResponse = Nothing
                                myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.QueryNeeded
                                myGlobal = myScreenDelegate.SendFWUTIL
                                MyClass.ManageFwUpdateInitialProgress()
                                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                                MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_QUERY")
                            End If


                        Case FwUpdateActions.QueryNeeded
                            MyClass.NeededUpdatesResponse = myFWResponseData
                            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            myGlobal = MyClass.UpdateFirmwareItems

                        Case FwUpdateActions.UpdateCPU
                            If myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU Then
                                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                                MyClass.NeededUpdatesResponse.IsUpdatedCPU = FW_GENERIC_RESULT.OK
                                myGlobal = MyClass.UpdateFirmwareItems
                            Else
                                'discard the second response
                                Dim a As Integer = 0
                            End If

                        Case FwUpdateActions.UpdatePER
                            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            MyClass.NeededUpdatesResponse.IsUpdatedPER = FW_GENERIC_RESULT.OK
                            myGlobal = MyClass.UpdateFirmwareItems

                        Case FwUpdateActions.UpdateMAN
                            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            MyClass.NeededUpdatesResponse.IsUpdatedMAN = FW_GENERIC_RESULT.OK
                            myGlobal = MyClass.UpdateFirmwareItems


                        Case FwUpdateActions.UpdateADJ
                            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None
                            MyClass.FirmwareUpdateResult = FW_GENERIC_RESULT.OK
                            MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
                            MyClass.PrepareArea()

                    End Select

                Else
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None
                    MyClass.FirmwareUpdateResult = FW_GENERIC_RESULT.KO
                    MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
                    MyClass.PrepareArea()
                End If

            End If

            If Not myGlobal.HasError Then

            Else
                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareErrorMode()
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            'Dim s As String = ex.StackTrace
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFwUtilReceivedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareFwUtilReceivedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Performs the Update process of the FW elements that have to be updated
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 26/06/2012</remarks>
    Private Function UpdateFirmwareItems() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.StopProgress(False)

            If MyClass.NeededUpdatesResponse IsNot Nothing Then
                If MyClass.NeededUpdatesResponse.IsUpdatedCPU = FW_GENERIC_RESULT.KO Then
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU
                    MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_CPU")
                    Me.BsWaitProgressBar.Visible = True

                ElseIf MyClass.NeededUpdatesResponse.IsUpdatedPER = FW_GENERIC_RESULT.KO Then
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdatePER
                    MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_PER")
                    Me.BsWaitProgressBar.Visible = True

                ElseIf MyClass.NeededUpdatesResponse.IsUpdatedMAN = FW_GENERIC_RESULT.KO Then
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateMAN
                    MyBase.DisplayLabel("Working", "LBL_SRV_FW_UPDATE_MAN")
                    Me.BsWaitProgressBar.Visible = True

                Else
                    myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None
                    MyClass.StopProgress()
                    MyClass.FirmwareUpdateResult = FW_GENERIC_RESULT.OK
                    MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
                    MyClass.PrepareArea()
                End If
                If myScreenDelegate.FWUpdateCurrentAction <> FwUpdateActions.None Then
                    myGlobal = myScreenDelegate.SendFWUTIL
                End If
            Else
                myGlobal.HasError = True
            End If
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            'Dim s As String = ex.StackTrace
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateFirmwareItems ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateFirmwareItems ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Sub PrepareFirmwareUpdatedMode()
        Try

            MyClass.IsActionRequested = False

            Select Case MyClass.FirmwareUpdateResult
                Case FW_GENERIC_RESULT.KO
                    MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.FW_UPDATE_ERROR.ToString)
                    MyBase.DisplayMessage(Messages.FW_UPDATE_ERROR.ToString)
                    MyBase.CreateLogActivity("Firmware Version " & MyClass.myScreenDelegate.FWFileHeaderVersion & " is NOT updated", Me.Name & ".PrepareFirmwareUpdatedMode ", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)

                Case FW_GENERIC_RESULT.OK
                    Me.BsFwCurrentVersionLabel.Text = myScreenDelegate.FWFileHeaderVersion
                    MyBase.DisplayMessage(Messages.SRV_FW_UPDATED.ToString)

                    MyBase.CreateLogActivity("Firmware Version " & MyClass.myScreenDelegate.FWFileHeaderVersion & " updated Ok", Me.Name & ".PrepareFirmwareUpdatedMode ", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)

                    myAnalyzerManager.IsFwSwCompatible = True
                    MyBase.ActivateMDIMenusButtons(True, False, True)


            End Select

            ' XBC 18/09/2012 - add Update Fw functionality flag 
            MyBase.myServiceMDI.UpdateFirmwareProcessEnd = True

            Me.BsWaitProgressBar.Visible = False

            MyClass.IsFwUpdateRequested = False
            Me.BsFileNameTextBox.Enabled = True

            Me.BsFwOpenFileButton.Enabled = True
            Me.BsFwUpdateButton.Enabled = True
            Me.BsExitButton.Enabled = True

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None

            Me.Refresh()



        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFirmwareUpdatedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareFirmwareUpdatedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Standby doing mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareStandByDoingMode()

        Dim myGlobal As New GlobalDataTO

        Try

            MyClass.IsActionRequested = True
            MyBase.DisplayMessage(Messages.SRV_STANDBY_DOING.ToString)

            If Not MyBase.SimulationMode Then

                Me.BsWaitProgressBar.Visible = True
            Else
                MyBase.myServiceMDI.SEND_INFO_STOP()
            End If

            ' Configurate GUI Controls
            Me.BsAdjOpenFileButton.Enabled = False
            Me.BsAdjSaveAsButton.Enabled = False
            Me.BsAdjEditButton.Enabled = False
            Me.BsAdjUndoButton.Enabled = False
            Me.BsAdjBackupButton.Enabled = False
            Me.BsAdjRestoreButton.Enabled = False
            Me.BsAdjRestoreFactoryButton.Enabled = False
            Me.BsAdjPrintButton.Enabled = False

            ' Buttons Area
            Me.BsExitButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStandByDoingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStandByDoingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Standby done mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareStandByDoneMode()

        Dim myGlobal As New GlobalDataTO

        Try
            Me.BsWaitProgressBar.Visible = False

            MyClass.IsActionRequested = False

            MyBase.DisplayMessage(Messages.SRV_STANDBY_DONE.ToString)


            If MyClass.IsRestoringRequested Or MyClass.IsRestoringToFactoryRequested Then

                MyBase.ActivateMDIMenusButtons(False)

                ' Configurate GUI Controls
                Me.BsAdjOpenFileButton.Enabled = False
                Me.BsAdjSaveAsButton.Enabled = False
                Me.BsAdjEditButton.Enabled = False
                Me.BsAdjUndoButton.Enabled = False
                Me.BsAdjBackupButton.Enabled = False
                Me.BsAdjRestoreButton.Enabled = False
                Me.BsAdjRestoreFactoryButton.Enabled = False
                Me.BsAdjPrintButton.Enabled = False

                ' Buttons Area
                Me.BsExitButton.Enabled = False

                If MyBase.SimulationMode Then

                    MyBase.myServiceMDI.SEND_INFO_START()

                    If MyClass.IsRestoringRequested Then
                        MyBase.DisplayMessage(Messages.SRV_RESTORE_ADJUSTMENTS.ToString)
                    ElseIf MyClass.IsRestoringToFactoryRequested Then
                        MyBase.DisplayMessage(Messages.SRV_RESTORE_DEF_ADJUSTMENTS.ToString)
                    End If

                    'MyBase.DisplaySimulationMessage("Restoring adjustments...")

                    MyClass.SimulateWaiting(3000)

                    MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                    MyClass.PrepareArea()

                    'MyBase.DisplaySimulationMessage("Analyzer successfully restored")
                Else


                    If Not MyClass.IsReadyToSendNext Then
                        MyClass.StartActionTimer()
                    Else
                        MyClass.IsReadyToSendNext = False
                        If MyClass.IsRestoringRequested Then
                            myGlobal = MyClass.RestoreAdjustments()
                        ElseIf MyClass.IsRestoringToFactoryRequested Then
                            myGlobal = MyClass.RestoreFactoryAdjustments()
                        End If
                    End If
                End If


            Else

                MyClass.AdjustmentsEditionMode = False

                ' Configurate GUI Controls
                Me.BsAdjOpenFileButton.Enabled = True
                Me.BsAdjSaveAsButton.Enabled = True
                Me.BsAdjBackupButton.Enabled = True
                Me.BsAdjRestoreButton.Enabled = True
                Me.BsAdjRestoreFactoryButton.Enabled = True
                Me.BsAdjPrintButton.Enabled = (Me.BsAdjustmentsRTextBox.Text.Length > 0)

                ' Buttons Area
                Me.BsExitButton.Enabled = Not MyClass.IsOnlyFirmwareUpdate

                MyClass.IsActionRequested = False
            End If

            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStandByDoneMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareStandByDoneMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsRestoringRequested = False
        End Try
    End Sub

    '''' <summary>
    '''' Prepare GUI for Sleep doing mode
    '''' </summary>
    '''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareSleepDoingMode()

        'Dim myGlobal As New GlobalDataTO

        'Try

        '    MyClass.IsActionRequested = True

        '    MyBase.DisplayMessage(Messages.SRV_SLEEP_DOING.ToString)

        '    If Not MyBase.SimulationMode Then
        '        Me.BsWaitProgressBar.Visible = True
        '    Else
        '        MyBase.myServiceMDI.SEND_INFO_STOP()
        '    End If

        '    ' Configurate GUI Controls
        '    Me.BsFwOpenFileButton.Enabled = False
        '    Me.BsFwUpdateButton.Enabled = False

        '    ' Buttons Area
        '    Me.BsExitButton.Enabled = False

        'Catch ex As Exception
        '    MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSleepDoingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    MyBase.ShowMessage(Me.Name & ".PrepareSleepDoingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        '    Me.BsWaitProgressBar.Visible = False
        'End Try
    End Sub

    '''' <summary>
    '''' Prepare GUI for Sleep done mode
    '''' </summary>
    '''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareSleepDoneMode()

        'Dim myGlobal As New GlobalDataTO

        'Try
        '    Me.BsWaitProgressBar.Visible = False

        '    MyClass.IsActionRequested = False



        '    MyBase.DisplayMessage(Messages.SRV_SLEEP_DONE.ToString)


        '    If MyClass.IsFwUpdateRequested Then

        '        ' Configurate GUI Controls
        '        Me.BsFwOpenFileButton.Enabled = False
        '        Me.BsFwUpdateButton.Enabled = False

        '        ' Buttons Area
        '        Me.BsExitButton.Enabled = False

        '        If MyBase.SimulationMode Then

        '            MyBase.myServiceMDI.SEND_INFO_STOP()

        '            If MyClass.IsFwUpdateRequested Then
        '                MyBase.DisplayMessage(Messages.SRV_UPDATEFW_FIRMWARE.ToString)
        '            End If

        '            'MyBase.DisplaySimulationMessage("Updating firmware...")

        '            MyClass.SimulateWaiting(3000)

        '            MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UPDATED
        '            MyClass.PrepareArea()

        '            'MyBase.DisplaySimulationMessage("Firmware successfully updated")
        '        Else
        '            If Not MyClass.IsReadyToSendNext Then
        '                MyClass.StartActionTimer()
        '            Else
        '                MyClass.IsReadyToSendNext = False
        '                If MyClass.IsFwUpdateRequested Then
        '                    myGlobal = MyClass.UpdateFirmware()
        '                End If
        '            End If
        '        End If

        '    Else
        '        ' Configurate GUI Controls
        '        Me.BsFwOpenFileButton.Enabled = True
        '        Me.BsFwUpdateButton.Enabled = MyClass.IsFwFileReadyToSend

        '        ' Buttons Area
        '        Me.BsExitButton.Enabled = Not MyClass.IsOnlyFirmwareUpdate

        '        MyClass.IsActionRequested = False
        '    End If

        '    If myGlobal.HasError Then
        '        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
        '        MyClass.PrepareArea()
        '    End If

        'Catch ex As Exception
        '    MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSleepDoneMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    MyBase.ShowMessage(Me.Name & ".PrepareSleepDoneMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        '    MyClass.IsFwUpdateRequested = False
        'End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Analyzer reseting mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAnalyzerResetingMode()
        Try

            MyClass.IsActionRequested = True

            MyBase.DisplayMessage(Messages.SRV_RESET_ANALYZER.ToString)


            If Not MyBase.SimulationMode Then
                Me.BsWaitProgressBar.Visible = True
            Else
                MyBase.myServiceMDI.SEND_INFO_STOP()
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAnalyzerResetingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAnalyzerResetingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Analyzer is reset mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAnalyzerIsResetMode()

        Dim myGlobal As New GlobalDataTO

        Try
            Me.BsWaitProgressBar.Visible = False

            MyBase.DisplayMessage(Messages.SRV_ANALYZER_IS_RESET.ToString)


            If MyClass.IsRestoringRequested Or MyClass.IsRestoringToFactoryRequested Then
                myGlobal = MyClass.RequestAdjustFromInstrument() 'read again the restored adjustments

            Else
                MyClass.IsRestoringRequested = False
                MyClass.IsRestoringToFactoryRequested = False
                MyClass.IsActionRequested = False
                MyClass.EnableButtons()
            End If

            MyBase.myServiceMDI.SEND_INFO_START()




        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAnalyzerIsResetMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAnalyzerIsResetMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments readed mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareAdjustmentsReadedMode()

        Dim myGlobal As New GlobalDataTO

        Try
            'restart info

            MyBase.myServiceMDI.SEND_INFO_START()

            myGlobal = MyBase.myServiceMDI.MDIAnalyzerManager.ReadFwAdjustmentsDS()
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                MyClass.CurrentAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                With MyClass.CurrentAdjustmentsDS
                    '.AnalyzerModel = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveAnalyzer
                    '.FirmwareVersion = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveFwVersion
                    If MyBase.SimulationMode Then
                        .ReadedDatetime = DateTime.Now
                    End If
                End With

                'MyClass.CurrentAdjustmentsHeader = New AdjustmentsHeader
                'With MyClass.CurrentAdjustmentsHeader
                '    .AnalyzerModel = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveAnalyzer
                '    .FirmwareVersion = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveFwVersion
                'End With


                Dim myFwAdjustmentsDelegate As New FwAdjustmentsDelegate(MyClass.CurrentAdjustmentsDS)
                myGlobal = myFwAdjustmentsDelegate.ConvertDSToString()
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myAdjText As String = CStr(myGlobal.SetDatos)
                    Dim myAdjHeader As String = myFwAdjustmentsDelegate.MakeAdjustmentsFileHeader()

                    If myAdjHeader.Length > 0 And myAdjText.Length > 0 Then
                        Me.BSAdjHeaderRTextBox.Text = myAdjHeader.Trim 'SGM 03/10/2012
                        Me.BsAdjustmentsRTextBox.Text = myAdjText.Trim 'SGM 03/10/2012
                        'Me.BsAdjustmentsRTextBox.Text = myAdjText.Trim
                        MyClass.InitialAdjustmentsText = myAdjText.Trim 'SGM 03/10/2012
                        'MyClass.InitialAdjustmentsText = Me.BsAdjustmentsRTextBox.Text
                    End If

                    MyClass.AdjustmentsEditionMode = False

                    Me.BsAdjOpenFileButton.Enabled = True
                    Me.BsAdjBackupButton.Enabled = True
                    Me.BsAdjSaveAsButton.Enabled = True
                    Me.BsAdjRestoreButton.Enabled = True
                    Me.BsAdjRestoreFactoryButton.Enabled = True
                    Me.BsAdjPrintButton.Enabled = True

                End If

                If Not MyClass.IsRestoringRequested And Not MyClass.IsRestoringToFactoryRequested Then
                    '3- want to save?
                    'Dim res As DialogResult = MessageBox.Show("Do you want to save the readed adjustments to a backup file?", MyBase.myServiceMDI.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    Dim res As DialogResult = MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_BACKUPADJ_SAVE_ASK.ToString)
                    If res = Windows.Forms.DialogResult.Yes Then
                        '3.1- save
                        myGlobal = MyClass.SaveAdjustmentsFileAs
                        MyBase.DisplayMessage(Messages.SRV_ADJ_BACKUP_DONE.ToString)
                        MyBase.ActivateMDIMenusButtons(True)
                    End If


                Else
                    If MyClass.IsRestoringRequested Then
                        MyBase.DisplayMessage(Messages.SRV_ADJ_RESTORED.ToString)
                        MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                        MyClass.IsRestoringRequested = False
                        MyClass.EnableButtons()
                    End If

                    If MyClass.IsRestoringToFactoryRequested Then
                        MyBase.DisplayMessage(Messages.SRV_FACTORYADJ_RESTORED.ToString)
                        MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                        MyClass.IsRestoringToFactoryRequested = False
                        MyClass.EnableButtons()
                    End If

                    MyClass.IsActionRequested = False

                End If

            End If

            MyClass.EnableButtons()

            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustmentsReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustmentsReadedMode ", Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Firmware reading mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareFirmwareReadingMode()
        Try
            ' Configurate GUI Controls
            Me.BsFwOpenFileButton.Enabled = False
            Me.BsFwUpdateButton.Enabled = False

            ' Buttons Area
            Me.BsExitButton.Enabled = False

            MyClass.IsCurrentFwRequested = True


            MyBase.DisplayMessage(Messages.SRV_READ_FIRMWARE.ToString)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFirmwareReadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareFirmwareReadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Firmware readed mode
    ''' </summary>
    ''' <remarks>Created by SGM 28/06/2011</remarks>
    Private Sub PrepareFirmwareReadedMode()

        'Dim myGlobal As New GlobalDataTO

        'Try

        '    myGlobal = MyClass.ExtractFirmwareVersionFromFile()

        '    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

        '        MyClass.AnalyzerFirmwareVersion = CStr(myGlobal.SetDatos)


        '        MyClass.IsCurrentFwRequested = False

        '        ' Configurate GUI Controls
        '        Me.BsFwOpenFileButton.Enabled = True
        '        Me.BsFwUpdateButton.Enabled = False


        '        ' Buttons Area
        '        Me.BsExitButton.Enabled = Not MyClass.IsOnlyFirmwareUpdate

        '        MyBase.myServiceMDI.SEND_INFO_START()

        '        If MyClass.IsFwUpdateRequested Then
        '            MyBase.DisplayMessage(Messages.SRV_FW_UPDATED.ToString)
        '        Else
        '            MyBase.DisplayMessage(Messages.SRV_FW_READED.ToString)
        '        End If


        '        MyClass.IsActionRequested = False
        '        MyClass.EnableButtons()
        '    Else
        '        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
        '        MyClass.PrepareArea()
        '    End If



        'Catch ex As Exception
        '    myGlobal.HasError = True
        '    myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '    myGlobal.ErrorMessage = ex.Message
        '    MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFirmwareReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    MyBase.ShowMessage(Me.Name & ".PrepareFirmwareReadedMode ", Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        'End Try
    End Sub

  

#End Region

#Region "Must Inherited"

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by SGM 21/06/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try

            Me.BsWaitProgressBar.Visible = False

            'If MyBase.myFwScriptDelegate.IsWaitingForResponse Then
            MyBase.ErrorMode()
            MyClass.AdjustmentsEditionMode = False
            Me.BsAdjOpenFileButton.Enabled = False
            Me.BsAdjSaveAsButton.Enabled = False
            Me.BsAdjBackupButton.Enabled = False
            Me.BsAdjRestoreButton.Enabled = False
            Me.BsAdjRestoreFactoryButton.Enabled = False
            Me.BsAdjPrintButton.Enabled = False

            'End If

            ' Buttons Area
            Me.BsExitButton.Enabled = Not MyClass.IsOnlyFirmwareUpdate ' Just Exit button is enabled in error case

            'SGM 21/10/2012
            'MyClass.IsActionRequested = False

            'myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None

            'If MyClass.IsFwUpdateRequested Then
            '    'MyBase.ShowMessage(Messages.FW_UPDATE_ERROR.ToString)
            '    MyBase.DisplayMessage(Messages.FW_UPDATE_ERROR.ToString)
            '    MyClass.IsFwUpdateRequested = False
            '    Me.BsExitButton.Enabled = True
            'End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'SGM 21/10/2012
            MyClass.PrepareErrorMode(pAlarmType)

            MyClass.IsActionRequested = False

            myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.None

            If MyClass.IsFwUpdateRequested Then
                'MyBase.ShowMessage(Messages.FW_UPDATE_ERROR.ToString)
                MyBase.DisplayMessage(Messages.FW_UPDATE_ERROR.ToString)
                MyClass.IsFwUpdateRequested = False
                Me.BsExitButton.Enabled = True
            End If
            'end SGM 21/10/2012

            'when stop action is finished, perform final operations after alarm received
            If pAlarmType <> ManagementAlarmTypes.NONE Then
                MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)
                Me.BsAdjOpenFileButton.Enabled = True
            End If

            Me.BsWaitProgressBar.Visible = False
            Me.Cursor = Cursors.Default

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "subsequent sendings timing"

    Private Function StartActionTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyBase.SimulationMode Then

            Else

                MyClass.ActionTimerCallBack = New System.Threading.TimerCallback(AddressOf OnActionTimerTick)
                MyClass.ActionTimer = New System.Threading.Timer(MyClass.ActionTimerCallBack, New Object, 400, 0)

            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & " StartActionTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function



    Private Sub OnActionTimerTick(ByVal stateInfo As Object)

        Dim myGlobal As New GlobalDataTO

        Try
            If MyBase.SimulationMode Then


            Else
                MyClass.IsReadyToSendNext = True
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & " OnActionTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


#End Region


#Region "wait timing"

    'Private StartTime As TimeSpan
    'Private ExpectedEndTime As TimeSpan
    'Private ElapsedTime As Integer


    'Private Function StartWaitTimer() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try

    '        MyClass.StartTime = New TimeSpan(Now.Ticks)
    '        MyClass.ExpectedEndTime = MyClass.StartTime.Add(New TimeSpan(0, 0, 0, 0, MyClass.TimeForWait))

    '        Me.WaitingProgressBar.Maximum = MyClass.TimeForWait

    '        MyClass.WaitTimerCallBack = New System.Threading.TimerCallback(AddressOf OnWaitTimerTick)
    '        MyClass.WaitTimer = New System.Threading.Timer(MyClass.WaitTimerCallBack, New Object, 10, 10)

    '        Me.WaitingProgressBar.Visible = True

    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        mybase.CreateLogActivity(ex.Message, Me.Name & " StartWaitTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return myGlobal
    'End Function



    'Private Sub OnWaitTimerTick(ByVal stateInfo As Object)

    '    Dim myGlobal As New GlobalDataTO

    '    Try

    '        Dim NowTime As New TimeSpan(Now.Ticks)
    '        MyClass.ElapsedTime = CInt(NowTime.TotalMilliseconds - MyClass.StartTime.TotalMilliseconds)

    '        If (MyClass.ElapsedTime <= Me.WaitingProgressBar.Maximum) Then
    '            Me.WaitingProgressBar.Value = ElapsedTime
    '            Me.WaitingProgressBar.Refresh()
    '        End If

    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        mybase.CreateLogActivity(ex.Message, Me.Name & " OnWaitTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub

    'Private Function EndWaitTimer() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try

    '        Me.WaitingProgressBar.Visible = False
    '        Me.WaitingProgressBar.Value = 0

    '        If MyClass.WaitTimer IsNot Nothing Then
    '            MyClass.WaitTimer.Dispose()
    '            MyClass.WaitTimer = Nothing
    '        End If

    '        If MyClass.WaitTimerCallBack IsNot Nothing Then
    '            MyClass.WaitTimerCallBack = Nothing
    '        End If

    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        mybase.CreateLogActivity(ex.Message, Me.Name & " EndWaitTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return myGlobal
    'End Function

#End Region

#Region "simulation timing"

    ''' <summary>
    ''' Manages the behaviour of the waiting progress bar while simulation mode
    ''' </summary>
    ''' <param name="pTime"></param>
    ''' <remarks>SGM 01/07/2011</remarks>
    Private Sub SimulateWaiting(ByVal pTime As Integer)
        Try
            Me.BsWaitProgressBar.Visible = True

            For i As Integer = 1 To pTime Step 100
                Application.DoEvents()
                System.Threading.Thread.Sleep(100)
            Next

            Me.BsWaitProgressBar.Visible = False

            'Me.WaitingProgressBar.Maximum = pTime
            'Me.WaitingProgressBar.Visible = True
            'For m As Integer = 0 To pTime Step 10
            '    System.Threading.Thread.Sleep(1)
            '    If m <= Me.WaitingProgressBar.Maximum Then
            '        Me.WaitingProgressBar.Value = m
            '    Else
            '        Exit Sub
            '    End If
            '    Me.WaitingProgressBar.Refresh()
            'Next
            'Me.WaitingProgressBar.Visible = False
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SimulateWaiting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulateWaiting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Me.BsWaitProgressBar.Visible = False

        End Try
    End Sub




#End Region

#End Region

#Region "Event Handling"

#Region "Common"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IInstrumentUpdateUtil_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Dim myGlobal As New GlobalDataTO

        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else

                If MyClass.AdjustmentsEditionMode Then
                    myGlobal = MyClass.WarnIfAdjustmentsEditionChanges
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        If CBool(myGlobal.SetDatos) Then
                            MyClass.IsFwUpdateRequested = False
                            MyClass.myScreenDelegate.Dispose()
                            MyClass.myScreenDelegate = Nothing
                            Me.Dispose()

                            MyBase.ActivateMDIMenusButtons(True)
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IInstrumentUpdateUtil_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'user permissions
            MyBase.GetUserNumericalLevel()
            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                Me.BsAdjRestoreButton.Enabled = False
                Me.BsAdjRestoreFactoryButton.Enabled = False
                Me.BsFwOpenFileButton.Enabled = False
                Me.BsFwUpdateButton.Enabled = False
            End If

            'allow editing in case of privileged user (BIOSYSTEMS)
            MyClass.EditionAllowed = (MyBase.CurrentUserNumericalLevel = USER_LEVEL.lBIOSYSTEMS)

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            MyClass.GetScreenLabels()
            MyClass.GetScreenTooltip()
            MyClass.PrepareButtons()

            'Information
            MyClass.SelectedInfoPanel = Me.BsAdjInfoPanel
            MyClass.SelectedAdjPanel = Me.BsAdjustmentsPanel
            Me.BsInfoAdjXPSViewer.FitToPageHeight()

            MyBase.DisplayInformation(APPLICATION_PAGES.UPDATE_ADJ, Me.BsInfoAdjXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.UPDATE_FW, Me.BsInfoFwXPSViewer)

            'Screen Delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New InstrumentUpdateUtilDelegate(MyBase.myServiceMDI.ActiveAnalyzer, MyBase.myFwScriptDelegate)


            ' Check communications with Instrument
            If Not Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
                ManageTabPages = False
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()

            End If

            If Not MyClass.IsOnlyFirmwareUpdate Then
                'get Adjustments MasterData
                MyClass.LoadAdjustmentsMasterData()

                If myGlobal.HasError Then
                    PrepareErrorMode()
                    MyBase.ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                End If
            End If

            Select Case MyClass.SelectedPage
                Case UPDATE_PAGES.ADJUSTMENTS
                    Me.BsTabPagesControl.SelectedTab = BsAdjustmentsTab
                Case UPDATE_PAGES.FIRMWARE
                    Me.BsTabPagesControl.SelectedTab = BsFirmwareTab
            End Select


            Me.BsAdjustmentsTextPanel.Enabled = True
            Me.BsAdjustmentsRTextBox.ReadOnly = False

            ResetBorderSRV()

            ''disable for PROTO 4
            ''BsAdjBackupButton.Enabled = False
            'BsAdjRestoreButton.Enabled = False
            'BsAdjRestoreFactoryButton.Enabled = False
            'BsAdjPrintButton.Enabled = False
            'BsAdjNewButton.Enabled = False
            'BsAdjEditButton.Enabled = False
            'BsAdjUndoButton.Enabled = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles BsTabPagesControl.Selected
        Try

            If e.TabPage Is BsAdjustmentsTab Then
                MyClass.SelectedInfoPanel = Me.BsAdjInfoPanel
                MyClass.SelectedAdjPanel = Me.BsAdjustmentsPanel

            ElseIf e.TabPage Is BsFirmwareTab Then
                MyClass.SelectedInfoPanel = Me.BsFwInfoPanel
                MyClass.SelectedAdjPanel = Me.BsFirmwarePanel

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Deselecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Deselecting

        Dim myGlobal As New GlobalDataTO

        Try

            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            If e.TabPage Is BsAdjustmentsTab Then
                myGlobal = MyClass.WarnIfAdjustmentsEditionChanges
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    If CBool(myGlobal.SetDatos) Then
                        Me.BSAdjHeaderRTextBox.Text = "" 'SGM 03/10/2012
                        Me.BsAdjustmentsRTextBox.Text = ""
                        MyClass.InitialAdjustmentsText = ""
                    Else
                        e.Cancel = True
                        Exit Sub
                    End If
                End If
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Deselecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Deselecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub
    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Selecting

        Dim myGlobal As New GlobalDataTO

        Try

            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            ''disable for PROTO 4
            'If e.TabPage Is Me.BsFirmwareTab Then
            '    e.Cancel = True
            '    Exit Sub
            'End If

            If MyClass.IsOnlyFirmwareUpdate Then
                If e.TabPage Is BsAdjustmentsTab Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabControl_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsTabPagesControl.SelectedIndexChanged
        Try

            Dim myResultData As New GlobalDataTO
            If BsTabPagesControl.SelectedTab Is BsAdjustmentsTab Then
                Me.SelectedPage = UPDATE_PAGES.ADJUSTMENTS
                PrepareLoadedMode()
                DefineScreenLayout(Me.SelectedPage)

            ElseIf BsTabPagesControl.SelectedTab Is BsFirmwareTab Then
                Me.SelectedPage = UPDATE_PAGES.FIRMWARE
                DefineScreenLayout(Me.SelectedPage)

                Me.Refresh()

                Application.DoEvents()

                If MyBase.myServiceMDI.MDIAnalyzerManager.ReadedFwVersion.Length > 0 Then
                    Me.BsFwCurrentVersionLabel.Text = MyBase.myServiceMDI.MDIAnalyzerManager.ReadedFwVersion
                Else
                    Me.BsFwCurrentVersionLabel.Text = "_.__"
                End If

                Me.BsFwNewVersionLabel.Text = ""

                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsTabPagesControl_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsTabPagesControl_SelectedIndexChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    Private Sub BsAdjustmentsRTextBox_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsAdjustmentsRTextBox.GotFocus
        Try
            If Not MyClass.AdjustmentsEditionMode Then
                HideCaret(Me.BsAdjustmentsRTextBox.Handle)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAdjustmentsRTextBox_GotFocus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAdjustmentsRTextBox_GotFocus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    Private Sub BsAdjustmentsRTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjustmentsRTextBox.TextChanged
        Try

            Me.BsAdjPrintButton.Enabled = Me.BsAdjustmentsRTextBox.Text.Length > 0
            Me.BsAdjSaveAsButton.Enabled = Me.BsAdjustmentsRTextBox.Text.Length > 0

            If MyClass.AdjustmentsEditionMode Then
                MyClass.AdjustmentsEditionChangesMade = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAdjustmentsRTextBox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAdjustmentsRTextBox_TextChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    Private Sub BsAdjustmentsRTextBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsAdjustmentsRTextBox.MouseDown
        Try
            If Not MyClass.AdjustmentsEditionMode Then
                HideCaret(Me.BsAdjustmentsRTextBox.Handle)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAdjustmentsRTextBox_MouseDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAdjustmentsRTextBox_MouseDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsAdjustmentsRTextBox_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsAdjustmentsRTextBox.MouseUp
        Try
            If Not MyClass.AdjustmentsEditionMode Then
                HideCaret(Me.BsAdjustmentsRTextBox.Handle)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAdjustmentsRTextBox_MouseDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAdjustmentsRTextBox_MouseDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsAdjustmentsRTextBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles BsAdjustmentsRTextBox.KeyPress
        Try
            If Not MyClass.AdjustmentsEditionMode Then
                e.Handled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAdjustmentsRTextBox_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAdjustmentsRTextBox_KeyPress ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Adjustments"


    Private Sub BsAdjOpenFileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjOpenFileButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            MyBase.DisplayMessage("")

            myGlobal = MyClass.OpenAdjustmentsFile

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                MyBase.ActivateMDIMenusButtons(True)
            Else
                MyClass.PrepareErrorMode()
            End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsAdjOpenFileButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsAdjOpenFileButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try
    End Sub

    Private Sub BsAdjSaveAsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjSaveAsButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            MyBase.DisplayMessage("")

            myGlobal = MyClass.SaveAdjustmentsFileAs()

            If Not myGlobal.HasError Then
                MyClass.IsAlreadySaved = True
                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.OK)
                MyBase.DisplayMessage(Messages.SRV_ADJ_FILE_SAVED.ToString)
                MyClass.AdjustmentsEditionMode = False
                MyBase.ActivateMDIMenusButtons(True)
            Else
                MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS.NOK, InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS.RESTORE_ADJFILE_ERROR)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsAdjSaveAsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsAdjSaveAsButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try

    End Sub

    Private Sub BsAdjEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjEditButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.DisplayMessage("")
            MyClass.AdjustmentsEditionMode = True
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsAdjEditButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsAdjEditButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try
    End Sub

    Private Sub BsBackupButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjBackupButton.Click

        Dim myGlobal As New GlobalDataTO

        Try

            MyBase.DisplayMessage("")

            myGlobal = MyClass.WarnIfAdjustmentsEditionChanges

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                If CBool(myGlobal.SetDatos) Then
                    MyClass.IsAlreadySaved = False

                    'request from analyzer
                    MyClass.AdjustmentsEditionMode = False
                    myGlobal = MyClass.RequestAdjustFromInstrument
                    If myGlobal.HasError Then
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                        MyClass.PrepareArea()
                    Else
                        MyBase.ActivateMDIMenusButtons(True)
                    End If
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsBackupButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsBackupButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try
    End Sub

    Private Sub BsRestoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjRestoreButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            MyBase.DisplayMessage("")

            myGlobal = MyClass.WarnIfAdjustmentsEditionChanges
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                'check if it is empty
                Dim myAdjustmentsDataToRestore As String = Me.BsAdjustmentsRTextBox.Text

                If myAdjustmentsDataToRestore.Length = 0 Then
                    Dim res As DialogResult = MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_RESTORE_ADJ_OPEN_ASK.ToString)
                    'Dim res As DialogResult = MessageBox.Show("There is no Adjustments data to restore." & vbCrLf & "Do you want to get from a saved file?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation)
                    Select Case res
                        Case Windows.Forms.DialogResult.Yes
                            myGlobal = MyClass.OpenAdjustmentsFile

                        Case Windows.Forms.DialogResult.No
                            Exit Sub
                        Case Windows.Forms.DialogResult.Cancel
                            Exit Sub
                    End Select
                Else
                    myGlobal.SetDatos = Windows.Forms.DialogResult.OK
                End If

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    If CType(myGlobal.SetDatos, DialogResult) = Windows.Forms.DialogResult.OK Then
                        MyClass.IsRestoringRequested = True
                        myGlobal = MyClass.RequestRestoreAction()

                        If myGlobal.HasError Then
                            MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                            MyClass.PrepareArea()
                        Else
                            MyBase.ActivateMDIMenusButtons(True)
                        End If
                    Else
                        MyBase.ActivateMDIMenusButtons(True)
                    End If
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsRestoreButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsRestoreButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            MyClass.IsRestoringRequested = False
        End Try
    End Sub


    Private Sub BsRestoreFactoryButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjRestoreFactoryButton.Click


        ''Factory Adjustments - No V1
        ''--------------------------------
        ''Se añadirá una nueva funcionalidad mediante la cual un usuario BIO encriptará el fichero de ajustes proporcionado por Electrónica y 
        ''este se incluirá en el dvd de distribución. De esta manera el botón de Factory cargará automáticamente este fichero mediante LOADADJ


        'Dim myGlobal As New GlobalDataTO

        'Try
        '    MyBase.DisplayMessage("")

        '    MyClass.IsRestoringToFactoryRequested = True

        '    myGlobal = MyClass.RequestRestoreAction()

        '    If myGlobal.HasError Then
        '        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
        '        MyClass.PrepareArea()
        '    Else
        '        MyBase.ActivateMDIMenusButtons(True)
        '    End If

        'Catch ex As Exception
        '    MyBase.CreateLogActivity(ex.Message, Name & ".BsRestoreFactoryButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    MyBase.ShowMessage(Name & ".BsRestoreFactoryButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        '    MyClass.IsRestoringToFactoryRequested = False
        'End Try
    End Sub

    Private Sub BsAdjNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjNewButton.Click
        Dim myGlobal As New GlobalDataTO

        Try

            MyBase.DisplayMessage("")

            myGlobal = MyClass.WarnIfAdjustmentsEditionChanges

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                If CBool(myGlobal.SetDatos) Then
                    MyClass.CurrentAdjustmentsDS = New SRVAdjustmentsDS
                    With MyClass.CurrentAdjustmentsDS
                        .AnalyzerModel = "A400" ' MyBase.myServiceMDI.MDIAnalyzerManager.ActiveAnalyzer
                        .AnalyzerID = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveAnalyzer
                        .FirmwareVersion = MyBase.myServiceMDI.MDIAnalyzerManager.ActiveFwVersion
                        .ReadedDatetime = Nothing
                    End With

                    Dim myFwAdjustmentsDelegate As New FwAdjustmentsDelegate(MyClass.CurrentAdjustmentsDS)
                    Dim myAdjHeader As String = myFwAdjustmentsDelegate.MakeAdjustmentsFileHeader()

                    Me.BSAdjHeaderRTextBox.Text = myAdjHeader 'SGM 03/10/2012
                    Me.BsAdjustmentsRTextBox.Text = ""
                    MyClass.AdjustmentsEditionMode = True
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsAdjNewButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsAdjNewButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try
    End Sub

    Private Sub BsAdjCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjUndoButton.Click
        Dim myGlobal As New GlobalDataTO

        Try

            MyBase.DisplayMessage("")

            myGlobal = MyClass.WarnIfAdjustmentsEditionChanges

            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                If CBool(myGlobal.SetDatos) Then
                    Me.BsAdjustmentsRTextBox.Text = MyClass.InitialAdjustmentsText
                End If

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsAdjCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsAdjCancelButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage)
        End Try
    End Sub

#Region "Printing"

    Private Sub BsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsAdjPrintButton.Click
        Try
            PrintDocument1.DefaultPageSettings = myPrintSettings
            myTextToPrint = Me.BSAdjHeaderRTextBox.Text & vbCrLf & Me.BsAdjustmentsRTextBox.Text 'SGM 03/10/2012
            'myTextToPrint =  Me.BsAdjustmentsRTextBox.Text
            PrintDialog1.Document = PrintDocument1

            Dim DR As DialogResult = PrintDialog1.ShowDialog
            If DR = DialogResult.OK Then

                PrintDocument1.Print()

                Me.BsAdjPrintButton.Enabled = False

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".bsPrintButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.BsAdjPrintButton.Enabled = True
        End Try
    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Try
            Dim nChars As Integer
            Dim nLines As Integer
            Dim sFormat As New StringFormat
            Dim rectAng As New RectangleF(e.MarginBounds.Left, e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height)
            Dim MySize As New SizeF(e.MarginBounds.Width, e.MarginBounds.Height - myFontToPrint.GetHeight(e.Graphics))

            sFormat.Trimming = StringTrimming.Word
            e.Graphics.MeasureString(myTextToPrint, myFontToPrint, MySize, sFormat, nChars, nLines)
            myPageToPrint = myTextToPrint.Substring(0, nChars)
            e.Graphics.DrawString(myPageToPrint, myFontToPrint, Brushes.Black, rectAng, sFormat)
            If nChars < myTextToPrint.Length Then
                myTextToPrint = myTextToPrint.Substring(nChars)
                e.HasMorePages = True
            Else
                e.HasMorePages = False
                myPageToPrint = myTextToPrint
                Me.BsAdjPrintButton.Enabled = False
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".PrintDocument1_PrintPage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".PrintDocument1_PrintPage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            e.HasMorePages = False
            Me.BsAdjPrintButton.Enabled = True
        End Try
    End Sub

#End Region


#Region "History Methods"


    ''' <summary>
    ''' Updates the screen delegate's properties used for History management
    ''' </summary>
    ''' <param name="pResult"></param>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistory(ByVal pResult As InstrumentUpdateUtilDelegate.HISTORY_RESULTS, _
                              Optional ByVal pNOKReason As InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS = InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS.NONE)
        Try

            MyClass.ClearHistoryData()


            'PDT
            'If MyBase.SimulationMode Then Exit Sub 'PONER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            With MyClass.myScreenDelegate

                .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                .HistoryResult = pResult

                Select Case MyClass.SelectedPage

                    Case UPDATE_PAGES.ADJUSTMENTS
                        If MyClass.IsRestoringRequested Then

                            .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.RESTORE_ADJ
                        ElseIf MyClass.IsRestoringToFactoryRequested Then
                            .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.RESTORE_FACTORY_ADJ
                        Else
                            .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.BACKUP_ADJ
                        End If


                    Case UPDATE_PAGES.FIRMWARE


                        Select Case myScreenDelegate.FWUpdateCurrentAction
                            Case FwUpdateActions.StartUpdate : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_START
                            Case FwUpdateActions.SendRepository : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_SEND
                            Case FwUpdateActions.QueryCRC32 : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_SEND
                            Case FwUpdateActions.QueryNeeded : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_QUERY
                            Case FwUpdateActions.UpdateCPU : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_CPU
                            Case FwUpdateActions.UpdatePER : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_PER
                            Case FwUpdateActions.UpdateMAN : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_MAN
                            Case FwUpdateActions.UpdateADJ : .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.UPDATE_FW_ADJ

                        End Select

                        'If MyClass.IsFwUpdateRequested Then
                        '    If Not MyClass.IsFwFileReadyToSend Then
                        '        .HistoryNOKReason = InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS.UPDATE_FWFILE_WRONG_VERSION
                        '    End If
                        'End If


                End Select

                .HistoryNOKReason = pNOKReason

            End With


            MyClass.myScreenDelegate.ManageHistoryResults()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' Report Task not performed successfully to History
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistoryError()
        Try

            MyClass.ReportHistory(InstrumentUpdateUtilDelegate.HISTORY_RESULTS._ERROR)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistoryError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistoryError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ''' <summary>
    ''' Clears all the History Data
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ClearHistoryData()
        Try
            With MyClass.myScreenDelegate

                .HistoryArea = InstrumentUpdateUtilDelegate.HISTORY_AREAS.NONE
                .HistoryNOKReason = InstrumentUpdateUtilDelegate.HISTORY_NOK_REASONS.NONE

            End With
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region


#End Region

#Region "Firmware"

    Private Sub BsUpdateFwButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsFwUpdateButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            If MyClass.IsFwFileReadyToSend Then

                MyClass.IsFwUpdateRequested = True

                myGlobal = MyClass.TryUpdateFirmware()

                If myGlobal.HasError Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareArea()
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsUpdateFwButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsUpdateFwButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            MyClass.IsFwUpdateRequested = False
        End Try
    End Sub

    Private Sub BsOpenFileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsFwOpenFileButton.Click

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            If Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_UPDATEFW_MUST_SLEEP.ToString)
                MyBase.DisplayMessage("")
            Else
                myGlobal = MyClass.OpenFirmwareFile
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".BsOpenFileButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".BsOpenFileButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            MyClass.IsFwUpdateRequested = False
        End Try
    End Sub

#End Region

#Region "Communication"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 15/09/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myScreenDelegate.ReceivedLastFwScriptEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub
    ''' <summary>
    ''' manages the response of the Analyzer after sending a FwScript List
    ''' The response can be OK, NG, Timeout or Exception
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>Created by XBC 19/04/11</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics
            Application.DoEvents()

            If pResponse = RESPONSE_TYPES.TIMEOUT Then

                Select Case pData.ToString
                    Case AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString
                        MyBase.DisplayMessage(Messages.TRY_CONNECTION.ToString)

                    Case AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                        MyBase.DisplayMessage(Messages.ERROR_COMM.ToString)
                        PrepareErrorMode()

                End Select
                Exit Function
            ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                PrepareErrorMode()
                Exit Function
            End If

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)
            'MyClass.myFwScriptDelegate.IsWaitingForResponse = False
            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        myGlobal = MyBase.myServiceMDI.MDIAnalyzerManager.ReadFwAdjustmentsDS()
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                            MyBase.myServiceMDI.AdjustmentsReaded = True
                            MyClass.PrepareArea()
                        End If

                    End If


                    'SGM 24/11/2011
                Case ADJUSTMENT_MODES.FW_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.DisplayMessage(Messages.SRV_FW_READED.ToString)

                        If Not myGlobal.HasError Then
                            If TypeOf pData Is Byte() Then
                                MyClass.AnalyzerBinaryFwFile = CType(pData, Byte())
                                If MyClass.AnalyzerBinaryFwFile IsNot Nothing Then
                                    MyClass.PrepareArea()
                                Else
                                    MyClass.PrepareErrorMode()
                                    Exit Function
                                End If
                            Else
                                MyClass.PrepareErrorMode()
                                Exit Function
                            End If
                        Else
                            MyClass.PrepareErrorMode()
                            Exit Function
                        End If
                    End If
                    'SGM 24/11/2011


                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function



#End Region

#End Region

#Region "Refresh events"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by SGM 30/06/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Static myCPUUpdateResponseData As FWUpdateResponseTO
        Try

            If Not MyClass.IsFwUpdateRequested Then
                Select Case MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerCurrentAction
                    Case AnalyzerManagerAx00Actions.SLEEP_START
                        MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                        MyBase.CurrentMode = ADJUSTMENT_MODES.SLEEP_DOING
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.SLEEP_END
                        MyBase.CurrentMode = ADJUSTMENT_MODES.SLEEP_DONE
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.STANDBY_START
                        MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                        MyBase.CurrentMode = ADJUSTMENT_MODES.STANDBY_DOING
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.STANDBY_END
                        MyBase.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.LOADADJ_START
                        MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                        MyBase.CurrentMode = ADJUSTMENT_MODES.SAVING
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.LOADADJ_END
                        MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                        MyClass.PrepareArea()

                        'Case AnalyzerManagerAx00Actions.FACTORYADJ_START
                        '    MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                        '    MyBase.CurrentMode = ADJUSTMENT_MODES.SAVING
                        '    MyClass.PrepareArea()

                        'Case AnalyzerManagerAx00Actions.FACTORYADJ_END
                        '    MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                        '    MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.RESET_START
                        MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ANALYZER_RESETING
                        MyClass.PrepareArea()

                    Case AnalyzerManagerAx00Actions.RESET_END
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ANALYZER_IS_RESET
                        MyClass.PrepareArea()

                End Select

            Else
                If MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.FWUTIL_START Then

                    MyClass.TimeForWait = MyBase.myServiceMDI.MDIAnalyzerManager.MaxWaitTime
                    MyClass.IsFwUpdateRequested = True
                    If myScreenDelegate.FWUpdateCurrentAction > FwUpdateActions.QueryNeeded Then
                        MyClass.StartProgress()
                    End If
                End If
            End If

            MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerCurrentAction = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION

            'FWUTIL
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                'FW Util received
                sensorValue = 0
                sensorValue = MyBase.myServiceMDI.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FW_UPDATE_UTIL_RECEIVED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    MyBase.myServiceMDI.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.FW_UPDATE_UTIL_RECEIVED) = 0

                    'In case of step:Update CPU, when receive response
                    If myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU Then
                        If myCPUUpdateResponseData Is Nothing Then
                            myCPUUpdateResponseData = MyBase.myServiceMDI.MDIAnalyzerManager.FWUpdateResponseData
                            If myCPUUpdateResponseData.ActionResult = FW_GENERIC_RESULT.OK Then

                                'System.Threading.Thread.Sleep(8000)
                                'MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                                'MyClass.PrepareArea()
                                'myCPUUpdateResponseData = Nothing
                                'Exit Sub

                                ''Stop/Start method
                                ''1-Stop communications
                                Dim stopCommOK As Boolean = False
                                stopCommOK = myAnalyzerManager.StopComm
                                If Not stopCommOK Then
                                    myGlobal.HasError = True
                                Else
                                    '1-Wait 8 secs until the CPU is self reset
                                    System.Threading.Thread.Sleep(8000)
                                    Application.DoEvents()

                                    '3-Sart again communications
                                    Dim startCommOK As Boolean = False
                                    startCommOK = myAnalyzerManager.StartComm()
                                    If Not startCommOK Then
                                        myGlobal.HasError = True
                                    End If
                                End If

                            Else
                                MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                                MyClass.PrepareArea()
                                myCPUUpdateResponseData = Nothing
                            End If
                            Else
                                'In case of second time received ignore it
                                Exit Sub
                            End If
                    Else

                        MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                        MyClass.PrepareArea()

                        Application.DoEvents()

                    End If

                    Exit Sub
                End If

                'FW CPU Update reset Disconection SGM 10/07/2012
                If myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU Then
                    sensorValue = 0
                    sensorValue = MyBase.myServiceMDI.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.CONNECTED)
                    If sensorValue = 1 Then
                        MyBase.myServiceMDI.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.CONNECTED) = 0

                        'finalize process
                        If myCPUUpdateResponseData IsNot Nothing Then
                            MyBase.myServiceMDI.MDIAnalyzerManager.FWUpdateResponseData = myCPUUpdateResponseData
                            MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                            MyClass.PrepareArea()
                            myCPUUpdateResponseData = Nothing
                        End If
                        Exit Sub
                    End If
                    Exit Sub
                End If
                'SGM 10/07/2012




                sensorValue = 0
                sensorValue = MyBase.myServiceMDI.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    MyBase.myServiceMDI.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED) = 0

                    If myScreenDelegate.FWUpdateCurrentAction = FwUpdateActions.UpdateCPU Then
                        If MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                            MyBase.CurrentMode = ADJUSTMENT_MODES.FW_UTIL_RECEIVED
                            MyClass.PrepareArea()
                        End If
                    End If



                End If


            End If


            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region


#Region "Key Events - Adjustments Edition"

    Protected Overrides Function ProcessDialogKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean

        Dim myGlobal As New GlobalDataTO
        Try
            Select Case keyData
                Case Keys.Escape
                    If MyClass.AdjustmentsEditionMode Then
                        myGlobal = MyClass.WarnIfAdjustmentsEditionChanges

                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                            If CBool(myGlobal.SetDatos) Then
                                MyClass.AdjustmentsEditionMode = False
                                Me.Close()
                            End If

                        End If
                    ElseIf (Me.BsExitButton.Enabled) Then
                        ' When the  ESC key is pressed, the screen is closed 
                        Me.Close()
                    End If

                Case Keys.Tab


                Case Keys.Enter



            End Select
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ProcessDialogKey ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ProcessDialogKey ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

#End Region

#Region "Progress Bar"

#Region "Private Properties"

    Private ProgressStartTime As TimeSpan = Nothing
    Private ProgressEndTime As TimeSpan = Nothing
    Private IsInprogress As Boolean = False

#End Region


    Private ReadOnly Property ProgressElapsedTime() As Integer
        Get
            Dim NowTime As New TimeSpan(Now.Ticks)
            Try
                Return CInt(NowTime.TotalMilliseconds - MyClass.ProgressStartTime.TotalMilliseconds)
            Catch ex As Exception
                Return -1
            End Try
        End Get
    End Property



    Private Sub StartProgress(Optional ByVal pTimeForWait As Integer = -1)

        Try
            If pTimeForWait > -1 Then
                MyClass.TimeForWait = pTimeForWait
            End If

            If MyClass.TimeForWait > 0 Then
                MyClass.ProgressStartTime = New TimeSpan(Now.Ticks)
                MyClass.ProgressEndTime = MyClass.ProgressStartTime.Add(New TimeSpan(0, 0, 0, 0, MyClass.TimeForWait * 1000))

                Me.BsWaitProgressBar.Minimum = 0
                Me.BsWaitProgressBar.Maximum = MyClass.TimeForWait * 1000

                MyClass.WaitTimerCallBack = New System.Threading.TimerCallback(AddressOf OnWaitTimerTick)
                MyClass.WaitTimer = New System.Threading.Timer(MyClass.WaitTimerCallBack, New Object, 10, 10)

                MyClass.IsInprogress = True

                Me.BsWaitProgressBar.Visible = True
                Me.BsWaitProgressBar.Refresh()
            End If

        Catch ex As Exception
            MyClass.StopProgress()
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StartProgress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StartProgress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub


    Private Sub StopProgress(Optional ByVal pHide As Boolean = True)

        Try

            If pHide Then
                If Me.BsWaitProgressBar.InvokeRequired Then
                    Dim d As New HideProgressBarCallBack(AddressOf HideProgress)
                    Me.BsWaitProgressBar.Invoke(d)
                Else
                    MyClass.HideProgress()
                End If
            End If
            If MyClass.WaitTimer IsNot Nothing Then
                MyClass.WaitTimer.Dispose()
                MyClass.WaitTimer = Nothing
            End If

            If MyClass.WaitTimerCallBack IsNot Nothing Then
                MyClass.WaitTimerCallBack = Nothing
            End If

            MyClass.IsInprogress = False

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StopProgress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopProgress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub HideProgress()
        Try
            Me.BsWaitProgressBar.Visible = False
            Me.BsWaitProgressBar.Value = Me.BsWaitProgressBar.Minimum
            Me.BsWaitProgressBar.Refresh()
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".HideProgress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".HideProgress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub UpdateProgress()
        Try
            If MyClass.ProgressElapsedTime >= Me.BsWaitProgressBar.Minimum And MyClass.ProgressElapsedTime <= Me.BsWaitProgressBar.Maximum Then
                Me.BsWaitProgressBar.Value = MyClass.ProgressElapsedTime
                Me.BsWaitProgressBar.Refresh()
                If MyBase.Container IsNot Nothing Then
                    Dim myContainer As Windows.Forms.Control = CType(MyBase.Container, Windows.Forms.Control)
                    If myContainer IsNot Nothing Then
                        myContainer.Refresh()
                    End If
                End If
            End If
        Catch ex As Exception
            MyClass.StopProgress()
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateProgress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateProgress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub OnWaitTimerTick(ByVal stateInfo As Object)

        Try

            If (MyClass.ProgressElapsedTime <= Me.BsWaitProgressBar.Maximum) Then
                If Me.BsWaitProgressBar.InvokeRequired Then
                    Dim d As New UpdateProgressCallBack(AddressOf UpdateProgress)
                    Me.BsWaitProgressBar.Invoke(d)
                Else
                    MyClass.UpdateProgress()
                End If
            Else
                MyClass.StopProgress()

            End If

        Catch ex As Exception
            MyClass.StopProgress()
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".OnWaitTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".OnWaitTimerTick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoAdjXPSViewer.Load, _
                                                                                            BsInfoFwXPSViewer.Load
        Try
            Dim myBsXPSViewer As BsXPSViewer = CType(sender, BsXPSViewer)
            If myBsXPSViewer IsNot Nothing Then
                If myBsXPSViewer.IsScrollable Then
                    myBsXPSViewer.FitToPageWidth()
                Else
                    myBsXPSViewer.FitToPageHeight()
                    myBsXPSViewer.FitToPageWidth()
                End If
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

End Class