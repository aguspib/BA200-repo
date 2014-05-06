Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class thrsResultsServiceDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Get all activities contents done with the Instrument
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 27/07/2011</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                Optional ByVal pTaskID As String = "", Optional ByVal pActionID As String = "", _
                                Optional ByVal pDateFrom As DateTime = Nothing, Optional ByVal pDateTo As DateTime = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT * " & vbCrLf
                        cmdText &= "FROM srv_thrsResultsService " & vbCrLf
                        cmdText &= "WHERE  AnalyzerID = '" & pAnalyzerID.ToString() & "'" & vbCrLf

                        'Add filters if optional parameters are informed
                        If (pTaskID.Length > 0) Then cmdText &= " AND    TaskID = '" & pTaskID.ToString() & "'" & vbCrLf
                        If (pActionID.Length > 0) Then cmdText &= " AND    ActionID = '" & pActionID.ToString() & "'" & vbCrLf

                        If (pDateFrom <> Nothing) Then cmdText &= " AND    DATEDIFF(DAY, '" & pDateFrom.ToString("yyyyMMdd") & "', TS_DateTime) >= 0 " & Environment.NewLine
                        If (pDateTo <> Nothing) Then cmdText &= " AND    DATEDIFF(DAY, TS_DateTime, '" & pDateTo.ToString("yyyyMMdd") & "')   >= 0 " & Environment.NewLine

                        'Sort Results by datetime
                        cmdText &= " ORDER BY TS_DateTime DESC "

                        Dim myDataSet As New SRVResultsServiceDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.srv_thrsResultsService)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsResultsServiceDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the different analyzers that have been registered activities with this software
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>XBC 03/08/2011</remarks>
        Public Function ReadByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= "SELECT AnalyzerID " & vbCrLf
                        cmdText &= "FROM srv_thrsResultsService " & vbCrLf
                        cmdText &= "GROUP BY AnalyzerID " & vbCrLf

                        Dim myDataSet As New SRVResultsServiceDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.srv_thrsResultsService)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsResultsServiceDAO.ReadByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Historic Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <remarks>
        ''' Created by: XBC 01/08/2011
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoricReport As SRVResultsServiceDS.srv_thrsResultsServiceRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO srv_thrsResultsService(TaskID, ActionID, Data, Comments, " & _
                                                                 " AnalyzerID, TS_User, TS_DateTime) " & _
                              " VALUES (N'" & pHistoricReport.TaskID.ToString.Replace("'", "''") & "', " & _
                                      " N'" & pHistoricReport.ActionID.ToString.Replace("'", "''") & "', " & _
                                      " N'" & pHistoricReport.Data.ToString.Replace("'", "''") & "', "

                    If (String.IsNullOrEmpty(pHistoricReport.Comments.ToString)) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " N'" & pHistoricReport.Comments.ToString.Replace("'", "''") & "', "
                    End If


                    cmdText &= " N'" & pHistoricReport.AnalyzerID.ToString.Replace("'", "''") & "', "


                    'Get the connected Username from the current Application Session
                    Dim currentSession As New GlobalBase
                    cmdText &= " N'" & currentSession.GetSessionInfo().UserName.ToString.Replace("'", "''") & "', "


                    cmdText &= " '" & CType(Now, DateTime).ToString("yyyyMMdd HH:mm:ss") & "') "

                    'Finally, get the automatically generated ID for the created Calculated Test
                    cmdText &= " SELECT SCOPE_IDENTITY() "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    Dim newHistoricReportID As Integer
                    newHistoricReportID = CType(dbCmd.ExecuteScalar(), Integer)

                    If (newHistoricReportID > 0) Then
                        pHistoricReport.BeginEdit()
                        pHistoricReport.SetField("ResultServiceID", newHistoricReportID)
                        pHistoricReport.EndEdit()

                        resultData.HasError = False
                        resultData.AffectedRecords = 1
                        resultData.SetDatos = pHistoricReport
                    Else
                        resultData.HasError = True
                        resultData.AffectedRecords = 0
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsResultsServiceDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Standard Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultServiceList">List of Result Reports to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by XBC 04/08/2011
        ''' </remarks>
        Public Function UpdateComments(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceList As SRVResultsServiceDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pResultServiceList Is Nothing) Then
                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True

                    For Each rowthrsResultService As SRVResultsServiceDS.srv_thrsResultsServiceRow In pResultServiceList.srv_thrsResultsService.Rows
                        Dim cmdText As String = ""
                        cmdText = " UPDATE srv_thrsResultsService " & _
                                  " SET    Comments = '" & rowthrsResultService.Comments & "'" & _
                                  " WHERE  ResultServiceID = " & rowthrsResultService.ResultServiceID

                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        recordOK = (myGlobalDataTO.AffectedRecords = 1)
                        i += 1

                        If (Not recordOK) Then Exit For
                    Next rowthrsResultService

                    If (recordOK) Then
                        myGlobalDataTO.HasError = False
                        myGlobalDataTO.AffectedRecords = i
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        myGlobalDataTO.AffectedRecords = 0
                    End If

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsResultsServiceDAO.UpdateComments", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Activity 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultServiceID">Result report Identifier</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 04/08/2011
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultServiceID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim cmd As New SqlCommand

                    cmdText &= " DELETE FROM  srv_thrsResultsService "
                    cmdText &= " WHERE ResultServiceID = " & pResultServiceID

                    cmd.CommandText = cmdText
                    cmd.Connection = pDBConnection

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()

                    If myGlobalDataTO.AffectedRecords > 0 Then
                        myGlobalDataTO.HasError = False
                    Else
                        myGlobalDataTO.HasError = True
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thrsResultsServiceDAO.Delete", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

#End Region

    End Class

End Namespace
