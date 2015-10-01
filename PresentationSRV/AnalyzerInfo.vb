Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports System.IO
Imports System.Drawing.Printing
Imports Biosystems.Ax00.App


Public Class AnalyzerInfo
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Constants"
    Private Const Window_Width_Short As Integer = 541
    Private Const Window_Height_Short As Integer = 212
    Private Const ExitButtonLocationTop_X As Integer = 493
    Private Const ExitButtonLocationTop_Y As Integer = 171

    Private Const Window_Width_Details As Integer = 541
    Private Const Window_Height_Details As Integer = 473
    Private Const ExitButtonLocationBottom_X As Integer = 493
    Private Const ExitButtonLocationBottom_Y As Integer = 430

    Private Const SpecifiedSerialNumberLength As Integer = 9 'SGM 15/10/2012

#End Region

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As AnalyzerInfoDelegate

    ' Language
    Private currentLanguage As String

    ' Waiting process functionality
    Private myWaitingTime As Boolean
    Protected wfPreload As Biosystems.Ax00.PresentationCOM.UiWaitScreen ' .IAx00StartUp 'DL 18/04/2012

    ' Print variables
    Private myTextToPrint As String
    Private ReadOnly OptPrnPage As New PageSettings()
    Private StrToPrn As String
    'Private FontPrn As New Font("Arial", 12)
    'Private myWARNINGImage As Image = Nothing

    Private Details As Boolean
    Private RequestMoreInfo As Boolean

    Private FirstScreenLoad As Boolean
    Private EnableMoveWindow As Boolean

    Private IsInfoAlreadyWritten As Boolean = False

    Private InitApplicationAllowed As Boolean = False

    'SGM 08/11/2012
    Public IsStopAlarmWhileInitialization As Boolean = False

    Private IsEditingSN As Boolean = False

#End Region

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout()

    End Sub
#End Region

#Region "Event Handlers"
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
    ''' <remarks>Created by XBC 25/05/11</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            ' XBC 05/05/2011 - timeout limit repetitions
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
            ' XBC 05/05/2011 - timeout limit repetitions

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)

            Select Case MyBase.CurrentMode

                ''SGM 28/11/2011
                'Case ADJUSTMENT_MODES.FW_READED
                '    If pResponse = RESPONSE_TYPES.OK Then

                '        MyBase.DisplayMessage(Messages.SRV_FW_READED.ToString)

                '        MyBase.myServiceMDI.ActiveFwVersion = CType(pData, String)

                '        If Not myGlobal.HasError Then
                '            PrepareArea()
                '        Else
                '            PrepareErrorMode()
                '            Exit Function
                '        End If
                '    End If


                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.myServiceMDI.AdjustmentsReaded = True
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                    'Case ADJUSTMENT_MODES.SAVED
                    '    If pResponse = RESPONSE_TYPES.START Then
                    '        ' Nothing by now
                    '    End If
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        PrepareArea()
                    '    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ManageReceptionEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDs"></param>
    ''' <remarks>Created by XBC 25/05/2011
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDs As Biosystems.Ax00.Types.UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            'sensors
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                'SGM 17/09/2012
                'ISE Monitor Data changed
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False
                    'AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS) = 0 'Once updated UI clear sensor
                End If
                'end SGM 17/09/2012

                'SGM 20/04/2012
                'ISE initiated
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                If sensorValue >= 1 Then
                    ScreenWorkingProcess = False

                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                    MyBase.ActivateMDIMenusButtons(True)

                End If

                'ISE procedure finished
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor

                End If
                'end SGM 20/04/2012

                'SGM 10/10/2012 - Serial Number saved
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.SERIAL_NUMBER_SAVED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.SERIAL_NUMBER_SAVED) = 0 'Once updated UI clear sensor

                    Select Case AnalyzerController.Instance.Analyzer.SaveSNResult
                        Case UTILSNSavedResults.OK : MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                        Case UTILSNSavedResults.KO : MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                    End Select
                    MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)
                    MyClass.PrepareArea()
                    Exit Sub
                End If
                'end SGM 10/10/2012 

            End If

            If myScreenDelegate IsNot Nothing Then
                If MyBase.CurrentMode = ADJUSTMENT_MODES.FW_READING Then
                    myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)
                End If
                'myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)
            End If


            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)
            PrepareArea()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#Region "Must Inherited"

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 24/05/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            Ax00ServiceMainMDI.ActivateMenus(True)
            Me.ProgressBar1.Visible = False
            Me.myWaitingTime = False
            MyBase.ErrorMode()
            DisableAll()
            MyBase.ActivateMDIMenusButtons(True)
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'SGM 08/11/2012
            If pAlarmType = ManagementAlarmTypes.UPDATE_FW Or pAlarmType = ManagementAlarmTypes.FATAL_ERROR Or pAlarmType = ManagementAlarmTypes.RECOVER_ERROR Then
                MyClass.IsStopAlarmWhileInitialization = True
            End If

            MyClass.PrepareLoadedMode()
            MyClass.PrepareErrorMode(pAlarmType)

            ''SGM 08/11/2012 - stop ISE operation
            'If MyClass.myServiceMDI.MDIAnalyzerManager.ISE_Manager IsNot Nothing Then
            '    MyClass.myServiceMDI.MDIAnalyzerManager.ISE_Manager.AbortCurrentProcedureDueToException()
            'End If

            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Private Methods"
    Private Sub Initializations()
        Try
            Me.bsSerialTextBox.Text = ""
            Me.BsRichTextBox1.Text = ""
            myScreenDelegate.ReadFWDone = False
            myScreenDelegate.ReadAnalyzerInfoDone = False
            Me.Details = False
            Me.FirstScreenLoad = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 24/05/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_INFOANALYZER", currentLanguage)
            'Me.bsSerialDetailsGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SERIAL_DETAILS", currentLanguage)
            Me.bsSerialLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SERIAL", currentLanguage) & ":"
            Me.bsInfoDetailsGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_DETAILS", currentLanguage)
            Me.bsFwVersionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirmwareVersion", currentLanguage) & ":"

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 26/07/11
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTipsControl.SetToolTip(bsEditSNButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(bsSaveSNButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(bsCancelSNButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 24/05/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath
        Try
            ' PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                bsPrintButton.BackgroundImageLayout = ImageLayout.Center
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If File.Exists(iconPath & auxIconName) Then
                BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                'BsExitButton.BackgroundImageLayout = ImageLayout.Stretch
            End If

            ' EDIT serial number button
            auxIconName = GetIconName("EDIT")
            If File.Exists(iconPath & auxIconName) Then
                bsEditSNButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                'bsEditSNButton.BackgroundImageLayout = ImageLayout.Center
            End If

            ' SAVE serial number button
            auxIconName = GetIconName("SAVE")
            If File.Exists(iconPath & auxIconName) Then
                bsSaveSNButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                'bsSaveSNButton.BackgroundImageLayout = ImageLayout.Center
            End If

            ' CANCEL serial number edition button
            auxIconName = GetIconName("UNDO")
            If File.Exists(iconPath & auxIconName) Then
                bsCancelSNButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                'bsCancelSNButton.BackgroundImageLayout = ImageLayout.Center
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: XBC 24/05/2011</remarks>
    Private Sub DisableAll()
        Try
            bsSerialTextBox.Enabled = False
            'AJG. BA-2132
            bsModelTextBox.Enabled = False
            bsEditSNButton.Enabled = False
            bsSaveSNButton.Enabled = False
            bsCancelSNButton.Enabled = False
            BsRichTextBox1.Enabled = False
            bsPrintButton.Enabled = False
            DetailsButton.Enabled = False
            MoreInfoButton.Enabled = False

            ' Disable Area Buttons
            BsExitButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 30/05/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel
            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 24/05/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED

                    If MyBase.SimulationMode Then
                        'Dim myGlobal As New GlobalDataTO
                        'MyBase.myServiceMDI.ReadAllFwAdjustments()

                    End If
                    'MyBase.myServiceMDI.AdjustmentsReaded = True 'SGM 15/11/2011
                    'MyBase.myServiceMDI.isWaitingForAdjust = False 'SGM 15/11/2011
                    'Me.myAnalyzerManager.ClearQueueToSend() 'SGM 15/11/2011
                    'MyBase.myServiceMDI.SEND_INFO_START() 'SGM 15/11/2011

                    Me.myWaitingTime = False
                    Application.DoEvents()
                    MyClass.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.FW_READED
                    MyClass.PrepareFWReadedMode()

                Case ADJUSTMENT_MODES.STANDBY_DONE
                    MyClass.PrepareStandByDoneMode()

                Case ADJUSTMENT_MODES.ANALYZER_INFO_READED
                    MyClass.PrepareAnalyzerInfoReadedMode()

                Case ADJUSTMENT_MODES.SAVING
                    MyClass.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    MyClass.PrepareSavedMode()

                Case ADJUSTMENT_MODES.SN_SAVING
                    MyClass.PrepareSNSavingMode()

                Case ADJUSTMENT_MODES.SN_SAVED
                    MyClass.PrepareSNSavedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareErrorMode()
            End Select

            'If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
            '    MyClass.PrepareErrorMode()
            'End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments Reading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 24/05/2011</remarks>
    Private Sub PrepareAdjustReadingMode()
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()

            ' Reading Adjustments
            If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                Me.Cursor = Cursors.WaitCursor
                ' Parent Reading Adjustments
                MyBase.ReadAdjustments()
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                    'System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyClass.WaitProcessSimulation()
                    MyBase.myServiceMDI.Focus()
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                        myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                    End If
                    'Application.DoEvents()
                    'System.Threading.Thread.Sleep(SimulationProcessTime)
                End If
            Else
                MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                PrepareArea()
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoids waiting window's GIF seem to be stopped
    ''' </summary>
    ''' <remarks>Created by SGM 04/01/2012</remarks>
    Private Sub WaitProcessSimulation()
        Try
            ' TEMPORAL FINS QUE FUNCIONI POLLFW !!!
            'If AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
            '    Exit Sub
            'End If

            Dim wait As Integer = 0
            While wait < 100
                'System.Threading.Thread.Sleep(1)
                Application.DoEvents()
                wait += 1
            End While
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WaitProcessSimulation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WaitProcessSimulation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <remarks>Created by SGM 19/09/2011</remarks>
    'Private Sub DeactivateANSINFRefreshingMode()
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        If Not MyBase.SimulationMode Then
    '            'the INFO data refreshing mode must be deactivated before reading adjustments
    '            'SGM 19/09/2011
    '            If Ax00ServiceMainMDI.MDIAnalyzerManager IsNot Nothing Then
    '                If AnalyzerController.Instance.Analyzer.Connected Then

    '                    myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
    '                                                         True, _
    '                                                         Nothing, _
    '                                                         GlobalEnumerates.Ax00InfoInstructionModes.STP)

    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateANSINFRefreshingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".DeactivateANSINFRefreshingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 24/05/2011</remarks>
    Private Sub PrepareLoadingMode()
        Dim myResultData As New GlobalDataTO
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Dim myText1 As String
        Dim myText2 As String

        Try
            DisableAll()
            ' Reading Fw Information
            myResultData = MyBase.ReadFw()
            If myResultData.HasError Then
                PrepareErrorMode()
                Exit Sub
            Else
                Me.Cursor = Cursors.WaitCursor
                ' Initializations
                myScreenDelegate.ReadFWDone = False

                'SGM 16/11/2011
                myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INITIALIZING", MyClass.currentLanguage)
                myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INIT_WAIT", MyClass.currentLanguage)

                Application.DoEvents()

                '' TO DELETE
                'myScreenDelegate.ReadFWDone = True
                'myScreenDelegate.AnalyzerController.Instance.Analyzer.IsFwSwCompatible = True
                'MyClass.CurrentMode = ADJUSTMENT_MODES.FW_READED
                'PrepareArea()
                'Exit Try
                '' TO DELETE

                Me.WaitingControl(myText1, myText2)

                wfPreload.Refresh()



                '' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    '    ' simulating...
                    'MyBase.DisplayMessage("Request Fw Information from Instrument...")

                    MyClass.WaitProcessSimulation() 'SGM 04/01/2012

                    MyBase.myServiceMDI.Focus()
                    'If MyClass.CurrentMode <> ADJUSTMENT_MODES.ADJUSTMENTS_READING Then 'SGM 15/11/2011
                    MyClass.CurrentMode = ADJUSTMENT_MODES.FW_READED
                    'End If
                    PrepareArea()
                Else
                    'SGM 19/11/2012
                    myResultData = myScreenDelegate.REQUEST_FW_INFO()

                    ''pending - stop connection process when E:21 (Recover needed)
                    'If Not AnalyzerController.Instance.Analyzer.IsInstructionAborted Then
                    '    myResultData = myScreenDelegate.REQUEST_FW_INFO()
                    'Else
                    '    MyBase.myServiceMDI.IsAutoConnecting = False
                    '    AnalyzerController.Instance.Analyzer.IsAutoInfoActivated = True
                    '    MyBase.myServiceMDI.SEND_INFO_STOP()
                    '    If wfPreload IsNot Nothing Then wfPreload.Close()
                    '    MyBase.ActivateMDIMenusButtons(True)
                    'End If
                    'end SGM 19/11/2012
                End If


                If myResultData.HasError Then
                    Me.PrepareErrorMode()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for FW Readed Mode
    ''' </summary>
    ''' <remarks>Created by XBC 02/06/2011</remarks>
    Private Sub PrepareFWReadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            If MyBase.SimulationMode Then
                ' Condition just to test
                AnalyzerController.Instance.Analyzer.IsFwSwCompatible = True '#REFACTORING
                'myScreenDelegate.FwCompatible = True
            End If

            ' Check Fw Compatibility
            If Not myScreenDelegate.FwCompatible Then
                Me.Cursor = Cursors.Default
                Me.myWaitingTime = False
                Exit Sub
            End If

            If myScreenDelegate.ReadFWDone Then

                PrepareInitiatingAnalyzerMode()

                If MyClass.myServiceMDI.AdjustmentsReaded Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()
                    myScreenDelegate.SendINFO_START()
                End If
            Else
                ' Continue with the Reading until finish it
                Me.Cursor = Cursors.WaitCursor
                myResultData = MyBase.ReadFw()
                If myResultData.HasError Then
                    PrepareErrorMode()
                    Exit Sub
                Else



                    If MyBase.SimulationMode Then
                        '    ' simulating...
                        'MyBase.DisplayMessage("Request Fw Information from Instrument...")

                        MyClass.WaitProcessSimulation() 'SGM 04/01/2012
                        MyBase.myServiceMDI.Focus()
                        myScreenDelegate.ReadFWDone = True
                        MyClass.CurrentMode = ADJUSTMENT_MODES.FW_READED
                        PrepareArea()
                    Else
                        myResultData = myScreenDelegate.REQUEST_FW_INFO()
                    End If

                End If

            End If

            If myResultData.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareFWReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareFWReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for StandBy Done mode
    ''' </summary>
    ''' <remarks>Created by XBC 13/07/2011</remarks>
    Private Sub PrepareStandByDoneMode()
        Try
            If MyBase.SimulationMode Then
                AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY '#REFACTORING
                MyBase.myServiceMDI.bsAnalyzerStatus.Text = AnalyzerController.Instance.Analyzer.AnalyzerStatus.ToString '#REFACTORING
            End If

            MyBase.myServiceMDI.isWaitingForStandBy = False 'SGM 15/11/2011

            PrepareAdjustReadingMode()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStandByDoneMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareStandByDoneMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Reading Analyzer Info
    ''' </summary>
    ''' <remarks>Created by XBC 31/05/2011</remarks>
    Private Sub PrepareReadingAnalyzerInfoMode()
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()

            myGlobal = MyBase.ReadAnalyzerInfo()
            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            Else
                Me.Cursor = Cursors.WaitCursor
                ' Initializations
                myScreenDelegate.ReadAnalyzerInfoDone = False

                MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                Me.ProgressBar1.Maximum = myScreenDelegate.MaxPOLLHW
                Me.ProgressBar1.Value = 0

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    Me.ProgressBar1.Visible = True
                    For i As Integer = 1 To myScreenDelegate.MaxPOLLHW
                        Me.ProgressBar1.Value = i
                        Me.ProgressBar1.Refresh()
                        'System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyClass.WaitProcessSimulation() 'SGM 04/01/2012
                    Next
                    myScreenDelegate.ReadAnalyzerInfoDone = True
                    PrepareAnalyzerInfoReadedMode()
                Else
                    If AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then '#REFACTORING
                        myGlobal = myScreenDelegate.REQUEST_ANALYZER_INFO()
                    Else
                        Me.PrepareErrorMode()
                    End If
                End If

                If myGlobal.HasError Then
                    Me.PrepareErrorMode()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareReadingAnalyzerInfoMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareReadingAnalyzerInfoMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Analyzer Info Readed Mode
    ''' </summary>
    ''' <remarks>Created by XBC 31/05/2011</remarks>
    Private Sub PrepareAnalyzerInfoReadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            If myScreenDelegate.ReadAnalyzerInfoDone Then

                Me.Cursor = Cursors.Default

                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)

                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()

                PrepareLoadedMode()

            Else
                ' Continue with the Reading until finish it
                Me.Cursor = Cursors.WaitCursor

                Me.ProgressBar1.Visible = True
                Me.ProgressBar1.Value = myScreenDelegate.NumPOLLHW
                Me.ProgressBar1.Refresh()

                myResultData = MyBase.ReadAnalyzerInfo()
                If myResultData.HasError Then
                    PrepareErrorMode()
                    Exit Sub
                Else
                    myResultData = myScreenDelegate.REQUEST_ANALYZER_INFO()
                End If

            End If

            If myResultData.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAnalyzerInfoReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareAnalyzerInfoReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Initializating Analyzer Mode
    ''' </summary>
    ''' <remarks>Created by XBC 25/05/2011</remarks>
    Private Sub PrepareInitiatingAnalyzerMode()
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()
            ' Initializating Analyzer
            InitAnalyzer()

            Cursor = Cursors.WaitCursor
            ' Manage FwScripts must to be sent at load screen
            If MyBase.SimulationMode Then
                ' simulating...
                myScreenDelegate.ReadAnalyzerInfoDone = True
                MyClass.TestProcessTimer.Interval = 5000 ' 5 seconds
                MyClass.TestProcessTimer.Enabled = True

            Else
                If AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING

                    ' XBC 13/10/2011 - wfPreload object not exist !
                    ''SGM 29/09/2011
                    'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                    'Dim myText1 As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SETTING_STANDBY", currentLanguage)
                    'Me.wfPreload.Title = myText1
                    ''end SGM 29/09/2011

                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                        myGlobal = MyBase.myServiceMDI.SEND_STANDBY
                        GlobalBase.CreateLogActivity("Send Standby ", Me.Name & ".PrepareInitiatingAnalyzerMode ", EventLogEntryType.Information, False)
                    End If
                ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then '#REFACTORING
                    MyClass.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE
                    PrepareArea()
                Else
                    Me.PrepareErrorMode()
                End If
            End If

            If myGlobal.HasError Then
                Me.PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareInitiatingAnalyzerMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareInitiatingAnalyzerMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Cursor = Cursors.Default
    End Sub

    Private Sub PrepareWaitingMode()
        Try
            Application.DoEvents()
            While Me.myWaitingTime
                Application.DoEvents()
            End While
            Application.DoEvents()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWaitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWaitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 25/05/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try
            Me.Cursor = Cursors.Default

            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                ' Operator User level can't change Serial number
                Me.bsEditSNButton.Visible = False
                Me.bsSaveSNButton.Visible = False
            Else
                Me.bsEditSNButton.Visible = True
                Me.bsEditSNButton.Enabled = True
            End If

            If Not MyClass.IsInfoAlreadyWritten Then
                If MyBase.SimulationMode Then
                    WriteInfoSimulatedToScreen()
                Else
                    CreateAnalyzerInfoFileOutput()
                    WriteInfoToScreen()
                End If
                MyClass.IsInfoAlreadyWritten = True
            End If

            Me.bsEditSNButton.Enabled = True
            Me.bsSaveSNButton.Enabled = MyClass.IsEditingSN
            Me.BsRichTextBox1.Enabled = True
            Me.bsPrintButton.Enabled = True
            Me.DetailsButton.Enabled = True
            Me.MoreInfoButton.Enabled = True
            Me.BsExitButton.Enabled = True

            If Not Ax00ServiceMainMDI.IsAnalyzerInitiated Then
                Me.bsEditSNButton.Enabled = False
                Me.BsExitButton.Enabled = False
            End If


            If Me.FirstScreenLoad Then
                Me.FirstScreenLoad = False
                Me.WindowState = FormWindowState.Normal
                Me.EnableMoveWindow = True
                'Me.TopMost = True
                Me.Show()
                Me.Location = New Point(260, 80) ' Me.Location.X - 150, Me.Location.Y - 250)
                Me.EnableMoveWindow = False
                ShowDetails(Me.Details)
                Ax00ServiceMainMDI.IsAutoConnecting = False 'SGM 17/11/2011
            End If

            If Me.bsSerialTextBox.Text.Length > 0 Then
                If IsNumeric(Me.bsSerialTextBox.Text) Then
                    If CLng(Me.bsSerialTextBox.Text) > 0 OrElse MyBase.SimulationMode Then 'SGM 12/07/2012
                        Me.InitApplicationAllowed = True
                    End If
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 30/05/2011</remarks>
    Private Sub PrepareSavingMode()
        Try
            MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)   ' PDT message for SAVING SERIAL NUMBER ...
            DisableAll()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSavingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 30/05/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myResultData As New GlobalDataTO
        Try
            Me.Cursor = Cursors.Default
            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
            Me.bsEditSNButton.Visible = True
            Me.bsEditSNButton.Enabled = True
            Me.BsRichTextBox1.Enabled = True
            Me.bsPrintButton.Enabled = True
            Me.DetailsButton.Enabled = True
            Me.MoreInfoButton.Enabled = True
            Me.BsExitButton.Enabled = True
            Me.bsSaveSNButton.Visible = False
            Me.bsCancelSNButton.Visible = False

            Me.InitApplicationAllowed = True

            Me.myServiceMDI.UpdatePreliminaryHomesMasterData(Nothing, myScreenDelegate.SerialNumber)
            ' Generate data content for adjustments before read them from the Instrument
            Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
            myResultData = myAdjustmentsDelegate.CopyCurrentAdjustments(Nothing, AnalyzerController.Instance.Analyzer.ActiveAnalyzer, myScreenDelegate.SerialNumber) '#REFACTORING

            If Not myResultData.HasError Then
                MyClass.PrepareLoadingMode()
            Else
                ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 15/10/2012</remarks>
    Private Sub PrepareSNSavingMode()
        Try
            MyBase.DisplayMessage(Messages.SRV_SAVE_GENERAL_CHANGES.ToString)   ' PDT message for SAVING SERIAL NUMBER ...
            DisableAll()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSNSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSNSavingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 15/10/2012</remarks>
    Private Sub PrepareSNSavedMode()
        Dim myResultData As New GlobalDataTO
        Try
            Me.Cursor = Cursors.Default
            MyBase.DisplayMessage(Messages.SRV_SAVE_OK.ToString)
            Me.bsEditSNButton.Visible = True
            Me.bsEditSNButton.Enabled = True
            Me.BsRichTextBox1.Enabled = True
            Me.bsPrintButton.Enabled = True
            Me.DetailsButton.Enabled = True
            Me.MoreInfoButton.Enabled = True
            Me.BsExitButton.Enabled = True
            Me.bsSaveSNButton.Visible = False
            Me.bsCancelSNButton.Visible = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSNSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareSNSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub WaitingControl(ByVal pText1 As String, ByVal pText2 As String)
        Try
            Application.DoEvents()
            bwPreload.RunWorkerAsync()
            Application.DoEvents()

            'DL 18/04/2012
            'Dim myBackground As String = IconsPath

            'wfPreload = New IAx00StartUp(Nothing) _
            '    With { _
            '        .Title = pText1, _
            '        .WaitText = pText2, _
            '        .Background = myBackground & "Embedded\ServiceSplash.png" _
            '    }

            wfPreload = New UiWaitScreen(Nothing) _
                With { _
                    .Title = pText1, _
                    .WaitText = pText2 _
                }

            'DL 18/04/2012

            wfPreload.Show()
            wfPreload.Focus()
            Application.DoEvents()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WaitingControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Modified by: IT 23/10/2014 - REFACTORING (BA-2016)</remarks>
    Private Sub WriteInfoToScreen()
        Dim myGlobal As New GlobalDataTO
        Dim validationOK As Boolean
        Dim textError As String
        Try
            validationOK = True

            Me.BsRichTextBox1.Clear()

            AppendRegularText(Me.BsRichTextBox1, "")
            AppendRegularText(Me.BsRichTextBox1, "Analyzer Information" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "==============" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

            '
            ' GENERAL CPU
            '

            ' Analyzer Serial number
            ' XBC 07/06/2012
            'Me.bsSerialTextBox.Text = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.CPU).AnalyzerSerialNumber.ToString
            Me.bsModelTextBox.Text = AnalyzerController.Instance.Analyzer.GetUpperPartSN(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)
            Me.bsSerialTextBox.Text = AnalyzerController.Instance.Analyzer.GetLowerPartSN(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)
            Me.bsFwVersionLabel2.Text = AnalyzerController.Instance.Analyzer.ActiveFwVersion
            ' XBC 07/06/2012

            AppendRegularText(Me.BsRichTextBox1, "Initializating Program CPU " + vbCrLf)
            myGlobal = myScreenDelegate.CheckCPU()
            If myGlobal.HasError Then
                validationOK = False
                textError = CType(myGlobal.SetDatos, String)
                AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
            Else
                AppendRegularText(Me.BsRichTextBox1, "Program Checksum CPU: OK " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Diagnostics: OK " + vbCrLf)
                'AppendRegularText(Me.BsRichTextBox1, "Buzzer Status: OK " + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "ISE Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE).Value.ToString + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CPU Initializated " + vbCrLf)

            End If


            'NOT TO WRITE NOTHING ABOUT ARMS UNTIL ANSF IS AVAILABLE!!!!
            If DateTime.Now = Nothing Then
                '
                ' ARMS
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Initializating Arms Motors " + vbCrLf)
                myGlobal = myScreenDelegate.CheckARMS()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLE Arm: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT1 Arm: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT2 Arm: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "MIXER1 Arm: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "MIXER2 Arm: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Arm Motors Initializated " + vbCrLf)
                End If

                '
                ' PROBES
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Programming sensibility of level detection " + vbCrLf)
                myGlobal = myScreenDelegate.CheckPROBES()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT1: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT2: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLE: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Sensibility is programmed " + vbCrLf)
                End If

                '
                ' ROTORS
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Initializating Rotors " + vbCrLf)
                myGlobal = myScreenDelegate.CheckROTORS()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "REAGENTS: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLES: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Rotors Initializated " + vbCrLf)
                End If

                '
                ' PHOTOMETRIC
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Optics Initialization " + vbCrLf)
                myGlobal = myScreenDelegate.CheckPHOTOMETRICS()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                    AppendErrorText(Me.BsRichTextBox1, "Optics Not Initializated!" + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "PHOTOMETRY TEST: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Optics Initializated " + vbCrLf)
                End If

                '
                ' MANIFOLD
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Manifold System Initialization " + vbCrLf)
                myGlobal = myScreenDelegate.CheckMANIFOLD()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                    AppendErrorText(Me.BsRichTextBox1, "Manifold System Not Initializated!" + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "MANIFOLD TEST: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Manifold System Initializated " + vbCrLf)
                End If

                '
                ' FLUIDICS
                '

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Fluidics System Initialization " + vbCrLf)
                myGlobal = myScreenDelegate.CheckFLUIDICS()
                If myGlobal.HasError Then
                    validationOK = False
                    textError = CType(myGlobal.SetDatos, String)
                    AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
                    AppendErrorText(Me.BsRichTextBox1, "Fluidics System Not Initializated!" + vbCrLf)
                Else
                    AppendRegularText(Me.BsRichTextBox1, "FLUIDICS TEST: OK " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Fluidics System Initializated " + vbCrLf)
                End If


                '
                ' ADDITIONAL INFO ************************************************************************************************************
                '
                Dim myArmInfo As UIRefreshDS.ArmValueChangedRow
                Dim myProbeInfo As UIRefreshDS.ProbeValueChangedRow
                Dim myRotorInfo As UIRefreshDS.RotorValueChangedRow

                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_TEMP) IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "HARDWARE INFORMATION : " + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "CPU : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_TEMP).Value.ToString + vbCrLf)
                End If
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BM1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Sample Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BM1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent1 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent2 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Mixer1 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Mixer2 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DM1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Sample Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DM1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent1 Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent2 Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RR1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagents Rotor: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RR1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RM1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Samples Rotor: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RM1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_GLF) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Photometric: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_GLF).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_SF1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Fluidics: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_SF1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_JE1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "CAN BUS Manifold: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_JE1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_MC) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "MAIN COVER: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_MC).Value.ToString + vbCrLf)
                'If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BUZ) IsNot Nothing Then _
                '    AppendRegularText(Me.BsRichTextBox1, "BUZZER Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BUZ).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_FWFM) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Firmware Repository Flash Memory: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_FWFM).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BBFM) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Black Box Flash Memory: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BBFM).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "ISE Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISEE) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "ISE Error: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISEE).Value.ToString + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)



                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BM1)
                If myArmInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "ARMS : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLE ARM : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myArmInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: " + myArmInfo.MotorVertical.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BR1)
                If myArmInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT1 ARM : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myArmInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: " + myArmInfo.MotorVertical.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BR2)
                If myArmInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT2 ARM : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myArmInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: " + myArmInfo.MotorVertical.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.AG1)
                If myArmInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "MIXER1 ARM : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myArmInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: " + myArmInfo.MotorVertical.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.AG2)
                If myArmInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "MIXER2 ARM : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myArmInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: " + myArmInfo.MotorVertical.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DR1)
                If myProbeInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "PROBES : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT1  : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myProbeInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection status: " + myProbeInfo.DetectionStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection: " + myProbeInfo.Detection.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Thermistor Value: " + myProbeInfo.ThermistorValue.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Thermistor Diagnostic: " + myProbeInfo.ThermistorDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Heater Status: " + myProbeInfo.HeaterStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Heater Diagnostic: " + myProbeInfo.HeaterDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Collision Detector: " + myProbeInfo.CollisionDetector.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DR2)
                If myProbeInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENT2  : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myProbeInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection status: " + myProbeInfo.DetectionStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection: " + myProbeInfo.Detection.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Thermistor Value: " + myProbeInfo.ThermistorValue.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Thermistor Diagnostic: " + myProbeInfo.ThermistorDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Heater Status: " + myProbeInfo.HeaterStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Heater Diagnostic: " + myProbeInfo.HeaterDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Collision Detector: " + myProbeInfo.CollisionDetector.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DM1)
                If myProbeInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLE  : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myProbeInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection status: " + myProbeInfo.DetectionStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Detection: " + myProbeInfo.Detection.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Collision Detector: " + myProbeInfo.CollisionDetector.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myRotorInfo = myScreenDelegate.InfoRotor(GlobalEnumerates.POLL_IDs.RR1)
                If myRotorInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "ROTORS : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "REAGENTS ROTOR Info : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myRotorInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor: " + myRotorInfo.Motor.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor Home: " + myRotorInfo.MotorHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor Position: " + myRotorInfo.MotorPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Fridge Thermistor Value: " + myRotorInfo.ThermistorFridgeValue.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Fridge Thermistor Diagnostic: " + myRotorInfo.ThermistorFridgeDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Fridge Peltiers Status: " + myRotorInfo.PeltiersFridgeStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Fridge Peltiers Diagnostic: " + myRotorInfo.PeltiersFridgeDiagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 1 Speed: " + myRotorInfo.PeltiersFan1Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 1 Diagnostic: " + myRotorInfo.PeltiersFan1Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 2 Speed: " + myRotorInfo.PeltiersFan2Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 2 Diagnostic: " + myRotorInfo.PeltiersFan2Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 3 Speed: " + myRotorInfo.PeltiersFan3Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 3 Diagnostic: " + myRotorInfo.PeltiersFan3Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 4 Speed: " + myRotorInfo.PeltiersFan4Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 5 Diagnostic: " + myRotorInfo.PeltiersFan4Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Frame Fan 1 Speed: " + myRotorInfo.FrameFan1Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Frame Fan 1 Diagnostic: " + myRotorInfo.FrameFan1Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Frame Fan 2 Speed: " + myRotorInfo.FrameFan2Speed.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Frame Fan 2 Diagnostic: " + myRotorInfo.FrameFan2Diagnostic.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Cover: " + myRotorInfo.Cover.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Barcode Status: " + myRotorInfo.BarCodeStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Barcode Error: " + myRotorInfo.BarcodeError.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If

                myRotorInfo = myScreenDelegate.InfoRotor(GlobalEnumerates.POLL_IDs.RM1)
                If myRotorInfo IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "SAMPLES ROTOR Info : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myRotorInfo.BoardTemp.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor: " + myRotorInfo.Motor.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor Home: " + myRotorInfo.MotorHome.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Motor Position: " + myRotorInfo.MotorPosition.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Cover: " + myRotorInfo.Cover.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Barcode Status: " + myRotorInfo.BarCodeStatus.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "Barcode Error: " + myRotorInfo.BarcodeError.ToString + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                End If


                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_TEMP) IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "PHOTOMETRIC : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_TEMP).Value.ToString + vbCrLf)
                End If
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MR) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Motor: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MR).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Home: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRA) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Position: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRA).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRE) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Encoder: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRE).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRED) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRED).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MW) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MW).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Home: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWA) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Position: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWA).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_CD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Collision Detector: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_CD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Thermistor Value: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTHD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Thermistor Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTHD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Peltier Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Peltier Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan1 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan1 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan2 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan2 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan3 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan3 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan4 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Peltier Fan4 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_RC) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Rotor Cover: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_RC).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHT) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Photometry Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHT).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHFM) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Photometry Flash Memory Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHFM).Value.ToString + vbCrLf)

                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_TEMP) IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "FLUIDICS : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_TEMP).Value.ToString + vbCrLf)
                End If
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Home: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSA) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Position: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSA).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1/Mixer2 Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1/Mixer2 Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2/Mixer1 Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2/Mixer1 Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Low Contamination output Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Low Contamination output Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 2,3 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 2,3 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 4,5 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 4,5 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 6 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 6 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 7 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 7 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 1 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 1 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Dispensation valves group State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Dispensation valves group Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external source) input valve: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external source) input valve Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input valve: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input valve Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater Thermistor Value: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTHD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Thermistor Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTHD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater state: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSHD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSHD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSW) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Solution Weight: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSW).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSWD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Washing Solution Weight Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSWD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCW) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "High Contamination Weight : " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCW).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCWD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "High Contamination Weight Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCWD).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WAS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Waste Sensor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WAS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WTS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Waste Tank Status: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WTS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_SLS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "System Liquid Sensor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_SLS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_STS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "System Liquid Tank State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_STS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Stirrer 1 State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Stirrer 2 State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST2).Value.ToString + vbCrLf)

                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_TEMP) IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "MANIFOLD : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_TEMP).Value.ToString + vbCrLf)
                End If
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MS) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Sample Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MS).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSH) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Home Motor Sample: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSH).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSA) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Motor Sample Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSA).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent 1 Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1H) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Home Motor Reagent 1: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1H).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1A) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Motor Reagent 1 Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1A).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent 2 Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2H) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Home Motor Reagent 2: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2H).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2A) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Motor Reagent 2 Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2A).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Samples dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Air OR Washing Solution Valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Air OR Washing Solution Valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Air/Washing OR Purified Water Solution Valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5D) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Air/Washing OR Purified Water Solution Valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5D).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLT) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Clot Sensor Value: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLT).Value.ToString + vbCrLf)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLTD) IsNot Nothing Then _
                    AppendRegularText(Me.BsRichTextBox1, "Clot Sensor Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLTD).Value.ToString + vbCrLf)

            End If

            '
            ' ****************************************************************************************************************************
            '

            ' Set text to Print
            myTextToPrint = Me.BsRichTextBox1.Text

            If validationOK Then
                MyBase.DisplayMessage(Messages.SRV_ANALYZER_OK.ToString)
            Else
                MyBase.DisplayMessage(Messages.SRV_ANALYZER_KO.ToString)
                Me.Details = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WriteInfoToScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WriteInfoToScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Public Sub AppendRegularText(ByVal RTC As RichTextBox, ByVal text As String)
        Try
            If RTC IsNot Nothing Then
                With RTC
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.SelectionFont, FontStyle.Regular)
                    .SelectionColor = Color.Black
                    .AppendText(text)
                End With
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AppendRegularText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AppendRegularText ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Public Sub AppendErrorText(ByVal RTC As RichTextBox, ByVal text As String)
        Try
            If RTC IsNot Nothing Then
                With RTC
                    .Select(.TextLength, 0)
                    .SelectionFont = New Font(.SelectionFont, FontStyle.Bold)
                    .SelectionColor = Color.Red
                    .AppendText(text)
                End With
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ". AppendErrorText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ". AppendErrorText ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CreateAnalyzerInfoFileOutput()
        Dim myPath As String
        'Dim myGlobalbase As New GlobalBase
        Dim myFWInfo As UIRefreshDS.FirmwareValueChangedRow
        Dim myArmInfo As UIRefreshDS.ArmValueChangedRow
        Dim myProbeInfo As UIRefreshDS.ProbeValueChangedRow
        Dim myRotorInfo As UIRefreshDS.RotorValueChangedRow
        Try
            ' Write Results into output file
            myPath = Application.StartupPath & GlobalBase.AnalyzerInfoFileOut

            If File.Exists(myPath) Then File.Delete(myPath)
            Dim myStreamWriter As StreamWriter = File.CreateText(myPath)

            myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.CPU)
            If myFWInfo IsNot Nothing Then
                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("Serial Number Analyzer : " + myFWInfo.AnalyzerSerialNumber.ToString)
                myStreamWriter.WriteLine("")


                ' FIRMWARE *******************************************************************************************************************

                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("FIRMWARE INFORMATION : ")

                myStreamWriter.WriteLine("")
                myStreamWriter.WriteLine("CPU : ")
                myStreamWriter.WriteLine("")

                myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                myStreamWriter.WriteLine("")



                'NOT TO WRITE NOTHING ABOUT ARMS UNTIL ANSF IS AVAILABLE!!!!
                If DateTime.Now = Nothing Then

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("ARMS : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.BM1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("SAMPLE ARM : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.BR1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT1 ARM : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.BR2)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT2 ARM : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.AG1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MIXER1 ARM : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.AG2)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MIXER2 ARM : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("PROBES : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.DR1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT 1 : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.DR2)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT 2 : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.DM1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("SAMPLE : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("ROTORS : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.RR1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENTS : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.RM1)
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("SAMPLES : ")
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("PHOTOMETRICS : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.GLF)
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MANIFOLD : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.JE1)
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("FLUIDICS : ")
                    myStreamWriter.WriteLine("")

                    myFWInfo = myScreenDelegate.InfoFirmware(GlobalEnumerates.POLL_IDs.SF1)
                    myStreamWriter.WriteLine("Board Serial Number: " + myFWInfo.BoardSerialNumber.ToString)
                    myStreamWriter.WriteLine("Firmware Version: " + myFWInfo.BoardFirmwareVersion.ToString)
                    myStreamWriter.WriteLine("CRC32 Result: " + myFWInfo.BoardFirmwareCRCResult.ToString)
                    myStreamWriter.WriteLine("CRC32 Value: " + myFWInfo.BoardFirmwareCRCValue.ToString)
                    myStreamWriter.WriteLine("CRC32 Size: " + myFWInfo.BoardFirmwareCRCSize.ToString)
                    myStreamWriter.WriteLine("Hardware Version: " + myFWInfo.BoardHardwareVersion.ToString)
                    myStreamWriter.WriteLine("")
                End If
            End If

            'NOT TO WRITE NOTHING ABOUT ARMS UNTIL ANS IS AVAILABLE!!!!
            If DateTime.Now = Nothing Then
                '
                ' HARDWARE *******************************************************************************************************************
                '
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_TEMP) IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("HARDWARE INFORMATION : ")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("CPU : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("Board Temperature: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_TEMP).Value.ToString)
                End If
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BM1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Sample Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BM1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Reagent1 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Reagent2 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_BR2).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Mixer1 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Mixer2 Arm: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_AG2).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DM1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Sample Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DM1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Reagent1 Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Reagent2 Probe: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_DR2).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RR1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Reagents Rotor: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RR1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RM1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Samples Rotor: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_RM1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_GLF) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Photometric: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_GLF).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_SF1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Fluidics: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_SF1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_JE1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("CAN BUS Manifold: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_CAN_JE1).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_MC) IsNot Nothing Then _
                    myStreamWriter.WriteLine("MAIN COVER: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_MC).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BUZ) IsNot Nothing Then _
                    myStreamWriter.WriteLine("BUZZER Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BUZ).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_FWFM) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Firmware Repository Flash Memory: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_FWFM).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BBFM) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Black Box Flash Memory: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_BBFM).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE) IsNot Nothing Then _
                    myStreamWriter.WriteLine("ISE Status: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISE).Value.ToString)
                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISEE) IsNot Nothing Then _
                    myStreamWriter.WriteLine("ISE Error: " + myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_ISEE).Value.ToString)
                myStreamWriter.WriteLine("")

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BM1)
                If myArmInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("ARMS : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("SAMPLE ARM : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myArmInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString)
                    myStreamWriter.WriteLine("Vertical Motor: " + myArmInfo.MotorVertical.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BR1)
                If myArmInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT1 ARM : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myArmInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString)
                    myStreamWriter.WriteLine("Vertical Motor: " + myArmInfo.MotorVertical.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.BR2)
                If myArmInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT2 ARM : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myArmInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString)
                    myStreamWriter.WriteLine("Vertical Motor: " + myArmInfo.MotorVertical.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.AG1)
                If myArmInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MIXER1 ARM : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myArmInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString)
                    myStreamWriter.WriteLine("Vertical Motor: " + myArmInfo.MotorVertical.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myArmInfo = myScreenDelegate.InfoArm(GlobalEnumerates.POLL_IDs.AG2)
                If myArmInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MIXER2 ARM : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myArmInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor: " + myArmInfo.MotorHorizontal.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Home: " + myArmInfo.MotorHorizontalHome.ToString)
                    myStreamWriter.WriteLine("Horizontal Motor Position: " + myArmInfo.MotorHorizontalPosition.ToString)
                    myStreamWriter.WriteLine("Vertical Motor: " + myArmInfo.MotorVertical.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Home: " + myArmInfo.MotorVerticalHome.ToString)
                    myStreamWriter.WriteLine("Vertical Motor Position: " + myArmInfo.MotorVerticalPosition.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DR1)
                If myProbeInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("PROBES : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT1  : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myProbeInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Detection status: " + myProbeInfo.DetectionStatus.ToString)
                    myStreamWriter.WriteLine("Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString)
                    myStreamWriter.WriteLine("Detection: " + myProbeInfo.Detection.ToString)
                    myStreamWriter.WriteLine("Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString)
                    myStreamWriter.WriteLine("Thermistor Value: " + myProbeInfo.ThermistorValue.ToString)
                    myStreamWriter.WriteLine("Thermistor Diagnostic: " + myProbeInfo.ThermistorDiagnostic.ToString)
                    myStreamWriter.WriteLine("Heater Status: " + myProbeInfo.HeaterStatus.ToString)
                    myStreamWriter.WriteLine("Heater Diagnostic: " + myProbeInfo.HeaterDiagnostic.ToString)
                    myStreamWriter.WriteLine("Collision Detector: " + myProbeInfo.CollisionDetector.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DR2)
                If myProbeInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENT2  : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myProbeInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Detection status: " + myProbeInfo.DetectionStatus.ToString)
                    myStreamWriter.WriteLine("Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString)
                    myStreamWriter.WriteLine("Detection: " + myProbeInfo.Detection.ToString)
                    myStreamWriter.WriteLine("Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString)
                    myStreamWriter.WriteLine("Thermistor Value: " + myProbeInfo.ThermistorValue.ToString)
                    myStreamWriter.WriteLine("Thermistor Diagnostic: " + myProbeInfo.ThermistorDiagnostic.ToString)
                    myStreamWriter.WriteLine("Heater Status: " + myProbeInfo.HeaterStatus.ToString)
                    myStreamWriter.WriteLine("Heater Diagnostic: " + myProbeInfo.HeaterDiagnostic.ToString)
                    myStreamWriter.WriteLine("Collision Detector: " + myProbeInfo.CollisionDetector.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myProbeInfo = myScreenDelegate.InfoProbe(GlobalEnumerates.POLL_IDs.DM1)
                If myProbeInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("SAMPLE  : ")
                    myStreamWriter.WriteLine("Board Temperature: " + myProbeInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Detection status: " + myProbeInfo.DetectionStatus.ToString)
                    myStreamWriter.WriteLine("Detection Frequency: " + myProbeInfo.DetectionFrequency.ToString)
                    myStreamWriter.WriteLine("Detection: " + myProbeInfo.Detection.ToString)
                    myStreamWriter.WriteLine("Last Internal Coeficient: " + myProbeInfo.LastInternalRate.ToString)
                    myStreamWriter.WriteLine("Collision Detector: " + myProbeInfo.CollisionDetector.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myRotorInfo = myScreenDelegate.InfoRotor(GlobalEnumerates.POLL_IDs.RR1)
                If myRotorInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("ROTORS : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("REAGENTS ROTOR Info : ")
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("Board Temperature: " + myRotorInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Motor: " + myRotorInfo.Motor.ToString)
                    myStreamWriter.WriteLine("Motor Home: " + myRotorInfo.MotorHome.ToString)
                    myStreamWriter.WriteLine("Motor Position: " + myRotorInfo.MotorPosition.ToString)
                    myStreamWriter.WriteLine("Fridge Thermistor Value: " + myRotorInfo.ThermistorFridgeValue.ToString)
                    myStreamWriter.WriteLine("Fridge Thermistor Diagnostic: " + myRotorInfo.ThermistorFridgeDiagnostic.ToString)
                    myStreamWriter.WriteLine("Fridge Peltiers Status: " + myRotorInfo.PeltiersFridgeStatus.ToString)
                    myStreamWriter.WriteLine("Fridge Peltiers Diagnostic: " + myRotorInfo.PeltiersFridgeDiagnostic.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 1 Speed: " + myRotorInfo.PeltiersFan1Speed.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 1 Diagnostic: " + myRotorInfo.PeltiersFan1Diagnostic.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 2 Speed: " + myRotorInfo.PeltiersFan2Speed.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 2 Diagnostic: " + myRotorInfo.PeltiersFan2Diagnostic.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 3 Speed: " + myRotorInfo.PeltiersFan3Speed.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 3 Diagnostic: " + myRotorInfo.PeltiersFan3Diagnostic.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 4 Speed: " + myRotorInfo.PeltiersFan4Speed.ToString)
                    myStreamWriter.WriteLine("Peltier Fan 5 Diagnostic: " + myRotorInfo.PeltiersFan4Diagnostic.ToString)
                    myStreamWriter.WriteLine("Frame Fan 1 Speed: " + myRotorInfo.FrameFan1Speed.ToString)
                    myStreamWriter.WriteLine("Frame Fan 1 Diagnostic: " + myRotorInfo.FrameFan1Diagnostic.ToString)
                    myStreamWriter.WriteLine("Frame Fan 2 Speed: " + myRotorInfo.FrameFan2Speed.ToString)
                    myStreamWriter.WriteLine("Frame Fan 2 Diagnostic: " + myRotorInfo.FrameFan2Diagnostic.ToString)
                    myStreamWriter.WriteLine("Cover: " + myRotorInfo.Cover.ToString)
                    myStreamWriter.WriteLine("Barcode Status: " + myRotorInfo.BarCodeStatus.ToString)
                    myStreamWriter.WriteLine("Barcode Error: " + myRotorInfo.BarcodeError.ToString)
                    myStreamWriter.WriteLine("")
                End If

                myRotorInfo = myScreenDelegate.InfoRotor(GlobalEnumerates.POLL_IDs.RM1)
                If myRotorInfo IsNot Nothing Then
                    myStreamWriter.WriteLine("SAMPLES ROTOR Info : ")
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("Board Temperature: " + myRotorInfo.BoardTemp.ToString)
                    myStreamWriter.WriteLine("Motor: " + myRotorInfo.Motor.ToString)
                    myStreamWriter.WriteLine("Motor Home: " + myRotorInfo.MotorHome.ToString)
                    myStreamWriter.WriteLine("Motor Position: " + myRotorInfo.MotorPosition.ToString)
                    myStreamWriter.WriteLine("Cover: " + myRotorInfo.Cover.ToString)
                    myStreamWriter.WriteLine("Barcode Status: " + myRotorInfo.BarCodeStatus.ToString)
                    myStreamWriter.WriteLine("Barcode Error: " + myRotorInfo.BarcodeError.ToString)
                    myStreamWriter.WriteLine("")
                End If


                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_TEMP) IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("PHOTOMETRIC : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("Board Temperature: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_TEMP).Value.ToString)
                End If
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MR) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Motor: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MR).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Motor Home: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRH).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRA) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Motor Position: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRA).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRE) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Motor Encoder: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRE).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRED) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Motor Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MRED).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MW) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Motor: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MW).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Motor Home: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWH).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWA) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Position: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_MWA).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_CD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Collision Detector: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_CD).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Thermistor Value: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTH).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTHD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Thermistor Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PTHD).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Peltier Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PH).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Peltier Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHD).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan1 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan1 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF1D).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan2 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan2 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF2D).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan3 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan3 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF3D).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan4 Speed: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Peltier Fan4 Diagnostic: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PF4D).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_RC) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Rotor Cover: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_RC).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHT) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Photometry Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHT).Value.ToString)
                If myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHFM) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Photometry Flash Memory Status: " + myScreenDelegate.InfoPhotometrics(PHOTOMETRICS_ELEMENTS.GLF_PHFM).Value.ToString)

                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_TEMP) IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("FLUIDICS : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("Board Temperature: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_TEMP).Value.ToString)
                End If
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Motor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MS).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Motor Home: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSH).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSA) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Motor Position: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_MSA).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B1D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1/Mixer2 Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1/Mixer2 Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B2D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2/Mixer1 Needle External Washing Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2/Mixer1 Needle External Washing Pump Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B3D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external tank) input Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external tank) input Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B4D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Low Contamination output Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Low Contamination output Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B5D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 2,3 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 2,3 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B6D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 4,5 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 4,5 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B7D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 6 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 6 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B8D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 7 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 7 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B9D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 1 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station needle 1 Aspiration Pump: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_B10D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Dispensation valves group State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Dispensation valves group Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_GE1D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external source) input valve: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external source) input valve Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV1D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external tank) input valve: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Purified Water (external tank) input valve Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_EV2D).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Heater Thermistor Value: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTH).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTHD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Thermistor Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSTHD).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Heater state: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSH).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSHD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Station Heater Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSHD).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSW) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Solution Weight: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSW).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSWD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Washing Solution Weight Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WSWD).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCW) IsNot Nothing Then _
                    myStreamWriter.WriteLine("High Contamination Weight : " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCW).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCWD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("High Contamination Weight Diagnostic: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_HCWD).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WAS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Waste Sensor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WAS).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WTS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Waste Tank Status: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_WTS).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_SLS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("System Liquid Sensor: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_SLS).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_STS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("System Liquid Tank State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_STS).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Stirrer 1 State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST1).Value.ToString)
                If myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Stirrer 2 State: " + myScreenDelegate.InfoFluidics(FLUIDICS_ELEMENTS.SF1_ST2).Value.ToString)

                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_TEMP) IsNot Nothing Then
                    myStreamWriter.WriteLine("")
                    myStreamWriter.WriteLine("MANIFOLD : ")
                    myStreamWriter.WriteLine("")

                    myStreamWriter.WriteLine("Board Temperature: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_TEMP).Value.ToString)
                End If
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MS) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Sample Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MS).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSH) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Home Motor Sample: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSH).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSA) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Motor Sample Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MSA).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent 1 Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1H) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Home Motor Reagent 1: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1H).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1A) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Motor Reagent 1 Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR1A).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent 2 Motor: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2H) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Home Motor Reagent 2: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2H).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2A) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Motor Reagent 2 Position: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_MR2A).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B1D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1 dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1 dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B2D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2 dosing pump State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2 dosing pump Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_B3D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Samples dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV1D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1 dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent1 dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV2D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2 dosing valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Reagent2 dosing valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV3D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Air OR Washing Solution Valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Air OR Washing Solution Valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV4D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Air/Washing OR Purified Water Solution Valve State: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5D) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Air/Washing OR Purified Water Solution Valve Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_EV5D).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLT) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Clot Sensor Value: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLT).Value.ToString)
                If myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLTD) IsNot Nothing Then _
                    myStreamWriter.WriteLine("Clot Sensor Diagnostic: " + myScreenDelegate.InfoManifold(MANIFOLD_ELEMENTS.JE1_CLTD).Value.ToString)
            End If




            myStreamWriter.Close()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CreateAnalyzerInfoFileOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateAnalyzerInfoFileOutput ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ShowDetails(ByVal pVisible As Boolean)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If pVisible Then
                ' Hide Details
                Me.Size = New System.Drawing.Size(Window_Width_Details, Window_Height_Details)
                Me.BsExitButton.Location = New Point(ExitButtonLocationBottom_X, ExitButtonLocationBottom_Y)
                Me.DetailsButton.Text = "< " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HideDetails", currentLanguage)
            Else
                ' Show Details
                Me.Size = New System.Drawing.Size(Window_Width_Short, Window_Height_Short)
                Me.BsExitButton.Location = New Point(ExitButtonLocationTop_X, ExitButtonLocationTop_Y)
                Me.DetailsButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ShowDetails", currentLanguage) + " >"
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ShowDetails ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowDetails ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#Region "Simulate Mode"

    Private Sub WriteInfoSimulatedToScreen()
        'Dim myGlobal As New GlobalDataTO
        Dim validationOK As Boolean
        Dim textError As String = ""
        Try
            Dim Rnd As New Random()
            Dim errorRandom As Integer = Rnd.Next(1, 4)
            errorRandom = 1   ' By now, forced to Success always

            Me.BsRichTextBox1.Clear()

            validationOK = True

            AppendRegularText(Me.BsRichTextBox1, "")
            AppendRegularText(Me.BsRichTextBox1, "Analyzer Information" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "==============" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

            Application.DoEvents()
            '
            ' GENERAL CPU
            '
            ' Analyzer Serial number

            ' XBC 07/06/2012
            'Me.bsSerialTextBox.Text = "x.0000000000"
            'Me.bslModelLabel.Text = "842"
            'Me.bsSerialTextBox.Text = "000000000"
            '#REFACTORING
            Me.bsModelTextBox.Text = AnalyzerController.Instance.Analyzer.GetUpperPartSN(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)
            Me.bsSerialTextBox.Text = AnalyzerController.Instance.Analyzer.GetLowerPartSN(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)
            Me.bsFwVersionLabel2.Text = AnalyzerController.Instance.Analyzer.ActiveFwVersion
            ' XBC 07/06/2012


            AppendRegularText(Me.BsRichTextBox1, "Initializating Program CPU " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Program Checksum CPU: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "CAN BUS Diagnostics: OK " + vbCrLf)
            'AppendRegularText(Me.BsRichTextBox1, "Buzzer Status: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "ISE Status: 0 " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "CPU Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' ARMS
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Initializating Arms Motors " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "SAMPLE Arm: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "REAGENT1 Arm: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "REAGENT2 Arm: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "MIXER1 Arm: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "MIXER2 Arm: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Arm Motors Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' PROBES
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Programming sensibility of level detection " + vbCrLf)
            If errorRandom > 2 Then
                validationOK = False
                textError = "Error! Incorrect REAGENT2 PROBE Checksum"
                AppendErrorText(Me.BsRichTextBox1, textError + vbCrLf)
            Else
                AppendRegularText(Me.BsRichTextBox1, "REAGENT1: OK " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENT2: OK " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "SAMPLE: OK " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Sensibility is programmed " + vbCrLf)
            End If

            Application.DoEvents()
            '
            ' ROTORS
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Initializating Rotors " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "REAGENTS: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "SAMPLES: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Rotors Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' PHOTOMETRIC
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Optics Initialization " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "PHOTOMETRY TEST: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Optics Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' MANIFOLD
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Manifold System Initialization " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "MANIFOLD TEST: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Manifold System Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' FLUIDICS
            '
            AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Fluidics System Initialization " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "FLUIDICS TEST: OK " + vbCrLf)
            AppendRegularText(Me.BsRichTextBox1, "Fluidics System Initializated " + vbCrLf)

            Application.DoEvents()
            '
            ' ADDITIONAL INFO ************************************************************************************************************
            '
            If Me.RequestMoreInfo Then

                If myScreenDelegate.InfoCpu(CPU_ELEMENTS.CPU_TEMP) IsNot Nothing Then
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "HARDWARE INFORMATION : " + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "CPU : " + vbCrLf)
                    AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                    AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                End If

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Sample Arm: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent1 Arm: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent2 Arm: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Mixer1 Arm: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Mixer2 Arm: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Sample Probe: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent1 Probe: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagent2 Probe: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Reagents Rotor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Samples Rotor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Photometric: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Fluidics: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "CAN BUS Manifold: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "MAIN COVER: 0" + vbCrLf)
                'AppendRegularText(Me.BsRichTextBox1, "BUZZER Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Firmware Repository Flash Memory: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Black Box Flash Memory: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "ISE Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "ISE Error: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "ARMS : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "SAMPLE ARM : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENT1 ARM : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENT2 ARM : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "MIXER1 ARM : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "MIXER2 ARM : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Horizontal Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Vertical Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "PROBES : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENT1  : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Thermistor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Thermistor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Heater Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Heater Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Collision Detector: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENT2  : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Thermistor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Thermistor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Heater Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Heater Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Collision Detector: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "SAMPLE  : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection Frequency: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Detection: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Last Internal Coeficient: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Collision Detector: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "ROTORS : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "REAGENTS ROTOR Info : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Fridge Thermistor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Fridge Thermistor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Fridge Peltiers Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Fridge Peltiers Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 1 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 1 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 2 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 2 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 3 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 3 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 4 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan 5 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Frame Fan 1 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Frame Fan 1 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Frame Fan 2 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Frame Fan 2 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Cover: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Barcode Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Barcode Error: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "SAMPLES ROTOR Info : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Cover: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Barcode Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Barcode Error: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "PHOTOMETRIC : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Encoder: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Motor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Collision Detector: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Thermistor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Thermistor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Peltier Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Peltier Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan1 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan1 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan2 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan2 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan3 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan3 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan4 Speed: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Peltier Fan4 Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Rotor Cover: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Photometry Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Photometry Flash Memory Status: 0" + vbCrLf)

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "FLUIDICS : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Home: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Motor Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples Needle External Washing Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples Needle External Washing Pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1/Mixer2 Needle External Washing Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1/Mixer2 Needle External Washing Pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2/Mixer1 Needle External Washing Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2/Mixer1 Needle External Washing Pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Low Contamination output Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Low Contamination output Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 2,3 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 2,3 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 4,5 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 4,5 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 6 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 6 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 7 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 7 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 1 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station needle 1 Aspiration Pump: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Dispensation valves group State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Dispensation valves group Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external source) input valve: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external source) input valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input valve: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Purified Water (external tank) input valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater Thermistor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Thermistor Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater state: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Station Heater Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Solution Weight: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Washing Solution Weight Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "High Contamination Weight : 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "High Contamination Weight Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Waste Sensor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Waste Tank Status: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "System Liquid Sensor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "System Liquid Tank State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Stirrer 1 State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Stirrer 2 State: 0" + vbCrLf)

                Application.DoEvents()

                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "MANIFOLD : " + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "" + vbCrLf)

                AppendRegularText(Me.BsRichTextBox1, "Board Temperature: 37" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Sample Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Home Motor Sample: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Sample Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent 1 Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Home Motor Reagent 1: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Reagent 1 Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent 2 Motor: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Home Motor Reagent 2: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Motor Reagent 2 Position: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples dosing pump State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples dosing pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing pump State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing pump State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing pump Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples dosing valve State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Samples dosing valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing valve State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent1 dosing valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing valve State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Reagent2 dosing valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Air OR Washing Solution Valve State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Air OR Washing Solution Valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Air/Washing OR Purified Water Solution Valve State: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Air/Washing OR Purified Water Solution Valve Diagnostic: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Clot Sensor Value: 0" + vbCrLf)
                AppendRegularText(Me.BsRichTextBox1, "Clot Sensor Diagnostic: 0" + vbCrLf)
            End If

            ' Set text to Print
            myTextToPrint = Me.BsRichTextBox1.Text

            If validationOK Then
                MyBase.DisplayMessage(Messages.SRV_ANALYZER_OK.ToString)
            Else
                MyBase.DisplayMessage(Messages.SRV_ANALYZER_KO.ToString)
                Me.Details = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WriteInfoSimulatedToScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WriteInfoSimulatedToScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Events"

    Private Sub AnalyzerInfo_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        MyBase.MyBase_FormClosed(sender, e)
    End Sub

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub AnalyzerInfo_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else
                ' XBC 21/09/2012
                MyBase.myServiceMDI.IsAnalyzerInfoScreenRunning = False

                Me.TopMost = False
                If Not MyClass.myScreenDelegate Is Nothing Then
                    MyClass.myScreenDelegate.Dispose()
                    MyClass.myScreenDelegate = Nothing
                End If

                'SGM 28/02/2012
                MyBase.ActivateMDIMenusButtons(Not MyBase.CloseRequestedByMDI)
                MyBase.myServiceMDI.IsFinalClosing = MyBase.CloseRequestedByMDI

            End If

            '' XBC 09/02/2012
            'MyClass.myServiceMDI.ControlBox = True
            'MyClass.myServiceMDI.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            MyBase.myServiceMDI.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub Startup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        ''Dim myGlobalbase As New GlobalBase
        Try
            Ax00ServiceMainMDI.ActivateMenus(False)
            Me.Cursor = Cursors.WaitCursor


            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get the current user level
            'Dim CurrentUserLevel As String = ""
            'CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            'Dim myUsersLevel As New UsersLevelDelegate
            'If CurrentUserLevel <> "" Then  'When user level exists then find his numerical level
            '    myGlobal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
            '    If Not myGlobal.HasError Then
            '        MyBase.CurrentUserNumericalLevel = CType(myGlobal.SetDatos, Integer)

            MyBase.GetUserNumericalLevel()

            'screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New AnalyzerInfoDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)


            Initializations()

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels()

            PrepareButtons()

            DisableAll()


            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myGlobal.ErrorCode = "ERROR_COMM"
                myGlobal.HasError = True
            End If

            '    End If
            'End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ' XBC 28-06-2011 - Apparance is change
            'ResetBorderSRV()

            ' XBC 21/09/2012
            MyBase.myServiceMDI.IsAnalyzerInfoScreenRunning = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub AnalyzerInfo_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
            Me.Hide()
            Me.PrepareLoadingMode()
        Else
            Me.Close()
        End If
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            If Me.InitApplicationAllowed Then
                Me.Close()
            Else
                If (ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.LOAD_SERIAL_NUM.ToString) = Windows.Forms.DialogResult.Yes) Then
                    Me.Close()
                    Ax00ServiceMainMDI.SkipQuestionExitProgram = True
                    Ax00ServiceMainMDI.Close()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsExitButton.Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bwPreload_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwPreload.DoWork
        Try
            Me.myWaitingTime = True
            PrepareWaitingMode()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bwPreload_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bwPreload_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bwPreload_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwPreload.RunWorkerCompleted
        Try
            wfPreload.Close()

            'Ax00ServiceMainMDI.ActivateMenus(True)
            Ax00ServiceMainMDI.FwCompatible = myScreenDelegate.FwCompatible

            MyBase.ActivateMDIMenusButtons(True)

            If Not myScreenDelegate.FwCompatible Then

                'obtain needed fw version SGM 22/06/2012
                Dim myGlobal As New GlobalDataTO
                Dim mySwVersion As String
                ''Dim myUtil As New Utilities.
                myGlobal = Utilities.GetSoftwareVersion()
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    mySwVersion = myGlobal.SetDatos.ToString

                    myGlobal = AnalyzerController.Instance.Analyzer.GetFwVersionNeeded(mySwVersion) '#REFACTORING
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myNeededFwVersion As String = CStr(myGlobal.SetDatos)

                        Dim TextPar As New List(Of String)
                        TextPar.Add(myNeededFwVersion)

                        Dim myMsgList As New List(Of String)
                        myMsgList.Add(Messages.FW_VERSION_NOT_VALID.ToString)
                        If AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                            myMsgList.Add("")
                            myMsgList.Add(Messages.FW_UPDATE_NEEDED.ToString)
                        End If
                        ShowMultipleMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), myMsgList, Nothing, TextPar)

                    End If

                End If

                Ax00ServiceMainMDI.ActivateMenus(True)
                Ax00ServiceMainMDI.ActivateActionButtonBar(True)
                Ax00ServiceMainMDI.Cursor = Cursors.Default


                Ax00ServiceMainMDI.OpenInstrumentUpdateToolScreen(True)

                Me.Close()


            End If

            ' XBC 09/02/2012
            MyClass.myServiceMDI.ControlBox = True
            MyClass.myServiceMDI.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bwPreload_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bwPreload_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub TestProcessTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestProcessTimer.Tick
        Try
            MyClass.TestProcessTimer.Enabled = False
            MyClass.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE
            PrepareArea()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".TestProcessTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TestProcessTimer_Tick", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            PrintDocument1.DefaultPageSettings = OptPrnPage
            StrToPrn = myTextToPrint
            PrintDialog1.Document = PrintDocument1
            Dim DR As DialogResult = PrintDialog1.ShowDialog
            Application.DoEvents()
            If DR = DialogResult.OK Then
                PrintDocument1.Print()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Try
            Application.DoEvents()
            'PrintPage is the foundational printing event. This event gets fired for every 
            ' page that will be printed
            Static intCurrentChar As Int32
            ' declaring a static variable to hold the position of the last printed char
            Dim font As New Font("Verdana", 14)
            ' initializing the font to be used for printing
            Dim PrintAreaHeight, PrintAreaWidth, marginLeft, marginTop As Int32
            With PrintDocument1.DefaultPageSettings
                ' initializing local variables that contain the bounds of the printing area rectangle
                PrintAreaHeight = .PaperSize.Height - .Margins.Top - .Margins.Bottom
                PrintAreaWidth = .PaperSize.Width - .Margins.Left - .Margins.Right
                ' initializing local variables to hold margin values that will serve
                ' as the X and Y coordinates for the upper left corner of the printing 
                ' area rectangle.
                marginLeft = .Margins.Left
                marginTop = .Margins.Top
                ' X and Y coordinate
            End With

            If PrintDocument1.DefaultPageSettings.Landscape Then
                Dim intTemp As Int32
                intTemp = PrintAreaHeight
                PrintAreaHeight = PrintAreaWidth
                PrintAreaWidth = intTemp
                ' if the user selects landscape mode, swap the printing area height and width
            End If

            'Dim intLineCount As Int32 = CInt(PrintAreaHeight / font.Height)
            ' calculating the total number of lines in the document based on the height of
            ' the printing area and the height of the font
            Dim rectPrintingArea As New RectangleF(marginLeft, marginTop, PrintAreaWidth, PrintAreaHeight)
            ' initializing the rectangle structure that defines the printing area
            Dim fmt As New StringFormat(StringFormatFlags.LineLimit)
            'instantiating the StringFormat class, which encapsulates text layout information
            Dim intLinesFilled, intCharsFitted As Int32
            e.Graphics.MeasureString(Mid(myTextToPrint, intCurrentChar + 1), font, New SizeF(PrintAreaWidth, PrintAreaHeight), fmt, intCharsFitted, intLinesFilled)
            ' calling MeasureString to determine the number of characters that will fit in
            ' the printing area rectangle
            e.Graphics.DrawString(Mid(myTextToPrint, intCurrentChar + 1), font, Brushes.Black, rectPrintingArea, fmt)
            ' print the text to the page
            intCurrentChar += intCharsFitted
            'advancing the current char to the last char printed on this page 
            If intCurrentChar < myTextToPrint.Length Then
                e.HasMorePages = True
                'HasMorePages tells the printing module whether another PrintPage event should be fired
            Else
                e.HasMorePages = False
                intCurrentChar = 0
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".PrintDocument1_PrintPage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrintDocument1_PrintPage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bsEditSNButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditSNButton.Click
        Try
            bsEditSNButton.Visible = False
            bsSaveSNButton.Visible = True
            bsCancelSNButton.Visible = True
            bsCancelSNButton.Enabled = True
            bsSerialTextBox.Enabled = True
            'AJG BA-2132
            bsModelTextBox.Enabled = True
            bsSerialTextBox.Focus()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bsEditSNButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsEditSNButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bsCancelSNButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelSNButton.Click
        Try
            IsEditingSN = False
            bsEditSNButton.Visible = True
            bsSaveSNButton.Visible = False
            bsCancelSNButton.Visible = False
            bsSaveSNButton.Enabled = False
            bsSerialTextBox.Enabled = False
            'AJG BA-2132
            bsModelTextBox.Enabled = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bsCancelSNButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelSNButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bsSaveSNButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveSNButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            Me.BsErrorProvider1.Clear()

            Dim SNValidated As Boolean = False
            If Me.bsSerialTextBox.Text.Length > 0 Then
                If IsNumeric(Me.bsSerialTextBox.Text) Then
                    If CLng(Me.bsSerialTextBox.Text) > 0 Then
                        SNValidated = True
                    End If
                End If
            End If

            If SNValidated Then
                If bsModelTextBox.Text <> "" AndAlso IsNumeric(bsModelTextBox.Text) AndAlso (bsModelTextBox.Text.Length = 3) Then
                    If AnalyzerController.Instance.Analyzer.GetModelCode() <> bsModelTextBox.Text Then
                        SNValidated = False
                    End If
                End If
            End If

            If Not SNValidated Then
                Me.BsErrorProvider1.SetError(Me.bsSerialTextBox, GetMessageText(GlobalEnumerates.Messages.SRV_INVALID_SN.ToString, currentLanguage))
                Exit Try
            End If

            MyClass.IsEditingSN = False

            Me.Cursor = Cursors.WaitCursor
            ' Saving Serial number
            Me.bsSaveSNButton.Enabled = False
            MyBase.CurrentMode = ADJUSTMENT_MODES.SN_SAVING 'SGM 15/10/2012
            'myGlobal = MyBase.Save()
            PrepareArea()

            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    'MyBase.DisplaySimulationMessage("Saving serial number Instrument...")
                    'System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyClass.WaitProcessSimulation() 'SGM 04/01/2012
                    MyBase.myServiceMDI.Focus()
                    MyClass.CurrentMode = ADJUSTMENT_MODES.SAVED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING

                        'SGM - 04/10/2012
                        Dim myUtilCommand As New UTILCommandTO()
                        With myUtilCommand
                            .ActionType = UTILInstructionTypes.SaveSerialNumber
                            .SerialNumberToSave = Me.bsModelTextBox.Text.Trim & Me.bsSerialTextBox.Text.Trim
                            .TanksActionType = UTILIntermediateTanksTestActions.NothingToDo
                            .CollisionTestActionType = UTILCollisionTestActions.Disable
                            .SaveSerialAction = UTILSaveSerialNumberActions.SaveSerialNumber
                        End With

                        myGlobal = myScreenDelegate.SendUTIL(myUtilCommand)
                        'end SGM - 04/10/2012

                    End If
                End If

                If myGlobal.HasError Then
                    Me.PrepareErrorMode()
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bsSaveSNButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveSNButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bsSerialTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSerialTextBox.TextChanged

        Try
            If Not myScreenDelegate Is Nothing Then
                myScreenDelegate.SerialNumber = Me.bsModelTextBox.Text + Me.bsSerialTextBox.Text
                Me.bsSaveSNButton.Enabled = (myScreenDelegate.SerialNumber.Length = SpecifiedSerialNumberLength) 'SGM 15/10/2012
                MyClass.IsEditingSN = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".bsSerialTextBox_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSerialTextBox_TextChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub DetailsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DetailsButton.Click
        Me.Details = Not Me.Details
        ShowDetails(Me.Details)
    End Sub

    Private Sub MoreInfoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoreInfoButton.Click
        Try
            Me.RequestMoreInfo = True
            PrepareReadingAnalyzerInfoMode()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".MoreInfoButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MoreInfoButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

End Class