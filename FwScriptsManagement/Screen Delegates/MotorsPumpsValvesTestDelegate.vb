Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.App

Namespace Biosystems.Ax00.FwScriptsManagement
    Public Class MotorsPumpsValvesTestDelegate
        Inherits BaseFwScriptDelegate

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID)
            myFwScriptDelegate = pFwScriptsDelegate
            MyClass.ReportCountTimeout = 0
        End Sub

        Public Sub New()
            MyBase.New()
            MyClass.ReportCountTimeout = 0
        End Sub
#End Region

#Region "Enumerations"
        Private Enum OPERATIONS
            NONE
            WASHING_STATION_UP
            WASHING_STATION_DOWN
        End Enum
#End Region

#Region "Attributes"
        'Private AnalyzerIdAttr As String = ""
        Private ActivationModeAttr As SWITCH_ACTIONS = SWITCH_ACTIONS.SET_TO_OFF
        Private MovementModeAttr As MOVEMENT = MOVEMENT.HOME
        Private Valve3ActivationModeAttr As SWITCH_ACTIONS = SWITCH_ACTIONS.SET_TO_0_1
        Private ItemAttr As String = ""
        Private ItemGroupAttr As String = ""
        Private ItemTypeAttr As String = ""
        Private CurrentTestAttr As ADJUSTMENT_GROUPS
        Private MotorsPumpsValvesTestAttr As String = ""

        Private IsWashingStationDownAttr As Boolean

        'HISTORY
        '***************************
        Private HistoryTaskAttr As PreloadedMasterDataEnum = Nothing
        Private HistoryAreaAttr As HISTORY_AREAS = HISTORY_AREAS.NONE

        'history elements by area
        Public HistoryInDoElements As New List(Of HISTORY_ELEMENTS)
        Public HistoryExWaElements As New List(Of HISTORY_ELEMENTS)
        Public HistoryWsAspElements As New List(Of HISTORY_ELEMENTS)
        Public HistoryWsDispElements As New List(Of HISTORY_ELEMENTS)
        Public HistoryInOutElements As New List(Of HISTORY_ELEMENTS)

        Public HisIndoValues As New Dictionary(Of HISTORY_ELEMENTS, Integer)
        Public HisIndoSimpleSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisIndoContinuousSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisIndoMovements As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)

        Public HisExWaValues As New Dictionary(Of HISTORY_ELEMENTS, Integer)
        Public HisExWaSimpleSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisExWaContinuousSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisExWaMovements As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)

        Public HisWsAspValues As New Dictionary(Of HISTORY_ELEMENTS, Integer)
        Public HisWsAspSimpleSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisWsAspContinuousSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisWsAspMovements As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)

        Public HisWsDispValues As New Dictionary(Of HISTORY_ELEMENTS, Integer)
        Public HisWsDispSimpleSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisWsDispContinuousSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisWsDispMovements As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)

        Public HisInOutValues As New Dictionary(Of HISTORY_ELEMENTS, Integer)
        Public HisInOutSimpleSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisInOutContinuousSwitches As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
        Public HisInOutMovements As New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)

        Private HisSubsystem As HISTORY_SUBSYSTEMS = HISTORY_SUBSYSTEMS.NONE
        Public HisElementType As HISTORY_ITEM_TYPES = HISTORY_ITEM_TYPES.NONE

        'Private HisSubsystemAttr As HISTORY_SUBSYSTEMS = HISTORY_SUBSYSTEMS.NONE
        'Private HisElementTypeAttr As HISTORY_ITEM_TYPES = HISTORY_ITEM_TYPES.NONE
        'Private HisElementAttr As HISTORY_ELEMENTS = HISTORY_ELEMENTS.NONE
        'Private HisElementValueAttr As Integer = -1
        'Private HisTestActionAttr As HISTORY_TEST_ACTIONS = HISTORY_TEST_ACTIONS.NONE

        'Private HisActionDeniedAttr As HISTORY_DENIED_ACTION = HISTORY_DENIED_ACTION.NONE

        Private HisAditionalInfoAttr As HISTORY_ADITIONAL_INFO = HISTORY_ADITIONAL_INFO.NONE

        Private HisTestActionResultAttr As HISTORY_RESULTS = HISTORY_RESULTS.NOT_DONE
        Private HisCollisionsAttr As New Dictionary(Of HISTORY_COLLISIONS, Boolean)
        Private HisEncoderLostStepsAttr As Integer = -1

        Private HomesDoneAttr As Boolean = False 'SGM 08/11/2011

        ' XBC 09/11/2011
        Private ReadAnalyzerInfoDoneAttr As Boolean
        Private ReadGLFInfoDoneAttr As Boolean
        Private ReadJEXInfoDoneAttr As Boolean
        Private ReadSFXInfoDoneAttr As Boolean
        Private MaxPOLLHWAttr As Integer = 3
        Private NumPOLLHWAttr As Integer
        ' XBC 09/11/2011
#End Region

#Region "Arm Positions Arguments"

        'washing
        Public ArmSamplesWashingPolar As Integer = -1
        Public ArmSamplesWashingZ As Integer = -1
        Public ArmReagent1WashingPolar As Integer = -1
        Public ArmReagent1WashingZ As Integer = -1
        Public ArmReagent2WashingPolar As Integer = -1
        Public ArmReagent2WashingZ As Integer = -1
        Public ArmMixer1WashingPolar As Integer = -1
        Public ArmMixer1WashingZ As Integer = -1
        Public ArmMixer2WashingPolar As Integer = -1
        Public ArmMixer2WashingZ As Integer = -1

        ' XBC 09/11/2011
        'parking
        'Public ArmSamplesParkingPolar As Integer = -1
        'Public ArmSamplesParkingZ As Integer = -1
        'Public ArmReagent1ParkingPolar As Integer = -1
        'Public ArmReagent1ParkingZ As Integer = -1
        'Public ArmReagent2ParkingPolar As Integer = -1
        'Public ArmReagent2ParkingZ As Integer = -1
        'Public ArmMixer1ParkingPolar As Integer = -1
        'Public ArmMixer1ParkingZ As Integer = -1
        'Public ArmMixer2ParkingPolar As Integer = -1
        'Public ArmMixer2ParkingZ As Integer = -1
        Public ArmSamplesParkingPolar As Integer = -1
        Public ArmSamplesParkingZ As Integer = -1
        Public ArmSamplesSecurityZ As Integer = -1

        Public ArmReagent1ParkingPolar As Integer = -1
        Public ArmReagent1SecurityZ As Integer = -1
        Public ArmReagent2ParkingPolar As Integer = -1
        Public ArmReagent2SecurityZ As Integer = -1
        Public ArmMixer1ParkingPolar As Integer = -1
        Public ArmMixer1ParkingZ As Integer = -1
        Public ArmMixer1SecurityZ As Integer = -1
        Public ArmMixer2ParkingPolar As Integer = -1
        Public ArmMixer2SecurityZ As Integer = -1
        ' XBC 09/11/2011

        'SGM 21/11/2011
        Public WSReadyRefPos As Integer = -1
        Public WSFinalPos As Integer = -1
        Public WSReferencePos As Integer = -1
        Private WSOffset As Integer = -1
        'SGM 21/11/2011


        ' XBC 08/11/2011
        ''motors
        'Public SamplesMotorDefaultPosition As Integer = -1
        'Public Reagent1MotorDefaultPosition As Integer = -1
        'Public Reagent2MotorDefaultPosition As Integer = -1
        'Public DispensationMotorDefaultPosition As Integer = -1
        'Public ReactionsRotorMotorDefaultPosition As Integer = -1
        'Public WashingStationMotorDefaultPosition As Integer = -1
        ' XBC 08/11/2011

        Public WashingPositioning As Washing_Positioning = Washing_Positioning._NONE

        Public EncoderTestRelativePositions As New List(Of Integer)

        'pumps
        Public PumpProgressiveStart As Integer = 1
        Public PumpDutyValue As Integer = 100
        ' XBC 09/07/2012
        Public PumpDutyValueSamplesExt As Integer
        Public PumpDutyValueSamplesInt As Integer

#End Region

#Region "Arms Positioning"
        Public RelativePositioning As Integer = 0
        Public AbsolutePositioning As Integer = 0
#End Region

#Region "Public Enumerates"

        Public Enum Washing_Positioning
            _NONE
            REAGENT1_TO_WASH
            REAGENT2_TO_WASH
            MIXER1_TO_WASH
            MIXER2_TO_WASH
        End Enum

        Public Enum HISTORY_RESULTS
            _ERROR = -1
            NOT_DONE = 0
            OK = 1
            NOK = 2
            CANCEL = 3
            NOT_APPLIED = 4
        End Enum

        Public Enum HISTORY_AREAS
            NONE = 0
            INTERNAL_DOSING = 1
            EXT_WASHING = 2
            WS_ASPIRATION = 3
            WS_DISPENSATION = 4
            IN_OUT = 5
            COLLISION = 6
            ENCODER = 7

        End Enum

        Public Enum HISTORY_SUBSYSTEMS
            NONE = 0
            MANIFOLD = 1
            FLUIDICS = 2
            PHOTOMETRICS = 3
        End Enum

        Public Enum HISTORY_ITEM_TYPES
            NONE = 0
            PUMP = 1
            EVALVE = 2
            EVALVE3 = 3
            DCMOTOR = 4
            DCPUMP = 5
        End Enum

        'Public Enum HISTORY_TEST_ACTIONS
        '    NONE = 0
        '    SWITCH_ON = 1
        '    SWITCH_OFF = 2
        '    SWITCH_CONTINUOUS = 3
        '    DC_MOVE = 4
        'End Enum

        Public Enum HISTORY_ELEMENTS

            NONE = 0

            'Reagent1 motor
            JE1_MR1A = 1
            'Reagent2 motor
            JE1_MR2A = 2
            'Samples motor
            JE1_MSA = 3
            'Samples Dosing pump
            JE1_B1 = 4
            'Reagent1 Dosing pump
            JE1_B2 = 5
            'Reagent2 Dosing pump
            JE1_B3 = 6
            'Samples Dosing valve
            JE1_EV1 = 7
            'Reagent1 Dosing valve
            JE1_EV2 = 8
            'Reagent2 Dosing valve
            JE1_EV3 = 9
            'Air OR Washing Solution Valve
            JE1_EV4 = 10
            'Air/Washing OR Purified Water Solution Valve
            JE1_EV5 = 11

            SF1_MSA = 21
            'Samples Arm
            SF1_B1 = 22
            'Reagent1/Mixer2 Arm
            SF1_B2 = 23
            'Reagent2/Mixer1 Arm
            SF1_B3 = 24
            'Purified Water (from external tank)
            SF1_B4 = 25 'state (dc)
            'Low Contamination
            SF1_B5 = 26
            'Washing Station needle 2,3
            SF1_B6 = 27
            'Washing Station needle 4,5
            SF1_B7 = 28
            'Washing Station needle 6
            SF1_B8 = 29
            'Washing Station needle 7
            SF1_B9 = 30
            'Washing Station needle 1
            SF1_B10 = 31
            'Electrovalves pump
            SF1_GE1 = 32
            'Purified Water (from external source)
            SF1_EV1 = 33
            'Purified Water (from external tank)
            SF1_EV2 = 34 'state (bool)
            'Reactions Rotor Motor
            GLF_MRA = 41
            GLF_MRE = 42 'encoder position
            'Washing Station Vertical Motor (Pinta)
            GLF_MWA = 43
            'Washing Station Collision Detector
            GLF_CD = 44

        End Enum

        Public Enum HISTORY_DENIED_ACTION
            NONE = 0
            LC_TANK_EMPTY = 1
            DW_TANK_FULL = 2
            PUMPOFF_BEFORE_VALVEOFF = 3
        End Enum

        Public Enum HISTORY_ADITIONAL_INFO
            NONE = 0
            DOSING_WS = 1
            DOSING_DW = 2
            DOSING_AIR = 3
        End Enum

        Public Enum HISTORY_COLLISIONS
            NONE = 0
            WASHING_STATION = 1
            SAMPLE = 2
            REAGENT1 = 3
            REAGENT2 = 4

        End Enum


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

        Public Property Item() As String
            Get
                Return ItemAttr
            End Get
            Set(ByVal value As String)
                ItemAttr = value
            End Set
        End Property
        Public Property ItemGroup() As String
            Get
                Return ItemGroupAttr
            End Get
            Set(ByVal value As String)
                ItemGroupAttr = value
            End Set
        End Property
        Public Property ItemType() As String
            Get
                Return ItemTypeAttr
            End Get
            Set(ByVal value As String)
                ItemTypeAttr = value
            End Set
        End Property

        Public Property ActivationMode() As SWITCH_ACTIONS
            Get
                Return Me.ActivationModeAttr
            End Get
            Set(ByVal value As SWITCH_ACTIONS)
                Me.ActivationModeAttr = value
            End Set
        End Property

        Public Property MovementMode() As MOVEMENT
            Get
                Return Me.MovementModeAttr
            End Get
            Set(ByVal value As MOVEMENT)
                Me.MovementModeAttr = value
            End Set
        End Property

        Public Property Valve3ActivationMode() As SWITCH_ACTIONS
            Get
                Return Me.Valve3ActivationModeAttr
            End Get
            Set(ByVal value As SWITCH_ACTIONS)
                Me.Valve3ActivationModeAttr = value
            End Set
        End Property

        '**************HISTORY********************ç

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

        'Public Property HisSubsystem() As HISTORY_SUBSYSTEMS
        '    Get
        '        Return HisSubsystemAttr
        '    End Get
        '    Set(ByVal value As HISTORY_SUBSYSTEMS)
        '        HisSubsystemAttr = value
        '    End Set
        'End Property

        'Public Property HisElementType() As HISTORY_ITEM_TYPES
        '    Get
        '        Return HisElementTypeAttr
        '    End Get
        '    Set(ByVal value As HISTORY_ITEM_TYPES)
        '        HisElementTypeAttr = value
        '    End Set
        'End Property

        'Public Property HisTestAction() As HISTORY_TEST_ACTIONS
        '    Get
        '        Return HisTestActionAttr
        '    End Get
        '    Set(ByVal value As HISTORY_TEST_ACTIONS)
        '        HisTestActionAttr = value
        '    End Set
        'End Property

        'Public Property HisElement() As HISTORY_ELEMENTS
        '    Get
        '        Return HisElementAttr
        '    End Get
        '    Set(ByVal value As HISTORY_ELEMENTS)
        '        HisElementAttr = value
        '    End Set
        'End Property

        'Public Property HisElementValue() As Integer
        '    Get
        '        Return HisElementValueAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        HisElementValueAttr = value
        '    End Set
        'End Property

        Public Property HisTestActionResult() As HISTORY_RESULTS
            Get
                Return HisTestActionResultAttr
            End Get
            Set(ByVal value As HISTORY_RESULTS)
                HisTestActionResultAttr = value
            End Set
        End Property

        'Public Property HisActionDenied() As HISTORY_DENIED_ACTION
        '    Get
        '        Return HisActionDeniedAttr
        '    End Get
        '    Set(ByVal value As HISTORY_DENIED_ACTION)
        '        HisActionDeniedAttr = value
        '    End Set
        'End Property

        Public Property HisAditionalInfo() As HISTORY_ADITIONAL_INFO
            Get
                Return HisAditionalInfoAttr
            End Get
            Set(ByVal value As HISTORY_ADITIONAL_INFO)
                HisAditionalInfoAttr = value
            End Set
        End Property

        Public Property HisCollisions() As Dictionary(Of HISTORY_COLLISIONS, Boolean)
            Get
                Return HisCollisionsAttr
            End Get
            Set(ByVal value As Dictionary(Of HISTORY_COLLISIONS, Boolean))
                HisCollisionsAttr = value
            End Set
        End Property

        Public Property HisEncoderLostSteps() As Integer
            Get
                Return HisEncoderLostStepsAttr
            End Get
            Set(ByVal value As Integer)
                HisEncoderLostStepsAttr = value
            End Set
        End Property

        'SGM 08/11/2011
        Public Property HomesDone() As Boolean
            Get
                Return HomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                HomesDoneAttr = value
            End Set
        End Property

        ' XBC 09/11/2011
        Public Property ReadAnalyzerInfoDone() As Boolean
            Get
                Return MyClass.ReadAnalyzerInfoDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadAnalyzerInfoDoneAttr = value
            End Set
        End Property

        Public Property ReadGLFInfoDone() As Boolean
            Get
                Return MyClass.ReadGLFInfoDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadGLFInfoDoneAttr = value
            End Set
        End Property

        Public Property ReadJEXInfoDone() As Boolean
            Get
                Return MyClass.ReadJEXInfoDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadJEXInfoDoneAttr = value
            End Set
        End Property

        Public Property ReadSFXInfoDone() As Boolean
            Get
                Return MyClass.ReadSFXInfoDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReadSFXInfoDoneAttr = value
            End Set
        End Property

        Public ReadOnly Property MaxPOLLHW() As Integer
            Get
                Return Me.MaxPOLLHWAttr
            End Get
        End Property

        Public Property NumPOLLHW() As Integer
            Get
                Return Me.NumPOLLHWAttr
            End Get
            Set(ByVal value As Integer)
                Me.NumPOLLHWAttr = value
            End Set
        End Property
        ' XBC 09/11/2011

        'SGM 21/05/2012
        Public Property IsWashingStationDown() As Boolean
            Get
                Return MyClass.IsWashingStationDownAttr
            End Get
            Set(ByVal value As Boolean)
                If MyClass.IsWashingStationDownAttr <> value Then
                    MyClass.IsWashingStationDownAttr = value
                End If
            End Set
        End Property

#End Region

#Region "Declarations"
        Private CurrentOperation As OPERATIONS
        Private RecommendationsList As New List(Of HISTORY_RECOMMENDATIONS)
        Private ReportCountTimeout As Integer
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  XB 19/04/11
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment

                Debug.Print("Send Script for current mode: " & pMode.ToString)

                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()

                    Case ADJUSTMENT_MODES.HOME_PREPARING
                        myResultData = MyClass.SendQueueForALLARMSHOMING(ADJUSTMENT_GROUPS.MOTORS_PUMPS_VALVES)
                        'myResultData = MyBase.SendQueueForHOMING(ADJUSTMENT_GROUPS.MOTORS_PUMPS_VALVES)

                    Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_WASHING
                        myResultData = MyClass.SendQueueForALLARMSTOWASHING

                    Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_DOWN
                        myResultData = MyClass.SendQueueForWASHING_STATION_DOWN

                    Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_UP
                        myResultData = MyClass.SendQueueForWASHING_STATION_UP

                    Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_PARKING
                        myResultData = MyClass.SendQueueForALLARMSTOPARKING

                    Case ADJUSTMENT_MODES.TESTING

                        'pump activation parameters
                        Dim myParamList As New List(Of String)

                        'Select Case MyClass.ItemType.ToUpper.Trim
                        Select Case MyClass.ItemType.Trim

                            Case "VALVE", "VALVE3"
                                myParamList = Nothing

                            Case "PUMP"
                                If MyClass.PumpDutyValue >= 0 And MyClass.PumpProgressiveStart >= 0 Then

                                    ' XBC 10/07/2012
                                    'Select Case MyClass.Item.ToUpper.ToString
                                    Select Case MyClass.Item.ToString
                                        Case FLUIDICS_ELEMENTS.SF1_B1.ToString
                                            ' Samples needle pump duty external washing
                                            myParamList.Add(MyClass.PumpDutyValueSamplesExt.ToString)
                                            myParamList.Add(MyClass.PumpProgressiveStart.ToString)
                                        Case MANIFOLD_ELEMENTS.JE1_B1.ToString
                                            ' Samples needle pump duty internal washing
                                            myParamList.Add(MyClass.PumpDutyValueSamplesInt.ToString)
                                            myParamList.Add(MyClass.PumpProgressiveStart.ToString)
                                        Case Else
                                            ' Generic duty for the rest of the tests
                                            myParamList.Add(MyClass.PumpDutyValue.ToString)
                                            myParamList.Add(MyClass.PumpProgressiveStart.ToString)
                                    End Select

                                    'myParamList.Add(MyClass.PumpDutyValue.ToString)
                                    'myParamList.Add(MyClass.PumpProgressiveStart.ToString)
                                    ' XBC 10/07/2012
                                End If

                            Case "MOTOR"
                                Select Case Me.MovementMode
                                    Case MOVEMENT.HOME
                                        myParamList = Nothing

                                    Case MOVEMENT.ABSOLUTE
                                        myParamList.Add(MyClass.AbsolutePositioning.ToString)

                                    Case MOVEMENT.RELATIVE
                                        myParamList.Add(MyClass.RelativePositioning.ToString)

                                End Select

                        End Select

                        'Select Case MyClass.ItemGroup.ToUpper.Trim
                        Select Case MyClass.ItemGroup.Trim
                            Case "MANIFOLD"
                                myResultData = Me.SendQueueForManifoldTESTING(MyClass.Item, myParamList)

                            Case "FLUIDICS"
                                myResultData = Me.SendQueueForFluidicsTESTING(MyClass.Item, myParamList)

                            Case "PHOTOMETRICS"
                                myResultData = Me.SendQueueForPhotometricsTESTING(MyClass.Item, myParamList)

                        End Select

                    Case ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                        myResultData = MyClass.SendQueueForSWITCHINGOFF

                    Case ADJUSTMENT_MODES.MBEV_ARM_TO_WASHING
                        myResultData = MyClass.SendQueueForARMTOWASHING
                    Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_EMPTY_WELLS
                        myResultData = MyClass.SendQueueParameterLessScript(FwSCRIPTS_IDS.ACTIVE_ALL_ASPIRATION_PUMPS.ToString)
                    Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_STOP_EMPTY_WELLS
                        myResultData = MyClass.SendQueueParameterLessScript(FwSCRIPTS_IDS.DEACTIVE_ALL_ASPIRATION_PUMPS.ToString)
                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 09/11/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing) 'SGM 21/05/2012

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED) Then
                    ' PHOTOMETRICS VALUES
                    Me.ReadGLFInfoDoneAttr = True
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.MANIFOLDVALUE_CHANGED) Then
                    ' MANIFOLD VALUES
                    Me.ReadJEXInfoDoneAttr = True
                End If

                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.FLUIDICSVALUE_CHANGED) Then
                    ' FLUIDICS VALUES
                    Me.ReadSFXInfoDoneAttr = True
                End If

                If Me.ReadGLFInfoDoneAttr And _
                   Me.ReadJEXInfoDoneAttr And _
                   Me.ReadSFXInfoDoneAttr Then

                    ' With all Analyzer HW information readed...
                    Me.ReadAnalyzerInfoDoneAttr = True
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' High Level Instruction to read Hardware information from the instrument 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 09/11/2011</remarks>
        Public Function REQUEST_ANALYZER_INFO() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' PHOTOMETRICS
                If Not ReadGLFInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.GLF)

                    ' MANIFOLD
                ElseIf Not ReadJEXInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.JE1)

                    ' FLUIDICS
                ElseIf Not ReadSFXInfoDoneAttr Then
                    Me.NumPOLLHWAttr += 1
                    myResultData = MyBase.SendPOLLHW(POLL_IDs.SF1)

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.REQUEST_ANALYZER_INFO", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Manages the Washing Station (copied from Positions)
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/05/2012</remarks>
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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendWASH_STATION_CTRL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' NRotor High Level Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004</remarks>
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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendNEW_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function




#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Get the Positioning Adj Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by : SGM 21/11/2011</remarks>
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
                    MyClass.WSOffset = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' XBC 09/07/2012
                ' Samples needle pump duty external washing
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_S_NEEDLE_PUMP_DUTY_EXT.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.PumpDutyValueSamplesExt = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' XBC 09/07/2012
                ' Samples needle pump duty internal washing
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_S_NEEDLE_PUMP_DUTY_INT.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.PumpDutyValueSamplesInt = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for all arms to parking operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/05/11</remarks>
        Private Function SendQueueForALLARMSTOPARKING() As GlobalDataTO
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
                    .FwScriptID = FwSCRIPTS_IDS.ALL_ARMS_TO_PARK.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    'expects 10 params
                    .ParamList = New List(Of String)


                    If AnalyzerController.Instance.IsBA200 Then
                        .ParamList.Add(Me.ArmSamplesSecurityZ.ToString)
                        .ParamList.Add(Me.ArmMixer1SecurityZ.ToString)

                        .ParamList.Add(Me.ArmSamplesParkingPolar.ToString)
                        .ParamList.Add(Me.ArmMixer1ParkingPolar.ToString)

                        .ParamList.Add(Me.ArmSamplesParkingZ.ToString)
                        .ParamList.Add(Me.ArmMixer1ParkingZ.ToString)
                    Else
                        ' XBC 09/11/2011
                        .ParamList.Add(Me.ArmSamplesSecurityZ.ToString)
                        .ParamList.Add(Me.ArmReagent1SecurityZ.ToString)
                        .ParamList.Add(Me.ArmReagent2SecurityZ.ToString)
                        .ParamList.Add(Me.ArmMixer1SecurityZ.ToString)
                        .ParamList.Add(Me.ArmMixer2SecurityZ.ToString)

                        .ParamList.Add(Me.ArmSamplesParkingPolar.ToString)
                        .ParamList.Add(Me.ArmReagent1ParkingPolar.ToString)
                        .ParamList.Add(Me.ArmReagent2ParkingPolar.ToString)
                        .ParamList.Add(Me.ArmMixer1ParkingPolar.ToString)
                        .ParamList.Add(Me.ArmMixer2ParkingPolar.ToString)
                        ' XBC 09/11/2011
                    End If


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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForALLARMSTOPARKING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Private Function SendQueueForALLARMSHOMING(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            'Dim myFwScript1 As New FwScriptQueueItem
            Try
                MyClass.HomesDoneAttr = False

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'sg 25/1/11
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
                                .NextOnResultOK = Nothing
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = Nothing
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
                    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                    'Else
                    '    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForALLARMSHOMING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Private Function SendQueueParameterLessScript(ScriptName As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                With myFwScript1
                    .FwScriptID = ScriptName
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = Nothing
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

                GlobalBase.CreateLogActivity(ex)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List for all arms to washing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/05/11</remarks>
        Private Function SendQueueForALLARMSTOWASHING() As GlobalDataTO
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
                    .FwScriptID = FwSCRIPTS_IDS.ALL_ARMS_TO_WASH.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    'expects 6 params on ba400 and 2 params on ba200
                    .ParamList = New List(Of String)

                    .ParamList.Add(MyClass.ArmSamplesWashingPolar.ToString)
                    .ParamList.Add(MyClass.ArmSamplesWashingZ.ToString)

                    If Not AnalyzerController.Instance.IsBA200() Then
                        .ParamList.Add(MyClass.ArmReagent1WashingPolar.ToString)
                        .ParamList.Add(MyClass.ArmReagent1WashingZ.ToString)

                        .ParamList.Add(MyClass.ArmReagent2WashingPolar.ToString)
                        .ParamList.Add(MyClass.ArmReagent2WashingZ.ToString)
                    End If

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

                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForALLARMSTOWASHING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List for items switching off operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/05/11</remarks>
        Private Function SendQueueForSWITCHINGOFF() As GlobalDataTO
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
                    .FwScriptID = FwSCRIPTS_IDS.ALL_ITEMS_TO_DEFAULT.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                        ' XBC 08/11/2011 
                        ''expects 6 params
                        '.ParamList = New List(Of String)
                        '.ParamList.Add(MyClass.SamplesMotorDefaultPosition.ToString)
                        '.ParamList.Add(MyClass.Reagent1MotorDefaultPosition.ToString)
                        '.ParamList.Add(MyClass.Reagent2MotorDefaultPosition.ToString)
                        '.ParamList.Add(MyClass.DispensationMotorDefaultPosition.ToString)
                        '.ParamList.Add(MyClass.ReactionsRotorMotorDefaultPosition.ToString)
                        '.ParamList.Add(MyClass.WashingStationMotorDefaultPosition.ToString)
                        .ParamList = Nothing
                        ' XBC 08/11/2011 
                  
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForSWITCHINGOFF", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for positioning an arm to washing operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 31/05/11</remarks>
        Private Function SendQueueForARMTOWASHING() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' XBC 09/11/2011 - PENDING TO STUDY !!!
                '' Arm Absolute Pos-Down 
                '' Move absolute position the current arm:  predefined approaching position (steps)
                'With myFwScript1
                '    Select Case MyClass.WashingPositioning
                '        Case Washing_Positioning.REAGENT1_TO_WASH
                '            .FwScriptID = FwSCRIPTS_IDS.R1_ARM_TO_WASH.ToString
                '        Case Washing_Positioning.REAGENT2_TO_WASH
                '            .FwScriptID = FwSCRIPTS_IDS.R2_ARM_TO_WASH.ToString
                '        Case Washing_Positioning.MIXER1_TO_WASH
                '            .FwScriptID = FwSCRIPTS_IDS.A1_ARM_TO_WASH.ToString
                '        Case Washing_Positioning.MIXER2_TO_WASH
                '            .FwScriptID = FwSCRIPTS_IDS.A2_ARM_TO_WASH.ToString

                '    End Select

                '    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                '    .EvaluateValue = 1
                '    .NextOnResultOK = Nothing
                '    .NextOnResultNG = Nothing
                '    .NextOnTimeOut = Nothing
                '    .NextOnError = Nothing

                '    'expects 4 params
                '    .ParamList = New List(Of String)

                '    Select Case MyClass.WashingPositioning
                '        Case Washing_Positioning.REAGENT1_TO_WASH
                '            .ParamList.Add(MyClass.ArmMixer2ParkingZ.ToString)
                '            .ParamList.Add(MyClass.ArmMixer2ParkingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmReagent1WashingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmReagent1WashingZ.ToString)

                '        Case Washing_Positioning.REAGENT2_TO_WASH
                '            .ParamList.Add(MyClass.ArmMixer1ParkingZ.ToString)
                '            .ParamList.Add(MyClass.ArmMixer1ParkingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmReagent2WashingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmReagent2WashingZ.ToString)

                '        Case Washing_Positioning.MIXER1_TO_WASH
                '            .ParamList.Add(MyClass.ArmReagent2ParkingZ.ToString)
                '            .ParamList.Add(MyClass.ArmReagent2ParkingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmMixer1WashingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmMixer1WashingZ.ToString)

                '        Case Washing_Positioning.MIXER2_TO_WASH
                '            .ParamList.Add(MyClass.ArmReagent1ParkingZ.ToString)
                '            .ParamList.Add(MyClass.ArmReagent1ParkingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmMixer2WashingPolar.ToString)
                '            .ParamList.Add(MyClass.ArmMixer2WashingZ.ToString)

                '    End Select


                'End With

                ''add to the queue list
                'If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForSWITCHINGOFF", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Sends the script for moving down the Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/11/2011</remarks>
        Private Function SendQueueForWASHING_STATION_DOWN() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                If Not myResultData.HasError Then
                    With myFwScript1
                        .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_ABS_Z.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing

                        .ParamList = New List(Of String)
                        '.ParamList.Add(MyClass.WSReadyRefPos.ToString)
                        .ParamList.Add(MyClass.WSFinalPos.ToString)
                        '.ParamList.Add(MyClass.WSReferencePos.ToString)

                    End With

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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForWASHING_STATION_DOWN", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Sends the script for moving down the Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 21/11/2011</remarks>
        Private Function SendQueueForWASHING_STATION_UP() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myListFwScript As New List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                If Not myResultData.HasError Then
                    With myFwScript1
                        .FwScriptID = FwSCRIPTS_IDS.WASHING_STATION_ABS_Z.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing

                        .ParamList = New List(Of String)
                        '.ParamList.Add(MyClass.WSReadyRefPos.ToString)
                        .ParamList.Add(MyClass.WSReferencePos.ToString)
                        '.ParamList.Add(MyClass.WSReferencePos.ToString)

                    End With

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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForWASHING_STATION_UP", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List for Screen Testing operation for Manifold
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 25/05/2011
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function SendQueueForManifoldTESTING(ByVal pElement As String, Optional ByVal pParamList As List(Of String) = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Switch commutator Test 
                With myFwScript1
                    'Select Case pElement.ToUpper.Trim
                    Select Case pElement.Trim

                        Case MANIFOLD_ELEMENTS.JE1_MS.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_ABS.ToString

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_MR1.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR1_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR1_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR1_ABS.ToString

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_MR2.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR2_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR2_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MR2_ABS.ToString

                            End Select


                        Case MANIFOLD_ELEMENTS.JE1_B1.ToString
                            ' sample 
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B1_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B1_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_B2.ToString
                            ' sample 
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B2_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B2_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_B3.ToString
                            ' sample 
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B3_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_B3_OFF.ToString
                                    pParamList = Nothing

                            End Select


                        Case MANIFOLD_ELEMENTS.JE1_EV1.ToString
                            ' sample 
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV1_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV1_OFF.ToString

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_EV2.ToString
                            ' sample 
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV2_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV2_OFF.ToString

                            End Select

                        Case MANIFOLD_ELEMENTS.JE1_EV3.ToString
                            ' sample 
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV3_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV3_OFF.ToString

                            End Select


                        Case MANIFOLD_ELEMENTS.JE1_EV4.ToString

                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV4_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV4_OFF.ToString

                            End Select


                        Case MANIFOLD_ELEMENTS.JE1_EV5.ToString

                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV5_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_EV5_OFF.ToString

                            End Select


                    End Select

                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = pParamList

                End With

                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForManifoldTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List for Screen Testing operation for Fluidics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 25/05/2011
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function SendQueueForFluidicsTESTING(ByVal pElement As String, Optional ByVal pParamList As List(Of String) = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Switch commutator Test 
                With myFwScript1

                    'INTERNAL DOSING
                    '**************************************************************************************

                    'Select Case pElement.ToUpper.Trim
                    Select Case pElement.Trim

                        Case FLUIDICS_ELEMENTS.SF1_MS.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_MS_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_MS_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_MS_ABS.ToString

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B1.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B1_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B1_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B2.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B2_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B2_OFF.ToString
                                    pParamList = Nothing

                            End Select


                        Case FLUIDICS_ELEMENTS.SF1_B3.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B3_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B3_OFF.ToString
                                    pParamList = Nothing

                            End Select


                        Case FLUIDICS_ELEMENTS.SF1_B4.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B4_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B4_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B5.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B5_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B5_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B6.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B6_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B6_OFF.ToString
                                    pParamList = Nothing
                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B7.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B7_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B7_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B8.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B8_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B8_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B9.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B9_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B9_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_B10.ToString
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B10_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_B10_OFF.ToString
                                    pParamList = Nothing

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_EV1.ToString
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV1_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV1_OFF.ToString


                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_EV2.ToString
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV2_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV2_OFF.ToString

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_EV3.ToString
                            pParamList = Nothing
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV3_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_EV3_OFF.ToString

                            End Select

                        Case FLUIDICS_ELEMENTS.SF1_GE1.ToString
                            ' sample 
                            Select Case Me.ActivationMode
                                Case SWITCH_ACTIONS.SET_TO_ON
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_GE1_ON.ToString

                                Case SWITCH_ACTIONS.SET_TO_OFF
                                    .FwScriptID = FwSCRIPTS_IDS.SF1_GE1_OFF.ToString
                                    pParamList = Nothing

                            End Select


                    End Select
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = pParamList
                End With

                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForFluidicsTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List for Screen Testing operation for Photometrics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 25/05/2011
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Private Function SendQueueForPhotometricsTESTING(ByVal pElement As String, Optional ByVal pParamList As List(Of String) = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                ' Switch commutator Test 
                With myFwScript1

                    'INTERNAL DOSING
                    '**************************************************************************************

                    'Select Case pElement.ToUpper.Trim
                    Select Case pElement.Trim

                        Case PHOTOMETRICS_ELEMENTS.GLF_MR.ToString
                            Select Case Me.MovementMode
                                ' XBC 10/11/2011 - repeated ! 
                                'Case MOVEMENT.HOME
                                '    .FwScriptID = FwSCRIPTS_IDS.GLF_MR_HOME.ToString
                                '    .ParamList = Nothing

                                'Case MOVEMENT.RELATIVE
                                '    .FwScriptID = FwSCRIPTS_IDS.GLF_MR_REL.ToString

                                'Case MOVEMENT.ABSOLUTE
                                '    .FwScriptID = FwSCRIPTS_IDS.GLF_MR_ABS.ToString

                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_REL_ROTOR.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString
                                    ' XBC 10/11/2011 - repeated ! 


                            End Select


                        Case PHOTOMETRICS_ELEMENTS.GLF_MW.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.GLF_MW_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE
                                    .FwScriptID = FwSCRIPTS_IDS.GLF_MW_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.GLF_MW_ABS.ToString

                            End Select

                    End Select

                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    .ParamList = pParamList
                End With

                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForPhotometricsTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Request for Encoder Motor testing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 06/06/11</remarks>
        Private Function SendQueueForMOTORTESTING(ByVal pElement As String, Optional ByVal pParamList As List(Of String) = Nothing) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
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

                    Select Case pElement

                        Case MANIFOLD_ELEMENTS.JE1_MS.ToString
                            Select Case Me.MovementMode
                                Case MOVEMENT.HOME
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_HOME.ToString
                                    .ParamList = Nothing

                                Case MOVEMENT.RELATIVE, MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_REL.ToString

                                Case MOVEMENT.ABSOLUTE
                                    .FwScriptID = FwSCRIPTS_IDS.JE1_MS_ABS.ToString

                            End Select


                    End Select



                End With


            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendQueueForMOTORTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

#Region "Collision Test"


#End Region

#Region "Encoder Test"

        ''' <summary>
        ''' Request for Encoder Test - NOT FOR VERSION 1.0
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 23/05/2011</remarks>
        Public Function SendENCODER_TEST() As GlobalDataTO
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
                    .FwScriptID = FwSCRIPTS_IDS.TEST_ENCODER.ToString
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing
                    'expects 10 params
                    .ParamList = New List(Of String)

                    .ParamList.Add(MyClass.EncoderTestRelativePositions(0).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(1).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(2).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(3).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(4).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(5).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(6).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(7).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(8).ToString)
                    .ParamList.Add(MyClass.EncoderTestRelativePositions(9).ToString)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.SendENCODER_TEST", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


#End Region

#End Region

#Region "High level Instructions"

        ''' <summary>
        ''' Manages High level Instruction responses
        ''' </summary>
        ''' <param name="pResponse"></param>
        ''' <param name="pData"></param>
        ''' <remarks>Created by SGM 21/05/2012</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As [Global].GlobalEnumerates.RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
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
                    MyClass.CurrentOperation = OPERATIONS.NONE
                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions

                Select Case CurrentOperation

                    ' XBC 30/11/2011
                    Case OPERATIONS.WASHING_STATION_UP
                        Select Case pResponse
                            Case RESPONSE_TYPES.START
                                ' Nothing by now
                            Case RESPONSE_TYPES.OK
                                MyClass.IsWashingStationDownAttr = False
                                MyClass.CurrentOperation = OPERATIONS.NONE

                        End Select

                        '    ' XBC 30/11/2011
                        'Case OPERATIONS.WASHING_STATION_DOWN
                        '    Select Case pResponse
                        '        Case RESPONSE_TYPES.START
                        '            ' Nothing by now
                        '        Case RESPONSE_TYPES.OK
                        '            MyClass.IsWashingStationDownAttr = True
                        '            MyClass.CurrentOperation = OPERATIONS.NONE

                        'End Select


                End Select

            Catch ex As Exception
                MyClass.CurrentOperation = OPERATIONS.NONE
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsTestDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

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

                If MyClass.HistoryArea <> Nothing Then

                    Dim myHistoricReportDS As New SRVResultsServiceDS
                    Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    Dim myTask As String = ""
                    Dim myAction As String = ""

                    Select Case MyClass.HistoryTask
                        Case PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES : myTask = "ADJUST"
                        Case PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES : myTask = "TEST"
                        Case PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES : myTask = "UTIL"
                    End Select

                    Select Case MyClass.HistoryArea
                        Case HISTORY_AREAS.INTERNAL_DOSING : myAction = "INT_DOS"
                        Case HISTORY_AREAS.EXT_WASHING : myAction = "EXT_WASH"
                        Case HISTORY_AREAS.WS_ASPIRATION : myAction = "WS_ASP"
                        Case HISTORY_AREAS.WS_DISPENSATION : myAction = "WS_DISP"
                        Case HISTORY_AREAS.IN_OUT : myAction = "IN_OUT"
                        Case HISTORY_AREAS.COLLISION : myAction = "COLLISION"
                        Case HISTORY_AREAS.ENCODER : myAction = "GLF_ENC"
                    End Select

                    Dim Report As Boolean = False

                    Select Case MyClass.HistoryArea
                        Case HISTORY_AREAS.INTERNAL_DOSING
                            For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                                If MyClass.HisIndoValues(E) > 0 Then
                                    myAction = "INT_DOS"
                                    Report = True
                                    Exit For
                                End If
                            Next

                        Case HISTORY_AREAS.EXT_WASHING
                            For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                                If MyClass.HisIndoValues(E) > 0 Then
                                    myAction = "EXT_WASH"
                                    Report = True
                                    Exit For
                                End If
                            Next


                        Case HISTORY_AREAS.WS_ASPIRATION
                            For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                                If MyClass.HisIndoValues(E) > 0 Then
                                    myAction = "WS_ASP"
                                    Report = True
                                    Exit For
                                End If
                            Next


                        Case HISTORY_AREAS.WS_DISPENSATION
                            For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                                If MyClass.HisIndoValues(E) > 0 Then
                                    myAction = "IN_OUT"
                                    Report = True
                                    Exit For
                                End If
                            Next


                        Case HISTORY_AREAS.IN_OUT
                            For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                                If MyClass.HisIndoValues(E) > 0 Then
                                    myAction = "IN_OUT"
                                    Report = True
                                    Exit For
                                End If
                            Next


                        Case HISTORY_AREAS.COLLISION
                            Report = True

                        Case HISTORY_AREAS.ENCODER
                            Report = True

                    End Select

                    If Report Then

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

                    End If

                    If Report And (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
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
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.ManageHistoryResults", EventLogEntryType.Error, False)
            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' Decodes the data from DB in order to display in History Screen
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pData"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pAction As String, ByVal pData As String, ByVal pLanguageId As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try

                'Dim myUtility As New Utilities()
                Dim text1 As String = ""
                Dim text2 As String = ""
                Dim text3 As String = ""
                Dim myLine As List(Of String)
                Dim myLines As List(Of List(Of String))
                Dim FinalText As String = ""

                Dim myColWidth As Integer = 30
                Dim myColSep As Integer = 0

                'obtain the connection
                myResultData = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myResultData.SetDatos, SqlClient.SqlConnection)
                End If

                Select Case pAction
                    Case "INT_DOS", "EXT_WASH", "WS_ASP", "WS_DISP", "IN_OUT"

                        Dim myAreaLine As New List(Of String)
                        Dim myAllLines As New List(Of List(Of List(Of String)))
                        Dim myDatas As String() = pData.Split(CChar("|"))
                        Dim containsAditionalInfo As Boolean = False


                        For d As Integer = 0 To myDatas.Length - 1

                            Dim myData As String = myDatas(d)

                            If myData.Length > 0 Then

                                'area
                                Dim myArea As HISTORY_AREAS = HISTORY_AREAS.NONE

                                'element
                                Dim myIDNumber As String = ""
                                Dim myID As HISTORY_ELEMENTS = HISTORY_ELEMENTS.NONE

                                'type
                                Dim myTypeNumber As String = ""
                                Dim myType As HISTORY_ITEM_TYPES = HISTORY_ITEM_TYPES.NONE

                                'subsystem
                                Dim mySystemNumber As String = ""
                                Dim mySystem As HISTORY_SUBSYSTEMS = HISTORY_SUBSYSTEMS.NONE

                                'value
                                Dim myValueNumber As String = ""
                                Dim myValue As Integer = -99999

                                'simple switch
                                Dim mySimpleSwitchNumber As String = ""
                                Dim mySimpleSwitch As HISTORY_RESULTS = HISTORY_RESULTS.NOT_APPLIED

                                'continuous switch
                                Dim myContinuousSwitchNumber As String = ""
                                Dim myContinuousSwitch As HISTORY_RESULTS = HISTORY_RESULTS.NOT_APPLIED

                                'movement
                                Dim myMovementNumber As String = ""
                                Dim myMovement As HISTORY_RESULTS = HISTORY_RESULTS.NOT_APPLIED

                                'aditional info
                                Dim myInfoNumber As String = ""
                                Dim myInfo As HISTORY_ADITIONAL_INFO = HISTORY_ADITIONAL_INFO.NONE

                                Select Case pAction
                                    Case "INT_DOS" : myArea = HISTORY_AREAS.INTERNAL_DOSING
                                    Case "EXT_WASH" : myArea = HISTORY_AREAS.EXT_WASHING
                                    Case "WS_ASP" : myArea = HISTORY_AREAS.WS_ASPIRATION
                                    Case "WS_DISP" : myArea = HISTORY_AREAS.WS_DISPENSATION
                                    Case "IN_OUT" : myArea = HISTORY_AREAS.IN_OUT

                                End Select

                                myIDNumber = myData.Substring(0, 2)
                                myTypeNumber = myData.Substring(2, 1)
                                mySystemNumber = myData.Substring(3, 1)
                                myValueNumber = myData.Substring(4, 5)
                                mySimpleSwitchNumber = myData.Substring(9, 1)
                                myContinuousSwitchNumber = myData.Substring(10, 1)
                                myMovementNumber = myData.Substring(11, 1)
                                myInfoNumber = myData.Substring(12, 1)

                                If IsNumeric(myTypeNumber) Then
                                    myID = CType(CInt(myIDNumber), HISTORY_ELEMENTS)
                                    If IsNumeric(myTypeNumber) Then
                                        myType = CType(CInt(myTypeNumber), HISTORY_ITEM_TYPES)
                                        If IsNumeric(mySystemNumber) Then
                                            mySystem = CType(CInt(mySystemNumber), HISTORY_SUBSYSTEMS)
                                            If IsNumeric(myValueNumber) Then
                                                myValue = CInt(myValueNumber)
                                                If IsNumeric(mySimpleSwitchNumber) Then
                                                    mySimpleSwitch = CType(CInt(mySimpleSwitchNumber), HISTORY_RESULTS)
                                                    If IsNumeric(myContinuousSwitchNumber) Then
                                                        myContinuousSwitch = CType(CInt(myContinuousSwitchNumber), HISTORY_RESULTS)
                                                        If IsNumeric(myMovementNumber) Then
                                                            myMovement = CType(CInt(myMovementNumber), HISTORY_RESULTS)
                                                            If IsNumeric(myInfoNumber) Then
                                                                myInfo = CType(CInt(myInfoNumber), HISTORY_ADITIONAL_INFO)

                                                                text1 = ""
                                                                text2 = ""

                                                                myLines = New List(Of List(Of String))

                                                                If myAreaLine.Count = 0 Then
                                                                    'Area LINE (test area)
                                                                    '*****************************************************************
                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_TEST_AREA", pLanguageId) + ": ")
                                                                    myAreaLine.Add(text1)

                                                                    text2 = MyClass.DecodeHistoryArea(myArea, dbConnection, pLanguageId)
                                                                    myAreaLine.Add(text2)
                                                                End If

                                                                '(element ID)
                                                                '*****************************************************************
                                                                myLine = New List(Of String)

                                                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ELEMENT_ID", pLanguageId) + ": ")
                                                                myLine.Add(text1)

                                                                text2 = MyClass.DecodeHistoryId(myID, dbConnection, pLanguageId) & " (" & myID.ToString.Replace("_", ".") & ")"
                                                                myLine.Add(text2)

                                                                myLines.Add(myLine)

                                                                '(element type)
                                                                '*****************************************************************
                                                                myLine = New List(Of String)

                                                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ELEMENT_TYPE", pLanguageId) + ": ")
                                                                myLine.Add(text1)

                                                                text2 = MyClass.DecodeHistoryType(myType, dbConnection, pLanguageId)
                                                                myLine.Add(text2)

                                                                myLines.Add(myLine)

                                                                ' (subsystem)
                                                                '*****************************************************************
                                                                myLine = New List(Of String)

                                                                text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_CYCLES_SubSystem", pLanguageId) + ": ")
                                                                myLine.Add(text1)

                                                                text2 = MyClass.DecodeHistorySubsystem(mySystem, dbConnection, pLanguageId)
                                                                myLine.Add(text2)

                                                                myLines.Add(myLine)


                                                                ' (State/Value)
                                                                '*****************************************************************
                                                                myLine = New List(Of String)

                                                                If myType = HISTORY_ITEM_TYPES.DCMOTOR Then
                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FINAL_VALUE", pLanguageId) + ": ")
                                                                    myLine.Add(text1)
                                                                    text2 = myValue.ToString
                                                                    myLine.Add(text2)
                                                                Else
                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_FINAL_STATE", pLanguageId) + ": ")
                                                                    myLine.Add(text1)
                                                                    If myValue <= 0 Then
                                                                        text2 = "OFF"
                                                                    Else
                                                                        text2 = "ON"
                                                                    End If
                                                                    myLine.Add(text2)
                                                                End If


                                                                myLines.Add(myLine)



                                                                ' (simple switch)
                                                                '*****************************************************************
                                                                If mySimpleSwitch <> HISTORY_RESULTS.NOT_APPLIED Then
                                                                    myLine = New List(Of String)

                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_SimpleSwitch", pLanguageId) + ": ")
                                                                    myLine.Add(text1)

                                                                    text2 = MyClass.GetResultLanguageResource(dbConnection, mySimpleSwitch, pLanguageId)
                                                                    myLine.Add(text2)

                                                                    myLines.Add(myLine)
                                                                End If

                                                                ' (continuous switch)
                                                                '*****************************************************************
                                                                If myContinuousSwitch <> HISTORY_RESULTS.NOT_APPLIED Then
                                                                    myLine = New List(Of String)

                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ContinuousSwitch", pLanguageId) + ": ")
                                                                    myLine.Add(text1)

                                                                    text2 = MyClass.GetResultLanguageResource(dbConnection, myContinuousSwitch, pLanguageId)
                                                                    myLine.Add(text2)

                                                                    myLines.Add(myLine)
                                                                End If

                                                                ' (movement)
                                                                '*****************************************************************
                                                                If myMovement <> HISTORY_RESULTS.NOT_APPLIED Then
                                                                    myLine = New List(Of String)

                                                                    text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_MOTOR_MOVE", pLanguageId) + ": ")
                                                                    myLine.Add(text1)

                                                                    text2 = MyClass.GetResultLanguageResource(dbConnection, myMovement, pLanguageId)
                                                                    myLine.Add(text2)

                                                                    myLines.Add(myLine)
                                                                End If

                                                                ' (Aditional Info)
                                                                '*****************************************************************


                                                                text1 = ""
                                                                text2 = ""
                                                                myLine = New List(Of String)

                                                                If myArea = HISTORY_AREAS.INTERNAL_DOSING Then
                                                                    If myInfo <> HISTORY_ADITIONAL_INFO.NONE Then
                                                                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_DOSING_SOURCE", pLanguageId) + ": ")
                                                                    End If
                                                                End If

                                                                myLine.Add(text1)

                                                                If myArea = HISTORY_AREAS.INTERNAL_DOSING Then
                                                                    Select Case myInfo
                                                                        Case HISTORY_ADITIONAL_INFO.DOSING_WS
                                                                            text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WashSolution", pLanguageId)).Trim

                                                                        Case HISTORY_ADITIONAL_INFO.DOSING_DW
                                                                            text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_DISTILLED_WATER", pLanguageId)).Trim

                                                                        Case HISTORY_ADITIONAL_INFO.DOSING_AIR
                                                                            text2 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_AIR", pLanguageId)).Trim

                                                                    End Select
                                                                End If

                                                                myLine.Add(text2)

                                                                containsAditionalInfo = (text1.Length > 0) And (text2.Length > 0)

                                                                myLines.Add(myLine)


                                                                myAllLines.Add(myLines)

                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next

                        If myAllLines.Count > 0 Then
                            FinalText = Utilities.FormatLineHistorics(myAreaLine, myColWidth, True)
                        End If

                        For Each LL As List(Of List(Of String)) In myAllLines
                            For Each L As List(Of String) In LL
                                Dim newLine As Boolean = False
                                Select Case LL.IndexOf(L) + 1
                                    Case 1 : newLine = False 'element ID
                                    Case 2 : newLine = False 'type
                                    Case 3 : newLine = False 'subsystem
                                    Case 4 : newLine = False 'simple switch
                                    Case 5 : newLine = False 'continuous
                                    Case 6 : newLine = False 'movement
                                    Case 7 : newLine = False 'state/value
                                    Case 8 : newLine = containsAditionalInfo 'Aditional Info

                                End Select

                                Dim hasText As Boolean = False
                                For Each t As String In L
                                    If t.Length > 0 Then
                                        t.PadRight(myColWidth)
                                        hasText = True
                                    End If
                                Next

                                If hasText Then FinalText &= Utilities.FormatLineHistorics(L, myColWidth, newLine)

                            Next L

                            FinalText &= vbCrLf

                        Next LL

                        myResultData.SetDatos = FinalText.Trim


                    Case "COLLISION"

                        text1 = ""
                        text2 = ""
                        myLines = New List(Of List(Of String))

                        '1st LINE WASHING STATION
                        '******************************************************************************
                        myLine = New List(Of String)
                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_WashStation", pLanguageId) + ": ")
                        myLine.Add(text1)
                        text2 = MyClass.DecodeHistoryCollision(pData, HISTORY_COLLISIONS.WASHING_STATION, dbConnection, pLanguageId)
                        myLine.Add(text2)
                        myLines.Add(myLine)

                        '1st LINE SAMPLE
                        '******************************************************************************
                        myLine = New List(Of String)
                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_SamplesNeedle", pLanguageId) + ": ")
                        myLine.Add(text1)
                        text2 = MyClass.DecodeHistoryCollision(pData, HISTORY_COLLISIONS.SAMPLE, dbConnection, pLanguageId)
                        myLine.Add(text2)
                        myLines.Add(myLine)


                        '1st LINE REAGENT1
                        '******************************************************************************
                        myLine = New List(Of String)
                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent1Needle", pLanguageId) + ": ")
                        myLine.Add(text1)
                        text2 = MyClass.DecodeHistoryCollision(pData, HISTORY_COLLISIONS.REAGENT1, dbConnection, pLanguageId)
                        myLine.Add(text2)
                        myLines.Add(myLine)


                        '1st LINE REAGENT2
                        '******************************************************************************
                        myLine = New List(Of String)
                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Reagent2Needle", pLanguageId) + ": ")
                        myLine.Add(text1)
                        text2 = MyClass.DecodeHistoryCollision(pData, HISTORY_COLLISIONS.REAGENT2, dbConnection, pLanguageId)
                        myLine.Add(text2)
                        myLines.Add(myLine)


                        For Each Line As List(Of String) In myLines
                            For Each t As String In Line
                                If t.Length > 0 Then
                                    t.PadRight(myColWidth)
                                End If
                            Next
                            FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, True)
                        Next

                        myResultData.SetDatos = FinalText


                    Case "ENCODER"

                        text1 = ""
                        text2 = ""

                        myLines = New List(Of List(Of String))

                        ' (Lost Steps)
                        '*****************************************************************
                        myLine = New List(Of String)

                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_ENCODER_LOSS", pLanguageId) + ": ")
                        myLine.Add(text1)

                        text2 = CInt(pData.Substring(0, 4)).ToString
                        myLine.Add(text2)

                        myLines.Add(myLine)

                        ' (Tested)
                        '*****************************************************************
                        myLine = New List(Of String)

                        Dim myResult As HISTORY_RESULTS = MyClass.DecodeHistoryDataResult(pData, HISTORY_AREAS.ENCODER)

                        text1 = (MLRD.GetResourceText(dbConnection, "LBL_SRV_Tested", pLanguageId) + ": ")
                        myLine.Add(text1)

                        text2 = MyClass.GetResultLanguageResource(dbConnection, myResult, pLanguageId)
                        myLine.Add(text2)

                        myLines.Add(myLine)

                        For Each Line As List(Of String) In myLines
                            For Each t As String In Line
                                If t.Length > 0 Then
                                    t.PadRight(myColWidth)
                                End If
                            Next
                            FinalText &= Utilities.FormatLineHistorics(Line, myColWidth, True)
                        Next

                        myResultData.SetDatos = FinalText

                End Select





            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeDataReport", EventLogEntryType.Error, False)

            Finally
                If Not dbConnection IsNot Nothing Then dbConnection.Close()
            End Try

            Return myResultData

        End Function

        Public Sub InitializeHistoryElements()
            Try

                MyClass.HisElementType = HISTORY_ITEM_TYPES.NONE

                'Internal Dosing
                MyClass.HistoryInDoElements = New List(Of HISTORY_ELEMENTS)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_B1)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_B2)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_B3)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_EV1)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_EV2)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_EV3)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_EV4)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_EV5)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_MSA)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_MR1A)
                MyClass.HistoryInDoElements.Add(HISTORY_ELEMENTS.JE1_MR2A)

                MyClass.HisIndoValues = New Dictionary(Of HISTORY_ELEMENTS, Integer)
                MyClass.HisIndoSimpleSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisIndoContinuousSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisIndoMovements = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                For Each E As HISTORY_ELEMENTS In HistoryInDoElements
                    MyClass.HisIndoValues.Add(E, -1)
                    MyClass.HisIndoSimpleSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisIndoContinuousSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisIndoMovements.Add(E, HISTORY_RESULTS.NOT_DONE)
                Next


                'External Washing Dosing
                MyClass.HistoryExWaElements = New List(Of HISTORY_ELEMENTS)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_B1)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_B2)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_B3)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_EV1)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_EV2)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_EV3)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_EV4)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_EV5)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_MSA)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_MR1A)
                MyClass.HistoryExWaElements.Add(HISTORY_ELEMENTS.JE1_MR2A)

                MyClass.HisExWaValues = New Dictionary(Of HISTORY_ELEMENTS, Integer)
                MyClass.HisExWaSimpleSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisExWaContinuousSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisExWaMovements = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                For Each E As HISTORY_ELEMENTS In HistoryExWaElements
                    MyClass.HisExWaValues.Add(E, -1)
                    MyClass.HisExWaSimpleSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisExWaContinuousSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisExWaMovements.Add(E, HISTORY_RESULTS.NOT_DONE)
                Next

                'Washing station Aspiration
                MyClass.HistoryWsAspElements = New List(Of HISTORY_ELEMENTS)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_B1)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_B2)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_B3)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_EV1)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_EV2)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_EV3)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_EV4)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_EV5)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_MSA)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_MR1A)
                MyClass.HistoryWsAspElements.Add(HISTORY_ELEMENTS.JE1_MR2A)

                MyClass.HisWsAspValues = New Dictionary(Of HISTORY_ELEMENTS, Integer)
                MyClass.HisWsAspSimpleSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisWsAspContinuousSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisWsAspMovements = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                For Each E As HISTORY_ELEMENTS In HistoryWsAspElements
                    MyClass.HisWsAspValues.Add(E, -1)
                    MyClass.HisWsAspSimpleSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisWsAspContinuousSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisWsAspMovements.Add(E, HISTORY_RESULTS.NOT_DONE)
                Next

                'Washing station Dispensation
                MyClass.HistoryWsDispElements = New List(Of HISTORY_ELEMENTS)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_B1)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_B2)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_B3)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_EV1)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_EV2)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_EV3)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_EV4)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_EV5)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_MSA)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_MR1A)
                MyClass.HistoryWsDispElements.Add(HISTORY_ELEMENTS.JE1_MR2A)

                MyClass.HisWsDispValues = New Dictionary(Of HISTORY_ELEMENTS, Integer)
                MyClass.HisWsDispSimpleSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisWsDispContinuousSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisWsDispMovements = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                For Each E As HISTORY_ELEMENTS In HistoryWsDispElements
                    MyClass.HisWsDispValues.Add(E, -1)
                    MyClass.HisWsDispSimpleSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisWsDispContinuousSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisWsDispMovements.Add(E, HISTORY_RESULTS.NOT_DONE)
                Next

                'System Fluids In Out
                MyClass.HistoryInOutElements = New List(Of HISTORY_ELEMENTS)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_B1)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_B2)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_B3)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_EV1)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_EV2)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_EV3)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_EV4)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_EV5)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_MSA)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_MR1A)
                MyClass.HistoryInOutElements.Add(HISTORY_ELEMENTS.JE1_MR2A)

                MyClass.HisInOutValues = New Dictionary(Of HISTORY_ELEMENTS, Integer)
                MyClass.HisInOutSimpleSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisInOutContinuousSwitches = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                MyClass.HisInOutMovements = New Dictionary(Of HISTORY_ELEMENTS, HISTORY_RESULTS)
                For Each E As HISTORY_ELEMENTS In HistoryInOutElements
                    MyClass.HisInOutValues.Add(E, -1)
                    MyClass.HisInOutSimpleSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisInOutContinuousSwitches.Add(E, HISTORY_RESULTS.NOT_DONE)
                    MyClass.HisInOutMovements.Add(E, HISTORY_RESULTS.NOT_DONE)
                Next

                MyClass.HisCollisions.Clear()
                MyClass.HisCollisions.Add(HISTORY_COLLISIONS.WASHING_STATION, False)
                MyClass.HisCollisions.Add(HISTORY_COLLISIONS.SAMPLE, False)
                MyClass.HisCollisions.Add(HISTORY_COLLISIONS.REAGENT1, False)
                MyClass.HisCollisions.Add(HISTORY_COLLISIONS.REAGENT2, False)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.InitializeHistoryElements", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Private"


        ''' <summary>
        ''' Reports the History data to DB. Encodes the result data to history
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GenerateResultData() As String

            Dim myData As String = ""

            Try

                Select Case MyClass.HistoryArea
                    Case HISTORY_AREAS.INTERNAL_DOSING

                        For Each E As HISTORY_ELEMENTS In MyClass.HistoryInDoElements
                            If MyClass.HisIndoValues(E) > 0 Then
                                myData &= "|"
                                Dim id As String = E.ToString
                                Select Case id.Substring(0, 3)
                                    Case "JE1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.MANIFOLD
                                    Case "SF1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.FLUIDICS
                                    Case "GLF" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                End Select

                                myData &= CInt(E).ToString("00")
                                myData &= CInt(MyClass.HisElementType).ToString
                                myData &= CInt(MyClass.HisSubsystem).ToString
                                myData &= CInt(MyClass.HisIndoValues(E)).ToString("00000")
                                myData &= CInt(MyClass.HisIndoSimpleSwitches(E)).ToString
                                myData &= CInt(MyClass.HisIndoContinuousSwitches(E)).ToString
                                myData &= CInt(MyClass.HisIndoMovements(E)).ToString
                                myData &= CInt(MyClass.HisAditionalInfo).ToString
                                myData &= "|"
                            End If
                        Next

                    Case HISTORY_AREAS.EXT_WASHING

                        For Each E As HISTORY_ELEMENTS In MyClass.HistoryExWaElements
                            If MyClass.HisExWaValues(E) > 0 Then
                                myData &= "|"
                                Dim id As String = E.ToString
                                Select Case id.Substring(0, 3)
                                    Case "JE1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.MANIFOLD
                                    Case "SF1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.FLUIDICS
                                    Case "GLF" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                End Select

                                myData &= CInt(E).ToString("00")
                                myData &= CInt(MyClass.HisElementType).ToString
                                myData &= CInt(MyClass.HisSubsystem).ToString
                                myData &= CInt(MyClass.HisExWaValues(E)).ToString("00000")
                                myData &= CInt(MyClass.HisExWaSimpleSwitches(E)).ToString
                                myData &= CInt(MyClass.HisExWaContinuousSwitches(E)).ToString
                                myData &= CInt(MyClass.HisExWaMovements(E)).ToString
                                myData &= CInt(MyClass.HisAditionalInfo).ToString
                                myData &= "|"
                            End If
                        Next


                    Case HISTORY_AREAS.WS_ASPIRATION

                        For Each E As HISTORY_ELEMENTS In MyClass.HistoryWsDispElements
                            If MyClass.HisWsDispValues(E) > 0 Then
                                myData &= "|"
                                Dim id As String = E.ToString
                                Select Case id.Substring(0, 3)
                                    Case "JE1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.MANIFOLD
                                    Case "SF1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.FLUIDICS
                                    Case "GLF" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                End Select

                                myData &= CInt(E).ToString("00")
                                myData &= CInt(MyClass.HisElementType).ToString
                                myData &= CInt(MyClass.HisSubsystem).ToString
                                myData &= CInt(MyClass.HisWsDispValues(E)).ToString("00000")
                                myData &= CInt(MyClass.HisWsDispSimpleSwitches(E)).ToString
                                myData &= CInt(MyClass.HisWsDispContinuousSwitches(E)).ToString
                                myData &= CInt(MyClass.HisWsDispMovements(E)).ToString
                                myData &= CInt(MyClass.HisAditionalInfo).ToString
                                myData &= "|"
                            End If
                        Next

                    Case HISTORY_AREAS.WS_DISPENSATION

                        For Each E As HISTORY_ELEMENTS In MyClass.HistoryWsAspElements
                            If MyClass.HisWsAspValues(E) > 0 Then
                                myData &= "|"
                                Dim id As String = E.ToString
                                Select Case id.Substring(0, 3)
                                    Case "JE1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.MANIFOLD
                                    Case "SF1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.FLUIDICS
                                    Case "GLF" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                End Select

                                myData &= CInt(E).ToString("00")
                                myData &= CInt(MyClass.HisElementType).ToString
                                myData &= CInt(MyClass.HisSubsystem).ToString
                                myData &= CInt(MyClass.HisWsAspValues(E)).ToString("00000")
                                myData &= CInt(MyClass.HisWsAspSimpleSwitches(E)).ToString
                                myData &= CInt(MyClass.HisWsAspContinuousSwitches(E)).ToString
                                myData &= CInt(MyClass.HisWsAspMovements(E)).ToString
                                myData &= CInt(MyClass.HisAditionalInfo).ToString
                                myData &= "|"
                            End If
                        Next


                    Case HISTORY_AREAS.IN_OUT

                        For Each E As HISTORY_ELEMENTS In MyClass.HistoryInOutElements
                            If MyClass.HisInOutValues(E) > 0 Then
                                myData &= "|"
                                Dim id As String = E.ToString
                                Select Case id.Substring(0, 3)
                                    Case "JE1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.MANIFOLD
                                    Case "SF1" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.FLUIDICS
                                    Case "GLF" : MyClass.HisSubsystem = HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                End Select

                                myData &= CInt(E).ToString("00")
                                myData &= CInt(MyClass.HisElementType).ToString
                                myData &= CInt(MyClass.HisSubsystem).ToString
                                myData &= CInt(MyClass.HisInOutValues(E)).ToString("00000")
                                myData &= CInt(MyClass.HisInOutSimpleSwitches(E)).ToString
                                myData &= CInt(MyClass.HisInOutContinuousSwitches(E)).ToString
                                myData &= CInt(MyClass.HisInOutMovements(E)).ToString
                                myData &= CInt(MyClass.HisAditionalInfo).ToString
                                myData &= "|"
                            End If
                        Next


                    Case HISTORY_AREAS.COLLISION
                        If MyClass.HisCollisions(HISTORY_COLLISIONS.WASHING_STATION) Then
                            myData = "1"
                        Else
                            myData = "0"
                        End If
                        If MyClass.HisCollisions(HISTORY_COLLISIONS.SAMPLE) Then
                            myData &= "1"
                        Else
                            myData &= "0"
                        End If
                        If MyClass.HisCollisions(HISTORY_COLLISIONS.REAGENT1) Then
                            myData &= "1"
                        Else
                            myData &= "0"
                        End If
                        If MyClass.HisCollisions(HISTORY_COLLISIONS.REAGENT2) Then
                            myData &= "1"
                        Else
                            myData &= "0"
                        End If


                    Case HISTORY_AREAS.ENCODER
                        myData = MyClass.EncodeHistoryValue(MyClass.HisEncoderLostSteps)
                        myData &= MyClass.EncodeHistoryResult(CInt(MyClass.HisTestActionResult))

                End Select


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.GenerateResultData", EventLogEntryType.Error, False)
            End Try

            Return myData

        End Function

        ''' <summary>
        ''' Encodes History Result
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/09/2011</remarks>
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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.EncodeHistoryResult", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Encodes History Value
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/09/2011</remarks>
        Private Function EncodeHistoryValue(ByVal pValue As Integer) As String

            Dim res As String = "xxxxx"

            Try
                If pValue >= 0 Then
                    res = pValue.ToString("00000")
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.EncodeHistoryValue", EventLogEntryType.Error, False)
                res = "xxxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Result
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataResult(ByVal pData As String, ByVal pArea As HISTORY_AREAS) As HISTORY_RESULTS

            Dim myResult As HISTORY_RESULTS

            Try

                Dim myResultNumber As String = ""

                If pArea = HISTORY_AREAS.ENCODER Then
                    myResultNumber = pData.Substring(5, 1)
                Else
                    myResultNumber = pData.Substring(11, 1)
                End If

                If IsNumeric(myResultNumber) Then
                    myResult = CType(CInt(myResultNumber), HISTORY_RESULTS)
                ElseIf myResultNumber = "x" Then
                    myResult = CType(-1, HISTORY_RESULTS)
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
            End Try

            Return myResult

        End Function

        ''' <summary>
        ''' Decodes History Value
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <param name="pType"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function DecodeHistoryDataValue(ByVal pData As String, ByVal pType As HISTORY_ITEM_TYPES) As String

            Dim res As String = "xxxxx"

            Try

                Dim myResultNumber As String = ""

                myResultNumber = pData.Substring(5, 5)

                If IsNumeric(myResultNumber) Then
                    If pType = HISTORY_ITEM_TYPES.DCMOTOR Then
                        res = CInt(myResultNumber).ToString
                    Else
                        Select Case CInt(myResultNumber)
                            Case 0 : res = "OFF"
                            Case 1 : res = "ON"
                        End Select
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryDataResult", EventLogEntryType.Error, False)
                res = "xxxxx"
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Area
        ''' </summary>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        ''' 
        Private Function DecodeHistoryArea(ByVal pArea As HISTORY_AREAS, ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pLanguageId As String = "ENG") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                Select Case pArea
                    Case HISTORY_AREAS.INTERNAL_DOSING : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_InternalDosingTitle", pLanguageId)
                    Case HISTORY_AREAS.EXT_WASHING : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_ExternalWashingTitle", pLanguageId)
                    Case HISTORY_AREAS.WS_ASPIRATION : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_WS_AspirationTitle", pLanguageId)
                    Case HISTORY_AREAS.WS_DISPENSATION : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_WS_DispensationTitle", pLanguageId)
                    Case HISTORY_AREAS.IN_OUT : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_IN_OUT", pLanguageId)
                    Case HISTORY_AREAS.COLLISION : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_CollisionTest", pLanguageId)
                    Case HISTORY_AREAS.ENCODER : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_ROTOR_ENCODER_TEST", pLanguageId)
                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryArea", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Subsystem
        ''' </summary>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/09/2011</remarks>
        ''' 
        Private Function DecodeHistorySubsystem(ByVal pSubsystem As HISTORY_SUBSYSTEMS, ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pLanguageId As String = "ENG") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                Select Case pSubsystem
                    Case HISTORY_SUBSYSTEMS.MANIFOLD : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_MANIFOLD", pLanguageId)
                    Case HISTORY_SUBSYSTEMS.FLUIDICS : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_FLUIDICS", pLanguageId)
                    Case HISTORY_SUBSYSTEMS.PHOTOMETRICS : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_PHOTOMETRICS", pLanguageId)

                End Select


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistorySubsystem", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Element Id
        ''' </summary>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/09/2011</remarks>
        ''' 
        Private Function DecodeHistoryId(ByVal pElement As HISTORY_ELEMENTS, ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pLanguageId As String = "ENG") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_" & pElement.ToString, pLanguageId)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryId", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function



        ''' <summary>
        ''' Decodes History Element Type
        ''' </summary>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/09/2011</remarks>
        ''' 
        Private Function DecodeHistoryType(ByVal pType As HISTORY_ITEM_TYPES, _
                                           ByVal pDBConnection As SqlClient.SqlConnection, _
                                           Optional ByVal pLanguageId As String = "ENG") As String

            Dim res As String = ""
            Dim MLRD As New MultilanguageResourcesDelegate

            Try

                Select Case pType
                    Case HISTORY_ITEM_TYPES.EVALVE : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_VALVE", pLanguageId)
                    Case HISTORY_ITEM_TYPES.EVALVE3 : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_3WAYVALVE", pLanguageId)
                    Case HISTORY_ITEM_TYPES.PUMP : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_Pump", pLanguageId)
                    Case HISTORY_ITEM_TYPES.DCMOTOR : res = MLRD.GetResourceText(pDBConnection, "LBL_SRV_DC_MOTOR", pLanguageId)
                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryType", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Decodes History Collision needle
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/09/2011</remarks>
        Private Function DecodeHistoryCollision(ByVal pData As String, _
                                                ByVal pNeedle As HISTORY_COLLISIONS, _
                                                ByVal pDBConnection As SqlClient.SqlConnection, _
                                                Optional ByVal pLanguageId As String = "ENG") As String

            Dim MLRD As New MultilanguageResourcesDelegate
            Dim myResultText As String = ""

            Try
                Dim myIndex As Integer = -1

                Select Case pNeedle
                    Case HISTORY_COLLISIONS.WASHING_STATION : myIndex = 0
                    Case HISTORY_COLLISIONS.SAMPLE : myIndex = 1
                    Case HISTORY_COLLISIONS.REAGENT1 : myIndex = 2
                    Case HISTORY_COLLISIONS.REAGENT2 : myIndex = 3
                End Select

                Dim myResult As Integer = -1
                Dim myCollision As String = ""

                myCollision = pData.Substring(myIndex, 1)

                If IsNumeric(myCollision) Then
                    myResult = CInt(myCollision)
                    If myResult > 0 Then
                        myResultText = MLRD.GetResourceText(pDBConnection, "LBL_SRV_Tested", pLanguageId)
                    Else
                        myResultText = MLRD.GetResourceText(pDBConnection, "LBL_SRV_NOT_TESTED", pLanguageId)
                    End If

                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.DecodeHistoryCollision", EventLogEntryType.Error, False)
            End Try

            Return myResultText

        End Function


        ''' <summary>
        ''' Gets Result Text
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pResult"></param>
        ''' <param name="pLanguageId"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/08/2011</remarks>
        Private Function GetResultLanguageResource(ByRef pdbConnection As SqlClient.SqlConnection, ByVal pResult As HISTORY_RESULTS, ByVal pLanguageId As String) As String

            Dim res As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Try
                Select Case pResult
                    Case HISTORY_RESULTS.OK
                        res = "OK"

                    Case HISTORY_RESULTS.NOK
                        res = "No OK"

                    Case HISTORY_RESULTS.NOT_DONE
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_NOT_DONE", pLanguageId)

                    Case HISTORY_RESULTS.CANCEL
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_CANCELLED", pLanguageId)

                    Case HISTORY_RESULTS._ERROR
                        res = myMultiLangResourcesDelegate.GetResourceText(pdbConnection, "LBL_SRV_FAILED", pLanguageId)

                End Select
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.GetResultLanguageResource", EventLogEntryType.Error, False)
            End Try

            Return res

        End Function

        ''' <summary>
        ''' registering the incidence in historical reports activity
        ''' </summary>
        ''' <remarks>Created by SGM 04/08/2011</remarks>
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
                GlobalBase.CreateLogActivity(ex.Message, "MotorsPumpsValvesTestDelegate.UpdateRecommendations", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#End Region


    End Class

End Namespace
