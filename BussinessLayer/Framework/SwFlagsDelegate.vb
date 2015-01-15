Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL



Namespace Biosystems.Ax00.BL

    Public Class SwFlagsDelegate

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>AG 25/02/2011 - Tested OK</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tfmwSwFlagsDAO
                        resultData = myDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SwFlagsDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
    End Class

End Namespace
