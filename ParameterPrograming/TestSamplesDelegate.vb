Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class TestSamplesDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Create a new Sample Type for a Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesDS">Typed DataSet TestSamplesDS containing the list of Test Samples to create</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 02/03/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesDS As TestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()
                        myGlobalDataTO = myTestSamplesDAO.Create(dbConnection, pTestSamplesDS)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update data of an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestSamplesDS">Typed DataSet TestSamplesDS containing the data of the Test Sample to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 02/03/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestSamplesDS As TestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()
                        myGlobalDataTO = myTestSamplesDAO.Update(dbConnection, pTestSamplesDS)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the EnableStatus by TestID and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pEnableStatus">Status</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 17/02/2011
        ''' </remarks>
        Public Function UpdateTestSampleEnableStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, ByVal pEnableStatus As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()

                        myGlobalDataTO = myTestSamplesDAO.UpdateTestSampleEnableStatus(dbConnection, pTestID, pSampleType, pEnableStatus)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.UpdateTestSampleEnableStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Update the Factory Calibrator by TestID and SampleType.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pFactoryCalib">Value for Factory Calib</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 08/03/2011
        ''' </remarks>
        Public Function UpdateTestSampleFactoryCalib(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String, ByVal pFactoryCalib As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()

                        myGlobalDataTO = myTestSamplesDAO.UpdateTestSampleFactoryCalibrator(dbConnection, pTestID, pSampleType, pFactoryCalib)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.UpdateTestSampleFactoryCalib", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Update value of field DefaultSampleType for the specified Test/SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 25/06/2012
        ''' </remarks>
        Public Function UpdateDefaultSampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                ByVal pTestID As Integer, _
                                                ByVal pSampleType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()
                        resultData = myTestSamplesDAO.UpdateDefaultSampleType(dbConnection, pTestID, pSampleType)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.UpdateDefaultSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Delete the specified Test/SampleType, deleting first all relations defined for it
        ''' (Reference Ranges, Reagent Volumes, Controls, Calibrator and Test Profiles) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 16/03/2010
        ''' Modified By: TR 18/09/2012 - Removed from Calculated Tests if SampleType is inuse
        '''              AG 09/10/2012 - Added changes to mark the deleted Test/SampleType as closed in Historic Module
        '''              SA 22/10/2012 - Do not call function HIST_CloseTestVersion, it is enough call function HIST_CloseTestSample
        ''' </remarks>
        Public Function DeleteCascadeByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                        ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String, _
                                                        ByVal pAnalyzerID As String, _
                                                        ByVal pWorkSessionID As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete the the Reference Ranges defined for the Test/SampleType 

                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate
                        myGlobalDataTO = myTestRefRangesDelegate.DeleteByTestID(dbConnection, pTestID, pSampleType)

                        'Delete all relations with Controls defined for the Test/SampleType
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myTestControlDelegate As New TestControlsDelegate()
                            myGlobalDataTO = myTestControlDelegate.DeleteTestControlsByTestIDNEW(dbConnection, "STD", pTestID, pAnalyzerID, pWorkSessionID, pSampleType, )
                        End If

                        'When for the Test/SampleType an Experimental Calibrator is used, delete all Calibrator information 
                        '(relation between the Test/SampleType and the Experimental Calibrator plus the values for each point)
                        If (Not myGlobalDataTO.HasError) Then
                            'Get the Calibrator used for the Test/SampleType
                            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate()
                            myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(dbConnection, pTestID, pSampleType)

                            If (Not myGlobalDataTO.HasError) Then
                                Dim myTestCalibratorDS As New TestCalibratorsDS
                                myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                                'Delete values defined for each Calibrator point
                                Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate()
                                For Each testCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In myTestCalibratorDS.tparTestCalibrators.Rows
                                    myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, testCalibRow.TestCalibratorID)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next

                                'Delete the relation between the Test/SampleType and the Experimental Calibrator
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myTestCalibratorDelegate.DeleteTestCaliByTestID(dbConnection, pTestID, pSampleType)
                                End If
                            End If
                        End If

                        'Delete Reagents Volumes defined for the Test/SampleType
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myTestReagentsVolumeDelegate As New TestReagentsVolumeDelegate
                            myGlobalDataTO = myTestReagentsVolumeDelegate.DeleteByTestIDAndSampleType(dbConnection, pTestID, pSampleType)
                        End If

                        'AG 10/05/2013 - In those saved WS with FromLIS = 1 mark as deletedFlag but do not remove records 
                        'Mark as DeletedFlag if exists into a saved Worksession from LIS
                        'NOTE This code must be place before Delete the test-sampletype in order to detect the LIS mapping value!!
                        If Not myGlobalDataTO.HasError Then
                            Dim mySavedWSOTs As New SavedWSOrderTestsDelegate
                            myGlobalDataTO = mySavedWSOTs.UpdateDeletedTestFlag(dbConnection, "STD", pTestID, pSampleType)
                        End If
                        'AG 10/05/2013


                        'Delete the Test/SampleType
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myTestSamplesDAO As New tparTestSamplesDAO()
                            myGlobalDataTO = myTestSamplesDAO.Delete(dbConnection, pTestID, pSampleType)
                        End If

                        'Delete the Test/SampleType from all Test Profiles in which it is included
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myTestProfileDelegate As New TestProfilesDelegate
                            myGlobalDataTO = myTestProfileDelegate.DeleteByTestIDSampleType(dbConnection, pTestID, pSampleType)
                        End If

                        'TR 18/09/2012 -Disable Calculated test.
                        If Not myGlobalDataTO.HasError Then
                            Dim myCalculatedTestDelegate As New CalculatedTestsDelegate
                            myGlobalDataTO = myCalculatedTestDelegate.DeleteCalculatedTestbyTestID(dbConnection, _
                                                                                                    pTestID, pSampleType)
                        End If
                        'TR 18/09/2012 -END.

                        'AG 10/05/2013 - Comment this code. When delete a sample type is not required because the test continues existing with the same ID, so no test change is possible
                        '
                        ''AG 10/05/2013 - Remove only in saved WS where FromLIS = 0 ('DL 08/05/2013)
                        ''NOTE This code must be place after Delete the test in order to detect the deleted elements!!
                        'If (Not myGlobalDataTO.HasError) Then
                        '    Dim mySavedWSDelegate As New SavedWSDelegate

                        '    'AG get all saved ws but the FromLIS = 1
                        '    myGlobalDataTO = mySavedWSDelegate.GetAll(dbConnection, False, False) 'True)
                        '    If Not myGlobalDataTO.HasError Then
                        '        If Not myGlobalDataTO.SetDatos Is Nothing AndAlso DirectCast(myGlobalDataTO.SetDatos, SavedWSDS).tparSavedWS.Count > 0 Then
                        '            Dim mySavedWS As SavedWSDS = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS)
                        '            Dim mySavedWSOrderTests As New SavedWSOrderTestsDelegate

                        '            For Each row As SavedWSDS.tparSavedWSRow In mySavedWS.tparSavedWS.Rows
                        '                'Delete all elements included in the Saved WS that have been deleted
                        '                myGlobalDataTO = mySavedWSOrderTests.ClearDeletedElements(dbConnection, row.SavedWSID)

                        '                If Not myGlobalDataTO.HasError Then
                        '                    myGlobalDataTO = mySavedWSDelegate.DeleteEmptySavedWS(dbConnection, row.SavedWSID)
                        '                End If

                        '            Next
                        '        End If
                        '    End If
                        'End If
                        ''DL 08/05/2013


                        'DL 25/06/2012. Error when delete sample type by default
                        Dim myTestSamplesDelegate As New TestSamplesDelegate
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestSamplesDelegate.GetSampleDataByTestID(dbConnection, pTestID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myTestSamplesDS As New TestSamplesDS
                                myTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                If myTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                                    myGlobalDataTO = myTestSamplesDelegate.UpdateDefaultSampleType(dbConnection, pTestID, _
                                                                                                   myTestSamplesDS.tparTestSamples.First.SampleType)
                                End If
                            End If
                        End If
                        'DL 25/06/2012

                        'AG 09/10/2012 - CloseSampleType in historic results
                        If (Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode) Then
                            myGlobalDataTO = myTestSamplesDelegate.HIST_CloseTestSample(dbConnection, pTestID, pSampleType)
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.DeleteCascadeByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the Test calibrator and the concenctration  values.for and specific Test and Sample type.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: TR 01/03/2011
        ''' </remarks>
        Public Function DeleteTestCalibratorDataByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'When for the Test/SampleType an Experimental Calibrator is used, delete all Calibrator information 
                        '(relation between the Test/SampleType and the Experimental Calibrator plus the values for each point)
                        If (Not myGlobalDataTO.HasError) Then
                            'Get the Calibrator used for the Test/SampleType
                            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate()
                            myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(dbConnection, pTestID, pSampleType)

                            If (Not myGlobalDataTO.HasError) Then
                                Dim myTestCalibratorDS As New TestCalibratorsDS
                                myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                                'Delete values defined for each Calibrator point
                                Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate()
                                For Each testCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In myTestCalibratorDS.tparTestCalibrators.Rows
                                    myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, testCalibRow.TestCalibratorID)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next

                                'Delete the relation between the Test/SampleType and the Experimental Calibrator
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myTestCalibratorDelegate.DeleteTestCaliByTestID(dbConnection, pTestID, pSampleType)
                                End If
                            End If
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.DeleteCascadeByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        '''<summary>
        '''Get all data for the specified Test and SampleType 
        '''</summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        '''<param name="pTestID">Test Identifier</param>
        '''<param name="pSampleType">Sample Type Code</param>
        ''' <param name="pGetReplicatesFlag"></param>
        '''<returns>GlobalDataTO containing a typed DataSet TestSamplesDS with data of the Calibrator defined
        '''         for the informed Test and Sample Type</returns>
        '''<remarks>
        ''' Modified by:  SA - 23/02/2010 - Return GlobalDataTO, added the DB Connection management, and call to Read
        '''                                 function (not generated) in the correspondent DAO Class
        ''' Modified by: GDS - 05/05/2010 - Changed name GetCalibratorDefinition by GetDefinition
        ''' AG 13/03/2013 - add optional parameters to get also the replicates number. Used when import from LIS (using embedded synapse xml)
        ''' </remarks>
        Public Function GetDefinition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                      ByVal pSampleType As String, Optional ByVal pGetReplicatesFlag As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        resultData = myTestSamplesDAO.Read(dbConnection, pTestID, pSampleType, pGetReplicatesFlag)
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetDefinition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the Sample Type marked as default for the specified Test. Get also TestName, BlankReplicates and TestVersionNumber
        ''' for the Test. The default SampleType is used to get some values when a Blank is executed for the Test
        ''' </summary>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestsDS with the information for the specified Test</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: SA 18/02/2011 - Get also TestName, BlankReplicates and TestVersionNumber for the Test.
        '''                              Return a GlobalDataTO containing a typed DataSet TestsDS
        ''' </remarks>
        Public Function GetDefaultSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        resultData = myTestSamplesDAO.GetDefaultSampleType(dbConnection, pTestID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetDefaultSampleType", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Sample Types defined for the specified Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with data of all
        '''          Sample Types defined for the specified Test</returns>
        ''' <remarks>
        ''' Created by:  TR 17/02/2010
        ''' </remarks>
        Public Function GetSampleDataByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        myGlobalDataTO = myTestSamplesDAO.ReadByTestID(dbConnection, pTestID)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetSampleDataByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate if Calibration values are initial or factory.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>True if Calibration values are the initial or factory values; otherwise, it returns False</returns>
        ''' <remarks>
        ''' Created by:  TR 08/03/2011
        ''' modified by: TR 19/10/2011 -Add the finally to close the open connection.
        ''' </remarks>
        Public Function ValidateFactoryCalibratorValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                                                        ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                Dim isFactory As Boolean = False
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myTestSamplesDS As New TestSamplesDS
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        myGlobalDataTO = myTestSamplesDAO.Read(dbConnection, pTestID, pSampleType, False)
                        If Not myGlobalDataTO.HasError Then
                            myTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                            'Validate is not null because it could be a new test.
                            If myTestSamplesDS.tparTestSamples.Count > 0 AndAlso _
                                        Not myTestSamplesDS.tparTestSamples(0).IsFactoryCalibNull AndAlso _
                                        myTestSamplesDS.tparTestSamples(0).FactoryCalib Then
                                isFactory = True
                            End If
                        End If
                    End If
                End If
                myGlobalDataTO.SetDatos = isFactory

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.ValidateFactoryCalibratorValue", _
                                                                                    EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update value of field NumberOfControls for the specified Test/SampleType according the number
        ''' of Controls linked to the it in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/05/2011
        ''' </remarks>
        Public Function UpdateNumOfControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO()
                        resultData = myTestSamplesDAO.UpdateNumOfControls(dbConnection, pTestID, pSampleType)

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
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.UpdateNumOfControls", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        '''<summary>
        '''Get data of the specified Test and SampleType 
        '''</summary>
        '''<param name="TestID">Test Identifier</param>
        '''<param name="SampleType">Sample Type</param>
        '''<returns>DataSet containing data of the specified Test and SampleType</returns>
        '''<remarks>
        ''' CREATE BY: 
        ''' 
        ''' </remarks>
        Public Function GetTestSampleDataByTestIDAndSampleType(ByVal TestID As Integer, ByVal SampleType As String) As TestSamplesDS
            Dim result As New TestSamplesDS
            Try
                Dim myTestSamplesDAO As New tparTestSamplesDAO()
                result = myTestSamplesDAO.Read(TestID, SampleType)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetTestSampleDataByTestIDAndSampleType", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get all data needed to export it to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCTestSamples with all data needed to export the Test/SampleType to QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Public Function GetDefinitionForQCModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        resultData = myTestSamplesDAO.GetDefinitionForQCModule(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetDefinitionForQCModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test/SampleType, get value of limits that are needed to save Results in Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSamplesDS with value of limits that are needed to save 
        '''          Results in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 18/10/2012
        ''' </remarks>
        Public Function GetLimitsForHISTResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestSamplesDAO As New tparTestSamplesDAO
                        resultData = myTestSamplesDAO.GetLimitsForHISTResults(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.GetLimitsForHISTResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "HISTORY methods"
        ''' <summary>
        ''' When a Test/SampleType is deleted in Parameters Programming Screen, if it exists in the corresponding table in Historics Module, then it 
        ''' is marked as closed by updating field ClosedTestSample = TRUE.  Additionally, field ClosedTestVersion is also updated to TRUE and all
        ''' Reagents linked to the Test as marked as closed (if they are not used for another Tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier in Parameters Programming Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2012
        ''' Modified by: SA 20/09/2012 - Method has been moved from class HisTestSamplesDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisTestSamples are called
        '''              SA 09/10/2012 - Defined as optional the pSampleType parameter (it will be "" when Test is deleted, and informed when the 
        '''                              Test/SampleType is deleted)
        ''' </remarks>
        Public Function HIST_CloseTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "") _
                                             As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Check if the Test/Sample Type already exists in Historics Module
                        Dim myDAO As New thisTestSamplesDAO
                        Dim myHistResultsDAO As New thisWSResultsDAO

                        resultData = myDAO.ReadByTestIDAndSampleType(dbConnection, pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myHisTestSamplesDS As HisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                            Dim sampleClass As String = String.Empty
                            If (pSampleType <> "") Then sampleClass = "CALIB"

                            For Each hisRow As HisTestSamplesDS.thisTestSamplesRow In myHisTestSamplesDS.thisTestSamples.Rows
                                resultData = myHistResultsDAO.CloseOLDBlankCalibResults(dbConnection, hisRow.HistTestID, hisRow.SampleType, "", sampleClass)

                                'The Test/SampleType already exists in Historics Module; close it, and close also the TestVersion
                                resultData = myDAO.CloseTestSampleAndTestVersion(dbConnection, hisRow.HistTestID, hisRow.SampleType)

                                If (Not resultData.HasError) Then
                                    'Verify if the first Reagent is used for another open Test/SampleType saved in Historic Module
                                    resultData = myDAO.VerifyReagentInUse(dbConnection, hisRow.HistReagent1ID, hisRow.HistTestID, hisRow.SampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        If (CType(resultData.SetDatos, Integer) = 0) Then
                                            'Mark the Reagent as closed
                                            Dim myReagentsDelegate As New ReagentsDelegate
                                            resultData = myReagentsDelegate.HIST_CloseReagent(dbConnection, hisRow.HistReagent1ID)
                                        End If
                                    End If
                                End If

                                If (Not resultData.HasError) Then
                                    If (Not hisRow.IsHistReagent2IDNull) Then
                                        'Verify if the second Reagent is used for another open Test/SampleType saved in Historic Module
                                        resultData = myDAO.VerifyReagentInUse(dbConnection, hisRow.HistReagent2ID, hisRow.HistTestID, hisRow.SampleType)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            If (CType(resultData.SetDatos, Integer) = 0) Then
                                                'Mark the Reagent as closed
                                                Dim myReagentsDelegate As New ReagentsDelegate
                                                resultData = myReagentsDelegate.HIST_CloseReagent(dbConnection, hisRow.HistReagent2ID)
                                            End If
                                        End If
                                    End If
                                End If

                                If resultData.HasError Then Exit For
                            Next
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.HIST_CloseTestSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set field thisTestSamples.ClosedTestVersion = TRUE for the version currently active for an specific Test/SampleType 
        ''' Also set the field thisWSResults.ClosedResults = TRUE
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier in Parameters Programming Module</param>
        ''' <param name="pSampleClass">Sample Class Code: BLANK or CALIB </param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter, informed only when Sample Class is CALIB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2012
        ''' Modified by: SA 20/09/2012 - Method has been moved from class HisTestSamplesDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisTestSamples are called
        '''              AG 08/10/2012 - Completed the business logic. After calling CloseTestVersion, close also the affected Blank or Calibrator 
        '''                              Historic results
        '''              SA 19/10/2012 - Declared all variables outside loops; implementation changed
        ''' </remarks>
        Public Function HIST_CloseTestVersion(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleClass As String, _
                                              Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistDAO As New thisTestSamplesDAO
                        Dim myHistResultsDAO As New thisWSResultsDAO
                        Dim myHisTestSamplesDS As New HisTestSamplesDS

                        Dim myHistTestID As Integer = -1
                        Dim myHistSampleType As String = ""
                        Dim closeResultFlag As Boolean = True
                        Dim SampleClasses() As String = {"CALIB", "BLANK"}

                        'Search the ID of the Test/SampleType in Historics Module by TestID and SampleType (if it is informed)
                        resultData = myHistDAO.ReadByTestIDAndSampleType(dbConnection, pTestID, pSampleType, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myHisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                            'If an specific SampleType was not informed, all Sample Types for the Test are returned
                            For Each hisRow As HisTestSamplesDS.thisTestSamplesRow In myHisTestSamplesDS.thisTestSamples.Rows
                                myHistTestID = hisRow.HistTestID
                                myHistSampleType = hisRow.SampleType

                                'Close the current TestVersion for the HistTestID
                                resultData = myHistDAO.CloseTestVersion(dbConnection, myHistTestID, hisRow.SampleType)
                                If (resultData.HasError) Then Exit For

                                'If there are Blank and/or Calibrator results with ClosedResult=FALSE for the HistTestID
                                '** If (pSampleClass = BLANK) Then CLOSE BOTH, BLANK AND CALIBRATOR RESULTS
                                '** If (pSampleClass = CALIB) Then CLOSE ONLY CALIBRATOR RESULTS 
                                For Each sclass As String In SampleClasses
                                    closeResultFlag = True
                                    If (sclass = "BLANK") Then
                                        myHistSampleType = ""
                                        If (pSampleClass = "CALIB") Then closeResultFlag = False
                                    Else
                                        'CALIB
                                        myHistSampleType = pSampleType
                                    End If

                                    If (closeResultFlag) Then
                                        resultData = myHistResultsDAO.CloseOLDBlankCalibResults(dbConnection, myHistTestID, myHistSampleType, "", sclass)
                                        If (resultData.HasError) Then Exit For
                                    End If
                                Next
                                If (resultData.HasError) Then Exit For
                            Next

                            If (Not resultData.HasError) Then
                                'If the TestVersion has been closed due to changes in Calibration values....
                                If (pSampleClass = "CALIB" AndAlso pSampleType <> String.Empty) Then
                                    'Verify if for the Test there are other SampleTypes using the informed SampleType as alternative Calibration values
                                    Dim myTestSamplesDAO As New tparTestSamplesDAO
                                    resultData = myTestSamplesDAO.GetSampleTypesUsingAlternativeCalib(dbConnection, pTestID, pSampleType)

                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        Dim myTestSamplesDS As TestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                                        For Each affectedSampleType As TestSamplesDS.tparTestSamplesRow In myTestSamplesDS.tparTestSamples.Rows
                                            'Verify if there is an open version for the TestID/affected SampleType in Historic Module
                                            resultData = myHistDAO.ReadByTestIDAndSampleType(dbConnection, pTestID, affectedSampleType.SampleType, True)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myHisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                                                If (myHisTestSamplesDS.thisTestSamples.Rows.Count > 0) Then
                                                    myHistTestID = myHisTestSamplesDS.thisTestSamples.First.HistTestID

                                                    'Close the current TestVersion for the HistTestID and the affected SampleType
                                                    resultData = myHistDAO.CloseTestVersion(dbConnection, myHistTestID, affectedSampleType.SampleType)
                                                    If (resultData.HasError) Then Exit For
                                                End If
                                            Else
                                                'Error searching if there is an open version for the TestID/affected SampleType in Historic Module
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.HIST_CloseTestVersion", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When an experimental Calibrator is marked as closed in Historics Module, the active TestVersion of all Test/Samples using 
        ''' that Calibrator have to be also marked as closed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' Modified by: SA 17/09/2012 - Method has been moved from class HisTestSamplesDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisTestSamples are called
        '''              AG 09/10/2012 - Changed implementation: new version adapted from HIST_CloseTestVersion
        '''              SA 19/10/2012 - Declared all variables outside loops; implementation changed
        ''' </remarks>
        Public Function HIST_CloseTestVersionByHistCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistDAO As New thisTestSamplesDAO
                        Dim myHistResultsDAO As New thisWSResultsDAO
                        Dim myHistOrderTestDAO As New thisWSOrderTestsDAO

                        Dim histOTDS As New HisWSOrderTestsDS
                        Dim myHisTestSamplesDS As New HisTestSamplesDS

                        'Search all Tests/SampleTypes linked to the Experimental Calibrator and having a not CLOSED TestVersion
                        resultData = myHistDAO.ReadByCalibratorID(dbConnection, pHistCalibratorID, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myHisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                            If (myHisTestSamplesDS.thisTestSamples.Rows.Count > 0) Then
                                For Each hisTestSampleRow As HisTestSamplesDS.thisTestSamplesRow In myHisTestSamplesDS.thisTestSamples.Rows
                                    'Close all OPEN Calibrator Results (if any) for the Test/SampleType
                                    resultData = myHistResultsDAO.CloseOLDBlankCalibResults(dbConnection, hisTestSampleRow.HistTestID, _
                                                                                            hisTestSampleRow.SampleType, "", "CALIB")
                                    If (resultData.HasError) Then Exit For
                                Next

                                If (Not resultData.HasError) Then
                                    'Finally, close TestVersion of all Tests/SampleTypes linked to the HistCalibratorID
                                    resultData = myHistDAO.CloseTestVersionByHistCalibratorID(dbConnection, pHistCalibratorID)
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.HIST_CloseTestVersionByHistCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When Test data is changed in Parameters Programming Module, if the Test already exists in Historic Module, data is updated in thisTestSamples 
        ''' table for all the SampleTypes. However, if the data changed was the CalibratorType and/or the CalibrationFactor, then the current TestVersion of
        ''' the Test/SampleType in Historic Module is marked as closed, and the data is not updated
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/10/2012
        ''' </remarks>
        Public Function HIST_Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim testHistUpdate As New HisTestSamplesDS 'Data to return
                        Dim toHistCloseTestVersion As New HisTestSamplesDS 'When Test/SampleType uses CalibrationFactor and the Factor value changes -> Close the Historic TestVersion

                        Dim myDAO As New tparTestSamplesDAO
                        resultData = myDAO.HIST_FillHisTestSamplesByTestID(dbConnection, pTestID, pSampleType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim testDefinition As HisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                            Dim myHistDAO As New thisTestSamplesDAO
                            Dim currentHist As New HisTestSamplesDS

                            Dim closeTestVersion As Boolean = False
                            Dim myTestCalibratorsDS As New TestSampleCalibratorDS
                            Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

                            For Each row As HisTestSamplesDS.thisTestSamplesRow In testDefinition.thisTestSamples.Rows
                                closeTestVersion = False

                                'Read if exists a NOT CLOSED Test Version in Historic Module
                                resultData = myHistDAO.ReadByTestIDAndSampleType(dbConnection, row.TestID, row.SampleType, True)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    currentHist = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                                    If (currentHist.thisTestSamples.Rows.Count > 0) Then
                                        'If exists get the HistTestID and the TestVersionNumber
                                        row.BeginEdit()
                                        If (Not currentHist.thisTestSamples(0).IsHistTestIDNull) Then row.HistTestID = currentHist.thisTestSamples(0).HistTestID Else row.SetHistTestIDNull()
                                        If (Not currentHist.thisTestSamples(0).IsTestVersionNumberNull) Then row.TestVersionNumber = currentHist.thisTestSamples(0).TestVersionNumber Else row.SetTestVersionNumberNull()
                                        row.EndEdit()

                                        'Evaluate if the Historical Test Version must be closed (if the Calibration Factor has been changed)
                                        If (Not row.IsCalibratorFactorNull AndAlso Not currentHist.thisTestSamples(0).IsCalibratorFactorNull AndAlso _
                                            row.CalibratorFactor <> currentHist.thisTestSamples(0).CalibratorFactor) Then
                                            'Close TestVersion
                                            toHistCloseTestVersion.thisTestSamples.ImportRow(row)
                                            toHistCloseTestVersion.thisTestSamples.AcceptChanges()
                                        Else
                                            'Update data of the Test/SampleType in the active TestVersion
                                            testHistUpdate.thisTestSamples.ImportRow(row)
                                            testHistUpdate.thisTestSamples.AcceptChanges()
                                        End If


                                        'If (row.CalibratorType <> currentHist.thisTestSamples.First.CalibratorType) Then
                                        '    closeTestVersion = True
                                        'ElseIf (row.CalibratorType = "FACTOR" AndAlso row.CalibratorFactor <> currentHist.thisTestSamples.First.CalibratorFactor) Then
                                        '    closeTestVersion = True
                                        'Else
                                        '    'Get current data of the Experimental Calibration for the Test/SampleType 
                                        '    resultData = myTestCalibratorsDelegate.GetTestCalibratorData(dbConnection, row.TestID, row.SampleType)
                                        '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        '        myTestCalibratorsDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                                        '        If (myTestCalibratorsDS.tparTestCalibrators.Rows.Count > 0) Then
                                        '            If (myTestCalibratorsDS.tparTestCalibrators.First.CalibratorID <> currentHist.thisTestSamples(0).CalibratorID) Then
                                        '                closeTestVersion = True
                                        '            Else
                                        '                If (Not myTestCalibratorsDS.tparTestCalibrators.First.IsCurveTypeNull) Then
                                        '                    If (myTestCalibratorsDS.tparTestCalibrators.First.CurveType <> currentHist.thisTestSamples.First.CurveType) OrElse _
                                        '                        (myTestCalibratorsDS.tparTestCalibrators.First.CurveGrowthType <> currentHist.thisTestSamples.First.CurveGrowthType) OrElse _
                                        '                        (myTestCalibratorsDS.tparTestCalibrators.First.CurveAxisXType <> currentHist.thisTestSamples.First.CurveAxisXType) OrElse _
                                        '                        (myTestCalibratorsDS.tparTestCalibrators.First.CurveAxisYType <> currentHist.thisTestSamples.First.CurveAxisYType) Then
                                        '                        closeTestVersion = True
                                        '                    End If
                                        '                End If
                                        '            End If
                                        '        End If
                                        '    End If
                                        'End If

                                        'If (Not resultData.HasError) Then
                                        '    If (closeTestVersion) Then
                                        '        'Close TestVersion
                                        '        toHistCloseTestVersion.thisTestSamples.ImportRow(row)
                                        '        toHistCloseTestVersion.thisTestSamples.AcceptChanges()
                                        '    Else
                                        '        'Update data of the Test/SampleType in the active TestVersion
                                        '        testHistUpdate.thisTestSamples.ImportRow(row)
                                        '        testHistUpdate.thisTestSamples.AcceptChanges()
                                        '    End If
                                        'End If
                                    End If
                                Else
                                    'Error searching if there is a not closed TestVersion of the Test/SampleType in Historic Module
                                    Exit For
                                End If
                            Next

                            'Finally update and/or close test versions in historic
                            If (Not resultData.HasError) Then
                                If (testHistUpdate.thisTestSamples.Rows.Count > 0) Then
                                    resultData = myHistDAO.Update(dbConnection, testHistUpdate)
                                End If

                                If (Not resultData.HasError AndAlso toHistCloseTestVersion.thisTestSamples.Rows.Count > 0) Then
                                    For Each row As HisTestSamplesDS.thisTestSamplesRow In toHistCloseTestVersion.thisTestSamples.Rows
                                        'When changes the CalibratorFactor close TestVersion by CALIB
                                        resultData = HIST_CloseTestVersion(dbConnection, row.TestID, "CALIB", row.SampleType)
                                        If (resultData.HasError) Then Exit For
                                    Next
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = testHistUpdate
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.HIST_Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "TO DELETE?"


        '''' <summary>
        '''' Delete a test sample by the test id an the 
        '''' Sample type.
        '''' Before deleting a sample type all related rows on 
        '''' table TestReagent volume nedd to be deleted, this method
        '''' is not supose to implement by it self. need to do a delete
        '''' Cascade.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pTestID ">Test ID</param>
        '''' <param name="pSampleType">Sample Type</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATE BY: TR 16/03/2010.
        '''' </remarks>
        'Public Function DeleteByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                'Dim tempTestSamplesDS As New TestSamplesDS()
        '                Dim myTestSamplesDAO As New tparTestSamplesDAO()

        '                myGlobalDataTO = myTestSamplesDAO.Delete(dbConnection, pTestID, pSampleType)

        '                If (Not myGlobalDataTO.HasError) Then
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
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "TestSamplesDelegate.Update", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function
#End Region

    End Class
End Namespace
