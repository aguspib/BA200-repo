Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Class Preparation

    Private _analyzerManager As IAnalyzerManager
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

    Private ReadOnly _pDbConnection As SqlConnection
    Private ReadOnly _startTimeTotal As DateTime = Now



    Public Sub New(ByVal analyzerManager As IAnalyzerManager, ByVal sqlConnection As SqlConnection)
        _analyzerManager = analyzerManager
        _pDbConnection = sqlConnection
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pNextWell"></param>
    ''' <param name="pLookForISEExecutionsFlag"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SearchNext(ByVal pNextWell As Integer, Optional ByVal pLookForISEExecutionsFlag As Boolean = True) As GlobalDataTO
        Try
            _resultData = GetOpenDBConnection(_pDbConnection)

            If (Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing) Then '(1)
                _dbConnection = DirectCast(_resultData.SetDatos, SqlConnection)

                If (Not _dbConnection Is Nothing) Then '(2)

                    CheckCuvetteStatus(pNextWell)

                    CheckIsIse(pLookForISEExecutionsFlag)


                    '''''
                    '4rh: Check if next preparation is an STD preparation or a contamination wash
                    '''''
                    Dim existContamination As Boolean = False

                    If Not _rejectedWell AndAlso Not _contaminatedWell AndAlso _executionFound = GlobalConstants.NO_PENDING_PREPARATION_FOUND Then '(4.0)
                        Dim sampleClassFound As String = ""

                        _resultData = _analyzerManager.SearchNextSTDPreparation(_dbConnection, _executionFound, existContamination, _washSol1, _washSol2, sampleClassFound)
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

                    'Return data
                    _nextPreparationDs.AcceptChanges()

                    _resultData.SetDatos = _nextPreparationDs

                End If '(2)
            End If '(1)

            GlobalBase.CreateLogActivity("SearchNextPreparation (Complete): " & Now.Subtract(_startTimeTotal).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)


        Catch ex As Exception
            _resultData = New GlobalDataTO()
            _resultData.HasError = True
            _resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            _resultData.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Error, False)
        End Try

        'We have used Exit Try so we have to be sure the connection becomes properly closed here
        If (_pDbConnection Is Nothing) AndAlso (Not _dbConnection Is Nothing) Then _dbConnection.Close()

        Return _resultData

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
    Public Function SendNext(ByVal pNextWell As Integer, ByRef pActionSentFlag As Boolean, ByRef pEndRunSentFlag As Boolean, ByRef pSystemErrorFlag As Boolean) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation
            'Dim myLogAcciones As New ApplicationLogManager()

            Dim iseStatusOK As Boolean = False
            Dim iseInstalledFlag As Boolean = False
            Dim adjustValue As String = ""
            adjustValue = _analyzerManager.ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
            If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                iseInstalledFlag = CType(adjustValue, Boolean)
            End If

            If iseInstalledFlag Then
                'Ise damaged (ERR) or switch off (WARN)
                If _analyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.ISE_FAILED_ERR) OrElse _analyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
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

                myAnManagerDS = CType(_analyzerManager.myNextPreparationToSendDS, AnalyzerManagerDS)

                If myAnManagerDS.nextPreparation.Rows.Count > 0 Then '(2)
                    'Analyze the row0 found
                    '1st: Check if next cuvette is optically rejected ... send nothing (DUMMY)
                    If Not actionAlreadySent And Not endRunToSend Then
                        If Not myAnManagerDS.nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull AndAlso myAnManagerDS.nextPreparation(0).CuvetteOpticallyRejectedFlag Then
                            'Send a SKIP instruction but mark as actionAlreadySend. So Ax00 will performe a Dummy cycle 
                            StartTime = Now 'AG 28/06/2012

                            myGlobal = _analyzerManager.AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
                            actionAlreadySent = True

                            GlobalBase.CreateLogActivity("SKIP sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
                        End If
                    End If

                    '2on: Check if next cuvette requires washing (cuvette contamination)
                    If Not actionAlreadySent And Not endRunToSend Then
                        If Not myAnManagerDS.nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso myAnManagerDS.nextPreparation(0).CuvetteContaminationFlag Then
                            StartTime = Now 'AG 28/06/2012

                            myGlobal = _analyzerManager.AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH_RUNNING, myAnManagerDS)
                            actionAlreadySent = True
                            If Not myGlobal.HasError Then
                                AddNewSentPreparationsList(Nothing, myAnManagerDS, pNextWell) 'Update sent instructions DS
                                GlobalBase.CreateLogActivity("WRUN cuvette sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
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
                                        ISEAnalyzer.CurrentProcedure = ISEManager.ISEProcedures.Test
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

                                    GlobalBase.CreateLogActivity("ISETEST sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
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
                                GlobalBase.CreateLogActivity("WRUN reagents sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
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

                                GlobalBase.CreateLogActivity("TEST or PTEST sent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SendNextPreparation", EventLogEntryType.Information, False) 'AG 28/06/2012
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

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SendNextPreparation", EventLogEntryType.Error, False)
        End Try
        Return myGlobal
    End Function


#Region "Aux Functions"

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
    Private Function CheckRejectedContaminatedNextWell(ByVal pDBConnection As SqlConnection, ByVal pNextWell As Integer, _
                                                ByRef pRejectedWell As Boolean, ByRef pContaminatedWell As Boolean, _
                                                ByRef pWashSol1 As String, ByRef pWashSol2 As String) As GlobalDataTO 'Implements IAnalyzerManager.CheckRejectedContaminatedNextWell
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

                            'Search if next well is reagent contaminated by his last execution on it                            
                            Dim rotorTurnWithWellContamination As Integer = 0
                            Dim listSTDtest = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In myReactionsDS.twksWSReactionsRotor _
                                           Where a.WellContent = "C" AndAlso a.WashRequiredFlag = True AndAlso a.WashedFlag = False _
                                           Select a Order By a.RotorTurn Descending)

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
                                               Select a Order By a.RotorTurn Descending)
                                If listSTDtest.Count > 0 Then
                                    If listSTDtest(0).RotorTurn > rotorTurnWithWellContamination Then pContaminatedWell = False
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

    ''' <summary>
    ''' Inform the instruction sent in Running (only STD test preparation, or WRUN (reagent contamination) or WRUN (cuvette contamination)
    ''' into the structure that contains the last instructions sent used for calculate the next to be send
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pNewPreparationSent"></param>
    ''' <param name="pNextWell"></param>
    ''' <returns></returns>
    ''' <remarks>Modified AG 10/01/2014</remarks>
    Private Function AddNewSentPreparationsList(ByVal pDBConnection As SqlConnection, ByVal pNewPreparationSent As AnalyzerManagerDS, ByVal pNextWell As Integer) As GlobalDataTO
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

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SearchNextSTDPreparation", EventLogEntryType.Error, False)

            'Finally
        End Try

        'We have used Exit Try so we have to sure the connection becomes properly closed here
        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        Return resultData
    End Function

    ''' <summary>
    ''' Check if next preparation is an ISE preparation
    ''' </summary>
    ''' <param name="pLookForISEExecutionsFlag"></param>
    ''' <remarks></remarks>
    Private Function CheckIsIse(ByVal pLookForISEExecutionsFlag As Boolean) As Boolean

        If Not _rejectedWell AndAlso Not _contaminatedWell AndAlso pLookForISEExecutionsFlag Then '(3.0)
            _resultData = _analyzerManager.SearchNextISEPreparationNEW(_dbConnection)
            GlobalBase.CreateLogActivity("SearchNextPreparation (SearchNextISEPreparationNEW): " & Now.Subtract(_startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.SearchNextPreparation", EventLogEntryType.Information, False)
            _startTime = Now

            If Not _resultData.HasError AndAlso Not _resultData.SetDatos Is Nothing Then '(3.1)
                Dim FoundPreparation As ExecutionsDS
                FoundPreparation = CType(_resultData.SetDatos, ExecutionsDS)

                'RH 26/06/2012 Get data from returned DS
                If FoundPreparation.twksWSExecutions.Rows.Count > 0 Then
                    Dim myRow As ExecutionsDS.twksWSExecutionsRow = FoundPreparation.twksWSExecutions(0)

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
    Private Function CheckCuvetteStatus(ByVal pNextWell As Integer) As Boolean

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

#End Region
End Class
