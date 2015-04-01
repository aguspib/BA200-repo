Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates

Namespace Biosystems.Ax00.Core.Entities
    Public Class Preparation

        Private _resultData As GlobalDataTO = Nothing
        Private _dbConnection As SqlConnection = Nothing
        Private _startTime As DateTime = Now
        Private _nextPreparationDs As New AnalyzerManagerDS
        Private _rejectedWell As Boolean = False
        Private _contaminatedWell As Boolean = False
        Private _washSol1 As String = ""
        Private _washSol2 As String = ""
        Private _nextRow As AnalyzerManagerDS.nextPreparationRow
        Private _executionFound As Integer = GlobalConstants.NO_PENDING_PREPARATION_FOUND

        Private ReadOnly _analyzerManager As IAnalyzerManager
        Private ReadOnly _pDbConnection As SqlConnection
        Private ReadOnly _startTimeTotal As DateTime = Now

        Public Sub New(ByVal analyzerManager As IAnalyzerManager, ByVal sqlConnection As SqlConnection)
            _analyzerManager = analyzerManager
            _pDbConnection = sqlConnection
        End Sub

        ''' <summary>
        ''' Search next pending preparation to send or wash contamination
        ''' </summary>
        ''' <param name="pNextWell"></param>
        ''' <param name="pLookForIseExecutionsFlag"></param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not
        ''' If no error: Return an AnalyzerManagerDS.nextPreparation (the ExecutionID (integer) to send. If no items found return NO_ITEMS_FOUND (-1)</returns>
        ''' <remarks>
        ''' AG 17/01/2011 - creation
        ''' AG 24/11/2011 - Change priority 1st cuvette washing, 2on optically rejected (skip)
        ''' AG 25/01/2012 - When predilution is found before send the preparation we have to evaluate cuvette washing or skip in well pNextWell + 4 (Sergi 24/01/2012)
        ''' RH 26/06/2012 - Take CONTROLs ISE executions into account
        ''' AG 12/07/2012 - add parameter (optional pLookForISEExecutionsFlag). This parameter is informed only when call this method from ManageSendAndSearchNext
        '''                 and the ise requests is FALSE (I:0)
        ''' 
        ''' REFACTOR: SearchNextPreparation
        ''' </remarks>
        Public Function SearchNext(ByVal pNextWell As Integer, Optional ByVal pLookForIseExecutionsFlag As Boolean = True) As GlobalDataTO
            Try
                _resultData = GetOpenDBConnection(_pDbConnection)

                If (Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing) Then

                    _dbConnection = DirectCast(_resultData.SetDatos, SqlConnection)

                    If (Not _dbConnection Is Nothing) Then

                        If Not CheckCuvetteIsContaminatedOrOpticallyRejected(pNextWell) Or _
                           Not CheckIsIsePreparation(pLookForISEExecutionsFlag) Then
                            Return _resultData
                        End If

                        CheckNextIsStdPreparationOrContaminationWash()

                        _nextPreparationDs.AcceptChanges()
                        _resultData.SetDatos = _nextPreparationDs

                    End If
                End If

                GlobalBase.CreateLogActivity("SearchNextPreparation (Complete): " & Now.Subtract(_startTimeTotal).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)

            Catch ex As Exception
                _resultData = New GlobalDataTO()
                _resultData.HasError = True
                _resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                _resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Error, False)
            End Try

            If (_pDbConnection Is Nothing) AndAlso (Not _dbConnection Is Nothing) Then _dbConnection.Close()

            Return _resultData

        End Function

#Region "SendNextPreparation"
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
        ''' 
        ''' REFACTOR: SendNextPreparation
        ''' </remarks>
        Public Function SendNext(ByVal pNextWell As Integer, ByRef pActionSentFlag As Boolean, ByRef pEndRunSentFlag As Boolean, ByRef pSystemErrorFlag As Boolean) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim startTime As DateTime

                Dim iseStatusOk As Boolean = False
                Dim iseInstalledFlag As Boolean = False
                Dim adjustValue = _analyzerManager.ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
                If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                    iseInstalledFlag = CType(adjustValue, Boolean)
                End If

                If iseInstalledFlag Then
                    If _analyzerManager.Alarms.Contains(Alarms.ISE_FAILED_ERR) OrElse _analyzerManager.Alarms.Contains(Alarms.ISE_OFF_ERR) Then
                        iseStatusOk = False
                    Else
                        iseStatusOk = True
                    End If
                End If

                'AG 07/06/2012 - This method only sends, not searchs
                'Search for the next preparation to send
                Dim actionAlreadySent As Boolean = False
                Dim endRunToSend As Boolean = False
                Dim systemErrorFlag As Boolean = False 'AG 25/01/2012 Indicates if search next has produced a system error. In this case send a END instruction
                Dim emptyFieldsDetected As Boolean = False 'AG 03/06/2014 - #1519 when Sw cannot send the proper instruction because there are emtpy fields error then send SKIP

                If Not myGlobal.HasError Then '(1)

                    Dim myAnManagerDs = _analyzerManager.NextPreparationsAnalyzerManagerDS

                    If myAnManagerDs.nextPreparation.Rows.Count > 0 Then '(2)
                        'Analyze the row0 found
                        '1st: Check if next cuvette is optically rejected ... send nothing (DUMMY)
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDs.nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull AndAlso myAnManagerDs.nextPreparation(0).CuvetteOpticallyRejectedFlag Then
                                'Send a SKIP instruction but mark as actionAlreadySend. So Ax00 will performe a Dummy cycle 
                                startTime = Now

                                myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SKIP)
                                actionAlreadySent = True

                                GlobalBase.CreateLogActivity("SKIP sent: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                            End If
                        End If

                        '2on: Check if next cuvette requires washing (cuvette contamination)
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDs.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso myAnManagerDs.nextPreparation(0).CuvetteContaminationFlag Then
                                startTime = Now 'AG 28/06/2012

                                myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDs)
                                actionAlreadySent = True
                                If Not myGlobal.HasError Then
                                    AddNewSentPreparationsList(Nothing, myAnManagerDs, pNextWell) 'Update sent instructions DS
                                    GlobalBase.CreateLogActivity("WRUN cuvette sent: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                Else
                                    'AG 27/09/2012 - the instruction could not be sent because empty fields. The cuvette must continue marked as Contamianted, so nothing to do
                                    ' except remove the error flag and code
                                    If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                        myGlobal.HasError = False
                                        myGlobal.ErrorCode = ""
                                        emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                    End If
                                End If
                            End If
                        End If

                        '3rd: Check if next preparation is an ISE preparation
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDs.nextPreparation(0).IsExecutionTypeNull AndAlso myAnManagerDs.nextPreparation(0).ExecutionType = "PREP_ISE" Then
                                'endRunToSend = True  'AG 30/11/2011 comment this line (Sw has to send ENDRUN)

                                If iseStatusOk Then 'Only if ISE modul is active 
                                    If Not myAnManagerDs.nextPreparation(0).IsExecutionIDNull AndAlso myAnManagerDs.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        If _analyzerManager.ISEAnalyzer IsNot Nothing Then
                                            _analyzerManager.ISEAnalyzer.CurrentProcedure = ISEManager.ISEProcedures.Test
                                        End If
                                        startTime = Now 'AG 28/06/2012

                                        myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SEND_ISE_PREPARATION, myAnManagerDs.nextPreparation(0).ExecutionID)
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
                                                myGlobal = _analyzerManager.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDs.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            End If
                                        End If

                                        GlobalBase.CreateLogActivity("ISETEST sent: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                    End If
                                End If

                            End If
                        End If

                        '4rh: Check if exists reagent contaminations
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDs.nextPreparation(0).IsReagentContaminationFlagNull AndAlso myAnManagerDs.nextPreparation(0).ReagentContaminationFlag Then
                                startTime = Now

                                myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDs)
                                actionAlreadySent = True

                                If Not myGlobal.HasError Then
                                    AddNewSentPreparationsList(Nothing, myAnManagerDs, pNextWell) 'Update sent instructions DS
                                    GlobalBase.CreateLogActivity("WRUN reagents sent: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                Else
                                    Dim sendWrunErrorCode As String = myGlobal.ErrorCode 'AG 27/09/2012

                                    'DL 06/07/2012 -In case of error the LOCK execution.
                                    'Lock the next execution to be sent
                                    Dim myExecutionDelegate As New ExecutionsDelegate

                                    'get the execution data 
                                    myGlobal = myExecutionDelegate.GetExecution(Nothing, myAnManagerDs.nextPreparation.First.ExecutionID)
                                    If Not myGlobal.HasError Then
                                        Dim myWsExecutionDs = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                        If myWsExecutionDs.twksWSExecutions.Rows.Count > 0 Then
                                            myWsExecutionDs.twksWSExecutions.First.BeginEdit()
                                            myWsExecutionDs.twksWSExecutions.First.ExecutionStatus = "LOCKED"
                                            myWsExecutionDs.twksWSExecutions.First.EndEdit()
                                            myGlobal = myExecutionDelegate.UpdateStatus(Nothing, myWsExecutionDs)

                                            ''make sure send error.
                                            'AG 27/09/2012 - If the instruction could not be sent because empty fields. Remove the error code, when all his treatment has done, else keep the error
                                            If sendWrunErrorCode = "EMPTY_FIELDS" Then
                                                myGlobal = _analyzerManager.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDs.nextPreparation.First.ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            Else
                                                myGlobal.HasError = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        '5rh: Check if next preparation is an STD preparation and executionID <> NO_PENDING_PREPARATION_FOUND
                        If Not actionAlreadySent And Not endRunToSend Then
                            If Not myAnManagerDs.nextPreparation(0).IsExecutionTypeNull AndAlso myAnManagerDs.nextPreparation(0).ExecutionType = "PREP_STD" Then

                                If Not myAnManagerDs.nextPreparation(0).IsExecutionIDNull AndAlso myAnManagerDs.nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                    startTime = Now 'AG 28/06/2012

                                    'AG 31/05/2012 - Evaluate the proper well for contamination or rejection for PTEST case
                                    Dim testPrepData As New PreparationTestDataDelegate
                                    Dim isPtesTinstructionFlag As Boolean = False
                                    myGlobal = testPrepData.isPTESTinstruction(Nothing, myAnManagerDs.nextPreparation(0).ExecutionID, isPtesTinstructionFlag)
                                    If Not isPtesTinstructionFlag Then 'TEST instruction
                                        myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDs.nextPreparation(0).ExecutionID)
                                        actionAlreadySent = True
                                        If Not myGlobal.HasError Then
                                            AddNewSentPreparationsList(Nothing, myAnManagerDs, pNextWell) 'Update sent instructions DS
                                        Else
                                            'AG 27/09/2012 - the instruction could not be sent because empty fields. Executions has been LOCKED in GeneratePreparation (test o ptest)
                                            ' method in Instructions class. Now we have to remove the error flag and code
                                            If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                                'Prepare UIRefreshDS - execution status changed
                                                myGlobal = _analyzerManager.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDs.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                myGlobal.HasError = False
                                                myGlobal.ErrorCode = ""
                                                emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                            End If
                                        End If


                                    Else 'PTEST instruction
                                        'Before send the PTEST instruction Sw has to evaluate the well (pNextWell + WELL_OFFSET_FOR_PREDILUTION)
                                        'looking for cuvette contamination or optical rejection
                                        Dim rejectedPtestWell As Boolean = False
                                        Dim contaminatePtesTdWell As Boolean = False
                                        Dim washPtestSol1 As String = ""
                                        Dim washPtestSol2 As String = ""

                                        'Next well where the PTEST instruction will be perform is (pNextwell + WELL_OFFSET_FOR_PREDILUTION) but from 1 to 120 ciclycal
                                        Dim reactRotorDlg As New ReactionsRotorDelegate
                                        Dim nextPtestWell = reactRotorDlg.GetRealWellNumber(pNextWell + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)

                                        myGlobal = CheckRejectedContaminatedNextWell(Nothing, nextPtestWell, rejectedPtestWell, contaminatePtesTdWell, washPtestSol1, washPtestSol2)
                                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                            If contaminatePtesTdWell OrElse rejectedPtestWell Then 'SKIP
                                                myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SKIP)
                                                actionAlreadySent = True

                                            Else 'PTEST 
                                                myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SEND_PREPARATION, myAnManagerDs.nextPreparation(0).ExecutionID)
                                                actionAlreadySent = True
                                                If Not myGlobal.HasError Then
                                                    AddNewSentPreparationsList(Nothing, myAnManagerDs, nextPtestWell) 'Update sent instructions DS
                                                Else
                                                    'AG 27/09/2012 - the instruction could not be sent because empty fields. Affected executions has been LOCKED in GeneratePreparation (test o ptest)
                                                    ' method in Instructions class. Now we have to remove the error flag and code
                                                    If myGlobal.ErrorCode = "EMPTY_FIELDS" Then
                                                        'Prepare UIRefreshDS - execution status changed
                                                        myGlobal = _analyzerManager.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, myAnManagerDs.nextPreparation(0).ExecutionID, 0, Nothing, False)

                                                        myGlobal.HasError = False
                                                        myGlobal.ErrorCode = ""
                                                        emptyFieldsDetected = True 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP
                                                    End If
                                                End If
                                            End If
                                        ElseIf myGlobal.HasError Then
                                            systemErrorFlag = True
                                        End If
                                    End If
                                    GlobalBase.CreateLogActivity("TEST or PTEST sent: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                                End If
                            End If
                        End If

                    Else

                    End If
                ElseIf myGlobal.HasError Then
                    systemErrorFlag = True
                End If

                'New Conditions 
                '- When EMPTY_FIELDS error detected --> Sw sends SKIP
                '- When NO ise installed: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
                '- When ise installed but ise switch off: No action sent + AnalyzerIsReady (R:1) --> Sw sends ENDRUN
                '- When ise installed and ise switch on: No action sent + AnalyzerIsReady (R:1) + ISEModuleIsReady (I:1) --> Sw sends ENDRUN
                '- Else if No action sent --> Sw sends SKIP
                If emptyFieldsDetected Then
                    myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SKIP) 'AG 03/06/2014 - #1519 if the proper instruction could not be sent because EMPTY_FIELDS error send a SKIP

                ElseIf Not actionAlreadySent Then
                    If Not systemErrorFlag Then
                        endRunToSend = False
                        If Not iseInstalledFlag AndAlso _analyzerManager.AnalyzerIsReady Then
                            endRunToSend = True
                        ElseIf iseInstalledFlag AndAlso Not iseStatusOk AndAlso _analyzerManager.AnalyzerIsReady Then
                            endRunToSend = True
                        ElseIf iseInstalledFlag AndAlso iseStatusOk Then
                            If _analyzerManager.AnalyzerIsReady AndAlso _analyzerManager.ISEModuleIsReady Then
                                endRunToSend = True
                            End If
                        End If

                    Else 'AG 25/01/2012 - systemErrorFlag: True
                        endRunToSend = True

                        'Send END and mark as it like if used had press paused. Else we will enter into a loop END <-> START <-> END <-> START ....
                        Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate

                        _analyzerManager.UpdateSessionFlags(myAnalyzerFlagsDs, GlobalEnumerates.AnalyzerManagerFlags.ENDprocess, "INPROCESS")
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                    End If

                    If endRunToSend Then 'ENDRUN - no more to be sent
                        myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.ENDRUN)
                        actionAlreadySent = True
                    Else 'SKIP - No more std tests but some ise test are pending to be sent but the ISE module is working...
                        myGlobal = _analyzerManager.ActivateProtocolWrapper(GlobalEnumerates.AppLayerEventList.SKIP)
                        actionAlreadySent = True
                    End If
                End If
                pActionSentFlag = actionAlreadySent
                pEndRunSentFlag = endRunToSend
                pSystemErrorFlag = systemErrorFlag

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SendNextPreparation", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function
#End Region

#Region "Aux Functions"

        ''' <summary>
        ''' Checks if next well is contaminated by last reagent used on it or if next well is optically rejected
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pNextWell"></param>
        ''' <param name="pRejectedWell " ></param>
        ''' <param name="pContaminatedWell"></param>
        ''' <param name="pWashSol1"></param>
        ''' <param name="pWashSol2"></param>
        ''' <returns>GolbalDataTo indicates if error or not. If no error use the byref parameters</returns>
        ''' <remarks>AG 18/0/2011
        ''' AG 24/11/2011 - Priority 1st Contaminated well, 2on Rejected Well + change the business to prepare the contaminated well washing</remarks>
        Private Function CheckRejectedContaminatedNextWell(ByVal pDbConnection As SqlConnection, ByVal pNextWell As Integer, _
                                                    ByRef pRejectedWell As Boolean, ByRef pContaminatedWell As Boolean, _
                                                    ByRef pWashSol1 As String, ByRef pWashSol2 As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        pContaminatedWell = False 'Initialize return byref variables
                        pWashSol1 = ""
                        pWashSol2 = ""

                        Dim myReactionsDelgte As New ReactionsRotorDelegate
                        'Read the well historical use in the current Worksession & Analyzer on parameter well (pNextWell)
                        resultData = myReactionsDelgte.ReadWellHistoricalUse(dbConnection, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer, pNextWell, True)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myReactionsDs = CType(resultData.SetDatos, ReactionsRotorDS)

                            If myReactionsDs.twksWSReactionsRotor.Rows.Count > 0 Then
                                'Look if next well is rejected or not
                                If Not myReactionsDs.twksWSReactionsRotor(0).IsRejectedFlagNull Then
                                    If String.Equals(myReactionsDs.twksWSReactionsRotor.First.WellContent, "E") And myReactionsDs.twksWSReactionsRotor(0).RejectedFlag Then
                                        pRejectedWell = True
                                    End If
                                End If

                                'Search if next well is reagent contaminated by his last execution on it                            
                                Dim rotorTurnWithWellContamination As Integer = 0
                                Dim listStDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDs.twksWSReactionsRotor _
                                               Where a.WellContent = "C" AndAlso a.WashRequiredFlag = True AndAlso a.WashedFlag = False _
                                               Select a Order By a.RotorTurn Descending)

                                If listStDtest.Count > 0 Then
                                    'Get the last cuvette contaminator washing information (WashingSolutionR1, WashingSolutionR2)
                                    pContaminatedWell = True
                                    rotorTurnWithWellContamination = listStDtest(0).RotorTurn
                                    If Not listStDtest(0).IsWashingSolutionR1Null Then pWashSol1 = listStDtest(0).WashingSolutionR1
                                    If Not listStDtest(0).IsWashingSolutionR2Null Then pWashSol2 = listStDtest(0).WashingSolutionR2
                                End If 'If listSTDtest.Count > 0 Then

                                'If well contaminated search if the previous well contamination has been washed or not
                                If pContaminatedWell Then
                                    listStDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDs.twksWSReactionsRotor _
                                                   Where a.WashRequiredFlag = True AndAlso a.WashedFlag = True _
                                                   Select a Order By a.RotorTurn Descending)
                                    If listStDtest.Count > 0 Then
                                        If listStDtest(0).RotorTurn > rotorTurnWithWellContamination Then pContaminatedWell = False
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

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.CheckRejectedContaminatedNextWell", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#Region "SendNextPreparation"
        ''' <summary>
        ''' Inform the instruction sent in Running (only STD test preparation, or WRUN (reagent contamination) or WRUN (cuvette contamination)
        ''' into the structure that contains the last instructions sent used for calculate the next to be send
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pNewPreparationSent"></param>
        ''' <param name="pNextWell"></param>
        ''' <returns></returns>
        ''' <remarks>Modified AG 10/01/2014</remarks>
        Private Function AddNewSentPreparationsList(ByVal pDbConnection As SqlConnection, ByVal pNewPreparationSent As AnalyzerManagerDS, ByVal pNextWell As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try

                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        If pNewPreparationSent.nextPreparation.Rows.Count > 0 Then '(3)
                            Dim added As Boolean = False
                            Dim nextRow As AnalyzerManagerDS.sentPreparationsRow

                            'CASE cuvette washing: Add new row with cuvette washing
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsCuvetteContaminationFlagNull Then '(4)
                                If pNewPreparationSent.nextPreparation(0).CuvetteContaminationFlag Then '(4.1)
                                    nextRow = _analyzerManager.SentPreparations.NewsentPreparationsRow
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

                                    _analyzerManager.SentPreparations.AddsentPreparationsRow(nextRow)
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                    added = True

                                End If '(4.1)
                            End If '(4)

                            'CASE reagent washing: Add new row with reagent washing
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsReagentContaminationFlagNull Then '(5)
                                If pNewPreparationSent.nextPreparation(0).ReagentContaminationFlag Then '(5.1)
                                    nextRow = _analyzerManager.SentPreparations.NewsentPreparationsRow

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

                                    _analyzerManager.SentPreparations.AddsentPreparationsRow(nextRow)
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                    added = True

                                End If '(5.1)
                            End If '(5)


                            'CASE STD preparation: Add new row with STD preparation
                            If Not added And Not pNewPreparationSent.nextPreparation(0).IsExecutionIDNull Then '(6)
                                Dim myExecution As Integer = pNewPreparationSent.nextPreparation(0).ExecutionID

                                'Get execution additional information
                                Dim execDelg As New ExecutionsDelegate
                                Dim myExecDs As New AnalyzerManagerDS

                                resultData = execDelg.GetSendPreparationDataByExecution(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myExecution)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myExecDs = CType(resultData.SetDatos, AnalyzerManagerDS)
                                Else
                                    Exit Try
                                End If

                                'Add row to mySentPreparationsDS
                                If myExecDs.searchNext.Rows.Count > 0 Then '(6.1)
                                    nextRow = _analyzerManager.SentPreparations.NewsentPreparationsRow
                                    nextRow.CuvetteWashFlag = False
                                    nextRow.ReagentWashFlag = False
                                    nextRow.ExecutionID = myExecDs.searchNext(0).ExecutionID
                                    nextRow.StatFlag = myExecDs.searchNext(0).StatFlag
                                    nextRow.SampleClass = myExecDs.searchNext(0).SampleClass
                                    nextRow.ExecutionType = myExecDs.searchNext(0).ExecutionType
                                    nextRow.ReagentID = myExecDs.searchNext(0).ReagentID
                                    nextRow.OrderTestID = myExecDs.searchNext(0).OrderTestID
                                    nextRow.OrderID = myExecDs.searchNext(0).OrderID
                                    nextRow.SampleType = myExecDs.searchNext(0).SampleType
                                    nextRow.WellUsed = pNextWell
                                    _analyzerManager.SentPreparations.AddsentPreparationsRow(nextRow)
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                    added = True
                                End If '(6.1)

                                'Finally delete the PREP_STD rows that exceeds the algorithm slope
                                '1st: If there are more PREP_STD than the persistance ... delete the older ones
                                Dim resLinq As List(Of AnalyzerManagerDS.sentPreparationsRow) = _
                                    (From a As AnalyzerManagerDS.sentPreparationsRow In _analyzerManager.SentPreparations _
                                     Where String.Equals(a.ExecutionType, "PREP_STD") _
                                     Select a).ToList

                                If resLinq.Count = 0 Then 'If no PREP_STD clear the DataSet table
                                    _analyzerManager.SentPreparations.Clear()
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                Else
                                    For i As Integer = 0 To resLinq.Count - REAGENT_CONTAMINATION_PERSISTANCE - 1
                                        resLinq(i).Delete()
                                    Next
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                End If

                                'AG 10/01/2014 - BT #1432 (sometimes Software does not send the reagent contamination wash)
                                'CHANGE RULE: When a new TEST is sent remove all previous Wash sent (reagents or cuvettes)
                                'The old was wrong and also the index used where wrong!!! - (Now, I think it has no sense)

                                '2on NEW RULE: When send a new PREP_STD clear all previous wash
                                resLinq = (From a As AnalyzerManagerDS.sentPreparationsRow In _analyzerManager.SentPreparations _
                                           Where a.ReagentWashFlag = True Or a.CuvetteWashFlag = True _
                                           Select a).ToList
                                If resLinq.Count > 0 Then '(6.2) 
                                    For i As Integer = resLinq.Count - 1 To 0 Step -1
                                        resLinq(i).Delete()
                                    Next
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                End If
                            End If '(6)
                        End If '(3)
                    End If '(2)
                End If '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextSTDPreparation", EventLogEntryType.Error, False)

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

            Return resultData
        End Function
#End Region

        ''' <summary>
        ''' Check if next preparation is an ISE preparation
        ''' </summary>
        ''' <param name="pLookForISEExecutionsFlag"></param>
        ''' <remarks></remarks>
        Private Function CheckIsIsePreparation(ByVal pLookForISEExecutionsFlag As Boolean) As Boolean

            If Not _rejectedWell AndAlso Not _contaminatedWell AndAlso pLookForISEExecutionsFlag Then '(3.0)
                _resultData = SearchNextISEPreparationNEW(_dbConnection)
                GlobalBase.CreateLogActivity("SearchNextPreparation (SearchNextISEPreparationNEW): " & Now.Subtract(_startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                _startTime = Now

                If Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing Then '(3.1)
                    Dim foundPreparation As ExecutionsDS
                    foundPreparation = CType(_resultData.SetDatos, ExecutionsDS)

                    'RH 26/06/2012 Get data from returned DS
                    If foundPreparation.twksWSExecutions.Rows.Count > 0 Then
                        Dim myRow As ExecutionsDS.twksWSExecutionsRow = foundPreparation.twksWSExecutions(0)

                        _nextRow = _nextPreparationDs.nextPreparation.NewnextPreparationRow
                        _nextRow.ExecutionType = "PREP_ISE"
                        _nextRow.ExecutionID = myRow.ExecutionID
                        _nextRow.SampleClass = myRow.SampleClass

                        _nextRow.CuvetteOpticallyRejectedFlag = False
                        _nextRow.CuvetteContaminationFlag = False
                        _nextRow.ReagentContaminationFlag = False
                        _nextRow.SetWashSolution1Null()
                        _nextRow.SetWashSolution2Null()

                        _nextPreparationDs.nextPreparation.AddnextPreparationRow(_nextRow)

                        _executionFound = myRow.ExecutionID 'RH keep the found ExecutionID in executionFound, as previously
                    End If
                Else
                    Return False
                End If
            End If
            Return True
        End Function

        ''' <summary>
        ''' Check if next cuvette is optically rejected
        ''' </summary>
        ''' <param name="pNextWell"></param>
        ''' <remarks>     
        ''' AG 24/11/2011 change order: 1st cuvette contamination, 2on optically rejected (first version was inverted)
        ''' 1st: Check if next cuvette requires washing (cuvette contamination)
        ''' 2on: Check if next cuvette is optically rejected 
        ''' </remarks>
        Private Function CheckCuvetteIsContaminatedOrOpticallyRejected(ByVal pNextWell As Integer) As Boolean

            If Not _rejectedWell Then '(2.0)
                _resultData = CheckRejectedContaminatedNextWell(_dbConnection, pNextWell, _rejectedWell, _contaminatedWell, _washSol1, _washSol2)
                GlobalBase.CreateLogActivity("SearchNextPreparation (CheckRejectedContaminatedNextWell): " & Now.Subtract(_startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                _startTime = Now

                If Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing Then '(2.1)
                    _analyzerManager.wellContaminatedWithWashSent = 0

                    Debug.Print("Step 1 - SearchNextPreparation -> pNextWell = " & pNextWell.ToString & _
                                "; contaminatedWell = " & _contaminatedWell & _
                                "; rejectedWell = " & _rejectedWell & _
                                "; WashSol2 = " & _washSol2.ToString)

                    If _contaminatedWell Then
                        _analyzerManager.wellContaminatedWithWashSent = pNextWell

                        _nextRow = _nextPreparationDs.nextPreparation.NewnextPreparationRow
                        _nextRow.CuvetteContaminationFlag = True

                        If _washSol1 = "" Then
                            _nextRow.SetWashSolution1Null()
                        Else
                            _nextRow.WashSolution1 = _washSol1
                        End If

                        If _washSol2 = "" Then
                            _nextRow.SetWashSolution2Null()
                        Else
                            _nextRow.WashSolution2 = _washSol2
                        End If

                        _nextRow.CuvetteOpticallyRejectedFlag = False
                        _nextRow.ReagentContaminationFlag = False
                        _nextRow.ExecutionID = 0
                        _nextRow.ExecutionType = ""

                        _nextPreparationDs.nextPreparation.AddnextPreparationRow(_nextRow)

                    ElseIf _rejectedWell Then
                        _nextRow = _nextPreparationDs.nextPreparation.NewnextPreparationRow
                        _nextRow.CuvetteOpticallyRejectedFlag = True

                        _nextRow.CuvetteContaminationFlag = False
                        _nextRow.ReagentContaminationFlag = False
                        _nextRow.ExecutionID = 0
                        _nextRow.ExecutionType = ""

                        _nextPreparationDs.nextPreparation.AddnextPreparationRow(_nextRow)
                    End If 'If contaminatedWell Then
                Else
                    Return False
                End If
            End If

            Return True
        End Function

        ''' <summary>
        ''' Check if next preparation is an STD preparation or a contamination wash
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckNextIsStdPreparationOrContaminationWash()
            Dim existContamination As Boolean = False

            If Not _rejectedWell AndAlso Not _contaminatedWell AndAlso _executionFound = GlobalConstants.NO_PENDING_PREPARATION_FOUND Then '(4.0)
                Dim sampleClassFound As String = ""

                _resultData = SearchNextSTDPreparation(_dbConnection, _executionFound, existContamination, _washSol1, _washSol2, sampleClassFound)
                GlobalBase.CreateLogActivity("SearchNextPreparation (SearchNextSTDPreparation): " & Now.Subtract(_startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
                _startTime = Now

                If Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing Then '(4.1)
                    'Check if exist reagents contaminations
                    If existContamination Then '(4.2)

                        _nextRow = _nextPreparationDs.nextPreparation.NewnextPreparationRow
                        _nextRow.ReagentContaminationFlag = True

                        If _washSol1 = "" Then
                            _nextRow.SetWashSolution1Null()
                            _nextRow.SetWashSolution2Null()
                        Else
                            _nextRow.WashSolution1 = _washSol1
                            _nextRow.WashSolution2 = _washSol1
                        End If
                        _nextRow.CuvetteOpticallyRejectedFlag = False
                        _nextRow.CuvetteContaminationFlag = False

                        _nextRow.ExecutionID = _executionFound 'Execution that requires the washing
                        _nextRow.ExecutionType = "PREP_STD"

                        _nextPreparationDs.nextPreparation.AddnextPreparationRow(_nextRow)

                    Else 'STD execution or NO_PENDING_PREPARATION_FOUND
                        _nextRow = _nextPreparationDs.nextPreparation.NewnextPreparationRow
                        _nextRow.ExecutionType = "PREP_STD"
                        _nextRow.ExecutionID = _executionFound
                        If Not String.Equals(sampleClassFound, "") Then _nextRow.SampleClass = sampleClassFound

                        _nextRow.CuvetteOpticallyRejectedFlag = False
                        _nextRow.CuvetteContaminationFlag = False
                        _nextRow.ReagentContaminationFlag = False
                        _nextRow.SetWashSolution1Null()
                        _nextRow.SetWashSolution2Null()

                        _nextPreparationDs.nextPreparation.AddnextPreparationRow(_nextRow)
                    End If '(4.2)
                End If '(4.1)
            End If '(4.0
        End Sub

        ''' <summary>
        ''' Returns the next STD execution in the same patient - sampletype to perform without contamination
        ''' If not is possible then returns a contamination wash to perform
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pExecutionStdFound"></param>
        ''' <param name="pReagentWashFlag"></param>
        ''' <param name="pWashSol1"></param>
        ''' <param name="pWashSol2"></param>
        ''' <param name="pSampleClassFound" ></param>
        ''' <returns>GolbalDataTo indicates if error or not. If no error use the byref parameters</returns>
        ''' <remarks>AG 18/01/2011</remarks>
        Private Function SearchNextStdPreparation(ByVal pDbConnection As SqlConnection, ByRef pExecutionStdFound As Integer, _
                                                  ByRef pReagentWashFlag As Boolean, ByRef pWashSol1 As String, _
                                                  ByRef pWashSol2 As String, ByRef pSampleClassFound As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                resultData = GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)
                        'Initialize byRef variables (parameters)
                        pExecutionSTDFound = GlobalConstants.NO_PENDING_PREPARATION_FOUND
                        pReagentWashFlag = False
                        pWashSol1 = ""
                        pWashSol2 = ""

                        Dim myExecDlg As New ExecutionsDelegate
                        resultData = myExecDlg.GetPendingExecutionForSendNextProcess(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, True)

                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(3)
                            Dim myExecList = CType(resultData.SetDatos, ExecutionsDS)

                            If myExecList.twksWSExecutions.Rows.Count > 0 Then '(4)
                                '// 0) Remember the last OrderID - SampleType sent to prepare
                                Dim lastOrderID As String = ""
                                Dim lastSampleType As String = ""
                                Dim lastSampleClass As String = ""

                                Dim sentLinqList = (From a As AnalyzerManagerDS.sentPreparationsRow In _analyzerManager.SentPreparations _
                                                Where String.Equals(a.ExecutionType, "PREP_STD") _
                                                Select a).ToList

                                If sentLinqList.Count > 0 Then
                                    If Not sentLinqList(sentLinqList.Count - 1).IsOrderIDNull Then lastOrderID = sentLinqList(sentLinqList.Count - 1).OrderID
                                    If Not sentLinqList(sentLinqList.Count - 1).IsSampleTypeNull Then lastSampleType = sentLinqList(sentLinqList.Count - 1).SampleType
                                    If Not sentLinqList(sentLinqList.Count - 1).IsSampleClassNull Then lastSampleClass = sentLinqList(sentLinqList.Count - 1).SampleClass
                                End If

                                'AG 28/11/2011 - 1) Get all R1 Contaminations and HIGH contamination persistance
                                Dim contaminationsDataDs As ContaminationsDS = Nothing
                                Dim highContaminationPersitance As Integer = 0

                                Dim myContaminationsDelegate As New ContaminationsDelegate
                                resultData = myContaminationsDelegate.GetContaminationsByType(dbConnection, "R1")
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    contaminationsDataDs = DirectCast(resultData.SetDatos, ContaminationsDS)
                                    highContaminationPersitance = REAGENT_CONTAMINATION_PERSISTANCE
                                End If

                                Dim found As Boolean = False
                                Dim myStatFlag As Boolean = True
                                Dim nextToSend As New AnalyzerManagerDS

                                Do
                                    '// First search STATs, if no items found then search ROUTINEs

                                    '// 1) Search myStatFlag executions with the same OrderID-SampleType as lastOrderID-lastSampleType
                                    '// (priority without contaminations with the last sent)
                                    If Not String.Equals(lastOrderID, "") And Not String.Equals(lastSampleType, "") And Not String.Equals(lastSampleClass, "PATIENT") Then
                                        resultData = GetNextExecution(dbConnection, found, myExecList, lastOrderID, lastSampleType, myStatFlag, "", contaminationsDataDs, highContaminationPersitance)
                                    End If


                                    '// 2) If no PENDING items found in 1) then
                                    '//Sorting by SampleClass search myStatFlag executions with the different OrderID-SampleType as lastOrderID-lastSampleType
                                    '// (priority without contaminations with the last sent)
                                    If Not resultData.HasError And Not found Then
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "BLANK", contaminationsDataDs, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "CALIB", contaminationsDataDs, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "CTRL", contaminationsDataDs, highContaminationPersitance)
                                    End If

                                    If Not resultData.HasError And Not found Then
                                        resultData = GetNextExecution(dbConnection, found, myExecList, "", "", myStatFlag, "PATIENT", contaminationsDataDs, highContaminationPersitance)
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

                            End If '(4) 

                        End If '(3)

                    End If '(2)
                End If '(1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextSTDPreparation", EventLogEntryType.Error, False)

            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search next STD preparation, taking care about the last executions sent and search next avoiding contaminations
        ''' using the executions matching the OrderID, SampleType, StatFlag and SampleClass in entry parameters
        ''' 
        ''' This method look for the next execution sorting using the same algoritm that the used in CreateWSExecutions
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pFound" ></param>
        ''' <param name="pStdExecutionList" ></param>
        ''' <param name="pOrderId " ></param>
        ''' <param name="pSampleType " ></param>
        ''' <param name="pStatFlag" ></param>
        ''' <param name="pSampleClass" ></param>
        ''' <param name="pContaminationsDs"></param>
        ''' <param name="pHighContaminationPersitance"></param>
        ''' <returns>AnalyzerManager.searchNext inside a GlobalDataTo</returns>
        ''' <remarks>AG 25/01/2011 - Created</remarks>
        Private Function GetNextExecution(ByVal pDbConnection As SqlConnection, _
                                          ByRef pFound As Boolean, _
                                          ByVal pStdExecutionList As ExecutionsDS, _
                                          ByVal pOrderId As String, _
                                          ByVal pSampleType As String, _
                                          ByVal pStatFlag As Boolean, _
                                          ByVal pSampleClass As String, _
                                          ByVal pContaminationsDs As ContaminationsDS, _
                                          ByVal pHighContaminationPersitance As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then '(2)

                        'Get the list of pending executions depending the entry parameters case
                        'Apply different LINQ (where clause)
                        Dim toSendList As New List(Of ExecutionsDS.twksWSExecutionsRow)
                        If pOrderId <> "" And pSampleType <> "" Then '(3)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pStdExecutionList.twksWSExecutions _
                                            Where a.OrderID = pOrderId AndAlso a.SampleType = pSampleType AndAlso a.StatFlag = pStatFlag _
                                            Select a).ToList
                        ElseIf pOrderId <> "" And pSampleType <> "" And pSampleClass <> "" Then '(1)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pStdExecutionList.twksWSExecutions _
                                            Where a.OrderID = pOrderId AndAlso a.SampleType = pSampleType AndAlso a.StatFlag = pStatFlag AndAlso a.SampleClass = pSampleClass _
                                            Select a).ToList

                        ElseIf pSampleClass <> "" Then '(1)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pStdExecutionList.twksWSExecutions _
                                            Where a.StatFlag = pStatFlag AndAlso a.SampleClass = pSampleClass _
                                            Select a).ToList

                        Else '(3)
                            toSendList = (From a As ExecutionsDS.twksWSExecutionsRow In pStdExecutionList.twksWSExecutions _
                                            Where a.StatFlag = pStatFlag _
                                            Select a).ToList

                        End If '(End 3)

                        'AG 02/03/2012 - CALIB, CTRLS and PATIENTS can not leave the current element until finish with it
                        '                Besides, the PATIENTS must follow the creationorder
                        '                We must edit the toSendList and add the ElementID information <CalibratorID (calibrators) or ControlID (controls) or CreationOrder (patients)>
                        If toSendList.Count > 0 AndAlso pSampleClass <> "BLANK" Then
                            Dim allOrderTestsDS As OrderTestsForExecutionsDS
                            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                            resultData = myWSOrderTestsDelegate.GetInfoOrderTestsForExecutions(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                allOrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsForExecutionsDS)

                                Dim auxOrderTestID As Integer = -1
                                Dim otInfo As List(Of OrderTestsForExecutionsDS.OrderTestsForExecutionsTableRow) = Nothing

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

                        Dim nextExecutionFound As Boolean = False
                        Dim indexNextToSend As Integer = 0

                        If toSendList.Count > 0 Then '(3)

                            '1) Search contamination between previous reagents and the first in toSendList
                            Dim previousReagentIDSentList As List(Of AnalyzerManagerDS.sentPreparationsRow) 'The last reagents used are in the higher array indexes
                            Dim contaminations As List(Of ContaminationsDS.tparContaminationsRow) = Nothing
                            Dim contaminationFound As Boolean = False
                            'Dim myLogAcciones As New ApplicationLogManager() 'Add temporal traces 'AG 28/03/2014 - #1563

                            previousReagentIDSentList = (From a As AnalyzerManagerDS.sentPreparationsRow In _analyzerManager.SentPreparations _
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

                                    'Search if the proper wash has been already sent or not
                                    For i = i To _analyzerManager.SentPreparations.Rows.Count - 1
                                        If _analyzerManager.SentPreparations(i).ReagentWashFlag = True AndAlso _
                                            _analyzerManager.SentPreparations(i).WashSolution1 = requiredWash Then
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
                                                'Search the proper row in _analyzerManager.SentPreparations
                                                For i = 0 To _analyzerManager.SentPreparations.Rows.Count - 1
                                                    'AG 28/03/2014 - #1563 evaluate that ExecutionID is not NULL
                                                    'If previousExecutionsIDSent = _analyzerManager.SentPreparations(i).ExecutionID Then
                                                    If Not _analyzerManager.SentPreparations(i).IsExecutionIDNull AndAlso previousExecutionsIDSent = _analyzerManager.SentPreparations(i).ExecutionID Then
                                                        Exit For
                                                    ElseIf _analyzerManager.SentPreparations(i).IsExecutionIDNull Then
                                                        GlobalBase.CreateLogActivity("Protection! Otherwise the bug #1563 was triggered", "AnalyzerManager.GetNextExecution", EventLogEntryType.Information, False)
                                                    End If
                                                    'AG 28/03/2014 - #1563 
                                                Next

                                                'Search if the proper wash has been already sent or not
                                                For i = i To _analyzerManager.SentPreparations.Rows.Count - 1
                                                    If _analyzerManager.SentPreparations(i).ReagentWashFlag = True AndAlso _
                                                        _analyzerManager.SentPreparations(i).WashSolution1 = requiredWash Then
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
                                    'Dim bestResultList As List(Of ExecutionsDS.twksWSExecutionsRow)
                                    Dim currentResultList As List(Of ExecutionsDS.twksWSExecutionsRow)
                                    'Dim bestContaminationNumber As Integer = 0
                                    'Dim currentContaminationNumber As Integer = 0

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

                                    '2.2) If contaminations: ApplyOptimizationPolicyANew, ApplyOptimizationPolicyBNew, ApplyOptimizationPolicyCNew and ApplyOptimizationPolicyDNew
                                    'and choose the best solution

                                    currentResultList = toSendList.ToList() 'Initial order                                    
                                    toSendList = myExDlgte.ManageContaminationsForRunningAndStatic(_analyzerManager.ActiveAnalyzer, dbConnection, pContaminationsDs, currentResultList, pHighContaminationPersitance, contaminNumber, myReagentsIDList, myMaxReplicatesList)

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
                                                'Search the proper row in _analyzerManager.SentPreparations
                                                For i = 0 To _analyzerManager.SentPreparations.Rows.Count - 1
                                                    'AG 28/03/2014 - #1563 evaluate that ExecutionID is not NULL
                                                    'If previousExecutionsIDSent = _analyzerManager.SentPreparations(i).ExecutionID Then
                                                    If Not _analyzerManager.SentPreparations(i).IsExecutionIDNull AndAlso previousExecutionsIDSent = _analyzerManager.SentPreparations(i).ExecutionID Then
                                                        Exit For
                                                    ElseIf _analyzerManager.SentPreparations(i).IsExecutionIDNull Then
                                                        GlobalBase.CreateLogActivity("Protection! Otherwise the bug #1563 was triggered", "AnalyzerManager.GetNextExecution", EventLogEntryType.Information, False)
                                                    End If
                                                    'AG 28/03/2014 - #1563 
                                                Next

                                                'Search if the proper wash has been already sent or not
                                                contaminationFound = True
                                                nextExecutionFound = False
                                                For i = i To _analyzerManager.SentPreparations.Rows.Count - 1
                                                    If _analyzerManager.SentPreparations(i).ReagentWashFlag = True AndAlso _
                                                        _analyzerManager.SentPreparations(i).WashSolution1 = myWashSolutionType Then

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

                        End If '(End 3)
                    End If '(End 2)
                End If '(End 1)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecution", EventLogEntryType.Error, False)
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
        Private Function SearchNextISEPreparationNEW(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim execDel As New ExecutionsDelegate
                        resultData = execDel.GetNextPendingISEExecutionNEW(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession)
                    Else
                        resultData.SetDatos = New ExecutionsDS
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextISEPreparationNEW", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function
#End Region
    End Class
End Namespace
