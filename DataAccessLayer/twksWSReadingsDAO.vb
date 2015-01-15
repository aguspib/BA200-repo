Option Strict On
Option Explicit On

Imports System.Text
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class twksWSReadingsDAO
        Inherits DAOBase

#Region "CRUD"
        ''' <summary>
        ''' Get data from twksWSReadings filter by executionid
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 24/02/2010
        ''' Modified by: AG 25/02/2010 (add reactioncomplete parameter) Tested OK
        '''              TR 03/03/2011 (add LedPosition column)
        ''' </remarks>
        Public Function GetReadingsByExecutionID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pAnalyzerID As String, _
                                                 ByVal pWorkSessionID As String, _
                                                 ByVal pExecutionID As Integer, _
                                                 ByVal pReactionComplete As Boolean) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Dim Cmd As New SqlClient.SqlCommand
                        Dim cmdText As String = ""

                        cmdText = "SELECT AnalyzerID" & vbCrLf & _
                                        ", WorkSessionID" & vbCrLf & _
                                        ", ExecutionID" & vbCrLf & _
                                        ", ReactionComplete" & vbCrLf & _
                                        ", ReadingNumber" & vbCrLf & _
                                        ", LedPosition" & vbCrLf & _
                                        ", MainCounts" & vbCrLf & _
                                        ", RefCounts" & vbCrLf & _
                                        ", Datetime" & vbCrLf & _
                                        ", Pause" & vbCrLf & _
                                  "  FROM twksWSReadings " & vbCrLf & _
                                  "  WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf & _
                                  "    AND WorkSessionID = '" & pWorkSessionID & "'" & vbCrLf & _
                                  "    AND ExecutionID = " & pExecutionID & vbCrLf & _
                                  "    AND ReactionComplete = '" & pReactionComplete & "'"

                        'Cmd.CommandText = cmdText
                        'Cmd.Connection = dbConnection

                        'Dim da As New SqlClient.SqlDataAdapter(Cmd)
                        'Dim mytwksWSReadingsDS As New twksWSReadingsDS

                        'da.Fill(mytwksWSReadingsDS.twksWSReadings)
                        Dim mytwksWSReadingsDS As New twksWSReadingsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(mytwksWSReadingsDS.twksWSReadings)
                            End Using
                        End Using

                        resultData.SetDatos = mytwksWSReadingsDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.GetReadingsByExecutionID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="ptwksWSReadingsRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: ¿?
        ''' Modified  : TR 03/03/2011 -Add the led position column.
        '''           : TR 10/10/2013 -Add pause column.
        ''' </remarks>
        Public Function Insert(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal ptwksWSReadingsRow As twksWSReadingsDS.twksWSReadingsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String

                    cmdText = "INSERT INTO twksWSReadings " & vbCrLf & _
                              "( AnalyzerID" & vbCrLf & _
                              ", WorkSessionID" & vbCrLf & _
                              ", ExecutionID" & vbCrLf & _
                              ", ReactionComplete" & vbCrLf & _
                              ", ReadingNumber" & vbCrLf & _
                              ", LedPosition" & vbCrLf & _
                              ", MainCounts" & vbCrLf & _
                              ", RefCounts" & vbCrLf & _
                              ", DateTime" & vbCrLf & _
                              ", Pause)" & vbCrLf & _
                              " VALUES " & vbCrLf & _
                              "( '" & ptwksWSReadingsRow.AnalyzerID.ToString & "'" & vbCrLf & _
                              ", '" & ptwksWSReadingsRow.WorkSessionID.ToString & "'" & vbCrLf & _
                              ", " & ptwksWSReadingsRow.ExecutionID.ToString & vbCrLf & _
                              ", '" & ptwksWSReadingsRow.ReactionComplete.ToString & "'" & vbCrLf & _
                              ", " & ptwksWSReadingsRow.ReadingNumber.ToString & vbCrLf


                    'TR 03/03/2011 -Add the led position value
                    If ptwksWSReadingsRow.IsLedPositionNull Then
                        cmdText += ", 0" & vbCrLf
                    Else
                        cmdText += ", " & ptwksWSReadingsRow.LedPosition.ToString() & vbCrLf
                    End If
                    'TR 03/03/2011 -END

                    If ptwksWSReadingsRow.IsMainCountsNull Then
                        cmdText += ", 0" & vbCrLf
                    Else
                        cmdText += ", " & ptwksWSReadingsRow.MainCounts.ToString & vbCrLf
                    End If

                    If ptwksWSReadingsRow.IsRefCountsNull Then
                        cmdText += ", 0" & vbCrLf
                    Else
                        cmdText += ", " & ptwksWSReadingsRow.RefCounts.ToString & vbCrLf
                    End If

                    If ptwksWSReadingsRow.IsDateTimeNull Then
                        cmdText += ", '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    Else
                        cmdText += ", '" & Convert.ToDateTime(ptwksWSReadingsRow.DateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    End If

                    'TR 10/10/2013 add the pause column, is a required value, so no need validation.
                    cmdText += ", '" & ptwksWSReadingsRow.Pause & "'" & vbCrLf

                    cmdText += ")"

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()

                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.Insert", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="ptwksWSReadingsRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: ¿?
        ''' Modified  : TR 03/03/2011 -Add the Led position column.
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal ptwksWSReadingsRow As twksWSReadingsDS.twksWSReadingsRow) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText = "UPDATE twksWSReadings SET" & vbCrLf

                    'TR 03/03/2011 -Add the Led position column
                    If ptwksWSReadingsRow.IsLedPositionNull Then
                        cmdText += "  LedPosition = 0" & vbCrLf
                    Else
                        cmdText += String.Format(" LedPosition = {0}{1}", ptwksWSReadingsRow.LedPosition, vbCrLf)
                    End If
                    'TR 03/03/2011 -End

                    If ptwksWSReadingsRow.IsMainCountsNull Then
                        cmdText += ", MainCounts = 0" & vbCrLf
                    Else
                        cmdText += ", MainCounts = " & ptwksWSReadingsRow.MainCounts.ToString & vbCrLf
                    End If

                    If ptwksWSReadingsRow.IsRefCountsNull Then
                        cmdText += ", RefCounts = 0" & vbCrLf
                    Else
                        cmdText += ", RefCounts = " & ptwksWSReadingsRow.RefCounts.ToString & vbCrLf
                    End If

                    If ptwksWSReadingsRow.IsDateTimeNull Then
                        cmdText += ", DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    Else
                        cmdText += ", DateTime = '" & Convert.ToDateTime(ptwksWSReadingsRow.DateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    End If


                    cmdText += "  WHERE AnalyzerID       = '" & ptwksWSReadingsRow.AnalyzerID.ToString & "'" & vbCrLf & _
                               "    AND WorkSessionID    = '" & ptwksWSReadingsRow.WorkSessionID.ToString & "'" & vbCrLf & _
                               "    AND ExecutionID      = " & ptwksWSReadingsRow.ExecutionID.ToString & vbCrLf & _
                               "    AND ReactionComplete = '" & ptwksWSReadingsRow.ReactionComplete.ToString & "'" & vbCrLf & _
                               "    AND ReadingNumber    = " & ptwksWSReadingsRow.ReadingNumber.ToString

                    Using dbCmd As New SqlCommand 'TR 10/10/2013 -Implemente the using.
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False

                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                    cmdText = String.Empty 'TR 10/10/2013 -Set value = to empty.
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.Update", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Check if exists the reading
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="ptwksWSReadingsRow"></param>
        ''' <param name="pLookForPreviousReadingFlag"></param>
        ''' <returns></returns>
        ''' <remarks>Created ???
        ''' Modified AG 02/07/2012 - add parameter pExistsPreviousReadingFlag (optional)</remarks>
        Public Function ExistsReading(ByVal pDBConnection As SqlClient.SqlConnection, _
                                      ByVal ptwksWSReadingsRow As twksWSReadingsDS.twksWSReadingsRow, Optional ByVal pLookForPreviousReadingFlag As Boolean = False) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String

                        cmdText = "SELECT 1" & vbCrLf & _
                                  "  FROM twksWSReadings" & vbCrLf & _
                                  "  WHERE AnalyzerID       = '" & ptwksWSReadingsRow.AnalyzerID.ToString & "'" & vbCrLf & _
                                  "    AND WorkSessionID    = '" & ptwksWSReadingsRow.WorkSessionID.ToString & "'" & vbCrLf & _
                                  "    AND ExecutionID      = " & ptwksWSReadingsRow.ExecutionID.ToString & vbCrLf & _
                                  "    AND ReactionComplete = '" & ptwksWSReadingsRow.ReactionComplete.ToString & "'"

                        If Not pLookForPreviousReadingFlag Then
                            cmdText += "    AND ReadingNumber    = " & ptwksWSReadingsRow.ReadingNumber.ToString
                        Else
                            Dim readingValue As Integer = ptwksWSReadingsRow.ReadingNumber - 1
                            cmdText += "    AND ReadingNumber    = " & readingValue.ToString
                        End If


                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet
                        Dim existsReadingData As New twksWSReadingsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(existsReadingData.twksWSReadings)

                        resultData.HasError = False
                        resultData.SetDatos = existsReadingData
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.ExistsReading", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get the baselines by analyzerid and worksession
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>returns the block enabled status in GlobalDataTO</returns>
        ''' <remarks>
        ''' Created by: DL 19/02/2010
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pAnalyzerID As String, _
                                         ByVal pWorkSessionID As String, _
                                         Optional ByVal pExecutionID As Integer = -1) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        '// 14/10/2013 - cf - v3.0.0 Added the "pause" column to the query. 
                        '//Changes for TASK + BUGS Tracking  #1331
                        cmdText &= " SELECT   executionid, reactioncomplete, readingnumber, maincounts, refcounts, AnalyzerID, WorkSessionID, LedPosition, Pause " & vbCrLf
                        cmdText &= "   FROM   twksWSReadings " & vbCrLf
                        cmdText &= "  WHERE   AnalyzerID    = '" & pAnalyzerID.Trim & "'" & vbCrLf
                        cmdText &= "    AND   WorkSessionID = '" & pWorkSessionID.Trim & "'" & vbCrLf

                        If pExecutionID > 0 Then
                            cmdText &= "    AND    ExecutionID = " & pExecutionID & vbCrLf
                        End If

                        cmdText &= " ORDER BY readingnumber"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        Dim myReadingsDS As New twksWSReadingsDS
                        dbDataAdapter.Fill(myReadingsDS.twksWSReadings)

                        resultData.SetDatos = myReadingsDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSBLinesDAO.GetByWorkSession", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Delete readings by Execution - Worksession - Analyzer
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pExecutionDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>AG 02/10/2012</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pExecutionDS As ExecutionsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionDS.twksWSExecutions.Rows
                        If Not row.IsExecutionIDNull Then
                            cmdText &= " DELETE FROM twksWSReadings " & vbCrLf & _
                                       " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                       " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                       " AND    ExecutionID = " & row.ExecutionID.ToString & " "
                            cmdText &= vbNewLine
                        End If
                    Next

                    If cmdText <> "" Then
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Others Methods"
        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, _
                                ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                Else
                    Dim cmdText As String
                    cmdText = "DELETE twksWSReadings" & vbCrLf & _
                              "  WHERE AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                              "  AND   WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'"

                    Dim cmd As SqlCommand

                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    cmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function

#End Region

#Region "NEW CRUD METHODS"
        ''' <summary>
        ''' Insert all Readings received for an specific Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSReadingsDS">Typed DataSet twksWSReadingsDS containing all Readings to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2012 
        ''' Modified by: TR 10/10/2013 -Add pause column.
        ''' </remarks>
        Public Function InsertNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSReadingsDS As twksWSReadingsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As New StringBuilder()
                    For Each readingRow As twksWSReadingsDS.twksWSReadingsRow In pWSReadingsDS.twksWSReadings
                        If (readingRow.IsLedPositionNull) Then readingRow.LedPosition = 0
                        If (readingRow.IsMainCountsNull) Then readingRow.MainCounts = 0
                        If (readingRow.IsRefCountsNull) Then readingRow.RefCounts = 0
                        If (readingRow.IsDateTimeNull) Then readingRow.DateTime = Now

                        cmdText.Append(" INSERT INTO twksWSReadings (AnalyzerID, WorkSessionID, ExecutionID, ReactionComplete, ")
                        cmdText.Append(" ReadingNumber, LedPosition, MainCounts, RefCounts, DateTime, Pause) ")
                        cmdText.Append(" VALUES(")
                        cmdText.AppendFormat("N'{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, '{8:yyyyMMdd HH:mm:ss}', '{9}') {10} ", _
                                             readingRow.AnalyzerID.Trim.Replace("'", "''"), readingRow.WorkSessionID.Trim, readingRow.ExecutionID.ToString, _
                                             IIf(readingRow.ReactionComplete, 1, 0).ToString, readingRow.ReadingNumber.ToString, readingRow.LedPosition.ToString, _
                                             readingRow.MainCounts.ToString, readingRow.RefCounts.ToString, readingRow.DateTime, readingRow.Pause, vbCrLf)
                    Next

                    If (cmdText.Length > 0) Then
                        'Execute all the INSERTS contained in the string 
                        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.InsertNEW", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Readings of all Executions for a group of Order Tests that fulfill the following condition: ALL their Executions 
        ''' have Execution Status PENDING or LOCKED 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestsListDS">Typed DataSet OrderTestsDS containing the group of OrderTests having ALL their Executions 
        '''                                 with status PENDING or LOCKED</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA  31/07/2012
        ''' AG 19/02/2014 - #1514
        ''' </remarks>
        Public Function DeleteReadingsForNotInCourseExecutions(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                              ByVal pOrderTestsListDS As OrderTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim i As Integer = 0
                    Dim maxUpdates As Integer = 500
                    Dim cmdText As New StringBuilder()

                    'AG 19/02/2014 - #1514 - previous query spend lot of time in wide worksessions
                    'For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In pOrderTestsListDS.twksOrderTests
                    '    cmdText.Append(" DELETE FROM twksWSReadings ")
                    '    cmdText.Append(" WHERE  AnalyzerID = ")
                    '    cmdText.AppendFormat("N'{0}'", pAnalyzerID.Trim.Replace("'", "''"))
                    '    cmdText.Append(" AND    WorkSessionID = ")
                    '    cmdText.AppendFormat("'{0}'", pWorkSessionID.Trim)
                    '    cmdText.Append(" AND    ExecutionID IN (SELECT ExecutionID FROM twksWSExecutions WHERE OrderTestID = ")
                    '    cmdText.AppendFormat("{0})", orderTestRow.OrderTestID.ToString)
                    '    cmdText.Append(vbCrLf)

                    '    i += 1
                    '    If (i = maxUpdates) Then
                    '        'Execute the SQL script
                    '        Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                    '            resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                    '        End Using

                    '        'Initialize the counter and the StringBuilder
                    '        i = 0
                    '        cmdText.Remove(0, cmdText.Length)
                    '    End If
                    'Next orderTestRow

                    Dim filterClause As String = ""
                    For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In pOrderTestsListDS.twksOrderTests
                        If pOrderTestsListDS.twksOrderTests.Rows.Count > 1 Then
                            Select Case i
                                Case 0 'First item
                                    filterClause &= " (OrderTestID = " & orderTestRow.OrderTestID.ToString & " "
                                Case pOrderTestsListDS.twksOrderTests.Rows.Count - 1 'Last item
                                    filterClause &= " OR OrderTestID = " & orderTestRow.OrderTestID.ToString & " ) "
                                Case Else 'Middle item
                                    filterClause &= " OR OrderTestID = " & orderTestRow.OrderTestID.ToString & " "
                            End Select
                        Else 'Only 1 item
                            filterClause &= " (OrderTestID = " & orderTestRow.OrderTestID.ToString & " ) "
                        End If
                        i += 1
                    Next

                    cmdText.Append(" DELETE FROM twksWSReadings ")
                    cmdText.Append(" WHERE  AnalyzerID = ")
                    cmdText.AppendFormat("N'{0}'", pAnalyzerID.Trim.Replace("'", "''"))
                    cmdText.Append(" AND    WorkSessionID = ")
                    cmdText.AppendFormat("'{0}'", pWorkSessionID.Trim)
                    cmdText.Append(" AND    ExecutionID IN (SELECT ExecutionID FROM twksWSExecutions WHERE " & filterClause & " )")
                    cmdText.Append(vbCrLf)
                    'AG 19/02/2014 - #1514

                    If (Not resultData.HasError) Then
                        If (cmdText.Length > 0) Then
                            'Execute the remaining deletes...
                            Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                                resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.DeleteReadingsForNotInCourseExecutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Readings that exist for Executions marked as LOCKED - to avoid errors when the Executions were locked due to lack of R2 volume
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 01/08/2012
        ''' </remarks>
        Public Function DeleteReadingsForLockedExecutions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                          ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksWSReadings " & vbCrLf & _
                                            " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND    ExecutionID IN (SELECT ExecutionID FROM twksWSExecutions " & vbCrLf & _
                                                                   " WHERE  AnalyzerID    = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                                   " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                   " AND    ExecutionStatus = 'LOCKED') " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSReadingsDAO.DeleteReadingsForLockedExecutions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace




