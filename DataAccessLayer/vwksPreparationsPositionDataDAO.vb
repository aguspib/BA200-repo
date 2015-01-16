
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class vwksPreparationsPositionDataDAO
        Inherits DAOBase

        Public Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal pOrderTestID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""

                        cmdText += " SELECT * "
                        cmdText += " FROM  vwksPreparationsPositionData "
                        cmdText += " WHERE OrderTestID = " & pOrderTestID

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'Dim myPreparationsPositionDataDS As New PreparationsPositionDataDS
                        'dbDataAdapter.Fill(myPreparationsPositionDataDS.vwksPreparationsPositionData)

                        Dim myPreparationsPositionDataDS As New PreparationsPositionDataDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPreparationsPositionDataDS.vwksPreparationsPositionData)
                            End Using
                        End Using

                        resultData.SetDatos = myPreparationsPositionDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksPreparationsPositionDataDAO.ReadByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function




    End Class


End Namespace
