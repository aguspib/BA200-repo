

Option Explicit On
Option Strict On


Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class TparPatientsDAO
          

#Region "AUTOMATICALLY GENERATED CODE"

#Region "CRUD"

#Region "CREATE"

        Public Function Create(ByVal tparpatients As PatientsDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO
            Const keys As String = "(PatientID, PatientType, FirstName, LastName, DateOfBirth, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime)"
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Dim values As String = " "
            Dim MyGlobalTo As New GlobalDataTO()
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())

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
                For Each tparpatientsDR As DataRow In tparpatients.tparPatients
                    If tparpatientsDR("PatientID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("PatientID").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("PatientType") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("PatientType").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("FirstName") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("FirstName").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("LastName") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("LastName").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("DateOfBirth") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= " '" & tparpatientsDR("DateOfBirth").ToString() & "', "
                    End If

                    'If tparpatientsDR("Age") Is DBNull.Value Then
                    '    values &= "NULL,"
                    'Else
                    '    values &= tparpatientsDR("Age").ToString() & ","
                    'End If

                    If tparpatientsDR("Gender") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("Gender").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("ExternalPID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("ExternalPID").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("ExternalArrivalDate") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= " '" & tparpatientsDR("ExternalArrivalDate").ToString() & "', "
                    End If

                    If tparpatientsDR("PerformedBy") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("PerformedBy").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("Comments") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("Comments").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("TS_User").ToString().Replace("'", "''") & "',"
                    End If

                    If tparpatientsDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= " '" & tparpatientsDR("TS_DateTime").ToString() & "'"
                    End If

                    cmdText = "INSERT INTO tparPatients  " & keys & " VALUES (" & values & ")"
                    cmd.CommandText = cmdText
                    MyGlobalTo.AffectedRecords = cmd.ExecuteNonQuery()
                    If MyGlobalTo.AffectedRecords > 0 Then
                        MyGlobalTo.HasError = False
                    Else
                        MyGlobalTo.HasError = True
                    End If


                Next

            Catch ex As ArgumentException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'CREATE

#Region "READ"

        Public Function Read(ByVal PatientID As String) As PatientsDS

            Dim conn As New SqlClient.SqlConnection(GetConnectionString())
            Dim result As New PatientsDS()
            Try
                conn.Open()
                Dim cmdText As String = "SELECT PatientID" & vbCrLf & _
                                        "     , PatientType" & vbCrLf & _
                                        "     , FirstName" & vbCrLf & _
                                        "     , LastName" & vbCrLf & _
                                        "     , DateOfBirth" & vbCrLf & _
                                        "     , Age" & vbCrLf & _
                                        "     , Gender" & vbCrLf & _
                                        "     , ExternalPID" & vbCrLf & _
                                        "     , ExternalArrivalDate" & vbCrLf & _
                                        "     , PerformedBy" & vbCrLf & _
                                        "     , Comments" & vbCrLf & _
                                        "     , TS_User" & vbCrLf & _
                                        "     , TS_DateTime" & vbCrLf & _
                                        "     , InUse" & vbCrLf & _
                                        "  FROM tparPatients" & vbCrLf & _
                                        "  WHERE PatientID = '" & PatientID.ToString() & "'"

                Dim cmd As SqlClient.SqlCommand = conn.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = conn
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)
            Finally
                conn.Close()
            End Try
            Return result
        End Function

#End Region 'READ

#Region "UPDATE"

        Public Function Update(ByVal tparpatients As PatientsDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO

            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Dim values As String = ""
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
                For Each tparpatientsDR As DataRow In tparpatients.tparPatients
                    values &= "PatientID = "
                    If tparpatientsDR("PatientID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("PatientID").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "PatientType = "
                    If tparpatientsDR("PatientType") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("PatientType").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "FirstName = "
                    If tparpatientsDR("FirstName") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("FirstName").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "LastName = "
                    If tparpatientsDR("LastName") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("LastName").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "DateOfBirth = "
                    If tparpatientsDR("DateOfBirth") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("DateOfBirth").ToString() _
                        & "',"
                    End If

                    'values &= "Age = "
                    'If tparpatientsDR("Age") Is DBNull.Value Then
                    '    values &= "NULL,"
                    'Else
                    '    values &= tparpatientsDR("Age").ToString() _
                    '     & ","
                    'End If

                    values &= "Gender = "
                    If tparpatientsDR("Gender") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("Gender").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "ExternalPID = "
                    If tparpatientsDR("ExternalPID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("ExternalPID").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "ExternalArrivalDate = "
                    If tparpatientsDR("ExternalArrivalDate") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("ExternalArrivalDate").ToString() _
                        & "',"
                    End If

                    values &= "PerformedBy = "
                    If tparpatientsDR("PerformedBy") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("PerformedBy").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "Comments = "
                    If tparpatientsDR("Comments") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & tparpatientsDR("Comments").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "TS_User = "
                    If tparpatientsDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("TS_User").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "TS_DateTime = "
                    If tparpatientsDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tparpatientsDR("TS_DateTime").ToString() _
                       & "'"
                    End If


                    cmdText = "UPDATE tparPatients SET " & values & _
                    " WHERE " & _
                        "PatientID = '" & tparpatientsDR("PatientID").ToString() & "'"
                    cmd.CommandText = cmdText

                    MyGlobalTo.AffectedRecords += cmd.ExecuteNonQuery()
                    If MyGlobalTo.AffectedRecords > 0 Then
                        MyGlobalTo.HasError = False
                    Else
                        MyGlobalTo.HasError = True
                    End If
                Next

            Catch ex As ArgumentException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'UPDATE

#Region "READ ALL"

        Public Function ReadAll() As PatientsDS

            Dim result As New PatientsDS()
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Try
                Dim cmdText As String = "SELECT PatientID" & vbCrLf & _
                                        "     , PatientType" & vbCrLf & _
                                        "     , FirstName" & vbCrLf & _
                                        "     , LastName" & vbCrLf & _
                                        "     , DateOfBirth" & vbCrLf & _
                                        "     , Age" & vbCrLf & _
                                        "     , Gender" & vbCrLf & _
                                        "     , ExternalPID" & vbCrLf & _
                                        "     , ExternalArrivalDate" & vbCrLf & _
                                        "     , PerformedBy" & vbCrLf & _
                                        "     , Comments" & vbCrLf & _
                                        "     , TS_User" & vbCrLf & _
                                        "     , TS_DateTime" & vbCrLf & _
                                        "     , InUse" & vbCrLf & _
                                        "  FROM tparPatients"

                Dim cmd As SqlCommand = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()

            End Try
            Return result
        End Function

#End Region 'READ ALL

#Region "READ BY"

        '''ALL READ BY


        Public Function ReadByPatientType(ByVal PatientType As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE PatientType = '" & PatientType & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByFirstName(ByVal FirstName As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE FirstName = '" & FirstName & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByLastName(ByVal LastName As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE LastName = '" & LastName & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByDateOfBirth(ByVal DateOfBirth As DateTime) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE CONVERT(VARCHAR, DateOfBirth, 112) = '" & DateOfBirth.ToString("yyyyMMdd") & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByAge(ByVal Age As Integer) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients WHERE Age = " & Age.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByGender(ByVal Gender As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE Gender = '" & Gender & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByExternalPID(ByVal ExternalPID As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE ExternalPID = '" & ExternalPID & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByExternalArrivalDate(ByVal ExternalArrivalDate As DateTime) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE CONVERT(VARCHAR, ExternalArrivalDate, 112) = '" & ExternalArrivalDate.ToString("yyyyMMdd") & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByPerformedBy(ByVal PerformedBy As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE PerformedBy = '" & PerformedBy & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByComments(ByVal Comments As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE Comments = '" & Comments & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByTS_User(ByVal TS_User As String) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE TS_User = '" & TS_User & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByTS_DateTime(ByVal TS_DateTime As DateTime) As PatientsDS

            Dim result As New PatientsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT PatientID, PatientType, FirstName, LastName, DateOfBirth, Age, Gender, ExternalPID, ExternalArrivalDate, PerformedBy, Comments, TS_User, TS_DateTime FROM tparPatients  WHERE CONVERT(VARCHAR, TS_DateTime, 112) = '" & TS_DateTime.ToString("yyyyMMdd") & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparPatients)

            Catch ex As ArgumentException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparPatientsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

#End Region 'READ BY


#End Region 'CRUD
#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

