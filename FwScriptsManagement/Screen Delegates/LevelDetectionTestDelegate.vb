Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.BL

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class LevelDetectionTestDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.ReportCountTimeout = 0
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Enumerations"
        Public Enum OPERATIONS
            _NONE = 0
            READ_FREQUENCY = 1
            START_DETECTION_TEST = 2
            END_DETECTION_TEST = 3
        End Enum

        Public Enum Arms
            _NONE = 0
            SAMPLE = 1
            REAGENT1 = 2
            REAGENT2 = 3
        End Enum

        Public Enum Rotors
            _NONE = 0
            SAMPLES = 1
            REAGENTS = 2
        End Enum

        Public Enum Rings
            _NONE = 0
            RING1 = 1
            RING2 = 2
            RING3 = 3
        End Enum

        Public Enum HISTORY_AREAS
            NONE = 0
            LEVEL_FREQ_READ = 1
            LEVEL_DET_TEST = 2
        End Enum

        Public Enum HISTORY_RESULTS
            _ERROR = -1
            NOT_DONE = 0
            DETECTED = 1
            NOT_DETECTED = 2
            FREQ_OK = 3
            FREQ_NOK = 4
        End Enum

#End Region

        Public Const FREQ_UNIT As String = "KHz"

#Region "Declarations"
        Private CurrentOperationAttr As OPERATIONS                  ' controls the operation which is currently executed in each time

        Private CurrentHistoryAreaAttr As HISTORY_AREAS = HISTORY_AREAS.NONE

        Private myGroupSeparator As String

        Private myFrequencyRefreshDS As New UIRefreshDS
        Private myRotorsConfiguration As New AnalyzerModelRotorsConfigDS

        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer

#End Region

#Region "Attributes"

        'Frequency Read
        Private ReadDM1FreqRequestedAttr As Boolean
        Private ReadDR1FreqRequestedAttr As Boolean
        Private ReadDR2FreqRequestedAttr As Boolean

        Private SampleFreqMaxLimitAttr As Single
        Private SampleFreqMinLimitAttr As Single

        Private ReagentsFreqMaxLimitAttr As Single
        Private ReagentsFreqMinLimitAttr As Single

        Private SampleFrequencyValueAttr As Single = -1
        Private Reagent1FrequencyValueAttr As Single = -1
        Private Reagent2FrequencyValueAttr As Single = -1

        'Level Detection
        Private CurrentArmAttr As Arms = Arms._NONE
        Private CurrentRotorAttr As Rotors = Rotors._NONE
        Private CurrentRotorPositionAttr As Integer = 0
        Private CurrentRingAttr As Rings = Rings._NONE

        'Sample
        Private S1RotorRing1HorPosAttr As Integer
        Private S1RotorRing1DetMaxVerPosAttr As Integer
        Private S1RotorRing2HorPosAttr As Integer
        Private S1RotorRing2DetMaxVerPosAttr As Integer
        Private S1RotorRing3HorPosAttr As Integer
        Private S1RotorRing3DetMaxVerPosAttr As Integer
        Private S1VerticalSafetyPosAttr As Integer
        Private S1RotorPosRing1Attr As Integer
        Private S1RotorPosRing2Attr As Integer
        Private S1RotorPosRing3Attr As Integer
        Private S1ParkingZAttr As Integer
        Private S1ParkingPolarAttr As Integer

        'Reagent 1
        Private R1RotorRing1HorPosAttr As Integer
        Private R1RotorRing1DetMaxVerPosAttr As Integer
        Private R1RotorRing2HorPosAttr As Integer
        Private R1RotorRing2DetMaxVerPosAttr As Integer
        Private R1VerticalSafetyPosAttr As Integer
        Private R1RotorPosRing1Attr As Integer
        Private R1RotorPosRing2Attr As Integer
        Private R1ParkingZAttr As Integer
        Private R1ParkingPolarAttr As Integer

        'Reagent 2
        Private R2RotorRing1HorPosAttr As Integer
        Private R2RotorRing1DetMaxVerPosAttr As Integer
        Private R2RotorRing2HorPosAttr As Integer
        Private R2RotorRing2DetMaxVerPosAttr As Integer
        Private R2VerticalSafetyPosAttr As Integer
        Private R2RotorPosRing1Attr As Integer
        Private R2RotorPosRing2Attr As Integer
        Private R2ParkingZAttr As Integer
        Private R2ParkingPolarAttr As Integer

        ''washing???
        'Private S1WSVerticalRefPosAttr As Integer
        'Private S1WSHorizontalPosAttr As Integer
        'Private R1WSVerticalRefPosAttr As Integer
        'Private R1WSHorizontalPosAttr As Integer
        'Private R2WSVerticalRefPosAttr As Integer
        'Private R2WSHorizontalPosAttr As Integer

        'Rotors positions
        Private SampleRotorFirstPositionAttr As Integer
        Private SampleRotorLastPositionAttr As Integer
        Private ReagentsRotorFirstPositionAttr As Integer
        Private ReagentsRotorLastPositionAttr As Integer

        Private currentLanguageAttr As String

        Private AnalyzerIDAttr As String

        Private HomesDoneAttr As Boolean = False


        'History
        Private SampleFrequencyReadResultAttr As HISTORY_RESULTS
        Private Reagent1FrequencyReadResultAttr As HISTORY_RESULTS
        Private Reagent2FrequencyReadResultAttr As HISTORY_RESULTS

        Private DetectionTestResultAttr As HISTORY_RESULTS

#End Region

#Region "Properties"

#Region "Common"

        Public Property currentLanguage() As String
            Get
                Return MyClass.currentLanguageAttr
            End Get
            Set(ByVal value As String)
                MyClass.currentLanguageAttr = value
            End Set
        End Property

        Public Property AnalyzerId() As String
            Get
                Return MyClass.AnalyzerIDAttr
            End Get
            Set(ByVal value As String)
                MyClass.AnalyzerIDAttr = value
            End Set
        End Property

        Public Property HomesDone() As Boolean
            Get
                Return HomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                If HomesDoneAttr <> value Then
                    HomesDoneAttr = value
                End If
            End Set
        End Property

        Public Property CurrentOperation() As OPERATIONS
            Get
                Return CurrentOperationAttr
            End Get
            Set(ByVal value As OPERATIONS)
                If CurrentOperationAttr <> value Then
                    CurrentOperationAttr = value
                End If
            End Set
        End Property

        Public Property CurrentHistoryArea() As HISTORY_AREAS
            Get
                Return CurrentHistoryAreaAttr
            End Get
            Set(ByVal value As HISTORY_AREAS)
                If CurrentHistoryAreaAttr <> value Then
                    CurrentHistoryAreaAttr = value
                End If
            End Set
        End Property
#End Region
        
#Region "Detection Test"

        Public Property CurrentArm() As Arms
            Get
                Return CurrentArmAttr
            End Get
            Set(ByVal value As Arms)
                CurrentArmAttr = value
            End Set
        End Property

        Public Property CurrentRotor() As Rotors
            Get
                Return CurrentRotorAttr
            End Get
            Set(ByVal value As Rotors)
                CurrentRotorAttr = value
            End Set
        End Property

        Public Property CurrentRotorPosition() As Integer
            Get
                Return CurrentRotorPositionAttr
            End Get
            Set(ByVal value As Integer)
                CurrentRotorPositionAttr = value
            End Set
        End Property

        Public ReadOnly Property CurrentRing() As Rings
            Get
                Dim myGlobal As New GlobalDataTO
                Dim myRing As Rings = Rings._NONE
                myGlobal = MyClass.GetRotorRingByPosition(MyClass.CurrentRotor, MyClass.CurrentRotorPosition)
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myRing = CType(myGlobal.SetDatos, Rings)
                End If
                Return myRing
            End Get
        End Property


        'Rotors Positions
        Public ReadOnly Property SampleRotorFirstPosition() As Integer
            Get
                Return SampleRotorFirstPositionAttr
            End Get
        End Property

        Public ReadOnly Property SampleRotorLastPosition() As Integer
            Get
                Return SampleRotorLastPositionAttr
            End Get
        End Property

        Public ReadOnly Property ReagentsRotorFirstPosition() As Integer
            Get
                Return ReagentsRotorFirstPositionAttr
            End Get
        End Property

        Public ReadOnly Property ReagentsRotorLastPosition() As Integer
            Get
                Return ReagentsRotorLastPositionAttr
            End Get
        End Property


        Public Property S1RotorRing1HorPos() As Integer
            Get
                Return S1RotorRing1HorPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing1HorPosAttr = value
            End Set
        End Property

        Public Property S1RotorRing1DetMaxVerPos() As Integer
            Get
                Return S1RotorRing1DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property


        Public Property S1RotorRing2HorPos() As Integer
            Get
                Return S1RotorRing2HorPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing2HorPosAttr = value
            End Set
        End Property

        Public Property S1RotorRing2DetMaxVerPos() As Integer
            Get
                Return S1RotorRing2DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing2DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property S1RotorRing3HorPos() As Integer
            Get
                Return S1RotorRing3HorPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing3HorPosAttr = value
            End Set
        End Property

        Public Property S1RotorRing3DetMaxVerPos() As Integer
            Get
                Return S1RotorRing3DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                S1RotorRing3DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property S1VerticalSafetyPos() As Integer
            Get
                Return S1VerticalSafetyPosAttr
            End Get
            Set(ByVal value As Integer)
                S1VerticalSafetyPosAttr = value
            End Set
        End Property


        Public Property S1RotorPosRing1() As Integer
            Get
                Return S1RotorPosRing1Attr
            End Get
            Set(ByVal value As Integer)
                S1RotorPosRing1Attr = value
            End Set
        End Property

        Public Property S1RotorPosRing2() As Integer
            Get
                Return S1RotorPosRing2Attr
            End Get
            Set(ByVal value As Integer)
                S1RotorPosRing2Attr = value
            End Set
        End Property

        Public Property S1RotorPosRing3() As Integer
            Get
                Return S1RotorPosRing3Attr
            End Get
            Set(ByVal value As Integer)
                S1RotorPosRing3Attr = value
            End Set
        End Property


        Public Property R1RotorRing1HorPos() As Integer
            Get
                Return R1RotorRing1HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1HorPosAttr = value
            End Set
        End Property

        Public Property R1RotorRing1DetMaxVerPos() As Integer
            Get
                Return R1RotorRing1DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property R1RotorRing2HorPos() As Integer
            Get
                Return R1RotorRing2HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing2HorPosAttr = value
            End Set
        End Property

        Public Property R1RotorRing2DetMaxVerPos() As Integer
            Get
                Return R1RotorRing2DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property


        Public Property R1VerticalSafetyPos() As Integer
            Get
                Return R1VerticalSafetyPosAttr
            End Get
            Set(ByVal value As Integer)
                R1VerticalSafetyPosAttr = value
            End Set
        End Property


        Public Property R1RotorPosRing1() As Integer
            Get
                Return R1RotorPosRing1Attr
            End Get
            Set(ByVal value As Integer)
                R1RotorPosRing1Attr = value
            End Set
        End Property

        Public Property R1RotorPosRing2() As Integer
            Get
                Return R1RotorPosRing2Attr
            End Get
            Set(ByVal value As Integer)
                R1RotorPosRing2Attr = value
            End Set
        End Property

        Public Property R2RotorRing1HorPos() As Integer
            Get
                Return R2RotorRing1HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorRing1HorPosAttr = value
            End Set
        End Property

        Public Property R2RotorRing1DetMaxVerPos() As Integer
            Get
                Return R2RotorRing1DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property R2RotorRing2HorPos() As Integer
            Get
                Return R2RotorRing2HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorRing2HorPosAttr = value
            End Set
        End Property

        Public Property R2RotorRing2DetMaxVerPos() As Integer
            Get
                Return R2RotorRing2DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property R2VerticalSafetyPos() As Integer
            Get
                Return R2VerticalSafetyPosAttr
            End Get
            Set(ByVal value As Integer)
                R2VerticalSafetyPosAttr = value
            End Set
        End Property


        Public Property R2RotorPosRing1() As Integer
            Get
                Return R2RotorPosRing1Attr
            End Get
            Set(ByVal value As Integer)
                R2RotorPosRing1Attr = value
            End Set
        End Property

        Public Property R2RotorPosRing2() As Integer
            Get
                Return R2RotorPosRing2Attr
            End Get
            Set(ByVal value As Integer)
                R2RotorPosRing2Attr = value
            End Set
        End Property


#Region "Parking"

        Public Property S1ParkingZ() As Integer
            Get
                Return S1ParkingZAttr
            End Get
            Set(ByVal value As Integer)
                S1ParkingZAttr = value
            End Set
        End Property

        Public Property S1ParkingPolar() As Integer
            Get
                Return S1ParkingPolarAttr
            End Get
            Set(ByVal value As Integer)
                S1ParkingPolarAttr = value
            End Set
        End Property

        Public Property R1ParkingZ() As Integer
            Get
                Return R1ParkingZAttr
            End Get
            Set(ByVal value As Integer)
                R1ParkingZAttr = value
            End Set
        End Property

        Public Property R1ParkingPolar() As Integer
            Get
                Return R1ParkingPolarAttr
            End Get
            Set(ByVal value As Integer)
                R1ParkingPolarAttr = value
            End Set
        End Property

        Public Property R2ParkingZ() As Integer
            Get
                Return R2ParkingZAttr
            End Get
            Set(ByVal value As Integer)
                R2ParkingZAttr = value
            End Set
        End Property

        Public Property R2ParkingPolar() As Integer
            Get
                Return R2ParkingPolarAttr
            End Get
            Set(ByVal value As Integer)
                R2ParkingPolarAttr = value
            End Set
        End Property

#End Region

#Region "Washing Used?"
        'Public Property S1WSHorizontalPos() As Integer
        '    Get
        '        Return S1WSHorizontalPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        S1WSHorizontalPosAttr = value
        '    End Set
        'End Property

        'Public Property S1WSVerticalRefPos() As Integer
        '    Get
        '        Return S1WSVerticalRefPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        S1WSVerticalRefPosAttr = value
        '    End Set
        'End Property

        'Public Property R1WSHorizontalPos() As Integer
        '    Get
        '        Return R1WSHorizontalPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        R1WSHorizontalPosAttr = value
        '    End Set
        'End Property

        'Public Property R1WSVerticalRefPos() As Integer
        '    Get
        '        Return R1WSVerticalRefPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        R1WSVerticalRefPosAttr = value
        '    End Set
        'End Property

        'Public Property R2WSHorizontalPos() As Integer
        '    Get
        '        Return R2WSHorizontalPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        R2WSHorizontalPosAttr = value
        '    End Set
        'End Property

        'Public Property R2WSVerticalRefPos() As Integer
        '    Get
        '        Return R2WSVerticalRefPosAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        R2WSVerticalRefPosAttr = value
        '    End Set
        'End Property

#End Region

#End Region

#Region "Frequency Read"

        'requesting flags
        Public Property ReadDM1FreqRequested() As Boolean
            Get
                Return MyClass.ReadDM1FreqRequestedAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadDM1FreqRequestedAttr = value
            End Set
        End Property

        Public Property ReadDR1FreqRequested() As Boolean
            Get
                Return MyClass.ReadDR1FreqRequestedAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadDR1FreqRequestedAttr = value
            End Set
        End Property

        Public Property ReadDR2FreqRequested() As Boolean
            Get
                Return MyClass.ReadDR2FreqRequestedAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadDR2FreqRequestedAttr = value
            End Set
        End Property

        Public ReadOnly Property SampleFreqMinLimit() As Single
            Get
                Return SampleFreqMinLimitAttr
            End Get
        End Property

        Public ReadOnly Property SampleFreqMaxLimit() As Single
            Get
                Return SampleFreqMaxLimitAttr
            End Get
        End Property

        Public ReadOnly Property ReagentsFreqMinLimit() As Single
            Get
                Return ReagentsFreqMinLimitAttr
            End Get
        End Property

        Public ReadOnly Property ReagentsFreqMaxLimit() As Single
            Get
                Return ReagentsFreqMaxLimitAttr
            End Get
        End Property


        Public Property SampleFrequencyValue() As Single
            Get
                Return SampleFrequencyValueAttr
            End Get
            Set(ByVal value As Single)
                SampleFrequencyValueAttr = value
            End Set
        End Property

        Public Property Reagent1FrequencyValue() As Single
            Get
                Return Reagent1FrequencyValueAttr
            End Get
            Set(ByVal value As Single)
                Reagent1FrequencyValueAttr = value
            End Set
        End Property

        Public Property Reagent2FrequencyValue() As Single
            Get
                Return Reagent2FrequencyValueAttr
            End Get
            Set(ByVal value As Single)
                Reagent2FrequencyValueAttr = value
            End Set
        End Property

#End Region

#Region "History"
       

        Public Property SampleFrequencyReadResult() As HISTORY_RESULTS
            Get
                Return SampleFrequencyReadResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If SampleFrequencyReadResultAttr <> value Then
                    SampleFrequencyReadResultAttr = value
                End If
            End Set
        End Property

        Public Property Reagent1FrequencyReadResult() As HISTORY_RESULTS
            Get
                Return Reagent1FrequencyReadResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If Reagent1FrequencyReadResultAttr <> value Then
                    Reagent1FrequencyReadResultAttr = value
                End If
            End Set
        End Property

        Public Property Reagent2FrequencyReadResult() As HISTORY_RESULTS
            Get
                Return Reagent2FrequencyReadResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If Reagent2FrequencyReadResultAttr <> value Then
                    Reagent2FrequencyReadResultAttr = value
                End If
            End Set
        End Property

        Public Property DetectionTestResult() As HISTORY_RESULTS
            Get
                Return DetectionTestResult
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                If DetectionTestResultAttr <> value Then
                    DetectionTestResultAttr = value
                End If
            End Set
        End Property

#End Region


#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 22/03/11</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles MyClass.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' XBC 05/05/2011 - timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        If MyClass.RecommendationsList Is Nothing Then
                            MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
                        End If
                        MyClass.RecommendationsList.Add(HISTORY_RECOMMENDATIONS.ERR_COMM)
                    End If

                    'If pData.ToString = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString Then
                    '    MyClass.StatusStressModeAttr = STRESS_STATUS.FINISHED_ERR
                    'End If

                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions

                Select Case CurrentOperation

                    Case OPERATIONS.START_DETECTION_TEST
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK

                        End Select

                    Case OPERATIONS.END_DETECTION_TEST
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK

                        End Select

                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "PhotometryAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XBC 22/03/11
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myAdjGroup As ADJUSTMENT_GROUPS
                Select Case CurrentArm
                    Case Arms.SAMPLE : myAdjGroup = ADJUSTMENT_GROUPS.SAMPLE_LEVEL_DET
                    Case Arms.REAGENT1 : myAdjGroup = ADJUSTMENT_GROUPS.REAGENT1_LEVEL_DET
                    Case Arms.REAGENT2 : myAdjGroup = ADJUSTMENT_GROUPS.REAGENT2_LEVEL_DET
                End Select

                ' Create the list of Scripts which are need to initialize this Adjustment
                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.LEVEL_DETECTING
                        myResultData = MyClass.SendQueueForLEVEL_DETECTION_START(myAdjGroup)

                    Case ADJUSTMENT_MODES.LEVEL_DETECTED
                        myResultData = MyClass.SendQueueForLEVEL_DETECTION_END(myAdjGroup)

                End Select
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function



        ''' <summary>
        ''' Sends the specific FwScript Queue for starting Level Detection of the indicated needle
        ''' </summary>
        ''' <param name="pAdjustment"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 15/12/2012</remarks>
        Private Function SendQueueForLEVEL_DETECTION_START(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myHomeScriptsList As List(Of FwScriptQueueItem) = Nothing
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.START_DETECTION_TEST

                myResultData = MyBase.GetPendingPreliminaryHomeScripts(pAdjustment, myFwScript1)
                If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                End If

                If Not myResultData.HasError Then

                    Dim myRing As Rings = MyClass.CurrentRing

                    If myRing <> Rings._NONE Then

                        With myFwScript1
                            Select Case pAdjustment
                                Case ADJUSTMENT_GROUPS.SAMPLE_LEVEL_DET : .FwScriptID = FwSCRIPTS_IDS.SAMPLE_LEVEL_DET.ToString
                                Case ADJUSTMENT_GROUPS.REAGENT1_LEVEL_DET : .FwScriptID = FwSCRIPTS_IDS.REAGENT1_LEVEL_DET.ToString
                                Case ADJUSTMENT_GROUPS.REAGENT2_LEVEL_DET : .FwScriptID = FwSCRIPTS_IDS.REAGENT2_LEVEL_DET.ToString

                            End Select

                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing

                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.CurrentRotorPositionAttr.ToString)
                            .ParamList.Add(MyClass.S1VerticalSafetyPosAttr.ToString)

                            Select Case pAdjustment
                                Case ADJUSTMENT_GROUPS.SAMPLE_LEVEL_DET

                                    Select Case myRing
                                        Case Rings.RING1
                                            .ParamList.Add(MyClass.S1RotorRing1HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.S1RotorRing1DetMaxVerPosAttr.ToString)

                                        Case Rings.RING2
                                            .ParamList.Add(MyClass.S1RotorRing2HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.S1RotorRing2DetMaxVerPosAttr.ToString)

                                        Case Rings.RING3
                                            .ParamList.Add(MyClass.S1RotorRing3HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.S1RotorRing3DetMaxVerPosAttr.ToString)

                                    End Select

                                Case ADJUSTMENT_GROUPS.REAGENT1_LEVEL_DET
                                    Select Case myRing
                                        Case Rings.RING1
                                            .ParamList.Add(MyClass.R1RotorRing1HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.R1RotorRing1DetMaxVerPosAttr.ToString)

                                        Case Rings.RING2
                                            .ParamList.Add(MyClass.R1RotorRing2HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.R1RotorRing2DetMaxVerPosAttr.ToString)

                                    End Select

                                Case ADJUSTMENT_GROUPS.REAGENT2_LEVEL_DET
                                    Select Case myRing
                                        Case Rings.RING1
                                            .ParamList.Add(MyClass.R2RotorRing1HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.R2RotorRing1DetMaxVerPosAttr.ToString)

                                        Case Rings.RING2
                                            .ParamList.Add(MyClass.R2RotorRing2HorPosAttr.ToString)
                                            .ParamList.Add(MyClass.R2RotorRing2DetMaxVerPosAttr.ToString)

                                    End Select

                            End Select
                        End With

                        'add to the queue list
                        If myHomeScriptsList IsNot Nothing And myHomeScriptsList.Count > 0 Then
                            For i As Integer = 0 To myHomeScriptsList.Count - 1
                                If i = 0 Then
                                    ' First Script
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), True)
                                Else
                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), False)
                                End If
                            Next
                            If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        Else
                            If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                        End If
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.SendQueueForLEVEL_DETECTION_START", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Sends the specific FwScript Queue for ending Level Detection of the indicated needle
        ''' </summary>
        ''' <param name="pAdjustment"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 15/12/2012</remarks>
        Private Function SendQueueForLEVEL_DETECTION_END(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.END_DETECTION_TEST

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
                        Case ADJUSTMENT_GROUPS.SAMPLE_LEVEL_DET
                            ' sample arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.SAMPLE_TEST_END.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.S1VerticalSafetyPosAttr.ToString)
                            .ParamList.Add(Me.S1ParkingPolarAttr.ToString)

                        Case ADJUSTMENT_GROUPS.REAGENT1_LEVEL_DET
                            ' reagent1 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT1_TEST_END.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.R1VerticalSafetyPosAttr.ToString)
                            .ParamList.Add(Me.R1ParkingPolarAttr.ToString)

                        Case ADJUSTMENT_GROUPS.REAGENT2_LEVEL_DET
                            ' reagent2 arm Parking
                            .FwScriptID = FwSCRIPTS_IDS.REAGENT2_TEST_END.ToString
                            ' expects 2 params
                            .ParamList = New List(Of String)
                            .ParamList.Add(Me.R2VerticalSafetyPosAttr.ToString)
                            .ParamList.Add(Me.R2ParkingPolarAttr.ToString)


                    End Select

                End With
                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.SendQueueForLEVEL_DETECTION_END", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get Level Detector's frequency Limits values
        ''' </summary>
        ''' <remarks>Created by SGM 15/12/2011</remarks>
        Public Function GetFrequencyLimits() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                ' Get Value limit ranges
                Dim myFieldLimitsDS As New FieldLimitsDS
                Dim myFieldLimitsDelegate As New FieldLimitsDelegate()

                myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_SAMPLE_LEVEL_DET_FREQ)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        MyClass.SampleFreqMinLimitAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        MyClass.SampleFreqMaxLimitAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    End If
                End If


                myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.SRV_REAGENTS_LEVEL_DET_FREQ)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        MyClass.ReagentsFreqMinLimitAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        MyClass.ReagentsFreqMaxLimitAttr = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.GetFrequencyLimits", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets all Rotors' configuration containing rings and positions' information
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 15/12/2012</remarks>
        Public Function LoadRotorsConfiguration() As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try

                Dim myAnalyzerModelRotorsConfigDelegate As New AnalyzerModelRotorsConfigDelegate

                myGlobalDataTO = myAnalyzerModelRotorsConfigDelegate.GetAnalyzerRotorsConfiguration(Nothing, MyClass.AnalyzerId)

                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                    MyClass.myRotorsConfiguration = CType(myGlobalDataTO.SetDatos, AnalyzerModelRotorsConfigDS)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.LoadRotorsConfiguration", EventLogEntryType.Error, False)

            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets positions' information related to the informed Rotor
        ''' </summary>
        ''' <param name="pRotor"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 15/12/2012
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function LoadRotorPositions(ByVal pRotor As Rotors) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myRotor As String = pRotor.ToString
                Dim myRingsCount As Integer


                If MyClass.myRotorsConfiguration IsNot Nothing AndAlso MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig.Rows.Count > 0 Then

                    Dim myRowRing1 As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow = _
                                (From a As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig _
                                Where a.AnalyzerID.Trim = MyClass.AnalyzerId.Trim And _
                                a.RotorType.Trim = myRotor.Trim And _
                                a.RingNumber = 1 Select a).First
                    'Where a.AnalyzerID.ToUpper.Trim = MyClass.AnalyzerId.ToUpper.Trim And _
                    'a.RotorType.ToUpper.Trim = myRotor.Trim And _

                    myRingsCount = (From a As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig _
                                Where a.AnalyzerID.Trim = MyClass.AnalyzerId.Trim And _
                                a.RotorType.Trim = myRotor.Trim Select a).ToList().Count
                    'Where a.AnalyzerID.ToUpper.Trim = MyClass.AnalyzerId.ToUpper.Trim And _
                    'a.RotorType.ToUpper.Trim = myRotor.Trim Select a).ToList().Count

                    If myRingsCount > 0 Then
                        Dim myRowRing2 As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow = _
                                   (From a As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig _
                                   Where a.AnalyzerID.Trim = MyClass.AnalyzerId.Trim And _
                                   a.RotorType.Trim = myRotor.Trim And _
                                   a.RingNumber = myRingsCount Select a).First
                        'Where a.AnalyzerID.ToUpper.Trim = MyClass.AnalyzerId.ToUpper.Trim And _
                        'a.RotorType.ToUpper.Trim = myRotor.Trim And _

                        If myRowRing1 IsNot Nothing And myRowRing2 IsNot Nothing Then

                            Select Case pRotor
                                Case Rotors.SAMPLES
                                    MyClass.SampleRotorFirstPositionAttr = myRowRing1.FirstCellNumber
                                    MyClass.SampleRotorLastPositionAttr = myRowRing2.LastCellNumber

                                Case Rotors.REAGENTS
                                    MyClass.ReagentsRotorFirstPositionAttr = myRowRing1.FirstCellNumber
                                    MyClass.ReagentsRotorLastPositionAttr = myRowRing2.LastCellNumber

                            End Select

                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.LoadRotorPositions", EventLogEntryType.Error, False)

            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets the Ring that the informed position and rotor belong to
        ''' </summary>
        ''' <param name="pRotor"></param>
        ''' <param name="pPosition"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 15/12/2012
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function GetRotorRingByPosition(ByVal pRotor As Rotors, ByVal pPosition As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myRotor As String = pRotor.ToString
                Dim myRing As Rings = Rings._NONE


                If MyClass.myRotorsConfiguration IsNot Nothing AndAlso MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig.Rows.Count > 0 Then

                    Dim myRingRows As List(Of AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow) = _
                                 (From a As AnalyzerModelRotorsConfigDS.tfmwAnalyzerModelRotorsConfigRow In MyClass.myRotorsConfiguration.tfmwAnalyzerModelRotorsConfig _
                                 Where a.AnalyzerID.Trim = MyClass.AnalyzerId.Trim And _
                                 a.RotorType.Trim = myRotor.Trim And _
                                a.FirstCellNumber <= pPosition And a.LastCellNumber >= pPosition _
                                Select a).ToList
                    'Where a.AnalyzerID.ToUpper.Trim = MyClass.AnalyzerId.ToUpper.Trim And _
                    'a.RotorType.ToUpper.Trim = myRotor.Trim And _

                    If myRingRows.Count > 0 Then
                        myRing = CType(myRingRows.First.RingNumber, Rings)
                    End If

                End If

                myGlobalDataTO.SetDatos = myRing

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.GetRotorRingByPosition", EventLogEntryType.Error, False)

            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by SGM 15/12/2012</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then
                    If MyClass.ReadDM1FreqRequested Then
                        ' sample
                        MyClass.ReadDM1FreqRequested = False
                    End If

                    If MyClass.ReadDR1FreqRequested Then
                        ' Reagent 1
                        MyClass.ReadDR1FreqRequested = False
                    End If

                    If MyClass.ReadDR2FreqRequested Then
                        ' Reagent 2
                        MyClass.ReadDR2FreqRequested = False
                    End If

                End If
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Sends POLL instruction for reading the informed Arm related detector's frequency
        ''' </summary>
        ''' <param name="pArm"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 15/12/2012</remarks>
        Public Function REQUEST_FREQUENCY_INFO(ByVal pArm As Arms) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.CurrentOperation = OPERATIONS.READ_FREQUENCY

                Select Case pArm
                    Case Arms.SAMPLE
                        MyClass.ReadDM1FreqRequested = True
                        myResultData = MyBase.SendPOLLHW(POLL_IDs.DM1)

                    Case Arms.REAGENT1
                        MyClass.ReadDR1FreqRequested = True
                        myResultData = MyBase.SendPOLLHW(POLL_IDs.DR1)

                    Case Arms.REAGENT2
                        MyClass.ReadDR2FreqRequested = True
                        myResultData = MyBase.SendPOLLHW(POLL_IDs.DR2)

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.REQUEST_ANALYZER_INFO", EventLogEntryType.Error, False)
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
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Public Function ManageHistoryResults() As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim myHistoryDelegate As New HistoricalReportsDelegate

            Try

                If MyClass.CurrentHistoryArea <> HISTORY_AREAS.NONE Then

                    Dim myHistoricReportDS As New SRVResultsServiceDS
                    Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    Dim myTask As String = "TEST"
                    Dim myAction As String = ""

                    myAction = MyClass.CurrentHistoryArea.ToString.Trim

                    'Fill the data
                    myHistoricReportRow = myHistoricReportDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                    With myHistoricReportRow
                        .BeginEdit()
                        .TaskID = myTask
                        .ActionID = myAction
                        .Data = MyClass.GenerateResultData()
                        .AnalyzerID = MyClass.AnalyzerId
                        .EndEdit()
                    End With

                    'save to history
                    myResultData = myHistoryDelegate.Add(Nothing, myHistoricReportRow)


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
                                MyClass.RecommendationsList = Nothing
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
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.ManageHistoryResults", EventLogEntryType.Error, False)
            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' Decodes the data from DB in order to display in History Screen
        ''' </summary>
        ''' <param name="pTaskID"></param>
        ''' <param name="pAction"></param>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Public Function DecodeDataReport(ByVal pTaskID As String, ByVal pAction As String, ByVal pData As String, ByVal pLanguageId As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                Dim myUtility As New Utilities()
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""

                Dim myLines As List(Of List(Of String))
                Dim FinalText As String = ""

                Dim myArea As HISTORY_AREAS = HISTORY_AREAS.NONE


                'set the area
                Select Case pAction
                    Case "LEVEL_FREQ_READ" : myArea = HISTORY_AREAS.LEVEL_FREQ_READ
                    Case "LEVEL_DET_TEST" : myArea = HISTORY_AREAS.LEVEL_DET_TEST
                End Select

                If myArea <> HISTORY_AREAS.NONE Then
                    'obtain the connection
                    myResultData = DAOBase.GetOpenDBConnection(Nothing)
                    If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myResultData.SetDatos, SqlClient.SqlConnection)
                    End If


                    Select Case myArea
                        Case HISTORY_AREAS.LEVEL_FREQ_READ

                            myLines = New List(Of List(Of String))

                            Dim myLine As List(Of String)
                            Dim myValue1 As String = ""
                            Dim myResult1 As HISTORY_RESULTS
                            Dim myValue2 As String = ""
                            Dim myResult2 As HISTORY_RESULTS
                            Dim myValue3 As String = ""
                            Dim myResult3 As HISTORY_RESULTS

                            myValue1 = MyClass.DecodeHistoryDataValue(pData, myArea, Arms.SAMPLE)
                            myResult1 = MyClass.DecodeHistoryDataResult(pData, myArea, Arms.SAMPLE)
                            myValue2 = MyClass.DecodeHistoryDataValue(pData, myArea, Arms.REAGENT1)
                            myResult2 = MyClass.DecodeHistoryDataResult(pData, myArea, Arms.REAGENT1)
                            myValue3 = MyClass.DecodeHistoryDataValue(pData, myArea, Arms.REAGENT2)
                            myResult3 = MyClass.DecodeHistoryDataResult(pData, myArea, Arms.REAGENT2)

                            Dim myColWidth As Integer = 30

                            '1st LINE (title)
                            '*****************************************************************
                            myLine = New List(Of String)

                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_LEVEL_FREQS", pLanguageId)) & ":"
                            myLine.Add(text1)

                            myLines.Add(myLine)

                            '2nd LINE (Arm titles)
                            '*****************************************************************
                            myLine = New List(Of String)
                            'sample
                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_SAMPLE", pLanguageId) + ":")
                            myLine.Add(text1)

                            'reagent 1
                            text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent1", pLanguageId) + ":")
                            myLine.Add(text2)

                            'reagent 2
                            text3 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent2", pLanguageId) + ":")
                            myLine.Add(text3)

                            myLines.Add(myLine)

                            '3rd LINE (Frequency value)
                            '*****************************************************************
                            'Dim myFreqText As String = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FREQUENCY", pLanguageId) + ": ")
                            myLine = New List(Of String)

                            'sample
                            text1 = "" ' myFreqText
                            text1 &= myValue1 & " " & LevelDetectionTestDelegate.FREQ_UNIT
                            myLine.Add(text1)

                            'reagent 1
                            text2 = "" ' myFreqText
                            text2 &= myValue2 & " " & LevelDetectionTestDelegate.FREQ_UNIT
                            myLine.Add(text2)

                            'reagent 2
                            text3 = "" ' myFreqText
                            text3 &= myValue3 & " " & LevelDetectionTestDelegate.FREQ_UNIT
                            myLine.Add(text3)

                            myLines.Add(myLine)

                            '4th LINE (Frequency Result)
                            '*****************************************************************
                            Dim myFreqOKText As String = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FREQ_IN_LIMITS", pLanguageId))
                            Dim myFreqNOKText As String = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FREQ_OUT_LIMITS", pLanguageId))

                            myLine = New List(Of String)

                            'sample
                            Select Case myResult1
                                Case HISTORY_RESULTS.FREQ_OK : text1 = myFreqOKText
                                Case HISTORY_RESULTS.FREQ_NOK : text1 = myFreqNOKText
                            End Select
                            myLine.Add(text1)

                            'reagent 1
                            Select Case myResult2
                                Case HISTORY_RESULTS.FREQ_OK : text2 = myFreqOKText
                                Case HISTORY_RESULTS.FREQ_NOK : text2 = myFreqNOKText
                            End Select
                            myLine.Add(text2)

                            'reagent 2
                            Select Case myResult3
                                Case HISTORY_RESULTS.FREQ_OK : text3 = myFreqOKText
                                Case HISTORY_RESULTS.FREQ_NOK : text3 = myFreqNOKText
                            End Select
                            myLine.Add(text3)

                            myLines.Add(myLine)

                            For Each Line As List(Of String) In myLines
                                Dim newLine As Boolean = False
                                Select Case myLines.IndexOf(Line) + 1
                                    Case 1 : newLine = True 'title
                                    Case 2 : newLine = False 'Arms titles
                                    Case 3 : newLine = False 'frequencies
                                    Case 4 : newLine = True 'range check result
                                End Select
                                FinalText &= myUtility.FormatLineHistorics(Line, myColWidth, newLine)
                            Next

                            myResultData.SetDatos = FinalText


                        Case HISTORY_AREAS.LEVEL_DET_TEST

                            myLines = New List(Of List(Of String))

                            Dim myLine As List(Of String)

                            Dim myNumArm As String = ""
                            Dim myNumPosition As String = ""
                            Dim myNumRing As String = ""
                            Dim mySelArm As Arms = Arms._NONE
                            Dim mySelPosition As Integer = -1
                            Dim mySelRing As Rings = Rings._NONE


                            Dim myResult As HISTORY_RESULTS

                            myNumArm = pData.Substring(0, 1)
                            If IsNumeric(myNumArm) Then
                                mySelArm = CType(CInt(myNumArm), Arms)
                            End If

                            myNumPosition = pData.Substring(0, 1)
                            If IsNumeric(myNumPosition) Then
                                mySelPosition = CInt(myNumArm)
                            End If

                            myNumRing = pData.Substring(0, 1)
                            If IsNumeric(myNumRing) Then
                                mySelRing = CType(CInt(myNumRing), Rings)
                            End If

                            myResult = MyClass.DecodeHistoryDataResult(pData, myArea, Arms.SAMPLE)
                            
                            Dim myColWidth As Integer = 20

                            '1st LINE (title)
                            '*****************************************************************
                            myLine = New List(Of String)

                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_DETECTION_TEST", pLanguageId)) & ":"
                            myLine.Add(text1)

                            myLines.Add(myLine)

                            '2nd LINE (Selected Arm)
                            '*****************************************************************
                            myLine = New List(Of String)
                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_SelectedArm", pLanguageId)) & ":"
                            myLine.Add(text1)

                            Select Case mySelArm
                                Case Arms.SAMPLE : text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_SAMPLE", pLanguageId))
                                Case Arms.REAGENT1 : text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent1", pLanguageId))
                                Case Arms.REAGENT2 : text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent2", pLanguageId))
                            End Select
                            myLine.Add(text2)

                            myLines.Add(myLine)

                            '3rd LINE (Selected Position)
                            '*****************************************************************
                            myLine = New List(Of String)
                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_POS_SELECTED", pLanguageId)) & ":"
                            myLine.Add(text1)

                            text2 = mySelPosition.ToString & " (" & (MLRD.GetResourceText(dbConnection, "LBL_SRV_RING", pLanguageId)) & " " & mySelRing.ToString & ")"
                            myLine.Add(text2)

                            myLines.Add(myLine)

                            '4th LINE (Result)
                            '*****************************************************************
                            myLine = New List(Of String)
                            text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_DETECTED", pLanguageId)) & ":"
                            myLine.Add(text1)

                            Select Case myResult
                                Case HISTORY_RESULTS.DETECTED
                                    text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_YES", pLanguageId))
                                Case HISTORY_RESULTS.NOT_DETECTED
                                    text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_NO", pLanguageId))
                                Case HISTORY_RESULTS._ERROR
                                    text2 = "Error"
                            End Select

                            myLine.Add(text2)

                            myLines.Add(myLine)

                            For Each Line As List(Of String) In myLines
                                Dim newLine As Boolean = False
                                Select Case myLines.IndexOf(Line) + 1
                                    Case 1 : newLine = True 'title
                                    Case 2 : newLine = False 'Arm
                                    Case 3 : newLine = False 'Position
                                    Case 4 : newLine = True 'result
                                End Select
                                FinalText &= myUtility.FormatLineHistorics(Line, myColWidth, newLine)
                            Next

                            myResultData.SetDatos = FinalText

                    End Select



                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.DecodeDataReport", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection IsNot Nothing Then dbConnection.Close()
            End Try

            Return myResultData

        End Function

#End Region

#Region "Private"

        ''' <summary>
        ''' Reports the History data to DB. Encodes the result data to history
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Private Function GenerateResultData() As String

            Dim myData As String = ""

            Try

                Select Case MyClass.CurrentHistoryArea

                    Case HISTORY_AREAS.LEVEL_FREQ_READ
                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.SampleFrequencyReadResultAttr))
                        myData &= MyClass.EncodeHistoryValue(CInt(MyClass.SampleFrequencyValue))
                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.Reagent1FrequencyReadResultAttr))
                        myData &= MyClass.EncodeHistoryValue(CInt(MyClass.Reagent1FrequencyValue))
                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.Reagent2FrequencyReadResultAttr))
                        myData &= MyClass.EncodeHistoryValue(CInt(MyClass.Reagent2FrequencyValue))

                    Case HISTORY_AREAS.LEVEL_DET_TEST
                        myData &= CInt(MyClass.CurrentArmAttr).ToString
                        myData &= MyClass.CurrentRotorPositionAttr.ToString("000")
                        myData &= CInt(MyClass.CurrentRing).ToString
                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.DetectionTestResultAttr))

                End Select


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.GenerateResultData", EventLogEntryType.Error, False)
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
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.EncodeHistoryResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Value
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Private Function EncodeHistoryValue(ByVal pValue As Integer) As String

            Dim res As String = "xxxx"

            Try
                res = pValue.ToString("0000")

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.EncodeHistoryValue", EventLogEntryType.Error, False)
                res = "xxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Result
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pArea"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataResult(ByVal pData As String, ByVal pArea As HISTORY_AREAS, ByVal pArm As Arms) As HISTORY_RESULTS

            Dim myResult As HISTORY_RESULTS

            Try
                Dim myIndex As Integer = -1

                Select Case pArea
                    Case HISTORY_AREAS.LEVEL_FREQ_READ
                        Select Case pArm
                            Case Arms.SAMPLE : myIndex = 0
                            Case Arms.REAGENT1 : myIndex = 5
                            Case Arms.REAGENT2 : myIndex = 10
                        End Select

                    Case HISTORY_AREAS.LEVEL_DET_TEST
                        myIndex = 5

                End Select
               

                If myIndex >= 0 Then

                    Dim myResultNumber As String = ""


                    myResultNumber = pData.Substring(myIndex, 1)

                    If IsNumeric(myResultNumber) Then
                        myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                    ElseIf myResultNumber = "x" Then
                        myResult = CType(-1, HISTORY_RESULTS)
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return myResult

        End Function

        ''' <summary>
        ''' Decodes History Value
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Private Function DecodeHistoryDataValue(ByVal pData As String, ByVal pArea As HISTORY_AREAS, ByVal pArm As Arms, Optional ByVal pLanguageId As String = "") As String

            Dim res As String = ""

            Try
                Dim myIndex As Integer = -1

                Select Case pArea
                    Case HISTORY_AREAS.LEVEL_FREQ_READ
                        Select Case pArm
                            Case Arms.SAMPLE : myIndex = 1
                            Case Arms.REAGENT1 : myIndex = 6
                            Case Arms.REAGENT2 : myIndex = 11
                        End Select

                    Case HISTORY_AREAS.LEVEL_DET_TEST

                End Select
                

                If myIndex >= 0 Then

                    Dim myDataText As String = ""
                    myDataText = pData.Substring(myIndex, 4)
                    If IsNumeric(myDataText) Then
                        res = CInt(myDataText).ToString
                    End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
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
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Private Function GetResultLanguageResource(ByRef pdbConnection As SqlClient.SqlConnection, ByVal pResult As HISTORY_RESULTS, ByVal pLanguageId As String) As String

            Dim res As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Try
                Select Case pResult
                    Case HISTORY_RESULTS.FREQ_OK
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FREQ_IN_LIMITS", pLanguageId)

                    Case HISTORY_RESULTS.FREQ_NOK
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FREQ_OUT_LIMITS", pLanguageId)

                    Case HISTORY_RESULTS.DETECTED
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_LEVEL_DETECTED", pLanguageId)

                    Case HISTORY_RESULTS.NOT_DETECTED
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_LEVEL_NOT_DETECTED", pLanguageId)

                    Case HISTORY_RESULTS._ERROR
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FAILED", pLanguageId)

                End Select
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.GetResultLanguageResource", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' registering the incidence in historical reports activity
        ''' </summary>
        ''' <remarks>Created by SGM 21/12/2011</remarks>
        Private Sub UpdateRecommendationsList(ByVal pRecommendationID As HISTORY_RECOMMENDATIONS)
            Try
                ' registering the incidence in historical reports activity

                If MyClass.RecommendationsList Is Nothing Then
                    MyClass.RecommendationsList = New List(Of HISTORY_RECOMMENDATIONS)
                End If
                MyClass.RecommendationsList.Add(pRecommendationID)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "LevelDetectionTestDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region


#End Region


    End Class

End Namespace