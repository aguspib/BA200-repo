Imports Biosystems.Ax00.Global

Public Class IntegrityDelegate

    ''' <summary>
    ''' Get Installed Database Version
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 25/01/2013
    ''' </remarks>
    Public Function GetInstalledDBVersion() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = IntegrityDAO.GetInstalledDBVersion()
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        End Try
        Return myGlobalDataTO
    End Function

    Public Function ExecuteScripts(ByVal pSQLScripts As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = IntegrityDAO.ExecuteScripts(pSQLScripts)

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        End Try
        Return myGlobalDataTO
    End Function

End Class
