Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class twksWSExecutionAlarmsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Get data from the specified result alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open Database connection</param>
        ''' <param name="pExecutionID">ExecutionID</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSExecutionAlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>Created by:  SG 07/06/2010</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT * " & _
                                  " FROM   twksWSExecutionAlarms " & _
                                  " WHERE  ExecutionID = " & pExecutionID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim WSExecutionAlarms As New WSExecutionAlarmsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(WSExecutionAlarms.twksWSExecutionAlarms)

                        resultData.SetDatos = WSExecutionAlarms
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Add a result alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open Database connection</param>
        ''' <param name="pWSExecutionAlarms">Data Set with only one datarow specifier of the result alarm to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSExecutionAlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>Created by:  SG 07/06/2010</remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSExecutionAlarms As WSExecutionAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (IsNothing(pDBConnection)) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each WSExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In pWSExecutionAlarms.twksWSExecutionAlarms

                        Dim cmdText As String = ""

                        cmdText = " INSERT INTO twksWSExecutionAlarms(ExecutionID, AlarmID, AlarmDateTime) " & _
                                  " VALUES(" & WSExecutionAlarmsRow.ExecutionID & ", " & _
                                        " '" & WSExecutionAlarmsRow.AlarmID & "', " & _
                                        " '" & Convert.ToDateTime(WSExecutionAlarmsRow.AlarmDateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "')"


                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        If (resultData.AffectedRecords > 0) Then
                            resultData.HasError = False
                            resultData.SetDatos = pWSExecutionAlarms.Clone
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        End If

                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.Add", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Delete a result alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open Database connection</param>
        ''' <param name="pWSExecutionAlarms">Data Set with only one datarow specifier of the result alarm to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSExecutionAlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>Created by:  SG 07/06/2010</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSExecutionAlarms As WSExecutionAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each WSExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In pWSExecutionAlarms.twksWSExecutionAlarms
                        Dim cmdText As String
                        cmdText = " DELETE FROM twksWSExecutionAlarms " & vbCrLf & _
                                  " WHERE  AlarmID = '" & WSExecutionAlarmsRow.AlarmID & "'"


                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region


#Region "Other Methods"

        ''' <summary>
        ''' Delete all alarms generated for Executions of the specified AnalyzerID and WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 07/06/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE twksWSExecutionAlarms " & _
                              " WHERE  ExecutionID IN (SELECT ExecutionID FROM twksWSExecutions " & _
                                                     " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                                     " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "')"

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
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all results alarms of an specific executionID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by AG 08/06/2010 (tested OK)
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM twksWSExecutionAlarms " & vbCrLf & _
                              " WHERE  ExecutionID = " & pExecutionID

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                    resultData.HasError = False

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

    End Class
End Namespace
