Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tfmwScreenBlocksDAO
          
#Region "Other Methods"
        ''' <summary>
        ''' Get the Screen Block details by informed Screen ID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>returns the Block details in GlobalDataTO</returns>
        ''' <remarks>
        ''' CREATED BY: BALAJI 10/12/2009 
        ''' Test: Inprogress
        ''' Tested BY : VR 11/01/2010 (Tested : OK)
        ''' Modified BY: DL 01/02/2010
        ''' </remarks>
        Public Function ReadByScreen(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim ScreenBlockDataDS As New ScreenBlockDS
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim cmdText As String = ""
                        cmdText = "Select * " & _
                                  " From tfmwScreenBlocks " & _
                                  " Where ScreenID = '" & pScreenID & "' "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(ScreenBlockDataDS.tfmwScreenBlocks)

                        resultData.SetDatos = ScreenBlockDataDS
                        resultData.HasError = False

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwScreenBlocksDAO.ReadByScreen", EventLogEntryType.Error, False)                
            Finally
                'When Database Connection was opened locally, it has to be closed 
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData
        End Function

    

#End Region

    End Class
End Namespace
