Option Explicit On
Option Strict On

'Imports System.IO
'Imports System.Text
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
'Imports Biosystems.Ax00.Global.TO


Namespace Biosystems.Ax00.BL

    Public Class OrderCalculatedTestsDelegate

#Region "Public Functions"
        ''' <summary>
        ''' Creates new OrderCalculatedTests rows
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderCalculatedTestsDS">Dataset with structure of table twksOrderCalculatedTests</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 05/14/2010
        ''' Modified by: SA 16/04/2012 - Open a DB Transaction instead a Connection; changed the function template
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderCalculatedTestsDS As OrderCalculatedTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.Create(dbConnection, pOrderCalculatedTestsDS)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete relations with Calculated Tests for all Order Tests included in the specified WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 17/05/2010
        ''' Modified by: SA 16/04/2012 - Open a DB Transaction instead a Connection; changed the function template
        ''' </remarks>
        Public Function DeleteByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.DeleteByWorkSession(dbConnection, pWorkSessionID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.DeleteByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes by Calc Order Test Id
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcOrderTestId"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 18/03/2012
        ''' </remarks>
        Public Function DeleteByCalOrderTestId(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestId As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.DeleteByCalOrderTestId(dbConnection, pCalcOrderTestId)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.DeleteByCalOrderTestId", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Order Tests requested that are contained in the Formula of Calculated Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcOrderTestID">Work Session Identifier</param>
        ''' <param name="pOnlyFormulaTests">True: Get all tests that form part of the calculted tests formula (parameter 'pOnlyTestsWithExecutionsFlag' is skipped // FALSE get ALL tests that form part of calculated test</param>
        ''' <param name="pOnlyTestsWithExecutionsFlag">TRUE get only the OT with executions (STD, ISE) that form the calculated test // FALSE get all OT that form the calculated test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderCalculatedTestsDS with the data obtained</returns>
        ''' <remarks>
        ''' Created by:  DL 25/09/2012
        ''' AG 04/04/2014 - #1576 add new parameters pOnlyFormulaTests, pWithExecutionsFlag
        ''' </remarks>
        Public Function GetByCalcOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestID As Integer, ByVal pOnlyFormulaTests As Boolean, ByVal pOnlyTestsWithExecutionsFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.GetOrderTestIDByCalcOrderTestID(dbConnection, pCalcOrderTestID, pOnlyFormulaTests, pOnlyTestsWithExecutionsFlag)

                        If Not pOnlyFormulaTests AndAlso Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim toReturnDS As New OrderCalculatedTestsDS
                            toReturnDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                            'Search others tests that form part of the calculated test matching condition: NOT pOnlyTestsWithExecutionsFlag
                            'For example: When calc test formula contains another calc test next code will return all tests that form part of this 2on calc test
                            '(in this case the entry parameters value must be: pOnlyFormulaTests = FALSE + pOnlyTestsWithExecutionsFlag = TRUE)
                            resultData = myDAO.GetOrderTestIDByCalcOrderTestID(dbConnection, pCalcOrderTestID, pOnlyFormulaTests, Not pOnlyTestsWithExecutionsFlag)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                For Each row As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In DirectCast(resultData.SetDatos, OrderCalculatedTestsDS).twksOrderCalculatedTests
                                    'Call this method in recursive mode
                                    resultData = GetByCalcOrderTestID(dbConnection, row.OrderTestID, pOnlyFormulaTests, pOnlyTestsWithExecutionsFlag)
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        'Add into dataset to return
                                        toReturnDS.twksOrderCalculatedTests.Merge(DirectCast(resultData.SetDatos, OrderCalculatedTestsDS).twksOrderCalculatedTests)

                                    ElseIf resultData.HasError Then
                                        Exit For
                                    End If
                                Next
                            End If

                            resultData.SetDatos = toReturnDS

                        End If

                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.GetByCalcOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Calculated Order Test related to the informed Order Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderCalculatedTestsDS with the data obtained</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.GetByOrderTestID(dbConnection, pOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.GetByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Patient OrderTest information of all members of the formula of the specified Calculated Test OrderTest
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcOrderTestID">Identifier of the OrderTest of a Calculated Test/SampleType</param>
        ''' <returns>GlobalDataTO containing a typed DS OrderTestsDS with all Patient OrderTest information for all members of the 
        '''          formula of the specified Calculated OrderTest</returns>
        ''' <remarks>
        ''' Created by:  SA 16/05/2013
        ''' </remarks>
        Public Function GetOTInfoByCalcOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.GetOTInfoByCalcOrderTestID(dbConnection, pCalcOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.GetOTInfoByCalcOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Patient Order Tests requested in a WorkSession that are contained in the Formula of Calculated Tests 
        ''' that have been also requested in the same WorkSession.  And for each one, return additionally the ID and Name 
        ''' of the Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderCalculatedTestsDS with the data obtained</returns>
        ''' <remarks>
        ''' Created by:  SA 18/05/2010
        ''' </remarks>
        Public Function ReadByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.ReadByWorkSession(dbConnection, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OrderCalculatedTestsDelegate.ReadByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all ordertests that form part of a CALCULATED TEST and has to be excluded from in the final patients / and compact patients reports
        ''' (apply only in current WS results)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo as list of integer with the OrderTestID that have to be excluded from patients final reports</returns>
        ''' <remarks>
        ''' AG 29/07/2014 - #1894 (tests that form part of a calculated test must be excluded from final report depends on the CALC test programming)
        ''' </remarks>
        Public Function GetOrderTestsToExcludeInPatientsReport(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO

                        'Get all order tests in worksession that form part of a calculated test (and also with PrintExpTests value programmed in calc tests)
                        resultData = myDAO.GetOrderTestsFormPartOfCalculatedTest(dbConnection)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myAuxDS As New OrderCalculatedTestsDS
                            myAuxDS = DirectCast(resultData.SetDatos, OrderCalculatedTestsDS)

                            Dim FinalToExcludeList As New List(Of Integer)
                            'Get ordertests that form part of a calculated test that exclude print their experimental tests
                            FinalToExcludeList = (From a As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myAuxDS.twksOrderCalculatedTests _
                                       Where a.PrintExpTests = False Select a.OrderTestID Distinct).ToList

                            'From FinalToExclude list remove those OrderTests that belongs to another calc test that prints their experimental tests
                            Dim toPrint As List(Of Integer)
                            toPrint = (From a As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myAuxDS.twksOrderCalculatedTests _
                                       Where a.PrintExpTests = True Select a.OrderTestID Distinct).ToList

                            'If the same ordertestID exists in toExclude and toPrint list we have to remove it from toExclude list
                            If toPrint.Count > 0 Then
                                Dim positionToDelete As Integer = 0
                                For Each item As Integer In toPrint
                                    If FinalToExcludeList.Contains(item) Then
                                        positionToDelete = FinalToExcludeList.IndexOf(item)
                                        FinalToExcludeList.RemoveAt(positionToDelete)
                                    End If
                                Next
                            End If

                            resultData.SetDatos = FinalToExcludeList

                            'Release memory
                            toPrint.Clear()
                            'FinalToExcludeList.Clear()
                            toPrint = Nothing
                            FinalToExcludeList = Nothing
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderCalculatedTestsDelegate.GetOrderTestsToExcludeInPatientsReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get all ordertests in worksession that form part of a calculated test
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 29/07/2014 - #1894 (tests that form part of a calculated test must be excluded from final report depends on the CALC test programming)
        ''' </remarks>
        Public Function GetOrderTestsFormPartOfCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksOrderCalculatedTestsDAO
                        resultData = myDAO.GetOrderTestsFormPartOfCalculatedTest(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OrderCalculatedTestsDelegate.GetOrderTestsFormPartOfCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace
