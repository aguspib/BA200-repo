Option Explicit On
Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
'Imports System.Configuration
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Controls.UserControls
'Imports System.Threading

Public Class IProgTest
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Internal structures Test Programming"

    Private Structure VolumeCalculations
        'volumes
        Dim sampleVol As Single
        Dim R1Vol As Single
        Dim R2Vol As Single
        Dim WashVol As Single

        'Steps
        Dim sampleSteps As Single
        Dim R1Steps As Single
        Dim R2Steps As Single
        Dim WashStepsVol As Single

        'Postdiluion calculation (input fields all but hasError & ErrorCode)
        Dim AnalysisMode As String
        Dim AbsorbanceFlag As Boolean
        Dim PostDilutionFactor As Single
        Dim hasError As Boolean 'Error postdiluted volume flag
        Dim ErrorCode As String 'Error volume code (use for postdilution volumes calculation)

    End Structure

#End Region

#Region "Declarations"
    'Private CalibratorError As Boolean = False 'TR 01/03/2011
    Private currentLanguage As String = "" 'TR 01/03/2011
    Private SelectedTestDS As New TestsDS()
    Private SelectedTestSamplesDS As New TestSamplesDS()
    Private SelectedReagentsDS As New ReagentsDS()
    Private SelectedTestReagentsDS As New TestReagentsDS()
    Private SelectedTestReagentsVolumesDS As New TestReagentsVolumesDS()
    Private SelectedTestCalibratorValuesDS As New TestCalibratorValuesDS 'TR 16/05/201
    Private SelectedTestRefRangesDS As New TestRefRangesDS 'SG 17/06/2010
    Private SelectedTestSampleCalibratorDS As New TestSampleCalibratorDS 'TR 25/02/2011

    Private UpdateTestDS As New TestsDS()
    Private UpdateReagentsDS As New ReagentsDS()
    Private UpdateTestSamplesDS As New TestSamplesDS()
    Private UpdateTestReagentsDS As New TestReagentsDS()
    Private UpdateTestReagentsVolumesDS As New TestReagentsVolumesDS()
    Private UpdateTestRefRangesDS As New TestRefRangesDS() 'SG 17/05/2010

    Private SwParametersDS As New ParametersDS
    Private DeletedTestProgramingList As New List(Of DeletedTestProgramingTO)
    Private DeletedTestReagentVolList As New List(Of DeletedTestReagentsVolumeTO)

    'Private SelectedIndexCtrl As Boolean = True
    Private clearSelection As Boolean = False 'TR 17/11/2010

    Private ValidationError As Boolean = False
    Private EditionMode As Boolean = False
    Private ChangesMade As Boolean = False
    Private NewTest As Boolean = False
    Private ReadyToSave As Boolean = False
    Private TestsPositionChange As Boolean = False
    Private ChangeSampleTypeDuringEdition As Boolean = False    'AG 19/07/2010

    Private PostReduce As New VolumeCalculations
    Private PostIncrease As New VolumeCalculations
    Private NormalVolumeSteps As New VolumeCalculations

    'Private SelectionControl As Integer = 0 'TR 10/03/2010

    'AG 10/03/10 - Load screen status variables definition
    Private isSortTestListAllowed As Boolean = False
    Private isEditTestAllowed As Boolean = False
    Private isCurrentTestInUse As Boolean   'Current test is in use? Yes/No. In multiple selection when there are some selected test in use value is also TRUE
    'TR 28/05/2010 -Variable use to indicate if there is at least one test in use
    Private existTetsInUse As Boolean
    'TR 22/04/2010
    Private randObj As New Random(0)

    'TR 14/06/2010 local stucture use for Calibrator.
    Private UpdatedCalibratorsDS As New CalibratorsDS
    Private UpdatedTestCalibratorDS As New TestCalibratorsDS
    Private UpdatedTestCalibratorValuesDS As New TestCalibratorValuesDS
    Private DeletedCalibratorList As New List(Of DeletedCalibratorTO)

    'SG 21/06/2010 detailed test ref ranges required control list
    Private RequiredDetailedRefControls As New List(Of Control)

    'SG 23/06/2010 detailed range is being edited
    Private IsDetailedRangeEditing As Boolean
    Private RemovedRefDetailedRangeIDs As New List(Of Integer)
    'TR 30/06/2010
    Private AlternativeCalibratorDS As New MasterDataDS()

    Private TempTestID As Integer = 500 'TR 21/07/2010 -Variable use for the Temporal Test id
    Private TempReagentIDMin As Integer = 1000 'TR 21/07/2010
    Private TempReagentIDMax As Integer = 10000 'TR 21/07/2010

    Private DeletedSampleType As String = "" 'SG 22/07/2010
    Private SaveAndClose As Boolean = False  'SG 22/07/2010
    Private CurrentSampleType As String
    Private currentTestViewEditionItem As Integer 'AG 15/11/2010 - Current item in edition mode

    'CAMBIO SG 06/09/2010
    Private selRefStdTestID As Integer
    Private selRefRangeType As String = ""
    Private selMeasureUnit As String = ""
    'END 'SG 06/09/2010

    'TR 19/11/2010 -Variable use to store the path for the Plus Icon 
    'use to show when test have more than one sample type.
    Private PlusIconPath As String = ""

    'TR 03/12/2010 -Variable use to indicate if there was any changes on the calibrator
    Private CalibratorChanges As Boolean

    'TR 09/03/2011 -Use to indicate the prev selected test.
    Private PrevSelectedTest As Integer = -1
    'TR 06/04/2011
    Private SelectedTestSampleMultirulesDS As New TestSamplesMultirulesDS

    'TR 07/04/2011
    Private ControlListDS As New ControlsDS
    Private SelectedTestControlDS As New TestControlsDS
    'Private ControlsListDS As New ControlsDS
    Private ControIDSel As String = ""
    'TR 07/04/2011 -END

    'TR 12/05/2011
    Private SampleTypeMasterDataDS As New MasterDataDS

    'TR 23/05/2011
    Private MinAllowedConcentration As Single              'To store the minimum allowed value for Min/Max Concentration fields
    Private MaxAllowedConcentration As Single              'To store the maximum allowed value for Min/Max Concentration fields
    'TR 23/05/2011

    'TR 24/05/2011 
    Private LocalDeleteControlTOList As New List(Of DeletedControlTO)
    'TR 24/05/2011 

    'TR 22/01/2011
    Private CopyTestData As Boolean = False

    'TR 12/09/2011 -Variable indicate if Selected Test is A Factory Test (Biosystems Test).
    Private isPreloadedTest As Boolean = False

    'DL 10/01/2012. Ini
    Private AddedSampleType As String
    Private ChangesMadeSampleType As Boolean = False
    'DL 10/01/2012. End

    Private IsSampleTypeChanged As Boolean = False 'RH 14/05/2012

#End Region

#Region "Attributes"    'AG 10/03/10
    Private WorkSessionIDAttribute As String
    Private WorkSessionStatusAttribute As String
    Private AnalyzerModelAttribute As String
    Private AnalyzerIDAttribute As String

#End Region             'AG 10/03/10

#Region "Properties"

    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public Property ActiveWorkSessionStatus() As String
        Get
            Return WorkSessionStatusAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionStatusAttribute = value
        End Set
    End Property

    Public Property AnalyzerModel() As String
        Get
            Return AnalyzerModelAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerModelAttribute = value
        End Set
    End Property

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public Property WorkSessionID() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

#End Region

#Region "AG functions (internal calculations)"

    ''' <summary>
    ''' Transform volume to steps (machine language) for sample, r1, r2 and washing
    ''' Call this function before save test programming in order to update steps fields (washingvol, samplevol, R1vol, R2vol)
    ''' Adapted from Ax5 (frmTecnicas: CalcularVolumenesEnPasos and ConvertirAPasos)
    ''' </summary>
    ''' <param name="pSteps_uL"></param>
    ''' <param name="pSample_Steps_uL" ></param>
    ''' <param name="pEntryVolumes"></param>
    ''' <returns>Return a GlobalDataTo with set data as internal structure VolumeCalculations</returns>
    ''' <remarks>
    ''' Created by AG 02/03/2010 (Tested Pending)
    ''' </remarks>
    Private Function CalculateVolumeInSteps(ByVal pSteps_uL As Single, ByVal pSample_Steps_uL As Single, ByVal pEntryVolumes As VolumeCalculations) As GlobalDataTO

        Dim globalToReturn As New GlobalDataTO
        Dim varToReturn As New VolumeCalculations
        Try
            varToReturn = pEntryVolumes 'Init data
            With varToReturn
                'Ax5 method ... changes in Ax00 due the steps_uL are different for samples & reagents
                ''Convert wash volume to steps (NOTE the unit in screen is mL, we need to convert to uL)
                '.WashStepsVol = .WashVol * 1000 * pSteps_uL
                ''Convert sample volume to steps
                '.sampleSteps = .sampleVol * pSteps_uL
                ''Convert R1 volume to steps maintaining the relationship (proportion)
                'Dim factor As Single = 0
                'If .sampleVol > 0 Then factor = .R1Vol / .sampleVol
                '.R1Steps = factor * .sampleSteps
                ''Convert R2 volume to steps maintaining the relationship (proportion)
                'factor = 0
                'If .sampleVol > 0 Then factor = .R2Vol / .sampleVol
                '.R2Steps = factor * .sampleSteps

                'Convert wash volume to steps (NOTE the unit in screen is mL, we need to convert to uL)
                .WashStepsVol = .WashVol * 1000 * pSteps_uL
                'Convert sample volume to steps
                .sampleSteps = .sampleVol * pSample_Steps_uL
                'Convert R1 volume to steps maintaining the relationship (proportion)
                Dim factor As Single = 0
                If .sampleVol > 0 Then factor = .R1Vol / .sampleVol
                .R1Steps = factor * .sampleVol * pSteps_uL
                'Convert R2 volume to steps maintaining the relationship (proportion)
                factor = 0
                If .sampleVol > 0 Then factor = .R2Vol / .sampleVol
                .R2Steps = factor * .sampleVol * pSteps_uL



            End With
            globalToReturn.SetDatos = varToReturn

        Catch ex As Exception
            globalToReturn.HasError = True
            globalToReturn.ErrorCode = "SYSTEM_ERROR"
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CalculateVolumeInSteps ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010 "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return globalToReturn
    End Function

    ''' <summary>
    ''' Calculate the dilution steps.
    ''' </summary>
    ''' <param name="pVolume"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 18/01/2011
    ''' AG 19/04/2011 add pReagentFlag parameter due the STEPS_UL conversion factor is different for reagents and samples
    ''' </remarks>
    Private Function CalculateDilutionSteps(ByVal pVolume As Single, ByVal pReagentFlag As Boolean) As Single
        Dim myResult As Single = 0
        Try
            'Get the machine Cicle
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)

            If pReagentFlag Then 'reagents
                qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                               Where a.ParameterName = GlobalEnumerates.SwParameters.STEPS_UL.ToString _
                               Select a).ToList()
            Else 'samples
                qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                               Where a.ParameterName = GlobalEnumerates.SwParameters.SAMPLE_STEPS_UL.ToString _
                               Select a).ToList()
            End If

            If qswParameter.Count > 0 Then
                myResult = pVolume * qswParameter.First().ValueNumeric
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CalculateDilutionSteps ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Calculate the sample and reagents volumes to his equivalents with the programmed dilution factors (increase and decrease)
    ''' This function is called:
    ''' - On changing each postdilution factor (increase or decrease)
    ''' - On changing each volume (sample or R1 or R2)
    ''' If the postdiluted volumes are out of range a warning message is shown to the user
    ''' Calculations are done only when all data is available
    ''' Adapted from Ax5 (frmTecnicas: ComprobarPostdiluciones and ComprobarVolTotalPostdiluciones)
    ''' </summary>
    ''' <param name="pPostType"></param>
    ''' <param name="pEntryVolumes"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by AG 03/03/2010 (Tested pending)
    ''' </remarks>
    Private Function CheckPostdilutions(ByVal pPostType As String, ByVal pEntryVolumes As VolumeCalculations) As GlobalDataTO
        Dim globalToReturn As New GlobalDataTO
        Dim varToReturn As New VolumeCalculations

        Try
            Dim tmpGlobal As New GlobalDataTO
            varToReturn = pEntryVolumes 'Init data
            globalToReturn.SetDatos = varToReturn

            'Proteccion only calculate for the correct value of postdilution type
            If pPostType <> "INC" AndAlso pPostType <> "RED" Then Exit Try

            'Read the parameter steps_uL
            Dim steps_uL As Single = 1
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
            qswParameter = (From a In SwParametersDS.tfmwSwParameters Where a.ParameterName = GlobalEnumerates.SwParameters.STEPS_UL.ToString Select a).ToList()
            steps_uL = CType(qswParameter.First().ValueNumeric, Single)

            'AG 19/04/2011 - different conversion factor for reagents and samples
            Dim sample_steps_uL As Single = 1 'AG 19/04/2011
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
            Where a.ParameterName = GlobalEnumerates.SwParameters.SAMPLE_STEPS_UL.ToString _
            Select a).ToList()
            sample_steps_uL = CType(qswParameter.First().ValueNumeric, Single)
            'AG 19/04/2011

            Dim volR1R2 As Single = 0
            Dim volS As Single = 0
            volR1R2 = varToReturn.R1Vol + varToReturn.R2Vol
            volS = varToReturn.sampleVol
            'The absorbance test allow no reagent volume ... then the postdiluted sample volume are equal to the normal sample
            If volR1R2 = 0 Then
                varToReturn.R1Vol = 0
                varToReturn.R2Vol = 0
                'Calculate the sample volume in steps
                tmpGlobal = CalculateVolumeInSteps(steps_uL, sample_steps_uL, varToReturn)
                If Not tmpGlobal.HasError And Not tmpGlobal.SetDatos Is Nothing Then
                    varToReturn = CType(tmpGlobal.SetDatos, VolumeCalculations)
                Else
                    globalToReturn = tmpGlobal
                    Exit Try
                End If
            Else
                Dim calculate As Boolean = False    'Calculate only when data is complete
                Select Case varToReturn.AnalysisMode
                    Case "MREP", "MRFT", "MRK"  'Monoreagent
                        If varToReturn.sampleVol <> 0 And varToReturn.R1Vol <> 0 Then calculate = True
                        varToReturn.R2Vol = 0

                    Case "BREP", "BRDIF", "BRFT", "BRK" 'Bireagent
                        If varToReturn.sampleVol <> 0 And varToReturn.R1Vol <> 0 And varToReturn.R2Vol <> 0 Then calculate = True
                End Select

                If calculate Then
                    'Calculate relationship proportions
                    Dim factor As Single = 1
                    Select Case pPostType
                        Case "RED"  'Reduce
                            factor = varToReturn.PostDilutionFactor
                        Case "INC"  'Increase
                            If varToReturn.PostDilutionFactor > 0 Then factor = 1 / varToReturn.PostDilutionFactor
                    End Select

                    Dim relation As Single = 0
                    Dim postrelation As Single = 0
                    If volS > 0 Then relation = volR1R2 / volS
                    postrelation = (factor * (relation + 1)) - 1

                    '1st change sample volume maintaining reagents volume constant
                    If postrelation > 0 Then varToReturn.sampleVol = volR1R2 / postrelation

                    '2on check the new volume with the programmed volume limits
                    Dim myFieldLimitsDS As New FieldLimitsDS
                    Dim limitvalue As Single

                    'Postdiluted Sample Volume (read the limits)
                    myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.POSTDILUTED_SAMPLES_VOLUME, MyBase.AnalyzerModel)

                    Dim vol_errorFlag As Boolean = False
                    'Dim vol_error_code As String = ""

                    Select Case pPostType
                        Case "RED"
                            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                            If varToReturn.sampleVol < limitvalue Then
                                varToReturn.sampleVol = limitvalue
                                RecalculateReagentVolumes(varToReturn, volR1R2, postrelation)

                                'Check if the new calculated reagent volume are inside the ranges
                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT1_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                If varToReturn.R1Vol > limitvalue Then vol_errorFlag = True

                                If Not vol_errorFlag And varToReturn.R2Vol > 0 Then
                                    myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT2_VOLUME, MyBase.AnalyzerModel)
                                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                    If varToReturn.R2Vol > limitvalue Then vol_errorFlag = True
                                End If

                            End If 'If varToReturn.sampleVol < limitvalue Then

                        Case "INC"
                            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                            'If calculated sample volume is higher than limit then samplevol = higher limit and recalculate reagents volumes
                            If varToReturn.sampleVol > limitvalue Then
                                varToReturn.sampleVol = limitvalue
                                RecalculateReagentVolumes(varToReturn, volR1R2, postrelation)

                                'Check if the new calculated reagent volume are inside the ranges
                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT1_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                If varToReturn.R1Vol < limitvalue Then vol_errorFlag = True

                                If Not vol_errorFlag And varToReturn.R2Vol > 0 Then
                                    myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT2_VOLUME, MyBase.AnalyzerModel)
                                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                    If varToReturn.R2Vol < limitvalue Then vol_errorFlag = True
                                End If

                            End If 'If varToReturn.sampleVol > limitvalue Then

                    End Select

                    'If NO postdiluted error flag ... check if the preparation TOTAL volume is inside the ranges
                    If Not vol_errorFlag Then
                        Dim totalVol As Single = 0
                        totalVol = varToReturn.sampleVol + varToReturn.R1Vol + varToReturn.R2Vol
                        myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.PREPARATION_VOLUME, MyBase.AnalyzerModel)

                        'AG 29/07/2013 - Fix bugtracking #903
                        'AG 16/11/2012 - V1 complete the Ax5 algorithm (Ax5: frmTecnicas.ComprobarVolTotalPostdiluciones)

                        ''v0.5.4 code but not it is correct!!!
                        'If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        'If totalVol > limitvalue Then vol_errorFlag = True
                        'If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        'If totalVol < limitvalue Then vol_errorFlag = True

                        'If totalVol > upper prep. volume and post type increase -> Try to modify volumes again
                        Dim factor2 As Single = 1 'New factor
                        Dim r1LimitForTotalCheckings As Single = 0 'Limits for R1 and R2 (max for "RED" criteria and min for "INC" criteria)
                        Dim r2LimitForTotalCheckings As Single = 0

                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        If totalVol > limitvalue Then
                            If totalVol <> 0 AndAlso pPostType = "INC" Then 'If criteria INCREASE ... recalculate
                                factor2 = limitvalue / totalVol
                                varToReturn.sampleVol *= factor2
                                varToReturn.R1Vol *= factor2
                                varToReturn.R2Vol *= factor2

                                'Check no minimum limits have been broken
                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT1_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                r1LimitForTotalCheckings = limitvalue 'VolR1 min

                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT2_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                                r2LimitForTotalCheckings = limitvalue 'VolR2 min

                                If (varToReturn.R1Vol < r1LimitForTotalCheckings) OrElse (varToReturn.R2Vol <> 0 AndAlso varToReturn.R2Vol < r2LimitForTotalCheckings) Then
                                    vol_errorFlag = True
                                End If
                            Else 'If criteria REDUCE ... error
                                vol_errorFlag = True
                            End If
                        End If

                        'If totalVol < lower prep. volume and post type reduce -> Try to modify volumes again
                        If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        If totalVol < limitvalue Then
                            If totalVol <> 0 AndAlso pPostType = "RED" Then 'If criteria REDUCE ... recalculate
                                factor2 = limitvalue / totalVol
                                varToReturn.sampleVol *= factor2
                                varToReturn.R1Vol *= factor2
                                varToReturn.R2Vol *= factor2

                                'Check no maximum limits have been broken
                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT1_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                r1LimitForTotalCheckings = limitvalue 'VolR1 max

                                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT2_VOLUME, MyBase.AnalyzerModel)
                                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then limitvalue = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                                r2LimitForTotalCheckings = limitvalue 'VolR2 max

                                If (varToReturn.R1Vol > r1LimitForTotalCheckings) OrElse (varToReturn.R2Vol <> 0 AndAlso varToReturn.R2Vol > r2LimitForTotalCheckings) Then
                                    vol_errorFlag = True
                                End If
                            Else 'If criteria INCREASE ... error
                                vol_errorFlag = True
                            End If
                        End If
                        'AG 29/07/2013


                    End If
                    If vol_errorFlag Then
                        varToReturn.hasError = True
                        varToReturn.ErrorCode = "WRONG_POSTDILUTION_FACTOR"
                    Else
                        'Convert new volumes to steps
                        tmpGlobal = CalculateVolumeInSteps(steps_uL, sample_steps_uL, varToReturn)
                        If Not tmpGlobal.HasError And Not tmpGlobal.SetDatos Is Nothing Then
                            varToReturn = CType(tmpGlobal.SetDatos, VolumeCalculations)
                        Else
                            globalToReturn = tmpGlobal
                            Exit Try
                        End If
                    End If
                End If 'If calculate Then
            End If 'If volR1R2 = 0 Then
            globalToReturn.SetDatos = varToReturn

        Catch ex As Exception
            globalToReturn.HasError = True
            globalToReturn.ErrorCode = "SYSTEM_ERROR"
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CheckPostdilutions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010 "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return globalToReturn
    End Function

    ''' <summary>
    ''' Recalculate the reagents volume because the postdiluted sample volume has fixed to the limit value (min or max)
    ''' NOTE the 1st parameter is ByRef
    ''' Adapted from Ax5 (frmTecnicas: ComprobarPostdiluciones)
    ''' </summary>
    ''' <param name="PostVolumes"></param>
    ''' <param name="volR1R2"></param>
    ''' <param name="PostRelation"></param>
    ''' <remarks>
    ''' Created by AG 03/03/2010 (Tested Pending)
    ''' </remarks>
    Private Sub RecalculateReagentVolumes(ByRef PostVolumes As VolumeCalculations, ByVal volR1R2 As Single, ByVal PostRelation As Single)
        Try
            If volR1R2 > 0 Then

                Dim initialReagentVol As Single = PostVolumes.R1Vol
                Dim NewPostRelationFactor As Single = 1
                'Recalculate R1 vol
                NewPostRelationFactor = (initialReagentVol / volR1R2) * PostRelation
                PostVolumes.R1Vol = PostVolumes.sampleVol * NewPostRelationFactor
                'Recalculate R2 vol is needed
                If PostVolumes.R2Vol > 0 Then
                    initialReagentVol = PostVolumes.R2Vol
                    NewPostRelationFactor = (initialReagentVol / volR1R2) * PostRelation
                    PostVolumes.R2Vol = PostVolumes.sampleVol * NewPostRelationFactor
                End If
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " RecalculateReagentVolumes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010 "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "AG functions 18/03/2010"

    ''' <summary>
    ''' Search if there are results for the TestID or for the TestID - SampleType
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <param name="pTestVersion"></param>
    ''' <param name="pSampleType"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks>Created by AG 18/03/2010 (Tested OK)
    '''          Modified AG 22/03/2010 use dbConnection</remarks>
    '''                   TR 14/10/2011 -Remove the connection form presentation layer
    Private Function ExistsTestResults(ByVal pTestID As Integer, ByVal pTestVersion As Integer, Optional ByVal pSampleType As String = "") As Boolean
        Dim toReturn As Boolean = False

        Try
            Dim res_delegate As New ResultsDelegate
            Dim localres As New GlobalDataTO

            'If no sampletype -> search blank results
            If pSampleType.Trim = "" Then
                localres = res_delegate.GetLastExecutedBlank(Nothing, pTestID, pTestVersion)
            Else    'If sampletype -> search calibrator results
                localres = res_delegate.GetLastExecutedCalibrator(Nothing, pTestID, pSampleType, pTestVersion)
            End If

            If Not localres.HasError And Not localres.SetDatos Is Nothing Then
                Dim AdditionalElements As New WSAdditionalElementsDS
                AdditionalElements = CType(localres.SetDatos, WSAdditionalElementsDS)

                'AG 22/03/2010
                If AdditionalElements.WSAdditionalElementsTable.Rows.Count > 0 Then toReturn = True
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ExistsTestResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
            toReturn = False
        End Try
        Return toReturn
    End Function

    ''' <summary>
    ''' Clear the parameter area
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearParametersArea()
        Try
            'Clear the structures.
            SelectedTestDS.tparTests.Clear()
            SelectedReagentsDS.tparReagents.Clear()
            SelectedTestSamplesDS.tparTestSamples.Clear()
            SelectedTestReagentsDS.tparTestReagents.Clear()
            SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Clear()

            'add the new row to the tests dataset
            Dim newTestRow As TestsDS.tparTestsRow
            newTestRow = SelectedTestDS.tparTests.NewtparTestsRow()

            'Add the temporal TESTID 
            newTestRow.TestID = 1
            newTestRow.NewTest = True
            ''add the row into the Test datatable.
            newTestRow.TestID = 1
            newTestRow.NewTest = True
            'add the row into the Test datatable.
            SelectedTestDS.tparTests.AddtparTestsRow(newTestRow)
            'Prepare the controls for a new test.
            TestDescriptionTextBox.Clear()
            TestDescriptionTextBox.Enabled = False
            TestNameTextBox.Clear()
            ShortNameTextBox.Clear()
            AbsCheckBox.Checked = False
            'TurbidimetryCheckBox.Checked = False 'TR 07/05/2011 -Do not implement turbidimetry
            SampleVolUpDown.ResetText()
            'WashSolVolUpDown.Value = 1
            VolR1UpDown.ResetText()
            VolR2UpDown.ResetText()
            FirstReadingCycleUpDown.ResetText()
            FirstReadingSecUpDown.ResetText()
            SecondReadingCycleUpDown.ResetText()
            SecondReadingSecUpDown.ResetText()
            UnitsCombo.SelectedIndex = -1
            'RedPostDilutionFactorTextBox.Text = "1"
            'IncPostDilutionFactorTextBox.Text = "1"
            CalibrationFactorTextBox.Clear()
            ReportsNameTextBox.Clear()
            'BlankAbsorbanceValueTextBox.Clear()
            BlankAbsorbanceUpDown.ResetText()
            'KineticBlancValueTextBox.Clear()
            KineticBlankUpDown.ResetText()
            'LinearityLimitValueTextBox.Clear()
            LinearityUpDown.ResetText()
            'DetectionLimitTextBox.Clear()
            DetectionUpDown.ResetText()
            BlankTypesCombo.SelectedIndex = -1
            BlankReplicatesUpDown.Value = 1
            DecimalsUpDown.ResetText()
            ReplicatesUpDown.ResetText()

            'DL 11/01/2012
            'SampleTypeCombo.SelectedIndex = -1
            'bsSampleTypeText.Text = String.Empty
            SampleTypeCboEx.Items.Clear()
            'DL 11/01/2012 

            AnalysisModeCombo.SelectedIndex = -1
            ReactionTypeCombo.SelectedIndex = -1
            ReactionTypeCombo.SelectedIndex = -1
            'ReadingModeCombo.SelectedIndex = -1 TR 07/04/2010
            MainFilterCombo.SelectedIndex = -1
            WashSolVolUpDown.ResetText()
            RedPostDilutionFactorTextBox.Clear()
            IncPostDilutionFactorTextBox.Clear()
            BlankReplicatesUpDown.ResetText()
            CalReplicatesUpDown.ResetText()
            PredilutionModeCombo.SelectedIndex = -1
            PredilutionFactorTextBox.Clear()

            DiluentComboBox.SelectedIndex = -1 'TR 17/01/2011

            ProzoneT1UpDown.ResetText()
            ProzoneT2UpDown.ResetText()

            'TODO clear the calibration grid and the reference ranges controls too

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ClearParametersArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "AG functions (Warning screen informing save changes and his effects) -10/03/10-"

    ''' <summary>
    ''' Search if changes are made in controls who involves deleting old results from blank and calibrator (in GENERAL tab)
    ''' 
    ''' Adapt from Ax5 (frmTecnicas.BuscarCambionsEnIdentificacion)
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <returns>
    ''' Return 0 -> No delete old results
    ''' Return 1 -> Delete old results (calibration)
    ''' Return 2 -> Delete old results (blank and calibration)
    ''' </returns>
    ''' <remarks>
    ''' Created by AG 08/03/2010 (Tested OK)
    ''' </remarks>
    Private Function SearchChangesInGeneralTab(ByVal pTestID As Integer) As Integer
        Dim codeToReturn As Integer = 0
        Try
            'Changes in tparTest dataset
            Dim qtest As New List(Of TestsDS.tparTestsRow)
            qtest = (From a In SelectedTestDS.tparTests Where a.TestID = pTestID Select a).ToList

            If qtest.Count > 0 Then
                If qtest.First.AnalysisMode.Trim <> AnalysisModeCombo.SelectedValue.ToString().TrimEnd() Then
                    codeToReturn = 2
                End If

                If qtest.First.ReactionType.Trim <> ReactionTypeCombo.SelectedValue.ToString().TrimEnd() Then
                    codeToReturn = 2
                End If
                'TR 07/05/2011 -do not implement turbidimetry
                'If qtest.First.TurbidimetryFlag <> TurbidimetryCheckBox.Checked Then
                '    codeToReturn = 2
                'End If
                'TR 07/05/2011 -END
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " SearchChangesInGeneralTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
            codeToReturn = 0
        End Try
        Return codeToReturn
    End Function

    ''' <summary>
    ''' Search if changes are made in controls who involves deleting old results from blank and calibrator (in PROCEDURE tab)
    ''' 
    ''' Adapt from Ax5 (frmTecnicas.BuscarCambionsEnProcedimiento)
    ''' </summary>
    ''' <param name="pTestID"></param>    
    ''' <param name="pSampleType"></param>    
    ''' <returns>
    ''' Return 0 -> No delete old results
    ''' Return 1 -> Delete old results (calibration)
    ''' Return 2 -> Delete old results (blank and calibration)
    ''' </returns>
    ''' <remarks>
    ''' Created by AG 08/03/2010 (Tested OK)
    ''' AG 14/01/2011 - Search changes filtering by sample type in TestSampleDS
    ''' </remarks>
    Private Function SearchChangesInProcedureTab(ByVal pTestID As Integer, ByVal pSampleType As String) As Integer
        Dim codeToReturn As Integer = 0
        Try

            'Changes in tparTest dataset
            ''''''''''''''''''''''''''''
            Dim qtest As New List(Of TestsDS.tparTestsRow)
            qtest = (From a In SelectedTestDS.tparTests Where a.TestID = pTestID Select a).ToList

            If qtest.Count > 0 Then
                If qtest.First.ReadingMode.Trim <> ReadingModeCombo.SelectedValue.ToString().TrimEnd() Then
                    codeToReturn = 2
                End If

                If qtest.First.MainWavelength.ToString() <> MainFilterCombo.SelectedValue.ToString().TrimEnd() Then
                    codeToReturn = 2
                End If

                'AG 25/03/2010
                'If Not qtest.First.IsReferenceWavelengthNull then
                If Not qtest.First.IsReferenceWavelengthNull And Not ReferenceFilterCombo.SelectedValue Is Nothing Then

                    If qtest.First.ReferenceWavelength.ToString() <> ReferenceFilterCombo.SelectedValue.ToString().TrimEnd() Then
                        codeToReturn = 2
                    End If
                End If

                If qtest.First.FirstReadingCycle.ToString.Trim <> FirstReadingCycleUpDown.Value.ToString.Trim Then
                    codeToReturn = 2
                End If
                If Not qtest.First.IsSecondReadingCycleNull Then
                    If qtest.First.SecondReadingCycle.ToString.Trim <> SecondReadingCycleUpDown.Value.ToString.Trim Then
                        codeToReturn = 2
                    End If
                End If
            End If

            'Changes in tparTestSample dataset
            ''''''''''''''''''''''''''''''''''            
            Dim qtestSample As New List(Of TestSamplesDS.tparTestSamplesRow)
            qtestSample = (From a In SelectedTestSamplesDS.tparTestSamples Where a.TestID = pTestID And a.SampleType = pSampleType Select a).ToList

            'Find changes for all sample types defined
            If qtestSample.Count > 0 Then
                For i As Integer = 0 To qtestSample.Count - 1
                    If qtestSample.Item(i).SampleVolume.ToString.Trim <> SampleVolUpDown.Value.ToString.Trim Then
                        codeToReturn = 2
                    End If
                    If qtestSample.Item(i).RedPostdilutionFactor.ToString.Trim <> RedPostDilutionFactorTextBox.Text.ToString.Trim Then
                        codeToReturn = 2
                    End If
                    If qtestSample.Item(i).IncPostdilutionFactor.ToString.Trim <> IncPostDilutionFactorTextBox.Text.ToString.Trim Then
                        codeToReturn = 2
                    End If

                    'Changes in tparTestReagentsVolumes dataset            
                    Dim qtestreagents As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                    qtestreagents = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes Where a.TestID = pTestID And a.SampleType = qtestSample.Item(i).SampleType And a.ReagentNumber = 1 Select a).ToList

                    If qtestreagents.Count > 0 Then
                        If qtestreagents.First.ReagentVolume.ToString.Trim <> VolR1UpDown.Value.ToString.Trim Then
                            codeToReturn = 2
                        End If
                    End If

                    qtestreagents = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes Where a.TestID = pTestID And a.SampleType = qtestSample.Item(i).SampleType And a.ReagentNumber = 2 Select a).ToList
                    If qtestreagents.Count > 0 Then
                        If qtestreagents.First.ReagentVolume.ToString.Trim <> VolR2UpDown.Value.ToString.Trim Then
                            codeToReturn = 2
                        End If
                    End If

                    If codeToReturn = 2 Then Exit For
                Next i
            End If

        Catch ex As Exception
            codeToReturn = 0
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SearchChangesInProcedureTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return codeToReturn
    End Function

    ''' <summary>
    ''' Search if changes are made in controls who involves deleting old results from blank and calibrator (in CALIBRATOR tab)
    ''' Adapt from Ax5 (frmTecnicas.BuscarCambionsEnCalibracion)
    ''' </summary>
    ''' <param name="pTestID"></param>    
    ''' <param name="pSampleType "></param>
    ''' <returns>
    ''' Return 0 -> No delete old results
    ''' Return 1 -> Delete old results (calibration)
    ''' Return 2 -> Delete old results (blank and calibration)
    ''' </returns>
    ''' <remarks>
    ''' Created by AG 08/03/2010 (Tested Pending ... TODO changes in calibration programming)
    ''' AG 14/01/2011 - Search changes filtering by sample type in TestSampleDS
    ''' </remarks>
    Private Function SearchChangesInCalibratorTab(ByVal pTestID As Integer, ByVal pSampleType As String) As Integer
        Dim codeToReturn As Integer = 0
        Try
            'Changes in tparTest dataset
            ''''''''''''''''''''''''''''
            Dim qtest As New List(Of TestsDS.tparTestsRow)
            qtest = (From a In SelectedTestDS.tparTests Where a.TestID = pTestID Select a).ToList
            If qtest.First.BlankMode.ToString.Trim <> BlankTypesCombo.SelectedValue.ToString.Trim Then
                codeToReturn = 2
            End If

            'changes in calibration (codeToReturn = 1)
            If codeToReturn = 0 Then
                'For all sample type defined search changes in calibration
                Dim qtestSample As New List(Of TestSamplesDS.tparTestSamplesRow)
                qtestSample = (From a In SelectedTestSamplesDS.tparTestSamples Where a.TestID = pTestID And a.SampleType = pSampleType Select a).ToList

                'Find changes for all sample types defined
                If qtestSample.Count > 0 Then
                    'For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In qtestSample 'TR 24/05/2011Commented
                    'TR 03/12/2010 -Validate changes.
                    If CalibratorChanges Then
                        codeToReturn = 1
                    End If
                    'Next
                End If
            End If

        Catch ex As Exception
            codeToReturn = 0
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SearchChangesInCalibratorTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return codeToReturn
    End Function

    ''' <summary>
    ''' Search changes in test - sample type that implicates delete old results for blank and / or calibrator to reuse
    ''' Adapt from Ax5 (frmTecnicas.AnalizarSihayCambiosEnTecnica)
    ''' </summary>
    ''' <returns>
    ''' Integer with Windows.Forms.DialogResult
    ''' </returns>
    ''' <remarks>
    ''' Created by AG 08/03/2010
    ''' Modified by AG 25/03/2010 - add optional parameter
    ''' </remarks>
    Private Function SavePendingWarningMessage(Optional ByVal pSaveButtonPressed As Boolean = False) As DialogResult
        'CHANGE THE VALUE TYPE TO THE CORRECT TYPE 
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes
        Try
            If (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                'Execute only when exist blank or calibrator results
                Dim qtest As New List(Of TestsDS.tparTestsRow)
                qtest = (From a In SelectedTestDS.tparTests Select a).ToList
                Dim myGlobalDataTO As New GlobalDataTO
                If qtest.Count > 0 Then
                    For i As Integer = 0 To qtest.Count - 1
                        Dim j As Integer = 0    'AG 25/03/2010
                        Dim existBlankResults As Boolean = False
                        Dim existCalibratorResults As Boolean = False
                        Dim userAnsweredYesToDeleteBlank As Boolean = False 'AG 25/03/2010
                        Dim userAnsweredYesToCalibBlank As Boolean = False 'AG 25/03/2010
                        Dim codeMessage As String = ""
                        Dim tempcode As Integer = 0
                        Dim ptestversion As Integer = 0
                        Dim existRemovedControl As Boolean = False 'TR 24/05/2011
                        Dim imageBytes As Byte() 'TR 24/05/2011
                        Dim myPreloadeDelegate As New PreloadedMasterDataDelegate 'TR 24/05/2011

                        'TR 24/05/2011
                        Dim myQCResultsDelegate As New QCResultsDelegate
                        Dim myHistoryQCInfoDS As New HistoryQCInformationDS
                        Dim myHistoryQCInfoList As New List(Of HistoryQCInformationDS.HistoryQCInfoTableRow)
                        'TR 24/05/2011 -END

                        Dim myAdditionalElementsDS As New WSAdditionalElementsDS 'TR 29/11/2010
                        Dim myAffectedElementsResults As New DialogResult

                        If Not qtest.Item(i).IsExistBlankResultNull Then existBlankResults = qtest.Item(i).ExistBlankResult
                        If Not qtest.Item(i).IsDeleteResultAnswerNull Then userAnsweredYesToDeleteBlank = qtest.Item(i).DeleteResultAnswer 'AG 25/03/2010
                        Dim ptestID As Integer = qtest.Item(i).TestID

                        'TG 22/03/2010 Validate is not null before assigned the value 
                        If Not qtest.Item(i).IsTestVersionNumberNull Then
                            ptestversion = qtest.Item(i).TestVersionNumber '//19/03/2010
                        End If
                        'END TG 22/03/2010

                        'Search in TestSamplesDS variable if exist calibrator results and update existCalibratorResults variable
                        Dim qtestsample As New List(Of TestSamplesDS.tparTestSamplesRow)
                        qtestsample = (From a In SelectedTestSamplesDS.tparTestSamples Where a.TestID = ptestID Select a).ToList

                        If qtestsample.Count > 0 Then
                            'For j As Integer = 0 To qtestsample.Count - 1  'AG 25/03/2010
                            For j = 0 To qtestsample.Count - 1

                                If Not qtestsample.Item(j).IsExistCalibResultNull Then existCalibratorResults = qtestsample.Item(j).ExistCalibResult
                                If Not qtestsample.Item(j).IsDeleteResultAnswerNull Then userAnsweredYesToCalibBlank = qtestsample.Item(j).DeleteResultAnswer 'AG 25/03/2010
                                If existCalibratorResults Then Exit For
                            Next j
                        End If

                        'TR 24/05/2011 -Validate if there are removed controls
                        If LocalDeleteControlTOList.Count > 0 Then
                            existRemovedControl = True
                        End If


                        'AG 25/03/2010 - If user wants to delete older results only ask one time for it
                        If (existBlankResults And Not userAnsweredYesToDeleteBlank) Or _
                        (existCalibratorResults And Not userAnsweredYesToCalibBlank) Or (existRemovedControl) Then

                            'Search changes in GENERAL tab
                            tempcode = Me.SearchChangesInGeneralTab(ptestID)

                            'Search changes in PROCEDURE tab (if = 2 dont find more because we had to delete older save results)
                            If tempcode <> 2 Then
                                Dim codeChangesProcedure As Integer = Me.SearchChangesInProcedureTab(ptestID, CurrentSampleType)
                                If codeChangesProcedure <> 0 Then tempcode = codeChangesProcedure
                            End If

                            'Search changes in CALIBRATION tab (if = 2 dont find more because we had to delete older save results)
                            If tempcode <> 2 Then
                                Dim codeCalibratorChanges As Integer = Me.SearchChangesInCalibratorTab(ptestID, CurrentSampleType)
                                If codeCalibratorChanges <> 0 Then tempcode = codeCalibratorChanges

                            End If


                        End If 'If existBlankResults Or existCalibratorResults Then

                        codeMessage = GlobalEnumerates.Messages.SAVE_PENDING.ToString 'AG 07/07/2010 "SAVE_PENDING"
                        'TR 29/11/2010 -Use to add the absorbance and the factor in one variable
                        Dim myProfileMember As String = ""

                        If tempcode = 0 AndAlso Not existRemovedControl Then
                            codeMessage = GlobalEnumerates.Messages.SAVE_PENDING.ToString 'AG 07/07/2010 (Dont use Discard_pending in this method, ask AG if you has doubts)"DISCARD_PENDING_CHANGES" '"SAVE_PENDING"
                        ElseIf tempcode = 1 And existCalibratorResults Then
                            codeMessage = GlobalEnumerates.Messages.DELETE_CALIB_RESULTS_STORED.ToString 'AG 07/07/2010 "DELETE_CALIB_RESULTS_STORED"
                            If userAnsweredYesToDeleteBlank Then codeMessage = GlobalEnumerates.Messages.SAVE_PENDING.ToString 'AG 25/03/2010 (protection case if user has answered YES to delete old results before)
                        ElseIf tempcode = 2 And existBlankResults Then
                            codeMessage = GlobalEnumerates.Messages.DELETE_TEST_RESULTS_STORED.ToString 'AG 07/07/2010 "DELETE_TEST_RESULTS_STORED"
                            If userAnsweredYesToCalibBlank Then codeMessage = GlobalEnumerates.Messages.SAVE_PENDING.ToString 'AG 25/03/2010 (protection case if user has answered YES to delete old results before)
                        ElseIf existRemovedControl Then
                            codeMessage = GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString() 'TR 24/05/2011 there are cumulate results
                        End If

                        'TR 29/11/2010 -GET HERE THE Result data Store.
                        If tempcode = 1 OrElse tempcode = 2 Then
                            Dim myResultDelegate As New ResultsDelegate
                            Dim myResultGlobalTO As New GlobalDataTO

                            If tempcode = 2 Then
                                'Get the Executed blanks.
                                myResultGlobalTO = myResultDelegate.GetLastExecutedBlank(Nothing, qtest.First().TestID, _
                                                                qtest.First().TestVersionNumber, "", "")
                                If Not myResultGlobalTO.HasError Then
                                    'Set the values to my ResultDS 
                                    myAdditionalElementsDS = DirectCast(myResultGlobalTO.SetDatos, WSAdditionalElementsDS)
                                End If
                            End If

                            For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples.Rows

                                myResultGlobalTO = myResultDelegate.GetLastExecutedCalibrator(Nothing, qtest.First().TestID, _
                                                                            TestSampleRow.SampleType, qtest.First().TestVersionNumber)

                                If Not myResultGlobalTO.HasError Then
                                    'Go throught each element and set the absorbance value into myProfilemember variable, 
                                    'then add it into the myAdditionalelementsDS
                                    For Each ResultRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow In _
                                                      DirectCast(myResultGlobalTO.SetDatos, WSAdditionalElementsDS).WSAdditionalElementsTable.Rows

                                        If ResultRow.IsCalibratorFactorNull Then
                                            myProfileMember &= ResultRow.ABSValue & vbCrLf

                                        Else
                                            myProfileMember &= ResultRow.ABSValue & " (" & _
                                                               ResultRow.CalibratorFactor & ")" & vbCrLf
                                        End If

                                        myAdditionalElementsDS.WSAdditionalElementsTable.ImportRow(ResultRow)
                                    Next
                                End If
                            Next

                        End If
                        'TR 03/12/2010 -END.

                        'AG 25/03/2010 - if user has press save button only ask message if save involves delete old results
                        '                otherwise save without asking
                        If pSaveButtonPressed And codeMessage = GlobalEnumerates.Messages.SAVE_PENDING.ToString Then
                            'SelectedTestRefRangesDS.tparTestRefRanges.Clear() 'AG 29/07/2010 - SG 06/07/2010

                            If SaveTestLocal(False, CurrentSampleType) Then 'AG 12/01/2011 - add parameter (global screen variable)
                                dialogResultToReturn = Windows.Forms.DialogResult.Yes
                            Else
                                dialogResultToReturn = Windows.Forms.DialogResult.No
                            End If

                        ElseIf Me.TestNameTextBox.Text <> "" And Me.ShortNameTextBox.Text <> "" Then 'Else: User hasnt pressed save button or has pressed it and changes involves delete old results
                            'END AG 25/03/2010
                            'TR 29/11/2010 
                            Dim myDependenciesElementsDS As New DependenciesElementsDS
                            'if additionale elements has row then star preparing the dependecieselementsDS.
                            If myAdditionalElementsDS.WSAdditionalElementsTable.Count > 0 Then
                                Dim myDepElemenRow As DependenciesElementsDS.DependenciesElementsRow
                                Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

                                For Each ResultRow As WSAdditionalElementsDS.WSAdditionalElementsTableRow In _
                                                                myAdditionalElementsDS.WSAdditionalElementsTable.Rows

                                    imageBytes = myPreloadeDelegate.GetIconImage(ResultRow.SampleClass)
                                    myDepElemenRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                    myDepElemenRow.Type = imageBytes
                                    'myDepElemenRow.Name = ResultRow.SampleClass 'TR 30/07/2013 -Commented 

                                    'TR 30/07/2013 - Get the Calibrator name if Sample class is CALIB
                                    If ResultRow.SampleClass = "BLANK" Then
                                        myDepElemenRow.Name = ResultRow.SampleClass
                                    Else
                                        'Get calibrator name.
                                        myGlobalDataTO = myTestCalibratorsDelegate.GetTestCalibratorData(Nothing, ResultRow.TestID, ResultRow.SampleType)
                                        If Not myGlobalDataTO.HasError Then
                                            If DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS).tparTestCalibrators.Count > 0 Then
                                                myDepElemenRow.Name = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS).tparTestCalibrators(0).CalibratorName
                                            Else
                                                myDepElemenRow.Name = ResultRow.SampleClass
                                            End If
                                        End If
                                    End If
                                    'TR 30/07/2013 -END 


                                    myDepElemenRow.FormProfileMember = GetMessageText(GlobalEnumerates.Messages.SAVED_RESULT_WARN.ToString(), currentLanguage) 'TR 12/05/2011
                                    myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDepElemenRow)

                                Next
                            End If

                            'TR 24/05/2011
                            If tempcode = 3 OrElse existRemovedControl Then

                                'Validate if there're deleted controls.
                                If LocalDeleteControlTOList.Count > 0 Then
                                    For Each myDelControTo As DeletedControlTO In LocalDeleteControlTOList
                                        'Get the pending results by the TestID/SampleType 
                                        '(Sample type can be empty in this case brings all the controls with pending results for the testID.)
                                        myGlobalDataTO = myQCResultsDelegate.SearchPendingResultsByTestIDSampleTypeNEW(Nothing, "STD", myDelControTo.TestID, _
                                                                                                                       myDelControTo.SampleType)
                                        If Not myGlobalDataTO.HasError Then
                                            myHistoryQCInfoDS = DirectCast(myGlobalDataTO.SetDatos, HistoryQCInformationDS)

                                            'TR 24/05/2011 -Validate if there are any cumulate QC
                                            myHistoryQCInfoList = (From a In myHistoryQCInfoDS.HistoryQCInfoTable _
                                                                  Where a.ControlID = myDelControTo.ControlID _
                                                                  Select a).ToList()

                                            If myHistoryQCInfoList.Count > 0 Then
                                                imageBytes = myPreloadeDelegate.GetIconImage("CTRL")
                                                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow
                                                myDependenciesElementsRow = myDependenciesElementsDS.DependenciesElements.NewDependenciesElementsRow()
                                                myDependenciesElementsRow.Type = imageBytes
                                                myDependenciesElementsRow.Name = myHistoryQCInfoList.First().ControlName
                                                myDependenciesElementsRow.FormProfileMember = myHistoryQCInfoList.First().TestName

                                                myDependenciesElementsRow.FormProfileMember &= " " & _
                                                                GetMessageText(GlobalEnumerates.Messages.CUMULATE_QCRESULTS.ToString, currentLanguage)

                                                myDependenciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                                            End If
                                            'TR 24/05/2011 -END.
                                        End If
                                    Next
                                End If
                            End If
                            'TR 24/05/2011
                            If myDependenciesElementsDS.DependenciesElements.Count > 0 Then
                                Using myAffectedElementsWarning As New IWarningAfectedElements()
                                    myAffectedElementsWarning.AffectedElements = myDependenciesElementsDS
                                    myAffectedElementsWarning.ShowDialog()

                                    myAffectedElementsResults = myAffectedElementsWarning.DialogResult
                                End Using
                            Else
                                myAffectedElementsResults = Windows.Forms.DialogResult.OK
                            End If
                            'TR 29/11/2010 -END
                            If myAffectedElementsResults = Windows.Forms.DialogResult.OK Then 'TR 29/11/2010 validate the affected elements.
                                'If ShowMessage("Warning", codeMessage) = Windows.Forms.DialogResult.Yes Then
                                dialogResultToReturn = Windows.Forms.DialogResult.Yes

                                'AG & TR 19/03/2010
                                If codeMessage = GlobalEnumerates.Messages.DELETE_TEST_RESULTS_STORED.ToString Then 'add the pTestID to the list for delete old results (blank and calibrator)
                                    Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO()
                                    myDeletedTestProgramingTO.TestID = ptestID
                                    myDeletedTestProgramingTO.SampleType = ""
                                    myDeletedTestProgramingTO.DeleteBlankCalibResults = True
                                    myDeletedTestProgramingTO.TestVersion = ptestversion
                                    'add the deleted sample type.
                                    DeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                                    qtest.Item(i).DeleteResultAnswer = True 'AG 25/03/2010
                                    'TR 09/12/2010 -Change index instead of j indicate the index 0
                                    qtestsample.Item(0).DeleteResultAnswer = True  'AG 25/03/2010

                                ElseIf codeMessage = GlobalEnumerates.Messages.DELETE_CALIB_RESULTS_STORED.ToString Then 'add the pTestID-sampleType to the list for delete old results (calibrator)
                                    Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO()
                                    myDeletedTestProgramingTO.TestID = ptestID
                                    myDeletedTestProgramingTO.DeleteOnlyCalibrationResult = True
                                    myDeletedTestProgramingTO.TestVersion = ptestversion

                                    '''''''''''''''''''''''''''''
                                    'TODO: Need to know the sample type to delete.
                                    ''''''''''''''''''''''''
                                    myDeletedTestProgramingTO.SampleType = CurrentSampleType 'TR 03/12/2010 -Set the current type.

                                    'add the deleted sample type.
                                    DeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                                    qtestsample.Item(j).DeleteResultAnswer = True  'AG 25/03/2010

                                End If
                                'END AG & TR 19/03/2010

                                'TR 29/03/2010 -update the position in case there is a change.
                                If TestsPositionChange Then
                                    UpdateTestPosition()
                                End If

                                'Call the local save button click event
                                'CurrentSampleType = SelectedSampleTypeCombo.SelectedValue.ToString 'AG 12/01/2011 - comment this line
                                If Not SaveTestLocal(False, CurrentSampleType) Then 'AG 12/01/2011 - add parameter (global screen variable)
                                    dialogResultToReturn = Windows.Forms.DialogResult.No    'SaveTestLocal validation rules failled. We continue in edition mode
                                End If

                            Else
                                'We return YES in order to leave edition mode but user has discard changes in test (dont save them)
                                'dialogResultToReturn = Windows.Forms.DialogResult.No
                                dialogResultToReturn = Windows.Forms.DialogResult.No
                                ReadyToSave = False 'TR 03/12/2010 
                            End If

                            'AG 25/03/2010 //AG 15/03/2010
                            'If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                            If dialogResultToReturn = Windows.Forms.DialogResult.Yes And Not pSaveButtonPressed Then
                                EditionMode = False
                                ChangesMade = False
                                NewTest = False
                                Me.bsTestRefRanges.isEditing = False 'SG 06/09/2010
                            End If
                            'END AG 15/03/2010
                        Else
                            EditionMode = False
                            ChangesMade = False
                            NewTest = False
                            Me.bsTestRefRanges.isEditing = False 'SG 06/09/2010
                        End If   'AG 25/03/2010
                    Next
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SavePendingWarningMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
            dialogResultToReturn = Windows.Forms.DialogResult.No
        End Try

        Return dialogResultToReturn
    End Function
#End Region

#Region "Methods"

    ''' <summary>
    ''' Uncheck all elements
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 13/01/2012
    ''' </remarks>
    Private Sub UncheckAllSampleTypes()
        Try

            For i As Integer = 0 To SampleTypeCheckList.Items.Count - 1
                SampleTypeCheckList.SetItemChecked(i, False)
            Next i


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UncheckAllSampleTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Hide sample type panel
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 11/01/2012
    ''' </remarks>
    Private Sub HideSampleTypePanel()
        Try
            SampleTypeCheckList.Visible = False
            'SampleTypePanel.Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " HideSampleTypePanel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Show the plus icon indicating if there are more than one sample type,
    ''' in case there is only one the icon desapear.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 19/11/2010
    ''' </remarks>
    Private Sub ShowPlusSampleTypeIcon()
        Try
            'Validate if there are more than one test sample
            If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 1 Then
                'TR 26/11/2010 -Create the list with the sample type to show on tooltip
                Dim myTestSampleList As String = ""
                For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples.Rows
                    myTestSampleList &= TestSampleRow.SampleType & " " & vbCrLf
                Next

                'bsScreenToolTips.SetToolTip(SampleTypePlus, myTestSampleList.TrimEnd())
                bsScreenToolTips.SetToolTip(SampleTypePlus2, myTestSampleList.TrimEnd())

                If PlusIconPath <> "" Then
                    If SelectedSampleTypeCombo.Enabled And SelectedSampleTypeCombo.Visible Then
                        SampleTypePlus2.ImageLocation = PlusIconPath
                    Else
                        SampleTypePlus2.ImageLocation = ""
                    End If
                End If
            Else
                SampleTypePlus2.ImageLocation = ""
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ShowPlusSampleTypeIcon ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the elements to be delete have any dependencies
    ''' search on the profiles and the Calculated test.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 24/11/2010
    ''' </remarks>
    Private Function ValidateDependenciesOnDeletedElemets() As DialogResult
        Dim myResult As New DialogResult
        Try
            'Validate if there are test to delete.
            If DeletedTestProgramingList.Count > 0 Then
                Dim myTestDelegate As New TestsDelegate
                Dim myGlobalDataTO As New GlobalDataTO()

                'Dim preloadedDataConfig As New PreloadedMasterDataDelegate()

                myGlobalDataTO = myTestDelegate.ValidatedDependencies(DeletedTestProgramingList)
                If Not myGlobalDataTO.HasError Then
                    Dim myDependeciesElementsDS As New DependenciesElementsDS
                    myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)
                    If Not myDependeciesElementsDS Is Nothing AndAlso myDependeciesElementsDS.DependenciesElements.Count > 0 Then
                        'Filter the affected elemente for not showing duplicate elements.
                        Using AffectedElement As New IWarningAfectedElements()
                            AffectedElement.AffectedElements = myDependeciesElementsDS
                            AffectedElement.ShowDialog()
                            myResult = AffectedElement.DialogResult
                        End Using

                    Else
                        'TR 26/11/2010 -Set the value to the result variable.
                        myResult = Windows.Forms.DialogResult.OK
                    End If
                End If
            Else
                myResult = Windows.Forms.DialogResult.OK
            End If
            Return myResult
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ValidateDependenciesOnDeletedElemets ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Function

    ''' <summary>
    ''' After save set the internal DataSet flags to the proper value (IsNew = False,...)
    ''' </summary>
    ''' <remarks>AG 12/01/2011</remarks>
    Private Sub SetToFalseInternalDataSetsFlags()
        Try
            '
            'SelectedTestDS (Set NewTest to FALSE)
            Dim myNewTest As New List(Of TestsDS.tparTestsRow)
            myNewTest = (From a In SelectedTestDS.tparTests _
                         Where a.NewTest = True Select a).ToList

            For Each row As TestsDS.tparTestsRow In myNewTest
                row.NewTest = False
            Next
            If myNewTest.Count > 0 Then SelectedTestDS.AcceptChanges()

            '
            'SelectedTestSamplesDS (Set IsNew to FALSE)
            Dim myNewTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)
            myNewTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples _
                         Where a.IsNew = True Select a).ToList

            For Each row As TestSamplesDS.tparTestSamplesRow In myNewTestSamples
                row.IsNew = False
            Next
            If myNewTestSamples.Count > 0 Then SelectedTestSamplesDS.AcceptChanges()

            '
            'SelectedTestReagentsVolumesDS As New TestReagentsVolumesDS() (Set IsNew to FALSE)
            Dim myNewTestReagentVolumes As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
            myNewTestReagentVolumes = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                        Where a.IsNew = True Select a).ToList

            For Each row As TestReagentsVolumesDS.tparTestReagentsVolumesRow In myNewTestReagentVolumes
                row.IsNew = False
            Next
            If myNewTestReagentVolumes.Count > 0 Then SelectedTestReagentsVolumesDS.AcceptChanges()

            '
            'SelectedReagentsDS As New ReagentsDS() (Set IsNew to FALSE)
            Dim myNewReagents As New List(Of ReagentsDS.tparReagentsRow)
            myNewReagents = (From a In SelectedReagentsDS.tparReagents _
                                        Where a.IsNew = True Select a).ToList

            For Each row As ReagentsDS.tparReagentsRow In myNewReagents
                row.IsNew = False
            Next
            If myNewReagents.Count > 0 Then SelectedReagentsDS.AcceptChanges()

            '
            'SelectedTestReagentsDS As New TestReagentsDS() (Set IsNew to FALSE)
            Dim myNewTestReagents As New List(Of TestReagentsDS.tparTestReagentsRow)
            myNewTestReagents = (From a In SelectedTestReagentsDS.tparTestReagents _
                                        Where a.IsNew = True Select a).ToList

            For Each row As TestReagentsDS.tparTestReagentsRow In myNewTestReagents
                row.IsNew = False
            Next
            If myNewTestReagents.Count > 0 Then SelectedTestReagentsDS.AcceptChanges()

            '
            'UpdatedCalibratorsDS As New CalibratorsDS (Set IsNew to FALSE)
            Dim myNewCalibrators As New List(Of CalibratorsDS.tparCalibratorsRow)
            myNewCalibrators = (From a In UpdatedCalibratorsDS.tparCalibrators _
                                        Where a.IsNew = True Select a).ToList

            For Each row As CalibratorsDS.tparCalibratorsRow In myNewCalibrators
                row.IsNew = False
            Next
            If myNewCalibrators.Count > 0 Then UpdatedCalibratorsDS.AcceptChanges()

            '
            'UpdatedTestCalibratorDS As New TestCalibratorsDS (Set IsNew to FALSE)
            Dim myNewTestCalibrators As New List(Of TestCalibratorsDS.tparTestCalibratorsRow)
            myNewTestCalibrators = (From a In UpdatedTestCalibratorDS.tparTestCalibrators _
                                        Where a.IsNew = True Select a).ToList

            For Each row As TestCalibratorsDS.tparTestCalibratorsRow In myNewTestCalibrators
                row.IsNew = False
            Next
            If myNewTestCalibrators.Count > 0 Then UpdatedTestCalibratorDS.AcceptChanges()

            'UpdatedTestCalibratorValuesDS As New TestCalibratorValuesDS (Set IsNew to FALSE)
            Dim myNewTestCalibratorsValues As New List(Of TestCalibratorValuesDS.tparTestCalibratorValuesRow)
            myNewTestCalibratorsValues = (From a In UpdatedTestCalibratorValuesDS.tparTestCalibratorValues _
                                        Where a.IsNew = True Select a).ToList

            For Each row As TestCalibratorValuesDS.tparTestCalibratorValuesRow In myNewTestCalibratorsValues
                row.IsNew = False
            Next
            If myNewTestCalibratorsValues.Count > 0 Then UpdatedTestCalibratorValuesDS.AcceptChanges()

            'SelectedTestRefRangesDS As New TestRefRangesDS
            'Set IsNew to FALSE
            Dim myTestRefRangesList As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            myTestRefRangesList = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                        Where a.IsNew = True Select a).ToList

            For Each row As TestRefRangesDS.tparTestRefRangesRow In myTestRefRangesList
                row.IsNew = False
            Next

            'Remove IsDeleted
            myTestRefRangesList = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                            Where a.IsDeleted = True Select a).ToList

            For Each row As TestRefRangesDS.tparTestRefRangesRow In myTestRefRangesList
                row.Delete()
            Next
            SelectedTestRefRangesDS.AcceptChanges()


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " SetToFalseInternalDataSetsFlags ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Search the selected test on the test list.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 17/11/2010
    ''' </remarks>
    Private Sub QuerySelectedTest()
        Try
            SelectedTestDS.tparTests.AcceptChanges()

            If clearSelection Then
                Exit Sub
            End If

            If TestListView.SelectedItems.Count = 1 Then
                'TR 27/05/2010 -Here is for validated if the test is in use or not to enable Edition or not.
                If CBool(TestListView.SelectedItems(0).Tag.ToString()) = True Then
                    isCurrentTestInUse = True
                Else
                    isCurrentTestInUse = False
                End If
                'TR 27/05/2010 -END

                If TestListView.SelectedItems(0).SubItems.Count = 2 Then ' TR 07/10/2011 validate the subitem.
                    'TR 12/09/2011 -Validate if is Preloaded Test.
                    If CBool(TestListView.SelectedItems(0).SubItems(1).Text) Then
                        isPreloadedTest = True
                    Else
                        isPreloadedTest = False
                    End If
                    'TR 12/09/2011 -END.
                End If

                If SelectedTestDS.tparTests.Rows.Count > 0 Then
                    Dim TreeViewSelectIndex As Integer = -1
                    If Not String.Compare(TestListView.SelectedItems(0).Name, SelectedTestDS.tparTests(0).TestID.ToString(), False) = 0 Then
                        TreeViewSelectIndex = CInt(TestListView.SelectedItems(0).Name)
                        If EditionMode And (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)

                            If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = _
                                                                                        Windows.Forms.DialogResult.Yes) Then
                                Dim EnableControls As Boolean = True
                                'TR 01/03/2011 -Validata calibrator data on DB

                                If Not NewTest AndAlso Not ValidateCalibratorDataCompleted(SelectedTestDS.tparTests(0).TestID, _
                                                                            SelectedSampleTypeCombo.SelectedValue.ToString()) Then

                                    TestListView.SelectedItems.Clear()
                                    TestListView.Items(SelectedTestDS.tparTests(0).TestID.ToString()).Selected = True
                                    TreeViewSelectIndex = CInt(TestListView.SelectedItems(0).Name)

                                    BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                    EnableControls = False
                                Else
                                    ChangesMade = False
                                    EditionMode = False
                                    'TR 22/07/2013 -BUG #1229.
                                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                                    UpdatedCalibratorsDS.tparCalibrators.Clear()
                                    UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                                    LocalDeleteControlTOList.Clear()
                                    'TR 22/07/2013 -BUG #1229-END

                                End If
                                If TestListView.SelectedItems.Count > 0 Then
                                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear() 'TR 01/03/2011 -Commented
                                    BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
                                    'Validate if enable controls.
                                    If EnableControls Then
                                        EnableDisableControls(TestDetailsTabs.Controls, False, True)
                                    End If
                                End If

                                SelectedSampleTypeCombo.Enabled = EnableControls 'TR 22/07/2013 BUG #1229.

                            Else
                                If Not NewTest Then 'AG 12/01/2011
                                    'TR 17/11/2010        
                                    clearSelection = True
                                    TestListView.SelectedItems.Clear()
                                    clearSelection = False
                                    'TR 30/11/2010  -Set the value in string mode to get the elements name and not the position
                                    'because the position can change an not be recalculate.
                                    TestListView.Items(currentTestViewEditionItem.ToString()).Selected = True
                                    TestListView.Select()
                                Else
                                    TestListView.SelectedItems.Clear()
                                End If 'AG 12/01/2011

                            End If

                        Else
                            EditionMode = False
                            ChangesMade = False
                            bsTestRefRanges.isEditing = False 'SG 06/09/2010
                            'EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
                            EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
                            CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString()) 'TR 18/11/210 
                        End If
                    End If
                Else
                    If Not SelectedSampleTypeCombo.SelectedValue Is Nothing Then
                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SelectedSampleTypeCombo.SelectedValue.ToString())
                    Else
                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
                    End If
                End If

            ElseIf TestListView.SelectedItems.Count > 1 Then
                EditButton.Enabled = False
                isCurrentTestInUse = False
                CopyTestButton.Enabled = False 'TR 10/01/2012 Disable copyTestbutton on multiselection.
                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869

                'AG 09/07/2010
                For Each mySelectedItem As ListViewItem In TestListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.Tag.ToString()) = True) Then
                        isCurrentTestInUse = True
                        Exit For
                    End If

                    'TR 12/09/2011 -Validate if is Preloaded Test.
                    If CBool(mySelectedItem.SubItems(1).Text) Then
                        isPreloadedTest = True
                        Exit For
                    Else
                        isPreloadedTest = False
                    End If

                Next mySelectedItem

                'TR 12/09/2011 -Validate if is Preloaded Test.
                If isCurrentTestInUse OrElse isPreloadedTest Then DeleteButton.Enabled = False Else DeleteButton.Enabled = True
                'END AG 09/07/2010
                'TR 12/09/2011
                SetEmptyValues() ' dl 14/04/2010 7.13, 7.14, 7.15
            End If

            ' DL 14/07/2010 7.51b
            If EditionMode = True Then
                'SelectedSampleTypeCombo.Visible = False ' TR 10/05/2011 -Commented
            Else
                SelectedSampleTypeCombo.Visible = True
            End If
            ' END DL 14/07/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " QuerySelectedTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub CancelEdition()
        Try ' Validate if there was any change

            SampleTypeCheckList.Visible = False 'DL 16/01/2012 

            BsErrorProvider1.Clear() 'TR 10/01/2012
            If (ChangesMade OrElse bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = _
                                                                     Windows.Forms.DialogResult.Yes) Then
                    ChangesMade = False
                    EditionMode = False
                    bsTestRefRanges.ChangesMade = False
                    CopyTestData = False 'TR 10/12/2010
                    'BsErrorProvider1.Clear() 'AG 03/12/2010
                    SelectedSampleTypeCombo.Enabled = True 'TR 26/01/2012

                    IsSampleTypeChanged = False
                    SelectedSampleTypeCombo.Enabled = Not IsSampleTypeChanged

                    'TR 8/11/2010 -Validate if there are selected test on the list view.
                    If TestListView.SelectedItems.Count > 0 Then
                        'TR 18/11/2010 -Clear all the updated structures.
                        UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        UpdatedCalibratorsDS.tparCalibrators.Clear()
                        UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                        'TR 18/11/2010 -END.

                        LocalDeleteControlTOList.Clear() 'TR 24/05/2011

                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer))

                        'TR 28/02/2011 -Validate calibrator values before disabling the controls
                        If ValidateCalibratorDataCompleted(CInt(TestListView.SelectedItems(0).Name), _
                                                           SelectedSampleTypeCombo.SelectedValue.ToString) Then

                            EnableDisableControls(TestDetailsTabs.Controls, False, True)
                        Else
                            BsErrorProvider1.SetError(MultipleCalibRadioButton, _
                                            GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                            UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            ChangesMade = True
                            EditionMode = True
                            Exit Try
                        End If
                        'TR 28/02/2011 -END 
                    Else 'TR 17/12/2010
                        If TestListView.Items.Count > 0 Then
                            TestListView.Items(0).Selected = True
                            QuerySelectedTest() 'TR 17/12/2010
                        End If

                    End If
                    'TR 8/11/2010 -END

                    'TR 18/11/2010 -Make visible the button.
                    SelectedSampleTypeCombo.Visible = True
                    'TR 18/11/2010 -END. 

                    'TR 17/11/2010 -Change the newtest to false
                    If NewTest Then
                        NewTest = False
                    End If
                    'TR 17/11/2010 -END.
                    'End If
                End If
            Else
                ChangesMade = False
                EditionMode = False
                BsErrorProvider1.Clear() 'TR 10/12/2010
                CopyTestData = False 'TR 25/01/2012
                SelectedSampleTypeCombo.Enabled = True  'TR 26/01/2012

                IsSampleTypeChanged = False
                SelectedSampleTypeCombo.Enabled = Not IsSampleTypeChanged

                'TR 8/11/2010 -Validate if there are selected test on the list view.
                If TestListView.SelectedItems.Count > 0 Then
                    'TR 18/11/2010 -Clear all the updated structures.
                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                    UpdatedCalibratorsDS.tparCalibrators.Clear()
                    UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                    'TR 18/11/2010 -END.

                    LocalDeleteControlTOList.Clear() 'TR 24/05/2011

                    BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
                    EnableDisableControls(TestDetailsTabs.Controls, False, True)
                Else
                    ClearAllControls()
                End If
                'TR 8/11/2010 -END

                'TR 18/11/2010 -Make visible the button.
                SelectedSampleTypeCombo.Visible = True
                'TR 18/11/2010 -END. 

                'TR 17/11/2010 -Change the newtest to false
                If NewTest Then
                    NewTest = False
                End If
                'TR 17/11/2010 -END.

                'AG 20/12/2010
                EnableDisableControls(TestDetailsTabs.Controls, False, True)
                If Not TestListView.SelectedItems.Count > 0 AndAlso TestListView.Items.Count > 0 Then
                    TestListView.Items(0).Selected = True
                    QuerySelectedTest()
                End If
                'AG 20/12/2010

            End If

            'TR 19/11/2010 
            If Not AnalysisModeCombo.SelectedValue Is Nothing Then
                CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())
            End If
            'TR 19/11/2010 -END

            AddedSampleType = String.Empty
            TestListView.Focus()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CancelEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill Sample Type List
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/01/2012
    ''' </remarks>
    Private Sub FillSampleTypeList()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMasteDataDS As New MasterDataDS()
            Dim myMasterDataDelegate As New MasterDataDelegate()

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")

            If Not myGlobalDataTO.HasError Then
                myMasteDataDS = CType(myGlobalDataTO.SetDatos, MasterDataDS)
                Dim qSampleTypes As List(Of MasterDataDS.tcfgMasterDataRow)
                Dim qSelectedSample As New List(Of TestSamplesDS.tparTestSamplesRow)
                Dim selectedType As Boolean = False
                'order the result.
                qSampleTypes = (From a In myMasteDataDS.tcfgMasterData _
                             Order By a.Position _
                             Select a).ToList()

                For Each SampleRow As MasterDataDS.tcfgMasterDataRow In qSampleTypes
                    Dim mySampleType As String = SampleRow.ItemID
                    qSelectedSample = (From b In SelectedTestSamplesDS.tparTestSamples _
                                        Where b.SampleType = mySampleType _
                                        Select b).ToList()
                    If qSelectedSample.Count > 0 Then
                        selectedType = True
                    End If
                    SampleTypeCheckList.Items.Add(SampleRow.ItemID, selectedType)
                    selectedType = False
                Next

            Else
                ShowMessage(Me.Name, myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillSampleTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillSampleTypeCombo", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load screen status control (NOTE: without using database)
    ''' </summary>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="pTestInUse"></param>
    ''' <param name="pWSStatus"></param>
    ''' <remarks>Created by AG 09/03/10 (tested OK)</remarks>
    Private Sub LoadScreenStatus(ByVal pWorkSessionID As String, Optional ByVal pWSStatus As String = "", Optional ByVal pTestInUse As Boolean = False)
        Try
            'pTestInUse = True   'Line active only during validation cases
            Dim existWorkSession As Boolean = False
            If Not pWorkSessionID Is Nothing AndAlso pWorkSessionID.Trim <> "" Then existWorkSession = True

            Select Case existWorkSession
                Case True
                    Select Case pWSStatus
                        Case "PENDING"
                            If Not pTestInUse Then
                                TestListView.Enabled = True
                                AddButton.Enabled = True
                                DeleteButton.Enabled = False
                                isSortTestListAllowed = False
                                SaveButton.Enabled = True
                                'TR 08/11/2010
                                CancelButton.Enabled = True

                                'ExitButton1.Enabled = True
                                ExitButton.Enabled = True
                                '                                PrintTestButton.Enabled = True dl 11/05/2012
                                DeleteSampleTypeButton.Enabled = True 'TR 25/04/2012

                                isEditTestAllowed = True
                                EditButton.Enabled = isEditTestAllowed

                                CopyTestButton.Enabled = True 'TR 09/01/2012 Enable Copy test Button.
                                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869

                            Else
                                'TestListView.Enabled = False
                                TestListView.Enabled = True
                                AddButton.Enabled = True
                                DeleteButton.Enabled = False
                                isSortTestListAllowed = False
                                SaveButton.Enabled = False

                                'TR 08/11/2010
                                CancelButton.Enabled = False

                                'ExitButton1.Enabled = True
                                ExitButton.Enabled = True
                                '                                PrintTestButton.Enabled = True dl 11/05/2012
                                DeleteSampleTypeButton.Enabled = False

                                isEditTestAllowed = False
                                EditButton.Enabled = isEditTestAllowed

                                CopyTestButton.Enabled = True 'TR 09/01/2012 Enable Copy test Button.
                                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                            End If

                        Case "RUNNING", "STANDBY", "SAMPLINGSTOP", "FINISHED", "ABORTED"
                            TestListView.Enabled = False
                            AddButton.Enabled = True
                            DeleteButton.Enabled = False
                            isSortTestListAllowed = False
                            SaveButton.Enabled = False

                            'TR 08/11/2010
                            CancelButton.Enabled = False

                            'ExitButton1.Enabled = True
                            ExitButton.Enabled = True
                            '                            PrintTestButton.Enabled = True dl 11/05/2012
                            DeleteSampleTypeButton.Enabled = False

                            isEditTestAllowed = False
                            EditButton.Enabled = isEditTestAllowed

                            CopyTestButton.Enabled = True 'TR 09/01/2012 Enable Copy test Button.
                            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                    End Select

                Case False
                    If Not pTestInUse Then
                        TestListView.Enabled = True
                        AddButton.Enabled = True
                        'DeleteButton.Enabled = True 'TR commmented and use the other line.
                        If existTetsInUse Then DeleteButton.Enabled = False Else DeleteButton.Enabled = True ' TR 28/05/2010
                        isSortTestListAllowed = True
                        SaveButton.Enabled = True

                        'TR 08/11/2010
                        CancelButton.Enabled = True

                        'ExitButton1.Enabled = True
                        ExitButton.Enabled = True
                        '                        PrintTestButton.Enabled = True dl 11/05/2012
                        DeleteSampleTypeButton.Enabled = True

                        isEditTestAllowed = True
                        EditButton.Enabled = isEditTestAllowed

                        CopyTestButton.Enabled = True 'TR 09/01/2012 Enable Copy test Button.
                        BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                    End If
            End Select

            'TR 12/09/2011 -Validate if  is Preloade Test to disable Delete button.
            If isPreloadedTest Then DeleteButton.Enabled = False Else DeleteButton.Enabled = True

            If CurrentUserLevel = "OPERATOR" Then
                'Read only
                AddButton.Enabled = False
                DeleteButton.Enabled = False
                isSortTestListAllowed = False
                SaveButton.Enabled = False
                'TR 08/11/2010
                CancelButton.Enabled = False
                DeleteSampleTypeButton.Enabled = False
                isEditTestAllowed = False
                EditButton.Enabled = False
                'TR 29/03/2012
                CopyTestButton.Enabled = False
                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                EditButton.Enabled = False
                'TR 29/03/2012 -END


            ElseIf CurrentUserLevel = "SUPERVISOR" Then
                'TR 29/03/2012 -Validate the amount of Test that Supervisor can create 
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myTestDelegate As New TestsDelegate
                'Get the Created Test
                myGlobalDataTO = myTestDelegate.ReadByPreloadedTest(Nothing, False)

                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    Dim myCreatedTest As Integer = DirectCast(myGlobalDataTO.SetDatos, TestsDS).tparTests.Count
                    Dim MyGlobalBase As New GlobalBase
                    If myCreatedTest >= MyGlobalBase.GetSessionInfo.MaxTestsNumber Then
                        'disable add button and copy button 
                        CopyTestButton.Enabled = False
                        BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                        AddButton.Enabled = False
                    End If
                End If
                'TR 29/03/2012 -END
            End If

            'General screen flow (actions allowed or not depending on screen status)
            If EditionMode Then
                '                PrintTestButton.Enabled = False dl 11/05/2012
                DeleteButton.Enabled = False
            Else
                SaveButton.Enabled = False
                'TR 08/11/2010
                CancelButton.Enabled = False
                DeleteSampleTypeButton.Enabled = False
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " LoadScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Config. the testlistView parameter.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 18/02/2010.
    ''' Modified by: PG 08/10/2010 - Add the LanguageID parameter for change language labels 
    ''' Modified by: RH 17/11/2011 Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter.
    ''' </remarks>
    Private Sub PrepareTestListView()
        'Private Sub PrepareTestListView()
        Try
            'Initialize 
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Initialization of Test Profiles list
            TestListView.Items.Clear()
            TestListView.Alignment = ListViewAlignment.Left
            TestListView.FullRowSelect = True
            TestListView.HeaderStyle = ColumnHeaderStyle.Clickable
            TestListView.MultiSelect = True  'TR 21/06/2010 -Set value to True, it wass commented before.
            TestListView.Scrollable = True
            TestListView.View = View.Details
            TestListView.HideSelection = False
            'TestListView.Sorting = SortOrder.Ascending

            ' PG 08/10/2010
            'TestListView.Columns.Add("Test Name", -2, HorizontalAlignment.Left)
            TestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", currentLanguage), -2, HorizontalAlignment.Left)
            TestListView.Columns(0).Width = 200 'TR 17/04/2012
            ' PG 08/10/2010
            TestListView.Columns.Add("TestName", 0, HorizontalAlignment.Left)

            'TR 12/09/2011 -Column use to indicate if is FactoryTest.
            TestListView.Columns.Add("FactoryTest", 0, HorizontalAlignment.Left)

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareTestListView ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the data for Curve frame combos
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 14/06/2010
    ''' </remarks>
    Private Sub PrepareCurveComboControls()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedDataDelegate As New PreloadedMasterDataDelegate
            Dim myPreloadedDataDS As New PreloadedMasterDataDS
            'Load the Curve types values.
            myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.CURVE_TYPES)
            If Not myGlobalDataTO.HasError Then
                myPreloadedDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qPreloadedMasterData As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'sort the result by the position.
                qPreloadedMasterData = (From a In myPreloadedDataDS.tfmwPreloadedMasterData _
                                        Select a Order By a.Position).ToList()
                CurveTypeCombo.DataSource = qPreloadedMasterData
                CurveTypeCombo.DisplayMember = "FixedItemDesc"
                CurveTypeCombo.ValueMember = "ItemID"
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

            'Load the Curve Axis Type
            myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.CURVE_AXIS_TYPES)
            If Not myGlobalDataTO.HasError Then
                myPreloadedDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qPreloadedMasterData As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'sort the result by the position.
                qPreloadedMasterData = (From a In myPreloadedDataDS.tfmwPreloadedMasterData _
                                        Select a Order By a.Position).ToList()

                If qPreloadedMasterData.Count > 0 Then
                    'X AXIS
                    XAxisCombo.DataSource = qPreloadedMasterData
                    XAxisCombo.DisplayMember = "FixedItemDesc"
                    XAxisCombo.ValueMember = "ItemID"

                    'Y AXIS
                    Dim qPreloadedMasterData2 As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    qPreloadedMasterData2.AddRange(qPreloadedMasterData)
                    YAxisCombo.DataSource = qPreloadedMasterData2
                    YAxisCombo.DisplayMember = "FixedItemDesc"
                    YAxisCombo.ValueMember = "ItemID"
                Else
                    ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                End If
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareCurveComboControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Setup the Concentration GridView.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 14/06/2010
    ''' Modified by: PG 08/10/2010
    '''              RH 17/11/2011 - Removed pLanguageID, because it is a class property/field. There is not need to pass it as a parameter
    '''              SA 15/11/2012 - Besides set all visible columns to ReadOnly, set the grid as ReadOnly to avoid focus in it
    ''' </remarks>
    Private Sub PrepareConcentrationGridView()
        'Private Sub PrepareConcentrationGridView()
        Try
            'Initialize 
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ConcentrationGridView.AutoGenerateColumns = False
            ConcentrationGridView.Enabled = False
            ConcentrationGridView.ReadOnly = True
            ConcentrationGridView.TabStop = False
            ConcentrationGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            ConcentrationGridView.AutoSize = False

            ConcentrationGridView.Columns.Clear()

            'Add(columns)
            ConcentrationGridView.Columns.Add("CalibratorNum", "N")
            ConcentrationGridView.Columns("CalibratorNum").Width = 50
            ConcentrationGridView.Columns("CalibratorNum").DataPropertyName = "CalibratorNum"
            ConcentrationGridView.Columns("CalibratorNum").ReadOnly = True
            ConcentrationGridView.Columns("CalibratorNum").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            ConcentrationGridView.Columns("CalibratorNum").DefaultCellStyle.ForeColor = Color.DarkGray

            ConcentrationGridView.Columns.Add("TheoricalConcentration", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", currentLanguage))
            ConcentrationGridView.Columns("TheoricalConcentration").Width = 130
            ConcentrationGridView.Columns("TheoricalConcentration").DataPropertyName = "TheoricalConcentration"
            ConcentrationGridView.Columns("TheoricalConcentration").ReadOnly = True
            ConcentrationGridView.Columns("TheoricalConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("TheoricalConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("TheoricalConcentration").DefaultCellStyle.ForeColor = Color.DarkGray

            ConcentrationGridView.Columns.Add("KitConcentrationRelation", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", currentLanguage))
            ConcentrationGridView.Columns("KitConcentrationRelation").Width = 112
            ConcentrationGridView.Columns("KitConcentrationRelation").DataPropertyName = "KitConcentrationRelation"
            ConcentrationGridView.Columns("KitConcentrationRelation").ReadOnly = True
            ConcentrationGridView.Columns("KitConcentrationRelation").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            ConcentrationGridView.Columns("KitConcentrationRelation").DefaultCellStyle.ForeColor = Color.DarkGray

            ConcentrationGridView.Columns.Add("TestCalibratorID", "TestCalibratorID")
            ConcentrationGridView.Columns("TestCalibratorID").Width = 50
            ConcentrationGridView.Columns("TestCalibratorID").DataPropertyName = "TestCalibratorID"
            ConcentrationGridView.Columns("TestCalibratorID").Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareConcentrationGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' fill the ConcentrationGridView with all the 
    ''' concentration values.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 14/06/2010
    ''' </remarks>
    Private Sub FillConcentrationValuesList(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO

            Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate()
            Dim qUpdateTestCalibVal As New List(Of TestCalibratorValuesDS.tparTestCalibratorValuesRow)
            Dim qUpdateTestCalibList As New List(Of TestCalibratorsDS.tparTestCalibratorsRow)

            'Get all the concentration values  for the selected test on DB.
            myGlobalDataTO = myTestCalibratorValuesDelegate.GetTestCalibratorValuesByTestIDSampleType(Nothing, pTestID, pSampleType)

            If Not myGlobalDataTO.HasError Then
                SelectedTestCalibratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                If SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Count > 0 Then
                    'validate if data is on update concentration values.
                    qUpdateTestCalibVal = (From a In UpdatedTestCalibratorValuesDS.tparTestCalibratorValues _
                                        Where a.TestCalibratorID = SelectedTestCalibratorValuesDS.tparTestCalibratorValues(0).TestCalibratorID _
                                        Select a).ToList()
                    'if value found then replace selecte test calibrator
                    If qUpdateTestCalibVal.Count > 0 Then
                        SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        'TR 09/11/2010 -Add for process 
                        For Each testcalValRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In qUpdateTestCalibVal
                            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(testcalValRow)
                        Next
                        'TR 09/11/2010 -END.
                        'SelectedTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(qUpdateTestCalibVal.First())
                    Else
                        'TR 03/08/2010
                        'search on internal structures if has an asigned calibrator.
                        qUpdateTestCalibList = (From a In UpdatedTestCalibratorDS.tparTestCalibrators _
                                                Where a.TestID = pTestID And a.SampleType = pSampleType _
                                                Select a).ToList()

                        If qUpdateTestCalibList.Count > 0 Then
                            'Get the values by the TestCalibratorID found on the UpdatedtestCalibDS
                            qUpdateTestCalibVal = (From a In UpdatedTestCalibratorValuesDS.tparTestCalibratorValues _
                                            Where a.TestCalibratorID = qUpdateTestCalibList.First().TestCalibratorID _
                                            Select a).ToList()

                            If qUpdateTestCalibVal.Count > 0 Then
                                SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                                'load all value found
                                For Each TestCaliValRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In qUpdateTestCalibVal
                                    SelectedTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(TestCaliValRow)
                                Next

                            End If
                        End If
                        'TR 03/08/2010
                    End If
                Else
                    'search on internal structures if has an asigned calibrator.
                    qUpdateTestCalibList = (From a In UpdatedTestCalibratorDS.tparTestCalibrators _
                                            Where a.TestID = pTestID And a.SampleType = pSampleType _
                                            Select a).ToList()

                    If qUpdateTestCalibList.Count > 0 Then
                        'Get the values by the TestCalibratorID found on the UpdatedtestCalibDS
                        qUpdateTestCalibVal = (From a In UpdatedTestCalibratorValuesDS.tparTestCalibratorValues _
                                        Where a.TestCalibratorID = qUpdateTestCalibList.First().TestCalibratorID _
                                        Select a).ToList()

                        If qUpdateTestCalibVal.Count > 0 Then
                            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            'load all value found
                            For Each TestCaliValRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In qUpdateTestCalibVal
                                SelectedTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(TestCaliValRow)
                            Next

                        End If
                    End If
                End If


                ConcentrationGridView.DataSource = SelectedTestCalibratorValuesDS.tparTestCalibratorValues
                BsCalibNumberTextBox.Text = ConcentrationGridView.Rows.Count.ToString()


                If ConcentrationGridView.SelectedRows.Count > 0 Then
                    ConcentrationGridView.SelectedRows(0).Selected = False
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillConcentrationValuesList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Bind Calibrations Curve controls with the selecte calibrator
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 14/06/2010
    ''' </remarks>
    Private Sub BindCalibrationCurveControls(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate

            Dim qUpdateTestSampleCalibList As New List(Of TestCalibratorsDS.tparTestCalibratorsRow)

            'Get Test calibrator data.
            myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorData(Nothing, pTestID, pSampleType)

            If Not myGlobalDataTO.HasError Then
                SelectedTestSampleCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestSampleCalibratorDS)
                qUpdateTestSampleCalibList = (From a In UpdatedTestCalibratorDS.tparTestCalibrators _
                                             Where a.TestID = pTestID And a.SampleType = pSampleType _
                                             Select a).ToList()

                If qUpdateTestSampleCalibList.Count > 0 Then
                    SelectedTestSampleCalibratorDS.tparTestCalibrators.Clear()
                    SelectedTestSampleCalibratorDS.tparTestCalibrators.ImportRow(qUpdateTestSampleCalibList.First())
                    SelectedTestSampleCalibratorDS.AcceptChanges()

                    'Search on the update calibrator structure. to coomplete the calibrator info data
                    Dim qUpdateCalibList As New List(Of CalibratorsDS.tparCalibratorsRow)
                    qUpdateCalibList = (From a In UpdatedCalibratorsDS.tparCalibrators _
                                       Where a.CalibratorID = qUpdateTestSampleCalibList.First().CalibratorID _
                                       Select a).ToList
                    'Set the values 
                    If qUpdateCalibList.Count > 0 Then
                        SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorName = qUpdateCalibList.First().CalibratorName
                        SelectedTestSampleCalibratorDS.tparTestCalibrators(0).LotNumber = qUpdateCalibList.First().LotNumber
                        SelectedTestSampleCalibratorDS.tparTestCalibrators(0).ExpirationDate = qUpdateCalibList.First().ExpirationDate
                    End If
                Else
                    UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                    For Each testSampleRow As TestSampleCalibratorDS.tparTestCalibratorsRow In SelectedTestSampleCalibratorDS.tparTestCalibrators.Rows
                        UpdatedTestCalibratorDS.tparTestCalibrators.ImportRow(testSampleRow)
                    Next
                End If
            Else
                'TR 17/11/2010 -Search the data on the update structures.
                If myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING" Then
                    qUpdateTestSampleCalibList = (From a In UpdatedTestCalibratorDS.tparTestCalibrators _
                                             Where a.TestID = pTestID And a.SampleType = pSampleType _
                                             Select a).ToList()

                    If qUpdateTestSampleCalibList.Count > 0 Then
                        SelectedTestSampleCalibratorDS.tparTestCalibrators.Clear()
                        SelectedTestSampleCalibratorDS.tparTestCalibrators.ImportRow(qUpdateTestSampleCalibList.First())
                        SelectedTestSampleCalibratorDS.AcceptChanges()

                        'Search on the update calibrator structure. to coomplete the calibrator info data
                        Dim qUpdateCalibList As New List(Of CalibratorsDS.tparCalibratorsRow)
                        qUpdateCalibList = (From a In UpdatedCalibratorsDS.tparCalibrators _
                                           Where a.CalibratorID = qUpdateTestSampleCalibList.First().CalibratorID _
                                           Select a).ToList
                        'Set the values 
                        If qUpdateCalibList.Count > 0 Then
                            SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorName = qUpdateCalibList.First().CalibratorName
                            SelectedTestSampleCalibratorDS.tparTestCalibrators(0).LotNumber = qUpdateCalibList.First().LotNumber
                            SelectedTestSampleCalibratorDS.tparTestCalibrators(0).ExpirationDate = qUpdateCalibList.First().ExpirationDate
                        End If
                    Else
                        SelectedTestSampleCalibratorDS.tparTestCalibrators.Clear()
                    End If
                End If
                'TR 17/11/2010 -END.
            End If
            'Validate if the selecte calibrator type is experimental 
            If SelectedTestSampleCalibratorDS.tparTestCalibrators.Count > 0 AndAlso MultipleCalibRadioButton.Checked Then

                'Set the Curve Type.
                If Not SelectedTestSampleCalibratorDS.tparTestCalibrators(0).IsCurveTypeNull AndAlso _
                                SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveType <> "" Then
                    CurveTypeCombo.SelectedValue = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveType
                Else
                    CurveTypeCombo.SelectedIndex = -1
                End If
                'Set the Curve Growth .
                Select Case SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveGrowthType
                    Case "INC"
                        IncreasingRadioButton.Checked = True
                    Case "DEC"
                        DecreasingRadioButton.Checked = True
                    Case Else
                        IncreasingRadioButton.Checked = False
                        DecreasingRadioButton.Checked = False
                End Select
                'Set the X Axis value. 
                If Not SelectedTestSampleCalibratorDS.tparTestCalibrators(0).IsCurveAxisXTypeNull AndAlso _
                                SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveAxisXType <> "" Then
                    XAxisCombo.SelectedValue = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveAxisXType
                Else
                    XAxisCombo.SelectedIndex = -1
                End If
                'Set the Y Axis value.
                If Not SelectedTestSampleCalibratorDS.tparTestCalibrators(0).IsCurveAxisYTypeNull AndAlso _
                                String.Compare(SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveAxisYType, "", False) <> 0 Then
                    YAxisCombo.SelectedValue = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CurveAxisYType
                Else
                    YAxisCombo.SelectedIndex = -1
                End If

                If ConcentrationGridView.Rows.Count > 0 Then
                    BsCalibNumberTextBox.Enabled = True
                    BsCalibNumberTextBox.Text = ConcentrationGridView.Rows.Count.ToString()
                    BsCalibNumberTextBox.Enabled = False
                Else
                    BsCalibNumberTextBox.Clear()
                End If

                'TR 17/11/2010 -Bind calibrator info controls
                CalibratorNameTextBox.Enabled = True
                CalibratorNameTextBox.Text = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorName
                CalibratorNameTextBox.Enabled = False

                CalibratorLotTextBox.Enabled = True
                CalibratorLotTextBox.Text = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).LotNumber
                CalibratorLotTextBox.Enabled = False

                CalibratorExpirationDate.Enabled = True
                'Validate date is not null 
                If Not SelectedTestSampleCalibratorDS.tparTestCalibrators(0).IsExpirationDateNull Then
                    CalibratorExpirationDate.Text = SelectedTestSampleCalibratorDS.tparTestCalibrators(0).ExpirationDate.ToShortDateString()
                Else
                    CalibratorExpirationDate.Clear()
                End If
                'TR 17/11/2010 -END
                CalibratorExpirationDate.Enabled = False
            Else
                CurveTypeCombo.SelectedIndex = -1
                'Set the Curve Growth .
                IncreasingRadioButton.Checked = False
                DecreasingRadioButton.Checked = False
                'Set the X Axis value. 
                XAxisCombo.SelectedIndex = -1
                'Set the Y Axis value.
                YAxisCombo.SelectedIndex = -1
                BsCalibNumberTextBox.Clear() 'TR 01/07/2010
                'TR 17/11/2010
                CalibratorNameTextBox.Clear()
                CalibratorLotTextBox.Clear()
                CalibratorExpirationDate.Clear()
                'TR 17/11/2010
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " BindCalibrationCurveControls ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the TestListView with all the tests.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 18/02/2010
    ''' </remarks>
    Private Sub FillTestListView()
        Try
            Dim IconNameVar As String = ""
            Dim testIconList As New ImageList
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestDelegate As New TestsDelegate()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'get the test list.
            myGlobalDataTO = myTestDelegate.GetList(Nothing)
            'validate if there's any error.
            If Not myGlobalDataTO.HasError Then
                If Not myGlobalDataTO.SetDatos Is Nothing Then
                    Dim myTestDS As New TestsDS()
                    myTestDS = CType(myGlobalDataTO.SetDatos, TestsDS)
                    'Get the icon name.
                    myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.ICON_PATHS)

                    If Not myGlobalDataTO.HasError Then
                        myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                        For Each PreMasterRow As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In _
                                                                        myPreMasterDataDS.tfmwPreloadedMasterData.Rows
                            Select Case PreMasterRow.ItemID
                                Case "TESTICON", "INUSETEST", "USERTEST", "INUSUSTEST"
                                    If IO.File.Exists(MyBase.IconsPath & PreMasterRow.FixedItemDesc) Then
                                        testIconList.Images.Add(PreMasterRow.ItemID, _
                                                Image.FromFile(MyBase.IconsPath & PreMasterRow.FixedItemDesc))
                                    End If
                                    Exit Select
                                Case Else
                                    Exit Select
                            End Select
                        Next
                    End If
                    TestListView.Items.Clear()
                    TestListView.SmallImageList = testIconList
                    For Each TestRow As TestsDS.tparTestsRow In myTestDS.tparTests.Rows
                        If TestRow.InUse Then
                            If TestRow.PreloadedTest Then
                                IconNameVar = "INUSETEST"
                            Else
                                IconNameVar = "INUSUSTEST"
                            End If
                        Else
                            If TestRow.PreloadedTest Then
                                IconNameVar = "TESTICON"
                            Else
                                IconNameVar = "USERTEST"
                            End If

                        End If
                        'TestListView.Items.Add(TestRow.TestID.ToString(), TestRow.TestName.TrimEnd(), IconNameVar).Tag = TestRow.InUse
                        TestListView.Items.Add(TestRow.TestID.ToString(), TestRow.TestName.TrimEnd(), IconNameVar).SubItems.Add(TestRow.PreloadedTest.ToString())
                        TestListView.Items(TestRow.TestID.ToString()).Tag = TestRow.InUse
                    Next

                    'TR 27/05/2010 -If there is a test in use then Disable the Delete Button.
                    myTestDS.tparTests().DefaultView.RowFilter = "InUse = True"
                    If myTestDS.tparTests().DefaultView.Count > 0 Then
                        existTetsInUse = True 'TR 28/05/2010 - Indicate that exist one or serveral test inUse
                    Else
                        existTetsInUse = False  ' TR 28/05/2010 - Indicate there is no test in use
                    End If

                    'clear filter.
                    myTestDS.tparTests().DefaultView.RowFilter = ""
                    'TR(27/05/2010) END
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorMessage, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillTestListView ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Analysis Mode ComboBox. with the analysis 
    ''' modes defined on the preloaded master data.
    ''' SubTableID = ANALYSIS_MODES
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 18/02/2010.
    ''' </remarks>
    Private Sub FillAnalysisModeCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                    GlobalEnumerates.PreloadedMasterDataEnum.ANALYSIS_MODES)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qAnalysisMode As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order the analysis mode by the position number.
                qAnalysisMode = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                AnalysisModeCombo.DisplayMember = "FixedItemDesc"
                AnalysisModeCombo.ValueMember = "ItemID"
                AnalysisModeCombo.DataSource = qAnalysisMode
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillAnalysisModeCombo ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the blank types combo.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillBlankTypesCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                    GlobalEnumerates.PreloadedMasterDataEnum.BLANK_MODES)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qBlankTypes As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'Order the analysis mode by the position number.
                qBlankTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                Order By a.Position _
                                Select a).ToList()

                BlankTypesCombo.DisplayMember = "FixedItemDesc"
                BlankTypesCombo.ValueMember = "ItemID"
                BlankTypesCombo.DataSource = qBlankTypes
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillBlankTypesCombo ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Units ComboBox, with the difened unist on the table masterdata
    ''' SubTableID = TEST_UNITS
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillUnitsCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMasteDataDS As New MasterDataDS()
            Dim myMasterDataDelegate As New MasterDataDelegate()

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "TEST_UNITS")

            If Not myGlobalDataTO.HasError Then
                myMasteDataDS = CType(myGlobalDataTO.SetDatos, MasterDataDS)
                Dim qTestUnits As List(Of MasterDataDS.tcfgMasterDataRow)
                'order the result.
                qTestUnits = (From a In myMasteDataDS.tcfgMasterData _
                             Order By a.Position _
                             Select a).ToList()
                UnitsCombo.DataSource = qTestUnits
                UnitsCombo.DisplayMember = "FixedItemDesc"
                UnitsCombo.ValueMember = "ItemID"
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " FillUnitsCombo ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ' DL 11/01/2012. Ini
    '''' <summary>
    '''' fill the sample type combo
    '''' </summary>
    '''' <remarks>Modified by SG 21/07/2010</remarks>
    'Private Sub FillSampleTypeCombo()
    '    Try
    '        Dim qSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)

    '        qSampleTypes = GetSampleTypesByPosition()

    '        If qSampleTypes.Count > 0 Then
    '            SampleTypeCombo.DataSource = qSampleTypes
    '            SampleTypeCombo.DisplayMember = "ItemID"
    '            SampleTypeCombo.ValueMember = "ItemID"
    '        End If

    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillSampleTypeCombo ", EventLogEntryType.Error, _
    '                                                        GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub
    ' DL 11/01/2012. End

    ''' <summary>
    ''' get sample types by its position
    ''' </summary>
    ''' <remarks>Created by SG 20/07/2010</remarks>
    Private Function GetSampleTypesByPosition() As List(Of MasterDataDS.tcfgMasterDataRow)
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMasteDataDS As New MasterDataDS()
            Dim myMasterDataDelegate As New MasterDataDelegate()
            Dim qSampleType As New List(Of MasterDataDS.tcfgMasterDataRow)

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")

            If Not myGlobalDataTO.HasError Then
                myMasteDataDS = CType(myGlobalDataTO.SetDatos, MasterDataDS)

                'order the result.
                qSampleType = (From a In myMasteDataDS.tcfgMasterData _
                             Order By a.Position _
                             Select a).ToList()

                Return qSampleType
            Else
                Throw New Exception(myGlobalDataTO.ErrorCode)
                Return Nothing
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetSampleTypesByPosition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")

            Return Nothing
        End Try
    End Function

    '''' <summary>
    '''' returns the list of the current test samples
    '''' </summary>
    '''' <remarks>Created by SG 20/07/2010</remarks>
    'Private Function GetTestSampleList(ByVal pTestId As Integer) As List(Of TestSamplesDS.tparTestSamplesRow)
    '    Try
    '        Dim qTestSampleRows As New List(Of TestSamplesDS.tparTestSamplesRow)

    '        If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
    '            'Filter the SelectedTestSample by the TestID and SampleTye.
    '            qTestSampleRows = (From a In SelectedTestSamplesDS.tparTestSamples _
    '                              Where a.TestID = pTestId _
    '                              Select a).ToList()
    '        End If

    '        Return qTestSampleRows

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetTestSampleList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

    '        Return Nothing

    '    End Try
    'End Function

    '''' <summary>
    '''' returns the list of the current test samples
    '''' </summary>
    '''' <remarks>Created by SG 20/07/2010</remarks>
    'Private Function GetTestSampleListBySampleType(ByVal pTestId As Integer, ByVal pSampleType As String) As List(Of TestSamplesDS.tparTestSamplesRow)
    '    Try
    '        Dim qTestSampleRows As New List(Of TestSamplesDS.tparTestSamplesRow)

    '        If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
    '            'Filter the SelectedTestSample by the TestID and SampleTye.
    '            qTestSampleRows = (From a In SelectedTestSamplesDS.tparTestSamples _
    '                              Where a.TestID = pTestId And a.SampleType = pSampleType _
    '                              Select a).ToList()
    '        End If

    '        Return qTestSampleRows

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetTestSampleTypeList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

    '        Return Nothing

    '    End Try
    'End Function

    ''' <summary>
    ''' fill the sample type combo
    ''' </summary>
    ''' <remarks>dl 15/07/2010</remarks>
    Private Sub FillSelectedSampleTypeCombo()
        Try
            Dim qSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)
            qSampleTypes = GetSampleTypesByPosition()

            If qSampleTypes.Count > 0 Then
                SelectedSampleTypeCombo.DataSource = qSampleTypes
                SelectedSampleTypeCombo.DisplayMember = "ItemIDDesc"
                SelectedSampleTypeCombo.ValueMember = "ItemID"
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillSelectedSampleTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the reaction type combo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillReactionTypeCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                GlobalEnumerates.PreloadedMasterDataEnum.REACTION_TYPES)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'order the result.
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()
                ReactionTypeCombo.DataSource = qReactionTypes
                ReactionTypeCombo.DisplayMember = "FixedItemDesc"
                ReactionTypeCombo.ValueMember = "ItemID"
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " FillSampleTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill reading mode combo.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillReadingModeCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.READING_MODES)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'order the result.
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()
                ReadingModeCombo.DataSource = qReactionTypes
                ReadingModeCombo.DisplayMember = "FixedItemDesc"
                ReadingModeCombo.ValueMember = "ItemID"
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillSampleTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the main reference filter combo.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillMainReferenceFilterCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                GlobalEnumerates.PreloadedMasterDataEnum.WAVELENGTHS)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qMainsWaveLengths As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                Dim qRefWaveLengths As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                'order the result.
                qMainsWaveLengths = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()
                'MAIN FILTER
                MainFilterCombo.DataSource = qMainsWaveLengths
                MainFilterCombo.DisplayMember = "FixedItemDesc"
                MainFilterCombo.ValueMember = "ItemID"

                'REFERENCE FILTER
                qRefWaveLengths = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                ReferenceFilterCombo.DataSource = qRefWaveLengths
                ReferenceFilterCombo.DisplayMember = "FixedItemDesc"
                ReferenceFilterCombo.ValueMember = "ItemID"
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillMainReferenceFilterCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' fill the predilution mode combo
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FillPredilutionModeCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                GlobalEnumerates.PreloadedMasterDataEnum.PREDILUTION_MODES)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qReactionTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'order the result.
                qReactionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position _
                                  Select a).ToList()

                PredilutionModeCombo.DataSource = qReactionTypes
                PredilutionModeCombo.DisplayMember = "FixedItemDesc"
                PredilutionModeCombo.ValueMember = "ItemID"

            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillPredilutionModeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the diluent combo with the dilution solutions.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 17/01/2011
    ''' </remarks>
    Private Sub FillDiluentCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Analysis type
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)

            If Not myGlobalDataTO.HasError Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim qDiluentSol As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                'order the result.
                qDiluentSol = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                  Order By a.Position Select a).ToList()

                DiluentComboBox.DataSource = qDiluentSol
                DiluentComboBox.DisplayMember = "FixedItemDesc"
                DiluentComboBox.ValueMember = "ItemID"

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FillDiluentCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the software parameter table.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadSWParameters()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim mySwParametersDelegate As New SwParametersDelegate()
            myGlobalDataTO = mySwParametersDelegate.ReadByAnalyzerModel(Nothing, AnalyzerModelAttribute)
            If Not myGlobalDataTO.HasError Then
                SwParametersDS = CType(myGlobalDataTO.SetDatos, ParametersDS)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " LoadSWParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Setup the limits and stept increment of all Numeric UpDown controls 
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 
    ''' Modified by: SG 11/06/2010 Factor and Repetition limits and steps
    '''              SG 16/06/2010 Reference ranges (normal and border line)
    ''' </remarks>
    Private Sub SetUpControlsLimits()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS()

            '** FIELDS IN GENERAL TAB ** '
            'Decimals Allowed
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CTEST_NUM_DECIMALS)
            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0) Then
                DecimalsUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                DecimalsUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                DecimalsUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    DecimalsUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Patient replicates UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.TEST_NUM_REPLICATES)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                ReplicatesUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                ReplicatesUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    ReplicatesUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            '** FIELDS IN PROCEDURE TAB ** '
            'Sample Volume
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SAMPLES_VOLUME, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                SampleVolUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                SampleVolUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                SampleVolUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                SampleVolUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
            End If

            'Wash Volume
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.WASHING_VOLUME)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                WashSolVolUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                WashSolVolUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                WashSolVolUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                WashSolVolUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
            End If

            'R1 VOLUME
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT1_VOLUME, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                VolR1UpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                VolR1UpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                VolR1UpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
            End If

            'R2 VOLUME
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.REAGENT2_VOLUME, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                VolR2UpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                VolR2UpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                VolR2UpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
            End If

            'Get the number of seconds for a machine cycle in the Analyzer Model
            Dim cycleMachineSeconds As Integer = 0
            Dim qswParameter As List(Of ParametersDS.tfmwSwParametersRow) = (From a In SwParametersDS.tfmwSwParameters _
                                                                            Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                                                          AndAlso String.Compare(a.AnalyzerModel, AnalyzerModelAttribute, False) = 0 _
                                                                           Select a).ToList()
            If (qswParameter.Count > 0) Then cycleMachineSeconds = CType(qswParameter.First().ValueNumeric, Integer)

            'READING CYCLE 1
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING1_CYCLES, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                FirstReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                FirstReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    FirstReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If

                'TIME SEC 1
                FirstReadingSecUpDown.Minimum = CType((FirstReadingCycleUpDown.Minimum - 1) * cycleMachineSeconds, Decimal)
                FirstReadingSecUpDown.Maximum = CType((FirstReadingCycleUpDown.Maximum - 1) * cycleMachineSeconds, Decimal)
                FirstReadingSecUpDown.Increment = cycleMachineSeconds
            End If

            'READING CYCLE 2
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING2_CYCLES, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                SecondReadingCycleUpDown.ResetText()
                SecondReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                SecondReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    SecondReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If

                'TIME SEC 2
                SecondReadingSecUpDown.Minimum = CType((SecondReadingCycleUpDown.Minimum - 1) * cycleMachineSeconds, Decimal)
                SecondReadingSecUpDown.Maximum = CType((SecondReadingCycleUpDown.Maximum - 1) * cycleMachineSeconds, Decimal)
                SecondReadingSecUpDown.Increment = cycleMachineSeconds
            End If


            '** FIELDS IN BLANK & CALIBRATION TAB ** '
            'AG 18/03/2010 - Blank and Calibrator Replicates UpDowns
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.BLK_CALIB_REPLICATES)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                BlankReplicatesUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                BlankReplicatesUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                CalReplicatesUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                CalReplicatesUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    BlankReplicatesUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    CalReplicatesUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If
            'END AG 18/03/2010

            '** FIELDS IN OPTIONS TAB **'
            'Blank Absorbance Limit UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.BLANK_ABSORBANCE_LIMIT)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                BlankAbsorbanceUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                BlankAbsorbanceUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                BlankAbsorbanceUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    BlankAbsorbanceUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Kinetic Blank Limit UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.KINETIC_BLANK_LIMIT)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                KineticBlankUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                KineticBlankUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                KineticBlankUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    KineticBlankUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Linearity Limit UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.LINEARITY_LIMIT)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                LinearityUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                LinearityUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                LinearityUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    LinearityUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Detection Limit UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.DETECTION_LIMIT)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                DetectionUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                DetectionUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                DetectionUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    DetectionUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Factor Limits UpDowns
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.FACTOR_LIMITS)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                FactorLowerLimitUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                FactorLowerLimitUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                FactorLowerLimitUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    FactorLowerLimitUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If

                FactorUpperLimitUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                FactorUpperLimitUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                FactorUpperLimitUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    FactorUpperLimitUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Rerun Limits UpDowns
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.RERUN_REF_RANGE)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                RerunLowerLimitUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                RerunLowerLimitUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                RerunLowerLimitUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    RerunLowerLimitUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If

                RerunUpperLimitUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                RerunUpperLimitUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                RerunUpperLimitUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    RerunUpperLimitUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Prozone Ratio UpDown
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.PROZONE_RATIO)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                ProzonePercentUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                ProzonePercentUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                ProzonePercentUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    ProzonePercentUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Slope Factor A
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SLOPE_FACTOR_A)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                SlopeAUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                SlopeAUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                SlopeAUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    SlopeAUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Slope Factor B
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SLOPE_FACTOR_B)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                SlopeBUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                SlopeBUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                SlopeBUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    SlopeBUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Substrate Depletion
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.SUBSTRATE_DEPLETION)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                SubstrateDepleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                SubstrateDepleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                SubstrateDepleUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed

                If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                    SubstrateDepleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetUpControlsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method in charge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>Created by: TR</remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO

            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the time Cycles control
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID, pAnalyzerModel)

            If Not myGlobalDataTO.HasError Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetControlsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myFieldLimitsDS

    End Function

    ''' <summary>
    ''' Initialize all the controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            GetScreenLabels()

            'TR 19/11/2010 
            Dim auxIconName As String = GetIconName("PLUS")
            If auxIconName <> "" Then
                PlusIconPath = MyBase.IconsPath & auxIconName
            End If
            'TR 19/11/2010 -END

            PrepareButtons()
            LoadSWParameters()
            PrepareTestListView() 'PG 08/10/2010
            ' TR 14/06/2010 -Prepare the controls with the Calib info.
            PrepareCurveComboControls()
            PrepareConcentrationGridView() 'PG 08/10/10
            ' TR 14/06/2010 -END

            'TR 04/04/2011
            PrepareQCTab()
            'TR 04/04/2011 -END.

            SetUpControlsLimits()
            FillTestListView()
            FillUnitsCombo()
            FillAnalysisModeCombo()
            FillBlankTypesCombo()
            FillReactionTypeCombo()
            FillReadingModeCombo()
            FillMainReferenceFilterCombo()
            FillPredilutionModeCombo()
            FillDiluentCombo() 'TR 17/01/2011

            'FillSampleTypeCombo() 'DL 11/01/2012
            FillSelectedSampleTypeCombo() ' dl 15/07/2010
            InitializeReferenceRangesControl() ' AG 12/01/2011
            bsTestRefRanges.isEditing = False

            'TR 01/07/2010
            BsCalibNumberTextBox.Enabled = False
            BsCalibNumberTextBox.BackColor = SystemColors.MenuBar
            'TR 01/07/2010 -End.

            'Set the focus to TestListView
            If TestListView.Items.Count > 0 Then
                TestListView.Items(0).Selected = True
                QuerySelectedTest() 'TR 17/11/2010
            Else
                ClearAllControls()
            End If

            'TR 18/11/2010 -Set BackGrounColor
            CalibratorNameTextBox.BackColor = SystemColors.MenuBar
            CalibratorExpirationDate.BackColor = SystemColors.MenuBar
            CalibratorLotTextBox.BackColor = SystemColors.MenuBar
            TestDescriptionTextBox.BackColor = SystemColors.MenuBar
            'TR 18/11/2010 -END.

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Validate if calibrator values is factory value.
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <param name="pSampleType"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 08/03/2011
    ''' </remarks>
    Private Function ValidateCalibratorFactoryValues(ByVal pTestID As Integer, ByVal pSampleType As String) As Boolean
        Dim isFactoryValue As Boolean = False
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestSampleDelegate As New TestSamplesDelegate

            'TR 08/03/2011 -Validate if Factory Calibrator value is true.
            myGlobalDataTO = myTestSampleDelegate.ValidateFactoryCalibratorValue(Nothing, pTestID, pSampleType)
            If Not myGlobalDataTO.HasError Then
                isFactoryValue = DirectCast(myGlobalDataTO.SetDatos, Boolean)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibratorFactoryValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return isFactoryValue

    End Function


    ''' <summary>
    ''' Connect all the screen controls to the data related to 
    ''' and specific test id an sample type.
    ''' </summary>
    ''' <param name="pTestID">Test ID</param>
    ''' <param name="pSampleType">Optional Sample Type.</param>
    ''' <param name="pReadTestInfo">Optional True call the GetTestInfo, else dont call it.</param>
    ''' <remarks>
    ''' Created by: TR 03/10/2010
    ''' Modified by: TR 19/03/2010
    ''' Modified by: SG 11/06/2010 Factor limits and Repetition range related textboxes became NumericUpDown
    ''' Modified by: SG 22/06/2010 Get the active range type from testsamples
    ''' Modified by: AG 28/09/2010 Add pReadTestInfo optional parameter
    ''' </remarks>
    Private Sub BindControls(ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                                    Optional ByVal pReadTestInfo As Boolean = True)
        Try
            Dim myTestID As Integer = pTestID
            Dim mySampleType As String = pSampleType
            Dim myRangeType As String
            'Get the data to connect the controls.

            'AG 28/09/2010
            'GetTestInfo(myTestID, mySampleType)
            If pReadTestInfo Then GetTestInfo(myTestID, mySampleType)
            'END AG 28/09/2010

            If SelectedTestDS.tparTests.Rows.Count > 0 Then

                'TESTDS
                TestNameTextBox.Text = SelectedTestDS.tparTests(0).TestName
                ShortNameTextBox.Text = SelectedTestDS.tparTests(0).ShortName
                AnalysisModeCombo.SelectedValue = SelectedTestDS.tparTests(0).AnalysisMode.TrimEnd()
                UnitsCombo.SelectedValue = SelectedTestDS.tparTests(0).MeasureUnit
                ReactionTypeCombo.SelectedValue = SelectedTestDS.tparTests(0).ReactionType.TrimEnd()

                If Not SelectedTestDS.tparTests(0).IsAbsorbanceFlagNull Then
                    AbsCheckBox.Checked = SelectedTestDS.tparTests(0).AbsorbanceFlag
                Else
                    'AG 15/03/2010
                    'AbsCheckBox.CheckState = CheckState.Indeterminate
                    AbsCheckBox.CheckState = CheckState.Unchecked
                End If

                'TR 07/05/2011 -Do not implement turbidimetry
                'AG 15/03/2010
                'TurbidimetryCheckBox.Checked = SelectedTestDS.tparTests(0).TurbidimetryFlag
                'If Not SelectedTestDS.tparTests(0).IsTurbidimetryFlagNull Then
                '    TurbidimetryCheckBox.Checked = SelectedTestDS.tparTests(0).TurbidimetryFlag
                'Else
                '    TurbidimetryCheckBox.CheckState = CheckState.Unchecked
                'End If
                'END AG 15/03/2010
                'TR 07/05/2011 -END.

                ReadingModeCombo.SelectedValue = SelectedTestDS.tparTests(0).ReadingMode.TrimEnd()
                'TR 06/04/2010
                'validate if mono to disable the reference filter combo.
                If SelectedTestDS.tparTests(0).ReadingMode.TrimEnd() = "MONO" Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                End If
                'END TR 06/04/2010

                'TR 19/03/2010
                If Not SelectedTestDS.tparTests(0).IsDecimalsAllowedNull Then
                    DecimalsUpDown.Text = SelectedTestDS.tparTests(0).DecimalsAllowed.ToString()
                Else
                    DecimalsUpDown.ResetText()
                End If

                If Not SelectedTestDS.tparTests(0).IsReplicatesNumberNull Then
                    ReplicatesUpDown.Text = SelectedTestDS.tparTests(0).ReplicatesNumber.ToString()
                Else
                    ReplicatesUpDown.ResetText()
                End If
                'END TR 19/03/2010

                If SelectedTestDS.tparTests(0).IsMainWavelengthNull Then
                    MainFilterCombo.SelectedIndex = -1
                Else
                    MainFilterCombo.SelectedValue = SelectedTestDS.tparTests(0).MainWavelength.ToString()
                End If

                If SelectedTestDS.tparTests(0).IsReferenceWavelengthNull Then
                    ReferenceFilterCombo.SelectedIndex = -1
                Else
                    ReferenceFilterCombo.SelectedValue = SelectedTestDS.tparTests(0).ReferenceWavelength.ToString()
                End If

                'TR 10/05/2010
                SetReadingLimits(AnalysisModeCombo.SelectedValue.ToString())

                'TR 19/03/2010
                If SelectedTestDS.tparTests(0).IsFirstReadingCycleNull Then
                    FirstReadingCycleUpDown.ResetText()
                    FirstReadingSecUpDown.ResetText()
                Else
                    FirstReadingCycleUpDown.Text = CType(SelectedTestDS.tparTests(0).FirstReadingCycle, Decimal).ToString()
                    CalculateFirstReadingSecReading()
                End If

                If SelectedTestDS.tparTests(0).IsSecondReadingCycleNull Then
                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingSecUpDown.ResetText()
                Else
                    SecondReadingCycleUpDown.Text = CType(SelectedTestDS.tparTests(0).SecondReadingCycle, Decimal).ToString()
                    CalculateSecondReadingSec()
                End If
            End If

            'TESTSAMPLES
            SelectedSampleTypeCombo.DataSource = SelectedTestSamplesDS.tparTestSamples
            SelectedSampleTypeCombo.DisplayMember = "ItemIDDesc"
            SelectedSampleTypeCombo.ValueMember = "SampleType"

            'DL 13/01/2012. Begin
            Dim qSelectedSample As New List(Of TestSamplesDS.tparTestSamplesRow)
            For i As Integer = 0 To SampleTypeCheckList.Items.Count - 1
                qSelectedSample = (From a In SelectedTestSamplesDS.tparTestSamples _
                                   Where a.SampleType = SampleTypeCheckList.Items(i).ToString _
                                   Select a).ToList()

                If qSelectedSample.Count > 0 Then
                    SampleTypeCheckList.SetItemChecked(i, True)
                Else
                    SampleTypeCheckList.SetItemChecked(i, False)
                End If

            Next i
            'DL 13/01/2012. End


            If mySampleType <> "" Then
                SelectedSampleTypeCombo.SelectedValue = mySampleType
            End If

            'TR 19/11/2010 -Validate if there are more than one test sample
            ShowPlusSampleTypeIcon()
            'TR 19/11/2010 -END

            Dim qTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)
            'filter testsamples bu the sample type

            If mySampleType <> "" Then
                qTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples _
                                   Where a.SampleType = mySampleType _
                                   Select a).ToList()
            Else
                qTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples _
                                Select a).ToList()
            End If
            'END AG 16/07/2010

            If qTestSamples.Count > 0 Then
                'AG 16/07/2010
                'SampleTypeCombo.DataSource = qTestSamples
                Dim qTestSamplesEditionCombo As New List(Of TestSamplesDS.tparTestSamplesRow)
                qTestSamplesEditionCombo = (From a In SelectedTestSamplesDS.tparTestSamples Select a).ToList()
                'SampleTypeCombo.DataSource = qTestSamplesEditionCombo
                If mySampleType <> "" Then
                    'bsSampleTypeText.Text = mySampleType ' DL 11/01/2012
                    SampleTypeCboEx.Items.Clear()
                    SampleTypeCboEx.Items.Add(mySampleType)
                    SampleTypeCboEx.SelectedIndex = 0

                    SampleTypeCboAux.Items.Clear()
                    SampleTypeCboAux.Items.Add(mySampleType)
                    SampleTypeCboAux.SelectedIndex = 0
                End If

                'END AG 16/07/2010

                'SampleTypeCombo.DisplayMember = "SampleType"
                'SampleTypeCombo.ValueMember = "SampleType"
                'If mySampleType <> "" Then
                'SampleTypeCombo.SelectedValue = mySampleType ' DL 16/07/2010
                'End If

                ReportsNameTextBox.Text = qTestSamples.First().TestLongName
                SampleVolUpDown.Text = CType(qTestSamples.First().SampleVolume, Decimal).ToString()
                WashSolVolUpDown.Text = CType(qTestSamples.First().WashingVolume, Decimal).ToString()

                'TR 15/06/2010 -Bind calibrator value
                If qTestSamples.First().IsCalibratorReplicatesNull Then
                    CalReplicatesUpDown.ResetText()
                Else
                    'TR 21/06/2010 Set the Text property
                    CalReplicatesUpDown.Text = qTestSamples.First().CalibratorReplicates.ToString()
                End If
                'TR 15/06/2010 -END.

                If qTestSamples.First().IsSubstrateDepletionValueNull Then
                    SubstrateDepleUpDown.ResetText()
                Else
                    SubstrateDepleUpDown.Text = CType(qTestSamples.First().SubstrateDepletionValue, Decimal).ToString()
                End If

                If qTestSamples.First().IsPredilutionModeNull Then
                    PredilutionModeCombo.SelectedIndex = -1
                Else
                    PredilutionModeCombo.SelectedValue = qTestSamples.First().PredilutionMode
                End If

                PredilutionFactorCheckBox.Checked = qTestSamples.First().PredilutionUseFlag

                If qTestSamples.First().IsPredilutionFactorNull Then
                    PredilutionFactorTextBox.Clear()
                Else
                    PredilutionFactorTextBox.Text = qTestSamples.First().PredilutionFactor.ToString()
                End If

                'TR 17/01/2011 -Bind Dilution controls. Only the diluent solution is loaded.
                If qTestSamples.First().IsDiluentSolutionNull Then
                    DiluentComboBox.SelectedIndex = -1
                Else
                    DiluentComboBox.SelectedValue = qTestSamples.First().DiluentSolution
                End If
                'TR 17/01/2011 -END.


                AutoRepetitionCheckbox.Checked = qTestSamples.First().AutomaticRerun

                If qTestSamples.First().IsRedPostdilutionFactorNull Then
                    RedPostDilutionFactorTextBox.Clear()
                Else
                    RedPostDilutionFactorTextBox.Text = qTestSamples.First().RedPostdilutionFactor.ToString()
                End If

                If qTestSamples.First().IsIncPostdilutionFactorNull Then
                    IncPostDilutionFactorTextBox.Clear()
                Else
                    IncPostDilutionFactorTextBox.Text = qTestSamples.First().IncPostdilutionFactor.ToString()
                End If

                If qTestSamples.First().IsBlankAbsorbanceLimitNull Then
                    BlankAbsorbanceUpDown.Value = 0
                    BlankAbsorbanceUpDown.ResetText()
                Else
                    'BlankAbsorbanceValueTextBox.Text = CType(qTestSamples.First().BlankAbsorbanceLimit, Decimal).ToString()
                    BlankAbsorbanceUpDown.Text = CType(qTestSamples.First().BlankAbsorbanceLimit, Decimal).ToString()
                End If

                If qTestSamples.First().IsLinearityLimitNull Then
                    LinearityUpDown.Value = 0
                    LinearityUpDown.ResetText()
                Else
                    'LinearityLimitValueTextBox.Text = qTestSamples.First().LinearityLimit.ToString()
                    LinearityUpDown.Text = CType(qTestSamples.First().LinearityLimit, Decimal).ToString()
                End If

                If qTestSamples.First().IsDetectionLimitNull Then
                    DetectionUpDown.Value = DetectionUpDown.Minimum 'TR 29/06/2010
                    DetectionUpDown.ResetText()
                Else
                    'DetectionLimitTextBox.Text = qTestSamples.First().DetectionLimit.ToString()
                    DetectionUpDown.Text = CType(qTestSamples.First().DetectionLimit, Decimal).ToString()
                End If

                'SG 11/06/2010
                If qTestSamples.First().IsFactorLowerLimitNull Then
                    FactorLowerLimitUpDown.Value = FactorLowerLimitUpDown.Minimum  'TR 29/06/2010
                    FactorLowerLimitUpDown.ResetText()
                Else
                    FactorLowerLimitUpDown.Text = CType(qTestSamples.First().FactorLowerLimit, Decimal).ToString()
                End If

                If qTestSamples.First().IsFactorUpperLimitNull Then
                    'FactorUpperLimitUpDown.Value = CType("0,90", Decimal) 'TR 29/06/2010
                    FactorUpperLimitUpDown.Value = FactorUpperLimitUpDown.Minimum 'TR 19/11/2010 
                    FactorUpperLimitUpDown.ResetText()
                Else
                    FactorUpperLimitUpDown.Text = CType(qTestSamples.First().FactorUpperLimit, Decimal).ToString()
                End If

                If qTestSamples.First().IsRerunLowerLimitNull Then
                    RerunLowerLimitUpDown.Value = RerunLowerLimitUpDown.Minimum 'TR 29/06/2010 
                    RerunLowerLimitUpDown.ResetText()
                Else
                    RerunLowerLimitUpDown.Text = CType(qTestSamples.First().RerunLowerLimit, Decimal).ToString()
                End If

                If qTestSamples.First().IsRerunUpperLimitNull Then
                    RerunUpperLimitUpDown.Value = RerunUpperLimitUpDown.Minimum 'TR 29/06/2010 
                    RerunUpperLimitUpDown.ResetText()
                Else
                    RerunUpperLimitUpDown.Text = CType(qTestSamples.First().RerunUpperLimit, Decimal).ToString()
                End If
                'END SG 11/06/2010

                If qTestSamples.First().IsSlopeFactorANull Then
                    SlopeAUpDown.Value = 0
                    SlopeAUpDown.ResetText()
                Else
                    SlopeAUpDown.Text = CType(qTestSamples.First().SlopeFactorA, Decimal).ToString()
                End If

                If qTestSamples.First().IsSlopeFactorBNull Then
                    SlopeBUpDown.Value = 0
                    SlopeBUpDown.ResetText()
                Else
                    SlopeBUpDown.Text = CType(qTestSamples.First().SlopeFactorB, Decimal).ToString()
                End If

                If qTestSamples.First().CalibratorType = "FACTOR" Then
                    FactorRadioButton.Checked = True
                    If Not qTestSamples.First().IsCalibrationFactorNull Then
                        CalibrationFactorTextBox.Text = qTestSamples.First().CalibrationFactor.ToString()
                        'TR 01/07/2010
                        ConcentrationGridView.DataSource = Nothing
                        'BindCalibrationCurveControls(0, "") 'TR 17/11/2010 Commented
                        ClearConcentrationsCalibValues() 'TR 17/11/2010 Clear concentration values.
                    Else
                        CalibrationFactorTextBox.Clear()
                    End If

                ElseIf qTestSamples.First().CalibratorType = "EXPERIMENT" Then
                    MultipleCalibRadioButton.Checked = True
                    CalibrationFactorTextBox.Clear()
                    'TR 01/07/2010 TEST CALIBRATOR VALUES
                    BindCalibrationCurveControls(myTestID, mySampleType)
                    'CONCENTRATION VALUES.
                    FillConcentrationValuesList(myTestID, mySampleType)
                    'TR 01/07/2010 -END 
                Else
                    ClearConcentrationsCalibValues() 'TR 02/12/2010
                End If

                'TR 15/06/2010 -Set the alternative calibrator value
                If Not qTestSamples.First().IsCalibratorTypeNull AndAlso _
                                qTestSamples.First().CalibratorType = "ALTERNATIV" Then

                    AlternativeCalibRadioButton.Checked = True
                    CalibrationFactorTextBox.Clear()
                    FillAlternativeCalibrator()

                    'TR 30/06/2010 -Select the calibrator on the combo list.
                    If Not qTestSamples.First().IsSampleTypeAlternativeNull Then
                        AlternativeCalComboBox.SelectedValue = qTestSamples.First().SampleTypeAlternative
                    End If
                    'TR 30/06/2010 -End.

                    'TR 01/07/2010
                    ConcentrationGridView.DataSource = Nothing
                    ClearConcentrationsCalibValues() 'TR 02/12/2010

                Else
                    AlternativeCalComboBox.SelectedIndex = -1
                    'TR 30/06/2010 -End.
                End If
                'TR 15/06/2010 -END.

                'TR 25/01/2012 -Change the edition mode.
                Dim isEditionMode As Boolean = EditionMode
                If EditionMode AndAlso Not ChangesMade Then
                    EditionMode = False
                End If

                'TR 05/04/2011 -QC TAB: Load QCControlInformation. 
                If EditionMode AndAlso Not ChangesMade Then
                    EditionMode = False
                    QCActiveCheckBox.Checked = qTestSamples.First().QCActive
                    EditionMode = True
                Else
                    QCActiveCheckBox.Checked = qTestSamples.First().QCActive
                End If

                If Not qTestSamples.First().IsControlReplicatesNull Then
                    QCReplicNumberNumeric.Text = qTestSamples.First().ControlReplicates.ToString()
                Else
                    QCReplicNumberNumeric.ResetText()
                End If

                If Not qTestSamples.First().IsRejectionCriteriaNull Then
                    QCRejectionCriteria.Text = CDec(qTestSamples.First().RejectionCriteria).ToString()
                Else
                    QCRejectionCriteria.ResetText()
                End If


                'Validate the calculation mode to select the corresponding CheckBox.
                If Not qTestSamples.First().IsCalculationModeNull AndAlso _
                            qTestSamples.First().CalculationMode = "MANUAL" Then
                    ManualRadioButton.Checked = True
                ElseIf Not qTestSamples.First().IsCalculationModeNull AndAlso _
                            qTestSamples.First().CalculationMode = "STATISTIC" Then
                    StaticRadioButton.Checked = True
                Else
                    ManualRadioButton.Checked = False
                    StaticRadioButton.Checked = False
                End If

                If Not qTestSamples.First().IsNumberOfSeriesNull AndAlso _
                        Not qTestSamples.First().NumberOfSeries = 0 Then
                    QCMinNumSeries.Text = qTestSamples.First().NumberOfSeries.ToString()
                Else
                    QCMinNumSeries.ResetText()
                End If

                If Not qTestSamples.First().IsTotalAllowedErrorNull Then
                    QCErrorAllowable.Text = CDec(qTestSamples.First().TotalAllowedError).ToString()
                Else
                    QCErrorAllowable.ResetText()
                End If

                'Bind the other QC control. on QC TAB.
                BindQCMultirulesTestID(pTestID, mySampleType)

                'NumberOfcontrols is a calculated field.
                'TR 05/04/2011 -END.

                'TR 25/01/2012 Set the edition mode back.
                EditionMode = isEditionMode

            End If

            TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & " - " & SampleTypeCboEx.Text

            'TESTREAGENTVOLUMES
            If SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count > 0 Then

                Dim qTestsReagentsVol As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                qTestsReagentsVol = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                    Where a.SampleType = mySampleType And a.ReagentNumber = 1 _
                                    Select a).ToList()

                If qTestsReagentsVol.Count > 0 Then
                    If Not qTestsReagentsVol.First().IsReagentVolumeNull Then
                        'TR 19/03/2010 use the property Text
                        VolR1UpDown.Text = CType(qTestsReagentsVol.First().ReagentVolume, Decimal).ToString()
                    End If
                Else
                    'if not found then reset
                    VolR1UpDown.ResetText()
                End If

                qTestsReagentsVol = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                    Where a.SampleType = mySampleType And a.ReagentNumber = 2 _
                                    Select a).ToList()

                If qTestsReagentsVol.Count > 0 Then
                    If Not qTestsReagentsVol.First().IsReagentVolumeNull Then
                        'TR 19/03/2010 use the property Text.
                        VolR2UpDown.Text = CType(qTestsReagentsVol.First().ReagentVolume, Decimal).ToString()
                    End If
                Else
                    'if not found then reset
                    VolR2UpDown.ResetText()
                End If
            End If

            ''TR 31/03/2010 set up the Prozone T1 AND  T2 limits
            SetProzone1Times()

            If SelectedTestDS.tparTests.Rows.Count > 0 Then 'AG 12/11/2010
                If Not SelectedTestDS.tparTests(0).IsBlankModeNull Then
                    BlankTypesCombo.SelectedValue = SelectedTestDS.tparTests(0).BlankMode
                Else
                    BlankTypesCombo.SelectedValue = -1
                End If

                If Not SelectedTestDS.tparTests(0).IsBlankReplicatesNull Then
                    BlankReplicatesUpDown.Text = SelectedTestDS.tparTests(0).BlankReplicates.ToString()
                Else
                    BlankReplicatesUpDown.ResetText()
                End If

                If SelectedTestDS.tparTests(0).IsKineticBlankLimitNull Then
                    KineticBlankUpDown.Value = 0
                    KineticBlankUpDown.ResetText()
                Else
                    KineticBlankUpDown.Text = CType(SelectedTestDS.tparTests(0).KineticBlankLimit, Decimal).ToString()
                End If

                'TR 08/04/2010 -Set the Prozones values after seting up the prozone controls.
                If SelectedTestDS.tparTests(0).IsProzoneRatioNull Then
                    ProzonePercentUpDown.ResetText()
                Else
                    ProzonePercentUpDown.Text = SelectedTestDS.tparTests(0).ProzoneRatio.ToString()
                End If
                'TR 30/03/2010
                If SelectedTestDS.tparTests(0).IsProzoneTime1Null Then
                    'ProzoneT1UpDown.Value = ProzoneT1UpDown.Minimum
                    ProzoneT1UpDown.ResetText()
                Else
                    ProzoneT1UpDown.Text = CType(SelectedTestDS.tparTests(0).ProzoneTime1, Decimal).ToString()
                End If

                If SelectedTestDS.tparTests(0).IsProzoneTime2Null Then
                    'ProzoneT2UpDown.Value = ProzoneT2UpDown.Minimum
                    ProzoneT2UpDown.ResetText()
                Else
                    ProzoneT2UpDown.Text = CType(SelectedTestDS.tparTests(0).ProzoneTime2, Decimal).ToString()
                End If
                'END TR 08/04/2010
            End If 'AG 12/11/2010

            'TEST REF RANGES
            'SG 17/06/2010
            'AG 12/11/2010
            myRangeType = ""
            If qTestSamples.Count > 0 Then
                If qTestSamples.First().IsActiveRangeTypeNull Then
                    myRangeType = ""
                Else
                    myRangeType = qTestSamples.First().ActiveRangeType
                End If
            End If 'AG 12/11/2010

            selRefStdTestID = pTestID
            selRefRangeType = myRangeType

            BindRefRanges(mySampleType)
            'BindReferenceRangesControls(myTestID, mySampleType, myRangeType.ToString.Trim) 'AG 19/07/2010 - add sampletype parameter 'SG 27/07/2010 add rangetype parameter
            'END SG 17/06/2010

            'AG 09/07/2010
            Dim decimalsNumber As Integer = 0
            If IsNumeric(DecimalsUpDown.Text) Then decimalsNumber = CInt(DecimalsUpDown.Text)
            SetupDecimalsNumber(decimalsNumber)
            'AG 09/07/2010

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " BindControls ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' fill the ref ranges control elements according to the selected calculated test
    ''' </summary>
    ''' <remarks>Created by SG 06/09/2010
    ''' Modified AG 12/01/2011 - adapt for new ref ranges control (copied and adapted from ProgOffSystemTest.LoadRefRangesData</remarks>
    Private Sub BindRefRanges(ByVal pSampleType As String)
        Try
            'AG 12/01/2011
            'If Me.SelectedTestSamplesDS IsNot Nothing AndAlso Me.SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
            '    If Me.SelectedTestRefRangesDS IsNot Nothing Then
            '        BsTestRefRanges.LoadDataStandard(Me.SelectedTestRefRangesDS, Me.SelectedTestSamplesDS, Me.selMeasureUnit, pSampleType)
            '    Else
            '        BsTestRefRanges.ClearData()
            '    End If
            'ElseIf pSampleType <> "" Then
            '    Me.SelectedTestRefRangesDS = New TestRefRangesDS
            '    BsTestRefRanges.NewDataStandard(Me.SelectedTestRefRangesDS, pSampleType)
            'Else
            '    BsTestRefRanges.ClearData()
            'End If

            If SelectedTestSamplesDS IsNot Nothing AndAlso SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                bsTestRefRanges.TestID = Me.SelectedTestSamplesDS.tparTestSamples(0).TestID
                bsTestRefRanges.SampleType = pSampleType

                Dim lnqSampleData As New List(Of TestSamplesDS.tparTestSamplesRow)
                'TR 22/07/2011 -Commented 
                'lnqSampleData = (From a In SelectedTestSamplesDS.tparTestSamples _
                '               Where a.SampleType = pSampleType Select a).ToList
                'TR 22/07/2011 -End
                'TR 22/07/2011 -Insert the Test ID on the where criteria.
                lnqSampleData = (From a In SelectedTestSamplesDS.tparTestSamples _
                                 Where a.TestID = SelectedTestSamplesDS.tparTestSamples(0).TestID _
                                 AndAlso a.SampleType = pSampleType Select a).ToList
                'TR 22/07/2011 -END.

                If lnqSampleData.Count > 0 Then
                    If lnqSampleData(0).IsActiveRangeTypeNull Then
                        bsTestRefRanges.ActiveRangeType = ""
                    Else
                        bsTestRefRanges.ActiveRangeType = lnqSampleData(0).ActiveRangeType
                    End If
                Else
                    bsTestRefRanges.ActiveRangeType = ""
                End If

                bsTestRefRanges.MeasureUnit = UnitsCombo.Text.ToString
                'TR 04/04/2011
                If Not Me.DesignMode Then
                    bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS
                End If

                Dim decimalsNumber As Integer = 0
                If (IsNumeric(DecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(DecimalsUpDown.Text)
                bsTestRefRanges.RefNumDecimals = decimalsNumber

                If (SelectedTestRefRangesDS IsNot Nothing) Then
                    If SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count > 0 Then
                        bsTestRefRanges.LoadReferenceRanges()
                    Else
                        bsTestRefRanges.ClearReferenceRanges()
                    End If

                Else
                    bsTestRefRanges.ClearReferenceRanges()
                End If
                'bsTestRefRanges.isEditing = TestNameTextBox.Enabled 'AG 13/01/2011
            Else
                bsTestRefRanges.ClearData()
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BindRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' New rule for reading limits depending on the Analysis Mode
    ''' </summary>
    ''' <param name="pAnalysisMode">Analysis Mode</param>
    ''' <remarks>
    ''' Created by: TR 10/05/2010
    ''' AG 10/02/2011 - Fix programming time limits error in BRDIFF, BRFT, BRK
    ''' </remarks>
    Private Sub SetReadingLimits(ByVal pAnalysisMode As String)
        Try
            'TR 10/05/2010 -Add new Rule
            Dim myFieldLimitsDS As New FieldLimitsDS()
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
            'TR 14/03/2011 -Add the analyzer model on the query.
            'Get the Cycle machine
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                           Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                           AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                           Select a).ToList()


            If pAnalysisMode = "BRDIF" Then 'MIN < Time1 < Time R2 < Time2 < MAX
                'READING CYCLE 1
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING1_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    FirstReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal) '3
                    FirstReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal) '68
                    FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum 'TR 08/06/2010
                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        FirstReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If

                    'TR 14/03/2011 -Add the analyzer model on the query.
                    'TIME SEC 1
                    qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                    Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                    AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                                    Select a).ToList()
                    If qswParameter.Count > 0 Then
                        FirstReadingSecUpDown.Minimum = CType((FirstReadingCycleUpDown.Minimum - 1) * _
                                                        CType(qswParameter.First().ValueNumeric, Integer), Decimal)
                        FirstReadingSecUpDown.Maximum = CType((FirstReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)
                        FirstReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)

                        'TR 08/06/2010 Reset the values 
                        FirstReadingCycleUpDown.ResetText()
                        FirstReadingSecUpDown.ResetText()
                    End If
                End If

                'READING CYCLE 2
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING2_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    'AG 10/02/2011 - Change R1
                    FirstReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal) '37 :MIN < Time1 < Time R2
                    FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
                    If qswParameter.Count > 0 Then
                        FirstReadingSecUpDown.Maximum = CType((FirstReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)
                    End If
                    'AG 10/02/2011

                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal) '37
                    SecondReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal) '68
                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        SecondReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If
                    'TIME SEC 2
                    If qswParameter.Count > 0 Then
                        'TR 08/06/2010 -Remove the convertion to integer for the value numeric to correct a calculation error.
                        SecondReadingSecUpDown.Minimum = CType((SecondReadingCycleUpDown.Minimum - 1) * qswParameter.First().ValueNumeric, Integer)
                        SecondReadingSecUpDown.Maximum = CType((SecondReadingCycleUpDown.Maximum - 1) * qswParameter.First().ValueNumeric, Decimal)
                        SecondReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)
                    End If
                    'TR 08/06/2010 Reset the values 
                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingSecUpDown.ResetText()
                End If


            ElseIf pAnalysisMode = "BRFT" Or pAnalysisMode = "BRK" Then 'Time R2 < Time1 < Time2 < MAX
                'Get the reading Cycles
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING2_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    SecondReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal) '37
                    SecondReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal) '68
                    SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum 'TR 08/06/2010

                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        SecondReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If

                    If qswParameter.Count > 0 Then
                        SecondReadingSecUpDown.Minimum = CType((SecondReadingCycleUpDown.Minimum - 1) * _
                                                            CType(qswParameter.First().ValueNumeric, Integer), Decimal)
                        SecondReadingSecUpDown.Maximum = CType((SecondReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)

                        SecondReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)
                    End If

                    'AG 10/02/2011
                    FirstReadingCycleUpDown.Minimum = SecondReadingCycleUpDown.Minimum '37
                    FirstReadingCycleUpDown.Maximum = SecondReadingCycleUpDown.Maximum '68
                    FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
                    FirstReadingSecUpDown.Minimum = SecondReadingSecUpDown.Minimum
                    FirstReadingSecUpDown.Maximum = SecondReadingSecUpDown.Maximum
                    FirstReadingSecUpDown.Increment = SecondReadingSecUpDown.Increment
                    'AG 10/02/2011

                    'TR 08/06/2010
                    FirstReadingCycleUpDown.ResetText()
                    FirstReadingSecUpDown.ResetText()
                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingSecUpDown.ResetText()
                End If
                'AG 10/02/2011

            ElseIf pAnalysisMode = "MRFT" Or pAnalysisMode = "MRK" Then 'MIN < Time1 < Time2 < MAX
                'Get the reading Cycles
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING1_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    SecondReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    SecondReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum 'TR 08/06/2010

                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        SecondReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If

                    If qswParameter.Count > 0 Then
                        SecondReadingSecUpDown.Minimum = CType((SecondReadingCycleUpDown.Minimum - 1) * _
                                                            CType(qswParameter.First().ValueNumeric, Integer), Decimal)
                        SecondReadingSecUpDown.Maximum = CType((SecondReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)

                        SecondReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)
                    End If

                    'AG 10/02/2011
                    FirstReadingCycleUpDown.Minimum = SecondReadingCycleUpDown.Minimum '37
                    FirstReadingCycleUpDown.Maximum = SecondReadingCycleUpDown.Maximum '68
                    FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
                    FirstReadingSecUpDown.Minimum = SecondReadingSecUpDown.Minimum
                    FirstReadingSecUpDown.Maximum = SecondReadingSecUpDown.Maximum
                    FirstReadingSecUpDown.Increment = SecondReadingSecUpDown.Increment
                    'AG 10/02/2011

                    'TR 08/06/2010
                    FirstReadingCycleUpDown.ResetText()
                    FirstReadingSecUpDown.ResetText()
                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingSecUpDown.ResetText()
                End If

                'AG 31/01/2011
            ElseIf pAnalysisMode = "MREP" Then 'MIN < Time1 < MAX
                SecondReadingCycleUpDown.Enabled = False 'disable reading 2
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING1_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    FirstReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    FirstReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        FirstReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If

                    'TR 14/03/2011 -Add the analyzer model on the query.
                    'TIME SEC 1
                    qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                    Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                    AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                                    Select a).ToList()

                    If qswParameter.Count > 0 Then
                        FirstReadingSecUpDown.Minimum = CType((FirstReadingCycleUpDown.Minimum - 1) * _
                                                        qswParameter.First().ValueNumeric, Integer)
                        FirstReadingSecUpDown.Maximum = CType((FirstReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)
                        FirstReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)

                        'TR 08/06/2010 Reset the values 
                        FirstReadingCycleUpDown.ResetText()
                        FirstReadingSecUpDown.ResetText()
                    End If
                End If
                'AG 31/01/2011

            ElseIf pAnalysisMode = "BREP" Then 'Time R2 < Time1 < MAX
                SecondReadingCycleUpDown.Enabled = False 'disable reading 2
                'TR 08/06/2010 -change the limits to 37 cycles minimum. (AG)
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.READING2_CYCLES, MyBase.AnalyzerModel)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    FirstReadingCycleUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    FirstReadingCycleUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    If Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull Then
                        FirstReadingCycleUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If

                    'TR 14/03/2011 -Add the analyzer model on the query.
                    'TIME SEC 1
                    qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                   AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                                   Select a).ToList()
                    If qswParameter.Count > 0 Then
                        FirstReadingSecUpDown.Minimum = CType((FirstReadingCycleUpDown.Minimum - 1) * _
                                                        qswParameter.First().ValueNumeric, Integer)
                        FirstReadingSecUpDown.Maximum = CType((FirstReadingCycleUpDown.Maximum - 1) * _
                                                            qswParameter.First().ValueNumeric, Decimal)
                        FirstReadingSecUpDown.Increment = CType(qswParameter.First().ValueNumeric, Integer)

                        'TR 08/06/2010 Reset the values 
                        FirstReadingCycleUpDown.ResetText()
                        FirstReadingSecUpDown.ResetText()
                    End If
                End If
            End If
            'TR 10/05/2010 -END Add new Rule
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetReadingLimits ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    Private Sub CtrlsActivationByAnalysisMode(ByVal pAnalysisMode As String)
        Try
            If EditionMode Then

                If pAnalysisMode = "BREP" Or pAnalysisMode = "BRFT" Or pAnalysisMode = "BRDIF" Or pAnalysisMode = "BRK" Then
                    'Activation when analysis mode is bireagent
                    VolR2UpDown.Enabled = True
                Else
                    VolR2UpDown.Enabled = False
                    VolR2UpDown.ResetText()
                End If

                'AG 31/01/2011 - Fixed time also allow bichromatic
                'If pAnalysisMode = "MREP" Or pAnalysisMode = "BREP" Then
                'If pAnalysisMode = "MREP" Or pAnalysisMode = "BREP" Or pAnalysisMode = "MRFT" Or pAnalysisMode = "BRFT"  Then
                If pAnalysisMode = "MREP" Or pAnalysisMode = "BREP" Or pAnalysisMode = "MRFT" Or pAnalysisMode = "BRFT" Or pAnalysisMode = "BRDIF" Then
                    ReadingModeCombo.Enabled = True
                    'ReadingModeCombo.SelectedIndex = 0
                    'ReferenceFilterCombo.Enabled = True
                Else
                    ReadingModeCombo.SelectedIndex = 0 'AG + TR 04/10/2011 
                    ReadingModeCombo.Enabled = False
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False

                End If
                'TR 11/05/2010 -change the if take out the BREP for new rule on 10/05/2010
                'If pAnalysisMode = "BRDIF" Or pAnalysisMode = "BREP" Or pAnalysisMode = "MRFT" Or pAnalysisMode = "BRFT" _ Or pAnalysisMode = "MRK" Or pAnalysisMode = "BRK" Then
                If pAnalysisMode = "BRDIF" Or pAnalysisMode = "MRFT" Or pAnalysisMode = "BRFT" _
                    Or pAnalysisMode = "MRK" Or pAnalysisMode = "BRK" Then
                    SecondReadingCycleUpDown.Enabled = True
                    SecondReadingSecUpDown.Enabled = True
                Else
                    SecondReadingCycleUpDown.ResetText()
                    SecondReadingSecUpDown.ResetText()
                    SecondReadingCycleUpDown.Enabled = False
                    SecondReadingSecUpDown.Enabled = False
                End If
                'Activate the Kinetics
                'TR 25/03/2010 add new elements to activate or desactivate.
                'DL 05/05/2010 
                'If pAnalysisMode = "MRK" Or pAnalysisMode = "BRK" Or pAnalysisMode = "BRFT" Or pAnalysisMode = "MRFT" Then
                If pAnalysisMode = "MRK" Or pAnalysisMode = "BRK" Or pAnalysisMode = "MRFT" Then
                    KineticBlankUpDown.Enabled = True
                    KineticBlankUpDown.BackColor = Color.White
                    SubstrateDepleUpDown.Enabled = True
                    SubstrateDepleUpDown.BackColor = Color.White

                Else
                    KineticBlankUpDown.Enabled = False
                    KineticBlankUpDown.BackColor = SystemColors.MenuBar
                    SubstrateDepleUpDown.Enabled = False
                    SubstrateDepleUpDown.BackColor = SystemColors.MenuBar
                End If

                'AG 19/03/2010 - No edition mode
            Else

                KineticBlankUpDown.Enabled = False
                KineticBlankUpDown.BackColor = SystemColors.MenuBar
                'TR 25/03/2010 -add new controls
                SubstrateDepleUpDown.Enabled = False
                SubstrateDepleUpDown.BackColor = SystemColors.MenuBar
                'END TR 25/03/2010 
                'END AG 19/03/2010
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CtrlsActivationByAnalysisMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Validate the wavelengths values are not equals .
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ValidateWavelengths()
        Try
            'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            ValidationError = False
            If Not ReferenceFilterCombo.SelectedValue Is Nothing Then
                If ReferenceFilterCombo.SelectedValue.ToString() = MainFilterCombo.SelectedValue.ToString() Then
                    BsErrorProvider1.SetError(ReferenceFilterCombo, GetMessageText(GlobalEnumerates.Messages.MAINFILTER_DIF_REFFILTER.ToString)) 'AG 07/07/2010 '("MAINFILTER_DIF_REFFILTER"))
                    ValidationError = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " WavelengthsValidation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ValidateReadingsParameters()
        Try
            'BsErrorProvider1.Clear() ' RH 22/06/2012 Don't remove previous error notifications
            ValidationError = False
            'TR 10/05/2010 Add the edition validation 
            If EditionMode Then
                'Check the programmed times
                If SecondReadingCycleUpDown.Text <> "" Then
                    If FirstReadingCycleUpDown.Value >= SecondReadingCycleUpDown.Value Then
                        BsErrorProvider1.SetError(FirstReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.FIRSTREAD_GREAT_SECONREAD.ToString)) 'AG 07/07/2010 ("FIRSTREAD_GREAT_SECONREAD"))
                        ValidationError = True
                    Else
                        BsErrorProvider1.Clear() 'TR 02/10/2012 -Clear errors fi validation is OK. 
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ReadingAutoCheckingParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Remove localy a Test Reagent vol and from all the 
    ''' local stucures, then add the deleted test Reagent vol
    ''' in to the list of deleted Test Reagent Volume.
    ''' </summary>
    ''' <param name="pTestID"></param>
    ''' <remarks>Created by: TR 15/03/2010</remarks>
    Private Sub RemoveTestReagentsVol2(ByVal pTestID As Integer)
        Try
            Dim myDelTestReagentVolTO As New DeletedTestReagentsVolumeTO
            Dim qTestReagent As New List(Of TestReagentsDS.tparTestReagentsRow)
            Dim qTestReagenteVol As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)

            If SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count > 1 Then

                qTestReagenteVol = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                    Where a.TestID = pTestID And a.ReagentNumber = 2).ToList()

                If qTestReagenteVol.Count > 0 Then
                    myDelTestReagentVolTO.TestID = qTestReagenteVol.First().TestID
                    myDelTestReagentVolTO.ReagentID = qTestReagenteVol.First().ReagentID
                    myDelTestReagentVolTO.SampleType = qTestReagenteVol.First().SampleType
                    myDelTestReagentVolTO.ReagentNumber = 2
                    'add the TO to the list of deleted test reagents volume
                    DeletedTestReagentVolList.Add(myDelTestReagentVolTO)

                    'Delete Test Reagent from my all local stuctures.
                    qTestReagenteVol.First().Delete()

                    qTestReagent = (From a In SelectedTestReagentsDS.tparTestReagents _
                                Where a.TestID = pTestID And a.ReagentNumber = 2).ToList()

                    For Each testReagentRow As TestReagentsDS.tparTestReagentsRow In qTestReagent
                        'Remove from TestReagent
                        testReagentRow.Delete()

                        'Remove from Reagent.
                        SelectedReagentsDS.tparReagents.DefaultView.RowFilter = "ReagentID = '" & myDelTestReagentVolTO.ReagentID & "'"

                        'if there's a record then delete it.
                        If SelectedReagentsDS.tparReagents.DefaultView.Count > 0 Then
                            SelectedReagentsDS.tparReagents.DefaultView(0).Delete()
                        End If

                        'clear filter
                        SelectedReagentsDS.tparReagents.DefaultView.RowFilter = ""
                    Next
                    'Accept all changes.
                    SelectedReagentsDS.tparReagents.AcceptChanges()
                    SelectedTestReagentsDS.tparTestReagents.AcceptChanges()
                    SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AcceptChanges()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " RemoveTestReagentsVol2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update the Test local structure.
    ''' </summary>
    ''' <param name="pTestID">Test ID</param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <remarks>
    ''' Created by: TR
    ''' Modified by: TR 19/03/2010.
    ''' Modified by: SG 17/06/2010 Test ref ranges
    ''' </remarks>
    Private Sub UpdateTestDatasets(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            If EditionMode And Not ValidationError Then

                Dim qSelectedTest As New List(Of TestsDS.tparTestsRow)

                qSelectedTest = (From a In SelectedTestDS.tparTests _
                                Where a.TestID = pTestID _
                                Select a).ToList()

                If qSelectedTest.Count > 0 Then
                    qSelectedTest.First().TestName = Me.TestNameTextBox.Text
                    qSelectedTest.First().ShortName = Me.ShortNameTextBox.Text
                    qSelectedTest.First().MeasureUnit = UnitsCombo.SelectedValue.ToString().TrimEnd()
                    qSelectedTest.First().AnalysisMode = AnalysisModeCombo.SelectedValue.ToString()

                    Select Case AnalysisModeCombo.SelectedValue.ToString()
                        Case "MREP", "MRFT", "MRK"
                            qSelectedTest.First().ReagentsNumber = 1
                            'remove reagent 2 if exist.
                            RemoveTestReagentsVol2(pTestID)
                            Exit Select
                        Case "BREP", "BRDIF", "BRFT", "BRK"
                            qSelectedTest.First().ReagentsNumber = 2
                            Exit Select
                        Case Else
                            Exit Select
                    End Select

                    qSelectedTest.First().ReactionType = ReactionTypeCombo.SelectedValue.ToString().TrimEnd()
                    qSelectedTest.First().DecimalsAllowed = CType(DecimalsUpDown.Value, Integer)
                    qSelectedTest.First().TurbidimetryFlag = False 'TR 07/05/2011 -Do not implement turbidimetry set false value.
                    qSelectedTest.First().AbsorbanceFlag = AbsCheckBox.Checked
                    qSelectedTest.First().ReadingMode = ReadingModeCombo.SelectedValue.ToString().TrimEnd()

                    'TR 07/04/2010
                    If Not FirstReadingCycleUpDown.Text = "" AndAlso Not FirstReadingCycleUpDown.Value = 0 Then
                        qSelectedTest.First().FirstReadingCycle = CType(FirstReadingCycleUpDown.Value, Integer)
                    Else
                        qSelectedTest.First().SetFirstReadingCycleNull()
                    End If

                    If Not SecondReadingCycleUpDown.Text = "" AndAlso Not SecondReadingCycleUpDown.Value = 0 Then
                        qSelectedTest.First().SecondReadingCycle = CType(SecondReadingCycleUpDown.Value, Integer)
                    Else
                        qSelectedTest.First().SetSecondReadingCycleNull()
                    End If
                    'END TR 07/04/2010

                    qSelectedTest.First().MainWavelength = CInt(MainFilterCombo.SelectedValue)
                    'TR 19/03/2010
                    'If BlankReplicatesUpDown.Text <> "" Then
                    qSelectedTest.First().BlankReplicates = CType(BlankReplicatesUpDown.Value, Integer)
                    'End If
                    'END TR 19/03/2010
                    If Not ReferenceFilterCombo.SelectedValue Is Nothing Then
                        qSelectedTest.First().ReferenceWavelength = CInt(ReferenceFilterCombo.SelectedValue)
                    Else 'AG 01/02/2011 - add Else case
                        qSelectedTest.First.SetReferenceWavelengthNull()
                    End If

                    If Not BlankTypesCombo.SelectedValue Is Nothing Then
                        qSelectedTest.First().BlankMode = BlankTypesCombo.SelectedValue.ToString()
                    End If

                    If ReplicatesUpDown.Text <> "" Then
                        qSelectedTest.First().ReplicatesNumber = CType(ReplicatesUpDown.Value, Integer)
                    End If
                    'TR 02/07/2010 -Cahnge value convertion from integer to single.
                    If KineticBlankUpDown.Text <> "" Then
                        qSelectedTest.First().KineticBlankLimit = CType(KineticBlankUpDown.Value, Single)
                    Else
                        qSelectedTest.First().SetKineticBlankLimitNull()
                    End If
                    'TR 02/07/2010  -End.
                    'TR 26/05/2010
                    If ProzonePercentUpDown.Text <> "" Then
                        qSelectedTest.First().ProzoneRatio = CType(ProzonePercentUpDown.Value, Single)
                    Else
                        qSelectedTest.First().SetProzoneRatioNull()
                    End If

                    If ProzoneT1UpDown.Text <> "" Then
                        qSelectedTest.First().ProzoneTime1 = CType(ProzoneT1UpDown.Value, Integer)
                    Else
                        qSelectedTest.First().SetProzoneTime1Null()
                    End If

                    If ProzoneT2UpDown.Text <> "" Then
                        qSelectedTest.First().ProzoneTime2 = CType(ProzoneT2UpDown.Value, Integer)
                    Else
                        qSelectedTest.First().SetProzoneTime2Null()
                    End If
                    qSelectedTest.First().TS_User = GetApplicationInfoSession.UserName
                    qSelectedTest.First().TS_DateTime = DateTime.Now
                    qSelectedTest.First().AcceptChanges()
                End If

                '//REAGENTSDS - TestReagent
                UpdateReagents(qSelectedTest.First().TestName, qSelectedTest.First().ReagentsNumber, qSelectedTest.First().TestID)

                'TR 22/07/2011 -Validate is not a test copy
                If Not CopyTestData AndAlso Not NewTest Then
                    'SG 18/06/2010
                    'TEST REF RANGES
                    UpdateRefRangesChanges(pTestID, pSampleType, False)
                End If
                'TR 22/07/2011 -END

                'QUITAR COMMENT
                'Dim qSelectedTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                'If SelectedTestRefRangesDS.tparTestRefRanges.Count > 0 Then
                '    qSelectedTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                '                        Where a.TestID = pTestID And a.SampleType = pSampleType _
                '                        Select a).ToList()
                'End If

                'If Me.BsTestRefRanges.IsReferenceRangesDefined(pSampleType) Then
                '    Me.BsTestRefRanges.UpdateTestRefRanges(pTestID, pSampleType)
                'Else
                '    'if the user has not defined any ref range then delete the pre-existing whole ref range datarow
                '    If qSelectedTestRefRanges.Count > 0 Then
                '        For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In qSelectedTestRefRanges
                '            'SelectedTestRefRangesDS.tparTestRefRanges.Rows.Remove(TestRefRangesRow)
                '            TestRefRangesRow.IsDeleted = True
                '        Next
                '    End If
                'End If


                'END SG 18/06/2010

                '//TESTSAMPLE
                UpdateTestSample(pTestID, pSampleType)

                UpdateTestSampleMultiRules(pTestID, pSampleType)

                ChangesMade = False 'TR 24/03/2010

            End If

            SelectedTestSamplesDS.AcceptChanges()
            SelectedTestDS.tparTests.AcceptChanges()
            SelectedReagentsDS.tparReagents.AcceptChanges()
            SelectedTestReagentsDS.tparTestReagents.AcceptChanges()
            SelectedTestReagentsVolumesDS.AcceptChanges()
            SelectedTestRefRangesDS.AcceptChanges() 'SG 17/06/2010

            'TR 08/11/2010 -Not needed
            'InsertUpdateRow(pTestID) 

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateTestDatasets ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Update reagents local dataset.
    ''' </summary>
    ''' <param name="pTestName">Test Name</param>
    ''' <param name="pReagNumber">Reagent Number</param>
    ''' <param name="pTestId">TestID</param>
    ''' <remarks>
    ''' Created by: TR 11/03/2010
    ''' </remarks>
    Private Sub UpdateReagents(ByVal pTestName As String, ByVal pReagNumber As Integer, ByVal pTestId As Integer)
        Try
            Dim myReagentID As Integer = 0
            Dim qReagents As New List(Of ReagentsDS.tparReagentsRow)
            Dim qTestReagent As New List(Of TestReagentsDS.tparTestReagentsRow)
            Dim RNumb As Integer = 0
            Dim myReagentNumber As Integer = 0
            'TR 19/05/2010 Correct and error when change from mono reagente to bireagent
            For i As Integer = 1 To pReagNumber Step 1
                myReagentNumber = i
                If SelectedReagentsDS.tparReagents.Rows.Count > 0 Then

                    qTestReagent = (From a In SelectedTestReagentsDS.tparTestReagents _
                                    Where a.TestID = pTestId And a.ReagentNumber = myReagentNumber _
                                    Select a).ToList()

                    If qTestReagent.Count > 0 Then
                        'filter by teh test reagentid  found on the TestReagent list
                        qReagents = (From a In SelectedReagentsDS.tparReagents _
                                    Where a.ReagentID = qTestReagent.First().ReagentID _
                                    Select a).ToList()
                    End If

                    If qReagents.Count = 0 Then
                        Dim newReagentRow As ReagentsDS.tparReagentsRow
                        newReagentRow = SelectedReagentsDS.tparReagents.NewtparReagentsRow()
                        newReagentRow.IsNew = True
                        qReagents.Add(newReagentRow)
                    End If
                Else
                    Dim newReagentRow As ReagentsDS.tparReagentsRow
                    newReagentRow = SelectedReagentsDS.tparReagents.NewtparReagentsRow()
                    newReagentRow.IsNew = True
                    qReagents.Add(newReagentRow)
                End If

                If qReagents.Count > 0 Then

                    For Each myReagent As ReagentsDS.tparReagentsRow In qReagents
                        RNumb += 1
                        'edit the reagents values
                        myReagent.BeginEdit()
                        If Not myReagent.IsReagentIDNull Then myReagentID = myReagent.ReagentID
                        myReagent.ReagentName = (pTestName & "-" & RNumb)
                        myReagent.ReagentNumber = RNumb  'TR 02/03/2012 -Set the reagent number.
                        myReagent.TS_User = GetApplicationInfoSession().UserName
                        myReagent.TS_DateTime = DateTime.Now
                        myReagent.EndEdit()

                        'validate if it's new to add it to the selectedReagents
                        If myReagent.RowState = DataRowState.Detached Then
                            myReagent.ReagentID = myReagentID + 1
                            SelectedReagentsDS.tparReagents.AddtparReagentsRow(myReagent)

                            'Update Test Reagents DS.
                            UpdateTestReagents(pTestId, qReagents.First().ReagentID, i)
                        End If
                    Next
                End If
                'clear the list.
                qTestReagent.Clear()
                qReagents.Clear()
            Next
            'TR 19/05/2010 END 

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateReagents ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update TestReagent Datasets
    ''' </summary>
    ''' <param name="pTestId"></param>
    ''' <param name="pReagentsId"></param>
    ''' <param name="pReagentsNumber"></param>
    ''' <remarks>Created by: TR 11/03/2010</remarks>
    Private Sub UpdateTestReagents(ByVal pTestId As Integer, ByVal pReagentsId As Integer, ByVal pReagentsNumber As Integer)
        Try
            Dim qSelectedTestReagents As New List(Of TestReagentsDS.tparTestReagentsRow)
            If SelectedTestReagentsDS.tparTestReagents.Count > 0 Then
                'filter the reagent by the reagent number.
                qSelectedTestReagents = (From a In SelectedTestReagentsDS.tparTestReagents _
                                    Where a.ReagentNumber = pReagentsNumber _
                                    Select a).ToList()

                If qSelectedTestReagents.Count = 0 Then
                    Dim newTestReagentsRow As TestReagentsDS.tparTestReagentsRow
                    newTestReagentsRow = SelectedTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                    newTestReagentsRow.IsNew = True
                    qSelectedTestReagents.Add(newTestReagentsRow)
                End If
            Else
                Dim newTestReagentsRow As TestReagentsDS.tparTestReagentsRow
                newTestReagentsRow = SelectedTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                newTestReagentsRow.IsNew = True
                qSelectedTestReagents.Add(newTestReagentsRow)
            End If

            If qSelectedTestReagents.Count > 0 Then
                qSelectedTestReagents.First().TestID = pTestId
                If qSelectedTestReagents.First().IsReagentIDNull Then qSelectedTestReagents.First().ReagentID = pReagentsId
                qSelectedTestReagents.First().ReagentNumber = pReagentsNumber

                If qSelectedTestReagents.First().RowState = DataRowState.Detached Then
                    'add the  row.
                    SelectedTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(qSelectedTestReagents.First())
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateNewTestReagents ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update the local stucture test sample.
    ''' </summary>
    ''' <param name="pTestID">Test Id </param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <remarks>
    ''' Created by: TR 11/03/2010
    ''' Modified by: SG 11/06/2010 Factor limits and Repetition range related textboxes became NumericUpDown
    ''' </remarks>
    Private Sub UpdateTestSample(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try

            Dim listDefault As List(Of TestSamplesDS.tparTestSamplesRow) = _
                    (From a In SelectedTestSamplesDS.tparTestSamples Where a.DefaultSampleType = True Select a).ToList

            Dim currentSampleTypeDefaultValue As Boolean = False
            If listDefault.Count > 0 Then
                'AG 28/09/2010 - Search if the pSampleType is marked as default or not
                Dim myDefaultSampleTypeValue As String = _
                            (From a In SelectedTestSamplesDS.tparTestSamples _
                             Where a.DefaultSampleType = True Select a.SampleType).First

                If pSampleType = myDefaultSampleTypeValue Then
                    currentSampleTypeDefaultValue = True
                End If
                'END AG 28/09/2010 

            Else
                'Assign new default sample type
                currentSampleTypeDefaultValue = True
            End If

            'TR 09/11/2010 -Clear the sample test.
            'SelectedTestSamplesDS.tparTestSamples.Clear()

            'TESTSAMPLESDS
            Dim qTestSampleRow As New List(Of TestSamplesDS.tparTestSamplesRow)

            If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                'Filter the SelectedTestSample by the TestID and SampleTye.
                qTestSampleRow = (From a In SelectedTestSamplesDS.tparTestSamples _
                                  Where a.TestID = pTestID And a.SampleType = pSampleType _
                                  Select a).ToList()
                'if not found create a new one. and add it to the list.
                If qTestSampleRow.Count = 0 Then

                    Dim newTestSampleRow As TestSamplesDS.tparTestSamplesRow
                    newTestSampleRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow()
                    newTestSampleRow.IsNew = True
                    qTestSampleRow.Add(newTestSampleRow)
                End If
                'if no row foun then add the new row to the list.
            Else
                Dim newTestSampleRow As TestSamplesDS.tparTestSamplesRow
                newTestSampleRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow()
                newTestSampleRow.IsNew = True
                qTestSampleRow.Add(newTestSampleRow)
            End If

            'set the values to the element.
            qTestSampleRow.First().TestID = pTestID
            qTestSampleRow.First().SampleType = pSampleType
            qTestSampleRow.First().TestLongName = ReportsNameTextBox.Text

            'TR 28/02/2011
            qTestSampleRow.First().EnableStatus = True

            'TR 16/05/2010 -Set the calibrator replicate.
            qTestSampleRow.First().CalibratorReplicates = CInt(CalReplicatesUpDown.Value)

            'TR 12/05/2011 -Get the itemIDDesc
            qTestSampleRow.First().ItemIDDesc = GetSampleTypeDescription(qTestSampleRow.First.SampleType)

            If SampleVolUpDown.Text <> "" Then
                qTestSampleRow.First().SampleVolume = CType(SampleVolUpDown.Value, Single)
            Else
                BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
            End If

            If WashSolVolUpDown.Text <> "" Then
                qTestSampleRow.First().WashingVolume = CType(WashSolVolUpDown.Value, Single)
            Else
                BsErrorProvider1.SetError(WashSolVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
            End If

            qTestSampleRow.First().PredilutionUseFlag = PredilutionFactorCheckBox.Checked
            If PredilutionFactorTextBox.Text <> "" Then
                qTestSampleRow.First().PredilutionFactor = CType(PredilutionFactorTextBox.Text, Single)
            Else 'AG 12/07/2010 - Add Else
                qTestSampleRow.First().SetPredilutionFactorNull()
            End If

            'TR 18/01/2011 -Add the Diluent solution.
            If Not DiluentComboBox.SelectedValue Is Nothing Then
                qTestSampleRow.First().DiluentSolution = DiluentComboBox.SelectedValue.ToString()

                'Dim myVolumePredilution As Single = CSng(ConfigurationManager.AppSettings("VolumePredilution").ToString())
                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myVolumePredilution As Single = GetVolumePredilution()
                'Calculate the the prediluted sample vol
                qTestSampleRow.First().PredilutedSampleVol = (myVolumePredilution / CType(PredilutionFactorTextBox.Text, Single))

                'Calculate the vol steps.
                qTestSampleRow.First().PredilutedSampleVolSteps = CalculateDilutionSteps(qTestSampleRow.First().PredilutedSampleVol, False)

                'calculate prediluted diluent volume.
                qTestSampleRow.First().PredilutedDiluentVol = (myVolumePredilution - qTestSampleRow.First().PredilutedSampleVol)

                'calculate the diluent vol steps.
                qTestSampleRow.First().PreDiluentVolSteps = CalculateDilutionSteps(qTestSampleRow.First().PredilutedDiluentVol, True)
            Else
                qTestSampleRow.First().DiluentSolution = String.Empty

            End If

            'TR 18/01/2011 -END

            If Not PredilutionModeCombo.SelectedValue Is Nothing Then
                qTestSampleRow.First().PredilutionMode = PredilutionModeCombo.SelectedValue.ToString()
            Else 'AG 12/07/2010 - Add Else
                qTestSampleRow.First().SetPredilutionModeNull()
            End If

            If RedPostDilutionFactorTextBox.Text <> "" Then
                qTestSampleRow.First().RedPostdilutionFactor = CType(RedPostDilutionFactorTextBox.Text, Single)
            End If
            'execute validate post dilution to get post reduce values.
            ValidatePostDilution()
            qTestSampleRow.First().WashingVolumeSteps = NormalVolumeSteps.WashStepsVol
            qTestSampleRow.First().SampleVolumeSteps = NormalVolumeSteps.sampleSteps
            qTestSampleRow.First().RedPostSampleVolume = PostReduce.sampleVol
            qTestSampleRow.First().RedPostSampleVolumeSteps = PostReduce.sampleSteps
            qTestSampleRow.First().IncPostSampleVolume = PostIncrease.sampleVol
            qTestSampleRow.First().IncPostSampleVolumeSteps = PostIncrease.sampleSteps
            If IncPostDilutionFactorTextBox.Text <> "" Then
                qTestSampleRow.First().IncPostdilutionFactor = CType(IncPostDilutionFactorTextBox.Text, Single)
            End If
            qTestSampleRow.First().AutomaticRerun = AutoRepetitionCheckbox.Checked
            'testSampleRow.AbsorbanceDilutionFactor = 
            If BlankAbsorbanceUpDown.Text <> "" Then
                qTestSampleRow.First().BlankAbsorbanceLimit = CType(BlankAbsorbanceUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetBlankAbsorbanceLimitNull()
            End If

            If LinearityUpDown.Text <> "" Then
                qTestSampleRow.First().LinearityLimit = CType(LinearityUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetLinearityLimitNull()
            End If

            If DetectionUpDown.Text <> "" Then
                qTestSampleRow.First().DetectionLimit = CType(DetectionUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetDetectionLimitNull()
            End If

            'TR 29/03/2010 Add the slope factor to save.
            If SlopeAUpDown.Text <> "" Then
                qTestSampleRow.First().SlopeFactorA = CType(SlopeAUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetSlopeFactorANull()
            End If

            If SlopeBUpDown.Text <> "" Then
                qTestSampleRow.First().SlopeFactorB = CType(SlopeBUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetSlopeFactorBNull()
            End If

            'SG 14/06/2010
            If Me.FactorLowerLimitUpDown.Text <> "" Then
                qTestSampleRow.First().FactorLowerLimit = CType(FactorLowerLimitUpDown.Value, Single)
            Else
                'TR 21/06/2010 
                qTestSampleRow.First().SetFactorLowerLimitNull()
            End If

            If Me.FactorUpperLimitUpDown.Text <> "" Then
                qTestSampleRow.First().FactorUpperLimit = CType(FactorUpperLimitUpDown.Value, Single)
            Else
                'TR 21/06/2010
                qTestSampleRow.First().SetFactorUpperLimitNull()
            End If

            'both of them are empty
            If Me.FactorLowerLimitUpDown.Text = "" And Me.FactorUpperLimitUpDown.Text = "" Then
                qTestSampleRow.First().SetFactorLowerLimitNull()
                qTestSampleRow.First().SetFactorUpperLimitNull()
            End If
            'END SG 14/06/2010

            'TR 12/04/2012
            If RerunLowerLimitUpDown.Text <> "" Then
                qTestSampleRow.First().RerunLowerLimit = CType(RerunLowerLimitUpDown.Value, Single)
            Else
                qTestSampleRow.First().SetRerunLowerLimitNull()
            End If

            If RerunLowerLimitUpDown.Text <> "" Then
                qTestSampleRow.First().RerunUpperLimit = CType(RerunUpperLimitUpDown.Value, Single)
            Else
                qTestSampleRow.First().SetRerunUpperLimitNull()
            End If
            'TR 12/04/2012 -END.

            'SG 11/06/2010 -Commented by TR 12/04/2012
            'If RerunLowerLimitUpDown.Value <> Nothing Then
            '    qTestSampleRow.First().RerunLowerLimit = CType(RerunLowerLimitUpDown.Value, Single)
            'End If

            'If RerunUpperLimitUpDown.Value <> Nothing Then
            '    qTestSampleRow.First().RerunUpperLimit = CType(RerunUpperLimitUpDown.Value, Single)
            'End If

            ''both of them are empty
            'If Me.RerunLowerLimitUpDown.Text = "" And Me.RerunUpperLimitUpDown.Text = "" Then
            '    qTestSampleRow.First().SetRerunLowerLimitNull()
            '    qTestSampleRow.First().SetRerunUpperLimitNull()
            'End If
            'END SG 11/06/2010  -Commented by TR 12/04/2012

            If SubstrateDepleUpDown.Text <> "" Then
                qTestSampleRow.First().SubstrateDepletionValue = CType(SubstrateDepleUpDown.Value, Single)
            Else
                qTestSampleRow.First().SetSubstrateDepletionValueNull() 'DL 20/07/2011
            End If

            'TR 22/03/2010 Validate selected calibration type to assigned value.
            If FactorRadioButton.Checked Then
                qTestSampleRow.First().CalibratorType = "FACTOR"
                qTestSampleRow.First().SampleTypeAlternative = "" 'TR 25/05/2011
                'TR 08/03/2011 -Set the Factory calibrator to false in case calibrator = factor
                qTestSampleRow.First().FactoryCalib = False
                'TR 08/03/2011 -END 

            ElseIf MultipleCalibRadioButton.Checked Then
                qTestSampleRow.First().CalibratorType = "EXPERIMENT"
                qTestSampleRow.First().SampleTypeAlternative = "" 'TR 25/05/2011
                'TR 08/03/2011 - validate if factory calib is true 
                If qTestSampleRow.First().FactoryCalib Then
                    ' if there was calibrator changes then set factory to false 
                    If CalibratorChanges Then
                        qTestSampleRow.First().FactoryCalib = False
                    End If
                End If
                'TR 08/03/2011 -END

                'TR 15/06/2010 -Add the Alternative calibrator
            ElseIf AlternativeCalibRadioButton.Checked Then
                qTestSampleRow.First().CalibratorType = "ALTERNATIV"
                If Not AlternativeCalComboBox.SelectedValue Is Nothing Then
                    qTestSampleRow.First().SampleTypeAlternative = AlternativeCalComboBox.SelectedValue.ToString()
                End If
                'TR 15/06/2010 -END
            End If
            'END TR 22/03/2010

            If CalibrationFactorTextBox.Text <> "" Then
                qTestSampleRow.First().CalibrationFactor = CType(CalibrationFactorTextBox.Text, Single)
            End If
            'testSampleRow.SampleTypeAlternative = 
            'qTestSampleRow.First().SpecificControl = False

            'AG 28/09/2010
            'qTestSampleRow.First().DefaultSampleType = True
            qTestSampleRow.First().DefaultSampleType = currentSampleTypeDefaultValue

            qTestSampleRow.First().TS_User = GetApplicationInfoSession.UserName
            qTestSampleRow.First().TS_DateTime = DateTime.Now()


            'validate the row state to see if it's a new or modified row
            If qTestSampleRow.First().RowState = DataRowState.Detached Then
                SelectedTestSamplesDS.tparTestSamples.AddtparTestSamplesRow(qTestSampleRow.First())
            End If
            Dim ReagentsNumber As Integer = 1
            If AnalysisModeCombo.SelectedValue.ToString() = "BREP" Or AnalysisModeCombo.SelectedValue.ToString() = "BRFT" _
                Or AnalysisModeCombo.SelectedValue.ToString() = "BRDIF" Or AnalysisModeCombo.SelectedValue.ToString() = "BRK" Then
                ReagentsNumber = 2
            End If
            UpdateReagentsVolume(pTestID, pSampleType, ReagentsNumber)
            'validate the reagents number and if all the sample type has all the volumes.
            If ReagentsNumber > 1 Then
                For Each TestSammplesRow As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples.Rows
                    SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.DefaultView.RowFilter = "SampleType = '" & _
                                                                                        TestSammplesRow.SampleType & "'"
                    If SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.DefaultView.Count < 2 Then
                        UpdateReagentsVolume(TestSammplesRow.TestID, TestSammplesRow.SampleType, ReagentsNumber)
                    End If
                    SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.DefaultView.RowFilter = ""
                Next
            End If

            'TR 06/04/2011 -Values on QCTAB that Belong to TestSample.
            qTestSampleRow.First().QCActive = QCActiveCheckBox.Checked
            qTestSampleRow.First().NumberOfControls = CountActiveControl() 'Calculate field. = number of active controls.

            If QCReplicNumberNumeric.Text <> "" Then
                qTestSampleRow.First().ControlReplicates = CInt(QCReplicNumberNumeric.Value)
            Else
                qTestSampleRow.First().SetControlReplicatesNull()
            End If

            If QCRejectionCriteria.Text <> "" Then
                qTestSampleRow.First().RejectionCriteria = QCRejectionCriteria.Value
            Else
                qTestSampleRow.First().SetRejectionCriteriaNull()
            End If

            If ManualRadioButton.Checked Then
                qTestSampleRow.First().CalculationMode = "MANUAL"
                qTestSampleRow.First().NumberOfSeries = 0
            ElseIf StaticRadioButton.Checked Then
                qTestSampleRow.First().CalculationMode = "STATISTIC"
                'Set the minimun num of series if checked the static.
                If QCMinNumSeries.Text <> "" Then
                    qTestSampleRow.First().NumberOfSeries = CInt(QCMinNumSeries.Value)
                Else
                    qTestSampleRow.First().SetNumberOfSeriesNull()
                End If
            Else
                qTestSampleRow.First().CalculationMode = String.Empty
                qTestSampleRow.First().NumberOfSeries = 0
            End If

            If QCErrorAllowable.Text <> "" Then
                qTestSampleRow.First.TotalAllowedError = QCErrorAllowable.Value
            Else
                qTestSampleRow.First.SetTotalAllowedErrorNull()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " .UpdateTestSample ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the global DS SampleTypeMasterDAtaDS
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 12/05/2011
    ''' </remarks>
    Private Sub GetAllSampleTypes()
        Try
            Dim myMasterDataDelegate As New MasterDataDelegate
            Dim myGlobalDataTO As New GlobalDataTO

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")
            If Not myGlobalDataTO.HasError Then
                SampleTypeMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " .GetAllSampleTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the sample type description by the sample Code.
    ''' </summary>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <returns></returns>
    ''' <remarks>Created by: TR 12/05/2011</remarks>
    Private Function GetSampleTypeDescription(ByVal pSampleType As String) As String
        Dim mySampleTypeDescription As String = ""
        Try
            Dim mySampleTypeList As New List(Of MasterDataDS.tcfgMasterDataRow)

            mySampleTypeList = (From a In SampleTypeMasterDataDS.tcfgMasterData _
                                Where a.ItemID = pSampleType Select a).ToList()
            If mySampleTypeList.Count > 0 Then
                mySampleTypeDescription = mySampleTypeList.First().ItemIDDesc
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " .GetSampleTypeDescription ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return mySampleTypeDescription
    End Function

    ''' <summary>
    ''' Update the Test Reagent Volumes local structures.
    ''' </summary>
    ''' <param name="pTestID">Test ID</param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <param name="pReagentsNumber">Reagents Number</param>
    ''' <remarks>
    ''' Created by: TR
    ''' </remarks>
    Private Sub UpdateReagentsVolume(ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pReagentsNumber As Integer)
        Try
            'TEST REAGENTS VOLUME.
            Dim ReagentsNumber As Integer = pReagentsNumber
            Dim qTestReagents As New List(Of TestReagentsDS.tparTestReagentsRow)
            Dim myTestReagentsVolROW As TestReagentsVolumesDS.tparTestReagentsVolumesRow
            Dim qTestsReagentsVol As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)

            For i As Integer = 1 To ReagentsNumber Step 1
                Dim myReagentNumber As Integer = i
                If SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Count > 0 Then
                    'query the Test reagent volume local stucture to see if it is an existing element or a new one
                    qTestsReagentsVol = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                        Where a.TestID = pTestID And a.SampleType = pSampleType _
                                        And a.ReagentNumber = myReagentNumber).ToList()
                    If qTestsReagentsVol.Count = 0 Then
                        myTestReagentsVolROW = SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.NewtparTestReagentsVolumesRow()
                        myTestReagentsVolROW.TestID = pTestID
                        myTestReagentsVolROW.IsNew = True
                        qTestsReagentsVol.Add(myTestReagentsVolROW)
                    End If
                Else
                    myTestReagentsVolROW = SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.NewtparTestReagentsVolumesRow()
                    myTestReagentsVolROW.TestID = pTestID
                    myTestReagentsVolROW.IsNew = True
                    qTestsReagentsVol.Add(myTestReagentsVolROW)
                End If
                'the reagent must exist before updating our test reagent volume.
                qTestReagents = (From a In SelectedTestReagentsDS.tparTestReagents _
                               Where a.TestID = pTestID AndAlso a.ReagentNumber = myReagentNumber _
                               Select a).ToList()

                If qTestReagents.Count > 0 Then
                    ValidatePostDilution()
                    'get the reagent id from SelectedReagentDS
                    qTestsReagentsVol.First().ReagentID = qTestReagents.First().ReagentID
                    qTestsReagentsVol.First().ReagentNumber = CByte(i)
                    qTestsReagentsVol.First().SampleType = pSampleType

                    If i = 1 Then
                        'set the values for R1
                        qTestsReagentsVol.First().ReagentVolume = CType(VolR1UpDown.Value, Single) 'required
                        qTestsReagentsVol.First().ReagentVolumeSteps = NormalVolumeSteps.R1Steps
                        qTestsReagentsVol.First().RedPostReagentVolume = PostReduce.R1Vol
                        qTestsReagentsVol.First().RedPostReagentVolumeSteps = PostReduce.R1Steps
                        qTestsReagentsVol.First().IncPostReagentVolume = PostIncrease.R1Vol
                        qTestsReagentsVol.First().IncPostReagentVolumeSteps = PostIncrease.R1Steps

                    Else
                        'set the values for R2
                        qTestsReagentsVol.First().ReagentVolume = CType(VolR2UpDown.Value, Single) 'required
                        qTestsReagentsVol.First().ReagentVolumeSteps = NormalVolumeSteps.R2Steps
                        qTestsReagentsVol.First().RedPostReagentVolume = PostReduce.R2Vol
                        qTestsReagentsVol.First().RedPostReagentVolumeSteps = PostReduce.R2Steps
                        qTestsReagentsVol.First().IncPostReagentVolume = PostIncrease.R2Vol
                        qTestsReagentsVol.First().IncPostReagentVolumeSteps = PostIncrease.R2Steps
                    End If

                    'AG 12/01/2012
                    'If RedPostDilutionFactorTextBox.Text <> "" Then
                    '    qTestsReagentsVol.First().RedPostReagentVolume = CType(RedPostDilutionFactorTextBox.Text, Single)
                    'End If

                    'If IncPostDilutionFactorTextBox.Text <> "" Then
                    '    qTestsReagentsVol.First().IncPostReagentVolume = CType(IncPostDilutionFactorTextBox.Text, Single)
                    'End If
                    'AG 12/01/2012

                    'Insert row if new.
                    If qTestsReagentsVol.First().RowState = DataRowState.Detached Then
                        SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AddtparTestReagentsVolumesRow(qTestsReagentsVol.First())
                    End If
                    SelectedTestReagentsVolumesDS.AcceptChanges()

                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateReagentsVolume ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' gets the sampletype of the test sample in the current test
    '''' </summary>
    '''' <param name="pTestID"></param>
    '''' <returns></returns>
    '''' <remarks>Created by SG 20/07/2010</remarks>
    'Private Function GetFirstSampleType(ByVal pTestID As Integer) As String
    '    'Dim myGlobalDataTO As New GlobalDataTO
    '    'Dim myTestSamplesDelegate As New TestSamplesDelegate
    '    Dim qSampleTypes As New List(Of MasterDataDS.tcfgMasterDataRow)

    '    Try
    '        qSampleTypes = GetSampleTypesByPosition()

    '        Dim qTestSampleRows As New List(Of TestSamplesDS.tparTestSamplesRow)

    '        If qSampleTypes IsNot Nothing AndAlso qSampleTypes.Count > 0 Then
    '            For Each pos As MasterDataDS.tcfgMasterDataRow In qSampleTypes
    '                qTestSampleRows = GetTestSampleListBySampleType(pTestID, pos.ItemID.Trim)
    '                If qTestSampleRows IsNot Nothing AndAlso qTestSampleRows.Count > 0 Then
    '                    Return qTestSampleRows(0).SampleType
    '                End If
    '            Next
    '            ' is newly edited
    '            Return "NEWLY_EDITED"
    '        Else
    '            Return ""
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetFirstSampleType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '        Return ""
    '    End Try
    'End Function

    ''' <summary>
    ''' Create new test localy
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateNewTest()
        Try
            Dim NewTestRow As TestsDS.tparTestsRow
            If SelectedTestDS.tparTests.Rows.Count > 0 Then
                NewTestRow = SelectedTestDS.tparTests(0)
            Else
                NewTestRow = SelectedTestDS.tparTests.NewtparTestsRow()
            End If
            NewTestRow.TestName = TestNameTextBox.Text
            NewTestRow.ShortName = ShortNameTextBox.Text
            NewTestRow.TestPosition = 1 'Set Temporal Value.
            NewTestRow.PreloadedTest = False 'TODO: Set the corresponding Control

            'AG 06/11/2012 - START: BA400 Change Test / Calculated Test programming on server Database
            'uncomment when you are creating new BioSystems tests. When finish comment this line
            'NewTestRow.PreloadedTest = True
            'AG 06/11/2012 - END: BA400 Change Test / Calculated Test programming on server Database

            NewTestRow.MeasureUnit = UnitsCombo.SelectedValue.ToString().TrimEnd()
            NewTestRow.AnalysisMode = AnalysisModeCombo.SelectedValue.ToString()
            NewTestRow.ReplicatesNumber = CType(ReplicatesUpDown.Value, Integer)

            Select Case AnalysisModeCombo.SelectedValue.ToString()

                Case "MREP", "MRFT", "MRK"
                    NewTestRow.ReagentsNumber = 1
                    Exit Select
                Case "BREP", "BRDIF", "BRFT", "BRK"
                    NewTestRow.ReagentsNumber = 2
                    Exit Select

                Case Else
                    Exit Select
            End Select

            NewTestRow.ReactionType = ReactionTypeCombo.SelectedValue.ToString().TrimEnd()
            NewTestRow.DecimalsAllowed = CType(DecimalsUpDown.Value, Integer)
            NewTestRow.TurbidimetryFlag = False 'TR 07/05/2011 -Do not implement turbidimetry set falset
            NewTestRow.AbsorbanceFlag = AbsCheckBox.Checked
            NewTestRow.ReadingMode = ReadingModeCombo.SelectedValue.ToString().TrimEnd()
            NewTestRow.FirstReadingCycle = CType(FirstReadingCycleUpDown.Value, Integer)
            'TR 07/04/2010
            If SecondReadingCycleUpDown.Text <> "" Then
                NewTestRow.SecondReadingCycle = CType(SecondReadingCycleUpDown.Value, Integer)
            End If
            'END TR 07/04/2010
            NewTestRow.MainWavelength = CInt(MainFilterCombo.SelectedValue)

            If Not ReferenceFilterCombo.SelectedValue Is Nothing Then
                NewTestRow.ReferenceWavelength = CInt(ReferenceFilterCombo.SelectedValue)
            Else 'AG 01/02/2011 - add Else case
                NewTestRow.SetReferenceWavelengthNull()
            End If

            If Not BlankTypesCombo.SelectedValue Is Nothing Then
                NewTestRow.BlankMode = BlankTypesCombo.SelectedValue.ToString().TrimEnd()
            End If

            'If BlankReplicatesUpDown.Text <> "" Then
            NewTestRow.BlankReplicates = CType(BlankReplicatesUpDown.Value, Integer)
            'End If
            If KineticBlankUpDown.Text <> "" Then
                NewTestRow.KineticBlankLimit = CType(KineticBlankUpDown.Value, Integer)
            End If

            NewTestRow.TestVersionNumber = 1
            NewTestRow.TestVersionDateTime = DateTime.Now
            NewTestRow.InUse = False
            NewTestRow.TS_User = GetApplicationInfoSession.UserName
            NewTestRow.TS_DateTime = DateTime.Now
            NewTestRow.NewTest = True

            For ReagNumber As Integer = 1 To NewTestRow.ReagentsNumber Step 1
                'Generate the new Reagent.
                CreateNewReagents(NewTestRow.TestName, ReagNumber, NewTestRow.TestID)
            Next

            'DL 11/01/2012. Begin
            'Dim mySampleType As String = SampleTypeCombo.Text ' CType(SampleTypeCombo.SelectedValue, TestSamplesDS.tparTestSamplesRow).SampleType.TrimEnd()
            'dim mySampleType As String = bsSampleTypeText.Text ' CType(SampleTypeCombo.SelectedValue, TestSamplesDS.tparTestSamplesRow).SampleType.TrimEnd()
            Dim mySampleType As String = SampleTypeCboEx.Text 'DL 16/01/2012
            'DL 11/01/2012. End

            'TR 22/07/2011 -Validate is not copy test.
            If Not CopyTestData Then
                'Create the TestSample.
                CreateNewTestSample(NewTestRow.TestID, mySampleType)
            End If

            'AG 07/07/2010
            NewTestRow.SampleType = mySampleType
            SelectedSampleTypeCombo.SelectedValue = NewTestRow.SampleType
            'END AG 07/07/2010

            'AG 13/01/2011
            ''SG 02/07/2010 Create TestRefRanges
            'Me.BsTestRefRanges.CreateNewTestRefRanges(NewTestRow.TestID, mySampleType)
            'TR 22/07/2011 -Validate is not copy test.
            If Not CopyTestData Then
                UpdateRefRangesChanges(NewTestRow.TestID, mySampleType, True)
            End If

            SelectedTestDS.tparTests.AcceptChanges()
            SelectedTestDS.tparTests.AcceptChanges()
            SelectedReagentsDS.tparReagents.AcceptChanges()
            SelectedTestReagentsDS.tparTestReagents.AcceptChanges()
            SelectedTestSamplesDS.tparTestSamples.AcceptChanges()
            SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AcceptChanges()
            SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges() 'SG 02/07/2010

            'TR 08/11/2010 -Not Needed.
            'InsertUpdateRow(NewTestRow.TestID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateNewTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Create a new Reagent on the local stuctures.
    ''' </summary>
    ''' <param name="pTestName">Test Name</param>
    ''' <param name="pReagNumber">Reagent Number</param>
    ''' <param name="pTestId">Test ID</param>
    ''' <remarks>Created by: TR</remarks>
    Private Sub CreateNewReagents(ByVal pTestName As String, ByVal pReagNumber As Integer, ByVal pTestId As Integer)
        Try
            Dim qReagents As New List(Of ReagentsDS.tparReagentsRow)

            If SelectedReagentsDS.tparReagents.Rows.Count > 0 Then

                qReagents = (From a In SelectedReagentsDS.tparReagents _
                            Where a.ReagentName = pTestName & "-" & pReagNumber _
                            Select a).ToList()

                If qReagents.Count = 0 Then
                    Dim newReagentRow As ReagentsDS.tparReagentsRow
                    newReagentRow = SelectedReagentsDS.tparReagents.NewtparReagentsRow()
                    qReagents.Add(newReagentRow)
                End If
            Else
                Dim newReagentRow As ReagentsDS.tparReagentsRow
                newReagentRow = SelectedReagentsDS.tparReagents.NewtparReagentsRow()
                qReagents.Add(newReagentRow)
            End If
            Dim reagId As Integer = 0
            If qReagents.Count > 0 Then
                qReagents.First().BeginEdit()
                If Not qReagents.First().IsReagentIDNull Then reagId = qReagents.First().ReagentID
                qReagents.First().ReagentName = (pTestName & "-" & pReagNumber)
                qReagents.First().ReagentNumber = pReagNumber 'TR 02/03/2012 -Set the reagent number.
                qReagents.First().TS_User = GetApplicationInfoSession().UserName
                qReagents.First().TS_DateTime = DateTime.Now
                qReagents.First().EndEdit()

                'validate if it is a new row to add it.
                If qReagents.First().RowState = DataRowState.Detached Then
                    'TR 21/07/2010 Use a random (temporally) value for reagent (In Database the identifier will be the correct)
                    qReagents.First().ReagentID = randObj.Next(TempReagentIDMin, TempReagentIDMax)
                    'qReagents.First().ReagentID = randObj.Next(1, 500)
                    SelectedReagentsDS.tparReagents.AddtparReagentsRow(qReagents.First())
                End If
                'Create new Test Reagents localy.
                CreateNewTestReagents(pTestId, qReagents.First().ReagentID, pReagNumber)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateNewReagents ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Create a new relation between the Test and the Reagents on the local 
    ''' stucture Test Reagents Data Set.
    ''' </summary>
    ''' <param name="pTestId">Test Id.</param>
    ''' <param name="pReagentsId">Reagent Id.</param>
    ''' <param name="pReagentsNumber">Reagents Number.</param>
    ''' <remarks>Created by: TR 10/03/2010 </remarks>
    Private Sub CreateNewTestReagents(ByVal pTestId As Integer, ByVal pReagentsId As Integer, ByVal pReagentsNumber As Integer)
        Try
            Dim qSelectedTestReagentes As New List(Of TestReagentsDS.tparTestReagentsRow)
            If SelectedTestReagentsDS.tparTestReagents.Count > 0 Then
                qSelectedTestReagentes = (From a In SelectedTestReagentsDS.tparTestReagents _
                                         Where a.TestID = pTestId And a.ReagentID = pReagentsId _
                                         And a.ReagentNumber = pReagentsNumber _
                                         Select a).ToList()

                If qSelectedTestReagentes.Count = 0 Then
                    Dim newTestReagentsRow As TestReagentsDS.tparTestReagentsRow
                    newTestReagentsRow = SelectedTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                    qSelectedTestReagentes.Add(newTestReagentsRow)
                End If
            Else
                Dim newTestReagentsRow As TestReagentsDS.tparTestReagentsRow
                newTestReagentsRow = SelectedTestReagentsDS.tparTestReagents.NewtparTestReagentsRow()
                qSelectedTestReagentes.Add(newTestReagentsRow)
            End If

            If qSelectedTestReagentes.Count > 0 Then
                qSelectedTestReagentes.First().TestID = pTestId
                qSelectedTestReagentes.First().ReagentID = pReagentsId
                qSelectedTestReagentes.First().ReagentNumber = pReagentsNumber
                If qSelectedTestReagentes.First().RowState = DataRowState.Detached Then
                    'add the  row.
                    SelectedTestReagentsDS.tparTestReagents.AddtparTestReagentsRow(qSelectedTestReagentes.First())
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateNewTestReagents ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Create a new Test Sample on the local stuctures.
    ''' </summary>
    ''' <param name="pTestID">Test ID.</param>
    ''' <param name="pSampleType">Sample Type.</param>
    ''' <remarks>Created by: TR
    ''' Modified by: SG 11/06/2010 Factor limits and Repetition range related textboxes became NumericUpDown
    ''' TR 18/01/2011 -Set the DiluentSolution, PredilutedSampleVol, PredilutedSampleVolSteps, 
    ''' PredilutedDiluentVol, PredilutedDiluentVolSteps.
    ''' </remarks>
    Private Sub CreateNewTestSample(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            'TESTSAMPLESDS
            Dim qtestSample As New List(Of TestSamplesDS.tparTestSamplesRow)

            If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                qtestSample = (From a In SelectedTestSamplesDS.tparTestSamples _
                              Where a.TestID = pTestID And a.SampleType = pSampleType _
                              Select a).ToList()
                If qtestSample.Count = 0 Then
                    Dim testSampleRow As TestSamplesDS.tparTestSamplesRow
                    testSampleRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow()
                    qtestSample.Add(testSampleRow)
                End If
            Else
                Dim testSampleRow As TestSamplesDS.tparTestSamplesRow
                testSampleRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow()
                qtestSample.Add(testSampleRow)
            End If
            qtestSample.First().TestID = pTestID
            qtestSample.First().SampleType = pSampleType
            qtestSample.First().TestLongName = ReportsNameTextBox.Text

            qtestSample.First().EnableStatus = True 'TR 25/02/2011

            qtestSample.First().FactoryCalib = False 'TR 08/03/2011

            qtestSample.First().ActiveRangeType = Me.bsTestRefRanges.RangeType 'SG 07/09/2010

            'TR 16/05/2010 -Set the calibrator replicate.
            qtestSample.First().CalibratorReplicates = CInt(CalReplicatesUpDown.Value)

            If SampleVolUpDown.Text <> "" Then
                qtestSample.First().SampleVolume = CType(SampleVolUpDown.Value, Single)
            Else
                BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
            End If
            qtestSample.First().SampleVolumeSteps = NormalVolumeSteps.sampleSteps

            If WashSolVolUpDown.Text <> "" Then
                qtestSample.First().WashingVolume = CType(WashSolVolUpDown.Value, Single)
            Else
                BsErrorProvider1.SetError(WashSolVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
            End If
            qtestSample.First().WashingVolumeSteps = NormalVolumeSteps.WashStepsVol

            qtestSample.First().PredilutionUseFlag = PredilutionFactorCheckBox.Checked

            If PredilutionFactorTextBox.Text <> "" Then
                qtestSample.First().PredilutionFactor = CType(PredilutionFactorTextBox.Text, Single)
            End If

            'TR 18/01/2011 -Set de dilutions

            If Not DiluentComboBox.SelectedValue Is Nothing Then
                qtestSample.First().DiluentSolution = DiluentComboBox.SelectedValue.ToString()

                'Dim myVolumePredilution As Single = CSng(ConfigurationManager.AppSettings("VolumePredilution").ToString())
                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myVolumePredilution As Single = GetVolumePredilution()

                'Calculate the the prediluted sample vol.
                qtestSample.First().PredilutedSampleVol = (myVolumePredilution / CType(PredilutionFactorTextBox.Text, Single))

                'Calculate the vol steps.
                qtestSample.First().PredilutedSampleVolSteps = CalculateDilutionSteps(qtestSample.First().PredilutedSampleVol, False)

                'calculate prediluted diluent volume.
                qtestSample.First().PredilutedDiluentVol = (myVolumePredilution - qtestSample.First().PredilutedSampleVol)

                'calculate the diluent vol steps.
                qtestSample.First().PreDiluentVolSteps = CalculateDilutionSteps(qtestSample.First().PredilutedDiluentVol, True)

            End If
            'TR 18/01/2011 -END

            If Not PredilutionModeCombo.SelectedValue Is Nothing Then
                qtestSample.First().PredilutionMode = PredilutionModeCombo.SelectedValue.ToString()
            End If

            If RedPostDilutionFactorTextBox.Text <> "" Then
                qtestSample.First().RedPostdilutionFactor = CType(RedPostDilutionFactorTextBox.Text, Single)
            End If

            qtestSample.First().RedPostSampleVolume = PostReduce.sampleVol
            qtestSample.First().RedPostSampleVolumeSteps = PostReduce.sampleSteps
            qtestSample.First().IncPostSampleVolume = PostIncrease.sampleVol
            qtestSample.First().IncPostSampleVolumeSteps = PostIncrease.sampleSteps

            If IncPostDilutionFactorTextBox.Text <> "" Then
                qtestSample.First().IncPostdilutionFactor = CType(IncPostDilutionFactorTextBox.Text, Single)
            End If

            qtestSample.First().AutomaticRerun = AutoRepetitionCheckbox.Checked

            'testSampleRow.AbsorbanceDilutionFactor = 
            If BlankAbsorbanceUpDown.Text <> "" Then
                qtestSample.First().BlankAbsorbanceLimit = CType(BlankAbsorbanceUpDown.Value, Single)
            End If

            If LinearityUpDown.Text <> "" Then
                qtestSample.First().LinearityLimit = CType(LinearityUpDown.Value, Single)
            End If

            If DetectionUpDown.Text <> "" Then
                qtestSample.First().DetectionLimit = CType(DetectionUpDown.Value, Single)
            End If
            'TR 29/03/2010
            If SlopeAUpDown.Text <> "" Then
                qtestSample.First().SlopeFactorA = CType(SlopeAUpDown.Value, Single)
            End If
            If SlopeBUpDown.Text <> "" Then
                qtestSample.First().SlopeFactorB = CType(SlopeBUpDown.Value, Single)
            End If

            'AG 20/09/2010
            If SubstrateDepleUpDown.Text <> "" Then
                qtestSample.First().SubstrateDepletionValue = CType(SubstrateDepleUpDown.Value, Single)
            End If
            'END AG 20/09/2010

            'SG 14/06/2010
            If FactorLowerLimitUpDown.Value <> Nothing Then
                qtestSample.First().FactorLowerLimit = CType(FactorLowerLimitUpDown.Value, Single)
            End If

            If FactorUpperLimitUpDown.Value <> Nothing Then
                qtestSample.First().FactorUpperLimit = CType(FactorUpperLimitUpDown.Value, Single)
            End If
            'END SG 14/06/2010

            'SG 11/06/2010
            If RerunLowerLimitUpDown.Value <> Nothing Then
                qtestSample.First().RerunLowerLimit = CType(RerunLowerLimitUpDown.Value, Single)
            End If

            If RerunUpperLimitUpDown.Value <> Nothing Then
                qtestSample.First().RerunUpperLimit = CType(RerunUpperLimitUpDown.Value, Single)
            End If
            'END SG 11/06/2010

            'TR 15/06/2010 -set the calibrator type
            If FactorRadioButton.Checked Then
                qtestSample.First().CalibratorType = "FACTOR"
            ElseIf MultipleCalibRadioButton.Checked Then
                qtestSample.First().CalibratorType = "EXPERIMENT"
            ElseIf AlternativeCalibRadioButton.Checked Then
                qtestSample.First().CalibratorType = "ALTERNATIV"
                If Not AlternativeCalComboBox.SelectedValue Is Nothing Then
                    qtestSample.First().SampleTypeAlternative = AlternativeCalComboBox.SelectedValue.ToString()
                End If
            End If
            'TR 15/06/2010 -END

            If CalibrationFactorTextBox.Text <> "" Then
                qtestSample.First().CalibrationFactor = CType(CalibrationFactorTextBox.Text, Single)
            End If

            'TR 15/05/2012 
            qtestSample.First().FactoryCalib = False

            'TR 06/04/2011 -Set Control Values
            qtestSample.First().QCActive = QCActiveCheckBox.Checked
            ' review by tomas
            'qtestSample.First().NumberOfControls = CountActiveControl()
            If QCReplicNumberNumeric.Text <> "" Then
                qtestSample.First().ControlReplicates = CInt(QCReplicNumberNumeric.Value)
            End If

            If QCRejectionCriteria.Text <> "" Then
                qtestSample.First().RejectionCriteria = QCRejectionCriteria.Value
            End If

            If ManualRadioButton.Checked Then
                qtestSample.First().CalculationMode = "MANUAL"
                qtestSample.First().NumberOfSeries = 0
            ElseIf StaticRadioButton.Checked Then
                qtestSample.First().CalculationMode = "STATISTIC"
                If QCMinNumSeries.Text <> "" Then
                    'Set the minimun num of series if checked the static.
                    qtestSample.First().NumberOfSeries = CInt(QCMinNumSeries.Value)
                End If
            Else
                qtestSample.First().CalculationMode = String.Empty
                qtestSample.First().NumberOfSeries = 0
            End If

            'Get the number of active controls
            qtestSample.First().NumberOfControls = CountActiveControl()

            If QCErrorAllowable.Text <> "" Then
                qtestSample.First.TotalAllowedError = QCErrorAllowable.Value
            End If

            'Set values for Multirules
            UpdateTestSampleMultiRules(pTestID, pSampleType)
            'TR 06/04/2011 -End

            'qtestSample.First().SpecificControl = False
            qtestSample.First().DefaultSampleType = True
            qtestSample.First().IsNew = True
            qtestSample.First().TS_User = GetApplicationInfoSession.UserName
            qtestSample.First().TS_DateTime = DateTime.Now()
            'validate the row state to see if it's a new or modified row
            If qtestSample.First().RowState = DataRowState.Detached Then
                SelectedTestSamplesDS.tparTestSamples.AddtparTestSamplesRow(qtestSample.First())
            End If
            Dim ReagentsNumber As Integer = 1
            If AnalysisModeCombo.SelectedValue.ToString() = "BREP" Or AnalysisModeCombo.SelectedValue.ToString() = "BRFT" _
                Or AnalysisModeCombo.SelectedValue.ToString() = "BRDIF" Or AnalysisModeCombo.SelectedValue.ToString() = "BRK" Then
                ReagentsNumber = 2
            End If

            CreateNewReagentsVolume(pTestID, pSampleType, ReagentsNumber)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CreateNewReagentsVolume ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Create a new Test Reagent Volume inthe local structure.
    ''' </summary>
    ''' <param name="pTestID">Test ID</param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <param name="pReagentsNumber">Reagent Number</param>
    ''' <remarks>
    ''' Created by: TR
    ''' Modified by: TR 19/03/2010.
    ''' </remarks>
    Private Sub CreateNewReagentsVolume(ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pReagentsNumber As Integer)
        Try
            '/TEST REAGENTS VOLUME.
            Dim ReagentsNumber As Integer = pReagentsNumber
            If AnalysisModeCombo.SelectedValue.ToString() = "BREP" Or AnalysisModeCombo.SelectedValue.ToString() = "BRFT" _
                Or AnalysisModeCombo.SelectedValue.ToString() = "BRDIF" Or AnalysisModeCombo.SelectedValue.ToString() = "BRK" Then
                ReagentsNumber = 2
            End If
            Dim myTestReagentsVolROW As TestReagentsVolumesDS.tparTestReagentsVolumesRow
            Dim qSelectedTestReagentVol As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
            Dim qTestReagents As New List(Of TestReagentsDS.tparTestReagentsRow)

            For i As Integer = 1 To ReagentsNumber Step 1
                Dim myReagentNumber As Integer = i
                'Validate if TestreagentVol exist on TestReagentVolDS
                If SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Rows.Count > 0 Then
                    qSelectedTestReagentVol = (From a In SelectedTestReagentsVolumesDS.tparTestReagentsVolumes _
                                               Where a.TestID = pTestID And a.SampleType = pSampleType And _
                                               a.ReagentNumber = myReagentNumber Select a).ToList()
                    If qSelectedTestReagentVol.Count = 0 Then
                        myTestReagentsVolROW = SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.NewtparTestReagentsVolumesRow()
                        myTestReagentsVolROW.TestID = pTestID
                        qSelectedTestReagentVol.Add(myTestReagentsVolROW)
                    End If
                Else
                    myTestReagentsVolROW = SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.NewtparTestReagentsVolumesRow()
                    myTestReagentsVolROW.TestID = pTestID
                    qSelectedTestReagentVol.Add(myTestReagentsVolROW)
                End If

                'get the reagents id by the test id and the reagents number.
                qTestReagents = (From a In SelectedTestReagentsDS.tparTestReagents _
                                Where a.TestID = pTestID AndAlso a.ReagentNumber = CByte(myReagentNumber) _
                                Select a).ToList()

                If qTestReagents.Count > 0 Then
                    'get the reagent id from SelectedReagentDS
                    qSelectedTestReagentVol.First().ReagentID = qTestReagents.First().ReagentID  'TODO: Set the real reagent value.
                    qSelectedTestReagentVol.First().ReagentNumber = CByte(i)
                    qSelectedTestReagentVol.First().SampleType = pSampleType

                    If i = 1 Then
                        'set the values for R1
                        qSelectedTestReagentVol.First().ReagentVolume = CType(VolR1UpDown.Value, Single) 'required
                        qSelectedTestReagentVol.First().ReagentVolumeSteps = NormalVolumeSteps.R1Steps
                        qSelectedTestReagentVol.First().RedPostReagentVolume = PostReduce.R1Vol
                        qSelectedTestReagentVol.First().RedPostReagentVolumeSteps = PostReduce.R1Steps
                        qSelectedTestReagentVol.First().IncPostReagentVolume = PostIncrease.R1Vol
                        qSelectedTestReagentVol.First().IncPostReagentVolumeSteps = PostIncrease.R1Steps
                        qSelectedTestReagentVol.First().IsNew = True
                    Else
                        'set the values for R2
                        qSelectedTestReagentVol.First().ReagentVolume = CType(VolR2UpDown.Value, Single) 'required
                        qSelectedTestReagentVol.First().ReagentVolumeSteps = NormalVolumeSteps.R2Steps
                        qSelectedTestReagentVol.First().RedPostReagentVolume = PostReduce.R2Vol
                        qSelectedTestReagentVol.First().RedPostReagentVolumeSteps = PostReduce.R2Steps
                        qSelectedTestReagentVol.First().IncPostReagentVolume = PostIncrease.R2Vol
                        qSelectedTestReagentVol.First().IncPostReagentVolumeSteps = PostIncrease.R2Steps
                        qSelectedTestReagentVol.First().IsNew = True
                    End If

                    'AG 12/01/2012
                    'If RedPostDilutionFactorTextBox.Text <> "" Then
                    '    qSelectedTestReagentVol.First().RedPostReagentVolume = CType(RedPostDilutionFactorTextBox.Text, Single)
                    'End If

                    'If IncPostDilutionFactorTextBox.Text <> "" Then
                    '    qSelectedTestReagentVol.First().IncPostReagentVolume = CType(IncPostDilutionFactorTextBox.Text, Single)
                    'End If
                    'AG 12/01/2012

                    If qSelectedTestReagentVol.First().RowState = DataRowState.Detached Then
                        'Insert row.
                        SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AddtparTestReagentsVolumesRow(qSelectedTestReagentVol.First())
                    End If
                    SelectedTestReagentsVolumesDS.AcceptChanges()
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateNewReagentsVolume ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    '''  Save changen on the database (New Implementation)
    ''' </summary>
    ''' <param name="pSaveButtonClicked">TRUE user press Save button / FALSE delete tests</param>
    ''' <param name="pBindControls" ></param>
    ''' <param name="pDeleteSampleType" ></param>
    ''' <remarks>
    ''' Created by: TR 08/11/2010
    ''' AG 12/11/2010 - add parameter
    ''' </remarks>
    Private Function SaveChanges(ByVal pSaveButtonClicked As Boolean, Optional ByVal pBindControls As Boolean = True, Optional ByVal pDeleteSampleType As Boolean = False) As Boolean
        Dim Result As Boolean = False
        Try
            'AG 11/11/2010 -  Validate on save!!!º
            'ValidateAllTabs(pSaveButtonClicked) 'AG 03/12/2010 - add optional parameter
            If Not CopyTestData Then ValidateAllTabs(pSaveButtonClicked) 'TR 10/01/2012 -Validate is not a copy Test
            If Not ValidationError Then
                'AG 11/11/2010
                Dim myTestDelegate As New TestsDelegate()
                Dim myGlobalDataTO As New GlobalDataTO()
                'Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow) 'SG 17/06/2010

                'TR 19/05/2011 -Set the value before cleaning the tables
                Dim myUpdateSampleType As String = String.Empty
                If Not SelectedSampleTypeCombo.SelectedValue Is Nothing Then
                    myUpdateSampleType = SelectedSampleTypeCombo.SelectedValue.ToString()
                End If
                'TR 19/05/2011 -END.

                'AG 11/02/2011 - Add pDeleteSampleType condition 'AG 12/11/2010
                If Not pSaveButtonClicked And pDeleteSampleType Then ' Clear all DS but the DeletedTestProgramingList
                    SelectedTestDS.Clear()
                    SelectedTestSamplesDS.Clear()
                    SelectedTestReagentsVolumesDS.Clear()
                    SelectedReagentsDS.Clear()
                    SelectedTestReagentsDS.Clear()
                    UpdatedCalibratorsDS.Clear()
                    UpdatedTestCalibratorDS.Clear()
                    SelectedTestCalibratorValuesDS.Clear()
                    SelectedTestRefRangesDS.Clear()
                    'TR 08/04/2011 -Clear stuctures.
                    SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Clear()
                    SelectedTestControlDS.tparTestControls.Clear()
                    'TR 08/04/2011
                    DeletedCalibratorList.Clear()
                    DeletedTestReagentVolList.Clear()
                    'Else
                    'AG 13/01/2011 - Comment due the DS is updated in method UpdateRefRangesChanges
                    'SelectedTestRefRangesDS = bsTestRefRanges.DefinedTestRangesDS
                End If
                'END AG 12/11/2010

                'Prepare the DS containing the group of linked Controls
                Dim NumComtrol As Integer = SelectedTestControlDS.tparTestControls.Count() - 1
                For i As Integer = 0 To NumComtrol
                    If (SelectedTestControlDS.tparTestControls(i).IsControlIDNull) Then
                        'Remove rows without ControlID 
                        SelectedTestControlDS.tparTestControls(i).Delete()
                    Else
                        'Inform the TestType in rows having the ControlID informed
                        SelectedTestControlDS.tparTestControls(i).TestType = "STD"
                    End If
                Next
                SelectedTestControlDS.AcceptChanges()

                'TR 10/11/2010 -Use the selectedTestCalibratorValues instead UpdatedTestCalibratorValuesDS.
                'Call the delagate and send the data.
                myGlobalDataTO = myTestDelegate.PrepareTestToSave(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                                  SelectedTestDS, SelectedTestSamplesDS, SelectedTestReagentsVolumesDS, SelectedReagentsDS, _
                                                                  SelectedTestReagentsDS, UpdatedCalibratorsDS, UpdatedTestCalibratorDS, UpdatedTestCalibratorValuesDS, _
                                                                  SelectedTestRefRangesDS, DeletedCalibratorList, DeletedTestReagentVolList, DeletedTestProgramingList, _
                                                                  SelectedTestSampleMultirulesDS, SelectedTestControlDS, LocalDeleteControlTOList, myUpdateSampleType)

                If (myGlobalDataTO.HasError) Then
                    'Show error
                    ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                    Result = False
                Else
                    'TR 22/07/2011 -Set value to false after creating in case test was copy.
                    CopyTestData = False
                    SelectedSampleTypeCombo.Enabled = True 'TR 26/01/2012
                    'AG 12/01/2011 - After save set to FALSE the field IsNew/NewTest in SelectedDS
                    'Remove rows with IsDeleted in SelectedTestRefRangesDS
                    SetToFalseInternalDataSetsFlags()
                    'AG 12/01/2011

                    'Prepare all local Variable.
                    'EditionMode = False
                    'bsTestRefRanges.isEditing = False 'SG 06/09/2010
                    If pSaveButtonClicked Then
                        EditionMode = False
                        bsTestRefRanges.isEditing = False 'SG 06/09/2010
                    End If
                    ChangesMade = False
                    CalibratorChanges = False 'TR 08/03/2011
                    Result = True
                    'NewTest = False 'TR 18/11/2010 'AG 12/01/2011

                    'TR 18/11/2010  -END
                    'set status redy to save
                    ReadyToSave = False

                    'CLEAR THE DELETE STUCTURES
                    DeletedTestReagentVolList.Clear()
                    DeletedTestProgramingList.Clear()

                    LocalDeleteControlTOList.Clear() 'TR 24/05/2011

                    'TR 17/11/2010 -Clear the Updated structures
                    UpdatedCalibratorsDS.tparCalibrators.Clear()
                    UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                    'TR 17/11/2010 -END 

                    'TR 06/05/2011 -Clear QCControl Data Structures
                    SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Clear()
                    'TR 06/05/2011 -END 

                    If pBindControls Then 'TR 21/12/2010 -Validate if want to bind an clear controls.
                        'TR 18/11/2010 
                        If Not AnalysisModeCombo.SelectedValue Is Nothing Then
                            CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())
                        End If

                        'AG 12/01/2011
                        Dim currentTestIndex As Integer = 0
                        If Not NewTest And TestListView.Items.Count > 0 Then
                            If pSaveButtonClicked Then
                                currentTestIndex = TestListView.SelectedItems(0).Index
                            Else
                                currentTestIndex = 0
                            End If
                        End If

                        FillTestListView()

                        If TestListView.Items.Count > 0 Then
                            If TestListView.SelectedItems.Count > 0 Then
                                TestListView.Items(TestListView.SelectedItems(0).Index).Selected = True
                                'TR 21/12/2010 -Send parammeters sample type.
                                'DL 11/01/2012
                                'BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCombo.SelectedValue.ToString())
                                BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCboEx.Text)
                                'DL 11/01/2012
                            Else
                                'AG 12/01/2011
                                If Not NewTest Then
                                    'TR 24/01/2011 -Removed the -1 because the current index is the index position.
                                    TestListView.Items(currentTestIndex).Selected = True
                                    BindControls(CType(TestListView.Items(currentTestIndex).Name, Integer))
                                    'TR 24/01/2011 -END.
                                Else 'New Test
                                    currentTestViewEditionItem = CInt(TestListView.Items(TestListView.Items.Count - 1).Name)
                                    TestListView.Items(TestListView.Items.Count - 1).Selected = True
                                    BindControls(CType(TestListView.Items(TestListView.Items.Count - 1).Name, Integer))
                                    NewTest = False
                                End If
                                'AG 12/01/2011
                            End If

                            'TR 08/03/2011 -Validate if factory calibrator values.
                            If ValidateCalibratorFactoryValues(SelectedTestDS.tparTests(0).TestID, SelectedSampleTypeCombo.SelectedValue.ToString()) Then
                                ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString())
                            End If
                            'TR 08/03/2011 -END.
                        Else
                            ClearAllControls()
                        End If
                    End If

                End If
            End If 'AG 11/11/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return Result
    End Function

    ''' <summary>
    ''' Enable or disable the controls on the screen 
    ''' </summary>
    ''' <param name="pControls"></param>
    ''' <param name="pEnable"></param>
    ''' <param name="pInitialCall"></param>
    ''' <remarks>
    ''' Created by: TR
    ''' Modified by: AG 10/03/10 - Add optional parameter pInitialCall. (AG 19/03/2010 - Default value FALSE)
    '''              JV 23/01/2014 #1013 - Add optional parameter pSupervisor
    ''' </remarks>
    Private Sub EnableDisableControls(ByVal pControls As Control.ControlCollection, _
                                      ByVal pEnable As Boolean, _
                                      Optional ByVal pInitialCall As Boolean = False, _
                                      Optional ByVal pSupervisor As Boolean = False)
        Try
            'And  DirectCast(myControl.GetType(), System.Type).Name <> "BSButton"
            For Each myControl As Control In pControls

                If myControl.Controls.Count > 0 Then

                    'TR 14/06/2010 -Do not enable calibrator data control.
                    'If Not myControl.Name = "SpecificCalibInfoGroupBox" Then
                    '    EnableDisableControls(myControl.Controls, pEnable)
                    'End If

                    Select Case myControl.Name
                        Case "bsTestRefRanges"
                            If pSupervisor Then
                                myControl.Enabled = pSupervisor
                            Else
                                EnableDisableControls(myControl.Controls, pEnable)
                            End If
                        Case "SpecificCalibInfoGroupBox"
                            'Nothing to do
                        Case Else
                            EnableDisableControls(myControl.Controls, pEnable)
                    End Select

                Else
                    If DirectCast(myControl.GetType(), System.Type).Name <> "BSLabel" And _
                       DirectCast(myControl.GetType(), System.Type).Name <> "BSListView" Then

                        'DL 16/01/2012. CONTROL CAN NOT BE ABLE
                        If myControl.Name <> "SampleTypeCboEx" And _
                           myControl.Name <> "SampleTypeCboAux" And _
                           myControl.Name <> "SampleTypeCheckList" Then
                            myControl.Enabled = pEnable
                        ElseIf String.Compare(myControl.Name, "SampleTypeCboEx", False) = 0 Then
                            If pEnable Then
                                myControl.Visible = True
                            Else
                                myControl.Visible = False
                            End If

                        ElseIf myControl.Name = "SampleTypeCboAux" Then
                            If pEnable Then
                                myControl.Visible = False
                            Else
                                myControl.Visible = True
                            End If

                        End If

                        If DirectCast(myControl.GetType(), System.Type).Name = "BSTextBox" OrElse _
                            DirectCast(myControl.GetType(), System.Type).Name = "BSNumericUpDown" Then 'OrElse _
                            'DirectCast(myControl.GetType(), System.Type).Name = "BSComboBoxEx" Then

                            If pEnable Then
                                myControl.BackColor = Color.White
                            Else
                                myControl.BackColor = SystemColors.MenuBar
                            End If
                        End If
                    End If

                    'AG 18/03/2010 - By now (march 2010) absorbance flag always disabled
                    If myControl.Name = "AbsCheckBox" Then myControl.Enabled = False
                    'TR 14/06/2010 -Allways disable the Calibrator info control.
                    'DL 18/07/2011. Begin
                    'SpecificCalibInfoGroupBox.Enabled = False
                    EnableSpecificCalibInfoGroupBox(False)
                    'DL 18/07/2011. End

                    If myControl.Name = "QCActiveCheckBox" Then
                        ActivateQCControlsByQCActive(DirectCast(myControl, BSCheckbox).Checked)
                    End If
                End If
            Next

            'AG 10/03/10
            If pInitialCall Then
                'If pEnable Then
                Me.LoadScreenStatus(WorkSessionIDAttribute, WorkSessionStatusAttribute, isCurrentTestInUse)
                'Else
                'SaveButton.Enabled = False
                'End If
            End If
            'END AG 10/03/10

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableDisableControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Enable or disable the content controls on SpecificCalibInfoGroupBox 
    ''' </summary>
    ''' <param name="pState"></param>
    ''' <remarks>
    ''' Created by: DL 18/07/2011
    ''' </remarks>
    Private Sub EnableSpecificCalibInfoGroupBox(ByVal pState As Boolean)
        Try
            BsCalibNumberTextBox.Enabled = pState
            CalibratorNameTextBox.Enabled = pState
            CalibratorLotTextBox.Enabled = pState
            CalibratorExpirationDate.Enabled = pState

            IncreasingRadioButton.Enabled = pState
            DecreasingRadioButton.Enabled = pState
            XAxisCombo.Enabled = pState
            YAxisCombo.Enabled = pState
            CurveTypeCombo.Enabled = pState

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableSpecificCalibInfoGroupBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate the values for the blankAbsorbance 
    ''' </summary>
    ''' <param name="BlankAbsValue">Blank Absorbance Value</param>
    ''' <remarks>
    ''' Created by:TR 
    ''' MODIFIE BY: TR 19/03/2010
    ''' </remarks>
    Private Sub BlankAbsorbanceValidation(ByVal BlankAbsValue As Decimal)
        Try
            Dim AbsorbanceValue As Double = 0
            AbsorbanceValue = CType(BlankAbsValue, Double)
            If AbsorbanceValue < 0 OrElse AbsorbanceValue > 2.3 Then
                ' BsErrorProvider1.SetError(BlankAbsorbanceUpDown, GetMessageText(GlobalEnumerates.Messages.BLANKABSORBANCE_LIMITS.ToString)) 'AG 07/07/2010("BLANKABSORBANCE_LIMITS"))
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " BlankAbsorbnceValidation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' Post dilution validation
    '''' </summary>
    '''' <param name="pPostDilutionType"></param>
    '''' <remarks>
    '''' Created by: TR
    '''' Modified by: TR 19/03/2010.
    '''' </remarks>
    'Private Sub PostDilutionsValidation(ByVal pPostDilutionType As String)
    '    Try
    '        Dim calculate As Boolean = True
    '        Dim R1PlusR2 As Double = 0

    '        R1PlusR2 = VolR1UpDown.Value + VolR2UpDown.Value

    '        If R1PlusR2 = 0 Then
    '            RedPostDilutionFactorTextBox.Text = "0"
    '            IncPostDilutionFactorTextBox.Text = "0"
    '        End If

    '        'Validate if the required data is complete depending the reading mode.
    '        If ReadingModeCombo.SelectedValue.ToString() = "MONO" Then
    '            'validate the MONO required fields.
    '            If SampleVolUpDown.Text = "" Or VolR1UpDown.Text = "" Then
    '                ' indicate the missing value 
    '                calculate = False
    '            End If

    '        ElseIf ReadingModeCombo.SelectedValue.ToString() = "BIC" Then
    '            'validate the BIC required fields.
    '            If SampleVolUpDown.Text = "" Or VolR1UpDown.Text = "" Or VolR2UpDown.Text = "" Then
    '                'indicate the missing value.
    '                calculate = False
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PostDilutionsValidation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try

    'End Sub

    '''' <summary>
    '''' enable controls to be edited
    '''' </summary>
    '''' <remarks>
    '''' Created by: TR
    '''' Modified by:  AG 10/03/10 (Add if condition)
    '''' </remarks>
    'Private Sub EnableEditionOLD12072010()
    '    Try
    '        If Not isCurrentTestInUse Then
    '            If isEditTestAllowed Then
    '                If Not EditionMode Then
    '                    EditionMode = True
    '                    EnableDisableControls(TestDetailsTabs.Controls, True, True)    '19/03/2010 - Add 3hd parameter (optional)
    '                    CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())
    '                    'RefRadioButtonsManagement() 'SG 21/06/2010
    '                    Me.bsTestRefRanges.isEditing = True 'SG 06/09/2010
    '                    'RefDetailAddEditEnabling() 'SG 27/06/2010

    '                    EditButton.Enabled = False   'AG 10/03/10 - Once we are in edition mode disable edition button
    '                Else
    '                    'AG 10/03/10
    '                    If SavePendingWarningMessage() = Windows.Forms.DialogResult.Yes Then
    '                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
    '                    End If

    '                End If
    '            End If

    '            'TR 15/03/2010 -Do extra validation for Monoreagent.
    '            If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
    '                ReferenceFilterCombo.SelectedIndex = -1
    '                ReferenceFilterCombo.Enabled = False
    '            Else
    '                'AG 19/03/2010
    '                'ReferenceFilterCombo.SelectedIndex = -1
    '                If ReferenceFilterCombo.SelectedValue Is Nothing Then ReferenceFilterCombo.SelectedIndex = -1

    '                ReferenceFilterCombo.Enabled = True
    '            End If
    '        End If

    '        EnableDisableCalibratorControls()   'TR 29/06/2010 -Enable and disable controls depending selection.

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableEditionOLD12072010 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try

    'End Sub

    ''' <summary>
    ''' enable controls to be edited
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR
    ''' Modified by:  AG 10/03/10 (Add if condition)
    ''' </remarks>
    Private Sub EnableEdition()
        Try
            If Not isCurrentTestInUse Then
                If isEditTestAllowed Then
                    If Not EditionMode Then
                        EditionMode = True
                        ReadyToSave = True 'AG 12/11/2010
                        currentTestViewEditionItem = CInt(TestListView.SelectedItems(0).Name) 'AG 15/11/2010
                        EnableDisableControls(TestDetailsTabs.Controls, True, True)    '19/03/2010 - Add 3hd parameter (optional)
                        CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())

                        SelectedSampleTypeCombo.Enabled = False 'TR 22/07/2013 -BUG #1229

                        'Me.bsTestRefRanges.isEditing = False 'True  'AG 13/01/2011
                        'RefRadioButtonsManagement() 'SG 21/06/2010
                        'RefDetailAddEditEnabling() 'SG 27/06/2010
                        EditButton.Enabled = False   'AG 10/03/10 - Once we are in edition mode disable edition button
                        CopyTestButton.Enabled = False 'TR 09/01/2012 -Disable copyTest button.
                        BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869

                        ' DL 26/08/2010
                        If Not SelectedTestDS.tparTests.Item(0).IsSpecialTestNull Then
                            If SelectedTestDS.tparTests.Item(0).SpecialTest Then

                                ' DL 02/09/2010
                                Dim mySpecialTestsSettings As New SpecialTestsSettingsDelegate
                                Dim mySpecialTestsSettingsDS As New SpecialTestsSettingsDS
                                Dim resultdata As New GlobalDataTO
                                Dim myFixedName As Boolean = False
                                Dim myFixedCalibrator As Boolean = False
                                Dim myFixedSampleType As Boolean = False

                                ' FIXED_NAME
                                resultdata = mySpecialTestsSettings.Read(Nothing, _
                                                                         SelectedTestDS.tparTests(0).TestID, _
                                                                         SelectedSampleTypeCombo.SelectedValue.ToString(), _
                                                                         "FIXED_NAME")

                                If Not resultdata.HasError Then
                                    mySpecialTestsSettingsDS = CType(resultdata.SetDatos, SpecialTestsSettingsDS)

                                    If mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0 Then
                                        myFixedName = CType(mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Item(0).SettingValue, Boolean)
                                    End If
                                End If

                                ' FIXED_CALIBRATOR
                                resultdata = mySpecialTestsSettings.Read(Nothing, _
                                                                         SelectedTestDS.tparTests(0).TestID, _
                                                                         SelectedSampleTypeCombo.SelectedValue.ToString(), _
                                                                         "FIXED_CALIBRATOR")

                                If Not resultdata.HasError Then
                                    mySpecialTestsSettingsDS = CType(resultdata.SetDatos, SpecialTestsSettingsDS)

                                    If mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0 Then
                                        myFixedCalibrator = CType(mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Item(0).SettingValue, Boolean)
                                    End If
                                End If

                                ' FIXED_SAMPLETYPE
                                resultdata = mySpecialTestsSettings.Read(Nothing, _
                                                                         SelectedTestDS.tparTests(0).TestID, _
                                                                         SelectedSampleTypeCombo.SelectedValue.ToString(), _
                                                                         "FIXED_SAMPLETYPE")

                                If Not resultdata.HasError Then
                                    mySpecialTestsSettingsDS = CType(resultdata.SetDatos, SpecialTestsSettingsDS)

                                    If mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Rows.Count > 0 Then
                                        myFixedSampleType = CType(mySpecialTestsSettingsDS.tfmwSpecialTestsSettings.Item(0).SettingValue, Boolean)
                                    End If
                                End If

                                ' End DL 02/09/2010

                                If myFixedName Then
                                    TestNameTextBox.Enabled = False

                                End If

                                If myFixedSampleType Then
                                    'AddSampleTypeButton.Enabled = False 'DL 11/01/2012
                                    DeleteSampleTypeButton.Enabled = False
                                    SampleTypeCboAux.Visible = True
                                    SampleTypeCboAux.Enabled = False 'TR 18/04/2012
                                    SampleTypeCboEx.Visible = False 'TR 18/04/2012
                                End If

                                If myFixedCalibrator Then
                                    FactorRadioButton.Enabled = False
                                    CalibrationFactorTextBox.Enabled = False

                                    MultipleCalibRadioButton.Enabled = False

                                    AlternativeCalibRadioButton.Enabled = False
                                    AlternativeCalComboBox.Enabled = False
                                End If
                            End If
                        End If
                        ' END DL

                        'TR 29/03/2012 -Enable by User Label
                        If CurrentUserLevel = "SUPERVISOR" Then
                            'TODO: Define & program the user level limited permissions for supervisor users
                            If isPreloadedTest Then
                                'Disable General Tab
                                EnableDisableControls(TestDetailsTabs.TabPages("GeneralTab").Controls, False)
                                'Disable Procedure Tab
                                EnableDisableControls(TestDetailsTabs.TabPages("ProcedureTab").Controls, False)
                                'Disable Options Tab
                                EnableDisableControls(TestDetailsTabs.TabPages("OptionsTab").Controls, False, pSupervisor:=True)

                                'TR 24/04/2012
                                BlankTypesCombo.Enabled = False
                                BlankReplicatesUpDown.Enabled = False
                                CalReplicatesUpDown.Enabled = False
                                'TR 24/04/2012 -END.

                            End If
                        End If
                        'TR 29/03/2012 -End.

                    End If 'If Not EditionMode Then

                    Me.bsTestRefRanges.isEditing = True  'AG 13/01/2011

                    'TR 15/03/2010 -Do extra validation for Monoreagent.
                    If ReadingModeCombo.SelectedValue.ToString = "MONO" Then

                        ReferenceFilterCombo.SelectedIndex = -1
                        ReferenceFilterCombo.Enabled = False
                    Else
                        'AG 19/03/2010
                        'ReferenceFilterCombo.SelectedIndex = -1
                        If ReferenceFilterCombo.SelectedValue Is Nothing Then ReferenceFilterCombo.SelectedIndex = -1
                        ReferenceFilterCombo.Enabled = True
                    End If
                    'TR 05/025/2011 -Remove relation between prozone and turbidimetry
                    ProzonePercentUpDown.Enabled = True
                    ProzoneT1UpDown.Enabled = True
                    ProzoneT2UpDown.Enabled = True

                    'TR 05/05/2011 -Commented 
                    'If TurbidimetryCheckBox.Checked Then
                    '    ProzonePercentUpDown.Enabled = True
                    '    ProzoneT1UpDown.Enabled = True
                    '    ProzoneT2UpDown.Enabled = True
                    'Else
                    '    ProzonePercentUpDown.Enabled = False
                    '    ProzoneT1UpDown.Enabled = False
                    '    ProzoneT2UpDown.Enabled = False
                    'End If
                    'TR 05/05/2011 -END Commented.

                    'TR 29/06/2010 -Enable and disable controls depending selection.
                    EnableDisableCalibratorControls()
                    EnableDisablePredilutionControls()  'AG 12/07/2010

                End If 'If isEditTestAllowed Then
            End If 'If Not isCurrentTestInUse Then

            'TR 19/11/2010 -Validate if there are more than one test sample.
            ShowPlusSampleTypeIcon()
            'TR 19/11/2010 -END

            'TR 07/04/2011 -QC Enable 
            Dim myBackColor As New Color
            Dim myForeColor As New Color

            If QCActiveCheckBox.Checked Then
                myBackColor = Color.White
                myForeColor = Color.Black
            Else
                myBackColor = SystemColors.MenuBar
                myForeColor = Color.Black
            End If

            'SA 22/06/2012 - When CalculationMode is Statistics field MinNumberOfSeries is not Enabled
            If (StaticRadioButton.Checked) Then
                QCMinNumSeries.BackColor = Color.White
                QCMinNumSeries.Enabled = True
            Else
                QCMinNumSeries.BackColor = SystemColors.MenuBar
                QCMinNumSeries.Enabled = False
            End If

            For Each GridRow As DataGridViewRow In UsedControlsGridView.Rows
                For Each UsedControlCell As DataGridViewCell In GridRow.Cells
                    If UsedControlCell.ColumnIndex = 0 Or UsedControlCell.ColumnIndex = 1 Or _
                        UsedControlCell.ColumnIndex = 4 Or UsedControlCell.ColumnIndex = 5 Then
                        UsedControlCell.Style.BackColor = myBackColor
                        UsedControlCell.Style.ForeColor = myForeColor
                        UsedControlCell.Style.SelectionBackColor = myBackColor
                    End If
                Next
            Next
            'TR 07/04/2011 -QC Enable END.

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Enable and disable the calibrators controls depending on the selected option
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 29/06/2010
    ''' </remarks>
    Private Sub EnableDisableCalibratorControls()
        Try
            'TR 29/06/2010 -Enable or disable Add calibrator Button
            If MultipleCalibRadioButton.Checked Then
                BsErrorProvider1.Clear()

                AddCalibratorButton.Enabled = True
                AlternativeCalComboBox.Enabled = False
                CalibrationFactorTextBox.Enabled = False
                CalibrationFactorTextBox.BackColor = Color.Gainsboro

            ElseIf FactorRadioButton.Checked Then
                BsErrorProvider1.Clear()
                AlternativeCalComboBox.Enabled = False
                CalibrationFactorTextBox.Enabled = True
                CalibrationFactorTextBox.BackColor = Color.White
                AddCalibratorButton.Enabled = False

            ElseIf AlternativeCalibRadioButton.Checked Then
                AlternativeCalComboBox.Enabled = True
                AddCalibratorButton.Enabled = False
                CalibrationFactorTextBox.Enabled = False
                CalibrationFactorTextBox.BackColor = Color.Gainsboro
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableDisableCalibratorControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by AG 12/07/2010</remarks>
    Private Sub EnableDisablePredilutionControls()
        Try
            If PredilutionFactorCheckBox.Checked Then
                PredilutionModeCombo.Enabled = True
                PredilutionFactorTextBox.Enabled = True
                PredilutionFactorTextBox.BackColor = Color.White

                'TR 17/01/2011 -Add the DiluentComboBox control
                DiluentComboBox.Enabled = True
                'TR 17/01/2011 -END

            Else
                PredilutionModeCombo.SelectedIndex = -1
                PredilutionModeCombo.Enabled = False
                PredilutionFactorTextBox.Clear()
                PredilutionFactorTextBox.Enabled = False
                PredilutionFactorTextBox.BackColor = Color.Gainsboro

                'TR 17/01/2011 -Add the DiluentComboBox control
                DiluentComboBox.Enabled = False
                'TR 17/01/2011 -END
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EnableDisablePredilutionControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Validate Genetal Tab
    ''' </summary>
    ''' <remarks>
    ''' Modified by: TR 19/03/2010
    ''' Modified by AG 03/12/2010 - add optional parameter pSaveButtonClicked
    ''' </remarks>
    Private Sub ValidateGeneralTabs(Optional ByVal pSaveButtonClicked As Boolean = False)
        Try
            ValidationError = False
            BsErrorProvider1.Clear() 'RH 22/06/2012 Don't remove previous error notifications

            If TestNameTextBox.Text = "" Then
                BsErrorProvider1.SetError(TestNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

            'AG 07/07/2010
            If TestNameExits(TestNameTextBox.Text, SelectedTestDS.tparTests(0).TestID) Then
                BsErrorProvider1.SetError(TestNameTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_NAME.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If
            'END AG 07/07/2010

            If ShortNameTextBox.Text = "" Then
                BsErrorProvider1.SetError(ShortNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

            'TR 22/07/2011
            If ShortNameExist(ShortNameTextBox.Text) Then
                BsErrorProvider1.SetError(ShortNameTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_SHORTNAME.ToString))
                ValidationError = True
            End If
            'TR 22/07/2011 -END.

            If UnitsCombo.SelectedIndex <= 0 Then
                BsErrorProvider1.SetError(UnitsCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

            'DL 11/01/2012. Begin
            'If SampleTypeCboEx.SelectedValue Is Nothing Then
            'BsErrorProvider1.SetError(SampleTypeCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'SG 23/07/2010("REQUIRED_VALUE"))
            If SampleTypeCboEx.Text = "" Then
                BsErrorProvider1.SetError(SampleTypeCboEx, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'SG 23/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If
            'DL 11/01/2012. End

            'if not error then clear.
            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            Else
                'AG 03/12/2010 - Go to wrong TAB only when saving over the database
                'TestDetailsTabs.SelectTab("GeneralTab")
                'If pSaveButtonClicked Then TestDetailsTabs.SelectTab("GeneralTab")

                'RH 22/06/2012 Don't rely on changeable text values the compiler is unaware of
                If pSaveButtonClicked Then TestDetailsTabs.SelectedTab = GeneralTab
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateGeneralTabs", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Validate the procedure tab.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: TR 19/03/2010
    ''' Modified by AG 03/12/2010 - add optional parameter pSaveButtonClicked
    ''' </remarks>
    Private Sub ValidateProcedureTabs(Optional ByVal pSaveButtonClicked As Boolean = False)
        Try
            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
                If SampleVolUpDown.Text = "" Then
                    BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If VolR1UpDown.Text = "" Then
                    BsErrorProvider1.SetError(VolR1UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If WashSolVolUpDown.Text = "" Then
                    BsErrorProvider1.SetError(WashSolVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'TR 15/11/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If FirstReadingCycleUpDown.Text = "" Then
                    BsErrorProvider1.SetError(FirstReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If PredilutionFactorCheckBox.Checked Then
                    If PredilutionModeCombo.SelectedValue Is Nothing Then
                        BsErrorProvider1.SetError(PredilutionModeCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                        ValidationError = True
                    End If
                    If PredilutionFactorTextBox.Text = "" Then
                        BsErrorProvider1.SetError(PredilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                        ValidationError = True
                    End If
                End If

                If RedPostDilutionFactorTextBox.Text.TrimEnd() = "" Then
                    BsErrorProvider1.SetError(RedPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                Else
                    'TR 17/05/2012 -Validate the control Limits
                    If ValidateControlLimits(CType(RedPostDilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.POSTDILUTION_FACTOR, RedPostDilutionFactorTextBox) Then
                        'TR 17/05/2012 -Get the range value to show on error message.
                        BsErrorProvider1.SetError(RedPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                                                                                    GetStringLimitsValues(FieldLimitsEnum.POSTDILUTION_FACTOR))
                        ValidationError = True
                    End If
                    'TR 17/05/2012 -END.
                End If

                If IncPostDilutionFactorTextBox.Text.TrimEnd() = "" Then
                    BsErrorProvider1.SetError(IncPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                Else
                    'TR 17/05/2012 -Validate the control Limits
                    If ValidateControlLimits(CType(IncPostDilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.POSTDILUTION_FACTOR, IncPostDilutionFactorTextBox) Then
                        'TR 17/05/2012 -Get the range value to show on error message.
                        BsErrorProvider1.SetError(IncPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                                                                                    GetStringLimitsValues(FieldLimitsEnum.POSTDILUTION_FACTOR))
                        ValidationError = True
                    End If
                    'TR 17/05/2012 -END.
                End If

                'AG 10/02/2011 - Validate times for bichromatic (Fixed time or bireagent differential)
                '(if cycle2 is even -> cycle1 has to be even)
                '(if cycle2 is odd -> cycle1 has to be odd)
                If ReadingModeCombo.Enabled AndAlso Not ReadingModeCombo.SelectedValue Is Nothing AndAlso _
                                ReadingModeCombo.SelectedValue.ToString <> "MONO" Then

                    Dim testMode As String = AnalysisModeCombo.SelectedValue.ToString().TrimEnd()
                    If testMode <> "MREP" And testMode <> "BREP" Then
                        If SecondReadingCycleUpDown.Value Mod 2 = 0 Then
                            'T2 is Odd
                            If FirstReadingCycleUpDown.Value Mod 2 <> 0 Then
                                ValidationError = True
                                BsErrorProvider1.SetError(FirstReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.SAME_PARITY.ToString))
                            End If
                        Else
                            'T2 is Even
                            If FirstReadingCycleUpDown.Value Mod 2 = 0 Then
                                ValidationError = True
                                BsErrorProvider1.SetError(FirstReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.SAME_PARITY.ToString))
                            End If
                        End If
                    End If
                End If
                'AG 10/02/2011

                Select Case AnalysisModeCombo.SelectedValue.ToString()
                    Case "BRDIF", "BRFT", "BRK"
                        If VolR2UpDown.Enabled AndAlso VolR2UpDown.Text = "" Then
                            BsErrorProvider1.SetError(VolR2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                            ValidationError = True
                        End If
                        If SecondReadingCycleUpDown.Text = "" Then
                            BsErrorProvider1.SetError(SecondReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                            ValidationError = True
                        End If
                    Case "MREP", "BREP"
                        If Not ReadingModeCombo.SelectedValue Is Nothing AndAlso _
                                                ReadingModeCombo.SelectedValue.ToString <> "MONO" Then
                            If ReferenceFilterCombo.SelectedValue Is Nothing Then
                                BsErrorProvider1.SetError(ReferenceFilterCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                                ValidationError = True
                            ElseIf Not ReferenceFilterCombo.SelectedValue Is Nothing Then
                                If Not ValidationError Then ValidateWavelengths()
                            End If
                        End If
                        If VolR2UpDown.Enabled AndAlso VolR2UpDown.Text = "" Then
                            BsErrorProvider1.SetError(VolR2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                            ValidationError = True
                        End If

                    Case "BRDIF", "MRFT", "BRFT", "MRK", "BRK"
                        If SecondReadingCycleUpDown.Text = "" Then
                            BsErrorProvider1.SetError(SecondReadingCycleUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                            ValidationError = True
                        End If
                End Select

                If ReadingModeCombo.Enabled AndAlso Not ReadingModeCombo.SelectedValue Is Nothing AndAlso _
                                                ReadingModeCombo.SelectedValue.ToString <> "MONO" Then
                    If ReferenceFilterCombo.SelectedValue Is Nothing Then
                        BsErrorProvider1.SetError(ReferenceFilterCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                        ValidationError = True
                    ElseIf Not ReferenceFilterCombo.SelectedValue Is Nothing Then
                        If Not ValidationError Then ValidateWavelengths()
                    End If
                End If

                If PredilutionFactorTextBox.Text <> "" Then
                    If ValidateControlLimits(CType(PredilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.PREDILUTION_FACTOR, PredilutionFactorTextBox) Then
                        'TR 17/05/2012 -show the min and max value on error message.
                        BsErrorProvider1.SetError(PredilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                                  GetStringLimitsValues(FieldLimitsEnum.PREDILUTION_FACTOR)) 'AG 07/07/2010("VALUE_OUT_RANGE"))
                        ValidationError = True
                        PredilutionFactorTextBox.Select()
                    End If
                End If

                ''TR 19/07/2012 validate the readingsParameters and there are no errors.
                If Not ValidationError Then
                    ValidateReadingsParameters()
                End If

                If Not ValidationError Then
                    If ValidatePreparationVolumeLimits() Then
                        'TR 25/03/2010 -validate the volumes.
                        Dim myGlobalDataTO As New GlobalDataTO
                        myGlobalDataTO = ValidatePostDilution()

                    Else
                        ShowPreparationVolumeError() 'TR 27/03/2012
                        VolR1UpDown.Select()
                        ValidationError = True
                    End If

                End If

                'if not error then clear.
                If Not ValidationError Then
                    'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
                Else
                    'AG 03/12/2010 - Go to wrong TAB only when saving over the database
                    'TestDetailsTabs.SelectTab("ProcedureTab")
                    'If pSaveButtonClicked Then TestDetailsTabs.SelectTab("ProcedureTab")

                    'RH 22/06/2012 Don't rely on changeable text values the compiler is unaware of
                    If pSaveButtonClicked Then TestDetailsTabs.SelectedTab = ProcedureTab
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ValidateProcedureTabs", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Show the preparation volume limits loading the limits from limits table.
    ''' </summary>
    ''' <remarks>Created by: TR 27/03/2012</remarks>
    Private Sub ShowPreparationVolumeError()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS()
            'get the Preparation volume Limits to create warning message
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.PREPARATION_VOLUME, MyBase.AnalyzerModel)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                Dim MinMaxValues As String = "[" & myFieldLimitsDS.tfmwFieldLimits(0).MinValue.ToString() & "-" & myFieldLimitsDS.tfmwFieldLimits(0).MaxValue.ToString() & "]"
                'BsErrorProvider1.SetError(VolR1UpDown, GetMessageText(GlobalEnumerates.Messages.PREPARATION_VOLUME.ToString) & " " & MinMaxValues)
                'TR 17/05/2012 -Set the error icon to the group ox instead setting the error to the VolR1UpDown.
                BsErrorProvider1.SetError(VolumesGroupBox, GetMessageText(GlobalEnumerates.Messages.PREPARATION_VOLUME.ToString) & " " & MinMaxValues)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ShowPreparationVolumeError", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    ''' <summary>
    ''' Validate the calibration tab controls.
    ''' </summary>
    ''' <remarks>
    ''' TR 10/03/2010
    ''' Modified by: TR 19/03/2010
    ''' Modified by AG 03/12/2010 - add optional parameter pSaveButtonClicked
    ''' </remarks>
    Private Sub ValidateCalibrationTabs(Optional ByVal pSaveButtonClicked As Boolean = False)
        Try
            'BsErrorProvider1.Clear()
            If FactorRadioButton.Checked Then
                If CalibrationFactorTextBox.Text = "" Then
                    BsErrorProvider1.SetError(CalibrationFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
            End If
            If BlankTypesCombo.SelectedValue Is Nothing Then
                BsErrorProvider1.SetError(BlankTypesCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

            If Not ValidationError Then
                'TR 15/06/2010
                ValidateExperimentalCalib()
            End If


            If AlternativeCalibRadioButton.Checked Then
                If AlternativeCalComboBox.SelectedValue Is Nothing Then
                    BsErrorProvider1.SetError(AlternativeCalComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
            End If

            'if not error then clear.
            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            Else
                'AG 03/12/2010 - Go to wrong TAB only when saving over the database
                'TestDetailsTabs.SelectTab("CalibrationTab")
                'If pSaveButtonClicked Then TestDetailsTabs.SelectTab("CalibrationTab")

                'RH 22/06/2012 Don't rely on changeable text values the compiler is unaware of
                If pSaveButtonClicked Then TestDetailsTabs.SelectedTab = CalibrationTab
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateCalibrationTabs ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Special method use to validate if the calibrator data is complete on DB for a Test-Sample.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 01/03/2011
    ''' </remarks>
    Private Function ValidateCalibratorDataCompleted(ByVal pTestID As Integer, ByVal pSampleType As String) As Boolean
        Dim myResult As Boolean = True
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestCalibratorDS As New TestCalibratorsDS
            Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
            'Get the testCalibrator data from db.
            myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(Nothing, pTestID, pSampleType)

            If Not myGlobalDataTO.HasError Then
                myTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                If myTestCalibratorDS.tparTestCalibrators.Count > 0 Then
                    'Get the testCalibrator Values
                    Dim myTestCalibratorValuesDelegate As New TestCalibratorValuesDelegate
                    Dim myTestCalibaratorValuesDS As New TestCalibratorValuesDS
                    'Get the concentration values.
                    myGlobalDataTO = myTestCalibratorValuesDelegate.GetTestCalibratorValuesByTestIDSampleType(Nothing, pTestID, pSampleType)

                    If Not myGlobalDataTO.HasError Then
                        myTestCalibaratorValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                        If myTestCalibaratorValuesDS.tparTestCalibratorValues.Count = 0 Then
                            myResult = False
                        End If
                    End If
                Else
                    'Get TestSample calibrator Type
                    Dim myTestSampleDelegate As New TestSamplesDelegate
                    Dim myTestSampleDS As New TestSamplesDS
                    myTestSampleDS = myTestSampleDelegate.GetTestSampleDataByTestIDAndSampleType(pTestID, pSampleType)

                    If myTestSampleDS.tparTestSamples.Count > 0 Then
                        If myTestSampleDS.tparTestSamples(0).CalibratorType = "EXPERIMENT" Then
                            SelectedTestSampleCalibratorDS.tparTestCalibrators.Clear()
                            UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            myResult = False
                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ValidateCalibratorDataCompleted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myResult

    End Function

    ''' <summary>
    ''' Validate the Option Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 30/03/2010
    ''' Modified by AG 03/12/2010 - add optional parameter pSaveButtonClicked
    '''             WE 27/08/2014 - #1865. Extended with control activation in case of validation error regarding SlopeFactor.
    ''' </remarks>
    Private Sub ValidateOptionsTab(Optional ByVal pSaveButtonClicked As Boolean = False)
        Try
            'ValidateProzoneTimes()
            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications

                ' Linearity and Detection Limits
                ' Detection
                If LinearityUpDown.Text <> "" And DetectionUpDown.Text <> "" Then
                    'If LinearityUpDown.Text <> "" Then 'TR 31/05/2010 then linearity is not required
                    If CType(DetectionUpDown.Text, Decimal) > CType(LinearityUpDown.Text, Decimal) Then
                        BsErrorProvider1.SetError(DetectionUpDown, GetMessageText(GlobalEnumerates.Messages.DETGREATHERLIN.ToString)) 'AG 07/07/2010("DETGREATHERLIN"))
                        ValidationError = True
                        DetectionUpDown.Select()
                    End If
                End If
                ValidateLimitsUpDown(Me.FactorLowerLimitUpDown, Me.FactorUpperLimitUpDown, False)
                ValidateLimitsUpDown(Me.RerunLowerLimitUpDown, Me.RerunUpperLimitUpDown, False)
                'END SG 11/06/2010
            End If

            If Not ValidateRefRanges() Then
                ValidationError = True
                Me.bsTestRefRanges.Select()
            End If

            'TR 02/07/2010 -Validation of slope factors
            If Not SlopeAUpDown.Text = "" AndAlso SlopeAUpDown.Value = 0 Then
                BsErrorProvider1.SetError(SlopeAUpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
                ValidationError = True
                SlopeAUpDown.Select()
            ElseIf Not SlopeAUpDown.Text = "" AndAlso SlopeBUpDown.Text = "" Then
                BsErrorProvider1.SetError(SlopeBUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
                SlopeBUpDown.Select()

                'AG 07/07/2010 - If B informed then A is required
            ElseIf Not SlopeBUpDown.Text = "" AndAlso SlopeAUpDown.Text = "" Then
                BsErrorProvider1.SetError(SlopeAUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
                SlopeAUpDown.Select()
            End If
            'TR 02/07/2010 -Validation of slope factors

            'Prozone
            'AG 15/06/2012- Remove the following IF (the turbi check never is visible!!! 'AG 06/04/2010
            'If TurbidimetryCheckBox.Checked Then 'TR 06/04/2010 -add the turbi validation 

            'TR 08/04/2010 -Validate if there are any data on the prozone controls to start validating.
            If ProzonePercentUpDown.Text <> "" Or ProzoneT1UpDown.Text <> "" Or ProzoneT2UpDown.Text <> "" Then
                'TR 07/04/2010
                If ProzoneT1UpDown.Text <> "" AndAlso ProzoneT2UpDown.Text <> "" Then
                    If ProzoneT1UpDown.Value >= ProzoneT2UpDown.Value Then
                        If ProzoneT1UpDown.Value <= ProzoneT1UpDown.Minimum Then
                            'If T1 is minimum allowed then send focus and error in T2
                            BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.PROZONE_TIMES.ToString)) 'AG 07/07/2010("PROZONE_TIMES"))
                            'ProzoneT2UpDown.Select()
                            ValidationError = True
                        Else
                            'If T1 isnt the minimum allowed then send focus and error to T1
                            BsErrorProvider1.SetError(ProzoneT1UpDown, GetMessageText(GlobalEnumerates.Messages.PROZONE_TIMES.ToString)) 'AG 07/07/2010("PROZONE_TIMES"))
                            'ProzoneT1UpDown.Select()
                            ValidationError = True
                        End If
                        'END AG 06/04/2010
                    Else
                        If SecondReadingSecUpDown.Text <> "" And ProzoneT2UpDown.Text <> "" Then
                            'AG 18/06/2012 - comment unused message, this condition never triggers. Besides it was too long > 64 characters
                            'If SecondReadingSecUpDown.Value < ProzoneT2UpDown.Value Then
                            '    BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.READING2GREATERT2.ToString)) 'AG 07/07/2010("READING2GREATERT2"))
                            '    ValidationError = True
                            'End If
                        End If
                    End If
                End If
                'if one of the value is empty send error message
                If ProzonePercentUpDown.Text = "" Then
                    BsErrorProvider1.SetError(ProzonePercentUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
                If ProzoneT1UpDown.Text = "" Then
                    BsErrorProvider1.SetError(ProzoneT1UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
                If ProzoneT2UpDown.Text = "" Then
                    BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
            End If
            'End If

            'if not error then clear.
            If Not ValidationError Then
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            Else
                'AG 03/12/2010 - Go to wrong TAB only when saving over the database
                'TestDetailsTabs.SelectTab("OptionsTab")
                'If pSaveButtonClicked Then TestDetailsTabs.SelectTab("OptionsTab")

                'RH 22/06/2012 Don't rely on changeable text values the compiler is unaware of
                If pSaveButtonClicked Then TestDetailsTabs.SelectedTab = OptionsTab
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateOptionsTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method that validate all the required value on all the tabs
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 11/03/2010
    ''' Modified by: TR 19/03/2010
    ''' Modified by AG 03/12/2010 - add optional parameter pSaveButtonClicked
    ''' </remarks>
    Private Sub ValidateAllTabs(Optional ByVal pSaveButtonClicked As Boolean = False)

        Try
            'AG 19/03/2010 - Only validate tabs in edition mode
            If EditionMode Then
                'TR 29/03/2010 Validate there is not error.
                ValidateGeneralTabs(pSaveButtonClicked)
                If Not ValidationError Then
                    ValidateProcedureTabs(pSaveButtonClicked)
                    If Not ValidationError Then
                        ValidateCalibrationTabs(pSaveButtonClicked)
                        If Not ValidationError Then
                            ValidateOptionsTab(pSaveButtonClicked)
                            If Not ValidationError Then
                                ValidateErrorOnQCTab(pSaveButtonClicked)
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateAllTabs ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' validates ref ranges
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 06/09/2010
    ''' Modified AG 13/01/2011 - (copied and adapted from ProgOffSystemTest.ValidateRefRanges)</remarks>
    Private Function ValidateRefRanges() As Boolean
        Try
            If (bsTestRefRanges.ActiveRangeType <> String.Empty) Then
                bsTestRefRanges.ValidateRefRangesLimits(False, True)
                If (bsTestRefRanges.ValidationError) Then Return False
            End If
            Return True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Add the Delete Sample type or Test in to the list of DeletedTestProgramingList.
    ''' And remove the item from the local stuctures.
    ''' </summary>
    ''' <param name="pTestID">Test ID </param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <remarks>Modified by SG 23/07/2010</remarks>
    Private Sub DeleteTestSampleType(ByVal pTestID As Integer, ByVal pSampleType As String, Optional ByVal pTestName As String = "")
        Try
            DeletedSampleType = pSampleType 'SG 22/07/2010
            Dim myNewTest As Boolean = False
            'TR 29/11/2010 -Change updatedTest for selectedTest
            'search on the local update test to see if not a new created test.
            Dim qTest As New List(Of TestsDS.tparTestsRow)
            qTest = (From a In SelectedTestDS.tparTests _
                     Where a.TestID = pTestID _
                     Select a).ToList()
            'if found then remove from table.
            If qTest.Count > 0 Then
                If qTest.First().NewTest Then
                    'qTest.First().Delete()
                    'qTest.First().AcceptChanges()
                    myNewTest = True
                End If
            End If
            'validate that is not a new test.
            If Not myNewTest Then
                Dim myDeletedTestProgramingTO As New DeletedTestProgramingTO()
                myDeletedTestProgramingTO.TestID = pTestID
                myDeletedTestProgramingTO.SampleType = pSampleType
                myDeletedTestProgramingTO.TestName = pTestName 'TR 24/11/2010 -Set the test name
                'add the deleted sample type.
                DeletedTestProgramingList.Add(myDeletedTestProgramingTO)
                ReadyToSave = True 'TR 29/03/2010 set ready to save = true 

            End If 'AG 28/09/2010

            'AG 28/09/2010 - Use this same code when new or not new test. It was only executed in case not new test
            'and when new test no additional code was perfoming causing fails

            'Remove sample type localy from Test
            Dim qDeletedSampleType As New List(Of TestSamplesDS.tparTestSamplesRow)

            If pSampleType <> "" Then
                'Filter sample type to delete
                qDeletedSampleType = (From a In SelectedTestSamplesDS.tparTestSamples _
                                 Where a.TestID = pTestID AndAlso a.SampleType = pSampleType _
                                 Select a).ToList()
                'Else
                'remove Test
                'TestListView.SelectedItems.Item(0).Remove() 'TR 24/11/2010 -Commented
                'select the first test on the list
                'TestListView.Items(0).Selected = True
            End If

            If qDeletedSampleType.Count > 0 Then
                'TR 24/11/2010
                If ValidateDependenciesOnDeletedElemets() = Windows.Forms.DialogResult.OK Then

                    SelectedSampleTypeCombo.SelectedIndex = 0 'change the selection to the main Sample Type.
                    qDeletedSampleType.First().Delete()
                    qDeletedSampleType.First().AcceptChanges()
                    SelectedTestSamplesDS.tparTestSamples.AcceptChanges()

                    'If no DefaultSampleType mark the first
                    Dim qAcceptedSampleType As New List(Of TestSamplesDS.tparTestSamplesRow)
                    'AG 10/02/2011
                    'qAcceptedSampleType = (From a In SelectedTestSamplesDS.tparTestSamples _
                    '                    Where a.TestID = pTestID And a.SampleType = pSampleType And a.DefaultSampleType = True _
                    '                    Select a).ToList()
                    qAcceptedSampleType = (From a In SelectedTestSamplesDS.tparTestSamples _
                    Where a.TestID = pTestID And a.DefaultSampleType = True _
                    Select a).ToList()
                    'AG 10/02/2011
                    'Change the default sample type and change the alternative calibrator in case 
                    'there are any test sample using as alternative calibrator previous deleted default sample type
                    If qAcceptedSampleType.Count = 0 Then
                        If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                            SelectedTestSamplesDS.tparTestSamples(0).BeginEdit()
                            SelectedTestSamplesDS.tparTestSamples(0).DefaultSampleType = True
                            SelectedTestSamplesDS.tparTestSamples(0).EndEdit()
                            SelectedTestSamplesDS.AcceptChanges()
                            'TODO: Go throught each record on selected test sample an set the new default sample type if use
                            'alternative calibrator.

                        End If
                    End If
                    CurrentSampleType = SelectedSampleTypeCombo.SelectedValue.ToString() 'AG 11/02/2011
                    BindControls(pTestID, SelectedSampleTypeCombo.SelectedValue.ToString(), False)

                    SelectedSampleTypeCombo.Items.Remove(pSampleType)

                    ' DL 11/01/2012. Begin
                    'SampleTypeCombo.Items.Remove(pSampleType) 
                    For i As Integer = 0 To SampleTypeCheckList.CheckedItems.Count - 1
                        If SampleTypeCheckList.CheckedItems(i).ToString = pSampleType Then
                            SampleTypeCheckList.SetItemChecked(i, False)
                        End If
                    Next i
                    ' DL 11/01/2012. End

                    SaveTestLocal(NewTest)
                    ChangesMade = True
                Else
                    DeletedTestProgramingList.Clear()
                    ChangesMade = False
                End If
                'TR 24/11/2010 -END

            Else
                ChangesMade = False 'In these case enters when create a new test and before savelocal we delete it
            End If
            'End If 'AG 28/09/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DeleteTestSampleType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Calculate the first readign second from the value of 
    ''' the first reading cycle.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateFirstReadingSecReading()
        Try
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
            'TR 14/03/2011 -Add the analyzer model on the query.
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                            Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                            AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                            Select a).ToList()
            If qswParameter.Count > 0 Then
                FirstReadingSecUpDown.ResetText()
                FirstReadingSecUpDown.Text = CType((FirstReadingCycleUpDown.Value - 1) * qswParameter.First().ValueNumeric, Integer).ToString()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CalculateFirstReadingSecReading", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Calculate the second readign second from the value of 
    ''' the second reading cycle.
    ''' </summary>
    ''' <remarks>Created by: TR 19/03/2010</remarks>
    Private Sub CalculateSecondReadingSec()
        Try
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
            'TR 14/03/2011 -Add the analyzer model on the query.
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                            Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                            AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                            Select a).ToList()
            If qswParameter.Count > 0 Then
                SecondReadingSecUpDown.ResetText()

                SecondReadingSecUpDown.Text = CType(((SecondReadingCycleUpDown.Value - 1) * qswParameter.First().ValueNumeric), Integer).ToString()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CalculateSecondReadingSec", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Search the Test locally or in the database depending if the 
    ''' test is allready modified or not.
    ''' </summary>
    ''' <param name="pTestID">Test ID</param>
    ''' <param name="pSampleType">Sample Type</param>
    ''' <remarks>
    ''' Created by: TR 17/03/2010
    ''' Modified by: TR 19/03/2010
    ''' Modified by: SG 17/06/2010 test ref ranges
    ''' </remarks>
    Private Sub GetTestInfo(ByVal pTestID As Integer, ByRef pSampleType As String)

        Dim qTest As New List(Of TestsDS.tparTestsRow)
        'Dim dbConnection As New SqlClient.SqlConnection 'AG 22/03/2010

        Try
            SelectedTestDS.tparTests.Clear()
            SelectedReagentsDS.tparReagents.Clear()
            SelectedTestReagentsDS.tparTestReagents.Clear()
            SelectedTestSamplesDS.tparTestSamples.Clear()
            SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Clear()
            SelectedTestRefRangesDS.tparTestRefRanges.Clear() 'SG 17/06/2010
            UncheckAllSampleTypes()
            SelectedTestDS.tparTests.AcceptChanges()
            SelectedReagentsDS.tparReagents.AcceptChanges()
            SelectedTestReagentsDS.tparTestReagents.AcceptChanges()
            SelectedTestSamplesDS.tparTestSamples.AcceptChanges()
            SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AcceptChanges()
            SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges() 'SG 17/06/2010

            'validate if there are any record on the local update Dataset
            'to search the test locally
            If UpdateTestDS.tparTests.Rows.Count > 0 Then
                qTest = (From a In UpdateTestDS.tparTests _
                    Where a.TestID = pTestID).ToList()
            End If
            'if record not found on locally then look for the test on the 
            'database.
            If qTest.Count = 0 Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myTestDelegate As New TestsDelegate()

                Dim myReagentsDelegate As New ReagentsDelegate()
                Dim myTestSamplesDelegate As New TestSamplesDelegate()
                Dim myTestReagentsDelegate As New TestReagentsDelegate()
                Dim myTestReagentsVolumeDelegate As New TestReagentsVolumeDelegate()
                'Dim myTestRefRangesDelegate As New TestRefRangesDelegate() 'SG 17/06/2010

                'get the test information.
                'myGlobalDataTO = myTestDelegate.Read(Nothing, pTestID) 'AG 22/03/2010                
                myGlobalDataTO = myTestDelegate.Read(Nothing, pTestID)
                'END AG 22/03/2010

                If Not myGlobalDataTO.HasError Then
                    For Each TestRow As TestsDS.tparTestsRow In CType(myGlobalDataTO.SetDatos, TestsDS).tparTests.Rows
                        'AG 18/03/2010 - Check if the test has blank results
                        TestRow.ExistBlankResult = ExistsTestResults(pTestID, TestRow.TestVersionNumber) 'AG 22/03/2010 
                        TestRow.DeleteResultAnswer = False 'AG 25/03/2010
                        SelectedTestDS.tparTests.ImportRow(TestRow)
                        qTest.Add(TestRow)
                    Next TestRow
                End If
                If Not myGlobalDataTO.HasError Then
                    'get the Sample data
                    myGlobalDataTO = myTestSamplesDelegate.GetSampleDataByTestID(Nothing, pTestID) 'AG 22/03/2010
                    If Not myGlobalDataTO.HasError Then
                        'import all Test Sample Row 
                        For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In _
                                                        CType(myGlobalDataTO.SetDatos, TestSamplesDS).tparTestSamples.Rows
                            'AG 18/03/2010 - Check if the test has calib results
                            TestSampleRow.ExistCalibResult = ExistsTestResults(pTestID, _
                                                             SelectedTestDS.tparTests(0).TestVersionNumber, TestSampleRow.SampleType) 'AG 22/03/2010
                            TestSampleRow.DeleteResultAnswer = False 'AG 25/03/2010
                            SelectedTestSamplesDS.tparTestSamples.ImportRow(TestSampleRow)
                        Next TestSampleRow
                        'validate if sample type is empty to assigned the default sample type.
                        If pSampleType = "" Then
                            SelectedTestSamplesDS.tparTestSamples.DefaultView.RowFilter = "DefaultSampleType = True"

                            'AG 10/02/2011 - System error when user deletes the default sampletype
                            'pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                            If SelectedTestSamplesDS.tparTestSamples.DefaultView.Count > 0 Then
                                'DL 14/06/2011. Begin
                                'Keep the last sample type edited
                                If Not CurrentSampleType Is Nothing AndAlso CurrentSampleType <> "" Then
                                    If isEditTestAllowed Then
                                        pSampleType = CurrentSampleType
                                    Else
                                        pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                                    End If
                                Else
                                    pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                                End If
                                'DL 14/06/2011. End
                                pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                            Else
                                pSampleType = SelectedTestSamplesDS.tparTestSamples(0).SampleType.ToString
                            End If
                            'AG 10/02/2011
                            CurrentSampleType = pSampleType 'AG 12/01/2011
                            SelectedTestSamplesDS.tparTestSamples.DefaultView.RowFilter = ""
                        End If

                        'Get Test Reagents
                        myGlobalDataTO = myTestReagentsDelegate.GetTestReagents(Nothing, pTestID)  'AG 22/03/2010 (use dbconnection instead nothing)
                        If Not myGlobalDataTO.HasError Then
                            For Each TestReagentRow As TestReagentsDS.tparTestReagentsRow In _
                                                        CType(myGlobalDataTO.SetDatos, TestReagentsDS).tparTestReagents.Rows
                                SelectedTestReagentsDS.tparTestReagents.ImportRow(TestReagentRow)
                            Next TestReagentRow

                            'Go through each row in selectedTestReagents to load the reagent data
                            Dim tempReagent As New ReagentsDS
                            SelectedReagentsDS.tparReagents.Clear()
                            For Each testReagentRow As TestReagentsDS.tparTestReagentsRow In SelectedTestReagentsDS.tparTestReagents.Rows
                                'Get the Reagents by the TestId.
                                myGlobalDataTO = myReagentsDelegate.GetReagentsData(Nothing, testReagentRow.ReagentID) 'AG 22/03/2010 (use dbconnection instead nothing)
                                If Not myGlobalDataTO.HasError Then
                                    tempReagent = CType(myGlobalDataTO.SetDatos, ReagentsDS)
                                    If tempReagent.tparReagents.Rows.Count > 0 Then
                                        SelectedReagentsDS.tparReagents.ImportRow(tempReagent.tparReagents(0))
                                    End If
                                End If
                            Next testReagentRow
                        End If
                        'Get the ReagentsVolume an fill the Dataset
                        myGlobalDataTO = myTestReagentsVolumeDelegate.GetReagentsVolumesByTestID(Nothing, pTestID) 'AG 22/03/2010 (use dbconnection instead nothing)
                        If Not myGlobalDataTO.HasError Then
                            For Each TestReagentVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                        CType(myGlobalDataTO.SetDatos, TestReagentsVolumesDS).tparTestReagentsVolumes.Rows
                                SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.ImportRow(TestReagentVolRow)
                            Next TestReagentVolRow
                        End If

                        GetRefRanges(pTestID, pSampleType)

                        'END SG 17/06/2010
                    End If
                End If
            Else
                'Insert the Test in the test id table.
                SelectedTestDS.tparTests.ImportRow(qTest.First())
                Dim qTestSampleList As New List(Of TestSamplesDS.tparTestSamplesRow)
                Dim qTestReagentList As New List(Of TestReagentsDS.tparTestReagentsRow)
                Dim qReagentList As New List(Of ReagentsDS.tparReagentsRow)
                Dim qTestReagentVol As New List(Of TestReagentsVolumesDS.tparTestReagentsVolumesRow)
                Dim qTestRefRangeList As New List(Of TestRefRangesDS.tparTestRefRangesRow) 'SG 17/06/2010

                'Insert then Test Sample.
                If UpdateTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                    qTestSampleList = (From a In UpdateTestSamplesDS.tparTestSamples _
                                           Where a.TestID = pTestID Select a).ToList()

                    If qTestSampleList.Count > 0 Then
                        'Delete Test Samples.
                        For Each testSampleRow As TestSamplesDS.tparTestSamplesRow In qTestSampleList
                            SelectedTestSamplesDS.tparTestSamples.ImportRow(testSampleRow)
                        Next
                    End If
                End If

                'TEST REAGENT
                qTestReagentList = (From a In UpdateTestReagentsDS.tparTestReagents _
                                    Where a.TestID = pTestID _
                                    Select a).ToList()

                'remove all related test reagent anad reagents
                For Each TestReagentRow As TestReagentsDS.tparTestReagentsRow In qTestReagentList
                    'search reagent to Insert
                    qReagentList = (From a In UpdateReagentsDS.tparReagents _
                                    Where a.ReagentID = TestReagentRow.ReagentID _
                                    Select a).ToList()

                    If qReagentList.Count > 0 Then
                        'Insert Reagent.
                        SelectedReagentsDS.tparReagents.ImportRow(qReagentList.First())
                    End If
                    SelectedTestReagentsDS.tparTestReagents.ImportRow(TestReagentRow)
                Next

                'TEST REAGENT VOLUME
                qTestReagentVol = (From a In UpdateTestReagentsVolumesDS.tparTestReagentsVolumes _
                                   Where a.TestID = pTestID Select a).ToList

                'REMOVE TEST REAGENT VOLUME
                For Each TestReagVolRow As TestReagentsVolumesDS.tparTestReagentsVolumesRow In _
                                                                                    qTestReagentVol
                    SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.ImportRow(TestReagVolRow)
                Next

                'Test Reference Ranges
                'SG 17/06/2010
                If UpdateTestRefRangesDS.tparTestRefRanges.Rows.Count > 0 Then
                    qTestRefRangeList = (From a In UpdateTestRefRangesDS.tparTestRefRanges _
                                           Where a.TestID = pTestID Select a).ToList()

                    If qTestRefRangeList.Count > 0 Then
                        'Delete Test Reference Ranges.
                        For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In qTestRefRangeList
                            SelectedTestRefRangesDS.tparTestRefRanges.ImportRow(TestRefRangesRow)
                        Next
                    End If
                End If

                'END SG 17/06/2010

                If pSampleType = "" Then
                    SelectedTestSamplesDS.tparTestSamples.DefaultView.RowFilter = "DefaultSampleType = True"

                    'AG 10/02/2011 - System error when user deletes the default sampletype
                    'pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                    If SelectedTestSamplesDS.tparTestSamples.DefaultView.Count > 0 Then
                        pSampleType = SelectedTestSamplesDS.tparTestSamples.DefaultView(0).Item("SampleType").ToString()
                    Else
                        pSampleType = SelectedTestSamplesDS.tparTestSamples(0).SampleType.ToString
                    End If
                    'AG 10/02/2011


                    CurrentSampleType = pSampleType 'AG 12/01/2011
                    SelectedTestSamplesDS.tparTestSamples.DefaultView.RowFilter = ""
                End If

                SelectedTestDS.tparTests.AcceptChanges()
                SelectedReagentsDS.tparReagents.AcceptChanges()
                SelectedTestReagentsDS.tparTestReagents.AcceptChanges()
                SelectedTestSamplesDS.tparTestSamples.AcceptChanges()
                SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.AcceptChanges()
                SelectedTestRefRangesDS.tparTestRefRanges.AcceptChanges() 'SG 17/06/2010

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " .GetTestInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Gets the ref ranges defined for the specified OFF-SYSTEM test
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <remarks>
    ''' AG 12/01/2011 (Copied and adapted from ProgOffSystemTest)
    ''' </remarks>
    Private Sub GetRefRanges(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestRefRangesDelegate As New TestRefRangesDelegate

            myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(Nothing, pTestID, pSampleType, , "STD")
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
            Else
                SelectedTestRefRangesDS = New TestRefRangesDS
                ShowMessage(Me.Name & ".GetRefRanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAddSampleTypeNewTest"></param>
    ''' <param name="pCurrentSampleType"></param>
    ''' <returns></returns>
    ''' <remarks>AG 12/11/2010 - add optional parameter pAddDeleteSampleType and pCurrentSampleType</remarks>
    Private Function SaveTestLocal(Optional ByVal pAddSampleTypeNewTest As Boolean = False, Optional ByVal pCurrentSampleType As String = "") As Boolean
        'AG 10/03/10 - SaveButton_Click code move to this function
        'validate if there are any error()
        Try
            ValidateAllTabs(True) 'AG 12/01/2011 - add TRUE parameter 'TR 12/03/2010 -Integrate all the tabs validation in one method.

            If Not ValidationError Then
                BsErrorProvider1.Clear() 'RH 22/06/2012 Clear Error notifications after successful validation

                NormalVolumeSteps.sampleVol = CType(SampleVolUpDown.Value, Single)
                NormalVolumeSteps.WashVol = CType(WashSolVolUpDown.Value, Single)

                If VolR1UpDown.Text <> "" Then
                    NormalVolumeSteps.R1Vol = CType(VolR1UpDown.Value, Single)
                End If

                If VolR2UpDown.Text <> "" Then
                    NormalVolumeSteps.R2Vol = CType(VolR2UpDown.Value, Single)
                Else
                    NormalVolumeSteps.R2Vol = 0 'AG 26/03/2010
                End If

                'AG 26/03/2010 - Initialize steps values to zero (normalvolumesteps is a class variable and can contain data from other tests)
                NormalVolumeSteps.sampleSteps = 0
                NormalVolumeSteps.WashStepsVol = 0
                NormalVolumeSteps.R1Steps = 0
                NormalVolumeSteps.R2Steps = 0
                'END AG 26/03/2010

                'Read the parameter steps_uL
                Dim steps_uL As Single = 1
                Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)

                qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                Where a.ParameterName = GlobalEnumerates.SwParameters.STEPS_UL.ToString _
                                Select a).ToList()

                steps_uL = CType(qswParameter.First().ValueNumeric, Single)

                'AG 19/04/2011 - different conversion factor for reagents and samples
                Dim sample_steps_uL As Single = 1 'AG 19/04/2011
                qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                Where a.ParameterName = GlobalEnumerates.SwParameters.SAMPLE_STEPS_UL.ToString _
                Select a).ToList()

                sample_steps_uL = CType(qswParameter.First().ValueNumeric, Single)
                'AG 19/04/2011

                Dim myglobalDataTO As New GlobalDataTO
                myglobalDataTO = CalculateVolumeInSteps(steps_uL, sample_steps_uL, NormalVolumeSteps)
                If Not myglobalDataTO.HasError Then
                    NormalVolumeSteps = CType(myglobalDataTO.SetDatos, VolumeCalculations)
                End If

                'DL 11/01/2012. Begin
                'Dim mySampleType As String = SampleTypeCombo.Text
                Dim mySampleType As String = SampleTypeCboEx.Text
                'DL 11/01/2012. End

                If pCurrentSampleType <> "" Then mySampleType = pCurrentSampleType 'AG 12/11/2010

                'set status redy to save
                ReadyToSave = True
                If NewTest Then
                    CreateNewTest()
                    'NewTest = False 'AG 12/11/2010
                    EditButton.Enabled = True
                    DeleteButton.Enabled = True
                    '                    PrintTestButton.Enabled = True  dl 11/05/2012
                    TestListView.Enabled = True

                    UpdateTestDatasets(SelectedTestDS.tparTests(0).TestID, mySampleType)
                    If Not pAddSampleTypeNewTest Then 'AG 12/11/2010 - add if
                        TestListView.Items.Add(SelectedTestDS.tparTests(0).TestID.ToString(), SelectedTestDS.tparTests(0).TestName, "USERTEST").Tag = False
                        NewTest = False 'AG 12/11/2010 

                        If TestListView.SelectedItems.Count = 1 Then    'AG 07/07/2010 - Else the app crash on create the first test in list
                            ''SG 06/07/2010 Update the listview selection
                            TestListView.SelectedItems(0).Selected = False
                        Else
                            TestListView.SelectedItems.Clear()
                        End If
                        TestListView.Items(TestListView.Items.Count - 1).Selected = True
                    End If 'END AG 12/11/2010
                    'ChangesMade = False 'AG 12/11/2010

                    'UpdateTestDatasets(SelectedTestDS.tparTests(0).TestID, mySampleType)
                    'NewTest = False 'TR 10/01/2012 -Move here.
                Else
                    If mySampleType <> CurrentSampleType Then

                        Dim myRangeType As String = RefGetActiveRangeType(mySampleType) ' TR 19/11/2010) -pos solu Change for curren instead of mySample
                        'SG 06/09/2010
                        Me.selRefRangeType = myRangeType
                        Me.selRefStdTestID = SelectedTestDS.tparTests(0).TestID
                        Me.selMeasureUnit = UnitsCombo.Text.ToString
                        'Me.selMeasureUnit = GetUnitDefinition(SelectedTestDS.tparTests(0).MeasureUnit)
                        BindRefRanges(mySampleType)
                        'BindReferenceRangesControls(SelectedTestDS.tparTests(0).TestID, mySampleType, myRangeType)
                        UpdateTestDatasets(SelectedTestDS.tparTests(0).TestID, mySampleType)
                        'TR 08/11/2010 -Move here.
                        If SaveAndClose Then SaveAndClose = False
                    Else
                        UpdateTestDatasets(SelectedTestDS.tparTests(0).TestID, mySampleType)
                    End If
                End If

                'AG 28/09/2010 - validate if there are some SampleType using alternative calibrator with bad programming
                Dim qSampleTypeWrong As List(Of TestSamplesDS.tparTestSamplesRow) = _
                    (From a As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples _
                     Where a.CalibratorType = "ALTERNATIV" And a.SampleTypeAlternative IsNot _
                     (From b As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples _
                      Select b.SampleType) _
                    Select a).ToList()

                If qSampleTypeWrong.Count > 0 Then
                    If qSampleTypeWrong(0).SampleType <> SelectedSampleTypeCombo.SelectedValue.ToString() Then
                        'TR Preguntar alber el porque se hace esto
                        'BindControls(qSampleTypeWrong(0).TestID, qSampleTypeWrong(0).SampleType, False)
                        'ValidateCalibrationTabs()
                        'TR -END Preguntar
                    End If
                End If

                'If user has changed the TestName update the ListTreeView
                'TR 30/11/2010 -Validate if there are selecte test on the test list
                If TestListView.SelectedItems.Count > 0 AndAlso TestListView.SelectedItems(0).Text <> TestNameTextBox.Text Then
                    'AG 12/11/2010 - Fix a system error
                    'TestListView.SelectedItems(0).Text = TestNameTextBox.Text
                    If NewTest And pAddSampleTypeNewTest Then
                        'Nothing
                    Else
                        TestListView.SelectedItems(0).Text = TestNameTextBox.Text
                    End If
                    'END AG 12/11/2010
                End If
                'END AG 28/09/2010

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SaveTestLocal ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return Not ValidationError
    End Function

    ''' <summary>
    ''' Save Ref ranges Data
    ''' </summary>
    ''' <remarks>
    ''' Created by SG 02/09/2010
    ''' Modified AG 13/01/2011</remarks>
    Private Sub UpdateRefRangesChanges(ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pNewTestSaving As Boolean)

        Try
            If bsTestRefRanges.ChangesMade Then
                'SelectedTestRefRangesDS.Clear()
                If Not Me.DesignMode Then
                    'SelectedTestRefRangesDS.tparTestRefRanges.Clear() 'DL 15/10/2012
                    SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS) 'bsTestRefRanges.DefinedTestRangesDS
                End If

                'AG 13/01/2011 - If pNewTestSaving then update the TestID and SampleType in SelectedTestRefRangesDS
                If pNewTestSaving Then
                    For Each row As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                        row.BeginEdit()
                        If row.IsTestIDNull Then row.TestID = pTestID
                        If row.IsSampleTypeNull Then row.SampleType = pSampleType
                        row.AcceptChanges()
                    Next
                End If
                'AG 13/01/2011

                'Update the field SelectedTestSamplesDS.ActiveRangeType (for the current SampleType)
                If SelectedTestRefRangesDS.tparTestRefRanges.Rows.Count > 0 Then
                    Dim resLinq As List(Of TestSamplesDS.tparTestSamplesRow) = (From a In SelectedTestSamplesDS.tparTestSamples _
                                                                                Where a.TestID = pTestID And a.SampleType = pSampleType _
                                                                                Select a).ToList
                    If (resLinq.Count > 0) Then
                        resLinq(0).BeginEdit()
                        resLinq(0).ActiveRangeType = bsTestRefRanges.ActiveRangeType
                        resLinq(0).AcceptChanges()
                    End If
                End If
            End If
            'AG 13/01/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateRefRangesChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Deletes Ref ranges Data
    ''' </summary>
    ''' <remarks>
    ''' Created by SG 02/09/2010</remarks>
    Private Sub DeleteRefRanges(ByVal pTestID As Integer)

        Try
            Dim qTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
            qTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                               Where a.TestID = pTestID _
                               Select a).ToList()

            For Each RangeRow As TestRefRangesDS.tparTestRefRangesRow In qTestRefRanges
                RangeRow.IsDeleted = True
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' gets the current rangetype
    ''' </summary>
    ''' <param name="pSampleType"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SG 27/07/2010</remarks>
    Private Function RefGetActiveRangeType(ByVal pSampleType As String) As String
        Dim myRangeType As String = ""
        Try
            Dim qTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)
            qTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples _
                               Where a.SampleType = pSampleType _
                               Select a).ToList()

            If qTestSamples.Count > 0 Then
                If qTestSamples.First().IsActiveRangeTypeNull Then
                    myRangeType = ""
                Else
                    myRangeType = qTestSamples.First().ActiveRangeType
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " RefGetActiveRangeType ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")

            myRangeType = ""
        End Try

        Return myRangeType

    End Function

    ''' <summary>
    ''' Get the Test Name for the DB.
    ''' </summary>
    ''' <param name="pTestName"></param>
    ''' <returns></returns>
    ''' <remarks>Created by: TR 25/01/2011</remarks>
    Public Function GetTestTestName(ByVal pTestName As String) As TestsDS
        Dim myResult As New TestsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestDelegate As New TestsDelegate

            myGlobalDataTO = myTestDelegate.ExistsTestName(Nothing, pTestName)

            If myGlobalDataTO.HasError Then
                'Error getting the list of defined Sample Types, show the error message
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            Else
                myResult = CType(myGlobalDataTO.SetDatos, TestsDS)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetTestByShortName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myResult

    End Function

    ''' <summary>
    ''' Validate if the test name exist. on the TestListView.
    ''' validation is done when create a new test.
    ''' </summary>
    ''' <param name="pTestName">Test Name to Search.</param>
    ''' <returns>
    ''' True if name exist.
    ''' False if not exist.
    ''' </returns>
    ''' <remarks>
    ''' Created by: TR 22/03/2010
    ''' </remarks>
    Private Function TestNameExits(ByVal pTestName As String, ByVal pTestId As Integer) As Boolean
        Dim myResult As Boolean = False
        Try
            Dim mytestsDS As New TestsDS
            mytestsDS = GetTestTestName(pTestName.TrimStart().TrimEnd())

            If mytestsDS.tparTests.Rows.Count > 0 Then
                If mytestsDS.tparTests(0).TestID <> pTestId Then
                    myResult = True
                End If
                If Not myResult AndAlso CopyTestData AndAlso EditionMode Then
                    myResult = True
                End If
            End If

            ''ValidationError = False 'AG 28/09/2010
            'For Each Test As ListViewItem In TestListView.Items
            '    If CopyTestData Then
            '        If Test.Text.ToUpper() = pTestName.ToUpper() Then
            '            myResult = True
            '            ValidationError = True
            '            Exit For
            '        End If

            '    Else
            '        If Test.Text.ToUpper() = pTestName.ToUpper() AndAlso _
            '                                    CType(Test.Name, Integer) <> pTestId Then
            '            myResult = True
            '            ValidationError = True
            '            Exit For
            '        End If
            '    End If

            'Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " TestNameExits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myResult

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Validate if the value is betwen the range define.
    ''' The control to validate must be a BSTesxtBox control.
    ''' </summary>
    ''' <param name="pValue">Value to validate</param>
    ''' <param name="pFieldValidation">Validation to compare.</param>
    ''' <returns>
    ''' True if value is out of range.
    ''' False is value is between range.
    ''' </returns>
    ''' <remarks>
    ''' Created by: TR 25/03/2010
    ''' </remarks>
    Private Function ValidateControlLimits(ByVal pValue As Decimal, ByVal pFieldValidation As FieldLimitsEnum, ByVal pControl As BSTextBox) As Boolean
        Dim myError As Boolean = False
        Try
            Dim FieldLimitsDS As New FieldLimitsDS
            FieldLimitsDS = GetControlsLimits(pFieldValidation)
            If FieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                If pValue < FieldLimitsDS.tfmwFieldLimits(0).MinValue Or pValue > FieldLimitsDS.tfmwFieldLimits(0).MaxValue Then
                    myError = True
                End If
                If Not FieldLimitsDS.tfmwFieldLimits(0).IsDecimalsAllowedNull Then
                    'TR VALIDATE THE DECIMAL PLACES.
                    If pValue.ToString().Split(CChar(SystemInfoManager.OSDecimalSeparator)).Count > 1 Then
                        If pValue.ToString().Split(CChar(SystemInfoManager.OSDecimalSeparator))(1).Length > FieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed Then
                            'pControl.Text.Remove( pValue.ToString().Split(CChar(SystemsInformationTO.OSDecimalSeparator))(1).Length +1, 
                            pControl.Text = pValue.ToString("N" & FieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed)
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateControlLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myError
    End Function

    Private Function CheckProzoneMinCycle(ByVal ReadingCycle As Decimal) As Integer
        Dim result As Integer = 0

        Try
            'Validate if Mono reagent or Bi Reagent.
            Select Case AnalysisModeCombo.SelectedValue.ToString()

                Case "MREP", "MRFT", "MRK"  'Monoreagent
                    'validate if Mono Chromatic.
                    If ReadingModeCombo.SelectedValue.ToString() = "MONO" Then
                        result = ConvertCycletoTime(CType(ReadingCycle + 4, Integer))
                        'Validate if Bichromatic.
                    ElseIf ReadingModeCombo.SelectedValue.ToString() = "BIC" Then
                        'Validate if it's even or odd.
                        If FirstReadingCycleUpDown.Value Mod 2 = 0 Then
                            'Odd
                            result = ConvertCycletoTime(CType(ReadingCycle + 5, Integer))
                        Else
                            'Even
                            result = ConvertCycletoTime(CType(ReadingCycle + 4, Integer))
                        End If
                    End If
                    Exit Select

                Case "BREP", "BRDIF", "BRFT", "BRK" 'Bireagent
                    If ReadingModeCombo.SelectedValue.ToString() = "MONO" Then
                        result = ConvertCycletoTime(CType(ReadingCycle + 4, Integer))
                        'Validate if Bichromatic.
                    ElseIf ReadingModeCombo.SelectedValue.ToString() = "BIC" Then
                        'Validate if it's even or odd.
                        If SecondReadingCycleUpDown.Value Mod 2 = 0 Then
                            'Odd
                            result = ConvertCycletoTime(CType(ReadingCycle + 5, Integer))
                        Else
                            'Even
                            result = ConvertCycletoTime(CType(ReadingCycle + 4, Integer))
                        End If
                    End If
                    Exit Select

                Case Else
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CheckProzoneTime ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return result

    End Function

    ''' <summary>
    ''' Convert Cycle values to Time value.
    ''' </summary>
    ''' <param name="CycleNumber"></param>
    ''' <returns></returns>
    ''' <remarks>Created by: TR 26/03/2010</remarks>
    Private Function ConvertCycletoTime(ByVal CycleNumber As Integer) As Integer
        Dim result As Integer
        Try
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
            'TR 14/03/2011 -Add the analyzer model on the query.
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                            Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                            AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                            Select a).ToList()

            If qswParameter.Count > 0 Then
                result = CType((CycleNumber - 1) * qswParameter.First().ValueNumeric, Integer)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ConvertCycletoTime ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return result

    End Function

    ''' <summary>
    ''' Validate the prozone times value.
    ''' </summary>
    ''' <remarks>Created by:TR 26/03/2010</remarks>
    Private Sub SetProzone1Times()
        Try
            Dim clearProzoneValues As Boolean = False

            If Not ValidationError Then
                'TR 07/05/2011 -Do not implement turbidimetry
                'If TurbidimetryCheckBox.Checked AndAlso FirstReadingCycleUpDown.Text <> "" AndAlso _
                '                                    Not AnalysisModeCombo.SelectedValue Is Nothing Then
                'TR 07/05/2011 -END
                'TR 07/05/2011 -implement this validation without the turbidimetry
                If FirstReadingCycleUpDown.Text <> "" AndAlso Not AnalysisModeCombo.SelectedValue Is Nothing Then

                    'AG 08/07/2010 - Check if prozones controls are informed or not. If dont then dont assign any value!!!
                    If ProzonePercentUpDown.Text = "" And ProzoneT1UpDown.Text = "" And ProzoneT2UpDown.Text = "" Then
                        clearProzoneValues = True
                    End If
                    'END AG 08/07/2010

                    'TR 08/04/2010 -
                    ProzoneT1UpDown.Enabled = True
                    ProzoneT2UpDown.Enabled = True
                    ProzonePercentUpDown.Enabled = True
                    Dim prozoneT1Reset As Boolean = False
                    Dim prozoneT2Reset As Boolean = False

                    'END TR 08/04/2010 
                    Dim myMinReadingCycleTime As Integer = 0
                    Dim myMaxReadingcycletime As Decimal = 0
                    'validate if bireagent or mono to send the parameter to validate.
                    Select Case AnalysisModeCombo.SelectedValue.ToString()
                        Case "MREP", "MRFT", "MRK"
                            If ProzoneT1UpDown.Text = "" Then
                                ProzoneT1UpDown.ResetText()
                            End If
                            'validate with first reading 
                            myMinReadingCycleTime = CheckProzoneMinCycle(FirstReadingCycleUpDown.Minimum)
                            If AnalysisModeCombo.SelectedValue.ToString() = "MREP" Then
                                'ProzoneT1UpDown.Maximum = FirstReadingSecUpDown.Value   'AG TR 07/04/2010 
                                myMaxReadingcycletime = FirstReadingSecUpDown.Value   'AG TR 07/04/2010 

                            Else
                                'ProzoneT1UpDown.Maximum = SecondReadingSecUpDown.Value   'AG TR 07/04/2010
                                myMaxReadingcycletime = SecondReadingSecUpDown.Value   'AG TR 07/04/2010
                            End If

                            Exit Select

                        Case "BREP", "BRDIF", "BRFT", "BRK"
                            If ProzoneT1UpDown.Text = "" Then
                                ProzoneT1UpDown.ResetText()
                            End If
                            'validate with second reading.
                            myMinReadingCycleTime = CheckProzoneMinCycle(SecondReadingCycleUpDown.Minimum)
                            'ProzoneT1UpDown.Maximum = SecondReadingSecUpDown.Value   'AG 06/04/2010
                            myMaxReadingcycletime = SecondReadingSecUpDown.Value
                            Exit Select
                    End Select

                    If myMaxReadingcycletime > myMinReadingCycleTime Then 'TR 08/04/2010 -Validate that the Max value is greather than the Min value.
                        If ProzoneT1UpDown.Text <> "" Then
                            If ProzoneT1UpDown.Value < myMinReadingCycleTime Then
                                'ProzoneT1UpDown.ResetText()
                                prozoneT1Reset = True
                            End If

                            If ProzoneT1UpDown.Value > myMaxReadingcycletime Then
                                prozoneT1Reset = True
                            End If
                        End If

                        ProzoneT1UpDown.Minimum = myMinReadingCycleTime
                        ProzoneT1UpDown.Maximum = myMaxReadingcycletime
                        If prozoneT1Reset Then
                            ProzoneT1UpDown.ResetText()
                        End If

                        'set step
                        Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
                        Dim MachineCycle As Integer = 0

                        'TR 14/03/2011 -Add the analyzer model on the query.
                        qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                        Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                        AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                                        Select a).ToList()
                        If qswParameter.Count > 0 Then
                            MachineCycle = CType(qswParameter.First().ValueNumeric, Integer)
                        End If
                        'Set up steps
                        If Not ReadingModeCombo.SelectedValue Is Nothing Then
                            If ReadingModeCombo.SelectedValue.ToString() = "MONO" Then
                                ProzoneT1UpDown.Increment = MachineCycle
                            ElseIf ReadingModeCombo.SelectedValue.ToString() = "BIC" Then
                                ProzoneT1UpDown.Increment = MachineCycle * 2
                            End If
                        End If
                        'TR 07/04/2010

                        If ProzoneT2UpDown.Text <> "" Then
                            If ProzoneT2UpDown.Value < myMinReadingCycleTime Then
                                'ProzoneT2UpDown.ResetText()
                                'ProzoneT2UpDown.Text = ""
                                prozoneT2Reset = True
                            End If
                            If ProzoneT2UpDown.Value > myMaxReadingcycletime Then
                                'ProzoneT2UpDown.ResetText()
                                'ProzoneT2UpDown.Text = ""
                                prozoneT2Reset = True
                            End If
                        End If
                        'TR 08/04/2010
                        ProzoneT2UpDown.Maximum = ProzoneT1UpDown.Maximum
                        ProzoneT2UpDown.Minimum = ProzoneT1UpDown.Minimum
                        ProzoneT2UpDown.Increment = ProzoneT1UpDown.Increment
                        'END TR 08/04/2010
                        If prozoneT2Reset Then
                            ProzoneT2UpDown.ResetText()
                        End If
                    Else
                        'TR 08/04/2010 -clear the data
                        ProzoneT1UpDown.ResetText()
                        ProzoneT2UpDown.ResetText()
                        ProzonePercentUpDown.ResetText()
                        'TR 05/05/2011-Commented
                        'ProzoneT1UpDown.Enabled = False
                        'ProzoneT2UpDown.Enabled = False
                        'ProzonePercentUpDown.Enabled = False
                        'TR 05/05/2011-END
                        clearProzoneValues = True 'AG 08/07/2010

                    End If 'END TR 08/04/2010
                Else
                    'TR 08/04/2010 -clear the data
                    ProzoneT1UpDown.ResetText()
                    ProzoneT2UpDown.ResetText()
                    ProzonePercentUpDown.ResetText()

                    'TR 05/05/2011-Commented
                    'ProzoneT1UpDown.Enabled = False
                    'ProzoneT2UpDown.Enabled = False
                    'ProzonePercentUpDown.Enabled = False
                    'TR 05/05/2011-END

                    'END TR 08/04/2010 -clear the data

                    clearProzoneValues = True 'AG 08/07/2010
                End If

                'AG 08/07/2010 - Sometimes when activate Turbi check the prozone values has default value. When activate turbi we
                'have only to program the prozone limits but dont assign default values
                If clearProzoneValues Then
                    ProzonePercentUpDown.ResetText()
                    ProzoneT1UpDown.ResetText()
                    ProzoneT2UpDown.ResetText()
                End If
                'END AG 08/07/2010

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateProzeTimes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Update the test position number
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 29/03/2010
    ''' </remarks>
    Private Sub UpdateTestPosition()
        Try
            If TestsPositionChange Then
                Dim myListTestPositionTO As New List(Of TestPositionTO)
                Dim myTestPostionTO As New TestPositionTO()
                Dim position As Integer = 0
                'go throught all the items on the list view set the new position.
                'and insert into the Tests position TO.
                For Each myTest As ListViewItem In TestListView.Items
                    position += 1
                    myTestPostionTO.TestID = CType(myTest.Name, Integer)
                    myTestPostionTO.TestPosition = position
                    myListTestPositionTO.Add(myTestPostionTO)
                    'clear the object.
                    myTestPostionTO = New TestPositionTO
                Next
                'validate if there are any items before calling the delegate.
                If myListTestPositionTO.Count > 0 Then
                    Dim myTestDelegate As New TestsDelegate
                    Dim myGlobalDataTO As New GlobalDataTO
                    'update the position.
                    myGlobalDataTO = myTestDelegate.UpdateTestPosition(Nothing, myListTestPositionTO)
                    If myGlobalDataTO.HasError Then
                        ShowMessage("Error", myGlobalDataTO.ErrorCode)
                    End If
                    TestsPositionChange = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateTestPosition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to validate the prozones values.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 
    ''' Modified by AG 05/12/2010 - add optional parameter pSaveButtonClicked
    ''' </remarks>
    Private Sub ValidateProzones(Optional ByVal pSaveButtonClicked As Boolean = False)
        Try
            'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
            ValidationError = False
            'Validate required Values 

            'TR 07/05/2011 -Validate the prozone values 
            If ProzonePercentUpDown.Text <> "" AndAlso (ProzoneT1UpDown.Text = "" OrElse ProzoneT2UpDown.Text = "") Then
                BsErrorProvider1.SetError(ProzoneT1UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                BsErrorProvider1.SetError(ProzoneT1UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            ElseIf ProzonePercentUpDown.Text = "" AndAlso (ProzoneT1UpDown.Text <> "" OrElse ProzoneT2UpDown.Text <> "") Then
                BsErrorProvider1.SetError(ProzonePercentUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            End If

            If ProzoneT1UpDown.Text <> "" OrElse ProzoneT2UpDown.Text <> "" Then
                If ProzoneT1UpDown.Text = ProzoneT2UpDown.Text Then
                    BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.PROZONE_TIMES.ToString))
                    ValidationError = True
                End If
                'TR 17/10/2011 -Commented to avoid the exit try
                ''If there are validation error then exit try
                'If ValidationError Then
                '    Exit Try 
                'End If
                'TR 17/10/2011 -END.
            End If
            'TR 07/05/2011 -END.

            'TR 17/10/2011 -If not validation error continue.
            If Not ValidationError Then
                'this validation only apply for Bichromatics
                If ReadingModeCombo.SelectedValue.ToString() = "BIC" Then
                    Dim T1Cycle As Integer = 0
                    Dim T2Cycle As Integer = 0
                    Dim mainCycle As Integer = 0
                    Dim myMachineCycle As Integer = 0

                    'get the machine cycle
                    Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)
                    'TR 14/03/2011 -Add the analyzer model on the query.
                    qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                                    Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                                    AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                                    Select a).ToList()

                    If qswParameter.Count > 0 Then
                        myMachineCycle = CType(qswParameter.First().ValueNumeric, Integer)
                    End If

                    If myMachineCycle > 0 Then
                        'calculate the cycles values for the T1 and T2
                        If ProzoneT1UpDown.Text <> "" Then T1Cycle = CType((ProzoneT1UpDown.Value / myMachineCycle) + 1, Integer)
                        If ProzoneT2UpDown.Text <> "" Then T2Cycle = CType((ProzoneT2UpDown.Value / myMachineCycle) + 1, Integer)
                    End If
                    If T1Cycle > 0 AndAlso T2Cycle > 0 Then
                        mainCycle = CType((ProzoneT1UpDown.Minimum / myMachineCycle) + 1, Integer)
                        While mainCycle < T1Cycle
                            mainCycle = mainCycle + 2
                        End While

                        If mainCycle <> T1Cycle Then
                            ProzoneT1UpDown.ResetText()
                            ValidationError = True
                        End If

                        mainCycle = CType((ProzoneT2UpDown.Maximum / myMachineCycle) + 1, Integer)
                        While mainCycle > T2Cycle
                            mainCycle = mainCycle - 2
                        End While

                        If mainCycle <> T2Cycle Then
                            ProzoneT2UpDown.ResetText()
                            ValidationError = True
                        End If

                        If ValidationError Then
                            If ProzoneT1UpDown.Text = "" OrElse ProzoneT2UpDown.Text = "" Then
                                'AG 05/12/2010 - show message and go to options tab only when optional parameter is TRUE
                                'ShowMessage("Warning", GlobalEnumerates.Messages.CHANGE_PROZONE_PROGRAM.ToString) '"CHANGE_PROZONE_PROGRAM")
                                If pSaveButtonClicked Then
                                    ShowMessage("Warning", GlobalEnumerates.Messages.CHANGE_PROZONE_PROGRAM.ToString) '"CHANGE_PROZONE_PROGRAM")
                                    TestDetailsTabs.SelectTab("OptionsTab")
                                End If
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ProzonesValidation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the preparation volume is on the limits
    ''' </summary>
    ''' <returns>
    ''' True if value is on the limits; false if not ok.
    ''' </returns>
    ''' <remarks>
    ''' Created by: TR 08/04/2010
    ''' </remarks>
    Private Function ValidatePreparationVolumeLimits() As Boolean
        Dim myResult As Boolean = False
        Try
            'TR 23/01/2012 -validate is not a copyTestData if copy test validation is not needed.
            If EditionMode AndAlso Not CopyTestData Then
                Dim myPreparationVol As Decimal = 0
                If Not VolR2UpDown.Enabled Then
                    'validate that at leat the R2 Vol is informed.
                    If VolR1UpDown.Text <> "" Then
                        'validate the preparation volume with R2
                        myPreparationVol = SampleVolUpDown.Value + VolR1UpDown.Value
                    Else
                        'return true because the there's not reagent volume informed.
                        myResult = True
                    End If
                Else
                    If VolR2UpDown.Text <> "" Then
                        'validate the preparation volume with all volumes
                        myPreparationVol = SampleVolUpDown.Value + VolR1UpDown.Value + VolR2UpDown.Value
                    Else
                        myResult = True
                    End If
                End If

                Dim myFieldLimitsDS As New FieldLimitsDS()
                'get the Preparation volume Limits
                myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.PREPARATION_VOLUME, MyBase.AnalyzerModel)

                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    'Validate if limit are out of range.
                    If myPreparationVol >= myFieldLimitsDS.tfmwFieldLimits(0).MinValue AndAlso _
                                           myPreparationVol <= myFieldLimitsDS.tfmwFieldLimits(0).MaxValue Then
                        myResult = True
                    End If
                End If
            Else
                'TR 23/01/2012 -Send true value if copy a test because test values has been validate before
                myResult = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidatePreparationVolume ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate the experimental calibrator values
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 15/06/2010
    ''' </remarks>
    Private Sub ValidateExperimentalCalib()
        Try
            If MultipleCalibRadioButton.Checked Then
                'then validate if there is any concentration values
                If Not ConcentrationGridView.Rows.Count > 0 Then
                    ValidationError = True 'AG 20/12/2010
                    BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                Else
                    ValidationError = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateExperimentalCalib ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' Create a new tes locally
    '''' </summary>
    '''' <remarks>
    '''' TR 10/03/2010
    '''' </remarks>
    'Private Sub NewSelectedTest()
    '    Try
    '        Dim pTestID As Integer = 0
    '        If TestListView.SelectedItems.Count = 1 Then
    '            EditButton.Enabled = True
    '            'Validate if it's the same as selected.
    '            If SelectedTestDS.tparTests.Rows.Count > 0 Then
    '                pTestID = SelectedTestDS.tparTests(0).TestID
    '                If Not TestListView.SelectedItems(0).Name = pTestID.ToString() Then
    '                    If EditionMode AndAlso (ChangesMade Or bsTestRefRanges.ChangesMade) AndAlso SelectionControl = 0 Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
    '                        If ShowMessage("Warning", GlobalEnumerates.Messages.SAVE_PENDING.ToString) = Windows.Forms.DialogResult.Yes Then
    '                            EditionMode = False
    '                            ChangesMade = False
    '                            CalibratorChanges = False 'TR 08/03/2011
    '                            NewTest = False
    '                            Me.bsTestRefRanges.isEditing = False 'SG 06/09/2010
    '                            EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
    '                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
    '                        Else
    '                            If Not NewTest Then
    '                                SelectionControl = 1
    '                                TestListView.SelectedItems.Clear()
    '                                TestListView.Items(SelectedTestDS.tparTests(0).TestID.ToString()).Selected = True
    '                                SelectionControl = 0
    '                                TestListView.Update()
    '                                TestListView.Select()
    '                            End If
    '                        End If
    '                    Else
    '                        EditionMode = False
    '                        ChangesMade = False
    '                        CalibratorChanges = False 'TR 08/03/2011
    '                        Me.bsTestRefRanges.isEditing = False 'SG 06/09/2010
    '                        EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
    '                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
    '                    End If
    '                End If
    '            Else
    '                If Not SelectedSampleTypeCombo.SelectedValue Is Nothing Then
    '                    BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SelectedSampleTypeCombo.SelectedValue.ToString())
    '                Else
    '                    BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
    '                End If

    '            End If
    '        ElseIf TestListView.SelectedItems.Count > 1 Then
    '            EditButton.Enabled = False

    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " NewSelectedTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub


    ''' <summary>
    ''' ValidatePostDilution
    ''' </summary>
    ''' <param name="pPostType"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' AG 26/03/2010 - add new pPostType optional parameter
    ''' Modified by: TR 19/10/2011 -Commented code no needed to avoid the Exit Try.
    ''' </remarks>
    Private Function ValidatePostDilution(Optional ByVal pPostType As String = "") As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myVolCalc As New VolumeCalculations
            myVolCalc.R1Vol = 0
            myVolCalc.R2Vol = 0
            myVolCalc.WashVol = 0
            myVolCalc.sampleVol = 0

            'AG 26/03/2010 - Reset steps values before calcute them
            myVolCalc.sampleSteps = 0
            myVolCalc.WashStepsVol = 0
            myVolCalc.R1Steps = 0
            myVolCalc.R2Steps = 0
            'END AG 26/03/2010

            myVolCalc.sampleVol = CType(SampleVolUpDown.Value, Single)
            myVolCalc.WashVol = CType(WashSolVolUpDown.Value, Single)

            If VolR1UpDown.Text <> "" Then
                myVolCalc.R1Vol = CType(VolR1UpDown.Value, Single)
            End If

            If VolR2UpDown.Text <> "" Then
                myVolCalc.R2Vol = CType(VolR2UpDown.Value, Single)
            End If

            myVolCalc.AbsorbanceFlag = AbsCheckBox.Checked
            myVolCalc.AnalysisMode = AnalysisModeCombo.SelectedValue.ToString

            If pPostType = "RED" Or pPostType = "" Then 'AG 26/03/2010
                'Reduce
                myVolCalc.PostDilutionFactor = CSng(RedPostDilutionFactorTextBox.Text)
                myGlobalDataTO = CheckPostdilutions("RED", myVolCalc)
                If Not myGlobalDataTO.HasError Then
                    PostReduce = CType(myGlobalDataTO.SetDatos, VolumeCalculations)
                    If PostReduce.hasError Then
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = PostReduce.ErrorCode
                        'BsErrorProvider1.SetError(RedPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.WRONG_POSTDILUTION_FACTOR.ToString))   'AG 08/07/2010

                        'TR 17/05/2012 -Set the error to the group box 
                        BsErrorProvider1.SetError(PostDilutionFactorGroupbox, GetMessageText(GlobalEnumerates.Messages.WRONG_POSTDILUTION_FACTOR.ToString))
                        ValidationError = True 'AG 08/07/2010
                        'Exit Try 'AG 08/07/2010
                    End If
                    'AG 26/03/2010
                End If
            End If
            'END AG 26/03/2010

            'AG 26/03/2010 - Reset steps values before calcute them
            myVolCalc.sampleSteps = 0
            myVolCalc.WashStepsVol = 0
            myVolCalc.R1Steps = 0
            myVolCalc.R2Steps = 0
            'END AG 26/03/2010

            If pPostType = "INC" Or pPostType = "" Then 'AG 26/03/2010
                myVolCalc.PostDilutionFactor = CSng(IncPostDilutionFactorTextBox.Text)
                'Increment
                myGlobalDataTO = CheckPostdilutions("INC", myVolCalc)
                If Not myGlobalDataTO.HasError Then
                    PostIncrease = CType(myGlobalDataTO.SetDatos, VolumeCalculations)
                    If PostIncrease.hasError Then
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = PostIncrease.ErrorCode
                        'BsErrorProvider1.SetError(IncPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.WRONG_POSTDILUTION_FACTOR.ToString))   'AG 08/07/2010

                        'TR 17/05/2012 -Set the error to the group box 
                        BsErrorProvider1.SetError(PostDilutionFactorGroupbox, GetMessageText(GlobalEnumerates.Messages.WRONG_POSTDILUTION_FACTOR.ToString))
                        ValidationError = True
                        'Exit Try 'AG 08/07/2010
                    End If
                End If
            End If 'AG 26/03/2010
            'TR 17/10/2011 -Commented 
            'If myGlobalDataTO.HasError Then
            '    ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidatePostDilution ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myGlobalDataTO

    End Function

    ''' <summary>
    ''' Select the first control when select an specific tab.
    ''' </summary>
    ''' <param name="pTabName">Tab Name</param>
    ''' <remarks>
    ''' Created by: TR 19/03/2010
    ''' </remarks>
    Private Sub FocusControlByTab(ByVal pTabName As String)
        Try
            Select Case pTabName
                Case "GeneralTab"
                    SampleTypeCheckList.Visible = False 'DL 14/01/2012

                    If Not TestNameTextBox.Enabled Then
                        TestNameTextBox.Focus()
                    Else
                        TestNameTextBox.Select()
                    End If

                    Exit Select

                Case "ProcedureTab"
                    ReadingModeCombo.Select()
                    Exit Select

                Case "CalibrationTab"
                    BlankTypesCombo.Select()
                    Exit Select

                Case "OptionsTab"
                    BlankAbsorbanceUpDown.Select()
                    Exit Select

                Case Else
                    Exit Select

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FocusControlByTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Delete the selected test on the testlistview.
    ''' </summary>
    ''' <remarks>Created by: TR 22/03/2010</remarks>
    Private Sub DeleteSelectedTest(Optional ByVal ShowConfirmation As Boolean = True)
        Try
            If TestListView.SelectedItems.Count > 0 Then
                'TR 23/03/2010 Validate is not in use test.
                If Not isCurrentTestInUse Then

                    'Show warning message.
                    If Not ShowConfirmation OrElse ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes Then
                        Me.Cursor = Cursors.WaitCursor
                        ReadyToSave = True
                        Dim myDelTestListIndex As Integer = -1
                        For Each testID As ListViewItem In TestListView.SelectedItems
                            myDelTestListIndex = testID.Index
                            If String.Compare(testID.Tag.ToString, "False", False) = 0 Then
                                DeleteRefRanges(CType(testID.Name, Integer)) 'SG 06/09/2010
                                DeleteTestSampleType(CType(testID.Name, Integer), "", testID.Text) 'TR 24/11/2010 -Add the Test name 
                                'testID.Remove()'TR 24/11/2010 -Commented
                            End If
                        Next
                        'TR 24/11/2010 -Validate if there are dependencies
                        If ValidateDependenciesOnDeletedElemets() = Windows.Forms.DialogResult.OK Then
                            If SaveChanges(False) Then 'validate the function before to remove the elements from the list.
                                'TR 30/11/2010 -Select the first element on the test list
                                If TestListView.Items.Count > 0 Then
                                    TestListView.Items(0).Selected = True
                                    QuerySelectedTest() 'TR 24/11/2010 -Search selected test.
                                Else
                                    'AG 07/07/2010
                                    ClearParametersArea()
                                End If
                                'TR 30/11/2010 -END
                            End If
                            EnableDisableControls(TestDetailsTabs.Controls, False, True) 'TR 30/11/2010 after selecting disable cntrols
                        Else
                            DeletedTestProgramingList.Clear()
                        End If

                        Me.Cursor = Cursors.Default
                    End If
                End If
                'END TR 23/03/2010 Validate is not in use Test.
            End If

            'DL 15/07/2010. Begin
            If TestListView.Items.Count < 1 Then
                SetEmptyValues()
                EditButton.Enabled = False
                DeleteButton.Enabled = False
                SaveButton.Enabled = False

                CancelButton.Enabled = False 'TR 08/11/2010
            End If
            'DL 15/07/2010. End

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DeletedSelectedTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created AG 08/07/2010</remarks>
    Private Sub PrepareGeneralTab()
        Try
            TestDescriptionTextBox.Clear()
            TestDescriptionTextBox.Enabled = False
            TestNameTextBox.Clear()
            ShortNameTextBox.Clear()

            'SG 23/07/2010
            'DL 11/01/2012, Begin
            'SampleTypeCombo.SelectedIndex = -1
            'SampleTypeCombo.DataSource = Nothing
            'SampleTypeCombo.Items.Clear()
            '            bsSampleTypeText.Text = String.Empty
            SampleTypeCboEx.Items.Clear()
            'DL 11/01/2012. End
            'SG 23/07/2010

            If AnalysisModeCombo.Items.Count > 0 Then AnalysisModeCombo.SelectedIndex = 0
            UnitsCombo.SelectedIndex = -1
            DecimalsUpDown.Value = 0
            ReplicatesUpDown.Value = 1
            ReactionTypeCombo.SelectedIndex = 0
            ReportsNameTextBox.Clear()
            AbsCheckBox.Checked = False
            'TurbidimetryCheckBox.Checked = False 'TR 07/05/2011 -Do not implement turbidimetry

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " PrepareGeneralTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Procedure Tab
    ''' </summary>
    ''' <remarks>Created AG 08/07/2010</remarks>
    Private Sub PrepareProcedureTab()
        Try
            ReadingModeCombo.SelectedValue = "MONO"
            MainFilterCombo.SelectedIndex = 0   'AG 08/07/2010
            If Not ReadingModeCombo.SelectedValue Is Nothing Then
                If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                Else
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = True
                End If
            End If
            'SG 23/07/2010 do not fill the sampletype combos when creating a new test
            'FillSampleTypeCombo()
            FillSelectedSampleTypeCombo() ' dl 15/07/2010
            'END SG 23/07/2010
            If Not ReadingModeCombo.SelectedValue Is Nothing Then
                If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                Else
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = True
                End If
            End If
            SampleVolUpDown.ResetText()
            SampleVolUpDown.Value = SampleVolUpDown.Minimum + 1
            SampleVolUpDown.ResetText()

            'TR 02/01/2012 -The Washing Volume is Fixed to the maxvalue = 1,2
            'WashSolVolUpDown.Value = 1
            WashSolVolUpDown.Value = WashSolVolUpDown.Maximum
            'TR 02/01/2012 -END.

            VolR1UpDown.ResetText()
            VolR1UpDown.Value = VolR1UpDown.Minimum + 1
            VolR1UpDown.ResetText()

            VolR2UpDown.ResetText()
            VolR2UpDown.Value = VolR2UpDown.Minimum + 1
            VolR2UpDown.ResetText()

            FirstReadingCycleUpDown.ResetText()
            FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
            FirstReadingCycleUpDown.ResetText()
            FirstReadingSecUpDown.ResetText()
            FirstReadingSecUpDown.ResetText()

            SecondReadingCycleUpDown.ResetText()
            SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum
            SecondReadingCycleUpDown.ResetText()
            SecondReadingSecUpDown.ResetText()
            SecondReadingSecUpDown.ResetText()

            PredilutionFactorTextBox.Clear()
            'AG 08/07/2010
            'If PredilutionModeCombo.Items.Count > 0 Then PredilutionModeCombo.SelectedIndex = 0 Else PredilutionModeCombo.SelectedIndex = -1
            PredilutionModeCombo.SelectedIndex = -1
            PredilutionFactorCheckBox.Checked = False
            PredilutionModeCombo.Enabled = False
            PredilutionFactorTextBox.Enabled = False
            PredilutionFactorTextBox.BackColor = Color.Gainsboro

            'TR 12/04/2011
            DiluentComboBox.SelectedIndex = -1
            DiluentComboBox.Enabled = False
            'TR 12/04/2011 -END


            AutoRepetitionCheckbox.Checked = False
            RedPostDilutionFactorTextBox.Text = "1"
            IncPostDilutionFactorTextBox.Text = "1"
            'END AG 08/07/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareProcedureTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Calibration Tab
    ''' </summary>
    ''' <remarks>Created AG 08/07/2010</remarks>
    Private Sub PrepareCalibrationTab()
        Try
            If BlankTypesCombo.Items.Count > 0 Then BlankTypesCombo.SelectedIndex = 0 'TR 29/06/2010 -Select the first element on the list.

            'TR 29/06/2010 -Set the 3 as defaul value.
            BlankReplicatesUpDown.Value = 3
            CalReplicatesUpDown.Value = 3
            'TR 29/06/2010 -End.

            BsCalibNumberTextBox.Clear() 'TR 01/07/2010
            CalibrationFactorTextBox.Clear()

            'TR  29/06/2010 -Set by defaul calibrator radito button
            'FactorRadioButton.Checked = False  'TR  29/06/2010 -Set factor false
            MultipleCalibRadioButton.Checked = True
            AddCalibratorButton.Enabled = True
            AddCalibratorButton.Select()
            AlternativeCalComboBox.Enabled = False
            CalibrationFactorTextBox.BackColor = Color.Gainsboro
            CalibrationFactorTextBox.Enabled = False

            ClearConcentrationsCalibValues()
            'TR  29/06/2010 -END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareCalibrationTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Option Tab
    ''' </summary>
    ''' <remarks>Created AG 08/07/2010</remarks>
    Private Sub PrepareOptionTab()
        Try

            BlankAbsorbanceUpDown.Value = BlankAbsorbanceUpDown.Minimum
            BlankAbsorbanceUpDown.ResetText()

            KineticBlankUpDown.Value = KineticBlankUpDown.Minimum + 1
            KineticBlankUpDown.ResetText()

            LinearityUpDown.Value = LinearityUpDown.Minimum + 1
            LinearityUpDown.ResetText()

            DetectionUpDown.Value = DetectionUpDown.Minimum
            DetectionUpDown.ResetText()

            'SG 11/06/2010
            FactorLowerLimitUpDown.Value = FactorLowerLimitUpDown.Minimum
            FactorLowerLimitUpDown.ResetText()

            FactorUpperLimitUpDown.Value = FactorUpperLimitUpDown.Minimum
            FactorUpperLimitUpDown.ResetText()

            RerunLowerLimitUpDown.Value = RerunLowerLimitUpDown.Minimum
            RerunLowerLimitUpDown.ResetText()

            RerunUpperLimitUpDown.Value = RerunUpperLimitUpDown.Minimum
            RerunUpperLimitUpDown.ResetText()
            'END SG 11/06/2010

            ProzonePercentUpDown.ResetText()
            'ProzonePercentUpDown.Enabled = False 'TR 05/05/2011
            ProzoneT1UpDown.ResetText()
            'ProzoneT1UpDown.Enabled = False 'TR 05/05/2011
            ProzoneT2UpDown.ResetText()
            'ProzoneT2UpDown.Enabled = False 'TR 05/05/2011

            'TR 29/06/2010
            SlopeAUpDown.Value = 0
            SlopeAUpDown.ResetText()
            SlopeBUpDown.Value = 0
            SlopeBUpDown.ResetText()
            'TR 29/06/2010

            SubstrateDepleUpDown.ResetText()
            SubstrateDepleUpDown.Enabled = False
            'TR 04/04/2011 validate design mode
            If Not Me.DesignMode Then
                'AG 12/01/2011
                'Me.BsTestRefRanges.ClearData() 'SG 06/09/2010
                'Area of Reference Ranges
                SelectedTestRefRangesDS.Clear()

                bsTestRefRanges.TestID = -1
                bsTestRefRanges.SampleType = ""
                bsTestRefRanges.ActiveRangeType = ""
                bsTestRefRanges.MeasureUnit = ""
                bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS

                Dim decimalsNumber As Integer = 0
                If (IsNumeric(DecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(DecimalsUpDown.Text)
                bsTestRefRanges.RefNumDecimals = decimalsNumber

                bsTestRefRanges.ClearReferenceRanges()
                bsTestRefRanges.isEditing = True
                'AG 12/01/2011
            End If

            'TR 21/07/2010 -Set the decimals values 
            SetupDecimalsNumber(CInt(DecimalsUpDown.Value))
            'TR 21/07/2010 -End.

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " PrepareOptionTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Method incharge to load the buttons image.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: DL 21/06/2010
    '''              DL 03/11/2010 - Load Icon in Image Property instead of in BackGroundImage Property
    '''              TR 08/11/2010 - Get Icon for Cancel Button
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ADD AddButton, AddSampleTypeButton, AddCalibratorButton, BSReferenceRanges-Add Range Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                AddButton.Image = Image.FromFile(iconPath & auxIconName)
                'AddSampleTypeButton.Image = Image.FromFile(iconPath & auxIconName)'TR 10/05/2011 commented
                AddCalibratorButton.Image = Image.FromFile(iconPath & auxIconName)
                'TR 06/04/2011 -Add image for Add control button.
                AddControls.Image = Image.FromFile(iconPath & auxIconName)

            End If

            'EDIT EditButton
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                EditButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'TR 10/05/2011 -Add sampletype icon
            'auxIconName = GetIconName("CHECKL")

            'DL 11/01/2012. Begin
            'DL 27/09/2011
            'auxIconName = GetIconName("DROPDOWN")
            'If (auxIconName <> "") Then
            'AddSampleTypeButton.Image = Image.FromFile(iconPath & auxIconName)
            'End If
            'DL 11/01/2012. End
            'TR 10/05/2011 -END

            'DELETE DeleteButton; DeleteSampleTypeButton; DeleteGenderAgeButton; BSReferenceRanges-Delete Ranges Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                DeleteButton.Image = Image.FromFile(iconPath & auxIconName)
                DeleteSampleTypeButton.Image = Image.FromFile(iconPath & auxIconName)

                bsTestRefRanges.DeleteButtonImage = Image.FromFile(iconPath & auxIconName)

                'TR 06/04/2011 -Add image to Delete control button.
                DeleteControlButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT PrintTestButton
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                PrintTestButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'JB 30/08/2012 - Hide Print button
            PrintTestButton.Visible = False

            'TR 09/01/2012 -Get the copy test button icon.
            auxIconName = GetIconName("COPY")
            If (auxIconName <> "") Then
                CopyTestButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'TR 09/01/2012 -END.

            'AG 05/09/2014 - BA-1869
            auxIconName = GetIconName("ORDER_TESTS")
            If (auxIconName <> "") Then
                BsCustomOrderButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'SAVE SaveButton
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                SaveButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                CancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EXIT ExitButton
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set up Decimals Number
    ''' </summary>
    ''' <param name="pDecimals"></param>
    ''' <remarks>Created by: AG 09/07/2010</remarks>
    Private Sub SetupDecimalsNumber(ByVal pDecimals As Integer)
        Try
            LinearityUpDown.DecimalPlaces = pDecimals
            'TR 21/07/2010 -Set the value with the corresponding format.
            If LinearityUpDown.Text <> "" Then
                LinearityUpDown.Value = CDec(LinearityUpDown.Text)
            End If

            DetectionUpDown.DecimalPlaces = pDecimals
            If DetectionUpDown.Text <> "" Then
                DetectionUpDown.Value = CDec(DetectionUpDown.Text)
            End If

            RerunLowerLimitUpDown.DecimalPlaces = pDecimals
            If RerunLowerLimitUpDown.Text <> "" Then
                RerunLowerLimitUpDown.Value = CDec(RerunLowerLimitUpDown.Text)
            End If

            RerunUpperLimitUpDown.DecimalPlaces = pDecimals
            If RerunUpperLimitUpDown.Text <> "" Then
                RerunUpperLimitUpDown.Value = CDec(RerunUpperLimitUpDown.Text)
            End If

            'SG 04/10/10
            Me.bsTestRefRanges.RefNumDecimals = pDecimals

            ConcentrationGridView.Refresh()

            'TR 12/04/2011
            RecalculateAllTarget()

            'PENDING:
            ' Detailed reference range table (be carefull the local DS are also updated)
            ' Calibration Theorecial concentration table (be carefull the local DS are also updated)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetupDecimalsNumber ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Clear all the controls on form
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 30/11/2010  
    ''' </remarks>
    Private Sub ClearAllControls()
        Try

            TestDescriptionTextBox.Clear()
            TestDescriptionTextBox.Enabled = False
            TestNameTextBox.Clear()
            ShortNameTextBox.Clear()
            AbsCheckBox.Checked = False
            'TurbidimetryCheckBox.Checked = False 'TR 07/05/2011 -Do not implement turbidimetry
            SampleVolUpDown.ResetText()
            SampleVolUpDown.Value = SampleVolUpDown.Minimum + 1
            SampleVolUpDown.ResetText()
            'TR 02/01/2012 -The Washing Volume is Fixed to the maxvalue = 1,2
            'WashSolVolUpDown.Value = 1
            WashSolVolUpDown.Value = WashSolVolUpDown.Maximum
            'TR 02/01/2012 -END.

            VolR1UpDown.ResetText()
            VolR1UpDown.Value = VolR1UpDown.Minimum + 1
            VolR1UpDown.ResetText()
            VolR2UpDown.ResetText()
            VolR2UpDown.Value = VolR2UpDown.Minimum + 1
            VolR2UpDown.ResetText()
            FirstReadingCycleUpDown.ResetText()
            FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
            FirstReadingCycleUpDown.ResetText()
            FirstReadingSecUpDown.ResetText()
            FirstReadingSecUpDown.ResetText()
            PredilutionFactorTextBox.Clear()
            SecondReadingCycleUpDown.ResetText()
            SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum
            SecondReadingCycleUpDown.ResetText()
            SecondReadingSecUpDown.ResetText()
            SecondReadingSecUpDown.Value = SecondReadingSecUpDown.Minimum
            SecondReadingSecUpDown.ResetText()
            RerunLowerLimitUpDown.ResetText()
            RerunUpperLimitUpDown.ResetText()
            ProzonePercentUpDown.ResetText()
            ProzoneT1UpDown.ResetText()
            ProzoneT2UpDown.ResetText()
            'ProzonePercentUpDown.Enabled = False 'TR 05/05/2011
            'ProzoneT1UpDown.Enabled = False 'TR 05/05/2011
            'ProzoneT2UpDown.Enabled = False 'TR 05/05/2011
            SlopeAUpDown.ResetText()
            SlopeBUpDown.ResetText()
            SubstrateDepleUpDown.ResetText()
            UnitsCombo.SelectedIndex = -1
            RedPostDilutionFactorTextBox.Text = "1"
            IncPostDilutionFactorTextBox.Text = "1"
            CalibrationFactorTextBox.Clear()
            ReportsNameTextBox.Clear()
            BlankAbsorbanceUpDown.ResetText()
            BlankAbsorbanceUpDown.Value = BlankAbsorbanceUpDown.Minimum
            BlankAbsorbanceUpDown.ResetText()
            KineticBlankUpDown.ResetText()
            KineticBlankUpDown.Value = KineticBlankUpDown.Minimum + 1
            KineticBlankUpDown.ResetText()
            LinearityUpDown.ResetText()
            LinearityUpDown.Value = LinearityUpDown.Minimum + 1
            LinearityUpDown.ResetText()

            DetectionUpDown.ResetText()
            DetectionUpDown.Value = CDec(DetectionUpDown.Minimum + 0.9)
            DetectionUpDown.ResetText()

            BlankTypesCombo.SelectedIndex = -1
            BlankReplicatesUpDown.Value = 1
            DecimalsUpDown.Value = 0
            ReplicatesUpDown.Value = 1

            AnalysisModeCombo.SelectedIndex = -1
            ReactionTypeCombo.SelectedIndex = -1

            SelectedTestDS.tparTests.AcceptChanges()

            ReadingModeCombo.SelectedValue = "MONO"

            If Not ReadingModeCombo.SelectedValue Is Nothing Then
                If String.Compare(ReadingModeCombo.SelectedValue.ToString, "MONO", False) = 0 Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                Else
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = True
                End If
            End If

            'DL 11/01/2012, Begin
            'FillSampleTypeCombo()
            'SampleTypeCombo.SelectedIndex = -1
            'bsSampleTypeText.Text = String.Empty
            SampleTypeCboEx.Items.Clear()
            'DL 10/01/2012. End

            FillSelectedSampleTypeCombo()
            SelectedSampleTypeCombo.SelectedIndex = -1

            If Not ReadingModeCombo.SelectedValue Is Nothing Then
                If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                Else
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = True
                End If
            End If

            'TR 02/12/2010 -Clear all the concentration and calibrator controls.
            ClearConcentrationsCalibValues()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ClearAllControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    'Private Sub ClearTests()
    '    Try
    '        'select general tab to start creating new test.
    '        TestDetailsTabs.SelectTab("GeneralTab")

    '        EditionMode = False
    '        NewTest = True

    '        Me.bsTestRefRanges.isEditing = False 'SG 06/09/2010

    '        EnableDisableControls(TestDetailsTabs.Controls, False)
    '        'TR 21/06/2010 'Validate there is a selection.
    '        If AnalysisModeCombo.SelectedIndex >= 0 Then
    '            CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())
    '        End If

    '        'Clear the structures.
    '        SelectedTestDS.tparTests.Clear()
    '        SelectedReagentsDS.tparReagents.Clear()
    '        SelectedTestSamplesDS.tparTestSamples.Clear()
    '        SelectedTestReagentsDS.tparTestReagents.Clear()
    '        SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Clear()
    '        SelectedTestRefRangesDS.tparTestRefRanges.Clear()

    '        'add the new row to the tests dataset
    '        Dim newTestRow As TestsDS.tparTestsRow
    '        newTestRow = SelectedTestDS.tparTests.NewtparTestsRow()
    '        Dim qTestDS As New List(Of TestsDS.tparTestsRow)
    '        Dim myTestid As Integer = 1
    '        'if there are row on the update test 
    '        If UpdateTestDS.tparTests.Rows.Count > 0 Then
    '            'search if there are any new test to set the next temporal TestID
    '            qTestDS = (From a In UpdateTestDS.tparTests _
    '                      Where a.NewTest Select a).ToList()
    '            'if found the increase by one to set the new test id
    '            If qTestDS.Count > 0 Then
    '                myTestid = qTestDS.First().TestID + 1
    '            End If
    '        End If

    '        newTestRow.TestID = myTestid
    '        newTestRow.NewTest = True

    '        FactorRadioButton.Checked = True 'TR 08/04/2010

    '        'add the row into the Test datatable.
    '        SelectedTestDS.tparTests.AddtparTestsRow(newTestRow)
    '        'Prepare the controls for a new test.
    '        TestDescriptionTextBox.Clear()
    '        TestDescriptionTextBox.Enabled = False
    '        TestNameTextBox.Clear()
    '        ShortNameTextBox.Clear()
    '        AbsCheckBox.Checked = False
    '        'TurbidimetryCheckBox.Checked = False 'TR 07/05/2011 -Do not implement turbidimetry

    '        SampleVolUpDown.ResetText()
    '        SampleVolUpDown.Value = SampleVolUpDown.Minimum + 1
    '        SampleVolUpDown.ResetText()

    '        'TR 02/01/2012 -The Washing Volume is Fixed to the maxvalue = 1,2
    '        'WashSolVolUpDown.Value = 1
    '        WashSolVolUpDown.Value = WashSolVolUpDown.Maximum
    '        'TR 02/01/2012 -END.

    '        VolR1UpDown.ResetText()
    '        VolR1UpDown.Value = VolR1UpDown.Minimum + 1
    '        VolR1UpDown.ResetText()

    '        VolR2UpDown.ResetText()
    '        VolR2UpDown.Value = VolR2UpDown.Minimum + 1
    '        VolR2UpDown.ResetText()

    '        FirstReadingCycleUpDown.ResetText()
    '        FirstReadingCycleUpDown.Value = FirstReadingCycleUpDown.Minimum
    '        FirstReadingCycleUpDown.ResetText()

    '        FirstReadingSecUpDown.ResetText()
    '        'FirstReadingSecUpDown.Value = FirstReadingSecUpDown.Minimum + 1
    '        FirstReadingSecUpDown.ResetText()

    '        PredilutionFactorTextBox.Clear()

    '        SecondReadingCycleUpDown.ResetText()
    '        'SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum + 1
    '        'SecondReadingCycleUpDown.Text = ""
    '        SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum
    '        SecondReadingCycleUpDown.ResetText()

    '        SecondReadingSecUpDown.ResetText()
    '        SecondReadingSecUpDown.Value = SecondReadingSecUpDown.Minimum
    '        'SecondReadingSecUpDown.Text = ""
    '        SecondReadingSecUpDown.ResetText()

    '        'RerunLimitMaxTextBox.Clear()
    '        'RerunLimitMinTextBox.Clear()

    '        'SG 11/06/2010
    '        RerunLowerLimitUpDown.ResetText()
    '        RerunUpperLimitUpDown.ResetText()
    '        'END SG 11/06/2010

    '        ProzonePercentUpDown.ResetText()
    '        ProzoneT1UpDown.ResetText()
    '        ProzoneT2UpDown.ResetText()
    '        'ProzonePercentUpDown.Enabled = False 'TR 05/05/2011
    '        'ProzoneT1UpDown.Enabled = False 'TR 05/05/2011
    '        'ProzoneT2UpDown.Enabled = False 'TR 05/05/2011

    '        SlopeAUpDown.ResetText()
    '        SlopeBUpDown.ResetText()

    '        SubstrateDepleUpDown.ResetText()

    '        UnitsCombo.SelectedIndex = -1
    '        RedPostDilutionFactorTextBox.Text = "1"
    '        IncPostDilutionFactorTextBox.Text = "1"
    '        CalibrationFactorTextBox.Clear()
    '        ReportsNameTextBox.Clear()
    '        'BlankAbsorbanceValueTextBox.Clear()
    '        BlankAbsorbanceUpDown.ResetText()
    '        BlankAbsorbanceUpDown.Value = BlankAbsorbanceUpDown.Minimum
    '        BlankAbsorbanceUpDown.ResetText()
    '        'KineticBlancValueTextBox.Clear()
    '        KineticBlankUpDown.ResetText()
    '        KineticBlankUpDown.Value = KineticBlankUpDown.Minimum + 1
    '        KineticBlankUpDown.ResetText()
    '        'LinearityLimitValueTextBox.Clear()
    '        LinearityUpDown.ResetText()
    '        LinearityUpDown.Value = LinearityUpDown.Minimum + 1
    '        LinearityUpDown.ResetText()

    '        'DetectionLimitTextBox.Clear()
    '        DetectionUpDown.ResetText()
    '        DetectionUpDown.Value = CDec(DetectionUpDown.Minimum + 0.9)
    '        DetectionUpDown.ResetText()

    '        BlankTypesCombo.SelectedIndex = -1
    '        BlankReplicatesUpDown.Value = 1
    '        DecimalsUpDown.Value = 0
    '        ReplicatesUpDown.Value = 1

    '        'DL 11/01/2012. Begin
    '        'SampleTypeCombo.SelectedIndex = -1
    '        'bsSampleTypeText.Text = String.Empty
    '        SampleTypeCboEx.Items.Clear()
    '        'DL 11/01/2012. End

    '        AnalysisModeCombo.SelectedIndex = 0
    '        ReactionTypeCombo.SelectedIndex = 0

    '        SelectedTestDS.tparTests.AcceptChanges()

    '        ReadingModeCombo.SelectedValue = "MONO"

    '        If Not ReadingModeCombo.SelectedValue Is Nothing Then
    '            If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
    '                ReferenceFilterCombo.SelectedIndex = -1
    '                ReferenceFilterCombo.Enabled = False
    '            Else
    '                ReferenceFilterCombo.SelectedIndex = -1
    '                ReferenceFilterCombo.Enabled = True
    '            End If
    '        End If

    '        'FillSampleTypeCombo()          'DL 11/01/2012
    '        FillSelectedSampleTypeCombo() ' dl 15/07/2010

    '        If Not ReadingModeCombo.SelectedValue Is Nothing Then
    '            If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
    '                ReferenceFilterCombo.SelectedIndex = -1
    '                ReferenceFilterCombo.Enabled = False
    '            Else
    '                ReferenceFilterCombo.SelectedIndex = -1
    '                ReferenceFilterCombo.Enabled = True
    '            End If
    '        End If
    '        SaveButton.Enabled = True
    '        'focus on the testname textbox
    '        TestNameTextBox.Select()

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ClearTests ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    ''' <summary>
    ''' Clear all the concentration and calibrators values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearConcentrationsCalibValues()
        Try
            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()

            'TR 17/11/2010 -Clear Calibrator info control
            CalibratorNameTextBox.Clear()
            CalibratorLotTextBox.Clear()
            CalibratorExpirationDate.Clear()
            'TR 17/11/2010 -END

            'TR 02/12/2010
            BsCalibNumberTextBox.Clear()
            CurveTypeCombo.SelectedIndex = -1
            XAxisCombo.SelectedIndex = -1
            YAxisCombo.SelectedIndex = -1
            IncreasingRadioButton.Checked = False
            DecreasingRadioButton.Checked = False
            'TR 02/12/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ClearConcentrationsGridValues " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill Alternative Calibrator
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 30/06/2010
    ''' Modified by: DL 11/01/2012
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub FillAlternativeCalibrator()
        Try
            Dim qSampleTypes As List(Of TestSamplesDS.tparTestSamplesRow)
            'order the result and filter the selected test..

            'DL 11/01/2012. Begin
            'qSampleTypes = (From a In SelectedTestSamplesDS.tparTestSamples _
            '                Where a.SampleType <> SampleTypeCombo.SelectedValue.ToString _
            '                And a.CalibratorType.ToUpper <> "ALTERNATIV" _
            '                Select a Order By a.DefaultSampleType Descending).ToList()
            qSampleTypes = (From a In SelectedTestSamplesDS.tparTestSamples _
                            Where String.Compare(a.SampleType, SampleTypeCboEx.Text.ToString, False) <> 0 _
                            And a.CalibratorType <> "ALTERNATIV" _
                            Select a Order By a.DefaultSampleType Descending).ToList()
            'And a.CalibratorType.ToUpper <> "ALTERNATIV" _
            'DL 11/01/2012. End

            AlternativeCalComboBox.DataSource = qSampleTypes
            AlternativeCalComboBox.DisplayMember = "SampleType"
            AlternativeCalComboBox.ValueMember = "SampleType"

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FillAlternativeCalibrator " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 07/10/10
    ''' Modified by: RH 17/11/2011 Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter.
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            SecondsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_[S]", currentLanguage) + ":"
            AnalysisModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_AnalysisMode", currentLanguage) + ":"
            BlankReplicatesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_BlankReplicates", currentLanguage) + ":"
            BlankAbsorbanceLimintLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_AbsLimit_Long", currentLanguage) + ":"
            bsBlankModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_BlankMode", currentLanguage)
            BlankTypesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_BlankTypes", currentLanguage) + ":"
            CalibrationModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_CalibrationMode", currentLanguage)
            bsCalibratorValuesCurveLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibValues&Curve", currentLanguage)
            CalibratorReplicatesLabels.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_CalibReplicates", currentLanguage) + ":"
            CycleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Cycle", currentLanguage) + ":"
            DecimalsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decimals", currentLanguage) + ":"
            DetectionLimitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_DetectionLimit", currentLanguage) + ":"
            FactorLimitsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FactorLimits", currentLanguage) + ":"
            IncreaseLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Increased", currentLanguage) + ":"
            KineticBlanKLimitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_KineticBlankLimit", currentLanguage) + ":"
            LinearityLimitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_LinearityLimit", currentLanguage) + ":"
            FilterMainLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_MainFilter", currentLanguage) + ":"
            MaxValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", currentLanguage) + ":"
            MinValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", currentLanguage) + ":"
            NameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", currentLanguage) + ":"
            ReplicatesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_#Replicates_Long", currentLanguage) + ":"
            NumCalibratorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NumCalibrators", currentLanguage) + ":"
            PredilutionModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_PredilutionMode", currentLanguage) + ":"
            ProzoneEffectLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ProzoneEffect", currentLanguage) + ":"
            ReactionTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ReactionType", currentLanguage) + ":"
            Reading1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Reading1", currentLanguage) + ":"
            Reading2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Reading2", currentLanguage) + ":"
            ReadingModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ReadingMode", currentLanguage) + ":"
            ReducedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Reduced", currentLanguage) + ":"
            FilterReferenceLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_RefFilter", currentLanguage) + ":"
            ReferenceRangesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Long", currentLanguage) + ":"
            RepetitionRangeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_RerunRange", currentLanguage) + ":"
            ReportNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ReportName", currentLanguage) + ":"
            VolSampleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", currentLanguage) + ":"
            SampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", currentLanguage) + ":"
            ShortNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ShortName", currentLanguage) + ":"
            SlopeFunctionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SlopeFunction", currentLanguage) + ":"
            SubstrateDepletionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SubstrateDep", currentLanguage) + ":"
            TestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage)
            BsLabel1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Tests", currentLanguage)
            UnitsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", currentLanguage) + ":"
            ValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Value", currentLanguage) + ":"
            VolR1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_R1Volume", currentLanguage) + ":"
            VolR2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_R2Volume", currentLanguage) + ":"
            VolWashLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_WashingVolume", currentLanguage) + ":"
            XaxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisX", currentLanguage) + ":"
            YaxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisY", currentLanguage) + ":"
            LBL_CalibratorName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", currentLanguage) + ":"
            LBL_Lot.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", currentLanguage) + ":"
            LBL_ExpDate_Full.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Full", currentLanguage) + ":"
            DiluentLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Diluent", currentLanguage) + ":" ' TR 18/01/2010

            'TR 11/04/2011 -Set the screen labels for QC TAB
            QCValuesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Quality_Control_Values", currentLanguage)
            QCActiveCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QCActive", currentLanguage)
            RulesToApplyGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RulesTo_Apply", currentLanguage)
            ControlValuesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Values", currentLanguage)
            CalculationModeGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CALCULATION_Mode", currentLanguage)
            SixSigmaValuesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SixSigma_Values", currentLanguage)
            ControlReplicatesNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Replicates", currentLanguage) + ":"
            RejectionCriteriaLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rejection_CriteriKSD", currentLanguage) + ":"
            StaticRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Static", currentLanguage)
            ManualRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Manual", currentLanguage)
            MinimumNumSeries.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinNum_Series", currentLanguage) + ":"

            ErrorAllowableLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Error_Allowable", currentLanguage) + ":"
            ControlsSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ControlSelection", currentLanguage)
            'TR 06/03/2012 -Control label.
            AddControlLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CREATE_NEW_CONTROLS", currentLanguage)

            'TR 11/04/2011 -END 
            GetScreenMoreLabels()
            GetScreenTooltip()
            GetScreenTestRefRanges()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 08/10/10
    ''' Modified by: RH 17/11/2011 Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter.
    ''' </remarks>
    Private Sub GetScreenMoreLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For CheckBox, RadioButtons...
            AbsCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", currentLanguage)
            AutoRepetitionCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_AutomaticReruns", currentLanguage)
            PredilutionFactorCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_PredilutionFactor", currentLanguage)
            TurbidimetryCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Turbidimetry", currentLanguage)

            CalibCurveInfoGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibrationCurve", currentLanguage)
            PostDilutionFactorGroupbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_PostDilutionFactor", currentLanguage)
            TimesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Times", currentLanguage)
            VolumesGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Volumes", currentLanguage)

            DecreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decreasing", currentLanguage)
            MultipleCalibRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_ExperimentalCalib", currentLanguage)
            FactorRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", currentLanguage)
            IncreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Increasing", currentLanguage)
            AlternativeCalibRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_AlternativeCalib", currentLanguage)
            'TR 11/04/2011
            CalibrationTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_CalibrationBlank", currentLanguage)
            GeneralTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_General", currentLanguage)
            OptionsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Options", currentLanguage)
            ProcedureTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Procedure", currentLanguage)
            QCTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QualityControl", currentLanguage)
            'TR 11/04/2011 -END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenMoreLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenMoreLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 08/10/10
    ''' Modified by: RH 17/11/2011 Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter.
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            'bsScreenToolTips.SetToolTip(ExitButton1, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(AddCalibratorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Tests_AddCalibValues", currentLanguage))
            bsScreenToolTips.SetToolTip(AddButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", currentLanguage))
            'bsScreenToolTips.SetToolTip(AddSampleTypeButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Tests_AddSampleType", currentLanguage))
            bsScreenToolTips.SetToolTip(DeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", currentLanguage))
            'bsScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            bsScreenToolTips.SetToolTip(CancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))

            bsScreenToolTips.SetToolTip(EditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(PrintTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))
            bsScreenToolTips.SetToolTip(DeleteSampleTypeButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_DelSampleType", currentLanguage))
            bsScreenToolTips.SetToolTip(SaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            'TR 11/04/2011
            bsScreenToolTips.SetToolTip(BsButton1, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CLIABV_Values", currentLanguage))
            'TR 11/04/2011 -END

            bsScreenToolTips.SetToolTip(CopyTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_COPY_TEST", currentLanguage))
            'TR 06/03/2012
            bsScreenToolTips.SetToolTip(AddControls, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ADD_CONTROLS", currentLanguage))

            bsScreenToolTips.SetToolTip(DeleteControlButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DELETE_CONTROLS", currentLanguage))
            'bsScreenToolTips.SetToolTip(BsCustomOrderButton , myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_COPY_TEST", currentLanguage)) 'AG 05/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: PG 08/10/10
    ''' Modified by: RH 17/11/2011 Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter.
    ''' </remarks>
    Private Sub GetScreenTestRefRanges()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For bsTestRefRanges
            bsTestRefRanges.TextForGenericRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Generic", currentLanguage)
            bsTestRefRanges.TextForNormalityLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Normality", currentLanguage) & ":"
            bsTestRefRanges.TextForMinValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", currentLanguage)
            bsTestRefRanges.TextForMaxValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", currentLanguage)

            bsTestRefRanges.TextForDetailedRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DetailedReferenceRange", currentLanguage)
            bsTestRefRanges.TextForGenderColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", currentLanguage)
            bsTestRefRanges.TextForAgeColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", currentLanguage)
            bsTestRefRanges.TextForFromColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", currentLanguage)
            bsTestRefRanges.TextForToColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", currentLanguage)

            bsTestRefRanges.ToolTipForDetailDeleteButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelReferenceRange", currentLanguage)

            'RH 17/11/2011
            bsTestRefRanges.TextForBorderLineLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_BorderLine", currentLanguage) & ":"

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenTestRefRanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTestRefRanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the Reference Ranges User control, passing to it all DB data it neededs to work
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/12/2010 - AG 12/01/2011 copied and adapted from OffSystems programming screen)
    ''' </remarks>
    Private Sub InitializeReferenceRangesControl()
        Try
            'Control will be shown using the "big" layout and used for Off System Tests
            bsTestRefRanges.SmallLayout = False
            bsTestRefRanges.TestType = "STD"

            'Load the necessary data to the Reference Ranges User Control
            Dim myAllFieldLimitsDS As FieldLimitsDS = GetLimits()
            Dim myGendersMasterDataDS As PreloadedMasterDataDS = GetGenders()
            Dim myAgeUnitsMasterDataDS As PreloadedMasterDataDS = GetAgeUnits()
            Dim myDetailedRangeSubTypesDS As PreloadedMasterDataDS = GetDetailedRangesSubTypes()
            Dim myAllMessagesDS As MessagesDS = GetMessages()

            bsTestRefRanges.LoadFrameworkData(myAllFieldLimitsDS, myGendersMasterDataDS, myAgeUnitsMasterDataDS, _
                                              myDetailedRangeSubTypesDS, myAllMessagesDS, SystemInfoManager.OSDecimalSeparator)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeReferenceRangesControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeReferenceRangesControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Indicate if a Sample type is used as alternative calibrator.
    ''' </summary>
    ''' <param name="pSampleType"></param>
    ''' <returns></returns>
    ''' <remarks>Created by: TR 23/05/2011</remarks>
    Private Function IsSampleTypeUseAsAlternativeCalib(ByVal pSampleType As String) As Boolean
        Dim isUsed As Boolean = False
        Try
            Dim qTestSampleList As New List(Of TestSamplesDS.tparTestSamplesRow)
            qTestSampleList = (From a In SelectedTestSamplesDS.tparTestSamples _
                               Where String.Compare(a.SampleTypeAlternative, pSampleType, False) = 0).ToList()

            If qTestSampleList.Count > 0 Then
                isUsed = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IsSampleTypeUseAsAlternativeCalib", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IsSampleTypeUseAsAlternativeCalib", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return isUsed
    End Function

    'Private Sub SetHelpProvider()
    '    Try

    '        BsHelpProvider1.HelpNamespace = GetHelpFilePath(HELP_FILE_TYPE.MANUAL, currentLanguage)
    '        'BsHelpProvider1.SetHelpNavigator(Me, HelpNavigator.Topic)
    '        BsHelpProvider1.SetHelpKeyword(Me, "Programacion")

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetHelpProvider ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub


    ''' <summary>
    ''' Validate Sample Type Selection
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 11/01/2012
    ''' </remarks>
    Private Sub ValidateSelection(ByVal pSampleType As String)
        Try
            Dim qSelectedSample As New List(Of TestSamplesDS.tparTestSamplesRow)

            'Search if the SampleType Exist on The SelectedSampleTypeAttribute
            qSelectedSample = (From b In SelectedTestSamplesDS.tparTestSamples _
                               Where b.SampleType = pSampleType _
                               Select b).ToList()

            If qSelectedSample.Count > 0 Then
                'if record found , validate is not last one.
                If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 1 Then
                    'TR 11/05/2011 -Set checked to true. 
                    SampleTypeCheckList.SetItemChecked(SampleTypeCheckList.SelectedIndex, True)

                    'TR 11/05/2011 -Commented
                    ''validate if it's the default.
                    'If qSelectedSample.First().DefaultSampleType Then
                    '    qSelectedSample.First().Delete()
                    '    SelectedSampleTypeAttribute.AcceptChanges()
                    '    'get the first position on the table to set the Defaulsample as true.
                    '    SelectedSampleTypeAttribute.tparTestSamples(0).DefaultSampleType = True
                    'Else
                    '    qSelectedSample.First().Delete()
                    'End If
                    'TR 11/05/2011 -END
                    '                    Me.AddedSampleType = SelectedSampleTypeAttribute.tparTestSamples(0).SampleType
                Else
                    SampleTypeCheckList.SetItemChecked(SampleTypeCheckList.SelectedIndex, True)

                End If

            Else
                'if do not exist the add it
                qSelectedSample = (From b In SelectedTestSamplesDS.tparTestSamples _
                                   Where b.DefaultSampleType _
                                   Select b).ToList()

                'Dim NewSelectedTestTable As New TestSamplesDS
                Dim NewSelectedTestRow As TestSamplesDS.tparTestSamplesRow
                Dim myTempSampleTable As New TestSamplesDS.tparTestSamplesDataTable

                NewSelectedTestRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow

                If qSelectedSample.Count > 0 Then
                    myTempSampleTable.ImportRow(qSelectedSample.First())
                    'change the sample type for the selected one going to last position.

                    myTempSampleTable(0).SampleType = pSampleType
                    myTempSampleTable(0).IsNew = True ' set the new status.

                    If Not NewTest Then
                        myTempSampleTable(0).DefaultSampleType = False
                    End If

                    myTempSampleTable(0).ItemIDDesc = GetSampleTypeDescription(pSampleType)
                    NewSelectedTestRow = myTempSampleTable(0)
                Else
                    'set the selected sample type.
                    NewSelectedTestRow.SampleType = SampleTypeCheckList.SelectedItem.ToString()
                    NewSelectedTestRow.TestID = SelectedTestDS.tparTests(0).TestID
                End If

                SelectedTestSamplesDS.tparTestSamples.ImportRow(NewSelectedTestRow)
                AddedSampleType = NewSelectedTestRow.SampleType
                'accept all the changes made to resultDataset.


                'Dim myDefaultSampleType As String

                'If Not SelectedTestSamplesDS Is Nothing AndAlso SelectedTestSamplesDS.tparTestSamples.Count > 0 Then
                Dim myDefaultSampleType As String = (From b In SelectedTestSamplesDS.tparTestSamples _
                                                     Where b.DefaultSampleType And b.SampleType <> "" _
                                                     Select b.SampleType).ToList.First
                'Else
                'myDefaultSampleType = AddedSampleType
                'End If

                ValidateRefRangesSelection(myDefaultSampleType) 'AG 11/02/2011

                'DL 10/01/2012. Begin
                For i As Integer = 0 To SampleTypeCheckList.Items.Count - 1
                    If SampleTypeCheckList.Items(i).ToString = pSampleType Then
                        SampleTypeCheckList.SetItemChecked(i, True)
                    End If
                Next i
                'DL 10/01/2012. End

            End If

            SelectedTestSamplesDS.tparTestSamples.AcceptChanges()

            ChangesMadeSampleType = True
            'bsSampleTypeText.Text = pSampleType
            SampleTypeCboEx.Items.Clear()
            SampleTypeCboEx.Items.Add(pSampleType)
            SampleTypeCboEx.SelectedIndex = 0

            SampleTypeCboAux.Items.Clear()
            SampleTypeCboAux.Items.Add(pSampleType)
            SampleTypeCboAux.SelectedIndex = 0

            SelectedSampleTypeCombo.SelectedValue = pSampleType
            'when is not new. Probar muchos nuevos samples type
            'If AddedSampleType = "" Then
            'SelectedSampleTypeCombo_SelectionChangeCommitted(Nothing, Nothing)
            'End If

            'SelectedSampleTypeCombo.Refresh()
            AnalysisModeCombo.Focus() 'dl 11/01/2012
            SampleTypeCheckList.Visible = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateSelection ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateSelection", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate Ref Ranges Selection
    ''' </summary>
    ''' <remarks>
    ''' AG 11/02/2011
    ''' </remarks>
    Private Sub ValidateRefRangesSelection(ByVal pDefaultSampleType As String)
        Try
            If Not SelectedTestRefRangesDS Is Nothing Then

                'AG 14/02/2011 - when create a new sample type if copy the default sample type ref ranges doesn't work!!
                'Si se intentan copiar los rangos del default sample type no sabemos que pasa pero no funciona!! Empieza a duplicar rangos de ref
                'SOLUCION: Al crear un sample type NO se heredan los ref ranges y punto !!!
                'Dim qSelectedRefRange As New List(Of TestRefRangesDS.tparTestRefRangesRow)

                ''Search if the SampleType Exist on The SelectedSampleTypeAttribute
                'qSelectedRefRange = (From b In SelectedRefRangesAttribute.tparTestRefRanges _
                '                                Where b.SampleType = pSampleType _
                '                                Select b).ToList()

                'If qSelectedRefRange.Count > 0 Then
                '    'Delete all ref ranges of the deleted sample type
                '    For Each row As TestRefRangesDS.tparTestRefRangesRow In qSelectedRefRange
                '        'row.Delete()
                '        row.BeginEdit()
                '        row.IsDeleted = True
                '        row.EndEdit()
                '    Next

                'Else
                '    'if do not exist the add it (change SampleType)
                '    qSelectedRefRange = (From b In SelectedRefRangesAttribute.tparTestRefRanges _
                '                                Where b.SampleType = pDefaultSampleType _
                '                                Select b).ToList()


                '    If qSelectedRefRange.Count > 0 Then
                '        Dim NewSelectedTable As New TestRefRangesDS
                '        Dim NewSelectedRow As TestRefRangesDS.tparTestRefRangesRow
                '        Dim tempTable As New TestRefRangesDS.tparTestRefRangesDataTable

                '        For Each row As TestRefRangesDS.tparTestRefRangesRow In qSelectedRefRange
                '            'AG 14/02/2011 - commented code from 11/02/2011
                '            NewSelectedRow = SelectedRefRangesAttribute.tparTestRefRanges.NewtparTestRefRangesRow
                '            If Not row.IsDeleted Then
                '                With NewSelectedRow
                '                    .BeginEdit()
                '                    .SampleType = pSampleType
                '                    .IsNew = True
                '                    .IsDeleted = False

                '                    .SetRangeIDNull()
                '                    If Not row.IsTestIDNull Then .TestID = row.TestID
                '                    If Not row.IsTestTypeNull Then .TestType = row.TestType
                '                    If Not row.IsRangeTypeNull Then .RangeType = row.RangeType
                '                    If Not row.IsGenderNull Then .Gender = row.Gender
                '                    If Not row.IsGenderDescNull Then .GenderDesc = row.GenderDesc
                '                    If Not row.IsAgeUnitNull Then .AgeUnit = row.AgeUnit
                '                    If Not row.IsAgeRangeFromNull Then .AgeRangeFrom = row.AgeRangeFrom
                '                    If Not row.IsAgeRangeToNull Then .AgeRangeTo = row.AgeRangeTo
                '                    If Not row.IsNormalLowerLimitNull Then .NormalLowerLimit = row.NormalLowerLimit
                '                    If Not row.IsNormalUpperLimitNull Then .NormalUpperLimit = row.NormalUpperLimit
                '                    If Not row.IsBorderLineLowerLimitNull Then .BorderLineLowerLimit = row.BorderLineLowerLimit
                '                    If Not row.IsBorderLineUpperLimitNull Then .BorderLineUpperLimit = row.BorderLineUpperLimit
                '                    If Not row.IsTS_UserNull Then .TS_User = row.TS_User
                '                    If Not row.IsTS_DateTimeNull Then .TS_DateTime = row.TS_DateTime
                '                    .EndEdit()
                '                End With

                '                SelectedRefRangesAttribute.tparTestRefRanges.AddtparTestRefRangesRow(NewSelectedRow)
                '            End If
                '            row.Delete() 'Delete the row for the old sample type
                '            'AG 14/02/2011

                '        Next
                '    End If

                'End If

                SelectedTestRefRangesDS.Clear()
                'accept all the changes made to resultDataset.
                SelectedTestRefRangesDS.AcceptChanges()
                ChangesMadeSampleType = True

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidateRefRangesSelection ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRangesSelection", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the limist and create a string with the values 
    ''' </summary>
    ''' <param name="FieldLimit "></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 17/05/2012
    ''' </remarks>
    Private Function GetStringLimitsValues(ByVal FieldLimit As FieldLimitsEnum) As String
        Dim myFielLimits As String = ""
        Try
            'TR 17/05/2012 -Get the min and max value to show on error message.
            Dim FieldLimitsDS As New FieldLimitsDS
            FieldLimitsDS = GetControlsLimits(FieldLimit)
            Dim minValue As Single = 0
            Dim maxValue As Single = 0
            If FieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                minValue = FieldLimitsDS.tfmwFieldLimits(0).MinValue
                maxValue = FieldLimitsDS.tfmwFieldLimits(0).MaxValue
            End If
            myFielLimits = "[" & minValue.ToString() & "-" & maxValue.ToString() & "]"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "GetStringLimitsValues " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetStringLimitsValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myFielLimits
    End Function

    Private Sub ReleaseElements()
        Try

            '--- Detach variable defined using WithEvents ---
            DeleteButton = Nothing
            EditButton = Nothing
            AddButton = Nothing
            PrintTestButton = Nothing
            TestDetailsTabs = Nothing
            ProcedureTab = Nothing
            CalibrationTab = Nothing
            MultipleCalibRadioButton = Nothing
            FactorRadioButton = Nothing
            AddCalibratorButton = Nothing
            XLabel = Nothing
            CalibrationFactorTextBox = Nothing
            BlankReplicatesLabel = Nothing
            CalibratorReplicatesLabels = Nothing
            AlternativeCalComboBox = Nothing
            TestListView = Nothing
            TestDescriptionTextBox = Nothing
            VolR1Label = Nothing
            VolSampleLabel = Nothing
            FilterReferenceLabel = Nothing
            FilterMainLabel = Nothing
            ReadingModeLabel = Nothing
            ReadingModeCombo = Nothing
            ReferenceFilterCombo = Nothing
            MainFilterCombo = Nothing
            WashSolVolUpDown = Nothing
            VolR1UpDown = Nothing
            VolR2UpDown = Nothing
            SampleVolUpDown = Nothing
            SamUnitLabel = Nothing
            VolR2Label = Nothing
            VolWashLabel = Nothing
            Reading1Label = Nothing
            WashUnitLabel = Nothing
            R1UnitLabel = Nothing
            VolumesGroupBox = Nothing
            R2UnitLabel = Nothing
            TimesGroupBox = Nothing
            Reading2Label = Nothing
            CycleLabel = Nothing
            FirstReadingSecUpDown = Nothing
            FirstReadingCycleUpDown = Nothing
            SecondsLabel = Nothing
            SecondReadingSecUpDown = Nothing
            Div1 = Nothing
            PredilutionModeLabel = Nothing
            PredilutionModeCombo = Nothing
            PredilutionFactorTextBox = Nothing
            PredilutionFactorCheckBox = Nothing
            AutoRepetitionCheckbox = Nothing
            SaveButton = Nothing
            BlankReplicatesUpDown = Nothing
            CalReplicatesUpDown = Nothing
            ExitButton = Nothing
            SelectedSampleTypeCombo = Nothing
            BlankTypesLabel = Nothing
            BlankTypesCombo = Nothing
            OptionsTab = Nothing
            PostDilutionFactorGroupbox = Nothing
            IncPostDilutionFactorTextBox = Nothing
            MultLabel = Nothing
            IncreaseLabel = Nothing
            DivReduce = Nothing
            RedPostDilutionFactorTextBox = Nothing
            ReducedLabel = Nothing
            DetectionUnitLabel = Nothing
            LinearityUnitLabel = Nothing
            ValueLabel = Nothing
            DetectionLimitLabel = Nothing
            LinearityLimitLabel = Nothing
            KineticBlanKLimitLabel = Nothing
            BlankAbsorbanceLimintLabel = Nothing
            SecondReadingCycleUpDown = Nothing
            BsErrorProvider1 = Nothing
            BlankAbsorbanceUpDown = Nothing
            LinearityUpDown = Nothing
            DetectionUpDown = Nothing
            MaxValueLabel = Nothing
            MinValueLabel = Nothing
            ProzonePercentUpDown = Nothing
            BLabel = Nothing
            ALabel = Nothing
            PercentLabel = Nothing
            SubstrateDepletionLabel = Nothing
            SlopeFunctionLabel = Nothing
            SlopeBUpDown = Nothing
            SlopeAUpDown = Nothing
            ProzoneT2UpDown = Nothing
            ProzoneT1UpDown = Nothing
            ProzoneEffectLabel = Nothing
            T2Label = Nothing
            T1Label = Nothing
            BsLabel1 = Nothing
            TestLabel = Nothing
            BsPanel1 = Nothing
            BsPanel2 = Nothing
            AlternativeCalibRadioButton = Nothing
            CalibrationModeLabel = Nothing
            bsCalibratorValuesCurveLabel = Nothing
            bsBlankModeLabel = Nothing
            FactorLimitsLabel = Nothing
            FactorUpperLimitUpDown = Nothing
            FactorLowerLimitUpDown = Nothing
            RepetitionRangeLabel = Nothing
            RerunUpperLimitUpDown = Nothing
            RerunLowerLimitUpDown = Nothing
            Separa1 = Nothing
            RepetitionRangeUnitLabel = Nothing
            SpecificCalibInfoGroupBox = Nothing
            BsCalibNumberTextBox = Nothing
            ConcentrationGridView = Nothing
            CalibNumber = Nothing
            Concentration = Nothing
            Factor = Nothing
            CalibCurveInfoGroupBox = Nothing
            YAxisCombo = Nothing
            YaxisLabel = Nothing
            XAxisCombo = Nothing
            XaxisLabel = Nothing
            CurveTypeCombo = Nothing
            DecreasingRadioButton = Nothing
            IncreasingRadioButton = Nothing
            NumCalibratorLabel = Nothing
            Separa2 = Nothing
            ReferenceRangesLabel = Nothing
            bsScreenToolTips = Nothing
            GeneralTab = Nothing
            AbsCheckBox = Nothing
            ReactionTypeCombo = Nothing
            ReplicatesUpDown = Nothing
            DecimalsUpDown = Nothing
            ReportsNameTextBox = Nothing
            TurbidimetryCheckBox = Nothing
            ReportNameLabel = Nothing
            ReplicatesLabel = Nothing
            ReactionTypeLabel = Nothing
            DecimalsLabel = Nothing
            UnitsLabel = Nothing
            UnitsCombo = Nothing
            DeleteSampleTypeButton = Nothing
            SampleTypeLabel = Nothing
            AnalysisModeCombo = Nothing
            ShortNameTextBox = Nothing
            AnalysisModeLabel = Nothing
            ShortNameLabel = Nothing
            TestNameTextBox = Nothing
            NameLabel = Nothing
            CancelButton = Nothing
            CalibratorLotTextBox = Nothing
            CalibratorNameTextBox = Nothing
            LBL_CalibratorName = Nothing
            LBL_ExpDate_Full = Nothing
            LBL_Lot = Nothing
            CalibratorExpirationDate = Nothing
            KineticBlankUpDown = Nothing
            SampleTypePlus2 = Nothing
            DiluentLabel = Nothing
            DiluentComboBox = Nothing
            BsGroupBox1 = Nothing
            BsGroupBox2 = Nothing
            QCTabPage = Nothing
            QCValuesLabel = Nothing
            BsLabel2 = Nothing
            ControlValuesGroupBox = Nothing
            QCActiveCheckBox = Nothing
            RejectionCriteriaLabel = Nothing
            QCRejectionCriteria = Nothing
            QCReplicNumberNumeric = Nothing
            ControlReplicatesNumberLabel = Nothing
            BsGroupBox3 = Nothing
            TestProgHelpProvider = Nothing
            BsLabel3 = Nothing
            CalculationModeGroupBox = Nothing
            QCMinNumSeries = Nothing
            MinimumNumSeries = Nothing
            StaticRadioButton = Nothing
            ManualRadioButton = Nothing
            SixSigmaValuesGroupBox = Nothing
            RulesToApplyGroupBox = Nothing
            BsCheckbox1 = Nothing
            BsCheckbox2 = Nothing
            BsCheckbox3 = Nothing
            BsCheckbox4 = Nothing
            BsCheckbox5 = Nothing
            BsCheckbox6 = Nothing
            SDLabel = Nothing
            BsGroupBox4 = Nothing
            BsLabel4 = Nothing
            x22CheckBox = Nothing
            s13CheckBox = Nothing
            s12CheckBox = Nothing
            BsGroupBox5 = Nothing
            BsCheckbox8 = Nothing
            BsCheckbox9 = Nothing
            x22 = Nothing
            x10CheckBox = Nothing
            s41CheckBox = Nothing
            r4sCheckBox = Nothing
            QCErrorAllowable = Nothing
            ErrorAllowableLabel = Nothing
            ControlsSelectionLabel = Nothing
            BsButton1 = Nothing
            UsedControlsGridView = Nothing
            AddControls = Nothing
            DeleteControlButton = Nothing
            BsDataGridView1 = Nothing
            BsNumericUpDown1 = Nothing
            BsNumericUpDown4 = Nothing
            TubesBySampleTypeDS1 = Nothing
            bsTestRefRanges = Nothing
            BsRadioButton1 = Nothing
            SubstrateDepleUpDown = Nothing
            BsHelpProvider1 = Nothing
            CopyTestButton = Nothing
            BsCustomOrderButton = Nothing 'AG 05/09/2014 - BA-1869
            SampleTypeCheckList = Nothing
            SampleTypeCboEx = Nothing
            SampleTypeCboAux = Nothing
            AddControlLabel = Nothing
            '------------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Framework Data Methods"

    ''' <summary>
    ''' Gets all the field limits to pass to the Ref Ranges Control
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SG 01/09/2010
    ''' </remarks>
    Private Function GetLimits() As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()

            myGlobalDataTO = myFieldLimitsDelegate.GetAllList(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                'Error getting the defined Field Limits; shown it
                ShowMessage(Me.Name & ".GetLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myFieldLimitsDS
    End Function

    ''' <summary>
    ''' Returns the FixedItemDesc of Gender constants
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 01/09/2010</remarks>
    Private Function GetGenders() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SEX_LIST)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of available Genders; shown it
                ShowMessage(Name & ".GetGenders", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetGenders ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns the FixedItemDesc of AgeUnits constants
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 01/09/2010</remarks>
    Private Function GetAgeUnits() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.AGE_UNITS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of defined Age Units; shown it
                ShowMessage(Me.Name & ".GetAgeUnits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAgeUnits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAgeUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Gets all defined SubTypes for Detailed Reference Ranges to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 14/12/2010 (as copy of GetGenders)
    ''' </remarks>
    Private Function GetDetailedRangesSubTypes() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.RANGE_SUBTYPES)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of available Detailed Reference Ranges Subtypes; shown it
                ShowMessage(Name & ".GetDetailedRangesSubTypes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetDetailedRangesSubTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetDetailedRangesSubTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Get Volume Predilution
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 08/03/11</remarks>
    Private Function GetVolumePredilution() As Single
        Dim myVolume As Single = 150
        Try
            'SGM 08/03/11 Get from SWParameters table
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            'TR 14/03/2011 -Set the Analyzer Model
            myGlobalDataTO = myParams.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.VOLUME_PREDILUTION.ToString, AnalyzerModelAttribute)
            If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                myVolume = CSng(myGlobalDataTO.SetDatos)
            Else
                myVolume = 150
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myVolume
    End Function

    '''' <summary>
    '''' gets all Sample Types
    '''' </summary>
    '''' <remarks>
    '''' Modified by: SG 01/09/2010
    '''' </remarks>
    'Private Function GetSampleTypes() As MasterDataDS
    '    Try
    '        'Get the list of existing Sample Types
    '        Dim myGlobalDataTo As New GlobalDataTO
    '        Dim masterDataConfig As New MasterDataDelegate
    '        myGlobalDataTo = masterDataConfig.GetList(Nothing, MasterDataEnum.SAMPLE_TYPES.ToString)

    '        If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
    '            Dim masterDataDS As New MasterDataDS
    '            masterDataDS = DirectCast(myGlobalDataTo.SetDatos, MasterDataDS)

    '            Return masterDataDS
    '        Else
    '            'Error getting the list of defined Sample Types, show the error message
    '            ShowMessage("Error", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
    '            Return Nothing
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTypes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '        Return Nothing
    '    End Try
    'End Function

    ''' <summary>
    ''' Get all messages in the current application Language
    ''' </summary>
    ''' <returns>A String value containing the Message Text</returns>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Public Function GetMessages() As MessagesDS
        'Dim textMessage As String = String.Empty
        Try
            Dim myMessageDelegate As New MessageDelegate()
            Dim myGlobalDataTO As New GlobalDataTO()

            myGlobalDataTO = myMessageDelegate.GetAllMessageDescription(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As New MessagesDS
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                Return myMessagesDS
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "GetMessages ", EventLogEntryType.Error, False)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' gets the description for a spicified measure unit
    ''' </summary>
    ''' <param name="pID"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SG 05/09/2010</remarks>
    Private Function GetUnitDefinition(ByVal pID As String) As String
        Dim returnValue As String = ""

        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMasteDataDS As New MasterDataDS()
            Dim myMasterDataDelegate As New MasterDataDelegate()

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "TEST_UNITS")

            If Not myGlobalDataTO.HasError Then
                myMasteDataDS = CType(myGlobalDataTO.SetDatos, MasterDataDS)
                Dim qTestUnits As List(Of MasterDataDS.tcfgMasterDataRow)
                'order the result.
                qTestUnits = (From a In myMasteDataDS.tcfgMasterData _
                             Select a Where a.ItemID = pID).ToList()
                If qTestUnits.Count > 0 Then
                    returnValue = qTestUnits(0).FixedItemDesc
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetUnitDefinition ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return returnValue
    End Function


    ''' <summary>
    ''' Get the test value by the test shortname
    ''' </summary>
    ''' <param name="pShortName"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 16/11/2010
    ''' </remarks>
    Public Function GetTestByShortName(ByVal pShortName As String) As TestsDS
        Dim myResult As New TestsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestDelegate As New TestsDelegate

            myGlobalDataTO = myTestDelegate.ReadByShortName(Nothing, pShortName)

            If myGlobalDataTO.HasError Then
                'Error getting the list of defined Sample Types, show the error message
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            Else
                myResult = CType(myGlobalDataTO.SetDatos, TestsDS)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetTestByShortName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myResult

    End Function

    ''' <summary>
    ''' Validate if the short name is in use or exist.
    ''' </summary>
    ''' <param name="pShortName"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 16/11/2010
    ''' </remarks>
    Public Function ShortNameExist(ByVal pShortName As String) As Boolean
        Dim myResult As Boolean = False
        Try
            Dim mytestsDS As New TestsDS
            If pShortName <> "" Then
                mytestsDS = GetTestByShortName(pShortName.TrimStart().TrimEnd())
                If mytestsDS.tparTests.Rows.Count > 0 Then
                    'TR 10/12/2010 -validate the test id 
                    If mytestsDS.tparTests(0).TestID <> SelectedTestDS.tparTests(0).TestID Then
                        myResult = True
                    End If

                    If Not myResult AndAlso CopyTestData AndAlso EditionMode Then
                        myResult = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ShortNameExist ", EventLogEntryType.Error, _
                              GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

#End Region

#Region "Events"

    Private Sub UsedControlsGridView_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.RowEnter
        Try
            If e.RowIndex >= 0 Then
                If EditionMode AndAlso Not UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value Is DBNull.Value _
                AndAlso Not UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
                    ControIDSel = UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value.ToString()
                Else
                    ControIDSel = ""
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_RowEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRangesSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Sample Type Combo Mouse down
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 17/01/2012
    ''' </remarks>
    Private Sub SampleTypeCboEx_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles SampleTypeCboEx.MouseDown

        Try
            If SampleTypeCheckList.SelectedItems.Count > 0 Then
                RemoveHandler SampleTypeCheckList.SelectedValueChanged, AddressOf SampleTypeCheckList_SelectedValueChanged
                SampleTypeCheckList.SelectedItems.Clear()
                AddHandler SampleTypeCheckList.SelectedValueChanged, AddressOf SampleTypeCheckList_SelectedValueChanged
            End If

            If EditionMode Then

                If Not IsSampleTypeChanged Then
                    SampleTypeCheckList.Visible = True
                    SampleTypeCheckList.Focus()
                Else
                    SampleTypeCheckList.Visible = False
                    'ValidateAllTabs()
                    'If Not ValidationError Then
                    '    SampleTypeCheckList.Visible = True
                    '    SampleTypeCheckList.Focus()
                    'Else
                    '    SampleTypeCheckList.Visible = False
                    'End If
                End If

                'If SampleTypeCheckList.Visible Then
                '    SampleTypeCheckList.Visible = False
                'Else
                '    SampleTypeCheckList.Visible = True
                '    SampleTypeCheckList.Focus()
                'End If

            Else
                SampleTypeCheckList.Visible = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SampleTypeCboEx_MouseDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SampleTypeCboEx_MouseDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Hide sample type panel when lost focus
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 11/01/2012
    ''' </remarks>
    Private Sub Control_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestNameTextBox.Enter, _
                                                                                                  ShortNameTextBox.Enter, _
                                                                                                  ReportsNameTextBox.Enter, _
                                                                                                  DecimalsUpDown.Enter, _
                                                                                                  ReplicatesUpDown.Enter, _
                                                                                                  GeneralTab.MouseDown, _
                                                                                                  TestDetailsTabs.MouseDown, _
                                                                                                  AnalysisModeCombo.Enter, _
                                                                                                  UnitsCombo.Enter, _
                                                                                                  ReactionTypeCombo.Enter

        HideSampleTypePanel()

    End Sub


    ''' <summary>
    ''' Value changed
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/01/2012
    ''' </remarks>
    Private Sub SampleTypeCheckList_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SampleTypeCheckList.SelectedValueChanged
        Try
            'TR 25/01/2012 -This functionality is not used any more NOW IS USE THE COPY BUTTON.
            'TR 21/07/2011. Begin. Validate tests have diferent name
            'If TestListView.SelectedItems.Count = 1 AndAlso TestListView.SelectedItems(0).Text <> TestNameTextBox.Text Then
            '    NewTest = True
            '    CopySelectedTestValues()        'Update Elements like TestCalibrators and Calibrators values.
            'End If
            ''TR 21/07/2011. End
            'TR 25/01/2012 -END.

            Dim NewSampleType As String = ""    'TR 11/04/2011. Declare variable here to make visible.

            If NewTest Then
                CurrentSampleType = ""
            Else
                If SelectedSampleTypeCombo.SelectedValue IsNot Nothing Then
                    CurrentSampleType = SelectedSampleTypeCombo.SelectedValue.ToString()
                Else
                    CurrentSampleType = ""
                End If
            End If

            Dim update As Boolean = False 'TR 21/1/2010 -Initialize value with false.
            Dim myTestSampleRow As TestSamplesDS.tparTestSamplesRow = SelectedTestSamplesDS.tparTestSamples.NewtparTestSamplesRow

            If NewTest Then
                myTestSampleRow.DefaultSampleType = True
                'add the new test sample
                SelectedTestSamplesDS.tparTestSamples.Rows.Add(myTestSampleRow)
                SelectedTestSamplesDS.tparTestSamples.AcceptChanges()

                'TESTSAMPLES
                'dl 26/01/2012
                'SelectedSampleTypeCombo.DataSource = SelectedTestSamplesDS.tparTestSamples
                'SelectedSampleTypeCombo.DisplayMember = "ItemIDDesc"
                'SelectedSampleTypeCombo.ValueMember = "SampleType"
                'dl 26/01/2012
                update = True
            Else
                'TR 21/12/2010. Begin
                'update = SaveTestLocal(NewTest) 'Save localy before adding a new test() 'TR 21/12/2010 -Commented
                If (ChangesMade OrElse bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                    Dim tmpNewTest As Boolean = NewTest 'AG 12/01/2011 - The newtest variable value is reset in SavePending... but i need it into SaveChanges when new sample type is added
                    Dim myresultDialog As DialogResult = SavePendingWarningMessage(True)
                    If myresultDialog = Windows.Forms.DialogResult.Yes Then
                        NewTest = tmpNewTest 'AG 12/01/2011 - The newtest variable value is reset in SavePending... but i need it into SaveChanges when new sample type is added
                        If SaveChanges(True) Then
                            update = True
                            If Not EditionMode Then EditionMode = True 'AG 11/01/2011
                            If Not ReadyToSave Then ReadyToSave = True 'AG 11/01/2011
                        End If
                    End If
                Else
                    update = True
                End If
                'TR 21/12/2010. End
            End If
            'SG 23/07/2010. End

            If update Then
                ''send the test SamplesTypes 
                'mySampleTypeDialog.SelectedSampleType = SelectedTestSamplesDS
                SelectedTestRefRangesDS.Clear() 'Delete the old sample type ref ranges - No se heredan los ref ranges al crear sampletype pq no funciona codigo (no se sabe pq pero empieza a duplicar ref ranges)
                'mySampleTypeDialog.SelectedRefRanges = SelectedTestRefRangesDS
                'mySampleTypeDialog.ShowDialog()

                If SampleTypeCheckList.SelectedItems.Count > 0 Then
                    ValidateSelection(SampleTypeCheckList.SelectedItem.ToString())
                End If

                If NewTest Then
                    SelectedTestSamplesDS.tparTestSamples.Rows.Remove(myTestSampleRow)
                    SelectedTestSamplesDS.tparTestSamples.AcceptChanges()
                End If

                If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                    If Not AddedSampleType Is Nothing AndAlso Not AddedSampleType Is String.Empty Then
                        'If mySampleTypeDialog.AddedSampleType <> "" Then
                        SelectedSampleTypeCombo.SelectedValue = AddedSampleType 'GetSampleTypeDescription(AddedSampleType)
                        'SelectedSampleTypeCombo.SelectedValue = mySampleTypeDialog.AddedSampleType
                        'CurrentSampleType = mySampleTypeDialog.AddedSampleType 'TR 21/12/2010
                        CurrentSampleType = AddedSampleType 'TR 21/12/2010

                        ' DL 15/07/2010
                        TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & "- " & _
                                                      SelectedTestSamplesDS.tparTestSamples(SelectedTestSamplesDS.tparTestSamples.Rows.Count - 1).SampleType
                    Else
                        'DL 09/01/2012
                        'SelectedSampleTypeCombo.SelectedValue = SelectedTestSamplesDS.tparTestSamples(SelectedTestSamplesDS.tparTestSamples.Rows.Count - 1).SampleType ' dl 09/01/2012
                        'CurrentSampleType = SelectedTestSamplesDS.tparTestSamples(SelectedTestSamplesDS.tparTestSamples.Rows.Count - 1).SampleType ' dl 09/01/2012
                        CurrentSampleType = SampleTypeCboEx.Text
                        If Not ChangesMade Then ChangesMade = ChangesMadeSampleType

                        'TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & "- " & _
                        '                              SelectedTestSamplesDS.tparTestSamples(SelectedTestSamplesDS.tparTestSamples.Rows.Count - 1).SampleType
                        TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & "- " & CurrentSampleType

                        If Not SelectedSampleTypeCombo.SelectedValue.ToString = CurrentSampleType Then
                            SelectedSampleTypeCombo_SelectionChangeCommitted(Nothing, Nothing)
                        Else
                            If NewTest Then
                                BindControls(SelectedTestDS.tparTests(0).TestID, CurrentSampleType, False)
                            ElseIf TestListView.SelectedItems.Count > 0 And EditionMode Then 'in case is not new then select from the list.
                                BindControls(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType, False)
                            ElseIf TestListView.SelectedItems.Count > 0 Then
                                BindControls(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType)
                            End If
                        End If

                        'DL 09/01/2012

                        Exit Try
                    End If

                    Dim qTestSamples As New List(Of TestSamplesDS.tparTestSamplesRow)
                    'filter testsamples bu the sample type
                    qTestSamples = (From a In SelectedTestSamplesDS.tparTestSamples Select a).ToList()

                    If qTestSamples.Count > 0 Then
                        'DL 10/01/2012
                        'SampleTypeCombo.DataSource = qTestSamples
                        'SampleTypeCombo.DisplayMember = "SampleType"
                        'SampleTypeCombo.ValueMember = "SampleType"
                        'SampleTypeCombo.SelectedValue = SelectedSampleTypeCombo.SelectedValue.ToString() ' qTestSamples(0).SampleType
                        'bsSampleTypeText.Text = SelectedSampleTypeCombo.SelectedValue.ToString()
                        SampleTypeCboEx.Items.Clear()
                        SampleTypeCboEx.Items.Add(SelectedSampleTypeCombo.SelectedValue.ToString())
                        SampleTypeCboEx.SelectedIndex = 0

                        SampleTypeCboAux.Items.Clear()
                        SampleTypeCboAux.Items.Add(SelectedSampleTypeCombo.SelectedValue.ToString())
                        SampleTypeCboAux.SelectedIndex = 0

                        'DL 10/01/2012
                        'SelectedSampleTypeCombo.SelectedValue = SelectedSampleTypeCombo.SelectedValue.ToString()

                        'RH 14/05/2012
                        IsSampleTypeChanged = True
                        SelectedSampleTypeCombo.Enabled = Not IsSampleTypeChanged
                    End If

                    If NewTest Then
                        'DL 10/01/2012
                        SelectedSampleTypeCombo.SelectedValue = SampleTypeCboEx.Text 'SampleTypeCombo.Text
                        SelectedTestSamplesDS.tparTestSamples.Clear()
                        CurrentSampleType = SampleTypeCboEx.Text 'SampleTypeCombo.Text
                        'DL 10/01/2012

                        'AG 13/01/2011
                        bsTestRefRanges.SampleType = CurrentSampleType 'Inform the sampletype
                        ValidateAllTabs()
                        If ValidationError Then Exit Sub

                    End If

                    'SG 20/07/2010
                    'Dim NewSampleType As String = SelectedSampleTypeCombo.SelectedValue.ToString() 'TR 11/04/2011 -Commented
                    'TR 11/04/2001 -set the value
                    NewSampleType = SelectedSampleTypeCombo.SelectedValue.ToString()

                    If Not NewTest Then
                        If Not Me.DesignMode Then
                            'AG 14/02/2011
                            Dim RangesList As List(Of TestRefRangesDS.tparTestRefRangesRow) = _
                                                   (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                                                   Where a.SampleType = CurrentSampleType Select a).ToList
                            bsTestRefRanges.ClearReferenceRanges()
                            bsTestRefRanges.SampleType = CurrentSampleType
                            bsTestRefRanges.ActiveRangeType = ""
                            If RangesList.Count > 0 Then
                                bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS
                                bsTestRefRanges.ChangesMade = True
                            End If
                            bsTestRefRanges.LoadReferenceRanges()
                            bsTestRefRanges.isEditing = EditionMode
                            'AG 14/02/2011
                        End If

                        ''TR 09/11/2010 -Set the Alternative calibrator as calibrator for added test.
                        AlternativeCalibRadioButton.Checked = True
                        ClearConcentrationsCalibValues()
                        ''TR 09/11/2010 -END
                    End If

                    UpdateTestDatasets(SelectedTestDS.tparTests(0).TestID, NewSampleType)

                    'TR 1/12/2010 -Set the changes to true 
                    'TR 26/11/2010 
                    If Not ChangesMade Then
                        ' Set the changes depending on the recived value from the sample dialog box
                        ChangesMade = ChangesMadeSampleType
                    End If
                    'TR 26/11/2010 -END 
                End If
                'End Using

                'TR 11/04/2011 -Update QCStructures
                UpdateTestSampleMultiRules(SelectedTestDS.tparTests(0).TestID, NewSampleType)
                'Set the new sample type to the Test Controls date structure
                For Each testControlRow As TestControlsDS.tparTestControlsRow In SelectedTestControlDS.tparTestControls.Rows
                    testControlRow.SampleType = NewSampleType
                Next

            End If

            ShowPlusSampleTypeIcon()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SampleTypeCheckList_SelectedValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SampleTypeCheckList_SelectedValueChanged", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    'dl 11/01/2012
    Private Sub SampleTypeCboEx_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles SampleTypeCboEx.LostFocus
        SampleTypeCheckList.Visible = True
    End Sub

    Private Sub AddCalibratorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddCalibratorButton.Click
        'TR 21/06/2010
        Try
            'TR 01/03/2011
            'Validate the required data before calling the Calibrator windows.
            If TestNameTextBox.Text.Trim() = "" Then
                'Show message indicating Test name required
                BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.TESTNAME_REQUIRED.ToString, currentLanguage))

                Exit Try
                'DL 11/01/2012. Begin
                'ElseIf SampleTypeCombo.SelectedIndex < 0 Then
            ElseIf SampleTypeCboEx.Text Is String.Empty Then
                'DL 11/01/2012. End
                'Show message indicating Sample Type is required
                BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.SAMPLETYPE_REQUIRED.ToString(), currentLanguage))
                Exit Try
            End If

            'RH 19/10/2010 Introduce the Using statement
            Using myMultiCalibProgrammingForm As New IProgCalibrator()

                If SelectedTestDS.tparTests.Rows.Count > 0 Then
                    myMultiCalibProgrammingForm.StartPosition = FormStartPosition.CenterParent
                    myMultiCalibProgrammingForm.TestIdentification = SelectedTestDS.tparTests(0).TestID
                    myMultiCalibProgrammingForm.TestSampleType = SelectedSampleTypeCombo.SelectedValue.ToString()
                    myMultiCalibProgrammingForm.TestName = TestNameTextBox.Text
                    myMultiCalibProgrammingForm.IsTestParammeterWindows = True
                    myMultiCalibProgrammingForm.DecimalsAllow = CInt(DecimalsUpDown.Value)

                    'TR 02/08/2010
                    myMultiCalibProgrammingForm.ResultCalibratorsDS = UpdatedCalibratorsDS

                    UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                    If SelectedTestSampleCalibratorDS.tparTestCalibrators.Count > 0 Then
                        UpdatedTestCalibratorDS.tparTestCalibrators.ImportRow(SelectedTestSampleCalibratorDS.tparTestCalibrators(0))
                    End If
                    myMultiCalibProgrammingForm.ResultTestCalibrator = UpdatedTestCalibratorDS

                    myMultiCalibProgrammingForm.ResultTestCalibratorsValue = SelectedTestCalibratorValuesDS
                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                    SelectedTestCalibratorValuesDS.tparTestCalibratorValues.CopyToDataTable(UpdatedTestCalibratorValuesDS.tparTestCalibratorValues, _
                                                                                                                            LoadOption.OverwriteChanges)
                    'TR 02/08/2010 -End
                    'TR 21/06/2010
                    'myMultiCalibProgrammingForm.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedToolWindow
                    'TR 21/06/2010 -END
                    'myMultiCalibProgrammingForm.StartPosition = FormStartPosition.CenterParent

                    'RH 20/12/2010
                    myMultiCalibProgrammingForm.Tag = "Put something here before showing, so the form will execute its normal Close()"
                    'RH 20/12/2010 -END

                    'RH 11/05/2012 This is the right value
                    'It is the used througout the application
                    'It enables the aplication to show it's icon when the user press Alt + Tab. The other one not.
                    myMultiCalibProgrammingForm.FormBorderStyle = FormBorderStyle.FixedDialog
                    'RH 11/05/2012

                    myMultiCalibProgrammingForm.AnalyzerID = AnalyzerIDAttribute
                    myMultiCalibProgrammingForm.WorkSessionID = WorkSessionIDAttribute

                    myMultiCalibProgrammingForm.ShowDialog()
                    'TR 15/11/2010 -Validate if there was any changes on calibrators.
                    If myMultiCalibProgrammingForm.ChangesMade Then

                        UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        SelectedTestCalibratorValuesDS.tparTestCalibratorValues.CopyToDataTable(UpdatedTestCalibratorValuesDS.tparTestCalibratorValues, LoadOption.OverwriteChanges)

                        ' TR 01/10/2013 Do not validate  if change made on test form if there was a change on Calibrator form the set the calibrator changes to true.
                        'If Not ChangesMade Then
                        'ChangesMade = True
                        CalibratorChanges = True  'TR 03/12/2010 Indicate there was a change on the calibrator.
                        'End If
                    End If
                    'TR 15/11/2010 -END.
                End If

                'TR 17/11/2010 -Save on the local tables 
                'AG 03/12/2010 - Save test if changes in calibration
                If CalibratorChanges Then 'If ChangesMade Then
                    SaveTestLocal(NewTest, CurrentSampleType)
                End If
                'TR 17/11/2010 -END

                BindCalibrationCurveControls(SelectedTestDS.tparTests(0).TestID, SelectedSampleTypeCombo.SelectedValue.ToString())
                'Fill the data for selected test. 
                FillConcentrationValuesList(SelectedTestDS.tparTests(0).TestID, SelectedSampleTypeCombo.SelectedValue.ToString())

                'TR 1/12/2010 -Validate if there was any changes on calibrators.
                If myMultiCalibProgrammingForm.ChangesMade Then
                    If Not ChangesMade Then
                        ChangesMade = True
                    End If
                End If
                'TR 1/12/2010 -END.
            End Using

            'TR 08/02/2012 -After programming the calibrators validate tab.
            ValidateCalibrationTabs(False)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " AddCalibratorButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ProgTest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If Not Me.DesignMode Then
                If e.KeyCode = Keys.Escape Then
                    If CancelButton.Enabled Then
                        CancelButton.PerformClick()
                    Else
                        'RH 17/12/2010
                        IAx00MainMDI.OpenMonitorForm(Me)
                    End If
                ElseIf e.KeyCode = Keys.F1 Then ' TR 07/11/2011 -Search the Help File and  Chapter
                    'Help.ShowHelp(Me, GetHelpFilePath(HELP_FILE_TYPE.MANUAL, currentLanguage), GetScreenChapter(Me.Name))
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ProgTest_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub TestPrograming_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If (Not Me.DesignMode) Then
                'AG 10/03/10
                'WorkSessionIDAttribute = ""     '"", "2010100310"
                WorkSessionStatusAttribute = "PENDING" '"", "PENDING", "RUNNING", "STANDBY", "SAMPLINGSTOP", "FINISHED", "ABORTED"
                'isCurrentTestInUse = False
                'END AG 10/03/10

                'TR 29/03/2012 -Get the current level
                Dim MyGlobalBase As New GlobalBase
                CurrentUserLevel = MyGlobalBase.GetSessionInfo.UserLevel
                'TR 29/03/2012 -END

                bsTestRefRanges.UserLevel = CurrentUserLevel 'JV 23/01/2014 #1013

                InitializeScreen()
                Application.DoEvents() 'RH 15/12/2010

                EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
                Application.DoEvents() 'RH 15/12/2010

                LoadScreenStatus(WorkSessionIDAttribute, WorkSessionStatusAttribute, isCurrentTestInUse) 'AG 10/03/10
                Application.DoEvents() 'RH 15/12/2010

                If TestListView.Items.Count < 1 Then
                    'SetEmptyValues() ' dl 14/07/2010
                    ClearAllControls() 'TR 02/12/2010 -Use the clear control instead setEmpty values because it has the bussiness rules.
                Else
                    'TR 02/12/2010 -Bind controls with the firts element on the list.
                    TestListView.Items(0).Selected = True
                    BindControls(CType(TestListView.Items(0).Name, Integer))
                    If isCurrentTestInUse Then
                        EditButton.Enabled = False
                        DeleteButton.Enabled = False
                    End If

                End If
                GetAllSampleTypes() 'TR 12/05/2011
                'RH 15/12/2010

                'DL 11/01/2012. Begin
                FillSampleTypeList()
                SampleTypeCboEx.DroppedDown = False
                'AddHandler SampleTypeCboEx.Click, AddressOf SampleTypeCboEx_Click

                'SampleTypeCboEx.Enabled = True

                'SampleTypeCboEx.BackColor = Color.White
                'SampleTypeCboEx.DroppedDown = False


                ResetBorder()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " TestPrograming_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub TestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles TestListView.DoubleClick
        'TR 28/05/2010 Validate if edition mode is enable before sending to enable again.
        If Not EditionMode Then EnableEdition()
    End Sub

    ' DL 14/07/2010 7.13, 7.14, 7.15
    Private Sub SetEmptyValues()
        Try
            ' General
            TestNameTextBox.Text = ""
            ShortNameTextBox.Text = ""
            'DL 11/01/2012. Begin
            'SampleTypeCombo.Text = "" ' .SelectedIndex = -1
            'bsSampleTypeText.Text = ""
            SampleTypeCboEx.Items.Clear()
            'DL 11/01/2012. End
            'AnalysisModeCombo.Text = "" '.SelectedIndex = -1
            UnitsCombo.Text = "" 'SelectedIndex = -1
            DecimalsUpDown.ResetText()
            ReplicatesUpDown.ResetText()
            ReactionTypeCombo.Text = "" '.SelectedIndex = -1
            ReportsNameTextBox.Text = ""
            AbsCheckBox.Checked = False
            'TurbidimetryCheckBox.Checked = False 'TR 07/05/2011 -Do not implement turbidimetry

            'procedure
            ReadingModeCombo.Text = "" '.SelectedIndex = -1
            MainFilterCombo.SelectedIndex = -1
            ReferenceFilterCombo.Text = "" '.SelectedIndex = -1
            SampleVolUpDown.ResetText()
            WashSolVolUpDown.ResetText()
            VolR1UpDown.ResetText()
            VolR2UpDown.ResetText()
            FirstReadingSecUpDown.ResetText()
            SecondReadingSecUpDown.ResetText()
            FirstReadingCycleUpDown.ResetText()
            SecondReadingCycleUpDown.ResetText()
            PredilutionFactorCheckBox.Checked = False
            PredilutionModeCombo.SelectedIndex = -1
            PredilutionFactorTextBox.Text = ""
            AutoRepetitionCheckbox.Checked = False
            RedPostDilutionFactorTextBox.Text = ""
            IncPostDilutionFactorTextBox.Text = ""

            ' Calibrations and blanks
            BlankTypesCombo.SelectedIndex = -1
            BlankReplicatesUpDown.ResetText()
            CalibrationFactorTextBox.Text = ""
            FactorRadioButton.Checked = False
            MultipleCalibRadioButton.Checked = False
            AlternativeCalibRadioButton.Checked = False
            AlternativeCalComboBox.SelectedIndex = -1
            CalReplicatesUpDown.ResetText()
            BsCalibNumberTextBox.Text = ""
            ConcentrationGridView.DataSource = Nothing
            ConcentrationGridView.DataMember = Nothing
            IncreasingRadioButton.Checked = False
            DecreasingRadioButton.Checked = False
            XAxisCombo.SelectedIndex = -1
            YAxisCombo.SelectedIndex = -1

            'Options
            BlankAbsorbanceUpDown.ResetText()
            KineticBlankUpDown.ResetText()
            LinearityUpDown.ResetText()
            DetectionUpDown.ResetText()
            FactorLowerLimitUpDown.ResetText()
            FactorUpperLimitUpDown.ResetText() 'TR 19/11/2010 -Reset the factor upper value
            RerunUpperLimitUpDown.ResetText()
            ProzonePercentUpDown.ResetText()
            SlopeAUpDown.ResetText()
            ProzoneT1UpDown.ResetText()
            ProzoneT2UpDown.ResetText()
            SlopeBUpDown.ResetText()
            SubstrateDepleUpDown.ResetText()

            'TR 04/04/2011
            If Not Me.DesignMode Then
                bsTestRefRanges.ClearData()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "SetEmptyValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub UnitsCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnitsCombo.SelectedIndexChanged
        Try

            LinearityUnitLabel.Text = UnitsCombo.Text
            DetectionUnitLabel.Text = UnitsCombo.Text
            RepetitionRangeUnitLabel.Text = UnitsCombo.Text

            selMeasureUnit = UnitsCombo.Text
            Me.bsTestRefRanges.MeasureUnit = UnitsCombo.Text 'SG 06/09/2010

            'ValidateAllTabs() TR 20/07/2012 -Commented is not needed to validate.

            'TR 23/07/2012 -Validate the selected value on this comboBox.
            If EditionMode AndAlso UnitsCombo.SelectedIndex <= 0 Then
                BsErrorProvider1.SetError(UnitsCombo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            Else
                BsErrorProvider1.Clear() 'TR 04/06/2013
            End If
            'TR 23/07/2012 -END.


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "UnitsCombo_SelectedIndexChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub FirstReadingCycleUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstReadingCycleUpDown.ValueChanged
        Try

            CalculateFirstReadingSecReading()
            ValidateReadingsParameters()
            If EditionMode Then
                ChangesMade = True
                SetProzone1Times()
                If ChangeSampleTypeDuringEdition Then ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FirstReadingCycleUpDown_ValueChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SecondReadingCycleUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SecondReadingCycleUpDown.ValueChanged

        Try

            CalculateSecondReadingSec()
            ValidateReadingsParameters()
            If EditionMode Then
                'AG 06/04/2010
                'If SecondReadingSecUpDown.Value < ProzoneT2UpDown.Value Then
                'SetProzoneT2Limits()
                SetProzone1Times() 'TR 07/04/2010
                'End If
                'END AG 06/04/2010
                ChangesMade = True
                If ChangeSampleTypeDuringEdition Then ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SecondReadingCycleUpDown_ValueChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub FirsReadingSecUpDown_ValueChanged(ByVal sender As System.Object, _
                                                  ByVal e As System.EventArgs) Handles FirstReadingSecUpDown.ValueChanged
        Try
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)

            'TR 14/03/2011 -Add the analyzer model on the query.
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                            Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString, False) = 0 _
                            AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                            Select a).ToList()
            If qswParameter.Count > 0 Then
                FirstReadingCycleUpDown.Value = CType((FirstReadingSecUpDown.Value / qswParameter.First().ValueNumeric) + 1, Integer)
            End If
            ValidateReadingsParameters()

            'AG 18/03/2010
            If EditionMode Then
                'SetProzone1Times() 'TR 07/04/2010
                ChangesMade = True
                If ChangeSampleTypeDuringEdition Then ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If
            'END AG 18/03/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FirsReadingSecUpDown_ValueChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SecondReadingSecUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SecondReadingSecUpDown.ValueChanged
        Try
            Dim qswParameter As New List(Of ParametersDS.tfmwSwParametersRow)

            'TR 14/03/2011 -Add the analyzer model on the query.
            qswParameter = (From a In SwParametersDS.tfmwSwParameters _
                            Where a.ParameterName = GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString _
                            AndAlso a.AnalyzerModel = AnalyzerModelAttribute _
                            Select a).ToList()

            If qswParameter.Count > 0 Then
                'SecondReadingCycleUpDown.Value = CType((SecondReadingSecUpDown.Value / qswParameter.First().ValueNumeric) + 1, Integer)
                ''AG 05/12/2010
                Dim myValue As Decimal = CType((SecondReadingSecUpDown.Value / qswParameter.First().ValueNumeric) + 1, Integer)
                If myValue < SecondReadingCycleUpDown.Minimum Then
                    SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Minimum
                ElseIf myValue > SecondReadingCycleUpDown.Maximum Then
                    SecondReadingCycleUpDown.Value = SecondReadingCycleUpDown.Maximum
                Else
                    SecondReadingCycleUpDown.Value = myValue
                End If
                'END AG 05/12/2010
                If SecondReadingCycleUpDown.Text = "" Then ' TR 08/06/2010
                    SecondReadingCycleUpDown.Text = SecondReadingCycleUpDown.Value.ToString()
                End If
            End If
            ValidateReadingsParameters()

            'AG 18/03/2010
            If EditionMode Then
                ChangesMade = True
                If ChangeSampleTypeDuringEdition Then ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If
            'END AG 18/03/2010
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SecondReadingSecUpDown_ValueChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SecondReadingSecUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub AnalysisModeCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalysisModeCombo.SelectedIndexChanged
        Try
            If Not AnalysisModeCombo.SelectedValue Is Nothing Then
                'change the TestDescriptionTextBox for the new analisis mode.
                'DL 11/01/2012. Begin
                'TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCombo.Text)
                TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCboEx.Text)
                'DL 11/01/2012. End
                'enable or disable controls when analysis type is changed.
                CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())

                'TR 10/05/2010 -add the reading limits setup
                SetReadingLimits(AnalysisModeCombo.SelectedValue.ToString())

                SetProzone1Times()  'TR 07/04/2010
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "AnalysisModeCombo_SelectedIndexChanged " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    Private Sub TestDetailsTabs_Selecting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles TestDetailsTabs.Selecting
        'AG 18/03/2010
        'TO DO only validate tabs in edition mode
        'TR 02/07/2010 -Validation of multiselected items and the editions mode
        If Not TestListView.SelectedItems.Count > 1 AndAlso EditionMode Then
            'AG 03/12/2010 - Try reduce the calls to validate all tabs, leave only when SAVE. Does it work???
            'ValidateAllTabs()
        End If

    End Sub

    Private Sub ReferenceFilterCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReferenceFilterCombo.SelectedIndexChanged
        If Not ReferenceFilterCombo.SelectedValue Is Nothing Then ValidateWavelengths()
    End Sub

    Private Sub MainFilterCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainFilterCombo.SelectedIndexChanged
        If Not MainFilterCombo.SelectedValue Is Nothing Then ValidateWavelengths()
    End Sub

    Private Sub EditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditButton.Click
        EnableEdition()
    End Sub

    Private Sub TestListView_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles TestListView.ColumnClick
        Try
            'AG 10/03/10 (Add if condition)
            If isSortTestListAllowed Then
                'TR 29/03/2010 -Add valible testsposition change and implement select case.
                TestsPositionChange = True
                Select Case TestListView.Sorting
                    Case SortOrder.None
                        TestListView.Sorting = SortOrder.Ascending
                        Exit Select
                    Case SortOrder.Ascending
                        TestListView.Sorting = SortOrder.Descending
                        Exit Select
                    Case SortOrder.Descending
                        TestListView.Sorting = SortOrder.Ascending
                        Exit Select
                    Case Else
                        Exit Select
                End Select
                TestListView.Sort()

                'TR 28/05/2010 Set true on the changeMade variable to indicate there was a change inthe data.
                'ChangesMade = True
                'TR 14/12/2011 -Save the position changes
                UpdateTestPosition()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TestListView_ColumnClick " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Try
            Application.DoEvents()
            Dim screenClose As Boolean = True
            If Not EditionMode Then ChangesMade = False
            If (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    screenClose = False
                Else
                    'TR 19/05/2011 -Validate if there are selected items
                    If TestListView.SelectedItems.Count > 0 Then
                        'TR 01/03/2011 -Validate if data is complete
                        If Not ValidateCalibratorDataCompleted(CInt(TestListView.SelectedItems(0).Name), SelectedSampleTypeCombo.SelectedValue.ToString()) Then
                            BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                            UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                            screenClose = False
                        End If
                        'TR 01/03/2011 -End.
                    End If
                    'TR 19/05/2011 -END.
                End If
            End If

            If (screenClose) Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                ChangesMade = False
                EditionMode = False
                BsErrorProvider1.Clear()
                'RH 16/12/2010
                If Not Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If
            'END AG 11/11/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExitButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            'Me.Cursor = Cursors.Default  'AG 13/10/2010
        End Try
    End Sub

    ''' Modified by: SG 11/06/2010 Factor limits and Repetition range related textboxes became NumericUpDown
    Private Sub AddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddButton.Click
        Try
            'AG 08/07/2010
            Dim createNew As Boolean = True
            If EditionMode And (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                If ShowMessage("Question", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No Then
                    createNew = False
                Else
                    'TR 01/03/2011 -Validate if data is complete
                    If Not ValidateCalibratorDataCompleted(CInt(TestListView.SelectedItems(0).Name), SelectedSampleTypeCombo.SelectedValue.ToString()) Then
                        BsErrorProvider1.SetError(MultipleCalibRadioButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        SelectedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        createNew = False
                    End If
                    'TR 01/03/2011 -End.
                End If
            End If

            If createNew Then
                'END AG 08/07/2010
                TestListView.SelectedItems.Clear() 'TR 17/12/2010
                EditButton.Enabled = False
                DeleteButton.Enabled = False
                '                PrintTestButton.Enabled = False dl 11/05/2012
                CopyTestButton.Enabled = False 'TR 25/01/2012
                BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869

                'select general tab to start creating new test.
                TestDetailsTabs.SelectTab("GeneralTab")

                EditionMode = True
                NewTest = True
                ChangesMade = False ' AG 11/11/2010

                EnableDisableControls(TestDetailsTabs.Controls, True)
                'TR 10/05/2011 -Commented
                'SelectedSampleTypeCombo.Visible = False ' dl 16/07/2010
                'TR 10/05/2011 -End Commented
                If String.Compare(AnalysisModeCombo.Text, "", False) = 0 Then AnalysisModeCombo.SelectedIndex = 0 ' dl 15/07/2010
                CtrlsActivationByAnalysisMode(AnalysisModeCombo.SelectedValue.ToString())

                'Clear the structures.
                SelectedTestDS.tparTests.Clear()
                SelectedReagentsDS.tparReagents.Clear()
                SelectedTestSamplesDS.tparTestSamples.Clear()
                SelectedTestReagentsDS.tparTestReagents.Clear()
                SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Clear()
                SelectedTestRefRangesDS.tparTestRefRanges.Clear() 'SG 23/06/2010
                SelectedTestControlDS.tparTestControls.Clear() 'TR 11/04/2011 clear the selecte control DS.

                UncheckAllSampleTypes()

                'TR 06/04/2011 -Clear the control structure.
                SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Clear()

                'TR  25/02/2011 -Clear Update structure
                UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                UpdatedCalibratorsDS.tparCalibrators.Clear()
                UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                SelectedTestSampleCalibratorDS.tparTestCalibrators.Clear()
                'TR  25/02/2011 -END

                'add the new row to the tests dataset
                Dim newTestRow As TestsDS.tparTestsRow
                newTestRow = SelectedTestDS.tparTests.NewtparTestsRow()
                Dim qTestDS As New List(Of TestsDS.tparTestsRow)

                Dim myTestid As Integer = 500 'TR 30/06/2010 -AVOID COLLITIONS 

                'if there are row on the update test 
                If UpdateTestDS.tparTests.Rows.Count > 0 Then
                    'search if there are any new test to set the next temporal TestID
                    qTestDS = (From a In UpdateTestDS.tparTests _
                              Where a.NewTest Select a).ToList()
                    'if found the increase by one to set the new test id
                    If qTestDS.Count > 0 Then
                        myTestid = qTestDS.Last.TestID + 1
                    End If
                End If

                newTestRow.TestID = myTestid
                newTestRow.NewTest = True

                'add the row into the Test datatable.
                SelectedTestDS.tparTests.AddtparTestsRow(newTestRow)

                'Prepare the controls for a new test.
                PrepareGeneralTab() 'GENERAL TAB
                PrepareProcedureTab() 'PROCEDURE TAB
                PrepareCalibrationTab()   'CALIBRATOR TAB
                PrepareOptionTab()  'OPTION TAB
                PrepareQCTab() 'QC TAB -TR 06/04/2011

                SelectedTestDS.tparTests.AcceptChanges()

                SaveButton.Enabled = True

                'TR 08/11/2010
                CancelButton.Enabled = True

                'focus on the testname textbox
                TestNameTextBox.Select()
                ChangesMade = False ' AG 11/11/2010
                'TR 18/11/2010 -Clear the image (+)
                'SampleTypePlus.ImageLocation = ""
                SampleTypePlus2.ImageLocation = ""
                'TR 18/11/2010 -END

                'DL 26/01/2012. Begin
                '                SelectedSampleTypeCombo.DataSource = Nothing
                'SelectedTestSamplesDS.tparTestSamples.Clear()
                ''dl 26/01/2012
                'SelectedSampleTypeCombo.DataSource = SelectedTestSamplesDS.tparTestSamples
                'SelectedSampleTypeCombo.DisplayMember = "ItemIDDesc"
                'SelectedSampleTypeCombo.ValueMember = "SampleType"
                ''dl 26/01/2012
                'SelectedSampleTypeCombo.SelectedIndex = -1
                'DL(26 / 1 / 2012.End)

            End If ''AG 08/07/2010

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " AddButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub TestNameTextBox_Validating(ByVal sender As System.Object, _
                                           ByVal e As System.ComponentModel.CancelEventArgs) Handles TestNameTextBox.Validating
        Try
            BsErrorProvider1.Clear()
            If EditionMode Then
                If TestNameTextBox.Text.Length = 0 Then
                    TestNameTextBox.Focus()
                    BsErrorProvider1.SetError(TestNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True 'AG 28/09/2010
                Else
                    If SelectedTestDS.tparTests.Rows.Count > 0 Then
                        ValidationError = False 'AG 28/09/2010 - Initialize class variable
                        If TestNameExits(TestNameTextBox.Text, SelectedTestDS.tparTests(0).TestID) Then
                            BsErrorProvider1.SetError(TestNameTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_NAME.ToString)) 'AG 07/07/2010("REPETED_NAME"))
                            TestNameTextBox.Select()
                        End If
                    End If

                    'DL 11/01/2012. Begin
                    'TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & " - " & SampleTypeCombo.Text
                    TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & " - " & SampleTypeCboEx.Text
                    'DL 11/01/2012. End
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TestNameTextBox_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SampleVolUpDown_Validating(ByVal sender As System.Object, _
                                           ByVal e As System.ComponentModel.CancelEventArgs) Handles SampleVolUpDown.Validating
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            ValidationError = False
            If SampleVolUpDown.Text <> "" Then
                'validate if the preparation limits are ok.
                If ValidatePreparationVolumeLimits() Then
                    myGlobalDataTO = ValidatePostDilution()
                    If myGlobalDataTO.HasError Then
                        BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(myGlobalDataTO.ErrorCode))
                        ValidationError = True
                    End If
                Else
                    ShowPreparationVolumeError() 'TR 27/03/2012
                    'BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(GlobalEnumerates.Messages.PREPARATION_VOLUME.ToString)) 'AG 07/07/2010("PREPARATION_VOLUME"))
                    'SampleVolUpDown.Select() 'TR 15/05/2012 -Allow the user go to another control.
                    ValidationError = True
                End If
            Else
                BsErrorProvider1.SetError(SampleVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SampleVolUpDown_Validating " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SampleVolUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub WashSolVolUpDown_Validating(ByVal sender As System.Object, _
                                            ByVal e As System.ComponentModel.CancelEventArgs) Handles WashSolVolUpDown.Validating

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            ValidationError = False
            If WashSolVolUpDown.Text <> "" Then
                'myGlobalDataTO = ValidatePostDilution()    'AG 26/03/2010 - Dont need to validate postdilution when washing volume changes
                If myGlobalDataTO.HasError Then
                    BsErrorProvider1.SetError(WashSolVolUpDown, GetMessageText(myGlobalDataTO.ErrorCode))
                    WashSolVolUpDown.Select()
                    ValidationError = True
                End If
            Else
                BsErrorProvider1.SetError(WashSolVolUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
                WashSolVolUpDown.Select() 'TR 15/11/2010 Set the focus
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WashSolVolUpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WashSolVolUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub VolR1UpDown_Validating(ByVal sender As System.Object, _
                                       ByVal e As System.ComponentModel.CancelEventArgs) Handles VolR1UpDown.Validating

        Try

            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            ValidationError = False
            If VolR1UpDown.Text <> "" Then
                'validate if the preparation limits are ok.
                If ValidatePreparationVolumeLimits() Then
                    myGlobalDataTO = ValidatePostDilution()
                    If myGlobalDataTO.HasError Then
                        BsErrorProvider1.SetError(VolR1UpDown, GetMessageText(myGlobalDataTO.ErrorCode))
                        'VolR1UpDown.Select() 'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                        ValidationError = True
                    End If
                Else
                    ShowPreparationVolumeError() 'TR 27/03/2012
                    'BsErrorProvider1.SetError(VolR1UpDown, GetMessageText(GlobalEnumerates.Messages.PREPARATION_VOLUME.ToString)) 'AG 07/07/2010("PREPARATION_VOLUME"))
                    'VolR1UpDown.Select()'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                    ValidationError = True
                End If
            Else
                BsErrorProvider1.SetError(VolR1UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "VolR1UpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VolR1UpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub VolR2UpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles VolR2UpDown.Validating
        Try

            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            ValidationError = False
            If VolR2UpDown.Text <> "" Then
                'validate if the preparation limits are ok.
                If ValidatePreparationVolumeLimits() Then
                    myGlobalDataTO = ValidatePostDilution()
                    If myGlobalDataTO.HasError Then
                        BsErrorProvider1.SetError(VolR2UpDown, GetMessageText(myGlobalDataTO.ErrorCode))
                        'VolR2UpDown.Select()'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                        ValidationError = True
                    End If
                Else
                    ShowPreparationVolumeError() 'TR 17/05/2012
                    'BsErrorProvider1.SetError(VolR2UpDown, GetMessageText(GlobalEnumerates.Messages.PREPARATION_VOLUME.ToString)) 'AG 07/07/2010("PREPARATION_VOLUME"))
                    'VolR2UpDown.Select()'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                    ValidationError = True
                End If
            Else
                BsErrorProvider1.SetError(VolR2UpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "VolR2UpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VolR2UpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub RedPostDilutionFactorTextBox_Validating(ByVal sender As System.Object, _
                                                        ByVal e As System.ComponentModel.CancelEventArgs) Handles RedPostDilutionFactorTextBox.Validating

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            If (RedPostDilutionFactorTextBox.Text <> "") AndAlso (IsNumeric(RedPostDilutionFactorTextBox.Text)) Then
                If ValidateControlLimits(CType(RedPostDilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.POSTDILUTION_FACTOR, RedPostDilutionFactorTextBox) Then
                    'TR 17/05/2012 -Get the range value to show on error message.
                    BsErrorProvider1.SetError(RedPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                              GetStringLimitsValues(FieldLimitsEnum.POSTDILUTION_FACTOR))
                    'RedPostDilutionFactorTextBox.Select() 'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                    ValidationError = True
                Else
                    myGlobalDataTO = ValidatePostDilution("RED")    'AG 26/03/2010 - Add optional parameter
                    If myGlobalDataTO.HasError Then
                        BsErrorProvider1.Clear()
                        BsErrorProvider1.SetError(PostDilutionFactorGroupbox, GetMessageText(myGlobalDataTO.ErrorCode))
                        ValidationError = True
                    End If
                End If
            Else
                RedPostDilutionFactorTextBox.Text = "1" 'TR 17/05/2012 -Set the default value = 1 if empty.
                'BsErrorProvider1.SetError(RedPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RedPostDilutionFactorTextBox_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RedPostDilutionFactorTextBox_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub IncPostDilutionFactorTextBox_Validating(ByVal sender As System.Object, _
                                                        ByVal e As System.ComponentModel.CancelEventArgs) Handles IncPostDilutionFactorTextBox.Validating
        Try

            Dim myGlobalDataTO As New GlobalDataTO
            BsErrorProvider1.Clear()
            If (IncPostDilutionFactorTextBox.Text <> "") AndAlso (IsNumeric(IncPostDilutionFactorTextBox.Text)) Then
                If ValidateControlLimits(CType(IncPostDilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.POSTDILUTION_FACTOR, IncPostDilutionFactorTextBox) Then
                    'TR 17/05/2012 -Get the range value to show on error message.
                    BsErrorProvider1.SetError(IncPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                              GetStringLimitsValues(FieldLimitsEnum.POSTDILUTION_FACTOR))
                    'IncPostDilutionFactorTextBox.Select()  'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                    ValidationError = True
                Else
                    myGlobalDataTO = ValidatePostDilution("INC")    'AG 26/03/2010 - add optional parameter
                    If myGlobalDataTO.HasError Then
                        BsErrorProvider1.Clear()
                        BsErrorProvider1.SetError(PostDilutionFactorGroupbox, GetMessageText(myGlobalDataTO.ErrorCode))
                        'IncPostDilutionFactorTextBox.Select() 'TR 12/03/2012 Commented to allow the user go to another control and correct changes 
                        ValidationError = True
                    End If
                End If
            Else
                IncPostDilutionFactorTextBox.Text = "1" 'TR 17/05/2012 -Set the default value = 1 if empty.
                'BsErrorProvider1.SetError(IncPostDilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                'IncPostDilutionFactorTextBox.Select()
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "IncPostDilutionFactorTextBox_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IncPostDilutionFactorTextBox_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Activate the Change made variable when some value of 
    ''' the corresponding control is change.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 18/03/2010 - complete control list and change function name ControlValueChanged</remarks>
    Private Sub ControlValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                                    ReferenceFilterCombo.TextChanged, RedPostDilutionFactorTextBox.TextChanged, _
                                    ReadingModeCombo.TextChanged, PredilutionModeCombo.TextChanged, _
                                    PredilutionFactorTextBox.TextChanged, MainFilterCombo.TextChanged, _
                                    IncPostDilutionFactorTextBox.TextChanged, LinearityUpDown.ValueChanged, _
                                    DetectionUpDown.ValueChanged, _
                                    CalibrationFactorTextBox.TextChanged, BlankTypesCombo.TextChanged, _
                                    BlankAbsorbanceUpDown.ValueChanged, AlternativeCalComboBox.TextChanged, _
                                    PredilutionFactorCheckBox.CheckedChanged, AutoRepetitionCheckbox.CheckedChanged, _
                                    WashSolVolUpDown.ValueChanged, VolR2UpDown.ValueChanged, VolR1UpDown.ValueChanged, _
                                    CalReplicatesUpDown.ValueChanged, _
                                    BlankReplicatesUpDown.ValueChanged, FactorLowerLimitUpDown.ValueChanged, _
                                    FactorUpperLimitUpDown.ValueChanged, RerunLowerLimitUpDown.ValueChanged, _
                                    RerunUpperLimitUpDown.ValueChanged, MultipleCalibRadioButton.TextChanged, _
                                    FactorRadioButton.TextChanged, AlternativeCalibRadioButton.TextChanged, _
                                    SlopeBUpDown.ValueChanged, UnitsCombo.TextChanged, _
                                    TestNameTextBox.TextChanged, ShortNameTextBox.TextChanged, ReportsNameTextBox.TextChanged, _
                                    ReplicatesUpDown.ValueChanged, ReactionTypeCombo.TextChanged, _
                                    DecimalsUpDown.ValueChanged, AnalysisModeCombo.TextChanged, _
                                    AbsCheckBox.CheckedChanged, SlopeAUpDown.ValueChanged, _
                                    ProzonePercentUpDown.ValueChanged, KineticBlankUpDown.ValueChanged, _
                                    DiluentComboBox.TextChanged, QCReplicNumberNumeric.ValueChanged, _
                                    QCMinNumSeries.ValueChanged, QCErrorAllowable.ValueChanged, _
                                    SubstrateDepleUpDown.ValueChanged, SampleVolUpDown.ValueChanged

        If EditionMode Then
            If ChangeSampleTypeDuringEdition Then
                ChangesMade = False 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
                'AG 11/01/2011 -comment TR code, try save test-sample type on change sampletype
                'SampleTypeCombo.Enabled = True 'TR 22/12/2010 disable the combo 
                'AddSampleTypeButton.Enabled = True 'TR 22/12/2010 disable the add test sample button 
                'DeleteSampleTypeButton.Enabled = True  'TR 22/12/2010 disable the delete test sample button
            Else
                ChangesMade = True
            End If
        End If

    End Sub

    ''' <summary>
    ''' Activate the Change made variable when some value of the corresponding numericupdown control is change by text (not only by arrows)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' DL 28/07/2011 
    ''' Modified by; TR 18/11/2011 -Add more elements to event handler (elements on the Options Tabs). 
    ''' </remarks>
    Private Sub TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _
                            SampleVolUpDown.TextChanged, VolR1UpDown.TextChanged, VolR1UpDown.TextChanged, _
                            VolR2UpDown.TextChanged, LinearityUpDown.TextChanged, KineticBlankUpDown.TextChanged, _
                            FactorLowerLimitUpDown.TextChanged, FactorUpperLimitUpDown.TextChanged, _
                            RerunLowerLimitUpDown.TextChanged, RerunUpperLimitUpDown.TextChanged, DetectionUpDown.TextChanged, _
                            SlopeAUpDown.TextChanged, SlopeBUpDown.TextChanged, ProzonePercentUpDown.TextChanged, _
                            ProzoneT1UpDown.TextChanged, ProzoneT2UpDown.TextChanged, SubstrateDepleUpDown.TextChanged, _
                            BlankAbsorbanceUpDown.TextChanged
        Try
            If EditionMode Then
                Dim myNewValue As String = DirectCast(sender, BSNumericUpDown).Text
                Dim myMaxValue As Single = DirectCast(sender, BSNumericUpDown).Maximum
                Dim myMinValue As Single = DirectCast(sender, BSNumericUpDown).Minimum

                If myNewValue.Length > 0 AndAlso myNewValue.Length > myMaxValue.ToString.Length Then
                    If CSng(myNewValue) > CSng(myMaxValue) Then
                        DirectCast(sender, Biosystems.Ax00.Controls.UserControls.BSNumericUpDown).Text = myMaxValue.ToString
                    ElseIf CSng(myNewValue) < myMinValue Then
                        If IsNumeric(Microsoft.VisualBasic.Left(myNewValue, 1)) Then
                            DirectCast(sender, Biosystems.Ax00.Controls.UserControls.BSNumericUpDown).Text = myMinValue.ToString
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".TextChange", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TextChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub CalibrationFactorTestBox_Validated(ByVal sender As System.Object, _
                                                   ByVal e As System.EventArgs) Handles CalibrationFactorTextBox.Validated

        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            If FactorRadioButton.Checked Then
                'AG 27/09/2010 - dont allow zero values
                If IsNumeric(CalibrationFactorTextBox.Text) Then
                    If CSng(CalibrationFactorTextBox.Text) = 0 Then
                        CalibrationFactorTextBox.Clear()
                    End If
                End If
                'END AG 27/09/2010 

                If String.Compare(CalibrationFactorTextBox.Text, "", False) = 0 Then
                    BsErrorProvider1.SetError(CalibrationFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                Else
                    'TR 22/03/2010
                    If Not IsNumeric(CalibrationFactorTextBox.Text) Then
                        BsErrorProvider1.SetError(CalibrationFactorTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                        ValidationError = True
                    End If
                    'END TR 22/03/2010
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "CalibrationFactorTestBox_Validated " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalibrationFactorTestBox_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub DeleteSampleTypeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteSampleTypeButton.Click
        Try
            BsErrorProvider1.Clear() 'TR 12/09/2011
            Dim confirmDeletion As Boolean = True 'TR 17/12/2010 -Use to show deletion messages on Deleteselected test
            'DL 11/01/2012. Begin
            'If Not SampleTypeCombo.SelectedValue Is Nothing AndAlso SelectedTestSamplesDS.tparTestSamples.Rows.Count > 1 Then
            If Not SampleTypeCboEx.Text Is String.Empty AndAlso SelectedTestSamplesDS.tparTestSamples.Rows.Count > 1 Then
                'DL 11/01/2012. End
                'Show warning message before deleting.
                'AG 07/07/2010
                If ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_SAMPTYPE_CONFIRMATION.ToString()) = Windows.Forms.DialogResult.Yes Then
                    'DL 11/01/2012. Begin
                    'If Not IsSampleTypeUseAsAlternativeCalib(SampleTypeCombo.SelectedValue.ToString()) Then
                    'If Not IsSampleTypeUseAsAlternativeCalib(bsSampleTypeText.Text.ToString()) Then

                    If Not IsSampleTypeUseAsAlternativeCalib(SampleTypeCboEx.Text) Then
                        'If Not IsSampleTypeUseAsAlternativeCalib(SampleTypeCombo.SelectedValue.ToString()) Then
                        'Delete the sample Type.
                        'Dim NewSampleType As String = SampleTypeCombo.SelectedValue.ToString()
                        'Dim NewSampleType As String = SampleTypeCboEx.Text.ToString()
                        'DeleteTestSampleType(SelectedTestDS.tparTests(0).TestID, SampleTypeCombo.SelectedValue.ToString())
                        DeleteTestSampleType(SelectedTestDS.tparTests(0).TestID, SampleTypeCboEx.Text)
                        'DL 11/01/2012. End

                        SaveChanges(False, False, True) 'AG 11/02/2011
                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer)) 'AG 14/02/2011 -Bind the default sample type
                        bsTestRefRanges.isEditing = EditionMode 'AG 14/02/2011
                        ChangesMade = False 'TR 01/02/2012 
                        AddedSampleType = String.Empty 'TR 02/02/2012 'Set the value to empty after removing sample type
                        ''SG 20/07/2010
                        'ChangesMade = True 'TR 24/11/2010 Commented value is change on deleteTestSampleType
                    Else
                        'Show message indicating that is used as alternative calibrator an need to change.
                        ShowMessage("Warning", GlobalEnumerates.Messages.USED_ALTERNATIVECAL.ToString())
                    End If

                End If
            Else
                Dim deleteConfirm As Boolean = True
                'DL 11/01/2012. Begin
                'If Not SampleTypeCombo.SelectedValue Is Nothing AndAlso SelectedTestSamplesDS.tparTestSamples.Rows.Count = 1 Then
                If Not SampleTypeCboEx.Text Is String.Empty AndAlso SelectedTestSamplesDS.tparTestSamples.Rows.Count = 1 Then
                    'DL 11/01/2012. End
                    deleteConfirm = False
                    'TR 12/09/2011 -Validate if is preloaded test.
                    If CBool(TestListView.SelectedItems(0).SubItems(1).Text) Then
                        BsErrorProvider1.SetError(TestListView, GetMessageText(GlobalEnumerates.Messages.LAST_SAMPLETYPE.ToString(), currentLanguage))
                        BsErrorProvider1.SetIconPadding(TestListView, 10)

                    Else
                        If ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_SINGLE_SAMPLETYPE.ToString) = Windows.Forms.DialogResult.Yes Then
                            deleteConfirm = True
                            confirmDeletion = False
                        End If
                    End If

                ElseIf UpdateTestSamplesDS.tparTestSamples.Count = 0 Then 'TR 17/12/2010
                    'SampleTypeCombo.DataSource = Nothing 'DL 11/01/2012
                    'CancelEdition()
                    deleteConfirm = False
                End If

                If deleteConfirm Then 'AG 12/11/2010
                    'Delete the Test.
                    'AG 07/07/2010 - Sw deletes Test but dont update listview. We use the same code as in Delete test button
                    'DeleteTestSampleType(SelectedTestDS.tparTests(0).TestID, "")
                    For Each testID As ListViewItem In TestListView.SelectedItems
                        If testID.Tag.ToString = "False" Then
                            DeleteSelectedTest(confirmDeletion)
                            'DeleteTestSampleType(CType(testID.Name, Integer), "")
                            'testID.Remove() 'TR 25/11/2010 -Commented
                        End If
                    Next

                    'END AG 07/07/2010
                    ChangesMade = False
                    'SELECT THE FIRST ELEMENT ON THE LIST
                    If TestListView.SelectedItems.Count = 0 AndAlso TestListView.Items.Count > 0 Then
                        TestListView.Items(0).Selected = True
                        QuerySelectedTest() 'TR 17/12/2010
                    End If
                    'TR 30/11/2010 -END Commented


                End If 'END 'AG 12/11/2010
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "DeleteSampleTypeButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSampleTypeButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    Private Sub SelectedSampleTypeCombo_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedSampleTypeCombo.SelectionChangeCommitted
        Try
            Dim savelocalerror As Boolean = False

            If (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                'TR 13/01/2012 -Before saving set the selected sample type to local variable 
                Dim SampleBeforeSaving As String = SelectedSampleTypeCombo.SelectedValue.ToString
                'If Not SaveTestLocal(NewTest, CurrentSampleType) Then
                If SavePendingWarningMessage(True) = Windows.Forms.DialogResult.No Then
                    savelocalerror = True
                Else

                    If SaveChanges(True, False) Then 'TR 21/12/2010 -Save changes on database.
                        EditionMode = True

                        SelectedSampleTypeCombo.SelectedValue = SampleBeforeSaving 'Set the selected element after saving.
                    End If
                End If
            End If

            If Not savelocalerror Then
                bsTestRefRanges.ChangesMade = False
                ChangeSampleTypeDuringEdition = True
                bsTestRefRanges.ChangeSampleTypeDuringEdition = ChangeSampleTypeDuringEdition

                If TestListView.SelectedItems.Count > 0 Then 'AG 12/11/2010 - fix a system error if click over cbo when test is new
                    If Not SelectedSampleTypeCombo.SelectedValue Is Nothing AndAlso TestListView.SelectedItems(0).Name <> "" Then
                        If SelectedSampleTypeCombo.Text.ToString <> CurrentSampleType Then 'AG 12/11/2010 - save only when selected item changes else a system error is produce is test is new
                            'TR 17/11/2010 -Validate if is a new test to select the test id form the selectedTest Table

                            CurrentSampleType = SelectedSampleTypeCombo.SelectedValue.ToString() 'AG 13/01/2011
                            GetRefRanges(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType) 'AG 13/01/2011

                            If NewTest Then
                                BindControls(SelectedTestDS.tparTests(0).TestID, CurrentSampleType, False)
                            ElseIf TestListView.SelectedItems.Count > 0 And EditionMode Then 'in case is not new then select from the list.
                                BindControls(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType, False)
                            ElseIf TestListView.SelectedItems.Count > 0 Then
                                BindControls(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType)
                            End If
                            'TR 17/11/2010 -END
                            CurrentSampleType = SelectedSampleTypeCombo.SelectedValue.ToString()  'TR 21/12/2010 -Set current type.
                        End If 'END AG 12/11/2010
                    End If
                End If 'END AG 12/11/2010
            Else
                'Data changed for the previous SampleType could not be changed due to 
                'error warnings; recovery the previous SampleType in the ComboBox
                SelectedSampleTypeCombo.SelectedValue = CurrentSampleType
            End If
            'DL 11/01/2012. Begin
            'TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCombo.Text)
            TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCboEx.Text)
            'DL 11/01/2012. End
            ChangeSampleTypeDuringEdition = False
            bsTestRefRanges.ChangeSampleTypeDuringEdition = ChangeSampleTypeDuringEdition
            'TR 21/07/2011- This combo is enabled only in edition mode use the variable Edition mode instead setting True 
            bsTestRefRanges.isEditing = EditionMode
            'END AG 19/07/2010

            If ChangesMade Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = Windows.Forms.DialogResult.Yes) Then
                    SelectedSampleTypeCombo.SelectedValue = CurrentSampleType
                    ChangesMade = False
                    EditionMode = False
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SelectedSampleTypeCombo_SelectionChangeCommitted " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SelectedSampleTypeCombo_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ReadingModeCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReadingModeCombo.SelectedIndexChanged
        Try
            If Not ReadingModeCombo.SelectedValue Is Nothing Then
                If ReadingModeCombo.SelectedValue.ToString = "MONO" Then
                    ReferenceFilterCombo.SelectedIndex = -1
                    ReferenceFilterCombo.Enabled = False
                Else
                    If EditionMode Then 'TR 18/03/2010
                        ReferenceFilterCombo.SelectedIndex = -1
                        'AG 19/03/2010
                        'ReferenceFilterCombo.Enabled = True
                        If EditionMode Then ReferenceFilterCombo.Enabled = True
                        ValidateProzones() 'TR 08/04/2010
                    End If

                End If
                SetProzone1Times() 'TR 07/04/2010
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReadingModeCombo_SelectedIndexChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReadingModeCombo_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub BlankAbsorbanceUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BlankAbsorbanceUpDown.ValueChanged
        BlankAbsorbanceValidation(BlankAbsorbanceUpDown.Value)
    End Sub

    Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteButton.Click
        DeleteSelectedTest()
    End Sub

    Private Sub TestDetailsTabs_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles TestDetailsTabs.Selected
        FocusControlByTab(TestDetailsTabs.SelectedTab.Name)
    End Sub

    Private Sub ShortNameTextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ShortNameTextBox.Validating
        BsErrorProvider1.Clear()
        If ShortNameTextBox.Text.Length = 0 Then
            ShortNameTextBox.Focus()
            BsErrorProvider1.SetError(ShortNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
        Else
            If SelectedTestDS.tparTests.Rows.Count > 0 Then
                'TR 16/11/2010 -Validate if the short name exist 
                If ShortNameExist(ShortNameTextBox.Text) Then
                    BsErrorProvider1.SetError(ShortNameTextBox, GetMessageText(GlobalEnumerates.Messages.REPEATED_SHORTNAME.ToString))
                    ShortNameTextBox.Select()
                End If
                'TR 16/11/2010 -END
            End If
        End If
    End Sub

    ' DL 21/06/2010
    Private Sub TestListView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles TestListView.PreviewKeyDown
        If e.KeyCode = Keys.Delete And Not EditionMode Then
            If DeleteButton.Enabled = True Then DeleteSelectedTest()
        ElseIf e.KeyCode = Keys.Enter And Not EditionMode Then
            EnableEdition()
        End If
    End Sub

    Private Sub CalibrationFactorTestBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
        End If
    End Sub


    Private Sub TestNameTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TestNameTextBox.KeyPress
        'AG 27/04/2010 - Add 2on parameter
        If ValidateSpecialCharacters(e.KeyChar, "[@#~$%&/()-+><_.:,;!¿?=·ªº'¡|}^{]") Then e.Handled = True
    End Sub

    Private Sub ShortNameTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ShortNameTextBox.KeyPress
        'AG 27/04/2010 - Add 2on parameter
        If ValidateSpecialCharacters(e.KeyChar, "[@#~$%&/()-+><_.:,;!¿?=·ªº'¡|}^{]") Then e.Handled = True
    End Sub

    Private Sub SampleTypeCombo_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'AG 19/07/2010
        'If Not SampleTypeCombo.SelectedValue Is Nothing Then
        '    TestDescriptionTextBox.Text = TestNameTextBox.Text & " (" & AnalysisModeCombo.Text & ") " & " - " & SampleTypeCombo.Text
        'End If

        Dim savelocalerror As Boolean = False

        If (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
            'If Not SaveTestLocal(NewTest, CurrentSampleType) Then
            If SavePendingWarningMessage(True) = Windows.Forms.DialogResult.No Then
                savelocalerror = True
            Else
                If SaveChanges(True, False) Then 'TR 21/12/2010 -Save changes on database.
                    EditionMode = True
                End If
            End If
        End If

        If Not savelocalerror Then
            bsTestRefRanges.ChangesMade = False
            ChangeSampleTypeDuringEdition = True
            bsTestRefRanges.ChangeSampleTypeDuringEdition = ChangeSampleTypeDuringEdition

            If TestListView.SelectedItems.Count > 0 Then 'AG 12/11/2010 - fix a system error if click over cbo when test is new
                'DL 11/01/2012. Begin
                If Not SampleTypeCboEx.Text Is String.Empty AndAlso TestListView.SelectedItems(0).Name <> "" Then
                    'If Not bsSampleTypeText.Text Is String.Empty AndAlso TestListView.SelectedItems(0).Name <> "" Then
                    If SampleTypeCboEx.Text.ToString <> CurrentSampleType Then 'AG 12/11/2010 - save only when selected item changes else a system error is produce is test is new
                        'If bsSampleTypeText.Text.ToString <> CurrentSampleType Then 'AG 12/11/2010 - save only when selected item changes else a system error is produce is test is new
                        'TR 17/11/2010 -Validate if is a new test to select the test id form the selectedTest Table
                        CurrentSampleType = SampleTypeCboEx.Text.ToString 'AG 13/01/2011
                        'CurrentSampleType = bsSampleTypeText.Text.ToString 'AG 13/01/2011

                        GetRefRanges(CType(TestListView.SelectedItems(0).Name, Integer), CurrentSampleType) 'AG 13/01/2011

                        If NewTest Then
                            'BindControls(SelectedTestDS.tparTests(0).TestID, SampleTypeCombo.Text, False)
                            BindControls(SelectedTestDS.tparTests(0).TestID, SampleTypeCboEx.Text, False)
                        ElseIf TestListView.SelectedItems.Count > 0 And EditionMode Then 'in case is not new then select from the list.
                            'BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCombo.Text, False)
                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCboEx.Text, False)
                        ElseIf TestListView.SelectedItems.Count > 0 Then
                            'BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCombo.Text)
                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer), SampleTypeCboEx.Text)
                        End If

                        'CurrentSampleType = SampleTypeCombo.Text 'TR 21/12/2010 -Set current type.
                        CurrentSampleType = SampleTypeCboEx.Text
                        'DL 11/01/2012, End
                    End If 'END AG 12/11/2010
                End If
            End If 'END AG 12/11/2010

        Else
            'Data changed for the previous SampleType could not be changed due to 
            'error warnings; recovery the previous SampleType in the ComboBox
            'DL 11/01/2012. Begin
            'SampleTypeCombo.SelectedValue = CurrentSampleType
            'bsSampleTypeText.Text = CurrentSampleType
            SampleTypeCboEx.Items.Clear()
            SampleTypeCboEx.Items.Add(CurrentSampleType)
            SampleTypeCboEx.SelectedIndex = 0

            SampleTypeCboAux.Items.Clear()
            SampleTypeCboAux.Items.Add(CurrentSampleType)
            SampleTypeCboAux.SelectedIndex = 0
            'DL 11/01/2012. End
        End If

        'DL 11/01/2012. Begin
        'TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCombo.Text)
        TestDescriptionTextBox.Text = String.Format("{0} ({1})  - {2}", TestNameTextBox.Text, AnalysisModeCombo.Text, SampleTypeCboEx.Text)
        'DL 11/01/2012. End

        ChangeSampleTypeDuringEdition = False
        bsTestRefRanges.ChangeSampleTypeDuringEdition = ChangeSampleTypeDuringEdition
        bsTestRefRanges.isEditing = True 'This combo is enabled only in edition mode
        'END AG 19/07/2010

    End Sub

    Private Sub SlopeAUpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SlopeAUpDown.Validating

        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            If Not SlopeAUpDown.Text = "" AndAlso SlopeAUpDown.Value = 0 Then
                BsErrorProvider1.SetError(SlopeAUpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
                ValidationError = True
                SlopeAUpDown.Select()
            ElseIf Not SlopeAUpDown.Text = "" AndAlso SlopeBUpDown.Text = "" Then
                BsErrorProvider1.SetError(SlopeBUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                ValidationError = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SlopeAUpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SlopeAUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ProzoneT2UpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ProzoneT2UpDown.Validating

        Try
            'prozone validation rule

            BsErrorProvider1.Clear()
            ValidationError = False
            'validate if turbidimetry is enable before validating.
            'If TurbidimetryCheckBox.Checked Then 'TR 07/05/2011 -commented 
            ValidateProzones()
            'AG 06/04/2010
            'TR 07/5/2011 -validate values are not empty
            If ProzoneT1UpDown.Text <> "" AndAlso ProzoneT2UpDown.Text <> "" Then
                If ProzoneT1UpDown.Value >= ProzoneT2UpDown.Value Then
                    BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.PROZONE_TIMES.ToString)) 'AG 07/07/2010("PROZONE_TIMES"))
                    'ProzoneT2UpDown.Select()
                    ProzoneT2UpDown.ResetText() 'TR 07/04/2010 rest the text value to clear the box
                    ValidationError = True
                    'AG 06/04/2010
                Else
                    If SecondReadingSecUpDown.Text <> "" And ProzoneT2UpDown.Text <> "" Then
                        'AG 18/06/2012 - comment unused message, this condition never triggers. Besides it was too long > 64 characters
                        'If SecondReadingSecUpDown.Value < ProzoneT2UpDown.Value Then
                        '    BsErrorProvider1.SetError(ProzoneT2UpDown, GetMessageText(GlobalEnumerates.Messages.READING2GREATERT2.ToString)) 'AG 07/07/2010("READING2GREATERT2"))
                        '    ProzoneT2UpDown.ResetText() 'TR 07/04/2010 rest the text value to clear the box
                        '    ValidationError = True
                        'End If
                    End If
                End If
                'End If 'TR 07/05/2011 -commented 
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ProzoneT2UpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProzoneT2UpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ProzoneT1UpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProzoneT1UpDown.ValueChanged
        Try
            'AG 06/04/2010
            If EditionMode Then
                If ChangeSampleTypeDuringEdition Then
                    ChangesMade = False
                Else
                    ChangesMade = True
                End If 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
                ValidateProzones() 'TR 08/04/2010
            End If
            'END AG 06/04/2010
            If ProzoneT2UpDown.Text = "" Then
                ProzoneT2UpDown.ResetText()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ProzoneT1UpDown_ValueChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProzoneT1UpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SlopeAUpDown_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ValidationError = False
            BsErrorProvider1.Clear()
            If SlopeAUpDown.Text <> "" Then
                If SlopeAUpDown.Value = 0 Then
                    BsErrorProvider1.SetError(SlopeAUpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
                    SlopeAUpDown.Select()
                    ValidationError = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SlopeAUpDown_Validated " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SlopeAUpDown_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ProzoneT1UpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ProzoneT1UpDown.Validating
        Try
            'AG 06/04/2010
            'ValidateProzoneTimes()
            'prozone validation rule.
            'set the min value to the prozone T2
            If ProzoneT1UpDown.Text <> "" Then
                ProzoneT2UpDown.Minimum = ProzoneT1UpDown.Value
            Else
                ProzoneT1UpDown.ResetText()
            End If

            'TR 07/0/2011
            ValidationError = False
            BsErrorProvider1.Clear()
            If ProzonePercentUpDown.Text = "" OrElse ProzoneT1UpDown.Value >= ProzoneT2UpDown.Value Then
                BsErrorProvider1.SetError(ProzonePercentUpDown, GetMessageText(GlobalEnumerates.Messages.PROZONE_TIMES.ToString))
                ValidationError = True
            End If
            'TR 07/0/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ProzoneT1UpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProzoneT1UpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    Private Sub PredilutionFactorTextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles PredilutionFactorTextBox.Validating
        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            If (PredilutionFactorTextBox.Text <> "") AndAlso (IsNumeric(PredilutionFactorTextBox.Text)) Then
                If ValidateControlLimits(CType(PredilutionFactorTextBox.Text, Decimal), FieldLimitsEnum.PREDILUTION_FACTOR, PredilutionFactorTextBox) Then
                    'TR 17/05/2012 -Get the min and max value to show on error message.
                    BsErrorProvider1.SetError(PredilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString) & " " & _
                                              GetStringLimitsValues(FieldLimitsEnum.PREDILUTION_FACTOR))

                    'BsErrorProvider1.SetError(PredilutionFactorTextBox, GetMessageText(GlobalEnumerates.Messages.VALUE_OUT_RANGE.ToString)) 'AG 07/07/2010("VALUE_OUT_RANGE"))
                    ValidationError = True
                    'PredilutionFactorTextBox.Select()'TR 17/05/2012
                End If
            Else
                'TR 17/05/2012 -Set the min value as default.
                Dim FieldLimitsDS As New FieldLimitsDS
                FieldLimitsDS = GetControlsLimits(FieldLimitsEnum.PREDILUTION_FACTOR)
                Dim minValue As Single = 0
                If FieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    minValue = FieldLimitsDS.tfmwFieldLimits(0).MinValue
                    PredilutionFactorTextBox.Text = minValue.ToString()
                End If
                'TR 17/05/2012 -END.
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "PredilutionFactorTextBox_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PredilutionFactorTextBox_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ProzoneT2UpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProzoneT2UpDown.ValueChanged
        Try
            If EditionMode Then
                If ChangeSampleTypeDuringEdition Then
                    ChangesMade = False
                Else
                    ChangesMade = True
                End If 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ProzoneT2UpDown_ValueChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProzoneT2UpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created AG 08/07/2010
    ''' </remarks>
    Private Sub PredilutionFactorCheckBox_CheckStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PredilutionFactorCheckBox.CheckStateChanged
        Try
            If EditionMode Then
                EnableDisablePredilutionControls()
                If PredilutionFactorCheckBox.CheckState = CheckState.Checked Then
                    If PredilutionModeCombo.Items.Count > 0 Then
                        PredilutionModeCombo.SelectedIndex = 0
                        DiluentComboBox.SelectedIndex = 0 ' TR 17/01/2011
                        PredilutionFactorTextBox.Text = "2" 'TR 17/05/2012 Set the min value as default value 
                    End If
                Else
                    PredilutionModeCombo.SelectedIndex = -1
                    PredilutionModeCombo.Select()
                    DiluentComboBox.SelectedIndex = -1 ' TR 17/01/2011
                    PredilutionFactorTextBox.Clear() 'TR 17/05/2012
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "PredilutionFactorCheckBox_CheckStateChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PredilutionFactorCheckBox_CheckStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ' DL 05/05/2010
    Private Sub DetectionUpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DetectionUpDown.Validating, _
                                                                                                                                    LinearityUpDown.Validating
        Try
            BsErrorProvider1.Clear()
            If LinearityUpDown.Text <> "" And DetectionUpDown.Text <> "" Then
                'If DetectionUpDown.Text <> "" Then
                If CType(DetectionUpDown.Text, Decimal) > CType(LinearityUpDown.Text, Decimal) Then
                    BsErrorProvider1.SetError(DetectionUpDown, GetMessageText(GlobalEnumerates.Messages.DETGREATHERLIN.ToString)) 'AG 07/07/2010("DETGREATHERLIN"))
                    ValidationError = True
                    DetectionUpDown.Select()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "DetectionUpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DetectionUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' This generic method allows to validate the values of a pair of BSNumericUpDowns in which
    ''' if one of then is empty or the minimum one is higher than the maximum one the validation is not approved
    ''' </summary>
    ''' <param name="LimitUpdown1">Minimum limit BSNumericUpDown to evaluate</param>
    ''' <param name="LimitUpdown2">Maximum limit BSNumericUpDown to evaluate</param>
    ''' <remarks>
    ''' CREATED: SG 11/06/2010
    ''' </remarks>
    Private Sub ValidateLimitsUpDown(ByRef LimitUpdown1 As BSNumericUpDown, ByRef LimitUpdown2 As BSNumericUpDown, ByVal pMandatory As Boolean)

        Try
            'ValidationError = False

            If LimitUpdown1.Text = "0" AndAlso LimitUpdown2.Text = "0" Then
                LimitUpdown1.Text = ""
                LimitUpdown2.Text = ""
            End If

            If pMandatory Then
                If LimitUpdown1.Text = "" And LimitUpdown2.Text = "" Then
                    BsErrorProvider1.SetError(LimitUpdown1, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    BsErrorProvider1.SetError(LimitUpdown2, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If
            End If

            If LimitUpdown1.Text <> "" Or LimitUpdown2.Text <> "" Then
                If LimitUpdown1.Text = "" And LimitUpdown2.Text <> "" Then
                    BsErrorProvider1.SetError(LimitUpdown1, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If LimitUpdown2.Text = "" And LimitUpdown1.Text <> "" Then
                    BsErrorProvider1.SetError(LimitUpdown2, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
                    ValidationError = True
                End If

                If LimitUpdown2.Text <> "" And LimitUpdown1.Text <> "" Then
                    If CDbl(LimitUpdown1.Text) > CDbl(LimitUpdown2.Text) Then
                        BsErrorProvider1.SetError(LimitUpdown1, GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)) 'AG 07/07/2010("MIN_MUST_BE_LOWER_THAN_MAX"))
                        ValidationError = True
                    End If
                    If CDbl(LimitUpdown1.Text) = CDbl(LimitUpdown2.Text) Then
                        BsErrorProvider1.SetError(LimitUpdown1, GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)) 'AG 07/07/2010("MIN_MUST_BE_LOWER_THAN_MAX"))
                        ValidationError = True
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ValidateLimitsUpDown " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateLimitsUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub FactorLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                                        Handles FactorLowerLimitUpDown.Validating, FactorUpperLimitUpDown.Validating

        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            ValidateLimitsUpDown(FactorLowerLimitUpDown, FactorUpperLimitUpDown, False)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FactorLimits_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FactorLimits_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub RerunLimits_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                                        Handles RerunLowerLimitUpDown.Validating, RerunUpperLimitUpDown.Validating
        Try
            BsErrorProvider1.Clear()
            ValidationError = False
            ValidateLimitsUpDown(RerunLowerLimitUpDown, RerunUpperLimitUpDown, False)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RerunLimits_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RerunLimits_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' generic event handler to capture the NumericUpDown content deletion
    ''' </summary>
    ''' <remarks>Created by SG 18/06/2010</remarks>
    Private Sub NumericUpDown_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles RerunUpperLimitUpDown.KeyUp, _
                                    RerunLowerLimitUpDown.KeyUp, LinearityUpDown.KeyUp, FactorUpperLimitUpDown.KeyUp, FactorLowerLimitUpDown.KeyUp, _
                                    DetectionUpDown.KeyUp, BlankAbsorbanceUpDown.KeyUp, KineticBlankUpDown.KeyUp, SubstrateDepleUpDown.KeyUp, _
                                    SlopeBUpDown.KeyUp, SlopeAUpDown.KeyUp, ProzoneT2UpDown.KeyUp, ProzoneT1UpDown.KeyUp, ProzonePercentUpDown.KeyUp
        Try
            Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)

            If miNumericUpDown.Text <> "" Then

            Else
                miNumericUpDown.Value = miNumericUpDown.Minimum
                miNumericUpDown.ResetText()
            End If

            If EditionMode Then
                ChangesMade = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "NumericUpDown_KeyUp " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".NumericUpDown_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub MultipleCalibRadioButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultipleCalibRadioButton.Click
        Try
            'TR 15/06/2010 -clear the calibration factor value.
            If CalibrationFactorTextBox.Text <> "" Then
                CalibrationFactorTextBox.Clear()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "MultipleCalibRadioButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MultipleCalibRadioButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub FactorRadioButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FactorRadioButton.Click
        Try
            'TR 15/06/2010
            If FactorRadioButton.Checked Then
                BsCalibNumberTextBox.Clear() 'TR 01/07/2010
                'TR 29/06/2010 - Replace the code here by a method.
                ClearConcentrationsCalibValues()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FactorRadioButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FactorRadioButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Click over element in TestListView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/06/2010
    ''' Modified by: SA 15/11/2012 - Flag NewTest has to be set to FALSE to avoid creation of duplicated Tests when edition of a new
    '''                              Test is cancelled by clicking in an existing Tests in the list
    ''' </remarks>
    Private Sub TestListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TestListView.Click
        Try
            'NewTest = False 'TR 22/07/2013 -BUG #1130. Comment this line and uncomment the line with the same mark under.

            'DL 11/01/2012. Begin
            'UncheckAllSampleTypes()
            HideSampleTypePanel()
            'DL 11/01/2012. End
            QuerySelectedTest() 'AG 13/12/2010

            If TestListView.SelectedItems.Count = 1 Then

                NewTest = False 'TR 22/07/2013 -BUG #1130.

                'TR 10/03/2011
                If PrevSelectedTest <> SelectedTestDS.tparTests(0).TestID Then

                    PrevSelectedTest = SelectedTestDS.tparTests(0).TestID

                    ' Ini DL 14/06/2011
                    ' Select last sample type edited
                    If SelectedSampleTypeCombo.SelectedValue Is Nothing Then
                        If SelectedTestSamplesDS.tparTestSamples.Rows.Count > 0 Then
                            'SelectedSampleTypeCombo.SelectedValue = SelectedTestSamplesDS.tparTestSamples.First.SampleType.ToString
                            'End If

                            Dim listDefault As List(Of TestSamplesDS.tparTestSamplesRow) = _
                                      (From a In SelectedTestSamplesDS.tparTestSamples Where a.DefaultSampleType = True Select a).ToList

                            If listDefault.Count > 0 Then
                                SelectedSampleTypeCombo.SelectedValue = listDefault(0).SampleType.ToString
                                CurrentSampleType = listDefault(0).SampleType.ToString
                            Else
                                SelectedSampleTypeCombo.SelectedValue = SelectedTestSamplesDS.tparTestSamples.First.SampleType.ToString
                            End If
                        End If
                    End If
                    ' End DL 14/06/2011

                    If ValidateCalibratorFactoryValues(SelectedTestDS.tparTests(0).TestID, SelectedSampleTypeCombo.SelectedValue.ToString()) Then
                        ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString())
                    End If
                End If
                'TR 10/03/2011 -END

                'QuerySelectedTest() 'AG 13/12/2010 (TR 17/11/2010 )
                If Not EditionMode And Not (ChangesMade Or bsTestRefRanges.ChangesMade) Then 'AG 13/01/2011 - change ChangesMode for (ChangesMade or bsTestRefRanges.ChangesMade)
                    If (Control.ModifierKeys And Keys.Control) = Keys.Control Or (Control.ModifierKeys And Keys.Shift) = Keys.Shift Then
                        EnableDisableControls(TestDetailsTabs.Controls, False, True)    '19/03/2010 - Add 3hd parameter (optional)
                        ClearParametersArea()
                        EditButton.Enabled = False
                        AddButton.Enabled = False
                        SaveButton.Enabled = False
                        'TR 08/11/2010 -Add new button cancel.
                        CancelButton.Enabled = False

                        TestListView.MultiSelect = True
                    Else
                        'AG 09/07/2010
                        ''TestListView.MultiSelect = False 'TR 01/07/2010 comment this line to allow the multiselection all the time.
                        'If TestListView.SelectedItems.Item(0).ImageKey = "INUSETEST" Then
                        If isCurrentTestInUse Then 'TR 12/09/2011 -Validate if Preloaded test 
                            EditButton.Enabled = False
                            DeleteButton.Enabled = False
                        Else
                            EditButton.Enabled = isEditTestAllowed
                            'CopyTestButton.Enabled = True 'TR 10/01/2012 
                            'DeleteButton.Enabled = Not isPreloadedTest
                        End If
                    End If
                End If

            ElseIf TestListView.SelectedItems.Count > 1 Then
                'TR 30/11/2010 -Clear the structures.
                SelectedTestDS.tparTests.Clear()
                SelectedReagentsDS.tparReagents.Clear()
                SelectedTestSamplesDS.tparTestSamples.Clear()
                SelectedTestReagentsDS.tparTestReagents.Clear()
                SelectedTestReagentsVolumesDS.tparTestReagentsVolumes.Clear()
                SelectedTestRefRangesDS.tparTestRefRanges.Clear()
                TestDescriptionTextBox.Clear()
                'TR 30/11/2010 -END clear structures.

                EditButton.Enabled = False  'AG 09/07/2010
                'If isCurrentTestInUse Then DeleteButton.Enabled = False 'AG 09/07/2010
                If isCurrentTestInUse OrElse isPreloadedTest Then DeleteButton.Enabled = False 'TR 12/09/2011 -Validate if is Preloaded Test

                'TR 24/04/2012 -Validate iser level
                If CurrentUserLevel = "OPERATOR" Then
                    DeleteButton.Enabled = False
                End If
                'TR 24/04/2012 -END.

            End If

            'TR 19/11/2010 -Validate if there are more than one test sample
            ShowPlusSampleTypeIcon()
            'TR 19/11/2010 -END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " TestListView_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ' DL 21/06/2010
    Private Sub TestListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TestListView.KeyUp
        Try
            If TestListView.SelectedItems.Count = 1 Then

                QuerySelectedTest()

                'TR 08/03/2011 -Validate if factory calibrator values.
                Dim tmpTestID As Integer = SelectedTestDS.tparTests(0).TestID 'RH 14/05/2012
                If (PrevSelectedTest <> tmpTestID) AndAlso (ValidateCalibratorFactoryValues(tmpTestID, SelectedSampleTypeCombo.SelectedValue.ToString())) Then
                    ShowMessage("Warning", GlobalEnumerates.Messages.FACTORY_VALUES.ToString())
                    PrevSelectedTest = tmpTestID 'TR 15/05/2012 -Set the value of selected test.
                End If

                ShowPlusSampleTypeIcon()

                If isCurrentTestInUse Then 'TR 12/09/2011 -Validate if is preloaded
                    EditButton.Enabled = False
                    DeleteButton.Enabled = False
                    'Else
                    'TR 30/03/2012 
                    'EditButton.Enabled = True
                    'EditButton.Enabled = isEditTestAllowed
                    'TR 30/03/2012
                    'DeleteButton.Enabled = Not isPreloadedTest
                End If

                'CopyTestButton.Enabled = True 'TR 10/01/2012

            ElseIf TestListView.SelectedItems.Count > 1 Then
                QuerySelectedTest()  'AG 13/12/2010
                Dim bEnabled As Boolean = True
                If isCurrentTestInUse OrElse isPreloadedTest Then bEnabled = False
                EditButton.Enabled = False
                DeleteButton.Enabled = bEnabled

            End If

            'TR 24/04/2012 -Validate iser level
            If CurrentUserLevel = "OPERATOR" Then
                DeleteButton.Enabled = False
            End If
            'TR 24/04/2012 -END.

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TestListView_KeyUp " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)

            'Show error message
            ShowMessage(Name & ".TestListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 09/07/2010
    ''' Modified by: SA 09/11/2012 - Changes due to an error was raised when trying to convert to INT a "big" number that 
    '''                              was entered in the numeric UpDown
    ''' </remarks>
    Private Sub DecimalsUpDown_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DecimalsUpDown.Validated
        Try
            Dim decimalsNumber As Integer = 0
            Dim myDecimalNum As String = DecimalsUpDown.Text.ToString

            If (myDecimalNum = String.Empty) Then
                decimalsNumber = CInt(DecimalsUpDown.Minimum)
            Else
                If (myDecimalNum.Length = DecimalsUpDown.Maximum.ToString.Length) Then
                    decimalsNumber = CInt(DecimalsUpDown.Text)
                    decimalsNumber = CInt(IIf(decimalsNumber < DecimalsUpDown.Minimum, DecimalsUpDown.Minimum, decimalsNumber))
                    decimalsNumber = CInt(IIf(decimalsNumber > DecimalsUpDown.Maximum, DecimalsUpDown.Maximum, decimalsNumber))
                Else
                    decimalsNumber = CInt(DecimalsUpDown.Maximum)
                End If
            End If

            DecimalsUpDown.Text = decimalsNumber.ToString
            SetupDecimalsNumber(decimalsNumber)

            'If IsNumeric(DecimalsUpDown.Text) Then decimalsNumber = CInt(DecimalsUpDown.Text)
            ''AG 28/09/2010
            'decimalsNumber = CInt(IIf(decimalsNumber < DecimalsUpDown.Minimum, DecimalsUpDown.Minimum, decimalsNumber))
            'decimalsNumber = CInt(IIf(decimalsNumber > DecimalsUpDown.Maximum, DecimalsUpDown.Maximum, decimalsNumber))
            'DecimalsUpDown.Text = decimalsNumber.ToString
            ''END AG 28/09/2010
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "DecimalsUpDown_Validated " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecimalsUpDown_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub AlternativeCalibRadioButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlternativeCalibRadioButton.Click
        Try
            'TR 29/06/2010
            If AlternativeCalibRadioButton.Checked Then
                ClearConcentrationsCalibValues()
                AlternativeCalComboBox.Select()
            End If
            'TR 29/06/2010 -END

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AlternativeCalibRadioButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AlternativeCalibRadioButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub TestListView_ColumnWidthChanging(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles TestListView.ColumnWidthChanging
        Try
            ' DL 15/07/2010
            e.Cancel = True
            If e.ColumnIndex = 0 Then
                e.NewWidth = 203 'TR 15/11/2010 
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    Private Sub MultipleCalibRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MultipleCalibRadioButton.CheckedChanged
        Try
            'TR 29/06/2010 -Enable or disable Add calibrator Button
            If MultipleCalibRadioButton.Checked And EditionMode Then
                BsErrorProvider1.Clear()
                AddCalibratorButton.Enabled = True
                AddCalibratorButton.Select()
                AlternativeCalComboBox.Enabled = False
                CalibrationFactorTextBox.BackColor = Color.Gainsboro
                CalibrationFactorTextBox.Enabled = False

                AddCalibratorButton.Select()

                'DL 11/01/2012. Begin
                'If Not bsSampleTypeText.Text Is String.Empty Then
                If Not SampleTypeCboEx.Text Is String.Empty Then
                    'TR 01/07/2010 -Commented
                    ''TR 01/07/2010 TEST CALIBRATOR VALUES
                    BindCalibrationCurveControls(SelectedTestDS.tparTests(0).TestID, SampleTypeCboEx.Text) '  bsSampleTypeText.Text.ToString()) 'SampleTypeCombo.SelectedValue.ToString())
                    'CONCENTRATION VALUES.
                    FillConcentrationValuesList(SelectedTestDS.tparTests(0).TestID, SampleTypeCboEx.Text) ' SampleTypeCombo.SelectedValue.ToString())
                    'TR 01/07/2010 -END 
                    'TR 01/07/2010 -End Commented
                End If
                'DL 11/01/2012. End

            Else
                AddCalibratorButton.Enabled = False
            End If

            If EditionMode Then
                If ChangeSampleTypeDuringEdition Then
                    ChangesMade = False
                Else
                    ChangesMade = True
                End If 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If
            'TR 29/06/2010 -End.

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "MultipleCalibRadioButton_CheckedChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MultipleCalibRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub FactorRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FactorRadioButton.CheckedChanged
        Try
            'TR 29/06/2010 -Enable or disable Textbox
            If FactorRadioButton.Checked And EditionMode Then
                BsErrorProvider1.Clear()
                CalibrationFactorTextBox.Enabled = True
                CalibrationFactorTextBox.BackColor = Color.White
                AddCalibratorButton.Enabled = False
                CalibrationFactorTextBox.Select()

            Else
                AlternativeCalComboBox.Enabled = False
                CalibrationFactorTextBox.BackColor = Color.Gainsboro
                CalibrationFactorTextBox.Enabled = False
            End If

            If EditionMode Then
                If ChangeSampleTypeDuringEdition Then
                    ChangesMade = False

                Else
                    ChangesMade = True
                End If 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If
            'TR 29/06/2010 -End.
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "FactorRadioButton_CheckedChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FactorRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub AlternativeCalibRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AlternativeCalibRadioButton.CheckedChanged
        Try
            'TR 29/06/2010 -Load the alternative calibrator.
            If AlternativeCalibRadioButton.Checked AndAlso EditionMode Then
                AlternativeCalComboBox.Enabled = True
                CalibrationFactorTextBox.Clear()
                FillAlternativeCalibrator()
            Else
                AlternativeCalComboBox.SelectedIndex = -1
                AlternativeCalComboBox.Enabled = False
            End If

            If EditionMode Then
                If ChangeSampleTypeDuringEdition Then
                    ChangesMade = False
                Else
                    ChangesMade = True
                End If 'AG 19/07/2010 (During edition user changes sampletype. all control are bind to new values but no changes are made!!)
            End If
            'TR 29/06/2010 -End

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "AlternativeCalibRadioButton_CheckedChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AlternativeCalibRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Apply format to values in Theoretical Concentration cells
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 07/11/2012 - Theoretical Concentration values have to be format using the number of decimals defined 
    '''                              for the Test; they should not be rounded
    ''' </remarks>
    Private Sub ConcentrationGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles ConcentrationGridView.CellFormatting
        Try
            If e.ColumnIndex = 1 Then
                If Not ConcentrationGridView.Rows(e.RowIndex).Cells("Theoricalconcentration").Value Is DBNull.Value Then
                    'e.Value = Math.Round(CType(e.Value, Double), CInt(DecimalsUpDown.Value))
                    e.Value = CType(e.Value, Double).ToString("F" & DecimalsUpDown.Value.ToString)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ConcentrationGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        CancelEdition()

    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Try
            SampleTypeCheckList.Visible = False 'DL 10/01/2012 
            'AG 12/11/2010
            Dim treeListSelectedElement As Integer = -1
            If Not NewTest Then treeListSelectedElement = TestListView.SelectedItems(0).Index
            'End AG 12/11/2010

            'TR 08/11/2010 -Save directly to the database (new Implementation).
            Dim closeScreen As Boolean = False
            If EditionMode Then
                ValidateAllTabs(True) 'AG 03/12/2010 - add optional parameter 'AG 04/08/2010
                'TR 01/07/2010 -Implement the save local if there is something pending
                'If TurbidimetryCheckBox.Checked Then 'TR 07/05/2011 -commente the turbidimetri is not implemented
                'ValidateAllTabs() 'AG 04/08/2010
                If Not ValidationError Then
                    ValidateProzones(True) 'AG 05/12/2010 - add optional parameter
                End If
                'End If 'TR 07/05/2011 -END 
                'AG 08/07/2010 Change the IF order (Validate if there are any error. before saving.)
                SaveAndClose = True
                'If SaveTestLocal() Then    'AG 29/07/2010
                If Not ValidationError Then
                    'ChangesMade = False 'AG 29/07/2010
                    'Ask user for save current edition (the function returns NO when the local save fails)
                    If SavePendingWarningMessage(True) = Windows.Forms.DialogResult.No Then
                        'If current edition validation fails stop saving process
                        closeScreen = False
                        SaveAndClose = False
                    End If
                End If
            End If

            If Not ValidationError Then
                Cursor = Cursors.WaitCursor 'AG 13/10/2010
                If ReadyToSave Then
                    'Save into database              
                    If SaveChanges(True) Then ' TR 30/11/2010 -Validate the save result.
                        'RH 21/11/2011 Update bsTestRefRanges.ChangesMade = False, so no Warning message
                        'when closing the form, even having saved the changes.
                        bsTestRefRanges.ChangesMade = False

                        IsSampleTypeChanged = False
                        SelectedSampleTypeCombo.Enabled = Not IsSampleTypeChanged

                        'TR 29/03/2010 -update the position in case there is a change.
                        If TestsPositionChange Then
                            UpdateTestPosition()
                        End If

                        'TR 01/07/2010 -Implementatation SA
                        IAx00MainMDI.SetNumOfTests(TestListView.Items.Count)

                        'AG 12/11/2010
                        'If closeScreen Then Me.Close()
                        If treeListSelectedElement <> -1 Then
                            TestListView.SelectedItems.Clear()
                            TestListView.Items(treeListSelectedElement).Selected = True
                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer)) 'Bind the controls
                            TestListView.Items(treeListSelectedElement).EnsureVisible() 'TR 18/11/2010 -Make sure selecte item is visible

                        Else
                            TestListView.SelectedItems.Clear() 'TR 30/11/2010 -clear all the selection.
                            TestListView.Items(TestListView.Items.Count - 1).Selected = True
                            BindControls(CType(TestListView.SelectedItems(0).Name, Integer)) 'Bind the controls
                            TestListView.Items(TestListView.Items.Count - 1).EnsureVisible() 'TR 18/11/2010 -Make sure selecte item is visible
                        End If

                        'AG 11/01/2011 -comment TR code, try save test-sample type on change sampletype
                        'SelectedSampleTypeCombo.SelectedValue = CurrentSampleType 'TR 22/12/2010 -Commented
                        SelectedSampleTypeCombo.SelectedValue = CurrentSampleType 'Uncommented!!!

                        EnableDisableControls(TestDetailsTabs.Controls, False, True) 'TR 30/11/2010 Enable and disable controls after save

                        SampleTypeCboAux.Enabled = True

                        'AG 11/01/2011 -comment TR code, try save test-sample type on change sampletype
                        'CurrentSampleType = "" 'TR 22/12/2010 Set current type to empty
                    End If
                Else
                    'CancelEdition() 'TR 03/12/2010 -Cancel an reload previous values.
                    'TR 09/12/2010
                    ChangesMade = False
                    EditionMode = False
                    BsErrorProvider1.Clear()
                    If TestListView.SelectedItems.Count > 0 Then
                        UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.Clear()
                        UpdatedCalibratorsDS.tparCalibrators.Clear()
                        UpdatedTestCalibratorDS.tparTestCalibrators.Clear()
                        LocalDeleteControlTOList.Clear() 'TR 24/05/2011

                        BindControls(CType(TestListView.SelectedItems(0).Name, Integer))
                        EnableDisableControls(TestDetailsTabs.Controls, False, True)
                    End If
                    SelectedSampleTypeCombo.Visible = True
                    If NewTest Then
                        NewTest = False
                    End If
                    'TR 09/12/2010 -END.
                End If
                Cursor = Cursors.Default  'AG 13/10/2010

                'TR 18/11/2010 -Enable Controls
                SelectedSampleTypeCombo.Visible = True
                'TR 18/11/2010 -END

            End If
            'END AG 19/03/2010

            'TR 26/11/2010 -Validate if there are more than one test sample
            ShowPlusSampleTypeIcon()
            'TR 26/11/2010 -END
            'EnableDisableControls(TestDetailsTabs.Controls, False, True) 'TR 30/11/2010 Enable and disable controls after save

            AddedSampleType = String.Empty 'DL 01/11/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SaveButton_Click " & Name, EventLogEntryType.Error, _
                              GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

            Cursor = Cursors.Default  'AG 13/10/2010
        End Try
    End Sub

    Private Sub ProgTest_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        If Not Me.DesignMode Then
            'TR 10/03/2011 -Validate fist element on test list
            If TestListView.Items.Count > 0 AndAlso TestListView.SelectedItems.Count > 0 Then
                PrevSelectedTest = SelectedTestDS.tparTests(0).TestID
                'Validate if factory calibrator values.
                If ValidateCalibratorFactoryValues(SelectedTestDS.tparTests(0).TestID, SelectedSampleTypeCombo.SelectedValue.ToString()) Then
                    ShowMessage("Warning", "FACTORY_VALUES")
                End If
            End If
            'TR 10/03/2011 -END.
        End If
    End Sub

    Private Sub SelectedSampleTypeCombo_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedSampleTypeCombo.VisibleChanged
        'TR 19/11/2010
        SampleTypePlus2.Visible = SelectedSampleTypeCombo.Visible
    End Sub

    'Private Sub SampleTypeCombo_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    'TR 19/11/2010
    'SampleTypePlus.Visible = SampleTypeCombo.Enabled
    'End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub EditingGridComboBox_MouseWeel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim mwe As HandledMouseEventArgs = DirectCast(e, HandledMouseEventArgs)
        mwe.Handled = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' 
    ''' MODIFIED BY: TR 24/07/2013 -Bug #1158 Error selecting controls. change the selected value validationd from >= 0 to 
    '''                             > 0, becuase 0 is the empty value on the combobox.
    ''' </remarks>
    Private Sub editingGridComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim myControlDS As New ControlsDS
        Dim myComboBoxCell As ComboBox = CType(sender, ComboBox)
        'clear errors.
        UsedControlsGridView.CurrentRow.Cells("ControlName").ErrorText = String.Empty

        BsErrorProvider1.Clear()
        Try
            If EditionMode Then
                'TR 24/07/2013 -bug #1158 Validate if selected index is > than 0 because 0 is the empty value on the combo.
                If Not myComboBoxCell.SelectedValue Is Nothing AndAlso myComboBoxCell.SelectedIndex > 0 AndAlso _
                                                            ControIDSel <> myComboBoxCell.SelectedValue.ToString() Then
                    'Validate control do not exist.
                    If Not ControlExist(CInt(myComboBoxCell.SelectedValue)) Then
                        'Get Selected Control information.
                        myControlDS = GetQCControlInfo(CInt(myComboBoxCell.SelectedValue))
                        If myControlDS.tparControls.Count > 0 Then
                            UsedControlsGridView.BeginEdit(False)
                            'Set the values for lot number and Exp. Date
                            UsedControlsGridView.CurrentRow.Cells("LotNumber").Value = myControlDS.tparControls(0).LotNumber
                            UsedControlsGridView.CurrentRow.Cells("ExpDate").Value = myControlDS.tparControls(0).ExpirationDate
                            UsedControlsGridView.CurrentRow.Cells("TestID").Value = currentTestViewEditionItem 'set the test id 
                            'Set the sample type

                            'DL 11/01/2012. Begin
                            'UsedControlsGridView.CurrentRow.Cells("SampleType").Value = SampleTypeCombo.Text
                            'UsedControlsGridView.CurrentRow.Cells("SampleType").Value = bsSampleTypeText.Text
                            UsedControlsGridView.CurrentRow.Cells("SampleType").Value = SampleTypeCboEx.Text
                            'DL 11/01/2012. End

                            'Clear calculated values 
                            UsedControlsGridView.CurrentRow.Cells("MinConcentration").Value = DBNull.Value
                            UsedControlsGridView.CurrentRow.Cells("MaxConcentration").Value = DBNull.Value
                            UsedControlsGridView.CurrentRow.Cells("TargetMean").Value = DBNull.Value
                            UsedControlsGridView.CurrentRow.Cells("TargetSD").Value = DBNull.Value
                            UsedControlsGridView.EndEdit()
                            ValidationError = False
                            ChangesMade = True 'TR 18/05/2011
                        End If
                    Else
                        UsedControlsGridView.CurrentRow.Cells("ControlName").Value = DBNull.Value
                        'Clear the values for lot number and Exp. Date
                        UsedControlsGridView.CurrentRow.Cells("LotNumber").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("ExpDate").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("SampleType").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("ControlID").Value = DBNull.Value
                        'Clear calculated values 
                        UsedControlsGridView.CurrentRow.Cells("MinConcentration").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("MaxConcentration").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("TargetMean").Value = DBNull.Value
                        UsedControlsGridView.CurrentRow.Cells("TargetSD").Value = DBNull.Value
                        'Show message indicating that control exist and set the cell value to null
                        UsedControlsGridView.CurrentRow.Cells("ControlName").ErrorText = GetMessageText(GlobalEnumerates.Messages.CONTROL_AREADYSEL.ToString())
                        ValidationError = True
                        UsedControlsGridView.CancelEdit()
                        UsedControlsGridView.CurrentRow.Selected = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " editingGridComboBox_SelectionChangeCommitted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles UsedControlsGridView.CellPainting
        Try
            If e.RowIndex >= 0 AndAlso (e.ColumnIndex = 2 OrElse e.ColumnIndex = 3 OrElse _
                                    e.ColumnIndex = 6 OrElse e.ColumnIndex = 7) Then
                e.CellStyle.BackColor = SystemColors.MenuBar
                e.CellStyle.ForeColor = Color.DarkGray   'Color.bLACK DL 19/07/2011

            ElseIf e.RowIndex >= 0 AndAlso (e.ColumnIndex = 0 OrElse e.ColumnIndex = 1 OrElse _
                                                e.ColumnIndex = 4 OrElse e.ColumnIndex = 5) Then
                'DL 19/07/2011. Begin
                If UsedControlsGridView.Columns("ControlName").Index = e.ColumnIndex Then
                    Dim dgvCbo As DataGridViewComboBoxCell = TryCast(UsedControlsGridView(e.ColumnIndex, e.RowIndex), DataGridViewComboBoxCell)

                    If EditionMode AndAlso QCActiveCheckBox.Checked Then
                        dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                        dgvCbo.ReadOnly = False
                        dgvCbo.Style.BackColor = Color.White
                        dgvCbo.Style.ForeColor = Color.Black
                    Else
                        dgvCbo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                        dgvCbo.ReadOnly = True
                        dgvCbo.Style.BackColor = SystemColors.MenuBar
                        dgvCbo.Style.ForeColor = Color.DarkGray
                    End If

                    'DL 19/07/2011. End
                Else
                    If EditionMode AndAlso QCActiveCheckBox.Checked Then
                        e.CellStyle.BackColor = Color.White
                        e.CellStyle.ForeColor = Color.Black
                        e.CellStyle.SelectionBackColor = Color.LightSlateGray
                    Else
                        e.CellStyle.BackColor = SystemColors.MenuBar
                        e.CellStyle.ForeColor = Color.DarkGray
                    End If
                End If


            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles UsedControlsGridView.EditingControlShowing
        Try
            'Dim currentRow As Integer = UsedControlsGridView.CurrentCell.RowIndex
            Dim currentCol As Integer = UsedControlsGridView.CurrentCell.ColumnIndex

            If (currentCol = 1) Then
                Dim editingComboBox As ComboBox = CType(e.Control, ComboBox)
                editingComboBox.DropDownStyle = ComboBoxStyle.DropDownList
                If (Not editingComboBox Is Nothing) Then
                    RemoveHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted
                    AddHandler editingComboBox.SelectionChangeCommitted, AddressOf editingGridComboBox_SelectionChangeCommitted

                    RemoveHandler editingComboBox.KeyDown, AddressOf UsedControlsGridView_KeyDown
                    AddHandler editingComboBox.KeyDown, AddressOf UsedControlsGridView_KeyDown

                    'Disable the mouse wheel 
                    RemoveHandler editingComboBox.MouseWheel, AddressOf EditingGridComboBox_MouseWeel
                    AddHandler editingComboBox.MouseWheel, AddressOf EditingGridComboBox_MouseWeel
                End If
            End If

            'Validate columns 4 and 5
            If Me.UsedControlsGridView.CurrentRow.Index >= 0 AndAlso _
              (Me.UsedControlsGridView.CurrentCell.ColumnIndex = 4 OrElse _
                    Me.UsedControlsGridView.CurrentCell.ColumnIndex = 5) Then
                'TR 01/07/2011 -Dont allow to show copy/paste/Cut menu on cell
                If e.Control.GetType().Name = "DataGridViewTextBoxEditingControl" Then
                    DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False
                End If

                AddHandler e.Control.KeyPress, AddressOf CheckNumericCell

            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckNumericCell

            End If
            'TR 28/07/2011 -Commented 
            'ChangesMade = True

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_CellLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.CellEndEdit
        Try
            If EditionMode Then
                If Not UsedControlsGridView.Rows(e.RowIndex).Cells("ControlName").Value Is DBNull.Value AndAlso _
                        Not UsedControlsGridView.Rows(e.RowIndex).Cells("ControlName").Value Is Nothing _
                        AndAlso Not UsedControlsGridView.Rows(e.RowIndex).Cells("ControlName").Value.ToString() = String.Empty Then
                    If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 Then
                        SetLimitValues(e.RowIndex, e.ColumnIndex)
                        'Calculate the values for Target (Mean/SD) if (Mean min or max change) SD (min or max or KSD change)
                        CalculatedTargets(e.RowIndex, e.ColumnIndex)
                    End If
                End If
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellLeave ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub UsedControlsGridView_RowValidating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles UsedControlsGridView.RowValidating
        Try
            If EditionMode Then
                If ValidateErrorOnQCUsedControls() Then
                    'UsedControlsGridView.CancelEdit()
                    'e.Cancel = True 'TR 19/04/2012
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_RowValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.CellClick
        Try
            'If e.RowIndex > 0 Then
            '    If EditionMode AndAlso Not UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value Is DBNull.Value _
            '    AndAlso Not UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value Is Nothing Then
            '        ControIDSel = UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value.ToString()
            '    Else
            '        ControIDSel = ""
            '    End If
            'End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellClick ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles UsedControlsGridView.CellValueChanged
        Try
            If e.ColumnIndex = 0 AndAlso EditionMode Then

                If CountActiveControl() > 3 Then
                    UsedControlsGridView.CurrentCell.Value = CBool(False)
                End If
                ''Validate the number of active controls.
                'If ValidateErrorOnQCUsedControls() Then 'TR 06/03/2012 -Validate if error before countactiveControl
                '    UsedControlsGridView.CancelEdit()
                'End If

            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellValueChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Format the cell values for Target values, Min And Max.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub UsedControlsGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles UsedControlsGridView.CellFormatting
        Try
            If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 OrElse e.ColumnIndex = 6 Then
                If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
                    e.Value = DirectCast(e.Value, Single).ToString("F" & DecimalsUpDown.Value)
                End If
            ElseIf e.ColumnIndex = 7 Then
                If Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing Then
                    'TR 10/05/2011  -Show the decimals allowed + 1 
                    e.Value = DirectCast(e.Value, Single).ToString("F" & (CInt(DecimalsUpDown.Value) + 1))
                End If
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_CellFormatting ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub QCActiveCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCActiveCheckBox.CheckedChanged
        Try
            If EditionMode Then
                ActivateQCControlsByQCActive(QCActiveCheckBox.Checked)
                ChangesMade = True
                'Set Default Values for QC Area.
                If NewTest AndAlso QCActiveCheckBox.Checked Then
                    QCReplicNumberNumeric.Text = "1"
                    QCRejectionCriteria.Text = "3"
                    ManualRadioButton.Checked = True
                    StaticRadioButton.Checked = False
                    QCErrorAllowable.Value = QCErrorAllowable.Minimum
                    QCErrorAllowable.ResetText()
                ElseIf NewTest AndAlso Not QCActiveCheckBox.Checked Then
                    QCReplicNumberNumeric.Value = QCReplicNumberNumeric.Minimum
                    QCReplicNumberNumeric.ResetText()
                    QCRejectionCriteria.Value = QCRejectionCriteria.Minimum
                    QCRejectionCriteria.ResetText()
                    ManualRadioButton.Checked = False
                    StaticRadioButton.Checked = False
                    QCErrorAllowable.Value = QCErrorAllowable.Minimum
                    QCErrorAllowable.ResetText()
                ElseIf QCActiveCheckBox.Checked Then
                    'Validate if has value else assigned
                    If QCReplicNumberNumeric.Text = "" Then
                        QCReplicNumberNumeric.Text = "1"
                    End If

                    'TR 20/01/2012 -Validate if the statistic RB is checked 
                    If Not StaticRadioButton.Checked Then
                        ManualRadioButton.Checked = True
                    End If
                    'TR 20/01/2012 -END.

                    If QCRejectionCriteria.Text = "" Then
                        QCRejectionCriteria.Text = "3"
                    End If
                End If
                'TR 12/04/2011 -END

                UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Checked

            Else
                ActivateQCControlsByQCActive(False)
                UsedControlsGridView.AllowUserToAddRows = False
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " QCActiveCheckBox_CheckedChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)

            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub ManualRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ManualRadioButton.CheckedChanged
        Try
            If ManualRadioButton.Checked Then
                QCMinNumSeries.ResetText()
                QCMinNumSeries.Enabled = False
            Else
                QCMinNumSeries.Enabled = True
            End If

            If EditionMode Then
                ChangesMade = True
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ManualRadioButton_CheckedChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub StaticRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaticRadioButton.CheckedChanged
        Try
            QCMinNumSeries.Enabled = StaticRadioButton.Checked
            If EditionMode Then
                ChangesMade = True
                If NewTest AndAlso StaticRadioButton.Checked Then
                    'Set default value.
                    QCMinNumSeries.Value = 10
                ElseIf NewTest AndAlso Not StaticRadioButton.Checked Then
                    QCMinNumSeries.Value = QCMinNumSeries.Minimum
                    QCMinNumSeries.ResetText()
                Else
                    'Set default value.
                    QCMinNumSeries.Text = "10"
                End If

            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " StaticRadioButton_CheckedChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub RulesToApplyCheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles s12CheckBox.CheckedChanged, _
                                                                                x22CheckBox.CheckedChanged, x10CheckBox.CheckedChanged, _
                                                                                s41CheckBox.CheckedChanged, s13CheckBox.CheckedChanged, _
                                                                                r4sCheckBox.CheckedChanged
        If EditionMode Then
            ChangesMade = True
        End If
    End Sub

    Private Sub QCRejectionCriteria_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCRejectionCriteria.ValueChanged
        Try
            If EditionMode Then
                ChangesMade = True
                'Recalculate targets
                RecalculateAllTarget()
            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " QCRejectionCriteria_ValueChanged ", _
                                            EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub QCReplicNumberNumeric_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCReplicNumberNumeric.Validating
        BsErrorProvider1.Clear()
        If QCReplicNumberNumeric.Text = "" Then
            BsErrorProvider1.SetError(QCReplicNumberNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            QCReplicNumberNumeric.Value = QCReplicNumberNumeric.Minimum
            QCReplicNumberNumeric.Focus()
        End If
    End Sub

    Private Sub QCRejectionCriteria_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCRejectionCriteria.Validating
        BsErrorProvider1.Clear()
        If QCRejectionCriteria.Text = "" Then
            BsErrorProvider1.SetError(QCRejectionCriteria, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            QCRejectionCriteria.Value = QCRejectionCriteria.Minimum
            QCRejectionCriteria.Focus()
        End If
    End Sub

    Private Sub QCMinNumSeries_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles QCMinNumSeries.Validating
        BsErrorProvider1.Clear()
        If (StaticRadioButton.Checked) AndAlso (String.Compare(QCMinNumSeries.Text, String.Empty, False) = 0) Then
            BsErrorProvider1.SetError(QCMinNumSeries, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            'QCMinNumSeries.Value = QCMinNumSeries.Minimum
            QCMinNumSeries.Focus()
        End If

    End Sub

    Private Sub QCActiveCheckBox_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCActiveCheckBox.EnabledChanged
        If QCActiveCheckBox.Enabled Then
            ActivateQCControlsByQCActive(QCActiveCheckBox.Checked)
            UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Checked
        Else
            ActivateQCControlsByQCActive(False)
            UsedControlsGridView.AllowUserToAddRows = QCActiveCheckBox.Enabled
        End If
    End Sub

    Private Sub DeleteControlButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteControlButton.Click
        DeleteSelectedControl()
    End Sub

    Private Sub StaticRadioButton_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StaticRadioButton.EnabledChanged
        If StaticRadioButton.Enabled AndAlso StaticRadioButton.Checked Then
            QCMinNumSeries.Enabled = StaticRadioButton.Checked
        Else
            QCMinNumSeries.Enabled = StaticRadioButton.Checked
        End If
    End Sub

    Private Sub AddControls_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddControls.Click
        Try
            'Open controls Programming
            Using myProgControls As New IProgControls
                myProgControls.AnalyzerID = AnalyzerIDAttribute
                myProgControls.WorkSessionID = WorkSessionIDAttribute
                myProgControls.SourceScreen = GlbSourceScreen.TEST_QCTAB.ToString
                myProgControls.StartPosition = FormStartPosition.CenterScreen
                'myProgControls.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedToolWindow

                'RH 20/06/2012 This is the right value
                'It is the used througout the application.
                'It enables the aplication to show it's icon when the user presses Alt + Tab. The other one not.
                myProgControls.FormBorderStyle = FormBorderStyle.FixedDialog

                myProgControls.Tag = "Put something here before showing, so the form will execute its normal Close()"
                myProgControls.ShowDialog()
            End Using

            'Reload the control list to get the new data.
            ControlListDS = GetAllQCControls()
            'Asigned the Datasource to the grid column to reload the data.
            DirectCast(UsedControlsGridView.Columns(1), DataGridViewComboBoxColumn).DataSource = ControlListDS.tparControls

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " AddControls_Click ", EventLogEntryType.Error, _
                                                        GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub UsedControlsGridView_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles UsedControlsGridView.KeyDown
        Try
            If sender.GetType.Name = "DataGridViewComboBoxEditingControl" Then
                '(BUG) implent this to avoi that selected control change to the first control on the list
                UsedControlsGridView.CancelEdit()
            End If
            If e.KeyCode = Keys.Delete Then
                DeleteSelectedControl()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UsedControlsGridView_KeyDown ", EventLogEntryType.Error, _
                                                       GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub ProzoneT1UpDown_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ProzoneT1UpDown.KeyDown
        If EditionMode AndAlso e.KeyCode = Keys.Delete Then
            ChangesMade = True
        End If
    End Sub


    Private Sub ProzoneT2UpDown_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ProzoneT2UpDown.KeyDown
        If EditionMode AndAlso e.KeyCode = Keys.Delete Then
            ChangesMade = True
        End If
    End Sub

    Private Sub ProzonePercentUpDown_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ProzonePercentUpDown.KeyDown
        If EditionMode AndAlso e.KeyCode = Keys.Delete Then
            ChangesMade = True
        End If
    End Sub

    Private Sub UsedControlsGridView_CellValidating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles UsedControlsGridView.CellValidating

        Try
            If EditionMode Then
                If e.ColumnIndex = 4 OrElse e.ColumnIndex = 5 Then
                    If UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value Is DBNull.Value AndAlso _
                       UsedControlsGridView.Rows(e.RowIndex).Cells(1).Value.ToString() = String.Empty Then
                        'Dim i As Integer = 0
                    End If

                    If Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso _
                            e.FormattedValue.ToString() <> "" AndAlso Not IsNumeric(e.FormattedValue) Then
                        e.Cancel = True
                    End If
                ElseIf e.ColumnIndex = 1 Then
                    If Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso _
                            e.FormattedValue.ToString() = "" Then
                        UsedControlsGridView.CancelEdit()
                    End If
                End If

            End If
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UsedControlsGridView_CellValidating ", EventLogEntryType.Error, _
                                                        GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Validate if the value entered is numeric. Do not allow string values, only numeric values. 
    ''' Validate also the decimal separator
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2011
    ''' </remarks>
    Private Sub CheckNumericCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

            'TR 21/02/201 -Validate the back key press.
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)

                    If (CType(sender, TextBox).Text.Contains(",") OrElse CType(sender, TextBox).Text.Contains(".")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    If (Not IsNumeric(e.KeyChar)) Then
                        e.Handled = True
                    End If
                End If
            End If

            'TR 28/07/2011 -Set the change made if is in edition mode
            If (EditionMode And Not ChangesMade And Not e.KeyChar = ChrW(Keys.Tab) And Not e.KeyChar = ChrW(Keys.Back)) Then
                ChangesMade = True
            End If
            'TR 28/07/2011 -END.
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CheckCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CopyTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyTestButton.Click
        Try
            'Block all Controls and enable only the Name and Short Name.
            EnableDisableControls(TestDetailsTabs.Controls, False)
            EnableDisableControls(BsPanel1.Controls, False)
            VolR2UpDown.Enabled = False '

            Me.TestNameTextBox.Text = ""
            Me.ShortNameTextBox.Text = ""

            Me.SaveButton.Enabled = True
            Me.CancelButton.Enabled = True
            Me.TestNameTextBox.Enabled = True
            Me.CopyTestButton.Enabled = False 'disable the copy button
            Me.ShortNameTextBox.Enabled = True

            SelectedSampleTypeCombo.Enabled = False 'TR 26/01/2012 

            'Select General Tab.
            TestDetailsTabs.SelectTab("GeneralTab")

            'Focus on Test name control. 
            Me.TestNameTextBox.Focus()

            EditionMode = True
            NewTest = True
            'Update Elements like TestCalibrators and Calibrators values.
            CopySelectedTestValues()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CopyTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub SelectedSampleTypeCombo_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectedSampleTypeCombo.EnabledChanged
        If SelectedSampleTypeCombo.Enabled Then
            SelectedSampleTypeCombo.BackColor = Color.White
        Else
            SelectedSampleTypeCombo.BackColor = SystemColors.MenuBar
        End If
    End Sub

    ''' <summary>
    ''' To avoid entered following characters in the Numeric Up/Down allowing only integer numbers greater than zero:
    '''   ** Minus sign
    '''   ** Dot, Apostrophe or Comma as decimal point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/11/2012
    ''' </remarks>
    Private Sub IntegerNumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles DecimalsUpDown.KeyPress, _
                                                                                                                                        ReplicatesUpDown.KeyPress, _
                                                                                                                                        VolR1UpDown.KeyPress, _
                                                                                                                                        VolR2UpDown.KeyPress, _
                                                                                                                                        FirstReadingSecUpDown.KeyPress, _
                                                                                                                                        SecondReadingSecUpDown.KeyPress, _
                                                                                                                                        FirstReadingCycleUpDown.KeyPress, _
                                                                                                                                        SecondReadingCycleUpDown.KeyPress, _
                                                                                                                                        BlankReplicatesUpDown.KeyPress, _
                                                                                                                                        CalReplicatesUpDown.KeyPress, _
                                                                                                                                        QCReplicNumberNumeric.KeyPress, _
                                                                                                                                        QCMinNumSeries.KeyPress, _
                                                                                                                                        WashSolVolUpDown.KeyPress

        Try
            If (e.KeyChar = CChar("-") OrElse e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = "'") Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IntegerNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IntegerNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Handle for NumericUpDown controls allowing numbers with decimals. Only numbers and the decimal separator are allowed
    ''' </summary>
    ''' <remarks>
    '''    Modified by: TR 18/11/2011 - Added more elements to the Handle
    '''              SA 09/11/2012 - Implementation changed
    '''              TR 09/10/2013 -Bug #1315 allow too introduce  (-)minus on SlopeFactor A  B. 
    '''                             to allow negative values introduce by key press.
    ''' </remarks>
    Private Sub RealNumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles SampleVolUpDown.KeyPress, _
                                                                                                                                     QCRejectionCriteria.KeyPress, _
                                                                                                                                     BlankAbsorbanceUpDown.KeyPress, _
                                                                                                                                     KineticBlankUpDown.KeyPress, _
                                                                                                                                     LinearityUpDown.KeyPress, _
                                                                                                                                     DetectionUpDown.KeyPress, _
                                                                                                                                     FactorLowerLimitUpDown.KeyPress, _
                                                                                                                                     FactorUpperLimitUpDown.KeyPress, _
                                                                                                                                     RerunLowerLimitUpDown.KeyPress, _
                                                                                                                                     RerunUpperLimitUpDown.KeyPress, _
                                                                                                                                     ProzonePercentUpDown.KeyPress, _
                                                                                                                                     SlopeAUpDown.KeyPress, _
                                                                                                                                     SlopeBUpDown.KeyPress, _
                                                                                                                                     SubstrateDepleUpDown.KeyPress

        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)

                    If (CType(sender, BSNumericUpDown).Text.Contains(".") Or CType(sender, BSNumericUpDown).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    'TR 09/10/2013 -BUG #1315 -Validate if slope controls to allow the -
                    If (CType(sender, BSNumericUpDown).Name = "SlopeAUpDown" OrElse CType(sender, BSNumericUpDown).Name = "SlopeBUpDown") Then
                        'Validate if it's a numeric value and not the - character used to indicate negative values.
                        If (Not IsNumeric(e.KeyChar) AndAlso Not e.KeyChar = "-") Then
                            e.Handled = True
                        ElseIf e.KeyChar = "-" Then
                            'Allow only one -Simbol on control
                            If CType(sender, BSNumericUpDown).Text.Contains("-") Then
                                e.Handled = True
                            End If
                        End If
                        'TR 09/10/2013 -BUG #1315 -END.
                    Else
                        If (Not IsNumeric(e.KeyChar)) Then
                            e.Handled = True
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RealNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RealNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ProzoneTUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ProzoneT1UpDown.KeyPress, _
                                                                                                                                  ProzoneT2UpDown.KeyPress
        Try
            e.Handled = True
            If (EditionMode AndAlso e.KeyChar = CChar(CChar(CStr(Keys.Delete)))) Then
                ChangesMade = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProzoneTUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProzoneTUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Modified by: SA 09/11/2012 - Implementation changed
    ''' </remarks>
    Private Sub TextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles PredilutionFactorTextBox.KeyPress, _
                                                                                                                           RedPostDilutionFactorTextBox.KeyPress, _
                                                                                                                           IncPostDilutionFactorTextBox.KeyPress, _
                                                                                                                           CalibrationFactorTextBox.KeyPress
        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)

                    If (CType(sender, BSTextBox).Text.Contains(".") Or CType(sender, BSTextBox).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    If (Not IsNumeric(e.KeyChar)) Then
                        e.Handled = True
                    End If
                End If
            End If

            'If Char.IsControl(e.KeyChar) Then
            '    e.Handled = False
            'Else
            '    If e.KeyChar = "." Or e.KeyChar = "," Then
            '        If CType(sender, BSTextBox).DecimalsValues Then
            '            e.KeyChar = CChar(SystemInfoManager.OSDecimalSeparator)
            '            If CType(sender, BSTextBox).Text.Contains(".") Or CType(sender, BSTextBox).Text.Contains(",") Then
            '                e.Handled = True
            '            End If
            '        Else
            '            e.Handled = True
            '        End If
            '    End If
            'End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TextBox_KeyPress " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TextBox_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Open the customize order and availability for profiles tests selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 05/09/2014 - BA-1869</remarks>
    Private Sub BsCustomOrderButton_Click(sender As Object, e As EventArgs) Handles BsCustomOrderButton.Click
        Try
            'Shown the Positioning Warnings Screen
            Using AuxMe As New ISortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "STD"
                AuxMe.ShowDialog()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsCustomOrderButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCustomOrderButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "CONTROLS"

    ''' <summary>
    ''' Initialize the QC Tab
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2011
    ''' Modified by: RH 17/11/2011 - Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter
    ''' </remarks>
    Private Sub PrepareQCTab()
        Try
            SetQCLimits()

            QCActiveCheckBox.Checked = False

            QCReplicNumberNumeric.Text = ""
            QCReplicNumberNumeric.Value = QCReplicNumberNumeric.Minimum
            QCReplicNumberNumeric.ResetText()

            QCRejectionCriteria.Text = ""
            QCRejectionCriteria.Value = QCRejectionCriteria.Minimum
            QCRejectionCriteria.ResetText()

            QCMinNumSeries.Value = QCMinNumSeries.Minimum
            QCMinNumSeries.ResetText()

            ManualRadioButton.Checked = False
            StaticRadioButton.Checked = False

            QCErrorAllowable.Value = QCErrorAllowable.Minimum
            QCErrorAllowable.ResetText()

            's12CheckBox.Checked = False
            s13CheckBox.Checked = False
            x22CheckBox.Checked = False
            r4sCheckBox.Checked = False
            s41CheckBox.Checked = False
            x10CheckBox.Checked = False

            PrepareUsedControlsGrid()
            UsedControlsGridView.DataSource = SelectedTestControlDS.tparTestControls
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareQCTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareQCTab ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure the grid of linked Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/04/2011 
    ''' Modified by: RH 17/11/2011 - Remove pLanguageID, because it is a class property/field. There is no need to pass it as a parameter
    ''' </remarks>
    Private Sub PrepareUsedControlsGrid()
        Try
            UsedControlsGridView.DataSource = Nothing
            UsedControlsGridView.Columns.Clear()
            UsedControlsGridView.Rows.Clear()
            UsedControlsGridView.AutoGenerateColumns = False
            UsedControlsGridView.AllowUserToDeleteRows = False
            UsedControlsGridView.AllowUserToResizeColumns = True
            UsedControlsGridView.RowsDefaultCellStyle.SelectionForeColor = Color.Black
            UsedControlsGridView.EditMode = DataGridViewEditMode.EditOnEnter

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim ActiveControlColChkBox As New DataGridViewCheckBoxColumn
            ActiveControlColChkBox.Width = 20
            ActiveControlColChkBox.Name = "ActiveControl"
            ActiveControlColChkBox.HeaderText = ""
            ActiveControlColChkBox.DataPropertyName = "ActiveControl"
            ActiveControlColChkBox.Resizable = DataGridViewTriState.False
            UsedControlsGridView.Columns.Add(ActiveControlColChkBox)


            Dim ControlNameComboBoxRow As New DataGridViewComboBoxColumn
            ControlNameComboBoxRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", currentLanguage)
            ControlNameComboBoxRow.Name = "ControlName"
            ControlNameComboBoxRow.DataPropertyName = "ControlID"
            ControlNameComboBoxRow.Width = 140
            ControlNameComboBoxRow.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            ControlNameComboBoxRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Get the data to show on combo
            ControlListDS = GetAllQCControls()
            ControlNameComboBoxRow.DataSource = ControlListDS.tparControls
            ControlNameComboBoxRow.DisplayMember = "ControlName"
            ControlNameComboBoxRow.ValueMember = "ControlID"
            ControlNameComboBoxRow.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            UsedControlsGridView.Columns.Add(ControlNameComboBoxRow)

            Dim LotNumberColumn As New DataGridViewTextBoxColumn
            LotNumberColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", currentLanguage)
            LotNumberColumn.Name = "LotNumber"
            LotNumberColumn.DataPropertyName = "LotNumber"
            LotNumberColumn.ReadOnly = True
            LotNumberColumn.Width = 90
            LotNumberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            UsedControlsGridView.Columns.Add(LotNumberColumn)

            Dim ExpirationDateRow As New DataGridViewTextBoxColumn
            ExpirationDateRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", currentLanguage)
            ExpirationDateRow.Name = "ExpDate"
            ExpirationDateRow.DataPropertyName = "ExpirationDate"

            ExpirationDateRow.ReadOnly = True
            ExpirationDateRow.MaxInputLength = 10
            ExpirationDateRow.Width = 80
            UsedControlsGridView.Columns.Add(ExpirationDateRow)
            UsedControlsGridView.Columns("ExpDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            UsedControlsGridView.Columns("ExpDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            UsedControlsGridView.Columns("ExpDate").DefaultCellStyle.Format = "dd'/'MM'/'yyyy"

            Dim MinRow As New DataGridViewTextBoxColumn
            MinRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Min", currentLanguage)
            MinRow.MaxInputLength = 11
            MinRow.Name = "MinConcentration"
            MinRow.DataPropertyName = "MinConcentration"
            MinRow.Width = 62
            UsedControlsGridView.Columns.Add(MinRow)
            UsedControlsGridView.Columns("MinConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns("MinConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim MaxRow As New DataGridViewTextBoxColumn
            MaxRow.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Max", currentLanguage)
            MaxRow.Name = "MaxConcentration"
            MaxRow.MaxInputLength = 11
            MaxRow.DataPropertyName = "MaxConcentration"
            MaxRow.Width = 62
            UsedControlsGridView.Columns.Add(MaxRow)
            UsedControlsGridView.Columns("MaxConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns("MaxConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim TargetMean As New DataGridViewTextBoxColumn
            TargetMean.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetMean", currentLanguage)
            TargetMean.Name = "TargetMean"
            TargetMean.DataPropertyName = "TargetMean"
            TargetMean.ReadOnly = True
            TargetMean.Width = 84
            TargetMean.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            TargetMean.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns.Add(TargetMean)

            Dim TargetSD As New DataGridViewTextBoxColumn
            TargetSD.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetSD", currentLanguage)
            TargetSD.Name = "TargetSD"
            TargetSD.DataPropertyName = "TargetSD"
            TargetSD.ReadOnly = True
            TargetSD.Width = 74
            TargetSD.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            TargetSD.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Columns.Add(TargetSD)

            Dim ControlID As New DataGridViewTextBoxColumn
            ControlID.Name = "ControlID"
            ControlID.Visible = False
            ControlID.DataPropertyName = "ControlID"
            UsedControlsGridView.Columns.Add(ControlID)

            Dim SampleTypeCol As New DataGridViewTextBoxColumn
            SampleTypeCol.Name = "SampleType"
            SampleTypeCol.Visible = False
            SampleTypeCol.DataPropertyName = "SampleType"
            UsedControlsGridView.Columns.Add(SampleTypeCol)

            Dim TestIDTypeCol As New DataGridViewTextBoxColumn
            TestIDTypeCol.Name = "TestID"
            TestIDTypeCol.Visible = False
            TestIDTypeCol.DataPropertyName = "TestID"
            UsedControlsGridView.Columns.Add(TestIDTypeCol)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareUsedControlsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareUsedControlsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the limits defined for all QC fields and set them to the screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2011
    ''' </remarks>
    Private Sub SetQCLimits()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS

            'Control Replicates
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_REPLICATES)
            If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
                QCReplicNumberNumeric.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCReplicNumberNumeric.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    QCReplicNumberNumeric.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Rejection Criteria
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_REJECTION)
            If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
                QCRejectionCriteria.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCRejectionCriteria.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    QCRejectionCriteria.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsDecimalsAllowedNull) Then
                    QCRejectionCriteria.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
                End If
            End If

            'Minimum Number of Series
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_MIN_NUM_SERIES)
            If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
                QCMinNumSeries.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                QCMinNumSeries.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)

                If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                    QCMinNumSeries.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                End If
            End If

            'Min/Max Concentration values
            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROL_MIN_MAX_CONC)
            If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                MinAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MinValue
                MaxAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MaxValue
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetQCLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetQCLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get all programmed Controls - Used to load the ComboBox of Controls in the DataGrid
    ''' </summary>
    ''' <returns>Typed DataSet ControlsDS containing all existing Controls</returns>
    ''' <remarks>
    ''' Created by:  TR 07/04/2011
    ''' </remarks>
    Private Function GetAllQCControls() As ControlsDS
        Dim myControlsDS As New ControlsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate

            myGlobalDataTO = myControlsDelegate.GetAll(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'Insert an empty row
                Dim myControlRow As ControlsDS.tparControlsRow
                myControlRow = myControlsDS.tparControls.NewtparControlsRow
                myControlRow.ControlName = ""
                myControlsDS.tparControls.AddtparControlsRow(myControlRow)

                'Move all read Controls to the DS with an empty row
                For Each controlRow As ControlsDS.tparControlsRow In DirectCast(myGlobalDataTO.SetDatos, ControlsDS).tparControls.Rows
                    myControlsDS.tparControls.ImportRow(controlRow)
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAllQCControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAllQCControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myControlsDS
    End Function

    ''' <summary>
    ''' Get data of the specified Control
    ''' </summary>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>Typed DataSet ControlsDS containing data of the informed Control</returns>
    ''' <remarks>
    ''' Created by:  TR 06/04/2011
    ''' </remarks>
    Private Function GetQCControlInfo(ByVal pControlID As Integer) As ControlsDS
        Dim myControlsDS As New ControlsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate

            myGlobalDataTO = myControlsDelegate.GetControlData(Nothing, pControlID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)
            Else
                'Error getting the Control data; shown it
                ShowMessage(Me.Name & ".GetQCControlsInfo", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetQCControlsInfo", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetQCControlsInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myControlsDS
    End Function

    ''' <summary>
    ''' For the selected TestID/SampleType, get the status of the available Multirules and the list of linked Controls
    ''' and bind them to the corresponding screen fields 
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <remarks>
    ''' Created by:  TR 05/04/2011
    ''' Modified by: SA 14/06/2012 - Inform the TestType when called functions to get Multirules and linked Controls   
    ''' </remarks>
    Private Sub BindQCMultirulesTestID(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestControlDelegate As New TestControlsDelegate
            Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate

            'Get status of the available Multirules for the TestID/SampleType
            myGlobalDataTO = myTestSampleMultiDelegate.GetByTestIDAndSampleTypeNEW(Nothing, "STD", pTestID, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myTestSampleMultiDS As TestSamplesMultirulesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesMultirulesDS)

                If (myTestSampleMultiDS.tparTestSamplesMultirules.Count > 0) Then
                    Dim qTestSampleMultiList As List(Of TestSamplesMultirulesDS.tparTestSamplesMultirulesRow) = (From a In myTestSampleMultiDS.tparTestSamplesMultirules _
                                                                                                                Select a).ToList()
                    If (qTestSampleMultiList.Count > 0) Then
                        's12CheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").ToList().First().SelectedRule()

                        s13CheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().SelectedRule
                        x22CheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().SelectedRule
                        r4sCheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().SelectedRule
                        s41CheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().SelectedRule
                        x10CheckBox.Checked = qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().SelectedRule
                    End If
                Else
                    's12CheckBox.Checked = False
                    s13CheckBox.Checked = False
                    x22CheckBox.Checked = False
                    r4sCheckBox.Checked = False
                    s41CheckBox.Checked = False
                    x10CheckBox.Checked = False
                End If
            Else
                'Error getting the status of available Multirules for the TestID/SampleType; shown it
                ShowMessage(Me.Name & ".BindQCMultirulesTestID", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage, Me)
            End If

            'Get all Controls linked to the TestID/SampleType
            myGlobalDataTO = myTestControlDelegate.GetControlsNEW(Nothing, "STD", pTestID, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestControlDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                'Prepare and fill the Controls DataGrid
                PrepareUsedControlsGrid()
                UsedControlsGridView.DataSource = SelectedTestControlDS.tparTestControls
                UsedControlsGridView.ClearSelection()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BindQCControlsByTestID", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BindQCControlsByTestID", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Remove a Control from the list of linked Controls, adding it to the local DeleteControlTO
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 07/04/2011
    ''' </remarks>
    Private Sub RemoveTestControl(ByVal pTestID As Integer, ByVal pSampleType As String, ByVal pControlID As Integer)
        Try
            Dim qTestControlToRemove As List(Of TestControlsDS.tparTestControlsRow) = (From a In SelectedTestControlDS.tparTestControls _
                                                                                  Where Not a.IsTestIDNull AndAlso a.TestID = pTestID _
                                                                                    AndAlso a.SampleType = pSampleType _
                                                                                    AndAlso a.ControlID = pControlID _
                                                                                     Select a).ToList()
            If (qTestControlToRemove.Count > 0) Then
                qTestControlToRemove.First().Delete()

                Dim myDeleteControlTO As New DeletedControlTO
                myDeleteControlTO.ControlID = pControlID
                myDeleteControlTO.TestID = pTestID
                myDeleteControlTO.SampleType = pSampleType
                LocalDeleteControlTOList.Add(myDeleteControlTO)
                ChangesMade = True
            End If
            SelectedTestControlDS.AcceptChanges()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RemoveTestControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RemoveTestControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Calculate the Target Mean and SD for a Control linked to the TestID/SampleType
    ''' </summary>
    ''' <param name="pRowIndex">Current Row index</param>
    ''' <param name="pColumnIndex">Current Column Index</param>
    ''' <remarks>
    ''' Created by:  TR 06/04/2011
    ''' Modified by: TR 19/01/2012 - Zero is allowed as value of MinConcentration; validation is removed
    ''' </remarks>
    Private Sub CalculatedTargets(ByVal pRowIndex As Integer, ByVal pColumnIndex As Integer)
        Try
            'Clear all errors and recover the cell alignment
            UsedControlsGridView.Rows(pRowIndex).Cells("ControlName").ErrorText = String.Empty
            UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").ErrorText = String.Empty
            UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").ErrorText = String.Empty

            UsedControlsGridView.Rows(pRowIndex).Cells(pColumnIndex).Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Rows(pRowIndex).Cells("ControlName").Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight
            UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Before calculate target values, make sure a Control has been selected
            If (UsedControlsGridView.Rows(pRowIndex).Cells("ControlName").Value Is DBNull.Value) Then
                UsedControlsGridView.Rows(pRowIndex).Cells("ControlName").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                Exit Try
            End If

            'Validate that Min/Max Concentration values are informed and that Max Concentration is different of zero
            If (Not UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Value Is DBNull.Value) AndAlso _
               (Not UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value Is DBNull.Value) AndAlso _
               (CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value) <> 0) Then

                'Validate MinConcentration < MaxConcentration
                If (CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Value) < CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value)) Then
                    'Calculate the Target Mean as (MinConcentration + MaxConcentration)/2
                    UsedControlsGridView.Rows(pRowIndex).Cells("TargetMean").Value = ((CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Value) + _
                                                                                       CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value)) / 2)

                    'Calculate the Target SD as (MaxConcentration - MinConcentration)/ (2 * RejectionCriteria)
                    UsedControlsGridView.Rows(pRowIndex).Cells("TargetSD").Value = ((CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value) - _
                                                                                     CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Value)) / _
                                                                                     (2 * QCRejectionCriteria.Value))
                Else
                    'Values are wrong; shown error MIN MUST BE LOWER THAN MAX
                    UsedControlsGridView.Rows(pRowIndex).Cells(pColumnIndex).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    UsedControlsGridView.Rows(pRowIndex).Cells(pColumnIndex).ErrorText = GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)

                    UsedControlsGridView.Rows(pRowIndex).Cells("TargetMean").Value = DBNull.Value
                    UsedControlsGridView.Rows(pRowIndex).Cells("TargetSD").Value = DBNull.Value
                End If
            Else
                'Values are not informed, the error is shown in the cell
                If (UsedControlsGridView.Rows(pRowIndex).Cells("MinConcentration").Value Is DBNull.Value) Then
                    'If Min Concentration is not informed, shown error REQUIRED VALUE in the cell
                    UsedControlsGridView.Rows(pRowIndex).Cells(4).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    UsedControlsGridView.Rows(pRowIndex).Cells(4).ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())

                ElseIf (UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value Is DBNull.Value OrElse _
                        CSng(UsedControlsGridView.Rows(pRowIndex).Cells("MaxConcentration").Value) = 0) Then
                    'If Max Concentration is not informed or its value is zero, shown error REQUIRED VALUE in the cell
                    UsedControlsGridView.Rows(pRowIndex).Cells(5).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    UsedControlsGridView.Rows(pRowIndex).Cells(5).ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalculatedTargets", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalculatedTargets", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activate/Desactivate fields in QC TAB depending on value selected in the CheckBox of QC Active
    ''' </summary>
    ''' <param name="pEnable">True to activate fields; False to deactivate them</param>
    ''' <remarks>
    ''' Created by:  TR 06/04/2011
    ''' Modified by: DL 18/07/2011 - Enable/disable only individual fields, not the GroupBox
    '''              TR 25/01/2012 - CheckBox Rule 1-2s is always checked and disabled
    ''' </remarks>
    Private Sub ActivateQCControlsByQCActive(ByVal pEnable As Boolean)
        Try
            If (Not EditionMode AndAlso pEnable) Then
                pEnable = False
            End If

            QCReplicNumberNumeric.Enabled = pEnable
            QCRejectionCriteria.Enabled = pEnable

            ManualRadioButton.Enabled = pEnable
            StaticRadioButton.Enabled = pEnable
            QCMinNumSeries.Enabled = pEnable

            RulesToApplyGroupBox.Enabled = pEnable
            s12CheckBox.Enabled = False '1-2s is always disabled

            QCErrorAllowable.Enabled = pEnable
            BsButton1.Enabled = pEnable

            UsedControlsGridView.Enabled = pEnable
            AddControls.Enabled = pEnable
            DeleteControlButton.Enabled = pEnable
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ActivateQCControlsByQCActive ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ActivateQCControlsByQCActive ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load or update the global DS containing the list of Multirules for the TestID/SampleType, selecting or not each Rule 
    ''' depending on the checked/unchecked value of the corresponding CheckBox
    ''' </summary>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <remarks>
    ''' Created by:  TR 12/04/2011
    ''' Modified by: TR 25/01/2012 - Rule 1-2s is always selected
    '''              SA 14/06/2012 - Inform the TestType when verify if the Multirules have been created, and also when fill the DS
    ''' </remarks>
    Private Sub UpdateTestSampleMultiRules(ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            'Execute only if the Test is not being copied
            If (CopyTestData) Then Return

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate

            'Verify if the group of Rules have been already created for the TestID/SampleType
            myGlobalDataTO = myTestSampleMultiDelegate.GetByTestIDAndSampleTypeNEW(Nothing, "STD", pTestID, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestSampleMultirulesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesMultirulesDS)

                If (SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Count = 0) Then
                    'The group of Rules do not exist for the TestID/SampleType
                    'Search all Rules in QC_MULTIRULES subtable in Preloaded Master Data table and load them in a TestSamplesMultirulesDS
                    Dim myPreloadedDataDelegate As New PreloadedMasterDataDelegate
                    myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.QC_MULTIRULES)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                        Dim myTestSampleMultiRulesRow As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow
                        For Each multiRule As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows
                            myTestSampleMultiRulesRow = SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.NewtparTestSamplesMultirulesRow
                            myTestSampleMultiRulesRow.TestType = "STD"
                            myTestSampleMultiRulesRow.TestID = pTestID
                            myTestSampleMultiRulesRow.SampleType = pSampleType
                            myTestSampleMultiRulesRow.RuleID = multiRule.ItemID

                            SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.AddtparTestSamplesMultirulesRow(myTestSampleMultiRulesRow)
                        Next
                    Else
                        ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If

                'Update each Rule as selected/unselected according value of the corresponding CheckBox
                Dim qTestSampleMultiList As List(Of TestSamplesMultirulesDS.tparTestSamplesMultirulesRow) = (From a As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow _
                                                                                                                    In SelectedTestSampleMultirulesDS.tparTestSamplesMultirules _
                                                                                                           Select a).ToList()

                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").First().SelectedRule = True  'Rule 1-2s is always selected
                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().SelectedRule = s13CheckBox.Checked
                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().SelectedRule = x22CheckBox.Checked
                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().SelectedRule = r4sCheckBox.Checked
                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().SelectedRule = s41CheckBox.Checked
                qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().SelectedRule = x10CheckBox.Checked
            Else
                ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTestSampleMultiRules ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate all required fields in QC Tab are informed with allowed values
    ''' </summary>
    ''' <returns>TRUE if an error was found; FALSE if all fields have correct values</returns>
    ''' <remarks>
    ''' Created by:  TR 07/04/2011
    ''' </remarks>
    Private Function ValidateErrorOnQCTab(Optional ByVal pSaveButtonClicked As Boolean = False) As Boolean
        Dim ErrorFound As Boolean = False

        Try

            If QCActiveCheckBox.Checked Then 'DL 25/06/2012
                'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications

                If String.Compare(QCReplicNumberNumeric.Text, String.Empty, False) = 0 Then 'DL 22/06/2012
                    BsErrorProvider1.SetError(QCReplicNumberNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                    ErrorFound = True
                End If

                If String.Compare(QCRejectionCriteria.Text, String.Empty, False) = 0 Then 'DL 22/06/2012
                    BsErrorProvider1.SetError(QCRejectionCriteria, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                    ErrorFound = True
                End If

                If (StaticRadioButton.Checked) AndAlso String.Compare(QCMinNumSeries.Text, String.Empty, False) = 0 Then 'DL 22/06/2012
                    BsErrorProvider1.SetError(QCMinNumSeries, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                    ErrorFound = True
                End If

                'DL 22/06/2012
                'Validate Controls grid
                If Not ErrorFound Then ErrorFound = ValidateErrorOnQCUsedControls()

                ValidationError = ErrorFound
                'DL 22/06/2012

                If (Not ValidationError) Then
                    'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications
                Else
                    'Go to QC TAB only when saving over the database
                    If (pSaveButtonClicked) Then TestDetailsTabs.SelectTab("QCTabPage")
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateErrorOnQCTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateErrorOnQCTab ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ErrorFound
    End Function

    ''' <summary>
    ''' Validate data in all rows in the grid of linked Controls is correct
    ''' </summary>
    ''' <returns>TRUE if an error was found; FALSE if all fields have correct values</returns>
    ''' <remarks>
    ''' Created by:  TR 13/04/2011
    ''' </remarks>
    Private Function ValidateErrorOnQCUsedControls() As Boolean
        Dim ErrorFound As Boolean = False

        Try
            'BsErrorProvider1.Clear() RH 22/06/2012 Don't remove previous error notifications

            For Each usedControlRow As DataGridViewRow In Me.UsedControlsGridView.Rows
                usedControlRow.Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                usedControlRow.Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                If (Not usedControlRow.IsNewRow) Then
                    'Validate a Control has been informed
                    If (usedControlRow.Cells("ControlName").Value Is DBNull.Value) Then
                        usedControlRow.Cells("ControlName").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True
                    End If

                    'Validate Null and/or Zero values in Min/Max Concentration cells
                    If (Not usedControlRow.Cells("MinConcentration").Value Is DBNull.Value AndAlso Not usedControlRow.Cells("MaxConcentration").Value Is DBNull.Value) AndAlso _
                       (Not CSng(usedControlRow.Cells("MinConcentration").Value) < 0 AndAlso Not CSng(usedControlRow.Cells("MaxConcentration").Value) = 0) Then
                        'Validate MinConcentration < MaxConcentration
                        If (CSng(usedControlRow.Cells("MinConcentration").Value) >= CSng(usedControlRow.Cells("MaxConcentration").Value)) Then
                            usedControlRow.Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                            usedControlRow.Cells("MinConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
                            ErrorFound = True
                        End If

                    ElseIf (usedControlRow.Cells("MinConcentration").Value Is DBNull.Value OrElse CSng(usedControlRow.Cells("MinConcentration").Value) < 0) Then
                        usedControlRow.Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        usedControlRow.Cells("MinConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True

                    ElseIf (usedControlRow.Cells("MaxConcentration").Value Is DBNull.Value OrElse CSng(usedControlRow.Cells("MaxConcentration").Value) = 0) Then
                        usedControlRow.Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        usedControlRow.Cells("MaxConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString())
                        ErrorFound = True
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateQCUsedControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateQCUsedControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ErrorFound
    End Function

    ''' <summary>
    ''' Verify if the specified Control is already linked to the TestID/SampleType
    ''' </summary>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>TRUE if the informed Control is already linked to the TestID/SampleType; otherwise FALSE</returns>
    ''' <remarks>
    ''' Created by:  TR 07/04/2011
    ''' </remarks>
    Private Function ControlExist(ByVal pControlID As Integer) As Boolean
        Dim LocalControlExist As Boolean = False
        Try
            Dim qTestControlExist As List(Of TestControlsDS.tparTestControlsRow) = (From a As TestControlsDS.tparTestControlsRow In SelectedTestControlDS.tparTestControls _
                                                                               Where Not a.IsControlIDNull AndAlso a.ControlID = pControlID _
                                                                                  Select a).ToList
            If (qTestControlExist.Count > 0) Then LocalControlExist = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ControlExist ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ControlExist ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return LocalControlExist
    End Function

    ''' <summary>
    ''' Count how many of the linked Controls are marked as active for the TestID/SampleType
    ''' </summary>
    ''' <returns>The number or Controls marked as active</returns>
    ''' <remarks>
    '''Created by:  TR 07/04/2011
    ''' </remarks>
    Private Function CountActiveControl() As Integer
        Dim ActiveControlCount As Integer = 0
        Try
            UsedControlsGridView.Refresh()
            For Each TestControlRow As DataGridViewRow In UsedControlsGridView.Rows
                If CBool(TestControlRow.Cells("ActiveControl").Value) Then
                    ActiveControlCount += 1
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CountActiveControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CountActiveControl ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ActiveControlCount
    End Function

    ''' <summary>
    ''' Recalculate Target Values (Mean and SD) for all Controls linked to the TestID/SampleType
    ''' </summary>
    ''' <remarks>
    '''Created by:  TR 12/04/2011
    ''' </remarks>
    Private Sub RecalculateAllTarget()
        Try
            'Recalculate the target values for each Control in the grid
            For Each UsedControlRow As DataGridViewRow In UsedControlsGridView.Rows
                If (Not UsedControlRow.Cells("MinConcentration").Value Is Nothing) Then
                    For Each UsedControlCol As DataGridViewCell In UsedControlRow.Cells
                        If (UsedControlCol.ColumnIndex = 4 Or UsedControlCol.ColumnIndex = 5) Then
                            CalculatedTargets(UsedControlCol.RowIndex, UsedControlCol.ColumnIndex)
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RecalculateAllTarget ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RecalculateAllTarget ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the Controls selected on the DataGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 14/04/2011
    ''' MODIFIED BY: TR 24/07/2013 -Bug #986 Order del list of selecte row before deleting and use a For instead of 
    '''                             ForEach, using step -1.
    ''' </remarks>
    Private Sub DeleteSelectedControl()
        Try
            If (UsedControlsGridView.SelectedRows.Count > 0) AndAlso _
               (ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                Dim mySelectedRow As New List(Of Integer)
                'Load the ID of the selected Control to the list 
                For Each TestControlRow As DataGridViewRow In UsedControlsGridView.SelectedRows
                    If (Not TestControlRow.Cells("ControlID").Value Is Nothing) Then
                        mySelectedRow.Add(TestControlRow.Index)
                    End If
                Next
                'TR 24/07/2013 -Order the list and Process each selected Control 
                mySelectedRow.Sort()
                For i As Integer = mySelectedRow.Count - 1 To 0 Step -1
                    If (Not UsedControlsGridView.Rows(mySelectedRow(i)).Cells("TestID").Value Is DBNull.Value) AndAlso _
                       (Not UsedControlsGridView.Rows(mySelectedRow(i)).Cells("TestID").Value Is Nothing) Then
                        'Remove the link between the TestID/SampleType and the Control
                        RemoveTestControl(CInt(UsedControlsGridView.Rows(mySelectedRow(i)).Cells("TestID").Value), _
                                          UsedControlsGridView.Rows(mySelectedRow(i)).Cells("SampleType").Value.ToString(), _
                                          CInt(UsedControlsGridView.Rows(mySelectedRow(i)).Cells("ControlID").Value))
                    Else
                        'Just remove the row from the DataGrid
                        UsedControlsGridView.Rows.Remove(UsedControlsGridView.Rows(mySelectedRow(i)))
                    End If
                Next
                'TR 24/07/2013 -END.

                'TR 24/07/2013 commented cause error on bug (#986) Process each selected Control 
                'For Each TestControlRowIndex As Integer In mySelectedRow
                '    If (Not UsedControlsGridView.Rows(TestControlRowIndex).Cells("TestID").Value Is DBNull.Value) AndAlso _
                '       (Not UsedControlsGridView.Rows(TestControlRowIndex).Cells("TestID").Value Is Nothing) Then
                '        'Remove the link between the TestID/SampleType and the Control
                '        RemoveTestControl(CInt(UsedControlsGridView.Rows(TestControlRowIndex).Cells("TestID").Value), _
                '                          UsedControlsGridView.Rows(TestControlRowIndex).Cells("SampleType").Value.ToString(), _
                '                          CInt(UsedControlsGridView.Rows(TestControlRowIndex).Cells("ControlID").Value))
                '    Else
                '        'Just remove the row from the DataGrid
                '        UsedControlsGridView.Rows.Remove(UsedControlsGridView.Rows(TestControlRowIndex))
                '    End If
                'Next
                'TR 24/07/2013 commented -END.

                If (UsedControlsGridView.Rows.Count = 0) Then UsedControlsGridView.AllowUserToAddRows = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteSelectedControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteSelectedControl ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' If the value informed is less than the allowed, set the minimum allowed as cell value
    ''' If the value informed is greater than the allowed, set the maximum allowed as cell value
    ''' Finally, apply format to numeric cells:
    ''' ** Min, Max and TargetMean are shown with the number of decimals defined for the Test
    ''' ** TargetSD is shown with the number of decimals defined for the Test + 1
    ''' </summary>
    ''' <param name="pRow">Current Row index</param>
    ''' <param name="pCol">Current Column Index</param>
    ''' <remarks>
    ''' Created by: SA 18/05/2011
    ''' </remarks>
    Private Sub SetLimitValues(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            Dim numDecimals As Integer = Convert.ToInt32(DecimalsUpDown.Value)

            If (Not UsedControlsGridView.Rows(pRow).Cells(pCol).Value Is Nothing) AndAlso _
               (Not UsedControlsGridView.Rows(pRow).Cells(pCol).Value Is DBNull.Value) AndAlso _
               (UsedControlsGridView.Rows(pRow).Cells(pCol).Value.ToString <> String.Empty) AndAlso _
               (String.Compare(UsedControlsGridView.Rows(pRow).Cells(pCol).Value.ToString, myDecimalSeparator, False) <> 0) Then

                Dim myValue As String = UsedControlsGridView.Rows(pRow).Cells(pCol).Value.ToString
                If (CSng(myValue) < MinAllowedConcentration) Then
                    Dim numToAdd As Single = 1

                    If (Convert.ToInt32(DecimalsUpDown.Value) = 0) Then
                        myValue = (MinAllowedConcentration + numToAdd).ToString
                    Else
                        numToAdd = 0
                        numToAdd = CSng(numToAdd.ToString("F" & (numDecimals - 1).ToString) & "1")

                        myValue = (MinAllowedConcentration + numToAdd).ToString
                    End If

                ElseIf (CSng(myValue) > MaxAllowedConcentration) Then
                    myValue = MaxAllowedConcentration.ToString
                End If

                'Format the value
                If (UsedControlsGridView.Columns(pCol).Name = "TargetSD") Then numDecimals += 1
                UsedControlsGridView.Rows(pRow).Cells(pCol).Value = CSng(myValue).ToString("F" & numDecimals.ToString)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetLimitValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetLimitValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Copy Selected Test"

    ''' <summary>
    ''' Copy Selected Test Values.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 21/07/2011
    ''' </remarks>
    Private Sub CopySelectedTestValues()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            CopyTestData = True
            If UpdatedTestCalibratorDS.tparTestCalibrators.Count > 0 Then

                Dim myCalibratorDelegate As New CalibratorsDelegate
                Dim myTestCalibratorDelegate As New TestCalibratorsDelegate
                Dim myTestCalibratorsValuesDS As New TestCalibratorValuesDS
                Dim myTestCalibratorsValuesDelegate As New TestCalibratorValuesDelegate
                'Dim myTestSampleDelegate As New TestSamplesDelegate

                'CopyTestData = True 'Set global variable to true
                'Change the TestName and TestShortName
                'SelectedTestDS.tparTests(0).TestName &= "Copy" & SelectedTestDS.tparTests(0).TestName
                'SelectedTestDS.tparTests(0).ShortName &= "Copy" & SelectedTestDS.tparTests(0).ShortName

                'Get the test calibrator information 
                myGlobalDataTO = myTestCalibratorDelegate.GetTestCalibratorByTestID(Nothing, SelectedTestDS.tparTests(0).TestID)
                If Not myGlobalDataTO.HasError Then
                    UpdatedTestCalibratorDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                    'Change the isnew value to True.
                    For Each TestCalibRow As TestCalibratorsDS.tparTestCalibratorsRow In _
                                                                        UpdatedTestCalibratorDS.tparTestCalibrators.Rows
                        TestCalibRow.IsNew = True
                        'Get the calibrator info
                        myGlobalDataTO = myCalibratorDelegate.GetCalibratorData(Nothing, TestCalibRow.CalibratorID)
                        If Not myGlobalDataTO.HasError Then
                            If DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS).tparCalibrators.Count > 0 Then
                                UpdatedCalibratorsDS.tparCalibrators.ImportRow(DirectCast(myGlobalDataTO.SetDatos, CalibratorsDS).tparCalibrators(0))
                            End If
                        End If
                        'Get the TestCalibrator values 
                        If Not myGlobalDataTO.HasError Then
                            myGlobalDataTO = myTestCalibratorsValuesDelegate.GetTestCalibratorValuesByTestCalibratorID(Nothing, _
                                                                                                        TestCalibRow.TestCalibratorID)
                            If Not myGlobalDataTO.HasError Then
                                myTestCalibratorsValuesDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorValuesDS)
                                For Each TestCalValuesRow As TestCalibratorValuesDS.tparTestCalibratorValuesRow In _
                                                                                    myTestCalibratorsValuesDS.tparTestCalibratorValues.Rows
                                    TestCalValuesRow.IsNew = True
                                    UpdatedTestCalibratorValuesDS.tparTestCalibratorValues.ImportRow(TestCalValuesRow)

                                Next
                            End If
                        End If
                    Next
                End If
            End If

            'Get the controls information 
            If Not myGlobalDataTO.HasError Then
                Dim myTestControlDS As New TestControlsDS
                Dim myTestControlsDelegate As New TestControlsDelegate

                Dim myTestSampleMultiRulesDS As New TestSamplesMultirulesDS
                Dim myTestSampleMultiRulesDelegate As New TestSamplesMultirulesDelegate

                SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.Clear() 'TR 25/01/2012 -cLEAR THE SELECTED TABLE 
                SelectedTestControlDS.tparTestControls.Clear()
                'Get Test Control information by Test/Sample Type
                For Each TestSampleRow As TestSamplesDS.tparTestSamplesRow In SelectedTestSamplesDS.tparTestSamples
                    'myGlobalDataTO = myTestControlsDelegate.GetControls(Nothing, TestSampleRow.TestID, TestSampleRow.SampleType)

                    TestSampleRow.IsNew = True 'TR 20/02/2013 -Set is new TestsSample.
                    'TR 14/06/2012 -implement new funtionality.
                    myGlobalDataTO = myTestControlsDelegate.GetControlsNEW(Nothing, "STD", TestSampleRow.TestID, TestSampleRow.SampleType)
                    If Not myGlobalDataTO.HasError Then
                        myTestControlDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)
                        'Import all rows.
                        For Each testControlRow As TestControlsDS.tparTestControlsRow In myTestControlDS.tparTestControls
                            testControlRow.TS_User = GetApplicationInfoSession.UserName
                            testControlRow.TS_DateTime = DateTime.Now
                            SelectedTestControlDS.tparTestControls.ImportRow(testControlRow)
                        Next

                    Else
                        Exit For
                    End If

                    'Get multirules
                    'myGlobalDataTO = myTestSampleMultiRulesDelegate.GetByTestIDAndSampleType(Nothing, TestSampleRow.TestID, TestSampleRow.SampleType)
                    'TR 14/06/2012 -implement new funtionality.
                    myGlobalDataTO = myTestSampleMultiRulesDelegate.GetByTestIDAndSampleTypeNEW(Nothing, "STD", TestSampleRow.TestID, TestSampleRow.SampleType)
                    If Not myGlobalDataTO.HasError Then
                        myTestSampleMultiRulesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesMultirulesDS)
                        'Import all rows.
                        For Each testSampMultirulesRow As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow In _
                                                                            myTestSampleMultiRulesDS.tparTestSamplesMultirules.Rows

                            SelectedTestSampleMultirulesDS.tparTestSamplesMultirules.ImportRow(testSampMultirulesRow)
                        Next
                    End If
                Next
                'TR 23/01/2012 -Filter the controls by the selected Sample Type for visual effect.
                SelectedTestControlDS.AcceptChanges()
                UsedControlsGridView.DataSource = SelectedTestControlDS.tparTestControls.Where(Function(a) a.SampleType = CurrentSampleType).ToList()

            End If

            'Get the Reference Ranges:
            If Not myGlobalDataTO.HasError Then
                Dim myTestRefRangesDelegates As New TestRefRangesDelegate
                'Get all the Ref Ranges by the test ID.
                myGlobalDataTO = myTestRefRangesDelegates.ReadByTestID(Nothing, SelectedTestDS.tparTests(0).TestID)
                If Not myGlobalDataTO.HasError Then
                    SelectedTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)

                    'Set status new to true.
                    For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In SelectedTestRefRangesDS.tparTestRefRanges.Rows
                        TestRefRangesRow.IsNew = True
                        TestRefRangesRow.TS_User = GetApplicationInfoSession.UserName
                        TestRefRangesRow.TS_DateTime = DateTime.Now
                    Next
                End If
            End If

            'TR 02/03/2012 -Remove the reagent testcode for copy testNameExits
            For Each myReagentRow As ReagentsDS.tparReagentsRow In SelectedReagentsDS.tparReagents.Rows
                myReagentRow.CodeTest = String.Empty
            Next
            'TR 02/03/2012 -END.

            If myGlobalDataTO.HasError Then
                ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CopySelectedTestValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "TO DELETE"

    '''' <summary>
    '''' Validate the postdilutions value
    '''' </summary>
    '''' <param name="pSamplevol"></param>
    '''' <param name="pWashVol"></param>
    '''' <param name="pR1Vol"></param>
    '''' <param name="R2Vol"></param>
    '''' <param name="pAbsorbance"></param>
    '''' <param name="AnalysisMode"></param>
    '''' <param name="pRedPostDilutionFact"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function ValidatePostDilution(ByVal pSamplevol As Single, ByVal pWashVol As Single, _
    '                                           ByVal pR1Vol As Single, ByVal R2Vol As Single, _
    '                                           ByVal pAbsorbance As Boolean, ByVal AnalysisMode As String, _
    '                                           ByVal pRedPostDilutionFact As Single) As GlobalDataTO
    '    Dim myGlobalDataTO As New GlobalDataTO
    '    Try
    '        Dim myVolCalc As New VolumeCalculations
    '        myVolCalc.R1Vol = 0
    '        myVolCalc.R2Vol = 0
    '        myVolCalc.WashVol = 0
    '        myVolCalc.sampleVol = 0

    '        'AG 26/03/2010 - Reset steps values before calcute them
    '        myVolCalc.sampleSteps = 0
    '        myVolCalc.WashStepsVol = 0
    '        myVolCalc.R1Steps = 0
    '        myVolCalc.R2Steps = 0
    '        'END AG 26/03/2010

    '        myVolCalc.sampleVol = pSamplevol
    '        myVolCalc.WashVol = pWashVol
    '        myVolCalc.R1Vol = pR1Vol
    '        myVolCalc.R2Vol = R2Vol
    '        myVolCalc.AbsorbanceFlag = pAbsorbance
    '        myVolCalc.AnalysisMode = AnalysisMode
    '        myVolCalc.PostDilutionFactor = pRedPostDilutionFact
    '        myGlobalDataTO = CheckPostdilutions("RED", myVolCalc)

    '        If Not myGlobalDataTO.HasError Then
    '            PostReduce = CType(myGlobalDataTO.SetDatos, VolumeCalculations)
    '            If PostReduce.hasError Then
    '                myGlobalDataTO.HasError = True
    '                myGlobalDataTO.ErrorCode = PostReduce.ErrorCode
    '                'Exit Try
    '            End If

    '            If Not myGlobalDataTO.HasError Then
    '                'AG 26/03/2010 - Reset steps values before calcute them
    '                myVolCalc.sampleSteps = 0
    '                myVolCalc.WashStepsVol = 0
    '                myVolCalc.R1Steps = 0
    '                myVolCalc.R2Steps = 0
    '                'END AG 26/03/2010

    '                myVolCalc.PostDilutionFactor = CSng(IncPostDilutionFactorTextBox.Text)
    '                'Increment
    '                myGlobalDataTO = CheckPostdilutions("INC", myVolCalc)
    '                If Not myGlobalDataTO.HasError Then
    '                    PostIncrease = CType(myGlobalDataTO.SetDatos, VolumeCalculations)
    '                    If PostIncrease.hasError Then
    '                        myGlobalDataTO.HasError = True
    '                        myGlobalDataTO.ErrorCode = PostIncrease.ErrorCode
    '                        'Exit Try
    '                    End If
    '                End If
    '            End If
    '        End If

    '        If myGlobalDataTO.HasError Then
    '            ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
    '        End If

    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ValidatePostDilution ", EventLogEntryType.Error, _
    '                                                        GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010  "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    '    Return myGlobalDataTO
    'End Function



    'Private Sub ComboBox_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalysisModeCombo.Enter, _
    '                                                                                               UnitsCombo.Enter, _
    '                                                                                               ReactionTypeCombo.Enter

    '    HideSampleTypePanel()

    'End Sub

    'Private Sub TestDetailsTabs_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TestDetailsTabs.MouseDown
    '    HideSampleTypePanel()
    'End Sub

    'Private Sub GeneralTab_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GeneralTab.MouseDown
    '    HideSampleTypePanel()
    'End Sub

    'Private Sub NumericUpDown_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DecimalsUpDown.Enter, ReplicatesUpDown.Enter
    '    HideSampleTypePanel()
    'End Sub
#End Region

End Class