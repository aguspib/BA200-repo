Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports System.Data
Imports System.IO
'AG 20/04/2011 - added when create instance to an BackGroundWorker

Namespace Biosystems.Ax00.CommunicationsSwFw

    Partial Public Class AnalyzerManager


#Region "Temporally testing Methods"

        ''' <summary>
        ''' This auxiliary method allow us simulate the instructions reception events
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/05/2010
        ''' </remarks>
        Public Function SimulateInstructionReception(ByVal pInstructionReceived As String, Optional ByVal pReturnData As Object = Nothing) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.RECEIVE, pReturnData, pInstructionReceived)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SimulateInstructionReception", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Simulate send next preparation
        ''' 
        ''' </summary>
        ''' <param name="pNextWell ">entre 1 y 120</param>
        ''' <returns></returns>
        ''' <remarks>Created by AG,RH 27/06/2012</remarks>
        Public Function SimulateSendNext(ByVal pNextWell As Integer) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            Try
                ISEModuleIsReadyAttribute = True 'Simulation
                myGlobal = SearchNextPreparation(Nothing, pNextWell)

            Catch ex As Exception
                myGlobal = New GlobalDataTO()
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SimulateSendNext", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' This auxiliary method allow us simulate the basic alarm management (save in database and inform to presentation layer)
        '''         ''' </summary>
        ''' <param name="pAlarmList" ></param>
        ''' <param name="pAlarmStatusList" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 03/05/2012
        ''' </remarks>
        Public Function SimulateAlarmsManagement(ByVal pAlarmList As List(Of GlobalEnumerates.Alarms), ByVal pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim finalAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim finalAlarmStatusList As New List(Of Boolean)

                For item As Integer = 0 To pAlarmList.Count - 1
                    PrepareLocalAlarmList(pAlarmList(item), pAlarmStatusList(item), finalAlarmList, finalAlarmStatusList)
                Next

                If finalAlarmList.Count > 0 Then
                    'myGlobal = ManageAlarms(Nothing, pAlarmList, pAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        myGlobal = ManageAlarms_SRV(Nothing, finalAlarmList, finalAlarmStatusList)
                    Else
                        myGlobal = ManageAlarms(Nothing, finalAlarmList, finalAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SimulateAlarmsManagement", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Public Function SimulateRequestAdjustmentsFromAnalyzer(ByVal pPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'PENDING 
                'REQUEST ADJUSTMENTS TO ANALYZER BY CORRESPONDING SCRIPT

                'SIMULATE
                Dim myData As String = ""
                'Dim myGlobalbase As New GlobalBase
                Dim objReader As StreamReader
                Dim path As String = pPath
                objReader = New StreamReader(path)
                myData = objReader.ReadToEnd()
                objReader.Close()



                myGlobal = AppLayer.ReadFwAdjustmentsDS()
                If myGlobal.SetDatos IsNot Nothing Then
                    Dim myAdjDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If myAdjDS IsNot Nothing Then
                        Dim myAdjustmentsDelegate As New FwAdjustmentsDelegate(myAdjDS)
                        myGlobal = myAdjustmentsDelegate.ConvertReceivedDataToDS(myData, MyClass.ActiveAnalyzer, MyClass.ActiveFwVersion)
                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
            End Try
            Return myGlobal
        End Function



        Public Function ProcessArmCollisionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            myGlobal = ProcessArmCollisionDuringRecover(pPrepWithKO)

            Return myGlobal
        End Function

        Public Function ProcessClotDetectionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            myGlobal = ProcessClotDetectionDuringRecover(pPrepWithKO)

            Return myGlobal
        End Function

        Public Function ProcessLevelDetectionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            myGlobal = ProcessLevelDetectionDuringRecover(pPrepWithKO)

            Return myGlobal
        End Function

#End Region

#Region "Commented OLD functions"

        '''' <summary>
        '''' Search next pending preparation and call the application layer to generate instruction and send it
        '''' 
        '''' If no more PENDING preparation call the application layer to generate EndRunning instruction and send it
        '''' </summary>
        '''' <param name="pNextWell" ></param>
        '''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        '''' <remarks>
        '''' Creation AG 16/04/2010
        '''' AG 17/01/2011 - add parameter NextWell and modify 
        '''' </remarks>
        'Public Function SendNextPreparationOLD07062012(ByVal pNextWell As Integer) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim startTime As DateTime = Now 'AG 05/06/2012 - time estimation

        '        Dim iseStatusOK As Boolean = False
        '        Dim iseInstalledFlag As Boolean = False
        '        Dim adjustValue As String = ""
        '        adjustValue = ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
        '        If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
        '            iseInstalledFlag = CType(adjustValue, Boolean)
        '        End If

        '        If iseInstalledFlag Then
        '            'Ise damaged (ERR) or switch off (WARN)
        '            If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_FAILED_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
        '                iseStatusOK = False
        '            Else
        '                iseStatusOK = True
        '            End If
        '        End If


        '        'Search for the next preparation to send
        '        Dim actionAlreadySent As Boolean = False
        '        Dim endRunToSend As Boolean = False
        '        Dim systemErrorFlag As Boolean = False 'AG 25/01/2012 Indicates if search next has produced a system error. In this case send a END instruction
        '        myGlobal = Me.SearchNextPreparation(Nothing, pNextWell)

        '        Debug.Print("AnalyzerManager.Search Next: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
        '        startTime = Now

        '        'iteraToDelete += 1 'Temporal code for SKIP testing
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
        '            Dim myAnManagerDS As New AnalyzerManagerDS
        '            myAnManagerDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)

        '            If myAnManagerDS.nextPreparation.Rows.Count > 0 Then '(2)
        '                'Analyze the row0 found
        '                '1st: Check if next cuvette is optically rejected ... send nothing (DUMMY)
        '                If Not actionAlreadySent And Not endRunToSend Then
        '                    If Not myAnManagerDS.nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull Then
        '                        If myAnManagerDS.nextPreparation(0).CuvetteOpticallyRejectedFlag Then
        '                            'Send a SKIP instruction but mark as actionAlreadySend. So Ax00 will performe a Dummy cycle
        '                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
        '                            actionAlreadySent = True
        '                        End If
        '                    End If
        '                End If

        '                '2on: Check if next cuvette requires washing (cuvette contamination)
        '                If Not actionAlreadySent And Not endRunToSend Then
        '                    If Not myAnManagerDS.nextPreparation(0).IsCuvetteContaminationFlagNull Then
        '                        If myAnManagerDS.nextPreparation(0).CuvetteContaminationFlag Then
        '                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDS)
        '                            actionAlreadySent = True

        '                            If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
        '                        End If
        '                    End If
        '                End If

        '                '3rd: Check if next preparation is an ISE preparation
        '                If Not actionAlreadySent And Not endRunToSend Then
        '                    If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
        '                        If myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_ISE" Then
        '                            'endRunToSend = True  'AG 30/11/2011 comment this line (Sw has to send ENDRUN)

        '                            If iseStatusOK Then 'Only if ISE modul is active 
        '                                If Not myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
        '                                    If myAnManagerDS.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
        '                                        'ISEModuleIsReadyAttribute = False 'AG 27/10/2011 This information is sent by Analzyer (AG 18/01/2011 ISE module becomes not available)
        '                                        'SGM 08/03/2012
        '                                        If MyClass.ISE_Manager IsNot Nothing Then
        '                                            MyClass.ISE_Manager.CurrentProcedure = ISEManager.ISEProcedures.Test
        '                                        End If
        '                                        'end SGM 08/03/2012
        '                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
        '                                        'endRunToSend = False 'AG 30/11/2011 comment this line
        '                                        actionAlreadySent = True

        '                                        If myGlobal.HasError Then
        '                                            'ISEModuleIsReadyAttribute = True 'AG 27/10/2011 This information is sent by Analzyer (AG 18/01/2011 ISE module becomes available again)
        '                                            'Not needed in this case due the ISE test no affect to the reagent contaminations
        '                                            'If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS) 'Update sent instructions DS
        '                                        End If

        '                                    End If

        '                                End If 'If myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
        '                            End If

        '                        End If 'If myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_ISE" Then
        '                    End If 'If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
        '                End If 'If Not actionAlreadySent And Not endRunToSend Then

        '                '4rh: Check if exists reagent contaminations
        '                If Not actionAlreadySent And Not endRunToSend Then
        '                    If Not myAnManagerDS.nextPreparation(0).IsReagentContaminationFlagNull Then
        '                        If myAnManagerDS.nextPreparation(0).ReagentContaminationFlag Then
        '                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDS)
        '                            actionAlreadySent = True

        '                            If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
        '                        End If
        '                    End If
        '                End If

        '                '5rh: Check if next preparation is an STD preparation and executionID <> NO_PENDING_PREPARATION_FOUND
        '                If Not actionAlreadySent And Not endRunToSend Then
        '                    If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
        '                        If myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_STD" Then
        '                            'endRunToSend = True  'AG 30/11/2011 comment this line (Sw has to send ENDRUN)

        '                            If Not myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
        '                                If myAnManagerDS.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
        '                                    'AG 31/05/2012 - Evaluate the proper well for contamination or rejection for PTEST case
        '                                    Dim testPrepData As New PreparationTestDataDelegate
        '                                    Dim isPTESTinstructionFlag As Boolean = False
        '                                    myGlobal = testPrepData.isPTESTinstruction(Nothing, myAnManagerDS.nextPreparation(0).ExecutionID, isPTESTinstructionFlag)
        '                                    If Not isPTESTinstructionFlag Then 'TEST instruction
        '                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
        '                                        actionAlreadySent = True
        '                                        If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS

        '                                    Else 'PTEST instruction
        '                                        'Before send the PTEST instruction Sw has to evaluate the well (pNextWell + WELL_OFFSET_FOR_PREDILUTION)
        '                                        'looking for cuvette contamination or optical rejection
        '                                        Dim rejectedPTESTWell As Boolean = False
        '                                        Dim contaminatePTESTdWell As Boolean = False
        '                                        Dim WashPTESTSol1 As String = ""
        '                                        Dim WashPTESTSol2 As String = ""

        '                                        Dim nextPTESTWell As Integer = pNextWell
        '                                        'Next well where the PTEST instruction will be perform is (pNextwell + WELL_OFFSET_FOR_PREDILUTION) but from 1 to 120 ciclycal
        '                                        Dim reactRotorDlg As New ReactionsRotorDelegate
        '                                        nextPTESTWell = reactRotorDlg.GetRealWellNumber(pNextWell + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)

        '                                        myGlobal = CheckRejectedContaminatedNextWell(Nothing, nextPTESTWell, rejectedPTESTWell, contaminatePTESTdWell, WashPTESTSol1, WashPTESTSol2)
        '                                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '                                            If contaminatePTESTdWell OrElse rejectedPTESTWell Then 'SKIP
        '                                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
        '                                                actionAlreadySent = True

        '                                            Else 'PTEST 
        '                                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDS.nextPreparation(0).ExecutionID)
        '                                                actionAlreadySent = True
        '                                                If Not myGlobal.HasError Then AddNewSentPreparationsList(Nothing, myAnManagerDS, nextPTESTWell) 'Update sent instructions DS

        '                                            End If
        '                                        ElseIf myGlobal.HasError Then
        '                                            systemErrorFlag = True
        '                                        End If

        '                                    End If
        '                                    'AG 31/05/2012
        '                                End If

        '                            End If 'If myAnManagerDS.nextPreparation(0).IsExecutionIDNull Then
        '                        End If 'If myAnManagerDS.nextPreparation(0).ExecutionType = "PREP_STD" Then
        '                    End If 'If Not myAnManagerDS.nextPreparation(0).IsExecutionTypeNull Then
        '                End If 'If Not actionAlreadySent And Not endRunToSend Then

        '            Else '(2) If myNextPrepDS.nextPreparation.Rows.Count > 0 Then

        '            End If
        '            'TR 18/10/2011 -Exit try is not needed
        '            'Else '(3) If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            '    Exit Try

        '        ElseIf myGlobal.HasError Then
        '            systemErrorFlag = True
        '        End If

        '        'AG 30/11/2011 - New conditions for send ENDRUN due the new Ise Request
        '        '6th: Finally if nothing has been sent decide between send ENDRUN or SKIP
        '        ' ''6th: Finally if nothing has been sent but the endRunTosend flag is set ... send end run instruction
        '        'If Not actionAlreadySent AndAlso endRunToSend Then
        '        '    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
        '        '    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
        '        'End If

        '        'New Conditions 
        '        '- When NO ise installed: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
        '        '- When ise installed but ise switch off: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
        '        '- When ise installed and ise switch on: No action sent + AnalyzerIsReady (R:1) + ISEModuleIsReady (I:1) --> Sw sends ENDRUN
        '        '- Else if No action sent --> Sw sends SKIP
        '        If Not actionAlreadySent Then
        '            If Not systemErrorFlag Then
        '                endRunToSend = False
        '                If Not iseInstalledFlag AndAlso AnalyzerIsReadyAttribute Then
        '                    endRunToSend = True
        '                ElseIf iseInstalledFlag AndAlso Not iseStatusOK AndAlso AnalyzerIsReadyAttribute Then
        '                    endRunToSend = True
        '                ElseIf iseInstalledFlag AndAlso iseStatusOK Then
        '                    If AnalyzerIsReadyAttribute AndAlso ISEModuleIsReadyAttribute Then
        '                        endRunToSend = True
        '                    End If
        '                End If

        '            Else 'AG 25/01/2012 - systemErrorFlag: True
        '                endRunToSend = True

        '                'Send END and mark as it like if used had press paused. Else we will enter into a loop END <-> START <-> END <-> START ....
        '                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
        '                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate

        '                UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "INPROCESS")
        '                myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
        '            End If

        '            If endRunToSend Then 'ENDRUN - no more to be sent
        '                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
        '                actionAlreadySent = True
        '            Else 'SKIP - No more std tests but some ise test are pending to be sent but the ISE module is working...
        '                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
        '                actionAlreadySent = True
        '            End If
        '        End If
        '        'AG 30/11/2011

        '        Debug.Print("AnalyzerManager.Send Instruction: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
        '        'Debug.Print("AnalyzerManager.SendNext (TOTAL): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SendNextPreparation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        'This method was used when no contaminations and no ISE management was implemented
        '''' <summary>
        '''' Search next pending preparation
        '''' Priority criteriom:
        '''' 1st Stats
        '''' 2st SampleClass (blank, calib, ctrl, patients)
        '''' 3st MultiItemNumber (ASC) (without change ordertestid)
        '''' 4st Replicate number (ASC) (without change ordertestid) 
        '''' </summary>
        '''' <returns> GlobalDataTo indicating if an error has occurred or not
        '''' If no error: Return the ExecutionID (integer) to send. If no items found return NO_ITEMS_FOUND (-1)</returns>
        '''' <remarks>
        '''' Creation GDS 19/04/2010 (Revision AG 19/04/2010)
        '''' </remarks>
        'Private Function SearchNextPreparation17012011() As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        Dim myExecution As New ExecutionsDelegate
        '        Dim myStatFlag As Boolean = True
        '        Dim foundItem As Boolean = False

        '        If AnalyzerIDAttribute = "" Or WorkSessionIDAttribute = "" Then '(1) 'AG 03/11/2010 If no exists analyzer or worksession then return NO PENDING PREPARATION
        '            myGlobal.SetDatos = GlobalConstants.NO_PENDING_PREPARATION_FOUND

        '        Else
        '            myGlobal = DAOBase.GetOpenDBConnection(Nothing)

        '            If Not myGlobal.HasError Then '(2)
        '                dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

        '                Do
        '                    If Not foundItem Then
        '                        myGlobal = myExecution.GetNextPreparation(dbConnection, "BLANK", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute)
        '                        If Not myGlobal.HasError Then
        '                            foundItem = (CType(myGlobal.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                        Else
        '                            Exit Try
        '                        End If
        '                    End If

        '                    If Not foundItem Then
        '                        myGlobal = myExecution.GetNextPreparation(dbConnection, "CALIB", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute)
        '                        If Not myGlobal.HasError Then
        '                            foundItem = (CType(myGlobal.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                        Else
        '                            Exit Try
        '                        End If
        '                    End If

        '                    If Not foundItem Then
        '                        myGlobal = myExecution.GetNextPreparation(dbConnection, "CTRL", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute)
        '                        If Not myGlobal.HasError Then
        '                            foundItem = (CType(myGlobal.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                        Else
        '                            Exit Try
        '                        End If
        '                    End If

        '                    If Not foundItem Then
        '                        myGlobal = myExecution.GetNextPreparation(dbConnection, "PATIENT", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute)
        '                        If Not myGlobal.HasError Then
        '                            foundItem = (CType(myGlobal.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                        Else
        '                            Exit Try
        '                        End If
        '                    End If

        '                    myStatFlag = Not myStatFlag
        '                Loop Until myStatFlag Or foundItem

        '            End If '(2)
        '        End If '(1)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function



        'AG 05/05/2010 - Comment this function
        'OLD functions that where used before changes in Ax00 Physical layer (INIT instruction) -May 2010
        '''''' <summary>
        '''''' Main function in analyzer management class
        '''''' 
        '''''' Receives an action identifier and depending the analyzer state, alarms,... decide what is the
        '''''' next action to be performed
        '''''' </summary>
        '''''' <param name="pAction"></param>
        '''''' <param name="pInstructionReceived">Optional parameter, only informed when Software receives instructions</param>
        '''''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        '''''' <remarks>
        '''''' Creation AG 16/04/2010
        '''''' </remarks>
        ' ''Public Function ManageAnalyzer05052010(ByVal pAction As ClassActionList, _
        ' ''                               Optional ByVal pInstructionReceived As List(Of InstructionParameterTO) = Nothing) As GlobalDataTO

        ' ''    Dim myGlobal As New GlobalDataTO

        ' ''    Try
        ' ''        If CommThreadsStartedAttribute Then

        ' ''            Select Case pAction
        ' ''                Case ClassActionList.INIT_COMMS
        ' ''                    'Ax5 (Comm_CapaApp.Abrir) // iPRO (GestorEstados.Abrir)
        ' ''                    InitCommAttribute = False
        ' ''                    myGlobal = AppLayer.Open(PortNameAttribute, BaudsAttribute)
        ' ''                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        ' ''                        InitCommAttribute = DirectCast(myGlobal.SetDatos, Boolean)

        ' ''                        'When successfully inform the connection results
        ' ''                        If InitCommAttribute Then
        ' ''                            PortNameAttribute = AppLayer.ConnectedPortName
        ' ''                            BaudsAttribute = AppLayer.ConnectedBauds
        ' ''                        End If

        ' ''                    Else
        ' ''                        InitCommAttribute = False
        ' ''                    End If
        ' ''                    Exit Select


        ' ''                Case ClassActionList.CONNECT
        ' ''                    If Not InitCommAttribute Then
        ' ''                        myGlobal = Me.ManageAnalyzer(ClassActionList.INIT_COMMS)
        ' ''                    End If

        ' ''                    If InitCommAttribute Then
        ' ''                        'Inform the port and bauds to the AppLayer and connect
        ' ''                        AppLayer.ConnectedPortName = PortNameAttribute
        ' ''                        AppLayer.ConnectedBauds = BaudsAttribute

        ' ''                        ConnectedAttribute = False
        ' ''                        myGlobal = AppLayer.ActivateProtocol(ApplicationLayer.ClassEventList.SHORTINSTRUCTION, ApplicationLayer.ClassEventList.CONNECT)
        ' ''                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        ' ''                            ConnectedAttribute = DirectCast(myGlobal.SetDatos, Boolean)
        ' ''                        Else
        ' ''                            ConnectedAttribute = False
        ' ''                        End If
        ' ''                    End If
        ' ''                    Exit Select


        ' ''                    'AG 03/05/2010 - Short instructions (generical)
        ' ''                    'Code version 0
        ' ''                    'In future create an own case for each element in the case list
        ' ''                Case ClassActionList.SLEEP, ClassActionList.STANDBY, ClassActionList.WUP, ClassActionList.RUN, _
        ' ''                ClassActionList.SAMPLSTOP, ClassActionList.ENDRUN, ClassActionList.NROTOR, ClassActionList.ABORT, _
        ' ''                ClassActionList.SOUND, ClassActionList.ENDSOUND, ClassActionList.RESRECOVER, ClassActionList.ASKSTATUS

        ' ''                    myGlobal = AppLayer.ActivateProtocol(ApplicationLayer.ClassEventList.SHORTINSTRUCTION, pAction)
        ' ''                    Exit Select

        ' ''                Case ClassActionList.NEW_PREPARATION
        ' ''                    If Not ConnectedAttribute Then
        ' ''                        myGlobal = Me.ManageAnalyzer(ClassActionList.CONNECT)
        ' ''                    End If

        ' ''                    If ConnectedAttribute Then
        ' ''                        myGlobal = Me.SendNextPreparation
        ' ''                    End If
        ' ''                    Exit Select


        ' ''                Case ClassActionList.PREPARATION_ACCEPTED
        ' ''                    myGlobal = Me.MarkPreparationAccepted
        ' ''                    Exit Select


        ' ''                Case ClassActionList.STATE_RECEIVED
        ' ''                    myGlobal = Me.ProcessState(pInstructionReceived)
        ' ''                    Exit Select


        ' ''                Case ClassActionList.WAITING_TIME_EXPIRED
        ' ''                    'TODO: Ask for new state instruction
        ' ''                    Exit Select


        ' ''                Case ClassActionList.RESULTS_RECEIVED
        ' ''                    myGlobal = Me.ProcessResultsReception(pInstructionReceived)
        ' ''                    Exit Select


        ' ''                Case Else
        ' ''                    myGlobal.HasError = True
        ' ''                    myGlobal.ErrorCode = "NOT_CASE_FOUND"
        ' ''                    Exit Select
        ' ''            End Select

        ' ''        Else
        ' ''            myGlobal.HasError = True
        ' ''            myGlobal.ErrorCode = "NO_THREADS"
        ' ''        End If


        ' ''    Catch ex As Exception
        ' ''        myGlobal.HasError = True
        ' ''        myGlobal.ErrorCode = "SYSTEM_ERROR"
        ' ''        myGlobal.ErrorMessage = ex.Message

        ' ''        Dim myLogAcciones As New ApplicationLogManager()
        ' ''        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Error, False)
        ' ''    End Try
        ' ''    Return myGlobal
        ' ''End Function

        '''' 18/02/2011 - The old method open a transaction for: decode readings, call calculations, call export on line, manage auto repetitions and finally commint transaction
        ''''            If some error appears the readings are lost!!!
        '''' <summary>
        '''' Readings reception process (save readings, update executions tables and trigger calculations when needed)
        '''' </summary>
        '''' <param name="pInstructionReceived"></param>
        '''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        '''' <remarks>
        '''' Defined by:  AG - 26/04/2010
        '''' Created by: GDS - 27/04/2010
        '''' Modified by AG 12/05/2010 - Call the repetitions and autoexport when the ordertestid is closed
        '''' Modified by AG 08/06/2010 - Validate execution readings before call calculations methods (not SATURATED READINGS neiter READING_ERROR values exits)
        '''' AG 26/11/2010 - prepare the UI_Refresh event
        '''' </remarks>
        'Private Function ProcessReadingsReceivedOLD(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        myGlobal = DAOBase.GetOpenDBTransaction(Nothing)

        '        If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                'Delegates needed
        '                Dim myExecution As New ExecutionsDelegate
        '                Dim myOrderTest As New OrderTestsDelegate
        '                Dim myTest As New TestsDelegate
        '                Dim myReading As New WSReadingsDelegate
        '                Dim myCalc As New CalculationsDelegate
        '                Dim resLinq As New List(Of InstructionParameterTO)
        '                Dim totalReadingsNumber As Integer = 0
        '                Dim myPreparationID As Integer = 0
        '                Dim myTestReadingNumber As Integer = 0
        '                Dim myReadingNumberReceived As Integer = 0
        '                Dim myTestID As Integer = 0
        '                Dim myExecutionID As Integer = 0
        '                Dim myAnalyzerID As String = ""
        '                Dim myWorkSessionID As String = ""
        '                Dim myCurrentMultiItem As Integer = 0
        '                Dim myCurrentRerun As Integer = 0
        '                Dim myWellUsed As Integer = 0
        '                Dim myUtility As New Utilities()
        '                '1st read the total readings in instruction
        '                'Read the total readings number (parameter index 3)
        '                totalReadingsNumber = CInt((From a In pInstructionReceived Where a.ParameterIndex = 3 Select a.ParameterValue).First)

        '                Dim myOffset As Integer = 5 'Instruction offset

        '                'Treat all reads received!!!
        '                For i As Integer = 1 To totalReadingsNumber
        '                    Dim iteration As Integer = i

        '                    'myPreparationID = CInt((From a In pInstructionReceived Where a.ParameterIndex = (4 + (iteration - 1) * myOffset) Select a.ParameterValue).First)
        '                    'Read the preparation ID (parameter index = 4 + 5*i)
        '                    'TR 25/05/2010 implement the method to get a value through the index.
        '                    myGlobal = myUtility.GetItemByParameterIndex(pInstructionReceived, (4 + (iteration - 1) * myOffset))
        '                    If Not myGlobal.HasError Then
        '                        myPreparationID = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
        '                    Else : Exit For
        '                    End If
        '                    'TR 25/05/2010

        '                    'Read data from twksWSExecutions related with myPreparationID
        '                    myGlobal = myExecution.GetExecutionByPreparationID(dbConnection, myPreparationID)
        '                    If myGlobal.HasError Then Exit For

        '                    Dim myExecutionDS As New ExecutionsDS
        '                    myExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

        '                    myExecutionID = myExecutionDS.twksWSExecutions(0).ExecutionID
        '                    myAnalyzerID = myExecutionDS.twksWSExecutions(0).AnalyzerID
        '                    myWorkSessionID = myExecutionDS.twksWSExecutions(0).WorkSessionID
        '                    myCurrentMultiItem = myExecutionDS.twksWSExecutions(0).MultiItemNumber
        '                    myCurrentRerun = myExecutionDS.twksWSExecutions(0).RerunNumber

        '                    'Get the testID from the OrderTestID
        '                    Dim otDelegate As New OrderTestsDelegate
        '                    myGlobal = otDelegate.GetOrderTest(dbConnection, myExecutionDS.twksWSExecutions(0).OrderTestID)
        '                    If myGlobal.HasError Then Exit For
        '                    Dim myOrderTestDS As New OrderTestsDS
        '                    myOrderTestDS = DirectCast(myGlobal.SetDatos, OrderTestsDS)
        '                    myTestID = myOrderTestDS.twksOrderTests(0).TestID


        '                    'Only one time: Read the fields WellUsed, BaseLineID and update myExectuionDS current row
        '                    If i = 1 Then

        '                        'myWellUsed = CInt((From a In pInstructionReceived Where a.ParameterIndex = (8 + (iteration - 1) * myOffset) Select a.ParameterValue).First)
        '                        'Read the well used (parameter index 8) & inform it and the base line too
        '                        'TR 25/05/2010 implement the method to get a value through the index.
        '                        myGlobal = myUtility.GetItemByParameterIndex(pInstructionReceived, (8 + (iteration - 1) * myOffset))
        '                        If Not myGlobal.HasError Then
        '                            myWellUsed = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
        '                        Else : Exit For
        '                        End If
        '                        'TR 25/05/2010 END 

        '                        Dim localBaseLineID As Integer = 0
        '                        'BaseLineID for the base lines without adjust (twksWSBLinesByWell)
        '                        myGlobal = Me.GetCurrentBaseLineID(dbConnection, myAnalyzerID, myWorkSessionID, myWellUsed, False) 'AG 03/01/2011 - add FALSE parameter
        '                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '                            localBaseLineID = DirectCast(myGlobal.SetDatos, Integer)
        '                        End If

        '                        'AG 03/01/2011
        '                        Dim myAdjustBaseLineID As Integer = 0
        '                        'BaseLineID for the base lines without adjust (twksWSBLines)
        '                        myGlobal = Me.GetCurrentBaseLineID(dbConnection, myAnalyzerID, myWorkSessionID, myWellUsed, True) 'AG 03/01/2011 - add TRUE parameter
        '                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '                            myAdjustBaseLineID = DirectCast(myGlobal.SetDatos, Integer)
        '                        End If
        '                        'AG 03/01/2011

        '                        'Edit dataset
        '                        myExecutionDS.twksWSExecutions(0).BeginEdit()
        '                        myExecutionDS.twksWSExecutions(0).WellUsed = myWellUsed
        '                        myExecutionDS.twksWSExecutions(0).BaseLineID = localBaseLineID
        '                        myExecutionDS.twksWSExecutions(0).AdjustBaseLineID = myAdjustBaseLineID
        '                        myExecutionDS.twksWSExecutions(0).EndEdit()
        '                        myExecutionDS.AcceptChanges()
        '                        'END AG 17/05/2010

        '                        'Update fields into twksWSExecutions table (wellused and baselineID)
        '                        myGlobal = myExecution.UpdateReadingsFields(dbConnection, myExecutionDS)
        '                        If myGlobal.HasError Then Exit For
        '                    End If

        '                    'Save readings
        '                    Dim myReadingDS As New twksWSReadingsDS
        '                    Dim myReadingRow As twksWSReadingsDS.twksWSReadingsRow

        '                    myReadingRow = myReadingDS.twksWSReadings.NewtwksWSReadingsRow
        '                    myReadingRow.AnalyzerID = myExecutionDS.twksWSExecutions(0).AnalyzerID
        '                    myReadingRow.WorkSessionID = myExecutionDS.twksWSExecutions(0).WorkSessionID
        '                    myReadingRow.ExecutionID = myExecutionDS.twksWSExecutions(0).ExecutionID
        '                    myReadingRow.ReactionComplete = False

        '                    'Read Reading number, main counts and reference counts
        '                    'AG 19/05/2010 - IF Reading number > than READING1.CYCLES.min --- reactioncomplete = TRUE
        '                    'myReadingRow.ReadingNumber = CInt((From a In pInstructionReceived Where a.ParameterIndex = (5 + (iteration - 1) * myOffset) Select a.ParameterValue).First)
        '                    'TR 25/05/2010 implement the method to get a value through the index.
        '                    myGlobal = myUtility.GetItemByParameterIndex(pInstructionReceived, (5 + (iteration - 1) * myOffset))
        '                    If Not myGlobal.HasError Then
        '                        myReadingRow.ReadingNumber = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
        '                    Else : Exit For
        '                    End If

        '                    Dim myLimitsDlg As New FieldLimitsDelegate
        '                    myGlobal = myLimitsDlg.GetList(dbConnection, GlobalEnumerates.FieldLimitsEnum.READING1_CYCLES)
        '                    If myGlobal.HasError Then Exit For
        '                    Dim minReading1Cycles As Integer = 0
        '                    Dim fieldsDS As New FieldLimitsDS
        '                    fieldsDS = DirectCast(myGlobal.SetDatos, FieldLimitsDS)
        '                    minReading1Cycles = CInt(fieldsDS.tfmwFieldLimits(0).MinValue)
        '                    If myReadingRow.ReadingNumber > minReading1Cycles Then myReadingRow.ReactionComplete = True

        '                    'Counts
        '                    'TR 25/05/2010 implement the method to get a value through the index.
        '                    'myReadingRow.MainCounts = CInt((From a In pInstructionReceived Where a.ParameterIndex = (6 + (iteration - 1) * myOffset) Select a.ParameterValue).First)
        '                    myGlobal = myUtility.GetItemByParameterIndex(pInstructionReceived, (6 + (iteration - 1) * myOffset))
        '                    If Not myGlobal.HasError Then
        '                        myReadingRow.MainCounts = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
        '                    Else : Exit For
        '                    End If

        '                    'myReadingRow.RefCounts = CInt((From a In pInstructionReceived Where a.ParameterIndex = (7 + (iteration - 1) * myOffset) Select a.ParameterValue).First)
        '                    myGlobal = myUtility.GetItemByParameterIndex(pInstructionReceived, (7 + (iteration - 1) * myOffset))
        '                    If Not myGlobal.HasError Then
        '                        myReadingRow.RefCounts = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
        '                    Else : Exit For
        '                    End If
        '                    'TR 25/05/2010 END 

        '                    myReadingRow.DateTime = Now
        '                    myReadingDS.twksWSReadings.Rows.Add(myReadingRow)
        '                    myGlobal = myReading.SaveReadings(dbConnection, myReadingDS)
        '                    If myGlobal.HasError Then Exit For

        '                    'AG 09/02/2011 - UI RefreshDS - Readings received
        '                    If Not myGlobal.HasError Then
        '                        myGlobal = Me.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.NEW_READINGS_RECEIVED, myReadingRow.ExecutionID, myReadingRow.ReadingNumber)
        '                    End If
        '                    'AG 09/02/2011 

        '                    'Call CALCULATIONS, REPETITIONS AND LIS EXPORT only when needed!!!
        '                    myGlobal = myTest.GetTestReadingNumber(dbConnection, myTestID)
        '                    If myGlobal.HasError Then Exit For

        '                    'Call the calculations process if reading number = Test readings and current item = MAX(ordertest(multiitem))
        '                    myTestReadingNumber = CType(myGlobal.SetDatos, Integer)
        '                    myReadingNumberReceived = myReadingRow.ReadingNumber

        '                    ' dl 10/05/2010
        '                    myGlobal = myExecution.GetNumberOfMultititem(dbConnection, myExecutionDS.twksWSExecutions(0).ExecutionID)
        '                    myExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

        '                    If myTestReadingNumber = myReadingNumberReceived And _
        '                       myCurrentMultiItem = myExecutionDS.twksWSExecutions(0).MultiItemNumber Then

        '                        'AG 08/06/2010 - Validate execution readings before call calculations methods
        '                        Dim validReadings As Boolean = False
        '                        myGlobal = myReading.ValidateByExecutionID(dbConnection, myAnalyzerID, myWorkSessionID, myExecutionID)
        '                        validReadings = DirectCast(myGlobal.SetDatos, Boolean)
        '                        If validReadings Then
        '                            'AG 08/06/2010

        '                            myGlobal = myCalc.CalculateExecution(dbConnection, myExecutionID, myAnalyzerID, myWorkSessionID, False, "")   'Not recalculus!!

        '                            'AG 26/11/2010
        '                            If Not myGlobal.HasError Then
        '                                myGlobal = Me.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.NEW_RESULT_CALCULATED, myExecutionID, myReadingNumberReceived)
        '                            End If
        '                            'END AG 26/11/2010

        '                            'dl 10/05/2010 - Call repetitions and Lis export if we have receive the last replicate of the ordertest
        '                            Dim myOrderTestsDelegate As New OrderTestsDelegate
        '                            Dim myOrderTestsDS As New OrderTestsDS

        '                            myGlobal = myOrderTestsDelegate.GetOrderTest(dbConnection, myExecutionDS.twksWSExecutions(0).OrderTestID)
        '                            myOrderTestsDS = CType(myGlobal.SetDatos, OrderTestsDS)

        '                            If myOrderTestsDS.twksOrderTests(0).OrderTestStatus = "CLOSED" Then 'AG 12/05/2010
        '                                Dim myRepetitionsDelegate As New RepetitionsDelegate

        '                                myGlobal = myRepetitionsDelegate.ManageRepetitions(dbConnection, _
        '                                                                        myAnalyzerID, _
        '                                                                        myWorkSessionID, _
        '                                                                        myOrderTestsDS.twksOrderTests(0).OrderTestID, myCurrentRerun, False)
        '                                If myGlobal.HasError Then Exit For

        '                                'call the online exportation method
        '                                Dim myExport As New ExportDelegate
        '                                myGlobal = myExport.ManageLISExportation(dbConnection, myAnalyzerID, myWorkSessionID, myExecutionDS.twksWSExecutions(0).OrderTestID, False)
        '                                If myGlobal.HasError Then Exit For

        '                            End If 'If myOrderTestsDS.twksOrderTests(0).OrderTestStatus = "CLOSED"
        '                        End If 'If validReadings
        '                    End If 'If myTestReadingNumber = myReadingNumberReceived And myCurrentMultiItem = myExecutionDS.twksWSExecutions(0).MultiItemNumber
        '                Next

        '                If (Not myGlobal.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SERVICE - Processes the received Adjustments data
        '''' </summary>
        '''' <param name="pInstructionReceived"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Private Function ProcessAbsorbanceScanReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        '!!!!!!!!!PENDING TO SPEC
        '        Dim myUtilities As New Utilities
        '        Dim myInstParamTO As New InstructionParameterTO
        '        'Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION


        '        'myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
        '        'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '        '    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        'Else
        '        '    Exit Try
        '        'End If

        '        'If IsNumeric(myInstParamTO.ParameterValue) Then
        '        '    myActionValue = DirectCast(CInt(myInstParamTO.ParameterValue), GlobalEnumerates.AnalyzerManagerAx00Actions)
        '        '    AnalyzerCurrentActionAttribute = myActionValue
        '        'End If

        '        Dim myExpectedTime As Integer = WAITING_TIME_OFF 'If no request .... get the estimated time and activate the waiting timer
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If


        '        If IsNumeric(myInstParamTO.ParameterValue) Then
        '            myExpectedTime = CInt(myInstParamTO.ParameterValue)
        '            'AnalyzerIsReady = (myExpectedTimeValue = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
        '        Else
        '            Exit Try
        '        End If

        '        Dim myValues As String

        '        'values
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '            myValues = myInstParamTO.ParameterValue.Trim

        '            myGlobal = LoadAbsorbanceScanData(myValues)
        '        Else
        '            Exit Try
        '        End If

        '        ' XBC cal ?
        '        'myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END
        '        'If AnalyzerIsReadyAttribute = True Then
        '        '    myGlobal = Me.ManageFwScriptAnswer()
        '        'End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwAdjustmentsReceived", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SERVICE - Processes the sensor data received
        '''' </summary>
        '''' <param name="pInstructionReceived"></param>
        '''' <returns></returns>
        '''' <remarks>SG 28/02/11</remarks>
        'Private Function ProcessSensorsDataReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        '!!!!!!!!!PENDING TO SPEC
        '        Dim myUtilities As New Utilities
        '        Dim myInstParamTO As New InstructionParameterTO
        '        Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION


        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If

        '        If IsNumeric(myInstParamTO.ParameterValue) Then
        '            myActionValue = DirectCast(CInt(myInstParamTO.ParameterValue), GlobalEnumerates.AnalyzerManagerAx00Actions)
        '            AnalyzerCurrentActionAttribute = myActionValue
        '        End If

        '        Dim myExpectedTime As Integer = WAITING_TIME_OFF 'If no request .... get the estimated time and activate the waiting timer
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If


        '        If IsNumeric(myInstParamTO.ParameterValue) Then
        '            myExpectedTime = CInt(myInstParamTO.ParameterValue)
        '            'AnalyzerIsReady = (myExpectedTimeValue = 0) 'T = 0 Ax00 is ready to work, else Ax00 is busy
        '        Else
        '            Exit Try
        '        End If

        '        Dim mySensors As String
        '        Dim myValues As String

        '        'sensors
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '            mySensors = myInstParamTO.ParameterValue.Trim
        '        Else
        '            Exit Try
        '        End If

        '        'values
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '            myValues = myInstParamTO.ParameterValue.Trim
        '        Else
        '            Exit Try
        '        End If

        '        'update sensors data
        '        myGlobal = MyClass.UpdateSensorsData(mySensors, myValues)

        '        myActionValue = GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END
        '        If AnalyzerIsReadyAttribute = True Then
        '            myGlobal = Me.ManageFwScriptAnswer(myActionValue)
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessFwSensorsReceived", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SW has received and ANSINF instruction for real time monitoring
        '''' </summary>
        '''' <param name="pInstructionReceived"></param>
        '''' <returns></returns>
        '''' <remarks>SGM 13/04/2011 - creation</remarks>
        'Private Function ProcessSERVICEInformationStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim myUtilities As New Utilities
        '        Dim myInstParamTO As New InstructionParameterTO
        '        Dim mySensorsDS As New SRVSensorsDS

        '        'Get General cover (parameter index 3)
        '        Dim myIntValue As Integer = 0
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 3)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.MAIN_COVER.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()

        '        Else
        '            Exit Try
        '        End If

        '        'Get Photometrics (Reaccions) cover (parameter index 4)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 4)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.REACTIONS_COVER.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Reagents (Fridge) cover (parameter index 5)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 5)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If


        '        'Get Samples cover (parameter index 6)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 6)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.SAMPLES_COVER.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If

        '        'Get System liquid sensor (parameter index 7)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 7)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.DISTILLED_WATER_EMPTY.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()

        '        Else
        '            Exit Try
        '        End If


        '        'Get Waste sensor (parameter index 8)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 8)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.LOW_CONTAMINATION_FULL.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If

        '        'Get Weight sensors (wash solution ) (parameter index 9)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 9)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.WASHING_SOLUTION_LEVEL.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If

        '        'Get Weight sensors ('high contamination waste ) (parameter index 9)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 10)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.HIGH_CONTAMINATION_LEVEL.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Temperature values (reactions) (parameter index 11

        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 11)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.REACTIONS_ROTOR_TEMP.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Temperature values ( fridge) (parameter index  13)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 13)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.FRIDGE_TEMP.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Temperature values ( wash station heater) (parameter index  14)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 14)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.WASHING_STATION_HEATER_TEMP.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Temperature values ( R1) (parameter index  15)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 15)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.REAGENTS_ROTOR1_TEMP.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If


        '        'Get Temperature values ( R2) (parameter index  16)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 16)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

        '            Dim mySensorRow As SRVSensorsDS.srv_tfmwSensorsRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '            With mySensorRow
        '                .SensorId = GlobalEnumerates.SENSOR.REAGENTS_ROTOR2_TEMP.ToString
        '                .Value = CDbl(myInstParamTO.ParameterValue)
        '                .HasError = False 'PDT
        '                .GroupId = ""
        '                .CodeFw = ""
        '                .Type = ""
        '            End With

        '            mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(mySensorRow)
        '            mySensorsDS.AcceptChanges()
        '        Else
        '            Exit Try
        '        End If




        '        'Get Fridge status (parameter index 12)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 12)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If



        '        'Get ISE status (parameter index 17)
        '        myGlobal = myUtilities.GetItemByParameterIndex(pInstructionReceived, 17)
        '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
        '        Else
        '            Exit Try
        '        End If


        '        ''''''''
        '        myGlobal = AppLayer.UpdateSensorsDS(mySensorsDS)

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessSERVICEInformationStatusReceived", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal
        'End Function


        'AG 02/03/2012 commented
        '''' <summary>
        '''' Transalte the Fw error code to an internal Sw alarm ID enumerate
        '''' Depending the error code the internal flag AnalyzerIsFREEZEMode is activated
        '''' </summary>
        '''' <param name="pErrorCode"></param>
        '''' <param name="pAlarmStatus" ></param>
        '''' <returns>The alarmID related to the error code. Also returns the byRef parameter with the alarm status</returns>
        '''' <remarks>
        '''' AG 25/03/2011 - creation ... but not implemented due the error codes are not still defined
        '''' </remarks>
        'Private Function TranslateErrorCodeToAlarmID(ByVal pErrorCode As Integer, ByRef pAlarmStatus As Boolean) As GlobalEnumerates.Alarms
        '    Dim myReturnValue As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
        '    'NOTE: Do not use Try Catch do the caller method implements it

        '    Dim isFreezeMode As Boolean = False 'Must be set to TRUE for several error codes. Default value FALSE
        '    pAlarmStatus = True 'Indicates if the alarm exists or is solved. Default value TRUE (alarm exists)

        '    Select Case pErrorCode
        '        'We have to translate the Fw error code into the Sw AlarmID enum code 
        '        'and assign his proper status True when the alarm exists or False if the alarm is solved

        '        'Based on \\Servidor\Documents\Instruments\RDI\Gestio Projectes\AX00\02 REQUISITS\03 FIRMWARE\Instrucciones AX00 (rev40).xls (01/02/2012)
        '        'AG 17/02/2012 - This error code is not defined
        '        ''XBC 04/11/2011
        '        'Case 44
        '        '    myReturnValue = GlobalEnumerates.Alarms.ADJUST_NO_EXIST

        '        Case 60
        '            'AG 23/11/2011 - this alarm is only generated when Instrument has ISE module installed - adjust ISEINS = 1
        '            'myReturnValue = GlobalEnumerates.Alarms.ISE_STATUS_ERR
        '            Dim ISEInstalledValue As String = ""
        '            ISEInstalledValue = ReadAdjustValue(Ax00Adjustsments.ISEINS)
        '            If ISEInstalledValue <> "" AndAlso IsNumeric(ISEInstalledValue) Then
        '                If CType(ISEInstalledValue, Boolean) Then
        '                    myReturnValue = GlobalEnumerates.Alarms.ISE_STATUS_ERR
        '                End If
        '            End If
        '            'AG 23/11/2011

        '        Case 70
        '            myReturnValue = GlobalEnumerates.Alarms.MAIN_COVER_WARN
        '            isFreezeMode = True

        '        Case 100
        '            myReturnValue = GlobalEnumerates.Alarms.R1_V_HOME_ERR
        '            isFreezeMode = True

        '        Case 101
        '            myReturnValue = GlobalEnumerates.Alarms.R1_H_HOME_ERR
        '            isFreezeMode = True

        '        Case 110
        '            myReturnValue = GlobalEnumerates.Alarms.R2_V_HOME_ERR
        '            isFreezeMode = True

        '        Case 111
        '            myReturnValue = GlobalEnumerates.Alarms.R2_H_HOME_ERR
        '            isFreezeMode = True

        '        Case 120
        '            myReturnValue = GlobalEnumerates.Alarms.S_V_HOME_ERR
        '            isFreezeMode = True

        '        Case 121
        '            myReturnValue = GlobalEnumerates.Alarms.S_H_HOME_ERR
        '            isFreezeMode = True

        '        Case 130
        '            myReturnValue = GlobalEnumerates.Alarms.STIRRER1_V_HOME_ERR
        '            isFreezeMode = True

        '        Case 131
        '            myReturnValue = GlobalEnumerates.Alarms.STIRRER1_H_HOME_ERR
        '            isFreezeMode = True

        '        Case 140
        '            myReturnValue = GlobalEnumerates.Alarms.STIRRER2_V_HOME_ERR
        '            isFreezeMode = True

        '        Case 141
        '            myReturnValue = GlobalEnumerates.Alarms.STIRRER2_H_HOME_ERR
        '            isFreezeMode = True

        '        Case 200
        '            myReturnValue = GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR
        '            isFreezeMode = True

        '        Case 201, 202
        '            myReturnValue = GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR

        '        Case 203
        '            myReturnValue = GlobalEnumerates.Alarms.R1_COLLISION_WARN

        '        Case 210
        '            myReturnValue = GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR
        '            isFreezeMode = True

        '        Case 211, 212
        '            myReturnValue = GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR

        '        Case 213
        '            myReturnValue = GlobalEnumerates.Alarms.R2_COLLISION_WARN

        '        Case 220
        '            myReturnValue = GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR
        '            isFreezeMode = True

        '        Case 223
        '            myReturnValue = GlobalEnumerates.Alarms.S_COLLISION_WARN

        '        Case 300
        '            myReturnValue = GlobalEnumerates.Alarms.FRIDGE_HOME_ERR
        '            isFreezeMode = True

        '        Case 301, 302
        '            myReturnValue = GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR

        '        Case 303
        '            myReturnValue = GlobalEnumerates.Alarms.FRIDGE_FAN_WARN

        '        Case 304
        '        Case 305
        '            myReturnValue = GlobalEnumerates.Alarms.FRIDGE_COVER_WARN
        '            isFreezeMode = True

        '        Case 350
        '            myReturnValue = GlobalEnumerates.Alarms.SAMPLES_HOME_ERR
        '            isFreezeMode = True

        '        Case 354
        '        Case 355
        '            myReturnValue = GlobalEnumerates.Alarms.S_COVER_WARN
        '            isFreezeMode = True

        '        Case 500
        '            myReturnValue = GlobalEnumerates.Alarms.REACTIONS_HOME_ERR
        '            isFreezeMode = True

        '        Case 501
        '            myReturnValue = GlobalEnumerates.Alarms.REACT_ENCODER_ERR
        '            isFreezeMode = True

        '        Case 502
        '            'myReturnValue = GlobalEnumerates.Alarms.WS_COLLISION_ERR
        '            'isFreezeMode = True

        '        Case 503
        '            myReturnValue = GlobalEnumerates.Alarms.REACT_COVER_WARN
        '            isFreezeMode = True

        '        Case 504
        '            myReturnValue = GlobalEnumerates.Alarms.WS_COLLISION_ERR
        '            isFreezeMode = True

        '        Case 510, 511
        '            myReturnValue = GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR

        '        Case 512
        '            myReturnValue = GlobalEnumerates.Alarms.REACT_ROTOR_FAN_WARN

        '        Case 520
        '            myReturnValue = GlobalEnumerates.Alarms.WS_HOME_ERR
        '            isFreezeMode = True

        '        Case 530

        '        Case 600
        '            myReturnValue = GlobalEnumerates.Alarms.WS_PUMP_HOME_ERR
        '            isFreezeMode = True

        '        Case 610, 611
        '            myReturnValue = GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR

        '        Case 620, 621
        '            myReturnValue = GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR

        '        Case 630, 631
        '            myReturnValue = GlobalEnumerates.Alarms.WATER_SYSTEM_ERR

        '        Case 640
        '        Case 650
        '        Case 660
        '        Case 661
        '        Case 662
        '        Case 663
        '        Case 664
        '        Case 665
        '        Case 666
        '        Case 667
        '        Case 668
        '        Case 669
        '        Case 670
        '        Case 671
        '        Case 672
        '        Case 673

        '        Case 700
        '            myReturnValue = GlobalEnumerates.Alarms.S_PUMP_HOME_ERR
        '            isFreezeMode = True

        '        Case 701
        '            myReturnValue = GlobalEnumerates.Alarms.R1_PUMP_HOME_ERR
        '            isFreezeMode = True

        '        Case 702
        '            myReturnValue = GlobalEnumerates.Alarms.R2_PUMP_HOME_ERR
        '            isFreezeMode = True

        '        Case 710
        '        Case 720
        '        Case 721
        '        Case 722
        '        Case 723
        '        Case 724
        '        Case 725
        '        Case 726
        '        Case 727
        '        Case 728
        '        Case 729
        '        Case 730
        '        Case 731


        '        Case Else
        '            myReturnValue = GlobalEnumerates.Alarms.NONE
        '            pAlarmStatus = False
        '            isFreezeMode = False

        '    End Select

        '    If Not myAnalyzerIsFREEZEFlagAttribute And isFreezeMode Then
        '        myAnalyzerIsFREEZEFlagAttribute = isFreezeMode
        '    End If

        '    Return (myReturnValue)

        'End Function

#End Region

#Region "Temporal testing IdBuilder Class definitions"

        'Class IdBuilder
        '    Private Shared Id As Integer = 0

        '    Public Shared Function NextId() As Integer
        '        Id += 1
        '        Return Id
        '    End Function

        'End Class

#End Region

    End Class

End Namespace
