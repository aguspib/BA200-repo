Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class HelpFilesSettingDelegate

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTypeID"></param>
        ''' <param name="pLanguage"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/11/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, _
                             ByVal pTypeID As GlobalEnumerates.HELP_FILE_TYPE, _
                             ByVal pLanguage As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myHelpFileSettingDAO As New tfmwHelpFilesSettingDAO
                        myGlobalDataTO = myHelpFileSettingDAO.Read(dbConnection, pTypeID, pLanguage)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HelpFilesSettingDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

    End Class

End Namespace
