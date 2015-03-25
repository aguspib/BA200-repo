﻿Option Strict On
Option Infer On

Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Namespace Biosystems.Ax00.Core.Services

    Public Enum BaseLineStepsEnum
        NotStarted
        CheckPreviousAlarms
        ConditioningWashing
        StaticBaseLine
        DynamicBaseLineFill
        DynamicBaseLineRead
        DynamicBaseLineEmpty
        Finalize
        None
    End Enum

    Public Class BaseLineService
        Inherits AsyncService

#Region "Consutrcutors"
        Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)
            Me._currentStep = BaseLineStepsEnum.NotStarted
        End Sub
#End Region

#Region "Public methods"

        'Public Overrides Function StartService() As Boolean
        '    Return StartService(False)
        'End Function

        Public Overrides Function StartService() As Boolean

            'Previous conditions: (no flow here)
            If _analyzer.Connected = False Then Return False
            If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then Return False

            'Method flow:
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'If isInRecover Then
            'InitializeRecover()
            'Else
            Initialize()

            _analyzer.StopAnalyzerRinging()
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "")

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

            'End If

            Status = ServiceStatusEnum.Running
            AddRequiredEventHandlers()

            Return True

        End Function


        Private Sub DirectlyGoToDynamicReadStep()
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Ended) 'Re-send Washing
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, StepStringStatus.Ended) 'Re-send ALIGHT
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, StepStringStatus.Ended) 'Re-send FLIGHT mode fill
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, StepStringStatus.Initialized)

            UpdateFlags(myAnalyzerFlagsDs)
            'Re-send FLIGHT mode read

            'We set propper internal flags, it's treated the same as a recover process that goes directly to the reading
            InitializeRecover()

        End Sub

        Private Sub UpdateFlags(ByVal FlagsDS As AnalyzerManagerFlagsDS)

            If FlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, FlagsDS)
            End If
        End Sub

        Private Sub PerformAEmptyRotorStep()

        End Sub

        ''' <summary>
        ''' Set the flags into a stable value for repeat last action and recover the process
        ''' </summary>
        ''' <remarks>Created by:  AG 20/01/2015 BA-2216
        '''          Modified by: IT 16/02/2015 BA-2266
        '''  </remarks>
        Private Sub InitializeRecover()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = StepStringStatus.Initialized) Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty) 'Re-send Washing

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = StepStringStatus.Initialized) Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, StepStringStatus.Empty) 'Re-send ALIGHT

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = StepStringStatus.Initialized) Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, StepStringStatus.Empty) 'Re-send FLIGHT mode fill

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = StepStringStatus.Initialized) Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, StepStringStatus.Empty) 'Re-send FLIGHT mode read

            ElseIf ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Empty) Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Initialized)) And
                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = StepStringStatus.Canceled) Then
                _dynamicBaseLineValid = False
                _forceEmptyAndFinalize = True
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, StepStringStatus.Empty) 'Re-send FLIGHT mode read

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Initialized) Then
                _staticBaseLineFinished = True
                _dynamicBaseLineValid = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, StepStringStatus.Empty) 'Re-send FLIGHT mode empty

                'NEWROTORprocess in PAUSED status
            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Canceled) And
                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = StepStringStatus.Canceled) Then
                _forceEmptyAndFinalize = True
                _dynamicBaseLineValid = False
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Canceled) And
               (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = StepStringStatus.Ended) Then
                _dynamicBaseLineValid = True

            End If

            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        Public Function CurrentStep() As BaseLineStepsEnum
            Return _currentStep
        End Function

        Public Sub EmptyAndFinalizeProcess()
            _forceEmptyAndFinalize = True
            Status = ServiceStatusEnum.Running
        End Sub


        Public Function RecoverProcess() As Boolean
            Try
                _isInRecovering = True
                '_analyzer.CurrentInstructionAction = InstructionActions.None 'AG 04/02/2015 BA-2246 (informed in the event of USB disconnection AnalyzerManager.ProcessUSBCableDisconnection)
                InitializeRecover()

                Dim nextStep = GetNextStep()
                Select Case nextStep
                    Case BaseLineStepsEnum.Finalize,
                        BaseLineStepsEnum.DynamicBaseLineRead
                        ValidateProcess()
                    Case BaseLineStepsEnum.DynamicBaseLineEmpty
                        ProcessDynamicBaseLine()
                End Select

                _isInRecovering = False
                AddRequiredEventHandlers()
                Return True

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex)
                Return False
            End Try
        End Function

        Public Sub RepeatDynamicBaseLineReadStep()

            _analyzer.DynamicBaselineInitializationFailures = 0

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

            RestartProcess()

            ExecuteDynamicBaseLineReadStep()

        End Sub

#End Region

#Region "Attributes"

        Private _forceEmptyAndFinalize As Boolean = False
        Private _checkedPreviousAlarms As Boolean = False
        Private _staticBaseLineFinished As Boolean = False
        Private _dynamicBaseLineValid As Boolean = False
        Private _currentStep As BaseLineStepsEnum
        Private _alreadyFinalized As Boolean = False
        Private _isInRecovering As Boolean = False
        Private eventHandlersAdded As Boolean = False

#End Region

#Region "Event Handlers"

        Private Sub OnReceivedStatusInformationEvent()
            'Handles _analyzer.ReceivedStatusInformationEventHandler
            'Debug.WriteLine("Hello 1")
            If _currentStep = BaseLineStepsEnum.NotStarted Then Return
            TryToResume()
        End Sub

        Private Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags)
            'Handles _analyzer.ProcessFlagEventHandler
            'Debug.WriteLine("Hello 2")
            If _currentStep = BaseLineStepsEnum.NotStarted Then Return
            Select Case pFlagCode
                Case AnalyzerManagerFlags.BaseLine
                    ProcessStaticBaseLine()

                Case AnalyzerManagerFlags.DynamicBL_Fill,
                    AnalyzerManagerFlags.DynamicBL_Empty
                    ValidateProcess()

                Case AnalyzerManagerFlags.DynamicBL_Read
                    ProcessDynamicBaseLine()

            End Select
        End Sub

#End Region

#Region "Private methods"

        Private Sub Initialize()
            _currentStep = BaseLineStepsEnum.None
            _forceEmptyAndFinalize = False
            _staticBaseLineFinished = False
            _dynamicBaseLineValid = False
            _analyzer.DynamicBaselineInitializationFailures = 0
            _analyzer.CurrentInstructionAction = InstructionActions.None
            _alreadyFinalized = False

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If
            Status = ServiceStatusEnum.Running

        End Sub

        ''' <summary>
        ''' This method will try to resume from pause mode and continue the FLIGHT process
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub TryToResume()

            Dim nextStep As BaseLineStepsEnum
            nextStep = GetNextStep()
            Select Case nextStep
                Case BaseLineStepsEnum.ConditioningWashing,
                     BaseLineStepsEnum.StaticBaseLine,
                     BaseLineStepsEnum.DynamicBaseLineFill,
                     BaseLineStepsEnum.DynamicBaseLineEmpty,
                     BaseLineStepsEnum.CheckPreviousAlarms
                    ValidateProcess()
            End Select

        End Sub

        Private Function GetNextStep() As BaseLineStepsEnum
            Dim nextStep = BaseLineStepsEnum.None
            If Not _checkedPreviousAlarms Then
                nextStep = BaseLineStepsEnum.CheckPreviousAlarms

            Else
                If Status = ServiceStatusEnum.Running Then
                    If _alreadyFinalized Then Return BaseLineStepsEnum.None
                    nextStep = GetNextStepWhileInProcess(nextStep)


                ElseIf Status = ServiceStatusEnum.Paused Then

                    'If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) <> "INI") Then


                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = "CANCELED") Then
                        nextStep = BaseLineStepsEnum.ConditioningWashing

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        nextStep = BaseLineStepsEnum.StaticBaseLine

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED") Then
                        nextStep = BaseLineStepsEnum.DynamicBaseLineFill

                    ElseIf ((_isInRecovering) And (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") And (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "")) Then
                        nextStep = BaseLineStepsEnum.DynamicBaseLineRead

                    ElseIf ((_forceEmptyAndFinalize) Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                        nextStep = BaseLineStepsEnum.DynamicBaseLineEmpty

                    End If
                    'End If
                End If

            End If
            Return nextStep



        End Function

        Private Function GetNextStepWhileInProcess(ByVal nextStep As BaseLineStepsEnum) As BaseLineStepsEnum

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                Case "", "CANCELED"
                    nextStep = BaseLineStepsEnum.ConditioningWashing

                Case "END"
                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "") OrElse (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        nextStep = BaseLineStepsEnum.StaticBaseLine

                    Else

                        If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then

                            If (_analyzer.CurrentInstructionAction = InstructionActions.None) Then

                                If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") And
                                   ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "") OrElse
                                    (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED")) Then
                                    nextStep = BaseLineStepsEnum.DynamicBaseLineFill

                                ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "END") And
                                       (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "") Then
                                    nextStep = BaseLineStepsEnum.DynamicBaseLineRead

                                ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END") And
                                       ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "") OrElse
                                        (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                                    nextStep = BaseLineStepsEnum.DynamicBaseLineEmpty

                                ElseIf (_forceEmptyAndFinalize) AndAlso (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) <> "END") Then
                                    nextStep = BaseLineStepsEnum.DynamicBaseLineEmpty

                                End If

                            End If

                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then
                                nextStep = BaseLineStepsEnum.Finalize
                            End If

                        Else
                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") OrElse
                               (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                                nextStep = BaseLineStepsEnum.Finalize
                            End If
                        End If

                    End If
            End Select
            If nextStep <> BaseLineStepsEnum.None Then
                Debug.Print("Currently doing: " & nextStep.ToString)
            End If
            Return nextStep
        End Function


        Private Sub ProcessStaticBaseLine()
            _staticBaseLineFinished = True

            If (_analyzer.BaseLineTypeForCalculations <> BaseLineType.DYNAMIC) Then
                ValidateProcess()
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateProcess()

            _currentStep = GetNextStep()

            Select Case _currentStep

                Case BaseLineStepsEnum.CheckPreviousAlarms
                    CheckPreviousAlarms()

                Case BaseLineStepsEnum.ConditioningWashing
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        RestartProcess()
                        ExecuteWashingStep()
                    Else
                        WashingStepPaused()
                    End If

                Case BaseLineStepsEnum.StaticBaseLine
                    If (Not _staticBaseLineFinished) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            RestartProcess()
                            ExecuteStaticBaseLineStep()
                        Else
                            StaticBaseLineStepPaused()
                        End If
                    Else
                        If (Not IsValidStaticBaseLine()) Then
                            StaticBaseLineStepPaused()
                            FinalizeProcess()
                        End If
                    End If


                Case BaseLineStepsEnum.DynamicBaseLineFill
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        RestartProcess()
                        ExecuteDynamicBaseLineFillStep()
                    Else
                        DynamicBaseLineFillStepPaused()
                    End If

                Case BaseLineStepsEnum.DynamicBaseLineRead
                    RestartProcess()
                    ExecuteDynamicBaseLineReadStep()

                Case BaseLineStepsEnum.DynamicBaseLineEmpty
                    If (IsEmptyingAllowed()) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            RestartProcess()
                            ExecuteDynamicBaseLineEmptyStep()
                        Else
                            DynamicBaseLineEmptyStepPaused()
                        End If
                    Else
                        If (IsReadingAllowed()) Then
                            RestartProcess()
                            ExecuteDynamicBaseLineReadStep()
                        Else
                            DynamicBaseLineReadPaused()
                        End If
                    End If

                Case BaseLineStepsEnum.Finalize
                    FinalizeProcess()

            End Select

            Return
        End Sub

        Private Sub RestartProcess()

            AddRequiredEventHandlers()
            Status = ServiceStatusEnum.Running

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteWashingStep()
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'Case Start instrument failed before wash was performed (for example no reactions missing)
            'User execute the change reaction rotor: Sw instructions NRotor + Wash + Alight
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "INI")
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH, True) 'Send a WASH instruction (Conditioning complete)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub WashingStepPaused()
            'Prefviously names CancelWashingStep

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "CANCELED")


                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)

                'TODO: MIC Discuss later
                '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If
            Status = ServiceStatusEnum.Paused

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified AG 15/01/2015 BA-2212
        ''' </remarks>
        Private Sub ExecuteStaticBaseLineStep()
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'Alight
            'User execute the change reaction rotor when start instrument OK: Sw instructions NRotor + Alight

            'Delete the current reactions rotor status (configuration) and crete a new one
            Dim cfgAnReactionsRotor As New AnalyzerReactionsRotorDelegate
            Dim resultData As GlobalDataTO = cfgAnReactionsRotor.ChangeRotorPerformed(Nothing, _analyzer.ActiveAnalyzer)

            'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
            If Not resultData.HasError Then
                Dim aLightDelg As New WSBLinesDelegate
                resultData = aLightDelg.DeleteBLinesValues(Nothing, _analyzer.ActiveAnalyzer, _analyzer.ActiveWorkSession, "", _analyzer.BaseLineTypeForCalculations.ToString) 'AG 15/01/2015 BA-2212 inform new parameter BaseLineTypeForCalculations)
                If Not resultData.HasError Then
                    'Once the conditioning is finished the Sw send an ALIGHT instruction 
                    _analyzer.ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                    'AG 12/09/2011 - when change rotor is performed the ALIGHt well starts in 1, ignore the well field in status instruction (RPalazon, JGelabert, STortosa)
                    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, pNextWell)
                    resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)

                    'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                    If Not resultData.HasError AndAlso _analyzer.Connected Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "INI")
                        _analyzer.SetAnalyzerNotReady()
                    End If
                End If
            End If

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        Private Sub ProcessDynamicBaseLine()
            _dynamicBaseLineValid = _analyzer.ProcessFlightReadAction()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub StaticBaseLineStepPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) <> "CANCELED") Then
                '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "CANCELED")

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)

                '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)

            End If
            Status = ServiceStatusEnum.Paused
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsValidStaticBaseLine() As Boolean
            If (_staticBaseLineFinished) Then
                Return (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) <> "CANCELED")
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FinalizeProcess()

            'Reentrance check:
            If _alreadyFinalized Then Return
            _alreadyFinalized = True

            RemoveRequiredEventHandlers()
            NotifyProcessResult()

        End Sub

        Private Sub NotifyProcessResult()

            'We provide results callback
            'Dim status = ServiceStatusEnum.EndError
            If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then
                If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then
                    Status = ServiceStatusEnum.EndSuccess
                Else
                    Status = ServiceStatusEnum.EndError
                End If
            Else
                If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") Then
                    Status = ServiceStatusEnum.EndSuccess
                Else
                    Status = ServiceStatusEnum.EndError
                End If
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineFillStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightFilling
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.FillRotor), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady()

            'Ensure empiting always after a fill operation
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "")
            Me.UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DynamicBaseLineFillStepPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) <> "CANCELED") Then
                '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)

                '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If
            Status = ServiceStatusEnum.Paused

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineReadStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightReading
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.Perform), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            _dynamicBaseLineValid = False
            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsEmptyingAllowed() As Boolean
            Return (_dynamicBaseLineValid Or _forceEmptyAndFinalize)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineEmptyStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightEmptying
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.EmptyRotor), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready
            _forceEmptyAndFinalize = False

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DynamicBaseLineEmptyStepPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) <> "CANCELED") Then
                '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")

                'Update analyzer session flags into DataBase

                UpdateFlags(myAnalyzerFlagsDs)
                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If
            Status = ServiceStatusEnum.Paused
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsReadingAllowed() As Boolean
            Return (_analyzer.DynamicBaselineInitializationFailures < _analyzer.FlightInitFailures)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub DynamicBaseLineReadPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) <> "CANCELED") Then
                '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "CANCELED")

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.DYNAMIC_BASELINE_ERROR, 1, True)
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)

                'Update analyzer session flags into DataBase

                UpdateFlags(myAnalyzerFlagsDs)

                'FinalizeProcess()   'MI
                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If
            Status = ServiceStatusEnum.Paused

        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            RemoveRequiredEventHandlers()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub RemoveRequiredEventHandlers()
            If eventHandlersAdded Then
                Try
                    RemoveHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf Me.OnReceivedStatusInformationEvent
                    RemoveHandler _analyzer.ProcessFlagEventHandler, AddressOf Me.OnProcessFlagEvent
                    eventHandlersAdded = False
                Catch : End Try
            End If
        End Sub

        Private Sub AddRequiredEventHandlers()
            If Not eventHandlersAdded Then
                AddHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf Me.OnReceivedStatusInformationEvent
                AddHandler _analyzer.ProcessFlagEventHandler, AddressOf Me.OnProcessFlagEvent
                eventHandlersAdded = True
            End If
        End Sub

#End Region

        Private Sub CheckPreviousAlarms()
            'Previous constraint:
            _checkedPreviousAlarms = True
            If _analyzer.Connected = False Then Return

            'TODO: Replace with actual code from Alarms Cross-Cut being developed
            Dim Alarm551Present As Boolean = True, Alarm552Present As Boolean = False
            Dim Alarm551Date = Now.AddMinutes(-15)
            '/TODO

            If Alarm552Present Then
                ExecuteDynamicBaseLineEmptyStep()
            ElseIf Alarm551Present Then
                ProcessAlarmFullCleanRotor(Alarm551Date)
            Else
                'No alarms to process!
            End If

            'Throw New NotImplementedException
        End Sub

        Private Sub ProcessAlarmFullCleanRotor(pAlarmDate As Date)
            Dim minutosCaducidadRotorLleno = 30
            Dim caducityParameter = GeneralSettingsDelegate.GetGeneralSettingValue(GlobalEnumerates.GeneralSettingsEnum.FLIGHT_FULL_ROTOR_CADUCITY)
            If caducityParameter IsNot Nothing AndAlso caducityParameter.HasError = False Then minutosCaducidadRotorLleno = CInt(caducityParameter.SetDatos)

            If pAlarmDate < Now.AddMinutes(-minutosCaducidadRotorLleno) Then
                '551 caducada
                ExecuteDynamicBaseLineEmptyStep()
            Else
                '551 correcta

            End If
        End Sub

    End Class

End Namespace

