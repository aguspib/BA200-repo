Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports System.Timers
Imports System.Data
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Threading 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public MustInherit Class AnalyzerManager

#Region "Declarations"
        'General use Class variables
        Private WithEvents AppLayer As ApplicationLayer ' AG 20/04/2010 - app layer instance
        Private myAnalyzerModel As String = "" 'Use for read several software parameters depending on the model
        Private myApplicationName As String ' XBC 02/03/2011 'Different business code in some cases for Service & User Sw


        'Class variables for implement the waiting time processes until Ax00 becomes ready again
        Private myInstructionsQueue As New List(Of AnalyzerManagerSwActionList)    ' AG 20/05/2010
        Private myParamsQueue As New List(Of Object)    ' XBC 24/05/2010
        Private useRequestFlag As Boolean = False 'Ag 02/11/2010 - Flag for start using request (True when START instruction finish) / Return to False on send ENDRUN or ABORT
        Private waitingTimer As New Timers.Timer() ' AG 07/05/2010 - waiting time (watchdog)

        Private timeISEOffsetFirstTime As Boolean = False ' Just set the ISE offset time the first time when receives T=0 by Firmware - BA-1872
        ' XB 09/12/2014 - Parametizers all these values - BA-1872

        'Class variables to inform the presentation layer about UI refresh after instruction receptions
        'AG 07/10/2011 - Duplicate the refresh variable due:
        ' 1) The general mode is every instruction received raises a ReceptionEvent when his treatment is finish working in the same thread 
        '    (so use variables myUI_RefreshEvent, myUI_RefreshDS and eventDataPendingToTriggerFlag)
        ' 2) But some instructions (ANSPHR) are treated using 2 threads (main thread for chemical reactions and secondary for well base line). Both generates differents event refresh when finish
        '    (so use variables myUI_RefreshEvent, myUI_RefreshDS and eventDataPendingToTriggerFlag for the main thread)
        '    (and use mySecondaryUI_RefreshEvent, mySecondaryUI_RefreshDS and secondaryEventDataPendingToTriggerFlag for the secondary thread) --> REJECTED!!!! In running refreshs are added in 1 DSet and event is triggered 1 time each machine cycle
        Private myUI_RefreshEvent As New List(Of UI_RefreshEvents)
        Private myUI_RefreshDS As New UIRefreshDS
        Private eventDataPendingToTriggerFlag As Boolean = False 'This flag tell us if exits information in myUI_RefreshDS pending to be informed (True) or not exists information to be informed (False) (event ReceptionEvent)

        Private pauseSendingTestPreparationsFlag As Boolean = False 'This flag becomes to TRUE when some alarm appears and Sw stop sending new test preparations

        'Class variables for the send next preparation process
        Private mySentPreparationsDS As New AnalyzerManagerDS 'AG 18/01/2011 - remembers the lasts preparation & reagent wash sents (in RUNNING)
        Private myNextPreparationToSendDS As New AnalyzerManagerDS 'AG 07/06/2012 - remembers the next preparation & reagent wash to be sent (in RUNNING)
        Private futureRequestNextWell As Integer = 0 'AG 07/06/2012 - Using the current next well received in Request, estimate the future well in next request

        Private FLIGHT_INIT_FAILURES As Integer = 2 'AG 27/11/2014 BA-2066 - Default initial value for MAX FLIGHT failures without warning (real value will be read in the Init method)

        Private SENSORUNKNOWNVALUE As Integer = -1 'Default value for several sensors when the 0 value means alarm                        

        'Alarm Class variables
        Private myClassFieldLimitsDS As New FieldLimitsDS 'AG 30/03/2011 - tfmwFieldLimits contents are read once during the constructor. During running use this DS using Linq

        ' This dictionary implements all flags for saving the analyzer current state (status, action, processes done, processes in course, ...)
        Dim mySessionFlags As New Dictionary(Of String, String)

        'AG 31/03/2011 - Ellapse time for change Warning to Error (performed in Software)
        Private thermoReactionsRotorWarningTimer As New Timers.Timer() 'AG 05/01/2012- Controls thermo ReactionsRotor maximum time allowed for the warning alarm
        Private thermoFridgeWarningTimer As New Timers.Timer() 'AG 05/01/2012- Controls thermo Fridge maximum time allowed for the warning alarm
        Private wasteDepositTimer As New Timers.Timer() 'AG 01/12/2011 - controls the waste deposit removing system
        Private waterDepositTimer As New Timers.Timer() 'AG 01/12/2011 - controls the water deposit incoming system
        Private thermoR1ArmWarningTimer As New Timers.Timer() 'AG 05/01/2012- Controls thermo R1 arm maximum time allowed for the warning alarm
        Private thermoR2ArmWarningTimer As New Timers.Timer() 'AG 05/01/2012- Controls thermo R2 arm maximum time allowed for the warning alarm

        Private wellBaseLineWorker As New BackgroundWorker 'Worker to perform the well base line process C:\Documents and Settings\Sergio_Garcia\Mis documentos\Ax00_v1.0\FwScriptsManagement\Screen Delegates\PositionsAdjustmentDelegate.vb
        '#REFACTORING Private WithEvents baselineCalcs As BaseLineCalculations 'AG 29/04/2011 - Instance is created in AnalyzerManager constructor

        Private wellContaminatedWithWashSentAttr As Integer = 0
        Private myBarcodeRequestDS As New AnalyzerManagerDS 'AG 03/08/2011 - When barcode request has several records. Sw has to send: 1st row ... wait results, 2on row ... wait results and so on
        Private readBarCodeBeforeRunningPerformedFlag As Boolean = False 'AG 09/05/2012

        'AG 12/06/2012 - The photometric instructions are treated in a different threat in order to attend quickly to the analyzer requests in Running
        Private processingLastANSPHRInstructionFlag As Boolean = False
        Private bufferANSPHRReceived As New List(Of List(Of InstructionParameterTO)) 'AG 28/06/2012        

        'Recovery results variables
        Private bufferInstructionsRESULTSRECOVERYProcess As New List(Of List(Of InstructionParameterTO)) 'AG 27/08/2012 - buffer of instructions received (preparations with problems, results missing, ISE missing)
        Private recoveredPrepIDList As New List(Of Integer) 'AG 25/09/2012

        Private stopRequestedByUserInPauseModeFlag As Boolean = False 'AG 30/10/2013 - Task Task #1342. This flag is set to TRUE when user decides STOP the WorkSession
        '                                                   is used when analyzer is in running pause mode because in this case:
        '                                                   - Sw has to send START instruction + add END into queue


        ' XBC 28/10/2011 - timeout limit repetitions for Start Tasks
        Private numRepetitionsTimeout As Integer
        Private sendingRepetitions As Boolean
        Private waitingStartTaskTimer As New Timers.Timer()
        Private myStartTaskInstructionsQueue As New List(Of AnalyzerManagerSwActionList)
        Private myStartTaskParamsQueue As New List(Of Object)
        Private myStartTaskFwScriptIDsQueue As New List(Of String)
        Private myStartTaskFwScriptParamsQueue As New List(Of List(Of String))

        Private ISECMDLost As Boolean
        Private RUNNINGLost As Boolean
        Private numRepetitionsSTATE As Integer
        Private waitingSTATETimer As New Timers.Timer()

        Private InfoRefreshFirstTimeAttr As Boolean = True

        'provisional flag for starting ISE just the first time 
        Private ISEAlreadyStarted As Boolean = False

        Private myTmpConnectedAnalyzerDS As New AnalyzersDS


        Public Structure ErrorCodesDisplayStruct
            Public ErrorDateTime As DateTime
            Public ErrorCode As String
            Public ResourceID As String
            Public Solved As Boolean
        End Structure

        'This flag is required because if reconnect in running sometimes firmware sends 2 STATUS instruction, depends on the cycle machine moment where the connection is received
        Private runningConnectionPollSnSent As Boolean = False
        Private startingRunningFirstTimeAttr As Boolean = False
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
        Private AnalyzerStatusAttribute As AnalyzerManagerStatus = AnalyzerManagerStatus.NONE
        ' AG+XBC 24/05/2012

        Private AnalyzerCurrentActionAttribute As AnalyzerManagerAx00Actions = AnalyzerManagerAx00Actions.NO_ACTION

        Private InstructionSentAttribute As String = "" 'AG 20/10/2010
        Private InstructionReceivedAttribute As String = "" 'AG 20/10/2010
        Private InstructionTypeReceivedAttribute As AnalyzerManagerSwActionList

        Private myAlarmListAttribute As New List(Of AlarmEnumerates.Alarms) 'AG 16/03/2011
        Private analyzerFREEZEFlagAttribute As Boolean = False 'AG 31/03/2011 indicates the Fw has become into FREEZE mode (several error code indicates this mode)
        Private analyzerFREEZEModeAttribute As String = "" 'AG 02/03/2010 - indicates if the FREEZE is single element or if applies the whole instrument

        Private _analyzerSubStatusAttr As Boolean = False

        Private validALIGHTAttribute As Boolean = False 'AG - inform if exist an valid ALIGHT results (twksWSBLines table)
        Private existsALIGHTAttribute As Boolean = False 'AG 20/06/2012
        Private baselineInitializationFailuresAttribute As Integer = 0 'Alight base line initialization failures (used for repeat instructions or show messages)
        Private dynamicbaselineInitializationFailuresAttribute As Integer = 0 'AG 27/11/2014 BA-2066 Flight base line initialization failures (used for repeat instructions or show messages)
        Private WELLbaselineParametersFailuresAttribute As Boolean = False 'well base line parameters update failures (in Running) (used for show messages)



        'AG 01/04/2011 - Analyzer numerical values attributes (for inform the presentation is needed)
        Private SensorValuesAttribute As New Dictionary(Of AnalyzerSensors, Single)
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
        Private ManifoldValuesAttribute As New Dictionary(Of MANIFOLD_ELEMENTS, String)
        Private FluidicsValuesAttribute As New Dictionary(Of FLUIDICS_ELEMENTS, String)
        Private PhotometricsValuesAttribute As New Dictionary(Of PHOTOMETRICS_ELEMENTS, String)

        'XBC 08/06/2011
        Private CpuValuesAttribute As New Dictionary(Of CPU_ELEMENTS, String)

        'SGM 23/09/2011
        Private IsShutDownRequestedAttribute As Boolean = False
        Private IsUserConnectRequestedAttribute As Boolean = False

        'SGM 24/11/2011
        Private IsAutoInfoActivatedAttr As Boolean = False

        'SGM 28/11/2011
        Private FwVersionAttribute As String = ""

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

        ' XB 07/01/2014 - Create these new Attributes - BA-1178
        Private ShutDownisPendingAttr As Boolean = False
        Private StartSessionisPendingAttr As Boolean = False

        ' XB 09/01/2014 - Create this new Attribute - BA-2187
        Private LockISEAttr As Boolean = False

        Private IsAlreadyManagedAlarmsAttr As Boolean = False
#End Region

#Region "Properties"

        Public Property classInitializationError() As Boolean Implements IAnalyzerManager.classInitializationError '15/07/2011 AG
            Get
                Return classInitializationErrorAttribute
            End Get
            Set(ByVal value As Boolean)
                classInitializationErrorAttribute = value
            End Set
        End Property

        Public Property ActiveWorkSession() As String Implements IAnalyzerManager.ActiveWorkSession   '19/04/2010 AG
            Get
                Return WorkSessionIDAttribute
            End Get

            Set(ByVal value As String)
                WorkSessionIDAttribute = value
                AppLayer.currentWorkSessionID = WorkSessionIDAttribute
            End Set
        End Property

        Public ReadOnly Property WorkSessionStatus() As String Implements IAnalyzerManager.WorkSessionStatus  ' XBC 14/06/2012
            Get
                Return WorkSessionStatusAttribute
            End Get
        End Property


        Public Property ActiveAnalyzer() As String Implements IAnalyzerManager.ActiveAnalyzer  '19/04/2010 AG
            Get
                Return AnalyzerIDAttribute
            End Get

            Set(ByVal value As String)
                If AnalyzerIDAttribute <> value Then
                    Dim previousValue As String = AnalyzerIDAttribute 'AG 21/06/2012 - Previous analyzer id
                    AnalyzerIDAttribute = value

                    If value <> "" Then 'AG 20/11/2014 BA-2133
                        myAnalyzerModel = GetModelValue(AnalyzerIDAttribute)

                        'AG 20/03/2012 - inform about the current analyzer to the applayer
                        If Not AppLayer Is Nothing Then
                            AppLayer.currentAnalyzerID = AnalyzerIDAttribute
                            AppLayer.currentAnalyzerModel = myAnalyzerModel '13/11/2014 AG BA-2118
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
                        'REFACTORING
                        'If ISE_Manager Is Nothing Then
                        '    ISE_Manager = New ISEManager(Me, ActiveAnalyzer, myAnalyzerModel, True)
                        'End If

                        ''Initialize the FwAdjustments master data
                        'If FwVersionAttribute.Length > 0 Then
                        '    InitializeFWAdjustments(Nothing)
                        'End If

                        'AG 30/08/2013 - Bug # 1271 - when new analyzer is connected Sw has to clear all alarms related to the last analyzer connected
                        'The complete solution is next code but requires a futher validation because it could exists some exceptions (alarm not depending on the connected analyzer) - so remove
                        'only the alarms recalculated with data in DataBase (the base line alarms)
                        'myAlarmListAttribute.Clear()
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_INIT_ERR)
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_WELL_WARN) Then myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_WELL_WARN)
                        'AG 30/08/2013

                        ' XBC 14/06/2012
                        TemporalAnalyzerConnectedAttr = value
                        'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                        'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If Not GlobalBase.IsServiceAssembly Then
                            ' User Sw
                            InitBaseLineCalculations(Nothing, StartingApplicationAttr)
                        End If
                        ' XBC 14/06/2012
                    End If 'AG 20/11/2014 BA-2133

                End If
            End Set

        End Property

        Public Property ActiveFwVersion() As String Implements IAnalyzerManager.ActiveFwVersion  'SGM 28/11/2011
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
        Public Property ReadedFwVersion() As String Implements IAnalyzerManager.ReadedFwVersion  'SGM 28/11/2011
            Get
                Return ReadedFwVersionAttribute
            End Get

            Set(ByVal value As String)
                If ReadedFwVersionAttribute <> value Then
                    ReadedFwVersionAttribute = value
                End If
            End Set
        End Property

        Public Property CommThreadsStarted() As Boolean Implements IAnalyzerManager.CommThreadsStarted    '21/04/2010 AG
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

        Public Property Connected() As Boolean Implements IAnalyzerManager.Connected    '22/04/2010 AG
            Get

                If REAL_DEVELOPMENT_MODE > 0 Then
                    ConnectedAttribute = True
                End If


                Return ConnectedAttribute

            End Get

            Set(ByVal value As Boolean)
                ConnectedAttribute = value
            End Set
        End Property

        'SGM 22/09/2011
        Public Property IsShutDownRequested() As Boolean Implements IAnalyzerManager.IsShutDownRequested
            Get
                Return IsShutDownRequestedAttribute
            End Get
            Set(ByVal value As Boolean)
                IsShutDownRequestedAttribute = value
            End Set
        End Property

        'SGM 22/09/2011
        Public Property IsUserConnectRequested() As Boolean Implements IAnalyzerManager.IsUserConnectRequested
            Get
                Return IsUserConnectRequestedAttribute
            End Get
            Set(ByVal value As Boolean)
                IsUserConnectRequestedAttribute = value
            End Set
        End Property

        Public Property PortName() As String Implements IAnalyzerManager.PortName  '22/04/2010 AG
            Get
                Return PortNameAttribute
            End Get

            Set(ByVal value As String)
                PortNameAttribute = value
            End Set
        End Property

        Public Property Bauds() As String Implements IAnalyzerManager.Bauds  '22/04/2010 AG
            Get
                Return BaudsAttribute
            End Get

            Set(ByVal value As String)
                BaudsAttribute = value
            End Set
        End Property

        Public Property AnalyzerIsReady() As Boolean Implements IAnalyzerManager.AnalyzerIsReady    '19/05/2010 AG
            Get
                Return AnalyzerIsReadyAttribute
            End Get

            Set(ByVal value As Boolean)
                AnalyzerIsReadyAttribute = value
            End Set
        End Property

        Public Property AnalyzerStatus() As AnalyzerManagerStatus Implements IAnalyzerManager.AnalyzerStatus 'AG 01/06/2010
            Get
                Return AnalyzerStatusAttribute
            End Get
            Set(ByVal value As AnalyzerManagerStatus)
                If AnalyzerStatusAttribute <> value Then
                    AnalyzerStatusAttribute = value
                End If
            End Set
        End Property

        Public Property AnalyzerCurrentAction() As AnalyzerManagerAx00Actions Implements IAnalyzerManager.AnalyzerCurrentAction   'AG 01/06/2010
            Get
                Return AnalyzerCurrentActionAttribute
            End Get
            Set(ByVal value As AnalyzerManagerAx00Actions)
                AnalyzerCurrentActionAttribute = value
            End Set
        End Property

        Public Property InstructionSent() As String Implements IAnalyzerManager.InstructionSent    '20/10/2010 AG
            Get
                Return InstructionSentAttribute
            End Get

            Set(ByVal value As String)
                InstructionSentAttribute = value
            End Set
        End Property

        Public Property InstructionReceived() As String Implements IAnalyzerManager.InstructionReceived    '20/10/2010 AG
            Get
                Return InstructionReceivedAttribute
            End Get

            Set(ByVal value As String)
                InstructionReceivedAttribute = value
            End Set
        End Property

        Public Property InstructionTypeReceived() As AnalyzerManagerSwActionList Implements IAnalyzerManager.InstructionTypeReceived    '02/11/2010 AG
            Get
                Return InstructionTypeReceivedAttribute
            End Get

            Set(ByVal value As AnalyzerManagerSwActionList)
                InstructionTypeReceivedAttribute = value
            End Set
        End Property

        Public Property InstructionTypeSent() As AppLayerEventList Implements IAnalyzerManager.InstructionTypeSent    '27/03/2012 AG
            Get
                Return AppLayer.LastInstructionTypeSent
            End Get
            Set(value As AppLayerEventList)
                AppLayer.LastInstructionTypeSent = value
            End Set
        End Property

        Public Property ISEModuleIsReady() As Boolean Implements IAnalyzerManager.ISEModuleIsReady    '18/01/2011 AG
            Get
                Return ISEModuleIsReadyAttribute
            End Get

            Set(ByVal value As Boolean)
                ISEModuleIsReadyAttribute = value
            End Set
        End Property

        Public ReadOnly Property Alarms() As List(Of AlarmEnumerates.Alarms) Implements IAnalyzerManager.Alarms
            Get
                Return myAlarmListAttribute
            End Get
        End Property

        ' XBC 16/10/2012
        Public ReadOnly Property ErrorCodes() As String Implements IAnalyzerManager.ErrorCodes
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
        Public Property IsServiceAlarmInformed() As Boolean Implements IAnalyzerManager.IsServiceAlarmInformed
            Get
                Return IsServiceAlarmInformedAttr
            End Get
            Set(ByVal value As Boolean)
                IsServiceAlarmInformedAttr = value
            End Set
        End Property

        'SGM 09/11/2012 SERVICE
        Public Property IsServiceRotorMissingInformed() As Boolean Implements IAnalyzerManager.IsServiceRotorMissingInformed
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
        Public Property IsFwUpdateInProcess() As Boolean Implements IAnalyzerManager.IsFwUpdateInProcess
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
        Public Property IsConfigGeneralProcess() As Boolean Implements IAnalyzerManager.IsConfigGeneralProcess
            Get
                Return IsConfigGeneralProcessAttr
            End Get
            Set(ByVal value As Boolean)
                If value <> IsConfigGeneralProcessAttr Then
                    IsConfigGeneralProcessAttr = value
                End If
            End Set
        End Property

        Public Property AnalyzerIsFreeze() As Boolean Implements IAnalyzerManager.AnalyzerIsFreeze
            Get
                Return analyzerFREEZEFlagAttribute
            End Get
            Set(value As Boolean)
                analyzerFREEZEFlagAttribute = value
            End Set
        End Property

        ''' <summary>
        ''' SubStatus examples: 551, 552
        ''' </summary>
        Public Property AnalyzerHasSubStatus() As Boolean Implements IAnalyzerManager.AnalyzerHasSubStatus
            Get
                Return _analyzerSubStatusAttr
            End Get
            Set(value As Boolean)
                _analyzerSubStatusAttr = value
            End Set
        End Property

        Public Property AnalyzerFreezeMode() As String Implements IAnalyzerManager.AnalyzerFreezeMode
            Get
                Return analyzerFREEZEModeAttribute
            End Get
            Set(value As String)
                analyzerFREEZEModeAttribute = value
            End Set
        End Property
        Public Property SessionFlag(ByVal pFlag As AnalyzerManagerFlags) As String Implements IAnalyzerManager.SessionFlag  '19/04/2010 AG
            Get
                If mySessionFlags.ContainsKey(pFlag.ToString) Then
                    Return mySessionFlags(pFlag.ToString)
                Else
#If config = "Debug" Then
                    Debug.WriteLine("Attention!! Session flag " & pFlag & " was not found.")
#End If
                    Return String.Empty
                End If
            End Get

            Set(ByVal pValue As String)
                mySessionFlags(pFlag.ToString) = pValue
            End Set
        End Property

        Public ReadOnly Property GetSensorValue(ByVal pSensorID As AnalyzerSensors) As Single Implements IAnalyzerManager.GetSensorValue
            Get
                If SensorValuesAttribute.ContainsKey(pSensorID) Then
                    Return SensorValuesAttribute(pSensorID)
                Else
                    'AG 30/03/2012 - several sensors have a unknown value because Nothing = 0 means alarm)
                    'Return Nothing
                    If pSensorID = AnalyzerSensors.FRIDGE_STATUS OrElse pSensorID = AnalyzerSensors.ISE_STATUS Then
                        Return SENSORUNKNOWNVALUE
                    Else
                        Return Nothing
                    End If
                    'AG 30/03/2012
                End If

            End Get
        End Property

        Public WriteOnly Property SetSensorValue(ByVal pSensorID As AnalyzerSensors) As Single Implements IAnalyzerManager.SetSensorValue
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

        Public Property MaxWaitTime() As Integer Implements IAnalyzerManager.MaxWaitTime
            Get
                Return AppLayer.MaxWaitTime
            End Get
            Set(value As Integer)
                AppLayer.MaxWaitTime = value
            End Set
        End Property


        ''' <summary>
        ''' ALIGHT fails or FLIGHT fails (all tentatives)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Modify AG 27/11/2014 BA-2066</remarks>
        Public ReadOnly Property ShowBaseLineInitializationFailedMessage() As Boolean Implements IAnalyzerManager.ShowBaseLineInitializationFailedMessage
            Get
                Return (CBool(baselineInitializationFailuresAttribute >= ALIGHT_INIT_FAILURES) OrElse CBool(dynamicbaselineInitializationFailuresAttribute >= FLIGHT_INIT_FAILURES))
            End Get

        End Property


        Public ReadOnly Property ShowBaseLineWellRejectionParameterFailedMessage() As Boolean Implements IAnalyzerManager.ShowBaseLineWellRejectionParameterFailedMessage
            Get
                Dim result As Boolean = CBool(IIf(AnalyzerStatusAttribute <> GlobalEnumerates.AnalyzerManagerStatus.RUNNING And WELLbaselineParametersFailuresAttribute, True, False))
                If result Then WELLbaselineParametersFailuresAttribute = False 'Once the alarm is shown clear this flag

                Return result
            End Get

        End Property

        Public ReadOnly Property SensorValueChanged() As UIRefreshDS.SensorValueChangedDataTable Implements IAnalyzerManager.SensorValueChanged
            Get
                Return Me.myUI_RefreshDS.SensorValueChanged
            End Get
        End Property

        Public ReadOnly Property ReceivedAlarms() As UIRefreshDS.ReceivedAlarmsDataTable Implements IAnalyzerManager.ReceivedAlarms
            Get
                Return myUI_RefreshDS.ReceivedAlarms
            End Get
        End Property

        Public ReadOnly Property ValidALIGHT() As Boolean Implements IAnalyzerManager.ValidALIGHT    '20/05/2011 AG
            Get
                Return validALIGHTAttribute
            End Get
        End Property

        Public ReadOnly Property ExistsALIGHT() As Boolean Implements IAnalyzerManager.ExistsALIGHT    '20/06/2012 AG
            Get
                Return existsALIGHTAttribute
            End Get
        End Property

        Public Property CurrentWell() As Integer Implements IAnalyzerManager.CurrentWell
            Get
                Return CurrentWellAttribute
            End Get
            Set(ByVal value As Integer)
                CurrentWellAttribute = value
            End Set
        End Property

        Public Property BarCodeProcessBeforeRunning() As BarcodeWorksessionActionsEnum Implements IAnalyzerManager.BarCodeProcessBeforeRunning 'AG 04/08/2011
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


        Public ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String Implements IAnalyzerManager.GetModelValue
            Get
                'AG 10/11/2014 BA-2082 pending to adapt for compatibility between BA200 and BA400
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    Dim strTocompare As String

                    strTocompare = GetUpperPartSN(pAnalyzerID)

                    Select Case strTocompare
                        Case "SN0"  ' Generic
                            returnValue = "A200"

                        Case GlobalBase.BA400ModelID
                            returnValue = "A200"
                    End Select
                End If

                Return returnValue
            End Get
        End Property

        ' XBC 07/06/2012
        Public ReadOnly Property GetUpperPartSN(ByVal pAnalyzerID As String) As String Implements IAnalyzerManager.GetUpperPartSN
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    returnValue = pAnalyzerID.Substring(0, GlobalBase.SNUpperPartSize)
                End If

                Return returnValue
            End Get
        End Property

        ' XBC 07/06/2012
        Public ReadOnly Property GetLowerPartSN(ByVal pAnalyzerID As String) As String Implements IAnalyzerManager.GetLowerPartSN
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then
                    returnValue = pAnalyzerID.Substring(GlobalBase.SNUpperPartSize, GlobalBase.SNLowerPartSize)
                End If

                Return returnValue
            End Get
        End Property


        Public Property Ringing() As Boolean Implements IAnalyzerManager.Ringing
            Get
                Return AnalyzerIsRingingAttribute
            End Get
            Set(ByVal value As Boolean)
                AnalyzerIsRingingAttribute = value
            End Set
        End Property


        'SGM 24/11/2011
        Public Property IsAutoInfoActivated() As Boolean Implements IAnalyzerManager.IsAutoInfoActivated
            Get
                Return IsAutoInfoActivatedAttr
            End Get
            Set(ByVal value As Boolean)
                IsAutoInfoActivatedAttr = value
            End Set
        End Property

        'SGM 26/10/2012 -SERVICE: INFO:Q2 is sent. wait for ANSERR
        Public Property IsAlarmInfoRequested() As Boolean Implements IAnalyzerManager.IsAlarmInfoRequested
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
        Public Property IsInstructionRejected() As Boolean Implements IAnalyzerManager.IsInstructionRejected
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
        Public Property IsRecoverFailed() As Boolean Implements IAnalyzerManager.IsRecoverFailed
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
        Public Property IsInstructionAborted() As Boolean Implements IAnalyzerManager.IsInstructionAborted
            Get
                Return IsInstructionAbortedAttr
            End Get
            Set(ByVal value As Boolean)
                If IsInstructionAbortedAttr <> value Then
                    IsInstructionAbortedAttr = value
                End If
            End Set
        End Property

        Public Property LevelDetected() As Boolean Implements IAnalyzerManager.LevelDetected
            Get
                Return (LevelDetectedAttr > 0)
            End Get
            Set(ByVal value As Boolean)
                LevelDetectedAttr = -1
            End Set
        End Property

        Public Property EndRunInstructionSent() As Boolean Implements IAnalyzerManager.EndRunInstructionSent
            Get
                Return endRunAlreadySentFlagAttribute
            End Get
            Set(ByVal value As Boolean)
                endRunAlreadySentFlagAttribute = value
            End Set
        End Property

        Public Property AbortInstructionSent() As Boolean Implements IAnalyzerManager.AbortInstructionSent
            Get
                Return abortAlreadySentFlagAttribute
            End Get
            Set(ByVal value As Boolean)
                abortAlreadySentFlagAttribute = value
            End Set
        End Property

        Public Property RecoverInstructionSent() As Boolean Implements IAnalyzerManager.RecoverInstructionSent
            Get
                Return recoverAlreadySentFlagAttribute
            End Get
            Set(ByVal value As Boolean)
                recoverAlreadySentFlagAttribute = value
            End Set
        End Property

        ' XB 15/10/2013 - BT #1318
        Public Property PauseInstructionSent() As Boolean Implements IAnalyzerManager.PauseInstructionSent
            Get
                Return PauseAlreadySentFlagAttribute
            End Get
            Set(ByVal value As Boolean)
                PauseAlreadySentFlagAttribute = value
            End Set
        End Property

        'TR 25/10/2013 BT #1340
        Public ReadOnly Property ContinueAlreadySentFlag() As Boolean Implements IAnalyzerManager.ContinueAlreadySentFlag
            Get
                Return ContinueAlreadySentFlagAttribute
            End Get
        End Property

        Public Property InfoActivated() As Integer Implements IAnalyzerManager.InfoActivated 'AG 14/03/2012
            Get
                Return AnalyzerIsInfoActivatedAttribute
            End Get
            Set(ByVal value As Integer)
                AnalyzerIsInfoActivatedAttribute = value
            End Set
        End Property

        ' XBC 03/05/2012
        Public Property IsStressing() As Boolean Implements IAnalyzerManager.IsStressing
            Get
                Return IsStressingAttribute
            End Get
            Set(ByVal value As Boolean)
                IsStressingAttribute = value
            End Set
        End Property


        'SGM 30/05/2012
        Public Property IsFwSwCompatible() As Boolean Implements IAnalyzerManager.IsFwSwCompatible
            Get
                Return IsFwSwCompatibleAttr
            End Get
            Set(ByVal value As Boolean)
                IsFwSwCompatibleAttr = value
            End Set
        End Property

        'SGM 21/06/2012
        Public Property IsFwReaded() As Boolean Implements IAnalyzerManager.IsFwReaded
            Get
                Return IsFwReadedAttr
            End Get
            Set(ByVal value As Boolean)
                IsFwReadedAttr = value
            End Set
        End Property

        'AG 07/06/2012 - Informs that well base line calculations has paused the work session (consecutive rejected wells > Limit)
        Public ReadOnly Property wellBaseLineAutoPausesSession() As Integer Implements IAnalyzerManager.wellBaseLineAutoPausesSession
            Get
                Return BaseLine.exitRunningType
            End Get
        End Property

        ' XBC 21/06/2012
        Public Property IsAnalyzerIDNotLikeWS() As Boolean Implements IAnalyzerManager.IsAnalyzerIDNotLikeWS
            Get
                Return IsAnalyzerIDNotLikeWSAttr
            End Get
            Set(ByVal value As Boolean)
                IsAnalyzerIDNotLikeWSAttr = value
            End Set
        End Property

        ' XBC 14/06/2012
        Public Property StartingApplication() As Boolean Implements IAnalyzerManager.StartingApplication
            Get
                Return StartingApplicationAttr
            End Get
            Set(ByVal value As Boolean)
                StartingApplicationAttr = value
            End Set
        End Property

        ' XCB 18/06/2012
        Public ReadOnly Property TemporalAnalyzerConnected() As String Implements IAnalyzerManager.TemporalAnalyzerConnected
            Get
                Return TemporalAnalyzerConnectedAttr
            End Get
        End Property

        ' XBC 19/06/2012
        Public Property ForceAbortSession() As Boolean Implements IAnalyzerManager.ForceAbortSession
            Get
                Return ForceAbortSessionAttr
            End Get
            Set(ByVal value As Boolean)
                ForceAbortSessionAttr = value
            End Set
        End Property

        ' XBC 02/08/2012
        Public Property DifferentWSType() As String Implements IAnalyzerManager.DifferentWSType
            Get
                Return DifferentWSTypeAtrr
            End Get
            Set(ByVal value As String)
                DifferentWSTypeAtrr = value
            End Set
        End Property

        ' XBC 03/10/2012
        Public Property AdjustmentsRead() As Boolean Implements IAnalyzerManager.AdjustmentsRead
            Get
                Return AdjustmentsReadAttr
            End Get
            Set(ByVal value As Boolean)
                AdjustmentsReadAttr = value
            End Set
        End Property

        'SGM 10/10/2012 - SERVICE
        Public Property TestingCollidedNeedle() As UTILCollidedNeedles Implements IAnalyzerManager.TestingCollidedNeedle
            Get
                Return TestingCollidedNeedleAttribute
            End Get
            Set(ByVal value As UTILCollidedNeedles)
                TestingCollidedNeedleAttribute = value
            End Set
        End Property

        'SGM 10/10/2012 - SERVICE
        Public Property SaveSNResult() As UTILSNSavedResults Implements IAnalyzerManager.SaveSNResult
            Get
                Return SaveSNResultAttribute
            End Get
            Set(ByVal value As UTILSNSavedResults)
                SaveSNResultAttribute = value
            End Set
        End Property

        ' XBC 07/11/2012 - SERVICE
        Public ReadOnly Property ErrorCodesDisplay() As List(Of ErrorCodesDisplayStruct) Implements IAnalyzerManager.ErrorCodesDisplay
            Get
                Return myErrorCodesDisplayAttribute
            End Get
        End Property

        'SGM 08/11/2012 SERVICE for refreshing sensors monitor
        Public Property InfoRefreshFirstTime() As Boolean Implements IAnalyzerManager.InfoRefreshFirstTime
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
        Public ReadOnly Property LastExportedResults() As ExecutionsDS Implements IAnalyzerManager.LastExportedResults
            Get
                SyncLock LockThis
                    Return lastExportedResultsDSAttribute
                End SyncLock
            End Get
        End Property
        'AG 19/03/2013

        Public WriteOnly Property autoWSCreationWithLISMode() As Boolean Implements IAnalyzerManager.autoWSCreationWithLISMode 'AG 11/07/2013
            Set(ByVal value As Boolean)
                autoWSCreationWithLISModeAttribute = value
            End Set
        End Property

        Public ReadOnly Property AllowScanInRunning() As Boolean Implements IAnalyzerManager.AllowScanInRunning
            Get
                Return AllowScanInRunningAttribute
            End Get
        End Property

        ' XB 29/01/2014 - Task #1438
        Public Property BarcodeStartInstrExpected As Boolean Implements IAnalyzerManager.BarcodeStartInstrExpected
            Get
                Return BarcodeStartInstrExpectedAttr
            End Get
            Set(ByVal value As Boolean)
                BarcodeStartInstrExpectedAttr = value
            End Set
        End Property

        ' XB 07/01/2014 - Create these new Properties - BA-1178
        Public Property ShutDownisPending As Boolean Implements IAnalyzerManager.ShutDownisPending
            Get
                Return ShutDownisPendingAttr
            End Get
            Set(value As Boolean)
                ShutDownisPendingAttr = value
            End Set
        End Property

        Public Property StartSessionisPending As Boolean Implements IAnalyzerManager.StartSessionisPending
            Get
                Return StartSessionisPendingAttr
            End Get
            Set(value As Boolean)
                StartSessionisPendingAttr = value
            End Set
        End Property
        ' XB 07/01/2014 - BA-1178

        ' XB 09/01/2014 - Create this new Property - BA-2187
        Public Property LockISE As Boolean Implements IAnalyzerManager.LockISE
            Get
                Return LockISEAttr
            End Get
            Set(value As Boolean)
                LockISEAttr = value
            End Set
        End Property

        'SGM 29/05/2012
        Public Property FWUpdateResponseData As FWUpdateResponseTO Implements IAnalyzerManager.FWUpdateResponseData '#REFACTORING
        Public Property AdjustmentsFilePath As String Implements IAnalyzerManager.AdjustmentsFilePath '#REFACTORING
        <DefaultValue(InstructionActions.None)>
        Public Property CurrentInstructionAction As InstructionActions Implements IAnalyzerManager.CurrentInstructionAction 'BA-2075

        'IT 19/12/2014 - BA-2143
        Public Property FlightInitFailures As Integer Implements IAnalyzerManager.FlightInitFailures
            Get
                Return FLIGHT_INIT_FAILURES
            End Get
            Set(ByVal value As Integer)
                FLIGHT_INIT_FAILURES = value
            End Set
        End Property

        'IT 19/12/2014 - BA-2143
        Public Property DynamicBaselineInitializationFailures As Integer Implements IAnalyzerManager.DynamicBaselineInitializationFailures
            Get
                Return dynamicbaselineInitializationFailuresAttribute
            End Get
            Set(ByVal value As Integer)
                dynamicbaselineInitializationFailuresAttribute = value
            End Set
        End Property

        Public Property ISECMDStateIsLost As Boolean Implements IAnalyzerManager.ISECMDStateIsLost
            Get
                Return ISECMDLost
            End Get
            Set(ByVal value As Boolean)
                ISECMDLost = value
            End Set
        End Property

        Public Property CanSendingRepetitions As Boolean Implements IAnalyzerManager.CanSendingRepetitions
            Get
                Return sendingRepetitions
            End Get
            Set(ByVal value As Boolean)
                sendingRepetitions = value
            End Set
        End Property

        Public Property NumSendingRepetitionsTimeout As Integer Implements IAnalyzerManager.NumSendingRepetitionsTimeout
            Get
                Return numRepetitionsTimeout
            End Get
            Set(ByVal value As Integer)
                numRepetitionsTimeout = value
            End Set
        End Property

        Public Property WaitingStartTaskTimerEnabled As Boolean Implements IAnalyzerManager.WaitingStartTaskTimerEnabled
            Get
                Return waitingStartTaskTimer.Enabled
            End Get
            Set(ByVal value As Boolean)
                waitingStartTaskTimer.Enabled = value
            End Set
        End Property

        Public Property SetTimeISEOffsetFirstTime As Boolean Implements IAnalyzerManager.SetTimeISEOffsetFirstTime
            Get
                Return timeISEOffsetFirstTime
            End Get
            Set(ByVal value As Boolean)
                timeISEOffsetFirstTime = value
            End Set
        End Property

        Public ReadOnly Property ConnectedPortName() As String Implements IAnalyzerManager.ConnectedPortName
            Get
                Return AppLayer.ConnectedPortName
            End Get
        End Property

        Public ReadOnly Property ConnectedBauds() As String Implements IAnalyzerManager.ConnectedBauds
            Get
                Return AppLayer.ConnectedBauds
            End Get
        End Property

        Public Property RecoveryResultsInPause() As Boolean Implements IAnalyzerManager.RecoveryResultsInPause
            Get
                Return AppLayer.RecoveryResultsInPause
            End Get
            Set(value As Boolean)
                AppLayer.RecoveryResultsInPause = value
            End Set
        End Property

        Public Property RunningLostState As Boolean Implements IAnalyzerManager.RunningLostState
            Get
                Return RUNNINGLost
            End Get
            Set(value As Boolean)
                RUNNINGLost = value
            End Set
        End Property

        Public Property NumRepetitionsStateInstruction As Integer Implements IAnalyzerManager.NumRepetitionsStateInstruction
            Get
                Return numRepetitionsSTATE
            End Get
            Set(value As Integer)
                numRepetitionsSTATE = value
            End Set
        End Property

        Public Property RunningConnectionPollSnSentStatus As Boolean Implements IAnalyzerManager.RunningConnectionPollSnSentStatus
            Get
                Return runningConnectionPollSnSent
            End Get
            Set(value As Boolean)
                runningConnectionPollSnSent = value
            End Set
        End Property

        Public Property IseIsAlreadyStarted() As Boolean Implements IAnalyzerManager.IseIsAlreadyStarted
            Get
                Return ISEAlreadyStarted
            End Get
            Set(value As Boolean)
                ISEAlreadyStarted = value
            End Set
        End Property

        Public ReadOnly Property BufferANSPHRReceivedCount() As Integer Implements IAnalyzerManager.BufferANSPHRReceivedCount
            Get
                Return bufferANSPHRReceived.Count
            End Get
        End Property

        Public Property ProcessingLastANSPHRInstructionStatus() As Boolean Implements IAnalyzerManager.ProcessingLastANSPHRInstructionStatus
            Get
                Return processingLastANSPHRInstructionFlag
            End Get
            Set(value As Boolean)
                processingLastANSPHRInstructionFlag = value
            End Set
        End Property

        Public Property StartUseRequestFlag() As Boolean Implements IAnalyzerManager.StartUseRequestFlag
            Get
                Return useRequestFlag
            End Get
            Set(value As Boolean)
                useRequestFlag = value
            End Set
        End Property

        Public ReadOnly Property StartTaskInstructionsQueueCount() As Integer Implements IAnalyzerManager.StartTaskInstructionsQueueCount
            Get
                Return myStartTaskInstructionsQueue.Count
            End Get
        End Property

        Public Property ThermoReactionsRotorWarningTimerEnabled() As Boolean Implements IAnalyzerManager.ThermoReactionsRotorWarningTimerEnabled
            Get
                Return thermoReactionsRotorWarningTimer.Enabled
            End Get
            Set(value As Boolean)
                thermoReactionsRotorWarningTimer.Enabled = value
            End Set
        End Property

        Public ReadOnly Property PauseModeIsStartingState() As Boolean Implements IAnalyzerManager.PauseModeIsStartingState
            Get
                Return pauseModeIsStarting
            End Get
        End Property

        Public Property PauseSendingTestPreparations() As Boolean Implements IAnalyzerManager.PauseSendingTestPreparations
            Get
                Return pauseSendingTestPreparationsFlag
            End Get
            Set(value As Boolean)
                pauseSendingTestPreparationsFlag = value
            End Set
        End Property

        Public ReadOnly Property BaselineInitializationFailures As Integer Implements IAnalyzerManager.BaselineInitializationFailures
            Get
                Return baselineInitializationFailuresAttribute
            End Get
        End Property

        Public Property WELLbaselineParametersFailures As Boolean Implements IAnalyzerManager.WELLbaselineParametersFailures
            Get
                Return WELLbaselineParametersFailuresAttribute
            End Get
            Set(value As Boolean)
                WELLbaselineParametersFailuresAttribute = value
            End Set
        End Property

        Public Property FutureRequestNextWellValue As Integer Implements IAnalyzerManager.FutureRequestNextWellValue
            Get
                Return futureRequestNextWell
            End Get
            Set(value As Integer)
                futureRequestNextWell = value
            End Set
        End Property

        Public ReadOnly Property FieldLimits As FieldLimitsDS.tfmwFieldLimitsDataTable Implements IAnalyzerManager.FieldLimits
            Get
                Return myClassFieldLimitsDS.tfmwFieldLimits
            End Get
        End Property

        Public ReadOnly Property SentPreparations As AnalyzerManagerDS.sentPreparationsDataTable Implements IAnalyzerManager.SentPreparations
            Get
                Return mySentPreparationsDS.sentPreparations
            End Get
        End Property

        Public ReadOnly Property NextPreparationsToSend As AnalyzerManagerDS.nextPreparationDataTable Implements IAnalyzerManager.NextPreparationsToSend
            Get
                Return myNextPreparationToSendDS.nextPreparation
            End Get
        End Property

        Public Property NextPreparationsAnalyzerManagerDS As AnalyzerManagerDS Implements IAnalyzerManager.NextPreparationsAnalyzerManagerDS
            Get
                Return myNextPreparationToSendDS
            End Get
            Set(value As AnalyzerManagerDS)
                myNextPreparationToSendDS = value
            End Set
        End Property

        Public Property wellContaminatedWithWashSent As Integer Implements IAnalyzerManager.wellContaminatedWithWashSent
            Get
                Return wellContaminatedWithWashSentAttr
            End Get
            Set(value As Integer)
                wellContaminatedWithWashSentAttr = value
            End Set
        End Property

        Public Property CanManageRetryAlarm As Boolean Implements IAnalyzerManager.CanManageRetryAlarm
            Get
                Return IsAlreadyManagedAlarmsAttr
            End Get
            Set(value As Boolean)
                IsAlreadyManagedAlarmsAttr = value
            End Set
        End Property

        Public Property StartingRunningFirstTime As Boolean Implements IAnalyzerManager.StartingRunningFirstTime
            Get
                Return startingRunningFirstTimeAttr
            End Get
            Set(value As Boolean)
                startingRunningFirstTimeAttr = value
            End Set
        End Property

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
                                           ByVal pUIRefresh_Event As List(Of UI_RefreshEvents), ByVal pUI_RefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Implements IAnalyzerManager.ReceptionEvent '#REFACTORING
        ''' <summary>
        ''' Event with the instruction sent
        ''' </summary>
        ''' <param name="pInstructionSent"></param>
        ''' <remarks>AG 29/10/2010</remarks>
        Public Event SendEvent(ByVal pInstructionSent As String) Implements IAnalyzerManager.SendEvent '#REFACTORING

        ''' <summary>
        ''' Event to activate/deactivate WatchDog timer - Task #1438
        ''' </summary>
        ''' <param name="pEnable"></param>
        ''' <remarks>XB 29/01/2014</remarks>

        Public Event WatchDogEvent(ByVal pEnable As Boolean) Implements IAnalyzerManager.WatchDogEvent '#REFACTORING

        'AG 20/04/2010
        ''' <summary>
        ''' Event from ApplicationLayer
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <remarks>
        ''' Creation AG 20/04/2010
        ''' </remarks>
        Public Sub OnManageAnalyzerEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal pInstructionReceived As List(Of InstructionParameterTO)) Handles AppLayer.ManageAnalyzer
            Try

                ManageAnalyzer(pAction, False, pInstructionReceived)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageAnalyzerEvent", EventLogEntryType.Error, False)
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
                myGlobalDataTO = myMessageDelegate.GetMessageDescription(Nothing, Messages.ERROR_COMM.ToString(), "ENG")
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As New MessagesDS
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        TextMessage = myMessagesDS.tfmwMessages(0).MessageText
                    End If
                End If

                GlobalBase.CreateLogActivity(TextMessage, "AnalyzerManager.OnManageCommunicateErrorEvent", EventLogEntryType.Information, False)
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageCommunicateErrorEvent", EventLogEntryType.Error, False)
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

                Debug.Print("SALTA TIMER WAITING_TIME_EXPIRED !!!!")
                ManageAnalyzer(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED, True)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.waitingTimer_Timer", EventLogEntryType.Error, False)
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
                InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                ClearQueueToSend()

                ManageAnalyzer(AnalyzerManagerSwActionList.START_TASK_TIMEOUT, True)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.waitingStartTaskTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Manage the waiting time for STATE instruction
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Creation XB 05/11/2014 - timeout limit repetitions for STATE
        ''' </remarks>
        Private Sub waitingSTATETimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Try
                'Deactivate waiting time control
                InitializeTimerSTATEControl(WAITING_TIME_OFF)
                numRepetitionsSTATE += 1

                Debug.Print("SALTA TIMER STATE !!!!")
                ManageAnalyzer(AnalyzerManagerSwActionList.START_TASK_TIMEOUT, True)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.waitingSTATETimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub


        ''' <summary>
        ''' Get the event generated by the base line well calculations
        ''' </summary>
        ''' <param name="pReactionsRotorWellDS"></param>
        ''' <param name="pFromDynamicBaseLineProcessingFlag">TRUE when the event is raised after process dynamic base line results</param>
        ''' <remarks>AG 20/05/2011 - creation
        ''' AG 13/09/2012 - only in Running, during well base line (not in standby when treat the ansphr queue
        ''' AG 28/11/2014 BA.2081 in standby the refresh information has to be prepared after dynamic base line processing</remarks>
        Public Sub OnManageWellReactionsChanges(ByVal pReactionsRotorWellDS As ReactionsRotorDS, ByVal pFromDynamicBaseLineProcessingFlag As Boolean) Handles _baseLine.WellReactionsChanges
            Try
                If (AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING OrElse pFromDynamicBaseLineProcessingFlag) AndAlso pReactionsRotorWellDS.twksWSReactionsRotor.Rows.Count > 0 Then
                    Dim myGlobal = PrepareUIRefreshEventNum3(Nothing, UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, pReactionsRotorWellDS, True) 'AG 12/06/2012 'False)
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnManageWellReactionsChanges", EventLogEntryType.Error, False)
            End Try
        End Sub


        ' SERVICE SOFTWARE
        ' XBC 16/11/2010
        Public Event ReceptionFwScriptEvent(ByVal pDataReceived As String, _
                                                   ByVal pResponseValue As String, _
                                                   ByVal pTreated As Boolean) Implements IAnalyzerManager.ReceptionFwScriptEvent

        ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
        ' SGM 15/04/2011 
        'Public Shared Event SensorValuesChangedEvent(ByVal pSensorValuesChangedDT As UIRefreshDS.SensorValueChangedDataTable)

        Public Event ReceivedStatusInformationEventHandler() Implements IAnalyzerManager.ReceivedStatusInformationEventHandler 'IT 19/12/2014 - BA-2143
        Public Event ProcessFlagEventHandler(ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags) Implements IAnalyzerManager.ProcessFlagEventHandler 'IT 19/12/2014 - BA-2143

        ''' <summary>
        ''' Detects that the ISE data aimed to be displayed in the Monitor has been changed
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEMonitorDataChanged() Handles _iseAnalyzer.ISEMonitorDataChanged
            Try
                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_MONITOR_DATA_CHANGED, 1, True)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEOperationFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE is ready for ISE Preparations
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEReadyChanged() Handles _iseAnalyzer.ISEReadyChanged
            Try
                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_READY_CHANGED, 1, True)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEReadyChanged", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has been switched On
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISESwitchedOnChanged() Handles _iseAnalyzer.ISESwitchedOnChanged
            Try
                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_SWITCHON_CHANGED, 1, True)
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISESwitchedOnChanged", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has finished the initial data retrieving process (Ok or not)
        ''' </summary>
        ''' <remarks>
        ''' Created by SGM 15/03/2012
        ''' Modified by XB 21/01/2015 - Refresh Alarms about ISE calibrations and clean - BA-1873
        ''' </remarks>
        Public Sub OnISEConnectionFinished(ByVal pOk As Boolean) Handles _iseAnalyzer.ISEConnectionFinished
            Try
                Dim myGlobal As New GlobalDataTO
                Dim myValue As Integer = 0

                Dim myAlarmListTmp As New List(Of AlarmEnumerates.Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobal = ISEAnalyzer.CheckAlarms(Connected, myAlarmList, myAlarmStatusList)

                If pOk Then
                    myValue = 1

                    'The ISE Module was switched On and the user has connected from Utilities

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                    myAlarmStatusList.Add(False)

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_OFF_ERR)
                    myAlarmStatusList.Add(False)

                Else
                    myValue = 2

                    'The ISE Module was switched On but the connection is failed and then is pending
                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                    myAlarmStatusList.Add(True)

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_OFF_ERR)
                    myAlarmStatusList.Add(False)
                End If

                If ISEAnalyzer.IsISEModuleInstalled And ISEAnalyzer.IsISEModuleReady Then
                    ' Check if ISE Electrodes calibration is required
                    Dim electrodesCalibrationRequired As Boolean = False
                    myGlobal = ISEAnalyzer.CheckElectrodesCalibrationIsNeeded()
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        electrodesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                    End If

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CALB_PDT_WARN)
                    If electrodesCalibrationRequired Then
                        myAlarmStatusList.Add(True)
                    Else
                        myAlarmStatusList.Add(False)
                    End If

                    ' Check if ISE Pumps calibration is required
                    Dim pumpsCalibrationRequired As Boolean = False
                    myGlobal = ISEAnalyzer.CheckPumpsCalibrationIsNeeded
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        pumpsCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                    End If

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_PUMP_PDT_WARN)
                    If pumpsCalibrationRequired Then
                        myAlarmStatusList.Add(True)
                    Else
                        myAlarmStatusList.Add(False)
                    End If

                    ' Check if ISE Clean is required
                    Dim cleanRequired As Boolean = False
                    myGlobal = ISEAnalyzer.CheckCleanIsNeeded
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        cleanRequired = CType(myGlobal.SetDatos, Boolean)
                    End If

                    myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CLEAN_PDT_WARN)
                    If cleanRequired Then
                        myAlarmStatusList.Add(True)
                    Else
                        myAlarmStatusList.Add(False)
                    End If
                End If
                ' XB 21/01/2015 - BA-1873

                For i As Integer = 0 To myAlarmListTmp.Count - 1
                    PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                Next

                If myAlarmList.Count > 0 Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_CONNECTION_FINISHED, myValue, True)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEConnectionFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that the ISE has finished the procedure requested
        ''' </summary>
        ''' <remarks>
        ''' Created by SGM 15/03/2012
        ''' </remarks>
        Public Sub OnISEProcedureFinished() Handles _iseAnalyzer.ISEProcedureFinished
            Try
                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_PROCEDURE_FINISHED, 1, True)

                'SGM 12/04/2012 Update ISE conssumption flag in case of Session ended, paused or aborted
                If ISEAnalyzer.CurrentProcedure = ISEManager.ISEProcedures.WriteConsumption Then

                    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEConsumption, "END")
                    'Once the ise consumption is updated activate ansinf
                    If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then

                        AnalyzerIsInfoActivatedAttribute = 0
                        Dim myGlobalDataTO = ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STR)
                    End If
                End If

                ISEAnalyzer.CurrentCommandTO = Nothing
                ISEAnalyzer.CurrentProcedure = ISEManager.ISEProcedures.None

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEProcedureFinished", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Detects that some maintenance schedule is needed
        ''' </summary>
        ''' <remarks>Created by SGM 15/03/2012</remarks>
        Public Sub OnISEMaintenanceRequired(ByVal pOperation As ISEManager.MaintenanceOperations) Handles _iseAnalyzer.ISEMaintenanceRequired
            Try
                Select Case pOperation
                    Case ISEManager.MaintenanceOperations.ElectrodesCalibration : UpdateSensorValuesAttribute(AnalyzerSensors.ISE_CALB_REQUIRED, 1, True)
                    Case ISEManager.MaintenanceOperations.PumpsCalibration : UpdateSensorValuesAttribute(AnalyzerSensors.ISE_PUMPCAL_REQUIRED, 1, True)
                    Case ISEManager.MaintenanceOperations.Clean : UpdateSensorValuesAttribute(AnalyzerSensors.ISE_CLEAN_REQUIRED, 1, True)
                End Select

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.OnISEElectrodeInstalled", EventLogEntryType.Error, False)
            End Try
        End Sub
        'end SGM 07/03/2012


#End Region

#Region "Constructor and other initialization methods"
        Public Sub New(ByVal pApplicationName As String, ByVal pAnalyzerModel As String)
            Try
                classInitializationErrorAttribute = False

                If AppLayer Is Nothing Then
                    AppLayer = New ApplicationLayer
                End If
                waitingTimer.Enabled = False
                AddHandler waitingTimer.Elapsed, AddressOf waitingTimer_Timer

                'XBC 28/10/2011 - timeout limit repetitions for Start Tasks
                waitingStartTaskTimer.Enabled = False
                AddHandler waitingStartTaskTimer.Elapsed, AddressOf waitingStartTaskTimer_Timer

                ' XB 05/11/2014 - STATE needs a singular timer because its use is inside another generic instructions (like ISECMD) - BA-1872
                AddHandler waitingSTATETimer.Elapsed, AddressOf waitingSTATETimer_Timer

                myApplicationName = pApplicationName
                myAnalyzerModel = pAnalyzerModel 'SGM 04/03/11

                Dim myAlarmsDelegate As New AlarmsDelegate
                Dim myGlobalDataTo = myAlarmsDelegate.ReadAll(Nothing)
                If Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing Then
                    alarmsDefintionTableDS = DirectCast(myGlobalDataTo.SetDatos, AlarmsDS)
                End If
                
                'AG 20/04/2011 - Define methods who will implement work
                AddHandler wellBaseLineWorker.DoWork, AddressOf wellBaseLineWorker_DoWork
                AddHandler wellBaseLineWorker.RunWorkerCompleted, AddressOf wellBaseLineWorker_RunWorkerCompleted

                'AG 01/12/2011 - Water & Waste deposit error is generated by a timer
                Dim mySwParameterDelegate As New SwParametersDelegate
                wasteDepositTimer.Enabled = False
                waterDepositTimer.Enabled = False

                wasteDepositTimer.Interval = 60000 'default interval value 1 minute
                waterDepositTimer.Interval = 60000 'default interval value 1 minute

                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.MAX_TIME_DEPOSIT_WARN.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    If IsNumeric(myGlobalDataTo.SetDatos) AndAlso CInt(myGlobalDataTo.SetDatos) > 0 Then
                        wasteDepositTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                        waterDepositTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                    End If
                End If
                AddHandler wasteDepositTimer.Elapsed, AddressOf WasteDepositError_Timer
                AddHandler waterDepositTimer.Elapsed, AddressOf WaterDepositError_Timer

                'AG 05/01/2012 - Initialize thermo alarm errors timers (REACTIONS and FRIDGE)
                thermoReactionsRotorWarningTimer.Enabled = False
                thermoReactionsRotorWarningTimer.Interval = 300000 'default interval value 5 minutes
                thermoFridgeWarningTimer.Enabled = False
                thermoFridgeWarningTimer.Interval = 120000 'default interval value 2 minutes

                'Read the maximum time allowed for pass form warning to alarm (reactions rotor)
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.MAX_TIME_THERMO_REACTIONS_WARN.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    thermoReactionsRotorWarningTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                End If

                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.MAX_TIME_THERMO_FRIDGE_WARN.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    thermoFridgeWarningTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                End If

                AddHandler thermoReactionsRotorWarningTimer.Elapsed, AddressOf ThermoReactionsRotorError_Timer
                AddHandler thermoFridgeWarningTimer.Elapsed, AddressOf ThermoFridgeError_Timer

                'AG 05/01/2012 - Initialize thermo alarm errors timers (R1 and R2 arms)
                thermoR1ArmWarningTimer.Enabled = False
                thermoR1ArmWarningTimer.Interval = 60000 'default interval value 1 minutes
                thermoR2ArmWarningTimer.Enabled = False
                thermoR2ArmWarningTimer.Interval = 60000 'default interval value 1 minutes


                'Read the maximum time allowed for pass form warning to alarm (reactions rotor)
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.MAX_TIME_THERMO_ARM_REAGENTS_WARN.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    thermoR1ArmWarningTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                    thermoR2ArmWarningTimer.Interval = CInt(myGlobalDataTo.SetDatos) * 1000
                End If

                AddHandler thermoR1ArmWarningTimer.Elapsed, AddressOf thermoR1ArmWarningTimer_Timer
                AddHandler thermoR2ArmWarningTimer.Elapsed, AddressOf thermoR2ArmWarningTimer_Timer

                ' XB 09/12/2014 - Read timer values from Parameters table - BA-1872
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.WAITING_TIME_OFF.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    WAITING_TIME_OFF = CInt(myGlobalDataTo.SetDatos)
                End If
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.SYSTEM_TIME_OFFSET.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    SYSTEM_TIME_OFFSET = CInt(myGlobalDataTo.SetDatos)
                End If
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.WAITING_TIME_DEFAULT.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    WAITING_TIME_DEFAULT = CInt(myGlobalDataTo.SetDatos)
                End If
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.WAITING_TIME_FAST.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    WAITING_TIME_FAST = CInt(myGlobalDataTo.SetDatos)
                End If
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.WAITING_TIME_ISE_FAST.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    WAITING_TIME_ISE_FAST = CInt(myGlobalDataTo.SetDatos)
                End If
                myGlobalDataTo = SwParametersDelegate.ReadNumValueByParameterName(Nothing, SwParameters.WAITING_TIME_ISE_OFFSET.ToString, Nothing)
                If Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing Then
                    WAITING_TIME_ISE_OFFSET = CInt(myGlobalDataTo.SetDatos)
                End If

                AdjustmentsFilePath = String.Empty '#REFACTORING

                'Initialization State alarms
                StatusParameters.IsActive = False
                StatusParameters.State = StatusParameters.RotorStates.None
                StatusParameters.LastSaved = DateTime.Now

                Dim currentAlarmsDelg As New WSAnalyzerAlarmsDelegate

                Dim execDelg As New ExecutionsDelegate
                myGlobalDataTo = currentAlarmsDelg.GetAlarmsMonitor(Nothing, execDelg.GetActiveAnalyzerId(Nothing), "")
                If Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing Then
                    Dim temporalDs = DirectCast(myGlobalDataTo.SetDatos, WSAnalyzerAlarmsDS)

                    SetValuesOnSaveRotorStatusFromActiveAlarms(temporalDs, StatusParameters.RotorStates.FBLD_ROTOR_FULL, True)
                    SetValuesOnSaveRotorStatusFromActiveAlarms(temporalDs, StatusParameters.RotorStates.UNKNOW_ROTOR_FULL, False)
                End If

            Catch ex As Exception
                classInitializationErrorAttribute = True
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.New", EventLogEntryType.Error, False)
            End Try
        End Sub

        Private Sub SetValuesOnSaveRotorStatusFromActiveAlarms(ByVal temporalDs As WSAnalyzerAlarmsDS, ByVal rotorState As StatusParameters.RotorStates, ByVal isMin As Boolean)

            Dim alarmState = (From a In temporalDs.vwksAlarmsMonitor _
                    Where (a.AlarmID = rotorState.ToString) _
                          AndAlso a.AlarmStatus = True _
                    Select a.AlarmDateTime).ToList()
            If alarmState.Count > 0 Then
                StatusParameters.IsActive = True
                StatusParameters.State = rotorState
                StatusParameters.LastSaved = If(isMin, alarmState.Min, alarmState.Max)
            End If
        End Sub

        ''' <summary>
        ''' Initializes the base line calculation object when app is started
        ''' </summary>
        ''' <param name="pDBConnection" ></param>
        ''' <param name="pStartingApplication" >TRUE when app is starting FALSE when a reportsat is loaded</param>
        ''' <remarks>AG 29/04/2011
        '''          AG 25/10/2011 - add parameter pStartingApplication for differentiate when the application is started and when a reportsat or restore point is loaded</remarks>
        Public Sub InitBaseLineCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStartingApplication As Boolean) Implements IAnalyzerManager.InitBaseLineCalculations
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'AG 25/10/2011 - When a ReportSAT is loaded or a RestorePoint is restored the baseline calc must to be created again
                        If Not BaseLine Is Nothing AndAlso Not pStartingApplication Then

                            'AG 09/03/2012 - when load a reportsat and no connection remove all previous refresh information
                            If Not ConnectedAttribute Then
                                ClearRefreshDataSets(True, True) 'AG 22/05/2014 - #1637
                            End If

                            If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_INIT_ERR)
                            If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_WELL_WARN)
                                WELLbaselineParametersFailuresAttribute = False
                            End If

                            resultData = InitClassStructuresFromDataBase(dbConnection)
                        End If

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
                                Dim myAlarm = CType(resultData.SetDatos, AlarmEnumerates.Alarms)
                                'Update internal alarm list if exists alarm but not saved it into database!!!
                                If myAlarm <> AlarmEnumerates.Alarms.NONE Then
                                    If Not myAlarmListAttribute.Contains(myAlarm) Then
                                        myAlarmListAttribute.Add(myAlarm)

                                        'AG 12/09/2012 - If base line error when app is starting set the attribute baselineInitializationFailuresAttribute to value in order to show the alarm globe in monitor
                                        'Wup not in process 
                                        If pStartingApplication AndAlso myAlarm = AlarmEnumerates.Alarms.BASELINE_INIT_ERR Then
                                            Dim showGlobeFlag As Boolean = False
                                            If (String.Equals(mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS") OrElse _
                                                String.Equals(mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString), "PAUSED")) Then
                                                If Not IgnoreAlarmWhileWarmUp(myAlarm) Then
                                                    showGlobeFlag = True
                                                End If
                                            Else
                                                showGlobeFlag = True
                                            End If

                                            If showGlobeFlag Then
                                                'Set this attribute to limit in order the alarm will be shown as monitor globe
                                                baselineInitializationFailuresAttribute = ALIGHT_INIT_FAILURES
                                            End If

                                        End If

                                        'Prepare UIRefresh DS (generate event only when a ReportSAT is loaded or a RestorePoint is restored)
                                        resultData = PrepareUIRefreshEvent(dbConnection, UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                                        If Not BaseLine Is Nothing AndAlso Not pStartingApplication Then
                                            InstructionReceivedAttribute = ""
                                            RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.InitBaseLineCalculations", EventLogEntryType.Error, False)
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
        Private Function InitClassStructuresFromDataBase(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        'TR 14/03/2011 -Initialize REAGENT_CONTAMINATION_PERSISTANCE variable
                        Dim mySwParameterDelegate As New SwParametersDelegate

                        Dim myGlobal = mySwParameterDelegate.ReadByAnalyzerModel(dbConnection, myAnalyzerModel)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            Dim myParamDs = CType(myGlobal.SetDatos, ParametersDS)

                            Dim myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.CONTAMIN_REAGENT_PERSIS.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                REAGENT_CONTAMINATION_PERSISTANCE = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.MULTIPLE_ERROR_CODE.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                MULTIPLE_ERROR_CODE = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.BLINE_INIT_FAILURES.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                ALIGHT_INIT_FAILURES = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.SENSORUNKNOWNVALUE.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                SENSORUNKNOWNVALUE = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.PREDILUTION_CYCLES.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_PREDILUTION = CInt(myQRes(0).ValueNumeric) - 1 'Well offset for predilution = (predilution cycles used for time estimation - 1)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where String.Compare(a.ParameterName, SwParameters.MAX_REACTROTOR_WELLS.ToString, False) = 0 Select a).ToList

                            If myQRes.Count > 0 Then
                                MAX_REACTROTOR_WELLS = CInt(myQRes(0).ValueNumeric)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.ISETEST_CYCLES_SERPLM.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_ISETEST_SERPLM = CInt(myQRes(0).ValueNumeric)  'Well offset for ise test (ser or plm) (cycles until Fw activates the biochemical request after receive an SER or PLM isetest)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = SwParameters.ISETEST_CYCLES_URI.ToString Select a).ToList

                            If myQRes.Count > 0 Then
                                WELL_OFFSET_FOR_ISETEST_URI = CInt(myQRes(0).ValueNumeric)  'Well offset for ise test (uri) (cycles until Fw activates the biochemical request after receive an URI isetest)
                            End If

                            myQRes = (From a As ParametersDS.tfmwSwParametersRow In myParamDs.tfmwSwParameters _
                                      Where a.ParameterName = GlobalEnumerates.SwParameters.FLIGHT_INIT_FAILURES.ToString Select a).ToList
                            If myQRes.Count > 0 Then
                                FLIGHT_INIT_FAILURES = CInt(myQRes(0).ValueNumeric)
                            End If

                        End If

                        Dim myFieldLimitsDelegate As New FieldLimitsDelegate
                        myGlobal = myFieldLimitsDelegate.GetAllList(dbConnection)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myClassFieldLimitsDS = DirectCast(myGlobal.SetDatos, FieldLimitsDS)
                        End If


                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.InitClassStructuresFromDataBase", EventLogEntryType.Error, False)
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
        Public Function Start(ByVal pPooling As Boolean) As Boolean Implements IAnalyzerManager.Start
            Dim myReturn As Boolean = False
            Try
                CommThreadsStartedAttribute = False
                Dim myGlobal = AppLayer.Start(pPooling)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    CommThreadsStartedAttribute = myReturn
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.Start", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Restart communications channel
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 04/07/2012</remarks>
        Public Function SynchronizeComm() As Boolean Implements IAnalyzerManager.SynchronizeComm
            Dim myReturn As Boolean = False
            Try
                CommThreadsStartedAttribute = False
                Dim myGlobal = AppLayer.SynchronizeComm()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    CommThreadsStartedAttribute = myReturn
                    ConnectedAttribute = myReturn
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SynchronizeComm", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Dispose communications channel
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Terminate() As Boolean Implements IAnalyzerManager.Terminate
            Dim myReturn As Boolean = False
            Try
                CommThreadsStartedAttribute = False
                AppLayer.Terminate()
                myReturn = True

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.Terminate", EventLogEntryType.Error, False)
                myReturn = False
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Stop communications channel
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StopComm() As Boolean Implements IAnalyzerManager.StopComm
            Dim myReturn As Boolean = False
            Try
                CommThreadsStartedAttribute = False
                Dim myGlobal = AppLayer.StopComm()

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myReturn = CType(myGlobal.SetDatos, Boolean)
                    UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, 0, True)
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.StopComm", EventLogEntryType.Error, False)
                myReturn = False
            End Try
            Return myReturn
        End Function

        ''' <summary>
        ''' Start communications channel after a previous Stop
        ''' </summary>
        ''' <remarks>Created by XBC 18/06/2012</remarks>
        Public Function StartComm() As Boolean Implements IAnalyzerManager.StartComm
            Dim myReturn As Boolean = False
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                'SGM 01/02/2012 - Check if it is User Assembly - Bug #1112
                'If Not My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If Not GlobalBase.IsServiceAssembly Then
                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")
                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.WaitForAnalyzerReady, "") 'AG 20/06/2012
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
                        UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(IIf(ConnectedAttribute, 1, 0)), True)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.StartComm", EventLogEntryType.Error, False)
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
        Public Function ReadRegistredPorts() As GlobalDataTO Implements IAnalyzerManager.ReadRegistredPorts
            Dim myReturn As New GlobalDataTO
            Try
                myReturn = AppLayer.ReadRegistredPorts  'Pending to develop!!

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadRegistredPorts", EventLogEntryType.Error, False)
            End Try
            Return myReturn
        End Function


        ''' <summary>
        ''' When a worksession finish clear the DS with the last reagents prepared
        ''' </summary>
        ''' <remarks>AG 19/01/2011</remarks>
        Public Sub ResetWorkSession() Implements IAnalyzerManager.ResetWorkSession
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

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ResetWorkSession", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Inform the presentation layer has copied the ui refresh ds into a local variable
        ''' </summary>
        ''' <param name="pMainThread"></param>
        ''' <remarks></remarks>
        Public Sub ReadyToClearUIRefreshDS(ByVal pMainThread As Boolean) Implements IAnalyzerManager.ReadyToClearUIRefreshDS
            Try
                If pMainThread Then
                    eventDataPendingToTriggerFlag = False
                Else
                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'secondaryEventDataPendingToTriggerFlag = False
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadyToClearUIRefreshDS", EventLogEntryType.Error, False)
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
        ''' <param name="pFwScriptId">Identifier of the script/action to be sended</param>
        ''' <param name="pServiceParams">param values to script/action to be sended - IMPORTANT NOTE: Use it only in Service Scripts!!!</param>
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
        '''          XB 26/09/2014 - Implement Start Task Timeout for ISE commands - BA-1872
        '''          XB 06/11/2014 - Implement Start Task Timeout for RUNNING instruction - BA-1872
        '''          AG 11/12/2014 BA-2170 Analyzer does not becomes ready when receives an ANSFBLD instruction. This is an exception to the general rule!!
        '''          AG 09/02/2015 rename pParams to pServiceParams
        ''' </remarks>
        Public Function ManageAnalyzer(ByVal pAction As AnalyzerManagerSwActionList, ByVal pSendingEvent As Boolean, _
                                       Optional ByVal pInstructionReceived As List(Of InstructionParameterTO) = Nothing, _
                                       Optional ByVal pSwAdditionalParameters As Object = Nothing, _
                                       Optional ByVal pFwScriptId As String = "", _
                                       Optional ByVal pServiceParams As List(Of String) = Nothing) As GlobalDataTO Implements IAnalyzerManager.ManageAnalyzer

            Dim myGlobal As New GlobalDataTO

            Try
                If CommThreadsStartedAttribute Then

                    'Ignore all reception instructions (except the STATUS instructions) meanwhile the WaitForAnalyzerReady flag is 'INI'
                    If Not GlobalBase.IsServiceAssembly Then
                        ' User Sw
                        If mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" AndAlso _
                            mySessionFlags(AnalyzerManagerFlags.WaitForAnalyzerReady.ToString) = "INI" AndAlso _
                            Not pSendingEvent AndAlso pAction <> AnalyzerManagerSwActionList.STATUS_RECEIVED Then

                            Return myGlobal
                        End If
                    End If

                    If pAction = AnalyzerManagerSwActionList.CONNECT OrElse pAction = AnalyzerManagerSwActionList.RECOVER Then AnalyzerIsReadyAttribute = True

                    '--------------------------------------------------------------- Treat other special cases a part from WAITING TIME EXPIRED: SOUND, ENDSOUND
                    If pAction <> AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED AndAlso pAction <> AnalyzerManagerSwActionList.SOUND _
                        AndAlso pAction <> AnalyzerManagerSwActionList.ENDSOUND AndAlso pSendingEvent AndAlso Not AnalyzerIsReadyAttribute Then

                        If pAction <> AnalyzerManagerSwActionList.NEXT_PREPARATION Then
                            'If analyzer is busy dont send nothing, add in queue
                            Debug.Print(" ADD ACTION TO QUEUE : [" & pAction.ToString & "] count : " & myInstructionsQueue.Count.ToString)
                            myInstructionsQueue.Add(pAction)
                            myParamsQueue.Add(pSwAdditionalParameters) ' and its parameters XBC 24/05/2011
                        End If

                    Else

                        '>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                        '>>>------------------------------------------------------------------instruction reception or send instruction with analyzer ready !!!
                        '>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                        'AG 03/11/2010 - When receive instruction raise event before his threatment
                        If Not pSendingEvent Then
                            'Reception
                            InstructionReceivedAttribute = AppLayer.InstructionReceived
                            InstructionSentAttribute = ""

                            If Not eventDataPendingToTriggerFlag Then
                                ClearRefreshDataSets(True, True) 'AG 22/05/2014 - #1637
                            End If

                            InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - disable waiting time control if active

                            'AG 27/06/2011 - Several ANSWER instructions must set AnalyzerIsReadyAttribute = True due
                            'the ANSWER instruction indicates the analyzer can perform another task
                            'Other ANS instruction not due the Sw has not ask for then (ansphr, ansise, ansbr1, ansbm1, ansbr2,...)
                            If Not AnalyzerIsReadyAttribute Then
                                Select Case pAction
                                    Case AnalyzerManagerSwActionList.STATUS_RECEIVED
                                        'Do nothing ... when the status instruction is treated the Software updates the AnalyzerIsReadyAttribute

                                    Case AnalyzerManagerSwActionList.ANSFBLD_RECEIVED
                                        'Exception: The analyzer will be ready when receives STATUS with A:47

                                    Case Else
                                        If AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING Then
                                            AnalyzerIsReadyAttribute = True
                                        End If
                                End Select
                            End If

                        Else
                            If GlobalBase.IsServiceAssembly Then
                                If IsAlarmInfoRequested Then
                                    Thread.Sleep(1000)
                                End If
                            End If
                        End If

                        Select Case pAction
                            '''''''''''''''
                            'SEND EVENTS!!!
                            '''''''''''''''
                            Case AnalyzerManagerSwActionList.CONNECT
                                myGlobal = ConnectSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.SLEEP
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.SLEEP)

                            Case AnalyzerManagerSwActionList.STANDBY
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STANDBY)

                            Case AnalyzerManagerSwActionList.SKIP
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.SKIP)

                            Case AnalyzerManagerSwActionList.RUNNING
                                myGlobal = RunningSendEvent(pAction, pSwAdditionalParameters, pFwScriptID, pServiceParams)

                            Case AnalyzerManagerSwActionList.PAUSE
                                myGlobal = PauseSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.START
                                myGlobal = StartSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.ENDRUN
                                myGlobal = EndRunSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.NROTOR
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.NROTOR)

                            Case AnalyzerManagerSwActionList.ABORT
                                myGlobal = AbortSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.SOUND
                                myGlobal = SoundSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.ENDSOUND
                                myGlobal = EndSoundSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.RESULTSRECOVER
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.RESRECOVER)

                            Case AnalyzerManagerSwActionList.STATE
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)

                            Case AnalyzerManagerSwActionList.WASH
                                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.WASH)

                            Case AnalyzerManagerSwActionList.NEXT_PREPARATION
                                myGlobal = NextPreparationSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.ADJUST_LIGHT
                                myGlobal = AdjustLightSendEvent(myGlobal, pSwAdditionalParameters, pServiceParams)

                            Case AnalyzerManagerSwActionList.ADJUST_FLIGHT
                                myGlobal = AdjustFLightSendEvent(myGlobal, pSwAdditionalParameters, pFwScriptId, pServiceParams)

                            Case AnalyzerManagerSwActionList.INFO
                                myGlobal = InfoSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.WASH_STATION_CTRL
                                myGlobal = WashStationControlSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.BARCODE_REQUEST
                                myGlobal = BarCodeSendEvent(pAction, myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.CONFIG
                                myGlobal = ConfigSendEvent(myGlobal)

                            Case AnalyzerManagerSwActionList.READADJ
                                myGlobal = ReadAdjSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.ISE_CMD
                                myGlobal = IseCmdSendEvent(pAction, myGlobal, pSwAdditionalParameters, pFwScriptID, pServiceParams)

                            Case AnalyzerManagerSwActionList.FW_UTIL
                                myGlobal = FwUtilSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.RECOVER
                                myGlobal = RecoverSendEvent()

                            Case AnalyzerManagerSwActionList.POLLRD
                                myGlobal = PollRdSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.COMMAND
                                myGlobal = CommandSendEvent(pAction, pSwAdditionalParameters, pFwScriptID, pServiceParams)

                            Case AnalyzerManagerSwActionList.LOADADJ
                                myGlobal = LoadAdjSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.ADJUST_BLIGHT
                                myGlobal = If(ConnectedAttribute, AppLayer.ActivateProtocol(AppLayerEventList.BLIGHT, Nothing, "", "", pServiceParams), myGlobal)

                            Case AnalyzerManagerSwActionList.TANKS_TEST
                                myGlobal = TanksTestSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.SDMODE
                                myGlobal = If(ConnectedAttribute, AppLayer.ActivateProtocol(AppLayerEventList.SDMODE, Nothing, "", "", pServiceParams), myGlobal)

                            Case AnalyzerManagerSwActionList.SDPOLL
                                myGlobal = If(ConnectedAttribute, AppLayer.ActivateProtocol(AppLayerEventList.SDPOLL), myGlobal)

                            Case AnalyzerManagerSwActionList.POLLFW
                                myGlobal = PollFwSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.POLLHW
                                myGlobal = PollHwSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.RESET_ANALYZER
                                myGlobal = If(ConnectedAttribute, AppLayer.ActivateProtocol(AppLayerEventList.RESET_ANALYZER, Nothing), myGlobal)

                            Case AnalyzerManagerSwActionList.LOADFACTORYADJ
                                myGlobal = If(ConnectedAttribute, AppLayer.ActivateProtocol(AppLayerEventList.LOADFACTORYADJ, Nothing), myGlobal)

                            Case AnalyzerManagerSwActionList.UPDATEFW
                                myGlobal = UpdateFwSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.READCYCLES
                                myGlobal = ReadCyclesSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.WRITECYCLES
                                myGlobal = WriteCyclesSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.UTIL
                                myGlobal = UtilSendEvent(myGlobal, pSwAdditionalParameters)

                            Case AnalyzerManagerSwActionList.POLLSN
                                myGlobal = PollSnSendEvent(myGlobal)

                                ' SERVICE SOFTWARE END--------------------
                                ''''''''''''''''''''''''''''''''''''''''
                                'TIME OUT: HOW ARE YOU? <SEND EVENTS!!!>
                                ''''''''''''''''''''''''''''''''''''''''
                            Case AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED
                                myGlobal = WaitingTimeExpiredSendEvent(myGlobal)

                            Case AnalyzerManagerSwActionList.START_TASK_TIMEOUT
                                If Not StartTaskTimeoutSendEvent(myGlobal) Then
                                    Return myGlobal
                                End If

                                ''''''''''''''''''''
                                'RECEPTION EVENTS!!!
                                ''''''''''''''''''''
                            Case AnalyzerManagerSwActionList.STATUS_RECEIVED
                                myGlobal = StatusReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSERR_RECEIVED
                                myGlobal = AnsErrReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSINF_RECEIVED
                                myGlobal = AnsInfReceivedEvent(myGlobal, pInstructionReceived)

                            Case AnalyzerManagerSwActionList.READINGS_RECEIVED
                                myGlobal = ReadingsReceivedEvent(myGlobal, pInstructionReceived)

                            Case AnalyzerManagerSwActionList.BASELINE_RECEIVED
                                myGlobal = BaseLineReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFBLD_RECEIVED
                                myGlobal = AnsFbldReceivedEvent(myGlobal, pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSCBR_RECEIVED
                                myGlobal = AnsCbrReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED
                                myGlobal = IseResultReceivedEvent(myGlobal, pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ARM_STATUS_RECEIVED
                                myGlobal = ArmStatusReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSPSN_RECEIVED
                                myGlobal = AnsPsnReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSTIN_RECEIVED
                                myGlobal = AnsTinReceivedEvent(myGlobal, pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSPRD_RECEIVED
                                myGlobal = AnsPrdReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.COMMAND_RECEIVED
                                myGlobal = CommandReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
                                myGlobal = AdjustmentsReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.CYCLES_RECEIVED
                                myGlobal = CyclesReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSSDM
                                myGlobal = AnsSdmReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSCPU_RECEIVED
                                myGlobal = AnsCpuReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSJEX_RECEIVED
                                myGlobal = AnsJexReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSSFX_RECEIVED
                                myGlobal = AnsSfxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSGLF_RECEIVED
                                myGlobal = AnsGlfReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFCP_RECEIVED
                                myGlobal = AnsFcpReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFBX_RECEIVED
                                myGlobal = AnsFbxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFDX_RECEIVED
                                myGlobal = AnsFdxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFRX_RECEIVED
                                myGlobal = AnsFrxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFGL_RECEIVED
                                myGlobal = AnsFglReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFJE_RECEIVED
                                myGlobal = AnsFjeReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFSF_RECEIVED
                                myGlobal = AnsFsfReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSBXX_RECEIVED
                                myGlobal = AnsBxxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSDXX_RECEIVED
                                myGlobal = AnsDxxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSRXX_RECEIVED
                                myGlobal = AnsRxxReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSUTIL_RECEIVED
                                myGlobal = AnsUtilReceivedEvent(pInstructionReceived)

                            Case AnalyzerManagerSwActionList.ANSFWU_RECEIVED
                                myGlobal = AnsFwuReceivedEvent(pInstructionReceived)

                            Case Else
                                myGlobal.HasError = True
                                myGlobal.ErrorCode = "NOT_CASE_FOUND"
                        End Select

                        '---------------------------------------------------------------------INIT1
                        If GlobalBase.IsServiceAssembly Then
                            If pSendingEvent Then
                                If myGlobal.HasError OrElse Not ConnectedAttribute Then
                                    UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                                    InstructionTypeReceived = AnalyzerManagerSwActionList.NONE
                                    ' Set Waiting Timer Current Instruction OFF
                                    ClearQueueToSend()
                                    RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                                End If
                            Else
                                If InstructionTypeReceivedAttribute <> AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED And InstructionTypeReceivedAttribute <> AnalyzerManagerSwActionList.NONE Then
                                    If AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                                        If Not ConnectedAttribute Then
                                            ConnectedAttribute = True
                                            UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, 1, True) 'Prepare UI refresh event when Connected changes
                                        End If
                                    End If
                                End If
                            End If

                        Else 'UserSw TimeOut treatment
                            'AG 14/10/2011 -check if instruction has been successfully sent
                            If pSendingEvent Then
                                If myGlobal.HasError OrElse Not ConnectedAttribute Then
                                    UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                                End If
                            End If
                        End If

                        If pSendingEvent Then
                            If ConnectedAttribute Then
                                InstructionSentAttribute = AppLayer.InstructionSent
                                InstructionReceivedAttribute = ""

                                If AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING AndAlso _
                                    (pAction <> AnalyzerManagerSwActionList.INFO AndAlso pAction <> AnalyzerManagerSwActionList.CONNECT) Then
                                    SetAnalyzerNotReady()
                                End If

                            End If

                            Dim addText As String = ""
                            If pAction = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED Then
                                addText = "Time expired!!. "
                            End If

                            If GlobalBase.IsServiceAssembly Then
                                If pAction = AnalyzerManagerSwActionList.START_TASK_TIMEOUT Then
                                    addText = AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString & ". (" & numRepetitionsTimeout.ToString & ") "
                                End If

                                RaiseEvent SendEvent(addText & InstructionSentAttribute) 'AG 22/02/2012 moved only for service 
                            End If

                            If myUI_RefreshEvent.Count = 0 Then
                                ClearRefreshDataSets(True, False)
                            Else
                                If (Not ConnectedAttribute OrElse pAction = AnalyzerManagerSwActionList.CONNECT) AndAlso eventDataPendingToTriggerFlag Then
                                    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                                End If
                            End If

                        Else '(CASE ELSE ... instruction reception)
                            InstructionReceivedAttribute = AppLayer.InstructionReceived
                            InstructionSentAttribute = ""

                            Dim refreshPresentation As Boolean = False
                            If AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING Then
                                refreshPresentation = True
                            ElseIf pAction = AnalyzerManagerSwActionList.ANSCBR_RECEIVED Then
                                refreshPresentation = True

                            ElseIf pAction = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then
                                refreshPresentation = True

                            ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso _
                                AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED Then
                                refreshPresentation = True
                            End If

                            If refreshPresentation Then

                                ClearRefreshDataSets(True, False)

                                If Not sendingRepetitions Then
                                    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                                End If

                            ElseIf analyzerFREEZEFlagAttribute _
                              AndAlso (String.Equals(analyzerFREEZEModeAttribute, "TOTAL") _
                              OrElse String.Equals(analyzerFREEZEModeAttribute, "RESET")) _
                              AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then

                                ClearRefreshDataSets(True, False)

                                RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)

                            ElseIf String.Compare(mySessionFlags(AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString), "INPROCESS", False) = 0 _
                                   AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING _
                                   AndAlso (InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.STATUS_RECEIVED OrElse InstructionTypeReceivedAttribute = AnalyzerManagerSwActionList.ANSPSN_RECEIVED) Then

                                ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637
                                RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)

                            End If

                            If AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING AndAlso _
                                AnalyzerIsReadyAttribute AndAlso myInstructionsQueue.Count > 0 AndAlso _
                                mySessionFlags(AnalyzerManagerFlags.RUNNINGprocess.ToString) <> "INPROCESS" Then
                                Dim queuedAction = GetFirstFromQueue()
                                Dim queuedParam = GetFirstParametersFromQueue()
                                If queuedAction <> AnalyzerManagerSwActionList.NONE Then
                                    If queuedParam Is Nothing Then
                                        myGlobal = ManageAnalyzer(queuedAction, True)
                                    Else
                                        myGlobal = ManageAnalyzer(queuedAction, True, Nothing, queuedParam)
                                    End If
                                End If
                            End If
                        End If
                        '---------------------------------------------------------------------END1
                    End If
                    '<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    '<<<-----------------------------------------------------------------END: instruction reception or send instruction with analyzer ready !!!
                    '<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = "NO_THREADS"

                    If GlobalBase.IsServiceAssembly Then
                        UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, 0, True) 'Prepare UI refresh event when Connected changes
                        InstructionTypeReceived = AnalyzerManagerSwActionList.NONE
                        ClearQueueToSend()
                        RaiseEvent SendEvent(AnalyzerManagerSwActionList.NO_THREADS.ToString)
                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Remove any instruction from the queue
        ''' </summary>
        ''' <remarks>Created by XB 06/11/2013</remarks>
        Public Sub RemoveItemFromQueue(ByVal pAction As AnalyzerManagerSwActionList) Implements IAnalyzerManager.RemoveItemFromQueue
            Try
                Dim j As Integer = -1
                For Each a As AnalyzerManagerSwActionList In myInstructionsQueue
                    If a = pAction Then
                        j = myInstructionsQueue.IndexOf(a)
                        Exit For
                    End If
                Next

                If myInstructionsQueue.Count > 0 AndAlso j > -1 Then
                    myInstructionsQueue.RemoveAt(j)
                    myParamsQueue.RemoveAt(j)
                    Debug.Print("INSTRUCTION REMOVED FROM QUEUE : [" & pAction.ToString & "] ____________________________________")
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveItemFromQueue", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Enables the Sw communications to send new instructions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by AG 29/10/2010</remarks>
        Public Function ClearQueueToSend() As GlobalDataTO Implements IAnalyzerManager.ClearQueueToSend
            Dim myGlobal As New GlobalDataTO

            Try
                InitializeTimerControl(WAITING_TIME_OFF)
                myInstructionsQueue.Clear()
                myParamsQueue.Clear()   ' XBC 24/05/2011
                AnalyzerIsReadyAttribute = True

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ClearQueueToSend", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Informs if the instructions queue contains a specific instruction pending to be sent
        ''' </summary>
        ''' <param name="pInstruction"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 26/03/2012 - creation</remarks>
        Public Function QueueContains(ByVal pInstruction As AnalyzerManagerSwActionList) As Boolean Implements IAnalyzerManager.QueueContains
            Dim returnValue As Boolean
            Try
                returnValue = myInstructionsQueue.Contains(pInstruction)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.QueueContains", EventLogEntryType.Error, False)
                returnValue = False
            End Try
            Return returnValue
        End Function

        ''' <summary>
        ''' Add Parameters to the InstructionQueue and ParamsQueue
        ''' </summary>
        ''' <param name="pInstruction"></param>
        ''' <param name="pParamsQueue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function QueueAdds(ByVal pInstruction As AnalyzerManagerSwActionList, ByVal pParamsQueue As Object) As Boolean Implements IAnalyzerManager.QueueAdds

            If Not myInstructionsQueue.Contains(pInstruction) Then
                myInstructionsQueue.Add(pInstruction)
                myParamsQueue.Add(pParamsQueue)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Returns Scripts Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Scripts</returns>
        ''' <remarks>Created by XBC 08/11/2010</remarks>
        Public Function ReadFwScriptData() As GlobalDataTO Implements IAnalyzerManager.ReadFwScriptData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadFwScriptData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadFwScriptList", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' SERVICE SOFTWARE
        ''' Loads to the Application the Global Scripts Data from the Xml File
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 24/11/10</remarks>
        Public Function LoadAppFwScriptsData() As GlobalDataTO Implements IAnalyzerManager.LoadAppFwScriptsData
            Dim myGlobal As New GlobalDataTO
            Try

                myGlobal = AppLayer.LoadAppFwScriptsData

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.LoadAppFwScriptsData", EventLogEntryType.Error, False)
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
        Public Function LoadFwAdjustmentsMasterData(Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO Implements IAnalyzerManager.LoadFwAdjustmentsMasterData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.LoadFwAdjustmentsMasterData("MasterData", pSimulationMode)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Reads FW Adjustments DS
        ''' </summary>
        ''' <returns>DS with FW Adjustments</returns>
        ''' <remarks>Created by SG 26/01/11</remarks>
        Public Function ReadFwAdjustmentsDS() As GlobalDataTO Implements IAnalyzerManager.ReadFwAdjustmentsDS
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadFwAdjustmentsDS

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Updates FW Adjustments DS
        ''' </summary>
        ''' <remarks>Created by SG 26/01/11</remarks>
        Public Function UpdateFwAdjustmentsDS(ByVal pAdjustmentsDs As SRVAdjustmentsDS) As GlobalDataTO Implements IAnalyzerManager.UpdateFwAdjustmentsDS
            Dim myGlobal As New GlobalDataTO
            Try
                If pAdjustmentsDS IsNot Nothing Then

                    'SGM 05/12/2011
                    pAdjustmentsDS.AnalyzerModel = myAnalyzerModel
                    pAdjustmentsDS.AnalyzerID = ActiveAnalyzer
                    pAdjustmentsDS.FirmwareVersion = FwVersionAttribute
                    pAdjustmentsDS.ReadedDatetime = DateTime.Now
                    'end SGM 05/12/2011

                    AppLayer.UpdateFwAdjustmentsDS(pAdjustmentsDS)
                    myGlobal.SetDatos = pAdjustmentsDS
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateFwAdjustmentsDS", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns PhotometryDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Photometry</returns>
        ''' <remarks>Created by XBC 28/02/2011</remarks>
        Public Function ReadPhotometryData() As GlobalDataTO Implements IAnalyzerManager.ReadPhotometryData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadPhotometryData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadPhotometryData", EventLogEntryType.Error, False)
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
        Public Function SetPhotometryData(ByVal pPhotometryData As PhotometryDataTO) As GlobalDataTO Implements IAnalyzerManager.SetPhotometryData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.SetPhotometryData(pPhotometryData)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SetPhotometryData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns StressDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Stress Mode</returns>
        ''' <remarks>Created by XBC 22/03/2011</remarks>
        Public Function ReadStressModeData() As GlobalDataTO Implements IAnalyzerManager.ReadStressModeData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.ReadStressModeData()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadStressModeData", EventLogEntryType.Error, False)
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
        Public Function SetStressModeData(ByVal pStressModeData As StressDataTO) As GlobalDataTO Implements IAnalyzerManager.SetStressModeData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = AppLayer.SetStressModeData(pStressModeData)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SetStressModeData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns  OpticCenterDataTO Data object
        ''' </summary>
        ''' <returns>GlobalDataTo with the data of the Optic centering</returns>
        ''' <remarks>Created by XBC 12/05/2011</remarks>
        Public Function ReadOpticCenterData() As GlobalDataTO Implements IAnalyzerManager.ReadOpticCenterData
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal.SetDatos = OpticCenterResultsAttr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadOpticCenterData", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' USB connection cable has been disconnected and presentation inform this to AnalyzerManager class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 06/09/2012 - Add new parameter pConnectionCompleted
        '''             XB 06/11/2014 - ISE Timeout management - BA-1872
        '''             AG 06/02/2015 BA-2246 update CurrentInstructionAction = none in order to achieve recover an interrupted process (dynamic base line)
        ''' </remarks>
        Public Function ProcessUSBCableDisconnection(Optional ByVal pConnectionCompleted As Boolean = False) As GlobalDataTO Implements IAnalyzerManager.ProcessUSBCableDisconnection
            Dim myGlobal As New GlobalDataTO
            Try
                ConnectedAttribute = False

                ' XBC 16/01/2013 - Prepare ISE Object to reset it after a disconnection communications (Bugs tracking #1109)
                If Not ISEAnalyzer Is Nothing Then
                    ISEAnalyzer.IsAnalyzerDisconnected = True
                    ISEAlreadyStarted = False
                End If

                ISECMDLost = False
                RUNNINGLost = False
                sendingRepetitions = False
                InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                ClearStartTaskQueueToSend()

                If GlobalBase.IsServiceAssembly Then
                    SensorValuesAttribute.Clear()
                    ClearQueueToSend()
                    RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                End If

                'AG 08/01/2014 - If communications are lost in running clear all instructions in queue pending to be sent BT #1436
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    myInstructionsQueue.Clear()
                    myParamsQueue.Clear()
                End If

                'AG 13/02/2012 - The method UpdateSensorValuesAttribute add the alarm (sensor & database) but 
                'do not use it because the disconnection does not causes UI Refresh event
                'So we only update the sensor and the presentation method is who shows the message and add alarm into database

                If Not SensorValuesAttribute.ContainsKey(AnalyzerSensors.CONNECTED) Then
                    SensorValuesAttribute.Add(AnalyzerSensors.CONNECTED, 0)
                Else
                    SensorValuesAttribute(AnalyzerSensors.CONNECTED) = 0
                End If

                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                If pConnectionCompleted AndAlso String.Compare(mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString), "INPROCESS", False) = 0 Then
                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "CLOSED")
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                CurrentInstructionAction = InstructionActions.None 'In order to resend last instruction

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessUSBCableDisconnection", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Stop analyzer sound alarm (called from presentation layer)
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="pForceEndSound">Force End Sound.</param>
        ''' <remarks>create by: TR 28/10/2011</remarks>
        Public Function StopAnalyzerRinging(Optional ByVal pForceEndSound As Boolean = False) As GlobalDataTO Implements IAnalyzerManager.StopAnalyzerRinging
            Dim myGlobalDataTo As New GlobalDataTO
            Try
                'Send the sound off only when is ringing
                If ConnectedAttribute AndAlso (AnalyzerIsRingingAttribute OrElse pForceEndSound) Then
                    myGlobalDataTo = ManageAnalyzer(AnalyzerManagerSwActionList.ENDSOUND, True)
                    Thread.Sleep(500)
                End If

            Catch ex As Exception
                myGlobalDataTo.HasError = True
                myGlobalDataTo.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTo.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.StopAnalyzerRinging", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTo
        End Function

        ''' <summary>
        ''' Start the analyzer sound (called from presentation layer)
        ''' </summary>
        ''' <param name="pForceSound"></param>
        ''' <returns></returns>
        ''' <remarks>AG 22/07/2013 - based and adapted from StopAnalyzerRinging</remarks>
        Public Function StartAnalyzerRinging(Optional ByVal pForceSound As Boolean = False) As GlobalDataTO Implements IAnalyzerManager.StartAnalyzerRinging
            Dim myGlobalDataTo As New GlobalDataTO
            Try
                'Check the alarm sound setting is not disabled
                myGlobalDataTo = IsAlarmSoundDisable(Nothing)
                If Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing Then
                    'If not disabled ... continue
                    If Not DirectCast(myGlobalDataTo.SetDatos, Boolean) Then
                        'Send the sound ON only when analyzer is not still ringing
                        If ConnectedAttribute AndAlso (Not AnalyzerIsRingingAttribute OrElse pForceSound) Then
                            myGlobalDataTo = ManageAnalyzer(AnalyzerManagerSwActionList.SOUND, True)
                            Thread.Sleep(500)
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTo.HasError = True
                myGlobalDataTo.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTo.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.StartAnalyzerRinging", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTo
        End Function

        ''' <summary>
        ''' Requests to Analyzer to Stop ANSINFO continuous mode responsing
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/07/2012</remarks>
        Public Function StopAnalyzerInfo() As GlobalDataTO Implements IAnalyzerManager.StopAnalyzerInfo
            Dim myGlobal As New GlobalDataTO
            Try
                If ConnectedAttribute AndAlso AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.INFO, _
                                                             True, _
                                                             Nothing, _
                                                             Ax00InfoInstructionModes.STP)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.StopAnalyzerInfo", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
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
        Public Function ReadAdjustValue(ByVal pAdjust As Ax00Adjustsments) As String Implements IAnalyzerManager.ReadAdjustValue
            Dim returnValue As String = ""
            Dim myGlobal = ReadFwAdjustmentsDS()

            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                Dim myAdjDs As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDs.srv_tfmwAdjustments _
                           Where String.Compare(a.CodeFw, pAdjust.ToString, False) = 0 Select a).ToList
                If linqRes.Count > 0 Then
                    returnValue = linqRes(0).Value
                End If
            End If
            Return returnValue

        End Function


        ''' <summary>
        ''' Barcode functionality: Before enter RUNNING the analyzer
        ''' Before go Running: 1st read Reagents barcode (if configurated and barcode enabled)
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
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
        Public Function ManageBarCodeRequestBeforeRUNNING(ByVal pDbConnection As SqlConnection, ByVal pBarcodeProcessCurrentStep As BarcodeWorksessionActionsEnum) As GlobalDataTO Implements IAnalyzerManager.ManageBarCodeRequestBeforeRUNNING
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

                        'When user press START or CONTINUE buttons ... check the configuration status as first step
                        If (pBarcodeProcessCurrentStep = BarcodeWorksessionActionsEnum.BEFORE_RUNNING_REQUEST) Then
                            'Read barcode before RUNNING is enabled?
                            '** If YES: send active barcode requests for read + apply results business
                            '** If NO: go to running
                            readBarCodeBeforeRunningPerformedFlag = False

                            Dim usSettingsDelg As New UserSettingsDelegate
                            resultData = usSettingsDelg.GetCurrentValueBySettingID(dbConnection, UserSettingsEnum.BARCODE_BEFORE_START_WS.ToString())
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
                                    Dim barcodeDisabled As Boolean = ReadBarCodeRotorSettingEnabled(dbConnection, AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
                                    If (Not barcodeDisabled) Then
                                        BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.REAGENTS_REQUEST_BEFORE_RUNNING
                                    Else
                                        '2nd) If Samples Barcode is active -> send a request for full Samples Rotor scanning
                                        barcodeDisabled = ReadBarCodeRotorSettingEnabled(dbConnection, AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
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
                            Dim samplesBarcodeDisabled As Boolean = ReadBarCodeRotorSettingEnabled(dbConnection, AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
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
                                    Dim barCodeDs As New AnalyzerManagerDS
                                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

                                    Dim rotorType As String = "REAGENTS"
                                    If (BarCodeBeforeRunningProcessStatusAttribute = BarcodeWorksessionActionsEnum.SAMPLES_REQUEST_BEFORE_RUNNING) Then rotorType = "SAMPLES"

                                    'All positions
                                    rowBarCode = barCodeDs.barCodeRequests.NewbarCodeRequestsRow
                                    With rowBarCode
                                        .RotorType = rotorType
                                        .Action = Ax00CodeBarAction.FULL_ROTOR
                                        .Position = 0
                                    End With
                                    barCodeDs.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                    barCodeDs.AcceptChanges()

                                    resultData = ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, barCodeDs, "")

                                    If (Not resultData.HasError AndAlso ConnectedAttribute) Then
                                        UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BarcodeSTARTWSProcess, "INPROCESS")
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
                                        UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BarcodeSTARTWSProcess, "CLOSED")

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
                                                    'Dim myLogAcciones As New ApplicationLogManager()
                                                    GlobalBase.CreateLogActivity("Launch CreateWSExecutions !", "AnalyzerManager.ManageBarCodeRequestBeforeRUNNING", EventLogEntryType.Information, False)

                                                    'AG 30/05/2014 #1644 - Redesing correction #1584 for avoid DeadLocks (runningMode + new parameter AllowScanInRunning)
                                                    'Verify is the current Analyzer Status is RUNNING ==> BT #1584: ...and it is not in PAUSE
                                                    'Dim runningFlag As Boolean = (AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso _
                                                    '                              Not AllowScanInRunning)
                                                    ''Create the Executions
                                                    'resultData = myExecutionsDlg.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, runningFlag, -1, _
                                                    '                                                String.Empty, iseModuleReady)
                                                    Dim runningFlag As Boolean = (AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING)

                                                    'Create the Executions
                                                    resultData = myExecutionsDlg.CreateWSExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, runningFlag, -1, _
                                                                                                    String.Empty, iseModuleReady, Nothing, AllowScanInRunning)
                                                    'AG 30/05/2014 #1644


                                                End If 'AG 01/04/2014 - #1565

                                                If (Not resultData.HasError) Then
                                                    'Prepare UI for refresh Presentation layer screen (first execution)
                                                    resultData = PrepareUIRefreshEvent(Nothing, UI_RefreshEvents.EXECUTION_STATUS, 1, 0, Nothing, False)

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
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ISEConditioningProcess, "INPROCESS")
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ISEClean, "")
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ISEPumpCalib, "")
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ISECalibAB, "")
                                                Else
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                                                End If

                                                If (Not resultData.HasError) Then
                                                    resultData = PerformAutoIseConditioning(dbConnection)
                                                End If
                                            Else
                                                'Do not use resultData for not lose error if exists
                                                Dim resultDataAux = ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True)    'Running can not be sent. Ask for status for reactivate UI

                                                UpdateSensorValuesAttribute(AnalyzerSensors.BARCODE_WARNINGS, 2, True)     'Not pending executions to be sent
                                                UpdateSensorValuesAttribute(AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due scan barcode warnings
                                            End If
                                        Else
                                            UpdateSensorValuesAttribute(AnalyzerSensors.BARCODE_WARNINGS, 1, True)     'Samples barcode warnings
                                            UpdateSensorValuesAttribute(AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due scan barcode warnings
                                        End If
                                    End If
                            End Select
                        End If

                        If (Not resultData.HasError AndAlso myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            resultData = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDs)
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageBarCodeRequestBeforeRUNNING", EventLogEntryType.Error, False)
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
        Public Function ExistBottleAlarms() As Boolean Implements IAnalyzerManager.ExistBottleAlarms
            Dim returnValue As Boolean = False
            Try
                'In Running pause mode only the alarms of washing solution or high contamination bottles
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso AllowScanInRunningAttribute Then
                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                   OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then

                        returnValue = True
                    End If

                    'Otherwise all: washing solution, high contamination, waster deposit or water deposit
                Else
                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                   OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                   OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_SYSTEM_ERR) _
                   OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR) Then

                        returnValue = True
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ExistBottleAlarms", EventLogEntryType.Error, False)
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
        Public Function ProcessUpdateWSByAnalyzerID(ByVal pDbConnection As SqlConnection) As GlobalDataTO Implements IAnalyzerManager.ProcessUpdateWSByAnalyzerID
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                Dim myWSAnalyzerID As String = ""

                ' open db transaction
                myGlobal = GetOpenDBTransaction(pDbConnection)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        ' Insert (if not exists) the Analyzer identifier connected
                        'Dim myLogAccionesAux As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity("(Analyzer Change) Insert Connect Analyzer ", "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)

                        myGlobal = InsertConnectedAnalyzer(dbConnection)

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

                                    'Dim myLogAcciones As New ApplicationLogManager()
                                    Select Case WorkSessionStatusAttribute

                                        Case "EMPTY"
                                            If Not String.Compare(myWSAnalyzerID, ActiveAnalyzer, False) = 0 Then
                                                GlobalBase.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
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
                                                GlobalBase.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
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
                                                GlobalBase.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                myGlobal = ProcessToMountTheNewSession(dbConnection, myWSAnalyzerID)

                                                If Not myGlobal.HasError Then
                                                    Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                                                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.READADJ, True, Nothing, Ax00Adjustsments.ALL)

                                                    If Not myGlobal.HasError AndAlso myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                        myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDs)
                                                    End If
                                                End If
                                            End If

                                        Case Else
                                            If Not myWSAnalyzerID = ActiveAnalyzer Then
                                                GlobalBase.CreateLogActivity("Change AnalyzerID : case " & WorkSessionStatusAttribute, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Information, False)
                                                myGlobal = ProcessToMountTheNewSession(dbConnection, myWSAnalyzerID)
                                                ForceAbortSessionAttr = True

                                                If Not myGlobal.HasError Then
                                                    Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                                                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                                                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.READADJ, True, Nothing, Ax00Adjustsments.ALL)

                                                    If Not myGlobal.HasError AndAlso myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                        myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDs)
                                                    End If
                                                End If
                                            Else

                                                ' TODO : ANALYZER RECOVERY PROCESS - PENDING !!!

                                            End If

                                    End Select

                                End If

                            End If

                        End If

                        ' Finally Old Analyzer (in case to exists) must be deleted
                        If myWSAnalyzerID.Length > 0 AndAlso Not myWSAnalyzerID = ActiveAnalyzer Then
                            If Not myGlobal.HasError Then
                                Dim confAnalyzers As New AnalyzersDelegate
                                myGlobal = confAnalyzers.DeleteConnectedAnalyzersNotActive(dbConnection)
                            End If
                        End If


                        If Not myGlobal.HasError Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then RollbackTransaction(dbConnection)
                        End If

                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessUpdateWSByAnalyzerID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' When there is a WorkSession defined for an Analyzer different to the connected one, the WorkSession definition is 
        ''' "copied" for the Active Analyzer and the WorkSession is reset for the previous one
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pWsAnalyzerId">Identifier of the Analyzer where the active WorkSession was prepared/executed</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 12/06/2012
        ''' Modified by: XBC 25/04/2013 - Add value = FALSE for the new optional pSaveLISPendingOrders parameter to indicate that the process to Save the LIS orders not processed Is NOT required
        ''' </remarks>
        Public Function ProcessToMountTheNewSession(ByVal pDbConnection As SqlConnection, ByVal pWsAnalyzerId As String) As GlobalDataTO Implements IAnalyzerManager.ProcessToMountTheNewSession
            Dim myGlobal As New GlobalDataTO

            Try
                Dim myWsDelegate As New WorkSessionsDelegate
                Dim mySavedWsDelegate As New SavedWSDelegate

                myGlobal = WorkSessionsDelegate.GetOrderTestsForWS(pDbConnection, ActiveWorkSession)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim myWorkSessionResultDs As WorkSessionResultDS = DirectCast(myGlobal.SetDatos, WorkSessionResultDS)

                    'Save temporally the WS to recuperate it after the Reset of the current WS
                    myGlobal = mySavedWsDelegate.Save(pDbConnection, ActiveAnalyzer, myWorkSessionResultDs, -1)
                End If

                Dim savedWsid As Integer
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    savedWsid = DirectCast(myGlobal.SetDatos, Integer)

                    'Reset the current WS
                    If (Not HISTWorkingMode) Then
                        myGlobal = myWsDelegate.ResetWS(pDbConnection, pWSAnalyzerID, ActiveWorkSession, myAnalyzerModel, False) 'AG 17/11/2014 BA-2065 inform analyzerModel
                    Else
                        myGlobal = myWsDelegate.ResetWSNEW(pDbConnection, pWSAnalyzerID, ActiveWorkSession, myAnalyzerModel, False, False) 'AG 17/11/2014 BA-2065 inform analyzerModel
                    End If
                    If (Not myGlobal.HasError) Then ResetWorkSession()
                End If

                If (Not myGlobal.HasError) Then
                    'Update field AnalyzerID in table Reactions Rotor WS
                    Dim myReactionsRotorDelegate As New ReactionsRotorDelegate
                    myGlobal = myReactionsRotorDelegate.UpdateWSAnalyzerID(pDbConnection, ActiveAnalyzer, pWSAnalyzerID)
                End If

                If (Not myGlobal.HasError) Then
                    'Load new WorkSession saved with name ActiveAnalyzer
                    Dim myOrderTests As New OrderTestsDelegate

                    myGlobal = myOrderTests.LoadFromSavedWSToChangeAnalyzer(pDbConnection, savedWsid, ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim myWorkSessionsDs As WorkSessionsDS = DirectCast(myGlobal.SetDatos, WorkSessionsDS)

                        If (myWorkSessionsDs.twksWorkSessions.Rows.Count = 1) Then
                            ActiveWorkSession = myWorkSessionsDs.twksWorkSessions(0).WorkSessionID
                            WorkSessionStatusAttribute = myWorkSessionsDs.twksWorkSessions(0).WorkSessionStatus
                        End If
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    myGlobal = mySavedWsDelegate.ExistsSavedWS(pDbConnection, ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim mySavedWsds As SavedWSDS = DirectCast(myGlobal.SetDatos, SavedWSDS)

                        'Delete the temporary Work Session created initially
                        myGlobal = mySavedWsDelegate.Delete(pDbConnection, mySavedWsds)
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessToMountTheNewSession", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Insert Active Analyzer into database
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 13/06/2012</remarks>
        Public Function InsertConnectedAnalyzer(ByVal pDBConnection As SqlConnection) As GlobalDataTO Implements IAnalyzerManager.InsertConnectedAnalyzer
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim myLogAccionesAux As New ApplicationLogManager()
                If Not myTmpConnectedAnalyzerDS Is Nothing Then

                    GlobalBase.CreateLogActivity("(Analyzer Change) 1st condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                    If myTmpConnectedAnalyzerDS.tcfgAnalyzers.Count > 0 Then
                        GlobalBase.CreateLogActivity("(Analyzer Change) 2nd condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                        If Not myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).IsIsNewNull Then
                            GlobalBase.CreateLogActivity("(Analyzer Change) 3rd condition ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                            If myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).IsNew Then
                                Dim confAnalyzers As New AnalyzersDelegate
                                GlobalBase.CreateLogActivity("(Analyzer Change) Insert Analyzer ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                myGlobal = confAnalyzers.InsertAnalyzer(pDBConnection, myTmpConnectedAnalyzerDS.tcfgAnalyzers(0))

                                If Not myGlobal.HasError Then
                                    GlobalBase.CreateLogActivity("(Analyzer Change) Copy Analyzer Settings ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                    CopyAnalyzerSettings(pDBConnection, myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID)
                                    ActiveAnalyzer = myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID
                                    GlobalBase.CreateLogActivity("(Analyzer Change) Active Analyzer OK [" & ActiveAnalyzer & "] ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                                End If
                            End If
                        Else
                            ActiveAnalyzer = myTmpConnectedAnalyzerDS.tcfgAnalyzers(0).AnalyzerID
                            GlobalBase.CreateLogActivity("(Analyzer Change) Active Analyzer [" & ActiveAnalyzer & "] ", "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Information, False)
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.InsertConnectedAnalyzer", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Read Adjustments
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 06/07/2012</remarks>
        Public Function ReadAdjustments(ByVal pDbConnection As SqlConnection) As GlobalDataTO Implements IAnalyzerManager.ReadAdjustments
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                ' open db transaction
                myGlobal = GetOpenDBTransaction(pDbConnection)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                        UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")

                        myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.READADJ, True, Nothing, Ax00Adjustsments.ALL)

                        If Not myGlobal.HasError AndAlso myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDs)
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
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadAdjustments", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Remove the DS with information with the results automatically exported
        ''' </summary>
        ''' <remarks>AG 02/04/2013 - Creation</remarks>
        Public Sub ClearLastExportedResults() Implements IAnalyzerManager.ClearLastExportedResults
            SyncLock LockThis
                lastExportedResultsDSAttribute.Clear()
            End SyncLock
        End Sub

        Public Sub ClearReceivedAlarmsFromRefreshDs() Implements IAnalyzerManager.ClearReceivedAlarmsFromRefreshDs
            'Use exclusive lock over myUI_RefreshDS variables
            SyncLock myUI_RefreshDS.ReceivedAlarms
                myUI_RefreshDS.ReceivedAlarms.Clear()
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
        Public Function ExistSomeAlarmThatRequiresStopWS() As Boolean Implements IAnalyzerManager.ExistSomeAlarmThatRequiresStopWS
            Dim returnValue As Boolean = False
            Try
                If myAlarmListAttribute.Count > 0 Then
                    'AG 16/02/2012 the bottles warning level do not implies ENDRUN - remove from condition the alarms HIGH_CONTAMIN_WARN and WASH_CONTAINER_WARN
                    'AG 25/07/2012 - the clot detection error do not implies ENDRUN - remove from condition the alarm CLOT_DETECTION_ERR

                    'AG 22/02/2012 - NOTE: To prevent the bottle level ERROR or WARNING oscillations when near the limits Sw will not remove the ERROR level alarm until neither error neither warming exists 
                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                    myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                    myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                    myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                    myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.R1_COLLISION_WARN) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.R2_COLLISION_WARN) OrElse _
                    myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.S_COLLISION_WARN) OrElse myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                    BaseLine.exitRunningType <> 0 OrElse analyzerFREEZEFlagAttribute Then
                        returnValue = True
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ExistSomeAlarmThatRequiresStopWS", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function

        ''' <summary>
        ''' Using the results of the FLIGHT instruction (for all leds) prepares the database for a quick running (updates table twksWSBLinesByWell)
        ''' Used:
        ''' - After FLIGHT results reception
        ''' - After RESET WS the last FLIGHT results must be treated again in order to prepare next worksession - CANCELLED!!!
        '''              (after reset the well rejections will be obtained from the last rotor turn in previous worksession)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pInitialWell"></param>
        ''' <returns></returns>
        ''' <remarks>AG 20/11/2014 BA-2065</remarks>
        Public Function ProcessDynamicBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pInitialWell As Integer) As GlobalDataTO Implements IAnalyzerManager.ProcessDynamicBaseLine
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = BaseLine.ControlDynamicBaseLine(pDBConnection, pWorkSessionID, pInitialWell)
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessDynamicBaseLine", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="itemToRemove"></param>
        ''' <remarks></remarks>
        Public Sub AlarmListRemoveItem(itemToRemove As AlarmEnumerates.Alarms) Implements IAnalyzerManager.AlarmListRemoveItem
            myAlarmListAttribute.Remove(itemToRemove)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="itemToAdd"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AlarmListAddtem(itemToAdd As AlarmEnumerates.Alarms) As Boolean Implements IAnalyzerManager.AlarmListAddtem
            If Not myAlarmListAttribute.Contains(itemToAdd) Then
                myAlarmListAttribute.Add(itemToAdd)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub AlarmListClear() Implements IAnalyzerManager.AlarmListClear
            myAlarmListAttribute.Clear()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub MyErrorCodesClear() Implements IAnalyzerManager.MyErrorCodesClear
            myErrorCodesAttribute.Clear()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <remarks></remarks>
        Public Sub FillNextPreparationToSend(ByRef myGlobal As GlobalDataTO) Implements IAnalyzerManager.FillNextPreparationToSend
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myNextPreparationToSendDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)
            End If
        End Sub

        Public Function ActivateProtocolWrapper(ByVal pEvent As AppLayerEventList, Optional ByVal pSwEntry As Object = Nothing, _
                                          Optional ByVal pFwEntry As String = "", _
                                          Optional ByVal pFwScriptID As String = "", _
                                          Optional ByVal pServiceParams As List(Of String) = Nothing) As GlobalDataTO _
                                      Implements IAnalyzerManager.ActivateProtocolWrapper
            Return AppLayer.ActivateProtocol(pEvent, pSwEntry, pFwEntry, pFwScriptID, pServiceParams)
        End Function
#End Region

    End Class

End Namespace
