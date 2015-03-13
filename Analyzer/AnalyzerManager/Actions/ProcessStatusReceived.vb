Imports System.Globalization
Imports System.Threading
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Namespace Biosystems.Ax00.Core.Entities
    Public Class ProcessStatusReceived

        Public AnalyzerManager As IAnalyzerManager
        Private _startingRunningFirstTime As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="analyzerMan"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef analyzerMan As IAnalyzerManager)
            AnalyzerManager = analyzerMan
        End Sub

        ''' <summary>
        ''' Software has received a new state instruction
        ''' This function treats this instruction and decides next action to perform
        ''' 
        ''' Treats the Thermo values and evaluate if exists alarm or not - TO CONFIRM????
        ''' When errors then ask for Hw alarm details
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
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
        Public Function InstructionProcessing(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myInstParamTo As New InstructionParameterTO
            Dim startTime As DateTime = Now
            Dim myStatusValue As AnalyzerManagerStatus = AnalyzerManagerStatus.NONE

            Try

                If AnalyzerManager.ISECMDStateIsLost() Or AnalyzerManager.RunningLostState() Then
                    Debug.Print("Deactivate waiting time control ...")
                    AnalyzerManager.NumRepetitionsStateInstruction = 0
                    AnalyzerManager.InitializeTimerSTATEControl(WAITING_TIME_OFF)
                End If

                'Get State field (parameter index 3)
                If Not GetStateField(pInstructionReceived, myStatusValue, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get Action field (parameter index 4)
                Dim myActionValue As AnalyzerManagerAx00Actions
                If Not GetActionField(pInstructionReceived, myActionValue, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get Time field (parameter index 5)
                Dim myExpectedTime As Integer
                Dim myExpectedTimeRaw As Integer
                If Not GetTimeField(pInstructionReceived, myExpectedTime, myExpectedTimeRaw, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get Well field (parameter index 7)
                Dim myWellValue As Integer
                If Not GetWellField(pInstructionReceived, myWellValue, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get Request field (parameter index 8)
                Dim myRequestValue As Integer
                If Not GetRequestField(pInstructionReceived, myStatusValue, myActionValue, myExpectedTime, myRequestValue, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get Error field (parameter index 9)
                Dim errorValue As Integer
                If Not GetErrorField(pInstructionReceived, myActionValue, myExpectedTimeRaw, errorValue, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Get ISE field (parameter index 10) also ISEModuleIsReadyAttribute is updated using the Fw information send
                If Not GetIseField(pInstructionReceived, myGlobal, myInstParamTo) Then
                    Return myGlobal
                End If

                ' Treatment single and multiple errors 
                TreatmentErrorValue(myGlobal, errorValue)


                '-------------------------------------------------------------------------------------                
                'Finally:
                'Do business depending the requestvalue, action value, status value, alarms value,....
                '-------------------------------------------------------------------------------------                
                'If action say us CONNECTION_ESTABLISHMENT update internal flags
                If myActionValue = AnalyzerManagerAx00Actions.CONNECTION_DONE Then

                    If GlobalBase.IsServiceAssembly AndAlso errorValue = 0 Then
                        AnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.STATUS_RECEIVED
                        AnalyzerManager.ConnectionDoneReceptionEvent()
                        If Not AnalyzerManager.IsFwUpdateInProcess Then AnalyzerManager.InfoRefreshFirstTime = True
                    End If

                    'When successfully inform the connection results
                    AnalyzerManager.Connected() = True
                    AnalyzerManager.PortName() = AnalyzerManager.ConnectedPortName
                    AnalyzerManager.Bauds() = AnalyzerManager.ConnectedBauds()

                End If

                'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                If Not GlobalBase.IsServiceAssembly Then 'NOT for SERVICE SOFTWARE 01/07/2011
                    Select Case myStatusValue
                        Case AnalyzerManagerStatus.SLEEPING
                            Dim resetFlags As Boolean = True
                            If Not AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" AndAlso _
                               Not AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                                myGlobal = ManageSleepStatus(myActionValue)
                            End If

                            If myActionValue = AnalyzerManagerAx00Actions.STANDBY_START OrElse myActionValue = AnalyzerManagerAx00Actions.SLEEP_END _
                            OrElse AnalyzerManager.SessionFlag(AnalyzerManagerFlags.WaitForAnalyzerReady) = "INI" Then
                                resetFlags = False
                            End If

                            If resetFlags AndAlso Not myGlobal.HasError Then
                                'reset internal flags when analyzer is sleeping and no action has been performed
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                myGlobal = myFlagsDelg.ResetFlags(Nothing, AnalyzerManager.ActiveAnalyzer())
                                AnalyzerManager.InitializeAnalyzerFlags(Nothing)
                            End If

                        Case AnalyzerManagerStatus.STANDBY
                            If Not AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" AndAlso _
                               Not AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'AG 14/06/2012 - Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                                myGlobal = AnalyzerManager.ManageStandByStatus(myActionValue, myWellValue)
                            End If

                        Case AnalyzerManagerStatus.RUNNING
                            If Not AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" AndAlso _
                               Not AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'Do not manage instruction by analyzer status if connection process is in course (RUNNING)
                                myGlobal = AnalyzerManager.ManageRunningStatus(myActionValue, myWellValue)

                                If myRequestValue = 1 Then startTime = Now

                            End If
                    End Select


                    '(status <> RUNNING) After connection establishment Sw has to wait until BAx00 is ready to receive the remaining instructions of the connection process
                    '(status = RUNNING) Sound OFF + no more changes (until spec how to know the analyzerID)
                    Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                    If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE AndAlso _
                        AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then

                        'Different connect business when analyzer status <> Running and when is running
                        If AnalyzerManager.AnalyzerStatus() <> AnalyzerManagerStatus.RUNNING Then
                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ReportSATonRUNNING, "CLOSED")

                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
                            myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True) 'Ask for status (and waits unitl analyzer becomes ready)
                        Else
                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                            'when Connecting on RUNNING Session we must ask for Serial Number (POLLSN) 

                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RESULTSRECOVERProcess, "INPROCESS")
                            AnalyzerManager.EndRunInstructionSent() = True 'AG 25/09/2012 - When resultsrecover process starts the Fw implements an automatic END, so add protection in order Sw do not sent this instruction again

                            'in running Sw has to know if analyzer is in normal or paused running before call POLLSN so we has to send STATE instruction
                            myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True) 'Send STATE instruction

                        End If

                        If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                            AnalyzerManager.SetAnalyzerNotReady()
                        End If

                    ElseIf AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" AndAlso _
                           AnalyzerManager.SessionFlag(AnalyzerManagerFlags.WaitForAnalyzerReady) = "INI" Then

                        'SGM 21/06/2012 - Protection!! INFO will be activated later, on POLLFW answer reception
                        If myStatusValue = AnalyzerManagerStatus.STANDBY Then
                            myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STP)
                            Thread.Sleep(1000)
                        End If

                        If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.NO_ACTION Then
                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                            'Continue with the connection process instructions flow
                            If myStatusValue <> AnalyzerManagerStatus.RUNNING Then
                                myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.POLLFW, True, Nothing, POLL_IDs.CPU)
                            End If

                        Else
                            If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT
                            AnalyzerManager.InitializeTimerControl(myExpectedTime)
                            AnalyzerManager.MaxWaitTime() = myExpectedTime + SYSTEM_TIME_OFFSET
                        End If

                    ElseIf AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONFIG_DONE AndAlso _
                            AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                        'Send SOUND OFF instruction
                        myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.ENDSOUND, True)
                        If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                            AnalyzerManager.SetAnalyzerNotReady()
                        End If

                    ElseIf AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.SOUND_DONE AndAlso _
                            AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "CLOSED")
                        AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(AnalyzerManager.Connected(), 1, 0)), True) 'Inform connection finished OK

                        'when recovery results finished, Sw call the processConnection method for execute the full connection
                        If AnalyzerManager.SessionFlag(AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" AndAlso AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY Then
                            AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")
                            AnalyzerManager.RecoveryResultsInPause() = False
                            AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.RECOVERY_RESULTS_STATUS, 0, True) 'Generate UI refresh for presentation - Inform the recovery results has finished!!
                            AnalyzerManager.ManageStandByStatus(AnalyzerManagerAx00Actions.STANDBY_END, AnalyzerManager.CurrentWell()) 'Call this method with this action code to update ISE consumption if needed

                            'We have to do it now because the normal connection finishes is when action SOUND_DONE is received
                            If AnalyzerManager.AnalyzerIsFreeze() Then
                                myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.SOUND, True)
                                If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                                    AnalyzerManager.SetAnalyzerNotReady()
                                End If
                            End If

                        End If

                        'If no ISE installed LOCK all pending executions in current worksession/analyzer
                        If Not myGlobal.HasError Then
                            Dim adjustValue As String
                            adjustValue = AnalyzerManager.ReadAdjustValue(Ax00Adjustsments.ISEINS)
                            If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                                Dim iseInstalledFlag = CType(adjustValue, Boolean)
                                If Not iseInstalledFlag Then
                                    Dim myExecutions As New ExecutionsDelegate
                                    myGlobal = myExecutions.UpdateStatusByExecutionTypeAndStatus(Nothing, AnalyzerManager.ActiveWorkSession(), AnalyzerManager.ActiveAnalyzer(), "PREP_ISE", "PENDING", "LOCKED")
                                End If
                            End If
                        End If

                        If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY Then
                            AnalyzerManager.InfoActivated() = 0
                            myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                        End If

                        myGlobal = AnalyzerManager.ManageInterruptedProcess(Nothing) 'Evaluate value for FLAGS and determine the next action to be sent in order to achieve a stable setup

                    ElseIf AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" AndAlso Not AnalyzerManager.RunningConnectionPollSnSentStatus() AndAlso _
                        AnalyzerManager.SessionFlag(AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" AndAlso AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                        myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.POLLSN, True) 'Send POLLSN instruction

                    End If

                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                    End If

                End If


                If myStatusValue = AnalyzerManagerStatus.SLEEPING Then

                    If AnalyzerManager.ISEAnalyzer IsNot Nothing AndAlso AnalyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                        AnalyzerManager.ISEAnalyzer.IsISEInitiatedDone = False
                        AnalyzerManager.IseIsAlreadyStarted = False
                    End If

                    If AnalyzerManager.IsShutDownRequested() Then
                        AnalyzerManager.Connected() = False
                        AnalyzerManager.InfoRefreshFirstTime = True
                        AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(AnalyzerManager.Connected(), 1, 0)), True)
                    End If
                End If

                'AG 28/06/2012 - Launch the parallel threat when Running and Request received and preparation instruction is already sent
                'Also during running initialization

                If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                    SyncLock LockThis
                        If AnalyzerManager.BufferANSPHRReceivedCount() > 0 AndAlso Not AnalyzerManager.ProcessingLastANSPHRInstructionStatus() Then

                            Dim startDoWorker As Boolean = False 'Evaluate if first ANSPHR in queue can be processed or not
                            If Not AnalyzerManager.EndRunInstructionSent() AndAlso Not AnalyzerManager.AbortInstructionSent() AndAlso Not AnalyzerManager.PauseInstructionSent() Then
                                Select Case AnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.TEST_PREPARATION_RECEIVED, AnalyzerManagerAx00Actions.PREDILUTED_TEST_RECEIVED, _
                                        AnalyzerManagerAx00Actions.ISE_TEST_RECEIVED, AnalyzerManagerAx00Actions.SKIP_START, _
                                        AnalyzerManagerAx00Actions.WASHING_RUN_START, AnalyzerManagerAx00Actions.START_INSTRUCTION_START, _
                                        AnalyzerManagerAx00Actions.START_INSTRUCTION_END, AnalyzerManagerAx00Actions.PAUSE_START, _
                                        AnalyzerManagerAx00Actions.PAUSE_END, AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED

                                        startDoWorker = True

                                        'AG 19/11/2013 - This case is not possible but we add the protection
                                        If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED AndAlso Not AnalyzerManager.AllowScanInRunning Then
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
                                    AnalyzerManager.ProcessingLastANSPHRInstructionStatus() = True
                                    AnalyzerManager.RunWellBaseLineWorker()
                                Else
                                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore busy. Don't process ANSPHR this cycle!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                                End If
                            End If
                        End If
                    End SyncLock
                End If

                If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                    GlobalBase.CreateLogActivity("Treat STATUS received: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex)
            End Try

            Return myGlobal
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
            AnalyzerManager.AnalyzerCurrentAction = myActionValue

            Select Case myActionValue

                Case AnalyzerManagerAx00Actions.SOUND_DONE
                    'Change the status of AnalyzerManager.Ringing() (TRUE --> FALSE, FALSE --> TRUE) Except when connection ... always FALSE in this case
                    If AnalyzerManager.SessionFlag(AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                        AnalyzerManager.Ringing() = False
                    Else
                        AnalyzerManager.Ringing() = Not AnalyzerManager.Ringing()
                    End If
                    AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ANALYZER_SOUND_CHANGED, CSng(IIf(AnalyzerManager.Ringing(), 1, 0)), True)

                Case AnalyzerManagerAx00Actions.END_RUN_START
                    AnalyzerManager.EndRunInstructionSent() = True

                Case AnalyzerManagerAx00Actions.END_RUN_END
                    AnalyzerManager.EndRunInstructionSent() = False

                Case AnalyzerManagerAx00Actions.ABORT_START
                    AnalyzerManager.AbortInstructionSent() = True
                    AnalyzerManager.EndRunInstructionSent() = True

                Case AnalyzerManagerAx00Actions.ABORT_END
                    AnalyzerManager.PauseInstructionSent() = False

                    AnalyzerManager.AbortInstructionSent() = False
                    AnalyzerManager.EndRunInstructionSent() = False

                Case AnalyzerManagerAx00Actions.PAUSE_START
                    If String.Compare(AnalyzerManager.SessionFlag(AnalyzerManagerFlags.PAUSEprocess), "", False) <> 0 Then
                        AnalyzerManager.PauseInstructionSent() = True
                    End If

                Case AnalyzerManagerAx00Actions.PAUSE_END
                    AnalyzerManager.PauseInstructionSent() = False

                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START
                    AnalyzerManager.RecoverInstructionSent() = True

                Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END
                    AnalyzerManager.RecoverInstructionSent() = False
                    myGlobal = AnalyzerManager.RemoveErrorCodeAlarms(Nothing, myActionValue)

                    If GlobalBase.IsServiceAssembly Then
                        AnalyzerManager.InfoRefreshFirstTime = True
                    End If

                    AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished

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

            If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING And myStatusValue = AnalyzerManagerStatus.STANDBY Then
                AnalyzerManager.ExecuteSpecialBusinessOnAnalyzerStatusChanges(Nothing, myStatusValue)
            End If

            If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then GlobalConstants.AnalyzerIsRunningFlag = True Else GlobalConstants.AnalyzerIsRunningFlag = False

            If AnalyzerManager.AnalyzerStatus() <> myStatusValue Then
                AnalyzerManager.AnalyzerStatus() = myStatusValue
                AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ANALYZER_STATUS_CHANGED, 1, True) 'Prepare UI refresh event when analyzer status changes
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
                AnalyzerManager.ISEModuleIsReady() = CType(IIf(iseAvailableValue = 1, 1, 0), Boolean)
            Else
                Return False
            End If

            'SGM 29/10/2012 - SERVICE: reset E:20 flag
            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                AnalyzerManager.IsInstructionRejected() = False
                AnalyzerManager.IsRecoverFailed() = False
                AnalyzerManager.IsInstructionAborted() = False
            End If
            Return True
        End Function

        Private Function GetErrorField(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myActionValue As AnalyzerManagerAx00Actions, ByVal myExpectedTimeRaw As Integer, _
                                       ByRef errorValue As Integer, ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO) As Boolean

            errorValue = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 9)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTO.ParameterValue) Then
                errorValue = CInt(myInstParamTO.ParameterValue)
            Else
                Return False
            End If

            If errorValue <> 61 Then
                If AnalyzerManager.ISECMDStateIsLost() Then
                    AnalyzerManager.ISECMDStateIsLost() = False

                    If AnalyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.ISE_ACTION_START Then
                        AnalyzerManager.CanSendingRepetitions() = True
                        AnalyzerManager.NumSendingRepetitionsTimeout() += 1
                        If AnalyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                            GlobalBase.CreateLogActivity("Num of Repetitions for Start Tasks timeout excedeed !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                            AnalyzerManager.WaitingStartTaskTimerEnabled() = False
                            AnalyzerManager.CanSendingRepetitions() = False

                            ' Activates Alarm begin
                            Dim alarmId As Alarms
                            Dim alarmStatus As Boolean
                            Dim myAlarmList As New List(Of Alarms)
                            Dim myAlarmStatusList As New List(Of Boolean)

                            alarmId = Alarms.ISE_TIMEOUT_ERR
                            alarmStatus = True
                            AnalyzerManager.ISEAnalyzer.IsTimeOut = True

                            AnalyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                            If myAlarmList.Count > 0 Then
                                ' Note that this alarm is common on User and Service !
                                Dim currentAlarms = New CurrentAlarms(AnalyzerManager)
                                myGlobal = currentAlarms.Manage(Nothing, myAlarmList, myAlarmStatusList)                                
                            End If
                            ' Activates Alarm end

                            AnalyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                        Else
                            ' Instruction has not started by Fw, so is need to send it again
                            GlobalBase.CreateLogActivity("Repeat Start Task Instruction [" & AnalyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                            myGlobal = AnalyzerManager.SendStartTaskinQueue()
                        End If
                    End If

                End If

                If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.ISE_ACTION_START Then
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Action Start =34")

                    ' Update the interval of the Timer with the expected time received from the Analyzer
                    If myExpectedTimeRaw <= 0 Then
                        If Not AnalyzerManager.SetTimeISEOffsetFirstTime() Then
                            AnalyzerManager.SetTimeISEOffsetFirstTime() = True
                            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & WAITING_TIME_ISE_OFFSET.ToString & "] seconds")
                            AnalyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_ISE_OFFSET)
                        End If
                    Else
                        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & AnalyzerManager.MaxWaitTime().ToString & "] seconds")
                        AnalyzerManager.InitializeTimerStartTaskControl(AnalyzerManager.MaxWaitTime())
                    End If
                End If

            End If
            If AnalyzerManager.RunningLostState() Then
                AnalyzerManager.RunningLostState() = False

                If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                    If (AnalyzerManager.SessionFlag(AnalyzerManagerFlags.RUNNINGprocess) = "INPROCESS") AndAlso _
                       (AnalyzerManager.SessionFlag(AnalyzerManagerFlags.EnterRunning) = "INI") Then
                        myActionValue = AnalyzerManagerAx00Actions.RUNNING_END
                        AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_END
                    End If
                End If

                If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY AndAlso _
                   AnalyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_START AndAlso _
                   AnalyzerManager.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_END Then
                    AnalyzerManager.CanSendingRepetitions() = True
                    AnalyzerManager.NumSendingRepetitionsTimeout() += 1
                    If AnalyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                        GlobalBase.CreateLogActivity("Num of Repetitions for RUNNING excedeed !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                        AnalyzerManager.WaitingStartTaskTimerEnabled() = False
                        AnalyzerManager.CanSendingRepetitions() = False

                        ' Activates Alarm begin
                        Dim alarmId As Alarms
                        Dim alarmStatus As Boolean
                        Dim myAlarmList As New List(Of Alarms)
                        Dim myAlarmStatusList As New List(Of Boolean)

                        alarmId = Alarms.COMMS_TIMEOUT_ERR
                        alarmStatus = True
                        AnalyzerManager.ISEAnalyzer.IsTimeOut = True

                        AnalyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                        If myAlarmList.Count > 0 Then
                            ' Note that this alarm is common on User and Service !
                            Dim currentAlarms = New CurrentAlarms(AnalyzerManager)
                            myGlobal = currentAlarms.Manage(Nothing, myAlarmList, myAlarmStatusList)
                        End If
                        ' Activates Alarm end

                        Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")

                        'Update internal flags. Basically used by the running normal business
                        If (Not myGlobal.HasError AndAlso AnalyzerManager.Connected()) Then
                            'Update analyzer session flags into DataBase
                            If (myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                            End If
                        End If

                        AnalyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                    Else
                        ' Instruction has not started by Fw, so is need to send it again
                        GlobalBase.CreateLogActivity("Repeat RUNNING Instruction [" & AnalyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                        myGlobal = AnalyzerManager.SendStartTaskinQueue()
                    End If
                End If

            End If

            If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_START Then
                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - RUNNING Action Start =7 UPDATED TIME TO [" & AnalyzerManager.MaxWaitTime().ToString & "] seconds")
                ' Update the interval of the Timer with the expected time received from the Analyzer
                Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & AnalyzerManager.MaxWaitTime().ToString & "] seconds")
                AnalyzerManager.InitializeTimerControl(WAITING_TIME_OFF)    ' This timer is disabled because this operation is managed by StartTaskTimer
                AnalyzerManager.InitializeTimerStartTaskControl(AnalyzerManager.MaxWaitTime())
                _startingRunningFirstTime = True
            End If

            If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.RUNNING_END Or _
               AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                If _startingRunningFirstTime Then
                    _startingRunningFirstTime = False
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - RUNNING Action END =8")
                    AnalyzerManager.RunningLostState() = False
                    AnalyzerManager.CanSendingRepetitions() = False
                    AnalyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                    AnalyzerManager.ClearStartTaskQueueToSend()

                    ' Deactivates Alarm begin - BA-1872
                    Dim alarmId As Alarms
                    Dim alarmStatus As Boolean
                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)

                    alarmId = Alarms.COMMS_TIMEOUT_ERR
                    alarmStatus = False

                    AnalyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        If Not GlobalBase.IsServiceAssembly Then
                            Dim currentAlarms = New CurrentAlarms(AnalyzerManager)
                            myGlobal = currentAlarms.Manage(Nothing, myAlarmList, myAlarmStatusList)
                        End If

                    End If
                    If AnalyzerManager.Alarms().Contains(Alarms.COMMS_TIMEOUT_ERR) Then AnalyzerManager.AlarmListRemoveItem(Alarms.COMMS_TIMEOUT_ERR)
                End If
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
            AnalyzerManager.StartUseRequestFlag() = False
            If myStatusValue = AnalyzerManagerStatus.RUNNING Then
                If myActionValue = AnalyzerManagerAx00Actions.START_INSTRUCTION_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.TEST_PREPARATION_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.PREDILUTED_TEST_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.ISE_TEST_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.SKIP_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.WASHING_RUN_END Or _
                   myActionValue = AnalyzerManagerAx00Actions.LOADADJ_END Then

                    AnalyzerManager.StartUseRequestFlag() = True
                End If
            End If

            If Not AnalyzerManager.StartUseRequestFlag() Then
                AnalyzerManager.AnalyzerIsReady() = (myExpectedTime = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
            Else
                AnalyzerManager.AnalyzerIsReady() = (myRequestValue = 1) '1 Ax00 is ready to work, 0 Ax00 is busy (for use only in RUNNING mode)
            End If

            'AG 03/09/2012 - If connection analyzer always ready to receive new instructions
            If AnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONNECTION_DONE Then
                AnalyzerManager.AnalyzerIsReady() = True
            End If

            If Not AnalyzerManager.AnalyzerIsReady() Then
                If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT

                'AG 13/02/2012 - In Running activate always the waiting time
            ElseIf AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.RUNNING Then
                'AG 21/03/2012 - exception when the abort instruction has started. In this case take into account the TIME received
                If myActionValue <> AnalyzerManagerAx00Actions.ABORT_START Then
                    myExpectedTime = WAITING_TIME_DEFAULT
                End If

            End If
            AnalyzerManager.InitializeTimerControl(myExpectedTime)
            AnalyzerManager.MaxWaitTime() = myExpectedTime + SYSTEM_TIME_OFFSET

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
                AnalyzerManager.CurrentWell() = myWellValue 'Inform the class attribute
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

                        AnalyzerManager.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                        AnalyzerManager.ClearStartTaskQueueToSend()

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

        Private Sub TreatmentErrorValue(ByRef myGlobal As GlobalDataTO, ByVal errorValue As Integer)

            If errorValue <> 0 Then

                If Not AnalyzerManager.IgnoreErrorCodes(AnalyzerManager.InstructionTypeSent(), AnalyzerManager.InstructionSent(), errorValue) Then
                    If errorValue = MULTIPLE_ERROR_CODE Then 'If multiple alarm error code ask for details

                        myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.ALR)

                        'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                        If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                            AnalyzerManager.SetAnalyzerNotReady()
                        End If


                    Else 'If single error code then treat it!!

                        'Translation method
                        Dim myAlarmsReceivedList As New List(Of Alarms)
                        Dim myAlarmsStatusList As New List(Of Boolean)
                        Dim myAlarmsAdditionalInfoList As New List(Of String) 'AG 09/12/2014 BA-2236

                        Dim myErrorCode As New List(Of Integer)
                        Dim myFwCodeErrorReceivedList As New List(Of String)

                        myErrorCode.Add(errorValue)

                        Dim myAlarms = AnalyzerManager.TranslateErrorCodeToAlarmID(Nothing, myErrorCode)

                        If GlobalBase.IsServiceAssembly Then
                            If Not myAlarms.Contains(Alarms.REACT_MISSING_ERR) Then
                                AnalyzerManager.IsServiceRotorMissingInformed() = False
                            End If
                        End If

                        If myAlarms.Contains(Alarms.ISE_TIMEOUT_ERR) Then

                            If AnalyzerManager.ISEAnalyzer IsNot Nothing Then
                                If Not AnalyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                                    ' If ISE module isn't Installed remove the ISE Timeout Alarm
                                    Debug.Print("ISE Module NOT installed !")
                                    myAlarms.Remove(Alarms.ISE_TIMEOUT_ERR)

                                    If GlobalBase.IsServiceAssembly Then
                                        ' Only Sw Service
                                        If Not myAlarms.Contains(Alarms.ISE_OFF_ERR) Then
                                            myAlarms.Add(Alarms.ISE_OFF_ERR)
                                            AnalyzerManager.ISEAnalyzer.IsISESwitchON = False
                                        End If

                                    End If
                                Else

                                    AnalyzerManager.CanSendingRepetitions() = True
                                    AnalyzerManager.NumSendingRepetitionsTimeout() += 1
                                    If AnalyzerManager.NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsTimeout Then
                                        GlobalBase.CreateLogActivity("Num of Repetitions for Start Tasks timeout excedeed because error 61 !!!", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                                        AnalyzerManager.WaitingStartTaskTimerEnabled() = False
                                        AnalyzerManager.CanSendingRepetitions() = False

                                        ' Activates Alarm begin
                                        Dim alarmStatus As Boolean
                                        Dim myAlarmList As New List(Of Alarms)
                                        Dim myAlarmStatusList As New List(Of Boolean)

                                        Const alarmId As Alarms = Alarms.ISE_TIMEOUT_ERR
                                        alarmStatus = True
                                        AnalyzerManager.ISEAnalyzer.IsTimeOut = True

                                        AnalyzerManager.PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                                        If myAlarmList.Count > 0 Then
                                            ' Note that this alarm is common on User and Service !
                                            Dim currentAlarms = New CurrentAlarms(AnalyzerManager)
                                            myGlobal = currentAlarms.Manage(Nothing, myAlarmList, myAlarmStatusList)                                            
                                        End If
                                        ' Activates Alarm end

                                        AnalyzerManager.ActionToSendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                    Else
                                        If AnalyzerManager.StartTaskInstructionsQueueCount() > 0 Then
                                            Debug.Print("Deactivate waiting time control (2) ...")
                                            AnalyzerManager.NumRepetitionsStateInstruction() = 0
                                            AnalyzerManager.InitializeTimerSTATEControl(WAITING_TIME_OFF)

                                            GlobalBase.CreateLogActivity("Waiting because error 61 [" & WAITING_TIME_ISE_FAST.ToString & "] seconds ...", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                                            Dim myDateTime As DateTime = DateAdd(DateInterval.Second, WAITING_TIME_ISE_FAST, DateTime.Now)
                                            While myDateTime > DateTime.Now
                                                ' spending time ...
                                            End While
                                            GlobalBase.CreateLogActivity("Waiting because error 61 consumed ! ", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)

                                            ' Instruction has not started by Fw, so is need to send it again
                                            GlobalBase.CreateLogActivity("Repeat Start Task Instruction because error 61 [" & AnalyzerManager.NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                                            myGlobal = AnalyzerManager.SendStartTaskinQueue()
                                        End If

                                    End If

                                End If
                            End If

                        End If
                        Dim index As Integer = 0
                        Dim errorCodeId As String

                        For Each alarmId As Alarms In myAlarms
                            errorCodeId = ""
                            If index <= myErrorCode.Count - 1 Then
                                errorCodeId = myErrorCode(index).ToString
                            End If
                            AnalyzerManager.PrepareLocalAlarmList(alarmId, True, myAlarmsReceivedList, myAlarmsStatusList, errorCodeId, myAlarmsAdditionalInfoList, True)
                            index += 1 'AG 30/01/2015 BA-2222 increment the counter!!
                        Next

                        If GlobalBase.IsServiceAssembly Then
                            ' Initialize Error Codes List
                            AnalyzerManager.MyErrorCodesClear()
                            ' Prepare error codes List received from Analyzer
                            AnalyzerManager.PrepareLocalAlarmList_SRV(myErrorCode, myFwCodeErrorReceivedList)
                        End If

                        If myAlarmsReceivedList.Count > 0 Then
                            '3- Finally call manage all alarms detected (new or fixed)
                            If GlobalBase.IsServiceAssembly Then
                                myGlobal = AnalyzerManager.ManageAlarms_SRV(Nothing, myAlarmsReceivedList, myAlarmsStatusList, myFwCodeErrorReceivedList)
                            Else
                                Dim currentAlarms = New CurrentAlarms(AnalyzerManager)
                                myGlobal = currentAlarms.Manage(Nothing, myAlarmsReceivedList, myAlarmsStatusList, myAlarmsAdditionalInfoList)
                            End If

                        Else 'if not new alarms sure the ansinfo instruction is activated
                            If AnalyzerManager.AnalyzerStatus() = AnalyzerManagerStatus.STANDBY AndAlso AnalyzerManager.SessionFlag(AnalyzerManagerFlags.RUNNINGprocess) <> "INPROCESS" Then

                                Dim updateIseConsumptionFlag As Boolean = False
                                ' Estimated ISE Consumption by Firmware during WS
                                If Not AnalyzerManager.ISEAnalyzer Is Nothing _
                                   AndAlso AnalyzerManager.ISEAnalyzer.IsISEModuleInstalled Then
                                    AnalyzerManager.ISEAnalyzer.EstimatedFWConsumptionWS()
                                    ' Update ISE consumptions if required
                                    If AnalyzerManager.ISEAnalyzer.IsCalAUpdateRequired Or AnalyzerManager.ISEAnalyzer.IsCalBUpdateRequired Then
                                        updateIseConsumptionFlag = True
                                    End If
                                End If

                                'AG 12/04/2012 - New Fw disables info when analyzer leaves running, so Sw has to activate info when standby end
                                If Not updateIseConsumptionFlag AndAlso AnalyzerManager.SessionFlag(AnalyzerManagerFlags.ABORTprocess) <> "INPROCESS" Then

                                    myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                                    'AG 04/04/2012 - When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                    If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then AnalyzerManager.SetAnalyzerNotReady()
                                End If
                            End If
                        End If
                    End If
                End If
            Else 'Error code = 0
                If GlobalBase.IsServiceAssembly Then
                    If AnalyzerManager.ErrorCodesDisplay.Count > 0 Then
                        Dim pErrorCodeList As New List(Of String)
                        pErrorCodeList.Add("0")
                        ' Solve all previous alarms
                        AnalyzerManager.SolveErrorCodesToDisplay(pErrorCodeList)
                    End If

                    AnalyzerManager.MyErrorCodesClear()
                End If

                'Reset the freeze flags information
                If AnalyzerManager.AnalyzerIsFreeze() Then
                    'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                    'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If Not GlobalBase.IsServiceAssembly Then
                        'Clear all alarms with error code
                        myGlobal = AnalyzerManager.RemoveErrorCodeAlarms(Nothing, AnalyzerManager.AnalyzerCurrentAction())
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
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, "INI")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WupStartDateTime, Now.ToString(CultureInfo.InvariantCulture))

                        'Delete flags for SLEEP status
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SDOWNprocess, "")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SleepInstrument, "")

                        'Ax00 enters in SLEEP status
                    Case AnalyzerManagerAx00Actions.SLEEP_END
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SleepInstrument, "END")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
                        AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.SDOWNprocess, "CLOSED")

                        AnalyzerManager.AlarmListClear() 'AG 23/05/2012 - In sleeping Remove all alarms

                        'AG 16/04/2012 - Stop the sensor information instructions
                        myGlobal = AnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STP)
                        AnalyzerManager.SetAnalyzerNotReady() 'analyzer is not ready to perform anything but CONNECT

                        'AG 03/10/2011 - Set to false the Connected attribute + prepare ui refresh
                        AnalyzerManager.Connected() = False
                        AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(AnalyzerManager.Connected(), 1, 0)), True)

                        'AG 23/03/2012 - case change status not succeeded because some error codes appears
                        '(curent status SLEEP but Fw informs action standby end (status must be stand by) or running end (status must be running)
                    Case AnalyzerManagerAx00Actions.STANDBY_END, AnalyzerManagerAx00Actions.RUNNING_END
                        'resetFlagsValue = True
                        AnalyzerManager.UpdateSensorValuesAttribute(AnalyzerSensors.ERROR_IN_STATUS_CHANGING, 1, True)
                End Select

                If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
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
