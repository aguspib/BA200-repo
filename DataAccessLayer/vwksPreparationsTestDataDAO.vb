Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class vwksPreparationsTestDataDAO
          

        ''' <summary>
        ''' Get all data contained in view vwksPreparationsTestData for the specified OrderTest Identifier
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Identifier of the Order Test ID</param>
        ''' <returns>GlobalDataTO containing a Typed DataSet PreparationsTestDataDS with all data of the
        '''          specified OrderTest Identifier </returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' </remarks>
        Public Shared Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText += " SELECT * "
                        cmdText += " FROM  vwksPreparationsTestData "
                        cmdText += " WHERE OrderTestID = " & pOrderTestID

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'Dim myPreparationsTestDataDS As New PreparationsTestDataDS
                        'dbDataAdapter.Fill(myPreparationsTestDataDS.vwksPreparationsTestData)

                        Dim myPreparationsTestDataDS As New PreparationsTestDataDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myPreparationsTestDataDS.vwksPreparationsTestData)
                            End Using
                        End Using

                        resultData.SetDatos = myPreparationsTestDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "vwksPreparationsTestDataDAO.ReadByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
    End Class

End Namespace

