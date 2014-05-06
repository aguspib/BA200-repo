Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL
    Partial Public Class CurveResultsDelegate


        ''' <summary>
        ''' Find inside the last CurveResultsID and return the next free one identifier
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with dataset as INTEGER</returns>
        ''' <remarks>
        ''' Created by AG 02/03/2010 (Tested Pending)
        ''' </remarks>
        Public Function FindNextID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksCurveResultsDAO

                        resultData = myDAO.FindLastCurveID(dbConnection)

                        If Not resultData.HasError Then
                            Dim lastCurveId As Integer = 1

                            If Not resultData.SetDatos Is Nothing And Not resultData.SetDatos Is DBNull.Value Then
                                lastCurveId = CType(resultData.SetDatos, Integer) + 1
                            End If 'If Not resultData.SetDatos Is Nothing Then

                            resultData.SetDatos = lastCurveId
                        End If 'If Not resultData.HasError Then
                    End If 'If (Not dbConnection Is Nothing) Then

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.FindNextID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Save results 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTO with set data as CurveResultsDS</returns>
        ''' <remarks>
        ''' Created by GDS 03/03/2010
        ''' </remarks>
        Public Function SaveResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                    ByVal pCurveResultsDS As CurveResultsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                Dim mytwksCurveResultDAO As New twksCurveResultsDAO()

                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        For Each myRow As CurveResultsDS.twksCurveResultsRow In pCurveResultsDS.twksCurveResults.Rows
                            resultData = mytwksCurveResultDAO.ExistsCurveResult(dbConnection, myRow)

                            If (Not resultData.HasError) Then
                                If DirectCast(resultData.SetDatos, CurveResultsDS).twksCurveResults.Rows.Count > 0 Then
                                    resultData = mytwksCurveResultDAO.UpdateCurve(dbConnection, myRow)
                                Else
                                    resultData = mytwksCurveResultDAO.InsertCurve(dbConnection, myRow)
                                End If
                            End If

                            If resultData.HasError Then
                                Exit For
                            End If
                        Next

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
                If (pDBConnection Is Nothing) And _
                   (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.SaveResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Get all records for pCurveResultsID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with dataset as CurveResultsDS</returns>
        ''' <remarks>
        ''' Created by GDS 03/03/2010
        ''' </remarks>
        Public Function GetResults(ByVal pDBConnection As SqlClient.SqlConnection, _
                                   ByVal pCurveResultsID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksCurveResultsDAO As New twksCurveResultsDAO

                        resultData = mytwksCurveResultsDAO.ReadCurve(dbConnection, pCurveResultsID)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.GetResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return resultData

        End Function


        ''' <summary>
        ''' Delete the curve results by OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <returns>GlobalDataTO indicating success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 09/03/2010
        ''' Modified by: SA 16/04/2012 - Changed the function template
        ''' </remarks>
        Public Function DeleteByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestId As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete curve results from table twksCurveResults
                        Dim myDAO As New twksCurveResultsDAO
                        resultData = myDAO.DeleteResultsByOrderTestId(dbConnection, pOrderTestId)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.DeleteByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete curve results 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCurveID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by AG 09/06/2010 (Tested pending)</remarks>
        Public Function DeleteCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCurveID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Delete curve results from table twksCurveResults
                        Dim myDAO As New twksCurveResultsDAO
                        resultData = myDAO.DeleteCurve(dbConnection, pCurveID)

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
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.DeleteCurve", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete curve results for all NOTCALC Calibrators 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  JB 10/10/2012
        ''' </remarks>
        Public Function DeleteForNOTCALCCalibrators(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'Delete curve results from table twksCurveResults
                        Dim myDAO As New twksCurveResultsDAO
                        resultData = myDAO.DeleteForNOTCALCCalibrators(dbConnection)

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
                myLogAcciones.CreateLogActivity(ex.Message, "CurveResultsDelegate.DeleteForNOTCALCCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

    End Class

End Namespace

