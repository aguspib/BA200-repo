Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Partial Public Class twksImportErrorsLogDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get all errors found during import of a LIMS file
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ImportErrorsLogDS with the list of errors found
        '''          during import of a LIMS file</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2010
        ''' Modified by: DL 08/10/2010 - Changed the query to implement multilanguage
        '''              SA 25/10/2010 - Fixed bad implementation of multilanguange
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the current Application Language
                        Dim myGlobalBase As New GlobalBase
                        Dim appLanguage As String = GlobalBase.GetSessionInfo.ApplicationLanguage()

                        Dim cmdText As String = ""
                        cmdText = " SELECT IE.ImportID, IE.ImportDate, IE.ErrorCode, IE.LineNumber, IE.LineText, MR.ResourceText AS ErrorMessage " & _
                                  " FROM   twksImportErrorsLog IE INNER JOIN tfmwMessages M ON IE.ErrorCode = M.MessageID " & _
                                                                " INNER JOIN tfmwMultiLanguageResources MR ON MR.ResourceID = M.ResourceID " & _
                                  " WHERE  MR.LanguageID = '" & appLanguage & "' " & _
                                  " ORDER BY IE.LineNumber "

                        Dim cmd As SqlCommand = dbConnection.CreateCommand()
                        cmd.CommandText = cmdText
                        cmd.Connection = dbConnection

                        Dim myImportErrorsDS As New ImportErrorsLogDS
                        Dim sqlAdapter As New SqlClient.SqlDataAdapter(cmd)
                        sqlAdapter.Fill(myImportErrorsDS.twksImportErrorsLog)

                        resultData.SetDatos = myImportErrorsDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksImportErrorsLogDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add all errors generated during reading and processing of a LIMS file in an Import from LIMS process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pImportErrors">Typed DataSet ImportErrorsLogDS containing the list of Import Errors
        '''                             to add to the Import from LIMS process log table</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2010
        ''' Modified by: SA 28/10/2010 - Added N preffix for multilanguage of field LineText
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pImportErrors As ImportErrorsLogDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    For Each importError As ImportErrorsLogDS.twksImportErrorsLogRow In pImportErrors.twksImportErrorsLog
                        cmdText = " INSERT INTO twksImportErrorsLog (ImportDate, ErrorCode, LineNumber, LineText) " & _
                                  " VALUES ('" & Now.ToString("yyyyMMdd HH:mm:ss") & "', '" & importError.ErrorCode & "', "

                        If (Not importError.IsLineNumberNull) Then
                            cmdText &= importError.LineNumber & ", "
                        Else
                            cmdText &= "NULL, "
                        End If
                        If (Not importError.IsLineTextNull) Then
                            cmdText &= "N'" & importError.LineText.Replace("'", "''") & "') "
                        Else
                            cmdText &= "NULL) "
                        End If

                        Dim cmd As New SqlCommand
                        cmd.Connection = pDBConnection
                        cmd.CommandText = cmdText

                        resultData.AffectedRecords = cmd.ExecuteNonQuery()
                        If (resultData.AffectedRecords = 0) Then
                            resultData.HasError = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksImportErrorsLogDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all import errors that exist in the log table used for the Import from LIMS process
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2010
        ''' </remarks>
        Public Function DeleteAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM twksImportErrorsLog "

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText
                    cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "twksImportErrorsLogDAO.DeleteAll", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace