Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Linq

Namespace Biosystems.Ax00.Core.Entities
    Public Class CurrentAlarms

        Private ReadOnly _analyzerManager As IAnalyzerManager
        Private _myGlobal As New GlobalDataTO
        Private _dbConnection As SqlConnection = Nothing
        Private _myIseOffErrorFixed As Boolean = False
        Private _myIseTimeoutErrorFixed As Boolean = False
        Private _myCommsTimeoutErrorFixed As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="analyzerMan"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef analyzerMan As IAnalyzerManager)
            _analyzerManager = analyzerMan
        End Sub


        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' Note that some alarms has already perfomed previous business (alarms in instructions different than AlarmsDetails,
        ''' for instance ANSBR1, ANSBR2, ANSBM1, STATUS, ...)
        ''' Finally save alarms into Database and prepare DS for UI refresh
        ''' </summary>
        ''' <param name="pAlarmIdList">List of alarms to treat</param>
        ''' <param name="pAlarmStatusList" >For each alarm in pAlarmCode indicates his status: TRUE alarm exists, FALSE alarm solved</param>
        ''' <param name="pAdditionalInfoList">Additional info for some alams: volume missing, prep clot warnings, prep locked</param>
        ''' <returns>Global Data To indicating when an error has occurred </returns>
        ''' <remarks>
        ''' Created by:  AG 16/03/2011
        ''' Modified by: SA 29/06/2012 - Removed the OpenDBTransaction; all functions called by this method should open their own DBTransaction
        '''                              or DBConnection, depending if they update or read data - This is to avoid locks between the different
        '''                              threads in execution
        '''              AG 25/072012    pAdditionalInfoList
        '''              AG 30/11/2012 - Do not inform the attribute AnalyzerManager.EndRunInstructionSent()as TRUE when call the ManageAnalyzer with ENDRUN or when you add it to the queue
        '''                              This flag will be informed once the instruction has been really sent. Current code causes that sometimes the ENDRUN instruction is added to
        '''                              queue but how the flag is informed before send the instruction it wont never be sent!!
        '''              AG 22/05/2014 - #1637 Use exclusive lock (multithread protection)
        '''              AG 05/06/2014 - #1657 Protection (provisional solution)! (do not clear instructions queue when there are only 1 alarm and it is ISE_OFF_ERR)
        '''                            - PENDING FINAL SOLUTION: AlarmID ISE_OFF_ERR must be generated only 1 time when alarm appears, and only 1 time when alarm is fixed (now this alarm with status FALSE is generated with each ANSINF received)
        '''              XB 04/11/2014 - Add ISE_TIMEOUT_ERR alarm - BA-1872
        '''              XB 06/11/2014 - Add COMMS_TIMEOUT_ERR alarm - BA-1872
        ''' </remarks>
        ''' <code>
        ''' --------------------------------DELETED CODE------------------------------------------
        ''' If String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "AUTO") Then
        ''' 
        ''' Case GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN, GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN, GlobalEnumerates.Alarms.S_NO_VOLUME_WARN
        ''' 'NOTE: Previous business is performed in method ProcessArmStatusRecived
        ''' 
        ''' Case GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN, GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR
        ''' FRIDGE: No business (only inform user)
        ''' ISE: Send ISE test preparations are not allowed and inform user
        ''' 
        ''' </code>
        Public Function Manage(ByVal pAlarmIdList As List(Of GlobalEnumerates.Alarms), _
                                      ByVal pAlarmStatusList As List(Of Boolean), _
                                      Optional ByVal pAdditionalInfoList As List(Of String) = Nothing) As GlobalDataTO



            Try
                Dim index As Integer = 0
                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

                If Not _analyzerManager.EndRunInstructionSent() AndAlso _analyzerManager.AnalyzerCurrentAction() = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                    _analyzerManager.EndRunInstructionSent() = True
                End If

                Dim methodHasToAddInstructionToQueueFlag As Integer = 0
                'Once one instruction has been sent. The other instructions managed in this method will be added to queue
                '  0 -> Instruction can be sent
                '  1 -> One instruction has already been sent. New instructions to be sent in ManageAlarms will be add to queue
                '  2 -> None instruction can be sent (RESET FREEZE). New instructions to be sent in ManageAlarms wont be add to queue (except SOUND)

                If _analyzerManager.AnalyzerIsFreeze() Then
                    If ((pAlarmIdList.Count = 1 AndAlso Not pAlarmIdList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIdList.Count > 1) Then
                        _analyzerManager.ClearQueueToSend() 'Clear all instruction in queue to be sent
                    End If

                    Select Case _analyzerManager.AnalyzerFreezeMode()
                        Case "PARTIAL"
                            ApplyActionsForAnalyzerInPartialFreezeMode(methodHasToAddInstructionToQueueFlag)

                        Case "TOTAL"
                            ApplyActionsForAnalyzerInTotalFreezeMode(methodHasToAddInstructionToQueueFlag, myAnalyzerFlagsDs)

                        Case "RESET"
                            ApplyActionsForAnalyzerInResetFreezeMode(methodHasToAddInstructionToQueueFlag)
                    End Select

                End If
                Dim alarmsDelg As New WSAnalyzerAlarmsDelegate

                Dim glfBoardErrorFixed As Boolean = False
                For Each alarmItem As GlobalEnumerates.Alarms In pAlarmIdList
                    'General description: Apply special Business depending the alarm code
                    '        1- Launch Sw processes
                    '        2- Automatically send new instruction to the Analyzer

                    Select Case alarmItem

                        Case GlobalEnumerates.Alarms.REACT_MISSING_ERR
                            NoReactionsRotor(myAnalyzerFlagsDs, pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.REACT_TEMP_WARN, GlobalEnumerates.Alarms.R1_TEMP_WARN, _
                             GlobalEnumerates.Alarms.R2_TEMP_WARN, GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN, _
                             GlobalEnumerates.Alarms.WS_TEMP_WARN, GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR, _
                             GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR, GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR
                            TemperatureWarning(alarmItem, pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN, GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR, _
                            GlobalEnumerates.Alarms.WASH_CONTAINER_WARN, GlobalEnumerates.Alarms.WASH_CONTAINER_ERR
                            ContainerWarnings(alarmItem, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR, GlobalEnumerates.Alarms.WATER_SYSTEM_ERR
                            WaterError(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR, GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR
                            WasteSystemError(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.R1_COLLISION_WARN, GlobalEnumerates.Alarms.R2_COLLISION_WARN, _
                            GlobalEnumerates.Alarms.S_COLLISION_WARN
                            NonCriticalCollisions(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.CLOT_DETECTION_WARN, GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                            ClotDetection(alarmItem, pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                            BaseLineInitError(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                            BaselineWellWarning(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.RECOVER_ERR
                            RecoverInstructionError(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.R1_COLLISION_ERR, GlobalEnumerates.Alarms.R2_COLLISION_ERR, _
                            GlobalEnumerates.Alarms.S_COLLISION_ERR
                            ArmCollitionError(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.INST_ABORTED_ERR, GlobalEnumerates.Alarms.INST_REJECTED_ERR
                            InstructionAborted(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.INST_REJECTED_WARN
                            InstructionRejected(myAnalyzerFlagsDs)

                        Case GlobalEnumerates.Alarms.ISE_OFF_ERR
                            IseSwitchOff(pAlarmStatusList, index, alarmsDelg)

                        Case GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR
                            IseTimeout(alarmsDelg)

                        Case GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR
                            CommsTimeout(alarmsDelg)

                        Case GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR
                            IseConnectPdtError(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.FW_CPU_ERR, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR, _
                             GlobalEnumerates.Alarms.FW_REPOSITORY_ERR, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR, _
                             GlobalEnumerates.Alarms.FW_MAN_ERR
                            FirmwareError(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.INST_NOALLOW_INS_ERR, GlobalEnumerates.Alarms.INST_REJECTED_WARN
                            InstructionError(myAnalyzerFlagsDs)

                        Case GlobalEnumerates.Alarms.GLF_BOARD_ERR
                            GlfBoardError(alarmsDelg, methodHasToAddInstructionToQueueFlag)
                    End Select
                    index = index + 1
                Next

                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    _myGlobal = myFlagsDelg.Update(_dbConnection, myAnalyzerFlagsDs)
                End If

                'Save alarms into DataBase and prepare DS for UI refresh
                Dim wsAlarmsDs As New WSAnalyzerAlarmsDS
                index = 0

                _analyzerManager.ClearReceivedAlarmsFromRefreshDs()

                'Get the alarms defined with OKType = False (never are marked as solved)
                Dim alarmsWithOkTypeFalse = (From a As AlarmsDS.tfmwAlarmsRow In _
                                                            alarmsDefintionTableDS.tfmwAlarms Where a.OKType = False Select a.AlarmID).ToList

                For Each alarmIdItem As GlobalEnumerates.Alarms In pAlarmIdList
                    If alarmIdItem <> GlobalEnumerates.Alarms.NONE Then
                        CheckIfSaveAlarm(pAlarmStatusList, index, alarmIdItem, _
                                         alarmsWithOkTypeFalse, wsAlarmsDs, pAdditionalInfoList)
                    End If
                    index = index + 1
                Next
                wsAlarmsDs.AcceptChanges()

                If wsAlarmsDs.twksWSAnalyzerAlarms.Rows.Count > 0 Then
                    _myGlobal = alarmsDelg.Save(_dbConnection, wsAlarmsDs, alarmsDefintionTableDS) 'AG 24/07/2012 change Create for Save who inserts alarms with status TRUE and updates alarms with status FALSE
                    If Not _myGlobal.HasError AndAlso Not _analyzerManager.Ringing Then
                        SoundActivationByAlarm(methodHasToAddInstructionToQueueFlag)
                    End If
                End If

                If _analyzerManager.AnalyzerIsFreeze Then
                    ManageAllFreezeAlarms(pAlarmIdList, methodHasToAddInstructionToQueueFlag)
                End If

            Catch ex As Exception
                _myGlobal.HasError = True
                _myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                _myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex)
            End Try

            Return _myGlobal
        End Function

        ''' <summary>
        ''' Save alarms and also the alarms solved
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="alarmIdItem"></param>
        ''' <param name="alarmsWithOkTypeFalse"></param>
        ''' <param name="wsAlarmsDs"></param>
        ''' <param name="pAdditionalInfoList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Sub CheckIfSaveAlarm(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByVal alarmIdItem As GlobalEnumerates.Alarms, _
                                     ByVal alarmsWithOkTypeFalse As List(Of String), ByVal wsAlarmsDs As WSAnalyzerAlarmsDS, ByVal pAdditionalInfoList As List(Of String))
            Dim newRowFlag = False

            If pAlarmStatusList(index) Then
                If _analyzerManager.AlarmListAddtem(alarmIdItem) Or alarmsWithOkTypeFalse.Contains(alarmIdItem.ToString) Then
                    newRowFlag = True
                End If
            ElseIf _myIseOffErrorFixed AndAlso alarmIdItem = GlobalEnumerates.Alarms.ISE_OFF_ERR Then
                newRowFlag = True
            ElseIf _myIseTimeoutErrorFixed AndAlso alarmIdItem = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                newRowFlag = True
            ElseIf _myCommsTimeoutErrorFixed AndAlso alarmIdItem = GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR Then
                newRowFlag = True
            Else
                If _analyzerManager.Alarms.Contains(alarmIdItem) Then
                    _analyzerManager.AlarmListRemoveItem(alarmIdItem)
                    newRowFlag = True
                End If
            End If
            AddAlarmIntoWsAnalyzerAlarmDsAndRefreshUi(pAlarmStatusList, index, alarmIdItem, newRowFlag, wsAlarmsDs, pAdditionalInfoList)
        End Sub

        ''' <summary>
        ''' Add rows into WSAnalyzerAlarmsDS with the new or solved alarms and save them
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="alarmIdItem"></param>
        ''' <param name="newRowFlag"></param>
        ''' <param name="wsAlarmsDs"></param>
        ''' <param name="pAdditionalInfoList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Sub AddAlarmIntoWsAnalyzerAlarmDsAndRefreshUi(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByVal alarmIdItem As GlobalEnumerates.Alarms, _
                                                              ByVal newRowFlag As Boolean, ByVal wsAlarmsDs As WSAnalyzerAlarmsDS, ByVal pAdditionalInfoList As List(Of String))

            If newRowFlag Then
                Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                alarmRow = wsAlarmsDs.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                With alarmRow
                    .BeginEdit()
                    .AlarmID = alarmIdItem.ToString
                    .AnalyzerID = _analyzerManager.ActiveAnalyzer()
                    .AlarmDateTime = Now
                    If _analyzerManager.ActiveWorkSession() <> "" Then
                        .WorkSessionID = _analyzerManager.ActiveWorkSession()
                    Else
                        .SetWorkSessionIDNull()
                    End If
                    .AlarmStatus = pAlarmStatusList(index)
                    .AlarmItem = 1
                    .SetAdditionalInfoNull()
                    If pAdditionalInfoList IsNot Nothing AndAlso pAdditionalInfoList.Count > index AndAlso
                       pAdditionalInfoList(index) IsNot Nothing AndAlso
                       String.Equals(pAdditionalInfoList(index), String.Empty) Then
                        .AdditionalInfo = pAdditionalInfoList(index)
                    End If
                    .EndEdit()
                End With

                wsAlarmsDs.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                'Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                _myGlobal = _analyzerManager.PrepareUIRefreshEvent(_dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmIdItem.ToString, pAlarmStatusList(index))

                If _myIseOffErrorFixed Then
                    _analyzerManager.AlarmListRemoveItem(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                End If

            End If
        End Sub

        ''' <summary>
        ''' GLF_BOARD_ERR
        ''' </summary>
        ''' <param name="alarmsDelg"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks></remarks>
        Private Sub GlfBoardError(ByVal alarmsDelg As WSAnalyzerAlarmsDelegate, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            _myGlobal = alarmsDelg.GetByAlarmID(_dbConnection, GlobalEnumerates.Alarms.GLF_BOARD_ERR.ToString(), Nothing, Nothing, _analyzerManager.ActiveAnalyzer(), "")
            If Not _myGlobal.HasError AndAlso Not _myGlobal.SetDatos Is Nothing Then
                Dim temporalDs = DirectCast(_myGlobal.SetDatos, WSAnalyzerAlarmsDS).twksWSAnalyzerAlarms
                If (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDs _
                    Where a.AlarmID = GlobalEnumerates.Alarms.GLF_BOARD_ERR.ToString AndAlso a.AlarmStatus = True Select a).Count > 1 Then
                    ApplyActionsForAnalyzerInResetFreezeMode(methodHasToAddInstructionToQueueFlag)
                Else
                    'myInstructionsQueue.Add(pAction)
                    'myParamsQueue.Add(pSwAdditionalParameters)
                End If
            End If
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: Stop the sensor information instructions
        ''' BUSINESS: AnalyzerManager.SetAnalyzerNotReady
        ''' PRESENTATION: Show message box informing the user the analyzer must be restarted
        ''' </remarks>
        Private Sub ApplyActionsForAnalyzerInResetFreezeMode(ByRef methodHasToAddInstructionToQueueFlag As Integer)

            _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
            _analyzerManager.SetAnalyzerNotReady()

            'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
            If Not _myGlobal.HasError Then
                methodHasToAddInstructionToQueueFlag = 2

                If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    Dim myWsAnalyzerDelegate As New WSAnalyzersDelegate
                    _myGlobal = myWsAnalyzerDelegate.UpdateWSStatus(_dbConnection, _analyzerManager.ActiveAnalyzer(), _analyzerManager.ActiveWorkSession(), "ABORTED")
                End If
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <param name="myAnalyzerFlagsDs"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: If Running then send STANDBY instruction (if not sent yet)
        ''' BUSINESS: Nothing
        ''' PRESENTATION: Show message box informing the user the worksesion has been aborted + activate Recover button
        ''' </remarks>
        Private Sub ApplyActionsForAnalyzerInTotalFreezeMode(ByRef methodHasToAddInstructionToQueueFlag As Integer, ByRef myAnalyzerFlagsDs As AnalyzerManagerFlagsDS)

            If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then 'Alarm Exists
                If _analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                End If
                'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
                If Not _myGlobal.HasError Then
                    methodHasToAddInstructionToQueueFlag = 1
                    _analyzerManager.EndRunInstructionSent() = True 'AG 14/05/2012 - when StandBy instruction is sent the ENDRUN instruction has no sense!!

                    Dim myWsAnalyzerDelegate As New WSAnalyzersDelegate
                    _myGlobal = myWsAnalyzerDelegate.UpdateWSStatus(_dbConnection, _analyzerManager.ActiveAnalyzer(), _analyzerManager.ActiveWorkSession(), "ABORTED")
                End If
            Else
                'If new freeze total alarm appears during recover washings ... mark recover process as closed                            
                If _analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS" _
                   AndAlso _analyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "CLOSED")
                End If
            End If
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: If Running then send END instruction (if not sent yet)
        ''' BUSINESS: Nothing
        ''' PRESENTATION: Show message box informing the user the worksesion has been paused 'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages +
        '''               When analyzer leaves RUNNING and becomes STANDBY activate Recover button
        ''' </remarks>
        Private Sub ApplyActionsForAnalyzerInPartialFreezeMode(ByRef methodHasToAddInstructionToQueueFlag As Integer)
            If Not _analyzerManager.AllowScanInRunning() AndAlso Not _analyzerManager.PauseModeIsStartingState AndAlso _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
               Not _analyzerManager.EndRunInstructionSent() AndAlso _analyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then

                _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)

                If Not _myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                    methodHasToAddInstructionToQueueFlag = 1
                    _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Special code for FREEZE (AUTO) - once solved the cover alarms error check if there are any other freeze alarm o not
        ''' </summary>
        ''' <param name="pAlarmIdList"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' BUSINESS: 
        ''' If not remove the attributes flags (freeze and freeze mode and also update the sensor FREERZE to 0)
        ''' NOTE: for call this method here the parameter can not be pAlarmIDList. Instead use the current alarms in analyzer attribute (myAlarmListAttribute)
        ''' </remarks>
        Private Sub ManageAllFreezeAlarms(ByVal pAlarmIdList As List(Of GlobalEnumerates.Alarms), ByVal methodHasToAddInstructionToQueueFlag As Integer)

            If ((pAlarmIdList.Count = 1 AndAlso Not pAlarmIdList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIdList.Count > 1) Then
                If String.Equals(_analyzerManager.AnalyzerFreezeMode(), "AUTO") Then


                    If Not ExistFreezeAlarms(_analyzerManager.Alarms) Then 'Freeze auto solved
                        'Analyzer has recovered an operating status automatically by closing covers
                        _analyzerManager.AnalyzerIsFreeze = False
                        _analyzerManager.AnalyzerFreezeMode = ""
                        _analyzerManager.AnalyzerIsReady = True 'Analyzer is ready
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 0, True) 'Inform presentation the analyzer is ready

                        If methodHasToAddInstructionToQueueFlag = 0 Then
                            _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status
                        ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                            _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, "")
                        End If

                    Else 'Exists freeze auto
                        'Reset all processes flags
                        Dim flagsDlg As New AnalyzerManagerFlagsDelegate
                        _myGlobal = flagsDlg.ResetFlags(_dbConnection, _analyzerManager.ActiveAnalyzer())

                        'Finally read and update our internal structures
                        If Not _myGlobal.HasError Then
                            _analyzerManager.InitializeAnalyzerFlags(_dbConnection)
                        End If

                        'if standby ... activate ansinf (some processes as shutdown, nrotor, ise utilities deactivate it, if freezes
                        'appears while sensors deactivated there is no other way to auto recover
                        If Not _myGlobal.HasError Then
                            If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                                'if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected Then _analyzerManager.SetAnalyzerNotReady()
                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, CInt(GlobalEnumerates.Ax00InfoInstructionModes.STR))
                                End If
                            End If
                        End If
                    End If

                ElseIf Not String.Equals(_analyzerManager.AnalyzerFreezeMode(), "PARTIAL") Then
                    Dim flagsDlg As New AnalyzerManagerFlagsDelegate
                    _myGlobal = flagsDlg.ResetFlags(_dbConnection, _analyzerManager.ActiveAnalyzer())
                End If
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myAnalyzerFlagsDs"></param>
        ''' <remarks></remarks>
        Private Sub InstructionError(ByVal myAnalyzerFlagsDs As AnalyzerManagerFlagsDS)
            If _analyzerManager.InstructionTypeSent() = GlobalEnumerates.AppLayerEventList.PAUSE Then
                _analyzerManager.PauseInstructionSent() = False
                _analyzerManager.SetAllowScanInRunningValue(False)
                If String.Compare(_analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess), "INPROCESS", False) = 0 Then
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                End If
            End If
        End Sub

        ''' <summary>
        ''' FWM ERROR
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: nothing
        ''' 
        ''' BUSINESS: call stopcomm Cut off communications channel
        ''' 
        ''' PRESENTATION: disconnect Showmessage from Presntation
        ''' </remarks>
        Private Sub FirmwareError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            If pAlarmStatusList(index) Then
                If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                End If
                If Not _myGlobal.HasError Then _analyzerManager.StopComm()
            End If
        End Sub
        ''' <summary>
        ''' Ise Connect PDT Error
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks></remarks>
        Private Sub IseConnectPdtError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)
            If pAlarmStatusList(index) Then
                _analyzerManager.RefreshISEAlarms()
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alarmsDelg"></param>
        ''' <remarks>
        ''' Search if exists alarm COMMS_TIMEOUT_ERR with status TRUE, in this case set flag myCOMMSTimeoutErrorFixed = True (FIXED) in order to mark it as fixed
        ''' </remarks>
        Private Sub CommsTimeout(ByVal alarmsDelg As WSAnalyzerAlarmsDelegate)

            _myGlobal = alarmsDelg.GetByAlarmID(_dbConnection, GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR.ToString, Nothing, Nothing, _analyzerManager.ActiveAnalyzer(), "")
            If Not _myGlobal.HasError AndAlso Not _myGlobal.SetDatos Is Nothing Then
                Dim temporalDs = DirectCast(_myGlobal.SetDatos, WSAnalyzerAlarmsDS)
                Dim auxList As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDs.twksWSAnalyzerAlarms _
                    Where a.AlarmID = GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                If auxList.Count > 0 Then
                    _myCommsTimeoutErrorFixed = True
                End If
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alarmsDelg"></param>
        ''' <remarks>
        ''' Search if exists alarm ISE_TIMEOUT_ERR with status TRUE, in this case set flag myISETimeoutErrorFixed = True (FIXED) in order to mark it as fixed
        ''' </remarks>
        Private Sub IseTimeout(ByVal alarmsDelg As WSAnalyzerAlarmsDelegate)

            _myGlobal = alarmsDelg.GetByAlarmID(_dbConnection, GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR.ToString, Nothing, Nothing, _analyzerManager.ActiveAnalyzer(), "")
            If Not _myGlobal.HasError AndAlso Not _myGlobal.SetDatos Is Nothing Then
                Dim temporalDs = DirectCast(_myGlobal.SetDatos, WSAnalyzerAlarmsDS)
                Dim auxList As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDs.twksWSAnalyzerAlarms _
                    Where a.AlarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                If auxList.Count > 0 Then
                    _myIseTimeoutErrorFixed = True
                End If
            End If
        End Sub

        ''' <summary>
        ''' ISE switch off
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="alarmsDelg"></param>
        ''' <remarks>
        ''' IF: When this alarm appears in RUNNING: Lock all pending ISE preparations
        ''' 1st: Get all ISE executions pending (required to inform the UI with the locked executions
        ''' 2on: Lock all pending ISE preparation executions
        ''' 3rd: Prepare data for generate user interface refresh event
        ''' 
        ''' ELSE: Previous code causes the MONITOR screen refreshs always each 2 seconds because each ANSINF causes new alarm notification ISE_OFF_ERR with status false
        ''' Search if exists alarm ISE_OFF_ERR with status TRUE, in this case set flag myISEOffsErrorFixed = True (FIXED) in order to mark it as fixed
        ''' </remarks>
        Private Sub IseSwitchOff(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, _
                                 ByVal alarmsDelg As WSAnalyzerAlarmsDelegate)

            If pAlarmStatusList(index) Then
                _analyzerManager.ISEAnalyzer.IsISESwitchON = False
                _analyzerManager.RefreshISEAlarms()

                If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    Dim execDelg As New ExecutionsDelegate
                    _myGlobal = execDelg.GetExecutionsByStatus(_dbConnection, _analyzerManager.ActiveAnalyzer(), _analyzerManager.ActiveWorkSession(), "PENDING", False, "PREP_ISE")
                    If Not _myGlobal.HasError AndAlso Not _myGlobal.SetDatos Is Nothing Then
                        Dim myExecutionsDs As New ExecutionsDS
                        _myGlobal = execDelg.UpdateStatusByExecutionTypeAndStatus(_dbConnection, _analyzerManager.ActiveWorkSession(), _analyzerManager.ActiveAnalyzer(), "PREP_ISE", "PENDING", "LOCKED")
                        If Not _myGlobal.HasError Then
                            For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDs.twksWSExecutions.Rows
                                _myGlobal = _analyzerManager.PrepareUIRefreshEvent(_dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                If _myGlobal.HasError Then Exit For
                            Next
                        End If
                    End If
                End If
            Else
                _myGlobal = alarmsDelg.GetByAlarmID(_dbConnection, GlobalEnumerates.Alarms.ISE_OFF_ERR.ToString, Nothing, Nothing, _analyzerManager.ActiveAnalyzer(), "")
                If Not _myGlobal.HasError AndAlso Not _myGlobal.SetDatos Is Nothing Then
                    Dim temporalDs = DirectCast(_myGlobal.SetDatos, WSAnalyzerAlarmsDS)
                    Dim auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDs.twksWSAnalyzerAlarms _
                        Where a.AlarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                    If auxList.Count > 0 Then
                        _myIseOffErrorFixed = True
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myAnalyzerFlagsDs"></param>
        ''' <remarks>
        ''' Sw has received a Request but has sent instruction out of time.
        ''' Search for next instruction to be sent but using next well (only in Running)
        ''' </remarks>
        Private Sub InstructionRejected(ByVal myAnalyzerFlagsDs As AnalyzerManagerFlagsDS)

            'TR BUG #1339  before sending the next instruction, validate if the last intruction send was pause to star logic
            If _analyzerManager.InstructionTypeSent() = GlobalEnumerates.AppLayerEventList.PAUSE Then
                _analyzerManager.PauseInstructionSent = False
                _analyzerManager.SetAllowScanInRunningValue(False)
                If String.Compare(_analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess), "INPROCESS", False) = 0 Then
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                End If
            End If
            If _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                Dim reactRotorDlg As New ReactionsRotorDelegate
                _analyzerManager.FutureRequestNextWellValue = reactRotorDlg.GetRealWellNumber(_analyzerManager.CurrentWell + 1, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)
                _myGlobal = _analyzerManager.SearchNextPreparation(_dbConnection, _analyzerManager.FutureRequestNextWellValue) 'Search for next instruction to be sent ... and sent it!!
                _analyzerManager.FillNextPreparationToSend(_myGlobal)
            End If

            GlobalBase.CreateLogActivity("Instruction rejected (out of time)", "AnalyzerManager.ManageAlarms", EventLogEntryType.Information, False)
            Debug.Print(Now.ToString & " .Instruction rejected (out of time)")
        End Sub

        ''' <summary>
        ''' Instructions aborted or rejected
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: ask for detailed errors
        ''' if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
        ''' When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
        ''' 
        ''' BUSINESS: NOTHING
        ''' 
        ''' PRESENTATION: NOTHING
        ''' </remarks>
        Private Sub InstructionAborted(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            If pAlarmStatusList(index) Then
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR)
                    methodHasToAddInstructionToQueueFlag = 1
                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected() Then _analyzerManager.SetAnalyzerNotReady()
                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    'Do not use ToString here because Sw will send INFO;Q:ALR; instead of INFO;Q:2;
                    _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, CInt(GlobalEnumerates.Ax00InfoInstructionModes.ALR))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Arm collision error (error codes)
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks></remarks>
        Private Sub ArmCollitionError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            If pAlarmStatusList(index) Then 'Alarm Exists
                'Mark all INPROCESS executions as PENDING or LOCKED
                'Dim exec_dlg As New ExecutionsDelegate
                'myGlobal = exec_dlg.ChangeINPROCESSStatusByCollision(dbConnection, AnalyzerManager.ActiveAnalyzer(), AnalyzerManager.ActiveWorkSession())
            End If
        End Sub

        ''' <summary>
        ''' Recover instruction error
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks></remarks>
        Private Sub RecoverInstructionError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)
            If pAlarmStatusList(index) Then 'Alarm Exists
                'The recover instruction has finished with error
                If _analyzerManager.RecoverInstructionSent() Then _analyzerManager.RecoverInstructionSent = False
                _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished (but with errors)
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: 
        ''' (NO, IMPLEMENT THIS BUSINESS, REJECTED!!!!) If no executions IN process -> Send ENDRUN, else do nothing (wait)
        ''' 
        ''' BUSINESS: Activate flag
        ''' 
        ''' PRESENTATION: Show message "Change methacrylate rotor is high recommend" when user performs Wup, Start or finish work session 
        ''' (Ax00MainMDI.OnManageReceptionEvent and ShowAlarmWarningMessages)
        ''' </remarks>
        Private Sub BaselineWellWarning(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)
            If pAlarmStatusList(index) Then
                _analyzerManager.WELLbaselineParametersFailures = True
            Else
                _analyzerManager.WELLbaselineParametersFailures = False
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: 
        ''' STANDBY
        ''' If alarm is new: Automatically perform new Alight
        ''' If already exists (2on tentative also fails): If running -> Go to StandBy
        ''' 'If 3 consecutive well rejection initializations have been refused (baselineCalcs.exitRunningType = 2) -> STANDBY
        ''' RPM 06/09/2012 - StandBy NO!!!, leave running using END or ABORT (no prep started, so better use END)
        ''' If 30 consecutive wells have been rejected once the initialization is OK (baselineCalcs.exitRunningType = 1) -> END
        ''' 
        ''' BUSINESS: Nothing
        ''' 
        ''' PRESENTATION: 
        ''' If alarm is new: None
        ''' If already exists (2on tentative also fails): Show message It is highly recommended change the reactions rotor
        ''' (Ax00MainMDI.OnManageReceptionEvent and ShowAlarmWarningMessages)
        ''' MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
        ''' </remarks>
        Private Sub BaseLineInitError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            If pAlarmStatusList(index) Then 'Alarm Exists
                If _analyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY AndAlso _analyzerManager.BaselineInitializationFailures < ALIGHT_INIT_FAILURES Then

                    'business will be implemented in method SendAutomaticALIGHTRerun 
                ElseIf _analyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    Select Case _analyzerManager.wellBaseLineAutoPausesSession
                        Case 1, 2 'END 
                            If Not _analyzerManager.AllowScanInRunning AndAlso Not _analyzerManager.PauseModeIsStartingState AndAlso _
                                Not _analyzerManager.EndRunInstructionSent() AndAlso _analyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected Then
                                        methodHasToAddInstructionToQueueFlag = 1
                                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                    End If

                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    If _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                    End If
                                End If
                            End If
                    End Select
                End If
            Else
                _analyzerManager.ResetBaseLineFailuresCounters()
            End If
        End Sub

        ''' <summary>
        ''' Clot detection
        ''' </summary>
        ''' <param name="alarmItem"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: Nothing
        ''' 
        ''' BUSINESS:
        ''' NOTE: Previous business is performed in method ProcessArmStatusRecived (mark preparation results as possible clot)
        ''' Case CLOT_DETECTION_ERR -> Stop sending test preparations
        ''' 
        ''' PRESENTATION:
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' </remarks>
        Private Sub ClotDetection(ByVal alarmItem As GlobalEnumerates.Alarms, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            If pAlarmStatusList(index) Then 'Alarm Exists
                If alarmItem = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR Then

                    Dim clotDetectionEnabled As Boolean = True
                    Dim adjustValue = _analyzerManager.ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.CLOT)
                    If Not String.Equals(adjustValue, String.Empty) AndAlso IsNumeric(adjustValue) Then
                        clotDetectionEnabled = CType(adjustValue, Boolean)
                    End If

                    If clotDetectionEnabled Then
                        'AG 17/07/2012 - CANCELLED again (JE+RP+ST+EF) !! Bugs Tracking 706
                    End If
                End If

            Else 'Alarm solved
                If alarmItem = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR Then
                    _analyzerManager.PauseSendingTestPreparations = False
                End If
            End If
        End Sub

        ''' <summary>
        ''' Non critical collisions (R1, R2 or S arms)
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' Sw sends Ax00 to S and S
        ''' Sw activates CONTINE or ABORT (Recover will be sent automatically before send the button instruction)
        ''' 
        ''' INSTRUCTIONS: S and S (only if the ENDRUN has not been sent yet)
        ''' TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
        ''' AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
        ''' 
        ''' BUSINESS:
        ''' NOTE: Previous business is performed in method ProcessArmStatusRecived
        ''' 
        ''' PRESENTATION:
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' </remarks>
        Private Sub NonCriticalCollisions(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            If Not _analyzerManager.AllowScanInRunning() AndAlso Not _analyzerManager.PauseModeIsStartingState() AndAlso pAlarmStatusList(index) And _
                _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not _analyzerManager.EndRunInstructionSent _
               And _analyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    If Not _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                    End If
                End If
            End If


        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' Inform the user and S and S
        ''' INSTRUCTIONS: (only if the ENDRUN has not been sent yet)
        ''' 
        ''' BUSINESS: Nothing
        ''' 
        ''' PRESENTATION:
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
        ''' (OK) 2) Show message when Ax00 stops ("Revise la salida de residuos") (Ax00MainMDI.ShowAlarmWarningMessages)
        ''' OK: (StandBy: Disable buttons START, CONTINUE,...)
        ''' </remarks>
        Private Sub WasteSystemError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)


            If pAlarmStatusList(index) And _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not _analyzerManager.EndRunInstructionSent _
               And _analyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                'if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    If _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' When Ax00 stop running do not allow continue, block machine and show message
        ''' TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
        ''' AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
        ''' BUSINESS: Nothing
        ''' 
        ''' PRESENTATION:
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
        ''' (OK) 2 Show message informing the user ("Revise el depósito de entrada, o la conexión a la red de agua") (Ax00MainMDI.ShowAlarmWarningMessages)
        ''' OK: (StandBy: Disable buttons START, CONTINUE,...)
        ''' </remarks>
        Private Sub WaterError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            If Not _analyzerManager.AllowScanInRunning AndAlso Not _analyzerManager.PauseModeIsStartingState() AndAlso pAlarmStatusList(index) And _
               _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not _analyzerManager.EndRunInstructionSent _
               And _analyzerManager.AnalyzerCurrentAction <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                'if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not _myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    ' to avoid send the same instruction more than 1 time
                    If _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sw has already determined Warning or not (CheckContainerAlarms method)
        ''' </summary>
        ''' <param name="alarmItem"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <param name="methodHasToAddInstructionToQueueFlag"></param>
        ''' <remarks>
        ''' LIMITE_WARNING. LIMITE_CRITICO
        ''' TODO
        ''' Antes de lanzar la sesion (START o CONTINUE) calcular si se puede acabar con volumen actual
        ''' ¿Como calculamos el uso de depositos en funcion de los tests que nos quedan pendientes? ¿Que hacemos si la sesión es muy larga
        ''' y necesitamos más de una botella de agua o de residuos?
        ''' Cuando decidamos dejar de enviar Tests, ¿que significa? ¿enviar END o no enviar nada?
        ''' Por ahora se puede hacer que cuando haya que dejar de enviar preparaciones se debe activar flag pauseSendingTestPreparationsFlag = TRUE
        ''' 
        ''' INSTRUCTIONS:
        ''' (OK)Case HIGH_CONTAMIN_ERR, WASH_CONTANIER_ERR stop sending test but Sw inform the user can not remove or refill container until Ax00 indicates
        ''' (only if the ENDRUN has not been sent yet)
        ''' 
        ''' BUSINESS: Nothing
        ''' 
        ''' PRESENTATION:
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' MDI: When ERROR case button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
        ''' (OK) (StandBy: Disable buttons START, CONTINUE,...)
        ''' (OK) 2 Case HIGH_CONTAMIN_ERR, WASH_CONTANIER_ERR Show message informing the user
        ''' (“No sustituya la botella hasta que el analizador le indique que puede hacerlo con seguridad”) (Ax00MainMDI.ShowAlarmWarningMessages)
        ''' </remarks>
        Private Sub ContainerWarnings(ByVal alarmItem As GlobalEnumerates.Alarms, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, _
                                           ByRef methodHasToAddInstructionToQueueFlag As Integer)
            If alarmItem = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR Or alarmItem = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR Then
                If Not _analyzerManager.AllowScanInRunning() AndAlso Not _analyzerManager.PauseModeIsStartingState AndAlso pAlarmStatusList(index) And _analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                   And Not _analyzerManager.EndRunInstructionSent() And _analyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                    If methodHasToAddInstructionToQueueFlag = 0 Then
                        _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                        If Not _myGlobal.HasError AndAlso _analyzerManager.Connected() Then
                            methodHasToAddInstructionToQueueFlag = 1
                            _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        End If

                    ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                        If _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                            _analyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        End If
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Warnings for reactions temperature and fridge temperature
        ''' </summary>
        ''' <param name="alarmItem"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: Nothing
        ''' 
        ''' BUSINESS:
        ''' AG 05/01/2012 - Sw implements timers
        ''' Manage is the warning timers (cases REACTIONS TEMPERATURE, FRIDGE TEMPERATURE) for change warning to error
        ''' (OK) case REACT_TEMP_WARN -> In method ProcessReadingsReceived
        ''' 
        ''' PRESENTATION: 
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' Bottle or scales levels
        ''' </remarks>
        Private Sub TemperatureWarning(ByVal alarmItem As GlobalEnumerates.Alarms, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            If pAlarmStatusList(index) Then 'Alarm Exists
                'Activate thermo reactions rotor timer for pass from Warning to Error if needed
                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And Not _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() = True
                End If

                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And Not _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() = True
                End If

            Else 'Alarm Solved
                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() = False
                End If

                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And _analyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    _analyzerManager.ThermoReactionsRotorWarningTimerEnabled = False
                End If

            End If
        End Sub

        ''' <summary>
        ''' Alarm detected during WarmUp or during Change Rotor process
        ''' </summary>
        ''' <param name="myAnalyzerFlagsDs"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <param name="index"></param>
        ''' <remarks>
        ''' INSTRUCTIONS: Nothing
        ''' 
        ''' BUSINESS:
        ''' If detected during WarmUp ... PAUSE the process, inform the user to change reactions rotor and finally continue WarmUp process
        ''' If detected during Change Rotor ... inform the user, reset the change rotor process 
        ''' 
        ''' PRESENTATION: 
        ''' Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
        ''' OK: (MDI (StandBy): Disable buttons START, CONTINUE) 
        ''' Change reactions rotor: start process  
        ''' Thermo Alarms (Warnings)
        ''' </remarks>
        Private Sub NoReactionsRotor(ByVal myAnalyzerFlagsDs As AnalyzerManagerFlagsDS, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)
            If pAlarmStatusList(index) Then 'Alarm Exists
                If String.Equals(_analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") Then
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "PAUSED")
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")

                ElseIf String.Equals(_analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess), "INPROCESS") Then
                    'Re-start the whole process 
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "")
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "")
                    _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                End If
            End If
        End Sub

#Region "Aux Functions"
        ''' <summary>
        ''' Activate the Analyzer Alarm Sound if prosuce alarm require sound.
        ''' The analyzer alarm sound must be activate on Analyzer configuration.
        ''' </summary>
        ''' <param name="pSomeInstructionAlreadySent" ></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 27/10/2011
        ''' AG 14/05/2012 - add byRef parameter psomeinstructionAlreadySent. When 0 send Sound instruction, else add to queue</remarks>
        Private Sub SoundActivationByAlarm(ByRef pSomeInstructionAlreadySent As Integer)
            Try
                _myGlobal = GetOpenDBConnection(_dbConnection)
                If (Not _myGlobal.HasError And Not _myGlobal.SetDatos Is Nothing) Then
                    _dbConnection = DirectCast(_myGlobal.SetDatos, SqlConnection)

                    If (Not _dbConnection Is Nothing) Then

                        'if alarm sound is configured an analyzer is not currently ringing then search inside 
                        'the new alarms if any requires sound.
                        If Not _analyzerManager.Ringing Then
                            'Validate if Analyzer Alarm sound is enable.
                            _myGlobal = _analyzerManager.IsAlarmSoundDisable(_dbConnection)
                            If Not _myGlobal.HasError Then
                                'Alarm sound is enable
                                If Not DirectCast(_myGlobal.SetDatos, Boolean) Then
                                    'Validate if Analyzer status is Freeze
                                    Dim sendSoundInstruction As Boolean = False

                                    'Get the new active alarm, Linq over RecivedAlarms
                                    For Each myAlarmRow As UIRefreshDS.ReceivedAlarmsRow In _analyzerManager.ReceivedAlarms.Where(Function(a) a.AlarmStatus)
                                        Dim myAlarmList As List(Of AlarmsDS.tfmwAlarmsRow) = (From a In alarmsDefintionTableDS.tfmwAlarms Where String.Equals(a.AlarmID, myAlarmRow.AlarmID) Select a).ToList()
                                        If myAlarmList.Count = 1 Then
                                            If _analyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso myAlarmList.First().OnRunningSound Then
                                                sendSoundInstruction = True
                                                Exit For
                                            ElseIf myAlarmList.First().Sound Then
                                                sendSoundInstruction = True
                                                Exit For
                                            End If
                                        End If
                                    Next

                                    If sendSoundInstruction Then
                                        If pSomeInstructionAlreadySent = 0 Then
                                            If Not _analyzerManager.QueueContains(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND) Then
                                                _myGlobal = _analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                                                pSomeInstructionAlreadySent = 1
                                            End If
                                        Else
                                            _analyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, "")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                _myGlobal.HasError = True
                _myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                _myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SoundActivation", EventLogEntryType.Error, False)
            Finally
                If Not _dbConnection Is Nothing Then _dbConnection.Close()
            End Try
        End Sub

        ''' <summary>
        ''' If some of the alarms in pAlarmList means analyzer FREEZE then return TRUE
        ''' If none of the alarms in pAlarmList means analyzer FREEZE then return FALSE
        ''' </summary>
        ''' <param name="pAlarmList"></param>
        ''' <returns></returns>
        ''' <remarks>AG 07/03/2012</remarks>
        Private Function ExistFreezeAlarms(ByVal pAlarmList As List(Of GlobalEnumerates.Alarms)) As Boolean
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing
            Dim returnData As Boolean = False

            Try
                resultData = GetOpenDBConnection(_dbConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myLinq As List(Of AlarmsDS.tfmwAlarmsRow)

                        For Each item As GlobalEnumerates.Alarms In pAlarmList
                            myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                      Where String.Equals(a.AlarmID, item.ToString) AndAlso Not a.IsFreezeNull Select a).ToList

                            If myLinq.Count > 0 Then
                                returnData = True 'Currently exist some alarm that means analyzer in FREEZE mode
                                Exit For
                            End If
                        Next
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ExistFreezeAlarms", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection Is Nothing Then dbConnection.Close()

            End Try
            Return returnData
        End Function
#End Region
    End Class
End Namespace