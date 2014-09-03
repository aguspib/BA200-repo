Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.BL

    Public Class ISETestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Verify if there are QC Results pending to cumulate for the specified ISE Test/SampleType and in this case, create
        ''' a new cumulate for each linked Control/Lot
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 12/06/2012
        ''' </remarks>
        Private Function CalculateQCCumulate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Search if there are QC Results pending to cumulate for the ISETest/Sample Type
                        Dim myQCResultsDelegate As New QCResultsDelegate

                        myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByTestIDSampleTypeNEW(pDBConnection, "ISE", pTestID, pSampleType)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myHistoryQCInfoDS As HistoryQCInformationDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                            Dim myCumulateResultDelegate As New CumulatedResultsDelegate
                            For Each historyQCInfoRow As HistoryQCInformationDS.HistoryQCInfoTableRow In myHistoryQCInfoDS.HistoryQCInfoTable.Rows
                                myGlobalDataTO = myCumulateResultDelegate.SaveCumulateResultNEW(pDBConnection, historyQCInfoRow.QCTestSampleID, historyQCInfoRow.QCControlLotID, _
                                                                                                historyQCInfoRow.AnalyzerID)
                                If (myGlobalDataTO.HasError) Then Exit For

                                If String.Equals(historyQCInfoRow.CalculationMode, "STATISTIC") Then
                                    'Delete the group of results used to calculate the statistic values
                                    myGlobalDataTO = myQCResultsDelegate.DeleteStatisticResultsNEW(pDBConnection, historyQCInfoRow.QCTestSampleID, historyQCInfoRow.QCControlLotID, _
                                                                                                   historyQCInfoRow.AnalyzerID)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                End If
                            Next
                        End If

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.CalculateQCCumulate", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search ISE Test data for the informed Test Name (for Import From LIMS Process) or verify the informed ShortName 
        ''' (when modifying) is unique     
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestName">ISE Test Name to search by</param>
        ''' <param name="pNameToSearch">Value indicating which is the name to validate: the short name or the long one. Possible values are: 
        '''                             FNAME, NAME, LNAME; Calling method must pass one of these values, otherwise function ReadName will result in an error.</param>
        ''' <param name="pISETestID">ISE Test Identifier. It is an optional parameter informed only in case of updation</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the informed ISE Test</returns>
        ''' <remarks>
        ''' Created by:  SA 25/10/2010
        ''' Modified by: WE 27/08/2014 - #1865. Modified for use with new function ReadName, which replaces former functions ReadByName and ReadByShortName.
        '''                              FNAME and NAME (param pNameToSearch) are used for compatibility with former function calls to ReadByName and ReadByShortName respectively.
        '''                              LNAME has been introduced to correctly determine the uniqueness of an occurrence of field [Name] in [tparISETests].
        ''' </remarks>
        Public Function ExistsISETestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestName As String, _
                                          Optional ByVal pNameToSearch As String = "FNAME", Optional ByVal pISETestID As Integer = 0) _
                                          As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO

                        'If String.Equals(pNameToSearch, "FNAME") Then
                        '    resultData = myDAO.ReadByName(dbConnection, pISETestName)
                        'Else
                        '    resultData = myDAO.ReadByShortName(dbConnection, pISETestName, pISETestID)
                        'End If
                        resultData = myDAO.ReadName(dbConnection, pISETestName, pNameToSearch, pISETestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.ExistsISETestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE Tests using the specified SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pGetForControls">When FALSE it indicates all ISE Tests using the specified SampleType have to be returned
        '''                               When TRUE it indicates that only ISE Tests using the specified SampleType, having QC active and
        '''                               at least an active linked Control will be returned</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the ISETests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 21/10/2010
        ''' Modified by: SA 22/06/2012 - Added parameter to indicate if the group of ISE Tests returned have to be restricted to only 
        '''                              those having QC active and at least an active linked Control
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, _
                                        ByVal pGetForControls As Boolean, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETests As New tparISETestsDAO
                        If (Not pGetForControls) Then
                            resultData = myISETests.ReadBySampleType(dbConnection, pSampleType, pCustomizedTestSelection) 'AG 01/09/2014 - BA-1869 parameter pCustomizedTestSelection
                        Else
                            resultData = myISETests.GetAllWithQCActive(dbConnection, pSampleType, pCustomizedTestSelection) 'AG 01/09/2014 - BA-1869 parameter pCustomizedTestSelection
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.GetBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the data of the specified ISETests and SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the ISETests using the specified SampleType</returns>
        ''' <remarks>
        ''' Created by DL: 23/02/2012
        ''' </remarks>
        Public Function GetByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) _
                                                 As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETests As New tparISETestsDAO
                        resultData = myISETests.ReadByTestIDAndSampleType(dbConnection, pISETestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.GetByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with the list of ISE Tests</returns>
        ''' <remarks>
        ''' Created by: XBC 15/10/2010
        ''' </remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of defined ISE Tests
                        Dim myISETestsDAO As New tparISETestsDAO
                        resultData = myISETestsDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns selected ISE info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="SelectedISE">List of selected profiles IDs</param>
        ''' <remarks>
        ''' Created by: DL 27/06/2012
        ''' </remarks>
        Public Function GetISEForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal AppLang As String, _
                                        Optional ByVal SelectedISE As List(Of String) = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myISEDAO As New tparISETestSamplesDAO
                        resultData = myISEDAO.GetISEForReport(dbConnection, AppLang, SelectedISE)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.GetISEForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update data of the specified ISE Test
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pISETestsDetails">Typed DataSet ISETestsDS containing the data of the ISETest to be updated</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by: XBC 15/10/2010
        ''' Modified by: AG 27/10/2010 - Perfom loop and pass Row to DAO
        '''              SA 10/11/2010 - Before executing the Update, verify there is not another ISE Test with the 
        '''                              informed ShortName
        '''              SA 13/01/2011 - Validation of duplicated ShortName was moved to the screen
        ''' </remarks>
        Public Function Modify(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestsDetails As ISETestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim ISETestToUpdate As New tparISETestsDAO
                        For Each row As ISETestsDS.tparISETestsRow In pISETestsDetails.tparISETests.Rows
                            'Update the ISE Test
                            resultData = ISETestToUpdate.Update(dbConnection, row)
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
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified ISE Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">Identifier of the ISE Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the specified ISE Test</returns>
        ''' <remarks>
        ''' Created by: SA 21/10/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETestsDAO As New tparISETestsDAO
                        resultData = myISETestsDAO.Read(dbConnection, pISETestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all ISE Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for ISE Tests that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/10/2010 
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO
                        myGlobalDataTO = myDAO.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, pFlag, pUpdateForExcluded)

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the field InUse by TestID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pInUseFlag"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/05/2013</remarks>
        Public Function UpdateInUseByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pInUseFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO
                        myGlobalDataTO = myDAO.UpdateInUseByTestID(dbConnection, pTestID, pInUseFlag)

                        If (Not myGlobalDataTO.HasError) Then
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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.UpdateInUseByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function



        ''' <summary>
        ''' Save an ISE Test: basic data, SampleType data, Quality Control data and Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdatedISETests">Typed DataSet ISETestsDS containing updated basic data for an specific ISE Test</param>
        ''' <param name="pUpdatedISETestSamples">Typed DataSet ISETestSamplesDS containing SampleType-related data for an specific 
        '''                                       ISE Test and SampleType</param>
        ''' <param name="pNewISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to add for the ISETest/SampleType</param>
        ''' <param name="pUpdatedISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to update for the ISETest/SampleType</param>
        ''' <param name="pDeletedISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to delete for the ISETest/SampleType</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG
        ''' Modified by: RH 08/06/2012 - Code optimization. Add new parameters to allow saving the Quality Control data for a 
        '''                              specific ISE Test and SampleType
        '''              SA 12/06/2012 - Removed DataSets for adding: ISE Tests/SampleType cannot be added nor deleted
        '''              SA 18/06/2012 - Added code to save QC values for the ISE Tests/SampleType also in QC Module when the ISE Test/SampleType
        '''                              already exists in it
        ''' </remarks>
        Public Function SaveISETestNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUpdatedISETests As ISETestsDS, _
                                       ByVal pUpdatedISETestSamples As ISETestSamplesDS, ByVal pNewISERefRanges As TestRefRangesDS, _
                                       ByVal pUpdatedISERefRanges As TestRefRangesDS, ByVal pDeletedISERefRanges As TestRefRangesDS, _
                                       ByVal pTestSamplesMultiRulesDS As TestSamplesMultirulesDS, ByVal pTestsControlsDS As TestControlsDS, _
                                       ByVal pDeleteControlTOList As List(Of DeletedControlTO)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update basic data of the ISE Test
                        If (pUpdatedISETests.tparISETests.Rows.Count > 0) Then resultData = Me.Modify(dbConnection, pUpdatedISETests)

                        'Update SampleType-related data for the ISE Test/SampleType
                        If (Not resultData.HasError) Then
                            Dim myISESampleType As New ISETestSamplesDelegate
                            If (pUpdatedISETestSamples.tparISETestSamples.Rows.Count > 0) Then resultData = myISESampleType.Modify(dbConnection, pUpdatedISETestSamples)
                        End If

                        'Update Reference Ranges for the ISETest/SampleType
                        If (Not resultData.HasError) Then
                            Dim myRefRangesDel As New TestRefRangesDelegate

                            'Delete Range Types contained in pDeletedISERefRanges
                            If (pDeletedISERefRanges.tparTestRefRanges.Rows.Count > 0) Then myRefRangesDel.Delete(dbConnection, pDeletedISERefRanges, "ISE")

                            'Add Range Types contained in pNewISERefRanges
                            If (Not resultData.HasError) Then
                                If (pNewISERefRanges.tparTestRefRanges.Rows.Count > 0) Then myRefRangesDel.Create(dbConnection, pNewISERefRanges, "ISE")
                            End If

                            'Update data of Range Types contained in pUpdatedISERefRanges
                            If (Not resultData.HasError) Then
                                If (pUpdatedISERefRanges.tparTestRefRanges.Rows.Count > 0) Then myRefRangesDel.Update(dbConnection, pUpdatedISERefRanges, "ISE")
                            End If
                        End If

                        'Update Multirules and list of Controls linked to the ISETest/SampleType
                        If (Not resultData.HasError) Then
                            'Update Multirules
                            Dim myTestSamplesMultiRulesDelegate As New TestSamplesMultirulesDelegate
                            resultData = myTestSamplesMultiRulesDelegate.SaveMultiRulesNEW(dbConnection, pTestSamplesMultiRulesDS)

                            'If there are Controls to unlink in pDeleteControlTOList, for each one of them, cumulate open QC Results
                            If (Not pDeleteControlTOList Is Nothing AndAlso pDeleteControlTOList.Count > 0) Then
                                For Each myDeleteControlTO As DeletedControlTO In pDeleteControlTOList
                                    resultData = CalculateQCCumulate(dbConnection, myDeleteControlTO.TestID, myDeleteControlTO.SampleType)
                                    If (resultData.HasError) Then Exit For
                                Next
                            End If

                            'Update the list of Controls linked to the ISETest/SampleType
                            If (Not resultData.HasError) Then
                                Dim myTestControlsDelegate As New TestControlsDelegate
                                If (pTestsControlsDS.tparTestControls.Count > 0) Then
                                    resultData = myTestControlsDelegate.SaveTestControlsNEW(dbConnection, pTestsControlsDS, Nothing, False)
                                Else
                                    'If there are Controls 
                                    If (Not pDeleteControlTOList Is Nothing AndAlso pDeleteControlTOList.Count > 0) Then
                                        resultData = myTestControlsDelegate.DeleteTestControlsByTestIDAndTestTypeNEW(dbConnection, pDeleteControlTOList(0).TestID, "ISE")
                                    End If
                                End If
                            End If
                        End If

                        Dim myQCTestSampleID As Integer = 0
                        Dim myHistoryControlLotDelegate As New HistoryControlLotsDelegate
                        Dim myHistoryControlLotDS As New HistoryControlLotsDS

                        'Dim myHistoryTestControlLotsDS As New HistoryTestControlLotsDS
                        Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate
                        Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
                        Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
                        Dim myHistoryTestSamplesRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow
                        Dim myHistoryTestSamplesRulesDelegate As New HistoryTestSamplesRulesDelegate

                        'After updating QC data for the ISETest/SampleType, then update the corresponding data on the QC Module 
                        If (Not resultData.HasError) Then
                            'UPDATE data of the ISE Test in QC Module
                            myHistoryTestSamplesRow = myHistoryTestSamplesDS.tqcHistoryTestSamples.NewtqcHistoryTestSamplesRow()
                            myHistoryTestSamplesRow.TestType = "ISE"
                            myHistoryTestSamplesRow.TestID = pUpdatedISETestSamples.tparISETestSamples(0).ISETestID
                            myHistoryTestSamplesRow.TestName = pUpdatedISETestSamples.tparISETestSamples(0).ISETestName
                            myHistoryTestSamplesRow.TestShortName = pUpdatedISETestSamples.tparISETestSamples(0).ISETestShortName
                            myHistoryTestSamplesRow.PreloadedTest = False
                            myHistoryTestSamplesRow.MeasureUnit = pUpdatedISETestSamples.tparISETestSamples(0).MeasureUnit
                            myHistoryTestSamplesDS.tqcHistoryTestSamples.AddtqcHistoryTestSamplesRow(myHistoryTestSamplesRow)

                            resultData = myHistoryTestSamplesDelegate.UpdateByTestIDNEW(dbConnection, myHistoryTestSamplesDS)
                            If (Not resultData.HasError) Then
                                'UPDATE data of the ISE Test/Sample Type in QC Module
                                resultData = myHistoryTestSamplesDelegate.ReadByTestIDAndSampleTypeNEW(dbConnection, "ISE", pUpdatedISETestSamples.tparISETestSamples(0).ISETestID, _
                                                                                                       pUpdatedISETestSamples.tparISETestSamples(0).SampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myHistoryTestSamplesDS = DirectCast(resultData.SetDatos, HistoryTestSamplesDS)
                                    If (myHistoryTestSamplesDS.tqcHistoryTestSamples.Count > 0) Then myQCTestSampleID = myHistoryTestSamplesDS.tqcHistoryTestSamples(0).QCTestSampleID

                                    If (Not myQCTestSampleID = 0) Then
                                        'Update Calculation Criteria fields in QC Module
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria = pUpdatedISETestSamples.tparISETestSamples(0).RejectionCriteria
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode = pUpdatedISETestSamples.tparISETestSamples(0).CalculationMode
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries = pUpdatedISETestSamples.tparISETestSamples(0).NumberOfSeries

                                        ' WE 26/08/2014 - #1865 Moved update Decimals and new field TestLongName to here from above ('UPDATE data of the ISE Test in QC Module).
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed = pUpdatedISETestSamples.tparISETestSamples(0).Decimals
                                        myHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestLongName = pUpdatedISETestSamples.tparISETestSamples(0).TestLongName    ' WE 31/07/2014 - #1865

                                        resultData = myHistoryTestSamplesDelegate.UpdateByQCTestIdAndSampleTypeNEW(dbConnection, myHistoryTestSamplesDS)
                                        If (Not resultData.HasError) Then
                                            'UPDATE MultiRules for the ISETest/SampleType in QC Module 
                                            '1 - Delete existing MultiRules
                                            resultData = myHistoryTestSamplesRulesDelegate.Delete(dbConnection, myQCTestSampleID)
                                            If (Not resultData.HasError) Then
                                                '2 - Insert new status for available MultiRules
                                                resultData = myHistoryTestSamplesRulesDelegate.InsertFromTestSampleMultiRulesNEW(dbConnection, "ISE", pUpdatedISETestSamples.tparISETestSamples(0).ISETestID, _
                                                                                                                                 pUpdatedISETestSamples.tparISETestSamples(0).SampleType, myQCTestSampleID)
                                            End If
                                        End If
                                    End If
                                End If

                                If (Not resultData.HasError) Then
                                    'UPDATE relations between ISETest/SampleType and the linked Controls in QC Module
                                    For Each iseTestControlRow As TestControlsDS.tparTestControlsRow In pTestsControlsDS.tparTestControls.Rows

                                        If Not iseTestControlRow.IsControlIDNull Then 'dl
                                            resultData = myHistoryControlLotDelegate.GetQCControlLotIDByControlIDAndLotNumber(dbConnection, iseTestControlRow.ControlID, _
                                                                                                                              iseTestControlRow.LotNumber)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myHistoryControlLotDS = DirectCast(resultData.SetDatos, HistoryControlLotsDS)
                                                If (myHistoryControlLotDS.tqcHistoryControlLots.Rows.Count = 1) Then
                                                    resultData = myHistoryTestControlLotsDelegate.Update(dbConnection, myQCTestSampleID, myHistoryControlLotDS.tqcHistoryControlLots(0).QCControlLotID, _
                                                                                                         iseTestControlRow.MinConcentration, iseTestControlRow.MaxConcentration)
                                                End If
                                            End If
                                        End If 'DL
                                    Next
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
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.SaveISETest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all ISE Tests/SampleTypes using Quality Control (those having QCActive = TRUE). If a Control Identifier 
        ''' is specified, then it also get the Standard Tests/SampleTypes with QCActive = FALSE but that are linked to the 
        ''' informed Control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with all Tests/SampleTypes to show in the screen of 
        '''          Tests Selection when it is opened from Control Programming Screen</returns>
        ''' <remarks>
        ''' Created by:  RH 14/06/2012 
        ''' </remarks>
        Public Function GetForControlsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                  Optional ByVal pControlID As Integer = -1) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Get all ISETests/Sample Types having QCActive=TRUE
                        Dim mytparISETestsDAO As New tparISETestsDAO
                        resultData = mytparISETestsDAO.GetAllWithQCActive(dbConnection)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myAllTestsDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                            If (pControlID > 0) Then
                                'A Control has been informed, get all ISETests/SampleTypes linked to it
                                resultData = mytparISETestsDAO.GetAllByControl(dbConnection, pControlID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myCtrlTestsDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                                    For Each ctrlTestRow As ISETestsDS.tparISETestsRow In myCtrlTestsDS.tparISETests.Rows
                                        'The ISETest/SampleType has QCActive; inform in the DS to return, if it is Active for the Control
                                        If (ctrlTestRow.QCActive) Then
                                            Dim qControlTestList As List(Of ISETestsDS.tparISETestsRow)
                                            qControlTestList = (From a In myAllTestsDS.tparISETests _
                                                                Where String.Equals(a.ISETestID, ctrlTestRow.ISETestID) _
                                                                AndAlso String.Equals(a.SampleType, ctrlTestRow.SampleType) _
                                                                Select a).ToList()

                                            If (qControlTestList.Count = 1) Then
                                                qControlTestList.First.ActiveControl = ctrlTestRow.ActiveControl
                                            End If
                                        Else
                                            'Move the Test/SampleType to the DS to return
                                            myAllTestsDS.tparISETests.ImportRow(ctrlTestRow)
                                        End If

                                        myAllTestsDS.AcceptChanges()
                                    Next ctrlTestRow
                                End If
                            End If

                            resultData.SetDatos = myAllTestsDS
                            resultData.HasError = False
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.GetForControlsProgramming", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update the LISValue by the ISETestID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pISETestID">ISE Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, pISETestID As Integer, pLISValue As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO
                        myGlobalDataTO = myDAO.UpdateLISValueByTestID(dbConnection, pISETestID, pLISValue)

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.UpdateLISValueByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets all ISE tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with setDatos ReportsTestsSortingDS</returns>
        ''' <remarks>
        ''' AG 02/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetCustomizedSortedTestSelectionList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO
                        resultData = myDAO.GetCustomizedSortedTestSelectionList(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ISETestsDelegate.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for ISE tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateCustomPositionAndAvailable(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsSortingDS As ReportsTestsSortingDS) _
                                           As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparISETestsDAO
                        myGlobalDataTO = myDAO.UpdateCustomPositionAndAvailable(dbConnection, pTestsSortingDS)
                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function

#End Region

#Region "TO DELETE - NOT USED"
        ' ''' <summary>
        ' ''' Add a new ISETest (NOT USED)
        ' ''' </summary>
        ' ''' <param name="pDbConnection">Open DB Connection</param>
        ' ''' <param name="pISETestsDetails">Typed DataSet ISETestsDS containing the data of the ISETest to add</param>
        ' ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ' ''' <remarks>Created by: XBC 15/10/2010
        ' ''' AG 27/10/2010 - Perfom loop and pass Row to DAO</remarks>
        'Public Function Create(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestsDetails As ISETestsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim ISETestToAdd As New tparISETestsDAO

        '                For Each row As ISETestsDS.tparISETestsRow In pISETestsDetails.tparISETests.Rows
        '                    'Validate if there is another ISETest with the same ID --> PDT !!!!!!!!!!!!!

        '                    If (Not resultData.HasError) Then
        '                        'Add the new ISETestSample
        '                        resultData = ISETestToAdd.Create(dbConnection, row)
        '                    End If

        '                    If resultData.HasError Then Exit For
        '                Next

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Delete the specified ISETest (NOT USED)
        ' ''' </summary>
        ' ''' <param name="pDbConnection">Open DB Connection</param>
        ' ''' <param name="pISETestsList">Typed DataSet ISETestsDS containing the list of the ISETests to be deleted</param>
        ' ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ' ''' <remarks>Created by: XBC 15/10/2010</remarks>
        'Public Function Delete(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pISETestsList As ISETestsDS) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim ISETestToDelete As New tparISETestsDAO
        '                Dim myISETestSamplesDelegate As New ISETestSamplesDelegate

        '                For Each ISETestRow As ISETestsDS.tparISETestsRow In pISETestsList.tparISETests.Rows
        '                    'First Setp : Delete ISETestSamples
        '                    resultData = myISETestSamplesDelegate.Delete(dbConnection, ISETestRow.ISETestID)
        '                    If (resultData.HasError) Then Exit For

        '                    'Second step : Delete ISETests
        '                    resultData = ISETestToDelete.Delete(dbConnection, ISETestRow.ISETestID)
        '                    If (resultData.HasError) Then Exit For
        '                Next

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.Delete", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Save an ISE Test: basic data, SampleType data, Quality Control data and Reference Ranges
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pNewISETests">NOT USED - Typed DataSet ISETestsDS containing basic data of an added ISE Test</param>
        ' ''' <param name="pUpdatedISETests">Typed DataSet ISETestsDS containing updated basic data for an specific ISE Test</param>
        ' ''' <param name="pNewISETestSamples">NOT USED - Typed DataSet ISETestSamplesDS containing SampleType-related data for an added
        ' '''                                  ISE Test and SampleType</param>
        ' ''' <param name="pUpdatedISETestSamples">Typed DataSet ISETestSamplesDS containing SampleType-related data for an specific 
        ' '''                                       ISE Test and SampleType</param>
        ' ''' <param name="pNewISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to add for the ISETest/SampleType</param>
        ' ''' <param name="pUpdatedISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to update for the ISETest/SampleType</param>
        ' ''' <param name="pDeletedISERefRanges">Typed DataSet TestRefRangesDS containing all Range Types to delete for the ISETest/SampleType</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  AG
        ' ''' Modified by: RH 08/06/2012 - Code optimization. Add new parameters to allow saving the Quality Control data for an 
        ' '''                              specific ISE Test and SampleType
        ' ''' </remarks>
        'Public Function SaveISETest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pNewISETests As ISETestsDS, ByVal pUpdatedISETests As ISETestsDS, _
        '                            ByVal pNewISETestSamples As ISETestSamplesDS, ByVal pUpdatedISETestSamples As ISETestSamplesDS, _
        '                            ByVal pNewISERefRanges As TestRefRangesDS, ByVal pUpdatedISERefRanges As TestRefRangesDS, ByVal pDeletedISERefRanges As TestRefRangesDS, _
        '                            ByVal pTestSamplesMultiRulesDS As TestSamplesMultirulesDS, ByVal pTestsControlsDS As TestControlsDS, _
        '                            ByVal pDeleteControlTOList As List(Of DeletedControlTO)) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'New ISE tests
        '                If (pNewISETests.tparISETests.Rows.Count > 0) Then
        '                    resultData = Me.Create(dbConnection, pNewISETests)
        '                End If

        '                If (Not resultData.HasError) Then '(1)
        '                    'Updated ISE tests
        '                    If (pUpdatedISETests.tparISETests.Rows.Count > 0) Then
        '                        resultData = Me.Modify(dbConnection, pUpdatedISETests)
        '                    End If

        '                    If Not resultData.HasError Then '(2)
        '                        'New ISE test sample types
        '                        Dim myISESampleType As New ISETestSamplesDelegate
        '                        If (pNewISETestSamples.tparISETestSamples.Rows.Count > 0) Then
        '                            resultData = myISESampleType.Add(dbConnection, pNewISETestSamples)
        '                        End If

        '                        If (Not resultData.HasError) Then '(3)
        '                            'Updated ISE test sample types
        '                            If (pUpdatedISETestSamples.tparISETestSamples.Rows.Count > 0) Then
        '                                resultData = myISESampleType.Modify(dbConnection, pUpdatedISETestSamples)
        '                            End If

        '                            If Not resultData.HasError Then '(4)
        '                                Dim myRefRangesDel As New TestRefRangesDelegate
        '                                If (pDeletedISERefRanges.tparTestRefRanges.Rows.Count > 0) Then
        '                                    resultData = myRefRangesDel.Delete(dbConnection, pDeletedISERefRanges, "ISE")
        '                                End If

        '                                If Not resultData.HasError Then '(5)
        '                                    If pNewISERefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                                        resultData = myRefRangesDel.Create(dbConnection, pNewISERefRanges, "ISE")
        '                                    End If

        '                                    If Not resultData.HasError Then '(6)
        '                                        If pUpdatedISERefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                                            resultData = myRefRangesDel.Update(dbConnection, pUpdatedISERefRanges, "ISE")
        '                                        End If

        '                                        'Save the TestControl
        '                                        If Not resultData.HasError Then
        '                                            'Call the Multirules Delegate
        '                                            Dim myTestSamplesMultiRulesDelegate As New TestSamplesMultirulesDelegate
        '                                            Dim myTestControlsDelegate As New TestControlsDelegate

        '                                            resultData = myTestSamplesMultiRulesDelegate.SaveMultiRulesNEW(dbConnection, pTestSamplesMultiRulesDS)

        '                                            If Not resultData.HasError Then
        '                                                If (Not pDeleteControlTOList Is Nothing AndAlso pDeleteControlTOList.Count > 0) Then
        '                                                    'Calculate Cumulate if exist.
        '                                                    For Each myDeleteControlTO As DeletedControlTO In pDeleteControlTOList
        '                                                        resultData = CalculateQCCumulate(dbConnection, myDeleteControlTO.TestID, myDeleteControlTO.SampleType)
        '                                                    Next
        '                                                End If
        '                                            End If

        '                                            'Save the TestControl
        '                                            If Not resultData.HasError Then
        '                                                If pTestsControlsDS.tparTestControls.Count > 0 Then
        '                                                    'Call the delegate to save Testcontrol
        '                                                    resultData = myTestControlsDelegate.SaveTestControlsNEW( _
        '                                                            dbConnection, pTestsControlsDS, Nothing, False)
        '                                                Else
        '                                                    If Not pDeleteControlTOList Is Nothing AndAlso pDeleteControlTOList.Count > 0 Then
        '                                                        'If no test control then remove all by testid and sample type.
        '                                                        'Use the parammeter pUpdateSampleType indicating the updated sample type.
        '                                                        resultData = myTestControlsDelegate.DeleteTestControlsByTestIDAndTestTypeNEW(dbConnection, _
        '                                                                                                                                     pDeleteControlTOList(0).TestID, _
        '                                                                                                                                     "ISE")
        '                                                    End If
        '                                                End If
        '                                            End If
        '                                        End If
        '                                    End If '(6)
        '                                End If '(5)
        '                            End If '(4)
        '                        End If '(3)
        '                    End If '(2)
        '                End If '(1)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ISETestsDelegate.SaveISETest", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region

    End Class

End Namespace
