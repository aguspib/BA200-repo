Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Class DBAdjustmentsDelegate

#Region "Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAdjDS"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 24/01/2012</remarks>
        Public Function CreateMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAdjDS As SRVAdjustmentsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAdjustments As New tfmwAdjustmentsDAO
                        resultData = myAdjustments.CreateMasterData(dbConnection, pAdjDS)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.CreateMasterData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AdjustmentsDS with data of the specified Adjustments</returns>
        ''' <remarks>Created by: XBC 04/10/2011</remarks>
        Public Function ReadAdjustmentsFromDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get data fields from the specified Analyzer
                        Dim myAdjustments As New tfmwAdjustmentsDAO
                        resultData = myAdjustments.GetMasterData(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.ReadAdjustmentsFromDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the contents of Adjustments table with the values received from the Instrument
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAdjustmentsDS">Contents to be updated in DB</param>
        ''' <returns></returns>
        ''' <remarks>Created by: XBC 04/10/2011</remarks>
        Public Function UpdateAdjustmentsDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAdjustments As New tfmwAdjustmentsDAO
                        resultData = myAdjustments.UpdateAdjustmentsDB(dbConnection, pAdjustmentsDS)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.UpdateAdjustmentsDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new contents of Adjustments for the connected Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns></returns>
        ''' <remarks>Created by: XBC 04/10/2011</remarks>
        Public Function InsertAdjustmentsDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pFwVersion As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAdjustments As New tfmwAdjustmentsDAO
                        resultData = myAdjustments.CreateMasterData(dbConnection, pAnalyzerID, pFwVersion)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.InsertAdjustmentsDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete contents of Adjustments by an specific Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAnalyzerID">Identifier of the Analyzer</param>
        ''' <returns></returns>
        ''' <remarks>Created by: XBC 17/10/2011</remarks>
        Public Function DeleteAdjustmentsDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAdjustments As New tfmwAdjustmentsDAO
                        resultData = myAdjustments.Delete(dbConnection, pAnalyzerID)

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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.DeleteAdjustmentsDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the contents of Adjustments table with the values of the current Adjustments read from the Instrument
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pAdjustmentIDold">Old Identifier of the Analyzer</param>
        ''' <param name="pAdjustmentIDnew">New Identifier of the Analyzer</param>
        ''' <returns></returns>
        ''' <remarks>Created by: XBC 25/06/2012</remarks>
        Public Function CopyCurrentAdjustments(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pAdjustmentIDold As String, _
                                               ByVal pAdjustmentIDnew As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Dim myAdjustments As New tfmwAdjustmentsDAO
                        Dim readAdjustmentsDS As New SRVAdjustmentsDS

                        'The current AnalyzerIdAttribute has contents in database? Yes do nothing. No create
                        resultData = MyClass.ReadAdjustmentsFromDB(dbConnection, pAdjustmentIDnew)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            readAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                            If readAdjustmentsDS.srv_tfmwAdjustments.Rows.Count = 0 Then
                                'If not exists: Read the Current Contents Adjustments and create for AnalyzerIdAttribute
                                resultData = MyClass.ReadAdjustmentsFromDB(dbConnection, pAdjustmentIDold)

                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    readAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                                    If readAdjustmentsDS.srv_tfmwAdjustments.Rows.Count > 0 Then
                                        For Each row As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In readAdjustmentsDS.srv_tfmwAdjustments.Rows
                                            row.BeginEdit()
                                            row.AnalyzerID = pAdjustmentIDnew
                                            row.EndEdit()
                                        Next
                                        readAdjustmentsDS.AcceptChanges()
                                        resultData = MyClass.CreateMasterData(dbConnection, readAdjustmentsDS)
                                    End If
                                End If

                            End If
                        End If


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
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AdjustmentsDelegate.CopyCurrentAdjustments", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace
