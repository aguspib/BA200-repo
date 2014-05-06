Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL

    Public Class TestCalibratorsDelegate

#Region "Methods"

        ''' <summary>
        ''' Due to continous SystemError I decide to create it and use it in TestCalibratorValues.Create method
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 12/11/2010
        ''' </remarks>
        Public Function Exists(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pTestCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDao As New tparTestCalibratorsDAO
                        resultData = myDao.Exists(dbConnection, pTestCalibratorID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.Exists", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add all values of a link between an Experimental Calibrator and an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorDS">Typed DataSet TestCalibratorsDS containing the information to add</param>
        ''' <returns>GlobalDataTO containing success/error information. When success also return the same entry 
        '''          DataSet updated with the identity value automatically generated for the DB for field TestCalibratorID</returns>
        ''' <remarks>
        ''' Created by : TR 09/06/2010 
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pTestCalibratorDS As TestCalibratorsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim tempTestCalibratorDS As New TestCalibratorsDS
                        Dim myTestSampleCalibratorDS As New TestSampleCalibratorDS
                        Dim mytparTestCalibratorsDAO As New tparTestCalibratorsDAO
                        Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate

                        For Each TestCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In pTestCalibratorDS.tparTestCalibrators.Rows
                            'Validate if the Test and SampleType have a link with an Experimental Calibrator
                            myGlobalDataTO = mytparTestCalibratorsDAO.GetTestCalibratorData(dbConnection, TestCalibRow.TestID, TestCalibRow.SampleType)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestSampleCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)

                                If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 0) Then
                                    'Test/SampleType exists in tparTestCalibrators, delete the defined Calibration Values
                                    myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, myTestSampleCalibratorDS.tparTestCalibrators(0).TestCalibratorID)
                                    If (Not myGlobalDataTO.HasError) Then
                                        'Delete the current link of the Test/SampleType with an Experimental Calibrator
                                        myGlobalDataTO = mytparTestCalibratorsDAO.DeleteByTestID(dbConnection, TestCalibRow.TestID, TestCalibRow.SampleType)
                                        If (myGlobalDataTO.HasError) Then
                                            'Error deleting current information for the Test/SampleType in tparTestCalibrators
                                            Exit For
                                        End If
                                    Else
                                        'Error deleting current information for the Test/SampleType in tparTestCalibratorsValues
                                        Exit For
                                    End If
                                End If
                            Else
                                'Error validating if the Test and SampleType have a link with an Experimental Calibrator
                                Exit For
                            End If

                            'Create the new link between the Test/SampleType and an Experimental Calibrator
                            tempTestCalibratorDS.tparTestCalibrators.Clear()
                            tempTestCalibratorDS.tparTestCalibrators.ImportRow(TestCalibRow)

                            myGlobalDataTO = mytparTestCalibratorsDAO.Create(dbConnection, tempTestCalibratorDS)
                            If (Not myGlobalDataTO.HasError) Then
                                pTestCalibratorDS = tempTestCalibratorDS
                            Else
                                'Error adding information to tparTestCalibrators
                                Exit For
                            End If
                        Next

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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update values of fields containing the Curve definition for an Experimental Calibrator when it is used
        ''' for an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorDS">Typed DataSet TestCalibratorsDS containing the information to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by : TR 09/06/2010 
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorDS As TestCalibratorsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim tempTestCalibratorDS As New TestCalibratorsDS
                        Dim mytparTestCalibratorsDAO As New tparTestCalibratorsDAO

                        For Each TestCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In pTestCalibratorDS.tparTestCalibrators.Rows
                            tempTestCalibratorDS.tparTestCalibrators.Clear()
                            tempTestCalibratorDS.tparTestCalibrators.ImportRow(TestCalibRow)

                            myGlobalDataTO = mytparTestCalibratorsDAO.Update(dbConnection, tempTestCalibratorDS)
                            If (myGlobalDataTO.HasError) Then
                                'Error executing the update
                                Exit For
                            End If
                        Next

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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the Calibrator used for an specific Test and Sample Type. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS with all data of the
        '''          Calibrator needed for the specified Test and SampleType. When the Calibrator is Alternative
        '''          two rows are returned in the DataSet: the first one will contain the Calibrator data for the 
        '''          Test and the Alternative Sample Type, and the second one will containing the information of 
        '''          the informed Test and SampleType</returns>
        ''' <remarks>
        ''' Modified by: SA 09/02/2010 - Added field CalibratorReplicates to DataSet TestSampleCalibratorDS
        '''              SA 23/02/2010 - Function GetCalibratorDefinition in TestSamplesDelegate now return a GlobalDataTO; changes 
        '''                              needed when calling this function were implemented.
        '''              SA 31/08/2010 - For each Test/SampleType using an Experimental Calibrator marked as SpecialCalib, function 
        '''                              searches if Setting TOTAL_CAL_POINTS is defined for it, in which case, updates field 
        '''                              NumberOfCalibrators in the DS to return with the value of the Setting.  Same process is
        '''                              executed for each Test/SampleType using an Alternative Calibrator defined as Experimental
        '''                              and marked as SpecialCalib
        '''              SA 02/10/2012 - For Experimental Calibrators, added new field RealNumOfCalibrators in the DS to save the Number
        '''                              of Points of the Calibrator when it is marked as Special, in which case, field NumberOfCalibrators
        '''                              contains the real number of points used for the Test/SampleType. If the Calibrator is not marked
        '''                              as Special, both fields will contain the same value
        ''' </remarks>
        Public Function GetTestCalibratorData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                              ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Calibrator Type defined for the informed Test and Sample Type
                        Dim dataMissing As Boolean = False
                        Dim testCalibratorDataDS As New TestSampleCalibratorDS
                        Dim myTestSamplesDelegate As New TestSamplesDelegate
                        Dim testCalibratorData As New tparTestCalibratorsDAO

                        resultData = myTestSamplesDelegate.GetDefinition(dbConnection, pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim testSampleDataDS As TestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                            If (testSampleDataDS.tparTestSamples.Rows.Count = 0) Then
                                dataMissing = True  'There is no information for the Test and Sample Type in tparTestSamples
                            Else
                                If (testSampleDataDS.tparTestSamples(0).CalibratorType = "FACTOR") Then
                                    'If the Calibrator needed is a theorical Factor, add a row in typed DataSet TestSampleCalibratorDS
                                    'informing CalibratorType, CalibratorFactor and CalibratorReplicates
                                    Dim testCalibratorRow As TestSampleCalibratorDS.tparTestCalibratorsRow

                                    testCalibratorRow = testCalibratorDataDS.tparTestCalibrators.NewtparTestCalibratorsRow
                                    testCalibratorRow.TestID = pTestID
                                    testCalibratorRow.SampleType = pSampleType
                                    testCalibratorRow.CalibratorType = testSampleDataDS.tparTestSamples(0).CalibratorType
                                    testCalibratorRow.AlternativeFlag = False
                                    testCalibratorRow.CalibratorFactor = testSampleDataDS.tparTestSamples(0).CalibrationFactor

                                    If Not testSampleDataDS.tparTestSamples(0).IsCalibratorReplicatesNull Then
                                        testCalibratorRow.CalibratorReplicates = testSampleDataDS.tparTestSamples(0).CalibratorReplicates
                                    End If
                                    testCalibratorDataDS.tparTestCalibrators.Rows.Add(testCalibratorRow)

                                ElseIf (testSampleDataDS.tparTestSamples(0).CalibratorType = "EXPERIMENT") Then
                                    'If the Calibrator needed is Experimental get the Calibrator data for the indicated Test and SampleType         
                                    resultData = testCalibratorData.GetTestCalibratorData(dbConnection, pTestID, pSampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        testCalibratorDataDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)
                                        If (testCalibratorDataDS.tparTestCalibrators.Rows.Count = 0) Then
                                            dataMissing = True   'There is not information for the Test and SampleType in tparTestCalibrators
                                        Else
                                            '...If the Calibrator needed is marked as SpecialCalib, search if Setting TOTAL_CAL_POINTS is defined for
                                            'the Test/SampleType to set the real number of Calibrator points in the kit used for it
                                            Dim realNumberOfCalibrators As Integer = testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                            If (testCalibratorDataDS.tparTestCalibrators(0).SpecialCalib) Then
                                                Dim mySpTestSettingsDelegate As New SpecialTestsSettingsDelegate
                                                resultData = mySpTestSettingsDelegate.Read(dbConnection, testSampleDataDS.tparTestSamples(0).TestID, _
                                                                                           testSampleDataDS.tparTestSamples(0).SampleType, _
                                                                                           GlobalEnumerates.SpecialTestsSettings.TOTAL_CAL_POINTS.ToString)

                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                    Dim mySpTestSettingsDS As SpecialTestsSettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)

                                                    If (mySpTestSettingsDS.tfmwSpecialTestsSettings.Rows.Count = 1) Then
                                                        realNumberOfCalibrators = Convert.ToInt32(mySpTestSettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                                    End If
                                                End If
                                            End If

                                            If (Not resultData.HasError) Then
                                                'Complete the Calibrator data informing CalibratorType, CalibratorReplicates and the number of 
                                                'Calibrator points
                                                testCalibratorDataDS.BeginInit()
                                                testCalibratorDataDS.tparTestCalibrators(0).CalibratorType = "EXPERIMENT"
                                                testCalibratorDataDS.tparTestCalibrators(0).AlternativeFlag = False
                                                testCalibratorDataDS.tparTestCalibrators(0).CalibratorReplicates = testSampleDataDS.tparTestSamples(0).CalibratorReplicates
                                                testCalibratorDataDS.tparTestCalibrators(0).RealNumOfCalibrators = testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                                testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators = realNumberOfCalibrators
                                                testCalibratorDataDS.EndInit()
                                            End If
                                        End If
                                    Else
                                        dataMissing = True   'There is not information for the Test and SampleType in tparTestCalibrators
                                    End If

                                ElseIf (testSampleDataDS.tparTestSamples(0).CalibratorType = "ALTERNATIV") Then
                                    'For Alternative Calibrators, get the CalibratorType of the indicated Alternative Sample Type
                                    If (Not testSampleDataDS.tparTestSamples(0).IsSampleTypeAlternativeNull) Then
                                        Dim auxSampleType As String = pSampleType
                                        auxSampleType = testSampleDataDS.tparTestSamples(0).SampleTypeAlternative

                                        'Get the Calibrator Type defined for the Test and Sample Type
                                        Dim mypTestSamplesDelegate As New TestSamplesDelegate
                                        resultData = mypTestSamplesDelegate.GetDefinition(dbConnection, pTestID, auxSampleType)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            testSampleDataDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                                            If (testSampleDataDS.tparTestSamples.Rows.Count = 0) Then
                                                dataMissing = True   'There is not information for the Test and the Alternative Sample Type in tparTestSamples
                                            Else
                                                If (testSampleDataDS.tparTestSamples(0).CalibratorType = "FACTOR") Then
                                                    'If the Calibrator needed is a theorical Factor, add a row in typed DataSet TestSampleCalibratorDS
                                                    'informing CalibratorType, CalibratorFactor and CalibratorReplicates
                                                    Dim myTestSampleCalibRow As TestSampleCalibratorDS.tparTestCalibratorsRow

                                                    myTestSampleCalibRow = testCalibratorDataDS.tparTestCalibrators.NewtparTestCalibratorsRow()
                                                    myTestSampleCalibRow.TestID = pTestID
                                                    myTestSampleCalibRow.SampleType = auxSampleType
                                                    myTestSampleCalibRow.CalibratorType = testSampleDataDS.tparTestSamples(0).CalibratorType
                                                    myTestSampleCalibRow.AlternativeFlag = False
                                                    myTestSampleCalibRow.CalibratorFactor = testSampleDataDS.tparTestSamples(0).CalibrationFactor
                                                    myTestSampleCalibRow.CalibratorReplicates = testSampleDataDS.tparTestSamples(0).CalibratorReplicates
                                                    testCalibratorDataDS.tparTestCalibrators.AddtparTestCalibratorsRow(myTestSampleCalibRow)

                                                ElseIf (testSampleDataDS.tparTestSamples(0).CalibratorType = "EXPERIMENT") Then
                                                    'If the Calibrator needed is Experimental, get the Calibrator data for the Test and Alternative Sample Type
                                                    resultData = testCalibratorData.GetTestCalibratorData(dbConnection, pTestID, auxSampleType)
                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        testCalibratorDataDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)
                                                        If (testCalibratorDataDS.tparTestCalibrators.Rows.Count = 0) Then
                                                            dataMissing = True  ''There is not information for the Test and the Alternative SampleType in tparTestCalibrators
                                                        Else
                                                            '...If the Calibrator needed is marked as SpecialCalib, search if Setting TOTAL_CAL_POINTS is defined for
                                                            'the Test/AlternativeSampleType to set the real number of Calibrator points in the kit used for it
                                                            Dim realNumberOfCalibrators As Integer = testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                                            If (testCalibratorDataDS.tparTestCalibrators(0).SpecialCalib) Then
                                                                Dim mySpTestSettingsDelegate As New SpecialTestsSettingsDelegate
                                                                resultData = mySpTestSettingsDelegate.Read(dbConnection, testSampleDataDS.tparTestSamples(0).TestID, _
                                                                                                           auxSampleType, GlobalEnumerates.SpecialTestsSettings.TOTAL_CAL_POINTS.ToString)

                                                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                                    Dim mySpTestSettingsDS As SpecialTestsSettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)
                                                                    If (mySpTestSettingsDS.tfmwSpecialTestsSettings.Rows.Count = 1) Then
                                                                        realNumberOfCalibrators = Convert.ToInt32(mySpTestSettingsDS.tfmwSpecialTestsSettings(0).SettingValue)
                                                                    End If
                                                                End If
                                                            End If

                                                            'Set data of the Calibrator used for the Test with the Alternative Sample Type
                                                            testCalibratorDataDS.BeginInit()
                                                            testCalibratorDataDS.tparTestCalibrators(0).CalibratorType = testSampleDataDS.tparTestSamples(0).CalibratorType
                                                            testCalibratorDataDS.tparTestCalibrators(0).AlternativeFlag = False
                                                            If Not testSampleDataDS.tparTestSamples(0).IsCalibratorReplicatesNull Then
                                                                testCalibratorDataDS.tparTestCalibrators(0).CalibratorReplicates = testSampleDataDS.tparTestSamples(0).CalibratorReplicates
                                                            End If
                                                            testCalibratorDataDS.tparTestCalibrators(0).RealNumOfCalibrators = testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators
                                                            testCalibratorDataDS.tparTestCalibrators(0).NumberOfCalibrators = realNumberOfCalibrators
                                                            testCalibratorDataDS.EndInit()

                                                            'Add an additional row for the Test and original Sample Type indicating the Calibrator used is an Alternative one
                                                            Dim myTestSampleCalibRow As TestSampleCalibratorDS.tparTestCalibratorsRow

                                                            myTestSampleCalibRow = testCalibratorDataDS.tparTestCalibrators.NewtparTestCalibratorsRow()
                                                            myTestSampleCalibRow.TestID = pTestID
                                                            myTestSampleCalibRow.SampleType = pSampleType
                                                            myTestSampleCalibRow.CalibratorType = "ALTERNATIV"
                                                            myTestSampleCalibRow.AlternativeFlag = True
                                                            myTestSampleCalibRow.AlternativeSampleType = auxSampleType
                                                            testCalibratorDataDS.tparTestCalibrators.AddtparTestCalibratorsRow(myTestSampleCalibRow)
                                                        End If
                                                    Else
                                                        dataMissing = True  'There is not information for the Test and the Alternative SampleType in tparTestCalibrators
                                                    End If
                                                End If
                                            End If
                                        Else
                                            dataMissing = True   'There is not information for the Test and the Alternative Sample Type in tparTestSamples
                                        End If
                                    Else
                                        dataMissing = True   'The alternative Sample Type is not informed in tparTestSamples
                                    End If
                                End If
                            End If
                        Else
                            dataMissing = True   'There is no information for the Test and Sample Type in tparTestSamples
                        End If

                        If (dataMissing) Then
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                        Else
                            'Calibrator data is returned
                            resultData.HasError = False
                            resultData.SetDatos = testCalibratorDataDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.GetTestCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data defined for a calibrator 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleType Code</param>
        ''' <returns>Dataset with structure of tables tparTestCalibratorsValues</returns>
        ''' <remarks>
        ''' Created by:  DL 23/02/2010
        ''' Modified by: AG 01/09/2010 - Added optional parameter pSampleType
        ''' </remarks>
        Public Function GetTestCalibratorValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestCalibrators As New tparTestCalibratorsDAO
                        resultData = mytparTestCalibrators.GetTestCalibratorValues(pDBConnection, pTestID, pSampleType)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.GetTestCalibratorValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Theorical Concentration values for all points of the Experimental Calibrator used for the specified Test/SampleType
        ''' Besides, if the Calibrator is multipoint, values for definition of the Calibration Curve are obtained (data will be duplicated
        ''' for each point, so it is enough get these values for the first point) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorsValuesDS with all the information</returns>
        ''' <remarks>
        ''' Created  by: SA 12/07/2012 - Based in GetTestCalibratorValuesNEW but: parameter SampleType is required, not optional, and
        '''                              the new function in DAO Class is called
        ''' </remarks>
        Public Function GetTestCalibratorValuesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                   ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestCalibrators As New tparTestCalibratorsDAO
                        resultData = mytparTestCalibrators.GetTestCalibratorValuesNEW(pDBConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.GetTestCalibratorValuesNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all links the specified Test has with Experimental Calibrators or, if a SampleType
        ''' is informed, only the link between the specified Test/SampleType and an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleT Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing typed DataSet TestCalibratorsDS with the links between 
        '''          the specified Test (and optionally SampleType) and Experimental Calibrators</returns>
        ''' <remarks>
        ''' Created by: TR 17/05/2010
        ''' </remarks>
        Public Function GetTestCalibratorByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                  Optional ByVal pSampleType As String = "") As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestCalibrators As New tparTestCalibratorsDAO
                        resultData = mytparTestCalibrators.GetTestCalibratorByTestID(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.GetTestCalibratorByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of all Tests/SampleTypes linked to the specified Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS with data of all
        '''          Tests/SampleTypes linked to the specified Calibrator</returns>
        ''' <remarks>
        ''' Created by: TR 03/06/2010
        ''' </remarks>
        Public Function GetAllTestCalibratorByCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                           ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestCalibrators As New tparTestCalibratorsDAO
                        myGlobalDataTO = mytparTestCalibrators.GetAllTestCalibratorByCalibratorID(pDBConnection, pCalibratorID)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.GetAllTestCalibratorByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all links the specified Test has with experimental Calibrators or, if a SampleType
        ''' is informed, only the link between the specified Test/SampleType and an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleT Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 17/05/2010
        ''' </remarks>
        Public Function DeleteTestCaliByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                               Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError) AndAlso (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytparTestCalibratorsDAO As New tparTestCalibratorsDAO
                        myGlobalDataTO = mytparTestCalibratorsDAO.DeleteByTestID(dbConnection, pTestID, pSampleType)

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

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestCalibratorsDelegate.DeleteTestCaliByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the Calibrator to which corresponds the specified TestID, SampleType
        ''' (Used in calculations class)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS containing the 
        '''          data of the Calibrator to which corresponds the specified TestID-SampleType</returns>
        ''' <remarks>
        ''' Created by:  AG 02/09/2010
        ''' </remarks>
        Public Function GetCalibratorDataForCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                         ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparTestCalibratorsDAO
                        resultData = myDAO.GetCalibratorDataForCalculations(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace
