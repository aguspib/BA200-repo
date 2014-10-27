Option Explicit On
Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO


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

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
        ''' <summary>
        ''' Search all STD Tests in FACTORY DB that do not exist in Customer DB and, for each one of them, execute the process of adding it to Customer DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection></param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA-1980)
        ''' </remarks>
        Public Function CREATENewSTDTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myNewTestDS As New TestsDS
                Dim myNewTestsList As New TestsDS
                Dim myNewReagentsDS As New ReagentsDS
                Dim myNewTestReagentsDS As New TestReagentsDS

                Dim myTestStructure As New TestStructure
                Dim myTestsDelegate As New TestsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new STD TESTS
                myGlobalDataTO = myTestParametersUpdateDAO.GetNewFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewTestsList = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    '(2) Process each new STD Test in Factory DB to add it to Customer DB
                    For Each newTest As TestsDS.tparTestsRow In myNewTestsList.tparTests
                        '(2.1) Get all data in table tparTests in Factory DB
                        myGlobalDataTO = myTestParametersUpdateDAO.GetDataInFactoryDB(pDBConnection, newTest.TestID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myNewTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                        End If

                        '(2.2) Verify if there is an User STD Test in Customer DB with the same Name and/or ShortName of the new Factory STD Test, and in this case, 
                        '      rename the User Test 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateRenamedTest(pDBConnection, myNewTestDS.tparTests.First.TestName, myNewTestDS.tparTests.First.ShortName, pUpdateVersionChangesList)
                        End If

                        '(2.3) Get all Reagents used for the NEW STD Test in FACTORY DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsByTestID(pDBConnection, newTest.TestID, True)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                'Fill Reagents sub-table in TestStructure
                                myNewReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)

                                'Fill TestReagents sub-table in TestStructure
                                myNewTestReagentsDS.Clear()
                                Dim myTestReagentsRow As TestReagentsDS.tparTestReagentsRow

                                For Each reagentRow As ReagentsDS.tparReagentsRow In myNewReagentsDS.tparReagents
                                    myTestReagentsRow = myNewTestReagentsDS.tparTestReagents.NewtparTestReagentsRow
                                    myTestReagentsRow.TestID = newTest.TestID
                                    myTestReagentsRow.ReagentID = reagentRow.ReagentID
                                    myTestReagentsRow.ReagentNumber = reagentRow.ReagentNumber
                                    myTestReagentsRow.IsNew = True
                                    myNewTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(myTestReagentsRow)
                                Next
                            End If
                        End If

                        '(2.4) Get rest of Test data in Factory DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = GetOtherTestDataInfoFromFactory(pDBConnection, newTest.TestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestStructure = DirectCast(myGlobalDataTO.SetDatos, TestStructure)
                            End If
                        End If

                        '(2.5) Generate the temporary ReagentIDs and inform them in all Reagents related DataSets
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = SetTempReagentID(pDBConnection, myNewReagentsDS, myNewTestReagentsDS, myTestStructure.myTestReagentsVolumesDS)
                        End If

                        '(2.6) Generate the temporary TestCalibratorIDs and inform them in all Test Calibrators related DataSets
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = SetTempTestCalibratorID(pDBConnection, myTestStructure.myTestCalibratorDS, myTestStructure.myTestCalibratorValuesDS)
                        End If

                        '(2.7) Save the NEW STD Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, myNewTestDS, _
                                                                               myTestStructure.myTestSampleDS, myTestStructure.myTestReagentsVolumesDS, _
                                                                               myNewReagentsDS, myNewTestReagentsDS, _
                                                                               myTestStructure.myCalibratorDS, myTestStructure.myTestCalibratorDS, _
                                                                               myTestStructure.myTestCalibratorValuesDS, New TestRefRangesDS, _
                                                                               New List(Of DeletedCalibratorTO), New List(Of DeletedTestReagentsVolumeTO), _
                                                                               New List(Of DeletedTestProgramingTO), New TestSamplesMultirulesDS, _
                                                                               New TestControlsDS, Nothing)
                        End If

                        '(2.8) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                        '      for each Sample Type added for the new Factory STD Test
                        If (Not myGlobalDataTO.HasError) Then
                            For Each sampleTypeRow As TestSamplesDS.tparTestSamplesRow In myTestStructure.myTestSampleDS.tparTestSamples
                                myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                                myUpdateVersionAddedElementsRow.ElementType = "STD"
                                myUpdateVersionAddedElementsRow.ElementName = myNewTestDS.tparTests.First.TestName & " (" & myNewTestDS.tparTests.First.ShortName & ")"
                                myUpdateVersionAddedElementsRow.SampleType = sampleTypeRow.SampleType
                                pUpdateVersionChangesList.AddedElements.AddAddedElementsRow(myUpdateVersionAddedElementsRow)
                            Next
                            pUpdateVersionChangesList.AddedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.CREATENewSTDTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the maximum ReagentID currently saved in tparReagents table in CUSTOMER DB and update each ReagentID read from FACTORY DB 
        ''' (in all Reagents related DataSets) by adding one to the maximum. This process is needed due to the the way function PrepareTestsToSave 
        ''' in TestsDelegate saves the Test data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pReagentsDS">DS containing the group of Reagents to add</param>
        ''' <param name="pTestReagentsDS">DS containing the group of relations between STD Tests and Reagents to add</param>
        ''' <param name="pTestReagentsVolumesDS">DS containing the group of Reagents Volumes by STD Test/SampleType to add;
        '''                                      it is an optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/10/2014 - BA-1944 
        ''' </remarks>
        Private Function SetTempReagentID(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pReagentsDS As ReagentsDS, ByRef pTestReagentsDS As TestReagentsDS, _
                                          Optional ByRef pTestReagentsVolumesDS As TestReagentsVolumesDS = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get the maximum ReagentID currently saved in table tparReagents in Customer DB
                Dim tempReagentID As Integer = 0
                Dim myReagentsDelegate As New ReagentsDelegate

                myGlobalDataTO = myReagentsDelegate.GetMaxReagentID(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    tempReagentID = Convert.ToInt32(myGlobalDataTO.SetDatos)

                    'Set the temporary ReagentID only for NEW Reagents
                    For Each reagentRow As ReagentsDS.tparReagentsRow In pReagentsDS.tparReagents.ToList.Where(Function(a) a.IsNew = True)
                        'Generate the temporary ReagentID 
                        tempReagentID += 1

                        'Search in pTestReagentsDS the row for the Reagent in process and change value of field ReagentID
                        pTestReagentsDS.tparTestReagents.ToList.Where(Function(a) a.ReagentID = reagentRow.ReagentID).First.ReagentID = tempReagentID

                        'Search in pTestReagentsVolumesDS all rows for the Reagent in process and change value of field ReagentID for all of them
                        If (Not pTestReagentsVolumesDS Is Nothing) Then
                            For Each reagentVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow _
                                                   In pTestReagentsVolumesDS.tparTestReagentsVolumes.ToList.Where(Function(a) a.ReagentID = reagentRow.ReagentID)
                                reagentVolRow.ReagentID = tempReagentID
                            Next
                        End If

                        'Finally, change value of field ReagentID for the current row
                        reagentRow.ReagentID = tempReagentID
                    Next

                    pReagentsDS.AcceptChanges()
                    pTestReagentsDS.AcceptChanges()
                    If (Not pTestReagentsVolumesDS Is Nothing) Then pTestReagentsVolumesDS.AcceptChanges()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.SetTempReagentID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the maximum TestCalibratorID currently saved in tparTestCalibrators table in CUSTOMER DB and update each Test/SampleType/Calibrator relation
        ''' read from FACTORY DB (in all Test Calibrators related DataSets) by adding one to the maximum. This process is needed due to the the way function 
        ''' PrepareTestsToSave in TestsDelegate saves the Test data
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorsDS">DS containing the group of relations between STD Tests and Calibrators to add</param>
        ''' <param name="pTestCalibValuesDS">DS containing the group of Calibrator Values (by Calibrator point) for each  by STD Test/SampleType to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 09/10/2014 - BA-1944 
        ''' </remarks>
        Private Function SetTempTestCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pTestCalibratorsDS As TestCalibratorsDS, _
                                                 ByRef pTestCalibValuesDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get the maximum TestCalibratorID currently saved in table tparTestCalibrators in Customer DB
                Dim tempTestCalibratorID As Integer = 0
                Dim myTestCalibsDelegate As New TestCalibratorsDelegate

                myGlobalDataTO = myTestCalibsDelegate.GetMaxTestCalibratorID(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    tempTestCalibratorID = Convert.ToInt32(myGlobalDataTO.SetDatos)

                    For Each testCalibratorRow As TestCalibratorsDS.tparTestCalibratorsRow In pTestCalibratorsDS.tparTestCalibrators
                        'Generate the temporary TestCalibratorID 
                        tempTestCalibratorID += 1

                        'Search in pTestCalibValuesDS all rows for the Test/SampleType/Calibrator relation in process and change value of field TestCalibratorID for all of them
                        For Each testCalibValueRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow _
                                                   In pTestCalibValuesDS.tparTestCalibratorValues.ToList.Where(Function(a) a.TestCalibratorID = testCalibratorRow.TestCalibratorID)
                            testCalibValueRow.TestCalibratorID = tempTestCalibratorID
                        Next

                        'Finally, change value of field TestCalibratorID for the current row
                        testCalibratorRow.TestCalibratorID = tempTestCalibratorID
                    Next

                    pTestCalibratorsDS.AcceptChanges()
                    pTestCalibValuesDS.AcceptChanges()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.SetTempTestCalibratorID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there is an User STD Test in Customer DB with the same Name and/or ShortName of a new Factory STD Test, and in this case, rename
        ''' the User Test by adding as many "R" letters at the beginning of the Name and ShortName as needed until get an unique Name and ShortName
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestName">Name of the new Factory STD Test to verify</param>
        ''' <param name="pShortName">ShortName of the new Factory STD Test to verify</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA-1980)
        ''' </remarks>
        Private Function UpdateRenamedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String, ByVal pShortName As String, _
                                           ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestsDS As New TestsDS
                Dim myAuxTestsDS As New TestsDS
                Dim myTestDelegate As New TestsDelegate

                Dim myTestReagentsDS As New TestReagentsDS
                Dim myTestReagentsDelegate As New TestReagentsDelegate

                Dim myReagentsDS As New ReagentsDS
                Dim myReagentsDelegate As New ReagentsDelegate
                Dim myReagentsRow As ReagentsDS.tparReagentsRow

                Dim myCalcTestsDelegate As New CalculatedTestsDelegate

                Dim myQCTestSamplesDS As New HistoryTestSamplesDS
                Dim myQCTestSamplesDelegate As New HistoryTestSamplesDelegate
                Dim myQCTestSamplesRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow

                Dim myHistTestSamplesDAO As New thisTestSamplesDAO

                Dim myUpdateVersionRenamedElementsRow As UpdateVersionChangesDS.RenamedElementsRow

                '(1) Search if there is an User Test with the same Name...
                myGlobalDataTO = myTestDelegate.ExistsTestName(pDBConnection, pTestName)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                End If

                '(2) Search if there is an User Test with the same ShortName
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestDelegate.ReadByShortName(pDBConnection, pShortName)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myAuxTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                    End If
                End If

                '(3) Merge results of both searches
                If (Not myGlobalDataTO.HasError) Then
                    If (myTestsDS.tparTests.Rows.Count > 0 AndAlso myAuxTestsDS.tparTests.Rows.Count > 0) Then
                        If (myTestsDS.tparTests.First.TestID <> myAuxTestsDS.tparTests.First.TestID) Then
                            myTestsDS.tparTests.ImportRow(myAuxTestsDS.tparTests.First)
                        End If
                    ElseIf (myTestsDS.tparTests.Rows.Count = 0 AndAlso myAuxTestsDS.tparTests.Rows.Count > 0) Then
                        myTestsDS.tparTests.ImportRow(myAuxTestsDS.tparTests.First)
                    End If
                End If

                '(4) Process each one of the STD Tests found
                For Each testRow As TestsDS.tparTestsRow In myTestsDS.tparTests
                    'Rename the STD Test (add as many "R" letters as needed at the beginning of Name and ShortName)
                    myGlobalDataTO = RenameTestName(pDBConnection, testRow)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        If (Convert.ToBoolean(myGlobalDataTO.SetDatos)) Then
                            '(4.1) Update the renamed STD Test in Customer DB
                            myAuxTestsDS.Clear()
                            myAuxTestsDS.tparTests.ImportRow(testRow)
                            myAuxTestsDS.AcceptChanges()

                            myGlobalDataTO = myTestDelegate.Update(pDBConnection, myAuxTestsDS)

                            '(4.2) Rename the linked Reagents and update them in Customer DB 
                            If (Not myGlobalDataTO.HasError) Then
                                'Get all Reagents linked to the STD Test in Customer DB
                                myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, testRow.TestID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                                    'Rename the linked Reagents using the new Test Name
                                    myReagentsDS.Clear()
                                    For Each testReagent As TestReagentsDS.tparTestReagentsRow In myTestReagentsDS.tparTestReagents
                                        'Rename the Reagent and add it to a ReagentsDS
                                        myReagentsRow = myReagentsDS.tparReagents.NewtparReagentsRow
                                        myReagentsRow.ReagentID = testReagent.ReagentID
                                        myReagentsRow.ReagentName = testRow.TestName & "-" & testReagent.ReagentNumber.ToString
                                        myReagentsDS.tparReagents.AddtparReagentsRow(myReagentsRow)
                                    Next
                                    myReagentsDS.AcceptChanges()

                                    'Finally, update the Reagents
                                    myGlobalDataTO = myReagentsDelegate.Update(pDBConnection, myReagentsDS)
                                End If
                            End If

                            '(4.3) Update field FormulaText of all Calculated Tests containing the renamed STD Test in their Formula
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myCalcTestsDelegate.UpdateFormulaText(pDBConnection, "STD", testRow.TestID)
                            End If

                            '(4.4) Update the TestName and ShortName in QC Module (only for OPEN Tests)
                            If (Not myGlobalDataTO.HasError) Then
                                myQCTestSamplesRow = myQCTestSamplesDS.tqcHistoryTestSamples.NewtqcHistoryTestSamplesRow()
                                myQCTestSamplesRow.TestType = "STD"
                                myQCTestSamplesRow.TestID = testRow.TestID
                                myQCTestSamplesRow.TestName = testRow.TestName
                                myQCTestSamplesRow.TestShortName = testRow.ShortName
                                myQCTestSamplesRow.PreloadedTest = testRow.PreloadedTest
                                myQCTestSamplesRow.MeasureUnit = testRow.MeasureUnit
                                myQCTestSamplesRow.DecimalsAllowed = testRow.DecimalsAllowed
                                myQCTestSamplesDS.tqcHistoryTestSamples.AddtqcHistoryTestSamplesRow(myQCTestSamplesRow)

                                myGlobalDataTO = myQCTestSamplesDelegate.UpdateByTestIDNEW(pDBConnection, myQCTestSamplesDS)
                            End If

                            '(4.5) Update the TestName and ShortName in Historic Module (only for OPEN Tests) - Delegate Class is not visible from this Class, 
                            '      and due to that, the function in DAO Class is directly used
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myHistTestSamplesDAO.UpdateNameByTestID(pDBConnection, testRow.TestID, testRow.TestName)
                            End If

                            '(4.6) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table RenamedElements)
                            myUpdateVersionRenamedElementsRow = pUpdateVersionChangesList.RenamedElements.NewRenamedElementsRow
                            myUpdateVersionRenamedElementsRow.ElementType = "STD"
                            myUpdateVersionRenamedElementsRow.PreviousName = pTestName & " (" & pShortName & ")"
                            myUpdateVersionRenamedElementsRow.UpdatedName = testRow.TestName & " (" & testRow.ShortName & ")"
                            pUpdateVersionChangesList.RenamedElements.AddRenamedElementsRow(myUpdateVersionRenamedElementsRow)
                            pUpdateVersionChangesList.RenamedElements.AcceptChanges()
                        Else
                            'If it was not possible to rename the Test (really unlikely case), it is considered an error
                            myGlobalDataTO.HasError = True
                        End If
                    End If

                    'If an error has been raised, then the process is finished
                    If (myGlobalDataTO.HasError) Then Exit For
                Next
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
        ''' Search in FACTORY DB all data (basic TestSample data, Reagents Volumes and Calibrators) for the informed STD TestID 
        ''' (all Sample Types) or, if optional parameter pSampleType is informed, only the data for that specific SampleType. This function is used
        ''' to search data of NEW STD Tests or to search data of NEW SAMPLE TYPE for an existing STD Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <returns>GlobalDataTO containing a TestStructure with all additional data for the Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA-1980)
        ''' </remarks>
        Private Function GetOtherTestDataInfoFromFactory(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                         Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestStructure As New TestStructure
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                '(1) Get all data in table tparTestSamples for the Test in Factory DB
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryTestSamplesByTestID(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestSamples sub-table in TestStructure
                        myTestStructure.myTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)
                    End If
                End If

                '(2) Get from table tparTestReagentsVolumes in Factory DB all Reagents Volumes for all Sample Types linked to the Test 
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestReagentsVolumes sub-table in TestStructure
                        myTestStructure.myTestReagentsVolumesDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                    End If
                End If

                '(3) Get from table tparTestCalibrators in Factory DB, all data of the experimental Calibrators used for all Sample Types linked to the Test
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

                '(4) Get from table tparTestCalibratorValues in Factory DB, values for each point of the experimental Calibrators used for all Sample Types linked to the Test 
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryTestCalibValuesByTestID(pDBConnection, pTestID, pSampleType, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Fill TestCalibratorValuesDS sub-table in TestStructure
                        myTestStructure.myTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                    End If

                    'Finally, return all information inside the TestStructure 
                    myGlobalDataTO.SetDatos = myTestStructure
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

        ''' <summary>
        ''' Add "R" letters at the beginning of a STD Test Name and ShortName until get an unique Name/ShortName.
        ''' Used during Update Version process when a new Factory STD Test is added and there is an User Test in 
        ''' Customer DB with the same Test Name and/or ShortName of the one to add
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsRow">Row of TestsDS containing the basic data of the STD Test to rename</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the STD Test has been renamed (when TRUE)</returns>
        ''' <remarks>
        ''' Created by:  TR 04/02/2013
        ''' Modified by: SA 08/10/2014 - BA-1944 (SubTask BA-1980) ==> Return a Boolean value to indicate if the STD Test has been renamed
        ''' </remarks>
        Private Function RenameTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pTestsRow As TestsDS.tparTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myTestDS As New TestsDS
                Dim myTestsDelegate As New TestsDelegate

                Dim isValidNewName As Boolean = False
                Dim isValidTestName As Boolean = False
                Dim isValidShortName As Boolean = False

                Dim numOfRs As Integer = 1
                Dim errorFound As Boolean = False
                Dim myValidTestName As String = pTestsRow.TestName
                Dim myValidShortName As String = pTestsRow.ShortName

                While (Not isValidNewName AndAlso numOfRs < 16 AndAlso Not errorFound)
                    If (Not isValidTestName) Then
                        'Add an "R" at the beginning of the Test Name
                        myValidTestName = "R" & myValidTestName

                        'If the length of the new Test Name is greater than 16, remove the last character
                        If (myValidTestName.Length > 16) Then myValidTestName = myValidTestName.Remove(myValidTestName.Length - 1)

                        'Verify if the new Test Name is unique in Customer DB (there is not another Test with the same Name)
                        myGlobalDataTO = myTestsDelegate.ExistsTestName(pDBConnection, myValidTestName)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidTestName = (DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count = 0)
                        Else
                            errorFound = True
                        End If
                    End If

                    If (Not isValidShortName) Then
                        'Add an "R" at the beginning of the Test Short Name
                        myValidShortName = "R" & myValidShortName

                        'If the length of the new Test Short Name is greater than 8, remove the last character
                        If (myValidShortName.Length > 8) Then myValidShortName = myValidShortName.Remove(myValidShortName.Length - 1)

                        'Verify if the new Test Short Name is unique in Customer DB (there is not another Test with the same Short Name)
                        myGlobalDataTO = myTestsDelegate.ReadByShortName(pDBConnection, myValidShortName)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidShortName = (DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count = 0)
                        Else
                            errorFound = True
                        End If
                    End If

                    'The rename is accepted with the rename of both fields Name and ShortName is accepted
                    isValidNewName = (isValidShortName AndAlso isValidTestName)

                    'Just to avoid the very unlikely probability of endless loop 
                    numOfRs += 1
                End While

                'Finally, update the name in the TestsDS row received as entry parameter
                If (isValidNewName) Then
                    pTestsRow.BeginEdit()
                    pTestsRow.TestName = myValidTestName
                    pTestsRow.ShortName = myValidShortName
                    pTestsRow.EndEdit()
                End If

                myGlobalDataTO.SetDatos = isValidNewName
                myGlobalDataTO.HasError = errorFound

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.RenameTestName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search all Tests that should be deleted from CUSTOMER DB (those preloaded STD Tests that exist in CUSTOMER DB but not in FACTORY DB) and 
        ''' remove them from CUSTOMER DB 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1983)
        ''' </remarks>
        Public Function DELETERemovedSTDTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myDeletedTestDS As New TestsDS
                Dim myTestSamplesDS As New TestsDS
                Dim myTestsDelegate As New TestsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                Dim myUpdateVersionDeletedElementsRow As UpdateVersionChangesDS.DeletedElementsRow

                '(1) Search in Customer DB all Preloaded STD TESTS that do not exist in Factory DB:
                myGlobalDataTO = myTestParametersUpdateDAO.GetDeletedPreloadedSTDTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myDeletedTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    '(2) Process each returned STD Test to delete it from CUSTOMER DB
                    For Each deletedTest As TestsDS.tparTestsRow In myDeletedTestDS.tparTests
                        '(2.1) Add the TestID to the TO of Tests to delete
                        myDeletedTestProgramingTO.TestID = deletedTest.TestID
                        myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)

                        '(2.2) Get all different Sample Types for the Test in CUSTOMER DB 
                        myGlobalDataTO = myTestsDelegate.ReadByTestIDSampleType(pDBConnection, deletedTest.TestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                        End If

                        '(2.3) Delete the STD Test from CUSTOMER DB
                        myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, New TestsDS, New TestSamplesDS, New TestReagentsVolumesDS, _
                                                                           New ReagentsDS, New TestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                           New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                           New List(Of DeletedTestReagentsVolumeTO), myDeletedTestProgramingList, _
                                                                           New TestSamplesMultirulesDS, New TestControlsDS, Nothing)

                        '(2.4) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table DeletedElements) 
                        '      for each Sample Type that was linked to the deleted STD Test
                        If (Not myGlobalDataTO.HasError) Then
                            For Each sampleTypeRow As TestsDS.tparTestsRow In myTestSamplesDS.tparTests
                                myUpdateVersionDeletedElementsRow = pUpdateVersionChangesList.DeletedElements.NewDeletedElementsRow
                                myUpdateVersionDeletedElementsRow.ElementType = "STD"
                                myUpdateVersionDeletedElementsRow.ElementName = sampleTypeRow.TestName & " (" & sampleTypeRow.ShortName & ")"
                                myUpdateVersionDeletedElementsRow.SampleType = sampleTypeRow.SampleType
                                pUpdateVersionChangesList.DeletedElements.AddDeletedElementsRow(myUpdateVersionDeletedElementsRow)
                            Next
                            pUpdateVersionChangesList.DeletedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

                myDeletedTestProgramingTO = Nothing
                myDeletedTestProgramingList = Nothing
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.DELETERemovedSTDTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search all Tests that should be deleted from CUSTOMER DB (those preloaded STD Tests that exist in CUSTOMER DB but not in FACTORY DB) and 
        ''' remove them from CUSTOMER DB 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1983)
        ''' </remarks>
        Public Function DELETERemovedSTDTestSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myDeletedTestSamplesDS As New TestsDS
                Dim myTestsDS As New TestsDS
                Dim myTestsDelegate As New TestsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                Dim myUpdateVersionDeletedElementsRow As UpdateVersionChangesDS.DeletedElementsRow

                '(1) Search in Customer DB all Sample Types for Preloaded STD TESTS that do not exist in Factory DB:
                myGlobalDataTO = myTestParametersUpdateDAO.GetDeletedPreloadedSTDTestSamples(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myDeletedTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    '(2) Process each returned STD Test/Sample Type to delete it from CUSTOMER DB
                    For Each deletedTestSample As TestsDS.tparTestsRow In myDeletedTestSamplesDS.tparTests
                        '(2.1) Get Name and TestVersionNumber of the Test in CUSTOMER DB 
                        myGlobalDataTO = myTestsDelegate.Read(pDBConnection, deletedTestSample.TestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                        End If

                        '(2.2) Add the TestID/SampleType to the TO of Test/SampleTypes to delete
                        If (Not myGlobalDataTO.HasError) Then
                            myDeletedTestProgramingTO.TestID = deletedTestSample.TestID
                            myDeletedTestProgramingTO.SampleType = deletedTestSample.SampleType
                            myDeletedTestProgramingTO.TestVersion = myTestsDS.tparTests.First.TestVersionNumber
                            myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)

                            '(2.3) Delete the STD Test/Sample Type from CUSTOMER DB
                            myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, New TestsDS, New TestSamplesDS, New TestReagentsVolumesDS, _
                                                                               New ReagentsDS, New TestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                               New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                               New List(Of DeletedTestReagentsVolumeTO), myDeletedTestProgramingList, _
                                                                               New TestSamplesMultirulesDS, New TestControlsDS, Nothing)
                        End If

                        '(2.4) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table DeletedElements) 
                        '      for the deleted STD Test/Sample Type
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionDeletedElementsRow = pUpdateVersionChangesList.DeletedElements.NewDeletedElementsRow
                            myUpdateVersionDeletedElementsRow.ElementType = "STD"
                            myUpdateVersionDeletedElementsRow.ElementName = myTestsDS.tparTests.First.TestName & " (" & myTestsDS.tparTests.First.ShortName & ")"
                            myUpdateVersionDeletedElementsRow.SampleType = deletedTestSample.SampleType
                            pUpdateVersionChangesList.DeletedElements.AddDeletedElementsRow(myUpdateVersionDeletedElementsRow)
                            pUpdateVersionChangesList.DeletedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

                myDeletedTestProgramingTO = Nothing
                myDeletedTestProgramingList = Nothing
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.DELETERemovedSTDTestSamples", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a STD Test, compare value of relevant fields in table tparTests in CUSTOMER DB with value of the same fields in FACTORY DB, and update in CUSTOMER DB
        ''' all modified fields.
        ''' </summary>
        ''' <param name="pFactoryTestRow">Row of TestsDS containing the Test data in FACTORY DB</param>
        ''' <param name="pCustomerTestRow">Row of TestsDS containing the Test data in CUSTOMER DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value that indicates (when TRUE) that previous Blank and Calibrator Results have to be deleted due to
        '''          at least one of the following fields have changed: AnalysisMode, ReactionType, ReadingMode, MainWaveLength, ReferenceWaveLength,
        '''          FirstReadingCycle or SecondReadingCycle</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1983)
        ''' </remarks>
        Private Function UpdateCustomerTest(ByVal pFactoryTestRow As TestsDS.tparTestsRow, ByVal pCustomerTestRow As TestsDS.tparTestsRow, _
                                            ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim deleteBlkCalibResults As Boolean = False
                Dim myTestName As String = pCustomerTestRow.TestName & " (" & pCustomerTestRow.ShortName & ")"

                'Verify if field AnalysisMode has changed; in this case, it is possible that the ReagentsNumber field may also have changed
                '(if the the AnalysisMode has changed from Mono to Bi Reagent or vice versa)
                pCustomerTestRow.BeginEdit()
                If (pCustomerTestRow.AnalysisMode <> pFactoryTestRow.AnalysisMode) Then
                    'Add a row for field AnalysisMode in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "AnalysisMode", pCustomerTestRow.AnalysisMode, _
                                                        pFactoryTestRow.AnalysisMode)

                    'Update fields in the Customer DataSet
                    pCustomerTestRow.AnalysisMode = pFactoryTestRow.AnalysisMode
                    pCustomerTestRow.ReagentsNumber = pFactoryTestRow.ReagentsNumber
                    deleteBlkCalibResults = True
                End If

                'Verify if field ReactionType has changed
                If (pCustomerTestRow.ReactionType <> pFactoryTestRow.ReactionType) Then
                    'Add a row for field ReactionType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "ReactionType", pCustomerTestRow.ReactionType, _
                                                        pFactoryTestRow.ReactionType)

                    'Update field in the Customer DataSet
                    pCustomerTestRow.ReactionType = pFactoryTestRow.ReactionType
                    deleteBlkCalibResults = True
                End If

                'Verify if field ReadingMode has changed
                If (pCustomerTestRow.ReadingMode <> pFactoryTestRow.ReadingMode) Then
                    'Add a row for field ReadingMode in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "ReadingMode", pCustomerTestRow.ReadingMode, _
                                                        pFactoryTestRow.ReadingMode)

                    'Update field in the Customer DataSet
                    pCustomerTestRow.ReadingMode = pFactoryTestRow.ReadingMode
                    deleteBlkCalibResults = True
                End If

                'Verify if field MainWaveLenth has changed
                If (pCustomerTestRow.MainWavelength <> pFactoryTestRow.MainWavelength) Then
                    'Add a row for field MainWavelength in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "MainWavelength", pCustomerTestRow.MainWavelength.ToString, _
                                                        pFactoryTestRow.MainWavelength.ToString)

                    'Update field in the Customer DataSet
                    pCustomerTestRow.MainWavelength = pFactoryTestRow.MainWavelength
                    deleteBlkCalibResults = True
                End If

                'Verify if field FirstReadingCycle has changed
                If (pCustomerTestRow.FirstReadingCycle <> pFactoryTestRow.FirstReadingCycle) Then
                    'Add a row for field FirstReadingCycle in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "FirstReadingCycle", pCustomerTestRow.FirstReadingCycle.ToString, _
                                                        pFactoryTestRow.FirstReadingCycle.ToString)

                    'Update field in the Customer DataSet
                    pCustomerTestRow.FirstReadingCycle = pFactoryTestRow.FirstReadingCycle
                    deleteBlkCalibResults = True
                End If

                'Verify if field SecondReadingCycle has changed
                Dim mySndReading As String = "--"
                If (Not pFactoryTestRow.IsSecondReadingCycleNull) Then
                    If (pCustomerTestRow.IsSecondReadingCycleNull OrElse pCustomerTestRow.SecondReadingCycle <> pFactoryTestRow.SecondReadingCycle) Then
                        If (Not pCustomerTestRow.IsSecondReadingCycleNull) Then mySndReading = pCustomerTestRow.SecondReadingCycle.ToString

                        'Add a row for field SecondReadingCycle in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "SecondReadingCycle", mySndReading, _
                                                            pFactoryTestRow.SecondReadingCycle.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestRow.SecondReadingCycle = pFactoryTestRow.SecondReadingCycle
                        deleteBlkCalibResults = True
                    End If
                Else
                    If (Not pCustomerTestRow.IsSecondReadingCycleNull) Then
                        'Add a row for field SecondReadingCycle in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "SecondReadingCycle", pCustomerTestRow.SecondReadingCycle.ToString, _
                                                            mySndReading)

                        'Update field in the Customer DataSet
                        pCustomerTestRow.SetSecondReadingCycleNull()
                        deleteBlkCalibResults = True
                    End If
                End If

                'Verify if field ReferenceWaveLength has changed
                Dim myReferenceWL As String = "--"
                If (Not pFactoryTestRow.IsReferenceWavelengthNull) Then
                    If (pCustomerTestRow.IsReferenceWavelengthNull OrElse pCustomerTestRow.ReferenceWavelength <> pFactoryTestRow.ReferenceWavelength) Then
                        If (Not pCustomerTestRow.IsReferenceWavelengthNull) Then myReferenceWL = pCustomerTestRow.ReferenceWavelength.ToString

                        'Add a row for field ReferenceWaveLength in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "ReferenceWaveLength", myReferenceWL, _
                                                            pFactoryTestRow.ReferenceWavelength.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestRow.ReferenceWavelength = pFactoryTestRow.ReferenceWavelength
                        deleteBlkCalibResults = True
                    End If
                Else
                    If (Not pCustomerTestRow.IsReferenceWavelengthNull) Then
                        'Add a row for field ReferenceWaveLength in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "ReferenceWaveLength", pCustomerTestRow.ReferenceWavelength.ToString, _
                                                            myReferenceWL)

                        'Update field in the Customer DataSet
                        pCustomerTestRow.SetReferenceWavelengthNull()
                        deleteBlkCalibResults = True
                    End If
                End If

                'Verify if field KineticBlankLimit has changed
                Dim myKineticBlkLimit As String = "--"
                If (Not pFactoryTestRow.IsKineticBlankLimitNull) Then
                    If (Not pCustomerTestRow.IsKineticBlankLimitNull) Then myKineticBlkLimit = pCustomerTestRow.KineticBlankLimit.ToString

                    'Add a row for field KineticBlankLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "KineticBlankLimit", myKineticBlkLimit, _
                                                        pFactoryTestRow.KineticBlankLimit.ToString)

                    'Update field in the Customer DataSet
                    pCustomerTestRow.KineticBlankLimit = pFactoryTestRow.KineticBlankLimit
                Else
                    If (Not pCustomerTestRow.IsKineticBlankLimitNull) Then
                        'Add a row for field KineticBlankLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, String.Empty, "KineticBlankLimit", pCustomerTestRow.KineticBlankLimit.ToString, _
                                                            myKineticBlkLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestRow.SetKineticBlankLimitNull()
                    End If
                End If
                pCustomerTestRow.EndEdit()
                pCustomerTestRow.AcceptChanges()

                'Return the Boolean value indicating if previous results of Blanks and Calibrators for the STD Test have to be deleted 
                myGlobalDataTO.SetDatos = deleteBlkCalibResults
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateCustomerTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a STD Test, compare values in tables tparTestReagents in CUSTOMER DB with corresponding values in FACTORY DB, and execute following
        ''' actions: 
        ''' ** Update field CodeTest for Reagents linked to the STD Test in both DBs
        ''' ** Add to CUSTOMER DB new Reagents linked to the STD Test in FACTORY DB
        ''' ** Remove from CUSTOMER DB Reagents that do not exist in FACTORY DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pTestName">Name of the STD Test in CUSTOMER DB</param>
        ''' <param name="pTestSamplesDS">TestsDS containing all SampleTypes linked to the STD Test</param>
        ''' <param name="pReagentsDS">ReagentsDS to return basic data of all Reagents linked to the STD Test</param>
        ''' <param name="pTestReagentsDS">TestReagentsDS to return the relation between the STD Test and all its linked Reagents</param>
        ''' <param name="pDeletedTestReagentsVols">List of DeletedTestReagentsVolumeTO to return if a Reagent linked to the STD Test has to be removed;
        '''                                        in this case, a TO for each SampleType linked to the STD Test will be added to the list </param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1983)
        ''' </remarks>
        Private Function UpdateCustomerTestReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pTestName As String, _
                                                    ByVal pTestSamplesDS As TestsDS, ByRef pReagentsDS As ReagentsDS, ByRef pTestReagentsDS As TestReagentsDS, _
                                                    ByRef pDeletedTestReagentsVols As List(Of DeletedTestReagentsVolumeTO)) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim factoryReagentsDS As New ReagentsDS
                Dim factoryReagentsNumber As Integer = 0
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                Dim customerReagentsNumber As Integer = 0
                Dim customerReagentsDS As New TestReagentsDS
                Dim myTestReagentsDelegate As New TestReagentsDelegate

                Dim myTestReagentsRow As TestReagentsDS.tparTestReagentsRow
                Dim myDelTestReagentsVolsTO As New DeletedTestReagentsVolumeTO
                Dim myCustomerReagentsList As List(Of TestReagentsDS.tparTestReagentsRow)

                '(1) Get from FACTORY DB data of all Reagents linked to the STD Tests
                myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsByTestID(pDBConnection, pTestID, True)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    factoryReagentsDS = DirectCast(myGlobalDataTO.SetDatos, ReagentsDS)

                    'Get the Number of Reagents linked to the STD Test in FACTORY DB
                    factoryReagentsNumber = factoryReagentsDS.tparReagents.Rows.Count
                End If

                '(2) Get from CUSTOMER DB data of all Reagents linked to the STD Tests
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, pTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        customerReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)

                        'Get the Number of Reagents linked to the STD Test in CUSTOMER DB
                        customerReagentsNumber = customerReagentsDS.tparTestReagents.Rows.Count
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    '(3) Compare Reagents linked to the STD Test in both DBs, CUSTOMER AND FACTORY:
                    '    If (Number of Reagents in FACTORY DB < Number of Reagents in CUSTOMER DB) Then
                    '       ==> Some Reagents have to be deleted from CUSTOMER DB
                    '    If (Number of Reagents in FACTORY DB > Number of Reagents in CUSTOMER DB) Then 
                    '       ==> Some Reagents have to be added to CUSTOMER DB
                    '    If (Number of Reagents in FACTORY DB = Number of Reagents in CUSTOMER DB) Then 
                    '       ==> Update value of field CodeTest in CUSTOMER DB if it has been changed in FACTORY DB
                    For Each factoryReagentRow As ReagentsDS.tparReagentsRow In factoryReagentsDS.tparReagents
                        If (factoryReagentRow.ReagentNumber <= customerReagentsNumber) Then
                            '(3.1) Update REAGENTS that exist in FACTORY and CUSTOMER DBs

                            'Search the Reagent data in CUSTOMER DB to update fields with data from FACTORY DB; then move the row to the ReagentsDS to return
                            myCustomerReagentsList = (From a As TestReagentsDS.tparTestReagentsRow In customerReagentsDS.tparTestReagents _
                                                     Where a.ReagentNumber = factoryReagentRow.ReagentNumber _
                                                    Select a).ToList

                            If (Not factoryReagentRow.IsCodeTestNull) Then
                                myCustomerReagentsList.First.CodeTest = factoryReagentRow.CodeTest
                            Else
                                myCustomerReagentsList.First.SetCodeTestNull()
                            End If
                            pReagentsDS.tparReagents.ImportRow(myCustomerReagentsList.First)

                            'Add also a row to the TestReagentsDS to return
                            myTestReagentsRow = pTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                            myTestReagentsRow.TestID = pTestID
                            myTestReagentsRow.ReagentID = myCustomerReagentsList.First.ReagentID
                            myTestReagentsRow.ReagentNumber = factoryReagentRow.ReagentNumber
                            pTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(myTestReagentsRow)
                        Else
                            '(3.2) Add new REAGENTS (those that exists only in FACTORY DB)

                            'Build the ReagentName using the TestName in CUSTOMER BD and the ReagentNumber and mark it as new Reagent
                            factoryReagentRow.ReagentName = pTestName.Trim & "-" & factoryReagentRow.ReagentNumber.ToString
                            factoryReagentRow.IsNew = True

                            'Move the row with the new Reagent to the ReagentsDS to return
                            pReagentsDS.tparReagents.ImportRow(factoryReagentRow)

                            'Add a new row to the TestReagentsDS to return
                            myTestReagentsRow = pTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                            myTestReagentsRow.TestID = pTestID
                            myTestReagentsRow.ReagentID = factoryReagentRow.ReagentID
                            myTestReagentsRow.ReagentNumber = factoryReagentRow.ReagentNumber
                            myTestReagentsRow.IsNew = True
                            pTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(myTestReagentsRow)

                            'Set the ReagentID for the new Reagent to add in Customer DB
                            myGlobalDataTO = SetTempReagentID(pDBConnection, pReagentsDS, pTestReagentsDS)

                            'If an error has been raised, then the process is finished
                            If (myGlobalDataTO.HasError) Then Exit For
                        End If
                    Next
                    pReagentsDS.AcceptChanges()
                    pTestReagentsDS.AcceptChanges()
                End If

                '(3.3) Check if there is a Reagent that has to be deleted from CUSTOMER DB (due to it does not exist in FACTORY DB)
                If (Not myGlobalDataTO.HasError) Then
                    myCustomerReagentsList = (From a As TestReagentsDS.tparTestReagentsRow In customerReagentsDS.tparTestReagents _
                                             Where a.ReagentNumber > factoryReagentsNumber _
                                            Select a).ToList

                    If (myCustomerReagentsList.Count > 0) Then
                        'Add the Reagent to delete (with all Sample Types linked to the STD Test) to the list of objects to delete
                        For Each sampleTypeRow As TestsDS.tparTestsRow In pTestSamplesDS.tparTests
                            myDelTestReagentsVolsTO.TestID = pTestID
                            myDelTestReagentsVolsTO.SampleType = sampleTypeRow.SampleType
                            myDelTestReagentsVolsTO.ReagentID = myCustomerReagentsList.First.ReagentID
                            myDelTestReagentsVolsTO.ReagentNumber = myCustomerReagentsList.First.ReagentNumber

                            pDeletedTestReagentsVols.Add(myDelTestReagentsVolsTO)
                        Next
                    End If
                    myCustomerReagentsList = Nothing
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateCustomerTestReagents", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all STD TESTS that exists in CUSTOMER DB but for which at least one of the relevant Test fields 
        ''' have changed and modify data in CUSTOMER DB (tables tparTests, tparReagents and tparTestReagents; additionally, depending on the modified 
        ''' fields, previous results of Blanks and Calibrators for the STD Test can be deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA-1983)
        ''' </remarks>
        Public Function UPDATEModifiedSTDTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myUpdatedTestsDS As New TestsDS
                Dim myCustomerTestDS As New TestsDS
                Dim myTestSamplesDS As New TestsDS

                Dim myTestsDelegate As New TestsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
                Dim myDeletedTestReagentsVolsList As New List(Of DeletedTestReagentsVolumeTO)

                '(1) Search in Factory DB all STD TESTS that exists in Customer DB but for which at least one of the
                '    relevant Test fields have changed
                myGlobalDataTO = myTestParametersUpdateDAO.GetUpdatedFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myUpdatedTestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                    Dim myReagentsDS As New ReagentsDS
                    Dim myTestReagentsDS As New TestReagentsDS
                    Dim deleteBlankCalibResults As Boolean = False

                    '(2) Process each returned STD Test to update it in CUSTOMER DB
                    For Each updatedTest As TestsDS.tparTestsRow In myUpdatedTestsDS.tparTests
                        '(2.1) Get data of the Test in Customer DB
                        myGlobalDataTO = myTestsDelegate.Read(pDBConnection, updatedTest.TestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myCustomerTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                        End If

                        '(2.2) Get all different SampleTypes for the STD Test in Customer DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestsDelegate.ReadByTestIDSampleType(pDBConnection, updatedTest.TestID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                            End If
                        End If

                        '(2.3) Verify changed fields and update values in myCustomerTestDS
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateCustomerTest(updatedTest, myCustomerTestDS.tparTests.First, pUpdateVersionChangesList)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                deleteBlankCalibResults = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                            End If
                        End If

                        '(2.4) Verify if there are changes in the Reagents used for the STD Test
                        If (Not myGlobalDataTO.HasError) Then
                            myReagentsDS.Clear()
                            myTestReagentsDS.Clear()
                            myDeletedTestReagentsVolsList.Clear()

                            myGlobalDataTO = UpdateCustomerTestReagents(pDBConnection, updatedTest.TestID, myCustomerTestDS.tparTests.First.TestName, _
                                                                        myTestSamplesDS, myReagentsDS, myTestReagentsDS, myDeletedTestReagentsVolsList)
                        End If

                        '(2.5) If previous Results of Blanks and Calibrators for the STD Tests have to be deleted, prepare the needed TO
                        If (Not myGlobalDataTO.HasError) Then
                            If (deleteBlankCalibResults) Then
                                'Add the TestID and TestVersionNumber to the TO of Tests to delete, and mark DeleteBlankCalibResults = TRUE
                                myDeletedTestProgramingTO.TestID = myCustomerTestDS.tparTests.First.TestID
                                myDeletedTestProgramingTO.TestVersion = myCustomerTestDS.tparTests.First.TestVersionNumber
                                myDeletedTestProgramingTO.DeleteBlankCalibResults = True
                                myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                            End If
                        End If

                        '(2.6) Update the STD Test from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, myCustomerTestDS, New TestSamplesDS, New TestReagentsVolumesDS, _
                                                                               myReagentsDS, myTestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                               New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                               myDeletedTestReagentsVolsList, myDeletedTestProgramingList, _
                                                                               New TestSamplesMultirulesDS, New TestControlsDS, Nothing, String.Empty, False)
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

                myDeletedTestProgramingTO = Nothing
                myDeletedTestProgramingList = Nothing
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UPDATEModifiedSTDTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all pairs of STD TestID/SampleType in FACTORY DB that do not exist in CUSTOMER DB and, for each one of them, execute the process 
        ''' of adding it to CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection></param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 09/10/2014 - BA-1944 (SubTask BA-1981)
        ''' </remarks>
        Public Function CREATENewSamplesForSTDTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myTestDS As New TestsDS
                Dim myTestReagentsDS As New TestReagentsDS
                Dim myNewTestSamplesList As New TestsDS
                Dim myTestStructure As New TestStructure
                Dim myTestsDelegate As New TestsDelegate
                Dim myTestReagentsDelegate As New TestReagentsDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new pairs of STD TestID/SampleType
                myGlobalDataTO = myTestParametersUpdateDAO.GetNewFactoryTestSamples(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewTestSamplesList = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                End If

                '(2) Get the list of different TestIDs (to manage the case of several SampleTypes added to one STD Test)
                Dim myTestsList As List(Of Integer) = (From a In myNewTestSamplesList.tparTests _
                                                     Select a.TestID Distinct).ToList()

                For Each myTestID As Integer In myTestsList
                    '(2.1) Get all data in table tparTests in CUSTOMER DB for the TestID 
                    myGlobalDataTO = myTestsDelegate.Read(pDBConnection, myTestID)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                    End If

                    '(2.2) Get all Reagents linked to the STD Test in CUSTOMER DB
                    If (Not myGlobalDataTO.HasError) Then
                        myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(pDBConnection, myTestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myTestReagentsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsDS)
                        End If
                    End If

                    '(2.3) Process each new SampleType for the STD Test in Factory DB to add it to Customer DB
                    If (Not myGlobalDataTO.HasError) Then
                        For Each newTestSample As TestsDS.tparTestsRow In myNewTestSamplesList.tparTests.ToList.Where(Function(a) a.TestID = myTestID).ToList
                            '(2.3.1) Get from FACTORY DB, all data for the new Sample Type
                            myGlobalDataTO = GetOtherTestDataInfoFromFactory(pDBConnection, myTestID, newTestSample.SampleType)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myTestStructure = DirectCast(myGlobalDataTO.SetDatos, TestStructure)
                            End If

                            '(2.3.2) Generate the temporary TestCalibratorIDs and inform them in all Test Calibrators related DataSets
                            If (Not myGlobalDataTO.HasError) Then
                                If (myTestStructure.myTestCalibratorDS.tparTestCalibrators.Rows.Count > 0) Then
                                    myGlobalDataTO = SetTempTestCalibratorID(pDBConnection, myTestStructure.myTestCalibratorDS, myTestStructure.myTestCalibratorValuesDS)
                                End If
                            End If

                            '(2.3.3) Fill the ReagentsDS before call the function to save the STD Test / SampleType 
                            Dim myReagentRow As ReagentsDS.tparReagentsRow
                            myTestStructure.myReagentsDS = New ReagentsDS
                            For Each testReagentRow As TestReagentsDS.tparTestReagentsRow In myTestReagentsDS.tparTestReagents
                                myReagentRow = myTestStructure.myReagentsDS.tparReagents.NewtparReagentsRow()
                                myReagentRow.ReagentID = testReagentRow.ReagentID
                                myReagentRow.ReagentName = testReagentRow.ReagentName
                                myReagentRow.ReagentNumber = testReagentRow.ReagentNumber
                                myReagentRow.CodeTest = testReagentRow.CodeTest
                                myTestStructure.myReagentsDS.tparReagents.AddtparReagentsRow(myReagentRow)
                            Next
                            myTestStructure.myReagentsDS.AcceptChanges()

                            '(2.3.4) Save the NEW SampleType for the STD Test in CUSTOMER DB 
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, myTestDS, _
                                                                                   myTestStructure.myTestSampleDS, myTestStructure.myTestReagentsVolumesDS, _
                                                                                   myTestStructure.myReagentsDS, myTestReagentsDS, _
                                                                                   myTestStructure.myCalibratorDS, myTestStructure.myTestCalibratorDS, _
                                                                                   myTestStructure.myTestCalibratorValuesDS, New TestRefRangesDS, _
                                                                                   New List(Of DeletedCalibratorTO), New List(Of DeletedTestReagentsVolumeTO), _
                                                                                   New List(Of DeletedTestProgramingTO), New TestSamplesMultirulesDS, _
                                                                                   New TestControlsDS, Nothing, String.Empty, False)
                            End If

                            '(2.3.4) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                            '        for the Sample Type added for the new Factory STD Test
                            If (Not myGlobalDataTO.HasError) Then
                                myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                                myUpdateVersionAddedElementsRow.ElementType = "STD"
                                myUpdateVersionAddedElementsRow.ElementName = myTestDS.tparTests.First.TestName & " (" & myTestDS.tparTests.First.ShortName & ")"
                                myUpdateVersionAddedElementsRow.SampleType = newTestSample.SampleType
                                pUpdateVersionChangesList.AddedElements.AddAddedElementsRow(myUpdateVersionAddedElementsRow)
                                pUpdateVersionChangesList.AddedElements.AcceptChanges()
                            End If

                            'If an error has been raised, then the process is finished
                            If (myGlobalDataTO.HasError) Then Exit For
                        Next
                    End If

                    'If an error has been raised, then the process is finished
                    If (myGlobalDataTO.HasError) Then Exit For
                Next
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.CREATENewSamplesForSTDTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a STD Test/SampleType, compare value of relevant fields in table tparTestSamples in CUSTOMER DB with value of the same fields 
        ''' in FACTORY DB, and update in CUSTOMER DB all modified fields.
        ''' </summary>
        ''' <param name="pFactoryTestSampleRow">Row of TestSamplesDS containing the Test/SampleType data in FACTORY DB</param>
        ''' <param name="pCustomerTestSampleRow">Row of TestSamplesDS containing the Test/SampleType data in CUSTOMER DB</param>
        ''' <param name="pTestName">Name of the STD Test</param>
        ''' <param name="pTestShortName">Shortname of the STD Test</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value that indicates (when TRUE) that previous Blank and Calibrator Results have to be deleted due to
        '''          field SampleVolume has changed</returns>
        ''' <remarks>
        ''' Created by: SA 10/10/2014 - BA-1944 (SubTask BA-1985)
        ''' </remarks>
        Private Function UpdateCustomerTestSamples(ByVal pFactoryTestSampleRow As TestSamplesDS.tparTestSamplesRow, _
                                                   ByVal pCustomerTestSampleRow As TestSamplesDS.tparTestSamplesRow, _
                                                   ByVal pTestName As String, ByVal pTestShortName As String, _
                                                   ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim deleteBlkCalibResults As Boolean = False
                Dim myTestName As String = pTestName & " (" & pTestShortName & ")"
                Dim mySampleType As String = pCustomerTestSampleRow.SampleType

                'Verify if field SampleVolume has changed
                pCustomerTestSampleRow.BeginEdit()
                If (pCustomerTestSampleRow.SampleVolume <> pFactoryTestSampleRow.SampleVolume) Then
                    'Add a row for field SampleVolume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SampleVolume", pCustomerTestSampleRow.SampleVolume.ToString, _
                                                        pFactoryTestSampleRow.SampleVolume.ToString)

                    'Update fields in the Customer DataSet ==> Field SampleVolumeSteps is also affected
                    pCustomerTestSampleRow.SampleVolume = pFactoryTestSampleRow.SampleVolume
                    pCustomerTestSampleRow.SampleVolumeSteps = pFactoryTestSampleRow.SampleVolumeSteps
                    deleteBlkCalibResults = True
                End If

                'Verify if field PredilutionUseFlag has changed
                If (pCustomerTestSampleRow.PredilutionUseFlag <> pFactoryTestSampleRow.PredilutionUseFlag) Then
                    'Add a row for field PredilutionUseFlag in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "PredilutionUseFlag", pCustomerTestSampleRow.PredilutionUseFlag.ToString, _
                                                        pFactoryTestSampleRow.PredilutionUseFlag.ToString)

                    'Update field in the Customer DataSet
                    pCustomerTestSampleRow.PredilutionUseFlag = pFactoryTestSampleRow.PredilutionUseFlag
                End If

                'Verify if field PredilutionMode has changed
                Dim myPredilutionMode As String = "--"
                If (Not pFactoryTestSampleRow.IsPredilutionModeNull) Then
                    If (Not pCustomerTestSampleRow.IsPredilutionModeNull AndAlso _
                        pCustomerTestSampleRow.PredilutionMode <> String.Empty) Then myPredilutionMode = pCustomerTestSampleRow.PredilutionMode

                    If (pCustomerTestSampleRow.IsPredilutionModeNull OrElse pCustomerTestSampleRow.PredilutionMode <> pFactoryTestSampleRow.PredilutionMode) Then
                        'Add a row for field PredilutionMode in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "PredilutionMode", myPredilutionMode, pFactoryTestSampleRow.PredilutionMode)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.PredilutionMode = pFactoryTestSampleRow.PredilutionMode
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsPredilutionModeNull) Then
                        'Add a row for field PredilutionMode in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "PredilutionMode", pCustomerTestSampleRow.PredilutionMode, myPredilutionMode)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetPredilutionModeNull()
                    End If
                End If

                'Verify if field PredilutionFactor has changed
                Dim myPredilutionFactor As String = "--"
                If (Not pFactoryTestSampleRow.IsPredilutionFactorNull) Then
                    If (Not pCustomerTestSampleRow.IsPredilutionFactorNull) Then myPredilutionFactor = pCustomerTestSampleRow.PredilutionFactor.ToString

                    If (pCustomerTestSampleRow.IsPredilutionFactorNull OrElse pCustomerTestSampleRow.PredilutionFactor <> pFactoryTestSampleRow.PredilutionFactor) Then
                        'Add a row for field PredilutionFactor in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "PredilutionFactor", myPredilutionFactor, _
                                                            pFactoryTestSampleRow.PredilutionFactor.ToString)

                        'Update fields in the Customer DataSet ==> Fields PredilutedSampleVol, PredilutedSampleVolSteps, PredilutedDiluentVol and PreDiluentVolSteps are also affected
                        pCustomerTestSampleRow.PredilutionFactor = pFactoryTestSampleRow.PredilutionFactor
                        pCustomerTestSampleRow.PredilutedSampleVol = pFactoryTestSampleRow.PredilutedSampleVol
                        pCustomerTestSampleRow.PredilutedSampleVolSteps = pFactoryTestSampleRow.PredilutedSampleVolSteps
                        pCustomerTestSampleRow.PredilutedDiluentVol = pFactoryTestSampleRow.PredilutedDiluentVol
                        pCustomerTestSampleRow.PreDiluentVolSteps = pFactoryTestSampleRow.PreDiluentVolSteps
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsPredilutionFactorNull) Then
                        'Add a row for field PredilutionFactor in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "PredilutionFactor", pCustomerTestSampleRow.PredilutionFactor.ToString, _
                                                            myPredilutionMode)

                        'Update fields in the Customer DataSet ==> Fields PredilutedSampleVol, PredilutedSampleVolSteps, PredilutedDiluentVol and PreDiluentVolSteps are also affected
                        pCustomerTestSampleRow.SetPredilutionFactorNull()
                        pCustomerTestSampleRow.SetPredilutedSampleVolNull()
                        pCustomerTestSampleRow.SetPredilutedSampleVolStepsNull()
                        pCustomerTestSampleRow.SetPredilutedDiluentVolNull()
                        pCustomerTestSampleRow.SetPreDiluentVolStepsNull()
                    End If
                End If

                'Verify if field DiluentSolution has changed
                Dim myDiluentSolution As String = "--"
                If (Not pFactoryTestSampleRow.IsDiluentSolutionNull) Then
                    If (Not pCustomerTestSampleRow.IsDiluentSolutionNull AndAlso _
                        pCustomerTestSampleRow.DiluentSolution <> String.Empty) Then myDiluentSolution = pCustomerTestSampleRow.DiluentSolution

                    If (pCustomerTestSampleRow.IsDiluentSolutionNull OrElse pCustomerTestSampleRow.DiluentSolution <> pFactoryTestSampleRow.DiluentSolution) Then
                        'Add a row for field DiluentSolution in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "DiluentSolution", myDiluentSolution, pFactoryTestSampleRow.DiluentSolution)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.DiluentSolution = pFactoryTestSampleRow.DiluentSolution
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsDiluentSolutionNull) Then
                        'Add a row for field DiluentSolution in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "DiluentSolution", pCustomerTestSampleRow.DiluentSolution, myDiluentSolution)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetDiluentSolutionNull()
                    End If
                End If

                'Verify if field IncPostdilutionFactor has changed
                Dim myIncPostDilutionFactor As String = "--"
                If (Not pFactoryTestSampleRow.IsIncPostdilutionFactorNull) Then
                    If (Not pCustomerTestSampleRow.IsIncPostdilutionFactorNull) Then myIncPostDilutionFactor = pCustomerTestSampleRow.IncPostdilutionFactor.ToString

                    If (pCustomerTestSampleRow.IsIncPostdilutionFactorNull OrElse pCustomerTestSampleRow.IncPostdilutionFactor <> pFactoryTestSampleRow.IncPostdilutionFactor) Then
                        'Add a row for field IncPostdilutionFactor in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "IncPostdilutionFactor", myIncPostDilutionFactor, _
                                                            pFactoryTestSampleRow.IncPostdilutionFactor.ToString)

                        'Update fields in the Customer DataSet ==> Fields IncPostSampleVolume are IncPostSampleVolumeSteps are also affected
                        pCustomerTestSampleRow.IncPostdilutionFactor = pFactoryTestSampleRow.IncPostdilutionFactor
                        pCustomerTestSampleRow.IncPostSampleVolume = pFactoryTestSampleRow.IncPostSampleVolume
                        pCustomerTestSampleRow.IncPostSampleVolumeSteps = pFactoryTestSampleRow.IncPostSampleVolumeSteps
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsIncPostdilutionFactorNull) Then
                        'Add a row for field IncPostdilutionFactor in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "IncPostdilutionFactor", pCustomerTestSampleRow.IncPostdilutionFactor.ToString, _
                                                            myIncPostDilutionFactor)

                        'Update fields in the Customer DataSet ==> Fields IncPostSampleVolume are IncPostSampleVolumeSteps are also affected
                        pCustomerTestSampleRow.SetIncPostdilutionFactorNull()
                        pCustomerTestSampleRow.SetIncPostSampleVolumeNull()
                        pCustomerTestSampleRow.SetIncPostSampleVolumeStepsNull()
                    End If
                End If

                'Verify if field RedPostdilutionFactor has changed
                Dim myRedPostDilutionFactor As String = "--"
                If (Not pFactoryTestSampleRow.IsRedPostdilutionFactorNull) Then
                    If (Not pCustomerTestSampleRow.IsRedPostdilutionFactorNull) Then myRedPostDilutionFactor = pCustomerTestSampleRow.RedPostdilutionFactor.ToString

                    If (pCustomerTestSampleRow.IsRedPostdilutionFactorNull OrElse pCustomerTestSampleRow.RedPostdilutionFactor <> pFactoryTestSampleRow.RedPostdilutionFactor) Then
                        'Add a row for field RedPostdilutionFactor in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "RedPostdilutionFactor", myRedPostDilutionFactor, _
                                                            pFactoryTestSampleRow.RedPostdilutionFactor.ToString)

                        'Update fields in the Customer DataSet ==> Fields RedPostSampleVolume are RedPostSampleVolumeSteps are also affected
                        pCustomerTestSampleRow.RedPostdilutionFactor = pFactoryTestSampleRow.RedPostdilutionFactor
                        pCustomerTestSampleRow.RedPostSampleVolume = pFactoryTestSampleRow.RedPostSampleVolume
                        pCustomerTestSampleRow.RedPostSampleVolumeSteps = pFactoryTestSampleRow.RedPostSampleVolumeSteps
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsRedPostdilutionFactorNull) Then
                        'Add a row for field RedPostdilutionFactor in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "RedPostdilutionFactor", pCustomerTestSampleRow.RedPostdilutionFactor.ToString, _
                                                            myRedPostDilutionFactor)

                        'Update fields in the Customer DataSet ==> Fields RedPostSampleVolume are RedPostSampleVolumeSteps are also affected
                        pCustomerTestSampleRow.SetRedPostdilutionFactorNull()
                        pCustomerTestSampleRow.SetRedPostSampleVolumeNull()
                        pCustomerTestSampleRow.SetRedPostSampleVolumeStepsNull()
                    End If
                End If

                'Verify if field BlankAbsorbanceLimit has changed
                Dim myBlkAbsLimit As String = "--"
                If (Not pFactoryTestSampleRow.IsBlankAbsorbanceLimitNull) Then
                    If (Not pCustomerTestSampleRow.IsBlankAbsorbanceLimitNull) Then myBlkAbsLimit = pCustomerTestSampleRow.BlankAbsorbanceLimit.ToString

                    If (pCustomerTestSampleRow.IsBlankAbsorbanceLimitNull OrElse pCustomerTestSampleRow.BlankAbsorbanceLimit <> pFactoryTestSampleRow.BlankAbsorbanceLimit) Then
                        'Add a row for field BlankAbsorbanceLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "BlankAbsorbanceLimit", myBlkAbsLimit, _
                                                            pFactoryTestSampleRow.BlankAbsorbanceLimit.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.BlankAbsorbanceLimit = pFactoryTestSampleRow.BlankAbsorbanceLimit
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsBlankAbsorbanceLimitNull) Then
                        'Add a row for field BlankAbsorbanceLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "BlankAbsorbanceLimit", pCustomerTestSampleRow.BlankAbsorbanceLimit.ToString, _
                                                            myBlkAbsLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetBlankAbsorbanceLimitNull()
                    End If
                End If

                'Verify if field LinearityLimit has changed
                Dim myLinearityLimit As String = "--"
                If (Not pFactoryTestSampleRow.IsLinearityLimitNull) Then
                    If (Not pCustomerTestSampleRow.IsLinearityLimitNull) Then myLinearityLimit = pCustomerTestSampleRow.LinearityLimit.ToString

                    If (pCustomerTestSampleRow.IsLinearityLimitNull OrElse pCustomerTestSampleRow.LinearityLimit <> pFactoryTestSampleRow.LinearityLimit) Then
                        'Add a row for field LinearityLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "LinearityLimit", myLinearityLimit, _
                                                            pFactoryTestSampleRow.LinearityLimit.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.LinearityLimit = pFactoryTestSampleRow.LinearityLimit
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsLinearityLimitNull) Then
                        'Add a row for field LinearityLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "LinearityLimit", pCustomerTestSampleRow.LinearityLimit.ToString, _
                                                            myLinearityLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetLinearityLimitNull()
                    End If
                End If

                'Verify if field DetectionLimit has changed
                Dim myDetectionLimit As String = "--"
                If (Not pFactoryTestSampleRow.IsDetectionLimitNull) Then
                    If (Not pCustomerTestSampleRow.IsDetectionLimitNull) Then myDetectionLimit = pCustomerTestSampleRow.DetectionLimit.ToString

                    If (pCustomerTestSampleRow.IsDetectionLimitNull OrElse pCustomerTestSampleRow.DetectionLimit <> pFactoryTestSampleRow.DetectionLimit) Then
                        'Add a row for field DetectionLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "DetectionLimit", myDetectionLimit, _
                                                            pFactoryTestSampleRow.DetectionLimit.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.DetectionLimit = pFactoryTestSampleRow.DetectionLimit
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsDetectionLimitNull) Then
                        'Add a row for field DetectionLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "DetectionLimit", pCustomerTestSampleRow.DetectionLimit.ToString, _
                                                            myDetectionLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetDetectionLimitNull()
                    End If
                End If

                'Verify if field FactorLowerLimit has changed
                Dim myFactorLowerLimit As String = "--"
                If (Not pFactoryTestSampleRow.IsFactorLowerLimitNull) Then
                    If (Not pCustomerTestSampleRow.IsFactorLowerLimitNull) Then myFactorLowerLimit = pCustomerTestSampleRow.FactorLowerLimit.ToString

                    If (pCustomerTestSampleRow.IsFactorLowerLimitNull OrElse pCustomerTestSampleRow.FactorLowerLimit <> pFactoryTestSampleRow.FactorLowerLimit) Then
                        'Add a row for field FactorLowerLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "FactorLowerLimit", myFactorLowerLimit, _
                                                            pFactoryTestSampleRow.FactorLowerLimit.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.FactorLowerLimit = pFactoryTestSampleRow.FactorLowerLimit
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsFactorLowerLimitNull) Then
                        'Add a row for field FactorLowerLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "FactorLowerLimit", pCustomerTestSampleRow.FactorLowerLimit.ToString, _
                                                            myFactorLowerLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetFactorLowerLimitNull()
                    End If
                End If

                'Verify if field FactorUpperLimit has changed
                Dim myFactorUpperLimit As String = "--"
                If (Not pFactoryTestSampleRow.IsFactorUpperLimitNull) Then
                    If (Not pCustomerTestSampleRow.IsFactorUpperLimitNull) Then myFactorUpperLimit = pCustomerTestSampleRow.FactorUpperLimit.ToString

                    If (pCustomerTestSampleRow.IsFactorUpperLimitNull OrElse pCustomerTestSampleRow.FactorUpperLimit <> pFactoryTestSampleRow.FactorUpperLimit) Then
                        'Add a row for field FactorUpperLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "FactorUpperLimit", myFactorUpperLimit, _
                                                            pFactoryTestSampleRow.FactorUpperLimit.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.FactorUpperLimit = pFactoryTestSampleRow.FactorUpperLimit
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsFactorUpperLimitNull) Then
                        'Add a row for field FactorLowerLimit in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "FactorUpperLimit", pCustomerTestSampleRow.FactorUpperLimit.ToString, _
                                                            myFactorUpperLimit)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetFactorUpperLimitNull()
                    End If
                End If

                'Verify if field SlopeFactorA has changed
                Dim mySlopeA As String = "--"
                If (Not pFactoryTestSampleRow.IsSlopeFactorANull) Then
                    If (Not pCustomerTestSampleRow.IsSlopeFactorANull) Then mySlopeA = pCustomerTestSampleRow.SlopeFactorA.ToString

                    If (pCustomerTestSampleRow.IsSlopeFactorANull OrElse pCustomerTestSampleRow.SlopeFactorA <> pFactoryTestSampleRow.SlopeFactorA) Then
                        'Add a row for field SlopeFactorA in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SlopeFactorA", mySlopeA, _
                                                            pFactoryTestSampleRow.SlopeFactorA.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SlopeFactorA = pFactoryTestSampleRow.SlopeFactorA
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsSlopeFactorANull) Then
                        'Add a row for field SlopeFactorA in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SlopeFactorA", pCustomerTestSampleRow.SlopeFactorA.ToString, _
                                                            mySlopeA)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetSlopeFactorANull()
                    End If
                End If

                'Verify if field SlopeFactorB has changed
                Dim mySlopeB As String = "--"
                If (Not pFactoryTestSampleRow.IsSlopeFactorBNull) Then
                    If (Not pCustomerTestSampleRow.IsSlopeFactorBNull) Then mySlopeB = pCustomerTestSampleRow.SlopeFactorB.ToString

                    If (pCustomerTestSampleRow.IsSlopeFactorBNull OrElse pCustomerTestSampleRow.SlopeFactorB <> pFactoryTestSampleRow.SlopeFactorB) Then
                        'Add a row for field SlopeFactorB in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SlopeFactorB", mySlopeB, _
                                                            pFactoryTestSampleRow.SlopeFactorB.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SlopeFactorB = pFactoryTestSampleRow.SlopeFactorB
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsSlopeFactorBNull) Then
                        'Add a row for field SlopeFactorB in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SlopeFactorB", pCustomerTestSampleRow.SlopeFactorB.ToString, _
                                                            mySlopeB)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetSlopeFactorBNull()
                    End If
                End If

                'Verify if field SubstrateDepletionValue has changed
                Dim mySubstrateDep As String = "--"
                If (Not pFactoryTestSampleRow.IsSubstrateDepletionValueNull) Then
                    If (Not pCustomerTestSampleRow.IsSubstrateDepletionValueNull) Then mySubstrateDep = pCustomerTestSampleRow.SubstrateDepletionValue.ToString

                    If (pCustomerTestSampleRow.IsSubstrateDepletionValueNull OrElse pCustomerTestSampleRow.SubstrateDepletionValue <> pFactoryTestSampleRow.SubstrateDepletionValue) Then
                        'Add a row for field SubstrateDepletionValue in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SubstrateDepletionValue", mySubstrateDep, _
                                                            pFactoryTestSampleRow.SubstrateDepletionValue.ToString)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SubstrateDepletionValue = pFactoryTestSampleRow.SubstrateDepletionValue
                    End If
                Else
                    If (Not pCustomerTestSampleRow.IsSubstrateDepletionValueNull) Then
                        'Add a row for field SlopeFactorB in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SubstrateDepletionValue", pCustomerTestSampleRow.SubstrateDepletionValue.ToString, _
                                                            mySubstrateDep)

                        'Update field in the Customer DataSet
                        pCustomerTestSampleRow.SetSubstrateDepletionValueNull()
                    End If
                End If

                'Verify if Calibration data has to be updated
                If (pCustomerTestSampleRow.CalibratorType = "EXPERIMENT") Then
                    'Nothing to do --> Customer Calibration data is not updated
                Else
                    If (pFactoryTestSampleRow.CalibratorType = "FACTOR") Then
                        Dim myCalibFactor As String = "--"
                        If (Not pCustomerTestSampleRow.IsCalibrationFactorNull) Then myCalibFactor = pCustomerTestSampleRow.CalibrationFactor.ToString

                        If (pCustomerTestSampleRow.IsCalibrationFactorNull OrElse pCustomerTestSampleRow.CalibrationFactor <> pFactoryTestSampleRow.CalibrationFactor) Then
                            'Add a row for field CalibrationFactor in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "CalibrationFactor", myCalibFactor, _
                                                                pFactoryTestSampleRow.CalibrationFactor.ToString)
                        End If

                        If (pCustomerTestSampleRow.CalibratorType = "ALTERNATIV") Then
                            'Add a row for field CalibratorType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SampleTypeAlternative", pCustomerTestSampleRow.CalibratorType, _
                                                                pFactoryTestSampleRow.CalibratorType)

                            'Add a row for field SampleTypeAlternative in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SampleTypeAlternative", pCustomerTestSampleRow.SampleTypeAlternative, _
                                                                "--")
                        End If

                        'Update fields CalibratorType and CalibrationFactor; set to NULL field SampleTypeAlternative
                        pCustomerTestSampleRow.CalibratorType = pFactoryTestSampleRow.CalibratorType
                        pCustomerTestSampleRow.CalibrationFactor = pFactoryTestSampleRow.CalibrationFactor
                        pCustomerTestSampleRow.SetSampleTypeAlternativeNull()

                    ElseIf (pFactoryTestSampleRow.CalibratorType = "ALTERNATIV") Then
                        Dim mySTypeAlternative As String = "--"
                        If (Not pCustomerTestSampleRow.IsSampleTypeAlternativeNull AndAlso _
                            pCustomerTestSampleRow.SampleTypeAlternative <> String.Empty) Then mySTypeAlternative = pCustomerTestSampleRow.SampleTypeAlternative

                        If (pCustomerTestSampleRow.IsSampleTypeAlternativeNull OrElse pCustomerTestSampleRow.SampleTypeAlternative <> pFactoryTestSampleRow.SampleTypeAlternative) Then
                            'Add a row for field SampleTypeAlternative in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SampleTypeAlternative", mySTypeAlternative, _
                                                                pFactoryTestSampleRow.SampleTypeAlternative.ToString)
                        End If

                        If (pCustomerTestSampleRow.CalibratorType = "FACTOR") Then
                            'Add a row for field CalibratorType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "SampleTypeAlternative", pCustomerTestSampleRow.CalibratorType, _
                                                                pFactoryTestSampleRow.CalibratorType)

                            'Add a row for field CalibrationFactor in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "CalibrationFactor", pCustomerTestSampleRow.CalibrationFactor.ToString, _
                                                                "--")
                        End If

                        'Update fields CalibratorType and SampleTypeAlternative; set to NULL field CalibrationFactor
                        pCustomerTestSampleRow.CalibratorType = pFactoryTestSampleRow.CalibratorType
                        pCustomerTestSampleRow.SampleTypeAlternative = pFactoryTestSampleRow.SampleTypeAlternative
                        pCustomerTestSampleRow.SetCalibrationFactorNull()
                    Else
                        'Nothing to do --> The Calibrator was added to Customer DB, but it is not linked to the STD Test/SampleType
                    End If
                End If

                pCustomerTestSampleRow.EndEdit()
                pCustomerTestSampleRow.AcceptChanges()

                'Return the Boolean value indicating if previous results of Blanks and Calibrators for the STD Test have to be deleted 
                myGlobalDataTO.SetDatos = deleteBlkCalibResults
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateCustomerTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new row with data of an updated element in the DS containing all changes in CUSTOMER DB due to the Update Version Process.
        ''' Data is added in sub-table UpdatedElements
        ''' </summary>
        ''' <param name="pUpdateVersionChangesList">DataSet containing all changes made in CUSTOMER DB for the Update Version Process</param>
        ''' <param name="pElementType">Type of affected Element: CALIB, STD, ...</param>
        ''' <param name="pElementName">Name of the affected Element</param>
        ''' <param name="pUpdatedField">Name of the affected field</param>
        ''' <param name="pPreviousValue">Previous value of the field in CUSTOMER DB</param>
        ''' <param name="pNewValue">New value of the field in CUSTOMER DB (from FACTORY DB)</param>
        ''' <remarks>
        ''' Created by:  SA 10/10/2014 - BA-1944 (SubTasks BA-1984 and BA-1985)
        ''' </remarks>
        Private Sub AddUpdatedElementToChangesStructure(ByRef pUpdateVersionChangesList As UpdateVersionChangesDS, ByVal pElementType As String, _
                                                        ByVal pElementName As String, ByVal pSampleType As String, ByVal pUpdatedField As String, ByVal pPreviousValue As String, _
                                                        ByVal pNewValue As String)
            Try
                Dim myUpdateVersionChangedElementsRow As UpdateVersionChangesDS.UpdatedElementsRow
                myUpdateVersionChangedElementsRow = pUpdateVersionChangesList.UpdatedElements.NewUpdatedElementsRow
                myUpdateVersionChangedElementsRow.ElementType = pElementType
                myUpdateVersionChangedElementsRow.ElementName = pElementName
                myUpdateVersionChangedElementsRow.SampleType = pSampleType
                myUpdateVersionChangedElementsRow.UpdatedField = pUpdatedField
                myUpdateVersionChangedElementsRow.PreviousValue = pPreviousValue
                myUpdateVersionChangedElementsRow.NewValue = pNewValue
                pUpdateVersionChangesList.UpdatedElements.AddUpdatedElementsRow(myUpdateVersionChangedElementsRow)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.AddUpdatedElementToChangesStructure", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' For a STD Test/SampleType and a linked Reagent, compare value of relevant fields in table tparTestReagentsVolumes in CUSTOMER DB with value of 
        ''' the same fields in FACTORY DB, and update in CUSTOMER DB all modified fields.
        ''' </summary>
        ''' <param name="pFactoryReagentVolsRow">Row of TestReagentsVolumesDS containing Volumes for a modified ReagentNumer in FACTORY DB</param>
        ''' <param name="pCustomerReagentVolsRow">Row of TestReagentsVolumesDS containing current values of Volumes for a modified Reagent in CUSTOMER DB</param>
        ''' <param name="pTestName">Name of the STD Test</param>
        ''' <param name="pTestShortName">Shortname of the STD Test</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value that indicates (when TRUE) that previous Blank and Calibrator Results have to be deleted due to
        '''          field SampleVolume has changed</returns>
        ''' <remarks>
        ''' Created by:  SA 13/10/2014 - BA-1944 (SubTask BA-1985)
        ''' </remarks>
        Private Function UpdateCustomerReagentVolumes(ByVal pFactoryReagentVolsRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow, _
                                                      ByRef pCustomerReagentVolsRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow, _
                                                      ByVal pTestName As String, ByVal pTestShortName As String, _
                                                      ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim deleteBlkCalibResults As Boolean = False
                Dim myTestName As String = pTestName & " (" & pTestShortName & ")"
                Dim mySampleType As String = pCustomerReagentVolsRow.SampleType

                'Update values in the Row containing values for the ReagentNumber in CUSTOMER DB
                pCustomerReagentVolsRow.BeginEdit()

                If (pCustomerReagentVolsRow.ReagentVolume <> pFactoryReagentVolsRow.ReagentVolume) Then
                    'Add a row for field ReagentVolume in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "STD", myTestName, mySampleType, "ReagentVolume", pCustomerReagentVolsRow.ReagentVolume.ToString, _
                                                        pFactoryReagentVolsRow.ReagentVolume.ToString)

                    'Update fields ReagentVolume and ReagentVolumeSteps in the Customer DataSet
                    pCustomerReagentVolsRow.ReagentVolume = pFactoryReagentVolsRow.ReagentVolume
                    pCustomerReagentVolsRow.ReagentVolumeSteps = pFactoryReagentVolsRow.ReagentVolumeSteps

                    'Update fields IncPostReagentVolume and IncPostReagentVolumeSteps in the Customer DataSet
                    pCustomerReagentVolsRow.IncPostReagentVolume = pFactoryReagentVolsRow.IncPostReagentVolume
                    pCustomerReagentVolsRow.IncPostReagentVolumeSteps = pFactoryReagentVolsRow.IncPostReagentVolumeSteps

                    'Update fields RedPostReagentVolume and RedPostReagentVolumeSteps in the Customer DataSet
                    pCustomerReagentVolsRow.RedPostReagentVolume = pFactoryReagentVolsRow.RedPostReagentVolume
                    pCustomerReagentVolsRow.RedPostReagentVolumeSteps = pFactoryReagentVolsRow.RedPostReagentVolumeSteps

                    deleteBlkCalibResults = True
                End If
                pCustomerReagentVolsRow.EndEdit()
                pCustomerReagentVolsRow.AcceptChanges()

                'Return value of the flag indicating if previous Blank and Calibrator results have to be deleted 
                myGlobalDataTO.SetDatos = deleteBlkCalibResults
                myGlobalDataTO.HasError = False
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateCustomerReagentVolumes", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search all changes in Test Reagents Volumes between FACTORY DB and CUSTOMER DB (for all STD Tests/Sample Types or for an specific 
        ''' STD Test/Sample Type, depending on value of parameter pUpdateByTestSample)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateByTestSample">Flag indicating the type of update to do:
        '''                                   ** When TRUE, a TestID/SampleType is informed and the process for search and update changes in Reagents
        '''                                      Volumes is executed only for the specified Test/SampleType
        '''                                   ** When FALSE, all STD Tests/SampleTypes with changes in Reagents Volumes are obtained, and for each 
        '''                                      different STD Test/SampleType, the process of update Test Reagents Volumes and delete previous Blank 
        '''                                      and Calibrator Results is executed</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <param name="pTestID">STD Test Identifier. Optional, informed only when parameter pUpdateByTestSample = TRUE</param>
        ''' <param name="pSampleType">Sample Type Code. Optional, informed only when parameter pUpdateByTestSample = TRUE</param>
        ''' <param name="pCustomerTestReagentsVolsDS">TestReagentsVolsDS to return all updated Reagents Volumes for an specific STD Test/Sample Type. 
        '''                                           Optional, informed only when parameter pUpdateByTestSample = TRUE</param>
        ''' <param name="pCustomerTestDS">TestsDS containing data of the specified STD Test. Optional, informed only when parameter pUpdateByTestSample = TRUE</param>
        ''' <returns>GlobalDataTO containing a different value according the value of entry parameter pUpdateByTestSample:
        '''          ** When its value was TRUE, GlobalDataTO contains a Boolean value indicating if previous Blank and Calibrator results for the
        '''             STD Test/SampleType have to be deleted
        '''          ** When its value was FALSE, GlobalDataTO contains success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 13/10/2014 - BA-1944 (SubTask BA-1985)
        ''' </remarks>
        Private Function UpdateModifiedReagentVols(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUpdateByTestSample As Boolean, _
                                                   ByRef pUpdateVersionChangesList As UpdateVersionChangesDS, Optional ByVal pTestID As Integer = -1, _
                                                   Optional ByVal pSampleType As String = "", Optional ByRef pCustomerTestReagentsVolsDS As TestReagentsVolumesDS = Nothing, _
                                                   Optional ByVal pCustomerTestDS As TestsDS = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCustomerTestDS As New TestsDS
                Dim myCustomerTestSampleDS As New TestSamplesDS
                Dim myUpdatedReagentsVolsDS As New TestReagentsVolumesDS
                Dim myFactoryReagentsVolsDS As New TestReagentsVolumesDS
                Dim myCustomerReagentsVolsDS As New TestReagentsVolumesDS

                Dim myTestsDelegate As New TestsDelegate
                Dim myTestSamplesDelegate As New TestSamplesDelegate
                Dim myReagentVolsDelegate As New TestReagentsVolumeDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                Dim deleteBlkCalibResults As Boolean = False
                Dim deleteOnlyCalibResults As Boolean = False
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)

                Dim myFactoryReagentVolumesList As List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                Dim myCustomerReagentVolumesList As List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)

                Dim myTestID As Integer = -1
                Dim mySampleType As String = String.Empty

                'Get differences in Reagents Volumes between FACTORY DB and CUSTOMER DB
                myGlobalDataTO = myTestParametersUpdateDAO.GetUpdatedFactoryTestReagentsVolumes(pDBConnection, pTestID, pSampleType)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myUpdatedReagentsVolsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)

                    If (myUpdatedReagentsVolsDS.tparTestReagentsVolumes.Rows.Count > 0) Then
                        'Get the list of different pairs of TestID/SampleType in the returned DataSet
                        Dim myTestSampleTypes As List(Of String) = (From a In myUpdatedReagentsVolsDS.tparTestReagentsVolumes _
                                                                  Select a.TestID.ToString & "|" & a.SampleType).Distinct.ToList

                        For Each testSampleType As String In myTestSampleTypes
                            'Get the TestID and SampleType to process
                            myTestID = Convert.ToInt32(testSampleType.Split(CChar("|"))(0))
                            mySampleType = testSampleType.Split(CChar("|"))(1).ToString

                            'Get values for the STD Test in FACTORY DB
                            If (Not pUpdateByTestSample) Then
                                myGlobalDataTO = myTestsDelegate.Read(pDBConnection, myTestID)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myCustomerTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)
                                End If
                            Else
                                'If a STD Test/SampleType has been informed, get the TestsDS received as entry parameter
                                myCustomerTestDS = pCustomerTestDS
                            End If

                            'Get values for the STD Test/Sample Type in CUSTOMER DB
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myTestSamplesDelegate.GetDefinition(pDBConnection, myTestID, mySampleType, False)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myCustomerTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                    'This case is not possible...
                                    If (myCustomerTestSampleDS.tparTestSamples.Rows.Count = 0) Then myGlobalDataTO.HasError = True
                                End If
                            End If

                            'Get values of Reagents Volumes for the STD Test/SampleType in FACTORY DB (to get Steps fields)
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myTestParametersUpdateDAO.GetFactoryReagentsVolumesByTesIDAndSampleType(pDBConnection, myTestID, mySampleType, False)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myFactoryReagentsVolsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                                End If
                            End If

                            'Get values of Reagents Volumes for the STD Test/SampleType in CUSTOMER DB
                            If (Not myGlobalDataTO.HasError) Then
                                myGlobalDataTO = myReagentVolsDelegate.GetReagentsVolumesByTestID(pDBConnection, myTestID, mySampleType)
                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                    myCustomerReagentsVolsDS = DirectCast(myGlobalDataTO.SetDatos, TestReagentsVolumesDS)
                                End If
                            End If

                            'Process for each Reagent Number which Volumes has been changed for the Test/SampleType 
                            If (Not myGlobalDataTO.HasError) Then
                                For Each testReagentVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow _
                                                           In myUpdatedReagentsVolsDS.tparTestReagentsVolumes.ToList.Where(Function(a) a.TestID = myTestID AndAlso a.SampleType = mySampleType)
                                    'Search by ReagentNumber in the DS containing values in CUSTOMER DB
                                    myCustomerReagentVolumesList = (From a As TestReagentsVolumesDS.tparTestReagentsVolumesRow In myCustomerReagentsVolsDS.tparTestReagentsVolumes _
                                                                   Where a.ReagentNumber = testReagentVolRow.ReagentNumber _
                                                                  Select a).ToList()

                                    'Search by ReagentNumber in the DS containing values in FACTORY DB
                                    myFactoryReagentVolumesList = (From a As TestReagentsVolumesDS.tparTestReagentsVolumesRow In myFactoryReagentsVolsDS.tparTestReagentsVolumes _
                                                                  Where a.ReagentNumber = testReagentVolRow.ReagentNumber _
                                                                 Select a).ToList()

                                    'Verify changed fields and update values in myCustomerReagentsVolsDS
                                    If (myCustomerReagentVolumesList.Count > 0 AndAlso myFactoryReagentVolumesList.Count > 0) Then
                                        myGlobalDataTO = UpdateCustomerReagentVolumes(myFactoryReagentVolumesList.First, myCustomerReagentVolumesList.First, _
                                                                                      myCustomerTestDS.tparTests.First.TestName, myCustomerTestDS.tparTests.First.ShortName, _
                                                                                      pUpdateVersionChangesList)

                                        'Returned value is obtained only if flag deleteBlankCalibResults is still FALSE
                                        If (Not deleteBlkCalibResults) Then
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                deleteBlkCalibResults = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                                            End If
                                        End If

                                    ElseIf (myCustomerReagentVolumesList.Count = 0 AndAlso myFactoryReagentVolumesList.Count > 0) Then
                                        'It is a new Reagent for the STD Test (AnalysisMode was changed from Mono to Bi Reagent). New Test Reagent Volumes
                                        myFactoryReagentVolumesList.First.IsNew = True
                                        myCustomerReagentsVolsDS.tparTestReagentsVolumes.ImportRow(myFactoryReagentVolumesList.First)
                                        deleteBlkCalibResults = True
                                    End If

                                    'If an error has happened, then the process finishes
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                            End If

                            'Only when a Test/SampleType has not been informed, update the volumes and call the function to delete previous results of Blank and 
                            'Calibrators for the Test/SampleType in process
                            If (Not pUpdateByTestSample) Then
                                'Update the Reagents Volumes for the Test/SampleType
                                If (Not myGlobalDataTO.HasError) Then
                                    myCustomerReagentsVolsDS.AcceptChanges()
                                    myGlobalDataTO = myReagentVolsDelegate.CreateOrUpdate(pDBConnection, myCustomerReagentsVolsDS)
                                End If

                                'If previous Results of Blanks and Calibrators for the STD Tests have to be deleted, prepare the needed TO
                                If (Not myGlobalDataTO.HasError) Then
                                    If (deleteBlkCalibResults) Then
                                        If (myCustomerTestSampleDS.tparTestSamples.First.DefaultSampleType) Then
                                            'If the SampleType is the default for the Test, Blank and Calibrator Results have to be deleted
                                            deleteOnlyCalibResults = False
                                            deleteBlkCalibResults = True
                                        Else
                                            'If the SampleType is not the default for the Test, only the Calibrator Results have to be deleted, while the Blank Results remain
                                            'This is valid for all Calibrator Types due to when it is ALTERNATIV or FACTOR, it is also possible that exist previous results of 
                                            'the last Experimental Calibrator assigned to the Test/Sample Type
                                            deleteOnlyCalibResults = True
                                            deleteBlkCalibResults = False
                                        End If

                                    If (deleteBlkCalibResults OrElse deleteOnlyCalibResults) Then
                                        'Add the TestID, SampleType and TestVersionNumber to the TO of Tests with programming values changed, and mark flags DeleteBlankCalibResults
                                        'and DeleteOnlyCalibrationResult with the corresponding local variable value
                                        myDeletedTestProgramingTO.TestID = myTestID
                                        myDeletedTestProgramingTO.SampleType = IIf(deleteBlkCalibResults, String.Empty, mySampleType).ToString
                                        myDeletedTestProgramingTO.TestVersion = myCustomerTestDS.tparTests.First.TestVersionNumber
                                        myDeletedTestProgramingTO.DeleteBlankCalibResults = deleteBlkCalibResults
                                        myDeletedTestProgramingTO.DeleteOnlyCalibrationResult = deleteOnlyCalibResults
                                        myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)

                                        'Call the function to delete previous results of Blank and Calibrator for the Test/SampleType
                                        myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, New TestsDS, New TestSamplesDS, _
                                                                                           New TestReagentsVolumesDS, New ReagentsDS, New TestReagentsDS, New CalibratorsDS, _
                                                                                           New TestCalibratorsDS, New TestCalibratorValuesDS, New TestRefRangesDS, _
                                                                                           New List(Of DeletedCalibratorTO), New List(Of DeletedTestReagentsVolumeTO), _
                                                                                           myDeletedTestProgramingList, New TestSamplesMultirulesDS, New TestControlsDS, _
                                                                                           Nothing)

                                        'If an error has happened, then the process finishes
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    End If
                                End If
                                End If
                            End If
                        Next

                        'If an specific Test/SampleType was informed, then return value of flag deleteBlkCalibResults and also the TestReagentsVolsDS
                        'with the updated volumes 
                        If (pUpdateByTestSample) Then
                            If (Not myGlobalDataTO.HasError) Then
                                myCustomerReagentsVolsDS.AcceptChanges()
                                pCustomerTestReagentsVolsDS = myCustomerReagentsVolsDS
                                myGlobalDataTO.SetDatos = deleteBlkCalibResults
                            End If
                        End If
                    Else
                        If (pUpdateByTestSample) Then myGlobalDataTO.SetDatos = deleteBlkCalibResults
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UpdateModifiedReagentVols", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all STD TESTS that exists in CUSTOMER DB but for which at least one of the relevant Test fields 
        ''' have changed and modify data in CUSTOMER DB (tables tparTests, tparReagents and tparTestReagents; additionally, depending on the modified 
        ''' fields, previous results of Blanks and Calibrators for the STD Test can be deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 10/10/2014 - BA-1944 (SubTask BA-1985)
        ''' </remarks>
        Public Function UPDATEModifiedSTDTestSamples(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myCustomerTestDS As New TestsDS
                Dim myUpdatedTestSamplesDS As New TestSamplesDS
                Dim myCustomerTestSampleDS As New TestSamplesDS
                Dim myCustomerReagentsVolsDS As New TestReagentsVolumesDS

                Dim myTestsDelegate As New TestsDelegate
                Dim myTestSamplesDelegate As New TestSamplesDelegate
                Dim myTestParametersUpdateDAO As New TestParametersUpdateDAO

                Dim deleteOnlyCalibResults As Boolean = False
                Dim deleteBlankCalibResults As Boolean = False
                Dim myCustomerCalibType As String = String.Empty
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO
                Dim myDeletedTestProgramingList As New List(Of DeletedTestProgramingTO)

                '(1) Search in Factory DB all STD TESTS/SAMPLE TYPES that exists in Customer DB but for which at least one of the
                '    relevant SampleType fields have changed
                myGlobalDataTO = myTestParametersUpdateDAO.GetUpdatedFactoryTestSamples(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myUpdatedTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                    '(1.1) Process each returned STD Test to update it in CUSTOMER DB
                    For Each updatedTestSample As TestSamplesDS.tparTestSamplesRow In myUpdatedTestSamplesDS.tparTestSamples
                        '(1.1.1) Get data of the Test in Customer DB
                        myGlobalDataTO = myTestsDelegate.Read(pDBConnection, updatedTestSample.TestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myCustomerTestDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                            'This case is not possible...
                            If (myCustomerTestDS.tparTests.Rows.Count = 0) Then myGlobalDataTO.HasError = True
                        End If

                        '(1.1.2) Get data of the SampleType for the STD Test in Customer DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestSamplesDelegate.GetDefinition(pDBConnection, updatedTestSample.TestID, updatedTestSample.SampleType, False)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myCustomerTestSampleDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesDS)

                                'This case is not possible...
                                If (myCustomerTestSampleDS.tparTestSamples.Rows.Count = 0) Then myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(1.1.3) Verify changed fields and update values in myCustomerTestSampleDS
                        If (Not myGlobalDataTO.HasError) Then
                            'Before update fields in Customer DS, store in a local variable the current CalibrationType in CUSTOMER DB
                            myCustomerCalibType = myCustomerTestSampleDS.tparTestSamples.First.CalibratorType

                            'Verify changes in Test/SampleType fields 
                            myGlobalDataTO = UpdateCustomerTestSamples(updatedTestSample, myCustomerTestSampleDS.tparTestSamples.First, myCustomerTestDS.tparTests.First.TestName, _
                                                                       myCustomerTestDS.tparTests.First.ShortName, pUpdateVersionChangesList)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                deleteBlankCalibResults = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                            End If
                        End If

                        '(1.1.4) Verify if there are changes in Volumes of Reagents used for the STD Test/SampleType
                        If (Not myGlobalDataTO.HasError) Then
                            myCustomerReagentsVolsDS = New TestReagentsVolumesDS
                            myGlobalDataTO = UpdateModifiedReagentVols(pDBConnection, True, pUpdateVersionChangesList, updatedTestSample.TestID, updatedTestSample.SampleType, _
                                                                       myCustomerReagentsVolsDS, myCustomerTestDS)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                If (Not deleteBlankCalibResults) Then deleteBlankCalibResults = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                            End If
                        End If

                        '(1.1.5) If previous Results of Blanks and Calibrators for the STD Tests have to be deleted, prepare the needed TO
                        If (Not myGlobalDataTO.HasError) Then
                            If (deleteBlankCalibResults) Then
                                If (myCustomerTestSampleDS.tparTestSamples.First.DefaultSampleType) Then
                                    'If the SampleType is the default for the Test, Blank and Calibrator Results have to be deleted
                                    deleteOnlyCalibResults = False
                                    deleteBlankCalibResults = True
                                Else
                                    'If the SampleType is not the default for the Test, only the Calibrator Results have to be deleted, while the Blank Results remain
                                    'This is valid for all Calibrator Types due to when it is ALTERNATIV or FACTOR, it is also possible that exist previous results of 
                                    'the last Experimental Calibrator assigned to the Test/Sample Type
                                    deleteOnlyCalibResults = True
                                    deleteBlankCalibResults = False
                                End If

                                If (deleteBlankCalibResults OrElse deleteOnlyCalibResults) Then
                                    'Add the TestID, SampleType and TestVersionNumber to the TO of Tests with programming values changed, and mark flags DeleteBlankCalibResults
                                    'and DeleteOnlyCalibrationResult with the corresponding local variable value
                                    myDeletedTestProgramingTO.TestID = updatedTestSample.TestID
                                    myDeletedTestProgramingTO.SampleType = IIf(deleteBlankCalibResults, String.Empty, updatedTestSample.SampleType).ToString
                                    myDeletedTestProgramingTO.TestVersion = myCustomerTestDS.tparTests.First.TestVersionNumber
                                    myDeletedTestProgramingTO.DeleteBlankCalibResults = deleteBlankCalibResults
                                    myDeletedTestProgramingTO.DeleteOnlyCalibrationResult = deleteOnlyCalibResults
                                    myDeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                                End If
                            End If
                        End If

                        '(1.1.6) Update the STD Test from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestsDelegate.PrepareTestToSave(pDBConnection, String.Empty, String.Empty, myCustomerTestDS, myCustomerTestSampleDS, myCustomerReagentsVolsDS, _
                                                                               New ReagentsDS, New TestReagentsDS, New CalibratorsDS, New TestCalibratorsDS, _
                                                                               New TestCalibratorValuesDS, New TestRefRangesDS, New List(Of DeletedCalibratorTO), _
                                                                               New List(Of DeletedTestReagentsVolumeTO), myDeletedTestProgramingList, _
                                                                               New TestSamplesMultirulesDS, New TestControlsDS, Nothing, String.Empty, False)
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

                '(2) Search in FACTORY DB all STD Test/SampleType/ReagentNumber for preloaded STD Tests that exists in CUSTOMER DB but for which only the
                '    Reagents Volumes have been changed. Update all Reagents Volumes changed and delete previous Blank and Calibrator Results for the affected STD Tests/SampleTypes
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = UPDATEModifiedReagentVols(pDBConnection, False, pUpdateVersionChangesList)
                End If

                myDeletedTestProgramingTO = Nothing
                myDeletedTestProgramingList = Nothing
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("SDT Test Update Error", "TestParametersUpdateData.UPDATEModifiedSTDTestSamples", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Recalculate value of field TestPosition for all STD Tests in CUSTOMER DB:
        ''' ** Preloaded STD Tests are sorted by TestName and then field TestPosition is set for them begining with 1
        ''' ** User STD Tests are placed after the last of the Preloaded ones but they are not sorted by TestName but by TestID (they 
        '''    will be shown in the same order they were created)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XB 04/06/2014 - BT #1646
        ''' Modified by: SA 21/10/2014 - BA-1944 ==> Function changed from Private to Public
        ''' </remarks>
        Public Function UpdateTestSortByTestName(pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                '(1) Sort Preloaded STD Tests by TestName and then set TestPosition field begining with 1
                Dim myTestDelegate As New TestsDelegate
                myGlobalDataTO = myTestDelegate.UpdatePreloadedTestSortByTestName(pDBConnection)

                '(2) Get all User STD Tests (in the order they were created) and then set TestPosition field following the last Preloaded STD Test
                If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = myTestDelegate.UpdateUserTestPosition(pDBConnection)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("Test Update Error.", "TestParametersUpdateData.UpdateTestSortByTestName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

    End Class

End Namespace
