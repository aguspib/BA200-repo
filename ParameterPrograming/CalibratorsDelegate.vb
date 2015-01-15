Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.BL
    Public Class CalibratorsDelegate

#Region "Public methods"
        ''' <summary>
        ''' Add a new Calibrator to Calibrators table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing the data of the Calibrator to add</param>
        ''' <returns>GlobalDataTO containing the entry typed DataSet CalibratorsDS updated with value
        '''          of the CalibratorID generated automatically for the DB when the insert was successful; 
        '''          otherwise, the error information is returned</returns>
        ''' <remarks>
        ''' Created by:  TR 04/06/2010
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalibratorDAO As New tparCalibratorsDAO
                        Dim mytempCalibratorDS As New CalibratorsDS

                        For Each calibRow As CalibratorsDS.tparCalibratorsRow In pCalibratorsDS.tparCalibrators.Rows
                            mytempCalibratorDS.tparCalibrators.Clear()
                            mytempCalibratorDS.tparCalibrators.ImportRow(calibRow)

                            myGlobalDataTO = myCalibratorDAO.Create(dbConnection, mytempCalibratorDS)

                            'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            'RH 14/05/2012 Remove "Not myGlobalDataTO.SetDatos Is Nothing" because Create() function does
                            'return no value in SetDatos. Here we have a bug, because calibRow.CalibratorID will never get updated.
                            If Not myGlobalDataTO.HasError Then
                                'Set the identity value generated as Calibrator Identifier
                                calibRow.CalibratorID = mytempCalibratorDS.tparCalibrators(0).CalibratorID
                                calibRow.IsNew = False
                            Else : Exit For
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the specified Calibrator with all its related elements
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsID">Identifier of the Calibrator to be deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 10/06/2010
        ''' Modified by: TR 25/02/2011 -Change the enable status for each test.
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pCalibratorsID As Integer, _
                               ByVal pAnalyzerID As String, _
                               ByVal pWorkSessionID As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all TestID/SampleType linked to the specified Calibrator
                        Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
                        myGlobalDataTO = myTestCalibratorDelegate.GetAllTestCalibratorByCalibratorID(dbConnection, pCalibratorsID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myTestSampleCalibratorDS As TestSampleCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)

                            Dim myTestSampleDelegate As New TestSamplesDelegate
                            Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate

                            For Each testSampleCalRow As TestSampleCalibratorDS.tparTestCalibratorsRow In myTestSampleCalibratorDS.tparTestCalibrators.Rows
                                'Remove Concentration values for all TestID/SampleType linked to the Calibrator
                                myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, testSampleCalRow.TestCalibratorID)
                                If (Not myGlobalDataTO.HasError) Then
                                    'Remove the link between the TestID/SampleTypes and the Calibrator
                                    myGlobalDataTO = myTestCalibratorDelegate.DeleteTestCaliByTestID(dbConnection, testSampleCalRow.TestID, testSampleCalRow.SampleType)
                                    If (Not myGlobalDataTO.HasError) Then
                                        'The TestID/SampleTypes are marked as not enabled while they remain without linked Calibrator
                                        myGlobalDataTO = myTestSampleDelegate.UpdateTestSampleEnableStatus(dbConnection, testSampleCalRow.TestID, _
                                                                                                           testSampleCalRow.SampleType, False)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Else
                                        'Error deleting the link between the TestIDs/SampleTypes and the Calibrator
                                        Exit For
                                    End If
                                Else
                                    'Error deleting the Concentration values 
                                    Exit For
                                End If
                            Next
                        End If

                        'If all relations where deleted, then remove the Calibrator
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myCalibratorDAO As New tparCalibratorsDAO
                            Dim myVRotorsDelegate As New VirtualRotorsDelegate


                            'DL 11/10/2012. Begin
                            'Delete positions in virtual rotor position and update positions in the rotor when this is not in use
                            Dim myVirtualRotorPosDelegate As New VirtualRotorsPositionsDelegate
                            Dim mySelectedElementInfo As New WSRotorContentByPositionDS
                            Dim myVirtualRotorPosDS As New VirtualRotorPosititionsDS

                            myGlobalDataTO = myVirtualRotorPosDelegate.GetPositionsByCalibrationID(Nothing, pCalibratorsID)

                            If Not myGlobalDataTO.HasError Then
                                myVirtualRotorPosDS = DirectCast(myGlobalDataTO.SetDatos, VirtualRotorPosititionsDS)

                                For Each myvirtualRow As VirtualRotorPosititionsDS.tparVirtualRotorPosititionsRow In myVirtualRotorPosDS.tparVirtualRotorPosititions.Rows
                                    Dim myRCPRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                    myRCPRow = mySelectedElementInfo.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                    myRCPRow.CalibratorID = myvirtualRow.CalibratorID
                                    myRCPRow.AnalyzerID = pAnalyzerID
                                    myRCPRow.WorkSessionID = pWorkSessionID
                                    myRCPRow.CellNumber = myvirtualRow.CellNumber
                                    myRCPRow.RingNumber = myvirtualRow.RingNumber
                                    myRCPRow.RotorType = "SAMPLES"
                                    myRCPRow.TubeContent = "CALIB"
                                    mySelectedElementInfo.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(myRCPRow)
                                Next myvirtualRow
                            End If

                            For Each myRotorContPosResetElem As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In mySelectedElementInfo.twksWSRotorContentByPosition.Rows
                                myRotorContPosResetElem.BeginEdit()
                                myRotorContPosResetElem.SetElementIDNull()
                                myRotorContPosResetElem.SetBarCodeInfoNull()
                                myRotorContPosResetElem.SetBarcodeStatusNull()
                                myRotorContPosResetElem.SetScannedPositionNull()
                                myRotorContPosResetElem.MultiTubeNumber = 0
                                myRotorContPosResetElem.SetTubeTypeNull()
                                myRotorContPosResetElem.SetTubeContentNull()
                                myRotorContPosResetElem.RealVolume = 0
                                myRotorContPosResetElem.RemainingTestsNumber = 0
                                myRotorContPosResetElem.Status = "FREE"
                                myRotorContPosResetElem.ElementStatus = "NOPOS"
                                myRotorContPosResetElem.Selected = False
                                myRotorContPosResetElem.EndEdit()
                            Next

                            'Clear content of selected rotor positions 
                            Dim myWSRotorContentByPosDAO As New twksWSRotorContentByPositionDAO
                            myGlobalDataTO = myWSRotorContentByPosDAO.Update(dbConnection, mySelectedElementInfo)

                            If Not myGlobalDataTO.HasError Then
                                'Remove virtual rotor position
                                myGlobalDataTO = myVRotorsDelegate.DeleteByCalibratorID(Nothing, pCalibratorsID, pAnalyzerID, pWorkSessionID)
                                If Not myGlobalDataTO.HasError Then
                                    myGlobalDataTO = myCalibratorDAO.Delete(dbConnection, pCalibratorsID)

                                    'AG 09/10/2012 - close calibrator in history results
                                    If GlobalConstants.HISTWorkingMode AndAlso Not myGlobalDataTO.HasError Then
                                        myGlobalDataTO = HIST_CloseCalibrator(dbConnection, pCalibratorsID)
                                    End If
                                End If
                            End If
                            'DL 11/10/2012. End

                            'myGlobalDataTO = myCalibratorDAO.Delete(dbConnection, pCalibratorsID)
                            ''AG 09/10/2012 - close calibrator in history results
                            'If GlobalConstants.HISTWorkingMode AndAlso Not myGlobalDataTO.HasError Then
                            'myGlobalDataTO = HIST_CloseCalibrator(dbConnection, pCalibratorsID)
                            'End If
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Experimental Calibrators from the Calibrators table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of all Calibrators</returns>
        ''' <remarks>
        ''' Created by:  TR 03/06/2010
        ''' Modified by: SA 09/05/2012 - Changed the function template 
        ''' </remarks>
        Public Function GetAllCalibrators(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calibratorsDataDAO As New tparCalibratorsDAO
                        myGlobalDataTO = calibratorsDataDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.GetAllCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get basic data of the specified Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of the informed Calibrator</returns>
        ''' <remarks>
        ''' Created by:  DL 21/01/2010
        ''' </remarks>
        Public Function GetCalibratorData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calibratorsDataDAO As New tparCalibratorsDAO
                        resultData = calibratorsDataDAO.Read(dbConnection, pCalibratorID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.GetCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns selected Calibrators info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="AppLang">Application Language</param>
        ''' <param name="SelectedCalibrators">List of selected Calibrators IDs</param>
        ''' <remarks>
        ''' Created by: RH 16/12/2011
        ''' </remarks>
        Public Function GetCalibratorsForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String, _
                                                Optional ByVal SelectedCalibrators As List(Of Integer) = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalibratorsDAO As New tparCalibratorsDAO
                        resultData = myCalibratorsDAO.GetCalibratorsForReport(dbConnection, AppLang, SelectedCalibrators)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.GetCalibratorsForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the specified Calibrator (searching by Calibrator Name)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorName">Calibrator Name</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalibratorDS with data of the informed Calibrator</returns>
        ''' <remarks>
        ''' Created by: TR 24/03/2011
        ''' </remarks>
        Public Function ReadByCalibratorName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorName As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim calibratorsDataDAO As New tparCalibratorsDAO
                        myGlobalDataTO = calibratorsDataDAO.ReadByCalibratorName(dbConnection, pCalibratorName)
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.ReadByCalibratorName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add/update/delete Calibrators with all relations (links with TestIDs/SampleType and Concentration values for each point)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing basic data of Calibrator to add or update</param>
        ''' <param name="pTestCalibratorsDS">Typed DataSet TestCalibratorsDS containing the TestID/SampleType linked to the
        '''                                  Calibrator to add or update</param>
        ''' <param name="pTestCalibratorValueDS">Typed DataSet TestCalibratorValuesDS containing Concentration Calibrator values for the 
        '''                                      linked Test/SampleType</param>
        ''' <param name="pDeleteCalibratorList">List of DeletedCalibratorTO containing the list of Calibrators selected to be deleted</param>
        ''' <param name="pSaveFromTestProgramming">When TRUE, it indicates the saving is executed from the Tests Programming Screen</param>
        ''' <param name="pDeleteCalibrationResult">When TRUE, it indicates all previous results saved for the Calibrator and the TestID/SampleType
        '''                                        have to be deleted</param>
        ''' <param name="pDeleteTestCalibratorValues">When TRUE, it indicates all Concentration values for the Calibrator and the TestID/SampleType
        '''                                           have to be deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: AG 12/11/2010 - Added parameter pSaveFromTestProgramming due to if a Test/SampleType is programmed as EXPERIMENTAL 
        '''                              and then changes to FACTOR, the current code continue saving experiment</remarks>
        Public Function Save(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS, ByVal pTestCalibratorsDS As TestCalibratorsDS, _
                             ByVal pTestCalibratorValueDS As TestCalibratorValuesDS, ByVal pDeleteCalibratorList As List(Of DeletedCalibratorTO), _
                             Optional ByVal pSaveFromTestProgramming As Boolean = False, Optional ByVal pDeleteCalibrationResult As Boolean = False, _
                             Optional ByVal pDeleteTestCalibratorValues As Boolean = False, _
                             Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myPrevTestCalibratorID As Integer = 0
                        Dim myCalibratorDAO As New tparCalibratorsDAO
                        Dim myResultDAO As New twksResultsDAO
                        Dim mytempCalibratorDS As New CalibratorsDS

                        Dim myTestSampleDS As New TestSamplesDS
                        Dim tempTestCalibratorDS As New TestCalibratorsDS
                        Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
                        Dim qTestCalibratorList As New List(Of TestCalibratorsDS.tparTestCalibratorsRow)

                        Dim myTestSampleDelegate As New TestSamplesDelegate
                        Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate
                        Dim tempTestCalibratorValuesDS As New TestCalibratorValuesDS
                        Dim qTestCalibValList As New List(Of TestCalibratorValuesDS.tparTestCalibratorValuesRow)

                        Dim initialCalibratorID As Integer = -1 'TR 09/11/2010 -New variable to set the previous calibrator ID value.

                        Dim myHisCalibratorsDS As New HisCalibratorsDS 'TR 25/09/2012 -Used to update history info.
                        Dim myHisCalibratorsRow As HisCalibratorsDS.thisCalibratorsRow

                        'Create or update the Calibrator
                        Dim testVersionsClosed As Boolean = False
                        For Each calibRow As CalibratorsDS.tparCalibratorsRow In pCalibratorsDS.tparCalibrators.Rows
                            'TR 09/11/2010 -Set the calibrator ID to the local variable.
                            initialCalibratorID = calibRow.CalibratorID

                            mytempCalibratorDS.tparCalibrators.Clear()
                            mytempCalibratorDS.tparCalibrators.ImportRow(calibRow)

                            'Validate if it's new calibrator to create, else update.
                            If (calibRow.IsNew) Then
                                'Create a new Calibrator
                                myGlobalDataTO = Create(dbConnection, mytempCalibratorDS)
                                If (myGlobalDataTO.HasError) Then Exit For
                            Else
                                'Update an existing Calibrator
                                myGlobalDataTO = Update(dbConnection, mytempCalibratorDS)
                                If (myGlobalDataTO.HasError) Then Exit For

                                'TR 25/09/2012 -Validate if history working mode is enabled.
                                If (GlobalConstants.HISTWorkingMode) Then
                                    'TR 25/09/2012 -Update the history Info. after update.

                                    myHisCalibratorsRow = myHisCalibratorsDS.thisCalibrators.NewthisCalibratorsRow
                                    myHisCalibratorsRow.CalibratorID = calibRow.CalibratorID
                                    myHisCalibratorsRow.CalibratorName = calibRow.CalibratorName
                                    myHisCalibratorsRow.LotNumber = calibRow.LotNumber
                                    myHisCalibratorsRow.NumberOfCalibrators = calibRow.NumberOfCalibrators
                                    myHisCalibratorsDS.thisCalibrators.AddthisCalibratorsRow(myHisCalibratorsRow)

                                    myGlobalDataTO = HIST_UpdateByCalibratorID(dbConnection, myHisCalibratorsDS)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        testVersionsClosed = CType(myGlobalDataTO.SetDatos, Boolean)
                                    Else
                                        'Error updating Calibrator data in Historic Module
                                        Exit For
                                    End If
                                End If
                                'TR 25/09/2012 -END.
                            End If

                            'TR 09/06/2010 -TestCalibrator Table.
                            calibRow.IsNew = False 'TR 02/08/2010 -Set value to false after creation

                            'TR 09/11/2010 -Set the calibrator id
                            If (mytempCalibratorDS.tparCalibrators.Rows.Count > 0) Then
                                calibRow.CalibratorID = mytempCalibratorDS.tparCalibrators(0).CalibratorID
                            End If
                            'TR 09/11/2010 -End

                            'TR 09/11/2010 Implemented the use of initial calibratorID
                            qTestCalibratorList = (From a In pTestCalibratorsDS.tparTestCalibrators _
                                                   Where a.CalibratorID = initialCalibratorID _
                                                   Select a).ToList()

                            For Each TestCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In qTestCalibratorList
                                'Set the CalibratorID (CREATE OR UPDATE)
                                TestCalibRow.CalibratorID = mytempCalibratorDS.tparCalibrators(0).CalibratorID

                                'Clear for new test calibrator
                                tempTestCalibratorDS.tparTestCalibrators.Clear()
                                tempTestCalibratorDS.tparTestCalibrators.ImportRow(TestCalibRow)

                                If TestCalibRow.IsNew Then
                                    'Create
                                    myGlobalDataTO = myTestCalibratorDelegate.Create(dbConnection, tempTestCalibratorDS)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Else
                                    'Update
                                    myGlobalDataTO = myTestCalibratorDelegate.Update(dbConnection, tempTestCalibratorDS)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                End If

                                'CHANGES IN TEST/SAMPLE DEFINITION 
                                TestCalibRow.IsNew = False 'TR 02/08/2010 -Set value to false after creation

                                'TR 16/12/2010 -Set the test calibrator id value 
                                myPrevTestCalibratorID = TestCalibRow.TestCalibratorID
                                TestCalibRow.TestCalibratorID = tempTestCalibratorDS.tparTestCalibrators(0).TestCalibratorID

                                'Update CalibratorType for the TestID/SampleType 
                                myGlobalDataTO = myTestSampleDelegate.GetDefinition(dbConnection, TestCalibRow.TestID, TestCalibRow.SampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                    If (myTestSampleDS.tparTestSamples.Rows.Count > 0) Then
                                        'AG 12/11/2010
                                        'If myTestSampleDS.tparTestSamples(0).CalibratorType <> "EXPERIMENT" Then
                                        If (Not pSaveFromTestProgramming AndAlso String.Compare(myTestSampleDS.tparTestSamples(0).CalibratorType, "EXPERIMENT", False) <> 0) Then
                                            'Change value of field CalibratorType in the DS, and set value of CalibratorFactor to NULL
                                            myTestSampleDS.tparTestSamples(0).BeginEdit()
                                            myTestSampleDS.tparTestSamples(0).CalibratorType = "EXPERIMENT"
                                            myTestSampleDS.tparTestSamples(0).SetCalibrationFactorNull()
                                            ''TR 17/02/2011
                                            'myTestSampleDS.tparTestSamples(0).EnableStatus = DeleteTestCalibratorValues

                                            myTestSampleDS.tparTestSamples(0).EndEdit()
                                            myTestSampleDS.AcceptChanges()

                                            'Update fields CalibratorType and CalibratorFactor for the TestID/SampleType 
                                            myGlobalDataTO = myTestSampleDelegate.Update(dbConnection, myTestSampleDS)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    End If
                                Else
                                    'Error getting the Calibration fields for the Test/SampleType
                                    Exit For
                                End If

                                'CHANGES IN TEST CALIBRATOR CONCENTRATION VALUES
                                qTestCalibValList = (From a In pTestCalibratorValueDS.tparTestCalibratorValues _
                                                    Where a.TestCalibratorID = myPrevTestCalibratorID _
                                                   Select a).ToList()

                                For Each testCalibValue As TestCalibratorValuesDS.tparTestCalibratorValuesRow In qTestCalibValList
                                    tempTestCalibratorValuesDS.tparTestCalibratorValues.Clear()

                                    'Set the Process TestCalibratorID (NEW or UPDATE)
                                    testCalibValue.TestCalibratorID = tempTestCalibratorDS.tparTestCalibrators(0).TestCalibratorID
                                    tempTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(testCalibValue)

                                    'Validate if new 
                                    If (testCalibValue.IsNew) Then
                                        'New link between TestID/SampleType and a Calibrator: add TestCalibrator Values
                                        myGlobalDataTO = myTestCalibratorValuesDelegate.Create(dbConnection, tempTestCalibratorValuesDS)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Else
                                        'Validate if there are Concentration values defined for the TestID/SampleType and Calibrator  
                                        myGlobalDataTO = myTestCalibratorValuesDelegate.GetTestCalibratorValuesByTestCalibratorID(dbConnection, _
                                                                                                                                  tempTestCalibratorValuesDS.tparTestCalibratorValues(0).TestCalibratorID)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            'Validate if the Concentration for the Calibrator point is defined
                                            Dim qTestCalValueList As New List(Of TestCalibratorValuesDS.tparTestCalibratorValuesRow)
                                            qTestCalValueList = (From a In DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS).tparTestCalibratorValues _
                                                                Where a.CalibratorNum = tempTestCalibratorValuesDS.tparTestCalibratorValues(0).CalibratorNum).ToList()
                                            If (qTestCalValueList.Count > 0) Then
                                                'Update the value
                                                myGlobalDataTO = myTestCalibratorValuesDelegate.Update(dbConnection, tempTestCalibratorValuesDS)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            Else
                                                'Add the new Concentration 
                                                myGlobalDataTO = myTestCalibratorValuesDelegate.Create(dbConnection, tempTestCalibratorValuesDS)
                                                If (myGlobalDataTO.HasError) Then Exit For
                                            End If
                                        Else
                                            'Error getting the Concentration values defined for the TestID/SampleType and Calibrator  
                                            Exit For
                                        End If


                                        If (pDeleteCalibrationResult) Then

                                            'TR 29/07/2013 -Delete Curve Results.
                                            'myGlobalDataTO = DeleteCurveResults(dbConnection, TestCalibRow.TestID, TestCalibRow.SampleType, TestCalibRow.TestVersionNumber)
                                            'TR 29/07/2013 -END.


                                            'If there are previous Calibrator results for the TestID/SampleType, then they are deleted
                                            myGlobalDataTO = myResultDAO.DeleteCalibrationResultsByTestIDSampleType(dbConnection, tempTestCalibratorDS.tparTestCalibrators(0).TestID, _
                                                                                                                    tempTestCalibratorDS.tparTestCalibrators(0).TestVersionNumber, _
                                                                                                                    tempTestCalibratorDS.tparTestCalibrators(0).SampleType)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If
                                    End If

                                    testCalibValue.IsNew = False
                                Next


                                'AG 09/10/2012 - Close Test Version in Historic Module 
                                If (Not myGlobalDataTO.HasError AndAlso GlobalConstants.HISTWorkingMode AndAlso Not testVersionsClosed) Then
                                    myGlobalDataTO = myTestSampleDelegate.HIST_CloseTestVersion(dbConnection, TestCalibRow.TestID, "CALIB", TestCalibRow.SampleType)
                                    If (myGlobalDataTO.HasError) Then Exit For
                                End If
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                'TR 14/12/2010 -Validate if wants to delete the TestCalibrators Values
                                If (pDeleteTestCalibratorValues) Then
                                    'Get all TestIDs/SampleTypes linked to the Calibrator
                                    myGlobalDataTO = myTestCalibratorDelegate.GetAllTestCalibratorByCalibratorID(dbConnection, calibRow.CalibratorID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myTestCalibratorsDS As TestSampleCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)

                                        For Each testCalibRow As TestSampleCalibratorDS.tparTestCalibratorsRow In myTestCalibratorsDS.tparTestCalibrators.Rows

                                            'TR 29/07/2013 -Delete Curve Results.
                                            'myGlobalDataTO = DeleteCurveResults(dbConnection, testCalibRow.TestID, testCalibRow.SampleType, testCalibRow.TestVersionNumber)
                                            'TR 29/07/2013 -END.

                                            If (myGlobalDataTO.HasError) Then Exit For

                                            myGlobalDataTO = myResultDAO.DeleteCalibrationResultsByTestIDSampleType(dbConnection, testCalibRow.TestID, testCalibRow.TestVersionNumber, _
                                                                                                                    testCalibRow.SampleType)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'Delete TestCalibratorsValues
                                            myGlobalDataTO = myTestCalibratorValuesDelegate.DeleteByTestCalibratorID(dbConnection, testCalibRow.TestCalibratorID)
                                            If (myGlobalDataTO.HasError) Then Exit For

                                            'Set the TestSample Enable Status To False.
                                            myGlobalDataTO = myTestSampleDelegate.UpdateTestSampleEnableStatus(dbConnection, testCalibRow.TestID, testCalibRow.SampleType, False)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        Next
                                    Else
                                        'Error getting all Tests/SampleTypes linked to the Calibrator
                                        Exit For
                                    End If
                                    If (myGlobalDataTO.HasError) Then Exit For
                                End If
                                'TR 14/12/2010 -END.
                            End If
                        Next

                        'Delete calibrator
                        If (Not pDeleteCalibratorList Is Nothing AndAlso pDeleteCalibratorList.Count > 0) Then
                            'Go through each Calibrator selected to be deleted
                            For Each myDelCalibTO As DeletedCalibratorTO In pDeleteCalibratorList
                                'Remove the Calibrator (by CalibratorID) and all its relations (TestCalibrators and TestCalibratorValues)
                                If (myDelCalibTO.CalibratorID <> 0) Then
                                    myGlobalDataTO = Delete(dbConnection, myDelCalibTO.CalibratorID, pAnalyzerID, pWorkSessionID)
                                    If (myGlobalDataTO.HasError) Then Exit For

                                ElseIf (myDelCalibTO.TestID <> 0 AndAlso String.Compare(myDelCalibTO.SampleType, "", False) <> 0) Then
                                    'Delete only TestCalibrator and TestCalibratorValues for the specified Test/SampleType and Calibrator
                                    myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(dbConnection, myDelCalibTO.TestID, myDelCalibTO.SampleType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        Dim myTestCalibratorDS As TestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                                        For Each testCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In myTestCalibratorDS.tparTestCalibrators.Rows
                                            myGlobalDataTO = myTestCalibratorDelegate.DeleteTestCaliByTestID(dbConnection, testCalibRow.TestID, testCalibRow.SampleType)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        Next
                                    Else
                                        'Error getting all Tests/SampleTypes linked to the Calibrator
                                    End If
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.Save", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the curve results on table twksCurveResults when required.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test id.</param>
        ''' <param name="pSampleType">Sample Type.</param>
        ''' <param name="pTestVersionNumber">Test Version.</param>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 29/07/2013</remarks>
        Private Function DeleteCurveResults(ByVal pDBConnection As SqlClient.SqlConnection, pTestID As Integer, pSampleType As String, pTestVersionNumber As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim lastResults As New ResultsDS
                        Dim curveIDToReturn As Integer = -1
                        Dim myResultDAO As New twksResultsDAO
                        Dim myCurveDAO As New twksCurveResultsDAO
                        Dim AdditionalElements As New WSAdditionalElementsDS

                        curveIDToReturn = -1
                        AdditionalElements.WSAdditionalElementsTable.Clear()
                        lastResults.twksResults.Clear()
                        myGlobalDataTO = myResultDAO.GetLastExecutedCalibrator(dbConnection, pTestID, pSampleType, _
                                                                               pTestVersionNumber)
                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                            AdditionalElements = CType(myGlobalDataTO.SetDatos, WSAdditionalElementsDS)
                        End If
                        'Search the curve result ID
                        If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then
                            Dim foundOrderTestId As Integer = AdditionalElements.WSAdditionalElementsTable(0).PreviousOrderTestID

                            'Get blank results
                            myGlobalDataTO = myResultDAO.GetAcceptedResults(dbConnection, foundOrderTestId)
                            If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then

                                lastResults = CType(myGlobalDataTO.SetDatos, ResultsDS)

                                If lastResults.twksResults.Rows.Count > 0 Then
                                    If Not lastResults.twksResults(0).IsCurveResultsIDNull Then curveIDToReturn = lastResults.twksResults(0).CurveResultsID
                                End If
                                If curveIDToReturn > 0 Then
                                    myGlobalDataTO = myCurveDAO.DeleteCurve(dbConnection, curveIDToReturn)
                                End If
                            End If
                        End If
                        'TR 29/07/2013 
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.DeleteCurveReuslts", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update values of the specified Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing data to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 07/06/2010
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCalibratorDAO As New tparCalibratorsDAO
                        Dim mytempCalibratorDS As New CalibratorsDS

                        For Each calibRow As CalibratorsDS.tparCalibratorsRow In pCalibratorsDS.tparCalibrators.Rows
                            mytempCalibratorDS.tparCalibrators.Clear()
                            mytempCalibratorDS.tparCalibrators.ImportRow(calibRow)

                            myGlobalDataTO = myCalibratorDAO.Update(dbConnection, mytempCalibratorDS)
                            If (myGlobalDataTO.HasError) Then Exit For
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Calibrators added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Calibrators that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Calibrators 
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                        ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparCalibratorsDAO
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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' A Calibrator is closed in Historics Module (field ClosedCalibrator is set to TRUE) in following cases:
        '''   ** When the LotNumber and/or the Number of Calibrators is changed in Calibrators Programming Screen
        '''   ** When the Calibrator is deleted in Calibrators Programming screen
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' Modified by: SA 17/09/2012 - Method has been moved from class HisCalibratorsDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisCalibrators are called
        ''' </remarks>
        Private Function HIST_CloseCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the Calibrator already exists in Historics Module
                        Dim myDAO As New thisCalibratorsDAO

                        resultData = myDAO.ReadByCalibratorID(dbConnection, pCalibratorID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myHisCalibsDS As HisCalibratorsDS = DirectCast(resultData.SetDatos, HisCalibratorsDS)
                            If (myHisCalibsDS.thisCalibrators.Rows.Count > 0) Then
                                'Mark Calibrator as closed
                                resultData = myDAO.CloseCalibrator(dbConnection, myHisCalibsDS.thisCalibrators.First.HistCalibratorID)

                                If (Not resultData.HasError) Then
                                    'Close the active TestVersion of all Test/SampleTypes linked to the Calibrator
                                    Dim myTestSamplesDelegate As New TestSamplesDelegate
                                    resultData = myTestSamplesDelegate.HIST_CloseTestVersionByHistCalibratorID(dbConnection, myHisCalibsDS.thisCalibrators.First.HistCalibratorID)
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
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.HIST_CloseCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When basic data of a Calibrator (Name) is changed in Calibrators Programming Screen, values are updated for the corresponding 
        ''' record in the Historics Module table (the one having the same CalibratorID and ClosedCalibrator = False)
        ''' When besides basic data, the LotNumber or the NumberOfCalibrators is changed, then the Calibrator is marked as closed and the 
        ''' basic data is not updated.  The current version of all  Test/Samples using the Calibrator are also marked as closed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalibratorsDS">Typed DataSet HisCalibratorsDS containing Calibrator data updated in Parameters Programming
        '''                                 Module</param>
        ''' <returns>GlobalDataTO containing a boolean value: when TRUE, it indicates the TestVersion of all linked Tests/SampleTypes has been closed;
        '''          when FALSE, the Calibrator data has been updated but linked Tests/SampleTypes remain open</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' Modified by: SA 17/09/2012 - Method has been moved from class HisCalibratorsDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisCalibrators are called
        '''              SA 22/10/2012 - Changed the value returned by function: when only basic Calibrator data has been updated, returns FALSE, but
        '''                              when the Test Version of all linked Tests/SampleTypes has been closed, returns TRUE
        ''' </remarks>
        Private Function HIST_UpdateByCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalibratorsDS As HisCalibratorsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if the Calibrator exists in Historics Module
                        Dim myDAO As New thisCalibratorsDAO
                        Dim testVersionClosed As Boolean = False

                        resultData = myDAO.ReadByCalibratorID(dbConnection, pHisCalibratorsDS.thisCalibrators.First.CalibratorID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisCalibratorsDS = DirectCast(resultData.SetDatos, HisCalibratorsDS)

                            If (auxiliaryDS.thisCalibrators.Rows.Count > 0) Then
                                'Update value of field HistCalibratorID in the entry DS
                                pHisCalibratorsDS.thisCalibrators.First.HistCalibratorID = auxiliaryDS.thisCalibrators.First.HistCalibratorID

                                'Verify if fields LotNumber or NumberOfCalibrators have been changed
                                If (pHisCalibratorsDS.thisCalibrators.First.LotNumber = auxiliaryDS.thisCalibrators.First.LotNumber AndAlso _
                                    pHisCalibratorsDS.thisCalibrators.First.NumberOfCalibrators = auxiliaryDS.thisCalibrators.First.NumberOfCalibrators AndAlso _
                                    pHisCalibratorsDS.thisCalibrators.First.CalibratorName <> auxiliaryDS.thisCalibrators.First.CalibratorName) Then
                                    'Update basic Calibrator data 
                                    resultData = myDAO.Update(dbConnection, pHisCalibratorsDS)

                                ElseIf (pHisCalibratorsDS.thisCalibrators.First.LotNumber <> auxiliaryDS.thisCalibrators.First.LotNumber OrElse _
                                    pHisCalibratorsDS.thisCalibrators.First.NumberOfCalibrators <> auxiliaryDS.thisCalibrators.First.NumberOfCalibrators) Then
                                    'Mark Calibrator as closed
                                    resultData = myDAO.CloseCalibrator(dbConnection, auxiliaryDS.thisCalibrators.First.HistCalibratorID)

                                    If (Not resultData.HasError) Then
                                        'Close the active TestVersion of all Test/SampleType linked to the Calibrator
                                        testVersionClosed = True

                                        Dim myTestSamplesDelegate As New TestSamplesDelegate
                                        resultData = myTestSamplesDelegate.HIST_CloseTestVersionByHistCalibratorID(dbConnection, pHisCalibratorsDS.thisCalibrators.First.HistCalibratorID)
                                    End If
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = testVersionClosed
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
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorsDelegate.HIST_UpdateByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace


