Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL
    Public Class OffSystemTestSamplesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Add a new offSystemTestSample
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pTestSampleTypesDS">Typed DataSet OffSystemTestSamplesDS containing the data of all SampleTypes
        '''                                  to link to an specific OFF-SYSTEM Test (currently only one is allowed by Test)</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by: DL 29/11/2010
        ''' </remarks>
        Public Function Add(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pTestSampleTypesDS As OffSystemTestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim offSystemTestSampleToAdd As New tparOffSystemTestSamplesDAO

                        For Each row As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow In pTestSampleTypesDS.tparOffSystemTestSamples.Rows
                            resultData = offSystemTestSampleToAdd.Create(dbConnection, row)
                            If (resultData.HasError) Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestSamplesDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update SampleType data for the specified OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pTestSampleTypesDS">Typed DataSet OffSystemTestSamplesDS containing the data of all SampleTypes
        '''                                  linked to an specific OFF-SYSTEM Test (currently only one is allowed by Test)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 29/11/2010
        ''' </remarks>
        Public Function Modify(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pTestSampleTypesDS As OffSystemTestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim OffSystemTestSampleToUpdate As New tparOffSystemTestSamplesDAO

                        For Each row As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow In pTestSampleTypesDS.tparOffSystemTestSamples.Rows
                            resultData = OffSystemTestSampleToUpdate.Update(dbConnection, row)
                            If (resultData.HasError) Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestSamplesDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all SampleTypes  linked to the specified OFF-SYSTEM Test (currently only a SampleType is allowed)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">OFF-SYSTEM Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 01/12/2010
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTestSamplesDAO As New tparOffSystemTestSamplesDAO
                        resultData = myOffSystemTestSamplesDAO.DeleteByTestID(dbConnection, pOffSystemTestID)

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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestSamplesDelegate.DeleteByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined SampleTypes for the specified OFF-SYSTEM Test. Besides, if a SampleType is informed,
        ''' then verify if the specified SampleType exists for the OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">OffSystemTest Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestSamplesDS with the list of Sample Types
        '''          linked to the specified OFF-SYSTEM Test</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' </remarks>
        Public Function GetListByOffSystemTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of Sample Types linked to the specified OFF-SYSTEM Test
                        Dim myOffSystemTestSamplesDAO As New tparOffSystemTestSamplesDAO
                        resultData = myOffSystemTestSamplesDAO.GetByOffSystemTestID(dbConnection, pOffSystemTestID, pSampleType)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "OffSystemTestSamplesDelegate.GetListByOffSystemTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class

End Namespace
