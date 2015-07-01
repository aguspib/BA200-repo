Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.App

Namespace Biosystems.Ax00.FwScriptsManagement


    ''' <summary>
    ''' Delegate that manages the Scripts' sending and receiving
    ''' </summary>
    ''' <remarks>Created by SG 17/11/10</remarks>
    Public Class PositionsAdjustmentDelegate
        Inherits BaseFwScriptDelegate

#Region "Declarations"
        Private CurrentOperation As OPERATIONS                  ' controls the operation which is currently executed in each time
        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer
#End Region

#Region "Attributes"
        'Private AnalyzerIdAttr As String = ""
        ' General
        Private pAxisAdjustAttr As AXIS
        Private pMovAdjustAttr As MOVEMENT
        Private pValueAdjustAttr As String = ""
        Private NoneInstructionToSendAttr As Boolean
        ' Absolute Movements parameters
        Private pArmABSMovPolarAttr As String = ""
        Private pArmABSMovZAttr As String = ""
        Private pValueRotorABSMovAttr As String = ""
        Private pValueRotorZTubeABSMovAttr As String = ""
        ' Relative Movements parameters
        Private pArmRELMovPolarAttr As String = ""
        Private pArmRELMovZAttr As String = ""
        Private pValueRotorRELMovAttr As String = ""
        ' Offset parameters
        Private pWashingStationOffsetAttr As Single
        Private pSampleArmOffsetAttr As Single
        Private pReagent1ArmOffsetAttr As Single
        Private pReagent2ArmOffsetAttr As Single
        Private pMixer1ArmOffsetAttr As Single
        Private pMixer2ArmOffsetAttr As Single
        ' Parking parameters
        Private pSampleArmParkVAttr As String = ""
        Private pSampleArmParkHAttr As String = ""
        Private pSampleArmParked As Boolean
        Private pReagent1ArmParkVAttr As String = ""
        Private pReagent1ArmParkHAttr As String = ""
        Private pReagent1ArmParked As Boolean
        Private pReagent2ArmParkVAttr As String = ""
        Private pReagent2ArmParkHAttr As String = ""
        Private pReagent2ArmParked As Boolean
        Private pMixer1ArmParkVAttr As String = ""
        Private pMixer1ArmParkHAttr As String = ""
        Private pMixer1ArmParked As Boolean
        Private pMixer2ArmParkVAttr As String = ""
        Private pMixer2ArmParkHAttr As String = ""
        Private pMixer2ArmParked As Boolean
        Private pWashingStationParkAttr As String = ""

        ' Security fly parameters
        Private pSampleSecurityFlyAttr As String = ""
        Private pReagent1SecurityFlyAttr As String = ""
        Private pReagent2SecurityFlyAttr As String = ""
        Private pMixer1SecurityFlyAttr As String = ""
        Private pMixer2SecurityFlyAttr As String = ""


        ' Operations done
        Private LoadAdjDoneAttr As Boolean
        Private ParkDoneAttr As Boolean

        ' Optic Centering parameters
        Private LEDIntensityAttr As Integer
        Private WaveLengthAttr As Integer
        Private AbsorbanceScanDoneAttr As Boolean
        Private AbsorbanceScanReadingsAttr As Integer
        Private NumWellsAttr As Integer
        Private StepsbyWellAttr As Integer
        Private ReadedCountsAttr As List(Of OpticCenterDataTO)

        'HISTORY
        '**********************************************************************************************
        Private HistoryAdjustNewValueAttr As String = ""
        Private HistoryTaskAttr As PreloadedMasterDataEnum = Nothing
        Private HistoryAreaAttr As HISTORY_AREAS = Nothing

        'OPTIC CENTERING
        Private HisOpticCenteringValueAttr As Integer = -1
        ' XBC 05/01/2012 - Add Encoder functionality
        Private HisEncoderValueAttr As Integer = -1
        ' XBC 05/01/2012 - Add Encoder functionality
        Private HisLedIntensityAttr As Integer = -1
        Private HisWaveLengthAttr As Integer = -1
        Private HisRotorPosValueAttr As Integer = -1
        Private HisNumWellsAttr As Integer = -1
        Private HisStepsByWellAttr As Integer = -1

        Private HisOpticCenteringAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HisOpticCenteringTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE


        'WASHING STATION
        Private HisWashingStationValueAttr As Integer = -1

        Private HisWashingStationAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HisWashingStationTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE


        'ARMS
        'Private HisArmRotorValueAttr As Integer = -1
        'Private HisArmPolarValueAttr As Integer = -1
        'Private HisArmZValueAttr As Integer = -1
        Private HisArmPosNameAttr As HISTORY_ARM_POSITIONS = HISTORY_ARM_POSITIONS.None
        Private HisIsStirrerTestAttr As Boolean = False

        Private HisArmAdjResultsAttr As New Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS)
        Private HisArmTestResultsAttr As New Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS)
        Private HisArmAxesValuesAttr As New Dictionary(Of HISTORY_ARM_POSITIONS, List(Of Integer))

        'Private HisArmAdjResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HisStirrerTestResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE

        Private HomesDoneAttr As Boolean

        Private SampleArmPolarZREFAttr As Integer
        Private Reagent1ArmPolarZREFAttr As Integer
        Private Reagent2ArmPolarZREFAttr As Integer
        Private Mixer1ArmPolarZREFAttr As Integer
        Private Mixer2ArmPolarZREFAttr As Integer

        ' XBC 30/11/2011
        Private IsWashingStationUpAttr As Boolean

        ' XBC 27/02/2012
        Private Reagent1SV_ZOffsetAttr As Single
        Private Reagent1WVR_ZOffsetAttr As Single
        Private Reagent1DV_ZOffsetAttr As Single
        Private Reagent1PI_ZOffsetAttr As Single
        Private Reagent2SV_ZOffsetAttr As Single
        Private Reagent2WVR_ZOffsetAttr As Single
        Private Reagent2DV_ZOffsetAttr As Single
        Private Reagent2PI_ZOffsetAttr As Single
        Private SampleSV_ZOffsetAttr As Single
        Private SampleWVR_ZOffsetAttr As Single
        Private SampleDV1_ZOffsetAttr As Single
        Private SamplePI_ZOffsetAttr As Single
        Private SampleRPI_ZOffsetAttr As Single
        Private SampleDV2_ZOffsetAttr As Single
        Private Mixer1SV_ZOffsetAttr As Single
        Private Mixer1WVR_ZOffsetAttr As Single
        Private Mixer1DV_ZOffsetAttr As Single
        Private Mixer2SV_ZOffsetAttr As Single
        Private Mixer2WVR_ZOffsetAttr As Single
        Private Mixer2DV_ZOffsetAttr As Single
        Private WashingStationRR_ZOffsetAttr As Single

        ' XBC 18/04/2012
        Private WSAdjustPreparedAttr As Boolean

#End Region

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            MyClass.myFwScriptDelegate = pFwScriptsDelegate
            MyClass.CurrentOperation = OPERATIONS.NONE
            MyClass.ReportCountTimeout = 0
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Properties"

        'Public Property AnalyzerId() As String
        '    Get
        '        Return AnalyzerIdAttr
        '    End Get
        '    Set(ByVal value As String)
        '        AnalyzerIdAttr = value
        '    End Set
        'End Property
        Public Property HomesDone() As Boolean
            Get
                Return HomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                HomesDoneAttr = value
            End Set
        End Property
        Public Property pAxisAdjust() As AXIS
            Get
                Return Me.pAxisAdjustAttr
            End Get
            Set(ByVal value As AXIS)
                Me.pAxisAdjustAttr = value
            End Set
        End Property
        Public Property pMovAdjust() As MOVEMENT
            Get
                Return Me.pMovAdjustAttr
            End Get
            Set(ByVal value As MOVEMENT)
                Me.pMovAdjustAttr = value
            End Set
        End Property
        Public Property pValueAdjust() As String
            Get
                Return Me.pValueAdjustAttr
            End Get
            Set(ByVal value As String)
                Me.pValueAdjustAttr = value
            End Set
        End Property
        Public Property pArmABSMovPolar() As String
            Get
                Return Me.pArmABSMovPolarAttr
            End Get
            Set(ByVal value As String)
                Me.pArmABSMovPolarAttr = value
            End Set
        End Property
        Public Property pArmABSMovZ() As String
            Get
                Return Me.pArmABSMovZAttr
            End Get
            Set(ByVal value As String)
                Me.pArmABSMovZAttr = value
            End Set
        End Property
        Public Property pArmRELMovPolar() As String
            Get
                Return Me.pArmRELMovPolarAttr
            End Get
            Set(ByVal value As String)
                Me.pArmRELMovPolarAttr = value
            End Set
        End Property
        Public Property pArmRELMovZ() As String
            Get
                Return Me.pArmRELMovZAttr
            End Get
            Set(ByVal value As String)
                Me.pArmRELMovZAttr = value
            End Set
        End Property
        Public Property pValueRotorRELMov() As String
            Get
                Return Me.pValueRotorRELMovAttr
            End Get
            Set(ByVal value As String)
                Me.pValueRotorRELMovAttr = value
            End Set
        End Property
        Public Property pValueRotorABSMov() As String
            Get
                Return Me.pValueRotorABSMovAttr
            End Get
            Set(ByVal value As String)
                Me.pValueRotorABSMovAttr = value
            End Set
        End Property
        Public Property pValueRotorZTubeABSMov() As String
            Get
                Return Me.pValueRotorZTubeABSMovAttr
            End Get
            Set(ByVal value As String)
                Me.pValueRotorZTubeABSMovAttr = value
            End Set
        End Property
        Public ReadOnly Property WashingStationOffset() As Single
            Get
                Return Me.pWashingStationOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleArmOffset() As Single
            Get
                Return Me.pSampleArmOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent1ArmOffset() As Single
            Get
                Return Me.pReagent1ArmOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent2ArmOffset() As Single
            Get
                Return Me.pReagent2ArmOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer1ArmOffset() As Single
            Get
                Return Me.pMixer1ArmOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer2ArmOffset() As Single
            Get
                Return Me.pMixer2ArmOffsetAttr
            End Get
        End Property
        Public Property LoadAdjDone() As Boolean
            Get
                Return Me.LoadAdjDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.LoadAdjDoneAttr = value
            End Set
        End Property
        Public Property ParkDone() As Boolean
            Get
                Return Me.ParkDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.ParkDoneAttr = value
            End Set
        End Property
        Public Property SampleArmParkV() As String
            Get
                Return Me.pSampleArmParkVAttr
            End Get
            Set(ByVal value As String)
                Me.pSampleArmParkVAttr = value
            End Set
        End Property
        Public Property SampleArmParkH() As String
            Get
                Return Me.pSampleArmParkHAttr
            End Get
            Set(ByVal value As String)
                Me.pSampleArmParkHAttr = value
            End Set
        End Property
        Public Property Reagent1ArmParkV() As String
            Get
                Return Me.pReagent1ArmParkVAttr
            End Get
            Set(ByVal value As String)
                Me.pReagent1ArmParkVAttr = value
            End Set
        End Property
        Public Property Reagent1ArmParkH() As String
            Get
                Return Me.pReagent1ArmParkHAttr
            End Get
            Set(ByVal value As String)
                Me.pReagent1ArmParkHAttr = value
            End Set
        End Property
        Public Property Reagent2ArmParkV() As String
            Get
                Return Me.pReagent2ArmParkVAttr
            End Get
            Set(ByVal value As String)
                Me.pReagent2ArmParkVAttr = value
            End Set
        End Property
        Public Property Reagent2ArmParkH() As String
            Get
                Return Me.pReagent2ArmParkHAttr
            End Get
            Set(ByVal value As String)
                Me.pReagent2ArmParkHAttr = value
            End Set
        End Property
        Public Property Mixer1ArmParkV() As String
            Get
                Return Me.pMixer1ArmParkVAttr
            End Get
            Set(ByVal value As String)
                Me.pMixer1ArmParkVAttr = value
            End Set
        End Property
        Public Property Mixer1ArmParkH() As String
            Get
                Return Me.pMixer1ArmParkHAttr
            End Get
            Set(ByVal value As String)
                Me.pMixer1ArmParkHAttr = value
            End Set
        End Property
        Public Property Mixer2ArmParkV() As String
            Get
                Return Me.pMixer2ArmParkVAttr
            End Get
            Set(ByVal value As String)
                Me.pMixer2ArmParkVAttr = value
            End Set
        End Property
        Public Property Mixer2ArmParkH() As String
            Get
                Return Me.pMixer2ArmParkHAttr
            End Get
            Set(ByVal value As String)
                Me.pMixer2ArmParkHAttr = value
            End Set
        End Property
        Public Property WashingStationPark() As String
            Get
                Return Me.pWashingStationParkAttr
            End Get
            Set(ByVal value As String)
                Me.pWashingStationParkAttr = value
            End Set
        End Property
        Public Property NoneInstructionToSend() As Boolean
            Get
                Return NoneInstructionToSendAttr
            End Get
            Set(ByVal value As Boolean)
                NoneInstructionToSendAttr = value
            End Set
        End Property
        Public Property LEDIntensity() As Integer
            Get
                Return Me.LEDIntensityAttr
            End Get
            Set(ByVal value As Integer)
                Me.LEDIntensityAttr = value
            End Set
        End Property
        Public Property WaveLength() As Integer
            Get
                Return Me.WaveLengthAttr
            End Get
            Set(ByVal value As Integer)
                Me.WaveLengthAttr = value
            End Set
        End Property
        Public Property AbsorbanceScanDone() As Boolean
            Get
                Return Me.AbsorbanceScanDoneAttr
            End Get
            Set(ByVal value As Boolean)
                Me.AbsorbanceScanDoneAttr = value
            End Set
        End Property
        Public Property AbsorbanceScanReadings() As Integer
            Get
                Return Me.AbsorbanceScanReadingsAttr
            End Get
            Set(ByVal value As Integer)
                Me.AbsorbanceScanReadingsAttr = value
            End Set
        End Property
        Public Property ReadedCounts() As List(Of OpticCenterDataTO)
            Get
                Return Me.ReadedCountsAttr
            End Get
            Set(ByVal value As List(Of OpticCenterDataTO))
                Me.ReadedCountsAttr = value
            End Set
        End Property
        Public Property NumWells() As Integer
            Get
                Return Me.NumWellsAttr
            End Get
            Set(ByVal value As Integer)
                Me.NumWellsAttr = value
            End Set
        End Property
        Public Property StepsbyWell() As Integer
            Get
                Return Me.StepsbyWellAttr
            End Get
            Set(ByVal value As Integer)
                Me.StepsbyWellAttr = value
            End Set
        End Property
        Public ReadOnly Property ReadedCountsResult() As List(Of Single)
            Get
                Dim returnValue As New List(Of Single)
                Dim myCalc As New CalculationsDelegate()
                ' first value
                returnValue.Add(myCalc.CalculateOpticalCorrection(Me.ReadedCountsAttr(0).CountsMain, Me.ReadedCountsAttr(0).CountsRef, Me.ReadedCountsAttr(0).CountsRef))
                ' next values
                For i As Integer = 1 To Me.ReadedCountsAttr.Count - 1
                    returnValue.Add(myCalc.CalculateOpticalCorrection(Me.ReadedCountsAttr(i).CountsMain, Me.ReadedCountsAttr(i).CountsRef, Me.ReadedCountsAttr(0).CountsRef))
                Next
                Return returnValue
            End Get
        End Property

        Public Property SampleSecurityFly() As String
            Get
                Return MyClass.pSampleSecurityFlyAttr
            End Get
            Set(ByVal value As String)
                MyClass.pSampleSecurityFlyAttr = value
            End Set
        End Property
        Public Property Reagent1SecurityFly() As String
            Get
                Return MyClass.pReagent1SecurityFlyAttr
            End Get
            Set(ByVal value As String)
                MyClass.pReagent1SecurityFlyAttr = value
            End Set
        End Property
        Public Property Reagent2SecurityFly() As String
            Get
                Return MyClass.pReagent2SecurityFlyAttr
            End Get
            Set(ByVal value As String)
                MyClass.pReagent2SecurityFlyAttr = value
            End Set
        End Property
        Public Property Mixer1SecurityFly() As String
            Get
                Return MyClass.pMixer1SecurityFlyAttr
            End Get
            Set(ByVal value As String)
                MyClass.pMixer1SecurityFlyAttr = value
            End Set
        End Property
        Public Property Mixer2SecurityFly() As String
            Get
                Return MyClass.pMixer2SecurityFlyAttr
            End Get
            Set(ByVal value As String)
                MyClass.pMixer2SecurityFlyAttr = value
            End Set
        End Property

        'HISTORY
        Public Property HistoryTask() As PreloadedMasterDataEnum
            Get
                Return HistoryTaskAttr
            End Get
            Set(ByVal value As PreloadedMasterDataEnum)
                HistoryTaskAttr = value
            End Set
        End Property

        Public Property HistoryArea() As HISTORY_AREAS
            Get
                Return HistoryAreaAttr
            End Get
            Set(ByVal value As HISTORY_AREAS)
                HistoryAreaAttr = value
            End Set
        End Property

        Public Property HisOpticCenteringTestResult() As HISTORY_RESULTS
            Get
                Return HisOpticCenteringTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisOpticCenteringTestResultAttr = value
            End Set
        End Property

        Public Property HisOpticCenteringAdjResult() As HISTORY_RESULTS
            Get
                Return HisOpticCenteringAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisOpticCenteringAdjResultAttr = value
            End Set
        End Property

        Public Property HisOpticCenteringValue() As Integer
            Get
                Return HisOpticCenteringValueAttr
            End Get
            Set(ByVal value As Integer)
                HisOpticCenteringValueAttr = value
            End Set
        End Property

        ' XBC 05/01/2012 - Add Encoder functionality
        Public Property HisEncoderValue() As Integer
            Get
                Return HisEncoderValueAttr
            End Get
            Set(ByVal value As Integer)
                HisEncoderValueAttr = value
            End Set
        End Property
        ' XBC 05/01/2012 - Add Encoder functionality

        Public Property HisLEDIntensity() As Integer
            Get
                Return HisLedIntensityAttr
            End Get
            Set(ByVal value As Integer)
                HisLedIntensityAttr = value
            End Set
        End Property

        Public Property HisWaveLength() As Integer
            Get
                Return HisWaveLengthAttr
            End Get
            Set(ByVal value As Integer)
                HisWaveLengthAttr = value
            End Set
        End Property

        Public Property HisNumWells() As Integer
            Get
                Return NumWellsAttr
            End Get
            Set(ByVal value As Integer)
                HisNumWellsAttr = value
            End Set
        End Property
        Public Property HisStepsbyWell() As Integer
            Get
                Return StepsbyWellAttr
            End Get
            Set(ByVal value As Integer)
                HisStepsByWellAttr = value
            End Set
        End Property


        Public Property HisWashingStationTestResult() As HISTORY_RESULTS
            Get
                Return HisWashingStationTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisWashingStationTestResultAttr = value
            End Set
        End Property

        Public Property HisWashingStationAdjResult() As HISTORY_RESULTS
            Get
                Return HisWashingStationAdjResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisWashingStationAdjResultAttr = value
            End Set
        End Property

        Public Property HisWashingStationValue() As Integer
            Get
                Return HisWashingStationValueAttr
            End Get
            Set(ByVal value As Integer)
                HisWashingStationValueAttr = value
            End Set
        End Property

        'arms

        Public Property HisArmAdjResults() As Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS)
            Get
                Return HisArmAdjResultsAttr
            End Get
            Set(ByVal value As Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS))
                HisArmAdjResultsAttr = value
            End Set
        End Property

        Public Property HisArmTestResults() As Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS)
            Get
                Return HisArmTestResultsAttr
            End Get
            Set(ByVal value As Dictionary(Of HISTORY_ARM_POSITIONS, HISTORY_RESULTS))
                HisArmTestResultsAttr = value
            End Set
        End Property

        Public Property HisArmAxesValues() As Dictionary(Of HISTORY_ARM_POSITIONS, List(Of Integer))
            Get
                Return HisArmAxesValuesAttr
            End Get
            Set(ByVal value As Dictionary(Of HISTORY_ARM_POSITIONS, List(Of Integer)))
                HisArmAxesValuesAttr = value
            End Set
        End Property

        Public Property HisArmPosName() As HISTORY_ARM_POSITIONS
            Get
                Return HisArmPosNameAttr
            End Get
            Set(ByVal value As HISTORY_ARM_POSITIONS)
                HisArmPosNameAttr = value
            End Set
        End Property

        Public Property HisStirrerTestResult() As HISTORY_RESULTS
            Get
                Return HisStirrerTestResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisStirrerTestResultAttr = value
            End Set
        End Property

        Public Property SampleArmPolarZREF() As Integer
            Get
                Return MyClass.SampleArmPolarZREFAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.SampleArmPolarZREFAttr = value
            End Set
        End Property

        Public Property Reagent1ArmPolarZREF() As Integer
            Get
                Return MyClass.Reagent1ArmPolarZREFAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.Reagent1ArmPolarZREFAttr = value
            End Set
        End Property

        Public Property Reagent2ArmPolarZREF() As Integer
            Get
                Return MyClass.Reagent2ArmPolarZREFAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.Reagent2ArmPolarZREFAttr = value
            End Set
        End Property

        Public Property Mixer1ArmPolarZREF() As Integer
            Get
                Return MyClass.Mixer1ArmPolarZREFAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.Mixer1ArmPolarZREFAttr = value
            End Set
        End Property

        Public Property Mixer2ArmPolarZREF() As Integer
            Get
                Return MyClass.Mixer2ArmPolarZREFAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.Mixer2ArmPolarZREFAttr = value
            End Set
        End Property

        Public Property IsWashingStationUp() As Boolean
            Get
                Return MyClass.IsWashingStationUpAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsWashingStationUpAttr = value
            End Set
        End Property

        ' XBC 21/12/2011 - Add Encoder functionality
        Public ReadOnly Property ReadedEncoderResult() As List(Of Single)
            Get
                Dim returnValue As New List(Of Single)

                For i As Integer = 0 To Me.ReadedCountsAttr.Count - 1
                    returnValue.Add(Me.ReadedCountsAttr(i).Encoder)
                Next

                Return returnValue
            End Get
        End Property

        ' XBC 27/02/2012
        Public ReadOnly Property Reagent1SV_ZOffset() As Single
            Get
                Return MyClass.Reagent1SV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent1WVR_ZOffset() As Single
            Get
                Return MyClass.Reagent1WVR_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent1DV_ZOffset() As Single
            Get
                Return MyClass.Reagent1DV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent1PI_ZOffset() As Single
            Get
                Return MyClass.Reagent1PI_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent2SV_ZOffset() As Single
            Get
                Return MyClass.Reagent2SV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent2WVR_ZOffset() As Single
            Get
                Return MyClass.Reagent2WVR_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent2DV_ZOffset() As Single
            Get
                Return MyClass.Reagent2DV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Reagent2PI_ZOffset() As Single
            Get
                Return MyClass.Reagent2PI_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleSV_ZOffset() As Single
            Get
                Return MyClass.SampleSV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleWVR_ZOffset() As Single
            Get
                Return MyClass.SampleWVR_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleDV1_ZOffset() As Single
            Get
                Return MyClass.SampleDV1_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SamplePI_ZOffset() As Single
            Get
                Return MyClass.SamplePI_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleRPI_ZOffset() As Single
            Get
                Return MyClass.SampleRPI_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property SampleDV2_ZOffset() As Single
            Get
                Return MyClass.SampleDV2_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer1SV_ZOffset() As Single
            Get
                Return MyClass.Mixer1SV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer1WVR_ZOffset() As Single
            Get
                Return MyClass.Mixer1WVR_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer1DV_ZOffset() As Single
            Get
                Return MyClass.Mixer1DV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer2SV_ZOffset() As Single
            Get
                Return MyClass.Mixer2SV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer2WVR_ZOffset() As Single
            Get
                Return MyClass.Mixer2WVR_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property Mixer2DV_ZOffset() As Single
            Get
                Return MyClass.Mixer2DV_ZOffsetAttr
            End Get
        End Property
        Public ReadOnly Property WashingStationRR_ZOffset() As Single
            Get
                Return MyClass.WashingStationRR_ZOffsetAttr
            End Get
        End Property
        ' XBC 27/02/2012

        ' XBC 18/04/2012
        Public ReadOnly Property WSAdjustPrepared() As Boolean
            Get
                Return MyClass.WSAdjustPreparedAttr
            End Get
        End Property
#End Region

#Region "Enumerations"
        Private Enum OPERATIONS
            NONE
            ABSORBANCESCAN
            ABSORBANCE_PREPARE
            SAVE_ADJUSMENTS
            SAVE_EXITING
            WASHING_STATION_UP
            WASHING_STATION_DOWN
        End Enum

        Public Enum HISTORY_AREAS
            NONE = 0
            OPT = 1
            WS_POS = 2
            SAM_POS = 3
            REG1_POS = 4
            REG2_POS = 5
            MIX1_POS = 6
            MIX2_POS = 7
            MIX1_TEST = 8
            MIX2_TEST = 9
        End Enum

        Public Enum HISTORY_RESULTS
            _ERROR = -1
            NOT_DONE = 0
            OK = 1
            NOK = 2
            CANCEL = 3
            NOT_APPLIED = 4
        End Enum

        Public Enum HISTORY_ARM_NAMES
            None = 0
            SAMPLE = 1
            REAGENT1 = 2
            REAGENT2 = 3
            MIXER1 = 4
            MIXER2 = 5
        End Enum

        Public Enum HISTORY_ARM_POSITIONS
            None = 0
            Dispensation = 1
            Predilution = 3
            Z_Ref = 4
            Washing = 5
            Ring1 = 6
            Ring2 = 7
            Ring3 = 8
            Z_Tube = 9
            ISE_Pos = 10
            Parking = 11
        End Enum
#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 06/05/2011</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' XBC 05/05/2011 - timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        MyClass.UpdateRecommendationsList(HISTORY_RECOMMENDATIONS.ERR_COMM)
                    End If

                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions

                Select Case CurrentOperation
                    Case OPERATIONS.ABSORBANCESCAN
                        If pResponse = RESPONSE_TYPES.OK Then
                            Me.AbsorbanceScanReadingsAttr += 1
                            ' Read counts done !
                            myGlobal = ManageCountsMeasured()
                            If Not myGlobal.HasError Then
                                Debug.Print(Me.AbsorbanceScanReadingsAttr.ToString & "_ " & _
                                            ReadedCountsAttr(Me.AbsorbanceScanReadingsAttr - 1).CountsMain.ToString & " " & _
                                            ReadedCountsAttr(Me.AbsorbanceScanReadingsAttr - 1).CountsRef.ToString & " " & _
                                            ReadedCountsAttr(Me.AbsorbanceScanReadingsAttr - 1).Encoder.ToString)
                                If Me.AbsorbanceScanReadingsAttr >= Me.NumWellsAttr * Me.StepsbyWellAttr Then
                                    If Not myGlobal.HasError Then
                                        myGlobal = InvertCountsMeasured()   ' XBC 05/01/2012 - Add Encoder functionality
                                        If Not myGlobal.HasError Then
                                            Me.AbsorbanceScanDoneAttr = True
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    Case OPERATIONS.SAVE_ADJUSMENTS
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                Me.LoadAdjDoneAttr = True
                        End Select

                    Case OPERATIONS.SAVE_EXITING
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                Me.ParkDoneAttr = True
                        End Select

                        ' XBC 30/11/2011
                    Case OPERATIONS.WASHING_STATION_UP
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = True
                        End Select

                        ' XBC 30/11/2011
                    Case OPERATIONS.WASHING_STATION_DOWN
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationUpAttr = False
                        End Select


                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <param name="pAdjustmentGroup">Adjustment type</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 17/11/10
        ''' Modified by XBC 04/01/11 - add pAdjustment parameter
        '''              XB 15/10/2014 - Use FCK to fine optical centering - BA-2004 
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES, _
                                               Optional ByVal pAdjustmentGroup As ADJUSTMENT_GROUPS = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.ADJUST_PREPARING
                        myResultData = Me.SendQueueForADJUST_PREPARING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.ABSORBANCE_SCANNING
                        myResultData = Me.SendQueueForABSORBANCESCAN() 'SGM 14/03/11

                    Case ADJUSTMENT_MODES.ABSORBANCE_PREPARING
                        myResultData = Me.SendQueueForABSORBANCE_PREPARE() 'XBC 23/09/11

                    Case ADJUSTMENT_MODES.ADJUSTING
                        myResultData = Me.SendQueueForADJUSTING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.ADJUST_EXITING
                        myResultData = Me.SendQueueForADJUST_EXITING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.TESTING
                        myResultData = Me.SendQueueForTESTING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.TEST_EXITING
                        myResultData = Me.SendQueueForTEST_EXITING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.SAVING
                        myResultData = Me.SendQueueForSAVING(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.STIRRER_TEST
                        myResultData = Me.SendQueueForMIXER_ON(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.STIRRER_TESTING
                        myResultData = Me.SendQueueForMIXER_OFF(pAdjustmentGroup)

                    Case ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_PERFORMING
                        myResultData = Me.SendQueueForFINE_OPTICAL_CENTERING()

                    Case Else
                        Debug.Print("Aturat !!!")

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the list of available Arm Positions to load into a specified Control
        ''' </summary>
        ''' <param name="pAdjustPoint">Type of Arm</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>Created by : XBC 03/01/2011</remarks>
        Public Function ReadPositionsValues(ByVal pAdjustPoint As GlobalEnumerates.PreloadedMasterDataEnum) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = LoadPositionsValuesTO(pAdjustPoint)
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ReadPositionsValues", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the list of available Arm Movements to load into a specified Control
        ''' </summary>
        ''' <param name="pAdjustPoint">Type of Arm</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>Created by : XBC 03/01/2011</remarks>
        Public Function ReadMovementsValues(ByVal pAdjustPoint As GlobalEnumerates.PreloadedMasterDataEnum) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                myResultData = LoadMovementsValuesTO(pAdjustPoint)
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ReadMovementsValues", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the Positioning Adj Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : XBC 06/05/2011</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            'Dim myGlobalbase As New GlobalBase
            Try
                ' Offet Washing Station for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WASHING_STATION_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pWashingStationOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Offet Sample Arm for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_SAMPLE_ARM_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pSampleArmOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Offet Reagent1 Arm for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REAGENT1_ARM_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pReagent1ArmOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Offet Reagent2 Arm for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REAGENT2_ARM_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pReagent2ArmOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Offet Mixer1 Arm for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MIXER1_ARM_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pMixer1ArmOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Offet Mixer2 Arm for Z aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MIXER2_ARM_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    pMixer2ArmOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Wavelength by default to Optic Centering
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WAVELENGTH_OPTICAL_CENTERING.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    WaveLengthAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                MyClass.NumWellsAttr = GlobalBase.OpticalCenteringWellsToRead
                MyClass.StepsbyWellAttr = GlobalBase.OpticalCenteringStepsbyWell

                ' Samples Arm ZRef position for Polar aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_SAMPLE_ARM_ZREF_POLAR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleArmPolarZREFAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Reagent1 Arm ZRef position for Polar aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REAGENT1_ARM_ZREF_POLAR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent1ArmPolarZREFAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Reagent2 Arm ZRef position for Polar aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REAGENT2_ARM_ZREF_POLAR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent2ArmPolarZREFAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Mixer1 Arm ZRef position for Polar aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MIXER1_ARM_ZREF_POLAR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer1ArmPolarZREFAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Mixer2 Arm ZRef position for Polar aproximation
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MIXER2_ARM_ZREF_POLAR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer2ArmPolarZREFAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' XBC 27/02/2012
                ' ZRef Offset for Safety position Reagent1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R1SV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent1SV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Washing Station position Reagent1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R1WVR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent1WVR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Dispensation position Reagent1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R1DV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent1DV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Detection Level position Reagent1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R1PI_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent1PI_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Safety position Reagent2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R2SV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent2SV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Washing Station position Reagent2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R2WVR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent2WVR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Dispensation position Reagent2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R2DV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent2DV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Detection Level position Reagent2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_R2PI_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Reagent2PI_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Safety position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1SV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleSV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Washing Station position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1WVR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleWVR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Dispensation one position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1DV1_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleDV1_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Detection Level position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1PI_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SamplePI_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for initial detection level Reactions rotor position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1RPI_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleRPI_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for max detection level Reactions Rotor position Sample
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_M1DV2_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    SampleDV2_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Safety position Mixer1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A1SV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer1SV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Washing Station position Mixer1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A1WVR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer1WVR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for shake vertical position Mixer1
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A1DV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer1DV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' ZRef Offset for Safety position Mixer2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A2SV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer2SV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for Washing Station position Mixer2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A2WVR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer2WVR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for shake vertical position Mixer2
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_A2DV_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    Mixer2DV_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' ZRef Offset for ready to wash position Washing Station
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_REFZ_WSRR_OFFSET.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    WashingStationRR_ZOffsetAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' XBC 27/02/2012

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Sub ManageArmsParkingStatus(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal pValue As Boolean)
            Try
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                        ' sample arm 
                        Me.pSampleArmParked = pValue

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                        ' reagent1 arm 
                        Me.pReagent1ArmParked = pValue

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                        ' reagent2 arm
                        Me.pReagent2ArmParked = pValue

                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                        ' mixer1 arm 
                        Me.pMixer1ArmParked = pValue

                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        ' mixer2 arm 
                        Me.pMixer2ArmParked = pValue

                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ManageArmsParkingStatus", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Load Adjustments High Level Instruction to save values into the instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 06/05/2011</remarks>
        Public Function SendLoad_Adjustments() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Me.CurrentOperation = OPERATIONS.SAVE_ADJUSMENTS
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, Me.pValueAdjustAttr) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        '''' <summary>
        '''' Constructs the Adjustment structure data for read values from Instrument
        '''' </summary>
        '''' <returns>GlobalDataTO containing the configured structure of Fw Adjustments</returns>
        '''' <remarks>Created by : XBC 12/01/2011</remarks>
        'Public Function CreateFwAdjustmentsData() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try
        '        Dim myPosition As New PositionTO
        '        Dim myFwAdjustmentsData As New FwAdjustmentsDataTO
        '        Dim myPreloadedMasterDataDS As New PreloadedMasterDataDS

        '        'SGX
        '        ' OPTIC CENTERING
        '        'myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_PHOTOMETRY)
        '        'If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '        '    myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '        '    For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '        myPosition = New PositionTO
        '        myPosition.Id = ""
        '        myPosition.Polar = ""
        '        myPosition.Z = ""
        '        myPosition.Rotor = ""
        '        myFwAdjustmentsData.OpticCentering.PositionArm.Add(myPosition)
        '        '    Next
        '        'End If

        '        ' WASHING STATION
        '        'myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_WASHING_STATION)
        '        'If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '        '    myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '        '    For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '        myPosition = New PositionTO
        '        myPosition.Id = ""
        '        myPosition.Polar = ""
        '        myPosition.Z = ""
        '        myPosition.Rotor = ""
        '        myFwAdjustmentsData.WashingStation.PositionArm.Add(myPosition)
        '        '    Next
        '        'End If




        '        ' SAMPLE ARM
        '        If Not myResultData.HasError Then
        '            myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_SAMPLE_POSITIONS)
        '            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '                    myPosition = New PositionTO
        '                    myPosition.Id = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
        '                    myPosition.Polar = ""
        '                    myPosition.Z = ""
        '                    myPosition.Rotor = ""
        '                    myFwAdjustmentsData.SampleArm.PositionArm.Add(myPosition)
        '                Next
        '            End If
        '        End If

        '        ' REAGENT1 ARM
        '        If Not myResultData.HasError Then
        '            myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_REAG_POSITIONS)
        '            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '                    myPosition = New PositionTO
        '                    myPosition.Id = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
        '                    myPosition.Polar = ""
        '                    myPosition.Z = ""
        '                    myPosition.Rotor = ""
        '                    myFwAdjustmentsData.Reagent1Arm.PositionArm.Add(myPosition)
        '                Next
        '            End If
        '        End If

        '        ' REAGENT2 ARM
        '        If Not myResultData.HasError Then
        '            myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_REAG_POSITIONS)
        '            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '                    myPosition = New PositionTO
        '                    myPosition.Id = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
        '                    myPosition.Polar = ""
        '                    myPosition.Z = ""
        '                    myPosition.Rotor = ""
        '                    myFwAdjustmentsData.Reagent2Arm.PositionArm.Add(myPosition)
        '                Next
        '            End If
        '        End If

        '        ' MIXER1 ARM
        '        If Not myResultData.HasError Then
        '            myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_MIXER_POSITIONS)
        '            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '                    myPosition = New PositionTO
        '                    myPosition.Id = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
        '                    myPosition.Polar = ""
        '                    myPosition.Z = ""
        '                    myPosition.Rotor = ""
        '                    myFwAdjustmentsData.Mixer1Arm.PositionArm.Add(myPosition)
        '                Next
        '            End If
        '        End If

        '        ' MIXER2 ARM
        '        If Not myResultData.HasError Then
        '            myResultData = ReadPositionsValues(PreloadedMasterDataEnum.SRV_MIXER_POSITIONS)
        '            If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
        '                myPreloadedMasterDataDS = DirectCast(myResultData.SetDatos, PreloadedMasterDataDS)
        '                For i As Integer = 0 To myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count - 1
        '                    myPosition = New PositionTO
        '                    myPosition.Id = myPreloadedMasterDataDS.tfmwPreloadedMasterData(i).ItemID
        '                    myPosition.Polar = ""
        '                    myPosition.Z = ""
        '                    myPosition.Rotor = ""
        '                    myFwAdjustmentsData.Mixer2Arm.PositionArm.Add(myPosition)
        '                Next
        '            End If
        '        End If

        '        If Not myResultData.HasError Then
        '            myResultData.SetDatos = myFwAdjustmentsData
        '        End If
        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.CreateFwAdjustmentsData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to move Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 30/11/2011</remarks>
        Public Function SendWASH_STATION_CTRL(ByVal pAction As Ax00WashStationControlModes) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Select Case pAction
                    Case Ax00WashStationControlModes.UP
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_UP
                    Case Ax00WashStationControlModes.DOWN
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_DOWN
                End Select
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, pAction) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendWASH_STATION_CTRL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' NRotor High Level Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XB 14/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004</remarks>
        Public Function SendNEW_ROTOR() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try

                CurrentOperation = OPERATIONS.WASHING_STATION_DOWN

                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendNEW_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 05/12/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Private Methods"
        Private Function GetArmsParkingStatus(ByVal pAdjustment As ADJUSTMENT_GROUPS) As Boolean
            Dim returnValue As Boolean
            Try
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                        ' sample arm 
                        returnValue = Me.pSampleArmParked

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                        ' reagent1 arm 
                        returnValue = Me.pReagent1ArmParked

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                        ' reagent2 arm
                        returnValue = Me.pReagent2ArmParked

                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                        ' mixer1 arm 
                        returnValue = Me.pMixer1ArmParked

                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        ' mixer2 arm 
                        returnValue = Me.pMixer2ArmParked

                End Select

                Return returnValue

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.GetArmsParkingStatus", EventLogEntryType.Error, False)
            End Try
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Loading operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10
        ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
        ''' </remarks>
        Private Function SendQueueForADJUST_PREPARING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript0 As New FwScriptQueueItem ' XBC 28/03/2012 - Add previous movement to security fly position
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem

            Try
                HomesDoneAttr = False
                WSAdjustPreparedAttr = False

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If


                'sg 25/1/11
                'get the pending Homes  
                Dim myHomes As New tadjPreliminaryHomesDAO
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    GetPreliminaryHomeScripts(pAdjustment, myResultData, myListFwScript, myFwScript0)
                End If

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.PHOTOMETRY
                        myResultData = GetPhotometryScripts(myResultData, myListFwScript, myFwScript0, myFwScript1)

                    Case ADJUSTMENT_GROUPS.WASHING_STATION
                        myResultData = GetWashingStationScripts(myResultData, myListFwScript)

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1
                        myResultData = GetDispensationArmScripts(pAdjustment, myResultData, myListFwScript, myFwScript0, myFwScript1)

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_WASH
                        myResultData = GetWashingArmScripts(pAdjustment, myResultData, myListFwScript, myFwScript0, myFwScript1)

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                        myResultData = GetRingByArmScripts(pAdjustment, myResultData, myListFwScript, myFwScript0, myFwScript1, myFwScript2)

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1
                        myResultData = GetSampleArmZTubeScripts(myResultData, myListFwScript, myFwScript0, myFwScript1, myFwScript2)

                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        myResultData = GetArmZrefParkISEScripts(pAdjustment, myResultData, myListFwScript, myFwScript0, myFwScript1)

                End Select

                ManageArmsParkingStatus(pAdjustment, False)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForADJUST_PREPARING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Private Sub GetPreliminaryHomeScripts(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem)
            Dim myHomesDS As SRVPreliminaryHomesDS

            myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

            Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                    Where a.Done = False Select a).ToList

            For Each H In myPendingHomesList
                Dim myFwScript As New FwScriptQueueItem
                myListFwScript.Add(myFwScript)
            Next

            Dim i As Integer = 0
            For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                If i = myListFwScript.Count - 1 Then
                    'Last index
                    With myListFwScript(i)
                        .FwScriptID = H.RequiredHomeID.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = myFwScript0   ' XBC 28/03/2012 - Add previous movement to security fly position
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = myFwScript0   ' XBC 28/03/2012 - Add previous movement to security fly position
                        .NextOnError = Nothing
                        .ParamList = Nothing

                        ' XBC 28/03/2012 - Add previous movement to security fly position
                        If pAdjustment = ADJUSTMENT_GROUPS.WASHING_STATION Then
                            .NextOnResultOK = Nothing
                            .NextOnTimeOut = Nothing
                        End If
                        ' XBC 28/03/2012 - Add previous movement to security fly position
                    End With
                Else
                    With myListFwScript(i)
                        .FwScriptID = H.RequiredHomeID.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = myListFwScript(i + 1)
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = myListFwScript(i + 1)
                        .NextOnError = Nothing
                        .ParamList = Nothing
                    End With
                End If
                i += 1
            Next
        End Sub

        Private Function GetArmZrefParkISEScripts(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem) As GlobalDataTO

            ' XBC 28/03/2012 - Add previous movement to security fly position
            With myFwScript0
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                        ' sample arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pSampleSecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                        ' reagent1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                        ' reagent2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent2SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                        ' mixer1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        ' mixer2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                End Select

            End With
            ' XBC 28/03/2012 - Add previous movement to security fly position

            ' Absolute positioning of the sample arm axis to last adjustment position
            With myFwScript1
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                        ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_DISP.ToString
                End Select
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pArmABSMovPolarAttr)
            End With
            ' XBC 18/04/2012 - Z aproximation is anuled

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
            End If
            Return myResultData
        End Function

        Private Function GetSampleArmZTubeScripts(ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem, ByVal myFwScript2 As FwScriptQueueItem) As GlobalDataTO

            ' XBC 28/03/2012 - Add previous movement to security fly position
            With myFwScript0
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing
                ' sample arm Move Z to security fly
                .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pSampleSecurityFlyAttr)

            End With
            ' XBC 28/03/2012 - Add previous movement to security fly position

            ' Mov ABS Polar
            ' Absolute positioning of the sample arm axis to last adjustment position
            With myFwScript1
                .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_POLAR.ToString
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript2
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript2
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pArmABSMovPolarAttr)
            End With

            ' Mov REL Reactions Rotor
            ' Relative positioning of the reactions rotor to a predefined position (wells) 
            With myFwScript2
                .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing   ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing    ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pValueRotorABSMovAttr)
            End With

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
            End If
            Return myResultData
        End Function

        Private Function GetRingByArmScripts(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem, ByVal myFwScript2 As FwScriptQueueItem) As GlobalDataTO

            ' XBC 28/03/2012 - Add previous movement to security fly position
            With myFwScript0
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3
                        ' sample arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pSampleSecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2
                        ' reagent1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                        ' reagent2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent2SecurityFlyAttr)

                End Select

            End With
            ' XBC 28/03/2012 - Add previous movement to security fly position

            ' Mov ABS Polar
            ' Absolute positioning of the sample arm axis to last adjustment position
            With myFwScript1
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_POLAR.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_POLAR.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_POLAR.ToString
                End Select
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript2
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript2
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pArmABSMovPolarAttr)
            End With

            ' Mov REL Reactions Rotor
            ' Relative positioning of the reactions rotor to a predefined position (wells) 
            With myFwScript2
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                        ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                        .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                End Select
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing   ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing    ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pValueRotorABSMovAttr)
            End With

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
            End If
            Return myResultData
        End Function

        Private Function GetWashingArmScripts(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem) As GlobalDataTO

            ' XBC 28/03/2012 - Add previous movement to security fly position
            With myFwScript0
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH
                        ' sample arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pSampleSecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH
                        ' reagent1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH
                        ' reagent2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent2SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_WASH
                        ' mixer1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_WASH
                        ' mixer2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                End Select

            End With

            ' Absolute positioning of the sample arm axis to last adjustment position
            With myFwScript1
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_WASH
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_WASH
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_DISP.ToString
                End Select
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pArmABSMovPolarAttr)
            End With
            ' XBC 18/04/2012 - Z aproximation is anuled

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
            End If
            Return myResultData
        End Function

        Private Function GetDispensationArmScripts(ByVal pAdjustment As ADJUSTMENT_GROUPS, ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem) As GlobalDataTO

            ' XBC 28/03/2012 - Add previous movement to security fly position
            With myFwScript0
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2
                        ' sample arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pSampleSecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1
                        ' reagent1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1
                        ' reagent2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pReagent2SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1
                        ' mixer1 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer1SecurityFlyAttr)

                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1
                        ' mixer2 arm Move Z to security fly
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_Z.ToString
                        ' expects 1 param
                        .ParamList = New List(Of String)
                        .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                End Select

            End With
            ' XBC 28/03/2012 - Add previous movement to security fly position



            ' Mov ABS Polar
            ' Absolute positioning of the sample arm axis to last adjustment position
            With myFwScript1
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                        ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2
                        .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1
                        .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1
                        .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_DISP.ToString
                    Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1
                        .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_DISP.ToString
                End Select
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing   ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing    ' XBC 18/04/2012 - Z aproximation is anuled
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pArmABSMovPolarAttr)
            End With

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False) ' XBC 18/04/2012 - Z aproximation is anuled
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 28/03/2012 - Add previous movement to security fly position
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False) ' XBC 18/04/2012 - Z aproximation is anuled
            End If
            Return myResultData
        End Function

        Private Function GetWashingStationScripts(ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem)) As GlobalDataTO

            'add to the queue list
            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 18/04/2012 - Z aproximation is anuled
            Else
                ' XBC 18/04/2012 - Z aproximation is anuled
                'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True)  
                MyClass.NoneInstructionToSend = False
                MyClass.WSAdjustPreparedAttr = True
                ' XBC 18/04/2012 - Z aproximation is anuled
            End If
            Return myResultData
        End Function

        Private Function GetPhotometryScripts(ByVal myResultData As GlobalDataTO, ByVal myListFwScript As List(Of FwScriptQueueItem), ByVal myFwScript0 As FwScriptQueueItem, ByVal myFwScript1 As FwScriptQueueItem) As GlobalDataTO

            'AG 01/10/2014 - BA-1953 'After home move abs to the current value of adjustment GFWR1
            With myFwScript0
                .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString   ' REACTIONS_REL_ROTOR
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = myFwScript1
                .NextOnResultNG = Nothing
                .NextOnTimeOut = myFwScript1
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add(Me.pValueAdjustAttr)
            End With

            'With myFwScript0
            'AG 01/10/2014 - BA-1953

            ' Relative movement to place close the wall between 5-6
            With myFwScript1
                .FwScriptID = FwSCRIPTS_IDS.REACTIONS_REL_ROTOR_2SECONDS.ToString   ' REACTIONS_REL_ROTOR
                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                .EvaluateValue = 1
                .NextOnResultOK = Nothing
                .NextOnResultNG = Nothing
                .NextOnTimeOut = Nothing
                .NextOnError = Nothing
                ' expects 1 param
                .ParamList = New List(Of String)
                .ParamList.Add("360")
            End With

            If myListFwScript.Count > 0 Then
                For i As Integer = 0 To myListFwScript.Count - 1
                    If i = 0 Then
                        ' First Script
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                    Else
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                    End If
                Next
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False) 'AG 01/10/2014 - BA-1953
            Else
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False) 'AG 01/10/2014 - BA-1953
            End If

            ' XBC 04/01/2012 - Add Encoder functionality


            ' XBC 09/05/2012
            myFwScriptDelegate.INFOManagementEnabled = False
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Loading operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 17/11/10
        ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver for REACTIONS HOME (use REAGENTS_HOME_ROTOR + REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
        ''' </remarks>
        Private Function SendQueueForADJUSTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem 'AG 01/10/2014 BA-1953
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    Select Case pAdjustment

                        Case ADJUSTMENT_GROUPS.PHOTOMETRY
                            Select Case pAxisAdjustAttr
                                Case AXIS.ROTOR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reactions rotor Home
                                            'AG 01/10/2014 BA-1953
                                            '.FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString
                                            '.ParamList = Nothing
                                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_HOME_ROTOR.ToString
                                            .ParamList = Nothing
                                            .NextOnResultOK = myFwScript2
                                            .NextOnTimeOut = myFwScript2
                                            'AG 01/10/2014 BA-1953

                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS
                                            ' Absolute positioning of the reactions rotor
                                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL
                                            ' Relative positioning of the reactions rotor for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_REL_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                        Case ADJUSTMENT_GROUPS.WASHING_STATION
                            Select Case pAxisAdjustAttr
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' washing station Home
                                            .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS 
                                            ' Absolute positioning of the sample arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL 
                                            ' Relative positioning of the sample arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                        Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK

                            Select Case pAxisAdjustAttr
                                Case AXIS.POLAR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' sample arm Polar Home
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_HOME_POLAR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Polar
                                            ' Absolute positioning of the sample arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Polar
                                            ' Relative positioning of the sample arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_REL_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' sample arm Z Home
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Z
                                            ' Absolute positioning of the sample arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Z
                                            ' Relative positioning of the sample arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.ROTOR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' samples rotor Home
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLES_HOME_ROTOR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Rotor
                                            ' Relative positioning of the samples rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Rotor
                                            ' Relative positioning of the samples rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.SAMPLES_REL_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                        Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK

                            Select Case pAxisAdjustAttr
                                Case AXIS.POLAR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagent1 Polar Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_HOME_POLAR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Polar
                                            ' Absolute positioning of the reagent1 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Polar
                                            ' Relative positioning of the reagent1 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_REL_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagent1 Z Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Z
                                            ' Absolute positioning of the reagent1 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Z
                                            ' Relative positioning of the reagent1 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.ROTOR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagents rotor Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_HOME_ROTOR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Rotor
                                            ' Relative positioning of the reagents rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Rotor
                                            ' Relative positioning of the reagents rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_REL_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                        Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK

                            Select Case pAxisAdjustAttr
                                Case AXIS.POLAR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagent2 Polar Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_HOME_POLAR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Polar
                                            ' Absolute positioning of the reagent2 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Polar
                                            ' Relative positioning of the reagent2 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_REL_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagent2 Z Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Z
                                            ' Absolute positioning of the reagent2 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Z
                                            ' Relative positioning of the reagent2 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.ROTOR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' reagents rotor Home
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_HOME_ROTOR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Rotor
                                            ' Relative positioning of the reagents rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Rotor
                                            ' Relative positioning of the reagents rotor axis for x wells
                                            .FwScriptID = FwSCRIPTS_IDS.REAGENTS_REL_ROTOR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_PARK

                            Select Case pAxisAdjustAttr
                                Case AXIS.POLAR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' mixer1 Polar Home
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_HOME_POLAR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Polar
                                            ' Absolute positioning of the mixer1 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Polar
                                            ' Relative positioning of the mixer1 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_REL_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' mixer1 Z Home
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Z
                                            ' Absolute positioning of the mixer1 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Z
                                            ' Relative positioning of the mixer1 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select


                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_PARK

                            Select Case pAxisAdjustAttr
                                Case AXIS.POLAR
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' mixer2 Polar Home
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_HOME_POLAR.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Polar
                                            ' Absolute positioning of the mixer2 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Polar
                                            ' Relative positioning of the mixer2 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_REL_POLAR.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                                Case AXIS.Z
                                    Select Case pMovAdjust
                                        Case MOVEMENT.HOME
                                            ' mixer2 Z Home
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_HOME_Z.ToString
                                            .ParamList = Nothing
                                        Case MOVEMENT.ABSOLUTE
                                            ' Mov ABS Z
                                            ' Absolute positioning of the mixer2 arm axis to x adjustment position
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                        Case MOVEMENT.RELATIVE
                                            ' Mov REL Z
                                            ' Relative positioning of the mixer2 arm axis for x steps
                                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_REL_Z.ToString
                                            ' expects 1 param
                                            .ParamList = New List(Of String)
                                            .ParamList.Add(Me.pValueAdjustAttr)
                                    End Select
                            End Select

                    End Select

                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)


                'AG 01/10/2014 - BA-1953 apply only pAdjustment = PHOTOMETRY, pAxisAdjustAttr = ROTOR and pMovAdjust = HOME
                'After home move abs to the current value of adjustment GFWR1
                If Not myResultData.HasError Then
                    If pAdjustment = ADJUSTMENT_GROUPS.PHOTOMETRY AndAlso pAxisAdjustAttr = AXIS.ROTOR AndAlso pMovAdjustAttr = MOVEMENT.HOME Then
                        With myFwScript2
                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString   ' REACTIONS_REL_ROTOR
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pValueAdjustAttr)
                        End With
                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
                    End If
                End If
                'AG 01/10/2014 - BA-1953

                ManageArmsParkingStatus(pAdjustment, False)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForADJUSTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Exiting of Adjust operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 04/01/11</remarks>
        Private Function SendQueueForADJUST_EXITING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                If GetArmsParkingStatus(pAdjustment) Then
                    Me.NoneInstructionToSend = False
                    Me.ParkDoneAttr = True
                    Exit Try
                End If

                ' Move to parking position the current arm
                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    Select Case pAdjustment
                        Case ADJUSTMENT_GROUPS.PHOTOMETRY
                            Me.NoneInstructionToSend = False
                            Me.ParkDoneAttr = True
                            Exit Try

                        Case ADJUSTMENT_GROUPS.WASHING_STATION
                            ' sample arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_PARK.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pWashingStationParkAttr)

                        Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                            ' sample arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pSampleSecurityFlyAttr)
                            .ParamList.Add(Me.pSampleArmParkHAttr)

                        Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                            ' reagent1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent1SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent1ArmParkHAttr)

                        Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                            ' reagent2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent2SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent2ArmParkHAttr)

                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer1SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer1ArmParkHAttr)

                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer2ArmParkHAttr)
                    End Select

                End With
                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                ManageArmsParkingStatus(pAdjustment, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForADJUST_EXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Saving operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 04/01/2011
        ''' Modified by XBC 06/05/2011 - LOAD/SAVE operation is deleted into this funtion because has been replaced as HIGH LEVEL instruction (SendLOAD_ADJUSTMENTS)
        ''' </remarks>
        Private Function SendQueueForSAVING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                If GetArmsParkingStatus(pAdjustment) Then
                    Me.NoneInstructionToSend = False
                    Me.ParkDoneAttr = True
                    Exit Try
                End If

                Me.CurrentOperation = OPERATIONS.SAVE_EXITING

                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 2
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    Select Case pAdjustment
                        'Case ADJUSTMENT_GROUPS.PHOTOMETRY
                        '    Me.NoneInstructionToSend = False
                        '    Me.ParkDoneAttr = True
                        '    Exit Try

                        Case ADJUSTMENT_GROUPS.WASHING_STATION
                            ' washing station Parking
                            .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_PARK.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pWashingStationParkAttr)

                        Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                            ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                            ' sample arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pSampleSecurityFlyAttr)
                            .ParamList.Add(Me.pSampleArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                            ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                            ' reagent1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent1SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent1ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                            ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                            'reagent2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent2SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent2ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer1SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer1ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                            ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer2ArmParkHAttr)
                    End Select

                    'add to the queue list
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End With

                ManageArmsParkingStatus(pAdjustment, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForSAVING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Testing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 04/01/11</remarks>
        Private Function SendQueueForTESTING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript0 As New FwScriptQueueItem ' XBC 06/02/2012 - Add previous movement to security fly position
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Dim myMovAproxZ As Single
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'get the pending Homes
                Dim myHomes As New tadjPreliminaryHomesDAO
                Dim myHomesDS As SRVPreliminaryHomesDS
                myResultData = myHomes.GetPreliminaryHomesByAdjID(Nothing, AnalyzerId, pAdjustment.ToString)
                If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
                    myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

                    Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
                                    (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
                                    Where a.Done = False Select a).ToList


                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
                        Dim myFwScript As New FwScriptQueueItem
                        myListFwScript.Add(myFwScript)
                    Next

                    Dim i As Integer = 0
                    For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList

                        'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
                        If i = myListFwScript.Count - 1 Then
                            'Last index
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myFwScript0   ' XBC 06/02/2012 - Add previous movement to security fly position
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myFwScript0    ' XBC 06/02/2012 - Add previous movement to security fly position
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        Else
                            With myListFwScript(i)
                                .FwScriptID = H.RequiredHomeID.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myListFwScript(i + 1)
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = myListFwScript(i + 1)
                                .NextOnError = Nothing
                                .ParamList = Nothing
                            End With
                        End If
                        i += 1

                    Next

                End If

                ' XBC 06/02/2012 - Add previous movement to security fly position
                ' Move to parking position the current arm
                With myFwScript0
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = myFwScript1
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = myFwScript1
                    .NextOnError = Nothing

                    Select Case pAdjustment
                        Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                            ' sample arm Move Z to security fly
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_Z.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pSampleSecurityFlyAttr)

                        Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                            ' reagent1 arm Move Z to security fly
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_Z.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent1SecurityFlyAttr)

                        Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                             ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                            ' reagent2 arm Move Z to security fly
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_Z.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent2SecurityFlyAttr)

                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 arm Move Z to security fly
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_Z.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer1SecurityFlyAttr)

                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 arm Move Z to security fly
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_Z.ToString
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                    End Select

                End With
                ' XBC 06/02/2012 - Add previous movement to security fly position

                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                         ADJUSTMENT_GROUPS.MIXER2_ARM_PARK

                        ' Arm Absolute Pos-Down 
                        ' Move absolute position the current arm:  predefined approaching position (steps)
                        With myFwScript1
                            Select Case pAdjustment
                                Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                                    ' sample arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                                     ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                                     ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                                     ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                                    ' reagent1 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                                    ' reagent2 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                                     ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                                     ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                                     ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                                    ' mixer1 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                                     ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                                     ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                                     ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                                    ' mixer2 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                            End Select
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pArmABSMovPolarAttr.ToString)
                            .ParamList.Add(myMovAproxZ.ToString)
                        End With


                        'add to the queue list
                        If myListFwScript.Count > 0 Then
                            For i As Integer = 0 To myListFwScript.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                                End If
                            Next
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 06/02/2012 - Add previous movement to security fly position
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 06/02/2012 - Add previous movement to security fly position
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        End If


                    Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                         ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                         ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2

                        ' Mov REL Reactions Rotor
                        ' Relative positioning of the reactions rotor to a predefined position (wells) 
                        With myFwScript1
                            Select Case pAdjustment
                                Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLES_ABS_ROTOR.ToString
                                Case ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                            End Select
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = myFwScript2
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = myFwScript2
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pValueRotorABSMovAttr)
                        End With

                        ' Arm Absolute Pos-Down 
                        ' Move absolute position the current arm:  predefined approaching position (steps)
                        With myFwScript2
                            Select Case pAdjustment
                                Case ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                                     ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1
                                    ' sample arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2
                                    ' reagent1 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                                Case ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                                     ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2
                                    ' reagent2 arm Parking
                                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_DOWN.ToString
                                    myMovAproxZ = CSng(Me.pArmABSMovZAttr)
                            End Select
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pArmABSMovPolarAttr.ToString)
                            .ParamList.Add(myMovAproxZ.ToString)
                        End With


                        'add to the queue list
                        If myListFwScript.Count > 0 Then
                            For i As Integer = 0 To myListFwScript.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                                End If
                            Next
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, False) ' XBC 06/02/2012 - Add previous movement to security fly position
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript0, True) ' XBC 06/02/2012 - Add previous movement to security fly position
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
                        End If
                End Select

                ManageArmsParkingStatus(pAdjustment, False)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Screen Exiting of Test operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 28/01/11</remarks>
        Private Function SendQueueForTEST_EXITING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                If GetArmsParkingStatus(pAdjustment) Then
                    Me.NoneInstructionToSend = False
                    Me.ParkDoneAttr = True
                    Exit Try
                End If

                ' Move to parking position the current arm
                'Script1
                With myFwScript1
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    Select Case pAdjustment
                        Case ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_DISP2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING2, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_RING3, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ZTUBE1, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_ISE, _
                             ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK
                            ' sample arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pSampleSecurityFlyAttr)
                            .ParamList.Add(Me.pSampleArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.REAGENT1_ARM_DISP1, _
                                                    ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, _
                                                    ADJUSTMENT_GROUPS.REAGENT1_ARM_ZREF, _
                                                    ADJUSTMENT_GROUPS.REAGENT1_ARM_RING1, _
                                                    ADJUSTMENT_GROUPS.REAGENT1_ARM_RING2, _
                                                    ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK
                            ' reagent1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent1SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent1ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.REAGENT2_ARM_DISP1, _
                                               ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, _
                                               ADJUSTMENT_GROUPS.REAGENT2_ARM_ZREF, _
                                               ADJUSTMENT_GROUPS.REAGENT2_ARM_RING1, _
                                               ADJUSTMENT_GROUPS.REAGENT2_ARM_RING2, _
                                               ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK
                            ' reagent2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pReagent2SecurityFlyAttr)
                            .ParamList.Add(Me.pReagent2ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                                                   ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                                                   ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                                                   ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer1SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer1ArmParkHAttr)
                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                                                ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                                                ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                                                ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ARM_ABS_UP.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.pMixer2SecurityFlyAttr)
                            .ParamList.Add(Me.pMixer2ArmParkHAttr)
                    End Select

                End With
                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                ManageArmsParkingStatus(pAdjustment, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForTEST_EXITING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for getting Absorbance Data for Optic Centering
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/03/11
        ''' </remarks>
        Protected Friend Function SendQueueForABSORBANCESCAN() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Rotor is auto filled and then ready to start BLDC test
                CurrentOperation = OPERATIONS.ABSORBANCESCAN

                ' Mov REL Rotor
                ' Relative positioning of the Reactions Rotor to next step
                With myFwScript1
                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_REL_ROTOR.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = myFwScript2
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = myFwScript2
                    .NextOnError = Nothing
                    ' expects 1 param
                    .ParamList = New List(Of String)
                    ' XBC 04/01/2012 - Add Encoder functionality
                    '.ParamList.Add("1") ' Fixed Rotor Step = 1
                    .ParamList.Add("-1") ' Fixed Rotor Step = -1 reading backwards
                    ' XBC 04/01/2012 - Add Encoder functionality
                End With

                ' Read Counts
                With myFwScript2
                    ' temporarily until Fw implement it
                    .FwScriptID = FwSCRIPTS_IDS.READ_COUNTS_ENCODER.ToString    ' READ_COUNTS   ' XBC 04/01/2012 - Add Encoder functionality
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    ' expects 2 params
                    .ParamList = New List(Of String)
                    .ParamList.Add(Me.WaveLengthAttr.ToString)
                    .ParamList.Add(Me.LEDIntensityAttr.ToString)
                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)

                If myFwScriptDelegate.CurrentFwScriptsQueue.Count = 0 Then
                    Debug.Print("Aturat !!!")
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForABSORBANCESCAN", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for preparing Optic Centering in well 1
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 23/09/11
        ''' AG 01/10/2014 - BA-1953 new photometry adjustment maneuver (use REAGENTS_ABS_ROTOR (with parameter = current value of GFWR1) instead of REACTIONS_ROTOR_HOME_WELL1)
        ''' </remarks>
        Protected Friend Function SendQueueForABSORBANCE_PREPARE() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Rotor is placed in well 1
                CurrentOperation = OPERATIONS.ABSORBANCE_PREPARE

                With myFwScript1
                    'AG 01/10/2014 BA-1953
                    '.FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString
                    '.ParamList = Nothing
                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString
                    .ParamList = New List(Of String)
                    .ParamList.Add(Me.pValueAdjustAttr)
                    'AG 01/10/2014 BA-1953

                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                End With

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                ' XBC 09/05/2012
                myFwScriptDelegate.INFOManagementEnabled = True

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForABSORBANCE_PREPARE", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the list of available Arm Positions to load into a specified Control
        ''' </summary>
        ''' <param name="pAdjustPoint">Type of Arm</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>Created by : XBC 03/01/2011</remarks>
        Private Function LoadPositionsValuesTO(ByVal pAdjustPoint As GlobalEnumerates.PreloadedMasterDataEnum) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                If (Not resultData.HasError) Then
                    'Get the list of Position Types as well as specified Arm
                    resultData = myPreloadedMasterDataDelegate.GetList(Nothing, pAdjustPoint)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.LoadPositionsValuesTO", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of available Arm Movements to load into a specified Control
        ''' </summary>
        ''' <param name="pAdjustPoint">Type of Arm</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset PreloadedMasterDataDS with the list of items in the
        '''          specified SubTable</returns>
        ''' <remarks>Created by : XBC 03/01/2011</remarks>
        Private Function LoadMovementsValuesTO(ByVal pAdjustPoint As GlobalEnumerates.PreloadedMasterDataEnum) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
                If (Not resultData.HasError) Then
                    'Get the list of Movement Types as well as especified Arm
                    resultData = myPreloadedMasterDataDelegate.GetList(Nothing, pAdjustPoint)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.LoadMovementsValuesTO", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Mixer testing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 04/01/11</remarks>
        Private Function SendQueueForMIXER_ON(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If



                ' Arm Absolute Pos-Down 
                ' Move absolute position the current arm:  predefined approaching position (steps)
                With myFwScript1
                    Select Case pAdjustment
                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 ON
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_ON.ToString

                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 ON
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_ON.ToString

                    End Select
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = Nothing
                End With

                'add to the queue list
                If myListFwScript.Count > 0 Then
                    For i As Integer = 0 To myListFwScript.Count - 1
                        If i = 0 Then
                            ' First Script
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                        End If
                    Next
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                Else
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForMIXER_ON", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for stop Mixer testing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 04/01/11</remarks>
        Private Function SendQueueForMIXER_OFF(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If



                ' Arm Absolute Pos-Down 
                ' Move absolute position the current arm:  predefined approaching position (steps)
                With myFwScript1
                    Select Case pAdjustment
                        Case ADJUSTMENT_GROUPS.MIXER1_ARM_DISP1, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_ZREF, _
                            ADJUSTMENT_GROUPS.MIXER1_ARM_PARK
                            ' mixer1 ON
                            .FwScriptID = FwSCRIPTS_IDS.MIXER1_OFF.ToString

                        Case ADJUSTMENT_GROUPS.MIXER2_ARM_DISP1, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_ZREF, _
                             ADJUSTMENT_GROUPS.MIXER2_ARM_PARK
                            ' mixer2 ON
                            .FwScriptID = FwSCRIPTS_IDS.MIXER2_OFF.ToString

                    End Select
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = Nothing
                End With

                'add to the queue list
                If myListFwScript.Count > 0 Then
                    For i As Integer = 0 To myListFwScript.Count - 1
                        If i = 0 Then
                            ' First Script
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                        End If
                    Next
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                Else
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForMIXER_OFF", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for a fine optical centering
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XB 15/10/2014 - Use FCK to fine optical centering - BA-2004
        ''' </remarks>
        Private Function SendQueueForFINE_OPTICAL_CENTERING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Arm Absolute Pos-Down 
                ' Move absolute position the current arm:  predefined approaching position (steps)
                With myFwScript1
                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_AUTO_CENTERING.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = Nothing
                End With

                'add to the queue list
                If myListFwScript.Count > 0 Then
                    For i As Integer = 0 To myListFwScript.Count - 1
                        If i = 0 Then
                            ' First Script
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
                        Else
                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
                        End If
                    Next
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                Else
                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForFINE_OPTICAL_CENTERING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#End Region

#Region "Reading Counts"
        ''' <summary> 
        ''' Routine called after reading counts obtained from the Instrument with a WaveLength by default
        ''' </summary>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>Created by : XBC 11/05/2011</remarks>
        Private Function ManageCountsMeasured() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myOpticCenterDataTO As OpticCenterDataTO
            Try
                myResultData = AnalyzerController.Instance.Analyzer.ReadOpticCenterData '#REFACTORING
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myOpticCenterDataTO = CType(myResultData.SetDatos, OpticCenterDataTO)
                    myOpticCenterDataTO.Steps = ReadedCountsAttr.Count + 1
                    ReadedCountsAttr.Add(myOpticCenterDataTO)
                Else
                    myResultData.HasError = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ManageCountsMeasured", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Invert List of Counts Readed because Fw needs to read it backwards
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 05/01/2012 - Add Encoder functionality</remarks>
        Private Function InvertCountsMeasured() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim AuxReadedCounts As New List(Of OpticCenterDataTO)

                For i As Integer = ReadedCountsAttr.Count - 1 To 0 Step -1
                    AuxReadedCounts.Add(ReadedCountsAttr(i))
                Next

                ReadedCountsAttr = AuxReadedCounts

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.InvertCountsMeasured", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region


#Region "History Data Report"

#Region "Public"

        ''' <summary>
        ''' Saves the History data to DB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function ManageHistoryResults() As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim myHistoryDelegate As New HistoricalReportsDelegate

            Try
                Dim myTask As String = ""
                Dim myAction As String = ""
                Dim myHistoricReportDS As New SRVResultsServiceDS
                Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                If MyClass.HistoryArea <> Nothing Then

                    myAction = MyClass.HistoryArea.ToString

                    Select Case MyClass.HistoryArea
                        Case HISTORY_AREAS.OPT, HISTORY_AREAS.WS_POS

                            Select Case MyClass.HistoryTask
                                Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES : myTask = "ADJUST"
                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES : myTask = "TEST"
                            End Select

                            'Fill the data
                            myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                            With myHistoricReportRow
                                .BeginEdit()
                                .TaskID = myTask
                                .ActionID = myAction
                                .Data = MyClass.GenerateResultData(MyClass.HistoryTask)
                                .AnalyzerID = MyClass.AnalyzerId
                                .EndEdit()
                            End With

                            'save to history
                            myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)

                        Case HISTORY_AREAS.MIX1_TEST, HISTORY_AREAS.MIX2_TEST

                            myTask = "TEST"
                            'Fill the data
                            myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                            With myHistoricReportRow
                                .BeginEdit()
                                .TaskID = myTask
                                .ActionID = myAction
                                .Data = MyClass.GenerateResultData(MyClass.HistoryTask)
                                .AnalyzerID = MyClass.AnalyzerId
                                .EndEdit()
                            End With

                            'save to history
                            myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)

                        Case Else

                            If MyClass.HisArmAdjResults.Count > 0 Then
                                myTask = "ADJUST"
                                'Fill the data
                                myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                                With myHistoricReportRow
                                    .BeginEdit()
                                    .TaskID = myTask
                                    .ActionID = myAction
                                    .Data = MyClass.GenerateResultData(PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES)
                                    .AnalyzerID = MyClass.AnalyzerId
                                    .EndEdit()
                                End With

                                'save to history
                                myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)

                            End If

                            If MyClass.HisArmTestResults.Count > 0 Then
                                myTask = "TEST"
                                'Fill the data
                                myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                                With myHistoricReportRow
                                    .BeginEdit()
                                    .TaskID = myTask
                                    .ActionID = myAction
                                    .Data = MyClass.GenerateResultData(PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES)
                                    .AnalyzerID = MyClass.AnalyzerId
                                    .EndEdit()
                                End With

                                'save to history
                                myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)

                            End If

                    End Select

                    If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                        'Get the generated ID from the dataset returned 
                        Dim generatedID As Integer = -1
                        generatedID = DirectCast(myResultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                        'PDT!!
                        ' pending implementation for add possibles Recommendations usings in case of INCIDENCE!!!
                        If generatedID >= 0 Then
                            ' Insert recommendations if existing
                            If MyClass.RecommendationsList IsNot Nothing Then
                                Dim myRecommendationsList As New SRVRecommendationsServiceDS
                                Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                                For Each R As HISTORY_RECOMMENDATIONS In MyClass.RecommendationsList
                                    myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                                    myRecommendationsRow.ResultServiceID = generatedID
                                    myRecommendationsRow.RecommendationID = CInt(R)
                                    myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                                Next

                                myResultData = myHistoryDelegate.AddRecommendations(Nothing, myRecommendationsList)
                                If myResultData.HasError Then
                                    myResultData.HasError = True
                                End If
                            End If
                        End If

                    End If

                Else
                    myResultData.HasError = True
                    myResultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ManageHistoryResults", EventLogEntryType.Error, False)
            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' Decodes the data from DB in order to display in History Screen
        ''' </summary>
        ''' <param name="pTask"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pLanguageId As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                'Dim myUtility As New Utilities()
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""

                Dim myLines As List(Of List(Of String))
                Dim FinalText As String = ""

                Dim myTask As PreloadedMasterDataEnum
                Dim myArea As HISTORY_AREAS


                'set the task type
                Select Case pTask
                    Case "ADJUST" : myTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                    Case "TEST" : myTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                    Case "UTIL" : myTask = PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                End Select

                'set the area
                Select Case pAction
                    Case "OPT" : myArea = HISTORY_AREAS.OPT
                    Case "WS_POS" : myArea = HISTORY_AREAS.WS_POS
                    Case "SAM_POS" : myArea = HISTORY_AREAS.SAM_POS
                    Case "REG1_POS" : myArea = HISTORY_AREAS.REG1_POS
                    Case "REG2_POS" : myArea = HISTORY_AREAS.REG2_POS
                    Case "MIX1_POS" : myArea = HISTORY_AREAS.MIX1_POS
                    Case "MIX2_POS" : myArea = HISTORY_AREAS.MIX2_POS
                    Case "MIX1_TEST" : myArea = HISTORY_AREAS.MIX1_TEST
                    Case "MIX2_TEST" : myArea = HISTORY_AREAS.MIX2_TEST
                End Select

                'obtain the connection
                myResultData = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myResultData.SetDatos, SqlClient.SqlConnection)
                End If

                Select Case myTask
                    Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES

                        Select Case myArea
                            Case HISTORY_AREAS.OPT

                                Dim myLine As List(Of String)
                                Dim myValue As String = ""
                                Dim myResult As HISTORY_RESULTS

                                Dim myColWidth As Integer = 25
                                Dim myColSep As Integer = 0

                                myLines = New List(Of List(Of String))

                                '1st LINE (title)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_OpticCenter", pLanguageId) + ": ") 'TEXT
                                myLine.Add(text1)
                                myLines.Add(myLine)

                                '2nd LINE (Led Intensity)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_LED_INTENSITY", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "LED_INT", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '3rd LINE (Wave Length)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WAVE_LENGTH", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "WAVE_LEN", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '4th LINE (Num Wells)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_NUM_WELLS", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "NUM_WELL", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                '5th LINE (Well Step)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WELL_STEP", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "WELL_STEP", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)


                                '6th LINE (Rotor Adjusted Value)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ROTOR_POS_VALUE", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "ROTOR_VAL", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)

                                ' XBC 05/01/2012 - Add Encoder functionality
                                '7th LINE (Encoder Adjusted Value)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Encoder", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "ENCODER_VAL", 2)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)
                                ' XBC 05/01/2012 - Add Encoder functionality

                                '8th LINE (Optic Centering Tested)
                                '*****************************************************************
                                myLine = New List(Of String)
                                'text1 = MLRD.GetResourceText(dbConnection, "LBL_SRV_OpticCenter", pLanguageId) + ":"
                                'myLine.Add(text1)
                                myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask)
                                'text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId)) + ":"
                                'myLine.Add(text1)

                                text1 = MyClass.GetResultLanguageResource(myTask, dbConnection, myResult, pLanguageId) '.PadLeft(4)
                                myLine.Add(text1)

                                myLines.Add(myLine)

                                'Final
                                '**************
                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    Select Case myLines.IndexOf(Line) + 1
                                        Case 1 : newLine = True 'input
                                        Case 2 : newLine = False 'title & tested

                                    End Select
                                    FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText




                            Case HISTORY_AREAS.WS_POS

                                Dim myLine As List(Of String)
                                Dim myValue As String = ""
                                Dim myResult As HISTORY_RESULTS

                                Dim myColWidth As Integer = 25
                                Dim myColSep As Integer = 0

                                myLines = New List(Of List(Of String))

                                '1st LINE (title)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WashStation", pLanguageId) + ": ") 'TEXT
                                myLine.Add(text1)
                                myLines.Add(myLine)


                                '2nd LINE (WS Adjusted Value)
                                '*****************************************************************
                                myLine = New List(Of String)

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WS_POS_VALUE", pLanguageId) + ":") 'TEXT
                                myLine.Add(text1)

                                myValue = MyClass.DecodeHistoryDataValue(pData, myArea, myTask, "WS_VAL", 5)
                                text2 = myValue.ToString '.PadLeft(4)
                                myLine.Add(text2)

                                myLines.Add(myLine)


                                '7th LINE (Washing Station Adjusted)
                                '*****************************************************************
                                myLine = New List(Of String)

                                myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask)
                                'text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ADJUSTED", pLanguageId)) + ":"
                                'myLine.Add(text1)

                                text1 = MyClass.GetResultLanguageResource(myTask, dbConnection, myResult, pLanguageId) '.PadLeft(4)
                                myLine.Add(text1)

                                myLines.Add(myLine)

                                'Final
                                '**************
                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    Select Case myLines.IndexOf(Line) + 1
                                        Case 1 : newLine = True 'input
                                        Case 2 : newLine = False

                                    End Select
                                    FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText




                            Case HISTORY_AREAS.SAM_POS, _
                                HISTORY_AREAS.REG1_POS, _
                                HISTORY_AREAS.REG2_POS, _
                                HISTORY_AREAS.MIX1_POS, _
                                HISTORY_AREAS.MIX2_POS

                                Dim myColWidth As Integer = 35
                                Dim myColSep As Integer = 0

                                myLines = New List(Of List(Of String))
                                Dim myLine As List(Of String)

                                '1st LINE (Arm Title:Name)
                                '*****************************************************************
                                Dim myArmTitle As String = ""
                                Dim myArmName As String = ""

                                myLine = New List(Of String)

                                myArmTitle = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ARM", pLanguageId) + ":")
                                'myLine.Add(myArmTitle)

                                myArmName = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId)
                                myLine.Add(myArmTitle + myArmName)

                                myLines.Add(myLine)

                                '2nd LINE
                                '****************************************************************
                                myLine = New List(Of String)

                                Dim myPositionsValues As New List(Of List(Of String))

                                Select Case myArea
                                    Case HISTORY_AREAS.SAM_POS
                                        Dim mySAMPLEPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Predilution)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring1)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring2)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring3)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Z_Tube)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.ISE_Pos)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In mySAMPLEPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next

                                    Case HISTORY_AREAS.REG1_POS, HISTORY_AREAS.REG2_POS
                                        Dim myREAGENTPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Ring1)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Ring2)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In myREAGENTPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next

                                    Case HISTORY_AREAS.MIX1_POS, HISTORY_AREAS.MIX2_POS
                                        Dim myMIXERPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In myMIXERPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next


                                End Select

                                Dim myLeftColumn As New List(Of List(Of String))
                                Dim myRightColumn As New List(Of List(Of String))
                                For Each PP As List(Of String) In myPositionsValues
                                    If myPositionsValues.IndexOf(PP) Mod 2 = 0 Then
                                        myLeftColumn.Add(PP)
                                    Else
                                        myRightColumn.Add(PP)
                                    End If
                                Next

                                For L As Integer = 0 To myLeftColumn.Count - 1 Step 1

                                    'position name and result
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(0) + " " + myLeftColumn(L)(1))
                                    myLine.Add(myRightColumn(L)(0) + " " + myRightColumn(L)(1))
                                    myLines.Add(myLine)

                                    'polar value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(2))
                                    myLine.Add(myRightColumn(L)(2))
                                    myLines.Add(myLine)

                                    'z value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(3))
                                    myLine.Add(myRightColumn(L)(3))
                                    myLines.Add(myLine)

                                    'rotor value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(4))
                                    myLine.Add(myRightColumn(L)(4))
                                    myLines.Add(myLine)

                                Next

                                'Final
                                '**************
                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    'If (Line.Count = 1 AndAlso Line(0).Length > 0) Or (Line.Count = 2 AndAlso (Line(0).Length > 0 Or Line(1).Length > 0)) Then
                                    If myLines.IndexOf(Line) = 0 Then
                                        newLine = True
                                    ElseIf myLines.IndexOf(Line) Mod 4 = 0 Then
                                        newLine = True
                                    Else
                                        newLine = False
                                    End If
                                    FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                    'End If
                                Next

                                myResultData.SetDatos = FinalText

                                'Formato anterior

                                '   Case HISTORY_AREAS.SAM_POS, _
                                'HISTORY_AREAS.REG1_POS, _
                                'HISTORY_AREAS.REG2_POS, _
                                'HISTORY_AREAS.MIX1_POS, _
                                'HISTORY_AREAS.MIX2_POS

                                '       Dim myLine As List(Of String)
                                '       Dim myValue As String = ""
                                '       Dim myResult As HISTORY_RESULTS

                                '       Dim myColWidth As Integer = 25
                                '       Dim myColSep As Integer = 2

                                '       myLines = New List(Of List(Of String))


                                '       '1st LINE (Arm Name)
                                '       '*****************************************************************
                                '       myLine = New List(Of String)

                                '       text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ARM", pLanguageId) + ":")
                                '       myLine.Add(text1)

                                '       text2 = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId) + ":"
                                '       myLine.Add(text2)

                                '       myLines.Add(myLine)


                                '       'DISPENSATION
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Dispensation, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Dispensation)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Dispensation, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'PREDILUTION
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Predilution, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Predilution)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Predilution, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'Z REFERENCE
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Z_Ref, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Z_Ref)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Z_Ref, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'WASHING
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Washing, dbConnection, pLanguageId)
                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Washing)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Washing, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If


                                '       'RING 1
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring1, dbConnection, pLanguageId)
                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring1)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring1, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'RING 2
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring2, dbConnection, pLanguageId)
                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring2)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring2, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'RING 3
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring3, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring3)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring3, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If


                                '       'Z TUBE
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Z_Tube, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Z_Tube)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Z_Tube, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If


                                '       'ISE POSITION
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.ISE_Pos, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.ISE_Pos)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.ISE_Pos, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If


                                '       'PARKING
                                '       '*****************************************************************
                                '       myLine = New List(Of String)
                                '       text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Parking, dbConnection, pLanguageId)

                                '       If text1.Length > 0 Then
                                '           myLine.Add(text1)

                                '           myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Parking)
                                '           If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '               text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Adjusted", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '               myLine.Add(text2)

                                '               myLines.Add(myLine)

                                '               'Values
                                '               myLine = New List(Of String)
                                '               myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Parking, dbConnection, pLanguageId)
                                '               myLines.Add(myLine)
                                '           End If
                                '       End If

                                '       'Final
                                '       '**************
                                '       For Each Line As List(Of String) In myLines
                                '           Dim newLine As Boolean = (myLines.IndexOf(Line) Mod 2 = 0)
                                '           FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                '       Next

                                '       myResultData.SetDatos = FinalText

                        End Select


                    Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES

                        Select Case myArea

                            ' XBC 24/11/2011
                            Case HISTORY_AREAS.MIX1_TEST, _
                                 HISTORY_AREAS.MIX2_TEST

                                Dim myResult As HISTORY_RESULTS
                                Dim myColWidth As Integer = 35
                                Dim myColSep As Integer = 0

                                myLines = New List(Of List(Of String))
                                Dim myLine As List(Of String)

                                '1st LINE (Arm position Name)
                                '*****************************************************************
                                Dim myArmTitle As String = ""
                                Dim myArmName As String = ""

                                myLine = New List(Of String)

                                myArmTitle = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ARM", pLanguageId) + ":")

                                myArmName = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId)
                                myLine.Add(myArmTitle + myArmName)

                                myLines.Add(myLine)

                                '2nd LINE
                                '****************************************************************
                                myLine = New List(Of String)

                                Dim myResultNumber As String = ""
                                myResultNumber = pData.Substring(1, 1)
                                If IsNumeric(myResultNumber) Then
                                    myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                                ElseIf myResultNumber = "x" Then
                                    myResult = CType(-1, HISTORY_RESULTS)
                                End If

                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId))
                                myLine.Add(text1)

                                myLines.Add(myLine)


                                'Final
                                '**************
                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    Select Case myLines.IndexOf(Line) + 1
                                        Case 1 : newLine = True 'input
                                        Case 2 : newLine = True

                                    End Select
                                    FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText

                                ' XBC 24/11/2011

                            Case HISTORY_AREAS.SAM_POS, _
                               HISTORY_AREAS.REG1_POS, _
                               HISTORY_AREAS.REG2_POS, _
                               HISTORY_AREAS.MIX1_POS, _
                               HISTORY_AREAS.MIX2_POS

                                Dim myColWidth As Integer = 35
                                Dim myColSep As Integer = 0

                                myLines = New List(Of List(Of String))
                                Dim myLine As List(Of String)

                                '1st LINE (Arm Title:Name)
                                '*****************************************************************
                                Dim myArmTitle As String = ""
                                Dim myArmName As String = ""

                                myLine = New List(Of String)

                                myArmTitle = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ARM", pLanguageId) + ":")
                                'myLine.Add(myArmTitle)

                                myArmName = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId)
                                myLine.Add(myArmTitle + myArmName)

                                myLines.Add(myLine)

                                '2nd LINE
                                '****************************************************************
                                myLine = New List(Of String)

                                Dim myPositionsValues As New List(Of List(Of String))

                                Select Case myArea
                                    Case HISTORY_AREAS.SAM_POS
                                        Dim mySAMPLEPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Predilution)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring1)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring2)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Ring3)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Z_Tube)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.ISE_Pos)
                                        mySAMPLEPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In mySAMPLEPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next

                                    Case HISTORY_AREAS.REG1_POS, HISTORY_AREAS.REG2_POS
                                        Dim myREAGENTPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Ring1)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Ring2)
                                        myREAGENTPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In myREAGENTPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next

                                    Case HISTORY_AREAS.MIX1_POS, HISTORY_AREAS.MIX2_POS
                                        Dim myMIXERPointList As New List(Of HISTORY_ARM_POSITIONS)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Dispensation)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Z_Ref)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Washing)
                                        myMIXERPointList.Add(HISTORY_ARM_POSITIONS.Parking)

                                        For Each P As HISTORY_ARM_POSITIONS In myMIXERPointList
                                            Dim myPosValues As List(Of String) = MyClass.DecodeHistoryPositionValues(pData, myTask, myArea, P, dbConnection, pLanguageId)
                                            myPositionsValues.Add(myPosValues)
                                        Next


                                End Select

                                Dim myLeftColumn As New List(Of List(Of String))
                                Dim myRightColumn As New List(Of List(Of String))
                                For Each PP As List(Of String) In myPositionsValues
                                    If myPositionsValues.IndexOf(PP) Mod 2 = 0 Then
                                        myLeftColumn.Add(PP)
                                    Else
                                        myRightColumn.Add(PP)
                                    End If
                                Next

                                For L As Integer = 0 To myLeftColumn.Count - 1 Step 1

                                    'position name and result
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(0) + " " + myLeftColumn(L)(1))
                                    myLine.Add(myRightColumn(L)(0) + " " + myRightColumn(L)(1))
                                    myLines.Add(myLine)

                                    'polar value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(2))
                                    myLine.Add(myRightColumn(L)(2))
                                    myLines.Add(myLine)

                                    'z value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(3))
                                    myLine.Add(myRightColumn(L)(3))
                                    myLines.Add(myLine)

                                    'rotor value
                                    myLine = New List(Of String)
                                    myLine.Add(myLeftColumn(L)(4))
                                    myLine.Add(myRightColumn(L)(4))
                                    myLines.Add(myLine)

                                Next

                                'Final
                                '**************
                                For Each Line As List(Of String) In myLines
                                    Dim newLine As Boolean = False
                                    If myLines.IndexOf(Line) = 0 Then
                                        newLine = True
                                    ElseIf myLines.IndexOf(Line) Mod 4 = 0 Then
                                        newLine = True
                                    Else
                                        newLine = False
                                    End If
                                    FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                Next

                                myResultData.SetDatos = FinalText

                                'Formato anterior

                                '       Case HISTORY_AREAS.SAM_POS, _
                                'HISTORY_AREAS.REG1_POS, _
                                'HISTORY_AREAS.REG2_POS, _
                                'HISTORY_AREAS.MIX1_POS, _
                                'HISTORY_AREAS.MIX2_POS

                                '           Dim myLine As List(Of String)
                                '           Dim myValue As String = ""
                                '           Dim myResult As HISTORY_RESULTS

                                '           Dim myColWidth As Integer = 25
                                '           Dim myColSep As Integer = 0

                                '           myLines = New List(Of List(Of String))


                                '           '1st LINE (Arm Name)
                                '           '*****************************************************************
                                '           myLine = New List(Of String)

                                '           text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ARM", pLanguageId) + ":")
                                '           myLine.Add(text1)

                                '           text2 = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId)
                                '           myLine.Add(text2)

                                '           myLines.Add(myLine)


                                '           'DISPENSATION
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Dispensation, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Dispensation)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Dispensation, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'PREDILUTION
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Predilution, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Predilution)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Predilution, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'Z REFERENCE
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Z_Ref, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Z_Ref)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Z_Ref, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'WASHING
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Washing, dbConnection, pLanguageId)
                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Washing)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Washing, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If


                                '           'RING 1
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring1, dbConnection, pLanguageId)
                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring1)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring1, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'RING 2
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring2, dbConnection, pLanguageId)
                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring2)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring2, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'RING 3
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Ring3, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Ring3)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Ring3, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If


                                '           'Z TUBE
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Z_Tube, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Z_Tube)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Z_Tube, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If


                                '           'ISE POSITION
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.ISE_Pos, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.ISE_Pos)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.ISE_Pos, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If


                                '           'PARKING
                                '           '*****************************************************************
                                '           myLine = New List(Of String)
                                '           text1 = MyClass.DecodeHistoryPosName(pData, myArea, HISTORY_ARM_POSITIONS.Parking, dbConnection, pLanguageId)

                                '           If text1.Length > 0 Then
                                '               myLine.Add(text1)

                                '               myResult = MyClass.DecodeHistoryDataResult(pData, myArea, myTask, HISTORY_ARM_POSITIONS.Parking)
                                '               If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                                '                   text2 = MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": " + MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                                '                   myLine.Add(text2)

                                '                   myLines.Add(myLine)

                                '                   'Values
                                '                   myLine = New List(Of String)
                                '                   myLine = MyClass.DecodeHistoryArmValues(pData, HISTORY_ARM_POSITIONS.Parking, dbConnection, pLanguageId)
                                '                   myLines.Add(myLine)
                                '               End If
                                '           End If

                                '           'Final
                                '           '**************
                                '           For Each Line As List(Of String) In myLines
                                '               Dim newLine As Boolean = (myLines.IndexOf(Line) Mod 2 = 0)
                                '               FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                '           Next

                                '           myResultData.SetDatos = FinalText



                                '       Case HISTORY_AREAS.MIX1_TEST, HISTORY_AREAS.MIX2_TEST

                                '           Dim myLine As List(Of String)
                                '           Dim myValue As String = ""
                                '           Dim myResult As HISTORY_RESULTS

                                '           Dim myColWidth As Integer = 50
                                '           Dim myColSep As Integer = 2

                                '           myLines = New List(Of List(Of String))

                                '           '1st LINE (Arm position Name)
                                '           '*****************************************************************
                                '           myLine = New List(Of String)

                                '           text1 = MyClass.DecodeHistoryArmName(pData, myArea, dbConnection, pLanguageId) + ":"
                                '           myLine.Add(text1)

                                '           myLines.Add(myLine)

                                '           '2nd LINE (Tested)
                                '           '*****************************************************************
                                '           myLine = New List(Of String)

                                '           Dim myResultNumber As String = ""
                                '           myResultNumber = pData.Substring(1, 1)
                                '           If IsNumeric(myResultNumber) Then
                                '               myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                                '           ElseIf myResultNumber = "x" Then
                                '               myResult = CType(-1, HISTORY_RESULTS)
                                '           End If

                                '           text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId)) + ":"
                                '           myLine.Add(text1)

                                '           text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId) '.PadLeft(4)
                                '           myLine.Add(text2)

                                '           myLines.Add(myLine)

                                '           'Final
                                '           '**************
                                '           For Each Line As List(Of String) In myLines
                                '               Dim newLine As Boolean = False
                                '               Select Case myLines.IndexOf(Line) + 1
                                '                   Case 1 : newLine = True 'input
                                '                   Case 2 : newLine = False

                                '               End Select
                                '               FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, newLine)
                                '           Next

                                '           myResultData.SetDatos = FinalText

                        End Select

                    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                        '    'not applied


                End Select



            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection IsNot Nothing Then dbConnection.Close()
            End Try

            Return myResultData

        End Function

        Public Function DecodeHistoryPositionValues(ByVal pData As String, ByVal pTask As PreloadedMasterDataEnum, ByVal pArea As HISTORY_AREAS, _
                                                    ByVal pPosition As HISTORY_ARM_POSITIONS, _
                                                    Optional ByVal pConnection As SqlClient.SqlConnection = Nothing, _
                                                    Optional ByVal pLanguageId As String = "ENG") As List(Of String)

            Dim myPointName As String = ""
            Dim myResultText As String = ""
            Dim myPText As String = ""
            Dim myZText As String = ""
            Dim myRText As String = ""

            Dim PositionValues As New List(Of String)

            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                Dim pos As String = ""
                pos = MyClass.DecodeHistoryPosName(pData, pArea, pPosition, pConnection, pLanguageId) + ":"
                If pos.Length > 0 Then
                    myPointName = pos
                    Dim myResult As HISTORY_RESULTS = HISTORY_RESULTS.NOT_APPLIED
                    Dim res As String = ""
                    myResult = MyClass.DecodeHistoryDataResult(pData, pArea, pTask, pPosition)
                    If myResult <> HISTORY_RESULTS.NOT_APPLIED Then
                        myResultText = MyClass.GetResultLanguageResource(pTask, pConnection, myResult, pLanguageId)

                        Dim myValueNumber As String = ""
                        Dim myIndex As Integer = -1
                        Dim myValue As String = ""

                        ' XBC 23/11/2011
                        'Select Case pPosition
                        'Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 0 
                        'Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 16
                        'Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 32
                        'Case HISTORY_ARM_POSITIONS.Washing : myIndex = 48
                        'Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 64
                        'Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 80
                        'Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 96
                        'Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 112
                        'Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 128
                        'Case HISTORY_ARM_POSITIONS.Parking : myIndex = 144
                        'End Select

                        Select Case pArea
                            Case HISTORY_AREAS.SAM_POS

                                Select Case pPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 1
                                    Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 17
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 33
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 49
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 65
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 81
                                    Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 97
                                    Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 113
                                    Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 129
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 145
                                End Select

                            Case HISTORY_AREAS.REG1_POS, _
                                 HISTORY_AREAS.REG2_POS

                                Select Case pPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 1
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 17
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 33
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 49
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 65
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 81
                                End Select

                            Case HISTORY_AREAS.MIX1_POS, _
                                 HISTORY_AREAS.MIX2_POS

                                Select Case pPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 1
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 17
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 33
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 49
                                End Select

                        End Select
                        ' XBC 23/11/2011

                        '(Polar value)
                        myValue = ""

                        ' XBC 23/11/2011
                        'myValueNumber = pData.Substring(myIndex, 4)
                        myValueNumber = pData.Substring(myIndex, 5)
                        ' XBC 23/11/2011

                        If IsNumeric(myValueNumber) Then
                            myValue = CInt(myValueNumber).ToString
                            myPText = (MLRD.GetResourceText(pConnection, "GRID_SRV_POLAR", pLanguageId) + ": ").PadRight(10) + myValue.ToString.PadRight(10)
                        Else
                            myPText = (MLRD.GetResourceText(pConnection, "GRID_SRV_POLAR", pLanguageId) + ": ").PadRight(10) + "----"
                        End If


                        '(Z value)
                        myValue = ""

                        ' XBC 23/11/2011
                        'myValueNumber = pData.Substring(myIndex + 4, 4)
                        myValueNumber = pData.Substring(myIndex + 5, 5)
                        ' XBC 23/11/2011

                        If IsNumeric(myValueNumber) Then
                            myValue = CInt(myValueNumber).ToString
                            myZText = (MLRD.GetResourceText(pConnection, "GRID_SRV_Z", pLanguageId) + ": ").PadRight(10) + myValue.ToString.PadRight(10)
                        Else
                            myZText = (MLRD.GetResourceText(pConnection, "GRID_SRV_Z", pLanguageId) + ": ").PadRight(10) + "----"
                        End If



                        '(Rotor value)
                        myValue = ""

                        ' XBC 23/11/2011
                        'myValueNumber = pData.Substring(myIndex + 8, 4)
                        myValueNumber = pData.Substring(myIndex + 10, 5)
                        ' XBC 23/11/2011

                        If IsNumeric(myValueNumber) Then
                            myValue = CInt(myValueNumber).ToString
                            myRText = (MLRD.GetResourceText(pConnection, "GRID_SRV_ROTOR", pLanguageId) + ": ").PadRight(10) + myValue.ToString.PadRight(10)
                        Else
                            myRText = (MLRD.GetResourceText(pConnection, "GRID_SRV_ROTOR", pLanguageId) + ": ").PadRight(10) + "----"
                        End If


                    Else
                        myResultText = MyClass.GetResultLanguageResource(pTask, pConnection, HISTORY_RESULTS.NOT_DONE, pLanguageId)
                        myPText = (MLRD.GetResourceText(pConnection, "GRID_SRV_POLAR", pLanguageId) + ": ").PadRight(10) + "----"
                        myZText = (MLRD.GetResourceText(pConnection, "GRID_SRV_Z", pLanguageId) + ": ").PadRight(10) + "----"
                        myRText = (MLRD.GetResourceText(pConnection, "GRID_SRV_ROTOR", pLanguageId) + ": ").PadRight(10) + "----"
                    End If

                End If

                PositionValues.Add(myPointName)
                PositionValues.Add(myResultText)
                PositionValues.Add(myPText)
                PositionValues.Add(myRText)
                PositionValues.Add(myZText)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryPositionValues", EventLogEntryType.Error, False)
            End Try

            Return PositionValues

        End Function

        ''' <summary>
        ''' Save a record when User no complete positions adjustments
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 01/12/2011</remarks>
        Public Function ReportExitWithOutFinishAdjustment() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
                'Get the data of the Analyzers which have already registered activities
                myResultData = myHistoricalReportsDelegate.GetAllResultsService(Nothing, AnalyzerId, "ADJUST", "OPT", Nothing, Nothing)

                If (Not myResultData.HasError And Not myResultData.SetDatos Is Nothing) Then
                    Dim myResultsDS As SRVResultsServiceDecodedDS
                    myResultsDS = DirectCast(myResultData.SetDatos, SRVResultsServiceDecodedDS)

                    Dim qHistoricsIDs As New List(Of SRVResultsServiceDecodedDS.srv_thrsResultsServiceRow)
                    qHistoricsIDs = (From a In myResultsDS.srv_thrsResultsService _
                                     Order By a.ResultServiceID Descending _
                                     Select a).ToList()

                    If qHistoricsIDs.Count > 0 Then
                        ' Insert recommendation
                        Dim myHistoryDelegate As New HistoricalReportsDelegate
                        Dim myRecommendationsList As New SRVRecommendationsServiceDS
                        Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                        myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                        myRecommendationsRow.ResultServiceID = qHistoricsIDs(0).ResultServiceID
                        myRecommendationsRow.RecommendationID = HISTORY_RECOMMENDATIONS.ADJ_CANCELLED_BY_USER
                        myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)

                        myResultData = myHistoryDelegate.AddRecommendations(Nothing, myRecommendationsList)
                        If myResultData.HasError Then
                            myResultData.HasError = True
                        End If
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.ReportExitWithOutFinishAdjustment", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#End Region

#Region "Private"

        ''' <summary>
        ''' Reports the History data to DB. Encodes the result data to history
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GenerateResultData(ByVal pTask As PreloadedMasterDataEnum) As String

            Dim myData As String = ""

            Try

                If MyClass.HistoryArea <> Nothing Then

                    Select Case MyClass.HistoryArea

                        'OPTIC CENTERING
                        Case HISTORY_AREAS.OPT
                            Select Case pTask
                                Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                    myData = MyClass.EncodeHistoryValue(MyClass.HisLEDIntensity, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisWaveLength, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisNumWells, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisStepsbyWell, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisOpticCenteringValue, 5)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisEncoderValue, 2)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisOpticCenteringAdjResult))

                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                    myData = MyClass.EncodeHistoryValue(MyClass.HisLEDIntensity, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisWaveLength, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisNumWells, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisStepsbyWell, 5)
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisOpticCenteringValue, 5)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    myData &= MyClass.EncodeHistoryValue(MyClass.HisEncoderValue, 2)
                                    ' XBC 05/01/2012 - Add Encoder functionality
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisOpticCenteringTestResult))

                            End Select

                            'WASHING STATION
                        Case HISTORY_AREAS.WS_POS
                            Select Case pTask
                                Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                    myData = MyClass.EncodeHistoryValue(MyClass.HisWashingStationValue, 5)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisWashingStationAdjResult))

                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                    myData = MyClass.EncodeHistoryValue(MyClass.HisWashingStationValue, 5)
                                    myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisWashingStationTestResult))

                            End Select


                        Case HISTORY_AREAS.SAM_POS, _
                                HISTORY_AREAS.REG1_POS, _
                                HISTORY_AREAS.REG2_POS, _
                                HISTORY_AREAS.MIX1_POS, _
                                HISTORY_AREAS.MIX2_POS

                            Select Case pTask
                                Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES

                                    myData &= MyClass.EncodeHistoryArmName(MyClass.HistoryArea)

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Dispensation) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Dispensation, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Dispensation)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Predilution) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Predilution, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Predilution)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Z_Ref) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Z_Ref, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Z_Ref)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Washing) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Washing, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Washing)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring1) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring1, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Ring1)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring2) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring2, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Ring2)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring3) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring3, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Ring3)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Z_Tube) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Z_Tube, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Z_Tube)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.ISE_Pos) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.ISE_Pos, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.ISE_Pos)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Parking) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Parking, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmAdjResults(HISTORY_ARM_POSITIONS.Parking)))
                                    End If




                                Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES

                                    myData &= MyClass.EncodeHistoryArmName(MyClass.HistoryArea)

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Dispensation) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Dispensation, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Dispensation)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Predilution) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Predilution, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Predilution)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Z_Ref) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Z_Ref, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Z_Ref)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Washing) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Washing, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Washing)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring1) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring1, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Ring1)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring2) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring2, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Ring2)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Ring3) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Ring3, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Ring3)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Z_Tube) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Z_Tube, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Z_Tube)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.ISE_Pos) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.ISE_Pos, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.ISE_Pos)))
                                    End If

                                    ' XBC 23/11/2011
                                    If MyClass.HisArmAxesValues.ContainsKey(HISTORY_ARM_POSITIONS.Parking) Then
                                        myData &= MyClass.EncodeHistoryArmAxesValues(HISTORY_ARM_POSITIONS.Parking, MyClass.HisArmAxesValues, 15)
                                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisArmTestResults(HISTORY_ARM_POSITIONS.Parking)))
                                    End If

                            End Select

                        Case HISTORY_AREAS.MIX1_TEST, HISTORY_AREAS.MIX2_TEST

                            myData &= MyClass.EncodeHistoryArmName(MyClass.HistoryArea)
                            myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisStirrerTestResult))

                    End Select

                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.GenerateResultData", EventLogEntryType.Error, False)
            End Try

            Return myData

        End Function

        ''' <summary>
        ''' Encodes History Result
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryResult(ByVal pValue As Integer) As String

            Dim res As String = ""

            Try
                If pValue >= 0 Then
                    res = pValue.ToString
                Else
                    res = "x"
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.EncodeHistoryResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Value
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryValue(ByVal pValue As Integer, Optional ByVal pLength As Integer = 5) As String

            Dim res As String = "xxxx"

            Try
                If pValue >= -99999 Then
                    Dim myFormat As String = ""
                    If pValue < 0 Then pLength = pLength - 1
                    For i As Integer = 1 To pLength
                        myFormat &= "0"
                    Next
                    If myFormat.Length > 0 Then
                        res = pValue.ToString(myFormat)
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.EncodeHistoryValue", EventLogEntryType.Error, False)
                res = "xxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Arm Axes Values
        ''' </summary>
        ''' <param name="pPosition"></param>
        ''' <param name="pValues"></param>
        ''' <param name="pLength"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function EncodeHistoryArmAxesValues(ByVal pPosition As HISTORY_ARM_POSITIONS, _
                                                    ByVal pValues As Dictionary(Of HISTORY_ARM_POSITIONS, List(Of Integer)), _
                                                    Optional ByVal pLength As Integer = 12) As String

            Dim res As String = "xxxxxxxxxxxxxxx"

            Try

                Dim myPolar As String = "xxxxx"
                Dim myZ As String = "xxxxx"
                Dim myRotor As String = "xxxxx"

                If pPosition <> HISTORY_ARM_POSITIONS.None And pValues IsNot Nothing Then

                    ' XBC 23/11/2011
                    'Dim myFormat As String = ""
                    'For i As Integer = 1 To 5
                    '    myFormat &= "0"
                    'Next
                    Dim myFormatPolar As String = ""
                    Dim myFormatZ As String = ""
                    Dim myFormatRotor As String = ""
                    For i As Integer = 1 To 5
                        myFormatPolar &= "0"
                        myFormatZ &= "0"
                        myFormatRotor &= "0"
                    Next

                    'If myFormat.Length > 0 Then
                    ' XBC 23/11/2011

                    Dim myValue As Integer

                    myValue = pValues(pPosition).Item(0)
                    If myValue >= -99999 Then
                        ' XBC 23/11/2011
                        'If myValue < 0 Then myFormat = myFormat.Substring(1)
                        'myPolar = myValue.ToString(myFormat)
                        If myValue < 0 Then myFormatPolar = myFormatPolar.Substring(1)
                        myPolar = myValue.ToString(myFormatPolar)
                        ' XBC 23/11/2011
                    End If

                    myValue = pValues(pPosition).Item(1)
                    If myValue >= -99999 Then
                        ' XBC 23/11/2011
                        'If myValue < 0 Then myFormat = myFormat.Substring(1)
                        'myZ = myValue.ToString(myFormat)
                        If myValue < 0 Then myFormatZ = myFormatZ.Substring(1)
                        myZ = myValue.ToString(myFormatZ)
                        ' XBC 23/11/2011
                    End If

                    myValue = pValues(pPosition).Item(2)
                    If myValue >= -99999 Then
                        ' XBC 23/11/2011
                        'If myValue < 0 Then myFormat = myFormat.Substring(1)
                        'myRotor = myValue.ToString(myFormat)
                        If myValue < 0 Then myFormatRotor = myFormatRotor.Substring(1)
                        myRotor = myValue.ToString(myFormatRotor)
                        ' XBC 23/11/2011
                    End If
                End If

                ' XBC 23/11/2011
                'End If
                ' XBC 23/11/2011

                res = myPolar & myZ & myRotor

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.EncodeHistoryArmAxesValues", EventLogEntryType.Error, False)
                res = "xxxxxxxxxxxxxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Result
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pArea"></param>
        ''' <param name="pTaskType"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataResult(ByVal pData As String, _
                                                 ByVal pArea As HISTORY_AREAS, _
                                                 ByVal pTaskType As PreloadedMasterDataEnum, _
                                                 Optional ByVal pArmPosition As HISTORY_ARM_POSITIONS = HISTORY_ARM_POSITIONS.None) As HISTORY_RESULTS

            Dim myResult As HISTORY_RESULTS

            Try
                Dim myIndex As Integer = -1

                Select Case pArea
                    Case HISTORY_AREAS.OPT
                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                ' XBC 05/01/2012 - Add Encoder functionality
                                myIndex = 27    ' 25

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                myIndex = 25

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select

                    Case HISTORY_AREAS.WS_POS
                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                myIndex = 5


                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                myIndex = 5


                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select


                        ' XBC 23/11/2011
                        'Case HISTORY_AREAS.SAM_POS, _
                        '    HISTORY_AREAS.REG1_POS, _
                        '    HISTORY_AREAS.REG2_POS, _
                        '    HISTORY_AREAS.MIX1_POS, _
                        '    HISTORY_AREAS.MIX2_POS

                        'Select Case pTaskType
                        '    Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                        '        Select Case pArmPosition
                        '            Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 15
                        '            Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 31
                        '            Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 47
                        '            Case HISTORY_ARM_POSITIONS.Washing : myIndex = 63
                        '            Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 79
                        '            Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 95
                        '            Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 111
                        '            Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 127
                        '            Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 143
                        '            Case HISTORY_ARM_POSITIONS.Parking : myIndex = 159

                        '        End Select

                        '    Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                        '        Select Case pArmPosition
                        '            Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 15
                        '            Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 31
                        '            Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 47
                        '            Case HISTORY_ARM_POSITIONS.Washing : myIndex = 63
                        '            Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 79
                        '            Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 95
                        '            Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 111
                        '            Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 127
                        '            Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 143
                        '            Case HISTORY_ARM_POSITIONS.Parking : myIndex = 159

                        '        End Select

                        '    Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                        '        not applied
                        'End Select


                    Case HISTORY_AREAS.SAM_POS
                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 62
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 80
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 96
                                    Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 112
                                    Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 128
                                    Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 144
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 160

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 64
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 80
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 96
                                    Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 112
                                    Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 128
                                    Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 144
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 160

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select


                    Case HISTORY_AREAS.REG1_POS, _
                         HISTORY_AREAS.REG2_POS

                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 64
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 80
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 96

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 64
                                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 80
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 96

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select

                    Case HISTORY_AREAS.MIX1_POS, _
                         HISTORY_AREAS.MIX2_POS

                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 64

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                Select Case pArmPosition
                                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 16
                                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 32
                                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 48
                                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 64

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select
                        ' XBC 23/11/2011

                End Select


                If myIndex >= 0 Then
                    Dim myResultNumber As String = ""
                    myResultNumber = pData.Substring(myIndex, 1)
                    If IsNumeric(myResultNumber) Then
                        myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                    ElseIf myResultNumber = "x" Then
                        myResult = HISTORY_RESULTS.NOT_APPLIED
                    End If

                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return myResult

        End Function

        ''' <summary>
        ''' Encodes History Arm Name
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/08/2011</remarks>
        Private Function EncodeHistoryArmName(ByVal pArm As HISTORY_AREAS) As String

            Dim res As String = "x"

            Try
                Select Case pArm
                    Case HISTORY_AREAS.SAM_POS : res = CInt(HISTORY_ARM_NAMES.SAMPLE).ToString
                    Case HISTORY_AREAS.REG1_POS : res = CInt(HISTORY_ARM_NAMES.REAGENT1).ToString
                    Case HISTORY_AREAS.REG2_POS : res = CInt(HISTORY_ARM_NAMES.REAGENT2).ToString
                    Case HISTORY_AREAS.MIX1_POS, HISTORY_AREAS.MIX1_TEST : res = CInt(HISTORY_ARM_NAMES.MIXER1).ToString
                    Case HISTORY_AREAS.MIX2_POS, HISTORY_AREAS.MIX2_TEST : res = CInt(HISTORY_ARM_NAMES.MIXER2).ToString

                End Select


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.EncodeHistoryArmName", EventLogEntryType.Error, False)
                res = "x"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Arm Position Identification
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/08/2011</remarks>
        Private Function EncodeHistoryArmPos(ByVal pPos As HISTORY_ARM_POSITIONS) As String

            Dim res As String = "xx"

            Try
                res = CInt(pPos).ToString("00")

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.EncodeHistoryArmPos", EventLogEntryType.Error, False)
                res = "xx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Value
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pTaskType"></param>
        ''' <param name="pItem"></param>
        ''' <param name="pLength"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataValue(ByVal pData As String, _
                                                ByVal pArea As HISTORY_AREAS, _
                                                ByVal pTaskType As PreloadedMasterDataEnum, _
                                                ByVal pItem As String, _
                                                Optional ByVal pLength As Integer = 5) As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try
                Dim myIndex As Integer = -1

                Select Case pArea
                    Case HISTORY_AREAS.OPT
                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                Select Case pItem
                                    Case "LED_INT" : myIndex = 0
                                    Case "WAVE_LEN" : myIndex = 5
                                    Case "NUM_WELL" : myIndex = 10
                                    Case "WELL_STEP" : myIndex = 15
                                    Case "ROTOR_VAL" : myIndex = 20
                                    Case "ENCODER_VAL" : myIndex = 25    ' XBC 05/01/2012 - Add Encoder functionality

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                Select Case pItem
                                    Case "LED_INT" : myIndex = 0
                                    Case "WAVE_LEN" : myIndex = 5
                                    Case "NUM_WELL" : myIndex = 10
                                    Case "WELL_STEP" : myIndex = 15
                                    Case "ROTOR_VAL" : myIndex = 20

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select

                    Case HISTORY_AREAS.WS_POS
                        Select Case pTaskType
                            Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES
                                Select Case pItem
                                    Case "WS_VAL" : myIndex = 0

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES
                                Select Case pItem
                                    Case "WS_VAL" : myIndex = 0

                                End Select

                            Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES
                                'not applied
                        End Select

                End Select

                If myIndex >= 0 Then

                    Dim myResultNumber As String = ""
                    myResultNumber = pData.Substring(myIndex, pLength)
                    If IsNumeric(myResultNumber) Then
                        res = CInt(myResultNumber).ToString
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes Arms’ positions values
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pPosition"></param>
        ''' <param name="pConnection"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/08/2011</remarks>
        Private Function DecodeHistoryArmValues(ByVal pData As String, _
                                                ByVal pPosition As HISTORY_ARM_POSITIONS, _
                                                Optional ByVal pConnection As SqlClient.SqlConnection = Nothing, _
                                               Optional ByVal pLanguageId As String = "") As List(Of String)

            Dim myLine As New List(Of String)
            Dim MLRD As New MultilanguageResourcesDelegate

            Try
                Dim myIndex As Integer = -1
                Dim myValue As String = ""
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""

                Select Case pPosition
                    Case HISTORY_ARM_POSITIONS.Dispensation : myIndex = 1
                    Case HISTORY_ARM_POSITIONS.Predilution : myIndex = 14
                    Case HISTORY_ARM_POSITIONS.Z_Ref : myIndex = 27
                    Case HISTORY_ARM_POSITIONS.Washing : myIndex = 40
                    Case HISTORY_ARM_POSITIONS.Ring1 : myIndex = 53
                    Case HISTORY_ARM_POSITIONS.Ring2 : myIndex = 66
                    Case HISTORY_ARM_POSITIONS.Ring3 : myIndex = 79
                    Case HISTORY_ARM_POSITIONS.Z_Tube : myIndex = 92
                    Case HISTORY_ARM_POSITIONS.ISE_Pos : myIndex = 105
                    Case HISTORY_ARM_POSITIONS.Parking : myIndex = 118

                End Select


                If myIndex >= 0 Then

                    Dim myResultNumber As String

                    '(Polar value)
                    myResultNumber = pData.Substring(myIndex, 4)
                    If IsNumeric(myResultNumber) Then
                        myValue = CInt(myResultNumber).ToString
                        If myValue.Length >= 0 Then
                            text1 = (MLRD.GetResourceText(pConnection, "LBL_SRV_ARM_POLAR_VALUE", pLanguageId) + ": " + myValue.ToString)
                            myLine.Add(text1)
                        End If
                    End If

                    '(Z value)
                    myResultNumber = pData.Substring(myIndex + 4, 4)
                    If IsNumeric(myResultNumber) Then
                        myValue = CInt(myResultNumber).ToString
                        If myValue.Length >= 0 Then
                            text2 = (MLRD.GetResourceText(pConnection, "LBL_SRV_ARM_Z_VALUE", pLanguageId) + ": " + myValue.ToString)
                            myLine.Add(text2)
                        End If
                    End If


                    '(Rotor value)
                    myResultNumber = pData.Substring(myIndex + 8, 4)
                    If IsNumeric(myResultNumber) Then
                        myValue = CInt(myResultNumber).ToString
                        If myValue.Length >= 0 Then
                            text3 = (MLRD.GetResourceText(pConnection, "LBL_SRV_ARM_ROTOR_VALUE", pLanguageId) + ": " + myValue.ToString)
                            myLine.Add(text3)
                        End If
                    End If

                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryArmValues", EventLogEntryType.Error, False)
            End Try

            Return myLine

        End Function

        ''' <summary>
        ''' Decodes Arms’ names
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pArea"></param>
        ''' <param name="pConnection"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/08/2011</remarks>
        Private Function DecodeHistoryArmName(ByVal pData As String, _
                                               ByVal pArea As HISTORY_AREAS, _
                                               Optional ByVal pConnection As SqlClient.SqlConnection = Nothing, _
                                               Optional ByVal pLanguageId As String = "") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try
                Dim myIndex As Integer = -1

                Select Case pArea

                    Case HISTORY_AREAS.SAM_POS, _
                    HISTORY_AREAS.REG1_POS, _
                    HISTORY_AREAS.REG2_POS, _
                    HISTORY_AREAS.MIX1_POS, _
                    HISTORY_AREAS.MIX2_POS, _
                    HISTORY_AREAS.MIX1_TEST, _
                    HISTORY_AREAS.MIX2_TEST

                        myIndex = 0


                End Select


                Dim myArmName As HISTORY_ARM_NAMES = HISTORY_ARM_NAMES.None

                If myIndex >= 0 Then
                    Dim myResultNumber As String = ""
                    myResultNumber = pData.Substring(myIndex, 1)
                    If IsNumeric(myResultNumber) Then
                        myArmName = CType(myResultNumber, HISTORY_ARM_NAMES)

                        Select Case myArmName
                            Case HISTORY_ARM_NAMES.SAMPLE
                                res = MLRD.GetResourceText(pConnection, "LBL_SRV_SampleArm", pLanguageId)
                            Case HISTORY_ARM_NAMES.REAGENT1
                                res = MLRD.GetResourceText(pConnection, "LBL_SRV_Reagent1Arm", pLanguageId)
                            Case HISTORY_ARM_NAMES.REAGENT2
                                res = MLRD.GetResourceText(pConnection, "LBL_SRV_Reagent2Arm", pLanguageId)
                            Case HISTORY_ARM_NAMES.MIXER1
                                res = MLRD.GetResourceText(pConnection, "LBL_SRV_Mixer1Arm", pLanguageId)
                            Case HISTORY_ARM_NAMES.MIXER2
                                res = MLRD.GetResourceText(pConnection, "LBL_SRV_Mixer2Arm", pLanguageId)



                        End Select
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryArmName", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes Arm’s position’s names
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pArm"></param>
        ''' <param name="pPosition"></param>
        ''' <param name="pConnection"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/08/2011</remarks>
        Private Function DecodeHistoryPosName(ByVal pData As String, _
                                              ByVal pArm As HISTORY_AREAS, _
                                               ByVal pPosition As HISTORY_ARM_POSITIONS, _
                                               Optional ByVal pConnection As SqlClient.SqlConnection = Nothing, _
                                               Optional ByVal pLanguageId As String = "") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                Select Case pPosition
                    Case HISTORY_ARM_POSITIONS.Dispensation
                        res = MLRD.GetResourceText(pConnection, "LBL_SRV_DISP_POS", pLanguageId)

                    Case HISTORY_ARM_POSITIONS.Predilution
                        If pArm = HISTORY_AREAS.SAM_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_DISP2_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.Z_Ref
                        res = MLRD.GetResourceText(pConnection, "LBL_SRV_ZREF_POS", pLanguageId)

                    Case HISTORY_ARM_POSITIONS.Washing
                        res = MLRD.GetResourceText(pConnection, "LBL_SRV_WASH_POS", pLanguageId)

                    Case HISTORY_ARM_POSITIONS.Ring1
                        If pArm = HISTORY_AREAS.SAM_POS Or pArm = HISTORY_AREAS.REG1_POS Or pArm = HISTORY_AREAS.REG2_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_RING1_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.Ring2
                        If pArm = HISTORY_AREAS.SAM_POS Or pArm = HISTORY_AREAS.REG1_POS Or pArm = HISTORY_AREAS.REG2_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_RING2_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.Ring3
                        If pArm = HISTORY_AREAS.SAM_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_RING3_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.Z_Tube
                        If pArm = HISTORY_AREAS.SAM_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_ZTUBE_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.ISE_Pos
                        If pArm = HISTORY_AREAS.SAM_POS Then
                            res = MLRD.GetResourceText(pConnection, "LBL_SRV_ISE_POS", pLanguageId)
                        End If

                    Case HISTORY_ARM_POSITIONS.Parking
                        res = MLRD.GetResourceText(pConnection, "LBL_SRV_PARK_POS", pLanguageId)

                End Select


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.DecodeHistoryPosName", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Gets Result Text
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pResult"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GetResultLanguageResource(ByVal pTask As PreloadedMasterDataEnum, ByRef pdbConnection As SqlClient.SqlConnection, ByVal pResult As HISTORY_RESULTS, ByVal pLanguageId As String) As String

            Dim res As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Try
                Select Case pResult
                    Case HISTORY_RESULTS.OK
                        If pTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES Then
                            res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_Adjusted", pLanguageId)
                        ElseIf pTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES Then
                            res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_Tested", pLanguageId)
                        End If

                    Case HISTORY_RESULTS.NOK, HISTORY_RESULTS.NOT_DONE
                        If pTask = PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES Then
                            res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_NOT_ADJUSTED", pLanguageId)
                        ElseIf pTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES Then
                            res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_NOT_TESTED", pLanguageId)
                        End If

                    Case HISTORY_RESULTS.CANCEL
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_CANCELLED", pLanguageId)

                    Case HISTORY_RESULTS._ERROR
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FAILED", pLanguageId)

                End Select
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.GetResultLanguageResource", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' registering the incidence in historical reports activity
        ''' </summary>
        ''' <remarks>
        ''' Created by SGM 04/08/2011
        ''' </remarks>
        Private Sub UpdateRecommendationsList(ByVal pRecommendationID As HISTORY_RECOMMENDATIONS)
            Try
                ' registering the incidence in historical reports activity

                'If MyClass.RecommendationsReport Is Nothing Then
                '    ReDim MyClass.RecommendationsReport(0)
                'Else
                '    ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                'End If
                'MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = pRecommendationID

                If MyClass.RecommendationsList Is Nothing Then
                    MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
                End If
                MyClass.RecommendationsList.Add(pRecommendationID)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region


        '''' <summary>
        '''' Obtains the text according to the informed Task type, action code and data
        '''' </summary>
        '''' <param name="pTaskID"></param>
        '''' <param name="pActionName"></param>
        '''' <param name="pActionData"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 01/08/2011</remarks>
        'Private Function DecodeHistoryAction(ByVal pTaskID As PreloadedMasterDataEnum, ByVal pActionName As String, ByVal pActionData As String, ByVal pLanguageID As String) As GlobalDataTO

        '    Dim myResultData As New GlobalDataTO
        '    Dim myResultsList As List(Of String)

        '    Try
        '        'Dim myTask As PreloadedMasterDataEnum = CType(pTaskID, GlobalEnumerates.HISTORY_TASK_TYPES)
        '        'Dim myAction As String


        '        'Once the action is determined it is time for make the text with 
        '        'the required captions and obtained values
        '        Dim myResult As HISTORY_RESULTS
        '        For Each c As String In pActionData
        '            If IsNumeric(c) Then
        '                Dim myResultCode As Integer = CInt(c)
        '                myResult = CType(myResultCode, HISTORY_RESULTS)
        '                If myResultsList Is Nothing Then
        '                    myResultsList = New List(Of String)
        '                End If


        '                'create the corresponding text and values
        '                Select Case pActionName.ToUpper.Trim
        '                    Case "WSMIN"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_WS_MINIMUM", myResult.ToString, pLanguageID)

        '                    Case "WSMAX"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_WS_MAXIMUM", myResult.ToString, pLanguageID)

        '                    Case "HCMAX"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_HC_MINIMUM", myResult.ToString, pLanguageID)

        '                    Case "HCMIN"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_HC_MAXIMUM", myResult.ToString, pLanguageID)

        '                    Case "EMPTYLC"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_EMPTY_LC", myResult.ToString, pLanguageID)

        '                    Case "FILLDW"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_FILL_DW", myResult.ToString, pLanguageID)

        '                    Case "TRANSFER"
        '                        myResultData = MyClass.GetHistoryActionDataText("HIS_TRANSFER_DW_LC", myResult.ToString, pLanguageID)

        '                End Select


        '                If Not myResultData.HasError And myResultData.SetDatos IsNot Nothing Then
        '                    Dim myResultItem As String = CStr(myResultData.SetDatos)
        '                    If myResultItem IsNot Nothing Then
        '                        myResultsList.Add(myResult.ToString)
        '                    End If
        '                End If

        '            Else
        '                myResultData.HasError = True
        '                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '                Exit For
        '            End If
        '        Next


        '        Dim myFinalText As String = ""

        '        If myResultsList IsNot Nothing Then
        '            For Each T As String In myResultsList
        '                myFinalText &= vbCrLf & T & vbCrLf
        '            Next
        '        End If

        '        myResultData.SetDatos = myFinalText

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.DecodeHistoryAction", EventLogEntryType.Error, False)
        '    End Try

        '    Return myResultData

        'End Function

        '''' <summary>
        '''' Retrieves the caption related to the specified text
        '''' </summary>
        '''' <param name="pTextId"></param>
        '''' <param name="pValue"></param>
        '''' <param name="pLanguageId"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 01/08/2011</remarks>
        'Private Function GetHistoryActionDataText(ByVal pTextId As String, ByVal pValue As String, ByVal pLanguageId As String) As GlobalDataTO

        '    Dim myResultData As New GlobalDataTO
        '    Dim myMLRD As New MultilanguageResourcesDelegate

        '    Try

        '        Dim myCaption As String = myMLRD.GetResourceText(Nothing, pTextId, pLanguageId)

        '        myResultData.SetDatos = myCaption & vbTab & pValue

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "TankLevelsAdjustmentDelegate.GetHistoryActionDataText", EventLogEntryType.Error, False)
        '    End Try

        '    Return myResultData

        'End Function
#End Region


#Region "To DELETE"
        '''' <summary>
        '''' Creates the Script List for Screen Loading operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 17/11/10</remarks>
        'Private Function SendQueueForLOADING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    'Dim myResultData As New GlobalDataTO
        '    'Try

        '    '    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '    '        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '    '    End If


        '    '    'FOR TESTING
        '    '    '*************************************************************************************************
        '    '    Dim myFwScript1 As New FwScriptQueueItem
        '    '    Dim myFwScript2 As New FwScriptQueueItem

        '    '    'Script1
        '    '    With myFwScript1
        '    '        .FwScriptID = "action2"
        '    '        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '    '        .EvaluateValue = 1
        '    '        .NextOnResultOK = myFwScript2
        '    '        .NextOnResultNG = Nothing
        '    '        .NextOnTimeOut = Nothing
        '    '        .NextOnError = Nothing
        '    '        .ParamList = Nothing ' New List(Of String)

        '    '    End With

        '    '    'Script2
        '    '    With myFwScript2
        '    '        .FwScriptID = "action4"
        '    '        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '    '        .EvaluateValue = 1
        '    '        .NextOnResultOK = Nothing
        '    '        .NextOnResultNG = Nothing
        '    '        .NextOnTimeOut = Nothing
        '    '        .NextOnError = Nothing
        '    '        .ParamList = Nothing ' New List(Of String)
        '    '    End With

        '    '    '*************************************************************************************************************
        '    '    ''in case of specific handling the response
        '    '    '.ResponseEventHandling  = True 'enable event handling
        '    '    'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '    '    'add to the queue list
        '    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)

        '    'Catch ex As Exception
        '    '    myResultData.HasError = True
        '    '    myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '    '    myResultData.ErrorMessage = ex.Message

        '    '    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '    '        myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '    '    End If

        '    '    'Dim myLogAcciones As New ApplicationLogManager()
        '    '    GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForLOADING", EventLogEntryType.Error, False)
        '    'End Try
        '    'Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Starting the Test operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForTEST_BEGINING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If



        '        'FOR TESTING
        '        '*************************************************************************************************
        '        Dim myFwScript1 As New FwScriptQueueItem
        '        Dim myFwScript2 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = "action2"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)

        '        End With

        '        'Script2
        '        With myFwScript2
        '            .FwScriptID = "action4"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForTEST_BEGINING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Closing Screen operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 04/01/11</remarks>
        'Private Function SendQueueForCLOSING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Try

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If



        '        'FOR TESTING
        '        '*************************************************************************************************
        '        Dim myFwScript1 As New FwScriptQueueItem
        '        Dim myFwScript2 As New FwScriptQueueItem

        '        'Script1
        '        With myFwScript1
        '            .FwScriptID = "action2"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)

        '        End With

        '        'Script2
        '        With myFwScript2
        '            .FwScriptID = "action4"
        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            .ParamList = Nothing ' New List(Of String)
        '        End With

        '        '*************************************************************************************************************
        '        ''in case of specific handling the response
        '        '.ResponseEventHandling  = True 'enable event handling
        '        'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

        '        'add to the queue list
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "PositionsAdjustmentDelegate.SendQueueForCLOSING", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function
#End Region

    End Class

End Namespace