Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class ISETestSamplesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' For the specified ISE Test/SampleType, get all data needed to export it to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCTestSamples with all data needed to export the ISE Test/SampleType to QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Public Function GetDefinitionForQCModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETestSamplesDAO As New tparISETestSamplesDAO
                        resultData = myISETestSamplesDAO.GetDefinitionForQCModule(dbConnection, pISETestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.GetDefinitionForQCModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE TestSamples for the specified ISE Test. Besides, if a SampleType is informed,
        ''' then verify if the specified SampleType exists for the ISE Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISETest Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestSamplesDS with the list of ISE Test Samples</returns>
        ''' <remarks>
        ''' Created by:  XBC 18/10/2010
        ''' Modified by: SA  25/10/2010 - Added optional parameter pSampleType, to allow verify if an specific SampleType is used
        '''                               for the informed ISE Test. 
        ''' </remarks>
        Public Function GetListByISETestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, _
                                           Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of defined ISE Test Samples
                        Dim myISETestSamplesDAO As New tparISETestSamplesDAO
                        resultData = myISETestSamplesDAO.GetByISETestID(dbConnection, pISETestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.GetListByISETestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified ISETestSamples
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pISETestSamplesDetails">Typed DataSet ISETestSamplesDS containing the data of the ISETestSamples to be updated</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 15/10/2010
        ''' Modified by: AG  27/10/2010 - Perform loop and pass row to dao
        ''' </remarks>
        Public Function Modify(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestSamplesDetails As ISETestSamplesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ISETestSampleToUpdate As New tparISETestSamplesDAO

                        For Each row As ISETestSamplesDS.tparISETestSamplesRow In pISETestSamplesDetails.tparISETestSamples.Rows
                            resultData = ISETestSampleToUpdate.Update(dbConnection, row)
                            If resultData.HasError Then Exit For
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
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update value of field NumberOfControls for the specified ISETest/SampleType according the number of Controls linked to the it 
        ''' in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/06/2012
        ''' </remarks>
        Public Function UpdateNumOfControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETestSamplesDAO As New tparISETestSamplesDAO()
                        resultData = myISETestSamplesDAO.UpdateNumOfControls(dbConnection, pISETestID, pSampleType)

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

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.UpdateNumOfControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update QC values (CalculationMode, NumberOfSeries and RejectionCriteria) for an specific ISETest/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed Dataset HistoryTestSamplesDS containing the information to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2012
        ''' </remarks>
        Public Function UpdateQCValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestSamplesDAO
                        resultData = myDAO.UpdateQCValues(dbConnection, pHistoryTestSamplesDS)

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.UpdateQCValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "HISTORY methods"
        ''' <summary>
        ''' When ISE Test data is changed in Parameters Programming Module, if the ISE Test/Sample Type already exists in Historic Module, data is updated  
        ''' in thisISETestSamples table. If field Name has been changed, it is updated not only for the informed SampleType, but all the Sample Types that
        ''' exist in Historic Module for the informed ISETestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 04/09/2014 - BA-1865
        ''' </remarks>
        Public Function HIST_Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testSampleDefinitionDS As New HisISETestSamplesDS    'DS to return

                        'Get data currently programmed for the informed ISETestID/SampleType 
                        Dim myDAO As New tparISETestSamplesDAO
                        resultData = myDAO.HIST_GetISETestSampleData(dbConnection, pISETestID, pSampleType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            testSampleDefinitionDS = DirectCast(resultData.SetDatos, HisISETestSamplesDS)

                            If (testSampleDefinitionDS.thisISETestSamples.Rows.Count > 0) Then
                                'Read if the ISETestID already exists in Historic Module (read all existing SampleTypes)
                                Dim myHistDAO As New thisISETestSamplesDAO

                                resultData = myHistDAO.ReadByISETestID(dbConnection, pISETestID)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim currentHist As HisISETestSamplesDS = DirectCast(resultData.SetDatos, HisISETestSamplesDS)

                                    For Each row As HisISETestSamplesDS.thisISETestSamplesRow In currentHist.thisISETestSamples.Rows
                                        If (row.SampleType = pSampleType) Then
                                            'Get the HistISETestID and inform it in testSampleDefinitionDS
                                            testSampleDefinitionDS.thisISETestSamples.First.BeginEdit()
                                            testSampleDefinitionDS.thisISETestSamples.First.HistISETestID = row.HistISETestID
                                            testSampleDefinitionDS.thisISETestSamples.First.EndEdit()
                                        Else
                                            'Check if field Name has been changed
                                            If (row.ISETestName <> testSampleDefinitionDS.thisISETestSamples.First.ISETestName) Then
                                                'Update the field Name and move the row to testSampleDefinitionDS
                                                row.ISETestName = testSampleDefinitionDS.thisISETestSamples.First.ISETestName
                                                testSampleDefinitionDS.thisISETestSamples.ImportRow(row)
                                            End If
                                        End If
                                    Next
                                    testSampleDefinitionDS.AcceptChanges()

                                    'Finally, update data in Historic Module for all affected ISETests/SampleTypes
                                    resultData = myHistDAO.Update(dbConnection, testSampleDefinitionDS)
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
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.HIST_Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"
        ''' <summary>
        ''' Get all data in table tparISETestSamples for the informed ISETestID and SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an ISETestSamplesDS with all data of the informed ISETestID and SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944 (SubTask BA-2013)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestSamplesDAO
                        resultData = myDAO.Read(dbConnection, pISETestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE - NOT USED"
        ''' <summary>
        ''' Add a new ISETestSample (NOT USED)
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pISETestSamplesDetails">Typed DataSet ISETestSamplesDS containing the data of the ISETestSample to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 15/10/2010
        ''' Modified by: AG  Perform loop and pass row to DAO
        ''' </remarks>
        Public Function Add(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestSamplesDetails As ISETestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ISETestSampleToAdd As New tparISETestSamplesDAO

                        For Each row As ISETestSamplesDS.tparISETestSamplesRow In pISETestSamplesDetails.tparISETestSamples.Rows
                            'Validate if there is another ISETestSample with the same ID --> PDT !!!!!!!!!!!!!
                            If (Not resultData.HasError) Then
                                'Add the new ISETestSample
                                resultData = ISETestSampleToAdd.Create(dbConnection, row)
                            End If

                            If resultData.HasError Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
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
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Delete the specified ISETestSample (NOT USED)
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISETest identifier</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>Created by: XBC 15/10/2010</remarks>
        Public Function Delete(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ISETestSampleToDelete As New tparISETestSamplesDAO

                        resultData = ISETestSampleToDelete.Delete(dbConnection, pISETestID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
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
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestSamplesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class

End Namespace
