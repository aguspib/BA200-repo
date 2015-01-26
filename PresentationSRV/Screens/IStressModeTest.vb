Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL

Public Class UiStressModeTest
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As StressModeTestDelegate

    ' Language
    Private currentLanguage As String

    Private IsInitiating As Boolean
    Private IsStoping As Boolean
#End Region

#Region "Variables"
    Private ElementToStress As STRESS_TYPE
    Private TotalTimeStress As Long
    Private CompletedTimeStress As Long
    Private Initializating As Boolean
    Private Const InitCycles As Integer = 20
#End Region

#Region "Attributes"
    Private ActiveAnalyzerModelAttr As String
#End Region

#Region "Properties"
    Public Property ActiveAnalyzerModel() As String
        Get
            Return Me.ActiveAnalyzerModelAttr
        End Get
        Set(ByVal value As String)
            Me.ActiveAnalyzerModelAttr = value
        End Set
    End Property
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
    ''' <remarks>Created by XBC 22/03/11</remarks>
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
                        PrepareStressReadedMode()
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
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        If Me.IsInitiating Then
                            Me.RequestStatusStressTimer.Enabled = True
                            Debug.Print("CONFIRM TIMER IS ACTIVE !!!!")
                        End If

                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.STRESS_READED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        '' Activate timer
                        'Me.RequestStatusStressTimer.Interval = CInt(myScreenDelegate.TimerStatusModeStress) * 1000
                        'Me.RequestStatusStressTimer.Enabled = True
                        PrepareArea()
                        Me.RequestStressMode()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_TEST_STOP_BY_USER.ToString)
                        Me.RequestStressMode()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function

#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by XBC 23/05/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            'If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
            '    Dim linq As New List(Of UIRefreshDS.ReceivedAlarmsRow)

            '    linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
            '            Select a).ToList

            '    If linq.Count > 0 Then
            '        For i As Integer = 0 To linq.Count - 1
            '            If myScreenDelegate.CodeErrorsStress Is Nothing Then
            '                myScreenDelegate.CodeErrorsStress = New List(Of String)
            '            End If

            '            myScreenDelegate.CodeErrorsStress.Add(linq(i).AlarmID.ToString)
            '            myScreenDelegate.NumErrorsStress += 1
            '        Next
            '    End If

            '    If Not myScreenDelegate.CodeErrorsStress Is Nothing Then
            '        PrepareTestedMode()
            '    End If

            'End If


            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim sensorValue As Single = 0

                sensorValue = Me.myServiceMDI.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                If sensorValue >= 1 Then
                    ScreenWorkingProcess = False

                    Me.myServiceMDI.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                    Me.IsInitiating = False
                    MyBase.ActivateMDIMenusButtons(True)

                End If

                'ISE procedure finished
                sensorValue = Me.myServiceMDI.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False

                    Me.myServiceMDI.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor

                End If

                If Not Me.IsInitiating Then
                    PrepareTestedMode()
                End If
            End If

            If Not MyBase.myServiceMDI.MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                Me.ReadStressStatus()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Must Inherited"
    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            PrepareErrorMode()

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function InitializeHomes() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.ResetAllPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    Private Sub Initializations()
        Dim myGlobal As New GlobalDataTO
        Dim myResultData As New GlobalDataTO
        Try
            Me.Initializating = True

            Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS

            Me.RequestStatusStressTimer.Enabled = False
            Me.ElementToStress = STRESS_TYPE.COMPLETE

            Me.TypeTestLabel.Text = ""
            Me.CyclesLabel.Text = ""
            Me.CyclesCompletedLabel.Text = ""
            Me.TimeStartLabel.Text = ""
            Me.TimeTotalLabel.Text = ""
            Me.TimeCompletedLabel.Text = ""
            Me.ResetsNumLabel.Text = ""
            Me.ErrorsNumLabel.Text = ""
            Me.BsResetsTextBox.Text = ""
            Me.BsErrorsDataGridView.DataSource = Nothing

            myResultData = myScreenDelegate.ReadPartialElements(PreloadedMasterDataEnum.SRV_STRESS_ARMS)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = CType(myResultData.SetDatos, PreloadedMasterDataDS)

                Dim qElements As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order elements mode by the position number.
                qElements = (From a In myPreloadedMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BsArmsComboBox.DisplayMember = "FixedItemDesc"
                BsArmsComboBox.ValueMember = "ItemID"
                BsArmsComboBox.DataSource = qElements
            Else
                PrepareErrorMode()
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            myResultData = myScreenDelegate.ReadPartialElements(PreloadedMasterDataEnum.SRV_STRESS_ROTOR)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = CType(myResultData.SetDatos, PreloadedMasterDataDS)

                Dim qElements As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order elements mode by the position number.
                qElements = (From a In myPreloadedMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BsRotorsComboBox.DisplayMember = "FixedItemDesc"
                BsRotorsComboBox.ValueMember = "ItemID"
                BsRotorsComboBox.DataSource = qElements
            Else
                PrepareErrorMode()
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            myResultData = myScreenDelegate.ReadPartialElements(PreloadedMasterDataEnum.SRV_STRESS_PHOTO)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = CType(myResultData.SetDatos, PreloadedMasterDataDS)

                Dim qElements As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order elements mode by the position number.
                qElements = (From a In myPreloadedMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BsPhotometryComboBox.DisplayMember = "FixedItemDesc"
                BsPhotometryComboBox.ValueMember = "ItemID"
                BsPhotometryComboBox.DataSource = qElements
            Else
                PrepareErrorMode()
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            myResultData = myScreenDelegate.ReadPartialElements(PreloadedMasterDataEnum.SRV_STRESS_SYRINGE)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = CType(myResultData.SetDatos, PreloadedMasterDataDS)

                Dim qElements As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order elements mode by the position number.
                qElements = (From a In myPreloadedMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BsSyringesComboBox.DisplayMember = "FixedItemDesc"
                BsSyringesComboBox.ValueMember = "ItemID"
                BsSyringesComboBox.DataSource = qElements
            Else
                PrepareErrorMode()
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            myResultData = myScreenDelegate.ReadPartialElements(PreloadedMasterDataEnum.SRV_STRESS_FLUID)
            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                myPreloadedMasterDataDS = CType(myResultData.SetDatos, PreloadedMasterDataDS)

                Dim qElements As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order elements mode by the position number.
                qElements = (From a In myPreloadedMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BsFluidsComboBox.DisplayMember = "FixedItemDesc"
                BsFluidsComboBox.ValueMember = "ItemID"
                BsFluidsComboBox.DataSource = qElements
            Else
                PrepareErrorMode()
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".Initializations ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Initializating = False
        End Try
    End Sub

    ''' <summary>
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <remarks>Created by: XBC 22/03/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode)
                If Not myGlobal.HasError Then
                    ' Send FwScripts
                    myGlobal = myFwScriptDelegate.StartFwScriptQueue
                End If
            Else
                myGlobal.HasError = True
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 22/03/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_STRESS", currentLanguage)
            Me.BsConfigLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONFIG_TEST", currentLanguage)
            Me.BsResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RESULTS_TEST", currentLanguage)
            Me.bsNumCyclesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_NUMCYCLES", currentLanguage) & ":"
            Me.BsStressTypeGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_TYPE", currentLanguage) & ":"
            Me.bsCompleteRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_COMPLETE", currentLanguage)
            Me.bsPartialRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_PARTIAL", currentLanguage)
            Me.BsArmsRadBtn.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ARMS", currentLanguage)
            Me.BsRotorsRadioBtn.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ROTORS", currentLanguage)
            Me.BsPhotometryRadioBtn.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PHOTOMETRY", currentLanguage)
            Me.BsSyringesRadioBtn.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SYRINGES", currentLanguage)
            Me.BsFluidRadioBtn.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FLUID", currentLanguage)
            Me.BsStressTypeResultLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_TYPERES", currentLanguage) & ":"
            Me.BsProgramCyclesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_CYCLES_PROG_RES", currentLanguage) & ":"
            Me.BsCompleteCyclesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_CYCLES_COMP_RES", currentLanguage) & ":"
            Me.BsResetsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CYCLES_RESETS", currentLanguage)
            Me.BsNumResetsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NUM_CYCLES_RESETS", currentLanguage) & ":"
            Me.BsTimeStartLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TIME_START", currentLanguage) & ":"
            Me.BsTimeProgLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TIME_PROG", currentLanguage) & ":"
            Me.BsTimeCompleteLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TIME_COMP", currentLanguage) & ":"
            Me.BsErrDescLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ERRDESC", currentLanguage)
            Me.BsNumErrLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ERRNUM", currentLanguage) & ":"

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
    ''' Modified by XBC 24/11/2011 - Unify buttons Start and Stop Test Stress
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...

            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then
                MyBase.bsScreenToolTipsControl.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))
            Else
                MyBase.bsScreenToolTipsControl.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))
            End If

            MyBase.bsScreenToolTipsControl.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 22/03/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Try
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                BsTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If


            'MyBase.SetButtonImage(BsExitButton, "CANCEL")
            'MyBase.SetButtonImage(BsTestButton, "ADJUSTMENT")


            ''Me.BsAdjustButtonTODELETE.Visible = False
            ''Me.BsTestButton.Visible = False
            ''Me.BsAbortTestButton.Visible = False

            ' ''ADJUST Button
            ''auxIconName = GetIconName("ADJUSTMENT")
            ''If System.IO.File.Exists(iconPath & auxIconName) Then
            ''    BsAdjustButtonTODELETE.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            ''    BsAdjustButtonTODELETE.BackgroundImageLayout = ImageLayout.Center
            ''End If

            ' ''SAVE Button
            ''auxIconName = GetIconName("SAVE")
            ''If System.IO.File.Exists(iconPath & auxIconName) Then
            ''    BsTestButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            ''    BsTestButton.BackgroundImageLayout = ImageLayout.Center
            ''End If

            ' ''CANCEL Button
            ''auxIconName = GetIconName("UNDO") 'CANCEL
            ''If System.IO.File.Exists(iconPath & auxIconName) Then
            ''    BsAbortTestButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            ''    BsAbortTestButton.BackgroundImageLayout = ImageLayout.Center
            ''End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    'BsExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

            '' XBC 24/11/2011 - Unify buttons Start and Stop Test Stress
            ' ''TEST Button
            ''auxIconName = GetIconName("ADJUSTMENT")
            ''If System.IO.File.Exists(iconPath & auxIconName) Then
            ''    BsTestButton_TODELETE.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            ''    BsTestButton_TODELETE.BackgroundImageLayout = ImageLayout.Center
            ''End If

            ''TEST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    'BsTestButton.BackgroundImageLayout = ImageLayout.Center
            'End If
            '' XBC 24/11/2011 - Unify buttons Start and Stop Test Stress

            ''Info Button
            'auxIconName = GetIconName("RIGHT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Me.BsInfoExpandButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    Me.BsInfoExpandButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: XBC 22/03/2011</remarks>
    Private Sub DisableAll()
        Try
            Me.BsCyclesUpDown.Enabled = False
            Me.bsCompleteRadioButton.Enabled = False
            Me.bsPartialRadioButton.Enabled = False
            Me.BsArmsRadBtn.Enabled = False
            Me.BsRotorsRadioBtn.Enabled = False
            Me.BsPhotometryRadioBtn.Enabled = False
            Me.BsSyringesRadioBtn.Enabled = False
            Me.BsFluidRadioBtn.Enabled = False
            Me.BsArmsComboBox.Enabled = False
            Me.BsRotorsComboBox.Enabled = False
            Me.BsPhotometryComboBox.Enabled = False
            Me.BsSyringesComboBox.Enabled = False
            Me.BsFluidsComboBox.Enabled = False

            'Me.BsResultsGroupBox.Enabled = False

            ' Disable Area Buttons
            If Not myServiceMDI.MDIAnalyzerManager.IsStressing Then
                Me.BsTestButton.Enabled = False
                Me.BsExitButton.Enabled = False
            End If

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 22/03/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareLoadingMode()

                Case ADJUSTMENT_MODES.STRESS_READING
                    PrepareStressReadingMode()

                Case ADJUSTMENT_MODES.STRESS_READED
                    PrepareStressReadedMode()

                Case ADJUSTMENT_MODES.TESTING, _
                     ADJUSTMENT_MODES.TESTED
                    PrepareTestingMode()

                Case ADJUSTMENT_MODES.TEST_EXITING
                    PrepareTestExitingMode()

                    'Case ADJUSTMENT_MODES.TEST_EXITED

                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

            If Not MyBase.SimulationMode And Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments Reading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareAdjustReadingMode()
        'Dim myResultData As New GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()

            ' Reading Adjustments
            If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                ' Parent Reading Adjustments
                MyBase.ReadAdjustments()
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    MyClass.myServiceMDI.AdjustmentsReaded = True
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                        myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                    End If
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
    ''' Prepare GUI for Loading Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareLoadingMode()
        Dim myResultData As New GlobalDataTO
        Try
            DisableAll()
            Initializations()

            Me.ActiveAnalyzerModel = Ax00ServiceMainMDI.ActiveAnalyzerModel
            ' Get common Parameters
            myResultData = GetParameters()

            If Not myResultData.HasError Then
                Me.RequestStressMode()
            Else
                Me.PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Request current Stress Mode
    ''' </summary>
    ''' <remarks>Created by XBC 29/03/2011</remarks>
    Private Sub RequestStressMode()
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.ReadStressMode()
            PrepareArea()

            ' Manage FwScripts must to be sent at load screen
            If MyBase.SimulationMode Then
                ' simulating...
                'MyBase.DisplaySimulationMessage("Request Status Stress Mode from Instrument...")
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                MyClass.CurrentMode = ADJUSTMENT_MODES.STRESS_READED
                PrepareArea()
            Else
                'SendFwScript(Me.CurrentMode)
                If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                    myGlobal = myScreenDelegate.SendSDPOLL()
                End If
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestStressMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RequestStressMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try
            'If IsStoping Then
            '    Debug.Print("aqui !!!")
            'End If

            Me.BsCyclesUpDown.Enabled = True
            Me.bsCompleteRadioButton.Enabled = True

            ' temporally disabled for PROTO 5
            Me.bsPartialRadioButton.Enabled = False  ' True

            Me.BsTestButton.Enabled = True
            Me.BsExitButton.Enabled = True

            MyBase.ActivateMDIMenusButtons(True)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareStressReadingMode()
        Try
            'Me.DisableAll()
            Me.BsResultsGroupBox.Enabled = True
            'Me.BsResetsTextBox.ReadOnly = True
            'Me.BsErrorsDataGridView.ReadOnly = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStressReadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareStressReadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareStressReadedMode()
        Try
            Me.ReadStressStatus()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareStressReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareStressReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            'Dim Utilities As New Utilities
            ' Configurate GUI Controls
            Me.DisableAll()
            Me.BsResultsGroupBox.Enabled = True

            ' Update values
            Me.TypeTestLabel.Text = TranslateStressType(Me.ElementToStress)
            Me.CyclesLabel.Text = myScreenDelegate.NumCycles.ToString
            Me.CyclesCompletedLabel.Text = myScreenDelegate.NumCompletedCycles.ToString

            'Me.TimeStartLabel.Text = myScreenDelegate.TimeStartStressTest.ToString("HH:mm:ss")
            Me.TimeStartLabel.Text = myScreenDelegate.HourStartStress.ToString("00") + ":" + myScreenDelegate.MinuteStartStress.ToString("00") + ":" + myScreenDelegate.SecondStartStress.ToString("00")

            Me.TotalTimeStress = (InitCycles * myScreenDelegate.TimeMachineCycle) + _
                                 (myScreenDelegate.NumCycles * myScreenDelegate.TimeMachineCycle)
            Me.TimeTotalLabel.Text = Utilities.FormatToHHmmss(Me.TotalTimeStress)

            Me.CompletedTimeStress = (InitCycles * myScreenDelegate.TimeMachineCycle) + myScreenDelegate.TimeElapsedStressTest
            Me.TimeCompletedLabel.Text = Utilities.FormatToHHmmss(Me.CompletedTimeStress)

            Me.ResetsNumLabel.Text = myScreenDelegate.NumResetsStress.ToString
            Me.ErrorsNumLabel.Text = myScreenDelegate.NumErrorsStress.ToString
            Me.BsResetsTextBox.Text = myScreenDelegate.CyclesResetsStressToString
            If Not myScreenDelegate.CodeErrorsStressToDS Is Nothing AndAlso myScreenDelegate.CodeErrorsStressToDS.Tables.Count > 0 Then
                Me.BsErrorsDataGridView.DataSource = myScreenDelegate.CodeErrorsStressToDS.Tables(0)
                Me.BsErrorsDataGridView.Refresh()
            End If
            Me.ProgressBar1.Maximum = CInt(Me.TotalTimeStress)
            If CInt(myScreenDelegate.NumCompletedCycles * myScreenDelegate.TimeMachineCycle) > Me.ProgressBar1.Maximum Then
                Me.ProgressBar1.Value = Me.ProgressBar1.Maximum
            Else
                Me.ProgressBar1.Value = CInt(myScreenDelegate.NumCompletedCycles * myScreenDelegate.TimeMachineCycle)
            End If

            If Me.ProgressBar1.Value > 0 Then
                Me.ProgressBar1.Visible = True
                Me.ProgressBar1.Refresh()
            End If


            ' Buttons Area
            Me.BsTestButton.Enabled = True
            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareTestedMode()
        Try
            'If IsStoping Then
            '    Debug.Print("aqui !!!")
            'End If

            'Dim Utilities As New Utilities
            Me.BsCyclesUpDown.Enabled = True
            Me.bsCompleteRadioButton.Enabled = True

            ' temporally disabled for PROTO 5
            Me.bsPartialRadioButton.Enabled = False ' True

            If Me.bsPartialRadioButton.Checked Then
                Me.BsArmsRadBtn.Enabled = True
                If Me.BsArmsRadBtn.Checked Then
                    Me.BsArmsComboBox.Enabled = True
                End If
                Me.BsRotorsRadioBtn.Enabled = True
                If Me.BsRotorsRadioBtn.Checked Then
                    Me.BsRotorsComboBox.Enabled = True
                End If
                Me.BsPhotometryRadioBtn.Enabled = True
                If Me.BsPhotometryRadioBtn.Checked Then
                    Me.BsPhotometryComboBox.Enabled = True
                End If
                Me.BsSyringesRadioBtn.Enabled = True
                If Me.BsSyringesRadioBtn.Checked Then
                    Me.BsSyringesComboBox.Enabled = True
                End If
                Me.BsFluidRadioBtn.Enabled = True
                If Me.BsFluidRadioBtn.Checked Then
                    Me.BsFluidsComboBox.Enabled = True
                End If
            End If

            Me.BsResultsGroupBox.Enabled = True

            ' Update values
            Me.TypeTestLabel.Text = TranslateStressType(Me.ElementToStress)
            Me.CyclesLabel.Text = myScreenDelegate.NumCycles.ToString
            Me.CyclesCompletedLabel.Text = myScreenDelegate.NumCompletedCycles.ToString

            'Me.TimeStartLabel.Text = myScreenDelegate.TimeStartStressTest.ToString("HH:mm:ss")
            Me.TimeStartLabel.Text = myScreenDelegate.HourStartStress.ToString("00") + ":" + myScreenDelegate.MinuteStartStress.ToString("00") + ":" + myScreenDelegate.SecondStartStress.ToString("00")

            Me.TotalTimeStress = (InitCycles * myScreenDelegate.TimeMachineCycle) + _
                                 (myScreenDelegate.NumCycles * myScreenDelegate.TimeMachineCycle)
            Me.TimeTotalLabel.Text = Utilities.FormatToHHmmss(Me.TotalTimeStress)

            Me.CompletedTimeStress = (InitCycles * myScreenDelegate.TimeMachineCycle) + myScreenDelegate.TimeElapsedStressTest
            Me.TimeCompletedLabel.Text = Utilities.FormatToHHmmss(Me.CompletedTimeStress)

            Me.ResetsNumLabel.Text = myScreenDelegate.NumResetsStress.ToString
            Me.ErrorsNumLabel.Text = myScreenDelegate.NumErrorsStress.ToString
            Me.BsResetsTextBox.Text = myScreenDelegate.CyclesResetsStressToString
            If Not myScreenDelegate.CodeErrorsStressToDS Is Nothing AndAlso myScreenDelegate.CodeErrorsStressToDS.Tables.Count > 0 Then
                Me.BsErrorsDataGridView.DataSource = myScreenDelegate.CodeErrorsStressToDS.Tables(0)
                Me.BsErrorsDataGridView.Refresh()
            End If
            ' Hide Progress Bar
            Me.ProgressBar1.Value = 0
            Me.ProgressBar1.Visible = False
            ' Buttons Area
            Me.BsTestButton.Enabled = True
            Me.BsExitButton.Enabled = True

            MyBase.ActivateMDIMenusButtons(True)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exiting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 04/04/2011</remarks>
    Private Sub PrepareTestExitingMode()
        Try
            DisableAll()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            Me.ProgressBar1.Visible = False
            DisableAll()
            MyBase.ActivateMDIMenusButtons(True)
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for Stress Tests
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Function GetParameters() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myScreenDelegate.GetParameters(Me.ActiveAnalyzerModel)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    Private Sub ReadStressStatus()
        Try
            Select Case myScreenDelegate.StatusStressMode
                Case STRESS_STATUS.NOT_STARTED
                    Me.PrepareLoadedMode()
                    MyBase.DisplayMessage("")
                Case STRESS_STATUS.UNFINISHED
                    Me.PrepareTestingMode()
                    ' Activate timer
                    Me.RequestStatusStressTimer.Interval = CInt(myScreenDelegate.TimerStatusModeStress) * 1000
                    Me.RequestStatusStressTimer.Enabled = True
                    MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)
                Case STRESS_STATUS.FINISHED_OK
                    MyBase.myServiceMDI.MDIAnalyzerManager.IsStressing = False
                    If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                        Me.IsInitiating = True
                        PrepareAdjustReadingMode()
                        Debug.Print(" - " & Date.Now.ToString & " - READ ADJUSTMENTS ")
                    Else
                        Me.DisableAll()
                        If Not MyBase.myServiceMDI.MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                            Me.IsInitiating = False
                        End If
                        Me.GenerateReportsOutput()
                        MyBase.myServiceMDI.SEND_INFO_START()
                        Debug.Print(" - " & Date.Now.ToString & " - SEND INFO START ")
                        While Me.IsInitiating
                            Debug.Print(" ...... ")
                            Application.DoEvents()
                        End While
                        Debug.Print(" - " & Date.Now.ToString & " - TESTED MODE ")
                        Me.PrepareTestedMode()
                        Me.RequestStatusStressTimer.Enabled = False
                        MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                    End If
                Case STRESS_STATUS.FINISHED_ERR
                    MyBase.myServiceMDI.MDIAnalyzerManager.IsStressing = False
                    If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                        Me.DisableAll()
                        Me.IsInitiating = True
                        'System.Threading.Thread.Sleep(500)
                        PrepareAdjustReadingMode()
                    Else
                        Me.GenerateReportsOutput()
                        MyBase.myServiceMDI.SEND_INFO_START()
                        While Me.IsInitiating
                            Application.DoEvents()
                        End While
                        Me.PrepareTestedMode()
                        Me.RequestStatusStressTimer.Enabled = False
                        MyBase.DisplayMessage(Messages.SRV_TESTEND_ERROR.ToString)
                    End If
            End Select

            ' XBC 24/11/2011 - Unify buttons Start and Stop Test Stress
            Me.PrepareTestButton()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadStressStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadStressStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 01/04/2011</remarks>
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

    Private Function TranslateStressType(ByVal pElementToStress As STRESS_TYPE) As String
        Dim returnValue As String = ""
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Select Case pElementToStress
                Case STRESS_TYPE.COMPLETE
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STRESS_COMPLETE", currentLanguage)
                Case STRESS_TYPE.SAMPLE_ARM_MH
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SampleArm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MH", currentLanguage)
                Case STRESS_TYPE.SAMPLE_ARM_MV
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SampleArm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MV", currentLanguage)
                Case STRESS_TYPE.REAGENT1_ARM_MH
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MH", currentLanguage)
                Case STRESS_TYPE.REAGENT1_ARM_MV
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MV", currentLanguage)
                Case STRESS_TYPE.REAGENT2_ARM_MH
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MH", currentLanguage)
                Case STRESS_TYPE.REAGENT2_ARM_MV
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MV", currentLanguage)
                Case STRESS_TYPE.MIXER1_ARM_MH
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer1Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MH", currentLanguage)
                Case STRESS_TYPE.MIXER1_ARM_MV
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer1Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MV", currentLanguage)
                Case STRESS_TYPE.MIXER2_ARM_MH
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer2Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MH", currentLanguage)
                Case STRESS_TYPE.MIXER2_ARM_MV
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer2Arm", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MV", currentLanguage)
                Case STRESS_TYPE.SAMPLES_ROTOR
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_ROTOR_TYPES_SAMPLES", currentLanguage)
                Case STRESS_TYPE.REACTIONS_ROTOR
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_REACT_ROTOR", currentLanguage)
                Case STRESS_TYPE.REAGENTS_ROTOR
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_ROTOR_TYPES_REAGENTS", currentLanguage)
                Case STRESS_TYPE.WASHING_STATION
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashStation", currentLanguage)
                Case STRESS_TYPE.SAMPLE_SYRINGE
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SYRINGE", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SAMPLE", currentLanguage)
                Case STRESS_TYPE.REAGENT1_SYRINGE
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SYRINGE", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1", currentLanguage)
                Case STRESS_TYPE.REAGENT2_SYRINGE
                    returnValue = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SYRINGE", currentLanguage) & " - " & _
                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2", currentLanguage)
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".TranslateStressType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TranslateStressType ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return returnValue
    End Function

    Private Sub ExitScreen()
        Try
            Select Case myScreenDelegate.StatusStressMode
                Case STRESS_STATUS.UNFINISHED
                    Me.RequestStatusStressTimer.Enabled = False
                    Ax00ServiceMainMDI.Close()
                Case STRESS_STATUS.NOT_STARTED
                    If MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING Then
                        Me.RequestStatusStressTimer.Enabled = False
                        Ax00ServiceMainMDI.Close()
                    Else
                        MyBase.myServiceMDI.SEND_INFO_START()
                        Me.Close()
                    End If
                Case STRESS_STATUS.FINISHED_OK, _
                     STRESS_STATUS.FINISHED_ERR
                    MyBase.myServiceMDI.SEND_INFO_START()
                    Me.Close()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Function in charge on register any activity on database
    ''' </summary>
    ''' <remarks>Created by XBC 31/08/2011</remarks>
    Private Function GenerateReportsOutput() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.InsertReport("TEST", "STRESS")

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GenerateReportsOutput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
        Return myGlobal
    End Function

#End Region

#Region "Events"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IStressModeTest_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else
                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing
                Me.Dispose()

                MyBase.ActivateMDIMenusButtons(True)

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub StressModeTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Try
            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New StressModeTestDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString
            myScreenDelegate.currentLanguage = Me.currentLanguage

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels()

            PrepareButtons()

            'SGM 12/11/2012 - Information
            'Me.BsInfoXPSViewer.FitToPageHeight()
            MyBase.DisplayInformation(APPLICATION_PAGES.STRESS, Me.BsInfoXPSViewer)

            DisableAll()

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            ' Check communications with Instrument
            If Not Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                If Ax00ServiceMainMDI.MDIAnalyzerManager.IsStressing Then
                    PrepareLoadingMode()
                Else
                    PrepareAdjustReadingMode()
                End If
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ResetBorderSRV()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IStressModeTest_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            Me.BsInfoXPSViewer.Visible = True
            Me.BsInfoXPSViewer.RefreshPage()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsCompleteRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCompleteRadioButton.CheckedChanged
        Try
            If Me.bsCompleteRadioButton.Checked Then
                Me.ElementToStress = STRESS_TYPE.COMPLETE
                Me.BsArmsRadBtn.Enabled = False
                Me.BsRotorsRadioBtn.Enabled = False
                Me.BsPhotometryRadioBtn.Enabled = False
                Me.BsSyringesRadioBtn.Enabled = False
                Me.BsFluidRadioBtn.Enabled = False
                Me.BsArmsComboBox.Enabled = False
                Me.BsRotorsComboBox.Enabled = False
                Me.BsPhotometryComboBox.Enabled = False
                Me.BsSyringesComboBox.Enabled = False
                Me.BsFluidsComboBox.Enabled = False
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".bsCompleteRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCompleteRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub bsPartialRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPartialRadioButton.CheckedChanged
        Try
            If Me.bsPartialRadioButton.Checked Then
                Me.BsArmsRadBtn.Checked = True
                Me.BsArmsRadBtn.Enabled = True
                Me.BsRotorsRadioBtn.Enabled = True
                Me.BsPhotometryRadioBtn.Enabled = True
                Me.BsSyringesRadioBtn.Enabled = True
                Me.BsFluidRadioBtn.Enabled = True
                Me.BsArmsComboBox.Enabled = True
                Me.ElementToStress = STRESS_TYPE.SAMPLE_ARM_MH
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "bsPartialRadioButton.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "bsPartialRadioButton.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub RequestStatusStressTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RequestStatusStressTimer.Tick
        Try
            'Me.RequestStatusStressTimer.Enabled = False

            If MyBase.SimulationMode Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.STRESS_READED
                myScreenDelegate.SimulateTestComplete()
                PrepareArea()
            Else
                Me.RequestStressMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestStatusStressTimer.Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RequestStatusStressTimer.Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 24/11/2011 - Unify buttons Start and Stop Test Stress
    'Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton_TODELETE.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = MyBase.Test
    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '        Else
    '            PrepareArea()

    '            ' Initializations
    '            myScreenDelegate.NumCycles = CLng(BsWellUpDown.Value)
    '            myScreenDelegate.NumCompletedCycles = 0
    '            myScreenDelegate.StressType = Me.ElementToStress
    '            Me.TotalTimeStress = myScreenDelegate.NumCycles * myScreenDelegate.TimeMachineCycle
    '            myScreenDelegate.CyclesResetsStress = New List(Of Long)
    '            myScreenDelegate.CodeErrorsStress = Nothing
    '            ' Initialize progress bar
    '            Me.ProgressBar1.Maximum = CInt(Me.TotalTimeStress)
    '            Me.ProgressBar1.Value = 0
    '            ' Prepare controls GUI
    '            Me.BsWellUpDown.Value = 1  ' Initialize value in case of change machine
    '            Me.BsResetsTextBox.Text = ""
    '            If Not Me.BsErrorsDataGridView Is Nothing Then
    '                If Me.BsErrorsDataGridView.Rows.Count > 0 Then
    '                    'Me.BsErrorsDataGridView.Rows.Clear()
    '                    Me.BsErrorsDataGridView.DataSource = Nothing
    '                End If
    '            End If

    '            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

    '            If Not myGlobal.HasError Then
    '                If MyBase.SimulationMode Then
    '                    ' simulating
    '                    Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Stress Mode Testing ..."
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
    '                    myScreenDelegate.SimulateTestStart()
    '                    PrepareStressReadedMode()
    '                Else
    '                    ' Manage FwScripts must to be sent to testing
    '                    'SendFwScript(Me.CurrentMode)
    '                    If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
    '                        myGlobal = myScreenDelegate.SendSTRESS_TEST()
    '                    End If
    '                End If
    '            End If

    '        End If

    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTestButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsTestButton.Click", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Modified by XBC 24/11/2011 - Unify buttons Start and Stop Test Stress</remarks>
    Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then

                ' STOP STRESS

                Me.IsStoping = True
                Me.RequestStatusStressTimer.Enabled = False
                MyBase.DisplayMessage(Messages.SRV_TEST_STOP_BY_USER.ToString)

                myGlobal = MyBase.ExitTest
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()

                    MyBase.DisplayMessage(Messages.SRV_TEST_ABORTING.ToString)

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Stress Mode Exiting ..."
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                            MyClass.myServiceMDI.AdjustmentsReaded = True
                            myScreenDelegate.SimulateTestComplete()
                            PrepareStressReadedMode()
                        Else
                            ' Manage FwScripts must to be sent to testing
                            'SendFwScript(Me.CurrentMode)
                            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                myGlobal = myScreenDelegate.SendSTRESS_STOP()
                            End If
                        End If
                    End If

                End If

            Else

                ' START STRESS

                Ax00ServiceMainMDI.MDIAnalyzerManager.IsStressing = True

                myGlobal = MyBase.Test
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()
                    Me.DisableAll()

                    ' Initializations
                    myScreenDelegate.NumCycles = CLng(BsCyclesUpDown.Value)
                    myScreenDelegate.NumCompletedCycles = 0
                    myScreenDelegate.StressType = Me.ElementToStress
                    Me.TotalTimeStress = (InitCycles * myScreenDelegate.TimeMachineCycle) + _
                                         (myScreenDelegate.NumCycles * myScreenDelegate.TimeMachineCycle)
                    myScreenDelegate.CyclesResetsStress = New List(Of Long)
                    myScreenDelegate.CodeErrorsStress = Nothing
                    ' Initialize progress bar
                    Me.ProgressBar1.Maximum = CInt(Me.TotalTimeStress)
                    Me.ProgressBar1.Value = 0
                    ' Prepare controls GUI
                    Me.BsCyclesUpDown.Value = 1  ' Initialize value in case of change machine
                    Me.BsResetsTextBox.Text = ""
                    If Not Me.BsErrorsDataGridView Is Nothing Then
                        If Me.BsErrorsDataGridView.Rows.Count > 0 Then
                            'Me.BsErrorsDataGridView.Rows.Clear()
                            Me.BsErrorsDataGridView.DataSource = Nothing
                        End If
                    End If

                    MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                    MyBase.myServiceMDI.SEND_INFO_STOP()
                    System.Threading.Thread.Sleep(500)

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Stress Mode Testing ..."
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
                            myScreenDelegate.SimulateTestStart()
                            PrepareStressReadedMode()
                        Else
                            ' Manage FwScripts must to be sent to testing
                            'SendFwScript(Me.CurrentMode)
                            If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
                                myGlobal = myScreenDelegate.SendSDMODE()
                            End If
                        End If
                    End If

                End If
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTestButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTestButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change Icon button depending on screen status
    ''' </summary>
    ''' <remarks>Created by XBC 24/11/2011 - Unify buttons Start and Stop Test Stress</remarks>
    Private Sub PrepareTestButton()

        'Dim myGlobal As New GlobalDataTO
        ''Dim myUtil As New Utilities.

        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath

        Try

            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then
                auxIconName = GetIconName("STOP")
            Else
                auxIconName = GetIconName("ADJUSTMENT")
            End If
            If (auxIconName <> "") Then
                BsTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'Dim myNewImage As Image
            'If System.IO.File.Exists(iconPath & auxIconName) Then

            '    Dim myImage As Image
            '    myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            '    myGlobal = Utilities.ResizeImage(myImage, New Size(24, 24))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myNewImage = CType(myGlobal.SetDatos, Bitmap)
            '    Else
            '        myNewImage = CType(myImage, Bitmap)
            '    End If

            '    BsTestButton.Image = myNewImage
            '    'BsTestButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestButton ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.ExitScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
                '    Me.ExitScreen()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsArmsRadBtn_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsArmsRadBtn.CheckedChanged
        Try
            Me.BsArmsComboBox.Enabled = True
            Me.BsRotorsComboBox.Enabled = False
            Me.BsPhotometryComboBox.Enabled = False
            Me.BsSyringesComboBox.Enabled = False
            Me.BsFluidsComboBox.Enabled = False
            Me.BsArmsComboBox.SelectedIndex = 0
            Me.ElementToStress = STRESS_TYPE.SAMPLE_ARM_MV
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsArmsRadBtn.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsArmsRadBtn.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsRotorsRadioBtn_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRotorsRadioBtn.CheckedChanged
        Try
            If Me.BsRotorsRadioBtn.Checked Then
                Me.BsArmsComboBox.Enabled = False
                Me.BsRotorsComboBox.Enabled = True
                Me.BsPhotometryComboBox.Enabled = False
                Me.BsSyringesComboBox.Enabled = False
                Me.BsFluidsComboBox.Enabled = False
                Me.BsRotorsComboBox.SelectedIndex = 0
                Me.ElementToStress = STRESS_TYPE.SAMPLES_ROTOR
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsRotorsRadioBtn.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsRotorsRadioBtn.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPhotometryRadioBtn_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsPhotometryRadioBtn.CheckedChanged
        Try
            If Me.BsPhotometryRadioBtn.Checked Then
                Me.BsArmsComboBox.Enabled = False
                Me.BsRotorsComboBox.Enabled = False
                Me.BsPhotometryComboBox.Enabled = True
                Me.BsSyringesComboBox.Enabled = False
                Me.BsFluidsComboBox.Enabled = False
                Me.BsPhotometryComboBox.SelectedIndex = 0
                Me.ElementToStress = STRESS_TYPE.REACTIONS_ROTOR
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsPhotometryRadioBtn.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsPhotometryRadioBtn.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsSyringesRadioBtn_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSyringesRadioBtn.CheckedChanged
        Try
            If Me.BsSyringesRadioBtn.Checked Then
                Me.BsArmsComboBox.Enabled = False
                Me.BsRotorsComboBox.Enabled = False
                Me.BsPhotometryComboBox.Enabled = False
                Me.BsSyringesComboBox.Enabled = True
                Me.BsFluidsComboBox.Enabled = False
                Me.BsSyringesComboBox.SelectedIndex = 0
                Me.ElementToStress = STRESS_TYPE.SAMPLE_SYRINGE
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsSyringesRadioBtn.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsSyringesRadioBtn.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsFluidRadioBtn_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsFluidRadioBtn.CheckedChanged
        Try
            If Me.BsFluidRadioBtn.Checked Then
                Me.BsArmsComboBox.Enabled = False
                Me.BsRotorsComboBox.Enabled = False
                Me.BsPhotometryComboBox.Enabled = False
                Me.BsSyringesComboBox.Enabled = False
                Me.BsFluidsComboBox.Enabled = True
                Me.BsFluidsComboBox.SelectedIndex = 0
                Me.ElementToStress = STRESS_TYPE.WASHING_STATION
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsFluidRadioBtn.CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsFluidRadioBtn.CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsArmsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsArmsComboBox.SelectedIndexChanged
        Try
            If Not Me.Initializating Then
                If Me.BsArmsRadBtn.Checked Then
                    Select Case BsArmsComboBox.SelectedValue.ToString()
                        Case "SAMPLEV"
                            Me.ElementToStress = STRESS_TYPE.SAMPLE_ARM_MV
                        Case "REAGENT1V"
                            Me.ElementToStress = STRESS_TYPE.REAGENT1_ARM_MV
                        Case "REAGENT2V"
                            Me.ElementToStress = STRESS_TYPE.REAGENT2_ARM_MV
                        Case "MIXER1V"
                            Me.ElementToStress = STRESS_TYPE.MIXER1_ARM_MV
                        Case "MIXER2V"
                            Me.ElementToStress = STRESS_TYPE.MIXER2_ARM_MV
                        Case "SAMPLEH"
                            Me.ElementToStress = STRESS_TYPE.SAMPLE_ARM_MH
                        Case "REAGENT1H"
                            Me.ElementToStress = STRESS_TYPE.REAGENT1_ARM_MH
                        Case "REAGENT2H"
                            Me.ElementToStress = STRESS_TYPE.REAGENT2_ARM_MH
                        Case "MIXER1H"
                            Me.ElementToStress = STRESS_TYPE.MIXER1_ARM_MH
                        Case "MIXER2H"
                            Me.ElementToStress = STRESS_TYPE.MIXER2_ARM_MH
                    End Select
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsArmsComboBox.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsArmsComboBox.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsRotorsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsRotorsComboBox.SelectedIndexChanged
        Try
            If Not Me.Initializating Then
                Select Case BsRotorsComboBox.SelectedValue.ToString
                    Case "SAMPLES"
                        Me.ElementToStress = STRESS_TYPE.SAMPLES_ROTOR
                    Case "REAGENTS"
                        Me.ElementToStress = STRESS_TYPE.REAGENTS_ROTOR
                End Select
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsRotorsComboBox.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsRotorsComboBox.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsPhotometryComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsPhotometryComboBox.SelectedIndexChanged
        Try
            If Not Me.Initializating Then
                Select Case BsPhotometryComboBox.SelectedValue.ToString()
                    Case "REACT"
                        Me.ElementToStress = STRESS_TYPE.REACTIONS_ROTOR
                    Case "WASH"
                        Me.ElementToStress = STRESS_TYPE.WASHING_STATION
                End Select
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsPhotometryComboBox.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsPhotometryComboBox.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsSyringesComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSyringesComboBox.SelectedIndexChanged
        Try
            If Not Me.Initializating Then
                Select Case BsSyringesComboBox.SelectedValue.ToString()
                    Case "SAMPLES"
                        Me.ElementToStress = STRESS_TYPE.SAMPLE_SYRINGE
                    Case "REAGENT1"
                        Me.ElementToStress = STRESS_TYPE.REAGENT1_SYRINGE
                    Case "REAGENT2"
                        Me.ElementToStress = STRESS_TYPE.REAGENT2_SYRINGE
                End Select
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "BsSyringesComboBox.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsSyringesComboBox.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsFluidsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsFluidsComboBox.SelectedIndexChanged
        Try
            If Not Me.Initializating Then
                Me.ElementToStress = STRESS_TYPE.WASHING_STATION
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsFluidsComboBox.SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "BsFluidsComboBox.SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsButtons_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsTestButton.MouseHover, BsExitButton.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub Buttons_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsTestButton.MouseLeave, BsExitButton.MouseLeave
        If MyBase.myServiceMDI.MDIAnalyzerManager.IsStressing Then
            Me.Cursor = Cursors.WaitCursor
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub Buttons_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsTestButton.MouseMove, BsExitButton.MouseMove
        Me.Cursor = Cursors.Hand
    End Sub
#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoXPSViewer.Load
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
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

End Class
