

Option Explicit On
Option Strict On


Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class ApplicationSettingDAO
          

#Region "AUTOMATICALLY GENERATED CODE"

#Region "CRUD"

#Region "READ"

        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal SettingID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim ApplicationSettingData As New ApplicationSettingDS()

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = "SELECT SettingID, Description, CurrentValue, Status " & _
                                                "FROM ApplicationSetting " & " WHERE " & "SettingID = '" & SettingID.ToString() & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(ApplicationSettingData.ApplicationSetting)

                        resultData.SetDatos = ApplicationSettingData
                        resultData.HasError = False

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSettingDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

#End Region 'READ

#Region "UPDATE"

        Public Function Update(ByVal applicationsetting As ApplicationSettingDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As Integer

            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As New SqlCommand
            Dim cmdText As String = ""
            Dim values As String = ""
            Dim result As Integer = 0
            Try
               
                cmd = Conn.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = Conn
                cmd.Transaction = Transaccion


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSettingDAO", EventLogEntryType.Error, False)
            End Try

            Try
                For Each applicationsettingDR As DataRow In applicationsetting.ApplicationSetting
                    values = "SettingID = '" & applicationsettingDR("SettingID").ToString().Replace("'", "''") & "', Description = " & _
                             "'" & applicationsettingDR("Description").ToString().Replace("'", "''") & _
                             "'," & "CurrentValue = " & "'" & applicationsettingDR("CurrentValue").ToString().Replace("'", "''") & _
                             "'," & "Status = '" & applicationsettingDR("Status").ToString() & "'"

                    cmdText = "UPDATE ApplicationSetting SET " & values & _
                    " WHERE " & _
                        "SettingID = '" & applicationsettingDR("SettingID").ToString() & "'"
                    cmd.CommandText = cmdText

                    result += cmd.ExecuteNonQuery()
                Next

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSettingDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSettingDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSettingDAO", EventLogEntryType.Error, False)

            Finally

                'always call Close when done with conn.
                connectionNew.Close()

            End Try

            Return result

        End Function

#End Region 'UPDATE

#End Region 'CRUD
#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

