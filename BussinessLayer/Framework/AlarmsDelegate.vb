Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class AlarmsDelegate

#Region "Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared GettfmwtfmwAlarmsDAO As Func(Of ItfmwAlarms) = Function() New tfmwAlarmsDAO

        ''' <summary>
        ''' Get data of the specified Alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAlarmID">Identifier of the Alarm</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>
        ''' Created by:  SG 03/06/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the identified alarm
                        Dim myAlarms As ItfmwAlarms = GettfmwtfmwAlarmsDAO()
                        resultData = myAlarms.Read(dbConnection, pAlarmID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "AlarmsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the description fields in Alarms table with the translated texts according to the language set
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLanguageID">Language Identifier to be translate</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by : XBC 18/10/2010
        ''' </remarks>
        Public Function UpdateLanguageResource(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLanguageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the identified alarm
                        Dim myAlarms As ItfmwAlarms = GettfmwtfmwAlarmsDAO()
                        resultData = myAlarms.UpdateLanguageResource(dbConnection, pLanguageID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "AlarmsDelegate.UpdateLanguageResource", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all alarms table
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As ItfmwAlarms = GettfmwtfmwAlarmsDAO()
                        resultData = myDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "AlarmsDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get management information from the specified Alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAlarmID">Identifier of the Alarm</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>
        ''' Created by XBC 16/10/2012
        ''' </remarks>
        Public Function ReadManagementAlarm(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get management data from the identified alarm
                        Dim myAlarms As ItfmwAlarms = GettfmwtfmwAlarmsDAO()
                        resultData = myAlarms.ReadManagementAlarm(dbConnection, pAlarmID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                CreateLogActivity(ex.Message, "AlarmsDelegate.ReadManagementAlarm", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace

