Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.TO


Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class TestParametersUpdateData
        Inherits UpdateElementParent

#Region "Declarations"
        'TR 03/07/2013 -Variable used to indicated if need to delete previous results of blank and calibrators.
        Private DeleteBlankAndCalibrator As Boolean
#End Region


        'Structure used for the Create, Update, Delete Test.
        Structure TestStructure
            Dim myTestDS As TestsDS 'Update
            Dim myTestSampleDS As TestSamplesDS 'Update
            Dim myReagentsDS As ReagentsDS
            Dim myTestReagentsDS As TestReagentsDS
            Dim myTestReagentsVolumesDS As TestReagentsVolumesDS 'Update
            Dim myCalibratorDS As CalibratorsDS
            Dim myTestCalibratorDS As TestCalibratorsDS 'Update
            Dim myTestCalibratorValuesDS As TestCalibratorValuesDS
            Dim myTestControlsDS As TestControlsDS
            Dim myTestRefRangesDS As TestRefRangesDS
            'Structure to indicate when sample volume is delete.
            Dim myDeletedTestReagentsVolumeTO As List(Of DeletedTestReagentsVolumeTO)

        End Structure

#Region "PUBLIC METHODS"

        ''' <summary>
        ''' Execute required final actions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 04/06/2014 - BT #1646
        ''' </remarks>
        Public Overrides Function DoFinalActions(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                myGlobalDataTO = UpdateTestSortByTestName(pDBConnection)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoFinalActions", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

#End Region


#Region "PRIVATE METHODS"

        ''' <summary>
        ''' Update the local TestsDS wint the information from factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 31/01/2013
        ''' UPDATED BY: TR 03/07/2013 -Implement functionality to delete Delete Blank or Calib results.
        ''' AG 10/03/2014 - #1538 be carefull with the NULL values updating in localTest
        ''' AG 10/04/2014 #1585 spec changes
        ''' </remarks>
        Private Function UpdateLocalTestDS(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Set the Dataset to real Type
                Dim myLocalTestDS As New TestsDS
                Dim myFactoryTestDS As New TestsDS
                'Set local values (client) to my DataSet
                myLocalTestDS = DirectCast(pDataInLocalDB, TestsDS)
                myFactoryTestDS = DirectCast(pDataInFactoryDB, TestsDS)

                'Update the local TestDS whith the information from factory DB
                For Each myLocalTestRow As TestsDS.tparTestsRow In myLocalTestDS.tparTests.Rows
                    myLocalTestRow.BeginEdit()
                    myLocalTestRow.TestName = myFactoryTestDS.tparTests(0).TestName
                    myLocalTestRow.ShortName = myFactoryTestDS.tparTests(0).ShortName

                    If myLocalTestRow.AnalysisMode <> myFactoryTestDS.tparTests(0).AnalysisMode Then 'Delete Blank and calib if diferent
                        DeleteBlankAndCalibrator = True
                    End If
                    myLocalTestRow.AnalysisMode = myFactoryTestDS.tparTests(0).AnalysisMode

                    myLocalTestRow.ReagentsNumber = myFactoryTestDS.tparTests(0).ReagentsNumber 'Indirect affected element

                    'AG 10/04/2014 #1585 - Spec changes -  Do not update these fields
                    'myLocalTestRow.MeasureUnit = myFactoryTestDS.tparTests(0).MeasureUnit
                    'myLocalTestRow.DecimalsAllowed = myFactoryTestDS.tparTests(0).DecimalsAllowed

                    'If myLocalTestRow.BlankMode <> myFactoryTestDS.tparTests(0).BlankMode Then 'Delete Blank and calib if diferent
                    '    DeleteBlankAndCalibrator = True
                    'End If
                    'myLocalTestRow.BlankMode = myFactoryTestDS.tparTests(0).BlankMode
                    'myLocalTestRow.ReplicatesNumber = myFactoryTestDS.tparTests(0).ReplicatesNumber
                    'AG 10/04/2014 #1585

                    If myLocalTestRow.ReactionType <> myFactoryTestDS.tparTests(0).ReactionType Then 'Delete Blank and calib if diferent
                        DeleteBlankAndCalibrator = True
                    End If
                    myLocalTestRow.ReactionType = myFactoryTestDS.tparTests(0).ReactionType

                    If myLocalTestRow.ReadingMode <> myFactoryTestDS.tparTests(0).ReadingMode Then 'Delete Blank and calib if diferent
                        DeleteBlankAndCalibrator = True
                    End If
                    myLocalTestRow.ReadingMode = myFactoryTestDS.tparTests(0).ReadingMode

                    If myLocalTestRow.MainWavelength <> myFactoryTestDS.tparTests(0).MainWavelength Then 'Delete Blank and calib if diferent
                        DeleteBlankAndCalibrator = True
                    End If
                    myLocalTestRow.MainWavelength = myFactoryTestDS.tparTests(0).MainWavelength

                    If myLocalTestRow.FirstReadingCycle <> myFactoryTestDS.tparTests(0).FirstReadingCycle Then 'Delete Blank and calib if diferent
                        DeleteBlankAndCalibrator = True
                    End If
                    myLocalTestRow.FirstReadingCycle = myFactoryTestDS.tparTests(0).FirstReadingCycle

                    'SecondReadingCycle.
                    If Not myFactoryTestDS.tparTests(0).IsSecondReadingCycleNull Then
                        'AG 10/03/2014 - #1538 be carefull with the NULL values in localTest
                        'If myLocalTestRow.SecondReadingCycle <> myFactoryTestDS.tparTests(0).SecondReadingCycle Then 'Delete Blank and calib if diferent
                        If myLocalTestRow.IsSecondReadingCycleNull OrElse myLocalTestRow.SecondReadingCycle <> myFactoryTestDS.tparTests(0).SecondReadingCycle Then 'Delete Blank and calib if diferent
                            DeleteBlankAndCalibrator = True
                        End If
                        myLocalTestRow.SecondReadingCycle = myFactoryTestDS.tparTests(0).SecondReadingCycle
                    Else
                        If Not myLocalTestRow.IsSecondReadingCycleNull Then 'Delete Blank and calib if diferent
                            DeleteBlankAndCalibrator = True
                        End If
                        myLocalTestRow.SetSecondReadingCycleNull()
                    End If

                    'ReferenceWavelength.
                    If Not myFactoryTestDS.tparTests(0).IsReferenceWavelengthNull Then
                        'AG 10/03/2014 - #1538 be carefull with the NULL values in localTest
                        'If myLocalTestRow.ReferenceWavelength <> myFactoryTestDS.tparTests(0).ReferenceWavelength Then 'Delete Blank and calib if diferent
                        If myLocalTestRow.IsReferenceWavelengthNull OrElse myLocalTestRow.ReferenceWavelength <> myFactoryTestDS.tparTests(0).ReferenceWavelength Then 'Delete Blank and calib if diferent
                            DeleteBlankAndCalibrator = True
                        End If
                        myLocalTestRow.ReferenceWavelength = myFactoryTestDS.tparTests(0).ReferenceWavelength
                    Else
                        If Not myLocalTestRow.IsReferenceWavelengthNull Then 'Delete Blank and calib if diferent
                            DeleteBlankAndCalibrator = True
                        End If
                        myLocalTestRow.SetReferenceWavelengthNull()
                    End If

                    'KineticBlankLimit
                    If Not myFactoryTestDS.tparTests(0).IsKineticBlankLimitNull Then
                        myLocalTestRow.KineticBlankLimit = myFactoryTestDS.tparTests(0).KineticBlankLimit
                    Else
                        myLocalTestRow.SetKineticBlankLimitNull()
                    End If
                    myLocalTestRow.EndEdit()
                Next

                'Accept all changes.
                myLocalTestDS.AcceptChanges()

                'Set the updated DS to the globlal.
                myGlobalDataTO.SetDatos = myLocalTestDS

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateLocalTestDS", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the Local TestSamples DS with the  information from factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 31/01/20136
        ''' UPDATED BY: TR 03/07/2013 -Implement functionality to delete Delete Blank or Calib results.
        ''' AG 10/03/2014 - #1538 be carefull with the NULL values updating in localTest
        ''' AG 10/04/2014 #1585 spec changes + fix issue (pred factor do not has to remove old blank/calib results)
        ''' </remarks>
        Private Function UpdateLocalTestSamplesDS(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'GET LOCAL
                Dim myLocalTestDS As New TestsDS
                myLocalTestDS = DirectCast(pDataInLocalDB, TestsDS)

                Dim myLocalTestSamplesDS As New TestSamplesDS
                Dim myTestSamplesDelegates As New tparTestSamplesDAO
                myGlobalDataTO = myTestSamplesDelegates.ReadByTestID(pDBConnection, myLocalTestDS.tparTests(0).TestID)
                If Not myGlobalDataTO.HasError Then
                    myLocalTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                End If

                'GET FACTORY
                Dim myFactoryTestDS As New TestsDS
                myFactoryTestDS = DirectCast(pDataInFactoryDB, TestsDS)
                Dim myFactoryTestSamplesDS As New TestSamplesDS
                Dim myTestParameterUpdateDAO As New TestParametersUpdateDAO
                myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestSampleByTestIDAndSampleType(pDBConnection, myFactoryTestDS.tparTests(0).TestID)
                If Not myGlobalDataTO.HasError Then
                    myFactoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                End If

                Dim myFactoryTestSampleList As New List(Of TestSamplesDS.tparTestSamplesRow)
                'Update the information on local DS with Factory DS.
                For Each LocalTestSampleRow As TestSamplesDS.tparTestSamplesRow In myLocalTestSamplesDS.tparTestSamples.Rows
                    LocalTestSampleRow.BeginEdit()
                    'Get the factory CalibratorType value
                    myFactoryTestSampleList = (From a In myFactoryTestSamplesDS.tparTestSamples _
                                               Where a.SampleType = LocalTestSampleRow.SampleType).ToList()
                    If myFactoryTestSampleList.Count > 0 Then
                        'LocalTestSampleRow.SampleType = myFactoryTestSampleList.First().SampleType
                        LocalTestSampleRow.PredilutionUseFlag = myFactoryTestSampleList.First().PredilutionUseFlag
                        LocalTestSampleRow.SampleVolume = myFactoryTestSampleList.First().SampleVolume
                        LocalTestSampleRow.SampleVolumeSteps = myFactoryTestSampleList.First().SampleVolumeSteps 'Indirect affected element.

                        If Not myFactoryTestSampleList.First().IsPredilutionModeNull Then
                            LocalTestSampleRow.PredilutionMode = myFactoryTestSampleList.First().PredilutionMode
                        Else
                            LocalTestSampleRow.SetPredilutionModeNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsPredilutionFactorNull Then
                            'AG 10/04/2014 #1585 - fix issue
                            ''AG 10/03/2014 - #1538 be carefull with the NULL values in localTest
                            ''If LocalTestSampleRow.PredilutionFactor <> myFactoryTestSampleList.First().PredilutionFactor Then 'Delete Blank and calib if diferent
                            'If LocalTestSampleRow.IsPredilutionFactorNull OrElse LocalTestSampleRow.PredilutionFactor <> myFactoryTestSampleList.First().PredilutionFactor Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585
                            
                            LocalTestSampleRow.PredilutionFactor = myFactoryTestSampleList.First().PredilutionFactor
                            'Indirect affected element.
                            LocalTestSampleRow.PredilutedSampleVol = myFactoryTestSampleList.First().PredilutedSampleVol
                            LocalTestSampleRow.PredilutedSampleVolSteps = myFactoryTestSampleList.First().PredilutedSampleVolSteps
                            LocalTestSampleRow.PredilutedDiluentVol = myFactoryTestSampleList.First().PredilutedDiluentVol
                            LocalTestSampleRow.PreDiluentVolSteps = myFactoryTestSampleList.First().PreDiluentVolSteps

                        Else
                            'AG 10/04/2014 #1585 - fix issue
                            'If Not LocalTestSampleRow.IsPredilutionFactorNull Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.SetPredilutionFactorNull()
                            'Indirect affected element.
                            LocalTestSampleRow.SetPredilutedSampleVolNull()
                            LocalTestSampleRow.SetPredilutedSampleVolStepsNull()
                            LocalTestSampleRow.SetPredilutedDiluentVolNull()
                            LocalTestSampleRow.SetPreDiluentVolStepsNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsDiluentSolutionNull Then
                            'AG 10/04/2014 #1585 - Spec change
                            ''AG 10/03/2014 - #1538 be carefull with the NULL values in localTest
                            ''If LocalTestSampleRow.DiluentSolution <> myFactoryTestSampleList.First().DiluentSolution Then 'Delete Blank and calib if diferent
                            'If LocalTestSampleRow.IsDiluentSolutionNull OrElse LocalTestSampleRow.DiluentSolution <> myFactoryTestSampleList.First().DiluentSolution Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.DiluentSolution = myFactoryTestSampleList.First().DiluentSolution
                        Else
                            'AG 10/04/2014 #1585 - Spec change
                            'If Not LocalTestSampleRow.IsDiluentSolutionNull Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585
                            LocalTestSampleRow.SetDiluentSolutionNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsIncPostdilutionFactorNull Then
                            'AG 10/04/2014 #1585 - Spec change
                            ''AG 10/03/2014 - #1538 be carefull with the NULL values in localTest (add protection althougth PostFactor is a mandatory field)
                            ''If LocalTestSampleRow.IncPostdilutionFactor <> myFactoryTestSampleList.First().IncPostdilutionFactor Then 'Delete Blank and calib if diferent
                            'If LocalTestSampleRow.IsIncPostdilutionFactorNull OrElse LocalTestSampleRow.IncPostdilutionFactor <> myFactoryTestSampleList.First().IncPostdilutionFactor Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.IncPostdilutionFactor = myFactoryTestSampleList.First().IncPostdilutionFactor
                            'Indirect affected element.
                            LocalTestSampleRow.IncPostSampleVolume = myFactoryTestSampleList.First().IncPostSampleVolume
                            LocalTestSampleRow.IncPostSampleVolumeSteps = myFactoryTestSampleList.First().IncPostSampleVolumeSteps
                        Else
                            'AG 10/04/2014 #1585 - Spec change
                            'If Not LocalTestSampleRow.IsIncPostdilutionFactorNull Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.SetIncPostdilutionFactorNull()
                            'Indirect affected element.
                            LocalTestSampleRow.SetIncPostSampleVolumeNull()
                            LocalTestSampleRow.SetIncPostSampleVolumeStepsNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsRedPostdilutionFactorNull Then
                            'AG 10/04/2014 #1585 - Spec change
                            ''AG 10/03/2014 - #1538 be carefull with the NULL values in localTest (add protection althougth PostFactor is a mandatory field)
                            ''If LocalTestSampleRow.RedPostdilutionFactor <> myFactoryTestSampleList.First().RedPostdilutionFactor Then 'Delete Blank and calib if diferent
                            'If LocalTestSampleRow.IsRedPostdilutionFactorNull OrElse LocalTestSampleRow.RedPostdilutionFactor <> myFactoryTestSampleList.First().RedPostdilutionFactor Then 'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.RedPostdilutionFactor = myFactoryTestSampleList.First().RedPostdilutionFactor
                            'Indirect affected element.
                            LocalTestSampleRow.RedPostSampleVolume = myFactoryTestSampleList.First().RedPostSampleVolume
                            LocalTestSampleRow.RedPostSampleVolumeSteps = myFactoryTestSampleList.First().RedPostSampleVolumeSteps
                        Else
                            'AG 10/04/2014 #1585 - Spec change
                            'If Not LocalTestSampleRow.IsRedPostdilutionFactorNull Then  'Delete Blank and calib if diferent
                            '    DeleteBlankAndCalibrator = True
                            'End If
                            'AG 10/04/2014 #1585

                            LocalTestSampleRow.SetRedPostdilutionFactorNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsBlankAbsorbanceLimitNull Then
                            LocalTestSampleRow.BlankAbsorbanceLimit = myFactoryTestSampleList.First().BlankAbsorbanceLimit
                        Else
                            LocalTestSampleRow.SetCalibratorReplicatesNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsKineticsBlankLimitNull Then
                            LocalTestSampleRow.KineticsBlankLimit = myFactoryTestSampleList.First().KineticsBlankLimit
                        Else
                            LocalTestSampleRow.SetKineticsBlankLimitNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsLinearityLimitNull Then
                            LocalTestSampleRow.LinearityLimit = myFactoryTestSampleList.First().LinearityLimit
                        Else
                            LocalTestSampleRow.SetLinearityLimitNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsDetectionLimitNull Then
                            LocalTestSampleRow.DetectionLimit = myFactoryTestSampleList.First().DetectionLimit
                        Else
                            LocalTestSampleRow.SetDetectionLimitNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsFactorLowerLimitNull Then
                            LocalTestSampleRow.FactorLowerLimit = myFactoryTestSampleList.First().FactorLowerLimit
                        Else
                            LocalTestSampleRow.SetFactorLowerLimitNull()
                        End If

                        If Not myFactoryTestSampleList.First().IsFactorUpperLimitNull Then
                            LocalTestSampleRow.FactorUpperLimit = myFactoryTestSampleList.First().FactorUpperLimit
                        Else
                            LocalTestSampleRow.SetFactorUpperLimitNull()
                        End If

                        'TR 07/10/2013 -Include the Slope Factor A and B as factory values.

                        'SlopeFactor A
                        If Not myFactoryTestSampleList.First().IsSlopeFactorANull Then
                            LocalTestSampleRow.SlopeFactorA = myFactoryTestSampleList.First().SlopeFactorA
                        Else
                            LocalTestSampleRow.SetSlopeFactorANull()
                        End If
                        'SlopeFactor B
                        If Not myFactoryTestSampleList.First().IsSlopeFactorBNull Then
                            LocalTestSampleRow.SlopeFactorB = myFactoryTestSampleList.First().SlopeFactorB
                        Else
                            LocalTestSampleRow.SetSlopeFactorBNull()
                        End If

                        'Include the SubstrateDepletionValue because is indicated as factory value but is not updated. insert here.
                        If Not myFactoryTestSampleList.First().IsSubstrateDepletionValueNull Then
                            LocalTestSampleRow.SubstrateDepletionValue = myFactoryTestSampleList.First().SubstrateDepletionValue
                        Else
                            LocalTestSampleRow.SetSubstrateDepletionValueNull()
                        End If

                        'TR 07/10/2013 -END.


                        'UPDATE THE CALIBRATOR INFORMATION
                        If Not LocalTestSampleRow.CalibratorType = "EXPERIMENT" Then
                            If myFactoryTestSampleList.First().CalibratorType = "FACTOR" Then
                                'Set the calibration type.
                                LocalTestSampleRow.CalibratorType = myFactoryTestSampleList.First().CalibratorType
                                'Set the calibration factor value. 
                                LocalTestSampleRow.CalibrationFactor = myFactoryTestSampleList.First().CalibrationFactor

                            ElseIf myFactoryTestSampleList.First().CalibratorType = "ALTERNATIV" Then
                                LocalTestSampleRow.CalibratorType = myFactoryTestSampleList.First().CalibratorType
                                LocalTestSampleRow.SampleTypeAlternative = myFactoryTestSampleList.First().SampleTypeAlternative

                            End If
                        End If

                        LocalTestSampleRow.EndEdit()
                    End If
                Next
                myLocalTestSamplesDS.AcceptChanges()

                'TR 03/07/2013 -Validate there are new sample types to create on Test.
                If Not myGlobalDataTO.HasError Then
                    For Each myTestSample As TestSamplesDS.tparTestSamplesRow In myFactoryTestSamplesDS.tparTestSamples.Rows
                        myFactoryTestSampleList = (From a In myLocalTestSamplesDS.tparTestSamples _
                                                   Where a.TestID = myTestSample.TestID AndAlso a.SampleType = myTestSample.SampleType _
                                                   Select a).ToList()

                        If myFactoryTestSampleList.Count = 0 Then
                            myTestSample.IsNew = True
                            'Insert new Test
                            myLocalTestSamplesDS.tparTestSamples.ImportRow(myTestSample)
                        End If
                    Next
                End If
                'TR 03/07/2013 -END.
                myGlobalDataTO.SetDatos = myLocalTestSamplesDS

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateLocalTestSamplesDS", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the local Test Reagents volume with the information from factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:
        ''' UPDATE BY: TR 03/07/2013 - Implement functionality to delete Delete Blank or Calib results. 
        '''                            On getting the info. related to reagent on local data return erro if 
        '''                            reagent no found, insert validation if error code = MASTER_DATA_MISSING then set HasErro = False.
        ''' </remarks>
        Private Function UpdateLocalTestReagentsVolDS(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, _
                                                      pDataInLocalDB As Object, ByRef pDeletedTestReagentsVolumeTO As List(Of DeletedTestReagentsVolumeTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'GET THE INFORMATION 
                'LOCAL
                Dim myLocalTestDS As New TestsDS
                myLocalTestDS = DirectCast(pDataInLocalDB, TestsDS)
                Dim myLocalTestReagentsVolDS As New TestReagentsVolumesDS
                Dim myTestReagentsVolDelegate As New TestReagentsVolumeDelegate
                myGlobalDataTO = myTestReagentsVolDelegate.GetReagentsVolumesByTestID(pDBConnection, myLocalTestDS.tparTests(0).TestID)
                If Not myGlobalDataTO.HasError Then
                    myLocalTestReagentsVolDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                Else
                    'TR 03/07/2013 -if not reagent found the set the has error = false 
                    If myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING" Then
                        myGlobalDataTO.HasError = False
                    End If
                End If

                'FACTORY
                Dim myFactoryTestDS As New TestsDS
                myFactoryTestDS = DirectCast(pDataInFactoryDB, TestsDS)
                Dim myFactoryTestReagentsVolDS As New TestReagentsVolumesDS
                Dim myTestparametersUpdateDAO As New TestParametersUpdateDAO
                If Not myGlobalDataTO.HasError Then
                    myGlobalDataTO = myTestparametersUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, myFactoryTestDS.tparTests(0).TestID, _
                                                                                                             "")

                    If Not myGlobalDataTO.HasError Then
                        myFactoryTestReagentsVolDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                    End If
                End If

                'UPDATE
                Dim myDeletedTestReagentsVolumeTO As DeletedTestReagentsVolumeTO
                If Not myGlobalDataTO.HasError Then
                    Dim myReagentFactResults As List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                    For Each TestReagVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                                        myLocalTestReagentsVolDS.tparTestReagentsVolumes.Rows

                        myReagentFactResults = (From a In myFactoryTestReagentsVolDS.tparTestReagentsVolumes _
                                                Where a.TestID = TestReagVolRow.TestID _
                                                AndAlso a.SampleType = TestReagVolRow.SampleType _
                                                AndAlso a.ReagentNumber = TestReagVolRow.ReagentNumber Select a).ToList()

                        'Get the reagent number on factory and update the value
                        If myReagentFactResults.Count > 0 Then

                            TestReagVolRow.ReagentVolume = myReagentFactResults.First().ReagentVolume

                            'Indirect affected Elements
                            If TestReagVolRow.ReagentVolumeSteps <> myReagentFactResults.First().ReagentVolumeSteps Then 'Delete Blank and calib if diferent
                                DeleteBlankAndCalibrator = True
                            End If
                            TestReagVolRow.ReagentVolumeSteps = myReagentFactResults.First().ReagentVolumeSteps
                            TestReagVolRow.IncPostReagentVolume = myReagentFactResults.First().IncPostReagentVolume
                            TestReagVolRow.IncPostReagentVolumeSteps = myReagentFactResults.First().IncPostReagentVolumeSteps

                            TestReagVolRow.RedPostReagentVolume = myReagentFactResults.First().RedPostReagentVolume
                            TestReagVolRow.RedPostReagentVolumeSteps = myReagentFactResults.First().RedPostReagentVolumeSteps

                        Else
                            'If the reagent volume do not exist on table means it's delete.
                            'myDeletedTestReagentsVolumeTO = New DeletedTestReagentsVolumeTO()
                            'myDeletedTestReagentsVolumeTO.ReagentID = TestReagVolRow.ReagentID
                            'myDeletedTestReagentsVolumeTO.ReagentNumber = TestReagVolRow.ReagentNumber
                            'myDeletedTestReagentsVolumeTO.TestID = TestReagVolRow.TestID
                            'myDeletedTestReagentsVolumeTO.SampleType = TestReagVolRow.SampleType


                            ''Add to list
                            'pDeletedTestReagentsVolumeTO.Add(myDeletedTestReagentsVolumeTO)
                        End If
                    Next

                    'Validate if all Factory reagents volume exist on local ds
                    For Each TestReagVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                                       myFactoryTestReagentsVolDS.tparTestReagentsVolumes.Rows
                        If (From a In myLocalTestReagentsVolDS.tparTestReagentsVolumes _
                           Where a.ReagentID = TestReagVolRow.ReagentID AndAlso a.SampleType = TestReagVolRow.SampleType Select a.ReagentVolume).Count = 0 Then
                            TestReagVolRow.IsNew = True
                            'Insert the reagent volume
                            myLocalTestReagentsVolDS.tparTestReagentsVolumes.ImportRow(TestReagVolRow)
                        End If
                    Next

                    myGlobalDataTO.SetDatos = myLocalTestReagentsVolDS

                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateLocalTestReagentsVolDS", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information for diferents data structures on the local database
        ''' TestReagentsDS, TestCalibratorsDS, CalibratorsDS, TestCalibratorValuesDS
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID.</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pUpdateStructure"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 01/02/2013
        ''' </remarks>
        Private Function GetOtherDataInfoFromLocal(pDBConnection As SqlClient.SqlConnection, pTestID As Integer, pSampleType As String, _
                                                   ByRef pUpdateStructure As TestStructure) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get Test Reagents
                Dim myTestReagentsDelegate As New TestReagentsDelegate
                Dim myReagentID As Integer = 0
                myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, pTestID)
                If Not myGlobalDataTO.HasError Then
                    pUpdateStructure.myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)
                    If pUpdateStructure.myTestReagentsDS.tparTestReagents.Count > 0 Then
                        'Get Reagent Info
                        Dim myReagentsDelegate As New ReagentsDelegate
                        pUpdateStructure.myReagentsDS = New ReagentsDS
                        For Each TestReagentRow As TestReagentsDS.tparTestReagentsRow In pUpdateStructure.myTestReagentsDS.tparTestReagents.Rows
                            myGlobalDataTO = myReagentsDelegate.GetReagentsData(pDBConnection, TestReagentRow.ReagentID)
                            If Not myGlobalDataTO.HasError Then
                                If DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Count > 0 Then
                                    pUpdateStructure.myReagentsDS.tparReagents.ImportRow(DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents(0))
                                End If
                            End If
                        Next
                    End If
                End If

                'Get TestCalibrator
                If Not myGlobalDataTO.HasError Then
                    Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
                    myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(pDBConnection, pTestID, pSampleType)
                    If Not myGlobalDataTO.HasError Then
                        pUpdateStructure.myTestCalibratorDS = New TestCalibratorsDS
                        pUpdateStructure.myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                    End If

                    'Get Calibrators
                    If Not myGlobalDataTO.HasError Then
                        Dim myCalibratorsDelegate As New CalibratorsDelegate
                        pUpdateStructure.myCalibratorDS = New CalibratorsDS
                        If pUpdateStructure.myTestCalibratorDS.tparTestCalibrators.Count > 0 Then
                            myGlobalDataTO = myCalibratorsDelegate.GetCalibratorData(pDBConnection, pUpdateStructure.myTestCalibratorDS.tparTestCalibrators(0).CalibratorID)
                            If Not myGlobalDataTO.HasError Then
                                pUpdateStructure.myCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                            End If
                        End If
                    End If
                End If

                'Get Test Calibrator Values
                If Not myGlobalDataTO.HasError Then
                    Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate
                    pUpdateStructure.myTestCalibratorValuesDS = New TestCalibratorValuesDS
                    myGlobalDataTO = myTestCalibratorValuesDelegate.GetTestCalibratorValuesByTestIDSampleType(pDBConnection, pTestID, pSampleType)
                    If Not myGlobalDataTO.HasError Then
                        pUpdateStructure.myTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                    End If
                End If

                'GET Local the TestControl
                If Not myGlobalDataTO.HasError Then
                    Dim myTestControlDelegate As New TestControlsDelegate
                    pUpdateStructure.myTestControlsDS = New TestControlsDS
                    myGlobalDataTO = myTestControlDelegate.GetControlsNEW(pDBConnection, "STD", pTestID, pSampleType)
                    If Not myGlobalDataTO.HasError Then
                        pUpdateStructure.myTestControlsDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                        'Fill the TestType Value on Update structure
                        For Each TestControlRow As TestControlsDS.tparTestControlsRow In pUpdateStructure.myTestControlsDS.tparTestControls.Rows
                            TestControlRow.TestType = "STD"
                        Next

                    End If
                End If

                'GET Local the RefRanges.
                If Not myGlobalDataTO.HasError Then
                    Dim myTestRefRangesDelegate As New TestRefRangesDelegate
                    pUpdateStructure.myTestRefRangesDS = New TestRefRangesDS()
                    myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(pDBConnection, pTestID, pSampleType)
                    If Not myGlobalDataTO.HasError Then

                        pUpdateStructure.myTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                    End If
                End If


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetOtherDataInfoFromLocal", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Fill the others requieres structures for Test Creation.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pUpdateStructure"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 07/02/2013
        ''' </remarks>
        Private Function GetOtherDataInfoFromFactory(pDBConnection As SqlClient.SqlConnection, pTestID As Integer, _
                                                    ByRef pUpdateStructure As TestStructure) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                pUpdateStructure.myReagentsDS = New ReagentsDS
                pUpdateStructure.myCalibratorDS = New CalibratorsDS
                Dim mySampleType As String = String.Empty
                Dim myTestDelegate As New TestsDelegate
                'Dim myNewTestID As Integer = 0


                'Set is new Test to create.
                pUpdateStructure.myTestDS.tparTests(0).NewTest = True

                If Not myGlobalDataTO.HasError Then
                    'GET THE ALL THE DATA FROM THE FACTORY
                    'Fill TestSamplesDS
                    Dim myFactoryTestSamplesDS As New TestSamplesDS
                    Dim myTestParameterUpdateDAO As New TestParametersUpdateDAO
                    myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestSampleByTestIDAndSampleType(pDBConnection, pTestID)
                    If Not myGlobalDataTO.HasError Then
                        pUpdateStructure.myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                    End If

                    Dim tempTestCalibratorsDS As New TestCalibratorsDS
                    pUpdateStructure.myTestCalibratorDS = New TestCalibratorsDS
                    Dim tempReagentsVolumeDS As New TestReagentsVolumesDS
                    pUpdateStructure.myTestReagentsVolumesDS = New TestReagentsVolumesDS

                    If Not myGlobalDataTO.HasError Then
                        'For each test sample get the required info.
                        For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In pUpdateStructure.myTestSampleDS.tparTestSamples.Rows
                            'Set the isNew to all rows.
                            testSampleRow.IsNew = True
                            testSampleRow.FactoryCalib = True 'TR 14/02/2013 -Set by default true value to factory calib because is new Test.

                            'Get the Reagents Volume.
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, pTestID, testSampleRow.SampleType)
                                If Not myGlobalDataTO.HasError Then
                                    tempReagentsVolumeDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                                    'Set the isNew to all row and impor to structure
                                    For Each myTestReagentsVolumeRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                                       tempReagentsVolumeDS.tparTestReagentsVolumes.Rows
                                        'myTestReagentsVolumeRow.TestID = myNewTestID
                                        myTestReagentsVolumeRow.IsNew = True
                                        pUpdateStructure.myTestReagentsVolumesDS.tparTestReagentsVolumes.ImportRow(myTestReagentsVolumeRow)

                                    Next
                                End If
                            End If

                            'Get TestCalibrator.
                            If Not myGlobalDataTO.HasError Then
                                myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestCalibratorByTestID(pDBConnection, pTestID, testSampleRow.SampleType)
                                If Not myGlobalDataTO.HasError Then
                                    tempTestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                                    Dim myCalibratorID As Integer = 0

                                    If tempTestCalibratorsDS.tparTestCalibrators.Count > 0 Then
                                        myCalibratorID = tempTestCalibratorsDS.tparTestCalibrators(0).CalibratorID
                                        'import the row to my structure
                                        pUpdateStructure.myTestCalibratorDS.tparTestCalibrators.ImportRow(tempTestCalibratorsDS.tparTestCalibrators(0))
                                        'Clear temporal structure
                                        tempTestCalibratorsDS.tparTestCalibrators.Clear()
                                    End If

                                    For Each TestCalibratorRow As TestCalibratorsDS.tparTestCalibratorsRow In pUpdateStructure.myTestCalibratorDS.tparTestCalibrators.Rows
                                        'Set the isNew to all rows
                                        TestCalibratorRow.IsNew = True

                                        'Get Test Calibrator Values
                                        myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestCalibratorValues(pDBConnection, TestCalibratorRow.TestCalibratorID)
                                        If Not myGlobalDataTO.HasError Then
                                            pUpdateStructure.myTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                                            For Each myTestCalibratorValueRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In _
                                                                    pUpdateStructure.myTestCalibratorValuesDS.tparTestCalibratorValues.Rows
                                                'Set the isNew to all rows
                                                myTestCalibratorValueRow.IsNew = True
                                            Next
                                        End If
                                    Next
                                    'Get Calibrator 
                                    Dim MyFactoryCalibratorDS As New CalibratorsDS
                                    If myCalibratorID > 0 Then
                                        myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryCalibrator(pDBConnection, myCalibratorID)
                                        If Not myGlobalDataTO.HasError Then
                                            MyFactoryCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                                            'Validate if the calibrator exist on Client
                                            myGlobalDataTO = GetCalibratotOnClient(pDBConnection, MyFactoryCalibratorDS.tparCalibrators(0).CalibratorName)
                                            If Not myGlobalDataTO.HasError Then
                                                'Validate if DS has data 
                                                If DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS).tparCalibrators.Count > 0 Then
                                                    pUpdateStructure.myCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                                                Else
                                                    pUpdateStructure.myCalibratorDS = MyFactoryCalibratorDS
                                                    'Set is new calibrator.
                                                    pUpdateStructure.myCalibratorDS.tparCalibrators(0).IsNew = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next

                        'Get Test Reagents.
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myTestParameterUpdateDAO.GetFactroyTestReagents(pDBConnection, pTestID)
                            If Not myGlobalDataTO.HasError Then
                                pUpdateStructure.myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                                For Each TestReagentRow As TestReagentsDS.tparTestReagentsRow In pUpdateStructure.myTestReagentsDS.tparTestReagents.Rows
                                    'Set the isNew to all rows
                                    TestReagentRow.IsNew = True
                                    'TestReagentRow.TestID = myNewTestID
                                    'Get Reagents.
                                    myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryReagent(pDBConnection, TestReagentRow.ReagentID)
                                    If Not myGlobalDataTO.HasError Then
                                        If (DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Count > 0) Then
                                            'Import row to my local structure.
                                            pUpdateStructure.myReagentsDS.tparReagents.ImportRow((DirectCast(myGlobalDataTO.SetDatos, ReagentsDS).tparReagents.Rows(0)))
                                            'Set the isNew to all rows
                                            For Each myReagentRow As ReagentsDS.tparReagentsRow In pUpdateStructure.myReagentsDS.tparReagents.Rows
                                                myReagentRow.IsNew = True
                                            Next
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetOtherDataInfoFromLocal", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get a valid  TestName and shortName.
        ''' This method is used by the Update process.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestsRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/02/02013
        ''' </remarks>
        Private Function RenameTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsRow As TestsDS.tparTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Validate Change the name and validate if do not exist on Client DB.
                Dim isValidNewName As Boolean = False

                Dim myValidTestName As String = String.Empty
                Dim myValidShortName As String = String.Empty
                myValidTestName = pTestsRow.TestName
                myValidShortName = pTestsRow.ShortName

                Dim myTestDAO As New tparTestsDAO
                Dim myTestDS As New TestsDS

                Dim isValidTestName As Boolean = False
                Dim isValidShortName As Boolean = False

                While Not isValidNewName

                    If Not isValidTestName Then
                        'Add the R at the begining
                        myValidTestName = "R" & myValidTestName
                        'Validate the name lenght should not be more than 16
                        If myValidTestName.Length > 16 Then
                            'Remove the last letter
                            myValidTestName = myValidTestName.Remove(myValidTestName.Length - 1)
                        End If
                    End If

                    If Not isValidShortName Then
                        'Add the R at the begining
                        myValidShortName = "R" & myValidShortName
                        ' the Short Name lenght should not be more than 8
                        If myValidShortName.Length > 8 Then
                            myValidShortName = myValidShortName.Remove(myValidShortName.Length - 1)
                        End If
                    End If

                    'Search on local db if name exist.
                    myGlobalDataTO = myTestDAO.ReadByTestName(pDBConnection, myValidTestName)
                    If Not myGlobalDataTO.HasError AndAlso DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count = 0 Then
                        isValidTestName = True
                    End If

                    myGlobalDataTO = myTestDAO.ReadByShortName(pDBConnection, myValidShortName)
                    If Not myGlobalDataTO.HasError AndAlso DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count = 0 Then
                        isValidShortName = True
                    End If

                    If isValidShortName AndAlso isValidTestName Then
                        isValidNewName = True
                    End If
                End While

                pTestsRow.BeginEdit()
                pTestsRow.TestName = myValidTestName
                pTestsRow.ShortName = myValidShortName
                pTestsRow.EndEdit()

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateTestName", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update local test data.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestDS"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 07/02/2013</remarks>
        Private Function UpdateLocalData(pDBConnection As SqlClient.SqlConnection, pTestDS As TestsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim myTestID As Integer = 0
                Dim myTestDelegate As New TestsDelegate
                Dim myTestStructure As New TestStructure
                Dim tempTestSampleDS As New TestSamplesDS
                Dim myLocalTestReagentsVolDS As New TestReagentsVolumesDS
                Dim myTestReagentsVolDelegate As New TestReagentsVolumeDelegate

                myTestID = pTestDS.tparTests(0).TestID

                'GET TESTSAMPLE
                Dim myTestSampleDelegate As New TestSamplesDelegate
                myGlobalDataTO = myTestSampleDelegate.GetSampleDataByTestID(pDBConnection, myTestID)
                If Not myGlobalDataTO.HasError Then
                    myTestStructure.myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                End If

                myTestStructure.myTestDS = pTestDS
                myTestStructure.myTestReagentsVolumesDS = New TestReagentsVolumesDS
                For Each myTestSampleRow As TestSamplesDS.tparTestSamplesRow In myTestStructure.myTestSampleDS.tparTestSamples.Rows
                    'Fill the structures Reagents, TestCalibrator, Calibrator, Calibrator values
                    myGlobalDataTO = GetOtherDataInfoFromLocal(pDBConnection, myTestID, myTestSampleRow.SampleType, myTestStructure)

                    If myGlobalDataTO.HasError Then Exit For
                    'Get the Reagents Volume
                    myGlobalDataTO = myTestReagentsVolDelegate.GetReagentsVolumesByTestID(pDBConnection, myTestID, myTestSampleRow.SampleType)
                    If Not myGlobalDataTO.HasError Then
                        For Each TestReagVol As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                            DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS).tparTestReagentsVolumes.Rows
                            'Import each row
                            myTestStructure.myTestReagentsVolumesDS.tparTestReagentsVolumes.ImportRow(TestReagVol)
                        Next
                    End If
                Next

                'Update the test on local database.
                myGlobalDataTO = myTestDelegate.PrepareTestToSave(pDBConnection, "", "", myTestStructure.myTestDS, myTestStructure.myTestSampleDS, _
                                                          myTestStructure.myTestReagentsVolumesDS, myTestStructure.myReagentsDS, _
                                                          myTestStructure.myTestReagentsDS, myTestStructure.myCalibratorDS, myTestStructure.myTestCalibratorDS, _
                                                          myTestStructure.myTestCalibratorValuesDS, myTestStructure.myTestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                          New List(Of DeletedTestReagentsVolumeTO), New List(Of DeletedTestProgramingTO), _
                                                          New TestSamplesMultirulesDS, myTestStructure.myTestControlsDS, Nothing)

                myGlobalDataTO.SetDatos = myTestStructure

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.FillWithLocalData", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the calibrator data by calibrator name from Client DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalibratorName">Calibrator name</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 05/02/2013
        ''' </remarks>
        Private Function GetCalibratotOnClient(pDBConnection As SqlClient.SqlConnection, pCalibratorName As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myCalibratorDelegate As New CalibratorsDelegate
                myGlobalDataTO = myCalibratorDelegate.ReadByCalibratorName(pDBConnection, pCalibratorName)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetCalibratotOnClient", EventLogEntryType.Information, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        Private Function ExistOnDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object, Optional GetSampleType As Boolean = False) As Boolean
            Dim myResult As Boolean = False

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
            If GetSampleType Then
                myGlobalDataTO = myTestParametersUpdateDAO.GetDataInFactoryDB(pDBConnection, DirectCast(pDataInLocalDB, TestsDS).tparTests(0).TestName, _
                                                                          DirectCast(pDataInLocalDB, TestsDS).tparTests(0).SampleType)
            Else
                myGlobalDataTO = myTestParametersUpdateDAO.GetDataInFactoryDB(pDBConnection, DirectCast(pDataInLocalDB, TestsDS).tparTests(0).TestName)
            End If

            If Not myGlobalDataTO.HasError Then
                If DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count > 0 Then
                    myResult = True
                End If
            End If

            Return myResult
        End Function

        ''' <summary>
        ''' Get the calibrator and reagent informations for a testID and Sample type.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID">Test ID</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <param name="pUpdateStructure"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/07/2013
        ''' </remarks>
        Private Function GetCalibAndReagentInfoFromFactory(pDBConnection As SqlClient.SqlConnection, pTestID As Integer, pSampleType As String, _
                                                    ByRef pUpdateStructure As TestStructure) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim mySampleType As String = String.Empty
                Dim myTestDelegate As New TestsDelegate

                If Not myGlobalDataTO.HasError Then
                    ''GET THE ALL THE DATA FROM THE FACTORY
                    Dim myTestParameterUpdateDAO As New TestParametersUpdateDAO
                    Dim tempTestCalibratorsDS As New TestCalibratorsDS
                    Dim tempReagentsVolumeDS As New TestReagentsVolumesDS

                    If Not myGlobalDataTO.HasError Then
                        'Get the Reagents Volume.
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, pTestID, pSampleType)

                            If Not myGlobalDataTO.HasError Then
                                tempReagentsVolumeDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                                'Set the isNew to all row and impor to structure
                                For Each myTestReagentsVolumeRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                                              tempReagentsVolumeDS.tparTestReagentsVolumes.Rows
                                    'Validate the data is not in the update structure.
                                    If (From a In pUpdateStructure.myTestReagentsVolumesDS.tparTestReagentsVolumes _
                                        Where a.TestID = myTestReagentsVolumeRow.TestID AndAlso a.SampleType = myTestReagentsVolumeRow.SampleType _
                                        Select a).ToList().Count = 0 Then
                                        myTestReagentsVolumeRow.IsNew = True
                                        pUpdateStructure.myTestReagentsVolumesDS.tparTestReagentsVolumes.ImportRow(myTestReagentsVolumeRow)
                                    End If
                                Next
                            End If
                        End If

                        'Get TestCalibrator.
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestCalibratorByTestID(pDBConnection, pTestID, pSampleType)
                            If Not myGlobalDataTO.HasError Then
                                tempTestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                                Dim myCalibratorID As Integer = 0

                                If tempTestCalibratorsDS.tparTestCalibrators.Count > 0 Then
                                    myCalibratorID = tempTestCalibratorsDS.tparTestCalibrators(0).CalibratorID
                                    tempTestCalibratorsDS.tparTestCalibrators(0).IsNew = True 'Set new to true 
                                    'import the row to my structure
                                    pUpdateStructure.myTestCalibratorDS.tparTestCalibrators.ImportRow(tempTestCalibratorsDS.tparTestCalibrators(0))
                                    'Clear temporal structure
                                    tempTestCalibratorsDS.tparTestCalibrators.Clear()
                                End If

                                For Each TestCalibratorRow As TestCalibratorsDS.tparTestCalibratorsRow In pUpdateStructure.myTestCalibratorDS.tparTestCalibrators.Rows
                                    'Set the isNew to all rows
                                    If TestCalibratorRow.IsNew Then
                                        'Get Test Calibrator Values factory database
                                        myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryTestCalibratorValues(pDBConnection, TestCalibratorRow.TestCalibratorID)
                                        If Not myGlobalDataTO.HasError Then
                                            pUpdateStructure.myTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                                            For Each myTestCalibratorValueRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In _
                                                                    pUpdateStructure.myTestCalibratorValuesDS.tparTestCalibratorValues.Rows
                                                'Set the isNew to all rows
                                                myTestCalibratorValueRow.IsNew = True
                                            Next
                                        End If
                                    End If
                                Next

                                'Get Calibrator 
                                Dim MyFactoryCalibratorDS As New CalibratorsDS
                                Dim MyClientCalibratorDS As New CalibratorsDS
                                Dim myStructureCalibratorList As New List(Of CalibratorsDS.tparCalibratorsRow)

                                If myCalibratorID > 0 Then
                                    myGlobalDataTO = myTestParameterUpdateDAO.GetFactoryCalibrator(pDBConnection, myCalibratorID)
                                    If Not myGlobalDataTO.HasError Then
                                        MyFactoryCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                                        'Validate if the calibrator exist on Client
                                        myGlobalDataTO = GetCalibratotOnClient(pDBConnection, MyFactoryCalibratorDS.tparCalibrators(0).CalibratorName)
                                        If Not myGlobalDataTO.HasError Then
                                            MyClientCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS)
                                            'Validate if DS has data 
                                            If MyClientCalibratorDS.tparCalibrators.Count > 0 Then
                                                'Validate if calibrator is into my update structure.
                                                myStructureCalibratorList = (From a In pUpdateStructure.myCalibratorDS.tparCalibrators _
                                                                             Where a.CalibratorID = MyClientCalibratorDS.tparCalibrators(0).CalibratorID _
                                                                             Select a).ToList()

                                                If myStructureCalibratorList.Count = 0 Then
                                                    pUpdateStructure.myCalibratorDS.tparCalibrators.ImportRow(MyClientCalibratorDS.tparCalibrators(0))
                                                End If
                                            Else
                                                pUpdateStructure.myCalibratorDS.tparCalibrators.ImportRow(MyFactoryCalibratorDS.tparCalibrators(0))
                                                'Set is new calibrator.
                                                pUpdateStructure.myCalibratorDS.tparCalibrators(0).IsNew = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetOtherDataInfoFromLocal", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the sort of the Tests as alphabetically order by the name of the test (except User tests)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 04/06/2014 - BT #1646
        ''' </remarks>
        Private Function UpdateTestSortByTestName(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestDelegate As New TestsDelegate

                myGlobalDataTO = myTestDelegate.UpdatePreloadedTestSortByTestName(pDBConnection)
                If Not myGlobalDataTO.HasError Then
                    myGlobalDataTO = myTestDelegate.UpdateUserTestPosition(pDBConnection)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateTestSortByTestName", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

#End Region

        ''' <summary>
        '''  Create new preloaded Test in client DataBase
        '''  Set the calibrator to the test.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 07/02/2013</remarks>
        Protected Overrides Function DoCreateNewElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Create the Test on Client.
                Dim myTestStructure As New TestStructure
                Dim myTestID As Integer = 0
                myTestStructure.myTestDS = DirectCast(pDataInFactoryDB, TestsDS)

                If myTestStructure.myTestDS.tparTests.Count >= 0 Then
                    myTestID = myTestStructure.myTestDS.tparTests(0).TestID
                    myTestStructure.myTestDS.tparTests(0).NewTest = True 'Set is new test.
                    myGlobalDataTO = GetOtherDataInfoFromFactory(pDBConnection, myTestID, myTestStructure)

                    If Not myGlobalDataTO.HasError Then
                        Dim myTestDelegate As New TestsDelegate
                        'Update the test on local database.
                        myGlobalDataTO = myTestDelegate.PrepareTestToSave(pDBConnection, "", "", myTestStructure.myTestDS, myTestStructure.myTestSampleDS, _
                                         myTestStructure.myTestReagentsVolumesDS, myTestStructure.myReagentsDS, myTestStructure.myTestReagentsDS, _
                                         myTestStructure.myCalibratorDS, myTestStructure.myTestCalibratorDS, myTestStructure.myTestCalibratorValuesDS, _
                                         New TestRefRangesDS, New List(Of DeletedCalibratorTO), New List(Of DeletedTestReagentsVolumeTO), _
                                         New List(Of DeletedTestProgramingTO), New TestSamplesMultirulesDS, New TestControlsDS, Nothing)
                        'Clear the structure.
                        myTestStructure = New TestStructure

                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoCreateNewElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementRemoving(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        Protected Overrides Function DoIgnoreElementUpdating(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Return New GlobalDataTO
        End Function

        ''' <summary>
        ''' Remove Factory Test from Client DB
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 06/02/2013
        ''' </remarks>
        Protected Overrides Function DoRemoveFactoryElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myLocalData As New TestsDS
                myLocalData = DirectCast(pDataInLocalDB, TestsDS)
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO

                If myLocalData.tparTests.Count > 0 Then
                    myDeletedTestProgramingTO.TestID = myLocalData.tparTests(0).TestID
                    myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                End If

                Dim myTestDelegate As New TestsDelegate
                myGlobalDataTO = myTestDelegate.PrepareTestToSave(pDBConnection, "", "", New TestsDS, New TestSamplesDS, New TestReagentsVolumesDS, _
                                                                  New ReagentsDS, New TestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                  New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                  New List(Of DeletedTestReagentsVolumeTO), myDeletedTestProgramingList, _
                                                                  New TestSamplesMultirulesDS, New TestControlsDS, Nothing)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Remove sample type that are not on factory db.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 07//02/2013</remarks>
        Protected Overrides Function DoRemoveUserElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myLocalData As New TestsDS
                myLocalData = DirectCast(pDataInLocalDB, TestsDS)
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                'Remove sample type only.
                If myLocalData.tparTests.Count > 0 Then
                    myDeletedTestProgramingTO.TestID = myLocalData.tparTests(0).TestID
                    myDeletedTestProgramingTO.SampleType = myLocalData.tparTests(0).SampleType
                    myDeletedTestProgramingTO.DeleteBlankCalibResults = True
                    myDeletedTestProgramingTO.DeleteOnlyCalibrationResult = False

                    myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                End If

                Dim myTestDelegate As New TestsDelegate
                myGlobalDataTO = myTestDelegate.PrepareTestToSave(pDBConnection, "", "", New TestsDS, New TestSamplesDS, New TestReagentsVolumesDS, _
                                                                  New ReagentsDS, New TestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                  New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                  New List(Of DeletedTestReagentsVolumeTO), myDeletedTestProgramingList, _
                                                                  New TestSamplesMultirulesDS, New TestControlsDS, Nothing)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoRemoveFactoryElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update factory test on Client DB  when there are differences between client and factory database values.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:
        ''' UPDATED BY: TR 04/07/02013 Implement Get the Calibrator and reagent information for a new sample type.
        ''' </remarks>
        Protected Overrides Function DoUpdateFactoryDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestDelegate As New TestsDelegate
                Dim myDataInLocalDB As New TestsDS
                Dim myDataInFactoryDB As New TestsDS

                'TR 19/02/2012 -Before update valiate if there is a  (no Preloaded) test with the same name to rename.
                'Set the values to my local variables
                myDataInFactoryDB = DirectCast(pDataInFactoryDB, TestsDS)
                'Search if the test name exist on the Client DB.
                myGlobalDataTO = myTestDelegate.ExistsTestName(pDBConnection, myDataInFactoryDB.tparTests(0).TestName)
                If Not myGlobalDataTO.HasError Then
                    myDataInLocalDB = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                    'Validate if it's a user test to update
                    ''If Not myDataInLocalDB.tparTests(0).PreloadedTest Then
                    If myDataInLocalDB.tparTests.Count > 0 AndAlso Not myDataInLocalDB.tparTests(0).PreloadedTest Then
                        'Change the Test name on client.
                        myGlobalDataTO = RenameTestName(pDBConnection, myDataInLocalDB.tparTests(0))
                        'Update all the local information to update the test with the new names
                        myGlobalDataTO = UpdateLocalData(pDBConnection, myDataInLocalDB)
                    End If
                    ''End If
                End If
                'TR 19/02/2012 -END.

                If Not myGlobalDataTO.HasError Then

                    Dim myTestID As Integer = 0
                    Dim mySampleType As String = String.Empty
                    Dim myUpdateStructure As New TestStructure
                    Dim myTestVersion As Integer  'TR 03/07/2013

                    'TR 03/07/2013
                    DeleteBlankAndCalibrator = False
                    Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                    Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                    'TR 03/07/2013 -END.

                    'Update the Test on Client DS.
                    myGlobalDataTO = UpdateLocalTestDS(pDBConnection, pDataInFactoryDB, pDataInLocalDB)
                    If Not myGlobalDataTO.HasError Then
                        myUpdateStructure.myTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                        If myUpdateStructure.myTestDS.tparTests.Count > 0 Then
                            'Set the TestID and sample type to get the other tables data.
                            myTestID = myUpdateStructure.myTestDS.tparTests(0).TestID
                            mySampleType = myUpdateStructure.myTestDS.tparTests(0).SampleType
                            myTestVersion = myUpdateStructure.myTestDS.tparTests(0).TestVersionNumber
                        End If
                    End If

                    'Update the TestSample on Client DS
                    If Not myGlobalDataTO.HasError Then
                        myGlobalDataTO = UpdateLocalTestSamplesDS(pDBConnection, pDataInFactoryDB, pDataInLocalDB)
                        If Not myGlobalDataTO.HasError Then
                            myUpdateStructure.myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                        End If
                    End If

                    'Update the TestReagentsVolumes on Client DS
                    If Not myGlobalDataTO.HasError Then
                        'Initialize the structure for delete test reagents.
                        myUpdateStructure.myDeletedTestReagentsVolumeTO = New List(Of DeletedTestReagentsVolumeTO)
                        myGlobalDataTO = UpdateLocalTestReagentsVolDS(pDBConnection, pDataInFactoryDB, pDataInLocalDB, myUpdateStructure.myDeletedTestReagentsVolumeTO)
                        If Not myGlobalDataTO.HasError Then
                            myUpdateStructure.myTestReagentsVolumesDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                        End If
                    End If

                    For Each myTestSampleRow As TestSamplesDS.tparTestSamplesRow In myUpdateStructure.myTestSampleDS.tparTestSamples.Rows

                        If myTestSampleRow.IsNew Then
                            'If new test sample row the the Calib and reagent information from Factory database to complete data
                            myGlobalDataTO = GetCalibAndReagentInfoFromFactory(pDBConnection, myTestSampleRow.TestID, myTestSampleRow.SampleType, myUpdateStructure)
                            myTestSampleRow.FactoryCalib = True 'Set factory calib 'cause it's new.
                        Else
                            'Get other info fo fill structures from Local Tables (Client)
                            myGlobalDataTO = GetOtherDataInfoFromLocal(pDBConnection, myTestSampleRow.TestID, myTestSampleRow.SampleType, myUpdateStructure)
                        End If
                    Next

                    'Get other info fo fill structures from Local Tables (Client)
                    'myGlobalDataTO = GetOtherDataInfoFromLocal(pDBConnection, myTestID, mySampleType, myUpdateStructure)

                    'TR 03/07/2013 -Validate if need to delete prev. blank and calibrator
                    If Not myGlobalDataTO.HasError Then
                        If DeleteBlankAndCalibrator Then
                            myDeletedTestProgramingTO.TestID = myTestID
                            myDeletedTestProgramingTO.DeleteBlankCalibResults = True
                            myDeletedTestProgramingTO.TestVersion = myTestVersion
                            myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                        End If
                    End If
                    'TR 03/07/2013 -END.


                    If Not myGlobalDataTO.HasError Then
                        'After having all the information then update the TEST
                        myGlobalDataTO = myTestDelegate.PrepareTestToSave(pDBConnection, "", "", myUpdateStructure.myTestDS, myUpdateStructure.myTestSampleDS, _
                                                                          myUpdateStructure.myTestReagentsVolumesDS, myUpdateStructure.myReagentsDS, _
                                                                          myUpdateStructure.myTestReagentsDS, myUpdateStructure.myCalibratorDS, _
                                                                          myUpdateStructure.myTestCalibratorDS, myUpdateStructure.myTestCalibratorValuesDS, _
                                                                          myUpdateStructure.myTestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                          myUpdateStructure.myDeletedTestReagentsVolumeTO, myDeletedTestProgramingList, _
                                                                          New TestSamplesMultirulesDS, myUpdateStructure.myTestControlsDS, Nothing)

                        'TR 03/07/2013 -After update set value of DeleteBlankAndCalibrator to false
                        DeleteBlankAndCalibrator = False
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoUpdateFactoryDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Update a user define test with the same name as a factory test(rename processadd and R infrom of the user name)
        ''' after renaming the user test then Create the new Factory Test into client db.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR </remarks>
        Protected Overrides Function DoUpdateUserDefinedElement(pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDataInLocalDB As New TestsDS
                Dim myDataInFactoryDB As New TestsDS

                'Set the values to my local variables
                myDataInLocalDB = DirectCast(pDataInLocalDB, TestsDS)
                myDataInFactoryDB = DirectCast(pDataInFactoryDB, TestsDS)

                If myDataInLocalDB.tparTests.Count > 0 Then
                    'Validate if the test name are the same 
                    If myDataInLocalDB.tparTests(0).TestName = myDataInFactoryDB.tparTests(0).TestName AndAlso Not myDataInLocalDB.tparTests(0).PreloadedTest Then
                        'Change the Test name on client.
                        myGlobalDataTO = RenameTestName(pDBConnection, myDataInLocalDB.tparTests(0))
                        'Update all the local information to update the test with the new names
                        myGlobalDataTO = UpdateLocalData(pDBConnection, myDataInLocalDB)
                    End If
                End If

                If Not myGlobalDataTO.HasError Then
                    'Create the new test on client.
                    myGlobalDataTO = DoCreateNewElement(pDBConnection, pDataInFactoryDB, pDataInLocalDB)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.DoUpdateUserDefinedElement", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all the element to be removed from Client.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 
        ''' </remarks>
        Protected Overrides Function GetAffectedItemsFromFactoryRemoves(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDataSet As New DataSet
                Dim myResultDS As New DataSet
                Dim myFirst As Boolean = True
                Dim myDistingTest As New List(Of DataRow)
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
                'All element to be removed from Client (Get All the test to be removed from client DB.
                myGlobalDataTO = myTestParametersUpdateDAO.GetAffectedItemsFromFactoryRemoves(pDBConnection)
                If Not myGlobalDataTO.HasError Then
                    myDataSet = DirectCast(myGlobalDataTO.SetDatos, DataSet)
                    'Remove duplicate elements on Dataset
                    For Each myRow As DataRow In myDataSet.Tables(0).Rows
                        If myFirst Then
                            myDistingTest.Add(myRow)
                            myFirst = False
                        Else
                            If (From a In myDistingTest Where a("TestID").ToString() = myRow("TestID").ToString() _
                                AndAlso a("SampleType").ToString() = myRow("SampleType").ToString() Select a).ToList().Count = 0 Then
                                myDistingTest.Add(myRow)
                            End If
                        End If
                    Next

                    If myDistingTest.Count > 0 Then
                        myResultDS.Tables.Add(myDistingTest.CopyToDataTable())
                        myGlobalDataTO.SetDatos = myResultDS
                    End If
                End If


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetAffectedItemsFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get items do not macht in the Client DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
        Protected Overrides Function GetAffectedItemsFromFactoryUpdates(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDataSet As New DataSet
                Dim myResultDS As New DataSet
                Dim myFirst As Boolean = True
                Dim myDistingTest As New List(Of DataRow)
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                'Get all elements that does not match with the elements on factory db.
                myGlobalDataTO = myTestParametersUpdateDAO.GetAffectedItemsFromFactory(pDBConnection)
                If Not myGlobalDataTO.HasError Then
                    myDataSet = DirectCast(myGlobalDataTO.SetDatos, DataSet)

                    'Remove duplicate elements on Dataset
                    For Each myRow As DataRow In myDataSet.Tables(0).Rows
                        If myFirst Then
                            myDistingTest.Add(myRow)
                            myFirst = False
                        Else
                            If (From a In myDistingTest Where a("TestID").ToString() = myRow("TestID").ToString() Select a).ToList().Count = 0 Then
                                myDistingTest.Add(myRow)
                            End If
                        End If
                    Next

                    If myDistingTest.Count > 0 Then
                        myResultDS.Tables.Add(myDistingTest.CopyToDataTable())
                        myGlobalDataTO.SetDatos = myResultDS
                    End If
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetAffectedItemsFromFactoryUpdates", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get Test information from factory DB.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 07/2013
        ''' </remarks>
        Protected Overrides Function GetDataInFactoryDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestPArametersUpdaterData As New TestParametersUpdateDAO
                'myGlobalDataTO = myTestPArametersUpdaterData.GetDataInFactoryDB(pDBConnection, pItemRow("TestName").ToString)
                myGlobalDataTO = myTestPArametersUpdaterData.GetDataInFactoryDB(pDBConnection, CInt(pItemRow("TestID").ToString))
                If Not myGlobalDataTO.HasError Then
                    'Fill the row SampleType.
                    If DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count > 0 Then
                        DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).SampleType = pItemRow("SampleType").ToString()
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetDataInFactoryDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get Element information on Client DB
        ''' Search by the test id in case the test is not found on local DB then 
        ''' search one more time by the TestName. because the test could exist but 
        ''' with difrent Test ID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pItemRow"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 01/02/2013</remarks>
        Protected Overrides Function GetDataInLocalDB(pDBConnection As SqlClient.SqlConnection, pItemRow As DataRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestDS As New TestsDS
                Dim myTestDelegate As New TestsDelegate
                'Search on client db by the testID and sample Type
                myGlobalDataTO = myTestDelegate.ReadByTestIDSampleType(pDBConnection, CInt(pItemRow("TestID").ToString()), pItemRow("SampleType").ToString())
                If Not myGlobalDataTO.HasError Then
                    If DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count = 0 Then
                        'Then Search by the test Name
                        myGlobalDataTO = myTestDelegate.ExistsTestName(pDBConnection, pItemRow("TestName").ToString())
                        If Not myGlobalDataTO.HasError Then
                            If DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count > 0 Then
                                'Set the sample type.
                                DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests(0).SampleType = pItemRow("SampleType").ToString()
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetDataInLocalDB", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        Protected Overrides Function GetDataTable(pMyDataSet As DataSet) As DataTable
            Return pMyDataSet.Tables(0)
        End Function

        Protected Overrides Function IsDataEmpty(pDataInDB As Object) As Boolean
            Dim myResult As Boolean = False
            If DirectCast(pDataInDB, DataSet).Tables.Count > 0 AndAlso DirectCast(pDataInDB, DataSet).Tables(0).Rows.Count = 0 Then
                myResult = True
            End If
            Return myResult
        End Function

        ''' <summary>
        ''' Validate if the element is factory element.
        ''' </summary>
        ''' <param name="pDataInDB"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
        Protected Overrides Function IsFactoryDefinedItem(pDataInDB As Object, pDBConnection As SqlClient.SqlConnection) As Boolean
            Dim myResult As Boolean = False
            If DirectCast(pDataInDB, DataSet).Tables.Count > 0 AndAlso DirectCast(pDataInDB, DataSet).Tables(0).Rows.Count > 0 Then
                'The table type is TestsDS, set the value on preloaded test. column
                myResult = DirectCast(pDataInDB, TestsDS).tparTests(0).PreloadedTest
            End If
            Return myResult
        End Function

        ''' <summary>
        ''' Adapted method for Test, requires more validations
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pDataInFactoryDB"></param>
        ''' <param name="pDataInLocalDB"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR
        ''' </remarks>
        Protected Overrides Function GetActionFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection, pDataInFactoryDB As Object, pDataInLocalDB As Object) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                ' If Not Exists in Factory DB and:
                ' If Exists in Local DB as UserTest: Action = RemoveUserDefined
                ' If Exists in Local DB as PreloadedTest: Action = RemoveFactoryDefined
                myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore

                If IsDataEmpty(pDataInLocalDB) Then
                    If IsFactoryDefinedItem(pDataInFactoryDB, pDBConnection) Then 'TR 08/05/2013 send the connection
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveFactoryDefined
                    Else
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveUserDefined
                    End If
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
                ElseIf IsFactoryDefinedItem(pDataInLocalDB, pDBConnection) Then
                    'Search if do not  test exist on datafactoryDB
                    If Not ExistOnDataInFactoryDB(pDBConnection, pDataInFactoryDB, pDataInLocalDB) Then
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveFactoryDefined
                    ElseIf Not ExistOnDataInFactoryDB(pDBConnection, pDataInFactoryDB, pDataInLocalDB, True) Then
                        'Validate if sample type exist on db
                        myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.RemoveUserDefined
                    End If
                Else
                    myGlobalDataTO.SetDatos = LocalActionsWithFactoryRemoves.Ignore
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.GetActionFromFactoryRemoves", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO

        End Function

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS"

        Public Function UpdateRenamedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String, ByVal pShortName As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateRenamedTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all data (TestReagents data, basic TestSample data, Reagents Volumes and Calibrators) for the informed STD TestID 
        ''' (all Sample Types) or, if optional parameter pSampleType is informed, only the data for that specific SampleType. This function is used
        ''' to search data of NEW STD Tests or to search data of NEW SAMPLE TYPE for an existing STD Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <returns>GlobalDataTO containing a TestStructure with all additional data for the Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetOtherTestDataInfoFromFactory(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                        Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestStructure As New TestStructure
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                '(1) Get all Reagents used for the NEW STD Test in FACTORY DB
                myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsByTestID(pDBConnection, pTestID, True)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    'Fill Reagents sub-table in TestStructure
                    myTestStructure.myReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)

                    'Fill TestReagents sub-table in TestStructure
                    myTestStructure.myTestReagentsDS = New TestReagentsDS
                    Dim myTestReagentsRow As TestReagentsDS.tparTestReagentsRow

                    For Each reagentRow As ReagentsDS.tparReagentsRow In myTestStructure.myReagentsDS.tparReagents
                        myTestReagentsRow = myTestStructure.myTestReagentsDS.tparTestReagents.NewtparTestReagentsRow
                        myTestReagentsRow.TestID = pTestID
                        myTestReagentsRow.ReagentID = reagentRow.ReagentID
                        myTestReagentsRow.ReagentNumber = reagentRow.ReagentNumber
                        myTestReagentsRow.IsNew = True
                        myTestStructure.myTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(myTestReagentsRow)
                    Next
                End If

                '(2) Get all data in table tparTestSamples for the Test in Factory DB
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryTestSamplesByTestID(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestSamples sub-table in TestStructure
                        myTestStructure.myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                    End If
                End If

                '(3) Get from table tparTestReagentsVolumes in Factory DB all Reagents Volumes for all Sample Types linked to the Test 
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestReagentsVolumes sub-table in TestStructure
                        myTestStructure.myTestReagentsVolumesDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                    End If
                End If

                '(4) Get from table tparTestCalibrators in Factory DB, all data of the experimental Calibrators used for all Sample Types linked to the Test
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryTestCalibratorByTestID(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestCalibrators sub-table in TestStructure
                        myTestStructure.myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)

                        'Initialize Calibrators sub-table in TestStructure
                        myTestStructure.myCalibratorDS = New CalibratorsDS

                        'For each Calibrator, search it by name in CUSTOMER DB to get the CalibratorID (it always exists due to UPDATE VERSION 
                        'processes Calibrators before STD Tests)
                        Dim myCustomerRow As CalibratorsDS.tparCalibratorsRow
                        Dim myCalibratorsDelegate As New CalibratorsDelegate

                        For Each factoryRow As TestCalibratorsDS.tparTestCalibratorsRow In myTestStructure.myTestCalibratorDS.tparTestCalibrators
                            myGlobalDataTO = myCalibratorsDelegate.ReadByCalibratorName(pDBConnection, factoryRow.CalibratorName)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                'Get the row with Calibrator information from the returned CalibratorsDS
                                myCustomerRow = DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS).tparCalibrators.First

                                'Inform the CalibratorID saved in CUSTOMER DB in the row of sub-table TestCalibrators in TestStructure
                                factoryRow.CalibratorID = myCustomerRow.CalibratorID

                                'Add the row to Calibrators sub-table in TestStructure (if it not exists in it)
                                If (myTestStructure.myCalibratorDS.tparCalibrators.ToList.Where(Function(a) a.CalibratorID = myCustomerRow.CalibratorID).Count = 0) Then
                                    myTestStructure.myCalibratorDS.tparCalibrators.ImportRow(myCustomerRow)
                                End If
                            Else
                                'Unexpected error...
                                Exit For
                            End If
                        Next
                    End If
                End If

                '(5) Get from table tparTestCalibratorValues in Factory DB, values for each point of the experimental Calibrators used for all Sample Types linked to the Test 
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryTestCalibValuesByTestID(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestCalibratorValuesDS sub-table in TestStructure
                        myTestStructure.myTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                    End If

                    'Finally, return all information inside the TestStructure 
                    myGlobalDataTO.SetDatos = myTestStructure
                    myGlobalDataTO.HasError = False
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.GetOtherTestDataInfoFromFactory", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        Public Function CreateNEWSTDTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myNewTestDS As New TestsDS
                Dim myNewTestsList As New TestsDS
                Dim myTestStructure As New TestStructure
                Dim myTestsDelegate As New TestsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                '(1) Search in Factory DB all new STD TESTS
                myGlobalDataTO = myTestParametersUpdateDAO.GetNewFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewTestsList = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    For Each newTest As TestsDS.tparTestsRow In myNewTestsList.tparTests
                        '(2) Get all data in table tparTests in Factory DB
                        myGlobalDataTO = myTestParametersUpdateDAO.GetDataInFactoryDB(pDBConnection, newTest.TestID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myNewTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                            'Rename!!
                            myGlobalDataTO = UpdateRenamedTest(pDBConnection, myNewTestDS.tparTests.First.TestName, myNewTestDS.tparTests.First.ShortName)
                        End If

                        '(4) Get rest of Test data in Factory DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = GetOtherTestDataInfoFromFactory(pDBConnection, newTest.TestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestStructure = DirectCast(myGlobalDataTO.SetDatos, TestStructure)
                            End If
                        End If

                        '(5) Save the NEW STD Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, myNewTestDS, _
                                                                               myTestStructure.myTestSampleDS, myTestStructure.myTestReagentsVolumesDS, _
                                                                               myTestStructure.myReagentsDS, myTestStructure.myTestReagentsDS, _
                                                                               myTestStructure.myCalibratorDS, myTestStructure.myTestCalibratorDS, _
                                                                               myTestStructure.myTestCalibratorValuesDS, New TestRefRangesDS, _
                                                                               New List(Of DeletedCalibratorTO), New List(Of DeletedTestReagentsVolumeTO), _
                                                                               New List(Of DeletedTestProgramingTO), New TestSamplesMultirulesDS, _
                                                                               New TestControlsDS, Nothing)
                        End If
                    Next
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.CreateNEWSTDTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class

End Namespace
