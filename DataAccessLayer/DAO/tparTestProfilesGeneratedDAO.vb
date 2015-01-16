

Option Explicit On
Option Strict On


Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class TparTestProfilesDAO
        Inherits DAOBase

#Region "AUTOMATICALLY GENERATED CODE"

#Region "CRUD"

#Region "CREATE"

        Public Function Create(ByVal tpartestprofiles As TestProfilesDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO


            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Dim keys As String = "(TestProfileName, SampleType, TestProfilePosition, TS_User, TS_DateTime)"
            Dim values As String = " "
            Dim MyGlobalTo As New GlobalDataTO()

            Try
                If Transaccion Is Nothing Then
                    connectionNew.Open()
                    cmd = connectionNew.CreateCommand()
                    cmd.CommandText = cmdText
                    cmd.Connection = connectionNew
                Else
                    cmd = Conn.CreateCommand()
                    cmd.CommandText = cmdText
                    cmd.Connection = Conn
                    cmd.Transaction = Transaccion
                End If
                For Each tpartestprofilesDR As DataRow In tpartestprofiles.tpartestprofiles
                    'If tpartestprofilesDR("TestProfileID") Is DBNull.Value Then
                    '    values &= "NULL,"
                    'Else
                    '    values &= tpartestprofilesDR("TestProfileID").ToString() & ","
                    'End If

                    If tpartestprofilesDR("TestProfileName") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tpartestprofilesDR("TestProfileName").ToString().Replace("'", "''") & "',"
                    End If

                    If tpartestprofilesDR("SampleType") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestprofilesDR("SampleType").ToString().Replace("'", "''") & "',"
                    End If

                    If tpartestprofilesDR("TestProfilePosition") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= tpartestprofilesDR("TestProfilePosition").ToString() & ","
                    End If

                    If tpartestprofilesDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestprofilesDR("TS_User").ToString().Replace("'", "''") & "',"
                    End If

                    If tpartestprofilesDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= " '" & tpartestprofilesDR("TS_DateTime").ToString() & "'"
                    End If

                    cmdText = "INSERT INTO tparTestProfiles  " & keys & " VALUES (" & values & ")"
                    cmd.CommandText = cmdText

                    cmd.CommandText &= " SELECT SCOPE_IDENTITY()"
                    tpartestprofilesDR.BeginEdit()
                    tpartestprofilesDR.SetField("TestProfileID", CType(cmd.ExecuteScalar(), Integer))
                    tpartestprofilesDR.EndEdit()
                    MyGlobalTo.AffectedRecords += 1

                    If MyGlobalTo.AffectedRecords > 0 Then
                        MyGlobalTo.HasError = False
                    Else
                        MyGlobalTo.HasError = True
                    End If

                Next tpartestprofilesDR

            Catch ex As ArgumentException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestProfilesDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestProfilesDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestProfilesDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'CREATE

#End Region 'CRUD
#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

