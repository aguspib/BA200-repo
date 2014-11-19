Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.Calculations

    Public Class OperateCalculatedTestDelegate

#Region "Declarations"
        Private TestsResults As New Dictionary(Of String, Double)
        Private AverageResultsDS As New ResultsDS

        Private myAnalyzerModel As String 'SGM 04/03/11
#End Region

#Region "Attributes"
        'AG 25/06/2012
        Private AnalyzerIDAttribute As String = String.Empty
        Private WorkSessionIDAttribute As String = String.Empty
#End Region

#Region "Constructor"
        'SGM 04/03/11
        Public Sub New(Optional ByVal pAnalyzerModel As String = "")
            myAnalyzerModel = pAnalyzerModel
        End Sub
#End Region

#Region "Properties"
        'AG 25/06/2012
        Public WriteOnly Property AnalyzerID() As String
            Set(ByVal value As String)
                AnalyzerIDAttribute = value
            End Set
        End Property

        Public WriteOnly Property WorkSessionID() As String
            Set(ByVal value As String)
                WorkSessionIDAttribute = value
            End Set
        End Property

#End Region

#Region "Public methods"
        ''' <summary>
        ''' Verify if the informed OrderTestID is related with one or more requested Calculated Tests and in that case,
        ''' check if it is possible to calculate the result for each one of them. All calculated Results are saved and 
        ''' the status of the OrderTests of the Calculated Tests are set to CLOSED
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pManualRecalculation">When TRUE, it indicates the function has been called after modifying a Calibrator Manual Factor</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 05/13/2010
        ''' Modified by: AG 10/09/2010 - Added parameter ManualRecalculation
        '''              SA 20/04/2012 - Function rewritten; changed the key part of the values added to dictionary:  it will 
        '''                              be TestType|TestID instead of only TestID (to avoid a duplicated key error when two Tests 
        '''                              of different type but with the same ID are included in the Formula of a Calculated Test)   
        '''              SA 09/05/2012 - Clear DataSet ResultsDS before adding a new Calculated Test Result (to solve error raised when 
        '''                              the specified OrderTestID corresponds to a Test included in more than one of the Calculated Test
        '''                              requested in the WorkSession
        '''              SA 29/06/2012 - Open DB Transaction instead a DB Connection
        '''              SA 26/07/2012 - Inform field SampleClass = PATIENT when the result of the Calculated Test is saved
        '''              SA 01/10/2012 - When there is not an accepted and validated Result for at least one of the Test included in the Formula 
        '''                              of the Calculated Test, if there was a result calculated previously for the Calculated Test, delete it 
        '''                              (along with the corresponding Result Alarms) and call the function recursively to delete also results
        '''                              of all Order Tests of another Calculated Tests in which Formula the Calculated Test in process is included
        '''              AG 25/02/2014 - BT #1521 ==> Set all Linqs to Nothing
        '''              AG 30/07/2014 - #1887 On CTRL or PATIENT recalculations set OrderToExport = TRUE
        '''              SA 19/09/2014 - BA-1927 ==> When calling function UpdateOrderToExport in OrdersDelegate, pass the local DB Connection instead 
        '''                                          of the received as parameter (to avoid timeouts)
        '''              AG 16/10/2014 - BA-2011 ==> Updated properly the OrderToExport field when the recalculated result is an accepted one
        '''              SA 19/11/2014 - BA-979  ==> Changed the key part of the values added to dictionary:  it will include also the SampleType (besides 
        '''                                          TestType and TestID) to allow evaluation of Calculated Tests having the same Test (with different 
        '''                                          SampleType) in the Formula of a Calculated Test
        ''' </remarks> 
        Public Function ExecuteCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                              ByVal pManualRecalculation As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim relatedStandardTestHasRemark As Boolean = False

                        If (AverageResultsDS.vwksResultsAlarms.Rows.Count = 0) Then 'It is empty, so we load it
                            'Get Average Result Alarms
                            Dim myResultsDelegate As New ResultsDelegate
                            resultData = myResultsDelegate.GetResultAlarms(dbConnection)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                For Each resultRow As ResultsDS.vwksResultsAlarmsRow In DirectCast(resultData.SetDatos, ResultsDS).vwksResultsAlarms.Rows
                                    AverageResultsDS.vwksResultsAlarms.ImportRow(resultRow)
                                Next
                            End If
                        End If

                        TestsResults.Clear()
                        If (Not resultData.HasError) Then
                            'Verify if the specified OrderTest corresponds to a Test included in the Formula of a requested Calculated Test
                            Dim orderCalculatedTests As New ViewOrderCalculatedTestsDelegate()

                            resultData = orderCalculatedTests.ReadByOrderTest(dbConnection, pOrderTestID)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim requiredOrderCalculatedTests As ViewOrderCalculatedTestsDS = DirectCast(resultData.SetDatos, ViewOrderCalculatedTestsDS)

                                If (requiredOrderCalculatedTests.vwksOrderCalculatedTests.Rows.Count > 0) Then
                                    'Get the logged User
                                    Dim currentSession As New GlobalBase
                                    Dim loggedUser As String = currentSession.GetSessionInfo.UserName

                                    Dim orderTestsDS As New ViewOrderCalculatedTestsDS
                                    Dim operateCalculatedTestDS As New OperateCalculatedTestDS

                                    Dim testResult As Double = 0
                                    Dim concErrorCode As String = String.Empty

                                    Dim testFormulaDS As New FormulasDS
                                    Dim formulaDelegate As New FormulasDelegate

                                    Dim activeRangeType As String = String.Empty
                                    Dim myCalculatedTestsDS As New CalculatedTestsDS
                                    Dim myCalculatedTestsDelegate As New CalculatedTestsDelegate

                                    Dim stdOtNotClosed As Integer = 0
                                    Dim myOrderTestDelegate As New OrderTestsDelegate

                                    Dim resultDS As New ResultsDS
                                    Dim resultsDelegate As New ResultsDelegate()
                                    Dim resultRow As ResultsDS.twksResultsRow = resultDS.twksResults.NewtwksResultsRow()

                                    For Each orderCalculatedTestsRow1 As ViewOrderCalculatedTestsDS.vwksOrderCalculatedTestsRow In requiredOrderCalculatedTests.vwksOrderCalculatedTests.Rows
                                        Dim allComponentTestsHaveResults As Boolean = True 'AG 01/10/2012

                                        'Search the OrderTestID of all Tests included in the Calculated Test
                                        resultData = orderCalculatedTests.ReadByCalcOrderTest(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            orderTestsDS = DirectCast(resultData.SetDatos, ViewOrderCalculatedTestsDS)

                                            For Each orderCalculatedTestRow2 As ViewOrderCalculatedTestsDS.vwksOrderCalculatedTestsRow In orderTestsDS.vwksOrderCalculatedTests.Rows
                                                'Count not closed related Order Tests
                                                If (orderCalculatedTestRow2.OrderTestStatus <> "CLOSED") Then stdOtNotClosed += 1

                                                'Get the accepted and validated Result for the component Test
                                                resultData = GetAcceptedResultByOrderTest(dbConnection, orderCalculatedTestRow2.OrderTestID)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    operateCalculatedTestDS = DirectCast(resultData.SetDatos, OperateCalculatedTestDS)

                                                    If (operateCalculatedTestDS.vwksOperateCalculatedTest.Count > 0) Then
                                                        'Insert into the diccionary the pairs <TestType|TestID|SampleType, Result> for evaluating the formula ahead
                                                        'BA-979: SampleType has been added as part of the key along with TestType and TestID
                                                        With (operateCalculatedTestDS.vwksOperateCalculatedTest(0))
                                                            TestsResults.Add(.TestType & "|" & .TestID.ToString() & "|" & .SampleType, .Result)
                                                        End With

                                                        If (Not relatedStandardTestHasRemark) Then
                                                            Dim myDescriptions As List(Of String)
                                                            With operateCalculatedTestDS.vwksOperateCalculatedTest(0)
                                                                myDescriptions = (From row In AverageResultsDS.vwksResultsAlarms _
                                                                                 Where row.OrderTestID = .OrderTestID _
                                                                               AndAlso row.AcceptedResultFlag = True _
                                                                                Select row.Description Distinct).ToList()
                                                            End With
                                                            relatedStandardTestHasRemark = (myDescriptions.Count > 0)
                                                            myDescriptions = Nothing 'AG 25/02/2014 - #1521
                                                        End If
                                                    Else
                                                        'If there is a previous Result for the Calculated Test, delete its Alarms
                                                        Dim myResultAlarmsDelegate As New ResultAlarmsDelegate
                                                        resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID, 1, 1)
                                                        If (resultData.HasError) Then Exit For

                                                        'If there is a previous Result for the Calculated Test, delete it
                                                        resultData = resultsDelegate.DeleteByOrderTestID(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID)
                                                        If (resultData.HasError) Then Exit For

                                                        'If the Order Test for the Calculated Test is linked to another requested Calculated Tests, call this same
                                                        'function to evaluate them
                                                        resultData = ExecuteCalculatedTest(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID, pManualRecalculation)
                                                        If (resultData.HasError) Then Exit For

                                                        'There is no Accepted Result for the Order Test: get the result of the Calculated Test is not possible
                                                        resultData.HasError = False
                                                        allComponentTestsHaveResults = False
                                                        Exit For
                                                    End If
                                                Else
                                                    'Error getting the accepted and validated Result for the component Test
                                                    Exit For
                                                End If
                                            Next

                                            If (Not resultData.HasError AndAlso allComponentTestsHaveResults) Then
                                                'Get the Formula defined for the Calculated Test
                                                resultData = formulaDelegate.GetFormulaValues(dbConnection, orderCalculatedTestsRow1.CalcTestID)
                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    testFormulaDS = DirectCast(resultData.SetDatos, FormulasDS)

                                                    'Prepare and execute the Formula to get the Result
                                                    resultData = Evaluate(dbConnection, testFormulaDS)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        'Get the calculated Result and the Concentration Error code
                                                        testResult = CDbl(resultData.SetDatos)
                                                        concErrorCode = resultData.ErrorCode

                                                        'Add the calculated result to a ResultsDS DataSet
                                                        resultDS.Clear()
                                                        resultRow.OrderTestID = orderCalculatedTestsRow1.CalcOrderTestID
                                                        resultRow.SampleClass = "PATIENT"
                                                        resultRow.CONC_Value = CSng(testResult)
                                                        resultRow.AcceptedResultFlag = True
                                                        resultRow.MultiPointNumber = 1
                                                        resultRow.RerunNumber = 1
                                                        resultRow.ValidationStatus = "OK"
                                                        resultRow.ManualResultFlag = False
                                                        resultRow.TestVersion = 0
                                                        resultRow.CONC_Error = concErrorCode

                                                        resultRow.ExportStatus = "NOTSENT"
                                                        If (pManualRecalculation) Then
                                                            'Recalculate the Export Status value
                                                            resultData = resultsDelegate.RecalculateExportStatusValue(dbConnection, resultRow.OrderTestID, resultRow.RerunNumber)
                                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                resultRow.ExportStatus = CType(resultData.SetDatos, String)
                                                            Else
                                                                'Error recalculating the Export Status
                                                                Exit For
                                                            End If
                                                        End If

                                                        resultRow.Printed = False
                                                        resultRow.ResultDateTime = DateTime.Now
                                                        resultRow.TS_User = loggedUser
                                                        resultRow.TS_DateTime = DateTime.Now
                                                        resultRow.AnalyzerID = AnalyzerIDAttribute
                                                        resultRow.WorkSessionID = WorkSessionIDAttribute
                                                        resultDS.twksResults.AddtwksResultsRow(resultRow)

                                                        'Save the calculated Result; if an error happens, the loop is finished
                                                        resultData = resultsDelegate.SaveResults(dbConnection, resultDS)

                                                        If (resultData.HasError) Then Exit For

                                                        'AG 16/10/2014 BA-2011 - Update properly the OrderToExport field after save new Calc test result (always accepted)
                                                        If Not resultData.HasError Then
                                                            Dim orders_dlg As New OrdersDelegate
                                                            resultData = orders_dlg.SetNewOrderToExportValue(dbConnection, , resultRow.OrderTestID)
                                                        End If
                                                        'AG 16/10/2014 BA-2011

                                                        'Update the OrderTestStatus of the requested Calculated Test (if all the OrderTestID related are closed,
                                                        'then the Calculated OrderTestID becomes CLOSED too
                                                        If (orderCalculatedTestsRow1.CalcOrderTestStatus <> "CLOSED") Then
                                                            If (stdOtNotClosed = 0) Then
                                                                'All related OrderTestIDs are CLOSED, the OrderTestID for the Calculated Test is also CLOSED
                                                                resultData = myOrderTestDelegate.UpdateStatusByOrderTestID(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID, "CLOSED")
                                                                If (resultData.HasError) Then Exit For
                                                            End If
                                                        End If

                                                        'Verify if there are Reference Ranges defined for the Calculated Tests
                                                        resultData = myCalculatedTestsDelegate.GetCalcTest(dbConnection, orderCalculatedTestsRow1.CalcTestID)
                                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                            myCalculatedTestsDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                                                            If (myCalculatedTestsDS.tparCalculatedTests.Rows.Count > 0 AndAlso _
                                                                Not myCalculatedTestsDS.tparCalculatedTests.First.IsActiveRangeTypeNull) Then
                                                                activeRangeType = myCalculatedTestsDS.tparCalculatedTests.First.ActiveRangeType
                                                            Else
                                                                activeRangeType = String.Empty
                                                            End If

                                                            'Add all alarms generated for the Calculated Tests to a ResultsAlarmsDS
                                                            CreateAlarms(dbConnection, relatedStandardTestHasRemark, resultRow, activeRangeType, orderCalculatedTestsRow1.CalcTestID)

                                                            'If the Order Test for the Calculated Test is linked to another requested Calculated TestsResults, call this same
                                                            'function to evaluate them
                                                            resultData = ExecuteCalculatedTest(dbConnection, orderCalculatedTestsRow1.CalcOrderTestID, pManualRecalculation)
                                                            If (resultData.HasError) Then Exit For
                                                        Else
                                                            'Error getting data of the Calculated Test
                                                            Exit For
                                                        End If
                                                    Else
                                                        'Error evaluating the Formula
                                                        Exit For
                                                    End If
                                                Else
                                                    'Error getting the Formula defined for the Calculated Test
                                                    Exit For
                                                End If

                                            ElseIf (resultData.HasError) Then
                                                'BA-979: Exit For only when there was an error. When allComponentTestsHaveResults = False, evaluate 
                                                '        if the rest of Calculated Tests in which the Test is included can be calculated (otherwise some Calculated 
                                                '        Tests can be never calculated!!)
                                                Exit For
                                            End If
                                        Else
                                            'Error getting the list of OrderTests included in the Formula of the Calculated Test
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Transaction was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Transaction was opened locally, then it is undone (Rollback is executed)
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Transacction was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDelegate.ExecuteCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private methods"

        ''' <summary>
        ''' Add a row into a ResultsAlarmsDS DataSet
        ''' </summary>
        ''' <param name="pResultAlarmsDS">Typed DataSet ResultAlarmsDS to which the new row will be added</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Number of last Rerun</param>
        ''' <param name="pMultiPointNumber">Multipoint Number</param>
        ''' <param name="pAlarmID">Alarm Identifier</param>
        ''' <remarks>
        ''' Created by:  RH 09/14/2010
        ''' </remarks>
        Private Sub AddResultAlarm(ByRef pResultAlarmsDS As ResultAlarmsDS, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, _
                                   ByVal pMultiPointNumber As Integer, ByVal pAlarmID As String)
            Try
                'Add a row with the Alarm in the Dataset
                Dim resultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow

                resultAlarmRow = pResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                With (resultAlarmRow)
                    .OrderTestID = pOrderTestID
                    .RerunNumber = pRerunNumber
                    .MultiPointNumber = pMultiPointNumber
                    .AlarmID = pAlarmID
                    .AlarmDateTime = Now
                End With
                pResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(resultAlarmRow)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDelegate.AddResultAlarm", EventLogEntryType.Error, False)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Validates calculated results an emits alarms if needed
        ''' </summary>
        ''' <param name="dbConnection">Open DB Connection</param>
        ''' <param name="pRelatedStandardTestHasRemark"></param>
        ''' <param name="pResultRow"></param>
        ''' <param name="pActiveRangeType"></param>
        ''' <param name="pCalcTestID"></param>
        ''' <remarks>
        ''' Created by:  RH 09/14/2010
        ''' Modified by: SA 25/01/2011 - Changed the way of getting the Reference Range Interval
        '''              SA 05/06/2014 - BT #1659 ==> When CONC_Value is greater than the Max Range, save CONC_REMARK8 instead of CONC_REMARK7
        '''                                           Previous result Alarms have to be deleted always, not only when new Alarms have been found
        ''' </remarks>
        Private Sub CreateAlarms(ByVal dbConnection As SqlClient.SqlConnection, ByVal pRelatedStandardTestHasRemark As Boolean, _
                                 ByVal pResultRow As ResultsDS.twksResultsRow, ByVal pActiveRangeType As String, ByVal pCalcTestID As Integer)
            Try
                Dim resultData As GlobalDataTO
                Dim myResultAlarmsDS As New ResultAlarmsDS

                If (pResultRow.CONC_Error = GlobalEnumerates.ConcentrationErrors.OUT.ToString()) Then
                    'Conc NOT calculated
                    AddResultAlarm(myResultAlarmsDS, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber, _
                                   GlobalEnumerates.CalculationRemarks.CONC_REMARK1.ToString)
                ElseIf (pResultRow.CONC_Error = String.Empty AndAlso pResultRow.CONC_Value < 0) Then
                    'Conc < 0
                    AddResultAlarm(myResultAlarmsDS, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber, _
                                   GlobalEnumerates.CalculationRemarks.CONC_REMARK4.ToString)
                End If

                'Read Reference Range Limits
                Dim minimunValue As Nullable(Of Single) = Nothing
                Dim maximunValue As Nullable(Of Single) = Nothing

                If (Not String.IsNullOrEmpty(pActiveRangeType)) Then
                    'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    resultData = myOrderTestsDelegate.GetReferenceRangeInterval(dbConnection, pResultRow.OrderTestID, "CALC", pCalcTestID, "", pActiveRangeType)

                    If (Not resultData.HasError AndAlso Not resultData.HasError) Then
                        Dim myTestRefRangesDS As TestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                        If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                            minimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                            maximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit
                        End If
                    End If
                End If

                If (minimunValue.HasValue AndAlso maximunValue.HasValue) Then
                    If (minimunValue <> -1 AndAlso maximunValue <> -1) Then
                        'BT #1659 - Save CONC_REMARK7 if CONC_Value < Min Range and CONC_REMARK8 if CONC_Value > Max Range
                        If (pResultRow.CONC_Value < minimunValue.Value) Then
                            AddResultAlarm(myResultAlarmsDS, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber, _
                                           GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString)

                        ElseIf (pResultRow.CONC_Value > maximunValue.Value) Then
                            AddResultAlarm(myResultAlarmsDS, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber, _
                                           GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString)
                        End If
                    End If
                End If

                If (pRelatedStandardTestHasRemark) Then
                    'Some tests with remarks
                    AddResultAlarm(myResultAlarmsDS, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber, _
                                   GlobalEnumerates.CalculationRemarks.CONC_REMARK11.ToString)
                End If

                'BT #1659- Delete all previous Alarms always, not only when new alarms have been found
                Dim myResultAlarmsDelegate As New ResultAlarmsDelegate
                resultData = myResultAlarmsDelegate.DeleteAll(dbConnection, pResultRow.OrderTestID, pResultRow.RerunNumber, pResultRow.MultiPointNumber)

                '... and finally save the generated Alarms
                If (myResultAlarmsDS.twksResultAlarms.Rows.Count > 0) Then resultData = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDelegate.CreateAlarms", EventLogEntryType.Error, False)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' For the specified OrderTestID, search if there is an accepted and validated Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OperateCalculatedTestDS containing the TestType, TestID, SampleType
        '''          and the accepted and validated Result for the specified OrderTestID
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 13/05/2010
        ''' Modified by: SA 20/04/2012 - Changed the function template
        ''' </remarks>
        Private Function GetAcceptedResultByOrderTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim operateCalculatedTestDAO As New vwksOperateCalculatedTestDAO
                        resultData = operateCalculatedTestDAO.Read(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDelegate.GetAcceptedResultsByOrderTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Receive the all members of the Formula defined for a Calculated Test, replace each TestID for its Result and build an string 
        ''' that is sent to SQL to calculate the Result
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pFormulaDS">Typed DataSet FormulaDS containing all members of the Formula defined for a Calculated Test</param>
        ''' <returns>GlobalDataTO containing a double value with the Result calculated after applied the Formula for the Test
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 18/05/2010
        ''' Modified by: SA 20/04/2012
        '''              SA 19/11/2014 - BA-979 ==> Changes due to field TestTypeAndID has been replaced by field TestTypeTestIDSampleType 
        '''                                         in typed DataSet FormulasDS. Besides, use field ValueType (instead of TestType) to verify 
        '''                                         the type of value to append to the Expression to evaluate (this change allows evaluation 
        '''                                         of Calculated Tests with ISE and OFFS Tests in their Formula)
        ''' </remarks>
        Private Function Evaluate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pFormulaDS As FormulasDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim Expression As New System.Text.StringBuilder
                        For Each row As FormulasDS.tparFormulasRow In pFormulaDS.tparFormulas
                            Select Case row.ValueType
                                Case "TEST"
                                    Expression.Append("(" & TestsResults(row.TestTypeTestIDSampleType).ToSQLString() & ")")
                                Case Else
                                    Expression.Append(row.Value)
                            End Select
                        Next

                        Dim operateCalculatedTestDAO As New vwksOperateCalculatedTestDAO
                        resultData = operateCalculatedTestDAO.Evaluate(dbConnection, Expression.ToString())
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OperateCalculatedTestDelegate.Evaluate", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
