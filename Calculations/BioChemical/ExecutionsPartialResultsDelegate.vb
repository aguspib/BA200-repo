Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Partial Public Class ExecutionsPartialResultsDelegate

        '''' <summary>
        '''' Save results 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <returns>GlobalDataTO with set data as ExecutionsPartialResultsDS</returns>
        '''' <remarks>
        '''' Created by GDS 03/03/2010
        '''' </remarks>
        'Public Function SaveResults(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                            ByVal pExecutionsPartialResultsDS As ExecutionsPartialResultsDS) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        Dim mytcalcExecutionsPartialResultsDAO As New tcalcExecutionsPartialResultsDAO

        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                For Each myRow As ExecutionsPartialResultsDS.tcalcExecutionsPartialResultsRow In pExecutionsPartialResultsDS.tcalcExecutionsPartialResults.Rows
        '                    resultData = mytcalcExecutionsPartialResultsDAO.ExistsResult(dbConnection, myRow)

        '                    If (Not resultData.HasError) Then
        '                        If DirectCast(resultData.SetDatos, ExecutionsPartialResultsDS).tcalcExecutionsPartialResults.Rows.Count > 0 Then
        '                            resultData = mytcalcExecutionsPartialResultsDAO.UpdateResults(dbConnection, myRow)
        '                        Else
        '                            resultData = mytcalcExecutionsPartialResultsDAO.InsertResult(dbConnection, myRow)
        '                        End If
        '                    End If

        '                    If resultData.HasError Then
        '                        Exit For
        '                    End If
        '                Next

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) And _
        '           (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ExecutionsPartialResultsDelegate.SaveResults", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData

        'End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, _
                                ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tcalcExecutionsPartialResultsDAO

                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ExecutionsPartialResultsDelegate.ResetWS", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

    End Class

End Namespace
