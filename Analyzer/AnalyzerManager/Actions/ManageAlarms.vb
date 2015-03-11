Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities
    Public Class ManageAlarms

        Private AnalyzerManager As IAnalyzerManager

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="analyzerMan"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef analyzerMan As IAnalyzerManager)
            AnalyzerManager = analyzerMan
        End Sub


        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' Note that some alarms has already perfomed previous business (alarms in instructions different than AlarmsDetails,
        ''' for instance ANSBR1, ANSBR2, ANSBM1, STATUS, ...)
        ''' Finally save alarms into Database and prepare DS for UI refresh
        ''' </summary>
        ''' <param name="pdbConnection"></param>
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
        Public Function ManageAlarms(ByVal pdbConnection As SqlConnection, _
                                      ByVal pAlarmIdList As List(Of GlobalEnumerates.Alarms), _
                                      ByVal pAlarmStatusList As List(Of Boolean), _
                                      Optional ByVal pAdditionalInfoList As List(Of String) = Nothing) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                Dim index As Integer = 0
                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

                If Not AnalyzerManager.EndRunInstructionSent() AndAlso AnalyzerManager.AnalyzerCurrentAction() = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                    AnalyzerManager.EndRunInstructionSent() = True
                End If

                Dim methodHasToAddInstructionToQueueFlag As Integer = 0
                'Once one instruction has been sent. The other instructions managed in this method will be added to queue
                '  0 -> Instruction can be sent
                '  1 -> One instruction has already been sent. New instructions to be sent in ManageAlarms will be add to queue
                '  2 -> None instruction can be sent (RESET FREEZE). New instructions to be sent in ManageAlarms wont be add to queue (except SOUND)

                If AnalyzerManager.AnalyzerIsFreeze() Then
                    'except ISE_OFF_ERR that is always generated with status FALSE
                    If ((pAlarmIdList.Count = 1 AndAlso Not pAlarmIdList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIdList.Count > 1) Then
                        AnalyzerManager.ClearQueueToSend() 'Clear all instruction in queue to be sent
                    End If
                    'AG 05/06/2014 - #1657

                    If String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "AUTO") Then
                        'INSTRUCTIONS: NOTHING
                        'BUSINESS: NOTHING
                        'PRESENTATION: Nothing

                    ElseIf String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "PARTIAL") Then 'PARTIAL freeze
                        'INSTRUCTIONS: If Running then send END instruction (if not sent yet)
                        'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                        If Not AnalyzerManager.AllowScanInRunning() AndAlso Not pauseModeIsStarting AndAlso AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                            Not AnalyzerManager.EndRunInstructionSent() AndAlso AnalyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists

                            myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)

                            If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                                'AnalyzerManager.EndRunInstructionSent()= True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                methodHasToAddInstructionToQueueFlag = 1
                                AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                            End If
                        End If

                        'BUSINESS: Nothing

                        'PRESENTATION: Show message box informing the user the worksesion has been paused 'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages +
                        '              When analyzer leaves RUNNING and becomes STANDBY activate Recover button


                    ElseIf String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "TOTAL") Then 'TOTAL freeze
                        'INSTRUCTIONS: If Running then send STANDBY instruction (if not sent yet)
                        If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then 'Alarm Exists
                            'AG 25/09/2012 - do not sent STANDBY if recovery results process is INPROCESS
                            If AnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" Then
                                myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                            End If

                            'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
                            If Not myGlobal.HasError Then
                                methodHasToAddInstructionToQueueFlag = 1
                                AnalyzerManager.EndRunInstructionSent() = True 'AG 14/05/2012 - when StandBy instruction is sent the ENDRUN instruction has no sense!!

                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerManager.ActiveAnalyzer(), AnalyzerManager.ActiveWorkSession(), "ABORTED")
                            End If

                        Else
                            'If new freeze total alarm appears during recover washings ... mark recover process as closed
                            'TODO : Take care with this change! SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) AndAlso SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                            If AnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS" _                            
                            AndAlso AnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                                AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "CLOSED")
                            End If
                        End If


                        'BUSINESS: Nothing

                        'PRESENTATION: Show message box informing the user the worksesion has been aborted + activate Recover button
                        'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages

                    ElseIf String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "RESET") Then
                        'INSTRUCTIONS: Stop the sensor information instructions
                        myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)

                        'BUSINESS: 
                        AnalyzerManager.SetAnalyzerNotReady()

                        'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
                        If Not myGlobal.HasError Then
                            methodHasToAddInstructionToQueueFlag = 2 'AG 14/05/2012

                            If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then 'Only abort work session if analyzer is in Running
                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerManager.ActiveAnalyzer(), AnalyzerManager.ActiveWorkSession(), "ABORTED")
                            End If
                        End If

                        'PRESENTATION: Show message box informing the user the analyzer must be restarted
                        'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages

                    End If
                End If
                Dim alarmsDelg As New WSAnalyzerAlarmsDelegate

                Dim myISEOffErrorFixed As Boolean = False
                Dim myISETimeoutErrorFixed As Boolean = False
                Dim myCOMMSTimeoutErrorFixed As Boolean = False
                For Each alarmItem As GlobalEnumerates.Alarms In pAlarmIdList
                    'General description
                    'Apply special Business depending the alarm code
                    '        1- Launch Sw processes
                    '        2- Automatically send new instruction to the Analyzer
                    'To inform the user we generate an AnalyzerManager.ReceptionEvent. We prepare event data with the 
                    'PrepareUIRefreshEvent method called below the End Select
                    'Buttons activation code in Ax00MainMDI (methods ActivateActionButtonBar, ActiveButtonWithAlarms)

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
                            ContainerWarnings(alarmItem, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag, myGlobal)

                        Case GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR, GlobalEnumerates.Alarms.WATER_SYSTEM_ERR
                            WaterError(pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag, myGlobal)

                        Case GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR, GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR
                            WasteSystemError(myGlobal, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)


                        Case GlobalEnumerates.Alarms.R1_COLLISION_WARN, GlobalEnumerates.Alarms.R2_COLLISION_WARN, _
                            GlobalEnumerates.Alarms.S_COLLISION_WARN
                            NonCriticalCollisions(myGlobal, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                            
                        Case GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN, GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR
                            'Status alarms (Fridge, ISE)
                            'ISE status alarm is only generated when Instrument has ISE module installed - adjust ISEINS:1
                            'GlobalEnumerates.Alarms.ISE_OFF_ERR 'AG 30/05/2012  - comment this line, this alarm has an individual treatment (look for mark 'AG 23/03/2012 - ISE switch off' in this method)

                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'FRIDGE: No business (only inform user)
                            'ISE: Send ISE test preparations are not allowed & inform user
                            'NOTE: All ISE alarms apply only if Ax00 has ISE module

                            'INSTRUCTIONS: Nothing

                            'BUSINESS:
                            'AG 21/03/2012 - this business rule is taken into account in method SendNextPreparation
                            ''Case ISE_STATUS_WARN: Send ISETEST instruction is not allowed
                            'If alarmItem = GlobalEnumerates.Alarms.ISE_STATUS_WARN Then
                            '    If pAlarmStatusList(index) Then 'Alarm Exists
                            '        If alarmItem = GlobalEnumerates.Alarms.ISE_STATUS_WARN Then
                            '            pauseSendingISETestFlag = True
                            '        End If
                            '    Else 'Alarm solved
                            '        If alarmItem = GlobalEnumerates.Alarms.ISE_STATUS_WARN Then
                            '            pauseSendingISETestFlag = False
                            '        End If
                            '    End If
                            'End If

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent


                            'Level Detection alarms
                        Case GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN, GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN, _
                            GlobalEnumerates.Alarms.S_NO_VOLUME_WARN
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'INSTRUCTIONS: Nothing

                            'BUSINESS:
                            'NOTE: Previous business is performed in method ProcessArmStatusRecived

                            'PRESENTATION: Nothing
                            
                        Case GlobalEnumerates.Alarms.CLOT_DETECTION_WARN, GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                            ClotDetection(alarmItem, pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                            BaseLineInitError(myGlobal, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                            BaselineWellWarning(pAlarmStatusList, index)

                        Case GlobalEnumerates.Alarms.RECOVER_ERR
                            RecoverInstructionError(pAlarmStatusList, index)


                        Case GlobalEnumerates.Alarms.R1_COLLISION_ERR, GlobalEnumerates.Alarms.R2_COLLISION_ERR, _
                            GlobalEnumerates.Alarms.S_COLLISION_ERR
                            ArmCollitionError(pAlarmStatusList, index)


                        Case GlobalEnumerates.Alarms.INST_ABORTED_ERR, GlobalEnumerates.Alarms.INST_REJECTED_ERR
                            InstructionAborted(myGlobal, pAlarmStatusList, index, methodHasToAddInstructionToQueueFlag)

                        Case GlobalEnumerates.Alarms.INST_REJECTED_WARN
                            InstructionRejected(myGlobal, dbConnection, myAnalyzerFlagsDs)

                            ''''''''''''
                            'ISE ALARMS
                            ''''''''''''

                        Case GlobalEnumerates.Alarms.ISE_OFF_ERR
                            IseSwitchOff(myGlobal, dbConnection, pAlarmStatusList, index, alarmsDelg, myISEOffErrorFixed)

                        Case GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR
                            IseTimeout(myGlobal, dbConnection, alarmsDelg, myISETimeoutErrorFixed)

                        Case GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR
                            CommsTimeout(myGlobal, dbConnection, alarmsDelg, myCOMMSTimeoutErrorFixed)

                        Case GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR
                            If pAlarmStatusList(index) Then
                                MyClass.RefreshISEAlarms()
                            End If
                            'SGM 19/06/2012


                            'DL 31/07/2012
                        Case GlobalEnumerates.Alarms.FW_CPU_ERR, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR, _
                             GlobalEnumerates.Alarms.FW_REPOSITORY_ERR, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR, _
                             GlobalEnumerates.Alarms.FW_MAN_ERR

                            If pAlarmStatusList(index) Then
                                'INSTRUCTIONS: nothing
                                'BUSINESS: call stopcomm
                                ' Cut off communications channel
                                If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                              True, _
                                                              Nothing, _
                                                              GlobalEnumerates.Ax00InfoInstructionModes.STP)
                                End If

                                If Not myGlobal.HasError Then StopComm()
                                'PRESENTATION: disconnect Showmessage from Presntation
                            End If

                        Case GlobalEnumerates.Alarms.INST_NOALLOW_INS_ERR, GlobalEnumerates.Alarms.INST_REJECTED_WARN
                            'TR BUG #1339 Validate if the last intruction send was pause to star logic
                            If AppLayer.LastInstructionTypeSent = GlobalEnumerates.AppLayerEventList.PAUSE Then
                                PauseAlreadySentFlagAttribute = False
                                SetAllowScanInRunningValue(False) 'AG 08/11/2013   AllowScanInRunningAttribute = False
                                If String.Compare(AnalyzerManager.SessionFlag()(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "INPROCESS", False) = 0 Then
                                    UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                                End If
                            End If
                            'TR BUG #1339 -END
                        Case Else
                            'Nothing
                    End Select
                    index = index + 1
                Next

                'AG 12/03/2012 - Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDs)
                End If
                'AG 12/03/2012

                'Save alarms into DataBase and prepare DS for UI refresh
                'Dim alarmsDelg As New WSAnalyzerAlarmsDelegate 'AG 16/01/2014 - Move definition upper
                Dim wsAlarmsDS As New WSAnalyzerAlarmsDS
                Dim newRowFlag As Boolean = False
                index = 0

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ReceivedAlarms
                    myUI_RefreshDS.ReceivedAlarms.Clear() 'Clear DS used for update presentation layer (only the proper data table)
                End SyncLock

                'Get the alarms defined with OKType = False (never are marked as solved)
                Dim alarmsWithOKTypeFalse As List(Of String) = (From a As AlarmsDS.tfmwAlarmsRow In _
                                                            alarmsDefintionTableDS.tfmwAlarms Where a.OKType = False Select a.AlarmID).ToList

                For Each alarmIDItem As Alarms In pAlarmIdList
                    newRowFlag = False
                    If alarmIDItem <> GlobalEnumerates.Alarms.NONE Then
                        If pAlarmStatusList(index) Then 'Save alarms and also the alarms solved
                            If Not myAlarmListAttribute.Contains(alarmIDItem) Then
                                myAlarmListAttribute.Add(alarmIDItem)
                                newRowFlag = True

                                'AG 24/07/2012 - some alarms are never markt as solved. They can be duplicate in database but not in myAlarmListAttribute
                            ElseIf alarmsWithOKTypeFalse.Contains(alarmIDItem.ToString) Then
                                newRowFlag = True

                            End If
                            'JV 08/01/2014 BT #1118
                        ElseIf myISEOffErrorFixed AndAlso alarmIDItem = GlobalEnumerates.Alarms.ISE_OFF_ERR Then
                            newRowFlag = True
                            'JV 08/01/2014 BT #1118
                            ' XB 04/11/2014 - BA-1872
                        ElseIf myISETimeoutErrorFixed AndAlso alarmIDItem = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                            newRowFlag = True
                            ' XB 04/11/2014 - BA-1872
                            ' XB 06/11/2014 - BA-1872
                        ElseIf myCOMMSTimeoutErrorFixed AndAlso alarmIDItem = GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR Then
                            newRowFlag = True
                            ' XB 06/11/2014 - BA-1872
                        Else 'Fixed alarms
                            If myAlarmListAttribute.Contains(alarmIDItem) Then
                                myAlarmListAttribute.Remove(alarmIDItem)
                                newRowFlag = True
                            End If

                        End If

                        'Add rows into WSAnalyzerAlarmsDS with the new or solved alarms and save them
                        If newRowFlag Then
                            Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                            alarmRow = wsAlarmsDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                            With alarmRow
                                .BeginEdit()
                                .AlarmID = alarmIDItem.ToString
                                .AnalyzerID = AnalyzerManager.ActiveAnalyzer()

                                .AlarmDateTime = Now

                                If AnalyzerManager.ActiveWorkSession() <> "" Then
                                    .WorkSessionID = AnalyzerManager.ActiveWorkSession()
                                Else
                                    .SetWorkSessionIDNull()
                                End If
                                .AlarmStatus = pAlarmStatusList(index)

                                .AlarmItem = 1 'By now (25/03/2011 )always 1

                                'AG 25/07/2012 - v043 code when the parameter pVolumeMissingAdditionalInfo was a single string
                                'in v050 it is defined as a list of string because a single ansbm1 instruction can generate additional info for sample volume missing and also clot warning alarms
                                'If String.IsNullOrEmpty(pVolumeMissingAdditionalInfo) Then
                                '    .SetAdditionalInfoNull() 'By now (25/03/2011 )always NULL
                                'Else
                                '    .AdditionalInfo = pVolumeMissingAdditionalInfo
                                'End If
                                .SetAdditionalInfoNull() 'MI: Added count > index to condition, and item null, as strings can be null to prevent bug being generated
                                If pAdditionalInfoList IsNot Nothing AndAlso pAdditionalInfoList.Count > index AndAlso
                                    pAdditionalInfoList(index) IsNot Nothing AndAlso
                                    String.Equals(pAdditionalInfoList(index), String.Empty) Then
                                    .AdditionalInfo = pAdditionalInfoList(index)
                                End If
                                'AG 25/07/2012

                                .EndEdit()
                            End With

                            wsAlarmsDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                            'Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                            myGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmIDItem.ToString, pAlarmStatusList(index))

                            'JV 08/01/2014 BT #1118
                            If myISEOffErrorFixed Then
                                myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                            End If
                            'JV 08/01/2014 BT #1118

                        End If

                    End If
                    index = index + 1
                Next
                wsAlarmsDS.AcceptChanges()

                If wsAlarmsDS.twksWSAnalyzerAlarms.Rows.Count > 0 Then
                    myGlobal = alarmsDelg.Save(dbConnection, wsAlarmsDS, alarmsDefintionTableDS) 'AG 24/07/2012 change Create for Save who inserts alarms with status TRUE and updates alarms with status FALSE
                    ''TR 26/10/2011 
                    If Not myGlobal.HasError AndAlso Not AnalyzerIsRingingAttribute Then
                        myGlobal = SoundActivationByAlarm(dbConnection, methodHasToAddInstructionToQueueFlag)
                    End If
                    ''TR 26/10/2011 -END.
                End If

                'AG 14/03/2012
                If analyzerFREEZEFlagAttribute Then
                    'AG 05/06/2014 - #1657 - (except ISE_OFF_ERR that is always generated with status FALSE)
                    If ((pAlarmIdList.Count = 1 AndAlso Not pAlarmIdList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIdList.Count > 1) Then
                        If String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "AUTO") Then
                            'BUSINESS: 
                            'Special code for FREEZE (AUTO) - once solved the cover alarms error check if there are any other freeze alarm o not
                            'If not remove the attributes flags (freeze and freeze mode and also update the sensor FREERZE to 0)

                            'NOTE: for call this method here the parameter can not be pAlarmIDList. Instead use the current alarms in analyzer attribute (myAlarmListAttribute)

                            If Not ExistFreezeAlarms(dbConnection, myAlarmListAttribute) Then 'Freeze auto solved
                                'Analyzer has recovered an operating status automatically by closing covers
                                analyzerFREEZEFlagAttribute = False
                                AnalyzerManager.AnalyzerFreezeMode() = ""
                                AnalyzerIsReadyAttribute = True 'Analyzer is ready
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 0, True) 'Inform presentation the analyzer is ready

                                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status
                                    methodHasToAddInstructionToQueueFlag = 1
                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                    If Not myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.STATE) Then
                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.STATE)
                                        myParamsQueue.Add("")
                                    End If
                                End If
                                'AG 14/05/2012

                            Else 'Exists freeze auto
                                'Reset all processes flags
                                Dim flags_dlg As New AnalyzerManagerFlagsDelegate
                                myGlobal = flags_dlg.ResetFlags(dbConnection, AnalyzerManager.ActiveAnalyzer())

                                'Finally read and update our internal structures
                                If Not myGlobal.HasError Then
                                    InitializeAnalyzerFlags(dbConnection)
                                End If

                                'if standby ... activate ansinf (some processes as shutdown, nrotor, ise utilities deactivate it, if freezes
                                'appears while sensors deactivated there is no other way to auto recover
                                If Not myGlobal.HasError Then
                                    If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                                        'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                                        'If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()
                                        If methodHasToAddInstructionToQueueFlag = 0 Then
                                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                                            methodHasToAddInstructionToQueueFlag = 1
                                            'AG 04/04/2012 - When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                            If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()
                                        ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                            ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                            If Not myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.INFO) Then
                                                myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.INFO)
                                                'AG 10/12/2012 - Do not use ToString here because Sw will send INFO;Q:STR; instead of INFO;Q:3;
                                                'myParamsQueue.Add(GlobalEnumerates.Ax00InfoInstructionModes.STR.ToString)
                                                myParamsQueue.Add(CInt(GlobalEnumerates.Ax00InfoInstructionModes.STR))
                                            End If
                                        End If
                                        'AG 14/05/2012
                                    End If
                                End If
                            End If

                        ElseIf Not String.Equals(AnalyzerManager.AnalyzerFreezeMode(), "PARTIAL") Then
                            'Remove all flags ... recover is required
                            Dim flags_dlg As New AnalyzerManagerFlagsDelegate
                            myGlobal = flags_dlg.ResetFlags(dbConnection, AnalyzerManager.ActiveAnalyzer())
                        End If

                    End If
                End If
                'AG 14/03/2012

                alarmsWithOKTypeFalse = Nothing

                'If (Not myGlobal.HasError) Then
                '    'When the Database Connection was opened locally, then the Commit is executed
                '    If (pdbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                'Else
                '    'When the Database Connection was opened locally, then the Rollback is executed
                '    If (pdbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                'End If
                '    End If
                'End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAlarms", EventLogEntryType.Error, False)
                GlobalBase.CreateLogActivity(ex)


                'Finally
                '    If (pdbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobal
        End Function

        Private Sub CommsTimeout(ByRef myGlobal As GlobalDataTO, ByVal dbConnection As SqlConnection, ByVal alarmsDelg As WSAnalyzerAlarmsDelegate, ByRef myCOMMSTimeoutErrorFixed As Boolean)

            myGlobal = alarmsDelg.GetByAlarmID(dbConnection, GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR.ToString, Nothing, Nothing, AnalyzerManager.ActiveAnalyzer(), "")
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                Dim temporalDS As New WSAnalyzerAlarmsDS
                temporalDS = DirectCast(myGlobal.SetDatos, WSAnalyzerAlarmsDS)

                'Search if exists alarm COMMS_TIMEOUT_ERR with status TRUE, in this case set flag myCOMMSTimeoutErrorFixed = True (FIXED)
                'in order to mark it as fixed
                Dim auxList As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDS.twksWSAnalyzerAlarms _
                    Where a.AlarmID = GlobalEnumerates.Alarms.COMMS_TIMEOUT_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                If auxList.Count > 0 Then
                    myCOMMSTimeoutErrorFixed = True
                End If
                auxList = Nothing
            End If
            ' XB 06/11/2014 - BA-1872

            'SGM 19/06/2012
        End Sub

        Private Sub IseTimeout(ByRef myGlobal As GlobalDataTO, ByVal dbConnection As SqlConnection, ByVal alarmsDelg As WSAnalyzerAlarmsDelegate, ByRef myISETimeoutErrorFixed As Boolean)

            myGlobal = alarmsDelg.GetByAlarmID(dbConnection, GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR.ToString, Nothing, Nothing, AnalyzerManager.ActiveAnalyzer(), "")
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                Dim temporalDS As New WSAnalyzerAlarmsDS
                temporalDS = DirectCast(myGlobal.SetDatos, WSAnalyzerAlarmsDS)

                'Search if exists alarm ISE_TIMEOUT_ERR with status TRUE, in this case set flag myISETimeoutErrorFixed = True (FIXED)
                'in order to mark it as fixed
                Dim auxList As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDS.twksWSAnalyzerAlarms _
                    Where a.AlarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                If auxList.Count > 0 Then
                    myISETimeoutErrorFixed = True
                End If
                auxList = Nothing
            End If
            ' XB 04/11/2014 - BA-1872

            ' XB 06/11/2014 - BA-1872
        End Sub

        Private Sub IseSwitchOff(ByRef myGlobal As GlobalDataTO, ByVal dbConnection As SqlConnection, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByVal alarmsDelg As WSAnalyzerAlarmsDelegate, ByRef myISEOffErrorFixed As Boolean)

            'AG 23/03/2012 - ISE switch off
            If pAlarmStatusList(index) Then
                ISEAnalyzer.IsISESwitchON = False 'SGM 29/06/2012
                MyClass.RefreshISEAlarms() 'SGM 19/06/2012

                'When this alarm appears in RUNNING: Lock all pending ISE preparations
                If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    Dim exec_delg As New ExecutionsDelegate

                    '1st: Get all ISE executions pending (required to inform the UI with the locked executions
                    myGlobal = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerManager.ActiveAnalyzer(), AnalyzerManager.ActiveWorkSession(), "PENDING", False, "PREP_ISE")
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        Dim myExecutionsDS As New ExecutionsDS

                        '2on: Lock all pending ISE preparation executions
                        myGlobal = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, AnalyzerManager.ActiveWorkSession(), AnalyzerManager.ActiveAnalyzer(), "PREP_ISE", "PENDING", "LOCKED")

                        '3rd: Prepare data for generate user interface refresh event
                        If Not myGlobal.HasError Then
                            For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                myGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                If myGlobal.HasError Then Exit For
                            Next
                        End If

                    End If

                End If
                'JV 08/01/2014 BT #1118
            Else
                'AG 16/01/2014 - Mark IseOffError as FIXED only when exists the alarm ISE_OFF_ERR in database with status TRUE
                ' Previous code causes the MONITOR screen refreshs always each 2 seconds because each ANSINF causes new alarm notification ISE_OFF_ERR with status false
                'myISEOffsErrorFixed = True
                myGlobal = alarmsDelg.GetByAlarmID(dbConnection, GlobalEnumerates.Alarms.ISE_OFF_ERR.ToString, Nothing, Nothing, AnalyzerManager.ActiveAnalyzer(), "")
                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    Dim temporalDS As New WSAnalyzerAlarmsDS
                    temporalDS = DirectCast(myGlobal.SetDatos, WSAnalyzerAlarmsDS)

                    'Search if exists alarm ISE_OFF_ERR with status TRUE, in this case set flag myISEOffsErrorFixed = True (FIXED)
                    'in order to mark it as fixed
                    Dim auxList As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                    auxList = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In temporalDS.twksWSAnalyzerAlarms _
                        Where a.AlarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR.ToString AndAlso a.AlarmStatus = True Select a).ToList
                    If auxList.Count > 0 Then
                        myISEOffErrorFixed = True
                    End If
                    auxList = Nothing
                End If
                'AG 16/01/2014 - JV 08/01/2014 BT #1118

            End If

            'AG 23/03/2012

            ' XB 04/11/2014 - BA-1872
        End Sub

        Private Sub InstructionRejected(ByRef myGlobal As GlobalDataTO, ByVal dbConnection As SqlConnection, ByVal myAnalyzerFlagsDs As AnalyzerManagerFlagsDS)

            'TR BUG #1339  before sending the next instruction, validate if the last intruction send was pause to star logic
            If AppLayer.LastInstructionTypeSent = GlobalEnumerates.AppLayerEventList.PAUSE Then
                PauseAlreadySentFlagAttribute = False
                SetAllowScanInRunningValue(False) 'AG 08/11/2013 #1358  AllowScanInRunningAttribute = False
                If String.Compare(AnalyzerManager.SessionFlag()(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "INPROCESS", False) = 0 Then
                    UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                End If
            End If
            'TR BUG #1339 -END

            'Sw has received a Request but has sent instruction out of time.
            'Search for next instruction to be sent but using next well (only in Running)
            If AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                Dim reactRotorDlg As New ReactionsRotorDelegate
                futureRequestNextWell = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + 1, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)
                myGlobal = Me.SearchNextPreparation(dbConnection, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
                    myNextPreparationToSendDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)
                End If
            End If


            'Debug print to leave developent traces
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity("Instruction rejected (out of time)", "AnalyzerManager.ManageAlarms", EventLogEntryType.Information, False)
            Debug.Print(Now.ToString & " .Instruction rejected (out of time)")
        End Sub

        Private Sub InstructionAborted(ByRef myGlobal As GlobalDataTO, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            'Instructions aborted or rejected
            If pAlarmStatusList(index) Then
                'INSTRUCTIONS: ask for detailed errors
                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR) 'AG 12/03/2012: (INFO;Q:ALR)
                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR)
                    methodHasToAddInstructionToQueueFlag = 1
                    'AG 04/04/2012 - When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()
                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                    If Not myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.INFO) Then
                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.INFO)
                        'AG 10/12/2012 - Do not use ToString here because Sw will send INFO;Q:ALR; instead of INFO;Q:2;
                        'myParamsQueue.Add(GlobalEnumerates.Ax00InfoInstructionModes.ALR.ToString)
                        myParamsQueue.Add(CInt(GlobalEnumerates.Ax00InfoInstructionModes.ALR))
                    End If
                End If
                'AG 14/05/2012

                'BUSINESS: NOTHING
                'PRESENTATION: NOTHING
            End If
        End Sub

        Private Sub ArmCollitionError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            'Arm collision error (error codes)
            If pAlarmStatusList(index) Then 'Alarm Exists
                'Mark all INPROCESS executions as PENDING or LOCKED
                'AG 08/03/2012 - This method is called only when Software receives ANSBxx with collision information
                'Dim exec_dlg As New ExecutionsDelegate
                'myGlobal = exec_dlg.ChangeINPROCESSStatusByCollision(dbConnection, AnalyzerManager.ActiveAnalyzer(), AnalyzerManager.ActiveWorkSession())
            End If
        End Sub

        Private Sub RecoverInstructionError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer)

            'Recover instruction error
            If pAlarmStatusList(index) Then 'Alarm Exists
                'The recover instruction has finished with error
                If recoverAlreadySentFlagAttribute Then recoverAlreadySentFlagAttribute = False
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished (but with errors)
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
                AnalyzerManager.WELLbaselineParametersFailures = True
            Else
                AnalyzerManager.WELLbaselineParametersFailures = False
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
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
        Private Sub BaseLineInitError(ByRef myGlobal As GlobalDataTO, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            If pAlarmStatusList(index) Then 'Alarm Exists
                If AnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY AndAlso AnalyzerManager.BaselineInitializationFailures < ALIGHT_INIT_FAILURES Then

                    'AG 27/11/2014 BA-2144 comment code, business will be implemented in method SendAutomaticALIGHTRerun 
                    ''When ALIGHT has been rejected ... increment the variable CurrentWellAttribute to perform the new in other well
                    'Dim SwParams As New SwParametersDelegate
                    'myGlobal = SwParams.GetParameterByAnalyzer(dbConnection, AnalyzerManager.ActiveAnalyzer(), GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString, False)
                    'If Not myGlobal.HasError Then
                    '    Dim SwParamsDS As New ParametersDS
                    '    SwParamsDS = CType(myGlobal.SetDatos, ParametersDS)
                    '    If SwParamsDS.tfmwSwParameters.Rows.Count > 0 Then
                    '        Dim reactionsDelegate As New ReactionsRotorDelegate
                    '        CurrentWellAttribute = reactionsDelegate.GetRealWellNumber(CurrentWellAttribute + 1, CInt(SwParamsDS.tfmwSwParameters(0).ValueNumeric))
                    '    End If
                    'End If

                    ''AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                    ''myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, CurrentWellAttribute)
                    'If methodHasToAddInstructionToQueueFlag = 0 Then
                    '    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, CurrentWellAttribute)
                    '    methodHasToAddInstructionToQueueFlag = 1
                    'ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    '    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                    '    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ADJUST_LIGHT) Then
                    '        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT)
                    '        myParamsQueue.Add(CurrentWellAttribute.ToString)
                    '    End If
                    'End If
                    ''AG 14/05/2012

                    ''When a process involve an instruction sending sequence automatic change the AnalyzerIsReady value
                    'If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()

                ElseIf AnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    Select Case AnalyzerManager.wellBaseLineAutoPausesSession
                        Case 1, 2 'END 
                            If Not AnalyzerManager.AllowScanInRunning AndAlso Not AnalyzerManager.PauseModeIsStartingState AndAlso _
                                Not AnalyzerManager.EndRunInstructionSent() AndAlso AnalyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    If Not myGlobal.HasError AndAlso AnalyzerManager.Connected Then
                                        methodHasToAddInstructionToQueueFlag = 1
                                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                    End If

                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    If AnalyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                    End If
                                End If
                            End If
                    End Select
                End If
            Else
                AnalyzerManager.ResetBaseLineFailuresCounters()
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
                    Dim adjustValue = AnalyzerManager.ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.CLOT)
                    If Not String.Equals(adjustValue, String.Empty) AndAlso IsNumeric(adjustValue) Then
                        clotDetectionEnabled = CType(adjustValue, Boolean)
                    End If
                    
                End If

            Else 'Alarm solved
                If alarmItem = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR Then
                    AnalyzerManager.PauseSendingTestPreparations = False
                End If
            End If
        End Sub

        ''' <summary>
        ''' Non critical collisions (R1, R2 or S arms)
        ''' </summary>
        ''' <param name="myGlobal"></param>
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
        Private Sub NonCriticalCollisions(ByRef myGlobal As GlobalDataTO, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)
            
            If Not AnalyzerManager.AllowScanInRunning() AndAlso Not AnalyzerManager.PauseModeIsStartingState() AndAlso pAlarmStatusList(index) And _
                AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not AnalyzerManager.EndRunInstructionSent _
               And AnalyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not myGlobal.HasError AndAlso AnalyzerManager.Connected Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    If Not AnalyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                    End If
                End If
            End If

            
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
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
        Private Sub WasteSystemError(ByRef myGlobal As GlobalDataTO, ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer)

            
            If pAlarmStatusList(index) And AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not AnalyzerManager.EndRunInstructionSent _
               And AnalyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    If AnalyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
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
        Private Sub WaterError(ByVal pAlarmStatusList As List(Of Boolean), ByVal index As Integer, ByRef methodHasToAddInstructionToQueueFlag As Integer, ByRef myGlobal As GlobalDataTO)

            If Not AnalyzerManager.AllowScanInRunning AndAlso Not AnalyzerManager.PauseModeIsStartingState() AndAlso pAlarmStatusList(index) And _
               AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not AnalyzerManager.EndRunInstructionSent _
               And AnalyzerManager.AnalyzerCurrentAction <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                If methodHasToAddInstructionToQueueFlag = 0 Then
                    myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        methodHasToAddInstructionToQueueFlag = 1
                    End If

                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                    If AnalyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                        AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
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
                                           ByRef methodHasToAddInstructionToQueueFlag As Integer, ByRef myGlobal As GlobalDataTO)
            If alarmItem = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR Or alarmItem = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR Then
                'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                If Not AnalyzerManager.AllowScanInRunning() AndAlso Not AnalyzerManager.PauseModeIsStartingState AndAlso pAlarmStatusList(index) And AnalyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                   And Not AnalyzerManager.EndRunInstructionSent() And AnalyzerManager.AnalyzerCurrentAction() <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                    If methodHasToAddInstructionToQueueFlag = 0 Then
                        myGlobal = AnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                        If Not myGlobal.HasError AndAlso AnalyzerManager.Connected() Then
                            methodHasToAddInstructionToQueueFlag = 1
                            AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                        End If

                    ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                        If AnalyzerManager.QueueAdds(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, "") Then
                            AnalyzerManager.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
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
                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And Not AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() = True
                End If

                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And Not AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() = True
                End If

            Else 'Alarm Solved
                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() = False
                End If

                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled() Then
                    AnalyzerManager.ThermoReactionsRotorWarningTimerEnabled = False
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
                If String.Equals(AnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") Then
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "PAUSED")
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")

                ElseIf String.Equals(AnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess), "INPROCESS") Then
                    'Re-start the whole process 
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "")
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "")
                    AnalyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                End If
            End If
        End Sub
    End Class
End Namespace