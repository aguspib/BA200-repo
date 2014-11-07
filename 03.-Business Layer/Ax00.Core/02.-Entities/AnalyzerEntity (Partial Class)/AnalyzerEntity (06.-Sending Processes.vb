Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.InfoAnalyzer
Imports System.Timers
Imports System.Data
Imports System.ComponentModel 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerEntity

#Region "Connection process"

        ''' <summary>
        ''' Process connection algorithm - adapted from iPRO and Ax5
        ''' Final connection process in Ax00:
        ''' 1 CONNECT
        ''' 2 READ ADJUSTMENTS
        ''' 3 CONFIGURE BARCODE TYPES
        ''' 4 CONFIGURE ANALYZER
        ''' 5 SOUND OFF
        ''' 
        ''' </summary>
        ''' <param name="pConnectObject"></param>
        ''' <param name="pFromRecoveryResults">optional FALSE default value, TRUE when this method is called after recovery results</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by AG 07/05/2010 (move from ManageAnalyzer case CONNECT
        ''' Modified by SGM 23/02/2012: include optional parameter for testing communications from Service SW
        ''' AG 25/09/2012 - Add 2on parameter and define both parameters as NOT optional. After recovery results call this method with parameter pFromRecoveryResults = True
        ''' </remarks>
        Private Function ProcessConnection(ByVal pConnectObject As Object, ByVal pFromRecoveryResults As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Ax5 (Comm_CapaApp.Abrir) // iPRO (GestorEstados.Abrir)
                'ConnectedAttribute = False
                InfoRefreshFirstTime = True
                runningConnectionPollSnSent = False 'AG 03/12/2013 - BT #1397

                IsFwReadedAttr = False 'SGM 21/06/2012

                'Clear all refresh DS before send CONNECT
                If SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.CONNECTED) Then
                    SensorValuesAttribute.Remove(GlobalEnumerates.AnalyzerSensors.CONNECTED)
                End If
                If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.COMMS_ERR) Then
                    myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.COMMS_ERR)
                End If
                'myUI_RefreshEvent.Clear()
                'myUI_RefreshDS.Clear()
                ClearRefreshDataSets(True, True) 'AG 22/05/2014 - #1637

                'AG 22/05/2014 - #1637 Clear code. Comment dead code
                'mySecondaryUI_RefreshEvent.Clear()
                'mySecondaryUI_RefreshDS.Clear()

                'AG 29/03/2012 - clear freeze alarms
                If analyzerFREEZEFlagAttribute AndAlso Not pFromRecoveryResults Then
                    'Use a false RECOVER INSTRUCTION END to remove all freeze alarms and re-evaluate with next status instruction
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FREEZE, 0, False)
                    myGlobal = RemoveErrorCodeAlarms(Nothing, GlobalEnumerates.AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END)
                End If

                'SGM 23/02/2012
                If pConnectObject Is Nothing Then
                    'AG 24/10/2011 - When connection auto then portnameattribute = "", else portnameattribute = configured port name
                    Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                    Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                    myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.COMM_AUTO.ToString())
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                        If myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                            If CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean) Then 'Auto connect configured
                                PortNameAttribute = ""
                                BaudsAttribute = ""
                            Else 'Manual port connect configured
                                myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.COMM_PORT.ToString())
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)
                                End If
                                If myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                    PortNameAttribute = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, String)
                                    BaudsAttribute = GlobalBase.PortSpeed.ToString()
                                Else 'Protection case
                                    PortNameAttribute = ""
                                    BaudsAttribute = ""
                                End If
                            End If
                        End If

                    End If
                    'AG 24/10/2011

                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'elseIf My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                ElseIf GlobalBase.IsServiceAssembly Then
                    'for testing communications from Service SW
                    If TypeOf pConnectObject Is Boolean Then
                        PortNameAttribute = ""
                        BaudsAttribute = ""
                    ElseIf TypeOf pConnectObject Is List(Of String) Then
                        PortNameAttribute = CType(pConnectObject, List(Of String))(0)
                        BaudsAttribute = CType(pConnectObject, List(Of String))(1)
                    End If

                End If
                'end SGM 23/02/2012

                'Use the Open method instead the ActivateProtocol only for the CONNECT instruction
                'Ax5 & iPRO: Has two different instructions: 1) INIT comms, 2)CONNECT
                'Ax00: Has only one instruction CONNECT that execute (INIT comms + CONNECT)
                'myGlobal = AppLayer.ActivateProtocol(ApplicationLayer.ClassEventList.SHORTINSTRUCTION, ApplicationLayer.ClassEventList.CONNECT)

                myGlobal = AppLayer.Open(PortNameAttribute, BaudsAttribute)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    ConnectedAttribute = DirectCast(myGlobal.SetDatos, Boolean)

                    'When successfully inform the connection results
                    If ConnectedAttribute Then
                        PortNameAttribute = AppLayer.ConnectedPortName
                        BaudsAttribute = AppLayer.ConnectedBauds
                    End If
                End If



                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    'SERVICE SOFTWARE 01/07/2011
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True)

                Else
                    'USER SOFTWARE
                    If ConnectedAttribute Then
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "")

                        If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                            'Update analyzer session flags into DataBase
                            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                            End If
                        End If

                        'The next instructions to be sent into the CONNECT process will be managed in ProcessStatusReceived

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessConnection", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Send & Search Next Preparation methods"

        ''' <summary>
        ''' Manage send the next instruction to analyzer while analyzer is in RUNNING (test, isetest, skip, reagent washings, cuvette washing,...)
        ''' 
        ''' Algorithm (NEW 2012/06/07)
        ''' 1) If last estimated future request well = pNextWell and next prep already found (it has found in last machine cycle)
        '''    1.1) If is an execution and it is not locked
        '''         1.1.1) Send it + update a flag actionAlreadySent = TRUE
        '''         1.1.2) If instruction sent is END update flag endRunToSend = TRUE
        ''' 2) Else (initial case)
        '''    2.1) Search for next instruction to be send
        '''    2.29 Send it
        ''' 
        ''' NOTE: the future instruction to be sent is prepared when software receives the fw acceptation of TEST, ISETEST, PTEST, SKIP OR WRUN
        ''' </summary>
        ''' <param name="pNextWell"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 07/06/2012
        ''' Modified by: XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
        ''' </remarks>
        Public Function ManageSendAndSearchNext(ByVal pNextWell As Integer) As GlobalDataTO Implements IAnalyzerEntity.ManageSendAndSearchNext
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim startTime As DateTime = Now 'AG 05/06/2012 - time estimation

                'Flags
                Dim actionAlreadySent As Boolean = False
                Dim endRunInstructionSent As Boolean = False
                Dim systemErrorFlag As Boolean = False

                If AnalyzerIsReadyAttribute Then 'Send only when analyzer is ready 

                    'Check if is an ISE or STD test. If ise the ise module must be ready (else search & send)
                    If myNextPreparationToSendDS.nextPreparation.Rows.Count > 0 AndAlso futureRequestNextWell = pNextWell Then
                        If Not myNextPreparationToSendDS.nextPreparation(0).IsExecutionTypeNull AndAlso myNextPreparationToSendDS.nextPreparation(0).ExecutionType = "PREP_ISE" Then
                            If Not ISEModuleIsReadyAttribute Then
                                myNextPreparationToSendDS.Clear()
                            End If
                        End If
                    End If

                    'If next instruction to be sent (TEST, PTEST, ISETEST, SKIP, WRUN, END, ...) is already found ... proceed to sent it!!
                    If myNextPreparationToSendDS.nextPreparation.Rows.Count > 0 AndAlso futureRequestNextWell = pNextWell Then
                        'Sent it
                        myGlobal = SendNextPreparation(pNextWell, actionAlreadySent, endRunInstructionSent, systemErrorFlag)
                        If actionAlreadySent Then myNextPreparationToSendDS.Clear()
                        'Debug.Print("AnalyzerManager.ManageSendAndSearchNext (future estimation sent): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
                    Else
                        'The next instruction to be sent has not been found yet by Software. It has to Search + Send
                        'CASE (example the 1st request received in RUNNING)
                        myGlobal = Me.SearchNextPreparation(Nothing, pNextWell, ISEModuleIsReadyAttribute) 'Search for next instruction to be sent ... and sent it!!
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
                            If myNextPreparationToSendDS.nextPreparation.Rows.Count > 0 Then
                                myGlobal = SendNextPreparation(pNextWell, actionAlreadySent, endRunInstructionSent, systemErrorFlag)
                                If actionAlreadySent Then myNextPreparationToSendDS.Clear()
                                'Debug.Print("AnalyzerManager.ManageSendAndSearchNext (search + sent): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
                            End If

                            'System error appears while looking for next preparation (Sw sends END and informs pause in process in order to do not send START automatically
                        ElseIf myGlobal.HasError Then
                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
                            If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                                endRunAlreadySentFlagAttribute = True

                                'Send END and mark as it like if used had press paused. Else we will enter into a loop END <-> START <-> END <-> START ....
                                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate

                                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "INPROCESS")
                                myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageSendAndSearchNext", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Search next pending preparation and call the application layer to generate instruction and send it
        ''' If no more PENDING preparation call the application layer to generate EndRunning instruction and send it
        ''' </summary>
        ''' <param name="pNextWell" ></param>
        ''' <param name="pActionSentFlag " >AG 07/06/2012</param>
        ''' <param name="pEndRunSentFlag " >AG 07/06/2012</param>
        ''' <param name="pSystemErrorFlag" >AG 07/06/2012</param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by  AG 16/04/2010
        ''' Modified by AG 17/01/2011 - add parameter NextWell and modify 
        '''             AG 07/06/2012 - Changes for improve speed in STRESS work sessions (the old method is copied and commented with name SendNextPreparationOLD07062012
        '''                 1) This method is called from ManageSendAndSearchNext instead of ManagerAnalyzer
        '''                 2) This method does not search, only send next instruction
        '''             XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
        ''' </remarks>
        Public Function SendNextPreparation(ByVal pNextWell As Integer, ByRef pActionSentFlag As Boolean, ByRef pEndRunSentFlag As Boolean, ByRef pSystemErrorFlag As Boolean) As GlobalDataTO Implements IAnalyzerEntity.SendNextPreparation

            Dim myGlobal As New GlobalDataTO

            Try
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation
                Dim myLogAcciones As New ApplicationLogManager()

                Dim iseStatusOK As Boolean = False
                Dim iseInstalledFlag As Boolean = False
                Dim adjustValue As String = ""
                adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
                If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                    iseInstalledFlag = CType(adjustValue, Boolean)
                End If

                If iseInstalledFlag Then
                    'Ise damaged (ERR) or switch off (WARN)
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_FAILED_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                        iseStatusOK = False
                    Else
                        iseStatusOK = True
                    End If
                End If

                'AG 07/06/2012 - This method only sends, not searchs
                'Search for the next preparation to send
                Dim actionAlreadySent As Boolean = False
                Dim endRunToSend As Boolean = False
                Dim systemErrorFlag As Boolean = False 'AG 25/01/2012 Indicates if search next has produced a system error. In this case send a END instruction
                Dim emptyFieldsDetected As Boolean = False 'AG 03/06/2014 - #1519 when Sw cannot send the proper instruction because there are emtpy fields error then send SKIP

                'myGlobal = Me.SearchNextPreparation(Nothing, pNextWell)
                'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
                'AG 07/06/2012

                If Not myGlobal.HasError Then '(1)
                    Dim myAnManagerDS As New AnalyzerManagerDS
                    'AG 07/06/2012
                    'myAnManagerDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)

                    myAnManagerDS = CType(myNextPreparationToSendDS, AnalyzerManagerDS)

                    If myAnManagerDS.nextPreparation.Rows.Count > 0 Then '(2)
                        'Analyze the row0 found
                        '1st: Check if next cuvette is optically rejected ... send nothing (DUMMY)
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDS.nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull AndAlso myAnManagerDS.nextPreparation(0).CuvetteOpticallyRejectedFlag Then
                                'Send a SKIP instruction but mark as actionAlreadySend. So Ax00 will performe a Dummy cycle 
                                StartTime = Now 'AG 28/06/2012

                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
                                actionAlreadySent = True

                                myLogAcciones.CreateLogActivity("SKIP sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                            End If
                        End If

                        '2on: Check if next cuvette requires washing (cuvette contamination)
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso myAnManagerDS.nextPreparation(0).CuvetteContaminationFlag Then
                                StartTime = Now 'AG 28/06/2012

                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDS)
                                actionAlreadySent = True
                                If Not myGlobal.HasError Then
                                    AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
                                    myLogAcciones.CreateLogActivity("WRUN cuvette sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                Else
                                    'AG 27/09/2012 - the instruction could not be sent because empty fields. The cuvette must continue marked as Contamianted, so nothing to do
                                    ' except remove the error flag and code
                                    If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                        myGlobal.HasError = False
                                        myGlobal.ErrorCode = ""
                                        emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                    End If
                                    'AG 27/09/2012
                                End If
                            End If
                        End If

                        '3rd: Check if next preparation is an ISE preparation
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull AndAlso myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_ISE" Then
                                'endRunToSend = True  'AG 30/11/2011 comment this line (Sw has to send ENDRUN)

                                If iseStatusOK Then 'Only if ISE modul is active 
                                    If Not myAnManagerDS.nextPreparation(0).IsExecutionIDNull AndAlso myAnManagerDS.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        'ISEModuleIsReadyAttribute = False 'AG 27/10/2011 This information is sent by Analzyer (AG 18/01/2011 ISE module becomes not available)
                                        'SGM 08/03/2012
                                        If ISEAnalyzer IsNot Nothing Then
                                            ISEAnalyzer.CurrentProcedure = ISEAnalyzerEntity.ISEProcedures.Test
                                        End If
                                        'end SGM 08/03/2012
                                        StartTime = Now 'AG 28/06/2012

                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
                                        'endRunToSend = False 'AG 30/11/2011 comment this line
                                        actionAlreadySent = True

                                        If myGlobal.HasError Then
                                            'ISEModuleIsReadyAttribute = True 'AG 27/10/2011 This information is sent by Analzyer (AG 18/01/2011 ISE module becomes available again)
                                            'Not needed in this case due the ISE test no affect to the reagent contaminations
                                            'If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS) 'Update sent instructions DS

                                            'AG 27/09/2012 - the instruction could not be sent because empty fields. Affected executions has been LOCKED in GenerateIsePreparation
                                            ' method in Instructions class. Now we have to remove the error flag and code
                                            If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                                'Prepare UIRefreshDS - execution status changed
                                                myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDS.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            End If
                                            'AG 27/09/2012
                                        End If

                                        myLogAcciones.CreateLogActivity("ISETEST sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                    End If 'If myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
                                End If

                            End If 'If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
                        End If 'If Not actionAlreadySent And Not endRunToSend Then

                        '4rh: Check if exists reagent contaminations
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDS.nextPreparation(0).IsReagentContaminationFlagNull AndAlso myAnManagerDS.nextPreparation(0).ReagentContaminationFlag Then
                                StartTime = Now 'AG 28/06/2012

                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDS)
                                actionAlreadySent = True

                                If Not myGlobal.HasError Then
                                    AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
                                    myLogAcciones.CreateLogActivity("WRUN reagents sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                Else
                                    Dim sendWRUNErrorCode As String = myGlobal.ErrorCode 'AG 27/09/2012

                                    'DL 06/07/2012 -In case of error the LOCK execution.
                                    'Lock the next execution to be sent
                                    Dim myWSExecutionDS As New ExecutionsDS
                                    Dim myExecutionDelegate As New ExecutionsDelegate

                                    'get the execution data 
                                    myGlobal = myExecutionDelegate.GetExecution(Nothing, myAnManagerDS.nextPreparation.First.ExecutionID)
                                    If Not myGlobal.HasError Then
                                        myWSExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                        If myWSExecutionDS.twksWSExecutions.Rows.Count > 0 Then
                                            myWSExecutionDS.twksWSExecutions.First.BeginEdit()
                                            myWSExecutionDS.twksWSExecutions.First.ExecutionStatus = "LOCKED"
                                            myWSExecutionDS.twksWSExecutions.First.EndEdit()
                                            myGlobal = myExecutionDelegate.UpdateStatus(Nothing, myWSExecutionDS)

                                            ''make sure send error.
                                            'If Not myGlobal.HasError Then myGlobal.HasError = True
                                            'AG 27/09/2012 - If the instruction could not be sent because empty fields. Remove the error code, when all his treatment has done, else keep the error
                                            If sendWRUNErrorCode = "EMPTY_FIELDS" Then
                                                'Prepare UIRefreshDS - execution status changed
                                                myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDS.nextPreparation.First.ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            Else
                                                myGlobal.HasError = True
                                            End If
                                            'AG 27/09/2012

                                        End If
                                    End If
                                    'DL 06/07/2012 -END.

                                End If
                            End If
                        End If

                        '5rh: Check if next preparation is an STD preparation and executionID <> NO_PENDING_PREPARATION_FOUND
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull AndAlso myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_STD" Then
                                'endRunToSend = True  'AG 30/11/2011 comment this line (Sw has to send ENDRUN)

                                If Not myAnManagerDS.nextPreparation(0).IsExecutionIDNull AndAlso myAnManagerDS.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                    StartTime = Now 'AG 28/06/2012

                                    'AG 31/05/2012 - Evaluate the proper well for contamination or rejection for PTEST case
                                    Dim testPrepData As New PreparationTestDataDelegate
                                    Dim isPTESTinstructionFlag As Boolean = False
                                    myGlobal = testPrepData.isPTESTinstruction(Nothing, myAnManagerDS.nextPreparation(0).ExecutionID, isPTESTinstructionFlag)
                                    If Not isPTESTinstructionFlag Then 'TEST instruction
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
                                        actionAlreadySent = True
                                        If Not myGlobal.HasError Then
                                            AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
                                        Else
                                            'AG 27/09/2012 - the instruction could not be sent because empty fields. Executions has been LOCKED in GeneratePreparation (test o ptest)
                                            ' method in Instructions class. Now we have to remove the error flag and code
                                            If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                                'Prepare UIRefreshDS - execution status changed
                                                myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDS.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            End If
                                            'AG 27/09/2012
                                        End If


                                    Else 'PTEST instruction
                                        'Before send the PTEST instruction Sw has to evaluate the well (pNextWell + WELL_OFFSET_FOR_PREDILUTION)
                                        'looking for cuvette contamination or optical rejection
                                        Dim rejectedPTESTWell As Boolean = False
                                        Dim contaminatePTESTdWell As Boolean = False
                                        Dim WashPTESTSol1 As String = ""
                                        Dim WashPTESTSol2 As String = ""

                                        Dim nextPTESTWell As Integer = pNextWell
                                        'Next well where the PTEST instruction will be perform is (pNextwell + WELL_OFFSET_FOR_PREDILUTION) but from 1 to 120 ciclycal
                                        Dim reactRotorDlg As New ReactionsRotorDelegate
                                        nextPTESTWell = reactRotorDlg.GetRealWellNumber(pNextWell + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)

                                        myGlobal = CheckRejectedContaminatedNextWell(Nothing, nextPTESTWell, rejectedPTESTWell, contaminatePTESTdWell, WashPTESTSol1, WashPTESTSol2)
                                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                            If contaminatePTESTdWell OrElse rejectedPTESTWell Then 'SKIP
                                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
                                                actionAlreadySent = True

                                            Else 'PTEST 
                                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
                                                actionAlreadySent = True
                                                If Not myGlobal.HasError Then
                                                    AddNewSentPreparationsList(Nothing, myAnManagerDS, nextPTESTWell) 'Update sent instructions DS
                                                Else
                                                    'AG 27/09/2012 - the instruction could not be sent because empty fields. Affected executions has been LOCKED in GeneratePreparation (test o ptest)
                                                    ' method in Instructions class. Now we have to remove the error flag and code
                                                    If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                                        'Prepare UIRefreshDS - execution status changed
                                                        myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDS.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                        myGlobal.HasError = False
                                                        myGlobal.ErrorCode = ""
                                                        emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                                    End If
                                                    'AG 27/09/2012
                                                End If


                                            End If
                                        ElseIf myGlobal.HasError Then
                                            systemErrorFlag = True
                                        End If

                                    End If
                                    'AG 31/05/2012

                                    myLogAcciones.CreateLogActivity("TEST or PTEST sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                End If 'If myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
                            End If 'If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
                        End If 'If Not actionAlreadySent And Not endRunToSend Then

                    Else '(2) If myNextPrepDS.nextPreparation.Rows.Count > 0 Then

                    End If
                    'TR 18/10/2011 -Exit try is not needed
                    'Else '(3) If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    '    Exit Try

                ElseIf myGlobal.HasError Then
                    systemErrorFlag = True
                End If

                'AG 30/11/2011 - New conditions for send ENDRUN due the new Ise Request
                '6th: Finally if nothing has been sent decide between send ENDRUN or SKIP
                ' ''6th: Finally if nothing has been sent but the endRunTosend flag is set ... send end run instruction
                'If Not actionAlreadySent AndAlso endRunToSend Then
                '    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                '    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
                'End If

                'New Conditions 
                '- When EMPTY_FIELDS error detected --> Sw sends SKIP
                '- When NO ise installed: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
                '- When ise installed but ise switch off: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
                '- When ise installed and ise switch on: No action sent + AnalyzerIsReady (R:1) + ISEModuleIsReady (I:1) --> Sw sends ENDRUN
                '- Else if No action sent --> Sw sends SKIP
                If emptyFieldsDetected Then
                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP) 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP

                ElseIf Not actionAlreadySent Then
                    If Not systemErrorFlag Then
                        endRunToSend = False
                        If Not iseInstalledFlag AndAlso AnalyzerIsReadyAttribute Then
                            endRunToSend = True
                        ElseIf iseInstalledFlag AndAlso Not iseStatusOK AndAlso AnalyzerIsReadyAttribute Then
                            endRunToSend = True
                        ElseIf iseInstalledFlag AndAlso iseStatusOK Then
                            If AnalyzerIsReadyAttribute AndAlso ISEModuleIsReadyAttribute Then
                                endRunToSend = True
                            End If
                        End If

                    Else 'AG 25/01/2012 - systemErrorFlag: True
                        endRunToSend = True

                        'Send END and mark as it like if used had press paused. Else we will enter into a loop END <-> START <-> END <-> START ....
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate

                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "INPROCESS")
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If

                    If endRunToSend Then 'ENDRUN - no more to be sent
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
                        actionAlreadySent = True
                    Else 'SKIP - No more std tests but some ise test are pending to be sent but the ISE module is working...
                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
                        actionAlreadySent = True
                    End If
                End If
                'AG 30/11/2011

                'AG 07/06/2012 - Update byRef parameters
                pActionSentFlag = actionAlreadySent
                pEndRunSentFlag = endRunToSend
                pSystemErrorFlag = systemErrorFlag
                'AG 07/06/2012

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SendNextPreparation", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Search next pending preparation to send or wash contamination
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNextWell"></param>
        ''' <param name="pLookForISEExecutionsFlag"></param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not
        ''' If no error: Return an AnalyzerManagerDS.nextPreparation (the ExecutionID (integer) to send. If no items found return NO_ITEMS_FOUND (-1)</returns>
        ''' <remarks>
        ''' AG 17/01/2011 - creation
        ''' AG 24/11/2011 - Change priority 1st cuvette washing, 2on optically rejected (skip)
        ''' AG 25/01/2012 - When predilution is found before send the preparation we have to evaluate cuvette washing or skip in well pNextWell + 4 (Sergi 24/01/2012)
        ''' RH 26/06/2012 - Take CONTROLs ISE executions into account
        ''' AG 12/07/2012 - add parameter (optional pLookForISEExecutionsFlag). This parameter is informed only when call this method from ManageSendAndSearchNext
        '''                 and the ise requests is FALSE (I:0)
        ''' </remarks>
        Private Function SearchNextPreparation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNextWell As Integer, Optional ByVal pLookForISEExecutionsFlag As Boolean = True) As GlobalDataTO

            'Dim resultData As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim StartTimeTotal As DateTime = Now
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation
                Dim myLogAcciones As New ApplicationLogManager()
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        Dim nextPreparationDS As New AnalyzerManagerDS
                        Dim nextRow As AnalyzerManagerDS.nextPreparationRow

                        ''''''
                        ''1st: Check if next cuvette is optically rejected 
                        ''''''
                        'Dim rejectedWell As Boolean = False
                        'resultData = Me.CheckOpticalNextWell(dbConnection, pNextWell, rejectedwell)
                        'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(1.1)
                        '    If rejectedwell Then
                        '        nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                        '        nextRow.CuvetteOpticallyRejectedFlag = True

                        '        nextRow.CuvetteContaminationFlag = False
                        '        nextRow.ReagentContaminationFlag = False
                        '        nextRow.ExecutionID = 0
                        '        nextRow.ExecutionType = ""

                        '        nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)
                        '    End If
                        'Else '(1.1)
                        '    Exit Try
                        'End If '(1.1)

                        '''''
                        'AG 24/11/2011 change order: 1st cuvette contamination, 2on optically rejected (first version was inverted)
                        '1st: Check if next cuvette requires washing (cuvette contamination)
                        '2on: Check if next cuvette is optically rejected 
                        '''''
                        Dim rejectedWell As Boolean = False
                        Dim contaminatedWell As Boolean = False
                        Dim WashSol1 As String = ""
                        Dim WashSol2 As String = ""

                        If Not rejectedWell Then '(2.0)
                            resultData = Me.CheckRejectedContaminatedNextWell(dbConnection, pNextWell, rejectedWell, contaminatedWell, WashSol1, WashSol2)
                            myLogAcciones.CreateLogActivity("SearchNextPreparation (CheckRejectedContaminatedNextWell): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                            StartTime = Now

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(2.1)
                                wellContaminatedWithWashSent = 0 'AG 24/11/2011

                                Debug.Print("Step 1 - SearchNextPreparation -> pNextWell = " & pNextWell.ToString & _
                                            "; contaminatedWell = " & contaminatedWell & _
                                            "; rejectedWell = " & rejectedWell & _
                                            "; WashSol2 = " & WashSol2.ToString)

                                If contaminatedWell Then
                                    wellContaminatedWithWashSent = pNextWell 'AG 24/11/2011

                                    nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                                    nextRow.CuvetteContaminationFlag = True

                                    If WashSol1 = "" Then
                                        nextRow.SetWashSolution1Null()
                                    Else
                                        nextRow.WashSolution1 = WashSol1
                                    End If

                                    If WashSol2 = "" Then
                                        nextRow.SetWashSolution2Null()
                                    Else
                                        nextRow.WashSolution2 = WashSol2
                                    End If

                                    nextRow.CuvetteOpticallyRejectedFlag = False
                                    nextRow.ReagentContaminationFlag = False
                                    nextRow.ExecutionID = 0
                                    nextRow.ExecutionType = ""

                                    nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)

                                ElseIf rejectedWell Then
                                    nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                                    nextRow.CuvetteOpticallyRejectedFlag = True

                                    nextRow.CuvetteContaminationFlag = False
                                    nextRow.ReagentContaminationFlag = False
                                    nextRow.ExecutionID = 0
                                    nextRow.ExecutionType = ""

                                    nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)
                                End If 'If contaminatedWell Then
                            Else '(2.1)
                                Exit Try
                            End If '(2.1)
                        End If '(2.0)

                        '''''
                        '3rd: Check if next preparation is an ISE preparation
                        '''''
                        Dim executionFound As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND

                        If Not rejectedWell AndAlso Not contaminatedWell AndAlso pLookForISEExecutionsFlag Then '(3.0)
                            resultData = Me.SearchNextISEPreparationNEW(dbConnection)
                            myLogAcciones.CreateLogActivity("SearchNextPreparation (SearchNextISEPreparationNEW): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                            StartTime = Now

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(3.1)
                                Dim FoundPreparation As ExecutionsDS
                                FoundPreparation = CType(resultData.SetDatos, ExecutionsDS)

                                'RH 26/06/2012 Get data from returned DS
                                If FoundPreparation.twksWSExecutions.Rows.Count > 0 Then
                                    Dim myRow As ExecutionsDS.twksWSExecutionsRow = FoundPreparation.twksWSExecutions(0)

                                    nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                                    nextRow.ExecutionType = "PREP_ISE"
                                    nextRow.ExecutionID = myRow.ExecutionID
                                    nextRow.SampleClass = myRow.SampleClass

                                    nextRow.CuvetteOpticallyRejectedFlag = False
                                    nextRow.CuvetteContaminationFlag = False
                                    nextRow.ReagentContaminationFlag = False
                                    nextRow.SetWashSolution1Null()
                                    nextRow.SetWashSolution2Null()

                                    nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)

                                    executionFound = myRow.ExecutionID 'RH keep the found ExecutionID in executionFound, as previously
                                End If
                            Else '(3.1)
                                Exit Try
                            End If '(3.1)
                        End If '(3.0)

                        '''''
                        '4rh: Check if next preparation is an STD preparation or a contamination wash
                        '''''
                        Dim existContamination As Boolean = False

                        If Not rejectedWell AndAlso Not contaminatedWell AndAlso executionFound = GlobalConstants.NO_PENDING_PREPARATION_FOUND Then '(4.0)
                            Dim sampleClassFound As String = ""

                            resultData = Me.SearchNextSTDPreparation(dbConnection, executionFound, existContamination, WashSol1, WashSol2, sampleClassFound)
                            myLogAcciones.CreateLogActivity("SearchNextPreparation (SearchNextSTDPreparation): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                            StartTime = Now

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then '(4.1)
                                'Check if exist reagents contaminations
                                If existContamination Then '(4.2)

                                    nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                                    nextRow.ReagentContaminationFlag = True

                                    If WashSol1 = "" Then
                                        nextRow.SetWashSolution1Null()
                                        nextRow.SetWashSolution2Null() 'AG 24/02/2012
                                    Else
                                        nextRow.WashSolution1 = WashSol1
                                        nextRow.WashSolution2 = WashSol1 'AG 24/02/2012
                                    End If
                                    'AG 24/02/2012 - reagent contamination uses only one bottle or none but not two
                                    'If WashSol2 = "" Then
                                    '    nextRow.SetWashSolution2Null()
                                    'Else
                                    '    nextRow.WashSolution2 = WashSol2
                                    'End If
                                    'nextRow.ExecutionID = executionFound 'AG + DL 06/07/2012 - Inform the next execution to be sent when the wash was performed
                                    nextRow.CuvetteOpticallyRejectedFlag = False
                                    nextRow.CuvetteContaminationFlag = False

                                    'AG 28/09/2012
                                    'nextRow.ExecutionID = 0
                                    'nextRow.ExecutionType = ""
                                    nextRow.ExecutionID = executionFound 'Execution that requires the washing
                                    nextRow.ExecutionType = "PREP_STD"
                                    'AG 28/09/2012

                                    nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)

                                Else 'STD execution or NO_PENDING_PREPARATION_FOUND
                                    nextRow = nextPreparationDS.nextPreparation.NewnextPreparationRow
                                    nextRow.ExecutionType = "PREP_STD"
                                    nextRow.ExecutionID = executionFound
                                    If Not String.Equals(sampleClassFound, "") Then nextRow.SampleClass = sampleClassFound

                                    nextRow.CuvetteOpticallyRejectedFlag = False
                                    nextRow.CuvetteContaminationFlag = False
                                    nextRow.ReagentContaminationFlag = False
                                    nextRow.SetWashSolution1Null()
                                    nextRow.SetWashSolution2Null()

                                    nextPreparationDS.nextPreparation.AddnextPreparationRow(nextRow)
                                End If '(4.2)
                            End If '(4.1)
                        End If '(4.0

                        'Return data
                        nextPreparationDS.AcceptChanges()

                        resultData.SetDatos = nextPreparationDS

                    End If '(2)
                End If '(1)

                myLogAcciones.CreateLogActivity("SearchNextPreparation (Complete): " & Now.Subtract(StartTimeTotal).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)


            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to be sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            Return resultData

        End Function


        ''' <summary>
        ''' Checks the next well and determine if is optically rejected or not
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNextWell"></param>
        ''' <param name="pRejected " ></param>
        ''' <returns>GlobalDataTo with error or not. ByRef prejected updated</returns>
        ''' <remarks>AG 17/01/2011</remarks>
        Private Function CheckOpticalNextWell(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNextWell As Integer, ByRef pRejected As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        pRejected = False

                        Dim reactionsDlgt As New ReactionsRotorDelegate
                        'Get only the last RotorNumber
                        resultData = reactionsDlgt.ReadWellHistoricalUse(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, pNextWell, False)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myReactionsDS As New ReactionsRotorDS
                            myReactionsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            If myReactionsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                If Not myReactionsDS.twksWSReactionsRotor(0).IsRejectedFlagNull Then
                                    If myReactionsDS.twksWSReactionsRotor(0).RejectedFlag Then
                                        pRejected = True
                                    End If
                                End If

                            End If
                        End If

                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CheckOpticalNextWell", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Checks if next well is contaminated by last reagent used on it or if next well is optically rejected
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNextWell"></param>
        ''' <param name="pRejectedWell " ></param>
        ''' <param name="pContaminatedWell"></param>
        ''' <param name="pWashSol1"></param>
        ''' <param name="pWashSol2"></param>
        ''' <returns>GolbalDataTo indicates if error or not. If no error use the byref parameters</returns>
        ''' <remarks>AG 18/0/2011
        ''' AG 24/11/2011 - Priority 1st Contaminated well, 2on Rejected Well + change the business to prepare the contaminated well washing</remarks>
        Private Function CheckRejectedContaminatedNextWell(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNextWell As Integer, _
                                                    ByRef pRejectedWell As Boolean, ByRef pContaminatedWell As Boolean, _
                                                    ByRef pWashSol1 As String, ByRef pWashSol2 As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        pContaminatedWell = False 'Initialize return byref variables
                        pWashSol1 = ""
                        pWashSol2 = ""

                        Dim myReactionsDelgte As New ReactionsRotorDelegate
                        'Read the well historical use in the current Worksession & Analyzer on parameter well (pNextWell)
                        resultData = myReactionsDelgte.ReadWellHistoricalUse(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, pNextWell, True)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myReactionsDS As New ReactionsRotorDS
                            myReactionsDS = CType(resultData.SetDatos, ReactionsRotorDS)

                            If myReactionsDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                'Look if next well is rejected or not
                                If Not myReactionsDS.twksWSReactionsRotor(0).IsRejectedFlagNull Then
                                    If String.Equals(myReactionsDS.twksWSReactionsRotor.First.WellContent, "E") And myReactionsDS.twksWSReactionsRotor(0).RejectedFlag Then
                                        pRejectedWell = True
                                    End If
                                End If

                                'AG 24/11/2011
                                'If Not pRejectedWell Then
                                '    'Search if next well is reagent contaminated by his last execution on it
                                '    Dim listSTDtest As New List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                '    listSTDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor _
                                '                   Where a.WellContent = "C" Select a).ToList

                                '    If listSTDtest.Count > 0 Then
                                '        'Search the last execution performed in this well
                                '        listSTDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor _
                                '                       Where Not a.IsExecutionIDNull Select a).ToList

                                '        If listSTDtest.Count > 0 Then
                                '            'Look if the last test prepared in this well is a well contaminator or not and his washing mode
                                '            Dim myExecutionsDlgte As New ExecutionsDelegate
                                '            resultData = myExecutionsDlgte.GetExecutionContaminationCuvette(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, listSTDtest(0).ExecutionID)

                                '            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                '                Dim myLocalDS As New AnalyzerManagerDS
                                '                myLocalDS = CType(resultData.SetDatos, AnalyzerManagerDS)

                                '                If myLocalDS.searchNext.Rows.Count > 0 Then
                                '                    If Not myLocalDS.searchNext(0).IsContaminationIDNull Then
                                '                        'Last TEST is a well contaminator test ... look if a wash cuvette cycle has been already sent
                                '                        'Else activate Contaminated Well flag
                                '                        Dim myWellContaminatorExecutionID As Integer = myLocalDS.searchNext(0).ExecutionID
                                '                        Dim washAlreadySend As Boolean = False

                                '                        For Each itemRow As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor.Rows
                                '                            If Not itemRow.IsExecutionIDNull Then
                                '                                If itemRow.ExecutionID = myWellContaminatorExecutionID Then Exit For
                                '                            End If

                                '                            'If thw washFlag is TRUE means the cuvette has been washed
                                '                            If itemRow.WashedFlag Then 'Wash cuvette contaminator has been sent after sending myWellContaminatorExecutionID
                                '                                washAlreadySend = True
                                '                                Exit For
                                '                            End If
                                '                        Next

                                '                        If Not washAlreadySend Then
                                '                            pContaminatedWell = True
                                '                            If Not myLocalDS.searchNext(0).IsWashingSolution1Null Then pWashSol1 = myLocalDS.searchNext(0).WashingSolution1
                                '                            If Not myLocalDS.searchNext(0).IsWashingSolution2Null Then pWashSol2 = myLocalDS.searchNext(0).WashingSolution2

                                '                            wellContaminatedWithWashSent = myWellContaminatorExecutionID 'Inform the class variable for use it into method MarkWashRunningAccepted
                                '                        End If

                                '                    End If
                                '                End If
                                '            End If

                                '        End If
                                '    End If 'If listSTDtest.Count > 0 Then

                                'End If 'If Not pRejectedWell Then

                                'Search if next well is reagent contaminated by his last execution on it
                                Dim listSTDtest As New List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                Dim rotorTurnWithWellContamination As Integer = 0
                                listSTDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor _
                                               Where a.WellContent = "C" AndAlso a.WashRequiredFlag = True AndAlso a.WashedFlag = False _
                                               Select a Order By a.RotorTurn Descending).ToList

                                If listSTDtest.Count > 0 Then
                                    'Get the last cuvette contaminator washing information (WashingSolutionR1, WashingSolutionR2)
                                    pContaminatedWell = True
                                    rotorTurnWithWellContamination = listSTDtest(0).RotorTurn
                                    If Not listSTDtest(0).IsWashingSolutionR1Null Then pWashSol1 = listSTDtest(0).WashingSolutionR1
                                    If Not listSTDtest(0).IsWashingSolutionR2Null Then pWashSol2 = listSTDtest(0).WashingSolutionR2
                                End If 'If listSTDtest.Count > 0 Then

                                'If well contaminated search if the previous well contamination has been washed or not
                                If pContaminatedWell Then
                                    listSTDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor _
                                                   Where a.WashRequiredFlag = True AndAlso a.WashedFlag = True _
                                                   Select a Order By a.RotorTurn Descending).ToList
                                    If listSTDtest.Count > 0 Then
                                        If listSTDtest(0).RotorTurn > rotorTurnWithWellContamination Then pContaminatedWell = False
                                    End If
                                End If
                                'AG 24/11/2011
                                listSTDtest = Nothing 'AG 02/08/2012 release memory
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.CheckRejectedContaminatedNextWell", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Search if Sw has to send a new ISE execution
        ''' Requires de ISEModule is ready, search execution inside current patient or inside previous ones
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionISEFound"></param>
        ''' <returns>GolbalDataTo indicates if error or not. If no error use the byref parameters</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Private Function SearchNextISEPreparation(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pExecutionISEFound As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        If ISEModuleIsReadyAttribute Then '(3) 'Search ISE prep only when the ISE module is ready
                            Dim execDel As New ExecutionsDelegate
                            resultData = execDel.GetNextPendingISEPatientExecution(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)

                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                pExecutionISEFound = CType(resultData.SetDatos, Integer)
                            End If

                        End If '(3)
                    End If '(2)
                End If '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextISEPreparation", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Search if Sw has to send a new ISE execution
        ''' Requires the ISEModule is ready, search execution inside current patient or inside previous ones
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>An ExecutionsDS inside a GlobalDataTO with the data found</returns>
        ''' <remarks>
        ''' Created by: RH 26/06/2012
        ''' AG 11/07/2012 - remove IseModuleIsReadyAttribute from IF (now Sw search next using the status accepted instruction and this instruction contains the field I:0)
        ''' </remarks>
        Private Function SearchNextISEPreparationNEW(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    'If (Not dbConnection Is Nothing) AndAlso ISEModuleIsReadyAttribute Then
                    If (Not dbConnection Is Nothing) Then
                        Dim execDel As New ExecutionsDelegate
                        'AG 11/07/2012
                        'resultData = execDel.GetNextPendingISEExecution(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
                        resultData = execDel.GetNextPendingISEExecutionNEW(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
                    Else
                        resultData.SetDatos = New ExecutionsDS
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextISEPreparationNEW", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Returns the next STD execution in the same patient - sampletype to perform without contamination
        ''' If not is possible then returns a contamination wash to perform
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionSTDFound"></param>
        ''' <param name="pReagentWashFlag"></param>
        ''' <param name="pWashSol1"></param>
        ''' <param name="pWashSol2"></param>
        ''' <param name="pSampleClassFound" ></param>
        ''' <returns>GolbalDataTo indicates if error or not. If no error use the byref parameters</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Private Function SearchNextSTDPreparation(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pExecutionSTDFound As Integer, _
                                                  ByRef pReagentWashFlag As Boolean, ByRef pWashSol1 As String, _
                                                  ByRef pWashSol2 As String, ByRef pSampleClassFound As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        'Initialize byRef variables (parameters)
                        pExecutionSTDFound = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                        pReagentWashFlag = False
                        pWashSol1 = ""
                        pWashSol2 = ""

                        Dim myExecDlg As New ExecutionsDelegate
                        resultData = myExecDlg.GetPendingExecutionForSendNextProcess(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, True)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(3)
                            Dim myExecList As New ExecutionsDS 'AnalyzerManagerDS
                            myExecList = CType(resultData.SetDatos, ExecutionsDS)


                            If myExecList.twksWSExecutions.Rows.Count > 0 Then '(4)
                                '// 0) Remember the last OrderID - SampleType sent to prepare
                                Dim lastOrderID As String = ""
                                Dim lastSampleType As String = ""
                                Dim lastSampleClass As String = ""

                                Dim sentLinqList As New List(Of AnalyzerManagerDS.sentPreparationsRow)
                                sentLinqList = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
                                                Where String.Equals(a.ExecutionType, "PREP_STD") _
                                                Select a).ToList

                                If sentLinqList.Count > 0 Then
                                    If Not sentLinqList(sentLinqList.Count - 1).IsOrderIDNull Then lastOrderID = sentLinqList(sentLinqList.Count - 1).OrderID
                                    If Not sentLinqList(sentLinqList.Count - 1).IsSampleTypeNull Then lastSampleType = sentLinqList(sentLinqList.Count - 1).SampleType
                                    If Not sentLinqList(sentLinqList.Count - 1).IsSampleClassNull Then lastSampleClass = sentLinqList(sentLinqList.Count - 1).SampleClass
                                End If

                                'AG 28/11/2011 - 1) Get all R1 Contaminations and HIGH contamination persistance
                                Dim contaminationsDataDS As ContaminationsDS = Nothing
                                Dim highContaminationPersitance As Integer = 0

                                Dim myContaminationsDelegate As New ContaminationsDelegate
                                resultData = myContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    contaminationsDataDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                                    'Dim swParametersDlg As New SwParametersDelegate
                                    'resultData = swParametersDlg.ReadNumValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS.ToString, Nothing)
                                    'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    '    highContaminationPersitance = CInt(resultData.SetDatos)
                                    'End If
                                    highContaminationPersitance = REAGENT_CONTAMINATION_PERSISTANCE
                                End If
                                'AG 28/11/2011

                                Dim found As Boolean = False
                                Dim myStatFlag As Boolean = True
                                Dim nextToSend As New AnalyzerManagerDS

                                Do
                                    '// First search STATs, if no items found then search ROUTINEs

                                    '// 1) Search myStatFlag executions with the same OrderID-SampleType as lastOrderID-lastSampleType
                                    '// (priority without contaminations with the last sent)
                                    If Not String.Equals(lastOrderID, "") And Not String.Equals(lastSampleType, "") And Not String.Equals(lastSampleClass, "PATIENT") Then
                                        'resultData = GetNextExecutionUntil28112011(found, myExecList, lastOrderID, lastSampleType, myStatFlag, "")
                                        resultData = GetNextExecution(dbConnection, found, myExecList, lastOrderID, lastSampleType, myStatFlag, "", contaminationsDataDS, highContaminationPersitance)
                                    End If


                                    '// 2) If no PENDING items found in 1) then
                                    '//Sorting by SampleClass search myStatFlag executions with the different OrderID-SampleType as lastOrderID-lastSampleType
                                    '// (priority without contaminations with the last sent)
                                    If Not resultData.HasError And Not found Then
                                        'resultData = GetNextExecutionUntil28112011(found, myExecList, "", "", myStatFlag, "BLANK")
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "BLANK", contaminationsDataDS, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        'resultData = GetNextExecutionUntil28112011(found, myExecList, "", "", myStatFlag, "CALIB")
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "CALIB", contaminationsDataDS, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        'resultData = GetNextExecutionUntil28112011(found, myExecList, "", "", myStatFlag, "CTRL")
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "CTRL", contaminationsDataDS, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        'resultData = GetNextExecutionUntil28112011(found, myExecList, "", "", myStatFlag, "PATIENT")
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "PATIENT", contaminationsDataDS, highContaminationPersitance)
                                    End If

                                    myStatFlag = Not myStatFlag
                                Loop Until (myStatFlag Or found Or resultData.HasError)


                                'Finally prepare data 
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing And found Then '(5)
                                    nextToSend = CType(resultData.SetDatos, AnalyzerManagerDS)
                                    If nextToSend.searchNext.Rows.Count > 0 Then '(6)

                                        If Not nextToSend.searchNext(0).IsContaminationIDNull Then 'Exist contamination ... send wash
                                            pReagentWashFlag = True
                                            pExecutionSTDFound = nextToSend.searchNext(0).ExecutionID 'AG + DL 06/07/2012 - This is the next execution to be sent after the wash was performed
                                            If Not nextToSend.searchNext(0).IsWashingSolution1Null Then
                                                pWashSol1 = nextToSend.searchNext(0).WashingSolution1
                                            End If
                                        Else 'No contamination .... send test preparation
                                            pExecutionSTDFound = nextToSend.searchNext(0).ExecutionID
                                            If Not nextToSend.searchNext(0).IsSampleClassNull Then pSampleClassFound = nextToSend.searchNext(0).SampleClass
                                        End If

                                    End If '(6)

                                ElseIf Not resultData.HasError And Not found Then '(5)
                                    resultData.SetDatos = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                End If ''(5)
                                sentLinqList = Nothing 'AG 02/08/2012 release memory

                            End If '(4) If myExecList.searchNext.Rows.Count > 0 Then

                        End If '(3)

                    End If '(2)
                End If '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextSTDPreparation", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Inform the instruction sent in Running (only STD test preparation, or WRUN (reagent contamination) or WRUN (cuvette contamination)
        ''' into the structure that contains the last instructions sent used for calculate the next to be send
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pNewPreparationSent"></param>
        ''' <param name="pNextWell"></param>
        ''' <returns></returns>
        ''' <remarks>Modified AG 10/01/2014</remarks>
        Private Function AddNewSentPreparationsList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewPreparationSent As AnalyzerManagerDS, ByVal pNextWell As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        If pNewPreparationSent.nextPreparation.Rows.Count > 0 Then '(3)
                            Dim added As Boolean = False
                            Dim nextRow As AnalyzerManagerDS.sentPreparationsRow

                            'CASE cuvette washing: Add new row with cuvette washing
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsCuvetteContaminationFlagNull Then '(4)
                                If pNewPreparationSent.nextPreparation(0).CuvetteContaminationFlag Then '(4.1)
                                    nextRow = mySentPreparationsDS.sentPreparations.NewsentPreparationsRow
                                    'nextRow.ReagentID = 0 'Do not use NULL due 
                                    nextRow.CuvetteWashFlag = pNewPreparationSent.nextPreparation(0).CuvetteContaminationFlag
                                    nextRow.ReagentWashFlag = False
                                    nextRow.SetExecutionTypeNull()
                                    nextRow.SetSampleClassNull()
                                    If Not pNewPreparationSent.nextPreparation(0).IsWashSolution1Null Then
                                        nextRow.WashSolution1 = pNewPreparationSent.nextPreparation(0).WashSolution1
                                    Else
                                        nextRow.SetWashSolution1Null()
                                    End If
                                    If Not pNewPreparationSent.nextPreparation(0).IsWashSolution2Null Then
                                        nextRow.WashSolution2 = pNewPreparationSent.nextPreparation(0).WashSolution2
                                    Else
                                        nextRow.SetWashSolution2Null()
                                    End If

                                    mySentPreparationsDS.sentPreparations.AddsentPreparationsRow(nextRow)
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                    added = True

                                End If '(4.1)
                            End If '(4)

                            'CASE reagent washing: Add new row with reagent washing
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsReagentContaminationFlagNull Then '(5)
                                If pNewPreparationSent.nextPreparation(0).ReagentContaminationFlag Then '(5.1)
                                    nextRow = mySentPreparationsDS.sentPreparations.NewsentPreparationsRow

                                    nextRow.ReagentWashFlag = pNewPreparationSent.nextPreparation(0).ReagentContaminationFlag
                                    nextRow.CuvetteWashFlag = False
                                    nextRow.SetExecutionTypeNull()
                                    nextRow.SetSampleClassNull()
                                    If Not pNewPreparationSent.nextPreparation(0).IsWashSolution1Null Then
                                        nextRow.WashSolution1 = pNewPreparationSent.nextPreparation(0).WashSolution1
                                    Else
                                        nextRow.SetWashSolution1Null()
                                    End If
                                    If Not pNewPreparationSent.nextPreparation(0).IsWashSolution2Null Then
                                        nextRow.WashSolution2 = pNewPreparationSent.nextPreparation(0).WashSolution2
                                    Else
                                        nextRow.SetWashSolution2Null()
                                    End If

                                    mySentPreparationsDS.sentPreparations.AddsentPreparationsRow(nextRow)
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                    added = True

                                End If '(5.1)
                            End If '(5)


                            'CASE STD preparation: Add new row with STD preparation
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsExecutionIDNull Then '(6)
                                Dim myExecution As Integer = pNewPreparationSent.nextPreparation(0).ExecutionID

                                'Get execution additional information
                                Dim execDelg As New ExecutionsDelegate
                                Dim myExecDS As New AnalyzerManagerDS

                                resultData = execDelg.GetSendPreparationDataByExecution(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myExecution)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myExecDS = CType(resultData.SetDatos, AnalyzerManagerDS)
                                Else
                                    Exit Try
                                End If

                                'Add row to mySentPreparationsDS
                                If myExecDS.searchNext.Rows.Count > 0 Then '(6.1)
                                    nextRow = mySentPreparationsDS.sentPreparations.NewsentPreparationsRow
                                    nextRow.CuvetteWashFlag = False
                                    nextRow.ReagentWashFlag = False
                                    nextRow.ExecutionID = myExecDS.searchNext(0).ExecutionID
                                    nextRow.StatFlag = myExecDS.searchNext(0).StatFlag
                                    nextRow.SampleClass = myExecDS.searchNext(0).SampleClass
                                    nextRow.ExecutionType = myExecDS.searchNext(0).ExecutionType
                                    nextRow.ReagentID = myExecDS.searchNext(0).ReagentID
                                    nextRow.OrderTestID = myExecDS.searchNext(0).OrderTestID
                                    nextRow.OrderID = myExecDS.searchNext(0).OrderID
                                    nextRow.SampleType = myExecDS.searchNext(0).SampleType
                                    nextRow.WellUsed = pNextWell
                                    mySentPreparationsDS.sentPreparations.AddsentPreparationsRow(nextRow)
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                    added = True
                                End If '(6.1)

                                'Finally delete the PREP_STD rows that exceeds the algorithm slope
                                '1st: If there are more PREP_STD than the persistance ... delete the older ones
                                Dim resLinq As List(Of AnalyzerManagerDS.sentPreparationsRow) = _
                                    (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
                                     Where String.Equals(a.ExecutionType, "PREP_STD") _
                                     Select a).ToList

                                If resLinq.Count = 0 Then 'If no PREP_STD clear the DataSet table
                                    mySentPreparationsDS.sentPreparations.Clear()
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                Else
                                    For i As Integer = 0 To resLinq.Count - REAGENT_CONTAMINATION_PERSISTANCE - 1
                                        resLinq(i).Delete()
                                    Next
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                End If

                                'AG 10/01/2014 - BT #1432 (sometimes Software does not send the reagent contamination wash)
                                'CHANGE RULE: When a new TEST is sent remove all previous Wash sent (reagents or cuvettes)
                                'The old was wrong and also the index used where wrong!!! - (Now, I think it has no sense)

                                '2on NEW RULE: When send a new PREP_STD clear all previous wash
                                resLinq = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
                                           Where a.ReagentWashFlag = True Or a.CuvetteWashFlag = True _
                                           Select a).ToList
                                If resLinq.Count > 0 Then '(6.2) 
                                    For i As Integer = resLinq.Count - 1 To 0 Step -1
                                        resLinq(i).Delete()
                                    Next
                                    mySentPreparationsDS.sentPreparations.AcceptChanges()
                                End If
                                'AG 10/01/2014

                                'End deletion rows process...

                                resLinq = Nothing 'AG 02/08/2012 release memory
                            End If '(6)
                        End If '(3)
                    End If '(2)
                End If '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextSTDPreparation", EventLogEntryType.Error, False)

                'Finally
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

            Return resultData
        End Function


        ''' <summary>
        ''' Search next STD preparation, taking care about the last executions sent and search next avoiding contaminations
        ''' using the executions matching the OrderID, SampleType, StatFlag and SampleClass in entry parameters
        ''' 
        ''' This method look for the next execution sorting using the same algoritm that the used in CreateWSExecutions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pFound" ></param>
        ''' <param name="pSTDExecutionList" ></param>
        ''' <param name="pOrderID " ></param>
        ''' <param name="pSampleType " ></param>
        ''' <param name="pStatFlag" ></param>
        ''' <param name="pSampleClass" ></param>
        ''' <param name="pContaminationsDS"></param>
        ''' <param name="pHighContaminationPersitance"></param>
        ''' <returns>AnalyzerManager.searchNext inside a GlobalDataTo</returns>
        ''' <remarks>AG 25/01/2011 - Created</remarks>
        Private Function GetNextExecution(ByVal pDBConnection As SqlClient.SqlConnection, _
                                          ByRef pFound As Boolean, _
                                          ByVal pSTDExecutionList As ExecutionsDS, _
                                          ByVal pOrderID As String, _
                                          ByVal pSampleType As String, _
                                          ByVal pStatFlag As Boolean, _
                                          ByVal pSampleClass As String, _
                                          ByVal pContaminationsDS As ContaminationsDS, _
                                          ByVal pHighContaminationPersitance As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)

                        'Get the list of pending executions depending the entry parameters case
                        'Apply different LINQ (where clause)
                        Dim toSendList As New List(Of ExecutionsDS.twksWSExecutionsRow)
                        If pOrderID <> "" And pSampleType <> "" Then '(3)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pSTDExecutionList.twksWSExecutions _
                                            Where a.OrderID = pOrderID AndAlso a.SampleType = pSampleType AndAlso a.StatFlag = pStatFlag _
                                            Select a).ToList
                        ElseIf pOrderID <> "" And pSampleType <> "" And pSampleClass <> "" Then '(1)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pSTDExecutionList.twksWSExecutions _
                                            Where a.OrderID = pOrderID AndAlso a.SampleType = pSampleType AndAlso a.StatFlag = pStatFlag AndAlso a.SampleClass = pSampleClass _
                                            Select a).ToList

                        ElseIf pSampleClass <> "" Then '(1)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pSTDExecutionList.twksWSExecutions _
                                            Where a.StatFlag = pStatFlag AndAlso a.SampleClass = pSampleClass _
                                            Select a).ToList

                        Else '(3)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pSTDExecutionList.twksWSExecutions _
                                            Where a.StatFlag = pStatFlag _
                                            Select a).ToList

                        End If '(End 3)

                        'AG 02/03/2012 - CALIB, CTRLS and PATIENTS can not leave the current element until finish with it
                        '                Besides, the PATIENTS must follow the creationorder
                        '                We must edit the toSendList and add the ElementID information <CalibratorID (calibrators) or ControlID (controls) or CreationOrder (patients)>
                        If toSendList.Count > 0 AndAlso pSampleClass <> "BLANK" Then
                            Dim allOrderTestsDS As OrderTestsForExecutionsDS
                            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                            resultData = myWSOrderTestsDelegate.GetInfoOrderTestsForExecutions(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                allOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                                Dim auxOrderTestID As Integer = -1
                                Dim otInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow)

                                For Each row As ExecutionsDS.twksWSExecutionsRow In toSendList
                                    If row.OrderTestID <> auxOrderTestID Then
                                        auxOrderTestID = row.OrderTestID
                                        'Search information for the Calibrator or control or patient ordertest
                                        otInfo = (From a In allOrderTestsDS.OrderTestsForExecutionsTable _
                                                    Where a.OrderTestID = auxOrderTestID _
                                                   Select a).ToList()
                                    End If

                                    If otInfo.Count >= 1 Then 'AG 10/04/2012 old condition fails when same test has 2 control levels (If otInfo.Count = 1 Then)
                                        row.BeginEdit()
                                        If pSampleClass = "PATIENT" Then
                                            row.ElementID = otInfo(0).CreationOrder
                                        Else
                                            row.ElementID = otInfo(0).ElementID 'CalibratorID or ControlID
                                        End If
                                        row.AcceptChanges()
                                        row.EndEdit()
                                    End If
                                Next
                                otInfo = Nothing

                                Dim auxElementID As Integer = -1
                                auxElementID = toSendList(0).ElementID 'Apply linq using the same ElementID as the first item in toSendLinq previous results

                                toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In toSendList _
                                              Where a.ElementID = auxElementID Select a).ToList
                            End If

                        End If
                        'AG 02/03/2012


                        Dim nextExecutionFound As Boolean = False
                        Dim indexNextToSend As Integer = 0

                        If toSendList.Count > 0 Then '(3)

                            '1) Search contamination between previous reagents and the first in toSendList
                            Dim previousReagentIDSentList As List(Of AnalyzerManagerDS.sentPreparationsRow) 'The last reagents used are in the higher array indexes
                            Dim contaminations As List(Of ContaminationsDS.tparContaminationsRow)
                            Dim contaminationFound As Boolean = False
                            Dim myLogAcciones As New ApplicationLogManager() 'Add temporal traces 'AG 28/03/2014 - #1563

                            previousReagentIDSentList = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
                                                     Where a.ExecutionType = "PREP_STD" Select a).ToList

                            If previousReagentIDSentList.Count > 0 Then '(3.bis)
                                '1.1) Check if exists contamination between last reagent and next reagent (LOW or HIGH contamiantion)
                                contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                        Where wse.ReagentContaminatorID = previousReagentIDSentList(previousReagentIDSentList.Count - 1).ReagentID _
                                                        AndAlso wse.ReagentContaminatedID = toSendList(0).ReagentID _
                                                        Select wse).ToList()

                                If contaminations.Count > 0 Then '(4)
                                    contaminationFound = True 'LOW or HIGH contamination found

                                    'Check if the required wash has been already sent or not
                                    Dim requiredWash As String = ""
                                    If Not previousReagentIDSentList(previousReagentIDSentList.Count - 1).IsWashSolution1Null Then requiredWash = previousReagentIDSentList(previousReagentIDSentList.Count - 1).WashSolution1

                                    'AG 28/03/2014 - #1563 it is not necessary modify the next line , ExecutionID can not be NULL because the list has been get using Linq where executionType = PREP_STD
                                    Dim previousExecutionsIDSent As Integer = previousReagentIDSentList(previousReagentIDSentList.Count - 1).ExecutionID

                                    Dim i As Integer = 0
                                    'DL 04/07/2012. Begin
                                    'Search the proper row in mySentPreparationsDS.sentPreparations
                                    'For i = 0 To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                    'If previousExecutionsIDSent = mySentPreparationsDS.sentPreparations(i).ExecutionID Then
                                    'Exit For
                                    'End If
                                    'Next i
                                    'DL 04/07/2012. End

                                    'Search if the proper wash has been already sent or not
                                    For i = i To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                        If mySentPreparationsDS.sentPreparations(i).ReagentWashFlag = True AndAlso _
                                            mySentPreparationsDS.sentPreparations(i).WashSolution1 = requiredWash Then
                                            contaminationFound = False
                                            Exit For
                                        End If
                                    Next

                                ElseIf pHighContaminationPersitance > 0 Then '(4)
                                    '1.2) If no LOW contamination exists between the last reagent used and next take care about the previous due the high contamination
                                    'has persistance > 1
                                    Dim highIndex As Integer = 0
                                    For highIndex = previousReagentIDSentList.Count - pHighContaminationPersitance To previousReagentIDSentList.Count - 2 'The index -1 has already been evaluated
                                        If highIndex >= 0 Then 'Avoid overflow
                                            contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                              Where wse.ReagentContaminatorID = previousReagentIDSentList(highIndex).ReagentID _
                                                              AndAlso wse.ReagentContaminatedID = toSendList(0).ReagentID _
                                                              AndAlso Not wse.IsWashingSolutionR1Null _
                                                              Select wse).ToList()
                                            If contaminations.Count > 0 Then
                                                contaminationFound = True 'HIGH contamination found

                                                'Check if the required wash has been already sent or not
                                                Dim requiredWash As String = ""
                                                If Not previousReagentIDSentList(highIndex).IsWashSolution1Null Then requiredWash = previousReagentIDSentList(highIndex).WashSolution1

                                                'AG 28/03/2014 - #1563 it is not necessary modify the next line , ExecutionID can not be NULL because the list has been get using Linq where executionType = PREP_STD
                                                Dim previousExecutionsIDSent As Integer = previousReagentIDSentList(highIndex).ExecutionID

                                                Dim i As Integer = 0
                                                'Search the proper row in mySentPreparationsDS.sentPreparations
                                                For i = 0 To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                                    'AG 28/03/2014 - #1563 evaluate that ExecutionID is not NULL
                                                    'If previousExecutionsIDSent = mySentPreparationsDS.sentPreparations(i).ExecutionID Then
                                                    If Not mySentPreparationsDS.sentPreparations(i).IsExecutionIDNull AndAlso previousExecutionsIDSent = mySentPreparationsDS.sentPreparations(i).ExecutionID Then
                                                        Exit For
                                                    ElseIf mySentPreparationsDS.sentPreparations(i).IsExecutionIDNull Then
                                                        myLogAcciones.CreateLogActivity("Protection! Otherwise the bug #1563 was triggered", "AnalyzerManager.GetNextExecution", EventLogEntryType.Information, False)
                                                    End If
                                                    'AG 28/03/2014 - #1563 
                                                Next

                                                'Search if the proper wash has been already sent or not
                                                For i = i To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                                    If mySentPreparationsDS.sentPreparations(i).ReagentWashFlag = True AndAlso _
                                                        mySentPreparationsDS.sentPreparations(i).WashSolution1 = requiredWash Then
                                                        contaminationFound = False
                                                        Exit For
                                                    End If
                                                Next

                                            End If
                                        End If

                                        If contaminationFound Then Exit For
                                    Next

                                End If '(EndIf 4)
                            End If '(3.bis)

                            '2) If exists contamination between previous reagents sent and next in list, so sort the pending executions using the same algortihm 
                            'as in WS Creation and try found a better solution, then send the FIRST
                            '<Take a look of method ExecutionsDelegate.SortWSExecutionsByElementGroupContaminationNew>

                            'When a WRUN instruction has to be sent ... these variables contains contaminatorID and wash type ("" or WS1 or WS2 or ...)
                            Dim myContaminationID As Integer = -1
                            Dim myWashSolutionType As String = ""

                            If contaminationFound Then '(4)
                                Dim myExDlgte As New ExecutionsDelegate
                                Dim contaminNumber As Integer = 0

                                '2.1) Calculate contaminations number with current executions sort
                                contaminNumber = 1 + myExDlgte.GetContaminationNumber(pContaminationsDS, toSendList, pHighContaminationPersitance)

                                If contaminNumber > 0 Then '(5)
                                    Dim bestResultList As List(Of ExecutionsDS.twksWSExecutionsRow)
                                    Dim currentResultList As List(Of ExecutionsDS.twksWSExecutionsRow)
                                    Dim bestContaminationNumber As Integer = 0
                                    Dim currentContaminationNumber As Integer = 0

                                    'AG 19/12/2011
                                    Dim myReagentsIDList As New List(Of Integer) 'List of previous reagents sent before the current previousElementLastReagentID, 
                                    '                                                   remember this information in order to check the high contamination persistance (One Item for each different OrderTest)
                                    Dim myMaxReplicatesList As New List(Of Integer) 'AG 19/12/2011 - Same item number as previous list, indicates the replicate number for each item in previous list

                                    'Transform previousReagentIDSentList List(Of AnalyzerManagerDS.sentPreparationsRow) into List (Of Integer): PreviousReagentsIDList and previousMaxReplicatesList
                                    '(the nearest reagents use the higher indexs)
                                    Dim maxReplicates As Integer = 0
                                    For i = 0 To previousReagentIDSentList.Count - 1
                                        If myReagentsIDList.Count = 0 Then myReagentsIDList.Add(previousReagentIDSentList(i).ReagentID)
                                        maxReplicates += 1

                                        'When change reagent inform max replicates into previousMaxReplicatesList
                                        If myReagentsIDList(myReagentsIDList.Count - 1) <> previousReagentIDSentList(i).ReagentID Then
                                            myMaxReplicatesList.Add(maxReplicates) 'Previous reagent max replicates
                                            myReagentsIDList.Add(previousReagentIDSentList(i).ReagentID) 'New reagent
                                            maxReplicates = 1 'Initialize max replicates
                                        End If
                                    Next
                                    If myReagentsIDList.Count > 0 Then
                                        myMaxReplicatesList.Add(maxReplicates) 'Last reagent max replicates
                                    End If
                                    'AG 19/12/2011

                                    '2.2) If contaminations: ApplyOptimizationPolicyANew, ApplyOptimizationPolicyBNew, ApplyOptimizationPolicyCNew and ApplyOptimizationPolicyDNew
                                    'and choose the best solution

                                    'Original sort result
                                    currentResultList = toSendList.ToList() 'Initial order
                                    bestContaminationNumber = contaminNumber
                                    currentContaminationNumber = contaminNumber
                                    bestResultList = toSendList.ToList()

                                    'Apply Optimization Policy A (move contaminated OrderTest down until it becomes no contaminated)
                                    currentContaminationNumber = myExDlgte.ApplyOptimizationPolicyANew(pContaminationsDS, currentResultList, pHighContaminationPersitance, myReagentsIDList, myMaxReplicatesList)

                                    'Accept policy A only when improves '' Assume it is the best result.
                                    If currentContaminationNumber < bestContaminationNumber Then
                                        bestContaminationNumber = currentContaminationNumber
                                        bestResultList = currentResultList
                                    End If

                                    'Apply Optimization Policy B.(move contaminated OrderTest up until it becomes no contaminated)
                                    If currentContaminationNumber > 0 Then
                                        currentResultList = toSendList.ToList() 'Initial order.ToList()
                                        currentContaminationNumber = myExDlgte.ApplyOptimizationPolicyBNew(pContaminationsDS, currentResultList, pHighContaminationPersitance, myReagentsIDList, myMaxReplicatesList)

                                        If currentContaminationNumber < bestContaminationNumber Then
                                            bestContaminationNumber = currentContaminationNumber
                                            bestResultList = currentResultList
                                        End If
                                    End If

                                    'Apply Optimization Policy C. (move contaminator OrderTest down until it no contaminates)
                                    If currentContaminationNumber > 0 Then
                                        currentResultList = toSendList.ToList() 'Initial order.ToList()
                                        currentContaminationNumber = myExDlgte.ApplyOptimizationPolicyCNew(pContaminationsDS, currentResultList, pHighContaminationPersitance, myReagentsIDList, myMaxReplicatesList)
                                        If currentContaminationNumber < bestContaminationNumber Then
                                            bestContaminationNumber = currentContaminationNumber
                                            bestResultList = currentResultList
                                        End If
                                    End If

                                    'Apply Optimization Policy D. (move contaminator OrderTest up until it no contaminates)
                                    If currentContaminationNumber > 0 Then
                                        currentResultList = toSendList.ToList() 'Initial order.ToList()
                                        currentContaminationNumber = myExDlgte.ApplyOptimizationPolicyDNew(pContaminationsDS, currentResultList, pHighContaminationPersitance, myReagentsIDList, myMaxReplicatesList)
                                        If currentContaminationNumber < bestContaminationNumber Then
                                            bestContaminationNumber = currentContaminationNumber
                                            bestResultList = currentResultList
                                        End If
                                    End If

                                    toSendList = bestResultList

                                    '2.3) Finally check if exists contamination between last reagents used and next reagent that will be used (High or Low contamination)
                                    'If contamination sent Wash, else sent toSendList(0).ExecutionID
                                    'NOTE: previousReagentIDSentList contains the last reagents used, the nearest in time used are the higher array indexes
                                    Dim highIndex As Integer = 0
                                    'For highIndex = previousReagentIDSentList.Count - pHighContaminationPersitance To previousReagentIDSentList.Count - 1
                                    For highIndex = previousReagentIDSentList.Count - 1 To previousReagentIDSentList.Count - pHighContaminationPersitance Step -1
                                        If highIndex < 0 Then

                                        Else
                                            If highIndex < previousReagentIDSentList.Count - 1 Then 'Evaluate only High contamination
                                                contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                                  Where wse.ReagentContaminatorID = previousReagentIDSentList(highIndex).ReagentID _
                                                                  AndAlso wse.ReagentContaminatedID = toSendList(0).ReagentID _
                                                                  AndAlso Not wse.IsWashingSolutionR1Null _
                                                                  Select wse).ToList()

                                            Else 'With the last reagents sent evaluate both High or Low contamination
                                                contaminations = (From wse In pContaminationsDS.tparContaminations _
                                                                  Where wse.ReagentContaminatorID = previousReagentIDSentList(highIndex).ReagentID _
                                                                  AndAlso wse.ReagentContaminatedID = toSendList(0).ReagentID _
                                                                  Select wse).ToList()
                                            End If

                                            If contaminations.Count > 0 Then
                                                'Check if the required wash has been already sent or not
                                                If Not contaminations(0).IsContaminationIDNull Then myContaminationID = contaminations(0).ContaminationID

                                                myWashSolutionType = ""
                                                'If Not previousReagentIDSentList(highIndex).IsWashSolution1Null Then myWashSolutionType = previousReagentIDSentList(highIndex).WashSolution1
                                                If Not contaminations(0).IsWashingSolutionR1Null Then myWashSolutionType = contaminations(0).WashingSolutionR1

                                                'AG 28/03/2014 - #1563 it is not necessary modify the next line , ExecutionID can not be NULL because the list has been get using Linq where executionType = PREP_STD
                                                Dim previousExecutionsIDSent As Integer = previousReagentIDSentList(highIndex).ExecutionID

                                                Dim i As Integer = 0
                                                'Search the proper row in mySentPreparationsDS.sentPreparations
                                                For i = 0 To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                                    'AG 28/03/2014 - #1563 evaluate that ExecutionID is not NULL
                                                    'If previousExecutionsIDSent = mySentPreparationsDS.sentPreparations(i).ExecutionID Then
                                                    If Not mySentPreparationsDS.sentPreparations(i).IsExecutionIDNull AndAlso previousExecutionsIDSent = mySentPreparationsDS.sentPreparations(i).ExecutionID Then
                                                        Exit For
                                                    ElseIf mySentPreparationsDS.sentPreparations(i).IsExecutionIDNull Then
                                                        myLogAcciones.CreateLogActivity("Protection! Otherwise the bug #1563 was triggered", "AnalyzerManager.GetNextExecution", EventLogEntryType.Information, False)
                                                    End If
                                                    'AG 28/03/2014 - #1563 
                                                Next

                                                'Search if the proper wash has been already sent or not
                                                contaminationFound = True
                                                nextExecutionFound = False
                                                For i = i To mySentPreparationsDS.sentPreparations.Rows.Count - 1
                                                    If mySentPreparationsDS.sentPreparations(i).ReagentWashFlag = True AndAlso _
                                                        mySentPreparationsDS.sentPreparations(i).WashSolution1 = myWashSolutionType Then

                                                        contaminationFound = False
                                                        nextExecutionFound = True
                                                        indexNextToSend = 0
                                                        Exit For
                                                    End If
                                                Next

                                                If contaminationFound Then Exit For

                                            End If
                                        End If
                                    Next

                                    'AG 24/02/2012 - This code is placed because before in this case the Sw do not send anything an Fw do a Dummy
                                    If Not contaminations Is Nothing AndAlso contaminations.Count = 0 Then
                                        nextExecutionFound = True
                                        indexNextToSend = 0
                                    End If
                                    'AG 24/02/2012

                                Else '(5) (If contaminNumber = 0 Then)
                                    nextExecutionFound = True
                                    indexNextToSend = 0
                                End If

                            Else '(4) If contaminationFound Then
                                'If no contamination between previous reagents sent and the next one so sent it
                                nextExecutionFound = True
                                indexNextToSend = 0

                            End If '(End 4)If contaminationFound Then


                            'Once the best option is found prepare the variable to return
                            Dim myReturn As New AnalyzerManagerDS
                            Dim myRow As AnalyzerManagerDS.searchNextRow

                            pFound = True ' Inform something is found to be sent: execution or wash
                            If nextExecutionFound Then '(4)
                                'Prepare output DS with the proper information (execution to be sent)
                                myRow = myReturn.searchNext.NewsearchNextRow
                                myRow.ExecutionID = toSendList(indexNextToSend).ExecutionID
                                myRow.SampleClass = toSendList(indexNextToSend).SampleClass
                                myRow.SetContaminationIDNull()
                                myRow.SetWashingSolution1Null()
                                myReturn.searchNext.AddsearchNextRow(myRow)

                            Else
                                'Contamination (wash has to be sent)
                                myRow = myReturn.searchNext.NewsearchNextRow
                                myRow.ExecutionID = toSendList(indexNextToSend).ExecutionID 'AG + DL 06/07/2012 'GlobalConstants.NO_PENDING_PREPARATION_FOUND
                                myRow.SetSampleClassNull()
                                myRow.ContaminationID = myContaminationID
                                myRow.WashingSolution1 = myWashSolutionType
                                myReturn.searchNext.AddsearchNextRow(myRow)
                            End If '(4)
                            resultData.SetDatos = myReturn

                            contaminations = Nothing 'AG 02/08/2012 release memory
                            previousReagentIDSentList = Nothing 'AG 02/08/2012 release memory

                            'Else '(3) If toSendList.Count > 0 Then
                            'No exectution matches with linq where criteria
                        End If '(End 3)
                        toSendList = Nothing 'AG 02/08/2012 release memory
                    End If '(End 2)
                End If '(End 1)

            Catch ex As Exception
                resultData.HasError = True 'AG 19/06/2012
                resultData.ErrorCode = "SYSTEM_ERROR" 'AG 19/06/2012
                resultData.ErrorMessage = ex.Message 'AG 19/06/2012

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecution", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Adjust light methods"
        ''' <summary>
        ''' Search the next not rejected well and send an ALIGHT instruction
        ''' </summary>
        ''' <param name="pNextWell"></param>
        ''' <returns></returns>
        ''' <remarks>AG 30/05/2011 - creation</remarks>
        Private Function SendAdjustLightInstruction(ByVal pNextWell As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim adjustWell As Integer = pNextWell

                'Change rotor adjustWell = 1, else look for the 1st not rejected
                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess.ToString) <> "INPROCESS" Then
                    'Read current reactions rotor well status (rejection)
                    Dim myDelegate As New ReactionsRotorDelegate
                    myGlobal = myDelegate.GetAllWellsLastTurn(Nothing, AnalyzerIDAttribute)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        Dim reactionsDS As New ReactionsRotorDS
                        reactionsDS = CType(myGlobal.SetDatos, ReactionsRotorDS)

                        'Use linq to search the next NOT rejected well
                        '1st exist the current well? No: Use it / Yes: Search next NOT rejected
                        Dim myList As New List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                        myList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In reactionsDS.twksWSReactionsRotor _
                                  Where a.WellNumber = pNextWell Select a).ToList

                        If myList.Count > 0 Then
                            'Search next NOT rejected (1st from pNextWell .. 120)
                            myList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In reactionsDS.twksWSReactionsRotor _
                                      Where a.WellNumber >= pNextWell And a.RejectedFlag = False Select a Order By a.WellNumber Ascending).ToList
                            If myList.Count > 0 Then
                                adjustWell = myList(0).WellNumber
                            Else
                                '(2on from 1 to pNextWell)
                                myList = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In reactionsDS.twksWSReactionsRotor _
                                        Where a.WellNumber < pNextWell And a.RejectedFlag = False Select a Order By a.WellNumber Ascending).ToList
                                If myList.Count > 0 Then
                                    adjustWell = myList(0).WellNumber
                                End If
                            End If

                        Else 'If all wells are rejected ... use the parameter well value
                            adjustWell = pNextWell
                        End If
                        myList = Nothing 'AG 02/08/2012 release memory
                    End If
                End If

                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ALIGHT, adjustWell)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SendAdjustLightInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Get the Current (last) baseline ID in order to inform every execution with his correct base line
        ''' 
        ''' If pBaseLineWithAdjust = TRUE is ANSAL get the ID from twksWSBLines table, else get from twksWSBLinesByWell table
        ''' 
        ''' NOTE: Dont use connection TEMPLATE in order to avoid circular references (DataAccessLayer uses Global proyect)!!
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWell"></param>
        ''' <param name="pBaseLineWithAdjust"></param>
        ''' <param name="pType">STATIC or DYNAMIC</param>
        ''' <returns>GlobalDataTO indicating if error. If no error returns and integer with the current BaseLineID</returns>
        ''' <remarks>
        ''' Created by AG ?
        ''' Modified by AG 03/01/2011 - if pBaseLineWithAdjust = true (if ANSAL get current baselineid from twksWSBLines, else (FALSE) get from twksWSBLinesByWell
        ''' AG 29/10/2014 BA-2064 adapt method to read the static or dynamic base line (add pType parameter)
        ''' </remarks>
        Public Function GetCurrentBaseLineIDByType(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                             ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean, ByVal pType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pdbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If pBaseLineWithAdjust Then 'Base line with adjust (ANSAL instruction received)
                            'Option1: Using table WSBLinesDelegate (adjustment base line)
                            Dim myDelegate As New WSBLinesDelegate
                            resultData = myDelegate.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWell, pType)

                        Else 'base line without adjust (ANSBL or ANSDL instruction received)
                            ''Option2: Using table WSBLinesByWellDelegate (1 base line in every well during washing cycles)
                            Dim myDelegate2 As New WSBLinesByWellDelegate
                            resultData = myDelegate2.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWell)
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetCurrentBaseLineIDByType", EventLogEntryType.Error, False)
            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Save the new base line performed results (INSERT or UPDATE) depending if the BaseLineId exists or not
        ''' 
        ''' If not exist Create
        ''' Else Update
        ''' 
        ''' If pBaseLineWithAdjust = TRUE is ANSAL (save into twksWSBLines) is ANSBL or ANSDL (save into twksWSBLinesByWell)
        ''' 
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pBaseLineDS"></param>
        ''' <param name="pBaseLineWithAdjust"></param>
        ''' <param name="pType"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 20/05/2010
        ''' AG 03/01/2011 - new parameter pBaseLineWithAdjust if ANSAL then save into twksWSBLines, else save into twksWSBLinesByWell
        ''' AG 29/10/2014 BA-2057 inform new parameter pType in method blDelegate.Exists
        ''' 
        ''' </remarks>
        Private Function SaveBaseLineResults(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pBaseLineDS As BaseLinesDS, ByVal pBaseLineWithAdjust As Boolean, ByVal pType As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobal = DAOBase.GetOpenDBTransaction(pdbConnection)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        ' Method business
                        If pBaseLineDS.twksWSBaseLines.Rows.Count > 0 Then
                            If Not pBaseLineDS.twksWSBaseLines(0).IsBaseLineIDNull Then

                                If pBaseLineWithAdjust Then '(1) Base line with adjust ("ANSAL" instruction received)
                                    'Option1 using table twksBLines
                                    Dim blDelegate As New WSBLinesDelegate
                                    myGlobal = blDelegate.Exists(dbConnection, pBaseLineDS.twksWSBaseLines(0).AnalyzerID, pBaseLineDS.twksWSBaseLines(0).WorkSessionID, _
                                                                 pBaseLineDS.twksWSBaseLines(0).BaseLineID, pBaseLineDS.twksWSBaseLines(0).Wavelength, pType)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        If DirectCast(myGlobal.SetDatos, Boolean) = True Then
                                            'Update
                                            myGlobal = blDelegate.Update(dbConnection, pBaseLineDS)
                                        Else
                                            'Create
                                            myGlobal = blDelegate.Create(dbConnection, pBaseLineDS)
                                        End If
                                    End If

                                Else '(1) base line without adjust (ANSBL or ANSDL received)

                                    'Option2 using table twksBLinesByWell
                                    Dim blDelegate2 As New WSBLinesByWellDelegate
                                    myGlobal = blDelegate2.Exists(dbConnection, pBaseLineDS.twksWSBaseLines(0).AnalyzerID, pBaseLineDS.twksWSBaseLines(0).WorkSessionID, pBaseLineDS.twksWSBaseLines(0).BaseLineID, pBaseLineDS.twksWSBaseLines(0).WellUsed)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        If DirectCast(myGlobal.SetDatos, Boolean) = True Then
                                            'Update
                                            myGlobal = blDelegate2.Update(dbConnection, pBaseLineDS)
                                        Else
                                            'Create
                                            myGlobal = blDelegate2.Create(dbConnection, pBaseLineDS)
                                        End If
                                    End If
                                End If '(1)

                            End If
                        End If

                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pdbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pdbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pdbConnection Is Nothing) And _
                   (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SaveBaseLineResults", EventLogEntryType.Error, False)

            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Search the current BaseLineId an check if is complete
        ''' 
        ''' If complete: returns Current BaseLineID + 1
        ''' Else: returns Current BaseLineID
        ''' 
        ''' If pBaseLineWithAdjust = TRUE is ANSAL uses the twksWSBLines table
        ''' If pBaseLineWithAdjust = FALSE is ANSBL or ANSDL uses the twksWSBLinesByWell table
        ''' 
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWellUsed"></param>
        ''' <param name="pBaseLineWithAdjust">AG 03/01/2011</param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not. If no error the setData = Integer</returns>
        ''' <remarks>
        ''' Created by AG 21/05/2010
        ''' AG 03/01/2011 - use twksWSBLines or twksWSBLinesByWell depending pBaseLineWithAdjust parameter value
        ''' AG 29/10/2014 BA-2057 and BA-2062 define new optional parameter and inform it as part of the new parameters in myDelegate.GetCurrentBaseLineID
        ''' </remarks>
        Private Function GetNextBaseLineID(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                           ByVal pWorkSessionID As String, ByVal pWellUsed As Integer, ByVal pBaseLineWithAdjust As Boolean, _
                                           Optional ByVal pType As String = "STATIC", Optional ByVal pLed As Integer = -1) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pdbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim currBaseLineID As Integer = 0
                        Dim incrementIDFlag As Boolean = False

                        If pBaseLineWithAdjust Then 'Base line with adjust (ansal instruction received)
                            ''Option1: Using table WSBLinesDelegate (adjustment base line)
                            Dim myDelegate As New WSBLinesDelegate
                            'AG 29/10/2014 BA-2062 - We are saving a new value, so get the maximum independent of the Type
                            resultData = myDelegate.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWellUsed, "")
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                currBaseLineID = DirectCast(resultData.SetDatos, Integer)
                            Else
                                Exit Try
                            End If

                            incrementIDFlag = True

                            'If saving STATIC then next baseline ID is current+1 (code does it)
                            'If DYNAMIC:
                            '- Read base line with ID = currBaseLineID
                            '    - If the type is STATIC then next ID = current+1
                            '    - Else: If already exists for well, led then nextId = current+1
                            If currBaseLineID > 0 AndAlso pType = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then
                                resultData = myDelegate.Read(dbConnection, pAnalyzerID, pWorkSessionID, currBaseLineID, pLed, "")
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    Dim auxDS As New BaseLinesDS
                                    auxDS = DirectCast(resultData.SetDatos, BaseLinesDS)

                                    If auxDS.twksWSBaseLines.Rows.Count > 0 AndAlso auxDS.twksWSBaseLines(0).Type = GlobalEnumerates.BaseLineType.DYNAMIC.ToString Then
                                        Dim linqRes As List(Of BaseLinesDS.twksWSBaseLinesRow)
                                        linqRes = (From a As BaseLinesDS.twksWSBaseLinesRow In auxDS.twksWSBaseLines _
                                                   Where a.WellUsed = pWellUsed AndAlso a.Wavelength = pLed Select a).ToList
                                        If linqRes.Count = 0 Then
                                            incrementIDFlag = False
                                        End If
                                    End If

                                End If
                            End If

                        Else 'ANSBL or ANSDL base line without adjust
                            'Option2: Using table WSBLinesByWellDelegate (1 base line in every well during washing cycles)
                            Dim myDelegate2 As New WSBLinesByWellDelegate
                            resultData = myDelegate2.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWellUsed)
                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                currBaseLineID = DirectCast(resultData.SetDatos, Integer)
                            Else
                                Exit Try
                            End If
                            incrementIDFlag = True

                        End If

                        'Return the next baselineid depending if the current is completed or not
                        If incrementIDFlag = True Then
                            resultData.SetDatos = currBaseLineID + 1  'Next BaseLine = Current + 1
                        Else
                            resultData.SetDatos = currBaseLineID    'Next BaseLine = Current 
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextBaseLineID", EventLogEntryType.Error, False)
            Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return resultData

        End Function

#End Region

    End Class

End Namespace
