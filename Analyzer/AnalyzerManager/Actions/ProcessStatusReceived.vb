Imports System.Globalization
Imports System.Threading
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Namespace Biosystems.Ax00.Core.Entities
    Public Class ProcessStatusReceived

        Private _analyzerManager As IAnalyzerManager        
        Private ReadOnly _instructionReceived As List(Of InstructionParameterTO)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="analyzerMan"></param>
        ''' <param name="instructionReceived"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef analyzerMan As IAnalyzerManager, ByVal instructionReceived As List(Of InstructionParameterTO))
            _analyzerManager = analyzerMan
            _instructionReceived = instructionReceived
        End Sub

        ''' <summary>
        ''' Software has received a new state instruction
        ''' This function treats this instruction and decides next action to perform
        ''' 
        ''' Treats the Thermo values and evaluate if exists alarm or not - TO CONFIRM????
        ''' When errors then ask for Hw alarm details
        ''' 
        ''' </summary>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation: AG 16/04/2010
        ''' Modified by: RH 30/06/2010 - AnalyzerIsReady depends on myRequestValue.
        '''                            - Change "If myRequestValue = 1" by "If AnalyzerIsReady"
        '''              AG 20/06/2012 - after connection establishment Sw has to wait until analyzer becomes ready to receive new instructions (Action = Ready)
        '''              AG 19/11/2013 - call the method to process the last ANSPHR instruction received also in pause mode while barcode is scanning
        '''              AG 28/11/2013 - #1397 changes into connection process in running (they are required for the recovery results in pause mode: after 1st STATUS after connection call STATE + POLLSN)
        '''              AG 06/02/2014 - Fix issues numbers #1484
        '''              XB 03/04/2014 - Fix a malfunction when INFO;Q:3 was sent meanwhile a ISE operation was working or Abort process was not finished - task #1557
        '''              AG 15/04/2014 - #1594 paused in v300
        '''              XB 26/09/2014 - Implement Start Task Timeout for ISE commands - BA-1872
        '''              XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        '''              XB 06/11/2014 - Implement Comms Timeout for RUNNING instruction - BA-1872
        ''' </remarks>
        Public Function InstructionProcessing() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myInstParamTo As New InstructionParameterTO
            Dim startTime As DateTime = Now
            Dim myStatusValue As AnalyzerManagerStatus = AnalyzerManagerStatus.NONE

            Try

                If _analyzerManager.ISECMDStateIsLost() Or _analyzerManager.RunningLostState() Then
                    Debug.Print("Deactivate waiting time control ...")
                    _analyzerManager.NumRepetitionsStateInstruction = 0
                    _analyzerManager.InitializeTimerSTATEControl(WAITING_TIME_OFF)
                End If

                Dim myActionValue As AnalyzerManagerAx00Actions
                Dim myExpectedTime As Integer
                Dim myExpectedTimeRaw As Integer
                Dim myWellValue As Integer
                Dim myRequestValue As Integer
                Dim errorValue As Integer

                If Not GetFieldsFromInstructionRecived(myStatusValue, myGlobal, myInstParamTo, myActionValue, myExpectedTime, myExpectedTimeRaw, myWellValue, myRequestValue, errorValue) Then
                    Return myGlobal
                End If
                If StatusParameters.IsActive AndAlso errorValue <> 99 AndAlso errorValue <> 551 AndAlso errorValue <> 552 Then
                    StatusParameters.IsActive = False
                End If
                'Alarms management
                ManageErrorFieldAndStates(myGlobal, errorValue, myActionValue, myExpectedTimeRaw)

                If errorValue <> 0 Then
                    If errorValue = 551 Or errorValue = 552 Then
                        StateManagementAlarm(errorValue)
                    Else
                        TreatmentSingleAndMultipleErrors(myGlobal, errorValue)
                    End If
                Else
                    If GlobalBase.IsServiceAssembly Then
                        'Solve all previous alarms
                        If _analyzerManager.ErrorCodesDisplay.Count > 0 Then _analyzerManager.SolveErrorCodesToDisplay(New List(Of String)({"0"}))
                        _analyzerManager.MyErrorCodesClear()
                    End If

                    'Reset the freeze flags information
                    If _analyzerManager.AnalyzerIsFreeze() AndAlso Not GlobalBase.IsServiceAssembly Then
                        'Clear all alarms with error code
                        myGlobal = _analyzerManager.RemoveErrorCodeAlarms(Nothing, _analyzerManager.AnalyzerCurrentAction())
                    End If
                End If

                'Do business depending the requestvalue, action value, status value, alarms value,....
                DoActionsDependingFieldValues(myStatusValue, myGlobal, myActionValue, myExpectedTime, myWellValue, myRequestValue, errorValue, startTime)
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex)
            End Try

            Return myGlobal
        End Function

        Private Sub StateManagementAlarm(ByVal errorValue As Integer)
            Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)

            Dim errorTranslated = _analyzerManager.SimpleTranslateErrorCodeToAlarmId(Nothing, errorValue)

            If Not StatusParameters.IsActive Then
                'Don't have active alarms
                StatusParameters.IsActive = True
                StatusParameters.State = DirectCast(errorValue, StatusParameters.RotorStates)
                StatusParameters.LastSaved = DateTime.Now

                currentAlarms.AddNewAlarmState(errorTranslated.ToString())
                'Debug.WriteLine("Entro en StateManagementAlarm con errorcode:" + errorValue.ToString())

                'If exists some active AlarmState and recibe a 551 state -> do nothing (552 is priority and the first 551 is the valid state)
            ElseIf errorTranslated.Equals(Alarms.UNKNOW_ROTOR_FULL) Then
                StatusParameters.State = StatusParameters.RotorStates.UNKNOW_ROTOR_FULL
                StatusParameters.LastSaved = DateTime.Now

                currentAlarms.AddNewAlarmState(errorTranslated.ToString())
            End If

        End Sub

        Private Sub DoActionsDependingFieldValues(ByVal myStatusValue As AnalyzerManagerStatus, ByRef myGlobal As GlobalDataTO, ByVal myActionValue As AnalyzerManagerAx00Actions, ByVal myExpectedTime As Integer, ByVal myWellValue As Integer, ByVal myRequestValue As Integer, ByVal errorValue As Integer, ByVal startTime As Date)

            'If action say us CONNECTION_ESTABLISHMENT update internal flags
            If myActionValue = AnalyzerManagerAx00Actions.CONNECTION_DONE Then

                If GlobalBase.IsServiceAssembly AndAlso errorValue = 0 Then
                    _analyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.STATUS_RECEIVED
                    _analyzerManager.ConnectionDoneReceptionEvent()
                    If Not _analyzerManager.IsFwUpdateInProcess Then _analyzerManager.InfoRefreshFirstTime = True
                End If

                'When successfully inform the connection results
                _analyzerManager.Connected() = True
                _analyzerManager.PortName() = _analyzerManager.ConnectedPortName
                _analyzerManager.Bauds() = _analyzerManager.ConnectedBauds()

            End If

            If myActionValue = AnalyzerManagerAx00Actions.FLIGHT_ACTION_DONE AndAlso errorValue = 0 Then
                Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                If currentAlarms.ExistsActiveAlarm(Alarms.GLF_BOARD_FBLD_ERR.ToString()) Then currentAlarms.RemoveAlarmState(Alarms.GLF_BOARD_FBLD_ERR.ToString())
                _analyzerManager.CanSendingRepetitions() = False
                _analyzerManager.NumSendingRepetitionsTimeout() = 0
            End If

            'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
            If Not GlobalBase.IsServiceAssembly Then 'NOT for SERVICE SOFTWARE 01/07/2011
                Select Case myStatusValue
                    Case AnalyzerManagerStatus.SLEEPING
                        Dim resetFlags As Boolean = True
                        If Not _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState AndAlso _
                           Not _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                            myGlobal = ManageSleepStatus(myActionValue)
                        End If

                        If myActionValue = AnalyzerManagerAx00Actions.STANDBY_START OrElse myActionValue = AnalyzerManagerAx00Actions.SLEEP_END _
                           OrElse _analyzerManager.SessionFlag(AnalyzerManagerFlags.WaitForAnalyzerReady) = "INI" Then
                            resetFlags = False
                        End If

                        If resetFlags AndAlso Not myGlobal.HasError Then
                            'reset internal flags when analyzer is sleeping and no action has been performed
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.ResetFlags(Nothing, _analyzerManager.ActiveAnalyzer())
                            _analyzerManager.InitializeAnalyzerFlags(Nothing)
                        End If

                    Case AnalyzerManagerStatus.STANDBY
                        If Not _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState AndAlso _
                           Not _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'AG 14/06/2012 - Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                            myGlobal = _analyzerManager.ManageStandByStatus(myActionValue, myWellValue)
                        End If

                    Case AnalyzerManagerStatus.RUNNING
                        If Not _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState AndAlso _
                           Not _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'Do not manage instruction by analyzer status if connection process is in course (RUNNING)
                            myGlobal = _analyzerManager.ManageRunningStatus(myActionValue, myWellValue)

                            If myRequestValue = 1 Then startTime = Now

                        End If
                End Select


                '(status <> RUNNING) After connection establishment Sw has to wait until BAx00 is ready to receive the remaining instructions of the connection process
                '(status = RUNNING) Sound OFF + no more changes (until spec how to know the analyzerID)
                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE AndAlso _
                   _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState Then

                    'Different connect business when analyzer status <> Running and when is running
                    If _analyzerManager.AnalyzerStatus() <> AnalyzerManagerStatus.RUNNING Then
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ReportSATonRUNNING, "CLOSED")

                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
                        myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True) 'Ask for status (and waits unitl analyzer becomes ready)
                    Else
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                        'when Connecting on RUNNING Session we must ask for Serial Number (POLLSN) 

                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RESULTSRECOVERProcess, InProcessState)
                        _analyzerManager.EndRunInstructionSent() = True 'AG 25/09/2012 - When resultsrecover process starts the Fw implements an automatic END, so add protection in order Sw do not sent this instruction again

                        'in running Sw has to know if analyzer is in normal or paused running before call POLLSN so we has to send STATE instruction
                        myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True) 'Send STATE instruction

                    End If

                    If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                        _analyzerManager.SetAnalyzerNotReady()
                    End If

                ElseIf _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState AndAlso _
                       _analyzerManager.SessionFlag(AnalyzerManagerFlags.WaitForAnalyzerReady) = "INI" Then

                    'SGM 21/06/2012 - Protection!! INFO will be activated later, on POLLFW answer reception
                    If myStatusValue = AnalyzerManagerStatus.STANDBY Then
                        myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STP)
                        Thread.Sleep(1000)
                    End If

                    If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.NO_ACTION Then
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                        'Continue with the connection process instructions flow
                        If myStatusValue <> AnalyzerManagerStatus.RUNNING Then
                            myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.POLLFW, True, Nothing, POLL_IDs.CPU)
                        End If

                    Else
                        If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT
                        _analyzerManager.InitializeTimerControl(myExpectedTime)
                        _analyzerManager.MaxWaitTime() = myExpectedTime + SYSTEM_TIME_OFFSET
                    End If

                ElseIf _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONFIG_DONE AndAlso _
                       _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState Then
                    'Send SOUND OFF instruction
                    myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.ENDSOUND, True)
                    If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                        _analyzerManager.SetAnalyzerNotReady()
                    End If

                ElseIf _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.SOUND_DONE AndAlso _
                       _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState Then
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "CLOSED")
                    _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(_analyzerManager.Connected(), 1, 0)), True) 'Inform connection finished OK

                    'when recovery results finished, Sw call the processConnection method for execute the full connection
                    If _analyzerManager.SessionFlag(AnalyzerManagerFlags.RESULTSRECOVERProcess) = InProcessState AndAlso _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY Then
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")
                        _analyzerManager.RecoveryResultsInPause() = False
                        _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.RECOVERY_RESULTS_STATUS, 0, True) 'Generate UI refresh for presentation - Inform the recovery results has finished!!
                        _analyzerManager.ManageStandByStatus(AnalyzerManagerAx00Actions.STANDBY_END, _analyzerManager.CurrentWell()) 'Call this method with this action code to update ISE consumption if needed

                        'We have to do it now because the normal connection finishes is when action SOUND_DONE is received
                        If _analyzerManager.AnalyzerIsFreeze() Then
                            myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.SOUND, True)
                            If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                                _analyzerManager.SetAnalyzerNotReady()
                            End If
                        End If

                    End If

                    'If no ISE installed LOCK all pending executions in current worksession/analyzer
                    If Not myGlobal.HasError Then
                        Dim adjustValue As String
                        adjustValue = _analyzerManager.ReadAdjustValue(Ax00Adjustsments.ISEINS)
                        If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                            Dim iseInstalledFlag = CType(adjustValue, Boolean)
                            If Not iseInstalledFlag Then
                                Dim myExecutions As New ExecutionsDelegate
                                myGlobal = myExecutions.UpdateStatusByExecutionTypeAndStatus(Nothing, _analyzerManager.ActiveWorkSession(), _analyzerManager.ActiveAnalyzer(), "PREP_ISE", "PENDING", "LOCKED")
                            End If
                        End If
                    End If

                    If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY Then
                        _analyzerManager.InfoActivated() = 0
                        myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                    End If

                    myGlobal = _analyzerManager.ManageInterruptedProcess(Nothing) 'Evaluate value for FLAGS and determine the next action to be sent in order to achieve a stable setup

                ElseIf _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState AndAlso Not _analyzerManager.RunningConnectionPollSnSentStatus() AndAlso _
                       _analyzerManager.SessionFlag(AnalyzerManagerFlags.RESULTSRECOVERProcess) = InProcessState AndAlso _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                    myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.POLLSN, True) 'Send POLLSN instruction

                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

            End If


            If myStatusValue = AnalyzerManagerStatus.SLEEPING Then

                If _analyzerManager.ISEAnalyzer IsNot Nothing AndAlso _analyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                    _analyzerManager.ISEAnalyzer.IsISEInitiatedDone = False
                    _analyzerManager.IseIsAlreadyStarted = False
                End If

                If _analyzerManager.IsShutDownRequested() Then
                    _analyzerManager.Connected() = False
                    _analyzerManager.InfoRefreshFirstTime = True
                    _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(_analyzerManager.Connected(), 1, 0)), True)
                End If
            End If

            'AG 28/06/2012 - Launch the parallel threat when Running and Request received and preparation instruction is already sent
            'Also during running initialization

            If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                SyncLock LockThis
                    If _analyzerManager.BufferANSPHRReceivedCount() > 0 AndAlso Not _analyzerManager.ProcessingLastANSPHRInstructionStatus() Then

                        Dim startDoWorker As Boolean = False 'Evaluate if first ANSPHR in queue can be processed or not
                        If Not _analyzerManager.EndRunInstructionSent() AndAlso Not _analyzerManager.AbortInstructionSent() AndAlso Not _analyzerManager.PauseInstructionSent() Then
                            Select Case _analyzerManager.AnalyzerCurrentAction
                                Case AnalyzerManagerAx00Actions.TEST_PREPARATION_RECEIVED, AnalyzerManagerAx00Actions.PREDILUTED_TEST_RECEIVED, _
                                    AnalyzerManagerAx00Actions.ISE_TEST_RECEIVED, AnalyzerManagerAx00Actions.SKIP_START, _
                                    AnalyzerManagerAx00Actions.WASHING_RUN_START, AnalyzerManagerAx00Actions.START_INSTRUCTION_START, _
                                    AnalyzerManagerAx00Actions.START_INSTRUCTION_END, AnalyzerManagerAx00Actions.PAUSE_START, _
                                    AnalyzerManagerAx00Actions.PAUSE_END, AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED

                                    startDoWorker = True

                                    'AG 19/11/2013 - This case is not possible but we add the protection
                                    If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED AndAlso Not _analyzerManager.AllowScanInRunning Then
                                        startDoWorker = False
                                    End If

                                    'Special cases: ise test and prediluted test appears several cycles (when dummies are performed)
                                Case AnalyzerManagerAx00Actions.ISE_TEST_END, AnalyzerManagerAx00Actions.PREDILUTED_TEST_END
                                    'Activate startDoWorker to process readings only when biochemical Request = 0
                                    If myRequestValue = 0 Then 'AG 14/09/2012 v052 - If Not AnalyzerManager.AnalyzerIsReady() Then
                                        startDoWorker = True
                                    Else
                                        'When request = 1 the Software the priority is ask the request and send next preparation The readings will treated when preparation will be accepted 
                                    End If

                                    'SOUND_DONE has not to be taken into account Fw answers with status just receive it (exception of running instructions timming)
                                Case AnalyzerManagerAx00Actions.SOUND_DONE
                                    startDoWorker = True
                            End Select

                            'When a instruction is rejected (out of time or repeated in cycle)
                            If Not startDoWorker AndAlso (errorValue = 28 OrElse errorValue = 34) Then
                                startDoWorker = True
                            End If

                        Else 'Leaving RUNNING
                            startDoWorker = True
                        End If

                        If startDoWorker Then
                            'check if create WS executions semaphore is busy or ready If busy we cannot process readings now, so we will evaluate readings next BAx00 cycle
                            Dim semaphoreFree As Boolean = True
                            If GlobalConstants.CreateWSExecutionsWithSemaphore Then
                                semaphoreFree = CBool(GlobalSemaphores.createWSExecutionsQueue = 0)
                            End If
                            If semaphoreFree Then
                                _analyzerManager.ProcessingLastANSPHRInstructionStatus() = True
                                _analyzerManager.RunWellBaseLineWorker()
                            Else
                                GlobalBase.CreateLogActivity("CreateWSExecutions semaphore busy. Don't process ANSPHR this cycle!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                            End If
                        End If
                    End If
                End SyncLock
            End If

            If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                GlobalBase.CreateLogActivity("Treat STATUS received: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
            End If
        End Sub

        Private Function GetFieldsFromInstructionRecived(ByRef myStatusValue As AnalyzerManagerStatus, ByRef myGlobal As GlobalDataTO, ByVal myInstParamTo As InstructionParameterTO, ByRef myActionValue As AnalyzerManagerAx00Actions, _
                                                         ByRef myExpectedTime As Integer, ByRef myExpectedTimeRaw As Integer, ByRef myWellValue As Integer, ByRef myRequestValue As Integer, ByRef errorValue As Integer) As Boolean

            'Get State field (parameter index 3)
            If Not GetStateField(_instructionReceived, myStatusValue, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get Action field (parameter index 4)
            If Not GetActionField(_instructionReceived, myActionValue, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get Time field (parameter index 5)
            If Not GetTimeField(_instructionReceived, myExpectedTime, myExpectedTimeRaw, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get Well field (parameter index 7)
            If Not GetWellField(_instructionReceived, myWellValue, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get Request field (parameter index 8)
            If Not GetRequestField(_instructionReceived, myStatusValue, myActionValue, myExpectedTime, myRequestValue, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get Error field (parameter index 9)
            If Not GetErrorField(_instructionReceived, errorValue, myGlobal, myInstParamTo) Then
                Return False
            End If

            ' Get ISE field (parameter index 10) also ISEModuleIsReadyAttribute is updated using the Fw information send
            If Not GetIseField(_instructionReceived, myGlobal, myInstParamTo) Then
                Return False
            End If

            Return True
        End Function


#Region "RefactMethods"
        ''' <summary>
        ''' Evaluate the Action code value (in all analyzer status)
        ''' </summary>
        ''' <param name="myInstParamTo"></param>
        ''' <param name="myGlobal"></param>
        ''' <remarks></remarks>
        Public Sub EvaluateActionCodeValue(ByRef myActionValue As AnalyzerManagerAx00Actions, myInstParamTo As InstructionParameterTO, ByRef myGlobal As GlobalDataTO)
            myActionValue = DirectCast(CInt(myInstParamTo.ParameterValue), AnalyzerManagerAx00Actions)
            _analyzerManager.AnalyzerCurrentAction = myActionValue

            Select Case myActionValue

                Case AnalyzerManagerAx00Actions.SOUND_DONE
                    'Change the status of AnalyzerManager.Ringing() (TRUE --> FALSE, FALSE --> TRUE) Except when connection ... always FALSE in this case
                    If _analyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = InProcessState Then
                        _analyzerManager.Ringing() = False
                    Else
                        _analyzerManager.Ringing() = Not _analyzerManager.Ringing()
                    End If
                    _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ANALYZER_SOUND_CHANGED, CSng(IIf(_analyzerManager.Ringing(), 1, 0)), True)

                Case AnalyzerManagerAx00Actions.END_RUN_START
                    _analyzerManager.EndRunInstructionSent() = True

                Case AnalyzerManagerAx00Actions.END_RUN_END
                    _analyzerManager.EndRunInstructionSent() = False

                Case AnalyzerManagerAx00Actions.ABORT_START
                    _analyzerManager.AbortInstructionSent() = True
                    _analyzerManager.EndRunInstructionSent() = True

                Case AnalyzerManagerAx00Actions.ABORT_END
                    _analyzerManager.PauseInstructionSent() = False

                    _analyzerManager.AbortInstructionSent() = False
                    _analyzerManager.EndRunInstructionSent() = False

                Case AnalyzerManagerAx00Actions.PAUSE_START
                    If String.Compare(_analyzerManager.SessionFlag(AnalyzerManagerFlags.PAUSEprocess), "", False) <> 0 Then
                        _analyzerManager.PauseInstructionSent() = True
                    End If

                Case AnalyzerManagerAx00Actions.PAUSE_END
                    _analyzerManager.PauseInstructionSent() = False

                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START
                    _analyzerManager.RecoverInstructionSent() = True

                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END
                    _analyzerManager.RecoverInstructionSent() = False
                    myGlobal = _analyzerManager.RemoveErrorCodeAlarms(Nothing, myActionValue)

                    If GlobalBase.IsServiceAssembly Then
                        _analyzerManager.InfoRefreshFirstTime = True
                    End If

                    _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished

            End Select
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myInstParamTo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function EvaluateAnalyzerState(ByVal myInstParamTo As InstructionParameterTO) As AnalyzerManagerStatus
            Dim myStatusValue = DirectCast(CInt(myInstParamTo.ParameterValue), AnalyzerManagerStatus)

            If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING And myStatusValue = AnalyzerManagerStatus.STANDBY Then
                _analyzerManager.ExecuteSpecialBusinessOnAnalyzerStatusChanges(Nothing, myStatusValue)
            End If

            If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then GlobalConstants.AnalyzerIsRunningFlag = True Else GlobalConstants.AnalyzerIsRunningFlag = False

            If _analyzerManager.AnalyzerStatus() <> myStatusValue Then
                _analyzerManager.AnalyzerStatus() = myStatusValue
                _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ANALYZER_STATUS_CHANGED, 1, True) 'Prepare UI refresh event when analyzer status changes
            End If
            Return myStatusValue
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <param name="myGlobal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetIseField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myGlobal = GetItemByParameterIndex(pInstructionReceived, 10)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                Dim iseAvailableValue = CInt(myInstParamTo.ParameterValue)
                _analyzerManager.ISEModuleIsReady() = CType(IIf(iseAvailableValue = 1, 1, 0), Boolean)
            Else
                Return False
            End If

            'SGM 29/10/2012 - SERVICE: reset E:20 flag            
            If GlobalBase.IsServiceAssembly Then
                _analyzerManager.IsInstructionRejected() = False
                _analyzerManager.IsRecoverFailed() = False
                _analyzerManager.IsInstructionAborted() = False
            End If
            Return True
        End Function

        Private Function GetErrorField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef errorValue As Integer, _
                                       ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO) As Boolean

            errorValue = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 9)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                errorValue = CInt(myInstParamTo.ParameterValue)
            Else
                Return False
            End If

            Return True
        End Function

        Private Function GetRequestField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByVal myStatusValue As AnalyzerManagerStatus, _
                                         ByVal myActionValue As AnalyzerManagerAx00Actions, ByRef myExpectedTime As Integer, ByRef myRequestValue As Integer, _
                                         ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myRequestValue = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 8)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                myRequestValue = CInt(myInstParamTO.ParameterValue)
            Else
                Return False
            End If

            'AG 25/10/2010 - Depending the status and action use Request or not
            _analyzerManager.StartUseRequestFlag() = False
            If myStatusValue = AnalyzerManagerStatus.RUNNING Then
                If myActionValue = AnalyzerManagerAx00Actions.START_INSTRUCTION_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.TEST_PREPARATION_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.PREDILUTED_TEST_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.ISE_TEST_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.SKIP_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.WASHING_RUN_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.LOADADJ_END Then

                    _analyzerManager.StartUseRequestFlag() = True
                End If
            End If

            If Not _analyzerManager.StartUseRequestFlag() Then
                _analyzerManager.AnalyzerIsReady() = (myExpectedTime = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
            Else
                _analyzerManager.AnalyzerIsReady() = (myRequestValue = 1) '1 Ax00 is ready to work, 0 Ax00 is busy (for use only in RUNNING mode)
            End If

            'AG 03/09/2012 - If connection analyzer always ready to receive new instructions
            If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then
                _analyzerManager.AnalyzerIsReady() = True
            End If

            If Not _analyzerManager.AnalyzerIsReady() Then
                If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT

                'AG 13/02/2012 - In Running activate always the waiting time
            ElseIf _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                'AG 21/03/2012 - exception when the abort instruction has started. In this case take into account the TIME received
                If myActionValue <> AnalyzerManagerAx00Actions.ABORT_START Then
                    myExpectedTime = WAITING_TIME_DEFAULT
                End If

            End If
            _analyzerManager.InitializeTimerControl(myExpectedTime)
            _analyzerManager.MaxWaitTime() = myExpectedTime + SYSTEM_TIME_OFFSET

            Return True
        End Function

        Private Function GetWellField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myWellValue As Integer, ByRef myGlobal As GlobalDataTO, _
                                      ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myWellValue = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 7)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                myWellValue = CInt(myInstParamTO.ParameterValue)
                _analyzerManager.CurrentWell() = myWellValue 'Inform the class attribute
            Else
                Return False
            End If

            Return True
        End Function

        Private Function GetTimeField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myExpectedTime As Integer, _
                                      ByRef myExpectedTimeRaw As Integer, ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myExpectedTime = WAITING_TIME_OFF
            'If no request .... get the estimated time and activate the waiting timer
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 5)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                myExpectedTime = CInt(myInstParamTO.ParameterValue)
            Else
                Return False
            End If

            myExpectedTimeRaw = myExpectedTime

            Return True
        End Function

        Private Function GetActionField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myActionValue As AnalyzerManagerAx00Actions, ByRef myGlobal As GlobalDataTO, _
                                        ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myActionValue = AnalyzerManagerAx00Actions.NO_ACTION
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 4)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                EvaluateActionCodeValue(myActionValue, myInstParamTO, myGlobal)

                If GlobalBase.IsServiceAssembly Then
                    If myActionValue = AnalyzerManagerAx00Actions.COMMAND_START Or _
                       myActionValue = AnalyzerManagerAx00Actions.COMMAND_END Then

                        _analyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                        _analyzerManager.ClearStartTaskQueueToSend()

                    End If

                End If
            End If
            Return True
        End Function

        Private Function GetStateField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myStatusValue As AnalyzerManagerStatus, ByRef myGlobal As GlobalDataTO, _
                                       ByRef myInstParamTo As InstructionParameterTO) As Boolean

            myGlobal = GetItemByParameterIndex(pInstructionReceived, 3)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                myStatusValue = EvaluateAnalyzerState(myInstParamTO)
            End If

            Return True
        End Function

        Private Sub ManageErrorFieldAndStates(ByRef myGlobal As GlobalDataTO, ByVal errorValue As Integer, ByRef myActionValue As AnalyzerManagerAx00Actions, ByVal myExpectedTimeRaw As Integer)
            If errorValue = 560 Then
                _analyzerManager.CanSendingRepetitions() = True
                _analyzerManager.NumSendingRepetitionsTimeout() += 1

                If _analyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsRetry Then
                    GlobalBase.CreateLogActivity("FLIGHT Error: GLF_BOARD_FBLD_ERR", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                    _analyzerManager.CanSendingRepetitions() = False

                    ' Activates Alarm begin
                    _analyzerManager.CanManageRetryAlarm = True

                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)
                    _analyzerManager.PrepareLocalAlarmList(Alarms.GLF_BOARD_FBLD_ERR, True, myAlarmList, myAlarmStatusList)
                Else
                    GlobalBase.CreateLogActivity("Repeat Start Task Instruction [" & _analyzerManager.NumSendingRepetitionsTimeout().ToString & "] because GLF_BOARD_FBLD_ERR", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                    myGlobal = _analyzerManager.SendStartTaskinQueue()

                End If

            ElseIf errorValue <> 61 Then
                If _analyzerManager.ISECMDStateIsLost() Then
                    _analyzerManager.ISECMDStateIsLost() = False

                    If _analyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.ISE_ACTION_START Then
                        _analyzerManager.CanSendingRepetitions() = True
                        _analyzerManager.NumSendingRepetitionsTimeout() += 1
                        If _analyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                            GlobalBase.CreateLogActivity("Num of Repetitions for Start Tasks timeout excedeed !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                            _analyzerManager.WaitingStartTaskTimerEnabled() = False
                            _analyzerManager.CanSendingRepetitions() = False

                            ' Activates Alarm begin
                            Dim alarmId As Alarms
                            Dim alarmStatus As Boolean
                            Dim myAlarmList As New List(Of Alarms)
                            Dim myAlarmStatusList As New List(Of Boolean)

                            alarmId = Alarms.ISE_TIMEOUT_ERR
                            alarmStatus = True
                            _analyzerManager.ISEAnalyzer.IsTimeOut = True

                            _analyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                            If myAlarmList.Count > 0 Then
                                ' Note that this alarm is common on User and Service !
                                Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                            End If
                            ' Activates Alarm end

                            _analyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                        Else
                            ' Instruction has not started by Fw, so is need to send it again
                            GlobalBase.CreateLogActivity("Repeat Start Task Instruction [" & _analyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                            myGlobal = _analyzerManager.SendStartTaskinQueue()
                        End If
                    End If

                End If

                If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.ISE_ACTION_START Then
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Action Start =34")

                    ' Update the interval of the Timer with the expected time received from the Analyzer
                    If myExpectedTimeRaw <= 0 Then
                        If Not _analyzerManager.SetTimeISEOffsetFirstTime() Then
                            _analyzerManager.SetTimeISEOffsetFirstTime() = True
                            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & WAITING_TIME_ISE_OFFSET.ToString & "] seconds")
                            _analyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_ISE_OFFSET)
                        End If
                    Else
                        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & _analyzerManager.MaxWaitTime().ToString & "] seconds")
                        _analyzerManager.InitializeTimerStartTaskControl(_analyzerManager.MaxWaitTime())
                    End If
                End If

            End If

            If _analyzerManager.RunningLostState() Then
                _analyzerManager.RunningLostState() = False

                If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                    If (_analyzerManager.SessionFlag(AnalyzerManagerFlags.RUNNINGprocess) = InProcessState) AndAlso _
                       (_analyzerManager.SessionFlag(AnalyzerManagerFlags.EnterRunning) = "INI") Then
                        myActionValue = AnalyzerManagerAx00Actions.RUNNING_END
                        _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_END
                    End If
                End If

                If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY AndAlso _
                   _analyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_START AndAlso _
                   _analyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_END Then
                    _analyzerManager.CanSendingRepetitions() = True
                    _analyzerManager.NumSendingRepetitionsTimeout() += 1
                    If _analyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                        GlobalBase.CreateLogActivity("Num of Repetitions for RUNNING excedeed !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                        _analyzerManager.WaitingStartTaskTimerEnabled() = False
                        _analyzerManager.CanSendingRepetitions() = False

                        ' Activates Alarm begin
                        Dim alarmId As Alarms
                        Dim alarmStatus As Boolean
                        Dim myAlarmList As New List(Of Alarms)
                        Dim myAlarmStatusList As New List(Of Boolean)

                        alarmId = Alarms.COMMS_TIMEOUT_ERR
                        alarmStatus = True
                        _analyzerManager.ISEAnalyzer.IsTimeOut = True

                        _analyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                        If myAlarmList.Count > 0 Then
                            ' Note that this alarm is common on User and Service !
                            Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                        End If
                        ' Activates Alarm end

                        Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")

                        'Update internal flags. Basically used by the running normal business
                        If (Not myGlobal.HasError AndAlso _analyzerManager.Connected()) Then
                            'Update analyzer session flags into DataBase
                            If (myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                            End If
                        End If

                        _analyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                    Else
                        ' Instruction has not started by Fw, so is need to send it again
                        GlobalBase.CreateLogActivity("Repeat RUNNING Instruction [" & _analyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                        myGlobal = _analyzerManager.SendStartTaskinQueue()
                    End If
                End If

            End If

            If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_START Then
                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - RUNNING Action Start =7 UPDATED TIME TO [" & _analyzerManager.MaxWaitTime().ToString & "] seconds")
                ' Update the interval of the Timer with the expected time received from the Analyzer
                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & _analyzerManager.MaxWaitTime().ToString & "] seconds")
                _analyzerManager.InitializeTimerControl(WAITING_TIME_OFF)    ' This timer is disabled because this operation is managed by StartTaskTimer
                _analyzerManager.InitializeTimerStartTaskControl(_analyzerManager.MaxWaitTime())
                _analyzerManager.StartingRunningFirstTime = True
            End If

            If _analyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_END Or _
               _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                If _analyzerManager.StartingRunningFirstTime Then
                    _analyzerManager.StartingRunningFirstTime = False
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - RUNNING Action END =8")
                    _analyzerManager.RunningLostState() = False
                    _analyzerManager.CanSendingRepetitions() = False
                    _analyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                    _analyzerManager.ClearStartTaskQueueToSend()

                    ' Deactivates Alarm begin - BA-1872
                    Dim alarmId As Alarms
                    Dim alarmStatus As Boolean
                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)

                    alarmId = Alarms.COMMS_TIMEOUT_ERR
                    alarmStatus = False

                    _analyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        If Not GlobalBase.IsServiceAssembly Then
                            Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                        End If

                    End If
                    If _analyzerManager.Alarms().Contains(Alarms.COMMS_TIMEOUT_ERR) Then _analyzerManager.AlarmListRemoveItem(Alarms.COMMS_TIMEOUT_ERR)
                End If
            End If
        End Sub

        Private Sub TreatmentSingleAndMultipleErrors(ByRef myGlobal As GlobalDataTO, ByVal errorValue As Integer)



            If Not _analyzerManager.IgnoreErrorCodes(_analyzerManager.InstructionTypeSent(), _analyzerManager.InstructionSent(), errorValue) Then
                If errorValue = MULTIPLE_ERROR_CODE Then 'If multiple alarm error code ask for details

                    myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.ALR)

                    'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                    If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                        _analyzerManager.SetAnalyzerNotReady()
                    End If

                Else 'If single error code then treat it!!

                    Dim myAlarmsReceivedList As New List(Of Alarms)
                    Dim myAlarmsStatusList As New List(Of Boolean)
                    Dim myAlarmsAdditionalInfoList As New List(Of String)
                    Dim myErrorCode As New List(Of Integer)
                    Dim myFwCodeErrorReceivedList As New List(Of String)

                    myErrorCode.Add(errorValue)

                    'Translation method
                    Dim myAlarms = _analyzerManager.TranslateErrorCodeToAlarmID(Nothing, myErrorCode)

                    ' SERVICE ROTOR MISSING
                    If GlobalBase.IsServiceAssembly Then
                        If Not myAlarms.Contains(Alarms.REACT_MISSING_ERR) Then
                            _analyzerManager.IsServiceRotorMissingInformed() = False
                        End If
                    End If

                    ' ISE TIMEOUT
                    If myAlarms.Contains(Alarms.ISE_TIMEOUT_ERR) Then
                        IseTimeoutErrorTreatment(myGlobal, myAlarms)
                    End If

                    myAlarmsReceivedList = PrepareLocalAlarms(myErrorCode, myAlarms, myAlarmsReceivedList, myAlarmsStatusList, myAlarmsAdditionalInfoList)

                    If GlobalBase.IsServiceAssembly Then
                        ' Initialize Error Codes List
                        _analyzerManager.MyErrorCodesClear()
                        ' Prepare error codes List received from Analyzer
                        _analyzerManager.PrepareLocalAlarmList_SRV(myErrorCode, myFwCodeErrorReceivedList)
                    End If

                    If myAlarmsReceivedList.Count > 0 Then
                        '3- Finally call manage all alarms detected (new or fixed)
                        If GlobalBase.IsServiceAssembly Then
                            myGlobal = _analyzerManager.ManageAlarms_SRV(Nothing, myAlarmsReceivedList, myAlarmsStatusList, myFwCodeErrorReceivedList)
                        Else
                            Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                            myGlobal = currentAlarms.Manage(myAlarmsReceivedList, myAlarmsStatusList, myAlarmsAdditionalInfoList)
                        End If

                    Else 'if not new alarms sure the ansinfo instruction is activated
                        If _analyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY AndAlso _analyzerManager.SessionFlag(AnalyzerManagerFlags.RUNNINGprocess) <> InProcessState Then

                            Dim updateIseConsumptionFlag As Boolean = False
                            ' Estimated ISE Consumption by Firmware during WS
                            If Not _analyzerManager.ISEAnalyzer Is Nothing _
                               AndAlso _analyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                                _analyzerManager.ISEAnalyzer.EstimatedFWConsumptionWS()
                                ' Update ISE consumptions if required
                                If _analyzerManager.ISEAnalyzer.IsCalAUpdateRequired Or _analyzerManager.ISEAnalyzer.IsCalBUpdateRequired Then
                                    updateIseConsumptionFlag = True
                                End If
                            End If

                            'AG 12/04/2012 - New Fw disables info when analyzer leaves running, so Sw has to activate info when standby end
                            If Not updateIseConsumptionFlag AndAlso _analyzerManager.SessionFlag(AnalyzerManagerFlags.ABORTprocess) <> InProcessState Then

                                myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                                'AG 04/04/2012 - When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then _analyzerManager.SetAnalyzerNotReady()
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Function PrepareLocalAlarms(ByVal myErrorCode As List(Of Integer), ByVal myAlarms As List(Of Alarms), ByVal myAlarmsReceivedList As List(Of Alarms), ByRef myAlarmsStatusList As List(Of Boolean), ByRef myAlarmsAdditionalInfoList As List(Of String)) As List(Of Alarms)

            Dim index As Integer = 0
            Dim errorCodeId As String

            For Each alarmId As Alarms In myAlarms
                errorCodeId = ""
                If index <= myErrorCode.Count - 1 Then
                    errorCodeId = myErrorCode(index).ToString
                End If
                _analyzerManager.PrepareLocalAlarmList(alarmId, True, myAlarmsReceivedList, myAlarmsStatusList, errorCodeId, myAlarmsAdditionalInfoList, True)
                index += 1
            Next
            Return myAlarmsReceivedList
        End Function

        Private Sub IseTimeoutErrorTreatment(ByRef myGlobal As GlobalDataTO, ByVal myAlarms As List(Of Alarms))

            If _analyzerManager.ISEAnalyzer IsNot Nothing Then
                If Not _analyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                    ' If ISE module isn't Installed remove the ISE Timeout Alarm
                    Debug.Print("ISE Module NOT installed !")
                    myAlarms.Remove(Alarms.ISE_TIMEOUT_ERR)

                    If GlobalBase.IsServiceAssembly Then
                        ' Only Sw Service
                        If Not myAlarms.Contains(Alarms.ISE_OFF_ERR) Then
                            myAlarms.Add(Alarms.ISE_OFF_ERR)
                            _analyzerManager.ISEAnalyzer.IsISESwitchON = False
                        End If

                    End If
                Else

                    _analyzerManager.CanSendingRepetitions() = True
                    _analyzerManager.NumSendingRepetitionsTimeout() += 1
                    If _analyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                        GlobalBase.CreateLogActivity("Num of Repetitions for Start Tasks timeout excedeed because error 61 !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                        _analyzerManager.WaitingStartTaskTimerEnabled() = False
                        _analyzerManager.CanSendingRepetitions() = False

                        ' Activates Alarm begin
                        Dim alarmStatus As Boolean
                        Dim myAlarmList As New List(Of Alarms)
                        Dim myAlarmStatusList As New List(Of Boolean)

                        Const alarmId As Alarms = Alarms.ISE_TIMEOUT_ERR
                        alarmStatus = True
                        _analyzerManager.ISEAnalyzer.IsTimeOut = True

                        _analyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                        If myAlarmList.Count > 0 Then
                            ' Note that this alarm is common on User and Service !
                            Dim currentAlarms = New AnalyzerAlarms(_analyzerManager)
                            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                        End If
                        ' Activates Alarm end

                        _analyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                    Else
                        If _analyzerManager.StartTaskInstructionsQueueCount() > 0 Then
                            Debug.Print("Deactivate waiting time control (2) ...")
                            _analyzerManager.NumRepetitionsStateInstruction() = 0
                            _analyzerManager.InitializeTimerSTATEControl(WAITING_TIME_OFF)

                            GlobalBase.CreateLogActivity("Waiting because error 61 [" & WAITING_TIME_ISE_FAST.ToString & "] seconds ...", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                            Dim myDateTime As DateTime = DateAdd(DateInterval.Second, WAITING_TIME_ISE_FAST, DateTime.Now)
                            While myDateTime > DateTime.Now
                                ' spending time ...
                            End While
                            GlobalBase.CreateLogActivity("Waiting because error 61 consumed ! ", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)

                            ' Instruction has not started by Fw, so is need to send it again
                            GlobalBase.CreateLogActivity("Repeat Start Task Instruction because error 61 [" & _analyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                            myGlobal = _analyzerManager.SendStartTaskinQueue()
                        End If

                    End If

                End If
            End If
        End Sub

        ''' <summary>
        ''' Sleep business logic
        ''' </summary>
        ''' <param name="pAx00ActionCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG 28/02/2011 - Tested PENDING
        ''' Modified by XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
        ''' </remarks>
        Private Function ManageSleepStatus(ByVal pAx00ActionCode As AnalyzerManagerAx00Actions) As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
            Dim myGlobal As New GlobalDataTO

            Try

                Select Case pAx00ActionCode
                    'When the STANDBY instruction starts then update internal Sw flags
                    Case AnalyzerManagerAx00Actions.STANDBY_START
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, InProcessState)
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, "INI")
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WupStartDateTime, Now.ToString(CultureInfo.InvariantCulture))

                        'Delete flags for SLEEP status
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SDOWNprocess, "")
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SleepInstrument, "")

                        'Ax00 enters in SLEEP status
                    Case AnalyzerManagerAx00Actions.SLEEP_END
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SleepInstrument, "END")
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "")
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SDOWNprocess, "CLOSED")

                        _analyzerManager.AlarmListClear() 'AG 23/05/2012 - In sleeping Remove all alarms

                        'AG 16/04/2012 - Stop the sensor information instructions
                        myGlobal = _analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STP)
                        _analyzerManager.SetAnalyzerNotReady() 'analyzer is not ready to perform anything but CONNECT

                        'AG 03/10/2011 - Set to false the Connected attribute + prepare ui refresh
                        _analyzerManager.Connected() = False
                        _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(_analyzerManager.Connected(), 1, 0)), True)

                        'AG 23/03/2012 - case change status not succeeded because some error codes appears
                        '(curent status SLEEP but Fw informs action standby end (status must be stand by) or running end (status must be running)
                    Case AnalyzerManagerAx00Actions.STANDBY_END, AnalyzerManagerAx00Actions.RUNNING_END
                        'resetFlagsValue = True
                        _analyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ERROR_IN_STATUS_CHANGING, 1, True)
                End Select

                If Not myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageSleepStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region


    End Class
End Namespace
