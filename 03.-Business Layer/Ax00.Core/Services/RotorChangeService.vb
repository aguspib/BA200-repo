Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Core.Services.Interfaces
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Framework.Core


Namespace Biosystems.Ax00.Core.Services
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 19/12/2014 - BA-2143
    ''' Modified by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Public Class RotorChangeService
        Inherits AsyncService
        Implements IRotorChangeService

#Region "Constructors"
        Public Sub New(analyzer As IAnalyzerManager, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            Me.New(analyzer, Nothing, myFlagsDelg)
        End Sub

        Public Sub New(analyzer As IAnalyzerManager, warmUpService As IWarmUpService, myFlaDelg As IAnalyzerManagerFlagsDelegate)
            Me.New(analyzer, warmUpService, New BaseLineService(analyzer, True, myFlaDelg), myFlaDelg)
        End Sub

        Public Sub New(analyzer As IAnalyzerManager, warmUpService As IWarmUpService, baseLineService As IBaseLineService, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            MyBase.New(analyzer, myFlagsDelg)
            _baseLineService = baseLineService
            _warmUpService = warmUpService
            _baseLineService.OnServiceStatusChange = AddressOf BaseLineStatusChanged
        End Sub

#End Region

#Region "Attributes"

        'Private WithEvents _analyzer As IAnalyzerManager
        Private _currentStep As RotorChangeStepsEnum
        Private _isInRecovering As Boolean = False
        Private _eventHandlersAdded As Boolean = False

        Private _baseLineService As IBaseLineService
        Private _warmUpService As IWarmUpService
        Public Property CurrentStepForTest As RotorChangeStepsEnum

#End Region

#Region "Event Handlers"

        Public Sub OnReceivedStatusInformationEvent() Handles _analyzer.ReceivedStatusInformationEventHandler
            TryToResume()
        End Sub

        Public Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags) Handles _analyzer.ProcessFlagEventHandler

        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function StartService() As Boolean
            'Public Overloads Function StartService(ByVal isInRecovering As Boolean) As Boolean
            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            If _analyzer.ExistBottleAlarms Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NewRotor, StepStringStatus.Canceled)
                Return False
            Else
                'TR 28/10/2011 -Turn off Sound alarm
                _analyzer.StopAnalyzerRinging()
                If _analyzer.Connected Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NewRotor, StepStringStatus.Empty)

                    resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")

                    If resultData.HasError Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, StepStringStatus.Empty)
                        Throw New Exception(resultData.ErrorCode)
                    End If
                Else
                    Return False
                End If
            End If

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

            Return True

        End Function

        ' ''' <summary>
        ' ''' Starts the change rotor service, assuming it's not in recovery mode.
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Overrides Function StartService() As Boolean
        '    Return Me.StartService(False)
        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub PauseService()

            If (_eventHandlersAdded) Then
                RemoveHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                RemoveHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent

                _baseLineService.PauseService()

                _eventHandlersAdded = False
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub RestartService()

            If (Not _eventHandlersAdded) Then
                AddHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                AddHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent

                _baseLineService.RestartService()

                _eventHandlersAdded = True
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContinueProcess() As Boolean Implements IRotorChangeService.ContinueProcess
            If (_analyzer.Connected) Then 'AG 06/02/2012 - add AnalyzerController.Instance.Analyzer.Connected to the activation rule
                ValidateProcess()
            Else
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RepeatDynamicBaseLineReadStep() Implements IRotorChangeService.RepeatDynamicBaseLineReadStep
            _baseLineService.RepeatDynamicBaseLineReadStep()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub EmptyAndFinalizeProcess() Implements IRotorChangeService.EmptyAndFinalizeProcess
            _baseLineService.EmptyAndFinalizeProcess()
        End Sub


        ''' <summary>
        ''' Recovers the system to a stable point after close and start application during change rotor process 'in course'
        ''' </summary>
        ''' <returns>TRUE process recovered | FALSE process could not be recovered</returns>
        ''' <remarks>
        ''' Modified by:  AG 20/01/2015 - BA-2216
        ''' </remarks>
        Public Function RecoverProcess() As Boolean Implements IRotorChangeService.RecoverProcess
            Try
                _isInRecovering = True

                InitializeRecover()
                '_analyzer.CurrentInstructionAction = InstructionActions.None 'AG 04/02/2015 BA-2246 (informed in the event of USB disconnection AnalyzerManager.ProcessUSBCableDisconnection)

                Dim nextStep As RotorChangeStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case RotorChangeStepsEnum.NewRotor,
                        RotorChangeStepsEnum.Finalize
                        ValidateProcess()

                    Case Else
                        _baseLineService.RecoverProcess()
                End Select

                _isInRecovering = False

                Return True

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "RotorChangeServices.RecoverProcess", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

#End Region

#Region "Private Methods"

        Private Sub BaseLineStatusChanged(callback As IServiceStatusCallback)

            Static NeedToLog As Boolean = True

            Select Case callback.Sender.Status
                Case ServiceStatusEnum.Paused
                    NeedToLog = False
                    PauseProcess()

                Case ServiceStatusEnum.Running
                    'We register the start of rotor change process by log.
                    If NeedToLog Then StatisticsUpkeepDelegate.LogRotorChangeConsum(Me._analyzer.ActiveAnalyzer)
                    RestartProcess()

                Case ServiceStatusEnum.EndError, ServiceStatusEnum.EndSuccess
                    FinalizeProcess()
                    NeedToLog = True
            End Select
        End Sub

        Private Sub PauseProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

            _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub TryToResume()

            If (Not _isInRecovering) Then
                Dim nextStep As RotorChangeStepsEnum
                nextStep = GetNextStep()
                CurrentStepForTest = nextStep

                Select Case nextStep
                    Case RotorChangeStepsEnum.BaseLine
                        ValidateProcess()

                End Select
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateProcess()

            _currentStep = GetNextStep()

            Select Case _currentStep

                Case RotorChangeStepsEnum.NewRotor
                    ExecuteNewRotorStep()

                Case RotorChangeStepsEnum.BaseLine
                    ExecuteBaseLineStep()

                Case RotorChangeStepsEnum.Finalize
                    FinalizeProcess()

            End Select

            Return
        End Sub

        Private Sub ExecuteBaseLineStep()

            _baseLineService.StartService()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As RotorChangeStepsEnum

            Dim nextStep As RotorChangeStepsEnum

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
                nextStep = GetNextStepWhileInProcess()

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then
                nextStep = GetNextStepWhilePaused()
            Else
                nextStep = RotorChangeStepsEnum.None
            End If

            Return nextStep

        End Function

        Private Function GetNextStepWhilePaused() As RotorChangeStepsEnum
            Return RotorChangeStepsEnum.None
        End Function

        Private Function GetNextStepWhileInProcess() As RotorChangeStepsEnum

            Dim nextStep As RotorChangeStepsEnum = RotorChangeStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor)
                Case StepStringStatus.Empty, StepStringStatus.Initialized
                    nextStep = RotorChangeStepsEnum.NewRotor

                Case StepStringStatus.Ended
                    Select Case _baseLineService.Status
                        Case ServiceStatusEnum.NotYetStarted, ServiceStatusEnum.EndError
                            nextStep = RotorChangeStepsEnum.BaseLine
                        Case ServiceStatusEnum.EndSuccess
                            nextStep = RotorChangeStepsEnum.Finalize
                        Case ServiceStatusEnum.Running
                            nextStep = RotorChangeStepsEnum.None

                    End Select


            End Select

            Return nextStep

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FinalizeProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "CLOSED")

            ''Remove alarm change rotor recommend
            If _analyzer.Alarms.Contains(Alarms.BASELINE_WELL_WARN) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_WELL_WARN)
            End If

            _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PERFORMED, 1, True) 'Inform the FALSE sensor the new rotor process is finished for UI refresh

            'AG 26/03/2012 - Special case: maybe the user was starting the instrument and the process has been
            'paused because there is not reactions rotor ... in this case when a valid alight is received
            'Sw must inform the start instrument process is OK

            'BA-2288
            'If _analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
            '    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "INPROCESS")
            '    _analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDs, WarmUpProcessFlag.Finalize) 'BA-2075
            'End If
            If _analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
                If (Not _warmUpService Is Nothing) Then
                    _warmUpService.FinalizeProcess()
                End If
            End If

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RestartProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Initialize()

            _analyzer.DynamicBaselineInitializationFailures = 0
            _analyzer.CurrentInstructionAction = InstructionActions.None

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

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

            'NEWROTORprocess in INPROCESS status
            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = StepStringStatus.Initialized) Then
                _analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = StepStringStatus.Empty
            End If

            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' Sends instruction NROTOR again
        ''' </summary>
        ''' <remarks>AG 20/01/2015 BA-2216</remarks>
        Private Sub ExecuteNewRotorStep()

            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.NROTOR, True, Nothing, Nothing, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            If Not resultData.HasError Then
                _analyzer.SetSensorValue(AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once instruction has been sent clear sensor
            Else
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "")
                Throw New Exception(resultData.ErrorCode)
            End If

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

#End Region

    End Class

End Namespace