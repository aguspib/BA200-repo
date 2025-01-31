﻿Option Strict On
Option Infer On

Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Core.Services.Interfaces
Imports Biosystems.Ax00.Core.Entities

Namespace Biosystems.Ax00.Core.Services
    Public Class BaseLineService
        Inherits AsyncService
        Implements IBaseLineService

#Region "Constructors"
        Sub New(analyzer As IAnalyzerManager, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            MyBase.New(analyzer, myFlagsDelg)
            _currentStep = BaseLineStepsEnum.NotStarted
            SetAllInjectedDependencies()
        End Sub

        Sub New(analyzer As IAnalyzerManager, ByVal pauseWhenReadErrors As Boolean, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            Me.New(analyzer, myFlagsDelg)
            _pauseWhenReadErrors = pauseWhenReadErrors
            SetAllInjectedDependencies()
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' If this function is set to True, if the rotor is full of clean water that can be used to perform a FLIGHT, the process starts directly from the Read step.
        ''' This happens when a 551 status (alarm) is present and rotor contents hasn't lapsed.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DecideToReuseRotorContentsCallback As Action(Of ReuseRotorResponse) Implements IBaseLineService.DecideToReuseRotorContents

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CurrentStep As BaseLineStepsEnum Implements IBaseLineService.CurrentStep
            Get
                Return _currentStep
            End Get
            Set(ByVal value As BaseLineStepsEnum)
                _currentStep = value
            End Set
        End Property


        Public Overrides Property Status As ServiceStatusEnum
            Get
                Return MyBase.Status
            End Get
            Set(value As ServiceStatusEnum)
                MyBase.Status = value
                If Status = ServiceStatusEnum.EndSuccess Then UpdateAnalyzerSettings()
            End Set
        End Property
#End Region

#Region "Attributes"

        Private _forceEmptyAndFinalize As Boolean = False
        Private _checkedPreviousAlarms As Boolean = False
        Private _staticBaseLineFinished As Boolean = False
        Private _dynamicBaseLineValid As Boolean = False
        Private _currentStep As BaseLineStepsEnum
        Private _alreadyFinalized As Boolean = False
        Private _isInRecovering As Boolean = False
        Private _eventHandlersAdded As Boolean = False
        Private _pauseWhenReadErrors As Boolean = False

#End Region

#Region "Event Handlers"

        Private Sub OnReceivedStatusInformationEvent()
            If _currentStep = BaseLineStepsEnum.NotStarted Then Return
            TryToResume()
        End Sub

        Private Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags)
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

#Region "Public methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function StartService() As Boolean

            'Previous conditions: (no flow here)
            If _analyzer.Connected = False Then Return False
            If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then Return False

            'Method flow:
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            _analyzer.StopAnalyzerRinging()
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "")
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "")

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

            Status = ServiceStatusEnum.Running
            AddRequiredEventHandlers()

            Return True

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub PauseService()
            RemoveRequiredEventHandlers()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub RestartService()
            AddRequiredEventHandlers()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub EmptyAndFinalizeProcess() Implements IBaseLineService.EmptyAndFinalizeProcess
            _forceEmptyAndFinalize = True
            Status = ServiceStatusEnum.Running
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RecoverProcess() As Boolean Implements IBaseLineService.RecoverProcess
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
                GlobalBase.CreateLogActivity(ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RepeatDynamicBaseLineReadStep() Implements IBaseLineService.RepeatDynamicBaseLineReadStep

            _analyzer.DynamicBaselineInitializationFailures = 0

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

            RestartProcess()

            ExecuteDynamicBaseLineReadStep()

        End Sub

        ''' <summary>
        ''' This function returns TRUE if current ractions rotor is full of clean water that can be used to perform directly a FLIGHT read step.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CanRotorContentsByDirectlyRead() As Boolean

            'Calculate expiration time:
            Dim caducityMinutesString = GetGeneralSettingValue(GeneralSettingsEnum.FLIGHT_FULL_ROTOR_CADUCITY).SetDatos

            If caducityMinutesString Is Nothing OrElse caducityMinutesString = String.Empty Then caducityMinutesString = "0"

            Dim caducityMinutes = CInt(caducityMinutesString)
            Dim expirationTime = StatusParameters.LastSaved.AddMinutes(caducityMinutes)

            'Process flags:
            Return StatusParameters.State =
                StatusParameters.RotorStates.FBLD_ROTOR_FULL And
                StatusParameters.IsActive = True And
                Now < expirationTime
        End Function




#End Region

#Region "Private methods"

        Private Sub Initialize()
            _currentStep = BaseLineStepsEnum.None
            _forceEmptyAndFinalize = False
            _staticBaseLineFinished = False
            _dynamicBaseLineValid = False
            _analyzer.ResetFLIGHT()
            _analyzer.DynamicBaselineInitializationFailures = 0
            _analyzer.CurrentInstructionAction = InstructionActions.None
            _alreadyFinalized = False
            _checkedPreviousAlarms = False
            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If
            Status = ServiceStatusEnum.Running

        End Sub

        ''' <summary>
        ''' Set the flags into a stable value for repeat last action and recover the process
        ''' </summary>
        ''' <remarks>Created by:  AG 20/01/2015 BA-2216
        '''          Modified by: IT 16/02/2015 BA-2266
        '''  </remarks>
        Private Sub InitializeRecover()
            _checkedPreviousAlarms = False
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
                     BaseLineStepsEnum.CheckPreviousAlarms,
                     BaseLineStepsEnum.DynamicBaseLineRead,
                     BaseLineStepsEnum.Finalize

                    ValidateProcess()
            End Select

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As BaseLineStepsEnum
            Dim nextStep = BaseLineStepsEnum.None
            If Not _checkedPreviousAlarms Then
                nextStep = BaseLineStepsEnum.CheckPreviousAlarms

            Else
                If Status = ServiceStatusEnum.Running Then
                    If _alreadyFinalized Then Return BaseLineStepsEnum.None
                    nextStep = GetNextStepWhileInProcess(nextStep)


                ElseIf Status = ServiceStatusEnum.Paused Then

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

                End If

            End If
            Return nextStep



        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="nextStep"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
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

                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "END") And
                                ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END") Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED")) And
                                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then 'IT 30/01/2015 - BA-2358
                                If CurrentStep() <> BaseLineStepsEnum.CheckPreviousAlarms Then nextStep = BaseLineStepsEnum.Finalize
                            End If

                        Else
                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") OrElse
                               (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                                nextStep = BaseLineStepsEnum.Finalize
                            End If
                        End If

                    End If
            End Select
            Static lastdone As BaseLineStepsEnum = BaseLineStepsEnum.None
            If nextStep <> BaseLineStepsEnum.None AndAlso lastdone <> nextStep Then
                Debug.WriteLine("Currently doing: " & nextStep.ToString)
                lastdone = nextStep
            End If
            Return nextStep
        End Function

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
                    If (Not ExistsBottleAlarmsOrRotorIsMissing()) Then
                        RestartProcess()
                        ExecuteWashingStep()
                    Else
                        WashingStepPaused()
                    End If

                Case BaseLineStepsEnum.StaticBaseLine
                    If (Not _staticBaseLineFinished) Then
                        If (Not ExistsBottleAlarmsOrRotorIsMissing()) Then
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
                    If (Not ExistsBottleAlarmsOrRotorIsMissing()) Then
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
                        If (Not ExistsBottleAlarmsOrRotorIsMissing()) Then
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RestartProcess()

            AddRequiredEventHandlers()
            Status = ServiceStatusEnum.Running

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
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
            Dim resultData As GlobalDataTO = ReactRotorStatusSerializer.ChangeRotorPerformed(Nothing, _analyzer.ActiveAnalyzer)

            'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
            If Not resultData.HasError Then

                resultData = BaselineValuesDeleter.DeleteBLinesValues(Nothing, _analyzer.ActiveAnalyzer, _analyzer.ActiveWorkSession, "", _analyzer.BaseLineTypeForCalculations.ToString) 'AG 15/01/2015 BA-2212 inform new parameter BaseLineTypeForCalculations)
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

        Public ReactRotorCRUD As IAnalyzerSettingCRUD(Of AnalyzerReactionsRotorDS)
        Public ReactRotorStatusSerializer As IReactionsRotorStatusSerializer
        Public BaselineValuesDeleter As IWSBLinesDelegateValuesDeleter
        Public AnalyzerAlarmsManager As IAnalyzerAlarms


        'Public Shared Function Save(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAnalyzerSettings As AnalyzerSettingsDS, ByVal pSessionSettings As UserSettingDS) As GlobalDataTO
        Public AnalyzerSettingsSaver As Func(Of SqlClient.SqlConnection, String, AnalyzerSettingsDS, UserSettingDS, GlobalDataTO)

        Private Sub SetAllInjectedDependencies()

            If ReactRotorCRUD Is Nothing Then
                Dim instance = New AnalyzerReactionsRotorDelegate
                ReactRotorCRUD = instance
                ReactRotorStatusSerializer = instance
                BaselineValuesDeleter = New WSBLinesDelegate
                AnalyzerSettingsSaver = AddressOf AnalyzerSettingsDelegate.Save
                AnalyzerAlarmsManager = New AnalyzerAlarms(_analyzer)
            End If

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
            If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then
                If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") AndAlso Me._dynamicBaseLineValid Then
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
            UpdateFlags(myAnalyzerFlagsDs)

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

            If (Not _pauseWhenReadErrors And Not IsReadingAllowed()) Then
                Return True
            End If

            If (_dynamicBaseLineValid Or _forceEmptyAndFinalize) Then
                Return True
            End If

            Return False

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

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If
            Status = ServiceStatusEnum.Paused

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
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
            _checkedPreviousAlarms = True

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="disposing"></param>
        ''' <remarks></remarks>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            RemoveRequiredEventHandlers()
            MyBase.Dispose(disposing)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RemoveRequiredEventHandlers()
            If _eventHandlersAdded Then
                Try
                    RemoveHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                    RemoveHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent
                    _eventHandlersAdded = False
                Catch : End Try
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AddRequiredEventHandlers()
            If Not _eventHandlersAdded Then
                AddHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                AddHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent
                _eventHandlersAdded = True
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckPreviousAlarms()
            'Previous constraint:
            _checkedPreviousAlarms = True

            If _analyzer.Connected = False Then Return

            Dim alarm551Present = StatusParameters.IsActive And (StatusParameters.State = StatusParameters.RotorStates.FBLD_ROTOR_FULL)

            Dim alarm552Present = StatusParameters.IsActive And (StatusParameters.State = StatusParameters.RotorStates.UNKNOW_ROTOR_FULL)

            If alarm552Present Then
                ExecuteDynamicBaseLineEmptyStep()
            ElseIf alarm551Present Then
                ProcessAlarmFullCleanRotor()
            Else
                'No alarms to process. Do nothing
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ProcessAlarmFullCleanRotor()

            If CanRotorContentsByDirectlyRead() Then
                Dim result As New ReuseRotorResponse
                If DecideToReuseRotorContentsCallback IsNot Nothing Then DecideToReuseRotorContentsCallback.Invoke(result)
                If result.Reuse Then
                    DirectlyGoToDynamicReadStep()
                Else
                    ExecuteDynamicBaseLineEmptyStep()
                End If
            Else
                ExecuteDynamicBaseLineEmptyStep()
            End If

        End Sub

        ''' <summary>
        ''' Set DynamicBLDatetime setting in the DB, last datetime when the baselineservice was end sucessfully.
        ''' Function called from status property always when we set  BaselineService.status = ServiceStatusEnum.EndSuccess. 
        ''' </summary>
        ''' <remarks>Created by MR: BA 2601 - 16/06/2015</remarks>
        ''' 
        Public Sub UpdateAnalyzerSettings()
            Dim myGlobal As GlobalDataTO = Nothing
            Dim myAnalyzerSettingsDs As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

            Dim currentNow = Now

            'BLDATETIME Setting
            myAnalyzerSettingsRow = myAnalyzerSettingsDs.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = _analyzer.ActiveAnalyzer
                .SettingID = AnalyzerSettingsEnum.BL_DATETIME.ToString()

                .CurrentValue = currentNow.ToString(Globalization.CultureInfo.InvariantCulture)
            End With
            myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            UpdateAnalyzerSettingsDsCache(currentNow)
            myGlobal = AnalyzerSettingsSaver(Nothing, _analyzer.ActiveAnalyzer, myAnalyzerSettingsDs, Nothing)
            deleteAlarmBlExpired()


        End Sub

        ''' <summary>
        ''' Function to update the datetime in our dataset in cache of Analyzer object to avoid go to the DB to refresh the dates.
        ''' </summary>
        ''' <param name="newBLDate">Datetime of last baseline process.</param>
        ''' <remarks></remarks>
        Private Sub UpdateAnalyzerSettingsDsCache(newBLDate As DateTime)

            Dim dtrAnalyzerSettings = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In _analyzer.AnalyzerSettings.tcfgAnalyzerSettings
                                           Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString()).First()

            If dtrAnalyzerSettings IsNot Nothing Then

                If Not dtrAnalyzerSettings.IsCurrentValueNull() Then
                    dtrAnalyzerSettings.CurrentValue = newBLDate.ToString(Globalization.CultureInfo.InvariantCulture)
                    _analyzer.AnalyzerSettings.AcceptChanges()
                    _analyzer.IsBlExpired = False
                End If
            End If
        End Sub

        ''' <summary>
        ''' Function to delete the Base Line over date from alarms of the analyzer, once this one is solved.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub deleteAlarmBlExpired()

            AnalyzerAlarmsManager.ActionAlarm(False, AlarmEnumerates.Alarms.BASELINE_EXPIRED)

        End Sub

#End Region

        Public Class ReuseRotorResponse
            Public Property Reuse As Boolean = False
        End Class

    End Class

End Namespace

