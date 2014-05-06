Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwHelpFilesSettingDAO
        Inherits DAOBase

        ''' <summary>
        ''' Read the table tfmwHelpFilesSetting by the ID and the Language.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pID"></param>
        ''' <param name="pLanguage"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/11/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pID As GlobalEnumerates.HELP_FILE_TYPE, _
                                            ByVal pLanguage As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT TypeID, Language, HelpFileName " & Environment.NewLine
                        cmdText &= " FROM tfmwHelpFilesSetting "
                        cmdText &= " WHERE TypeID = '" & pID.ToString & "' " & Environment.NewLine
                        cmdText &= " AND Language = '" & pLanguage & "' " & Environment.NewLine

                        Using dbCmd As New SqlClient.SqlCommand
                            dbCmd.Connection = dbConnection
                            dbCmd.CommandText = cmdText
                            'Fill the DataSet to return 
                            Dim myHelpFilesSettingDS As New HelpFilesSettingDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHelpFilesSettingDS.tfmwHelpFilesSetting)
                                myGlobalDataTO.SetDatos = myHelpFilesSettingDS
                            End Using
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwHelpFilesSettingDAO.ReadByIDAndLanguage", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


    End Class

End Namespace
