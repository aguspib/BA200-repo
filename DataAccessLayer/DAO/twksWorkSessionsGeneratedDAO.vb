Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class TwksWorkSessionsDAO
          

#Region "AUTOMATICALLY GENERATED CODE"

#Region "READ ALL"

        Public Function ReadAll() As WorkSessionsDS

            Dim result As New WorkSessionsDS()
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Try
                Const cmdText As String = "SELECT WorkSessionID, WSDateTime, SequenceNumber, TS_User, TS_DateTime, StartDateTime FROM twksWorkSessions"

                Dim cmd As SqlCommand = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.twksWorkSessions)

            Catch ex As ArgumentException
                GlobalBase.CreateLogActivity(ex.Message, "TwksWorkSessionsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                GlobalBase.CreateLogActivity(ex.Message, "TwksWorkSessionsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "TwksWorkSessionsDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()

            End Try
            Return result
        End Function


#End Region 'READ ALL

#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

