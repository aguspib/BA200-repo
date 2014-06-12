Option Explicit On
Option Strict On

'Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.Calculations
'Imports Biosystems.Ax00.InfoAnalyzer
'Imports System.Timers
Imports System.Data
Imports System.Windows.Forms
'Imports System.ComponentModel 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports System.Globalization    ' XBC 29/01/2013 - change IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)

Namespace Biosystems.Ax00.CommunicationsSwFw

    Partial Public Class AnalyzerManager

#Region "Private Reception Methods"

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
        ''' Modified by: RH 30/06/2010
        '''              AnalyzerIsReady depends on myRequestValue.
        '''              Change "If myRequestValue = 1" by "If AnalyzerIsReady"
        ''' AG 20/06/2012 - after connection establishment Sw has to wait until analyzer becomes ready to receive new instructions (Action = Ready)
        ''' AG 19/11/2013 - call the method to process the last ANSPHR instruction received also in pause mode while barcode is scanning
        ''' AG 28/11/2013 - #1397 changes into connection process in running (they are required for the recovery results in pause mode: after 1st STATUS after connection call STATE + POLLSN)
        ''' AG 06/02/2014 - Fix issues numbers #1484
        ''' XB 03/04/2014 - Fix a malfunction when INFO;Q:3 was sent meanwhile a ISE operation was working or Abort process was not finished - task #1557
        ''' AG 15/04/2014 - #1594 paused in v300
        ''' </remarks>
        Private Function ProcessStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                ' Get Status field (parameter index 3)

                ' AG+XBC 24/05/2012
                'Dim myStatusValue As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                Dim myStatusValue As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
                ' AG+XBC 24/05/2012

                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myStatusValue = DirectCast(CInt(myInstParamTO.ParameterValue), GlobalEnumerates.AnalyzerManagerStatus)
                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And myStatusValue = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        ExecuteSpecialBusinessOnAnalyzerStatusChanges(Nothing, myStatusValue) 'AG 29/06/2011
                    End If

                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then GlobalConstants.AnalyzerIsRunningFlag = True Else GlobalConstants.AnalyzerIsRunningFlag = False 'AG + DL 24/01/2012

                    'AnalyzerStatusAttribute = myStatusValue 'AG 01/06/2010 - Inform the class attribute value
                    If AnalyzerStatusAttribute <> myStatusValue Then
                        AnalyzerStatusAttribute = myStatusValue 'AG 01/06/2010 - Inform the class attribute value
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED, 1, True) 'Prepare UI refresh event when analyzer status changes
                    End If

                End If

                ' Get Action field (parameter index 4)
                Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myActionValue = DirectCast(CInt(myInstParamTO.ParameterValue), GlobalEnumerates.AnalyzerManagerAx00Actions)
                    AnalyzerCurrentActionAttribute = myActionValue 'AG 01/06/2010 - Inform the class attribute value

                    'Evaluate the Action code value (in all analyzer status)
                    If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.SOUND_DONE Then 'AG 26/10/2011 - Sound Alarm
                        'Change the status of AnalyzerIsRingingAttribute (TRUE --> FALSE, FALSE --> TRUE)
                        'Except when connection ... always FALSE in this case
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                            AnalyzerIsRingingAttribute = False
                        Else
                            AnalyzerIsRingingAttribute = Not AnalyzerIsRingingAttribute
                        End If
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ANALYZER_SOUND_CHANGED, CSng(IIf(AnalyzerIsRingingAttribute, 1, 0)), True)

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START Then 'AG 05/12/2011 - Remember the ENDRUN instruction has been sent and do not send it again
                        endRunAlreadySentFlagAttribute = True

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_END Then
                        endRunAlreadySentFlagAttribute = False

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START Then 'AG 16/12/2011 - Remember the ABORT instruction has been sent and do not send it again
                        abortAlreadySentFlagAttribute = True
                        endRunAlreadySentFlagAttribute = True 'AG 10/12/2012

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_END Then
                        PauseAlreadySentFlagAttribute = False ' XB 15/10/2013 - BT #1318

                        'PAUSED (comment these 2 lines) - AG 20/03/2014 - #1547 do not reset these flags here, do it once the StanBy instruction is sent and accepted!!!
                        abortAlreadySentFlagAttribute = False
                        endRunAlreadySentFlagAttribute = False 'AG 10/12/2012


                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_START Then ' XB 15/10/2013 - Remember the PAUSE instruction has been sent and do not send it again - BT #1318
                        If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString), "", False) <> 0 Then 'TR 21/10/2013 -Bug #1339
                            PauseAlreadySentFlagAttribute = True
                        End If


                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_END Then ' XB 15/10/2013 - BT #1318
                        PauseAlreadySentFlagAttribute = False

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START Then
                        recoverAlreadySentFlagAttribute = True

                    ElseIf myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END Then
                        recoverAlreadySentFlagAttribute = False
                        myGlobal = RemoveErrorCodeAlarms(Nothing, myActionValue)

                        'SGM 08/11/2012 for updating monitor refresh
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            InfoRefreshFirstTime = True
                            'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True) 'SGM 10/05/2011 - 1 True, 0 False
                        End If

                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED, 1, True) 'Inform the recover instruction has finished

                    End If


                    'SGM 7/06/2012 ISE Start Timeout Managing
                    If myActionValue = AnalyzerManagerAx00Actions.ISE_ACTION_START Then
                        'If MyClass.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
                        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Action Start =34")
                        MyClass.ISE_Manager.StopInstructionStartedTimer()
                        'End If
                    End If
                    'SGM 7/06/2012

                    ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.COMMAND_START Or _
                           myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.COMMAND_END Then

                            MyClass.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                            MyClass.ClearStartTaskQueueToSend()

                        End If

                    End If
                    ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks

                End If

                'AG 25/10/2010 - Get Time field (parameter index 5)
                Dim myExpectedTime As Integer = WAITING_TIME_OFF 'If no request .... get the estimated time and activate the waiting timer
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myExpectedTime = CInt(myInstParamTO.ParameterValue)
                    'AnalyzerIsReady = (myExpectedTimeValue = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
                Else
                    Exit Try
                End If
                'END AG 25/10/2010

                Dim myExpectedTimeRaw As Integer = myExpectedTime  ' XB 21/10/2013 - BT #1334

                ' Get Well field (parameter index 7)
                Dim myWellValue As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myWellValue = CInt(myInstParamTO.ParameterValue)
                    CurrentWellAttribute = myWellValue 'Inform the class attribute
                Else
                    Exit Try
                End If

                ' Get Request field (parameter index 8)
                Dim myRequestValue As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myRequestValue = CInt(myInstParamTO.ParameterValue)
                Else
                    Exit Try
                End If

                'AG 25/10/2010 - Depending the status and action use Request or not
                useRequestFlag = False
                If myStatusValue = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    'AG 16/04/2014 - #1594 add SOUND DONE to the action codes that will use R in order to evaluate if analyzer is ready or not!! - paused in v300
                    'AnalyzerIsReadyAttribute = (myRequestValue = 1) '1 Ax00 is ready to work, 0 Ax00 is busy (for use only in RUNNING mode)
                    If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.TEST_PREPARATION_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.PREDILUTED_TEST_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.ISE_TEST_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.SKIP_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.WASHING_RUN_END Or _
                       myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.LOADADJ_END Then

                        useRequestFlag = True
                    End If
                End If

                If Not useRequestFlag Then
                    ''AG 11/12/2012 not integrated in v1.0.0 - RPM: STANDBY ini in running have T:0 but it can NOT be used to set AnalyzerISReady to TRUE
                    ''                     It is the only ini action with T:0
                    ''                     Sw: has to treat this exception
                    'If myActionValue = AnalyzerManagerAx00Actions.STANDBY_START AndAlso myStatusValue = AnalyzerManagerStatus.RUNNING _
                    'AndAlso myExpectedTime = 0 Then
                    '    myExpectedTime = WAITING_TIME_DEFAULT
                    'End If
                    ''AG 11/12/2012 
                    AnalyzerIsReadyAttribute = (myExpectedTime = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
                Else
                    AnalyzerIsReadyAttribute = (myRequestValue = 1) '1 Ax00 is ready to work, 0 Ax00 is busy (for use only in RUNNING mode)
                End If

                'AG 03/09/2012 - If connection analyzer always ready to receive new instructions
                If AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE Then
                    AnalyzerIsReadyAttribute = True
                End If
                'AG 03/09/2012

                If Not AnalyzerIsReadyAttribute Then
                    If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT

                    'AG 13/02/2012 - In Running activate always the waiting time
                ElseIf AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    'AG 21/03/2012 - exception when the abort instruction has started. In this case take into account the TIME received
                    'myExpectedTime = WAITING_TIME_DEFAULT
                    If myActionValue <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START Then
                        myExpectedTime = WAITING_TIME_DEFAULT
                    End If
                    'AG 21/03/2012 /'AG 13/02/2012/

                End If
                Me.InitializeTimerControl(myExpectedTime)
                AppLayer.MaxWaitTime = myExpectedTime + SYSTEM_TIME_OFFSET   ' XBC 04/05/2011
                'END AG 25/10/2010

                ' Get Error field (parameter index 9)
                Dim errorValue As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    errorValue = CInt(myInstParamTO.ParameterValue)
                Else
                    Exit Try
                End If

                'AG 23/11/2011 - Get ISE field (parameter index 10) also ISEModuleIsReadyAttribute is updated using the Fw information send
                Dim ISEAvailableValue As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    ISEAvailableValue = CInt(myInstParamTO.ParameterValue)
                    ISEModuleIsReadyAttribute = CType(IIf(ISEAvailableValue = 1, 1, 0), Boolean) 'ISEAvailableValue = 0 means ISEModuleIsReadyAttribute = True, ISEAvailableValue = 1 means ISEModuleIsReadyAttribute = False (is working)
                Else
                    Exit Try
                End If
                'AG 23/11/2011

                'SGM 29/10/2012 - SERVICE: reset E:20 flag
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    MyClass.IsInstructionRejected = False
                    MyClass.IsRecoverFailed = False 'SGM 07/11/2012
                    MyClass.IsInstructionAborted = False 'SGM 19/11/2012
                End If

                If errorValue <> 0 Then

                    If Not IgnoreErrorCodes(AppLayer.LastInstructionTypeSent, AppLayer.InstructionSent, errorValue) Then
                        If errorValue = MULTIPLE_ERROR_CODE Then 'If multiple alarm error code ask for details
                            'AG 14/03/2011: Ask for errors details (INFO;Q:ALR)

                            ' XB+SG 26/10/2012 - Is no need because 99 is not displayed, it is waiting for next ANSERR answer
                            'SGM+XBC 24/10/2012 - force Analyzer Ready
                            'If myApplicationName.ToUpper.Contains("SERVICE") Then
                            ' SERVICE SW
                            '' Update Alarm sensors to inform to Presentation layer what kind of management is need to display
                            'If Not myUI_RefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.MULTIPLE_ERROR_CODE) Then
                            '    PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, MULTIPLE_ERROR_CODE.ToString, True)
                            '    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, ManagementAlarmTypes.REQUEST_INFO, True)
                            'End If
                            'End If
                            ' XB+SG 26/10/2012


                            'Debug.Print(" ERROR 99 Received !")

                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR)
                            '' XB 30/04/2014 - PENDING TO IMPLEMENT !!!!!!!!!!!! + REMEMBER COMMENT THE PREVIOUS LINE !!! - Task #1615

                            'Dim updateISEConsumptionFlag As Boolean = False
                            '' Estimated ISE Consumption by Firmware during WS
                            'If Not MyClass.ISE_Manager Is Nothing _
                            '   AndAlso MyClass.ISE_Manager.IsISEModuleInstalled Then
                            '    MyClass.ISE_Manager.EstimatedFWConsumptionWS()
                            '    ' Update ISE consumptions if required
                            '    If MyClass.ISE_Manager.IsCalAUpdateRequired Or MyClass.ISE_Manager.IsCalBUpdateRequired Then
                            '        updateISEConsumptionFlag = True
                            '    End If
                            'End If

                            'If Not updateISEConsumptionFlag Then
                            '    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.ALR)
                            'End If
                            '' XB 30/04/2014 - PENDING TO IMPLEMENT !!!!!!!!!!!! - Task #1615


                            'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                            If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                SetAnalyzerNotReady()
                            End If


                        Else 'If single error code then treat it!!

                            'Translation method
                            Dim myAlarmsReceivedList As New List(Of GlobalEnumerates.Alarms)
                            Dim myAlarmsStatusList As New List(Of Boolean)
                            Dim myAlarms As New List(Of GlobalEnumerates.Alarms)
                            Dim myErrorCode As New List(Of Integer)
                            ' XBC 16/10/2012
                            Dim myFwCodeErrorReceivedList As New List(Of String)

                            myErrorCode.Add(errorValue)

                            myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)

                            'SGM 09/11/2012 - reset flag in case of Rotor missing error is not received
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                If Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) Then
                                    MyClass.IsServiceRotorMissingInformed = False
                                End If
                            End If

                            'SGM 02/07/2012
                            If myAlarms.Contains(GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR) Then
                                If Not myAlarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                                    myAlarms.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)

                                    'SGM 18/09/2012
                                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                    If GlobalBase.IsServiceAssembly Then
                                        MyClass.ISE_Manager.IsISESwitchON = False
                                    End If

                                End If
                                myAlarms.Remove(GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR)
                            End If
                            'end SGM 02/07/2012

                            For Each alarmID As GlobalEnumerates.Alarms In myAlarms
                                PrepareLocalAlarmList(alarmID, True, myAlarmsReceivedList, myAlarmsStatusList, "", Nothing, True) 'AG 13/04/2012 - last parameter (optional) must be true for the error code alarms
                            Next

                            ' XBC 16/10/2012 - Alarms treatment for Service
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                ' Initialize Error Codes List
                                myErrorCodesAttribute.Clear()
                                ' Prepare error codes List received from Analyzer
                                PrepareLocalAlarmList_SRV(myErrorCode, myFwCodeErrorReceivedList)
                            End If
                            ' XBC 16/10/2012

                            If myAlarmsReceivedList.Count > 0 Then
                                '3- Finally call manage all alarms detected (new or fixed)
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    ' XBC 16/10/2012 - Alarms treatment for Service
                                    'myGlobal = ManageAlarms_SRV(Nothing, myAlarmsReceivedList, myAlarmsStatusList)
                                    myGlobal = ManageAlarms_SRV(Nothing, myAlarmsReceivedList, myAlarmsStatusList, myFwCodeErrorReceivedList)
                                    ' XBC 16/10/2012
                                Else
                                    myGlobal = ManageAlarms(Nothing, myAlarmsReceivedList, myAlarmsStatusList)
                                End If

                            Else 'if not new alarms sure the ansinfo instruction is activated
                                'AG 31/08/2012 - condition incomplete. When Analyzer is enter in running sw has not to sent the INFO STR instruction 
                                'If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess.ToString) <> "INPROCESS" Then

                                    ' XB 03/04/2014
                                    Dim updateISEConsumptionFlag As Boolean = False
                                    ' Estimated ISE Consumption by Firmware during WS
                                    If Not MyClass.ISE_Manager Is Nothing _
                                       AndAlso MyClass.ISE_Manager.IsISEModuleInstalled Then
                                        MyClass.ISE_Manager.EstimatedFWConsumptionWS()
                                        ' Update ISE consumptions if required
                                        If MyClass.ISE_Manager.IsCalAUpdateRequired Or MyClass.ISE_Manager.IsCalBUpdateRequired Then
                                            updateISEConsumptionFlag = True
                                        End If
                                    End If

                                    'AG 12/04/2012 - New Fw disables info when analyzer leaves running, so Sw has to activate info when standby end
                                    If Not updateISEConsumptionFlag AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) <> "INPROCESS" Then
                                        ' XB 03/04/2014

                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                                        'AG 04/04/2012 - When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then SetAnalyzerNotReady()

                                    End If ' XB 03/04/2014

                                End If
                            End If

                        End If

                    End If

                Else 'Error code = 0

                    'AG 19/11/2013 - #1396-b comment this code, out of date: XB 21/10/2013 - BT #1334
                    'If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED And _
                    '   myExpectedTimeRaw <= 0 Then
                    '    ' Ignore this instruction when is Barcode action started and Expected time <= 0 
                    '    ' because Fw is informing that that actions is still being executed
                    '    Exit Try
                    'End If

                    ' XB 21/11/2013 - #1399 comment this code, out of date: XB 23/10/2013 - BT #1343
                    'If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.ISE_ACTION_START And _
                    '   myExpectedTimeRaw <= 0 Then
                    '    ' Ignore this instruction when is Barcode action started and Expected time <= 0 
                    '    ' because Fw is informing that that actions is still being executed
                    '    Exit Try
                    'End If

                    ' XBC 07/11/2012
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        If myErrorCodesDisplayAttribute.Count > 0 Then
                            Dim pErrorCodeList As New List(Of String)
                            pErrorCodeList.Add("0")
                            ' Solve all previous alarms
                            SolveErrorCodesToDisplay(pErrorCodeList)
                        End If

                        'SGM 15/11/2012 - Initialize Error Codes List
                        myErrorCodesAttribute.Clear()

                    End If

                    'Reset the freeze flags information
                    If analyzerFREEZEFlagAttribute Then
                        'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                        'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If Not GlobalBase.IsServiceAssembly Then
                            'Clear all alarms with error code
                            myGlobal = RemoveErrorCodeAlarms(Nothing, AnalyzerCurrentActionAttribute)
                        End If
                    End If
                End If


                '-------------------------------------------------------------------------------------                
                'Finally:
                'Do business depending the requestvalue, action value, status value, alarms value,....
                '-------------------------------------------------------------------------------------                
                'If action say us CONNECTION_ESTABLISHMENT update internal flags
                If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE Then

                    'force to STANDBY_END when connection stablished SGM 14/11/2011
                    'If myApplicationName.ToUpper.Contains("SERVICE") Then SGM 22/10/2012
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly AndAlso errorValue = 0 Then
                        MyClass.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED
                        RaiseEvent ReceptionEvent(CInt(GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE).ToString, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                        If Not MyClass.IsFwUpdateInProcess Then MyClass.InfoRefreshFirstTime = True 'SGM 16/11/2012
                    End If

                    'When successfully inform the connection results
                    ConnectedAttribute = True
                    PortNameAttribute = AppLayer.ConnectedPortName
                    BaudsAttribute = AppLayer.ConnectedBauds

                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(ConnectedAttribute), True) 'AG 28/09/2011 - comment this line. The event will be generated when the connect process ends
                End If

                'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If Not GlobalBase.IsServiceAssembly Then 'NOT for SERVICE SOFTWARE 01/07/2011
                    'AG 29/06/2011 - comment this IF (AG 03/11/2010)
                    'If AnalyzerIsReadyAttribute = True Then
                    Select Case myStatusValue
                        Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                            'AG 20/06/2012
                            'myGlobal = Me.ManageSleepStatus(myActionValue)
                            Dim resetFlags As Boolean = True
                            If Not mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                               Not AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                                myGlobal = Me.ManageSleepStatus(myActionValue)
                            End If

                            If myActionValue = AnalyzerManagerAx00Actions.STANDBY_START OrElse myActionValue = AnalyzerManagerAx00Actions.SLEEP_END _
                            OrElse mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady.ToString) = "INI" Then
                                resetFlags = False
                            End If

                            If resetFlags AndAlso Not myGlobal.HasError Then
                                'reset internal flags when analyzer is sleeping and no action has been performed
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                myGlobal = myFlagsDelg.ResetFlags(Nothing, AnalyzerIDAttribute)
                                InitializeAnalyzerFlags(Nothing)
                            End If
                            'AG 20/06/2012

                        Case GlobalEnumerates.AnalyzerManagerStatus.STANDBY
                            If Not mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                               Not AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'AG 14/06/2012 - Do not manage instruction by analyzer status if connection process is in course (STANDBY)
                                myGlobal = Me.ManageStandByStatus(myActionValue, myWellValue)
                            End If

                        Case GlobalEnumerates.AnalyzerManagerStatus.RUNNING
                            'PAUSED (uncomment these 4 lines) AG 20/03/2014 - #1547 Once the Standby instruction has been accepted reset these two flags
                            'If myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_START Then
                            '    abortAlreadySentFlagAttribute = False
                            '    endRunAlreadySentFlagAttribute = False
                            'End If
                            'AG 20/03/2014 - #1547

                            If Not mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                               Not AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE Then 'AG 14/06/2012 - Do not manage instruction by analyzer status if connection process is in course (RUNNING)
                                myGlobal = Me.ManageRunningStatus(myActionValue, myWellValue)

                                If myRequestValue = 1 Then StartTime = Now 'AG 28/06/2012 - time estimation

                            End If
                    End Select


                    'AG 22/06/2012 - (status <> RUNNING) After connection establishment Sw has to wait until BAx00 is ready to receive the remaining instructions of the connection process
                    '                (status = RUNNING) Sound OFF + no more changes (until spec how to know the analyzerID)
                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                    'IF clause (priority 1)
                    If AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONNECTION_DONE AndAlso _
                        mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then

                        'AG 20/06/2012 - Different connect business when analyzer status <> Running and when is running
                        If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                            ' XBC 02/08/2012 - initialize ReportSATonRUNNING
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ReportSATonRUNNING, "CLOSED")

                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Ask for status (and waits unitl analyzer becomes ready)
                        Else
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                            ' XBC 02/08/2012 - when Connecting on RUNNING Session we must ask for Serial Number (POLLSN) 
                            'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True) 'Send SOUND OFF instruction

                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess, "INPROCESS")
                            endRunAlreadySentFlagAttribute = True 'AG 25/09/2012 - When resultsrecover process starts the Fw implements an automatic END, so add protection in order Sw do not sent this instruction again

                            'AG 28/11/2013 - BT #1397 (in running Sw has to know if analyzer is in normal or paused running before call POLLSN so we has to send STATE instruction)
                            'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLSN, True) 'Send POLLSN instruction
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True) 'Send STATE instruction

                            ' XBC 02/08/2012 

                        End If
                        'AG 20/06/2012

                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                            SetAnalyzerNotReady()
                        End If

                        'IF clause (priority 2)
                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                           mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady.ToString) = "INI" Then

                        'SGM 21/06/2012 - Protection!! INFO will be activated later, on POLLFW answer reception
                        If myStatusValue = AnalyzerManagerStatus.STANDBY Then
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                            System.Threading.Thread.Sleep(1000)
                        End If

                        'AG 14/09/2012 - Final code apply Fw >= 0.7.1
                        'AG 21/06/2012 - Leave commented until new Fw returns action = 0 when ready (now it remembers the last action process)
                        If AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "END")

                            'Continue with the connection process instructions flow
                            ' XBC 05/06/2012 - include POLLFW request before READADJ
                            'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, GlobalEnumerates.Ax00Adjustsments.ALL)
                            If myStatusValue <> AnalyzerManagerStatus.RUNNING Then
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLFW, True, Nothing, GlobalEnumerates.POLL_IDs.CPU)
                            End If
                            ' XBC 05/06/2012

                        Else
                            If myExpectedTime <= 0 Then myExpectedTime = WAITING_TIME_DEFAULT
                            Me.InitializeTimerControl(myExpectedTime)
                            AppLayer.MaxWaitTime = myExpectedTime + SYSTEM_TIME_OFFSET   ' XBC 04/05/2011
                        End If

                        'AG 14/09/2012 - Comment temporal code apply Fw < 0.7.1
                        ''To remove when new Fw installed (26/06/2012)
                        'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "END")
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLFW, True, Nothing, GlobalEnumerates.POLL_IDs.CPU)
                        'AG 21/06/2012

                    ElseIf AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.CONFIG_DONE AndAlso _
                            mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                        'Send SOUND OFF instruction
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True)
                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                            SetAnalyzerNotReady()
                        End If

                    ElseIf AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.SOUND_DONE AndAlso _
                            mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "CLOSED")
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True) 'Inform connection finished OK

                        'AG 04/09/2012 - when recovery results finished, Sw call the processConnection method for execute the full connection
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")
                            AppLayer.RecoveryResultsInPause = False 'AG 28/11/2013 - BT #1397
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS, 0, True) 'Generate UI refresh for presentation - Inform the recovery results has finished!!
                            ManageStandByStatus(AnalyzerManagerAx00Actions.STANDBY_END, CurrentWellAttribute) 'Call this method with this action code to update ISE consumption if needed

                            'AG 17/09/2012 - Activate final code for v052. Temporally commented for setup 051
                            'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                            '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")
                            '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "INPROCESS")
                            '    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True)
                            '    Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                            '    myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")


                            'AG 25/09/2012 - Once the connection process after recovery results has finished. If analyzer is freeze then send the SOUND ON
                            'We have to do it now because the normal connection finishes is when action SOUND_DONE is received
                            If analyzerFREEZEFlagAttribute Then
                                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                    SetAnalyzerNotReady()
                                End If
                            End If

                        End If
                        'AG 04/09/2012

                        'AG 18/01/2012 - If no ISE installed LOCK all pending executions in current worksession/analyzer
                        If Not myGlobal.HasError Then
                            Dim adjustValue As String = ""
                            Dim iseInstalledFlag As Boolean = False
                            adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
                            If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                                iseInstalledFlag = CType(adjustValue, Boolean)
                                If Not iseInstalledFlag Then
                                    Dim myExecutions As New ExecutionsDelegate
                                    myGlobal = myExecutions.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                                End If
                            End If
                        End If

                        'AG 29/03/2012 - activate info in standby
                        'SGM 13/03/2012
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                        If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                            AnalyzerIsInfoActivatedAttribute = 0
                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                        End If
                        'AG 29/03/2012

                        myGlobal = RecoverStableSetup(Nothing) 'AG 20/06/2012 - Evaluate value for FLAGS and determine the next action to be sent in order to achieve a stable setup

                        'AG 28/11/2013 - BT #1397
                    ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso Not runningConnectionPollSnSent AndAlso _
                        mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLSN, True) 'Send POLLSN instruction
                        'AG 28/11/2013

                    End If

                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                    'AG 23/11/2011

                End If


                If myStatusValue = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then

                    ' XBC 24/05/2012
                    If MyClass.ISE_Manager IsNot Nothing AndAlso MyClass.ISE_Manager.IsISEModuleInstalled Then
                        MyClass.ISE_Manager.IsISEInitiatedDone = False
                        ISEAlreadyStarted = False
                    End If
                    ' XBC 24/05/2012

                    'SGM 23/09/2011
                    If IsShutDownRequested Then
                        ConnectedAttribute = False
                        InfoRefreshFirstTime = True
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True) 'SGM 10/05/2011 - 1 True, 0 False
                    End If
                End If

                'AG 28/06/2012 - Launch the parallel threat when Running and Request received and preparation instruction is already sent
                'Also during running initialization

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    SyncLock lockThis
                        If bufferANSPHRReceived.Count > 0 AndAlso Not processingLastANSPHRInstructionFlag Then
                            'If useRequestFlag AndAlso myRequestValue = 1 Then ' Request received and instruction already sent
                            '    processingLastANSPHRInstructionFlag = True
                            '    wellBaseLineWorker.RunWorkerAsync(bufferANSPHRReceived(0))
                            'ElseIf Not useRequestFlag Then 'On initialization running phase
                            '    processingLastANSPHRInstructionFlag = True
                            '    wellBaseLineWorker.RunWorkerAsync(bufferANSPHRReceived(0))
                            'End If

                            ' XB 15/10/2013 - Add PauseAlreadySentFlagAttribute + PAUSE_START and PAUSE_END also activate startDoWorker flag - BT #1318
                            'AG 19/11/2013 - #1396-b Add also BARCODE_ACTION_RECEIVED

                            Dim startDoWorker As Boolean = False 'Evaluate if first ANSPHR in queue can be processed or not
                            If Not endRunAlreadySentFlagAttribute AndAlso Not abortAlreadySentFlagAttribute AndAlso Not PauseAlreadySentFlagAttribute Then
                                Select Case AnalyzerCurrentActionAttribute
                                    Case AnalyzerManagerAx00Actions.TEST_PREPARATION_RECEIVED, AnalyzerManagerAx00Actions.PREDILUTED_TEST_RECEIVED, AnalyzerManagerAx00Actions.ISE_TEST_RECEIVED, _
                                        AnalyzerManagerAx00Actions.SKIP_START, AnalyzerManagerAx00Actions.WASHING_RUN_START, AnalyzerManagerAx00Actions.START_INSTRUCTION_START, AnalyzerManagerAx00Actions.START_INSTRUCTION_END, _
                                        AnalyzerManagerAx00Actions.PAUSE_START, AnalyzerManagerAx00Actions.PAUSE_END, AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED

                                        startDoWorker = True

                                        'AG 19/11/2013 - This case is not possible but we add the protection
                                        If AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED AndAlso Not AllowScanInRunning Then
                                            startDoWorker = False
                                        End If

                                        'Special cases: ise test and prediluted test appears several cycles (when dummies are performed)
                                    Case AnalyzerManagerAx00Actions.ISE_TEST_END, AnalyzerManagerAx00Actions.PREDILUTED_TEST_END
                                        'Activate startDoWorker to process readings only when biochemical Request = 0
                                        If myRequestValue = 0 Then 'AG 14/09/2012 v052 - If Not AnalyzerIsReadyAttribute Then
                                            startDoWorker = True
                                        Else
                                            'When request = 1 the Software the priority is ask the request and send next preparation
                                            'The readings will treated when preparation will be accepted 
                                        End If

                                        'AG 07/02/2104 - BT #1484 Process readings also with A:60
                                        'SOUND_DONE has not to be taken into account Fw answers with status just receive it (exception of running instructions timming)
                                        'AG 29/01/214 -> The sound done can be added to the actions that extract 1st readings instruction from queue and process it
                                        '                because in final v100 during running the SOUND instruction is sent after status reception
                                    Case AnalyzerManagerAx00Actions.SOUND_DONE
                                        startDoWorker = True
                                        'AG 07/02/2014 - BT #1484

                                    Case Else
                                        'WASHING_RUN_END, TEST_PREPARATION_END, SKIP_END have not to be taken into account (because we receive R:1 with these actions, the request is used for search and send next , not for process readings!!!)
                                End Select

                                'When a instruction is rejected (out of time or repeated in cycle)
                                If Not startDoWorker AndAlso (errorValue = 28 OrElse errorValue = 34) Then
                                    startDoWorker = True
                                End If

                            Else 'Leaving RUNNING
                                startDoWorker = True
                            End If

                            If startDoWorker Then
                                'AG 02/06/2014 - #1644 check if create WS executions semaphore is busy or ready
                                'If busy we cannot process readings now, so we will evaluate readings next BAx00 cycle
                                Dim semaphoreFree As Boolean = True
                                If GlobalConstants.CreateWSExecutionsWithSemaphore Then
                                    'semaphoreFree = CBool(IIf(GlobalSemaphores.createWSExecutionsQueue = 0, True, False))
                                End If
                                If semaphoreFree Then
                                    processingLastANSPHRInstructionFlag = True
                                    wellBaseLineWorker.RunWorkerAsync(bufferANSPHRReceived(0))
                                End If
                                'AG 02/06/2014 - #1644
                            End If


                        End If 'If bufferANSPHRReceived.Count > 0 AndAlso Not processingLastANSPHRInstructionFlag Then
                    End SyncLock
                End If

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Treat STATUS received: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Information, False)
                End If
                'AG 28/06/2012

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function


        ''' <summary>
        ''' Readings reception process (save readings, update executions tables and trigger calculations when needed)
        ''' 
        ''' Method scheme (AG 18/02/2011)
        ''' Open Transaction (Connection1)
        ''' --- Loop
        ''' --- ... Decode readings and build DS
        ''' --- End Loop
        ''' --- Save readings
        ''' Commint transaction (Connection1)
        ''' Open transaction (Connection1)
        ''' --- Use the same DS for loop
        ''' --- ... Call calculations, auto repetitions, online export, ... when needed
        ''' --- End Loop
        ''' Commint transaction (Connection1)
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Defined by:  AG - 26/04/2010
        ''' Created by: GDS - 27/04/2010
        ''' Modified by AG 12/05/2010 - Call the repetitions and autoexport when the ordertestid is closed
        ''' Modified by AG 08/06/2010 - Validate execution readings before call calculations methods (not SATURATED READINGS neiter READING_ERROR values exits)
        ''' AG 26/11/2010 - prepare the UI_Refresh event
        ''' AG 18/02/2011 - implement scheme described in summary
        ''' AG 22/03/2011 - Update executions.ThermoWarningFlag when exists the thermo reactions rotor alarm 
        '''                 Do not save readings when execution status is PENDING
        ''' Modified by AG 20/04/2011 - changes in instruction definition Excel Instruction Rev23)
        ''' Modified by AG 04/10/2011 - export generates event RESULTS_CALCULATED + move code for Auto repetitions, auto export to be used also when
        '''                             last replicate in ordertest is finished with CLOSEDNOK
        ''' Modified AG 12/06/202 - complete redesing ... the photometric readings are treated in a different thread in order the Sw can attend quickly to the Analzyer requests in Running
        '''                         The old method is commented, renamed as ProcessReadingsReceived_AG12062012 and moved into '9- Temporal Testing and Old Methods.vb'
        ''' </remarks>
        Private Function ProcessReadingsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'AG 28/06/2012 - Add to buffer and not launch the threat here (unless the end instruction has been sent)
                'Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                ''Loop Sw can not process more than 1 ansphr instruction at any time
                'While processingLastANSPHRInstructionFlag
                '    Application.DoEvents() 'This line requires import windows forms
                'End While

                ''Inform last readings received are in process 
                'processingLastANSPHRInstructionFlag = True
                'wellBaseLineWorker.RunWorkerAsync(pInstructionReceived)

                'Dim myLogAcciones As New ApplicationLogManager()
                'myLogAcciones.CreateLogActivity("Treat ANSPHR received. Launch parallel trheat: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Information, False)

                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                SyncLock lockThis
                    bufferANSPHRReceived.Add(pInstructionReceived)

                    ''Launch the parallel threat when END or ABORT instruction has been already sent
                    'If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso bufferANSPHRReceived.Count > 0 _
                    '   AndAlso (endRunAlreadySentFlagAttribute OrElse abortAlreadySentFlagAttribute) _
                    '   AndAlso Not processingLastANSPHRInstructionFlag Then
                    '    processingLastANSPHRInstructionFlag = True
                    '    wellBaseLineWorker.RunWorkerAsync(bufferANSPHRReceived(0))
                    'End If

                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Treat ANSPHR (Add to buffer) received. Buffer items: " & bufferANSPHRReceived.Count.ToString & ". Time: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Information, False)
                End SyncLock

                'AG 28/06/2012
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY:AG
        ''' Modifies BY: TR -Add the functionally to process the base line reception.
        ''' Modified by AG 01/03/2011 - ANSAL is rename as ANSBLD (ANSBL and ANSDL are removed) - Tested PENDING
        ''' </remarks>
        Private Function ProcessBaseLineReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myGlobalDataTO As New GlobalDataTO


            Try
                'InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - the waiting timer is disabled on every reception process ('AG 18/07/2011)

                myGlobalDataTO = DAOBase.GetOpenDBTransaction(Nothing)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Business

                        'AG 28/02/2012 - move this code when the received results generates baseline init err alarm 
                        ''If exits base line alarm delete it before send a new ALIGHT instruction
                        'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
                        '    myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                        'End If

                        Dim query As New List(Of InstructionParameterTO)
                        'Const myRowIndex As Integer = 0
                        Dim myOffset As Integer = -1
                        Dim myUtility As New Utilities()
                        Dim nextBaseLineID As Integer = 0
                        Dim myInstructionType As String = ""
                        Dim myWell As Integer = 0
                        Dim myTotalResults As Integer = 0
                        Dim baseLineWithAdjust As Boolean = False

                        'AG 03/01/2011 - Get the instruction type value. to set the offset.
                        myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 2)

                        If Not myGlobalDataTO.HasError Then
                            myInstructionType = DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue
                            If myInstructionType = GlobalEnumerates.AppLayerInstrucionReception.ANSBLD.ToString Then baseLineWithAdjust = True
                        Else
                            Exit Try
                        End If
                        'AG 03/01/2011

                        'Get the well used from pInstructionReceived and initialize myWell variable
                        query = (From a In pInstructionReceived Where a.ParameterIndex = 3 Select a).ToList()
                        If query.Count > 0 Then
                            myWell = CType(query.First().ParameterValue, Integer)
                            CurrentWellAttribute = myWell 'AG 30/06/2011 - When Sw receives a ALIGHT updates this value for repeat alight in the next well is results are rejected
                            '                                                                          (this code is required due the well in status instruction has another value due means another thing and _
                            '                                                                           else the Alight repetition can be perform in the same well another time)
                        End If
                        '1st Get the next baselineID
                        myGlobalDataTO = Me.GetNextBaseLineID(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myWell, baseLineWithAdjust)
                        If Not myGlobalDataTO.HasError Or Not myGlobalDataTO.SetDatos Is Nothing Then
                            nextBaseLineID = DirectCast(myGlobalDataTO.SetDatos, Integer)
                        Else
                            Exit Try
                        End If

                        'filter to get the results only from the instruction recived.
                        query = (From a In pInstructionReceived Where a.ParameterIndex > 3 Select a).ToList()

                        'Set the offset value depending on the instruction type
                        myOffset = 7
                        Dim myBaseLineDS As New BaseLinesDS

                        If myOffset > 0 And query.Count > 0 Then

                            myTotalResults = CInt(query.Count / myOffset)
                            Dim myIteration As Integer
                            Dim myBaseLineRow As BaseLinesDS.twksWSBaseLinesRow
                            'Const myTempIndexMD As Integer = 0
                            'Const myTempIndexRD As Integer = 0

                            For i As Integer = 1 To myTotalResults Step 1
                                myIteration = i
                                myBaseLineRow = myBaseLineDS.twksWSBaseLines.NewtwksWSBaseLinesRow
                                myBaseLineRow.BeginEdit()
                                myBaseLineRow.BaseLineID = nextBaseLineID
                                myBaseLineRow.WellUsed = myWell
                                myBaseLineRow.WorkSessionID = Me.WorkSessionIDAttribute
                                myBaseLineRow.AnalyzerID = Me.AnalyzerIDAttribute
                                myBaseLineRow.DateTime = DateTime.Now

                                'Get the Wavelenght
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 4 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    myBaseLineRow.Wavelength = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                Else
                                    Exit For
                                End If

                                'MainLine
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 5 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.MainLight = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefLight
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 6 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.RefLight = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'MainDark
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 7 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.MainDark = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefDark
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 8 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.RefDark = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'IT
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 9 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.IT = CType(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If

                                Else
                                    Exit For
                                End If
                                'DAC
                                myGlobalDataTO = myUtility.GetItemByParameterIndex(pInstructionReceived, 10 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.DAC = CType(DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If
                                Else
                                    Exit For
                                End If

                                myBaseLineRow.SetABSvalueNull() 'This field is use in base line by well
                                myBaseLineRow.SetIsMeanNull() 'This field is use in base line by well

                                myBaseLineRow.EndEdit()
                                myBaseLineDS.twksWSBaseLines.AddtwksWSBaseLinesRow(myBaseLineRow)
                            Next
                            myBaseLineDS.AcceptChanges()

                        End If


                        If (Not myGlobalDataTO.HasError) Then
                            'Save baseline results into database
                            myGlobalDataTO = Me.SaveBaseLineResults(dbConnection, myBaseLineDS, baseLineWithAdjust)

                            If Not myGlobalDataTO.HasError Then
                                'Perform ANSBLD results business
                                validALIGHTAttribute = False
                                myGlobalDataTO = baselineCalcs.ControlAdjustBaseLine(dbConnection, myBaseLineDS)
                                validALIGHTAttribute = baselineCalcs.validALight
                                existsALIGHTAttribute = baselineCalcs.existsAlightResults 'AG 20/06/2012

                                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                                    'ControlAdjustBaseLine only generates myAlarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR  ... now 
                                    'we have to calculate his status = TRUE or FALSE
                                    Dim alarmStatus As Boolean = False 'By default no alarm
                                    Dim myAlarm As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                                    myAlarm = CType(myGlobalDataTO.SetDatos, GlobalEnumerates.Alarms)
                                    If myAlarm <> GlobalEnumerates.Alarms.NONE Then
                                        alarmStatus = True

                                        'AG 28/02/2012 - If exits base line alarm delete it before send a new ALIGHT instruction
                                        If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
                                            myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                                        End If


                                    Else ' Valid alight
                                        myAlarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR
                                        alarmStatus = False
                                        baselineInitializationFailuresAttribute = 0
                                    End If

                                    Dim AlarmList As New List(Of GlobalEnumerates.Alarms)
                                    Dim AlarmStatusList As New List(Of Boolean)
                                    PrepareLocalAlarmList(myAlarm, alarmStatus, AlarmList, AlarmStatusList) 'Baseline_init_err (true or false)

                                    'AG 23/05/2012 - Baseline err excludes methacrylate rotor warn
                                    If alarmStatus AndAlso myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                        myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                                    End If
                                    'AG 23/05/2012

                                    If AlarmList.Count > 0 Then
                                        'myGlobalDataTO = ManageAlarms(dbConnection, AlarmList, AlarmStatusList)
                                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                        If GlobalBase.IsServiceAssembly Then
                                            ' XBC 17/10/2012 - Alarms treatment for Service
                                            ' Not Apply
                                            'myGlobalDataTO = ManageAlarms_SRV(dbConnection, AlarmList, AlarmStatusList)
                                        Else
                                            myGlobalDataTO = ManageAlarms(dbConnection, AlarmList, AlarmStatusList)
                                        End If
                                    End If

                                    'If no alarm or all ALIGHT has been rejected ... inform presentation depending the current Sw process
                                    If Not myGlobalDataTO.HasError Then
                                        'AG 28/02/2012
                                        'If AlarmList.Count = 0 Or baselineInitializationFailuresAttribute >= BASELINE_INIT_FAILURES Then
                                        If validALIGHTAttribute Or baselineInitializationFailuresAttribute >= BASELINE_INIT_FAILURES Then

                                            'Inform flag for alight is finished
                                            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "END")

                                            Dim wupManeuversFinishFlag As Boolean = False 'AG 23/05/2012
                                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS" Then

                                                ' XBC 13/02/2012 - CODEBR Configuration instruction
                                                'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "CLOSED")
                                                ' XBC 13/02/2012 - CODEBR Configuration instruction

                                                'Inform the start instrument (wup process has finished)
                                                Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                                                Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                                                'WUPCOMPLETEFLAG
                                                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                                                With myAnalyzerSettingsRow
                                                    .AnalyzerID = AnalyzerIDAttribute
                                                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                                                    .CurrentValue = "1"
                                                End With
                                                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                                                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                                                myGlobalDataTO = myAnalyzerSettings.Save(dbConnection, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

                                                wupManeuversFinishFlag = True

                                                ' XBC 13/02/2012 - CODEBR Configuration instruction
                                                'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process FINISH sent again the config instruction (maybe user has changed something)

                                                Dim BarCodeDS As New AnalyzerManagerDS
                                                Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                                                rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                                                With rowBarCode
                                                    .RotorType = "SAMPLES"
                                                    .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                                                    .Position = 0
                                                End With
                                                BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                                BarCodeDS.AcceptChanges()
                                                myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
                                                ' XBC 13/02/2012 - CODEBR Configuration instruction

                                            ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BASELINEprocess.ToString) = "INPROCESS" Then
                                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BASELINEprocess, "CLOSED")


                                            ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "CLOSED")

                                                'Remove alarm change rotor recommend
                                                If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                                    myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                                                End If

                                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED, 1, True) 'Inform the FALSE sensor the new rotor process is finished for UI refresh

                                                'AG 26/03/2012 - Special case: maybe the user was starting the instrument and the process has been
                                                'paused because there is not reactions rotor ... in this case when a valid alight is received
                                                'Sw must inform the start instrument process is OK
                                                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "PAUSED" Then
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")

                                                    'Inform the start instrument (wup process has finished)
                                                    Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                                                    Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                                                    'WUPCOMPLETEFLAG
                                                    myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                                                    With myAnalyzerSettingsRow
                                                        .AnalyzerID = AnalyzerIDAttribute
                                                        .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                                                        .CurrentValue = "1"
                                                    End With
                                                    myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                                                    Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                                                    myGlobalDataTO = myAnalyzerSettings.Save(dbConnection, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

                                                    wupManeuversFinishFlag = True

                                                    ' XBC 13/02/2012 - CODEBR Configuration instruction
                                                    Dim BarCodeDS As New AnalyzerManagerDS
                                                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                                                    rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                                                    With rowBarCode
                                                        .RotorType = "SAMPLES"
                                                        .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                                                        .Position = 0
                                                    End With
                                                    BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                                    BarCodeDS.AcceptChanges()
                                                    myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
                                                    ' XBC 13/02/2012 - CODEBR Configuration instruction
                                                End If

                                            End If

                                            'AG 23/05/2012
                                            If wupManeuversFinishFlag Then
                                                'Inform the presentation layer to activate the STOP wup button
                                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED, 1, True)
                                                MyClass.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 - ISE alarms ready to be shown

                                                'AG 23/05/2012 - Evaluate if Sw has to recommend to change reactions rotor (remember all alarms are remove when Start Instruments starts)
                                                Dim analyzerReactRotor As New AnalyzerReactionsRotorDelegate
                                                myGlobalDataTO = analyzerReactRotor.ChangeReactionsRotorRecommended(dbConnection, AnalyzerIDAttribute, myAnalyzerModel)
                                                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                                                    If CType(myGlobalDataTO.SetDatos, String) <> "" Then
                                                        myAlarm = GlobalEnumerates.Alarms.BASELINE_WELL_WARN
                                                        'Update internal alarm list if exists alarm but not saved it into database!!!
                                                        'But generate refresh
                                                        If Not myAlarmListAttribute.Contains(myAlarm) Then
                                                            myAlarmListAttribute.Add(myAlarm)
                                                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                                                        End If
                                                    End If
                                                End If
                                            End If
                                            'AG 23/05/2012

                                            'AG 16/05/2012 - If warm up maneuvers are finished check for the ise alarms 
                                            If SensorValuesAttribute.ContainsKey(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) AndAlso SensorValuesAttribute(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 Then

                                                Dim tempISEAlarmList As New List(Of GlobalEnumerates.Alarms)
                                                Dim tempISEAlarmStatusList As New List(Of Boolean)
                                                myGlobalDataTO = MyClass.ISE_Manager.CheckAlarms(MyClass.Connected, tempISEAlarmList, tempISEAlarmStatusList)

                                                AlarmList.Clear()
                                                AlarmStatusList.Clear()
                                                For i As Integer = 0 To tempISEAlarmList.Count - 1
                                                    PrepareLocalAlarmList(tempISEAlarmList(i), tempISEAlarmStatusList(i), AlarmList, AlarmStatusList)
                                                Next

                                                If AlarmList.Count > 0 Then
                                                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                                    If GlobalBase.IsServiceAssembly Then
                                                        ' XBC 16/10/2012 - Alarms treatment for Service
                                                        ' Not Apply
                                                        'myGlobalDataTO = ManageAlarms_SRV(dbConnection, AlarmList, AlarmStatusList)
                                                    Else
                                                        myGlobalDataTO = ManageAlarms(dbConnection, AlarmList, AlarmStatusList)
                                                    End If
                                                End If
                                            End If
                                            'AG 16/05/2012

                                            'Update analyzer session flags into DataBase
                                            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                myGlobalDataTO = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                                            End If

                                        End If
                                    End If

                                End If
                            End If

                        End If
                        query = Nothing 'AG 02/08/2012 - free memory

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessBaseLineReceived", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Software has received a new Hw alarms details instruction
        ''' Detect the alarms status new or fixed and call to the alamrs manager system 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation: AG 14/03/2011
        ''' </remarks>
        Private Function ProcessHwAlarmDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim myGlobal As New GlobalDataTO

            Try
                Debug.Print("ANSERR received !")
                'AG 03/07/2012 - Running Cycles lost - Solution!
                'myGlobal = DAOBase.GetOpenDBTransaction(Nothing)

                'If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                '    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                '    If (Not dbConnection Is Nothing) Then
                Dim myAlarmsReceivedList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmsStatusList As New List(Of Boolean)

                '1- Read the different instruction fields (implement in the same way as we do for instance at ProcessStatusReceived)
                '   and fill the internal alarm & alarm status lists using the method PrepareLocalAlarmList 
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                ' Get error number field
                Dim errorNumber As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    errorNumber = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                End If

                Dim errorCode As Integer = 0
                'Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                'Dim alarmStatus As Boolean = False
                Dim myAlarms As New List(Of GlobalEnumerates.Alarms)
                Dim myErrorCode As New List(Of Integer)

                For i As Integer = 1 To errorNumber
                    myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3 + i)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If

                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        errorCode = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)

                        'SGM 16/10/2012 - Ommit these alarms in order to avoid endless loop
                        'E:20 (INST_REJECTED_ERR)
                        'E:21 (INST_ABORTED_ERR)
                        'E:99 Multiple

                        ' XBC 29/10/2012 - Service Sw no ommit err 21 on ANSERR
                        'If errorCode <> 20 And errorCode <> 21 And errorCode <> 99 Then
                        '    myErrorCode.Add(errorCode)
                        'End If
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            ' Service Sw
                            If errorCode <> 20 And errorCode <> 99 Then
                                myErrorCode.Add(errorCode)
                            End If
                        Else
                            ' User Sw
                            If errorCode <> 20 And errorCode <> 21 And errorCode <> 99 Then
                                myErrorCode.Add(errorCode)
                            End If
                        End If
                        ' XBC 29/10/2012

                        'end SGM 16/10/2012

                        'AG 02/03/2012 old translation method
                        'Translate the Fw error code into the Sw AlarmID enum code 
                        'AG 17/02/2012 Activated (AG 19/01/2012 - disable this functionality until fw & sw work together with alarms)
                        'alarmID = TranslateErrorCodeToAlarmID(errorCode, alarmStatus)
                        'PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmsReceivedList, myAlarmsStatusList)

                    End If
                Next

                'AG 02/03/2012 old translation method
                'AG 28/02/2012 - Evaluate if the fridge is damaged then add the alarm 
                'alarmID = GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR
                'alarmStatus = IsFridgeStatusDamaged(myAlarmsReceivedList, myAlarmsStatusList)
                'PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmsReceivedList, myAlarmsStatusList)
                'AG 28/02/2012

                'New translation method
                myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)

                'SGM 09/11/2012 - reset flag in case of Rotor missing error is not received
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    If Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) Then
                        MyClass.IsServiceRotorMissingInformed = False
                    End If
                End If
                'end SGM 09/11/2012

                For Each alarmID As GlobalEnumerates.Alarms In myAlarms
                    PrepareLocalAlarmList(alarmID, True, myAlarmsReceivedList, myAlarmsStatusList, "", Nothing, True) 'AG 13/04/2012 - last parameter (optional) must be true for the error code alarms
                Next

                'AG 02/03/2012

                ' XBC 17/10/2012 - Alarms treatment for Service
                Dim myFwCodeErrorReceivedList As New List(Of String)
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ' Initialize Error Codes List
                    myErrorCodesAttribute.Clear()
                    ' Prepare error codes List received from Analyzer
                    PrepareLocalAlarmList_SRV(myErrorCode, myFwCodeErrorReceivedList)
                End If
                ' XBC 17/10/2012

                If Not myGlobal.HasError Then
                    If myAlarmsReceivedList.Count > 0 Then
                        '3- Finally call manage all alarms detected (new or fixed)
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            ' XBC 16/10/2012 - Alarms treatment for Service
                            'myGlobal = ManageAlarms_SRV(dbConnection, myAlarmsReceivedList, myAlarmsStatusList)
                            myGlobal = ManageAlarms_SRV(dbConnection, myAlarmsReceivedList, myAlarmsStatusList, myFwCodeErrorReceivedList, True)
                            ' XBC 16/10/2012
                        Else
                            myGlobal = ManageAlarms(dbConnection, myAlarmsReceivedList, myAlarmsStatusList)
                        End If
                    End If

                End If

                'AG 03/07/2012 - Running Cycles lost - Solution!
                '    End If
                'End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwAlarmDetailsReceived", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If

            Return myGlobal
        End Function

        ''' <summary>
        ''' Sw has received an ANSBR1 or ANSBR2 or ANSBM1 instruction 
        ''' Do business depending the instruction parameter values
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 15/03/2011
        ''' Modified by: TR 27/09/2012 - Implement funcitionality of reagents on board, when the bottle status change to LOCKED
        '''              SA 15/11/2012 - Before calling function ReagentBottleManagement, if field BarcodeInfo is not NULL, check 
        '''                              also it is informed (different of an empty string)
        '''              AG 18/11/2013 - (#1385) Decrement the number of tests using the positions for sample or reagent2 or arm r2 washing
        '''              JV 09/01/2014 - BT #1443 ==> Added Status value in his
        '''              SA 20/01/2014 - BT #1443 ==> Changes made previously for this same item did not work due to the Position Status passed when call function 
        '''                                           ReagentsOnBoardDelegate.ReagentBottleManagement was the current Position Status instead of the re-calculated 
        '''                                           Position Status (according value of the LevelControl informed for the Analyzer in the received Instruction).
        '''                                           Code included in section labelled as (2.2) has been re-written to make it more clear. 
        '''                                           Added a new call to ReagentsOnBoardDelegate.ReagentBottleManagement in section labelled as (2.3), to allow
        '''                                           save the DEPLETED Status when the Instruction indicates the Detection Level has failed
        '''              SA 28/05/2014 - BT #1627 ==> Added changes to avoid call functions that are specific for REAGENTS (functions of ReagentsOnBoardManagement)  
        '''                                           when DILUTION and/or WASHING SOLUTIONS are dispensed. Current code just verify the Rotor Type, but not the 
        '''                                           Bottle content. Changes have been made in section labelled (2.2) 
        '''              AG 04/06/2014 - #1653 Check if WRUN (reagents washings) could not be completed remove the last WRUN for wash reagents sent 
        ''' </remarks>
        Private Function ProcessArmStatusRecived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                '*******************************************************'
                '(1) GET ALL FIELDS INFORMED IN THE RECEIVED INSTRUCTION'
                '*******************************************************'
                'Get Instruction field (parameter index 2)
                Dim myInst As String = String.Empty
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 2)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                myInst = myInstParamTO.ParameterValue

                'Get Preparation Identifier field (ID, parameter index 3)
                Dim myPrepID As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myPrepID = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Well Number field (W, parameter index 4)
                Dim myWell As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myWell = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Well Status field (S, parameter index 5)
                Dim myWellStatus As String = ""
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                myWellStatus = myInstParamTO.ParameterValue

                'Get Bottle Position  field (P, parameter index 6)
                Dim myBottlePos As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myBottlePos = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Level Control field (L, parameter index 7)
                Dim myLevelControl As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myLevelControl = CInt(myInstParamTO.ParameterValue)
                End If

                Dim myClotStatus As String = ""
                Dim myRotorName As String = "REAGENTS"
                If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then
                    myRotorName = "SAMPLES"

                    'Get Clot Status field (C, parameter index 8)
                    myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If
                    myClotStatus = myInstParamTO.ParameterValue
                End If

                '**********************************************************'
                '(2) BUSINESS LOGIC FOR PROCESSING THE RECEIVED INSTRUCTION'
                '**********************************************************'

                '********************************'
                '(2.1) MONITORING REACTIONS ROTOR'
                '********************************'
                Dim exec_delg As New ExecutionsDelegate
                Dim uiRefreshMyGlobal As New GlobalDataTO 'Use an auxiliar GlobalDataTO
                Dim myReactionsRotor As New ReactionsRotorDelegate

                'Using the variables: myPrepID, myWell & myWellStatus create or update a record into twksWSReactionsRotor table for monitoring the Reactions Rotor
                myGlobal = myReactionsRotor.UpdateWellByArmStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myWell, myWellStatus, myPrepID, _
                                                                  AnalyzerCurrentActionAttribute, myRotorName)

                'Prepare UIRefresh Dataset (NEW_WELLSTATUS_RECEIVED) for refresh screen when needed
                If (Not myGlobal.HasError AndAlso myPrepID > 0) Then
                    Dim reactionsDS As ReactionsRotorDS = DirectCast(myGlobal.SetDatos, ReactionsRotorDS)
                    uiRefreshMyGlobal = PrepareUIRefreshEventNum3(dbConnection, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, reactionsDS, True)
                End If

                'Prepare internal variables for Alarms Management
                Dim alarmStatusList As New List(Of Boolean)
                Dim alarmAdditionalInfoList As New List(Of String)
                Dim alarmList As New List(Of GlobalEnumerates.Alarms)

                Dim reagentNumberWithNoVolume As Integer = 0 'NOTE: 0=Sample, 1=Reagent1, 2=Reagent2
                Dim alarmLevelDetectionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim alarmCollisionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE

                Select Case (myInst)
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.R1_COLLISION_WARN
                        reagentNumberWithNoVolume = 1

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.R2_COLLISION_WARN
                        reagentNumberWithNoVolume = 2

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.S_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.S_COLLISION_WARN
                        reagentNumberWithNoVolume = 0
                    Case Else
                End Select

                'BT #1385 ==> Decrement the Number of Tests using the Position for Sample or Reagent2 or Arm R2 Washing
                If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString OrElse myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString) Then
                    Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                    myGlobal = inProcessDlg.DecrementInProcessTestsNumber(dbConnection, AnalyzerIDAttribute, myRotorName, myBottlePos)
                End If

                '*********************************************************************************************************************************'
                '(2.2) UPDATE INFORMATION OF THE ROTOR POSITION (FOR ALL ARMS, ALTHOUGH THE BUSINESS IS DIFFERENT FOR SAMPLES AND REAGENTS ROTORS)' 
                '*********************************************************************************************************************************'
                Dim newElementStatus As String = "POS"
                Dim moreVolumeAvailable As Boolean = True
                Dim myBottleStatus As String = String.Empty
                Dim newPositionStatus As String = String.Empty

                'Read the current Rotor Position Status and assign it to initialRotorPositionStatus
                Dim rcp_DS As New WSRotorContentByPositionDS
                Dim rcp_del As New WSRotorContentByPositionDelegate
                Dim initialRotorPositionStatus As String = String.Empty
                Dim elementTubeContent As String = String.Empty 'AG 30/05/2014 #1627

                myGlobal = rcp_del.ReadByRotorTypeAndCellNumber(dbConnection, myRotorName, myBottlePos, WorkSessionIDAttribute, AnalyzerIDAttribute)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    rcp_DS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                    If (rcp_DS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                        If (Not rcp_DS.twksWSRotorContentByPosition(0).IsStatusNull) Then initialRotorPositionStatus = rcp_DS.twksWSRotorContentByPosition(0).Status
                        If (Not rcp_DS.twksWSRotorContentByPosition(0).IsTubeContentNull) Then elementTubeContent = rcp_DS.twksWSRotorContentByPosition(0).TubeContent 'AG 30/05/2014 #1627
                    End If
                End If

                Dim testLeft As Integer = 0
                Dim realVolume As Single = 0

                'If Reagent1 or WashingSolution or Predilution or Sample or ISE or Reagent2 dispensed
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.WS.ToString Or _
                    myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.PD.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString Or _
                    myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.DI.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString) Then
                    'Calculate the remaining Bottle Volume
                    Dim changesInPosition As Boolean = False
                    realVolume = 60
                    testLeft = 0

                    'Calculate the real volume test left for the bottle in position myBottlePos (using LevelControl value myLevelControl)
                    If (myRotorName = "REAGENTS") Then 'For reagents
                        Dim reagOnBoard As New ReagentsOnBoardDelegate
                        myGlobal = reagOnBoard.CalculateBottleVolumeTestLeft(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myBottlePos, myLevelControl, realVolume, testLeft)

                        'BT #1443 - Calculate the new Position Status according the value of the Level Control returned by the Analyzer
                        Dim limitList As List(Of FieldLimitsDS.tfmwFieldLimitsRow) = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                                                                     Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.REAGENT_LEVELCONTROL_LIMIT.ToString _
                                                                                    Select a).ToList
                        If (limitList.Count > 0) Then
                            Debug.Print("=====================================")
                            Debug.Print("LEVEL CONTROL --> " & myLevelControl.ToString())

                            If (myLevelControl = 0) Then
                                newPositionStatus = "DEPLETED"

                            ElseIf (myLevelControl < CInt(limitList(0).MinValue)) Then
                                newPositionStatus = "FEW"

                            ElseIf (myLevelControl < CInt(limitList(0).MaxValue)) Then
                                newPositionStatus = "FEW"
                            Else
                                newPositionStatus = "INUSE"
                            End If

                            Debug.Print("NEW POS STATUS --> " & newPositionStatus.ToString())
                        End If
                        limitList = Nothing

                        'BT #1627 - Execute this block only when Reagent1 or Reagent2 have been dispensed, skip it for Dilution and Washing Solutions
                        If (elementTubeContent = "REAGENT") Then
                            'Validate and update Volumes in the Reagents Historic Table (verify if the Bottle has to be locked due to an invalid refill
                            If (Not myGlobal.HasError) Then
                                If (rcp_DS.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso (Not rcp_DS.twksWSRotorContentByPosition(0).IsBarCodeInfoNull) AndAlso _
                                   (rcp_DS.twksWSRotorContentByPosition(0).BarCodeInfo <> String.Empty) Then
                                    'BT #1443 - Added the new calculated Position Status to save it Reagents Historic Table (newPositionStatus, not current Rotor Position) 
                                    myGlobal = reagOnBoard.ReagentBottleManagement(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myBottlePos, _
                                                                                   rcp_DS.twksWSRotorContentByPosition(0).BarCodeInfo, _
                                                                                   newPositionStatus, realVolume)

                                    If (Not myGlobal.HasError) Then
                                        myBottleStatus = myGlobal.SetDatos.ToString()

                                        If (myBottleStatus = "LOCKED") Then
                                            newPositionStatus = myBottleStatus

                                            'Add the Alarm of Bottle blocked due to an invalid refill was detected only if the previous Position Status was not LOCKED
                                            If (initialRotorPositionStatus <> "LOCKED") Then
                                                Dim myExecution As Integer = 0
                                                Dim addInfoInvalidRefill As String = String.Empty

                                                myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                    Dim lockedBottleExecDS As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                                    If (lockedBottleExecDS.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedBottleExecDS.twksWSExecutions(0).IsExecutionIDNull) Then
                                                        myExecution = lockedBottleExecDS.twksWSExecutions(0).ExecutionID
                                                    End If

                                                    'Get the information needed and add the Alarm to the Alarms List
                                                    myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, ActiveAnalyzer, ActiveWorkSession, myExecution, myBottlePos, _
                                                                                              GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, reagentNumberWithNoVolume)
                                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                        addInfoInvalidRefill = CType(myGlobal.SetDatos, String)
                                                        PrepareLocalAlarmList(GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, True, alarmList, alarmStatusList, addInfoInvalidRefill, alarmAdditionalInfoList, False)
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            If (Not myGlobal.HasError AndAlso (realVolume <> 60 OrElse testLeft <> 0)) Then
                                changesInPosition = True
                            End If
                        End If
                    Else
                        realVolume = 0
                        testLeft = 0

                        'NOTE: Is required implement similar business for SAMPLES or not? By now do not implement it
                    End If

                    '(OK) Using myLevelControl if volume is lower than a security volume then change STATUS to "FEW" in order to indicate few volume (twksWSRotorContentByPosition table)
                    '(OK)If volume is lower than a critical volume then set STATUS to "DEPLETED" (twksWSRotorContentByPosition table)

                    ''TR 28/09/2012 - Validate if BottleStatus is LOCKED to set it as new Position Status
                    'Dim limitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                    'If (myBottleStatus = "LOCKED") Then
                    '    newPositionStatus = myBottleStatus

                    '    'TR 02/10/2012 - Add the Alarm of Bottle blocked due to an invalid refill was detected only if the previous Position Status was not LOCKED
                    '    If (initialRotorPositionStatus <> "LOCKED") Then
                    '        Dim myExecution As Integer = 0
                    '        Dim addInfoInvalidRefill As String = String.Empty

                    '        myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                    '        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    '            Dim lockedBottleExecDS As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                    '            If (lockedBottleExecDS.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedBottleExecDS.twksWSExecutions(0).IsExecutionIDNull) Then
                    '                myExecution = lockedBottleExecDS.twksWSExecutions(0).ExecutionID
                    '            End If
                    '            'lockedExecutionsDS.Clear() 'AG 07/02/2012 this line can not be here because the LP case neededs it!! (AG 02/02/2012 - clear the dataset. The executions that must be informed to be locked are calculated later)
                    '        End If

                    '        myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, ActiveAnalyzer, ActiveWorkSession, myExecution, myBottlePos, _
                    '                                                  GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, reagentNumberWithNoVolume)
                    '        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    '            addInfoInvalidRefill = CType(myGlobal.SetDatos, String)
                    '        End If

                    '        PrepareLocalAlarmList(GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, True, alarmList, alarmStatusList, addInfoInvalidRefill, alarmAdditionalInfoList, False) 'Inform about the bottle locked. 
                    '        'AG + TR 01/10/2012 -END
                    '    End If
                    'Else
                    '    If myRotorName = "REAGENTS" Then 'For reagents
                    '        'Read the parameter REAGENT_BOTTLE_STEP_LIMIT in tfmwFieldLimits
                    '        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                    '                     Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.REAGENT_LEVELCONTROL_LIMIT.ToString _
                    '                     Select a).ToList

                    '        If limitList.Count > 0 Then
                    '            If myLevelControl = 0 Then
                    '                newPositionStatus = "DEPLETED" 'AG 22/02/2012

                    '            ElseIf myLevelControl < CInt(limitList(0).MinValue) Then
                    '                'AG 22/02/2012 - commented until calculation real volume formula is confirmed
                    '                'newPositionStatus = "DEPLETED"
                    '                newPositionStatus = "FEW"

                    '            ElseIf myLevelControl < CInt(limitList(0).MaxValue) Then
                    '                newPositionStatus = "FEW"
                    '            Else 'ElseIf myPrepID > 0 Then 'AG 27/02/2012 (do not use myPrepID >0 because the washings returns myPrepID = 0) 
                    '                newPositionStatus = "INUSE"
                    '            End If
                    '        End If

                    '        'Else
                    '        'NOTE: Is required implement similar business for SAMPLES or not? By now, NOT NECESSARY FOR SAMPLES ROTOR
                    '    End If
                    '    limitList = Nothing 'AG 02/08/2012 - free memory

                    'End If

                    If (Not myGlobal.HasError AndAlso (newPositionStatus <> String.Empty OrElse changesInPosition)) Then 'AG 05/12/2011 - Add changesInPosition (by now december 2011 only for REAGENTS ROTOR)
                        myGlobal = rcp_del.UpdateByRotorTypeAndCellNumber(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myRotorName, _
                                                                          myBottlePos, newPositionStatus, realVolume, testLeft, False, False)

                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            rcp_DS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                            If (rcp_DS.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                If (Not rcp_DS.twksWSRotorContentByPosition(0).IsElementStatusNull) Then newElementStatus = rcp_DS.twksWSRotorContentByPosition(0).ElementStatus
                            End If

                            'If the Position Status is DEPLETED or LOCKED (no more volume available in this Position), search if there is another Position 
                            'containing a not DEPLETED or LOCKED Bottle for the same Element 
                            If (newPositionStatus = "DEPLETED" OrElse newPositionStatus = "LOCKED") Then
                                myGlobal = rcp_del.SearchOtherPosition(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, myRotorName, myBottlePos)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    rcp_DS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                    If (rcp_DS.twksWSRotorContentByPosition.Count = 0) Then
                                        'There is not another available Bottle of the same Element
                                        moreVolumeAvailable = False
                                        If (newElementStatus = "POS") Then newElementStatus = "NOPOS"
                                    Else
                                        'Special business for Reagent2 (MTEST) -> For v2
                                        If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString) Then
                                            'Be carefull: Only one instruction can be sent at a time
                                            'Special case: R2 current bottle R2 volume lower critical volume but exists other R2 bottles ... Sw has to inform Fw for use the new bottle from now
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If

                    'Only when there has been a change in the Position Status
                    If (newPositionStatus <> String.Empty) Then
                        'Prepare UIRefresh Dataset (NEW_ROTORPOSITION_STATUS) for refresh screen when needed
                        uiRefreshMyGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, myRotorName, myBottlePos, newPositionStatus, _
                                                                      newElementStatus, realVolume, testLeft, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                    End If
                    'limitList = Nothing 'AG 24/10/2013
                End If

                '********************************************************'
                '(2.3) LEVEL DETECTION FAILS (Block Executions, all Arms)' 
                '********************************************************'
                Dim myExecutionID As Integer = -1
                Dim lockedExecutionsDS As ExecutionsDS = Nothing
                Dim addInfoVolMissingOrClot As String = String.Empty 'Additional info for volume missing or clot detection

                'AG 04/10/2012 - R1 or S or R2 level detection failed but exists additional position, then the Execution Status changes from INPROCESS to PENDING. 
                'AG 04/06/2014 - also used when a WRUN (reagents contamination) detects no volume in arm R1 (rename variable to 'reEvaluateNextToSend' instead of 'executionTurnToPendingAgainFlag')
                Dim reEvaluateNextToSend As Boolean = False

                'If level detection fail has been detected or volume detected but bottle exhausted and no move volume available -> call the  volume missing process
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString OrElse _
                    Not moreVolumeAvailable) Then
                    'AG 28/02/2012 - The Level Control is used only to mark the bottle as DEPLETED (remove condition ... OrElse myLevelControl = 0)
                    'It is possible that the Fw detects but informs L:0 (It cannot descend more)

                    'Get affected ExecutionID
                    Dim affectedExecutionID As Integer = -1
                    myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        lockedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                        If (lockedExecutionsDS.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedExecutionsDS.twksWSExecutions(0).IsExecutionIDNull) Then
                            affectedExecutionID = lockedExecutionsDS.twksWSExecutions(0).ExecutionID
                        End If
                    End If

                    'Level detection fails (no Bottle or Sample Tube volume) in Reagents or Samples Rotors 
                    '(or <only for Bottles in Reagents Rotor> when detection was OK but Tests Left calculation says there are no more volume for other test preparations)

                    If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Or Not moreVolumeAvailable) Then 'AG 02/02/2012
                        lockedExecutionsDS.Clear() 'AG 07/02/2012 Clear the DS. The executions that must be informed to be locked are calculated later

                        'The volume missing alarms are only generated when each position becomes DEPLETED (initial position status <> 'DEPLETED' but current status = 'DEPLETED')
                        If (initialRotorPositionStatus <> "DEPLETED") Then
                            'XBC 17/07/2012 - Estimated ISE Consumption by Firmware
                            'Every Samples volume alarm when ISE test operation increases PurgeA by firmware counter
                            If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then
                                'Dim myLogAccionesTmp As New ApplicationLogManager()    ' TO COMMENT !!!
                                'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 1) - Sample Volume Alarm !", "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!

                                myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myExecutionsDS As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                    If (myExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                        Dim myExecutionType As String = myExecutionsDS.twksWSExecutions(0).ExecutionType
                                        'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 1) - Execution Type : " & myExecutionType, "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!

                                        If (myExecutionType = "PREP_ISE") Then
                                            If (Not MyClass.ISE_Manager Is Nothing) Then
                                                MyClass.ISE_Manager.PurgeAbyFirmware += 1
                                                'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 1) - PurgeA [" & MyClass.ISE_Manager.PurgeAbyFirmware.ToString & "]", "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            'XBC 17/07/2012

                            'TR 01/10/2012 - If the Bottle Status is LOCKED jump this
                            If (myBottleStatus <> "LOCKED") Then
                                'AG 23/05/2012 - Remove condition because when WashSolution fails, affectedExecutionID = -1 
                                'If affectedExecutionID <> -1 Then
                                addInfoVolMissingOrClot = ""
                                myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, ActiveAnalyzer, ActiveWorkSession, affectedExecutionID, myBottlePos, _
                                                                          alarmLevelDetectionEnum, reagentNumberWithNoVolume)

                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    addInfoVolMissingOrClot = CType(myGlobal.SetDatos, String)
                                End If
                                'End If
                                PrepareLocalAlarmList(alarmLevelDetectionEnum, True, alarmList, alarmStatusList, addInfoVolMissingOrClot, alarmAdditionalInfoList, False) 'Inform about the volume missing warm
                            End If
                            'TR 01/10/2012 - END

                            'AG 14/09/2012 v052 - previous versions used "Not moreVolumeAvailable" as parameter pPreparationIDPerformedOK, it works well but was confused
                            'More readable code for v052 
                            'myGlobal = exec_delg.ProcessVolumeMissing(dbConnection, myPrepID, myRotorName, myBottlePos, Not moreVolumeAvailable, ActiveWorkSession, ActiveAnalyzer)
                            Dim prepPerformedOKFlag As Boolean = False
                            If (myWellStatus <> GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString AndAlso myWellStatus <> GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString) Then
                                prepPerformedOKFlag = True 'Means preparation performed OK but no more volume for other test preparations
                            End If

                            'TR 28/09/2012
                            Dim IsBottleLocked As Boolean = False
                            If (myBottleStatus = "LOCKED") Then
                                IsBottleLocked = True
                            ElseIf (myBottleStatus = String.Empty) Then
                                myBottleStatus = "DEPLETED"
                            End If
                            'TR 28/09/2012

                            'BT #1443 - Only when Bottle Status is DEPLETED, call the function to update the Status in the Reagents Historic Table
                            '#1627 - Add elementTubeContent = "REAGENT" 
                            If (myRotorName = "REAGENTS" AndAlso myBottleStatus = "DEPLETED") AndAlso elementTubeContent = "REAGENT" Then
                                Debug.Print("DETECTION FAILS: BOTTLE STATUS --> " & myBottleStatus.ToString())

                                Dim reagOnBoard As New ReagentsOnBoardDelegate
                                myGlobal = reagOnBoard.ReagentBottleManagement(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myBottlePos, _
                                                                               rcp_DS.twksWSRotorContentByPosition(0).BarCodeInfo, myBottleStatus, realVolume)
                                Debug.Print("CHECK THE TABLE NOW!!!")
                            End If


                            myGlobal = exec_delg.ProcessVolumeMissing(dbConnection, myPrepID, myRotorName, myBottlePos, prepPerformedOKFlag, ActiveWorkSession, ActiveAnalyzer, _
                                                                      reEvaluateNextToSend, IsBottleLocked, realVolume, testLeft)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                lockedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS) 'Executions locked due to the volume missing alarm 
                            End If
                            'AG 14/09/2012 v052

                            'AG 23/01/2012 - Prepare UIRefresh into rotor position
                            If (myRotorName = "SAMPLES") Then
                                newElementStatus = "NOPOS" 'Samples rotor does not allow multiple positions of the same element
                            ElseIf (newElementStatus = String.Empty) Then '¿??
                                'Assign  value: POS or NOPOS
                                If (moreVolumeAvailable) Then newElementStatus = "POS" Else newElementStatus = "NOPOS"
                            End If

                            'TR 28/09/2012 - Implement the variable myBottleStatus instead of DEPLETED.
                            uiRefreshMyGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, myRotorName, _
                                                                          myBottlePos, myBottleStatus, newElementStatus, 0, 0, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                            'TR 28/09/2012 - END
                            'AG 23/01/2012

                            'AG 04/06/2014 - #1653 Check if WRUN could not be completed and remove the last WRUN (for wash reagents) sent because it was not performed
                            '(Apply only for ANSBR1 + prepID =0 + status = LD)
                            If myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString AndAlso myPrepID = 0 AndAlso myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Then
                                Dim wrunSentLinq As List(Of AnalyzerManagerDS.sentPreparationsRow) = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
                                                                                                   Where a.ReagentWashFlag = True _
                                                                                                   Select a).ToList
                                If wrunSentLinq.Count > 0 Then
                                    wrunSentLinq(wrunSentLinq.Count - 1).Delete()
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                    reEvaluateNextToSend = True 'Search for the next instruction to send again
                                End If
                                wrunSentLinq = Nothing 'Release memory
                            End If
                            'AG 04/06/2014 -

                            'AG 04/10/2012 V053 Remove moreVolumeAvailable from condition - when LD is received Sw do not search for more position so variable moreVolumeAvailable has his
                            'initial declaration value (TRUE).
                            '1st time LD is received and no more positions Sw works well because the position is not still market as DEPLETED
                            '2on, 3rd, ... time Sw works bad because the "If initialRotorPositionStatus <> "DEPLETED" Then" condition not is valid and Sw enters here
                            '
                            'Required change 04/10/2012: 
                            '          -> Search another position if exists then mark execution as PENDING
                            ''AG 01/10/2012 - Temporally code until the MTEST instruction is not implemented
                            ''R2 bottle has been set to DEPLETED before executing code in previous IF (If initialRotorPositionStatus <> "DEPLETED" Then)
                            ''It works well only for the 1st execution with R2 missing and change his status from INPROCESS to PENDING
                            ''but it does not work well for the others executions using this position because they do not change their status
                            ''(add also case samples because duplicated sample tubes is possible with samples barcode)
                            ''
                            ''IMPORTANT NOTE: This code AG 01/10/2012 must be commented or redesigned once the MTEST instruction will be developped

                            'When LD but previous condition If initialRotorPositionStatus <> "DEPLETED" is FALSE execute this code:
                        ElseIf (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString OrElse myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) _
                                AndAlso myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Then

                            'When no more volume available (no more bottles with volume) set the flag moreVolumeAvailable = False
                            myGlobal = rcp_del.SearchOtherPosition(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, myRotorName, myBottlePos)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                rcp_DS = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                If rcp_DS.twksWSRotorContentByPosition.Count = 0 Then
                                    moreVolumeAvailable = False
                                Else
                                    moreVolumeAvailable = True
                                    If affectedExecutionID > -1 Then
                                        myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                                        If Not myGlobal.HasError Then
                                            lockedExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)
                                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions.Rows 'Implements loop for ISE executions
                                                If Not execRow.IsExecutionIDNull Then
                                                    'Change execution status from INPROCESS -> PENDING
                                                    reEvaluateNextToSend = True
                                                    myGlobal = exec_delg.UpdateStatusByExecutionID(dbConnection, "PENDING", execRow.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                                    'Prepare UIRefresh Dataset (EXECUTION_STATUS) for refresh screen when needed
                                                    If Not myGlobal.HasError Then
                                                        uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, execRow.ExecutionID, 0, Nothing, False)
                                                    End If
                                                End If
                                            Next
                                        End If
                                        lockedExecutionsDS.Clear()
                                    End If
                                End If
                            End If
                            'AG 04/10/2012 'AG 01/10/2012

                        End If

                        'Level detection fails (no diluted sample) in reactions rotors
                    ElseIf (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString()) Then
                        'Mark the execution (PTEST instruction) as locked
                        'myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                        'If Not myGlobal.HasError Then
                        'lockedExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)

                        If (Not lockedExecutionsDS Is Nothing) Then
                            If (lockedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                'Inform Diluted Sample Volume missing Alarm
                                PrepareLocalAlarmList(GlobalEnumerates.Alarms.DS_NO_VOLUME_WARN, True, alarmList, alarmStatusList, "", alarmAdditionalInfoList, False) 'Inform about the volume missing warm as "" in this case (with additionalinfoList)

                                'Dim myExecID As Integer = 0
                                'If Not lockedExecutionsDS.twksWSExecutions(0).IsExecutionIDNull Then myExecID = lockedExecutionsDS.twksWSExecutions(0).ExecutionID
                                'myGlobal = exec_delg.UpdateStatusByExecutionID(dbConnection, "LOCKED", myExecID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                myGlobal = exec_delg.UpdateStatusByExecutionID(dbConnection, "LOCKED", affectedExecutionID, ActiveWorkSession, ActiveAnalyzer)
                            End If
                        End If
                        'End If
                    End If
                    'AG 02/01/2012

                    'AG 07/06/2012 - Bottle or tube empty but no executions locked (exists more volume in other positions or wash the last execution)
                    '                Despite that the software has to inform about the depleted position
                ElseIf (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString _
                        OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString) AndAlso myLevelControl = 0 Then
                    'AG 13/07/2012 
                    'The volume missing alarms are only generated when each position becomes DEPLETED (initial position status <> 'DEPLETED' but current status = 'DEPLETED')
                    'If analyzer is bad adjusted Fw can send several instructions for the same position with S:R1 or S or R2 amd LevelControl=0. Sw not has to duplicate bottle depleted alarm!!!
                    If (initialRotorPositionStatus <> "DEPLETED") Then
                        'XBC 17/07/2012 - Estimated ISE Consumption by Firmware
                        'Every Samples volume alarm when ISE test operation increases PurgeA by firmware counter
                        If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then
                            'Dim myLogAccionesTmp As New ApplicationLogManager()    ' TO COMMENT !!!
                            'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 2) - Sample Volume Alarm !", "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!

                            Dim myExecutionsDS As New ExecutionsDS
                            Dim myExecutionType As String
                            myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)
                                If myExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
                                    myExecutionType = myExecutionsDS.twksWSExecutions(0).TestType

                                    'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 2) - Execution Type : " & myExecutionType, "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                                    If myExecutionType = "PREP_ISE" Then
                                        If Not MyClass.ISE_Manager Is Nothing Then
                                            MyClass.ISE_Manager.PurgeAbyFirmware += 1
                                            'myLogAccionesTmp.CreateLogActivity("Update Consumptions (Alarm 2) - PurgeA [" & MyClass.ISE_Manager.PurgeAbyFirmware.ToString & "]", "AnalyzerManager.ProcessstatusReceived", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        ' XBC 17/07/2012

                        Dim affectedExecutionID As Integer = -1
                        addInfoVolMissingOrClot = ""
                        myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, ActiveWorkSession, ActiveAnalyzer)
                        If Not myGlobal.HasError Then
                            lockedExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)
                            If (lockedExecutionsDS.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedExecutionsDS.twksWSExecutions(0).IsExecutionIDNull) Then
                                affectedExecutionID = lockedExecutionsDS.twksWSExecutions(0).ExecutionID

                                myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, ActiveAnalyzer, ActiveWorkSession, affectedExecutionID, myBottlePos, _
                                                                          alarmLevelDetectionEnum, reagentNumberWithNoVolume)
                                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                    addInfoVolMissingOrClot = CType(myGlobal.SetDatos, String)
                                End If
                            End If
                            lockedExecutionsDS.Clear() 'Clear dataset, because in this case no executions have been locked
                        End If

                        PrepareLocalAlarmList(alarmLevelDetectionEnum, True, alarmList, alarmStatusList, addInfoVolMissingOrClot, alarmAdditionalInfoList, False) 'Inform about the volume missing warm (with additionalinfoList)

                    End If
                    'AG 07/06/2012

                    'AG 20/07/2012 when the samples arm goes to photometric rotor to get diluted sample and dispense it into another well in photometric rotor
                    '                new FW has to return PS instead of S1, in this way until L:0 Sw knows the position value not applies to the samples rotor
                ElseIf myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.PS.ToString AndAlso myLevelControl = 0 Then
                    'Nothing to do

                End If

                '***********************************'
                '(2.4) COLLISION DETECTED (ALL ARMS)'
                '***********************************'
                Dim collisionAffectedExecutionsDS As New ExecutionsDS
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.KO.ToString) Then
                    'Implement business using the variables: myWellStatus, myPrepID,...
                    'If not already read it ... read the execution related to the status arm now
                    If (myExecutionID = -1) Then
                        myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            collisionAffectedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                            If (collisionAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                myExecutionID = collisionAffectedExecutionsDS.twksWSExecutions(0).ExecutionID
                            End If
                        End If
                    End If

                    'Mark execution as PENDING (or LOCKED if not volume)
                    If myExecutionID <> -1 AndAlso collisionAffectedExecutionsDS.twksWSExecutions(0).ExecutionStatus = "INPROCESS" Then
                        'AG 27/03/2012 - do not use myExecutionID ... when ise test several executions can share the same preparationID
                        For Each execRow As ExecutionsDS.twksWSExecutionsRow In collisionAffectedExecutionsDS.twksWSExecutions.Rows
                            myGlobal = exec_delg.ChangeINPROCESSStatusByCollision(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, execRow.ExecutionID)

                            'AG 08/03/2012 - Prepare UIRefresh Dataset (EXECUTION_STATUS, ROTORPOSITION_CHANGED) for refresh screen when needed
                            If Not myGlobal.HasError Then
                                myGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, execRow.ExecutionID, 0, Nothing, False)
                            End If

                            'Update rotor position status (SAMPLES: PENDING, INPROCESS, FINISHED)
                            If Not myGlobal.HasError Then
                                myGlobal = rcp_del.UpdateSamplePositionStatus(dbConnection, execRow.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                If Not myGlobal.HasError Then
                                    rcp_DS = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcp_DS.twksWSRotorContentByPosition.Rows
                                        myGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                             row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                        'AG 09/03/2012 - do not inform about ElementStatus (old code: '... row.Status, "POS", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")'

                                        If myGlobal.HasError Then Exit For
                                    Next
                                End If
                            End If

                            If myGlobal.HasError Then Exit For
                        Next
                        'AG 27/03/2012

                    End If
                    'In method ProcessREadingsReceived implements:
                    '... (OK) Do not save reading neither call calculations if ExecutionStatus = PENDING or LOCKED

                    'AG 20/07/2012 - Collision Arm alarm (Warning level) has not to be saved. Fw sends both Warning (ansbxx) / Error (Status)
                    'If Sw generates both alarms it seems like repeated alarm, so we save only the error level (but keep doing the alarm warning treatment)
                    'PrepareLocalAlarmList(alarmCollisionEnum, True, alarmList, alarmStatusList)
                End If


                '*********************************************'
                '(2.5) CLOT DETECTED (ONLY FOR SAMPLES ROTOR) '
                '*********************************************'
                Dim alarmClotDetectionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim clotAffectedExecutionsDS As New ExecutionsDS
                'AG 27/10/2011 - NOTE when required save CLOT alarm always but when CLOT detection is disabled do not treat it in ManageAlarms
                'If Not clotDetectionDisabled Then 'Treat field clot status only when clot detection is enabled
                If (myRotorName = "SAMPLES" AndAlso myClotStatus <> GlobalEnumerates.Ax00ArmClotDetectionValues.OK.ToString) Then
                    If (Not myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.CLOT_SYSTEM_ERR)) Then 'Ignore ANSBM clot warnings if clot system err alarm exists 
                        'Implement business using the variables: myWellStatus, myPrepID, myClotStatus,...
                        'If not already read it ... read the execution related to the status arm now
                        If (myExecutionID = -1) Then
                            myGlobal = exec_delg.GetExecutionByPreparationID(dbConnection, myPrepID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                clotAffectedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                                If (clotAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                    myExecutionID = clotAffectedExecutionsDS.twksWSExecutions(0).ExecutionID
                                End If
                            End If
                        End If

                        'Mark the execution with ClotValue = myClotStatus
                        If (myExecutionID <> -1) Then
                            'AG 27/03/2012 - do not use myExecutionID ... when ise test several executions can share the same preparationID
                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In clotAffectedExecutionsDS.twksWSExecutions.Rows
                                With execRow
                                    .BeginEdit()
                                    .ClotValue = myClotStatus
                                    .EndEdit()
                                End With
                            Next
                            'AG 27/03/2012
                            clotAffectedExecutionsDS.AcceptChanges()
                            myGlobal = exec_delg.UpdateClotValue(dbConnection, clotAffectedExecutionsDS)
                        End If

                        If (myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.BS.ToString) Then 'Blocked system 
                            alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                        ElseIf (myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CD.ToString Or myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CP.ToString) Then 'Clot Detected or Clot possible
                            alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_WARN
                        End If
                    End If

                ElseIf myRotorName = "SAMPLES" Then 'No clot found: If exists clear alarms 
                    'AG 25/07/2012 - these alarms has not OK
                    'alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                    'PrepareLocalAlarmList(alarmClotDetectionEnum, False, alarmList, alarmStatusList)
                    '
                    'alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_WARN
                    'PrepareLocalAlarmList(alarmClotDetectionEnum, False, alarmList, alarmStatusList)
                End If
                'End If 'If Not clotDetectionDisabled Then

                If alarmList.Count > 0 Then
                    'Finally call manage all alarms detected (new or fixed)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'myGlobal = ManageAlarms_SRV(dbConnection, alarmList, alarmStatusList)
                    Else
                        myGlobal = ManageAlarms(dbConnection, alarmList, alarmStatusList, alarmAdditionalInfoList)
                    End If
                End If

                'Finally if exists executions locked due volume missing inform them into database after inform the other alarms!!
                'Inform the locked executions list (prepLockedDS)
                Dim wsAnAlarmsDelg As New WSAnalyzerAlarmsDelegate
                If Not myGlobal.HasError AndAlso Not lockedExecutionsDS Is Nothing Then
                    If lockedExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
                        Dim alarmPrepLockedDS As New WSAnalyzerAlarmsDS  'AG 19/04/2011 - New DS with all preparation locked due this instruction
                        Dim i As Integer = 2 'AG 16/02/2012 - The prep_locked_warn starts with AlarmItem = 2 not 1 '1

                        'AG 18/06/2012 - Alarms tab show only one record "Some replicate has been locked due to volume missing"
                        'For Each item As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions.Rows
                        '    Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                        '    alarmRow = alarmPrepLockedDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                        '    With alarmRow
                        '        .AlarmID = GlobalEnumerates.Alarms.PREP_LOCKED_WARN.ToString
                        '        .AnalyzerID = AnalyzerIDAttribute
                        '        .AlarmDateTime = Now
                        '        If WorkSessionIDAttribute <> "" Then
                        '            .WorkSessionID = WorkSessionIDAttribute
                        '        Else
                        '            .SetWorkSessionIDNull()
                        '        End If
                        '        .AlarmStatus = True
                        '        .AlarmItem = i

                        '        'Create DATA with information of the locked execution (sample class, sampleid, test name, replicates,...)
                        '        myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, item.AnalyzerID, item.WorkSessionID, _
                        '                                                  item.ExecutionID, myBottlePos, _
                        '                                                  GlobalEnumerates.Alarms.NONE, reagentNumberWithNoVolume)

                        '        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        '            .AdditionalInfo = CType(myGlobal.SetDatos, String)
                        '        Else
                        '            .SetAdditionalInfoNull()
                        '        End If

                        '        .EndEdit()
                        '    End With
                        '    alarmPrepLockedDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)
                        '    i = i + 1

                        '    'AG 24/01/2012 - Prepare UIRefresh DS for WS status
                        '    uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, item.ExecutionID, 0, Nothing, False)

                        '    'AG 26/01/2012 - Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                        '    uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmRow.AlarmID.ToString, True)

                        'Next

                        Dim distinctOTlocked As List(Of Integer) = (From a As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions _
                                                                  Select a.OrderTestID Distinct).ToList
                        Dim firstExecutionLockedByOT As List(Of ExecutionsDS.twksWSExecutionsRow)
                        For Each itemOT As Integer In distinctOTlocked
                            firstExecutionLockedByOT = (From a As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDS.twksWSExecutions _
                                                        Where a.OrderTestID = itemOT Select a).ToList
                            If firstExecutionLockedByOT.Count > 0 Then
                                Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                                alarmRow = alarmPrepLockedDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                                With alarmRow
                                    .AlarmID = GlobalEnumerates.Alarms.PREP_LOCKED_WARN.ToString
                                    .AnalyzerID = AnalyzerIDAttribute
                                    .AlarmDateTime = Now
                                    If WorkSessionIDAttribute <> "" Then
                                        .WorkSessionID = WorkSessionIDAttribute
                                    Else
                                        .SetWorkSessionIDNull()
                                    End If
                                    .AlarmStatus = True
                                    .AlarmItem = i

                                    'Create DATA with information of the locked execution (sample class, sampleid, test name, replicates,...)
                                    myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, firstExecutionLockedByOT.First.AnalyzerID, firstExecutionLockedByOT.First.WorkSessionID, _
                                                                              firstExecutionLockedByOT.First.ExecutionID, myBottlePos, _
                                                                              GlobalEnumerates.Alarms.NONE, reagentNumberWithNoVolume)

                                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                        .AdditionalInfo = CType(myGlobal.SetDatos, String)
                                    Else
                                        .SetAdditionalInfoNull()
                                    End If

                                    .EndEdit()
                                End With
                                alarmPrepLockedDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                                'WS Status refresh updates only the OrderTest affected, so we have to generate 1 uiRefreshrow for every ordertest affected
                                'AG 24/01/2012 - Prepare UIRefresh DS for WS status
                                uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, firstExecutionLockedByOT.First.ExecutionID, 0, Nothing, False)

                                If i = 2 Then 'Alarms tab reload always all alarms, only inform the first 
                                    'AG 26/01/2012 - Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                                    uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmRow.AlarmID.ToString, True)
                                End If

                                i = i + 1
                            End If
                        Next
                        firstExecutionLockedByOT = Nothing
                        distinctOTlocked = Nothing
                        'AG 18/06/2012

                        alarmPrepLockedDS.AcceptChanges()
                        myGlobal = wsAnAlarmsDelg.Save(dbConnection, alarmPrepLockedDS, alarmsDefintionTableDS) 'AG 24/07/2012 - change Create for Save
                    End If
                End If 'If Not myGlobal.HasError Then

                'AG 26/07/2012 - clot affected executions - Biochemical 1 execution for each ANSBM1 instruction, ISE several executions for each ANSBM1 instruction
                If Not myGlobal.HasError AndAlso Not clotAffectedExecutionsDS Is Nothing AndAlso clotAffectedExecutionsDS.twksWSExecutions.Count > 0 Then
                    Dim alarmPrepWithClotDS As New WSAnalyzerAlarmsDS  'AG 19/04/2011 - New DS with all preparation locked due this instruction
                    Dim i As Integer = 1 'AG 16/02/2012 - The clot_detection_err or clot_detection_warn starts with AlarmItem = 1

                    For Each item As ExecutionsDS.twksWSExecutionsRow In clotAffectedExecutionsDS.twksWSExecutions.Rows
                        Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                        alarmRow = alarmPrepWithClotDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                        With alarmRow
                            .AlarmID = alarmClotDetectionEnum.ToString
                            .AnalyzerID = AnalyzerIDAttribute
                            .AlarmDateTime = Now
                            If WorkSessionIDAttribute <> "" Then
                                .WorkSessionID = WorkSessionIDAttribute
                            Else
                                .SetWorkSessionIDNull()
                            End If
                            .AlarmStatus = True
                            .AlarmItem = i

                            'Create DATA with information of the locked execution (sample class, sampleid, test name, replicates,...)
                            myGlobal = exec_delg.EncodeAdditionalInfo(dbConnection, item.AnalyzerID, item.WorkSessionID, _
                                                                      item.ExecutionID, myBottlePos, _
                                                                      GlobalEnumerates.Alarms.NONE, reagentNumberWithNoVolume)
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                .AdditionalInfo = CType(myGlobal.SetDatos, String)
                            Else
                                .SetAdditionalInfoNull()
                            End If
                            .EndEdit()
                        End With
                        alarmPrepWithClotDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                        'Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                        If i = 1 Then 'Alarms tab reload always all alarms, only inform the first 
                            uiRefreshMyGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmRow.AlarmID.ToString, True)
                        End If

                        i = i + 1
                    Next
                    alarmPrepWithClotDS.AcceptChanges()
                    myGlobal = wsAnAlarmsDelg.Save(dbConnection, alarmPrepWithClotDS, alarmsDefintionTableDS) 'AG 24/07/2012 - change Create for Save
                End If
                'AG 26/07/2012


                'AG 08/06/2012 - Finally if Sw has the new WS instruction to be sent to analyzer in Running. Evaluate if is a test preparation (test, ptest or isetest)
                'In this case check the execution found continues PENDING, otherwise look for another execution

                If myNextPreparationToSendDS.nextPreparation.Rows.Count > 0 Then
                    'AG 04/10/2012 - Following change is not necessary because if Rows.Count = 0 means the END instruction has been sent and the Sw has to sent START in order to receive new requests
                    'If myNextPreparationToSendDS.nextPreparation.Rows.Count > 0 OrElse executionTurnToPendingAgainFlag Then

                    'Set to FALSE again when next preparation found belong to a NOT PENDING execution
                    Dim lookForNewInstructionToBeSent As Boolean = False
                    If reEvaluateNextToSend Then
                        'AG 04/10/2012 - R1 or S or R2 level detection failed but exists additional position the execution status changes from INPROCESS to PENDING. 
                        '                So remove previous search next estimation a searc again
                        lookForNewInstructionToBeSent = True

                    Else
                        With myNextPreparationToSendDS

                            'Priority: (1) rejected well, (2) contaminated well, (3) ise test, (4) reagent contamination, (5) std test
                            If Not .nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull AndAlso Not .nextPreparation(0).CuvetteOpticallyRejectedFlag AndAlso _
                            Not .nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso Not .nextPreparation(0).CuvetteContaminationFlag Then 'No cuvette rejected neither contaminated
                                Dim lookForExecutionStatus As Boolean = False

                                If Not .nextPreparation(0).IsExecutionTypeNull AndAlso .nextPreparation(0).ExecutionType = "PREP_ISE" Then
                                    lookForExecutionStatus = True
                                ElseIf Not .nextPreparation(0).IsReagentContaminationFlagNull AndAlso .nextPreparation(0).ReagentContaminationFlag Then
                                    lookForExecutionStatus = False
                                ElseIf Not .nextPreparation(0).IsExecutionTypeNull AndAlso .nextPreparation(0).ExecutionType = "PREP_STD" Then
                                    lookForExecutionStatus = True
                                End If

                                If lookForExecutionStatus Then
                                    If Not .nextPreparation(0).IsExecutionIDNull AndAlso .nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myExecutionsDelg As New ExecutionsDelegate
                                        myGlobal = myExecutionsDelg.GetExecution(dbConnection, .nextPreparation(0).ExecutionID, AnalyzerIDAttribute, WorkSessionIDAttribute)
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            Dim localDS As New ExecutionsDS
                                            localDS = CType(myGlobal.SetDatos, ExecutionsDS)
                                            If localDS.twksWSExecutions.Rows.Count > 0 Then
                                                If localDS.twksWSExecutions(0).ExecutionStatus <> "PENDING" Then
                                                    lookForNewInstructionToBeSent = True 'Look for new exectuton
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End With
                    End If

                    If lookForNewInstructionToBeSent Then
                        Dim wellOffset As Integer = 0
                        If InStr(InstructionSentAttribute, "PTEST") > 0 Then 'Last instruction sent was a ptest
                            wellOffset = WELL_OFFSET_FOR_PREDILUTION
                        ElseIf InStr(InstructionSentAttribute, "ISETEST") > 0 Then 'Last instruction sent was an isetest
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
                        myGlobal = Me.SearchNextPreparation(dbConnection, futureRequestNextWell) 'Search for next instruction to be sent ... and sent it!!
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
                            myNextPreparationToSendDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)
                        End If
                    End If

                End If
                'AG 08/06/2012

                '   End If 'AG 29/06/2012 - Running Cycles lost - Solution!
                'End If'AG 29/06/2012 - Running Cycles lost - Solution!

                'Debug.Print("AnalyzerManager.ProcessArmStatusRecived (" & myInst.ToString & "):" & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 11/06/2012 - time estimation
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Treat ARM STATUS Received (" & myInst.ToString & "): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessArmStatusRecived", EventLogEntryType.Information, False)
                'AG 11/06/2012 - time estimation

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(String.Format("Message: {0} InnerException: {1}", ex.Message, ex.InnerException.Message), "AnalyzerManager.ProcessArmStatusRecived", EventLogEntryType.Error, False)
            End Try

            'AG 29/06/2012 - Running Cycles lost - Solution!
            ''We have used Exit Try so we have to sure the connection becomes properly closed here
            'If (Not dbConnection Is Nothing) Then
            '    If (Not myGlobal.HasError) Then
            '        'When the Database Connection was opened locally, then the Commit is executed
            '        DAOBase.CommitTransaction(dbConnection)
            '    Else
            '        'When the Database Connection was opened locally, then the Rollback is executed
            '        DAOBase.RollbackTransaction(dbConnection)
            '    End If
            '    dbConnection.Close()
            'End If

            Return myGlobal
        End Function


        ''' <summary>
        ''' SW has received and ANSINF instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG  07/04/2011
        ''' Modified by XB 29/01/2013 - change IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)
        '''             XB 30/01/2013 - Add Trace log information about malfunctions on temperature values (Bugs tracking #1122)
        ''' </remarks>
        Private Function ProcessInformationStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            ' XBC 24/05/2012 - Declare this flag Private for the class
            'Static Dim ISEAlreadyStarted As Boolean 'provisional flag for starting ISE just the first time SGM 28/03/2012

            Dim myGlobal As New GlobalDataTO

            Try


                Dim myUtilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim mySensors As New Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single) 'Local structure
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                ' Set Waiting Timer Current Instruction OFF
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'Get General cover (parameter index 3)
                Dim myIntValue As Integer = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.COVER_GENERAL, CSng(myIntValue))
                End If


                'Get Photometrics (Reaccions) cover (parameter index 4)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.COVER_REACTIONS, CSng(myIntValue))
                End If


                'Get Reagents (Fridge) cover (parameter index 5)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.COVER_FRIDGE, CSng(myIntValue))
                End If


                'Get Samples cover (parameter index 6)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.COVER_SAMPLES, CSng(myIntValue))
                End If


                'Get System liquid sensor (parameter index 7)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT, CSng(myIntValue))
                End If


                'Get Waste sensor (parameter index 8)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT, CSng(myIntValue))
                End If


                'Get Weight sensors (wash solution & high contamination waste) (parameter index 9, 10)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, CSng(myIntValue))
                End If

                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, CSng(myIntValue))
                End If


                'Get Temperature Reactions
                Dim mySingleValue As Single = 0
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'If IsNumeric(myInstParamTO.ParameterValue) Then    
                If Double.TryParse(myInstParamTO.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'mySingleValue = CSng(myInstParamTO.ParameterValue)
                    mySingleValue = myUtilities.FormatToSingle(myInstParamTO.ParameterValue)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS, mySingleValue)
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input Temperature value not valid [" & myInstParamTO.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
                End If

                'Get Temperature fridge
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'If IsNumeric(myInstParamTO.ParameterValue) Then
                If Double.TryParse(myInstParamTO.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'mySingleValue = CSng(myInstParamTO.ParameterValue)
                    mySingleValue = myUtilities.FormatToSingle(myInstParamTO.ParameterValue)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE, mySingleValue)
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input Temperature value not valid [" & myInstParamTO.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
                End If

                'Get Temperature Washing station heater
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 14)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'If IsNumeric(myInstParamTO.ParameterValue) Then
                If Double.TryParse(myInstParamTO.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'mySingleValue = CSng(myInstParamTO.ParameterValue)
                    mySingleValue = myUtilities.FormatToSingle(myInstParamTO.ParameterValue)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION, mySingleValue)
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input Temperature value not valid [" & myInstParamTO.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
                End If

                'Get Temperature R1 probe
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 15)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'If IsNumeric(myInstParamTO.ParameterValue) Then
                If Double.TryParse(myInstParamTO.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'mySingleValue = CSng(myInstParamTO.ParameterValue)
                    mySingleValue = myUtilities.FormatToSingle(myInstParamTO.ParameterValue)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1, mySingleValue)
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input Temperature value not valid [" & myInstParamTO.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
                End If

                'Get Temperature R2 probe
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 16)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                'If IsNumeric(myInstParamTO.ParameterValue) Then
                If Double.TryParse(myInstParamTO.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'mySingleValue = CSng(myInstParamTO.ParameterValue)
                    mySingleValue = myUtilities.FormatToSingle(myInstParamTO.ParameterValue)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2, mySingleValue)
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input Temperature value not valid [" & myInstParamTO.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
                End If

                'Get Fridge status (parameter index 12)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                    mySensors.Add(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS, CSng(myIntValue))
                End If

                'Get ISE status (parameter index 17)
                myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 17)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If


                ' XBC 03/05/2012 - Just for SERVICE, in Stressing mode no check ISE
                If myApplicationName.ToUpper.Contains("SERVICE") AndAlso IsStressingAttribute Then
                    ' Nothing to do
                Else
                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        myIntValue = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                        mySensors.Add(GlobalEnumerates.AnalyzerSensors.ISE_STATUS, CSng(myIntValue))

                        ''SGM 17/02/2012 ISE Module is switched On
                        If MyClass.ISE_Manager IsNot Nothing AndAlso MyClass.ISE_Manager.IsISEModuleInstalled Then
                            If Not MyClass.ISE_Manager.IsISEInitiatedOK OrElse Not MyClass.ISE_Manager.IsISEInitializationDone Then
                                If MyClass.AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then

                                    If myIntValue > 0 Then

                                        MyClass.ISE_Manager.IsISESwitchON = True

                                        'Acabar de revisar con SG una vez el Setup se haya generado
                                        ''AG 26/03/2012 - start ise initiation only in wup process and in connect process
                                        'If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS") _
                                        '  OrElse (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS") Then

                                        'SGM 14/06/2012
                                        If MyClass.ISE_Manager.IsPendingToInitializeAfterActivation Then
                                            ISEAlreadyStarted = True
                                        End If

                                        If Not ISEAlreadyStarted Then
                                            'SGM 28/03/2012
                                            'this condition must be replaced by the 
                                            'condition that allows the ISE module to start connecting
                                            If MyClass.ISE_Manager.IsISESwitchON And Not MyClass.ISE_Manager.IsLongTermDeactivation Then
                                                If Not MyClass.ISE_Manager.IsISEInitiating Then
                                                    MyClass.ISE_Manager.DoGeneralCheckings()
                                                    ISEAlreadyStarted = True
                                                End If
                                            Else
                                                'MyClass.ISE_Manager.IsISEInitiatedDone = True
                                                ISEAlreadyStarted = True
                                            End If

                                            'SGM 21/01/2013 - refresh ISE Alarms when ISE deactivated - Bug #1108
                                            If MyClass.ISE_Manager.IsLongTermDeactivation Then
                                                MyClass.RefreshISEAlarms()
                                            End If

                                        ElseIf Not MyClass.ISE_Manager.IsISEInitiating And Not MyClass.ISE_Manager.IsLongTermDeactivation Then

                                            'The ISE Module is switched On but its required that user connects from Utilities (Pending to implement)
                                            Dim myAlarmListTmp As New List(Of GlobalEnumerates.Alarms)
                                            Dim myAlarmStatusListTmp As New List(Of Boolean)
                                            Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                                            Dim myAlarmStatusList As New List(Of Boolean)

                                            If Not myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR) Then
                                                myAlarmList.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                                                myAlarmStatusList.Add(True)
                                            End If
                                            If Not myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                                                myAlarmList.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                                                myAlarmStatusList.Add(False)
                                            End If

                                            For i As Integer = 0 To myAlarmListTmp.Count - 1
                                                PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                                            Next

                                            If myAlarmList.Count > 0 Then
                                                myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                                If GlobalBase.IsServiceAssembly Then
                                                    ' XBC 17/10/2012 - Alarms treatment for Service
                                                    ' Not Apply
                                                    'myGlobal = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                                                Else
                                                    myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                                                End If
                                            End If


                                        End If
                                    Else
                                        MyClass.ISE_Manager.IsISESwitchON = False
                                        If Not ISEAlreadyStarted Then
                                            MyClass.RefreshISEAlarms()
                                        End If
                                        'If MyClass.Alarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                                        '    ISEAlreadyStarted = True
                                        'End If

                                        'SGM 09/11/2012 - ISE initiation is finished
                                        If Not ISEAlreadyStarted Then
                                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                            If GlobalBase.IsServiceAssembly Then
                                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED, 1, True)
                                            End If
                                        End If
                                        'SGM 09/11/2012

                                        ISEAlreadyStarted = True


                                    End If
                                End If
                            Else
                                If myIntValue = 0 Then
                                    MyClass.ISE_Manager.IsISESwitchON = False
                                End If
                            End If
                        End If

                    End If
                End If

                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ServiceSwAnsInfTreatment(mySensors)
                Else
                    UserSwANSINFTreatment(mySensors)
                End If

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    'Debug.Print("AnalyzerManager.ProcessInformationStatusReceived: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 11/06/2012 - time estimation
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Treat ANSINF received: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessInformationStatusReceived", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessInformationStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function




        ''' <summary>
        ''' SW has received and ANSCBR instruction 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <param name="pRotorType" >Input = "" , Output inform the rotor type read by the barcode reader</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 22/06/2011 
        ''' Modified by: SA 11/06/2013 - When load the WSRotorContentByPositionDS that has to be passed as parameter when calling function ManageBarcodeInstruction
        '''                              in BarcodeWSDelegate, inform field WSStatus with value of WorkSessionStatusAttribute (needed to update the WS Status from 
        '''                              OPEN to PENDING when the WS Required Elements are created after scanning the Samples Rotor -> new functionality added for
        '''                              LIS with ES)
        ''' </remarks>
        Private Function ProcessCodeBarInstructionReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef pRotorType As String) As GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim myGlobal As New GlobalDataTO

            Try
                'InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - the waiting timer is disabled on every reception process ('AG 18/07/2011)

                myGlobal = DAOBase.GetOpenDBTransaction(Nothing)

                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'AG 11/10/2011 - Check if the pInstructionReceived parameters are correct or not
                        Dim auxStr() As String = InstructionReceivedAttribute.Split(CChar(";"))
                        Dim parameterSeparatorsNumber As Integer = auxStr.Length - 1
                        If pInstructionReceived.Count <> parameterSeparatorsNumber Then
                            'There is some ';' character inside data. The instruction InstructionReceivedAttribute requires a new decode with additional business

                            'Get the samples barcode full total size configured
                            Dim settingsDlg As New UserSettingsDelegate
                            Dim sampleBarcodeFullTotal As Integer
                            myGlobal = settingsDlg.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_FULL_TOTAL.ToString())
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                sampleBarcodeFullTotal = CType(myGlobal.SetDatos, Integer)
                            End If

                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                Dim myLax00 As New LAX00Interpreter
                                myGlobal = myLax00.ReadWithAdditionalBusiness(InstructionReceivedAttribute, GlobalEnumerates.AppLayerInstrucionReception.ANSCBR, sampleBarcodeFullTotal)
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                    Dim myParameterTOList As New List(Of ParametersTO)
                                    myParameterTOList = CType(myGlobal.SetDatos, List(Of ParametersTO))

                                    Dim myInstruction As New Instructions
                                    myGlobal = myInstruction.GenerateReception(myParameterTOList)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        pInstructionReceived = DirectCast(myGlobal.SetDatos, List(Of InstructionParameterTO))
                                    End If
                                End If
                            End If

                        End If
                        'AG 11/10/2011

                        Dim myUtilities As New Utilities
                        Dim myInstParamTO As New InstructionParameterTO
                        Dim errorFlag As Boolean = True

                        'Get Reader selector (parameter index 3)
                        Dim rotorSelected As Integer = 0
                        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            If IsNumeric(myInstParamTO.ParameterValue) Then
                                rotorSelected = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                                errorFlag = False
                            End If
                        End If
                        If errorFlag = True Then
                            myGlobal.HasError = True
                            Exit Try
                        End If


                        ' Get status (parameter index 4)
                        Dim barCodeStatus As Integer = 0
                        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            If IsNumeric(myInstParamTO.ParameterValue) Then
                                barCodeStatus = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                                'Debug.Print("STATUS : " & barCodeStatus.ToString)
                                errorFlag = False
                            End If
                        End If
                        If errorFlag = True Then
                            myGlobal.HasError = True
                            Exit Try
                        End If


                        ' Get number of reads (parameter index 5)
                        Dim readsNumber As Integer = 0
                        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                            If IsNumeric(myInstParamTO.ParameterValue) Then
                                readsNumber = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                                errorFlag = False
                            End If
                        End If
                        If errorFlag = True Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                        Select Case barCodeStatus
                            Case GlobalEnumerates.Ax00AnsCodeBarStatus.CONFIG_DONE
                                'Configuration done OK
                                ' XBC 13/02/2012 - CODEBR Configuration instruction
                                'If myApplicationName.ToUpper.Contains("SERVICE") Then
                                'Service Sw
                                'TODO business

                                'Else
                                'User Sw
                                'If connection process in process then sent the general config instruction
                                'If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then

                                'Read the general configuration and sent the CONFIG instruction
                                'The analyzer answers with a Status instruction action = config done

                                If rotorSelected = GlobalEnumerates.Ax00CodeBarReader.SAMPLES Then
                                    ' When Codebar SAMPLES CONFIG_DONE is received then Codebar REAGENTS CONFIG instruction is sent
                                    Dim BarCodeDS As New AnalyzerManagerDS
                                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                                    rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                                    With rowBarCode
                                        .RotorType = "REAGENTS"
                                        .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                                        .Position = 0
                                    End With
                                    BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                    BarCodeDS.AcceptChanges()
                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)

                                ElseIf rotorSelected = GlobalEnumerates.Ax00CodeBarReader.REAGENTS Then

                                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                    If GlobalBase.IsServiceAssembly Then
                                        'Service Sw
                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True)
                                    Else
                                        'User Sw
                                        ' When Codebar REAGENTS CONFIG_DONE is received then General CONFIG instruction is sent
                                        If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString), "INPROCESS", False) = 0 Then
                                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True)
                                        ElseIf String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS", False) = 0 Then
                                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "CLOSED")
                                            myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True)
                                        End If
                                    End If

                                End If

                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    'Service Sw
                                    ' Nothing by now
                                Else
                                    'User Sw
                                    If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                        'AG + TR 03/02/2014 - BT #1490 (if change barcode config in sleep the action button becomes disable)
                                        ''SA + AG 11/12/2012 Not integrated in v1.0.0 
                                        ''If rotorSelected = GlobalEnumerates.Ax00CodeBarReader.SAMPLES Then 'After configurate SAMPLES barcode the SW configures automatically the REAGENTS barcode 
                                        'SetAnalyzerNotReady()
                                        ''End If
                                        ''SA + AG 11/12/2012 

                                        'Leave analyzer ready only in sleeping mode and after received reagents rotor
                                        If Not (AnalyzerStatusAttribute = AnalyzerManagerStatus.SLEEPING AndAlso rotorSelected = GlobalEnumerates.Ax00CodeBarReader.REAGENTS) Then
                                            SetAnalyzerNotReady()
                                        End If
                                        'AG + TR 03/02/2014
                                    End If
                                End If

                                'End If
                                'End If
                                ' XBC 13/02/2012 - CODEBR Configuration instruction



                            Case GlobalEnumerates.Ax00AnsCodeBarStatus.CODEBAR_ERROR
                                'Code bar reader error
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    'Service Sw
                                    'TODO business
                                    'Else
                                    'User Sw
                                    'TODO business
                                End If


                            Case GlobalEnumerates.Ax00AnsCodeBarStatus.FULL_ROTOR_DONE, _
                                GlobalEnumerates.Ax00AnsCodeBarStatus.SINGLE_POS_DONE, _
                                GlobalEnumerates.Ax00AnsCodeBarStatus.TEST_MODE_ANSWER, _
                                GlobalEnumerates.Ax00AnsCodeBarStatus.TEST_MODE_ENDED
                                ' XBC 28/03/2012 - Add Answers TEST_MODE_ANSWER, TEST_MODE_ENDED
                                'Code bar readings results ... save them into twksRotorContentByPosition table
                                Dim barCodeDS As New WSRotorContentByPositionDS
                                Dim row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
                                Const offset As Integer = 3

                                For itera As Integer = 0 To readsNumber - 1
                                    '''''''''
                                    'Get Data FIELDS
                                    ' Get number of reads (parameter index 6)
                                    Dim readPosition As Integer = 0
                                    myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6 + itera * offset)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                                        If IsNumeric(myInstParamTO.ParameterValue) Then
                                            readPosition = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                                            'Debug.Print("READ POS : " & readPosition.ToString)
                                            errorFlag = False
                                        End If
                                    End If
                                    If errorFlag = True Then
                                        myGlobal.HasError = True
                                        Exit For
                                    End If

                                    ' Get diagnostics (parameter index 7)
                                    Dim diagnostics As Integer = 0
                                    myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7 + itera * offset)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                                        If IsNumeric(myInstParamTO.ParameterValue) Then
                                            diagnostics = DirectCast(CInt(myInstParamTO.ParameterValue), Integer)
                                            errorFlag = False
                                        End If
                                    End If
                                    If errorFlag = True Then
                                        myGlobal.HasError = True
                                        Exit For
                                    End If

                                    ' Get value (parameter index 8)
                                    Dim value As String = ""
                                    myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8 + itera * offset)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                                        value = myInstParamTO.ParameterValue
                                        errorFlag = False
                                    End If
                                    If errorFlag = True Then
                                        myGlobal.HasError = True
                                        Exit For
                                    End If

                                    '''''''''
                                    'Add into dataset
                                    row = barCodeDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                    row.AnalyzerID = AnalyzerIDAttribute
                                    row.WorkSessionID = WorkSessionIDAttribute
                                    row.WSStatus = WorkSessionStatusAttribute  'SA 11/06/2013: Inform status of the active WorkSession
                                    row.CellNumber = readPosition
                                    If rotorSelected = GlobalEnumerates.Ax00CodeBarReader.REAGENTS Then
                                        row.RotorType = "REAGENTS"
                                    Else
                                        row.RotorType = "SAMPLES"
                                    End If
                                    pRotorType = row.RotorType 'Inform the byRef parameter!!

                                    row.SetBarCodeInfoNull()
                                    row.SetBarcodeStatusNull()
                                    row.SetScannedPositionNull()
                                    Select Case diagnostics
                                        Case GlobalEnumerates.Ax00AnsCodeBarDiagnosis.OK
                                            row.BarcodeStatus = "OK"
                                            row.BarCodeInfo = value
                                            row.ScannedPosition = True

                                        Case GlobalEnumerates.Ax00AnsCodeBarDiagnosis.NO_READ
                                            row.SetBarCodeInfoNull()
                                            row.BarcodeStatus = "EMPTY"
                                            row.SetScannedPositionNull()

                                        Case GlobalEnumerates.Ax00AnsCodeBarDiagnosis.BAD_CODE
                                            row.BarcodeStatus = "ERROR"
                                            row.ScannedPosition = True
                                            'row.BarCodeInfo = "" 'Clear the wrong barcode contents (use "" due in DAO this is the condition to set to NULL)
                                            row.BarCodeInfo = value 'Save the barcode read value

                                    End Select
                                    barCodeDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(row)

                                Next
                                barCodeDS.AcceptChanges()

                                Dim myRotorType As String = "SAMPLES"
                                Dim myCellNumber As Integer = -1    ' XBC 16/12/2011
                                Dim myStatus As String = ""
                                Dim myElementStatus As String = ""
                                Dim myBarcodeInfo As String = ""
                                Dim myBarcodeStatus As String = ""
                                Dim myScannedPosition As Boolean = Nothing
                                Dim myElementId As Integer = -1
                                Dim myMultiTubeNumber As Integer = -1
                                Dim myTubeType As String = ""
                                Dim myTubeContent As String = ""

                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    'Service Sw management

                                    ' XBC 16/12/2011
                                    'Inform all fields but RealVolume and TestLeft
                                    For Each row In barCodeDS.twksWSRotorContentByPosition.Rows

                                        If row.IsRotorTypeNull Then myRotorType = "" Else myRotorType = row.RotorType
                                        If row.IsCellNumberNull Then myCellNumber = -1 Else myCellNumber = row.CellNumber
                                        If row.IsStatusNull Then myStatus = "" Else myStatus = row.Status
                                        If row.IsElementStatusNull Then myElementStatus = "" Else myElementStatus = row.ElementStatus
                                        If row.IsBarCodeInfoNull Then myBarcodeInfo = "" Else myBarcodeInfo = row.BarCodeInfo
                                        If row.IsBarcodeStatusNull Then myBarcodeStatus = "" Else myBarcodeStatus = row.BarcodeStatus
                                        If row.IsScannedPositionNull Then myScannedPosition = Nothing Else myScannedPosition = row.ScannedPosition
                                        If row.IsElementIDNull Then myElementId = -1 Else myElementId = row.ElementID
                                        If row.IsMultiTubeNumberNull Then myMultiTubeNumber = -1 Else myMultiTubeNumber = row.MultiTubeNumber
                                        If row.IsTubeTypeNull Then myTubeType = "" Else myTubeType = row.TubeType
                                        If row.IsTubeContentNull Then myTubeContent = "" Else myTubeContent = row.TubeContent

                                        myGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ, _
                                                                            myRotorType, myCellNumber, myStatus, myElementStatus, _
                                                                            -1, -1, myBarcodeInfo, myBarcodeStatus, Nothing, Nothing, _
                                                                            myScannedPosition, myElementId, myMultiTubeNumber, myTubeType, myTubeContent)
                                    Next
                                    ' XBC 16/12/2011

                                Else
                                    'User Sw management
                                    'AG temporally uncommented 'Initial instruction testings
                                    'Update twksWSRotorContentByPosition barcode & barcode status
                                    'If barCodeDS.twksWSRotorContentByPosition.Rows.Count > 0 Then
                                    '    Dim rotorDelegate As New WSRotorContentByPositionDelegate
                                    '    myGlobal = rotorDelegate.UpdateBarCodeFields(dbConnection, barCodeDS, False)
                                    'End If
                                    'AG temporally uncommented

                                    'AG temporally commented when uncoment previous code
                                    Dim bcWSDelegate As New BarcodeWSDelegate
                                    'Dim myRotorType As String = "SAMPLES"
                                    If rotorSelected = GlobalEnumerates.Ax00CodeBarReader.REAGENTS Then myRotorType = "REAGENTS"
                                    myGlobal = bcWSDelegate.ManageBarcodeInstruction(dbConnection, barCodeDS, myRotorType)

                                    'Generate the UIRefresh DataSET (only for the positions read)
                                    If Not myGlobal.HasError Then
                                        Dim currentRotorTypeContentDS As New WSRotorContentByPositionDS
                                        Dim linqRes As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)

                                        currentRotorTypeContentDS = CType(myGlobal.SetDatos, WSRotorContentByPositionDS) 'The current whole rotor (by rotortype)

                                        'Dim myStatus As String = ""
                                        'Dim myElementStatus As String = ""
                                        'Dim myBarcodeInfo As String = ""
                                        'Dim myBarcodeStatus As String = ""
                                        'Dim myScannedPosition As Boolean = Nothing
                                        'Dim myElementId As Integer = -1
                                        'Dim myMultiTubeNumber As Integer = -1
                                        'Dim myTubeType As String = ""
                                        'Dim myTubeContent As String = ""

                                        'The UIRefresh event has only the rotor positions read by barcode
                                        For Each row In barCodeDS.twksWSRotorContentByPosition.Rows
                                            linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow _
                                                       In currentRotorTypeContentDS.twksWSRotorContentByPosition _
                                                       Where String.Compare(a.RotorType, row.RotorType, False) = 0 And a.CellNumber = row.CellNumber _
                                                       Select a).ToList


                                            If linqRes.Count > 0 Then
                                                If linqRes(0).IsStatusNull Then myStatus = "" Else myStatus = linqRes(0).Status
                                                If linqRes(0).IsElementStatusNull Then myElementStatus = "" Else myElementStatus = linqRes(0).ElementStatus
                                                If linqRes(0).IsBarCodeInfoNull Then myBarcodeInfo = "" Else myBarcodeInfo = linqRes(0).BarCodeInfo
                                                If linqRes(0).IsBarcodeStatusNull Then myBarcodeStatus = "" Else myBarcodeStatus = linqRes(0).BarcodeStatus
                                                If linqRes(0).IsScannedPositionNull Then myScannedPosition = Nothing Else myScannedPosition = linqRes(0).ScannedPosition
                                                If linqRes(0).IsElementIDNull Then myElementId = -1 Else myElementId = linqRes(0).ElementID
                                                If linqRes(0).IsMultiTubeNumberNull Then myMultiTubeNumber = -1 Else myMultiTubeNumber = linqRes(0).MultiTubeNumber
                                                If linqRes(0).IsTubeTypeNull Then myTubeType = "" Else myTubeType = linqRes(0).TubeType
                                                If linqRes(0).IsTubeContentNull Then myTubeContent = "" Else myTubeContent = linqRes(0).TubeContent

                                                'Inform all fields but RealVolume and TestLeft
                                                myGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ, _
                                                                                     linqRes(0).RotorType, linqRes(0).CellNumber, myStatus, myElementStatus, _
                                                                                      -1, -1, myBarcodeInfo, myBarcodeStatus, Nothing, Nothing, _
                                                                                      myScannedPosition, myElementId, myMultiTubeNumber, myTubeType, myTubeContent)
                                            End If
                                            If myGlobal.HasError Then Exit For
                                        Next
                                        linqRes = Nothing 'AG 02/08/2012 - free memory
                                    End If
                                    'AG temporally commented

                                End If 'If myApplicationName.ToUpper.Contains("SERVICE") Then

                        End Select

                        'XB + AG 05/03/2012 - Update analyzer session flags into DataBase
                        If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessCodeBarInstructionReceived", EventLogEntryType.Error, False)
            End Try


            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If

            Return myGlobal
        End Function


        ''' <summary>
        ''' The last sent preparation has been accepted by Analyzer
        ''' Insert the sent preparation into the twksWSPreparations table and also update 
        ''' the twksWSExecutions table (fields PreparationID and ExecutionStatus)
        '''         ''' </summary>
        ''' <param name="pDBConnection">Opened DB Connection</param>
        ''' <returns> GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 16/04/2010
        ''' Modified by: AG 17/05/2010 - Changes + using of connection template
        '''              AG 26/11/2010 - Prepare the UI_Refresh events
        '''              SA 02/07/2012 - Implementation changed to improve performance
        '''              SA 10/07/2012 - Inform SampleClass = "" when calling function GetAffectedISEExecutions in ExecutionsDelegate
        '''              AG 18/11/2013 - Increment the number of tests using the positions for sample and reagent2
        ''' </remarks>
        Private Function MarkPreparationAccepted(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                Dim myExecutionsDS As New ExecutionsDS
                Dim affectedExecutions As New ExecutionsDS
                Dim exeDelegate As New ExecutionsDelegate

                'If the Preparation to sent is an ISE one, get all affected Executions (all ISE Tests requested for the same 
                'Patient/Control and Sample Type 
                If (AppLayer.LastInstructionTypeSent = GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION) Then
                    myGlobal = exeDelegate.GetAffectedISEExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, AppLayer.LastExecutionIDSent, "", True)

                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    'Begin TRANSACTION to create the new PreparationID and update all affected Executions
                    myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                        If (Not dbConnection Is Nothing) Then
                            Dim myNextPreparationID As Integer
                            Dim myPreparationDS As New WSPreparationsDS
                            Dim myPrepDelegate As New WSPreparationsDelegate
                            Dim prepRow As WSPreparationsDS.twksWSPreparationsRow

                            'Generate the next PreparationID and insert the new Preparation
                            myGlobal = myPrepDelegate.GeneratePreparationID(dbConnection)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                myNextPreparationID = CType(myGlobal.SetDatos, Integer)

                                'Prepare data to insert the new Preparation
                                prepRow = myPreparationDS.twksWSPreparations.NewtwksWSPreparationsRow
                                prepRow.AnalyzerID = AnalyzerIDAttribute
                                prepRow.WorkSessionID = WorkSessionIDAttribute
                                prepRow.PreparationID = myNextPreparationID
                                prepRow.LAX00Data = AppLayer.LastPreparationInstructionSent
                                prepRow.PreparationStatus = "INPROCESS"

                                'AG 20/09/2012 v052 - inform the well where the reactions is dispensed on (when PTEST apply the correct offset factor)
                                prepRow.WellUsed = CurrentWellAttribute
                                If InStr(prepRow.LAX00Data, "PTEST") > 0 Then
                                    Dim reactRotorDlg As New ReactionsRotorDelegate
                                    prepRow.WellUsed = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)
                                End If

                                myPreparationDS.twksWSPreparations.AddtwksWSPreparationsRow(prepRow)

                                'Insert the new Preparation
                                myGlobal = myPrepDelegate.AddWSPreparation(dbConnection, myPreparationDS)
                            End If

                            If (Not myGlobal.HasError) Then
                                If (AppLayer.LastInstructionTypeSent = GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION) Then
                                    'Update fields ExecutionStatus and PreparationID for all Executions returned when called function GetAffectedISEExecutions
                                    For Each affectedExecution As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                        affectedExecution.BeginEdit()
                                        affectedExecution.ExecutionStatus = "INPROCESS"
                                        affectedExecution.PreparationID = myNextPreparationID
                                        affectedExecution.EndEdit()
                                    Next
                                Else
                                    'Prepare the DS needed to update fields ExecutionStatus and PreparationID for the informed Execution
                                    Dim exeRow As ExecutionsDS.twksWSExecutionsRow
                                    exeRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
                                    exeRow.ExecutionID = AppLayer.LastExecutionIDSent
                                    exeRow.ExecutionStatus = "INPROCESS"
                                    exeRow.PreparationID = myNextPreparationID
                                    myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(exeRow)
                                End If
                            End If

                            'AG 22/05/2014 #1634 - NOTE, NO CHANGES! Here we should set executions flags (ValidReadings = TRUE and CompleteReadings = TRUE) but
                            'method UpdateStatus is called in more places, so we decide to initiate these flags when 1st reading is received (method ProcessBiochemicalReadingsNEW)
                            'Flags ThermoWarningFlag and ClotValue will be reevaluate when readings are received too

                            'Update fields ExecutionStatus and PreparationID for all Executions in DS ExecutionsDS and Commit or Rollback the opened DB TRANSACTION
                            myGlobal = exeDelegate.UpdateStatus(dbConnection, myExecutionsDS)

                            'AG 28/05/2014 - #1644 - Do not delete readings here!! They will be removed when the new preparation receives his 1st reading
                            ''AG 02/10/2012 - When the execution is updated to INPROCESS remove all his readings (only for STD_TESTS)
                            'If Not myGlobal.HasError Then
                            '    Dim readingsDlg As New WSReadingsDelegate
                            '    myGlobal = readingsDlg.Delete(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myExecutionsDS)
                            'End If

                            If (Not myGlobal.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    'Notify to the application interfase the Status of some Executions has changed
                    For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                        myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                        If myGlobal.HasError Then Exit For
                    Next
                End If

                'Update status of all tubes of the Patient Sample/Control positioned in SAMPLES Rotor: PENDING, INPROCESS, FINISHED
                If (Not myGlobal.HasError) Then
                    Dim rcpDelegate As New WSRotorContentByPositionDelegate
                    myGlobal = rcpDelegate.UpdateSamplePositionStatus(Nothing, AppLayer.LastExecutionIDSent, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim rotorPosDS As WSRotorContentByPositionDS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)

                        For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
                            myGlobal = PrepareUIRefreshEventNum2(Nothing, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                 row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                            If (myGlobal.HasError) Then Exit For
                        Next
                    End If
                End If

                'AG 18/11/2013 - (#1385) Increment the number of tests that will be used positions samples / r2
                If Not myGlobal.HasError Then
                    Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                    myGlobal = inProcessDlg.IncrementInProcessTestsNumber(Nothing, AnalyzerIDAttribute, AppLayer.LastPreparationInstructionSent)
                End If

            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.MarkPreparationAccepted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            'Try

            '    myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)
            '    If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
            '        dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)
            '        If (Not dbConnection Is Nothing) Then
            '            '<Function Logic>

            '            '1ST: Insert record into preparations table
            '            '''''''''''''''''''''''''''''''''''''''''''
            '            Dim myPreparationDS As New WSPreparationsDS
            '            Dim myPrepDelegate As New WSPreparationsDelegate

            '            Dim prepRow As WSPreparationsDS.twksWSPreparationsRow
            '            prepRow = myPreparationDS.twksWSPreparations.NewtwksWSPreparationsRow

            '            'Fill the differents fields:
            '            'Get the next PreparationID
            '            myGlobal = myPrepDelegate.GeneratePreparationID(dbConnection)
            '            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
            '                prepRow.PreparationID = CType(myGlobal.SetDatos, Integer)
            '            Else
            '                Exit Try
            '            End If

            '            'Other fields
            '            prepRow.AnalyzerID = AnalyzerIDAttribute
            '            prepRow.WorkSessionID = WorkSessionIDAttribute
            '            'prepRow.LAX00Data = "AppLayer.LastPreparationInstructionSent" '(for testings)
            '            prepRow.LAX00Data = AppLayer.LastPreparationInstructionSent 'Get the Application layer attribute

            '            prepRow.PreparationStatus = "INPROCESS"
            '            myPreparationDS.twksWSPreparations.AddtwksWSPreparationsRow(prepRow)

            '            'Insert record
            '            myGlobal = myPrepDelegate.AddWSPreparation(dbConnection, myPreparationDS)
            '            If myGlobal.HasError Then Exit Try


            '            '2ON: Update Execution Status (INPROCESS)
            '            '''''''''''''''''''''''''''''''''''''''''
            '            Dim myExecutionsDS As New ExecutionsDS
            '            Dim exeRow As ExecutionsDS.twksWSExecutionsRow

            '            exeRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow

            '            'exeRow.ExecutionID = IdBuilder.NextId() (for testings)
            '            'exeRow.ExecutionID = 34
            '            exeRow.ExecutionID = AppLayer.LastExecutionIDSent  'Get the Application layer attribute

            '            exeRow.ExecutionStatus = prepRow.PreparationStatus
            '            exeRow.PreparationID = prepRow.PreparationID
            '            'AG 17/05/2010 - We cannt inform the baselineid in this point because the well used isnt known!!!

            '            myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(exeRow)

            '            Dim exeDelegate As New ExecutionsDelegate

            '            'AG 03/02/2011 - If an ISE TEST instruction is sent maybe several executions (same OrderTestId - RerunNumber - ReplicateNumber)
            '            'becomes to INPROCESS (Tested pending)
            '            If AppLayer.LastInstructionTypeSent = GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION Then
            '                myGlobal = exeDelegate.GetAffectedISEExecutions(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, AppLayer.LastExecutionIDSent)
            '                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
            '                    Dim affectedExecutions As New ExecutionsDS
            '                    affectedExecutions = CType(myGlobal.SetDatos, ExecutionsDS)

            '                    Dim pending As List(Of ExecutionsDS.twksWSExecutionsRow) = _
            '                        (From a In affectedExecutions.twksWSExecutions _
            '                         Where a.ExecutionStatus = "PENDING" Select a).ToList

            '                    If pending.Count > 0 Then
            '                        For Each item In pending
            '                            exeRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
            '                            exeRow.ExecutionID = item.ExecutionID
            '                            exeRow.PreparationID = prepRow.PreparationID 'AG 30/11/2011 - several ise executions are performed using the same preparationid
            '                            exeRow.ExecutionStatus = prepRow.PreparationStatus '"INPROCESS"
            '                            myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(exeRow)
            '                        Next
            '                    End If
            '                End If
            '            End If
            '            'AG 03/02/2011

            '            myGlobal = exeDelegate.UpdateStatus(dbConnection, myExecutionsDS)

            '            'AG 26/11/2010 - Prepare data for generate communications UI event
            '            If Not myGlobal.HasError Then
            '                'AG 08/02/2011 - Using ISE several executions can become INPROCESS at the same time
            '                'myUI_RefreshDS.ExecutionStatusChanged.Clear() 'AG 27/02/2012 (comment line) Clear current DS table contents 
            '                For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
            '                    myGlobal = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
            '                    If myGlobal.HasError Then Exit For
            '                Next
            '                'END AG 08/02/2011
            '            End If
            '            'END AG 26/11/2010

            '            'Update rotor position status (SAMPLES: PENDING, INPROCESS, FINISHED)
            '            If Not myGlobal.HasError Then
            '                Dim rcp_del As New WSRotorContentByPositionDelegate
            '                myGlobal = rcp_del.UpdateSamplePositionStatus(dbConnection, exeRow.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
            '                If Not myGlobal.HasError Then
            '                    Dim rotorPosDS As New WSRotorContentByPositionDS
            '                    rotorPosDS = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)
            '                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
            '                        myGlobal = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
            '                                                             row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
            '                        'AG 09/03/2012 - do not inform about ElementStatus (old code: '... row.Status, "POS", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")'

            '                        If myGlobal.HasError Then Exit For
            '                    Next
            '                End If
            '            End If

            '            If (Not myGlobal.HasError) Then
            '                'When the Database Connection was opened locally, then the Commit is executed
            '                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

            '            Else
            '                'When the Database Connection was opened locally, then the Rollback is executed
            '                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            '            End If

            '        End If
            '    End If

            'Catch ex As Exception
            '    myGlobal.HasError = True
            '    myGlobal.ErrorCode = "SYSTEM_ERROR"
            '    myGlobal.ErrorMessage = ex.Message

            '    Dim myLogAcciones As New ApplicationLogManager()
            '    myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.MarkPreparationAccepted", EventLogEntryType.Error, False)
            'End Try

            ''We have used Exit Try so we have to sure the connection becomes properly closed here
            'If myGlobal.HasError Then
            '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
            '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            'End If


            Return myGlobal

        End Function


        ''' <summary>
        ''' After send a wash well (cuvette) due a well contaminated inform into twksWSReactionsRotor the washedFlag = True
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 07/06/2011
        ''' AG 18/11/2013 - Increment the number of tests using the positions for arm reagent2 washings
        ''' </remarks>
        Public Function MarkWashWellContaminationRunningAccepted(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReactionsDS As New ReactionsRotorDS
                        Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
                        newRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                        With newRow
                            .AnalyzerID = AnalyzerIDAttribute
                            .WellNumber = wellContaminatedWithWashSent
                            .SetRotorTurnNull()
                            .SetWellContentNull()
                            .SetWellStatusNull()
                            .SetExecutionIDNull()
                            .SetRejectedFlagNull()
                            .SetCurrentTurnFlagNull()
                            .WashedFlag = True
                            .SetWashRequiredFlagNull()
                        End With
                        myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
                        myReactionsDS.AcceptChanges()

                        Dim reactionsDelegate As New ReactionsRotorDelegate
                        resultData = reactionsDelegate.Update(dbConnection, myReactionsDS, True)

                        'AG 18/11/2013 - (#1385) Increment the number of tests that will be used positions for arm R2 washing
                        If Not resultData.HasError Then
                            Debug.print("Step 3 - MarkWashWellContaminationRunningAccepted -> LastPreparationInstructionSent = " & applayer.LastPreparationInstructionSent)

                            Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                            resultData = inProcessDlg.IncrementInProcessTestsNumber(Nothing, AnalyzerIDAttribute, AppLayer.LastPreparationInstructionSent)
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.MarkWashWellContaminationRunningAccepted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add the last exported results into the object attribute
        ''' </summary>
        ''' <param name="pExecutionsDS"></param>
        ''' <remarks>AG 19/03/2013</remarks>
        Private Sub AddIntoLastExportedResults(ByVal pExecutionsDS As ExecutionsDS)
            Try
                'Move row from parameter to lastExportedResultsDSAttribute
                SyncLock lockThis
                    For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions.Rows
                        lastExportedResultsDSAttribute.twksWSExecutions.ImportRow(row)
                    Next
                    lastExportedResultsDSAttribute.twksWSExecutions.AcceptChanges()
                End SyncLock

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.AddIntoLastExportedResults", EventLogEntryType.Error, False)

            End Try
        End Sub
#End Region

#Region "UIRefresh DataSet Methods"

        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#1)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pExecutionID">Used when new execution status changes or new calculation is performed</param>
        ''' <param name="pReadingNumber">Used when new readings are received</param>
        ''' <param name="pAlarmID">Used when alarms occurs or solved</param>
        ''' <param name="pAlarmStatus" >Used when alarms occurs (TRUE) or is solved (FALSE)</param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>25/11/2010 AG: EXECUTION_STATUS case
        ''' AG 09/02/2011 - add NEW READINGS RECEIVED case 
        ''' AG 14/03/2011 - add NEW_ALARMS_RECEIVED case 
        ''' AG 22/05/2014 - #1637 Reorder code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEvent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUI_EventType As GlobalEnumerates.UI_RefreshEvents, _
                                               ByVal pExecutionID As Integer, ByVal pReadingNumber As Integer, _
                                               ByVal pAlarmID As String, ByVal pAlarmStatus As Boolean) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myglobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event

                        'AG 22/05/2014 - #1637 reorder code to use easily SyncLock
                        'Next code was inside Select Case
                        Dim local_DS As New UIRefreshDS
                        If pUI_EventType = UI_RefreshEvents.EXECUTION_STATUS OrElse pUI_EventType = UI_RefreshEvents.RESULTS_CALCULATED Then
                            Dim exeDelegate As New ExecutionsDelegate
                            myglobal = exeDelegate.GetExecutionStatusChangeInfo(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, pExecutionID)
                            If Not myglobal.HasError And Not myglobal.SetDatos Is Nothing Then
                                local_DS = CType(myglobal.SetDatos, UIRefreshDS)
                            End If
                        End If
                        'AG 22/05/2014

                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                        Select Case pUI_EventType
                            'Case (execution sent), (execution calculated)
                            Case GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED
                                If Not myglobal.HasError And Not myglobal.SetDatos Is Nothing Then
                                    'Import new rows into the general class DataSet
                                    If local_DS.ExecutionStatusChanged.Rows.Count > 0 Then
                                        'AG 14/03/2014 - #1533
                                        'For Each row As UIRefreshDS.ExecutionStatusChangedRow In local_DS.ExecutionStatusChanged.Rows
                                        '    myUI_RefreshDS.ExecutionStatusChanged.ImportRow(row)
                                        'Next
                                        SyncLock myUI_RefreshDS.ExecutionStatusChanged 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                            myUI_RefreshDS.ExecutionStatusChanged.Merge(local_DS.ExecutionStatusChanged)
                                            'AG 14/03/2014 - #1533
                                            myUI_RefreshDS.ExecutionStatusChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                        End SyncLock
                                    End If
                                End If

                                'Case (new result instruction has been received)
                            Case GlobalEnumerates.UI_RefreshEvents.READINGS_RECEIVED
                                Dim myNewReadRow As UIRefreshDS.ReceivedReadingsRow
                                SyncLock myUI_RefreshDS.ReceivedReadings 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                    myNewReadRow = myUI_RefreshDS.ReceivedReadings.NewReceivedReadingsRow
                                    With myNewReadRow
                                        .BeginEdit()
                                        .WorkSessionID = WorkSessionIDAttribute
                                        .AnalyzerID = AnalyzerIDAttribute
                                        .ExecutionID = pExecutionID
                                        .ReadingNumber = pReadingNumber
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.ReceivedReadings.AddReceivedReadingsRow(myNewReadRow)
                                    myUI_RefreshDS.ReceivedReadings.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock

                                'Case (new alarms instruction has been received)
                            Case GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED
                                Dim myNewAlarmRow As UIRefreshDS.ReceivedAlarmsRow
                                SyncLock myUI_RefreshDS.ReceivedAlarms 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                    myNewAlarmRow = myUI_RefreshDS.ReceivedAlarms.NewReceivedAlarmsRow
                                    With myNewAlarmRow
                                        .BeginEdit()
                                        .WorkSessionID = WorkSessionIDAttribute
                                        .AnalyzerID = AnalyzerIDAttribute
                                        .AlarmID = pAlarmID
                                        .AlarmStatus = pAlarmStatus
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.ReceivedAlarms.AddReceivedAlarmsRow(myNewAlarmRow)
                                    myUI_RefreshDS.ReceivedAlarms.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                            Case Else

                        End Select

                        'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                    End If
                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEvent", EventLogEntryType.Error, False)
                'Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function


        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#2)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pRotorType" ></param>
        ''' <param name="pCellNumber" ></param>
        ''' <param name="pStatus" ></param>
        ''' <param name="pElementStatus" ></param>
        ''' <param name="pRealVolume" ></param>
        ''' <param name="pTestsLeft" ></param>
        ''' <param name="pBarCodeInfo" ></param>
        ''' <param name="pBarCodeStatus" ></param>
        ''' <param name="pSensorID" ></param>
        ''' <param name="pSensorValue" ></param>
        ''' <param name="pScannedPosition" ></param>
        ''' <param name="pElementID" ></param>
        ''' <param name="pMultiTubeNumber" ></param>
        ''' <param name="pTubeType" ></param>
        ''' <param name="pTubeContent" ></param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>25/11/2010 AG 
        ''' AG 30/03/2011 - add case ROTORPOSITION_CHANGED (use new method signature)
        ''' add case SENSORVALUE_CHANGED 
        ''' AG 22/09/2011 - add case BARCODE_POSITION_READ
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventNum2(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUI_EventType As GlobalEnumerates.UI_RefreshEvents, _
                                               ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pStatus As String, ByVal pElementStatus As String, _
                                               ByVal pRealVolume As Single, ByVal pTestsLeft As Integer, ByVal pBarCodeInfo As String, ByVal pBarCodeStatus As String, _
                                               ByVal pSensorId As GlobalEnumerates.AnalyzerSensors, ByVal pSensorValue As Single, _
                                               ByVal pScannedPosition As Boolean, ByVal pElementID As Integer, ByVal pMultiTubeNumber As Integer, _
                                               ByVal pTubeType As String, ByVal pTubeContent As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                Select Case pUI_EventType
                    'Case (some rotor positions status, volume, ... has changed)
                    Case GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ
                        Dim myNewRotorPositionRow As UIRefreshDS.RotorPositionsChangedRow
                        'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                        SyncLock myUI_RefreshDS.RotorPositionsChanged
                            myNewRotorPositionRow = myUI_RefreshDS.RotorPositionsChanged.NewRotorPositionsChangedRow
                            With myNewRotorPositionRow
                                .BeginEdit()
                                .WorkSessionID = WorkSessionIDAttribute
                                .AnalyzerID = AnalyzerIDAttribute
                                .RotorType = pRotorType
                                .CellNumber = pCellNumber
                                If pStatus <> "" Then .Status = pStatus Else .SetStatusNull()
                                If String.Compare(pElementStatus, "", False) <> 0 Then .ElementStatus = pElementStatus Else .SetElementStatusNull()
                                If pRealVolume <> -1 Then .RealVolume = pRealVolume Else .SetRealVolumeNull()
                                If pTestsLeft <> -1 Then .RemainingTestsNumber = pTestsLeft Else .SetRemainingTestsNumberNull()
                                If pBarCodeInfo <> "" Then .BarCodeInfo = pBarCodeInfo Else .SetBarCodeInfoNull()
                                If pBarCodeStatus <> "" Then .BarcodeStatus = pBarCodeStatus Else .SetBarcodeStatusNull()
                                If pScannedPosition = Nothing Then .SetScannedPositionNull() Else .ScannedPosition = pScannedPosition
                                If pElementID <> -1 Then .ElementID = pElementID Else .SetElementIDNull()
                                If pMultiTubeNumber <> -1 Then .MultiTubeNumber = pMultiTubeNumber Else .SetMultiTubeNumberNull()
                                If pTubeType <> "" Then .TubeType = pTubeType Else .SetTubeTypeNull()
                                If pTubeContent <> "" Then .TubeContent = pTubeContent Else .SetTubeContentNull()
                                .EndEdit()
                            End With
                            myUI_RefreshDS.RotorPositionsChanged.AddRotorPositionsChangedRow(myNewRotorPositionRow)
                            myUI_RefreshDS.RotorPositionsChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                        End SyncLock

                    Case GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED
                        'This case prepare DS 
                        'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                        If pSensorId <> Nothing Then
                            Dim myNewSensorChangeRow As UIRefreshDS.SensorValueChangedRow

                            'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                            Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)
                            SyncLock myUI_RefreshDS.SensorValueChanged
                                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In myUI_RefreshDS.SensorValueChanged _
                                          Where String.Compare(a.SensorID, pSensorId.ToString, False) = 0 _
                                          Select a).ToList

                                If lnqRes.Count > 0 Then
                                    lnqRes(0).BeginEdit()
                                    lnqRes(0).Value = pSensorValue
                                    lnqRes(0).EndEdit()
                                    lnqRes(0).AcceptChanges()

                                Else
                                    ' insert new value
                                    myNewSensorChangeRow = myUI_RefreshDS.SensorValueChanged.NewSensorValueChangedRow
                                    With myNewSensorChangeRow
                                        .BeginEdit()
                                        .SensorID = pSensorId.ToString
                                        .Value = pSensorValue
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.SensorValueChanged.AddSensorValueChangedRow(myNewSensorChangeRow)
                                    myUI_RefreshDS.SensorValueChanged.AcceptChanges()
                                End If
                            End SyncLock
                            lnqRes = Nothing 'AG 02/08/2012 - free memory
                            ' XBC 26/10/2011
                        End If

                    Case Else

                End Select
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventNum2", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function


        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#3)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pReactionsRotorWellDS" ></param>
        ''' <param name="pMainThreadIsUsedFlag">TRUE indicates instruction treatment is using the main thread (arm status instructions, change status business is executed) 
        '''                                     FALSE indicates instruction treatment is using a secondary thread (ANSPHR used for well rejection)</param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>
        ''' AG 03/06/2011 - add case REACTIONS_WELL_STATUS_CHANGED (use new method signature)
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventNum3(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUI_EventType As GlobalEnumerates.UI_RefreshEvents, _
                                               ByVal pReactionsRotorWellDS As ReactionsRotorDS, ByVal pMainThreadIsUsedFlag As Boolean) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'AG 03/07/2012 - Running Cycles lost - Solution!
                If Not pReactionsRotorWellDS Is Nothing Then

                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'If pMainThreadIsUsedFlag Then
                    '    eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    '    If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                    'Else
                    '    secondaryEventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    '    If Not mySecondaryUI_RefreshEvent.Contains(pUI_EventType) Then mySecondaryUI_RefreshEvent.Add(pUI_EventType)
                    'End If
                    eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                    'AG 22/05/2014 - #1637

                    'Case (some reactions rotor well ... has changed)
                    Dim myNewRow As UIRefreshDS.ReactionWellStatusChangedRow
                    For Each row As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionsRotorWellDS.twksWSReactionsRotor.Rows

                        'AG 22/05/2014 - #1637 Clear code. Comment dead code
                        'If pMainThreadIsUsedFlag Then
                        '    myNewRow = myUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow
                        'Else
                        '    myNewRow = mySecondaryUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow
                        'End If
                        'AG 22/05/2014 - #1637

                        'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                        SyncLock myUI_RefreshDS.ReactionWellStatusChanged
                            myNewRow = myUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow

                            With myNewRow
                                .BeginEdit()
                                If Not row.IsAnalyzerIDNull Then .AnalyzerID = row.AnalyzerID
                                If Not row.IsWellNumberNull Then .WellNumber = row.WellNumber
                                If Not row.IsRotorTurnNull Then .RotorTurn = row.RotorTurn
                                If Not row.IsWellContentNull Then .WellContent = row.WellContent
                                If Not row.IsWellStatusNull Then .WellStatus = row.WellStatus
                                If Not row.IsExecutionIDNull Then .ExecutionID = row.ExecutionID
                                If Not row.IsRejectedFlagNull Then .RejectedFlag = row.RejectedFlag
                                .EndEdit()
                            End With

                            'AG 22/05/2014 - #1637 Clear code. Comment dead code
                            'If pMainThreadIsUsedFlag Then
                            '    myUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'Else
                            '    mySecondaryUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'End If
                            myUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'AG 22/05/2014 - #1637

                            myUI_RefreshDS.ReactionWellStatusChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                        End SyncLock

                    Next

                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'If pMainThreadIsUsedFlag Then
                    '    myUI_RefreshDS.AcceptChanges()
                    'Else
                    '    mySecondaryUI_RefreshDS.AcceptChanges()
                    'End If
                    'AG 22/05/2014 - #1637 Clear code. Comment dead code

                    'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                End If

                'AG 03/07/2012 - Running Cycles lost - Solution!
                'End If
                'End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventNum3", EventLogEntryType.Error, False)
                'Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            'If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()'AG 03/07/2012 - Running Cycles lost - Solution!
            Return myglobal
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWS"></param>
        ''' <param name="pOrderList"></param>
        ''' <returns></returns>
        ''' <remarks> Created ??
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventNum4(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUI_EventType As GlobalEnumerates.UI_RefreshEvents, _
                                               ByVal pAnalyzerID As String, ByVal pWS As String, ByVal pOrderList As List(Of String)) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'If (Not dbConnection Is Nothing) Then
                eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                Dim myNewRow As UIRefreshDS.AutoReportRow

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.AutoReport
                    For Each order As String In pOrderList
                        myNewRow = myUI_RefreshDS.AutoReport.NewAutoReportRow()
                        With myNewRow
                            .BeginEdit()
                            .AnalyzerID = pAnalyzerID
                            .OrderID = order
                            '.OrderTestID = cal??
                            .WorkSessionID = pWS
                            '.RerunNumber = cal??
                            .EndEdit()
                        End With
                        myUI_RefreshDS.AutoReport.AddAutoReportRow(myNewRow)
                        myUI_RefreshDS.AutoReport.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                    Next
                End SyncLock
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventNum4", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Clear the user interface refresh information but with functionality for lock multithreads
        ''' </summary>
        ''' <param name="pMultiThreadLock">Use SyncLock or not</param>
        ''' <param name="pForceClear">Clear always or only if no refresh to perform</param>
        ''' <remarks>AG 22/05/2014 - #1637</remarks>
        Private Sub ClearRefreshDataSets(ByVal pMultiThreadLock As Boolean, ByVal pForceClear As Boolean)
            Try
                If pMultiThreadLock Then
                    SyncLock lockThis
                        If pForceClear Then 'Clear always
                            myUI_RefreshEvent.Clear()
                            myUI_RefreshDS.Clear()
                        ElseIf myUI_RefreshEvent.Count = 0 Then 'Clear only if no refresh to perform
                            myUI_RefreshDS.Clear()
                        End If
                    End SyncLock
                Else
                    If pForceClear Then
                        myUI_RefreshEvent.Clear()
                        myUI_RefreshDS.Clear()
                    ElseIf myUI_RefreshEvent.Count = 0 Then
                        myUI_RefreshDS.Clear()
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ClearRefreshDataSets", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "ISE (Send and Receive Results)"

        ''' <summary>
        ''' Method incharge to process all Received ISE Results.
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 03/01/2010
        ''' Modified by: SA 12/06/2014 - BT #1660 ==> Replaced call to function ProcessISETESTResults in ISEReception class, for its new 
        '''                                           version (ProcessISETESTResultsNEW) 
        ''' </remarks>
        Public Function ProcessRecivedISEResult(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'AG 03/07/2012 - Running Cycles lost - Solution!
                'myGlobalDataTO = DAOBase.GetOpenDBTransaction(Nothing)
                'If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                '    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                Dim myPreparationID As Integer = -1
                Dim myISEResultStr As String = ""
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                myPreparationID = CInt((From a In pInstructionReceived Where a.ParameterIndex = 3 Select a.ParameterValue).First)
                myISEResultStr = (From a In pInstructionReceived Where a.ParameterIndex = 4 Select a.ParameterValue).First()

                Dim myISEResult As New ISEResultTO
                myISEResult.ReceivedResults = myISEResultStr


                'SGM 08/03/11 Get from SWParameters table
                Dim myISEMode As String = "SimpleMode"
                Dim myParams As New SwParametersDelegate
                myGlobalDataTO = myParams.ReadTextValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.ISE_MODE.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myISEMode = CStr(myGlobalDataTO.SetDatos)
                Else
                    myISEMode = "SimpleMode"
                End If

                'XBC 20/01/2012
                'Dim myISEResultsDelegate As New ISEResultsDelegate
                Dim myISEResultsDelegate As New ISEReception(Me)
                'XBC 20/01/2012
                'SGM 10/01/2012 choose method depending on instruction type sent (ISECMD or ISETEST)

                If myPreparationID = 0 Then

                    MyClass.ISE_Manager.LastISEResult = New ISEResultTO

                    'ISECMD answer
                    Dim myCurrentProcedure As ISEManager.ISEProcedures = MyClass.ISE_Manager.CurrentProcedure
                    myGlobalDataTO = myISEResultsDelegate.ProcessISECMDResults(myISEResult, AnalyzerIDAttribute, MyClass.ISE_Manager.IsBiosystemsValidationMode)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        MyClass.ISE_Manager.LastISEResult = CType(myGlobalDataTO.SetDatos, ISEResultTO)
                        'SGM 17/02/2012 ISE Communications Ok
                        If MyClass.ISE_Manager IsNot Nothing Then
                            If MyClass.ISE_Manager.IsCommErrorDetected Then
                                MyClass.ISE_Manager.IsISECommsOk = False
                                MyClass.ISE_Manager.IsCommErrorDetected = False
                            Else
                                MyClass.ISE_Manager.IsISECommsOk = True
                            End If
                        End If
                        'end SGM 17/02/2012
                    Else
                        'SGM 11/05/2012
                        Dim myISEResultWithErrors As ISEResultTO = New ISEResultTO
                        myISEResultWithErrors.ISEResultType = ISEResultTO.ISEResultTypes.SwError
                        MyClass.ISE_Manager.LastISEResult = myISEResultWithErrors
                    End If

                    'SGM 27/07/2012 - Activate alarmas related to ERC errors (or CALB results error)
                    If Not MyClass.ISE_Manager.IsInUtilities Then
                        If ISE_Manager.LastISEResult IsNot Nothing Then
                            If ISE_Manager.LastISEResult.IsCancelError Then
                                Select Case ISE_Manager.LastISEResult.Errors(0).ErrorCycle
                                    Case ISEErrorTO.ErrorCycles.Calibration, ISEErrorTO.ErrorCycles.PumpCalibration, ISEErrorTO.ErrorCycles.BubbleDetCalibration
                                        MyClass.ActivateAnalyzerISEAlarms(MyClass.ISE_Manager.LastISEResult)
                                End Select
                            ElseIf ISE_Manager.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Then
                                If ISE_Manager.LastISEResult.Errors.Count > 0 Then
                                    For Each E As ISEErrorTO In ISE_Manager.LastISEResult.Errors
                                        If myCurrentProcedure = ISEManager.ISEProcedures.CalibrateElectrodes Then
                                            MyClass.BlockISEPreparationByElectrode(dbConnection, MyClass.ISE_Manager.LastISEResult, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                        End If
                                    Next
                                End If
                            End If

                        End If


                    End If

                    'AG 12/01/2012 - Prepare UI Refresh
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED, 1, True)

                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        'SERVICE SOFTWARE 01/07/2011
                        'TODO

                    Else
                        'USER SOFTWARE
                        If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEClean.ToString), "INI") Then
                            If Not MyClass.ISE_Manager.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEClean, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEClean, "CANCELED")
                            End If

                        ElseIf String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib.ToString), "INI") Then
                            If Not MyClass.ISE_Manager.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib, "CANCELED")
                            End If

                        ElseIf String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB.ToString), "INI") Then
                            If Not MyClass.ISE_Manager.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB, "CANCELED")
                            End If

                            'The ISE consumption flag is updated in OnISEProcedureFinished event handler


                            '    'AG 12/04/2012 - ISE consumption updated
                            'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption.ToString) = "INI" Then
                            '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption, "END")

                            '    'If pause in process mark as closed
                            '    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString) = "INPROCESS" Then
                            '        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "CLOSED")
                            '    End If

                            '    'Once the ise consumption is updated activate ansinf
                            '    If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                            '        AnalyzerIsInfoActivatedAttribute = 0
                            '        myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                            '        'This is not required with the INFO because it is an immediate instruction
                            '        ''When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                            '        'If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                            '        '    SetAnalyzerNotReady()
                            '        'End If
                            '    End If
                            '    'AG 12/04/2012

                        End If

                        If MyClass.ISE_Manager.LastISEResult.IsCancelError Then
                            'Mark ise auto conditioning process as finished
                            If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess.ToString), "INPROCESS") Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                            End If

                            'Cases
                            '1) Performing ISE utilities: Inform about the error
                            '2) Performing ISE auto conditioning process: Abort it
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS, 1, True)
                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due ISE conditioning warnings

                        Else
                            'Cases
                            '1) Performing ISE utilities: Nothing
                            '2) Performing ISE auto conditioning process: Call PerformAutoIseconditioning
                            If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess.ToString), "INPROCESS") Then
                                If String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB.ToString), "END") Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                                End If
                                myGlobalDataTO = PerformAutoIseConditioning(dbConnection)
                            End If
                        End If

                    End If

                ElseIf myPreparationID > 0 Then
                    'ISETEST answer
                    MyClass.ISE_Manager.LastISEResult = New ISEResultTO

                    'BT #1660 - Call the new version of function ProcessISETESTResults
                    myGlobalDataTO = myISEResultsDelegate.ProcessISETESTResultsNEW(dbConnection, myPreparationID, myISEResult, myISEMode, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    'UI refresh dataset (new results AND new rotor position status)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                        MyClass.ISE_Manager.LastISEResult = myISEResult 'SGM 12/01/2012
                        'SGM 17/02/2012 ISE Communications Ok
                        If MyClass.ISE_Manager IsNot Nothing Then
                            MyClass.ISE_Manager.IsISECommsOk = True
                        End If
                        'end SGM 17/02/2012

                        Dim myExecDS As New ExecutionsDS
                        myExecDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)
                        For Each ex_row As ExecutionsDS.twksWSExecutionsRow In myExecDS.twksWSExecutions.Rows
                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED, ex_row.ExecutionID, 0, Nothing, False)

                            'AG 02/12/2011 - UIRefresh sample rotor position status
                            'Get the ordertestStatus for the Execution
                            Dim myOTDelegate As New OrderTestsDelegate
                            Dim myOTDS As New OrderTestsDS
                            myGlobalDataTO = myOTDelegate.GetOrderTest(dbConnection, ex_row.OrderTestID)
                            myOTDS = CType(myGlobalDataTO.SetDatos, OrderTestsDS)

                            'When orderTestStatus is closed call method UpdateSamplePositionStatus in RotorContentByPositionDelegate class
                            If String.Equals(myOTDS.twksOrderTests(0).OrderTestStatus, "CLOSED") Then
                                Dim rcp_del As New WSRotorContentByPositionDelegate
                                myGlobalDataTO = rcp_del.UpdateSamplePositionStatus(dbConnection, ex_row.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                If Not myGlobalDataTO.HasError Then
                                    Dim rotorPosDS As New WSRotorContentByPositionDS
                                    rotorPosDS = CType(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                                    For Each rcp_row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
                                        myGlobalDataTO = PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", rcp_row.CellNumber, _
                                                                             rcp_row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                        'AG 09/03/2012 - do not inform about ElementStatus (old code: '... rcp_row.Status, "POS", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")'

                                        If myGlobalDataTO.HasError Then Exit For
                                    Next
                                End If

                                'AG 21/05/2012 - Call the online exportation method
                                If Not myGlobalDataTO.HasError Then
                                    Dim myExport As New ExportDelegate
                                    myGlobalDataTO = myExport.ManageLISExportation(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, ex_row.OrderTestID, False)
                                    If myGlobalDataTO.HasError Then Exit For
                                    AddIntoLastExportedResults(CType(myGlobalDataTO.SetDatos, ExecutionsDS)) 'AG 19/03/2013

                                    ''UI refresh DataSet informing the exported OrderTests - NOT Required, the UI refresh dataset RESULTS_CALCULATED case has been already updated at the begining of  this loop
                                    'If Not myGlobalDataTO.SetDatos Is Nothing Then
                                    '    For Each ex_row As ExecutionsDS.twksWSExecutionsRow In myExecDS.twksWSExecutions.Rows
                                    '        myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED, ex_row.ExecutionID, Nothing, Nothing, False)
                                    '        If myGlobalDataTO.HasError Then Exit For
                                    '    Next
                                    'End If
                                End If
                                'AG 21/05/2012

                            End If
                            'AG 02/12/2011
                        Next

                        'SGM 24/07/2012
                        'Block ISE Preparations in case of Cancel Error (ERC) more than 2 times
                        If MyClass.ISE_Manager.ISEWSCancelErrorCounter >= 3 Then

                            Dim exec_delg As New ExecutionsDelegate

                            '1st: Get all ISE executions pending (required to inform the UI with the locked executions
                            myGlobalDataTO = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "PENDING", False, "PREP_ISE")
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                                Dim myExecutionsDS As New ExecutionsDS
                                myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                                If myExecutionsDS.twksWSExecutions.Count > 0 Then
                                    '2on: Lock all pending ISE preparation executions
                                    myGlobalDataTO = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")

                                    '3rd: Prepare data for generate user interface refresh event
                                    If Not myGlobalDataTO.HasError Then
                                        For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                            If myGlobalDataTO.HasError Then Exit For
                                        Next
                                    End If
                                End If
                            End If
                            myGlobalDataTO = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS", False, "PREP_ISE")
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                                Dim myExecutionsDS As New ExecutionsDS
                                myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                                If myExecutionsDS.twksWSExecutions.Count > 0 Then
                                    '2on: Lock all pending ISE preparation executions
                                    myGlobalDataTO = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "INPROCESS", "LOCKED")

                                    '3rd: Prepare data for generate user interface refresh event
                                    If Not myGlobalDataTO.HasError Then
                                        For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                            If myGlobalDataTO.HasError Then Exit For
                                        Next
                                    End If
                                End If
                            End If
                        End If
                        'end SGM 24/07/2012

                    End If
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobalDataTO = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                End If

                ' XBC 21/03/2012   
                ' Check Alarms
                Dim myAlarmListTmp As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobalDataTO = MyClass.ISE_Manager.CheckAlarms(MyClass.Connected, myAlarmListTmp, myAlarmStatusListTmp)
                If Not myGlobalDataTO.HasError Then

                    For i As Integer = 0 To myAlarmListTmp.Count - 1
                        PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                    Next

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        myGlobalDataTO = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            ' XBC 17/10/2012 - Alarms treatment for Service
                            ' Not Apply
                            'myGlobalDataTO = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                        Else
                            myGlobalDataTO = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                        End If

                    End If
                End If
                ' XBC 21/03/2012

                'End If'AG 03/07/2012 - Running Cycles lost - Solution!

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessRecivedISEResult", EventLogEntryType.Error, False)
            End Try

            'AG 29/11/2011 - This method does not follow the standard and not receive a pDbConnection as parameter so we have to close the connection properly here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    DAOBase.RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If
            'AG 29/11/2011

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Performe the auto ise conditioning before enter in running
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG 19/0/2012
        ''' Modified by XB+AG 16/10/2013 - Leave pause running mode - BT #1333
        ''' </remarks>
        Private Function PerformAutoIseConditioning(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess.ToString) = "INPROCESS" Then
                            Dim myScreenIseCmdTo As New ISECommandTO
                            'Dim myIseManager As New ISEManager

                            '   Required: Clean + Pump Calib + Calib A + B
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEClean.ToString) = "" Then 'Start process sending Ise clean
                                'Send ISECMD clean
                                resultData = MyClass.ISE_Manager.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, GlobalEnumerates.ISEModes.Cleaning_Cycle, GlobalEnumerates.ISECommands.CLEAN)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = MyClass.ISE_Manager.SendISECommand 'SGM 08/03/2012
                                    'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, myScreenIseCmdTo)

                                    'If not comms errors update flags
                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEClean, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEClean.ToString) = "END" AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib.ToString) <> "END" Then 'When ISEClean finish send pump calib
                                'Send ISECMD pump calib
                                resultData = MyClass.ISE_Manager.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, GlobalEnumerates.ISEModes.Pump_Calibration, GlobalEnumerates.ISECommands.PUMP_CAL)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = MyClass.ISE_Manager.SendISECommand 'SGM 08/03/2012
                                    'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, myScreenIseCmdTo)

                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEClean.ToString) = "END" AndAlso String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib.ToString), "END", False) = 0 Then
                                'Send ISECMD calib AB
                                resultData = MyClass.ISE_Manager.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, GlobalEnumerates.ISEModes.Low_Level_Control, GlobalEnumerates.ISECommands.CALB)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = MyClass.ISE_Manager.SendISECommand 'SGM 08/03/2012
                                    'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, myScreenIseCmdTo)

                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            End If


                        ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess.ToString) = "CLOSED" Then
                            '   Not required: Running

                            ' XB+AG 16/10/2013 - BT #1333
                            'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "INPROCESS") 'AG 31/08/2012
                            'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True) 'Send go to RUNNING
                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "INPROCESS") 'AG 31/08/2012
                                resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True) 'Send go to RUNNING
                            ElseIf AllowScanInRunning Then
                                'AG 12/11/2013 - Call manage analyer instead of applayer
                                'resultData = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.START)
                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.StartRunning, "INI") 'AG 19/11/2012 - task #1396-e
                                resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.START, True, Nothing)
                            End If
                            ' XB+AG 16/10/2013 - BT #1333

                            If Not resultData.HasError AndAlso ConnectedAttribute Then
                                endRunAlreadySentFlagAttribute = False
                                abortAlreadySentFlagAttribute = False
                                PauseAlreadySentFlagAttribute = False ' XB 15/10/2013 - BT #1318
                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished (running instruction has been sent)

                                'Set WorkSession Status to INPROCESS
                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                resultData = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS")
                            End If

                        End If

                        If Not resultData.HasError AndAlso ConnectedAttribute Then
                            'Update analyzer session flags into DataBase
                            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                resultData = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
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
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.PerformAutoIseConditioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Processes the ISE procedures
        ''' </summary>
        ''' <remarks>
        ''' Created by  SGM 07/03/2012
        ''' Modified by SGM 16/01/2013 - Bug #1108
        '''              XB 20/05/2014 - Add protections #1614
        ''' </remarks>
        Private Function ProcessISEManagerProcedures() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            'If My.Computer.Name <> "AREASW1" Then Return myGlobal
            'If My.Computer.Name <> "AUXSOFTWARE1" Then Return myGlobal

            Try

                'The ISE Module must be factory installed and switched on
                If MyClass.ISE_Manager.IsISEModuleInstalled And MyClass.ISE_Manager.IsISESwitchON Then

                    Select Case MyClass.ISE_Manager.CurrentProcedure

                        Case ISEManager.ISEProcedures.Test
                            If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then      ' #1614
                                If MyClass.ISE_Manager.CurrentCommandTO.TestType = ISECycles.URINE1 Then
                                    MyClass.ISE_Manager.CurrentCommandTO = New ISECommandTO(ISECycles.URINE2)
                                End If
                            End If

                        Case ISEManager.ISEProcedures.SingleReadCommand
                            'If MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_0_DALLAS Then
                            '    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)
                            'End If

                        Case ISEManager.ISEProcedures.GeneralCheckings
                            If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then
                                Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID

                                    '1- Read Page 00 from Reagents Pack
                                    Case ISECommands.POLL
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(0)

                                        '2- Read Page 01 from Reagents Pack
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                                    Case ISECommands.READ_PAGE_1_DALLAS
                                        'If myISEManager.IsElectrodesReady Then
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_mV
                                        'End If
                                End Select
                            End If

                            'Case ISEManager.ISEProcedures.ActivateModule
                            '    If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then
                            '        Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID

                            '            '1- Read Page 01 from Reagents Pack
                            '            Case ISECommands.READ_PAGE_0_DALLAS
                            '                myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                            '            Case ISECommands.READ_PAGE_1_DALLAS
                            '                'If myISEManager.IsElectrodesReady Then
                            '                myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_mV
                            '                'End If
                            '        End Select
                            '    End If

                        Case ISEManager.ISEProcedures.ActivateReagentsPack
                            If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then
                                Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID

                                    'SGM 16/01/2013 Follow the next sequence
                                    '1-Read Dallas Page 00 (biosystems code)
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        'reset Clean Pack flag
                                        MyClass.ISE_Manager.IsCleanPackInstalled = False
                                        '2-Read Dallas Page 01 (install date)
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)
                                        myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        Debug.Print("Reading Dallas Page 01 ...")

                                    Case ISECommands.READ_PAGE_1_DALLAS
                                        '3-If not installed:
                                        If MyClass.ISE_Manager.ReagentsPackInstallationDate = Nothing Then
                                            ' 3.1-Write Day
                                            myGlobal = MyClass.ISE_Manager.PrepareDataToSend_WRITE_INSTALL_DAY(Now.Day)
                                            myGlobal = MyClass.ISE_Manager.SendISECommand()
                                            MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_DAY_INSTALL
                                            Debug.Print("Writing Install Day: " & Now.Day.ToString)
                                        End If

                                    Case ISECommands.WRITE_DAY_INSTALL
                                        ' 3.2-Write Month
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_WRITE_INSTALL_MONTH(Now.Month)
                                        myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_MONTH_INSTALL
                                        Debug.Print("Writing Install Month: " & Now.Month.ToString)

                                    Case ISECommands.WRITE_MONTH_INSTALL
                                        ' 3.3-Write Year
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_WRITE_INSTALL_YEAR(Now.Year)
                                        myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_YEAR_INSTALL
                                        Debug.Print("Writing Install Year: " & Now.Year.ToString)

                                    Case ISECommands.WRITE_YEAR_INSTALL
                                        '4-Read Dallas Page 01 (install date, confirmation)
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)
                                        myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        Debug.Print("Reading Dallas Page 01 ... confirmation")


                                        'Commented SGM 16/01/2013 - Bug #1108                                         
                                        '    ' XBC 19/03/2012
                                        '    ''2- Read Page 01 from Reagents Pack
                                        '    'Case ISECommands.READ_PAGE_0_DALLAS
                                        '    '    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                                        'Case ISECommands.WRITE_DAY_INSTALL
                                        '    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_WRITE_INSTALL_MONTH(Month(Now))
                                        '    Debug.Print("Guardant MES Reagents Pack ..." & Month(Now).ToString)
                                        '    myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        '    MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_MONTH_INSTALL
                                        '    Debug.Print("Canvi d'estat a WRITE_MONTH_INSTALL ...")


                                        'Case ISECommands.WRITE_MONTH_INSTALL
                                        '    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_WRITE_INSTALL_YEAR(Year(Now))
                                        '    Debug.Print("Guardant ANY Reagents Pack ..." & Year(Now).ToString)
                                        '    myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        '    MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_YEAR_INSTALL
                                        '    Debug.Print("Canvi d'estat a WRITE_YEAR_INSTALL ...")


                                        'Case ISECommands.WRITE_YEAR_INSTALL
                                        '    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)
                                        '    Debug.Print("Llegint PAG1 Reagents Pack ...")
                                        '    myGlobal = MyClass.ISE_Manager.SendISECommand()
                                        '    Debug.Print("Fi d'Activar Reagents Pack !")
                                        '    ' XBC 19/03/2012

                                End Select
                            End If

                        Case ISEManager.ISEProcedures.CheckReagentsPack
                            If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then
                                Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID

                                    '2- Read Page 01 from Reagents Pack
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                                End Select
                            End If

                        Case ISEManager.ISEProcedures.CheckCleanPackInstalled
                            Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                                Case ISECommands.PURGEA
                                    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_PURGEB

                            End Select

                        Case ISEManager.ISEProcedures.Clean 'PDT to optimization
                            'Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                            '    Case ISECommands.CLEAN
                            '        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_CALB

                            'End Select

                        Case ISEManager.ISEProcedures.MaintenanceExit
                            Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                                Case ISECommands.PURGEA
                                    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_PURGEB

                            End Select


                            ' XBC 04/04/2012
                        Case ISEManager.ISEProcedures.WriteConsumption
                            Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                                Case ISECommands.WRITE_CALA_CONSUMPTION
                                    If MyClass.ISE_Manager.IsCalBUpdateRequired Then 'update calB consumption

                                        ' Save it into Dallas Xip
                                        myGlobal = MyClass.ISE_Manager.SaveConsumptionCalToDallasData("B")
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            MyClass.ISE_Manager.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)
                                            Debug.Print("Guardant Consum Cal B ..." & Microsoft.VisualBasic.DateAndTime.Day(Now).ToString)

                                            myGlobal = MyClass.ISE_Manager.SendISECommand()
                                            MyClass.ISE_Manager.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALB_CONSUMPTION
                                            Debug.Print("Canvi d'estat a WRITE_CALB_CONSUMPTION ...")
                                        End If
                                    End If

                            End Select

                        Case ISEManager.ISEProcedures.PrimeAndCalibration
                            Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                                Case ISECommands.PRIME_CALB
                                    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_PRIME_CALA

                                Case ISECommands.PRIME_CALA
                                    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_CALB
                            End Select

                        Case ISEManager.ISEProcedures.PrimeX2AndCalibration 'SGM 11/06/2012
                            Static PrimeCount As Integer = 0
                            Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID
                                Case ISECommands.PRIME_CALB
                                    myGlobal = MyClass.ISE_Manager.PrepareDataToSend_PRIME_CALA

                                Case ISECommands.PRIME_CALA
                                    PrimeCount += 1
                                    If PrimeCount < 2 Then
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_PRIME_CALB
                                    Else
                                        PrimeCount = 0
                                        myGlobal = MyClass.ISE_Manager.PrepareDataToSend_CALB
                                    End If


                            End Select


                        Case Else
                            Debug.Print("Caso NONE !")
                            ' XBC 04/04/2012


                    End Select

                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then

                        ' XBC 23/03/2012
                        If MyClass.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.ActivateReagentsPack And _
                           MyClass.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.WriteConsumption Then
                            myGlobal = MyClass.ISE_Manager.SendISECommand()
                        End If
                        ' XBC 23/03/2012

                    End If

                    'Special case for cheking before installing ISE Module
                ElseIf MyClass.ISE_Manager.CurrentProcedure = ISEManager.ISEProcedures.ActivateModule Then
                    If MyClass.ISE_Manager.CurrentCommandTO IsNot Nothing Then
                        Select Case MyClass.ISE_Manager.CurrentCommandTO.ISECommandID

                            '1- Read Page 01 from Reagents Pack
                            Case ISECommands.READ_PAGE_0_DALLAS
                                myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_PAGE_DALLAS(1)

                            Case ISECommands.READ_PAGE_1_DALLAS
                                'If myISEManager.IsElectrodesReady Then
                                myGlobal = MyClass.ISE_Manager.PrepareDataToSend_READ_mV
                                'End If
                        End Select
                    End If

                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        myGlobal = MyClass.ISE_Manager.SendISECommand()
                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEManager.ProcessISEManagerProcedures", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Refreshes ISE alarms
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/06/2012</remarks>
        Public Function RefreshISEAlarms() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAlarmListTmp As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobal = MyClass.ISE_Manager.CheckAlarms(MyClass.Connected, myAlarmListTmp, myAlarmStatusListTmp)
                If Not myGlobal.HasError Then

                    For i As Integer = 0 To myAlarmListTmp.Count - 1
                        PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                    Next

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        'myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)   ' XBC+AG+SG 30/10/2012
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            ' XBC 17/10/2012 - Alarms treatment for Service
                            ' Not Apply
                            'myGlobal = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                        Else
                            myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                        End If

                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEManager.RefreshISEAlarms", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Activates Analyzer Alarms in case of errors in calibrations
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 25/07/2012</remarks>
        Public Function ActivateAnalyzerISEAlarms(ByVal pISEResult As ISEResultTO) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            'Return myGlobal 'QUITAR

            Try

                If pISEResult.Errors.Count > 0 Then

                    'Prepare internal variables for alarms management
                    Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)
                    Dim myAlarmDescriptions As New List(Of String)


                    Dim myAlarmOrigin As String = "" 'origin

                    Dim myAlarmID As GlobalEnumerates.Alarms 'Identifier

                    If pISEResult.IsCancelError Then

                        Dim myAdditionalInfo As String = ""

                        Select Case pISEResult.Errors(0).ErrorCycle
                            Case ISEErrorTO.ErrorCycles.Calibration : myAlarmOrigin = "CALB"
                            Case ISEErrorTO.ErrorCycles.PumpCalibration : myAlarmOrigin = "PMCL"
                            Case ISEErrorTO.ErrorCycles.BubbleDetCalibration : myAlarmOrigin = "BBCL"
                            Case ISEErrorTO.ErrorCycles.Clean : myAlarmOrigin = "CALB"
                            Case ISEErrorTO.ErrorCycles.Communication : myAlarmOrigin = "COMM"
                            Case ISEErrorTO.ErrorCycles.DallasReadWrite : myAlarmOrigin = "DALLAS RW"
                            Case ISEErrorTO.ErrorCycles.Maintenance : myAlarmOrigin = "MANT"
                            Case ISEErrorTO.ErrorCycles.PurgeA : myAlarmOrigin = "PUGA"
                            Case ISEErrorTO.ErrorCycles.PurgeB : myAlarmOrigin = "PUGB"
                            Case ISEErrorTO.ErrorCycles.Sample : myAlarmOrigin = "SER"
                            Case ISEErrorTO.ErrorCycles.Urine : myAlarmOrigin = "URN"
                            Case ISEErrorTO.ErrorCycles.SipCycle : myAlarmOrigin = "SIP"
                        End Select
                        Select Case pISEResult.Errors(0).CancelErrorCode
                            Case ISEErrorTO.ISECancelErrorCodes.A : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_A
                            Case ISEErrorTO.ISECancelErrorCodes.B : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_B
                            Case ISEErrorTO.ISECancelErrorCodes.C : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_C
                            Case ISEErrorTO.ISECancelErrorCodes.D : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_D
                            Case ISEErrorTO.ISECancelErrorCodes.F : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_F
                            Case ISEErrorTO.ISECancelErrorCodes.M : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_M
                            Case ISEErrorTO.ISECancelErrorCodes.N : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_N
                            Case ISEErrorTO.ISECancelErrorCodes.P : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_P
                            Case ISEErrorTO.ISECancelErrorCodes.R : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_R
                            Case ISEErrorTO.ISECancelErrorCodes.S : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_S
                            Case ISEErrorTO.ISECancelErrorCodes.T : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_T
                            Case ISEErrorTO.ISECancelErrorCodes.W : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_W

                        End Select

                        'myAlarmOrigin = "ISE Error: " & myAlarmOrigi
                        myAdditionalInfo &= GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myAlarmOrigin & ":E" & CInt(pISEResult.Errors(0).CancelErrorCode).ToString & ":"

                        If myAlarmID <> GlobalEnumerates.Alarms.NONE Then
                            myAlarmDescriptions.Add(myAdditionalInfo)
                            myAlarmList.Add(myAlarmID)
                        End If

                        'special case for calibration result errors
                    ElseIf pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Or _
                        pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC Or _
                        pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BBC Then

                        Dim myAdditionalInfo As String = ""

                        For Each E As ISEErrorTO In pISEResult.Errors
                            If Not E.IsCancelError Then
                                'SGM 26/07/2012 - special case for CALB, PMCL, BBCL results no ok
                                Select Case pISEResult.ISEResultType
                                    Case ISEResultTO.ISEResultTypes.CAL : myAlarmOrigin = "CALB Error - "
                                    Case ISEResultTO.ISEResultTypes.PMC : myAlarmOrigin = "PMCL Error - "
                                    Case ISEResultTO.ISEResultTypes.BBC : myAlarmOrigin = "BBCL Error - "
                                End Select

                                Dim myResultAlarm As Integer = -1
                                If E.ResultErrorCode <> ISEErrorTO.ISEResultErrorCodes.None Then
                                    myResultAlarm = CInt(E.ResultErrorCode)
                                    Dim myAffected As String = ISEErrorTO.FormatAffected(E.Affected)
                                    myAdditionalInfo &= GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myAlarmOrigin & ":R" & myResultAlarm.ToString & ": (" + myAffected + ")"
                                End If

                            End If

                        Next

                        myAlarmDescriptions.Add(myAdditionalInfo)
                        myAlarmID = GlobalEnumerates.Alarms.ISE_CALIB_ERROR
                        myAlarmList.Add(myAlarmID)

                        'myAlarmStatusList.Add(True)
                    End If

                    For Each A As Alarms In myAlarmList

                        Dim myAnalyzerAlarmsDS As New WSAnalyzerAlarmsDS
                        Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                        alarmRow = myAnalyzerAlarmsDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow

                        Dim myTempAlarmList As New List(Of Alarms)
                        PrepareLocalAlarmList(myAlarmID, True, myTempAlarmList, myAlarmStatusList, myAlarmOrigin, myAlarmDescriptions, False)

                        If myAlarmList.Count > 0 Then
                            'Finally call manage all alarms detected (new or fixed)
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                ' XBC 17/10/2012 - Alarms treatment for Service
                                ' Not Apply
                                'myGlobal = ManageAlarms_SRV(dbConnection, myTempAlarmList, myAlarmStatusList)
                            Else
                                myGlobal = ManageAlarms(dbConnection, myTempAlarmList, myAlarmStatusList, myAlarmDescriptions)
                            End If
                        End If

                    Next

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEReception.ActivateAnalyzerISEAlarms", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Blocks Individual ISE Electrode after receiving results erros
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 02/08/2012
        ''' Modified by XB 20/05/2014 - Fix BT #1629
        ''' </remarks>
        Public Function BlockISEPreparationByElectrode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISEResult As ISEResultTO, ByVal pWorkSessionId As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim isLiAffected As Boolean = False
                Dim isNaAffected As Boolean = False
                Dim isKAffected As Boolean = False
                Dim isClAffected As Boolean = False
                For Each E As ISEErrorTO In MyClass.ISE_Manager.LastISEResult.Errors
                    isLiAffected = (isLiAffected Or (E.Affected.Contains("Li"))) And MyClass.ISE_Manager.IsLiEnabledByUser
                    isNaAffected = isNaAffected Or (E.Affected.Contains("Na"))
                    isKAffected = isKAffected Or (E.Affected.Contains("K"))
                    isClAffected = isClAffected Or (E.Affected.Contains("Cl"))
                Next

                Dim myExecutionsDelegate As New ExecutionsDelegate
                myGlobalDataTO = myExecutionsDelegate.GetExecutionsByStatus(pDBConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "PENDING", False, "PREP_ISE")
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                    Dim myExecutionsDS As New ExecutionsDS
                    myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                    If myExecutionsDS.twksWSExecutions.Count > 0 Then

                        If isLiAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Li", "PENDING", "LOCKED")
                        End If
                        If isNaAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Na", "PENDING", "LOCKED")
                        End If
                        If isKAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "K", "PENDING", "LOCKED")
                        End If
                        If isClAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Cl", "PENDING", "LOCKED")
                        End If

                        If Not myGlobalDataTO.HasError Then
                            For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                myGlobalDataTO = PrepareUIRefreshEvent(pDBConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                If myGlobalDataTO.HasError Then Exit For
                            Next
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISEReception.BlockISEPreparationByElectrode", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO

        End Function



#End Region

#Region "Auxiliary methods others"

        ''' <summary>
        ''' Method for code common used
        ''' </summary>
        ''' <remarks>
        ''' AG 14/03/2011</remarks>
        Private Sub SetAnalyzerNotReady()
            If AnalyzerIsReadyAttribute Then
                AnalyzerIsReadyAttribute = False
                'Me.InitializeTimerControl(WAITING_TIME_DEFAULT) 'AG 18/07/2011 - commment this line
            End If
        End Sub


        Private Function ReadBarCodeRotorSettingEnabled(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSetting As String) As Boolean
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim returnedFlag As Boolean = False
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'If reagents barcode enabled send read FULL barcode reagents rotor
                        Dim anSettingsDelg As New AnalyzerSettingsDelegate
                        resultData = anSettingsDelg.GetAnalyzerSetting(dbConnection, AnalyzerIDAttribute, pSetting)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                            Dim myAnSettingsDS As New AnalyzerSettingsDS
                            myAnSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                            If (myAnSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then '(1)
                                returnedFlag = CType(myAnSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                            End If

                        End If 'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadBarCodeRotorSettingEnabled", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            'resultData.SetDatos = returnedFlag
            'Return resultData
            Return returnedFlag
        End Function

#End Region

#Region "OLD version methods"

        '''' <summary>
        '''' Search next STD preparation, taking care about the last executions sent and search next avoiding contaminations
        '''' using the executions matching the OrderID, SampleType, StatFlag and SampleClass in entry parameters
        '''' 
        '''' This method is up to date due the sort algoritm by look for the next preparation is different that the used in CreateWSExecutions
        '''' </summary>
        '''' <param name="pFound" ></param>
        '''' <param name="pSTDExecutionList" ></param>
        '''' <param name="pOrderID " ></param>
        '''' <param name="pSampleType " ></param>
        '''' <param name="pStatFlag" ></param>
        '''' <param name="pSampleClass" ></param>
        '''' <returns>AnalyzerManager.searchNext inside a GlobalDataTo</returns>
        '''' <remarks>AG 25/01/2011 - Created</remarks>
        'Private Function GetNextExecutionUntil28112011(ByRef pFound As Boolean, ByVal pSTDExecutionList As AnalyzerManagerDS, _
        '                                 ByVal pOrderID As String, ByVal pSampleType As String, _
        '                                 ByVal pStatFlag As Boolean, ByVal pSampleClass As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try

        '        'Get the list of pending executions depending the entry parameters case
        '        'Apply different LINQ (where clause)
        '        Dim toSendList As New List(Of AnalyzerManagerDS.searchNextRow)
        '        If pOrderID <> "" And pSampleType <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.OrderID = pOrderID And a.SampleType = pSampleType And a.StatFlag = pStatFlag _
        '                            Select a).ToList
        '        ElseIf pOrderID <> "" And pSampleType <> "" And pSampleClass <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.OrderID = pOrderID And a.SampleType = pSampleType And a.StatFlag = pStatFlag And a.SampleClass = pSampleClass _
        '                            Select a).ToList

        '        ElseIf pSampleClass <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.StatFlag = pStatFlag And a.SampleClass = pSampleClass _
        '                            Select a).ToList

        '        Else '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.StatFlag = pStatFlag _
        '                            Select a).ToList

        '        End If '(1)


        '        Dim nextExecutionFound As Boolean = False
        '        Dim rowExecutionFound As AnalyzerManagerDS.searchNextRow
        '        Dim jIndex As Integer = 0
        '        Dim iIndex As Integer = 0

        '        If toSendList.Count > 0 Then '(2)
        '            Dim tmpExeID As Integer = -1
        '            For j As Integer = 0 To toSendList.Count - 1 'For (1)
        '                jIndex = j
        '                tmpExeID = toSendList(j).ExecutionID

        '                'Is reagent has contaminators ... analyze last sent preparations
        '                If Not toSendList(j).IsContaminationIDNull And Not toSendList(j).IsReagentContaminatorIDNull Then '(3)
        '                    'Get all contaminations defined for the reagent used in the execution tmpExeID
        '                    Dim list2 As New List(Of AnalyzerManagerDS.searchNextRow)
        '                    list2 = (From a As AnalyzerManagerDS.searchNextRow In toSendList _
        '                                  Where a.ExecutionID = tmpExeID Select a).ToList

        '                    Dim contaminationFound As Boolean = False
        '                    Dim i As Integer = 0
        '                    If list2.Count > 0 Then '(4)
        '                        For i = 0 To list2.Count - 1 'For (2)
        '                            iIndex = i

        '                            Dim myReagentContaminator As Integer = -1
        '                            Dim myWashSolution As String = ""
        '                            If Not list2(i).IsReagentContaminatorIDNull Then myReagentContaminator = list2(i).ReagentContaminatorID
        '                            If Not list2(i).IsWashingSolution1Null Then myWashSolution = list2(i).WashingSolution1

        '                            'Using Linq determine if the reagentcontaminator is one of the last preparations sent
        '                            Dim list3 As New List(Of AnalyzerManagerDS.sentPreparationsRow)
        '                            list3 = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
        '                                      Where Not a.IsReagentIDNull Select a).ToList

        '                            If list3.Count = 0 Then
        '                                Exit For '(2) 'If no reagent sent then no contamination exists
        '                            Else
        '                                list3 = (From a As AnalyzerManagerDS.sentPreparationsRow In list3 _
        '                                          Where a.ReagentID = myReagentContaminator Select a).ToList
        '                            End If

        '                            If list3.Count > 0 Then '(5)
        '                                'Using the last PREP_STD executions sent and look for contaminations (linq is not possible due we have to determine
        '                                'if the proper wash has been sent after
        '                                Dim rowPointer As Integer = -1
        '                                For Each row As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations.Rows 'For (3)
        '                                    rowPointer += 1
        '                                    If Not row.IsExecutionTypeNull Then '(6)
        '                                        If row.ExecutionType = "PREP_STD" And row.ReagentID = myReagentContaminator Then '(7)
        '                                            contaminationFound = True

        '                                            'If exits contamination ... look if the proper wash has been already performed
        '                                            For k As Integer = rowPointer + 1 To mySentPreparationsDS.sentPreparations.Rows.Count - 1 'For (4)
        '                                                Dim pastWashFlag As Boolean = False
        '                                                Dim pastWashSolution As String = ""
        '                                                If Not mySentPreparationsDS.sentPreparations(k).IsReagentWashFlagNull Then pastWashFlag = mySentPreparationsDS.sentPreparations(k).ReagentWashFlag
        '                                                If Not mySentPreparationsDS.sentPreparations(k).IsWashSolution1Null Then pastWashSolution = mySentPreparationsDS.sentPreparations(k).WashSolution1

        '                                                If pastWashFlag And pastWashSolution = myWashSolution Then
        '                                                    contaminationFound = False 'The contamination has been already solved!
        '                                                    Exit For 'Exit For (4)
        '                                                End If
        '                                            Next 'For (4)
        '                                        End If '(7)
        '                                    End If '(6)

        '                                    If contaminationFound Then
        '                                        'If a contamination is found finish loop
        '                                        Exit For 'Exit For (3)
        '                                    End If
        '                                Next 'For (3)

        '                                If Not contaminationFound Then Exit For 'Exit For (2) - myWashSolution has the proper WashSolution to use

        '                            End If '(5)
        '                        Next 'For (2)

        '                        If Not contaminationFound Then
        '                            nextExecutionFound = True
        '                            rowExecutionFound = toSendList(jIndex)
        '                            Exit For 'Exit For (1)
        '                        Else
        '                            nextExecutionFound = False
        '                            rowExecutionFound = list2(0) '(iIndex) If contamination found sent the first item in list2. Usign iIndex the last was sent
        '                        End If
        '                    End If '(4) If list2.Count > 0 Then


        '                    'Is reagent has no contaminators ... sent it
        '                Else '(3) If Not toSendList(j).IsContaminationIDNull And Not toSendList(j).IsReagentContaminatorIDNull Then
        '                    nextExecutionFound = True
        '                    rowExecutionFound = toSendList(jIndex)
        '                    Exit For 'Exit For (1)
        '                End If '(3)

        '            Next 'For (1)

        '            Dim myReturn As New AnalyzerManagerDS
        '            Dim myRow As AnalyzerManagerDS.searchNextRow

        '            pFound = True ' Inform something is found to be sent: execution or wash
        '            If nextExecutionFound Then '(4)
        '                'Prepare output DS with the proper information (execution to be sent)
        '                myRow = myReturn.searchNext.NewsearchNextRow
        '                myRow.ExecutionID = rowExecutionFound.ExecutionID
        '                myRow.SampleClass = rowExecutionFound.SampleClass
        '                myRow.SetContaminationIDNull()
        '                myRow.SetWashingSolution1Null()
        '                myReturn.searchNext.AddsearchNextRow(myRow)

        '            Else
        '                'Contamination (wash has to be sent)
        '                myRow = myReturn.searchNext.NewsearchNextRow
        '                myRow.ExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
        '                myRow.SetSampleClassNull()
        '                myRow.ContaminationID = rowExecutionFound.ContaminationID
        '                myRow.WashingSolution1 = rowExecutionFound.WashingSolution1
        '                myReturn.searchNext.AddsearchNextRow(myRow)
        '            End If '(4)
        '            resultData.SetDatos = myReturn


        '            'Else '(2) If toSendList.Count > 0 Then
        '            'No exectution matches with linq where criteria
        '        End If '(2)

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecutionUntil28112011", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Basic method for search next STD or ISE preparation execution based in OLD SearchNextPreparation
        '''' 
        '''' Search next pending preparation
        '''' Priority criteriom:
        '''' 1st Stats
        '''' 2st SampleClass (blank, calib, ctrl, patients)
        '''' 3st MultiItemNumber (ASC) (without change ordertestid)
        '''' 4st Replicate number (ASC) (without change ordertestid) 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pExecutionType"></param>
        '''' <returns></returns>
        '''' <remarks>AG 17/01/2011 - Created</remarks>
        'Private Function GetNextExecutionOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionType As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then '(2)
        '                Dim myExecution As New ExecutionsDelegate
        '                Dim myStatFlag As Boolean = True
        '                Dim foundItem As Boolean = False

        '                If AnalyzerIDAttribute = "" Or WorkSessionIDAttribute = "" Then '(3) 'AG 03/11/2010 If no exists analyzer or worksession then return NO PENDING PREPARATION
        '                    resultData.SetDatos = GlobalConstants.NO_PENDING_PREPARATION_FOUND

        '                Else
        '                    Do
        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "BLANK", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "CALIB", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "CTRL", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "PATIENT", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        myStatFlag = Not myStatFlag
        '                    Loop Until myStatFlag Or foundItem
        '                End If '(3)

        '            End If '(2)
        '        End If '(1)

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecutionOLD", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

#End Region

    End Class

End Namespace

