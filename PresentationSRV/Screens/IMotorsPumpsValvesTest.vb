Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.App

#Region "Comments"
'PREGUNTAS:
'********************************************************

'GENERAL
'errores gráficos 
'cursor sobre todos los elementos de la pantalla al habilitar y deshabilitar

'INTERNAL DOSING

'EXTERNAL WASHING

'WS ASPIRATION

'WS DISPENSATION
'Cómo se deben actuar los elementos para dispensar? se debe seguir un orden? 

'IN OUT

'COLLISION

'ENCODER ROTOR




'PENDIENTE:
'*********************************************************************

'GENERAL
'Falta el POLHW en Firmware por lo que ahora los valores de estado de los elementos se presuponen

'INTERNAL DOSING

'EXTERNAL WASHING
'No funciona bien la comprobacion de nivel de depositos antes de activar bombas

'WS ASPIRATION

'WS DISPENSATION


'IN OUT
'No funciona bien la comprobacion de nivel de depositos antes de activar bombas

'COLLISION
'Falta del firmware la funcionalidad de desabilitar alarmas

'ENCODER ROTOR



'*****************************************************************************************************************
'REUNIÓN 02/06/2011 (Raúl)
'Las EV tienen los campos: 
'   Ex = EN(on) DI(off)
'   EDx = 0(ok) >0(nok)
'Las Bombas tienen los campos: 
'   Bx = EN(on) DI(off)
'   BDx = 0(ok) >0(nok)


'ANSJEX:
'Las Motores (MS-muestras, MR1-reactivos1, MR2-reactivos2) tienen los campos: 
'   MX = EN(on) DI(off)
'   BXH = C(closed) O(open) KO(error)
'   MXA = -xxxxxx to xxxxxxx (posicion absoluta)
'Detector de coágulo
'   CLT = XXXXX (valor ASCII)
'   CLTD = OK(Clot free)  BS(Fluid system Blocked)  CD(Clot Detected)  CP(Clot Possible)  KO(Out of Scale - Impossible Value - Error)



'ANSSFX:
'Las Motores (MS-washing station Pump) tienen los campos: 
'   MS = EN(on) DI(off)
'   BSH = C(closed) O(open) KO(error)
'   MSA = -xxxxxx to xxxxxxx (posicion absoluta)
'Boyas (WAS-low contamination SLS-Distilled Water)
'   WASTE TANK:
'       WAS = 0 1 2 3
'               0	Low sensor Down - High Sensor Down (Empty)
'               1	Low sensor High - High Sensor Down (Middle)
'               2	Low Sensor Down - High sensor High (En Running es un estado imposible - error) (En servicio es posible con manipulación)
'               3	Both sensors High (Full)
'       WTS = EN(Emptying Tank) DI(Ok)

'   SYSTEM LIQUID TANK:
'       SLS = 0 1 2 3
'               0	Low sensor Down - High Sensor Down (Empty)
'               1	Low sensor High - High Sensor Down (Middle)
'               2	Low Sensor Down - High sensor High (En Running es un estado imposible - error) (En servicio es posible con manipulación)
'               3	Both sensors High (Full)
'       STS	= EN(Filling Tank) DI(Ok)

'ANSGLF:
'Las Motores (MR-Reactions Rotor, MW- Washing Station Motor(Pinta)) tienen los campos: 
'   MR = EN(on) DI(off)
'   BRH = C(closed) O(open) KO(error)
'   MRA = -xxxxxx to xxxxxxx (posicion absoluta)
'   MRE = -xxxxxx to xxxxxxx (posicion encoder)
'   MRED = 0(ok) >0(nok) diagnóstico encoder

'   MW = EN(on) DI(off)
'   BWH = C(closed) O(open) KO(error)
'   MWA = -xxxxxx to xxxxxxx (posicion absoluta)

'***************************************************************************************************************
'REUNIÓN 01/06/2011 (Raúl)
'In/Out:
'Antes de realizar cualquier acción mirar estado de PW o LC (leer boyas) y actuar en consecuencia (desahbilitando si procede)
'Collision:
'En principio se tratan las colisiones como alarmas recibidas a través de la trama de estado -> ANSERR
'Pendiente definir qué hacer para restablecer el sistema (Instrucción RECOVER??)
'Si hay una válvula en serie con una bomba
'Activar Bomba:     EV_ON    50ms    B_ON
'Desactivar Bomba:  B_OFF    500ms   EV_OFF
'Las bombas siempre se arrancan en progresivo
'El grupo EV en Dispensación se activan SF1.GE1.DCS.100.0   Desactivan: SF1.GE1.DCE 
'Motor émbolo dispensación: SF1.M1
'Motor Rotor Reacciones: GLF.MR
'Motor WS: GLF.MP
'JE1_B1: Duty 60%
'SF1_B1: Duty 50%

'*******************************************************************************************************
'REUNIÓN 26/05/2011 (sergi)
'Leer del sistema (ANSINF ó ANSSFX?) si esta en marcha el sitema de Autoacondicionamiento para 
'mostrarlo en Tanques y Motores (limitando al activación de elementos). En ese caso mostrar limitación en barra de mensajes
'Test encoder: consiste en ir moviendo aleatoriamente el Rotor a la vez que se van sumando los pasos
'en un acumulador intreno. Finalmente se pregunta con POLLHW ID=GLF los pasos que nos dice el FW y
'se compara con lo sumado
'Comprobar checksum al recibir (y enviar?) el fichero de ajustes

#End Region

'

'''<summary>
'''This Test Screen is aimed for testing the correct function of the electro-mechanic-hydraulic 
'''elements (motors, pumps, valves and detectors) in the fluidic systems of the Analyzer like 
'''Internal Dosing, arms needles External Washing, aspiration and dispensation  in Washing Station 
'''and fluids In/Out. Each of these systems has its corresponding visual scheme in which all testable 
'''elements are displayed.
''' </summary>
''' <remarks>Created by SGM 03/05/2011</remarks>
Public Class UiMotorsPumpsValvesTest
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        DefineScreenLayout()

        ' Add any initialization after the InitializeComponent() call.
        'Me.BsTabPagesControl.Controls.Add(Me.BsInternalDosingTabPage)
        'Me.BsTabPagesControl.Controls.Add(Me.BsExternalWashingTabPage)
        'Me.BsTabPagesControl.Controls.Add(Me.BsAspirationTabPage)
        'Me.BsTabPagesControl.Controls.Add(Me.BsDispensationTabPage)
        'Me.BsTabPagesControl.Controls.Add(Me.BsInOutTabPage)
        'Me.BsTabPagesControl.Controls.Add(Me.BsCollisionDetectionTabPage)

    End Sub
#End Region

#Region "Testable Elements Collection"
    Private TestableElements As List(Of BsScadaControl)

#End Region

#Region "Private Structures"

    Private Structure ArmAdjustmentRowData

        Public CodeFw As String
        Public Value As String
        Public CanMove As Boolean
        Public AxisID As String
        Public GroupID As String


        Public Sub New(ByVal pCodeFw As String)
            CodeFw = pCodeFw
            Value = ""
            CanMove = False
            GroupID = ""
            AxisID = ""
        End Sub

        Public Sub Clear()
            CodeFw = ""
            Value = ""
            CanMove = False
            GroupID = ""
            AxisID = ""
        End Sub
    End Structure

#End Region

#Region "Private Constants"
    'FIELD LIMITS PENDING TO SPEC!!!!!!!!!!!!!!
    Private Const SwitchInterval As Integer = 1000

    Private Const SimPreActivationDelay As Integer = 500 '50ms
    Private Const SimPostActivationDelay As Integer = 500 '50ms

#End Region

#Region "Flags"
    Private ManageTabPages As Boolean = False
    Private IsLoaded As Boolean = False
    Private IsInternalDosingSourceChanging As Boolean = False
    Private IsDispensationValvesSwitching As Boolean = False
    Private IsSettingDefaultStates As Boolean = False
    Private IsArmsToHoming As Boolean = False
    Private IsArmsToWashing As Boolean = False
    Private IsArmsToParking As Boolean = False
    Private IsWashingStationDownRequested As Boolean = False 'SGM 22/11/2011
    'Private IsWashingStationToDown As Boolean = False 'SGM 21/11/2011
    'Private IsWashingStationToUp As Boolean = False 'SGM 21/11/2011
    Private IsTabChangeRequested As Boolean = False
    Private IsScreenCloseRequested As Boolean = False
    Private IsFirstReadingDone As Boolean = False
    Private IsOutQuickConnectorWarnt As Boolean = False
    Private IsCollisionTestEnabledAttr As Boolean = False
    Private CollidedNeedleAttr As GlobalEnumerates.UTILCollidedNeedles = UTILCollidedNeedles.None
    Private DefaultStatesDone As Boolean    ' XBC 11/11/2011

    Private IsReadyToCloseAttr As Boolean
    Private IsAllToDefaultRequested As Boolean = False

#End Region

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As MotorsPumpsValvesTestDelegate

    ' Language
    Private LanguageID As String
    Private SelectedPage As TEST_PAGES
    Private currentItemtoTest As ADJUSTMENT_GROUPS

    'general
    Private AirFluidColor As Color = Color.White
    Private WashingSolutionFluidColor As Color = Color.Honeydew
    Private DistilledWaterFluidColor As Color = Color.Azure
    Private HighContaminationFluidColor As Color = Color.Yellow
    Private LowContaminationFluidColor As Color = Color.LightYellow

    Private WashingSolutionCaption As String = "Washing Solution"
    Private DistilledWaterCaption As String = "Distilled Water"
    Private AirCaption As String = "Air"
    Private HighContaminationCaption As String = "High Contamination"
    Private LowContaminationCaption As String = "High Contamination"

    Private ValveCaption As String = "Valve"
    Private Valve3Caption As String = "3 way Valve"
    Private PumpCaption As String = "Valve"
    Private MotorCaption As String = "Valve"

    Private SelectedElement As BsScadaControl
    Private SelectedElementGroup As ItemGroups = ItemGroups._NONE

    Private myTooltip As ToolTip

    'ADJUSTMENTS
    Private SelectedAdjustmentsDS As New SRVAdjustmentsDS


    Private ReadRequestedElement As BsScadaControl
    Private SwitchRequestedElement As BsScadaControl
    Private MoveRequestedElement As BsScadaMotorControl
    Private RequestedNewState As BsScadaControl.States = BsScadaControl.States._NONE

    Private MoveRequestedMode As MOVEMENT = MOVEMENT._NONE
    Private RequestedNewPosition As Integer = 0
    'Private RequestedNewValve3State As BsScadaValve3Control.Valve3States = BsScadaValve3Control.Valve3States._None

    Private CurrentTestPanel As Panel = Me.BsInternalDosingTestPanel
    Private CurrentSwitchGroupBox As BSGroupBox = InDo_SwitchGroupBox
    Private CurrentSimpleSwitchRButton As BSRadioButton = InDo_SwitchSimpleRButton
    Private CurrentContinuousSwitchRButton As BSRadioButton = InDo_SwitchContinuousRButton
    Private CurrentMotorAdjustControl As BSAdjustControl = InDo_Motors_AdjustControl
    Private CurrentContinuousStopButton As BSButton = InDo_StopButton


    Private NewSelectedTab As DevExpress.XtraTab.XtraTabPage

    ' XBC 10/11/2011
    Private NewSelectedTabIndex As Integer
    Private SelectedTabInvoked As Boolean
    ' XBC 10/11/2011

    ' XBC 08/11/2011
    ''arms to washing timer
    'Private ArmsToWashingTimerCallBack As System.Threading.TimerCallback
    'Private WithEvents ArmsToWashingTimer As System.Threading.Timer

    ''arms to parking timer
    'Private ArmsToParkingTimerCallBack As System.Threading.TimerCallback
    'Private WithEvents ArmsToParkingTimer As System.Threading.Timer

    ''setting default timer
    'Private SettingDefaultTimerCallBack As System.Threading.TimerCallback
    'Private WithEvents SettingDefaultTimer As System.Threading.Timer

    ''reading timer
    'Private ReadingTimerCallBack As System.Threading.TimerCallback
    'Private WithEvents ReadingTimer As System.Threading.Timer

    ''action timer
    'Private ActionTimerCallBack As System.Threading.TimerCallback
    'Private WithEvents ActionTimer As System.Threading.Timer
    ' XBC 08/11/2011

    Private StopContinuousSwitchingRequested As Boolean = False

    'system tanks level limits
    Private MinHighContaminationTankLevel As Integer
    Private MaxHighContaminationTankLevel As Integer
    Private MinDistilledWaterTankLevel As Integer
    Private MaxDistilledWaterTankLevel As Integer

    'system tanks level current values
    Private CurrentHighContaminationTankLevel As Integer
    Private CurrentDistilledWaterTankLevel As Integer

    'external tanks level current values
    Private CurrentLowContaminationTankLevel As HW_TANK_LEVEL_SENSOR_STATES
    Private CurrentSystemLiquidTankLevel As HW_TANK_LEVEL_SENSOR_STATES

    'Virtual elements

    Private WithEvents WSDisp_ValvesGroupDCUnit As New BsScadaPumpControl 'WS dispensation valves
    Private WithEvents Enco_ReactionsRotorMotor As New BsScadaMotorControl 'Reactions rotor motor
    Private Enco_ReactionsRotorEncoderPos As Integer = 0 'Reactions rotor encoder

    Private WithEvents Col_WashingStationMotor As New BsScadaMotorControl 'Reactions rotor motor

    Private CurrentHistoryArea As MotorsPumpsValvesTestDelegate.HISTORY_AREAS = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.INTERNAL_DOSING
    Private IsPendingToReportHistory As Boolean = False

    'Information
    Private SelectedInfoPanel As Panel
    Private SelectedAdjPanel As Panel


    Private IsActionNotAllowed As Boolean = False 'SGM 22/11/2011

    'SGM 22/11/2011
    Private myStopButtonToolTip As String
    Private myStartButtonToolTip As String

    Private IsWorkingAttr As Boolean = False


#End Region

#Region "Private properties"

#Region "Common"

    ''' <summary>
    ''' Property that informs if needed homes have been already performed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>SG 05/25/2011</remarks>
    Private Property AllHomesAreDone() As Boolean
        Get
            If myScreenDelegate IsNot Nothing Then
                Dim myGlobal As New GlobalDataTO
                myGlobal = myScreenDelegate.GetPendingPreliminaryHomes(ADJUSTMENT_GROUPS.MOTORS_PUMPS_VALVES)
                If Not myGlobal.HasError And myGlobal IsNot Nothing Then
                    Dim myPendingList As New List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow)
                    myPendingList = CType(myGlobal.SetDatos, List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow))
                    AllHomesAreDoneAttr = (myPendingList.Count = 0)
                Else
                    AllHomesAreDoneAttr = False
                End If
            Else
                AllHomesAreDoneAttr = False
            End If

            Return AllHomesAreDoneAttr

        End Get
        Set(ByVal value As Boolean)
            AllHomesAreDoneAttr = value
        End Set
    End Property
    Private AllHomesAreDoneAttr As Boolean = False

    ''' <summary>
    ''' Activation mode that is being carried out (SIMPLE or CONTINUOUS) 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Property CurrentActivationMode() As ACTIVATION_MODES
        Get
            Return CurrentActivationModeAttr
        End Get
        Set(ByVal value As ACTIVATION_MODES)
            If CurrentActivationModeAttr <> value Then
                CurrentActivationModeAttr = value
            End If

        End Set
    End Property
    Private CurrentActivationModeAttr As ACTIVATION_MODES = ACTIVATION_MODES._NONE

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Property IsContinuousSwitching() As Boolean
        Get
            Return IsContinuousSwitchingAttr
        End Get
        Set(ByVal value As Boolean)

            If IsContinuousSwitchingAttr <> value Then

                IsContinuousSwitchingAttr = value

                If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                    MyClass.CurrentContinuousStopButton.Enabled = value
                End If

                'If value Then
                '    MyClass.StopContinuousSwitchingRequested = False
                'End If



            End If
        End Set
    End Property
    Private IsContinuousSwitchingAttr As Boolean = False

    ''' <summary>
    ''' Defines that an Activating Action is being carried out
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Property IsActionRequested() As Boolean
        Get
            Return IsActionRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            If IsActionRequestedAttr <> value Then
                IsActionRequestedAttr = value

                If value Then
                    If Not MyBase.SimulationMode Then MyBase.myServiceMDI.SEND_INFO_STOP(MyClass.IsWorking)
                    MyClass.DisableCurrentPage(MyClass.SwitchRequestedElement)
                    MyClass.ManageTabPages = False

                Else

                    MyClass.IsManifoldRequested = False
                    MyClass.IsFluidicsRequested = False
                    MyClass.IsPhotometricsRequested = False
                    MyClass.IsSettingDefaultStates = False

                    If Not MyClass.IsContinuousSwitching Then
                        'MyClass.EnableCurrentPage()
                        MyClass.ManageTabPages = True
                        Me.Cursor = Cursors.Default
                        MyClass.RequestedNewState = BsScadaControl.States._NONE
                        MyClass.SwitchRequestedElement = Nothing
                        MyClass.myScreenDelegate.ActivationMode = SWITCH_ACTIONS._NONE

                        MyBase.myServiceMDI.SEND_INFO_START(IsWorking)
                    End If

                    'MyBase.myServiceMDI.SEND_INFO_START()

                End If
            End If
        End Set
    End Property
    Private IsActionRequestedAttr As Boolean = False

    ''' <summary>
    ''' Informs if the Manifold elements' states have been requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 03/06/2011</remarks>
    Private Property IsManifoldRequested() As Boolean
        Get
            Return IsManifoldRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            IsManifoldRequestedAttr = value

        End Set
    End Property
    Private IsManifoldRequestedAttr As Boolean = False

    ''' <summary>
    ''' Informs if the Fluidics elements' states have been requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 03/06/2011</remarks>
    Private Property IsFluidicsRequested() As Boolean
        Get
            Return IsFluidicsRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            IsFluidicsRequestedAttr = value

        End Set
    End Property
    Private IsFluidicsRequestedAttr As Boolean = False

    ''' <summary>
    ''' Informs if the Photometrics elements' states have been requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 03/06/2011</remarks>
    Private Property IsPhotometricsRequested() As Boolean
        Get
            Return IsPhotometricsRequestedAttr
        End Get
        Set(ByVal value As Boolean)
            IsPhotometricsRequestedAttr = value

        End Set
    End Property
    Private IsPhotometricsRequestedAttr As Boolean = False

    Private Property SelectedMotor() As BsScadaMotorControl
        Get
            Return SelectedMotorAttr
        End Get
        Set(ByVal value As BsScadaMotorControl)
            If value IsNot SelectedMotorAttr Then
                SelectedMotorAttr = value
            End If
        End Set
    End Property
    Private SelectedMotorAttr As BsScadaMotorControl = Nothing

    ''' <summary>
    ''' Informs if the Manifold, Fluidics or Photometrics elements' states have been requested
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 06/06/2011</remarks>
    Private ReadOnly Property IsReading() As Boolean
        Get
            Return (MyClass.IsManifoldRequested Or _
                    MyClass.IsFluidicsRequested Or _
                    MyClass.IsPhotometricsRequested)
        End Get
    End Property

    ''' <summary>
    ''' Defines for each Test page if any element has been modified or not
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Property IsAnyChanged() As Boolean
        Get
            Return IsAnyChangedAttr
        End Get
        Set(ByVal value As Boolean)
            IsAnyChangedAttr = value
        End Set
    End Property
    Private IsAnyChangedAttr As Boolean = False


    ''' <summary>
    ''' Informs if the Low Contamination Tank is not being emptied
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property IsNotLowContaminationTankEmptying() As Boolean
        Get
            Return IsNotLowContaminationTankEmptyingAttr
        End Get
        Set(ByVal value As Boolean)
            IsNotLowContaminationTankEmptyingAttr = value
        End Set
    End Property
    Private IsNotLowContaminationTankEmptyingAttr As Boolean = True 'we assume that no process is being performed

    ''' <summary>
    ''' Informs if the System Liquid Tank is not being filled
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property IsNotSystemLiquidTankFilling() As Boolean
        Get
            Return IsNotSystemLiquidTankFillingAttr
        End Get
        Set(ByVal value As Boolean)
            IsNotSystemLiquidTankFillingAttr = value
        End Set
    End Property
    Private IsNotSystemLiquidTankFillingAttr As Boolean = True 'we assume that no process is being performed

    Private Property CurrentActionStep() As ActionSteps
        Get
            Return CurrentActionStepAttr
        End Get
        Set(ByVal value As ActionSteps)

            'stop continuous
            'If value <> CurrentActionStepAttr Then
            If MyClass.IsContinuousSwitching And MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE Then
                'stop continuous switching
                If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                    MyClass.CurrentContinuousStopButton.Enabled = False
                End If
                MyClass.RequestedNewState = BsScadaControl.States._OFF
                CurrentActionStepAttr = value
                If value <> ActionSteps.ACTION_REQUESTING Then
                    MyClass.StartActionTimer()
                End If
                MyClass.IsContinuousSwitching = False
            Else
                CurrentActionStepAttr = value
            End If

            'End If

        End Set
    End Property
    Private CurrentActionStepAttr As ActionSteps = ActionSteps._NONE

#End Region

#Region "Internal Dosing"

    ''' <summary>
    ''' Defines the Source for the Internal Dosing sub-system (Air, WS or PW)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Property InterNalDosingSource() As INTERNAL_DOSING_SOURCES
        Get
            If Me.InDo_Air_Ws_3Evalve.ActivationState = BsScadaControl.States._OFF And _
               Me.InDo_AirWs_Pw_3Evalve.ActivationState = BsScadaControl.States._OFF Then

                InterNalDosingSourceAttr = INTERNAL_DOSING_SOURCES.DISTILLED_WATER

            ElseIf Me.InDo_Air_Ws_3Evalve.ActivationState = BsScadaControl.States._OFF And _
           Me.InDo_AirWs_Pw_3Evalve.ActivationState = BsScadaControl.States._ON Then

                InterNalDosingSourceAttr = INTERNAL_DOSING_SOURCES.WASHING_SOLUTION

            ElseIf Me.InDo_Air_Ws_3Evalve.ActivationState = BsScadaControl.States._ON And _
               Me.InDo_AirWs_Pw_3Evalve.ActivationState = BsScadaControl.States._ON Then

                InterNalDosingSourceAttr = INTERNAL_DOSING_SOURCES.AIR

            End If
            Return InterNalDosingSourceAttr
        End Get
        Set(ByVal value As INTERNAL_DOSING_SOURCES)
            Select Case value
                Case INTERNAL_DOSING_SOURCES.AIR
                    '31-LBL_SRV_AIR 
                    Me.InDo_SourceTypeLabel.Text = MyClass.AirCaption

                Case INTERNAL_DOSING_SOURCES.DISTILLED_WATER
                    '32-LBL_SRV_DISTILLED_WATER
                    Me.InDo_SourceTypeLabel.Text = MyClass.DistilledWaterCaption

                Case INTERNAL_DOSING_SOURCES.WASHING_SOLUTION
                    '33-LBL_SRV_WASHING_SOLUTION 
                    Me.InDo_SourceTypeLabel.Text = MyClass.WashingSolutionCaption

            End Select
            InterNalDosingSourceAttr = value
        End Set
    End Property
    Private InterNalDosingSourceAttr As INTERNAL_DOSING_SOURCES = INTERNAL_DOSING_SOURCES.WASHING_SOLUTION

    ''' <summary>
    ''' Defines the color asigned to the current fluid
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private ReadOnly Property InterNalDosingSourceFluidColor() As Color

        Get
            Select Case InterNalDosingSource
                Case INTERNAL_DOSING_SOURCES.DISTILLED_WATER
                    Return DistilledWaterFluidColor

                Case INTERNAL_DOSING_SOURCES.WASHING_SOLUTION
                    Return WashingSolutionFluidColor

                Case INTERNAL_DOSING_SOURCES.AIR
                    Return AirFluidColor

            End Select


        End Get
    End Property
#End Region

#Region "Collision "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 23/11/2011</remarks>
    Private Property IsCollisionTestEnabled() As Boolean
        Get
            Return IsCollisionTestEnabledAttr
        End Get
        Set(ByVal value As Boolean)

            IsCollisionTestEnabledAttr = value

            Try
                Dim myGlobal As New GlobalDataTO
                Dim auxIconName As String = String.Empty
                Dim iconPath As String = MyBase.IconsPath
                Dim myStartStopButtonImage As Image
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                If value Then
                    auxIconName = GetIconName("STOP")
                    Dim myStopImage As Image
                    If System.IO.File.Exists(iconPath & auxIconName) Then

                        Dim myImage As Image
                        myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                        myGlobal = Utilities.ResizeImage(myImage, New Size(26, 26))
                        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                            myStopImage = CType(myGlobal.SetDatos, Bitmap)
                        Else
                            myStopImage = CType(myImage, Bitmap)
                        End If

                    Else
                        myStopImage = Nothing
                    End If

                    myStartStopButtonImage = myStopImage
                    bsScreenToolTips2.SetToolTip(Col_StartStopButton, MyClass.myStopButtonToolTip)

                    Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status._ON
                    Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status._ON
                    Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status._ON

                    Me.ManageTabPages = False

                    MyClass.ActivateMDIMenusButtons(False)
                    Me.BsExitButton.Enabled = False
                    Me.Cursor = Cursors.Default

                    'SGM 25/10/2012
                    Me.bsScreenToolTips2.SetToolTip(Col_StartStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", MyClass.LanguageID))
                    Me.Col_StartStopButton.Enabled = True
                Else
                    auxIconName = GetIconName("ADJUSTMENT")
                    Dim myStartImage As Image
                    If System.IO.File.Exists(iconPath & auxIconName) Then

                        Dim myImage As Image
                        myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                        myGlobal = Utilities.ResizeImage(myImage, New Size(26, 26))
                        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                            myStartImage = CType(myGlobal.SetDatos, Bitmap)
                        Else
                            myStartImage = CType(myImage, Bitmap)
                        End If

                    Else
                        myStartImage = Nothing
                    End If

                    myStartStopButtonImage = myStartImage
                    bsScreenToolTips2.SetToolTip(Col_StartStopButton, MyClass.myStartButtonToolTip)

                    Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                    Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                    Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status.DISABLED

                    If MyClass.CurrentTestPanel Is BsCollisionTestPanel Then
                        Me.ManageTabPages = True
                        Me.BsExitButton.Enabled = True
                    End If

                    'SGM 25/10/2012
                    Me.bsScreenToolTips2.SetToolTip(Col_StartStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", MyClass.LanguageID))
                    Me.Col_StartStopButton.Enabled = True
                End If

                If myStartStopButtonImage IsNot Nothing Then
                    Col_StartStopButton.Image = myStartStopButtonImage
                    'Col_StartStopButton.BackgroundImageLayout = ImageLayout.Center
                End If

            Catch ex As Exception
                Throw ex
            End Try

        End Set
    End Property

    'SGM 10/10/2012
    Private Property CollidedNeedle() As GlobalEnumerates.UTILCollidedNeedles
        Get
            Return CollidedNeedleAttr
        End Get
        Set(ByVal value As GlobalEnumerates.UTILCollidedNeedles)
            MyClass.CollidedNeedleAttr = value

            If value <> UTILCollidedNeedles.None Then

                Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status._ON
                Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status._ON
                Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status._ON

                Select Case value
                    Case UTILCollidedNeedles.DR1 : Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status._OFF
                    Case UTILCollidedNeedles.DR2 : Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status._OFF
                    Case UTILCollidedNeedles.DM1 : Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status._OFF
                End Select

            End If
        End Set
    End Property
#End Region

#Region "Encoder - NOT V1"

    ''' <summary>
    ''' Informs if the Reactions Rotor encoder does not count properly the steps
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 9/06/2011</remarks>
    Private Property IsReactionsRotorEncoderError() As Boolean
        Get
            Return IsReactionsRotorEncoderErrorAttr
        End Get
        Set(ByVal value As Boolean)
            IsReactionsRotorEncoderErrorAttr = value
            If value Then

            Else

            End If
        End Set
    End Property
    Private IsReactionsRotorEncoderErrorAttr As Boolean = False  'Reactions rotor encoder diagnosis


#End Region

#End Region

#Region "Public properties"
    Public ReadOnly Property IsReadyToClose() As Boolean
        Get
            Return IsReadyToCloseAttr
        End Get
    End Property

#End Region

#Region "Private properties"

    Private Property IsWorking() As Boolean
        Get
            Return IsWorkingAttr
        End Get
        Set(ByVal value As Boolean)
            If IsWorkingAttr <> value Then
                IsWorkingAttr = value

                'Me.bsInternalDosingTabPage.PageEnabled = Not value
                'Me.bsExternalWashingTabPage.PageEnabled = Not value
                'Me.bsAspirationTabPage.PageEnabled = Not value
                'Me.bsDispensationTabPage.PageEnabled = Not value
                'Me.bsInOutTabPage.PageEnabled = Not value
                'Me.bsCollisionTabPage.PageEnabled = Not value
                'Me.BsEncoderTabPage.PageEnabled = Not value

                Me.ManageTabPages = Not value
                Me.BsExitButton.Enabled = Not value 'SGM 23/10/2012
            End If
        End Set
    End Property

    'Analyzer Alarm has been detected
    Private CurrentAlarmTypeAttr As ManagementAlarmTypes = ManagementAlarmTypes.NONE
    Private Property CurrentAlarmType() As ManagementAlarmTypes
        Get
            Return CurrentAlarmTypeAttr
        End Get
        Set(ByVal value As ManagementAlarmTypes)
            CurrentAlarmTypeAttr = value
        End Set
    End Property

#End Region

#Region "Enumerations"

    Private Enum ActionSteps
        _NONE = 0
        PREVIOUS_READING = 1 'initial reading
        PREVIOUS_CHECKING = 2 'previous check before action
        ACTION_REQUESTING = 3 'action request
        FINAL_READING = 4 'final reading after action
        ACTION_REJECTED = 5 'rejected because of other elements' status
    End Enum

    Private Enum TEST_PAGES
        INTERNAL_DOSING
        EXTERNAL_WASHING
        WS_ASPIRATION
        WS_DISPENSATION
        IN_OUT
        COLLISION_DETECTION
        ENCODER_TEST
    End Enum

    Private Enum ACTIVATION_MODES
        _NONE
        SIMPLE
        CONTINUOUS
    End Enum

    Private Enum INTERNAL_DOSING_SOURCES
        AIR
        WASHING_SOLUTION
        DISTILLED_WATER
    End Enum

    Private Enum ItemGroups
        _NONE
        MANIFOLD
        FLUIDICS
        PHOTOMETRICS
    End Enum

    Public Enum ItemTypes
        _NONE
        VALVE
        VALVE3
        PUMP
        MOTOR
    End Enum

    Private Enum Arms
        _NONE
        SAMPLES
        REAGENT1
        REAGENT2
        MIXER1
        MIXER2
    End Enum
#End Region

#Region "Initializations"

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 19/04/2011</remarks>
    Private Sub PrepareButtons()

        Dim myGlobal As New GlobalDataTO
        'Dim Utilities As New Utilities
        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath

        Try

            Me.BsExitButton.Visible = True
            Me.BsCancelButton.Visible = True
            Me.BsSaveButton.Visible = False
            Me.BsAdjustButton.Visible = False
            Me.Col_StartStopButton.Visible = True


            MyBase.SetButtonImage(BsExitButton, "CANCEL")
            MyBase.SetButtonImage(BsCancelButton, "HOME")
            'MyBase.SetButtonImage(Col_StartButton, "ADJUSTMENT")
            MyBase.SetButtonImage(InDo_StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(ExWa_StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(WSAsp_StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(WsDisp_StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(InOut_StopButton, "STOP", 24, 24)
            MyBase.SetButtonImage(Col_StartStopButton, "ADJUSTMENT", 24, 24)

            'SGM 18/05/2012
            MyBase.SetButtonImage(WSAsp_UpDownButton, "UPDOWN")
            MyBase.SetButtonImage(WSDisp_UpDownButton, "UPDOWN")


            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    'BsExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

            ''CANCEL Button
            'auxIconName = GetIconName("HOME")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            '    'BsCancelButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

            ''START STOP Collision Test Button
            'bsScreenToolTips.SetToolTip(Col_StartStopButton, MyClass.myStartButtonToolTip)
            'auxIconName = GetIconName("ADJUSTMENT")
            'Dim myStartImage As Image
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image
            '    myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            '    myGlobal = Utilities.ResizeImage(myImage, New Size(28, 28))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myStartImage = CType(myGlobal.SetDatos, Bitmap)
            '    Else
            '        myStartImage = CType(myImage, Bitmap)
            '    End If

            '    Col_StartStopButton.Image = myStartImage
            '    'Col_StartStopButton.BackgroundImageLayout = ImageLayout.Center

            'Else
            '    myStartImage = Nothing
            'End If




            ''STOP Buttons
            'auxIconName = GetIconName("STOP")
            'Dim myStopImage As Image
            'If System.IO.File.Exists(iconPath & auxIconName) Then

            '    Dim myImage As Image
            '    myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            '    myGlobal = Utilities.ResizeImage(myImage, New Size(26, 26))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myStopImage = CType(myGlobal.SetDatos, Bitmap)
            '    Else
            '        myStopImage = CType(myImage, Bitmap)
            '    End If


            '    InDo_StopButton.Image = myStopImage
            '    'InDo_StopButton.BackgroundImageLayout = ImageLayout.Center

            '    ExWa_StopButton.Image = myStopImage
            '    'ExWa_StopButton.BackgroundImageLayout = ImageLayout.Center

            '    WSAsp_StopButton.Image = myStopImage
            '    'WSAsp_StopButton.BackgroundImageLayout = ImageLayout.Center

            '    WsDisp_StopButton.Image = myStopImage
            '    'WsDisp_StopButton.BackgroundImageLayout = ImageLayout.Center

            '    InOut_StopButton.Image = myStopImage
            '    'InOut_StopButton.BackgroundImageLayout = ImageLayout.Center

            '    Col_StartStopButton.Image = myStopImage
            '    'Col_StartStopButton.BackgroundImageLayout = ImageLayout.Center

            'Else
            '    myStopImage = Nothing
            'End If




        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 19/04/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            '
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Common
            Me.BsTabPagesControl.TabPages(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_InternalDosing", pLanguageID)
            Me.BsTabPagesControl.TabPages(1).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ExternalWashing", pLanguageID)
            Me.BsTabPagesControl.TabPages(2).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSAspiration", pLanguageID)
            Me.BsTabPagesControl.TabPages(3).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSDispensation", pLanguageID)
            Me.BsTabPagesControl.TabPages(4).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_InOut", pLanguageID)
            Me.BsTabPagesControl.TabPages(5).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CollisionTest", pLanguageID)
            Me.BsTabPagesControl.TabPages(6).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_EncoderTest", pLanguageID)

            Me.BsInternalDosingTestTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_InternalDosingTitle", pLanguageID)
            Me.BsExternalWashingTestTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ExternalWashingTitle", pLanguageID)
            Me.BsWSAspirationTestTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSAspirationTitle", pLanguageID)
            Me.BsWSDispensationTestTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WSDispensationTitle", pLanguageID)
            Me.BsInOutTestTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FLUIDS_IN_OUT", pLanguageID)

            Me.InDo_SourceGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CUR_DOSING_SOURCE", pLanguageID)

            MyClass.ValveCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_VALVE", pLanguageID)
            MyClass.Valve3Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_3WAYVALVE", pLanguageID)
            MyClass.PumpCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_PUMP", pLanguageID)
            MyClass.MotorCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DC_MOTOR", pLanguageID)


            'INTERNAL DOSING
            Me.BsInternalDosingInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.InDo_SwitchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Switch", pLanguageID)
            Me.InDo_SwitchSimpleRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SimpleSwitch", pLanguageID)
            Me.InDo_SwitchContinuousRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ContinuousSwitch", pLanguageID)
            MyClass.WashingSolutionCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashSolution", pLanguageID)
            MyClass.DistilledWaterCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DISTILLED_WATER", pLanguageID)
            MyClass.AirCaption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AIR", pLanguageID)
            Me.InDo_WashingSolutionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashSolution", pLanguageID)
            Me.InDo_DistilledWaterLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DISTILLED_WATER", pLanguageID)
            Me.InDo_AirLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AIR", pLanguageID)
            Me.InDo_SamplesNeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SamplesNeedle", pLanguageID)
            Me.InDo_Reagent1NeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Needle", pLanguageID)
            Me.InDo_Reagent2NeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Needle", pLanguageID)
            Me.InDo_Motors_AdjustControl.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"

            'EXTERNAL WASHING
            Me.BsExternalWashingInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.ExWa_SwitchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Switch", pLanguageID)
            Me.ExWa_SwitchSimpleRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SimpleSwitch", pLanguageID)
            Me.ExWa_SwitchContinuousRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ContinuousSwitch", pLanguageID)
            Me.ExWa_DistilledWaterLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DISTILLED_WATER", pLanguageID)

            If AnalyzerController.Instance.IsBA200 Then
                Me.ExWa_SamplesNeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SampleAndReagent", pLanguageID)
                Me.ExWa_Reagent1NeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Mixer1", pLanguageID)
                'Hide Pump B3
                Me.ExWa_Reagent2NeedleLabel.Visible = False
                Me.ExWa_Reagent2_WS.Visible = False
                Me.BsScadaPipeControl21.Visible = False
                Me.ExWa_Reagent2_Pump.Visible = False
                Me.ExWa_B3_Label.Visible = False
                Me.BsScadaPipeControl20.Visible = False
            Else
                Me.ExWa_SamplesNeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Sample", pLanguageID)
                Me.ExWa_Reagent1NeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1Mixer2Arms", pLanguageID)
                Me.ExWa_Reagent2NeedleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2Mixer1Arms", pLanguageID)
            End If
            


            'WS ASPIRATION
            Me.BsWSAspirationInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.WSAsp_SwitchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Switch", pLanguageID)
            Me.WsAsp_SwitchSimpleRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SimpleSwitch", pLanguageID)
            Me.WsAsp_SwitchContinuousRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ContinuousSwitch", pLanguageID)
            Me.WsAsp_HCLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_HighContaminationTank", pLanguageID)
            Me.WsAsp_LCLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LowContaminationTank", pLanguageID)

            'Needle Labels numbering of wash station aspiration tab are different depending of Model
            Select Case AnalyzerController.Instance.Analyzer.Model
                Case AnalyzerModelEnum.A200.ToString
                    Me.WSAsp_Needle1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 6"
                    Me.WSAsp_Needle23Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 7,8"
                    Me.WSAsp_Needle45Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 9,10"
                    'Needle 11 is for level detection
                    Me.WSAsp_Needle6Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 12"
                    Me.WSAsp_Needle7Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 13"
                    'Pumps Name are Labeled BxF
                    Me.WsAsp_B6_Label.Text = "B6F"
                    Me.WsAsp_B7_Label.Text = "B7F"
                    Me.WsAsp_B8_Label.Text = "B8F"
                    Me.WsAsp_B9_Label.Text = "B9F"
                    Me.WsAsp_B10_Label.Text = "B10F"
                    Me.WsAsp_B10_Label.Width = 55
                Case Else
                    Me.WSAsp_Needle1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 7" ' " 1" ' XBC 18/09/2012 - spec change
                    Me.WSAsp_Needle23Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 8,9" '" 2,3" ' XBC 18/09/2012 - spec change
                    Me.WSAsp_Needle45Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 10,11" ' " 4,5" ' XBC 18/09/2012 - spec change
                    Me.WSAsp_Needle6Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 12" ' " 6" ' XBC 18/09/2012 - spec change
                    Me.WSAsp_Needle7Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 13" ' " 7" ' XBC 18/09/2012 - spec change
            End Select

    'WS DISPENSATION
            Me.BsWSDispensationInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.WsDisp_SwitchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Switch", pLanguageID)
            Me.WsDisp_SwitchSimpleRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SimpleSwitch", pLanguageID)
            Me.WsDisp_SwitchContinuousRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ContinuousSwitch", pLanguageID)
            Me.WSDisp_DistilledWaterLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DISTILLED_WATER", pLanguageID)
            Me.WSDisp_WashingSolutionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashSolution", pLanguageID)
            Me.WSDisp_Needle1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 1"
            Me.WSDisp_Needle2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 2"
            Me.WSDisp_Needle3Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 3"
            Me.WSDisp_Needle4Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 4"
            Me.WSDisp_Needle5Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID) + " 5"
            Me.WSDisp_MotorAdjustControl.RangeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Range", pLanguageID) & ":"

    'IN OUT
            Me.BsInOutInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.InOut_SwitchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Switch", pLanguageID)
            Me.InOut_SwitchSimpleRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SimpleSwitch", pLanguageID)
            Me.InOut_SwitchContinuousRButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ContinuousSwitch", pLanguageID)
            Me.InOut_LC_InternalTankLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LowContaminationTank", pLanguageID)
            Me.InOut_PW_InternalTankLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DistilledWaterTank", pLanguageID)
            Me.InOut_LC_ExternalTankLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_LowContaminationExtTank", pLanguageID)
            Me.InOut_PW_ExternalTankLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_DistilledWaterExtTank", pLanguageID)
            Me.InOut_ExternalSourceLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_EXTERNAL_SOURCE", pLanguageID)

    'COLLISION
            Me.BsCollisionInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.Col_WashingStationLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashStation", pLanguageID)
            Me.Col_SamplesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SAMPLE", pLanguageID) + " " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID)
            Me.Col_Reagent1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1", pLanguageID) + " " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID)
            Me.Col_Reagent2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2", pLanguageID) + " " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID)

    'ENCODER 'PDT
            Me.BsEncoderInfoTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", pLanguageID)
            Me.Enco_MotorCaptionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_EncoderTestMotor", pLanguageID)
            Me.Enco_EncoderCaptionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_EncoderTestEncoder", pLanguageID) + " " + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Needle", pLanguageID)

    'LBL_SRV_CollisionTest
    'LBL_SRV_EncoderTest
    'LBL_SRV_EncoderTestMotor
    'LBL_SRV_EncoderTestEncoder

    ' Tooltips
            GetScreenTooltip(pLanguageID)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: XBC 26/07/11
    ''' </remarks>
    Private Sub GetScreenTooltip(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            bsScreenToolTips2.SetToolTip(InDo_StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))
            bsScreenToolTips2.SetToolTip(ExWa_StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))
            bsScreenToolTips2.SetToolTip(WSAsp_StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))
            bsScreenToolTips2.SetToolTip(WsDisp_StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))
            bsScreenToolTips2.SetToolTip(InOut_StopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID))
            bsScreenToolTips2.SetToolTip(Col_StartStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStart", pLanguageID))

            bsScreenToolTips2.SetToolTip(Col_StartStopButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", pLanguageID))

            bsScreenToolTips2.SetToolTip(BsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Homes", pLanguageID))
            bsScreenToolTips2.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))

            MyClass.myStartButtonToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStart", pLanguageID)
            MyClass.myStopButtonToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", pLanguageID)

            'SGM 18/05/2012
            MyBase.bsScreenToolTipsControl.SetToolTip(WSAsp_UpDownButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", pLanguageID))
            MyBase.bsScreenToolTipsControl.SetToolTip(WSDisp_UpDownButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_UPDOWN_WS", pLanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 23/05/11</remarks>
    Private Function InitializeHomes() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.ResetAllPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeCommonObjects()
        Try
            Me.BsTabPagesControl.SelectedTabPageIndex = 0
            MyClass.CurrentTestPanel = Me.BsInternalDosingTestPanel
            MyClass.CurrentSwitchGroupBox = Me.InDo_SwitchGroupBox
            MyClass.CurrentSimpleSwitchRButton = Me.InDo_SwitchSimpleRButton
            MyClass.CurrentContinuousSwitchRButton = Me.InDo_SwitchContinuousRButton
            MyClass.CurrentSimpleSwitchRButton.Checked = True
            MyClass.CurrentContinuousSwitchRButton.Checked = False
            MyClass.CurrentMotorAdjustControl = Me.InDo_Motors_AdjustControl

            ' XBC 08/11/2011 - No initial absolute positions, instead of this, execute homes
            ''default motor positions 'PDT
            'With MyClass.myScreenDelegate
            '    .SamplesMotorDefaultPosition = 1000
            '    .Reagent1MotorDefaultPosition = 1000
            '    .Reagent2MotorDefaultPosition = 1000
            '    .WashingStationMotorDefaultPosition = 1000
            '    .DispensationMotorDefaultPosition = 1000
            '    .ReactionsRotorMotorDefaultPosition = 1000
            'End With
            myScreenDelegate.ReadAnalyzerInfoDone = False
            ' XBC 08/11/2011 

            Me.myTooltip = New ToolTip(Me.components)
            Me.myTooltip.Active = True
            Me.myTooltip.ShowAlways = True


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeCommonObjects ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeCommonObjects ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeInternalDosing()
        Try

            'Syringes
            With InDo_Samples_Syringe
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With
            With InDo_Reagent1_Syringe
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With

            With InDo_Reagent2_Syringe
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With

            'Motors
            With InDo_Samples_Motor
                .Identity = MANIFOLD_ELEMENTS.JE1_MS.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .CurrentPosition = 1542 'for simulation
                .Cursor = Cursors.Hand
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Samples_Motor)

            With InDo_Reagent1_Motor
                .Identity = MANIFOLD_ELEMENTS.JE1_MR1.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .CurrentPosition = 1009 'for simulation
                .Cursor = Cursors.Hand
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent1_Motor)


            With InDo_Reagent2_Motor
                .Identity = MANIFOLD_ELEMENTS.JE1_MR2.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .CurrentPosition = 897 'for simulation
                .Cursor = Cursors.Hand
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent2_Motor)

            'Valves
            With InDo_Samples_EValve
                .Identity = MANIFOLD_ELEMENTS.JE1_EV1.ToString
                .Selected = False
                .WayType = BsScadaValveControl.WayTypes.Normally_Closed
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Samples_EValve)

            With InDo_Reagent1_EValve
                .Identity = MANIFOLD_ELEMENTS.JE1_EV2.ToString
                .Selected = False
                .WayType = BsScadaValveControl.WayTypes.Normally_Closed
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent1_EValve)

            With InDo_Reagent2_EValve
                .Identity = MANIFOLD_ELEMENTS.JE1_EV3.ToString
                .Selected = False
                .WayType = BsScadaValveControl.WayTypes.Normally_Closed
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent2_EValve)

            'Pumps
            With InDo_Samples_Pump
                .Identity = MANIFOLD_ELEMENTS.JE1_B1.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Samples_Pump)

            With InDo_Reagent1_Pump
                .Identity = MANIFOLD_ELEMENTS.JE1_B2.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent1_Pump)

            With InDo_Reagent2_Pump
                .Identity = MANIFOLD_ELEMENTS.JE1_B3.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Reagent2_Pump)

            'Valves 3 ways
            With InDo_Air_Ws_3Evalve
                .Identity = MANIFOLD_ELEMENTS.JE1_EV4.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._ON
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_1_To_2
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .ActivatorVisible = False
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_Air_Ws_3Evalve)

            With InDo_AirWs_Pw_3Evalve
                .Identity = MANIFOLD_ELEMENTS.JE1_EV5.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._ON
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_1_To_2
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_1
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .ActivatorVisible = False
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InDo_AirWs_Pw_3Evalve)

            'Pipes
            For Each C As Control In Me.BsInternalDosingTestPanel.Controls
                If TypeOf C Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        'myPipe.Cursor = Cursors.Default
                        myPipe.FluidColor = Color.WhiteSmoke
                        myPipe.Refresh()
                    End If
                End If
            Next

            'for simulation mode only, the motors are initially activated
            If MyBase.SimulationMode Then
                Me.InDo_Samples_Motor.ActivationState = BsScadaControl.States._ON
                Me.InDo_Reagent1_Motor.ActivationState = BsScadaControl.States._ON
                Me.InDo_Reagent2_Motor.ActivationState = BsScadaControl.States._ON
            End If

            Me.InDo_Motors_AdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.UpDown




            Me.InDo_StopButton.Enabled = False

            'Motors
            Me.InDo_Motors_AdjustControl.MinimumLimit = 0
            Me.InDo_Motors_AdjustControl.MaximumLimit = 10000
            Me.InDo_Motors_AdjustControl.IncreaseMode = BSAdjustControl.IncreaseModes.Inverse

            'Get Motors Limits
            Dim myGlobal As New GlobalDataTO
            Dim myFieldLimitsDS As New FieldLimitsDS
            myGlobal = GetMotorsLimits(FieldLimitsEnum.SRV_INDO_DC_PUMP)
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    Me.InDo_Motors_AdjustControl.MinimumLimit = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    Me.InDo_Motors_AdjustControl.MaximumLimit = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeInternalDosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeInternalDosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeExternalWashing()
        Dim ToolTipTextForBA200 As String = ""
        Try

            'Syringes
            With ExWa_Samples_WS
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With

            With ExWa_Reagent1_WS
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With

            With ExWa_Reagent2_WS
                .Selected = False
                .FluidColor = Color.Gainsboro
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
            End With
            If AnalyzerController.Instance.Analyzer.Model = AnalyzerModelEnum.A200.ToString Then
                ToolTipTextForBA200 = "_BA200"
            End If

            'Pumps
            With ExWa_Samples_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B1.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity & ToolTipTextForBA200, LanguageID)
            End With
            TestableElements.Add(ExWa_Samples_Pump)

            With ExWa_Reagent1_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B2.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity & ToolTipTextForBA200, LanguageID)
            End With
            TestableElements.Add(ExWa_Reagent1_Pump)

            With ExWa_Reagent2_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B3.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(ExWa_Reagent2_Pump)

            'Pipes
            For Each C As Control In BsExternalWashingTestPanel.Controls
                If TypeOf C Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        'myPipe.Cursor = Cursors.Default
                        myPipe.FluidColor = Color.WhiteSmoke
                        myPipe.Refresh()
                    End If
                End If
            Next

            Me.ExWa_StopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeExternalWashing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeExternalWashing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeWSAspiration()
        Dim ToolTipText As String
        Dim DefaultToolTipText As String
        Try
            DefaultToolTipText = MyBase.GetHWElementsName("SF1_BX_FOR_TIPS", LanguageID)

            'Pumps
            With WsAsp_B6_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B6.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                Select Case AnalyzerController.Instance.Analyzer.Model
                    Case AnalyzerModelEnum.A200.ToString
                        ToolTipText = DefaultToolTipText & " 7,8"
                    Case Else
                        ToolTipText = DefaultToolTipText & " 8,9"
                End Select
                .ToolTipText = ToolTipText
            End With
            TestableElements.Add(WsAsp_B6_Pump)

            With WsAsp_B7_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B7.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                Select Case AnalyzerController.Instance.Analyzer.Model
                    Case AnalyzerModelEnum.A200.ToString
                        ToolTipText = DefaultToolTipText & " 9,10"
                    Case Else
                        ToolTipText = DefaultToolTipText & " 10,11"
                End Select
                .ToolTipText = ToolTipText
            End With
            TestableElements.Add(WsAsp_B7_Pump)

            With WsAsp_B8_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B8.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                ToolTipText = DefaultToolTipText & " 12"
                .ToolTipText = ToolTipText
            End With
            TestableElements.Add(WsAsp_B8_Pump)

            With WsAsp_B9_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B9.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                ToolTipText = DefaultToolTipText & " 13"
                .ToolTipText = ToolTipText
            End With
            TestableElements.Add(WsAsp_B9_Pump)

            With WsAsp_B10_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B10.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                Select Case AnalyzerController.Instance.Analyzer.Model
                    Case AnalyzerModelEnum.A200.ToString
                        ToolTipText = DefaultToolTipText & " 6"
                    Case Else
                        ToolTipText = DefaultToolTipText & " 7"
                End Select
                .ToolTipText = ToolTipText
            End With
            TestableElements.Add(WsAsp_B10_Pump)

            'Pipes
            For Each C As Control In BsWSAspirationTestPanel.Controls
                If TypeOf C Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        'myPipe.Cursor = Cursors.Default
                        myPipe.FluidColor = Color.WhiteSmoke
                        myPipe.Refresh()
                    End If
                End If
            Next

            Me.BsWSAspirationPipe1.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe2.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe3.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe4.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe5.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe6.InnerColor = Color.DimGray
            Me.BsWSAspirationPipe7.InnerColor = Color.DimGray

            Me.WSAsp_StopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeWSAspiration ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeWSAspiration ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeWSDispensation()
        Try

            'Plungers
            WsDisp_Plunger1Panel_On.Visible = False
            WsDisp_Plunger1Panel_Off.Visible = True

            WsDisp_Plunger2Panel_On.Visible = False
            WsDisp_Plunger2Panel_Off.Visible = True

            WsDisp_Plunger3Panel_On.Visible = False
            WsDisp_Plunger3Panel_Off.Visible = True

            WsDisp_Plunger4Panel_On.Visible = False
            WsDisp_Plunger4Panel_Off.Visible = True

            WsDisp_Plunger5Panel_On.Visible = False
            WsDisp_Plunger5Panel_Off.Visible = True



            'Motors
            With WsDisp_M1_Motor
                .Identity = FLUIDICS_ELEMENTS.SF1_MS.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .CurrentPosition = 782 'for simulation
                .Cursor = Cursors.Hand
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(WsDisp_M1_Motor)

            'valves group virtual pump
            WSDisp_ValvesGroupDCUnit = New BsScadaPumpControl
            With WSDisp_ValvesGroupDCUnit
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .DutyValue = 100
                .ProgressiveStart = 1
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With

            'Valves3
            With WsDisp_GE1_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_0_To_1
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .ActivatorVisible = False
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(FLUIDICS_ELEMENTS.SF1_GE1.ToString, LanguageID)
            End With
            TestableElements.Add(WsDisp_GE1_Valve)


            With WsDisp_GE2_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_0_To_1
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .ActivatorVisible = False
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(FLUIDICS_ELEMENTS.SF1_GE1.ToString, LanguageID)
            End With
            TestableElements.Add(WsDisp_GE2_Valve)


            With WsDisp_GE3_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_0_To_1
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .ActivatorVisible = False
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(FLUIDICS_ELEMENTS.SF1_GE1.ToString, LanguageID)
            End With
            TestableElements.Add(WsDisp_GE3_Valve)


            With WsDisp_GE4_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_0_To_1
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .ActivatorVisible = False
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(FLUIDICS_ELEMENTS.SF1_GE1.ToString, LanguageID)
            End With
            TestableElements.Add(WsDisp_GE4_Valve)


            With WsDisp_GE5_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_GE1.ToString
                .Selected = False
                .Valve3StateWhenOFF = BsScadaValve3Control.Valve3States.From_0_To_1
                .Valve3StateWhenON = BsScadaValve3Control.Valve3States.From_0_To_2
                .ActivationState = BsScadaControl.States._OFF
                .ActivatorVisible = False
                .Cursor = Cursors.Hand
                .Enabled = False
                .ToolTipText = MyBase.GetHWElementsName(FLUIDICS_ELEMENTS.SF1_GE1.ToString, LanguageID)
            End With
            TestableElements.Add(WsDisp_GE5_Valve)



            'Pipes
            For Each C As Control In BsWSDispensationTestPanel.Controls
                If TypeOf C Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        'myPipe.Cursor = Cursors.Default
                        myPipe.FluidColor = Color.WhiteSmoke
                        myPipe.Refresh()
                    End If
                End If
            Next

            Me.BsWSDispensationPipe1.InnerColor = Color.DimGray
            Me.BsWSDispensationPipe2.InnerColor = Color.DimGray
            Me.BsWSDispensationPipe3.InnerColor = Color.DimGray
            Me.BsWSDispensationPipe4.InnerColor = Color.DimGray
            Me.BsWSDispensationPipe5.InnerColor = Color.DimGray

            'for simulation mode only, the motors are initially activated
            If MyBase.SimulationMode Then
                Me.WsDisp_M1_Motor.ActivationState = BsScadaControl.States._ON
            End If

            Me.WSDisp_MotorAdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.LeftRight

            'PENDING TO GET FROM PARAMETERS (SRV_WSDISP_DC_PUMP)
            Me.WSDisp_MotorAdjustControl.MinimumLimit = 0
            Me.WSDisp_MotorAdjustControl.MaximumLimit = 1500
            Me.WSDisp_MotorAdjustControl.IncreaseMode = BSAdjustControl.IncreaseModes.Inverse

            'Get Motors Limits
            Dim myGlobal As New GlobalDataTO
            Dim myFieldLimitsDS As New FieldLimitsDS
            myGlobal = GetMotorsLimits(FieldLimitsEnum.SRV_WSDISP_DC_PUMP)
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    Me.WSDisp_MotorAdjustControl.MinimumLimit = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    Me.WSDisp_MotorAdjustControl.MaximumLimit = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                End If
            End If


            Me.WsDisp_StopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeWSDispensation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeWSDispensation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeInOut()
        Try

            'Valves
            With InOut_PWTank_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_EV2.ToString
                .Selected = False
                .WayType = BsScadaValveControl.WayTypes.Normally_Closed
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .BackColor = Color.LightGray
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InOut_PWTank_Valve)

            With InOut_PWSource_Valve
                .Identity = FLUIDICS_ELEMENTS.SF1_EV1.ToString
                .Selected = False
                .WayType = BsScadaValveControl.WayTypes.Normally_Closed
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .BackColor = Color.LightGray
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InOut_PWSource_Valve)

            'Pumps
            With InOut_PW_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B4.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .BackColor = Color.LightGray
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InOut_PW_Pump)

            With InOut_LC_Pump
                .Identity = FLUIDICS_ELEMENTS.SF1_B5.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .Cursor = Cursors.Hand
                .Enabled = False
                .BackColor = Color.LightGray
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With
            TestableElements.Add(InOut_LC_Pump)

            'Pipes
            For Each C As Control In BsInOutTestPanel.Controls
                If TypeOf C Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        'myPipe.Cursor = Cursors.Default
                        myPipe.FluidColor = Color.WhiteSmoke
                        myPipe.Refresh()
                    End If
                End If
            Next

            Me.InOut_StopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeInOut ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeInOut ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InitializeCollision()
        Try

            'Me.Col_StartButton.Enabled = True
            Me.Col_StartStopButton.Enabled = True

            MyClass.IsCollisionTestEnabled = False

            'LEDS
            With Col_SamplesLED
                .CurrentStatus = BSMonitorControlBase.Status._ON
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With
            With Col_Reagent1LED
                .CurrentStatus = BSMonitorControlBase.Status._ON
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With
            With Col_Reagent2LED
                .CurrentStatus = BSMonitorControlBase.Status._ON
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With

            'WS NOT FOR TESTING
            Me.Col_WashingStationGroupBox.Visible = False
            'With Col_WashingStationLED
            '    .CurrentStatus = BSMonitorControlBase.Status._ON
            '    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            'End With

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeCollision ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeCollision ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub InitializeEncoder()
        Try
            With Enco_ReactionsRotorMotor
                .Identity = PHOTOMETRICS_ELEMENTS.GLF_MR.ToString
                .Selected = False
                .ActivationState = BsScadaControl.States._OFF
                .CurrentPosition = 1542 'for simulation
                .Cursor = Cursors.Hand
                .ToolTipText = MyBase.GetHWElementsName(.Identity, LanguageID)
            End With

            Me.Enco_EncoderPosLabel.Text = "1542"
            TestableElements.Add(Me.Enco_ReactionsRotorMotor)

            'MyBase.DisplayMessage(Messages.SRV_ENCODER_TEST_READY.ToString)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeEncoder ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeEncoder ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Private Methods"

#Region "Common"

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 20/04/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel
                .ButtonsPanel.Container = Me.BsButtonsPanel
                .ButtonsPanel.ExitButton = Me.BsExitButton
                .ButtonsPanel.CancelButton = Me.BsCancelButton
            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Returns the value of the specified tank level
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 28/05/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadTanksAdjustmentData(ByVal pGroupID As ADJUSTMENT_GROUPS, ByVal pAxis As AXIS) As Integer
        Dim myGlobal As New GlobalDataTO

        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "_NONE" Then myAxis = ""

            Dim myGroup As String = pGroupID.ToString
            If myGroup = "_NONE" Then myGroup = ""

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyBase.myAllAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.GroupID.Trim = myGroup.Trim _
                                And a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.GroupID.Trim.ToUpper = myGroup.Trim.ToUpper _
            'And a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                Return CInt(myAdjustmentRows(0).Value)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadTanksAdjustmentData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadTanksAdjustmentData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

    ''' <summary>
    '''Updates the values of the tank levels
    ''' </summary>
    ''' <remarks>SGM 28/05/11</remarks>
    Private Sub UpdateCurrentExternalTankLevels(ByVal pElement As String, ByVal pValue As String)

        Try
            Dim myLevel As HW_TANK_LEVEL_SENSOR_STATES
            Select Case CInt(pValue)

                Case CInt(HW_TANK_LEVEL_SENSOR_STATES.EMPTY)
                    myLevel = HW_TANK_LEVEL_SENSOR_STATES.EMPTY

                Case CInt(HW_TANK_LEVEL_SENSOR_STATES.MIDDLE)
                    myLevel = HW_TANK_LEVEL_SENSOR_STATES.MIDDLE

                Case CInt(HW_TANK_LEVEL_SENSOR_STATES.FULL)
                    myLevel = HW_TANK_LEVEL_SENSOR_STATES.FULL

                Case Else

            End Select
            Select Case pElement
                Case FLUIDICS_ELEMENTS.SF1_WAS.ToString
                    CurrentLowContaminationTankLevel = myLevel

                Case FLUIDICS_ELEMENTS.SF1_SLS.ToString
                    CurrentSystemLiquidTankLevel = myLevel
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateCurrentExternalTankLevels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateCurrentExternalTankLevels ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enables the current page tab elements
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 13/05/2011
    ''' Modified by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub EnableCurrentPage()
        Try
            'SGM 21/05/2012
            If MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_DOWN Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_UP Or _
               MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_NROTOR Then
                Exit Sub
            End If


            If MyClass.CurrentTestPanel IsNot Nothing Then
                'MyClass.CurrentTestPanel.Cursor = Cursors.Default
                For Each C As Control In MyClass.CurrentTestPanel.Controls
                    If TypeOf C Is BsScadaControl Then
                        If Not TypeOf C Is BsScadaPipeControl And Not TypeOf C Is BsScadaSyringeControl And Not TypeOf C Is BsScadaWashControl Then
                            C.Enabled = True
                            C.Cursor = Cursors.Hand
                        End If
                    End If
                Next
                'Internal dosing motors
                If MyClass.CurrentTestPanel Is Me.BsInternalDosingTestPanel Then
                    For Each C As Control In Me.BsInternalDosingMotorsPanel.Controls
                        If TypeOf C Is BsScadaControl Then
                            C.Cursor = Cursors.Default
                            If Not TypeOf C Is BsScadaPipeControl And Not TypeOf C Is BsScadaSyringeControl Then
                                C.Enabled = True
                                C.Cursor = Cursors.Hand
                            Else
                                C.Enabled = False
                                C.Cursor = Cursors.Default
                            End If
                        End If
                    Next
                End If

                'WS Aspiration or Dispensation SGM 18/05/2012
                Dim isWSDown As Boolean = MyClass.myScreenDelegate.IsWashingStationDown
                If MyClass.CurrentTestPanel Is Me.BsWSAspirationTestPanel Then
                    Me.WsAsp_B10_Pump.Enabled = isWSDown
                    Me.WsAsp_B9_Pump.Enabled = isWSDown
                    Me.WsAsp_B8_Pump.Enabled = isWSDown
                    Me.WsAsp_B7_Pump.Enabled = isWSDown
                    Me.WsAsp_B6_Pump.Enabled = isWSDown
                    Me.WSAsp_UpDownButton.Enabled = True

                ElseIf MyClass.CurrentTestPanel Is Me.BsWSDispensationTestPanel Then
                    Me.WsDisp_GE1_Valve.Enabled = isWSDown
                    Me.WsDisp_GE2_Valve.Enabled = isWSDown
                    Me.WsDisp_GE3_Valve.Enabled = isWSDown
                    Me.WsDisp_GE4_Valve.Enabled = isWSDown
                    Me.WsDisp_GE5_Valve.Enabled = isWSDown
                    Me.WsDisp_M1_Motor.Enabled = isWSDown
                    Me.WSDisp_UpDownButton.Enabled = True

                End If


                'Collision
                If MyClass.CurrentTestPanel Is Me.BsCollisionTestPanel Then
                    Me.Col_StartStopButton.Enabled = True
                End If

            End If
            If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                MyClass.UpdateActivationMode()
                MyClass.CurrentSwitchGroupBox.Enabled = True
                MyClass.EnableCurrentSwitchGroupBox()
            End If
            If MyClass.CurrentTestPanel IsNot Nothing Then
                MyClass.CurrentTestPanel.Enabled = True
            End If
            Me.BsCancelButton.Enabled = MyClass.IsAnyChanged
            Me.BsExitButton.Enabled = True

            Me.Cursor = Cursors.Default

            Me.Refresh()

            MyBase.ActivateMDIMenusButtons(True)

            ''SGM 22/11/2011
            'Me.bsInternalDosingTabPage.Enabled = True
            'Me.bsExternalWashingTabPage.Enabled = True
            'Me.bsAspirationTabPage.Enabled = True
            'Me.bsDispensationTabPage.Enabled = True
            'Me.bsInOutTabPage.Enabled = True
            'Me.bsCollisionTabPage.Enabled = True
            'Me.BsEncoderTabPage.Enabled = True

            MyClass.IsWorking = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".EnableCurrentElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableCurrentElements", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Disables the current page tab elements
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub DisableCurrentPage(Optional ByRef pExcept As BsScadaControl = Nothing)

        Dim myCopyOfExcept As BsScadaControl = Nothing
        If pExcept IsNot Nothing Then
            myCopyOfExcept = pExcept
        End If

        Try

            If MyClass.CurrentTestPanel IsNot Nothing Then
                'MyClass.CurrentTestPanel.Cursor = Cursors.WaitCursor
                For Each C As Control In MyClass.CurrentTestPanel.Controls
                    If TypeOf C Is BsScadaControl Then

                        If Not TypeOf C Is BsScadaPipeControl And Not TypeOf C Is BsScadaSyringeControl And Not TypeOf C Is BsScadaWashControl Then

                            If pExcept IsNot Nothing Then

                                If C IsNot pExcept Then
                                    'special case for dispensing
                                    If MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
                                        If C IsNot WsDisp_GE1_Valve And _
                                            C IsNot WsDisp_GE2_Valve And _
                                            C IsNot WsDisp_GE3_Valve And _
                                            C IsNot WsDisp_GE4_Valve And _
                                            C IsNot WsDisp_GE5_Valve Then

                                            C.Enabled = False

                                        End If
                                    Else
                                        C.Enabled = False
                                    End If
                                End If
                            Else
                                C.Enabled = False
                            End If
                        End If
                    End If
                Next

                'Internal dosing motors
                If MyClass.CurrentTestPanel Is Me.BsInternalDosingTestPanel Then
                    For Each C As Control In Me.BsInternalDosingMotorsPanel.Controls
                        If TypeOf C Is BsScadaControl Then
                            C.Cursor = Cursors.WaitCursor
                            If Not TypeOf C Is BsScadaPipeControl And Not TypeOf C Is BsScadaSyringeControl Then
                                If pExcept IsNot Nothing Then
                                    If C IsNot pExcept Then
                                        C.Enabled = False
                                    End If
                                Else
                                    C.Enabled = False
                                End If
                            End If
                        End If
                    Next
                End If


            End If

            'Collision
            If MyClass.CurrentTestPanel Is Me.BsCollisionTestPanel Then
                Me.Col_StartStopButton.Enabled = False
            End If

            If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                MyClass.CurrentSwitchGroupBox.Enabled = True
                MyClass.DisableCurrentSwitchGroupBox()
            End If
            'If MyClass.CurrentTestPanel IsNot Nothing Then
            '    MyClass.CurrentTestPanel.Enabled = False
            'End If
            Me.BsCancelButton.Enabled = False
            Me.BsExitButton.Enabled = False

            Me.Cursor = Cursors.WaitCursor
            If MyClass.IsContinuousSwitching Then
                If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                    MyClass.CurrentContinuousStopButton.Cursor = Cursors.Default
                    'MyClass.CurrentContinuousStopButton.Enabled = False 
                End If
            End If
            Me.Refresh()

            MyBase.ActivateMDIMenusButtons(False)

            MyClass.IsWorking = True

            ''SGM 22/11/2011
            'Me.bsInternalDosingTabPage.Enabled = False
            'Me.bsExternalWashingTabPage.Enabled = False
            'Me.bsAspirationTabPage.Enabled = False
            'Me.bsDispensationTabPage.Enabled = False
            'Me.bsInOutTabPage.Enabled = False
            'Me.bsCollisionTabPage.Enabled = False
            'Me.BsEncoderTabPage.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableCurrentElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableCurrentElements", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)

        Finally
            If myCopyOfExcept IsNot Nothing Then
                pExcept = myCopyOfExcept
            End If
        End Try
    End Sub

    Private Sub EnableCurrentSwitchGroupBox()
        Try
            If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                For Each C As Control In MyClass.CurrentSwitchGroupBox.Controls
                    If Not TypeOf C Is BSButton Then
                        C.Enabled = True
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".EnableCurrentSwitchGroupBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".EnableCurrentSwitchGroupBox", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DisableCurrentSwitchGroupBox()
        Try
            If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                For Each C As Control In MyClass.CurrentSwitchGroupBox.Controls
                    If Not TypeOf C Is BSButton Then
                        C.Enabled = False
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableCurrentSwitchGroupBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableCurrentSwitchGroupBox", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Switch element state
    ''' </summary>
    ''' <remarks>Created by SGM 06/05/2011</remarks>
    Private Function RequestSwitchElement(ByVal pElement As BsScadaControl, Optional ByVal pSetDefault As Boolean = False) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.IsActionRequested = True
            MyClass.SwitchRequestedElement = pElement


            If Not pSetDefault Then
                Select Case SwitchRequestedElement.ActivationState
                    Case BsScadaControl.States._OFF
                        RequestedNewState = BsScadaControl.States._ON
                    Case BsScadaControl.States._ON
                        RequestedNewState = BsScadaControl.States._OFF
                End Select
            Else
                'SGM 18/05/2012 if is requested to set to OFF
                RequestedNewState = SwitchRequestedElement.DefaultState
            End If

            'if you try to close a valve you must assure first that you can..
            If TypeOf SwitchRequestedElement Is BsScadaValveControl Then
                If RequestedNewState = BsScadaControl.States._OFF Then
                    myGlobal = MyClass.CanCloseValve(SwitchRequestedElement.Identity)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not CBool(myGlobal.SetDatos) Then
                        MyClass.UnSelectElement()
                        MyClass.IsActionRequested = False
                        Return myGlobal
                    End If
                End If
            End If


            'SGM 22/11/2011
            'preview the tanks state
            If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Then
                myGlobal.SetDatos = True 'The checking has been already performed
                'If TypeOf SwitchRequestedElement Is BsScadaPumpControl Then
                '    If RequestedNewState = BsScadaControl.States._ON Then
                '        MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING 'SGM 22/11/2011
                '        myGlobal = MyClass.WSAsp_CanActivatePump(SwitchRequestedElement.Identity)
                '        If Not CBool(myGlobal.SetDatos) Then
                '            MyClass.UnSelectElement()
                '            MyClass.IsActionRequested = False
                '            Return myGlobal
                '        End If
                '    End If
                'End If
            End If

            'preview the tanks state
            If MyClass.SelectedPage = TEST_PAGES.IN_OUT Then
                myGlobal.SetDatos = True 'The checking has been already performed
                'MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING 'SGM 22/11/2011
                'myGlobal = MyClass.InOut_CanActivateElement(SwitchRequestedElement.Identity)
                'If Not CBool(myGlobal.SetDatos) Then
                '    MyClass.UnSelectElement()
                '    MyClass.IsActionRequested = False
                '    Return myGlobal
                'End If
            End If
            'SGM 22/11/2011


            Select Case MyClass.CurrentActivationMode
                Case ACTIVATION_MODES.SIMPLE
                    MyClass.IsContinuousSwitching = False

                Case ACTIVATION_MODES.CONTINUOUS
                    If Not MyClass.StopContinuousSwitchingRequested Then
                        MyClass.IsContinuousSwitching = True
                    Else
                        MyClass.RequestedNewState = BsScadaControl.States._OFF
                        MyClass.IsContinuousSwitching = False
                        'MyClass.StopContinuousSwitchingRequested = False
                    End If

            End Select

            MyClass.SwitchRequestedElement.Enabled = False

            If MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
                Me.WsDisp_GE1_Valve.Enabled = MyClass.WSDisp_ValvesGroupDCUnit.Enabled
                Me.WsDisp_GE2_Valve.Enabled = MyClass.WSDisp_ValvesGroupDCUnit.Enabled
                Me.WsDisp_GE3_Valve.Enabled = MyClass.WSDisp_ValvesGroupDCUnit.Enabled
                Me.WsDisp_GE4_Valve.Enabled = MyClass.WSDisp_ValvesGroupDCUnit.Enabled
                Me.WsDisp_GE5_Valve.Enabled = MyClass.WSDisp_ValvesGroupDCUnit.Enabled
            End If

            If MyBase.SimulationMode Then


            Else


                Select Case RequestedNewState
                    Case BsScadaControl.States._ON
                        myScreenDelegate.ActivationMode = SWITCH_ACTIONS.SET_TO_ON
                        'pump activation parameters
                        MyClass.myScreenDelegate.PumpDutyValue = -1
                        MyClass.myScreenDelegate.PumpProgressiveStart = -1
                        If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl Then
                            MyClass.myScreenDelegate.PumpDutyValue = 100 'PDT constante
                            MyClass.myScreenDelegate.PumpProgressiveStart = 1 'PDT constante
                            Dim myPump As BsScadaPumpControl = CType(MyClass.SwitchRequestedElement, BsScadaPumpControl)
                            If myPump IsNot Nothing Then
                                MyClass.myScreenDelegate.PumpDutyValue = myPump.DutyValue
                                MyClass.myScreenDelegate.PumpProgressiveStart = myPump.ProgressiveStart
                            End If
                        End If



                    Case BsScadaControl.States._OFF
                        myScreenDelegate.ActivationMode = SWITCH_ACTIONS.SET_TO_OFF

                End Select

                MyClass.myScreenDelegate.Item = SwitchRequestedElement.Identity

                'define group
                If MyClass.SwitchRequestedElement.Identity.Contains("JE1") Then
                    MyClass.myScreenDelegate.ItemGroup = ItemGroups.MANIFOLD.ToString
                ElseIf MyClass.SwitchRequestedElement.Identity.Contains("SF1") Then
                    MyClass.myScreenDelegate.ItemGroup = ItemGroups.FLUIDICS.ToString
                End If

                'define type
                If TypeOf MyClass.SwitchRequestedElement Is BsScadaValveControl Then
                    MyClass.myScreenDelegate.ItemType = ItemTypes.VALVE.ToString
                ElseIf TypeOf MyClass.SwitchRequestedElement Is BsScadaValve3Control Then
                    MyClass.myScreenDelegate.ItemType = ItemTypes.VALVE3.ToString
                ElseIf TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl Then
                    MyClass.myScreenDelegate.ItemType = ItemTypes.PUMP.ToString
                ElseIf TypeOf MyClass.SwitchRequestedElement Is BsScadaMotorControl Then
                    MyClass.myScreenDelegate.ItemType = ItemTypes.MOTOR.ToString
                End If

                MyClass.TestSwitch()

            End If


            MyBase.DisplayMessage(Messages.SRV_TEST_REQUESTED.ToString)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestSwitchElement ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestSwitchElement", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
            MyClass.ReportHistoryError()
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Moves the motor
    ''' </summary>
    ''' <param name="pElement"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 06/05/2011</remarks>
    Private Function RequestMotorMove(ByVal pElement As BsScadaMotorControl) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO

        Try

            MyClass.IsActionRequested = True

            MyClass.MoveRequestedElement = pElement

            If Not myGlobal.HasError Then

                MyClass.MoveRequestedElement.Enabled = False
                MyClass.CurrentMotorAdjustControl.EditionMode = False
                'MyClass.CurrentMotorAdjustControl.Enabled = False

                Me.WSDisp_UpDownButton.Enabled = False

                If MyBase.SimulationMode Then

                    'MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING

                    'If MyClass.MoveRequestedMode = MOVEMENT.RELATIVE Then
                    '    MyClass.RequestedNewPosition = MyClass.MoveRequestedElement.CurrentPosition + MyClass.RequestedNewPosition
                    'ElseIf MyClass.MoveRequestedMode = MOVEMENT.ABSOLUTE Then
                    '    MyClass.RequestedNewPosition = MyClass.MoveRequestedElement.CurrentPosition
                    'ElseIf MyClass.MoveRequestedMode = MOVEMENT.HOME Then
                    '    MyClass.RequestedNewPosition = 0
                    'End If

                    'MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
                    'MyClass.PrepareArea()
                    'System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)

                    'MyClass.CurrentMotorAdjustControl.CurrentValue = MyClass.RequestedNewPosition
                    ''MyClass.CurrentMotorAdjustControl.EditionMode = True
                    'MyClass.CurrentMotorAdjustControl.Enabled = True
                    ''MyClass.MoveRequestedElement.Enabled = True

                    'MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                    'MyClass.PrepareArea()

                Else
                    'define group
                    If MyClass.MoveRequestedElement.Identity.Contains("JE1") Then
                        MyClass.myScreenDelegate.ItemGroup = ItemGroups.MANIFOLD.ToString
                    ElseIf MyClass.MoveRequestedElement.Identity.Contains("SF1") Then
                        MyClass.myScreenDelegate.ItemGroup = ItemGroups.FLUIDICS.ToString
                    ElseIf MyClass.MoveRequestedElement.Identity.Contains("GLF") Then
                        MyClass.myScreenDelegate.ItemGroup = ItemGroups.PHOTOMETRICS.ToString
                    End If

                    MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING
                    MyClass.StartReadingTimer()


                End If

            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestSwitchElement ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestSwitchElement", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
            MyClass.MoveRequestedElement = Nothing
            MyClass.ReportHistoryError()
        End Try

        Return myGlobal
    End Function


#Region "Adjustments"


    ''' <summary>
    ''' Gets the Dataset that corresponds to the editing adjustments
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 28/01/11</remarks>
    Private Function LoadArmsData() As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim CopyOfSelectedAdjustmentsDS As SRVAdjustmentsDS = MyClass.SelectedAdjustmentsDS
        Dim myAdjustmentsGroups As New List(Of String)
        Try
            If MyClass.SelectedAdjustmentsDS IsNot Nothing Then
                MyClass.SelectedAdjustmentsDS.Clear()
            End If
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK.ToString)

            ' XBC 09/11/2011
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER1_ARM_VSEC.ToString)
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.MIXER2_ARM_VSEC.ToString)
            ' XBC 09/11/2011

            'SGM 21/11/2011
            myAdjustmentsGroups.Add(ADJUSTMENT_GROUPS.WASHING_STATION.ToString)
            'SGM 21/11/2011

            resultData = MyBase.myAdjustmentsDelegate.ReadAdjustmentsByGroupIDs(myAdjustmentsGroups)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                MyClass.SelectedAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                'update screen delagate's parameters
                With MyClass.myScreenDelegate

                    'washing
                    .ArmSamplesWashingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, AXIS.POLAR).Value)
                    .ArmSamplesWashingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_WASH, AXIS.Z).Value)

                    .ArmReagent1WashingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, AXIS.POLAR).Value)
                    .ArmReagent1WashingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_WASH, AXIS.Z).Value)

                    .ArmReagent2WashingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, AXIS.POLAR).Value)
                    .ArmReagent2WashingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_WASH, AXIS.Z).Value)

                    .ArmMixer1WashingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, AXIS.POLAR).Value)
                    .ArmMixer1WashingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_WASH, AXIS.Z).Value)

                    .ArmMixer2WashingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, AXIS.POLAR).Value)
                    .ArmMixer2WashingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_WASH, AXIS.Z).Value)

                    ' XBC 09/11/2011
                    'parking
                    '.ArmSamplesParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, AXIS.POLAR).Value)
                    '.ArmSamplesParkingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, AXIS.Z).Value)

                    '.ArmReagent1ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, AXIS.POLAR).Value)
                    '.ArmReagent1ParkingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, AXIS.Z).Value)

                    '.ArmReagent2ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, AXIS.POLAR).Value)
                    '.ArmReagent2ParkingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, AXIS.Z).Value)

                    '.ArmMixer1ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, AXIS.POLAR).Value)
                    '.ArmMixer1ParkingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, AXIS.Z).Value)

                    '.ArmMixer2ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK, AXIS.POLAR).Value)
                    '.ArmMixer2ParkingZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK, AXIS.Z).Value)

                    .ArmSamplesParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_PARK, AXIS.POLAR).Value)
                    .ArmSamplesSecurityZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.SAMPLES_ARM_VSEC, AXIS.Z).Value)

                    .ArmReagent1ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_PARK, AXIS.POLAR).Value)
                    .ArmReagent1SecurityZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT1_ARM_VSEC, AXIS.Z).Value)

                    .ArmReagent2ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_PARK, AXIS.POLAR).Value)
                    .ArmReagent2SecurityZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.REAGENT2_ARM_VSEC, AXIS.Z).Value)

                    .ArmMixer1ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_PARK, AXIS.POLAR).Value)
                    .ArmMixer1SecurityZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER1_ARM_VSEC, AXIS.Z).Value)

                    .ArmMixer2ParkingPolar = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_PARK, AXIS.POLAR).Value)
                    .ArmMixer2SecurityZ = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.MIXER2_ARM_VSEC, AXIS.Z).Value)
                    ' XBC 09/11/2011

                    'SGM 21/11/2011
                    .WSReadyRefPos = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.WASHING_STATION, GlobalEnumerates.AXIS.REL_Z).Value)
                    .WSFinalPos = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.WASHING_STATION, GlobalEnumerates.AXIS.Z).Value)
                    .WSReferencePos = CInt(ReadArmPositionData(ADJUSTMENT_GROUPS.WASHING_STATION_PARK, GlobalEnumerates.AXIS.Z).Value)
                    'SGM 21/11/2011

                End With
            End If

        Catch ex As Exception
            MyClass.SelectedAdjustmentsDS = CopyOfSelectedAdjustmentsDS
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".LoadArmsData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".LoadArmsData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return resultData
    End Function


    ''' <summary>
    ''' Gets the value corresponding to informed Axis from the selected adjustments dataset
    ''' </summary>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 28/01/11
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadArmPositionData(ByVal pGroup As ADJUSTMENT_GROUPS, ByVal pAxis As GlobalEnumerates.AXIS) As ArmAdjustmentRowData
        Dim myGlobal As New GlobalDataTO
        Dim myArmRowData As New ArmAdjustmentRowData("")
        Try
            Dim myAxis As String = pAxis.ToString
            If myAxis = "NONE" Then myAxis = ""

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In MyClass.SelectedAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.GroupID.Trim = pGroup.ToString And _
                                a.AxisID.Trim = myAxis.Trim _
                                Select a).ToList
            'Where a.GroupID.Trim.ToUpper = pGroup.ToString And _
            'a.AxisID.Trim.ToUpper = myAxis.Trim.ToUpper _

            If myAdjustmentRows.Count > 0 Then
                With myArmRowData
                    .CodeFw = myAdjustmentRows(0).CodeFw
                    .Value = myAdjustmentRows(0).Value
                    .AxisID = myAdjustmentRows(0).AxisID
                    .GroupID = myAdjustmentRows(0).GroupID
                    .CanMove = myAdjustmentRows(0).CanMove
                End With
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadArmPositionData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReadArmPositionData ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        If myArmRowData.Value = "" Then myArmRowData.Value = "0"

        Return myArmRowData
    End Function

#End Region


#Region "Switch Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub SwitchSimpleButtonClick()
        Try
            If SelectedElement IsNot Nothing Then
                If TypeOf SelectedElement Is BsScadaValveControl Or _
                TypeOf SelectedElement Is BsScadaValve3Control Or _
                TypeOf SelectedElement Is BsScadaPumpControl Then
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE
                    MyClass.DisableCurrentSwitchGroupBox()
                    MyClass.RequestSwitchElement(SelectedElement)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SwitchSimpleButtonClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SwitchSimpleButtonClick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try
    End Sub

    Private Sub WhenElementCliked(ByVal pElement As BsScadaControl)

        Dim myGlobal As New GlobalDataTO

        Try
            MyClass.DisableCurrentPage() 'SGM 22/11/2011

            If MyClass.IsLoaded And MyClass.IsFirstReadingDone Then
                myGlobal = RequestSelectElement(pElement)

                If myGlobal.HasError Then

                    MyClass.SwitchRequestedElement = Nothing
                    MyClass.RequestedNewState = BsScadaControl.States._NONE
                    MyClass.MoveRequestedElement = Nothing
                    MyClass.MoveRequestedMode = MOVEMENT._NONE

                    MyClass.EnableCurrentPage()

                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WhenElementCliked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WhenElementCliked ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 13/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function RequestSelectElement(ByVal pElement As BsScadaControl) As GlobalDataTO

        Dim myglobal As New GlobalDataTO

        Try

            If pElement.Selected Then

                If MyClass.SelectedElement IsNot Nothing Then
                    If MyClass.SelectedElement IsNot pElement Then
                        MyClass.SelectedElement.Selected = False
                    End If
                End If

                MyClass.DeactivateAll()

                MyClass.SelectedElement = pElement
                'If SelectedElement.Identity.ToUpper.Contains("JE1") Then
                '    MyClass.SelectedElementGroup = ItemGroups.MANIFOLD
                'ElseIf SelectedElement.Identity.ToUpper.Contains("SF1") Then
                '    MyClass.SelectedElementGroup = ItemGroups.FLUIDICS
                'ElseIf SelectedElement.Identity.ToUpper.Contains("GLF") Then
                '    MyClass.SelectedElementGroup = ItemGroups.PHOTOMETRICS
                'End If
                If SelectedElement.Identity.Contains("JE1") Then
                    MyClass.SelectedElementGroup = ItemGroups.MANIFOLD
                ElseIf SelectedElement.Identity.Contains("SF1") Then
                    MyClass.SelectedElementGroup = ItemGroups.FLUIDICS
                ElseIf SelectedElement.Identity.Contains("GLF") Then
                    MyClass.SelectedElementGroup = ItemGroups.PHOTOMETRICS
                End If

                'MOTORS
                If TypeOf SelectedElement Is BsScadaMotorControl Then
                    Dim myMotor As BsScadaMotorControl = CType(pElement, BsScadaMotorControl)
                    If myMotor IsNot Nothing Then

                        myMotor.BackColor = Color.WhiteSmoke
                        myMotor.BorderStyle = BorderStyle.FixedSingle
                        MyClass.SelectedMotor = myMotor

                        myMotor.ActivationState = BsScadaControl.States._ON

                        With MyClass.CurrentMotorAdjustControl
                            .CurrentValue = MyClass.SelectedMotor.CurrentPosition
                            .LastValueSaved = MyClass.SelectedMotor.CurrentPosition
                            .HomingEnabled = True
                            .MaxNumDecimals = 0
                            .Enabled = True
                            .Focus()
                        End With

                    End If

                    If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                        MyClass.CurrentSimpleSwitchRButton.Checked = True
                        MyClass.CurrentContinuousSwitchRButton.Checked = False
                        MyClass.EnableCurrentSwitchGroupBox()
                        MyClass.CurrentContinuousStopButton.Enabled = False
                    End If

                    MyClass.EnableCurrentPage()

                    Return myglobal

                End If

                If MyClass.SelectedElement IsNot Nothing Then
                    If MyBase.SimulationMode Then

                        'warn about the output QUICK CONNECTOR must be open before activating the outlet pump
                        If MyClass.SelectedElement Is Me.InOut_LC_Pump Then
                            If Not MyClass.IsOutQuickConnectorWarnt Then
                                MyBase.ShowMessage(MyBase.myServiceMDI.Text, Messages.SRV_LC_QUICKCONNECT.ToString)
                                MyClass.IsOutQuickConnectorWarnt = True
                            End If
                        End If


                        If Not TypeOf MyClass.SelectedElement Is BsScadaMotorControl Then
                            MyClass.SwitchRequestedElement = MyClass.SelectedElement
                            Select Case MyClass.CurrentActivationMode
                                Case ACTIVATION_MODES.SIMPLE
                                    MyClass.IsContinuousSwitching = False

                                Case ACTIVATION_MODES.CONTINUOUS
                                    MyClass.IsContinuousSwitching = True

                            End Select

                            Select Case MyClass.SwitchRequestedElement.ActivationState
                                Case BsScadaControl.States._OFF
                                    MyClass.RequestedNewState = BsScadaControl.States._ON
                                Case BsScadaControl.States._ON
                                    MyClass.RequestedNewState = BsScadaControl.States._OFF
                            End Select



                            'preview the tanks state
                            If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Then
                                If TypeOf SwitchRequestedElement Is BsScadaPumpControl Then
                                    If RequestedNewState = BsScadaControl.States._ON Then
                                        MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING
                                        myglobal = MyClass.WSAsp_CanActivatePump(SwitchRequestedElement.Identity)
                                    End If
                                End If

                            ElseIf MyClass.SelectedPage = TEST_PAGES.IN_OUT Then
                                MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING 'SGM 22/11/2011
                                myglobal = MyClass.InOut_CanActivateElement(SwitchRequestedElement.Identity)
                            End If

                            If myglobal.SetDatos IsNot Nothing AndAlso Not CBool(myglobal.SetDatos) Then
                                MyClass.UnSelectElement()
                                MyClass.IsActionRequested = False
                                MyClass.IsActionNotAllowed = True
                                MyClass.PrepareActionNotAllowedMode(MyClass.SwitchRequestedElement)

                            Else
                                'read current values and status
                                MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING
                                myglobal = MyClass.StartReadingTimer()

                                If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl And MyClass.RequestedNewState = BsScadaControl.States._ON Then
                                    SimulatePumpPreActivations(pElement.Identity)
                                End If
                            End If

                        ElseIf MyClass.SelectedMotor IsNot Nothing Then
                            MyClass.MoveRequestedElement = MyClass.SelectedMotor
                            If MyClass.MoveRequestedMode = MOVEMENT.RELATIVE Then
                                MyClass.RequestedNewPosition = MyClass.MoveRequestedElement.CurrentPosition + MyClass.RequestedNewPosition
                            End If

                            'read current values and status
                            MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING
                            myglobal = MyClass.StartReadingTimer()

                        End If




                    Else

                        If Not TypeOf MyClass.SelectedElement Is BsScadaMotorControl Then
                            MyClass.SwitchRequestedElement = MyClass.SelectedElement
                            Select Case MyClass.CurrentActivationMode
                                Case ACTIVATION_MODES.SIMPLE
                                    MyClass.IsContinuousSwitching = False

                                Case ACTIVATION_MODES.CONTINUOUS
                                    MyClass.IsContinuousSwitching = True

                            End Select
                            Select Case MyClass.SwitchRequestedElement.ActivationState
                                Case BsScadaControl.States._OFF
                                    MyClass.RequestedNewState = BsScadaControl.States._ON
                                Case BsScadaControl.States._ON
                                    MyClass.RequestedNewState = BsScadaControl.States._OFF
                            End Select

                            'preview the tanks state
                            If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Then
                                If TypeOf SwitchRequestedElement Is BsScadaPumpControl Then
                                    If RequestedNewState = BsScadaControl.States._ON Then
                                        MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING
                                        myglobal = MyClass.WSAsp_CanActivatePump(SwitchRequestedElement.Identity)
                                    End If
                                End If

                            ElseIf MyClass.SelectedPage = TEST_PAGES.IN_OUT Then
                                MyClass.CurrentActionStep = ActionSteps.PREVIOUS_CHECKING 'SGM 22/11/2011
                                myglobal = MyClass.InOut_CanActivateElement(SwitchRequestedElement.Identity)
                            End If

                            If myglobal.SetDatos IsNot Nothing AndAlso Not CBool(myglobal.SetDatos) Then
                                MyClass.UnSelectElement()
                                MyClass.IsActionRequested = False
                                MyClass.IsActionNotAllowed = True
                                MyClass.PrepareActionNotAllowedMode(MyClass.SwitchRequestedElement)

                            Else
                                'read current values and status
                                MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING
                                myglobal = MyClass.StartReadingTimer()

                                If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl And MyClass.RequestedNewState = BsScadaControl.States._ON Then
                                    SimulatePumpPreActivations(pElement.Identity)
                                End If
                            End If

                        ElseIf MyClass.SelectedMotor IsNot Nothing Then
                            MyClass.MoveRequestedElement = MyClass.SelectedMotor
                            If MyClass.MoveRequestedMode = MOVEMENT.RELATIVE Then
                                MyClass.RequestedNewPosition = MyClass.MoveRequestedElement.CurrentPosition + MyClass.RequestedNewPosition
                            End If

                            'read current values and status
                            MyClass.CurrentActionStep = ActionSteps.PREVIOUS_READING
                            myglobal = MyClass.StartReadingTimer()

                        End If

                        If MyClass.CurrentActionStep = ActionSteps.ACTION_REJECTED Then
                            MyClass.CurrentActionStep = ActionSteps._NONE
                        Else
                            If Not myglobal.HasError Then
                                If MyClass.IsContinuousSwitching Then
                                    MyClass.DisableCurrentPage(SelectedElement)
                                Else
                                    MyClass.DisableCurrentPage()
                                End If
                            End If
                        End If


                    End If
                End If

                If Not myglobal.HasError Then

                    Dim myItem As String = ""

                    If MyClass.SelectedElement IsNot Nothing Then
                        If TypeOf MyClass.SelectedElement Is BsScadaValveControl Then
                            myItem = MyClass.ValveCaption
                        ElseIf TypeOf MyClass.SelectedElement Is BsScadaValve3Control Then
                            myItem = MyClass.Valve3Caption
                        ElseIf TypeOf MyClass.SelectedElement Is BsScadaPumpControl Then
                            myItem = MyClass.PumpCaption
                        ElseIf TypeOf MyClass.SelectedElement Is BsScadaMotorControl Then
                            myItem = MyClass.MotorCaption
                        End If

                        If myItem.Length > 0 Then
                            myItem = myItem + " (" + MyClass.SelectedElement.Identity + ")"
                        End If
                    End If

                    'myglobal = MyBase.DisplayMessage(Messages.SRV_ELEMENT_SELECTED.ToString + " " + myItem)

                Else

                    MyClass.CurrentActionStep = ActionSteps._NONE
                    MyClass.SwitchRequestedElement = Nothing
                    MyClass.RequestedNewState = BsScadaControl.States._NONE
                    MyClass.MoveRequestedElement = Nothing
                    MyClass.MoveRequestedMode = MOVEMENT._NONE

                End If

            Else
                'not selected
                With pElement
                    .BorderStyle = BorderStyle.None
                    .BackColor = Color.Transparent
                End With

                'MOTORS
                If SelectedElement IsNot Nothing Then
                    MyClass.UnSelectMotor()
                    MyClass.SelectedElement = Nothing
                    MyClass.SelectedElementGroup = ItemGroups._NONE
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RequestSelectElement ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RequestSelectElement ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
            MyClass.ReportHistoryError()
        End Try

        Return myglobal

    End Function

    Private Sub DeactivateAll()
        Try

            MyClass.UnSelectElement()
            MyClass.SwitchRequestedElement = Nothing
            MyClass.RequestedNewState = BsScadaControl.States._NONE
            MyClass.SelectedElementGroup = ItemGroups._NONE
            MyClass.IsActionRequested = False

            MyClass.UnSelectMotor()
            MyClass.MoveRequestedElement = Nothing
            MyClass.MoveRequestedMode = MOVEMENT._NONE

            MyClass.StopContinuousSwitchingRequested = False

            MyBase.ActivateMDIMenusButtons(False)

            'SGM 22/11/2011
            Me.bsInternalDosingTabPage.Enabled = False
            Me.bsExternalWashingTabPage.Enabled = False
            Me.bsAspirationTabPage.Enabled = False
            Me.bsDispensationTabPage.Enabled = False
            Me.bsInOutTabPage.Enabled = False
            Me.bsCollisionTabPage.Enabled = False
            Me.BsEncoderTabPage.Enabled = False

            'SGM 18/05/2012
            Me.WSAsp_UpDownButton.Enabled = False
            Me.WSDisp_UpDownButton.Enabled = False


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DeactivateAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DeactivateAll ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function TryActivateElement() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            'VALVES & PUMPS
            If TypeOf SelectedElement Is BsScadaValveControl Or _
            TypeOf SelectedElement Is BsScadaValve3Control Or _
            TypeOf SelectedElement Is BsScadaPumpControl Then

                myGlobal = RequestSwitchElement(SelectedElement)

                If myGlobal.HasError Then

                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareArea()

                ElseIf MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Then 'SGM 22/11/2011
                    If CBool(myGlobal.SetDatos) = False Then
                        MyClass.PrepareActionNotAllowedMode(MyClass.SelectedElement)
                        myGlobal.SetDatos = True

                        MyClass.IsActionNotAllowed = False

                        If Not MyBase.SimulationMode Then
                            MyBase.myServiceMDI.SEND_INFO_START(MyClass.IsWorking) 'SGM 22/11/2011
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".TryActivateElement ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TryActivateElement ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Before switching ON a Pump in the Analyzer it is necessary to switch ON their 
    ''' corresponding Valve some miliseconds before 
    ''' 
    ''' </summary>
    ''' <param name="pItem"></param>
    ''' <remarks>Created by SGM 11/05/2011</remarks>
    Private Function CanCloseValve(ByVal pItem As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal.SetDatos = True

            Select Case pItem

                Case MANIFOLD_ELEMENTS.JE1_EV1.ToString
                    myGlobal.SetDatos = (Me.InDo_Samples_Pump.ActivationState = BsScadaControl.States._OFF)

                Case MANIFOLD_ELEMENTS.JE1_EV2.ToString
                    myGlobal.SetDatos = (Me.InDo_Reagent1_Pump.ActivationState = BsScadaControl.States._OFF)

                Case MANIFOLD_ELEMENTS.JE1_EV3.ToString
                    myGlobal.SetDatos = (Me.InDo_Reagent2_Pump.ActivationState = BsScadaControl.States._OFF)

                Case FLUIDICS_ELEMENTS.SF1_EV2.ToString
                    myGlobal.SetDatos = (Me.InOut_PW_Pump.ActivationState = BsScadaControl.States._OFF)

            End Select

            If myGlobal.SetDatos IsNot Nothing AndAlso Not CBool(myGlobal.SetDatos) Then
                MyBase.DisplayMessage(Messages.SRV_PUMP_MUST_DEACTIVATE.ToString)
                MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.NOK)
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CanCloseValve ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CanCloseValve", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try

        Return myGlobal

    End Function

    '''' <summary>
    '''' Get Limits values from BD for Motors and Other Tests
    '''' </summary>
    '''' <remarks>Created by XBC 19/04/2011</remarks>
    'Private Function GetParameters() As GlobalDataTO
    '    Dim myGlobalDataTO As New GlobalDataTO
    '    Try
    '        'myGlobalDataTO = myScreenDelegate.GetParameters(Me.ActiveAnalyzerModel)
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    '    Return myGlobalDataTO
    'End Function

    ''' <summary>
    ''' Method incharge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 27/04/2012</remarks>
    Private Function GetMotorsLimits(ByVal pLimitsID As FieldLimitsEnum) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the specified limits
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetMotorsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & " GetMotorsLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobalDataTO
    End Function


#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub RefreshSyringes()
        Try
            For Each C As Control In MyClass.CurrentTestPanel.Controls
                If TypeOf C Is BsScadaSyringeControl Then
                    Dim mySyr As BsScadaSyringeControl = CType(C, BsScadaSyringeControl)
                    If mySyr IsNot Nothing Then
                        mySyr.RefreshSyringe()
                        mySyr.Refresh()
                    End If
                ElseIf TypeOf C Is BSGroupBox Or TypeOf C Is BSPanel Then
                    For Each K As Control In C.Controls
                        If TypeOf K Is BsScadaSyringeControl Then
                            Dim mySyr As BsScadaSyringeControl = CType(K, BsScadaSyringeControl)
                            If mySyr IsNot Nothing Then
                                mySyr.RefreshSyringe()
                                mySyr.Refresh()
                            End If
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshSyringes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshSyringes ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ManageTabChanged()
        Try

            If MyClass.NewSelectedTab Is bsInternalDosingTabPage Then
                Me.SelectedPage = TEST_PAGES.INTERNAL_DOSING
                MyClass.CurrentTestPanel = Me.BsInternalDosingTestPanel
                MyClass.CurrentSwitchGroupBox = Me.InDo_SwitchGroupBox
                MyClass.CurrentSimpleSwitchRButton = Me.InDo_SwitchSimpleRButton
                MyClass.CurrentContinuousSwitchRButton = Me.InDo_SwitchContinuousRButton
                MyClass.CurrentMotorAdjustControl = Me.InDo_Motors_AdjustControl
                MyClass.CurrentContinuousStopButton = Me.InDo_StopButton
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = True

            ElseIf MyClass.NewSelectedTab Is bsExternalWashingTabPage Then
                Me.SelectedPage = TEST_PAGES.EXTERNAL_WASHING
                MyClass.CurrentTestPanel = Me.BsExternalWashingTestPanel
                MyClass.CurrentSwitchGroupBox = Me.ExWa_SwitchGroupBox
                MyClass.CurrentSimpleSwitchRButton = Me.ExWa_SwitchSimpleRButton
                MyClass.CurrentContinuousSwitchRButton = Me.ExWa_SwitchContinuousRButton
                MyClass.CurrentMotorAdjustControl = Nothing
                MyClass.CurrentContinuousStopButton = Me.ExWa_StopButton
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = True

            ElseIf MyClass.NewSelectedTab Is bsAspirationTabPage Then
                Me.SelectedPage = TEST_PAGES.WS_ASPIRATION
                MyClass.CurrentTestPanel = Me.BsWSAspirationTestPanel
                MyClass.CurrentSwitchGroupBox = Me.WSAsp_SwitchGroupBox
                MyClass.CurrentSimpleSwitchRButton = Me.WsAsp_SwitchSimpleRButton
                MyClass.CurrentContinuousSwitchRButton = Me.WsAsp_SwitchContinuousRButton
                MyClass.CurrentMotorAdjustControl = Nothing
                MyClass.CurrentContinuousStopButton = Me.WSAsp_StopButton
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = True
                Me.WSAsp_UpDownButton.Enabled = False
                MyClass.DisableCurrentPage()

            ElseIf MyClass.NewSelectedTab Is bsDispensationTabPage Then
                Me.SelectedPage = TEST_PAGES.WS_DISPENSATION
                MyClass.CurrentTestPanel = Me.BsWSDispensationTestPanel
                MyClass.CurrentSwitchGroupBox = Me.WsDisp_SwitchGroupBox
                MyClass.CurrentSimpleSwitchRButton = Me.WsDisp_SwitchSimpleRButton
                MyClass.CurrentContinuousSwitchRButton = Me.WsDisp_SwitchContinuousRButton
                MyClass.CurrentMotorAdjustControl = Me.WSDisp_MotorAdjustControl
                MyClass.CurrentContinuousStopButton = Me.WsDisp_StopButton
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = True
                Me.WSDisp_UpDownButton.Enabled = False
                MyClass.DisableCurrentPage()

            ElseIf MyClass.NewSelectedTab Is bsInOutTabPage Then
                Me.SelectedPage = TEST_PAGES.IN_OUT
                MyClass.CurrentTestPanel = Me.BsInOutTestPanel
                MyClass.CurrentSwitchGroupBox = Me.InOut_SwitchGroupBox
                MyClass.CurrentSimpleSwitchRButton = Me.InOut_SwitchSimpleRButton
                MyClass.CurrentContinuousSwitchRButton = Me.InOut_SwitchContinuousRButton
                MyClass.CurrentMotorAdjustControl = Nothing
                MyClass.CurrentContinuousStopButton = Me.InOut_StopButton
                MyClass.IsOutQuickConnectorWarnt = False
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = True

            ElseIf MyClass.NewSelectedTab Is bsCollisionTabPage Then
                Me.SelectedPage = TEST_PAGES.COLLISION_DETECTION
                MyClass.CurrentTestPanel = Me.BsCollisionTestPanel
                MyClass.CurrentSwitchGroupBox = Nothing
                MyClass.CurrentSimpleSwitchRButton = Nothing
                MyClass.CurrentContinuousSwitchRButton = Nothing
                MyClass.CurrentMotorAdjustControl = Nothing
                MyClass.SelectedMotor = Nothing
                MyClass.CurrentContinuousStopButton = Nothing
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = False
                MyClass.IsCollisionTestEnabled = False

            ElseIf MyClass.NewSelectedTab Is BsEncoderTabPage Then
                Me.SelectedPage = TEST_PAGES.ENCODER_TEST
                MyClass.CurrentTestPanel = Me.BsEncoderTestPanel
                MyClass.CurrentSwitchGroupBox = Nothing
                MyClass.CurrentSimpleSwitchRButton = Nothing
                MyClass.CurrentContinuousSwitchRButton = Nothing
                MyClass.CurrentMotorAdjustControl = Enco_MotorAdjustControl
                MyClass.SelectedMotor = Me.Enco_ReactionsRotorMotor
                MyClass.CurrentContinuousStopButton = Nothing
                MyBase.myScreenLayout.ButtonsPanel.CancelButton.Visible = False
            End If

            MyClass.RefreshSyringes()

            MyClass.UnSelectElement()

            If MyClass.CurrentSimpleSwitchRButton IsNot Nothing Then
                MyClass.CurrentSimpleSwitchRButton.Checked = True
            End If

            If MyClass.CurrentContinuousSwitchRButton IsNot Nothing Then
                MyClass.CurrentContinuousSwitchRButton.Checked = False
            End If

            If MyClass.CurrentSwitchGroupBox IsNot Nothing Then
                MyClass.EnableCurrentSwitchGroupBox()
            End If

            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                MyClass.CurrentMotorAdjustControl.ClearText()
            End If

            If MyClass.CurrentTestPanel IsNot Nothing Then
                For Each C As Control In MyClass.CurrentTestPanel.Controls
                    If TypeOf C Is BsScadaSyringeControl Then
                        Dim mySyr As BsScadaSyringeControl = CType(C, BsScadaSyringeControl)
                        If mySyr IsNot Nothing Then
                            mySyr.RefreshSyringe()
                        End If
                    End If
                    If TypeOf C Is BsScadaPipeControl Then
                        Dim myPipe As BsScadaPipeControl = CType(C, BsScadaPipeControl)
                        If myPipe IsNot Nothing Then
                            'myPipe.Cursor = Cursors.Default
                            'If myPipe.FluidColor <> Color.DimGray Then
                            '    myPipe.FluidColor = Color.WhiteSmoke
                            'End If
                            myPipe.Refresh()
                        End If
                    End If
                Next
            End If


            MyClass.IsAnyChanged = False
            MyClass.UpdateActivationMode()

            If Not MyClass.myScreenDelegate.IsWashingStationDown Then
                MyBase.DisplayMessage(GlobalEnumerates.Messages.SRV_TEST_READY.ToString)
            End If

            If MyClass.IsLoaded And Not MyClass.myScreenDelegate.IsWashingStationDown Then 'SGM 22/11/2011

                MyClass.EnableCurrentPage()

            End If

            If Me.SelectedPage = TEST_PAGES.ENCODER_TEST Then
                Me.Enco_ReactionsRotorMotor.Selected = True
                WhenElementCliked(Me.Enco_ReactionsRotorMotor)
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ManageTabChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ManageTabChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub UnSelectElement(Optional ByVal pOnlyFrame As Boolean = False)
        Try
            If MyClass.SelectedElement IsNot Nothing Then
                If Not TypeOf MyClass.SelectedElement Is BsScadaMotorControl Then
                    With MyClass.SelectedElement
                        .BorderStyle = BorderStyle.None
                        .BackColor = Color.Transparent
                    End With
                    If pOnlyFrame Then
                        MyClass.SelectedElement = Nothing
                    End If
                End If
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CurrentTestPanel_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".CurrentTestPanel_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub UnSelectMotor()
        Try
            'unselect the motor
            If MyClass.SelectedMotor IsNot Nothing Then
                If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                    MyClass.CurrentMotorAdjustControl.CurrentValue = 0
                    MyClass.CurrentMotorAdjustControl.Enabled = False
                    MyClass.CurrentMotorAdjustControl.ClearText()
                End If
                With MyClass.SelectedMotor
                    .BackColor = Color.Transparent
                    .BorderStyle = BorderStyle.None
                    .ActivationState = BsScadaControl.States._OFF
                End With
                MyClass.SelectedMotor = Nothing
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UnSelectMotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UnSelectMotor ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub UpdateActivationMode()
        Try
            If MyClass.CurrentSimpleSwitchRButton IsNot Nothing Then
                If MyClass.CurrentSimpleSwitchRButton.Checked Then
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE
                    If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                        MyClass.CurrentContinuousStopButton.Enabled = False
                    End If
                End If
            End If
            If MyClass.CurrentContinuousSwitchRButton IsNot Nothing Then
                If MyClass.CurrentContinuousSwitchRButton.Checked Then
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.CONTINUOUS
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateActivationMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateActivationMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub AsumeAllSwitchOff()
        Try

            '-	Values by default : always when leave every TAB or exit Screen
            'o	All pumps and electro-valves : OFF
            'o	Water station : IN???????????????????????
            'o	Fluid selection Manifold valves : valve2 OFF + valve1 OFF = WASHING SOLUTION

            'first pumps
            '***************************************************************************************

            'INTERNAL DOSING
            Me.InDo_Samples_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent1_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent2_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_UpdateSyringeColors()
            MyClass.SimulateHOMEPositioning(Me.InDo_Samples_Motor, 0, True)
            MyClass.SimulateHOMEPositioning(Me.InDo_Reagent1_Motor, 0, True)
            MyClass.SimulateHOMEPositioning(Me.InDo_Reagent2_Motor, 0, True)

            'EXTERNAL WASHING
            Me.ExWa_Samples_Pump.ActivationState = BsScadaControl.States._OFF
            Me.ExWa_Reagent1_Pump.ActivationState = BsScadaControl.States._OFF
            Me.ExWa_Reagent2_Pump.ActivationState = BsScadaControl.States._OFF

            'WS (aspiration)
            Me.WsAsp_B6_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B7_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B8_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B9_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B10_Pump.ActivationState = BsScadaControl.States._OFF

            'WS (dispensation)
            Me.WsDisp_GE1_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE2_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE3_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE4_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE5_Valve.ActivationState = BsScadaControl.States._OFF
            MyClass.SimulateHOMEPositioning(Me.WsDisp_M1_Motor, 0, True)

            'In/Out
            Me.InOut_PW_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InOut_LC_Pump.ActivationState = BsScadaControl.States._OFF


            'after valves and others
            '***************************************************************************************
            'INTERNAL DOSING
            Me.InDo_Air_Ws_3Evalve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_AirWs_Pw_3Evalve.ActivationState = BsScadaControl.States._OFF

            'PDT
            'change source label to PW

            Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Samples_Motor.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent1_Motor.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent2_Motor.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Samples_Motor.CurrentPosition = 0
            Me.InDo_Reagent1_Motor.CurrentPosition = 0
            Me.InDo_Reagent2_Motor.CurrentPosition = 0

            Me.InDo_UpdateSyringeColors()

            'EXTERNAL WASHING

            'WS (aspiration)

            'WS (dispensation)
            Me.WsDisp_M1_Motor.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_M1_Motor.CurrentPosition = 0
            Me.WsDisp_GE1_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE2_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE3_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE4_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE5_Valve.ActivationState = BsScadaControl.States._OFF

            'In/Out

            Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._OFF
            Me.InOut_PWSource_Valve.ActivationState = BsScadaControl.States._OFF

            MyClass.IsAnyChanged = False
            MyClass.IsActionRequested = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AsumeAllSwitchOff ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AsumeAllSwitchOff ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#Region "Simulation Methods"

    ''' <summary>
    ''' Before switching ON a Pump in the Analyzer it is necessary to switch ON their 
    ''' corresponding Valve some miliseconds before 
    ''' 
    ''' </summary>
    ''' <param name="pItem"></param>
    ''' <remarks>Created by SGM 11/05/2011</remarks>
    Private Function SimulatePumpPreActivations(ByVal pItem As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            Select Case pItem

                Case MANIFOLD_ELEMENTS.JE1_B1.ToString
                    If Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._OFF Then
                        Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._ON
                        System.Threading.Thread.Sleep(SimPreActivationDelay)
                    End If

                Case MANIFOLD_ELEMENTS.JE1_B2.ToString
                    If Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._OFF Then
                        Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._ON
                        System.Threading.Thread.Sleep(SimPreActivationDelay)
                    End If

                Case MANIFOLD_ELEMENTS.JE1_B3.ToString
                    If Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._OFF Then
                        Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._ON
                        System.Threading.Thread.Sleep(SimPreActivationDelay)
                    End If


                Case FLUIDICS_ELEMENTS.SF1_B4.ToString
                    If Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._OFF Then
                        Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._ON
                        System.Threading.Thread.Sleep(SimPreActivationDelay)
                    End If
            End Select

            Me.Refresh()

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SimulatePumpPreActivations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulatePumpPreActivations", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' After switching OFF a Pump in the Analyzer it is necessary to switch OFF their 
    ''' corresponding Valve some miliseconds after 
    ''' </summary>
    ''' <param name="pItem"></param>
    ''' <remarks>Created by SGM 11/05/2011</remarks>
    Private Function SimulatePumpPostActivations(ByVal pItem As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try



            Select Case pItem
                Case MANIFOLD_ELEMENTS.JE1_B1.ToString
                    If Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._ON Then
                        If MyBase.SimulationMode Then System.Threading.Thread.Sleep(SimPostActivationDelay)
                        Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._OFF
                    End If

                Case MANIFOLD_ELEMENTS.JE1_B2.ToString
                    If Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._ON Then
                        If MyBase.SimulationMode Then System.Threading.Thread.Sleep(SimPostActivationDelay)
                        Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._OFF
                    End If

                Case MANIFOLD_ELEMENTS.JE1_B3.ToString
                    If Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._ON Then
                        If MyBase.SimulationMode Then System.Threading.Thread.Sleep(SimPostActivationDelay)
                        Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._OFF
                    End If

                Case FLUIDICS_ELEMENTS.SF1_B4.ToString
                    If Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._ON Then
                        If MyBase.SimulationMode Then System.Threading.Thread.Sleep(SimPostActivationDelay)
                        Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._OFF
                    End If

            End Select

            Me.Refresh()

            If MyBase.SimulationMode Then System.Threading.Thread.Sleep(500)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SimulatePumpPostActivations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulatePumpPostActivations", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

    Private Sub SimulateSetToDefault()
        Try

            '-	Values by default : always when leave every TAB or exit Screen
            'o	All pumps and electro-valves : OFF
            'o	Water station : IN???????????????????????
            'o	Fluid selection Manifold valves : valve2 OFF + valve1 OFF = WASHING SOLUTION

            'first pumps
            '***************************************************************************************

            MyClass.IsActionRequested = True

            'INTERNAL DOSING
            Me.InDo_Samples_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent1_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent2_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InDo_UpdateSyringeColors()

            'EXTERNAL WASHING
            Me.ExWa_Samples_Pump.ActivationState = BsScadaControl.States._OFF
            Me.ExWa_Reagent1_Pump.ActivationState = BsScadaControl.States._OFF
            Me.ExWa_Reagent2_Pump.ActivationState = BsScadaControl.States._OFF

            'WS (aspiration)
            Me.WsAsp_B6_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B7_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B8_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B9_Pump.ActivationState = BsScadaControl.States._OFF
            Me.WsAsp_B10_Pump.ActivationState = BsScadaControl.States._OFF

            'WS (dispensation)
            'In/Out
            Me.InOut_PW_Pump.ActivationState = BsScadaControl.States._OFF
            Me.InOut_LC_Pump.ActivationState = BsScadaControl.States._OFF


            'delay
            System.Threading.Thread.Sleep(SimPostActivationDelay)

            'after valves and others
            '***************************************************************************************
            'INTERNAL DOSING
            Me.InDo_Air_Ws_3Evalve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_AirWs_Pw_3Evalve.ActivationState = BsScadaControl.States._OFF

            'PDT
            'change source label to PW

            Me.InDo_Samples_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent1_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Reagent2_EValve.ActivationState = BsScadaControl.States._OFF
            Me.InDo_Samples_Motor.ActivationState = BsScadaControl.States._ON
            Me.InDo_Reagent1_Motor.ActivationState = BsScadaControl.States._ON
            Me.InDo_Reagent2_Motor.ActivationState = BsScadaControl.States._ON
            Me.InDo_Samples_Motor.CurrentPosition = 0
            Me.InDo_Reagent1_Motor.CurrentPosition = 0
            Me.InDo_Reagent2_Motor.CurrentPosition = 0

            Me.InDo_UpdateSyringeColors()

            'EXTERNAL WASHING

            'WS (aspiration)

            'WS (dispensation)
            Me.WsDisp_M1_Motor.ActivationState = BsScadaControl.States._ON
            Me.WsDisp_M1_Motor.CurrentPosition = 0
            Me.WsDisp_GE1_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE2_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE3_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE4_Valve.ActivationState = BsScadaControl.States._OFF
            Me.WsDisp_GE5_Valve.ActivationState = BsScadaControl.States._OFF

            'In/Out

            Me.InOut_PWTank_Valve.ActivationState = BsScadaControl.States._OFF
            Me.InOut_PWSource_Valve.ActivationState = BsScadaControl.States._OFF

            MyClass.IsAnyChanged = False
            MyClass.IsActionRequested = False

            MyClass.IsFirstReadingDone = True

        Catch ex As Exception
            MyClass.IsActionRequested = False
            Throw ex
        End Try
    End Sub



    Private Sub SimulateSwitchCompleted()
        Try



            If MyClass.SwitchRequestedElement IsNot Nothing Then

                If TypeOf SwitchRequestedElement Is BsScadaValveControl Then
                    Dim myValve As BsScadaValveControl = CType(SwitchRequestedElement, BsScadaValveControl)
                    If myValve IsNot Nothing Then
                        myValve.ActivationState = MyClass.RequestedNewState
                    End If
                ElseIf TypeOf SwitchRequestedElement Is BsScadaPumpControl Then
                    Dim myPump As BsScadaPumpControl = CType(SwitchRequestedElement, BsScadaPumpControl)
                    If myPump IsNot Nothing Then
                        myPump.ActivationState = MyClass.RequestedNewState
                        If MyClass.RequestedNewState = BsScadaControl.States._OFF Then
                            MyClass.SimulatePumpPostActivations(SwitchRequestedElement.Identity)
                        End If
                        'caso especial de WW DISPENSATION
                        If myPump Is WSDisp_ValvesGroupDCUnit Then
                            WsDisp_GE1_Valve.ActivationState = myPump.ActivationState
                            WsDisp_GE2_Valve.ActivationState = myPump.ActivationState
                            WsDisp_GE3_Valve.ActivationState = myPump.ActivationState
                            WsDisp_GE4_Valve.ActivationState = myPump.ActivationState
                            WsDisp_GE5_Valve.ActivationState = myPump.ActivationState
                        End If
                    End If

                    'caso especial de INTERNAL DOSING
                ElseIf TypeOf SwitchRequestedElement Is BsScadaValve3Control Then
                    Dim myValve3 As BsScadaValve3Control = CType(SwitchRequestedElement, BsScadaValve3Control)
                    If myValve3 IsNot Nothing Then
                        If MyClass.RequestedNewState <> BsScadaControl.States._NONE Then

                            'Internal Dosing Valves PW, WS, AIR
                            If MyClass.SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                                If myValve3 Is Me.InDo_Air_Ws_3Evalve Then
                                    Me.InDo_Air_Ws_3Evalve.ActivationState = MyClass.RequestedNewState
                                    If MyClass.IsInternalDosingSourceChanging Then
                                        MyClass.InDo_RequestSwitchSource(2)
                                        Exit Sub
                                    End If
                                End If
                                If myValve3 Is Me.InDo_AirWs_Pw_3Evalve Then
                                    Me.InDo_AirWs_Pw_3Evalve.ActivationState = MyClass.RequestedNewState
                                End If

                                MyClass.IsInternalDosingSourceChanging = True
                                MyClass.Indo_UpdateSource()
                                MyClass.IsInternalDosingSourceChanging = False
                                Me.InDo_SourceGroupBox.Enabled = True
                            End If

                            'Dispensation Valves
                            If MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
                                If myValve3 Is Me.WsDisp_GE1_Valve Or _
                                    myValve3 Is Me.WsDisp_GE2_Valve Or _
                                    myValve3 Is Me.WsDisp_GE3_Valve Or _
                                    myValve3 Is Me.WsDisp_GE4_Valve Or _
                                    myValve3 Is Me.WsDisp_GE5_Valve Then

                                    Me.WsDisp_GE1_Valve.ActivationState = MyClass.RequestedNewState
                                    Me.WsDisp_GE2_Valve.ActivationState = MyClass.RequestedNewState
                                    Me.WsDisp_GE3_Valve.ActivationState = MyClass.RequestedNewState
                                    Me.WsDisp_GE4_Valve.ActivationState = MyClass.RequestedNewState
                                    Me.WsDisp_GE5_Valve.ActivationState = MyClass.RequestedNewState

                                End If
                            End If

                        End If

                    End If

                End If




                If SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                    InDo_UpdateSyringeColors()
                End If

                MyClass.IsActionRequested = False

                'Select Case CurrentActivationMode
                '    Case ACTIVATION_MODES.SIMPLE
                '        MyBase.DisplaySimulationMessage("Switch action successfully completed")

                '    Case ACTIVATION_MODES.CONTINUOUS
                '        MyBase.DisplaySimulationMessage("Continuous Switch action successfully performing...")

                '    Case ACTIVATION_MODES._NONE
                '        MyBase.DisplaySimulationMessage("")

                'End Select


            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SimulateSwitchCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SimulateSwitchCompleted ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateRELPositioning(ByRef pMotor As BsScadaMotorControl, ByVal pNewValue As Integer, Optional ByVal pHideMessages As Boolean = False)
        Try

            ' simulating
            If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)

            'MyClass.DisableCurrentPage()

            Me.Cursor = Cursors.WaitCursor
            System.Threading.Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default
            If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            'MyClass.EnableCurrentPage()

            pMotor.CurrentPosition = pNewValue

            Application.DoEvents()

            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                MyClass.CurrentMotorAdjustControl.CurrentValue = pNewValue
                'MyClass.CurrentMotorAdjustControl.Enabled = True
                'MyClass.CurrentMotorAdjustControl.Focus()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateStepPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateStepPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateABSPositioning(ByRef pMotor As BsScadaMotorControl, ByVal pNewValue As Integer, Optional ByVal pHideMessages As Boolean = False)
        Try
            ' simulating
            If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)

            'MyClass.DisableCurrentPage()

            Me.Cursor = Cursors.WaitCursor
            System.Threading.Thread.Sleep(SimulationProcessTime)
            MyBase.myServiceMDI.Focus()
            Me.Cursor = Cursors.Default

            If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)

            'MyClass.EnableCurrentPage()

            pMotor.CurrentPosition = pNewValue

            Application.DoEvents()

            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                MyClass.CurrentMotorAdjustControl.CurrentValue = pNewValue
                'MyClass.CurrentMotorAdjustControl.Enabled = True
                'MyClass.CurrentMotorAdjustControl.Focus()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateAbsPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateAbsPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SimulateHOMEPositioning(ByRef pMotor As BsScadaMotorControl, ByVal pNewValue As Integer, _
                                        Optional ByVal pAreAllSettingToDefault As Boolean = False, Optional ByVal pHideMessages As Boolean = False)
        Try

            ' simulating
            If Not pAreAllSettingToDefault Then
                If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                'MyClass.DisableCurrentPage()
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                If Not pHideMessages Then MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString)
                'MyClass.EnableCurrentPage()
            End If

            pMotor.CurrentPosition = pNewValue

            Application.DoEvents()

            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                MyClass.CurrentMotorAdjustControl.CurrentValue = pNewValue
                'MyClass.CurrentMotorAdjustControl.Enabled = True
                'MyClass.CurrentMotorAdjustControl.Focus()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Name & ".SimulateHOMEPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Name & ".SimulateHOMEPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Prepare Methods"

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 19/04/2011
    ''' Modified by: XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    MyClass.PrepareAdjustReadedMode()

                Case ADJUSTMENT_MODES.HOME_FINISHED
                    MyClass.PrepareHomesFinishedMode()

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_WASHING
                    MyClass.PrepareAllArmsInWashingMode()

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_NROTOR
                    MyClass.PrepareWashingStationIsDowningMode()

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_NROTOR_PERFORMED
                    MyClass.WashingStationToDownAfterNRotor()

                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_DOWN
                    MyClass.PrepareWashingStationIsDownMode()

                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_UP
                    MyClass.PrepareWashingStationIsUpMode()

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_PARKING
                    MyClass.PrepareAllArmsInParkingMode()

                Case ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                    MyClass.PrepareAllSwitchedOffMode()

                Case ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareLoadedMode()
                    If Not MyClass.IsContinuousSwitching Then
                        MyBase.ActivateMDIMenusButtons(MyClass.CurrentTestPanel.Enabled)
                        If Not MyClass.CurrentTestPanel.Enabled Then MyClass.EnableCurrentPage()
                    End If

                Case ADJUSTMENT_MODES.COLLISION_TEST_ENABLING
                    MyClass.PrepareCollisionTestEnablingMode()

                Case ADJUSTMENT_MODES.COLLISION_TEST_ENABLED
                    MyClass.PrepareCollisionTestEnabledMode()

                Case ADJUSTMENT_MODES.COLLISION_TEST_DISABLING
                    MyClass.PrepareCollisionTestDisablingMode()

                Case ADJUSTMENT_MODES.TESTING
                    MyClass.PrepareTestingMode()

                Case ADJUSTMENT_MODES.TESTED
                    MyClass.PrepareTestedMode()

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareErrorMode()
            End Select

            If MyBase.myServiceMDI IsNot Nothing Then
                If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                    MyClass.PrepareErrorMode()
                    MyBase.DisplayMessage("")
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Adjust Prepared Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/01/2011</remarks>
    Private Sub PrepareAdjustReadedMode()
        Try
            MyClass.LoadArmsData()

            'read system tanks level limits
            MyClass.MinHighContaminationTankLevel = MyClass.ReadTanksAdjustmentData(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.EMPTY)
            MyClass.MaxHighContaminationTankLevel = MyClass.ReadTanksAdjustmentData(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.FULL)
            MyClass.MinDistilledWaterTankLevel = MyClass.ReadTanksAdjustmentData(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.EMPTY)
            MyClass.MaxDistilledWaterTankLevel = MyClass.ReadTanksAdjustmentData(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.FULL)

            MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READY.ToString)

            Me.BsCancelButton.Enabled = False
            Me.BsExitButton.Enabled = True


            If Not Me.AllHomesAreDone Then

                MyClass.CurrentMode = ADJUSTMENT_MODES.HOME_PREPARING

                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                Me.BsExitButton.Enabled = False

                MyClass.IsArmsToHoming = True

                ' Manage FwScripts must to be sent at load screen
                If MyBase.SimulationMode Then
                    'MyBase.DisplaySimulationMessage("Home for all Arms")
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()

                    Me.Cursor = Cursors.WaitCursor
                    BsHomingSimulationTimer.Enabled = True
                Else
                    MyClass.SendFwScript(Me.CurrentMode)
                    Me.BsExitButton.Enabled = False
                End If
            Else

                MyBase.CurrentMode = ADJUSTMENT_MODES.HOME_FINISHED
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjustPreparedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAdjustPreparedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Scren Loaded Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/01/2011</remarks>
    Private Sub PrepareLoadedMode()
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.IsTabChangeRequested Then Exit Sub 'SGM 23/1/2011

            If Not MyClass.IsScreenCloseRequested Then

                If MyBase.SimulationMode Then
                    Select Case MyClass.CurrentActionStep
                        Case ActionSteps.PREVIOUS_READING
                            MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                            MyClass.StartActionTimer()
                            Application.DoEvents()

                            'SGM 22/11/2011
                        Case ActionSteps.PREVIOUS_CHECKING
                            If MyClass.IsActionNotAllowed Then
                                MyClass.IsActionNotAllowed = False
                                MyClass.EnableCurrentPage()
                                Application.DoEvents()
                            Else
                                MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                                MyClass.StartActionTimer()
                                Application.DoEvents()
                            End If
                            'SGM 22/11/2011

                        Case ActionSteps.ACTION_REQUESTING
                            MyClass.CurrentActionStep = ActionSteps.FINAL_READING
                            MyClass.StartReadingTimer()
                            Application.DoEvents()

                        Case ActionSteps.FINAL_READING
                            MyClass.CurrentActionStep = ActionSteps._NONE

                            MyClass.SimulateSwitchCompleted()
                            If MyClass.SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                                MyClass.Indo_UpdateSource()
                            End If

                            If Not MyClass.IsContinuousSwitching Then
                                MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                                MyClass.EnableCurrentPage()

                            Else

                                'keep on
                                System.Threading.Thread.Sleep(SwitchInterval) 'PDT continuous switch interval?

                                If MyClass.CurrentActivationMode = ACTIVATION_MODES.CONTINUOUS Then
                                    Select Case MyClass.SwitchRequestedElement.ActivationState
                                        Case BsScadaControl.States._OFF
                                            MyClass.RequestedNewState = BsScadaControl.States._ON
                                        Case BsScadaControl.States._ON
                                            MyClass.RequestedNewState = BsScadaControl.States._OFF
                                    End Select
                                ElseIf MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE Then
                                    MyClass.IsContinuousSwitching = False
                                End If
                                If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl And MyClass.RequestedNewState = BsScadaControl.States._ON Then
                                    SimulatePumpPreActivations(MyClass.SwitchRequestedElement.Identity)
                                End If

                                MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                                MyClass.StartActionTimer()
                            End If
                            Application.DoEvents()

                        Case ActionSteps._NONE
                            If MyClass.SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                                MyClass.Indo_UpdateSource()
                            End If
                            If Not MyClass.IsTabChangeRequested Then
                                MyClass.EnableCurrentPage()
                            End If

                        Case ActionSteps.ACTION_REJECTED
                            MyClass.EnableCurrentPage()

                    End Select

                Else

                    If Not MyClass.IsLoaded Then
                        'before first reading
                        MyClass.IsActionRequested = True
                        MyClass.IsLoaded = True
                        MyClass.EnableCurrentPage()
                        MyClass.StartReadingTimer()
                    Else
                        If Not IsFirstReadingDone Then
                            If Not MyClass.IsReading Then
                                'first reading
                                MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                                MyClass.IsFirstReadingDone = True
                                MyClass.IsActionRequested = False
                            End If
                        ElseIf MyClass.CurrentActionStep <> ActionSteps._NONE Then

                            Select Case MyClass.CurrentActionStep

                                Case ActionSteps.PREVIOUS_READING
                                    ' XBC 09/11/2011 - change order set variable
                                    MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                                    MyClass.StartActionTimer()
                                    Application.DoEvents()

                                    '    'SGM 22/11/2011
                                    'Case ActionSteps.PREVIOUS_CHECKING
                                    '    If MyClass.IsActionNotAllowed Then
                                    '        MyClass.IsActionNotAllowed = False
                                    '        MyClass.EnableCurrentPage()
                                    '        Application.DoEvents()
                                    '    Else
                                    '        MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                                    '        MyClass.StartActionTimer()
                                    '        Application.DoEvents()
                                    '    End If
                                    '    'SGM 22/11/2011

                                Case ActionSteps.ACTION_REQUESTING
                                    ' XBC 09/11/2011 - change order set variable
                                    MyClass.CurrentActionStep = ActionSteps.FINAL_READING
                                    MyClass.StartReadingTimer()
                                    Application.DoEvents()

                                Case ActionSteps.FINAL_READING

                                    MyClass.CurrentActionStep = ActionSteps._NONE
                                    Application.DoEvents()

                                    ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!
                                    MyClass.SimulateSwitchCompleted()
                                    If MyClass.SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                                        MyClass.Indo_UpdateSource()
                                    End If
                                    ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!

                                    If Not MyClass.IsContinuousSwitching Then
                                        MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                                        MyClass.IsActionRequested = False
                                        MyClass.UnSelectElement(True)

                                        If MyClass.MoveRequestedElement IsNot Nothing Then
                                            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                                                ' XBC 10/11/2011 
                                                'MyClass.CurrentMotorAdjustControl.Enabled = True
                                                ''MyClass.CurrentMotorAdjustControl.Focus()
                                                ' XBC 10/11/2011 
                                            End If
                                            MyClass.MoveRequestedElement.Enabled = True
                                            'MyClass.MoveRequestedElement = Nothing ' XBC 10/11/2011 
                                        End If
                                    Else
                                        'If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl And MyClass.RequestedNewState = BsScadaControl.States._ON Then
                                        '    SimulatePumpPreActivations(MyClass.SwitchRequestedElement.Identity)
                                        'End If
                                        'keep on
                                        System.Threading.Thread.Sleep(SwitchInterval) 'PDT continuous switch interval?
                                        MyClass.StartActionTimer()
                                        MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING
                                        Application.DoEvents()

                                    End If

                                Case ActionSteps._NONE
                                    If MyClass.SelectedPage = TEST_PAGES.INTERNAL_DOSING Then
                                        MyClass.Indo_UpdateSource()
                                    End If

                                    'If Not MyClass.IsLoaded Then
                                    '    MyBase.DisplaySimulationMessage("Screen successfully loaded")
                                    'Else
                                    '    MyBase.DisplaySimulationMessage("Screen successfully loaded")
                                    'End If

                                    If Not MyClass.IsTabChangeRequested Then
                                        MyClass.EnableCurrentPage()
                                    End If

                                Case ActionSteps.ACTION_REJECTED
                                    MyClass.EnableCurrentPage()


                            End Select

                        Else


                            MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                            MyClass.IsActionRequested = False
                            MyClass.UnSelectElement(True)

                            If MyClass.MoveRequestedElement IsNot Nothing Then
                                If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                                    ' XBC 10/11/2011 
                                    MyClass.CurrentMotorAdjustControl.Enabled = True
                                    MyClass.CurrentMotorAdjustControl.Focus()
                                    Select Case MyClass.SelectedMotor.Name
                                        Case "InDo_Samples_Motor"
                                            MyClass.InDo_Samples_Motor.Select()
                                        Case "InDo_Reagent1_Motor"
                                            MyClass.InDo_Reagent1_Motor.Select()
                                        Case "InDo_Reagent2_Motor"
                                            MyClass.InDo_Reagent2_Motor.Select()
                                        Case "WsDisp_M1_Motor"
                                            MyClass.WsDisp_M1_Motor.Select()
                                        Case "BsScadaMotorControl"
                                            MyClass.Enco_MotorAdjustControl.Focus()
                                    End Select
                                    ' XBC 10/11/2011 
                                End If
                                MyClass.MoveRequestedElement.Enabled = True
                                MyClass.MoveRequestedElement = Nothing
                            End If


                            MyClass.EnableCurrentPage()


                        End If

                    End If

                End If

            Else

                MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_COMPLETED.ToString)
                'System.Threading.Thread.Sleep(1000) 'quitar
                Me.Close()

            End If

            If MyClass.SelectedPage = TEST_PAGES.COLLISION_DETECTION Then
                MyClass.InitializeCollision()
                Me.BsCollisionSimulationTimer.Enabled = False
                If MyClass.IsScreenCloseRequested Then
                    MyClass.AllItemsToDefault() 'set to default state all the previous tab elements
                End If
            Else


                MyClass.IsLoaded = True
            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Fw Events Enabling Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/07/2011</remarks>
    Private Sub PrepareCollisionTestEnablingMode()

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

            Me.Col_StartStopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareCollisionTestEnablingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareCollisionTestEnablingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Fw Events Enabled Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/07/2011</remarks>
    Private Sub PrepareCollisionTestEnabledMode()

        Dim myGlobal As New GlobalDataTO

        Try

            myGlobal = MyBase.DisplayMessage(Messages.SRV_COLLISION_TEST_READY.ToString)
            MyClass.IsCollisionTestEnabled = True
            If MyBase.SimulationMode Then
                Me.BsCollisionSimulationTimer.Enabled = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareCollisionTestEnabledMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareCollisionTestEnabledMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Fw Events Disabling Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/07/2011</remarks>
    Private Sub PrepareCollisionTestDisablingMode()

        Dim myGlobal As New GlobalDataTO

        Try
            'myGlobal = MyBase.DisplayMessage(Messages.SRV_DISABLING_FW_EVENTS.ToString)
            'Me.Col_StartButton.Enabled = False
            Me.Col_StartStopButton.Enabled = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareCollisionTestDisablingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareCollisionTestDisablingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/05/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Tested Mode
    ''' </summary>
    ''' <remarks>Created by XBC 19/04/2011</remarks>
    Private Sub PrepareTestedMode()
        Dim myResultData As New GlobalDataTO
        Try


            MyClass.IsAnyChanged = True
            If MyClass.IsActionRequested Then

                'HISTORY


                If MyClass.MoveRequestedElement Is Me.Enco_ReactionsRotorMotor Then 'encoder test
                    If Me.Enco_ReactionsRotorMotor.CurrentPosition <> Me.Enco_ReactionsRotorEncoderPos Then
                        MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.NOK)
                    Else
                        MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.OK)
                    End If

                    'ElseIf (MyClass.MoveRequestedElement Is Me.InDo_Samples_Motor) Or (MyClass.MoveRequestedElement Is Me.InDo_Reagent1_Motor) Or (MyClass.MoveRequestedElement Is Me.InDo_Reagent2_Motor) Or (MyClass.MoveRequestedElement Is Me.WsDisp_M1_Motor) Then
                    '    MyClass.EnableCurrentPage()
                    '    If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                    '        MyClass.CurrentMotorAdjustControl.Enabled = True
                    '        MyClass.CurrentMotorAdjustControl.Focus()
                    '    End If

                ElseIf MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE Then 'motors, pumps, valves
                    If (MyClass.SwitchRequestedElement IsNot Nothing AndAlso MyClass.SwitchRequestedElement.ActivationState = MyClass.SwitchRequestedElement.DefaultState) OrElse _
                    (MyClass.MoveRequestedElement IsNot Nothing AndAlso MyClass.MoveRequestedElement.ActivationState = MyClass.MoveRequestedElement.DefaultState) Then

                        'WS Aspiration or Dispensation SGM 18/05/2012
                        If MyClass.CurrentTestPanel Is Me.BsWSAspirationTestPanel Then
                            Me.WSAsp_UpDownButton.Enabled = True 'MyClass.DefaultStatesDone
                        End If
                        If MyClass.CurrentTestPanel Is Me.BsWSDispensationTestPanel Then
                            Me.WSDisp_UpDownButton.Enabled = True ' MyClass.DefaultStatesDone
                        End If
                    End If
                    MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.OK)


                    'SGM 18/05/2012 Continuous switch must stop at default state
                ElseIf MyClass.CurrentActivationMode = ACTIVATION_MODES.CONTINUOUS AndAlso MyClass.StopContinuousSwitchingRequested Then
                    'SGM 06/11/2012
                    If AnalyzerController.Instance.Analyzer.IsAlarmInfoRequested Then '#REFACTORING
                        System.Threading.Thread.Sleep(1000)
                    End If

                    MyClass.RequestedNewState = SelectedElement.DefaultState
                    MyClass.StopContinuousSwitchingRequested = False
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE
                    MyClass.RequestSwitchElement(SwitchRequestedElement, True) 'Force to default
                    MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.OK)

                End If

                'If MyClass.MoveRequestedElement IsNot Nothing Then
                '    MyClass.EnableCurrentPage()
                '    If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                '        MyClass.CurrentMotorAdjustControl.Focus()
                '        MyClass.CurrentMotorAdjustControl.Enabled = True
                '    End If
                'End If

                'MyClass.CurrentActionStep = ActionSteps.FINAL_READING
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()

                If MyClass.SelectedMotor IsNot Nothing Then
                    If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                        MyClass.CurrentMotorAdjustControl.Focus()
                        MyClass.CurrentMotorAdjustControl.Enabled = True
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Action is not allowed Mode
    ''' </summary>
    ''' <remarks>Created by SGM 22/11/2011</remarks>
    Private Sub PrepareActionNotAllowedMode(ByVal pElement As BsScadaControl)
        Dim myResultData As New GlobalDataTO
        Try

            MyClass.IsActionNotAllowed = True

            'HISTORY
            If MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE Or MyClass.StopContinuousSwitchingRequested Then 'motors, pumps, valves
                MyClass.SelectedElement = pElement
                MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.CANCEL)
                MyClass.SelectedElement = Nothing
            End If

            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            MyClass.PrepareArea()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestNotAllowedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestNotAllowedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Homes Finished Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareHomesFinishedMode()
        Try
            Dim myGlobal As New GlobalDataTO

            myGlobal = myScreenDelegate.SetPreliminaryHomesAsDone(ADJUSTMENT_GROUPS.MOTORS_PUMPS_VALVES)
            If Not myGlobal.HasError Then
                MyClass.IsArmsToHoming = False
                MyClass.AllHomesAreDone = True
                MyBase.DisplayMessage(Messages.SRV_HOMES_FINISHED.ToString)

                MyClass.IsArmsToWashing = True

                If MyBase.SimulationMode Then

                    'MyBase.DisplaySimulationMessage("All Arms to Washing")
                    MyBase.DisplayMessage(Messages.SRV_ARMS_TO_WASHING.ToString)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_WASHING
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_WASHING
                    MyClass.PrepareArea()
                Else
                    MyClass.AllArmsToWashing()
                End If
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareHomesFinishedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareHomesFinishedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Analyzer Info Readed Mode
    ''' </summary>
    ''' <remarks>Created by XBC 09/11/2011</remarks>
    Private Sub PrepareAnalyzerInfoReadedMode()
        Dim myResultData As New GlobalDataTO
        Try
            If myScreenDelegate.ReadAnalyzerInfoDone Then

                If Not MyClass.IsContinuousSwitching Then
                    Me.Cursor = Cursors.Default
                End If
                'SGM 22/11/2011
                'MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)

                'MyBase.myServiceMDI.SEND_INFO_START() 'SGM 22/11/2011

                Me.ProgressBar1.Value = 0
                Me.ProgressBar1.Visible = False
                Me.ProgressBar1.Refresh()

                PrepareLoadedMode()

            Else
                ' Continue with the Reading until finish it
                Me.Cursor = Cursors.WaitCursor

                Me.ProgressBar1.Visible = True
                Me.ProgressBar1.Value = myScreenDelegate.NumPOLLHW
                Me.ProgressBar1.Refresh()

                myResultData = MyBase.ReadAnalyzerInfo()
                If myResultData.HasError Then
                    PrepareErrorMode()
                    Exit Sub
                Else
                    myResultData = myScreenDelegate.REQUEST_ANALYZER_INFO()
                End If

            End If

            If myResultData.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAnalyzerInfoReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareAnalyzerInfoReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for All Arms in Washing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareAllArmsInWashingMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then
                'myGlobal = MyBase.DisplayMessage(Messages.SRV_ARMS_IN_WASHING.ToString)
                myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                MyClass.IsArmsToWashing = False
                MyClass.IsSettingDefaultStates = True

                If MyBase.SimulationMode Then
                    If Not MyClass.IsLoaded Then
                        'MyBase.DisplaySimulationMessage("All Items to their default state")
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_ALL_ITEMS_TO_DEFAULT.ToString)
                        MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                        System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                        MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                        MyClass.PrepareArea()
                    Else
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()
                    End If

                Else

                    If Not MyClass.IsLoaded Then
                        MyClass.AllItemsToDefault()
                    Else

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()

                    End If
                End If

            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()

            End If


            Exit Sub

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAllArmsInWashingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAllArmsInWashingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Washing Station is Down Mode
    ''' </summary>
    ''' <remarks>Created by SGM 21/11/2011</remarks>
    Private Sub PrepareWashingStationIsDownMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then

                MyClass.myScreenDelegate.IsWashingStationDown = True

                If MyBase.SimulationMode Then

                    'MyBase.DisplaySimulationMessage("Washing Station ready for the Test")
                    myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()


                Else
                    MyClass.IsTabChangeRequested = False
                    MyClass.IsActionRequested = False
                    MyClass.CurrentActionStep = ActionSteps._NONE
                    MyClass.EnableCurrentPage()
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()
                    MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)

                    'WS Aspiration or Dispensation SGM 18/05/2012
                    If MyClass.CurrentTestPanel Is Me.BsWSAspirationTestPanel Then
                        Me.WSAsp_UpDownButton.Enabled = True 'MyClass.DefaultStatesDone
                    End If
                    If MyClass.CurrentTestPanel Is Me.BsWSDispensationTestPanel Then
                        Me.WSDisp_UpDownButton.Enabled = True ' MyClass.DefaultStatesDone
                    End If

                End If


            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWashingStationIsDownMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareWashingStationIsDownMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try


    End Sub


    ''' <summary>
    ''' Prepare GUI for Washing Station is Downing Mode (NRotor)
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub PrepareWashingStationIsDowningMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then

                Me.WSAsp_UpDownButton.Enabled = False
                Me.WSDisp_UpDownButton.Enabled = False

                If MyBase.SimulationMode Then
                    myGlobal = MyBase.DisplayMessage(Messages.SRV_WS_TO_DOWN.ToString)
                    MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_DOWN
                    MyClass.PrepareArea()
                End If
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWashingStationIsDowningMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareWashingStationIsDowningMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' Prepare GUI for Washing Station is Up Mode
    ''' </summary>
    ''' <remarks>Created by SGM 21/11/2011</remarks>
    Private Sub PrepareWashingStationIsUpMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then

                MyClass.myScreenDelegate.IsWashingStationDown = False

                If MyBase.SimulationMode Then

                    MyClass.DisableCurrentPage()
                    MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                    myGlobal = MyBase.DisplayMessage(Messages.SRV_ALL_ITEMS_TO_DEFAULT.ToString)
                    MyClass.PrepareArea()

                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)

                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()


                Else

                    If Not MyClass.IsScreenCloseRequested Then
                        MyClass.IsActionRequested = False

                        MyClass.EnableCurrentPage()

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()

                        'WS Aspiration or Dispensation SGM 18/05/2012
                        If MyClass.CurrentTestPanel Is Me.BsWSAspirationTestPanel Or MyClass.CurrentTestPanel Is Me.BsWSDispensationTestPanel Then
                            MyBase.DisplayMessage(Messages.SRV_MEBV_WS_IS_UP.ToString)
                        Else
                            MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                        End If

                    Else
                        MyClass.AllArmsToParking()
                    End If
                End If
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWashingStationIsUpMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareWashingStationIsUpMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try



    End Sub

    ''' <summary>
    ''' Prepare GUI for All Arms in Parking Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareAllArmsInParkingMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then

                myGlobal = MyBase.DisplayMessage(Messages.SRV_ARMS_IN_PARKING.ToString)
                MyClass.IsArmsToParking = False

                ' XBC 09/11/2011
                If MyClass.IsScreenCloseRequested Then
                    MyClass.IsReadyToCloseAttr = True
                    Me.Close()
                    Application.DoEvents()

                    If MyBase.CloseRequestedByMDI Then
                        If MyBase.CloseWithShutDownRequestedByMDI Then
                            MyBase.myServiceMDI.WithShutToolStripMenuItem.PerformClick()
                        ElseIf MyBase.CloseWithoutShutDownRequestedByMDI Then
                            MyBase.myServiceMDI.WithOutShutDownToolStripMenuItem.PerformClick()
                        Else
                            Me.Close()
                        End If

                    End If

                    Exit Sub

                End If
                ' XBC 09/11/2011
                If MyBase.SimulationMode Then
                    MyClass.IsActionRequested = False
                    'MyBase.DisplaySimulationMessage("All arms in parking")
                Else
                    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                    MyClass.PrepareArea()
                End If
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

            ' XBC 09/11/2011
            'Exit Sub

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAllArmsInParkingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAllArmsInParkingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for All Items Switched Off Mode
    ''' </summary>
    ''' <remarks>Created by SGM 11/03/11</remarks>
    Private Sub PrepareAllSwitchedOffMode()
        Try
            Dim myGlobal As New GlobalDataTO

            If Not myGlobal.HasError Then
                'myGlobal = MyBase.DisplayMessage(Messages.SRV_ITEMS_IN_DEFAULT.ToString)
                'myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)

                MyClass.IsSettingDefaultStates = False

                ' XBC 11/11/2011
                MyClass.DefaultStatesDone = True
                ' XBC 11/11/2011

                If MyBase.SimulationMode Then
                    If Not MyClass.IsScreenCloseRequested Then

                        MyClass.SimulateSetToDefault()
                        MyClass.AsumeAllSwitchOff()
                        'MyBase.DisplaySimulationMessage("All items in default states")
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_READY.ToString)
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()
                        ' XBC 10/11/2011
                    Else
                        MyClass.WashingStationToUp()
                        MyClass.AllArmsToParking()
                    End If
                Else

                    'while asuming Open Loop
                    MyClass.AsumeAllSwitchOff() 'we asume that all elements are switched off

                    'MyClass.myScreenDelegate.IsWashingStationDown = False

                    If Not MyClass.IsScreenCloseRequested Then
                        ' XBC 10/11/2011

                        If Not MyClass.IsFirstReadingDone Then
                            MyClass.myScreenDelegate.IsWashingStationDown = True
                            MyClass.WashingStationToUp()
                            'MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                            'MyClass.PrepareArea()

                        ElseIf MyClass.IsActionRequested And Not MyClass.IsReading Then
                            MyClass.StartReadingTimer()

                        Else
                            If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Or MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
                                MyClass.myScreenDelegate.IsWashingStationDown = False
                                MyClass.WashingStationToDown()
                            Else
                                If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Or MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
                                    MyClass.myScreenDelegate.IsWashingStationDown = False
                                    MyClass.WashingStationToDown()
                                Else
                                    MyClass.myScreenDelegate.IsWashingStationDown = True
                                    MyClass.WashingStationToUp()
                                End If
                                'MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                                'MyClass.PrepareArea()
                            End If

                        End If



                        'If Not MyClass.IsFirstReadingDone Then
                        '    MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        '    MyClass.PrepareArea()
                        'Else
                        '    If MyClass.IsActionRequested And Not MyClass.IsReading Then
                        '        MyClass.StartReadingTimer()
                        '    End If
                        'End If
                        ' XBC 10/11/2011
                    Else
                        If MyClass.myScreenDelegate.IsWashingStationDown Then
                            MyClass.WashingStationToUp()
                        Else
                            MyClass.AllArmsToParking()
                        End If
                    End If
                End If

                MyClass.Indo_UpdateSource()

            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAllSwitchedOffMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareAllSwitchedOffMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#End Region

#Region "Test Methods"

    ''' <summary>
    ''' Brings all arms to their Washing position
    ''' </summary>
    ''' <remarks>SGM 14/05/2011</remarks>
    Private Sub AllArmsToWashing()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_WASHING

            MyClass.IsArmsToWashing = True

            MyClass.StartArmsToWashingTimer()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AllArmsToWashing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AllArmsToWashing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Brings all arms to their Parking position
    ''' </summary>
    ''' <remarks>SGM 14/05/2011</remarks>
    Private Sub AllArmsToParking()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_PARKING
            MyClass.PrepareCommonAreas()

            MyClass.IsArmsToParking = True

            MyClass.StartArmsToParkingTimer()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AllArmsToParking ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AllArmsToParking ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Brings the Washing Station Down
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 21/11/2011
    ''' Modified by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub WashingStationToDown()
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.myScreenDelegate.IsWashingStationDown Then Exit Sub

            'SGM 18/05/2012
            Me.WSAsp_UpDownButton.Enabled = False
            Me.WSDisp_UpDownButton.Enabled = False


            'in case some motor selected
            MyClass.UnSelectMotor()

            MyBase.DisplayMessage(Messages.SRV_WS_TO_DOWN.ToString)

            MyClass.myScreenDelegate.IsWashingStationDown = True

            MyClass.DisableCurrentPage()

            'MyClass.StartWashingStationToDownTimer()
            MyClass.StartWashingStationToNRotorTimer()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WashingStationToDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WashingStationToDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Brings the Washing Station Down
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub WashingStationToDownAfterNRotor()
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.StartWashingStationToDownTimer()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WashingStationToDownAfterNRotor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WashingStationToDownAfterNRotor ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Brings the Washing Station Up
    ''' </summary>
    ''' <remarks>SGM 21/11/2011</remarks>
    Private Sub WashingStationToUp()
        Dim myGlobal As New GlobalDataTO
        Try
            If MyBase.SimulationMode Then Exit Sub

            If Not MyClass.myScreenDelegate.IsWashingStationDown Then Exit Sub

            'SGM 18/05/2012
            Me.WSAsp_UpDownButton.Enabled = False
            Me.WSDisp_UpDownButton.Enabled = False

            'in case some motor selected
            MyClass.UnSelectMotor()

            MyBase.DisplayMessage(Messages.SRV_WS_TO_UP.ToString)

            MyClass.myScreenDelegate.IsWashingStationDown = False

            MyClass.DisableCurrentPage()

            MyClass.StartWashingStationToUpTimer()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WashingStationToDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WashingStationToDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' Sets all items to their default values
    ''' </summary>
    ''' <remarks>SGM 14/05/2011</remarks>
    Private Sub AllItemsToDefault()
        Dim myGlobal As New GlobalDataTO
        Try
            'in case some motor selected
            MyClass.UnSelectMotor()

            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
            MyClass.PrepareCommonAreas()

            MyClass.IsSettingDefaultStates = True

            ' XBC 11/11/2011
            'If Not MyBase.SimulationMode Then
            '    MyClass.StartSettingDefaultTimer()
            'End If
            MyClass.StartSettingDefaultTimer()
            ' XBC 11/11/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".AllItemsToDefault ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".AllItemsToDefault ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Defines the needed parameters for the screen delegate in order to make a test
    ''' </summary>
    ''' <remarks>SGM 14/05/2011</remarks>
    Private Sub TestSwitch()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Test()
            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            Else

                If MyBase.SimulationMode Then
                    ' simulating
                    'MyBase.DisplaySimulationMessage("Switching Element State...")
                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(SimulationProcessTime)
                    MyBase.myServiceMDI.Focus()
                    Me.Cursor = Cursors.Default
                    'LoadAdjustmentGroupData()
                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTED
                    MyClass.PrepareArea()
                Else


                    MyClass.SendFwScript(Me.CurrentMode)

                    Me.Cursor = Cursors.WaitCursor

                    'SGM 18/04/2012
                    'assume that the requested action is succsessfully performed
                    If MyClass.SwitchRequestedElement IsNot Nothing Then
                        If TypeOf MyClass.SwitchRequestedElement Is BsScadaPumpControl And MyClass.RequestedNewState = BsScadaControl.States._ON Then
                            SimulatePumpPreActivations(MyClass.SwitchRequestedElement.Identity)
                        End If
                        MyClass.SwitchRequestedElement.ActivationState = MyClass.RequestedNewState
                        Me.Refresh()
                        Application.DoEvents()
                    End If
                    'end SGM 18/04/2012

                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".TestSwitch ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".TestSwitch ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pNewPosition"></param>
    ''' <remarks>SGM 14/05/2011</remarks>
    Private Function TestMove(ByVal pNewPosition As Integer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyClass.SelectedMotor IsNot Nothing Then

                myGlobal = MyBase.Test
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    Select Case MyClass.MoveRequestedMode
                        Case MOVEMENT.HOME
                            MyClass.myScreenDelegate.RelativePositioning = 0
                            MyClass.myScreenDelegate.AbsolutePositioning = 0

                        Case MOVEMENT.RELATIVE
                            MyClass.myScreenDelegate.RelativePositioning = pNewPosition
                            MyClass.myScreenDelegate.AbsolutePositioning = 0

                        Case MOVEMENT.ABSOLUTE
                            MyClass.myScreenDelegate.RelativePositioning = 0
                            MyClass.myScreenDelegate.AbsolutePositioning = pNewPosition

                    End Select

                    SendFwScript(Me.CurrentMode)

                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MoveTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".MoveTest ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myGlobal

    End Function

#End Region

#Region "Script Methods"

    ''' <summary>
    ''' Generic function to send FwScripts to the Instrument through own delegates
    ''' </summary>
    ''' <param name="pMode"></param>
    ''' <param name="pParamList"></param>
    ''' <remarks>Created by: XBC 19/04/2011</remarks>
    Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES, _
                             Optional ByVal pParamList As List(Of String) = Nothing)
        Dim myGlobal As New GlobalDataTO
        Try
            ' XBC 11/11/2011
            If MyClass.IsActionRequested Then 'SGM 22/11/2011
                MyClass.DefaultStatesDone = False
            End If
            ' XBC 11/11/2011

            If AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                MyClass.PrepareErrorMode()
                MyBase.DisplayMessage("")
                Exit Sub
            End If

            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode)
                If Not myGlobal.HasError Then
                    ' Send FwScripts
                    myGlobal = myFwScriptDelegate.StartFwScriptQueue
                End If
            Else
                myGlobal.HasError = True
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If
                GlobalBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Arms To Washing Methods"

    Private Function StartArmsToWashingTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyBase.SimulationMode Then
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_WASHING
                MyClass.PrepareArea()
            Else
                ' XBC 08/11/2011
                'MyClass.ArmsToWashingTimerCallBack = New System.Threading.TimerCallback(AddressOf OnArmsToWashingTimerTick)
                'MyClass.ArmsToWashingTimer = New System.Threading.Timer(MyClass.ArmsToWashingTimerCallBack, New Object, 100, 0)
                Me.OnArmsToWashingTimerTick()
                ' XBC 08/11/2011

                myGlobal = MyBase.DisplayMessage(Messages.SRV_ARMS_TO_WASHING.ToString)
            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartArmsToWashingTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ' XBC 08/11/2011
    ' Private Sub OnArmsToWashingTimerTick(ByVal stateInfo As Object)
    Private Sub OnArmsToWashingTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.IsActionRequested = True

            If Not myGlobal.HasError Then
                MyClass.SendFwScript(Me.CurrentMode)
                Me.Cursor = Cursors.WaitCursor
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnArmsToWashingTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Arms To Parking Methods"

    Private Function StartArmsToParkingTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyBase.SimulationMode Then
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_PARKING
                MyClass.PrepareArea()
            Else
                ' XBC 08/11/2011
                'MyClass.ArmsToParkingTimerCallBack = New System.Threading.TimerCallback(AddressOf OnArmsToParkingTimerTick)
                'MyClass.ArmsToParkingTimer = New System.Threading.Timer(MyClass.ArmsToParkingTimerCallBack, New Object, 100, 0)
                Me.OnArmsToParkingTimerTick()
                ' XBC 08/11/2011
                myGlobal = MyBase.DisplayMessage(Messages.SRV_ARMS_TO_PARKING.ToString)
            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartArmsToParkingTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    'SGM 21/11/2011
    Private Function StartWashingStationToDownTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.DisableCurrentPage()

            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_DOWN
            MyClass.PrepareArea()

            If MyBase.SimulationMode Then

                System.Threading.Thread.Sleep(SimulationProcessTime)

                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_DOWN
                MyClass.PrepareArea()
            Else

                Me.OnWashingStationToDownTimerTick()

            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartWashingStationToDownTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Previous step to WASHING_STATION_IS_DOWN
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Function StartWashingStationToNRotorTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.DisableCurrentPage()

            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_NROTOR
            MyClass.PrepareArea()

            If MyBase.SimulationMode Then

                System.Threading.Thread.Sleep(SimulationProcessTime)

                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_DOWN
                MyClass.PrepareArea()
            Else
                Me.OnWashingStationToNRotorTimerTick()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartWashingStationToNRotorTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    'SGM 21/11/2011
    Private Function StartWashingStationToUpTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            MyClass.DisableCurrentPage()

            MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_UP
            MyClass.PrepareArea()

            If MyBase.SimulationMode Then

                System.Threading.Thread.Sleep(SimulationProcessTime)

                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_UP
                MyClass.PrepareArea()
            Else

                Me.OnWashingStationToUpTimerTick()

            End If



        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartWashingStationToUpTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    ' XBC 08/11/2011
    'Private Sub OnArmsToParkingTimerTick(ByVal stateInfo As Object)
    Private Sub OnArmsToParkingTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.IsActionRequested = True

            If Not myGlobal.HasError Then
                MyClass.SendFwScript(Me.CurrentMode)
                Me.Cursor = Cursors.WaitCursor
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnArmsToParkingTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Sends NROTOR instruction
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 15/10/2014 - Use NROTOR when Wash Station is down - BA-2004
    ''' </remarks>
    Private Sub OnWashingStationToNRotorTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try

            If Not myGlobal.HasError Then
                Me.Cursor = Cursors.WaitCursor
                myScreenDelegate.SendNEW_ROTOR()
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnWashingStationToNRotorTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    'SGM 21/11/2011
    Private Sub OnWashingStationToDownTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            'MyClass.IsActionRequested = True

            If Not myGlobal.HasError Then

                'myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.DOWN)
                MyClass.SendFwScript(Me.CurrentMode)
                Me.Cursor = Cursors.WaitCursor
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnWashingStationToDownTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    'SGM 21/11/2011
    Private Sub OnWashingStationToUpTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            'MyClass.IsActionRequested = True

            If Not myGlobal.HasError Then
                myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
                'MyClass.SendFwScript(Me.CurrentMode)
                'Me.Cursor = Cursors.WaitCursor
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnWashingStationToUpTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Setting to Default Methods"

    Private Function StartSettingDefaultTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try


            If MyBase.SimulationMode Then
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                MyClass.PrepareArea()
            Else
                ' XBC 08/11/2011
                'MyClass.SettingDefaultTimerCallBack = New System.Threading.TimerCallback(AddressOf OnSettingDefaultTimerTick)
                'MyClass.SettingDefaultTimer = New System.Threading.Timer(MyClass.SettingDefaultTimerCallBack, New Object, 100, 0)
                Me.OnSettingDefaultTimerTick()
                ' XBC 08/11/2011
            End If

            Me.WSAsp_UpDownButton.Enabled = False
            Me.WSDisp_UpDownButton.Enabled = False

            myGlobal = MyBase.DisplayMessage(Messages.SRV_ALL_ITEMS_TO_DEFAULT.ToString)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartSettingDefaultTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ' XBC 08/11/2011
    'Private Sub OnSettingDefaultTimerTick(ByVal stateInfo As Object)
    Private Sub OnSettingDefaultTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            MyClass.IsActionRequested = True

            If Not myGlobal.HasError Then
                MyClass.SendFwScript(Me.CurrentMode)
                Me.Cursor = Cursors.WaitCursor
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnSettingDefaultTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Read Update Methods"

    Private Function StartReadingTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyBase.SimulationMode Then
                Dim myExcept As BsScadaControl = Nothing
                If MyClass.IsContinuousSwitching Then
                    If MyClass.SwitchRequestedElement IsNot Nothing Then
                        myExcept = MyClass.SwitchRequestedElement
                    End If
                End If
                MyClass.DisableCurrentPage(myExcept)
                'MyBase.DisplaySimulationMessage("Reading current values...")
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyClass.IsFirstReadingDone = True
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()

            Else
                ' XBC 08/11/2011
                'MyClass.ReadingTimerCallBack = New System.Threading.TimerCallback(AddressOf OnReadingTimerTick)
                'MyClass.ReadingTimer = New System.Threading.Timer(MyClass.ReadingTimerCallBack, New Object, 300, 0)
                Me.OnReadingTimerTick()
                ' XBC 08/11/2011
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartHardwareReaderTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ' XBC 08/11/2011
    'Private Sub OnReadingTimerTick(ByVal stateInfo As Object)
    Private Sub OnReadingTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try
            If MyClass.CurrentActionStep = ActionSteps.ACTION_REQUESTING Then
                MyBase.myServiceMDI.SEND_INFO_STOP(MyClass.IsWorking)
            End If

            If MyBase.SimulationMode Then

                ' ???

            Else

                Dim RequestManifold As Boolean = False
                Dim RequestFluidics As Boolean = False
                Dim RequestPhotometrics As Boolean = False
                If MyClass.SelectedElement IsNot Nothing Then
                    RequestManifold = (MyClass.SelectedElementGroup = ItemGroups.MANIFOLD)
                    RequestFluidics = (MyClass.SelectedElementGroup = ItemGroups.FLUIDICS)
                    RequestPhotometrics = (MyClass.SelectedElementGroup = ItemGroups.PHOTOMETRICS)
                Else
                    RequestManifold = True
                    RequestFluidics = True
                    RequestPhotometrics = True
                End If

                myGlobal = MyClass.ReadHardwareValues(RequestManifold, RequestFluidics, RequestPhotometrics)

                If Not myGlobal.HasError Then
                    If MyClass.CurrentActionStep <> ActionSteps.ACTION_REQUESTING Then
                        MyClass.PrepareArea()
                    End If
                Else
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareArea()
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnReadingTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Function ReadHardwareValues(ByVal pReadManifold As Boolean, _
                                        ByVal pReadFluidics As Boolean, _
                                        ByVal pReadPhotometrics As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            'Application.DoEvents()


            If MyBase.SimulationMode Then

                MyClass.IsActionRequested = True

                Me.Cursor = Cursors.WaitCursor
                BsHomingSimulationTimer.Enabled = True

                'SGM 22/11/2011
                MyBase.myServiceMDI.SEND_INFO_STOP(MyClass.IsWorking And Not MyClass.IsContinuousSwitching)
                System.Threading.Thread.Sleep(100)
                MyBase.myServiceMDI.SEND_INFO_START(MyClass.IsWorking)
                'SGM 22/11/2011

                ' XBC 09/11/2011
                myScreenDelegate.ReadAnalyzerInfoDone = True
                ' XBC 09/11/2011

            Else

                If Not MyClass.IsActionRequested Then MyBase.myServiceMDI.SEND_INFO_STOP(Not MyClass.CurrentTestPanel.Enabled) 'SGM 22/11/2011

                ' XBC 09/11/2011
                myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                'Me.ProgressBar1.Maximum = myScreenDelegate.MaxPOLLHW
                'Me.ProgressBar1.Value = 0

                ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!
                ' simulating...
                'Me.ProgressBar1.Visible = True
                'For i As Integer = 1 To myScreenDelegate.MaxPOLLHW
                '    Me.ProgressBar1.Value = i
                '    Me.ProgressBar1.Refresh()
                '    System.Threading.Thread.Sleep(1000)
                'Next
                myScreenDelegate.ReadAnalyzerInfoDone = True
                Me.IsFirstReadingDone = True
                PrepareAnalyzerInfoReadedMode()

                'If pReadManifold Or pReadFluidics Or pReadPhotometrics Then

                '    pReadPhotometrics = myScreenDelegate.ReadGLFInfoDone
                '    pReadManifold = myScreenDelegate.ReadJEXInfoDone
                '    pReadFluidics = myScreenDelegate.ReadSFXInfoDone

                '    myGlobal = myScreenDelegate.REQUEST_ANALYZER_INFO()

                '    If myGlobal.HasError Then
                '        Me.PrepareErrorMode()
                '        Exit Try
                '    End If
                'End If
                ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!

                'If pReadManifold Then
                '    myGlobal = MyClass.ReadManifoldElements()
                '    If myGlobal.HasError Then
                '        Throw New Exception(myGlobal.ErrorMessage)
                '    End If
                'End If

                'Application.DoEvents()

                'If pReadFluidics Then
                '    myGlobal = MyClass.ReadFluidicsElements()
                '    If myGlobal.HasError Then
                '        Throw New Exception(myGlobal.ErrorMessage)
                '    End If
                'End If

                'Application.DoEvents()

                'If pReadPhotometrics Then
                '    myGlobal = MyClass.ReadPhotometricsElements()
                '    If myGlobal.HasError Then
                '        Throw New Exception(myGlobal.ErrorMessage)
                '    End If
                'End If

                'Application.DoEvents()

                'MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                'MyClass.PrepareArea()
                ' XBC 09/11/2011

                If MyClass.IsContinuousSwitching Then

                    If CurrentActivationMode = ACTIVATION_MODES.SIMPLE Then
                        MyClass.IsContinuousSwitching = False
                    End If
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " ReadHardwareValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ' XBC 09/11/2011 
    '''' <summary>
    '''' Read Manifold Elements
    '''' </summary>
    '''' <remarks>Created by SGM 23/05/2011</remarks>
    'Private Function ReadManifoldElements() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = MyBase.DisplayMessage(Messages.SRV_READING_MANIFOLD.ToString)

    '        MyClass.IsManifoldRequested = True

    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!
    '        ' Manage FwScripts must to be sent at load screen
    '        'If MyBase.SimulationMode Then
    '        ' simulating
    '        MyBase.DisplaySimulationMessage("Reading Manifold Elements")
    '        Me.Cursor = Cursors.WaitCursor
    '        System.Threading.Thread.Sleep(SimulationProcessTime)
    '        MyBase.myServiceMDI.Focus()
    '        Me.Cursor = Cursors.Default
    '        MyBase.DisplaySimulationMessage("Manifold Elements readed")
    '        'SIMULATE RECEIVE MANIFOLD
    '        'Else
    '        '    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then
    '        '        myGlobal = myScreenDelegate.SendPOLLHW(POLL_IDs.JE1)
    '        '    End If
    '        'End If
    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadManifoldElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".ReadManifoldElements ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    '    Return myGlobal
    'End Function

    '''' <summary>
    '''' Read Fluidics Elements
    '''' </summary>
    '''' <remarks>Created by SGM 23/05/2011</remarks>
    'Private Function ReadFluidicsElements() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = MyBase.DisplayMessage(Messages.SRV_READING_FLUIDICS.ToString)

    '        MyClass.IsFluidicsRequested = True

    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!
    '        'If MyBase.SimulationMode Then
    '        ' simulating
    '        MyBase.DisplaySimulationMessage("Reading Fluidic Elements...")
    '        Me.Cursor = Cursors.WaitCursor
    '        System.Threading.Thread.Sleep(SimulationProcessTime)
    '        MyBase.DisplaySimulationMessage("")
    '        MyBase.myServiceMDI.Focus()
    '        Me.Cursor = Cursors.Default
    '        MyBase.DisplaySimulationMessage("Fluidic Elements readed")
    '        'SIMULATE RECEIVE FLUIDICS
    '        'Else
    '        '    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then
    '        '        myGlobal = myScreenDelegate.SendPOLLHW(POLL_IDs.SF1)
    '        '    End If
    '        'End If
    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadFluidicsElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".ReadFluidicsElements ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try

    '    Return myGlobal

    'End Function

    '''' <summary>
    '''' Read Photometrics Elements
    '''' </summary>
    '''' <remarks>Created by SGM 23/05/2011</remarks>
    'Private Function ReadPhotometricsElements() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = MyBase.DisplayMessage(Messages.SRV_READING_PHOTOMETRICS.ToString)

    '        MyClass.IsPhotometricsRequested = True

    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!
    '        'If MyBase.SimulationMode Then
    '        ' simulating
    '        MyBase.DisplaySimulationMessage("Reading Photometrics Elements...")
    '        Me.Cursor = Cursors.WaitCursor
    '        System.Threading.Thread.Sleep(SimulationProcessTime)
    '        MyBase.DisplaySimulationMessage("")
    '        MyBase.myServiceMDI.Focus()
    '        Me.Cursor = Cursors.Default
    '        MyBase.DisplaySimulationMessage("Photometrics Elements readed")
    '        'SIMULATE RECEIVE Photometrics
    '        'Else
    '        '    If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then
    '        '        myGlobal = myScreenDelegate.SendPOLLHW(POLL_IDs.GLF)
    '        '    End If
    '        'End If
    '        ' XBC 09/11/2011 - TEMPORAL - PENDING ON POLLHW INSTRUCTIONS !!!

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadPhotometricsElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyBase.ShowMessage(Me.Name & ".ReadPhotometricsElements ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try

    '    Return myGlobal

    'End Function
    ' XBC 09/11/2011 

#End Region

#Region "Action preparing methods"

    Private Function StartActionTimer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If MyBase.SimulationMode Then

                Dim myExcept As BsScadaControl = Nothing
                If MyClass.IsContinuousSwitching Then
                    If MyClass.SwitchRequestedElement IsNot Nothing Then
                        myExcept = MyClass.SwitchRequestedElement
                    End If
                End If
                MyClass.DisableCurrentPage(myExcept)
                'MyBase.DisplaySimulationMessage("Performing switching action...")
                System.Threading.Thread.Sleep(SimulationProcessTime)
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                MyClass.PrepareArea()

            Else
                ' XBC 08/11/2011
                'MyClass.ActionTimerCallBack = New System.Threading.TimerCallback(AddressOf OnActionTimerTick)
                'MyClass.ActionTimer = New System.Threading.Timer(MyClass.ActionTimerCallBack, New Object, 400, 0)
                Me.OnActionTimerTick()
                ' XBC 08/11/2011
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " StartActionTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function


    ' XBC 08/11/2011
    'Private Sub OnActionTimerTick(ByVal stateInfo As Object)
    Private Sub OnActionTimerTick()
        Dim myGlobal As New GlobalDataTO
        Try

            MyBase.myServiceMDI.SEND_INFO_STOP(MyClass.IsWorking)

            If MyBase.SimulationMode Then


            Else
                If MyClass.SwitchRequestedElement IsNot Nothing Then
                    myGlobal = MyClass.TryActivateElement
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not CBool(myGlobal.SetDatos) Then
                        MyClass.SwitchRequestedElement = Nothing
                        MyClass.RequestedNewState = BsScadaControl.States._NONE
                        MyClass.MoveRequestedElement = Nothing
                        MyClass.MoveRequestedMode = MOVEMENT._NONE
                        MyClass.CurrentActionStep = ActionSteps.ACTION_REJECTED
                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()
                    End If

                ElseIf MyClass.MoveRequestedElement IsNot Nothing Then
                    MyClass.myScreenDelegate.ActivationMode = SWITCH_ACTIONS._NONE
                    MyClass.myScreenDelegate.MovementMode = MyClass.MoveRequestedMode
                    MyClass.myScreenDelegate.Item = MoveRequestedElement.Identity
                    MyClass.myScreenDelegate.ItemType = ItemTypes.MOTOR.ToString
                    myGlobal = MyClass.TestMove(MyClass.RequestedNewPosition)
                End If

                If myGlobal.HasError Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.PrepareArea()
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " OnActionTimerTick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Util Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pFwValue"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function ConvertFwStringToBoolean(ByVal pFwValue As String) As Boolean
        Try
            'If pFwValue.ToUpper.Trim = GlobalEnumerates.HW_DC_STATES.DI.ToString Or _
            'pFwValue.ToUpper.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._OFF).ToString Then
            If pFwValue.ToUpperBS.Trim = GlobalEnumerates.HW_DC_STATES.DI.ToString Or _
            pFwValue.ToUpperBS.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._OFF).ToString Then
                Return False
                'ElseIf pFwValue.ToUpper.Trim = GlobalEnumerates.HW_DC_STATES.EN.ToString Or _
                'pFwValue.ToUpper.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._ON).ToString Then
            ElseIf pFwValue.ToUpperBS.Trim = GlobalEnumerates.HW_DC_STATES.EN.ToString Or _
            pFwValue.ToUpperBS.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._ON).ToString Then
                Return True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertFwStringToBoolean ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertFwStringToBoolean ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pFwValue"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function ConvertFwStringToState(ByVal pFwValue As String) As BsScadaControl.States
        Try
            'If pFwValue.ToUpper.Trim = GlobalEnumerates.HW_DC_STATES.DI.ToString Or _
            'pFwValue.ToUpper.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._OFF).ToString Then
            If pFwValue.ToUpperBS.Trim = GlobalEnumerates.HW_DC_STATES.DI.ToString Or _
            pFwValue.ToUpperBS.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._OFF).ToString Then
                Return BsScadaControl.States._OFF
                'ElseIf pFwValue.ToUpper.Trim = GlobalEnumerates.HW_DC_STATES.EN.ToString Or _
                'pFwValue.ToUpper.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._ON).ToString Then
            ElseIf pFwValue.ToUpperBS.Trim = GlobalEnumerates.HW_DC_STATES.EN.ToString Or _
            pFwValue.ToUpperBS.Trim = CInt(GlobalEnumerates.HW_BOOL_STATES._ON).ToString Then
                Return BsScadaControl.States._ON
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertFWStringToState ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertFWStringToState ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            Return BsScadaControl.States._NONE
        End Try
    End Function

    Private Function ConvertFwStringToDiagnosis(ByVal pDiagnosisValue As String) As Boolean
        Try
            If pDiagnosisValue.Length > 0 Then

                Return (CInt(pDiagnosisValue) <> CInt(GlobalEnumerates.HW_GENERIC_DIAGNOSIS.OK))


            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertFwStringToDiagnosis ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertFwStringToDiagnosis ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pMotor"></param>
    ''' <param name="pFwValue"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ConvertFwStringToHomeStatus(ByRef pMotor As BsScadaMotorControl, ByVal pFwValue As String) As Integer
        Try
            If pMotor IsNot Nothing Then
                'If pFwValue.ToUpper.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.C.ToString Then
                '    pMotor.CurrentPosition = 0
                '    pMotor.IsAlarm = False

                'ElseIf pFwValue.ToUpper.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.O.ToString Then
                '    'not to change
                '    pMotor.IsAlarm = False

                'ElseIf pFwValue.ToUpper.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.KO.ToString Then
                '    pMotor.IsAlarm = True
                'End If
                If pFwValue.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.C.ToString Then
                    pMotor.CurrentPosition = 0
                    pMotor.IsAlarm = False

                ElseIf pFwValue.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.O.ToString Then
                    'not to change
                    pMotor.IsAlarm = False

                ElseIf pFwValue.Trim = GlobalEnumerates.HW_MOTOR_HOME_STATES.KO.ToString Then
                    pMotor.IsAlarm = True
                End If

                Return pMotor.CurrentPosition
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertFwStringToHomeStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertFwStringToHomeStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function

    Private Function ConvertFwStringToCollisionStatus(ByVal pValue As String) As BSMonitorControlBase.Status
        Try
            If pValue.Length > 0 Then

                If CInt(pValue) > 0 Then
                    Return BSMonitorControlBase.Status._OFF
                Else
                    Return BSMonitorControlBase.Status._ON
                End If


            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ConvertFwStringToCollisionStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ConvertFwStringToCollisionStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Function
#End Region

#End Region

#Region "Internal Dosing Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Public Sub InDo_UpdateSyringeColors()
        Try

            If InDo_Samples_Pump.ActivationState = BsScadaControl.States._ON And InDo_Samples_EValve.PhysicalState = BsScadaValveControl.PhysicalStates.Open Then
                InDo_Samples_Syringe.FluidColor = MyClass.InterNalDosingSourceFluidColor
            Else
                InDo_Samples_Syringe.FluidColor = Color.Gainsboro
            End If

            If InDo_Reagent1_Pump.ActivationState = BsScadaControl.States._ON And InDo_Reagent1_EValve.PhysicalState = BsScadaValveControl.PhysicalStates.Open Then
                InDo_Reagent1_Syringe.FluidColor = MyClass.InterNalDosingSourceFluidColor
            Else
                InDo_Reagent1_Syringe.FluidColor = Color.Gainsboro
            End If

            If InDo_Reagent2_Pump.ActivationState = BsScadaControl.States._ON And InDo_Reagent2_EValve.PhysicalState = BsScadaValveControl.PhysicalStates.Open Then
                InDo_Reagent2_Syringe.FluidColor = MyClass.InterNalDosingSourceFluidColor
            Else
                InDo_Reagent2_Syringe.FluidColor = Color.Gainsboro
            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".UpdateSyringeColors ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".UpdateSyringeColors ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_RequestSwitchSource(ByVal pStep As Integer)

        Dim myGlobal As New GlobalDataTO

        Try


            'myGlobal = PrepareBeforeSwitchingElementState(pElement.Identity)

            If Not myGlobal.HasError Then

                MyClass.IsInternalDosingSourceChanging = True

                MyClass.UnSelectElement()

                Me.InDo_SourceGroupBox.Enabled = False

                If pStep = 1 Then
                    MyClass.SelectedElement = Me.InDo_Air_Ws_3Evalve
                    MyClass.SwitchRequestedElement = Me.InDo_Air_Ws_3Evalve
                    Select Case InterNalDosingSource
                        Case INTERNAL_DOSING_SOURCES.AIR
                            If MyClass.SelectedElement.ActivationState <> BsScadaControl.States._ON Then
                                MyClass.RequestSwitchElement(MyClass.SwitchRequestedElement)
                            Else
                                pStep = 2
                            End If

                        Case INTERNAL_DOSING_SOURCES.WASHING_SOLUTION
                            If MyClass.SelectedElement.ActivationState <> BsScadaControl.States._OFF Then
                                MyClass.RequestSwitchElement(MyClass.SwitchRequestedElement)
                            Else
                                pStep = 2
                            End If

                        Case INTERNAL_DOSING_SOURCES.DISTILLED_WATER
                            pStep = 2

                    End Select

                End If

                If pStep = 2 Then
                    MyClass.SelectedElement = Me.InDo_AirWs_Pw_3Evalve
                    MyClass.SwitchRequestedElement = Me.InDo_AirWs_Pw_3Evalve
                    Select Case InterNalDosingSource
                        Case INTERNAL_DOSING_SOURCES.AIR
                            If MyClass.SelectedElement.ActivationState <> BsScadaControl.States._ON Then
                                MyClass.RequestSwitchElement(MyClass.SwitchRequestedElement)
                            Else
                                MyClass.IsInternalDosingSourceChanging = False
                            End If

                        Case INTERNAL_DOSING_SOURCES.WASHING_SOLUTION
                            If MyClass.SelectedElement.ActivationState <> BsScadaControl.States._ON Then
                                MyClass.RequestSwitchElement(MyClass.SwitchRequestedElement)
                            Else
                                MyClass.IsInternalDosingSourceChanging = False
                            End If

                        Case INTERNAL_DOSING_SOURCES.DISTILLED_WATER
                            If MyClass.SelectedElement.ActivationState <> BsScadaControl.States._OFF Then
                                MyClass.RequestSwitchElement(MyClass.SwitchRequestedElement)
                            Else
                                MyClass.IsInternalDosingSourceChanging = False
                            End If

                    End Select

                End If


                If MyClass.IsInternalDosingSourceChanging Then
                    'If MyBase.SimulationMode Then

                    '    BsSimulationTimer.Enabled = True

                    'Else
                    '    TestSwitch()
                    'End If

                    'MyClass.UnSelectMotor()

                Else
                    MyClass.Indo_UpdateSource()
                    Me.InDo_SourceGroupBox.Enabled = True
                    MyClass.IsActionRequested = False
                    MyClass.UnSelectElement()
                End If
            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_RequestSwitchSource ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_RequestSwitchSource ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try
    End Sub



    Private Sub Indo_UpdateSource()
        Try
            Select Case Me.InDo_Air_Ws_3Evalve.ActivationState
                Case BsScadaControl.States._OFF
                    Select Case Me.InDo_AirWs_Pw_3Evalve.ActivationState
                        Case BsScadaControl.States._OFF
                            MyClass.InterNalDosingSource = INTERNAL_DOSING_SOURCES.DISTILLED_WATER

                        Case BsScadaControl.States._ON
                            MyClass.InterNalDosingSource = INTERNAL_DOSING_SOURCES.WASHING_SOLUTION

                    End Select

                Case BsScadaControl.States._ON
                    Select Case Me.InDo_AirWs_Pw_3Evalve.ActivationState
                        Case BsScadaControl.States._OFF
                            MyClass.InterNalDosingSource = INTERNAL_DOSING_SOURCES.DISTILLED_WATER

                        Case BsScadaControl.States._ON
                            MyClass.InterNalDosingSource = INTERNAL_DOSING_SOURCES.AIR

                    End Select
            End Select


            MyClass.InDo_UpdateSyringeColors()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Indo_UpdateSource ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Indo_UpdateSource ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "External Washing Methods"

#End Region

#Region "Washing Station (aspiration) Methods"

    ''' <summary>
    ''' Before switching ON a Pump in the Washing Station Aspiration Tab
    ''' it is preliminary to check that the Tanks are prepared 
    ''' 
    ''' </summary>
    ''' <param name="pItem"></param>
    ''' <remarks>Created by SGM 11/05/2011</remarks>
    Private Function WSAsp_CanActivatePump(ByVal pItem As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal.SetDatos = True

            Select Case pItem.ToString

                Case FLUIDICS_ELEMENTS.SF1_B10.ToString
                    myGlobal.SetDatos = (MyClass.CurrentHighContaminationTankLevel < MyClass.MaxHighContaminationTankLevel)
                    If Not CBool(myGlobal.SetDatos) Then
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_HCTANK_FULL.ToString)
                        myGlobal.SetDatos = False
                        myGlobal.HasError = True
                    End If

                Case FLUIDICS_ELEMENTS.SF1_B6.ToString, _
                    FLUIDICS_ELEMENTS.SF1_B7.ToString, _
                    FLUIDICS_ELEMENTS.SF1_B8.ToString, _
                    FLUIDICS_ELEMENTS.SF1_B9.ToString

                    myGlobal.SetDatos = MyClass.IsNotLowContaminationTankEmptying And _
                                        (MyClass.CurrentLowContaminationTankLevel <> HW_TANK_LEVEL_SENSOR_STATES.FULL)

                    If Not CBool(myGlobal.SetDatos) Then
                        MyBase.DisplayMessage(Messages.SRV_LCTANK_FULL.ToString)
                        myGlobal.SetDatos = False
                        myGlobal.HasError = True
                    End If

            End Select

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSAsp_CanActivatePump ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSAsp_CanActivatePump", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try
        Return myGlobal
    End Function

#End Region

#Region "Washing Station (dispensation) Methods"

    Private Sub WsDisp_UpdatePlungersState(ByVal pOn As Boolean)
        Try
            WsDisp_Plunger1Panel_On.Visible = pOn
            WsDisp_Plunger1Panel_Off.Visible = Not pOn

            WsDisp_Plunger2Panel_On.Visible = pOn
            WsDisp_Plunger2Panel_Off.Visible = Not pOn

            WsDisp_Plunger3Panel_On.Visible = pOn
            WsDisp_Plunger3Panel_Off.Visible = Not pOn

            WsDisp_Plunger4Panel_On.Visible = pOn
            WsDisp_Plunger4Panel_Off.Visible = Not pOn

            WsDisp_Plunger5Panel_On.Visible = pOn
            WsDisp_Plunger5Panel_Off.Visible = Not pOn

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WsDisp_UpdatePlungersState ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WsDisp_UpdatePlungersState ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "In/Out Methods"

    ''' <summary>
    ''' Before switching ON/OFF a Pump in the In/Out Tab
    ''' it is preliminary to check that the Tanks are prepared 
    ''' 
    ''' </summary>
    ''' <param name="pItem"></param>
    ''' <remarks>Created by SGM 11/05/2011</remarks>
    Private Function InOut_CanActivateElement(ByVal pItem As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            myGlobal.SetDatos = True

            Select Case pItem.ToString

                Case FLUIDICS_ELEMENTS.SF1_B4.ToString, FLUIDICS_ELEMENTS.SF1_EV2.ToString, FLUIDICS_ELEMENTS.SF1_EV1.ToString
                    myGlobal.SetDatos = (MyClass.CurrentDistilledWaterTankLevel < MyClass.MaxDistilledWaterTankLevel)
                    If Not CBool(myGlobal.SetDatos) Then
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_PWTANK_FULL.ToString)
                        MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.NOK)
                    End If

                Case FLUIDICS_ELEMENTS.SF1_B5.ToString
                    myGlobal.SetDatos = (MyClass.CurrentLowContaminationTankLevel <> HW_TANK_LEVEL_SENSOR_STATES.EMPTY)
                    If Not CBool(myGlobal.SetDatos) Then
                        myGlobal = MyBase.DisplayMessage(Messages.SRV_HCTANK_EMPTY.ToString)
                        MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.NOK)
                    End If
            End Select


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InOut_CanActivateElement ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InOut_CanActivateElement", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try

        Return myGlobal

    End Function


#End Region

#Region "Collision Detection Methods"

    ''' <summary>
    ''' Enables Needle Collision test
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created SGM 04/10/2012</remarks>
    Private Function Col_EnableTest() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Me.Col_StartStopButton.Enabled = False
            myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

            If MyBase.SimulationMode Then
                System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                myGlobal = MyBase.DisplayMessage(Messages.SRV_COLLISION_TEST_READY.ToString)

            Else

                If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING

                    Dim myUtilCommand As New UTILCommandTO()
                    With myUtilCommand
                        .ActionType = UTILInstructionTypes.NeedleCollisionTest
                        .CollisionTestActionType = UTILCollisionTestActions.Enable
                        .SerialNumberToSave = "0"
                        .TanksActionType = UTILIntermediateTanksTestActions.NothingToDo
                    End With

                    myGlobal = MyBase.myServiceMDI.SEND_INFO_STOP

                    If Not myGlobal.HasError Then
                        myGlobal = myScreenDelegate.SendUTIL(myUtilCommand)
                    End If

                End If

                If myGlobal.HasError Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Col_EnableTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Col_EnableTest", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try

        Return myGlobal

    End Function

    Private Function Col_DisableTest() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            Me.Col_StartStopButton.Enabled = False
            myGlobal = MyBase.DisplayMessage(Messages.SRV_DISABLING_FW_EVENTS.ToString)

            If MyBase.SimulationMode Then
                System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                myGlobal = MyBase.DisplayMessage(Messages.SRV_TEST_COMPLETED.ToString)
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            Else

                If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING

                    Dim myUtilCommand As New UTILCommandTO()
                    With myUtilCommand
                        .ActionType = UTILInstructionTypes.NeedleCollisionTest
                        .CollisionTestActionType = UTILCollisionTestActions.Disable
                        .SerialNumberToSave = "0"
                        .TanksActionType = UTILIntermediateTanksTestActions.NothingToDo
                    End With

                    myGlobal = myScreenDelegate.SendUTIL(myUtilCommand)

                    If Not myGlobal.HasError Then
                        myGlobal = MyBase.myServiceMDI.SEND_INFO_START
                    End If

                End If

                If myGlobal.HasError Then
                    MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                End If

            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Col_DisableTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Col_DisableTest", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try

        Return myGlobal

    End Function

    Private Sub Col_SimulateCollision()
        Try


            If Now.Second > 0 And Now.Second <= 15 Then
                Me.Col_SamplesLED.CurrentStatus = ConvertFwStringToCollisionStatus("1")
                Me.Col_Reagent1LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent2LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_WashingStationLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
            ElseIf Now.Second > 15 And Now.Second <= 30 Then
                Me.Col_SamplesLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent1LED.CurrentStatus = ConvertFwStringToCollisionStatus("1")
                Me.Col_Reagent2LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_WashingStationLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
            ElseIf Now.Second > 30 And Now.Second <= 45 Then
                Me.Col_SamplesLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent1LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent2LED.CurrentStatus = ConvertFwStringToCollisionStatus("1")
                Me.Col_WashingStationLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
            ElseIf Now.Second > 45 And Now.Second <= 60 Then
                Me.Col_SamplesLED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent1LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_Reagent2LED.CurrentStatus = ConvertFwStringToCollisionStatus("0")
                Me.Col_WashingStationLED.CurrentStatus = ConvertFwStringToCollisionStatus("1")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Col_SimulateCollision ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Col_SimulateCollision", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, Nothing, Me)
        End Try
    End Sub



#End Region

#Region "Encoder Test Methods"

#End Region

#End Region

#Region "History Methods"

    ''' <summary>
    ''' Updates the screen delegate's properties used for History management
    ''' </summary>
    ''' <param name="pResult"></param>
    ''' <remarks>
    ''' Created by SGM 02/08/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ReportHistory(ByVal pResult As MotorsPumpsValvesTestDelegate.HISTORY_RESULTS, _
                              Optional ByVal pReadyToReport As Boolean = False)
        Try

            'PDT
            'If MyBase.SimulationMode Then Exit Sub 'PONER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            With MyClass.myScreenDelegate

                .HistoryTask = PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES

                Select Case .HistoryArea

                    'COLLISION
                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.COLLISION

                        If Not pReadyToReport Then

                            If Me.Col_WashingStationLED.CurrentStatus = BSMonitorControlBase.Status._OFF Then
                                .HisCollisions(MotorsPumpsValvesTestDelegate.HISTORY_COLLISIONS.WASHING_STATION) = True
                            ElseIf Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status._OFF Then
                                .HisCollisions(MotorsPumpsValvesTestDelegate.HISTORY_COLLISIONS.SAMPLE) = True
                            ElseIf Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status._OFF Then
                                .HisCollisions(MotorsPumpsValvesTestDelegate.HISTORY_COLLISIONS.REAGENT1) = True
                            ElseIf Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status._OFF Then
                                .HisCollisions(MotorsPumpsValvesTestDelegate.HISTORY_COLLISIONS.REAGENT2) = True
                            End If

                            MyClass.IsPendingToReportHistory = True

                        Else
                            MyClass.myScreenDelegate.ManageHistoryResults()
                            MyClass.ClearHistoryData()
                            MyClass.IsPendingToReportHistory = False
                        End If

                        'ENCODER
                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.ENCODER

                        MyClass.ClearHistoryData()
                        .HisEncoderLostSteps = Math.Abs(Me.Enco_ReactionsRotorMotor.CurrentPosition - Me.Enco_ReactionsRotorEncoderPos)
                        .HisTestActionResult = pResult

                        MyClass.myScreenDelegate.ManageHistoryResults()



                    Case Else

                        If Not MyClass.SelectedElement Is Nothing Then

                            Dim myElement As MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.NONE

                            If Not pReadyToReport Then

                                'MOTORS, PUMPS, VALVES
                                Select Case MyClass.SelectedElementGroup
                                    Case ItemGroups.MANIFOLD
                                        '.HisSubsystem = MotorsPumpsValvesTestDelegate.HISTORY_SUBSYSTEMS.MANIFOLD
                                        'Select Case MyClass.SelectedElement.Identity.ToUpper.Trim
                                        Select Case MyClass.SelectedElement.Identity.Trim
                                            Case MANIFOLD_ELEMENTS.JE1_B1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_B1
                                            Case MANIFOLD_ELEMENTS.JE1_B2.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_B2
                                            Case MANIFOLD_ELEMENTS.JE1_B3.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_B3
                                            Case MANIFOLD_ELEMENTS.JE1_EV1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_EV1
                                            Case MANIFOLD_ELEMENTS.JE1_EV2.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_EV2
                                            Case MANIFOLD_ELEMENTS.JE1_EV3.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_EV3
                                            Case MANIFOLD_ELEMENTS.JE1_EV4.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_EV4
                                            Case MANIFOLD_ELEMENTS.JE1_EV5.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_EV5
                                            Case MANIFOLD_ELEMENTS.JE1_MS.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_MSA
                                            Case MANIFOLD_ELEMENTS.JE1_MR1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_MR1A
                                            Case MANIFOLD_ELEMENTS.JE1_MR2.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.JE1_MR2A

                                        End Select

                                    Case ItemGroups.FLUIDICS
                                        '.HisSubsystem = MotorsPumpsValvesTestDelegate.HISTORY_SUBSYSTEMS.FLUIDICS
                                        'Select Case MyClass.SelectedElement.Identity.ToUpper.Trim
                                        Select Case MyClass.SelectedElement.Identity.Trim
                                            Case FLUIDICS_ELEMENTS.SF1_B1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B1
                                            Case FLUIDICS_ELEMENTS.SF1_B2.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B2
                                            Case FLUIDICS_ELEMENTS.SF1_B3.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B3
                                            Case FLUIDICS_ELEMENTS.SF1_B4.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B4
                                            Case FLUIDICS_ELEMENTS.SF1_B5.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B5
                                            Case FLUIDICS_ELEMENTS.SF1_B6.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B6
                                            Case FLUIDICS_ELEMENTS.SF1_B7.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B7
                                            Case FLUIDICS_ELEMENTS.SF1_B8.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B8
                                            Case FLUIDICS_ELEMENTS.SF1_B9.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B9
                                            Case FLUIDICS_ELEMENTS.SF1_B10.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_B10
                                            Case FLUIDICS_ELEMENTS.SF1_EV1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_EV1
                                            Case FLUIDICS_ELEMENTS.SF1_EV2.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_EV2
                                            Case FLUIDICS_ELEMENTS.SF1_GE1.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_GE1
                                            Case FLUIDICS_ELEMENTS.SF1_MSA.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.SF1_MSA

                                        End Select

                                    Case ItemGroups.PHOTOMETRICS
                                        '.HisSubsystem = MotorsPumpsValvesTestDelegate.HISTORY_SUBSYSTEMS.PHOTOMETRICS
                                        'Select Case MyClass.SelectedElement.Identity.ToUpper.Trim
                                        Select Case MyClass.SelectedElement.Identity.Trim
                                            Case PHOTOMETRICS_ELEMENTS.GLF_MRA.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.GLF_MRA
                                            Case PHOTOMETRICS_ELEMENTS.GLF_MRE.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.GLF_MRE
                                            Case PHOTOMETRICS_ELEMENTS.GLF_MWA.ToString : myElement = MotorsPumpsValvesTestDelegate.HISTORY_ELEMENTS.GLF_MWA
                                        End Select

                                End Select

                                'test action
                                Select Case .ActivationMode
                                    Case SWITCH_ACTIONS.SET_TO_ON
                                        Select Case MyClass.CurrentActivationMode
                                            Case ACTIVATION_MODES.SIMPLE : .HisIndoSimpleSwitches(myElement) = pResult
                                            Case ACTIVATION_MODES.CONTINUOUS : .HisIndoContinuousSwitches(myElement) = pResult
                                        End Select


                                    Case SWITCH_ACTIONS.SET_TO_OFF
                                        Select Case MyClass.CurrentActivationMode
                                            Case ACTIVATION_MODES.SIMPLE : .HisIndoSimpleSwitches(myElement) = pResult
                                            Case ACTIVATION_MODES.CONTINUOUS : .HisIndoContinuousSwitches(myElement) = pResult
                                        End Select

                                End Select

                                'value
                                Dim myElementValue As Integer = -1
                                If TypeOf (MyClass.SelectedElement) Is BsScadaValveControl Then
                                    Dim myValve As BsScadaValveControl = CType(MyClass.SelectedElement, BsScadaValveControl)
                                    If myValve IsNot Nothing Then
                                        Select Case MyClass.RequestedNewState
                                            Case BsScadaControl.States._OFF : myElementValue = 0
                                            Case BsScadaControl.States._ON : myElementValue = 1
                                        End Select
                                        .HisElementType = MotorsPumpsValvesTestDelegate.HISTORY_ITEM_TYPES.EVALVE
                                    End If

                                ElseIf TypeOf (MyClass.SelectedElement) Is BsScadaValve3Control Then
                                    Dim myValve3 As BsScadaValve3Control = CType(MyClass.SelectedElement, BsScadaValve3Control)
                                    If myValve3 IsNot Nothing Then
                                        Select Case MyClass.RequestedNewState
                                            Case BsScadaControl.States._OFF : myElementValue = 0
                                            Case BsScadaControl.States._ON : myElementValue = 1
                                        End Select
                                        .HisElementType = MotorsPumpsValvesTestDelegate.HISTORY_ITEM_TYPES.EVALVE3
                                    End If

                                ElseIf TypeOf (MyClass.SelectedElement) Is BsScadaPumpControl Then
                                    Dim myPump As BsScadaPumpControl = CType(MyClass.SelectedElement, BsScadaPumpControl)
                                    If myPump IsNot Nothing Then
                                        Select Case MyClass.RequestedNewState
                                            Case BsScadaControl.States._OFF : myElementValue = 0
                                            Case BsScadaControl.States._ON : myElementValue = 1
                                        End Select
                                        .HisElementType = MotorsPumpsValvesTestDelegate.HISTORY_ITEM_TYPES.PUMP
                                    End If

                                ElseIf TypeOf (MyClass.SelectedElement) Is BsScadaMotorControl Then
                                    Dim myMotor As BsScadaMotorControl = CType(MyClass.SelectedElement, BsScadaMotorControl)
                                    If myMotor IsNot Nothing Then
                                        myElementValue = myMotor.CurrentPosition
                                        .HisIndoMovements(myElement) = pResult
                                        .HisElementType = MotorsPumpsValvesTestDelegate.HISTORY_ITEM_TYPES.DCMOTOR
                                    End If
                                End If




                                .HistoryArea = MyClass.CurrentHistoryArea

                                'aditional info
                                Select Case .HistoryArea
                                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.INTERNAL_DOSING
                                        Select Case MyClass.InterNalDosingSource
                                            Case INTERNAL_DOSING_SOURCES.WASHING_SOLUTION : .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.DOSING_WS
                                            Case INTERNAL_DOSING_SOURCES.DISTILLED_WATER : .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.DOSING_DW
                                            Case INTERNAL_DOSING_SOURCES.AIR : .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.DOSING_AIR
                                        End Select

                                        If myElementValue >= 0 Then
                                            .HisIndoValues(myElement) = myElementValue
                                        End If

                                        'If .HisElementType = MotorsPumpsValvesTestDelegate.HISTORY_ITEM_TYPES.EVALVE3 Then .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.EXT_WASHING
                                        If myElementValue >= 0 Then
                                            .HisExWaValues(myElement) = myElementValue
                                        End If
                                        .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.WS_ASPIRATION
                                        If myElementValue >= 0 Then
                                            .HisWsAspValues(myElement) = myElementValue
                                        End If
                                        .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.WS_DISPENSATION
                                        If myElementValue >= 0 Then
                                            .HisWsDispValues(myElement) = myElementValue
                                        End If
                                        .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                                    Case MotorsPumpsValvesTestDelegate.HISTORY_AREAS.IN_OUT
                                        If myElementValue >= 0 Then
                                            .HisInOutValues(myElement) = myElementValue
                                        End If
                                        .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                                End Select

                                MyClass.IsPendingToReportHistory = True

                            Else
                                If .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.NONE Then .HistoryArea = MyClass.CurrentHistoryArea

                                MyClass.myScreenDelegate.ManageHistoryResults()

                                MyClass.ClearHistoryData()

                                MyClass.IsPendingToReportHistory = False

                            End If


                        End If

                End Select





            End With



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' Report Task not performed successfully to History
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ReportHistoryError()
        Try


            MyClass.ReportHistory(MotorsPumpsValvesTestDelegate.HISTORY_RESULTS._ERROR)


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistoryError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistoryError ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clears all the History Data
    ''' </summary>
    ''' <remarks>Created by SGM 02/08/2011</remarks>
    Private Sub ClearHistoryData()
        Try
            With MyClass.myScreenDelegate

                .InitializeHistoryElements()
                .HisTestActionResult = MotorsPumpsValvesTestDelegate.HISTORY_RESULTS.NOT_DONE
                .HisAditionalInfo = MotorsPumpsValvesTestDelegate.HISTORY_ADITIONAL_INFO.NONE

                .HisEncoderLostSteps = -1

            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReportHistory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ReportHistory ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "private Event Handlers"

#Region "General"

    Private Sub IMotorsPumpsValvesTest_CursorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CursorChanged
        Try
            If MyClass.IsContinuousSwitching And Me.Cursor <> Cursors.WaitCursor Then
                Me.Cursor = Cursors.WaitCursor
            End If

            If MyClass.CurrentTestPanel IsNot Nothing Then
                MyClass.CurrentTestPanel.Cursor = Me.Cursor
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".IMotorsPumpsValvesTest_CursorChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IMotorsPumpsValvesTest_CursorChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IMotorsPumpsValvesTest_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
                'ElseIf e.CloseReason = CloseReason.UserClosing Then
                '    MyClass.myScreenDelegate.Dispose()
                '    MyClass.myScreenDelegate = Nothing
                '    e.Cancel = True
            Else
                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing

                'SGM 28/02/2012
                MyBase.ActivateMDIMenusButtons(Not MyBase.CloseRequestedByMDI)
                MyBase.myServiceMDI.IsFinalClosing = MyBase.CloseRequestedByMDI

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub MotorsTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If DesignMode Then
            'Me.BsTabPagesControl.Controls.Add(Me.BsInternalDosingTabPage)
            'Me.BsTabPagesControl.Controls.Add(Me.BsExternalWashingTabPage)
            'Me.BsTabPagesControl.Controls.Add(Me.BsAspirationTabPage)
            'Me.BsTabPagesControl.Controls.Add(Me.BsDispensationTabPage)
            'Me.BsTabPagesControl.Controls.Add(Me.BsInOutTabPage)
            'Me.BsTabPagesControl.Controls.Add(Me.BsCollisionDetectionTabPage)
        End If

        TestableElements = New List(Of BsScadaControl)

        Dim myGlobal As New GlobalDataTO
        Try

            'Get the current user level
            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels(LanguageID)

            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New MotorsPumpsValvesTestDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            ' PDT !!! Temporal !!! se debería controlar según el acceso de usuario !!!
            'If GlobalConstants.REAL_DEVELOPMENT_MODE = 1 Then
            '    Me.SimulationMode = True
            'Else
            '    Me.SimulationMode = False
            'End If

            ' MyBase.SimulationMode = False 'QUITAR


            myScreenDelegate.InitializeHistoryElements()
            myScreenDelegate.GetParameters(MyBase.AnalyzerModel)

            If MyClass.CurrentMotorAdjustControl IsNot Nothing Then
                MyClass.CurrentMotorAdjustControl.ClearText()
            End If

            MyClass.NewSelectedTab = Me.bsInternalDosingTabPage
            MyClass.ManageTabChanged()

            'disable all
            MyClass.DisableCurrentPage()
            MyClass.ManageTabPages = False

            Me.PrepareButtons()

            'SGM 12/11/2012 - Information
            MyClass.SelectedInfoPanel = Me.BsInternalDosingInfoPanel
            MyClass.SelectedAdjPanel = Me.BsInternalDosingTestPanel
            Me.BsInfoInDoXPSViewer.FitToPageHeight()
            'Me.BsInfoExWaXPSViewer.FitToPageHeight()
            'Me.BsInfoWsAspXPSViewer.FitToPageHeight()
            'Me.BsInfoWsDispXPSViewer.FitToPageHeight()
            'Me.BsInfoInOutXPSViewer.FitToPageHeight()
            'Me.BsInfoColXPSViewer.FitToPageHeight()

            MyBase.DisplayInformation(APPLICATION_PAGES.SCADA_DOSING, Me.BsInfoInDoXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.SCADA_EX_WASH, Me.BsInfoExWaXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.SCADA_WS_ASP, Me.BsInfoWsAspXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.SCADA_WS_DISP, Me.BsInfoWsDispXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.SCADA_INOUT, Me.BsInfoInOutXPSViewer)
            MyBase.DisplayInformation(APPLICATION_PAGES.COLISION, Me.BsInfoColXPSViewer)


            MyClass.InitializeHomes()

            Me.BsExitButton.Enabled = False 'SGM 23/10/2012

            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                ' Reading Adjustments
                If Not MyClass.myServiceMDI.AdjustmentsReaded Then
                    ' Parent Reading Adjustments
                    MyBase.ReadAdjustments()

                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    PrepareArea()

                    ' Manage FwScripts must to be sent at load screen
                    If MyBase.SimulationMode Then
                        ' simulating...
                        'LoadAdjustmentGroupData()


                        'MyBase.DisplaySimulationMessage("Request Adjustments from Instrument...")
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                        PrepareArea()
                    Else
                        If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                            myGlobal = myScreenDelegate.SendREAD_ADJUSTMENTS(GlobalEnumerates.Ax00Adjustsments.ALL)
                        End If
                    End If
                Else
                    MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    PrepareArea()
                End If

                'Initializations

                MyClass.InitializeCommonObjects()
                MyClass.InitializeInternalDosing()
                MyClass.InitializeExternalWashing()
                MyClass.InitializeWSAspiration()
                MyClass.InitializeWSDispensation()
                MyClass.InitializeInOut()
                MyClass.InitializeCollision()

                MyClass.UnSelectMotor()

                'PROTO 5
                Me.BsEncoderTabPage.PageEnabled = False
                'MyClass.InitializeEncoder()
                'PROTO 5

                If Not MyBase.SimulationMode And AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then '#REFACTORING
                    MyClass.PrepareErrorMode()
                    MyBase.DisplayMessage("")
                End If

            End If


            If myGlobal.HasError Then
                PrepareErrorMode()
                MyBase.ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If



            MyBase.MyBase_Load(sender, e)

            ResetBorderSRV()



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        'End If
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 12/11/2012</remarks>
    Private Sub IMotorsPumpsValvesTest_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try

            Me.BsInfoInDoXPSViewer.Visible = True
            Me.BsInfoInDoXPSViewer.RefreshPage()

            Me.BsInfoExWaXPSViewer.Visible = True
            Me.BsInfoExWaXPSViewer.RefreshPage()

            Me.BsInfoWsAspXPSViewer.Visible = True
            Me.BsInfoWsAspXPSViewer.RefreshPage()

            Me.BsInfoWsDispXPSViewer.Visible = True
            Me.BsInfoWsDispXPSViewer.RefreshPage()

            Me.BsInfoInOutXPSViewer.Visible = True
            Me.BsInfoInOutXPSViewer.RefreshPage()

            Me.BsInfoColXPSViewer.Visible = True
            Me.BsInfoColXPSViewer.RefreshPage()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub MotorsTest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                If MyClass.CurrentMotorAdjustControl Is Nothing OrElse MyClass.CurrentMotorAdjustControl.EditionMode <> True Then
                    BsExitButton.PerformClick()
                Else
                    MyClass.CurrentMotorAdjustControl.EscapeRequest()
                End If

            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 07/06/2011</remarks>
    Private Sub BsTabPagesControl_DeSelecting(ByVal sender As Object, ByVal e As DevExpress.XtraTab.TabPageCancelEventArgs) Handles BsTabPagesControl.Deselecting
        Dim myGlobal As New GlobalDataTO
        Try


            ' XBC 10/11/2011
            If Me.SelectedTabInvoked Then
                Me.SelectedTabInvoked = False
                Exit Sub
            End If
            ' XBC 10/11/2011

            If Not Me.ManageTabPages Then
                e.Cancel = True
                Exit Sub
            End If

            If MyClass.CurrentActionStep <> ActionSteps._NONE Then
                e.Cancel = True
                Exit Sub
            End If

            'The user has to perform a Set to Defaault operation
            If Not MyClass.DefaultStatesDone Then
                MyBase.ShowMessage("", Messages.SRV_MEBV_CHANGE_TAB.ToString)
                e.Cancel = True
                Exit Sub
            End If

            MyClass.DeactivateAll()


            If MyClass.IsLoaded And MyClass.IsFirstReadingDone Then
                'If Not IsTabChangeRequested Then
                MyClass.IsTabChangeRequested = True
                'MyClass.IsSettingDefaultStates = True



                If MyBase.SimulationMode Then
                    MyClass.DisableCurrentPage()
                    MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                    MyBase.DisplayMessage(Messages.SRV_ALL_ITEMS_TO_DEFAULT.ToString)
                    MyClass.PrepareArea()

                    Me.Cursor = Cursors.WaitCursor
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)

                    Me.ManageTabPages = True

                End If

                'e.Cancel = True
                'Exit Sub ' XBC 10/11/2011 - Why ?
                'End If

            Else
                e.Cancel = True
                Exit Sub
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_TrySelecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_TrySelecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 07/06/2011</remarks>
    Private Sub BsTabPagesControl_Selected(ByVal sender As Object, ByVal e As DevExpress.XtraTab.TabPageEventArgs) Handles BsTabPagesControl.Selected
        Try


            If e.Page Is bsInternalDosingTabPage Then
                MyClass.SelectedInfoPanel = Me.BsInternalDosingInfoPanel
                MyClass.SelectedAdjPanel = Me.BsInternalDosingTestPanel
                MyClass.BsInfoInDoXPSViewer.RefreshPage()

                MyClass.WashingStationToUp()

            ElseIf e.Page Is bsExternalWashingTabPage Then
                MyClass.SelectedInfoPanel = Me.BsExternalWashingInfoPanel
                MyClass.SelectedAdjPanel = Me.BsExternalWashingTestPanel
                MyClass.BsInfoExWaXPSViewer.RefreshPage()

                MyClass.WashingStationToUp()

            ElseIf e.Page Is bsAspirationTabPage Then
                MyClass.SelectedInfoPanel = Me.BsWSAspirationInfoPanel
                MyClass.SelectedAdjPanel = Me.BsWSAspirationTestPanel
                MyClass.BsInfoWsAspXPSViewer.RefreshPage()

                MyClass.WashingStationToDown()

            ElseIf e.Page Is bsDispensationTabPage Then
                MyClass.SelectedInfoPanel = Me.BsWSDispensationInfoPanel
                MyClass.SelectedAdjPanel = Me.BsWSDispensationTestPanel
                MyClass.BsInfoWsDispXPSViewer.RefreshPage()

                MyClass.WashingStationToDown()

            ElseIf e.Page Is bsInOutTabPage Then
                MyClass.SelectedInfoPanel = Me.BsInOutInfoPanel
                MyClass.SelectedAdjPanel = Me.BsInOutTestPanel
                MyClass.BsInfoInOutXPSViewer.RefreshPage()

                MyClass.WashingStationToUp()

            ElseIf e.Page Is bsCollisionTabPage Then
                MyClass.SelectedInfoPanel = Me.BsCollisionInfoPanel
                MyClass.SelectedAdjPanel = Me.BsCollisionTestPanel
                MyClass.BsInfoColXPSViewer.RefreshPage()

                MyClass.WashingStationToUp()


            ElseIf e.Page Is BsEncoderTabPage Then 'NOT V1
                'MyClass.WashingStationToUp()

            End If


            If MyClass.CurrentActionStep = ActionSteps._NONE Then
                MyClass.EnableCurrentPage()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabPagesControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabPagesControl_Selected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub BsTabPagesControl_Selecting(ByVal sender As Object, ByVal e As DevExpress.XtraTab.TabPageCancelEventArgs) Handles BsTabPagesControl.Selecting
        Dim myGlobal As New GlobalDataTO
        Try



            If Not Me.ManageTabPages Then
                ' XBC 10/11/2011
                If IsTabChangeRequested Then
                    Me.NewSelectedTabIndex = e.PageIndex
                End If
                ' XBC 10/11/2011

                e.Cancel = True
                Exit Sub
            End If

            If MyClass.IsLoaded And MyClass.IsFirstReadingDone Then

                MyClass.NewSelectedTab = e.Page

                If MyBase.SimulationMode Then
                    If IsTabChangeRequested Then
                        MyClass.NewSelectedTab = e.Page

                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                        MyClass.PrepareArea()

                        MyClass.IsTabChangeRequested = False

                        MyClass.ManageTabChanged()

                        'MyClass.IsSettingDefaultStates = False

                        MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                        MyClass.PrepareArea()



                    End If
                Else
                    If IsTabChangeRequested Then

                        MyClass.IsTabChangeRequested = False

                        MyClass.ManageTabChanged()



                    End If
                End If

                If MyClass.CurrentHistoryArea <> MotorsPumpsValvesTestDelegate.HISTORY_AREAS.NONE And MyClass.IsPendingToReportHistory Then
                    MyClass.ReportHistory(Nothing, True)
                End If

                'history area/page
                With MyClass.myScreenDelegate
                    .InitializeHistoryElements()
                    Select Case MyClass.SelectedPage
                        Case TEST_PAGES.INTERNAL_DOSING : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.INTERNAL_DOSING
                        Case TEST_PAGES.EXTERNAL_WASHING : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.EXT_WASHING
                        Case TEST_PAGES.WS_ASPIRATION : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.WS_ASPIRATION
                        Case TEST_PAGES.WS_DISPENSATION : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.WS_DISPENSATION
                        Case TEST_PAGES.IN_OUT : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.IN_OUT
                        Case TEST_PAGES.COLLISION_DETECTION : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.COLLISION
                        Case TEST_PAGES.ENCODER_TEST : .HistoryArea = MotorsPumpsValvesTestDelegate.HISTORY_AREAS.ENCODER
                    End Select

                    MyClass.CurrentHistoryArea = .HistoryArea

                End With

            Else
                e.Cancel = True
                Exit Sub
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsTabControl_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsTabControl_Selecting ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SwitchGroupBox_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InDo_SwitchGroupBox.EnabledChanged, _
                                                                                                                   ExWa_SwitchGroupBox.EnabledChanged, _
                                                                                                                   WSAsp_SwitchGroupBox.EnabledChanged, _
                                                                                                                   WsDisp_SwitchGroupBox.EnabledChanged, _
                                                                                                                   InOut_SwitchGroupBox.EnabledChanged
        Try
            Dim myGrp As BSGroupBox = CType(sender, BSGroupBox)
            If myGrp IsNot Nothing Then
                For Each C As Control In myGrp.Controls
                    If Not TypeOf C Is BSButton Then
                        C.Enabled = myGrp.Enabled
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SwitchGroupBox_EnabledChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SwitchGroupBox_EnabledChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SwitchSimpleRButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InDo_SwitchSimpleRButton.CheckedChanged, _
                                                                                                                   ExWa_SwitchSimpleRButton.CheckedChanged, _
                                                                                                                   WsAsp_SwitchSimpleRButton.CheckedChanged, _
                                                                                                                   WsDisp_SwitchSimpleRButton.CheckedChanged, _
                                                                                                                   InOut_SwitchSimpleRButton.CheckedChanged
        Try
            Dim myRButton As BSRadioButton = CType(sender, BSRadioButton)
            If myRButton IsNot Nothing Then
                If myRButton.Checked Then
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SwitchSimpleRButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SwitchSimpleRButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SwitchContinuousRButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InDo_SwitchContinuousRButton.CheckedChanged, _
                                                                                                                    ExWa_SwitchContinuousRButton.CheckedChanged, _
                                                                                                                    WsAsp_SwitchContinuousRButton.CheckedChanged, _
                                                                                                                    WsDisp_SwitchContinuousRButton.CheckedChanged, _
                                                                                                                    InOut_SwitchContinuousRButton.CheckedChanged
        Try
            Dim myRButton As BSRadioButton = CType(sender, BSRadioButton)
            If myRButton IsNot Nothing Then
                If myRButton.Checked Then
                    MyClass.CurrentActivationMode = ACTIVATION_MODES.CONTINUOUS
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SwitchContinuousRButton_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SwitchContinuousRButton_CheckedChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Stop Continuous button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 13/06/2011</remarks>
    Private Sub StopContinuousButtons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InDo_StopButton.Click, _
                                                                                                                ExWa_StopButton.Click, _
                                                                                                                WSAsp_StopButton.Click, _
                                                                                                                WsDisp_StopButton.Click, _
                                                                                                                InOut_StopButton.Click
        Try

            MyClass.StopContinuousSwitchingRequested = True
            MyClass.IsContinuousSwitching = False
            'MyClass.CurrentActivationMode = ACTIVATION_MODES.SIMPLE

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopContinuousButtons_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopContinuousButtons_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Dim myGlobal As New GlobalDataTO
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Dim waitForScripts As Boolean = False
        Try
            If MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE Then
                MyBase.ShowMessage("", Messages.SRV_MEBV_ERROR_WARN.ToString)
                Me.Close()
                Exit Sub
            End If

            dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SRV_TEST_PENDING.ToString), Messages.SRV_TEST_PENDING.ToString)

            If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                MyClass.IsScreenCloseRequested = True
                MyBase.DisplayMessage(Messages.SRV_TEST_EXIT_REQUESTED.ToString)
                'System.Threading.Thread.Sleep(2000) 'quitar



                If Not MyClass.DefaultStatesDone Then
                    MyClass.AllItemsToDefault() 'set to default state all the previous tab elements
                ElseIf MyClass.myScreenDelegate.IsWashingStationDown Then
                    MyClass.WashingStationToUp()
                Else
                    MyClass.AllArmsToParking()
                End If

                'history
                If MyClass.CurrentHistoryArea <> MotorsPumpsValvesTestDelegate.HISTORY_AREAS.NONE And MyClass.IsPendingToReportHistory Then
                    MyClass.ReportHistory(Nothing, True)
                End If

                ' XBC 09/11/2011
                ''If MyBase.SimulationMode Then
                'Me.Close()
                ''End If

                'Exit Sub
                ' XBC 09/11/2011

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsExitButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
            MyClass.IsActionRequested = False
        End Try
    End Sub

    Private Sub BsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCancelButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            'If MyClass.SelectedPage = TEST_PAGES.WS_ASPIRATION Or MyClass.SelectedPage = TEST_PAGES.WS_DISPENSATION Then
            '    MyClass.IsSettingDefaultStates = True
            '    MyClass.WashingStationToUp()
            'Else
            MyClass.AllItemsToDefault()
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsCancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allows to handle the WashingStation Up or Down
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 18/05/2012</remarks>
    Private Sub WS_UpDownButtonn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles WSDisp_UpDownButton.Click, WSAsp_UpDownButton.Click
        Try
            If MyClass.myScreenDelegate.IsWashingStationDown Then

                If Not MyClass.DefaultStatesDone Then
                    MyBase.ShowMessage("", Messages.SRV_MEBV_WS_UP.ToString)
                    'Dim myButton As Button = CType(sender, Button)
                    'If myButton IsNot Nothing Then myButton.Enabled = False
                Else
                    MyClass.WashingStationToUp()
                End If

            Else
                MyClass.WashingStationToDown()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WS_UpDownButtonn_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WS_UpDownButtonn_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region



#Region "Refresh Event Handler"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by SGM 23/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''             IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Dim myGlobal As New GlobalDataTO

        Try



            'SGM 17/04/2012

            'For the moment no POLHW values can be obtained from Firmware
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim mySensorValuesChangedDT As New UIRefreshDS.SensorValueChangedDataTable

                mySensorValuesChangedDT = pRefreshDS.SensorValueChanged

                For Each S As UIRefreshDS.SensorValueChangedRow In mySensorValuesChangedDT.Rows
                    Select Case S.SensorID
                        Case AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE.ToString
                            MyClass.CurrentHighContaminationTankLevel = CInt(S.Value)

                        Case AnalyzerSensors.BOTTLE_WASHSOLUTION.ToString
                            MyClass.CurrentDistilledWaterTankLevel = CInt(S.Value)

                            'SGM 10/10/2012 - Collision detected
                        Case AnalyzerSensors.TESTING_NEEDLE_COLLIDED.ToString
                            Dim sensorValue As Single = 0
                            sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.TESTING_NEEDLE_COLLIDED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.TESTING_NEEDLE_COLLIDED) = 0 'Once updated UI clear sensor

                                'Refresh Needle Collided presentation
                                If MyClass.IsCollisionTestEnabled Then
                                    MyClass.CollidedNeedle = AnalyzerController.Instance.Analyzer.TestingCollidedNeedle
                                End If

                            End If
                            'end SGM 10/10/2012 

                    End Select
                Next


            Else
                myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)

                'if needed manage the event in the Base Form
                MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

                PrepareArea()
            End If

            Exit Sub


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Must Inherited"

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/03/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try

            If pAlarmType = ManagementAlarmTypes.NONE And MyClass.CurrentAlarmType <> ManagementAlarmTypes.NONE Then
                Exit Sub
            End If

            ' Stop Progress bar & Timer
            Me.ProgressBar1.Value = 0
            Me.ProgressBar1.Visible = False
            Me.ProgressBar1.Refresh()
            MyClass.BsSimulateContinuousTimer.Enabled = False

            MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE

            MyClass.IsActionRequested = False
            Me.ManageTabPages = False

            MyClass.CurrentActionStep = ActionSteps._NONE
            MyClass.SelectedElement = Nothing
            MyClass.SwitchRequestedElement = Nothing
            MyClass.MoveRequestedElement = Nothing
            MyClass.MoveRequestedMode = MOVEMENT._NONE

            MyClass.DisableCurrentPage()

            Me.Cursor = Cursors.Default

            If pAlarmType <> ManagementAlarmTypes.NONE Then
                MyClass.IsWorking = False
                Me.BsExitButton.Enabled = True
                If MyClass.CurrentTestPanel IsNot Nothing Then
                    MyClass.CurrentTestPanel.Cursor = Cursors.Default
                    MyClass.CurrentTestPanel.Enabled = False
                End If

                If MyClass.IsContinuousSwitching Then
                    If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                        MyClass.CurrentContinuousStopButton.Cursor = Cursors.Default
                        MyClass.CurrentContinuousStopButton.Enabled = False
                    End If
                    MyClass.IsContinuousSwitching = False
                End If
            End If

            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyClass.PrepareErrorMode(pAlarmType)

            ' Stop FwScripts
            myFwScriptDelegate.StopFwScriptQueue()

            myFwScriptDelegate.SEND_INFO_STOP()

            'Disable Test collision
            If MyClass.SelectedPage = TEST_PAGES.COLLISION_DETECTION Then

                Me.Col_Reagent1LED.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                Me.Col_Reagent2LED.CurrentStatus = BSMonitorControlBase.Status.DISABLED
                Me.Col_SamplesLED.CurrentStatus = BSMonitorControlBase.Status.DISABLED

                Dim myUtilCommand As New UTILCommandTO()
                With myUtilCommand
                    .ActionType = UTILInstructionTypes.NeedleCollisionTest
                    .CollisionTestActionType = UTILCollisionTestActions.Disable
                    .SerialNumberToSave = "0"
                    .TanksActionType = UTILIntermediateTanksTestActions.NothingToDo
                End With
                myScreenDelegate.SendUTIL(myUtilCommand)
            End If

            'when stop action is finished, perform final operations after alarm received
            If pAlarmType <> ManagementAlarmTypes.NONE Then
                MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)
            Else
                If MyClass.IsContinuousSwitching Then
                    If MyClass.CurrentContinuousStopButton IsNot Nothing Then
                        MyClass.CurrentContinuousStopButton.Cursor = Cursors.Default
                        MyClass.CurrentContinuousStopButton.Enabled = False
                    End If
                    MyClass.IsContinuousSwitching = False
                End If
            End If

            myFwScriptDelegate.SEND_INFO_START()
            Me.Cursor = Cursors.Default
            Me.BsExitButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Internal Dosing Event Handlers"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_Motors_AdjustControl_SetABSPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles InDo_Motors_AdjustControl.AbsoluteSetPointReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            MyClass.DisableCurrentPage()
            If MyBase.SimulationMode Then
                MyClass.SimulateABSPositioning(MyClass.SelectedMotor, CInt(Value))
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.ABSOLUTE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)

                'Open loop: assume that he target is acomplished
                MyClass.SimulateABSPositioning(MyClass.SelectedMotor, CInt(Value), True)

            End If



        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_Motors_AdjustControl_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_Motors_AdjustControl_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_Motors_AdjustControl_SetRELPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles InDo_Motors_AdjustControl.RelativeSetPointReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            MyClass.DisableCurrentPage()
            If MyBase.SimulationMode Then
                MyClass.SimulateRELPositioning(MyClass.SelectedMotor, CInt(InDo_Motors_AdjustControl.CurrentValue + Value))
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.RELATIVE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)

                'Open loop: assume that he target is acomplished
                MyClass.SimulateRELPositioning(MyClass.SelectedMotor, CInt(InDo_Motors_AdjustControl.CurrentValue + Value), True)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_Motors_AdjustControl_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_Motors_AdjustControl_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_Motors_AdjustControl_HomeRequestReleased(ByVal sender As System.Object) Handles InDo_Motors_AdjustControl.HomeRequestReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            MyClass.DisableCurrentPage()
            If MyBase.SimulationMode Then
                MyClass.SimulateHOMEPositioning(MyClass.SelectedMotor, 0)
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.HOME
                MyClass.RequestedNewPosition = 0
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)
                'Open loop: assume that he target is acomplished
                MyClass.SimulateHOMEPositioning(MyClass.SelectedMotor, 0, True)
            End If




        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_Motors_AdjustControl_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_Motors_AdjustControl_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_Motors_AdjustControl_SetPointOutOfRange(ByVal sender As Object) Handles InDo_Motors_AdjustControl.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_Motors_AdjustControl_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_Motors_AdjustControl_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InDo_ElementSelected(ByVal sender As Object) Handles InDo_Air_Ws_3Evalve.ControlClicked, _
                                                                    InDo_AirWs_Pw_3Evalve.ControlClicked, _
                                                                    InDo_Samples_EValve.ControlClicked, _
                                                                    InDo_Reagent1_EValve.ControlClicked, _
                                                                    InDo_Reagent2_EValve.ControlClicked, _
                                                                    InDo_Samples_Pump.ControlClicked, _
                                                                    InDo_Reagent1_Pump.ControlClicked, _
                                                                    InDo_Reagent2_Pump.ControlClicked, _
                                                                    InDo_Samples_Motor.ControlClicked, _
                                                                    InDo_Reagent1_Motor.ControlClicked, _
                                                                    InDo_Reagent2_Motor.ControlClicked


        Try
            Dim myElement As BsScadaControl = CType(sender, BsScadaControl)
            If myElement IsNot Nothing Then

                WhenElementCliked(myElement)

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InDo_ElementSelected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InDo_ElementSelected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

#End Region

#Region "External Washing Event Handlers"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub ExWa_ElementSelected(ByVal sender As Object) Handles ExWa_Samples_Pump.ControlClicked, _
                                                                    ExWa_Reagent1_Pump.ControlClicked, _
                                                                    ExWa_Reagent2_Pump.ControlClicked

        Try
            Dim myElement As BsScadaControl = CType(sender, BsScadaControl)
            If myElement IsNot Nothing Then

                WhenElementCliked(myElement)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExWa_ElementSelected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExWa_ElementSelected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub


#End Region

#Region "WS Aspiration Event Handlers"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSAsp_ElementSelected(ByVal sender As Object) Handles WsAsp_B6_Pump.ControlClicked, _
                                                                    WsAsp_B7_Pump.ControlClicked, _
                                                                    WsAsp_B8_Pump.ControlClicked, _
                                                                    WsAsp_B9_Pump.ControlClicked, _
                                                                    WsAsp_B10_Pump.ControlClicked

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myElement As BsScadaControl = CType(sender, BsScadaControl)
            If myElement IsNot Nothing Then

                WhenElementCliked(myElement)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExWa_ElementSelected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ExWa_ElementSelected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub


#End Region

#Region "WS Dispensation Event Handlers"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSDisp_ElementSelected(ByVal sender As Object) Handles WsDisp_M1_Motor.ControlClicked, _
                                                                        WsDisp_GE1_Valve.ControlClicked, _
                                                                        WsDisp_GE2_Valve.ControlClicked, _
                                                                        WsDisp_GE3_Valve.ControlClicked, _
                                                                        WsDisp_GE4_Valve.ControlClicked, _
                                                                        WsDisp_GE5_Valve.ControlClicked

        Try
            Dim myElement As BsScadaControl = CType(sender, BsScadaControl)
            If myElement IsNot Nothing Then

                If TypeOf myElement Is BsScadaValve3Control Then
                    If WSDisp_ValvesGroupDCUnit IsNot Nothing Then
                        WSDisp_ValvesGroupDCUnit.Selected = True
                        WhenElementCliked(WSDisp_ValvesGroupDCUnit)
                    End If
                Else
                    WhenElementCliked(myElement)
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSDisp_ElementSelected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSDisp_ElementSelected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub WSDisp_ValvesGroupDCUnit_EnableChanged(ByVal sender As Object, ByVal e As EventArgs) Handles WSDisp_ValvesGroupDCUnit.EnabledChanged
        Try

            WsDisp_GE1_Valve.Enabled = WSDisp_ValvesGroupDCUnit.Enabled
            WsDisp_GE2_Valve.Enabled = WSDisp_ValvesGroupDCUnit.Enabled
            WsDisp_GE3_Valve.Enabled = WSDisp_ValvesGroupDCUnit.Enabled
            WsDisp_GE4_Valve.Enabled = WSDisp_ValvesGroupDCUnit.Enabled
            WsDisp_GE5_Valve.Enabled = WSDisp_ValvesGroupDCUnit.Enabled

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSDisp_ValvesGroupDCUnit_EnableChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSDisp_ValvesGroupDCUnit_EnableChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSDisp_MotorAdjustControl_SetABSPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles WSDisp_MotorAdjustControl.AbsoluteSetPointReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            MyClass.DisableCurrentPage()
            If MyBase.SimulationMode Then
                MyClass.SimulateABSPositioning(MyClass.SelectedMotor, CInt(Value))
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.ABSOLUTE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)

                'Open loop: assume that he target is acomplished
                MyClass.SimulateABSPositioning(MyClass.SelectedMotor, CInt(Value), True)

            End If


            Me.WSDisp_UpDownButton.Enabled = True


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSDisp_MotorAdjustControl_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSDisp_MotorAdjustControl_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSDisp_MotorAdjustControl_SetRELPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles WSDisp_MotorAdjustControl.RelativeSetPointReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            MyClass.DisableCurrentPage()
            If MyBase.SimulationMode Then
                MyClass.SimulateRELPositioning(MyClass.SelectedMotor, CInt(WSDisp_MotorAdjustControl.CurrentValue + Value))
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.RELATIVE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)

                'Open loop: assume that he target is acomplished
                MyClass.SimulateRELPositioning(MyClass.SelectedMotor, CInt(WSDisp_MotorAdjustControl.CurrentValue + Value), True)

            End If

            Me.WSDisp_UpDownButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ". WSDisp_MotorAdjustControl_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ". WSDisp_MotorAdjustControl_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSDisp_MotorAdjustControl_HomeRequestReleased(ByVal sender As System.Object) Handles WSDisp_MotorAdjustControl.HomeRequestReleased
        Try
            If MyClass.SelectedMotor Is Nothing Then Exit Sub

            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If myAdjustControl Is Nothing Then Exit Sub
            'myAdjustControl.Enabled = False

            If MyBase.SimulationMode Then
                MyClass.SimulateHOMEPositioning(MyClass.SelectedMotor, 0)
                MyClass.EnableCurrentPage()
                'myAdjustControl.Enabled = False
                myAdjustControl.Focus()
            Else
                MyClass.MoveRequestedMode = MOVEMENT.HOME
                MyClass.RequestedNewPosition = 0
                MyClass.SelectedMotor.CurrentPosition = 0
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                RequestMotorMove(MyClass.SelectedMotor)

                'Open loop: assume that he target is acomplished
                MyClass.SimulateHOMEPositioning(MyClass.SelectedMotor, 0, True)


            End If

            Me.WSDisp_UpDownButton.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSDisp_MotorAdjustControl_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSDisp_MotorAdjustControl_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub WSDisp_MotorAdjustControl_SetPointOutOfRange(ByVal sender As Object) Handles WSDisp_MotorAdjustControl.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
            Me.WSDisp_UpDownButton.Enabled = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WSDisp_MotorAdjustControl_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".WSDisp_MotorAdjustControl_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "In/Out Event Handlers"

    'just for solving backcolor changes
    Private Sub InOut_PWTank_Valve_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles InOut_PWTank_Valve.BackColorChanged, _
                                                                                                        InOut_PWSource_Valve.BackColorChanged, _
                                                                                                        InOut_LC_Pump.BackColorChanged, _
                                                                                                        InOut_PW_Pump.BackColorChanged
        Dim myBsScadaControl As BsScadaControl = CType(sender, BsScadaControl)
        If myBsScadaControl IsNot Nothing Then
            If myBsScadaControl.BackColor <> Me.InOut_WallPanel.BackColor Then
                myBsScadaControl.BackColor = Me.InOut_WallPanel.BackColor
            End If
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub InOut_ElementSelected(ByVal sender As Object) Handles InOut_LC_Pump.ControlClicked, _
                                                                    InOut_PW_Pump.ControlClicked, _
                                                                    InOut_PWSource_Valve.ControlClicked, _
                                                                    InOut_PWTank_Valve.ControlClicked

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myElement As BsScadaControl = CType(sender, BsScadaControl)
            If myElement IsNot Nothing Then

                WhenElementCliked(myElement)

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InOut_ElementSelected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InOut_ElementSelected ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub



#End Region

#Region "Collision Event Handlers"



    Private Sub Col_StartStopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Col_StartStopButton.Click
        Dim myGlobal As New GlobalDataTO
        Try

            If MyClass.IsCollisionTestEnabled Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.COLLISION_TEST_DISABLING
                myGlobal = MyClass.Col_DisableTest
            Else
                'SGM 23/11/2011
                MyBase.CurrentMode = ADJUSTMENT_MODES.COLLISION_TEST_ENABLING
                myGlobal = MyClass.Col_EnableTest
                'SGM 23/11/2011
            End If

            If myGlobal.HasError Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                MyClass.PrepareArea()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Col_StopButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Col_StopButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try
    End Sub


    Private Sub Col_LED_StatusChanged(ByVal sender As Object) Handles Col_Reagent1LED.StatusChanged, _
                                                                    Col_Reagent2LED.StatusChanged, _
                                                                    Col_SamplesLED.StatusChanged, _
                                                                    Col_WashingStationLED.StatusChanged
        Try
            If MyClass.IsCollisionTestEnabled Then
                Dim myLED As BSMonitorLED = CType(sender, BSMonitorLED)
                If myLED IsNot Nothing Then
                    Select Case myLED.CurrentStatus
                        Case BSMonitorControlBase.Status._ON
                            MyBase.DisplayMessage(Messages.SRV_COLLISION_DETECTED.ToString)

                        Case BSMonitorControlBase.Status._OFF

                    End Select
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Col_LED_StatusChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Col_LED_StatusChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, Nothing, Me)
        End Try
    End Sub




#End Region

#Region "Encoder Event Handlers"

    Private Sub Enco_EncoderPosLabel_ForeColorChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Enco_EncoderPosLabel.ForeColorChanged
        Try
            Me.Enco_UnitsLabel.ForeColor = Me.Enco_EncoderPosLabel.ForeColor
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Enco_EncoderPosLabel_ForeColorChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Enco_EncoderPosLabel_ForeColorChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub Enco_Motor_AdjustControl_SetABSPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles Enco_MotorAdjustControl.AbsoluteSetPointReleased
        Try
            If MyBase.SimulationMode Then
                If MyClass.SelectedMotor IsNot Nothing Then
                    MyClass.SimulateABSPositioning(MyClass.SelectedMotor, CInt(Value))
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                    If Now.Second Mod 5 = 0 Then
                        Me.Enco_EncoderPosLabel.Text = (Me.Enco_MotorAdjustControl.CurrentValue - 1).ToString.Replace(",", ".")
                    Else
                        Me.Enco_EncoderPosLabel.Text = Me.Enco_MotorAdjustControl.CurrentValue.ToString.Replace(",", ".")
                    End If
                End If
            Else
                MyClass.MoveRequestedMode = MOVEMENT.ABSOLUTE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_ABS_REQUESTED.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Enco_Motor_AdjustControl_SetABSPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Enco_Motor_AdjustControl_SetABSPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub Enco_Motor_AdjustControl_SetRELPointReleased(ByVal sender As System.Object, ByVal Value As System.Single) Handles Enco_MotorAdjustControl.RelativeSetPointReleased
        Try
            If MyBase.SimulationMode Then
                If MyClass.SelectedMotor IsNot Nothing Then
                    MyClass.SimulateRELPositioning(MyClass.SelectedMotor, CInt(Enco_MotorAdjustControl.CurrentValue + Value))
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                    If Now.Second Mod 5 = 0 Then
                        Me.Enco_EncoderPosLabel.Text = (Me.Enco_MotorAdjustControl.CurrentValue + 1).ToString.Replace(",", ".")
                    Else
                        Me.Enco_EncoderPosLabel.Text = Me.Enco_MotorAdjustControl.CurrentValue.ToString.Replace(",", ".")
                    End If
                End If
            Else
                MyClass.MoveRequestedMode = MOVEMENT.RELATIVE
                MyClass.RequestedNewPosition = CInt(Value)
                MyBase.DisplayMessage(Messages.SRV_STEP_POSITIONING.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Enco_Motor_AdjustControl_SetRELPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Enco_Motor_AdjustControl_SetRELPointReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub Enco_Motor_AdjustControl_HomeRequestReleased(ByVal sender As System.Object) Handles Enco_MotorAdjustControl.HomeRequestReleased
        Try
            If MyBase.SimulationMode Then
                If MyClass.SelectedMotor IsNot Nothing Then
                    MyClass.SimulateHOMEPositioning(MyClass.SelectedMotor, 0)
                    System.Threading.Thread.Sleep(MyBase.SimulationProcessTime)
                    Me.Enco_EncoderPosLabel.Text = Me.Enco_MotorAdjustControl.CurrentValue.ToString.Replace(",", ".")
                End If
            Else
                MyClass.MoveRequestedMode = MOVEMENT.HOME
                MyClass.RequestedNewPosition = 0
                MyBase.DisplayMessage(Messages.SRV_HOMES_IN_PROGRESS.ToString)
                MyClass.RequestMotorMove(MyClass.SelectedMotor)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Enco_Motor_AdjustControl_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Enco_Motor_AdjustControl_HomeRequestReleased ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/05/2011</remarks>
    Private Sub Enco_Motor_AdjustControl_SetPointOutOfRange(ByVal sender As Object) Handles Enco_MotorAdjustControl.SetPointOutOfRange
        Try
            MyBase.DisplayMessage(Messages.SRV_OUTOFRANGE.ToString)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Enco_Motors_AdjustControl_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".Enco_Motors_AdjustControl_SetPointOutOfRange ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Simulation Event Handlers"

    Private Sub BsHomingSimulationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsHomingSimulationTimer.Tick
        Try

            BsHomingSimulationTimer.Enabled = False
            Me.Cursor = Cursors.Default

            System.Threading.Thread.Sleep(SimulationProcessTime)

            If MyClass.IsArmsToHoming Then
                MyBase.CurrentMode = ADJUSTMENT_MODES.HOME_FINISHED
                MyClass.PrepareArea()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsHomingSimulationTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsHomingSimulationTimer_Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub BsCollisionSimulationTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsCollisionSimulationTimer.Tick
        Try

            BsCollisionSimulationTimer.Enabled = False

            MyClass.Col_SimulateCollision()

            BsCollisionSimulationTimer.Enabled = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsHomingSimulationTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsHomingSimulationTimer_Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoInDoXPSViewer.Load, _
                                                                                            BsInfoExWaXPSViewer.Load, _
                                                                                            BsInfoWsAspXPSViewer.Load, _
                                                                                            BsInfoWsDispXPSViewer.Load, _
                                                                                            BsInfoInOutXPSViewer.Load, _
                                                                                            BsInfoColXPSViewer.Load
        Try
            Dim myBsXPSViewer As BsXPSViewer = CType(sender, BsXPSViewer)
            If myBsXPSViewer IsNot Nothing Then
                If myBsXPSViewer.IsScrollable Then
                    myBsXPSViewer.FitToPageWidth()
                Else
                    myBsXPSViewer.FitToPageHeight()
                    myBsXPSViewer.FitToPageWidth()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Communication Event Handlers"
    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <param name="pData"></param>
    ''' <remarks>Created by XBC 15/09/2011</remarks>
    Public Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles myScreenDelegate.ReceivedLastFwScriptEvent

        Me.UIThread(Function() ManageReceptionEvent(pResponse, pData))

    End Sub

    ''' <summary>
    ''' manages the response of the Analyzer after sending a FwScript List
    ''' The response can be OK, NG, Timeout or Exception
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>Created by XBC 19/04/11</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'SGM 23/10/2012 - in case of Alarm being treated, not treat the incoming data
            If MyClass.CurrentAlarmType <> ManagementAlarmTypes.NONE Then Return True

            'manage special operations according to the screen characteristics
            Application.DoEvents()

            If MyBase.SimulationMode Then Exit Function

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)
            'MyClass.myFwScriptDelegate.IsWaitingForResponse = False
            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    If pResponse = RESPONSE_TYPES.OK Then

                        MyBase.DisplayMessage(Messages.SRV_ADJUSTMENTS_READED.ToString)

                        If Not myGlobal.HasError Then
                            MyBase.myServiceMDI.AdjustmentsReaded = True
                            MyClass.PrepareArea()
                        Else
                            MyClass.PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.HOME_FINISHED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_WASHING
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_PARKING
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                    'SGM 21/11/2011
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_DOWN
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If
                    'SGM 21/11/2011

                    '    'SGM 21/11/2011
                    'Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_UP
                    '    If pResponse = RESPONSE_TYPES.OK Then
                    '        PrepareArea()
                    '    Else
                    '        PrepareErrorMode()
                    '        Exit Function
                    '    End If
                    '    'SGM 21/11/2011

                Case ADJUSTMENT_MODES.TEST_PREPARED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If


                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                    'SGM 10/10/2012
                Case ADJUSTMENT_MODES.COLLISION_TEST_ENABLED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If


                Case ADJUSTMENT_MODES.ERROR_MODE
                    'MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function

#End Region



End Class