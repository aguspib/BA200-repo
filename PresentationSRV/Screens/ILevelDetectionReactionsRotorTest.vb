Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.App

Public Class ILevelDetectionReactionsRotorTest
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As LevelDetectionReactionsRotorTestDelegate
    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS

    ' Language
    Private currentLanguage As String

    Private IsInfoExpanded As Boolean = False

#End Region

#Region "Enumerates"

    Private Enum HISTORY_TASKS
        NONE = 0
        FREQUENCY_READ = 1
        DETECTION_TEST = 2
    End Enum

#End Region

#Region "Attributes"
    Private ActiveAnalyzerModelAttr As String
    Private IsAlreadyLoadedAttr As Boolean = False
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
                '' ''MyClass.ReportHistoryError()
                MyClass.PrepareErrorMode()
                Exit Function
            End If
            ' XBC 05/05/2011 - timeout limit repetitions

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)

            Select Case MyBase.CurrentMode

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

                Case ADJUSTMENT_MODES.LEVEL_DETECTING
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    End If
                    If pResponse = RESPONSE_TYPES.OK Then
                        If pData IsNot Nothing Then
                            Dim myDetected As Boolean = AnalyzerController.Instance.Analyzer.LevelDetected '#REFACTORING
                            If myDetected Then
                                '' ''MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.DETECTED
                                '' ''Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._ON
                                MyBase.DisplayMessage(Messages.SRV_LEVEL_DETECTED.ToString)
                            Else
                                '' ''MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DETECTED
                                '' ''Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._OFF
                                MyBase.DisplayMessage(Messages.SRV_LEVEL_NOT_DETECTED.ToString)
                            End If
                            AnalyzerController.Instance.Analyzer.LevelDetected = False '#REFACTORING
                            MyBase.CurrentMode = ADJUSTMENT_MODES.LEVEL_DETECTED
                            MyClass.PrepareArea()

                            System.Threading.Thread.Sleep(1000)

                            SendFwScript(MyBase.CurrentMode)
                        End If


                    End If

                Case ADJUSTMENT_MODES.LEVEL_DETECTED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    End If
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()
                        MyBase.myServiceMDI.SEND_INFO_START()
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    '' ''MyClass.ReportHistoryError()
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ManageReceptionEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function

    Private Sub BsInfoExpandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsInfoExpandButton.Click

        Try

            Dim myInfoPanel As Panel = Me.BsLevelDetectionInfoPanel
            Dim myAdjPanel As Panel = Me.BsLevelDetectionTestPanel
            Dim myExpandButton As Panel = Me.BsInfoExpandButton
            Dim myGlobal As GlobalDataTO = MyBase.ExpandInformation(Not MyClass.IsInfoExpanded, myInfoPanel, myAdjPanel, myExpandButton)
            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                IsInfoExpanded = CBool(myGlobal.SetDatos)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsInfoExpandButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsInfoExpandButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region


#Region "Refresh Event Handler"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDs"></param>
    ''' <remarks>
    ''' Created by SGM 23/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDs As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try
            myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDs)

            MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)
            PrepareArea()

            '' ''If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then

            '' ''    Dim myProbeValueChangedDT As New UIRefreshDS.ProbeValueChangedDataTable
            '' ''    myProbeValueChangedDT = pRefreshDs.ProbeValueChanged
            '' ''    For Each P As UIRefreshDS.ProbeValueChangedRow In myProbeValueChangedDT.Rows

            '' ''        Dim myFreqValue As Single = P.DetectionFrequency
            '' ''        Dim myDisplayLabel As Label = Nothing
            '' ''        Dim myArm As LevelDetectionTestDelegate.Arms

            '' ''        Select Case P.ProbeID.ToUpperBS.Trim  ' ToUpper.Trim

            '' ''            'SAMPLE
            '' ''            Case POLL_IDs.DM1.ToString
            '' ''                myArm = LevelDetectionTestDelegate.Arms.SAMPLE
            '' ''                '' ''myDisplayLabel = Me.BsFreqSampleValueLabel


            '' ''                'REAGENT 1
            '' ''            Case POLL_IDs.DR1.ToString
            '' ''                myArm = LevelDetectionTestDelegate.Arms.REAGENT1
            '' ''                myDisplayLabel = Me.lblLdminValue


            '' ''                'REAGENT 2
            '' ''            Case POLL_IDs.DR2.ToString
            '' ''                myArm = LevelDetectionTestDelegate.Arms.REAGENT2
            '' ''                '' ''myDisplayLabel = Me.BsFreqReagent2ValueLabel

            '' ''        End Select

            '' ''        '' ''MyClass.CheckFrequencyLimits(myArm, myDisplayLabel, myFreqValue)

            '' ''        If myArm = LevelDetectionTestDelegate.Arms.REAGENT2 Then
            '' ''            MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READED
            '' ''            MyClass.PrepareArea()
            '' ''        End If
            '' ''    Next


            '' ''End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region



#Region "Private Methods"

#Region "Common"

    ''' <summary>
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <remarks>Created by: XBC 22/03/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
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
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            Dim MLRD As New MultilanguageResourcesDelegate

            Me.BsTestTitleLabel.Text = MLRD.GetResourceText(Nothing, "MSG_SRV_LEVEL_DETECTION_TEST", currentLanguage)
            Me.BsTitleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_LEV_DET", currentLanguage)

            'TODO: ACR Pass to the Base Class
            Me.BsInfoTitle.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)

            '' ''Me.gpbLevelDetection.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FREQUENCY_READ", currentLanguage) & " (" & LevelDetectionTestDelegate.FREQ_UNIT & ")"
            '' ''Me.BsFreqSampleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_SAMPLE", currentLanguage)
            '' ''Me.lblLdmin.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent1", currentLanguage)
            '' ''Me.BsFreqReagent2Label.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent2", currentLanguage)

            '' ''Me.BsDetectionGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_DETECTION_TEST", currentLanguage)
            '' ''Me.BsDetectionArmLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_ARM", currentLanguage)
            '' ''Me.BsDetectionPosLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_POSITION", currentLanguage)
            '' ''Me.BsDetectedLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_DETECTED", currentLanguage)


            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            Dim MLRD As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.btnStartTest, MLRD.GetResourceText(Nothing, "SRV_BTN_TestStart", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.btnShowLevel, MLRD.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.btnCancel, MLRD.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.SetButtonImage(Me.btnStartTest, "ADJUSTMENT")
            MyBase.SetButtonImage(Me.btnShowLevel, "ACCEPT1")
            MyBase.SetButtonImage(Me.BsExitButton, "CANCEL")
            MyBase.SetButtonImage(Me.btnCancel, "UNDO")
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: SGM 15/12/2011</remarks>
    Private Sub DisableAll()
        Try

            Me.btnStartTest.Enabled = False
            Me.btnShowLevel.Enabled = False
            Me.btnCancel.Enabled = False
            Me.BsExitButton.Enabled = False

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableAll ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                    MyClass.PrepareLoadedMode()
                    MyClass.LoadAdjustmentsData()

                Case ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareLoadedMode()

                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_UP
                    MyClass.PrepareWashingStationIsUpMode()

                Case ADJUSTMENT_MODES.LD_WASH_STATION_IS_UP_TO_START
                    SetToStartedTestState()
                    '' ''MyClass.PrepareWashingStationIsUpMode()
                Case ADJUSTMENT_MODES.LD_WASH_STATION_NROTOR_DONE
                    WashStationToDown()

                Case ADJUSTMENT_MODES.FREQUENCY_READING
                    MyClass.PrepareFrequencyReading()

                Case ADJUSTMENT_MODES.FREQUENCY_READED
                    MyClass.PrepareFrequencyReaded()

                Case ADJUSTMENT_MODES.LEVEL_DETECTING
                    MyClass.PrepareDetectingMode()

                Case ADJUSTMENT_MODES.LEVEL_DETECTED
                    MyClass.PrepareDetectedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

            If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjustments Reading Mode
    ''' </summary>
    ''' <remarks>Created by SGM 15/12/2011</remarks>
    Private Sub PrepareAdjustReadingMode()
        Dim myResultData As New GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            DisableAll()
            Me.Cursor = Cursors.WaitCursor

            ' Reading Adjustments
            If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                ' Parent Reading Adjustments
                MyBase.ReadAdjustments()
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating...
                    'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                    'Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    'Me.Cursor = Cursors.Default
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                Else
                    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
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
            MyBase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try

            Me.btnStartTest.Enabled = True
            Me.btnShowLevel.Enabled = False
            Me.btnCancel.Enabled = False
            Me.BsExitButton.Enabled = True
            Me.lblLdminValue.Text = ""
            Me.lblLdmemValue.Text = ""

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            MyBase.myServiceMDI.Focus()
            '' ''Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI to Started test Mode
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetToStartedTestState()
        DisableAll()

        Me.btnShowLevel.Enabled = True
        Me.btnCancel.Enabled = True
        Me.lblLdminValue.Text = ""
        Me.lblLdmemValue.Text = ""
        Me.btnShowLevel.Cursor = Cursors.Default
        Me.btnCancel.Cursor = Cursors.Default
        Me.Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' Prepare GUI for Washing Station is Up Mode
    ''' </summary>
    ''' <remarks>Created by SGM 21/11/2011</remarks>
    Private Sub PrepareWashingStationIsUpMode()
        Try
            MyClass.myScreenDelegate.IsWashingStationDown = False

            If MyBase.SimulationMode Then

                '' ''MyClass.DisableCurrentPage()
                '' ''MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                '' ''myGlobal = MyBase.DisplayMessage(Messages.SRV_ALL_ITEMS_TO_DEFAULT.ToString)
                '' ''MyClass.PrepareArea()

                '' ''Me.Cursor = Cursors.WaitCursor
                '' ''System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                '' ''System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)

                '' ''MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                '' ''MyClass.PrepareArea()


            Else

                ' '' ''    MyClass.EnableCurrentPage()

                ' '' ''    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                ' '' ''    MyClass.PrepareArea()

                ' '' ''    'WS Aspiration or Dispensation SGM 18/05/2012
                ' '' ''    If MyClass.CurrentTestPanel Is Me.BsWSAspirationTestPanel Or MyClass.CurrentTestPanel Is Me.BsWSDispensationTestPanel Then
                ' '' ''        MyBase.DisplayMessage(Messages.SRV_MEBV_WS_IS_UP.ToString)
                ' '' ''    Else
                ' '' ''        MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                ' '' ''    End If

                ' '' ''        Else
                ' '' ''    MyClass.AllArmsToParking()
                ' '' ''End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWashingStationIsUpMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareWashingStationIsUpMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyClass.DisableAll()
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pGroupID"></param>
    ''' <param name="pAxis"></param>
    ''' <param name="pNotForDisplaying"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As String, ByVal pAxis As GlobalEnumerates.AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As FwAdjustmentsDataTO
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New Global.FwAdjustmentsDataTO("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "_NONE" Then myAxis = ""

            Dim myGroup As String = pGroupID
            If myGroup = "_NONE" Then myGroup = ""


            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyBase.myAllAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.GroupID.Trim = myGroup.Trim _
                                And a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.GroupID.Trim.ToUpper = myGroup.Trim.ToUpper _
            'And a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .AnalyzerID = myAdjustmentRows(0).AnalyzerID
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    .CanMove = myAdjustmentRows(0).CanMove
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If pNotForDisplaying Then
            If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"
        End If

        Return myAdjustmentRowData
    End Function

    Private Sub LoadAdjustmentsData()
        Try

            '' ''myScreenDelegate.S1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.S1RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.S1RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.S1RotorPosRing3 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.S1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.S1RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.S1RotorRing3HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.S1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.S1RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.S1RotorRing3DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.S1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.S1ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.S1ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            '' ''myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R1RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.R1RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.R1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R1RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R1RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R1ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R1ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            '' ''myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R2RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.R2RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            '' ''myScreenDelegate.R2RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R2RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R2RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R2RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            '' ''myScreenDelegate.R2ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            '' ''myScreenDelegate.R2ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            ''Washing NEEDED?
            'myScreenDelegate.S1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.S1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            'myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.R1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            'myScreenDelegate.R2WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.R2WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitScreen()
        Try

            Me.Close()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Frequency Read"

    Private Sub PrepareFrequencyReading()
        Try
            MyClass.DisableAll()

            Me.Cursor = Cursors.WaitCursor

            MyBase.myServiceMDI.SEND_INFO_STOP()

            Me.lblLdmemValue.ForeColor = Color.Gray
            Me.lblLdminValue.ForeColor = Color.Gray

            MyBase.DisplayMessage(Messages.SRV_FREQUENCY_READING.ToString)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " PrepareFrequencyReading ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub PrepareFrequencyReaded()
        Try

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            Me.btnStartTest.Enabled = True
            Me.btnStartTest.Enabled = True
            Me.gpbLevelDetection.Enabled = True

            Me.BsExitButton.Enabled = True

            MyBase.DisplayMessage(Messages.SRV_FREQUENCY_READED.ToString)

            MyBase.myServiceMDI.SEND_INFO_START()

            MyBase.myServiceMDI.Focus()

            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            MyClass.PrepareArea()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " PrepareFrequencyReaded ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    '' ''Private Function ReadFrequencies() As GlobalDataTO

    '' ''    Dim myGlobal As New GlobalDataTO

    '' ''    Try
    '' ''        MyClass.myScreenDelegate.CurrentHistoryArea = LevelDetectionReactionsRotorTestDelegate.HISTORY_AREAS.LEVEL_FREQ_READ
    '' ''        MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READING
    '' ''        MyClass.PrepareArea()

    '' ''        If MyBase.SimulationMode Then
    '' ''            '' ''Dim myFreqValue As Single

    '' ''            '' ''System.Threading.Thread.Sleep(100)
    '' ''            '' ''myFreqValue = 1880 + (85 * Rnd(999) * Rnd(1))
    '' ''            '' ''MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.SAMPLE, Me.BsFreqSampleValueLabel, myFreqValue)

    '' ''            '' ''System.Threading.Thread.Sleep(100)
    '' ''            '' ''myFreqValue = 883 + (102 * Rnd(939) * Rnd(2))
    '' ''            '' ''MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.REAGENT1, Me.lblLdminValue, myFreqValue)


    '' ''            '' ''System.Threading.Thread.Sleep(100)
    '' ''            '' ''myFreqValue = 805 + (152 * Rnd(299) * Rnd(9))
    '' ''            '' ''MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.REAGENT2, Me.BsFreqReagent2ValueLabel, myFreqValue)


    '' ''            '' ''MyBase.myServiceMDI.SEND_INFO_START()

    '' ''            '' ''MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READED
    '' ''            '' ''MyClass.PrepareArea()

    '' ''        Else

    '' ''            myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionReactionsRotorTestDelegate.Arms.SAMPLE)
    '' ''            Application.DoEvents()
    '' ''            'System.Threading.Thread.Sleep(100)
    '' ''            myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionReactionsRotorTestDelegate.Arms.REAGENT1)
    '' ''            Application.DoEvents()
    '' ''            'System.Threading.Thread.Sleep(100)
    '' ''            myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionReactionsRotorTestDelegate.Arms.REAGENT2)

    '' ''        End If

    '' ''    Catch ex As Exception
    '' ''        myGlobal.HasError = True
    '' ''        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '' ''        myGlobal.ErrorMessage = ex.Message
    '' ''        GlobalBase.CreateLogActivity(ex.Message, Me.Name & " ReadHardwareValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '' ''        MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '' ''    End Try
    '' ''    Return myGlobal
    '' ''End Function



    Private Sub SetFrequencyTooltip(ByVal pArm As LevelDetectionTestDelegate.Arms, ByVal pIsOut As Boolean)
        Try
            '' ''Dim MLRD As New MultilanguageResourcesDelegate
            '' ''Dim myLabel As Label = Nothing

            '' ''Select Case pArm
            '' ''    Case LevelDetectionTestDelegate.Arms.SAMPLE : myLabel = Me.BsFreqSampleValueLabel
            '' ''    Case LevelDetectionTestDelegate.Arms.REAGENT1 : myLabel = Me.lblLdminValue
            '' ''    Case LevelDetectionTestDelegate.Arms.REAGENT2 : myLabel = Me.BsFreqReagent2ValueLabel
            '' ''End Select

            '' ''If myLabel IsNot Nothing Then
            '' ''    If pIsOut Then
            '' ''        MyBase.bsScreenToolTipsControl.SetToolTip(myLabel, MLRD.GetResourceText(Nothing, "LBL_SRV_FREQ_OUT_LIMITS", currentLanguage))
            '' ''    Else
            '' ''        MyBase.bsScreenToolTipsControl.SetToolTip(myLabel, MLRD.GetResourceText(Nothing, "LBL_SRV_FREQ_IN_LIMITS", currentLanguage))
            '' ''    End If
            '' ''End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SetFrequencyTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SetFrequencyTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region


#Region "Detection Test"

    Private Sub PrepareDetectingMode()
        Try
            MyClass.DisableAll()

            Me.Cursor = Cursors.WaitCursor

            MyBase.myServiceMDI.SEND_INFO_STOP()

            '' ''Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED

            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

            MyBase.myServiceMDI.Focus()
            '' ''Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " PrepareDetectingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub PrepareDetectedMode()

        Try

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            Me.btnStartTest.Enabled = True
            '' ''Me.BsDetectionButton.Enabled = True
            '' ''Me.BsDetectionGroupBox.Enabled = True

            Me.BsExitButton.Enabled = True

            'MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            MyBase.myServiceMDI.Focus()
            '' ''Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " PrepareDetectedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub SetCurrentArmRotor()
        Dim myGlobal As New GlobalDataTO
        Try
            With MyClass.myScreenDelegate
                '' ''Select Case BsDetectionArmComboBox.SelectedIndex
                '' ''    Case 0 : .CurrentArm = LevelDetectionTestDelegate.Arms.SAMPLE : .CurrentRotor = LevelDetectionTestDelegate.Rotors.SAMPLES
                '' ''    Case 1 : .CurrentArm = LevelDetectionTestDelegate.Arms.REAGENT1 : .CurrentRotor = LevelDetectionTestDelegate.Rotors.REAGENTS
                '' ''    Case 2 : .CurrentArm = LevelDetectionTestDelegate.Arms.REAGENT2 : .CurrentRotor = LevelDetectionTestDelegate.Rotors.REAGENTS
                '' ''    Case Else : Exit Sub
                '' ''End Select
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " SetCurrentArmRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Must Inherited"
    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'TODO

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Events"

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        PrepareLoadedMode()
    End Sub


    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 15/12/2011</remarks>
    Private Sub ILevelDetectionTest_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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
            MyBase.ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IlevelDetectionTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try

            MyBase.MyBase_Load(sender, e)

            '' ''MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            MyClass.currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            MyClass.GetScreenLabels()

            MyClass.PrepareButtons()


            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New LevelDetectionReactionsRotorTestDelegate(MyBase.AnalyzerModel, myFwScriptDelegate)
            '' ''MyClass.myScreenDelegate.CurrentRotorPosition = CInt(Me.BsDetectionPosUpDown.Value)

            'Frequency Limits
            '' ''MyClass.myScreenDelegate.GetFrequencyLimits()

            ' '' ''Rotors' positions
            '' ''myGlobal = MyClass.myScreenDelegate.LoadRotorsConfiguration()
            '' ''If Not myGlobal.HasError Then
            '' ''    myGlobal = MyClass.myScreenDelegate.LoadRotorPositions(LevelDetectionTestDelegate.Rotors.SAMPLES)
            '' ''    myGlobal = MyClass.myScreenDelegate.LoadRotorPositions(LevelDetectionTestDelegate.Rotors.REAGENTS)
            '' ''End If

            '' ''Me.BsDetectionArmComboBox.SelectedIndex = 0

            MyClass.DisableAll()

            'Initialize homes SGM 20/09/2011
            '' ''MyClass.InitializeHomes()

            '' ''Me.BsDetectionArmComboBox.SelectedIndex = 0

            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myGlobal.ErrorCode = "ERROR_COMM"
                myGlobal.HasError = True
            Else
                PrepareAdjustReadingMode()
            End If

            'Information
            MyBase.DisplayInformation(APPLICATION_PAGES.LEVEL_DETECTION, Me.BsInfoXPSViewer)

            If myGlobal.HasError Then
                PrepareErrorMode()
                MyBase.ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            MyBase.ResetBorderSRV()

            MyClass.IsAlreadyLoadedAttr = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub btnShowLevel_Click(sender As Object, e As EventArgs) Handles btnShowLevel.Click
        Me.StartLevelDetectionTest()
    End Sub

    Private Sub btnStartTest_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStartTest.Click
        Dim myGlobal As New GlobalDataTO
        Try
            Me.StartingLevelDetectionTest()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".btnStartTest_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".btnStartTest_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub StartLevelDetectionTest()
        DisableAll()
        Me.CurrentMode = ADJUSTMENT_MODES.LD_WASH_STATION_TO_NROTOR
        myScreenDelegate.SendNEW_ROTOR()
    End Sub

    Private Sub WashStationToDown()

    End Sub


    ''' <summary>
    ''' Brings the Washing Station Up, disable screen and displays Info
    ''' </summary>
    ''' <remarks>SGM 21/11/2011</remarks>
    Private Sub StartingLevelDetectionTest()
        Try
            If MyBase.SimulationMode Then Exit Sub

            MyBase.DisplayMessage(Messages.SRV_WS_TO_UP.ToString)

            Me.myScreenDelegate.IsWashingStationDown = False

            Me.DisableAll()

            Me.CurrentMode = ADJUSTMENT_MODES.LD_WASH_STATION_TO_UP_TO_START

            '' ''Me.OnWashingStationToUpTimerTick()
            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WashingStationToDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WashingStationToDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    '' ''Private Sub OnWashingStationToUpTimerTick()
    '' ''    Dim myGlobal As New GlobalDataTO
    '' ''    Try
    '' ''        'MyClass.IsActionRequested = True

    '' ''        If Not myGlobal.HasError Then
    '' ''            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
    '' ''            'MyClass.SendFwScript(Me.CurrentMode)
    '' ''            'Me.Cursor = Cursors.WaitCursor
    '' ''        Else
    '' ''            MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
    '' ''            MyClass.PrepareArea()
    '' ''        End If

    '' ''    Catch ex As Exception
    '' ''        myGlobal.HasError = True
    '' ''        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '' ''        myGlobal.ErrorMessage = ex.Message
    '' ''        GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnWashingStationToUpTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '' ''        MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '' ''    End Try
    '' ''End Sub


    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.ExitScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub Escape_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
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
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region



End Class
