Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports System.Text
Imports Biosystems.Ax00.Types
Imports System.Data.SqlClient
Imports System.Globalization

Namespace Biosystems.Ax00.Global.DAL
    Public Class tfmwApplicationLogDAO
        Inherits DAOBase

        ''' <summary>
        ''' Insert log information into database on table tfmApplicationLogDAO.
        ''' </summary>
        ''' <param name="pApplicationLogTOList"></param>
        ''' <returns></returns>
        ''' <remarks> 
        ''' Created by: TR 03/07/2012
        ''' Modified by DL 16/07/2012 - Solve ErrorCode -2146232060
        '''             XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
        '''             SG 25/07/2013 - Include miliseconds in DateTime format (http://msdn.microsoft.com/es-es/library/8tfzyc64.aspx)
        ''' </remarks>
        Public Function Create(ByVal pApplicationLogTOList As List(Of ApplicationLogTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDBConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            Try
                Dim cmdText As New StringBuilder
                Using dbCmd As New SqlClient.SqlCommand
                    myDBConnection.Open()

                    dbCmd.Connection = myDBConnection
                    For Each myApplicationLog As ApplicationLogTO In pApplicationLogTOList
                        cmdText.Append("INSERT INTO tfmwApplicationLog ")
                        cmdText.Append("(LogDateTime, Message, LogType, Module, ThreadId)")
                        cmdText.AppendFormat(" VALUES('{0}', N'{1}', '{2}', '{3}', {4})", _
                                             myApplicationLog.LogDate.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), _
                                             myApplicationLog.LogMessage.Replace("'", "''"), _
                                             myApplicationLog.LogType, _
                                             myApplicationLog.LogModule, _
                                             myApplicationLog.LogThreadId)

                        'myApplicationLog.LogDate.ToString("yyyy-MM-dd HH:mm:ss:fff"), _

                        dbCmd.CommandText = cmdText.ToString()

                        'DL 16/07/2012. Begin
                        Try
                            myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                            cmdText.Length = 0

                        Catch ex As SqlException

                            ' After restore DB, the first time to connect success the next error: 
                            ' -2146232060 Error en el nivel de transporte al enviar la solicitud al servidor. (provider: Proveedor de 
                            '             memoria compartida, error: 0 - No hay ningún proceso en el otro extremo de la canalización.)
                            ' To Solve this BUG only need to try open db connection.

                            If ex.ErrorCode = -2146232060 AndAlso ex.Errors(0).Number = 233 Then
                                myDBConnection.Open()

                                myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                                cmdText.Length = 0

                                'resultData.SetDatos = myMessages
                                myGlobalDataTO.HasError = False

                            Else
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                                myGlobalDataTO.ErrorMessage = ex.Message

                                Dim myLogAcciones As New ApplicationLogManager()
                                myLogAcciones.CreateLogActivity(ex.Message, "tfmwApplicationLogDAO.Create", EventLogEntryType.Error, False)
                            End If
                        End Try
                        'DL 16/07/2012. End

                    Next
                End Using

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            Finally
                'Close the open connection
                myDBConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Insert log information into database on table tfmApplicationLogDAO using an Strored Procedure.
        ''' </summary>
        ''' <param name="pApplicationLogTOList"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/07/2012</remarks>
        Public Function CreateWitStoreProcedure(ByVal pApplicationLogTOList As List(Of ApplicationLogTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDBConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            Try
                Using dbCmd As New SqlClient.SqlCommand
                    myDBConnection.Open()
                    dbCmd.Connection = myDBConnection
                    Dim myParameters As New SqlClient.SqlParameter
                    For Each myApplicationLog As ApplicationLogTO In pApplicationLogTOList

                        dbCmd.CommandText = "sp_CreateLogActivity"
                        dbCmd.CommandType = CommandType.StoredProcedure
                        dbCmd.Parameters.AddWithValue("@LogDateTime", myApplicationLog.LogDate)
                        dbCmd.Parameters.AddWithValue("@Message", myApplicationLog.LogMessage)
                        dbCmd.Parameters.AddWithValue("@LogType", myApplicationLog.LogType)
                        dbCmd.Parameters.AddWithValue("@Module", myApplicationLog.LogModule)
                        dbCmd.Parameters.AddWithValue("@ThreadId", myApplicationLog.LogThreadId)

                        'Execute procedure
                        dbCmd.ExecuteNonQuery()

                        'Clear all parameters
                        dbCmd.Parameters.Clear()
                    Next

                End Using
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            Finally
                'Close the open connection
                myDBConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Read all records on tfmwApplicationLog table order by logDateTime.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/07/2012</remarks>
        Public Function ReadAll() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDBConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            Try
                Dim cmdText As New StringBuilder
                Using dbCmd As New SqlClient.SqlCommand
                    myDBConnection.Open()
                    dbCmd.Connection = myDBConnection

                    cmdText.Append("SELECT LogDateTime, Message, LogType, Module, ThreadId")
                    cmdText.Append(" FROM tfmwApplicationLog ")
                    cmdText.Append(" ORDER BY LogDateTime ASC")

                    dbCmd.CommandText = cmdText.ToString()
                    Dim myApplicationLogDS As New ApplicationLogDS
                    Dim myDA As New SqlClient.SqlDataAdapter(dbCmd)

                    myDA.Fill(myApplicationLogDS.tApplicationLog)

                    cmdText.Length = 0

                    myGlobalDataTO.SetDatos = myApplicationLogDS

                End Using

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            Finally
                'Close the open connection
                myDBConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Export log to xml
        ''' </summary>
        ''' <param name="pfilename"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: DL 03/07/2012</remarks>
        Public Function ExportLogToXml(ByVal pFileName As String) As GlobalDataTO 'ByVal pApplicationLogTOList As List(Of ApplicationLogTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDBConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())

            Try
                Using dbCmd As New SqlClient.SqlCommand
                    myDBConnection.Open()
                    dbCmd.Connection = myDBConnection
                    'Dim myParameters As New SqlClient.SqlParameter
                    'For Each myApplicationLog As ApplicationLogTO In pApplicationLogTOList
                    dbCmd.CommandText = "sp_ExportLogToXML"
                    dbCmd.CommandType = CommandType.StoredProcedure
                    dbCmd.Parameters.AddWithValue("@FileName", pFileName)

                    'Execute procedure
                    dbCmd.ExecuteNonQuery()

                    'Clear all parameters
                    dbCmd.Parameters.Clear()
                    'Next
                End Using

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            Finally
                'Close the open connection
                myDBConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Delete all the log entry on table tfmwApplicationLog.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by:  DL 03/07/2012</remarks>
        Public Function DeleteAll() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDBConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            Try
                Dim cmdText As New StringBuilder

                Using dbCmd As New SqlClient.SqlCommand
                    myDBConnection.Open()
                    dbCmd.Connection = myDBConnection

                    cmdText.Append("TRUNCATE TABLE tfmwApplicationLog ")
                    dbCmd.CommandText = cmdText.ToString()
                    myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()

                    cmdText.Length = 0

                End Using

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwApplicationLogDAO.Delete", EventLogEntryType.Error, False)
            Finally
                'Close the open connection
                myDBConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ' Problems with Regional Settings Date format because do not use Invariant format
        ' ''' <summary>
        ' ''' Delete all records by date
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pMonthsToDelete">months to delete</param>
        ' ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  XB 13/11/2013
        ' ''' </remarks>
        'Public Function DeleteByDate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMonthsToDelete As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM tfmwApplicationLog " & vbCrLf & _
        '                                    " WHERE  DATEDIFF(MONTH, LogDateTime,  GETDATE()) > " & pMonthsToDelete & vbCrLf

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tfmwApplicationLogDAO.DeleteByDate", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

    End Class

End Namespace

