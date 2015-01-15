Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSPausedOrderTestsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Insert paused OrderTests in table twksWSPausedOrderTests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSPausedOrderTest">Typed DataSet WSPausedOrderTestsDS containing the list of paused OrderTests</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 25/07/2011
        ''' AG 20/03/2014 - #1545 (allow multiple rows insert in a unique string
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSPausedOrderTest As WSPausedOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()

                ElseIf (Not pWSPausedOrderTest Is Nothing) Then
                    Dim i As Integer = 0
                    Dim cmdText As String = ""

                    For Each PausedOrderTestRow As WSPausedOrderTestsDS.twksWSPausedOrderTestsRow In pWSPausedOrderTest.twksWSPausedOrderTests.Rows
                        'AG 20/03/2014 - #1545
                        'cmdText = " INSERT INTO twksWSPausedOrderTests (AnalyzerID, WorkSessionID, OrderTestID, RerunNumber) " & Environment.NewLine
                        cmdText &= " INSERT INTO twksWSPausedOrderTests (AnalyzerID, WorkSessionID, OrderTestID, RerunNumber) " & Environment.NewLine

                        cmdText &= " VALUES ( "
                        cmdText &= "'" & PausedOrderTestRow.AnalyzerID & "', '" & PausedOrderTestRow.WorkSessionID & "', " & Environment.NewLine
                        cmdText &= "'" & PausedOrderTestRow.OrderTestID & "', " & PausedOrderTestRow.RerunNumber & ") " & Environment.NewLine
                    Next

                    Dim dbCmd As New SqlClient.SqlCommand()
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                    If (Not myGlobalDataTO.AffectedRecords > 0) Then myGlobalDataTO.HasError = True
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Read all paused Order Tests by Analyzer ID, WorkSessionID, OrderTestID and RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPausedOrderTestsDS</returns>
        ''' <remarks>
        ''' Created by:  TR 25/07/2011
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSPausedOrderTests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim() & "' " & vbCrLf & _
                                                " AND    OrderTestID   = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber   = " & pRerunNumber.ToString & vbCrLf

                        Dim myWSPausedOrderTestDS As New WSPausedOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myWSPausedOrderTestDS.twksWSPausedOrderTests)
                        End Using

                        myGlobalDataTO.SetDatos = myWSPausedOrderTestDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read all paused Order Tests by AnalyzerID and WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSPausedOrderTestsDS</returns>
        ''' <remarks>
        ''' Created by:  TR 26/07/2011
        ''' </remarks>
        Public Function ReadByAnalyzerAndWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSPausedOrderTests " & vbCrLf & _
                                                " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim() & "' " & vbCrLf

                        Dim myWSPausedOrderTestDS As New WSPausedOrderTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSPausedOrderTestDS.twksWSPausedOrderTests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSPausedOrderTestDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.ReadByAnalyzerAndWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the paused Order Test by AnalyzerID, WorkSessionID, OrderTestID and RerunNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 25/07/2011
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                               ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSPausedOrderTests " & vbCrLf & _
                                            " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim() & "' " & vbCrLf & _
                                            " AND    OrderTestID   = " & pOrderTestID.ToString & vbCrLf & _
                                            " AND    RerunNumber   = " & pRerunNumber.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all paused Order Tests by Analyzer and WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 25/07/2011
        ''' </remarks>
        Public Function DeleteByAnalyzerIDAndWorkSessionID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                           ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSPausedOrderTests " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim() & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.DeleteByAnalyzerIDAndWorkSessionID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Delete the paused Order Test by AnalyzerID, WorkSessionID, OrderTestID list and RerunNumber list
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 20/03/2014 - #1545
        ''' </remarks>
        Public Function DeleteList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                       ByVal pOrderTestID As List(Of Integer), ByVal pRerunNumber As List(Of Integer)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim auxText As String = ""

                    For i As Integer = 0 To pOrderTestID.Count - 1
                        If pOrderTestID.Count > 1 Then
                            If i < pOrderTestID.Count - 1 Then
                                auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) OR "
                            Else
                                auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) "
                            End If

                        Else
                            auxText &= " (OrderTestID = " & pOrderTestID(i) & " AND RerunNumber = " & pRerunNumber(i) & " ) "
                        End If
                    Next i

                    If auxText.Length > 0 Then
                        Dim cmdText As String = " DELETE twksWSPausedOrderTests " & vbCrLf & _
                            " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                            " AND    WorkSessionID = '" & pWorkSessionID.Trim() & "' " & vbCrLf & _
                            " AND ( " & auxText & " ) "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                            resultData.HasError = False
                        End Using

                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSPausedOrderTestsDAO.DeleteList", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


#End Region
    End Class
End Namespace

