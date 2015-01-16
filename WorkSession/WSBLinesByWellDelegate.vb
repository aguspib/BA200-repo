Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Public Class WSBLinesByWellDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Create a group of BaseLines by Well
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLineDS">Typed DataSet BaseLinesDS containing all BaseLines to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBaseLineDS As BaseLinesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.Create(dbConnection, pBaseLineDS)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the informed BaseLineID already exists for the specified Analyzer WorkSession
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">BaseLine Identifier</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the BaseLine already exists or not</returns>
        ''' <remarks>
        ''' Created by:  AG 20/05/2010 
        ''' </remarks>
        Public Function Exists(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                               ByVal pBaseLineID As Integer, ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim existBaseline As Boolean = False

            Try
                resultData.SetDatos = existBaseline

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pWell)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myBaseLineDS As BaseLinesDS = DirectCast(resultData.SetDatos, BaseLinesDS)
                            If (myBaseLineDS.twksWSBaseLines.Rows.Count > 0) Then
                                existBaseline = True
                            End If
                        End If
                    End If
                End If

                resultData.SetDatos = existBaseline
                resultData.HasError = False
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.Exists", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Base Lines by Well used for an specific WorkSessionID and AnalyzerID
        ''' </summary>
        '''  <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  DL 31/05/2010 
        ''' </remarks>
        Public Function GetByWorkSession(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myWSBLinesbyWell As New twksWSBLinesByWellDAO
                        resultData = myWSBLinesbyWell.GetByWorkSession(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesDelegate.GetByWorkSession", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the ID of the last created BaseLine for the specified Analyzer, WorkSession and WellNumber in order to inform 
        ''' every WSExecution with his correct BaseLine
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pWell">Well Number</param>
        ''' <returns>GlobalDataTO containing and integer value with the ID of the last created BaseLine for the specified 
        '''          Analyzer, WorkSession and WellNumber</returns>
        ''' <remarks>
        ''' Created by:  AG 17/05/2010
        ''' </remarks>
        Public Function GetCurrentBaseLineID(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.GetCurrentBaseLineID(dbConnection, pAnalyzerID, pWorkSessionID, pWell)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.GetCurrentBaseLineID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search the last Well received for an Analyzer and WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing and integer value with the number of the last Well received for specified 
        '''          Analyzer WorkSession</returns>
        ''' <remarks>
        ''' Created by:  AG 31/07/2012
        ''' </remarks>
        Public Function GetLastWellBaseLineReceived(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Dim wellValue As Integer = 0

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.GetLastWellBaseLineReceived(dbConnection, pAnalyzerID, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myDS As BaseLinesDS = DirectCast(resultData.SetDatos, BaseLinesDS)

                            If (myDS.twksWSBaseLines.Rows.Count > 0) Then
                                wellValue = myDS.twksWSBaseLines.First.WellUsed
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.GetLastWellBaseLineReceived", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            resultData.SetDatos = wellValue
            Return resultData
        End Function

        ''' <summary>
        ''' Get all BaseLines by Well for the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        ''' </remarks>
        Public Function GetMeanWellBaseLineValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.GetMeanWellBaseLineValues(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.GetMeanWellBaseLineValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if data of the informed BaseLineID is completed (all fields informed) or not 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">BaseLine Identifier</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the BaseLine is completed or not</returns>
        ''' <remarks>
        ''' Created by:  AG 21/05/2010 
        ''' </remarks>
        Public Function IsComplete(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                   ByVal pBaseLineID As Integer, ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData.SetDatos = False

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pWell)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myBaseLineDS As BaseLinesDS = DirectCast(resultData.SetDatos, BaseLinesDS)

                            Dim blComplete As Boolean = True
                            For Each row As BaseLinesDS.twksWSBaseLinesRow In myBaseLineDS.twksWSBaseLines
                                If (row.IsAnalyzerIDNull) Then blComplete = False
                                If (row.IsWorkSessionIDNull) Then blComplete = False
                                If (row.IsBaseLineIDNull) Then blComplete = False
                                If (row.IsWavelengthNull) Then blComplete = False
                                If (row.IsWellUsedNull) Then blComplete = False
                                If (row.IsMainLightNull) Then blComplete = False
                                If (row.IsRefLightNull) Then blComplete = False

                                'These fields are not required in table twksWSBLinesByWell
                                'If row.IsMainDarkNull Then blComplete = False
                                'If row.IsRefDarkNull Then blComplete = False
                                'If row.IsITNull Then blComplete = False
                                'If row.IsDACNull Then blComplete = False

                                If (Not blComplete) Then Exit For
                            Next

                            resultData.SetDatos = blComplete
                            resultData.SetDatos = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.IsComplete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of all WaveLengths for the specified BaseLine by Well 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">Identifier of the Base Line</param>
        ''' <param name="pWell">Well Number</param>
        ''' <returns>GlobalDataTO containing a typed DataSet BaseLinesDS with the group of BaseLines by Well</returns>
        ''' <remarks>
        ''' Created by:  AG 14/05/2010 
        ''' Modified by: AG 20/05/2010 - Added parameters AnalyzerID, WorkSessionID and WellUsed
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                             ByVal pBaseLineID As Integer, ByVal pWell As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytwksWSBaseLines As New twksWSBLinesByWellDAO
                        resultData = mytwksWSBaseLines.Read(pDBConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pWell)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all BaseLines by Well for the specified Analyzer Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 14/05/2010 
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values of a group of BaseLines by Well
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBaseLineDS">Typed DataSet BaseLinesDS containing all BaseLines to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBaseLineDS As BaseLinesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.Update(dbConnection, pBaseLineDS)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field isMean
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pBaseLineID">BaseLine Identifier</param>
        ''' <param name="pIsMeanValue">Value to set to field isMean</param>
        ''' <param name="pWellsString">List of Well Numbers divided by comma character</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 04/05/2011
        ''' </remarks>
        Public Function UpdateIsMean(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                     ByVal pBaseLineID As Integer, ByVal pIsMeanValue As Boolean, ByVal pWellsString As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSBLinesByWellDAO
                        resultData = myDAO.UpdateIsMean(dbConnection, pAnalyzerID, pWorkSessionID, pBaseLineID, pIsMeanValue, pWellsString)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "WSBLinesByWellDelegate.UpdateIsMean", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
