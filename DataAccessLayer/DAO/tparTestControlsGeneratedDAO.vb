

    Option Explicit On
    Option Strict On
    
        
    Imports System.Data.SqlClient
    Imports Biosystems.Ax00.Types
    Imports Biosystems.Ax00.Global


namespace Biosystems.Ax00.DAL.DAO
  

    Partial Public Class TparTestControlsGeneratedDAO
        Inherits DAOBase

#Region "AUTOMATICALLY GENERATED CODE"



#Region "CRUD Methods"






#End Region

#Region "CRUD"

#Region "CREATE"

        Public Function Create(ByVal tpartestControls As TestControlsDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO


            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Dim keys As String = "(TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime)"
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
                For Each tpartestControlsDR As DataRow In tpartestControls.tparTestControls
                    If tpartestControlsDR("TestID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= tpartestControlsDR("TestID").ToString() & ","
                    End If

                    If tpartestControlsDR("SampleType") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestControlsDR("SampleType").ToString().Replace("'", "''") & "',"
                    End If

                    If tpartestControlsDR("ControlID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= tpartestControlsDR("ControlID").ToString() & ","
                    End If

                    If tpartestControlsDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestControlsDR("TS_User").ToString().Replace("'", "''") & "',"
                    End If

                    If tpartestControlsDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= " '" & tpartestControlsDR("TS_DateTime").ToString() & "'"
                    End If

                    cmdText = "INSERT INTO tparTestControls  " & keys & " VALUES (" & values & ")"
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
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'CREATE

#Region "READ"

        Public Function Read(ByVal TestID As Integer, ByVal SampleType As String, ByVal ControlID As Integer) As TestControlsDS

            Dim conn As New SqlClient.SqlConnection(GetConnectionString())
            Dim result As New TestControlsDS()
            Try
                conn.Open()
                Dim cmdText As String = _
                "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime " & _
                "FROM tparTestControls " & " WHERE " & "TestID = " & TestID.ToString() & " AND " & "SampleType = '" & SampleType.ToString() & " AND " & "ControlID = " & ControlID.ToString()

                Dim cmd As SqlClient.SqlCommand = conn.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = conn
                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)
            Finally
                conn.Close()
            End Try
            Return result
        End Function

#End Region 'READ

#Region "UPDATE"

        Public Function Update(ByVal tpartestControls As TestControlsDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO

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
                For Each tpartestControlsDR As DataRow In tpartestControls.tparTestControls
                    values &= "TestID = "
                    If tpartestControlsDR("TestID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= tpartestControlsDR("TestID").ToString() _
                         & ","
                    End If

                    values &= "SampleType = "
                    If tpartestControlsDR("SampleType") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestControlsDR("SampleType").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "ControlID = "
                    If tpartestControlsDR("ControlID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= tpartestControlsDR("ControlID").ToString() _
                         & ","
                    End If

                    values &= "ControlNum = "
                    values &= "MinConcentration = "
                    values &= "MaxConcentration = "
                    values &= "TS_User = "
                    If tpartestControlsDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestControlsDR("TS_User").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "TS_DateTime = "
                    If tpartestControlsDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & tpartestControlsDR("TS_DateTime").ToString() _
                       & "'"
                    End If


                    cmdText = "UPDATE tparTestControls SET " & values & _
                    " WHERE " & _
                        "TestID = " & tpartestControlsDR("TestID").ToString() & " AND " & _
                        "SampleType = '" & tpartestControlsDR("SampleType").ToString() & "' AND " & _
                        "ControlID = " & tpartestControlsDR("ControlID").ToString()
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
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'UPDATE

#Region "READ ALL"

        Public Function ReadAll() As TestControlsDS

            Dim result As New TestControlsDS()
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Try
                Dim cmdText As String = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls"

                Dim cmd As SqlCommand = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()

            End Try
            Return result
        End Function

#End Region 'READ ALL

#Region "READ BY"

        '''ALL READ BY


        Public Function ReadByTestID(ByVal TestID As Integer) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""

            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls WHERE TestID = " & TestID.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadBySampleType(ByVal SampleType As String) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls  WHERE SampleType = '" & SampleType & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByControlID(ByVal ControlID As Integer) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls WHERE ControlID = " & ControlID.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByControlNum(ByVal ControlNum As Byte) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls WHERE ControlNum = " & ControlNum.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByMinConcentration(ByVal MinConcentration As Single) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls WHERE MinConcentration = " & MinConcentration.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByMaxConcentration(ByVal MaxConcentration As Single) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls WHERE MaxConcentration = " & MaxConcentration.ToString()

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByTS_User(ByVal TS_User As String) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls  WHERE TS_User = '" & TS_User & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Finally
                ' always call Close when done with conn.
                connectionNew.Close()
            End Try

            Return result

        End Function

        Public Function ReadByTS_DateTime(ByVal TS_DateTime As DateTime) As TestControlsDS

            Dim result As New TestControlsDS
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Dim cmd As SqlCommand
            Dim cmdText As String = ""
            Try

                cmdText = "SELECT TestID, SampleType, ControlID, ControlNum, MinConcentration, MaxConcentration, TS_User, TS_DateTime FROM tparTestControls  WHERE CONVERT(VARCHAR, TS_DateTime, 112) = '" & TS_DateTime.ToString("yyyyMMdd") & "'"

                cmd = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.tparTestControls)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TparTestControlsDAO", EventLogEntryType.Error, False)

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
    End NameSpace

  