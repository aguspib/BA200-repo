

Option Explicit On
Option Strict On


Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class TwksOrderTestsDAO
        Inherits DAOBase

#Region "AUTOMATICALLY GENERATED CODE"


#Region "READ BY"

        Public Function ReadByOrderID(ByVal OrderID As String) As OrderTestsDS

            Dim result As New OrderTestsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT OrderTestID, OrderID, TestType, TestID, SampleType, OrderTestStatus, TubeType, AnalyzerID, ExportDateTime, TS_User, TS_DateTime FROM twksOrderTests  WHERE OrderID = '" & OrderID & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.twksOrderTests)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TwksOrderTestsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TwksOrderTestsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TwksOrderTestsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

#End Region 'READ BY



#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

