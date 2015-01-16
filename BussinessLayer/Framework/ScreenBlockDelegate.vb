Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL
    Public Class ScreenBlockDelegate
        ''' <summary>
        ''' Get Block Description Informed Screen
        ''' </summary>
        ''' <param name="pScreenID"></param>
        ''' <returns>returns Block status</returns>
        ''' <remarks>
        ''' CREATED BY : BALAJI - 10/12/2009
        ''' TEST: Inprogress
        ''' Tested BY : VR 11/01/2010 (Tested : OK)
        ''' Modified by: AG 15/01/2010 - Use the delegate template for SELECT (Tested OK)
        ''' </remarks>
        Public Function GetBlocksByScreen(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pScreenID As String) As GlobalDataTO
            'AG 15/01/2010
            'Dim myGlobalDataTO As New GlobalDataTO
            ''Dim result As New ScreenBlockDS
            'Try
            '    Dim mytfmwScreenBlocksDAO As New tfmwScreenBlocksDAO
            '    myGlobalDataTO = mytfmwScreenBlocksDAO.ReadByScreen(Nothing, pScreenID)

            '    'result = CType(myGlobalDataTO.SetDatos, ScreenBlockDS)

            'Catch ex As Exception
            '    Dim myLogAcciones As New ApplicationLogManager()
            '    GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockDelegate.GetBlocksByScreen", EventLogEntryType.Error, False)
            '    Throw ex
            'End Try
            'Return myGlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytfmwScreenBlocksDAO As New tfmwScreenBlocksDAO
                        resultData = mytfmwScreenBlocksDAO.ReadByScreen(dbConnection, pScreenID)

                        'result = CType(myGlobalDataTO.SetDatos, ScreenBlockDS)

                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockDelegate.GetBlocksByScreen", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

    End Class
End Namespace