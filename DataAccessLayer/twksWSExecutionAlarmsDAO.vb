Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class twksWSExecutionAlarmsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get all Alarms for the specified Execution 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSExecutionAlarmsDS with the list of Alarms of the specified Execution</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' Modified by: SA 12/06/2014 - Implement USING for SqlCommand and SqlDataAdapter
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksWSExecutionAlarms " & vbCrLf & _
                                                " WHERE  ExecutionID = " & pExecutionID

                        Dim WSExecutionAlarms As New WSExecutionAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(WSExecutionAlarms.twksWSExecutionAlarms)
                            End Using
                        End Using

                        resultData.SetDatos = WSExecutionAlarms
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add Alarms for an Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSExecutionAlarms">Typed DataSet WSExecutionAlarmsDS containing the list of Alarms to add for an Execution</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' Modified by: SA 12/06/2014 - Implement USING for SqlCommand
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSExecutionAlarms As WSExecutionAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    For Each WSExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In pWSExecutionAlarms.twksWSExecutionAlarms
                        cmdText = " INSERT INTO twksWSExecutionAlarms(ExecutionID, AlarmID, AlarmDateTime) " & _
                                  " VALUES(" & WSExecutionAlarmsRow.ExecutionID & ", " & _
                                        " '" & WSExecutionAlarmsRow.AlarmID & "', " & _
                                        " '" & Convert.ToDateTime(WSExecutionAlarmsRow.AlarmDateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "')"

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        End Using

                        'Execute the SQL Sentence
                        'Dim dbCmd As New SqlCommand
                        'dbCmd.Connection = pDBConnection
                        'dbCmd.CommandText = cmdText

                        'resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        'If (resultData.AffectedRecords > 0) Then
                        '    resultData.HasError = False
                        '    resultData.SetDatos = pWSExecutionAlarms.Clone
                        'Else
                        '    resultData.HasError = True
                        '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        'End If
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
        ''' Delete the specified Alarms (for all Executions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSExecutionAlarms">Typed DataSet WSExecutionAlarmsDS containing the list of Alarms to delete for all Executions</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' Modified by: SA 12/06/2014 - Implement USING for SqlCommand
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSExecutionAlarms As WSExecutionAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    For Each WSExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow In pWSExecutionAlarms.twksWSExecutionAlarms
                        cmdText = " DELETE FROM twksWSExecutionAlarms " & vbCrLf & _
                                  " WHERE  AlarmID = '" & WSExecutionAlarmsRow.AlarmID & "'"

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        End Using

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = pDBConnection
                        'dbCmd.CommandText = cmdText

                        'resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                        'resultData.HasError = False
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
        ''' Modified by: SA 12/06/2014 - Implement USING for SqlCommand
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSExecutionAlarms " & _
                                            " WHERE  ExecutionID IN (SELECT ExecutionID FROM twksWSExecutions " & _
                                                                   " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                                                   " AND    WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "')"

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        resultData.HasError = False
                    End Using

                    'Dim cmd As SqlCommand
                    'cmd = pDBConnection.CreateCommand
                    'cmd.CommandText = cmdText

                    'cmd.ExecuteNonQuery()
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
        ''' Delete all alarms for one specific Execution
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/06/2010
        ''' Modified by: SA 12/06/2014 - Implement USING for SqlCommand
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksWSExecutionAlarms " & vbCrLf & _
                                            " WHERE  ExecutionID = " & pExecutionID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        resultData.HasError = False
                    End Using

                    'Dim dbCmd As New SqlClient.SqlCommand
                    'dbCmd.Connection = pDBConnection
                    'dbCmd.CommandText = cmdText

                    'resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                    'resultData.HasError = False
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

        ''' <summary>
        ''' For the specified OrderTestID and RerunNumber, search all different Alarms for all the active Executions (excepting the Reference Ranges 
        ''' Alarms) that will be moved to the Average Result with it is recalculated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with the list of Executions Alarms found</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2014 - BT #1660
        ''' </remarks>
        Public Function ReadAlarmsForAverageResult(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                   ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT EA.AlarmID " & vbCrLf & _
                                                " FROM   twksWSExecutionAlarms EA INNER JOIN twksWSExecutions E ON EA.ExecutionID = E.ExecutionID " & vbCrLf & _
                                                " WHERE  EA.AlarmID NOT IN ('" & GlobalEnumerates.CalculationRemarks.CONC_REMARK7.ToString & "', " & vbCrLf & _
                                                                           "'" & GlobalEnumerates.CalculationRemarks.CONC_REMARK8.ToString & "') " & vbCrLf & _
                                                " AND    E.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    E.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " AND    E.OrderTestID = " & pOrderTestID & vbCrLf & _
                                                " AND    E.RerunNumber = " & pRerunNumber & vbCrLf & _
                                                " AND    E.InUse = 1 " & vbCrLf

                        Dim myAlarmsDS As New AlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myAlarmsDS.tfmwAlarms)
                            End Using
                        End Using

                        resultData.HasError = False
                        resultData.SetDatos = myAlarmsDS
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSExecutionAlarmsDAO.ReadAlarmsForAverageResult", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace
