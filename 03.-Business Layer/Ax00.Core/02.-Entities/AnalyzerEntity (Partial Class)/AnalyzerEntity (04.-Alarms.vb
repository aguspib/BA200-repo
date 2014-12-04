Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports System.Timers
Imports System.Data
Imports System.ComponentModel 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Globalization    ' XBC 29/01/2013 - change IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerEntity

#Region "Main Manage Alarms treatment"

        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' Note that some alarms has already perfomed previous business (alarms in instructions different than AlarmsDetails,
        ''' for instance ANSBR1, ANSBR2, ANSBM1, STATUS, ...)
        ''' Finally save alarms into Database and prepare DS for UI refresh
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAlarmIDList">List of alarms to treat</param>
        ''' <param name="pAlarmStatusList" >For each alarm in pAlarmCode indicates his status: TRUE alarm exists, FALSE alarm solved</param>
        ''' <param name="pAdditionalInfoList">Additional info for some alams: volume missing, prep clot warnings, prep locked</param>
        ''' <returns>Global Data To indicating when an error has occurred </returns>
        ''' <remarks>
        ''' Created by:  AG 16/03/2011
        ''' Modified by: SA 29/06/2012 - Removed the OpenDBTransaction; all functions called by this method should open their own DBTransaction
        '''                              or DBConnection, depending if they update or read data - This is to avoid locks between the different
        '''                              threads in execution
        ''' AG 25/072012 pAdditionalInfoList
        ''' AG 30/11/2012 - Do not inform the attribute endRunAlreadySentFlagAttribute as TRUE when call the ManageAnalyzer with ENDRUN or when you add it to the queue
        '''                 This flag will be informed once the instruction has been really sent. Current code causes that sometimes the ENDRUN instruction is added to
        '''                 queue but how the flag is informed before send the instruction it wont never be sent!!
        ''' AG 22/05/2014 - #1637 Use exclusive lock (multithread protection)
        ''' AG 05/06/2014 - #1657 Protection (provisional solution)! (do not clear instructions queue when there are only 1 alarm and it is ISE_OFF_ERR)
        '''               - PENDING FINAL SOLUTION: AlarmID ISE_OFF_ERR must be generated only 1 time when alarm appears, and only 1 time when alarm is fixed (now this alarm with status FALSE is generated with each ANSINF received)
        ''' </remarks>
        Private Function ManageAlarms(ByVal pdbConnection As SqlClient.SqlConnection, _
                                      ByVal pAlarmIDList As List(Of GlobalEnumerates.Alarms), _
                                      ByVal pAlarmStatusList As List(Of Boolean), _
                                      Optional ByVal pAdditionalInfoList As List(Of String) = Nothing) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'myGlobal = DAOBase.GetOpenDBTransaction(pdbConnection)
                'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                '    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then

                'AG 25/09/2012 - comment an use the attribute endRunAlreadySentFlagAttribute
                'Dim endRunInstructionSent As Boolean = endRunAlreadySentFlagAttribute 'False 'Only send one ENDRUN while looping but send none if it has been sent before

                Dim index As Integer = 0
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS 'AG 12/03/2012

                If Not endRunAlreadySentFlagAttribute AndAlso AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.END_RUN_START Then
                    endRunAlreadySentFlagAttribute = True
                End If

                Dim methodHasToAddInstructionToQueueFlag As Integer = 0 'AG 14/05/2012 - Once one instruction has been sent. The other instructions managed in this method will be added to queue
                '                                                              0 -> Instruction can be sent
                '                                                              1 -> One instruction has already been sent. New instructions to be sent in ManageAlarms will be add to queue
                '                                                              2 -> None instruction can be sent (RESET FREEZE). New instructions to be sent in ManageAlarms wont be add to queue (except SOUND)

                'AG 06/03/2012 - Freeze mode treatment
                If analyzerFREEZEFlagAttribute Then
                    'AG 05/06/2014 - #1657 - 'AG + XB 08/04/2014 - #1118 (except ISE_OFF_ERR that is always generated with status FALSE)
                    'ClearQueueToSend() 'Clear all instruction in queue to be sent
                    If ((pAlarmIDList.Count = 1 AndAlso Not pAlarmIDList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIDList.Count > 1) Then
                        ClearQueueToSend() 'Clear all instruction in queue to be sent
                    End If
                    'AG 05/06/2014 - #1657

                    If String.Equals(analyzerFREEZEModeAttribute, "AUTO") Then
                        'INSTRUCTIONS: NOTHING
                        'BUSINESS: NOTHING
                        'PRESENTATION: Nothing

                    ElseIf String.Equals(analyzerFREEZEModeAttribute, "PARTIAL") Then 'PARTIAL freeze
                        'INSTRUCTIONS: If Running then send END instruction (if not sent yet)
                        'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                        'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                        If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting AndAlso AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                            Not endRunAlreadySentFlagAttribute AndAlso AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists

                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)

                            If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                methodHasToAddInstructionToQueueFlag = 1 'AG 14/05/2012
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                            End If
                        End If

                        'BUSINESS: Nothing

                        'PRESENTATION: Show message box informing the user the worksesion has been paused 'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages +
                        '              When analyzer leaves RUNNING and becomes STANDBY activate Recover button


                    ElseIf String.Equals(analyzerFREEZEModeAttribute, "TOTAL") Then 'TOTAL freeze
                        'INSTRUCTIONS: If Running then send STANDBY instruction (if not sent yet)
                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then 'Alarm Exists
                            'AG 25/09/2012 - do not sent STANDBY if recovery results process is INPROCESS
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                            End If

                            'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
                            If Not myGlobal.HasError Then
                                ''AG 11/12/2012 not integrated in v1.0.0
                                'If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                                '    AnalyzerIsReadyAttribute = False
                                'End If
                                ''AG 11/12/2012
                                methodHasToAddInstructionToQueueFlag = 1 'AG 14/05/2012
                                endRunAlreadySentFlagAttribute = True 'AG 14/05/2012 - when StandBy instruction is sent the ENDRUN instruction has no sense!!

                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")
                            End If

                            'AG 16/04/2012
                        Else
                            'If new freeze total alarm appears during recover washings ... mark recover process as closed
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess.ToString) = "INPROCESS" _
                            AndAlso SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) _
                            AndAlso SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "CLOSED")
                            End If
                            'AG 16/04/2012
                        End If


                        'BUSINESS: Nothing

                        'PRESENTATION: Show message box informing the user the worksesion has been aborted + activate Recover button
                        'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages

                    ElseIf String.Equals(analyzerFREEZEModeAttribute, "RESET") Then
                        'INSTRUCTIONS: Stop the sensor information instructions
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)

                        'BUSINESS: 
                        SetAnalyzerNotReady() 'AG 07/03/2012 - analyzer is not ready to perform anything but CONNECT

                        'WorkSession aborted (not necessary to sent the ABORT instruction because the Fw has stopped automatically!!!)
                        If Not myGlobal.HasError Then
                            methodHasToAddInstructionToQueueFlag = 2 'AG 14/05/2012

                            If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then 'Only abort work session if analyzer is in Running
                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")
                            End If
                        End If

                        'PRESENTATION: Show message box informing the user the analyzer must be restarted
                        'IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages

                    End If
                End If
                'AG 06/03/2012
                Dim alarmsDelg As New WSAnalyzerAlarmsDelegate 'AG 16/01/2014 - Move declaration here!

                Dim myISEOffErrorFixed As Boolean = False  'JV 08/01/2014 BT #1118
                For Each alarmItem As GlobalEnumerates.Alarms In pAlarmIDList
                    'General description
                    'Apply special Business depending the alarm code
                    '        1- Launch Sw processes
                    '        2- Automatically send new instruction to the Analyzer
                    'To inform the user we generate an AnalyzerManager.ReceptionEvent. We prepare event data with the 
                    'PrepareUIRefreshEvent method called below the End Select
                    'Buttons activation code in Ax00MainMDI (methods ActivateActionButtonBar, ActiveButtonWithAlarms)

                    Select Case alarmItem
                        'No reactions rotor
                        Case GlobalEnumerates.Alarms.REACT_MISSING_ERR
                            'AG 12/03/2012
                            'Alarm detected during WarmUp or during Change Rotor process
                            'INSTRUCTIONS: Nothing

                            'BUSINESS:
                            'If detected during WarmUp ... PAUSE the process, inform the user to change reactions rotor and finally continue WarmUp process
                            'If detected during Change Rotor ... inform the user, reset the change rotor process 
                            If pAlarmStatusList(index) Then 'Alarm Exists
                                If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS") Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "PAUSED")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")

                                ElseIf String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString), "INPROCESS") Then
                                    'Re-start the whole process 
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                                End If
                            End If

                            'PRESENTATION: 
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
                            'OK: (MDI (StandBy): Disable buttons START, CONTINUE)
                            '    Change reactions rotor: start process 

                            'DL 31/07/2012. Begin
                            'Thermo Alarms (Warnings)
                        Case GlobalEnumerates.Alarms.REACT_TEMP_WARN, GlobalEnumerates.Alarms.R1_TEMP_WARN, _
                             GlobalEnumerates.Alarms.R2_TEMP_WARN, GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN, _
                             GlobalEnumerates.Alarms.WS_TEMP_WARN, GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR, _
                             GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR, GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR

                            'Thermo Alarms (Warnings)
                            'Case GlobalEnumerates.Alarms.REACT_TEMP_WARN, GlobalEnumerates.Alarms.R1_TEMP_WARN, GlobalEnumerates.Alarms.R2_TEMP_WARN, _
                            'GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN, GlobalEnumerates.Alarms.WS_TEMP_WARN, GlobalEnumerates.Alarms.WS_TEMP_SYS1_ERR, _
                            'GlobalEnumerates.Alarms.WS_TEMP_SYS2_ERR, GlobalEnumerates.Alarms.R1_TEMP_SYS1_ERR, GlobalEnumerates.Alarms.R1_TEMP_SYS2_ERR, _
                            'GlobalEnumerates.Alarms.R2_TEMP_SYS1_ERR, GlobalEnumerates.Alarms.R2_TEMP_SYS2_ERR
                            'DL 31/07/2012. End
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'Sw has already determined Temp is out of limits and generate Warning (CheckTemperaturesAlarms method)

                            'INSTRUCTIONS: Nothing

                            'BUSINESS:
                            'AG 05/01/2012 - Sw implements timers
                            'Manage is the warning timers (cases REACTIONS TEMPERATURE, FRIDGE TEMPERATURE) for change warning to error
                            If pAlarmStatusList(index) Then 'Alarm Exists
                                'Activate thermo reactions rotor timer for pass from Warning to Error if needed
                                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And Not thermoReactionsRotorWarningTimer.Enabled Then
                                    thermoReactionsRotorWarningTimer.Enabled = True
                                End If

                                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And Not thermoFridgeWarningTimer.Enabled Then
                                    thermoFridgeWarningTimer.Enabled = True
                                End If

                            Else 'Alarm Solved
                                If alarmItem = GlobalEnumerates.Alarms.REACT_TEMP_WARN And thermoReactionsRotorWarningTimer.Enabled Then
                                    thermoReactionsRotorWarningTimer.Enabled = False
                                End If

                                If alarmItem = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN And thermoFridgeWarningTimer.Enabled Then
                                    thermoFridgeWarningTimer.Enabled = False
                                End If

                            End If
                            'AG 05/01/2012

                            '(OK) case REACT_TEMP_WARN -> In method ProcessReadingsReceived
                            '                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_WARN) Then
                            '                            Mark the results with ThermoWarningFlag 

                            'AG 06/03/2012
                            ''(OK) recover instruction is needed
                            'If (alarmItem = GlobalEnumerates.Alarms.R1_TEMP_WARN OrElse alarmItem = GlobalEnumerates.Alarms.R2_TEMP_WARN OrElse _
                            '    alarmItem = GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR OrElse alarmItem = GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR) Then
                            '    myPendingRecoveryInstructionFlagAttribute = pAlarmStatusList(index)
                            'End If


                            'PRESENTATION: 
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent


                            'Bottle or scales levels
                        Case GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN, GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR, _
                            GlobalEnumerates.Alarms.WASH_CONTAINER_WARN, GlobalEnumerates.Alarms.WASH_CONTAINER_ERR
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'Sw has already determined Warning or not (CheckContainerAlarms method)

                            'LIMITE_WARNING. LIMITE_CRITICO
                            'TODO
                            'Antes de lanzar la sesion (START o CONTINUE) calcular si se puede acabar con volumen actual
                            '¿Como calculamos el uso de depositos en funcion de los tests que nos quedan pendientes? ¿Que hacemos si la sesión es muy larga
                            'y necesitamos más de una botella de agua o de residuos?
                            'Cuando decidamos dejar de enviar Tests, ¿que significa? ¿enviar END o no enviar nada?

                            'Por ahora se puede hacer que cuando haya que dejar de enviar preparaciones se debe activar flag pauseSendingTestPreparationsFlag = TRUE
                            'If pAlarmStatusList(index) Then pauseSendingTestPreparationsFlag = TRUE
                            'If Not pAlarmStatusList(index) Then pauseSendingTestPreparationsFlag = FALSE


                            'INSTRUCTIONS:
                            '(OK)Case HIGH_CONTAMIN_ERR, WASH_CONTANIER_ERR stop sending test but Sw inform the user can not remove or refill container until Ax00 indicates
                            '(only if the ENDRUN has not been sent yet)
                            If alarmItem = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR Or alarmItem = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR Then
                                'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                                'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                                If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting AndAlso pAlarmStatusList(index) And AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                                   And Not endRunAlreadySentFlagAttribute And AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                                    'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                    'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    'If Not myGlobal.HasError AndAlso ConnectedAttribute Then endRunAlreadySentFlagAttribute = True
                                    If methodHasToAddInstructionToQueueFlag = 0 Then
                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                            'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                            methodHasToAddInstructionToQueueFlag = 1
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                                        End If

                                    ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                        ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                        If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                            myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                            myParamsQueue.Add("")
                                            'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                        End If
                                    End If
                                    'AG 14/05/2012
                                End If
                            End If

                            'BUSINESS: Nothing

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
                            'MDI: When ERROR case button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
                            '(OK) (StandBy: Disable buttons START, CONTINUE,...)

                            '(OK) 2 Case HIGH_CONTAMIN_ERR, WASH_CONTANIER_ERR Show message informing the user
                            '        (“No sustituya la botella hasta que el analizador le indique que puede hacerlo con seguridad”) (Ax00MainMDI.ShowAlarmWarningMessages)



                        Case GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR, GlobalEnumerates.Alarms.WATER_SYSTEM_ERR
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'S&S
                            'When Ax00 stop running do not allow continue, block machine and show message
                            'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                            'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                            If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting AndAlso pAlarmStatusList(index) And AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not endRunAlreadySentFlagAttribute _
                            And AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then endRunAlreadySentFlagAttribute = True
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                                        methodHasToAddInstructionToQueueFlag = 1
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                    End If

                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                        myParamsQueue.Add("")
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                    End If
                                End If
                                'AG 14/05/2012
                            End If

                            'BUSINESS: Nothing

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
                            'MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
                            '(OK) 2 Show message informing the user ("Revise el depósito de entrada, o la conexión a la red de agua") (Ax00MainMDI.ShowAlarmWarningMessages)
                            'OK: (StandBy: Disable buttons START, CONTINUE,...)


                        Case GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR, GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'Inform the user and S&S

                            'INSTRUCTIONS: (only if the ENDRUN has not been sent yet)
                            If pAlarmStatusList(index) And AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not endRunAlreadySentFlagAttribute _
                            And AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then endRunAlreadySentFlagAttribute = True
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                                        methodHasToAddInstructionToQueueFlag = 1
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                    End If

                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                        myParamsQueue.Add("")
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                    End If
                                End If
                                'AG 14/05/2012
                            End If

                            'BUSINESS: Nothing

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent
                            'MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)
                            '(OK) 2) Show message when Ax00 stops ("Revise la salida de residuos") (Ax00MainMDI.ShowAlarmWarningMessages)
                            'OK: (StandBy: Disable buttons START, CONTINUE,...)


                            'Non critical collisions (R1, R2 or S arms)
                        Case GlobalEnumerates.Alarms.R1_COLLISION_WARN, GlobalEnumerates.Alarms.R2_COLLISION_WARN, _
                            GlobalEnumerates.Alarms.S_COLLISION_WARN
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'Sw sends Ax00 to S&S
                            'Sw activates CONTINE or ABORT (Recover will be sent automatically before send the button instruction)

                            'INSTRUCTIONS: S&S (only if the ENDRUN has not been sent yet)
                            'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                            'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                            If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting AndAlso pAlarmStatusList(index) And AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not endRunAlreadySentFlagAttribute _
                            And AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                'If Not myGlobal.HasError AndAlso ConnectedAttribute Then endRunAlreadySentFlagAttribute = True
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                                        methodHasToAddInstructionToQueueFlag = 1
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                    End If

                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                        myParamsQueue.Add("")
                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                    End If
                                End If
                                'AG 14/05/2012

                            End If

                            'BUSINESS:
                            'NOTE: Previous business is performed in method ProcessArmStatusRecived

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent


                            'Status alarms (Fridge, ISE)
                            'ISE status alarm is only generated when Instrument has ISE module installed - adjust ISEINS:1
                        Case GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN, GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR
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


                            'Clot detection
                        Case GlobalEnumerates.Alarms.CLOT_DETECTION_WARN, GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                            '[2011-03-17 - Llistat-SensorsActuadors.xls]
                            'INSTRUCTIONS: Nothing

                            'BUSINESS:
                            'NOTE: Previous business is performed in method ProcessArmStatusRecived (mark preparation results as possible clot)
                            'Case CLOT_DETECTION_ERR -> Stop sending test preparations
                            If pAlarmStatusList(index) Then 'Alarm Exists
                                If alarmItem = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR Then

                                    'AG 16/02/2012 - Do not use the pauseSendingTestPreparationsFlag because Sw leave sending preparations but never
                                    'sends the ENDRUN instruction ... I comment the old line and implement that when CLOT_DETECTION_ERR exits Sw sends ENDRUN
                                    ''AG 27/10/2011 - Set to TRUE the flag pause sending test when the clot detection is enabled
                                    ''pauseSendingTestPreparationsFlag = True
                                    Dim clotDetectionEnabled As Boolean = True
                                    Dim adjustValue As String = ""
                                    adjustValue = ReadAdjustValue(Ax00Adjustsments.CLOT)
                                    If Not String.Equals(adjustValue, String.Empty) AndAlso IsNumeric(adjustValue) Then
                                        clotDetectionEnabled = CType(adjustValue, Boolean)
                                    End If

                                    If clotDetectionEnabled Then

                                        'AG 17/07/2012 - CANCELLED again (JE+RP+ST+EF) !! Bugs Tracking 706

                                        ''AG 19/06/2012 - Re activated!!
                                        ''AG 15/03/2012 - CANCELLED - reunion seguimiento proyecto 13/03/2012: JE, RP, EF, ST, JM (se refleja en excel Instrucciones Ax00 (rev 40).xls
                                        ''AG 16/02/2012
                                        ''pauseSendingTestPreparationsFlag = True
                                        'If pAlarmStatusList(index) And AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And Not endRunAlreadySentFlagAttribute _
                                        '    And AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'Alarm Exists
                                        '    'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                        '    'If Not myGlobal.HasError AndAlso ConnectedAttribute Then endRunAlreadySentFlagAttribute = True
                                        '    If methodHasToAddInstructionToQueueFlag = 0 Then
                                        '        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                        '        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        '            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 15/05/2012
                                        '            methodHasToAddInstructionToQueueFlag = 1
                                        '            endRunAlreadySentFlagAttribute = True
                                        '        End If

                                        '    ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                        '        ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                        '        If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                        '            myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                        '            myParamsQueue.Add("")
                                        '            endRunAlreadySentFlagAttribute = True
                                        '            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                        '        End If
                                        '    End If
                                        'End If

                                    End If

                                    'AG 27/10/2011
                                End If

                            Else 'Alarm solved
                                If alarmItem = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR Then
                                    pauseSendingTestPreparationsFlag = False
                                End If
                            End If

                            'PRESENTATION:
                            'Monitor (Main Tab) (RH): Inform the User using pRefreshDS in Ax00MainMDI.OnManageReceptionEvent


                        Case GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                            '[2011-05-12 - !!! Ax00 - Sw Espec Meetings.xls]

                            'INSTRUCTIONS: 
                            'STANDBY
                            'If alarm is new: Automatically perform new Alight
                            'If already exists (2on tentative also fails): If running -> Go to StandBy
                            If pAlarmStatusList(index) Then 'Alarm Exists
                                'baselineInitializationFailuresAttribute += 1 'AG 27/11/2014 BA-2144
                                If AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso baselineInitializationFailuresAttribute < ALIGHT_INIT_FAILURES Then
                                    'AG 27/11/2014 BA-2144 comment code, business will be implemented in method SendAutomaticALIGHTRerun 
                                    ''When ALIGHT has been rejected ... increment the variable CurrentWellAttribute to perform the new in other well
                                    'Dim SwParams As New SwParametersDelegate
                                    'myGlobal = SwParams.GetParameterByAnalyzer(dbConnection, AnalyzerIDAttribute, GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString, False)
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



                                ElseIf AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                                    'If 3 consecutive well rejection initializations have been refused (baselineCalcs.exitRunningType = 2) -> STANDBY
                                    'RPM 06/09/2012 - StandBy NO!!!, leave running using END or ABORT (no prep started, so better use END)
                                    'If 30 consecutive wells have been rejected once the initialization is OK (baselineCalcs.exitRunningType = 1) -> END

                                    Select Case wellBaseLineAutoPausesSession
                                        Case 0 'Nothing

                                        Case 1, 2 'END 'AG 06/09/2012 - also add case 2
                                            'TR 22/10/2013 -Bug #1353 Validate if not pause mode to send the end instruction.
                                            'AG 26/03/2014 - #1501 (Physics #48) - END cannot be sent when pause mode is starting (pauseModeIsStarting)
                                            If Not AllowScanInRunningAttribute AndAlso Not pauseModeIsStarting AndAlso Not endRunAlreadySentFlagAttribute AndAlso AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then
                                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                                        methodHasToAddInstructionToQueueFlag = 1
                                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                                    End If

                                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.ENDRUN) Then
                                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN)
                                                        myParamsQueue.Add("")
                                                        'endRunAlreadySentFlagAttribute = True 'AG 30/11/2012 - Not inform this flag here. It will be informed once really sent
                                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                                    End If
                                                End If
                                            End If

                                            'AG 06/09/2012 - commented, RPM says not to send STANDBY, use END better
                                            'Case 2 'STANDBY
                                            '    If methodHasToAddInstructionToQueueFlag = 0 Then
                                            '        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                                            '        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                            '            endRunAlreadySentFlagAttribute = True 'when StandBy instruction is sent, send the ENDRUN instruction has no sense!!
                                            '            methodHasToAddInstructionToQueueFlag = 1
                                            '            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True)
                                            '        End If

                                            '    ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                            '        If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.STANDBY) Then
                                            '            myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY)
                                            '            myParamsQueue.Add("")
                                            '            endRunAlreadySentFlagAttribute = True 'when StandBy instruction is sent, send the ENDRUN instruction has no sense!!
                                            '            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM, 1, True) 'AG 19/06/2012
                                            '        End If
                                            '    End If
                                    End Select
                                    'AG 05/06/2012

                                End If

                            Else
                                ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                            End If

                            'BUSINESS: Nothing

                            'PRESENTATION: 
                            'If alarm is new: None
                            'If already exists (2on tentative also fails): Show message "It is highly recommended change the reactions rotor"
                            '(Ax00MainMDI.OnManageReceptionEvent & ShowAlarmWarningMessages)
                            'MDI: Button activation or deactivation list definition (complete the final list in method Ax00MainMDI.ActivateActionButtonBar)


                        Case GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                            '[2011-05-04 - Ax00 - Sw Espec Meetings]

                            If pAlarmStatusList(index) Then 'Alarm Exists
                                'INSTRUCTIONS: 
                                '(NO, IMPLEMENT THIS BUSINESS, REJECTED!!!!) If no executions IN process -> Send ENDRUN, else do nothing (wait)

                                'BUSINESS: Activate flag
                                WELLbaselineParametersFailuresAttribute = True

                                'PRESENTATION: Show message "Change methacrylate rotor is high recommend" when user performs Wup, Start or finish work session 
                                '(Ax00MainMDI.OnManageReceptionEvent & ShowAlarmWarningMessages)

                            Else
                                WELLbaselineParametersFailuresAttribute = False
                            End If

                            'Recover instruction error
                        Case GlobalEnumerates.Alarms.RECOVER_ERR

                            If pAlarmStatusList(index) Then 'Alarm Exists
                                'The recover instruction has finished with error
                                If recoverAlreadySentFlagAttribute Then recoverAlreadySentFlagAttribute = False
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished (but with errors)
                            End If

                            'Arm collision error (error codes)
                        Case GlobalEnumerates.Alarms.R1_COLLISION_ERR, GlobalEnumerates.Alarms.R2_COLLISION_ERR, _
                            GlobalEnumerates.Alarms.S_COLLISION_ERR

                            If pAlarmStatusList(index) Then 'Alarm Exists
                                'Mark all INPROCESS executions as PENDING or LOCKED
                                'AG 08/03/2012 - This method is called only when Software receives ANSBxx with collision information
                                'Dim exec_dlg As New ExecutionsDelegate
                                'myGlobal = exec_dlg.ChangeINPROCESSStatusByCollision(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
                            End If

                            'Instructions aborted or rejected
                        Case GlobalEnumerates.Alarms.INST_ABORTED_ERR, GlobalEnumerates.Alarms.INST_REJECTED_ERR
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
                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.INFO) Then
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

                        Case GlobalEnumerates.Alarms.INST_REJECTED_WARN

                            'TR BUG #1339  before sending the next instruction, validate if the last intruction send was pause to star logic
                            If AppLayer.LastInstructionTypeSent = AppLayerEventList.PAUSE Then
                                PauseAlreadySentFlagAttribute = False
                                SetAllowScanInRunningValue(False) 'AG 08/11/2013 #1358  AllowScanInRunningAttribute = False
                                If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "INPROCESS", False) = 0 Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                                End If
                            End If
                            'TR BUG #1339 -END

                            'Sw has received a Request but has sent instruction out of time.
                            'Search for next instruction to be sent but using next well (only in Running)
                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                                Dim reactRotorDlg As New ReactionsRotorDelegate
                                futureRequestNextWell = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + 1, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)
                                myGlobal = Me.SearchNextPreparation(dbConnection, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
                                    myNextPreparationToSendDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)
                                    '' PROVA XB !!!
                                    'If Not myNextPreparationToSendDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso myNextPreparationToSendDS.nextPreparation(0).CuvetteContaminationFlag Then
                                    '    Debug.Print("Quieto parao")
                                    'End If
                                    'If Not myNextPreparationToSendDS.nextPreparation(0).IsReagentContaminationFlagNull AndAlso myNextPreparationToSendDS.nextPreparation(0).ReagentContaminationFlag Then
                                    '    Debug.Print("Quieto parao")
                                    'End If
                                    '' PROVA XB !!!
                                End If
                            End If


                            'Debug print to leave developent traces
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("Instruction rejected (out of time)", "AnalyzerManager.ManageAlarms", EventLogEntryType.Information, False)
                            Debug.Print(Now.ToString & " .Instruction rejected (out of time)")

                            ''''''''''''
                            'ISE ALARMS
                            ''''''''''''
                            'AG 23/03/2012 - ISE switch off
                        Case GlobalEnumerates.Alarms.ISE_OFF_ERR
                            If pAlarmStatusList(index) Then
                                ISEAnalyzer.IsISESwitchON = False 'SGM 29/06/2012
                                MyClass.RefreshISEAlarms() 'SGM 19/06/2012

                                'When this alarm appears in RUNNING: Lock all pending ISE preparations
                                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                                    Dim exec_delg As New ExecutionsDelegate

                                    '1st: Get all ISE executions pending (required to inform the UI with the locked executions
                                    myGlobal = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "PENDING", False, "PREP_ISE")
                                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                        Dim myExecutionsDS As New ExecutionsDS

                                        '2on: Lock all pending ISE preparation executions
                                        myGlobal = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")

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
                                myGlobal = alarmsDelg.GetByAlarmID(dbConnection, GlobalEnumerates.Alarms.ISE_OFF_ERR.ToString, Nothing, Nothing, AnalyzerIDAttribute, "")
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


                            'SGM 19/06/2012
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
                                If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
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
                            If AppLayer.LastInstructionTypeSent = AppLayerEventList.PAUSE Then
                                PauseAlreadySentFlagAttribute = False
                                SetAllowScanInRunningValue(False) 'AG 08/11/2013   AllowScanInRunningAttribute = False
                                If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "INPROCESS", False) = 0 Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "")
                                End If
                            End If
                            'TR BUG #1339 -END
                        Case Else
                            'Nothing
                    End Select
                    index = index + 1
                Next

                'AG 12/03/2012 - Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
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

                For Each alarmIDItem As GlobalEnumerates.Alarms In pAlarmIDList
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
                                .AnalyzerID = AnalyzerIDAttribute

                                .AlarmDateTime = Now

                                If WorkSessionIDAttribute <> "" Then
                                    .WorkSessionID = WorkSessionIDAttribute
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
                                .SetAdditionalInfoNull()
                                If Not pAdditionalInfoList Is Nothing AndAlso Not String.Equals(pAdditionalInfoList(index), String.Empty) Then
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
                    If ((pAlarmIDList.Count = 1 AndAlso Not pAlarmIDList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse pAlarmIDList.Count > 1) Then
                        If String.Equals(analyzerFREEZEModeAttribute, "AUTO") Then
                            'BUSINESS: 
                            'Special code for FREEZE (AUTO) - once solved the cover alarms error check if there are any other freeze alarm o not
                            'If not remove the attributes flags (freeze and freeze mode and also update the sensor FREERZE to 0)

                            'NOTE: for call this method here the parameter can not be pAlarmIDList. Instead use the current alarms in analyzer attribute (myAlarmListAttribute)

                            If Not ExistFreezeAlarms(dbConnection, myAlarmListAttribute) Then 'Freeze auto solved
                                'Analyzer has recovered an operating status automatically by closing covers
                                analyzerFREEZEFlagAttribute = False
                                analyzerFREEZEModeAttribute = ""
                                AnalyzerIsReadyAttribute = True 'Analyzer is ready
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 0, True) 'Inform presentation the analyzer is ready

                                'AG 14/05/2012 - if no instruction sent call ManagerAnalyzer. Else add instruction to queue 
                                'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status
                                If methodHasToAddInstructionToQueueFlag = 0 Then
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status
                                    methodHasToAddInstructionToQueueFlag = 1
                                ElseIf methodHasToAddInstructionToQueueFlag = 1 Then
                                    ' XBC 21/05/2012 - to avoid send the same instruction more than 1 time
                                    If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.STATE) Then
                                        myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.STATE)
                                        myParamsQueue.Add("")
                                    End If
                                End If
                                'AG 14/05/2012

                            Else 'Exists freeze auto
                                'Reset all processes flags
                                Dim flags_dlg As New AnalyzerManagerFlagsDelegate
                                myGlobal = flags_dlg.ResetFlags(dbConnection, AnalyzerIDAttribute)

                                'Finally read and update our internal structures
                                If Not myGlobal.HasError Then
                                    InitializeAnalyzerFlags(dbConnection)
                                End If

                                'if standby ... activate ansinf (some processes as shutdown, nrotor, ise utilities deactivate it, if freezes
                                'appears while sensors deactivated there is no other way to auto recover
                                If Not myGlobal.HasError Then
                                    If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
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
                                            If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.INFO) Then
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

                        ElseIf Not String.Equals(analyzerFREEZEModeAttribute, "PARTIAL") Then
                            'Remove all flags ... recover is required
                            Dim flags_dlg As New AnalyzerManagerFlagsDelegate
                            myGlobal = flags_dlg.ResetFlags(dbConnection, AnalyzerIDAttribute)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAlarms", EventLogEntryType.Error, False)

                'Finally
                '    If (pdbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobal
        End Function


        ''' <summary>
        ''' Activate the Analyzer Alarm Sound if prosuce alarm require sound.
        ''' The analyzer alarm sound must be activate on Analyzer configuration.
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pSomeInstructionAlreadySent" ></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 27/10/2011
        ''' AG 14/05/2012 - add byRef parameter psomeinstructionAlreadySent. When 0 send Sound instruction, else add to queue</remarks>
        Private Function SoundActivationByAlarm(ByVal pdbConnection As SqlClient.SqlConnection, ByRef pSomeInstructionAlreadySent As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pdbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'TR 26/10/2011 

                        'if alarm sound is configured an analyzer is not currently ringing then search inside 
                        'the new alarms if any requires sound.
                        If Not AnalyzerIsRingingAttribute Then
                            'Validate if Analyzer Alarm sound is enable.
                            myGlobalDataTO = IsAlarmSoundDisable(dbConnection)
                            If Not myGlobalDataTO.HasError Then
                                'Alarm sound is enable
                                If Not DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
                                    'Validate if Analyzer status is Freeze
                                    Dim sendSoundInstruction As Boolean = False

                                    Dim myAlarmList As New List(Of AlarmsDS.tfmwAlarmsRow)
                                    'Get the new active alarm, Linq over RecivedAlarms
                                    For Each myAlarmRow As UIRefreshDS.ReceivedAlarmsRow In myUI_RefreshDS.ReceivedAlarms.Where(Function(a) a.AlarmStatus)
                                        myAlarmList = (From a In alarmsDefintionTableDS.tfmwAlarms Where String.Equals(a.AlarmID, myAlarmRow.AlarmID) Select a).ToList()
                                        If myAlarmList.Count = 1 Then
                                            If AnalyzerStatus = AnalyzerManagerStatus.RUNNING AndAlso myAlarmList.First().OnRunningSound Then
                                                sendSoundInstruction = True
                                                Exit For
                                            ElseIf myAlarmList.First().Sound Then
                                                sendSoundInstruction = True
                                                Exit For
                                            End If
                                        End If
                                    Next

                                    If sendSoundInstruction Then
                                        'AG 14/05/2012
                                        'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                                        If pSomeInstructionAlreadySent = 0 Then
                                            ' XBC 21/05/2012 - to avoid send SOUND more than 1 time
                                            'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                                            'pSomeInstructionAlreadySent = 1
                                            'If myInstructionsQueue.Count > 0 and 
                                            If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.SOUND) Then
                                                myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                                                pSomeInstructionAlreadySent = 1
                                            End If
                                            ' XBC 21/05/2012
                                        Else
                                            myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND)
                                            myParamsQueue.Add("")
                                        End If
                                        'AG 14/05/2012
                                    End If
                                    myAlarmList = Nothing 'AG 02/08/2012 - free memory
                                End If
                            End If
                        End If
                        'TR 26/10/2011 -END.
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SoundActivation", EventLogEntryType.Error, False)
            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Indicate if the Alarm Sound is enable on the Analyzer configuration.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 27/10/2011</remarks>
        Private Function IsAlarmSoundDisable(ByVal pdbConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pdbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Search on tcfgAnalyzerSettings where setting id = ALARM_DISABLED
                        Dim myAnalyzerSettingDelegate As New AnalyzerSettingsDelegate
                        Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                        myGlobalDataTO = myAnalyzerSettingDelegate.GetAnalyzerSetting(pdbConnection, Me.AnalyzerIDAttribute, AnalyzerSettingsEnum.ALARM_DISABLED.ToString())

                        If Not myGlobalDataTO.HasError Then
                            myAnalyzerSettingsDS = DirectCast(myGlobalDataTO.SetDatos, AnalyzerSettingsDS)
                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                myGlobalDataTO.SetDatos = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IsAlarmSoundEnable", EventLogEntryType.Error, False)

            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAlarmIDList">List of alarms to treat</param>
        ''' <param name="pAlarmStatusList" >For each alarm in pAlarmCode indicates his status: TRUE alarm exists, FALSE alarm solved</param>
        ''' <param name="pErrorCodeList">List of Error Firmware Codes to treat</param>
        ''' <param name="pAnswerErrorReception" >Comming from ANSERR instruction received</param>
        ''' <returns>Global Data To indicating when an error has occurred </returns>
        ''' <remarks>
        ''' Created by XBC 23/05/2011
        ''' Modified by XBC 16/10/2012 - Add pErrorCodeList functionality to manage Firmware Alarms into Service Software
        ''' </remarks>
        Private Function ManageAlarms_SRV(ByVal pdbConnection As SqlClient.SqlConnection, _
                                          ByVal pAlarmIDList As List(Of GlobalEnumerates.Alarms), _
                                          ByVal pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pErrorCodeList As List(Of String) = Nothing, _
                                          Optional ByVal pAnswerErrorReception As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try


                ' XBC 16/10/2012 - Is used this code ?
                'Dim index As Integer = 0
                'For Each alarmItem As GlobalEnumerates.Alarms In pAlarmIDList
                '    If alarmItem <> GlobalEnumerates.Alarms.NONE Then
                '        If Not myAlarmListAttribute.Contains(alarmItem) Then
                '            myAlarmListAttribute.Add(alarmItem)

                '            'Generate UI_Refresh event ALARMS_RECEIVED
                '            PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmItem.ToString, True)

                '            'SGM 19/06/2012
                '            Select Case alarmItem

                '                'SGM 18/09/2012
                '                Case GlobalEnumerates.Alarms.ISE_OFF_ERR
                '                    If pAlarmStatusList(index) Then
                '                        ISEAnalyzer.IsISESwitchON = False
                '                        MyClass.RefreshISEAlarms()
                '                    End If

                '                Case GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR
                '                    If pAlarmStatusList(index) Then
                '                        MyClass.RefreshISEAlarms()
                '                    End If
                '            End Select

                '        End If
                '    End If

                '    index = index + 1
                'Next
                ' XBC 16/10/2012 

                Dim myLogAcciones2 As New ApplicationLogManager()
                Dim myManageAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.OMMIT_ERROR

                If Not pErrorCodeList Is Nothing Then
                    If pErrorCodeList.Count > 0 Then

                        ' XBC 07/11/2012
                        SolveErrorCodesToDisplay(pErrorCodeList)

                        Dim myAlarmsDelegate As New AlarmsDelegate
                        ' Get Management priotity of each error
                        Dim myManageAlarmTypeTemp As GlobalEnumerates.ManagementAlarmTypes
                        Dim AddErrCode As Boolean = False
                        Dim InformAlarm As Boolean = False
                        Dim AlarmID As String = ""

                        For i As Integer = 0 To pErrorCodeList.Count - 1

                            'Generate UI_Refresh event ALARMS_RECEIVED
                            If i > pAlarmIDList.Count - 1 Then
                                ' Error - Firmare has sent and identifier that not exist into masterdata database
                                myLogAcciones2.CreateLogActivity("Firmware Alarm Code received not specified on Masterdata [" & pErrorCodeList(i) & "]", "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Error, False)
                                Exit Try
                            End If

                            AlarmID = pAlarmIDList(i).ToString

                            myGlobal = myAlarmsDelegate.ReadManagementAlarm(Nothing, pErrorCodeList(i))
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                Dim myErrorCodesDS As AlarmErrorCodesDS
                                myErrorCodesDS = CType(myGlobal.SetDatos, AlarmErrorCodesDS)
                                If myErrorCodesDS.tfmwAlarmErrorCodes.Count > 0 Then

                                    ' XBC 07/11/2012
                                    AddErrorCodesToDisplay(myErrorCodesDS)

                                    Select Case myErrorCodesDS.tfmwAlarmErrorCodes(0).ManagementID
                                        Case "1_UPDATE_FW"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.UPDATE_FW
                                            AddErrCode = True
                                            InformAlarm = True

                                        Case "2_FATAL_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.FATAL_ERROR
                                            AddErrCode = True
                                            InformAlarm = True

                                        Case "3_RECOVER_ERROR"
                                            MyClass.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'SGM 19/11/2012
                                            If (AlarmID = GlobalEnumerates.Alarms.INST_ABORTED_ERR.ToString Or AlarmID = GlobalEnumerates.Alarms.RECOVER_ERR.ToString) _
                                               AndAlso Not pAnswerErrorReception Then
                                                myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO
                                                AddErrCode = False
                                                InformAlarm = False
                                                MyClass.IsInstructionAborted = (AlarmID = GlobalEnumerates.Alarms.INST_ABORTED_ERR.ToString)
                                                myLogAcciones2.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                                            Else
                                                myManageAlarmTypeTemp = ManagementAlarmTypes.RECOVER_ERROR
                                                AddErrCode = True
                                                InformAlarm = True
                                            End If

                                        Case "4_SIMPLE_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.SIMPLE_ERROR
                                            AddErrCode = True
                                            If AlarmID = GlobalEnumerates.Alarms.REACT_MISSING_ERR.ToString Then 'SGM 07/11/2012 - reactions rotor
                                                If Not MyClass.IsServiceRotorMissingInformed Then
                                                    InformAlarm = True
                                                End If
                                            Else
                                                InformAlarm = InformAlarm Or (MyClass.IsInstructionRejected Or MyClass.IsRecoverFailed) 'SGM 29/10/2012 - manage only in case of origin of Error 20 - INST_REJECTED_WARN
                                            End If


                                        Case "5_REQUEST_INFO"
                                            'SGM 29/10/2012 - reset flag that indicates E:20 received (Instruction Rejected)
                                            MyClass.IsInstructionRejected = (AlarmID = GlobalEnumerates.Alarms.INST_REJECTED_ERR.ToString)

                                            'SGM 07/11/2012 - reset flag that indicates E:22 received (Recover failed)
                                            MyClass.IsRecoverFailed = (AlarmID = GlobalEnumerates.Alarms.RECOVER_ERR.ToString)

                                            myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO
                                            AddErrCode = False
                                            InformAlarm = False
                                            myLogAcciones2.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)

                                        Case "6_OMMIT_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.OMMIT_ERROR
                                            AddErrCode = False
                                            'InformAlarm = False
                                            myLogAcciones2.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)

                                        Case Else
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.NONE
                                            AddErrCode = False
                                            'InformAlarm = False

                                    End Select

                                    If AddErrCode Then
                                        ' Add New Error Codes received
                                        If Not myErrorCodesAttribute.Contains(pErrorCodeList(i)) Then
                                            myErrorCodesAttribute.Add(pErrorCodeList(i))
                                        End If
                                    End If

                                    ' Request Info if is need
                                    If myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO Then
                                        ' INSTRUCTIONS: ask for detailed errors
                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR)
                                    End If

                                    ' Choose maximum priority
                                    If myManageAlarmTypeTemp <> 0 Then
                                        If myManageAlarmType > myManageAlarmTypeTemp Then
                                            myManageAlarmType = myManageAlarmTypeTemp
                                        End If
                                    End If

                                End If
                            End If
                        Next

                        If Not myGlobal.HasError And InformAlarm Then
                            ' Update Alarm sensors to inform to Presentation layer what kind of management is need to display
                            PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, AlarmID, True)
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, myManageAlarmType, True)
                            IsServiceAlarmInformedAttr = True
                        End If

                    End If

                Else
                    ' Check if there exists any Deposits Alarm - by default these alarms belongs to 4_SIMPLE_ERROR alarm management type
                    If Not IsServiceAlarmInformedAttr Then
                        For Each alarmItem As GlobalEnumerates.Alarms In pAlarmIDList
                            Select Case alarmItem
                                Case GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR, GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR

                                    ' XBC 07/11/2012
                                    Dim myErrorCodesDS As New AlarmErrorCodesDS
                                    Dim myErrorCodesRow As AlarmErrorCodesDS.tfmwAlarmErrorCodesRow
                                    myErrorCodesRow = myErrorCodesDS.tfmwAlarmErrorCodes.NewtfmwAlarmErrorCodesRow
                                    myErrorCodesRow.AlarmID = alarmItem.ToString
                                    myErrorCodesRow.ErrorCode = -1
                                    myErrorCodesRow.ManagementID = ManagementAlarmTypes.SIMPLE_ERROR.ToString
                                    If alarmItem = GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR Then
                                        myErrorCodesRow.ResourceID = "ALM_WATER_DEPOSIT_ERR"
                                    Else
                                        myErrorCodesRow.ResourceID = "ALM_WASTE_DEPOSIT_ERR"
                                    End If
                                    myErrorCodesDS.tfmwAlarmErrorCodes.Rows.Add(myErrorCodesRow)
                                    AddErrorCodesToDisplay(myErrorCodesDS)
                                    ' XBC 07/11/2012

                                    ' Update Alarm sensors to inform to Presentation layer what kind of management is need to display
                                    PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmItem.ToString, True)
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, ManagementAlarmTypes.SIMPLE_ERROR, True)
                                    myLogAcciones2.CreateLogActivity("Deposits Alarm received [" & alarmItem.ToString & "] - Priority Management : " & ManagementAlarmTypes.SIMPLE_ERROR.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                                    IsServiceAlarmInformedAttr = True
                            End Select
                        Next
                    End If
                End If

                If myErrorCodesAttribute.Count > 0 Then
                    myLogAcciones2.CreateLogActivity("Alarm error codes received [" & ErrorCodes & "] - Priority Management : " & myManageAlarmType.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Error, False)

            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobal
        End Function

#End Region

#Region "Individual alarm treatment Methods"

        ''' <summary>
        ''' Prepare the alarm list and alarm status list
        ''' Also apply the exception rules
        ''' 
        ''' NEW SPEC Eva 10/02/2012 - During start instrument only Sw only generates the alarms that could affect (pause) the process 
        '''                         - WASH_CONTAINER (ERROR or WARN) (wash solution bottle)
        '''                         - HIGH_CONTAMIN (ERROR or WARN) (high contamination bottle)
        '''                         - WATER_DEPOSIT (ERROR or SYSTEM_ERROR) (water deposit)
        '''                         - WASTE_DEPOSIT (ERROR or SYSTEM_ERROR) (waste deposit)
        ''' </summary>
        ''' <param name="pAlarmCode">Alarm identifier</param>
        ''' <param name="pAlarmStatus">TRUE: Alarm exists (active), FALSE: Alarm no exists</param>
        ''' <param name="pAlarmList">Final Alarm list to be processed</param>
        ''' <param name="pAlarmStatusList">Final Status for alarm list to be processed</param>
        ''' <param name="pAddInfo">Additional info used for volume missing or clot warning generated by ANSBxx</param>
        ''' <param name="pAdditionalInfoList">Additonal info list used for volume missing or clot warning generated by ANSBxx</param>
        ''' <param name="pAddAlwaysFlag" >TRUE: Add alarm always, do not take care if is Wup or Shown or ... are in process (used for code error alarms)</param>
        ''' <remarks>
        ''' Created by:  AG 18/03/2011
        ''' Modified by: AG 13/04/2012 - Added optional parameter pAddAlwaysFlag
        '''              AG 25/07/2012 - Added optional parameters pAddInfo and pAdditionalInfoList to be used for the volume missing and clot alarms
        ''' </remarks>
        Private Sub PrepareLocalAlarmList(ByVal pAlarmCode As GlobalEnumerates.Alarms, ByVal pAlarmStatus As Boolean, _
                                          ByRef pAlarmList As List(Of GlobalEnumerates.Alarms), ByRef pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pAddInfo As String = "", _
                                          Optional ByRef pAdditionalInfoList As List(Of String) = Nothing, _
                                          Optional ByVal pAddAlwaysFlag As Boolean = False)
            'NOTE: Do not use Try/Catch: the caller method implements it

            If (pAlarmStatus) Then
                'ALARM EXISTS
                '''''''''''''
                Dim addFlag As Boolean = True 'By default add all new alarms

                'Exception Nr.1: if exists Reactions rotor thermo error/nok do not add reaction rotor thermo warning!!
                If pAlarmCode = GlobalEnumerates.Alarms.REACT_TEMP_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_ERR) Then addFlag = False

                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. End

                ElseIf pAlarmCode = GlobalEnumerates.Alarms.REACT_TEMP_ERR Then
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. End

                    'Exception Nr.2: if exists Fridge thermo error/nok do not add fridge thermo warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR) Then addFlag = False
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. End

                ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR Then
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. End

                    'Exception Nr.3: if exists High contamination deposit error do not add high contamination deposit warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) Then addFlag = False

                    'Exception Nr.4: if exists Wash solution deposit error do not add Wash solution deposit warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.WASH_CONTAINER_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) Then addFlag = False

                    'Exception Nr.5: if exists R1 thermo system error do not add R1 thermo warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.R1_TEMP_WARN Then
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R1_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R1_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. end

                    'Exception Nr.6: if exists R2 thermo system error do not add R2 thermo warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.R2_TEMP_WARN Then
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R2_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R2_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. end
                    'Exception Nr.7: if exists Washing station system error do not add washing station thermo warning!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.WS_TEMP_WARN Then
                    'DL 31/07/2012. Begin
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WS_TEMP_SYS1_ERR) Then addFlag = False
                    'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WS_TEMP_SYS2_ERR) Then addFlag = False
                    'DL 31/07/2012. End

                    'Exception Nr.8: if exists Water deposit system error do not add water deposit error (calculated by Sw)!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) Then addFlag = False

                    'Exception Nr.8: if exists Waste deposit system error do not add waste deposit error (calculated by Sw)!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) Then addFlag = False

                    'Exception Nr.9: if exists ISE system error do not add ise status off !!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.ISE_OFF_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then addFlag = False

                    'Exception Nr.10: if exists main cover error do not add main cover warn!! 'AG 15/03/2012
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.MAIN_COVER_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.MAIN_COVER_ERR) Then addFlag = False

                    'Exception Nr.11: if exists fridge cover error do not add fridge cover warn!! 'AG 15/03/2012
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_COVER_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR) Then addFlag = False

                    'Exception Nr.12: if exists samples cover error do not add samples cover warn!! 'AG 15/03/2012
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.S_COVER_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.S_COVER_ERR) Then addFlag = False

                    'Exception Nr.13: if exists reactions cover error do not add reactions cover warn!! 'AG 15/03/2012
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.REACT_COVER_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_COVER_ERR) Then addFlag = False

                    'Exception Nr.14: if exists base line error do not add BASE LINE WELL warn (baseline warn)!! 'AG 26/04/2012
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.BASELINE_WELL_WARN Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then addFlag = False

                    'AG 07/09/2012 - In running if the Base line init error appears remove the METHACRYL_ROTOR_WARN alarm if exists
                ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso pAlarmCode = GlobalEnumerates.Alarms.BASELINE_INIT_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then
                        myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                    End If
                End If

                'AG 10/02/2012 - While start instrument is inprocess only generate the alarms that affect the process
                Dim warmUpInProcess As Boolean = False
                If (String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS") OrElse _
                    String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "PAUSED")) Then
                    warmUpInProcess = True
                End If

                'AG 13/04/2012 - when warm up is in process check if the alarm must be generated or not
                If warmUpInProcess AndAlso Not pAddAlwaysFlag Then
                    'If warmUpInProcess Then
                    'AG 13/04/2012

                    addFlag = False 'By default no alarms generation

                    'Only those alarms affecting the start instrument process 
                    'AG 20/02/2012 - we can NOT use the method 'ExistBottleAlarms' because the alarm still has not been created
                    'AG 24/02/2012 - the baseline error alarm can also appear in wup process
                    'XBC 27/03/2012 - add ISE Management Alarms
                    'SGM 27/07/2012 - add ISE independent errors alarma (ERC)
                    If Not IgnoreAlarmWhileWarmUp(pAlarmCode) Then
                        addFlag = True
                    End If
                End If
                'AG 10/02/2012

                'AG 18/05/2012 - While shutdown instrument is inprocess only generate the alarms that affect the process
                Dim sDownInProcess As Boolean = False
                ' AG+XBC 24/05/2012 - no for status PAUSED for SDown Process
                'If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "INPROCESS" OrElse _
                '    mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "PAUSED") Then
                If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess.ToString) = "INPROCESS") Then
                    sDownInProcess = True
                End If

                If sDownInProcess AndAlso Not pAddAlwaysFlag Then
                    addFlag = False 'By default no alarms generation
                    If Not IgnoreAlarmWhileShutDown(pAlarmCode) Then
                        addFlag = True
                    End If
                End If
                'AG 18/05/2012

                'AG 28/02/2012 - rare case: sw shows message no comms but communications exists and sw continues receiving instructions (for example ANSINF)
                'When no comms Sw clears all alarms 
                'When Sw receives instructions (for instance ANSINF) the alarms will we generated and insert into database again and again every 2 seconds
                If addFlag AndAlso myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.COMMS_ERR) Then
                    'Software business alarms

                    ' XBC 23/03/2012
                    'If pAlarmCode <> GlobalEnumerates.Alarms.BASELINE_INIT_ERR AndAlso pAlarmCode <> GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN _
                    'AndAlso pAlarmCode <> GlobalEnumerates.Alarms.REPORTSATLOADED_WARN Then
                    If Not IgnoreAlarmWhileCommError(pAlarmCode) Then
                        addFlag = True
                    End If

                End If
                'AG 28/02/2012

                'Get the alarms defined with OKType = False (never are marked as solved)
                Dim alarmsWithOKTypeFalse As List(Of String) = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                                               Where a.OKType = False Select a.AlarmID).ToList

                If (addFlag) Then
                    'New alarm exists ... add to list with TRUE status only if alarm doesnt exists
                    If Not myAlarmListAttribute.Contains(pAlarmCode) Then
                        pAlarmList.Add(pAlarmCode)
                        pAlarmStatusList.Add(True)
                        If Not pAdditionalInfoList Is Nothing Then 'AG 25/07/2012 - used only for the volume missing, clot warnings, prep locked alarms
                            pAdditionalInfoList.Add(pAddInfo)
                        End If

                        'AG 24/07/2012 - some alarms are never markt as solved. They can be duplicated in pAlarmList
                    ElseIf alarmsWithOKTypeFalse.Contains(pAlarmCode.ToString) Then
                        'AG 29/08/2012 - exception BASELINE_WELL_WARN (change reactions rotor recommend). It can appear several times in the same WS but the alarm is generated once
                        If pAlarmCode <> GlobalEnumerates.Alarms.BASELINE_WELL_WARN Then
                            pAlarmList.Add(pAlarmCode)
                            pAlarmStatusList.Add(True)
                            'AG 24/07/2012
                            If Not pAdditionalInfoList Is Nothing Then 'AG 25/07/2012 - used only for the volume missing, clot warnings, prep locked alarms
                                pAdditionalInfoList.Add(pAddInfo)
                            End If
                        End If

                        'AG 07/09/2012 - base line init can appear several times in Running, for these alarm repetitions a flag defines when stop WS
                    ElseIf pAlarmCode = GlobalEnumerates.Alarms.BASELINE_INIT_ERR Then
                        If wellBaseLineAutoPausesSession <> 0 Then
                            pAlarmList.Add(pAlarmCode)
                            pAlarmStatusList.Add(True)
                            If Not pAdditionalInfoList Is Nothing Then
                                pAdditionalInfoList.Add(pAddInfo)
                            End If
                        End If
                        'AG 07/09/2012

                    End If
                End If

                'AG 15/03/2012 - Special code for AUTOrecove FREEZE alarms - if error code appears add it, but if the warning level already exists then remove it
                If pAlarmCode = GlobalEnumerates.Alarms.MAIN_COVER_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN) Then
                        pAlarmList.Add(GlobalEnumerates.Alarms.MAIN_COVER_WARN) 'If cover error is added remove the warning level
                        pAlarmStatusList.Add(False)
                    End If

                ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_COVER_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN) Then
                        pAlarmList.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN) 'If cover error is added remove the warning level
                        pAlarmStatusList.Add(False)
                    End If

                ElseIf pAlarmCode = GlobalEnumerates.Alarms.REACT_COVER_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN) Then
                        pAlarmList.Add(GlobalEnumerates.Alarms.REACT_COVER_WARN) 'If cover error is added remove the warning level
                        pAlarmStatusList.Add(False)
                    End If

                ElseIf pAlarmCode = GlobalEnumerates.Alarms.S_COVER_ERR Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.S_COVER_WARN) Then
                        pAlarmList.Add(GlobalEnumerates.Alarms.S_COVER_WARN) 'If cover error is added remove the warning level
                        pAlarmStatusList.Add(False)
                    End If
                End If
                'AG 15/03/2012

                alarmsWithOKTypeFalse = Nothing

            Else
                'ALARM SOLVED
                '''''''''''''

                'Alarm solved ... add to list with FALSE status only if alarm exists
                If myAlarmListAttribute.Contains(pAlarmCode) Then
                    pAlarmList.Add(pAlarmCode)
                    pAlarmStatusList.Add(False)
                End If


                Dim OtherAlarmsSolved As Boolean = False
                Dim solvedErrAlarmID As New List(Of GlobalEnumerates.Alarms)

                'AG 05/01/2011
                'Special code Nr.1: if Reactions rotor thermo warning is solved also mark the error alarm as solved too!!
                If pAlarmCode = GlobalEnumerates.Alarms.REACT_TEMP_WARN Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.REACT_TEMP_ERR)
                    OtherAlarmsSolved = True

                    'Special code Nr.2: if Firdge thermo warning is solved also mark the error/nok alarm as solved too!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR)
                    OtherAlarmsSolved = True

                    'AG 13/03/2012
                    'Special code Nr.6: if R1 collision err (freeze) is solved also mark the r1 collision warn as solved too!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.R1_COLLISION_ERR Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.R1_COLLISION_WARN)
                    OtherAlarmsSolved = True

                    'Special code Nr.7: if R2 collision err (freeze) is solved also mark the r2 collision warn as solved too!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.R2_COLLISION_ERR Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.R2_COLLISION_WARN)
                    OtherAlarmsSolved = True

                    'Special code Nr.8: if samples collision err (freeze) is solved also mark the samples collision warn as solved too!!
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.S_COLLISION_ERR Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.S_COLLISION_WARN)
                    OtherAlarmsSolved = True
                    'AG 13/03/2012
                    'JV 08/01/2014 BT #1118
                ElseIf pAlarmCode = GlobalEnumerates.Alarms.ISE_OFF_ERR Then
                    solvedErrAlarmID.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                    OtherAlarmsSolved = True
                    'JV 08/01/2014 BT #1118
                End If

                'AG 28/03/2012 - Special code Nr.9: for auto recover freeze alarms (when cover warn solved also mark the cover error as solved too!!)
                If Not analyzerFREEZEFlagAttribute OrElse analyzerFREEZEModeAttribute = "AUTO" Then
                    If pAlarmCode = GlobalEnumerates.Alarms.MAIN_COVER_WARN Then
                        solvedErrAlarmID.Add(GlobalEnumerates.Alarms.MAIN_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = GlobalEnumerates.Alarms.FRIDGE_COVER_WARN Then
                        solvedErrAlarmID.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = GlobalEnumerates.Alarms.S_COVER_WARN Then
                        solvedErrAlarmID.Add(GlobalEnumerates.Alarms.S_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = GlobalEnumerates.Alarms.REACT_COVER_WARN Then
                        solvedErrAlarmID.Add(GlobalEnumerates.Alarms.REACT_COVER_ERR)
                        OtherAlarmsSolved = True
                    End If
                End If
                'AG 28/03/2012

                If OtherAlarmsSolved Then
                    For Each item As GlobalEnumerates.Alarms In solvedErrAlarmID
                        If myAlarmListAttribute.Contains(item) Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                            'JV 08/01/2014 BT #1118
                        ElseIf item = GlobalEnumerates.Alarms.ISE_OFF_ERR Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                            'JV 08/01/2014 BT #1118
                        End If
                    Next
                End If

            End If
        End Sub

        ''' <summary>
        ''' Prepare the Error Code list
        ''' </summary>
        ''' <param name="pErrorCodeList">Fw Error identifiers</param>
        ''' <param name="pErrorCodeFinalList">Final Fw Error Code list to be processed</param>
        ''' <remarks>
        ''' Created XBC 16/10/2012
        ''' </remarks>
        Private Sub PrepareLocalAlarmList_SRV(ByVal pErrorCodeList As List(Of Integer), _
                                              ByRef pErrorCodeFinalList As List(Of String))

            'NOTE: Do not use Try Catch do the caller method implements it

            ' Initialize Alarms list to empty content value 
            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, 0, True)

            For i As Integer = 0 To pErrorCodeList.Count - 1
                ' Add new received Fw error codes
                If Not pErrorCodeFinalList.Contains(pErrorCodeList(i).ToString) Then
                    pErrorCodeFinalList.Add(pErrorCodeList(i).ToString)
                End If
            Next

        End Sub



        ''' <summary>
        ''' Verify if there are Temperature Warning Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pThermoFridge">Fridge Temperature</param>
        ''' <param name="pThermoReactions">Reactions Rotor Temperature</param>
        ''' <param name="pThermoWS">Washing Station Temperature</param>
        ''' <param name="pThermoR1">R1 Arm Temperature</param>
        ''' <param name="pThermoR2">R2 Arm Temperature</param>
        ''' <param name="pAlarmList"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2011
        ''' Modified by: AG 30/03/2011 - Used myClassFieldLimitsDS class variable. Dont use delegate and read again
        '''              SA 25/05/2012 - Implementation changed due to a especification change by RP (17/05/2012):
        '''                                 ** Reactions Rotor Temp has to be between SetPointTemp-0.5 and SetPointTemp+0.5
        '''                                 ** Fridge Temp has to be between SetPointTemp-3 and SetPointTemp+7
        '''                                 ** Washing Station Temp has to be between SetPointTemp-5 and SetPointTemp+5
        '''                                 ** R1 Arm Temp/R2 Arm Temp have to be between SetPointTemp-15 and SetPointTemp+5
        '''                               Due to this, target temperature and offset calculations have been removed from the code
        '''              SA 29/05/2012 - Changes of 25/05/2012 have been commented and the previous function implementation has been
        '''                              recovered due to it is not clear how to implement those changes in Service Software 
        '''              SA 01/06/2012 - Changes of 25/05/2012 have been implemented again
        '''              XB 29/01/2013 - Change IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)
        '''              XB 30/01/2013 - Add Trace log information about malfunctions on temperature values (Bugs tracking #1122)
        ''' </remarks>
        Private Function CheckTemperaturesAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pThermoFridge As Single, _
                                                 ByVal pThermoReactions As Single, ByVal pThermoWS As Single, ByVal pThermoR1 As Single, _
                                                 ByVal pThermoR2 As Single, ByRef pAlarmList As List(Of GlobalEnumerates.Alarms), _
                                                 ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Dim myUtil As New Utilities

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim alarmStatus As Boolean = False
                        Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                        Dim limitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)

                        Dim adjustValue As String = String.Empty
                        Dim tempSetPoint As Single = 0 'Temperature "consigna"

                        'THERMO REACTIONS ROTOR
                        'Read parameter THERMO_REACTIONS_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.THERMO_REACTIONS_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmID = GlobalEnumerates.Alarms.REACT_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.GTGF)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = myUtil.FormatToSingle(adjustValue)
                            Else
                                Dim myLogAcciones2 As New ApplicationLogManager()
                                myLogAcciones2.CreateLogActivity("Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Information, False)
                            End If

                            'Verify the Reactions Rotor Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= ReactionRotorsTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm REACT_TEMP_WARN
                            If (pThermoReactions < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoReactions > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True

                                'Alarm will be generated only if Reactions Cover detection is enabled
                                adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.PHCOV) '0=Disabled / 1=Enabled
                                Dim reactionsCoverDetectionEnabled As Boolean = True
                                If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                    reactionsCoverDetectionEnabled = CType(adjustValue, Boolean)
                                    If (Not reactionsCoverDetectionEnabled) Then alarmStatus = False
                                End If
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'THERMO FRIDGE (Reagents Rotor)
                        'Read parameter THERMO_FRIDGE_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.THERMO_FRIDGE_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmID = GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.GTN1)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = myUtil.FormatToSingle(adjustValue)
                            Else
                                Dim myLogAcciones2 As New ApplicationLogManager()
                                myLogAcciones2.CreateLogActivity("Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Information, False)
                            End If

                            'Verify the Fridge Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= FridgeTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm FRIDGE_TEMP_WARN
                            If (pThermoFridge < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoFridge > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True

                                'Alarm will be generated only if Fridge Cover detection is enabled
                                adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.RCOV) '0=Disabled / 1=Enabled
                                Dim fridgeCoverDetectionEnabled As Boolean = True
                                If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                    fridgeCoverDetectionEnabled = CType(adjustValue, Boolean)
                                    If (Not fridgeCoverDetectionEnabled) Then alarmStatus = False
                                End If
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'THERMO WASHING STATION
                        'Read parameter THERMO_WASHSTATION_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.THERMO_WASHSTATION_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmID = GlobalEnumerates.Alarms.WS_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.GTH1)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = myUtil.FormatToSingle(adjustValue)
                            Else
                                Dim myLogAcciones2 As New ApplicationLogManager()
                                myLogAcciones2.CreateLogActivity("Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Information, False)
                            End If

                            'Verify the Washing Station Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= WashingStationTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm WS_TEMP_WARN
                            If (pThermoWS < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoWS > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'AG 05/01/2012 - Do not treat R1 or R2 temperature in RUNNING mode
                        If (AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING) Then
                            'THERMO R1 ARM / R2 ARM
                            'This alarms can be triggered by Sw if out of limits or by Fw error code Exxx instead R1 or R2 temperatures XX,X

                            'Read parameter THERMO_ARMS_LIMIT in tfmwFieldLimits to get Min/Max values for it
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                        Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.THERMO_ARMS_LIMIT.ToString _
                                       Select a).ToList

                            If (limitList.Count > 0) Then
                                'THERMO R1 ARM
                                alarmStatus = False

                                'Get the SetPoint Temperarure
                                adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.GTR1)
                                'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                If (adjustValue <> String.Empty AndAlso _
                                    Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                    tempSetPoint = myUtil.FormatToSingle(adjustValue)
                                Else
                                    Dim myLogAcciones2 As New ApplicationLogManager()
                                    myLogAcciones2.CreateLogActivity("Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Information, False)
                                End If

                                'Verify the R1 Arm Temperature is inside the allowed limits: 
                                '       SetPointTemp(+MinLimit <= R1ArmTemp <= SetPointTemp + MaxLimit)
                                'If it is outside the allowed limits, then activate the Warning Timer for R1 Arm
                                If (pThermoR1 < (tempSetPoint + limitList(0).MinValue)) Or _
                                   (pThermoR1 > (tempSetPoint + limitList(0).MaxValue)) Then
                                    alarmStatus = True
                                    If (Not thermoR1ArmWarningTimer.Enabled) Then 'AG 05/01/2012 - Activate waiting temporal time period
                                        thermoR1ArmWarningTimer.Enabled = True
                                    End If
                                Else
                                    alarmStatus = False
                                    If (thermoR1ArmWarningTimer.Enabled) Then 'AG 05/01/2012 Deactivate waiting temporal time period
                                        thermoR1ArmWarningTimer.Enabled = False
                                    End If
                                End If

                                If (Not alarmStatus) Then
                                    alarmID = GlobalEnumerates.Alarms.R1_TEMP_WARN
                                    PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)
                                End If

                                'THERMO R2 ARM
                                alarmStatus = False

                                'Get the SetPoint Temperarure
                                adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.GTR2)
                                'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                If (adjustValue <> String.Empty AndAlso _
                                    Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                    tempSetPoint = myUtil.FormatToSingle(adjustValue)
                                Else
                                    Dim myLogAcciones2 As New ApplicationLogManager()
                                    myLogAcciones2.CreateLogActivity("Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Information, False)
                                End If

                                'Verify the R2 Arm Temperature is inside the allowed limits: 
                                '       SetPointTemp(+MinLimit <= R2ArmTemp <= SetPointTemp + MaxLimit)
                                'If it is outside the allowed limits, then activate the Warning Timer for R2 Arm
                                If (pThermoR2 < (tempSetPoint + limitList(0).MinValue)) Or _
                                   (pThermoR2 > (tempSetPoint + limitList(0).MaxValue)) Then
                                    alarmStatus = True
                                    If (Not thermoR2ArmWarningTimer.Enabled) Then 'AG 05/01/2012 - Activate waiting temporal time period
                                        thermoR2ArmWarningTimer.Enabled = True
                                    End If
                                Else
                                    alarmStatus = False
                                    If (thermoR2ArmWarningTimer.Enabled) Then 'AG 05/01/2012 Deactivate waiting temporal time period
                                        thermoR2ArmWarningTimer.Enabled = False
                                    End If
                                End If

                                If (Not alarmStatus) Then
                                    alarmID = GlobalEnumerates.Alarms.R2_TEMP_WARN
                                    PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)
                                End If
                            End If
                        Else
                            'AG 05/01/2012 Deactivate waiting temporal time period
                            If (thermoR1ArmWarningTimer.Enabled) Then thermoR1ArmWarningTimer.Enabled = False
                            If (thermoR2ArmWarningTimer.Enabled) Then thermoR2ArmWarningTimer.Enabled = False
                        End If

                        'AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS, pThermoReactions, False)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE, pThermoFridge, False)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION, pThermoWS, False)

                        'AG 05/01/2012 - Do not treat R1 or R2 temperature in RUNNING mode
                        If (AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING) Then
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1, pThermoR1, False)
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2, pThermoR2, False)
                        End If

                        limitList = Nothing 'AG 02/08/2012 - free memory
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CheckTemperaturesAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Check if exists or not High contamination or Washing solution container alarms
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHighContaminationContainer"></param>
        ''' <param name="pWashSolutionContainer"></param>
        ''' <param name="pAlarmList" ></param>
        ''' <param name="pAlarmStatusList" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 24/03/2011 creation
        ''' AG 30/03/2011 - used myClassFieldLimitsDS class variable. Dont use delegate and read again
        ''' </remarks>
        Private Function CheckContainerAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHighContaminationContainer As Single, ByVal pWashSolutionContainer As Single, _
                                              ByRef pAlarmList As List(Of GlobalEnumerates.Alarms), ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim myPercentage As Single = 0
                            Dim newSensorNumericalValueFlag As Boolean = False 'AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized

                            Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                            Dim alarmStatus As Boolean = False
                            Dim limitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                            Dim myUtils As New Utilities

                            'High contamination container
                            'Read the parameter HIGH_CONTAMIN_DEPOSIT_LIMIT in tfmwFieldLimits
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                         Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.HIGH_CONTAMIN_DEPOSIT_LIMIT.ToString _
                                         Select a).ToList

                            If limitList.Count > 0 Then
                                myPercentage = 0 'By default no alarm 
                                'Calculate the percentage using pHighContaminationContainer) get value into variable myPercentage
                                resultData = GetBottlePercentage(CInt(pHighContaminationContainer), False)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myPercentage = CSng(CType(resultData.SetDatos, Double))
                                    myPercentage = myUtils.FormatToSingle(myUtils.ToStringWithFormat(myPercentage, 0)) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena) ('AG 04/10/2011 - format mypercentage with 1 decimal)

                                    'AG 22/02/2012 Only inform new numerical value when the alarm is not locked to prevent level oscillations
                                    ''AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True)
                                End If

                                'Vol > Critical limit (error)
                                alarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR
                                alarmStatus = False
                                If myPercentage >= limitList(0).MaxValue Then
                                    'Alarm Warning
                                    alarmStatus = True
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                Else
                                    alarmStatus = False

                                    'AG 21/02/2012 - To prevent the ERROR or WARNING alarm level oscillations when near the limit (electronics tolerances),
                                    'when the ERROR level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button
                                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) Then
                                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                            alarmStatus = True
                                        End If
                                    End If

                                    If Not alarmStatus Then
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                    End If
                                    'AG 21/02/2012

                                End If
                                PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)

                                'If not exist high contamination error ... check if warning
                                If Not alarmStatus Then
                                    'Vol > Warning limit & Vol < Critical limit (warning)
                                    alarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN
                                    alarmStatus = False
                                    If myPercentage >= limitList(0).MinValue And myPercentage < limitList(0).MaxValue Then
                                        'Alarm Warning
                                        alarmStatus = True
                                    Else
                                        alarmStatus = False

                                        'AG 21/02/2012 - To prevent the WARNING or OK alarm level oscillations when near the limit (electronics tolerances),
                                        'when the WARNING level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button or after generate the ERROR alarm level
                                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then
                                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                                alarmStatus = True
                                            End If
                                        End If
                                        'AG 21/02/2012
                                    End If

                                Else 'If exists error warning can not be active
                                    alarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN
                                    alarmStatus = False
                                End If
                                PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)

                            End If


                            'Wash solution container
                            'Read the parameter WASH_SOLUTION_DEPOSIT_LIMIT in tfmwFieldLimits
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                        Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.WASH_SOLUTION_DEPOSIT_LIMIT.ToString _
                                        Select a).ToList

                            If limitList.Count > 0 Then
                                myPercentage = 100 'By Default no alarm 
                                'Calculate the percentage using pWashSolutionContainer) get value into variable myPercentage
                                resultData = GetBottlePercentage(CInt(pWashSolutionContainer), True)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myPercentage = CSng(CType(resultData.SetDatos, Double))
                                    myPercentage = myUtils.FormatToSingle(myUtils.ToStringWithFormat(myPercentage, 0)) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena)  'AG 04/10/2011 - format mypercentage with 1 decimal

                                    'AG 22/02/2012 Only inform new numerical value when the alarm is not locked to prevent level oscillations
                                    ''AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True)
                                End If

                                'Vol < Critical limit (error)
                                alarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR
                                alarmStatus = False
                                If myPercentage <= limitList(0).MinValue Then
                                    'Alarm Warning
                                    alarmStatus = True
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized

                                Else
                                    alarmStatus = False

                                    'AG 21/02/2012 - To prevent the ERROR or WARNING alarm level oscillations when near the limit (electronics tolerances),
                                    'when the ERROR level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button
                                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) Then
                                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                            alarmStatus = True
                                        End If
                                    End If

                                    If Not alarmStatus Then
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                    End If
                                    'AG 21/02/2012

                                End If
                                PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)

                                'If not exist wash solution container error ... check if warning
                                If Not alarmStatus Then
                                    'Vol < Warning limit & Vol > Critical limit (warning)
                                    alarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_WARN
                                    alarmStatus = False
                                    If myPercentage > limitList(0).MinValue And myPercentage <= limitList(0).MaxValue Then
                                        'Alarm Warning
                                        alarmStatus = True
                                    Else
                                        alarmStatus = False

                                        'AG 21/02/2012 - To prevent the WARNING or OK alarm level oscillations when near the limit (electronics tolerances),
                                        'when the WARNING level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button or after generate the ERROR alarm level
                                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) Then
                                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                                alarmStatus = True
                                            End If
                                        End If
                                        'AG 21/02/2012

                                    End If

                                Else 'If exists error warning can not be active
                                    alarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_WARN
                                    alarmStatus = False
                                End If
                                PrepareLocalAlarmList(alarmID, alarmStatus, pAlarmList, pAlarmStatusList)

                            End If
                            limitList = Nothing 'AG 02/08/2012 - free memory

                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CheckContainerAlarms", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' SW has received and ANSINF instruction and now check for alarms
        ''' User Sw has his own threatment different form service
        ''' </summary>
        ''' <param name="pSensors "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 14/04/2011
        ''' Modified by: XB 27/05/2014 - BT #1630 ==> Fix bug Abort+Reset after Tanks Alarms solved (in stanby requires user clicks button Change Bottle confirm, not automatically fixed as in Running)
        '''              IT 03/12/2014 - BA-2075
        ''' </remarks>
        Private Function UserSwANSINFTreatment(ByVal pSensors As Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)
                Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim alarmStatus As Boolean = False
                Dim myLogAcciones As New ApplicationLogManager() 'AG 29/05/2014 - #1630 add traces informing when deposit sensors activate/deactivate timers

                'Get General cover (parameter index 3)
                Dim myIntValue As Integer = 0
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL))
                    alarmID = GlobalEnumerates.Alarms.MAIN_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL), False)
                End If


                'Get Photometrics (Reaccions) cover (parameter index 4)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS))
                    alarmID = GlobalEnumerates.Alarms.REACT_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS), False)
                End If


                'Get Reagents (Fridge) cover (parameter index 5)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE))
                    alarmID = GlobalEnumerates.Alarms.FRIDGE_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE), False)
                End If


                'Get Samples cover (parameter index 6)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES))
                    alarmID = GlobalEnumerates.Alarms.S_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES), False)
                End If


                'Get System liquid sensor (parameter index 7)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT) Then
                    'AG 01/12/2011 - Sw do not generates the alarm WATER_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    alarmID = GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT))

                    If myIntValue = 0 Or myIntValue = 2 Then 'Empty or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False

                        'AG 29/05/2014 - BT #1630 - Alarm fixed automatically in running but in standby requires user action (click ChangeBottleButton)
                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) AndAlso _
                           (AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS") Then
                            alarmStatus = True ' Keep alarm until user clicks confirmation
                        End If
                        'AG 29/05/2014 - BT #1630
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT), False)

                    'AG 29/05/2014 - #1630 - Do not activate timer is alarm is already active!!
                    'If alarmStatus AndAlso Not waterDepositTimer.Enabled Then
                    If alarmStatus AndAlso Not waterDepositTimer.Enabled AndAlso Not myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) Then
                        myLogAcciones.CreateLogActivity("Water deposit empty!! Enable timer waterDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        waterDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)

                    ElseIf Not alarmStatus AndAlso waterDepositTimer.Enabled Then
                        myLogAcciones.CreateLogActivity("Water deposit OK!! Disable timer waterDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        waterDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    'AG 01/12/2011

                    If Not alarmStatus Then 'AG 09/01/2012 Remove alarm (if exists)
                        PrepareLocalAlarmList(alarmID, False, myAlarmList, myAlarmStatusList)
                    End If

                End If


                'Get Waste sensor (parameter index 8)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT) Then
                    'AG 01/12/2011 - Sw do not generates the alarm WASTE_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    alarmID = GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR

                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT))

                    If myIntValue = 3 Or myIntValue = 2 Then 'Full or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False

                        'AG 29/05/2014 - BT #1630 - Alarm fixed automatically in running but in standby requires user action (click ChangeBottleButton)
                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) AndAlso _
                           (AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS") Then
                            alarmStatus = True ' Keep alarm until user clicks confirmation
                        End If
                        'AG 29/05/2014 - BT #1630

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT), False)

                    'AG 29/05/2014 - #1630 - Do not activate timer is alarm is already active!!
                    'If alarmStatus AndAlso Not wasteDepositTimer.Enabled Then
                    If alarmStatus AndAlso Not wasteDepositTimer.Enabled AndAlso Not myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) Then
                        myLogAcciones.CreateLogActivity("Waste deposit full!! Enable timer wasteDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        wasteDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)

                    ElseIf Not alarmStatus AndAlso wasteDepositTimer.Enabled Then
                        myLogAcciones.CreateLogActivity("Waste deposit OK!! Disable timer wasteDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        wasteDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    'AG 01/12/2011

                    If Not alarmStatus Then 'AG 09/01/2012 Remove alarm (if exists)
                        PrepareLocalAlarmList(alarmID, False, myAlarmList, myAlarmStatusList)
                    End If


                End If


                'Get Weight sensors (wash solution & high contamination waste) (parameter index 9, 10)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) And _
                    pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                    'Evaluate if exists or not alarm
                    CheckContainerAlarms(Nothing, pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE), _
                                         pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION), myAlarmList, myAlarmStatusList)

                End If


                'Get Fridge status (parameter index 12)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS))
                    alarmID = GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN

                    If myIntValue = 0 Then 'OFF
                        alarmStatus = True
                    Else 'ON
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS, pSensors(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS), False)
                End If


                'Get Temperature values (reactions, fridge, wash station heater, R1, R2) (parameter index 11, 13, 14, 15, 16)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE) And _
                    pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS) And _
                    pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION) And _
                    pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1) And _
                    pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2) Then

                    'Evaluate if exists or not alarm
                    CheckTemperaturesAlarms(Nothing, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE), _
                                            pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS), _
                                            pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION), _
                                            pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1), _
                                            pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2), myAlarmList, myAlarmStatusList)

                End If


                'Get ISE status (parameter index 17)
                '(this alarm is only generated when Instrument has ISE module installed - adjust ISEINS = 1)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.ISE_STATUS) Then
                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.ISE_STATUS))
                    alarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR

                    If myIntValue = 0 Then 'OFF
                        'AG 23/11/2011 - this alarm is only generated when Instrument has ISE module installed - adjust ISEINS = 1
                        'alarmStatus = True
                        alarmStatus = False
                        Dim ISEInstalledValue As String = ""
                        ISEInstalledValue = ReadAdjustValue(Ax00Adjustsments.ISEINS)
                        If Not String.Equals(ISEInstalledValue, String.Empty) AndAlso IsNumeric(ISEInstalledValue) Then
                            If CType(ISEInstalledValue, Boolean) Then
                                alarmStatus = True
                            End If
                        End If
                        'AG 23/11/2011

                    Else 'ON
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_STATUS, pSensors(GlobalEnumerates.AnalyzerSensors.ISE_STATUS), False)
                End If

                ''''''''
                'Finally call manage all alarms detected (new or solved)
                Dim instrAddedToLogFlag As Boolean = False 'Indicates if the instruction has been added to log file
                If myAlarmList.Count > 0 Then
                    'NOTE: Here do not place different code for service or user because this method is called only for the user Software
                    myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)

                    'AG 18/06/2012 - The ANSINF instruction is saved to log only when new alarms are generated or solved
                    '                Previous code only saved it when new alarms were generated
                    'Dim newNoProbeTemperatureAlarms As List(Of Boolean)
                    'newNoProbeTemperatureAlarms = (From a As Boolean In myAlarmStatusList Where a = True Select a).ToList
                    'If newNoProbeTemperatureAlarms.Count > 0 Then

                    If ((myAlarmList.Count = 1 AndAlso Not myAlarmList.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR)) OrElse myAlarmList.Count > 1) Then 'AG + XB 08/04/2014 - #1118 (do not save always ANSINF in log due to ISE_OFF_ERR)
                        'Dim myLogAcciones As New ApplicationLogManager() 'AG 29/05/2014 - #1630 declare at the beginning
                        myLogAcciones.CreateLogActivity("Received Instruction [" & AppLayer.InstructionReceived & "]", "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        instrAddedToLogFlag = True
                    End If
                    'End If
                    'AG 04/10/2011
                End If

                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                If Not analyzerFREEZEFlagAttribute Then 'AG 14/03/2012 -Evaluate if process can continue only if analyzer is not freeze

                    'AG 16/03/2012 - Check if process can continue only when (ise intalled and isemanager initiated) or ise not installed 
                    '((Ise Instaled and (initiated or not swtiched on)) or not ise instaled)
                    If ISEAnalyzer IsNot Nothing AndAlso ISEAnalyzer.ConnectionTasksCanContinue Then

                        'SGM 27/03/2012 proposal
                        'If ISEAnalyzer.IsISEModuleInstalled And _
                        '((ISEAnalyzer.IsISESwitchedON And ISEAnalyzer.IsInitiatedOK) Or _
                        ' (Not ISEAnalyzer.IsISESwitchedON)) Then

                        If Not CheckIfProcessCanContinue() Then
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 1, True)
                        Else
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 0, False)

                            ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.Wash) 'BA-2075
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.ProcessStaticBaseLine) 'BA-2075
                            ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.ProcessDynamicBaseLine) 'BA-2075
                        End If

                        'AG 23/03/2012 - While ise is initiating save all the ansinf received (commented)
                    Else
                        'AG 24/05/2012 - If exists inform about bottle alarms 
                        'Case: Start instrument without reactions rotor and without bottles
                        '1) Sw ask for execute the change rotor utility
                        '2) When the washing station is risen the Sw starts initializating ISE but not inform about bottle alarms  if next code does not exists
                        If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString), "INPROCESS") Then
                            If ExistBottleAlarms() Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "CANCELED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "CANCELED")

                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 1, True)
                            Else
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 0, True)
                            End If
                        End If
                        'AG 24/05/2012
                    End If

                    'AG 19/04/2012 - Abort (with out wash) is allowed while Partial Freeze
                ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso String.Equals(analyzerFREEZEModeAttribute, "PARTIAL") Then
                    If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString), "INPROCESS") Then
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "CLOSED")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "END")

                        'AG 04/09/2012 - When ABORT session finishes. Auto ABORT due to re-connection in running with incosistent data ... execute the full connection process
                        'Else execute previous code: Ask for status
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True)
                        If ForceAbortSessionAttr Then
                            ProcessConnection(Nothing, True)
                        Else
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True)
                        End If
                        'AG 04/09/2012
                    End If
                    'AG 19/04/2012
                End If

                'Update analyzer session flags into DataBase
                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSINF instruction
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pSensors "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 15/04/2011
        ''' Modified by XBC 18/10/2012 - functionality to WATER_DEPOSIT and WASTE_DEPOSIT alarms using timers
        ''' </remarks>
        Private Function ServiceSwAnsInfTreatment(ByVal pSensors As Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myUtil As New Utilities
                Dim myCalc As New CalculationsDelegate()

                Dim isLogActivity As Boolean = False 'SGM 11/10/2011

                Dim alarmStatus As Boolean = False ' XBC 18/10/2012

                'If Not MyClass.IsDisplayingServiceData Then Exit Try 'only while not displaying data

                'Get General cover (parameter index 3)
                Dim myIntValue As Integer = 0
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL), True)
                End If


                'Get Photometrics (Reaccions) cover (parameter index 4)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS), True)
                End If


                'Get Reagents (Fridge) cover (parameter index 5)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE), True)
                End If


                'Get Samples cover (parameter index 6)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES, pSensors(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES), True)
                End If


                'Get System liquid sensor (parameter index 7)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT) Then
                    ' XBC 18/10/2012 - Sw do not generates the alarm WATER_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    ''Update the class attribute SensorValuesAttribute
                    'isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT), True)

                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT))

                    If myIntValue = 0 Or myIntValue = 2 Then 'Empty or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT), False)

                    If alarmStatus AndAlso Not waterDepositTimer.Enabled Then
                        waterDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)
                    ElseIf Not alarmStatus AndAlso waterDepositTimer.Enabled Then
                        waterDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    ' XBC 18/10/2012

                End If


                'Get Waste sensor (parameter index 8)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT) Then
                    ' XBC 18/10/2012 - Sw do not generates the alarm WASTE_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    ''Update the class attribute SensorValuesAttribute
                    'isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT), True)

                    myIntValue = CInt(pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT))

                    If myIntValue = 3 Or myIntValue = 2 Then 'Full or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT), False)

                    If alarmStatus AndAlso Not wasteDepositTimer.Enabled Then
                        wasteDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)
                    ElseIf Not alarmStatus AndAlso wasteDepositTimer.Enabled Then
                        wasteDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    ' XBC 18/10/2012

                End If

                'take adjustment values for Scales and Temperature managing
                Dim myAdjustmentsDS As New SRVAdjustmentsDS
                Dim myAdjustmentDelegate As FwAdjustmentsDelegate



                myGlobal = MyClass.ReadFwAdjustmentsDS()
                If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                    myAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                    'instantiate the delegate
                    myAdjustmentDelegate = New FwAdjustmentsDelegate(myAdjustmentsDS)
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If


                'SGM 11/10/2011 log only in case of variation >=5%

                'Get wash solution sensor (parameter index 9)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                    Dim myEmptyLevel As Single
                    Dim myFullLevel As Single
                    Dim myLastLevel As Single
                    Dim myNewLevel As Single
                    Dim myLastPercent As Single
                    Dim myNewPercent As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.WASHING_SOLUTION.ToString, _
                                                                                      GlobalEnumerates.AXIS.EMPTY.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myEmptyLevel = myUtil.FormatToSingle(myValue)
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.WASHING_SOLUTION.ToString, _
                                                                                      GlobalEnumerates.AXIS.FULL.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myFullLevel = myUtil.FormatToSingle(myValue)
                    End If

                    If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    If SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                        myLastLevel = SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION)
                        myNewLevel = pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION)

                        myGlobal = myUtil.CalculatePercent(myLastLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myLastPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        myGlobal = myUtil.CalculatePercent(myNewLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myNewPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        If Math.Abs(myNewPercent - myLastPercent) >= 5 Then
                            isLogActivity = True
                        Else
                            isLogActivity = False
                        End If

                        'Else

                        '    isLogActivity = True

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION), True)



                End If

                'Get high contamination sensor (parameter index 10)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) Then
                    Dim myEmptyLevel As Single
                    Dim myFullLevel As Single
                    Dim myLastLevel As Single
                    Dim myNewLevel As Single
                    Dim myLastPercent As Single
                    Dim myNewPercent As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.HIGH_CONTAMINATION.ToString, _
                                                                                      GlobalEnumerates.AXIS.EMPTY.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myEmptyLevel = myUtil.FormatToSingle(myValue)
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.HIGH_CONTAMINATION.ToString, _
                                                                                      GlobalEnumerates.AXIS.FULL.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myFullLevel = myUtil.FormatToSingle(myValue)
                    End If

                    If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    If SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) Then

                        myLastLevel = SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE)
                        myNewLevel = pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE)

                        myGlobal = myUtil.CalculatePercent(myLastLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myLastPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        myGlobal = myUtil.CalculatePercent(myNewLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myNewPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        If Math.Abs(myNewPercent - myLastPercent) >= 5 Then
                            isLogActivity = True
                        Else
                            isLogActivity = False
                        End If

                        'Else

                        '    isLogActivity = True

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, pSensors(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE), True)


                End If

                'END SGM 11/10/2011


                ' XBC 22/06/2011 - Thermos temperatures must be add a correction to monitorize them
                'obtain Master Data Adjustments DS


                'Get fridge temperature sensor (parameter index 11)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_FRIDGE.ToString, _
                                                                                      GlobalEnumerates.AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_FRIDGE.ToString, _
                                                                                      GlobalEnumerates.AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = myUtil.FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE), True)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                End If

                'Get Reactions Rotor temperature sensor (parameter index 13)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY.ToString, _
                                                                                      GlobalEnumerates.AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY.ToString, _
                                                                                      GlobalEnumerates.AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = myUtil.FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS), True)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get WS temperature sensor (parameter index 14)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_WS_HEATER.ToString, _
                                                                                      GlobalEnumerates.AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_WS_HEATER.ToString, _
                                                                                      GlobalEnumerates.AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = myUtil.FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION), True)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get R1 temperature sensor (parameter index 15)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_REAGENT1.ToString, _
                                                                                      GlobalEnumerates.AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_REAGENT1.ToString, _
                                                                                      GlobalEnumerates.AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = myUtil.FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1), True)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get R2 temperature sensor (parameter index 16)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_REAGENT2.ToString, _
                                                                                      GlobalEnumerates.AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(GlobalEnumerates.ADJUSTMENT_GROUPS.THERMOS_REAGENT2.ToString, _
                                                                                      GlobalEnumerates.AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = myUtil.FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = myUtil.FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2), True)
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If


                'Get Fridge status (parameter index 12)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS, pSensors(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS), False)
                End If


                'Get ISE status (parameter index 17)
                If pSensors.ContainsKey(GlobalEnumerates.AnalyzerSensors.ISE_STATUS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_STATUS, pSensors(GlobalEnumerates.AnalyzerSensors.ISE_STATUS), False)
                End If

                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                'RaiseEvent SensorValuesChangedEvent(myUI_RefreshDS.SensorValueChanged)


                If isLogActivity Then
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Received Instruction [" & AppLayer.InstructionReceived & "]", "ApplicationLayer.ActivateProtocol (case RECEIVE)", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwAnsInfTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function




        ''' <summary>
        ''' SW has received FW data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 02/06/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceFWInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.FW_INFO, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                'If Not MyClass.IsDisplayingServiceData Then Exit Try 'only while not displaying data

                'Generate UI_Refresh Firmware values dataset
                Dim myNewFirmwareValuesRow As UIRefreshDS.FirmwareValueChangedRow
                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.FirmwareValueChanged
                    myNewFirmwareValuesRow = myUI_RefreshDS.FirmwareValueChanged.NewFirmwareValueChangedRow
                    With myNewFirmwareValuesRow
                        .BeginEdit()
                        .ElementID = pElements(GlobalEnumerates.FW_INFO.ID)
                        .BoardSerialNumber = pElements(GlobalEnumerates.FW_INFO.SMC)

                        .RepositoryVersion = pElements(GlobalEnumerates.FW_INFO.RV)
                        .RepositoryCRCResult = pElements(GlobalEnumerates.FW_INFO.CRC)
                        .RepositoryCRCValue = pElements(GlobalEnumerates.FW_INFO.CRCV)
                        .RepositoryCRCSize = pElements(GlobalEnumerates.FW_INFO.CRCS)

                        .BoardFirmwareVersion = pElements(GlobalEnumerates.FW_INFO.FWV)
                        .BoardFirmwareCRCResult = pElements(GlobalEnumerates.FW_INFO.FWCRC)
                        .BoardFirmwareCRCValue = pElements(GlobalEnumerates.FW_INFO.FWCRCV)
                        .BoardFirmwareCRCSize = pElements(GlobalEnumerates.FW_INFO.FWCRCS)

                        .BoardHardwareVersion = pElements(GlobalEnumerates.FW_INFO.HWV)

                        If .ElementID = GlobalEnumerates.POLL_IDs.CPU.ToString Then
                            .AnalyzerSerialNumber = pElements(GlobalEnumerates.FW_INFO.ASN)
                        End If
                        .EndEdit()
                    End With
                    myUI_RefreshDS.FirmwareValueChanged.AddFirmwareValueChangedRow(myNewFirmwareValuesRow)
                    myUI_RefreshDS.FirmwareValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceFWInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ' TO DELETE !!!
        '''' <summary>
        '''' SW has received MANIFOLD data
        '''' Service Sw has his own threatment different form User SW
        '''' </summary>
        '''' <param name="pElements "></param>
        '''' <returns></returns>
        '''' <remarks>SGM 23/05/2011 - creation </remarks>
        'Private Function ServiceSwManifoldInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.MANIFOLD_ELEMENTS, String)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        'They must be in order
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_TEMP) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_TEMP, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_TEMP), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1H), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1H) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1H, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1H), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1A) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1A, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR1A), True)
        '        End If


        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2H), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2H) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2H, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2H), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2A) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2A, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MR2A), True)
        '        End If


        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MS) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MS, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MS), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSH) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSH, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSH), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSA) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSA, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_MSA), True)
        '        End If


        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B1D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B2D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_B3D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV1D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV2D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV3D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV4D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV5D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_EV6D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE1D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE2D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3D) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3D, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_GE3D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLT) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLT, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLT), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLTD) Then
        '            UpdateManifoldValuesAttribute(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLTD, pElements(GlobalEnumerates.MANIFOLD_ELEMENTS.JE1_CLTD), True)
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwManifoldInfoTreatment", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SW has received FLUIDIC data
        '''' Service Sw has his own threatment different form User SW
        '''' </summary>
        '''' <param name="pElements "></param>
        '''' <returns></returns>
        '''' <remarks>SGM 23/05/2011 - creation </remarks>
        'Private Function ServiceSwFluidicsInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.FLUIDICS_ELEMENTS, String)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try


        '        'They must be in order
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_TEMP) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_TEMP, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_TEMP), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSH) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSH, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSH), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSA) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSA, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MSA), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B1D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B2D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B3D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B4D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B5D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B6D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B7D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B8D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B9D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10D) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10D, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_B10D), True)
        '        End If

        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV1) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV1, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV1), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV2) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV2, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_EV2), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_MS), True)
        '        End If
        '        If pElements.ContainsKey(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_GE1) Then
        '            UpdateFluidicsValuesAttribute(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_GE1, pElements(GlobalEnumerates.FLUIDICS_ELEMENTS.SF1_GE1), True)
        '        End If
        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwManifoldInfoTreatment", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SW has received PHOTOMETRICS data
        '''' Service Sw has his own threatment different form User SW
        '''' </summary>
        '''' <param name="pElements "></param>
        '''' <returns></returns>
        '''' <remarks>SGM 23/05/2011 - creation </remarks>
        'Private Function ServiceSwPhotometricsInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.PHOTOMETRICS_ELEMENTS, String)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        'They must be in order
        '        If pElements.ContainsKey(GlobalEnumerates.PHOTOMETRICS_ELEMENTS.GLF_TEMP) Then
        '            UpdatePhotometricsValuesAttribute(GlobalEnumerates.PHOTOMETRICS_ELEMENTS.GLF_TEMP, pElements(GlobalEnumerates.PHOTOMETRICS_ELEMENTS.GLF_TEMP), True)
        '        End If



        '        'PDT

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwPhotometricsInfoTreatment", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function
        ' TO DELETE !!!

        ''' <summary>
        ''' SW has received ARMS data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 31/05/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwArmsInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.ARMS_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                'If Not MyClass.IsDisplayingServiceData Then Exit Try 'only while not displaying data

                'Generate UI_Refresh Arms values dataset
                Dim myNewArmsValuesRow As UIRefreshDS.ArmValueChangedRow
                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ArmValueChanged
                    myNewArmsValuesRow = myUI_RefreshDS.ArmValueChanged.NewArmValueChangedRow
                    With myNewArmsValuesRow
                        .BeginEdit()
                        .ArmID = pElements(GlobalEnumerates.ARMS_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(GlobalEnumerates.ARMS_ELEMENTS.TMP), Single)
                        .MotorHorizontal = pElements(GlobalEnumerates.ARMS_ELEMENTS.MH)
                        .MotorHorizontalHome = pElements(GlobalEnumerates.ARMS_ELEMENTS.MHH)
                        .MotorHorizontalPosition = CType(pElements(GlobalEnumerates.ARMS_ELEMENTS.MHA), Single)
                        .MotorVertical = pElements(GlobalEnumerates.ARMS_ELEMENTS.MV)
                        .MotorVerticalHome = pElements(GlobalEnumerates.ARMS_ELEMENTS.MVH)
                        .MotorVerticalPosition = CType(pElements(GlobalEnumerates.ARMS_ELEMENTS.MVA), Single)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.ArmValueChanged.AddArmValueChangedRow(myNewArmsValuesRow)

                    'myUI_RefreshDS.AcceptChanges()
                    myUI_RefreshDS.ArmValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                End SyncLock

                'Generate UI_Refresh event ARMSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ARMSVALUE_CHANGED) Then myUI_RefreshEvent.Add(GlobalEnumerates.UI_RefreshEvents.ARMSVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwArmsInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received PROBES data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation 
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwProbesInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.PROBES_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Generate UI_Refresh Arms Probes dataset
                Dim myNewprobesValuesRow As UIRefreshDS.ProbeValueChangedRow

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ProbeValueChanged
                    myNewprobesValuesRow = myUI_RefreshDS.ProbeValueChanged.NewProbeValueChangedRow
                    With myNewprobesValuesRow
                        .BeginEdit()
                        .ProbeID = pElements(GlobalEnumerates.PROBES_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.TMP), Single)
                        .DetectionStatus = pElements(GlobalEnumerates.PROBES_ELEMENTS.DST)
                        .DetectionFrequency = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.DFQ), Single)
                        .Detection = pElements(GlobalEnumerates.PROBES_ELEMENTS.D)
                        .LastInternalRate = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.DCV), Single)
                        .ThermistorValue = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.PTH), Single)
                        .ThermistorDiagnostic = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.PTHD), Single)
                        .HeaterStatus = pElements(GlobalEnumerates.PROBES_ELEMENTS.PH)
                        .HeaterDiagnostic = CType(pElements(GlobalEnumerates.PROBES_ELEMENTS.PHD), Single)
                        .CollisionDetector = pElements(GlobalEnumerates.PROBES_ELEMENTS.CD)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.ProbeValueChanged.AddProbeValueChangedRow(myNewprobesValuesRow)
                    myUI_RefreshDS.ProbeValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                'Generate UI_Refresh event PROBESSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then myUI_RefreshEvent.Add(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwProbesInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received ROTORS data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwRotorsInfoTreatment(ByVal pElements As Dictionary(Of GlobalEnumerates.ROTORS_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Generate UI_Refresh Rotors values dataset
                Dim myNewRotorsValuesRow As UIRefreshDS.RotorValueChangedRow
                SyncLock myUI_RefreshDS.RotorValueChanged
                    myNewRotorsValuesRow = myUI_RefreshDS.RotorValueChanged.NewRotorValueChangedRow
                    With myNewRotorsValuesRow
                        .BeginEdit()
                        .RotorID = pElements(GlobalEnumerates.ROTORS_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.TMP), Single)
                        .Motor = pElements(GlobalEnumerates.ROTORS_ELEMENTS.MR)
                        .MotorHome = pElements(GlobalEnumerates.ROTORS_ELEMENTS.MRH)
                        .MotorPosition = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.MRA), Single)
                        .ThermistorFridgeValue = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FTH), Single)
                        .ThermistorFridgeDiagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FTHD), Single)
                        .PeltiersFridgeStatus = pElements(GlobalEnumerates.ROTORS_ELEMENTS.FH)
                        .PeltiersFridgeDiagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FHD), Single)
                        .PeltiersFan1Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF1), Single)
                        .PeltiersFan1Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF1D), Single)
                        .PeltiersFan2Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF2), Single)
                        .PeltiersFan2Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF2D), Single)
                        .PeltiersFan3Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF3), Single)
                        .PeltiersFan3Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF3D), Single)
                        .PeltiersFan4Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF4), Single)
                        .PeltiersFan4Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.PF4D), Single)
                        .FrameFan1Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FF1), Single)
                        .FrameFan1Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FF1D), Single)
                        .FrameFan2Speed = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FF2), Single)
                        .FrameFan2Diagnostic = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.FF2D), Single)
                        .Cover = pElements(GlobalEnumerates.ROTORS_ELEMENTS.RC)
                        .BarCodeStatus = CType(pElements(GlobalEnumerates.ROTORS_ELEMENTS.CB), Single)
                        .BarcodeError = pElements(GlobalEnumerates.ROTORS_ELEMENTS.CBE)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.RotorValueChanged.AddRotorValueChangedRow(myNewRotorsValuesRow)
                    myUI_RefreshDS.RotorValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock

                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                'Generate UI_Refresh event ROTORSSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORSVALUE_CHANGED) Then myUI_RefreshEvent.Add(GlobalEnumerates.UI_RefreshEvents.ROTORSVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwRotorsInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function



        ''' <summary>
        ''' Update the internal attribte SensorValuesAttribute
        ''' </summary>
        ''' <param name="pSensor"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Function UpdateSensorValuesAttribute(ByVal pSensor As GlobalEnumerates.AnalyzerSensors, ByVal pNewValue As Single, ByVal pUIEventForChangesFlag As Boolean) As Boolean
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            Try

                'SGM 29/09/2011
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    If InfoRefreshFirstTime Then
                        InfoRefreshFirstTime = False
                        If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                            SensorValuesAttribute.Add(pSensor, pNewValue)

                        Else
                            SensorValuesAttribute(pSensor) = pNewValue
                        End If
                        flagChanges = True
                    Else
                        If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                            SensorValuesAttribute.Add(pSensor, pNewValue)
                            flagChanges = True

                        ElseIf SensorValuesAttribute(pSensor) <> pNewValue Then
                            SensorValuesAttribute(pSensor) = pNewValue
                            flagChanges = True
                        End If
                    End If


                Else

                    If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                        SensorValuesAttribute.Add(pSensor, pNewValue)
                        flagChanges = True

                    ElseIf SensorValuesAttribute(pSensor) <> pNewValue Then
                        SensorValuesAttribute(pSensor) = pNewValue
                        flagChanges = True
                    End If

                End If

                If pUIEventForChangesFlag And flagChanges Then
                    'Generate UI_Refresh event SENSORVALUE_CHANGED
                    PrepareUIRefreshEventNum2(Nothing, GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED, "", -1, "", "", -1, -1, "", "", pSensor, pNewValue, Nothing, -1, -1, "", "")

                    'AG 17/10/2011
                    'Special business for the CONNECTED false sensor ... insert into Alarms table (only for User Sw)
                    If pSensor = GlobalEnumerates.AnalyzerSensors.CONNECTED Then

                        '' TO DELETE - XBC 06/07/2012
                        'If pNewValue = 0 Then
                        '    Debug.Print("CONNECTED = 0")
                        'Else
                        '    Debug.Print("CONNECTED = 1")
                        'End If


                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            'Nothing

                        Else
                            'Add into alarms table
                            Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                            Dim myAlarmStatusList As New List(Of Boolean)
                            Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.COMMS_ERR
                            Dim alarmStatus As Boolean = False
                            If pNewValue <> 1 Then alarmStatus = True
                            PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)
                            If myAlarmList.Count > 0 Then
                                Dim myGlobal As New GlobalDataTO
                                myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                            End If
                        End If

                    End If
                    'AG 17/10/2011

                    Return True
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateSensorValuesAttribute", EventLogEntryType.Error, False)
            End Try

            Return False

        End Function


        ''' <summary>
        ''' Update the internal attribte CpuValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateCpuValuesAttribute(ByVal pElement As GlobalEnumerates.CPU_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not CpuValuesAttribute.ContainsKey(pElement) Then
                CpuValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf CpuValuesAttribute(pElement) <> pNewValue Then
                CpuValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'If pUIEventForChangesFlag And flagChanges Then
            ' XBC 09/06/2011 - is interesting event will be invoked always that an ANSCPU instruction is received
            'Generate UI_Refresh event CPUVALUE_CHANGED
            PrepareUIRefreshEventCpu(Nothing, GlobalEnumerates.UI_RefreshEvents.CPUVALUE_CHANGED, pElement, pNewValue)
            'End If

        End Sub

        ''' <summary>
        ''' Update the internal attribte CyclesValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateHwCyclesValuesAttribute(ByVal pElement As GlobalEnumerates.CYCLE_ELEMENTS, _
                                                  ByVal pSubsystem As GlobalEnumerates.SUBSYSTEMS, _
                                                  ByVal pUnits As GlobalEnumerates.CYCLE_UNITS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            'If Not CyclesValuesAttribute.ContainsKey(pElement) Then
            '    CyclesValuesAttribute.Add(pElement, pNewValue)
            '    flagChanges = True

            'ElseIf CyclesValuesAttribute(pElement) <> pNewValue Then
            '    CyclesValuesAttribute(pElement) = pNewValue
            '    flagChanges = True
            'End If

            'Generate UI_Refresh event MANIFOLDVALUE_CHANGED
            PrepareUIRefreshEventCycles(Nothing, GlobalEnumerates.UI_RefreshEvents.HWCYCLES_CHANGED, pElement, pSubsystem, pUnits, pNewValue)

        End Sub

        ''' <summary>
        ''' Update the internal attribte ManifoldValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateManifoldValuesAttribute(ByVal pElement As GlobalEnumerates.MANIFOLD_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not ManifoldValuesAttribute.ContainsKey(pElement) Then
                ManifoldValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf ManifoldValuesAttribute(pElement) <> pNewValue Then
                ManifoldValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event MANIFOLDVALUE_CHANGED
            PrepareUIRefreshEventManifold(Nothing, GlobalEnumerates.UI_RefreshEvents.MANIFOLDVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' Update the internal attribte FluidicsValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateFluidicsValuesAttribute(ByVal pElement As GlobalEnumerates.FLUIDICS_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not FluidicsValuesAttribute.ContainsKey(pElement) Then
                FluidicsValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf FluidicsValuesAttribute(pElement) <> pNewValue Then
                FluidicsValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event FLUIDICSVALUE_CHANGED
            PrepareUIRefreshEventFluidics(Nothing, GlobalEnumerates.UI_RefreshEvents.FLUIDICSVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' Update the internal attribte PhotometricsValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdatePhotometricsValuesAttribute(ByVal pElement As GlobalEnumerates.PHOTOMETRICS_ELEMENTS, _
                                                      ByVal pNewValue As String, _
                                                      ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not PhotometricsValuesAttribute.ContainsKey(pElement) Then
                PhotometricsValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf PhotometricsValuesAttribute(pElement) <> pNewValue Then
                PhotometricsValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event FLUIDICSVALUE_CHANGED
            PrepareUIRefreshEventPhotometrics(Nothing, GlobalEnumerates.UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pCounts"></param>
        ''' <param name="pWashSolutionBottleFlag" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created SGM
        ''' Modified AG 27/04/2011 - use linq instead loop for searching the adjustment
        ''' </remarks>
        Public Function GetBottlePercentage(ByVal pCounts As Integer, ByVal pWashSolutionBottleFlag As Boolean) As GlobalDataTO Implements IAnalyzerEntity.GetBottlePercentage
            Dim myGlobal As New GlobalDataTO

            Try
                'obtain adjustments
                Dim MinAdj As Double = -1
                Dim MaxAdj As Double = -1

                Dim minCode As String = ""
                Dim maxCode As String = ""

                If pWashSolutionBottleFlag Then 'Wash solution bottle adjustments
                    minCode = GlobalEnumerates.Ax00Adjustsments.GSMI.ToString
                    maxCode = GlobalEnumerates.Ax00Adjustsments.GSMA.ToString
                Else 'High contamination bottle adjustments
                    minCode = GlobalEnumerates.Ax00Adjustsments.GWMI.ToString
                    maxCode = GlobalEnumerates.Ax00Adjustsments.GWMA.ToString
                End If

                myGlobal = AppLayer.ReadFwAdjustmentsDS()
                If myGlobal.SetDatos IsNot Nothing Then
                    Dim myAdjDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If myAdjDS IsNot Nothing Then
                        Dim myRes As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                        myRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                 Where a.CodeFw = minCode Select a).ToList
                        If myRes.Count > 0 AndAlso IsNumeric(myRes(0).Value) Then
                            MinAdj = CDbl(myRes(0).Value)
                        End If

                        myRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                Where a.CodeFw = maxCode Select a).ToList
                        If myRes.Count > 0 AndAlso IsNumeric(myRes(0).Value) Then
                            MaxAdj = CDbl(myRes(0).Value)
                        End If
                        myRes = Nothing 'AG 02/08/2012 - free memory

                    End If
                End If

                If MinAdj <> -1 AndAlso MaxAdj <> -1 AndAlso MinAdj <> MaxAdj Then
                    Dim myUtil As New Utilities
                    myGlobal = myUtil.CalculatePercent(pCounts, MinAdj, MaxAdj, True)
                Else
                    myGlobal.SetDatos = Nothing
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetBottlePercentage", EventLogEntryType.Error, False)
            End Try
            Return myGlobal

        End Function



        ''' <summary>
        ''' The fridge status damaged is when fw sends any of these error codes: 301, 302 or 303 (belongs to FRIDGE_TEMP_SYS_ERR and FRIDGE_FAN_WARN)
        ''' </summary>
        ''' <param name="pAlarmList"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Private Function IsFridgeStatusDamaged(ByVal pAlarmList As List(Of GlobalEnumerates.Alarms), ByVal pAlarmStatusList As List(Of Boolean)) As Boolean
            Dim myReturn As Boolean = False
            Try
                'DL 31/07/2012. Begin
                If pAlarmList.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) OrElse pAlarmList.Contains(GlobalEnumerates.Alarms.FRIDGE_FAN_WARN) Then
                    'If pAlarmList.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS1_ERR) OrElse _
                    '  pAlarmList.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS2_ERR) OrElse _
                    ' pAlarmList.Contains(GlobalEnumerates.Alarms.FRIDGE_FAN_WARN) Then
                    myReturn = True
                End If
                'DL 31/07/2012. End
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IsFridgeStatusDamaged", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' Error codes not detected yet are marked as solved
        ''' </summary>
        ''' <param name="pErrorCodeList"></param>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Sub SolveErrorCodesToDisplay(ByVal pErrorCodeList As List(Of String)) Implements IAnalyzerEntity.SolveErrorCodesToDisplay
            Try
                If Not myErrorCodesDisplayAttribute Is Nothing AndAlso _
                   myErrorCodesDisplayAttribute.Count > 0 AndAlso _
                   Not pErrorCodeList Is Nothing AndAlso _
                   pErrorCodeList.Count > 0 Then

                    ' Clone myErrorCodesDisplayAttribute into Aux Structure
                    Dim myErrorCodesDisplayAttributeAux As New List(Of ErrorCodesDisplayStruct)
                    For Each myErrorCodesDisplayRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttribute
                        Dim myErrorCodesDisplayAuxRow As New ErrorCodesDisplayStruct
                        myErrorCodesDisplayAuxRow.ErrorDateTime = myErrorCodesDisplayRow.ErrorDateTime
                        myErrorCodesDisplayAuxRow.ErrorCode = myErrorCodesDisplayRow.ErrorCode
                        myErrorCodesDisplayAuxRow.ResourceID = myErrorCodesDisplayRow.ResourceID
                        myErrorCodesDisplayAuxRow.Solved = myErrorCodesDisplayRow.Solved
                        myErrorCodesDisplayAttributeAux.Add(myErrorCodesDisplayAuxRow)
                    Next

                    ' Remove myErrorCodesDisplayAttributeAux
                    myErrorCodesDisplayAttribute.Clear()

                    ' Check error codes that has been solved (not exists on pErrorCodeList)
                    For Each myErrorCodesDisplayAuxRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttributeAux
                        If Not pErrorCodeList.Contains(myErrorCodesDisplayAuxRow.ErrorCode) Then
                            ' Add the error code again but with the value Solved as TRUE
                            Dim myErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                            myErrorCodesDisplayRow.ErrorDateTime = myErrorCodesDisplayAuxRow.ErrorDateTime
                            myErrorCodesDisplayRow.ErrorCode = myErrorCodesDisplayAuxRow.ErrorCode
                            myErrorCodesDisplayRow.ResourceID = myErrorCodesDisplayAuxRow.ResourceID
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                                myErrorCodesDisplayRow.Solved = True
                            Else
                                myErrorCodesDisplayRow.Solved = False
                            End If
                            myErrorCodesDisplayAttribute.Add(myErrorCodesDisplayRow)
                        ElseIf pErrorCodeList.Contains(myErrorCodesDisplayAuxRow.ErrorCode) AndAlso _
                               myErrorCodesDisplayAuxRow.Solved Then
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                            End If
                            ' There are the same previous error but was solved, in this case, the historic log keep this information
                            Dim myErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                            myErrorCodesDisplayRow.ErrorDateTime = myErrorCodesDisplayAuxRow.ErrorDateTime
                            myErrorCodesDisplayRow.ErrorCode = myErrorCodesDisplayAuxRow.ErrorCode
                            myErrorCodesDisplayRow.ResourceID = myErrorCodesDisplayAuxRow.ResourceID
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                                myErrorCodesDisplayRow.Solved = True
                            Else
                                myErrorCodesDisplayRow.Solved = False
                            End If
                            myErrorCodesDisplayAttribute.Add(myErrorCodesDisplayRow)
                        End If
                    Next

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SolveErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Add new item to Error Codes to display List
        ''' </summary>
        ''' <param name="pErrorCodesDS"></param>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Sub AddErrorCodesToDisplay(ByVal pErrorCodesDS As AlarmErrorCodesDS) Implements IAnalyzerEntity.AddErrorCodesToDisplay
            Try
                If Not myErrorCodesDisplayAttribute Is Nothing AndAlso _
                   Not pErrorCodesDS Is Nothing AndAlso _
                   pErrorCodesDS.tfmwAlarmErrorCodes.Count > 0 Then

                    ' Check is Error Code exists on structure
                    Dim AddItPlease As Boolean = True
                    For Each myErrorCodesDisplayRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttribute
                        If myErrorCodesDisplayRow.ErrorCode = pErrorCodesDS.tfmwAlarmErrorCodes(0).ErrorCode.ToString Then
                            If myErrorCodesDisplayRow.Solved Then
                                ' Exist but it was previously solved and appears the same error again
                                AddItPlease = True
                                Exit For
                            Else
                                AddItPlease = False
                            End If
                        End If
                    Next
                    If AddItPlease Then
                        ' Add new error codes received (avoiding repetitions of NOT solved errors)
                        Dim newErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                        newErrorCodesDisplayRow.ErrorDateTime = DateTime.Now
                        newErrorCodesDisplayRow.ErrorCode = pErrorCodesDS.tfmwAlarmErrorCodes(0).ErrorCode.ToString
                        newErrorCodesDisplayRow.ResourceID = pErrorCodesDS.tfmwAlarmErrorCodes(0).ResourceID
                        newErrorCodesDisplayRow.Solved = False
                        myErrorCodesDisplayAttribute.Add(newErrorCodesDisplayRow)
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.AddErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Clear Error Codes to display list
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Function RemoveErrorCodesToDisplay() As GlobalDataTO Implements IAnalyzerEntity.RemoveErrorCodesToDisplay
            Dim myGlobal As New GlobalDataTO
            Try
                myErrorCodesDisplayAttribute.Clear()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Aborted processes due alarms"

        ''' <summary>
        ''' Some processes can be aborted due analyzer alarms:
        ''' - During Wup process do not sent WASH or ALIGHT if bottle/deposit alarms exists
        ''' - During Sdowh process do not sent WASH or ALIGHT if bottle/deposit alarms exists
        ''' - During ChangeRotor / Alight util process do not sent ALIGHT if bottle/deposit alarms exists
        ''' - During Abort do not sent WASH if bottle/deposit alarms exists
        ''' 
        ''' </summary>
        ''' <remarks>AG 29/09/2011
        ''' Modified by: IT 01/12/2014 - BA-2075
        ''' </remarks>
        Private Function CheckIfProcessCanContinue() As Boolean
            Dim readyToSentInstruction As Boolean = True ' True next instruction can be sent / False can not be sent due a bottle alarms
            Try
                Dim bottleErrAlarm As Boolean = False
                Dim reactRotorMissingAlarm As Boolean = myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) 'AG 12/03/2012
                Dim myGlobal As New GlobalDataTO
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                'AG 20/02/2012 - The following condition is added into a public method
                'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                '  OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                '  OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) _
                '  OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) Then
                If ExistBottleAlarms() Then
                    bottleErrAlarm = True
                End If

                'AG 22/02/2012 - Change bottle confirmation process
                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) = "INPROCESS" Then
                    Dim askForStateFlag As Boolean = True
                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process, "CLOSED")

                    If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                        readyToSentInstruction = False

                    Else
                        'If change bottles confirmation success and there was an ABORT ws wash pending ... send it automatically
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "PAUSED" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "INPROCESS")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                            askForStateFlag = False 'In this case is not necessary to ask for status, we sent the Wash instruction

                            'If change bottles confirmation success and there was an RECOVER ws wash pending ... send it automatically
                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess.ToString) = "PAUSED" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "INPROCESS")
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "")
                            askForStateFlag = False 'In this case is not necessary to ask for status, we sent the Wash instruction

                        Else 'for other processes activate action button but not send next instruction automatically

                        End If
                    End If

                    'Finally Sw also ask for status
                    'Send a STATE instruction
                    If askForStateFlag Then
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True)
                    End If

                End If
                'AG 22/02/2012

                If readyToSentInstruction Then

                    If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                        readyToSentInstruction = False
                    End If

                    'Some action button process required Wash instruction is bottle level OK before Sw sends it
                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.Washing.ToString) = "" Then

                        'AG 20/02/2012 - abort process
                        'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                                    readyToSentInstruction = False
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "PAUSED")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "CANCELED")

                                    'Else 'AG 28/02/2012 - when ABORT send the WASH instrucion
                                Else
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "INPROCESS") 'AG 05/03/2012
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "INI")

                                    'Send a WASH instruction (Conditioning complete)
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                                End If
                            End If
                            'AG 20/02/2012

                            'AG 08/03/2012 - recover process
                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess.ToString) = "INPROCESS" Then

                            If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                                readyToSentInstruction = False
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "PAUSED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "CANCELED")

                                'Else 'AG 28/02/2012 - when RECOVER finishes send the WASH instrucion only in standby
                            ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess, "INPROCESS") 'AG 05/03/2012
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "INI")

                                'Send a WASH instruction (Conditioning complete)
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                            End If

                        End If
                    End If
                End If

                'Update analyzer session flags into DataBase
                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CheckIfProcessCanContinue", EventLogEntryType.Error, False)
            End Try
            Return readyToSentInstruction
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CheckIfWashingIsPossible() As Boolean

            Dim readyToSentInstruction As Boolean = True ' True next instruction can be sent / False can not be sent due a bottle alarms
            Dim bottleErrAlarm As Boolean = False
            Dim reactRotorMissingAlarm As Boolean = myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) 'AG 12/03/2012
            Dim myGlobal As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If ExistBottleAlarms() Then
                bottleErrAlarm = True
            End If

            If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                readyToSentInstruction = False
            End If

            Return readyToSentInstruction

        End Function

#End Region

#Region "Freeze alarms methods"

        ''' <summary>
        ''' Transalte the Fw error code to an internal Sw alarm ID enumerate
        ''' Depending the error code the internal flag AnalyzerIsFREEZEMode is activated
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pErrorCodeList"></param>
        ''' <remarks>
        ''' AG 25/03/2011 - creation ... but not implemented due the error codes are not still defined
        ''' AG 02/03/2012 - redesigned with final Fw information
        ''' </remarks>
        Private Function TranslateErrorCodeToAlarmID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pErrorCodeList As List(Of Integer)) As List(Of GlobalEnumerates.Alarms)
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim toReturnAlarmList As New List(Of GlobalEnumerates.Alarms)

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim stringAlarmList As New List(Of String)
                        Dim myLinq As List(Of AlarmsDS.tfmwAlarmsRow)
                        Dim fwStartsFreezeFlag As Boolean = False 'AG 28/03/2012

                        'For each ErrorCode in pErrorCodeList search the related AlarmID and evaluate if Freeze
                        For Each row As Integer In pErrorCodeList
                            myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                      Where Not a.IsErrorCodeNull AndAlso a.ErrorCode = row Select a).ToList

                            If myLinq.Count > 0 Then
                                For Each item As AlarmsDS.tfmwAlarmsRow In myLinq
                                    'Get the related AlarmID

                                    'AG 05/03/2012
                                    'This line causes system error, vb .net can not convert a String (item.AlarmID) into a internal enumerate (GlobalEnumerates.Alarms)
                                    'Solution: Create a list of alarm string and the call a method who implements the select case to asign to proper GlobalEnumerates.Alarms ID
                                    'toReturnAlarmList.Add(CType(item.AlarmID, GlobalEnumerates.Alarms))

                                    If Not stringAlarmList.Contains(item.AlarmID) Then
                                        stringAlarmList.Add(item.AlarmID)

                                        ' XBC 16/10/2012 - Alarms treatment for Service
                                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                        If GlobalBase.IsServiceAssembly Then
                                            ' Not Apply
                                        Else
                                            'Evaluate if Freeze and FreezeMode
                                            If Not item.IsFreezeNull Then
                                                If Not analyzerFREEZEFlagAttribute Then
                                                    analyzerFREEZEFlagAttribute = True
                                                    analyzerFREEZEModeAttribute = item.Freeze '"Auto" or "Partial" or "Total" or "Reset"
                                                    fwStartsFreezeFlag = True 'AG 28/03/2012

                                                    'AG 07/03/2012 - RPM spec the cover errors are treated as "Total" freeze in running
                                                    '                in other status are a special freeze mode autosolved "auto"
                                                    If item.Freeze = "AUTO" And AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                                                        'analyzerFREEZEModeAttribute = "PARTIAL" 'AG - New spec RPM 08/03/2012 - Not partial, Total!!
                                                        analyzerFREEZEModeAttribute = "TOTAL"
                                                    End If
                                                    'AG 07/03/2012

                                                    'AG 28/03/2012
                                                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 1, True) 'AG 06/03/2012 - Inform presentation analyzer is FREEZE

                                                Else
                                                    'Inform when user try to connect after freeze on connection
                                                    If SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE) <> 1 Then
                                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 1, True) 'AG 06/03/2012 - Inform presentation analyzer is FREEZE
                                                    End If
                                                End If

                                                'Freeze priority: lower "AUTO", middle "PARTIAL", high "TOTAL", bestial "RESET"
                                                If item.Freeze = "PARTIAL" AndAlso analyzerFREEZEModeAttribute = "AUTO" Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Partial

                                                ElseIf item.Freeze = "TOTAL" AndAlso (analyzerFREEZEModeAttribute <> "TOTAL" AndAlso analyzerFREEZEModeAttribute <> "RESET") Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Total

                                                ElseIf item.Freeze = "RESET" AndAlso (analyzerFREEZEModeAttribute <> "RESET") Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Reset

                                                End If

                                            End If
                                        End If


                                    End If

                                Next

                            End If

                        Next
                        myLinq = Nothing

                        'Translate the stringAlarmList into toReturnAlarmList
                        If stringAlarmList.Count > 0 Then
                            toReturnAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)

                            ' XBC 16/10/2012 - Alarms treatment for Service
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                ' Not Apply
                            Else
                                'AG 28/03/2012 - The only AUTO freeze code error in standby is INSTRUCTION REJECTED ERROR (not covers)
                                'FREEZE is when Fw informs instruction rejected error because some enabled cover is opened
                                'not when informs only enabled cover opened
                                If fwStartsFreezeFlag AndAlso analyzerFREEZEModeAttribute = "AUTO" Then
                                    'Remove false Freeze
                                    If Not toReturnAlarmList.Contains(GlobalEnumerates.Alarms.INST_REJECTED_ERR) Then
                                        analyzerFREEZEFlagAttribute = False
                                        analyzerFREEZEModeAttribute = ""
                                    End If
                                End If

                                'Finally inform presentation layer the analyzer is FREEZE
                                If fwStartsFreezeFlag AndAlso analyzerFREEZEFlagAttribute Then
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 1, True)
                                End If
                                'AG 28/03/2012
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                toReturnAlarmList.Clear() 'Clear the alarm list to return

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.TranslateErrorCodeToAlarmID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (toReturnAlarmList)
        End Function

        ''' <summary>
        ''' Transforms a string list of Alarm Codes to a list of Alarm Enumerates
        ''' </summary>
        ''' <param name="pAlarmStringCodes">String List of Alarm Codes</param>
        ''' <returns>List of Alarm Enumerates</returns>
        ''' <remarks>
        ''' Created by:  AG 05/03/2012
        ''' Modified by: SA 22/10/2013 - BT #1355 - Added Case for new Alarm WS_PAUSE_MODE_WARN
        ''' </remarks>
        Private Function ConvertToAlarmIDEnumerate(ByVal pAlarmStringCodes As List(Of String)) As List(Of GlobalEnumerates.Alarms)
            'Do not implement Try/Catch: the caller method does it

            Dim alarmIDEnumList As New List(Of GlobalEnumerates.Alarms)
            For Each row As String In pAlarmStringCodes
                'AG 04/12/2014 BA-2146 code improvement
                alarmIDEnumList.Add(DirectCast([Enum].Parse(GetType(GlobalEnumerates.Alarms), row), GlobalEnumerates.Alarms))

                'This select must be updated with every alarmID added into enumerate GlobalEnumerates.Alarms
                'Select Case row
                '    Case "MAIN_COVER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.MAIN_COVER_ERR)
                '    Case "MAIN_COVER_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.MAIN_COVER_WARN)

                '    Case "WASH_CONTAINER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR)
                '    Case "WASH_CONTAINER_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN)

                '    Case "HIGH_CONTAMIN_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR)
                '    Case "HIGH_CONTAMIN_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN)

                '    Case "R1_TEMP_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_TEMP_WARN)
                '        'DL 31/07/2012. Begin. Remove R1_TEMP_SYSTEM_ERR
                '    Case "R1_TEMP_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR)
                '        'Case "R1_TEMP_SYS1_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_TEMP_SYS1_ERR)
                '        'Case "R1_TEMP_SYS2_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_TEMP_SYS2_ERR)
                '        'DL 31/07/2012. End
                '    Case "R1_DETECT_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR)
                '    Case "R1_NO_VOLUME_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN)
                '    Case "BOTTLE_LOCKED_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN) 'TR 01/10/2012 -Botlle locked alarm.
                '    Case "R1_COLLISION_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_COLLISION_ERR)
                '    Case "R1_COLLISION_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_COLLISION_WARN)
                '    Case "FRIDGE_COVER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR)
                '    Case "FRIDGE_COVER_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN)
                '    Case "FRIDGE_TEMP_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN)
                '    Case "FRIDGE_TEMP_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR)
                '        'DL 31/07/2012. Begin Remove FRIDGE_TEMP_SYS_ERR
                '    Case "FRIDGE_TEMP_SYS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR)
                '        'Case "FRIDGE_TEMP_SYS1_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS1_ERR)
                '        'Case "FRIDGE_TEMP_SYS2_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS2_ERR)
                '        'DL 31/07/2012. End
                '    Case "FRIDGE_STATUS_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN)
                '    Case "FRIDGE_STATUS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR)

                '    Case "R2_TEMP_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_TEMP_WARN)
                '        'DL 31/07/2012. Begin. Remove R2_TEMP_SYSTEM_ERR
                '    Case "R2_TEMP_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR)
                '        'Case "R2_TEMP_SYS1_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_TEMP_SYS1_ERR)
                '        'Case "R2_TEMP_SYS2_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_TEMP_SYS2_ERR)
                '        'DL 31/07/2012. End
                '    Case "R2_DETECT_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR)
                '    Case "R2_NO_VOLUME_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN)
                '    Case "R2_COLLISION_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_COLLISION_ERR)
                '    Case "R2_COLLISION_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_COLLISION_WARN)
                '    Case "WATER_DEPOSIT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR)
                '    Case "WATER_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR)
                '    Case "WASTE_DEPOSIT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR)
                '    Case "WASTE_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR)
                '    Case "REACT_ROTOR_FAN_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_ROTOR_FAN_WARN)
                '    Case "FRIDGE_FAN_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_FAN_WARN)
                '    Case "REACT_COVER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_COVER_ERR)
                '    Case "REACT_COVER_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_COVER_WARN)
                '    Case "REACT_MISSING_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_MISSING_ERR)
                '    Case "REACT_ENCODER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_ENCODER_ERR)
                '    Case "REACT_TEMP_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_TEMP_WARN)
                '    Case "REACT_TEMP_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_TEMP_ERR)
                '        'DL 31/07/2012. Begin. Remove REACT_TEMP_SYS_ERR
                '    Case "REACT_TEMP_SYS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR)
                '        'Case "REACT_TEMP_SYS1_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_TEMP_SYS1_ERR)
                '        'Case "REACT_TEMP_SYS2_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_TEMP_SYS2_ERR)
                '        'DL 31/07/2012. End
                '    Case "REACT_SAFESTOP_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACT_SAFESTOP_ERR)
                '    Case "WS_TEMP_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_TEMP_WARN)
                '        'DL 31/07/2012. Begin. Remove WS_TEMP_SYSTEM_ERR
                '    Case "WS_TEMP_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR)
                '        'Case "WS_TEMP_SYS1_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_TEMP_SYS1_ERR)
                '        'Case "WS_TEMP_SYS2_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_TEMP_SYS2_ERR)
                '        'DL 31/07/2012. End

                '    Case "WS_COLLISION_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_COLLISION_ERR)
                '    Case "CLOT_DETECTION_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.CLOT_DETECTION_ERR)
                '    Case "CLOT_DETECTION_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.CLOT_DETECTION_WARN)
                '    Case "CLOT_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.CLOT_SYSTEM_ERR)   'DL 13/06/2012

                '    Case "S_DETECT_SYSTEM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR)
                '    Case "S_NO_VOLUME_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_NO_VOLUME_WARN)
                '    Case "DS_NO_VOLUME_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.DS_NO_VOLUME_WARN)
                '    Case "S_COLLISION_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_COLLISION_ERR)
                '    Case "S_COLLISION_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_COLLISION_WARN)
                '    Case "S_OBSTRUCTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_OBSTRUCTED_ERR)

                '    Case "S_COVER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_COVER_ERR)
                '    Case "S_COVER_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_COVER_WARN)

                '    Case "R1_H_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_H_HOME_ERR)
                '    Case "R1_V_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_V_HOME_ERR)
                '    Case "R2_H_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_H_HOME_ERR)
                '    Case "R2_V_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_V_HOME_ERR)
                '    Case "S_H_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_H_HOME_ERR)
                '    Case "S_V_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_V_HOME_ERR)
                '    Case "STIRRER1_H_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER1_H_HOME_ERR)
                '    Case "STIRRER1_V_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER1_V_HOME_ERR)
                '    Case "STIRRER2_H_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER2_H_HOME_ERR)
                '    Case "STIRRER2_V_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER2_V_HOME_ERR)
                '    Case "FRIDGE_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_HOME_ERR)
                '    Case "SAMPLES_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.SAMPLES_HOME_ERR)
                '    Case "REACTIONS_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REACTIONS_HOME_ERR)
                '    Case "WS_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_HOME_ERR)
                '    Case "WS_PUMP_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_PUMP_HOME_ERR)
                '    Case "R1_PUMP_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_PUMP_HOME_ERR)
                '    Case "R2_PUMP_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_PUMP_HOME_ERR)
                '    Case "S_PUMP_HOME_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_PUMP_HOME_ERR)

                '    Case "INST_REJECTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_REJECTED_ERR)
                '    Case "INST_ABORTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_ABORTED_ERR)
                '    Case "RECOVER_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.RECOVER_ERR)
                '    Case "INST_REJECTED_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_REJECTED_WARN)
                '    Case "PH_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.PH_BOARD_ERR)
                '    Case "GLF_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.GLF_RESET_ERR)
                '    Case "SFX_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.SFX_RESET_ERR)
                '    Case "JEX_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.JEX_RESET_ERR)

                '        'DL 13/06/2012. Begin
                '    Case "R1_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_BOARD_ERR)
                '    Case "R1_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_RESET_ERR)
                '    Case "R2_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_BOARD_ERR)
                '    Case "R2_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_RESET_ERR)
                '    Case "S_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_BOARD_ERR)
                '    Case "S_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_RESET_ERR)
                '    Case "STIRRER1_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER1_BOARD_ERR)
                '    Case "STIRRER1_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER1_RESET_ERR)
                '    Case "STIRRER2_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER2_BOARD_ERR)
                '    Case "STIRRER2_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.STIRRER2_RESET_ERR)
                '    Case "R1_DETECT_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_DETECT_BOARD_ERR)
                '    Case "R1_DETECT_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R1_DETECT_RESET_ERR)
                '    Case "R2_DETECT_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_DETECT_BOARD_ERR)
                '    Case "R2_DETECT_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.R2_DETECT_RESET_ERR)
                '    Case "S_DETECT_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_DETECT_BOARD_ERR)
                '    Case "S_DETECT_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_DETECT_RESET_ERR)
                '    Case "FRIDGE_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_BOARD_ERR)
                '    Case "FRIDGE_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FRIDGE_RESET_ERR)
                '    Case "S_ROTOR_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_ROTOR_BOARD_ERR)
                '    Case "S_ROTOR_RESET_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.S_ROTOR_RESET_ERR)
                '    Case "GLF_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.GLF_BOARD_ERR)
                '    Case "SFX_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.SFX_BOARD_ERR)
                '    Case "JEX_BOARD_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.JEX_BOARD_ERR)
                '        'DL 13/06/2012. End

                '    Case "ISE_OFF_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                '    Case "ISE_FAILED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_FAILED_ERR)
                '    Case "ISE_TIMEOUT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR)

                '        ' XBC 21/03/2012
                '    Case "ISE_RP_INVALID_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR)
                '    Case "ISE_RP_NOT_INSTALL" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR)
                '    Case "ISE_RP_DEPLETED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR)
                '    Case "ISE_ELEC_WRONG_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR)
                '    Case "ISE_CP_INSTALL_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN)
                '    Case "ISE_CP_WRONG" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR)
                '    Case "ISE_LONG_DEACT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR)
                '    Case "ISE_RP_EXPIRED_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN)
                '    Case "ISE_ELEC_CONS_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN)
                '    Case "ISE_ELEC_DATE_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN)
                '    Case "ISE_ACTIVATED" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_ACTIVATED)
                '    Case "ISE_CONNECT_PDT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR) 'SGM 26/03/2012
                '        '
                '    Case "BASELINE_INIT_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                '    Case "BASELINE_WELL_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                '    Case "PREP_LOCKED_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.PREP_LOCKED_WARN)
                '    Case "COMMS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.COMMS_ERR)
                '    Case "REPORTSATLOADED_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.REPORTSATLOADED_WARN)
                '    Case "ADJUST_NO_EXIST" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.ADJUST_NO_EXIST)

                '        'BT #1355 - Added Case for new Alarm WS_PAUSE_MODE_WARN
                '    Case "WS_PAUSE_MODE_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.WS_PAUSE_MODE_WARN)

                '    Case "INST_NOALLOW_INS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_NOALLOW_INS_ERR) 'TR 21/10/2013 Bug #1339 add this alamr on User app.

                'End Select

                '' XBC 18/10/2012 - Alarms treatment for Service
                ''SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                ''If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                'If GlobalBase.IsServiceAssembly Then
                '    Select Case row
                '        Case "FW_CPU_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_CPU_ERR)
                '        Case "FW_DISTRIBUTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR)
                '        Case "FW_REPOSITORY_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_REPOSITORY_ERR)
                '        Case "FW_CHECKSUM_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_CHECKSUM_ERR)
                '        Case "FW_INTERNAL_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_INTERNAL_ERR)
                '        Case "FW_MAN_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_MAN_ERR)
                '        Case "FW_CAN_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.FW_CAN_ERR)
                '        Case "INST_UNKOWN_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_UNKOWN_ERR)
                '        Case "INST_NOALLOW_STA_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_NOALLOW_STA_ERR)
                '        Case "INST_NOALLOW_INS_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_NOALLOW_INS_ERR)
                '        Case "INST_COMMAND_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_COMMAND_WARN)
                '        Case "INST_LOADADJ_WARN" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_LOADADJ_WARN)
                '        Case "INST_REJECTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_REJECTED_ERR)
                '        Case "INST_ABORTED_ERR" : alarmIDEnumList.Add(GlobalEnumerates.Alarms.INST_ABORTED_ERR)
                '    End Select
                'End If
                '' XBC 18/10/2012 
                'AG 04/12/2014 BA-2146

            Next
            Return alarmIDEnumList
        End Function


        ''' <summary>
        ''' All alarms involving error code must be removed after receive a status instruction with E:0
        ''' All alarms involving FREEZE must be removed after recover instruction finishes
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>AG 06/03/2012
        ''' AG 27/03/2012</remarks>
        Private Function RemoveErrorCodeAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAction As GlobalEnumerates.AnalyzerManagerAx00Actions) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim stringAlarmList As New List(Of String)
                        Dim myLinq As List(Of AlarmsDS.tfmwAlarmsRow)
                        Dim okAlarmList As New List(Of GlobalEnumerates.Alarms)
                        Dim okAlarmStatusList As New List(Of Boolean)

                        'NOT FREEZE ALARMS!!!
                        'AG 27/03/2012 - all error code not freeze alarms 
                        If pAction <> AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END Then
                            'Using linq get only those that are not freeze
                            myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                      Where a.IsFreezeNull Select a).ToList
                            For Each row As AlarmsDS.tfmwAlarmsRow In myLinq
                                If Not stringAlarmList.Contains(row.AlarmID) Then stringAlarmList.Add(row.AlarmID)
                            Next

                            'Convert alarm string into internal alarm enumerate
                            Dim notFreezeErrorCodeAlarmList As New List(Of GlobalEnumerates.Alarms)
                            If stringAlarmList.Count > 0 Then
                                notFreezeErrorCodeAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)
                            End If

                            'All not freeze error code alarms in myAlarmListAttribute must be removed
                            For Each alarmID As GlobalEnumerates.Alarms In notFreezeErrorCodeAlarmList
                                PrepareLocalAlarmList(alarmID, False, okAlarmList, okAlarmStatusList)
                            Next
                        End If
                        'AG 27/03/2012
                        'NOT FREEZE ALARMS!!!

                        'FREEZE ALARMS!!!
                        'Using linq get only those that are freeze
                        myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                  Where Not a.IsFreezeNull Select a).ToList
                        For Each row As AlarmsDS.tfmwAlarmsRow In myLinq
                            If Not stringAlarmList.Contains(row.AlarmID) Then stringAlarmList.Add(row.AlarmID)
                        Next

                        'Convert alarm string into internal alarm enumerate
                        Dim freezeAlarmList As New List(Of GlobalEnumerates.Alarms)
                        If stringAlarmList.Count > 0 Then
                            freezeAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)
                        End If

                        'All freeze alarms in myAlarmListAttribute must be removed
                        For Each alarmID As GlobalEnumerates.Alarms In freezeAlarmList
                            PrepareLocalAlarmList(alarmID, False, okAlarmList, okAlarmStatusList)
                        Next

                        'Update internal flags
                        analyzerFREEZEFlagAttribute = False
                        analyzerFREEZEModeAttribute = ""
                        AnalyzerIsReadyAttribute = True 'Analyzer is ready
                        'FREEZE ALARMS!!!

                        'TREATMENT!!!
                        If okAlarmList.Count > 0 Then
                            '3- Finally call manage all alarms detected (new or fixed)
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                ' XBC 16/10/2012 - Alarms treatment for Service
                                ' Not Apply
                                'resultData = ManageAlarms_SRV(dbConnection, okAlarmList, okAlarmStatusList)
                            Else
                                resultData = ManageAlarms(dbConnection, okAlarmList, okAlarmStatusList)
                            End If
                        End If

                        If pAction <> AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START Then
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 0, True) 'AG 06/03/2012
                        End If

                        myLinq = Nothing 'AG 02/08/2012 - free memory
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveErrorCodeAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' If some of the alarms in pAlarmList means analyzer FREEZE then return TRUE
        ''' If none of the alarms in pAlarmList means analyzer FREEZE then return FALSE
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAlarmList"></param>
        ''' <returns></returns>
        ''' <remarks>AG 07/03/2012</remarks>
        Private Function ExistFreezeAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmList As List(Of GlobalEnumerates.Alarms)) As Boolean
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim returnData As Boolean = False

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
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
                        myLinq = Nothing

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ExistFreezeAlarms", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return returnData
        End Function

#End Region

#Region "Ignore alarms cases"
        ''' <summary>
        ''' Inform if the error code must be consider or not
        ''' When Fw send status instruction with error codes they must be treated or not depending
        ''' the last instruction sent by Software
        ''' </summary>
        ''' <param name="pLastInstructionTypeSent"></param>
        ''' <returns>Booelan</returns>
        ''' <remarks>
        ''' Created by AG 27/03/2012
        ''' Modifiedby XB 04/05/2012 - add pErrorValue and funtionality related to depurate errors ignores
        '''            TR 19/09/2012 -Add the STATE alarms on validation and POLLSN ( belong to Xavi Badia).
        '''            AG 25/09/2012 - Add the POLLRD instruction
        ''' </remarks>
        Private Function IgnoreErrorCodes(ByVal pLastInstructionTypeSent As GlobalEnumerates.AppLayerEventList, ByVal pInstructionSent As String, ByVal pErrorValue As Integer) As Boolean
            Dim ignoreFlag As Boolean = False 'Default value FALSE
            Try
                ' XBC 18/10/2012 - Alarms treatment for Service
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ' Service Software no ignore errors
                    If IsServiceAlarmInformed Then
                        ignoreFlag = True
                    ElseIf MyClass.IsFwUpdateInProcess Then
                        'in case that FW update is currently being performed, ignore alarms.
                        ignoreFlag = True
                    ElseIf MyClass.IsConfigGeneralProcess Then
                        'in case user is processing general configurations, ignore corresponding alarms.
                        Dim myAlarms As New List(Of GlobalEnumerates.Alarms)
                        Dim myErrorCode As New List(Of Integer)
                        myErrorCode.Add(pErrorValue)
                        myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)
                        For Each alarmID As GlobalEnumerates.Alarms In myAlarms
                            If alarmID = GlobalEnumerates.Alarms.MAIN_COVER_ERR Or _
                               alarmID = GlobalEnumerates.Alarms.MAIN_COVER_WARN Or _
                               alarmID = GlobalEnumerates.Alarms.FRIDGE_COVER_ERR Or _
                               alarmID = GlobalEnumerates.Alarms.FRIDGE_COVER_WARN Or _
                               alarmID = GlobalEnumerates.Alarms.S_COVER_ERR Or _
                               alarmID = GlobalEnumerates.Alarms.S_COVER_WARN Or _
                               alarmID = GlobalEnumerates.Alarms.REACT_COVER_ERR Or _
                               alarmID = GlobalEnumerates.Alarms.REACT_COVER_WARN Or _
                               alarmID = GlobalEnumerates.Alarms.CLOT_SYSTEM_ERR Then
                                ignoreFlag = True
                            End If
                        Next
                    End If

                    If ignoreFlag Then Exit Try

                    ' These instructions are no ignored by Service Sw
                    Select Case pLastInstructionTypeSent
                        Case AppLayerEventList.LOADADJ, AppLayerEventList.ISE_CMD, AppLayerEventList.CONNECT
                            Exit Try
                    End Select
                End If

                Select Case pLastInstructionTypeSent

                    Case AppLayerEventList.CONNECT, AppLayerEventList.CONFIG, AppLayerEventList.READADJ, AppLayerEventList.INFO, AppLayerEventList.SOUND, _
                        AppLayerEventList.POLLFW, AppLayerEventList.POLLHW, AppLayerEventList.LOADADJ, AppLayerEventList.STATE, AppLayerEventList.POLLSN, AppLayerEventList.POLLRD

                        'AG-XB 17/09/2012 - Ignore STATEs in connection process.
                        If pLastInstructionTypeSent = AppLayerEventList.STATE Then
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                                ignoreFlag = True
                            End If
                        Else
                            'Always ignore error code
                            ignoreFlag = True
                        End If



                    Case AppLayerEventList.BARCODE_REQUEST
                        'Ignore error code only when barcode request in configuration mode
                        If Not pInstructionSent Is Nothing Then
                            If InStr(pInstructionSent, "A:1;") > 0 Then
                                ignoreFlag = True
                            End If
                        End If


                    Case AppLayerEventList.ISE_CMD
                        'AG 30/03/2012
                        'Ignore error code only when ISE cmd request in low level control mode
                        If Not pInstructionSent Is Nothing Then
                            'The following code requires new Fw not available in 30/03/2012

                            ' XBC 03/09/2012 - Correction : add new ISE instructions M:4 and M:5
                            'If InStr(pInstructionSent, "M:1;") > 0 Then
                            If InStr(pInstructionSent, "M:1;") > 0 Or _
                               InStr(pInstructionSent, "M:4;") > 0 Or _
                               InStr(pInstructionSent, "M:5;") > 0 Then
                                ' XBC 03/09/2012 

                                ignoreFlag = True

                                ' XB 04/05/2012
                                Dim myAlarms As New List(Of GlobalEnumerates.Alarms)
                                Dim myErrorCode As New List(Of Integer)
                                myErrorCode.Add(pErrorValue)
                                myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)
                                For Each alarmID As GlobalEnumerates.Alarms In myAlarms
                                    If alarmID = GlobalEnumerates.Alarms.ISE_FAILED_ERR Or _
                                       alarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                                        ignoreFlag = False
                                    End If
                                Next
                                ' XB 04/05/2012

                            End If

                            ''Temporally code: never ignore error codes with ISECMD
                            'ignoreFlag = False

                            'Abort current ise procedure in ise_manager class
                            If Not ignoreFlag Then

                                'SGM 25/10/2012 - Abort ISE Operation only if E:61, E:20, E:21
                                If pErrorValue = 61 Or pErrorValue = 20 Or pErrorValue = 21 Then
                                    If ISEAnalyzer IsNot Nothing Then ISEAnalyzer.AbortCurrentProcedureDueToException()
                                End If
                                'If ISEAnalyzer IsNot Nothing Then ISEAnalyzer.AbortCurrentProcedureDueToException() SGM 25/10/2012
                            End If
                        End If
                        'AG 30/03/2012

                End Select


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreErrorCodes", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function


        ''' <summary>
        ''' Most of alarms must be ignored while the start instrument process is in process
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 27/03/2012</remarks>
        Private Function IgnoreAlarmWhileWarmUp(ByVal pAlarmID As GlobalEnumerates.Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                'AG 22/05/2012 - Note the ise alarms will be asked for when the Terminate button is activated
                If pAlarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_WARN OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WATER_SYSTEM_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.BASELINE_INIT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.REACT_MISSING_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.FRIDGE_COVER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.S_COVER_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.MAIN_COVER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.REACT_COVER_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_A OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_B OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_C OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_D OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_F OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_M OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_N OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_P OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_R OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_S OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_T OrElse pAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_W Then
                    ignoreFlag = False
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileWarmUp", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

        ''' <summary>
        ''' Most of alarms must be ignored while the communications is not established
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 27/03/2012</remarks>
        Private Function IgnoreAlarmWhileCommError(ByVal pAlarmID As GlobalEnumerates.Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                'AG 16/05/2012 - When no connection only treat the informative alarm reportsat loaded
                'If pAlarmID = GlobalEnumerates.Alarms.BASELINE_INIT_ERR OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.REPORTSATLOADED_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR OrElse _
                '   pAlarmID = GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR Then
                '    ignoreFlag = False
                'End If
                If pAlarmID = GlobalEnumerates.Alarms.REPORTSATLOADED_WARN Then
                    ignoreFlag = False
                End If
                'AG 16/05/2012

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileCommError", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

        ''' <summary>
        ''' Most of alarms must be ignored while the shut down instrument process is in process
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 18/05/2012</remarks>
        Private Function IgnoreAlarmWhileShutDown(ByVal pAlarmID As GlobalEnumerates.Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                If pAlarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WASH_CONTAINER_WARN OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WATER_SYSTEM_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.REACT_MISSING_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.FRIDGE_COVER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.S_COVER_ERR OrElse _
                  pAlarmID = GlobalEnumerates.Alarms.MAIN_COVER_ERR OrElse pAlarmID = GlobalEnumerates.Alarms.REACT_COVER_ERR Then
                    ignoreFlag = False
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileShutDown", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function


#End Region

#Region "Alarm timers methods"

        ''' <summary>
        ''' If the reactions rotor thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2012
        ''' </remarks>
        Private Sub ThermoReactionsRotorError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoReactionsRotorWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Change warning to error
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.REACT_TEMP_WARN, False, myAlarmList, myAlarmStatusList)
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.REACT_TEMP_ERR, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ThermoReactionsRotorError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' If the fridge thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub ThermoFridgeError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoFridgeWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Change warning for error
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN, False, myAlarmList, myAlarmStatusList)
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ThermoFridgeError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' If the waste deposit warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>AG 01/12/2011</remarks>
        Private Sub WasteDepositError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            'Fw implements the timer!!
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                wasteDepositTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Prepare internal alarm structures and variables
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR, True, myAlarmList, myAlarmStatusList)

                'Manage the new alarm
                If myAlarmList.Count > 0 Then
                    'AG 29/05/2014 - #1630
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Waste deposit full too much time!! Generate alarm WASTE_DEPOSIT_ERR", "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Information, False)

                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.WasteDepositError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' If the water deposit warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>AG 01/12/2011</remarks>
        Private Sub WaterDepositError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            'Fw implements the timer!!
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                waterDepositTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Prepare internal alarm structures and variables
                PrepareLocalAlarmList(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR, True, myAlarmList, myAlarmStatusList)

                'Manage the new alarm
                If myAlarmList.Count > 0 Then
                    'AG 29/05/2014 - #1630
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Water deposit empty too much time!! Generate alarm WATER_DEPOSIT_ERR", "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Information, False)

                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the R1 arm thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub thermoR1ArmWarningTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoR1ArmWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                PrepareLocalAlarmList(GlobalEnumerates.Alarms.R1_TEMP_WARN, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.thermoR1ArmWarningTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the R2 arm thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub thermoR2ArmWarningTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoR2ArmWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                PrepareLocalAlarmList(GlobalEnumerates.Alarms.R2_TEMP_WARN, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.thermoR2ArmWarningTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region


    End Class

End Namespace




