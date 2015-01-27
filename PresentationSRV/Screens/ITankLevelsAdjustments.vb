Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls
'Imports System.Runtime.InteropServices 'WIN32

Public Class UiTankLevelsAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm


#Region "Enumerations"
    Private Enum ADJUSTMENT_PAGES
        _NONE
        SCALES
        INTERMEDIATE
    End Enum

    Private Enum INTERMEDIATE_TANKS
        _NONE
        DISTILLED_WATER
        LOW_CONTAMINATION
    End Enum

    Private Enum TEST_PROCESS_STEPS
        NONE = 0
        UNDEFINED = 1
        INITIATED = 2
        LOW_CONTAMINATION_EMPTYING = 3
        LOW_CONTAMINATION_EMPTY = 4
        DISTILLED_WATER_FILLING = 5
        DISTILLED_WATER_FULL = 6
        TRANSFERING = 7
        FINALIZING = 8
        FINISHED = 9
    End Enum

    Private Enum PW_INPUTS
        FROM_TANK
        FROM_SOURCE
    End Enum

    Private Enum HISTORY_TASKS
        ADJUSTMENT = 1
        TEST = 2
    End Enum

    'Private Enum HISTORY_RESULTS
    '    _ERROR = -1
    '    NOT_DONE = 0
    '    OK = 1
    '    NOK = 2
    'End Enum

#End Region

#Region "Declarations"

    'Screen's business delegate
    Private WithEvents myScreenDelegate As TankLevelsAdjustmentDelegate

    Private SelectedPage As UiTankLevelsAdjustments.ADJUSTMENT_PAGES = ADJUSTMENT_PAGES.SCALES

    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private TemporalAdjustmentsDS As New SRVAdjustmentsDS

    Private WSSelectedAdjustmentsDS As New SRVAdjustmentsDS
    Private HCSelectedAdjustmentsDS As New SRVAdjustmentsDS

    Private TempToSendAdjustmentsDelegate As FwAdjustmentsDelegate


    ' Value ranges
    'Private WSLimitMin As Single
    'Private WSLimitMax As Single
    'Private HCLimitMin As Single
    'Private HCLimitMax As Single
    Private SCALESDINAMICRANGE As Single
    Private ScalesLimitMin As Single
    Private ScalesLimitMax As Single

    ' Edited value
    Private EditedValues As New List(Of EditedValueStruct) 'SGM 13/03/11


    Private TanksLevelsStatus As LevelsStatus 'SGM 15/03/11
    Private Event TankLevelChanged(ByVal pTank As INTERMEDIATE_TANKS)
    Private Event TankLevelUndefined(ByVal pTank As INTERMEDIATE_TANKS)

    'Private Event ProcessStepUndefined()       is used ???
    Private Event ProcessTimeOut(ByVal pStep As TEST_PROCESS_STEPS)

    ' Language
    Private LanguageId As String

    'Private TIME_TEST_LC_EMPTYING As Integer 'seconds
    'Private TIME_TEST_DW_FILLING As Integer  'seconds
    'Private TIME_TEST_TRANSFER As Integer  'seconds

    Private CurrentTask As GlobalEnumerates.PreloadedMasterDataEnum

    'Information
    Private IsInfoExpanded As Boolean = False
    Private SelectedInfoPanel As Panel
    Private SelectedAdjPanel As Panel
    Private SelectedInfoExpandButton As Panel

    Private myCountsText As String = "counts"
#End Region

#Region "Structures"

    Private Structure EditedValueStruct
        ' Adjustment
        Public AdjustmentID As GlobalEnumerates.ADJUSTMENT_GROUPS
        'Public TankID As TankLevelsAdjustments.TANKS
        Public LevelID As AXIS

        ' Saved values
        Public LastLevelValue As Single

        ' Current values
        Public CurrentLevelValue As Single

        ' New values
        Public NewLevelValue As Single

        Public CanSaveLevelValue As Boolean

    End Structure

    'SGM 28/01/11
    Private Structure AdjustmentRowData
        Public AnalyzerID As String
        Public CodeFw As String
        Public Value As String
        Public GroupID As String
        Public AxisID As String
        Public InFile As Boolean
        Public CanSave As Boolean


        Public Sub New(ByVal pCodeFw As String)
            CodeFw = pCodeFw
            AnalyzerID = ""
            Value = ""
            GroupID = ""
            InFile = False
        End Sub

        Public Sub Clear()
            CodeFw = ""
            AnalyzerID = ""
            Value = ""
            GroupID = ""
            InFile = False
        End Sub
    End Structure

    'SGM 15/03/11
    Private Structure LevelsStatus
        Public DW_Level As BSMonitorTankLevels.TankLevels
        Public LC_Level As BSMonitorTankLevels.TankLevels
        Private DW_Top As Boolean
        Private DW_Bottom As Boolean
        Private LC_Top As Boolean
        Private LC_Bottom As Boolean

        Public Sub New(ByVal p As Boolean)
            Try

                DW_Top = False
                DW_Bottom = False
                LC_Top = False
                LC_Bottom = False

                DW_Level = BSMonitorTankLevels.TankLevels.MIDDLE
                LC_Level = BSMonitorTankLevels.TankLevels.MIDDLE

            Catch ex As Exception
                Throw (ex)
            End Try
        End Sub

        Public Property DWTop() As Boolean
            Get
                Return DW_Top
            End Get
            Set(ByVal value As Boolean)
                DW_Top = Not value  '!!!!!INVERSE LOGIC for TOP sensors

            End Set
        End Property

        Public Property DWBottom() As Boolean
            Get
                Return DW_Bottom
            End Get
            Set(ByVal value As Boolean)
                DW_Bottom = value

            End Set
        End Property

        Public Property LCTop() As Boolean
            Get
                Return LC_Top
            End Get
            Set(ByVal value As Boolean)
                LC_Top = Not value  '!!!!!INVERSE LOGIC for TOP sensors

            End Set
        End Property

        Public Property LCBottom() As Boolean
            Get
                Return LC_Bottom
            End Get
            Set(ByVal value As Boolean)
                LC_Bottom = value

            End Set
        End Property



        ''' <summary>
        '''  The TOP Detector is ON (0) when the Level is over the detector
        '''  The BOTTOM Detector is ON (1) when the Level is under the detector
        ''' </summary>
        ''' <remarks>SG 16/03/11</remarks>
        Public Sub UpdateLevels()
            Try

                If DW_Top And Not DW_Bottom Then
                    DW_Level = BSMonitorTankLevels.TankLevels.TOP
                ElseIf Not DW_Top And Not DW_Bottom Then
                    DW_Level = BSMonitorTankLevels.TankLevels.MIDDLE
                ElseIf Not DW_Top And DW_Bottom Then
                    DW_Level = BSMonitorTankLevels.TankLevels.BOTTOM
                Else
                    DW_Level = BSMonitorTankLevels.TankLevels.UNDEFINED
                End If

                If LC_Top And Not LC_Bottom Then
                    LC_Level = BSMonitorTankLevels.TankLevels.TOP
                ElseIf Not LC_Top And Not LC_Bottom Then
                    LC_Level = BSMonitorTankLevels.TankLevels.MIDDLE
                ElseIf Not LC_Top And LC_Bottom Then
                    LC_Level = BSMonitorTankLevels.TankLevels.BOTTOM
                Else
                    LC_Level = BSMonitorTankLevels.TankLevels.UNDEFINED
                End If

            Catch ex As Exception
                Throw (ex)
            End Try
        End Sub

    End Structure
#End Region

#Region "Flags"

    Private ManageTabPages As Boolean = False
    Private TankTestRequested As Boolean = False
    Private ExitWhilePendingRequested As Boolean = False
    Private ExitWhileTestingRequested As Boolean = False

    'Private IsScalesTesting As Boolean = False
    Private IsScalesAdjusting As Boolean = False
    Private IsIntermediateTesting As Boolean = False

    Private IsPendingToHistoryScales As Boolean = False
    Private IsPendingToHistoryIntermediate As Boolean = False

#End Region

#Region "Properties"


    Public ReadOnly Property IsReadyToClose() As Boolean
        Get
            Return IsReadyToCloseAttr
        End Get
    End Property

    Private Property AllHomesAreDone() As Boolean
        Get
            If myScreenDelegate IsNot Nothing Then
                Dim myGlobal As New GlobalDataTO
                myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(ADJUSTMENT_GROUPS.INTERNAL_TANKS)
                If Not myGlobal.HasError And myGlobal IsNot Nothing Then
                    Dim myPendingList As New List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow)
                    myPendingList = CType(myGlobal.SetDatos, List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow))
                    AllHomesAreDoneAttr = (myPendingList.Count = 0)
                Else
                    AllHomesAreDoneAttr = False
                End If
            Else
                AllHomesAreDoneAttr = False
            End If

            Return AllHomesAreDoneAttr

        End Get
        Set(ByVal value As Boolean)
            AllHomesAreDoneAttr = value
        End Set
    End Property

    Private Property TestProcessStep() As TEST_PROCESS_STEPS
        Get
            Return TestProcessStepAttr
        End Get
        Set(ByVal value As TEST_PROCESS_STEPS)
            TestProcessStepAttr = value
            Select Case value
                Case TEST_PROCESS_STEPS.NONE

                Case TEST_PROCESS_STEPS.INITIATED

                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                    MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)

                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                    MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)

                Case TEST_PROCESS_STEPS.TRANSFERING
                    MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)

                Case TEST_PROCESS_STEPS.FINALIZING
                    MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)

                Case TEST_PROCESS_STEPS.FINISHED
                    MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_FINISHED.ToString)

                Case TEST_PROCESS_STEPS.UNDEFINED
                    'PENDING show message
                    MessageBox.Show("PENDING: TEST PROCESS UNDEFINED")

            End Select

            If CInt(value) >= 2 Then
                Me.BsCancelButton.Enabled = True
            End If
        End Set
    End Property

    Private Property SelectedSystemInput() As PW_INPUTS
        Get
            Return SelectedSystemInputAttr
        End Get

        Set(ByVal value As PW_INPUTS)

            SelectedSystemInputAttr = value

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Select Case value
                Case PW_INPUTS.FROM_SOURCE
                    Me.DWTankInputLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FROM_EXTERNAL_SOURCE", MyClass.LanguageId)

                Case PW_INPUTS.FROM_TANK
                    Me.DWTankInputLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FROM_EXTERNAL_TANK", MyClass.LanguageId)
            End Select

        End Set
    End Property

    Private Property IsAutoConditioningGLF() As Boolean
        Get
            Return IsAutoConditioningGLFAttr 'READ FROM POLLHW GLF
        End Get
        Set(ByVal value As Boolean)
            IsAutoConditioningGLFAttr = value
        End Set

    End Property
    Private IsAutoConditioningGLFAttr As Boolean = False

#End Region

#Region "Attributes"
    Private AllHomesAreDoneAttr As Boolean = False
    Private TestProcessStepAttr As TEST_PROCESS_STEPS = TEST_PROCESS_STEPS.NONE
    Private SelectedSystemInputAttr As PW_INPUTS = PW_INPUTS.FROM_SOURCE
    Private IsReadyToCloseAttr As Boolean
#End Region

#Region "Communication Event Handlers"
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
    ''' <remarks>Created by SG 17/11/10</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            ' XBC 05/05/2011 - timeout limit repetitions
            If pResponse = RESPONSE_TYPES.TIMEOUT Then

                ' Stop Progress bar & Timer
                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()
                MyClass.TestProcessTimer.Enabled = False

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


            'If MyBase.IsWaitingForResponse Then
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)
                        PrepareArea()
                    End If


                Case ADJUSTMENT_MODES.HOME_FINISHED
                    If pResponse = RESPONSE_TYPES.OK Then

                        myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(ADJUSTMENT_GROUPS.INTERNAL_TANKS)
                        If Not myGlobal.HasError Then
                            Me.AllHomesAreDone = True
                            MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_HOMES_FINISHED.ToString)

                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If



                Case ADJUSTMENT_MODES.SAVED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)
                        PrepareArea()

                        'MyBase.myServiceMDI.ReadAllFwAdjustments() 'needed???
                    End If



                    '************TANKS TEST*********************************************************************************
                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then

                        If myScreenDelegate.EmptyLcDoing And Not myScreenDelegate.EmptyLcDone Then
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = myScreenDelegate.CurrentTimeOperation
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)
                            PrepareTestEmptyLCRequestedMode()
                            ''simulate
                            'TestSimulatorTimer.Interval = 5000
                            'TestSimulatorTimer.Enabled = True

                        ElseIf myScreenDelegate.FillDwDoing And Not myScreenDelegate.FillDwDone Then
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = myScreenDelegate.CurrentTimeOperation
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                            PrepareTestFillDWRequestedMode()
                            ''simulate
                            'TestSimulatorTimer.Interval = 5000
                            'TestSimulatorTimer.Enabled = True

                        ElseIf myScreenDelegate.TransferDwLcDoing And Not myScreenDelegate.TransferDwLcDone Then
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = myScreenDelegate.CurrentTimeOperation
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                            PrepareTestTransferDWLCRequestedMode()
                            ''simulate
                            'TestSimulatorTimer.Interval = 5000
                            'TestSimulatorTimer.Enabled = True

                        End If

                    ElseIf pResponse = RESPONSE_TYPES.OK Then

                        ' Stop Progress bar & Timer
                        Me.ProgressBar1.Value = 0
                        Me.ProgressBar1.Visible = False
                        Me.ProgressBar1.Refresh()
                        MyClass.TestProcessTimer.Enabled = False

                        If myScreenDelegate.TransferDwLcDone And myScreenDelegate.FillDwDone And myScreenDelegate.EmptyLcDone Then
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_FINISHED.ToString)
                            PrepareTestFinishedMode()

                        ElseIf myScreenDelegate.FillDwDone And myScreenDelegate.EmptyLcDone Then
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                            PrepareTestFillDWEndedMode()

                        ElseIf myScreenDelegate.EmptyLcDone Then
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                            PrepareTestEmptyLCEndedMode()

                        End If

                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If


                    'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST_OK
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyClass.TIME_TEST_LC_EMPTYING = CInt(pData) * 100 'QUITAR Obtener tiempo estimado de la respuesta
                    '        MyClass.TestProcessTimer.Interval = TIME_TEST_LC_EMPTYING * 1000
                    '        MyClass.TestProcessTimer.Enabled = True
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)
                    '        PrepareArea()
                    '    End If
                    '    'MyBase.IsWaitingForResponse = True
                    '    'simulate
                    '    TestSimulatorTimer.Interval = 5000
                    '    TestSimulatorTimer.Enabled = True

                    'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_ENDED
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                    '        PrepareArea()
                    '    End If
                    '    'MyBase.IsWaitingForResponse = True

                    'Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST_OK
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyClass.TIME_TEST_DW_FILLING = CInt(pData) * 100 'QUITAR Obtener tiempo estimado de la respuesta
                    '        MyClass.TestProcessTimer.Interval = TIME_TEST_LC_EMPTYING * 1000
                    '        MyClass.TestProcessTimer.Enabled = True
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                    '        PrepareArea()
                    '    End If
                    '    'MyBase.IsWaitingForResponse = True
                    '    'simulate
                    '    TestSimulatorTimer.Interval = 5000
                    '    TestSimulatorTimer.Enabled = True

                    'Case ADJUSTMENT_MODES.TANKS_FILL_DW_ENDED
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                    '        PrepareArea()
                    '    End If
                    '    'MyBase.IsWaitingForResponse = True

                    'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST_OK
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyClass.TIME_TEST_TRANSFER = CInt(pData) * 100 'QUITAR Obtener tiempo estimado de la respuesta
                    '        MyClass.TestProcessTimer.Interval = TIME_TEST_LC_EMPTYING * 1000
                    '        MyClass.TestProcessTimer.Enabled = True
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                    '        PrepareArea()
                    '    End If
                    '    'MyBase.IsWaitingForResponse = True
                    '    'simulate
                    '    TestSimulatorTimer.Interval = 5000
                    '    TestSimulatorTimer.Enabled = True

                    'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_ENDED
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_FINISHED.ToString)
                    '        PrepareArea()
                    '    End If


                    '********************************************************************************

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.SRV_DATA_RECEIVED_ERROR.ToString)
                    Me.ErrorMode()


                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)
                        PrepareArea()
                    End If


            End Select
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function
#End Region

#Region "Private Methods"

#Region "TO DELETE"
    '#Region "Sensor Private Methods"

    '    ''' <summary>
    '    ''' Initialize the sensors assigned to the screen
    '    ''' </summary>
    '    ''' <returns></returns>
    '    ''' <remarks>SGM 01/03/11</remarks>
    '    Private Function InitializeSensors() As GlobalDataTO
    '        Dim myGlobal As New GlobalDataTO
    '        Try
    '            ''each screen has to define its Sensor List
    '            'Dim mySensorList As New List(Of SENSOR)
    '            'mySensorList.Add(SENSOR.WASHING_SOLUTION_LEVEL)
    '            'mySensorList.Add(SENSOR.HIGH_CONTAMINATION_LEVEL)
    '            'mySensorList.Add(SENSOR.DISTILLED_WATER_EMPTY)
    '            'mySensorList.Add(SENSOR.DISTILLED_WATER_FULL)
    '            'mySensorList.Add(SENSOR.LOW_CONTAMINATION_FULL)
    '            'mySensorList.Add(SENSOR.LOW_CONTAMINATION_EMPTY)

    '            'MyBase.ScreenSensorsDS.Clear()

    '            'myGlobal = MyBase.InitializeScreenSensors(mySensorList)


    '        Catch ex As Exception
    '            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '            myGlobal.ErrorMessage = ex.Message
    '            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " InitializeSensors ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '            mybase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '        End Try
    '        Return myGlobal
    '    End Function

    '    ''' <summary>
    '    ''' Initialize the sensors assigned to the monitor panel
    '    ''' </summary>
    '    ''' <returns></returns>
    '    ''' <remarks>SGM 22/03/11</remarks>
    '    Private Function InitializeMonitor() As GlobalDataTO
    '        Dim myGlobal As New GlobalDataTO
    '        Try
    '            ''each screen has to define its Sensor List
    '            'Dim pSensorList As New List(Of SENSOR)
    '            'pSensorList.Add(SENSOR.WASHING_SOLUTION_LEVEL)
    '            'pSensorList.Add(SENSOR.HIGH_CONTAMINATION_LEVEL)
    '            'pSensorList.Add(SENSOR.DISTILLED_WATER_EMPTY)
    '            'pSensorList.Add(SENSOR.DISTILLED_WATER_FULL)
    '            'pSensorList.Add(SENSOR.LOW_CONTAMINATION_FULL)
    '            'pSensorList.Add(SENSOR.LOW_CONTAMINATION_EMPTY)


    '            'MyBase.MonitorSensorsDS.Clear()

    '            'myGlobal = MyBase.InitializeMonitorSensors(pSensorList)

    '            'Me.BsMonitor.ColumnCount = 7
    '            'Me.BsMonitor.RowCount = 2

    '            ''fill the Monitor Panel
    '            'If Not myGlobal.HasError Then

    '            '    Me.BsMonitor.Clear()

    '            '    For Each S As SRVSensorsDS.srv_tfmwSensorsRow In MyBase.MonitorSensorsDS.srv_tfmwSensors.Rows
    '            '        Select Case S.SensorId.ToUpper.Trim
    '            '            Case SENSOR.WASHING_SOLUTION_LEVEL.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 0, 0, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORANALOGBAR, True, "Washing Solution", 0, 1024, "%")

    '            '            Case SENSOR.HIGH_CONTAMINATION_LEVEL.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 0, 1, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORANALOGBAR, True, "High Contamination", 0, 1024, "%")

    '            '            Case SENSOR.DISTILLED_WATER_EMPTY.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 1, 0, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORLED, True, "Distilled Water" & vbCrLf & "Empty")

    '            '            Case SENSOR.DISTILLED_WATER_FULL.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 1, 1, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORLED, True, "Distilled Water" & vbCrLf & "Full")

    '            '            Case SENSOR.LOW_CONTAMINATION_EMPTY.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 1, 2, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORLED, True, "Low Contamination" & vbCrLf & "Empty")

    '            '            Case SENSOR.LOW_CONTAMINATION_FULL.ToString
    '            '                myGlobal = Me.BsMonitor.AddSensor(S, 1, 3, BSMonitorPanel_Sensors1.MONITOR_CONTROLS.BSMONITORLED, True, "Low Contamination" & vbCrLf & "Full")

    '            '        End Select
    '            '        If myGlobal.HasError Then
    '            '            Return myGlobal
    '            '        End If
    '            '    Next
    '            'End If

    '            'Me.BsMonitor.Refresh()
    '            'Me.BsMonitor.Focus()

    '        Catch ex As Exception
    '            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '            myGlobal.ErrorMessage = ex.Message
    '            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " InitializeMonitor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '            mybase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '        End Try
    '        Return myGlobal
    '    End Function


    '    ''' <summary>
    '    ''' Displays the sensors data
    '    ''' </summary>
    '    ''' <remarks>SGM 11/03/11</remarks>
    '    Private Sub DisplaySensorsData()
    '        Try


    '        Catch ex As Exception

    '            MyBase.CurrentSensorsMode = SENSORS_MODES.OFF

    '            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisplaySensorsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '            mybase.ShowMessage(Me.Name & ".DisplaySensorsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '        End Try
    '    End Sub


    '#End Region
#End Region

#Region "Master Data Methods"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current Application Language</param>
    ''' <remarks>
    ''' Created by: XBC 31/01/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)

        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        Try

            'For Labels, CheckBox, RadioButtons.....
            Me.BsTabPagesControl.TabPages(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SCALES", pLanguageID)
            Me.BsTabPagesControl.TabPages(1).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_IntermediateTanks", pLanguageID)
            ' TAB 1
            Me.BsTanksInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.BsTanksAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SCALES_TITLE", pLanguageID)
            Me.WashingSolutionGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashSolution", pLanguageID)
            Me.HCWGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HIGH_WASTE", pLanguageID)
            Me.WSFullAdjustButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Full", pLanguageID)
            Me.HCFullAdjustButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Full", pLanguageID)
            Me.WSEmptyAdjustButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Empty", pLanguageID)
            Me.HCEmptyAdjustButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Empty", pLanguageID)
            Me.WSCountsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Counts", pLanguageID)
            Me.HCCountsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Counts", pLanguageID)
            'DL 23/04/2012
            Me.WSSavedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Saved", pLanguageID) & " (" & WSCountsLabel.Text & ")"
            Me.HCSavedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Saved", pLanguageID) & " (" & HCCountsLabel.Text & ")"
            'DL 23/04/2012
            ' TAB 2
            Me.BsIntermediateInfoTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.BsInternalTanksAdjustTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INT_TANKS_TITLE", pLanguageID)
            Me.DWInputGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DW_INPUT", pLanguageID)
            Me.DWGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DistilledWater", pLanguageID)
            Me.LCGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LowContaminationTank", pLanguageID)
            Me.IntermediateTanksTestGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PROCESS_STEP", pLanguageID)
            Me.BsTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST", pLanguageID)

            ' Tooltips
            MyClass.GetScreenTooltip(pLanguageID)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: XBC 26/07/11
    ''' </remarks>
    Private Sub GetScreenTooltip(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            ScreenTooltips.SetToolTip(Me.BsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            ScreenTooltips.SetToolTip(Me.BsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            ScreenTooltips.SetToolTip(Me.BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))

            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsStartTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", pLanguageID))
            MyBase.bsScreenToolTipsControl.SetToolTip(Me.BsStopTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 05/01/2011</remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try

            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the specified limits
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetControlsLimits ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            MyBase.ShowMessage(Me.Name & " GetControlsLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get Limits values from BD for every different arm
    ''' </summary>
    ''' <remarks>Created by XBC 05/01/2011</remarks>
    Private Function GetLimitValues() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            ' Get Value limit ranges
            Dim myFieldLimitsDS As New FieldLimitsDS

            'myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_WASH_SOLUTION_LIMIT)
            'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        Me.WSLimitMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
            '        Me.WSLimitMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
            '    End If
            'Else
            '    mybase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            '    Throw New Exception(myGlobalDataTO.ErrorMessage)
            'End If

            'myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_CONTAMINATED_LIMIT)
            'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            '    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
            '        Me.HCLimitMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
            '        Me.HCLimitMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
            '    End If
            'Else
            '    mybase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            '    Throw New Exception(myGlobalDataTO.ErrorMessage)
            'End If

            myGlobalDataTO = GetControlsLimits(FieldLimitsEnum.SRV_SCALES_PERCENT_LIMIT)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    Me.ScalesLimitMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    Me.ScalesLimitMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            Else
                MyBase.ShowMessage(Me.Name & ".GetLimitValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Throw New Exception(myGlobalDataTO.ErrorMessage)
            End If

            ' Another Initializations
            Me.SCALESDINAMICRANGE = GlobalBase.ScalesDinamicRange

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function
#End Region


#Region "Generic PREPARE Methods"

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>XBC 10/01/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim Utilities As New Utilities

        Try

            MyBase.SetButtonImage(BsAdjustButton, "ADJUSTMENT")
            MyBase.SetButtonImage(BsSaveButton, "SAVE")
            MyBase.SetButtonImage(BsCancelButton, "UNDO")
            MyBase.SetButtonImage(BsExitButton, "CANCEL")
            MyBase.SetButtonImage(WSFullAdjustButton, "ADJUSTMENT", 28, 28, ContentAlignment.MiddleLeft)
            MyBase.SetButtonImage(WSEmptyAdjustButton, "ADJUSTMENT", 28, 28, ContentAlignment.MiddleLeft)
            MyBase.SetButtonImage(HCFullAdjustButton, "ADJUSTMENT", 28, 28, ContentAlignment.MiddleLeft)
            MyBase.SetButtonImage(HCEmptyAdjustButton, "ADJUSTMENT", 28, 28, ContentAlignment.MiddleLeft)
            MyBase.SetButtonImage(BsStartTestButton, "ADJUSTMENT")
            MyBase.SetButtonImage(BsStopTestButton, "STOP", 24, 24)


            ''ADJUST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsAdjustButton.Image = myImage
            '    BsAdjustButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''SAVE Button
            'auxIconName = GetIconName("SAVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsSaveButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsSaveButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''CANCEL Button
            'auxIconName = GetIconName("UNDO") 'CANCEL
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsCancelButton.Image = myImage
            '    BsCancelButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            '    BsExitButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If


            ''Adjust Button Images
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then

            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)

            '    WSFullAdjustButton.Image = myImage
            '    WSFullAdjustButton.ImageAlign = ContentAlignment.MiddleLeft

            '    WSEmptyAdjustButton.Image = myImage
            '    WSEmptyAdjustButton.ImageAlign = ContentAlignment.MiddleLeft

            '    HCFullAdjustButton.Image = myImage
            '    HCFullAdjustButton.ImageAlign = ContentAlignment.MiddleLeft

            '    HCEmptyAdjustButton.Image = myImage
            '    HCEmptyAdjustButton.ImageAlign = ContentAlignment.MiddleLeft

            'End If

            'Check Images
            auxIconName = GetIconName("ACCEPTF")
            If System.IO.File.Exists(iconPath & auxIconName) Then

                WSFullSavedPictureBox.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                WSEmptySavedPictureBox.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                HCFullSavedPictureBox.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                HCEmptySavedPictureBox.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            End If

            'Info Button
            auxIconName = GetIconName("RIGHT")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Me.BsInfoExpandButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                Me.BsInfoExpandButton.BackgroundImageLayout = ImageLayout.Stretch
            End If

            ''START TEST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(28, 28)).SetDatos, Image)
            '    BsStartTestButton.Image = myImage
            '    BsAdjustButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

            ''STOP Button
            'auxIconName = GetIconName("STOP")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image = Image.FromFile(iconPath & auxIconName)
            '    myImage = CType(Utilities.ResizeImage(myImage, New Size(24, 24)).SetDatos, Image)
            '    BsStopTestButton.Image = myImage
            '    BsStopTestButton.ImageAlign = ContentAlignment.MiddleCenter
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareButtonsVisibility()
        Try
            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING, _
                ADJUSTMENT_MODES.ADJUSTMENTS_READED, _
                ADJUSTMENT_MODES.ADJUST_PREPARED
                    With MyBase.myScreenLayout.ButtonsPanel
                        If .AdjustButton IsNot Nothing Then .AdjustButton.Visible = False
                        If .SaveButton IsNot Nothing Then .SaveButton.Visible = True
                        If .ExitButton IsNot Nothing Then .ExitButton.Visible = True
                        If .CancelButton IsNot Nothing Then .CancelButton.Visible = True
                    End With

                Case Else
                    'MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Visible = False    ' Why buttons invisible ???
                    'MyBase.myScreenLayout.ButtonsPanel.SaveButton.Visible = False
                    'MyBase.myScreenLayout.ButtonsPanel.ExitButton.Visible = True
                    'MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = False

            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtonsVisiblility ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtonsVisiblility ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>XBC 10/01/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen

            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    MyBase.DisplayMessage(Messages.SRV_READ_ADJUSTMENTS.ToString)
                    If MyBase.SimulationMode Then
                        MyClass.PrepareSimulationAdjustmentsReadedMode()
                        MyClass.PrepareLoadedMode()
                    End If

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyClass.PrepareLoadedMode()

                    ' XBC 08/11/2011
                Case ADJUSTMENT_MODES.LOADED
                    PrepareLoadedMode()

                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    MyClass.PrepareAdjustPreparedMode()

                Case ADJUSTMENT_MODES.HOME_PREPARING
                    MyClass.PrepareHomesPreparingMode()

                Case ADJUSTMENT_MODES.HOME_FINISHED
                    MyClass.PrepareHomesFinishedMode()

                    'Case ADJUSTMENT_MODES.ADJUSTING
                    '    'PrepareAdjustingMode()

                    'Case ADJUSTMENT_MODES.ADJUSTED
                    '    'PrepareAdjustedMode()

                    'Case ADJUSTMENT_MODES.ADJUST_EXITING
                    '    'PrepareAdjustExitingMode()

                Case ADJUSTMENT_MODES.SAVING
                    MyClass.PrepareSavingMode()

                Case ADJUSTMENT_MODES.SAVED
                    MyClass.PrepareSavedMode()

                    '*********TEST*************************************************************************

                    'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST
                    '    PrepareTestEmptyLCMode()

                    'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST_OK
                    '    PrepareTestEmptyLCRequestedMode()

                    'Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_ENDED
                    '    PrepareTestEmptyLCEndedMode()

                    'Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST
                    '    PrepareTestFillDWMode()

                    'Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST_OK
                    '    PrepareTestFillDWRequestedMode()

                    'Case ADJUSTMENT_MODES.TANKS_FILL_DW_ENDED
                    '    PrepareTestFillDWEndedMode()

                    'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST
                    '    PrepareTestTransferDWLCMode()

                    'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST_OK
                    '    PrepareTestTransferDWLCRequestedMode()

                    'Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_ENDED
                    '    PrepareTestFinishedMode()

                    '********************************************************************************

                Case ADJUSTMENT_MODES.TEST_EXITING
                    MyClass.PrepareTestExitingMode()

                Case ADJUSTMENT_MODES.TEST_EXITED
                    MyClass.PrepareTestExitedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareErrorMode()
            End Select

            MyClass.PrepareButtonsVisibility()

            If MyBase.myServiceMDI IsNot Nothing Then
                If Not MyBase.SimulationMode And MyBase.myServiceMDI.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                    MyClass.PrepareErrorMode()
                    MyBase.DisplayMessage("")
                End If
            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Homes Preparing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 15/03/11</remarks>
    Private Sub PrepareHomesPreparingMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Enabled = False
            Me.BsStartTestButton.Visible = True
            Me.BsStopTestButton.Visible = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareHomesPreparingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareHomesPreparingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Homes Finished Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareHomesFinishedMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If MyBase.SimulationMode Then
                MyBase.ActivateMDIMenusButtons(True)
                myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(ADJUSTMENT_GROUPS.INTERNAL_TANKS)
                If Not myGlobal.HasError Then
                    Me.AllHomesAreDone = True
                    MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString, Messages.SRV_HOMES_FINISHED.ToString)
                Else
                    PrepareErrorMode()
                    Exit Sub
                End If
            End If

            If TankTestRequested Then
                myGlobal = MyClass.RequestTanksTest()
                If Not myGlobal.HasError Then

                Else
                    PrepareErrorMode()
                End If

                TankTestRequested = False

            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareHomesFinishedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareHomesFinishedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Simulation Adjustments readed mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/06/11</remarks>
    Private Sub PrepareSimulationAdjustmentsReadedMode()

        Dim myResultData As New GlobalDataTO

        Try

            MyClass.InitializeEditedValues()

            myResultData = MyClass.LoadAdjustmentGroupData()

            If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then

                MyClass.SelectedAdjustmentsDS = CType(myResultData.SetDatos, SRVAdjustmentsDS)

                myResultData = MyClass.ResetEditedNewValues()

                If Not myResultData.HasError Then

                    myResultData = MyClass.FillAdjustmentValues()

                    If Not myResultData.HasError Then

                        myResultData = MyClass.InitializeHomes()

                        If Not myResultData.HasError Then
                            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
                            ManageTabPages = True

                            'If Not MyBase.CurrentSensorsMode = SENSORS_MODES.OFF Then MyBase.SensorsTimer.Enabled = True

                            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                            MyClass.PrepareArea()
                        End If
                    End If
                End If
            ElseIf myResultData.HasError Then
                PrepareErrorMode()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSimulationAdjustmentsReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSimulationAdjustmentsReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ''' <summary>
    ''' Prepare GUI for Loaded Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareLoadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            MyClass.DefineScreenLayout()

            Me.WSEmptySavedPictureBox.Visible = False
            Me.WSFullSavedPictureBox.Visible = False
            Me.HCEmptySavedPictureBox.Visible = False
            Me.HCFullSavedPictureBox.Visible = False

            Select Case Me.SelectedPage

                Case ADJUSTMENT_PAGES.SCALES
                    With MyBase.myScreenLayout
                        If .ButtonsPanel.SaveButton IsNot Nothing Then .ButtonsPanel.SaveButton.Enabled = False
                        If .ButtonsPanel.CancelButton IsNot Nothing Then .ButtonsPanel.CancelButton.Enabled = False
                        If .ButtonsPanel.ExitButton IsNot Nothing Then .ButtonsPanel.ExitButton.Enabled = True
                    End With

                    'SGM 03/03/12
                    'initialize pre-saving buttons to disabled
                    Me.WSFullAdjustButton.Enabled = False
                    Me.WSEmptyAdjustButton.Enabled = False
                    Me.HCFullAdjustButton.Enabled = False
                    Me.HCEmptyAdjustButton.Enabled = False

                Case ADJUSTMENT_PAGES.INTERMEDIATE
                    With MyBase.myScreenLayout
                        If .ButtonsPanel.SaveButton IsNot Nothing Then .ButtonsPanel.SaveButton.Enabled = False
                        If .ButtonsPanel.CancelButton IsNot Nothing Then .ButtonsPanel.CancelButton.Enabled = False
                        If .ButtonsPanel.ExitButton IsNot Nothing Then .ButtonsPanel.ExitButton.Enabled = True
                    End With

            End Select

            MyClass.InitializeEditedValues()
            MyClass.InitializeTestProcessStatus()


            myResultData = MyClass.LoadAdjustmentGroupData()


            If Not myResultData.HasError Then

                myResultData = MyClass.ResetEditedNewValues()

                If Not myResultData.HasError Then

                    myResultData = MyClass.FillAdjustmentValues()

                    If Not myResultData.HasError Then

                        myResultData = MyClass.InitializeHomes()

                        If Not myResultData.HasError Then
                            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
                            ManageTabPages = True

                            'If Not MyBase.CurrentSensorsMode = SENSORS_MODES.OFF Then MyBase.SensorsTimer.Enabled = True

                            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                            MyClass.PrepareArea()
                        End If
                    End If
                End If
            End If

            MyBase.ActivateMDIMenusButtons(True)

            If myResultData.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Prepared Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareAdjustPreparedMode()
        Try

            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.SCALES
                    MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

                    MyClass.IsScalesAdjusting = False
                    MyClass.IsIntermediateTesting = False

                Case ADJUSTMENT_PAGES.INTERMEDIATE
                    MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)

                    MyClass.IsScalesAdjusting = False
                    MyClass.IsIntermediateTesting = True

            End Select

            With MyBase.myScreenLayout
                If .ButtonsPanel.SaveButton IsNot Nothing Then .ButtonsPanel.SaveButton.Enabled = False
                If .ButtonsPanel.CancelButton IsNot Nothing Then .ButtonsPanel.CancelButton.Enabled = False
                If .ButtonsPanel.ExitButton IsNot Nothing Then .ButtonsPanel.ExitButton.Enabled = True
            End With

            Me.ManageTabPages = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saving Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareSavingMode()
        Try
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
            MyBase.ActivateMDIMenusButtons(False)
            MyClass.DisableAll()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Saved Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Private Sub PrepareSavedMode()
        Dim myGlobal As New GlobalDataTO
        Try

            For Each E As EditedValueStruct In Me.EditedValues
                With E

                    .LastLevelValue = .NewLevelValue
                    myGlobal = UpdateSpecificAdjustmentsDS(ReadSpecificAdjustmentData(.AdjustmentID, .LevelID).CodeFw, .NewLevelValue.ToString)

                    If Not myGlobal.HasError Then
                        .NewLevelValue = 0

                        Me.WSCurrentLabel.ForeColor = Color.SteelBlue
                        Me.WSCountsLabel.ForeColor = Color.SteelBlue
                        Me.WSCurrentPercentLabel.ForeColor = Color.SteelBlue
                        Me.WSPercentLabel.ForeColor = Color.SteelBlue

                        Me.HCCurrentLabel.ForeColor = Color.SteelBlue
                        Me.HCCountsLabel.ForeColor = Color.SteelBlue
                        Me.HCCurrentPercentLabel.ForeColor = Color.SteelBlue
                        Me.HCPercentLabel.ForeColor = Color.SteelBlue

                        Me.WSEmptySavedPictureBox.Visible = False
                        Me.WSFullSavedPictureBox.Visible = False
                        Me.HCEmptySavedPictureBox.Visible = False
                        Me.HCFullSavedPictureBox.Visible = False
                    End If


                End With
            Next

            If Not myGlobal.HasError Then

                myGlobal = MyBase.UpdateAdjustments(MyClass.SelectedAdjustmentsDS)

                MyClass.ReportHistory(HISTORY_TASKS.ADJUSTMENT, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

                'If Not MyBase.CurrentSensorsMode = SENSORS_MODES.OFF Then MyBase.SensorsTimer.Enabled = True
                MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                PrepareArea()
                'FillAdjustmentValues()

                MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_SAVED.ToString)

                'update current tasks
                MyClass.IsScalesAdjusting = False
                MyClass.IsIntermediateTesting = False

                If ExitWhilePendingRequested Then
                    myGlobal = MyBase.CloseForm()
                    If myGlobal.HasError Then
                        PrepareErrorMode()
                    Else
                        PrepareArea()
                        MyClass.IsReadyToCloseAttr = True
                        Me.Close()

                        Application.DoEvents()

                        'SGM 22/05/2012
                        If MyBase.CloseRequestedByMDI Then
                            MyBase.myServiceMDI.isWaitingForCloseApp = True
                            If MyBase.CloseWithShutDownRequestedByMDI Then
                                MyBase.myServiceMDI.WithShutToolStripMenuItem.PerformClick()
                            ElseIf MyBase.CloseWithoutShutDownRequestedByMDI Then
                                MyBase.myServiceMDI.WithOutShutDownToolStripMenuItem.PerformClick()
                            Else
                                Me.Close()
                            End If
                        End If

                    End If
                End If

                MyBase.ActivateMDIMenusButtons(True)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareSavedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareSavedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub



    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestEmptyLCMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.INITIATED
            UpdateTestProcess()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestEmptyLCMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestEmptyLCMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestEmptyLCRequestedMode()
        Try
            ManageTabPages = False


            MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False


            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True


            MyClass.TestProcessStep = TEST_PROCESS_STEPS.INITIATED
            UpdateTestProcess()

            'If Not MyBase.CurrentSensorsMode = SENSORS_MODES.OFF Then MyBase.SensorsTimer.Enabled = True



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestEmptyLCRequestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestEmptyLCRequestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareTestEmptyLCEndedMode()
        Try
            ManageTabPages = False

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY
            UpdateTestProcess()

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            'MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

            'update current tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = True

            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True

            Dim myGlobal As New GlobalDataTO
            myGlobal = MyClass.RequestTanksTest()
            If myGlobal.HasError Then
                PrepareErrorMode()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestEmptyLCEndedMode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestEmptyLCEndedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestFillDWMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Enabled = False
            Me.BsStopTestButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestFillDWMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestFillDWMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestFillDWRequestedMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestFillDWRequestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestFillDWRequestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareTestFillDWEndedMode()
        Try
            ManageTabPages = False

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
            UpdateTestProcess()

            'MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK) 'History SGM 02/08/2011

            'update current tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = True

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True

            Dim myGlobal As New GlobalDataTO
            myGlobal = MyClass.RequestTanksTest()
            If myGlobal.HasError Then
                PrepareErrorMode()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestFillDWEndedMode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestFillDWEndedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestTransferDWLCMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Enabled = False
            Me.BsStopTestButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestTransferDWLCMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestEmptyLCMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestTransferDWLCRequestedMode()
        Try
            ManageTabPages = False

            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False

            Me.BsStartTestButton.Visible = False
            Me.BsStopTestButton.Visible = True

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
            UpdateTestProcess()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestTransferDWLCRequestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestTransferDWLCRequestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestFinishedMode()
        Try
            ManageTabPages = True

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True

            Me.BsStartTestButton.Visible = True
            Me.BsStartTestButton.Enabled = True
            Me.BsStopTestButton.Visible = False

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.FINISHED
            UpdateTestProcess()

            MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK)

            'update current tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = True

            MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
            PrepareArea()
            MyClass.TestProcessStep = TEST_PROCESS_STEPS.NONE

            MyBase.ActivateMDIMenusButtons(True)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestFinishedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestFinishedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exiting Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestExitingMode()
        Try
            ManageTabPages = False

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True

            Me.BsStartTestButton.Enabled = False
            Me.BsStopTestButton.Enabled = False

            'TestSimulatorTimer.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestExitingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exited Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/2011</remarks>
    Private Sub PrepareTestExitedMode()
        Try
            MyClass.ManageTabPages = True

            MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True

            Me.BsStartTestButton.Visible = True
            Me.BsStartTestButton.Enabled = True
            Me.BsStopTestButton.Visible = False

            MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)

            MyClass.TestProcessStep = TEST_PROCESS_STEPS.NONE
            MyClass.UpdateTestProcess()
            MyClass.InitializeTestGrid()



            'update current tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = False

            MyBase.ActivateMDIMenusButtons(True)

            If MyClass.ExitWhileTestingRequested Then

                Dim myGlobal As New GlobalDataTO
                myGlobal = MyBase.CloseForm()
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()
                    MyClass.IsReadyToCloseAttr = True
                    Me.Close()

                End If

            End If

            'If Not MyBase.CurrentSensorsMode = SENSORS_MODES.OFF Then MyBase.SensorsTimer.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestExitedMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#Region "History Methods"


    ''' <summary>
    ''' Updates the screen delegate's properties used for History management
    ''' </summary>
    ''' <param name="pTask"></param>
    ''' <param name="pResult"></param>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistory(ByVal pTask As HISTORY_TASKS, ByVal pResult As TankLevelsAdjustmentDelegate.HISTORY_RESULTS)
        Try

            MyClass.ClearHistoryData()


            'PDT
            'If MyBase.SimulationMode Then Exit Sub 'PONER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            Select Case MyClass.SelectedPage

                Case ADJUSTMENT_PAGES.SCALES

                    If pTask = HISTORY_TASKS.ADJUSTMENT And MyClass.IsPendingToHistoryScales Then
                        For Each E As EditedValueStruct In Me.EditedValues

                            With MyClass.myScreenDelegate

                                'define history parameters:
                                .HistoryArea = TankLevelsAdjustmentDelegate.HISTORY_AREAS.SCALES
                                .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES

                                'get the result and value to report
                                Dim myValue As Integer = -1
                                Dim myResult As TankLevelsAdjustmentDelegate.HISTORY_RESULTS

                                If E.NewLevelValue <> E.LastLevelValue Then 'if locally modified
                                    If pResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK Then 'if saving OK
                                        myValue = CInt(E.NewLevelValue)
                                        myResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    Else 'if not saving OK, Cancel, etc
                                        myValue = CInt(E.LastLevelValue)
                                        myResult = pResult
                                    End If
                                Else 'if not locally modified set as adjustment not done
                                    myValue = CInt(E.LastLevelValue)
                                    myResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                                End If


                                Select Case E.AdjustmentID
                                    Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
                                        Select Case E.LevelID
                                            Case AXIS.EMPTY
                                                .HisWsMinAdjValue = myValue
                                                .WsMinAdjResult = myResult


                                            Case AXIS.FULL
                                                .HisWsMaxAdjValue = myValue
                                                .WsMaxAdjResult = myResult

                                        End Select


                                    Case ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                                        Select Case E.LevelID
                                            Case AXIS.EMPTY
                                                .HisHcMinAdjValue = myValue
                                                .HcMinAdjResult = myResult

                                            Case AXIS.FULL
                                                .HisHcMaxAdjValue = myValue
                                                .HcMaxAdjResult = myResult


                                        End Select
                                End Select

                                'End If

                            End With
                        Next

                        MyClass.IsPendingToHistoryScales = False

                    ElseIf pTask = HISTORY_TASKS.TEST Then
                        'not applied

                    End If



                Case ADJUSTMENT_PAGES.INTERMEDIATE
                    If pTask = HISTORY_TASKS.TEST And MyClass.IsPendingToHistoryIntermediate Then

                        With MyClass.myScreenDelegate

                            .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                            .HistoryArea = TankLevelsAdjustmentDelegate.HISTORY_AREAS.INTERMEDIATE

                            Select Case MyClass.TestProcessStep


                                Case TEST_PROCESS_STEPS.INITIATED
                                    .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                                    .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                                    .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                                    .EmptyTestResult = pResult
                                    .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                                    .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY
                                    .EmptyTestResult = pResult
                                    .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                                    .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                                    .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .FillDwTestResult = pResult
                                    .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
                                    .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .FillDwTestResult = pResult
                                    .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                                Case TEST_PROCESS_STEPS.TRANSFERING
                                    .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .TransferDwLcTestResult = pResult

                                Case TEST_PROCESS_STEPS.FINISHED
                                    .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.OK
                                    .TransferDwLcTestResult = pResult

                            End Select

                        End With

                        MyClass.IsPendingToHistoryIntermediate = False

                    End If

            End Select


            MyClass.myScreenDelegate.ManageHistoryResults()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' Report Task not performed successfully to History
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistoryError()
        Try

            If MyClass.IsScalesAdjusting Then
                MyClass.ReportHistory(HISTORY_TASKS.ADJUSTMENT, TankLevelsAdjustmentDelegate.HISTORY_RESULTS._ERROR)
            ElseIf MyClass.IsIntermediateTesting Then
                MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS._ERROR)
            Else

            End If

            'reset tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistoryError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

                'results
                .WsMinAdjResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                .WsMaxAdjResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                .WsMinTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                .WsMaxTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                .EmptyTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                .FillDwTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE
                .TransferDwLcTestResult = TankLevelsAdjustmentDelegate.HISTORY_RESULTS.NOT_DONE

                'values
                .HisHcMinAdjValue = -1
                .HisHcMaxAdjValue = -1
                .HisWsMinAdjValue = -1
                .HisWsMaxAdjValue = -1


            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "NOT USED"

    '''' <summary>
    '''' Prepare GUI for Loading Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareLoadingMode()
    '    Dim myResultData As New GlobalDataTO
    '    Try


    '        DefineScreenLayout()


    '        ' XBC 28-01-2011 
    '        'DisableAll() 'SGM 24/01/11

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareLoadingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub



    '''' <summary>
    '''' Prepare GUI for Adjust Preparing Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareAdjustPreparingMode()
    '    Try

    '        Select Case Me.SelectedPage
    '            Case ADJUSTMENT_PAGES.WASHING_SOLUTION

    '            Case ADJUSTMENT_PAGES.CONTAMINATED_WASTE

    '            Case ADJUSTMENT_PAGES.INTERMEDIATE

    '        End Select

    '        DisableButtons() 'SGM 24/01/11

    '        MyBase.myScreenLayout.ButtonsPanel.ExitButton.Focus()

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareAdjustPreparingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub






    '''' <summary>
    '''' Prepare GUI for Adjusting Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareAdjustingMode()
    '    Try
    '        Select Case Me.SelectedPage
    '            Case ADJUSTMENT_PAGES.WASHING_SOLUTION
    '                Me.BsWashingTestButton.Enabled = False
    '            Case ADJUSTMENT_PAGES.CONTAMINATED_WASTE
    '                Me.BsWashingTestButton.Enabled = False

    '            Case ADJUSTMENT_PAGES.INTERMEDIATE
    '        End Select

    '        DisableButtons()

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareAdjustingMode", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Prepare GUI for Adjusted Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareAdjustedMode()

    '    Try
    '        Me.Enabled = True

    '        Select Case Me.SelectedPage
    '            Case ADJUSTMENT_PAGES.WASHING_SOLUTION
    '                Me.EditedValue.CurrentLevelValue = Me.EditedValue.NewLevelValue
    '                'Me.BsAdjustOptic.CurrentValue = Me.EditedValue.CurrentLevelValue

    '            Case ADJUSTMENT_PAGES.CONTAMINATED_WASTE
    '                Me.EditedValue.CurrentLevelValue = Me.EditedValue.NewLevelValue
    '                'Me.BsAdjustOptic.CurrentValue = Me.EditedValue.CurrentLevelValue

    '            Case ADJUSTMENT_PAGES.DISTILLED_WATER
    '                Me.EditedValue.CurrentLevelValue = Me.EditedValue.NewLevelValue
    '                'Me.BsAdjustOptic.CurrentValue = Me.EditedValue.CurrentLevelValue

    '            Case ADJUSTMENT_PAGES.INTERMEDIATE
    '                Me.EditedValue.CurrentLevelValue = Me.EditedValue.NewLevelValue
    '                'Me.BsAdjustOptic.CurrentValue = Me.EditedValue.CurrentLevelValue


    '        End Select

    '        'Me.ChangedValue = True
    '        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
    '        MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = True

    '        ManageTabPages = False

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareAdjustedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Prepare GUI for AdjustExiting Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareAdjustExitingMode()
    '    Try
    '        With Me.EditedValue
    '            .CurrentLevelValue = .LastLevelValue
    '            .NewLevelValue = 0
    '        End With

    '        DisableAll()

    '        'Me.ChangedValue = False


    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareAdjustExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub



    '''' <summary>
    '''' Prepare GUI for Tested Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareTestedMode()
    '    Try
    '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
    '        'Select Case SelectedArmTab
    '        '    Case ADJUSTMENT_ARMS.SAMPLE
    '        '        Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.REAGENT1
    '        '        Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.REAGENT2
    '        '        Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.MIXER1
    '        '        Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.MIXER2
    '        '        Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        'End Select
    '        'ActivateCheckButton(Me.SelectedRow, True)

    '        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Prepare GUI for Exiting Test Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareTestExitingMode()
    '    Try
    '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
    '        'ActivateCheckButton(Me.SelectedRow, False)
    '        'Select Case SelectedArmTab
    '        '    Case ADJUSTMENT_ARMS.SAMPLE
    '        '        Me.BsGridSample.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.REAGENT1
    '        '        Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.REAGENT2
    '        '        Me.BsGridReagent2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.MIXER1
    '        '        Me.BsGridMixer1.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        '    Case ADJUSTMENT_ARMS.MIXER2
    '        '        Me.BsGridMixer2.NameColumnButton2ByRow(Me.SelectedRow) = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SRV_EXIT_TEST", currentLanguage)
    '        'End Select

    '        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

    '        ManageTabPages = False

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Prepare GUI for Exited Test Mode
    '''' </summary>
    '''' <remarks>Created by XBC 14/01/2011</remarks>
    'Private Sub PrepareTestExitedMode()
    '    Try
    '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate


    '        MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = True
    '        MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
    '        MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = True
    '        MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False

    '        ManageTabPages = True

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

#End Region

#End Region


#Region "Generic Adjusments DS Methods"
    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>SG 31/01/11</remarks>
    Private Function UpdateSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.Value = pValue
                    Exit For
                End If
            Next

            MyClass.SelectedAdjustmentsDS.AcceptChanges()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        MyClass.SelectedAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' Update some value in the specific adjustments ds
    ''' </summary>
    ''' <param name="pCodew"></param>
    ''' <param name="pValue"></param>
    ''' <returns></returns>
    ''' <remarks>XBC 01/02/11</remarks>
    Private Function UpdateTemporalSpecificAdjustmentsDS(ByVal pCodew As String, ByVal pValue As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.TemporalAdjustmentsDS.srv_tfmwAdjustments.Rows
                If SR.CodeFw.Trim = pCodew.Trim Then
                    SR.Value = pValue
                    Exit For
                End If
            Next
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalSpecificAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalSpecificAdjustmentsDS", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Me.TemporalAdjustmentsDS.AcceptChanges()
        Return myGlobal
    End Function

    ''' <summary>
    ''' updates all a temporal dataset with changed current adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>XBC 01/02/11</remarks>
    Private Function UpdateTemporalAdjustmentsDS() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            For Each E As EditedValueStruct In Me.EditedValues
                With E

                    UpdateTemporalSpecificAdjustmentsDS(ReadSpecificAdjustmentData(.AdjustmentID, .LevelID).CodeFw, .NewLevelValue.ToString)

                End With
            Next

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTemporalAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTemporalAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Group and Axis from global adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 28/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadGlobalAdjustmentData(ByVal pGroupID As ADJUSTMENT_GROUPS, ByVal pAxis As AXIS, Optional ByVal pNotForDisplaying As Boolean = False) As AdjustmentRowData
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New AdjustmentRowData("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "_NONE" Then myAxis = ""

            Dim myGroup As String = pGroupID.ToString
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
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadGlobalAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadGlobalAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        'PDT if is not defined set to 0?
        If pNotForDisplaying Then
            If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"
        End If

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the value corresponding to informed Axis from the selected adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 28/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadSpecificAdjustmentData(ByVal pTank As ADJUSTMENT_GROUPS, ByVal pAxis As AXIS) As AdjustmentRowData
        Dim myGlobal As New GlobalDataTO
        Dim myAdjustmentRowData As New AdjustmentRowData("")
        Try
            Dim myTank As String = pTank.ToString
            If myTank = "_NONE" Then myTank = ""

            Dim myAxis As String = pAxis.ToString
            If myAxis = "_NONE" Then myAxis = ""

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.GroupID.Trim = myTank.Trim _
                                And a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.GroupID.Trim.ToUpper = myTank.Trim.ToUpper _
            'And a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myAdjustmentRowData
                    .AnalyzerID = myAdjustmentRows(0).AnalyzerID
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanSave = myAdjustmentRows(0).CanSave
                    '.CanMove = myAdjustmentRows(0).CanMove
                    .InFile = myAdjustmentRows(0).InFile
                End With
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadSpecificAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadSpecificAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try


        'PDT if is not defined set to 0?
        If myAdjustmentRowData.Value = "" Then myAdjustmentRowData.Value = "0"

        Return myAdjustmentRowData
    End Function

    ''' <summary>
    ''' Gets the Adjustments Dataset that corresponds to the editing adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function LoadAdjustmentGroupData() As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedWSAdjustmentsDS As SRVAdjustmentsDS = Me.WSSelectedAdjustmentsDS
        Dim CopyOfSelectedHCAdjustmentsDS As SRVAdjustmentsDS = Me.HCSelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try

            Me.WSSelectedAdjustmentsDS.Clear()
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.WASHING_SOLUTION.ToString)
            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Me.WSSelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

            Me.WSSelectedAdjustmentsDS.AcceptChanges()

        Catch ex As Exception
            Me.WSSelectedAdjustmentsDS = CopyOfSelectedWSAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Try
            myAdjustmentsGroups.Clear()
            Me.HCSelectedAdjustmentsDS.Clear()
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION.ToString)
            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Me.HCSelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
            End If

            Me.HCSelectedAdjustmentsDS.AcceptChanges()

        Catch ex As Exception
            Me.HCSelectedAdjustmentsDS = CopyOfSelectedHCAdjustmentsDS
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadAdjustmentGroupData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadAdjustmentGroupData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If Not resultData.HasError Then
            resultData = MergeSelectedAdjustmentsDS()
        End If

        Return resultData
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 05/03/11</remarks>
    Private Function MergeSelectedAdjustmentsDS() As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS

        Try

            MyClass.SelectedAdjustmentsDS.Clear()

            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.WSSelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
                With myNewRow
                    .AnalyzerID = R.AnalyzerID
                    .AreaFw = R.AreaFw
                    .AxisID = R.AxisID
                    .CanMove = R.CanMove
                    .CanSave = R.CanSave
                    .CodeFw = R.CodeFw
                    .DescriptionFw = R.DescriptionFw
                    .GroupID = R.GroupID
                    .InFile = R.InFile
                    .Value = R.Value
                    .FwVersion = R.FwVersion
                End With
                MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
            Next

            MyClass.SelectedAdjustmentsDS.AcceptChanges()


            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.HCSelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
                With myNewRow
                    .AnalyzerID = R.AnalyzerID
                    .AreaFw = R.AreaFw
                    .AxisID = R.AxisID
                    .CanMove = R.CanMove
                    .CanSave = R.CanSave
                    .CodeFw = R.CodeFw
                    .DescriptionFw = R.DescriptionFw
                    .GroupID = R.GroupID
                    .InFile = R.InFile
                    .Value = R.Value
                    .FwVersion = R.FwVersion
                End With
                MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
            Next

            MyClass.SelectedAdjustmentsDS.AcceptChanges()

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MergeSelectedAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MergeSelectedAdjustmentsDS ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return resultData
    End Function
#End Region


#Region "Initialize Methods"
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

    ''' <summary>
    ''' Initialize monitor tanks
    ''' </summary>
    ''' <remarks>SGM 14/03/11</remarks>
    Private Sub InitializeMonitorTanks()
        Try
            'SCALES
            'washing solution
            With WSMonitorTank
                .MinLimit = 0
                .MaxLimit = 100
                .ScaleStep = CInt((.MaxLimit - .MinLimit) / 2)
                .ScaleDivisions = 2
                .ScaleSubDivisions = 5
                .UpperLevelVisible = False
                .LowerLevelVisible = False
                .LevelValue = 0
            End With

            'high contamination
            With HCMonitorTank
                .MinLimit = 0
                .MaxLimit = 100
                .ScaleStep = CInt((.MaxLimit - .MinLimit) / 2)
                .ScaleDivisions = 2
                .ScaleSubDivisions = 5
                .UpperLevelVisible = False
                .LowerLevelVisible = False
                .LevelValue = 0
            End With

            'Internal Tanks
            'distilled water
            With DWMonitorTank
                .MinLimit = 0
                .MaxLimit = 100
                .TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM
            End With

            'low contamination
            With DWMonitorTank
                .MinLimit = 0
                .MaxLimit = 100
                .TankLevel = BSMonitorTankLevels.TankLevels.TOP
            End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeMonitorTanks ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeMonitorTanks ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize specific selected adjustment related structure
    ''' </summary>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Sub InitializeEditedValues()
        Try

            Dim myWS1 As New EditedValueStruct
            Dim myWS2 As New EditedValueStruct
            Dim myHC1 As New EditedValueStruct
            Dim myHC2 As New EditedValueStruct

            With myWS1
                .AdjustmentID = ADJUSTMENT_GROUPS.WASHING_SOLUTION
                '.TankID = TANKS.WASHING_SOLUTION
                .LevelID = AXIS.EMPTY
            End With
            With myWS2
                .AdjustmentID = ADJUSTMENT_GROUPS.WASHING_SOLUTION
                '.TankID = TANKS.WASHING_SOLUTION
                .LevelID = AXIS.FULL
            End With

            With myHC1
                .AdjustmentID = ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                '.TankID = TANKS.HIGH_CONTAMINATION
                .LevelID = AXIS.EMPTY
            End With
            With myHC2
                .AdjustmentID = ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                '.TankID = TANKS.HIGH_CONTAMINATION
                .LevelID = AXIS.FULL
            End With

            EditedValues.Add(myWS1)
            EditedValues.Add(myWS2)
            EditedValues.Add(myHC1)
            EditedValues.Add(myHC2)

            For Each E As EditedValueStruct In Me.EditedValues
                With E
                    .CurrentLevelValue = Nothing
                    .LastLevelValue = Nothing
                    .NewLevelValue = Nothing
                    .CanSaveLevelValue = False
                    .LevelID = AXIS._NONE
                End With
            Next


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeEditedValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeEditedValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub InitializeSystemInput()
        Try
            ' XBC 07/11/2011
            'Dim myInput As Integer = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.FLUIDIC_IN_OUT_SYSTEM, AXIS._NONE).Value)

            'Select Case myInput
            '    Case 0 : MyClass.SelectedSystemInput = PW_INPUTS.FROM_SOURCE
            '    Case 1 : MyClass.SelectedSystemInput = PW_INPUTS.FROM_TANK
            'End Select

            If IsNumeric(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.FLUIDIC_IN_OUT_SYSTEM, AXIS._NONE).Value) Then
                Dim myInput As Integer = CInt(ReadGlobalAdjustmentData(ADJUSTMENT_GROUPS.FLUIDIC_IN_OUT_SYSTEM, AXIS._NONE).Value)

                Select Case myInput
                    Case 0 : MyClass.SelectedSystemInput = PW_INPUTS.FROM_SOURCE
                    Case 1 : MyClass.SelectedSystemInput = PW_INPUTS.FROM_TANK
                End Select
            Else
                Me.PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeSystemInput ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeSystemInput ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>SGM 15/03/11</remarks>
    Private Sub InitializeTestProcessStatus()
        Try
            MyClass.TanksLevelsStatus = New LevelsStatus(True)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeTestProcessStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeTestProcessStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepares Intermediate Test Process Grid
    ''' </summary>
    ''' <remarks>SG 09/03/11</remarks>
    Private Sub InitializeTestGrid()

        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        Try
            Me.ProcessDataGridView.Visible = False
            Dim NG_IconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            Me.ProcessDataGridView.Rows.Clear()

            Me.ProcessDataGridView.Rows.Add(3)

            Dim myBigFont As Font = New Font(Me.ProcessDataGridView.Font.FontFamily, 12, FontStyle.Bold, Me.ProcessDataGridView.Font.Unit)

            NG_IconName = GetIconName("ACCEPTFBN")

            Dim myNewNGImage As Bitmap = Nothing

            If System.IO.File.Exists(iconPath & NG_IconName) Then

                Dim myNGImage As Image = ImageUtilities.ImageFromFile(iconPath & NG_IconName)


                'Dim Utilities As New Utilities
                Dim myGlobal As New GlobalDataTO

                myGlobal = Utilities.ResizeImage(myNGImage, New Size(20, 20))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myNewNGImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myNewNGImage = CType(myNGImage, Bitmap)
                End If

            Else
                myNewNGImage = Nothing
            End If


            For i As Integer = 1 To 3 Step 1
                Dim myNewRow As New DataGridViewRow

                myNewRow = Me.ProcessDataGridView.Rows(i - 1)
                myNewRow.Cells(0) = New DataGridViewDisableTextBoxCell
                myNewRow.Cells(0).Style.Font = myBigFont
                myNewRow.Cells(0).Value = (i).ToString

                myNewRow = Me.ProcessDataGridView.Rows(i - 1)
                myNewRow.Cells(1) = New DataGridViewImageCell
                myNewRow.Cells(1).Value = myNewNGImage

                Select Case i
                    Case 1
                        myNewRow.Cells(2).Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST_LC_EMPTY", MyClass.LanguageId)

                    Case 2
                        myNewRow.Cells(2).Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST_DW_FILL", MyClass.LanguageId)

                    Case 3
                        myNewRow.Cells(2).Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEST_TANKS_TRANSFER", MyClass.LanguageId)

                End Select
                myNewRow.Height = CInt(Me.ProcessDataGridView.Height / 3)
                myNewRow.Cells(2).Style.WrapMode = DataGridViewTriState.True
                myNewRow.Selected = False
            Next


            For Each R As DataGridViewRow In Me.ProcessDataGridView.Rows
                R.Selected = False
            Next

            For Each C As DataGridViewColumn In Me.ProcessDataGridView.Columns
                Select Case C.Index
                    Case 0 : C.Width = 20
                    Case 1 : C.Width = 30
                    Case 2 : C.Width = Me.ProcessDataGridView.Width - 20 - 30 - 2
                End Select
                C.Visible = True
            Next

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareIntermediateTestGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareIntermediateTestGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.ProcessDataGridView.Visible = True
        End Try
    End Sub
#End Region


#Region "Test Process Methods"
    ''' <summary>
    ''' Updates the process information
    ''' </summary>
    ''' <remarks>SG 15/03/11</remarks>
    Private Sub UpdateTestProcess()
        Try
            'MyClass.TestProcessTimer.Enabled = False

            'change step
            Select Case MyClass.TestProcessStepAttr
                Case TEST_PROCESS_STEPS.NONE
                    Me.TestProgressBar.Visible = False
                    Me.TestProgressBar.Value = 0

                Case TEST_PROCESS_STEPS.INITIATED
                    Me.TestProgressBar.Visible = True
                    Me.TestProgressBar.Value = 0

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.TOP

                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                    Me.TestProgressBar.Value = 50

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.MIDDLE

                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY
                    Me.TestProgressBar.Value = 100

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM

                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                    Me.TestProgressBar.Value = 150

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.MIDDLE
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM

                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
                    Me.TestProgressBar.Value = 200

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.TOP
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM

                Case TEST_PROCESS_STEPS.TRANSFERING
                    Me.TestProgressBar.Value = 250

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.MIDDLE
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.MIDDLE


                Case TEST_PROCESS_STEPS.FINISHED
                    Me.TestProgressBar.Visible = False
                    Me.TestProgressBar.Value = 0

                    Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.BOTTOM
                    Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.TOP

            End Select

            UpdateTestGrid()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateTestProcess", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateTestProcess", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)

        End Try
    End Sub

    Private Function CheckStepNotUndefined() As GlobalDataTO
        Dim myglobal As New GlobalDataTO
        Try
            Dim myResultStep As Boolean = False

            If TanksLevelsStatus.DW_Level = BSMonitorTankLevels.TankLevels.UNDEFINED Or _
            TanksLevelsStatus.LC_Level = BSMonitorTankLevels.TankLevels.UNDEFINED Then
                myResultStep = False
            Else

                Select Case MyClass.TestProcessStepAttr

                    Case TEST_PROCESS_STEPS.INITIATED
                        myResultStep = True

                    Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                        myResultStep = True


                    Case TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                        myResultStep = (TanksLevelsStatus.LC_Level <> BSMonitorTankLevels.TankLevels.TOP)


                    Case TEST_PROCESS_STEPS.TRANSFERING
                        myResultStep = True


                    Case TEST_PROCESS_STEPS.FINALIZING
                        myResultStep = ((TanksLevelsStatus.DW_Level <> BSMonitorTankLevels.TankLevels.TOP) And (TanksLevelsStatus.LC_Level <> BSMonitorTankLevels.TankLevels.BOTTOM))

                    Case TEST_PROCESS_STEPS.FINISHED
                        myResultStep = ((TanksLevelsStatus.DW_Level <> BSMonitorTankLevels.TankLevels.TOP) And (TanksLevelsStatus.LC_Level <> BSMonitorTankLevels.TankLevels.BOTTOM))

                End Select
            End If


            myglobal.SetDatos = myResultStep

            If Not myResultStep Then
                myResultStep = myResultStep
            End If

        Catch ex As Exception
            myglobal.HasError = True
            myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myglobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CheckStepNotUndefined", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CheckStepNotUndefined", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myglobal
    End Function

    ''' <summary>
    ''' Updates the process information
    ''' </summary>
    ''' <remarks>SG 15/03/11</remarks>
    Private Sub UpdateTestGrid()
        Try
            Dim RUN_IconName As String = ""
            Dim OK_IconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            RUN_IconName = GetIconName("GEAR")
            OK_IconName = GetIconName("ACCEPTF")

            Dim myNewRunImage As Bitmap = Nothing
            Dim myNewOKImage As Bitmap = Nothing

            If System.IO.File.Exists(iconPath & RUN_IconName) And System.IO.File.Exists(iconPath & OK_IconName) Then

                Dim myRunImage As Image = ImageUtilities.ImageFromFile(iconPath & RUN_IconName)
                Dim myOKImage As Image = ImageUtilities.ImageFromFile(iconPath & OK_IconName)


                'Dim Utilities As New Utilities
                Dim myGlobal As New GlobalDataTO

                myGlobal = Utilities.ResizeImage(myRunImage, New Size(20, 20))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myNewRunImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myNewRunImage = CType(myRunImage, Bitmap)
                End If

                myGlobal = Utilities.ResizeImage(myOKImage, New Size(20, 20))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myNewOKImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myNewOKImage = CType(myOKImage, Bitmap)
                End If

            Else

            End If

            Select Case MyClass.TestProcessStepAttr
                Case TEST_PROCESS_STEPS.NONE

                Case TEST_PROCESS_STEPS.INITIATED
                    Me.ProcessDataGridView.Rows(0).Cells(1) = New DataGridViewImageCell()
                    Me.ProcessDataGridView.Rows(0).Cells(1).Value = myNewRunImage
                    Me.ProcessDataGridView.Rows(0).Cells(2).Value = "Initiating.."

                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                    Me.ProcessDataGridView.Rows(0).Cells(1) = New DataGridViewImageCell()
                    Me.ProcessDataGridView.Rows(0).Cells(1).Value = myNewRunImage
                    Me.ProcessDataGridView.Rows(0).Cells(2).Value = "Empty Low Contamination tank"


                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY, TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                    Me.ProcessDataGridView.Rows(0).Cells(1).Value = myNewOKImage
                    Me.ProcessDataGridView.Rows(1).Cells(1) = New DataGridViewImageCell
                    Me.ProcessDataGridView.Rows(1).Cells(1).Value = myNewRunImage
                    Me.ProcessDataGridView.Rows(1).Cells(2).Value = "Fill Distilled Water tank"


                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FULL, TEST_PROCESS_STEPS.TRANSFERING, TEST_PROCESS_STEPS.FINALIZING
                    Me.ProcessDataGridView.Rows(1).Cells(1).Value = myNewOKImage
                    Me.ProcessDataGridView.Rows(2).Cells(1) = New DataGridViewImageCell
                    Me.ProcessDataGridView.Rows(2).Cells(1).Value = myNewRunImage
                    Me.ProcessDataGridView.Rows(2).Cells(2).Value = "Transfer from Distilled Water tank to Low Contamination tank"


                Case TEST_PROCESS_STEPS.FINISHED
                    Me.ProcessDataGridView.Rows(2).Cells(1).Value = myNewOKImage


            End Select


            For Each R As DataGridViewRow In Me.ProcessDataGridView.Rows
                R.Selected = False
            Next


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateIntermediateTestGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateIntermediateTestGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region


#Region "Edited Values Methods"
    ''' <summary>
    ''' resets specific selected adjustment related structure
    ''' </summary>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function ResetEditedNewValues() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myCopyEditedValues As List(Of EditedValueStruct) = Me.EditedValues

        Try
            Dim myNewEditedValues As New List(Of EditedValueStruct)
            For Each E As EditedValueStruct In Me.EditedValues
                With E
                    Dim myValue As String
                    myValue = ReadSpecificAdjustmentData(.AdjustmentID, .LevelID).Value.Replace(".", ",")
                    E.LastLevelValue = CSng(myValue)
                    E.NewLevelValue = CSng(myValue)
                    myNewEditedValues.Add(E)
                End With
            Next

            If myNewEditedValues.Count = Me.EditedValues.Count Then
                Me.EditedValues = myNewEditedValues
            Else
                myGlobal.HasError = True
                Me.EditedValues = myCopyEditedValues
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ResetEditedNewValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ResetEditedNewValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' get edited value
    ''' </summary>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function GetEditedNewValue(ByVal pTank As ADJUSTMENT_GROUPS, ByVal pLevel As AXIS) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            For Each E As EditedValueStruct In Me.EditedValues
                With E
                    If E.AdjustmentID = pTank And E.LevelID = pLevel Then
                        myGlobal.SetDatos = E.NewLevelValue
                        Exit For
                    End If
                End With

            Next

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetEditedNewValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetEditedNewValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' get current value
    ''' </summary>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function GetEditedCurrentValue(ByVal pTank As ADJUSTMENT_GROUPS, ByVal pLevel As AXIS) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            For Each E As EditedValueStruct In Me.EditedValues
                With E
                    If E.AdjustmentID = pTank And E.LevelID = pLevel Then
                        myGlobal.SetDatos = E.CurrentLevelValue
                        Exit For
                    End If
                End With

            Next

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetEditedCurrentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetEditedCurrentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' set current value
    ''' </summary>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function SetEditedCurrentValue(ByVal pTank As ADJUSTMENT_GROUPS, ByVal pLevel As AXIS, ByVal pValue As Single) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myNewEditedValues As New List(Of EditedValueStruct)
            For Each E As EditedValueStruct In Me.EditedValues
                With E
                    If E.AdjustmentID = pTank And E.LevelID = pLevel Then
                        E.CurrentLevelValue = pValue
                    End If
                    myNewEditedValues.Add(E)
                End With

            Next

            If myNewEditedValues.Count = Me.EditedValues.Count Then
                Me.EditedValues = myNewEditedValues
            Else
                myGlobal.HasError = True
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SetEditedCurrentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetEditedCurrentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function
#End Region


#Region "History Methods"



#End Region
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <param name="pAdjustmentGroup"></param>
    ''' <remarks>XBC 10/01/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES, _
                             Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing)
        Dim myGlobal As New GlobalDataTO
        Try
            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode)
                If Not myGlobal.HasError Then
                    Me.Cursor = Cursors.WaitCursor
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
            Else

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>SG 10/01/2011</remarks>
    Private Sub DefineScreenLayout()

        Try
            With MyBase.myScreenLayout

                .ButtonsPanel.SaveButton = Me.BsSaveButton
                .ButtonsPanel.CancelButton = Me.BsCancelButton
                .ButtonsPanel.ExitButton = Me.BsExitButton

                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                Select Case Me.SelectedPage
                    Case ADJUSTMENT_PAGES.SCALES
                        Me.BsSaveButton.Visible = True
                        Me.BsCancelButton.Visible = True
                        .ButtonsPanel.AdjustButton = Me.BsAdjustButton
                        .ButtonsPanel.SaveButton = Me.BsSaveButton
                        .ButtonsPanel.CancelButton = Me.BsCancelButton
                        .ButtonsPanel.TestButton = Nothing
                        .AdjustmentPanel.AdjustPanel.Container = Me.BsTanksAdjustPanel
                        .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsTanksAdjustPanel)


                    Case ADJUSTMENT_PAGES.INTERMEDIATE
                        Me.BsSaveButton.Visible = False
                        Me.BsCancelButton.Visible = False
                        .ButtonsPanel.AdjustButton = Nothing
                        .ButtonsPanel.CancelButton = Nothing
                        .ButtonsPanel.SaveButton = Nothing
                        .ButtonsPanel.TestButton = Nothing
                        .AdjustmentPanel.AdjustPanel.Container = Me.BsTanksAdjustPanel
                        .AdjustmentPanel.AdjustPanel.AdjustAreas = MyBase.GetAdjustAreas(Me.BsTanksAdjustPanel)

                End Select

                '.MonitorPanel.Monitor = Me.BsMonitor

            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the adjustments locally in the EditedValues list
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 11/03/11</remarks>
    Private Function SaveLocal(ByVal pTankId As ADJUSTMENT_GROUPS, ByVal pLevel As AXIS, ByVal pValue As Single) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myCopyEditedValues As List(Of EditedValueStruct) = Me.EditedValues

        Try
            Dim myNewEditedValues As New List(Of EditedValueStruct)
            For Each E As EditedValueStruct In Me.EditedValues
                If E.AdjustmentID = pTankId And E.LevelID = pLevel Then

                    'SGM 19/04/2012 not to validate because of specification
                    'myGlobal = ValidateLevel(E, pValue)
                    myGlobal.SetDatos = True 'NOT VALIDATE LIMITS
                    'end SGM 19/04/2012

                    If CBool(myGlobal.SetDatos) Then
                        E.NewLevelValue = pValue
                    Else
                        Exit For
                    End If
                End If
                myNewEditedValues.Add(E)
            Next

            If myNewEditedValues.Count = Me.EditedValues.Count Then
                Me.EditedValues = myNewEditedValues

                'check icons
                Select Case pTankId
                    Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
                        Select Case pLevel
                            Case AXIS.EMPTY
                                Me.WSEmptySavedPictureBox.Visible = True
                                Me.WSMonitorTank.LowerLevelValue = pValue

                            Case AXIS.FULL
                                Me.WSFullSavedPictureBox.Visible = True
                                Me.WSMonitorTank.UpperLevelValue = pValue

                        End Select

                    Case ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                        Select Case pLevel
                            Case AXIS.EMPTY
                                Me.HCEmptySavedPictureBox.Visible = True
                                Me.HCMonitorTank.LowerLevelValue = pValue

                            Case AXIS.FULL
                                Me.HCFullSavedPictureBox.Visible = True
                                Me.HCMonitorTank.UpperLevelValue = pValue

                        End Select
                End Select

                FillNewAdjustmentValue(pTankId, pLevel, pValue)

            Else
                myGlobal.HasError = True
                Me.EditedValues = myCopyEditedValues
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SaveLocal ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveLocal ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Validates that the levels are ok
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 11/03/11</remarks>
    Private Function ValidateLevel(ByVal pTank As ADJUSTMENT_GROUPS, ByVal pLevel As AXIS, ByVal pNewValue As Single) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            Dim MinMaxOK As Boolean = True
            Dim ResolutionOK As Boolean = True

            myGlobal.SetDatos = True

            'retrieve the pair of levels to validate
            Dim myOtherLevel As AXIS
            Select Case pLevel
                Case AXIS.EMPTY : myOtherLevel = AXIS.FULL
                Case AXIS.FULL : myOtherLevel = AXIS.EMPTY
            End Select

            For Each E As EditedValueStruct In Me.EditedValues
                If E.AdjustmentID = pTank And E.LevelID = myOtherLevel Then
                    Select Case pLevel
                        Case AXIS.EMPTY
                            If pNewValue >= E.NewLevelValue Then
                                MinMaxOK = False
                            ElseIf E.NewLevelValue - pNewValue < Me.SCALESDINAMICRANGE Then
                                ResolutionOK = False
                            End If

                        Case AXIS.FULL
                            If pNewValue <= E.NewLevelValue Then
                                MinMaxOK = False
                            ElseIf pNewValue - E.NewLevelValue < Me.SCALESDINAMICRANGE Then
                                ResolutionOK = False
                            End If

                    End Select

                    Exit For
                End If
            Next

            myGlobal.SetDatos = MinMaxOK And ResolutionOK

            If Not MinMaxOK Then
                DisplayMessage(Messages.SRV_MIN_GREATER_MAX.ToString)
            ElseIf Not ResolutionOK Then
                DisplayMessage(Messages.SRV_COUNTS_RANGE_WRONG.ToString)
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateLevel ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function


    ''' <summary>
    ''' validates levels before saving
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 02/10/2012</remarks>
    Private Function ValidateSavingLevels() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            Dim MinMaxOK As Boolean = True
            Dim ResolutionOK As Boolean = True

            myGlobal.SetDatos = True

            For Each E1 As EditedValueStruct In Me.EditedValues

                Dim myOtherLevel As AXIS
                Select Case E1.LevelID
                    Case AXIS.EMPTY : myOtherLevel = AXIS.FULL
                    Case AXIS.FULL : myOtherLevel = AXIS.EMPTY
                End Select

                For Each E2 As EditedValueStruct In Me.EditedValues
                    If E2.AdjustmentID = E1.AdjustmentID And E2.LevelID = myOtherLevel Then
                        Select Case E1.LevelID
                            Case AXIS.EMPTY
                                If E1.NewLevelValue >= E2.NewLevelValue Then
                                    MinMaxOK = False
                                ElseIf E2.NewLevelValue - E1.NewLevelValue < Me.SCALESDINAMICRANGE Then
                                    ResolutionOK = False
                                End If

                            Case AXIS.FULL
                                If E1.NewLevelValue <= E2.NewLevelValue Then
                                    MinMaxOK = False
                                ElseIf E1.NewLevelValue - E2.NewLevelValue < Me.SCALESDINAMICRANGE Then
                                    ResolutionOK = False
                                End If

                        End Select

                        If Not MinMaxOK OrElse Not ResolutionOK Then
                            Exit For
                        End If
                    End If
                Next

                If Not MinMaxOK OrElse Not ResolutionOK Then
                    Exit For
                End If
            Next


            myGlobal.SetDatos = MinMaxOK And ResolutionOK

            If Not MinMaxOK Then
                DisplayMessage(Messages.SRV_MIN_GREATER_MAX.ToString)
            ElseIf Not ResolutionOK Then
                DisplayMessage(Messages.SRV_COUNTS_RANGE_WRONG.ToString)
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ValidateLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ValidateLevel ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Saves changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>
    ''' Created by  SG 24/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub SaveAdjustment()
        Dim myGlobal As New GlobalDataTO
        Try

            myGlobal = ValidateSavingLevels()

            'update digit colors
            If myGlobal.HasError Then
                PrepareErrorMode()
                ManageTabPages = True
                Exit Sub

            ElseIf Not CBool(myGlobal.SetDatos) Then

                ManageTabPages = True
                Exit Sub
            Else

                Me.WSCurrentLabel.ForeColor = Color.LightSteelBlue
                Me.WSCountsLabel.ForeColor = Color.LightSteelBlue
                Me.WSCurrentPercentLabel.ForeColor = Color.LightSteelBlue
                Me.WSPercentLabel.ForeColor = Color.LightSteelBlue

                Me.HCCurrentLabel.ForeColor = Color.LightSteelBlue
                Me.HCCountsLabel.ForeColor = Color.LightSteelBlue
                Me.HCCurrentPercentLabel.ForeColor = Color.LightSteelBlue
                Me.HCPercentLabel.ForeColor = Color.LightSteelBlue

                'stop the monitor PDT
                'MyBase.SensorsTimer.Enabled = False

                myGlobal = MyBase.Save()
                PrepareArea()
            End If

            If Not myGlobal.HasError Then

                Dim myCodeFw As String

                'get from edited values
                For Each E As EditedValueStruct In Me.EditedValues
                    myCodeFw = ReadSpecificAdjustmentData(E.AdjustmentID, E.LevelID).CodeFw
                    If myCodeFw.Length > 0 Then
                        For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments.Rows
                            'If myCodeFw.ToUpper.Trim = R.CodeFw.ToUpper.Trim Then
                            If myCodeFw.Trim = R.CodeFw.Trim Then
                                R.Value = E.NewLevelValue.ToString("0")
                            End If
                        Next
                    End If
                    MyClass.SelectedAdjustmentsDS.AcceptChanges()
                Next

                ' Takes a copy of the changed values of the dataset of Adjustments
                myGlobal = myAdjustmentsDelegate.Clone(MyClass.SelectedAdjustmentsDS)
                If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                    Me.TemporalAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    Me.TempToSendAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.TemporalAdjustmentsDS)
                    ' Update dataset of the temporal dataset of Adjustments to sent to Fw
                    myGlobal = UpdateTemporalAdjustmentsDS()
                Else
                    MyClass.PrepareErrorMode()
                    Exit Sub
                End If


                If myGlobal.HasError Then
                    MyClass.PrepareErrorMode()
                Else
                    MyBase.DisplayMessage(Messages.SRV_SAVE_ADJUSTMENTS.ToString)

                    ' Convert dataset to String for sending to Fw
                    myGlobal = Me.TempToSendAdjustmentsDelegate.ConvertDSToString()

                    If Not myGlobal.SetDatos Is Nothing AndAlso Not myGlobal.HasError Then
                        Dim pAdjuststr As String = CType(myGlobal.SetDatos, String)

                        MyClass.myScreenDelegate.pValueAdjust = pAdjuststr

                        If MyBase.SimulationMode Then
                            ' simulating
                            'MyBase.DisplaySimulationMessage("Saving Values to Instrument...")
                            Me.Cursor = Cursors.WaitCursor
                            System.Threading.Thread.Sleep(SimulationProcessTime)
                            MyBase.myServiceMDI.Focus()
                            Me.Cursor = Cursors.Default
                            MyBase.CurrentMode = ADJUSTMENT_MODES.SAVED
                            PrepareArea()
                        Else
                            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                                Me.Cursor = Cursors.WaitCursor
                                myGlobal = myScreenDelegate.SendLoad_Adjustments(ADJUSTMENT_GROUPS.IT_EDITION)
                            End If

                            If Not myGlobal.HasError Then
                                With MyBase.myScreenLayout
                                    If .ButtonsPanel.SaveButton IsNot Nothing Then .ButtonsPanel.SaveButton.Enabled = False
                                    If .ButtonsPanel.CancelButton IsNot Nothing Then .ButtonsPanel.CancelButton.Enabled = False
                                End With

                                Me.WSFullAdjustButton.Enabled = False
                                Me.WSEmptyAdjustButton.Enabled = False
                                Me.HCFullAdjustButton.Enabled = False
                                Me.HCEmptyAdjustButton.Enabled = False
                            Else
                                MyClass.PrepareErrorMode()
                            End If

                        End If
                    Else
                        MyClass.PrepareErrorMode()
                    End If

                End If

            Else
                ManageTabPages = True
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SaveAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SaveAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancels changed values for the selected adjustment
    ''' </summary>
    ''' <remarks>SG 24/01/11</remarks>
    Private Sub CancelAdjustment()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.ExitAdjust()
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Cancelling changes...")
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                        PrepareArea()
                    Else
                        ' Manage FwScripts must to be sent to Adjust Exiting
                        'SendFwScript(Me.CurrentMode, EditedValue.AdjustmentID)
                    End If
                End If

                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()

                ManageTabPages = True
            End If


            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_CANCELLED.ToString)


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CancelAdjustment ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelAdjustment ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Request Internal Tanks Test
    ''' </summary>
    ''' <remarks>SGM 15/03/11</remarks>
    Private Function RequestTanksTest() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.ActivateMDIMenusButtons(False)

            Me.BsCancelButton.Enabled = True

            'update current tasks
            MyClass.IsScalesAdjusting = False
            MyClass.IsIntermediateTesting = True

            MyClass.TankTestRequested = True

            If Not Me.AllHomesAreDone Then

                MyBase.CurrentMode = ADJUSTMENT_MODES.HOME_PREPARING
                PrepareArea()

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    ' simulating
                    'MyBase.DisplaySimulationMessage("Home for all Arms")
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    'LoadAdjustmentGroupData()
                    MyBase.CurrentMode = ADJUSTMENT_MODES.HOME_FINISHED
                    PrepareArea()
                Else
                    SendFwScript(Me.CurrentMode)
                End If
            Else
                ' Continue with the IT Editing until finish it
                'myGlobal = MyBase.StartTestStep(Me.CurrentMode)
                myGlobal = MyBase.Test()

                If Not myGlobal.HasError Then

                    If MyBase.SimulationMode Then
                        ' simulating

                        'Select Case Me.CurrentMode
                        '    Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST
                        '        MyBase.DisplaySimulationMessage("Low Contamination Tank Empty")
                        '        MyBase.CurrentMode = ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST_OK

                        '    Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST
                        '        MyBase.DisplaySimulationMessage("Distilled Water Tank Fill")
                        '        MyBase.CurrentMode = ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST_OK

                        '    Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST
                        '        MyBase.DisplaySimulationMessage("Transfer from Distilled Water Tank to Low Contamination Tank")
                        '        MyBase.CurrentMode = ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST_OK
                        'End Select

                        If Not myScreenDelegate.EmptyLcDone Then
                            myScreenDelegate.EmptyLcDoing = True
                            myScreenDelegate.EmptyLcDone = True
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = 10 ' 10 seconds
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            MyClass.TestSimulatorTimer.Interval = Me.ProgressBar1.Maximum * 1000
                            MyClass.TestSimulatorTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_LC_EMPTYING.ToString)
                            PrepareTestEmptyLCRequestedMode()

                        ElseIf Not myScreenDelegate.FillDwDone Then
                            myScreenDelegate.FillDwDoing = True
                            myScreenDelegate.FillDwDone = True
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = 10
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            MyClass.TestSimulatorTimer.Interval = Me.ProgressBar1.Maximum * 1000
                            MyClass.TestSimulatorTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                            PrepareTestFillDWRequestedMode()

                        ElseIf Not myScreenDelegate.TransferDwLcDone Then
                            myScreenDelegate.TransferDwLcDoing = True
                            myScreenDelegate.TransferDwLcDone = True
                            ' Configuring Progress bar & timer associated
                            Me.ProgressBar1.Maximum = 10
                            Me.ProgressBar1.Value = 0
                            Me.ProgressBar1.Visible = True
                            MyClass.TestProcessTimer.Interval = 1000 ' 1 second
                            MyClass.TestProcessTimer.Enabled = True
                            MyClass.TestSimulatorTimer.Interval = Me.ProgressBar1.Maximum * 1000
                            MyClass.TestSimulatorTimer.Enabled = True
                            ' Refresh values
                            MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                            PrepareTestTransferDWLCRequestedMode()

                        End If

                    Else
                        If Not myScreenDelegate.EmptyLcDone Then
                            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                                myScreenDelegate.pValueAdjust = "1" ' PDT to specified !!!
                                myGlobal = myScreenDelegate.SendTanksTest_Adjustments(ADJUSTMENT_GROUPS.TANKS_EMPTY_LC)
                                PrepareTestEmptyLCMode()
                            End If
                        ElseIf Not myScreenDelegate.FillDwDone Then
                            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                                myScreenDelegate.pValueAdjust = "2"
                                myGlobal = myScreenDelegate.SendTanksTest_Adjustments(ADJUSTMENT_GROUPS.TANKS_FILL_DW)
                                PrepareTestFillDWMode()
                            End If
                        ElseIf Not myScreenDelegate.TransferDwLcDone Then
                            If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                                myScreenDelegate.pValueAdjust = "3"
                                myGlobal = myScreenDelegate.SendTanksTest_Adjustments(ADJUSTMENT_GROUPS.TANKS_TRANSFER_DW_LC)
                                PrepareTestTransferDWLCMode()
                            End If
                        End If
                    End If
                End If

                'PrepareArea()

            End If

        Catch ex As Exception
            TankTestRequested = False
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestTanksTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestTanksTest ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Cancels Internal Tanks Test
    ''' </summary>
    ''' <remarks>SG 15/03/11</remarks>
    Private Sub CancelTanksTest()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.ExitTest
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                If Not myGlobal.HasError Then
                    ' Stop Progress bar & Timer
                    Me.ProgressBar1.Value = 0
                    Me.ProgressBar1.Visible = False
                    Me.ProgressBar1.Refresh()
                    MyClass.TestProcessTimer.Enabled = False
                    Me.TestSimulatorTimer.Enabled = False

                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Canceling test...")
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        'MyBase.DisplaySimulationMessage("")
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                        PrepareArea()
                    Else
                        If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                            myScreenDelegate.pValueAdjust = "4" ' PDT to specified !!!
                            myGlobal = myScreenDelegate.SendTanksTest_Adjustments(ADJUSTMENT_GROUPS.TANKS_EMPTY_LC)

                            MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_REQUESTED.ToString)
                        End If
                    End If
                End If

                ManageTabPages = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CancelTanksTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelTanksTest ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fills the specific adjustment fields
    ''' </summary>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function FillAdjustmentValues() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            For Each E As EditedValueStruct In Me.EditedValues
                Select Case E.AdjustmentID
                    Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
                        Select Case E.LevelID
                            Case AXIS.EMPTY
                                Me.WSEmptySavedLabel.Text = E.NewLevelValue.ToString
                                'Me.WSMonitorTank.LowerLevelValue = E.NewLevelValue

                            Case AXIS.FULL
                                Me.WSFullSavedLabel.Text = E.NewLevelValue.ToString
                                'Me.WSMonitorTank.UpperLevelValue = E.NewLevelValue

                        End Select

                    Case ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                        Select Case E.LevelID
                            Case AXIS.EMPTY
                                Me.HCEmptySavedLabel.Text = E.NewLevelValue.ToString
                                'Me.HCMonitorTank.LowerLevelValue = E.NewLevelValue

                            Case AXIS.FULL
                                Me.HCFullSavedLabel.Text = E.NewLevelValue.ToString
                                'Me.HCMonitorTank.UpperLevelValue = E.NewLevelValue

                        End Select
                End Select
            Next




            ''show level values
            ''washing solution
            'With WSMonitorTank
            '    .LowerLevelVisible = True
            '    .UpperLevelVisible = True
            'End With

            ''high contamination
            'With HCMonitorTank
            '    .LowerLevelVisible = True
            '    .UpperLevelVisible = True
            'End With

            'hide new values labels
            Me.WSEmptyNewValueLabel.Visible = False
            Me.WSFullNewValueLabel.Visible = False
            Me.HCEmptyNewValueLabel.Visible = False
            Me.HCFullNewValueLabel.Visible = False

            'clear new values labels
            Me.WSEmptyNewValueLabel.Text = ""
            Me.WSFullNewValueLabel.Text = ""
            Me.HCEmptyNewValueLabel.Text = ""
            Me.HCFullNewValueLabel.Text = ""

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FillAdjustmentValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillAdjustmentValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Fills the specific adjustment fields
    ''' </summary>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function FillNewAdjustmentValue(ByVal pTankId As ADJUSTMENT_GROUPS, ByVal pLevel As GlobalEnumerates.AXIS, ByVal pValue As Single) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            Select Case pTankId
                Case ADJUSTMENT_GROUPS.WASHING_SOLUTION
                    Select Case pLevel
                        Case AXIS.EMPTY
                            Me.WSEmptyNewValueLabel.Text = pValue.ToString
                            Me.WSEmptyNewValueLabel.Visible = True
                            Me.WSMonitorTank.LowerLevelValue = pValue

                        Case AXIS.FULL
                            Me.WSFullNewValueLabel.Text = pValue.ToString
                            Me.WSFullNewValueLabel.Visible = True
                            Me.WSMonitorTank.UpperLevelValue = pValue

                    End Select

                Case ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                    Select Case pLevel
                        Case AXIS.EMPTY
                            Me.HCEmptyNewValueLabel.Text = pValue.ToString
                            Me.HCEmptyNewValueLabel.Visible = True
                            Me.HCMonitorTank.LowerLevelValue = pValue

                        Case AXIS.FULL
                            Me.HCFullNewValueLabel.Text = pValue.ToString
                            Me.HCFullNewValueLabel.Visible = True
                            Me.HCMonitorTank.UpperLevelValue = pValue

                    End Select
            End Select


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FillNewAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".FillNewAdjustmentValue  ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Set all the screen buttons to false
    ''' </summary>
    ''' <remarks>SGM 24/01/11</remarks>
    Private Sub DisableButtons()
        Try
            If MyBase.myScreenLayout.ButtonsPanel.SaveButton IsNot Nothing Then
                MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled = False
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Enabled = False
                MyBase.myScreenLayout.ButtonsPanel.ExitButton.Enabled = False
            Else
                Me.BsAdjustButton.Enabled = False
                Me.BsSaveButton.Enabled = False
                If CInt(MyClass.TestProcessStep) < 2 Then Me.BsCancelButton.Enabled = False
                Me.BsExitButton.Enabled = False
            End If

            If MyBase.myScreenLayout.ButtonsPanel.AdjustButton IsNot Nothing Then
                MyBase.myScreenLayout.ButtonsPanel.AdjustButton.Enabled = False
            Else
                Me.BsAdjustButton.Enabled = False
            End If

            ' XBC 08/11/2011
            ' Tab SCALES
            Me.WSEmptyAdjustButton.Enabled = False
            Me.WSFullAdjustButton.Enabled = False
            Me.HCEmptyAdjustButton.Enabled = False
            Me.HCFullAdjustButton.Enabled = False

            ' Tab INTERNAL TANKS
            Me.BsStartTestButton.Enabled = False
            'Me.BsStopTestButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableButtons ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set all the screen elements to disabled
    ''' </summary>
    ''' <remarks>SGM 24/01/11</remarks>
    Private Sub DisableAll()
        Try

            MyClass.DisableButtons() 'SGM 24/01/11

            MyClass.ManageTabPages = False


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeactivateAll ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Percent
    ''' </summary>
    ''' <remarks>SGM 11/03/11</remarks>
    Private Function GetPercentValue(ByVal pSensor As SENSOR, ByVal pValue As Single) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim min As Single
            Dim max As Single

            'minimum
            Select Case pSensor
                Case SENSOR.WASHING_SOLUTION_LEVEL
                    myGlobal = GetEditedNewValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.EMPTY)

                Case SENSOR.HIGH_CONTAMINATION_LEVEL
                    myGlobal = GetEditedNewValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.EMPTY)

            End Select

            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                min = CSng(myGlobal.SetDatos)
            End If


            'maximum
            Select Case pSensor
                Case SENSOR.WASHING_SOLUTION_LEVEL
                    myGlobal = GetEditedNewValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.FULL)

                Case SENSOR.HIGH_CONTAMINATION_LEVEL
                    myGlobal = GetEditedNewValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.FULL)

            End Select

            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                max = CSng(myGlobal.SetDatos)
            End If

            ''Dim myUtil As New Utilities.
            myGlobal = Utilities.CalculatePercent(pValue, min, max)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetPercentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".GetPercentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    Private Sub ExitScreen()
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim waitForScripts As Boolean = False
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
                MyClass.IsReadyToCloseAttr = True
                Me.Close()
                Exit Sub
            End If


            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.SCALES
                    If MyBase.myScreenLayout.ButtonsPanel.SaveButton.Enabled Then
                        dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SAVE_PENDING.ToString), Messages.SAVE_PENDING.ToString)

                        If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                            MyClass.ExitWhilePendingRequested = True
                            MyClass.SaveAdjustment()
                        End If
                    End If


                Case ADJUSTMENT_PAGES.INTERMEDIATE
                    If MyClass.TestProcessStep <> TEST_PROCESS_STEPS.NONE Then
                        dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_TEST_PENDING.ToString), Messages.SRV_TEST_PENDING.ToString)

                        If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                            MyClass.ExitWhileTestingRequested = True
                            MyClass.CancelTanksTest()
                        End If
                    End If

            End Select

            If Not ExitWhilePendingRequested And Not ExitWhileTestingRequested Then

                myGlobal = MyBase.CloseForm()
                If myGlobal.HasError Then
                    MyClass.PrepareErrorMode()
                Else

                    'report to history
                    If MyClass.IsScalesAdjusting And MyClass.IsPendingToHistoryScales Then
                        MyClass.ReportHistory(HISTORY_TASKS.ADJUSTMENT, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                    ElseIf MyClass.IsIntermediateTesting And MyClass.IsPendingToHistoryIntermediate Then
                        MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                    End If

                    MyClass.PrepareArea()
                    MyClass.IsReadyToCloseAttr = True
                    Me.Close()
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Must Inherited"

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/01/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            ' Stop Progress bar & Timer
            Me.ProgressBar1.Value = 0
            Me.ProgressBar1.Visible = False
            Me.ProgressBar1.Refresh()
            MyClass.TestProcessTimer.Enabled = False

            ' Tab SCALES
            Me.WSEmptyAdjustButton.Enabled = False
            Me.WSFullAdjustButton.Enabled = False
            Me.HCEmptyAdjustButton.Enabled = False
            Me.HCFullAdjustButton.Enabled = False

            ' Tab INTERNAL TANKS
            Me.BsStartTestButton.Enabled = False
            Me.BsStopTestButton.Enabled = False

            'report results to history 
            MyClass.ReportHistoryError()

            MyBase.ErrorMode()
            DisableAll()
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case

            If pAlarmType = ManagementAlarmTypes.NONE Then
                MyBase.ActivateMDIMenusButtons(True)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            MyClass.PrepareErrorMode(pAlarmType)

            'when stop action is finished, perform final operations after alarm received
            If pAlarmType <> ManagementAlarmTypes.NONE Then
                MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)
            End If

            Me.Cursor = Cursors.Default

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by XBC 17/05/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO
        Try
            'Dim validatedValue As Boolean
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then

                Dim myPercentValue As Single
                Dim mySensorValuesChangedDT As New UIRefreshDS.SensorValueChangedDataTable

                mySensorValuesChangedDT = pRefreshDS.SensorValueChanged
                'DisplayMessage("")

                For Each S As UIRefreshDS.SensorValueChangedRow In mySensorValuesChangedDT.Rows

                    Select Case S.SensorID
                        Case AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE.ToString
                            myGlobal = GetPercentValue(SENSOR.HIGH_CONTAMINATION_LEVEL, S.Value)
                            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                myPercentValue = CType(myGlobal.SetDatos, Single)

                                'SGM 19/04/2012
                                'Not to validate because of specification: 
                                Me.HCFullAdjustButton.Enabled = True
                                Me.HCEmptyAdjustButton.Enabled = True
                                ' Validations
                                'validatedValue = True
                                'If myPercentValue > Me.ScalesLimitMax Then
                                '    myPercentValue = Me.ScalesLimitMax
                                '    validatedValue = False
                                'Else
                                'End If
                                'If myPercentValue < Me.ScalesLimitMin Then
                                '    myPercentValue = Me.ScalesLimitMin
                                '    validatedValue = False
                                'End If
                                'If validatedValue Then

                                '    If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                '        Me.HCFullAdjustButton.Enabled = False
                                '        Me.HCEmptyAdjustButton.Enabled = False
                                '    Else
                                '        Me.HCFullAdjustButton.Enabled = True
                                '        Me.HCEmptyAdjustButton.Enabled = True
                                '    End If

                                'Else
                                '    MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
                                '    Me.HCFullAdjustButton.Enabled = False
                                '    Me.HCEmptyAdjustButton.Enabled = False
                                'End If
                                'end SGM 19/04/2012

                                Me.HCCurrentLabel.Visible = True
                                Me.HCCountsLabel.Visible = True
                                Me.HCCurrentPercentLabel.Visible = True
                                Me.HCPercentLabel.Visible = True

                                myPercentValue = CInt(myPercentValue)
                                If myPercentValue > 100 Then
                                    HCMonitorTank.LevelValue = 100
                                ElseIf myPercentValue < 0 Then
                                    HCMonitorTank.LevelValue = 0
                                Else
                                    HCMonitorTank.LevelValue = myPercentValue
                                End If

                                Me.HCCurrentLabel.Text = S.Value.ToString
                                Me.HCCurrentPercentLabel.Text = myPercentValue.ToString

                                SetEditedCurrentValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.FULL, S.Value)
                                SetEditedCurrentValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.EMPTY, S.Value)
                            Else
                                PrepareErrorMode()
                                Exit Sub
                            End If

                        Case AnalyzerSensors.BOTTLE_WASHSOLUTION.ToString
                            myGlobal = GetPercentValue(SENSOR.WASHING_SOLUTION_LEVEL, S.Value)
                            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                myPercentValue = CType(myGlobal.SetDatos, Single)

                                'SGM 19/04/2012
                                'Not to validate because of specification: 
                                Me.WSFullAdjustButton.Enabled = True
                                Me.WSEmptyAdjustButton.Enabled = True
                                ' Validations
                                'validatedValue = True
                                'If myPercentValue > Me.ScalesLimitMax Then
                                '    myPercentValue = Me.ScalesLimitMax
                                '    validatedValue = False
                                'Else
                                'End If
                                'If myPercentValue < Me.ScalesLimitMin Then
                                '    myPercentValue = Me.ScalesLimitMin
                                '    validatedValue = False
                                'End If
                                'If validatedValue Then

                                '    If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                '        Me.WSFullAdjustButton.Enabled = False
                                '        Me.WSEmptyAdjustButton.Enabled = False
                                '    Else
                                '        Me.WSFullAdjustButton.Enabled = True
                                '        Me.WSEmptyAdjustButton.Enabled = True
                                '    End If

                                'Else
                                '    DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
                                '    Me.WSFullAdjustButton.Enabled = False
                                '    Me.WSEmptyAdjustButton.Enabled = False
                                'End If
                                'end SGM 19/04/2012

                                Me.WSCurrentLabel.Visible = True
                                Me.WSCountsLabel.Visible = True
                                Me.WSCurrentPercentLabel.Visible = True
                                Me.WSPercentLabel.Visible = True

                                myPercentValue = CInt(myPercentValue)
                                If myPercentValue > 100 Then
                                    WSMonitorTank.LevelValue = 100
                                ElseIf myPercentValue < 0 Then
                                    WSMonitorTank.LevelValue = 0
                                Else
                                    WSMonitorTank.LevelValue = myPercentValue
                                End If

                                Me.WSCurrentLabel.Text = S.Value.ToString
                                Me.WSCurrentPercentLabel.Text = myPercentValue.ToString

                                SetEditedCurrentValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.FULL, S.Value)
                                SetEditedCurrentValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.EMPTY, S.Value)
                            Else
                                'PrepareErrorMode()
                                Exit Sub
                            End If

                    End Select

                Next

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region


#Region "AUX"
    Private Function FormatNum(ByVal pValue As String, ByVal pNumDecimals As Integer) As String
        Dim myValue As String = ""
        Dim myFormat As String
        Try
            If pNumDecimals > 0 Then
                myFormat = "0."
                For n As Integer = 1 To pNumDecimals Step 1
                    myFormat = myFormat + "0"
                Next
            Else
                myFormat = "0"
            End If

            myValue = CDbl(pValue).ToString(myFormat).Replace(",", ".")

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshSpecificAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshSpecificAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myValue
    End Function
#End Region

#Region "Events"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub ITankLevelsAdjustments_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else

                'report to history
                If MyClass.IsScalesAdjusting And MyClass.IsPendingToHistoryScales Then
                    MyClass.ReportHistory(HISTORY_TASKS.ADJUSTMENT, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                ElseIf MyClass.IsIntermediateTesting And MyClass.IsPendingToHistoryIntermediate Then
                    MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                End If

                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing

                'SGM 28/02/2012
                MyBase.ActivateMDIMenusButtons(Not MyBase.CloseRequestedByMDI)
                MyBase.myServiceMDI.IsFinalClosing = MyBase.CloseRequestedByMDI

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    'Private Sub ITankLevelsAdjustments_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
    '    Try

    '        MyClass.ExitScreen()

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
    Private Sub TankLevelsAdjustments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            ''sensors
            'With MyBase.SensorsTimer
            '    .Enabled = False
            '    .Interval = MyBase.SensorsSampleRate
            '    AddHandler .Tick, AddressOf SensorsTimer_Tick
            'End With

            'Get the current Language from the current Application Session
            MyClass.LanguageId = GlobalBase.GetSessionInfo.ApplicationLanguage.Trim.ToString

            MyClass.GetScreenLabels(MyClass.LanguageId)

            'Monitor Tanks
            MyClass.InitializeMonitorTanks()

            'Internal Tanks Test Grid
            MyClass.InitializeTestGrid()

            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New TankLevelsAdjustmentDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'Get the current user level
            MyBase.GetUserNumericalLevel()
            If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                Me.BsAdjustButton.Visible = False
                Me.BsCancelButton.Visible = False
                Me.BsSaveButton.Visible = False
            End If

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            'GetScreenLabels(currentLanguage)

            MyClass.SelectedPage = ADJUSTMENT_PAGES.SCALES
            MyClass.DefineScreenLayout()

            MyClass.PrepareButtons()

            'SGM 12/11/2012 - Information
            MyBase.DisplayInformation(APPLICATION_PAGES.TANKS_SCALES, Me.BsInfoScalesXPSViewer)


            MyClass.DisableAll()

            myGlobal = GetLimitValues()

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            ' TO DELETE
            'myGlobal = InitializeSensors() 'SGM 01/03/11

            'myGlobal = InitializeMonitor() 'SGM 22/03/11
            ' TO DELETE

            ' Check communications with Instrument
            If Not MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                myGlobal.ErrorCode = "ERROR_COMM"
                myGlobal.HasError = True
                ' Prepare Error Mode Controls in GUI
                MyClass.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                Me.PrepareArea()
            Else
                ' Reading Adjustments
                If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                    ' Parent Reading Adjustments
                    MyBase.ReadAdjustments()
                    PrepareArea()

                    ' Manage FwScripts must to be sent at load screen
                    If MyBase.SimulationMode Then
                        System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)

                        '' simulating...
                        ''LoadAdjustmentGroupData()
                        'myGlobal = MyBase.SimulateRequestAdjustmentsFromAnalyzer()

                        'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                        'Me.Cursor = Cursors.WaitCursor
                        'System.Threading.Thread.Sleep(SimulationProcessTime)
                        'MyBase.myServiceMDI.Focus()
                        'Me.Cursor = Cursors.Default
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                        'PrepareArea()
                    Else
                        If Not myGlobal.HasError AndAlso MyBase.myServiceMDI.MDIAnalyzerManager.Connected Then
                            myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                        End If
                    End If
                Else
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If MyBase.SimulationMode Then
                        MyClass.PrepareSimulationAdjustmentsReadedMode()
                    End If
                    PrepareArea()
                End If

            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                MyBase.ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            MyBase.MyBase_Load(sender, e)

            ResetBorderSRV()

            'remove until this Test is available
            Me.BsTabPagesControl.TabPages.Remove(Me.BsIntermediateTabPage)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 12/11/2012</remarks>
    Private Sub ITankLevelsAdjustments_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try

            Me.BsInfoScalesXPSViewer.Visible = True
            Me.BsInfoScalesXPSViewer.RefreshPage()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsTabPagesControl_Deselecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Deselecting

        Try

            If Me.BsTabPagesControl.SelectedTab Is BsScalesTabPage Then

                If MyClass.IsScalesAdjusting And MyClass.IsPendingToHistoryScales Then
                    MyClass.ReportHistory(HISTORY_TASKS.ADJUSTMENT, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                End If

                If MyClass.IsIntermediateTesting And MyClass.IsPendingToHistoryIntermediate Then
                    MyClass.ReportHistory(HISTORY_TASKS.TEST, TankLevelsAdjustmentDelegate.HISTORY_RESULTS.CANCEL)
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Deselecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Deselecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.PrepareErrorMode()
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles BsTabPagesControl.Selected
        Try
            If MyClass.IsInfoExpanded Then MyBase.ExpandInformation(False, MyClass.SelectedInfoPanel, MyClass.SelectedAdjPanel, MyClass.SelectedInfoExpandButton)

            Application.DoEvents()

            MyClass.IsInfoExpanded = False

            If e.TabPage Is BsScalesTabPage Then
                MyClass.SelectedInfoPanel = Me.BsTanksInfoPanel
                MyClass.SelectedAdjPanel = Me.BsTanksAdjustPanel
                'MyClass.SelectedInfoExpandButton = Me.BsInfoStep1ExpandButton
                Me.BsInfoScalesXPSViewer.RefreshPage()

            ElseIf e.TabPage Is BsIntermediateTabPage Then
                'MyClass.SelectedInfoPanel = Me.BsStep2InfoPanel
                'MyClass.SelectedAdjPanel = Me.BsStep2AdjustPanel
                'MyClass.SelectedInfoExpandButton = Me.BsInfoStep2ExpandButton

                Me.BsStartTestButton.Enabled = True
                Me.BsStartTestButton.Visible = True
                Me.BsStopTestButton.Visible = False

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.PrepareErrorMode()
        End Try
    End Sub

    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles BsTabPagesControl.Selecting

        Try

            If Not MyClass.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            'disable for PROTO 4
            If e.TabPage Is Me.BsIntermediateTabPage Then
                e.Cancel = True
                Exit Sub
            End If

            MyClass.IsInfoExpanded = False

            If Me.BsTabPagesControl.SelectedTab Is BsScalesTabPage Then
                MyClass.SelectedPage = ADJUSTMENT_PAGES.SCALES
                MyClass.PrepareLoadedMode()
                'PrepareAdjustPreparedMode()
                MyClass.DefineScreenLayout()
                Me.TestProgressBar.Visible = False

                'define current tasks
                MyClass.IsScalesAdjusting = False
                MyClass.IsIntermediateTesting = False

            ElseIf Me.BsTabPagesControl.SelectedTab Is BsIntermediateTabPage Then
                MyClass.SelectedPage = ADJUSTMENT_PAGES.INTERMEDIATE
                MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                MyClass.DefineScreenLayout()

                'System Input
                MyClass.InitializeSystemInput()

                'for testing: get from sensors
                Me.DWMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.MIDDLE
                Me.LCMonitorTank.TankLevel = BSMonitorTankLevels.TankLevels.TOP

                'define current tasks
                MyClass.IsScalesAdjusting = False
                MyClass.IsIntermediateTesting = False

            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabControl1_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabControl1_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.PrepareErrorMode()
        End Try
    End Sub

    Private Sub AdjustButtons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WSFullAdjustButton.Click, _
                                                                                                        WSEmptyAdjustButton.Click, _
                                                                                                        HCFullAdjustButton.Click, _
                                                                                                        HCEmptyAdjustButton.Click
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myButton As Button = CType(sender, Button)
            If myButton IsNot Nothing Then

                Dim myTank As ADJUSTMENT_GROUPS
                Dim myLevel As AXIS
                Dim myValue As Single

                If myButton Is Me.WSEmptyAdjustButton Then
                    myTank = ADJUSTMENT_GROUPS.WASHING_SOLUTION
                    myLevel = AXIS.EMPTY
                End If

                If myButton Is Me.WSFullAdjustButton Then
                    myTank = ADJUSTMENT_GROUPS.WASHING_SOLUTION
                    myLevel = AXIS.FULL
                End If

                If myButton Is Me.HCEmptyAdjustButton Then
                    myTank = ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                    myLevel = AXIS.EMPTY
                End If

                If myButton Is Me.HCFullAdjustButton Then
                    myTank = ADJUSTMENT_GROUPS.HIGH_CONTAMINATION
                    myLevel = AXIS.FULL
                End If

                If (myTank <> ADJUSTMENT_GROUPS.WASHING_SOLUTION Or _
                myTank <> ADJUSTMENT_GROUPS.HIGH_CONTAMINATION) And _
                myLevel <> AXIS._NONE Then

                    myGlobal = GetEditedCurrentValue(myTank, myLevel)
                    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                        myValue = CSng(myGlobal.SetDatos)

                        myGlobal = ValidateLevel(myTank, myLevel, myValue)

                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                            If CBool(myGlobal.SetDatos) Then

                                myGlobal = SaveLocal(myTank, myLevel, myValue)
                                If Not myGlobal.HasError Then

                                    'update current tasks
                                    MyClass.IsScalesAdjusting = True
                                    MyClass.IsIntermediateTesting = False

                                    If MyBase.CurrentUserNumericalLevel = USER_LEVEL.lOPERATOR Then
                                        Me.BsSaveButton.Enabled = False
                                        Me.BsCancelButton.Enabled = False
                                    Else
                                        Me.BsSaveButton.Enabled = True
                                        Me.BsCancelButton.Enabled = True
                                        MyClass.IsPendingToHistoryScales = True
                                    End If
                                    DisplayMessage(Messages.SRV_NEW_VALUE_SET.ToString)
                                    ManageTabPages = False

                                    ' XBC 08/11/2011 - Errors has been already displayed but are not a kind of errors for to redirect into PrepareErrorMode
                                    'Else
                                    '    MyClass.PrepareErrorMode()

                                End If
                            End If

                        End If

                    End If

                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AdjustButtons_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AdjustButtons_Click", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            MyClass.ExitScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
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
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub StartTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStartTestButton.Click
        Try
            Dim myGlobal As New GlobalDataTO
            ' initializations
            myScreenDelegate.EmptyLcDone = False
            myScreenDelegate.FillDwDone = False
            myScreenDelegate.TransferDwLcDone = False
            myScreenDelegate.EmptyLcDoing = False
            myScreenDelegate.FillDwDoing = False
            myScreenDelegate.TransferDwLcDoing = False

            myGlobal = MyClass.RequestTanksTest()
            If Not myGlobal.HasError Then
                BsStartTestButton.Enabled = False
                BsStopTestButton.Visible = True
                BsStopTestButton.Enabled = True
                InitializeTestGrid()
                MyClass.IsPendingToHistoryIntermediate = True
            Else
                TankTestRequested = False
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StartTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StartTestButton_Click", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CancelTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsStopTestButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim waitForScripts As Boolean = False
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
                MyClass.IsReadyToCloseAttr = True
                Me.Close()
                Exit Sub
            End If

            dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_TEST_PENDING.ToString), Messages.SRV_TEST_PENDING.ToString)

            If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                MyClass.CancelTanksTest()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CancelTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CancelTestButton_Click", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsSaveButton.Click
        Try
            MyClass.SaveAdjustment()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsSaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCancelButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Try

            'Dim myMessage As String = ""
            'Dim myMessage2 As String = ""

            Select Case Me.SelectedPage
                Case ADJUSTMENT_PAGES.SCALES
                    'myMessage = Messages.SRV_ADJUST_QUIT.ToString
                    'myMessage2 = Messages.SRV_ADJUSTMENTS_CANCELLED.ToString

                    dialogResultToReturn = MyBase.ShowMessage("", Messages.SRV_DISCARD_CHANGES.ToString)

                    If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                        CancelAdjustment()
                    End If

                Case ADJUSTMENT_PAGES.INTERMEDIATE

                    'myMessage = Messages.SRV_TEST_QUIT.ToString
                    'myMessage2 = Messages.SRV_TEST_EXIT_COMPLETED.ToString

            End Select

            'If myMessage.Length > 0 Then
            '    dialogResultToReturn = mybase.ShowMessage(GetMessageText(myMessage), myMessage)
            '    If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
            '        Me.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
            '        PrepareArea()

            '        MyBase.DisplayMessage(myMessage2)
            '    End If
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsCancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub TestProcessTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestProcessTimer.Tick
        Try
            Me.ProgressBar1.Value += 1
            Me.ProgressBar1.Refresh()

            Dim myStep As Integer = 0
            Select Case MyClass.TestProcessStepAttr
                Case TEST_PROCESS_STEPS.INITIATED
                    MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING
                    myStep += 1
                Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY
                    MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING
                    myStep += 1
                Case TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
                    MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.TRANSFERING
                    myStep += 1
            End Select

            If myStep > 0 Then
                UpdateTestProcess()
            End If

            'TestProcessTimer.Enabled = False
            'RaiseEvent ProcessTimeOut(MyClass.TestProcessStep)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".TestProcessTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TestProcessTimer_Tick", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub OnTankLevelChanged(ByVal pTank As INTERMEDIATE_TANKS) Handles Me.TankLevelChanged
        Try
            Select Case pTank
                Case INTERMEDIATE_TANKS.DISTILLED_WATER
                    Me.DWMonitorTank.TankLevel = MyClass.TanksLevelsStatus.DW_Level

                Case INTERMEDIATE_TANKS.LOW_CONTAMINATION
                    Me.LCMonitorTank.TankLevel = MyClass.TanksLevelsStatus.LC_Level

            End Select

            Me.DWMonitorTank.Refresh()
            Me.LCMonitorTank.Refresh()

            'update process step
            UpdateTestProcess()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnTankLevelChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".OnTankLevelChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub OnTankLevelUndefined(ByVal pTank As INTERMEDIATE_TANKS) Handles Me.TankLevelUndefined
        Try
            Select Case pTank
                Case INTERMEDIATE_TANKS.DISTILLED_WATER
                    'Me.DWMonitorTank.TankLevel = MyClass.TestProcessStatus.DW_Level
                    MessageBox.Show("Distilled Water Tank level UNDEFINED")

                Case INTERMEDIATE_TANKS.LOW_CONTAMINATION
                    'Me.LCMonitorTank.TankLevel = MyClass.TestProcessStatus.LC_Level
                    MessageBox.Show("Low Contamination Waste Tank level UNDEFINED")

            End Select

            Me.DWMonitorTank.Refresh()
            Me.LCMonitorTank.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnTankLevelUndefined ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".OnTankLevelUndefined ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub






    'Private myStep As Integer = 0
    Private Sub TestSimulatorTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestSimulatorTimer.Tick
        Try
            ' Stop Progress bar & Timer
            Me.ProgressBar1.Value = 0
            Me.ProgressBar1.Visible = False
            Me.ProgressBar1.Refresh()
            MyClass.TestProcessTimer.Enabled = False
            Me.TestSimulatorTimer.Enabled = False

            If myScreenDelegate.TransferDwLcDone And myScreenDelegate.FillDwDone And myScreenDelegate.EmptyLcDone Then
                ' Refresh values
                MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_FINISHED.ToString)
                MyClass.PrepareTestFinishedMode()

            ElseIf myScreenDelegate.FillDwDone And myScreenDelegate.EmptyLcDone Then
                ' Refresh values
                MyBase.DisplayMessage(Messages.SRV_TEST_TANKS_TRANSFER.ToString)
                MyClass.PrepareTestFillDWEndedMode()

            ElseIf myScreenDelegate.EmptyLcDone Then
                ' Refresh values
                MyBase.DisplayMessage(Messages.SRV_TEST_DW_FILLING.ToString)
                MyClass.PrepareTestEmptyLCEndedMode()
            End If


            'If MyBase.CurrentMode = ADJUSTMENT_MODES.ADJUST_EXITING Then
            '    TestSimulatorTimer.Enabled = False
            '    Exit Sub
            'End If

            'If myStep = 0 Then
            '    Select Case MyClass.TestProcessStepAttr
            '        Case TEST_PROCESS_STEPS.INITIATED
            '            MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTYING

            '        Case TEST_PROCESS_STEPS.LOW_CONTAMINATION_EMPTY
            '            MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.DISTILLED_WATER_FILLING

            '        Case TEST_PROCESS_STEPS.DISTILLED_WATER_FULL
            '            MyClass.TestProcessStepAttr = TEST_PROCESS_STEPS.TRANSFERING

            '    End Select

            '    UpdateTestProcess()

            '    myStep += 1
            'Else
            '    TestSimulatorTimer.Enabled = False
            '    OnScreenReceptionLastFwScriptEvent(GlobalEnumerates.RESPONSE_TYPES.OK, "1")
            '    myStep = 0
            'End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ' XBC 19/05/2011 Timeout is managed from Communications layer
    'Private Sub OnTestProcessTimeOut(ByVal pstep As TEST_PROCESS_STEPS) Handles MyClass.ProcessTimeOut
    '    Try
    '        Select Case pstep

    '        End Select

    '        'PENDING TO SPEC
    '        MessageBox.Show("Test Process TimeOut")

    '        MyClass.TestProcessStep = TEST_PROCESS_STEPS.NONE

    '        PrepareErrorMode()

    '        TestProcessTimer.Enabled = False

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnTestProcessTimeOut ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".OnTestProcessTimeOut ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    'Private Sub OnTestProcessStepUndefined() Handles MyClass.ProcessStepUndefined  ' is used ???
    '    Try

    '        'PENDING TO SPEC
    '        MessageBox.Show("Test Process Step Undefined")

    '        MyClass.TestProcessStep = TEST_PROCESS_STEPS.NONE

    '        PrepareErrorMode()

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnTestProcessTimeOut ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        mybase.ShowMessage(Me.Name & ".OnTestProcessTimeOut ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub
#End Region


#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoScalesXPSViewer.Load
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
