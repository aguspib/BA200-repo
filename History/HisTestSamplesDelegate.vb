Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.BL
    Public Class HisTestSamplesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Receive a list of TestIDs/Sample Types and for each one of them, verify if an open Version already exists in Historics Module for it, 
        ''' and when it does not exist, get the needed data and create the new version of the Test Sample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisTestSamplesDS">Typed Dataset HisTestSamplesDS containing the group of Test/SampleTypes to verify if they already exist 
        '''                                 in Historics Module and create them when not</param>
        ''' <param name="pHisCalibratorsDS">Typed Dataset HisCalibratorsDS containing data of all needed Calibrators for the group of Standard Test/SampleTypes</param>
        ''' <param name="pHisReagentsDS">Typed Dataset HisReagentsDS containing data of all needed Reagents for the group of Standard Test/SampleType</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the group of results to export</param>
        ''' <param name="pAnalyzerLedPosDS">Typed DataSet AnalyzerLedPositionsDS containing the relation between WaveLength and LedPosition for all
        '''                                 filters used in the Analyzer</param>
        ''' <returns>GlobalDataTO containing a typed DataSet </returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 27/09/2012 - Added verification of Special Tests: search if setting CAL_POINT_USED is informed for the Test/SampleType and in
        '''                              this case inform value of field CalibPointUsed in table thisTestSamples.
        '''                            - Added saving of Calibration values for all Test/SampleTypes using an Experimental Calibrator
        '''              SA 28/09/2012 - Added parameter pAnalyzerLedPositionsDS containing the relation between WaveLength and LedPosition for all
        '''                              filters used in the Analyzer
        '''              SA 18/10/2012 - Changes to get and inform fields KineticBlankLimit, BlankAbsorbanceLimit, FactorLowerLimit and FactorUpperLimit;
        '''                              although these values are not saved with the Test/SampleType in Historic Module, they will be saved later with the
        '''                              Historic Results. Changes to get also the Theoretical Calibration Values when the Test/SampleType uses an Experimental
        '''                              Calibrator of an Alternative SampleType
        '''              SA 25/10/2012 - When there is an active TestVersion in Historic Module, verify if it can be used or if it has to be closed (when
        '''                              values of the Calibration Curve have been changed from the screen of WorkSession Calibration Curve Results) 
        '''                              and a new one created
        ''' </remarks>
        Public Function CheckSTDTestSamplesInHistorics(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisTestSamplesDS As HisTestSamplesDS, _
                                                       ByVal pHisCalibratorsDS As HisCalibratorsDS, ByVal pHisReagentsDS As HisReagentsDS, _
                                                       ByVal pResultsDS As ResultsDS, ByVal pAnalyzerLedPosDS As AnalyzerLedPositionsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim newTest As Boolean
                        Dim mySampleType As String

                        Dim myTestsDS As New TestsDS
                        Dim myTestDelegate As New TestsDelegate

                        Dim myTestSamplesDS As New TestSamplesDS
                        Dim myTestSamplesDelegate As New TestSamplesDelegate

                        Dim myTestCalibDS As New TestCalibratorsDS
                        Dim myTestCalibDelegate As New TestCalibratorsDelegate

                        Dim myTestReagentsVolsDS As New TestReagentsVolumesDS
                        Dim myTestReagentsVolsDelegate As New TestReagentsVolumeDelegate

                        Dim nextHistID As Integer = 0
                        Dim myDAO As New thisTestSamplesDAO
                        Dim auxiliaryDS As New HisTestSamplesDS
                        Dim testSamplesToAddDS As New HisTestSamplesDS
                        Dim lstCalibResults As List(Of ResultsDS.vwksResultsRow)

                        Dim mySpecialTestsDS As New SpecialTestsSettingsDS
                        Dim mySpecialTestsDelegate As New SpecialTestsSettingsDelegate

                        For Each testSampleRow As HisTestSamplesDS.thisTestSamplesRow In pHisTestSamplesDS.thisTestSamples
                            'Search if there are TestVersions in Historics Module for the Test/SampleType
                            resultData = myDAO.ReadByTestIDAndSampleType(dbConnection, testSampleRow.TestID, testSampleRow.SampleType)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                auxiliaryDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                                newTest = False
                                If (auxiliaryDS.thisTestSamples.Rows.Count = 0) Then
                                    'Get the next HistTestID...
                                    If (nextHistID = 0) Then
                                        resultData = myDAO.GetNextHistTestID(dbConnection)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            nextHistID = CType(resultData.SetDatos, Integer)
                                        Else
                                            'Error getting the next HistTestID
                                            Exit For
                                        End If
                                    Else
                                        nextHistID += 1
                                    End If

                                    testSampleRow.BeginEdit()
                                    testSampleRow.HistTestID = nextHistID
                                    testSampleRow.TestVersionNumber = 0
                                    testSampleRow.TestVersionDateTime = Now
                                    testSampleRow.EndEdit()

                                    newTest = True
                                Else
                                    'Verify if the last Test Version is marked as closed
                                    If (auxiliaryDS.thisTestSamples.First.ClosedTestVersion) Then
                                        testSampleRow.BeginEdit()
                                        testSampleRow.HistTestID = auxiliaryDS.thisTestSamples.First.HistTestID
                                        testSampleRow.TestVersionNumber = auxiliaryDS.thisTestSamples.First.TestVersionNumber + 1
                                        testSampleRow.TestVersionDateTime = Now
                                        testSampleRow.EndEdit()

                                        newTest = True
                                    Else
                                        'There is an active TestVersion in Historic Module...verify if it can be used or if it has to be closed (when
                                        'values of the Calibration Curve have been changed from the screen of WorkSession Calibration Curve Results) 
                                        'and a new one created
                                        mySampleType = IIf(testSampleRow.IsAlternativeSampleTypeNull, testSampleRow.SampleType, testSampleRow.AlternativeSampleType).ToString

                                        'In the group of accepted and validated Results, verify if for the Test/SampleType in process there are Calibrator 
                                        'results with Curve definition values informed (which mean its linked Calibrator is a Multipoint one)
                                        lstCalibResults = (From a As ResultsDS.vwksResultsRow In pResultsDS.vwksResults _
                                                          Where a.SampleClass = "CALIB" _
                                                        AndAlso a.TestID = testSampleRow.TestID _
                                                        AndAlso a.SampleType = mySampleType _
                                                         AndAlso a.MultiPointNumber = 1 _
                                                    AndAlso Not a.IsCurveTypeNull _
                                                         Select a).ToList()

                                        If (lstCalibResults.Count = 1) Then
                                            'Verify if Calibration Curve definition values have changed regarding the last saved in Historic for the Test/SampleType
                                            If (lstCalibResults.First.CurveType <> auxiliaryDS.thisTestSamples.First.CurveType) OrElse _
                                               (lstCalibResults.First.CurveGrowthType <> auxiliaryDS.thisTestSamples.First.CurveGrowthType) OrElse _
                                               (lstCalibResults.First.CurveAxisXType <> auxiliaryDS.thisTestSamples.First.CurveAxisXType) OrElse _
                                               (lstCalibResults.First.CurveAxisYType <> auxiliaryDS.thisTestSamples.First.CurveAxisYType) Then

                                                'If definition values of the Calibration Curve have been changed, the current Test Version has to be closed...
                                                resultData = myDAO.CloseTestVersion(dbConnection, auxiliaryDS.thisTestSamples.First.HistTestID, testSampleRow.SampleType)
                                                If (resultData.HasError) Then Exit For

                                                '... and a new one has to be created
                                                testSampleRow.BeginEdit()
                                                testSampleRow.HistTestID = auxiliaryDS.thisTestSamples.First.HistTestID
                                                testSampleRow.TestVersionNumber = auxiliaryDS.thisTestSamples.First.TestVersionNumber + 1
                                                testSampleRow.TestVersionDateTime = Now
                                                testSampleRow.EndEdit()

                                                newTest = True
                                            End If
                                        End If

                                        If (Not newTest) Then
                                            'There is an active TestVersion in Historic Module; update all data in the entry DS
                                            FillTestSamplesDSFromHistorics(testSampleRow, auxiliaryDS.thisTestSamples.First, pAnalyzerLedPosDS)

                                            'Search in Parameters Programming current value of fields KineticsBlankLimit, BlankAbsorbanceLimit, 
                                            'FactorLowerLimit and FactorUpperLimit to inform them in the DS
                                            resultData = myTestSamplesDelegate.GetLimitsForHISTResults(dbConnection, testSampleRow.TestID, testSampleRow.SampleType)
                                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                myTestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                                                testSampleRow.SetKineticBlankLimitNull()
                                                testSampleRow.SetBlankAbsorbanceLimitNull()
                                                testSampleRow.SetFactorLowerLimitNull()
                                                testSampleRow.SetFactorUpperLimitNull()

                                                If (myTestSamplesDS.tparTestSamples.Rows.Count = 1) Then
                                                    If (Not myTestSamplesDS.tparTestSamples.First.IsKineticsBlankLimitNull) Then testSampleRow.KineticBlankLimit = myTestSamplesDS.tparTestSamples.First.KineticsBlankLimit
                                                    If (Not myTestSamplesDS.tparTestSamples.First.IsBlankAbsorbanceLimitNull) Then testSampleRow.BlankAbsorbanceLimit = myTestSamplesDS.tparTestSamples.First.BlankAbsorbanceLimit
                                                    If (Not myTestSamplesDS.tparTestSamples.First.IsFactorLowerLimitNull) Then testSampleRow.FactorLowerLimit = myTestSamplesDS.tparTestSamples.First.FactorLowerLimit
                                                    If (Not myTestSamplesDS.tparTestSamples.First.IsFactorUpperLimitNull) Then testSampleRow.FactorUpperLimit = myTestSamplesDS.tparTestSamples.First.FactorUpperLimit
                                                End If
                                            Else
                                                'Error getting limits defined for the Test/SampleType in Parameters Programming Module
                                                Exit For
                                            End If
                                        End If
                                    End If
                                End If

                                If (newTest) Then
                                    'Get data of the Test in Parameters Programming Module and inform fields in the entry DS
                                    resultData = myTestDelegate.Read(dbConnection, testSampleRow.TestID)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myTestsDS = DirectCast(resultData.SetDatos, TestsDS)

                                        FillTestFields(testSampleRow, myTestsDS, pAnalyzerLedPosDS)
                                    Else
                                        'Error getting the Test data
                                        Exit For
                                    End If

                                    'Get data of the Test/SampleType in Parameters Programming Module and inform fields in the entry DS
                                    resultData = myTestSamplesDelegate.GetDefinition(dbConnection, testSampleRow.TestID, testSampleRow.SampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myTestSamplesDS = DirectCast(resultData.SetDatos, TestSamplesDS)

                                        FillTestSampleFields(testSampleRow, myTestSamplesDS)

                                        'Get the Calibrator ID in Historic Module and inform field HistCalibratorID for the Test/SampleType
                                        'Remember value of CalibratorID in Parameters Programming was saved provitionally in the DS field for the ID in Historic Module
                                        If (testSampleRow.CalibratorType = "EXPERIMENT") Then
                                            testSampleRow.BeginEdit()
                                            testSampleRow.HistCalibratorID = pHisCalibratorsDS.thisCalibrators.ToList.Where(Function(a) a.CalibratorID = testSampleRow.HistCalibratorID).First().HistCalibratorID
                                            testSampleRow.EndEdit()
                                        End If

                                        'Search data of all Reagents used for the Test and fill all Reagents fields in the entry DS
                                        resultData = myTestReagentsVolsDelegate.GetReagentsVolumesByTestID(dbConnection, testSampleRow.TestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myTestReagentsVolsDS = DirectCast(resultData.SetDatos, TestReagentsVolumesDS)

                                            FillReagentsFields(testSampleRow, myTestReagentsVolsDS, pHisReagentsDS)
                                        Else
                                            'Error getting data of all Reagents used for the Test
                                            Exit For
                                        End If

                                        'Move the TestSampleRow to the DS containing all new TestSamples to add
                                        testSamplesToAddDS.thisTestSamples.ImportRow(testSampleRow)
                                    Else
                                        'Error getting the Test/SampleType data
                                        Exit For
                                    End If
                                End If
                            Else
                                'Error verifying if the Test/SampleType exists in Historics Module
                                Exit For
                            End If
                        Next

                        If (Not resultData.HasError AndAlso testSamplesToAddDS.thisTestSamples.Rows.Count > 0) Then
                            'Create all new Test/Sample Types in Historics Module
                            resultData = myDAO.Create(dbConnection, testSamplesToAddDS)

                            If (Not resultData.HasError) Then
                                'Get all added Test/Sample Types having CalibratorType = EXPERIMENTAL
                                Dim lstExpCalibration As List(Of HisTestSamplesDS.thisTestSamplesRow) = (From a As HisTestSamplesDS.thisTestSamplesRow In testSamplesToAddDS.thisTestSamples _
                                                                                                        Where a.CalibratorType = "EXPERIMENT" _
                                                                                                       Select a).ToList()

                                Dim myTestCalibValuesDS As New TestCalibratorValuesDS
                                Dim myTestCalibValuesDelegate As New TestCalibratorValuesDelegate
                                Dim myHisTestCalibValuesDelegate As New HisTestCalibratorsValuesDelegate

                                For Each row As HisTestSamplesDS.thisTestSamplesRow In lstExpCalibration
                                    mySampleType = row.SampleType
                                    If (Not row.IsAlternativeSampleTypeNull) Then mySampleType = row.AlternativeSampleType

                                    'Get Calibrator values for all points of the Experimental Calibrator used for the Test/Sample Type
                                    resultData = myTestCalibValuesDelegate.GetTestCalibratorValuesByTestIDSampleType(dbConnection, row.TestID, mySampleType)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myTestCalibValuesDS = DirectCast(resultData.SetDatos, TestCalibratorValuesDS)

                                        'Save the Calibrator values in Historic Module
                                        resultData = myHisTestCalibValuesDelegate.Create(dbConnection, row.HistTestID, row.SampleType, row.TestVersionNumber, _
                                                                                         row.HistCalibratorID, myTestCalibValuesDS)
                                        If (resultData.HasError) Then Exit For
                                    Else
                                        'Error getting Calibrator values for all points of the Experimental Calibrator used for the Test/Sample Type
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = pHisTestSamplesDS
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestSamplesDelegate.CheckSTDTestSamplesInHistorics", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all versions saved in Historics Module for the specified TestID/SampleType sorted by TestVersionNumber descending
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier in Parameters Programming Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pNotClosedTestVersion">When TRUE, it indicates that only the not closed TestVersion (if any) will be returned
        '''                                     When FALSE, all TestVersions that exist for the not closed Test/SampleType are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with data of all versions saved in Historics Module for the 
        '''          specified TestID and SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 25/06/2012
        ''' </remarks>
        Public Function GetByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
                                                 Optional ByVal pNotClosedTestVersion As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisTestSampleDAO As New thisTestSamplesDAO
                        resultData = myHisTestSampleDAO.ReadByTestIDAndSampleType(dbConnection, pTestID, pSampleType, pNotClosedTestVersion)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestSamplesDelegate.GetByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get closed all STD Tests / Sample Types saved in Historic Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with all closed STD Tests / SampleTypes in
        '''          Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function GetClosedSTDTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisTestSampleDAO As New thisTestSamplesDAO
                        resultData = myHisTestSampleDAO.GetClosedSTDTests(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestSamplesDelegate.GetClosedSTDTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified TestID / SampleType / TestVersionNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">STD Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestVersionNum">Test Version Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 02/07/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                           ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisTestSamplesDAO
                        resultData = myDAO.Delete(dbConnection, pHistTestID, pSampleType, pTestVersionNum)

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
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestSamplesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Fill Reagents fields for an specific Test/SampleType with the corresponding data in Parameters Programming
        ''' </summary>
        ''' <param name="pRowToUpdate">Row of typed DataSet HistTestSamplesDS that will be updated</param>
        ''' <param name="pTestReagentsVolsDS">Typed DataSet TestReagentsVolumesDS containing programmed volumes for the Reagents used for an 
        '''                                   specific Test/SampleType</param>
        ''' <param name="pHisReagentsDS">Typed DataSet HisReagentsDS containing data in Historics Module of Reagents needed in the WS</param>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 21/09/2012 - Removed fields for Reagent Volume expressed in bomb steps
        ''' </remarks>
        Private Sub FillReagentsFields(ByRef pRowToUpdate As HisTestSamplesDS.thisTestSamplesRow, ByVal pTestReagentsVolsDS As TestReagentsVolumesDS, _
                                       ByVal pHisReagentsDS As HisReagentsDS)
            Try
                pRowToUpdate.BeginEdit()
                For Each reagentRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In pTestReagentsVolsDS.tparTestReagentsVolumes
                    If (reagentRow.ReagentNumber = 1) Then
                        'Search the ID of the first Reagent in Historics Module and update the rest of first Reagent fields
                        pRowToUpdate.HistReagent1ID = pHisReagentsDS.thisReagents.ToList.Where(Function(a) a.ReagentID = reagentRow.ReagentID).First().HistReagentID
                        pRowToUpdate.Reagent1Volume = reagentRow.ReagentVolume
                        If (Not reagentRow.IsRedPostReagentVolumeNull) Then pRowToUpdate.RedPostReagent1Volume = reagentRow.RedPostReagentVolume
                        If (Not reagentRow.IsIncPostReagentVolumeNull) Then pRowToUpdate.IncPostReagent1Volume = reagentRow.IncPostReagentVolume
                    Else
                        'Search the ID of the second Reagent in Historics Module and update the rest of second Reagent fields
                        pRowToUpdate.HistReagent2ID = pHisReagentsDS.thisReagents.ToList.Where(Function(a) a.ReagentID = reagentRow.ReagentID).First().HistReagentID
                        pRowToUpdate.Reagent2Volume = reagentRow.ReagentVolume
                        If (Not reagentRow.IsRedPostReagentVolumeNull) Then pRowToUpdate.RedPostReagent2Volume = reagentRow.RedPostReagentVolume
                        If (Not reagentRow.IsIncPostReagentVolumeNull) Then pRowToUpdate.IncPostReagent2Volume = reagentRow.IncPostReagentVolume
                    End If
                Next
                pRowToUpdate.EndEdit()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Fill Test fields with the corresponding data in Parameters Programming
        ''' </summary>
        ''' <param name="pRowToUpdate">Row of typed DataSet HistTestSamplesDS that will be updated</param>
        ''' <param name="pTestsDS">Typed DataSet TestsDS containing programmed data of the Test</param>
        ''' <param name="pAnalyzerLedPosDS">Typed DataSet AnalyzerLedPositionsDS containing the relation between WaveLength and LedPosition for all
        '''                                 filters used in the Analyzer</param>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 28/09/2012 - Added parameter pAnalyzerLedPositionsDS containing the relation between WaveLength and LedPosition for all
        '''                              filters used in the Analyzer.  Content of this DS is used to inform fields MainLedPosition and ReferenceLedPosition
        ''' </remarks>
        Private Sub FillTestFields(ByRef pRowToUpdate As HisTestSamplesDS.thisTestSamplesRow, ByVal pTestsDS As TestsDS, ByVal pAnalyzerLedPosDS As AnalyzerLedPositionsDS)
            Try
                If (pTestsDS.tparTests.Rows.Count > 0) Then
                    pRowToUpdate.BeginEdit()
                    pRowToUpdate.TestName = pTestsDS.tparTests.First.TestName
                    pRowToUpdate.PreloadedTest = pTestsDS.tparTests.First.PreloadedTest
                    pRowToUpdate.MeasureUnit = pTestsDS.tparTests.First.MeasureUnit
                    pRowToUpdate.AnalysisMode = pTestsDS.tparTests.First.AnalysisMode
                    pRowToUpdate.ReactionType = pTestsDS.tparTests.First.ReactionType
                    pRowToUpdate.DecimalsAllowed = pTestsDS.tparTests.First.DecimalsAllowed
                    pRowToUpdate.ReadingMode = pTestsDS.tparTests.First.ReadingMode
                    pRowToUpdate.FirstReadingCycle = pTestsDS.tparTests.First.FirstReadingCycle
                    If (Not pTestsDS.tparTests.First.IsSecondReadingCycleNull) Then pRowToUpdate.SecondReadingCycle = pTestsDS.tparTests.First.SecondReadingCycle

                    pRowToUpdate.MainWavelength = pTestsDS.tparTests.First.MainWavelength
                    pRowToUpdate.MainLedPosition = pAnalyzerLedPosDS.tcfgAnalyzerLedPositions.ToList.Where(Function(a) a.WaveLength = pTestsDS.tparTests.First.MainWavelength).First.LedPosition

                    If (Not pTestsDS.tparTests.First.IsReferenceWavelengthNull) Then
                        pRowToUpdate.ReferenceWavelength = pTestsDS.tparTests.First.ReferenceWavelength
                        pRowToUpdate.ReferenceLedPosition = pAnalyzerLedPosDS.tcfgAnalyzerLedPositions.ToList.Where(Function(a) a.WaveLength = pTestsDS.tparTests.First.ReferenceWavelength).First.LedPosition
                    End If

                    pRowToUpdate.BlankMode = pTestsDS.tparTests.First.BlankMode
                    If (Not pTestsDS.tparTests.First.IsKineticBlankLimitNull) Then pRowToUpdate.KineticBlankLimit = pTestsDS.tparTests.First.KineticBlankLimit
                    If (Not pTestsDS.tparTests.First.IsProzoneRatioNull) Then pRowToUpdate.ProzoneRatio = pTestsDS.tparTests.First.ProzoneRatio
                    If (Not pTestsDS.tparTests.First.IsProzoneTime1Null) Then pRowToUpdate.ProzoneTime1 = pTestsDS.tparTests.First.ProzoneTime1
                    If (Not pTestsDS.tparTests.First.IsProzoneTime2Null) Then pRowToUpdate.ProzoneTime2 = pTestsDS.tparTests.First.ProzoneTime2
                    pRowToUpdate.EndEdit()
                Else
                    'This case is not possible...
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Fill Test/SampleType fields with the corresponding data in Parameters Programming
        ''' </summary>
        ''' <param name="pRowToUpdate">Row of typed DataSet HistTestSamplesDS that will be updated</param>
        ''' <param name="pTestSamplesDS">Typed DataSet TestSamplesDS containing programmed data of the Test/SampleType</param>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 21/09/2012 - Removed fields for Reagent Volume expressed in bomb steps and also field AbsorbanceDilutionFactor
        '''              SA 18/10/2012 - Added code to fill also fiels FactorLowerLimit and FactorUpperLimit when they are informed for the Test/SampleType
        '''                              These fields are not saved in table thisTestSamples, but they are needed later to be saved in thisWSResults
        ''' </remarks>
        Private Sub FillTestSampleFields(ByRef pRowToUpdate As HisTestSamplesDS.thisTestSamplesRow, ByVal pTestSamplesDS As TestSamplesDS)
            Try
                If (pTestSamplesDS.tparTestSamples.Rows.Count > 0) Then
                    pRowToUpdate.BeginEdit()
                    pRowToUpdate.SampleVolume = pTestSamplesDS.tparTestSamples.First.SampleVolume

                    pRowToUpdate.SetTestLongNameNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsTestLongNameNull) AndAlso _
                       (pTestSamplesDS.tparTestSamples.First.TestLongName.Trim <> String.Empty) Then
                        pRowToUpdate.TestLongName = pTestSamplesDS.tparTestSamples.First.TestLongName
                    End If

                    pRowToUpdate.SetPredilutionFactorNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsPredilutionFactorNull) Then pRowToUpdate.PredilutionFactor = pTestSamplesDS.tparTestSamples.First.PredilutionFactor

                    pRowToUpdate.SetPredilutionModeNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsPredilutionModeNull) AndAlso _
                       (pTestSamplesDS.tparTestSamples.First.PredilutionMode.Trim <> String.Empty) Then
                        pRowToUpdate.PredilutionMode = pTestSamplesDS.tparTestSamples.First.PredilutionMode
                    End If

                    pRowToUpdate.SetDiluentSolutionNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsDiluentSolutionNull) AndAlso _
                       (pTestSamplesDS.tparTestSamples.First.DiluentSolution.Trim <> String.Empty) Then
                        pRowToUpdate.DiluentSolution = pTestSamplesDS.tparTestSamples.First.DiluentSolution
                    End If

                    pRowToUpdate.SetPredilutedSampleVolNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsPredilutedSampleVolNull) Then pRowToUpdate.PredilutedSampleVol = pTestSamplesDS.tparTestSamples.First.PredilutedSampleVol

                    pRowToUpdate.SetPredilutedDiluentVolNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsPredilutedDiluentVolNull) Then pRowToUpdate.PredilutedDiluentVol = pTestSamplesDS.tparTestSamples.First.PredilutedDiluentVol

                    pRowToUpdate.SetRedPostdilutionFactorNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsRedPostdilutionFactorNull) Then pRowToUpdate.RedPostdilutionFactor = pTestSamplesDS.tparTestSamples.First.RedPostdilutionFactor

                    pRowToUpdate.SetRedPostSampleVolumeNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsRedPostSampleVolumeNull) Then pRowToUpdate.RedPostSampleVolume = pTestSamplesDS.tparTestSamples.First.RedPostSampleVolume

                    pRowToUpdate.SetIncPostdilutionFactorNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsIncPostdilutionFactorNull) Then pRowToUpdate.IncPostdilutionFactor = pTestSamplesDS.tparTestSamples.First.IncPostdilutionFactor

                    pRowToUpdate.SetIncPostSampleVolumeNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsIncPostSampleVolumeNull) Then pRowToUpdate.IncPostSampleVolume = pTestSamplesDS.tparTestSamples.First.IncPostSampleVolume

                    pRowToUpdate.SetLinearityLimitNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsLinearityLimitNull) Then pRowToUpdate.LinearityLimit = pTestSamplesDS.tparTestSamples.First.LinearityLimit

                    pRowToUpdate.SetDetectionLimitNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsDetectionLimitNull) Then pRowToUpdate.DetectionLimit = pTestSamplesDS.tparTestSamples.First.DetectionLimit

                    pRowToUpdate.SetSlopeFactorANull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsSlopeFactorANull) Then pRowToUpdate.SlopeFactorA = pTestSamplesDS.tparTestSamples.First.SlopeFactorA

                    pRowToUpdate.SetSlopeFactorBNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsSlopeFactorBNull) Then pRowToUpdate.SlopeFactorB = pTestSamplesDS.tparTestSamples.First.SlopeFactorB

                    pRowToUpdate.SetSubstrateDepletionValueNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsSubstrateDepletionValueNull) Then pRowToUpdate.SubstrateDepletionValue = pTestSamplesDS.tparTestSamples.First.SubstrateDepletionValue

                    'Fill fields that will be saved with each Result to move to Historic Module for the Test/SampleType
                    pRowToUpdate.SetBlankAbsorbanceLimitNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsBlankAbsorbanceLimitNull) Then pRowToUpdate.BlankAbsorbanceLimit = pTestSamplesDS.tparTestSamples.First.BlankAbsorbanceLimit

                    pRowToUpdate.SetFactorLowerLimitNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsFactorLowerLimitNull) Then pRowToUpdate.FactorLowerLimit = pTestSamplesDS.tparTestSamples.First.FactorLowerLimit

                    pRowToUpdate.SetFactorUpperLimitNull()
                    If (Not pTestSamplesDS.tparTestSamples.First.IsFactorUpperLimitNull) Then pRowToUpdate.FactorUpperLimit = pTestSamplesDS.tparTestSamples.First.FactorUpperLimit
                    pRowToUpdate.EndEdit()
                Else
                    'This case is not possible...
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' Copy data of an specific Test/Sample saved in Historics Module to a row in a different DS of the same type
        ''' </summary>
        ''' <param name="pRowToUpdate">Row of typed DataSet HistTestSamplesDS that will be updated</param>
        ''' <param name="pSourceDataRow">Row of typed DataSet HistTestSamplesDS containing the source data</param>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 21/09/2012 - Removed fields for Reagent Volume expressed in bomb steps and also field AbsorbanceDilutionFactor
        '''              SA 01/10/2012 - Added parameter pAnalyzerLedPositionsDS containing the relation between WaveLength and LedPosition for all
        '''                              filters used in the Analyzer.  Content of this DS is used to inform fields MainLedPosition and ReferenceLedPosition
        '''              SA 18/10/2012 - Removed fields KineticBlankLimit and BlankAbsorbanceLimit: they have been removed from table thisTestSamples and
        '''                              their value have to be get later from Parameters Programming Module
        ''' </remarks>
        Private Sub FillTestSamplesDSFromHistorics(ByRef pRowToUpdate As HisTestSamplesDS.thisTestSamplesRow, ByVal pSourceDataRow As HisTestSamplesDS.thisTestSamplesRow, _
                                                   ByVal pAnalyzerLedPosDS As AnalyzerLedPositionsDS)
            Try
                pRowToUpdate.BeginEdit()
                pRowToUpdate.HistTestID = pSourceDataRow.HistTestID
                pRowToUpdate.TestVersionNumber = pSourceDataRow.TestVersionNumber
                pRowToUpdate.TestVersionDateTime = pSourceDataRow.TestVersionDateTime
                pRowToUpdate.TestName = pSourceDataRow.TestName
                If (Not pSourceDataRow.IsTestLongNameNull) Then pRowToUpdate.TestLongName = pSourceDataRow.TestLongName
                pRowToUpdate.PreloadedTest = pSourceDataRow.PreloadedTest
                pRowToUpdate.MeasureUnit = pSourceDataRow.MeasureUnit
                pRowToUpdate.AnalysisMode = pSourceDataRow.AnalysisMode
                pRowToUpdate.ReactionType = pSourceDataRow.ReactionType
                pRowToUpdate.DecimalsAllowed = pSourceDataRow.DecimalsAllowed
                pRowToUpdate.ReadingMode = pSourceDataRow.ReadingMode
                pRowToUpdate.FirstReadingCycle = pSourceDataRow.FirstReadingCycle
                If (Not pSourceDataRow.IsSecondReadingCycleNull) Then pRowToUpdate.SecondReadingCycle = pSourceDataRow.SecondReadingCycle

                pRowToUpdate.MainWavelength = pSourceDataRow.MainWavelength
                pRowToUpdate.MainLedPosition = pAnalyzerLedPosDS.tcfgAnalyzerLedPositions.ToList.Where(Function(a) a.WaveLength = CInt(pSourceDataRow.MainWavelength)).First.LedPosition

                If (Not pSourceDataRow.IsReferenceWavelengthNull) Then
                    pRowToUpdate.ReferenceWavelength = pSourceDataRow.ReferenceWavelength
                    pRowToUpdate.ReferenceLedPosition = pAnalyzerLedPosDS.tcfgAnalyzerLedPositions.ToList.Where(Function(a) a.WaveLength = CInt(pSourceDataRow.ReferenceWavelength)).First.LedPosition
                End If

                pRowToUpdate.BlankMode = pSourceDataRow.BlankMode
                If (Not pSourceDataRow.IsProzoneRatioNull) Then pRowToUpdate.ProzoneRatio = pSourceDataRow.ProzoneRatio
                If (Not pSourceDataRow.IsProzoneTime1Null) Then pRowToUpdate.ProzoneTime1 = pSourceDataRow.ProzoneTime1
                If (Not pSourceDataRow.IsProzoneTime2Null) Then pRowToUpdate.ProzoneTime2 = pSourceDataRow.ProzoneTime2
                pRowToUpdate.SampleVolume = pSourceDataRow.SampleVolume
                If (Not pSourceDataRow.IsPredilutionFactorNull) Then pRowToUpdate.PredilutionFactor = pSourceDataRow.PredilutionFactor
                If (Not pSourceDataRow.IsPredilutionModeNull) Then pRowToUpdate.PredilutionMode = pSourceDataRow.PredilutionMode
                If (Not pSourceDataRow.IsDiluentSolutionNull) Then pRowToUpdate.DiluentSolution = pSourceDataRow.DiluentSolution
                If (Not pSourceDataRow.IsPredilutedSampleVolNull) Then pRowToUpdate.PredilutedSampleVol = pSourceDataRow.PredilutedSampleVol
                If (Not pSourceDataRow.IsPredilutedDiluentVolNull) Then pRowToUpdate.PredilutedDiluentVol = pSourceDataRow.PredilutedDiluentVol
                If (Not pSourceDataRow.IsRedPostdilutionFactorNull) Then pRowToUpdate.RedPostdilutionFactor = pSourceDataRow.RedPostdilutionFactor
                If (Not pSourceDataRow.IsRedPostSampleVolumeNull) Then pRowToUpdate.RedPostSampleVolume = pSourceDataRow.RedPostSampleVolume
                If (Not pSourceDataRow.IsIncPostdilutionFactorNull) Then pRowToUpdate.IncPostdilutionFactor = pSourceDataRow.IncPostdilutionFactor
                If (Not pSourceDataRow.IsIncPostSampleVolumeNull) Then pRowToUpdate.IncPostSampleVolume = pSourceDataRow.IncPostSampleVolume
                If (Not pSourceDataRow.IsLinearityLimitNull) Then pRowToUpdate.LinearityLimit = pSourceDataRow.LinearityLimit
                If (Not pSourceDataRow.IsDetectionLimitNull) Then pRowToUpdate.DetectionLimit = pSourceDataRow.DetectionLimit
                If (Not pSourceDataRow.IsSlopeFactorANull) Then pRowToUpdate.SlopeFactorA = pSourceDataRow.SlopeFactorA
                If (Not pSourceDataRow.IsSlopeFactorBNull) Then pRowToUpdate.SlopeFactorB = pSourceDataRow.SlopeFactorB
                If (Not pSourceDataRow.IsSubstrateDepletionValueNull) Then pRowToUpdate.SubstrateDepletionValue = pSourceDataRow.SubstrateDepletionValue
                pRowToUpdate.CalibratorType = pSourceDataRow.CalibratorType
                If (Not pSourceDataRow.IsHistCalibratorIDNull) Then pRowToUpdate.HistCalibratorID = pSourceDataRow.HistCalibratorID
                If (Not pSourceDataRow.IsCurveGrowthTypeNull AndAlso pSourceDataRow.CurveGrowthType <> String.Empty) Then pRowToUpdate.CurveGrowthType = pSourceDataRow.CurveGrowthType
                If (Not pSourceDataRow.IsCurveTypeNull AndAlso pSourceDataRow.CurveType <> String.Empty) Then pRowToUpdate.CurveType = pSourceDataRow.CurveType
                If (Not pSourceDataRow.IsCurveAxisXTypeNull AndAlso pSourceDataRow.CurveAxisXType <> String.Empty) Then pRowToUpdate.CurveAxisXType = pSourceDataRow.CurveAxisXType
                If (Not pSourceDataRow.IsCurveAxisYTypeNull AndAlso pSourceDataRow.CurveAxisYType <> String.Empty) Then pRowToUpdate.CurveAxisYType = pSourceDataRow.CurveAxisYType
                If (Not pSourceDataRow.IsCalibratorFactorNull) Then pRowToUpdate.CalibratorFactor = pSourceDataRow.CalibratorFactor
                pRowToUpdate.HistReagent1ID = pSourceDataRow.HistReagent1ID
                pRowToUpdate.Reagent1Volume = pSourceDataRow.Reagent1Volume
                pRowToUpdate.RedPostReagent1Volume = pSourceDataRow.RedPostReagent1Volume
                pRowToUpdate.IncPostReagent1Volume = pSourceDataRow.IncPostReagent1Volume
                If (Not pSourceDataRow.IsHistReagent2IDNull) Then pRowToUpdate.HistReagent2ID = pSourceDataRow.HistReagent2ID
                If (Not pSourceDataRow.IsReagent2VolumeNull) Then pRowToUpdate.Reagent2Volume = pSourceDataRow.Reagent2Volume
                If (Not pSourceDataRow.IsRedPostReagent2VolumeNull) Then pRowToUpdate.RedPostReagent2Volume = pSourceDataRow.RedPostReagent2Volume
                If (Not pSourceDataRow.IsIncPostReagent2VolumeNull) Then pRowToUpdate.IncPostReagent2Volume = pSourceDataRow.IncPostReagent2Volume
                pRowToUpdate.EndEdit()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region
    End Class
End Namespace
