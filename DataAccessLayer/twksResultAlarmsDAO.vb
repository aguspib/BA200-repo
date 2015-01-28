Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksResultAlarmsDAO
        '  

#Region "CRUD Methods"

        ''' <summary>
        ''' Get all Result Alarms that exist for the specified OrderTestID, RerunNumber and MultipointNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pMultiPointNumber">Multipoint Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultAlarmsDS with the list of Alarms</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                             ByVal pRerunNumber As Integer, ByVal pMultiPointNumber As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM twksResultAlarms " & vbCrLf & _
                                                " WHERE  OrderTestID      = " & pOrderTestID.ToString & vbCrLf & _
                                                " AND    RerunNumber      = " & pRerunNumber.ToString & vbCrLf & _
                                                " AND    MultiPointNumber = " & pMultiPointNumber.ToString & vbCrLf

                        Dim ResultAlarms As New ResultAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(ResultAlarms.twksResultAlarms)
                            End Using
                        End Using

                        resultData.SetDatos = ResultAlarms
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a group of Result Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open BD Connection</param>
        ''' <param name="pResultAlarms">Typed DataSet ResultAlarmsDS with the list of Result Alarms to add</param>
        ''' <returns>GlobalDataTO containing the same entry DataSet</returns>
        ''' <remarks>
        ''' Created by:  SG 07/06/2010
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultAlarms As ResultAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each resultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow In pResultAlarms.twksResultAlarms
                        Dim cmdText As String = ""
                        cmdText = " INSERT INTO twksResultAlarms(OrderTestID, RerunNumber, MultipointNumber, AlarmID, AlarmDateTime) " & _
                                  " VALUES(" & resultAlarmRow.OrderTestID & ", " & _
                                         " " & resultAlarmRow.RerunNumber & ", " & _
                                         " " & resultAlarmRow.MultiPointNumber & ", " & _
                                        " '" & resultAlarmRow.AlarmID & "', " & _
                                        " '" & Convert.ToDateTime(resultAlarmRow.AlarmDateTime.ToString).ToString("yyyyMMdd HH:mm:ss") & "')"

                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                        If (resultData.AffectedRecords = 0) Then
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                            Exit For
                        End If
                    Next

                    If (Not resultData.HasError) Then
                        resultData.SetDatos = pResultAlarms.Clone
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.Add", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Alarms for the specified OrderTestID, RerunNumber, MultiPointNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pMultiPointNumber">Multipoint Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/06/2010 
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, _
                                  ByVal pRerunNumber As Integer, ByVal pMultiPointNumber As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM twksResultAlarms " & vbCrLf & _
                              " WHERE  OrderTestID = " & pOrderTestID & vbCrLf & _
                              " AND    RerunNumber = " & pRerunNumber & vbCrLf & _
                              " AND    MultiPointNumber = " & pMultiPointNumber

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other methods"
        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: SG 19/07/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    'AJG
                    'cmdText = " DELETE FROM twksResultAlarms " & _
                    '          " WHERE  OrderTestID NOT IN (SELECT OrderTestID " & _
                    '                                     " FROM twksWSOrderTests) "

                    cmdText = " DELETE FROM twksResultAlarms " & _
                              " WHERE  NOT EXISTS (SELECT OrderTestID " & _
                                                         " FROM twksWSOrderTests WHERE twksResultAlarms.OrderTestID = OrderTestID) "

                    Dim cmd As New SqlCommand
                    cmd = New SqlCommand
                    cmd = pDBConnection.CreateCommand
                    cmd.CommandText = cmdText

                    resultData.AffectedRecords = cmd.ExecuteNonQuery()
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Result Alarms that exist for all Order Tests belonging to the specified Order
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderID">Order Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 25/01/2011
        ''' </remarks>
        Public Function DeleteResultAlarmsByOrderID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    'AJG
                    'cmdText = " DELETE FROM twksResultAlarms " & _
                    '          " WHERE  OrderTestID IN (SELECT OrderTestID FROM twksOrderTests " & _
                    '                                 " WHERE OrderID = '" & pOrderID & "') "

                    cmdText = " DELETE FROM twksResultAlarms " & _
                              " WHERE  EXISTS (SELECT OrderTestID FROM twksOrderTests " & _
                                                     " WHERE OrderID = '" & pOrderID & "' AND twksResultAlarms.OrderTestID = OrderTestID) "

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.DeleteResultAlarmsByOrderID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the Alarms related to informed Order Test Id
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 15/03/2013
        ''' </remarks>
        Public Function DeleteResultAlarmsByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE FROM twksResultAlarms " & _
                              " WHERE  OrderTestID = '" & pOrderTestID.ToString & "' "

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.DeleteResultAlarmsByOrderTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW"
        ''' <summary>
        ''' Delete a result alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB connection</param>
        ''' <param name="pResulAlarms">Data Set with only one datarow specifier of the result alarm to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultAlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>Created by:  SG 07/06/2010</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResulAlarms As ResultAlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else

                    Dim dbCmd As New SqlClient.SqlCommand
                    For Each ResultAlarmsRow As ResultAlarmsDS.twksResultAlarmsRow In pResulAlarms.twksResultAlarms
                        Dim cmdText As String
                        cmdText = " DELETE FROM twksResultAlarms " & vbCrLf & _
                                  " WHERE  AlarmID = '" & ResultAlarmsRow.AlarmID & "'"

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksResultAlarmsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class

End Namespace

