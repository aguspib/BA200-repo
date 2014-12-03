﻿Option Explicit On
Option Strict On

'Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports System.Data
Imports System.Windows.Forms
Imports System.Globalization
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities
    Partial Public Class AnalyzerEntity

#Region "STATUS Business Logic"

        ''' <summary>
        ''' Running business logic
        ''' Interrupt instructions can be sent in Running (shown in priority order): STANDBY, ABORT, ENDRUN, START, PAUSE, BARCODE (in running pause mode), SOUND, INFO, STATE, ...
        ''' </summary>
        ''' <param name="pAx00ActionCode"></param>
        ''' <param name="pNextWell "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 03/11/2010
        ''' Modified by: AG 17/01/2011 - Added parameter pNextWell
        '''              AG 26/01/2012 - Get the SOUND instruction from the queue 
        '''              AG 05/06/2012 - Get the STANDBY or INFO or STATE instruction from the queue 
        '''              AG 10/12/2012 - Queue priority establishment:
        '''                              1) Queued instructions: STANDBY, ABORT, END and START are priority over the received action
        '''                              2) Some received action values in STATUS are priority over SOUND, INFO and STATE
        '''                              3) Queued instructions: SOUND, INFO and STATUS are priority over the others action values not in 2)
        '''             XB 15/10/2013 - Implemented mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) + Change ENDprocess instead of PAUSEprocess - BT #1318
        '''             AG 15/10/2013 - Added new condition to send START (action PAUSE_END)
        '''             SA 22/10/2013 - BT #1355 ==> Added code to include in the Alarms List the new warning Alarm WS_PAUSE_MODE_WARN
        '''             TR 22/10/2013 - BT #1353 Validate if there are alarms that required to send End Instruction.
        '''             TR 25/10/2013 - BT #1353 Do not send the end instruction in case the analyzer is in pause.
        '''             AG 30/10/2013 - BT #1342 When user decides stops WS (presentation button) Sw sends START and later END (adding it to internal queue)
        '''                           - also change conditions to get the START instruction from the queue
        '''             AG 22/11/2013 - Task #1397 If reconnect while analyzer is in pause the Sw has to send automatically START + END because in this scenario the Firmware cannot launch his autoEND mode
        '''             AG 28/11/2013 - More changes #1397 (2on design)
        '''             AG 10/12/2013 - Comment the change required for #1422 (do not sent END while action is START_Ini)
        '''             XB 11/12/2013 - Add the condition required for #1422 (do not send END while action is AnalyzerManagerFlags.StartRunning)
        '''             AG 15/04/2014 - Fix issue number #1594 paused in v300 (if SOUND is in queue it will be sent on receives STATUS (it doesnt matter AnalyzerIsReadyAttribute))
        '''                             #1484 action SOUND_DONE in running also search next preparation
        ''' </remarks>
        Private Function ManageRunningStatus(ByVal pAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions, ByVal pNextWell As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            Try

                'AG 07/07/2011 - In Running we have to wait a STATUS reception before send another instruction.
                'Search if there is some instruction (requested by user, alarms treatment,...) in queue waiting to be sent:
                '                Instr Priority: STANDBY, ABORT, ENDRUN, START, PAUSE, BARCODE, SOUND, INFO, STATE,...
                Dim myInterruptInstruction As GlobalEnumerates.AnalyzerManagerSwActionList = GlobalEnumerates.AnalyzerManagerSwActionList.NONE
                If (AnalyzerIsReadyAttribute) Then
                    If myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY) AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_START Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY

                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT) AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ABORT

                        'AG + XB 11/12/2013 - #1422 do not get the ENDRUN from queue while mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) = "INI")
                        'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                        '(it is the same as Action <> START_INSTRUCTION_START + protection against action SOUND during running initialization phase)
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN) AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START And _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_END AndAlso _
                        mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) <> "INI" AndAlso _
                        Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting Then

                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN

                        'AG 30/10/2013 - solve conditions 'AG 15/10/2013 - The START instruction can be sent also in Pause Mode (AllowScanInRunningAttribute) - 
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.START) AndAlso _
                        (pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START OrElse _
                        (pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_END OrElse AllowScanInRunningAttribute)) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.START

                        ' XB 15/10/2013 - BT #1318
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE) AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_START And _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE

                        'AG 16/10/2013 - read barcode in running pause mode
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST) AndAlso _
                        pAx00ActionCode <> GlobalEnumerates.AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST

                        'AG 26/01/2012 - In running the SOUND and ENDSOUND are added into a queue 
                        'and will be sent with the LOWER PRIORITY!!!
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.SOUND

                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND
                        'AG 26/01/2012

                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.INFO) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.INFO

                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.STATE) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.STATE


                        ' XB 23/10/2013 - Specific ISE commands are allowed in RUNNING (pause mode) - BT #1343
                    ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD) Then
                        myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD
                        ' XB 23/10/2013

                    End If

                    ''AG 07/02/2014 - BT #1594 (move here this code) - Queue instructions priority establishment
                    'If myInterruptInstruction <> GlobalEnumerates.AnalyzerManagerSwActionList.NONE Then
                    '    myInterruptInstruction = TreatQueueExceptionsInRunning(myInterruptInstruction, pAx00ActionCode)
                    'End If
                    ''AG 07/02/2014 - BT #1594


                Else 'Anlayzer not ready
                    ''AG 07/02/2014 - BT #1594 - althought the analyzer is not ready the SOUND instruction can be sent on STATUS reception!!
                    'If myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND) Then
                    '    myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.SOUND

                    'ElseIf myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND) Then
                    '    myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND
                    'End If
                    ''AG 07/02/2014 - BT #1594
                End If

                'AG 07/02/2014 - BT #1594 (move here this code) - Queue instructions priority establishment
                If myInterruptInstruction <> GlobalEnumerates.AnalyzerManagerSwActionList.NONE Then
                    myInterruptInstruction = TreatQueueExceptionsInRunning(myInterruptInstruction, pAx00ActionCode)
                End If
                'AG 07/02/2014 - BT #1594

                'If there is NO instruction in queue apply the normal business for manage running status
                If (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.NONE) Then
                    'AG 07/07/2011
                    Select Case (pAx00ActionCode)
                        'When the RUNNING instruction finishes, then SW automatically sends the START instruction
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.RUNNING_END
                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.START)
                            If (Not myGlobal.HasError) Then
                                endRunAlreadySentFlagAttribute = False
                                abortAlreadySentFlagAttribute = False
                                PauseAlreadySentFlagAttribute = False   ' XB 15/10/2013 - BT #1318
                            End If

                            'When a process involves an instruction sending sequence automatic (for instance RUNNING (end) + START) change the AnalyzerIsReady value
                            If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                                SetAnalyzerNotReady()
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.EnterRunning, "END")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "CLOSED") 'AG 29/03/2012 - closed when running end not when start end
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'AG 24/02/2012 Process finished (running MODE)
                            End If

                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_START
                            futureRequestNextWell = 0
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartRunning, "INI")

                            'XB 15/10/2013 - BT #1318
                            ' AllowScanInRunningAttribute = False
                            ' UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")

                            ContinueAlreadySentFlagAttribute = True

                            'If the WorkSession has not been started then started it
                            Dim wsDelegate As New WorkSessionsDelegate
                            myGlobal = wsDelegate.GetByWorkSession(Nothing, WorkSessionIDAttribute)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                Dim wsDS As WorkSessionsDS = DirectCast(myGlobal.SetDatos, WorkSessionsDS)

                                If (wsDS.twksWorkSessions.Rows.Count > 0) Then
                                    'If ws StartData is NULL update it
                                    If (wsDS.twksWorkSessions(0).IsStartDateTimeNull) Then
                                        myGlobal = wsDelegate.UpdateStartDateTime(Nothing, WorkSessionIDAttribute)
                                    End If
                                End If
                            End If

                            'When process the START instruction finish then Sw automatically sends the first preparation
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_END
                            'When a process involves an instruction sending sequence automatic (for instance START (end) + NEXT PREP) change the AnalyzerIsReady value
                            If (Not myGlobal.HasError) Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartRunning, "END")
                                'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "CLOSED") 'AG 29/03/2012 - closed when running end not when start end
                            End If

                            'AG 08/11/2013 Task #1358 'TR 25/10/2013 - BT #1340
                            SetAllowScanInRunningValue(False) 'AllowScanInRunningAttribute = False

                            'AG 28/04/2014 - #1606 - StandBy -> Running and user clicks PAUSE button just after last STATUS with A:11
                            'Sw deleted the flag and when status with A:97 PAUSE_END received the proper business was not executed because the flag PAUSEprocess had been deleted!!!!
                            'CORRECTION: Do not clear the flag if instruction is still in queue!!! (means user has clicked the pause button but the instruction still has not been sent)
                            '
                            'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                            Dim clearPauseProcessFlag As Boolean = True
                            If myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE) Then
                                clearPauseProcessFlag = False
                            End If
                            If clearPauseProcessFlag Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                            End If
                            'AG 28/04/2014 - #1606

                            ContinueAlreadySentFlagAttribute = False
                            'If Fw say us Test preparation Accepted then the Sw has to mark the last execution send as INPROCESS!!
                            'NOTE: This process is performed when AnalyzerIsReady is FALSE

                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.TEST_PREPARATION_RECEIVED, _
                            GlobalEnumerates.AnalyzerManagerAx00Actions.PREDILUTED_TEST_RECEIVED, _
                            GlobalEnumerates.AnalyzerManagerAx00Actions.ISE_TEST_RECEIVED
                            myGlobal = MarkPreparationAccepted(Nothing)

                            'AG 07/06/2012 - Once the test is accepted search for next instruction to be sent in future
                            If (Not myGlobal.HasError AndAlso myNextPreparationToSendDS.nextPreparation.Rows.Count = 0) Then
                                Dim wellOffset As Integer = 0
                                If (pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.PREDILUTED_TEST_RECEIVED) Then
                                    wellOffset = WELL_OFFSET_FOR_PREDILUTION
                                ElseIf (pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.ISE_TEST_RECEIVED) Then
                                    'Different offset depending the last execution sample type
                                    If InStr(InstructionSentAttribute, "ISETEST;TI:1") > 0 Then
                                        'SER or PLM
                                        wellOffset = WELL_OFFSET_FOR_ISETEST_SERPLM
                                    ElseIf InStr(InstructionSentAttribute, "ISETEST;TI:2") > 0 Then
                                        'URI
                                        wellOffset = WELL_OFFSET_FOR_ISETEST_URI
                                    End If
                                End If

                                Dim reactRotorDlg As New ReactionsRotorDelegate
                                futureRequestNextWell = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + 1 + wellOffset, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)

                                myGlobal = SearchNextPreparation(Nothing, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then '(1)
                                    myNextPreparationToSendDS = DirectCast(myGlobal.SetDatos, AnalyzerManagerDS)
                                End If
                            End If
                            'AG 07/06/2012

                            'When Fw say us test preparation is near to be finished and Request = 1 then Sw has to search & send then next test
                            'TEST, PTEST, ISETEST, WRUN
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.TEST_PREPARATION_END, _
                                GlobalEnumerates.AnalyzerManagerAx00Actions.PREDILUTED_TEST_END, _
                                GlobalEnumerates.AnalyzerManagerAx00Actions.ISE_TEST_END, _
                                GlobalEnumerates.AnalyzerManagerAx00Actions.SKIP_END, _
                                GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_END
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION, True, Nothing, pNextWell)

                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_START, GlobalEnumerates.AnalyzerManagerAx00Actions.SKIP_START
                            'The well (cuvette) washings are required to be marked as already washed
                            If (pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_START) Then
                                Debug.Print("Setp 2 - ManageRunningStatus -> wellContaminatedWithWashSent = " & wellContaminatedWithWashSent)

                                If (wellContaminatedWithWashSent > 0) Then
                                    myGlobal = MarkWashWellContaminationRunningAccepted(Nothing)
                                End If
                            End If

                            'AG 07/06/2012 - Once the wash running or skip running is accepted search for next instruction to be sent in future
                            If (Not myGlobal.HasError AndAlso myNextPreparationToSendDS.nextPreparation.Rows.Count = 0) Then
                                Dim reactRotorDlg As New ReactionsRotorDelegate
                                futureRequestNextWell = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + 1, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)

                                myGlobal = SearchNextPreparation(Nothing, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then '(1)
                                    myNextPreparationToSendDS = DirectCast(myGlobal.SetDatos, AnalyzerManagerDS)
                                End If
                            End If
                            'AG 07/06/2012

                            'XB 15/10/2013 - BT #1318
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_START
                            'Fw inform us the analyzer start pausing the running mode (to allow scan rotors)
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString) = "INPROCESS") Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "INPROCESS")
                            End If
                            ContinueAlreadySentFlagAttribute = False ' TR 25/10/2013 #BT1340
                            pauseModeIsStarting = True 'AG 26/03/2014 - #1501 (Physics #48) inform pause mode is starting

                            'XB 15/10/2013 - BT #1318
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_END
                            'TR 21/10/2013 -Bug #1339 if the pause flag is null then update the attribute
                            If (String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "", False) <> 0) Then
                                'AG 08/11/2013 #1358
                                SetAllowScanInRunningValue(True) 'AllowScanInRunningAttribute = True
                            End If
                            'TR 21/10/2013 -Bug #1339 END.

                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")
                            If (String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "INPROCESS", False) = 0) Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "CLOSED")
                            End If

                            'AG 31/10/2013 #1342 - A previous END instruction sent is ignored once analyzer goes to pause mode
                            endRunAlreadySentFlagAttribute = False
                            If (String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess.ToString), "INPROCESS", False) = 0) Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "CLOSED")
                            End If
                            'AG 31/10/2013 #1342

                            'AG 27/11/2013 - Task #1397 - show auxiliary screen for recovery results
                            If (String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString), "INPROCESS", False) = 0) Then
                                SetAllowScanInRunningValue(True) 'AG 03/12/2013 - this code is required here for scenario: app restarts
                                'Update sensors for UI refresh - recovery results starts
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS, 1, True) 'Update sensors for UI refresh - recovery results starts (in pause mode, show message)
                            End If
                            'AG 27/11/2013

                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START
                            'Fw inform us the analyzer start ending the running mode
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess.ToString) = "INPROCESS") Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "INPROCESS")
                            End If


                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_END
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StopRunning, "END")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SoftwareWSonRUNNING, "CLOSED") 'AG 07/11/2014 - This flag was set to TRUE but never to FALSE except after results recover!!

                            If (String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess.ToString), "INPROCESS", False) = 0) Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "CLOSED")
                            End If

                            'Fw inform us the analyzer has finish the running mode
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS") Then
                                'Do nothing, the Start the recovery results process will be started when received action = 0
                                'Start the recovery results process
                                myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem)
                            Else
                                'SW sends an Standby
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)

                                'When a process involve an instruction sending sequence automatic (for instance RUNNING (end) + START) change the AnalyzerIsReady value
                                If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                                    SetAnalyzerNotReady()
                                End If
                            End If

                            'AG 20/02/2012 - When analyzer ABORT has finished then Sw automatically sends the STANDBY
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_END
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StopRunning, "END")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")

                            'Fw inform us the analyzer has finished the abort process 
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS") Then
                                'Do nothing, the Start the recovery results process will be started when received action = 0
                                'Start the recovery results process
                                myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem)
                            Else
                                'Sw send an Standby
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)

                                'When a process involve an instruction sending sequence automatic (for instance RUNNING (end) + START) change the AnalyzerIsReady value
                                If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                                    SetAnalyzerNotReady()
                                End If
                            End If

                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START
                            'XB 15/10/2013 - BT #1318
                            SetAllowScanInRunningValue(False) 'AG 08/11/2013 #1358    AllowScanInRunningAttribute = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "") 'AG 30/10/2013 - once the abort starts the end process does not apply

                            'Fw inform us the analyzer start the abort process (Sw has to save the flag into database)
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS") Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "INPROCESS")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                            End If

                            'AG 23/03/2012 - case change status not succeeded because some error codes appears
                            '(current status RUNNING but Fw informs action sleep end (status must be sleep) or standby end (status must be standby))
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.SLEEP_END, GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING, 1, True)

                            'Reset Flags
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.ResetFlags(Nothing, AnalyzerIDAttribute)
                            InitializeAnalyzerFlags(Nothing)
                            'AG 23/03/2012

                            'AG 28/08/2012 - If analyzer in running and no action means recovery results ready
                            'because Sw has reconnected once the reading has already finished
                        Case GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION
                            If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS") Then
                                'Start the recovery results process
                                myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem)
                            End If
                            'AG 28/08/2012

                            'AG 28/11/2013 - BT #1397
                        Case AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED
                            'If actions indicates barcode action in running but flag allow scan in running is false --> Set it to True
                            'This case is possible in re-connection in running pause while analyzer is scanning
                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso Not AllowScanInRunning Then
                                SetAllowScanInRunningValue(True)
                            End If
                            'AG 28/11/2013

                        Case AnalyzerManagerAx00Actions.SOUND_DONE
                            'AG 07/02/2014 - BT #1484 - Once the sound (on/off) is accepted search for next instruction to be sent in future
                            'Only if WS preparations not finished and not connection in process
                            If Not endRunAlreadySentFlagAttribute AndAlso Not abortAlreadySentFlagAttribute AndAlso Not PauseAlreadySentFlagAttribute AndAlso _
                                mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) <> "INPROCESS" Then
                                If (Not myGlobal.HasError AndAlso myNextPreparationToSendDS.nextPreparation.Rows.Count = 0) Then
                                    Dim reactRotorDlg As New ReactionsRotorDelegate
                                    futureRequestNextWell = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + 1, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)

                                    myGlobal = SearchNextPreparation(Nothing, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then '(1)
                                        myNextPreparationToSendDS = DirectCast(myGlobal.SetDatos, AnalyzerManagerDS)
                                    End If
                                End If
                            End If
                            'AG 07/02/2014

                    End Select

                    'There is some instruction in queue waiting to be sent
                    'Get his parameters, remove both from queue (instruction and parameters) and send it
                Else
                    Dim queuedParam As Object = Nothing
                    Dim InterruptPosition As Integer = 0

                    For InterruptPosition = 0 To myInstructionsQueue.Count - 1
                        If (myInstructionsQueue(InterruptPosition) = myInterruptInstruction) Then
                            If (myParamsQueue.Count >= InterruptPosition) Then
                                queuedParam = myParamsQueue(InterruptPosition)
                            End If
                            Exit For
                        End If
                    Next

                    If (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.STANDBY)

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ABORT And Not abortAlreadySentFlagAttribute) Then

                        ' XB 06/11/2013 - Remove END instructions from the queue when ABORT instruction is performed
                        RemoveItemFromQueue(AnalyzerManagerSwActionList.ENDRUN)

                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ABORT)

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN And Not endRunAlreadySentFlagAttribute) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.START) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.START)

                        If (Not myGlobal.HasError) Then
                            endRunAlreadySentFlagAttribute = False
                            abortAlreadySentFlagAttribute = False
                            PauseAlreadySentFlagAttribute = False ' XB 15/10/2013 - BT #1318
                        End If

                        'AG 30/10/2013 - Task #1342 - If user STOPS worksession in paused mode Sw sends 1st START and then add END instruction into queue
                        If stopRequestedByUserInPauseModeFlag Then
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("Once START instruction has been sent add the END instruction into queue", "AnalyzerManager.ManageRunningStatus", EventLogEntryType.Information, False)

                            ContinueAlreadySentFlagAttribute = True 'Inform here this attribute to make easier the presentation button actions refresh
                            stopRequestedByUserInPauseModeFlag = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                        End If
                        'AG 30/10/2013

                        ' XB 15/10/2013 - BT #1318
                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE And Not PauseAlreadySentFlagAttribute) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.PAUSE)

                        'AG 16/10/2013
                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST AndAlso AllowScanInRunning AndAlso Not queuedParam Is Nothing) Then
                        myBarcodeRequestDS = CType(queuedParam, AnalyzerManagerDS)
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)

                        ' XB 29/01/2014 - Re-Activate WatchDog timer - Task #1438
                        Debug.Print("******************************* WATCHDOG INTERVAL WOULD CHANGE TO [" & MaxWaitTime.ToString & "]")
                        Debug.Print("******************************* WATCHDOG ENABLE CHANGED RAISED TO [true]")
                        RaiseEvent WatchDogEvent(True)
                        ' XB 29/01/2014

                        'AG 26/01/2012
                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.SOUND) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SOUND)

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDSOUND)
                        'AG 26/01/2012

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.INFO) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.INFO, CInt(queuedParam)) 'AG 11/12/2012 add cint to the param for info instruction

                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.STATE) Then
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.STATE)

                        ' XB 23/10/2013 - Specific ISE commands are allowed in RUNNING (pause mode) - BT #1343
                    ElseIf (myInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD) Then
                        Dim myISECommand As ISECommandTO
                        myISECommand = CType(queuedParam, ISECommandTO)
                        If myISECommand.ISEMode <> GlobalEnumerates.ISEModes.None Then
                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ISE_CMD, myISECommand)
                        End If

                        If Not myGlobal.HasError Then
                            myGlobal = ISEAnalyzer.StartInstructionStartedTimer
                        End If
                        ' XB 23/10/2013

                    End If

                    If (Not myGlobal.HasError) Then
                        myInstructionsQueue.Remove(myInterruptInstruction) 'Remove instruction from queue list
                        If myParamsQueue.Count > InterruptPosition Then
                            myParamsQueue.RemoveAt(InterruptPosition)          'XB 21/10/2013 RemoveAt instead of Remove
                        End If
                        useRequestFlag = False
                        SetAnalyzerNotReady()
                    End If
                End If

                'Update internal flags. Basically used by the running normal business
                If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                    'Update analyzer session flags into DataBase
                    If (myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

                'BT #1355 - Verify if the new Alarm WS_PAUSE_MODE_WARN has to be added to the list of Analyzer Alarms
                If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                    Dim myAlarmStatusList As New List(Of Boolean)
                    Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)

                    PrepareLocalAlarmList(GlobalEnumerates.Alarms.WS_PAUSE_MODE_WARN, AllowScanInRunningAttribute, myAlarmList, myAlarmStatusList)
                    If (myAlarmList.Count > 0) Then myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)

                    'TR 22/10/2013 -BT #1353 
                    If (Not myGlobal.HasError) Then myGlobal = SendEndInstructionIfRequired()
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageRunningStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' StandBy business logic
        ''' </summary>
        ''' <param name="pAx00ActionCode"></param>
        ''' <param name="pNextWell" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG 03/11/2010
        ''' Modified by AG 02/03/2011 - add parameter pNextWell
        '''             XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
        '''             AG 14/11/2014 - BA-2065 Dynamic base line initial management (add cases FLIGHT_START and END)
        '''             IT 26/11/2014 - BA-2075 Modified the Warm up Process to add the FLIGHT process
        ''' </remarks>
        Private Function ManageStandByStatus(ByVal pAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions, ByVal pNextWell As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            Try
                Dim AlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim AlarmStatusList As New List(Of Boolean)
                Dim alarmStatus As Boolean = False 'By default no alarm
                Dim myAlarm As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE


                Select Case pAx00ActionCode
                    'The STANDBY instruction starts (prepare event for inform UI layer)
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_START
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WARMUP_STARTED, 1, True)

                        'AG 12/03/2012 - If exists remove the alarm REACTIONS ROTOR MISSING (only if current status is SLEEPING)
                        If AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                            PrepareLocalAlarmList(GlobalEnumerates.Alarms.REACT_MISSING_ERR, False, AlarmList, AlarmStatusList)

                            If AlarmList.Count > 0 Then
                                'myGlobal = ManageAlarms(Nothing, AlarmList, AlarmStatusList)
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    ' XBC 17/10/2012 - Alarms treatment for Service
                                    ' Not Apply
                                    'myGlobal = ManageAlarms_SRV(Nothing, AlarmList, AlarmStatusList)
                                Else
                                    myGlobal = ManageAlarms(Nothing, AlarmList, AlarmStatusList)
                                End If
                            End If
                        End If
                        'AG 12/03/2012

                        'PAUSED (uncomment these 2 lines) 'AG 20/03/2014 - #1547 Once the Standby instruction has been accepted reset these two flags
                        'abortAlreadySentFlagAttribute = False
                        'endRunAlreadySentFlagAttribute = False
                        'AG 20/03/2014 - #1547


                        'When the STANDBY instruction finish then Sw automatically sends the WASH instruction
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END

                        'AG 02/09/2012 - If recovery results in process and BAx00 becomes in StandBy execute the complete connection. Else execute previous code
                        'If user has press ABORT button 1st complete the wash and then execute the ProcessConnection
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) <> "INPROCESS" Then

                            'AG 07/03/2014 -integrate patches 'AG 18/02/2014 - #1513 - Do not abort the worksession after recover results
                            ''Mark work session as aborted
                            'Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                            'myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")
                            'AG 07/03/2014 -integrate patches

                            'AG 04/09/2012 - these 2 lines will be executed when the full conection finishes
                            'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")
                            ''Generate UI refresh for presentation - Inform the recovery results has finished!!
                            'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS, 0, True)
                            ProcessConnection(Nothing, True)

                        Else
                            'Previous code

                            Dim updateISEConsumptionFlag As Boolean = False
                            ' XBC 17/07/2012 - Estimated ISE Consumption by Firmware during WS
                            If Not ISEAnalyzer Is Nothing _
                               AndAlso ISEAnalyzer.IsISEModuleInstalled Then    ' XBC 02/08/2012 - correction to allow complete processing with INFO start
                                ISEAnalyzer.EstimatedFWConsumptionWS()

                                'AG 12/04/2012 - Update ISE consumptions if required
                                If ISEAnalyzer.IsCalAUpdateRequired Or ISEAnalyzer.IsCalBUpdateRequired Then
                                    updateISEConsumptionFlag = True
                                End If
                                'AG 12/04/2012
                            End If

                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS" Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "") 'Initialize the maneuvers during Wup process possibly affected by alarms
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartInstrument, "END") 'Once the new instruction has been sent update flags

                                'AG 20/03/2012 - New Fw disables info when leave running so Sw must activate it again
                                ''AG 29/09/2011 - Send a INFO instruction (Activate ANSINF instructions) during Wup
                                ''                Do not treat myGlobal.HasError
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                                '' We has to wait until receive a ANSINFO instruction ... if no bottle / deposit alarms then send the WASH, else show message and abort warmup process
                                ' ''Send a WASH instruction (Conditioning complete)
                                ''myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                                ''AG 29/09/2011

                                ''When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                '    SetAnalyzerNotReady()
                                '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartInstrument, "END") 'Once the new instruction has been sent update flags
                                'End If
                                updateISEConsumptionFlag = False 'AG 12/04/2012

                            ElseIf String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess.ToString), "INPROCESS", False) = 0 Then
                                'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "CLOSED")

                                'AG 12/04/2012 - Update ISE consumptions NO required -> finish process 
                                If Not updateISEConsumptionFlag Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption, "END")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "CLOSED")
                                End If
                                'AG 12/04/2012

                            ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                                'AG 20/02/2012 - If no bottle alarms then send the WASH complete instruction
                                'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "CLOSED")

                                'AG 20/03/2012 - New Fw disables info when leave running so Sw must activate it again
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                                ' We has to wait until receive a ANSINFO instruction ... if no bottle / deposit alarms then send the WASH, else show message and abort warmup process
                                ''Send a WASH instruction (Conditioning complete)
                                'AG 20/02/2012

                                'AG 12/04/2012 - Update ISE consumptions NO required -> finish process 
                                If Not updateISEConsumptionFlag Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption, "END")
                                End If
                                'AG 12/04/2012

                            End If


                            'AG 12/04/2012 - New Fw disables info when analyzer leaves running, so Sw has to activate info when standby end
                            If Not updateISEConsumptionFlag Then
                                AnalyzerIsInfoActivatedAttribute = 0
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                                'AG 29/03/2012 - this is not required with the INFO because it is an immediate instruction
                                ''When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                '    SetAnalyzerNotReady()
                                'End If
                                'AG 29/03/2012

                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption, "INI")
                                myGlobal = ISEAnalyzer.SaveConsumptions()

                                'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                'AG + XBC 28/09/2012 - also evaluate that any instruction has been sent before to set the analyzer not ready
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing AndAlso CType(myGlobal.HasError, Boolean) = True AndAlso ConnectedAttribute Then
                                    SetAnalyzerNotReady()
                                End If
                            End If
                            'AG 12/04/2012
                        End If 'AG 02/09/2012


                        'Ax00 STARTS the Washing cycles
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_STDBY_START
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "INI")

                        'Prepare structures to update data base (by now we have only update internal dictionary variable (mySessionFlags)
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "INPROCESS")


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WASHprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WASHprocess, "INPROCESS") 'Prepare structures to update data base


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess, "INPROCESS") 'Prepare structures to update data base


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.COND_WASHSTATIONprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.COND_WASHSTATIONprocess, "INPROCESS") 'Prepare structures to update data base

                        End If


                        'Ax00 ENDS the Washing cycles
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_STDBY_END
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "END")

                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WASHprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WASHprocess, "CLOSED")


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess, "CLOSED")


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.COND_WASHSTATIONprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.COND_WASHSTATIONprocess, "CLOSED")

                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then 'AG 20/02/2012
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "CLOSED")

                            'AG 04/09/2012 - When ABORT session finishes. Auto ABORT due to re-connection in running with incosistent data ... execute the full connection process
                            'Also when re-connection in running OK (flag RECOVERYRESULTS in process) but user press the ABORT button
                            If ForceAbortSessionAttr OrElse mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" Then
                                ProcessConnection(Nothing, True)
                            Else
                                ManageStandByStatus(GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END, CurrentWellAttribute) 'Call this method with this action code to update ISE consumption if needed
                            End If
                            'AG 04/09/2012

                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "INPROCESS" Then
                            'Send a SLEEP instruction 
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True)

                            'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                            If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()

                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess.ToString) = "INPROCESS" Then 'AG 08/03/2012
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "CLOSED")

                            'AG 24/05/2012
                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                            'Case Start instrument failed before wash was performed (for example no reactions missing)
                            'User execute the change reaction rotor: Sw instructions NRotor + Wash + Alight

                            'Send the ALIGHT
                            'Delete the current reactions rotor status (configuration) and crete a new one
                            Dim cfgAnReactionsRotor As New AnalyzerReactionsRotorDelegate
                            myGlobal = cfgAnReactionsRotor.ChangeRotorPerformed(Nothing, AnalyzerIDAttribute)

                            'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
                            If Not myGlobal.HasError Then
                                Dim ALightDelg As New WSBLinesDelegate
                                myGlobal = ALightDelg.ResetBLinesValues(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "")
                                If Not myGlobal.HasError Then
                                    'Once the conditioning is finished the Sw send an ALIGHT instruction 
                                    ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                                    'AG 12/09/2011 - when change rotor is performed the ALIGHt well starts in 1, ignore the well field in status instruction (RPalazon, JGelabert, STortosa)
                                    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, pNextWell)
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)

                                    'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If
                            End If
                            'AG 24/05/2012

                        End If

                        'Ax00 STARTS the go to SLEEP process
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.SLEEP_START
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SleepInstrument, "INI")

                        'Ax00 STARTS the go to RUN process
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.RUNNING_START
                        futureRequestNextWell = 0
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "INPROCESS")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.EnterRunning, "INI")

                        ' XBC 02/08/2012
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SoftwareWSonRUNNING, "INPROCESS")

                        'Ax00 STARTS the ALIGHT process
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.ALIGHT_START
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "INI")

                        'Ax00 STARTS the nrotor process (wash station control)
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")

                            'Prepare sensor values for future presentation refresh
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED, 0, False)
                        End If

                        'Ax00 ENDS the wash station control (up) (user can now change rotor using the change rotor utility)
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END
                        'Prepare DS for inform presentation layer
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED, 1, True)

                        'Ax00 STARTS the new reactions rotor detection
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.NEW_ROTOR_START
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "INI")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")

                            'Prepare sensor values for future presentation refresh
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED, 0, False)

                            'AG 12/03/2012 - If exists remove the alarm REACTIONS ROTOR MISSING
                            PrepareLocalAlarmList(GlobalEnumerates.Alarms.REACT_MISSING_ERR, False, AlarmList, AlarmStatusList)

                            If AlarmList.Count > 0 Then
                                'myGlobal = ManageAlarms(Nothing, AlarmList, AlarmStatusList)
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    ' XBC 17/10/2012 - Alarms treatment for Service
                                    ' Not Apply
                                    'myGlobal = ManageAlarms_SRV(Nothing, AlarmList, AlarmStatusList)
                                Else
                                    myGlobal = ManageAlarms(Nothing, AlarmList, AlarmStatusList)
                                End If
                            End If
                            'AG 12/03/2012

                        End If

                        'Ax00 ENDS the new reactions rotor detection
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.NEW_ROTOR_END
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "END")

                            'AG 24/05/2012 - If start instrument wash paused and wash not finished sent the wash
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "PAUSED" Then
                                ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Wash) 'BA-2075
                            Else
                                'Else Alight
                                'User execute the change reaction rotor when start instrument OK: Sw instructions NRotor + Alight

                                'Delete the current reactions rotor status (configuration) and crete a new one
                                Dim cfgAnReactionsRotor As New AnalyzerReactionsRotorDelegate
                                myGlobal = cfgAnReactionsRotor.ChangeRotorPerformed(Nothing, AnalyzerIDAttribute)

                                'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
                                If Not myGlobal.HasError Then
                                    Dim ALightDelg As New WSBLinesDelegate
                                    myGlobal = ALightDelg.ResetBLinesValues(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "")
                                    If Not myGlobal.HasError Then
                                        'Once the conditioning is finished the Sw send an ALIGHT instruction 
                                        ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                                        'AG 12/09/2011 - when change rotor is performed the ALIGHt well starts in 1, ignore the well field in status instruction (RPalazon, JGelabert, STortosa)
                                        'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, pNextWell)
                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)

                                        'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "INI")
                                            SetAnalyzerNotReady()
                                        End If
                                    End If
                                End If
                            End If
                            'AG 24/05/2012

                        End If

                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "INPROCESS")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "") 'Reset Flag


                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END
                        'AG 08/03/2012
                        'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "CLOSED")

                        ' We has to wait until receive a ANSINFO instruction ... if no bottle / deposit alarms then send the WASH (conditioning), else show message and abort wash process in recover
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                        'AG 29/03/2012 - this is not required with the INFO because it is an immediate instruction
                        ''When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                        'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                        '    SetAnalyzerNotReady()
                        'End If
                        'AG 08/03/2012

                        'AG 23/03/2012 - case change status not succeeded because some error codes appears
                        '(curent status STANDBY but Fw informs action sleep end (status must be sleep) or running end (status must be running)
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.SLEEP_END, GlobalEnumerates.AnalyzerManagerAx00Actions.RUNNING_END
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING, 1, True)

                        'Reset Flags
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.ResetFlags(Nothing, AnalyzerIDAttribute)
                        InitializeAnalyzerFlags(Nothing)
                        'AG 23/03/2012

                        'AG 14/11/2014 BA-2065
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.FLIGHT_ACTION_START

                        'IT 26/11/2014 - BA-2075 INI
                        Select Case CurrentInstructionAction
                            Case GlobalEnumerates.InstructionActions.FlightFilling
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "INI")
                            Case GlobalEnumerates.InstructionActions.FlightReading
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "INI")
                            Case GlobalEnumerates.InstructionActions.FlightEmptying
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "INI")
                        End Select
                        'IT 26/11/2014 - BA-2075 END

                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.FLIGHT_ACTION_DONE

                        'IT 26/11/2014 - BA-2075 INI
                        Select Case CurrentInstructionAction
                            'Fill rotor finishes
                            Case GlobalEnumerates.InstructionActions.FlightFilling
                                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill.ToString) = "INI" Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "END")
                                    CurrentInstructionAction = InstructionActions.None
                                End If
                                'Read rotor finishes
                            Case GlobalEnumerates.InstructionActions.FlightReading
                                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read.ToString) = "INI" Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "END")
                                    CurrentInstructionAction = InstructionActions.None
                                End If
                                'Empty rotor finishes
                            Case GlobalEnumerates.InstructionActions.FlightEmptying
                                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty.ToString) = "INI" Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "END")
                                    CurrentInstructionAction = InstructionActions.None
                                    ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Finalize)
                                End If
                        End Select
                        'IT 26/11/2014 - BA-2075 END
                    Case Else

                End Select

                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

                AlarmStatusList = Nothing
                AlarmList = Nothing

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageStandByStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Sleep business logic
        ''' </summary>
        ''' <param name="pAx00ActionCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG 28/02/2011 - Tested PENDING
        ''' Modified by XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
        ''' </remarks>
        Private Function ManageSleepStatus(ByVal pAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions) As GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
            Dim myGlobal As New GlobalDataTO

            Try
                'Dim resetFlagsValue As Boolean = True

                Select Case pAx00ActionCode
                    'When the STANDBY instruction starts then update internal Sw flags
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_START
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartInstrument, "INI")
                        'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WupStartDateTime, Now.ToString)
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WupStartDateTime, Now.ToString(CultureInfo.InvariantCulture))

                        'Delete flags for SLEEP status
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SleepInstrument, "")

                        'Delete flags for RUNNING status
                        '...
                        '...

                        'resetFlagsValue = False

                        'Ax00 enters in SLEEP status
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.SLEEP_END
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SleepInstrument, "END")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "CLOSED")

                        myAlarmListAttribute.Clear() 'AG 23/05/2012 - In sleeping Remove all alarms

                        'Delete flags for STANDBY, RUNNING status
                        '...
                        '...

                        'resetFlagsValue = False

                        'AG 16/04/2012 - Stop the sensor information instructions
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                        SetAnalyzerNotReady() 'analyzer is not ready to perform anything but CONNECT
                        'AG 16/04/2012

                        'AG 03/10/2011 - Set to false the Connected attribute + prepare ui refresh
                        ConnectedAttribute = False
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True)
                        'AG 03/10/2011 

                        'AG 23/03/2012 - case change status not succeeded because some error codes appears
                        '(curent status SLEEP but Fw informs action standby end (status must be stand by) or running end (status must be running)
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END, GlobalEnumerates.AnalyzerManagerAx00Actions.RUNNING_END
                        'resetFlagsValue = True
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING, 1, True)
                        'AG 23/03/2012

                    Case Else

                End Select

                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

                'AG 20/06/2012 - Reset flags is moved into ProcessStatusReceived method case SLEEPING because while connection process no instruction are treated
                'but this business is required
                ''AG 17/10/2011 - reset internal flags when analyzer is sleeping and no action has been performed
                'If Not myGlobal.HasError AndAlso resetFlagsValue Then
                '    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                '    myGlobal = myFlagsDelg.ResetFlags(Nothing, AnalyzerIDAttribute)
                '    InitializeAnalyzerFlags(Nothing)
                'End If
                'AG 20/06/2012 - 'AG 17/10/2011

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageSleepStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Scripts Manage business logic
        ''' </summary>
        ''' <param name="pAx00ActionCode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 16/11/2010
        ''' </remarks>
        Private Function ManageFwCommandAnswer(ByVal pAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions, _
                                               ByVal pResponseValue As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Select Case pAx00ActionCode
                    Case GlobalEnumerates.AnalyzerManagerAx00Actions.COMMAND_END
                        RaiseEvent ReceptionFwScriptEvent(InstructionReceivedAttribute, pResponseValue, True)
                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageFwCommandAnswer", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Some analyzer status changes requires a special business to be performed
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewStatusValue"></param>
        ''' <returns></returns>
        ''' <remarks>AG 26/09/2012 - modify: in standby WellContent must be "E" or "C"</remarks>
        Private Function ExecuteSpecialBusinessOnAnalyzerStatusChanges(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                                       ByVal pNewStatusValue As GlobalEnumerates.AnalyzerManagerStatus) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'From RUNNING to STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And pNewStatusValue = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            '1) Maybe there are some messages that can be shown
                            If WELLbaselineParametersFailuresAttribute Then
                                'Prepare DS for inform presentation
                                'Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                                resultData = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, GlobalEnumerates.Alarms.BASELINE_WELL_WARN.ToString, True)
                            End If

                            'AG 26/09/2012 -NEW -Simplification: When analyzer enters in StandBy repaint the whole reactions rotor, not only the WashStation wells
                            '2) Update the reactions rotor table (WellContent = 'E' or 'C' for all wells, we are in standby)
                            Dim reactionsDelegate As New ReactionsRotorDelegate

                            'AG 14/11/2014 BA-2065 REFACTORING
                            'resultData = reactionsDelegate.GetAllWellsLastTurn(dbConnection, AnalyzerIDAttribute)
                            'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            '    Dim wellsDS As ReactionsRotorDS
                            '    wellsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            '    'All wells with WellContent = 'W' changes to WellContent = 'E' or to 'C'
                            '    'resultData = reactionsDelegate.SetToEmptyTheWellsInWashStation(dbConnection, AnalyzerIDAttribute)
                            '    Dim contaminatedWellsDS As New ReactionsRotorDS
                            '    resultData = reactionsDelegate.AsignFinalValuesAfterLeavingRunning(dbConnection, AnalyzerIDAttribute, wellsDS)
                            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            '        contaminatedWellsDS = CType(resultData.SetDatos, ReactionsRotorDS)
                            '    End If

                            '    'Read again the complete current reactions rotor (last turn)
                            '    resultData = reactionsDelegate.GetAllWellsLastTurn(dbConnection, AnalyzerIDAttribute)
                            '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            '        wellsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            '        'Finally prepare DS for inform presentation with the wells inside Washing Station when Running has finished
                            '        Dim newWellsDS As New ReactionsRotorDS
                            '        For Each item As ReactionsRotorDS.twksWSReactionsRotorRow In wellsDS.twksWSReactionsRotor.Rows
                            '            item.BeginEdit()
                            '            'WellContent must be 'E' (empty) or 'C' (contaminated)
                            '            If item.WellContent <> "E" AndAlso item.WellContent <> "C" Then item.WellContent = "E"

                            '            'WellStatus must be 'R' (ready) or 'X' (rejected)
                            '            If item.WellStatus <> "R" AndAlso item.WellStatus <> "X" Then item.WellStatus = "R"
                            '            item.EndEdit()

                            '            newWellsDS.twksWSReactionsRotor.ImportRow(item)
                            '        Next
                            '        newWellsDS.AcceptChanges()
                            '        resultData = PrepareUIRefreshEventNum3(dbConnection, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, newWellsDS, True) 'AG 05/06/2012 - In this case use the Main treat refreshDS because we have leave Running mode
                            '    End If
                            'End If
                            ''AG 26/09/2012 -NEW
                            resultData = reactionsDelegate.RepaintAllReactionsRotor(dbConnection, AnalyzerIDAttribute)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                Dim newWellsDS As New ReactionsRotorDS
                                newWellsDS = DirectCast(resultData.SetDatos, ReactionsRotorDS)
                                resultData = PrepareUIRefreshEventNum3(dbConnection, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, newWellsDS, True) 'AG 05/06/2012 - In this case use the Main treat refreshDS because we have leave Running mode
                            End If
                            'AG 14/11/2014 REFACTORING

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                'resultData.SetDatos = <value to return; if any>
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If


                            'AG 28/06/2012 - If some ANSPHR instruction continues in buffer treat them now!!
                            Dim pendingANSPHRForTreat As Integer = 0
                            SyncLock lockThis
                                pendingANSPHRForTreat = bufferANSPHRReceived.Count
                            End SyncLock

                            Dim pendingInstruction As New List(Of InstructionParameterTO)
                            While pendingANSPHRForTreat > 0
                                SyncLock lockThis
                                    pendingInstruction = bufferANSPHRReceived(0)
                                    processingLastANSPHRInstructionFlag = True
                                End SyncLock

                                Application.DoEvents() 'This line requires import windows forms
                                resultData = ProcessANSPHRInstruction(pendingInstruction)

                                SyncLock lockThis
                                    If bufferANSPHRReceived.Count > 0 Then
                                        bufferANSPHRReceived.RemoveRange(0, 1)
                                    End If
                                    processingLastANSPHRInstructionFlag = False 'Inform NO readings received are in process
                                End SyncLock

                                pendingANSPHRForTreat = pendingANSPHRForTreat - 1
                            End While
                            'AG 28/06/2012

                        End If

                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                            'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                            ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637
                            RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                        End If

                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ExecuteSpecialBusinessOnAnalyzerStatusChanges", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' In general during Running the instructions in queue are priority than treat the received STATUS instruction
        ''' But there are several exceptions
        '''              1) If no instructions in queue: Treat the received STATUS instruction
        '''              2) Some queued instructions: STANDBY, ABORT, END, START, PAUSE, BARCODE (in Running) are priority over ALL received actions
        '''              3) Some received action values in STATUS are priority over SOUND, INFO and STATE
        '''              4) Queued instructions: SOUND, INFO and STATUS are priority over the others action values not in 3)
        ''' </summary>
        ''' <param name="pInterruptInstruction"></param>
        ''' <param name="pAx00ActionCode"></param>
        ''' <returns>New AnalyzerManagerSwActionList (default value the same as parameter)</returns>
        ''' <remarks>AG 10/12/2012</remarks>
        Private Function TreatQueueExceptionsInRunning(ByVal pInterruptInstruction As GlobalEnumerates.AnalyzerManagerSwActionList, ByVal pAx00ActionCode As GlobalEnumerates.AnalyzerManagerAx00Actions) As GlobalEnumerates.AnalyzerManagerSwActionList
            Dim actionToReturn As GlobalEnumerates.AnalyzerManagerSwActionList = pInterruptInstruction 'Default value the same as in parameter
            Try
                If pInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.SOUND OrElse _
                  pInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND OrElse _
                  pInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.INFO OrElse _
                  pInterruptInstruction = GlobalEnumerates.AnalyzerManagerSwActionList.STATE Then

                    'When the queue actions is SOUND or ENDSOUND or INFO or STATE there are several actions that can not be lost. They are priority!!!
                    'In these cases ignore the queue in current cycle
                    If pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.RUNNING_END OrElse _
                       pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_END OrElse _
                       pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_END OrElse _
                       pAx00ActionCode = GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_END Then

                        actionToReturn = GlobalEnumerates.AnalyzerManagerSwActionList.NONE

                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.TreatQueueExceptions", EventLogEntryType.Error, False)
            End Try
            Return actionToReturn
        End Function

        ''' <summary>
        ''' Method incharge to send the END Istruction in case there are some pending alarms that require
        ''' to send the END instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 22/10/2013 BUG #1353
        ''' MODIFIED BY: TR 29/11/2013 BT #1411 Do not send END Instruction while the flag AbortProcess is INPROCESS 
        ''' AG 27/03/2014 - #1501 do not evaluate alarms requiring END instruction if analyzer is in pause or the pause mode is starting!!
        ''' </remarks>
        Private Function SendEndInstructionIfRequired() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Validate the abort Instruction has not been sent.
                If Not abortAlreadySentFlagAttribute AndAlso _
                    mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString()) <> "INPROCESS" Then

                    If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting Then 'AG 27/03/2014 - #1501
                        'Validate if exist alarms that require End Instruction.
                        If ExistSomeAlarmThatRequiresStopWS() Then
                            'Send the end Instruction.
                            If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                                Not endRunAlreadySentFlagAttribute AndAlso AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists

                                ' myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)

                                '                            If Not myGlobalDataTO.HasError AndAlso ConnectedAttribute Then
                                'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                'End If

                                If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                    myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                    myParamsQueue.Add("")
                                    'Not inform this flag here. It will be informed once really sent
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                End If

                            End If
                        End If
                    End If 'AG 27/03/2014 - #1501

                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SendEndInstruction", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Sw has to treat the pause mode as a analyzer status change. So it is better to replace all places
        ''' where the variable AllowScanInRunningAttribute is set/reset this value and use this new method
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <remarks>
        ''' AG 08/11/2013 - creation
        ''' AG 19/11/2013 - #1375 fix issue
        ''' AG 22/11/2013 - Task #1397 - inform the pause mode also to the application layer
        ''' AG 10/12/2013 - Task #1397 move the 22/10/2013 code out the main IF
        ''' </remarks>
        Private Sub SetAllowScanInRunningValue(ByVal pValue As Boolean)
            Try
                If AllowScanInRunningAttribute <> pValue Then
                    'Set the new value
                    AllowScanInRunningAttribute = pValue

                    'Update flags if pause is achieved during running initialization
                    If pValue AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) = "INI" Then
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        Dim resultData As New GlobalDataTO

                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartRunning, "END")
                        resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If

                    If pValue Then pauseModeIsStarting = False 'AG 26/03/2014 - #1501 (Physics #48) when pause mode is achieved, reset the flag starting in pause to FALSE

                    'Generate refresh event similiar to an analyzer status change (only in running mode)
                    If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED, 1, True) 'Prepare UI refresh event when analyzer status changes
                    End If
                End If

                'AG 10/12/2013
                'AG 27/11/2013 - Task #1397 - Inform the app layer that when re-connection Analzyer was in pause mode
                'The well asked must be different from the normal running re-connection
                If pValue AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" Then
                    AppLayer.RecoveryResultsInPause = True
                End If
                'AG 10/12/2013

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SetAllowScanInRunningValue", EventLogEntryType.Error, False)
            End Try

        End Sub

#End Region

#Region "Status Flags management Methods"

        ''' <summary>
        ''' Read the flags by analyzer. If not exists then:
        ''' 1) Read exists flags from the previous analyzer ID
        '''    1.1) Exists: Copy flags from previous analyzerID, remove flags from previous analyzerID 
        '''    1.2) Not exists: Create them (from tfmwFlags table)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pPreviousAnalyzerID" ></param>
        ''' <remarks>AG 28/02/2011 Created
        ''' AG 10/03/2011 - add business for the case: If new Sw versions define more flags update the tcfgAnalyzerManagerFlags table with the new flags
        ''' AG 13/03/2012 - define as public
        ''' AG 21/06/2012 - add optional parameter
        ''' XB 03/05/2013 - Copy current flags state to the new Analyzer ID connected in case of change it
        ''' </remarks>
        Public Sub InitializeAnalyzerFlags(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pPreviousAnalyzerID As String = "") Implements IAnalyzerEntity.InitializeAnalyzerFlags
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim flagsDefinition As New SwFlagsDelegate

                        'Read table tfmwFlags (preloaded flags)
                        resultData = flagsDefinition.ReadAll(dbConnection)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myPreloadedDS As New SwFlagsDS
                            myPreloadedDS = CType(resultData.SetDatos, SwFlagsDS)

                            Dim flagsDelg As New AnalyzerManagerFlagsDelegate
                            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                            'Read table twksAnalyzerManagerFlags (filtering by current AnalyzerID)
                            resultData = flagsDelg.ReadByAnalyzerID(dbConnection, AnalyzerIDAttribute)
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                myAnalyzerFlagsDS = CType(resultData.SetDatos, AnalyzerManagerFlagsDS)

                                'If current AnalyzerID has no flags defined ... create ALL of them (from previous analyzerID or from preloaded table)
                                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count = 0 Then

                                    'Read flags from previous analyzer 
                                    Dim myPreviousAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                    If pPreviousAnalyzerID <> "" Then
                                        resultData = flagsDelg.ReadByAnalyzerID(dbConnection, pPreviousAnalyzerID)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            myPreviousAnalyzerFlagsDS = CType(resultData.SetDatos, AnalyzerManagerFlagsDS)
                                        End If
                                    End If

                                    'Create and initialize flags for the current AnalyzerID in table DataBase table
                                    Dim myAnalyzerFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow

                                    'If previous analyzer id has flags copy from it
                                    If myPreviousAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                        For Each previousFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In myPreviousAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows
                                            myAnalyzerFlagRow = myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                                            myAnalyzerFlagRow.BeginEdit()
                                            myAnalyzerFlagRow.AnalyzerID = AnalyzerIDAttribute
                                            myAnalyzerFlagRow.FlagID = previousFlagRow.FlagID
                                            If Not previousFlagRow.IsValueNull Then myAnalyzerFlagRow.Value = previousFlagRow.Value Else myAnalyzerFlagRow.SetValueNull()
                                            myAnalyzerFlagRow.EndEdit()
                                            myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(myAnalyzerFlagRow)
                                        Next

                                        'Once copied remove flags for the previous analyzer
                                        resultData = flagsDelg.ResetFlags(dbConnection, pPreviousAnalyzerID, False)

                                        'Copy from preloaded 
                                    Else
                                        For Each preloadedFlagRow As SwFlagsDS.tfmwSwFlagsRow In myPreloadedDS.tfmwSwFlags.Rows
                                            myAnalyzerFlagRow = myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                                            myAnalyzerFlagRow.BeginEdit()
                                            myAnalyzerFlagRow.AnalyzerID = AnalyzerIDAttribute
                                            myAnalyzerFlagRow.FlagID = preloadedFlagRow.FlagID
                                            myAnalyzerFlagRow.SetValueNull()
                                            myAnalyzerFlagRow.EndEdit()
                                            myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(myAnalyzerFlagRow)
                                        Next
                                    End If

                                    myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AcceptChanges()
                                    resultData = flagsDelg.Create(dbConnection, myAnalyzerFlagsDS)

                                Else
                                    ' XB 03/05/2013 - Copy current flags state to the new Analyzer ID connected in case of change it
                                    'Read flags from previous analyzer 
                                    If pPreviousAnalyzerID <> "" Then
                                        Dim myPreviousAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                        resultData = flagsDelg.ReadByAnalyzerID(dbConnection, pPreviousAnalyzerID)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            myPreviousAnalyzerFlagsDS = CType(resultData.SetDatos, AnalyzerManagerFlagsDS)

                                            If myPreviousAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then       ' XB 15/05/2013 - sometimes ReadByAnalyzerID returns a empty DS
                                                'Create and initialize flags for the current AnalyzerID in table DataBase table
                                                Dim myAnalyzerFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow

                                                ' Clear all previous flags data
                                                myAnalyzerFlagsDS.Clear()
                                                resultData = flagsDelg.Delete(dbConnection, AnalyzerIDAttribute)
                                                If Not resultData.HasError Then
                                                    ' Copy current flags data from previous analyzer
                                                    If myPreviousAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                        For Each previousFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In myPreviousAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows
                                                            myAnalyzerFlagRow = myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                                                            myAnalyzerFlagRow.BeginEdit()
                                                            myAnalyzerFlagRow.AnalyzerID = AnalyzerIDAttribute
                                                            myAnalyzerFlagRow.FlagID = previousFlagRow.FlagID
                                                            If Not previousFlagRow.IsValueNull Then myAnalyzerFlagRow.Value = previousFlagRow.Value Else myAnalyzerFlagRow.SetValueNull()
                                                            myAnalyzerFlagRow.EndEdit()
                                                            myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(myAnalyzerFlagRow)
                                                        Next

                                                        'Once copied remove flags for the previous analyzer
                                                        resultData = flagsDelg.ResetFlags(dbConnection, pPreviousAnalyzerID, False)
                                                    End If

                                                    myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AcceptChanges()
                                                    resultData = flagsDelg.Create(dbConnection, myAnalyzerFlagsDS)
                                                End If
                                            End If
                                        End If
                                    End If
                                    ' XB 03/05/2013 
                                End If

                                'AG 10/03/2011 - If new Sw versions define more flags .... update the tcfgAnalyzerManagerFlags table with the new flags
                                If myPreloadedDS.tfmwSwFlags.Rows.Count > myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count Then

                                    'Create and initialize flags the new flags for the current AnalyzerID in table DataBase table
                                    Dim newFlagsDS As New AnalyzerManagerFlagsDS
                                    Dim linqRes As List(Of AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow)

                                    For Each preloadedFlagRow As SwFlagsDS.tfmwSwFlagsRow In myPreloadedDS.tfmwSwFlags.Rows
                                        'Search each flag into flags defined for the current AnalyzerID
                                        linqRes = (From a As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags _
                                                   Where a.FlagID = preloadedFlagRow.FlagID Select a).ToList

                                        'If no exist add into internal Data Set and create into DataBase
                                        If linqRes.Count = 0 Then
                                            'Update internal DS
                                            Dim myAnalyzerFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
                                            myAnalyzerFlagRow = myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                                            myAnalyzerFlagRow.BeginEdit()
                                            myAnalyzerFlagRow.AnalyzerID = AnalyzerIDAttribute
                                            myAnalyzerFlagRow.FlagID = preloadedFlagRow.FlagID
                                            myAnalyzerFlagRow.SetValueNull()
                                            myAnalyzerFlagRow.EndEdit()
                                            myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(myAnalyzerFlagRow)

                                            'Update DS to create new records in Database
                                            Dim rowToCreate As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
                                            rowToCreate = newFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                                            rowToCreate.BeginEdit()
                                            rowToCreate.AnalyzerID = AnalyzerIDAttribute
                                            rowToCreate.FlagID = preloadedFlagRow.FlagID
                                            rowToCreate.SetValueNull()
                                            rowToCreate.EndEdit()
                                            newFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(rowToCreate)
                                        End If

                                    Next
                                    myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.AcceptChanges()
                                    newFlagsDS.tcfgAnalyzerManagerFlags.AcceptChanges()
                                    resultData = flagsDelg.Create(dbConnection, newFlagsDS)

                                    linqRes = Nothing 'AG 02/08/2012 - free memory
                                End If

                            End If

                            'Initialize mySessionFlags structure variable
                            For Each analyzerFlagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow In myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows

                                ''PENDING TO APPROVAL
                                ''SG 27/09/2012 - not to load active flags if ServiceSW (not used in service SW)
                                'If Not myApplicationName.ToUpper.Contains("SERVICE") Then
                                If Not analyzerFlagRow.IsValueNull Then
                                    mySessionFlags(analyzerFlagRow.FlagID) = analyzerFlagRow.Value
                                Else
                                    mySessionFlags(analyzerFlagRow.FlagID) = ""
                                End If
                                '        Else
                                '        mySessionFlags(analyzerFlagRow.FlagID) = ""
                                '        End If
                                ''end SG 27/09/2012
                            Next

                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeAnalyzerFlags", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            'Return resultData
        End Sub


        ''' <summary>
        ''' Read the current analyzer settings for the AnalyzerIDAttribute. If not exist the they are created
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <remarks>AG 29/11/2011</remarks>
        Private Sub InitializeAnalyzerSettings(ByVal pDbConnection As SqlClient.SqlConnection)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                        Dim myDS As New AnalyzerSettingsDS

                        'The current AnalyzerIdAttribute has settings in database? Yes do nothing. No create
                        resultData = myAnalyzerSettingsDelegate.ReadAll(dbConnection, AnalyzerIDAttribute)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, AnalyzerSettingsDS)
                            If myDS.tcfgAnalyzerSettings.Rows.Count = 0 Then
                                'If not exists: Read the MasterData settings and create for AnalyzerIdAttribute
                                resultData = myAnalyzerSettingsDelegate.ReadAll(dbConnection, "MasterData")

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myDS = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                    If myDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                        For Each row As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In myDS.tcfgAnalyzerSettings.Rows
                                            row.BeginEdit()
                                            row.AnalyzerID = AnalyzerIDAttribute
                                            row.EndEdit()
                                        Next
                                        myDS.AcceptChanges()
                                        resultData = myAnalyzerSettingsDelegate.Create(dbConnection, myDS)
                                    End If
                                End If

                            End If
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeAnalyzerSettings", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub

        ''' <summary>
        ''' Read the current analyzer led positions for the AnalyzerIDAttribute. If not exist the they are created
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <remarks>AG 16/12/2011</remarks>
        Private Sub InitializeAnalyzerLedPositions(ByVal pDbConnection As SqlClient.SqlConnection)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ledPosDelegate As New AnalyzerLedPositionsDelegate
                        Dim myDS As New AnalyzerLedPositionsDS

                        'The current AnalyzerIdAttribute has settings in database? Yes do nothing. No create
                        resultData = ledPosDelegate.GetAllWaveLengths(dbConnection, AnalyzerIDAttribute)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                            If myDS.tcfgAnalyzerLedPositions.Rows.Count = 0 Then
                                'If not exists: Read the MasterData settings and create for AnalyzerIdAttribute
                                resultData = ledPosDelegate.GetAllWaveLengths(dbConnection, "MasterData")

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                                    If myDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        For Each row As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow In myDS.tcfgAnalyzerLedPositions.Rows
                                            row.BeginEdit()
                                            row.AnalyzerID = AnalyzerIDAttribute
                                            row.EndEdit()
                                        Next
                                        myDS.AcceptChanges()
                                        resultData = ledPosDelegate.Create(dbConnection, myDS)
                                    End If
                                End If

                            End If
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeAnalyzerLedPositions", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub

        ''' <summary>
        ''' Read the current analyzer ISE Module information by the AnalyzerIDAttribute. If not exist the they are created
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <remarks>SGM 24/01/2012</remarks>
        Private Sub InitializeISEInformation(ByVal pDbConnection As SqlClient.SqlConnection)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISEInfoDelegate As New ISEDelegate
                        Dim myDS As New ISEInformationDS

                        'The current AnalyzerIdAttribute has settings in database? Yes do nothing. No create
                        resultData = myISEInfoDelegate.ReadAllInfo(dbConnection, AnalyzerIDAttribute)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, ISEInformationDS)
                            If myDS.tinfoISE.Rows.Count = 0 Then
                                'If not exists: Read the MasterData settings and create for AnalyzerIdAttribute
                                resultData = myISEInfoDelegate.ReadAllInfo(dbConnection, "MasterData")

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myDS = CType(resultData.SetDatos, ISEInformationDS)
                                    If myDS.tinfoISE.Rows.Count > 0 Then
                                        For Each row As ISEInformationDS.tinfoISERow In myDS.tinfoISE.Rows
                                            row.BeginEdit()
                                            row.AnalyzerID = AnalyzerIDAttribute
                                            row.EndEdit()
                                        Next
                                        myDS.AcceptChanges()
                                        resultData = myISEInfoDelegate.CreateNewMasterData(dbConnection, myDS)
                                    End If
                                End If

                            End If
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            ''SGM 17/02/2012 Initialization of ISE Manager
                            'If ISEAnalyzer Is Nothing Then
                            '    ISEAnalyzer = New ISEManager(Me, MyClass.AnalyzerIDAttribute, MyClass.myAnalyzerModel)
                            'End If
                            ''end SGM 17/02/2012
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeISEInformation", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub

        ''' <summary>
        ''' Read the current analyzer FW Adjustments by the AnalyzerIDAttribute. If not exist the they are created
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <remarks>SGM 24/01/2012</remarks>
        Private Sub InitializeFWAdjustments(ByVal pDbConnection As SqlClient.SqlConnection)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAdjDelegate As New DBAdjustmentsDelegate
                        Dim myDS As New SRVAdjustmentsDS

                        'The current AnalyzerIdAttribute has settings in database? Yes do nothing. No create
                        resultData = myAdjDelegate.ReadAdjustmentsFromDB(dbConnection, AnalyzerIDAttribute)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                            If myDS.srv_tfmwAdjustments.Rows.Count = 0 Then
                                'If not exists: Read the MasterData settings and create for AnalyzerIdAttribute
                                resultData = myAdjDelegate.ReadAdjustmentsFromDB(dbConnection, "MasterData")

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                                    If myDS.srv_tfmwAdjustments.Rows.Count > 0 Then
                                        For Each row As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myDS.srv_tfmwAdjustments.Rows
                                            row.BeginEdit()
                                            row.AnalyzerID = AnalyzerIDAttribute
                                            row.FwVersion = FwVersionAttribute
                                            row.EndEdit()
                                        Next
                                        myDS.AcceptChanges()
                                        resultData = myAdjDelegate.CreateMasterData(dbConnection, myDS)
                                    End If
                                End If

                            End If
                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeFWAdjustments", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub

        ''' <summary>
        ''' Update internal session flags variable for UserSw and prepare DS for update database flags table
        ''' </summary>
        ''' <param name="pFlagsDS"></param>
        ''' <param name="pFlagCode"></param>
        ''' <param name="pNewValue"></param>
        ''' <remarks>AG 01/03/2011 - Tested PENDING</remarks>
        Private Sub UpdateSessionFlags(ByRef pFlagsDS As AnalyzerManagerFlagsDS, ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags, _
                                            ByVal pNewValue As String)
            Try
                'Update dictionary flags variables
                mySessionFlags(pFlagCode.ToString) = pNewValue

                'Add row into Dataset to Update
                Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
                flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                With flagRow
                    .BeginEdit()
                    .AnalyzerID = AnalyzerIDAttribute
                    .FlagID = pFlagCode.ToString
                    If pNewValue <> "" Then
                        .Value = pNewValue
                    Else
                        .SetValueNull()
                    End If
                    .EndEdit()
                End With
                pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
                pFlagsDS.AcceptChanges()


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateSessionFlags", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Opens a way to allow set flags values from another layer
        ''' </summary>
        ''' <param name="pFlagCode"></param>
        ''' <param name="pNewValue"></param>
        ''' <remarks>Created by XB 06/02/2014</remarks>
        Public Function SetSessionFlags(ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags, ByVal pNewValue As String) As GlobalDataTO Implements IAnalyzerEntity.SetSessionFlags
            Dim resultData As GlobalDataTO = Nothing
            Try
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                UpdateSessionFlags(myAnalyzerFlagsDS, pFlagCode, pNewValue)

                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SetSessionFlags", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CreateNewFlag()
            Try

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CreateNewFlag", EventLogEntryType.Error, False)

            End Try

        End Sub

        ''' <summary>
        ''' Read the settings for the AnalyzerID Generic and copy it 
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <remarks>XBC 11/06/2012</remarks>
        Private Sub CopyAnalyzerSettings(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String)
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                        Dim myAnalyzerDelegate As New AnalyzersDelegate
                        Dim myDS As New AnalyzersDS
                        Dim myDSSettings As New AnalyzerSettingsDS

                        'The pAnalyzerID has settings in database? Yes do nothing. No copy from Generic Analyzer
                        resultData = myAnalyzerSettingsDelegate.ReadAll(dbConnection, pAnalyzerID)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            myDSSettings = CType(resultData.SetDatos, AnalyzerSettingsDS)
                            If myDSSettings.tcfgAnalyzerSettings.Rows.Count = 0 Then
                                'If not exists: Read the Generic settings and create for pAnalyzerID

                                resultData = myAnalyzerDelegate.GetAnalyzerGeneric(dbConnection)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myDS = CType(resultData.SetDatos, AnalyzersDS)
                                    If myDS.tcfgAnalyzers.Rows.Count > 0 Then

                                        'The AnalyzerId Generic has settings in database? Yes do nothing. No create
                                        resultData = myAnalyzerSettingsDelegate.ReadAll(dbConnection, myDS.tcfgAnalyzers(0).AnalyzerID.ToString)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            myDSSettings = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                            If myDSSettings.tcfgAnalyzerSettings.Rows.Count > 0 Then

                                                For Each row As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In myDSSettings.tcfgAnalyzerSettings.Rows
                                                    row.BeginEdit()
                                                    row.AnalyzerID = pAnalyzerID
                                                    row.EndEdit()
                                                Next
                                                myDS.AcceptChanges()
                                                resultData = myAnalyzerSettingsDelegate.Create(dbConnection, myDSSettings)

                                            End If
                                        End If

                                    End If

                                End If
                            End If


                        End If


                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeAnalyzerSettings", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

        End Sub


        ''' <summary>
        ''' Evaluate current flags and requests the proper actions to reachs a stable setup FW vs. SW
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with data as boolean (TRUE: No actions required. Stable setup achived // FALSE: Some action has been requested in order to achive a stable setup</returns>
        ''' <remarks>
        ''' Created by:  AG 20/06/2012 - Created and prepared for DL to complete it
        ''' Modified by: DL 21/06/2012 - 
        '''              AG 19/07/2012 -review and modify
        '''              XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
        ''' </remarks>
        Private Function RecoverStableSetup(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim stableSetupAchieved As Boolean = True '/True means that is not necessary any action to reachs a stable setup 
            '                                          /False means that some actions to reach stable setup has been started

            Try
                'AG 19/07/2012
                'resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then
                'AG 19/07/2012

                'DL 21/06/2012. Begin 
                Dim myGlobal As GlobalDataTO

                'Warm Up in course
                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS" Then
                    '1.	StartInstrument = 'INI'
                    '2.	Washing         = 'INI'
                    '3.	BaseLine        = 'INI'



                    'If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartInstrument.ToString) = "INI" Then
                    '    '1.	Re-send STANDBY instruction. Requires analyzer status SLEEP
                    '    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                    '        stableSetupAchieved = False
                    '        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                    '    End If

                    'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "INI" Then
                    '    '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                    '    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    '        stableSetupAchieved = False
                    '        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                    '    End If

                    'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BaseLine.ToString) = "INI" Then
                    '    '3.	Re-send ALIGHT instruction (well CurrentWellAttribute). Requires analyzer status STANDBY
                    '    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    '        stableSetupAchieved = False
                    '        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, CurrentWellAttribute)
                    '    End If

                    'End If
                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartInstrument.ToString) = "INI" Then
                        '1.	Re-send STANDBY instruction. Requires analyzer status SLEEP
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartInstrument, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.StartInstrument)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "INI" Then
                        '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Wash)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BaseLine.ToString) = "INI" Then
                        '3.	Re-send ALIGHT instruction (well CurrentWellAttribute). Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessStaticBaseLine)
                        End If
                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill.ToString) = "INI" Then

                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine)
                        End If
                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read.ToString) = "INI" Then

                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine)
                        End If
                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty.ToString) = "INI" Then

                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "")
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine)
                        End If
                    End If


                    'Shut Down in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "INPROCESS" Then
                    '1.	Washing = 'INI'
                    '2.	SleeptInstrument = 'INI'

                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "INI" Then
                        '1.	Re-send WASH instruction. Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SleepInstrument.ToString) = "INI" Then
                        '2.	Re-send SLEEP instruction. Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True)
                        End If
                    End If


                    'Ise conditioning in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess.ToString) = "INPROCESS" Then
                    '1.	ISEClean = 'INI'
                    '2.	ISEPumpCalib = 'INI'
                    '3.	ISECalibAB = 'INI'
                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                    Dim autoIseConditFlag As Boolean = False
                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEClean.ToString) = "INI" Then
                        '1.	Set flag ISEClean to "". Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEClean, "")
                            autoIseConditFlag = True
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib.ToString) = "INI" Then
                        '2.	Set flag ISEPumpCalib to "". Requires analyzer status STANDBYcmd
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib, "")
                            autoIseConditFlag = True
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB.ToString) = "INI" Then
                        '3.	Set flag ISECalibAB to "". Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB, "")
                            autoIseConditFlag = True
                        End If
                    End If

                    If autoIseConditFlag Then
                        If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                        End If

                        stableSetupAchieved = False
                        myGlobal = PerformAutoIseConditioning(dbConnection)
                    End If


                    'New rotor in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                    '1.	NewRotor = 'INI'
                    '2.	BaseLine = 'INI'	

                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NewRotor.ToString) = "INI" Then
                        '1.	Re-send NROTOR instruction. Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.NROTOR)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BaseLine.ToString) = "INI" Then
                        '2.	Re-send ALIGHT instruction (well 1). Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)
                        End If

                    End If


                    'Recover in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess.ToString) = "INPROCESS" Then
                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "INI" Then
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "" Then
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RECOVER, True, Nothing, 1)
                        End If
                    End If

                    'Read barcode before running in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess.ToString) = "INPROCESS" Then
                    'In this case the Sw must starts the process, read the barcode of reagents and samples rotor
                    'Call method ManageBarCodeRequestBeforeRUNNING in AnalyzerManager 
                    BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.BEFORE_RUNNING_REQUEST
                    myGlobal = ManageBarCodeRequestBeforeRUNNING(dbConnection, BarCodeBeforeRunningProcessStatusAttribute)


                    'Enter in running in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess.ToString) = "INPROCESS" Then
                    '1.	EnterRunning = 'INI'
                    '2.	StartRunning = 'INI'

                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.EnterRunning.ToString) = "INI" Then
                        '1.	Re-send RUNNING instruction. Requires analyzer status STANDBY
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                            stableSetupAchieved = False
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True, Nothing, 1)
                        End If

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) = "INI" Then
                        '2.	Re-send START instruction. Requires analyzer status RUNNING
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.START, True, Nothing, 1)
                        End If
                    End If

                    'Pause in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess.ToString) = "INPROCESS" Then
                    '1.	ISEConsumption = 'INI'	
                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption.ToString) = "INI" Then

                        ' XBC 23/07/2012 - Estimated ISE Consumption by Firmware during WS
                        If Not ISEAnalyzer Is Nothing _
                           AndAlso ISEAnalyzer.IsISEModuleInstalled Then    ' XBC 02/08/2012 - correction to allow complete processing with INFO start
                            ISEAnalyzer.EstimatedFWConsumptionWS()

                            myGlobal = ISEAnalyzer.SaveConsumptions()

                            'AG + XBC 28/09/2012 - also evaluate that any instruction has been sent before to set the analyzer not ready
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing AndAlso CType(myGlobal.HasError, Boolean) = True AndAlso ConnectedAttribute Then
                                stableSetupAchieved = False
                            End If
                            'AG + XBC 28/09/2012

                        End If
                    End If

                    'Abort in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                    '1.	Washing = 'INI' or ''
                    '2.	ISEConsumption = 'INI'	
                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                        stableSetupAchieved = False
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True, Nothing, 1)

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "INI" OrElse _
                          mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "" Then
                        stableSetupAchieved = False
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)

                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption.ToString) = "INI" Then

                        ' XBC 23/07/2012 - Estimated ISE Consumption by Firmware during WS
                        If Not ISEAnalyzer Is Nothing _
                           AndAlso ISEAnalyzer.IsISEModuleInstalled Then    ' XBC 02/08/2012 - correction to allow complete processing with INFO start
                            ISEAnalyzer.EstimatedFWConsumptionWS()

                            myGlobal = ISEAnalyzer.SaveConsumptions()

                            'AG + XBC 28/09/2012 - also evaluate that any instruction has been sent before to set the analyzer not ready
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing AndAlso CType(myGlobal.HasError, Boolean) = True AndAlso ConnectedAttribute Then
                                stableSetupAchieved = False
                            End If
                            'AG + XBC 28/09/2012
                        End If

                    End If

                    'AG 27/08/2012 -Recovery results in course
                ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" Then
                    'AG 03/09/2012 - comment, this method is called only in STANDBY and these subprocesses requires Running
                    ''1.	ResRecoverPrepProblems = 'INI'
                    ''2.	ResRecoverReadings = 'INI'
                    ''3.	ResRecoverISE = 'INI'
                    'If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    '    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverPrepProblems.ToString) = "INI" OrElse _
                    '      mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverPrepProblems.ToString) = "" Then
                    '        stableSetupAchieved = False
                    '        myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem)

                    '    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverReadings.ToString) = "INI" OrElse _
                    '          mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverReadings.ToString) = "" Then
                    '        stableSetupAchieved = False
                    '        myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.Biochemical)

                    '    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverISE.ToString) = "INI" OrElse _
                    '           mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverISE.ToString) = "" Then
                    '        stableSetupAchieved = False
                    '        myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.ISE)
                    '    End If
                    '
                    'End If
                    ''AG 27/08/2012

                    'DL 21/06/2012. End
                End If


                If Not stableSetupAchieved AndAlso Not resultData.HasError AndAlso ConnectedAttribute Then
                    SetAnalyzerNotReady()
                End If

                'End If'AG 19/07/2012
                'End If'AG 19/07/2012

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.RecoverStableSetup", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            resultData.SetDatos = stableSetupAchieved
            Return resultData
        End Function


#End Region

#Region "Queues (Instructions or Tasks)"

        ''' <summary>
        ''' Programm the waiting timer
        ''' </summary>
        ''' <param name="pInterval"></param>
        ''' <remarks>
        ''' Created by AG 07/05/2010 - To confirm
        ''' Modified by: RH 29/06/2010 - Remove invalid value for Interval. Remove AnalyzerIsReady
        ''' </remarks>
        Private Sub InitializeTimerControl(ByVal pInterval As Integer)
            Try
                'Warning: pInterval most be greater than 0

                If pInterval > 0 Then
                    pInterval = pInterval + SYSTEM_TIME_OFFSET
                Else
                    pInterval = WAITING_TIME_OFF
                End If

                If pInterval = WAITING_TIME_OFF Then
                    waitingTimer.Enabled = False

                    'AG 13/02/2012 - In Running the waiting time is always active with WAITING_TIME_DEFAULT
                    'When stop the waiting time restart it again!!
                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                        waitingTimer.Interval = (WAITING_TIME_DEFAULT + SYSTEM_TIME_OFFSET) * 1000    'Convert time from seconds to miliseconds
                        waitingTimer.Enabled = True
                    End If
                    'AG 13/02/2012
                Else
                    waitingTimer.Enabled = False
                    waitingTimer.Interval = pInterval * 1000    'Convert time from seconds to miliseconds

                    'AG 13/02/2012 - In Running the Analyzer Ready is evaluated only in Status instruction reception
                    'AnalyzerIsReadyAttribute = False
                    If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                        AnalyzerIsReadyAttribute = False
                    End If
                    'AG 13/02/2012

                    waitingTimer.Enabled = True
                End If
                'AnalyzerIsReady = Not waitingTimer.Enabled  'AG 19/05/2010

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeTimerControl", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Gets and deletes the first action in queue and send it!!!
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 20/05/2010 (tested pending)
        ''' </remarks>
        Private Function GetFirstFromQueue() As GlobalEnumerates.AnalyzerManagerSwActionList
            Dim myAction As New GlobalEnumerates.AnalyzerManagerSwActionList
            Try
                myAction = GlobalEnumerates.AnalyzerManagerSwActionList.NONE
                If myInstructionsQueue.Count > 0 Then
                    myAction = (From a In myInstructionsQueue Select a).First
                    myInstructionsQueue.Remove(myAction)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFirstFromQueue", EventLogEntryType.Error, False)
                myInstructionsQueue.Clear()
                myParamsQueue.Clear() 'AG 19/07/2011
            End Try
            Return myAction
        End Function


        ''' <summary>
        ''' Gets and deletes the first parameters in queue and send it!!!
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 24/05/2011 (tested pending)
        ''' </remarks>
        Private Function GetFirstParametersFromQueue() As Object
            Dim myParam As New Object
            Try
                myParam = Nothing
                If myParamsQueue.Count > 0 Then
                    myParam = (From a In myParamsQueue Select a).First
                    myParamsQueue.Remove(myParam)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFirstParametersFromQueue", EventLogEntryType.Error, False)
                myParamsQueue.Clear()
            End Try
            Return myParam
        End Function


        ''' <summary>
        ''' Programm the waiting timer for start instructions
        ''' </summary>
        ''' <param name="pInterval"></param>
        ''' <remarks>
        ''' Created by XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        ''' </remarks>
        Private Sub InitializeTimerStartTaskControl(ByVal pInterval As Integer)
            Try
                'Warning: pInterval most be greater than 0

                If pInterval > 0 Then
                    pInterval = pInterval + SYSTEM_TIME_OFFSET
                Else
                    pInterval = WAITING_TIME_OFF
                End If

                If pInterval = WAITING_TIME_OFF Then
                    waitingStartTaskTimer.Enabled = False
                Else
                    waitingStartTaskTimer.Enabled = False
                    waitingStartTaskTimer.Interval = pInterval * 1000    'Convert time from seconds to miliseconds

                    AnalyzerIsReadyAttribute = False

                    waitingStartTaskTimer.Enabled = True
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitializeTimerStartTaskControl", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Stores action to send in queue
        ''' </summary>
        ''' <remarks>
        ''' Created by XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        ''' </remarks>
        Private Sub StoreStartTaskinQueue(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList, _
                                          ByVal pSwAdditionalParameters As Object, _
                                          ByVal pFwScriptID As String, _
                                          ByVal pParams As List(Of String))
            Try
                MyClass.ClearStartTaskQueueToSend()

                MyClass.myStartTaskInstructionsQueue.Add(pAction)
                MyClass.myStartTaskParamsQueue.Add(pSwAdditionalParameters)
                MyClass.myStartTaskFwScriptIDsQueue.Add(pFwScriptID)
                MyClass.myStartTaskFwScriptParamsQueue.Add(pParams)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StoreStartTaskinQueue", EventLogEntryType.Error, False)
                MyClass.ClearStartTaskQueueToSend()
            End Try
        End Sub

        ''' <summary>
        ''' Clear Start Task previously stored
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 28/10/2010</remarks>
        Public Function ClearStartTaskQueueToSend() As GlobalDataTO Implements IAnalyzerEntity.ClearStartTaskQueueToSend
            Dim myGlobal As New GlobalDataTO
            Try
                MyClass.myStartTaskInstructionsQueue.Clear()
                MyClass.myStartTaskParamsQueue.Clear()
                MyClass.myStartTaskFwScriptIDsQueue.Clear()
                MyClass.myStartTaskFwScriptParamsQueue.Clear()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ClearStartTaskQueueToSend", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Send action in queue
        ''' </summary>
        ''' <remarks>
        ''' Created by XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        ''' </remarks>
        Private Function SendStartTaskinQueue() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim queuedAction As GlobalEnumerates.AnalyzerManagerSwActionList
                Dim queuedSwAdditionalParameters As Object
                Dim queuedFwScriptID As String
                Dim queuedFwParams As List(Of String)

                queuedAction = (From a In myStartTaskInstructionsQueue Select a).First
                queuedSwAdditionalParameters = (From a In myStartTaskParamsQueue Select a).First
                queuedFwScriptID = (From a In myStartTaskFwScriptIDsQueue Select a).First
                queuedFwParams = (From a In myStartTaskFwScriptParamsQueue Select a).First

                MyClass.ClearStartTaskQueueToSend()

                myGlobal = ManageAnalyzer(queuedAction, True, Nothing, queuedSwAdditionalParameters, queuedFwScriptID, queuedFwParams)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SendStartTaskinQueue", EventLogEntryType.Error, False)
                MyClass.ClearStartTaskQueueToSend()
            End Try
            Return myGlobal
        End Function

#End Region

    End Class

End Namespace
