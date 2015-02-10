Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.DAL

Public Class IntegrityDAO
      

    ''' <summary>
    ''' Executes scripts in a Applic server.
    ''' </summary>
    ''' <param name="pSQLScripts">SQL Script to run</param>
    ''' <returns>The recived result from scalar property</returns>
    ''' <remarks>
    ''' CREATED BY: TR 25/01/2013
    ''' </remarks>
    Public Shared Function ExecuteScripts(ByVal pSQLScripts As String) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            myGlobalDataTO = GetOpenDBConnection(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = pSQLScripts

                    myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar

                End If
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        Finally
            dbConnection.Close()
        End Try

        Return myGlobalDataTO

    End Function

    ''' <summary>
    ''' Get Installed Database Version
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' CREATED BY: TR 25/01/2013
    ''' </remarks>
    Public Shared Function GetInstalledDBVersion() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            myGlobalDataTO = GetOpenDBConnection(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then

                    Dim CmdText As String = " SELECT DBSoftware "
                    CmdText &= " FROM [Ax00].[dbo].[tfmwVersions]"
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = CmdText

                    myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar

                End If
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobalDataTO.ErrorMessage = ex.Message
        Finally
            dbConnection.Close()
        End Try

        Return myGlobalDataTO

    End Function



End Class
