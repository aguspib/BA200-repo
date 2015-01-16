Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksOrderCalculatedTestsDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Creates new OrderCalculatedTests rows
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pOrderCalculatedTestsDS">Dataset with structure of table  twksOrderCalculatedTests</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 05/14/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pOrderCalculatedTestsDS As OrderCalculatedTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection

                    Dim cmdText As String = String.Empty
                    For Each row As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In pOrderCalculatedTestsDS.twksOrderCalculatedTests
                        cmdText = String.Format("INSERT INTO twksOrderCalculatedTests (OrderTestID, CalcOrderTestID) VALUES ({0}, {1})", _
                                                row.OrderTestID, row.CalcOrderTestID)

                        cmd.CommandText = cmdText
                        resultData.AffectedRecords = cmd.ExecuteNonQuery()

                        'If (resultData.AffectedRecords = 0) Then
                        '    resultData.HasError = True
                        '    Exit For
                        'End If
                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.Create", EventLogEntryType.Error, False)
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
        ''' Modified by: SA 16/04/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderCalculatedTests " & vbCrLf & _
                                            " WHERE  OrderTestID IN (SELECT OrderTestID FROM twksWSOrderTests " & vbCrLf & _
                                                                   " WHERE  WorkSessionID = '" & pWorkSessionID & "') " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.DeleteByWorkSession", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deletes by CalcOrderTestId
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcOrderTestId"></param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 18/03/2012
        ''' </remarks>
        Public Function DeleteByCalOrderTestId(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestId As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksOrderCalculatedTests " & vbCrLf & _
                                            " WHERE  CalcOrderTestID = '" & pCalcOrderTestId.ToString & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.DeleteByCalOrderTestId", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
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
        ''' Modified by: SA 16/04/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT DISTINCT OCT.OrderTestID, OCT.CalcOrderTestID, CT.CalcTestID, CT.CalcTestLongName, OTL.SpecimenID " & vbCrLf
                        cmdText &= "           FROM twksOrderCalculatedTests OCT INNER JOIN twksOrderTests OT ON OCT.CalcOrderTestID = OT.OrderTestID " & vbCrLf
                        cmdText &= "                                             INNER JOIN tparCalculatedTests CT ON OT.TestID = CT.CalcTestID " & vbCrLf
                        cmdText &= "                                             LEFT OUTER JOIN twksOrderTestsLISInfo OTL ON OCT.CalcOrderTestID = OTL.OrderTestID" & vbCrLf
                        cmdText &= "          WHERE OCT.OrderTestID IN (SELECT OrderTestID " & vbCrLf
                        cmdText &= "                                      FROM twksWSOrderTests " & vbCrLf
                        cmdText &= "                                     WHERE WorkSessionID = '" & pWorkSessionID & "') " & vbCrLf
                        cmdText &= "            AND OT.TestType = 'CALC' " & vbCrLf
                        cmdText &= "ORDER BY OCT.OrderTestID "

                        Dim calcOrderTestsDS As New OrderCalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calcOrderTestsDS.twksOrderCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = calcOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.ReadByWorkSession", EventLogEntryType.Error, False)
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
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.*, OTLI.AwosID " & vbCrLf & _
                                                " FROM   twksOrderCalculatedTests OCT INNER JOIN twksOrderTests OT ON OCT.OrderTestID = OT.OrderTestID " & vbCrLf & _
                                                                               " LEFT OUTER JOIN twksOrderTestsLISInfo OTLI ON OT.OrderTestID = OTLI.OrderTestID " & vbCrLf & _
                                                " WHERE  OCT.CalcOrderTestID = " & pCalcOrderTestID.ToString & vbCrLf

                        Dim myOrderTestsDS As New OrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOrderTestsDS.twksOrderTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.GetOTInfoByCalcOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get all Order Tests of the Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcOrderTestID">Work Session Identifier</param>
        ''' <param name="pOnlyFormulaTests">True: Get all tests that form part of the calculted tests formula (parameter 'pOnlyTestsWithExecutionsFlag' is skipped // FALSE get ALL tests that form part of calculated test</param>
        ''' <param name="pOnlyTestsWithExecutionsFlag">TRUE get only the OT with executions (STD, ISE) that form the calculated test // FALSE get all OT that form the calculated test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderCalculatedTestsDS with the data obtained</returns>
        ''' <remarks>
        ''' Created by:  DL 25/09/2012
        ''' AG 04/04/2014 - #1576 add new parameters pAllTestsFlag, pWithExecutionsFlag and change query
        ''' </remarks>
        Public Function GetOrderTestIDByCalcOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcOrderTestID As Integer, ByVal pOnlyFormulaTests As Boolean, ByVal pOnlyTestsWithExecutionsFlag As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'All tests that form part of the calculated test formula
                        cmdText &= "SELECT OrderTestID " & vbCrLf
                        cmdText &= "  FROM twksOrderCalculatedTests" & vbCrLf
                        cmdText &= " WHERE CalcOrderTestID = " & pCalcOrderTestID

                        'AG 04/04/2014 - #1576 - If new parameter TRUE remove old query and execute new one
                        If Not pOnlyFormulaTests Then
                            'Partial tests that form part of the calculated test
                            cmdText = ""
                            cmdText &= " SELECT OCT.OrderTestID " & vbCrLf
                            cmdText &= "  FROM twksOrderCalculatedTests OCT INNER JOIN twksOrderTests OT ON  OCT.OrderTestID = OT.OrderTestID " & vbCrLf
                            cmdText &= " WHERE OCT.CalcOrderTestID = " & pCalcOrderTestID

                            If pOnlyTestsWithExecutionsFlag Then
                                'Only partial tests with executions
                                cmdText &= " AND (OT.TestType = 'STD' OR OT.TestType = 'ISE') "

                            Else
                                'Only partial tests with NO executions
                                cmdText &= " AND (OT.TestType = 'CALC' OR OT.TestType = 'OFFS') "
                            End If

                        End If
                        'AG 04/04/2014 - #1576

                        Dim calcOrderTestsDS As New OrderCalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calcOrderTestsDS.twksOrderCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = calcOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.GetOrderTestIDByCalcOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Calculated Order Test related to the informed Order Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">order test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OrderCalculatedTestsDS with the data obtained</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function GetByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalcOrderTestID, OrderTestID " & vbCrLf
                        cmdText &= "  FROM twksOrderCalculatedTests" & vbCrLf
                        cmdText &= " WHERE OrderTestID = " & pOrderTestID

                        Dim calcOrderTestsDS As New OrderCalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calcOrderTestsDS.twksOrderCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = calcOrderTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksOrderCalculatedTestsDAO.GetByOrderTestID", EventLogEntryType.Error, False)
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
        ''' AG 13/10/2014 - BA-2006 also return CalcOrderTestID
        ''' </remarks>
        Public Function GetOrderTestsFormPartOfCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText = " SELECT OCT.OrderTestID, OCT.CalcOrderTestID ,CT.CalcTestID , ct.PrintExpTests  " & vbCrLf
                        cmdText &= " FROM   twksOrderTests OT INNER JOIN tparCalculatedTests CT ON OT.TestID = CT.CalcTestID " & vbCrLf
                        cmdText &= " INNER JOIN twksOrderCalculatedTests OCT ON OT.OrderTestID = OCT.CalcOrderTestID  " & vbCrLf
                        cmdText &= " WHERE  OT.TestType = 'CALC'  " & vbCrLf

                        Dim myDataSet As New OrderCalculatedTestsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksOrderCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "twksOrderCalculatedTestsDAO.GetOrderTestsFormPartOfCalculatedTest", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

    End Class

End Namespace
