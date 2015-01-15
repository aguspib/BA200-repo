

Option Explicit On
Option Strict On


Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Partial Public Class TwksOrdersDAO
        Inherits DAOBase

#Region "AUTOMATICALLY GENERATED CODE"

#Region "CRUD"



#Region "UPDATE"

        Public Function Update(ByVal twksorders As OrdersDS, ByVal Conn As SqlConnection, ByVal Transaccion As SqlTransaction) As GlobalDataTO

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
                For Each twksordersDR As DataRow In twksorders.twksorders
                    values &= "OrderID = "
                    If twksordersDR("OrderID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("OrderID").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "SampleClass = "
                    If twksordersDR("SampleClass") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("SampleClass").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "StatFlag = "
                    If twksordersDR("StatFlag") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("StatFlag").ToString() _
                           & "',"
                    End If

                    values &= "PatientID = "
                    If twksordersDR("PatientID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "N'" & twksordersDR("PatientID").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "OrderDateTime = "
                    If twksordersDR("OrderDateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("OrderDateTime").ToString() _
                        & "',"
                    End If

                    values &= "SequenceNumber = "
                    If twksordersDR("SequenceNumber") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= twksordersDR("SequenceNumber").ToString() _
                         & ","
                    End If

                    values &= "OrderStatus = "
                    If twksordersDR("OrderStatus") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("OrderStatus").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "ExternalOID = "
                    If twksordersDR("ExternalOID") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("ExternalOID").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "ExternalDateTime = "
                    If twksordersDR("ExternalDateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("ExternalDateTime").ToString() _
                        & "',"
                    End If

                    values &= "ExportDateTime = "
                    If twksordersDR("ExportDateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("ExportDateTime").ToString() _
                        & "',"
                    End If

                    values &= "TS_User = "
                    If twksordersDR("TS_User") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("TS_User").ToString().Replace("'", "''") _
                        & "',"
                    End If

                    values &= "TS_DateTime = "
                    If twksordersDR("TS_DateTime") Is DBNull.Value Then
                        values &= "NULL,"
                    Else
                        values &= "'" & twksordersDR("TS_DateTime").ToString() _
                       & "'"
                    End If


                    cmdText = "UPDATE twksOrders SET " & values & _
                    " WHERE " & _
                        "OrderID = '" & twksordersDR("OrderID").ToString() & "'"
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
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                MyGlobalTo.HasError = True
                MyGlobalTo.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()
            End Try
            Return MyGlobalTo
        End Function

#End Region 'UPDATE

#Region "READ ALL"

        Public Function ReadAll() As OrdersDS

            Dim result As New OrdersDS()
            Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
            Try
                Dim cmdText As String = "SELECT OrderID, SampleClass, StatFlag, PatientID, OrderDateTime, SequenceNumber, OrderStatus, ExternalOID, ExternalDateTime, ExportDateTime, TS_User, TS_DateTime FROM twksOrders"

                Dim cmd As SqlCommand = connectionNew.CreateCommand()
                cmd.CommandText = cmdText
                cmd.Connection = connectionNew

                Dim da As New SqlClient.SqlDataAdapter(cmd)
                da.Fill(result.twksOrders)

            Catch ex As ArgumentException
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Catch ex As InvalidOperationException
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

            Finally
                'always call Close when done with conn.
                connectionNew.Close()

            End Try
            Return result
        End Function

#End Region 'READ ALL

#Region "READ BY"

        'AG 29/08/2011 - comment method
        'Public Function ReadByExternalOID(ByVal ExternalOID As String) As OrdersDS

        '    Dim result As New OrdersDS
        '    Dim connectionNew As New SqlClient.SqlConnection(GetConnectionString())
        '    Dim cmd As SqlCommand
        '    Dim cmdText As String = ""
        '    Try

        '        cmdText = "SELECT OrderID, SampleClass, StatFlag, PatientID, OrderDateTime, SequenceNumber, OrderStatus, ExternalOID, ExternalDateTime, ExportDateTime, TS_User, TS_DateTime FROM twksOrders  WHERE ExternalOID = '" & ExternalOID & "'"

        '        cmd = connectionNew.CreateCommand()
        '        cmd.CommandText = cmdText
        '        cmd.Connection = connectionNew

        '        Dim da As New SqlClient.SqlDataAdapter(cmd)
        '        da.Fill(result.twksOrders)

        '    Catch ex As ArgumentException
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

        '    Catch ex As InvalidOperationException
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TwksOrdersDAO", EventLogEntryType.Error, False)

        '    Finally
        '        ' always call Close when done with conn.
        '        connectionNew.Close()
        '    End Try

        '    Return result

        'End Function

#End Region 'READ BY

#End Region 'CRUD
#End Region 'AUTOMATICALLY GENERATED CODE"

    End Class
End Namespace

