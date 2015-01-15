Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwScreensDAO
        Inherits DAOBase

        ''' <summary>
        ''' Readtable tfmwScreens Table by the ScreenID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pScreenID"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 07/11/2011</remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT * " & Environment.NewLine
                        cmdText &= " FROM tfmwScreens " & Environment.NewLine
                        cmdText &= " WHERE ScreenID ='" & pScreenID & "'" & Environment.NewLine

                        Dim myScreenDS As New ScreenDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myScreenDS.tfmwScreens)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myScreenDS
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwScreensDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


    End Class

End Namespace
