Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL

Public Class ILevelDetectionTest
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As LevelDetectionTestDelegate
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
                MyClass.ReportHistoryError()
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
                            Dim myDetected As Boolean = Ax00ServiceMainMDI.MDIAnalyzerManager.LevelDetected
                            If myDetected Then
                                MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.DETECTED
                                Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._ON
                                MyBase.DisplayMessage(Messages.SRV_LEVEL_DETECTED.ToString)
                            Else
                                MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DETECTED
                                Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._OFF
                                MyBase.DisplayMessage(Messages.SRV_LEVEL_NOT_DETECTED.ToString)
                            End If
                            Ax00ServiceMainMDI.MDIAnalyzerManager.LevelDetected = False
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
                    MyClass.ReportHistoryError()
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()

            End Select

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsInfoExpandButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsInfoExpandButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region


#Region "Refresh Event Handler"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by SGM 23/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try

            myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)


            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then

                Dim myProbeValueChangedDT As New UIRefreshDS.ProbeValueChangedDataTable
                myProbeValueChangedDT = pRefreshDS.ProbeValueChanged
                For Each P As UIRefreshDS.ProbeValueChangedRow In myProbeValueChangedDT.Rows

                    Dim myFreqValue As Single = P.DetectionFrequency
                    Dim myDisplayLabel As Label
                    Dim myArm As LevelDetectionTestDelegate.Arms

                    Select Case P.ProbeID.ToUpperBS.Trim  ' ToUpper.Trim

                        'SAMPLE
                        Case POLL_IDs.DM1.ToString
                            myArm = LevelDetectionTestDelegate.Arms.SAMPLE
                            myDisplayLabel = Me.BsFreqSampleValueLabel


                            'REAGENT 1
                        Case POLL_IDs.DR1.ToString
                            myArm = LevelDetectionTestDelegate.Arms.REAGENT1
                            myDisplayLabel = Me.BsFreqReagent1ValueLabel


                            'REAGENT 2
                        Case POLL_IDs.DR2.ToString
                            myArm = LevelDetectionTestDelegate.Arms.REAGENT2
                            myDisplayLabel = Me.BsFreqReagent2ValueLabel

                    End Select

                    MyClass.CheckFrequencyLimits(myArm, myDisplayLabel, myFreqValue)

                    If myArm = LevelDetectionTestDelegate.Arms.REAGENT2 Then
                        MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READED
                        MyClass.PrepareArea()
                    End If
                Next

                
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region



#Region "Private Methods"

#Region "Common"

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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function


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
                MyBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

            Me.BsFrequencyReadGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_FREQUENCY_READ", currentLanguage) & " (" & LevelDetectionTestDelegate.FREQ_UNIT & ")"
            Me.BsFreqSampleLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_SAMPLE", currentLanguage)
            Me.BsFreqReagent1Label.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent1", currentLanguage)
            Me.BsFreqReagent2Label.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent2", currentLanguage)

            Me.BsDetectionGroupBox.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_DETECTION_TEST", currentLanguage)
            Me.BsDetectionArmLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_ARM", currentLanguage)
            Me.BsDetectionPosLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_POSITION", currentLanguage)
            Me.BsDetectedLabel.Text = MLRD.GetResourceText(Nothing, "LBL_SRV_DETECTED", currentLanguage)


            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyBase.bsScreenToolTips.SetToolTip(Me.BsFrequencyButton, MLRD.GetResourceText(Nothing, "SRV_BTN_ReadFrequency", currentLanguage))
            MyBase.bsScreenToolTips.SetToolTip(Me.BsDetectionButton, MLRD.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

            MyBase.SetButtonImage(Me.BsFrequencyButton, "ADJUSTMENT")
            MyBase.SetButtonImage(Me.BsDetectionButton, "ADJUSTMENT")
            MyBase.SetButtonImage(Me.BsExitButton, "CANCEL")


            ''ADJUST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Me.BsFrequencyButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'Me.BsFrequencyButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''SAVE Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Me.BsDetectionButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'Me.BsDetectionButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'BsExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If


        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>Created by: SGM 15/12/2011</remarks>
    Private Sub DisableAll()
        Try

            Me.BsFrequencyButton.Enabled = False
            Me.BsDetectionButton.Enabled = False

            Me.BsExitButton.Enabled = False

            MyBase.ActivateMDIMenusButtons(False)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

            If Not MyBase.SimulationMode And Ax00ServiceMainMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try
            If myScreenDelegate.CurrentHistoryArea = LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_FREQ_READ Or _
            myScreenDelegate.CurrentHistoryArea = LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_DET_TEST Then
                MyClass.ReportHistory()
            End If

            MyClass.myScreenDelegate.CurrentOperation = LevelDetectionTestDelegate.OPERATIONS._NONE
            MyClass.myScreenDelegate.CurrentHistoryArea = LevelDetectionTestDelegate.HISTORY_AREAS.NONE

            Me.BsFrequencyButton.Enabled = True

            Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED
            Me.BsDetectionButton.Enabled = True
            Me.BsDetectionArmComboBox.Enabled = True
            Me.BsDetectionPosUpDown.Enabled = True

            Me.BsExitButton.Enabled = True

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            MyBase.myServiceMDI.Focus()
            Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If pNotForDisplaying Then
            If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"
        End If

        Return myAdjustmentRowData
    End Function

    Private Sub LoadAdjustmentsData()
        Try

            myScreenDelegate.S1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.S1RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.S1RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.S1RotorPosRing3 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.S1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.S1RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.S1RotorRing3HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.S1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.S1RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.S1RotorRing3DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.S1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.S1ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.S1ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R1RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.R1RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.R1RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R1RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R1RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R1RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R1VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R1ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R1ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R2RotorPosRing1 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.R2RotorPosRing2 = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.ROTOR, True).Value)
            myScreenDelegate.R2RotorRing1HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R2RotorRing2HorPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R2RotorRing1DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R2RotorRing2DetMaxVerPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R2VerticalSafetyPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            myScreenDelegate.R2ParkingPolar = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            myScreenDelegate.R2ParkingZ = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString, GlobalEnumerates.AXIS.Z, True).Value)

            ''Washing NEEDED?
            'myScreenDelegate.S1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.S1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            'myScreenDelegate.R1WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.R1WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)
            'myScreenDelegate.R2WSHorizontalPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.POLAR, True).Value)
            'myScreenDelegate.R2WSVerticalRefPos = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString, GlobalEnumerates.AXIS.Z, True).Value)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    Private Sub ExitScreen()
        Try

            Me.Close()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

            Me.BsFreqSampleValueLabel.ForeColor = Color.Gray
            Me.BsFreqReagent1ValueLabel.ForeColor = Color.Gray
            Me.BsFreqReagent2ValueLabel.ForeColor = Color.Gray

            MyBase.DisplayMessage(Messages.SRV_FREQUENCY_READING.ToString)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " PrepareFrequencyReading ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub PrepareFrequencyReaded()
        Try

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            Me.BsFrequencyButton.Enabled = True
            Me.BsDetectionButton.Enabled = True
            Me.BsDetectionGroupBox.Enabled = True

            Me.BsExitButton.Enabled = True

            MyBase.DisplayMessage(Messages.SRV_FREQUENCY_READED.ToString)

            MyBase.myServiceMDI.SEND_INFO_START()

            MyBase.myServiceMDI.Focus()

            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            MyClass.PrepareArea()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " PrepareFrequencyReaded ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub






    Private Function ReadFrequencies() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.myScreenDelegate.CurrentHistoryArea = LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_FREQ_READ
            MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READING
            MyClass.PrepareArea()

            If MyBase.SimulationMode Then
                Dim myFreqValue As Single

                System.Threading.Thread.Sleep(100)
                myFreqValue = 1880 + (85 * Rnd(999) * Rnd(1))
                MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.SAMPLE, Me.BsFreqSampleValueLabel, myFreqValue)

                System.Threading.Thread.Sleep(100)
                myFreqValue = 883 + (102 * Rnd(939) * Rnd(2))
                MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.REAGENT1, Me.BsFreqReagent1ValueLabel, myFreqValue)


                System.Threading.Thread.Sleep(100)
                myFreqValue = 805 + (152 * Rnd(299) * Rnd(9))
                MyClass.CheckFrequencyLimits(LevelDetectionTestDelegate.Arms.REAGENT2, Me.BsFreqReagent2ValueLabel, myFreqValue)


                MyBase.myServiceMDI.SEND_INFO_START()

                MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READED
                MyClass.PrepareArea()

            Else

                myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionTestDelegate.Arms.SAMPLE)
                Application.DoEvents()
                'System.Threading.Thread.Sleep(100)
                myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionTestDelegate.Arms.REAGENT1)
                Application.DoEvents()
                'System.Threading.Thread.Sleep(100)
                myGlobal = MyClass.myScreenDelegate.REQUEST_FREQUENCY_INFO(LevelDetectionTestDelegate.Arms.REAGENT2)

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & " ReadHardwareValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    Private Sub CheckFrequencyLimits(ByVal pArm As LevelDetectionTestDelegate.Arms, ByVal pLabel As Label, ByVal pValue As Single)

        Dim myResult As LevelDetectionTestDelegate.HISTORY_RESULTS = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE

        Try
            If pLabel IsNot Nothing Then

                Dim isOK As Boolean

                Select Case pArm
                    Case LevelDetectionTestDelegate.Arms.SAMPLE
                        isOK = (pValue > MyClass.myScreenDelegate.SampleFreqMinLimit And pValue < MyClass.myScreenDelegate.SampleFreqMaxLimit)
                        myScreenDelegate.SampleFrequencyValue = pValue

                    Case LevelDetectionTestDelegate.Arms.REAGENT1
                        isOK = (pValue > MyClass.myScreenDelegate.ReagentsFreqMinLimit And pValue < MyClass.myScreenDelegate.ReagentsFreqMaxLimit)
                        myScreenDelegate.Reagent1FrequencyValue = pValue

                    Case LevelDetectionTestDelegate.Arms.REAGENT2
                        isOK = (pValue > MyClass.myScreenDelegate.ReagentsFreqMinLimit And pValue < MyClass.myScreenDelegate.ReagentsFreqMaxLimit)
                        myScreenDelegate.Reagent2FrequencyValue = pValue

                    Case Else
                        Exit Sub

                End Select

                pLabel.Text = CInt(pValue).ToString


                If isOK Then
                    pLabel.ForeColor = Color.LimeGreen
                    myResult = LevelDetectionTestDelegate.HISTORY_RESULTS.FREQ_OK
                Else
                    pLabel.ForeColor = Color.Red
                    myResult = LevelDetectionTestDelegate.HISTORY_RESULTS.FREQ_NOK
                End If

                Select Case pArm
                    Case LevelDetectionTestDelegate.Arms.SAMPLE : MyClass.myScreenDelegate.SampleFrequencyReadResult = myResult
                    Case LevelDetectionTestDelegate.Arms.REAGENT1 : MyClass.myScreenDelegate.Reagent1FrequencyReadResult = myResult
                    Case LevelDetectionTestDelegate.Arms.REAGENT2 : MyClass.myScreenDelegate.Reagent2FrequencyReadResult = myResult
                End Select

                pLabel.Refresh()

                MyClass.SetFrequencyTooltip(pArm, Not isOK)

                MyBase.myServiceMDI.Focus()

            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " CheckFrequencyLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try

    End Sub

    Private Sub SetFrequencyTooltip(ByVal pArm As LevelDetectionTestDelegate.Arms, ByVal pIsOut As Boolean)
        Try
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim myLabel As Label

            Select Case pArm
                Case LevelDetectionTestDelegate.Arms.SAMPLE : myLabel = Me.BsFreqSampleValueLabel
                Case LevelDetectionTestDelegate.Arms.REAGENT1 : myLabel = Me.BsFreqReagent1ValueLabel
                Case LevelDetectionTestDelegate.Arms.REAGENT2 : myLabel = Me.BsFreqReagent2ValueLabel
            End Select

            If myLabel IsNot Nothing Then
                If pIsOut Then
                    MyBase.bsScreenToolTips.SetToolTip(myLabel, MLRD.GetResourceText(Nothing, "LBL_SRV_FREQ_OUT_LIMITS", currentLanguage))
                Else
                    MyBase.bsScreenToolTips.SetToolTip(myLabel, MLRD.GetResourceText(Nothing, "LBL_SRV_FREQ_IN_LIMITS", currentLanguage))
                End If
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".SetFrequencyTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SetFrequencyTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region


#Region "Detection Test"

    Private Sub InitializeArmsCombo()
        Try
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim myArm As String

            Me.BsDetectionArmComboBox.Enabled = False

            Me.BsDetectionArmComboBox.Items.Clear()

            myArm = MLRD.GetResourceText(Nothing, "LBL_SRV_SAMPLE", currentLanguage)
            If myArm.Length > 0 Then Me.BsDetectionArmComboBox.Items.Add(myArm)

            myArm = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent1", currentLanguage)
            If myArm.Length > 0 Then Me.BsDetectionArmComboBox.Items.Add(myArm)

            myArm = MLRD.GetResourceText(Nothing, "LBL_SRV_Reagent2", currentLanguage)
            If myArm.Length > 0 Then Me.BsDetectionArmComboBox.Items.Add(myArm)

            Me.BsDetectionArmComboBox.Enabled = True

            Me.BsDetectionArmComboBox.SelectedIndex = -1

            MyBase.myServiceMDI.Focus()
            Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Name & ".InitializeArmsCombo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".InitializeArmsCombo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try


    End Sub

    Private Sub PrepareDetectingMode()
        Try
            MyClass.DisableAll()

            Me.Cursor = Cursors.WaitCursor

            MyBase.myServiceMDI.SEND_INFO_STOP()

            Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED

            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

            MyBase.myServiceMDI.Focus()
            Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " PrepareDetectingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

   
    Private Sub PrepareDetectedMode()

        Try

            MyBase.ActivateMDIMenusButtons(True)

            Me.Cursor = Cursors.Default

            Me.BsFrequencyButton.Enabled = True
            Me.BsDetectionButton.Enabled = True
            Me.BsDetectionGroupBox.Enabled = True

            Me.BsExitButton.Enabled = True

            'MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            MyBase.myServiceMDI.Focus()
            Me.BsDetectionArmComboBox.SelectionLength = 0

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " PrepareDetectedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub SetCurrentArmRotor()
        Dim myGlobal As New GlobalDataTO
        Try
            With MyClass.myScreenDelegate
                Select Case BsDetectionArmComboBox.SelectedIndex
                    Case 0 : .CurrentArm = LevelDetectionTestDelegate.Arms.SAMPLE : .CurrentRotor = LevelDetectionTestDelegate.Rotors.SAMPLES
                    Case 1 : .CurrentArm = LevelDetectionTestDelegate.Arms.REAGENT1 : .CurrentRotor = LevelDetectionTestDelegate.Rotors.REAGENTS
                    Case 2 : .CurrentArm = LevelDetectionTestDelegate.Arms.REAGENT2 : .CurrentRotor = LevelDetectionTestDelegate.Rotors.REAGENTS
                    Case Else : Exit Sub
                End Select
            End With

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " SetCurrentArmRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

   
#End Region


#Region "History Methods"


    ''' <summary>
    ''' Updates the screen delegate's properties used for History management
    ''' </summary>
    ''' <param name="pResult"></param>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistory(Optional ByVal pResult As LevelDetectionTestDelegate.HISTORY_RESULTS = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE, _
                              Optional ByVal pArm As LevelDetectionTestDelegate.Arms = LevelDetectionTestDelegate.Arms._NONE)
        Try


            Select Case MyClass.myScreenDelegate.CurrentHistoryArea

                Case LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_FREQ_READ
                    If pResult <> LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE Then
                        Select Case pArm
                            Case LevelDetectionTestDelegate.Arms.SAMPLE : MyClass.myScreenDelegate.SampleFrequencyReadResult = pResult
                            Case LevelDetectionTestDelegate.Arms.REAGENT1 : MyClass.myScreenDelegate.Reagent1FrequencyReadResult = pResult
                            Case LevelDetectionTestDelegate.Arms.REAGENT2 : MyClass.myScreenDelegate.Reagent2FrequencyReadResult = pResult
                        End Select
                    End If

                Case LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_DET_TEST
                    If pResult <> LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE Then
                        MyClass.myScreenDelegate.DetectionTestResult = pResult
                    End If

            End Select


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

            MyClass.ReportHistory(LevelDetectionTestDelegate.HISTORY_RESULTS._ERROR)

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

                ''results
                .SampleFrequencyReadResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE
                .Reagent1FrequencyReadResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE
                .Reagent2FrequencyReadResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE
                .DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DONE
            End With
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Events"



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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IlevelDetectionTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Dim myGlobalbase As New GlobalBase
        Try

            MyBase.MyBase_Load(sender, e)

            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            MyClass.currentLanguage = myGlobalbase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            MyClass.GetScreenLabels()

            MyClass.PrepareButtons()

            'Load Arms combo
            MyClass.InitializeArmsCombo()

            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New LevelDetectionTestDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)
            MyClass.myScreenDelegate.CurrentRotorPosition = CInt(Me.BsDetectionPosUpDown.Value)

            'Frequency Limits
            MyClass.myScreenDelegate.GetFrequencyLimits()

            'Rotors' positions
            myGlobal = MyClass.myScreenDelegate.LoadRotorsConfiguration()
            If Not myGlobal.HasError Then
                myGlobal = MyClass.myScreenDelegate.LoadRotorPositions(LevelDetectionTestDelegate.Rotors.SAMPLES)
                myGlobal = MyClass.myScreenDelegate.LoadRotorPositions(LevelDetectionTestDelegate.Rotors.REAGENTS)
            End If

            Me.BsDetectionArmComboBox.SelectedIndex = 0

            MyClass.DisableAll()

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            Me.BsDetectionArmComboBox.SelectedIndex = 0

            ' Check communications with Instrument
            If Not Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsFrequencyButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsFrequencyButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.ClearHistoryData()
            myGlobal = MyClass.ReadFrequencies()

            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsFrequencyButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsFrequencyButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.ExitScreen()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

   

    Private Sub BsDetectionArmComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsDetectionArmComboBox.SelectedIndexChanged

        Dim myGlobal As New GlobalDataTO

        Try
            If Not BsDetectionArmComboBox.Enabled Or _
            BsDetectionArmComboBox.SelectedIndex < 0 Or _
            MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READING Or _
            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING Then
                Exit Sub
            End If

            MyClass.SetCurrentArmRotor()

            Select Case MyClass.myScreenDelegate.CurrentArm
                Case LevelDetectionTestDelegate.Arms.SAMPLE
                    Me.BsDetectionPosUpDown.Minimum = MyClass.myScreenDelegate.SampleRotorFirstPosition
                    Me.BsDetectionPosUpDown.Maximum = MyClass.myScreenDelegate.SampleRotorLastPosition
                    Me.BsDetectionPosUpDown.Value = MyClass.myScreenDelegate.SampleRotorFirstPosition

                Case LevelDetectionTestDelegate.Arms.REAGENT1, LevelDetectionTestDelegate.Arms.REAGENT2
                    Me.BsDetectionPosUpDown.Minimum = MyClass.myScreenDelegate.ReagentsRotorFirstPosition
                    Me.BsDetectionPosUpDown.Maximum = MyClass.myScreenDelegate.ReagentsRotorLastPosition
                    Me.BsDetectionPosUpDown.Value = MyClass.myScreenDelegate.ReagentsRotorFirstPosition

            End Select

            

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsDetectionArmComboBox_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsDetectionArmComboBox_SelectedValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsDetectionPosUpDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsDetectionPosUpDown.ValueChanged
        Try
            If Not BsDetectionPosUpDown.Enabled Or _
           MyBase.CurrentMode = ADJUSTMENT_MODES.FREQUENCY_READING Or _
           MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING Then
                Exit Sub
            End If

            If Me.BsDetectionPosUpDown.Value < Me.BsDetectionPosUpDown.Minimum Then Me.BsDetectionPosUpDown.Value = Me.BsDetectionPosUpDown.Minimum
            If Me.BsDetectionPosUpDown.Value > Me.BsDetectionPosUpDown.Maximum Then Me.BsDetectionPosUpDown.Value = Me.BsDetectionPosUpDown.Maximum

            If IsAlreadyLoadedAttr Then
                myScreenDelegate.CurrentRotorPosition = CInt(Me.BsDetectionPosUpDown.Value)
            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsDetectionPosUpDown_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsDetectionPosUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsDetectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsDetectionButton.Click

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.ClearHistoryData()
            MyClass.myScreenDelegate.CurrentHistoryArea = LevelDetectionTestDelegate.HISTORY_AREAS.LEVEL_DET_TEST
            MyBase.CurrentMode = ADJUSTMENT_MODES.LEVEL_DETECTING
            MyClass.PrepareArea()

            If MyBase.SimulationMode Then

                System.Threading.Thread.Sleep(2000)
                Dim myDetected As Integer = CInt(Rnd(1))

                If myDetected > 0 Then
                    MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.DETECTED
                    Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._ON
                    MyBase.DisplayMessage(Messages.SRV_LEVEL_DETECTED.ToString)
                Else
                    MyClass.myScreenDelegate.DetectionTestResult = LevelDetectionTestDelegate.HISTORY_RESULTS.NOT_DETECTED
                    Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._OFF
                    MyBase.DisplayMessage(Messages.SRV_LEVEL_NOT_DETECTED.ToString)
                End If

                System.Threading.Thread.Sleep(1000)
                MyBase.CurrentMode = ADJUSTMENT_MODES.LEVEL_DETECTED
                MyClass.PrepareArea()

                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()
                MyBase.myServiceMDI.SEND_INFO_START()


            Else

                If MyClass.myScreenDelegate.CurrentArm = LevelDetectionTestDelegate.Arms._NONE Or _
                MyClass.myScreenDelegate.CurrentRotor = LevelDetectionTestDelegate.Rotors._NONE Then
                    MyClass.SetCurrentArmRotor()
                End If
                MyClass.myScreenDelegate.CurrentRotorPosition = CInt(Me.BsDetectionPosUpDown.Value)

                ' Manage FwScripts must to be sent to send Conditioning instructions to Instrument
                SendFwScript(Me.CurrentMode)

            End If

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsDetectionButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsDetectionButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region

   
   
    
    
End Class
