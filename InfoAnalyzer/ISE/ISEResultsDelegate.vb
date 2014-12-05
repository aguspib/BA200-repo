'Option Explicit On
'Option Strict On

'Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.DAL
'Imports System.Xml
'Imports System.Configuration

'Namespace Biosystems.Ax00.InfoAnalyzer
'    Public Class ISEResultsDelegate

'#Region "Declarations"
'        Private ReadOnly myAffectedElementsHT As New Hashtable()
'        Private ReadOnly myISEModuleErrorHT As New Hashtable()

'#End Region

'#Region "Declarations"
'        'Private OddParity() As Byte = {0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0}

'#End Region


'#Region "Constructor"
'        Public Sub New()
'            FillAffectedElementHT()
'            FillISEModuleErrorHT()
'        End Sub
'#End Region

'#Region "ISE TEST Results"

'        ''' <summary>
'        ''' Initializes structures, get the execution, set the values results and 
'        ''' finally save results into database.
'        ''' </summary>
'        ''' <param name="pDBConnection">Database Connection</param>
'        ''' <param name="pPreparationID">Preparation ID</param>
'        ''' <param name="pISEResult">Recived ISE Result</param>
'        ''' <param name="pISEMode"> 
'        ''' Indicate the ISE module operating Mode
'        '''  SimpleMode.
'        '''  DebugMode1.
'        '''  DebugMode2.
'        ''' </param>
'        ''' <param name="pWorkSessionID" ></param>
'        ''' <param name="pAnalyzerID" ></param>
'        ''' <returns>GlobalDataTo with setDatos as ExecutionsDS</returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 03/01/2010
'        ''' AG 31/03/2011 - add pWorkSessionID, pAnalyzerID parameters
'        ''' AG 29/11/2011 - returns a GlobalDataTO with an ExecutionDS (executions affected) inside
'        ''' </remarks>
'        Public Function ProcessISETESTResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pPreparationID As Integer, _
'                                                        ByRef pISEResult As ISEResultTO, ByVal pISEMode As String, _
'                                                        ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection
'            Try
'                Dim myReturnValue As New ExecutionsDS  'AG 29/11/2011

'                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
'                If (Not myGlobalDataTO.HasError) Then
'                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then

'                        Dim myDebugModeOn As Boolean
'                        If pISEMode = "SimpleMode" Then
'                            myDebugModeOn = False
'                        ElseIf pISEMode = "DebugMode1" OrElse pISEMode = "DebugMode2" Then
'                            myDebugModeOn = True
'                        End If

'                        'Decode the recived ISE Result.
'                        Dim myISEDecodeDelegate As New ISEDecodeDelegate
'                        Dim myISECycle As New GlobalEnumerates.ISECycles
'                        myISECycle = GlobalEnumerates.ISECycles.NONE

'                        Dim myISEResultStr As String = pISEResult.ReceivedResults
'                        myGlobalDataTO = myISEDecodeDelegate.ConvertISETESTResultToISEResultTO(myISEResultStr, myDebugModeOn)

'                        If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
'                            'Set the result to my ISE result TO 

'                            pISEResult = DirectCast(myGlobalDataTO.SetDatos, ISEResultTO)
'                            pISEResult.ReceivedResults = myISEResultStr

'                            'SG 23/01/2012 -Set the alarms.
'                            Dim myErrorStr As String = pISEResult.ResultErrorsString
'                            'Dim isCancelError As Boolean = Not myErrorStr.StartsWith("0")
'                            If pISEResult.IsCancelError Then
'                                myGlobalDataTO = myISEDecodeDelegate.GetCancelError(myErrorStr)
'                            Else
'                                myGlobalDataTO = myISEDecodeDelegate.GetResultErrors(myErrorStr)
'                            End If

'                            If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
'                                Dim myResultErrors As List(Of ISEErrorTO) = CType(myGlobalDataTO.SetDatos, List(Of ISEErrorTO))
'                                pISEResult.ResultErrors = myResultErrors
'                            End If
'                            'SG 23/01/2012 -END.

'                            Dim myExecutionDelegate As New ExecutionsDelegate
'                            Dim myExecutionDS As New ExecutionsDS

'                            'Get the execution by the preparation ID 
'                            myGlobalDataTO = myExecutionDelegate.GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID)
'                            If Not myGlobalDataTO.HasError Then
'                                myExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
'                                Dim myOrderTestID As Integer = -1
'                                Dim myOrderID As String = ""
'                                Dim myWorkSessionID As String = ""
'                                Dim myReplicateNumber As Integer = -1

'                                'Validate if there are any record before continue
'                                If myExecutionDS.twksWSExecutions.Count > 0 Then
'                                    'Set the order test id.
'                                    myOrderTestID = myExecutionDS.twksWSExecutions(0).OrderTestID
'                                    'Set the replicate number.
'                                    myReplicateNumber = myExecutionDS.twksWSExecutions(0).ReplicateNumber
'                                    'Set the worksession
'                                    myWorkSessionID = myExecutionDS.twksWSExecutions(0).WorkSessionID

'                                    'Get related execution in case there are any other ISEtest related to the current result.
'                                    myGlobalDataTO = myExecutionDelegate.GetISEExecutionsByOrderTestAndPreparationID(dbConnection, myOrderTestID, pPreparationID)

'                                    If Not myGlobalDataTO.HasError Then
'                                        myExecutionDS.twksWSExecutions.Clear() 'Clear the dataset before realoading the data.

'                                        myExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS) 'Fill with the executions 

'                                        'Prepare the Result DS
'                                        Dim myResultDS As New ResultsDS
'                                        Dim myResultsRow As ResultsDS.twksResultsRow
'                                        'Elements for Alarms treatment.
'                                        'Dim myISERemarksList As New List(Of ISERemarkTO)
'                                        'Dim qAlarmResult As New List(Of ISERemarkTO)
'                                        Dim myISEErrorsList As New List(Of ISEErrorTO)
'                                        Dim qAlarmResult As New List(Of ISEErrorTO)
'                                        Dim myExecutionsAlarmsDS As New WSExecutionAlarmsDS
'                                        Dim myExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow
'                                        Dim myExecutionAlarmsDelegate As New WSExecutionAlarmsDelegate
'                                        Dim myResultAlarmsDS As New ResultAlarmsDS
'                                        Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow
'                                        Dim myResultAlarmsDelegate As New ResultAlarmsDelegate 'TR 07/12/2011 
'                                        'Go trough each execution to set the values recived.
'                                        For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
'                                            'Validate if the replicate number is the same as the execution result.
'                                            If execRow.ReplicateNumber = myReplicateNumber AndAlso execRow.ExecutionStatus <> "CLOSED" Then
'                                                execRow.ExecutionStatus = "CLOSED"
'                                                execRow.InUse = True
'                                                execRow.ResultDate = DateTime.Now
'                                                'Set the concentration value depenting on the ISE ResultID
'                                                Select Case execRow.ISE_ResultID
'                                                    Case "Na"
'                                                        execRow.CONC_Value = pISEResult.ConcentrationValues.Na
'                                                        'execRow.CONC_Value = myISEResultTO.Na
'                                                        Exit Select
'                                                    Case "K"
'                                                        execRow.CONC_Value = pISEResult.ConcentrationValues.K
'                                                        'execRow.CONC_Value = myISEResultTO.K
'                                                        Exit Select
'                                                    Case "Cl"
'                                                        execRow.CONC_Value = pISEResult.ConcentrationValues.Cl
'                                                        'execRow.CONC_Value = myISEResultTO.Cl
'                                                        Exit Select
'                                                    Case "Li"
'                                                        execRow.CONC_Value = pISEResult.ConcentrationValues.Li
'                                                        'execRow.CONC_Value = myISEResultTO.Li
'                                                        Exit Select

'                                                    Case Else
'                                                        Exit Select
'                                                End Select

'                                                'Tratar las Alarmas. Set the alarms.
'                                                qAlarmResult = (From a In pISEResult.ResultErrors _
'                                                                Where a.Affected.Contains(execRow.ISE_ResultID) OrElse _
'                                                                (a.DigitNumber = 1 AndAlso Not a.Message = "") _
'                                                                Select a).ToList()

'                                                If qAlarmResult.Count > 0 Then
'                                                    For Each ISEError As ISEErrorTO In qAlarmResult
'                                                        myExecutionAlarmsRow = myExecutionsAlarmsDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
'                                                        myExecutionAlarmsRow.ExecutionID = execRow.ExecutionID
'                                                        myExecutionAlarmsRow.AlarmDateTime = DateTime.Now
'                                                        myExecutionAlarmsRow.AlarmID &= ISEError.Message
'                                                        myExecutionsAlarmsDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(myExecutionAlarmsRow)
'                                                    Next
'                                                End If

'                                                'Fill the result row
'                                                myResultsRow = myResultDS.twksResults.NewtwksResultsRow()
'                                                myResultsRow.OrderTestID = execRow.OrderTestID
'                                                myResultsRow.RerunNumber = execRow.RerunNumber
'                                                myResultsRow.MultiPointNumber = execRow.MultiItemNumber
'                                                myResultsRow.ValidationStatus = "OK"
'                                                myResultsRow.AcceptedResultFlag = True
'                                                myResultsRow.ExportStatus = "False"
'                                                myResultsRow.Printed = False
'                                                myResultsRow.CONC_Value = execRow.CONC_Value
'                                                'TR 05/12/2011 
'                                                myResultsRow.TestID = execRow.TestID
'                                                myResultsRow.SampleType = execRow.SampleType
'                                                'TR 05/12/2011 -END.
'                                                myResultsRow.ResultDateTime = execRow.ResultDate
'                                                myResultsRow.TS_DateTime = DateTime.Now

'                                                'Get the current application user.
'                                                Dim currentSession As New GlobalBase
'                                                myResultsRow.TS_User = currentSession.GetSessionInfo.UserName

'                                                'Add the new result row.
'                                                myResultDS.twksResults.AddtwksResultsRow(myResultsRow)

'                                                ''TR 05/12/2011 -Proccess the Ref Ranges Alarm and inser into Ranges alarm
'                                                myGlobalDataTO = IsValidISERefRanges(dbConnection, execRow.OrderTestID, execRow.TestID, execRow.SampleType, execRow.CONC_Value)
'                                                If Not myGlobalDataTO.HasError AndAlso Not CBool(myGlobalDataTO.SetDatos) Then
'                                                    'Fill the result alarm DataRow
'                                                    myExecutionAlarmsRow = myExecutionsAlarmsDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
'                                                    myExecutionAlarmsRow.ExecutionID = execRow.ExecutionID
'                                                    myExecutionAlarmsRow.AlarmDateTime = DateTime.Now
'                                                    myExecutionAlarmsRow.AlarmID &= GlobalEnumerates.Alarms.CONC_REMARK7.ToString
'                                                    myExecutionsAlarmsDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(myExecutionAlarmsRow)
'                                                End If
'                                                ''TR 05/12/2011 -END

'                                            Else
'                                                'Removed from dataset.
'                                                execRow.Delete()
'                                            End If
'                                        Next
'                                        'Accept all changes on dataset.
'                                        myExecutionDS.AcceptChanges()

'                                        'Save all the execution 
'                                        myGlobalDataTO = myExecutionDelegate.SaveExecutionsResults(dbConnection, myExecutionDS)
'                                        myReturnValue = myExecutionDS 'AG 29/11/2011

'                                        'Save the executions alarms.
'                                        If Not myGlobalDataTO.HasError Then
'                                            Dim myTempExecAlarmDS As New WSExecutionAlarmsDS
'                                            For Each execAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In myExecutionsAlarmsDS.twksWSExecutionAlarms.Rows
'                                                'TR 07/12/2011 -First remove alarm if exist
'                                                myGlobalDataTO = myExecutionAlarmsDelegate.DeleteAll(dbConnection, execAlarmRow.ExecutionID)
'                                                If myGlobalDataTO.HasError Then Exit For

'                                                myTempExecAlarmDS.Clear()
'                                                myTempExecAlarmDS.twksWSExecutionAlarms.ImportRow(execAlarmRow)
'                                                myGlobalDataTO = myExecutionAlarmsDelegate.Add(dbConnection, myTempExecAlarmDS)
'                                                If myGlobalDataTO.HasError Then
'                                                    Exit For
'                                                End If
'                                            Next
'                                        End If

'                                        'Save all the result if not errors.
'                                        If Not myGlobalDataTO.HasError Then
'                                            Dim myResultDelegate As New ResultsDelegate
'                                            Dim myTemResultDS As New ResultsDS
'                                            Dim myResultExecutionsAlarmsDS As New WSExecutionAlarmsDS 'TR 24/01/2012
'                                            Dim myTemExecutionDS As New ExecutionsDS
'                                            Dim myAverage As Single = 0
'                                            'Save the Results One by One.
'                                            For Each ResultRow As ResultsDS.twksResultsRow In myResultDS.twksResults.Rows
'                                                'Get the accepted result to set the average value if there are 
'                                                myGlobalDataTO = GetAverageConcentrationValue(dbConnection, ResultRow.OrderTestID, _
'                                                                                                                  ResultRow.RerunNumber)
'                                                If Not myGlobalDataTO.HasError Then
'                                                    'Get the concentration value to calculate the the average.
'                                                    myAverage = CType(myGlobalDataTO.SetDatos, Single)
'                                                    ResultRow.CONC_Value = myAverage
'                                                Else
'                                                    Exit For
'                                                End If
'                                                'AG 13/01/2011

'                                                'Import result row into temporal structure.
'                                                myTemResultDS.twksResults.ImportRow(ResultRow)
'                                                'Save results on result table.
'                                                myGlobalDataTO = myResultDelegate.SaveResults(dbConnection, myTemResultDS)
'                                                If myGlobalDataTO.HasError Then Exit For

'                                                'Reset the accepted result 
'                                                myGlobalDataTO = myResultDelegate.ResetAcceptedResultFlag(dbConnection, ResultRow.OrderTestID, ResultRow.RerunNumber)

'                                                If Not myGlobalDataTO.HasError Then
'                                                    'clear temporal structure to reuse.
'                                                    myTemResultDS.twksResults.Clear()
'                                                End If

'                                                'TR 05/12/2011 -Proccess the Ref Ranges Alarm and insert into Ranges alarm.
'                                                myGlobalDataTO = IsValidISERefRanges(dbConnection, ResultRow.OrderTestID, ResultRow.TestID, ResultRow.SampleType, myAverage)
'                                                If Not myGlobalDataTO.HasError Then
'                                                    If Not CBool(myGlobalDataTO.SetDatos) Then
'                                                        myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
'                                                        myResultAlarmRow.OrderTestID = ResultRow.OrderTestID
'                                                        myResultAlarmRow.RerunNumber = ResultRow.RerunNumber
'                                                        myResultAlarmRow.MultiPointNumber = 1
'                                                        myResultAlarmRow.AlarmID = GlobalEnumerates.Alarms.CONC_REMARK7.ToString
'                                                        myResultAlarmRow.AlarmDateTime = Now
'                                                        myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)

'                                                    End If
'                                                Else
'                                                    Exit For
'                                                End If
'                                                'TR 05/12/2011 -END

'                                                'TR 24/01/2012 -Search if there're any alarm for the current resutl to show.
'                                                If Not myGlobalDataTO.HasError Then
'                                                    myGlobalDataTO = myExecutionDelegate.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, ResultRow.OrderTestID, ResultRow.MultiPointNumber)
'                                                    If Not myGlobalDataTO.HasError Then
'                                                        myTemExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)

'                                                        For Each execRow As ExecutionsDS.twksWSExecutionsRow In myTemExecutionDS.twksWSExecutions.Rows
'                                                            'Get the execution alarm by the execution ID 
'                                                            myGlobalDataTO = myExecutionAlarmsDelegate.Read(dbConnection, execRow.ExecutionID)
'                                                            If Not myGlobalDataTO.HasError Then
'                                                                myResultExecutionsAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, WSExecutionAlarmsDS)
'                                                                'myResultExecutionsAlarmsDS
'                                                                For Each ResultExeAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In myResultExecutionsAlarmsDS.twksWSExecutionAlarms.Rows
'                                                                    If Not ResultExeAlarmRow.AlarmID = GlobalEnumerates.Alarms.CONC_REMARK7.ToString Then
'                                                                        'Before adding the row validate if not exist in curren Dataset
'                                                                        If Not myResultAlarmsDS.twksResultAlarms.Where(Function(a) a.OrderTestID = ResultRow.OrderTestID _
'                                                                                                                       AndAlso a.RerunNumber = ResultRow.RerunNumber _
'                                                                                                                       AndAlso a.MultiPointNumber = ResultRow.MultiPointNumber _
'                                                                                                                       AndAlso a.AlarmID = ResultExeAlarmRow.AlarmID).Count > 0 Then
'                                                                            myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
'                                                                            myResultAlarmRow.OrderTestID = ResultRow.OrderTestID
'                                                                            myResultAlarmRow.RerunNumber = ResultRow.RerunNumber
'                                                                            myResultAlarmRow.MultiPointNumber = 1
'                                                                            myResultAlarmRow.AlarmID = ResultExeAlarmRow.AlarmID
'                                                                            myResultAlarmRow.AlarmDateTime = Now
'                                                                            myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
'                                                                        End If
'                                                                    End If
'                                                                Next
'                                                            End If
'                                                        Next
'                                                    End If
'                                                End If
'                                                'TR 24/01/2012 -END

'                                                'TR 07/12/2011 -Delete all related alarms before entering.
'                                                myGlobalDataTO = myResultAlarmsDelegate.DeleteAll(dbConnection, ResultRow.OrderTestID, ResultRow.RerunNumber, ResultRow.MultiPointNumber)

'                                            Next

'                                        End If

'                                        If Not myGlobalDataTO.HasError Then
'                                            'TR 05/12/2011 -Insert. The Result alarms
'                                            myGlobalDataTO = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
'                                        End If

'                                        'Update the OrderTest status.
'                                        If Not myGlobalDataTO.HasError Then
'                                            Dim myOrderTestDelegate As New OrderTestsDelegate
'                                            Dim myOrderTestDS As New OrderTestsDS

'                                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
'                                                'Get the order Test to validate the replicate number.
'                                                myGlobalDataTO = myOrderTestDelegate.GetOrderTest(dbConnection, execRow.OrderTestID)
'                                                If Not myGlobalDataTO.HasError Then
'                                                    myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)
'                                                    If myOrderTestDS.twksOrderTests.Count > 0 Then
'                                                        'Set the order id to get the patient id later 
'                                                        myOrderID = myOrderTestDS.twksOrderTests(0).OrderID

'                                                        'If the same replicate number then update the status to close
'                                                        If myOrderTestDS.twksOrderTests(0).ReplicatesNumber = execRow.ReplicateNumber Then
'                                                            myGlobalDataTO = myOrderTestDelegate.UpdateStatusByOrderTestID(dbConnection, execRow.OrderTestID, "CLOSED")
'                                                            'If error then exit for.
'                                                            If myGlobalDataTO.HasError Then Exit For

'                                                        End If
'                                                    End If
'                                                End If
'                                            Next
'                                        End If

'                                        'TR 14/01/2011 -If the debug mode is on then save the results on the IseDebugModes Result
'                                        If myDebugModeOn AndAlso Not myGlobalDataTO.HasError Then
'                                            Dim myOrdersDelegate As New OrdersDelegate
'                                            'Call the Order to get the patient id or the Sample ID
'                                            myGlobalDataTO = myOrdersDelegate.ReadOrders(dbConnection, myOrderID)
'                                            If Not myGlobalDataTO.HasError Then
'                                                Dim myOrdersDS As New OrdersDS
'                                                Dim myPatientID As String = ""
'                                                myOrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
'                                                If myOrdersDS.twksOrders.Rows.Count > 0 Then

'                                                    If Not myOrdersDS.twksOrders(0).IsPatientIDNull Then
'                                                        myPatientID = myOrdersDS.twksOrders(0).PatientID 'Set the patienID if not null
'                                                    ElseIf Not myOrdersDS.twksOrders(0).IsSampleIDNull Then
'                                                        myPatientID = myOrdersDS.twksOrders(0).SampleID 'set the sampleID if not null
'                                                    End If
'                                                    'Validate the ise type to set the ise Cycle.
'                                                    Select Case pISEResult.ISEResultType
'                                                        Case ISEResultTO.ISEResultTypes.SER
'                                                            myISECycle = GlobalEnumerates.ISECycles.SAMPLE
'                                                            Exit Select
'                                                        Case ISEResultTO.ISEResultTypes.URN
'                                                            myISECycle = GlobalEnumerates.ISECycles.URINE
'                                                            Exit Select
'                                                        Case ISEResultTO.ISEResultTypes.CAL
'                                                            myISECycle = GlobalEnumerates.ISECycles.CALIBRATION
'                                                            Exit Select

'                                                        Case Else
'                                                            Exit Select

'                                                    End Select

'                                                    pISEResult.WorkSessionID = myWorkSessionID
'                                                    pISEResult.PatientID = myPatientID
'                                                    'Call the save debug result data to save into XML file.
'                                                    myGlobalDataTO = SaveDebugModeResultData(myWorkSessionID, myPatientID, pISEResult.ReceivedResults, myISECycle)
'                                                End If
'                                            End If
'                                        End If
'                                        'TR 14/01/2011 -END
'                                    End If
'                                End If
'                            End If
'                        End If

'                        If (Not myGlobalDataTO.HasError) Then
'                            'When the Database Connection was opened locally, then the Commit is executed
'                            myGlobalDataTO.SetDatos = myReturnValue 'AG 29/11/2011
'                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
'                        Else
'                            'When the Database Connection was opened locally, then the Rollback is executed
'                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
'                        End If

'                    End If
'                End If

'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.ProcessISEResults", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Validate if the concentration value is Between the Ref Ranges if APPLY.
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pOrderTestID"></param>
'        ''' <param name="pTestID"></param>
'        ''' <param name="pSampleType"></param>
'        ''' <param name="pCONC_Value"></param>
'        ''' <returns></returns>
'        ''' <remarks>CREATED BY: TR 05/12/2011</remarks>
'        Private Function IsValidISERefRanges(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
'                                             ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pCONC_Value As Single) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection
'            Try
'                Dim myResult As Boolean = True
'                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
'                If (Not myGlobalDataTO.HasError) Then
'                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
'                    If (Not dbConnection Is Nothing) Then
'                        'Get the ISE TEST INFO 
'                        Dim myISETestSampleDelegate As New ISETestSamplesDelegate
'                        Dim myISETestSampleDS As New ISETestSamplesDS
'                        myGlobalDataTO = myISETestSampleDelegate.GetListByISETestID(dbConnection, pTestID, pSampleType)

'                        If Not myGlobalDataTO.HasError Then
'                            myISETestSampleDS = DirectCast(myGlobalDataTO.SetDatos, ISETestSamplesDS)

'                            If myISETestSampleDS.tparISETestSamples.Count > 0 Then
'                                Dim myOrderTestsDelegate As New OrderTestsDelegate
'                                'Get the Reference Range Interval defined for the Test.
'                                myGlobalDataTO = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, pOrderTestID, "ISE", _
'                                                                                                pTestID, pSampleType, myISETestSampleDS.tparISETestSamples(0).ActiveRangeType)

'                                If Not myGlobalDataTO.HasError Then
'                                    'Validate the range
'                                    Dim myTestRefRangesDS As New TestRefRangesDS
'                                    myTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)

'                                    If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
'                                        If (myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit <> -1) And _
'                                           (myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit <> -1) Then
'                                            If (pCONC_Value < myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit) OrElse _
'                                               (pCONC_Value > myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit) Then

'                                                myResult = False

'                                            End If
'                                        End If
'                                    End If
'                                End If
'                            End If
'                        End If
'                    End If
'                End If
'                myGlobalDataTO.SetDatos = myResult

'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.IsValidISERefRanges", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pAnalyzerID"></param>
'        ''' <param name="pWorkSessionID"></param>
'        ''' <param name="pExecutionID"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' Created By:AG 13/01/2011
'        ''' Modified By: TR 17/10/2011 -Correct the Exit Try.
'        '''              TR 23/01/2012 -Implement to show alarms on media value.    
'        ''' </remarks>
'        Public Function RecalculateAverageValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
'                                                ByVal pWorkSessionID As String, ByVal pExecutionID As Integer) As GlobalDataTO
'            Dim resultData As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection

'            Try
'                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
'                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
'                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

'                    If (Not dbConnection Is Nothing) Then
'                        'Get the orderTest & rerun number
'                        Dim exec_delg As New ExecutionsDelegate
'                        resultData = exec_delg.GetExecution(dbConnection, pExecutionID, pAnalyzerID, pWorkSessionID)

'                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
'                            Dim myDS As New ExecutionsDS
'                            myDS = CType(resultData.SetDatos, ExecutionsDS)

'                            If myDS.twksWSExecutions.Rows.Count > 0 Then
'                                Dim myOT As Integer = myDS.twksWSExecutions(0).OrderTestID
'                                Dim myRerun As Integer = myDS.twksWSExecutions(0).RerunNumber

'                                'Get the current Results row (OrderTestID - RerunNumber
'                                Dim results_del As New ResultsDelegate
'                                Dim res_DS As New ResultsDS
'                                resultData = results_del.ReadByOrderTestIDandRerunNumber(dbConnection, myOT, myRerun)
'                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
'                                    res_DS = CType(resultData.SetDatos, ResultsDS)
'                                    'Else
'                                    '    Exit Try
'                                End If
'                                'TR 17/10/2011 -Implemented to avoid Exit Try
'                                If Not resultData.HasError Then
'                                    'Calculate OrderTestID - Rerun new average
'                                    resultData = GetAverageConcentrationValue(dbConnection, myOT, myRerun)
'                                    Dim myAverage As Single = 0
'                                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
'                                        'Get the concentration value to calculate the the average.
'                                        myAverage = CType(resultData.SetDatos, Single)
'                                        'Else
'                                        '  Exit Try
'                                    End If
'                                    'AG 13/01/2011

'                                    'TR 17/10/2011 -Implemented to avoid Exit Try
'                                    If Not resultData.HasError Then
'                                        'update result into DS
'                                        If res_DS.twksResults.Rows.Count > 0 Then
'                                            res_DS.twksResults(0).BeginEdit()
'                                            res_DS.twksResults(0).CONC_Value = myAverage
'                                            res_DS.twksResults(0).AcceptChanges()

'                                            'Save results on result table.
'                                            resultData = results_del.SaveResults(dbConnection, res_DS)
'                                            If resultData.HasError Then Exit Try

'                                            'TR 12/12/2011 -Delete all related alarms before entering.
'                                            Dim myResultAlarmsDelegate As New ResultAlarmsDelegate
'                                            resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, res_DS.twksResults(0).OrderTestID, _
'                                                                                          res_DS.twksResults(0).RerunNumber, res_DS.twksResults(0).MultiPointNumber)

'                                            'Proccess the Ref Ranges Alarm and insert into Ranges alarm.
'                                            resultData = IsValidISERefRanges(dbConnection, res_DS.twksResults(0).OrderTestID, myDS.twksWSExecutions(0).TestID, _
'                                                                             myDS.twksWSExecutions(0).SampleType, myAverage)

'                                            'TR 23/01/2012 -Move here
'                                            Dim myResultAlarmsDS As New ResultAlarmsDS
'                                            Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow
'                                            'TR 23/01/2012 -END.
'                                            If Not resultData.HasError Then
'                                                If Not CBool(resultData.SetDatos) Then
'                                                    'Dim myResultAlarmsDS As New ResultAlarmsDS
'                                                    'Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

'                                                    myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
'                                                    myResultAlarmRow.OrderTestID = res_DS.twksResults(0).OrderTestID
'                                                    myResultAlarmRow.RerunNumber = res_DS.twksResults(0).RerunNumber
'                                                    myResultAlarmRow.MultiPointNumber = 1
'                                                    myResultAlarmRow.AlarmID = GlobalEnumerates.Alarms.CONC_REMARK7.ToString
'                                                    myResultAlarmRow.AlarmDateTime = Now
'                                                    myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)

'                                                End If
'                                                'TR 24/01/2012 -Search if there're any alarm for the current resutl to show.
'                                                If Not resultData.HasError Then
'                                                    Dim myResultExecutionsAlarmsDS As New WSExecutionAlarmsDS 'TR 24/01/2012
'                                                    Dim myTemExecutionDS As New ExecutionsDS
'                                                    Dim myExecutionDelegate As New ExecutionsDelegate
'                                                    'Dim myResultAlarmsDS As New ResultAlarmsDS
'                                                    Dim myExecutionAlarmsDelegate As New WSExecutionAlarmsDelegate

'                                                    resultData = myExecutionDelegate.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, _
'                                                                                                    res_DS.twksResults(0).OrderTestID, res_DS.twksResults(0).MultiPointNumber)
'                                                    If Not resultData.HasError Then
'                                                        myTemExecutionDS = DirectCast(resultData.SetDatos, ExecutionsDS)

'                                                        For Each execRow As ExecutionsDS.twksWSExecutionsRow In myTemExecutionDS.twksWSExecutions.Rows
'                                                            'Validate execution is in Use 
'                                                            If Not execRow.IsInUseNull AndAlso execRow.InUse Then
'                                                                'Get the execution alarm by the execution ID 
'                                                                resultData = myExecutionAlarmsDelegate.Read(dbConnection, execRow.ExecutionID)
'                                                                If Not resultData.HasError Then
'                                                                    myResultExecutionsAlarmsDS = DirectCast(resultData.SetDatos, WSExecutionAlarmsDS)
'                                                                    'myResultExecutionsAlarmsDS
'                                                                    For Each ResultExeAlarmRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In myResultExecutionsAlarmsDS.twksWSExecutionAlarms.Rows
'                                                                        If Not ResultExeAlarmRow.AlarmID = GlobalEnumerates.Alarms.CONC_REMARK7.ToString Then
'                                                                            'Before adding the row validate if not exist in curren Dataset
'                                                                            If Not myResultAlarmsDS.twksResultAlarms.Where(Function(a) a.OrderTestID = res_DS.twksResults(0).OrderTestID _
'                                                                                                                               AndAlso a.RerunNumber = res_DS.twksResults(0).RerunNumber _
'                                                                                                                               AndAlso a.MultiPointNumber = res_DS.twksResults(0).MultiPointNumber _
'                                                                                                                               AndAlso a.AlarmID = ResultExeAlarmRow.AlarmID).Count > 0 Then

'                                                                                myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
'                                                                                myResultAlarmRow.OrderTestID = myOT
'                                                                                myResultAlarmRow.RerunNumber = myRerun
'                                                                                myResultAlarmRow.MultiPointNumber = 1
'                                                                                myResultAlarmRow.AlarmID = ResultExeAlarmRow.AlarmID
'                                                                                myResultAlarmRow.AlarmDateTime = Now
'                                                                                myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
'                                                                            End If

'                                                                        End If
'                                                                    Next
'                                                                End If
'                                                            End If

'                                                        Next
'                                                    End If
'                                                End If
'                                                'TR 24/01/2012 -END

'                                                If Not resultData.HasError Then
'                                                    'Insert. The Result alarms
'                                                    resultData = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
'                                                End If
'                                            End If
'                                            'TR 12/12/2011 -END
'                                        End If
'                                    End If
'                                End If
'                            End If
'                        End If
'                    End If
'                End If

'            Catch ex As Exception
'                resultData.HasError = True
'                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                resultData.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.RecalculateAverageValue", EventLogEntryType.Error, False)

'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try

'            Return resultData
'        End Function

'        ''' <summary>
'        ''' Get the accepted concentration values for specific OrderTestID and Rerun Number.
'        ''' </summary>
'        ''' <param name="pDBConnection"></param>
'        ''' <param name="pOrderTestID">Order Test ID</param>
'        ''' <returns>The concentration value on the Set Data property on GlobalDataTO (Single)</returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 05/01/2010
'        ''' </remarks>
'        Private Function GetAverageConcentrationValue(ByVal pDBConnection As SqlClient.SqlConnection, _
'                                                      ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Dim dbConnection As New SqlClient.SqlConnection

'            Try
'                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
'                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

'                    If (Not dbConnection Is Nothing) Then
'                        Dim myExecDelegate As New ExecutionsDelegate
'                        Dim newAverage As Single = 0

'                        myGlobalDataTO = myExecDelegate.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
'                        If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
'                            Dim execDS As New ExecutionsDS
'                            execDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

'                            Dim itemNumber As Integer = 0
'                            For Each row As ExecutionsDS.twksWSExecutionsRow In execDS.twksWSExecutions
'                                If Not row.IsInUseNull Then
'                                    If row.InUse Then
'                                        newAverage += row.CONC_Value
'                                        itemNumber += 1
'                                    End If
'                                End If
'                            Next
'                            If itemNumber > 0 Then
'                                newAverage = newAverage / itemNumber
'                            End If
'                        End If
'                        myGlobalDataTO.SetDatos = newAverage

'                    End If
'                End If

'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.GetAverageConcentrationValue", EventLogEntryType.Error, False)

'            Finally
'                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'            End Try
'            Return myGlobalDataTO

'        End Function


'#End Region

'#Region "ISE CMD Results"

'        ''' <summary>
'        ''' Overload for processing answer after ISECMD instruction is sent
'        ''' </summary>
'        ''' <param name="pISEResult"></param>
'        ''' <param name="pAnalyzerID"></param>
'        ''' <returns></returns>
'        ''' <remarks>created by SGM 10/01/2012</remarks>
'        Public Function ProcessISECMDResults(ByVal pISEResult As ISEResultTO, ByVal pAnalyzerID As String) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myEndIndex As Integer
'                Dim myDebugModeOn As Boolean = True
'                Dim myISEResultTO As ISEResultTO
'                Dim myISEResultItemType As ISEResultTO.ISEResultItemTypes = ISEResultTO.ISEResultItemTypes.None

'                Dim myResultStr As String = pISEResult.ReceivedResults.Trim

'                If myResultStr.Length > 0 Then
'                    'Decode the recived ISE Result.
'                    Dim myISEDecodeDelegate As New ISEDecodeDelegate

'                    'Dim IsChecksumOK As Boolean = MyClass.CheckChecksum(myResultStr)


'                    'initialize result object
'                    myISEResultTO = New ISEResultTO
'                    myISEResultTO.ReceivedResults = myResultStr

'                    Dim myBlockStr As String

'                    'first check if not an error
'                    If myResultStr.StartsWith("<ERC ") Then
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
'                        myISEResultTO.IsCancelError = True
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ERC)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If



'                    ElseIf myResultStr.Contains("<ISE!") Then
'                        'simple response
'                        myISEResultItemType = ISEResultTO.ISEResultItemTypes.Acknoledge
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.OK

'                    ElseIf myResultStr.Contains("<CAL ") Then
'                        'CALIBRATION VALUES
'                        '********************************************************************************************************************

'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.CAL

'                        'one of the results can be Cancel error
'                        If myResultStr.Contains("<ERC ") Then
'                            'myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
'                            myISEResultTO.IsCancelError = True
'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ERC)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If
'                        End If

'                        'Calibrator B milivolts
'                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Calibrator A milivolts
'                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Calibration Slopes (milivolts/decade)
'                        '<CAL Li xx.xx Na xx.xx K xx.xx Cl xx.xx>
'                        'Must be in the next limits:
'                        'Li+ 47-64 mV/dec
'                        'Na+ 52-64 mV/dec
'                        'K+ 52-64 mV/dec
'                        'Cl- 40-55 mV/dec
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.CAL)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.Calibration1
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        Dim myCal1Index As Integer = myResultStr.IndexOf("<" & ISEResultTO.ISEResultBlockTypes.CAL.ToString)
'                        Dim myCal2Index As Integer = myResultStr.LastIndexOf("<" & ISEResultTO.ISEResultBlockTypes.CAL.ToString)
'                        If (myCal1Index >= 0 And myCal2Index >= 0) AndAlso (myCal2Index > myCal1Index) Then
'                            Dim myCal2Str As String = myResultStr.Substring(myCal2Index)
'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myCal2Str, ISEResultTO.ISEResultBlockTypes.CAL)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Calibration2
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If
'                        End If

'                    ElseIf myResultStr.Contains("<PMC ") Then
'                        'PUMPS CALIBRATION
'                        '**************************************************************************************************************
'                        '<PMC A xxxx B xxxx W xxxx> 
'                        'Values A & B must be between 1500-3000
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.PMC

'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.PMC)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.PumpsCalibration
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If


'                    ElseIf myResultStr.Contains("<BBC ") Then
'                        'BUBBLE DETECTOR CALIBRATION
'                        '**************************************************************************************************************
'                        '<BBC A xxxx B xxxx W xxxx> 
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.BBC

'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BBC)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.BubbleCalibration
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                    ElseIf myResultStr.Contains("<SER ") Then
'                        'SERUM
'                        '*****************************************************************************************************************
'                        'Serum milivolts
'                        '<SMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.SMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.SerumMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Calibrator A milivolts
'                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Calibrator B milivolts
'                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Sample Concentrations
'                        '<SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.SER)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.SerumConcentration
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If



'                    ElseIf myResultStr.Contains("<URN ") Then
'                        'URINE
'                        '*****************************************************************************************************************

'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.URN


'                        'Urine milivolts
'                        '<UMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.UMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.UrineMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        ''Calibrator B milivolts
'                        ''<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        ''Calibrator A milivolts
'                        ''<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        ''urine Concentrations
'                        ''<URN Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.URN)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.UrineConcentration
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If



'                    ElseIf myResultStr.Contains("<ISV ") Then
'                        'CHEKSUM*********************************************************************************************************
'                        '<ISV cccc>
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ISV

'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ISV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.Checksum
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If
'                        'PDT se pasa a caracteres? o se deja en hexadecimal?


'                    ElseIf myResultStr.Contains("<AMV ") Then 'this case must be at the after SER and URN!!
'                        'READ MILIVOLTS or SIPPING (Debug 2)***************************************************************************************

'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.AMV

'                        'Calibrator A milivolts
'                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If

'                        'Calibrator B milivolts
'                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
'                        myBlockStr = ""
'                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
'                        If myBlockStr.Length > 0 Then
'                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
'                            myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                        End If



'                    ElseIf myResultStr.Contains("<DSN ") Or myResultStr.Contains("<DDT ") Then
'                        'DALLAS CARD*********************************************************************************************
'                        If myResultStr.Contains("<DDT 00 ") Then

'                            myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.DDT00

'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DSN)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_SN
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If

'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DDT_00)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_Page0
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If

'                        ElseIf myResultStr.Contains("DDT 01 ") Then

'                            myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.DDT01

'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DSN)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_SN
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If

'                            myBlockStr = ""
'                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DDT_01)
'                            If myBlockStr.Length > 0 Then
'                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_Page1
'                                myGlobal = myISEDecodeDelegate.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
'                            End If

'                        End If

'                    End If
'                    End If


'                    If myISEResultTO IsNot Nothing Then
'                        'Save results into Database?

'                        '??????
'                        ''Call the save debug result data to save into XML file.
'                        'myGlobalDataTO = SaveDebugModeResultData(myWorkSessionID, myPatientID, myISEResultTO.ReceivedResults, myISECycle)
'                        If Not myGlobal.HasError Then
'                            myGlobal.SetDatos = myISEResultTO
'                        End If
'                    End If

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.ProcessISEResults", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal
'        End Function



'#End Region

'#Region "Common"

'        Private Function ExtractResultBlock(ByVal pISEResultStr As String, ByVal pType As ISEResultTO.ISEResultBlockTypes) As String
'            Dim myBlockString As String = ""
'            Try
'                If pISEResultStr.Length > 0 AndAlso pType <> ISEResultTO.ISEResultTypes.None Then
'                    Dim myType As String = pType.ToString
'                    If myType = ISEResultTO.ISEResultBlockTypes.DDT_00.ToString Then myType = "DDT 00"
'                    If myType = ISEResultTO.ISEResultBlockTypes.DDT_01.ToString Then myType = "DDT 01"
'                    Dim myIndex As Integer = pISEResultStr.IndexOf("<" & myType)
'                    If myIndex >= 0 Then
'                        myBlockString = pISEResultStr.Substring(myIndex)
'                        myBlockString = myBlockString.Substring(0, myBlockString.IndexOf(">") + 1)
'                    End If
'                End If
'            Catch ex As Exception
'                Throw ex
'            End Try
'            Return myBlockString
'        End Function



'        ''' <summary>
'        ''' Save the ISE result and the patient or Sample id into 
'        ''' the ISEResultDebugMode.Xml
'        ''' </summary>
'        ''' <param name="pWorkSession">WorkSession ID.</param>
'        ''' <param name="pPatientID">Patient ID</param>
'        ''' <param name="pISEResult">Recived ISE Result</param>
'        ''' <returns></returns>
'        ''' <remarks>CREATE BY: TR 14/01/2011</remarks>
'        Public Function SaveDebugModeResultData(ByVal pWorkSession As String, ByVal pPatientID As String, _
'                                                 ByVal pISEResult As String, ByVal pISECycle As GlobalEnumerates.ISECycles) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim myISEResultsDebugModeDS As New ISEDebugResultsDS
'                'Validate if the Xml file exits

'                If IO.File.Exists(Windows.Forms.Application.StartupPath.ToString() & _
'                                                  GlobalBase.ISEResultDebugModeFilePath) Then

'                    'Load the file into the Type DataSet.
'                    myISEResultsDebugModeDS.ReadXml(Windows.Forms.Application.StartupPath.ToString() & _
'                                                    GlobalBase.ISEResultDebugModeFilePath, XmlReadMode.InferSchema)
'                End If

'                Dim myISEResultRow As ISEDebugResultsDS.DebugResultsRow
'                myISEResultRow = myISEResultsDebugModeDS.DebugResults.NewDebugResultsRow
'                'Set the values to the new row
'                myISEResultRow.WorkSessionID = pWorkSession
'                myISEResultRow.ISEDateTime = DateTime.Now
'                myISEResultRow.ISECycle = pISECycle.ToString()
'                myISEResultRow.PatientID = pPatientID.ToString()
'                myISEResultRow.ISEResult = pISEResult

'                myISEResultsDebugModeDS.DebugResults.AddDebugResultsRow(myISEResultRow)

'                'Save the Xml file into the default file path.
'                myISEResultsDebugModeDS.WriteXml(Windows.Forms.Application.StartupPath.ToString() & GlobalBase.ISEResultDebugModeFilePath)

'                myGlobalDataTO.SetDatos = myISEResultsDebugModeDS

'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.SaveDebugModeResultData", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobalDataTO
'        End Function


'        ''' <summary>
'        ''' Return the Affecte elements from a local stucture depending on the recived value.
'        ''' </summary>
'        ''' <param name="pPositionValue">Value on position.</param>
'        ''' <returns>
'        ''' Returns the affected elements on the indicated position value.
'        ''' </returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 06/01/2010
'        ''' </remarks>
'        Private Function GetAffectedElements(ByVal pPositionValue As String) As String
'            Dim myResult As String = ""
'            Try
'                If myAffectedElementsHT.ContainsKey(pPositionValue) Then
'                    myResult = myAffectedElementsHT(pPositionValue).ToString()
'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.GetAffectedElements", EventLogEntryType.Error, False)
'            End Try

'            Return myResult

'        End Function

'        ''' <summary>
'        ''' Return the ise module Error from local structure Dependin on the recived value.
'        ''' </summary>
'        ''' <param name="pPositionValue"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' CREATED BY: TR 12/12/2011
'        ''' </remarks>
'        Private Function GetISEModuleError(ByVal pPositionValue As String) As String
'            Dim myResult As String = ""
'            Try
'                If myISEModuleErrorHT.ContainsKey(pPositionValue) Then
'                    myResult = myISEModuleErrorHT(pPositionValue).ToString()
'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.GetISEModuleError", EventLogEntryType.Error, False)
'            End Try

'            Return myResult

'        End Function

'        ''' <summary>
'        ''' Load the ISE Parameter xml File into a DataSet on table AffectedElementTable
'        ''' </summary>
'        ''' <param name="pXmlPath"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 11/01/2010
'        ''' </remarks>
'        Private Function LoadISEModuleParammeters(ByVal pXmlPath As String) As DataSet
'            Dim myResultDataSet As New DataSet
'            Try
'                Dim myISEParamXml As New XmlDocument
'                'myISEParamXml.Load(Windows.Forms.Application.StartupPath.ToString() & ConfigurationManager.AppSettings("ISEParammetersFilePath").ToString())
'                'TR 25/01/2011 -Replace by corresponding value on global base.
'                myISEParamXml.Load(Windows.Forms.Application.StartupPath.ToString() & GlobalBase.ISEParammetersFilePath)
'                myResultDataSet.ReadXml(pXmlPath)

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.LoadISEModuleParammeters", EventLogEntryType.Error, False)
'            End Try
'            Return myResultDataSet
'        End Function


'        ''' <summary>
'        ''' Fill the hashTable use to store the affected elements.
'        ''' </summary>
'        ''' <remarks>
'        ''' CREATE BY: TR 06/1/2010
'        ''' </remarks>
'        Private Sub FillAffectedElementHT()
'            Try
'                Dim myISEParamXmlDS As New DataSet

'                'TR 25/01/2011
'                myISEParamXmlDS = LoadISEModuleParammeters(Windows.Forms.Application.StartupPath.ToString() & _
'                                                                                GlobalBase.ISEParammetersFilePath)
'                'TR 25/01/2011
'                For Each iseRow As DataRow In myISEParamXmlDS.Tables("AffectedElementTable").Rows
'                    myAffectedElementsHT.Add(iseRow("id").ToString(), iseRow("AffecteElements").ToString())
'                Next

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.FillAffectedElementHT", EventLogEntryType.Error, False)
'            End Try

'        End Sub

'        ''' <summary>
'        ''' Fill the Hastable to store ISE module Errors.
'        ''' </summary>
'        ''' <remarks>CREATED BY: TR 12/12/2011</remarks>
'        Private Sub FillISEModuleErrorHT()
'            Try
'                Dim myISEParamXmlDS As New DataSet
'                myISEParamXmlDS = LoadISEModuleParammeters(Windows.Forms.Application.StartupPath.ToString() & _
'                                                                                GlobalBase.ISEParammetersFilePath)
'                For Each iseRow As DataRow In myISEParamXmlDS.Tables("ISEModuleErrors").Rows
'                    myISEModuleErrorHT.Add(iseRow("id").ToString(), iseRow("AffecteElements").ToString())
'                Next

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.FillISEModuleErrorHT", EventLogEntryType.Error, False)
'            End Try

'        End Sub

'#End Region

'#Region "Not Used"

'        ''' <summary>
'        ''' Method incharge to process the Remark (alarm)
'        ''' </summary>
'        ''' <param name="pError"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 05/01/2010
'        ''' </remarks>
'        Private Function ProcessISETESTErrors(ByVal pError As String) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim myISEErrorTO As New ISEErrorTO
'                Dim myISEErrorsList As New List(Of ISEErrorTO)
'                Dim Position As Integer = 1
'                For Each posValue As Char In pError
'                    'Start from position 1 because position 0 has the IseModule Error or independent errors
'                    If Position > 1 And posValue <> "0" Then
'                        myISEErrorTO = New ISEErrorTO

'                        myISEErrorTO.DigitNumber = Position
'                        myISEErrorTO.DigitValue = posValue
'                        myISEErrorTO.Message = "ISE_REMARK" & Position
'                        myISEErrorTO.Affected = GetAffectedElements(posValue)

'                        myISEErrorsList.Add(myISEErrorTO)
'                    ElseIf Position = 1 AndAlso Not posValue = "0" Then 'TR 12/12/2011 Valide if ISE module Error
'                        myISEErrorTO = New ISEErrorTO
'                        myISEErrorTO.DigitNumber = Position
'                        myISEErrorTO.DigitValue = posValue
'                        myISEErrorTO.Message = "ISE_REMARK" & posValue 'Set the position value
'                        myISEErrorTO.Affected = GetISEModuleError(posValue)
'                        myISEErrorsList.Add(myISEErrorTO)
'                        'If ISE module error then exit for 
'                        Exit For
'                    End If
'                    Position += 1
'                Next

'                myGlobalDataTO.SetDatos = myISEErrorsList

'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultsDelegate.ProcessISETESTErrors", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobalDataTO
'        End Function

'        'PENDING TO DEFINE
'        Private Function CheckChecksum(ByVal pISEResultStr As String) As Boolean
'            Dim isOK As Boolean = False
'            Try
'                Dim myStr As String = pISEResultStr.Trim.Replace("<", "")
'                myStr = myStr.Replace(">", "")
'                Dim myReceivedString As String = myStr.Substring(0, myStr.Length - 1)
'                Dim myReceivedResultValue As Integer = Asc(pISEResultStr.Substring(pISEResultStr.LastIndexOf(">") - 1, 1))
'                Dim myChecksumVal As Integer = 0
'                For c As Integer = 0 To pISEResultStr.Length - 1 Step 1
'                    myChecksumVal += Asc(pISEResultStr.Substring(c, 1))
'                Next

'                If myReceivedResultValue = myChecksumVal Then
'                    isOK = True
'                End If

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.CheckChecksum", EventLogEntryType.Error, False)
'            End Try
'            Return isOK
'        End Function

'        Private Function CalculateCRC() As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try

'                Dim myCRC As Integer




'                myGlobal.SetDatos = myCRC

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.CalculateCRC", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function
'#End Region

'    End Class

'End Namespace
