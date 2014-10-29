Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports System.Timers
Imports System.Data
Imports System.ComponentModel 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerEntity

#Region "Declarations"
        'General use Class variables
        Private WithEvents AppLayer As ApplicationLayer ' AG 20/04/2010 - app layer instance
        Private myAnalyzerModel As String = "" 'Use for read several software parameters depending on the model
        Private myApplicationName As String ' XBC 02/03/2011 'Different business code in some cases for Service & User Sw


        'Class variables for implement the waiting time processes until Ax00 becomes ready again
        Private myInstructionsQueue As New List(Of GlobalEnumerates.AnalyzerManagerSwActionList)    ' AG 20/05/2010
        Private myParamsQueue As New List(Of Object)    ' XBC 24/05/2010
        Private useRequestFlag As Boolean = False 'Ag 02/11/2010 - Flag for start using request (True when START instruction finish) / Return to False on send ENDRUN or ABORT
        Private waitingTimer As New Timer() ' AG 07/05/2010 - waiting time (watchdog)
        Private Const WAITING_TIME_OFF As Integer = -1 'Wacthdoc off
        Private Const SYSTEM_TIME_OFFSET As Integer = 20 'Additional time (courtesy)    XB 04/06/2014 - BT #1656
        Private Const WAITING_TIME_DEFAULT As Integer = 12 'SECONDS Default time before ask again (if Ax00 is not ready and do not tell us any time estimation)

        'Class variables to inform the presentation layer about UI refresh after instruction receptions
        'AG 07/10/2011 - Duplicate the refresh variable due:
        ' 1) The general mode is every instruction received raises a ReceptionEvent when his treatment is finish working in the same thread 
        '    (so use variables myUI_RefreshEvent, myUI_RefreshDS and eventDataPendingToTriggerFlag)
        ' 2) But some instructions (ANSPHR) are treated using 2 threads (main thread for chemical reactions and secondary for well base line). Both generates differents event refresh when finish
        '    (so use variables myUI_RefreshEvent, myUI_RefreshDS and eventDataPendingToTriggerFlag for the main thread)
        '    (and use mySecondaryUI_RefreshEvent, mySecondaryUI_RefreshDS and secondaryEventDataPendingToTriggerFlag for the secondary thread) --> REJECTED!!!! In running refreshs are added in 1 DSet and event is triggered 1 time each machine cycle
        Private myUI_RefreshEvent As New List(Of GlobalEnumerates.UI_RefreshEvents)
        Private myUI_RefreshDS As New UIRefreshDS
        Private eventDataPendingToTriggerFlag As Boolean = False 'This flag tell us if exits information in myUI_RefreshDS pending to be informed (True) or not exists information to be informed (False) (event ReceptionEvent)

        'AG 22/05/2014 - #1637 Clear code, comment non used variables that could make read code difficult
        'Private mySecondaryUI_RefreshEvent As New List(Of GlobalEnumerates.UI_RefreshEvents)
        'Private mySecondaryUI_RefreshDS As New UIRefreshDS
        'Private secondaryEventDataPendingToTriggerFlag As Boolean = False 'This flag tell us if exits information in myUI_RefreshDS pending to be informed (True) or not exists information to be informed (False) (event ReceptionEvent)
        'AG 07/10/2011



        Private pauseSendingTestPreparationsFlag As Boolean = False 'This flag becomes to TRUE when some alarm appears and Sw stop sending new test preparations
        'Private pauseSendingISETestFlag As Boolean = False 'AG 21/03/2012 - do not use this flag ... use the alarms that activate it (ise status warn or err)

        'Class variables for the send next preparation process
        Private mySentPreparationsDS As New AnalyzerManagerDS 'AG 18/01/2011 - remembers the lasts preparation & reagent wash sents (in RUNNING)
        Private myNextPreparationToSendDS As New AnalyzerManagerDS 'AG 07/06/2012 - remembers the next preparation & reagent wash to be sent (in RUNNING)
        Private futureRequestNextWell As Integer = 0 'AG 07/06/2012 - Using the current next well received in Request, estimate the future well in next request

        Private REAGENT_CONTAMINATION_PERSISTANCE As Integer = 2 'Default initial value for the contamination persistance (real value will be read in the Init method)
        Private MULTIPLE_ERROR_CODE As Integer = 99 'Default value (real value will be read in the Init method)
        Private BASELINE_INIT_FAILURES As Integer = 2 'Default initial value for MAX baseline failures without warning (real value will be read in the Init method)
        Private SENSORUNKNOWNVALUE As Integer = -1 'Default value for several sensors when the 0 value means alarm
        Private WELL_OFFSET_FOR_PREDILUTION As Integer = 4 'Default well offset until next request when a PTEST instruction is sent
        Private WELL_OFFSET_FOR_ISETEST_SERPLM As Integer = 2 'Default well offset until next request when a ISETEST (ser or plm) instruction is sent
        Private WELL_OFFSET_FOR_ISETEST_URI As Integer = 3 'Default well offset until next request when a ISETEST (uri) instruction is sent
        Private MAX_REACTROTOR_WELLS As Integer = 120 'Max wells inside the reactions rotor

        'Alarm Class variables
        Private myClassFieldLimitsDS As New FieldLimitsDS 'AG 30/03/2011 - tfmwFieldLimits contents are read once during the constructor. During running use this DS using Linq

        ' This dictionary implements all flags for saving the analyzer current state (status, action, processes done, processes in course, ...)
        Dim mySessionFlags As New Dictionary(Of String, String)

        'AG 31/03/2011 - Ellapse time for change Warning to Error (performed in Software)
        Private thermoReactionsRotorWarningTimer As New Timer() 'AG 05/01/2012- Controls thermo ReactionsRotor maximum time allowed for the warning alarm
        Private thermoFridgeWarningTimer As New Timer() 'AG 05/01/2012- Controls thermo Fridge maximum time allowed for the warning alarm
        Private wasteDepositTimer As New Timer() 'AG 01/12/2011 - controls the waste deposit removing system
        Private waterDepositTimer As New Timer() 'AG 01/12/2011 - controls the water deposit incoming system
        Private thermoR1ArmWarningTimer As New Timer() 'AG 05/01/2012- Controls thermo R1 arm maximum time allowed for the warning alarm
        Private thermoR2ArmWarningTimer As New Timer() 'AG 05/01/2012- Controls thermo R2 arm maximum time allowed for the warning alarm

        Private wellBaseLineWorker As New BackgroundWorker 'Worker to perform the well base line process C:\Documents and Settings\Sergio_Garcia\Mis documentos\Ax00_v1.0\FwScriptsManagement\Screen Delegates\PositionsAdjustmentDelegate.vb
        '#REFACTORING Private WithEvents baselineCalcs As BaseLineCalculations 'AG 29/04/2011 - Instance is created in AnalyzerManager constructor

        Private wellContaminatedWithWashSent As Integer = 0 'AG 07/06/2011
        Private myBarcodeRequestDS As New AnalyzerManagerDS 'AG 03/08/2011 - When barcode request has several records. Sw has to send: 1st row ... wait results, 2on row ... wait results and so on
        Private readBarCodeBeforeRunningPerformedFlag As Boolean = False 'AG 09/05/2012

        'AG 12/06/2012 - The photometric instructions are treated in a different threat in order to attend quickly to the analyzer requests in Running
        Private processingLastANSPHRInstructionFlag As Boolean = False
        Private bufferANSPHRReceived As New List(Of List(Of InstructionParameterTO)) 'AG 28/06/2012
        Private lockThis As New Object() 'AG 28/06/2012

        'AG 23/07/2012 - Read the Alarm definition when analyzer manager class is created not in several 
        'methods as now (SoundActivationByAlarm, TranslateErrorCodeToAlarmID, RemoveErrorCodeAlarms, ExistFreezeAlarms
        Dim alarmsDefintionTableDS As New AlarmsDS


        'Recovery results variables
        Private bufferInstructionsRESULTSRECOVERYProcess As New List(Of List(Of InstructionParameterTO)) 'AG 27/08/2012 - buffer of instructions received (preparations with problems, results missing, ISE missing)
        Private recoveredPrepIDList As New List(Of Integer) 'AG 25/09/2012

        Private stopRequestedByUserInPauseModeFlag As Boolean = False 'AG 30/10/2013 - Task Task #1342. This flag is set to TRUE when user decides STOP the WorkSession
        '                                                   is used when analyzer is in running pause mode because in this case:
        '                                                   - Sw has to send START instruction + add END into queue


        ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        Private numRepetitionsTimeout As Integer
        Private sendingRepetitions As Boolean
        Private waitingStartTaskTimer As New Timer()
        Private myStartTaskInstructionsQueue As New List(Of GlobalEnumerates.AnalyzerManagerSwActionList)
        Private myStartTaskParamsQueue As New List(Of Object)
        Private myStartTaskFwScriptIDsQueue As New List(Of String)
        Private myStartTaskFwScriptParamsQueue As New List(Of List(Of String))
        ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks

        'Public IsDisplayingServiceData As Boolean = True 'SGM 15/09/2011 exclusion flag for avoiding to update data while displaying data
        'SGM 29/09/2011
        Private InfoRefreshFirstTimeAttr As Boolean = True

        'SGM 17/02/2012
        'Public WithEvents ISEAnalyzer As ISEManager 'REFACTORING

        ' XBC 24/05/2012 - Declare this flag Private for the class
        Private ISEAlreadyStarted As Boolean = False 'provisional flag for starting ISE just the first time 

        ' XBC 13/06/2012
        Private myTmpConnectedAnalyzerDS As New AnalyzersDS

        ' XBC 07/11/2012 - SERVICE
        Public Structure ErrorCodesDisplayStruct
            Public ErrorDateTime As DateTime
            Public ErrorCode As String
            Public ResourceID As String
            Public Solved As Boolean
        End Structure

        '' XB 17/09/2013 - Additional protection - commented by now
        'Public WaitingGUI As Boolean = True
        Private runningConnectionPollSnSent As Boolean = False 'AG 03/12/2013 - BT #1397 - This flag is required because if reconnect in running sometimes firmware sends 2 STATUS instruction, depends on the cycle machine moment where the connection is received
#End Region

#Region "Attributes definition"
        Private classInitializationErrorAttribute As Boolean = False 'AG 15/07/2011

        'AG 19/04/2010 - Definition of Attributes for the class
        Private WorkSessionIDAttribute As String = ""
        Private AnalyzerIDAttribute As String = ""

        ' XBC 14/06/20121
        Private WorkSessionStatusAttribute As String

        'AG 21 & 22/04/2010 Communications attributes
        Private CommThreadsStartedAttribute As Boolean = False      ' Communications threads started
        'Private InitCommAttribute As Boolean = False               ' Port open (init) - AG 05/05/2010 (The INIT instruction isnt used in Ax00 physical layer)
        Private ConnectedAttribute As Boolean = False               ' Connection established
        Private PortNameAttribute As String = ""                    ' If "" automatic (try all ports)||If <> "" connected port name
        Private BaudsAttribute As String = ""                       ' If "" automatic (try all bauds)||If <> "" connected bauds
        Private AnalyzerIsReadyAttribute As Boolean = True         ' AG 19/05/2010 - FALSE: when analyzer is performing some task. Sw isnt allowed to send it new action 
        '____________________________________________________________ TRUE: when analyzer is ready and Sw is can send more actions!!

        Private ISEModuleIsReadyAttribute As Boolean = False      'AG 18/01/2011 - The ISE module isnt ready when has some analysis in process (Fw sends this information in Status instruction)!!!
        Private AnalyzerIsRingingAttribute As Boolean = False 'AG 26/10/2011 - The analyzer is ringing or not
        Private AnalyzerIsInfoActivatedAttribute As Integer = -1 'AG 14/03/2012 - Controls if the ansinf instruction is activated or not
        '                                                                         -1: unknown, 0: no (deactivated), 1: yes (activated)

        'AG 01/06/2010

        ' AG+XBC 24/05/2012
        'Private AnalyzerStatusAttribute As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
        Private AnalyzerStatusAttribute As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
        ' AG+XBC 24/05/2012

        Private AnalyzerCurrentActionAttribute As GlobalEnumerates.AnalyzerManagerAx00Actions = GlobalEnumerates.AnalyzerManagerAx00Actions.NO_ACTION

        Private InstructionSentAttribute As String = "" 'AG 20/10/2010
        Private InstructionReceivedAttribute As String = "" 'AG 20/10/2010
        Private InstructionTypeReceivedAttribute As GlobalEnumerates.AnalyzerManagerSwActionList

        Private myAlarmListAttribute As New List(Of GlobalEnumerates.Alarms) 'AG 16/03/2011
        Private analyzerFREEZEFlagAttribute As Boolean = False 'AG 31/03/2011 indicates the Fw has become into FREEZE mode (several error code indicates this mode)
        Private analyzerFREEZEModeAttribute As String = "" 'AG 02/03/2010 - indicates if the FREEZE is single element or if applies the whole instrument

        Private validALIGHTAttribute As Boolean = False 'AG - inform if exist an valid ALIGHT results (twksWSBLines table)
        Private existsALIGHTAttribute As Boolean = False 'AG 20/06/2012
        Private baselineInitializationFailuresAttribute As Integer = 0 'Alight and well base line initialization failures (used for repeat instructions or show messages)
        Private baselineParametersFailuresAttribute As Boolean = False 'well base line parameters update failures (in Running) (used for show messages)


        'AG 01/04/2011 - Analyzer numerical values attributes (for inform the presentation is needed)
        Private SensorValuesAttribute As New Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single)
        Private CurrentWellAttribute As Integer = 1

        'AG 05/08/2011 - Allow manage the barcode requests before running
        'Basic: on press barcode button from rotor position screen -> request + manage results
        'Complex: on press START or CONTINUE worksession
        '1) Reagents Request + manage results
        '2) Samples Request + manage results
        '3) If warnings -> User solution are required Else Go to Running
        Private BarCodeBeforeRunningProcessStatusAttribute As BarcodeWorksessionActionsEnum = BarcodeWorksessionActionsEnum.BARCODE_AVAILABLE

        Private endRunAlreadySentFlagAttribute As Boolean = False 'AG 05/12/2011 - Remember the ENDRUN instruction has been sent and do not send it again
        '                                                This flag gets TRUE value when instrument sent action ENDRUN_START and is set to FALSE again when action is START_RUNNING_START
        Private abortAlreadySentFlagAttribute As Boolean = False 'AG 16/12/2011 - Remember the ABORT instruction has been sent and do not send it again
        '                                                This flag gets TRUE value when instrument sent action ABORT_START and is set to FALSE again when action is START_RUNNING_START
        Private recoverAlreadySentFlagAttribute As Boolean 'AG 06/03/2012 - Remember the RECOVER instruction has been sent and do not send it again
        '                                                This flag gets TRUE value when instrument sent action RECOVER_INSTRUMENT_START and is set to FALSE again when action is RECOVER_INSTRUMENT_END or when the alarm RECOVER_ERR appears
        Private PauseAlreadySentFlagAttribute As Boolean = False ' XB 15/10/2013 - Remember the PAUSE instruction has been sent and do not send it again - BT #1318
        '                                                This flag gets TRUE value when instrument sent action PAUSE_START and is set to FALSE again when action is START_RUNNING_START

        Private ContinueAlreadySentFlagAttribute As Boolean = False 'TR 25/10/2013 BT#1340 indicate the continue state has been sent.

        Private lastExportedResultsDSAttribute As New ExecutionsDS 'AG 19/03/2013 - Information to export to LIS (using ES) that will be executed from presentation layer

        ' XBC 12/05/2011
        Private OpticCenterResultsAttr As OpticCenterDataTO

        'SGM 20/12/2011
        Private LevelDetectedAttr As Integer = -1

        'SGM 24/05/2011
        Private ManifoldValuesAttribute As New Dictionary(Of GlobalEnumerates.MANIFOLD_ELEMENTS, String)
        Private FluidicsValuesAttribute As New Dictionary(Of GlobalEnumerates.FLUIDICS_ELEMENTS, String)
        Private PhotometricsValuesAttribute As New Dictionary(Of GlobalEnumerates.PHOTOMETRICS_ELEMENTS, String)

        'XBC 08/06/2011
        Private CpuValuesAttribute As New Dictionary(Of GlobalEnumerates.CPU_ELEMENTS, String)


        'SGM 28/07/2011
        'Private CyclesValuesAttribute As New Dictionary(Of GlobalEnumerates.CYCLE_ELEMENTS, String)

        'SGM 23/09/2011
        Private IsShutDownRequestedAttribute As Boolean = False
        Private IsUserConnectRequestedAttribute As Boolean = False

        'SGM 24/11/2011
        Private IsAutoInfoActivatedAttr As Boolean = False

        'SGM 28/11/2011
        Private FwVersionAttribute As String = ""

        'SGM 11/01/2012
        'Private LastISECommandAttr As ISECommandTO
        'Private LastISEResultAttr As ISEResultTO

        ' XBC 03/05/2012
        Private IsStressingAttribute As Boolean

        'SGM 30/05/2012
        Private IsFwSwCompatibleAttr As Boolean = False

        ' XBC 14/06/2012 - functionality to manage the Changing Analyzer which it is connected
        Private IsAnalyzerIDNotLikeWSAttr As Boolean        ' Comparation of Analyzer connected with the last connected
        Private StartingApplicationAttr As Boolean          ' TRUE when app is starting FALSE when a reportsat is loaded
        Private TemporalAnalyzerConnectedAttr As String     ' connected Analyzer ID while user decides continues with this Analyzer or not
        Private ForceAbortSessionAttr As Boolean            ' Invokes Abort functionality

        'SGM 21/06/2012
        Private IsFwReadedAttr As Boolean = False
        Private ReadedFwVersionAttribute As String = ""

        ' XBC 02/08/2012
        Private DifferentWSTypeAtrr As String

        ' XBC 03/10/2012
        Private AdjustmentsReadAttr As Boolean = False

        'SGM 10/10/2012 - SERVICE
        Private TestingCollidedNeedleAttribute As UTILCollidedNeedles = UTILCollidedNeedles.None
        Private SaveSNResultAttribute As UTILSNSavedResults = UTILSNSavedResults.None

        ' XBC 16/10/2012 - SERVICE
        Private myErrorCodesAttribute As New List(Of String)

        'SGM 22/10/2012 - SERVICE
        Private IsServiceAlarmInformedAttr As Boolean = False

        'SGM 22/10/2012 - SERVICE (for ignoring alarms while fw update)
        Private IsFwUpdateInProcessAttr As Boolean = False

        ' XBC 25/10/2012 - SERVICE (for ignoring covers/clot alarms when configure them)
        Private IsConfigGeneralProcessAttr As Boolean = False

        'SGM 26/10/2012 - SERVICE (is waiting for ANSERR after sending INFO Q:2)
        Private IsAlarmInfoRequestedAttr As Boolean = False

        'SGM 29/10/2012 - SERVICE (E:20 received - needed for knowing if a message must be shown)
        Private IsInstructionRejectedAttr As Boolean = False

        'SGM 05/11/2012 - SERVICE (INFO Q:3 locked due to INFO Q:2 sent)
        Private IsInfoRequestPendingAttr As Boolean = False

        ' XBC 07/11/2012 - SERVICE
        Private myErrorCodesDisplayAttribute As New List(Of ErrorCodesDisplayStruct)

        'SGM 07/11/2012 - SERVICE (E:22 received - recover failed)
        Private IsRecoverFailedAttr As Boolean = False

        'SGM 09/11/2012 - SERVICE (E:550 informed - rotor not detected)
        Private IsServiceRotorMissingInformedAttr As Boolean = False

        'SGM 19/11/2012 - SERVICE (E:21 received - inst aborted)
        Private IsInstructionAbortedAttr As Boolean = False
        Private autoWSCreationWithLISModeAttribute As Boolean = False 'AG 11/07/2013 - True when the START ws button creates the ws completely: read barcode, ask to lis, process lis orders and go to running

        Private AllowScanInRunningAttribute As Boolean ' XB 11/10/2013 - PAUSE mode management - BT #1318
        Private pauseModeIsStarting As Boolean = False 'AG 26/03/2014 - #1501 (Physics #48) (initial value: false // When pause mode is starting A 96: true // When pause mode achieved: false)
        '                                                                      this is not an attribute but I declare here for make code understandable

        ' XB 29/01/2014 - Task #1438
        Private BarcodeStartInstrExpectedAttr As Boolean

#End Region

#Region "Properties"

        Public Property classInitializationError() As Boolean Implements IAnalyzerEntity.classInitializationError '15/07/2011 AG
            Get
                Return classInitializationErrorAttribute
            End Get
            Set(ByVal value As Boolean)
                classInitializationErrorAttribute = value
            End Set
        End Property

        Public Property ActiveWorkSession() As String Implements IAnalyzerEntity.ActiveWorkSession   '19/04/2010 AG
            Get
                Return WorkSessionIDAttribute
            End Get

            Set(ByVal value As String)
                WorkSessionIDAttribute = value
                AppLayer.currentWorkSessionID = WorkSessionIDAttribute
            End Set
        End Property

        Public ReadOnly Property WorkSessionStatus() As String Implements IAnalyzerEntity.WorkSessionStatus  ' XBC 14/06/2012
            Get
                Return WorkSessionStatusAttribute
            End Get
        End Property


        Public Property ActiveAnalyzer() As String Implements IAnalyzerEntity.ActiveAnalyzer  '19/04/2010 AG
            Get
                Return AnalyzerIDAttribute
            End Get

            Set(ByVal value As String)
                If AnalyzerIDAttribute <> value Then
                    Dim previousValue As String = AnalyzerIDAttribute 'AG 21/06/2012 - Previous analyzer id
                    AnalyzerIDAttribute = value
                    myAnalyzerModel = GetModelValue(AnalyzerIDAttribute)

                    'AG 20/03/2012 - inform about the current analyzer to the applayer
                    If Not AppLayer Is Nothing Then
                        AppLayer.currentAnalyzerID = AnalyzerIDAttribute
                    End If

                    InitClassStructuresFromDataBase(Nothing) 'AG 31/05/2012

                    'AG 20/03/2012
                    'Initialize internal flags structure
                    InitializeAnalyzerFlags(Nothing, previousValue)

                    'Initialize the analyzer settings
                    InitializeAnalyzerSettings(Nothing)

                    'Initialize the analyzer led positions
                    InitializeAnalyzerLedPositions(Nothing)

                    'Initialize the ISE information master data
                    InitializeISEInformation(Nothing)

                    'SGM 22/03/2012 Init ISEManager instance as disconnected mode
                    'it will be replaced by a new instance when Adjustments received
                    If ISEAnalyzer Is Nothing Then
                        '#TODO REFACTORING ISEAnalyzer = New ISEManager(Me, MyClass.ActiveAnalyzer, myAnalyzerModel, True)
                    End If

                    ''Initialize the FwAdjustments master data
                    'If FwVersionAttribute.Length > 0 Then
                    '    InitializeFWAdjustments(Nothing)
                    'End If

                    'AG 30/08/2013 - Bug # 1271 - when new analyzer is connected Sw has to clear all alarms related to the last analyzer connected
                    'The complete solution is next code but requires a futher validation because it could exists some exceptions (alarm not depending on the connected analyzer) - so remove
                    'only the alarms recalculated with data in DataBase (the base line alarms)
                    'myAlarmListAttribute.Clear()
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                    'AG 30/08/2013

                    ' XBC 14/06/2012
                    TemporalAnalyzerConnectedAttr = value
                    'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                    'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If Not GlobalBase.IsServiceAssembly Then
                        ' User Sw
                        MyClass.InitBaseLineCalculations(Nothing, StartingApplicationAttr)
                    End If
                    ' XBC 14/06/2012

                End If
            End Set
        End Property

        Public Property ActiveFwVersion() As String Implements IAnalyzerEntity.ActiveFwVersion  'SGM 28/11/2011
            Get
                Return FwVersionAttribute
            End Get

            Set(ByVal value As String)
                If FwVersionAttribute <> value Then
                    FwVersionAttribute = value

                    ''Initialize the FwAdjustments master data
                    'If AnalyzerIDAttribute.Length > 0 Then
                    '    InitializeFWAdjustments(Nothing)
                    'End If

                End If
            End Set
        End Property

        ''' <summary>
        ''' FW Version readed from Analyzer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReadedFwVersion() As String Implements IAnalyzerEntity.ReadedFwVersion  'SGM 28/11/2011
            Get
                Return ReadedFwVersionAttribute
            End Get

            Set(ByVal value As String)
                If ReadedFwVersionAttribute <> value Then
                    ReadedFwVersionAttribute = value
                End If
            End Set
        End Property

        Public Property CommThreadsStarted() As Boolean Implements IAnalyzerEntity.CommThreadsStarted    '21/04/2010 AG
            Get
                Return CommThreadsStartedAttribute
            End Get

            Set(ByVal value As Boolean)
                CommThreadsStartedAttribute = value
            End Set
        End Property

        'AG 05/05/2010 - The INIT instruction isnt used in Ax00 physical layer
        'Public Property InitComm() As Boolean    '22/04/2010 AG
        '    Get
        '        Return InitCommAttribute
        '    End Get

        '    Set(ByVal value As Boolean)
        '        InitCommAttribute = value
        '    End Set
        'End Property
        'END AG 05/05/2010

        Public ReadOnly Property Connected() As Boolean Implements IAnalyzerEntity.Connected    '22/04/2010 AG
            Get

                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                    ConnectedAttribute = True
                End If


                Return ConnectedAttribute

            End Get

            'Set(ByVal value As Boolean)
            '    ConnectedAttribute = value
            'End Set
        End Property

        'SGM 22/09/2011
        Public Property IsShutDownRequested() As Boolean Implements IAnalyzerEntity.IsShutDownRequested
            Get
                Return IsShutDownRequestedAttribute
            End Get
            Set(ByVal value As Boolean)
                IsShutDownRequestedAttribute = value
            End Set
        End Property

        'SGM 22/09/2011
        Public Property IsUserConnectRequested() As Boolean Implements IAnalyzerEntity.IsUserConnectRequested
            Get
                Return IsUserConnectRequestedAttribute
            End Get
            Set(ByVal value As Boolean)
                IsUserConnectRequestedAttribute = value
            End Set
        End Property

        Public Property PortName() As String Implements IAnalyzerEntity.PortName  '22/04/2010 AG
            Get
                Return PortNameAttribute
            End Get

            Set(ByVal value As String)
                PortNameAttribute = value
            End Set
        End Property

        Public Property Bauds() As String Implements IAnalyzerEntity.Bauds  '22/04/2010 AG
            Get
                Return BaudsAttribute
            End Get

            Set(ByVal value As String)
                BaudsAttribute = value
            End Set
        End Property

        Public Property AnalyzerIsReady() As Boolean Implements IAnalyzerEntity.AnalyzerIsReady    '19/05/2010 AG
            Get
                Return AnalyzerIsReadyAttribute
            End Get

            Set(ByVal value As Boolean)
                AnalyzerIsReadyAttribute = value
            End Set
        End Property

        Public Property AnalyzerStatus() As GlobalEnumerates.AnalyzerManagerStatus Implements IAnalyzerEntity.AnalyzerStatus 'AG 01/06/2010
            Get
                Return AnalyzerStatusAttribute
            End Get
            Set(ByVal value As GlobalEnumerates.AnalyzerManagerStatus)
                If AnalyzerStatusAttribute <> value Then
                    AnalyzerStatusAttribute = value
                End If
            End Set
        End Property

        Public Property AnalyzerCurrentAction() As GlobalEnumerates.AnalyzerManagerAx00Actions Implements IAnalyzerEntity.AnalyzerCurrentAction   'AG 01/06/2010
            Get
                Return AnalyzerCurrentActionAttribute
            End Get
            Set(ByVal value As GlobalEnumerates.AnalyzerManagerAx00Actions)
                AnalyzerCurrentActionAttribute = value
            End Set
        End Property

        Public Property InstructionSent() As String Implements IAnalyzerEntity.InstructionSent    '20/10/2010 AG
            Get
                Return InstructionSentAttribute
            End Get

            Set(ByVal value As String)
                InstructionSentAttribute = value
            End Set
        End Property

        Public Property InstructionReceived() As String Implements IAnalyzerEntity.InstructionReceived    '20/10/2010 AG
            Get
                Return InstructionReceivedAttribute
            End Get

            Set(ByVal value As String)
                InstructionReceivedAttribute = value
            End Set
        End Property

        Public Property InstructionTypeReceived() As GlobalEnumerates.AnalyzerManagerSwActionList Implements IAnalyzerEntity.InstructionTypeReceived    '02/11/2010 AG
            Get
                Return InstructionTypeReceivedAttribute
            End Get

            Set(ByVal value As GlobalEnumerates.AnalyzerManagerSwActionList)
                InstructionTypeReceivedAttribute = value
            End Set
        End Property

        Public ReadOnly Property InstructionTypeSent() As GlobalEnumerates.AppLayerEventList Implements IAnalyzerEntity.InstructionTypeSent    '27/03/2012 AG
            Get
                Return AppLayer.LastInstructionTypeSent
            End Get
        End Property

        Public Property ISEModuleIsReady() As Boolean Implements IAnalyzerEntity.ISEModuleIsReady    '18/01/2011 AG
            Get
                Return ISEModuleIsReadyAttribute
            End Get

            Set(ByVal value As Boolean)
                ISEModuleIsReadyAttribute = value
            End Set
        End Property

        Public ReadOnly Property Alarms() As List(Of GlobalEnumerates.Alarms) Implements IAnalyzerEntity.Alarms
            Get
                Return myAlarmListAttribute
            End Get
        End Property

        ' XBC 16/10/2012
        Public ReadOnly Property ErrorCodes() As String Implements IAnalyzerEntity.ErrorCodes
            Get
                Dim returnValue As String = ""

                If myErrorCodesAttribute.Count > 0 Then
                    For i As Integer = 0 To myErrorCodesAttribute.Count - 1
                        returnValue = returnValue + myErrorCodesAttribute(i) + ", "
                    Next

                    ' substract last ", "
                    returnValue = returnValue.Substring(0, returnValue.Length - 2)
                End If

                Return returnValue
            End Get
        End Property

        'SGM 22/10/2012 SERVICE
        Public Property IsServiceAlarmInformed() As Boolean Implements IAnalyzerEntity.IsServiceAlarmInformed
            Get
                Return IsServiceAlarmInformedAttr
            End Get
            Set(ByVal value As Boolean)
                IsServiceAlarmInformedAttr = value
            End Set
        End Property

        'SGM 09/11/2012 SERVICE
        Public Property IsServiceRotorMissingInformed() As Boolean Implements IAnalyzerEntity.IsServiceRotorMissingInformed
            Get
                Return IsServiceRotorMissingInformedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsServiceRotorMissingInformedAttr <> value Then
                    IsServiceRotorMissingInformedAttr = value
                End If
            End Set
        End Property
        'SGM 24/10/2012 SERVICE
        Public Property IsFwUpdateInProcess() As Boolean Implements IAnalyzerEntity.IsFwUpdateInProcess
            Get
                Return IsFwUpdateInProcessAttr
            End Get
            Set(ByVal value As Boolean)
                If value <> IsFwUpdateInProcessAttr Then
                    IsFwUpdateInProcessAttr = value
                End If
            End Set
        End Property

        ' XBC 25/10/2012 SERVICE
        Public Property IsConfigGeneralProcess() As Boolean Implements IAnalyzerEntity.IsConfigGeneralProcess
            Get
                Return IsConfigGeneralProcessAttr
            End Get
            Set(ByVal value As Boolean)
                If value <> IsConfigGeneralProcessAttr Then
                    IsConfigGeneralProcessAttr = value
                End If
            End Set
        End Property

        Public ReadOnly Property AnalyzerIsFreeze() As Boolean Implements IAnalyzerEntity.AnalyzerIsFreeze
            Get
                Return analyzerFREEZEFlagAttribute
            End Get
        End Property

        'AG 02/03/2012
        Public ReadOnly Property AnalyzerFreezeMode() As String Implements IAnalyzerEntity.AnalyzerFreezeMode
            Get
                Return analyzerFREEZEModeAttribute
            End Get
        End Property

        Public Property SessionFlag(ByVal pFlag As GlobalEnumerates.AnalyzerManagerFlags) As String Implements IAnalyzerEntity.SessionFlag  '19/04/2010 AG
            Get
                Return mySessionFlags(pFlag.ToString)
            End Get

            Set(ByVal pValue As String)
                mySessionFlags(pFlag.ToString) = pValue
            End Set
        End Property

        Public ReadOnly Property GetSensorValue(ByVal pSensorID As GlobalEnumerates.AnalyzerSensors) As Single Implements IAnalyzerEntity.GetSensorValue
            Get
                If SensorValuesAttribute.ContainsKey(pSensorID) Then
                    Return SensorValuesAttribute(pSensorID)
                Else
                    'AG 30/03/2012 - several sensors have a unknown value because Nothing = 0 means alarm)
                    'Return Nothing
                    If pSensorID = GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS OrElse pSensorID = GlobalEnumerates.AnalyzerSensors.ISE_STATUS Then
                        Return SENSORUNKNOWNVALUE
                    Else
                        Return Nothing
                    End If
                    'AG 30/03/2012
                End If

            End Get
        End Property

        Public WriteOnly Property SetSensorValue(ByVal pSensorID As GlobalEnumerates.AnalyzerSensors) As Single Implements IAnalyzerEntity.SetSensorValue
            Set(ByVal pValue As Single)
                If SensorValuesAttribute.ContainsKey(pSensorID) Then
                    SensorValuesAttribute(pSensorID) = pValue
                Else
                    SensorValuesAttribute.Add(pSensorID, pValue)
                End If

            End Set
        End Property


        '''' <summary>
        '''' SERVICE Monitor Sensor Read Property
        '''' </summary>
        '''' <param name="pSensorID"></param>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>SGM 13/04/11
        '''' AG - Acuerdate de borrarlo sergio (metodo, global enumerates.SENSOR,... </remarks>
        'Public ReadOnly Property GetSensorValue(ByVal pSensorID As GlobalEnumerates.SENSOR) As Double
        '    Get
        '        Dim myGlobal As New GlobalDataTO
        '        myGlobal = AppLayer.GetSensorDataById(pSensorID.ToString)
        '        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
        '            Return CDbl(myGlobal.SetDatos)
        '        End If
        '    End Get
        'End Property

        Public ReadOnly Property MaxWaitTime() As Integer Implements IAnalyzerEntity.MaxWaitTime
            Get
                Return Me.AppLayer.MaxWaitTime
            End Get
        End Property

        Public ReadOnly Property ShowBaseLineInitializationFailedMessage() As Boolean Implements IAnalyzerEntity.ShowBaseLineInitializationFailedMessage
            Get
                Return CBool(IIf(baselineInitializationFailuresAttribute >= BASELINE_INIT_FAILURES, True, False))
            End Get

        End Property


        Public ReadOnly Property ShowBaseLineParameterFailedMessage() As Boolean Implements IAnalyzerEntity.ShowBaseLineParameterFailedMessage
            Get
                Dim result As Boolean = CBool(IIf(AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING And baselineParametersFailuresAttribute, True, False))
                If result Then baselineParametersFailuresAttribute = False 'Once the alarm is shown clear this flag

                Return result
            End Get

        End Property

        ' XBC 17/05/2011
        Public ReadOnly Property SensorValueChanged() As UIRefreshDS.SensorValueChangedDataTable Implements IAnalyzerEntity.SensorValueChanged
            Get
                Return Me.myUI_RefreshDS.SensorValueChanged
            End Get
        End Property

        Public ReadOnly Property ValidALIGHT() As Boolean Implements IAnalyzerEntity.ValidALIGHT    '20/05/2011 AG
            Get
                Return validALIGHTAttribute
            End Get
        End Property

        Public ReadOnly Property ExistsALIGHT() As Boolean Implements IAnalyzerEntity.ExistsALIGHT    '20/06/2012 AG
            Get
                Return existsALIGHTAttribute
            End Get
        End Property

        Public ReadOnly Property CurrentWell() As Integer Implements IAnalyzerEntity.CurrentWell
            Get
                Return CurrentWellAttribute
            End Get
        End Property

        Public Property BarCodeProcessBeforeRunning() As BarcodeWorksessionActionsEnum Implements IAnalyzerEntity.BarCodeProcessBeforeRunning 'AG 04/08/2011
            Get
                Return BarCodeBeforeRunningProcessStatusAttribute
            End Get
            Set(ByVal value As BarcodeWorksessionActionsEnum)
                BarCodeBeforeRunningProcessStatusAttribute = value
            End Set
        End Property

        ' XBC 07/06/2012
        'Public ReadOnly Property GetModelValue() As String
        '    Get
        '        Dim returnValue As String

        '        If AnalyzerIDAttribute.Length > 0 Then
        '            Dim i As Integer
        '            i = AnalyzerIDAttribute.IndexOf("_")
        '            returnValue = AnalyzerIDAttribute.Substring(i + 1)
        '            returnValue = returnValue.Replace("x", "")
        '        Else
        '            returnValue = ""
        '        End If

        '        Return returnValue
        '    End Get
        'End Property

        Public ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String Implements IAnalyzerEntity.GetModelValue
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    Dim strTocompare As String

                    strTocompare = GetUpperPartSN(pAnalyzerID)

                    Select Case strTocompare
                        Case "SN0"  ' TO DELETE
                            returnValue = "A400"
                        Case GlobalBase.BA400ModelID
                            returnValue = "A400"
                    End Select
                End If

                Return returnValue
            End Get
        End Property

        ' XBC 07/06/2012
        Public ReadOnly Property GetUpperPartSN(ByVal pAnalyzerID As String) As String Implements IAnalyzerEntity.GetUpperPartSN
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    returnValue = pAnalyzerID.Substring(0, GlobalBase.SNUpperPartSize)
                End If

                Return returnValue
            End Get
        End Property

        ' XBC 07/06/2012
        Public ReadOnly Property GetLowerPartSN(ByVal pAnalyzerID As String) As String Implements IAnalyzerEntity.GetLowerPartSN
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    returnValue = pAnalyzerID.Substring(GlobalBase.SNUpperPartSize, GlobalBase.SNLowerPartSize)
                End If

                Return returnValue
            End Get
        End Property


        Public ReadOnly Property Ringing() As Boolean Implements IAnalyzerEntity.Ringing    '26/10/2011 AG
            Get
                Return AnalyzerIsRingingAttribute
            End Get
        End Property


        'SGM 24/11/2011
        Public Property IsAutoInfoActivated() As Boolean Implements IAnalyzerEntity.IsAutoInfoActivated
            Get
                Return IsAutoInfoActivatedAttr
            End Get
            Set(ByVal value As Boolean)
                IsAutoInfoActivatedAttr = value
            End Set
        End Property

        'SGM 26/10/2012 -SERVICE: INFO:Q2 is sent. wait for ANSERR
        Public Property IsAlarmInfoRequested() As Boolean Implements IAnalyzerEntity.IsAlarmInfoRequested
            Get
                Return IsAlarmInfoRequestedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsAlarmInfoRequestedAttr <> value Then
                    IsAlarmInfoRequestedAttr = value
                    Debug.Print("AlarmInfoRequested = " & value.ToString)
                End If
            End Set
        End Property

        ''SGM 05/11/2012 -SERVICE: INFO:Q3 locked due to INFO:Q2 sent
        'Public Property IsInfoRequestPending() As Boolean
        '    Get
        '        Return IsInfoRequestPendingAttr
        '    End Get
        '    Set(ByVal value As Boolean)
        '        If IsInfoRequestPendingAttr <> value Then
        '            IsInfoRequestPendingAttr = value
        '        End If
        '    End Set
        'End Property

        'SGM 29/10/2012 -SERVICE: E:20 Instruction Rejected Error Received
        Public Property IsInstructionRejected() As Boolean Implements IAnalyzerEntity.IsInstructionRejected
            Get
                Return IsInstructionRejectedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsInstructionRejectedAttr <> value Then
                    IsInstructionRejectedAttr = value
                End If
            End Set
        End Property

        'SGM 07/11/2012 -SERVICE: E:22 Recover Failed
        Public Property IsRecoverFailed() As Boolean Implements IAnalyzerEntity.IsRecoverFailed
            Get
                Return IsRecoverFailedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsRecoverFailedAttr <> value Then
                    IsRecoverFailedAttr = value
                End If
            End Set
        End Property

        'SGM 19/11/2012 -SERVICE: E:22 Instruction Aborted
        Public Property IsInstructionAborted() As Boolean Implements IAnalyzerEntity.IsInstructionAborted
            Get
                Return IsInstructionAbortedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsInstructionAbortedAttr <> value Then
                    IsInstructionAbortedAttr = value
                End If
            End Set
        End Property

        Public Property LevelDetected() As Boolean Implements IAnalyzerEntity.LevelDetected
            Get
                Return (LevelDetectedAttr > 0)
            End Get
            Set(ByVal value As Boolean)
                LevelDetectedAttr = -1
            End Set
        End Property

        Public ReadOnly Property EndRunInstructionSent() As Boolean Implements IAnalyzerEntity.EndRunInstructionSent
            Get
                Return endRunAlreadySentFlagAttribute
            End Get
        End Property

        Public ReadOnly Property AbortInstructionSent() As Boolean Implements IAnalyzerEntity.AbortInstructionSent
            Get
                Return abortAlreadySentFlagAttribute
            End Get
        End Property

        Public ReadOnly Property RecoverInstructionSent() As Boolean Implements IAnalyzerEntity.RecoverInstructionSent
            Get
                Return recoverAlreadySentFlagAttribute
            End Get
        End Property

        ' XB 15/10/2013 - BT #1318
        Public ReadOnly Property PauseInstructionSent() As Boolean Implements IAnalyzerEntity.PauseInstructionSent
            Get
                Return PauseAlreadySentFlagAttribute
            End Get
        End Property

        'TR 25/10/2013 BT #1340
        Public ReadOnly Property ContinueAlreadySentFlag() As Boolean Implements IAnalyzerEntity.ContinueAlreadySentFlag
            Get
                Return ContinueAlreadySentFlagAttribute
            End Get
        End Property

        Public Property InfoActivated() As Integer Implements IAnalyzerEntity.InfoActivated 'AG 14/03/2012
            Get
                Return AnalyzerIsInfoActivatedAttribute
            End Get
            Set(ByVal value As Integer)
                AnalyzerIsInfoActivatedAttribute = value
            End Set
        End Property

        'SGM 10/01/2012
        'Public Property LastISECommand() As ISECommandTO
        '    Get
        '        Return LastISECommandAttr
        '    End Get
        '    Set(ByVal value As ISECommandTO)
        '        LastISECommandAttr = value
        '    End Set
        'End Property

        'Public Property LastISEResult() As ISEResultTO
        '    Get
        '        Return LastISEResultAttr
        '    End Get
        '    Set(ByVal value As ISEResultTO)
        '        LastISEResultAttr = value
        '    End Set
        'End Property


        ' XBC 03/05/2012
        Public Property IsStressing() As Boolean Implements IAnalyzerEntity.IsStressing
            Get
                Return IsStressingAttribute
            End Get
            Set(ByVal value As Boolean)
                IsStressingAttribute = value
            End Set
        End Property


        'SGM 30/05/2012
        Public Property IsFwSwCompatible() As Boolean Implements IAnalyzerEntity.IsFwSwCompatible
            Get
                Return IsFwSwCompatibleAttr
            End Get
            Set(ByVal value As Boolean)
                IsFwSwCompatibleAttr = value
            End Set
        End Property

        'SGM 21/06/2012
        Public Property IsFwReaded() As Boolean Implements IAnalyzerEntity.IsFwReaded
            Get
                Return IsFwReadedAttr
            End Get
            Set(ByVal value As Boolean)
                IsFwReadedAttr = value
            End Set
        End Property

        'AG 07/06/2012 - Informs that well base line calculations has paused the work session (consecutive rejected wells > Limit)
        Public ReadOnly Property wellBaseLineAutoPausesSession() As Integer Implements IAnalyzerEntity.wellBaseLineAutoPausesSession
            Get
                Return BaseLine.exitRunningType
            End Get
        End Property

        ' XBC 21/06/2012
        Public Property IsAnalyzerIDNotLikeWS() As Boolean Implements IAnalyzerEntity.IsAnalyzerIDNotLikeWS
            Get
                Return IsAnalyzerIDNotLikeWSAttr
            End Get
            Set(ByVal value As Boolean)
                IsAnalyzerIDNotLikeWSAttr = value
            End Set
        End Property

        ' XBC 14/06/2012
        Public Property StartingApplication() As Boolean Implements IAnalyzerEntity.StartingApplication
            Get
                Return StartingApplicationAttr
            End Get
            Set(ByVal value As Boolean)
                StartingApplicationAttr = value
            End Set
        End Property

        ' XCB 18/06/2012
        Public ReadOnly Property TemporalAnalyzerConnected() As String Implements IAnalyzerEntity.TemporalAnalyzerConnected
            Get
                Return TemporalAnalyzerConnectedAttr
            End Get
        End Property

        ' XBC 19/06/2012
        Public Property ForceAbortSession() As Boolean Implements IAnalyzerEntity.ForceAbortSession
            Get
                Return ForceAbortSessionAttr
            End Get
            Set(ByVal value As Boolean)
                ForceAbortSessionAttr = value
            End Set
        End Property

        ' XBC 02/08/2012
        Public Property DifferentWSType() As String Implements IAnalyzerEntity.DifferentWSType
            Get
                Return MyClass.DifferentWSTypeAtrr
            End Get
            Set(ByVal value As String)
                MyClass.DifferentWSTypeAtrr = value
            End Set
        End Property

        ' XBC 03/10/2012
        Public Property AdjustmentsRead() As Boolean Implements IAnalyzerEntity.AdjustmentsRead
            Get
                Return AdjustmentsReadAttr
            End Get
            Set(ByVal value As Boolean)
                AdjustmentsReadAttr = value
            End Set
        End Property

        'SGM 10/10/2012 - SERVICE
        Public Property TestingCollidedNeedle() As UTILCollidedNeedles Implements IAnalyzerEntity.TestingCollidedNeedle
            Get
                Return TestingCollidedNeedleAttribute
            End Get
            Set(ByVal value As UTILCollidedNeedles)
                TestingCollidedNeedleAttribute = value
            End Set
        End Property

        'SGM 10/10/2012 - SERVICE
        Public Property SaveSNResult() As UTILSNSavedResults Implements IAnalyzerEntity.SaveSNResult
            Get
                Return SaveSNResultAttribute
            End Get
            Set(ByVal value As UTILSNSavedResults)
                SaveSNResultAttribute = value
            End Set
        End Property

        ' XBC 07/11/2012 - SERVICE
        Public ReadOnly Property ErrorCodesDisplay() As List(Of ErrorCodesDisplayStruct) Implements IAnalyzerEntity.ErrorCodesDisplay
            Get
                Return myErrorCodesDisplayAttribute
            End Get
        End Property

        'SGM 08/11/2012 SERVICE for refreshing sensors monitor
        Public Property InfoRefreshFirstTime() As Boolean Implements IAnalyzerEntity.InfoRefreshFirstTime
            Get
                Return InfoRefreshFirstTimeAttr
            End Get
            Set(ByVal value As Boolean)
                If InfoRefreshFirstTimeAttr <> value Then
                    InfoRefreshFirstTimeAttr = value
                    If value Then
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            SensorValuesAttribute.Clear() 'SGM 08/11/2012
                        End If
                    End If
                End If
            End Set
        End Property

        'AG 19/03/2013 - Information to export to LIS (using ES) that will be executed from presentation layer
        Public ReadOnly Property LastExportedResults() As ExecutionsDS Implements IAnalyzerEntity.LastExportedResults
            Get
                SyncLock lockThis
                    Return lastExportedResultsDSAttribute
                End SyncLock
            End Get
        End Property
        'AG 19/03/2013

        Public WriteOnly Property autoWSCreationWithLISMode() As Boolean Implements IAnalyzerEntity.autoWSCreationWithLISMode 'AG 11/07/2013
            Set(ByVal value As Boolean)
                autoWSCreationWithLISModeAttribute = value
            End Set
        End Property

        Public ReadOnly Property AllowScanInRunning() As Boolean Implements IAnalyzerEntity.AllowScanInRunning
            Get
                Return AllowScanInRunningAttribute
            End Get
        End Property

        ' XB 29/01/2014 - Task #1438
        Public Property BarcodeStartInstrExpected As Boolean Implements IAnalyzerEntity.BarcodeStartInstrExpected
            Get
                Return BarcodeStartInstrExpectedAttr
            End Get
            Set(ByVal value As Boolean)
                BarcodeStartInstrExpectedAttr = value
            End Set
        End Property

        'SGM 29/05/2012
        Public Property FWUpdateResponseData As FWUpdateResponseTO Implements IAnalyzerEntity.FWUpdateResponseData '#REFACTORING
        Public Property AdjustmentsFilePath As String Implements IAnalyzerEntity.AdjustmentsFilePath '#REFACTORING

#End Region

#Region "Events definition & methods"
        ''' <summary>
        ''' Event with the instruction received
        ''' 2 reception events are raised:
        ''' - First at the begining, instruction received not threated, serves for trace instruction sequence (pTreated = False)
        ''' - Second at the end, instruction received and threated, serves for trace business sequence (pTreated = true)
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <param name="pTreated"></param>
        ''' <remarks>AG 29/10/2010</remarks>
        Public Event ReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                           ByVal pUIRefresh_Event As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pUI_RefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Implements IAnalyzerEntity.ReceptionEvent '#REFACTORING
        ''' <summary>
        ''' Event with the instruction sent
        ''' </summary>
        ''' <param name="pInstructionSent"></param>
        ''' <remarks>AG 29/10/2010</remarks>
        Public Event SendEvent(ByVal pInstructionSent As String) Implements IAnalyzerEntity.SendEvent '#REFACTORING

        ''' <summary>
        ''' Event to activate/deactivate WatchDog timer - Task #1438
        ''' </summary>
        ''' <param name="pEnable"></param>
        ''' <remarks>XB 29/01/2014</remarks>

        Public Event WatchDogEvent(ByVal pEnable As Boolean) Implements IAnalyzerEntity.WatchDogEvent '#REFACTORING

        'AG 20/04/2010
        ''' <summary>
        ''' Event from ApplicationLayer
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' </remarks>
        Public Sub OnManageAnalyzerEvent(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList, ByVal pInstructionReceived As List(Of InstructionParameterTO)) Handles AppLayer.ManageAnalyzer
            Try
                ''Track SGM
                'Dim myTrackTime As Double = DateTime.Now.Subtract(AppLayer.ResponseTrack).TotalMilliseconds
                'Dim myLogAcciones2 As New ApplicationLogManager()
                'myLogAcciones2.CreateLogActivity("AppLayer -- AnalyzerManager (" & pAction.ToString & "): " & myTrackTime.ToStringWithDecimals(0), "AnalyzerManager.OnManageAnalyzerEvent", EventLogEntryType.Information, False)
                ''Select Case pAction
                ''    Case AnalyzerManagerSwActionList.STATUS_RECEIVED, _
                ''        AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED, _
                ''        AnalyzerManagerSwActionList.READINGS_RECEIVED

                ''        Dim myTrackTime As Double = DateTime.Now.Subtract(AppLayer.ResponseTrack).TotalMilliseconds
                ''        Dim myLogAcciones2 As New ApplicationLogManager()
                ''        myLogAcciones2.CreateLogActivity("Instruccion Received Event Receive (" & pAction.ToString & "): " & myTrackTime.ToStringWithDecimals(0), "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Information, False)

                ''End Select
                ''end Track

                ManageAnalyzer(pAction, False, pInstructionReceived)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageAnalyzerEvent", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' CREATED BY:  AG 14/10/2011
        ''' Modified BY: TR 10/11/2011 -Create a trace on the application log 
        '''                             indicating there was a communicanito error..
        ''' </remarks>
        Private Sub OnManageCommunicateErrorEvent() Handles AppLayer.SendDataFailed
            Try
                ConnectedAttribute = False

                Dim myMessageDelegate As New MessageDelegate()
                Dim myGlobalDataTO As New GlobalDataTO()
                Dim TextMessage As String = ""
                'Get the message on English language.
                myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, GlobalEnumerates.Messages.ERROR_COMM.ToString(), "ENG")
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As New MessagesDS
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        TextMessage = myMessagesDS.tfmwMessages(0).MessageText
                    End If
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(TextMessage, "AnalyzerManager.OnManageCommunicateErrorEvent", EventLogEntryType.Information, False)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageCommunicateErrorEvent", EventLogEntryType.Error, False)
            End Try

        End Sub

        ''' <summary>
        ''' Manage the waiting time
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Creation AG 07/05/2010 - Pending define how to use
        ''' Modified by RH: 29/06/2010 Move "Deactivate waiting time control" logic
        ''' </remarks>
        Private Sub waitingTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Try
                'Deactivate waiting time control
                InitializeTimerControl(WAITING_TIME_OFF)

                ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED, True)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.waitingTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Manage the waiting time for Start Task instructions
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Creation XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        ''' </remarks>
        Private Sub waitingStartTaskTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Try
                'Deactivate waiting time control
                MyClass.InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                MyClass.ClearQueueToSend()

                MyClass.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.START_TASK_TIMEOUT, True)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.waitingStartTaskTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Get the event generated by the base line well calculations
        ''' </summary>
        ''' <param name="pReactionsRotorWellDS"></param>
        ''' <remarks>AG 20/05/2011 - creation
        ''' AG 13/09/2012 - only in Running</remarks>
        Public Sub OnManageWellReactionsChanges(ByVal pReactionsRotorWellDS As ReactionsRotorDS) Handles _baseLine.WellReactionsChanges
            Try
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso pReactionsRotorWellDS.twksWSReactionsRotor.Rows.Count > 0 Then
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = PrepareUIRefreshEventNum3(Nothing, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, pReactionsRotorWellDS, True) 'AG 12/06/2012 'False)

                    'AG 12/06/2012
                    ''If the ANSPHR instruction has already raise an event for the chemical reactions then generate another for the well base line calculations
                    'If Not myGlobal.HasError Then
                    '    If mySecondaryUI_RefreshEvent.Count = 0 Then mySecondaryUI_RefreshDS.Clear()
                    '    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, mySecondaryUI_RefreshEvent, mySecondaryUI_RefreshDS, False)
                    '    'secondaryEventDataPendingToTriggerFlag = False 'AG 07/10/2011 - inform not exists information in UI_RefreshDS to be send to the event
                    'End If

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageWellReactionsChanges", EventLogEntryType.Error, False)
            End Try
        End Sub


        ' SERVICE SOFTWARE
        ' XBC 16/11/2010
        Public Event ReceptionFwScriptEvent(ByVal pDataReceived As String, _
                                                   ByVal pResponseValue As String, _
                                                   ByVal pTreated As Boolean) Implements IAnalyzerEntity.ReceptionFwScriptEvent

        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
        ' SGM 15/04/2011 
        'Public Shared Event SensorValuesChangedEvent(ByVal pSensorValuesChangedDT As UIRefreshDS.SensorValueChangedDataTable)


        ''' <summary>
        ''' Detects that the ISE data aimed to be displayed in the Monitor has been changed
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEMonitorDataChanged() Handles _iseAnalyzer.ISEMonitorDataChanged
            Try
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_MONITOR_DATA_CHANGED, 1, True)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEOperationFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE is ready for ISE Preparations
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEReadyChanged() Handles _iseAnalyzer.ISEReadyChanged
            Try
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED, 1, True)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEReadyChanged", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has been switched On
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISESwitchedOnChanged() Handles _iseAnalyzer.ISESwitchedOnChanged
            Try
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED, 1, True)
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISESwitchedOnChanged", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has finished the initial data retrieving process (Ok or not)
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEConnectionFinished(ByVal pOk As Boolean) Handles _iseAnalyzer.ISEConnectionFinished
            Try
                Dim myGlobal As New GlobalDataTO
                Dim myValue As Integer = 0

                Dim myAlarmListTmp As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobal = ISEAnalyzer.CheckAlarms(MyClass.Connected, myAlarmList, myAlarmStatusList)

                If pOk Then
                    myValue = 1

                    'The ISE Module was switched On and the user has connected from Utilities

                    myAlarmList.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                    myAlarmStatusList.Add(False)

                    myAlarmList.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                    myAlarmStatusList.Add(False)

                Else
                    myValue = 2

                    'The ISE Module was switched On but the connection is failed and then is pending
                    myAlarmList.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                    myAlarmStatusList.Add(True)

                    myAlarmList.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                    myAlarmStatusList.Add(False)
                End If

                For i As Integer = 0 To myAlarmListTmp.Count - 1
                    PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                Next

                If myAlarmList.Count > 0 Then
                    'myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 17/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'myGlobal = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        myGlobal = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    End If
                End If

                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED, myValue, True)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEConnectionFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has finished the procedure requested
        ''' </summary>
        ''' <remarks>
        ''' Created by SGM 15/03/2012
        ''' </remarks>
        Public Sub OnISEProcedureFinished() Handles _iseAnalyzer.ISEProcedureFinished

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED, 1, True)


                'SGM 12/04/2012 Update ISE conssumption flag in case of Session ended, paused or aborted
                If ISEAnalyzer.CurrentProcedure = ISEAnalyzerEntity.ISEProcedures.WriteConsumption Then

                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption, "END")

                    'SGM 27/09/2012 - not necessary
                    ''If pause in process mark as closed
                    'If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString) = "INPROCESS" Then
                    '    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess, "CLOSED")
                    'End If

                    'Once the ise consumption is updated activate ansinf
                    If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then

                        AnalyzerIsInfoActivatedAttribute = 0
                        myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                    End If
                End If
                'end SGM 12/04/2012

                ISEAnalyzer.CurrentCommandTO = Nothing
                ISEAnalyzer.CurrentProcedure = ISEAnalyzerEntity.ISEProcedures.None


            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEProcedureFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that some maintenance schedule is needed
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEMaintenanceRequired(ByVal pOperation As ISEAnalyzerEntity.MaintenanceOperations) Handles _iseAnalyzer.ISEMaintenanceRequired
            Try
                Select Case pOperation
                    Case ISEAnalyzerEntity.MaintenanceOperations.ElectrodesCalibration : UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_CALB_REQUIRED, 1, True)
                    Case ISEAnalyzerEntity.MaintenanceOperations.PumpsCalibration : UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_PUMPCAL_REQUIRED, 1, True)
                    Case ISEAnalyzerEntity.MaintenanceOperations.Clean : UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.ISE_CLEAN_REQUIRED, 1, True)
                End Select

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEElectrodeInstalled", EventLogEntryType.Error, False)
            End Try
        End Sub
        'end SGM 07/03/2012


#End Region

#Region "Constructor and other initialization methods"
        ' XBC 02/03/2011
        'Public Sub New()
        Public Sub New(ByVal pApplicationName As String, ByVal pAnalyzerModel As String)
            'AG 20/04/2010
            Try
                classInitializationErrorAttribute = False

                If AppLayer Is Nothing Then
                    AppLayer = New ApplicationLayer
                End If
                'AG 07/05/2010
                waitingTimer.Enabled = False
                AddHandler waitingTimer.Elapsed, AddressOf waitingTimer_Timer

                'XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                waitingStartTaskTimer.Enabled = False
                AddHandler waitingStartTaskTimer.Elapsed, AddressOf waitingStartTaskTimer_Timer

                myApplicationName = pApplicationName
                myAnalyzerModel = pAnalyzerModel 'SGM 04/03/11

                Dim myGlobalDataTO As New GlobalDataTO

                'AG 23/07/2012
                Dim myAlarmsDelegate As New AlarmsDelegate
                myGlobalDataTO = myAlarmsDelegate.ReadAll(Nothing)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    alarmsDefintionTableDS = DirectCast(myGlobalDataTO.SetDatos, AlarmsDS)
                End If

                'AG 02/05/2011 - Move this code to property ActiveAnalyzer (set) once the analyzer model is informed
                'myGlobalDataTO = InitClassStructuresFromDataBase(Nothing)
                'If myGlobalDataTO.HasError Then
                '    classInitializationErrorAttribute = True
                'End If

                'AG 20/04/2011 - Define methods who will implement work
                AddHandler wellBaseLineWorker.DoWork, AddressOf wellBaseLineWorker_DoWork
                AddHandler wellBaseLineWorker.RunWorkerCompleted, AddressOf wellBaseLineWorker_RunWorkerCompleted
                'AG 20/04/2011

                'AG 01/12/2011 - Water & Waste deposit error is generated by a timer
                Dim mySwParameterDelegate As New SwParametersDelegate
                wasteDepositTimer.Enabled = False
                waterDepositTimer.Enabled = False

                wasteDepositTimer.Interval = 60000 'default interval value 1 minute
                waterDepositTimer.Interval = 60000 'default interval value 1 minute

                myGlobalDataTO = mySwParameterDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_TIME_DEPOSIT_WARN.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    If IsNumeric(myGlobalDataTO.SetDatos) AndAlso CInt(myGlobalDataTO.SetDatos) > 0 Then
                        wasteDepositTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                        waterDepositTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                    End If
                End If
                AddHandler wasteDepositTimer.Elapsed, AddressOf WasteDepositError_Timer
                AddHandler waterDepositTimer.Elapsed, AddressOf WaterDepositError_Timer
                'AG 01/12/2011

                'AG 05/01/2012 - Initialize thermo alarm errors timers (REACTIONS and FRIDGE)
                thermoReactionsRotorWarningTimer.Enabled = False
                thermoReactionsRotorWarningTimer.Interval = 300000 'default interval value 5 minutes
                thermoFridgeWarningTimer.Enabled = False
                thermoFridgeWarningTimer.Interval = 120000 'default interval value 2 minutes


                'Read the maximum time allowed for pass form warning to alarm (reactions rotor)
                myGlobalDataTO = mySwParameterDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_TIME_THERMO_REACTIONS_WARN.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    thermoReactionsRotorWarningTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                End If

                myGlobalDataTO = mySwParameterDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_TIME_THERMO_FRIDGE_WARN.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    thermoFridgeWarningTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                End If

                AddHandler thermoReactionsRotorWarningTimer.Elapsed, AddressOf ThermoReactionsRotorError_Timer
                AddHandler thermoFridgeWarningTimer.Elapsed, AddressOf ThermoFridgeError_Timer
                'END AG 05/01/2012

                'AG 05/01/2012 - Initialize thermo alarm errors timers (R1 and R2 arms)
                thermoR1ArmWarningTimer.Enabled = False
                thermoR1ArmWarningTimer.Interval = 60000 'default interval value 1 minutes
                thermoR2ArmWarningTimer.Enabled = False
                thermoR2ArmWarningTimer.Interval = 60000 'default interval value 1 minutes


                'Read the maximum time allowed for pass form warning to alarm (reactions rotor)
                myGlobalDataTO = mySwParameterDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_TIME_THERMO_ARM_REAGENTS_WARN.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    thermoR1ArmWarningTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                    thermoR2ArmWarningTimer.Interval = CInt(myGlobalDataTO.SetDatos) * 1000
                End If

                AddHandler thermoR1ArmWarningTimer.Elapsed, AddressOf thermoR1ArmWarningTimer_Timer
                AddHandler thermoR2ArmWarningTimer.Elapsed, AddressOf thermoR2ArmWarningTimer_Timer
                'END AG 05/01/2012

                AdjustmentsFilePath = String.Empty '#REFACTORING

            Catch ex As Exception
                classInitializationErrorAttribute = True
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.New", EventLogEntryType.Error, False)
            End Try
        End Sub
        ' XBC 02/03/2011

        ''' <summary>
        ''' Initializes the base line calculation object when app is started
        ''' </summary>
        ''' <param name="pDBConnection" ></param>
        ''' <param name="pStartingApplication" >TRUE when app is starting FALSE when a reportsat is loaded</param>
        ''' <remarks>AG 29/04/2011
        '''          AG 25/10/2011 - add parameter pStartingApplication for differentiate when the application is started and when a reportsat or restore point is loaded</remarks>
        Public Sub InitBaseLineCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStartingApplication As Boolean) Implements IAnalyzerEntity.InitBaseLineCalculations
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'AG 25/10/2011 - When a ReportSAT is loaded or a RestorePoint is restored the baseline calc must to be created again
                        If Not BaseLine Is Nothing AndAlso Not pStartingApplication Then
                            'BaseLine = Nothing '#REFACTORING

                            'AG 09/03/2012 - when load a reportsat and no connection remove all previous refresh information
                            If Not ConnectedAttribute Then
                                'myUI_RefreshEvent.Clear()
                                'myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, True) 'AG 22/05/2014 - #1637
                            End If

                            If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                            If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                myAlarmListAttribute.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
                                baselineParametersFailuresAttribute = False
                            End If

                            resultData = InitClassStructuresFromDataBase(dbConnection)
                            'Else '#REFACTORING
                            '    If Not BaseLine Is Nothing Then BaseLine = Nothing 'AG 30/08/2013 - When the analyzer is different from the last connected we must rebuild this structure
                        End If
                        'AG 25/10/2011

                        'If BaseLine Is Nothing Then '#REFACTORING
                        'baselineCalcs = New BaseLineCalculations '#REFACTORING
                        BaseLine.Initialize() '#REFACTORING

                        BaseLine.fieldLimits = myClassFieldLimitsDS

                        Dim mySwParameterDelegate As New SwParametersDelegate
                        resultData = mySwParameterDelegate.ReadByAnalyzerModel(Nothing, myAnalyzerModel)
                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                            Dim myParamDS As New ParametersDS
                            myParamDS = CType(resultData.SetDatos, ParametersDS)
                            BaseLine.SwParameters = myParamDS

                            validALIGHTAttribute = False
                            resultData = BaseLine.GetLatestBaseLines(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myAnalyzerModel)
                            validALIGHTAttribute = BaseLine.validALight
                            existsALIGHTAttribute = BaseLine.existsAlightResults 'AG 20/06/2012

                            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                Dim myAlarm As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                                myAlarm = CType(resultData.SetDatos, GlobalEnumerates.Alarms)
                                'Update internal alarm list if exists alarm but not saved it into database!!!
                                If myAlarm <> GlobalEnumerates.Alarms.NONE Then
                                    If Not myAlarmListAttribute.Contains(myAlarm) Then
                                        myAlarmListAttribute.Add(myAlarm)

                                        'AG 12/09/2012 - If base line error when app is starting set the attribute baselineInitializationFailuresAttribute to value in order to show the alarm globe in monitor
                                        'Wup not in process 
                                        If pStartingApplication AndAlso myAlarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR Then
                                            Dim showGlobeFlag As Boolean = False
                                            If (String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS") OrElse _
                                                String.Equals(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess.ToString), "PAUSED")) Then
                                                If Not IgnoreAlarmWhileWarmUp(myAlarm) Then
                                                    showGlobeFlag = True
                                                End If
                                            Else
                                                showGlobeFlag = True
                                            End If

                                            If showGlobeFlag Then
                                                'Set this attribute to limit in order the alarm will be shown as monitor globe
                                                baselineInitializationFailuresAttribute = BASELINE_INIT_FAILURES
                                            End If

                                        End If
                                        'AG 12/09/2012

                                        'AG 13/02/2012
                                        'Prepare UIRefresh DS (generate event only when a ReportSAT is loaded or a RestorePoint is restored)
                                        resultData = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                                        If Not BaseLine Is Nothing AndAlso Not pStartingApplication Then
                                            InstructionReceivedAttribute = ""
                                            RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                                        End If
                                        'AG 13/02/2012
                                    End If
                                End If
                            End If
                        End If
                        'End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitBaseLineCalculations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
        End Sub


        ''' <summary>
        ''' Read from database and initialize class structures (limits,...)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>AG 26/10/2011 - Create and move business code from constructor (New)</remarks>
        Private Function InitClassStructuresFromDataBase(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'TR 14/03/2011 -Initialize REAGENT_CONTAMINATION_PERSISTANCE variable
                        Dim mySwParameterDelegate As New SwParametersDelegate
                        Dim myGlobalDataTO As New GlobalDataTO
                        Dim myParamDS As New ParametersDS

                        myGlobalDataTO = mySwParameterDelegate.ReadByAnalyzerModel(dbConnection, myAnalyzerModel)
                        If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                            myParamDS = CType(myGlobalDataTO.SetDatos, ParametersDS)

                            Dim myQRes As New List(Of ParametersDS.tfmwSwParametersRow)
                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                REAGENT_CONTAMINATION_PERSISTANCE = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.MULTIPLE_ERROR_CODE.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                MULTIPLE_ERROR_CODE = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.BLINE_INIT_FAILURES.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                BASELINE_INIT_FAILURES = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.SENSORUNKNOWNVALUE.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                SENSORUNKNOWNVALUE = CInt(myQRes(0).ValueNumeric)
                            End If

                            'AG 31/05/2012
                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.PREDILUTION_CYCLES.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_PREDILUTION = CInt(myQRes(0).ValueNumeric) - 1 'Well offset for predilution = (predilution cycles used for time estimation - 1)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.MAX_REACTROTOR_WELLS.ToString, False) = 0 Select a).ToList

                            If myQRes.Count > 0 Then
                                MAX_REACTROTOR_WELLS = CInt(myQRes(0).ValueNumeric)
                            End If
                            'AG 31/05/2012

                            'AG 12/07/2012
                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.ISETEST_CYCLES_SERPLM.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_ISETEST_SERPLM = CInt(myQRes(0).ValueNumeric)  'Well offset for ise test (ser or plm) (cycles until Fw activates the biochemical request after receive an SER or PLM isetest)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDS.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.ISETEST_CYCLES_URI.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_ISETEST_URI = CInt(myQRes(0).ValueNumeric)  'Well offset for ise test (uri) (cycles until Fw activates the biochemical request after receive an URI isetest)
                            End If
                            'AG 12/07/2012
                            myQRes = Nothing 'AG 02/08/2012 - free memory
                        End If
                        'AG 02/05/2011

                        'AG 30/03/2011
                        Dim myFieldLimitsDelegate As New FieldLimitsDelegate
                        myGlobalDataTO = myFieldLimitsDelegate.GetAllList(dbConnection)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myClassFieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)
                        End If
                        'AG 30/03/2011


                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InitClassStructuresFromDataBase", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Start communications channel
        ''' </summary>
        ''' <param name="pPooling"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Start(ByVal pPooling As Boolean) As Boolean Implements IAnalyzerEntity.Start
            Dim myReturn As Boolean = False
            Try
                Dim myGlobal As New GlobalDataTO
                CommThreadsStartedAttribute = False
                myGlobal = AppLayer.Start(pPooling)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    CommThreadsStartedAttribute = myReturn
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.Start", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Restart communications channel
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 04/07/2012</remarks>
        Public Function SynchronizeComm() As Boolean Implements IAnalyzerEntity.SynchronizeComm
            Dim myReturn As Boolean = False
            Try
                Dim myGlobal As New GlobalDataTO
                CommThreadsStartedAttribute = False
                myGlobal = AppLayer.SynchronizeComm()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    CommThreadsStartedAttribute = myReturn
                    ConnectedAttribute = myReturn
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SynchronizeComm", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Dispose communications channel
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Terminate() As Boolean Implements IAnalyzerEntity.Terminate
            Dim myReturn As Boolean = False
            Try
                CommThreadsStartedAttribute = False
                AppLayer.Terminate()
                myReturn = True

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.Terminate", EventLogEntryType.Error, False)
                myReturn = False
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Stop communications channel
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StopComm() As Boolean Implements IAnalyzerEntity.StopComm
            Dim myReturn As Boolean = False
            Try
                Dim myGlobal As New GlobalDataTO
                CommThreadsStartedAttribute = False
                myGlobal = AppLayer.StopComm()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0, True)
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StopComm", EventLogEntryType.Error, False)
                myReturn = False
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Start communications channel after a previous Stop
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StartComm() As Boolean Implements IAnalyzerEntity.StartComm
            Dim myReturn As Boolean = False
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If Not GlobalBase.IsServiceAssembly Then
                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")
                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "") 'AG 20/06/2012
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                If Not myGlobal.HasError Then
                    CommThreadsStartedAttribute = False
                    ' Start communicactions through the Application layer to Link layer
                    myGlobal = AppLayer.StartComm()

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        ' Establish connection as success done
                        myReturn = CType(myGlobal.SetDatos, Boolean)
                        CommThreadsStartedAttribute = myReturn
                        ConnectedAttribute = myReturn
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True)
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StartComm", EventLogEntryType.Error, False)
                myReturn = False
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' Get all available ports in PC
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Creation AG 22/04/2010
        ''' </remarks>
        Public Function ReadRegistredPorts() As GlobalDataTO Implements IAnalyzerEntity.ReadRegistredPorts
            Dim myReturn As New GlobalDataTO
            Try
                myReturn = AppLayer.ReadRegistredPorts  'Pending to develop!!

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadRegistredPorts", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' When a worksession finish clear the DS with the last reagents prepared
        ''' </summary>
        ''' <remarks>AG 19/01/2011</remarks>
        Public Sub ResetWorkSession() Implements IAnalyzerEntity.ResetWorkSession
            Try
                If Not mySentPreparationsDS Is Nothing Then
                    mySentPreparationsDS.Clear()
                End If

                If Not myNextPreparationToSendDS Is Nothing Then
                    myNextPreparationToSendDS.Clear()
                End If

                If Not BaseLine Is Nothing Then
                    BaseLine.ResetWS()
                End If
                WorkSessionIDAttribute = ""
                futureRequestNextWell = 0

                processingLastANSPHRInstructionFlag = False
                bufferANSPHRReceived.Clear()
                recoveredPrepIDList.Clear() 'AG 25/09/2012 

                'AG 28/11/2013 - BT #1397 - Reset several flags
                SetAllowScanInRunningValue(False)
                AppLayer.RecoveryResultsInPause = False
                'AG 28/11/2013

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ResetWorkSession", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Inform the presentation layer has copied the ui refresh ds into a local variable
        ''' </summary>
        ''' <param name="pMainThread"></param>
        ''' <remarks></remarks>
        Public Sub ReadyToClearUIRefreshDS(ByVal pMainThread As Boolean) Implements IAnalyzerEntity.ReadyToClearUIRefreshDS
            Try
                If pMainThread Then
                    eventDataPendingToTriggerFlag = False
                Else
                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'secondaryEventDataPendingToTriggerFlag = False
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadyToClearUIRefreshDS", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Main function in analyzer management class
        ''' 
        ''' Receives an action identifier and depending the analyzer state, alarms,... decide what is the
        ''' next action to be performed
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pSendingEvent"></param>
        ''' <param name="pInstructionReceived">Optional parameter, only informed when Software receives instructions</param>
        ''' <param name="pFwScriptID">Identifier of the script/action to be sended</param>
        ''' <param name="pParams">param values to script/action to be sended</param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Creation AG 16/04/2010
        ''' Modified AG 05/05/2010 (Dont use the INIT instruction in Ax00 physical layer. For get the old code search by ManageAnalyzer05052010)
        '''          AG 18/05/2010 (1: cases Baseline send and receive, 2: add pSwAdditionalParameters parameter)
        '''          XB 08/11/2010 - SERVICE SOFTWARE - Add optional field pScriptID
        '''          XB 18/11/2010 - SERVICE SOFTWARE - Add optional field pParams
        '''          XB 21/10/2013 - ANSCBR must launch the ReceptionEvent to Presentation also when RUNNING - BT #1334
        '''          AG 09/12/2013 - #1397 General protection but specially if partial freeze during recover results (do not add the END instruction to queue if current action is STANDBY_START)
        '''          AG 10/12/2013 - Comment the change required for #1422 (do not sent END while action is START_Ini)
        '''          XB 11/12/2013 - Add the condition required for #1422 (do not send END while action is AnalyzerManagerFlags.StartRunning)
        '''          XB 06/02/2014 - Improve WDOG BARCODE_SCAN - Task #1438
        '''          XB 03/04/2014 - Avoid send ISECMD out of running cycle - task #1573
        '''          AG 15/04/2014 - #1591 do not send START while analyzer is starting pause
        ''' </remarks>
        Public Function ManageAnalyzer(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList, ByVal pSendingEvent As Boolean, _
                                       Optional ByVal pInstructionReceived As List(Of InstructionParameterTO) = Nothing, _
                                       Optional ByVal pSwAdditionalParameters As Object = Nothing, _
                                       Optional ByVal pFwScriptID As String = "", _
                                       Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO Implements IAnalyzerEntity.ManageAnalyzer

            Dim myGlobal As New GlobalDataTO

            Try
                If CommThreadsStartedAttribute Then

                    'AG 20/06/2012 - Ignore all reception instructions (except the STATUS instructions) meanwhile the WaitForAnalyzerReady flag is 'INI'
                    'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                    'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If Not GlobalBase.IsServiceAssembly Then
                        ' User Sw
                        If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                            mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady.ToString) = "INI" AndAlso _
                            Not pSendingEvent AndAlso pAction <> AnalyzerManagerSwActionList.STATUS_RECEIVED Then

                            Return myGlobal
                        End If
                    End If
                    'AG 20/06/2012

                    'AG 26/11/2010 - Empty UI refresh information
                    'myUI_RefreshEvent.Clear()
                    If pAction = GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT OrElse pAction = GlobalEnumerates.AnalyzerManagerSwActionList.RECOVER Then AnalyzerIsReadyAttribute = True

                    'AG 27/01/2012 - Treat other special cases a part from WAITING TIME EXPIRED: SOUND, ENDSOUND
                    If pAction <> GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED AndAlso pAction <> GlobalEnumerates.AnalyzerManagerSwActionList.SOUND _
                        AndAlso pAction <> GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND AndAlso pSendingEvent AndAlso Not AnalyzerIsReadyAttribute Then
                        'AG 27/01/2012

                        'AG 04/08/2011 - New condition: The NEXT_PREP event has not be put in queue!!
                        If pAction <> GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION Then
                            'If analyzer is busy dont send nothing, add in queue
                            Debug.Print(" ADD ACTION TO QUEUE : [" & pAction.ToString & "] count : " & myInstructionsQueue.Count.ToString)
                            myInstructionsQueue.Add(pAction)
                            myParamsQueue.Add(pSwAdditionalParameters) ' and its parameters XBC 24/05/2011
                            'RaiseEvent SendEvent("Action add in queue: " & pAction.ToString & " .Time max: " & CSng(waitingTimer.Interval / 1000)) 'Temporal testings
                        End If

                    Else 'Case: instruction reception or send instruction with analyzer ready

                        'AG 03/11/2010 - When receive instruction raise event before his threatment
                        If Not pSendingEvent Then
                            'Reception
                            InstructionReceivedAttribute = AppLayer.InstructionReceived
                            InstructionSentAttribute = ""

                            If Not eventDataPendingToTriggerFlag Then
                                'myUI_RefreshEvent.Clear()
                                'myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, True) 'AG 22/05/2014 - #1637
                            End If

                            'AG 22/05/2014 - #1637 Clear code. Comment dead code
                            'If Not secondaryEventDataPendingToTriggerFlag Then
                            '    mySecondaryUI_RefreshEvent.Clear()
                            '    mySecondaryUI_RefreshDS.Clear()
                            'End If

                            InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - disable waiting time control if active

                            'AG 27/06/2011 - Several ANSWER instructions must set AnalyzerIsReadyAttribute = True due
                            'the ANSWER instruction indicates the analyzer can perform another task
                            'Other ANS instruction not due the Sw has not ask for then (ansphr, ansise, ansbr1, ansbm1, ansbr2,...)
                            If Not AnalyzerIsReadyAttribute Then
                                Select Case pAction
                                    'AG 26/03/2012 - all reception instructions except status set analyzerisready = True (exception Running)
                                    'Case GlobalEnumerates.AnalyzerManagerSwActionList.BASELINE_RECEIVED, GlobalEnumerates.AnalyzerManagerSwActionList.ANSCBR_RECEIVED, _
                                    '    GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED, GlobalEnumerates.AnalyzerManagerSwActionList.ANSINF_RECEIVED

                                    '    AnalyzerIsReadyAttribute = True

                                    'Case GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED
                                    '    If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                    '        AnalyzerIsReadyAttribute = True
                                    '    End If
                                    'Case Else
                                    Case GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED
                                        'Do nothing ... when the status instruction is treated the Software updates the AnalyzerIsReadyAttribute

                                    Case Else
                                        If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                            AnalyzerIsReadyAttribute = True
                                        End If
                                        'AG 26/03/2012
                                End Select
                            End If
                            'AG 27/06/2011

                            '20/05/2011 AG - Comment the event before treat the instruction due it was for testings
                            'RaiseEvent ReceptionEvent(InstructionReceivedAttribute, False, myUI_RefreshEvent, myUI_RefreshDS)

                        Else
                            'SGM 07/11/2012
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                If MyClass.IsAlarmInfoRequested Then
                                    System.Threading.Thread.Sleep(1000)
                                End If
                            End If
                        End If


                        '' XBC 05/05/2011 - timeout limit repetitions
                        'If myApplicationName.ToUpper.Contains("SERVICE") Then
                        '    If pSendingEvent AndAlso _
                        '       pAction <> GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED Then
                        '        numRepetitionsTimeout = 0
                        '        Me.InitializeTimerControl(WAITING_TIME_DEFAULT)
                        '    End If
                        'End If
                        '' XBC 05/05/2011 - timeout limit repetitions
                        'pAction = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED



                        Select Case pAction
                            '''''''''''''''
                            'SEND EVENTS!!!
                            '''''''''''''''
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT
                                ' Set Waiting Timer Current Instruction OFF
                                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                                    ClearQueueToSend()
                                Else
                                    myGlobal = ProcessConnection(pSwAdditionalParameters, False)
                                End If
                                Exit Select

                                'AG 06/05/2010 - Short instructions (generical)
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SLEEP)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.STANDBY)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.SKIP
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SKIP)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.RUNNING)
                                Exit Select

                                ' XB 10/10/2013 - BT #1317
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE
                                If AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute AndAlso _
                                    AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_START AndAlso Not PauseAlreadySentFlagAttribute Then

                                    'If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And AnalyzerCurrentAction = GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_START Then
                                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                        If Not myInstructionsQueue.Contains(pAction) Then
                                            myInstructionsQueue.Add(pAction)
                                            myParamsQueue.Add(pSwAdditionalParameters)
                                        End If
                                    Else
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.PAUSE)
                                        If Not myGlobal.HasError Then useRequestFlag = False 'AG 02/11/2010
                                    End If
                                End If

                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.START
                                'AG 06/03/20102 - add into queue and send after receive status instruction
                                'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.START)
                                '    If Not myGlobal.HasError Then
                                '        endRunAlreadySentFlagAttribute = False 'AG 13/01/2012
                                '        abortAlreadySentFlagAttribute = False
                                '    End If

                                'AG 26/03/2014 - #1501 (1) add condition (AndAlso Not  ContinueAlreadySentFlagAttribute) - do not add START into queue if analyzer is starting running

                                'AG 15/04/2014 - #1591 - Condition ContinueAlreadySentFlagAttribute using another IF (besides add protection for not send START when analyzer is starting pause)
                                'If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso Not ContinueAlreadySentFlagAttribute Then
                                If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                    If Not ContinueAlreadySentFlagAttribute AndAlso _
                                        (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess.ToString) <> "INPROCESS") AndAlso Not myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE) Then
                                        'AG 15/04/2014 - #1591
                                        If Not myInstructionsQueue.Contains(pAction) Then
                                            myInstructionsQueue.Add(pAction)
                                            myParamsQueue.Add(pSwAdditionalParameters)
                                        End If
                                    End If 'AG 15/04/2014 - #1591

                                Else
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.START)
                                    If Not myGlobal.HasError Then
                                        endRunAlreadySentFlagAttribute = False 'AG 13/01/2012
                                        abortAlreadySentFlagAttribute = False
                                        PauseAlreadySentFlagAttribute = False ' XB 15/10/2013 - BT #1318
                                    End If
                                End If
                                'AG 06/03/2012
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN
                                'AG 08/07/2011 - Special case for sending ENDRUN during RUNNING initialization
                                'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
                                'If Not myGlobal.HasError Then
                                '    useRequestFlag = False 'AG 02/11/2010
                                'End If
                                'Exit Select
                                'AG 09/12/2013 - Add action <> STANDBY_START (for case partial freeze appears during recover results)
                                ' XB 15/10/2013 - Add PauseAlreadySentFlagAttribute - BT #1318
                                If AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.END_RUN_START AndAlso Not endRunAlreadySentFlagAttribute AndAlso _
                                    AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute AndAlso _
                                    AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.PAUSE_START AndAlso Not PauseAlreadySentFlagAttribute AndAlso _
                                    AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_START Then

                                    'If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING And AnalyzerCurrentAction = GlobalEnumerates.AnalyzerManagerAx00Actions.START_INSTRUCTION_START Then
                                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                        'AG 30/10/2013 - in pause mode if SwAdditionalParameter = TRue means EndUser has decided to stop worksession
                                        '1st send START, later END - Task #1342
                                        'If Not myInstructionsQueue.Contains(pAction) Then
                                        '    myInstructionsQueue.Add(pAction)
                                        '    myParamsQueue.Add(pSwAdditionalParameters)
                                        '    'RaiseEvent SendEvent("Action add in queue: " & pAction.ToString & " .Time max: " & CSng(waitingTimer.Interval / 1000)) 'Temporal testings
                                        'End If

                                        If AllowScanInRunningAttribute Then
                                            If Not stopRequestedByUserInPauseModeFlag AndAlso Not pSwAdditionalParameters Is Nothing Then
                                                If TypeOf (pSwAdditionalParameters) Is Boolean Then
                                                    stopRequestedByUserInPauseModeFlag = DirectCast(pSwAdditionalParameters, Boolean)

                                                    'AG + XB 11/12/2013 - inside the brackets add: OrElse mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) = "INI" - Task #1422
                                                    'AG 12/11/2013 - If current action is START_INI / START_END do not send the START instruction again. Add end into queue
                                                    If stopRequestedByUserInPauseModeFlag AndAlso _
                                                       (AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.START_INSTRUCTION_START OrElse _
                                                        AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.STANDBY_END OrElse _
                                                        mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.StartRunning.ToString) = "INI") Then
                                                        stopRequestedByUserInPauseModeFlag = False
                                                    End If
                                                    'AG 12/11/2013

                                                End If
                                            End If
                                        Else
                                            stopRequestedByUserInPauseModeFlag = False
                                        End If

                                        If Not stopRequestedByUserInPauseModeFlag Then 'Code until v2.1.1
                                            If Not myInstructionsQueue.Contains(pAction) Then
                                                myInstructionsQueue.Add(pAction) 'END instruction added to queue
                                                myParamsQueue.Add(Nothing)
                                            End If
                                        Else
                                            Dim myLogAcciones As New ApplicationLogManager()
                                            myLogAcciones.CreateLogActivity("In paused mode user decides stop the Worksession, 1st add START instruction into queue (or recovery results while analyzer is in pause mode)", "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Information, False)
                                            If Not myInstructionsQueue.Contains(GlobalEnumerates.AnalyzerManagerSwActionList.START) Then
                                                myInstructionsQueue.Add(GlobalEnumerates.AnalyzerManagerSwActionList.START)
                                                myParamsQueue.Add(Nothing)
                                            End If
                                        End If
                                        'AG 30/10/2013 - Task #1342

                                    Else
                                        'Death code. The ENDRUN instruction has no sense out of Running (it is here for develop. testings)
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDRUN)
                                        If Not myGlobal.HasError Then useRequestFlag = False 'AG 02/11/2010
                                    End If
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.NROTOR)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ABORT
                                If AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute Then
                                    'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ABORT)
                                    'If Not myGlobal.HasError Then useRequestFlag = False 'AG 02/11/2010
                                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                        If Not myInstructionsQueue.Contains(pAction) Then
                                            myInstructionsQueue.Add(pAction)
                                            myParamsQueue.Add(pSwAdditionalParameters)
                                            'RaiseEvent SendEvent("Action add in queue: " & pAction.ToString & " .Time max: " & CSng(waitingTimer.Interval / 1000)) 'Temporal testings
                                        End If
                                    Else
                                        'AG 08/11/2013 Task #1358 ' XB 15/10/2013 - BT #1318
                                        SetAllowScanInRunningValue(False) 'AllowScanInRunningAttribute = False

                                        ' XB 06/11/2013 - Remove END instructions from the queue when ABORT instruction is performed
                                        RemoveItemFromQueue(AnalyzerManagerSwActionList.ENDRUN)

                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ABORT)
                                        If Not myGlobal.HasError Then useRequestFlag = False 'AG 02/11/2010
                                    End If
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.SOUND
                                'AG 26/01/2012 - In Running the SOUND instruction is sent only after receive a STATUS instruction
                                'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SOUND)
                                If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso Not AnalyzerIsRingingAttribute Then
                                    If Not myInstructionsQueue.Contains(pAction) Then
                                        myInstructionsQueue.Add(pAction)
                                        myParamsQueue.Add(pSwAdditionalParameters)
                                        'RaiseEvent SendEvent("Action add in queue: " & pAction.ToString & " .Time max: " & CSng(waitingTimer.Interval / 1000)) 'Temporal testings
                                    End If
                                Else
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SOUND)
                                End If
                                'AG 26/01/2012
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND
                                'AG 26/01/2012 - In Running the SOUND instruction is sent only after receive a STATUS instruction
                                'AG 03/09/2012 - during conection (running) force sent always endsound, do not use queue
                                'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDSOUND)
                                If (AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso AnalyzerIsRingingAttribute) _
                                AndAlso mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) <> "INPROCESS" Then
                                    If Not myInstructionsQueue.Contains(pAction) Then
                                        myInstructionsQueue.Add(pAction)
                                        myParamsQueue.Add(pSwAdditionalParameters)
                                        'RaiseEvent SendEvent("Action add in queue: " & pAction.ToString & " .Time max: " & CSng(waitingTimer.Interval / 1000)) 'Temporal testings
                                    End If
                                Else
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENDSOUND)
                                End If
                                'AG 26/01/2012
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.RESULTSRECOVER
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.RESRECOVER)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.STATE
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.STATE)
                                Exit Select
                                'END AG 06/05/2010

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.WASH
                                'AG 02/03/2011 - By now implement only the conditioning case. When the wash utility has defined
                                'probably an specific parameter will be required
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WASH)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.NEXT_PREPARATION
                                ''ConnectedAttribute = True 'AG 07/02/2012 - Uncomment this line only for testings
                                'If ConnectedAttribute Then
                                Dim nextWell As Integer = 0
                                If IsNumeric(pSwAdditionalParameters) Then
                                    'myWell = DirectCast(pSwAdditionalParameters, Integer)
                                    nextWell = Integer.Parse(pSwAdditionalParameters.ToString)

                                    'The pauseSendingTestPreparationsFlag becomes TRUE with several alarms appears and becomes FALSE when solved
                                    'AG 29/06/2012 - Add 'AndAlso Not endRunAlreadySentFlagAttribute AndAlso Not abortAlreadySentFlagAttribute'
                                    ' XB 15/10/2013 - Add PauseAlreadySentFlagAttribute - BT #1318
                                    If Not pauseSendingTestPreparationsFlag AndAlso _
                                       Not endRunAlreadySentFlagAttribute AndAlso _
                                       Not abortAlreadySentFlagAttribute AndAlso _
                                       Not PauseAlreadySentFlagAttribute Then
                                        'AG 07/06/2012
                                        'myGlobal = Me.SendNextPreparation(nextWell)
                                        myGlobal = ManageSendAndSearchNext(nextWell)
                                    End If

                                End If
                                'End If
                                Exit Select

                                'AG 18/05/2010
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT
                                If ConnectedAttribute Then
                                    Dim nextWell As Integer = 0
                                    If IsNumeric(pSwAdditionalParameters) Then
                                        nextWell = Integer.Parse(pSwAdditionalParameters.ToString)
                                        myGlobal = Me.SendAdjustLightInstruction(nextWell)
                                    ElseIf Not pParams Is Nothing Then
                                        ' XBC 20/02/2012
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ALIGHT, Nothing, "", "", pParams)
                                    End If
                                End If
                                Exit Select


                                'IT 29/10/2014: BA-2061
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT
                                If ConnectedAttribute Then
                                    If Not pParams Is Nothing Then
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.FLIGHT, Nothing, String.Empty, String.Empty, pParams)
                                    End If
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.INFO

                                If ConnectedAttribute Then
                                    Dim queryMode As Integer = GlobalEnumerates.Ax00InfoInstructionModes.ALR
                                    If IsNumeric(pSwAdditionalParameters) Then
                                        queryMode = CInt(pSwAdditionalParameters)
                                    End If

                                    'SGM 06/11/2012
                                    '' XBC 21/09/2011 - this instruction don't expect answer
                                    'If myApplicationName.ToUpper.Contains("SERVICE") Then
                                    '    If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then
                                    '        ClearQueueToSend()
                                    '    End If
                                    'End If

                                    'AG 14/03/2012
                                    'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.INFO, queryMode)
                                    Dim allowToSendFlag As Boolean = True

                                    'AG 21/03/2012 - the following code try to optimize the INFO str or stp sending instructions depending
                                    'if sw has activated or deactivated it ... problem new Fw versions deactivates ansinf automatically so
                                    'this code not works ok always
                                    'If AnalyzerIsInfoActivatedAttribute = 1 AndAlso queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STR Then allowToSendFlag = False
                                    'If AnalyzerIsInfoActivatedAttribute = 0 AndAlso queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then allowToSendFlag = False

                                    'SGM 24/10/2012 - Not to allow send INFO Q:3 in case of Alarm details requested
                                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                    If GlobalBase.IsServiceAssembly Then

                                        'SGM 06/11/2012 - remove if the current action is alredy in queue
                                        Dim myInstructionsQueueTemp As New List(Of GlobalEnumerates.AnalyzerManagerSwActionList)
                                        Dim myParamsQueueTemp As New List(Of Object)

                                        For Each A As GlobalEnumerates.AnalyzerManagerSwActionList In myInstructionsQueue
                                            Dim j As Integer = myInstructionsQueue.IndexOf(A)
                                            myInstructionsQueueTemp.Add(A)
                                            myParamsQueueTemp.Add(myParamsQueue(j))
                                        Next
                                        For Each A As GlobalEnumerates.AnalyzerManagerSwActionList In myInstructionsQueueTemp
                                            If A = GlobalEnumerates.AnalyzerManagerSwActionList.INFO Then
                                                Dim myInfoType As GlobalEnumerates.Ax00InfoInstructionModes = CType(queryMode, GlobalEnumerates.Ax00InfoInstructionModes)
                                                Dim j As Integer = myInstructionsQueueTemp.IndexOf(A)
                                                If CType(myParamsQueueTemp(j), GlobalEnumerates.Ax00InfoInstructionModes) = myInfoType Then
                                                    myInstructionsQueue.RemoveAt(j)
                                                    myParamsQueue.RemoveAt(j)
                                                End If
                                            End If
                                        Next

                                        If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STR Or queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then
                                            allowToSendFlag = Not MyClass.IsAlarmInfoRequested 'SGM 26/10/2012
                                            If Not allowToSendFlag Then
                                                myInstructionsQueue.Add(AnalyzerManagerSwActionList.INFO)
                                                myParamsQueue.Add(CInt(pSwAdditionalParameters)) 'AG 11/12/2012 - add cint to querymode
                                            End If

                                        End If

                                    End If

                                    If allowToSendFlag Then

                                        ' SGM 06/11/2012 - this instruction don't expect answer
                                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                        If GlobalBase.IsServiceAssembly Then
                                            If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then
                                                ClearQueueToSend()
                                            End If
                                        End If

                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.INFO, CInt(queryMode)) 'AG 11/12/2012 - add cint to querymode

                                        'SGM 07/11/2012
                                        If Not myGlobal.HasError Then

                                            If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STR Then
                                                AnalyzerIsInfoActivatedAttribute = 1
                                                MyClass.IsAlarmInfoRequested = False
                                            End If

                                            If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then
                                                AnalyzerIsInfoActivatedAttribute = 0
                                                MyClass.IsAlarmInfoRequested = False
                                            End If

                                            If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.ALR Then
                                                AnalyzerIsInfoActivatedAttribute = 0
                                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                                If GlobalBase.IsServiceAssembly Then
                                                    MyClass.IsAlarmInfoRequested = True
                                                End If
                                            End If

                                        Else
                                            MyClass.IsAlarmInfoRequested = False
                                        End If


                                        'If Not myGlobal.HasError Then
                                        '    If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STR Then AnalyzerIsInfoActivatedAttribute = 1
                                        '    If queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then AnalyzerIsInfoActivatedAttribute = 0
                                        'End If

                                        'end SGM 07/11/2012

                                    End If


                                    'AG 14/03/2012

                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL
                                If ConnectedAttribute Then
                                    Dim queryMode As Integer = GlobalEnumerates.Ax00WashStationControlModes.UP
                                    If IsNumeric(pSwAdditionalParameters) Then
                                        queryMode = CInt(pSwAdditionalParameters)
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WSCTRL, queryMode)
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST
                                If ConnectedAttribute Then
                                    'AG 16/10/2013 - In Running add the barcoderequest and the BarcodeRequestDS into queue, the instruction will be sent after STATUS reception
                                    'myBarcodeRequestDS = CType(pSwAdditionalParameters, AnalyzerManagerDS)
                                    'myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)

                                    If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                        If (AnalyzerCurrentActionAttribute <> GlobalEnumerates.AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute) _
                                            AndAlso (AllowScanInRunning) Then
                                            If Not myInstructionsQueue.Contains(pAction) Then
                                                myInstructionsQueue.Add(pAction)
                                                myParamsQueue.Add(pSwAdditionalParameters)

                                                ' XB 29/01/2014 - Deactivate WatchDog timer - Task #1438
                                                RaiseEvent WatchDogEvent(False)
                                            End If
                                        End If
                                    Else
                                        myBarcodeRequestDS = CType(pSwAdditionalParameters, AnalyzerManagerDS)
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)

                                        ' XB 29/01/2014 - Task #1438
                                        MyClass.BarcodeStartInstrExpected = True
                                    End If
                                    'AG 16/10/2013

                                    Exit Select
                                End If
                                Exit Select

                                'AG 23/11/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG
                                If ConnectedAttribute Then
                                    Dim anSetts As New AnalyzerSettingsDelegate
                                    myGlobal = anSetts.ReadAll(Nothing, AnalyzerIDAttribute)
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        Dim mySettingsDS As New AnalyzerSettingsDS
                                        mySettingsDS = CType(myGlobal.SetDatos, AnalyzerSettingsDS)
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.CONFIG, mySettingsDS)
                                    End If
                                End If
                                Exit Select
                                'AG 23/11/2011


                                ' XBC 03/05/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.READADJ
                                If ConnectedAttribute Then
                                    Dim queryMode As String = ""
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = pSwAdditionalParameters.ToString
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.READADJ, queryMode)
                                End If
                                Exit Select


                                ' SGM 12/12/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD
                                If ConnectedAttribute Then
                                    Dim myISECommand As ISECommandTO
                                    myISECommand = CType(pSwAdditionalParameters, ISECommandTO)
                                    If myISECommand.ISEMode <> GlobalEnumerates.ISEModes.None Then


                                        ' XB 03/04/2014 - Avoid send ISECMD out of running cycle - task #1573
                                        If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                            If Not myInstructionsQueue.Contains(pAction) Then
                                                myInstructionsQueue.Add(pAction)
                                                myParamsQueue.Add(pSwAdditionalParameters)
                                                Debug.Print("ISECMD TO THE QUEUE !!! [" & pAction.ToString & "] [" & pSwAdditionalParameters.ToString & "]")
                                            End If
                                        Else
                                            ' XB 03/04/2014 

                                            myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ISE_CMD, myISECommand)

                                            ' XBC 05/09/2012 - Start Timeout for ISE commands must emplaced inside ManageAnalyzer
                                            If Not myGlobal.HasError Then
                                                myGlobal = ISEAnalyzer.StartInstructionStartedTimer
                                            End If
                                            ' XBC 05/09/2012 

                                        End If ' XB 03/04/2014 


                                    End If
                                End If
                                Exit Select

                                ' SGM 12/12/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.FW_UTIL
                                If ConnectedAttribute Then
                                    Dim myFWAction As FWUpdateRequestTO
                                    myFWAction = CType(pSwAdditionalParameters, FWUpdateRequestTO)
                                    If myFWAction.ActionType <> GlobalEnumerates.FwUpdateActions.None Then
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.FW_UTIL, myFWAction)
                                    End If
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.RECOVER 'AG 22/02/2012
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    SensorValuesAttribute.Clear() 'SGM 08/11/2012
                                End If
                                ClearQueueToSend() 'Before sent recover instruction clear the instructions in queue to be sent
                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.RECOVER)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.POLLRD  'AG 31/07/2012
                                If ConnectedAttribute Then
                                    Dim ActionMode As Integer = GlobalEnumerates.Ax00PollRDAction.Biochemical
                                    If IsNumeric(pSwAdditionalParameters) Then
                                        ActionMode = CInt(pSwAdditionalParameters)
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.POLLRD, ActionMode)
                                End If
                                Exit Select

                                '
                                ' SERVICE SOFTWARE----------------------------------------------------------------------------------------------------------
                                ' 

                                ' XBC 08/11/2010  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND


                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.COMMAND, pSwAdditionalParameters, "", pFwScriptID, pParams)

                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                                    If Not myGlobal.HasError Then
                                        If Not MyClass.sendingRepetitions Then
                                            MyClass.numRepetitionsTimeout = 0
                                        End If
                                        MyClass.InitializeTimerStartTaskControl(WAITING_TIME_DEFAULT)
                                        MyClass.StoreStartTaskinQueue(pAction, pSwAdditionalParameters, pFwScriptID, pParams)
                                    End If
                                    ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                                End If

                                Exit Select


                                ' XBC 03/05/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ
                                If ConnectedAttribute Then
                                    Dim queryMode As String = ""
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = pSwAdditionalParameters.ToString
                                        'queryMode = "{Brazo Muestra}" & vbCrLf & _
                                        '"M1PI:4400;   {REF Punto Inicial Detección de nivel - Vertical}" & vbCrLf & _
                                        '"M1RPI:4000;  {REF Punto Inicial Detección de nivel - Vertical - En Rotor de Reacciones}" '& vbCrLf & _

                                        '"CLOT:0;      {Detección de clot desactivada}" & vbCrLf & _
                                        '"M1DH1:16635; {Dispensación en Rotor Posicion 1 - Horizontal}" & vbCrLf & _
                                        '"M1DH2:16188; {Dispensación en Rotor Posicion 2 - Horizontal}" & vbCrLf & _
                                        '"M1DV1:7474;  {REF Dispensación en Rotor Posicion 1 - Vertical}" & vbCrLf & _
                                        '"M1DV2:8050;  {REF Dispensación en Rotor Posicion 2 - Vertical}" & vbCrLf & _
                                        '"M1H:1763;    {Posición Parking - Horizontal}" & vbCrLf & _
                                        '"M1ISEH:-60;  {Dispensación en ISE - Horizontal}" & vbCrLf & _
                                        '"M1ISEV:16000;{Dispensación en ISE - Vertical}" & vbCrLf & _
                                        '"M1PH1:10632; {Rotor de Muestras Corona 1 - Horizontal}" & vbCrLf & _
                                        '"M1PH2:9970;  {Rotor de Muestras Corona 2 - Horizontal}" & vbCrLf & _
                                        '"M1PH3:8710;  {Rotor de Muestras Corona 3 - Horizontal}" & vbCrLf & _
                                        '"M1PI:4400;   {REF Punto Inicial Detección de nivel - Vertical}" & vbCrLf & _
                                        '"M1PV1p:16300;{Rotor de Muestras Corona 1 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                                        '"M1PV1t:16400;{Rotor de Muestras Corona 1 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                                        '"M1PV2p:16200;{Rotor de Muestras Corona 2 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                                        '"M1PV2t:16400;{Rotor de Muestras Corona 2 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                                        '"M1PV3p:16200;{Rotor de Muestras Corona 3 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                                        '"M1PV3t:16400;{Rotor de Muestras Corona 3 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                                        '"M1RPI:4000;  {REF Punto Inicial Detección de nivel - Vertical - En Rotor de Reacciones}" & vbCrLf & _
                                        '"M1RV:200;    {Posicion referencia - Vertical}" & vbCrLf & _
                                        '"M1SV:580;    {Posición Segura para movimiento Horizontal - Vertical}" & vbCrLf & _
                                        '"M1V:16332;   {REF Posición Parking - Vertical}" & vbCrLf & _
                                        '"M1WH:12620;  {Estación de Lavado - Horizontal}" & vbCrLf & _
                                        '"M1WVR:4250;  {Estación de Lavado - Vertical Relativo a R1SV}"

                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.LOADADJ, queryMode)
                                End If
                                Exit Select

                                ' XBC 20/04/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_BLIGHT
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.BLIGHT, Nothing, "", "", pParams)
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.TANKS_TEST
                                If ConnectedAttribute Then
                                    Dim queryMode As String = ""
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = pSwAdditionalParameters.ToString
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.TANKSTEST, queryMode)
                                End If
                                Exit Select


                                ' XBC 23/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.SDMODE
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SDMODE, Nothing, "", "", pParams)
                                End If
                                Exit Select

                                ' XBC 23/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.SDPOLL
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.SDPOLL)
                                End If
                                Exit Select

                                'XBC 25/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.POLLFW
                                If ConnectedAttribute Then
                                    Dim queryMode As GlobalEnumerates.POLL_IDs
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = CType(pSwAdditionalParameters, GlobalEnumerates.POLL_IDs)
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.POLLFW, queryMode)
                                End If
                                Exit Select

                                'XBC 31/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.POLLHW
                                If ConnectedAttribute Then
                                    Dim queryMode As GlobalEnumerates.POLL_IDs
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = CType(pSwAdditionalParameters, GlobalEnumerates.POLL_IDs)
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.POLLHW, queryMode)
                                End If
                                Exit Select

                                '    'SGM 10/06/2011
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.ENABLE_FW_EVENTS
                                '    If ConnectedAttribute Then
                                '        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.ENABLE_EVENTS, Nothing)
                                '    End If
                                '    Exit Select

                                '    'SGM 10/06/2011
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.DISABLE_FW_EVENTS
                                '    If ConnectedAttribute Then
                                '        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.DISABLE_EVENTS, Nothing)
                                '    End If
                                '    Exit Select

                                ' SGM 01/07/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.RESET_ANALYZER
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.RESET_ANALYZER, Nothing)
                                End If
                                Exit Select

                                ' SGM 01/07/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.LOADFACTORYADJ
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.LOADFACTORYADJ, Nothing)
                                End If
                                Exit Select

                                ' SGM 05/07/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.UPDATEFW
                                If ConnectedAttribute Then
                                    Dim queryMode() As Byte = Nothing
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = CType(pSwAdditionalParameters, Byte())
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.UPDATE_FIRMWARE, queryMode)
                                End If
                                Exit Select

                                ' SGM 27/07/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.READCYCLES
                                If ConnectedAttribute Then
                                    Dim queryMode As String = ""
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = pSwAdditionalParameters.ToString
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.READCYC, queryMode)
                                End If
                                Exit Select

                                'SGM 27/07/2011  
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.WRITECYCLES
                                If ConnectedAttribute Then
                                    Dim queryMode As String = ""
                                    If Not pSwAdditionalParameters Is Nothing Then
                                        queryMode = pSwAdditionalParameters.ToString
                                    End If
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.WRITECYC, queryMode)
                                End If
                                Exit Select


                                ' XBC 04/06/2012
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.UTIL
                                If ConnectedAttribute Then
                                    Dim myUtilCommand As UTILCommandTO
                                    myUtilCommand = CType(pSwAdditionalParameters, UTILCommandTO)
                                    If myUtilCommand.ActionType <> GlobalEnumerates.UTILInstructionTypes.None Then
                                        myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.UTIL, myUtilCommand)
                                    End If
                                End If
                                Exit Select


                                ' XBC 30/07/2012
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.POLLSN
                                If ConnectedAttribute Then
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.POLLSN)
                                    runningConnectionPollSnSent = True
                                End If
                                Exit Select

                                '
                                ' SERVICE SOFTWARE END------------------------------------------------------------------------------------------------------
                                ' 

                                ''''''''''''''''''''''''''''''''''''''''
                                'TIME OUT: HOW ARE YOU? <SEND EVENTS!!!>
                                ''''''''''''''''''''''''''''''''''''''''
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED

                                'If myApplicationName.ToUpper.Contains("SERVICE") Then
                                '    ' XBC 05/05/2011 - timeout limit repetitions
                                '    numRepetitionsTimeout += 1
                                '    Me.InitializeTimerControl(WAITING_TIME_DEFAULT)
                                '    If numRepetitionsTimeout > GlobalBase.MaxRepetitionsTimeout Then
                                '        waitingTimer.Enabled = False

                                '        ConnectedAttribute = False 'SGM 20/09/2011
                                '        InfoRefreshFirstTime = True
                                '        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(ConnectedAttribute), True) 'AG 23/09/2011

                                '        ' Set Waiting Timer Current Instruction OFF
                                '        ClearQueueToSend()
                                '        RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                '        Exit Try
                                '    End If
                                '    ' XBC 05/05/2011 - timeout limit repetitions
                                'End If

                                ' XBC 11/05/2012
                                RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                ' XBC 11/05/2012

                                If ConnectedAttribute Then ' XBC 07/09/2012 - correction : Sw can't send instructions when is no connected
                                    myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.STATE)
                                End If
                                Exit Select

                                ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.START_TASK_TIMEOUT

                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    ' Set Waiting Timer Current Instruction OFF
                                    'Me.InitializeTimerStartTaskControl(WAITING_TIME_DEFAULT)
                                    MyClass.sendingRepetitions = True
                                    ClearQueueToSend()
                                    MyClass.numRepetitionsTimeout += 1
                                    If MyClass.numRepetitionsTimeout > GlobalBase.MaxRepetitionsTimeout Then
                                        waitingStartTaskTimer.Enabled = False
                                        MyClass.sendingRepetitions = False

                                        ConnectedAttribute = False
                                        InfoRefreshFirstTime = True
                                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, CSng(ConnectedAttribute), True)

                                        RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                        Exit Try

                                    Else
                                        ' Instruction has not started by Fw, so is need to send it again
                                        'Debug.Print("Repeat Instruction [" & MyClass.numRepetitionsTimeout.ToString & "]")
                                        Dim myLogAcciones As New ApplicationLogManager()
                                        myLogAcciones.CreateLogActivity("Repeat Start Task Instruction [" & MyClass.numRepetitionsTimeout.ToString & "]", "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Error, False)
                                        myGlobal = MyClass.SendStartTaskinQueue()
                                    End If
                                End If

                                Exit Select
                                ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks


                                ''''''''''''''''''''
                                'RECEPTION EVENTS!!!
                                ''''''''''''''''''''
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED
                                myGlobal = Me.ProcessStatusReceived(pInstructionReceived)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED
                                myGlobal = Me.ProcessHwAlarmDetailsReceived(pInstructionReceived)

                                'SGM 26/10/2012 - force Auto ANSINF
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    MyClass.IsAlarmInfoRequested = False
                                    If MyClass.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)
                                    End If
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSINF_RECEIVED
                                If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString), "INPROCESS", False) <> 0 Then
                                    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSINF_RECEIVED
                                    myGlobal = Me.ProcessInformationStatusReceived(pInstructionReceived)
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.READINGS_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.READINGS_RECEIVED
                                'AG 27/08/2012 - readings are received in running or when recovery results process is working
                                'myGlobal = Me.ProcessReadingsReceived(pInstructionReceived)
                                If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                                    myGlobal = Me.ProcessReadingsReceived(pInstructionReceived)
                                Else
                                    bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
                                End If
                                Exit Select

                                'AG 18/05/2010 
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.BASELINE_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.BASELINE_RECEIVED
                                ' XBC 02/03/2011
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                If GlobalBase.IsServiceAssembly Then
                                    myGlobal = Me.ProcessBaseLineReceived_SRV(pInstructionReceived)
                                Else
                                    myGlobal = Me.ProcessBaseLineReceived(pInstructionReceived)
                                End If
                                ' XBC 02/03/2011
                                Exit Select
                                'END AG 18/05/2010 

                                'AG 29/10/2014 BA-2062 - Dynamic base line results
                            Case AnalyzerManagerSwActionList.ANSFBLD_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFBLD_RECEIVED
                                If GlobalBase.IsServiceAssembly Then

                                Else

                                End If
                                'AG 29/10/2014 BA-2062


                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSCBR_RECEIVED
                                Dim myBarCodeRotorTypeRead As String = ""
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSCBR_RECEIVED
                                myGlobal = Me.ProcessCodeBarInstructionReceived(pInstructionReceived, myBarCodeRotorTypeRead)

                                If Not myGlobal.HasError Then

                                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                                    If GlobalBase.IsServiceAssembly Then
                                        'TODO

                                    Else
                                        If BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.NO_RUNNING_REQUEST Then 'Barcode read requests from RotorPosition Screen
                                            'AG 03/08/2011 - When several individual position to read ... Sw has to manage the send/receive results one by one
                                            If myBarcodeRequestDS.barCodeRequests.Rows.Count > 1 Then
                                                myBarcodeRequestDS.barCodeRequests(0).Delete() 'Remove the 1st row (already sent and with results treated)
                                                myBarcodeRequestDS.AcceptChanges()
                                                myGlobal = AppLayer.ActivateProtocol(GlobalEnumerates.AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)
                                            Else
                                                myBarcodeRequestDS.Clear()
                                                BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.BARCODE_AVAILABLE 'Barcode free for more work

                                                'When barcode finish reading the sample rotor ... Sw inform if some critical warnings exists
                                                If myBarCodeRotorTypeRead = "SAMPLES" Then
                                                    Dim dlgBarcode As New BarcodeWSDelegate
                                                    myGlobal = dlgBarcode.ExistBarcodeCriticalWarnings(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "SAMPLES")
                                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                        If CType(myGlobal.SetDatos, Boolean) = True Then 'No critical errors ... Sw can send the go to running instruction
                                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS, 1, True) 'Samples barcode warnings
                                                        End If
                                                    End If

                                                End If
                                            End If
                                            'AG 03/08/2011

                                        ElseIf BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActionsEnum.ENTER_RUNNING Then 'Barcode read requests from START or CONTINUE worksession
                                            myGlobal = ManageBarCodeRequestBeforeRUNNING(Nothing, BarCodeBeforeRunningProcessStatusAttribute)

                                        End If
                                    End If
                                End If
                                Exit Select

                                'TR 03/01/2010 -ISE RESULTS
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED
                                'ISEModuleIsReadyAttribute = True 'AG 27/10/2011 This information is sent by Analzyer (AG 18/01/2011 - ISE module becomes available again)

                                'AG 27/08/2012 - ISE results are received in running or when recovery results process is working
                                'If ISEAnalyzer.CurrentProcedure <> ISEManager.ISEProcedures.None OrElse AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                '    myGlobal = Me.ProcessRecivedISEResult(pInstructionReceived)
                                '    'SGM 07/03/2012
                                '    myGlobal = MyClass.ProcessISEManagerProcedures
                                'End If

                                ' XB 09/01/2014 - Complete a new case for Recover Results Process - Task #1449
                                ' If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS" Then
                                If (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) <> "INPROCESS") Or _
                                   (mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso _
                                    mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ResRecoverISE.ToString) = "END") Then
                                    ' XB 09/01/2014

                                    If ISEAnalyzer.CurrentProcedure <> ISEAnalyzerEntity.ISEProcedures.None OrElse AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                        myGlobal = Me.ProcessRecivedISEResult(pInstructionReceived)
                                        'SGM 07/03/2012
                                        myGlobal = MyClass.ProcessISEManagerProcedures
                                    End If
                                Else
                                    bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
                                End If
                                'AG 27/08/2012

                                Exit Select


                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED
                                myGlobal = Me.ProcessArmStatusRecived(pInstructionReceived)
                                Exit Select

                                ' XBC 30/07/2012 - serial number in running
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSPSN_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSPSN_RECEIVED
                                myGlobal = Me.ProcessSerialNumberReceived(pInstructionReceived)
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSTIN_RECEIVED 'AG 31/07/2012
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSTIN_RECEIVED
                                'AG 27/08/2012 - readings are received in running or when recovery results process is working
                                If String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString), "INPROCESS", False) <> 0 Then
                                    myGlobal = Me.ProcessANSTINInstructionReceived(pInstructionReceived)
                                Else
                                    bufferInstructionsRESULTSRECOVERYProcess.Add(pInstructionReceived)
                                End If
                                Exit Select

                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSPRD_RECEIVED  'AG 31/07/2012
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSPRD_RECEIVED
                                myGlobal = Me.ProcessANSPRDReceived(pInstructionReceived)
                                Exit Select


                                '
                                ' SERVICE SOFTWARE
                                ' 

                                ' XBC 16/11/2010 
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.COMMAND_RECEIVED
                                myGlobal = Me.ProcessFwCommandAnswerReceived(pInstructionReceived)

                                Exit Select

                                ' XBC 16/11/2010 
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
                                myGlobal = Me.ProcessFwAdjustmentsReceived(pInstructionReceived)

                                Exit Select

                                ' SGM 27/07/2011 
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.CYCLES_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.CYCLES_RECEIVED
                                myGlobal = Me.PDT_ProcessHwCyclesReceived(pInstructionReceived)

                                Exit Select

                                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                                '    'SGM 6/03/11
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.SENSORS_RECEIVED
                                '    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.SENSORS_RECEIVED
                                '    myGlobal = Me.ProcessSensorsDataReceived(pInstructionReceived)
                                '    Exit Select

                                '    'SGM 14/03/11
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.ABSORBANCESCAN_RECEIVED
                                '    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ABSORBANCESCAN_RECEIVED
                                '    myGlobal = Me.ProcessAbsorbanceScanReceived(pInstructionReceived)
                                '    Exit Select
                                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

                                ' XBC 22/03/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSSDM
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSSDM
                                myGlobal = Me.ProcessStressModeReceived_SRV(pInstructionReceived)
                                Exit Select

                                'XBC 08/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSCPU_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSCPU_RECEIVED
                                myGlobal = Me.ProcessHwCpuStatusReceived(pInstructionReceived)
                                Exit Select

                                'SGM 24/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSJEX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSJEX_RECEIVED
                                myGlobal = Me.ProcessHwManifoldStatusReceived(pInstructionReceived)
                                Exit Select

                                'SGM 24/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSSFX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSSFX_RECEIVED
                                myGlobal = Me.ProcessHwFluidicsStatusReceived(pInstructionReceived)
                                Exit Select

                                'SGM 24/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSGLF_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSGLF_RECEIVED
                                myGlobal = Me.ProcessHwPhotometricsStatusReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFCP_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFCP_RECEIVED
                                myGlobal = Me.ProcessFwCPUDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFBX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFBX_RECEIVED
                                myGlobal = Me.ProcessFwARMDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFDX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFDX_RECEIVED
                                myGlobal = Me.ProcessFwPROBEDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFRX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFRX_RECEIVED
                                myGlobal = Me.ProcessFwROTORDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFGL_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFGL_RECEIVED
                                myGlobal = Me.ProcessFwPHOTOMETRICDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFJE_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFJE_RECEIVED
                                myGlobal = Me.ProcessFwMANIFOLDDetailsReceived(pInstructionReceived)
                                Exit Select

                                ' XBC 02/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFSF_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFSF_RECEIVED
                                myGlobal = Me.ProcessFwFLUIDICSDetailsReceived(pInstructionReceived)
                                Exit Select

                                'XBC 31/05/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSBXX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSBXX_RECEIVED
                                myGlobal = Me.ProcessHwARMDetailsReceived(pInstructionReceived)
                                Exit Select

                                'XBC 01/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSDXX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSDXX_RECEIVED
                                myGlobal = Me.ProcessHwPROBEDetailsReceived(pInstructionReceived)
                                Exit Select

                                'XBC 01/06/2011
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSRXX_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSRXX_RECEIVED
                                myGlobal = Me.ProcessHwROTORDetailsReceived(pInstructionReceived)
                                Exit Select

                                'SGM 04/10/2012 -ANSUTIL RECEIVED
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSUTIL_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSUTIL_RECEIVED
                                myGlobal = Me.ProcessANSUTILReceived(pInstructionReceived)
                                Exit Select

                                'SGM 25/05/2012 -ANSFWU RECEIVED
                            Case GlobalEnumerates.AnalyzerManagerSwActionList.ANSFWU_RECEIVED
                                InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSFWU_RECEIVED
                                myGlobal = Me.ProcessFirmwareUtilReceived(pInstructionReceived)
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    MyClass.FWUpdateResponseData = CType(myGlobal.SetDatos, FWUpdateResponseTO)
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.FW_UPDATE_UTIL_RECEIVED, 1, True)
                                End If
                                Exit Select
                                'end SGM 25/05/2012

                                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                                '    'SGM 27/03/11
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTEMPTYLC_OK
                                '    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTEMPTYLC_OK
                                '    myGlobal = ProcessFwCommandAnswerReceived(pInstructionReceived)
                                '    Exit Select

                                '    'SGM 27/03/11
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTFILLDW_OK
                                '    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTFILLDW_OK
                                '    myGlobal = Me.ProcessFwCommandAnswerReceived(pInstructionReceived)
                                '    Exit Select

                                '    'SGM 27/03/11
                                'Case GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTTRANSFER_OK
                                '    InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.TANKTESTTRANSFER_OK
                                '    myGlobal = Me.ProcessFwCommandAnswerReceived(pInstructionReceived)
                                '    Exit Select
                                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent

                                '
                                ' SERVICE SOFTWARE END
                                ' 

                            Case Else
                                myGlobal.HasError = True
                                myGlobal.ErrorCode = "NOT_CASE_FOUND"
                                Exit Select
                        End Select

                        ''SGM 26/09/2011 update Connected attribute
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then

                            ' XBC 24/10/2011
                            'If InstructionTypeReceivedAttribute <> GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED And InstructionTypeReceivedAttribute <> GlobalEnumerates.AnalyzerManagerSwActionList.NONE Then
                            '    ConnectedAttribute = True
                            '    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 1, True) 'Prepare UI refresh event when Connected changes
                            '    '    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS) 'Esto No!!!
                            'End If

                            If pSendingEvent Then
                                If myGlobal.HasError OrElse Not ConnectedAttribute Then
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                                    MyClass.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.NONE
                                    ' Set Waiting Timer Current Instruction OFF
                                    ClearQueueToSend()
                                    RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                End If
                            Else
                                If InstructionTypeReceivedAttribute <> GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED And InstructionTypeReceivedAttribute <> GlobalEnumerates.AnalyzerManagerSwActionList.NONE Then
                                    If MyClass.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then 'SGM 14/11/2011
                                        If Not ConnectedAttribute Then
                                            ConnectedAttribute = True
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 1, True) 'Prepare UI refresh event when Connected changes
                                        End If
                                    End If
                                End If
                            End If
                            ' XBC 24/10/2011

                        Else 'User TimeOut treatment
                            'AG 14/10/2011 -check if instruction has been successfully sent
                            If pSendingEvent Then
                                If myGlobal.HasError OrElse Not ConnectedAttribute Then
                                    UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                                End If
                            End If
                            'AG 14/10/2011
                        End If
                        ''End SGM 26/09/2011

                        If pSendingEvent Then
                            'AG 27/03/2012
                            'InstructionSentAttribute = AppLayer.InstructionSent
                            'InstructionReceivedAttribute = ""
                            If ConnectedAttribute Then
                                InstructionSentAttribute = AppLayer.InstructionSent
                                InstructionReceivedAttribute = ""
                            End If
                            'AG 27/03/2012

                            Dim addText As String = ""
                            If pAction = GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED Then
                                addText = "Time expired!!. "
                            End If

                            ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If GlobalBase.IsServiceAssembly Then
                                If pAction = GlobalEnumerates.AnalyzerManagerSwActionList.START_TASK_TIMEOUT Then
                                    addText = GlobalEnumerates.AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString & ". (" & numRepetitionsTimeout.ToString & ") "
                                End If

                                RaiseEvent SendEvent(addText & InstructionSentAttribute) 'AG 22/02/2012 moved only for service 
                            End If
                            ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks

                            'RaiseEvent SendEvent(addText & InstructionSentAttribute) 'AG 22/02/2012 moved only for service 

                            'Some SENDinstructions also generate refresh ... for instance CONNECT instruction
                            'or when no connection is detected
                            If myUI_RefreshEvent.Count = 0 Then
                                'myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637
                            Else
                                'AG 14/10/2011
                                'Select Case pAction
                                '    Case GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT
                                '        RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS)
                                '        eventDataPendingToTriggerFlag = False 'AG 07/10/2011 - inform not exists information in UI_RefreshDS to be send to the event
                                'End Select
                                If (Not ConnectedAttribute OrElse pAction = GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT) AndAlso eventDataPendingToTriggerFlag Then
                                    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                                    'eventDataPendingToTriggerFlag = False 'AG 07/10/2011 - inform not exists information in UI_RefreshDS to be send to the event
                                End If
                                'AG 14/10/2011
                            End If

                        Else '(CASE ELSE ... instruction reception)
                            InstructionReceivedAttribute = AppLayer.InstructionReceived
                            InstructionSentAttribute = ""

                            'AG 22/02/2012 - minimize the resfresh number in running
                            'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                            'RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                            ''eventDataPendingToTriggerFlag = False 'AG 07/10/2011 - inform not exists information in UI_RefreshDS to be send to the event


                            ' XB 21/10/2013 - ANSCBR must launch the ReceptionEvent to Presentation also when RUNNING - BT #1334
                            Dim RefreshPresentation As Boolean = False
                            If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                                RefreshPresentation = True
                            ElseIf pAction = AnalyzerManagerSwActionList.ANSCBR_RECEIVED Then
                                RefreshPresentation = True

                                ' XB 23/10/2013 - ISE Results must launch the ReceptionEvent to Presentation also when RUNNING - BT #1343
                            ElseIf pAction = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then
                                RefreshPresentation = True



                                ' XB 30/01/2014 - Refresh Presentation with SCAN Barcode answer instruction - Task #1438
                            ElseIf AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                                AnalyzerCurrentActionAttribute = GlobalEnumerates.AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED Then
                                RefreshPresentation = True
                                ' XB 30/01/2014 
                            End If

                            'not in RUNNING every instruction received raises event to presentation
                            ' If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                            If RefreshPresentation Then
                                ' XB 21/10/2013

                                'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637
                                RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)

                                'AG 12/06/2012 - comment this part. Now the running refresh event is unique and called from wellBaseLineWorker_DoWork
                                'normal case in RUNNING: only photometric readings instruction received raises event to presentation
                                'ElseIf InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.READINGS_RECEIVED Then
                                '    If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                                '    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)

                                'AG 09/03/2012 - special case in RUNNING: if TOTAL or RESET freeze then every instruction received raises event to presentation
                            ElseIf analyzerFREEZEFlagAttribute _
                              AndAlso (String.Equals(analyzerFREEZEModeAttribute, "TOTAL") _
                              OrElse String.Equals(analyzerFREEZEModeAttribute, "RESET")) _
                              AndAlso AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then

                                'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637

                                RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)


                                'TR 03/08/2012 -Validate if the key value is on the dictionary before searching inside
                                ' XBC 02/08/2012 - when Connecting on RUNNING WS andalso Recovery Results is in process 
                                'then every instruction received raises event to presentation
                            ElseIf String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString), "INPROCESS", False) = 0 _
                                   AndAlso AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                                   AndAlso (InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED OrElse InstructionTypeReceivedAttribute = GlobalEnumerates.AnalyzerManagerSwActionList.ANSPSN_RECEIVED) Then

                                'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                                ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637
                                RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)

                            End If
                            'AG 22/02/2012


                            'If instruction received and there are instruction in queue: get & send the next one
                            'AG 27/01/2012 - Add condition AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING
                            'AG 17/04/2012 - Remove from IF the following condition "...AndAlso Not analyzerFREEZEFlagAttribute ..."
                            'XB 09/12/2013 - Add condition  mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess.ToString) <> "INPROCESS" - Task #1429
                            'Debug.Print("RUNNINGprocess [" & mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess.ToString) & "]")
                            If AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                                AnalyzerIsReadyAttribute AndAlso myInstructionsQueue.Count > 0 AndAlso _
                                mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess.ToString) <> "INPROCESS" Then
                                Dim queuedAction As New GlobalEnumerates.AnalyzerManagerSwActionList
                                Dim queuedParam As Object
                                queuedAction = Me.GetFirstFromQueue
                                queuedParam = Me.GetFirstParametersFromQueue
                                If queuedAction <> GlobalEnumerates.AnalyzerManagerSwActionList.NONE Then
                                    If queuedParam Is Nothing Then
                                        myGlobal = ManageAnalyzer(queuedAction, True)
                                    Else
                                        'Debug.Print("queuedAction [" & queuedAction & "]")
                                        myGlobal = ManageAnalyzer(queuedAction, True, Nothing, queuedParam)
                                    End If
                                End If
                            End If
                        End If 'If pSendingEvent Then
                    End If 'If Not AnalyzerIsReadyAttribute Then

                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = "NO_THREADS"

                    'SGM 17/11/2011
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                        MyClass.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.NONE
                        ' Set Waiting Timer Current Instruction OFF
                        ClearQueueToSend()
                        RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.NO_THREADS.ToString)
                    End If
                    'SGM 17/11/2011
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Remove any instruction from the queue
        ''' </summary>
        ''' <remarks>Created by XB 06/11/2013</remarks>
        Public Sub RemoveItemFromQueue(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList) Implements IAnalyzerEntity.RemoveItemFromQueue
            Try
                Dim j As Integer = -1
                For Each A As GlobalEnumerates.AnalyzerManagerSwActionList In myInstructionsQueue
                    If A = pAction Then
                        j = myInstructionsQueue.IndexOf(A)
                        Exit For
                    End If
                Next

                If myInstructionsQueue.Count > 0 AndAlso j > -1 Then
                    myInstructionsQueue.RemoveAt(j)
                    myParamsQueue.RemoveAt(j)
                    Debug.Print("INSTRUCTION REMOVED FROM QUEUE : [" & pAction.ToString & "] ____________________________________")
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveItemFromQueue", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Enables the Sw communications to send new instructions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by AG 29/10/2010</remarks>
        Public Function ClearQueueToSend() As GlobalDataTO Implements IAnalyzerEntity.ClearQueueToSend
            Dim myGlobal As New GlobalDataTO

            Try
                Me.InitializeTimerControl(WAITING_TIME_OFF)
                myInstructionsQueue.Clear()
                myParamsQueue.Clear()   ' XBC 24/05/2011
                AnalyzerIsReadyAttribute = True

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ClearQueueToSend", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function


        ''' <summary>
        ''' Informs if the instructions queue contains a specific instruction pending to be sent
        ''' </summary>
        ''' <param name="pInstruction"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 26/03/2012 - creation</remarks>
        Public Function QueueContains(ByVal pInstruction As GlobalEnumerates.AnalyzerManagerSwActionList) As Boolean Implements IAnalyzerEntity.QueueContains
            Dim returnValue As Boolean = False
            Try
                returnValue = myInstructionsQueue.Contains(pInstruction)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.QueueContains", EventLogEntryType.Error, False)
                returnValue = False
            End Try
        End Function

        ''' <summary>
        ''' Returns Scripts Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Scripts</returns>
        ''' <remarks>Created by XBC 08/11/2010</remarks>
        Public Function ReadFwScriptData() As GlobalDataTO Implements IAnalyzerEntity.ReadFwScriptData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadFwScriptData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadFwScriptList", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' SERVICE SOFTWARE
        ''' Loads to the Application the Global Scripts Data from the Xml File
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 24/11/10</remarks>
        Public Function LoadAppFwScriptsData() As GlobalDataTO Implements IAnalyzerEntity.LoadAppFwScriptsData
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal = AppLayer.LoadAppFwScriptsData

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.LoadAppFwScriptsData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Loads FW Adjustments from DB
        ''' </summary>
        ''' <returns>DS with FW Adjustments</returns>
        ''' <remarks>
        ''' Created by SG 26/01/11
        ''' Modified by XBC : 30/09/2011 - Add Master data feature
        ''' Modified by SGM : 24/11/2011 - Get Master data from resources if SimulationMode
        ''' </remarks>
        Public Function LoadFwAdjustmentsMasterData(Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO Implements IAnalyzerEntity.LoadFwAdjustmentsMasterData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.LoadFwAdjustmentsMasterData("MasterData", pSimulationMode)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Reads FW Adjustments DS
        ''' </summary>
        ''' <returns>DS with FW Adjustments</returns>
        ''' <remarks>Created by SG 26/01/11</remarks>
        Public Function ReadFwAdjustmentsDS() As GlobalDataTO Implements IAnalyzerEntity.ReadFwAdjustmentsDS
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadFwAdjustmentsDS

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Updates FW Adjustments DS
        ''' </summary>
        ''' <remarks>Created by SG 26/01/11</remarks>
        Public Function UpdateFwAdjustmentsDS(ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO Implements IAnalyzerEntity.UpdateFwAdjustmentsDS
            Dim myGlobal As New GlobalDataTO
            Try
                If pAdjustmentsDS IsNot Nothing Then

                    'SGM 05/12/2011
                    pAdjustmentsDS.AnalyzerModel = MyClass.myAnalyzerModel
                    pAdjustmentsDS.AnalyzerID = MyClass.ActiveAnalyzer
                    pAdjustmentsDS.FirmwareVersion = MyClass.FwVersionAttribute
                    pAdjustmentsDS.ReadedDatetime = DateTime.Now
                    'end SGM 05/12/2011

                    AppLayer.UpdateFwAdjustmentsDS(pAdjustmentsDS)
                    myGlobal.SetDatos = pAdjustmentsDS
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        '''' <summary>
        '''' Loads FW Sensors Master Data from DB
        '''' </summary>
        '''' <returns>DS with FW Sensors</returns>
        '''' <remarks>Created by SG 28/02/11</remarks>
        'Public Function LoadFwSensorsMasterData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        myGlobal = AppLayer.LoadFwSensorsMasterData

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.LoadFwSensorsMasterData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' SERVICE
        '''' Gets the Sensors Master data for a Screen
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>SGM 28/02/11</remarks>
        'Public Function GetGlobalSensorsMasterData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        myGlobal = AppLayer.GetGlobalSensorsMasterData

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ApplicationLayer.GetGlobalSensorsMasterData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <remarks>Created by SG 28/02/11</remarks>
        'Public Function ReadSensorsData(ByVal pSensors As SRVSensorsDS) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        Dim mySensorsDS As New SRVSensorsDS
        '        For Each S As SRVSensorsDS.srv_tfmwSensorsRow In pSensors.srv_tfmwSensors.Rows

        '            myGlobal = AppLayer.GetSensorDataById(S.SensorId)

        '            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
        '                Dim myRow As SRVSensorsDS.srv_tfmwSensorsRow
        '                Dim myNewRow As SRVSensorsDS.srv_tfmwSensorsRow
        '                myRow = CType(myGlobal.SetDatos, SRVSensorsDS.srv_tfmwSensorsRow)

        '                myNewRow = mySensorsDS.srv_tfmwSensors.Newsrv_tfmwSensorsRow
        '                With myNewRow
        '                    .SensorId = myRow.SensorId
        '                    .GroupId = myRow.GroupId
        '                    .CodeFw = myRow.CodeFw
        '                    .Type = myRow.Type
        '                    If Not myRow.IsValueNull And Not myRow.IsTimeStampNull Then
        '                        .Value = myRow.Value
        '                        .TimeStamp = myRow.TimeStamp
        '                    End If
        '                    .HasError = myRow.HasError
        '                    .Interval = myRow.Interval
        '                End With
        '                mySensorsDS.srv_tfmwSensors.Addsrv_tfmwSensorsRow(myNewRow)
        '                mySensorsDS.AcceptChanges()

        '            Else
        '                Exit Try
        '            End If
        '        Next


        '        myGlobal.SetDatos = mySensorsDS

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadSensorsData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function


        ''' <summary>
        ''' Returns PhotometryDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Photometry</returns>
        ''' <remarks>Created by XBC 28/02/2011</remarks>
        Public Function ReadPhotometryData() As GlobalDataTO Implements IAnalyzerEntity.ReadPhotometryData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadPhotometryData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadPhotometryData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Set PhotometryDataTO
        ''' </summary>
        ''' <param name="pPhotometryData">GlobalDataTo with the data of the Photometry</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function SetPhotometryData(ByVal pPhotometryData As PhotometryDataTO) As GlobalDataTO Implements IAnalyzerEntity.SetPhotometryData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.SetPhotometryData(pPhotometryData)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SetPhotometryData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns StressDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Stress Mode</returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Public Function ReadStressModeData() As GlobalDataTO Implements IAnalyzerEntity.ReadStressModeData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadStressModeData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadStressModeData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE
        ''' Set StressDataTO
        ''' </summary>
        ''' <param name="pStressModeData">GlobalDataTo with the data of the Stress Mode</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Public Function SetStressModeData(ByVal pStressModeData As StressDataTO) As GlobalDataTO Implements IAnalyzerEntity.SetStressModeData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.SetStressModeData(pStressModeData)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.SetStressModeData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        '''' <summary>
        '''' SERVICE - Gets the absorbance scan data
        '''' </summary>
        '''' <remarks></remarks>
        'Public Function GetAbsorbanceScanData() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try

        '        myGlobal = AppLayer.GetAbsorbanceScanData()

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.GetAbsorbanceScanData", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function


        ''' <summary>
        ''' Returns  OpticCenterDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Optic centering</returns>
        ''' <remarks>Created by XBC 12/05/2011</remarks>
        Public Function ReadOpticCenterData() As GlobalDataTO Implements IAnalyzerEntity.ReadOpticCenterData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal.SetDatos = Me.OpticCenterResultsAttr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadOpticCenterData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' USB connection cable has been disconnected and presentation inform this to AnalyzerManager class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Modified by XBC 06/09/2012 - Add new parameter pConnectionCompleted</remarks>
        Public Function ProcessUSBCableDisconnection(Optional ByVal pConnectionCompleted As Boolean = False) As GlobalDataTO Implements IAnalyzerEntity.ProcessUSBCableDisconnection
            Dim myGlobal As New GlobalDataTO
            Try
                ConnectedAttribute = False

                ' XBC 16/01/2013 - Prepare ISE Object to reset it after a disconnection communications (Bugs tracking #1109)
                If Not ISEAnalyzer Is Nothing Then
                    ISEAnalyzer.IsAnalyzerDisconnected = True
                    MyClass.ISEAlreadyStarted = False
                End If
                ' XBC 16/01/2013

                ' XBC 26/10/2011
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    ' Set Waiting Timer Current Instruction OFF
                    SensorValuesAttribute.Clear() 'SGM 08/11/2012
                    ClearQueueToSend()
                    RaiseEvent SendEvent(GlobalEnumerates.AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                End If
                ' XBC 26/10/2011

                'AG 08/01/2014 - If communications are lost in running clear all instructions in queue pending to be sent BT #1436
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    myInstructionsQueue.Clear()
                    myParamsQueue.Clear()
                End If
                'AG 08/01/2014

                'AG 13/02/2012 - The method UpdateSensorValuesAttribute add the alarm (sensor & database) but 
                'do not use it because the disconnection does not causes UI Refresh event
                'So we only update the sensor and the presentation method is who shows the message and add alarm into database

                'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0, True)'AG 10/02/2012
                If Not SensorValuesAttribute.ContainsKey(GlobalEnumerates.AnalyzerSensors.CONNECTED) Then
                    SensorValuesAttribute.Add(GlobalEnumerates.AnalyzerSensors.CONNECTED, 0)
                Else
                    SensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.CONNECTED) = 0
                End If
                'AG 13/02/2012

                ' XBC 06/09/2012
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS 'AG 09/09/2013
                If pConnectionCompleted AndAlso String.Compare(mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString), "INPROCESS", False) = 0 Then
                    'Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS 'AG 09/09/2013
                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "CLOSED")

                    ''AG 09/09/2013 'Update analyzer session flags into DataBase
                    'If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    '    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    '    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    'End If
                    'AG 09/09/2013
                End If
                ' XBC 06/09/2012

                ''AG 09/09/2013 - if error comms while barcode reading set the flag to NOT RUNNING REQUEST
                'This code doesnt work properly!! So we leave commented. Bug #1284
                'If BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActions.BARCODE_AVAILABLE Then
                '    BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActions.BARCODE_AVAILABLE
                '    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess.ToString) = "INPROCESS" Then
                '        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess, "CLOSED")
                '    End If
                'End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If
                'AG 09/09/2013

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessUSBCableDisconnection", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Stop analyzer sound alarm (called from presentation layer)
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="pForceEndSound">Force End Sound.</param>
        ''' <remarks>create by: TR 28/10/2011</remarks>
        Public Function StopAnalyzerRinging(Optional ByVal pForceEndSound As Boolean = False) As GlobalDataTO Implements IAnalyzerEntity.StopAnalyzerRinging
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Send the sound off only when is ringing
                If ConnectedAttribute AndAlso (AnalyzerIsRingingAttribute OrElse pForceEndSound) Then
                    myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True)
                    System.Threading.Thread.Sleep(500)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StopAnalyzerRinging", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Start the analyzer sound (called from presentation layer)
        ''' </summary>
        ''' <param name="pForceSound"></param>
        ''' <returns></returns>
        ''' <remarks>AG 22/07/2013 - based and adapted from StopAnalyzerRinging</remarks>
        Public Function StartAnalyzerRinging(Optional ByVal pForceSound As Boolean = False) As GlobalDataTO Implements IAnalyzerEntity.StartAnalyzerRinging
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Check the alarm sound setting is not disabled
                myGlobalDataTO = IsAlarmSoundDisable(Nothing)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    'If not disabled ... continue
                    If Not DirectCast(myGlobalDataTO.SetDatos, Boolean) Then
                        'Send the sound ON only when analyzer is not still ringing
                        If ConnectedAttribute AndAlso (Not AnalyzerIsRingingAttribute OrElse pForceSound) Then
                            myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SOUND, True)
                            System.Threading.Thread.Sleep(500)
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StartAnalyzerRinging", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Requests to Analyzer to Stop ANSINFO continuous mode responsing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/07/2012</remarks>
        Public Function StopAnalyzerInfo() As GlobalDataTO Implements IAnalyzerEntity.StopAnalyzerInfo
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If ConnectedAttribute AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                    myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                             True, _
                                                             Nothing, _
                                                             GlobalEnumerates.Ax00InfoInstructionModes.STP)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.StopAnalyzerInfo", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get the specific adjust value in parameter for the current analyzer
        ''' 
        ''' Use this method when Software business needs only one adjust value, when Software business
        ''' needs more than one adjusts is better call the ReadFwAdjustmentsDS and get all required
        ''' adjusts using several Linq
        ''' 
        ''' 
        ''' NOTE: no exit try is performed due the caller method it is supposed to implement it
        ''' </summary>
        ''' <param name="pAdjust"></param>
        ''' <returns>String with the adjust value (String due this field is saved as nvarchar into database</returns>
        ''' <remarks>Created by AG 23 11 2011</remarks>
        Public Function ReadAdjustValue(ByVal pAdjust As GlobalEnumerates.Ax00Adjustsments) As String Implements IAnalyzerEntity.ReadAdjustValue
            Dim myGlobal As New GlobalDataTO
            Dim returnValue As String = ""

            myGlobal = ReadFwAdjustmentsDS()
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                Dim myAdjDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Compare(a.CodeFw, pAdjust.ToString, False) = 0 Select a).ToList
                If linqRes.Count > 0 Then
                    returnValue = linqRes(0).Value
                End If
                linqRes = Nothing 'AG 02/08/2012 - free memory
            End If
            Return returnValue

        End Function


        ''' <summary>
        ''' Barcode functionality: Before enter RUNNING the analyzer
        ''' Before go Running: 1st read Reagents barcode (if configurated and barcode enabled)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBarcodeProcessCurrentStep"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 04/08/2011
        ''' Modified by: AG 09/05/2012 - Set default value of flag readBarCodeBeforeRunningPerformedFlag = TRUE when the barcode read is executed
        '''                              before RUNNING 
        '''              AG 29/05/2012 - If both Samples and Reagents Barcode are not available, the Analyzer enters in RUNNING Mode
        '''              SA 26/07/2012 - When calling function to create Executions, ISEModuleIsReadyAttribute is sent as parameter to allow blocking
        '''                              all ISE Executions when the module is not ready
        '''              AG 06/11/2013 - BT #1375 ==> In Pause mode, allow return to normal running although no more pending executions exists
        '''              SA 10/04/2014 - BT #1584 ==> When the Analyzer is in PAUSE mode, flag runningFlag has to be set to FALSE although the Analyzer 
        '''                                           Status is Running. This is to allow block/unblock Executions when elements have been positioned/unpositioned 
        '''                                           during the Pause
        ''' </remarks>
        Public Function ManageBarCodeRequestBeforeRUNNING(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarcodeProcessCurrentStep As BarcodeWorksessionActionsEnum) As GlobalDataTO Implements IAnalyzerEntity.ManageBarCodeRequestBeforeRUNNING
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                        'When user press START or CONTINUE buttons ... check the configuration status as first step
                        If (pBarcodeProcessCurrentStep = BarcodeWorksessionActionsEnum.BEFORE_RUNNING_REQUEST) Then
                            'Read barcode before RUNNING is enabled?
                            '** If YES: send active barcode requests for read + apply results business
                            '** If NO: go to running
                            readBarCodeBeforeRunningPerformedFlag = False

                            Dim usSettingsDelg As New UserSettingsDelegate
                            resultData = usSettingsDelg.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.BARCODE_BEFORE_START_WS.ToString())
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                                Dim barcodeReadBeforeRunningFlag As Boolean = CType(resultData.SetDatos, Boolean)

                                'AG 08/07/2013 - Special case for automate the WS creation with LIS (read barcode always and do not take care about the setting)
                                If autoWSCreationWithLISModeAttribute Then
                                    barcodeReadBeforeRunningFlag = True
                                End If
                                'AG 08/07/2013

                                If (Not barcodeReadBeforeRunningFlag) Then
                                    'BARCODE_BEFORE_START_WS not active: go to Running directly
                                    BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING  'AG 08/03/2013 change ENTER_RUNNING for the new enum value
                                Else
                                    'BARCODE_BEFORE_START_WS active
                                    '1st) If Reagents Barcode is active -> send a request for full Reagents Rotor scanning
                                    Dim barcodeDisabled As Boolean = ReadBarCodeRotorSettingEnabled(dbConnection, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
                                    If (Not barcodeDisabled) Then
                                        BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.REAGENTS_REQUEST_BEFORE_RUNNING
                                    Else
                                        '2nd) If Samples Barcode is active -> send a request for full Samples Rotor scanning
                                        barcodeDisabled = ReadBarCodeRotorSettingEnabled(dbConnection, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                                        If (Not barcodeDisabled) Then
                                            BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING
                                        Else
                                            BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.ENTER_RUNNING
                                        End If
                                    End If
                                End If
                            End If

                        ElseIf (pBarcodeProcessCurrentStep = BarcodeWorksessionActionsEnum.REAGENTS_REQUEST_BEFORE_RUNNING) Then
                            'After receive Reagents Barcode results, evaluate if Samples Barcode is active
                            'If Samples Barcode is active -> send a request for full Samples Rotor scanning
                            readBarCodeBeforeRunningPerformedFlag = True
                            Dim samplesBarcodeDisabled As Boolean = ReadBarCodeRotorSettingEnabled(dbConnection, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                            If (Not samplesBarcodeDisabled) Then
                                BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING
                            Else
                                BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.ENTER_RUNNING
                            End If

                        ElseIf (pBarcodeProcessCurrentStep = BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING) Then
                            'After receive Samples Barcode results go to Running
                            readBarCodeBeforeRunningPerformedFlag = True
                            BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.ENTER_RUNNING
                        End If

                        If (Not resultData.HasError) Then
                            Select Case BarCodeBeforeRunningProcessStatusAttribute
                                Case BarcodeWorksessionActionsEnum.BEFORE_RUNNING_REQUEST, BarcodeWorksessionActionsEnum.NO_RUNNING_REQUEST
                                    'Do nothing

                                Case BarcodeWorksessionActionsEnum.REAGENTS_REQUEST_BEFORE_RUNNING, BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING
                                    'Send read FULL REAGENTS / SAMPLES ROTOR request
                                    Dim BarCodeDS As New AnalyzerManagerDS
                                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

                                    Dim rotorType As String = "REAGENTS"
                                    If (BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING) Then rotorType = "SAMPLES"

                                    'All positions
                                    rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                                    With rowBarCode
                                        .RotorType = rotorType
                                        .Action = GlobalEnumerates.Ax00CodeBarAction.FULL_ROTOR
                                        .Position = 0
                                    End With
                                    BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                    BarCodeDS.AcceptChanges()

                                    resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS, "")

                                    If (Not resultData.HasError AndAlso ConnectedAttribute) Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess, "INPROCESS")
                                        SetAnalyzerNotReady()
                                    End If

                                    'AG 08/03/2013 - add FORCE_ENTER_RUNNING
                                    'AG 01/04/2014 - #1565 add FORCE_ENTER_RUNNING_AUTO_WS_CREATION 
                                Case BarcodeWorksessionActionsEnum.ENTER_RUNNING, BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING, BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING_WITHOUT_CREATE_EXECUTIONS
                                    'START WorkSession process finished. Barcodes are available
                                    'BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActions.BARCODE_AVAILABLE 'AG 08/03/2013

                                    Dim dlgBarcode As New BarcodeWSDelegate
                                    resultData = dlgBarcode.ExistBarcodeCriticalWarnings(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "SAMPLES")
                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess, "CLOSED")

                                        'AG 12/07/2013 - Special case for automate the WS creation with LIS (force the HQ monitor screen to be open)
                                        'AG 01/04/2014 - #1565 add also FORCE_ENTER_RUNNING_AUTO_WS_CREATION
                                        'If autoWSCreationWithLISModeAttribute AndAlso BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActions.FORCE_ENTER_RUNNING Then
                                        If autoWSCreationWithLISModeAttribute AndAlso BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING _
                                            AndAlso BarCodeBeforeRunningProcessStatusAttribute <> BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING_WITHOUT_CREATE_EXECUTIONS Then
                                            resultData.SetDatos = True
                                        End If
                                        'AG 12/07/2013

                                        'AG 08/03/2013 - case FORCE_ENTER_RUNNING do not stops process instead there are barcode warnings
                                        If BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING Then resultData.SetDatos = False
                                        If BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING_WITHOUT_CREATE_EXECUTIONS Then resultData.SetDatos = False 'AG 01/04/2014 - #1565

                                        If (CType(resultData.SetDatos, Boolean) = False) Then 'No critical errors ... Sw can send the go to running instruction
                                            'AG 09/05/2012 - When barcode is read before running call the method to create WS Executions again with the real rotor contents
                                            Dim enterInRunningModeFlag As Boolean = True
                                            If (readBarCodeBeforeRunningPerformedFlag) Then

                                                'AG 01/04/2014 - #1565 Do not execute CreateWSExecutions when we are in the AutoWSCreation case
                                                'and the WS has been modified (executions have been created on close rotor positions screen)
                                                '(put previous code inside the next IF - ENDIF block)
                                                If BarCodeProcessBeforeRunning <> BarcodeWorksessionActionsEnum.FORCE_ENTER_RUNNING_WITHOUT_CREATE_EXECUTIONS Then

                                                    Dim iseModuleReady As Boolean = (Not ISEAnalyzer Is Nothing AndAlso ISEAnalyzer.IsISEModuleReady)

                                                    'Inside this IF: Use Nothing instead of dbConnection to open a new transaction!!
                                                    Dim myExecutionsDlg As New ExecutionsDelegate

                                                    'AG 31/03/2014 - #1565 add trace + inform the real analyzer status!!
                                                    'resultData = myExecutionsDlg.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, False, -1, _
                                                    'String.Empty, iseModuleReady)
                                                    Dim myLogAcciones As New ApplicationLogManager()
                                                    myLogAcciones.CreateLogActivity("Launch CreateWSExecutions !", "AnalyzerManager.ManageBarCodeRequestBeforeRUNNING", EventLogEntryType.Information, False)

                                                    'AG 30/05/2014 #1644 - Redesing correction #1584 for avoid DeadLocks (runningMode + new parameter AllowScanInRunning)
                                                    'Verify is the current Analyzer Status is RUNNING ==> BT #1584: ...and it is not in PAUSE
                                                    'Dim runningFlag As Boolean = (AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                                                    '                              Not AllowScanInRunning)
                                                    ''Create the Executions
                                                    'resultData = myExecutionsDlg.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, runningFlag, -1, _
                                                    '                                                String.Empty, iseModuleReady)
                                                    Dim runningFlag As Boolean = (AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)

                                                    'Create the Executions
                                                    resultData = myExecutionsDlg.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, runningFlag, -1, _
                                                                                                    String.Empty, iseModuleReady, Nothing, AllowScanInRunning)
                                                    'AG 30/05/2014 #1644


                                                End If 'AG 01/04/2014 - #1565

                                                If (Not resultData.HasError) Then
                                                    'Prepare UI for refresh Presentation layer screen (first execution)
                                                    resultData = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, 1, 0, Nothing, False)

                                                    Dim execucionsCount As Integer = 0
                                                    Dim pendingExecutionsLeft As Boolean = False

                                                    Dim wsDelg As New WorkSessionsDelegate
                                                    resultData = wsDelg.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, execucionsCount, pendingExecutionsLeft)

                                                    'AG 06/11/2013 - Task #1375 - in paused mode allow return to normal running although no more pending executions exists
                                                    If Not pendingExecutionsLeft AndAlso AllowScanInRunningAttribute Then
                                                        pendingExecutionsLeft = True
                                                    End If
                                                    'AG 06/11/2013

                                                    If (execucionsCount = 0 OrElse Not pendingExecutionsLeft) Then
                                                        'Nothing to be sent
                                                        enterInRunningModeFlag = False
                                                    End If
                                                Else
                                                    'Error
                                                    enterInRunningModeFlag = False
                                                End If
                                            End If
                                            'AG 09/05/2012

                                            'AG 19/01/2012 - Do not enter in Running, instead check if ise auto conditioning is required or not
                                            'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True) 'Send go to RUNNING

                                            'If Not resultData.HasError AndAlso ConnectedAttribute Then
                                            '    endRunAlreadySentFlagAttribute = False
                                            '    abortAlreadySentFlagAttribute = False

                                            '    'Set WorkSession Status to INPROCESS
                                            '    Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                            '    resultData = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS")
                                            'End If

                                            '1) Check if ISE auto conditioning is required: 
                                            '   YES iseAutoConditioning = "INPROCESS"
                                            '   NO iseAutoConditioning = "CLOSED"
                                            '   Finally call method PerformeAutoIseConditioning
                                            'AG 19/01/2012

                                            If (enterInRunningModeFlag) Then
                                                BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.BARCODE_AVAILABLE 'AG 11/03/2013

                                                'TODO
                                                'Evaluate if auto ISE maintenance is required or not and update flag ISEConditioningProcess
                                                Dim iseAutoConditioning As Boolean = False

                                                If (iseAutoConditioning) Then
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess, "INPROCESS")
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEClean, "")
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEPumpCalib, "")
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISECalibAB, "")
                                                Else
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                                                End If

                                                If (Not resultData.HasError) Then
                                                    resultData = PerformAutoIseConditioning(dbConnection)
                                                End If
                                            Else
                                                'Do not use resultData for not lose error if exists
                                                Dim resultDataAux As New GlobalDataTO
                                                resultDataAux = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True)    'Running can not be sent. Ask for status for reactivate UI

                                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS, 2, True)     'Not pending executions to be sent
                                                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due scan barcode warnings
                                            End If
                                        Else
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS, 1, True)     'Samples barcode warnings
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due scan barcode warnings
                                        End If
                                    End If
                            End Select
                        End If

                        If (Not resultData.HasError AndAlso myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            resultData = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ManageBarCodeRequestBeforeRUNNING", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Return TRUE when some alarm in system fluids (wash solution, high contamination, water or waste) exists
        ''' This method is used for UI buttons activation or not and for processes who automatically send instruction that requires no system fluids alarms
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>AG 20/02/2012</remarks>
        ''' Modified by: JV 23/01/2014 #1467
        Public Function ExistBottleAlarms() As Boolean Implements IAnalyzerEntity.ExistBottleAlarms
            Dim returnValue As Boolean = False
            Try
                'JV 23/01/2014 #1467
                'If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                '    OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                '    OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) _
                '    OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) Then

                '    returnValue = True
                'End If

                'In Running pause mode only the alarms of washing solution or high contamination bottles
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso AllowScanInRunningAttribute Then
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                   OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then

                        returnValue = True
                    End If

                    'Otherwise all: washing solution, high contamination, waster deposit or water deposit
                Else
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                   OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                   OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) _
                   OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) Then

                        returnValue = True
                    End If
                End If
                'JV 23/01/2014 #1467

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ExistBottleAlarms", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function


        ''' <summary>
        ''' Update corresponding Work Session tables depending of the Analyzer Instrument to which is connected
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC : 11/06/2012
        ''' Modified by XB : 12/11/2013 - Allow Sw Service the full functionality about WorkSession - BT #169 SERVICE
        ''' </remarks>
        Public Function ProcessUpdateWSByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO Implements IAnalyzerEntity.ProcessUpdateWSByAnalyzerID
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                Dim myWSAnalyzerID As String = ""

                ' open db transaction
                myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        ' Insert (if not exists) the Analyzer identifier connected
                        Dim myLogAccionesAux As New ApplicationLogManager()
                        myLogAccionesAux.CreateLogActivity("(Analyzer Change) Insert Connect Analyzer ", "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)

                        myGlobal = MyClass.InsertConnectedAnalyzer(dbConnection)

                        ' XBC 06/07/2012
                        ' Initialize Adjustments
                        InitializeFWAdjustments(dbConnection)

                        'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                        'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        'If Not GlobalBase.IsServiceAssembly Then   ' XB 12/11/2013
                        ' For User Sw and also Service Sw

                        If Not myGlobal.HasError Then
                            'Get the worksession status
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                            Dim myReactionsRotorDelegate As New ReactionsRotorDelegate
                            Dim myRotorContentbyPositionDelegate As New WSRotorContentByPositionDelegate
                            Dim myWSNotInUseRotorPositionsDelegate As New WSNotInUseRotorPositionsDelegate

                            myGlobal = myWSAnalyzersDelegate.GetByWorkSession(Nothing, ActiveWorkSession)
                            If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                                Dim myWSAnalyzersDS As New WSAnalyzersDS
                                myWSAnalyzersDS = DirectCast(myGlobal.SetDatos, WSAnalyzersDS)
                                If myWSAnalyzersDS.twksWSAnalyzers.Count > 0 Then
                                    WorkSessionStatusAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                    myWSAnalyzerID = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID

                                    Dim myLogAcciones As New ApplicationLogManager()
                                    Select Case WorkSessionStatusAttribute

                                        Case "EMPTY"
                                            If Not String.Compare(myWSAnalyzerID, ActiveAnalyzer, False) = 0 Then
                                                myLogAcciones.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                ' Update field AnalyzerID in table Analyzers WS
                                                myGlobal = myWSAnalyzersDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                If Not myGlobal.HasError Then
                                                    ' Update field AnalyzerID in table Reactions Rotor WS
                                                    myGlobal = myReactionsRotorDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, myWSAnalyzerID)
                                                    If Not myGlobal.HasError Then
                                                        ' Update field AnalyzerID in table Rotor Content By Position WS
                                                        myGlobal = myRotorContentbyPositionDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                        If Not myGlobal.HasError Then
                                                            ' Update field AnalyzerID in table Not In Use Rotor Positions WS
                                                            myGlobal = myWSNotInUseRotorPositionsDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                        End If
                                                    End If
                                                End If
                                            End If

                                        Case "OPEN"
                                            If Not String.Compare(myWSAnalyzerID, ActiveAnalyzer, False) = 0 Then
                                                myLogAcciones.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                ' Update field AnalyzerID in table Analyzers WS
                                                myGlobal = myWSAnalyzersDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                If Not myGlobal.HasError Then
                                                    ' Update field AnalyzerID in table Reactions Rotor WS
                                                    myGlobal = myReactionsRotorDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, myWSAnalyzerID)
                                                    If Not myGlobal.HasError Then
                                                        ' Update field AnalyzerID in table Rotor Content By Position WS
                                                        myGlobal = myRotorContentbyPositionDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                        If Not myGlobal.HasError Then
                                                            ' Update field AnalyzerID in table Not In Use Rotor Positions WS
                                                            myGlobal = myWSNotInUseRotorPositionsDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, ActiveWorkSession)
                                                        End If
                                                    End If
                                                End If

                                                If Not myGlobal.HasError Then
                                                    ' Update field AnalyzerID in table Order Tests WS for all not CLOSED Order Tests
                                                    Dim myOrderTestDelegate As New OrderTestsDelegate
                                                    myGlobal = myOrderTestDelegate.UpdateWSAnalyzerID(dbConnection, ActiveAnalyzer, myWSAnalyzerID, "CLOSED")
                                                End If
                                            End If

                                        Case "PENDING"
                                            If Not myWSAnalyzerID = ActiveAnalyzer Then
                                                myLogAcciones.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                myGlobal = MyClass.ProcessToMountTheNewSession(dbConnection, myWSAnalyzerID)

                                                ' XBC 06/07/2012
                                                If Not myGlobal.HasError Then
                                                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, GlobalEnumerates.Ax00Adjustsments.ALL)

                                                    If Not myGlobal.HasError AndAlso myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                        myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                                                    End If
                                                End If
                                                ' XBC 06/07/2012
                                            End If

                                        Case Else
                                            If Not myWSAnalyzerID = ActiveAnalyzer Then
                                                myLogAcciones.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                myGlobal = MyClass.ProcessToMountTheNewSession(dbConnection, myWSAnalyzerID)
                                                ForceAbortSessionAttr = True

                                                ' XBC 06/07/2012
                                                If Not myGlobal.HasError Then
                                                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                                                    UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                                                    myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, GlobalEnumerates.Ax00Adjustsments.ALL)

                                                    If Not myGlobal.HasError AndAlso myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                        myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                                                    End If
                                                End If
                                                ' XBC 06/07/2012
                                            Else

                                                ' TODO : ANALYZER RECOVERY PROCESS - PENDING !!!

                                            End If

                                    End Select

                                End If

                            End If

                        End If

                        'End If     ' XB 12/11/2013


                        ' Finally Old Analyzer (in case to exists) must be deleted
                        If myWSAnalyzerID.Length > 0 AndAlso Not myWSAnalyzerID = ActiveAnalyzer Then
                            If Not myGlobal.HasError Then
                                Dim confAnalyzers As New AnalyzersDelegate
                                myGlobal = confAnalyzers.DeleteConnectedAnalyzersNotActive(dbConnection)
                            End If
                        End If


                        If Not myGlobal.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' When there is a WorkSession defined for an Analyzer different to the connected one, the WorkSession definition is 
        ''' "copied" for the Active Analyzer and the WorkSession is reset for the previous one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWSAnalyzerID">Identifier of the Analyzer where the active WorkSession was prepared/executed</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' Modified by: XBC 25/04/2013 - Add value = FALSE for the new optional pSaveLISPendingOrders parameter to indicate that the process to Save the LIS orders not processed Is NOT required
        ''' </remarks>
        Public Function ProcessToMountTheNewSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSAnalyzerID As String) As GlobalDataTO Implements IAnalyzerEntity.ProcessToMountTheNewSession
            Dim myGlobal As New GlobalDataTO

            Try
                Dim myWSDelegate As New WorkSessionsDelegate
                Dim mySavedWSDelegate As New SavedWSDelegate

                myGlobal = myWSDelegate.GetOrderTestsForWS(pDBConnection, ActiveWorkSession)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim myWorkSessionResultDS As WorkSessionResultDS = DirectCast(myGlobal.SetDatos, WorkSessionResultDS)

                    'Save temporally the WS to recuperate it after the Reset of the current WS
                    myGlobal = mySavedWSDelegate.Save(pDBConnection, ActiveAnalyzer, myWorkSessionResultDS, -1)
                End If

                Dim savedWSID As Integer
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    savedWSID = DirectCast(myGlobal.SetDatos, Integer)

                    'Reset the current WS
                    If (Not HISTWorkingMode) Then
                        myGlobal = myWSDelegate.ResetWS(pDBConnection, pWSAnalyzerID, ActiveWorkSession, False)
                    Else
                        myGlobal = myWSDelegate.ResetWSNEW(pDBConnection, pWSAnalyzerID, ActiveWorkSession, False, False)
                    End If
                    If (Not myGlobal.HasError) Then MyClass.ResetWorkSession()
                End If

                If (Not myGlobal.HasError) Then
                    'Update field AnalyzerID in table Reactions Rotor WS
                    Dim myReactionsRotorDelegate As New ReactionsRotorDelegate
                    myGlobal = myReactionsRotorDelegate.UpdateWSAnalyzerID(pDBConnection, ActiveAnalyzer, pWSAnalyzerID)
                End If

                If (Not myGlobal.HasError) Then
                    'Load new WorkSession saved with name ActiveAnalyzer
                    Dim myOrderTests As New OrderTestsDelegate

                    myGlobal = myOrderTests.LoadFromSavedWSToChangeAnalyzer(pDBConnection, savedWSID, ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(myGlobal.SetDatos, WorkSessionsDS)

                        If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                            ActiveWorkSession = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                            WorkSessionStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus
                        End If
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    myGlobal = mySavedWSDelegate.ExistsSavedWS(pDBConnection, ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim mySavedWSDS As SavedWSDS = DirectCast(myGlobal.SetDatos, SavedWSDS)

                        'Delete the temporary Work Session created initially
                        myGlobal = mySavedWSDelegate.Delete(pDBConnection, mySavedWSDS)
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessToMountTheNewSession", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Insert Active Analyzer into database
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 13/06/2012</remarks>
        Public Function InsertConnectedAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO Implements IAnalyzerEntity.InsertConnectedAnalyzer
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myLogAccionesAux As New ApplicationLogManager()
                If Not myTmpConnectedAnalyzerDS Is Nothing Then

                    myLogAccionesAux.CreateLogActivity("(Analyzer Change) 1st condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                    If myTmpConnectedAnalyzerDS.tcfgAnalyzers.Count > 0 Then
                        myLogAccionesAux.CreateLogActivity("(Analyzer Change) 2nd condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                        If Not myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).IsIsNewNull Then
                            myLogAccionesAux.CreateLogActivity("(Analyzer Change) 3rd condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                            If myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).IsNew Then
                                Dim confAnalyzers As New AnalyzersDelegate
                                myLogAccionesAux.CreateLogActivity("(Analyzer Change) Insert Analyzer ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                myGlobal = confAnalyzers.InsertAnalyzer(pDBConnection, myTmpConnectedAnalyzerDS.tcfgAnalyzers(0))

                                If Not myGlobal.HasError Then
                                    myLogAccionesAux.CreateLogActivity("(Analyzer Change) Copy Analyzer Settings ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                    MyClass.CopyAnalyzerSettings(pDBConnection, myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID)
                                    MyClass.ActiveAnalyzer = myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID
                                    myLogAccionesAux.CreateLogActivity("(Analyzer Change) Active Analyzer OK [" & MyClass.ActiveAnalyzer & "] ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                End If
                            End If
                        Else
                            MyClass.ActiveAnalyzer = myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID
                            myLogAccionesAux.CreateLogActivity("(Analyzer Change) Active Analyzer [" & MyClass.ActiveAnalyzer & "] ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Read Adjustments
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 06/07/2012</remarks>
        Public Function ReadAdjustments(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO Implements IAnalyzerEntity.ReadAdjustments
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                ' open db transaction
                myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, GlobalEnumerates.Ax00Adjustsments.ALL)

                        If Not myGlobal.HasError AndAlso myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                        End If


                        If Not myGlobal.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If

                    End If

                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ReadAdjustments", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Remove the DS with information with the results automatically exported
        ''' </summary>
        ''' <remarks>AG 02/04/2013 - Creation</remarks>
        Public Sub ClearLastExportedResults() Implements IAnalyzerEntity.ClearLastExportedResults
            SyncLock lockThis
                lastExportedResultsDSAttribute.Clear()
            End SyncLock
        End Sub

        ''' <summary>
        ''' If exist some alarm whose treatment (in RUNNING) requires send the ENDRUN instruction return TRUE, otherwise return FALSE
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>AG 01/12/2011
        ''' AG 15/03/2012 - clot alarm do not implies END instruction (removed from IF)
        ''' AG 29/05/2012 - when analyzer is FREEZE software can not send the START instruction (added to IF)
        ''' AG 19/06/2012 - add CLOT_DETECTION_ERR alarm
        ''' AG 21/10/2013 - Method moved from MDI.ExistSomeAlarmThatRequiresSendENDRUN to analyzer manager class and adapted (remove parameter pCurrentAlarmList and use attribute myAlarmListAttribute) </remarks>
        Public Function ExistSomeAlarmThatRequiresStopWS() As Boolean Implements IAnalyzerEntity.ExistSomeAlarmThatRequiresStopWS
            Dim returnValue As Boolean = False
            Try
                If myAlarmListAttribute.Count > 0 Then
                    'AG 16/02/2012 the bottles warning level do not implies ENDRUN - remove from condition the alarms HIGH_CONTAMIN_WARN and WASH_CONTAINER_WARN
                    'AG 25/07/2012 - the clot detection error do not implies ENDRUN - remove from condition the alarm CLOT_DETECTION_ERR

                    'AG 22/02/2012 - NOTE: To prevent the bottle level ERROR or WARNING oscillations when near the limits Sw will not remove the ERROR level alarm until neither error neither warming exists 
                    If myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R1_COLLISION_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.R2_COLLISION_WARN) OrElse _
                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.S_COLLISION_WARN) OrElse myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                    BaseLine.exitRunningType <> 0 OrElse analyzerFREEZEFlagAttribute Then
                        returnValue = True
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ExistSomeAlarmThatRequiresStopWS", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function

#End Region

    End Class

End Namespace
