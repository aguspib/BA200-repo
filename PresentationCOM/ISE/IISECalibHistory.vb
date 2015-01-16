Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls

'PENDING:
'Add Information XPS Docs for each Action
'The Reagents Pack Dallas Information MUST BE HIDE!!!!!!!!!! (DisplayISEInfo)
Public Class IISECalibHistory

#Region "Declarations"
    Private WithEvents mdiAnalyzerCopy As AnalyzerManager
    Private myISEManager As ISEManager

    ' Language
    Private currentLanguage As String

    Private myParentMDI As BSBaseForm

#End Region

#Region "Events definitions"
    Public Shared Event ActivateScreenEvent(ByVal pEnable As Boolean, ByVal pMessageID As GlobalEnumerates.Messages)
    Public Shared Event ActivateVerticalButtonsEvent(ByVal pEnable As Boolean) 'SGM 14/05/2012

#End Region

#Region "Constructor"

    Public Sub New(ByVal pMDI As BSBaseForm)

        MyClass.myParentMDI = pMDI
        InitializeComponent()

    End Sub
#End Region

#Region "Attributes"
   
#End Region

#Region "Properties"

    Private ReadOnly Property ThisIsService() As Boolean
        Get
            Return My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE")
        End Get
    End Property


#End Region

#Region "Enumerations"
    
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

    Private Sub InitializeGrid(ByVal pGrid As BSDataGridView)

        If pGrid IsNot Nothing Then Exit Sub

        'CalibrationDate	datetime	Unchecked
        'ReagentsPackID	nvarchar(30)	Checked
        'ResultsString	nvarchar(MAX)	Unchecked
        'ErrorString	nvarchar(MAX)	Unchecked
        'ActionType	nvarchar(30)	Unchecked

        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            pGrid.Columns.Clear()

            Dim STATColumn As New DataGridViewImageColumn
            With STATColumn
                .Name = "STAT"
                .HeaderText = ""
                .Width = 24
            End With
            pGrid.Columns.Add(STATColumn)

            'bsSamplesListDataGridView.Columns.Add("PatientName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", LanguageID))
            'bsSamplesListDataGridView.Columns("PatientName").Width = 122
            'bsSamplesListDataGridView.Columns("PatientName").SortMode = DataGridViewColumnSortMode.Automatic

            'RH 08/05/2012 Change PatientName by PatienID
            pGrid.Columns.Add("PatientID", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", MyClass.currentLanguage))
            pGrid.Columns("PatientID").Width = 122
            'bsSamplesListDataGridView.Columns("PatientID").Width = 105
            pGrid.Columns("PatientID").SortMode = DataGridViewColumnSortMode.Automatic

            Dim PrintReportColumn As New DataGridViewImageColumn
            With PrintReportColumn
                .Name = "Print"
                .HeaderText = ""
                '.Width =  30 dl 16/03/2011
                .Visible = False ' dl 16/03/2011
            End With
            pGrid.Columns.Add(PrintReportColumn)

            Dim HISExportColumn As New DataGridViewImageColumn
            With HISExportColumn
                .Name = "HISExport"
                .HeaderText = ""
                '.Width = 30 dl 16/03/2011
                .Visible = False ' dl 16/03/2011
            End With
            pGrid.Columns.Add(HISExportColumn)

            ' Ini DL 16/03/2011
            Dim PrintReportColumnNew As New DataGridViewCheckBoxColumn
            With PrintReportColumnNew
                .Name = "OrderToPrint"
                '.HeaderText = "Print"
                .HeaderText = ""
                .Width = 30
            End With
            pGrid.Columns.Add(PrintReportColumnNew)

            Dim ExportColumn As New DataGridViewCheckBoxColumn
            With ExportColumn
                .Name = "OrderToExport"
                '.HeaderText = "OrderToExport"
                .HeaderText = ""
                .Width = 30
            End With
            pGrid.Columns.Add(ExportColumn)
            ' End DL 16/03/2011

            pGrid.Columns.Add("OrderID", "")
            pGrid.Columns("OrderID").Visible = False

            'bsSamplesListDataGridView.Columns.Add("PatientID", "")
            'bsSamplesListDataGridView.Columns("PatientID").Visible = False

            'RH 08/05/2012 Change PatienID by PatientName
            pGrid.Columns.Add("PatientName", "")
            pGrid.Columns("PatientName").Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeSamplesGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Displays a mesage in the message area (service Sw)
    ''' </summary>
    ''' <param name="pMessageID"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 07/02/2012</remarks>
    Private Function DisplayMessage(ByVal pMessageID As String, Optional ByVal p2ndMessageID As String = "") As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            Dim Messages As New MessageDelegate
            Dim myMessagesDS As New MessagesDS
            Dim myGlobalDataTO As New GlobalDataTO

            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'Get type and multilanguage text for the informed Message
            Dim msgText As String = ""
            If pMessageID.Length > 0 Then
                myGlobalDataTO = Messages.GetMessageDescription(Nothing, pMessageID)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    Dim Exists As Boolean = False
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        msgText = myMessagesDS.tfmwMessages(0).MessageText
                        Me.BsMessageImage.BackgroundImage = Nothing
                        'Show message with the proper icon according the Message Type
                        If (myMessagesDS.tfmwMessages(0).MessageType = "Error") Then
                            'Error Message 
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("CANCELF")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Information") Then
                            'Information Message 
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("INFO")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Warning") Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("WARNING")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "OK") Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("ACCEPTF")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Working") Then
                            'Warning
                            Me.BsMessageLabel.Text = msgText
                            auxIconName = GetIconName("GEAR")
                            Exists = System.IO.File.Exists(iconPath & auxIconName)
                        End If

                    End If

                    'second line
                    If p2ndMessageID.Length > 0 Then
                        myGlobalDataTO = Messages.GetMessageDescription(Nothing, p2ndMessageID)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                            msgText = msgText & " | " & myMessagesDS.tfmwMessages(0).MessageText
                            Me.BsMessageLabel.Text = msgText
                        End If
                    End If

                    If Exists Then
                        If System.IO.File.Exists(iconPath & auxIconName) Then
                            Dim myUtil As New Utilities
                            Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
                            myGlobalDataTO = myUtil.ResizeImage(myImage, New Size(20, 20))
                            If Not myGlobalDataTO.HasError And myGlobalDataTO.SetDatos IsNot Nothing Then
                                Me.BsMessageImage.BackgroundImage = CType(myGlobalDataTO.SetDatos, Image) 'Image.FromFile(iconPath & auxIconName)
                            Else
                                Me.BsMessageImage.BackgroundImage = myImage
                            End If
                            Me.BsMessageImage.BackgroundImageLayout = ImageLayout.Center
                            'myScreenLayout.MessagesPanel.Icon.Size = New Size(20, 20)
                        End If
                    End If
                Else
                    MessageBox.Show(Me, myGlobalDataTO.ErrorCode, "BSAdjustmentBaseForm.mybase.displaymessage", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                'clear
                Me.BsMessageLabel.Text = ""
                Me.BsMessageImage.BackgroundImage = Nothing
            End If

            Application.DoEvents()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DisplayMessage", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>XBC 10/01/2012</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim myGlobal As New GlobalDataTO
        Try

            MyClass.SetButtonImage(BsExitButton, "CANCEL")

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub SetButtonImage(ByVal pButton As Button, ByVal pImageName As String, _
                             Optional ByVal pWidth As Integer = 28, _
                             Optional ByVal pHeight As Integer = 28, _
                             Optional ByVal pAlignment As ContentAlignment = ContentAlignment.MiddleCenter)

        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            Dim myButtonImage As Image

            auxIconName = GetIconName(pImageName)
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image
                myImage = Image.FromFile(iconPath & auxIconName)

                myGlobal = myUtil.ResizeImage(myImage, New Size(pWidth, pHeight))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myButtonImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myButtonImage = CType(myImage, Bitmap)
                End If

                pButton.Image = myButtonImage
                pButton.ImageAlign = pAlignment

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetButtonImage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetButtonImage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 27/01/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ISE", currentLanguage)
            Me.BsInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)

            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                ' Service Sw
                Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Utilities", currentLanguage)
            Else
                ' User Sw
                Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Utilities", currentLanguage)
            End If


            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 02/02/2012
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            Me.bsScreenToolTips.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    ''' <summary>
    ''' Get Limits values from BD for Ise tests
    ''' </summary>
    ''' <remarks>Created by XBC 24/01/2012</remarks>
    Public Function GetLimitValues() As GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDS As New FieldLimitsDS
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()



            'Load limits for Acceptable dispensations volume
            myResultData = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.ISE_DISPAB_LIMITS)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myResultData.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    'MyClass.DispVolumeMaxAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    'MyClass.DispVolumeMinAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    'MyClass.DispVolumeDefAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).DefaultValue, Decimal)
                End If
            End If

        Catch ex As Exception
            myResultData.HasError = True
            myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "IseAdjustmentDelegate.GetLimitValues", EventLogEntryType.Error, False)
        End Try
        Return myResultData
    End Function

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 24/01/2012</remarks>
    Private Sub PrepareLoadedMode()
        Try

            Me.DisplayMessage("")

            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub


    Public Sub PrepareErrorMode()
        Try
            'TODO

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub CreateFileOutput(ByVal pPath As String)
        Try
            'If File.Exists(pPath) Then File.Delete(pPath)
            'Dim myStreamWriter As StreamWriter = File.CreateText(pPath)

            'For Each line As String In BsRichTextBox1.Lines
            '    myStreamWriter.WriteLine(line)
            'Next line

            'myStreamWriter.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Copied and adapted from IAx00MainMDI (PresentationUSR) because we can not call to IAx00MainMDI.ActivateButtonWithAlarms
    ''' </summary>
    ''' <param name="pButton"></param>
    ''' <returns></returns>
    ''' <remarks>AG 29/03/2012</remarks>
    Public Function ActivateButtonWithAlarms(ByVal pButton As GlobalEnumerates.ActionButton) As Boolean
        Dim myStatus As Boolean = True
        Try

            If Not mdiAnalyzerCopy Is Nothing Then

                'Dim resultData As New GlobalDataTO
                Dim myAlarms As New List(Of GlobalEnumerates.Alarms)

                ' AG+XBC 24/05/2012
                'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
                ' AG+XBC 24/05/2012

                myAlarms = mdiAnalyzerCopy.Alarms
                myAx00Status = mdiAnalyzerCopy.AnalyzerStatus

                ''AG 25/10/2011 - Before treat the cover alarms read if they are deactivated (0 disabled, 1 enabled)
                'Dim mainCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN), 1, 0), Boolean)
                'Dim reactionsCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN), 1, 0), Boolean)
                'Dim fridgeCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN), 1, 0), Boolean)
                'Dim samplesCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_WARN), 1, 0), Boolean)

                'resultData = mdiAnalyzerCopy.ReadFwAdjustmentsDS
                'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                '    'Dim myAdjDS As SRVAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS) 'Causes system error in develop mode
                '    Dim myAdjDS As New SRVAdjustmentsDS
                '    myAdjDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                '    Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                '    'Main cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.MCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then mainCoverAlarm = False
                '    End If
                '    'Reactions cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then reactionsCoverAlarm = False
                '    End If
                '    'Reagents cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.RCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then fridgeCoverAlarm = False
                '    End If
                '    'Samples cover disabled (0 disabled, 1 enabled)
                '    linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                '               Where a.CodeFw = GlobalEnumerates.Ax00Adjustsments.SCOV.ToString Select a).ToList
                '    If linqRes.Count > 0 AndAlso linqRes(0).Value <> "" Then
                '        If Not CType(linqRes(0).Value, Boolean) Then samplesCoverAlarm = False
                '    End If
                'End If
                ''AG 25/10/2011

                'AG 02/04/2012
                '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
                Dim iseInitiatedFinishedFlag As Boolean = True
                If mdiAnalyzerCopy.ISE_Manager IsNot Nothing AndAlso _
                   (mdiAnalyzerCopy.ISE_Manager.IsISEInitiating OrElse Not mdiAnalyzerCopy.ISE_Manager.ConnectionTasksCanContinue) Then
                    iseInitiatedFinishedFlag = False
                End If
                'AG 02/04/2012

                Select Case pButton
                    'ISE COMMAND FROM ISE UTILITIES SCREEN
                    Case ActionButton.ISE_COMMAND
                        If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            'If mainCoverAlarm OrElse _
                            '   reactionsCoverAlarm OrElse _
                            '   fridgeCoverAlarm OrElse _
                            '   samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            '    myStatus = False
                            'End If
                        Else
                            myStatus = False
                        End If
                        'AG 28/03/2012
                End Select

                'XBC 03/04/2012
                If ThisIsService Then
                    If myISEManager.IsISEModuleInstalled And Not myISEManager.IsISESwitchON Then
                        myStatus = False
                    End If
                Else
                    If myAlarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                        myStatus = False
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ActivateButtonWithAlarms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateButtonWithAlarms, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myStatus
    End Function

#End Region

#Region "Events"

    Protected Sub IISECalibHistory_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            'Dim myGlobal As New GlobalDataTO
            Dim myGlobalbase As New GlobalBase

            'Get an instance of the ISE manager class
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) ' Use the same AnalyzerManager as the MDI
                myISEManager = mdiAnalyzerCopy.ISE_Manager
            End If

            'Get the current user level SGM 07/06/2012
            MyBase.CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel

            'Get the current Language from the current Application Session
            Me.currentLanguage = GlobalBase.GetSessionInfo.ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            Me.GetScreenLabels()
            Me.PrepareButtons()


            If ThisIsService Then
                'TODO
            Else
                'TODO
            End If



            'RH 26/03/2012
            If ThisIsService Then
                Me.BsBorderedPanel1.Visible = False
                ResetBorderSRV()
            Else
                ResizeUserForm()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Resizes the form and hides unused areas
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 26/03/2012
    ''' </remarks>
    Private Sub ResizeUserForm()
        Try
            Me.ControlBox = False
            Me.MinimizeBox = False
            Me.MaximizeBox = False
            Me.AutoScaleMode = Windows.Forms.AutoScaleMode.None
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            Me.WindowState = FormWindowState.Normal

            Me.Controls.Remove(Me.bsISEInformationGroupBox)
            Me.Width = 737
            Me.Height = 592

            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)

            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResizeForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResizeForm ", Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    Private Sub IIseAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Dim myGlobal As New GlobalDataTO
        Try

            If ThisIsService Then

            Else


            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click

        Dim myGlobal As New GlobalDataTO

        Try

            'TODO


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub




    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 01/02/2012
    ''' </remarks>
    Private Sub IISECalibHistory_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


#End Region

#Region "Lithium Enabling/Disabling"
    ''' <summary>
    ''' Check if a Li+ Test is in Use
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 17/05/2012</remarks>
    Public Function LithiumTestInUse() As Boolean
        Dim myGlobal As New GlobalDataTO
        Dim InUse As Boolean = False
        Try
            Dim myISETestDelegate As New ISETestsDelegate
            myGlobal = myISETestDelegate.ExistsISETestName(Nothing, "Li+", "NAME")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISETestsDS As ISETestsDS = CType(myGlobal.SetDatos, ISETestsDS)
                If myISETestsDS.tparISETests.Count > 0 Then
                    InUse = CBool(myISETestsDS.tparISETests(0).InUse)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LithiumTestInUse", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LithiumTestInUse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return InUse
    End Function

    ''' <summary>
    ''' Updates tparISETests table depending on the Li+ is enabled
    ''' </summary>
    ''' <param name="pEnabled"></param>
    ''' <returns></returns>
    ''' <remarks>SGM 17/05/2012</remarks>
    Public Function UpdateLithiumTestEnabled(ByVal pEnabled As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myISETestDelegate As New ISETestsDelegate

            myGlobal = myISETestDelegate.ExistsISETestName(Nothing, "Li+", "NAME")
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myISETestsDS As ISETestsDS = CType(myGlobal.SetDatos, ISETestsDS)
                If myISETestsDS.tparISETests.Count > 0 Then
                    myISETestsDS.tparISETests(0).Enabled = pEnabled
                    myGlobal = myISETestDelegate.Modify(Nothing, myISETestsDS)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateLithiumTestEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateLithiumTestEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobal
    End Function

#End Region

End Class