Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.BL.UpdateVersion
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls
Imports System.Globalization
Imports LIS.Biosystems.Ax00.LISCommunications 'AG 25/02/2013 - for LIS communications
Imports System.Xml 'AG 25/02/2013 - for LIS communications
Imports System.Threading 'AG 25/02/2013 - for LIS communications (release MDILISManager object in MDI closing event)
Imports System.Timers

Public Class IAx00MainMDI

#Region "Declarations"

    'AG 28/07/2010
    Private AutoConnectFailsTitle As String = ""
    Private AutoConnectFailsErrorCode As String = ""

    Private WithEvents MDIAnalyzerManager As AnalyzerManager

    Private ProcessActivate As Boolean = True
    Private ChangeReactionsRotorRecommendedWarning As Integer = 0

    Private OpeningLegend As Boolean

    '//22/10/2013 - CF - BT #1342 - Button text for CustomMessageBox
    Private abortButtonText As String
    Private stopButtonText As String
    Private cancelButtonText As String
    Private confirmAbortEndSessionMessage As String
    '//END CF-edit #1342

    'RH 01/07/2011 Alarm globes
    Private Const ParentMDITopHeight As Integer = 90
    Private incompleteSamplesOpenFlag As Boolean = False 'AG 03/04/2012

    'Front view
    Public CoverAlert As bsAlert
    Public ISEModuleAlert As bsAlert
    Public WashingSolutionAlert As bsAlert
    Public ResiduesBalanceAlert As bsAlert

    'Rotors
    Public Reagents1ArmAlert As bsAlert
    Public ReagentsRotorAlert As bsAlert
    Public Reagents2ArmAlert As bsAlert

    Public OthersAlert As bsAlert
    Public ReactionsRotorAlert As bsAlert
    Public WashingStationAlert As bsAlert
    Public SampleArmAlert As bsAlert
    Public SamplesRotorAlert As bsAlert

    Public AlertList As New List(Of bsAlert)
    Public AlertText As New Dictionary(Of GlobalEnumerates.Alarms, String)
    'RH 01/07/2011 END

    Private myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
    Private swParams As New SwParametersDelegate

    Public Enum WorkSessionUserActions 'AG 04/08/2011
        NO_WS
        START_WS
        PAUSE_WS
        ABORT_WS
    End Enum

    Private AutoConnectProcess As Boolean 'AG 08/11/2012 - move with the others processing business flags 'AG 28/07/2010 (, for the MDI in some cases fails because can exists several working processes with different exit conditions)
    Public processingBeforeRunning As String = "1" 'DL 18/01/2012  0: In process, 1: Finished OK, 2: Finished with Error
    '                                               Do not use screenWorkingProcess because we need more than 2 values (we need in process, finished ok and finished KO)
    '                                               TR 10/05/2012 It is public because is used on Rotor positioning
    Private readBarcodeIfConfigured As Boolean = True 'AG 08/03/2013 - true (read barcode before start if configured) / false (ignore it, used when exists some samples without request but user decides go to running)

    Private processingReset As Boolean = False    'AG 17/04/2012 -do not use screenWorkingProcess, for the MDI in some cases fails because can exists several working processes with different exit conditions
    Private processingRecover As Boolean = False  'AG 17/04/2012 -do not use screenWorkingProcess, for the MDI in some cases fails because can exists several working processes with different exit conditions
    Private myErrorsList As List(Of String)       'XB 23/04/2031 - errors management can't be use showmessage inside a thread

    'TR 20/10/2011 
    Private myDriveDetector As DetectorForm.DriveDetector
    Private myAdjustmentsFilePath As String

    Private WaitingCyclesTimes As Single = 0
    Private CycleMachine As Single = 0

    Private TotalSecs As Integer = 0
    Private TotalSecs2 As Integer = 0

    Private SaveStatusMessageID As GlobalEnumerates.Messages = Messages.CONNECTING
    Private completeWupProcessFlag As Boolean = False 'AG 12/09/2011

    Private lockThis As New Object() 'RH 30/03/2012

    Private ClosedByFormCloseButton As Boolean = False 'RH 13/04/2012 To be used only by the common forms

    Private ClosedByISEUtilCloseButton As Boolean = False 'SGM 10/05/2012 To be used only by ISE util form
    Private IsISEUtilClosing As Boolean = False 'SGM 10/05/2012

    Private flagExitWithShutDown As Boolean = False 'DL 01/06/2012

    Private NotDisplayAbortMsg As Boolean = False ' XBC 19/06/2012
    Private NotDisplayErrorCommsMsg As Boolean = False ' XBC 20/06/2012
    Private NotDisplayShutDownConfirmMsg As Boolean = False ' XBC 26/06/2012

    ' XBC 13/07/2012
    Private ShutDownisPending As Boolean = False
    Private StartSessionisPending As Boolean = False
    Private StartSessionisInitialPUGsent As Boolean = False
    Private StartSessionisCALBsent As Boolean = False
    Private StartSessionisPMCLsent As Boolean = False
    Private StartSessionisBMCLsent As Boolean = False
    Private isStartInstrumentActivating As Boolean = False
    Private executeSTD As Boolean = False
    Private LockISE As Boolean = False
    Private SkipCALB As Boolean = False
    Private SkipPMCL As Boolean = False
    Private SkipBMCL As Boolean = False
    Public ISEProcedureFinished As Boolean = False     ' XBC 27/09/2012 
    ' XBC 13/07/2012

    ' XBC 12/04/2013
    Private screenWorkingProcess2 As Boolean = False

    'DL 17/04/2013 (AG - Define as private in MDI instead of public in enumerates)
    Private Enum MDIWatchDogCases
        ISEConditioningBeforeRunning
        ReadBarcodeBeforeRunning
        GoToStatusRunning
    End Enum
    'DL 17/04/2013

    Private myAutoLISWaitingOrder As String = "" 'DL 17/07/2013 - used for automatic WS creation with LIS process

    'JV + AG revision 21/10/2013  task # 1341
    Private imagePlay, imagePause As Image
    Private showSTARTWSiconFlag As Boolean = True 'AG 18/10/2013 - true if STARTWS icon has to be shown
    Private showPAUSEWSiconFlag As Boolean = False 'AG 18/10/2013 - true if PAUSEWS icon has to be shown
    Private multisessionButtonEnableValue As Boolean = False 'AG 18/10/2013 - status (enabled) for the multisession button
    Private tooltipPlay, tooltipPause As String
    'JV + AG task # 1341
    Private infoCritical, waitButton As String 'JV + AG 28/11/2013 #1391

    Private StartingSession As Boolean = False ' XB 31/10/2013
    Private ForceRefreshOnFirstStandBy As Boolean = True    ' XB 06/11/2013
    Private recoveryResultsWarnFlag As Boolean = False 'AG 28/11/2013 - BT #1397

    Private CheckAnalyzerIDOnFirstConnection As Boolean = True  ' XB 03/12/2013
    Private CheckAnalyzerIDOnFirstConnectionForISE As Boolean = True  ' XB 03/12/2013

#End Region

#Region "Fields"


    'TR 21/05/2012 Variable used to indicate if the user has pause the worksession.
    Private UserPauseWSAttribute As Boolean = False

    Private Shared AnalyzerModelAttribute As String = ""
    Private Shared AnalyzerIDAttribute As String = ""
    Private Shared WorkSessionIDAttribute As String = ""
    Private Shared WSStatusAttribute As String = ""
    Private NumOfTestsAttribute As Integer
    Private CurrentLanguageAttribute As String = ""
    Private NoMDIChildActiveFormsAttribute As New List(Of Form) 'List of non mdi child form currently activated (for instance graphical abs(t), calibration curve)

    'AG 04/08/20111
    Private ExecuteSessionAttribute As Boolean = False 'TRUE when user has press button to enter in RUNNING (START or CONTINUE), FALSE when user has press button to finish RUNNING (PAUSE or ABORT)
    Private PauseSessionAttribute As Boolean = False
    Private AbortSessionAttribute As Boolean = False

    Private DisabledMDIChildAttribute As New List(Of Form) 'AG 26/09/2011

    'TR 16/11/2011 -Variable used to calculate the session time
    Private Shared LocalElapsedTimeAttribute As New DateTime
    Private Shared InitialRemainingTimeAttribute As New DateTime

    Private WarmUpFinishedAttribute As Boolean = False 'AG 12/09/2011 - Indicates if the wup process (start instrument, wash, alight) + thermo time is finished for the current analyzerID (AnalyzerIDAttribute)

    Private LoadingGlobesField As Boolean = False

    ' XBC 08/06/2012
    Private FwVersionAttribute As String

    Private isResetWSActiveAttribute As Boolean = False
    Private isSinglecanningRequestedAttribute As Boolean = False 'AG 06/11/2013 - #1375 scanning has been requested from rotor positions or sample requests screens

#End Region

#Region "LIS declarations & fields"

    Private WithEvents MDILISManager As ESWrapper 'AG 25/02/2013

    Public ProcessingLISManagerObject As Boolean = False
    ' AG 25/03/2013 - TRUE: Indicates the LIS manager is not available, FALSE: Indicates available. Used when MDI is loading and when MDI is closing
    ' XB 15/03/2013 - Change to public to be available from others forms

    Private ProcessingLISManagerUploadResults As Boolean = False
    Private processingLISOrderDownload As Boolean = False
    Private resultsOrderDownloadProcess As New GlobalDataTO 'AG 02/04/2013 - Results of the order download synchronous process
    Private pendingUploadToLisDS As New ExecutionsDS
    Private inCourseUploadToLisDS As New ExecutionsDS

    'TR -Variable used to indicate if LIS is Online or works with Files.
    Private LISWithFilesMode As Boolean = False

    'JC Usefull to know the total pending orders (xml with status PENDING and order tests in the LIS saved worksession)
    Private PendingOrders As Integer = 0
    'JC Limit defined when there are several orders pending to download and download process must require too much time
    Private MAX_PENDING_ORDERS As Integer


    'JC ToolTip description shown when there are any order pending to download or when there aren't any
    'JC 29/05/2013 Ordres Download Count, return more Orders Pending than real, because Synapse Driver Generates fake Orders.
    '              For this reason has been decided disable the tooltip with Orders Pending Count. Now Orders Pending Count is disable

    ' Before (when was shown the Orders Pending Count)
    ' Private PendingOrdersToolTip As String
    Private NoPendingOrdersToolTip As String

    ' XBC 24/04/2013
    Public InvokeReleaseFromConfigSettings As Boolean = False

    'DL 17/04/2013
    Private watchDogTimer As New System.Timers.Timer()
    Private watchDogCase As String = ""

    'AG 08/07/2013 - Automate process of WS creation using LIS
    Private automateProcessCurrentState As LISautomateProcessSteps = LISautomateProcessSteps.notStarted
    Dim pausingAutomateProcessFlag As Boolean = False
    Dim HQProcessByUserFlag As Boolean = False          ' XB 22/07/2013 - Auto LIS HQ
    Private LISWaitTimer As New System.Timers.Timer()
    Private autoWSCreationWithLISModeAttribute As Boolean = False 'AG 11/07/2013 - True when the START ws button creates the ws completely: read barcode, ask to lis, process lis orders and go to running
    'AG 08/07/2013

    Private appNameForLISAttribute As String = "BAx00" 'AG 27/02/2013 value is initiated here, is read from DB in method InitiateLISWrapper
    Private channelIdForLISAttribute As String = "1" 'AG 27/02/2013 value is initiated here, is read from DB in method InitiateLISWrapper

    Private DisplayISELockedPreparationsWarningAttribute As Boolean = False      ' XB 22/11/2013 - Task #1394
#End Region

#Region "Properties"

    Public Property UserPauseWS() As Boolean
        Get
            Return UserPauseWSAttribute
        End Get
        Set(ByVal value As Boolean)
            UserPauseWSAttribute = value
        End Set
    End Property

    'TR 18/05/2012 - variable used in others app parts.
    Public Property LocalTotalSecs2() As Integer
        Get
            Return TotalSecs2
        End Get
        Set(ByVal value As Integer)
            TotalSecs2 = value
        End Set
    End Property

    Public Property LocalElapsedTime() As DateTime
        Get
            Return LocalElapsedTimeAttribute
        End Get
        Set(ByVal value As DateTime)
            LocalElapsedTimeAttribute = value
        End Set
    End Property

    Public Property InitialRemainingTime() As DateTime
        Get
            Return InitialRemainingTimeAttribute
        End Get
        Set(ByVal value As DateTime)
            InitialRemainingTimeAttribute = value
        End Set
    End Property

    Public Shared ReadOnly Property ActiveAnalyzerModel() As String
        Get
            Return AnalyzerModelAttribute
        End Get
    End Property

    Public Shared ReadOnly Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDAttribute
        End Get
    End Property

    Public Shared ReadOnly Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDAttribute
        End Get
    End Property

    Public Shared Property ActiveStatus() As String
        Get
            Return WSStatusAttribute
        End Get
        Set(ByVal value As String)
            WSStatusAttribute = value
        End Set
    End Property

    Public ReadOnly Property NumOfTests() As Integer
        Get
            Return NumOfTestsAttribute
        End Get
    End Property

    Public WriteOnly Property CurrentLanguage() As String
        Set(ByVal value As String)
            If Not String.Equals(CurrentLanguageAttribute, value) Then
                CurrentLanguageAttribute = value
                GetScreenLabels()
                InitializeAlertGlobesTexts()
            End If
        End Set
    End Property

    Public WriteOnly Property AddNoMDIChildForm() As Form
        Set(ByVal value As Form)
            If Not NoMDIChildActiveFormsAttribute.Contains(value) Then
                NoMDIChildActiveFormsAttribute.Add(value)
            End If
        End Set
    End Property

    Public WriteOnly Property RemoveNoMDIChildForm() As Form
        Set(ByVal value As Form)
            If NoMDIChildActiveFormsAttribute.Contains(value) Then
                NoMDIChildActiveFormsAttribute.Remove(value)
            End If
        End Set
    End Property

    Public ReadOnly Property UserActionOverWorkSessionCurrentValue(ByVal pUserAction As WorkSessionUserActions) As Boolean
        Get
            Select Case pUserAction
                Case WorkSessionUserActions.NO_WS
                    Return False
                Case WorkSessionUserActions.START_WS
                    Return ExecuteSessionAttribute

                Case WorkSessionUserActions.PAUSE_WS
                    Return PauseSessionAttribute

                Case WorkSessionUserActions.ABORT_WS
                    Return AbortSessionAttribute

            End Select
        End Get
    End Property

    'AG 26/09/2011
    Public WriteOnly Property DisabledMdiForms() As Form
        Set(ByVal value As Form)
            If Not DisabledMDIChildAttribute.Contains(value) Then
                DisabledMDIChildAttribute.Add(value)
            End If
        End Set
    End Property

    Public WriteOnly Property RemoveDisabledMdiForms() As Form
        Set(ByVal value As Form)
            If DisabledMDIChildAttribute.Contains(value) Then
                DisabledMDIChildAttribute.Remove(value)
            End If
        End Set
    End Property
    'AG 26/09/2011

    Public ReadOnly Property WarmUpFinished() As Boolean
        Get
            Return WarmUpFinishedAttribute
        End Get
    End Property

    Public ReadOnly Property LoadingGlobes() As Boolean
        Get
            Return LoadingGlobesField
        End Get
    End Property

    ' XBC 08/06/2012
    Public Property ActiveFwVersion() As String
        Get
            Return FwVersionAttribute
        End Get
        Set(ByVal value As String)
            If (String.Compare(FwVersionAttribute, value, False) <> 0) Then
                FwVersionAttribute = value
            End If
        End Set
    End Property

    Public Property isResetWorkSessionActive() As Boolean
        Get
            Return isResetWSActiveAttribute
        End Get
        Set(ByVal value As Boolean)
            isResetWSActiveAttribute = value
        End Set
    End Property

    Public ReadOnly Property applicationNameForLIS() As String 'AG 27/02/2013
        Get
            Return appNameForLISAttribute
        End Get
    End Property

    Public ReadOnly Property channelIDForLIS() As String 'AG 27/02/2013
        Get
            Return channelIdForLISAttribute
        End Get
    End Property

    Public Property autoWSCreationWithLISMode() As Boolean 'AG 11/07/2013
        Get
            Return autoWSCreationWithLISModeAttribute
        End Get
        Set(ByVal value As Boolean)
            If autoWSCreationWithLISModeAttribute <> value Then
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'When change mark the process as not started!!!
                'AG 22/07/2013 When changes, clear the internal queue of specimen asked but with no response
                If Not MDILISManager Is Nothing Then
                    MDILISManager.ClearQueueOfSpecimenNotResponded()
                End If
            End If
            autoWSCreationWithLISModeAttribute = value
            If Not MDIAnalyzerManager Is Nothing Then
                'XB 23/07/2013 - auto HQ
                'MDIAnalyzerManager.autoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute 'Also inform the AnalyzerManager property
                MDIAnalyzerManager.autoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                'XB 23/07/2013
            End If
        End Set
    End Property

    Public WriteOnly Property LISWaitTime() As Integer
        Set(ByVal value As Integer)
            If LISWaitTimer.Interval <> CDbl(1000 * value) Then
                LISWaitTimer.Interval = 1000 * value
            End If
        End Set
    End Property

    'AG 06/11/2013 - Task #1375
    Public WriteOnly Property SinglecanningRequested As Boolean
        Set(value As Boolean)
            isSinglecanningRequestedAttribute = value
        End Set
    End Property

#End Region

#Region "Communications Events"

    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 27/05/2011
    ''' AG 25/02/2013 - declare as private instead of public
    ''' </remarks>
    Private Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles MDIAnalyzerManager.ReceptionEvent

        Me.UIThread(Function() ManageReceptionEvent(pInstructionReceived, pTreated, pRefreshEvent, pRefreshDS, pMainThread))

    End Sub

    'RH 30/05/2011 For recording generated UIRefreshDS data and reproduce bugs in Debug sessions
    'Private DSNumber As Integer = 0

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pInstructionReceived"></param>
    ''' <remarks>
    ''' AG 02/11/2010
    ''' Modified by: RH - 27/05/2011
    ''' modified by: DL - 16/09/2011 Lock prefreshDS copy to send to different functions
    '''              AG - 25/02/2013 - declare as private instead of public
    '''              XB - 23/07/2013 - Analyzer starts ringing everytime the Start Session process pauses and requires an user action. Stops ringing after the user chooses option
    '''              AG - 28/11/2013 - BT #1397 changes for recovery results if re-connection in running pause mode
    '''              XB - 16/12/2013 - Add ISE exception error cases management (like timeout, E:61) - Task #1441
    ''' </remarks>
    Private Function ManageReceptionEvent(ByVal pInstructionReceived As String, _
                                         ByVal pTreated As Boolean, _
                                         ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                         ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) As Boolean
        Dim myLogAcciones As New ApplicationLogManager()
        'RH Save UIRefreshDS for recording generated UIRefreshDS data and reproduce bugs in Debug sessions
        'pRefreshDS.WriteXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

        'RH Load UIRefreshDS for simulating generated UIRefreshDS data and reproduce bugs in Debug sessions
        'pRefreshDS.ReadXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

        'DSNumber += 1

        Try
            'Dim StartTime As DateTime = Now 'AG 12/06/2012 - time estimation

            'DL 16/09/2011
            'SyncLock ensures that multiple threads do not execute the statement block at the same time. 
            'SyncLock prevents each thread from entering the block until no other thread is executing it.
            'Protect pRefreshDS data from being updated by more than one thread simultaneously. 
            'If the statements that manipulate the data must go to completion without interruption, put them inside a SyncLock block.
            Dim copyRefreshDS As UIRefreshDS = Nothing
            Dim copyRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)

            ''TR 18/09/2012 -Log to trace incase Exception error is race.
            'myLogAcciones.CreateLogActivity("START BLOCK 0.", Me.Name & ".ManageReceptionEvent ", _
            '                                EventLogEntryType.FailureAudit, False)

            ' XB 30/08/2013
            If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSFCP_RECEIVED Then
                myLogAcciones.CreateLogActivity("START ANSFCP received with pTreated value as [" & pTreated & "] ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)
            End If

            'AG 04/04/2012 - This code is needed because the incomplete samples screen is open using ShowDialog
            'Check if the refresh event involves open the incomple samples screen
            Dim barcode_Samples_Warnings As Boolean = False
            If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then 'Samples barcode warnings
                        barcode_Samples_Warnings = True
                        incompleteSamplesOpenFlag = True
                    ElseIf lnqRes(0).Value = 2 Then 'No pending executions to be sent
                        processingBeforeRunning = "2" 'Finished Nok (no executions pending to be sent)
                        ScreenWorkingProcess = False
                    End If

                    'Once treated reset flag (This flag will be removed after treat flags barcode_Samples_Warnings or incompleteSamplesOpenFlag 
                    'or Warning message caused by value 2 in method ShowAlarmsOrSensorsWarningMessages)
                    'MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                End If
            End If
            'AG 04/04/2012

            SyncLock lockThis
                copyRefreshDS = CType(pRefreshDS.Copy(), UIRefreshDS)
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("BEFORE pRefreshEvent FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                For Each item As GlobalEnumerates.UI_RefreshEvents In pRefreshEvent
                    'copyRefreshEventList.Add(item)
                    If item = UI_RefreshEvents.BARCODE_POSITION_READ AndAlso incompleteSamplesOpenFlag Then
                        'In this case do not add item
                    Else
                        copyRefreshEventList.Add(item)
                    End If
                Next
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("AFTER pRefreshEvent FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)

                If pRefreshEvent.Count > 0 Then MDIAnalyzerManager.ReadyToClearUIRefreshDS(pMainThread) 'Inform the ui refresh dataset can be cleared so they are already copied
            End SyncLock
            'END DL 16/09/2011

            ''TR 18/09/2012 -Log to trace incase Exception error is race.
            'myLogAcciones.CreateLogActivity("END BLOCK 0.", Me.Name & ".ManageReceptionEvent ", _
            '                                EventLogEntryType.FailureAudit, False)

            If pTreated Then '(1) 
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 1.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)
                '////////////////////////////
                'REFRESH MDI BUTTONS & STATUS
                '////////////////////////////

                'Refresh button bar the Ax00 and the connection Status 

                'AG 27/02/2012 - previous code generate uirefresh event every instruction received. New code 24/02/2012 generate uirefresh events
                'in running only when receive the ansphr readings so add the 2on condition part
                'If MDIAnalyzerManager.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED Then '(2)
                If MDIAnalyzerManager.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED OrElse _
                   MDIAnalyzerManager.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED OrElse _
                   (MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING AndAlso pMainThread) Then '(2)
                    SetActionButtonsEnableProperty(True) 'AG 27/02/2012

                    ShowLabelInStatusBar() 'AG 23/10/2013 - move next code into this method

                    'AG 22/03/2012 - exception case: when connection is performed in StandBy keep the status text "Connection Process"
                    'until the ISE module is initialized (use the ANSISE instruction)

                    ' XBC 03/10/2012 - Correction : some case didn't actives the menu
                    ' ElseIf MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso _
                    ' MDIAnalyzerManager.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then

                ElseIf MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso _
                       MDIAnalyzerManager.AdjustmentsRead Then
                    ' XBC 03/10/2012

                    '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
                    If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
                       MDIAnalyzerManager.ISE_Manager.ConnectionTasksCanContinue Then

                        If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
                            ShowStatus(Messages.STARTING_INSTRUMENT)
                        Else
                            'SGM 10/04/2012 not to show Standby in case of ISE utilities open
                            If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IISEUtilities Then
                            Else
                                If Not ProcessingBusinessInCourse() AndAlso Not Me.ShutDownisPending AndAlso Not Me.StartSessionisPending Then ' AG 08/11/2012 - PENDING Validate - Execute only when the working process progress bar is activate
                                    'If Not Me.ShutDownisPending And Not Me.StartSessionisPending Then ' XBC 23/07/2012
                                    ShowStatus(Messages.STANDBY)

                                    ' XB 06/11/2013 - Force Refresh screen just the first time when Analyzer is connected on StandBy
                                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                                        If ForceRefreshOnFirstStandBy Then
                                            ForceRefreshOnFirstStandBy = False
                                            EnableButtonAndMenus(True, True)
                                        End If
                                    End If


                                    ' AG 07/11/2012 - Avoid blinking every 2 seconds when analyzer is connected and we are in StandBy
                                    '                 Do not call this method because is also called in EnableButtonAndMenus
                                    'SetActionButtonsEnableProperty(True) 'Call to activate vertical action buttons
                                    EnableButtonAndMenus(True) 'TR 20/09/2012 -uncommented by TR.  ('SA 07/09/2012 commented by SA.)
                                End If
                            End If
                        End If
                    End If

                End If '(2)

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 1.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                ChangeErrorStatusLabel(False) 'AG 18/05/2012 - When some process is paused by some error set the error providers text to 'Not Finished'

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 2.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)
                '////////////////
                ' REFRESH SCREENS
                '////////////////

                Dim monitorTreated As Boolean = False
                Dim resultTreated As Boolean = False
                Dim wsRotorPositionTreated As Boolean = False
                Dim changeRotorTreated As Boolean = False
                Dim conditioningTreated As Boolean = False
                Dim configSettingsLISTreated As Boolean = False    ' XB 24/04/2013
                'Refreh UI screens depending the current screen and the pRefreshEvent received
                'use the method RefreshScreen

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS) Then
                    'Call the RefreshScreen method when the current screen is:
                    '- Monitor (WSStates) ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)

                    'RH 10/10/2011
                    If Not ActiveMdiChild Is Nothing Then
                        If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            monitorTreated = True
                        End If
                    End If
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED) Then
                    '[This case means new results calculated + new readings received!!]
                    'Call the RefreshScreen method when the current screen is:

                    'RH 10/10/2011
                    If Not ActiveMdiChild Is Nothing Then
                        '- Monitor (WSStates ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                        If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            monitorTreated = True

                            '- ResultForm ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                        ElseIf (TypeOf ActiveMdiChild Is IResults AndAlso Not resultTreated) Then
                            Dim CurrentMdiChild As IResults = CType(ActiveMdiChild, IResults)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)

                            resultTreated = True

                            'CurveResultForm ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                            'BE CAREFULL: CurveResultForm & Graphical Abs(t) screen are open as popup not as MDIChild
                            ' DL 24/05/2011
                            Dim found As Boolean = False
                            Dim index As Integer
                            ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                            'myLogAcciones.CreateLogActivity("1- BEFORE NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                            For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                                If String.Compare(NoMDIChildActiveFormsAttribute.Item(index).Name, IResultsCalibCurve.Name, False) = 0 Then
                                    found = True
                                    Exit For
                                End If
                            Next index
                            ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                            'myLogAcciones.CreateLogActivity("1- AFTER NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)

                            If found Then
                                Dim CurrentNoMdiChild As IResultsCalibCurve
                                CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), IResultsCalibCurve)

                                With CurrentNoMdiChild
                                    .ActiveAnalyzer = AnalyzerIDAttribute
                                    .ActiveWorkSession = WorkSessionIDAttribute
                                    .AnalyzerModel = AnalyzerModelAttribute
                                    .AverageResults = CurrentMdiChild.AverageResults
                                    .ExecutionResults = CurrentMdiChild.ExecutionResults
                                    '.TestSelectedName = CurrentMdiChild.SelectedTestName
                                    .AcceptedRerunNumber = CurrentMdiChild.AcceptedRerunNumber
                                End With

                                CurrentNoMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentNoMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            End If
                            ' END DL 24/05/2011

                        End If

                    End If
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.READINGS_RECEIVED) Then
                    '[This case means new results calculated + new readings received!!]
                    'Call the RefreshScreen method when the current screen is:

                    If Not ActiveMdiChild Is Nothing Then
                        'Graphical Abs(t) screen ... (pRefreshDS.ReceivedReadings contains the information to refresh)
                        'BE CAREFULL: Graphical Abs(t) screen are open as popup not as MDIChild

                        ' DL 24/05/2011
                        Dim found As Boolean = False
                        Dim index As Integer
                        ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                        'myLogAcciones.CreateLogActivity("2- BEFORE NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                        For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                            If NoMDIChildActiveFormsAttribute.Item(index).Name = IResultsAbsCurve.Name Then
                                found = True
                                Exit For
                            End If
                        Next index
                        ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                        'myLogAcciones.CreateLogActivity("2- After NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                        If found Then
                            Dim CurrentNoMdiChild As IResultsAbsCurve
                            CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), IResultsAbsCurve)
                            CurrentNoMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentNoMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        End If
                        ' END DL 24/05/2011

                    End If
                End If


                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                    '[This case means new results calculated + new readings received!!]
                    'Call the RefreshScreen method when the current screen is:

                    'AG 17/10/2011 - during the application load and autoconnect the ActiveMdiChild is nothing but the current screen is monitor
                    Dim MdiChildIsDisabled As Boolean = False
                    Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
                    If DisabledMDIChildAttribute.Count > 0 Then
                        MdiChildIsDisabled = True
                    End If

                    Dim myCurrentMDIForm As Form = Nothing
                    If Not ActiveMdiChild Is Nothing Then
                        myCurrentMDIForm = ActiveMdiChild
                    ElseIf MdiChildIsDisabled Then
                        myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    End If
                    'AG 17/10/2011

                    If Not myCurrentMDIForm Is Nothing Then
                        '- Monitor (Main or Alarms) screen ... (pRefreshDS.ReceivedAlarms contains the information to refresh)
                        '- Change rotor ... (pRefreshDS.SensorValueChanged contains the information to refresh)

                        If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            monitorTreated = True
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                            'AG 16/03/2012 - If no reactions rotor alarm appears while the UI is disabled we must reactivated it
                            'Change rotor
                        ElseIf (TypeOf myCurrentMDIForm Is IChangeRotor AndAlso Not changeRotorTreated) Then
                            Dim CurrentMdiChild As IChangeRotor = CType(myCurrentMDIForm, IChangeRotor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            changeRotorTreated = True
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                            'AG 16/03/2012

                            'AG 28/03/2012 - When cover open and enabled alarms appears it disables Scan barcode button. Else enable it
                            'Sample request

                        ElseIf (TypeOf myCurrentMDIForm Is IConditioning AndAlso Not conditioningTreated) Then
                            Dim CurrentMdiChild As IConditioning = CType(myCurrentMDIForm, IConditioning)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            'conditioningTreated = True
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'DL 05/06/2012
                            'DL 05/06/2012

                        ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                            Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                            'AG 28/03/2012 - When cover open and enabled alarms appears it disables Scan barcode /Check bottle volume button. Else enable it
                            'Rotor positions
                        ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions) Then
                            Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                            'AG 28/03/2012 - When cover open and enabled alarms appears it disables ISE command button. Else enable it
                        ElseIf (TypeOf myCurrentMDIForm Is IISEUtilities) Then
                            Dim CurrentMdiChild As IISEUtilities = CType(myCurrentMDIForm, IISEUtilities)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        End If

                    End If

                    'AG 17/10/2011
                    If processingBeforeRunning = "0" Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in progress
                    If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                        'Activate the current mdi child who is disabled once the refresh method is finished
                        If DisabledMDIChildAttribute.Count > 0 Then
                            DisabledMDIChildAttribute(0).Enabled = True
                            If DisabledMDIChildAttribute.Count = 1 Then
                                DisabledMDIChildAttribute.Clear()
                            Else
                                DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                            End If
                        End If
                    End If
                    'AG 17/10/2011

                    'DL 31/07/2012. Begin
                    Dim linq As New List(Of UIRefreshDS.ReceivedAlarmsRow)

                    'DisconnectComms GUI
                    linq = (From a As UIRefreshDS.ReceivedAlarmsRow In copyRefreshDS.ReceivedAlarms _
                             Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CPU_ERR.ToString) _
                            OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR.ToString) _
                            OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_REPOSITORY_ERR.ToString) _
                            OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR.ToString) _
                            OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_MAN_ERR.ToString)) _
                               And a.AlarmStatus = True _
                            Select a).ToList

                    If linq.Count > 0 Then DisconnectComms()
                    'DL 31/07/2012. End

                    'Alarm Messages (Messages do not required ActiveMdiChild)
                    '--------------------------------------------------------
                    '(All screens) Finally show message depending the alarms received and update vertical button bar depending the current alarms
                    ShowAlarmsOrSensorsWarningMessages(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, copyRefreshDS) 'DL 16/09/2011 ShowAlarmWarningMessages(pRefreshDS)
                    SetActionButtonsEnableProperty(True)
                    Debug.Print("ShowAlarmsOrSensorsWarningMessages called from ManageReceptionEvent-1")
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                    '[This case means new results calculated + new readings received!!]
                    'Call the RefreshScreen method when the current screen is:

                    'AG 20/01/2012 - during the application load and autoconnect the ActiveMdiChild is nothing but the current screen is monitor
                    Dim MdiChildIsDisabled As Boolean = False
                    Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
                    If DisabledMDIChildAttribute.Count > 0 Then
                        MdiChildIsDisabled = True
                    End If

                    Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
                    If Not ActiveMdiChild Is Nothing Then
                        myCurrentMDIForm = ActiveMdiChild
                    ElseIf MdiChildIsDisabled Then
                        myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    End If
                    'AG 12/01/2012

                    If Not myCurrentMDIForm Is Nothing Then

                        'AG 27/08/2012 - Results recovery process has an special screen 
                        If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" AndAlso _
                           MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS) = 1 AndAlso _
                           Not (TypeOf myCurrentMDIForm Is IResultsRecover) Then

                            'AG 28/11/2013 - BT #1397
                            'Comment the old code
                            ''Open the results recover popup screen
                            'OpenMDIChildForm(IResultsRecover)
                            'EnableButtonAndMenus(False)
                            If Not MDIAnalyzerManager.AllowScanInRunning Then
                                'Normal running ... same code as in previous versions
                                'Open the results recover popup screen
                                recoveryResultsWarnFlag = False

                                OpenMDIChildForm(IResultsRecover)
                                EnableButtonAndMenus(False)

                                'Only show message one time
                                'Do not shown when the incomplete samples (or HQMonitor) screen are open because they are TopMost
                            ElseIf Not recoveryResultsWarnFlag Then

                                'Pause running ... show message to user and he has to choose between EXIT pause or ABORT
                                'The message is market to be shown when the auxiliary screen for incomplete samples is closed (it has TopMost = TRUE)
                                recoveryResultsWarnFlag = True
                            End If
                            'AG 28/11/2013

                        ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" AndAlso _
                               MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS) = 0 AndAlso _
                               (TypeOf myCurrentMDIForm Is IResultsRecover) Then

                            'Close the results recover popup screen and open the monitor screen
                            WSStatusAttribute = "ABORTED" 'After recovery results the worksession becomes aborted
                            Debug.Print("ManageReceptionEvent -> ABORTED -> CURRENT MDI FORM = " & myCurrentMDIForm.Name)
                            OpenMonitorForm(myCurrentMDIForm) 'myCurrentMDIForm = IResultsRecover = form to close
                            EnableButtonAndMenus(True)


                        Else 'Normal code

                            '- Monitor (Main) ... (pRefreshDS.SensorValueChanged contains the information to refresh)
                            '  <or maybe all monitor TAB's (LED's area)>
                            '- Change rotor ... (pRefreshDS.SensorValueChanged contains the information to refresh)
                            'NOTE: All sensor values are available using the property AnalyzerManager.GetSensorValue
                            If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                                Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                                monitorTreated = True
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                            ElseIf (TypeOf myCurrentMDIForm Is IChangeRotor AndAlso Not changeRotorTreated) Then
                                Dim CurrentMdiChild As IChangeRotor = CType(myCurrentMDIForm, IChangeRotor)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                                changeRotorTreated = True
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                                ' XBC 08/02/2012 ISE Utilities placed into PresetationCOM layer
                                'AG 12/01/2012


                            ElseIf (TypeOf myCurrentMDIForm Is IConditioning AndAlso Not conditioningTreated) Then
                                Dim CurrentMdiChild As IConditioning = CType(myCurrentMDIForm, IConditioning)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                conditioningTreated = True
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone

                                'DL 05/06/2012 ISE Utilities placed into PresetationCOM layer

                            ElseIf (TypeOf myCurrentMDIForm Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(myCurrentMDIForm, IISEUtilities)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                                'ElseIf (TypeOf myCurrentMDIForm Is Biosystems.Ax00.PresentationCOM.IISEUtilities) Then
                                '    Dim CurrentMdiChild As Biosystems.Ax00.PresentationCOM.IISEUtilities = CType(myCurrentMDIForm, Biosystems.Ax00.PresentationCOM.IISEUtilities)
                                '    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                '    refreshTriggeredFlag = True
                                'AG 12/01/2012
                                ' XBC 08/02/2012 ISE Utilities placed into PresetationCOM layer

                                'AG 16/03/2012 - If freeze appears while this screen is disable Sw must re-activate it
                                'Sample request
                            ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                                Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                                'Rotor positioning
                            ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions AndAlso Not wsRotorPositionTreated) Then
                                Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                                wsRotorPositionTreated = True
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                                'AG 16/03/2012

                                ' XB 24/04/2013
                            ElseIf (TypeOf myCurrentMDIForm Is IConfigLIS AndAlso Not configSettingsLISTreated) Then
                                Dim CurrentMdiChild As IConfigLIS = CType(myCurrentMDIForm, IConfigLIS)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                configSettingsLISTreated = True
                                refreshTriggeredFlag = CurrentMdiChild.RefreshDone

                            End If


                        End If
                        'AG 27/08/2012


                        'AG 23/09/2011 - Check if analyzer status has changed when is opened a screen witch business depends on analyzer status
                        If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED) = 1 Then
                            'Several screens have business witch depends on the analyzer status ... so if the analyzer status changes when screen is opened
                            'they has to be notified (IWSRotorPositions, IConfigAnalyzer, IBarcodeCondig, IChangeRotor, ISatReport_Load, ISettings, IWSSampleRequest, ISatReport ...)
                            TreatScreenDueAnalyzerStatusChanged()
                            MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED) = 0 'Once UI refresh ... clear the sensor
                        End If
                        'AG 23/09/2011

                        'AG 20/01/2012
                        If String.Compare(processingBeforeRunning, "0", False) = 0 Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
                        If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                            'Activate the current mdi child who is disabled once the refresh method is finished
                            If DisabledMDIChildAttribute.Count > 0 Then
                                DisabledMDIChildAttribute(0).Enabled = True
                                If DisabledMDIChildAttribute.Count = 1 Then
                                    DisabledMDIChildAttribute.Clear()
                                Else
                                    DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                                End If
                            End If
                        End If
                        'AG 20/01/2012

                    End If

                    'AG 23/03/2012 - If errors while status changing Sw must assure the UI becomes enabled again
                    If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING) = 1 Then
                        If ScreenWorkingProcess Then
                            ScreenWorkingProcess = False
                            processingBeforeRunning = "1"

                            'clear the possible sensor values affected
                            MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 0
                            MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0
                            MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0
                        End If

                        'Finally clear the error in change status sensor
                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING) = 0
                    End If

                    'DL 18/01/2012 - Check if process before running is finished or aborted and then enable UI again
                    If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 1 Then
                        'AG 13/02/2012 - these 2 lines must be performed only when BAx00 becomes in RUNNING, not before
                        'Time Ago it works because the running instruction was inmediate but in latest Fw versions Running requires 1 minut or more
                        'That is why I have to move them into method ActivateActionButtonBarOrSendNewAction (case Running)
                        'MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0 'clear sensor
                        'processBeforeRunningFinished = "1" 'Finished ok
                        'AG 13/02/2012
                        If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 1 Then
                            processingBeforeRunning = "2" 'Aborted due ise alarms
                            ScreenWorkingProcess = False
                        End If
                    End If

                    'AG 03/10/2012 - I think the 2on part of condition was wrong!!
                    'AG 26/03/2012 - Stop sound alarm button disabled when analyzer not sound and when sound off instruction is in queue to be sent ('TR 27/10/2011)
                    If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_SOUND_CHANGED) = 0 OrElse MDIAnalyzerManager.QueueContains(AnalyzerManagerSwActionList.SOUND) Then
                        'If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_SOUND_CHANGED) = 0 OrElse MDIAnalyzerManager.QueueContains(AnalyzerManagerSwActionList.ENDSOUND) Then
                        bsTSSoundOff.Enabled = False 'DL 22/03/212
                    Else
                        bsTSSoundOff.Enabled = True 'DL 22/03/212
                    End If
                    'TR 27/10/2011 -END

                    'AG 07/03/2012
                    If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                        ScreenWorkingProcess = False
                        processingRecover = False
                    End If

                    'AG 15/05/2012
                    If MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM) = 1 Then
                        'Clear the sensor once performed the business and stops the timer
                        If ElapsedTimeTimer.Enabled Then
                            ElapsedTimeTimer.Stop()
                            'TR 15/05/2012 clean the valible that handle the time because initialization required.
                            TotalSecs = 0
                            TotalSecs2 = 0
                            'TR 15/05/2012 -END.
                        End If

                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM) = 0
                    End If

                    'AG 14/03/2012 - Business only when specific sensors changes (are informed in copyRefreshDS) (code is moved from ShowAlarmsOrSensorsWarningMessages)
                    Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)

                    'WarmUp canceled due bottle alarms
                    lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                             Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS.ToString, False) = 0 _
                             Select a).ToList
                    If lnqRes.Count > 0 Then
                        If lnqRes(0).Value = 1 Then
                            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) = 0 Then
                                FinishWarmUp(True)
                            End If
                        End If
                    End If

                    'Freeze while analyzer is processing before running
                    lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                              Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.FREEZE.ToString, False) = 0 _
                              Select a).ToList
                    If lnqRes.Count > 0 Then
                        If processingBeforeRunning = "0" Then processingBeforeRunning = "2"
                        If ScreenWorkingProcess Then ScreenWorkingProcess = False

                        StopMarqueeProgressBar()
                        EnableButtonAndMenus(True)
                        SetActionButtonsEnableProperty(True)
                        Cursor = Cursors.Default
                    End If
                    'AG 14/03/2012

                    'AG 23/03/2012 No connection while screen is working leave the loop and activate screen
                    lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                              Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString) _
                              Select a).ToList
                    If lnqRes.Count > 0 Then
                        If lnqRes(0).Value = 0 Then 'Note this sensor works different (1 connected (ok), 0 (no connected, connection fails, error))
                            If ScreenWorkingProcess Then
                                ScreenWorkingProcess = False
                                processingBeforeRunning = "1"

                                'clear the possible sensor values affected
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 0
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0
                            End If

                        Else 'AG 23/05/2012 Connection establisment ok refresh monitor.main
                            If Not myCurrentMDIForm Is Nothing Then
                                If (TypeOf myCurrentMDIForm Is IMonitor) Then
                                    Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                                    CurrentMdiChild.RefreshAlarmsGlobes(Nothing)
                                End If
                            End If
                            'AG 23/05/2012

                        End If
                        'TR 20/09/2012 Commented by TR. 
                        'EnableButtonAndMenus(True)
                        Cursor = Cursors.Default
                    End If


                    'Sensor Messages (messages do not required ActiveMdiChild)
                    '---------------------------------------------------------
                    ShowAlarmsOrSensorsWarningMessages(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED, copyRefreshDS) 'AG 07/03/2012
                    Debug.Print("ShowAlarmsOrSensorsWarningMessages called from ManageReceptionEvent-2")

                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED) Then
                    '[This case means new results calculated + new readings received!!]
                    'Call the RefreshScreen method when the current screen is:

                    If Not ActiveMdiChild Is Nothing Then
                        '- Monitor (Reactions Rotor) screen ... (pRefreshDS.ReactionWellStatusChanged contains the information to refresh)
                        If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            monitorTreated = True
                        End If

                    End If
                End If


                'If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) Then
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) OrElse _
                    copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then
                    '[This case means new results calculated + new readings received!!]

                    If watchDogTimer.Enabled Then watchDogTimer.Enabled = False

                    'AG 26/09/2011 - during the barcode read the current screen is disable (son ActiveMdiChild is nothing)
                    Dim MdiChildIsDisabled As Boolean = False
                    Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
                    If DisabledMDIChildAttribute.Count > 0 Then
                        MdiChildIsDisabled = True
                    End If

                    Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing

                    If Not ActiveMdiChild Is Nothing Then
                        myCurrentMDIForm = ActiveMdiChild
                    ElseIf MdiChildIsDisabled Then
                        myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    End If
                    'AG 26/09/2011

                    'Call the RefreshScreen method when the current screen is:
                    If Not myCurrentMDIForm Is Nothing Then
                        '- Monitor (Reagents or Sample Rotor) screen ... (pRefreshDS.RotorPositionChanged contains the information to refresh)
                        If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            monitorTreated = True
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                            '- Rotor position screen ... (pRefreshDS.RotorPositionChanged contains the information to refresh)
                        ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions AndAlso Not wsRotorPositionTreated) Then
                            Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            wsRotorPositionTreated = True
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                        ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                            Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        End If
                    End If

                    If String.Compare(processingBeforeRunning, "0", False) = 0 Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
                    If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                        'Activate the current mdi child who is disabled once the refresh method is finished
                        If DisabledMDIChildAttribute.Count > 0 Then

                            'DL 19/01/2012 Only activate the monitor if not scanning
                            If String.Compare(processingBeforeRunning, "1", False) = 0 Then
                                DisabledMDIChildAttribute(0).Enabled = True

                                If DisabledMDIChildAttribute.Count = 1 Then
                                    DisabledMDIChildAttribute.Clear()
                                Else
                                    DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                                End If
                            End If
                            'DL 19/01/2012

                        End If
                    End If

                End If
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 2.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 3.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                'LAST EVENT REFRESH TREATMENT DUE some PopUp screen can be opened
                '================================================================

                'AG 07/01/2014 - BT #1445 protection, if recover results INPROCESS do not open the HQ barcode neither the incomplete samples screens
                If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" Then
                    barcode_Samples_Warnings = False
                    incompleteSamplesOpenFlag = False
                End If
                'AG 07/01/2014

                'AG 05/09/2011 - Finally open popup screen  when barcode warnings
                'When sensor BARCODE_WARNINGS = 1 then screens Monitor, WSRotorPositions & WSPreparation (open the auxiliary screen IWSIncompleteSamplesAuxScreen)
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) AndAlso _
                   barcode_Samples_Warnings Then 'MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 1 Then

                    Dim MdiChildIsDisabled As Boolean = False
                    Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
                    If DisabledMDIChildAttribute.Count > 0 Then
                        MdiChildIsDisabled = True
                    End If

                    Dim myCurrentMDIForm As Form = Nothing

                    If Not ActiveMdiChild Is Nothing Then
                        myCurrentMDIForm = ActiveMdiChild
                    ElseIf MdiChildIsDisabled Then
                        myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    End If

                    If Not myCurrentMDIForm Is Nothing Then
                        processingBeforeRunning = "2" 'Start instrument failed because some barcode alerts must be solved

                        'AG 03/04/2012 - In these case stop the mdi bar progress bar and enabled menus ... a new popup screen is opened
                        ScreenWorkingProcess = False
                        StopMarqueeProgressBar()
                        EnableButtonAndMenus(True)
                        ShowStatus(Messages.STANDBY)
                        Cursor = Cursors.Default

                        'Once treated reset flag
                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                        'AG 03/04/2012 

                        'AG 08/03/2013 - Ask the user for open the incomplete samples or skip them and enter in running mode
                        'In v1.0.0 the auxiliary screen will be always opened with no user confirmation
                        'Open popup the IWSIncompleteSamplesAuxScreen 
                        'Using MyForm As New IWSIncompleteSamplesAuxScreen()
                        '    MyForm.AnalyzerID = AnalyzerIDAttribute
                        '    MyForm.WorkSessionID = WorkSessionIDAttribute
                        '    MyForm.WorkSessionStatus = WSStatusAttribute
                        '    MyForm.SourceScreen = SourceScreen.START_BUTTON
                        '    'Validate if active screen is Sample request, 'cause requires the WorkSessionResultDS.
                        '    If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                        '        MyForm.SourceScreen = SourceScreen.SAMPLE_REQUEST
                        '        MyForm.WSOrderTests = CType(myCurrentMDIForm, IWSSampleRequest).myWorkSessionResultDS
                        '    End If

                        '    MyForm.ShowDialog()
                        '    incompleteSamplesOpenFlag = False

                        '    'TR 14/09/2011 -Set worksession status and set property ChangesMade when WS Samples Request is open
                        '    WSStatusAttribute = MyForm.WorkSessionStatus
                        '    If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                        '        CType(myCurrentMDIForm, IWSSampleRequest).ChangesMade = MyForm.ChangesMade
                        '    End If
                        'End Using

                        Dim myAction As AnalyzerManager.BarcodeWorksessionActions = AnalyzerManager.BarcodeWorksessionActions.NO_RUNNING_REQUEST
                        myAction = MDIAnalyzerManager.BarCodeProcessBeforeRunning

                        Dim userAns As DialogResult = Windows.Forms.DialogResult.Yes 'By default show the incomplete samples screen
                        If myAction = AnalyzerManager.BarcodeWorksessionActions.ENTER_RUNNING Then
                            'Ask question only in start ws process
                            'userAns = ShowMessage("Question", GlobalEnumerates.Messages.CONFIRM_BARCODE_WARNING.ToString)
                            'AG 09/07/2013 - In automate mode do not ask the user
                            'XB 23/07/2013 - auto HQ
                            'If Not autoWSCreationWithLISModeAttribute Then
                            If Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag Then
                                ' XB 06/11/2013 - Activate buzzer
                                If Not MDIAnalyzerManager Is Nothing Then
                                    MDIAnalyzerManager.StartAnalyzerRinging()
                                End If

                                'XB 23/07/2013
                                userAns = ShowMessage("Question", GlobalEnumerates.Messages.CONFIRM_BARCODE_WARNING.ToString)

                                ' XB 06/11/2013 - Close buzzer
                                If Not MDIAnalyzerManager Is Nothing Then
                                    MDIAnalyzerManager.StopAnalyzerRinging()
                                End If
                            End If
                            MDIAnalyzerManager.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE
                        End If

                        If (userAns = Windows.Forms.DialogResult.Yes) Then
                            EnableButtonAndMenus(True, True)   ' XB 07/11/2013

                            'AG 03/04/2013 - Open the incomplete samples or HostQuery screen depending this setting
                            Dim lisWithFilesMode As Boolean = False
                            Dim resultData As New GlobalDataTO
                            Dim userSettings As New UserSettingsDelegate
                            resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                lisWithFilesMode = CType(resultData.SetDatos, Boolean)
                            End If

                            If lisWithFilesMode Then
                                'Open popup the IWSIncompleteSamplesAuxScreen 
                                Using MyForm As New IWSIncompleteSamplesAuxScreen()
                                    MyForm.AnalyzerID = AnalyzerIDAttribute
                                    MyForm.WorkSessionID = WorkSessionIDAttribute
                                    MyForm.WorkSessionStatus = WSStatusAttribute
                                    MyForm.SourceScreen = SourceScreen.START_BUTTON
                                    'Validate if active screen is Sample request, 'cause requires the WorkSessionResultDS.
                                    If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                                        MyForm.SourceScreen = SourceScreen.SAMPLE_REQUEST
                                        MyForm.WSOrderTests = CType(myCurrentMDIForm, IWSSampleRequest).myWorkSessionResultDS
                                    End If

                                    MyForm.ShowDialog()
                                    incompleteSamplesOpenFlag = False

                                    'TR 14/09/2011 -Set worksession status and set property ChangesMade when WS Samples Request is open
                                    WSStatusAttribute = MyForm.WorkSessionStatus
                                    If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                                        CType(myCurrentMDIForm, IWSSampleRequest).ChangesMade = MyForm.ChangesMade
                                    End If
                                End Using

                            Else 'HostQuery screen
                                'Open popup the HostQuery screen 
                                Dim myOriginalStatus As String = bsAnalyzerStatus.Text 'DL 17/07/2013
                                'DL 18/07/2013
                                'bsAnalyzerStatus.Text = myAutoLISWaitingOrder
                                ShowStatus(Messages.AUTOLIS_WAITING_ORDERS)

                                Using MyForm As New HQBarcode
                                    MyForm.AnalyzerID = AnalyzerIDAttribute
                                    MyForm.WorkSessionID = WorkSessionIDAttribute
                                    MyForm.WorkSessionStatus = WSStatusAttribute
                                    MyForm.SourceScreen = SourceScreen.START_BUTTON
                                    'JC-SG 10/05/2013 Screen sometimes appear on bacground of MDIForm, and app looks like has hang out
                                    '#If Not Debug Then
                                    MyForm.TopMost = True
                                    '#End If

                                    'AG 08/07/2013 - Special mode for work with LIS with automatic actions
                                    'XB 23/07/2013 - auto HQ
                                    'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                        'XB 23/07/2013
                                        MyForm.OpenByAutomaticProcess = True
                                        'XB 23/07/2013 - auto HQ
                                        'MyForm.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute   ' XB 17/07/2013 - Auto WS process
                                        MyForm.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                                        'XB 23/07/2013
                                        EnableLISWaitTimer(False)
                                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessAskBySpecimen)
                                    End If
                                    'AG 08/07/2013

                                    AddNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is shown

                                    MyForm.ShowDialog()
                                    RemoveNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is closed
                                    incompleteSamplesOpenFlag = False

                                    'TR 14/09/2011 -Set worksession status and set property ChangesMade when WS Samples Request is open
                                    WSStatusAttribute = MyForm.WorkSessionStatus
                                End Using

                                bsAnalyzerStatus.Text = myOriginalStatus 'DL 17/07/2013

                                'AG 08/07/2013 - Special mode for work with LIS with automatic actions
                                'XB 23/07/2013 - auto HQ
                                'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                    'XB 23/07/2013
                                    myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: HostQuery monitor screen closed", "IAx00MainMDI.ManageReceptionEvent", EventLogEntryType.Information, False)

                                    'Check if there is something received from LIS pending to be add to WS (see conditions instead of button status)
                                    'If bsTSOrdersDownloadButton.Enabled Then 'Exists lis workorders pending to be added
                                    Dim myESRulesDlg As New ESBusiness
                                    If myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage) Then
                                        'OK - normal case
                                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessDownloadOrders)

                                    ElseIf automateProcessCurrentState = LISautomateProcessSteps.subProcessAskBySpecimen Then
                                        'Exception: No workorders received from LIS
                                        SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitNoWorkOrders)
                                        InitializeAutoWSFlags()

                                    ElseIf automateProcessCurrentState = LISautomateProcessSteps.ExitAutomaticProcesAndStop Then
                                        'Exception: Impossible ask for specimen and user decides stop
                                        EnableButtonAndMenus(True, True) 'Enable buttons before update attribute!!
                                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                        InitializeAutoWSFlags()
                                    End If
                                End If
                                'AG 08/07/2013

                            End If

                        Else 'Skip incomplete samples and continue enter Running
                            Cursor = Cursors.WaitCursor
                            SetActionButtonsEnableProperty(False)
                            EnableButtonAndMenus(False)
                            readBarcodeIfConfigured = False 'Enter running without barcode read neither warnings
                            StartEnterInRunningMode()
                        End If
                        'AG 08/03/2013

                        'TR 14/09/2011 -Refresh screen
                        If TypeOf myCurrentMDIForm Is IWSRotorPositions Then
                            Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                            CurrentMdiChild.ScreenWorkingProcess = False
                            CurrentMdiChild.RefreshAfterSamplesWithoutRequest(WSStatusAttribute)
                            refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        ElseIf TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                            'TR 16/09/2011
                            CType(myCurrentMDIForm, IWSSampleRequest).ScreenWorkingProcess = False
                            CType(myCurrentMDIForm, IWSSampleRequest).ActiveWSStatus = WSStatusAttribute
                            CType(myCurrentMDIForm, IWSSampleRequest).ActiveWorkSession = WorkSessionIDAttribute
                            CType(myCurrentMDIForm, IWSSampleRequest).ChangesMade = True
                            refreshTriggeredFlag = True
                            Cursor = Cursors.Default
                        Else
                            'AG 11/03/2013 - Open rotor positions screen (only if the auxiliary screen for complete samples has been shown and closed)
                            'OpenRotorPositionsForm(myCurrentMDIForm) 'AG 03/04/2012 - Parameter is nothing using myCurrentMDIForm a system error is triggered (myCurrentMDIForm)
                            'XB 23/07/2013 - auto HQ
                            'If userAns = Windows.Forms.DialogResult.Yes AndAlso Not autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                            If userAns = Windows.Forms.DialogResult.Yes AndAlso (Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                                'XB 23/07/2013
                                'AG 09/07/2013 - add condition not AUTO_WS_WITH_LIS_MODE
                                OpenRotorPositionsForm(myCurrentMDIForm)
                            End If
                        End If
                        'TR 14/09/2011 -END
                    End If

                    If String.Equals(processingBeforeRunning, "0") Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
                    If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                        'Activate the current mdi child who is disabled once the refresh method is finished
                        If DisabledMDIChildAttribute.Count > 0 Then
                            DisabledMDIChildAttribute(0).Enabled = True
                            If DisabledMDIChildAttribute.Count = 1 Then
                                DisabledMDIChildAttribute.Clear()
                            Else
                                DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                            End If
                        End If
                    End If
                    'AG 05/09/2011

                End If

                'SGM 12/03/2012
                ' XB 24/10/2013 - Specific ISE commands are allowed in RUNNING (pause mode) - BT #1343
                'If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then
                If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Or _
                   pInstructionReceived.Contains("A400;ANSISE;") Then
                    ' XB 24/10/2013
                    '
                    ' ISE RECEIVED
                    '

                    ' XB 03/12/2013 - protection : Ensure that connected Analyzer is fine registered on ISE table DB - task #1410
                    If CheckAnalyzerIDOnFirstConnectionForISE Then
                        If MDIAnalyzerManager.Connected AndAlso MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                                If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                                    MDIAnalyzerManager.ISE_Manager.UpdateAnalyzerInformation(AnalyzerIDAttribute, AnalyzerModelAttribute)
                                    CheckAnalyzerIDOnFirstConnectionForISE = False
                                End If
                            End If
                        End If
                    End If

                    ' Refresh BarCode Info screen
                    If Not ActiveMdiChild Is Nothing Then

                        If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                            Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        ElseIf (TypeOf ActiveMdiChild Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            monitorTreated = True
                        Else ' XBC 10/12/2012 - Correction : Allow ISE receptions also on another active screens, not only IMonitor and IISEUtilities
                            'ISE procedure finished
                            Dim sensorValue As Single = 0
                            sensorValue = MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                                ISEProcedureFinished = True
                            End If
                        End If

                    Else

                        'if no screen is active
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                            Dim sensorValue As Single = 0


                            ''ISE switch on changed
                            'sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED)
                            'If sensorValue = 1 Then
                            '    ScreenWorkingProcess = False
                            '    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED) = 0 'Once updated UI clear sensor

                            'End If

                            ''ISE initiated
                            'sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                            'If sensorValue >= 1 Then
                            '    ScreenWorkingProcess = False

                            '    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                            'End If

                            ''ISE ready changed
                            'sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED)
                            'If sensorValue = 1 Then
                            '    ScreenWorkingProcess = False
                            '    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED) = 0 'Once updated UI clear sensor

                            'End If

                            'ANSISE received SGM 12/06/2012
                            sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED) = 0 'Once updated UI clear sensor
                            End If

                            'ISE procedure finished
                            sensorValue = MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                                ISEProcedureFinished = True ' XBC 27/09/2012 
                            End If


                        End If

                    End If


                    ' XBC 26/06/2012 - ISE final self-maintenance 

                    ' XBC 04/09/2012 - Correction : MDI send can't send next instruction until the last operation has been whole completed
                    'If Not MDIAnalyzerManager.ISE_Manager Is Nothing AndAlso MDIAnalyzerManager.ISE_Manager.CurrentProcedureIsFinished Then
                    'JBL 06/09/2012 - Correction: Undo XBC correction: Now is not needed
                    If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then

                        ' XBC 27/09/2012 - Correction : This code just must be executed when the whole ISE procedure has finished
                        If ISEProcedureFinished Then
                            ISEProcedureFinished = False

                            If ShutDownisPending Then
                                ShutDownisPending = False
                                EnableButtonAndMenus(True)
                                SetEnableMainTab(True)
                                If MDIAnalyzerManager.ISE_Manager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                    ' Continues with Shut Down
                                    Me.NotDisplayShutDownConfirmMsg = True
                                    Me.bsTSShutdownButton.Enabled = True

                                    Me.bsTSShutdownButton.PerformClick()
                                Else
                                    'ActivateActionButtonBarOrSendNewAction()    ' XBC 10/12/2012 - Correction : activate again Shutdown button
                                    SetActionButtonsEnableProperty(True) 'AG 31/10/2013 - use this method instead of ActivateActionButtonBarOrSendNewAction (it has previous conditions to buttons activation)
                                    ShowMessage("Error", GlobalEnumerates.Messages.ISE_CLEAN_WRONG.ToString)
                                    Cursor = Cursors.Default
                                End If
                            ElseIf Me.StartSessionisPending Then
                                StopMarqueeProgressBar()
                                EnableButtonAndMenus(True)
                                SetEnableMainTab(True)
                                If MDIAnalyzerManager.ISE_Manager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                    ' Continues with Work Session
                                    Me.StartSession(True)

                                    ' XB 16/12/2013 - Manage Exceptions of ISE module - Task #1441
                                    'ElseIf Me.StartSessionisInitialPUGsent AndAlso _
                                    '       (Not Me.StartSessionisCALBsent And _
                                    '        Not Me.StartSessionisPMCLsent And _
                                    '        Not Me.StartSessionisBMCLsent) Then
                                ElseIf Me.StartSessionisInitialPUGsent AndAlso _
                                       (Not Me.StartSessionisCALBsent And Not Me.StartSessionisPMCLsent And Not Me.StartSessionisBMCLsent) AndAlso _
                                       MDIAnalyzerManager.ISE_Manager.LastProcedureResult <> ISEManager.ISEProcedureResult.Exception Then
                                    ' XB 16/12/2013

                                    ' Errors into initial Purges are ommited
                                    ' Continues with Work Session
                                    Me.StartSession(True)

                                    ' XB 16/12/2013 - Manage Exceptions of ISE module - Task #1441
                                    'ElseIf Me.StartSessionisCALBsent Or Me.StartSessionisPMCLsent Or Me.StartSessionisBMCLsent Then
                                ElseIf (Me.StartSessionisCALBsent Or Me.StartSessionisPMCLsent Or Me.StartSessionisBMCLsent) OrElse _
                                       (MDIAnalyzerManager.ISE_Manager.LastProcedureResult = ISEManager.ISEProcedureResult.Exception) Then
                                    ' XB 16/12/2013

                                    If MDIAnalyzerManager.ISE_Manager.LastProcedureResult = ISEManager.ISEProcedureResult.NOK Then
                                        ' Results Error
                                        If Me.StartSessionisCALBsent Then
                                            Me.SkipCALB = True
                                        ElseIf Me.StartSessionisPMCLsent Then
                                            Me.SkipPMCL = True
                                        ElseIf Me.StartSessionisBMCLsent Then
                                            Me.SkipBMCL = True
                                        End If

                                        Me.StartSession(True)
                                    Else
                                        ' ERC Error
                                        StartingSession = False     ' XB 31/10/2013
                                        Me.StartSessionisPending = False

                                        If StartSessionisCALBsent Then
                                            StartSessionisCALBsent = False
                                        ElseIf StartSessionisPMCLsent Then
                                            StartSessionisPMCLsent = False
                                        ElseIf StartSessionisBMCLsent Then
                                            StartSessionisBMCLsent = False
                                        End If

                                        Dim myGlobal As New GlobalDataTO
                                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                                        executeSTD = False
                                        LockISE = False
                                        myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then

                                            ' XB 23/07/2013 - Activate buzzer
                                            If Not MDIAnalyzerManager Is Nothing Then
                                                MDIAnalyzerManager.StartAnalyzerRinging()
                                                System.Threading.Thread.Sleep(500)
                                            End If
                                            ' XB 23/07/2013

                                            If (CType(myGlobal.SetDatos, Boolean)) Then
                                                'ISE Calibrations are wrong so Module is not ready to be used but there are STD Tests pending of execution; a question Message is shown, and the User 
                                                'can choose between stop the WS Start/Continue process to solve the problem or to continue with the execution despite of the problem
                                                Dim userAnswer As DialogResult

                                                ' XB 20/11/2013 - No ISE warnings can appear when ISE module is not installed
                                                'userAnswer = ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                                If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                                                    userAnswer = ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                                Else
                                                    userAnswer = Windows.Forms.DialogResult.Yes
                                                End If
                                                ' XB 20/11/2013

                                                Application.DoEvents()
                                                If (userAnswer = DialogResult.Yes) Then
                                                    'User has selected continue WS without ISE Tests; all pending ISE Tests will be blocked
                                                    executeSTD = True
                                                    LockISE = True
                                                    EnableButtonAndMenus(False)
                                                    Me.SkipPMCL = True ' XB 22/10/2013
                                                Else
                                                    EnableButtonAndMenus(True, True)   ' XB 07/11/2013
                                                    Application.DoEvents()
                                                End If

                                            Else
                                                'ISE Calibrations are wrong so Module is not ready and there are not STD Tests pending of execution; an error Message is shown due to 
                                                'the WS can not be started
                                                ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ONLY_ISE_WS_NOT_STARTED.ToString)
                                                Application.DoEvents()
                                                'All ISE Pending Tests will be blocked
                                                LockISE = True
                                            End If

                                            ' XB 23/07/2013 - Close buzzer
                                            If Not MDIAnalyzerManager Is Nothing Then
                                                MDIAnalyzerManager.StopAnalyzerRinging()
                                            End If
                                            ' XB 22/07/2013

                                        End If

                                        If (LockISE) Then
                                            If (Me.ActiveMdiChild Is Nothing) OrElse (Not TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                                Dim myExecutionsDelegate As New ExecutionsDelegate
                                                myGlobal = myExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                                            End If

                                            If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                                                'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                                                Dim myDummyUIRefresh As New UIRefreshDS
                                                IMonitor.UpdateWSState(myDummyUIRefresh)
                                            End If
                                        End If

                                        If executeSTD Then
                                            ' Continues with Work Session
                                            Me.StartSession(True)
                                        Else
                                            ShowStatus(Messages.STANDBY)
                                            Cursor = Cursors.Default
                                            ' XBC 29/08/2012 - Enable Monitor Panel
                                            'If (Not ActiveMdiChild Is Nothing) Then
                                            '    ActiveMdiChild.Enabled = True
                                            'End If
                                            ' XB 22/07/2013 - Auto WS Process
                                            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                            InitializeAutoWSFlags()
                                            'ActivateActionButtonBarOrSendNewAction()
                                            SetActionButtonsEnableProperty(True) 'AG 31/10/2013 - use this method instead of ActivateActionButtonBarOrSendNewAction (it has previous conditions to buttons activation)
                                        End If

                                    End If

                                End If
                            End If

                        End If ' XBC 27/09/2012 

                    End If
                    ' XBC 26/06/2012


                End If
                'end SGM 12/03/2012

                ' XBC 15/06/2012
                If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSFCP_RECEIVED Then
                    ' XB 30/08/2013
                    myLogAcciones.CreateLogActivity("(Analyzer Change) ANSFCP received (get it !) ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                    If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.FWCPUVALUE_CHANGED) Then
                        ' XB 30/08/2013
                        myLogAcciones.CreateLogActivity("(Analyzer Change) ANSFCP received (manage it !) ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                        Dim myLogAccionesAux As New ApplicationLogManager()
                        myLogAccionesAux.CreateLogActivity("(Analyzer Change) calling function ManageAnalyzerConnected ... ", Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                        ManageAnalyzerConnected(True, False)
                        MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.NONE

                        '' XB 17/09/2013 - Additional protection - commented by now
                        'MDIAnalyzerManager.WaitingGUI = False

                    End If
                End If
                ' XBC 15/06/2012

                ' XBC 02/08/2012
                If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSPSN_RECEIVED Then
                    MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.NONE

                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True) 'Send SOUND OFF instruction

                    If Not myGlobal.HasError Then

                        Select Case MDIAnalyzerManager.DifferentWSType
                            Case "1"
                                'the WS can not be started
                                ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.WS_NO_MATCH1.ToString)
                            Case "2"
                                'the WS can not be started
                                ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.WS_NO_MATCH2.ToString)
                        End Select

                        If MDIAnalyzerManager.ForceAbortSession Then
                            NotDisplayAbortMsg = True
                            bsTSAbortSessionButton.PerformClick()
                            NotDisplayAbortMsg = False
                            'MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS"
                            'MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""
                            'MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                            'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True)
                        End If
                    End If

                End If
                ' XBC 02/08/2012

                'AG 02/04/2013 - Upload results to LIS if any
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED) Then
                    Dim onlineExportResults As New ExecutionsDS
                    'Get the last results automatically exported (from AnalyzerManager object)
                    onlineExportResults = MDIAnalyzerManager.LastExportedResults

                    'Add into local structure with pending results to be uploaded and upload them (assynchronous call to a synchronous process)
                    If onlineExportResults.twksWSExecutions.Rows.Count > 0 Then
                        AddResultsIntoQueueToUpload(onlineExportResults)

                        CreateLogActivity("Current results automatic upload (STD, ISE, CALC)", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)

                        InvokeUploadResultsLIS(False)

                        'Clear the last results automatically exported (in AnalyzerManager object)
                        MDIAnalyzerManager.ClearLastExportedResults()
                    End If
                End If
                'AG 02/04/2013

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 3.", Me.Name & ".ManageReceptionEvent ", _
                '                                 EventLogEntryType.FailureAudit, False)
            End If

            'AG 17/07/2013 - Commented and moved until StartEnterInRunningMode to avoid create 2 threads calling EnterRunning method
            ''AG 08/07/2013 - Special mode for work with LIS with automatic actions
            'If autoWSCreationWithLISModeAttribute AndAlso (automateProcessCurrentState = LISautomateProcessSteps.subProcessDownloadOrders OrElse _
            '                                               automateProcessCurrentState = LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning OrElse automateProcessCurrentState = LISautomateProcessSteps.ExitNoWorkOrders) Then
            '    CreateAutomaticWSWithLIS()
            'End If
            ''AG 08/07/2013


            'Debug.Print("IAx00MainMDI.ManageReceptionEvent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 12/06/2012 - time estimation

            ' XB 30/08/2013 protection : Ensure that connected Analyzer is registered on DB
            Dim myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions
            myAx00Action = MDIAnalyzerManager.AnalyzerCurrentAction
            'If myAx00Action = GlobalEnumerates.AnalyzerManagerAx00Actions.SOUND_DONE Then  ' XB 03/12/2013 - Do not works when Analyzer on StandBy mode
            If CheckAnalyzerIDOnFirstConnection Then                                        ' XB 03/12/2013 - Works fine !- task #1410
                If MDIAnalyzerManager.Connected AndAlso MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                        'Get the connected Analyzer
                        Dim myAnalyzerDelegate As New AnalyzersDelegate
                        Dim myAnalyzerData As New AnalyzersDS
                        Dim myGlobal As New GlobalDataTO
                        myGlobal = myAnalyzerDelegate.GetAnalyzer(Nothing)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myAnalyzerData = DirectCast(myGlobal.SetDatos, AnalyzersDS)
                            If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                                Dim myTmpAnalyzerConnected As String = ""
                                myTmpAnalyzerConnected = MDIAnalyzerManager.TemporalAnalyzerConnected
                                If myAnalyzerData.tcfgAnalyzers(0).Generic OrElse myAnalyzerData.tcfgAnalyzers(0).AnalyzerID <> myTmpAnalyzerConnected Then
                                    myLogAcciones.CreateLogActivity("(Analyzer Change) Activate protection to register the Connected Analyzer... ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)
                                    ManageAnalyzerConnected(True, False)
                                    CheckAnalyzerIDOnFirstConnection = False
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            '//JVV 20/09/2013 Si el evento de Auto_Report está activo, se tratará según este if
            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.AUTO_REPORT) Then
                'Dim sOrderIDs As String = String.Empty
                'With copyRefreshDS
                '    For Each dr As DataRow In .AutoReport.Rows
                '        sOrderIDs &= dr("OrderID").ToString & " / "
                '    Next
                'End With
                'MessageBox.Show("AutoReport per: " & sOrderIDs)

                '//1- marcar las ordenes como impresas una vez lo estén (campo Printed=1) -> twksResults ??
                '//2- generar alarmas si ha habido un error en la impresión (impresora no disponible, etc.) ??
                Dim orderList As List(Of String)
                orderList = Enumerable.Cast(Of String)(From l In copyRefreshDS.AutoReport.AsEnumerable() Select l("OrderID")).Distinct().ToList()

                Dim myGlobalDataTO As New GlobalDataTO
                Dim myReportFrecuency As String = String.Empty, myReportType As String = String.Empty
                Dim myUserSettingDelegate As New UserSettingsDelegate()

                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_FREQ.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myReportFrecuency = myGlobalDataTO.SetDatos.ToString()
                End If
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_TYPE.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myReportType = myGlobalDataTO.SetDatos.ToString()
                End If

                '//Impresión del Report Automático en función de la frecuencia y tipo de informe escogidos por el usuario.
                If (myReportFrecuency <> String.Empty) Then
                    Select Case (myReportFrecuency)
                        Case "END_WS", "ORDER"
                            If (String.Equals(myReportType, "COMPACT")) Then
                                XRManager.PrintCompactPatientsReport(copyRefreshDS.AutoReport.Rows(0)("AnalyzerID").ToString, copyRefreshDS.AutoReport.Rows(0)("WorkSessionID").ToString, orderList, True)
                            ElseIf (String.Equals(myReportType, "INDIVIDUAL")) Then
                                XRManager.PrintPatientsFinalReport(copyRefreshDS.AutoReport.Rows(0)("AnalyzerID").ToString, copyRefreshDS.AutoReport.Rows(0)("WorkSessionID").ToString, orderList)
                            End If
                        Case Else
                            'Opción Reset: el informe se lanzará al pulsar el botón de Reset. No hacemos nada aquí.
                    End Select
                End If
            End If
            '//JVV 20/09/2013

            'AG 04/12/2013 - BT #1397 - Show the message here, not when the sensor changes because it could change when the analyzer
            'is scanning and the incomplete samples screen is shown (this screen is TopMost)
            If recoveryResultsWarnFlag AndAlso Not incompleteSamplesOpenFlag Then
                recoveryResultsWarnFlag = False 'Set to false in order to show it only 1 time

                'Pause running ... show message to user and he has to choose between EXIT pause or ABORT
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim recoverInPauseMessage As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_RES_RECOVER_INPAUSE", CurrentLanguageAttribute)

                'Dim exitPauseText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Exit", CurrentLanguageAttribute) 'Exit
                Dim exitPauseText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RecoverResults", CurrentLanguageAttribute) 'Recover results
                Dim userAnswer As DialogResult = DialogResult.No
                userAnswer = BSCustomMessageBox.Show(Me, recoverInPauseMessage, My.Application.Info.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, "", exitPauseText, abortButtonText)
                Dim resultData As New GlobalDataTO
                If userAnswer = Windows.Forms.DialogResult.Yes Then
                    'Exit pause
                    resultData = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True, Nothing, True) 'Last parameter means: START + END

                Else 'No
                    'Abort
                    AbortSession(resultData)
                End If

                'Once the user has answered open the auxiliary screen for recovery results
                OpenMDIChildForm(IResultsRecover)
                EnableButtonAndMenus(False)
            End If
            'AG 04/12/2013

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message & ". SOURCE --> " & ex.Source, "ManageReceptionEvent", _
                                            EventLogEntryType.Error, False)

            myLogAcciones.CreateLogActivity("Instruction Recived --> " & pInstructionReceived, "ManageReceptionEvent", _
                                            EventLogEntryType.Error, False)
        End Try

        Return True
    End Function


    'XBC 15/06/2012
    Private Sub ManageAnalyzerConnected(ByVal pCheckFwSwCompatible As Boolean, ByVal pisReportSATLoading As Boolean)
        Dim myGlobal As New GlobalDataTO

        Try
            Dim myLogAccionesAux As New ApplicationLogManager()
            myLogAccionesAux.CreateLogActivity("(Analyzer Change) Manage Analyzer Connection... ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)

            If (pisReportSATLoading) Then Exit Sub

            Dim RefreshScreen As Boolean = False
            Dim BlockCommunications As Boolean = False

            If (pCheckFwSwCompatible AndAlso Not MDIAnalyzerManager.IsFwSwCompatible) Then
                NotDisplayErrorCommsMsg = True

                'Obtain needed fw version
                Dim mySwVersion As String
                Dim myUtil As New Utilities

                myGlobal = myUtil.GetSoftwareVersion()
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    mySwVersion = myGlobal.SetDatos.ToString

                    myGlobal = MyClass.MDIAnalyzerManager.GetFwVersionNeeded(mySwVersion)
                    If (Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing) Then
                        Dim myNeededFwVersion As String = CStr(myGlobal.SetDatos)

                        Dim TextPar As New List(Of String)
                        TextPar.Add(myNeededFwVersion)

                        Dim myMsgList As New List(Of String)
                        myMsgList.Add(Messages.FW_VERSION_NOT_VALID2.ToString)
                        ShowMultipleMessage("Warning", myMsgList, Nothing, TextPar)
                    End If
                End If
                BlockCommunications = True
            Else
                'Get the worksession status in case exists
                If (ActiveWorkSession.Length > 0) Then
                    If (MDIAnalyzerManager.IsAnalyzerIDNotLikeWS) Then
                        RefreshScreen = True
                        MDIAnalyzerManager.IsAnalyzerIDNotLikeWS = False

                        Dim myResult As DialogResult
                        Dim myMessageIDList As New List(Of String)
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_1.ToString)
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_2.ToString)
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_3.ToString)
                        myResult = ShowMultipleMessage("Warning", myMessageIDList)

                        If (myResult = Windows.Forms.DialogResult.Cancel) Then
                            NotDisplayErrorCommsMsg = True
                            BlockCommunications = True
                        End If
                    End If
                End If
            End If

            If (BlockCommunications) Then
                'Cut off communications channel
                If (MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY) Then
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                End If

                If (Not myGlobal.HasError) Then
                    If (MDIAnalyzerManager.StopComm()) Then
                        DisconnectComms()
                    End If
                End If

                'XBC 06/09/2012 - Refresh status as Disconnected
                ShowStatus(Messages.NO_CONNECTED)
                Exit Try
            End If

            myLogAccionesAux.CreateLogActivity("(Analyzer Change) ActiveWorkSession [ " & ActiveWorkSession & " ] ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)

            If (ActiveWorkSession.Length > 0) Then
                ' Execute process to update the corresponding Work Session tables 
                MDIAnalyzerManager.ForceAbortSession = False
                myLogAccionesAux.CreateLogActivity("(Analyzer Change) Calling function ProcessUpdateWSByAnalyzerID ... ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)
                myGlobal = MDIAnalyzerManager.ProcessUpdateWSByAnalyzerID(Nothing)
                If (Not myGlobal.HasError) Then
                    'If (MDIAnalyzerManager.ForceAbortSession) Then
                    ' XBC 21/06/2012 - DISCARDED !!!
                    'bsTSAbortSessionButton.Enabled = True
                    'NotDisplayAbortMsg = True
                    'bsTSAbortSessionButton.PerformClick()
                    'NotDisplayAbortMsg = False
                    'bsTSAbortSessionButton.Enabled = False
                    'End If

                    AnalyzerIDAttribute = MDIAnalyzerManager.ActiveAnalyzer
                    AnalyzerModelAttribute = MDIAnalyzerManager.GetModelValue(AnalyzerIDAttribute)
                    WorkSessionIDAttribute = MDIAnalyzerManager.ActiveWorkSession
                    WSStatusAttribute = MDIAnalyzerManager.WorkSessionStatus

                    If (RefreshScreen) Then
                        CloseActiveMdiChild()

                        'Open the Monitor Screen an update status of the Vertical Buttons Bar
                        OpenMonitorForm(Nothing)

                    ElseIf (Not ActiveMdiChild Is Nothing) Then
                        'If monitor is the current screen inform analyzer and model
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)

                            CurrentMdiChild.ActiveAnalyzer = AnalyzerIDAttribute
                            CurrentMdiChild.AnalyzerModel = AnalyzerModelAttribute
                            CurrentMdiChild.ActiveWorkSession = WorkSessionIDAttribute
                            CurrentMdiChild.CurrentWorkSessionStatus = WSStatusAttribute
                        End If
                    End If

                Else
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Analyzer ID checking", "ManageAnalyzerConnected", EventLogEntryType.Error, False)
                    ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                End If
            End If
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "ManageAnalyzerConnected", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Disconnected Communications
    ''' </summary>
    ''' <remarks>Created by XBC 18/06/2012</remarks>
    Private Sub DisconnectComms()
        Try
            ForceRefreshOnFirstStandBy = True               ' XB 06/11/2013
            CheckAnalyzerIDOnFirstConnection = True         ' XB 03/12/2013
            CheckAnalyzerIDOnFirstConnectionForISE = True   ' XB 03/12/2013

            Dim myGlobal As New GlobalDataTO
            'Inform the analyzer manager
            myGlobal = MDIAnalyzerManager.ProcessUSBCableDisconnection(True)
            SetActionButtonsEnableProperty(False)
            If Not myGlobal.HasError Then
                ''does not call the method ManageAlarms
                'MDIAnalyzerManager.Alarms.Add(GlobalEnumerates.Alarms.COMMS_ERR)
                'Dim myWSAnalyzerAlarmDS As New WSAnalyzerAlarmsDS
                'myWSAnalyzerAlarmDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow( _
                '        GlobalEnumerates.Alarms.COMMS_ERR.ToString(), AnalyzerIDAttribute, _
                '        DateTime.Now, 1, WorkSessionIDAttribute, Nothing, True, Nothing)

                'Dim myWSAlarmDelegate As New WSAnalyzerAlarmsDelegate
                'myGlobal = myWSAlarmDelegate.Create(Nothing, myWSAnalyzerAlarmDS) 'AG 24/07/2012 - keep using Create, do not use Save, it is not necessary


                If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
                   MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
                   MDIAnalyzerManager.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
                    Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                    myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                    MDIAnalyzerManager.ISE_Manager.LastISEResult = myISEResultWithComErrors

                    If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                        Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        CurrentMdiChild.PrepareErrorMode()
                    End If

                End If

            Else
                ShowMessage("Error", myGlobal.ErrorMessage)
            End If
            SetActionButtonsEnableProperty(True)
            ControlBox = True

            If Not ActiveMdiChild Is Nothing Then
                '- Configuration screen (close screen without save changes)
                ' The 1st idea was update the ports combo but it was CANCELED due a system error was triggered and it was difficult to solve
                Dim myRefreshDS As UIRefreshDS = Nothing
                Dim myRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)
                If (TypeOf ActiveMdiChild Is IConfigGeneral) Then
                    Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                    CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS)

                ElseIf (TypeOf ActiveMdiChild Is IMonitor) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED)
                    CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".DisconnectComms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisconnectComms ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub
    ' XBC 15/06/2012


    Private Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles MDIAnalyzerManager.SendEvent

        Me.UIThread(Function() ManageSentEvent(pInstructionSent))

    End Sub


    ''' <summary>
    ''' Special Event sent from Communications Layer
    ''' </summary>
    ''' <param name="pInstructionSent"></param>
    ''' <remarks>
    ''' Created by XB-SG 11/05/2012
    ''' </remarks>
    ''' 
    Private Function ManageSentEvent(ByVal pInstructionSent As String) As Boolean
        Try
            If String.Compare(pInstructionSent, AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString, False) = 0 Then

                'SGM 11/05/2012
                If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
               MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
               MDIAnalyzerManager.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
                    Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                    myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                    MDIAnalyzerManager.ISE_Manager.LastISEResult = myISEResultWithComErrors
                End If

                If Not ActiveMdiChild Is Nothing Then

                    If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                        Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        CurrentMdiChild.PrepareErrorMode()
                    End If

                End If
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "ManageSentEvent", EventLogEntryType.Error, False)
        End Try
        Return True
    End Function


    ''' <summary>
    ''' Event incharge to show information if Ax00 Is disconected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>CREATE BY: 'TR 20/10/2011
    ''' AG 28/11/2013 - BT #1397</remarks>
    Private Sub OnDeviceRemoved(ByVal sender As Object, ByVal e As Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetectorEventArgs)
        Try
            ForceRefreshOnFirstStandBy = True               ' XB 06/11/2013
            CheckAnalyzerIDOnFirstConnection = True         ' XB 03/12/2013
            CheckAnalyzerIDOnFirstConnectionForISE = True   ' XB 03/12/2013

            'Get the open port.
            Dim myConnectedPort As String = ""
            Dim isConnected As Boolean = False
            If Not MDIAnalyzerManager Is Nothing Then
                isConnected = MDIAnalyzerManager.Connected
                myConnectedPort = MDIAnalyzerManager.PortName
                'Remove special character in case Port Number is Greather than 9 
                myConnectedPort = myConnectedPort.Replace("\\.\", String.Empty)
            End If

            If isConnected AndAlso Not String.Compare(myConnectedPort, String.Empty, False) = 0 Then
                Dim MyPortList As New List(Of String)
                MyPortList = IO.Ports.SerialPort.GetPortNames().ToList
                'Search on myPortList if the current port is on the list.
                If MyPortList.Where(Function(a) String.Compare(a, myConnectedPort, False) = 0).Count = 0 Then

                    'TR 10/11/2011 -Create a log entry to inform the desconnection.
                    CreateLogActivity(GetMessageText(e.Message, CurrentLanguageAttribute), Name & ".OnDeviceRemoved", EventLogEntryType.Error, False)

                    'XB + AG 09/09/2013 - error comms while automatic WS creation process is reading barcode
                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.subProcessReadBarcode Then
                        'processingBeforeRunning = "2" 'Aborted
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                        InitializeAutoWSFlags()
                        StopMarqueeProgressBar()
                    End If
                    'XB + AG 09/09/2013

                    'AG 24/10/2011
                    'Inform the analyzer manager
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MDIAnalyzerManager.ProcessUSBCableDisconnection()
                    SetActionButtonsEnableProperty(False)
                    If Not myGlobal.HasError Then
                        recoveryResultsWarnFlag = False 'AG 28/11/2013 - BT #1397

                        'AG 13/02/2012 - Insert a COMMS_ERR alarm in DB (this is required because the ProcessUSBCableDisconnection
                        'does not call the method ManageAlarms
                        'MDIAnalyzerManager.Alarms.Clear() 'AG 21/05/2012 - if not communications do not remove analyzermanager alarms, only do not show them
                        MDIAnalyzerManager.Alarms.Add(GlobalEnumerates.Alarms.COMMS_ERR)
                        Dim myWSAnalyzerAlarmDS As New WSAnalyzerAlarmsDS
                        myWSAnalyzerAlarmDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow( _
                                GlobalEnumerates.Alarms.COMMS_ERR.ToString(), AnalyzerIDAttribute, _
                                DateTime.Now, 1, WorkSessionIDAttribute, Nothing, True, Nothing)

                        Dim myWSAlarmDelegate As New WSAnalyzerAlarmsDelegate
                        myGlobal = myWSAlarmDelegate.Create(Nothing, myWSAnalyzerAlarmDS) 'AG 24/07/2012 - keep using Create, do not use Save, it is not necessary
                        'AG 13/02/2012

                        ShowMessage("Warning", GlobalEnumerates.Messages.ERROR_COMM.ToString)
                        Debug.Print("ERROR-COMM IN Ax00MainMDI.OnDeviceRemoved")

                        'SGM 11/05/2012
                        If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
                           MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
                           MDIAnalyzerManager.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then

                            Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                            myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                            MDIAnalyzerManager.ISE_Manager.LastISEResult = myISEResultWithComErrors

                            If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                CurrentMdiChild.PrepareErrorMode()
                            End If

                        End If
                        'SGM 11/05/2012

                    Else
                        ShowMessage("Error", myGlobal.ErrorMessage)
                    End If
                    SetActionButtonsEnableProperty(True)

                    If Not ActiveMdiChild Is Nothing Then
                        '- Configuration screen (close screen without save changes)
                        ' The 1st idea was update the ports combo but it was CANCELED due a system error was triggered and it was difficult to solve
                        Dim myRefreshDS As UIRefreshDS = Nothing
                        Dim myRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)
                        If (TypeOf ActiveMdiChild Is IConfigGeneral) Then
                            Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                            CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)

                            'AG 13/02/2012 - Refresh monitor (new alarm)
                        ElseIf (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED)
                            'TR 25/09/2012 -Add the sensor value change to update the pc no connected Led.
                            myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED)
                            CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            'AG 13/02/2012

                            'DL 26/09/2012
                            'ElseIf (TypeOf ActiveMdiChild Is IChangeRotor) Then
                            'SetActionButtonsEnableProperty(False)
                            '   EnableButtonAndMenus(False)

                            'DL 26/09/2012
                        End If
                    End If
                    'AG 24/10/2011

                End If
            End If

            ' XBC 04/09/2012 - Cut off communications channel
            'MDIAnalyzerManager.StopComm()
            ' ALBERT !!!!!!! HAURIA DE SER AIXO PERO NO FUNCIONA !!!!!!!!!!!!!!!!!!!!!

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OnDeviceRemoved ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OnDeviceRemoved ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


    ''' <summary>
    ''' Activation and deactivation event to be called from outher layer
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' Created by XBC 08/02/2012
    ''' Modified by XBC 11/07/2012 - add protection while Wup is processing
    ''' </remarks>
    Public Sub OnManageActivateScreenEvent(ByVal pEnable As Boolean, ByVal pMessageID As GlobalEnumerates.Messages) Handles myISEUtilities.ActivateScreenEvent
        Try
            If String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") Then
                Exit Sub  ' XBC 11/07/2012
            End If

            Me.EnableButtonAndMenus(pEnable)
            Me.ControlBox = pEnable

            If Not pEnable Then
                Me.InitializeMarqueeProgreesBar()
            Else
                Me.StopMarqueeProgressBar()
            End If
            Application.DoEvents()

            Me.ShowStatus(pMessageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".OnManageActivateScreenEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OnManageActivateScreenEvent ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>Created by SGM 14/05/2012</remarks>
    Public Sub OnActivateVerticalButtonsEvent(ByVal pEnable As Boolean) Handles myISEUtilities.ActivateVerticalButtonsEvent
        Try
            MyClass.SetActionButtonsEnableProperty(pEnable)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".OnActivateVerticalButtonsEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OnActivateVerticalButtonsEvent ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Common Forms" 'SG 03/12/10

    Private WithEvents myLanguageConfig As IConfigLanguage
    Private WithEvents myConfigAnalyzers As IConfigGeneral
    Private WithEvents myConfigUsers As IConfigUsers
    Private WithEvents myConfigBarCode As IBarCodesConfig   ' DL 22/07/2011
    Private WithEvents mySortingTest As ISortingTestsAux
    Private WithEvents myLogin As IAx00Login
    Private WithEvents myReport As IBandTemplateReport 'RH 15/12/2011
    Private WithEvents mySettings As ISettings    ' XBC 18/01/2012
    'Private WithEvents myISEUtilities As Biosystems.Ax00.PresentationCOM.IISEUtilities ' XBC 08/02/2012 ISE Utilities placed into PresetationCOM layer
    Private WithEvents myISEUtilities As IISEUtilities 'RH 26/03/2012
    Private WithEvents myAbout As IAboutBox 'RH 28/03/2012
    Private WithEvents myInstrumentInfo As IInstrumentInfo

#End Region

#Region "Events "

    ''' <summary>
    ''' Timer
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 17/04/2013
    ''' </remarks>
    Private Sub watchDogTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)

        'Disable timer
        watchDogTimer.Enabled = False

        'Finish loop
        If watchDogCase = CStr(MDIWatchDogCases.ReadBarcodeBeforeRunning) Then
            processingBeforeRunning = "2"
            UIThread(Sub() SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitBarcodeErrorAndStop)) 'AG 22/07/2013
        End If

        watchDogCase = ""
        ScreenWorkingProcess = False

        'AG 19/11/2013 - code add during developping tests (because the user interface remains disabled)
        UIThread(Sub() EnableButtonAndMenus(True, True))

    End Sub


    Private Sub WithShutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WithShutToolStripMenuItem.Click

        'If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then  'dl 04/06/2012
        'flagExitWithShutDown = False                    'dl 04/06/2012
        'Me.Close()
        'ApplicationClosing(Nothing, Nothing)            'dl 04/06/2012

        'Else                                                'dl 04/06/2012
        flagExitWithShutDown = True                     'dl 04/06/2012
        bsTSShutdownButton_Click(Nothing, Nothing)      'dl 04/06/2012
        'End If

    End Sub



    Private Sub IAx00MainMDI_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyData = Keys.F1 Then
            SetHelpProvider()
        End If
    End Sub

    ''' <summary>
    ''' Start auto connection when application (MDI) is shown
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' </remarks>
    Private Sub BsAutoConnect_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BsAutoConnect.DoWork
        Try
            AutoConnectProcess = True
            Connect()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".BsAutoConnect_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsAutoConnect_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' End of auto connection (MDI shown) process
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' </remarks>
    Private Sub BsAutoConnect_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BsAutoConnect.RunWorkerCompleted
        Try
            AutoConnectProcess = False
            If (Not MDIAnalyzerManager Is Nothing) Then
                If (Not MDIAnalyzerManager.Connected) Then
                    SetActionButtonsEnableProperty(True)
                    EnableButtonAndMenus(True)  'SA 07/09/2012
                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".BsAutoConnect_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsAutoConnect_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, MsgParent)
        Finally
            'SA 07/09/2012
            'If MDIAnalyzerManager.Connected Then EnableButtonAndMenus(True)
            Cursor = Cursors.Default
            'processingConnect = False  'SA 07/09/2012
        End Try
    End Sub

    ''' <summary>
    ''' Loads default Report Templates and initializes Alert Globes when application (MDI) is shown
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 17/02/2012
    ''' </remarks>
    Private Sub BsLoadDefaultReportTemplates_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BsLoadDefaultReportTemplates.DoWork
        Try
            LoadingGlobesField = True
            InitializeAlertGlobes()
            LoadingGlobesField = False

            'AG 16/05/2012
            ''AG 14/02/2012 - when app is loading evaluate if the change reactions rotor alarm is required, when
            ''required add the globe "Change Reactions Rotor is recommend" (only the globe, not the alarm)
            'Dim myGlobal As GlobalDataTO
            'If Not MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
            '    Dim AnalReactionsRotor As New AnalyzerReactionsRotorDelegate
            '    myGlobal = AnalReactionsRotor.ChangeReactionsRotorRecommended(Nothing, ActiveAnalyzer, IAx00MainMDI.ActiveAnalyzerModel)
            '    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
            '        If CType(myGlobal.SetDatos, String) <> "" Then
            '            'Add alarm globe (change reactions rotor is recommend)
            '            If Not MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN) Then MDIAnalyzerManager.Alarms.Add(GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN)
            '        End If
            '    End If
            'End If
            ''AG 14/02/2012
            '
            '' XBC 26/03/2012
            '' Check ISE Alarms
            'If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
            '    Dim myAlarmList As New List(Of GlobalEnumerates.Alarms)
            '    Dim myAlarmStatusList As New List(Of Boolean)

            '    myGlobal = MDIAnalyzerManager.ISE_Manager.CheckAlarms(MDIAnalyzerManager.Connected, myAlarmList, myAlarmStatusList)
            '    If Not myGlobal.HasError Then
            '        'Add alarms globe
            '        If myAlarmList.Count > 0 Then
            '            For i As Integer = 0 To myAlarmList.Count - 1
            '                If Not MDIAnalyzerManager.Alarms.Contains(myAlarmList(i)) Then MDIAnalyzerManager.Alarms.Add(myAlarmList(i))
            '            Next
            '        End If
            '    End If
            'End If
            '' XBC 26/03/2012
            'AG 16/05/2012

            'Load Reports Default Portrait Template
            If Not XRManager.LoadDefaultPortraitTemplate() Then
                'No Reports template has been loaded.
                'Take the proper action!
            End If

            'Load Reports Default Landscape Template
            If Not XRManager.LoadDefaultLandscapeTemplate() Then
                'No Reports template has been loaded.
                'Take the proper action!
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".BsLoadDefaultReportTemplates_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsLoadDefaultReportTemplates_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, MsgParent)

        End Try
    End Sub



    ''' <summary>
    ''' Hide menus that on ly BIOSYSTEMS USER can See.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: RH 16/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub HideBIOSYSTEMSOnlyMenuOptions()
        Dim UserName As String = New GlobalBase().GetSessionInfo().UserName.ToUpperBS()   ' ToUpper()
        Dim NewValue As Boolean = String.Equals(UserName, "BIOSYSTEMS")

        BorrameToolStripMenuItem.Visible = NewValue
        ChangeSettingsToolStripMenuItem.Visible = NewValue
        QCResultSimulatorMenuItem.Visible = NewValue
        '        BlankCalibratorResultsToolStripMenuItem.Visible = NewValue   dl 31/10/2012
        '        ISEResultsToolStripMenuItem.Visible = NewValue  dl 31/10/2012
        StatisticsToolStripMenuItem.Visible = NewValue
        HistoricalDDBBReviewToolStripMenuItem.Visible = NewValue
        'HistoricalAnalyzerAlarmsReviewToolStripMenuItem.Visible = NewValue  dl 31/10/2012

        'AG 05/10/2012 - When prepare a setup this menu option must remains to FALSE
        'BorrameToolStripMenuItem.Visible = False

        'MainResultsToolStripMenuItem.Visible = NewValue 'TR 20/04/2012 commented to test security for operator user level.
    End Sub


    ''' <summary>
    ''' Enable or disable menu option by the user level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase
    '''                             must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''             TR 05/03/2013 - Add the LIS Mapping Tool menu. - CANCELLED!!! Allowed screen access but not screen changes!!
    ''' </remarks>
    Private Sub MenuOptionsByUserLevel()
        Try
            Dim CurrentUserLevel As String = New GlobalBase().GetSessionInfo().UserLevel    '.ToUpper()
            Dim currentUserName As String = New GlobalBase().GetSessionInfo().UserName

            'Me.Text = "BA400 User Sw" & " - " & CurrentUserLevel 'DL 15/05/2012
            Me.Text = "BA400 User Sw" & " - " & currentUserName 'AG 22/05/2012


            Select Case CurrentUserLevel
                Case "OPERATOR"
                    DeleteSessionToolStripMenuItem.Enabled = False
                    Exit Select

                Case "SUPERVISOR"
                    DeleteSessionToolStripMenuItem.Enabled = True
                    Exit Select

                Case Else
                    DeleteSessionToolStripMenuItem.Enabled = True
                    Exit Select

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".HideMenuOptionsByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HideMenuOptionsByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, MsgParent)
        End Try
    End Sub

    'Private Sub Form_Click()

    '    StartWindow = FindWindow("Shell_TrayWnd", vbNullString)
    '    If MsgBox("¿Ocultar barra?", vbInformation + vbYesNo) = vbYes Then
    '        ShowWindow(StartWindow, 0&)  ' La ocultamos     Else 
    '        ShowWindow(StartWindow, 1&) ' La mostramos     End If 
    'End Sub

    ''' <summary>
    ''' Screen load event
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform GlobalAnalyzerManager properties from the global Attributes
    '''              SA 02/11/2010 - Get the current Language from application Session object and inform
    '''                              the screen property CurrentLanguageAttribute
    '''              SA 23/11/2010 - Inform required Properties before open Monitor Screen
    ''' </remarks>
    Private Sub Ax00MainMDI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'RH
            If MsgParent Is Nothing Then
                MsgParent = Me
            End If

            'RH 25/10/2010
            'Clear ErrorStatusLabel Text
            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None

            bsDateTimer_Tick(Nothing, Nothing) 'RH 27/03/2012

            Application.DoEvents()

            Cursor = Cursors.WaitCursor
            Application.DoEvents()

            watchDogTimer.Enabled = False
            AddHandler watchDogTimer.Elapsed, AddressOf watchDogTimer_Timer


            Dim myGlobalDataTO As GlobalDataTO
            'TR 05/04/2013 -Hide buttons depending on the parameter value LIS_WithFiles_Mode.
            Dim myUserSettingsDelegate As New UserSettingsDelegate
            myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())
            If Not myGlobalDataTO.HasError Then
                'Set the value to my local variable
                LISWithFilesMode = CBool(myGlobalDataTO.SetDatos)

            End If

            ' JC 26/04/2013 - MAX recomeded Pending Orders to Download while Analyzer is RUNNING
            myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.LIS_MAX_ORDER_DOWNLOAD_RUNNING.ToString())
            If Not myGlobalDataTO.HasError Then
                'Set the value to my local variable
                MAX_PENDING_ORDERS = CInt(myGlobalDataTO.SetDatos)
            End If

            'JV + AG revision 18/10/2013 task # 1341
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            imagePause = Image.FromFile(".\Images\Embedded\MDIVertPause.png")
            imagePlay = Image.FromFile(".\Images\Embedded\MDIVertPlay.png")
            tooltipPlay = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Start", CurrentLanguageAttribute)
            tooltipPause = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Pause", CurrentLanguageAttribute)
            showSTARTWSiconFlag = True
            showPAUSEWSiconFlag = False
            ChangeStatusImageMultipleSessionButton()
            'JV + AG revision 18/10/2013 task # 1341
            'JV + AG 28/11/2013 #1391
            infoCritical = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_CRITICAL_TESTS_PAUSEMODE", CurrentLanguageAttribute)
            waitButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WAIT", CurrentLanguageAttribute)
            'JV + AG 28/11/2013 #1391

            PrepareButtons()
            Application.DoEvents()

            InitializeAnalyzerAndWorkSession(True) 'AG 24/11/2010 - Move code into this method
            Application.DoEvents()

            SetActionButtonsEnableProperty(False)
            EnableButtonAndMenus(False)

            HideBIOSYSTEMSOnlyMenuOptions()
            MenuOptionsByUserLevel() 'TR 23/04/2012

            'AG 25/02/2013
            InitiateLISWrapper()
            Application.DoEvents()

            ''SG 04/04/11
            'LoadAdjustmentsDS()
            'Application.DoEvents()

            ''While LoadingGlobes
            ''    System.Threading.Thread.Sleep(1)
            ''    Application.DoEvents()
            ''End While

            ' ''AG 14/02/2012 - when app is loading evaluate if the change reactions rotor alarm is required, when
            ' ''required add the globe "Change Reactions Rotor is recommend" (only the globe, not the alarm)
            ''If Not MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
            ''    Dim AnalReactionsRotor As New AnalyzerReactionsRotorDelegate
            ''    Dim myGlobal As GlobalDataTO
            ''    myGlobal = AnalReactionsRotor.ChangeReactionsRotorRecommended(Nothing, ActiveAnalyzer, IAx00MainMDI.ActiveAnalyzerModel)
            ''    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
            ''        If CType(myGlobal.SetDatos, String) <> "" Then
            ''            'Add alarm globe (change reactions rotor is recommend)
            ''            If Not MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN) Then MDIAnalyzerManager.Alarms.Add(GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN)
            ''        End If
            ''    End If
            ''End If
            ' ''AG 14/02/2012

            ''Application.DoEvents()

            ''RH 01/07/2011 These are the last instructions. Please, keep that order!

            ''RH 16/12/2010
            'OpenMonitorForm()
            'Application.DoEvents()

            ''RH 03/11/2010
            'bsDateTimer_Tick(Nothing, Nothing)
            'bsDateTimer.Enabled = True

            ''RH 01/07/2011 END

            ''RH 20/03/2012
            'SetActionButtonsEnableProperty(False)    'Disable all buttons
            'Cursor = Cursors.Default

            'If MDIAnalyzerManager.classInitializationError Then
            '    'Show message "Communications initialization error"
            '    ShowMessage("Warning", GlobalEnumerates.Messages.COMMS_INITILIATION_ERROR.ToString)
            'Else
            '    'Try AutoConnect
            '    BsAutoConnect.RunWorkerAsync()
            'End If

            'Application.DoEvents()
            ''END RH 20/03/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".Ax00MainMDI_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Cursor = Cursors.Default
            ShowMessage(Name & ".Ax00MainMDI_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Code moved from Load()
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 22/03/2012
    ''' </remarks>
    Private Sub Ax00MainMDI_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            'SG 04/04/11
            LoadAdjustmentsDS()
            Application.DoEvents()

            'RH 01/07/2011 These are the last instructions. Please, keep that order!

            'RH 16/12/2010
            OpenMonitorForm()

            Application.DoEvents()

            'RH 03/11/2010
            bsDateTimer_Tick(Nothing, Nothing)
            bsDateTimer.Enabled = True

            'RH 01/07/2011 END

            'RH 20/03/2012
            SetActionButtonsEnableProperty(False)    'Disable all buttons
            Cursor = Cursors.Default

            If MDIAnalyzerManager.classInitializationError Then
                'Show message "Communications initialization error"
                ShowMessage("Warning", GlobalEnumerates.Messages.COMMS_INITILIATION_ERROR.ToString)
                SetActionButtonsEnableProperty(True)
                EnableButtonAndMenus(True)
            Else
                EnableButtonAndMenus(False) 'AG 07/09/2012
                ShowStatus(Messages.CONNECTING)

                'Try AutoConnect
                BsAutoConnect.RunWorkerAsync()
            End If

            Application.DoEvents()

            'AG 25/02/2013 - LIS manager object create channel
            InvokeCreateLISChannel()
            'AG 25/02/2013

            'SA 07/09/2012
            'processingConnect = True
            'While processingConnect
            '    Me.InitializeMarqueeProgreesBar()
            '    Application.DoEvents()
            'End While
            'Application.DoEvents()
            'StopMarqueeProgressBar()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".Ax00MainMDI_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Ax00MainMDI_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
        If ProcessingLISManagerObject Then ProcessingLISManagerObject = False 'AG 25/02/2013 - MDI load complete. Object is available yet

    End Sub

    '''' <summary>
    '''' Call the Connect process first time MDI is shown (by now it doesn't work)
    '''' </summary>
    '''' <remarks>
    '''' Created by:  AG 28/07/2010
    '''' </remarks>
    'Private Sub Ax00MainMDI_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
    '    Try
    '        'AG 22/09/2011 'AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager //
    '        'If GlobalAnalyzerManager.classInitializationError Then
    '        If MDIAnalyzerManager.classInitializationError Then
    '            'Show message "Communications initialization error"
    '            SetActionButtonsEnableProperty(False)    'Disable all buttons
    '            ShowMessage("Warning", GlobalEnumerates.Messages.COMMS_INITILIATION_ERROR.ToString)

    '        Else
    '            'Try AutoConnect
    '            'Option1: Slow UI refresh
    '            'Me.Connect()

    '            'Option2: Use new thread - Obsolete method
    '            'AutoConnectProcess = New Thread(AddressOf MyClass.Connect)
    '            'AutoConnectProcess.Start()
    '            'AutoConnectProcess.Name = "Connection Process"

    '            'Option3: Use Background worker control
    '            Enabled = False 'AG 03/08/2010
    '            Cursor = Cursors.WaitCursor
    '            DisabledMdiForms = IMonitor 'AG 17/10/2011
    '            SetActionButtonsEnableProperty(False)    'Disable all buttons

    '            BsAutoConnect.RunWorkerAsync()
    '        End If


    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".Ax00MainMDI_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".Ax00MainMDI_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

    '    End Try
    'End Sub

    Private Sub bsDateTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateTimer.Tick
        Try
            bsTSDateLabel.Text = String.Format("{0} {1}", Today.ToString(SystemInfoManager.OSDateFormat), _
                                               Now.ToString(SystemInfoManager.OSShortTimeFormat))

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsDateTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDateTimer_Tick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Test Parameters Programming
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform screen properties from the global Attributes instead of from DataSet
    ''' </remarks>
    Private Sub TestsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestsToolStripMenuItem.Click, BsTSTestButton.Click
        Try
            IProgTest.AnalyzerModel = AnalyzerModelAttribute
            IProgTest.AnalyzerID = AnalyzerIDAttribute
            IProgTest.WorkSessionID = WorkSessionIDAttribute

            'TR 12/04/2010 
            'Dim myTestProgrammingForm As New TestProgramingParammeters
            'If LocalAnalizerDS.tcfgAnalyzers.Rows.Count > 0 Then
            '    ProgTest.AnalyzerModel = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerModel
            '    ProgTest.AnalyzerID = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerID
            '    ProgTest.WorkSessionID = ""
            'End If

            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IProgTest)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".TestsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Calculated Tests Programming
    ''' </summary>
    Private Sub CalculatedTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalculatedTestToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IProgCalculatedTest)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CalculatedTestToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalculatedTestToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Contaminations
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 16/07/2010 - Inform screen properties from the global Attributes
    ''' </remarks>
    Private Sub ContaminationsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ContaminationsToolStripMenuItem.Click
        Try
            'ProgContaminations.ActiveWorkSession = WorkSessionIDAttribute
            'ProgContaminations.ActiveWSStatus = WSStatusAttribute

            ''This is the way a form should be called from the main window
            ''Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            ''The form to be opened should be assigned its AcceptButton property to its default exit button
            'OpenMDIChildForm(ProgContaminations)

            IProgTestContaminations.ActiveWorkSession = WorkSessionIDAttribute
            IProgTestContaminations.ActiveWSStatus = WSStatusAttribute

            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IProgTestContaminations)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ContaminationsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ContaminationsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 21/10/2010</remarks>
    Private Sub ISEModuleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ISEModuleToolStripMenuItem.Click
        Try
            IProgISETest.AnalyzerID = AnalyzerIDAttribute
            IProgISETest.WorkSessionID = WorkSessionIDAttribute
            OpenMDIChildForm(IProgISETest)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ISEModuleToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ISEModuleToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


    ''' <summary>
    ''' Open the screen of Work Session Preparation
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform screen properties from the global Attributes
    '''              SA 28/07/2010 - Change current code for a call to the same function used when the
    '''                              graphical button with the same functionality is clicked
    '''              SA 02/11/2010 - If screen of WS Preparation is opened, execute same code is executed when
    '''                              the button for opening Rotor Positioning Screen is clicked in the screen
    ''' </remarks>
    Private Sub RotorPositionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RotorPositionsToolStripMenuItem.Click
        Try
            If (IWSSampleRequest.Visible) Then
                IWSSampleRequest.SaveWSWithPositioning()
            Else
                OpenRotorPositionsForm(Nothing)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".RotorPositionsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RotorPositionsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Users Management
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UsersManagementToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UsersManagementToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            'OpenMDIChildForm(ConfigUsers)

            'SG 03/12/10
            'myConfigUsers = New IConfigUsers(Me)
            myConfigUsers = New IConfigUsers()
            OpenMDIChildForm(myConfigUsers)
            'END SG 03/12/10

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".UsersManagementToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UsersManagementToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Test Profiles
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProfilesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProfilesToolStripMenuItem.Click, bsTSProfileButtton.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IProgTestProfiles)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ProfilesToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProfilesToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Screen of Work Session Preparation 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform screen properties from global Attributes
    '''              SA 12/01/2011 - Enable button for WS Reset also when there is not an active WS
    ''' </remarks>
    Private Sub SampleRequestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SampleRequestToolStripMenuItem.Click, bsTSNewSampleButton.Click
        Try
            'Enable button for WS Reset, also when there is not an active WS
            bsTSResetSessionButton.Enabled = True

            'Inform screen properties
            IWSSampleRequest.ActiveAnalyzer = AnalyzerIDAttribute
            IWSSampleRequest.ActiveWorkSession = WorkSessionIDAttribute
            IWSSampleRequest.ActiveWSStatus = WSStatusAttribute

            'RH 05/21/2010 
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IWSSampleRequest)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SampleRequestToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SampleRequestToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Screen of Rotor Positioning
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 02/11/2010 - If screen of WS Preparation is opened, execute same code is executed when
    '''                              the button for opening Rotor Positioning Screen is clicked in the screen
    ''' </remarks>
    Private Sub ManualAssignationOfSamplePosToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSPositionButton.Click
        Try
            If (IWSSampleRequest.Visible) Then
                IWSSampleRequest.SaveWSWithPositioning()
            Else
                OpenRotorPositionsForm(Nothing)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & " ManualAssignationOfSamplePosToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ManualAssignationOfSamplePosToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Patients' Search
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PatientSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PatientSearchToolStripMenuItem.Click
        Try
            'RH 05/21/2010 
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IProgPatientData)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PatientSearchToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PatientSearchToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for SAT Reports Generation 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SATReportsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SATReportsToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(ISATReport)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SATReportsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SATReportsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Execute the RESET of the active WorkSession
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/11/2010 - Inform required Properties before open Monitor Screen
    '''              SA 12/01/2011 - Added process when there is not an active WS:
    '''                               * If there are not requested OrderTests pending to save: close WSPreparation
    '''                               * If there is at least a requested OrderTest pending to save, show the confirmation dialog box.
    '''                                 If answer is yes, close WS Preparation without saving; otherwise, do nothing 
    '''              SA 20/06/2011 - Before executing the Reset, verify if the Standard Tests with requested Controls have exceeded the 
    '''                              allowed maximum number of non cumulated QC Results and in this case, show the auxiliary screen of
    '''                              affected Elements
    '''              TR 16/05/2012 - Implement functionality for progress bar. Commented old functionality related to the active WorkSession 
    '''                              because there is always an active one
    '''              TR 03/07/2012 - Export the log information saved on DB to an XML file
    '''              SA 03/08/2012 - Call to new implementation of ExportLogToXml function
    '''              AG 30/05/2013 - Reset WS process must ignore errors in previous subprocesses (export to LIS, auto SATreport, ...)
    '''                              Fix bug #1102 (refresh in sample request grid while reset ws is in course)
    ''' </remarks>
    Private Sub ResetSessionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSResetSessionButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            CreateLogActivity("Btn Reset", Me.Name & ".ResetSessionButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013

            Dim openMonitorScreen As Boolean = False

            '//JVV 25/09/13 Print Auto-Report si es necesario
            Dim myGlobalDataTO As GlobalDataTO
            Dim IsAutomaticReport As Boolean = False
            Dim myReportFrecuency As String = String.Empty, myReportType As String = String.Empty
            Dim myUserSetting As New UserSettingsDelegate()
            Dim bPrintAutoReport As Boolean = False

            myGlobalDataTO = myUserSetting.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_PRINT.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                If (CType(myGlobalDataTO.SetDatos, Integer) = 1) Then
                    IsAutomaticReport = True
                    myGlobalDataTO = myUserSetting.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_FREQ.ToString)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myReportFrecuency = myGlobalDataTO.SetDatos.ToString()
                    End If
                    myGlobalDataTO = myUserSetting.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_TYPE.ToString)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myReportType = myGlobalDataTO.SetDatos.ToString()
                    End If
                End If
            End If
            bPrintAutoReport = IsAutomaticReport AndAlso myReportFrecuency = "RESET"
            '//JVV 25/09/13


            If (Not String.IsNullOrEmpty(ActiveWorkSession)) Then
                If (MDIAnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY) OrElse _
                   (MDIAnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING) OrElse _
                    Not MDIAnalyzerManager.Connected Then

                    'AG 30/05/2013 - Inform this flag before messagebox asking the user (Fix bug #1102)
                    Dim setIsResetToFalse As Boolean = False
                    If (Not ActiveMdiChild Is Nothing AndAlso (String.Compare(ActiveMdiChild.Name, IWSSampleRequest.Name, False) = 0)) Then
                        Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                        CurrentMdiChild.isWSReset = True
                    End If
                    'AG 30/05/2013

                    If (ShowMessage("AX00", GlobalEnumerates.Messages.CONFIRM_RESET_WS.ToString) = Windows.Forms.DialogResult.Yes) Then

                        'AG 30/10/2013 review - JV + AG revision 21/10/2013 task # 1341
                        showSTARTWSiconFlag = False ' True
                        showPAUSEWSiconFlag = False
                        ChangeStatusImageMultipleSessionButton(WorkSessionUserActions.START_WS) 'ChangeStatusImageMultipleSessionButton()
                        'AG 30/10/2013 review -  JV + AG revision 21/10/2013 task # 1341

                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'Protection case
                        InitializeAutoWSFlags()

                        'Verify if the Standard Tests with requested Controls have exceeded the allowed maximum number of non cumulated QC Results
                        Dim resultData As GlobalDataTO
                        Dim myWSDelegate As New WorkSessionsDelegate
                        Dim myResult As DialogResult = DialogResult.Cancel

                        isResetWSActiveAttribute = True 'TR 06/09/2012 -Set variable value to true when WS Reset Start 

                        resultData = myWSDelegate.ValidateDependenciesOnResetWS(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myDependeciesElementsDS As DependenciesElementsDS = DirectCast(resultData.SetDatos, DependenciesElementsDS)

                            If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                                'Show the auxiliary screen of affected Elements
                                Using AffectedElement As New IWarningAfectedElements
                                    AffectedElement.AffectedElements = myDependeciesElementsDS

                                    AffectedElement.ShowDialog()
                                    myResult = AffectedElement.DialogResult
                                End Using
                            Else
                                myResult = DialogResult.OK
                            End If
                        Else
                            'Error verifying number of non cumulated results, shown it and abort the Reset
                            ShowMessage("AX00", resultData.ErrorCode, resultData.ErrorMessage, Me)
                        End If

                        If (myResult = DialogResult.OK) Then

                            ' Me.Enabled = False 'RH 30/03/2012
                            EnableButtonAndMenus(False) 'AG 08/11/2012 - move here disable UI

                            'SGM 25/09/2012 - reset the ISE error counter
                            If MDIAnalyzerManager.ISE_Manager IsNot Nothing Then
                                MDIAnalyzerManager.ISE_Manager.ISEWSCancelErrorCounter = 0
                            End If
                            'end SGM 25/09/2012

                            'TR 16/05/2012
                            If (Not Me.ActiveMdiChild Is Nothing) Then
                                DisabledMdiForms = Me.ActiveMdiChild
                                Me.ActiveMdiChild.Enabled = False
                            End If
                            'TR 16/05/2012 -END.

                            Cursor = Cursors.WaitCursor
                            SetWorkSessionUserCommandFlags(WorkSessionUserActions.NO_WS)

                            'TR 28/08/2012 -Implement the export if allowed.
                            Dim myExportDelegate As New ExportDelegate
                            Dim myUserSettingDelegate As New UserSettingsDelegate
                            resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTOMATIC_RESET.ToString())
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                If CType(resultData.SetDatos, Boolean) Then
                                    resultData = myExportDelegate.ExportToLISManualNEW(AnalyzerIDAttribute, WorkSessionIDAttribute, True)

                                    'AG + SG 11/04/2013 - upload results to LIS
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        Dim exportResults As New ExecutionsDS
                                        exportResults = CType(resultData.SetDatos, ExecutionsDS)
                                        CreateLogActivity("Current Results upload in RESET WS", Me.Name & ".ResetSessionButton_Click ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)

                                        'Inform the new results to be updated into MDI property
                                        AddResultsIntoQueueToUpload(exportResults)

                                        'Call assynchronously to a synchronous process using another thread but not wait
                                        'At this point call the synchronous process instead of the assynchronous!!!
                                        'InvokeUploadResultsLIS(False, Nothing, Nothing, Nothing)
                                        SynchronousLISManagerUploadResults(False, Nothing, Nothing, Nothing)
                                    End If
                                    'AG + SG 11/04/2013
                                End If
                            Else
                                'Error getting the Session Setting value, show it 
                                ShowMessage(Name & ".ResetSessionButton_Click ", resultData.ErrorCode, resultData.ErrorMessage)
                                EnableButtonAndMenus(True) 'AG 08/11/2012
                                openMonitorScreen = True 'AG 08/11/2012
                                resultData.HasError = False 'AG 30/05/2013 - Reset WS process must ignore errors in previous subprocesses
                            End If
                            'TR 28/08/2012 -END.

                            '//JVV 26/09/2013 Auto-Report si corresponde.
                            If bPrintAutoReport Then
                                If (String.Equals(myReportType, "COMPACT")) Then
                                    XRManager.PrintCompactPatientsReport(AnalyzerIDAttribute, WorkSessionIDAttribute, Nothing, True)
                                ElseIf (String.Equals(myReportType, "INDIVIDUAL")) Then
                                    XRManager.PrintPatientsFinalReport(AnalyzerIDAttribute, WorkSessionIDAttribute, Nothing)
                                End If
                            End If
                            '//JVV 26/09/2013

                            Dim existsErrorFlag As Boolean = False 'AG 30/05/2013
                            Dim myErrorMessage As String = "" 'AG 30/05/2013
                            If (Not resultData.HasError) Then

                                'DL 31/05/2013
                                Dim myLogMaxDays As Integer = 30
                                Dim myParams As New SwParametersDelegate

                                resultData = myParams.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_DAYS_IN_PREVIOUSLOG.ToString(), Nothing)
                                If Not resultData.HasError Then myLogMaxDays = CInt(resultData.SetDatos)
                                'DL 31/05/2013

                                resultData = myLogAcciones.ExportLogToXml(WorkSessionIDAttribute, myLogMaxDays)
                                'If expor to xml OK then delete all records on Application log Table
                                If (Not resultData.HasError) Then resultData = myLogAcciones.DeleteAll()
                                'TR 03/07/2012 -END

                                If resultData.HasError Then 'AG 30/05/2013 - Reset WS process must ignore errors in previous subprocesses (the warning will show later)
                                    existsErrorFlag = True
                                    resultData.HasError = False
                                    myErrorMessage = resultData.ErrorMessage
                                End If
                            End If

                            If (Not resultData.HasError) Then

                                'TR 04/10/2011 -Implement new method.
                                'SetActionButtonsEnableProperty(False) 'AG 12/07/2011 - Disable all vertical action buttons bar
                                EnableButtonAndMenus(False) 'AG 08/11/2012 - move after 'If (myResult = DialogResult.OK) Then'

                                MDILISManager.ClearQueueOfSpecimenNotResponded() 'AG 23/07/2013 - patients are removed from rotor in reset so it has no sense keep this queue
                                Dim workingThread As New Threading.Thread(AddressOf ResetSession)
                                ScreenWorkingProcess = True
                                processingReset = True
                                workingThread.Start()

                                'Cursor = Cursors.WaitCursor

                                Application.DoEvents()
                                While processingReset 'ScreenWorkingProcess
                                    Me.InitializeMarqueeProgreesBar()
                                    Application.DoEvents()
                                End While
                                Application.DoEvents()

                                workingThread = Nothing
                                StopMarqueeProgressBar()

                                'TR 16/05/2012
                                Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
                                If Not ActiveMdiChild Is Nothing Then
                                    ActiveMdiChild.Enabled = True
                                ElseIf DisabledMDIChildAttribute.Count > 0 Then
                                    myCurrentMDIForm = DisabledMDIChildAttribute(0)
                                    myCurrentMDIForm.Enabled = True
                                    If DisabledMDIChildAttribute.Count = 1 Then
                                        DisabledMDIChildAttribute.Clear()
                                    End If
                                End If
                                'TR 16/05/2012
                                openMonitorScreen = True

                                ' XB 23/04/2013 - emplace ShowErrors here when thread has finished
                                If Not myErrorsList Is Nothing Then
                                    If myErrorsList.Count = 0 Then
                                        ShowMessage(Name, GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
                                    ElseIf myErrorsList.Count = 1 Then
                                        ShowMessage(Name, myErrorsList(0))
                                    ElseIf myErrorsList.Count > 1 Then
                                        'ShowMultipleMessage(Name, myErrorsList)    ' by now there are a unique generic message
                                        ShowMessage(Name, myErrorsList(0))
                                    End If
                                End If
                                myErrorsList = Nothing
                                ' XB 23/04/2013

                                'AG 30/05/2013
                                'Else
                                '    'Show error message
                                '    ShowMessage(Name, GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, resultData.ErrorMessage)
                                '    EnableButtonAndMenus(True)
                                '    openMonitorScreen = True
                            End If

                            'AG 30/05/2013
                            If existsErrorFlag Then
                                ShowMessage(Name, GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myErrorMessage)
                                EnableButtonAndMenus(True)
                                openMonitorScreen = True
                            End If
                            'AG 30/05/2013

                        Else
                            setIsResetToFalse = True 'AG 30/05/2013 - isReset = FALSE (Fix bug #1102)
                        End If

                    Else
                        setIsResetToFalse = True 'AG 30/05/2013 - isReset = FALSE (Fix bug #1102)
                    End If

                    If setIsResetToFalse Then
                        'AG 30/05/2013 - Inform this flag before messagebox asking the user (Fix bug #1102)
                        If (Not ActiveMdiChild Is Nothing AndAlso (String.Compare(ActiveMdiChild.Name, IWSSampleRequest.Name, False) = 0)) Then
                            Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                            CurrentMdiChild.isWSReset = False
                        End If
                        'AG 30/05/2013
                    End If
                Else
                    'If the current WS in not SLEEPING or in STANDBY, it is not allowed the reset
                    ShowMessage("AX00", GlobalEnumerates.Messages.RESET_NOTALLOW.ToString)
                End If
                'TR 16/05/2012 -This functionality is not used any more 'cause there's allways and active worksession.
                'Else  
                '    If (IWSSampleRequest.PendingToSaveOrderTests) Then
                '        If (ShowMessage("AX00", GlobalEnumerates.Messages.CONFIRM_RESET_WS.ToString) = DialogResult.Yes) Then
                '            openMonitorScreen = True
                '        End If
                '    Else
                '        'TR 16/05/2012 -Validate if the active mdi is the monitor form else open it.
                '        If Not ActiveMdiChild Is Nothing AndAlso Not ActiveMdiChild.Name = "IMonitor" Then
                '            openMonitorScreen = True
                '        End If
                '    End If
                'TR 16/05/2012 - END comment.
            End If

            If (openMonitorScreen) Then
                'AG 30/05/2013 - Inform this flag before messagebox asking the user (Fix bug #1102)
                'TR 03/04/2012 -Validate if sample request screen  then set reset property to true.
                'If (Not ActiveMdiChild Is Nothing AndAlso (String.Compare(ActiveMdiChild.Name, IWSSampleRequest.Name, False) = 0)) Then
                '    Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                '    CurrentMdiChild.isWSReset = True
                'End If
                'TR 03/04/2012 -END.
                'AG 30/05/2013

                WorkSessionIDAttribute = ""
                WSStatusAttribute = ""
                '
                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                Dim myGlobal As New GlobalDataTO
                myGlobal = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(Nothing)

                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    Dim myWSAnalyzersDS As New WSAnalyzersDS
                    myWSAnalyzersDS = DirectCast(myGlobal.SetDatos, WSAnalyzersDS)

                    If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                        If String.Equals(WorkSessionIDAttribute, String.Empty) Then
                            WorkSessionIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID
                        End If
                    End If
                End If
                'DL 01/10/2012

                If (Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing)) Then
                    MDIAnalyzerManager.ActiveWorkSession = ""
                End If

                'EnableWSMenu()
                CloseActiveMdiChild()

                'Open the Monitor Screen an update status of the Vertical Buttons Bar
                OpenMonitorForm(Nothing)
                'SetActionButtonsEnableProperty(True)
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("TOTAL TIME WorkSession RESET + EXPORT + MENU + OPEN MONITOR SCREEN " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ResetSessionButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            Me.Enabled = True 'RH 30/03/2012
            CreateLogActivity(ex.Message, Name & ".ResetSessionButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResetSessionButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            'Me.Enabled = True 'RH 30/03/2012
            'RH 30/03/2012 Move code here because we want it to be executed always at the end of the method
            'whenever there is an exception or not
            Cursor = Cursors.Default
            StopMarqueeProgressBar()
            'TR 04/10/2011 -Implement new method.
            'SetActionButtonsEnableProperty(True) 'AG 12/07/2011 - Enable vertical action buttons bar
            EnableButtonAndMenus(True)
            Application.DoEvents()
            'END RH 30/03/2012

            'TR Set value to false 'cause reset isfinished
            isResetWSActiveAttribute = False

        End Try
    End Sub

    ''' <summary>
    ''' Execute the LIMS Import process
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/09/2010
    ''' </remarks>
    Private Sub LIMSImportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LIMSImportToolStripMenuItem.Click
        Try
            LIMSImportToolStripMenuItem.Enabled = False
            If (IWSSampleRequest.Visible) Then
                IWSSampleRequest.ExecuteImportFromLIMSProcess()
            Else
                'Inform the required screen properties and open the screen
                IWSSampleRequest.ActiveAnalyzer = AnalyzerIDAttribute
                IWSSampleRequest.ActiveWorkSession = WorkSessionIDAttribute
                IWSSampleRequest.ActiveWSStatus = WSStatusAttribute
                IWSSampleRequest.OpenForLIMSImport = True

                OpenMDIChildForm(IWSSampleRequest)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSMonitorButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSMonitorButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Allow loading of a Saved WorkSession
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform the Screen Properties from the global Attributes
    '''              SA 23/09/2010 - Before open the screen, verify it is not currently opened as MDIChild for
    '''                              a different use, and in this case, close the opened instance. Besides,
    '''                              if screen is closed by clicking in EXIT button, then open Monitor Screen
    '''              SA 23/11/2010 - Inform required Properties before open WSMonitor Screen
    '''              SA 12/01/2011 - Before opening WS Preparation Screen, activate button for Reset WS
    ''' </remarks>
    Private Sub LoadSessionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadSessionToolStripMenuItem.Click
        Try
            'First verify is the screen is opened with a different configuration (the same screen is used to
            'Load or Save Saved WS and also VirtualRotors)
            Dim cancelOpening As Boolean = False
            If (Not ActiveMdiChild Is Nothing) Then
                'If (ActiveMdiChild.Name = "WSLoadSaveAuxScreen") Then

                'RH 30/06/2011 Do not rely in literal texts for the comparison, because texts can change
                'without the compiler be aware of it. Instead compare Types or Properties
                If (String.Compare(ActiveMdiChild.Name, IWSLoadSaveAuxScreen.Name, False) = 0) Then
                    If (String.Compare(IWSLoadSaveAuxScreen.ScreenUse, "SAVEDWS", False) = 0 AndAlso String.Compare(IWSLoadSaveAuxScreen.SourceButton, "LOAD", False) = 0) Then
                        cancelOpening = True
                    Else
                        'Execute the closing code of the screen currently open
                        IWSLoadSaveAuxScreen.Tag = "Performing Click" 'RH 17/12/2010 To tell the form it is closed by hand
                        IWSLoadSaveAuxScreen.AcceptButton.PerformClick()
                    End If

                ElseIf Not ExistSavedWorkSession() Then 'TR 01/08/2011 -Validate if there're any saved WorkSession.
                    cancelOpening = True
                End If
            End If

            If (Not cancelOpening) Then
                'Inform screen properties needed for initial configuration and open the screen as MDIChild
                IWSLoadSaveAuxScreen.ScreenUse = "SAVEDWS"
                IWSLoadSaveAuxScreen.SourceButton = "LOAD"

                OpenMDIChildForm(IWSLoadSaveAuxScreen)

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LoadSessionToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSessionToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Allow saving the active WorkSession 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/09/2010 - Open the Load Saved WS screen as MDIChild instead of as MODAL
    '''              SA 29/08/2012 - Inform property for the active AnalyzerID before opening the auxiliary screen
    ''' </remarks>
    Private Sub SaveSessionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveSessionToolStripMenuItem.Click
        Try
            'First verify is the screen is opened with a different configuration (the same screen is used to
            'Load or Save Saved WS and also VirtualRotors)
            Dim cancelOpening As Boolean = False
            'If (Not ActiveMdiChild Is Nothing) AndAlso (ActiveMdiChild.Name = "WSLoadSaveAuxScreen") Then

            'RH 30/06/2011 Do not rely in literal texts for the comparison, because texts can change
            'without the compiler be aware of it. Instead compare Types or Properties
            If (Not ActiveMdiChild Is Nothing) AndAlso (String.Compare(ActiveMdiChild.Name, IWSLoadSaveAuxScreen.Name, False) = 0) Then
                If (String.Compare(IWSLoadSaveAuxScreen.ScreenUse, "SAVEDWS", False) = 0 AndAlso String.Compare(IWSLoadSaveAuxScreen.SourceButton, "SAVE", False) = 0) Then
                    cancelOpening = True
                Else
                    'Execute the closing code of the screen currently open
                    IWSLoadSaveAuxScreen.Tag = "Performing Click" 'RH 17/12/2010 To tell the form it is closed by hand
                    IWSLoadSaveAuxScreen.AcceptButton.PerformClick()
                End If
            End If

            If (Not cancelOpening) Then
                'Inform screen properties needed for initial configuration and open the screen as MDIChild
                IWSLoadSaveAuxScreen.ScreenUse = "SAVEDWS"
                IWSLoadSaveAuxScreen.SourceButton = "SAVE"
                IWSLoadSaveAuxScreen.ActiveWorkSession = WorkSessionIDAttribute
                IWSLoadSaveAuxScreen.ActiveAnalyzer = AnalyzerIDAttribute

                OpenMDIChildForm(IWSLoadSaveAuxScreen)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SaveSessionToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveSessionToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow deletion of Saved WorkSessions 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/09/2010 - Screen opened as MDIChild instead of a Modal Popup
    ''' </remarks>
    Private Sub DeleteSessionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteSessionToolStripMenuItem.Click
        Try
            'First verify is the screen is opened with a different configuration (the same screen is used to
            'Delete Saved WS and also VirtualRotors)
            Dim cancelOpening As Boolean = False

            'If (Not ActiveMdiChild Is Nothing) AndAlso (ActiveMdiChild.Name = "WSDeleteAuxScreen") Then

            'RH 30/06/2011 Do not rely in literal texts for the comparison, because texts can change
            'without the compiler be aware of it. Instead compare Types or Properties
            If (Not ActiveMdiChild Is Nothing) AndAlso (String.Compare(ActiveMdiChild.Name, IWSDeleteAuxScreen.Name, False) = 0) Then
                If (String.Compare(IWSDeleteAuxScreen.ScreenUse, "SAVEDWS", False) = 0) Then
                    cancelOpening = True
                Else
                    'Execute the closing code of the screen currently open
                    IWSDeleteAuxScreen.Tag = "Performing Click" 'RH 17/12/2010 To tell the form it is closed by hand
                    IWSDeleteAuxScreen.AcceptButton.PerformClick()
                End If

            ElseIf Not ExistSavedWorkSession() Then 'TR 01/08/2011 -Validate if there're any saved WorkSession.
                'TR 15/02/2012 -Open Monitor Windows.
                OpenMonitorForm()
                IMonitor.RefreshAlarmsGlobes(Nothing)
                cancelOpening = True
            End If

            If (Not cancelOpening) Then
                IWSDeleteAuxScreen.AnalyzerModel = AnalyzerModelAttribute
                IWSDeleteAuxScreen.ScreenUse = "SAVEDWS"

                OpenMDIChildForm(IWSDeleteAuxScreen)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".DeleteSessionToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSessionToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen of Calibrators Programming
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalibratorsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibratorsToolStripMenuItem.Click
        Try

            IProgCalibrator.AnalyzerID = AnalyzerIDAttribute
            IProgCalibrator.WorkSessionID = WorkSessionIDAttribute
            OpenMDIChildForm(IProgCalibrator)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CalibratorsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalibratorsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Application closing method
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 22/04/2010 
    ''' Modified by: AG 17/06/2010 - Ask confirmation and close current screen properly (Using the code of the property AcceptButton)
    '''              TR 21/10/2011 -Make sure the DetectorForm Get clos on application closing.
    '''              AG 25/02/2013 - Release and disponse the LIS manager object
    '''              TR 10/07/2013 - Implement the shrink database when application's close.
    ''' </remarks>
    Private Sub ApplicationClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try

            ' to manage the ISE utilities closing SGM 11/05/2012
            Dim res As DialogResult = Windows.Forms.DialogResult.No

            If MyClass.IsISEUtilClosing Then
                res = Windows.Forms.DialogResult.Yes
            ElseIf flagExitWithShutDown Then            'DL 04/06/2012
                res = Windows.Forms.DialogResult.Yes    'DL 04/06/2012
            Else
                res = ShowMessage(Name, GlobalEnumerates.Messages.EXIT_PROGRAM.ToString())
            End If
            'end SGM 11/05/2012

            If res = Windows.Forms.DialogResult.Yes Then
                'DL 17/04/2013
                If Not watchDogTimer Is Nothing Then
                    watchDogTimer.Enabled = False
                    RemoveHandler watchDogTimer.Elapsed, AddressOf watchDogTimer_Timer
                    watchDogTimer = Nothing
                End If
                'DL 17/04/2013

                'Close the current screen (if any)
                If (OpenMDIChildForm(Nothing)) Then

                    'Close communication channel
                    If (Not MDIAnalyzerManager Is Nothing) Then
                        'Dim myGlobal As New GlobalDataTO
                        'Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                        'TR 27/10/2011 -Stop Sound.
                        MDIAnalyzerManager.StopAnalyzerRinging()

                        Application.DoEvents()

                        'SG 16/07/2012 -Stop INFO.
                        MDIAnalyzerManager.StopAnalyzerInfo()

                        Application.DoEvents()

                        If (MDIAnalyzerManager.CommThreadsStarted) Then
                            'Dispose communications channel
                            MDIAnalyzerManager.Terminate()
                        End If
                    End If

                    'AG 25/02/2013 - Release the LIS manager object using an auxiliar thread
                    InvokeReleaseLIS()

                    'AG 25/02/2013 - show the following auxiliary screen also when Sw is releasing the LIS object
                    'RH 30/03/2012 Don't execute these lines when back ground work has been finished.
                    'If RunningAx00MDBackGround Then
                    If RunningAx00MDBackGround Or ProcessingLISManagerObject Or ProcessingLISManagerUploadResults Or processingLISOrderDownload Then
                        'RH 03/11/2010 Wait until Ax00MDBackGround.RunWorkerAsync() is completed
                        'AG - Not multilanguage text
                        Using wfPreload As New IAx00StartUp(Me)
                            wfPreload.Title = "Waiting ongoing processes completion..."
                            wfPreload.WaitText = "Please wait..."
                            wfPreload.Show()
                            'While RunningAx00MDBackGround
                            While RunningAx00MDBackGround Or ProcessingLISManagerObject Or ProcessingLISManagerUploadResults Or processingLISOrderDownload 'AG 25/02/2013
                                Application.DoEvents()
                                'wfPreload.RefreshLoadingImage()
                                System.Threading.Thread.Sleep(100)
                            End While
                            wfPreload.Close()
                        End Using
                    End If
                    'AG 25/02/2013

                    'RH 15/10/2010
                    'Unregister CommAX00.exe COM server
                    'System.Diagnostics.Process.Start("CommAX00.exe", "/unregserver")

                    'RH 01/03/2012 Disable and release bsDateTimer
                    bsDateTimer.Enabled = False
                    bsDateTimer = Nothing

                    ''TR 21/10/2011 -Make sure the DetectorForm get close.
                    'If Application.OpenForms.Count > 0 Then
                    '    For Each myform As Form In Application.OpenForms
                    '        If myform.GetType().Name = "DetectorForm" Then
                    '            myform.Close()
                    '            Exit For
                    '        End If
                    '    Next
                    'End If
                    ''TR 21/10/2011 -END.

                    Me.Owner = Nothing 'RH 02/03/2012 Remove IBackground

                    'RH 01/03/2012 Close all opened forms
                    Dim Index As Integer = 0
                    While Application.OpenForms.Count > 1 'Only left this one open
                        If String.Equals(Application.OpenForms(Index).Name, Me.Name) Then
                            Index += 1
                        Else
                            Application.OpenForms(Index).Close()
                        End If
                    End While

                    'MDIAnalyzerManager = Nothing

                    'TR 10/07/2013 -Implement the Shrink Database before close application.
                    Dim myDataBaseUpdateManagerDelegate As New DataBaseUpdateManagerDelegate
                    myDataBaseUpdateManagerDelegate.ShrinkDatabase(Biosystems.Ax00.DAL.DAOBase.CurrentDB, Nothing)
                    'TR 10/07/2013 -END.

                    'SGM 07/11/2012 log app end
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(My.Application.Info.ProductName & " - Application END", "IAx00MainMDI_FormClosing", EventLogEntryType.Information, False)

                    'IR 27/09/2012 Kill main application process
                    Process.GetCurrentProcess().Kill()
                Else
                    e.Cancel = True
                End If
            Else
                e.Cancel = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & " ApplicationClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Open screen of Analyzer Configuration 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 06/07/2010 - Inform property for the AnalyzerID before opening the Form
    ''' </remarks>
    Private Sub AnalyzerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalyzerToolStripMenuItem.Click
        Try
            'ConfigAnalyzers.ActiveAnalyzer = AnalyzerIDAttribute
            'OpenMDIChildForm(ConfigAnalyzers)

            'SG 03/12/10
            'myConfigAnalyzers = New IConfigGeneral(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
            myConfigAnalyzers = New IConfigGeneral() With {.ActiveAnalyzer = AnalyzerIDAttribute}
            OpenMDIChildForm(myConfigAnalyzers)
            'END 03/12/10

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AnalyzerToolStripMenuItem_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AnalyzerToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen that allow set the current Application Language
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LanguageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LanguageToolStripMenuItem.Click
        Try
            'myLanguageConfig = New IConfigLanguage(Me)
            myLanguageConfig = New IConfigLanguage()
            OpenMDIChildForm(myLanguageConfig)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LanguageToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Close without shut down
    ''' </summary>
    ''' <remarks>
    ''' Created by DL 04/04/2012
    ''' </remarks>
    Private Sub WithOutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WithOutToolStripMenuItem.Click

        Try
            Me.Close() 'Activate ApplicationClosing

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".WithOutToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open screen of Analyzer Configuration
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 06/07/2010 - Inform property for the AnalyzerID before open the Form
    ''' </remarks>
    Private Sub bsTSAnalysersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSAnalysersButton.Click
        Try
            'ConfigAnalyzers.ActiveAnalyzer = AnalyzerIDAttribute
            'OpenMDIChildForm(ConfigAnalyzers)
            'SG 03/12/10
            'myConfigAnalyzers = New IConfigGeneral(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
            myConfigAnalyzers = New IConfigGeneral() With {.ActiveAnalyzer = AnalyzerIDAttribute}
            OpenMDIChildForm(myConfigAnalyzers)
            'END 03/12/10

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSAnalysersButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSAnalysersButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for SAT Reports Generation 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsTSReportSATButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSReportSATButton.Click
        Try
            OpenMDIChildForm(ISATReport)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSReportSATButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen that allows deletion of the selected Virtual Rotors
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/09/2010 - Screen opened as MDIChild instead of a Modal Popup
    ''' </remarks>
    Private Sub DeleteVirtualRotorsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteVirtualRotorsToolStripMenuItem.Click
        Try
            'First verify is the screen is opened with a different configuration (the same screen is used to
            'Delete Saved WS and also VirtualRotors)
            Dim cancelOpening As Boolean = False

            'If (Not ActiveMdiChild Is Nothing) AndAlso (ActiveMdiChild.Name = "WSDeleteAuxScreen") Then

            'RH 30/06/2011 Do not rely in literal texts for the comparison, because texts can change
            'without the compiler be aware of it. Instead compare Types or Properties
            If (Not ActiveMdiChild Is Nothing) AndAlso String.Equals(ActiveMdiChild.Name, IWSDeleteAuxScreen.Name) Then
                If String.Equals(IWSDeleteAuxScreen.ScreenUse, "VROTORS") Then
                    cancelOpening = True
                Else
                    'Execute the closing code of the screen currently open
                    IWSDeleteAuxScreen.Tag = "Performing Click" 'RH 17/12/2010 To tell the form it is closed by hand
                    IWSDeleteAuxScreen.AcceptButton.PerformClick()
                End If
            ElseIf Not ExistVirtualSavedRotor() Then
                cancelOpening = True
            End If

            If (Not cancelOpening) Then
                IWSDeleteAuxScreen.AnalyzerModel = AnalyzerModelAttribute
                IWSDeleteAuxScreen.ScreenUse = "VROTORS"

                If (TypeOf ActiveMdiChild Is IMonitor) Then
                    ActiveMdiChild.Close()
                End If

                OpenMDIChildForm(IWSDeleteAuxScreen)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".DeleteVirtualRotorsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteVirtualRotorsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Results Review screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsTSResultsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSResultsButton.Click, ResultsToolStripMenuItem.Click
        Try
            'OpenMDIChildForm(New IResults())
            IResults.ActiveWSStatus = WSStatusAttribute 'AG 20/03/2012 - inform the WS status (used to disable some buttons)
            IResults.AnalyzerModel = AnalyzerModelAttribute
            OpenMDIChildForm(IResults)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSResultsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSResultsButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Monitor screen
    ''' </summary>
    ''' <remarks>
    ''' 
    ''' </remarks>
    Private Sub bsTSMonitorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSMonitorButton.Click, MonitorToolStripMenuItem.Click
        Try
            'RH 16/12/2010
            OpenMonitorForm(Nothing)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSMonitorButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSMonitorButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Load ReportSAT screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub LoadSATReportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadSATReportToolStripMenuItem.Click
        Try
            ISATReportLoad.RestorePointMode = False
            OpenMDIChildForm(ISATReportLoad)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LoadSATReportToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSATReportToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Restore Previous Data screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RestorePreviousDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RestorePreviousDataToolStripMenuItem.Click
        Try
            ISATReportLoad.RestorePointMode = True
            OpenMDIChildForm(ISATReportLoad)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".RestorePreviousDataToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RestorePreviousDataToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open Create Restore Point screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CreateRestorePointToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateRestorePointToolStripMenuItem.Click
        Try
            OpenMDIChildForm(ICreateRestorePoint)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".RestorePreviousDataToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RestorePreviousDataToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the ProgOffSystemTest screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OffSystemModuleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffSystemModuleToolStripMenuItem.Click
        Try
            OpenMDIChildForm(IProgOffSystemTest)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OffSystemModuleToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OffSystemModuleToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    '''' <summary>
    '''' Open legend form
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>DL 22/06/2011 </remarks>
    'Private Sub LegendPicture_Click(ByVal sender As Object, ByVal e As System.EventArgs)

    '    Try
    '        Cursor = Cursors.WaitCursor

    '        Dim workingThread As New Threading.Thread(AddressOf ShowLegendMain)
    '        OpeningLegend = True
    '        workingThread.Start()

    '        While OpeningLegend
    '            InitializeMarqueeProgreesBar()
    '            Cursor = Cursors.WaitCursor
    '            Application.DoEvents()
    '        End While

    '        workingThread = Nothing
    '        StopMarqueeProgressBar()

    '        Cursor = Cursors.Default

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".LegendPicture_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".LegendPicture_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Start Show Legend thread 
    '''' </summary>
    '''' <remarks>DL 22/06/2011 </remarks>
    'Private Sub LegendToolButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LegendToolButton.Click

    '    Try
    '        Cursor = Cursors.WaitCursor

    '        Dim workingThread As New Threading.Thread(AddressOf ShowLegendMain)
    '        workingThread.SetApartmentState(Threading.ApartmentState.STA)

    '        OpeningLegend = True
    '        workingThread.Start()

    '        While OpeningLegend
    '            InitializeMarqueeProgreesBar()
    '            Application.DoEvents()
    '        End While

    '        workingThread = Nothing
    '        StopMarqueeProgressBar()

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".LegendToolButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".LegendToolButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

    '    Finally
    '        Cursor = Cursors.Default

    '    End Try

    'End Sub

    Private Sub ControlsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ControlsToolStripMenuItem.Click
        IProgControls.AnalyzerID = AnalyzerIDAttribute
        IProgControls.WorkSessionID = WorkSessionIDAttribute

        OpenMDIChildForm(IProgControls)
    End Sub

    Private Sub SampleResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SampleResultsToolStripMenuItem.Click
        'OpenMDIChildForm(IHistoricResultsReview)
        OpenMDIChildForm(IHisResults) 'JB 22/10/2012
    End Sub

    Private Sub BlankCalibratorResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BlankCalibratorResultsToolStripMenuItem.Click
        OpenMDIChildForm(IHisBlankCalibResults)
    End Sub

    Private Sub QCResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCResultsToolStripMenuItem.Click
        IQCResultsReview.AnalyzerID = AnalyzerIDAttribute
        OpenMDIChildForm(IQCResultsReview)
    End Sub

    Private Sub QCCumulatedResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCCumulatedResultsToolStripMenuItem.Click
        IQCCumulatedReview.AnalyzerID = AnalyzerIDAttribute
        OpenMDIChildForm(IQCCumulatedReview)
    End Sub

    Private Sub ISEResultsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ISEResultsToolStripMenuItem.Click
        IISEResultsHistory.AnalyzerID = AnalyzerIDAttribute
        OpenMDIChildForm(IISEResultsHistory)
    End Sub

    Private Sub HistoricalAnalyzerAlarmsReviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoricalAnalyzerAlarmsReviewToolStripMenuItem.Click
        IHisAlarms.AnalyzerID = AnalyzerIDAttribute
        OpenMDIChildForm(IHisAlarms)
    End Sub

    Private Sub bsTSQCResultsReview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSQCResultsReview.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            IQCResultsReview.AnalyzerID = AnalyzerIDAttribute
            OpenMDIChildForm(IQCResultsReview)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSQCResultsReview_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSQCResultsReview_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for change Rotor UTILITY 
    ''' </summary>
    ''' <remarks>AG 28/06/2011</remarks>
    Private Sub ChangeReactionsRotorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeReactionsRotorToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IChangeRotor)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ChangeReactionsRotorToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeReactionsRotorToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Open the screen for CONDITIONING Utility
    ''' </summary>
    ''' <remarks>DL 05/06/2012</remarks>
    Private Sub ConditioningToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConditioningToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IConditioning)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ConditioningToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ConditioningToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for change Barcode configuration 
    ''' </summary>
    ''' <remarks>RH 11/07/2011</remarks>
    Private Sub BarcodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarcodeToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button

            ' XBC 14/02/2012 BarCode Configuration placed into PresetationCOM layer
            'DL 22/07/2011
            'OpenMDIChildForm(IBarCodesConfig)
            'myConfigBarCode = New IBarCodesConfig(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
            myConfigBarCode = New IBarCodesConfig() With {.ActiveAnalyzer = AnalyzerIDAttribute, .WorkSessionID = WorkSessionIDAttribute}
            OpenMDIChildForm(myConfigBarCode)
            'END DL 22/07/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".BarcodeToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BarcodeToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for change LIS mapping configuration 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XB 01/03/2013</remarks>
    Private Sub LISMappingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LISMappingToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IConfigLISMapping)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LISMappingToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LISMappingToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for change LIS settings configuration 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XB 04/03/2013</remarks>
    Private Sub LISConfigToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LISConfigToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            OpenMDIChildForm(IConfigLIS)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LISConfigToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LISConfigToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    Private Sub bsTSInfoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSInfoButton.Click
        Try

            Dim myLegend As String

            Select Case Me.ActiveMdiChild.Name
                Case IWSRotorPositions.Name
                    myLegend = LEGEND_SOURCE.LEGEND_ROTOR.ToString()

                Case IWSSampleRequest.Name
                    myLegend = LEGEND_SOURCE.LEGEND_PREPARATION.ToString()

                Case Else
                    myLegend = String.Empty 'RH 30/06/2011
            End Select

            Using myForm As New ILegend()
                myForm.ParentMDI = Me.Location
                myForm.SourceForm = myLegend
                OpeningLegend = False
                myForm.ShowDialog()
            End Using
            'OpenMDIChildForm(myForm)

            'Cursor = Cursors.WaitCursor

            'Dim workingThread As New Threading.Thread(AddressOf ShowLegendMain)
            'workingThread.SetApartmentState(Threading.ApartmentState.STA)

            'OpeningLegend = True
            'workingThread.Start()

            'While OpeningLegend
            '    InitializeMarqueeProgreesBar()
            '    Application.DoEvents()
            'End While

            'workingThread = Nothing
            'StopMarqueeProgressBar()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSInfoButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSInfoButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    Private Sub CumulatedButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CumulatedButton.Click
        IQCCumulatedReview.AnalyzerID = AnalyzerIDAttribute
        OpenMDIChildForm(IQCCumulatedReview)
    End Sub


    'Public Sub bsRemTimeTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsRemTimeTimer.Tick
    '    'Try
    '    '    If (Not ActiveMdiChild Is Nothing) Then
    '    '        If (TypeOf ActiveMdiChild Is IMonitor) Then
    '    '            Dim CurrentMdiChild As IMonitor = DirectCast(ActiveMdiChild, IMonitor)

    '    '            Dim elapsedSeconds As Single = CurrentMdiChild.ConvertHHmmssInSeconds(Convert.ToDateTime(CurrentMdiChild.ElapsedTimeTextEdit.Text))
    '    '            Dim overallSeconds As Single = CurrentMdiChild.ConvertHHmmssInSeconds(Convert.ToDateTime(CurrentMdiChild.OverallTimeTextEdit.Text))

    '    '            Dim newPosition As Integer = Convert.ToInt32((elapsedSeconds / overallSeconds) * 100)

    '    '            CurrentMdiChild.TaskListProgressBar.Position = newPosition
    '    '            CurrentMdiChild.TaskListProgressBar.PerformStep()
    '    '            CurrentMdiChild.TaskListProgressBar.Update()
    '    '        End If

    '    '    End If
    '    'Catch ex As Exception
    '    '    CreateLogActivity(ex.Message, Name & ".bsRemTimeTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '    '    ShowMessage(Name & ".bsRemTimeTimer_Tick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    'End Try
    'End Sub

    ''' <summary>
    ''' Occurs when the specified timer interval has elapsed and the timer is enabled.
    ''' </summary>
    ''' <remarks>
    ''' DL 09/09/2011
    ''' Modified by: RH 06/03/2012 - Remove every unneeded and memory waste "New" instruction.
    '''                              As this method will run every few seconds, it will create a great presure
    '''                              over the garbage collector if we use non necessary memory request instructions.
    '''              XB 30/01/2013 - Change CType conversion DateTime to Invariant Format (Bugs tracking #1121)
    ''' </remarks>
    Public Sub BsTimerWUp_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTimerWUp.Tick
        Try
            'Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsDS As AnalyzerSettingsDS
            'Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
            Dim resultData As GlobalDataTO = Nothing
            Dim dtStartDate As Date
            Dim myDate As String = ""
            Dim WUPFullTime As Single

            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                       AnalyzerIDAttribute, _
                                                                       GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                    myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue
                End If

                If String.Equals(myDate.Trim, String.Empty) Then
                    Exit Sub
                Else
                    'dtStartDate = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Date) '.ToString("yyyyMMdd hh-mm")
                    ''dtStartDate = CDate(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue) testings
                    dtStartDate = DateTime.Parse(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, CultureInfo.InvariantCulture)
                End If

                'Dim swParams As New SwParametersDelegate

                ' Read W-Up full time configuration
                resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)

                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    'Dim myParametersDS As New ParametersDS
                    Dim myParametersDS As ParametersDS

                    myParametersDS = CType(resultData.SetDatos, ParametersDS)
                    'Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)
                    Dim myList As List(Of ParametersDS.tfmwSwParametersRow)

                    myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                              Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString) _
                              Select a).ToList

                    If myList.Count > 0 Then WUPFullTime = myList(0).ValueNumeric

                End If

                'Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                           AnalyzerIDAttribute, _
                                                                           GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString())

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        If String.Compare(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, "1", False) = 0 Then
                            IMonitor.bsEndWarmUp.Visible = True
                            completeWupProcessFlag = True
                            SetEnableMainTab(True) 'RH 26/03/2012
                            MyClass.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = False 'SGM 17/05/2012
                            'Else
                            'IMonitor.bsEndWarmUp.Visible = False
                        End If

                    End If
                End If

                If Not ActiveMdiChild Is Nothing Then
                    'AG 07/10/2011 - Finish automatically Wup when time is complete
                    'If (TypeOf ActiveMdiChild Is IMonitor) Then
                    '    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    '    Dim tsDiff As TimeSpan
                    '    tsDiff = Now.Subtract(dtStartDate)

                    '    If tsDiff.TotalSeconds >= (WUPFullTime * 60) And completeWupProcessFlag Then
                    '        FinishWarmUp()
                    '    Else
                    '        Dim newPosition As Integer = CInt((100 * (tsDiff.TotalSeconds / (WUPFullTime * 60))))
                    '        If newPosition > 100 Then newPosition -= 100
                    '        CurrentMdiChild.TimeWarmUpProgressBar.Position = newPosition
                    '        CurrentMdiChild.TimeWarmUpProgressBar.PerformStep()
                    '        CurrentMdiChild.TimeWarmUpProgressBar.Update()
                    '    End If
                    'End If

                    Dim tsDiff As TimeSpan
                    tsDiff = Now.Subtract(dtStartDate)
                    If tsDiff.TotalSeconds >= (WUPFullTime * 60) AndAlso completeWupProcessFlag Then
                        FinishWarmUp(False)
                    Else
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            Dim newPosition As Integer = CInt((100 * (tsDiff.TotalSeconds / (WUPFullTime * 60))))
                            If newPosition > 100 Then newPosition -= 100
                            CurrentMdiChild.TimeWarmUpProgressBar.Position = newPosition
                            CurrentMdiChild.TimeWarmUpProgressBar.PerformStep()
                            CurrentMdiChild.TimeWarmUpProgressBar.Update()
                        End If
                    End If
                    'AG 07/10/2011

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".BsTimerWUp_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsTimerWUp_Tick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open the screen for change SETTINGS
    ''' </summary>
    ''' <remarks>DL 13/07/2011</remarks>
    Private Sub ChangeSettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeSettingsToolStripMenuItem.Click

        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button

            ' XBC 18/01/2012
            'Dim myISettings As New ISettings
            'OpenMDIChildForm(myISettings)

            'mySettings = New ISettings(Me)
            mySettings = New ISettings()
            OpenMDIChildForm(mySettings)
            ' XBC 18/01/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ChangeSettingsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeSettingsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    Private Sub QCResultSimulatorMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCResultSimulatorMenuItem.Click
        Try
            'OpenMDIChildForm(QCResultsSimulator)

            QCSimulator.AnalyzerID = AnalyzerIDAttribute
            OpenMDIChildForm(QCSimulator)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".QCResultSimulatorMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QCResultSimulatorMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    'Private Sub SoundOffButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    '    'Dim myGlobalDataTO As New GlobalDataTO
    '    Try
    '        'TR 28/10/2011 -Send the end sound command.
    '        MDIAnalyzerManager.StopAnalyzerRinging()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".StopAnalyzerRinging ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Opens the About screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 28/03/2012
    ''' </remarks>
    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Try
            myAbout = New IAboutBox()

            'Inform the required screen properties and open the screen
            myAbout.IsUser = True

            OpenMDIChildForm(myAbout)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AboutToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Opens the Instrument Info screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 28/03/2012
    ''' </remarks>
    Private Sub InstrumentInfoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InstrumentInfoToolStripMenuItem.Click
        Try
            myInstrumentInfo = New IInstrumentInfo()
            OpenMDIChildForm(myInstrumentInfo)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InstrumentInfoToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    Private Sub bsTSConnectButton_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSConnectButton.EnabledChanged
        InstrumentInfoToolStripMenuItem.Enabled = Not bsTSConnectButton.Enabled
    End Sub

    'DL 10/10/2012. B&T 838. Hide time controls and not calculate times. Begin
    'Private Sub ElapsedTimeTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ElapsedTimeTimer.Tick
    '    Try
    '        'Provitional save the work session start time
    '        If TotalSecs2 = 0 Then
    '            Dim myWSDelegate As New WorkSessionsDelegate
    '            'Dim myglobalDataTO As New GlobalDataTO
    '            Dim myglobalDataTO As GlobalDataTO
    '            myglobalDataTO = myWSDelegate.UpdateStartDateTime(Nothing, WorkSessionIDAttribute)
    '        End If
    '        'TR 11/11/2011 -Increase elapse time variable to be use on Monitor screen 
    '        LocalElapsedTime = LocalElapsedTime.AddSeconds(1)
    '        'Increment in one sec the Task Bar.
    '        IMonitor.TaskListProgressBar.Increment(1)
    '        IMonitor.ElapsedTimeTextEdit.Text = LocalElapsedTime.ToString("HH:mm:ss")

    '        If CycleMachine = 0 Then
    '            CycleMachine = GetCycleMachine()
    '        End If

    '        If WaitingCyclesTimes = 0 Then
    '            WaitingCyclesTimes = GetWaitingCycle() * CycleMachine
    '        End If


    '        If TotalSecs2 >= WaitingCyclesTimes AndAlso TotalSecs = CycleMachine - 1 Then
    '            If (TypeOf ActiveMdiChild Is IMonitor) Then
    '                IMonitor.UpdateTimes(True, False, TotalSecs2 >= WaitingCyclesTimes)
    '            End If
    '            TotalSecs = 0
    '            TotalSecs2 += 1
    '        ElseIf TotalSecs2 < WaitingCyclesTimes Then
    '            TotalSecs2 = TotalSecs2 + 1
    '            TotalSecs = 0
    '        Else
    '            TotalSecs = TotalSecs + 1
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".ElapsedTimeTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try

    'End Sub
    'DL 10/10/2012. B&T 838. End

    Private Sub TestPrintSortingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestPrintSortingToolStripMenuItem.Click
        'mySortingTest = New ISortingTestsAux(Me)
        mySortingTest = New ISortingTestsAux()
        OpenMDIChildForm(mySortingTest)
    End Sub

    'RH 09/03/2012
    Private Sub ChangeUserToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeUserToolStripMenuItem.Click
        'If CloseActiveMdiChild() Then
        '    Using myLoginForm As New IAx00Login(True)
        '        myLoginForm.ShowDialog()
        '    End Using

        '    OpenMonitorForm()
        '    IMonitor.RefreshAlarmsGlobes(Nothing)
        'End If

        'RH 30/03/2012
        myLogin = New IAx00Login(True)
        OpenMDIChildForm(myLogin)
    End Sub

    ''' <summary>
    ''' Opens the screen that allow to design Report Templates
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 15/12/2011
    ''' </remarks>
    Private Sub HeadPToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HeadPToolStripMenuItem.Click
        Try
            myReport = New IBandTemplateReport()
            OpenMDIChildForm(myReport)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".HeadPToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    Private Sub IAx00MainMDI_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Created AndAlso WindowState = FormWindowState.Normal Then
            BsUpdateGlobesTimer.Enabled = True
        End If
    End Sub

    Private Sub BsUpdateGlobesTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsUpdateGlobesTimer.Tick
        BsUpdateGlobesTimer.Enabled = False
        IMonitor.ShowActiveAlerts(False)
    End Sub

    'SGM 10/04/2012
    Private Sub UtilitiesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UtilitiesToolStripMenuItem.Click
        Try
            'If Me.MDIAnalyzerManager.Connected Then
            '    If Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then

            '        Dim isShutDown As Boolean = (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "PAUSED")
            '        If Not isShutDown Then
            '            If Not Me.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp Then
            '                If Not Me.MDIAnalyzerManager.ISE_Manager.IsISEInitiating And Me.MDIAnalyzerManager.ISE_Manager.IsISEInitializationDone Then
            '                    'when initialization finished
            '                    Me.ISEUtilitiesToolStripMenuItem.Enabled = Me.MDIAnalyzerManager.ISE_Manager.IsISESwitchON

            '                ElseIf Me.MDIAnalyzerManager.ISE_Manager.IsISESwitchON And Not Me.MDIAnalyzerManager.ISE_Manager.IsISECommsOk And Not Me.MDIAnalyzerManager.ISE_Manager.IsISEInitiatedOK And Me.MDIAnalyzerManager.ISE_Manager.IsISEInitializationDone Then
            '                    'when switch on but initialization is pending
            '                    Me.ISEUtilitiesToolStripMenuItem.Enabled = True
            '                Else
            '                    Me.ISEUtilitiesToolStripMenuItem.Enabled = False
            '                End If

            '                'ConditioningToolStripMenuItem.Enabled = True                  'DL 06/06/2012
            '                'ChangeReactionsRotorToolStripMenuItem.Enabled = True          'DL 06/06/2012
            '            Else
            '                ISEUtilitiesToolStripMenuItem.Enabled = False
            '                'ConditioningToolStripMenuItem.Enabled = False                 'DL 06/06/2012
            '                'ChangeReactionsRotorToolStripMenuItem.Enabled = False         'DL 06/06/2012
            '            End If
            '        Else
            '            ISEUtilitiesToolStripMenuItem.Enabled = False
            '            'ConditioningToolStripMenuItem.Enabled = False                     'DL 06/06/2012
            '            'ChangeReactionsRotorToolStripMenuItem.Enabled = False             'DL 06/06/2012
            '        End If
            '    Else
            '        ISEUtilitiesToolStripMenuItem.Enabled = False
            '        'ConditioningToolStripMenuItem.Enabled = False                     'DL 06/06/2012
            '        'ChangeReactionsRotorToolStripMenuItem.Enabled = False             'DL 06/06/2012
            '    End If

            'ElseIf MDIAnalyzerManager.ISE_Manager.IsAnalyzerDisconnected Then
            '    ISEUtilitiesToolStripMenuItem.Enabled = True
            '    'ConditioningToolStripMenuItem.Enabled = True                     'DL 06/06/2012
            '    'ChangeReactionsRotorToolStripMenuItem.Enabled = True             'DL 06/06/2012
            'Else
            '    ISEUtilitiesToolStripMenuItem.Enabled = False
            '    'ConditioningToolStripMenuItem.Enabled = False                     'DL 06/06/2012
            '    'ChangeReactionsRotorToolStripMenuItem.Enabled = False             'DL 06/06/2012
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".UtilitiesToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UtilitiesToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ISEUtilitiesToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button

            ' XB 27/11/2013 - Inform to MDI that this screen is building - Task #1303
            LoadingScreen()
            EnableButtonAndMenus(True)

            Application.DoEvents()

            ' XBC 12/09/2012 - Correction : Set ISE Manager Worksession ID
            If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                MDIAnalyzerManager.ISE_Manager.WorkSessionID = WorkSessionIDAttribute
            End If

            'myISEUtilities = New IISEUtilities(Me)
            myISEUtilities = New IISEUtilities(Me)

            ' XBC 03/08/2012 - initialize to protect
            Me.ShutDownisPending = False
            Me.StartSessionisPending = False

            'Inform the required screen properties and open the screen
            myISEUtilities.ActiveWorkSession = ActiveWorkSession
            myISEUtilities.ActiveWSStatus = ActiveStatus

            OpenMDIChildForm(myISEUtilities)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ISEUtilitiesToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ISEUtilitiesToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Open Acrobat In One Button Click Using VB.Net { No API Or Other Ocx }
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 23/03/2012 
    ''' </remarks>
    Private Sub UserManualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserManualToolStripMenuItem.Click

        Try
            ''opc1
            '' Dim files() As String = Directory.GetFiles("C:\", "AcroRd32.exe")

            '' For Each FileName As String In Directory.GetDirectories("C:\")
            ''MessageBox.Show(FileName)

            ''Console.Write(FileName)
            ''Next

            ''For Each d In Directory.GetDirectories(sDir)
            ''    For Each f In Directory.GetFiles(d, txtFile.Text)
            ''        lstFilesFound.Items.Add(f)
            ''    Next
            ''    DirSearch(d)
            ''Next


            'DL 15/11/2012. Begin


            ' Get the Help File Path and Name.

            Dim myHelpPath As String = ""
            Dim myHelpFilesSettingDelegate As New HelpFilesSettingDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHelpFilesSettingDS As New HelpFilesSettingDS
            myGlobalDataTO = myHelpFilesSettingDelegate.Read(Nothing, HELP_FILE_TYPE.MANUAL_USR, CurrentLanguageAttribute)

            If Not myGlobalDataTO.HasError Then
                myHelpFilesSettingDS = DirectCast(myGlobalDataTO.SetDatos, HelpFilesSettingDS)
                If myHelpFilesSettingDS.tfmwHelpFilesSetting.Count = 1 Then
                    myHelpPath = System.AppDomain.CurrentDomain.BaseDirectory() & _
                                    myHelpFilesSettingDS.tfmwHelpFilesSetting(0).HelpFileName

                    System.Diagnostics.Process.Start(myHelpPath) 'System.AppDomain.CurrentDomain.BaseDirectory() & "HelpFiles\Manual de usuario-SE.pdf")

                Else
                    ShowMessage("Warning", GlobalEnumerates.Messages.MANUAL_MISSING.ToString)
                End If
            End If

            'DL 15/11/2012. End

            'opc3
            'Dim proc As New Process()

            'With proc.StartInfo
            '.Arguments = System.AppDomain.CurrentDomain.BaseDirectory() & "\HelpFiles\Manual de usuario-SE.pdf"
            '.UseShellExecute = True
            '.WindowStyle = ProcessWindowStyle.Maximized
            '.WorkingDirectory = "C:\Program Files\Adobe\Reader 8.0\Reader\" '<----- Set Acrobat Install Path
            '.FileName = "AcroRd32.exe" '<----- Set Acrobat Exe Name
            'End With

            'proc.Start()
            'proc.Close()
            'proc.Dispose()




            'WebBrowser1.Navigate(System.AppDomain.CurrentDomain.BaseDirectory() & "HelpFiles\Manual de usuario-SE.pdf")



        Catch ex As Exception

            If DirectCast(ex, System.ComponentModel.Win32Exception).ErrorCode = -2147467259 Then
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                CreateLogActivity(ex.Message, Name & ".UserManualToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage("UserManualToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_ERROR_READER", CurrentLanguageAttribute))
            Else
                CreateLogActivity(ex.Message, Name & ".UserManualToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage("UserManualToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            End If

        End Try

        'AxAcroPDF1.LoadFile("C:\MI DOCUMENTACION\Manual de usuario-SE.pdf")
        'OpenMDIChildForm(IHelp)

        'Using myfrm As New IHelp()
        '    myfrm.HelpFile = System.AppDomain.CurrentDomain.BaseDirectory() & "\HelpFiles\Manual de usuario-SE.pdf"
        '    myfrm.ShowDialog()
        'End Using

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>JV + AG revision 18/10/2013 New Button Play/Pause/Continue event task # 1341</remarks>
    Private Sub bsTSMultiFunctionSessionButton_Click(sender As Object, e As EventArgs) Handles bsTSMultiFunctionSessionButton.Click
        Try
            'Dim myLogAcciones As New ApplicationLogManager()
            If showSTARTWSiconFlag Then
                CreateLogActivity("Btn Start WS", Me.Name & ".bsTSMultiFunctionSessionButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
                PlaySessionActions()
            ElseIf showPAUSEWSiconFlag Then
                CreateLogActivity("Btn Pause WS", Me.Name & ".bsTSMultiFunctionSessionButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
                'JV + AG #1391 26/11/2013
                Dim resultData As New GlobalDataTO
                Dim myExDlg As New ExecutionsDelegate
                resultData = myExDlg.ExistCriticalPauseTests(Nothing, AnalyzerIDAttribute, AnalyzerModelAttribute, WorkSessionIDAttribute)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    If CType(resultData.SetDatos, Boolean) AndAlso _
                        BSCustomMessageBox.Show(Me, infoCritical, My.Application.Info.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, _
                                                "", waitButton, tooltipPause) = DialogResult.Yes Then Return
                End If
                'JV + AG #1391 26/11/2013
                PauseSessionActions()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSMultiFunctionSessionButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSMultiFunctionSessionButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pForceActionIcon"></param>
    ''' <remarks>
    ''' Created by  JV + AG revision 18/10/2013 New Button Play/Pause/Continue image and visibility task # 1341
    ''' Modified by AG 30/10/2013 - in some cases we need set a specific icon without buttons activation (for example ResetWS) so use the optional parameter
    '''             XB 30/10/2013 - Pause button must disable when user press pause button while Auto WS process or auto ISE maintenance is processing
    ''' Modified by AG 05/11/2013 - modified during validation of task #1375
    ''' </remarks>
    Private Sub ChangeStatusImageMultipleSessionButton(Optional ByVal pForceActionIcon As WorkSessionUserActions = WorkSessionUserActions.NO_WS)
        Try
            multisessionButtonEnableValue = (showPAUSEWSiconFlag Or showSTARTWSiconFlag)
            If multisessionButtonEnableValue Then
                'Change image
                If showPAUSEWSiconFlag Then
                    bsTSMultiFunctionSessionButton.Image = imagePause
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPause
                Else
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay
                End If

                'Set enabled true
                'AG 05/11/2013 - Task #1375 - change business and take into account the automatic WS creation with STARTWS icon. Change 'XB 30/10/2013' failed
                'If Not pausingAutomateProcessFlag And Not StartSessionisPending AndAlso Not StartingSession Then  ' XB 30/10/2013
                Dim myLocalStartingSession As Boolean = StartingSession

                'In automatic creation WS process (StandBy) the pause button must be enabled except just before go to running
                'In automatic creation WS process (running paused) the pause button must be enabled except just before leave the paused mode
                If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                    myLocalStartingSession = False
                    If automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning Then myLocalStartingSession = True
                End If
                If Not pausingAutomateProcessFlag And Not StartSessionisPending AndAlso Not myLocalStartingSession Then
                    'AG 05/11/2013

                    If Not MDIAnalyzerManager Is Nothing AndAlso _
                       MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then ' XB 07/11/2013
                        ' Nothing to do 
                    Else
                        bsTSMultiFunctionSessionButton.Enabled = True
                    End If
                Else
                    bsTSMultiFunctionSessionButton.Enabled = False
                End If
            Else
                'No change image. Only Set enabled false
                bsTSMultiFunctionSessionButton.Enabled = False

                'AG 22/10/2013 - Exception, change icon when
                If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                    'STARTWS icon
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay
                    'AG 30/10/2013
                ElseIf pForceActionIcon = WorkSessionUserActions.START_WS Then
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay

                ElseIf pForceActionIcon = WorkSessionUserActions.PAUSE_WS Then
                    bsTSMultiFunctionSessionButton.Image = imagePause
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPause
                    'AG 30/10/2013
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ChangeStatusImageMultipleSessionButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeStatusImageMultipleSessionButton ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>JV + AG revision 21/10/2013 task # 1341</remarks>
    Private Sub PlaySessionActions()
        Try
            'JV + AG revision 18/10/2013 task # 1341
            showPAUSEWSiconFlag = False
            ChangeStatusImageMultipleSessionButton()
            'JV + AG revision 18/10/2013 task # 1341
            SetHQProcessByUserFlag(False) 'AG 30/07/2013 - on press START WS button reset the flag for host query from MDI
            'Normal business
            MyClass.InitiateStartSession()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PlaySessionActions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PlaySessionActions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>JV + AG revision 21/10/2013 task # 1341</remarks>
    Private Sub PauseSessionActions()
        Try
            If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                'JV + AG revision 18/10/2013 task # 1341
                SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcessPaused) ' XB 30/10/2013 - emplaced before ChangeStatusImageMultipleSessionButton
                'bsTSPauseSessionButton.Enabled = False
                'JV + AG revision 18/10/2013 task # 1341
                showSTARTWSiconFlag = False
                ChangeStatusImageMultipleSessionButton()
            Else

                If (Not MDIAnalyzerManager Is Nothing) Then

                    'Disable all buttons until Ax00 accepts the new instruction
                    SetActionButtonsEnableProperty(False)

                    Dim myGlobal As New GlobalDataTO
                    MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                    MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute

                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS"
                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""

                    'TR 27/10/2011 -Stop Sound.
                    MDIAnalyzerManager.StopAnalyzerRinging()

                    'TR 02/12/2011 -Stop the elapsed time timer
                    ElapsedTimeTimer.Stop()
                    UserPauseWSAttribute = True 'TR 21/05/2012 -Set the flag user pause the work session to true after time stop.

                    ' XB 15/10/2013
                    'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE, True)

                    Dim activateButtonsFlag As Boolean = False
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        If (Not MDIAnalyzerManager.Connected) Then
                            MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = ""
                            'AG 14/10/2011 - Error Comms message is shown in ManageReceptionEvent
                            'Dim myAuxGlobal As New GlobalDataTO() With {.ErrorCode = "ERROR_COMM"}
                            'ShowMessage("Warning", myAuxGlobal.ErrorCode)
                            activateButtonsFlag = True
                        End If

                    ElseIf (myGlobal.HasError) Then
                        ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                        activateButtonsFlag = True
                    End If

                    If activateButtonsFlag Then
                        SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
                    Else
                        SetWorkSessionUserCommandFlags(WorkSessionUserActions.PAUSE_WS)
                    End If

                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PauseSessionActions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PauseSessionActions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' End session actions. Refactored method to encapsulate all the End Session running + Standby instructions
    ''' </summary>
    ''' <remarks>Refactored by CF - For v3.0.0 21/10/2013</remarks>
    Private Sub EndRunningSessionActions()
        Try
            If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                'JV + AG revision 18/10/2013 task # 1341
                SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcessPaused)   ' XB 30/10/2013 - emplaced before ChangeStatusImageMultipleSessionButton
                'bsTSPauseSessionButton.Enabled = False
                'JV + AG revision 18/10/2013 task # 1341
                showSTARTWSiconFlag = False
                ChangeStatusImageMultipleSessionButton()
            Else

                If (Not MDIAnalyzerManager Is Nothing) Then

                    'Disable all buttons until Ax00 accepts the new instruction
                    SetActionButtonsEnableProperty(False)

                    Dim myGlobal As New GlobalDataTO
                    MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                    MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute

                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess) = "INPROCESS"
                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""

                    'TR 27/10/2011 -Stop Sound.
                    MDIAnalyzerManager.StopAnalyzerRinging()

                    'TR 02/12/2011 -Stop the elapsed time timer
                    ElapsedTimeTimer.Stop()
                    UserPauseWSAttribute = True 'TR 21/05/2012 -Set the flag user pause the work session to true after time stop.

                    'AG 30/10/2013 - inform new parameter to AnalyzerManager -> END instruction required for user action!!
                    'I prefer not use the method PlaySessionActions because this method contains more functionality. For example
                    'the autoWS creation using LIS
                    Dim stopRequestedByUser As Boolean = True

                    ' XB 15/10/2013
                    'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True, Nothing, stopRequestedByUser)

                    Dim activateButtonsFlag As Boolean = False
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        If (Not MDIAnalyzerManager.Connected) Then
                            MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess) = ""
                            'AG 14/10/2011 - Error Comms message is shown in ManageReceptionEvent
                            'Dim myAuxGlobal As New GlobalDataTO() With {.ErrorCode = "ERROR_COMM"}
                            'ShowMessage("Warning", myAuxGlobal.ErrorCode)
                            activateButtonsFlag = True
                        End If

                    ElseIf (myGlobal.HasError) Then
                        ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                        activateButtonsFlag = True
                    End If

                    If activateButtonsFlag Then
                        SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
                    Else
                        SetWorkSessionUserCommandFlags(WorkSessionUserActions.PAUSE_WS)
                    End If

                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".EndRunningSessionActions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EndRunningSessionActions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Action Buttons Events"

    ''' <summary>
    ''' Send to the instrument the Connect instruction
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 - Tested: OK
    ''' Modified by: SA 02/04/2012 - Before execute Connect process, disable all buttons and menus to avoid errors
    ''' AG 07/01/2014 - BT #1445 if reconnection in running while the process for automatic WS creation with LIS is in process ... stop it!!
    '''                 (if the app has been re-started it is not necessary because there are volatile variables)
    ''' </remarks>
    Private Sub bsTSConnectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSConnectButton.Click
        Try
            CreateLogActivity("Btn Connect", Me.Name & ".bsTSConnectButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013

            'AG 07/01/2014 - BT #1445
            If Not MDIAnalyzerManager Is Nothing AndAlso MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                InitializeAutoWSFlags()
            End If
            'AG 07/01/2014

            EnableButtonAndMenus(False)
            Connect()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSConnectButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSConnectButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Send to the instrument the START instrument process: StandBy + info + Wash + Alight
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 - Tested: ok
    ''' </remarks>
    Private Sub bsTSStartInstrumentButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSStartInstrumentButton.Click
        Try
            Dim myGlobal As New GlobalDataTO

            CreateLogActivity("Btn Instrument", Me.Name & ".bsTSStartInstrumentButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013

            ' XBC 06/07/2012
            isStartInstrumentActivating = True

            'AG 12/09/2011 - move the DL 09/09/2011 code into InitializeStartInstrument method, and also move the code situation

            'Show message (one time) recommend change reactions rotor if needed
            '
            'AG 23/05/2012 - Do not inform "Change reactions rotor recommend" because user can not use the change reactions rotor utility"
            'Dim AnalReactionsRotor As New AnalyzerReactionsRotorDelegate
            'myGlobal = AnalReactionsRotor.ChangeReactionsRotorRecommended(Nothing, AnalyzerIDAttribute, AnalyzerModelAttribute)
            'If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
            '    Dim messageID As String = ""
            '    messageID = CType(myGlobal.SetDatos, String)
            '    If messageID <> "" And ChangeReactionsRotorRecommendedWarning = 0 Then
            '        ChangeReactionsRotorRecommendedWarning = 1
            '        ShowMessage("Warning", messageID)
            '    Else
            '        ChangeReactionsRotorRecommendedWarning = 0
            '    End If
            'End If
            ChangeReactionsRotorRecommendedWarning = 0
            'AG 23/05/2012

            If ChangeReactionsRotorRecommendedWarning = 0 Then 'If no message shown then send instruction
                If Not MDIAnalyzerManager Is Nothing Then
                    MDIAnalyzerManager.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound

                    Dim activateButtonsFlag As Boolean = False
                    Dim myCurrentAlarms As List(Of GlobalEnumerates.Alarms)
                    myCurrentAlarms = MDIAnalyzerManager.Alarms

                    'Disable all buttons until Ax00 accept the new instruction
                    SetActionButtonsEnableProperty(False)

                    'DL 17/05/2012
                    SetEnableMainTab(False, True) 'DL 26/03/2012 test
                    ChangeErrorStatusLabel(True) 'AG 18/05/2012 - Clear error provider 
                    MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = True
                    'DL 17/05/2012

                    'AG 20/02/2012 - The following condition is added into a public method
                    ''AG 30/09/2011 - before continue warm up check for current alarms
                    'If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso (myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                    '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                    '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) _
                    '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR)) Then
                    If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso MDIAnalyzerManager.ExistBottleAlarms Then
                        'AG 20/02/2012

                        'Show message
                        ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                        activateButtonsFlag = True

                        'AG 12/03/2012
                        'NOTE: This button is disable while the REACT_ROTOR_MISSING alarm exist. In other way we have to implement a new elseIf case
                        'Continue the process on the aborted task (Washing)

                    ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED", False) = 0 Then
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "" 'Reset flag
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "" 'Reset flag

                        'Do nothing
                        'When Sw receives the ANSINFO if no bottle alarm the proper instruction will be sent
                        'Continue the process on the aborted task (Adjust base light)

                    ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED", False) = 0 Then
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "" 'Reset flag

                        'Do nothing
                        'When Sw receives the ANSINFO if no bottle alarm the proper instruction will be sent
                        'If no previous start instrument canceled ... start the whole process: sent the standby

                    Else
                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 'clear sensor
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "" 'Reset flag
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "" 'Reset flag
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "" 'Reset flag
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                        myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)

                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            If Not MDIAnalyzerManager.Connected Then
                                MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = ""
                                'AG 14/10/2011 - Error Comms message is shown in ManageReceptionEvent
                                'Dim myAuxGlobal As New GlobalDataTO() With {.ErrorCode = "ERROR_COMM"}
                                'ShowMessage("Warning", myAuxGlobal.ErrorCode)
                                activateButtonsFlag = True

                            Else
                                ShowStatus(Messages.STARTING_INSTRUMENT) 'RH 21/03/2012

                                'Start instrument instruction send OK (initialize wup UI flags)
                                myGlobal = InitializeStartInstrument()
                            End If

                        ElseIf myGlobal.HasError Then
                            ShowMessage("Error", myGlobal.ErrorMessage)
                            activateButtonsFlag = True
                        End If

                        If activateButtonsFlag Then
                            SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
                            SetEnableMainTab(True) 'DL 26/03/2012 test
                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSStartInstrumentButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSStartInstrumentButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            ' XBC 06/07/2012
            isStartInstrumentActivating = False
        End Try
    End Sub

    ''' <summary>
    ''' Shut down the instrument
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  AG 02/11/2010 - Tested: ok
    ''' </remarks>
    Private Sub bsTSShutdownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSShutdownButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            CreateLogActivity("Btn ShutDown", Me.Name & ".bsTSShutdownButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If Not MDIAnalyzerManager Is Nothing Then
                MDIAnalyzerManager.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound

                Dim userAnswer As DialogResult = DialogResult.No 'DL 03/05/2012 Bug Tracking (533) Ask for ShutDown confirmation

                If Me.NotDisplayShutDownConfirmMsg Then  ' XBC 26/06/2012 - ISE final self-washing
                    userAnswer = DialogResult.Yes
                Else
                    If flagExitWithShutDown Then  'DL 01/06/2012
                        userAnswer = ShowMessage(Me.Name & ".bsTSShutdownButton_Click", _
                                                 GlobalEnumerates.Messages.CONFIRM_SHUTDOWN_EXIT.ToString) 'DL 04/06/2012

                    Else 'DL 01/06/2012
                        userAnswer = ShowMessage(Me.Name & ".bsTSShutdownButton_Click", _
                                                 GlobalEnumerates.Messages.CONFIRM_SHUTDOWN.ToString) 'DL 03/05/2012 Bug Tracking (533) 

                    End If 'DL 01/06/2012
                End If

                'Dim userAnswer As DialogResult = DialogResult.No 'DL 03/05/2012 Bug Tracking (533) Ask for ShutDown confirmation
                'userAnswer = ShowMessage(Me.Name & ".bsTSShutdownButton_Click", GlobalEnumerates.Messages.CONFIRM_SHUTDOWN.ToString) 'DL 03/05/2012 Bug Tracking (533) 
                If userAnswer = DialogResult.Yes Then 'DL 03/05/2012 Bug Tracking (533) 

                    ' XBC 03/10/2012
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                    System.Threading.Thread.Sleep(500)
                    ' XBC 03/10/2012

                    NotDisplayErrorCommsMsg = True ' XBC 19/06/2012

                    'AG 29/09/2011 - If bootle alarms show message and not start the shutdown process
                    Dim activateButtonsFlag As Boolean = False
                    Dim myCurrentAlarms As List(Of GlobalEnumerates.Alarms)
                    myCurrentAlarms = MDIAnalyzerManager.Alarms

                    'DL 10/10/2012. Begin !!!
                    'To press Shutdown button and accept by user, call to FinishWarmUp() 
                    If (Not ActiveMdiChild Is Nothing) AndAlso _
                       (TypeOf ActiveMdiChild Is IMonitor) AndAlso _
                       (IMonitor.bsEndWarmUp.Enabled = True) Then

                        FinishWarmUp(False)

                    End If
                    'DL 10/10/2012. End

                    SetActionButtonsEnableProperty(False) 'Disable all buttons until Ax00 accept the new instruction

                    ShowStatus(Messages.SHUTDOWN) 'RH 21/03/2012
                    SetEnableMainTab(False) 'RH 27/03/2012
                    ChangeErrorStatusLabel(True) 'AG 18/05/2012 - Clear error provider 

                    ' XBC 26/06/2012 - ISE final self-washing
                    If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                        ' XBC 03/10/2012
                        'If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then

                        ' XBC 08/11/2012 - No send ISE Cleans when It is configured as Log Term deactivated
                        'If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled And MDIAnalyzerManager.ISE_Manager.IsISEInitiatedOK Then
                        If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
                           MDIAnalyzerManager.ISE_Manager.IsISEInitiatedOK AndAlso _
                           Not MDIAnalyzerManager.ISE_Manager.IsLongTermDeactivation Then
                            ' XBC 08/11/2012

                            ' XBC 03/10/2012
                            ' Check if ISE cycle clean is required
                            myGlobal = MDIAnalyzerManager.ISE_Manager.CheckCleanIsNeeded
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                If (CType(myGlobal.SetDatos, Boolean)) Then

                                    Dim paramsDlg As New SwParametersDelegate
                                    Dim paramsDS As New ParametersDS
                                    Dim myTubePos As Integer
                                    myGlobal = paramsDlg.GetParameterByAnalyzer(Nothing, AnalyzerIDAttribute, SwParameters.ISE_UTIL_WASHSOL_POSITION.ToString, True)
                                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                        paramsDS = CType(myGlobal.SetDatos, ParametersDS)
                                        If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                            myTubePos = CInt(paramsDS.tfmwSwParameters(0).ValueNumeric)

                                            Dim TextPar As New List(Of String)
                                            TextPar.Add(myTubePos.ToString)

                                            Dim myMsgList As New List(Of String)
                                            myMsgList.Add(Messages.ISE_CLEAN_REQUIRED.ToString)
                                            If ShowMultipleMessage("Information", myMsgList, Nothing, TextPar) = Windows.Forms.DialogResult.Yes Then
                                                myGlobal = MDIAnalyzerManager.ISE_Manager.DoCleaning(myTubePos)
                                                If Not myGlobal.HasError Then
                                                    Me.ShutDownisPending = True
                                                    ' waiting until ISE CLEAN operation is performed
                                                    ' then will be back to continue Shut Down operation
                                                    Exit Sub
                                                End If
                                            End If

                                        End If
                                    End If
                                End If
                            End If

                            If myGlobal.HasError Then
                                ShowMessage("Error", GlobalEnumerates.Messages.ISE_CLEAN_WRONG.ToString)
                                Exit Sub
                            End If
                        End If
                    End If
                    ' XBC 26/06/2012

                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) = 0 AndAlso _
                       (String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED", False) = 0 OrElse _
                        String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED", False) = 0) Then

                        'If start instrument has been canceled send directly the SLEEP instruction without WASH
                        'Do not treat myGlobal.HasError
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = ""
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS" 'AG 11/04/2012
                        myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                        If MDIAnalyzerManager.Connected Then
                            myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True)
                        End If

                        'AG 20/02/2012 - The following condition is added into a public method
                        'ElseIf myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) _
                        '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) _
                        '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) _
                        '   OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse myCurrentAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) Then
                    ElseIf MDIAnalyzerManager.ExistBottleAlarms Then
                        'Show message
                        ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "PAUSED"
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED"
                        activateButtonsFlag = True

                        'AG 12/03/2012
                        'NOTE: This button is disable while the REACT_ROTOR_MISSING alarm exist. In other way we have to implement a new elseIf case

                    Else
                        'No bottle alarms the perform click button business
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = ""
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS"
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                        'AG 29/09/2011 - Send a INFO instruction (Deactivate ANSINF instructions) during ShutDown
                        'Do not treat myGlobal.HasError

                        ' XBC 03/10/2012
                        'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)

                        If MDIAnalyzerManager.Connected Then
                            myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                        End If

                    End If

                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        If Not MDIAnalyzerManager.Connected Then
                            MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                            activateButtonsFlag = True
                        End If

                    ElseIf myGlobal.HasError Then
                        ShowMessage("Error", myGlobal.ErrorMessage)
                        activateButtonsFlag = True
                    End If

                    If activateButtonsFlag Then
                        ' XBC 03/10/2012
                        myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                        SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
                        ShowStatus(Messages.STANDBY) 'RH 21/03/2012
                        SetEnableMainTab(True) 'RH 27/03/2012
                    End If

                End If 'DL 03/05/2012 Bug Tracking (533)

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSShutdownButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSShutdownButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Change bottles confirmation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 21/02/2012</remarks>
    Private Sub bsTSChangeBottlesConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSChangeBottlesConfirm.Click
        Try
            CreateLogActivity("Btn ChangeBottles", Me.Name & ".bsTSChangeBottlesConfirm_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If (Not MDIAnalyzerManager Is Nothing) Then
                MDIAnalyzerManager.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound

                Dim myGlobal As New GlobalDataTO
                Dim activateButtonsFlag As Boolean = False

                SetActionButtonsEnableProperty(False) 'Disable all buttons until Ax00 accept the new instruction
                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS) = 0 'clear sensor
                MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process) = "INPROCESS"

                'Activate the ANSINF reception (the next ansinf received will be used to confirm the bottles change)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    If Not MDIAnalyzerManager.Connected Then
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CHANGE_BOTTLES_Process) = ""
                        activateButtonsFlag = True
                    End If

                ElseIf myGlobal.HasError Then
                    ShowMessage("Error", myGlobal.ErrorMessage)
                    activateButtonsFlag = True
                End If

                If activateButtonsFlag Then
                    SetActionButtonsEnableProperty(True)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSChangeBottlesConfirm_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSChangeBottlesConfirm_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub


    Private Sub bsTSSoundOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSSoundOff.Click
        Try
            CreateLogActivity("Btn SoundOff", Me.Name & ".bsTSSoundOff_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If (Not MDIAnalyzerManager Is Nothing) Then
                bsTSSoundOff.Enabled = False 'TR 27/03/2012 -diasble the button.
                SetActionButtonsEnableProperty(False) 'Disable all buttons until Ax00 accept the new instruction
                MDIAnalyzerManager.StopAnalyzerRinging()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSSoundOff_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSSoundOff_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Business in click event of the START WS and CONTINUE WS buttons
    ''' Moved into a method during automate the creation of WS working with LIS
    ''' </summary>
    ''' <remarks>AG 08/07/2013</remarks>
    Private Sub InitiateStartSession()
        Try
            ' XBC 24/07/2012 - Estimated ISE Consumption by Firmware
            ' initializations
            MDIAnalyzerManager.ISE_Manager.PurgeAbyFirmware = 0
            MDIAnalyzerManager.ISE_Manager.PurgeBbyFirmware = 0
            Me.SkipCALB = False
            Me.SkipPMCL = False
            Me.SkipBMCL = False

            ' XBC 03/08/2012
            Me.StartSessionisPending = False
            Me.StartSessionisInitialPUGsent = False
            Me.StartSessionisCALBsent = False
            Me.StartSessionisPMCLsent = False
            Me.StartSessionisBMCLsent = False

            'XB 23/07/2013 - auto HQ
            'If autoWSCreationWithLISModeAttribute Then
            If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                'XB 23/07/2013
                SetAutomateProcessStatusValue(LISautomateProcessSteps.initialStatus)
            Else
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                InitializeAutoWSFlags()
            End If

            ' XB 07/11/2013
            SetActionButtonsEnableProperty(False)
            EnableButtonAndMenus(False, True)
            Application.DoEvents()
            ' XB 07/11/2013

            MyClass.StartSession()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitiateStartSession ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitiateStartSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Automate process of WS creation with LIS only with an user click (steps orderDownload + create executions)
    ''' </summary>
    ''' <remarks>
    ''' AG 09/07/2013
    ''' Modified by SG 11/07/2013
    ''' </remarks>
    Public Sub CreateAutomaticWSWithLIS()
        Try
            If automateProcessCurrentState = LISautomateProcessSteps.subProcessDownloadOrders Then
                SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessCreatinsWS)
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: Before orders downloading", "IAx00MainMDI.CreateAutomaticWSWithLIS", EventLogEntryType.Information, False)

                'Execute the orders download process

                ' XB 24/07/2013
                'bsTSOrdersDownloadButton.PerformClick()
                OrdersDownload()
                ' XB 24/07/2013

                'If we try to create executions without open the rotor positions screen it does not work properly
                'So although some blinking problems we will open the rotor positions and close it automatically
                ''Create executions table (without open rotor position screen)
                'automateProcessCurrentState = automateProcessSteps.subProcessCreateExecutions
                'CreateExecutionsProcessByISEChanges() 'Use an existing method although in this case the ISE changes has no sense!!!

            Else
                'Show warning
                Select Case automateProcessCurrentState
                    Case LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessEnterRunning)
                        'Message management have been done into the HQ monitor screen. MDI only continues with user answer
                        FinishAutomaticWSWithLIS()

                    Case LISautomateProcessSteps.ExitNoWorkOrders
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessCreatinsWS)
                        'Message management have NOT been done yet. MDI shows messanges and continues with user answer
                        Dim autoProcessUserAnswer As DialogResult = DialogResult.Yes
                        autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(5)
                        If autoProcessUserAnswer = DialogResult.Yes Then
                            'Positive case. No exceptions -- Not posible, this is the case 'ExitNoWorkOrders' and it is an exception
                        ElseIf autoProcessUserAnswer = DialogResult.OK Then 'User answers stops the automatic process
                            'Automatic process aborted
                            EnableButtonAndMenus(True, True) 'Enable buttons before update attribute!!
                            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                            InitializeAutoWSFlags()
                        Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                            SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessEnterRunning)
                            FinishAutomaticWSWithLIS()
                        End If

                End Select

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CreateAutomaticWSWithLIS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateAutomaticWSWithLIS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub


    ''' <summary>
    ''' Send to the instrument the Start (Run mode) instruction
    ''' </summary>
    ''' <param name="pSkipAutomaticWSWithLISChekings">TRUE when the automatic WS creation with LIS checkings (LIS status) - parameter set to TRUE when call this method from ManageEventReception </param>
    ''' <remarks>Created by XBC 23/07/2012
    ''' AG 10/07/2013 - adapt for use with the process of automatic WS creation with LIS
    ''' </remarks>
    Private Sub StartSession(Optional ByVal pSkipAutomaticWSWithLISChekings As Boolean = False)
        Try
            StartingSession = True    ' XB 31/10/2013

            'Stop Analyzer sound
            MDIAnalyzerManager.StopAnalyzerRinging()

            'TR 29/05/2012 -Reset time counter
            TotalSecs = 0
            TotalSecs2 = 0
            'TR 29/05/2012 -END.

            ' XBC 12/04/2013
            Dim WSExecutionsAlreadyCreated As Boolean = False

            'Show message recommending change of Reactions Rotor if needed; this warning message is shown only the first time the 
            'START/CONTINUE SESSION Button is clicked
            Dim myGlobal As New GlobalDataTO
            Dim myAnalizerReactionsRotor As New AnalyzerReactionsRotorDelegate


            ' XBC 03/08/2012 
            If Not Me.StartSessionisPending Then

                'AG 01/08/2013
                If pSkipAutomaticWSWithLISChekings AndAlso ChangeReactionsRotorRecommendedWarning = 0 Then
                    ChangeReactionsRotorRecommendedWarning = 1 'Skip this warning when call automatically in enter running process
                End If
                'AG 01/08/2013

                'AG 25/10/2013
                If MDIAnalyzerManager.AllowScanInRunning Then
                    ChangeReactionsRotorRecommendedWarning = 1 'Skip this warning when Analyzer is on Pause mode
                End If
                'AG 25/10/2013

                myGlobal = myAnalizerReactionsRotor.ChangeReactionsRotorRecommended(Nothing, AnalyzerIDAttribute, AnalyzerModelAttribute)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim messageID As String = CType(myGlobal.SetDatos, String)

                    If (String.Compare(messageID, String.Empty, False) <> 0 AndAlso ChangeReactionsRotorRecommendedWarning = 0) Then
                        'Message is shown and the WS Start/Continue is stopped
                        ChangeReactionsRotorRecommendedWarning = 1
                        ShowMessage(Me.Name & ".bsTSContinueSessionButton_Click", messageID)
                        Application.DoEvents()
                        Cursor = Cursors.Default
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 stop the process if warning message is shown
                        InitializeAutoWSFlags()
                    Else
                        ChangeReactionsRotorRecommendedWarning = 0
                    End If
                Else
                    'Error verifying if the Reactions Rotor needs to be changed; shown it
                    ShowMessage(Me.Name & ".bsTSContinueSessionButton_Click", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                End If

            End If

            If (Not myGlobal.HasError) Then
                'The WS START/CONTINUE process continues only if:
                '   ** It is not needed to change the Reactions Rotor 
                '   ** It is needed to change the Reactions Rotor but the warning message was shown in a previous button click
                If (ChangeReactionsRotorRecommendedWarning = 0) Then
                    Dim userAnswer As DialogResult = DialogResult.Yes

                    ''Verify if the WorkSession can be fully executed with the current level of Washing Solutions and High Contamination Waste bottles
                    'Dim ex_delg As New ExecutionsDelegate
                    'myGlobal = ex_delg.CalculateIfBottleLevelsAreSufficientForWS(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, _
                    '                                                             MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION), _
                    '                                                             MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE))

                    'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    '    If (CType(myGlobal.SetDatos, Boolean)) Then
                    '        Volume is not OK; a warning Message is shown, but the User can choose between stop the WS Start/Continue process to solve the problem
                    '        or to continue with the execution despite of the problem
                    '        userAnswer = ShowMessage(Me.Name & ".bsTSContinueSessionButton_Click", GlobalEnumerates.Messages.CHANGE_BOTTLE_RECOMMEND.ToString)
                    '    End If
                    'Else
                    '    Error verifying the current level of Washing Solutions and High Contamination Waste bottles; shown it
                    '    ShowMessage(Me.Name & ".bsTSContinueSessionButton_Click", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                    'End If

                    'If there is at least a requested ISE Test pending to execute in the WorkSession, verify if ISE Module is 
                    'installed and ready to be used
                    Dim wsReadyToBeSent As Boolean = True 'This flag only can be false if user do not want to regenerate the executions in method CreateExecutionsProcess
                    If (Not myGlobal.HasError AndAlso userAnswer = DialogResult.Yes) Then

                        'AG 12/07/2013 - Exception1 (Automate WS Creation Process can not be executed because LIS connection errors)
                        Dim autoProcessUserAnswer As DialogResult = DialogResult.Yes
                        'XB 23/07/2013 - auto HQ
                        'If autoWSCreationWithLISModeAttribute AndAlso Not pSkipAutomaticWSWithLISChekings Then
                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso Not pSkipAutomaticWSWithLISChekings Then
                            'XB 23/07/2013
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: Start process", "IAx00MainMDI.StartSession", EventLogEntryType.Information, False)

                            autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(1, Nothing, 0, True) 'AG 22/07/2013 - Skip alarm sound because this message is inmediate
                            If autoProcessUserAnswer = DialogResult.Yes Then
                                ''Positive case. No execptions

                            ElseIf autoProcessUserAnswer = DialogResult.OK Then 'User answers stops the automatic process
                                'Automatic process aborted
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                InitializeAutoWSFlags()
                                userAnswer = DialogResult.No

                            Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitNoLISConnectionButGoToRunning)
                                userAnswer = DialogResult.Yes
                            End If
                        End If

                        'AG 18/7/2013 - In automate mode if current screen is rotor positions ... create executions
                        'XB 23/07/2013 - auto HQ
                        'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState = LISautomateProcessSteps.initialStatus AndAlso userAnswer = DialogResult.Yes Then
                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.initialStatus AndAlso userAnswer = DialogResult.Yes Then
                            'XB 23/07/2013
                            If (Not Me.ActiveMdiChild Is Nothing) Then
                                If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                    Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                                    If Not WSExecutionsAlreadyCreated Then
                                        'XB 23/07/2013 - auto HQ
                                        'CurrentMdiChild.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute
                                        CurrentMdiChild.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                                        'XB 23/07/2013
                                        CurrentMdiChild.OpenByAutomaticProcess = True
                                        myGlobal = CurrentMdiChild.CreateExecutionsProcess(False, wsReadyToBeSent)
                                        CurrentMdiChild.OpenByAutomaticProcess = False 'AG 19/07/2013 - after create executions reset this flag, so if the screen closes the classic business will be executed instead (only close screen)
                                    End If

                                    'Once the WS execution has been recalculated. Look for pending executions
                                    If Not myGlobal.HasError And wsReadyToBeSent Then
                                        Dim wsDelg As New WorkSessionsDelegate
                                        Dim execucionsCount As Integer = 0
                                        Dim pendingExecutionsLeft As Boolean = False
                                        myGlobal = wsDelg.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, execucionsCount, pendingExecutionsLeft)

                                        If execucionsCount = 0 OrElse Not pendingExecutionsLeft Then
                                            wsReadyToBeSent = False 'Nothing to be sent
                                        End If
                                    End If

                                End If
                            End If
                        End If
                        'AG 18/07/2013

                        'AG 12/07/2013

                        'Move the previous code to new method: xxx in order to be reuse also for the process of auto WS creation with LIS
                        If userAnswer = DialogResult.Yes Then 'AG 12/07/2013
                            'AG 15/07/2013 - The following code is commented but moved into function VerifyISEConditioningBeforeRunning
                            '                in order to be reused also for the process of automatic WS creation with LIS only by START WS click
                            'XB 23/07/2013 - auto HQ
                            'If Not autoWSCreationWithLISModeAttribute OrElse automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning Then
                            If (Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag) OrElse automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning Then
                                'XB 23/07/2013
                                myGlobal = VerifyISEConditioningBeforeRunning(WSExecutionsAlreadyCreated)
                                If Not myGlobal.HasError Then
                                    If Me.StartSessionisPending = True Then
                                        Exit Sub
                                    Else
                                        userAnswer = DirectCast(myGlobal.SetDatos, DialogResult)
                                    End If
                                End If
                            End If

                            ''Verify if there are ISE Tests requested in the WorkSession and pending of execution
                            'Dim updateExecutions As Boolean = False
                            'Dim myOrderTestsDelegate As New OrderTestsDelegate

                            'myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                            'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            '    If (CType(myGlobal.SetDatos, Boolean)) Then
                            '        If (MDIAnalyzerManager.ISE_Manager Is Nothing) OrElse (Not MDIAnalyzerManager.ISE_Manager.IsISEModuleReady) Then
                            '            'ISE Module is not available, verify is there is at least an STANDARD Test pending of execution
                            '            'to shown the proper Message 
                            '            myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                            '            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            '                If (CType(myGlobal.SetDatos, Boolean)) Then
                            '                    'ISE Module is not ready to be used but there are STD Tests pending of execution; a question Message is shown, and the User 
                            '                    'can choose between stop the WS Start/Continue process to solve the problem or to continue with the execution despite of the problem
                            '                    userAnswer = ShowMessage(Me.Name & ".bsTSStartSessionButton_Click", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)

                            '                    If (userAnswer = DialogResult.Yes) Then
                            '                        'User has selected continue WS without ISE Tests; all pending ISE Tests will be blocked
                            '                        updateExecutions = True

                            '                    End If
                            '                Else
                            '                    'ISE Module is not ready and there are not STD Tests pending of execution; an error Message is shown due to 
                            '                    'the WS can not be started
                            '                    userAnswer = DialogResult.No
                            '                    ShowMessage(Me.Name & ".bsTSStartSessionButton_Click", GlobalEnumerates.Messages.ONLY_ISE_WS_NOT_STARTED.ToString)

                            '                    'All ISE Pending Tests will be blocked
                            '                    updateExecutions = True
                            '                End If

                            '                If (updateExecutions) Then
                            '                    If (Me.ActiveMdiChild Is Nothing) OrElse (Not TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                            '                        Dim myExecutionsDelegate As New ExecutionsDelegate
                            '                        myGlobal = myExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                            '                    End If

                            '                    If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                            '                        'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                            '                        Dim myDummyUIRefresh As New UIRefreshDS
                            '                        IMonitor.UpdateWSState(myDummyUIRefresh)
                            '                    End If
                            '                End If
                            '            End If

                            '            ' XBC 26/06/2012 - ISE self-maintenance
                            '        Else
                            '            MDIAnalyzerManager.ISE_Manager.WorkSessionID = WorkSessionIDAttribute
                            '            myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                            '            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            '                If (CType(myGlobal.SetDatos, Boolean)) Then
                            '                    MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "ALL"
                            '                Else
                            '                    MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "ISE"
                            '                End If
                            '            End If

                            '            If executeSTD Then
                            '                executeSTD = False
                            '                ' Do nothing. leaving to continue with the work session execution
                            '                ' Previously existing ISE preparations have been locked
                            '            Else
                            '                ' Check ISE calibrations required
                            '                If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                            '                    If MDIAnalyzerManager.ISE_Manager.IsISEModuleReady Then
                            '                        ' Check if initial Purges are required. This is required when any calibration is required
                            '                        Dim ElectrodesCalibrationRequired As Boolean = False
                            '                        Dim PumpsCalibrationRequired As Boolean = False
                            '                        Dim BubblesCalibrationRequired As Boolean = False
                            '                        ' Check if ISE Electrodes calibration is required
                            '                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckElectrodesCalibrationIsNeeded
                            '                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            '                            ElectrodesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                            '                        End If
                            '                        ' Check if ISE Pumps calibration is required
                            '                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckPumpsCalibrationIsNeeded
                            '                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            '                            PumpsCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                            '                        End If
                            '                        ' Check if ISE Bubbles calibration is required 
                            '                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckBubbleCalibrationIsNeeded
                            '                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            '                            BubblesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                            '                        End If


                            '                        If ElectrodesCalibrationRequired Or PumpsCalibrationRequired Or BubblesCalibrationRequired Then
                            '                            If Not StartSessionisInitialPUGsent Then
                            '                                ' Execute initial Purge A and Purge B
                            '                                Me.StartSessionisInitialPUGsent = True
                            '                                myGlobal = MDIAnalyzerManager.ISE_Manager.DoMaintenanceExit() ' this function throws Purge A and Purge B 
                            '                                If Not myGlobal.HasError Then
                            '                                    MDIAnalyzerManager.ISE_Manager.PurgeAbyFirmware += 1
                            '                                    MDIAnalyzerManager.ISE_Manager.PurgeBbyFirmware += 1
                            '                                    ShowStatus(Messages.STARTING_SESSION)
                            '                                    Me.StartSessionisPending = True
                            '                                    EnableButtonAndMenus(False)
                            '                                    Cursor = Cursors.WaitCursor
                            '                                    SetEnableMainTab(False)
                            '                                    ' XBC 29/08/2012 - Disable Monitor Panel
                            '                                    'If Not Me.ActiveMdiChild Is Nothing Then
                            '                                    '    Me.ActiveMdiChild.Enabled = False
                            '                                    'End If
                            '                                    InitializeMarqueeProgreesBar()
                            '                                End If
                            '                                ' waiting until CALIBRATION operation is performed
                            '                                ' then will be back to continue Work Session
                            '                                Exit Sub

                            '                            Else
                            '                                ' Execute required Calibrations

                            '                                If ElectrodesCalibrationRequired And Not Me.SkipCALB Then
                            '                                    Me.StartSessionisCALBsent = True
                            '                                    myGlobal = MDIAnalyzerManager.ISE_Manager.DoElectrodesCalibration()
                            '                                    If myGlobal.HasError Then
                            '                                        ShowMessage("Error", GlobalEnumerates.Messages.ISE_CALB_WRONG.ToString)
                            '                                    Else
                            '                                        ShowStatus(Messages.STARTING_SESSION)
                            '                                        Me.StartSessionisPending = True
                            '                                        EnableButtonAndMenus(False)
                            '                                        Cursor = Cursors.WaitCursor
                            '                                        SetEnableMainTab(False)
                            '                                        ' XBC 29/08/2012 - Disable Monitor Panel
                            '                                        'If Not Me.ActiveMdiChild Is Nothing Then
                            '                                        '    Me.ActiveMdiChild.Enabled = False
                            '                                        'End If
                            '                                        InitializeMarqueeProgreesBar()
                            '                                    End If
                            '                                    ' waiting until CALIBRATION operation is performed
                            '                                    ' then will be back to continue Work Session
                            '                                    Exit Sub
                            '                                End If


                            '                                If PumpsCalibrationRequired And Not Me.SkipPMCL Then
                            '                                    Me.StartSessionisPMCLsent = True
                            '                                    myGlobal = MDIAnalyzerManager.ISE_Manager.DoPumpsCalibration()
                            '                                    If myGlobal.HasError Then
                            '                                        ShowMessage("Error", GlobalEnumerates.Messages.ISE_PMCL_WRONG.ToString)
                            '                                    Else
                            '                                        ShowStatus(Messages.STARTING_SESSION)
                            '                                        Me.StartSessionisPending = True
                            '                                        EnableButtonAndMenus(False)
                            '                                        Cursor = Cursors.WaitCursor
                            '                                        SetEnableMainTab(False)
                            '                                        ' XBC 29/08/2012 - Disable Monitor Panel
                            '                                        'If Not Me.ActiveMdiChild Is Nothing Then
                            '                                        '    Me.ActiveMdiChild.Enabled = False
                            '                                        'End If
                            '                                        InitializeMarqueeProgreesBar()
                            '                                    End If
                            '                                    ' waiting until CALIBRATION operation is performed
                            '                                    ' then will be back to continue Work Session
                            '                                    Exit Sub
                            '                                End If


                            '                                If BubblesCalibrationRequired And Not Me.SkipBMCL Then
                            '                                    Me.StartSessionisBMCLsent = True
                            '                                    myGlobal = MDIAnalyzerManager.ISE_Manager.DoBubblesCalibration()
                            '                                    If myGlobal.HasError Then
                            '                                        ShowMessage("Error", GlobalEnumerates.Messages.ISE_BMCL_WRONG.ToString)
                            '                                    Else
                            '                                        ShowStatus(Messages.STARTING_SESSION)
                            '                                        Me.StartSessionisPending = True
                            '                                        EnableButtonAndMenus(False)
                            '                                        Cursor = Cursors.WaitCursor
                            '                                        SetEnableMainTab(False)
                            '                                        ' XBC 29/08/2012 - Disable Monitor Panel
                            '                                        'If Not Me.ActiveMdiChild Is Nothing Then
                            '                                        '    Me.ActiveMdiChild.Enabled = False
                            '                                        'End If
                            '                                        InitializeMarqueeProgreesBar()
                            '                                    End If
                            '                                    ' waiting until CALIBRATION operation is performed
                            '                                    ' then will be back to continue Work Session
                            '                                    Exit Sub
                            '                                End If

                            '                            End If

                            '                        End If

                            '                    End If

                            '                End If
                            '            End If
                            '        End If

                            '    Else
                            '        If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                            '            MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "STD"
                            '        End If
                            '        ' XBC 26/06/2012

                            '    End If
                            'Else
                            '    'Error verifying if there are requested ISE Tests pending of execution in the active Work Session; shown it
                            '    ShowMessage(Me.Name & ".bsTSStartSessionButton_Click", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                            'End If
                            'AG 15/07/2013 - End block of code moved into function VerifyISEConditioningBeforeRunning

                        End If 'AG 12/07/2013 - UserAnswer: NO
                    End If

                    'AG 15/07/2013 - The following code is commented but moved into function VerifyISEConditioningBeforeRunning
                    '                in order to be reused also for the process of automatic WS creation with LIS only by START WS click

                    ' XBC 23/07/2012
                    'Me.StartSessionisPending = False
                    'If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                    '    If Not ActiveMdiChild Is Nothing Then
                    '        If (TypeOf ActiveMdiChild Is IMonitor) Then
                    '            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    '            MDIAnalyzerManager.ISE_Manager.WorkSessionOverallTime = CurrentMdiChild.remainingTime
                    '        End If
                    '    End If
                    'End If
                    'AG 15/07/2013 - End block of code moved into function VerifyISEConditioningBeforeRunning

                    'The WS START/CONTINUE process can continue...
                    If (Not myGlobal.HasError AndAlso userAnswer = DialogResult.Yes) Then

                        'AG 15/07/2013 - The following code is commented but moved into function VerifyISEConditioningBeforeRunning
                        '                in order to be reused also for the process of automatic WS creation with LIS only by START WS click

                        ' XBC 17/07/2012 - Estimated ISE Consumption by Firmware
                        'If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then

                        '    'SGM 01/10/2012 - If no calibration is needed set ISE preparations as PENDING
                        '    Dim myOrderTestsDelegate As New OrderTestsDelegate
                        '    myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                        '    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        '        If CBool(myGlobal.SetDatos) Then

                        '            ' XBC 12/04/2013 - Create Executions is a long time process so is need to be called on threading mode
                        '            'Dim isReady As Boolean = False
                        '            'Dim myAffectedElectrodes As List(Of String)
                        '            'myGlobal = MDIAnalyzerManager.ISE_Manager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
                        '            'If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        '            '    isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)
                        '            'End If

                        '            'Dim myExecutionDelegate As New ExecutionsDelegate
                        '            'Dim createWSInRunning As Boolean = (MDIAnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)
                        '            'myGlobal = myExecutionDelegate.CreateWSExecutions(Nothing, MDIAnalyzerManager.ActiveAnalyzer, MDIAnalyzerManager.ActiveWorkSession, _
                        '            '                                              createWSInRunning, -1, String.Empty, isReady, myAffectedElectrodes)

                        '            ''refresh WS grid
                        '            'If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                        '            '    'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                        '            '    Dim myDummyUIRefresh As New UIRefreshDS
                        '            '    IMonitor.UpdateWSState(myDummyUIRefresh)
                        '            'End If

                        '            Me.StartSessionisPending = True
                        '            myGlobal = CreateExecutionsProcessByISEChanges()
                        '            If Not myGlobal.HasError Then
                        '                WSExecutionsAlreadyCreated = True
                        '                Me.StartSessionisPending = False
                        '            End If
                        '            ' XBC 12/04/2013

                        '        End If
                        '    End If

                        '    MDIAnalyzerManager.ISE_Manager.WorkSessionIsRunning = True

                        '    'Dim myLogAcciones As New ApplicationLogManager()    ' TO COMMENT !!!
                        '    'myLogAcciones.CreateLogActivity("Update Consumptions - Update Last Date WS ISE Operation [ " & DateTime.Now.ToString & "]", "Ax00MainMDI.StartSession", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                        '    ' Update date for the ISE test executed while running
                        '    MDIAnalyzerManager.ISE_Manager.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_WS_DATE, DateTime.Now.ToString, True)
                        '    'end SGM 01/10/2012
                        'End If
                        ' XBC 17/07/2012
                        'AG 15/07/2013 - End block of code moved into function VerifyISEConditioningBeforeRunning

                        'Disable all buttons and menu options until Analyzer accepts the new instruction
                        SetActionButtonsEnableProperty(False)
                        EnableButtonAndMenus(False)


                        MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                        MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute

                        Cursor = Cursors.WaitCursor

                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0 'clear sensor
                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0 'AG 01/02/2012 - clear sensor

                        SetActionButtonsEnableProperty(False)
                        EnableButtonAndMenus(False)

                        wsReadyToBeSent = True 'This flag only can be false if user do not want to regenerate the executions in method CreateExecutionsProcess
                        If (Not Me.ActiveMdiChild Is Nothing) Then
                            'AG 10/05/2012 - If rotor positions screen is opened create WS executions before call the ManageAnalyzer methods
                            'DisabledMdiForms = Me.ActiveMdiChild
                            'Me.ActiveMdiChild.Enabled = False
                            If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                'XB 23/07/2013 - auto HQ
                                'If Not autoWSCreationWithLISModeAttribute Then 'AG 09/07/2013
                                If Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag Then 'AG 09/07/2013
                                    'XB 23/07/2013
                                    Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)

                                    ' XBC 12/04/2013
                                    'myGlobal = CurrentMdiChild.CreateExecutionsProcess(False, wsReadyToBeSent)
                                    If Not WSExecutionsAlreadyCreated Then
                                        myGlobal = CurrentMdiChild.CreateExecutionsProcess(False, wsReadyToBeSent)
                                    End If
                                    ' XBC 12/04/2013

                                    'Once the WS execution has been recalculated. Look for pending executions
                                    If Not myGlobal.HasError And wsReadyToBeSent Then
                                        Dim wsDelg As New WorkSessionsDelegate
                                        Dim execucionsCount As Integer = 0
                                        Dim pendingExecutionsLeft As Boolean = False
                                        myGlobal = wsDelg.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, execucionsCount, pendingExecutionsLeft)

                                        'AG 06/11/2013 - Task #1375 - in paused mode allow return to normal running although no more pending executions exists
                                        If Not pendingExecutionsLeft AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                                            pendingExecutionsLeft = True
                                        End If
                                        'AG 06/11/2013

                                        If execucionsCount = 0 OrElse Not pendingExecutionsLeft Then
                                            wsReadyToBeSent = False 'Nothing to be sent
                                            ShowMessage("Warning", Messages.NO_PENDING_EXECUTIONS.ToString)
                                        End If
                                    End If

                                End If 'AG 09/07/2013
                            End If

                            If Not myGlobal.HasError And wsReadyToBeSent Then
                                DisabledMdiForms = Me.ActiveMdiChild
                                Me.ActiveMdiChild.Enabled = False
                            End If
                            'AG 10/05/2012
                        End If

                        If Not myGlobal.HasError And wsReadyToBeSent Then 'AG 10/05/2012
                            readBarcodeIfConfigured = True 'AG 08/03/2013 - Enter running but ,if configured, read barcode before
                            StartEnterInRunningMode() 'AG 08/03/2013 - previous code has been move to this method because it is called from several parts

                            '¡¡¡DO NOT DELETE THIS CODE!!! --> SA 21/03/2012
                            'If (processingBeforeRunning = "2") Then
                            '    If (MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.ISE_WARNINGS) = 1) Then
                            '        'ISE conditioning failed. Do you want to continue locking all ISE Tests preparations?
                            '        If (ShowMessage("Question", GlobalEnumerates.Messages.ISE_CONDITION_FAILED.ToString) = Windows.Forms.DialogResult.Yes) Then
                            '            '1) Lock all pending ISE Tests preparations
                            '            myGlobal = ex_delg.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")

                            '            '2) Enter Running
                            '            If (Not myGlobal.HasError) Then
                            '                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RUNNING, True)
                            '                If (myGlobal.HasError) Then
                            '                    ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                            '                End If

                            '                If (Not MDIAnalyzerManager.Connected Or myGlobal.HasError) Then
                            '                    SetActionButtonsEnableProperty(True)
                            '                Else
                            '                    SetWorkSessionUserCommandFlags(WorkSessionUserActions.START_WS)

                            '                    'Set WorkSession Status to INPROCESS
                            '                    Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                            '                    myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS")
                            '                    If (Not myGlobal.HasError) Then
                            '                        WSStatusAttribute = "INPROCESS"
                            '                    Else
                            '                        'Error updating the WS Status to INPROCESS
                            '                        If (Not myGlobal.ErrorCode Is Nothing) Then ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                            '                    End If
                            '                End If
                            '            End If

                            '            'Else
                            '            '1) Abort Enter Running: Do nothing
                            '        End If
                            '        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0 'clear sensor
                            '    Else
                            '        ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                            '    End If
                            '    'AG 24/01/2012
                            'Else
                            '    SetActionButtonsEnableProperty(True)
                            '    Me.ElapsedTimeTimer.Start()
                            'End If

                        Else
                            'AG 10/05/2012
                            SetActionButtonsEnableProperty(True)
                            Cursor = Cursors.Default
                        End If

                    Else
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 - ws not started automatically
                        InitializeAutoWSFlags()
                    End If
                End If

            End If

            'AG 11/07/2013 - if any error the process stops
            If myGlobal.HasError Then
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 - ws not started automatically
                InitializeAutoWSFlags()
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".StartSession ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".StartSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 - ws not started automatically
            InitializeAutoWSFlags()
        Finally
            StartingSession = False     ' XB 31/10/2013
            'Enable buttons and menus
            If Not Me.StartSessionisPending Then EnableButtonAndMenus(True, True) 'AG 18/11/2013 - set 2on parameter to TRUE (- XBC 28/06/2012)
        End Try
    End Sub

    ''' <summary>
    ''' The business in this method is copied from StartSession and adapted into a Function instead of a Sub
    ''' This change is required when the functionality for create WS with LIS only pressing START WS button requires the ISE conditioning verification must be
    ''' just before running
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by  AG 15/07/2013
    ''' Modified by XB 22/10/2013 - PMCL check is performed always (although there are no ISE preparations on WS) - BT #1343
    ''' </remarks>
    Private Function VerifyISEConditioningBeforeRunning(ByRef pWSExecutionsCreatedFlag As Boolean) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim userAnswer As DialogResult = DialogResult.Yes

            'Verify if there are ISE Tests requested in the WorkSession and pending of execution
            Dim updateExecutions As Boolean = False
            Dim myOrderTestsDelegate As New OrderTestsDelegate
            Dim WarningAreadyShown As Boolean = False

            ' XB 22/10/2013 - BT #1343
            Dim PumpsCalibrationRequired As Boolean = False

            ' XB 20/11/2013 - No ISE warnings can appear when ISE module is not installed
            If Not MDIAnalyzerManager.ISE_Manager Is Nothing AndAlso _
               MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then

                If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    If (MDIAnalyzerManager.ISE_Manager Is Nothing) OrElse (Not MDIAnalyzerManager.ISE_Manager.IsISEModuleReady) Then
                        'ISE Module is not available, verify is there is at least an STANDARD Test pending of execution
                        'to shown the proper Message 
                        myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then

                            ' Activate buzzer
                            If Not MDIAnalyzerManager Is Nothing Then
                                MDIAnalyzerManager.StartAnalyzerRinging()
                                System.Threading.Thread.Sleep(500)
                            End If

                            If (CType(myGlobal.SetDatos, Boolean)) Then
                                'ISE Module is not ready to be used but there are STD Tests pending of execution; a question Message is shown, and the User 
                                'can choose between stop the WS Start/Continue process to solve the problem or to continue with the execution despite of the problem

                                ' XB 20/11/2013 - No ISE warnings can appear when ISE module is not installed
                                'userAnswer = ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                                    userAnswer = ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                Else
                                    userAnswer = Windows.Forms.DialogResult.Yes
                                End If
                                ' XB 20/11/2013

                                WarningAreadyShown = True

                                If (userAnswer = DialogResult.Yes) Then
                                    'User has selected continue WS without ISE Tests; all pending ISE Tests will be blocked
                                    updateExecutions = True
                                    EnableButtonAndMenus(False)
                                Else
                                    EnableButtonAndMenus(True, True)   ' XB 07/11/2013
                                    Application.DoEvents()
                                End If
                            Else
                                'ISE Module is not ready and there are not STD Tests pending of execution; an error Message is shown due to 
                                'the WS can not be started

                                ' XB 21/11/2013 - In Pause mode, WS must go on
                                'userAnswer = DialogResult.No
                                If MDIAnalyzerManager.AllowScanInRunning Then
                                    userAnswer = DialogResult.Yes
                                Else
                                    userAnswer = DialogResult.No
                                End If
                                ' XB 21/11/2013

                                ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ONLY_ISE_WS_NOT_STARTED.ToString)
                                WarningAreadyShown = True

                                'All ISE Pending Tests will be blocked
                                updateExecutions = True
                            End If

                            ' Close buzzer
                            If Not MDIAnalyzerManager Is Nothing Then
                                MDIAnalyzerManager.StopAnalyzerRinging()
                            End If

                            If (updateExecutions) Then
                                If (Me.ActiveMdiChild Is Nothing) OrElse (Not TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                    Dim myExecutionsDelegate As New ExecutionsDelegate
                                    myGlobal = myExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                                End If

                                If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                                    'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                                    Dim myDummyUIRefresh As New UIRefreshDS
                                    IMonitor.UpdateWSState(myDummyUIRefresh)
                                End If
                            End If
                        End If

                        ' ISE self-maintenance
                    Else
                        ' Check if ISE Pumps calibration is required
                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckPumpsCalibrationIsNeeded
                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            PumpsCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                        End If

                        If PumpsCalibrationRequired Then
                            If Not StartSessionisInitialPUGsent Then
                                ' Execute initial Purge A and Purge B
                                Me.StartSessionisInitialPUGsent = True
                                myGlobal = MDIAnalyzerManager.ISE_Manager.DoMaintenanceExit() ' this function throws Purge A and Purge B 
                                If Not myGlobal.HasError Then
                                    MDIAnalyzerManager.ISE_Manager.PurgeAbyFirmware += 1
                                    MDIAnalyzerManager.ISE_Manager.PurgeBbyFirmware += 1
                                    ShowStatus(Messages.STARTING_SESSION)
                                    Me.StartSessionisPending = True
                                    EnableButtonAndMenus(False)
                                    Cursor = Cursors.WaitCursor
                                    SetEnableMainTab(False)
                                    InitializeMarqueeProgreesBar()
                                End If
                                ' waiting until CALIBRATION operation is performed
                                ' then will be back to continue Work Session
                                Return myGlobal
                                Exit Function
                            End If
                        End If

                        If PumpsCalibrationRequired And Not Me.SkipPMCL Then
                            Me.StartSessionisPMCLsent = True
                            myGlobal = MDIAnalyzerManager.ISE_Manager.DoPumpsCalibration()
                            If myGlobal.HasError Then
                                ShowMessage("Error", GlobalEnumerates.Messages.ISE_PMCL_WRONG.ToString)
                            Else
                                ShowStatus(Messages.STARTING_SESSION)
                                Me.StartSessionisPending = True
                                EnableButtonAndMenus(False)
                                Cursor = Cursors.WaitCursor
                                SetEnableMainTab(False)
                                InitializeMarqueeProgreesBar()
                            End If
                            ' waiting until CALIBRATION operation is performed
                            ' then will be back to continue Work Session
                            Return myGlobal
                            Exit Function
                        End If
                    End If

                End If
                ' XB 22/10/2013

                myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    If (CType(myGlobal.SetDatos, Boolean)) Then
                        If (MDIAnalyzerManager.ISE_Manager Is Nothing) OrElse (Not MDIAnalyzerManager.ISE_Manager.IsISEModuleReady) Then
                            'ISE Module is not available, verify is there is at least an STANDARD Test pending of execution
                            'to shown the proper Message 
                            myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then

                                If Not WarningAreadyShown Then
                                    ' XB 23/07/2013 - Activate buzzer
                                    If Not MDIAnalyzerManager Is Nothing Then
                                        MDIAnalyzerManager.StartAnalyzerRinging()
                                        System.Threading.Thread.Sleep(500)
                                    End If
                                    ' XB 23/07/2013

                                    If (CType(myGlobal.SetDatos, Boolean)) Then
                                        'ISE Module is not ready to be used but there are STD Tests pending of execution; a question Message is shown, and the User 
                                        'can choose between stop the WS Start/Continue process to solve the problem or to continue with the execution despite of the problem

                                        ' XB 20/11/2013 - No ISE warnings can appear when ISE module is not installed
                                        'userAnswer = ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                        If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                                            userAnswer = ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                        Else
                                            userAnswer = Windows.Forms.DialogResult.Yes
                                        End If
                                        ' XB 20/11/2013

                                        If (userAnswer = DialogResult.Yes) Then
                                            'User has selected continue WS without ISE Tests; all pending ISE Tests will be blocked
                                            updateExecutions = True
                                            EnableButtonAndMenus(False)

                                        Else
                                            EnableButtonAndMenus(True, True)   ' XB 07/11/2013
                                            Application.DoEvents()
                                        End If
                                    Else
                                        'ISE Module is not ready and there are not STD Tests pending of execution; an error Message is shown due to 
                                        'the WS can not be started

                                        ' XB 21/11/2013 - In Pause mode, WS must go on
                                        'userAnswer = DialogResult.No
                                        If MDIAnalyzerManager.AllowScanInRunning Then
                                            userAnswer = DialogResult.Yes
                                        Else
                                            userAnswer = DialogResult.No
                                        End If
                                        ' XB 21/11/2013

                                        ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", GlobalEnumerates.Messages.ONLY_ISE_WS_NOT_STARTED.ToString)

                                        'All ISE Pending Tests will be blocked
                                        updateExecutions = True
                                    End If

                                    ' XB 23/07/2013 - Close buzzer
                                    If Not MDIAnalyzerManager Is Nothing Then
                                        MDIAnalyzerManager.StopAnalyzerRinging()
                                    End If
                                    ' XB 22/07/2013

                                    If (updateExecutions) Then
                                        If (Me.ActiveMdiChild Is Nothing) OrElse (Not TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                            Dim myExecutionsDelegate As New ExecutionsDelegate
                                            myGlobal = myExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                                        End If

                                        If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                                            'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                                            Dim myDummyUIRefresh As New UIRefreshDS
                                            IMonitor.UpdateWSState(myDummyUIRefresh)
                                        End If
                                    End If
                                End If
                            End If

                            ' XBC 26/06/2012 - ISE self-maintenance
                        Else
                            MDIAnalyzerManager.ISE_Manager.WorkSessionID = WorkSessionIDAttribute
                            myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                If (CType(myGlobal.SetDatos, Boolean)) Then
                                    MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "ALL"
                                Else
                                    MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "ISE"
                                End If
                            End If

                            If executeSTD Then
                                executeSTD = False
                                ' Do nothing. leaving to continue with the work session execution
                                ' Previously existing ISE preparations have been locked
                            Else
                                ' Check ISE calibrations required
                                If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                                    If MDIAnalyzerManager.ISE_Manager.IsISEModuleReady Then
                                        ' Check if initial Purges are required. This is required when any calibration is required
                                        Dim ElectrodesCalibrationRequired As Boolean = False
                                        'Dim PumpsCalibrationRequired As Boolean = False    ' XB 22/10/2013 - BT #1343
                                        Dim BubblesCalibrationRequired As Boolean = False
                                        ' Check if ISE Electrodes calibration is required
                                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckElectrodesCalibrationIsNeeded
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            ElectrodesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                                        End If

                                        ' XB 22/10/2013 - BT #1343
                                        '' Check if ISE Pumps calibration is required
                                        'myGlobal = MDIAnalyzerManager.ISE_Manager.CheckPumpsCalibrationIsNeeded
                                        'If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                        '    PumpsCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                                        'End If
                                        ' XB 22/10/2013

                                        ' Check if ISE Bubbles calibration is required 
                                        myGlobal = MDIAnalyzerManager.ISE_Manager.CheckBubbleCalibrationIsNeeded
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            BubblesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                                        End If

                                        'If ElectrodesCalibrationRequired Or PumpsCalibrationRequired Or BubblesCalibrationRequired Then
                                        If ElectrodesCalibrationRequired Or BubblesCalibrationRequired Then
                                            If Not StartSessionisInitialPUGsent Then
                                                ' Execute initial Purge A and Purge B
                                                Me.StartSessionisInitialPUGsent = True
                                                myGlobal = MDIAnalyzerManager.ISE_Manager.DoMaintenanceExit() ' this function throws Purge A and Purge B 
                                                If Not myGlobal.HasError Then
                                                    MDIAnalyzerManager.ISE_Manager.PurgeAbyFirmware += 1
                                                    MDIAnalyzerManager.ISE_Manager.PurgeBbyFirmware += 1
                                                    ShowStatus(Messages.STARTING_SESSION)
                                                    Me.StartSessionisPending = True
                                                    EnableButtonAndMenus(False)
                                                    Cursor = Cursors.WaitCursor
                                                    SetEnableMainTab(False)
                                                    ' XBC 29/08/2012 - Disable Monitor Panel
                                                    'If Not Me.ActiveMdiChild Is Nothing Then
                                                    '    Me.ActiveMdiChild.Enabled = False
                                                    'End If
                                                    InitializeMarqueeProgreesBar()
                                                End If
                                                ' waiting until CALIBRATION operation is performed
                                                ' then will be back to continue Work Session
                                                Return myGlobal
                                                Exit Function

                                            Else
                                                ' Execute required Calibrations

                                                If ElectrodesCalibrationRequired And Not Me.SkipCALB Then
                                                    Me.StartSessionisCALBsent = True
                                                    myGlobal = MDIAnalyzerManager.ISE_Manager.DoElectrodesCalibration()
                                                    If myGlobal.HasError Then
                                                        ShowMessage("Error", GlobalEnumerates.Messages.ISE_CALB_WRONG.ToString)
                                                    Else
                                                        ShowStatus(Messages.STARTING_SESSION)
                                                        Me.StartSessionisPending = True
                                                        EnableButtonAndMenus(False)
                                                        Cursor = Cursors.WaitCursor
                                                        SetEnableMainTab(False)
                                                        ' XBC 29/08/2012 - Disable Monitor Panel
                                                        'If Not Me.ActiveMdiChild Is Nothing Then
                                                        '    Me.ActiveMdiChild.Enabled = False
                                                        'End If
                                                        InitializeMarqueeProgreesBar()
                                                    End If
                                                    ' waiting until CALIBRATION operation is performed
                                                    ' then will be back to continue Work Session
                                                    Return myGlobal
                                                    Exit Function
                                                End If

                                                ' XB 22/10/2013 - PMCL check is performed always (although there are no ISE preparations on WS) - BT #1343
                                                'If PumpsCalibrationRequired And Not Me.SkipPMCL Then
                                                '    Me.StartSessionisPMCLsent = True
                                                '    myGlobal = MDIAnalyzerManager.ISE_Manager.DoPumpsCalibration()
                                                '    If myGlobal.HasError Then
                                                '        ShowMessage("Error", GlobalEnumerates.Messages.ISE_PMCL_WRONG.ToString)
                                                '    Else
                                                '        ShowStatus(Messages.STARTING_SESSION)
                                                '        Me.StartSessionisPending = True
                                                '        EnableButtonAndMenus(False)
                                                '        Cursor = Cursors.WaitCursor
                                                '        SetEnableMainTab(False)
                                                '        ' XBC 29/08/2012 - Disable Monitor Panel
                                                '        'If Not Me.ActiveMdiChild Is Nothing Then
                                                '        '    Me.ActiveMdiChild.Enabled = False
                                                '        'End If
                                                '        InitializeMarqueeProgreesBar()
                                                '    End If
                                                '    ' waiting until CALIBRATION operation is performed
                                                '    ' then will be back to continue Work Session
                                                '    Return myGlobal
                                                '    Exit Function
                                                'End If
                                                ' XB 22/10/2013

                                                If BubblesCalibrationRequired And Not Me.SkipBMCL Then
                                                    Me.StartSessionisBMCLsent = True
                                                    myGlobal = MDIAnalyzerManager.ISE_Manager.DoBubblesCalibration()
                                                    If myGlobal.HasError Then
                                                        ShowMessage("Error", GlobalEnumerates.Messages.ISE_BMCL_WRONG.ToString)
                                                    Else
                                                        ShowStatus(Messages.STARTING_SESSION)
                                                        Me.StartSessionisPending = True
                                                        EnableButtonAndMenus(False)
                                                        Cursor = Cursors.WaitCursor
                                                        SetEnableMainTab(False)
                                                        ' XBC 29/08/2012 - Disable Monitor Panel
                                                        'If Not Me.ActiveMdiChild Is Nothing Then
                                                        '    Me.ActiveMdiChild.Enabled = False
                                                        'End If
                                                        InitializeMarqueeProgreesBar()
                                                    End If
                                                    ' waiting until CALIBRATION operation is performed
                                                    ' then will be back to continue Work Session
                                                    Return myGlobal
                                                    Exit Function
                                                End If

                                            End If

                                        End If

                                    End If

                                End If
                            End If
                        End If

                    Else
                        If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                            MDIAnalyzerManager.ISE_Manager.WorkSessionTestsByType = "STD"
                        End If
                        ' XBC 26/06/2012

                    End If
                Else
                    'Error verifying if there are requested ISE Tests pending of execution in the active Work Session; shown it
                    ShowMessage(Me.Name & ".VerifyISEConditioningBeforeRunning", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                End If

            End If  ' XB 20/11/2013

            ' XBC 23/07/2012
            Me.StartSessionisPending = False
            If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                If Not ActiveMdiChild Is Nothing Then
                    If (TypeOf ActiveMdiChild Is IMonitor) Then
                        Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                        MDIAnalyzerManager.ISE_Manager.WorkSessionOverallTime = CurrentMdiChild.remainingTime
                    End If
                End If
            End If

            If Not myGlobal.HasError AndAlso userAnswer = DialogResult.Yes Then
                If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then

                    'SGM 01/10/2012 - If no calibration is needed set ISE preparations as PENDING
                    'Dim myOrderTestsDelegate As New OrderTestsDelegate
                    myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        If CBool(myGlobal.SetDatos) Then

                            ' XBC 12/04/2013 - Create Executions is a long time process so is need to be called on threading mode
                            'Dim isReady As Boolean = False
                            'Dim myAffectedElectrodes As List(Of String)
                            'myGlobal = MDIAnalyzerManager.ISE_Manager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
                            'If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            '    isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)
                            'End If

                            'Dim myExecutionDelegate As New ExecutionsDelegate
                            'Dim createWSInRunning As Boolean = (MDIAnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)
                            'myGlobal = myExecutionDelegate.CreateWSExecutions(Nothing, MDIAnalyzerManager.ActiveAnalyzer, MDIAnalyzerManager.ActiveWorkSession, _
                            '                                              createWSInRunning, -1, String.Empty, isReady, myAffectedElectrodes)

                            ''refresh WS grid
                            'If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                            '    'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                            '    Dim myDummyUIRefresh As New UIRefreshDS
                            '    IMonitor.UpdateWSState(myDummyUIRefresh)
                            'End If

                            Me.StartSessionisPending = True
                            myGlobal = CreateExecutionsProcessByISEChanges()
                            If Not myGlobal.HasError Then
                                pWSExecutionsCreatedFlag = True
                                Me.StartSessionisPending = False
                            End If
                            ' XBC 12/04/2013

                        End If
                    End If
                    'end SGM 01/10/2012

                    MDIAnalyzerManager.ISE_Manager.WorkSessionIsRunning = True

                    'Dim myLogAcciones As New ApplicationLogManager()    ' TO COMMENT !!!
                    'myLogAcciones.CreateLogActivity("Update Consumptions - Update Last Date WS ISE Operation [ " & DateTime.Now.ToString & "]", "Ax00MainMDI.StartSession", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                    ' Update date for the ISE test executed while running
                    MDIAnalyzerManager.ISE_Manager.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_WS_DATE, DateTime.Now.ToString, True)
                End If

            End If

            myGlobal.SetDatos = userAnswer

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".VerifyISEConditioningBeforeRunning ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VerifyISEConditioningBeforeRunning ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Create Executions is a long time process so is need to be called on threading mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2013</remarks>
    Public Function CreateExecutionsProcessByISEChanges() As GlobalDataTO
        Dim returnValue As New GlobalDataTO
        Try
            Cursor = Cursors.WaitCursor

            SetActionButtonsEnableProperty(False)
            EnableButtonAndMenus(False)

            Dim workingThread As New Threading.Thread(AddressOf CreateWSExecutionsWithISEChanges)
            screenWorkingProcess2 = True
            workingThread.Start()

            While screenWorkingProcess2
                Me.InitializeMarqueeProgreesBar()
                Application.DoEvents()
            End While

            workingThread = Nothing
            Me.StopMarqueeProgressBar()

        Catch ex As Exception
            returnValue.HasError = True
            CreateLogActivity(ex.Message, Me.Name & ".CreateExecutionsProcess", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Me.Cursor = Cursors.Default
            Application.DoEvents()
            ShowMessage(Me.Name & "CreateExecutionsProcess ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, Me)
        Finally
            Cursor = Cursors.Default
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' Create Executions to WS with Status changes
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 12/04/2013
    ''' Modified by XB 18/07/2013 - fix a bug
    ''' </remarks>
    Private Sub CreateWSExecutionsWithISEChanges()
        Dim myGlobal As New GlobalDataTO
        Try
            Dim isReady As Boolean = False
            Dim myAffectedElectrodes As List(Of String)
            myGlobal = MDIAnalyzerManager.ISE_Manager.CheckAnyCalibrationIsNeeded(myAffectedElectrodes)
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                isReady = Not (CBool(myGlobal.SetDatos) And myAffectedElectrodes Is Nothing)
            End If

            If isReady Then
                Dim myExecutionDelegate As New ExecutionsDelegate
                Dim createWSInRunning As Boolean = (MDIAnalyzerManager.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)
                myGlobal = myExecutionDelegate.CreateWSExecutions(Nothing, MDIAnalyzerManager.ActiveAnalyzer, MDIAnalyzerManager.ActiveWorkSession, _
                                                                  createWSInRunning, -1, String.Empty, isReady, myAffectedElectrodes)

                'refresh WS grid
                If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                    'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                    Dim myDummyUIRefresh As New UIRefreshDS

                    ' XB+AG 18/07/2013 - there was a bug when try to refresh monitor grid from a thread without use Me.UIThread (# bugstracking)
                    'IMonitor.UpdateWSState(myDummyUIRefresh)
                    Me.UIThread(Sub() IMonitor.UpdateWSState(myDummyUIRefresh))
                    ' XB+AG 18/07/2013
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CreateWSExecutions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".CreateWSExecutions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".CreateWSExecutions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        Finally
            screenWorkingProcess2 = False
        End Try
    End Sub

    ''' <summary>
    ''' Send to the instrument the Start (Run mode) instruction
    ''' 
    ''' NOTE:Before send instruction calculate if the current bottle levels are enough for execute completely the WS
    ''' If not show a warning message to the user 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 (Tested pending)
    ''' Modified by: SA 15/06/2010 - Set WorkSession Status to INPROCESS
    '''              TR 04/10/2011 - Disable all buttons and menus in the MDI while the WorkSession is executing
    '''              TR 27/10/2011 - Stop the Analyzer Sound before begin the process
    '''              TR 11/11/2011 - Start the Elapse Time Timer
    '''              SA 21/03/2012 - Verify if ISE Module is installed, needed in the WS, and ready to use (COMMENTED)
    '''              SA 14/05/2012 - When ISE Module is not available, verify if there are STD Tests pending of execution to shown the 
    '''                              proper message: 
    '''                              ** If there are STD Tests pending of execution: Continue WS with ISE Tests blocked? - Question Message
    '''                              ** Otherwise, ISE Module not available - Error Message
    '''                              In the first case, when the User's answer is YES, if the screen of WSRotorPositions is not opened, then 
    '''                              call the function to block all pending ISE Preparations UpdateStatusByExecutionTypeAndStatus
    '''              XB 23/07/2012 - Move the code to StartSession Sub
    ''' </remarks>
    Private Sub bsTSStartSessionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            SetHQProcessByUserFlag(False) 'AG 30/07/2013 - on press START WS button reset the flag for host query from MDI
            'Normal business
            MyClass.InitiateStartSession()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSStartInstrumentButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSStartInstrumentButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Send and ENDRUN instruction for paused the instrument
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  AG 22/02/2011
    ''' Modified by: AG 11/07/2013 - changes for pause the auto create WS with LIS
    '''              XB 15/10/2013 - Send PAUSE instruction instead of ENDRUN instruction - BT #1333
    ''' </remarks>
    Private Sub bsTSPauseSessionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                'JV + AG revision 18/10/2013 task # 1341
                SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcessPaused)   ' XB 30/10/2013 - emplaced before ChangeStatusImageMultipleSessionButton
                'bsTSPauseSessionButton.Enabled = False
                'JV + AG revision 18/10/2013  task # 1341
                'bsTSMultiFunctionSessionButton.Image = imagePlay
                bsTSMultiFunctionSessionButton.Enabled = False
            Else

                If (Not MDIAnalyzerManager Is Nothing) Then

                    'Disable all buttons until Ax00 accepts the new instruction
                    SetActionButtonsEnableProperty(False)

                    Dim myGlobal As New GlobalDataTO
                    MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                    MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute

                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS"
                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""

                    'TR 27/10/2011 -Stop Sound.
                    MDIAnalyzerManager.StopAnalyzerRinging()

                    'TR 02/12/2011 -Stop the elapsed time timer
                    ElapsedTimeTimer.Stop()
                    UserPauseWSAttribute = True 'TR 21/05/2012 -Set the flag user pause the work session to true after time stop.

                    ' XB 15/10/2013 - BT #1333
                    'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True)
                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.PAUSE, True)

                    Dim activateButtonsFlag As Boolean = False
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        If (Not MDIAnalyzerManager.Connected) Then
                            MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = ""
                            'AG 14/10/2011 - Error Comms message is shown in ManageReceptionEvent
                            'Dim myAuxGlobal As New GlobalDataTO() With {.ErrorCode = "ERROR_COMM"}
                            'ShowMessage("Warning", myAuxGlobal.ErrorCode)
                            activateButtonsFlag = True
                        End If

                    ElseIf (myGlobal.HasError) Then
                        ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                        activateButtonsFlag = True
                    End If

                    If activateButtonsFlag Then
                        SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
                    Else
                        SetWorkSessionUserCommandFlags(WorkSessionUserActions.PAUSE_WS)
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSStartInstrumentButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSStartInstrumentButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Execute the cancelling (abort) of the active WorkSession
    ''' </summary>
    ''' <remarks>
    ''' Create by:  SA 15/06/2010 
    ''' AG 12/07/2011 - comment code until Fw implements the instruction
    ''' AG 20/02/2012 - uncommented
    ''' AG 27/11/2013 - Task #1397 during recovery results this button only implements the abort worksession process
    ''' </remarks>
    Private Sub bsTSAbortSessionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTSAbortSessionButton.Click
        Try
            AbortSessionFromEventClicked()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSAbortSessionButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSAbortSessionButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub



    ''' <summary>
    ''' Recover instruction is sent (code based on the start sesion button)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 07/03/2012</remarks>
    Private Sub bsTSRecover_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSRecover.Click
        Try
            CreateLogActivity("Btn Recover", Me.Name & ".bsTSRecover_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If Not MDIAnalyzerManager Is Nothing Then
                Cursor = Cursors.WaitCursor
                MDIAnalyzerManager.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound

                SetActionButtonsEnableProperty(False) 'Disable all vertical action buttons bar
                EnableButtonAndMenus(False)
                ChangeErrorStatusLabel(True) 'AG 18/05/2012 - Clear error provider 

                If Not Me.ActiveMdiChild Is Nothing Then
                    DisabledMdiForms = Me.ActiveMdiChild
                    Me.ActiveMdiChild.Enabled = False
                End If

                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 0 'clear sensor
                Dim workingThread As New Threading.Thread(AddressOf RecoverInstrument)
                ScreenWorkingProcess = True
                processingRecover = True
                workingThread.Start()

                While processingRecover 'ScreenWorkingProcess
                    InitializeMarqueeProgreesBar()
                    'AG 15/03/2012 - TEMPORAL special code for the recover method
                    If Not Me.ActiveMdiChild Is Nothing Then
                        If Me.ActiveMdiChild.Enabled Then
                            DisabledMdiForms = Me.ActiveMdiChild
                            Me.ActiveMdiChild.Enabled = False
                        End If
                    End If
                    'AG 15/03/2012
                    Cursor = Cursors.WaitCursor
                    Application.DoEvents()
                End While

                workingThread = Nothing

                StopMarqueeProgressBar()
                Cursor = Cursors.Default

                Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
                If Not ActiveMdiChild Is Nothing Then
                    ActiveMdiChild.Enabled = True
                ElseIf DisabledMDIChildAttribute.Count > 0 Then
                    myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    myCurrentMDIForm.Enabled = True
                    If DisabledMDIChildAttribute.Count = 1 Then
                        DisabledMDIChildAttribute.Clear()
                    End If
                End If
                SetActionButtonsEnableProperty(True)

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSRecover_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSRecover_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        EnableButtonAndMenus(True)
    End Sub

#End Region

#Region "Private methods"

    ''' <summary>
    ''' Validate if exist any Saved WorkSession.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 01/08/2011.</remarks>
    Private Function ExistSavedWorkSession() As Boolean
        Dim ExistWS As Boolean = True
        Try
            'TR 01/08/2011 -Validate if there're any saved worksession.
            Dim myGlobalDataTO As New GlobalDataTO
            Dim mySavedWSDelegate As New SavedWSDelegate

            myGlobalDataTO = mySavedWSDelegate.GetAll(Nothing)

            If Not myGlobalDataTO.HasError Then
                If Not myGlobalDataTO.SetDatos Is Nothing AndAlso _
                    DirectCast(myGlobalDataTO.SetDatos, SavedWSDS).tparSavedWS.Count = 0 Then
                    'Show the Error Message - There are not WS to Load
                    ShowMessage(Me.Name, GlobalEnumerates.Messages.WS_NOT_FOUND.ToString(), "", Me)
                    ExistWS = False
                End If
            End If
            'TR 01/08/2011 -END
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExistSavedWorkSession", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExistSavedWorkSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return ExistWS
    End Function

    ''' <summary>
    ''' Validate if exist any Saved Virtual Rotor.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 01/08/2011</remarks>
    Private Function ExistVirtualSavedRotor() As Boolean
        Dim ExistVRotor As Boolean = True
        Try
            Dim rmyGlobalDataTO As GlobalDataTO
            If String.Equals(AnalyzerModelAttribute, "A400") Then
                Dim myVRotorsDelegate As New VirtualRotorsDelegate

                'Get all Virtual Rotors 
                rmyGlobalDataTO = myVRotorsDelegate.GetVRotorsByRotorType(Nothing, "")

                If (Not rmyGlobalDataTO.HasError AndAlso Not rmyGlobalDataTO.SetDatos Is Nothing AndAlso _
                        DirectCast(rmyGlobalDataTO.SetDatos, VirtualRotorsDS).tparVirtualRotors.Count = 0) Then

                    ShowMessage(Me.Name, GlobalEnumerates.Messages.NO_VIRTUAL_ROTORS.ToString, "", Me)
                    ExistVRotor = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExistVirtualSavedRotor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExistVirtualSavedRotor ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
        Return ExistVRotor
    End Function

    ''' <summary>
    ''' Reset the active Analyzer WorkSession
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: AG 18/01/2011 - After Reset the WS, call function ResetWorkSession in AnalyzerManager to clear the DS with last used Reagents
    '''              DL 13/05/2011 - Added code for management of LogFiles
    '''              DL 14/06/2011 - Before executing the Reset, generate an Excel file with all the Results
    '''              AG 15/06/2011 - Remove the Excel generation, it becomes the Reset process too slow. Call function CreateSATReport instead
    '''              TR 21/11/2011 - Reset the global variables used to calculate the time needed to execute the WorkSession
    '''              SA 05/09/2012 - Removed code for management of LogFiles due to this process was executed just before executing
    '''                              this function in code of button event bsTSResetSessionButton_Click
    '''              XB 23/04/2013 - Add ShowMultipleMessage method to can display the multiples errors can be occurred
    '''              AG 28/11/2013 - BT #1397
    ''' </remarks>
    Private Sub ResetSession()
        Try
            Dim myGlobal As GlobalDataTO
            'Dim GenerateResultsExcel As Boolean = False
            Const ExcelPath As String = ""

            'If WorkSession has started then create an auto ReportSAT
            Dim myWS_Delg As New WorkSessionsDelegate
            Dim pendingExecutionsLeft As Boolean = False
            Dim executionsNumber As Integer = 0

            myGlobal = myWS_Delg.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                Dim wsStartedFlag As Boolean = DirectCast(myGlobal.SetDatos, Boolean)
                If (wsStartedFlag) Then
                    '' AG 15/06/2011 - Comment this code due the RESET process become slow DL 14/06/2011
                    '' Add to report sat excel with results
                    'Dim myAnalyzerManager As AnalyzerManager
                    'myAnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                    'If myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Or myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                    '    Dim swParams As New SwParametersDelegate
                    '    Dim resultData As New GlobalDataTO

                    '    resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)
                    '    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    '        Dim myDS As New ParametersDS
                    '        myDS = CType(resultData.SetDatos, ParametersDS)
                    '        Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)
                    '        myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                    '                  Where a.ParameterName = GlobalEnumerates.SwParameters.XLS_RESULTS_DOC.ToString Select a).ToList

                    '        Dim filename As String = ""
                    '        If myList.Count > 0 Then filename = myList(0).ValueText '= DateTime.Now.ToString("yyyyMMdd hh-mm") & "_" & myList(0).ValueText

                    '        myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                    '                  Where a.ParameterName = GlobalEnumerates.SwParameters.XLS_PATH.ToString Select a).ToList
                    '        Dim pathName As String = ""
                    '        If myList.Count > 0 Then pathname = myList(0).ValueText

                    '        If pathname.StartsWith("\") And Not pathname.StartsWith("\\") Then
                    '            pathname = Application.StartupPath & pathname & DateTime.Now.ToString("dd-MM-yyyy hh-mm") & "\"
                    '        End If

                    '        resultData = IResults.ExportResultsWithParameters(filename, pathName, WorkSessionIDAttribute)

                    '        If Not resultData.HasError Then
                    '            GenerateResultsExcel = True
                    '            ExcelPath = pathName
                    '        End If
                    '    End If
                    'End If
                    '' DL 14/06/2011

                    Dim mySATUtil As New SATReportUtilities
                    myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_REPORT, True, ExcelPath, myAdjustmentsFilePath)

                    'TR 21/11/2011 -Reset the time variable.
                    LocalElapsedTimeAttribute = New DateTime
                    InitialRemainingTimeAttribute = New DateTime
                    'TR 09/05/2012 -Reset the Seconds variables.
                    TotalSecs = 0
                    TotalSecs2 = 0
                    'TR 09/05/2012 -Reset the Seconds variables. 
                End If
            End If

            If (Not myGlobal.HasError) Then
                Dim myWS As New WorkSessionsDelegate

                If (Not HISTWorkingMode) Then
                    myGlobal = myWS.ResetWS(Nothing, ActiveAnalyzer, ActiveWorkSession)
                Else
                    myGlobal = myWS.ResetWSNEW(Nothing, ActiveAnalyzer, ActiveWorkSession)
                End If

                If (Not myGlobal.HasError) Then
                    recoveryResultsWarnFlag = False 'AG 28/11/2013 - BT #1397
                    MDIAnalyzerManager.ResetWorkSession() 'AG 18/01/2011 - Clear DS with reagents sent to the analyzer

                    ' DL 13/05/2011
                    'Const fileZip As String = "PreviousLog.zip"
                    'Dim sourcePath As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                    'Dim OldXmlFile As String = sourcePath & GlobalBase.XmlLogFile
                    'Dim arrayFileName() As String = Split(GlobalBase.XmlLogFile, ".xml")
                    'Dim NewXmlFile As String = sourcePath & arrayFileName(0) & " " & Format(Now, "yyyyMMdd hhmmss") & ".xml"

                    'If (System.IO.File.Exists(OldXmlFile)) Then
                    '    Rename(OldXmlFile, NewXmlFile)

                    '    Dim myUtil As New Utilities
                    '    If (System.IO.File.Exists(sourcePath & "Temp")) Then
                    '        myGlobal = myUtil.RemoveFolder(sourcePath & "Temp")
                    '    End If

                    '    myGlobal = myUtil.CreateFolder(sourcePath & "Temp")
                    '    If (Not myGlobal.HasError) Then
                    '        myGlobal = myUtil.MoveFiles(sourcePath, sourcePath & "Temp\", "AX00Log*.xml")

                    '        If (Not myGlobal.HasError) Then
                    '            If (System.IO.File.Exists(sourcePath & fileZip)) Then
                    '                'DL 03/09/2011
                    '                'Dim MyLogXmlDocument As New XmlDocument ' create a XmlDocument object to load the Log Xml file.
                    '                Dim fileDetail As IO.FileInfo
                    '                fileDetail = My.Computer.FileSystem.GetFileInfo(sourcePath & fileZip)

                    '                'If file size is greater 5MB rename old log and create new log
                    '                If (fileDetail.Length() / 1024) > 5120 Then
                    '                    If (System.IO.File.Exists(sourcePath & fileZip)) Then Rename(sourcePath & fileZip, sourcePath & "PreviousLog " & Format(Now, "yyyyMMdd hhmmss") & ".zip")
                    '                End If

                    '                myGlobal = myUtil.ExtractFromZip(sourcePath & fileZip, sourcePath & "Temp\")

                    '                If (Not myGlobal.HasError) Then Kill(sourcePath & fileZip)
                    '            End If

                    '            '' dl 14/06/2011
                    '            'If GenerateResultsExcel Then
                    '            '    myGlobal = myUtil.MoveFiles(ExcelPath, sourcePath & "Temp\", "*.xls")
                    '            'End If
                    '            '' dl 14/06/2011

                    '            myGlobal = myUtil.CompressToZip(sourcePath & "Temp\", sourcePath & fileZip)
                    '            If (Not myGlobal.HasError) Then
                    '                myGlobal = myUtil.RemoveFolder(sourcePath & "Temp")
                    '            End If
                    '        End If
                    '    End If

                    'Else
                    '    If (System.IO.File.Exists(sourcePath & fileZip)) Then
                    '        'DL 03/09/2011
                    '        'Dim MyLogXmlDocument As New XmlDocument ' create a XmlDocument object to load the Log Xml file.
                    '        Dim fileDetail As IO.FileInfo
                    '        fileDetail = My.Computer.FileSystem.GetFileInfo(sourcePath & fileZip)

                    '        'If file size is greater 5MB rename old log and create new log
                    '        If (fileDetail.Length() / 1024) > 5120 Then
                    '            If (System.IO.File.Exists(sourcePath & fileZip)) Then Rename(sourcePath & fileZip, sourcePath & "PreviousLog " & Format(Now, "yyyyMMdd hhmmss") & ".zip")
                    '        End If
                    '    End If
                    'End If
                Else
                    'Error executing the Reset WS; show it

                    ' XB 23/04/2013 - errors management can't be use showmessage inside a thread, so it is emplaced on event button click
                    'ShowMessage("AX00", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                    If myGlobal.SetDatos Is Nothing Then
                        myErrorsList.Add(myGlobal.ErrorCode)
                    Else
                        myErrorsList = DirectCast(myGlobal.SetDatos, List(Of String))
                    End If
                    ' XB 23/04/2013
                End If
            Else
                'Error checking if the WS is started or when create the automatic Report SAT; show it
                'DL 15/05/2013
                'ShowMessage("AX00", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
                Me.UIThread(Function() ShowMessage("AX00", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me))
                'DL 15/05/2013
                CreateLogActivity("Error to create automatic SAT", Name & ".ResetSessionButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ResetSession", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".ResetSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".ResetSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        Finally
            ScreenWorkingProcess = False
            processingReset = False
        End Try
    End Sub


    ''' <summary>
    ''' Method incharge to load the buttons image
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 25/05/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'temporal
            'Analyzer Configuration Button
            'auxIconName = GetIconName("ANALYZER")
            'If (auxIconName <> "") Then bsTSAnalysersButton.Image = Image.FromFile(iconPath & auxIconName)

            'Test Profiles Button
            'auxIconName = GetIconName("PROFILES")
            'If (auxIconName <> "") Then bsTSProfileButtton.Image = Image.FromFile(iconPath & auxIconName)

            'Work Session Preparation Button
            'auxIconName = GetIconName("SAMPLES")
            'If (auxIconName <> "") Then bsTSNewSampleButton.Image = Image.FromFile(iconPath & auxIconName)

            'temporal
            'Rotor Positioning Button
            'auxIconName = GetIconName("POSITIONIN")
            'If (auxIconName <> "") Then bsTSPositionButton.Image = Image.FromFile(iconPath & auxIconName)

            'Monitor Button
            'auxIconName = GetIconName("MONITOR")
            'If (auxIconName <> "") Then bsTSMonitorButton.Image = Image.FromFile(iconPath & auxIconName)

            'Results Review Button
            'auxIconName = GetIconName("RESULTS")
            'If (auxIconName <> "") Then bsTSResultsButton.Image = Image.FromFile(iconPath & auxIconName)

            'AG 17/06/2010
            'ReportSAT button
            'auxIconName = GetIconName("REPORTSAT")
            'If (auxIconName <> "") Then bsTSReportSATButton.Image = Image.FromFile(iconPath & auxIconName)

            'temporal
            'Reset button
            'auxIconName = GetIconName("RESETSES")
            'If (auxIconName <> "") Then bsTSResetSessionButton.Image = Image.FromFile(iconPath & auxIconName)

            ' temporal
            'Alarms button
            'auxIconName = GetIconName("ALARM")
            'If (auxIconName <> "") Then bsTSAlarmsButton.Image = Image.FromFile(iconPath & auxIconName)

            'temporal
            'Alarm Utilities button
            'auxIconName = GetIconName("UTILITIES")
            'If (auxIconName <> "") Then bsTSUtilitiesButton.Image = Image.FromFile(iconPath & auxIconName)

            'Test Programming button
            'auxIconName = GetIconName("TESTPROG")
            'If (auxIconName <> "") Then BsTSTestButton.Image = Image.FromFile(iconPath & auxIconName)

            'Error Status Label button 
            'TR 05/05/2011 -Change icon
            'auxIconName = GetIconName("FORMULAWR")
            auxIconName = GetIconName("STUS_WITHERRS")
            'TR 05/05/2011 -END
            If Not String.Equals(auxIconName, String.Empty) Then ErrorStatusLabel.Image = Image.FromFile(iconPath & auxIconName)

            ''TR 27/10/2011 -SOUND OFF BUTTON 
            'auxIconName = GetIconName("SOUNDOFF")
            'If (auxIconName <> "") Then SoundOffButton.BackgroundImage = Image.FromFile(iconPath & auxIconName)
            ''TR 27/10/2011 -

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Load all screen labels and tooltips in the current application language
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 02/11/2010 - TODO: Check if rest of required Labels and ToolTips have been already 
    '''                              created in BD and complete this function 
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Main Menu Options
            ConfigurationToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Configuration", CurrentLanguageAttribute)
            ProgrammingToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Programming", CurrentLanguageAttribute)
            WorksessionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WorkSession", CurrentLanguageAttribute)
            CurrentSessionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CurrentState", CurrentLanguageAttribute)
            MainResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Results", CurrentLanguageAttribute)
            HistoricalReportsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Reports", CurrentLanguageAttribute)
            UtilitiesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", CurrentLanguageAttribute)
            ExitToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Exit", CurrentLanguageAttribute)
            HelpToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Help", CurrentLanguageAttribute)

            'bsTSWestgard.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QualityControlResults", CurrentLanguageAttribute)

            'Other screen labels
            InstrumentToolStripLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Instrument", CurrentLanguageAttribute)
            SessionToolStripLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Session", CurrentLanguageAttribute)
            'TR 02/09/2011
            CumulatedButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Cumulate_Results", CurrentLanguageAttribute)

            'AG - No inicializar este texto
            'bsAnalyzerStatus.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", CurrentLanguageAttribute)

            'DL 14/10/2011
            'Configuration
            AnalyzerToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_General", CurrentLanguageAttribute)
            LanguageToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_LANGUAGE", CurrentLanguageAttribute)
            HeadPToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_REPORTS", CurrentLanguageAttribute)
            BarcodeToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", CurrentLanguageAttribute)
            ISEToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISEMODULE_Alert", CurrentLanguageAttribute)
            MasterDataDictionaryToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_MASTERDATA", CurrentLanguageAttribute)
            UsersManagementToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_USERS", CurrentLanguageAttribute)
            ChangeUserToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CHANGE_USER", CurrentLanguageAttribute)
            TestPrintSortingToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TEST_PRINT_SORT", CurrentLanguageAttribute)

            LISConfigToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_CONFIG_TITLE", CurrentLanguageAttribute)
            LISMappingToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS_MAPPING", CurrentLanguageAttribute)

            'DL 17/07/2013
            myAutoLISWaitingOrder = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_AUTOLIS_WAITING_ORDERS", CurrentLanguageAttribute)

            'Programming
            TestsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguageAttribute) 'JB 01/10/2012 - Resource String unification
            CalculatedTestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Long", CurrentLanguageAttribute)
            ContaminationsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations", CurrentLanguageAttribute)
            ProfilesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_TestProfiles", CurrentLanguageAttribute)
            CalibratorsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", CurrentLanguageAttribute)
            CalibratorsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", CurrentLanguageAttribute)
            ControlsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Controls", CurrentLanguageAttribute)
            PatientSearchToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_PatientData", CurrentLanguageAttribute)
            ISEModuleToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_ISETests", CurrentLanguageAttribute)
            OffSystemModuleToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_OFFSYSTEM", CurrentLanguageAttribute)
            RegentDisksToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_REAGENTSDISK", CurrentLanguageAttribute)
            SampleDisksToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SAMPLESDISK", CurrentLanguageAttribute)

            'WorkSession
            SampleRequestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SampleRequest", CurrentLanguageAttribute)
            RotorPositionsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_RotorPositioning", CurrentLanguageAttribute)
            LIMSImportToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_LIMSIMport", CurrentLanguageAttribute)
            LoadSessionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_LoadSavedWS", CurrentLanguageAttribute)
            SaveSessionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SaveWS", CurrentLanguageAttribute)
            DeleteSessionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_DeleteSavedWS", CurrentLanguageAttribute)
            DeleteVirtualRotorsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelAux_VRotors", CurrentLanguageAttribute)

            'Monitor
            MonitorToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Monitor", CurrentLanguageAttribute)
            ResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WSResults", CurrentLanguageAttribute)

            'Results
            SampleResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SampleResults", CurrentLanguageAttribute)
            BlankCalibratorResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BlankCalibResults", CurrentLanguageAttribute)
            QCResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_QCResults", CurrentLanguageAttribute)
            ISEResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ISEResults", CurrentLanguageAttribute)
            StatisticsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Statistics", CurrentLanguageAttribute)
            HistoricalDDBBReviewToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_HistDBReview", CurrentLanguageAttribute)
            HistoricalAnalyzerAlarmsReviewToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_HistReviewAlarms", CurrentLanguageAttribute)

            'Utilities
            'AnalyzerUtilitiesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_AnalyzerUtilities", CurrentLanguageAttribute)
            ChangeReactionsRotorToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ChangeReactionsRotor", CurrentLanguageAttribute)
            ChangeSettingsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ChangeSettings", CurrentLanguageAttribute)
            ISEUtilitiesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ISEUtilities", CurrentLanguageAttribute)

            LISUtilitiesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_LIS_UTILITIES", CurrentLanguageAttribute)

            AnalyzerDailyMaintenanceToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_AnalyzerDailyMaintenance", CurrentLanguageAttribute)
            DataBasesBackUpToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_DBBackUp", CurrentLanguageAttribute)
            ReviewFwBlackBoxToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MMENU_BlackBoxFW", CurrentLanguageAttribute)
            VersionsCheckingToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_VersionsCheck", CurrentLanguageAttribute)
            RemoteConnectionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_RemoteCnn", CurrentLanguageAttribute)
            SATReportsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SATReports", CurrentLanguageAttribute)
            LoadSATReportToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Title", CurrentLanguageAttribute)
            LoadSATReportToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Title", CurrentLanguageAttribute)
            CreateRestorePointToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Restore", CurrentLanguageAttribute)
            RestorePreviousDataToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RestorePrevious_Title", CurrentLanguageAttribute)
            ReagentConsumptionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ReagentConsum", CurrentLanguageAttribute)
            PreventiveMaintenanceToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_PreventiveMaintenance", CurrentLanguageAttribute)
            QCResultSimulatorMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_QCResultSimulator", CurrentLanguageAttribute)
            QCCumulatedResultsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_QCACCUMULATED_RES", CurrentLanguageAttribute)
            ConditioningToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CONDITIONING", CurrentLanguageAttribute) 'DL 06/06/2012

            'Reports
            StandardReportToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_StandardReports", CurrentLanguageAttribute)
            CustomReportsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CustoReports", CurrentLanguageAttribute)
            PatirentDataToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_PatientData", CurrentLanguageAttribute)

            'Help
            WhatsNewToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WhatIsNew", CurrentLanguageAttribute)
            QuickToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_QuickGuide", CurrentLanguageAttribute)
            TutorialsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Tutorials", CurrentLanguageAttribute)
            UserMaintenancePlanToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_MaintenancePlan", CurrentLanguageAttribute)
            ApplicationNotesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_AplNotes", CurrentLanguageAttribute)
            OnlineHelpToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_OnLineHelp", CurrentLanguageAttribute)
            TroubleshooterToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_Troubleshooter", CurrentLanguageAttribute)
            AboutToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_About", CurrentLanguageAttribute) 'TR 03/11/2011
            ToolStripMenuItem10.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_OperationGuide", CurrentLanguageAttribute) 'DL 18/04/2012
            UserManualToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_UserManual", CurrentLanguageAttribute) 'DL 27/06/2013

            'RH 14/03/2012
            InstrumentInfoToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstrumentInfo", CurrentLanguageAttribute)

            'Exit
            WithShutToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WithShutDown", CurrentLanguageAttribute)
            WithOutToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WithOutShutDown", CurrentLanguageAttribute)

            'Menu lateral derecho
            InstrumentToolStripLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Instrument", CurrentLanguageAttribute)
            SessionToolStripLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Session", CurrentLanguageAttribute)

            'ToolTips
            'Menu Horizontal
            bsTSAnalysersButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AnalyzersCfg", CurrentLanguageAttribute)
            BsTSTestButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_TestProgramming", CurrentLanguageAttribute)
            bsTSProfileButtton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Profiles", CurrentLanguageAttribute)
            bsTSNewSampleButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SampleRequest", CurrentLanguageAttribute)
            bsTSPositionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_PositioningSampleReagent", CurrentLanguageAttribute)
            bsTSMonitorButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Monitor", CurrentLanguageAttribute)
            bsTSResultsButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSResults", CurrentLanguageAttribute)
            bsTSQCResultsReview.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_QCResults", CurrentLanguageAttribute)
            CumulatedButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_QCCumulated", CurrentLanguageAttribute)
            bsTSAlarmsButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Alarms", CurrentLanguageAttribute) 'JB 01/10/2012 - Resource String unification
            bsTSUtilitiesButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AnalyzerUtilities", CurrentLanguageAttribute)
            bsTSReportSATButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SATReport", CurrentLanguageAttribute)
            bsTSInfoButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_ShowLegend", CurrentLanguageAttribute)
            bsTSResetSessionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSReset", CurrentLanguageAttribute)

            ' delete when translated, debug purpouse
            'PendingOrdersToolTip = "*Pending Orders: "
            NoPendingOrdersToolTip = "*Order Download"
            ' end delete 

            bsTSQueryAllButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_LBL_QUERYALL", CurrentLanguageAttribute)
            bsTSHostQueryButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_LBL_HOSTQUERY", CurrentLanguageAttribute)
            ' tool tip is edited for shown as:    " Orders: " + #PENDING_ORDERS
            bsTSOrdersDownloadButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_LBL_ORDERSDOWNLOAD", CurrentLanguageAttribute)
            'PendingOrdersToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_LBL_PENDING_ORDERSDOWNLOAD", CurrentLanguageAttribute)
            NoPendingOrdersToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_LBL_ORDERSDOWNLOAD", CurrentLanguageAttribute)



            'Menu vertical
            bsTSConnectButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Connect", CurrentLanguageAttribute)
            bsTSStartInstrumentButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_INST_Start", CurrentLanguageAttribute)
            bsTSShutdownButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_INST_ShutDown", CurrentLanguageAttribute)
            bsTSChangeBottlesConfirm.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_ChangeBottlesConfirmation", CurrentLanguageAttribute)
            'AG 21/02/2012 - remove old multilanguage resource BTN_RefillWashSolution

            bsTSRecover.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RecoverAnalyzer", CurrentLanguageAttribute)
            'bsTSStartSessionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Start", CurrentLanguageAttribute)
            'bsTSPauseSessionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Pause", CurrentLanguageAttribute)
            'bsTSContinueSessionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Continue", CurrentLanguageAttribute)
            bsTSAbortSessionButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Abort", CurrentLanguageAttribute)
            'DL 14/10/2011

            'TR 28/10/2011 -Tooltips for sound button.
            bsTSSoundOff.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SOUND_OFF", CurrentLanguageAttribute)     'DL 22/03/2012
            'TR 28/10/2011 -END.

            '//CF - 22/10/2013 - MessageBoxButtons
            abortButtonText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Abort", CurrentLanguageAttribute)
            cancelButtonText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", CurrentLanguageAttribute)
            stopButtonText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Stop", CurrentLanguageAttribute)
            confirmAbortEndSessionMessage = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_CONFIRM_ABORT_END_WS", CurrentLanguageAttribute)
            '//End CF - 22/10/2013	

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Load the labels in the current language and activate or deactivate the menu options
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 17/06/2010
    ''' Modified by: SA 02/11/2010 - Added call to function GetScreenLabels
    '''              SA 16/12/2010 - Hide following menu options: 
    '''                              * Configuration - ISE Module, Master Data
    '''                              * Reports - All  
    '''                              * Utilities - ISE Utilities, Reagent Consumption
    ''' AG 05/06/2013 Fix issue #1137 - After load a reportSAT the menu options available depends on the current user permissions
    ''' </remarks>
    Private Sub PrepareMenuOptions()
        Try
            'Load the menu options Language description
            GetScreenLabels()

            'Activate or deactivate menu options (by now deactivate options with out screen)
            'Configuration menu
            HeadPToolStripMenuItem.Enabled = True ' DL 30/11/2011
            'BarcodeToolStripMenuItem.Enabled = False 'AG 13/07/2011
            'LISConfigToolStripMenuItem.Enabled = False 'AG 27/02/2013 - comment this line when develop the lis config screen
            'LISMappingToolStripMenuItem.Enabled = False 'AG 27/02/2013 - comment this line when develop the lis mapping screen
            ISEToolStripMenuItem.Enabled = False
            MasterDataDictionaryToolStripMenuItem.Enabled = False
            ISEToolStripMenuItem.Visible = False
            MasterDataDictionaryToolStripMenuItem.Visible = False

            'Programming menu
            'ControlsToolStripMenuItem.Enabled = false
            'ISEModuleToolStripMenuItem.Enabled = False 'AG 21/10/2010
            RegentDisksToolStripMenuItem.Enabled = False
            SampleDisksToolStripMenuItem.Enabled = False
            RegentDisksToolStripMenuItem.Visible = False 'AG 17/12/2010
            SampleDisksToolStripMenuItem.Visible = False 'AG 17/12/2010

            'WorkSession menu
            LIMSImportToolStripMenuItem.Enabled = False
            'LIMSImportToolStripMenuItem.Visible = False 'AG 27/02/2013 - In first LIS version this option is hidden

            'TR 05/04/2013 -this menu option depens if LisWithFilesMode is Enable or disable.
            LIMSImportToolStripMenuItem.Visible = LISWithFilesMode

            'Current state
            'MonitorToolStripMenuItem.Enabled = False 'AG 10/09/10
            'ResultsToolStripMenuItem.Enabled = False 'AG 06/09/2010
            'SampleRotorStatusToolStripMenuItem.Enabled = False 'EF 02/06/2011
            'ReagentRotorStatusToolStripMenuItem.Enabled = False 'EF 02/06/2011
            'ReactionRotorStatusToolStripMenuItem.Enabled = False 'EF 02/06/2011
            'ISEStatusToolStripMenuItem.Enabled = False 'EF 02/06/2011

            'Results
            'SampleResultsToolStripMenuItem.Enabled = false 'AG 04/11/2011
            'BlankCalibratorResultsToolStripMenuItem.Enabled = False 'AG 19/10/2012
            'QCResultsToolStripMenuItem.Enabled = false 'TR 26/05/2011
            'QCCumulatedResultsToolStripMenuItem.Enabled = False
            'ISEResultsToolStripMenuItem.Enabled = False 'JB 26/07/2012
            StatisticsToolStripMenuItem.Enabled = False
            HistoricalDDBBReviewToolStripMenuItem.Enabled = False
            'HistoricalAnalyzerAlarmsReviewToolStripMenuItem.Enabled = False 'JB 28/09/2012
            HistoricalAnalyzerAlarmsReviewToolStripMenuItem.Enabled = HISTWorkingMode 'JB 01/10/2012

            'Reports
            StandardReportToolStripMenuItem.Enabled = False
            CustomReportsToolStripMenuItem.Enabled = False
            PatirentDataToolStripMenuItem.Enabled = False
            QualityControlToolStripMenuItem.Enabled = False

            StandardReportToolStripMenuItem.Visible = False
            CustomReportsToolStripMenuItem.Visible = False
            PatirentDataToolStripMenuItem.Visible = False
            QualityControlToolStripMenuItem.Visible = False

            'Utilities
            'AnalyzerUtilitiesToolStripMenuItem.Enabled = False
            'ChangeReactionsRotorToolStripMenuItem.Enabled = False
            AnalyzerDailyMaintenanceToolStripMenuItem.Enabled = False
            ReagentConsumptionToolStripMenuItem.Enabled = False
            'ISEUtilitiesToolStripMenuItem.Enabled = False
            LISUtilitiesToolStripMenuItem.Enabled = True 'AG 27/02/2013 - comment this line when develop the lis mapping screen
            DataBasesBackUpToolStripMenuItem.Enabled = False
            PreventiveMaintenanceToolStripMenuItem.Enabled = False
            PreventiveMaintenanceToolStripMenuItem.Visible = False 'AG 17/12/2010

            VersionsCheckingToolStripMenuItem.Enabled = False
            RemoteConnectionToolStripMenuItem.Enabled = False
            ReviewFwBlackBoxToolStripMenuItem.Enabled = False
            'LoadSATReportToolStripMenuItem.Enabled = False
            'RestorePreviousDataToolStripMenuItem.Enabled =False 
            'ISEUtilitiesToolStripMenuItem.Visible = False
            ReagentConsumptionToolStripMenuItem.Visible = False

            'Exit
            WithShutToolStripMenuItem.Enabled = True 'False DL 04/06/2012
            WithOutToolStripMenuItem.Enabled = True 'False DL 04/04/2012

            'Help
            'ToolStripMenuItem10.Enabled = True
            ToolStripMenuItem10.Visible = False 'DL 31/10/2012
            'QuickToolStripMenuItem.Enabled = True
            QuickToolStripMenuItem.Visible = False 'DL 31/10/2012

            WhatsNewToolStripMenuItem.Enabled = False
            UserMaintenancePlanToolStripMenuItem.Enabled = False
            TutorialsToolStripMenuItem.Enabled = False
            ApplicationNotesToolStripMenuItem.Enabled = False
            OnlineHelpToolStripMenuItem.Enabled = False
            TroubleshooterToolStripMenuItem.Enabled = False
            BorrameToolStripMenuItem.Visible = True

            InstrumentInfoToolStripMenuItem.Visible = True 'DL 15/11/2012

            ToolStripMenuItem10.Enabled = False
            QuickToolStripMenuItem.Enabled = False

            'AG 05/06/2013 - Fix issue #1137
            HideBIOSYSTEMSOnlyMenuOptions()
            MenuOptionsByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".PrepareMenuOptions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareMenuOptions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Method used for enabling or disabling some menu options and buttons according to active data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 05/26/2010
    ''' Modified by: SA 09/06/2010 - Added Try/Catch
    '''              SA 14/06/2010 - If there is not an Active WS, option for loading a Saved WS has to be enabled  
    '''              SA 16/09/2010 - Added control of availability of menu option for LIMS Import process
    '''              TR 19/09/2011 - Button for opening screen of Rotor Positions is always enabled
    '''              SA 08/03/2012 - Changed conditions to enable/disable options for Load/Save SavedWS; menu option for opening
    '''                              screen of Rotor Positioning has to be always enabled (same as the corresponding button)    '''              
    '''              AG 27/02/2013 - Evaluate the Enabled property for the menu option XXx only if option is visible LIMSImportToolStripMenuItem
    '''              DL 07/05/2013 - Disable LoadSession Menu Item when exist a Order with LIS Request as true
    ''' </remarks>
    Private Sub EnableWSMenu()
        Try
            If (String.IsNullOrEmpty(ActiveWorkSession)) Then
                CurrentSessionToolStripMenuItem.Enabled = False

                'SA 08/03/2012 - 'If there is not an active WS, it is not possible to load a SavedWS, although this situation
                'it is not possible (an empty WS is always created after the current one is reset)
                SaveSessionToolStripMenuItem.Enabled = False
                LoadSessionToolStripMenuItem.Enabled = False

                bsTSResetSessionButton.Enabled = False
            Else
                CurrentSessionToolStripMenuItem.Enabled = True

                'SA 08/03/2012 - Options for Save/Load a SavedWS are enabled depending on the status of the active WS
                If String.Equals(ActiveStatus, "EMPTY") Then
                    SaveSessionToolStripMenuItem.Enabled = False
                    LoadSessionToolStripMenuItem.Enabled = True
                ElseIf String.Equals(ActiveStatus, "OPEN") Then
                    SaveSessionToolStripMenuItem.Enabled = True

                    'DL 07/05/2013. Begin
                    Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                    Dim dataToReturn As New GlobalDataTO

                    dataToReturn = myWSOrderTestsDelegate.CountLISRequestActive(Nothing, AnalyzerIDAttribute)
                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                        Dim numLISRequest As Integer = CType(dataToReturn.SetDatos, Integer)

                        LoadSessionToolStripMenuItem.Enabled = (numLISRequest = 0)
                    End If

                    'LoadSessionToolStripMenuItem.Enabled = True
                    'DL 07/05/2013. End
                Else
                    SaveSessionToolStripMenuItem.Enabled = True
                    LoadSessionToolStripMenuItem.Enabled = False
                End If
            End If

            'Options and buttons that are always enabled
            SampleRequestToolStripMenuItem.Enabled = True
            RotorPositionsToolStripMenuItem.Enabled = True
            DeleteSessionToolStripMenuItem.Enabled = True
            DeleteVirtualRotorsToolStripMenuItem.Enabled = True

            bsTSNewSampleButton.Enabled = True
            bsTSPositionButton.Enabled = True
            bsTSResetSessionButton.Enabled = True

            If LIMSImportToolStripMenuItem.Visible Then 'AG 27/02/2013 - In first LIS version this menu option is hidden
                'Availability of LIMS Import option depends on if the Import File exists in an specific path
                LIMSImportToolStripMenuItem.Enabled = IO.File.Exists(LIMSImportFilePath & LIMS_IMPORT_FILE_NAME)
            End If

            MenuOptionsByUserLevel() 'TR 24/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".EnableWSMenu ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EnableWSMenu ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Method used for closing the active Mdi Child Form
    ''' </summary>
    ''' <remarks>
    ''' Created by RH 09/03/2012
    ''' modificate by: TR 05/04/2013 correct a bug to open monitor form after opening one of the commond form do some modificacition
    '''                              press the button Orders Download and cancel form close, then close the commond form. the monitor for do not open.
    ''' </remarks>
    Private Function CloseActiveMdiChild(Optional ByVal pEvaluateCodeForAutoWSLISMode As Boolean = False) As Boolean
        'For Each mdiChildForm As Form In MdiChildren
        '    If mdiChildForm.Visible Then
        '        mdiChildForm.Close()
        '    End If
        'Next

        Dim IsFormClosed As Boolean = True

        'AG 09/07/2013 - In automatic mode close current screen withno business!!!
        'XB 23/07/2013 - auto HQ
        'If pEvaluateCodeForAutoWSLISMode AndAlso autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        If pEvaluateCodeForAutoWSLISMode AndAlso (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
            'AG + XB 27/07/2013
            Dim MdiChildIsDisabled As Boolean = False
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            If (Not ActiveMdiChild Is Nothing) Then
                ActiveMdiChild.Close()
            ElseIf MdiChildIsDisabled Then
                DisabledMDIChildAttribute(0).Close()
                If DisabledMDIChildAttribute.Count = 1 Then
                    DisabledMDIChildAttribute.Clear()
                End If
            End If
            'AG + XB 27/07/2013
            Return IsFormClosed
        End If
        'AG 09/07/2013

        If (Not ActiveMdiChild Is Nothing) Then
            If TypeOf ActiveMdiChild Is IMonitor Then
                IMonitor.Close()
            Else
                'Execute the closing code of the screen currently open
                Dim myScreenName As String = ActiveMdiChild.Name
                ClosedByFormCloseButton = False
                ActiveMdiChild.Tag = "Performing Click" 'RH 16/12/2010 To tell the form it is closed by hand
                Cursor = Cursors.Default
                ActiveMdiChild.AcceptButton.PerformClick()

                'The screen currently open could not be closed, the opening is cancelled
                If (Not ActiveMdiChild Is Nothing AndAlso String.Compare(ActiveMdiChild.Name, myScreenName, False) = 0) Then
                    IsFormClosed = False
                    ActiveMdiChild.Tag = Nothing
                    'TR 05/04/2013 -Set value to true to avoid bug.
                    ClosedByFormCloseButton = True
                End If
            End If

        End If

        Return IsFormClosed
    End Function

    ''' <summary>
    ''' Get ID and Model of the Analyzer currently active
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 12/04/2010
    ''' Modified by: SA 09/06/2010 - Inform screen properties instead of a global DataSet
    ''' </remarks>
    Private Sub GetAnalyzerInfo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate

            ' XBC 07/06/2012
            'myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(Nothing)

            Dim myLogAccionesAux As New ApplicationLogManager()
            myLogAccionesAux.CreateLogActivity("(Analyzer Change) Check Analyzer ", Name & ".GetAnalyzerInfo ", EventLogEntryType.Information, False)

            myGlobalDataTO = myAnalyzerDelegate.CheckAnalyzer(Nothing)
            ' XBC 07/06/2012

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myAnalyzerData As New AnalyzersDS
                myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

                If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                    'Inform properties AnalyzerID and AnalyzerModel
                    AnalyzerModelAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel.ToString
                    AnalyzerIDAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerID.ToString

                    ' XBC 08/06/2012
                    FwVersionAttribute = myAnalyzerData.tcfgAnalyzers(0).FirmwareVersion.ToString
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetAnalyzerInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAnalyzerInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Activates the LIS action buttons (queryALL, HOSTquery and OrdersDownload)
    ''' </summary>
    ''' <remarks>
    ''' Created     AG 28/03/2013 - empty
    ''' Udpated by: TR 01/03/2013 - implementation.
    '''             XB 23/04/2013 - Another cause to enable OrdersDownloadButton validate if there is at least one Saved WS From LIS
    '''             AG 25/04/2013 - Move the activation rules for ES action into ESBusiness class
    '''             TR 02/05/2013 - Change from private to public to implement  in lis utilities form.
    ''' </remarks>
    Public Sub ActivateLISActionButton()
        Try
            If Not MDIAnalyzerManager Is Nothing AndAlso Not MDILISManager Is Nothing Then

                Dim isEnable As Boolean = False
                Dim myESRulesDlg As New ESBusiness
                Dim runningFlag As Boolean = CBool(IIf(MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
                Dim connectingFlag As Boolean = CBool(IIf(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))

                'Set the enable value.
                bsTSQueryAllButton.Enabled = myESRulesDlg.AllowLISAction(Nothing, LISActions.QueryAll_MDIButton, runningFlag, connectingFlag, MDILISManager.Status, MDILISManager.Storage)
                bsTSHostQueryButton.Enabled = myESRulesDlg.AllowLISAction(Nothing, LISActions.HostQuery_MDIButton, runningFlag, connectingFlag, MDILISManager.Status, MDILISManager.Storage)

                PendingOrders = 0 'AG 02/05/2013 - Set to 0 because they are recalculated

                bsTSOrdersDownloadButton.Enabled = myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, runningFlag, connectingFlag, MDILISManager.Status, MDILISManager.Storage, PendingOrders)

                ' XB 01/08/2013 - add functionality according to disable LIS buttons
                If DisableLISButtons() Then
                    bsTSQueryAllButton.Enabled = False
                    bsTSHostQueryButton.Enabled = False
                    bsTSOrdersDownloadButton.Enabled = False
                End If

                'AG 13/05/2013 - Moved to AllowLISAction
                ''DL 29/04/2013. BEGIN
                'Dim myGlobalDataTO As New GlobalDataTO
                'Dim mySavedWSDelegate As New SavedWSDelegate

                'myGlobalDataTO = mySavedWSDelegate.ReadLISSavedWS(Nothing)
                'If Not myGlobalDataTO.HasError Then
                '    Dim mySavedWS As New SavedWSDS
                '    mySavedWS = DirectCast(myGlobalDataTO.SetDatos, SavedWSDS)

                '    If Not myGlobalDataTO.SetDatos Is Nothing AndAlso mySavedWS.tparSavedWS.Rows.Count > 0 Then
                '        SavedOrdesfromLIMS = (From a In mySavedWS.tparSavedWS Where a.FromLIMS = True Select a).Count
                '    Else
                'SavedOrdesfromLIMS = 0
                '    End If
                'End If
                'AG 13/05/2013 - DL 29/04/2013. END 


                'JC 29/05/2013 Ordres Download Count, return more Orders Pending than real, because Synapse Driver Generates fake Orders.
                '              For this reason has been decided disable the tooltip with Orders Pending Count
                'If bsTSOrdersDownloadButton.Enabled Then
                '    'DL 29/04/2013 Add SavedOrdersfromLIMS
                '    bsTSOrdersDownloadButton.ToolTipText = PendingOrdersToolTip + " " + PendingOrders.ToString
                'Else
                bsTSOrdersDownloadButton.ToolTipText = NoPendingOrdersToolTip
                'End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ActivateLISActionButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ActivateLISActionButton", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Activates or not the vertical buttons (action buttons) depending the 
    ''' analyzer state (free or busy), the analyzer status, current alarms 
    ''' and so on
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 (Tested)
    ''' Modified by: AG 16/11/2010 - define as public method to call it from WS Rotor Positions
    '''              XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
    '''              AG 06/11/2013 - Task #1375
    ''' </remarks>
    Private Sub ActivateActionButtonBarOrSendNewAction()
        Try
            Dim myGlobal As New GlobalDataTO
            If Not MDIAnalyzerManager Is Nothing Then
                If Not MDIAnalyzerManager.classInitializationError Then

                    '1st differentiate if connection established or not
                    Dim myConnected As Boolean = False
                    If MDIAnalyzerManager.CommThreadsStarted Then myConnected = MDIAnalyzerManager.Connected

                    'Me.SetActionButtonsEnableProperty(False)    'All buttons are already disable. We only need to active the required ones

                    If Not myConnected Then 'Only button Connect enabled
                        SetActionButtonsEnableProperty(False) 'AG 09/05/2012 - Disable all buttons
                        bsTSConnectButton.Enabled = True 'But not the Connect button

                        'ShowStatus(Messages.NO_CONNECTED)
                        If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "INPROCESS", False) = 0 Then
                            ShowStatus(Messages.CONNECTING)
                        Else
                            ShowStatus(Messages.NO_CONNECTED)
                        End If

                        SetEnableMainTab(False) 'RH 27/03/2012

                    Else
                        'Activate or deactivate buttons depending the analyzer status and if it's ready

                        ' AG+XBC 24/05/2012
                        'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                        Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
                        ' AG+XBC 24/05/2012

                        Dim myAx00Ready As Boolean = True
                        Dim myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions
                        Dim myAx00InstructionReceived As GlobalEnumerates.AnalyzerManagerSwActionList
                        Dim myAx00CurrentAlarms As New List(Of GlobalEnumerates.Alarms) 'AG 01/12/11

                        myAx00Status = MDIAnalyzerManager.AnalyzerStatus
                        myAx00Ready = MDIAnalyzerManager.AnalyzerIsReady
                        myAx00Action = MDIAnalyzerManager.AnalyzerCurrentAction
                        myAx00InstructionReceived = MDIAnalyzerManager.InstructionTypeReceived
                        myAx00CurrentAlarms = MDIAnalyzerManager.Alarms 'AG 01/12/11

                        'AG 31/03/2011 - If analyzer not is in FREEZE mode take care about status, alarms, user actions,...
                        If Not MDIAnalyzerManager.AnalyzerIsFreeze Then
                            bsTSConnectButton.Enabled = False 'AG 12/03/2012
                            bsTSRecover.Enabled = False 'AG 12/03/2012

                            'AG 20/06/2012 - If analyzer is in STANDBY  but there are no alight results and start instrument is NOT IN PROCESS -> activate button Start instrument 
                            If myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso Not MDIAnalyzerManager.ExistsALIGHT AndAlso _
                            (String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) <> 0 AndAlso String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) <> 0) Then
                                'Means we are in Standby from Service Sw or we have restore database and connected with an analyzer already started
                                'Solution: Repeat the Start Instrument Process

                                'Active only the start instrument button
                                SetActionButtonsEnableProperty(False)

                                If myAx00Ready Then
                                    ' XBC 04/07/2012
                                    If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                                        If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" Then
                                            If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso Not MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                                                If Not isStartInstrumentActivating Then

                                                    bsTSStartInstrumentButton.Enabled = True
                                                    bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT) 'AG 12/03/2012
                                                    WarmUpFinishedAttribute = False

                                                    ' XBC 04/07/2012
                                                    If Not ActiveMdiChild Is Nothing Then
                                                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                                                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                                                            CurrentMdiChild.TimeWarmUpProgressBar.Position = 0
                                                            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                                                            ErrorStatusLabel.Text = GetMessageText(GlobalEnumerates.Messages.ALIGHT_REQUIRED.ToString)
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If


                                End If

                            Else 'Apply the normal business: Select cases

                                Select Case myAx00Status
                                    Case GlobalEnumerates.AnalyzerManagerStatus.MONITOR 'All buttons disable
                                        'TR 15/05/2012 -Stop the worksession timer
                                        If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()
                                        'Already done (all disable)

                                    Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING  'Only Start instrument button enabled if analyzer is ready 
                                        'TR 15/05/2012 -Stop the worksession timer
                                        If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()

                                        'Instrument buttons
                                        If myAx00Ready Then
                                            If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then ' XBC 06/09/2012 - Fix : button is enabled while Connect process is in in process
                                                bsTSStartInstrumentButton.Enabled = True
                                                bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT) 'AG 12/03/2012
                                            End If
                                            WarmUpFinishedAttribute = False
                                        End If

                                    Case GlobalEnumerates.AnalyzerManagerStatus.STANDBY
                                        'TR 15/05/2012 -Stop the worksession timer
                                        'If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop() TR 18/052012 commented

                                        'AG 23/03/2012 - ISE initialitation not finished (connection in StandBy) 
                                        If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "INPROCESS", False) = 0 Then
                                            'Do nothing

                                            'AG 04/09/2012 - ABORT session button:
                                            'Recovery results starts in Running and allow Abort but it must be disabled once the analyzer becomes in StandBy
                                            'Note that recovery results will be closed once the complete connection finishes
                                            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 AndAlso bsTSAbortSessionButton.Enabled Then
                                                bsTSAbortSessionButton.Enabled = False
                                            End If
                                            'AG 04/09/2012

                                        ElseIf MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                                            '    'Do nothing, no activate buttons until the ISE initialitazion finishes

                                        ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 Then
                                            'AG 27/08/2012 - STANDBY: All buttons Disabled | RUNNING: Abort button enabled
                                            SetActionButtonsEnableProperty(False)

                                        Else

                                            'Recover wash NOT in process, and analyzer is in active mode
                                            'If Abort NOT in process
                                            If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) <> "INPROCESS" AndAlso _
                                               MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) <> "PAUSED" AndAlso _
                                               MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) <> "INPROCESS" AndAlso _
                                               MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) <> "PAUSED" Then

                                                'AG 30/09/2011 - check if warm up has been canceled ... then activate start instrument and shut down
                                                Dim warmUpPaused As Boolean = False
                                                Dim abortPaused As Boolean = False
                                                Dim shutDownPaused As Boolean = False
                                                If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED" AndAlso _
                                                   (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED" OrElse _
                                                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED", False) = 0) Then
                                                    warmUpPaused = True
                                                ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) = 0 AndAlso _
                                                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED", False) = 0 Then
                                                    abortPaused = True
                                                ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "PAUSED", False) = 0 Then
                                                    shutDownPaused = True
                                                End If
                                                'AG 30/09/2011

                                                'AG 12/09/2011 - check if instrument has finished his warm up process
                                                If Not WarmUpFinishedAttribute AndAlso Not warmUpPaused Then
                                                    'Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                                                    Dim myAnalyzerSettingsDS As AnalyzerSettingsDS
                                                    'Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                                                    myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                           AnalyzerIDAttribute, _
                                                                           GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

                                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                        myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                                                        If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                                            Dim myDate As String = ""
                                                            myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue
                                                            If String.Compare(myDate, "", False) = 0 Then
                                                                WarmUpFinishedAttribute = True

                                                                'AG 22/03/2012
                                                                If MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 Then
                                                                    MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                                                                    MyClass.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 - Ise alarms ready to be shown
                                                                End If
                                                                'AG 22/03/2012

                                                            Else
                                                                If Not BsTimerWUp.Enabled Then BsTimerWUp.Enabled = True
                                                            End If
                                                        End If
                                                    End If

                                                    'AG 14/03/2012- If not finished, not paused, not in process and analyzer ready maybe it has been aborted by freeze
                                                    If Not WarmUpFinished AndAlso Not warmUpPaused AndAlso myAx00Ready AndAlso _
                                                       MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "" Then
                                                        myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                   AnalyzerIDAttribute, _
                                                                   GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString())

                                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                            myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                                                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                                                If myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue = "0" Then
                                                                    warmUpPaused = True
                                                                End If
                                                            End If
                                                        End If

                                                    End If
                                                    'AG 14/03/2012

                                                End If

                                                'If myAx00Ready Then
                                                Dim enableChangeBottlesConfirmFlag As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                                                If myAx00Ready AndAlso warmUpPaused Then
                                                    bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN) 'AG 14/05/2012 True 'AG - shut down with out washings
                                                    WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                                                    bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                                                    'AG 12/03/2012
                                                    'bsTSStartInstrumentButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                                                    If Not bsTSChangeBottlesConfirm.Enabled Then
                                                        bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT)
                                                    Else
                                                        bsTSStartInstrumentButton.Enabled = False
                                                    End If
                                                    'AG 12/03/2012

                                                    WarmUpFinishedAttribute = False

                                                ElseIf myAx00Ready AndAlso abortPaused Then
                                                    bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                                                    'AG 12/03/2012 - 'When abort paused user can not shut down because there is a conditioning wash pending
                                                    'bsTSShutdownButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                                                    If Not bsTSChangeBottlesConfirm.Enabled Then
                                                        bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                                                    Else
                                                        bsTSShutdownButton.Enabled = False
                                                    End If
                                                    'AG 12/03/2012
                                                    WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012


                                                ElseIf myAx00Ready AndAlso shutDownPaused Then
                                                    bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                                                    'AG 12/03/2012
                                                    'bsTSShutdownButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                                                    If Not bsTSChangeBottlesConfirm.Enabled Then
                                                        bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                                                    Else
                                                        bsTSShutdownButton.Enabled = False
                                                    End If

                                                    WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                                                    'AG 12/03/2012


                                                ElseIf myAx00Ready AndAlso WarmUpFinishedAttribute AndAlso Not warmUpPaused Then

                                                    'XB 23/07/2013 - auto HQ
                                                    'If autoWSCreationWithLISModeAttribute AndAlso (automateProcessCurrentState <> LISautomateProcessSteps.notStarted _
                                                    '                                               AndAlso automateProcessCurrentState <> LISautomateProcessSteps.ExitAutomaticProcesAndStop) Then
                                                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) _
                                                                    AndAlso (automateProcessCurrentState <> LISautomateProcessSteps.notStarted _
                                                                    AndAlso automateProcessCurrentState <> LISautomateProcessSteps.ExitAutomaticProcesAndStop) Then
                                                        'XB 23/07/2013

                                                        'Automate process in course (PAUSE enabled)
                                                        'bsTSStartSessionButton.Enabled = False

                                                        'JV + AG 18/10/2013 revision task # 1341
                                                        'bsTSMultiFunctionSessionButton.Image = imagePause
                                                        showSTARTWSiconFlag = False
                                                        'JV + AG 18/10/2013 revision task # 1341

                                                        'AG 19/07/2013 v2.1.1- when enter in running the pause button must remain disabled until BA400 becomes in Running or until the process fails
                                                        '                else evaluate same condition as previously
                                                        'If Not pausingAutomateProcessFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                                                        If automateProcessCurrentState <> LISautomateProcessSteps.subProcessEnterRunning Then
                                                            'AG 30/07/2013 - During the process automatic for HQ the pause button is disabled
                                                            'If Not pausingAutomateProcessFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                                                            'If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                                                            If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then showPAUSEWSiconFlag = True Else showPAUSEWSiconFlag = False 'JV + AG 18/10/2013 revision task # 1341
                                                        Else
                                                            'bsTSPauseSessionButton.Enabled = False
                                                            'JV + AG 18/10/2013 revision task # 1341
                                                            'bsTSMultiFunctionSessionButton.Image = imagePlay
                                                            showPAUSEWSiconFlag = False
                                                            'JV + AG 18/10/2013 revision task # 1341
                                                        End If

                                                        'bsTSContinueSessionButton.Enabled = False
                                                        'JV + AG 18/10/2013 - revision task # 1341
                                                        'bsTSMultiFunctionSessionButton.Image = imagePause
                                                        showSTARTWSiconFlag = False
                                                        'JV + AG 18/10/2013 task # 1341

                                                    Else 'Move here the code previous to the automate Creation of WS with LIS mode
                                                        'AG 12/09/2011
                                                        'Instrument buttons
                                                        'AG 12/03/2012
                                                        'bsTSShutdownButton.Enabled = True
                                                        bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                                                        WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                                                        'AG 12/03/2012

                                                        bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag
                                                        'bsTSPauseSessionButton.Enabled = False
                                                        'JV + AG 18/10/2013 revision task # 1341
                                                        'bsTSMultiFunctionSessionButton.Image = imagePlay
                                                        showPAUSEWSiconFlag = False
                                                        'JV + AG 18/10/2013 task # 1341
                                                        bsTSAbortSessionButton.Enabled = False

                                                        'Session buttons depending if exist PendingExecutionsLeft or not (AG 09/03/2012 add condition WS not aborted
                                                        If String.Compare(WorkSessionIDAttribute, "", False) <> 0 AndAlso AnalyzerIDAttribute <> "" AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) <> 0 _
                                                          AndAlso String.Compare(WSStatusAttribute, "ABORTED", False) <> 0 AndAlso myAx00Action <> AnalyzerManagerAx00Actions.RUNNING_START Then
                                                            Dim myWSDelegate As New WorkSessionsDelegate
                                                            Dim pendingExecutionsLeft As Boolean = False
                                                            Dim executionsNumber As Integer = 0
                                                            myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)

                                                            If executionsNumber > 0 Then
                                                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                                    Dim wsStarted As Boolean = CType(myGlobal.SetDatos, Boolean)

                                                                    ' XB 29/10/2013 - Enable START_WS if there are only ISE preparations pending but locked by any required calibration - BT #1070
                                                                    If Not pendingExecutionsLeft Then
                                                                        pendingExecutionsLeft = IsTherePendingOnlyISEExecutions()
                                                                    End If
                                                                    ' XB 29/10/2013

                                                                    If Not wsStarted AndAlso pendingExecutionsLeft Then
                                                                        'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                        Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                        showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                    ElseIf pendingExecutionsLeft Then
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                        'bsTSContinueSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                                        Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                                        showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                    Else 'All Executions pending are paused or locked
                                                                        'bsTSStartSessionButton.Enabled = False
                                                                        'bsTSContinueSessionButton.Enabled = False
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                        'bsTSMultiFunctionSessionButton.Image = imagePause
                                                                        showSTARTWSiconFlag = False
                                                                        'JV + AG 18/10/2013 revision task # 1341
                                                                        'AG 09/07/2013
                                                                        'XB 23/07/2013 - auto HQ
                                                                        'If autoWSCreationWithLISModeAttribute Then
                                                                        If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                                                                            'XB 23/07/2013
                                                                            If Not wsStarted Then
                                                                                'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                                'JV + AG 18/10/2013 revision task # 1341
                                                                                Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                                'If bReturn Then bsTSMultiFunctionSessionButton.Image = imagePlay Else bsTSMultiFunctionSessionButton.Image = imagePause
                                                                                showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                                'JV + AG 18/10/2013 revision task # 1341
                                                                            Else
                                                                                'JV + AG 18/10/2013 revision task # 1341
                                                                                'bsTSContinueSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                                                Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                                                showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                                'JV + AG 18/10/2013 revision task # 1341
                                                                            End If
                                                                        End If
                                                                        'AG 09/07/2013
                                                                    End If
                                                                End If

                                                                'AG 08/07/2013 - Special code for auto WS creation with LIS
                                                            Else
                                                                'XB 23/07/2013 - auto HQ
                                                                'If autoWSCreationWithLISModeAttribute Then
                                                                If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                                                                    'XB 23/07/2013
                                                                    'JV + AG 18/10/2013 revision task # 1341
                                                                    'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                    Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                    showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                    'JV + AG 18/10/2013 revision task # 1341
                                                                End If
                                                                'AG 08/07/2013
                                                            End If

                                                        Else
                                                            'bsTSStartSessionButton.Enabled = False
                                                            'bsTSContinueSessionButton.Enabled = False
                                                            'AG 18/10/2013 revision
                                                            'bsTSMultiFunctionSessionButton.Image = imagePause
                                                            showSTARTWSiconFlag = False
                                                            'AG 18/10/2013 revision
                                                            'AG 08/07/2013 - Special code for auto WS creation with LIS
                                                            'XB 23/07/2013 - auto HQ
                                                            ' If autoWSCreationWithLISModeAttribute AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) = 0 Then
                                                            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) = 0 Then
                                                                'XB 23/07/2013
                                                                'JV + AG 18/10/2013 revision task # 1341
                                                                'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                                showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                                                'JV + AG 18/10/2013 revision task # 1341
                                                            End If
                                                            'AG 08/07/2013 

                                                        End If 'If WorkSessionIDAttribute <> "" And AnalyzerIDAttribute <> "" Then

                                                    End If
                                                    'TODO
                                                    'AG 14/05/2012
                                                    'Ax00 is ready and wup no ha acabado y boton FinWup visible
                                                ElseIf myAx00Ready AndAlso Not WarmUpFinishedAttribute Then
                                                    bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag 'AG 23/07/2012

                                                    If ActiveMdiChild Is IMonitor Then
                                                        Dim myMonitor As IMonitor = CType(ActiveMdiChild, IMonitor)
                                                        If myMonitor.IsEndWarmUpButtonVisible Then
                                                            bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN) 'AG 14/05/2012 True 
                                                            WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                                                        End If
                                                    End If

                                                End If 'If myAx00Ready Then


                                                'AG 03/10/2011 - Buttons over analyzer not in Vertical Buttons Bar
                                                'Barcode button
                                                Dim bcButtonStatus As Boolean = False 'bar code button status
                                                Dim sensorValue As Single = MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)

                                                If myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso myAx00Ready AndAlso _
                                                    MDIAnalyzerManager.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE AndAlso _
                                                    sensorValue = 1 Then
                                                    bcButtonStatus = ActivateButtonWithAlarms(ActionButton.READ_BARCODE) 'AG 28/03/2012 - buttonStatus = True
                                                End If

                                                'Similar business for check bottle volume (in RotorPositions screen) but using (ActionButton.CHECK_BOTTLE_VOLUME)
                                                Dim cvbButtonStatus As Boolean = False 'check volume bottle button status
                                                'TODO


                                                If Not ActiveMdiChild Is Nothing Then
                                                    Dim analyzerSettings As New AnalyzerSettingsDelegate
                                                    Dim resultData As New GlobalDataTO

                                                    If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                                        Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                                                        If Not CurrentMdiChild.bsScanningButton Is Nothing AndAlso CurrentMdiChild.bsScanningButton.Enabled <> bcButtonStatus Then
                                                            'CurrentMdiChild.bsScanningButton.Enabled = buttonStatus
                                                            If bcButtonStatus Then
                                                                'A part from Analyzer status check if barcode reader is enabled
                                                                If String.Compare(CurrentMdiChild.CurrentRotorType, "SAMPLES", False) = 0 Then
                                                                    resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                                                                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                                        Dim myDataSet As New AnalyzerSettingsDS
                                                                        myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                                                        If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                                                            If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                                                        End If
                                                                    End If

                                                                Else
                                                                    resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
                                                                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                                        Dim myDataSet As New AnalyzerSettingsDS
                                                                        myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                                                        If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                                                            If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                                                        End If
                                                                    End If

                                                                End If
                                                            End If

                                                            CurrentMdiChild.bsScanningButton.Enabled = bcButtonStatus
                                                        End If

                                                        'AG 28/03/2012 - check bottle volume (only for reagents)
                                                        If Not CurrentMdiChild.bsCheckRotorVolumeButton Is Nothing AndAlso CurrentMdiChild.bsCheckRotorVolumeButton.Enabled <> cvbButtonStatus Then
                                                            'TODO

                                                            CurrentMdiChild.bsCheckRotorVolumeButton.Enabled = cvbButtonStatus
                                                            CurrentMdiChild.bsReagentsCheckVolumePosButton.Enabled = cvbButtonStatus
                                                            'CurrentMdiChild.bsSamplesCheckVolumePosButton.Enabled = False 'Always disabled for samples
                                                        End If
                                                        'AG 28/03/2012

                                                    ElseIf (TypeOf ActiveMdiChild Is IWSSampleRequest) Then
                                                        Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                                                        If Not CurrentMdiChild.bsScanningButton Is Nothing AndAlso CurrentMdiChild.bsScanningButton.Enabled <> bcButtonStatus Then
                                                            If bcButtonStatus Then
                                                                'A part from Analyzer status check if barcode reader is enabled
                                                                resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                                                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                                                    Dim myDataSet As New AnalyzerSettingsDS
                                                                    myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                                                    If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                                                        If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                                                    End If
                                                                End If
                                                            End If

                                                            CurrentMdiChild.bsScanningButton.Enabled = bcButtonStatus
                                                        End If

                                                        'AG 22/02/2012 - Update button in change rotor utility
                                                    ElseIf (TypeOf ActiveMdiChild Is IChangeRotor) Then
                                                        Dim CurrentMdiChild As IChangeRotor = CType(ActiveMdiChild, IChangeRotor)
                                                        If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess), "PAUSED", False) = 0 Then
                                                            bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag
                                                            'AG 28/03/2012
                                                            'CurrentMdiChild.bsChangeRotortButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                                                            If Not bsTSChangeBottlesConfirm.Enabled Then
                                                                CurrentMdiChild.bsChangeRotortButton.Enabled = ActivateButtonWithAlarms(ActionButton.CHANGE_REACTIONS_ROTOR)
                                                            End If

                                                        End If
                                                        'AG 22/02/2012

                                                    End If
                                                End If

                                                'TODO
                                                'bsCheckRotorVolumeButton, bsReagentsCheckVolumePosButton, bsSamplesCheckVolumePosButton
                                                'AG 03/10/2011

                                            Else 'If recover wash in process
                                                If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "PAUSED", False) = 0 OrElse _
                                                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) = 0 Then
                                                    'Activate the change bottle confirm
                                                    bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                                                End If
                                            End If

                                        End If 'AG 23/03/2012 - ISE initialitation not finished (connection in StandBy)


                                    Case GlobalEnumerates.AnalyzerManagerStatus.RUNNING    'Only Wash, Pause and Abort buttons enabled

                                        ' XB 31/10/2013
                                        bsTSStartInstrumentButton.Enabled = False
                                        bsTSShutdownButton.Enabled = False

                                        If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 Then
                                            'AG 27/08/2012 - STANDBY: All buttons Disabled | RUNNING: Abort button enabled
                                            SetActionButtonsEnableProperty(False)
                                            If MDIAnalyzerManager.AbortInstructionSent OrElse _
                                               String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") Then
                                                bsTSAbortSessionButton.Enabled = False

                                                'When analyzer finishes readings and the recovery results instructions flow starts disabled Abort button
                                                'AG 04/12/2013 - BT #1397 while analyzer is scanning the abort is not available too
                                            ElseIf MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.END_RUN_END OrElse _
                                                   MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.NO_ACTION OrElse _
                                                   MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED OrElse _
                                                   MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.POLLRD_RECEIVED Then
                                                bsTSAbortSessionButton.Enabled = False

                                                'Analyzer still perform readings
                                            Else
                                                bsTSAbortSessionButton.Enabled = True
                                            End If

                                        Else
                                            'AG 21/03/2012 (add abort condition) + AG 02/09/2011
                                            If (WSStatusAttribute <> "ABORTED") AndAlso (WSStatusAttribute <> "INPROCESS" OrElse Not ExecuteSessionAttribute) Then
                                                If Not MDIAnalyzerManager.AbortInstructionSent Then
                                                    SetWorkSessionUserCommandFlags(WorkSessionUserActions.START_WS)
                                                    WSStatusAttribute = "INPROCESS"
                                                Else
                                                    WSStatusAttribute = "ABORTED"
                                                    ElapsedTimeTimer.Stop() 'Stop the elapsed time timer
                                                End If
                                            End If
                                            'AG 02/09/2011

                                            'AG 24/02/2012 - when enter in running mark process before running finished
                                            'AG 13/02/2012 - Execute only when Sw enters in Running mode
                                            'If processBeforeRunningFinished = "0" AndAlso MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 1 Then
                                            '    MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0 'clear sensor
                                            '    processBeforeRunningFinished = "1" 'Finished ok
                                            'End If

                                            'AG 05/11/2013 - Task #1375 add new conditions required for the paused mode (in Running)
                                            'If String.Compare(processingBeforeRunning, "0", False) = 0 Then
                                            Dim finishFlag As Boolean = True
                                            If MDIAnalyzerManager.AllowScanInRunning Then
                                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.subProcessEnterRunning Then
                                                    finishFlag = False
                                                End If
                                            End If
                                            If String.Compare(processingBeforeRunning, "0", False) = 0 AndAlso finishFlag Then
                                                'AG 05/11/2013 - Task #1375

                                                processingBeforeRunning = "1" 'Finished ok
                                                ScreenWorkingProcess = False
                                            End If
                                            'AG 24/02/2012

                                            'Instrument buttons
                                            'bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.REFILL_WASH_BOTTLE)
                                            bsTSChangeBottlesConfirm.Enabled = False
                                            '...
                                            '...

                                            'Session buttons
                                            'bsTSStartSessionButton.Enabled = False
                                            'JV + AG 18/10/2013 revision task # 1341
                                            'bsTSMultiFunctionSessionButton.Image = imagePause
                                            showSTARTWSiconFlag = False
                                            'JV + AG 18/10/2013 task # 1341

                                            'AG + XB 15/10/2013 - BT #1333
                                            'bsTSContinueSessionButton.Enabled = False
                                            'bsTSContinueSessionButton.Enabled = MDIAnalyzerManager.AllowScanInRunning AndAlso Not MDIAnalyzerManager.ContinueAlreadySentFlag And _
                                            '                                    Not (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS")
                                            'JV + AG 18/10/2013 revision task # 1341
                                            Dim bReturn As Boolean = MDIAnalyzerManager.AllowScanInRunning AndAlso Not MDIAnalyzerManager.ContinueAlreadySentFlag And _
                                                                                Not (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS")
                                            showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                                            'JV + AG 18/10/2013 task # 1341


                                            'AG 21/12/2011
                                            'Option A: Buttons PAUSE & ABORT become enabled when analyzer enter in running mode
                                            'Are disabled when user press action button or when Sw sends instruction because an alarm

                                            ' XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - BT #1333
                                            'Pause disable when pause or abort requested
                                            'If (MDIAnalyzerManager.EndRunInstructionSent OrElse _
                                            '    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess), "INPROCESS") OrElse _
                                            '    MDIAnalyzerManager.AbortInstructionSent OrElse _
                                            '    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) Then bsTSPauseSessionButton.Enabled = False Else bsTSPauseSessionButton.Enabled = True
                                            If (MDIAnalyzerManager.PauseInstructionSent OrElse _
                                                Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess), "") OrElse _
                                                MDIAnalyzerManager.AbortInstructionSent OrElse _
                                                String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) _
                                                Then showPAUSEWSiconFlag = False Else showPAUSEWSiconFlag = True 'bsTSPauseSessionButton.Enabled = True String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) Then bsTSPauseSessionButton.Enabled = False Else bsTSPauseSessionButton.Enabled = True
                                            ' XB 15/10/2013 - BT #1333

                                            'AG 05/11/2013 - Task #1375 - new activation rule. In running paused mode once the STARTWS button has been clicked
                                            'during the automatic WS creation with LIS the active button must be the PAUSEWS not the STARTWS
                                            If showSTARTWSiconFlag AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                                    showSTARTWSiconFlag = False
                                                    'showPAUSEWSiconFlag = True
                                                    If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then showPAUSEWSiconFlag = True Else showPAUSEWSiconFlag = False

                                                    'AG 06/11/2013 - Task #1375 another new condition in paused mode: User goes to rotor position screen and read barcode -> STARTWS button disabled
                                                ElseIf isSinglecanningRequestedAttribute Then
                                                    showSTARTWSiconFlag = False

                                                    'AG 12/11/2013 - Task #1375 :Else read barcode before running functionality
                                                    'AG 19/11/2013 - Task #1396-e (add also flag StartRunning)
                                                ElseIf String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess), "INPROCESS") OrElse _
                                                    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS") OrElse _
                                                    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.StartRunning), "INI") Then
                                                    showSTARTWSiconFlag = False

                                                End If
                                            End If
                                            'AG 05/11/2013

                                            'Abort disable when abort requested
                                            If MDIAnalyzerManager.AbortInstructionSent OrElse _
                                               String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") Then
                                                bsTSAbortSessionButton.Enabled = False
                                            Else
                                                'AG 06/11/2013 - Task #1375 - new activation rule. In running paused mode once the STARTWS button has been clicked
                                                'during the automatic WS creation with LIS the abort WS button must be disabled
                                                'bsTSAbortSessionButton.Enabled = True
                                                Dim enabledValue As Boolean = True 'By default ABORT is enabled in running but when in pause mode Sw has started the autoCreation WS with LIS process
                                                If MDIAnalyzerManager.AllowScanInRunning Then
                                                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                                        enabledValue = False

                                                        'Task #1375 another new condition in paused mode: User goes to rotor position screen and read barcode -> STARTWS and ABORTWS buttons disabled
                                                    ElseIf isSinglecanningRequestedAttribute Then
                                                        enabledValue = False

                                                        'AG 12/11/2013 - Task #1375 :Else read barcode before running functionality
                                                    ElseIf String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess), "INPROCESS") OrElse _
                                                        String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS") Then
                                                        enabledValue = False

                                                    End If
                                                End If
                                                bsTSAbortSessionButton.Enabled = enabledValue
                                                'AG 06/11/2013

                                            End If


                                            'Session management ... when new executions are added in Running (for instance repetitions) and the A400;END  or ABORT has
                                            'been already sent (but not using the PAUSE or ABORT button) Sw has to send the A400;START; instruction

                                            'AG 06/03/2012 - remove condition "AndAlso myAx00InstructionReceived = AnalyzerManagerSwActionList.STATUS_RECEIVED" because now the reception event is triggered
                                            'only with the photometric results instruction
                                            'NOTE: (AG 06/11/2013) - Next code wont be executed (that's OK) in pause mode because in paused mode EndRunInstructionSent = FALSE!!!
                                            'AG 19/11/2013 - #1396-d Do not start automatically in pause mode (condition: 'Not MDIAnalyzerManager.AllowScanInRunning')
                                            If Not MDIAnalyzerManager.AllowScanInRunning AndAlso ExecuteSessionAttribute AndAlso MDIAnalyzerManager.EndRunInstructionSent AndAlso _
                                               Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess), "INPROCESS") AndAlso _
                                               Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") Then

                                                'AG 30/11/2011 - check if no exists alarms whose treatment in RUNNING is sent ENDRUN instruction
                                                Dim endRunDueAlarms As Boolean = False 'This variable takes TRUE value when exists alarms whose running treatment is send ENDRUN
                                                endRunDueAlarms = MDIAnalyzerManager.ExistSomeAlarmThatRequiresStopWS() 'endRunDueAlarms = ExistSomeAlarmThatRequiresSendENDRUN(myAx00CurrentAlarms)

                                                If Not endRunDueAlarms AndAlso String.Compare(WorkSessionIDAttribute, "", False) <> 0 AndAlso Not String.Equals(AnalyzerIDAttribute, String.Empty) Then
                                                    Dim myWSDelegate As New WorkSessionsDelegate
                                                    Dim pendingExecutionsLeft As Boolean = False
                                                    Dim executionsNumber As Integer = 0
                                                    myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)
                                                    If pendingExecutionsLeft Then
                                                        myGlobal = MDIAnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.START, True, Nothing, Nothing, Nothing, Nothing)
                                                    End If
                                                End If 'If WorkSessionIDAttribute <> "" And AnalyzerIDAttribute <> "" Then
                                            End If 'If myAx00Action = AnalyzerManagerAx00Actions.END_RUN_START Then
                                        End If

                                    Case Else
                                End Select
                                'JV + AG revision 18/10/2013 task # 1341
                                ChangeStatusImageMultipleSessionButton()
                                'JV + AG revision 18/10/2013 task # 1341
                            End If 'AG 20/06/2012

                        Else
                            'Stop the worksession timer
                            If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()
                            If BsTimerWUp.Enabled Then BsTimerWUp.Enabled = False
                            flagExitWithShutDown = False 'DL 04/06/2012

                            'Freeze RESET leave only connect button
                            If String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "RESET") Then
                                SetActionButtonsEnableProperty(False)
                                bsTSConnectButton.Enabled = True

                                'If analyzer is in FREEZE mode recover and connect buttons are enabled in standy (when freeze mode <> AUTOrecover)
                            ElseIf myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso _
                                   Not String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "AUTO") AndAlso _
                                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "INPROCESS") AndAlso _
                                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "PAUSED") Then

                                'bsTSPauseSessionButton.Enabled = False 'AG 01/10/2012 - force disable this button
                                'JV + AG 18/10/2013 task # 1341
                                'bsTSMultiFunctionSessionButton.Image = imagePlay
                                showPAUSEWSiconFlag = False
                                'JV + AG 18/10/2013 task # 1341
                                bsTSAbortSessionButton.Enabled = False 'AG 01/10/2012 - force disable this button
                                'bsTSConnectButton.Enabled = True    ' XB 07/11/2013 - Don't enable Connect button, just  Recover button must Enable - BT #1155
                                bsTSRecover.Enabled = True

                            ElseIf String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "AUTO") Then
                                SetActionButtonsEnableProperty(False)
                                'bsTSConnectButton.Enabled = True
                                If myAx00Status <> AnalyzerManagerStatus.RUNNING Then bsTSConnectButton.Enabled = True

                            ElseIf String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "PARTIAL") Then
                                SetActionButtonsEnableProperty(False)
                                'AG 16/05/2012 - when freeze the connect button only if not running
                                'bsTSConnectButton.Enabled = True
                                If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) <> 0 AndAlso _
                                       String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) <> 0 Then

                                        bsTSAbortSessionButton.Enabled = True 'AG 19/04/2012 - Abort (with out wash) is allowed while Partial Freeze

                                    End If
                                Else
                                    bsTSConnectButton.Enabled = True
                                End If

                            Else
                                SetActionButtonsEnableProperty(False)
                                If myAx00Status = AnalyzerManagerStatus.STANDBY Then
                                    bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                                End If
                            End If

                            'Total freeze means WS is aborted
                            If myAx00Status = AnalyzerManagerStatus.RUNNING AndAlso _
                               String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "TOTAL") Then

                                WSStatusAttribute = "ABORTED"
                                'TR 15/05/2012 -Commented because time is previously stopped.
                                'ElapsedTimeTimer.Stop() 'Stop the elapsed time timer
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ActivateActionButtonBarOrSendNewAction ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateActionButtonBarOrSendNewAction, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Check if there are only ISE preparations pending to execute
    ''' </summary>
    ''' <returns>boolean value</returns>
    ''' <remarks>Created by XB 29/10/2013</remarks>
    Private Function IsTherePendingOnlyISEExecutions() As Boolean
        Dim returnValue As Boolean = False
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myOrderTestDelegate As New OrderTestsDelegate
            Dim iseModuleReady As Boolean = True
            myGlobal = myOrderTestDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                If (CType(myGlobal.SetDatos, Boolean)) Then
                    ' There are ISE preparations
                    myGlobal = myOrderTestDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        If Not (CType(myGlobal.SetDatos, Boolean)) Then
                            ' ONLY ISE preparations
                            If (Not MDIAnalyzerManager Is Nothing) Then
                                If Not MDIAnalyzerManager.ISE_Manager Is Nothing AndAlso MDIAnalyzerManager.ISE_Manager.IsISEModuleReady Then
                                    returnValue = True
                                End If
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".IsTherePendingOnlyISEExecutions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " IsTherePendingOnlyISEExecutions, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return returnValue
    End Function


    ''' <summary>
    ''' When some alarms appears the Sw show a warning message to the user  (only 1st time ... when the alarm appears)
    ''' When some sensors changes values Sw must show a warning message to the user (only 1st time ... when the sensors changes the value)
    ''' 
    ''' </summary>
    ''' <param name="pRefreshEvent">If alarms check alarms for show messages, if sensors check sensors for show messages</param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' AG 01/04/2011 - Creation
    ''' AG 09/03/2012 - modified
    ''' </remarks>
    Private Sub ShowAlarmsOrSensorsWarningMessages(ByVal pRefreshEvent As GlobalEnumerates.UI_RefreshEvents, ByVal pRefreshDS As UIRefreshDS)

        Try
            Dim myMessageIDList As New List(Of String)
            Dim messageType As String = Name

            'Messages when some new alarms appears
            If pRefreshEvent = UI_RefreshEvents.ALARMS_RECEIVED Then
                Dim linq As New List(Of UIRefreshDS.ReceivedAlarmsRow)

                '"Reactions Rotor must be changed"
                'Condition: Base line initializations fails
                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                         Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.BASELINE_INIT_ERR.ToString)) _
                           And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    If MDIAnalyzerManager.ShowBaseLineInitializationFailedMessage Then
                        myMessageIDList.Add(GlobalEnumerates.Messages.CHANGE_REACTROTOR_REQUIRED.ToString)
                        messageType = "Error"
                    End If
                End If

                'AG 12/03/2012 - Reactions rotor missing messages
                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                        Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.REACT_MISSING_ERR.ToString.ToString)) _
                        And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    myMessageIDList.Add(GlobalEnumerates.Messages.MISS_REACT_ROTOR.ToString) 'Reactions rotor not found
                    messageType = "Error"

                    'If current screen <> Change rotor add a new message line
                    Dim addMessageLine As Boolean = True
                    If Not ActiveMdiChild Is Nothing Then
                        If (TypeOf ActiveMdiChild Is IChangeRotor) Then addMessageLine = False
                    End If

                    If addMessageLine Then
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.MISS_REACT_ROTOR2.ToString) 'Execute the change rotor utility
                    End If

                End If
                'AG 12/03/2012

                'DL 31/07/2012 
                'FW_CPU_ERR: CPU Firmware Error
                'FW_DISTRIBUTED_ERR: Distributed Firmware Error
                'FW_REPOSITORY_ERR: Repository Firmware Error
                'FW_CHECKSUM_ERR: Checksum Firmware Error
                'FW_MAN_ERR: Maneuvers or Adjustments Firmware Error

                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                        Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CPU_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_REPOSITORY_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_MAN_ERR.ToString)) _
                           And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    myMessageIDList.Add(GlobalEnumerates.Messages.FW_UPDATE.ToString) 'Reactions rotor not found
                    messageType = "Error"
                End If
                'DL 31/07/2012

                'Messages when some sensors change value
            ElseIf pRefreshEvent = UI_RefreshEvents.SENSORVALUE_CHANGED Then

                'AG 07/03/2012 - Moved from ManageReceptionEvent
                Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)

                'AG 30/09/2011 - WarmUp canceled due bottle alarms
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then
                        'Message ) Wup aborted due bottle alars - "You must solve the level alarms in bottle and tank before continue"
                        'ShowAlarmWarningMessages(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED, copyRefreshDS) 'DL 16/09/2011 ShowAlarmWarningMessages(pRefreshDS)
                        If String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED") OrElse _
                           String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED") Then

                            myMessageIDList.Add(GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                            If Not String.Equals(messageType, "Error") Then messageType = "Warning"
                        End If

                    End If
                End If

                'AG 06/03/2012 - Freeze messages
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.FREEZE.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then
                        If String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "TOTAL") Then
                            'Partial Freeze is form by 3 different messages
                            If MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC0.ToString)
                            Else
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_TOTAL.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC1.ToString)
                                WSStatusAttribute = "ABORTED" 'This is business ... maybe we can move outside the method by I leave here to do not duplicate the same condition
                                ElapsedTimeTimer.Stop()
                            End If

                            myMessageIDList.Add("")
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC2.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(MDIAnalyzerManager.AnalyzerFreezeMode, "PARTIAL", False) = 0 Then
                            'Partial Freeze is form with 3 different messages
                            If MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC0.ToString)
                            Else
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_PARTIAL.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC1.ToString)
                            End If

                            myMessageIDList.Add("")
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC2.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(MDIAnalyzerManager.AnalyzerFreezeMode, "RESET", False) = 0 Then
                            'Reset Freeze is form with 1 different messages
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_RESET.ToString)
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC_RESET.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(MDIAnalyzerManager.AnalyzerFreezeMode, "AUTO", False) = 0 Then
                            'Auto Freeze is form with 2 different messages
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC_AUTO.ToString)
                            messageType = "Warning"

                        End If
                    End If
                End If
                'AG 06/03/2012

                'AG 14/10/2011 - TimeOut Communication error message
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 0 Then 'Note this sensor works different (1 connected (ok), 0 (no connected, connection fails, error))
                        If NotDisplayErrorCommsMsg Then ' XBC 19/06/2012
                            NotDisplayErrorCommsMsg = False ' XBC 19/06/2012
                        Else
                            myMessageIDList.Add(GlobalEnumerates.Messages.ERROR_COMM.ToString)
                            Debug.Print("Added ERROR_COMM to Message list in IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages")
                        End If
                        messageType = "Error"
                    End If
                End If
                'AG 07/03/2012

                'AG 09/05/2012 - No Running instruction sent because no pending executions
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                         Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS.ToString, False) = 0 _
                         Select a).ToList
                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 2 Then
                        myMessageIDList.Add(GlobalEnumerates.Messages.NO_PENDING_EXECUTIONS.ToString)
                        messageType = "Warning"

                        'Once treated reset
                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                    End If

                End If
                'AG 09/05/2012

            End If

            'Finally show single or multiple message
            If myMessageIDList.Count = 1 Then
                ShowMessage(messageType, myMessageIDList(0))
            ElseIf myMessageIDList.Count > 1 Then
                ShowMultipleMessage(messageType, myMessageIDList)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ShowAlarmsOrSensorsWarningMessages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ShowAlarmWarningMessages, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


    ''' <summary>
    ''' Set enable/disable status of all buttons and menu options affected when there are/there aren't
    ''' Tests defined in the system
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/07/2010
    ''' </remarks>
    Private Sub SetButtonsByNumOfTests()
        Try
            Dim buttonEnabled As Boolean = (NumOfTestsAttribute > 0)

            'Graphical buttons affected 
            bsTSProfileButtton.Enabled = buttonEnabled
            bsTSNewSampleButton.Enabled = buttonEnabled

            'Options affected in Programming Menu
            CalculatedTestToolStripMenuItem.Enabled = buttonEnabled
            ContaminationsToolStripMenuItem.Enabled = buttonEnabled
            ProfilesToolStripMenuItem.Enabled = buttonEnabled

            If (Not buttonEnabled) Then
                'All options in WorkSession Menu are affected
                WorksessionToolStripMenuItem.Enabled = buttonEnabled
                CurrentSessionToolStripMenuItem.Enabled = buttonEnabled
            Else
                'Verify which options in the WorkSession Menu can be enabled
                EnableWSMenu()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetButtonsByNumOfTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetButtonsByNumOfTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Search in the DB if there is an Active WorkSession for the active Analyzer and if there is one, set
    ''' value of properties ActiveWorkSession and ActiveStatus
    ''' </summary>
    ''' <param name="pGetOnlyWSStateFlag" ></param>
    ''' <remarks>
    ''' Created by:  RH 26/05/2010
    ''' Modified by: SA 09/06/2010 - Changed the method logic. Added Try/Catch
    '''              SA 28/07/2010 - Get also the Analyzer Model and assign it to the correspondent screen attribute
    '''              AG 11/07/2011 - add optional parameter for get only the WS state when calling several screen (for instance WSRotorPositions)
    ''' </remarks>
    Private Sub SetWSActiveDataFromDB(Optional ByVal pGetOnlyWSStateFlag As Boolean = False)
        Try
            If Not pGetOnlyWSStateFlag Then
                WorkSessionIDAttribute = ""
                WSStatusAttribute = ""
            End If

            Dim dataToReturn As GlobalDataTO
            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate

            ' XBC 15/06/2012 - 'There Can Be Only One WS'
            'dataToReturn = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(Nothing, AnalyzerIDAttribute)
            dataToReturn = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(Nothing)
            ' XBC 15/06/2012

            If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                Dim myWSAnalyzersDS As New WSAnalyzersDS
                myWSAnalyzersDS = DirectCast(dataToReturn.SetDatos, WSAnalyzersDS)

                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                    If Not pGetOnlyWSStateFlag And String.Equals(WorkSessionIDAttribute, String.Empty) Then
                        WorkSessionIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID
                        If Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then 'AG 24/11/2010
                            MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute 'AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager //  GlobalAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                        End If
                    End If

                    WSStatusAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus

                    If (String.Compare(AnalyzerIDAttribute, "", False) = 0) Then
                        AnalyzerIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID
                        AnalyzerModelAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerModel
                        If Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then 'AG 24/11/2010
                            MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute 'AG 30/11/2011 AnalyzerModelAttribute 
                        End If

                    End If
                    'Else
                    '    'Create and empty WS
                    '    Dim myWSOrderTestsDS As New WSOrderTestsDS
                    '    Dim myWSDelegate As New WorkSessionsDelegate

                    '    dataToReturn = myWSDelegate.AddWorkSession(Nothing, myWSOrderTestsDS, True, AnalyzerIDAttribute)
                    '    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    '        'Get the ID of both: Work Session and Analyzer
                    '        Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(dataToReturn.SetDatos, WorkSessionsDS)

                    '        If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                    '            WorkSessionIDAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                    '            WSStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus
                    '        End If
                    '    End If
                End If

                'Control availability of the different Menu Entries
                EnableWSMenu()
            Else
                'Show the error message...
                ShowMessage(Name & ".SetWSActiveDataFromDB ", dataToReturn.ErrorCode, dataToReturn.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetWSActiveDataFromDB ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetWSActiveDataFromDB ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Get the number of tests that exists currently in the DB and set value of the correspondent property
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 01/07/2010
    ''' </remarks>
    Private Sub SetNumOfTestsFromDB()
        Try
            Dim resultData As New GlobalDataTO
            Dim myTestsDelegate As New TestsDelegate

            resultData = myTestsDelegate.GetList(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myTestsDS As New TestsDS
                myTestsDS = DirectCast(resultData.SetDatos, TestsDS)
                NumOfTestsAttribute = myTestsDS.tparTests.Rows.Count
            Else
                'Error getting the number of Tests currently in DB
                ShowMessage(Name & ".SetNumOfTestsFromDB ", resultData.ErrorCode, resultData.ErrorMessage)
                NumOfTestsAttribute = 0
            End If

            SetButtonsByNumOfTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetNumOfTestsFromDB ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetNumOfTestsFromDB ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Call the Connect process (move into a method due to we want to call it when the MDI screen is opened)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' Modified by: SA 06/09/2012 - When function is called for automatic Connection (AutoConnectProcess=TRUE), all Menus and 
    '''                              Buttons in the Main MDI Screen are disabled. Once the connection process finishes, all Menus 
    '''                              and Buttons are enabled again (in both cases: there is an Analyzer connected or not)
    ''' </remarks>
    Public Sub Connect()
        Try
            'InitializeMarqueeProgreesBar()
            NotDisplayErrorCommsMsg = False ' XBC 22/06/2012
            NotDisplayShutDownConfirmMsg = False ' XBC 10/12/2012

            If (Not MDIAnalyzerManager Is Nothing) Then
                'RH 27/05/2011 Make this assignment run in the Main Thread
                SetPropertyThreadSafe("Cursor", Cursors.WaitCursor)

                If (Not AutoConnectProcess) Then
                    EnableButtonAndMenus(False) ' TR 20/09/2012 -disable menus.
                    SetActionButtonsEnableProperty(False)
                    ShowStatus(Messages.CONNECTING)
                End If

                'XBC 18/06/2012
                If (Not MDIAnalyzerManager.CommThreadsStarted) Then
                    If (MDIAnalyzerManager.StartComm()) Then
                        System.Threading.Thread.Sleep(500)
                        Application.DoEvents()
                    End If
                End If
                'XBC 18/06/2012

                Dim myTitle As String = ""
                Dim myGlobal As GlobalDataTO
                MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.FREEZE) = 0 'AG 01/02/2012 - clear sensor

                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT, True)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    If (Not MDIAnalyzerManager.Connected) Then
                        myGlobal.ErrorCode = GlobalEnumerates.Messages.ERROR_COMM.ToString
                        myTitle = "Warning"

                        Debug.Print("SET AS ERROR CODE FOR AUTO CONNECT PROCESS IN IAx00MainMDI.Connect")
                    End If

                ElseIf (myGlobal.HasError) Then
                    myTitle = "Error"
                End If

                If (Not AutoConnectProcess) Then
                    'RH 20/03/2012 Make this assignment run in the Main Thread
                    SetPropertyThreadSafe("Cursor", Cursors.Default)

                    'AG 14/10/2011 - Message is shown in method ShowAlarmWarningMessages
                    'If Not myGlobal.ErrorCode Is Nothing Then ShowMessage(myTitle, myGlobal.ErrorCode)
                    If (Not MDIAnalyzerManager.Connected) Then
                        SetActionButtonsEnableProperty(True)
                        EnableButtonAndMenus(True) ' TR 20/09/2012 -Enable menus.
                    End If
                Else
                    AutoConnectFailsErrorCode = myGlobal.ErrorCode
                    AutoConnectFailsTitle = myTitle
                End If

                'AG 23/11/2011 - The endsound is sent as a part of the connect process, after send the config instruction
                ''TR 28/10/2011 -Send the stop Ringing.
                'If MDIAnalyzerManager.Connected Then
                '    MDIAnalyzerManager.StopAnalyzerRinging(True)
                'End If
                ''TR 28/10/2011 -END.
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".Connect ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Connect ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            'AG 28/07/2010 - Close AutoConnect Thread
            If (Not AutoConnectProcess) Then
                'RH 27/05/2011 Make this assignment run in the Main Thread
                SetPropertyThreadSafe("Cursor", Cursors.Default)
                'StopMarqueeProgressBar()
            End If
            'END AG 28/07/2010
        End Try
    End Sub

    ''' <summary>
    ''' Loads the Adjustments Master data to the Application Layer
    ''' </summary>
    ''' <remarks>SGM 04/04/11</remarks>
    Private Sub LoadAdjustmentsDS()
        'Dim myGlobal As New GlobalDataTO
        Dim myGlobal As GlobalDataTO

        Try
            'FW Adjustments Master Data
            myGlobal = MDIAnalyzerManager.LoadFwAdjustmentsMasterData 'AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager // myGlobal = MyClass.GlobalAnalyzerManager.LoadFwAdjustmentsMasterData
            If myGlobal.HasError OrElse myGlobal Is Nothing Then
                ' Loading Adjustments failed 
                MyBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Ax00MainMDI_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name, myGlobal.ErrorMessage, String.Empty, MsgParent) 'TR 29/11/2011 -Change ErrorCode by ErrorMessage.
                MyClass.ErrorStatusLabel.Text = GetMessageText(myGlobal.ErrorCode)
                'Application.Exit() PDT !!! sortir de l'aplicació ??
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LoadAdjustmentsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadAdjustmentsDS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub InitializeAlertGlobes()
        'NOTE ABOUT MULTILANGUAGE:
        'This method do not take care about multilanguage due only initialize and create objects
        'The multilanguage for this objects is performed in method InitializeAlertGlobesTexts who is 
        'called by this method and when change language is performed

        Try
            AlertList.Clear()
            AlertText.Clear()

            'CoverAlert
            Dim myAlarms As New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.MAIN_COVER_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.MAIN_COVER_ERR) 'AG 14/03/2012

            CoverAlert = New bsAlert(Me, 220, 45 + ParentMDITopHeight, 85, 0, "Analyzer", True)
            CoverAlert.Tag = myAlarms
            AlertList.Add(CoverAlert)

            'ISEModuleAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_FAILED_ERR) 'ag 09/01/2012 - this alarm excludes the ISE_STATUS_WARN so it is not necessary change the bsAlert size

            myAlarms.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR) 'SGM 26/03/2012
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR)
            'myAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_EXPIRED)
            'myAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_EXP_CONS)
            'myAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_EXP_DATE)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_ACTIVATED)
            myAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR)

            ISEModuleAlert = New bsAlert(Me, 220, 115 + ParentMDITopHeight, 170, 40, "ISE Module", True)
            ISEModuleAlert.Tag = myAlarms
            AlertList.Add(ISEModuleAlert)

            'WashingSolutionAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN)

            WashingSolutionAlert = New bsAlert(Me, 220, 300 + ParentMDITopHeight, 150, 0, "Washing Solution bottle", True)
            WashingSolutionAlert.Tag = myAlarms
            AlertList.Add(WashingSolutionAlert)

            'ResiduesBalanceAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN)

            'ResiduesBalanceAlert = New bsAlert(Me, 220, 295 + ParentMDITopHeight, 210, 0, "High Contamination bottle", True)
            ResiduesBalanceAlert = New bsAlert(Me, 645, 280 + ParentMDITopHeight, 135, 0, "High Contamination bottle", False)
            ResiduesBalanceAlert.Tag = myAlarms
            AlertList.Add(ResiduesBalanceAlert)

            'Reagents1ArmAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.R1_TEMP_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR) 'AG 05/01/2011 - this alarm excludes the R1_Temp_Warm so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR)
            'myAlarms.Add(GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN) 'DL 03/05/2012. Bugs Tracking (Nr: 518)
            myAlarms.Add(GlobalEnumerates.Alarms.R1_COLLISION_ERR) 'AG 21/05/2012 - R1_COLLISION_WARN

            Reagents1ArmAlert = New bsAlert(Me, 220, 390 + ParentMDITopHeight, 100, 70, "Reagent1 Arm", True)
            Reagents1ArmAlert.Tag = myAlarms
            AlertList.Add(Reagents1ArmAlert)

            'ReagentsRotorAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR) 'AG 14/03/2012
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) 'AG 21/05/2012
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN)

            ReagentsRotorAlert = New bsAlert(Me, 220, 482 + ParentMDITopHeight, 70, 40, "Reagents Rotor", True)
            ReagentsRotorAlert.Tag = myAlarms
            AlertList.Add(ReagentsRotorAlert)

            'Reagents2ArmAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.R2_TEMP_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR) 'AG 05/01/2011 - this alarm excludes the R2_Temp_Warm so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR)
            'myAlarms.Add(GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN) 'DL 03/05/2012. Bugs Tracking (Nr: 518)
            myAlarms.Add(GlobalEnumerates.Alarms.R2_COLLISION_ERR) 'AG 21/05/2012 - R2_COLLISION_WARN

            Reagents2ArmAlert = New bsAlert(Me, 220, 570 + ParentMDITopHeight, 179, -35, "Reagent2 Arm", True)
            Reagents2ArmAlert.Tag = myAlarms
            AlertList.Add(Reagents2ArmAlert)

            'OthersAlert       
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) 'AG 09/01/2012 - this alarm excludes the WATER_DEPOSIT_ERR so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) 'AG 09/01/2012 - this alarm excludes the WASTE_DEPOSIT_ERR so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_ROTOR_FAN_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_FAN_WARN)

            OthersAlert = New bsAlert(Me, 780, 60 + ParentMDITopHeight, 0, 0, "Others", False)
            OthersAlert.Tag = myAlarms
            AlertList.Add(OthersAlert)

            'ReactionsRotorAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_COVER_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_COVER_ERR) 'AG 14/03/2012
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_MISSING_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) 'AG 31/01/2012 - this alarm excludes the METHACRYL_ROTOR_WARN so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_ENCODER_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_ERR)
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR) 'AG 21/05/2012

            ReactionsRotorAlert = New bsAlert(Me, 605, 335 + ParentMDITopHeight, 175, 110, "Reactions Rotor", False)
            ReactionsRotorAlert.Tag = myAlarms
            AlertList.Add(ReactionsRotorAlert)

            'WashingStationHeaterTempAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.WS_TEMP_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR) 'AG 09/01/2012 - this alarm excludes WS_Temp_Warn so it is not necessary change the bsAlert size
            myAlarms.Add(GlobalEnumerates.Alarms.WS_COLLISION_ERR)

            WashingStationAlert = New bsAlert(Me, 635, 430 + ParentMDITopHeight, 145, 30, "Washing Station", False)
            WashingStationAlert.Tag = myAlarms
            AlertList.Add(WashingStationAlert)

            'SampleArmAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            'myAlarms.Add(GlobalEnumerates.Alarms.CLOT_DETECTION_ERR) 'AG 25/07/2012 - Alarm related with a preparation, not in globe in the same way as S_NO_VOLUME_WARN
            'myAlarms.Add(GlobalEnumerates.Alarms.CLOT_DETECTION_WARN) 'AG 25/07/2012 - Alarm related with a preparation, not in globe in the same way as S_NO_VOLUME_WARN
            myAlarms.Add(GlobalEnumerates.Alarms.S_OBSTRUCTED_ERR) 'AG 12/03/2012
            myAlarms.Add(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR)
            'myAlarms.Add(GlobalEnumerates.Alarms.S_NO_VOLUME_WARN) 'DL 03/05/2012. Bugs Tracking (Nr: 518)
            myAlarms.Add(GlobalEnumerates.Alarms.S_COLLISION_ERR) 'AG 21/05/2012- S_COLLISION_WARN

            SampleArmAlert = New bsAlert(Me, 620, 503 + ParentMDITopHeight, 160, 45, "Samples Arm", False)
            SampleArmAlert.Tag = myAlarms
            AlertList.Add(SampleArmAlert)

            'SamplesRotorCoverAlert
            myAlarms = New List(Of GlobalEnumerates.Alarms)
            myAlarms.Add(GlobalEnumerates.Alarms.S_COVER_WARN)
            myAlarms.Add(GlobalEnumerates.Alarms.S_COVER_ERR) 'AG 14/03/2012

            SamplesRotorAlert = New bsAlert(Me, 710, 607 + ParentMDITopHeight, 70, -55, "Samples Rotor", False)
            SamplesRotorAlert.Tag = myAlarms
            AlertList.Add(SamplesRotorAlert)

            'Fill AlertText
            InitializeAlertGlobesTexts()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitializeAlertGlobes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

        End Try
    End Sub

    ''' <summary>
    ''' Assign the alert globes the description in the current language
    ''' </summary>
    ''' <remarks>AG 14/09/2011</remarks>
    Private Sub InitializeAlertGlobesTexts()
        Try
            'Get texts from the DB for the alarm names grouped in globes
            Dim alarmsDlg As New AlarmsDelegate
            Dim resultData As GlobalDataTO
            Dim myAlarmsDS As AlarmsDS

            resultData = alarmsDlg.ReadAll(Nothing)

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                myAlarmsDS = CType(resultData.SetDatos, AlarmsDS)

                Dim linqRes As List(Of AlarmsDS.tfmwAlarmsRow)
                Dim myText As String

                For Each alarm As GlobalEnumerates.Alarms In [Enum].GetValues(GetType(GlobalEnumerates.Alarms))
                    linqRes = (From a As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms _
                               Where String.Equals(a.AlarmID, alarm.ToString()) _
                               Select a).ToList()

                    myText = String.Empty

                    If linqRes.Count > 0 AndAlso Not linqRes(0).IsNameResourceIDNull Then
                        myText = MultilanguageResourcesDelegate.GetResourceText(linqRes(0).NameResourceID)
                    End If

                    If String.IsNullOrEmpty(myText) Then myText = alarm.ToString() 'Protection case

                    AlertText(alarm) = myText
                Next

                'Get texts from the DB for the globe names
                CoverAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("LBL_SCRIPT_EDIT_Analyzer") ' "Analyzer" 'JB 01/10/2012 - String Resource unification
                ISEModuleAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("ISEMODULE_ALERT") ' "ISE Module"
                WashingSolutionAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("WASHINGSOLUTION_ALERT") ' "Washing Solution Bottle"
                ResiduesBalanceAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("RESIDUESBALANCE_ALERT") ' "High Contamination Bottle"
                Reagents1ArmAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("REAGENTS1ARM_ALERT") ' "Reagent1 Arm"
                ReagentsRotorAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("REAGENTSROTOR_ALERT") ' "Reagents Rotor"
                Reagents2ArmAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("REAGENTS2ARM_ALERT") ' "Reagent2 Arm"
                OthersAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("OTHERS_ALERT") ' "Others" '??????
                ReactionsRotorAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("REACTIONSROTOR_ALERT") ' "Reactions Rotor"
                WashingStationAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("WASHINGSTATION_ALERT") ' "Washing Station"
                SampleArmAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("SAMPLEARM_ALERT") ' "Sample Arm"
                SamplesRotorAlert.Caption = MultilanguageResourcesDelegate.GetResourceText("SAMPLESROTOR_ALERT") ' "Samples Rotor"

                'Else
                'Do something with thew error
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitializeAlertGlobesTexts ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

        End Try
    End Sub


    ''' <summary>
    ''' Manage the flags indicating the user actions over the vertical button bar
    ''' </summary>
    ''' <param name="pUserAction"></param>
    ''' <remarks></remarks>
    Private Sub SetWorkSessionUserCommandFlags(ByVal pUserAction As WorkSessionUserActions)
        Try
            If pUserAction = WorkSessionUserActions.START_WS Then
                ExecuteSessionAttribute = True
                PauseSessionAttribute = False
                AbortSessionAttribute = False

            ElseIf pUserAction = WorkSessionUserActions.PAUSE_WS Then
                ExecuteSessionAttribute = False
                PauseSessionAttribute = True
                AbortSessionAttribute = False

            ElseIf pUserAction = WorkSessionUserActions.ABORT_WS Then
                ExecuteSessionAttribute = False
                PauseSessionAttribute = False
                AbortSessionAttribute = True

            Else 'NO_WS
                ExecuteSessionAttribute = False
                PauseSessionAttribute = False
                AbortSessionAttribute = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetWorkSessionUserCommandFlags ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
    ''' </remarks>
    Private Function InitializeStartInstrument() As GlobalDataTO
        Dim myGlobal As GlobalDataTO = Nothing
        'Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            'DL 09/09/2011
            'Set Enable (or set visible meeting 12/09/2011 ??) frame time W-Up in common Monitor
            'IMonitor.bsWamUpGroupBox.Enabled = True
            Dim swParams As New SwParametersDelegate

            ' Read W-Up full time configuration
            myGlobal = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)

            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                Dim myParametersDS As New ParametersDS

                myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)

                myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString) _
                          Select a).ToList

                Dim WUPFullTime As Single
                If myList.Count > 0 Then WUPFullTime = myList(0).ValueNumeric '= DateTime.Now.ToString("yyyyMMdd hh-mm") & "_" & myList(0).ValueText
            End If

            ' Save initial states when press over w-up
            If Not myGlobal.HasError Then
                Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                'WUPCOMPLETEFLAG
                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                With myAnalyzerSettingsRow
                    .AnalyzerID = AnalyzerIDAttribute
                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                    .CurrentValue = "0"
                End With
                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                'WUPCOMPLETEFLAG
                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                With myAnalyzerSettingsRow
                    .AnalyzerID = AnalyzerIDAttribute
                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
                    ''.CurrentValue = Now.ToString 'AG + SA 05/10/2012
                    '.CurrentValue = Now.ToString("yyyy/MM/dd HH:mm:ss")
                    .CurrentValue = Now.ToString(CultureInfo.InvariantCulture)
                End With
                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                myGlobal = myAnalyzerSettings.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

            End If

            'Activate only if current screen is monitor
            WarmUpFinishedAttribute = False
            BsTimerWUp.Enabled = True
            If Not ActiveMdiChild Is Nothing Then
                If (TypeOf ActiveMdiChild Is IMonitor) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    'CurrentMdiChild.bsWamUpGroupBox.Enabled = True
                    CurrentMdiChild.bsWamUpGroupBox.Visible = True
                End If
            End If
            'END DL 09/09/2011

            Me.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = True 'SGM 13/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitializeStartInstrument ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Return myGlobal
    End Function


    ''' <summary>
    ''' Several screens have business witch depends on the analyzer status ... so if the analyzer status changes when screen is opened
    ''' they has to be notified (IWSRotorPositions, IConfigAnalyzer, IBarcodeCondig, IChangeRotor, ISatReport_Load, ISettings, IWSSampleRequest, ISatReport ...)
    ''' 
    ''' NOTE: By now 23/09/2011 only apply this notification in IWSRotorPositions
    ''' </summary>
    ''' <remarks>AG 23/09/2011</remarks>
    Private Sub TreatScreenDueAnalyzerStatusChanged()
        Try

            If Not ActiveMdiChild Is Nothing Then
                '- RotorPositions: Some user actions event are allowed or not depending the status
                If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                    Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                    CurrentMdiChild.RefreshScreenStatus(MDIAnalyzerManager.AnalyzerStatus.ToString, WSStatusAttribute)
                End If

                'TODO
                ' IConfigAnalyzer, IBarcodeCondig, IChangeRotor, ISatReport_Load, ISettings, IWSSampleRequest, ISatReport ...

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".TreatScreenDueAnalyzerStatusChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    '''' <summary>
    '''' Show legend known
    '''' </summary>
    '''' <remarks>DL 22/06/2011 </remarks>
    'Private Sub ShowLegendMain()
    '    Try
    '        Dim myLegend As String

    '        Select Case Me.ActiveMdiChild.Name
    '            Case IWSRotorPositions.Name
    '                myLegend = LEGEND_SOURCE.LEGEND_ROTOR.ToString()

    '            Case IWSSampleRequest.Name
    '                myLegend = LEGEND_SOURCE.LEGEND_PREPARATION.ToString()

    '            Case Else
    '                myLegend = String.Empty 'RH 30/06/2011
    '        End Select

    '        Using myForm As New ILegend()
    '            myForm.ParentMDI = Me.Location
    '            myForm.SourceForm = myLegend
    '            OpeningLegend = False
    '            myForm.ShowDialog()
    '        End Using

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".ShowLegendMain ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".ShowLegendMain ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub


    ' ''' <summary>
    ' ''' If exist some alarm whose treatment (in RUNNING) requires send the ENDRUN instruction return TRUE, otherwise return FALSE
    ' ''' </summary>
    ' ''' <param name="pCurrentAlarmList"></param>
    ' ''' <returns></returns>
    ' ''' <remarks>AG 01/12/2011
    ' ''' AG 15/03/2012 - clot alarm do not implies END instruction (removed from IF)
    ' ''' AG 29/05/2012 - when analyzer is FREEZE software can not send the START instruction (added to IF)
    ' ''' AG 19/06/2012 - add CLOT_DETECTION_ERR alarm
    ' ''' AG 21/10/2013 - commented and moved to analyzermanager class</remarks>
    'Private Function ExistSomeAlarmThatRequiresSendENDRUN(ByVal pCurrentAlarmList As List(Of GlobalEnumerates.Alarms)) As Boolean
    '    Dim returnValue As Boolean = False
    '    Try
    '        If pCurrentAlarmList.Count > 0 Then
    '            'AG 16/02/2012 the bottles warning level do not implies ENDRUN - remove from condition the alarms HIGH_CONTAMIN_WARN and WASH_CONTAINER_WARN
    '            'AG 25/07/2012 - the clot detection error do not implies ENDRUN - remove from condition the alarm CLOT_DETECTION_ERR

    '            'AG 22/02/2012 - NOTE: To prevent the bottle level ERROR or WARNING oscillations when near the limits Sw will not remove the ERROR level alarm until neither error neither warming exists 
    '            If pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
    '            pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
    '            pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
    '            pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
    '            pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.R1_COLLISION_WARN) OrElse pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.R2_COLLISION_WARN) OrElse _
    '            pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.S_COLLISION_WARN) OrElse pCurrentAlarmList.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
    '            MDIAnalyzerManager.wellBaseLineAutoPausesSession <> 0 OrElse MDIAnalyzerManager.AnalyzerIsFreeze Then
    '                returnValue = True
    '            End If
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".ExistSomeAlarmThatRequiresSendENDRUN ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".ExistSomeAlarmThatRequiresSendENDRUN ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return returnValue
    'End Function

    ''' <summary>
    ''' Get the Wating cycle before running.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 02/12/2011</remarks>
    Private Function GetWaitingCycle() As Single
        Dim waitingMachineTime As Single = 0
        Try
            Dim resultData As GlobalDataTO
            Dim myParametersDS As New ParametersDS
            Dim mySWParametersDelegate As New SwParametersDelegate

            resultData = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, AnalyzerIDAttribute, _
                               GlobalEnumerates.SwParameters.WAITING_CYCLES_BEFORE_RUNNING.ToString, True)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then waitingMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetWaitingCycle ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetWaitingCycle", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, Me)
        End Try
        Return waitingMachineTime
    End Function

    Private Function GetCycleMachine() As Single
        Dim cycleMachineTime As Single = 0
        Try
            Dim resultData As GlobalDataTO
            Dim myParametersDS As New ParametersDS
            Dim mySWParametersDelegate As New SwParametersDelegate

            resultData = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, AnalyzerIDAttribute, _
                               GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString, True)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then cycleMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetCycleMachine ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetCycleMachine", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, Me)
        End Try
        Return cycleMachineTime
    End Function


    ''' <summary>
    ''' Process:
    ''' 1) before enter in running mode: Scan bar code (if configured)
    ''' 2) before enter in running mode: ise conditioning
    ''' 3) Enter in running mode
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 18/01/2012
    ''' AG 08/03/2013 - changes in start WS (if barcode warnings ask the user for show auxiliary screen or go to running)
    ''' </remarks>
    Private Sub EnterRunning()
        Try
            Dim myGlobal As New GlobalDataTO

            'AG 08/03/2013
            'MDIAnalyzerManager.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BEFORE_RUNNING_REQUEST   'Initialize barcode read before running process
            'myGlobal = MDIAnalyzerManager.ManageBarCodeRequestBeforeRUNNING(Nothing, AnalyzerManager.BarcodeWorksessionActions.BEFORE_RUNNING_REQUEST)

            Dim myAction As AnalyzerManager.BarcodeWorksessionActions = AnalyzerManager.BarcodeWorksessionActions.BEFORE_RUNNING_REQUEST 'Enter running but, if configured, perform the barcode read before
            If Not readBarcodeIfConfigured Then
                myAction = AnalyzerManager.BarcodeWorksessionActions.FORCE_ENTER_RUNNING    'Force enter running and skip incomplete sampes if any
            End If
            MDIAnalyzerManager.BarCodeProcessBeforeRunning = myAction
            myGlobal = MDIAnalyzerManager.ManageBarCodeRequestBeforeRUNNING(Nothing, myAction)
            'AG 08/03/2013

            If (myGlobal.HasError) Then
                processingBeforeRunning = "2"
                ScreenWorkingProcess = False
                'ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage) 'Show message - ??
            End If

            'If activateButtonsFlag Then
            '    SetActionButtonsEnableProperty(True) 'AG 14/10/2011 - update vertical button bar
            'End If
            ''TR 11/11/2011- Start the Elapse Time Timer
            'Me.ElapsedTimeTimer.Start()

            'DL 12/06/2012
            If Not MDIAnalyzerManager Is Nothing Then
                If Not MDIAnalyzerManager.Connected Then
                    ScreenWorkingProcess = False
                    processingBeforeRunning = "2"
                End If
            Else
                ScreenWorkingProcess = False
                processingBeforeRunning = "2"
            End If
            'DL 12/06/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".EnterRunning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".EnterRunning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".EnterRunning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
            ScreenWorkingProcess = False
            processingBeforeRunning = "2"
            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 - ws not started automatically
            InitializeAutoWSFlags()
        End Try

    End Sub


    ''' <summary>
    ''' Analyzer recover process
    ''' </summary>
    ''' <remarks>
    ''' Created by: AG 07/03/2012
    ''' </remarks>
    Private Sub RecoverInstrument()
        Try
            Dim myGlobal As New GlobalDataTO
            myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop SENSOR information instruction

            If MDIAnalyzerManager.Connected Then
                'Reset all analyzer flags
                Dim myFlags As New AnalyzerManagerFlagsDelegate
                myGlobal = myFlags.ResetFlags(Nothing, AnalyzerIDAttribute)
                If Not myGlobal.HasError Then

                    'Clear all flags before recover
                    MDIAnalyzerManager.InitializeAnalyzerFlags(Nothing)
                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS"
                    MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                    ShowStatus(Messages.RECOVERING_INSTRUMENT)

                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.RECOVER, True)

                    Dim activateButtonsFlag As Boolean = False
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        If Not MDIAnalyzerManager.Connected Then
                            MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = ""
                            activateButtonsFlag = True
                        End If

                    ElseIf myGlobal.HasError Then
                        ShowMessage("Error", myGlobal.ErrorMessage)
                        activateButtonsFlag = True
                    End If

                    If activateButtonsFlag Then
                        ScreenWorkingProcess = False
                        processingRecover = False
                        ShowStatus(Messages._NONE)
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".RecoverInstrument", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".RecoverInstrument", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".RecoverInstrument", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
            ScreenWorkingProcess = False
            processingRecover = False
        End Try

    End Sub


    ''' <summary>
    ''' Clear or set text to MDI error provider
    ''' </summary>
    ''' <param name="pClearFlag"></param>
    ''' <remarks>AG 18/08/2012 - creation</remarks>
    Private Sub ChangeErrorStatusLabel(ByVal pClearFlag As Boolean)
        Try

            If Not pClearFlag Then
                Dim setProviderNotFinishedFlag As Boolean = False
                If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) = 0 OrElse _
                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "PAUSED", False) = 0 OrElse _
                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "PAUSED", False) = 0 OrElse _
                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) = 0 OrElse _
                   String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess), "PAUSED", False) = 0 Then
                    setProviderNotFinishedFlag = True
                ElseIf MDIAnalyzerManager.AnalyzerIsFreeze AndAlso (String.Compare(MDIAnalyzerManager.AnalyzerFreezeMode, "TOTAL", False) = 0 OrElse String.Compare(MDIAnalyzerManager.AnalyzerFreezeMode, "RESET", False) = 0) Then
                    setProviderNotFinishedFlag = True
                End If

                If setProviderNotFinishedFlag Then
                    'I am the MDI so do not search for mdi control (it does not work)
                    'BsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.NOT_FINISHED.ToString))
                    ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                    ErrorStatusLabel.Text = GetMessageText(GlobalEnumerates.Messages.NOT_FINISHED.ToString)
                End If

            Else ' Clear 
                'I am the MDI so do not search for mdi control (it does not work)
                'BsErrorProvider1.Clear
                ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None
                ErrorStatusLabel.Text = String.Empty
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ChangeErrorStatusLabel", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeErrorStatusLabel", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Evaluates if there is some business process in course and then returns TRUE, else returns FALSE
    ''' This information is used in order to not activate menus, buttons ,... while process is in course!!!
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessingBusinessInCourse() As Boolean
        Dim returnValue As Boolean = False
        Try
            'Evaluate if any of MDI processing flags is TRUE
            'NOTE that for the MDI we can not use the screen property (ScreenWorkingProcess) in some cases fails because can exists several working processes with different exit conditions
            If AutoConnectProcess OrElse processingBeforeRunning = "0" OrElse processingRecover OrElse processingReset Then
                returnValue = True

            Else
                Dim MdiChildIsDisabled As Boolean = False
                If DisabledMDIChildAttribute.Count > 0 Then
                    MdiChildIsDisabled = True
                End If

                Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
                If Not ActiveMdiChild Is Nothing Then
                    myCurrentMDIForm = ActiveMdiChild
                ElseIf MdiChildIsDisabled Then
                    myCurrentMDIForm = DisabledMDIChildAttribute(0)
                    returnValue = True
                End If

                If Not returnValue AndAlso Not myCurrentMDIForm Is Nothing Then
                    'Evaluate the ScreenWorkingProcess attribute of the current screen
                    'List of screen that use this property and call InitializeMarqueeProgreesBar method (evaluation date 08/11/2012)
                    'IAx00MainMDI
                    'IChangeRotor
                    'IConditioning
                    'IMonitor
                    'IWSRotorPositions
                    'IWSSampleRequest
                    'IISEUtilities
                    'ISATReport
                    'ISATReportLoad
                    'IResults
                    If ScreenWorkingProcess Then 'MDI
                        returnValue = True

                    ElseIf (TypeOf myCurrentMDIForm Is IChangeRotor) Then
                        Dim auxScreen As IChangeRotor = CType(myCurrentMDIForm, IChangeRotor)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IConditioning) Then
                        Dim auxScreen As IConditioning = CType(myCurrentMDIForm, IConditioning)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IMonitor) Then
                        Dim auxScreen As IMonitor = CType(myCurrentMDIForm, IMonitor)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions) Then
                        Dim auxScreen As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                        Dim auxScreen As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IISEUtilities) Then
                        Dim auxScreen As IISEUtilities = CType(myCurrentMDIForm, IISEUtilities)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is ISATReport) Then
                        Dim auxScreen As ISATReport = CType(myCurrentMDIForm, ISATReport)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is ISATReportLoad) Then
                        Dim auxScreen As ISATReportLoad = CType(myCurrentMDIForm, ISATReportLoad)
                        returnValue = auxScreen.ScreenWorkingProcess

                    ElseIf (TypeOf myCurrentMDIForm Is IResults) Then
                        Dim auxScreen As IResults = CType(myCurrentMDIForm, IResults)
                        returnValue = auxScreen.ScreenWorkingProcess

                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ProcessingBusinessInCourse", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProcessingBusinessInCourse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            returnValue = False
        End Try
        'Return False 'To simulate OLD code
        Return returnValue
    End Function



    ''' <summary>
    ''' Launch the start WS (enter in running mode process)
    ''' </summary>
    ''' <remarks>
    ''' Creation - AG 08/03/2013 - this code has been copied from StartSession method because it can be called from several points in code
    ''' Modified - XB 27/09/2013 - add WarmUpFinishedAttribute condition to execute subProcessAskBySpecimen or subProcessReadBarcode for HQ
    ''' Modified by AG 05/11/2013 - modified during validation of task #1375
    ''' </remarks>
    Private Sub StartEnterInRunningMode()
        Try
            'DL 17/04/2013. BEGIN
            Dim resultData As GlobalDataTO = Nothing
            Dim myUserSettingDelegate As New UserSettingsDelegate
            Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Dim barcodeBeforeStart As Boolean
            Dim barcodeReagentDisabled As Boolean
            Dim barcodeSampleDisabled As Boolean
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS

            resultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.BARCODE_BEFORE_START_WS.ToString())
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                barcodeBeforeStart = CType(resultData.SetDatos, Boolean)
            End If

            resultData = myAnalyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                myAnalyzerSettingsDS = CType(resultData.SetDatos, AnalyzerSettingsDS)
                If myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                    barcodeReagentDisabled = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                End If
            End If

            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString())
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDS = CType(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0) Then
                    barcodeSampleDisabled = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                End If
            End If

            'AG 08/07/2013 - Special case for automate the WS creation with LIS (read barcode always and do not take care about the setting)
            'XB 23/07/2013 - auto HQ
            'If autoWSCreationWithLISModeAttribute Then
            If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                'XB 23/07/2013
                If automateProcessCurrentState = LISautomateProcessSteps.initialStatus Then
                    'XB 24/07/2013 - auto HQ

                    'AG 05/11/2013 - Task #1375 (change condition because now barcode can be read in running (paused mode)!!!
                    'If Not MDIAnalyzerManager.Connected OrElse barcodeSampleDisabled OrElse _
                    'MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING OrElse _
                    'Not WarmUpFinishedAttribute Then
                    If Not MDIAnalyzerManager.Connected OrElse barcodeSampleDisabled OrElse _
                       (MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING AndAlso Not MDIAnalyzerManager.AllowScanInRunning) OrElse _
                       Not WarmUpFinishedAttribute Then
                        'AG 05/11/2013

                        'Open popup the HostQuery screen 
                        Dim myLogAcciones As New ApplicationLogManager()
                        myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: HostQuery monitor screen opening (on RUNNING)", "IAx00MainMDI.StartEnterInRunningMode", EventLogEntryType.Information, False)
                        Dim myOriginalStatus As String = bsAnalyzerStatus.Text
                        ShowStatus(Messages.AUTOLIS_WAITING_ORDERS)

                        Using MyForm As New HQBarcode
                            MyForm.AnalyzerID = AnalyzerIDAttribute
                            MyForm.WorkSessionID = WorkSessionIDAttribute
                            MyForm.WorkSessionStatus = WSStatusAttribute
                            MyForm.SourceScreen = SourceScreen.START_BUTTON
                            MyForm.TopMost = True

                            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                MyForm.OpenByAutomaticProcess = True
                                MyForm.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                                EnableLISWaitTimer(False)
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessAskBySpecimen)
                            End If

                            AddNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is shown

                            MyForm.ShowDialog()
                            RemoveNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is closed
                            incompleteSamplesOpenFlag = False

                            WSStatusAttribute = MyForm.WorkSessionStatus
                        End Using

                        bsAnalyzerStatus.Text = myOriginalStatus

                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                            myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: HostQuery monitor screen closed (on RUNNING)", "IAx00MainMDI.StartEnterInRunningMode", EventLogEntryType.Information, False)

                            'Check if there is something received from LIS pending to be add to WS (see conditions instead of button status)
                            Dim myESRulesDlg As New ESBusiness
                            If myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage) Then
                                'OK - normal case
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessDownloadOrders)

                                CreateAutomaticWSWithLIS()
                            ElseIf automateProcessCurrentState = LISautomateProcessSteps.subProcessAskBySpecimen Then
                                'Exception: No workorders received from LIS
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitNoWorkOrders)
                                InitializeAutoWSFlags()

                            ElseIf automateProcessCurrentState = LISautomateProcessSteps.ExitAutomaticProcesAndStop Then
                                'Exception: Impossible ask for specimen and user decides stop
                                EnableButtonAndMenus(True, True) 'Enable buttons before update attribute!!
                                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                InitializeAutoWSFlags()
                            End If
                        End If

                        Exit Sub 'Note, there is an Exit Sub here!!!
                    Else
                        'XB 24/07/2013 - auto HQ

                        barcodeBeforeStart = True
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessReadBarcode)
                        'JV + AG revision 18/10/2013  task # 1341
                        'If Not HQProcessByUserFlag Then bsTSPauseSessionButton.Enabled = True 'Activate the PAUSE (Automatic process will be paused just before going to Running)
                        If Not HQProcessByUserFlag Then
                            showPAUSEWSiconFlag = True
                            ChangeStatusImageMultipleSessionButton()
                        End If
                        'JV + AG revision 18/10/2013 task # 1341
                    End If
                Else
                    barcodeBeforeStart = False
                    If (automateProcessCurrentState = LISautomateProcessSteps.ExitNoLISConnectionButGoToRunning OrElse automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning) Then
                        readBarcodeIfConfigured = False 'Enter running without read barcode
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessEnterRunning)
                    End If
                End If
            End If
            'AG 08/07/2013

            'AG 19/11/2013 - Task #1375 corrections - Do not activate watchdog if readbarcodeIfConfigured = FALSE
            'If barcodeBeforeStart = True And (Not barcodeReagentDisabled Or Not barcodeSampleDisabled) Then
            If readBarcodeIfConfigured AndAlso barcodeBeforeStart = True AndAlso (Not barcodeReagentDisabled OrElse Not barcodeSampleDisabled) Then
                ' Read application name for LIS parameter
                resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.WDOG_TIME_BARCODE_SCAN.ToString, Nothing)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myParametersDS = CType(resultData.SetDatos, ParametersDS)
                    If myParametersDS.tfmwSwParameters.Count > 0 Then
                        Dim factor As Integer = 1
                        If Not barcodeSampleDisabled And Not barcodeReagentDisabled Then factor = 2

                        watchDogTimer.Interval = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric * factor * 1000
                    End If
                End If
                'DL 16/04/2013 Read setting WDOG_TIME_BARCODE_SCAN and assign to watchdog timer (interval property). END
                watchDogCase = CStr(MDIWatchDogCases.ReadBarcodeBeforeRunning)
                watchDogTimer.Enabled = True
            End If
            'DL 17/04/2013 END

            Dim workingThread As New Threading.Thread(AddressOf EnterRunning)
            processingBeforeRunning = "0"
            ScreenWorkingProcess = True
            workingThread.Start()

            While String.Equals(processingBeforeRunning, "0") '(ScreenWorkingProcess)
                InitializeMarqueeProgreesBar()
                Cursor = Cursors.WaitCursor
                Application.DoEvents()
            End While
            workingThread = Nothing
            watchDogTimer.Enabled = False
            StopMarqueeProgressBar()
            Cursor = Cursors.Default

            Dim myCurrentMDIForm As Form = Nothing
            If (Not ActiveMdiChild Is Nothing) Then
                ActiveMdiChild.Enabled = True

            ElseIf (DisabledMDIChildAttribute.Count > 0) Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
                myCurrentMDIForm.Enabled = True

                If (DisabledMDIChildAttribute.Count = 1) Then
                    DisabledMDIChildAttribute.Clear()
                End If
            End If

            'AG 17/07/2013 - Moved here from ManageEventReception method
            'XB 23/07/2013 - auto HQ
            ' If autoWSCreationWithLISModeAttribute 
            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) _
                    AndAlso (automateProcessCurrentState = LISautomateProcessSteps.subProcessDownloadOrders OrElse _
                             automateProcessCurrentState = LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning OrElse _
                             automateProcessCurrentState = LISautomateProcessSteps.ExitNoWorkOrders) Then
                'XB 23/07/2013 - auto HQ
                CreateAutomaticWSWithLIS()
                Exit Sub 'Note, there is an Exit Sub here!!!

                'AG 24/07/2013 - if barcode error activate sound and show message box to user
            ElseIf (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) _
                    AndAlso (automateProcessCurrentState = LISautomateProcessSteps.ExitBarcodeErrorAndStop) Then
                CheckForExceptionsInAutoCreateWSWithLISProcess(9)
                CloseActiveMdiChild(True) 'AG 25/07/2013
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                OpenMonitorForm(Nothing) 'AG 25/07/2013
                'AG 24/07/2013
            Else

                If Not String.Equals(processingBeforeRunning, "2") Then
                    SetActionButtonsEnableProperty(True)
                    TreatScreenDueAnalyzerStatusChanged() 'AG 11/05/2012
                    'TR 21/05/2012 -Set value to false after stating the time. Reset local time variables.
                    UserPauseWSAttribute = False
                    TotalSecs = 0
                    TotalSecs2 = 0
                    'TR 21/05/2012 -END.
                    Me.ElapsedTimeTimer.Start()

                Else
                    'Finished with error
                    ShowStatus(Messages.STANDBY)
                End If

                'AG 17/07/2013
                If automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning Then
                    'When exit this method reset the internal flag to the default value. Process stopped or finished
                    SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                    InitializeAutoWSFlags()
                End If
                'AG 17/07/2013

            End If
            'AG 08/07/2013

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".StartEnterInRunningMode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".StartEnterInRunningMode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 11/07/2013 - ws not started automatically
            InitializeAutoWSFlags()
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' AG 23/10/2013 creation - moved from ManageEventReception
    ''' </remarks>
    Private Sub ShowLabelInStatusBar()
        Try
            If Not bsAnalyzerStatus Is Nothing Then
                'bsAnalyzerStatus.DisplayStyle = ToolStripItemDisplayStyle.Text
                If MDIAnalyzerManager.Connected Then
                    'AG 03/10/2011 - Sleeping + analyzer ready: CONNECTED // Sleeping + NO analyzer ready: SLEEPING
                    'bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString()

                    If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING AndAlso MDIAnalyzerManager.AnalyzerIsReady Then
                        'bsAnalyzerStatus.Text = "CONNECTED"
                        'TR 20/09/2012 -enable buttons and menu elements.
                        EnableButtonAndMenus(True)
                        ShowStatus(Messages.CONNECTED)

                    ElseIf MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                        'bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString()
                        Select Case MDIAnalyzerManager.AnalyzerStatus
                            'Case GlobalEnumerates.AnalyzerManagerStatus.MONITOR
                            'Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING

                            Case GlobalEnumerates.AnalyzerManagerStatus.STANDBY
                                ' XBC 13/07/2012
                                Dim ISEOperating As Boolean = False
                                If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
                                   MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
                                   MDIAnalyzerManager.ISE_Manager.CurrentProcedure <> ISEManager.ISEProcedures.None Then
                                    'Do not change the status text
                                    ISEOperating = True
                                End If
                                ' XBC 13/07/2012

                                '1st Start recover instrument
                                If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS" OrElse _
                                   MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "PAUSED" Then

                                    ShowStatus(Messages.RECOVERING_INSTRUMENT)

                                    '2on Light adjustment (1st light adjustments because it can be performed: when wupprocess is closed or paused, sdownprocess is closed or paused, 
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED" Then
                                    ShowStatus(Messages.LIGHT_ADJUSTMENT)

                                    '3th Shut down instrument
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "PAUSED" Then
                                    ShowStatus(Messages.SHUTDOWN)


                                    '4rd Start instrument process Activate
                                ElseIf (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED") OrElse Not WarmUpFinishedAttribute Then
                                    'ShowStatus(Messages.STARTING_INSTRUMENT)
                                    If Not WarmUpFinishedAttribute Then
                                        Dim myAnalyzerSettingsDS As AnalyzerSettingsDS
                                        Dim myGlobal As GlobalDataTO

                                        'Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                                        myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                               AnalyzerIDAttribute, _
                                                               GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                            myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                                Dim myDate As String = ""
                                                myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue
                                                If myDate = "" Then
                                                    WarmUpFinishedAttribute = True
                                                    If MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 Then
                                                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                                                        MyClass.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 -Ise alarms ready to be shown
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                    If Not WarmUpFinishedAttribute And Not ISEOperating Then
                                        If Not BsTimerWUp.Enabled Then BsTimerWUp.Enabled = True
                                        ShowStatus(Messages.STARTING_INSTRUMENT)
                                        'Else
                                        'Do not change the status text
                                    End If

                                    '5th Abort session (once analyzer in Standby the abort process sends a complete wash)
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "PAUSED" Then
                                    ShowStatus(Messages.ABORTING_SESSION)

                                    '6th (a) Start Work session Reading bar code before Start(these flags has no the PAUSED value)
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then
                                    ShowStatus(Messages.BARCODE_READING)

                                    '6th (b) Start Work session (these flags has no the PAUSED value)
                                ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS", False) = 0 _
                                    OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess) = "INPROCESS" Then
                                    ShowStatus(Messages.STARTING_SESSION)

                                    '7th Connecting                                   
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                                    'Do not change the status text

                                    '8th ISE initialitation (connection in StandBy)                                   
                                ElseIf MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                                    'Do not change the status text

                                    'Other  cases if analyzer is working do not change the status text (for example bar code read from WSPreparation and RotorPosition)
                                ElseIf Not MDIAnalyzerManager.AnalyzerIsReady Then
                                    'Do not change the status text

                                Else
                                    If StartSessionisPending Or StartingSession Then  ' XB 30/10/2013 
                                        ' Nothing to do
                                    Else
                                        'TR 20/09/2012 -enable buttons and menu elements.
                                        EnableButtonAndMenus(True)
                                        ShowStatus(Messages.STANDBY)
                                    End If
                                End If

                                'AG 19/04/2012 - WS is aborted and user do not change screen
                                If String.Equals(WSStatusAttribute, "ABORTED") AndAlso Not ActiveMdiChild Is Nothing Then
                                    '- Monitor (WSStates ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                                    If (TypeOf ActiveMdiChild Is IMonitor) Then
                                        Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IResults) Then
                                        Dim CurrentMdiChild As IResults = CType(ActiveMdiChild, IResults)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                        Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IWSSampleRequest) Then
                                        Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    End If
                                End If
                                'AG 19/04/2012

                            Case GlobalEnumerates.AnalyzerManagerStatus.RUNNING
                                'Start Work session (these flags has no the PAUSED value)
                                'Aborting session
                                If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "PAUSED" Then
                                    ShowStatus(Messages.ABORTING_SESSION)

                                    'Normal case
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess) = "INPROCESS" _
                                    OrElse MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess) = "INPROCESS" Then
                                    'Do not change the status text
                                ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess).Equals("INPROCESS") OrElse _
                                    MDIAnalyzerManager.AllowScanInRunning Then
                                    ShowStatus(Messages.PAUSE_IN_RUNNING)
                                    '//25/10/2013 - CF BT#1348 Adds the 'paused' status in the statusbar.

                                Else
                                    ShowStatus(Messages.RUNNING)
                                End If

                            Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING    'DL 04/06/2012
                                If flagExitWithShutDown Then                        'DL 04/06/2012
                                    'Disable all buttons until Ax00 accept the new instruction
                                    SetActionButtonsEnableProperty(False)
                                    Me.Close() 'DL 04/06/2012
                                End If                                              'DL 04/06/2012  

                        End Select

                    End If
                    'AG 03/10/2011

                    'RH + AG 30/03/2012
                    Dim Flag1 As String = MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess)
                    Dim Flag2 As String = MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess)

                    If Not String.Equals(Flag1, "INPROCESS") AndAlso _
                       Not String.Equals(Flag1, "PAUSED") AndAlso _
                       Not String.Equals(Flag2, "INPROCESS") AndAlso _
                       Not String.Equals(Flag2, "PAUSED") Then

                        SetEnableMainTab(True)

                    End If
                    'RH + AG 30/03/2012
                Else

                    If flagExitWithShutDown AndAlso MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "CLOSED" Then                        'DL 04/06/2012
                        'Disable all buttons until Ax00 accept the new instruction
                        SetActionButtonsEnableProperty(False)
                        Me.Close() 'DL 04/06/2012

                    ElseIf MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                        ShowStatus(Messages.CONNECTING)
                    Else
                        'bsAnalyzerStatus.Text = "NO CONNECTED"
                        ShowStatus(Messages.NO_CONNECTED)
                        SetEnableMainTab(False) 'RH 27/03/2012
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ShowLabelInStatusBar", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowLabelInStatusBar", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Abort session has to show several options: STOP, ABORT or SKIP
    ''' STOP only if the END instruction has not been already sent or if not recovery results in process
    ''' </summary>
    ''' <remarks>AG 28/11/2013 Refactoring - moved from bsTSAbortSessionButton_Click
    ''' AG 02/12/2013 - (#1423) in partial freeze mode the messageboxType must be Yes/No (only abort option)</remarks>
    Private Sub AbortSessionFromEventClicked()
        Try
            Dim myGlobal As New GlobalDataTO
            '// CF v3.0.0 bugtracking -task # 1342  
            '//Var to store the messagebox type to be displayed to the user. 
            Dim messageBoxType As MessageBoxButtons

            CreateLogActivity("Btn Abort", Me.Name & ".AbortSessionFromEventClicked", EventLogEntryType.Information, False) 'JV #1360 24/10/2013

            If (Not MDIAnalyzerManager Is Nothing) Then

                'TR 27/10/2011 -Stop Sound.
                MDIAnalyzerManager.StopAnalyzerRinging()

                Dim userAnswer As DialogResult = DialogResult.No 'AG 24/04/2012 - Ask for Abort confirmation


                If NotDisplayAbortMsg Then
                    userAnswer = Windows.Forms.DialogResult.Yes ' XBC 19/06/2012
                    messageBoxType = MessageBoxButtons.YesNo ' CF - 22/10/2013 #1342 B&T
                Else
                    '#1342
                    'AG 31/10/2013 - add condition: in pause mode show always 3 options
                    'userAnswer = BSCustomMessageBox.Show(Me, confirmAbortEndSessionMessage, My.Application.Info.Title, messageBoxType, MessageBoxIcon.Question, abortButtonText, cancelButtonText, "")
                    ''
                    If (MDIAnalyzerManager.EndRunInstructionSent OrElse _
                        String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess), "INPROCESS")) _
                        AndAlso Not MDIAnalyzerManager.AllowScanInRunning Then
                        'End instruction already sent
                        'Options ABORT + CANCEL
                        ''// CF v3.0.0 bugtracking -task # 1342
                        messageBoxType = MessageBoxButtons.YesNo
                        'userAnswer = BSCustomMessageBox.Show(Me, GlobalEnumerates.Messages.CONFIRM_ABORT_WS.ToString, My.Application.Info.Title, messageBoxType, MessageBoxIcon.Question, "ABORT", "CANCEL", "")
                        userAnswer = ShowMessage(Me.Name & ".AbortSessionFromEventClicked", GlobalEnumerates.Messages.CONFIRM_ABORT_WS.ToString)

                        'AG 27/11/2013 - Task #1397 during recovery results this button only implements the abort process
                        'AG 02/12/2013 - Task #1423 in partial freeze mode is available only abort
                    ElseIf String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS") OrElse _
                        (MDIAnalyzerManager.AnalyzerIsFreeze AndAlso String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "PARTIAL")) Then
                        messageBoxType = MessageBoxButtons.YesNo
                        userAnswer = ShowMessage(Me.Name & ".AbortSessionFromEventClicked", GlobalEnumerates.Messages.CONFIRM_ABORT_WS.ToString)
                        'AG 27/11/2013

                    Else
                        'Options: STOP + ABORT + CANCEL
                        ''// CF v3.0.0 bugtracking -task # 1342
                        messageBoxType = MessageBoxButtons.YesNoCancel
                        'userAnswer = BSCustomMessageBox.Show(Me, confirmAbortEndSessionMessage, My.Application.Info.Title, messageBoxType, MessageBoxIcon.Question, stopButtonText, abortButtonText, cancelButtonText)
                        userAnswer = BSCustomMessageBox.Show(Me, confirmAbortEndSessionMessage, My.Application.Info.Title, messageBoxType, MessageBoxIcon.Question, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, stopButtonText, abortButtonText, cancelButtonText)
                    End If

                End If
                ''// CF v3.0.0 bugtracking -task # 1342
                Select Case messageBoxType
                    Case MessageBoxButtons.YesNo '//Happens when the END instruction has already been sent. 
                        If (userAnswer = Windows.Forms.DialogResult.Yes) Then
                            ' XB 24/10/2013 - When Abort Session StartSessionisPending  is initialized - BT #1343
                            StartSessionisPending = False

                            AbortSession(myGlobal)
                        End If
                    Case MessageBoxButtons.YesNoCancel '//Happens when the user wants to cancel and the END instruction has not been sent previously.
                        If userAnswer = Windows.Forms.DialogResult.Yes Then
                            ' XB 24/10/2013 - When End Session StartSessionisPending  is initialized - BT #1343
                            StartSessionisPending = False

                            '//Send END instruction
                            EndRunningSessionActions()
                        End If
                        If userAnswer = Windows.Forms.DialogResult.No Then
                            ' XB 24/10/2013 - When Abort Session StartSessionisPending  is initialized - BT #1343
                            StartSessionisPending = False

                            AbortSession(myGlobal)
                        End If
                End Select
                '// CF v3.0.0 bugtracking -task # 1342 - end of changes
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AbortSessionFromEventClicked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AbortSessionFromEventClicked ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


    ''' <summary>
    ''' Refactored method from the bsTSAbortSessionButton_Click event.
    ''' </summary>
    ''' <param name="myGlobal">GlobalDataTO Dataset with the necessary information.</param>
    ''' <remarks>Refactored method Created by CF - v3 - 21/10/2013 </remarks>
    Private Sub AbortSession(ByVal myGlobal As GlobalDataTO)
        'Disable all buttons until Ax00 accepts the new instruction
        ShowStatus(Messages.ABORTING_SESSION) 'AG 13/04/2012
        SetActionButtonsEnableProperty(False)
        EnableButtonAndMenus(False) 'TR 25/09/2012 -Disable horizontal menu and buttons bar on abort.

        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS"
        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""
        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
        myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True)
        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
            If (Not MDIAnalyzerManager.Connected) Then
                MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = ""
                ShowStatus(Messages.RUNNING) 'AG 13/04/2012 - Abort only available in running
            End If

        ElseIf (myGlobal.HasError) Then
            ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
            ShowStatus(Messages.RUNNING) 'AG 13/04/2012 - Abort only available in running
        End If

        If (Not MDIAnalyzerManager.Connected Or myGlobal.HasError) Then
            SetActionButtonsEnableProperty(True)
            EnableButtonAndMenus(True) 'TR 25/09/2012 Enable if not connected or error produced.
        Else
            SetWorkSessionUserCommandFlags(WorkSessionUserActions.ABORT_WS)

            'TR 028/02/2012 -Stop the elapsed time timer
            ElapsedTimeTimer.Stop()

            'Set WorkSession Status to INPROCESS
            Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
            myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")
            If (Not myGlobal.HasError) Then
                WSStatusAttribute = "ABORTED"
            Else
                'Error updating the WS Status to INPROCESS
                ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
            End If
        End If
    End Sub


#End Region

#Region "Public Methods"
    '''' <summary>
    '''' Enable/Disable the Menu and the menubars.
    '''' </summary>
    '''' <param name="pEnable"></param>
    '''' <remarks>CREATED BY: TR 02/08/2011
    '''' AG 07/11/2012 - comment this method. It is OLD the new method is EnableButtonAndMenus</remarks>
    'Public Sub EnableMenusBar(ByVal pEnable As Boolean)
    '    Try
    '        BsToolStrip1.Enabled = pEnable
    '        BsAx00MenuStrip.Enabled = pEnable
    '        ControlBox = pEnable
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".EnableMenusBar ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Enable/Disable Menu, ShortCut Button, Instrument tools bar
    ''' </summary>
    ''' <param name="pEnabled"></param>
    ''' <param name="pForceValue">When TRUE (automatic process of WS creation with LIS) force call SetActionButtonsEnableProperty although the menu is already enabled</param>
    ''' <remarks>CREATED BY: TR 04/10/2011
    ''' AG 07/11/2012 - avoid blinking every 2 seconds when analyzer is connected and we are in StandBy
    ''' AG 25/04/2013 - Move the activation rules for ES action into ESBusiness class
    ''' AG 15/07/2013 - Add pForceValue parameter
    ''' </remarks>
    Public Sub EnableButtonAndMenus(ByVal pEnabled As Boolean, Optional ByVal pForceValue As Boolean = False)
        Try
            ''SA 07/09/2012
            'If processingConnect Then pEnabled = False

            ' XB 25/11/2013 - Task #1303
            If ScreenChildShownInProcess Then
                pEnabled = False
            End If

            ' XB 07/11/2013
            If Not MDIAnalyzerManager Is Nothing AndAlso MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then
                pEnabled = False
            End If

            'AG 30/09/2012 - The special screen recovery results affects only to menu and shotcut bars
            'Not to the BA400 action buttons neither the control box button (for these pEnabled = True)
            Dim specialModeForRecoveryResultsScreenFlag As Boolean = False

            Dim MdiChildIsDisabled As Boolean = False
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If

            If Not myCurrentMDIForm Is Nothing AndAlso (TypeOf myCurrentMDIForm Is IResultsRecover) Then
                specialModeForRecoveryResultsScreenFlag = True
            End If
            'AG 30/09/2012

            If Not specialModeForRecoveryResultsScreenFlag Then
                'AG 07/11/2012 - To avoid blinking assign the pEnabled value only when it is different from current value
                If BsAx00MenuStrip.Enabled <> pEnabled OrElse pForceValue Then

                    'AG 24/05/2012 - Do not enables or disables the whole control (we must enable or disable all button one by one)
                    'If we disable the whole control and Sw receives some alarm who requires activate some button it can not be enabled
                    'because the control (father) is disabled 
                    'BsToolStrip2.Enabled = pEnabled
                    'SetActionButtonsEnableProperty(pEnabled) 'AG 08/11/2012 - move after ControlBox is updated
                    'AG 24/05/2012

                    BsToolStrip1.Enabled = pEnabled
                    BsAx00MenuStrip.Enabled = pEnabled
                    'bsTSSoundOff.Enabled = pEnabled       'DL 22/03/2012
                    ControlBox = pEnabled
                    SetActionButtonsEnableProperty(pEnabled)
                End If

            Else
                'Special code when the recovery results screen is loaded/unloaded
                SetActionButtonsEnableProperty(True) 'Because the ABORT button is available during results recovery (only option active)
                BsToolStrip1.Enabled = False 'Because user can NOT open other screen than the auxliary for results recovery
                BsAx00MenuStrip.Enabled = False 'Because user can NOT open other screen than the auxliary for results recovery
                ControlBox = True 'Because user can close app during results recovery
            End If

            '0- Read the required settings: LIS communication enable"
            'Dim resultData As GlobalDataTO
            'Dim userSettings As New UserSettingsDelegate
            'resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
            'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '    LISUtilitiesToolStripMenuItem.Enabled = CType(resultData.SetDatos, Boolean)
            'End If
            Dim myESBusin As New ESBusiness
            LISUtilitiesToolStripMenuItem.Enabled = myESBusin.AllowLISAction(Nothing, LISActions.LISUtilities_Menu, False, False, "", "")

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".EnableButtonAndMenus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Function to control the opening of all MDIChild Forms. This function replace the previous one (now named 
    ''' OpenMDIChildFormOLD), who has some mistakes. It assumes there can be only one MDIChild Form opened when
    ''' when trying to open another one.  Rules:
    '''   ** If there is not a screen currently open: opening action is executed.
    '''   ** If there is a screen currently open:
    '''        - If it is the same screen selected to be opened: opening action is cancelled, the screen is already open
    '''        - If the screen currently open is the MONITOR screen: it is closed and the opening action is executed.
    '''        - If it is a different screen: execute the CLICK event of the Form AcceptButton (each Application Form
    '''          that can be opened as MDIChild has to have that Property set. Once the event finishes: 
    '''             - If the screen remains opened, it means the closing was not possible and the opening action is cancelled
    '''             - If the screen was closed, then the opening action is executed
    ''' </summary>
    ''' <param name="pFormToOpen">Screen selected to be opened</param>
    ''' <returns>False is the screen opening was cancelled; otherwise, it returns True</returns>
    ''' <remarks>
    ''' Created by:  SA 17/09/2010
    ''' Modified by: SA 20/09/2010 - After validation on the ActiveMDIChild, close all open MdiChildren (to avoid problems with
    '''                              "small" forms like Language Conf., Analyzer Conf., etc)
    ''' Modified by XBC 11/07/2012 - Delay the loading operations into ISEUtilities screen just when assure 
    '''                              any previous open screen is closed
    ''' </remarks>
    Public Function OpenMDIChildForm(ByVal pFormToOpen As BSBaseForm) As Boolean
        Dim IsFormClosed As Boolean = True
        'Dim IsISEUtilClosing As Boolean = False 'SGM 10/05/2012
        Dim ISELoadingPending As Boolean = False ' XBC 11/07/2102
        Try

            'TRAZA DE APERTURA DE FORMULARIO
            If Not pFormToOpen Is Nothing AndAlso Not ActiveMdiChild Is Nothing Then
                Dim myApplicationLogMang As New ApplicationLogManager
                myApplicationLogMang.CreateLogActivity(ActiveMdiChild.Name & " ---> " & pFormToOpen.Name, "OpenMDIChildForm", EventLogEntryType.Information, False)
            End If
            'TRAZA DE APERTURA DE FORMULARIO


            EnableButtonAndMenus(False) 'TR 10/04/2012

            Cursor = Cursors.WaitCursor
            Dim FormToClose As Form = Nothing 'RH 14/12/2010
            ProcessActivate = False

            'RH 25/10/2010
            'Clear ErrorStatusLabel Text
            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None

            If (Not pFormToOpen Is Nothing) Then

                'RH 22/12/2011 Open Monitor from another screen
                If (TypeOf pFormToOpen Is IMonitor) AndAlso (Not ActiveMdiChild Is Nothing) Then
                    If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                        'Execute the closing code of the screen currently open
                        Dim myScreenName As String = ActiveMdiChild.Name
                        Cursor = Cursors.Default
                        'RH 22/12/2011 Tell the form "it is NOT closed by hand"
                        'and force it to close itself and open the Monitor form.
                        ActiveMdiChild.Tag = Nothing
                        ActiveMdiChild.AcceptButton.PerformClick()
                        Application.DoEvents()

                        'Get the closing operation result
                        If (Not ActiveMdiChild Is Nothing AndAlso String.Compare(ActiveMdiChild.Name, myScreenName, False) = 0) Then
                            IsFormClosed = False
                            ActiveMdiChild.Tag = Nothing
                        End If

                        Return IsFormClosed
                    End If
                End If

                If (Not ActiveMdiChild Is Nothing) Then
                    If (String.Compare(ActiveMdiChild.Name, pFormToOpen.Name, False) = 0) Then
                        'If the screen to open is already opened, do nothing
                        IsFormClosed = False
                    Else
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            'Close the monitor screen
                            'ActiveMdiChild.Close()
                            FormToClose = ActiveMdiChild
                            IMonitor.HideActiveAlerts()

                            MyClass.IsISEUtilClosing = False

                        ElseIf (Not ActiveMdiChild.AcceptButton Is Nothing) Then

                            'Execute the closing code of the screen currently open
                            Dim myScreenName As String = ActiveMdiChild.Name
                            Cursor = Cursors.Default
                            ClosedByFormCloseButton = False
                            ActiveMdiChild.Tag = "Performing Click" 'RH 16/12/2010 To tell the form it is closed by hand
                            ActiveMdiChild.AcceptButton.PerformClick()
                            Application.DoEvents()

                            'The screen currently open could not be closed, the opening is cancelled
                            If (Not ActiveMdiChild Is Nothing AndAlso String.Compare(ActiveMdiChild.Name, myScreenName, False) = 0) Then
                                'IsFormClosed = False

                                'SGM 10/05/2012
                                If TypeOf ActiveMdiChild Is IISEUtilities Then
                                    Dim myISEUtilities As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                    myISEUtilities.myScreenPendingToOpenWhileISEUtilClosing = pFormToOpen
                                    myISEUtilities.IsMDICloseRequested = False
                                    MyClass.IsISEUtilClosing = True
                                Else
                                    MyClass.IsISEUtilClosing = False
                                End If

                                IsFormClosed = False

                                'end SGM 10/05/2012

                                ActiveMdiChild.Tag = Nothing
                            End If

                            ' XB 26/11/2013 - When close a screen to open another (if it is not Monitor!) - Task #1303
                            ShownScreen()

                        End If
                    End If
                End If

                If (IsFormClosed) Then
                    ' XBC 11/07/2102
                    If TypeOf pFormToOpen Is IISEUtilities Then
                        ISELoadingPending = True
                    End If
                    ' XBC 11/07/2102


                    'RH 14/10/2011 Disable this button before open any other window
                    bsTSInfoButton.Enabled = False

                    'Everything is OK, open the selected screen
                    pFormToOpen.AnalyzerModel = ActiveAnalyzerModel
                    pFormToOpen.MdiParent = Me
                    MyClass.IsISEUtilClosing = False 'SGM 11/05/2012
                    pFormToOpen.Show()
                    Application.DoEvents()
                End If

                'RH 14/12/2010
                If Not FormToClose Is Nothing Then
                    If (TypeOf ActiveMdiChild Is IMonitor) Then
                        IMonitor.HideActiveAlerts()
                    End If
                    FormToClose.Close()
                    FormToClose = Nothing
                    Application.DoEvents()
                End If

            Else
                'RH 07/06/2011 Don't rely on changeable text strings, but in class type
                ' (ActiveMdiChild.Name <> "IMonitor" versus TypeOf ActiveMdiChild Is IMonitor)
                If (Not ActiveMdiChild Is Nothing) AndAlso (Not TypeOf ActiveMdiChild Is IMonitor) Then
                    'Execute the closing code of the screen currently open
                    Dim myScreenName As String = ActiveMdiChild.Name
                    ClosedByFormCloseButton = False
                    ActiveMdiChild.Tag = "Performing Click" 'RH 16/12/2010 To tell the form it is closed by hand
                    Cursor = Cursors.Default

                    'AG 30/09/2012 - special screen for recovery results has no accept button
                    'ActiveMdiChild.AcceptButton.PerformClick()
                    If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                        ActiveMdiChild.AcceptButton.PerformClick()
                    Else
                        ActiveMdiChild.Close()
                    End If
                    'AG 30/09/2012


                    'The screen currently open could not be closed, the opening is cancelled
                    If (Not ActiveMdiChild Is Nothing AndAlso String.Compare(ActiveMdiChild.Name, myScreenName, False) = 0) Then
                        'SGM 10/05/2012

                        IsFormClosed = False

                        If TypeOf ActiveMdiChild Is IISEUtilities Then
                            Dim myISEUtilities As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                            myISEUtilities.myScreenPendingToOpenWhileISEUtilClosing = Nothing
                            If Not MyClass.IsISEUtilClosing Then
                                myISEUtilities.IsMDICloseRequested = True
                                MyClass.IsISEUtilClosing = True
                            Else
                                IsFormClosed = True
                            End If
                        Else
                            MyClass.IsISEUtilClosing = False
                        End If



                        'end SGM 10/05/2012
                        ActiveMdiChild.Tag = Nothing
                    End If
                End If
                'TR 16/12/2010 -END.
            End If

        Catch ex As Exception
            IsFormClosed = False

            CreateLogActivity(ex.Message, Name & ".OpenMDIChildForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenMDIChildForm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)

        Finally

            ' XBC 11/07/2102
            If ISELoadingPending Then
                If (Not ActiveMdiChild Is Nothing) Then
                    If TypeOf ActiveMdiChild Is IISEUtilities Then
                        Dim myISEUtilities As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        myISEUtilities.PrepareLoadingMode()
                    End If
                End If
            End If
            ' XBC 11/07/2102

            'RH 20/10/2010 Move Me.Cursor = Cursors.Default here because we want to restore the default cursor in any case
            'wherever there is an exception or not
            'Cursor = Cursors.Default
            'EnableButtonAndMenus(True) 'TR 10/04/2012
            ProcessActivate = True

            'SGM 10/05/2012
            If Not IsISEUtilClosing Then
                ClosedByISEUtilCloseButton = False
                ClosedByFormCloseButton = True 'RH 16/04/2012
                Cursor = Cursors.Default
                EnableButtonAndMenus(True) 'TR 10/04/2012
                GC.GetTotalMemory(True)
            Else
                ClosedByISEUtilCloseButton = True
            End If

        End Try

        Return IsFormClosed
    End Function


    ''' <summary>
    ''' Initializes a new instance of the Ax00MainMDI class
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 15/10/2010
    ''' </remarks>
    Public Sub New()
        Try
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            Application.DoEvents()

            ' Add any initialization after the InitializeComponent() call.

            'TR 24/10/2011 
            myDriveDetector = New Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetector
            AddHandler myDriveDetector.DeviceRemoved, AddressOf OnDeviceRemoved

            Application.DoEvents()

            ''RH 22/02/2012 Get value of the Current Application Language
            'Dim myGlobalDataTO As GlobalDataTO
            'Dim myUserSettingsDelegate As New UserSettingsDelegate

            'myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)

            'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
            '    Dim CurrentLanguage As String = DirectCast(myGlobalDataTO.SetDatos, String)

            '    If String.IsNullOrEmpty(CurrentLanguage) Then
            '        CurrentLanguage = "ENG"
            '        Dim myLogAcciones As New ApplicationLogManager()
            '        myLogAcciones.CreateLogActivity("Unable to load App Current Language", "MultilanguageResourcesDelegate.UpdateCurrentLanguage", EventLogEntryType.Error, False)
            '    End If

            '    MultilanguageResourcesDelegate.SetCurrentLanguage(CurrentLanguage)
            '    CurrentLanguageAttribute = CurrentLanguage
            'End If
            ''END RH 22/02/2012

            CurrentLanguageAttribute = MultilanguageResourcesDelegate.GetCurrentLanguage() 'RH 01/03/2012

            BsLoadDefaultReportTemplates.RunWorkerAsync() 'RH 17/02/2012

            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & " New ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'RH: At this point the Ax00MainMDI new object does not exist yet.
            'As the ShowMessage() method references Ax00MainMDI object as the parent window,
            'we can't call it here, so we call MessageBox.Show() instead.
            MessageBox.Show(String.Format("{0} - {1}", GlobalEnumerates.Messages.SYSTEM_ERROR, ex.Message), _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Method used for opening the screen of Rotor Positioning
    ''' </summary>
    ''' <param name="FormToClose"></param>
    ''' <param name="pShowHQScreen"></param>
    ''' <remarks>
    ''' Created by:  RH 27/05/2010
    ''' Modified by: AG 01/06/2010 - Changed name OpenRotor for OpenRotorPositionsForm
    '''              SA 09/06/2010 - Update screen properties from Attributes
    '''              SA 28/07/2010 - Open Rotor Positioning Screen only when the Analyzer Model is A400; inform 
    '''                              property Analyzer Model
    '''              AG 03/04/2013 - New optional parameter pShowHQScreen
    '''              AG 09/07/2013 - New optional parameter to be used in the automate process of WS creation with LIS
    '''              SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous
    '''                              one still exists
    ''' </remarks>
    Public Sub OpenRotorPositionsForm(ByRef FormToClose As Form, Optional ByVal pShowHQScreen As Boolean = False, Optional ByVal pAutomateProcessWithLIS As Boolean = False)
        Try
            If (String.Compare(AnalyzerModelAttribute, "A400", False) = 0) Then
                Cursor = Cursors.WaitCursor

                SetWSActiveDataFromDB() 'AG 1/07/2011 - Get the current WSStatus value

                If (String.Compare(WorkSessionIDAttribute, String.Empty, False) = 0) Then
                    'Create and empty WS
                    Dim myWSOrderTestsDS As New WSOrderTestsDS
                    Dim myWSDelegate As New WorkSessionsDelegate
                    Dim dataToReturn As New GlobalDataTO

                    dataToReturn = myWSDelegate.AddWorkSession(Nothing, myWSOrderTestsDS, True, AnalyzerIDAttribute)
                    If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                        'Get the ID of both: Work Session and Analyzer
                        Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(dataToReturn.SetDatos, WorkSessionsDS)

                        If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                            WorkSessionIDAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                            WSStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus

                            'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                            If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                                Dim myLogAcciones As New ApplicationLogManager()
                                myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", "IAx00MainMDI.OpenRotorPositionsForm", EventLogEntryType.Error, False)
                            End If
                        End If
                    End If

                    SetWSActiveDataFromDB()
                End If

                ' XB 22/11/2013 - Task #1394
                DisplayISELockedPreparationsWarningAttribute = False

                IWSRotorPositions.ActiveAnalyzer = AnalyzerIDAttribute
                IWSRotorPositions.AnalyzerModel = AnalyzerModelAttribute
                IWSRotorPositions.ActiveWorkSession = WorkSessionIDAttribute
                IWSRotorPositions.WorkSessionStatus(MDIAnalyzerManager.AnalyzerStatus.ToString) = WSStatusAttribute
                IWSRotorPositions.ShowHostQueryScreen = pShowHQScreen 'AG 03/04/2013

                ' XB 17/07/2013 - Auto WS process
                'XB 23/07/2013 - auto HQ
                'IWSRotorPositions.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute
                IWSRotorPositions.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                'XB 23/07/2013

                'AG 09/07/2013
                IWSRotorPositions.OpenByAutomaticProcess = pAutomateProcessWithLIS
                'XB 23/07/2013 - auto HQ
                ' If autoWSCreationWithLISModeAttribute AndAlso pAutomateProcessWithLIS Then
                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso pAutomateProcessWithLIS Then
                    'XB 23/07/2013
                    SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessCreateExecutions)
                End If

                'AG 09/07/2013

                If FormToClose Is Nothing Then
                    OpenMDIChildForm(IWSRotorPositions)
                Else
                    'RH 16/12/2010 Directly opens the Rotor Positioning form and closes the calling form
                    IWSRotorPositions.MdiParent = Me
                    IWSRotorPositions.Show()
                    Application.DoEvents()
                    FormToClose.Close()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OpenRotorPositionsForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenRotorPositionsForm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Opens the WS Monitor form and closes the calling form.
    ''' Assumes there are some forms to close.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 16/12/2010
    ''' Modified by: SA 12/01/2011 - Check if button for Reset WS can be enabled before opening Monitor Screen
    '''              AG 17/07/2013 - Added optional parameter for special business for automatic WS creation with LIS
    '''              SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
    ''' </remarks>
    Public Sub OpenMonitorForm(ByRef FormToClose As Form, Optional ByVal pAutomaticProcessFlag As Boolean = False)
        Try
            'TRAZA DE APERTURA DE FORMULARIO
            If Not FormToClose Is Nothing Then
                Dim myApplicationLogMang As New ApplicationLogManager
                myApplicationLogMang.CreateLogActivity(FormToClose.Name & "---> IMonitor", "OpenMDIChildForm", EventLogEntryType.Information, False)
            End If
            'TRAZA DE APERTURA DE FORMULARIO


            EnableButtonAndMenus(False) 'RH 29/05/2012 Bugtracking 625

            Cursor = Cursors.WaitCursor
            Application.DoEvents()

            'RH 16/02/2012 Clear ErrorStatusLabel Text
            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None

            'AG 23/09/2011
            If (String.Compare(WorkSessionIDAttribute, String.Empty, False) = 0) Then
                'Create and empty WS
                Dim myWSOrderTestsDS As New WSOrderTestsDS
                Dim myWSDelegate As New WorkSessionsDelegate
                'Dim dataToReturn As New GlobalDataTO
                Dim dataToReturn As GlobalDataTO

                dataToReturn = myWSDelegate.AddWorkSession(Nothing, myWSOrderTestsDS, True, AnalyzerIDAttribute)

                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    'Get the ID of both: Work Session and Analyzer; get also the current WS status
                    Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(dataToReturn.SetDatos, WorkSessionsDS)

                    If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                        AnalyzerIDAttribute = myWorkSessionsDS.twksWorkSessions(0).AnalyzerID
                        WorkSessionIDAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                        WSStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus

                        'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                        If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", "IAx00MainMDI.OpenMonitorForm", EventLogEntryType.Error, False)
                        End If

                        'Inform WorkSession and Analyzer in the global object Analyzer Manager
                        If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                            MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                            MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                        End If

                        'Finally enable WS Buttons and Menu Options
                        EnableWSMenu()
                    End If
                End If
                'SetWSActiveDataFromDB()
            End If
            'AG 23/09/2011

            Application.DoEvents()

            'Check if button for Reset WS can be enabled
            bsTSResetSessionButton.Enabled = (Not String.IsNullOrEmpty(ActiveWorkSession))

            'Inform screen properties
            IMonitor.ActiveAnalyzer = AnalyzerIDAttribute
            IMonitor.ActiveWorkSession = WorkSessionIDAttribute
            IMonitor.CurrentWorkSessionStatus = WSStatusAttribute
            IMonitor.OpenByAutomaticProcess = pAutomaticProcessFlag 'AG 17/07/2013
            IMonitor.AutoWSCreationWithLISMode = autoWSCreationWithLISMode ' XB 17/07/2013
            Application.DoEvents()

            If (FormToClose Is Nothing) Then
                Application.DoEvents()
                OpenMDIChildForm(IMonitor)
            Else
                'RH 14/10/2011 Disable this button before open any other window
                bsTSInfoButton.Enabled = False

                IMonitor.MdiParent = Me
                IMonitor.Show()
                Application.DoEvents()
                FormToClose.Close()
            End If

            'TR 11/07/2012 -Validate if active mdiChild is IMonitor to refresh alarms globes.
            If Not ActiveMdiChild Is Nothing AndAlso String.Equals(ActiveMdiChild.Name, IMonitor.Name) Then
                IMonitor.RefreshAlarmsGlobes(Nothing)
            End If

            'DL 12/09/2011
            'Check if wup is in process and then activate BsTimerWUp, else not
            If (BsTimerWUp.Enabled) Then BsTimerWUp_Tick(Nothing, Nothing)


            ' XB 22/11/2013 - Task #1394
            If DisplayISELockedPreparationsWarningAttribute Then
                DisplayISELockedPreparationsWarningAttribute = False
                ShowMessage(Me.Name, "ISE_NEW_TEST_LOCKED")
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OpenMonitorForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenMonitorForm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            EnableButtonAndMenus(True) 'RH 29/05/2012 Bugtracking 625
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Procedure to set value to SetDisplayISELockedPreparations attribute from another forms
    ''' </summary>
    ''' <param name="pValue"></param>
    ''' <remarks>Create by XB 22/11/2013 - Task #1394</remarks>
    Public Sub SetDisplayISELockedPreparationsWarning(ByVal pValue As Boolean)
        Try
            DisplayISELockedPreparationsWarningAttribute = pValue
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetDisplayISELockedPreparationsWarning ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetDisplayISELockedPreparationsWarning ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Opens the WS Monitor form.
    ''' Assumes there are no forms to close.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 17/12/2010
    ''' Modified by: SA 09/09/2013 - Write a warning in the application LOG if the process of creation a new Empty WS was stopped due to a previous one still exists
    ''' </remarks>
    Public Sub OpenMonitorForm()
        Try
            EnableButtonAndMenus(False) 'RH 29/05/2012 Bugtracking 625

            Cursor = Cursors.WaitCursor
            Application.DoEvents()

            'RH 16/02/2012 Clear ErrorStatusLabel Text
            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.None

            'AG 23/09/2011
            If String.Equals(WorkSessionIDAttribute, String.Empty) Then
                'Create and empty WS
                Dim myWSOrderTestsDS As New WSOrderTestsDS
                Dim myWSDelegate As New WorkSessionsDelegate
                Dim dataToReturn As GlobalDataTO

                dataToReturn = myWSDelegate.AddWorkSession(Nothing, myWSOrderTestsDS, True, AnalyzerIDAttribute)

                Application.DoEvents()

                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    'Get the ID of both: Work Session and Analyzer; get also the current WS Status
                    Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(dataToReturn.SetDatos, WorkSessionsDS)

                    If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
                        AnalyzerIDAttribute = myWorkSessionsDS.twksWorkSessions(0).AnalyzerID
                        WorkSessionIDAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
                        WSStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus

                        'If there was an existing WS and the adding of a new Empty one was stopped, write the Warning in the Application LOG
                        If (myWorkSessionsDS.twksWorkSessions(0).CreateEmptyWSStopped) Then
                            Dim myLogAcciones As New ApplicationLogManager()
                            myLogAcciones.CreateLogActivity("WARNING: Source of call to add EMPTY WS when the previous one still exists", "IAx00MainMDI.OpenMonitorForm", EventLogEntryType.Error, False)
                        End If

                        'Inform WorkSession and Analyzer in the global object Analyzer Manager
                        If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                            MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                            MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                        End If

                        'Finally enable WS Buttons and Menu Options
                        EnableWSMenu()
                    End If
                End If

                'SetWSActiveDataFromDB()
                Application.DoEvents()
            End If
            'AG 23/09/2011

            IMonitor.ActiveAnalyzer = AnalyzerIDAttribute
            IMonitor.ActiveWorkSession = WorkSessionIDAttribute
            IMonitor.CurrentWorkSessionStatus = WSStatusAttribute

            'RH 14/10/2011 Disable this button before open any other window
            bsTSInfoButton.Enabled = False

            IMonitor.MdiParent = Me
            IMonitor.Show()
            Application.DoEvents()

            MyClass.IsISEUtilClosing = False 'SGM 11/05/2012

            'DL 12/09/2011
            'Check if wup is in process and then activate BsTimerWUp, else not
            If (BsTimerWUp.Enabled) Then BsTimerWUp_Tick(Nothing, Nothing)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OpenMonitorForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenMonitorForm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            EnableButtonAndMenus(True) 'RH 29/05/2012 Bugtracking 625
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Enable/disabled the menu option and button for opening screen of Rotor Positioning. Called from screen
    ''' of WS Prepartion: when the screen button is enabled/disabled the options in the MDI get the same status
    ''' </summary>
    ''' <param name="pEnabled">Indicates the status to set to the menu option and button for opening screen
    '''                        of Rotor Positioning</param>
    ''' <remarks>
    ''' Created by:  SA 02/11/2010
    ''' </remarks>
    Public Sub SetStatusRotorPosOptions(ByVal pEnabled As Boolean)
        Try
            RotorPositionsToolStripMenuItem.Enabled = pEnabled
            bsTSPositionButton.Enabled = pEnabled
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetStatusRotorPosOptions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetStatusRotorPosOptions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Set value of properties ActiveAnalyzer, ActiveWorkSession and ActiveStatus with the values received as parameters
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 26/05/2010
    ''' Modified by: SA 09/06/2010 - Added Try/Catch. Update of AnalyzerID is also optional
    ''' </remarks>
    Public Sub SetWSActiveData(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pWSStatus As String)
        Try
            WorkSessionIDAttribute = pWorkSessionID
            If Not String.Equals(pWSStatus, String.Empty) Then WSStatusAttribute = pWSStatus
            If Not String.Equals(pAnalyzerID, String.Empty) Then AnalyzerIDAttribute = pAnalyzerID

            If Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then 'AG 24/11/2010

                'AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager // 'GlobalAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                'TR 23/09/2011 -If MDIAnalyzerManager is nothing the set the value of GlobalAnalyzerManager in memory.
                If MDIAnalyzerManager Is Nothing Then
                    MDIAnalyzerManager = DirectCast(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                End If
                'TR 23/09/2011 -END.

                MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                If Not String.Equals(pAnalyzerID, String.Empty) Then MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute 'AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager //
            End If

            EnableWSMenu()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetWSActiveData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetWSActiveData ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Set value of the property that contains the number of Tests defined in DB
    ''' </summary>
    ''' <param name="pCurrentNumOfTests">Current number of tests defined in the DB</param>
    ''' <remarks>
    ''' Created by:  SA 01/07/2010
    ''' </remarks>
    Public Sub SetNumOfTests(ByVal pCurrentNumOfTests As Integer)
        Try
            NumOfTestsAttribute = pCurrentNumOfTests
            SetButtonsByNumOfTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetNumOfTests ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetNumOfTests ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize multilanguage texts, current analyzer and worksession and so on
    ''' It is used in the Load event and after load a reportSAT
    ''' </summary>
    ''' <param name="pStartingApplication" ></param>
    ''' <remarks>AG 24/11/2010
    ''' AG 22/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager
    ''' AG 25/10/2011 - add parameter pStartingApplication for differentiate when the application is started and when a reportsat or restore point is loaded
    ''' </remarks>
    Public Sub InitializeAnalyzerAndWorkSession(ByVal pStartingApplication As Boolean)
        Try
            ''Get the current application Language to set the correspondent attribute and prepare all menu options
            'Dim currentLanguageGlobal As New GlobalBase
            'CurrentLanguageAttribute = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            ShowStatus(SaveStatusMessageID) 'RH 21/03/2012

            PrepareMenuOptions()     'AG 17/06/2010
            Application.DoEvents()

            'Get values for the different Screen Properties
            GetAnalyzerInfo() 'TR 12/04/2010
            Application.DoEvents()
            SetWSActiveDataFromDB()
            Application.DoEvents()
            SetNumOfTestsFromDB()
            Application.DoEvents()

            'AG 22/04/0210 - Moved from BSBaseForm_Load
            If (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                'AG 06/09/2012 - Analyzer manager initialization requires WorkSessionID to be informed before AnalyzerID
                'MDIAnalyzerManager = New AnalyzerManager(My.Application.Info.AssemblyName, MyClass.AnalyzerModel) With {.ActiveAnalyzer = AnalyzerIDAttribute, .ActiveWorkSession = WorkSessionIDAttribute}
                MDIAnalyzerManager = New AnalyzerManager(My.Application.Info.AssemblyName, MyClass.AnalyzerModel) With {.StartingApplication = pStartingApplication, _
                                                                                                                        .ActiveWorkSession = WorkSessionIDAttribute, _
                                                                                                                        .ActiveAnalyzer = AnalyzerIDAttribute, _
                                                                                                                        .ActiveFwVersion = FwVersionAttribute}
                ' XBC 08/06/2012

                Application.DoEvents()

                Dim blnStartComm As Boolean = False

                'AG 22/09/2011
                'blnStartComm = GlobalAnalyzerManager.Start(False)   'AG 21/04/2010 Start the CommAx00 process
                'AppDomain.CurrentDomain.SetData("GlobalAnalyzerManager", GlobalAnalyzerManager)
                'MDIAnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 02/11/2010

                blnStartComm = MDIAnalyzerManager.Start(False)   'AG 21/04/2010 Start the CommAx00 process
                AppDomain.CurrentDomain.SetData("GlobalAnalyzerManager", MDIAnalyzerManager)
                Application.DoEvents()

                'If Not bsAnalyzerStatus Is Nothing Then
                '    bsAnalyzerStatus.DisplayStyle = ToolStripItemDisplayStyle.Text
                '    'bsAnalyzerStatus.Text = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING.ToString
                '    bsAnalyzerStatus.Text = "CONNECTING"


                'End If

                ' XBC 04/06/2012 - it is emplaced on MDIAnalyzerManager Set Property ActiveAnalyzer()
                'MDIAnalyzerManager.InitBaseLineCalculations(Nothing, pStartingApplication)

                Application.DoEvents()
                'AG 24/11/2010
            Else
                ' XBC 14/06/2012
                MDIAnalyzerManager.StartingApplication = pStartingApplication

                'AG 22/09/2011
                MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute 'GlobalAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute 'GlobalAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute

                ' XBC 04/06/2012 - it is emplaced on MDIAnalyzerManager Set Property ActiveAnalyzer()
                'MDIAnalyzerManager.InitBaseLineCalculations(Nothing, pStartingApplication) 'GlobalAnalyzerManager.InitBaseLineCalculations(Nothing)

                'END AG 24/11/2010
            End If

            ' XBC 15/06/2012
            Dim isReportSATLoading As Boolean = Not pStartingApplication
            Dim myLogAccionesAux As New ApplicationLogManager()
            myLogAccionesAux.CreateLogActivity("(Analyzer Change) calling function ManageAnalyzerConnected ... - ReportSAT loading [" & isReportSATLoading & "] ", Name & ".InitializeAnalyzerAndWorkSession ", EventLogEntryType.Information, False)

            ManageAnalyzerConnected(False, isReportSATLoading)
            ' XBC 15/06/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitializeAnalyzerAndWorkSession ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeAnalyzerAndWorkSession ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        End Try
    End Sub


    ''' <summary>
    ''' Start Marquee progress bar
    ''' </summary>
    ''' <remarks>
    ''' Created by DL 14/03/2011
    ''' </remarks>
    Public Sub InitializeMarqueeProgreesBar()

        If Not ProgressBar.Visible Then
            ProgressBar.Visible = True
            ProgressBar.Properties.Stopped = False
            ProgressBar.BringToFront()
            ProgressBar.Show()
        End If

        'ProgressBar.Update()
        'ProgressBar.Refresh()
        Application.DoEvents()
    End Sub


    Public Sub StopMarqueeProgressBar()
        ProgressBar.Visible = False
        ProgressBar.Properties.Stopped = True
        'ProgressBar.Refresh()

        Application.DoEvents()
    End Sub

    ''' <summary>
    ''' Activate/deactivate the action buttons (for Instrument Control and WorkSession Control)
    ''' </summary>
    ''' <param name="pEnable">Indicates if buttons have to be activated or deactivated</param>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 
    ''' </remarks>
    Public Sub SetActionButtonsEnableProperty(ByVal pEnable As Boolean)
        Try

            If pEnable Then
                'The button activation depends on instrument status, alarms,...
                'in some cases a new action is sent to the instrument

                ' XBC 21/05/2012 - None button can stay activated while Analyzer is warming up
                If (Not MDIAnalyzerManager Is Nothing) AndAlso _
                   ((Not String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) = 0) OrElse _
                    MDIAnalyzerManager.AnalyzerIsFreeze OrElse _
                    Not StartingSession) Then  ' XB 31/10/2013

                    ActivateActionButtonBarOrSendNewAction()

                End If

                If Not MDIAnalyzerManager.Connected Then
                    ActivateActionButtonBarOrSendNewAction()
                End If

                'AG 28/02/2013
                If Not MDILISManager Is Nothing Then
                    ActivateLISActionButton()
                End If
                'AG 28/02/2013

            Else
                'Instrument buttons
                bsTSStartInstrumentButton.Enabled = pEnable

                'bsTSConnectButton.Enabled = pEnable
                If Not AutoConnectProcess Then bsTSConnectButton.Enabled = pEnable

                bsTSShutdownButton.Enabled = pEnable
                WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                bsTSChangeBottlesConfirm.Enabled = pEnable
                bsTSRecover.Enabled = pEnable

                'Session buttons
                'bsTSStartSessionButton.Enabled = pEnable
                'bsTSPauseSessionButton.Enabled = pEnable
                'bsTSContinueSessionButton.Enabled = pEnable
                bsTSAbortSessionButton.Enabled = pEnable
                'JV + AG 21/10/2013 task # 1341
                bsTSMultiFunctionSessionButton.Enabled = pEnable
                showSTARTWSiconFlag = False
                showPAUSEWSiconFlag = False
                'bsTSMultiFunctionSessionButton.Image = imagePlay
                'JV + AG 21/10/2013 task # 1341

                ' XB 01/08/2013 - add functionality according to disable LIS buttons
                If DisableLISButtons() Then
                    'bsTSStartSessionButton.Enabled = False
                    'JV + AG revision 18/10/2013 task # 1341
                    bsTSMultiFunctionSessionButton.Enabled = False
                    'JV + AG revision 18/10/2013 task # 1341
                End If

                'Buttons over analyzer not in Vertical Buttons Bar
                'Barcode button
                If Not ActiveMdiChild Is Nothing Then
                    If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                        Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                        If Not CurrentMdiChild.bsScanningButton Is Nothing Then CurrentMdiChild.bsScanningButton.Enabled = pEnable
                        If Not CurrentMdiChild.bsCheckRotorVolumeButton Is Nothing Then CurrentMdiChild.bsCheckRotorVolumeButton.Enabled = pEnable
                        If Not CurrentMdiChild.bsReagentsCheckVolumePosButton Is Nothing Then CurrentMdiChild.bsReagentsCheckVolumePosButton.Enabled = pEnable
                        'CurrentMdiChild.bsSamplesCheckVolumePosButton.Enabled = pEnable

                    ElseIf (TypeOf ActiveMdiChild Is IWSSampleRequest) Then
                        Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                        If Not CurrentMdiChild.bsScanningButton Is Nothing Then CurrentMdiChild.bsScanningButton.Enabled = pEnable
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & " SetActionButtonsEnableProperty ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Activate/deactivate main tab (start Instrument)
    ''' </summary>
    ''' <param name="pEnable">Indicates if main tab have to be activated or deactivated</param>
    ''' <remarks>
    ''' Created by:  DL 26/03/2012
    ''' Modified by: RH 27/03/2012
    ''' Modified by: DL 17/05/2012 add pRemoveAllAlarms parameter. Remove the current alarms when required.
    ''' </remarks>
    Private Sub SetEnableMainTab(ByVal pEnable As Boolean, Optional ByVal pRemoveAllAlarms As Boolean = False)
        Try
            ' XBC 28/06/2012
            If Me.ShutDownisPending Or Me.StartSessionisPending Then
                pEnable = False
            End If
            ' XBC 28/06/2012

            'DL 17/05/2012
            If Not MDIAnalyzerManager Is Nothing AndAlso Not pEnable AndAlso pRemoveAllAlarms Then
                MDIAnalyzerManager.Alarms.Clear()
                If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IMonitor Then
                    IMonitor.RefreshAlarmsGlobes(Nothing)
                End If
            End If
            'DL 17/05/2012

            If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IMonitor Then
                IMonitor.SetEnableMainTab(pEnable)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " SetEnableMainTab", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetEnableMainTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message, MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' Finish warm
    ''' </summary>
    ''' <param name="isAlarm">Indicates if is an alarm</param>
    ''' <remarks>
    ''' Created by   DL 20/01/2012 
    ''' Modified by  TR 16/03/2012 - When user press END WARM UP button leave the Frame visible and the progress bar in 100%
    ''' Note by      DL 10/10/2012 - Si se añade algun envio de instrucción se ha de tener en cuenta que esta funcion se llama tambien desde el shutdown.
    ''' Modified by  XB 30/01/2013 - DateTime to Invariant Format (Bugs tracking #1121)
    ''' </remarks>
    Public Sub FinishWarmUp(ByVal isAlarm As Boolean)

        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow



            If Not isAlarm Then
                ShowStatus(Messages.STANDBY) 'RH 21/03/2012
                Me.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = False 'AG 16/05/2012

                'WUPCOMPLETEFLAG
                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                With myAnalyzerSettingsRow
                    .AnalyzerID = AnalyzerIDAttribute
                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                    .CurrentValue = "0"
                End With
                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                'WUPCOMPLETEFLAG
                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                With myAnalyzerSettingsRow
                    .AnalyzerID = AnalyzerIDAttribute
                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
                    .CurrentValue = ""
                End With
                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                resultData = myAnalyzerSettings.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

                If Not resultData.HasError Then
                    'Update UI
                    WarmUpFinishedAttribute = True
                    BsTimerWUp.Enabled = False
                    SetActionButtonsEnableProperty(True)

                    If Not ActiveMdiChild Is Nothing Then
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            'CurrentMdiChild.TimeWarmUpProgressBar.Position =0 'TR 16/03/2012 Commented
                            CurrentMdiChild.TimeWarmUpProgressBar.Position = 100 'TR 16/03/2012 -Set the value 100%
                            'CurrentMdiChild.bsWamUpGroupBox.Enabled = False
                            'CurrentMdiChild.bsWamUpGroupBox.Visible = False 'TR 16/03/2012 Commented.
                            CurrentMdiChild.bsWamUpGroupBox.Visible = True ' TR 16/03/2012 -Set visitble value to True.
                            CurrentMdiChild.bsEndWarmUp.Visible = False
                        End If
                    End If

                End If

            Else
                ''WUPCOMPLETEFLAG
                'myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                'With myAnalyzerSettingsRow
                '    .AnalyzerID = AnalyzerIDAttribute
                '    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                '    .CurrentValue = "0"
                'End With
                'myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                'WUPCOMPLETEFLAG
                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                With myAnalyzerSettingsRow
                    .AnalyzerID = AnalyzerIDAttribute
                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
                    ''.CurrentValue = Now.ToString 'AG + SA 05/10/2012
                    '.CurrentValue = Now.ToString("yyyy/MM/dd HH:mm:ss")
                    .CurrentValue = Now.ToString(CultureInfo.InvariantCulture)
                End With
                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                resultData = myAnalyzerSettings.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

                If Not resultData.HasError Then
                    'Update UI
                    WarmUpFinishedAttribute = True
                    BsTimerWUp.Enabled = False
                    SetActionButtonsEnableProperty(True)

                    If Not ActiveMdiChild Is Nothing Then
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.TimeWarmUpProgressBar.Position = 0
                            'CurrentMdiChild.bsWamUpGroupBox.Enabled = False
                            CurrentMdiChild.bsWamUpGroupBox.Visible = False
                            CurrentMdiChild.bsEndWarmUp.Visible = False
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".FinishWarmUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FinishWarmUp ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Shows a message text in the status bar
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 20/03/2012 (AG define as public)
    ''' </remarks>
    Public Sub ShowStatus(ByVal pMessageID As GlobalEnumerates.Messages)
        Try
            'AG + DL 17/07/2013 - Execute this code only when current text not is Waiting for LIS
            If bsAnalyzerStatus.Text.ToString.Trim <> myAutoLISWaitingOrder.ToString.Trim Then
                Dim msgText As String = String.Empty
                SaveStatusMessageID = pMessageID

                If pMessageID <> Messages._NONE Then
                    Dim myMessageMessageDelegate As New MessageDelegate()
                    Dim myMessagesDS As MessagesDS
                    Dim myGlobalDataTO As GlobalDataTO

                    myGlobalDataTO = myMessageMessageDelegate.GetMessageDescription(Nothing, pMessageID.ToString())

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                        msgText = myMessagesDS.tfmwMessages(0).MessageText
                    Else
                        'Do something with the error
                        ShowMessage(Name & ".ShowStatus ", Messages.SYSTEM_ERROR.ToString(), myGlobalDataTO.ErrorCode)
                    End If
                End If

                bsAnalyzerStatus.Text = String.Format("{0} ", msgText)
            End If
        Catch ex As Exception
            'Remove Status text
            bsAnalyzerStatus.Text = String.Empty

            CreateLogActivity(ex.Message, Me.Name & ".ShowStatus", EventLogEntryType.Error, False)
            ShowMessage(Name & ".ShowStatus ", Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Sets error status label's text
    ''' </summary>
    ''' <param name="pMsg"></param>
    ''' <remarks></remarks>
    Public Sub SetErrorStatusMessage(ByVal pMsg As String)
        Try
            ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            ErrorStatusLabel.Text = pMsg
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SetErrorStatusMessage", EventLogEntryType.Error, False)
            ShowMessage(Name & ".SetErrorStatusMessage ", Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Activate buttons depending the current alarms
    ''' 
    ''' NOTE: Sw has to check if the cover are disable before consider this cover alarm treatment
    ''' </summary>
    ''' <param name="pButton"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  AG
    ''' Modified by: AG 28/03/2012 - Define as public and add actions buttons in other screens (utilities, bar code read, ...)
    '''              AG 14/05/2012 - When ISE utilities screen is active the only vertical buttons that can be enabled are: ChangeBottle confirm, Recover and Sound Off
    '''              SA 15/10/2013 - BG #1334 ==> Changes in the availability of BARCODE Button when Analyzer Status is RUNNING: 
    '''                                            ** If it is not in PAUSE Mode: Barcode Button is disabled
    '''                                            ** If it is in PAUSE Mode but the Analyzer is FREEZE due to an Alarm: Barcode Button is disabled
    '''                                            ** Otherwise: Barcode Button is enabled  
    '''              AG 06/11/2013 - Task #1375 (take into account the start/continue WS is also allowed in running paused mode)
    ''' </remarks>
    Public Function ActivateButtonWithAlarms(ByVal pButton As GlobalEnumerates.ActionButton) As Boolean
        Dim myStatus As Boolean = True
        Try
            Dim resultData As New GlobalDataTO
            Dim myAlarms As New List(Of GlobalEnumerates.Alarms)

            ' AG+XBC 24/05/2012
            'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
            Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
            ' AG+XBC 24/05/2012

            myAlarms = MDIAnalyzerManager.Alarms
            myAx00Status = MDIAnalyzerManager.AnalyzerStatus

            'AG 25/10/2011 - Before treat the cover alarms read if they are deactivated (0 disabled, 1 enabled)
            Dim mainCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN), 1, 0), Boolean)
            Dim reactionsCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN), 1, 0), Boolean)
            Dim fridgeCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN), 1, 0), Boolean)
            Dim samplesCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_WARN), 1, 0), Boolean)

            resultData = MDIAnalyzerManager.ReadFwAdjustmentsDS
            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                'Dim myAdjDS As SRVAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS) 'Causes system error in develop mode
                Dim myAdjDS As New SRVAdjustmentsDS
                myAdjDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                'Main cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.MCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then mainCoverAlarm = False
                End If
                'Reactions cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then reactionsCoverAlarm = False
                End If
                'Reagents cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.RCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then fridgeCoverAlarm = False
                End If
                'Samples cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.SCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then samplesCoverAlarm = False
                End If
            End If
            'AG 25/10/2011

            'AG 02/04/2012
            '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
            Dim iseInitiatedFinishedFlag As Boolean = True
            If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
               (MDIAnalyzerManager.ISE_Manager.IsISEInitiating OrElse Not MDIAnalyzerManager.ISE_Manager.ConnectionTasksCanContinue) Then
                iseInitiatedFinishedFlag = False
            End If
            'AG 02/04/2012

            'AG 14/05/2012 - When ISE utilities screen is active the only vertical buttons that can be enabled are: ChangeBottle confirm, Recover & Sound Off
            Dim iSEUtilitiesActiveFlag As Boolean = False
            If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IISEUtilities Then
                iSEUtilitiesActiveFlag = True
            End If
            'AG 14/05/2012

            Select Case pButton
                'AG 14/09/2011 - Use the adjustment for treat or not the covers alarms (tested ok 26/10/2011 AG with cover detection disabled)
                'START INSTRUMENT button
                Case ActionButton.START_INSTRUMENT
                    If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                       mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse samplesCoverAlarm OrElse iSEUtilitiesActiveFlag Then
                        myStatus = False
                    Else
                        myStatus = True
                    End If

                    'SHUT DOWN INSTRUMENT button
                Case ActionButton.SHUT_DOWN
                    If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                       mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse samplesCoverAlarm OrElse iSEUtilitiesActiveFlag Then
                        myStatus = False
                    Else
                        If StartSessionisPending Or StartingSession Then  ' XB 30/10/2013 
                            myStatus = False
                        End If
                    End If

                    'START running button
                Case GlobalEnumerates.ActionButton.START_WS
                    'AG 06/11/2013 - Task #1375
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                        'AG 06/11/2013 - Task #1375

                        If mainCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                            reactionsCoverAlarm OrElse _
                            fridgeCoverAlarm OrElse _
                            samplesCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'CONTINUE running button
                Case GlobalEnumerates.ActionButton.CONTINUE_WS
                    'AG 06/11/2013 - Task #1375
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                        'AG 06/11/2013 - Task #1375
                        If mainCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                            reactionsCoverAlarm OrElse _
                            fridgeCoverAlarm OrElse _
                            samplesCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'CHANGE BOTTLE CONFIRMATION
                Case ActionButton.CHANGE_BOTTLES_CONFIRM
                    myStatus = False
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'Depending the analyzer alarms activate or not the button (myStatus = True or False)
                        'If myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                        '   myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then
                        If MDIAnalyzerManager.ExistBottleAlarms Then
                            myStatus = True

                        End If
                    End If

                    'AG 28/03/2012 - READ BARCODE FROM SAMPLE REQUEST OR ROTOR POSITIONING SCREENS
                Case ActionButton.READ_BARCODE
                    If (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY) Then
                        If (mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag) Then
                            myStatus = False
                        End If

                        'SA 15/10/2013 - ADDED NEW CASES FOR BUTTON AVAILABILITY IN RUNNING 
                    ElseIf (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                        If (Not MDIAnalyzerManager.AllowScanInRunning) Then
                            myStatus = False
                        Else
                            'If Analyzer is in PAUSE during RUNNING, Barcode is available excepting when the Analyzer is FREEZE 
                            myStatus = (Not MDIAnalyzerManager.AnalyzerIsFreeze)
                        End If
                    Else
                        myStatus = False
                    End If

                    'CHECK BOTTLE VOLUMES FROM ROTOR POSITIONING SCREEN
                Case ActionButton.CHECK_BOTTLE_VOLUME
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        If mainCoverAlarm OrElse _
                           reactionsCoverAlarm OrElse _
                           fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'RAISE WASHING STATION FROM CHANGE REACTIONS ROTOR UTILITY SCREEN
                Case ActionButton.RAISE_WASH_STATION
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'AG 03/05/2012 - do not allowed when ise not initiated and reactions rotor is found
                        'If Not iseInitiatedFinishedFlag Then
                        If Not iseInitiatedFinishedFlag AndAlso Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) Then
                            myStatus = False
                        End If

                    Else
                        myStatus = False
                    End If

                    'CHANGE REACTIONS ROTOR FROM CHANGE REACTIONS ROTOR UTILITY SCREEN
                Case ActionButton.CHANGE_REACTIONS_ROTOR
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'AG 03/05/2012 - do not allowed when reactions rotor cover error or (ise not initiated and reactions rotor is found)
                        'If reactionsCoverAlarm OrElse Not iseInitiatedFinishedFlag Then
                        If reactionsCoverAlarm OrElse (Not iseInitiatedFinishedFlag AndAlso Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR)) Then
                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'ISE COMMAND FROM ISE UTILITIES SCREEN
                Case ActionButton.ISE_COMMAND
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        If mainCoverAlarm OrElse _
                           reactionsCoverAlarm OrElse _
                           fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If
                    'AG 28/03/2012

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ActivateButtonWithAlarms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateButtonWithAlarms, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myStatus
    End Function

    ''' <summary>
    ''' Automate process of WS creation with LIS only with an user click (steps finish monitor loading + enter running in a parallel threading)
    ''' Define as public because is called after Monitor screen finishes load
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 09/07/2013
    ''' Modified by: XB 22/07/2013 - Add HQProcessByUserFlag condition for LIS auto HQ management
    ''' Modified by AG 04/11/2013 - Task #1375
    ''' </remarks>
    Public Sub FinishAutomaticWSWithLIS()
        Try
            If (automateProcessCurrentState <> LISautomateProcessSteps.notStarted) Then
                'Check if the conditions to enter running are still active
                Dim myWSDelegate As New WorkSessionsDelegate
                Dim pendingExecutionsLeft As Boolean = False
                Dim executionsNumber As Integer = 0
                Dim myGlobal As New GlobalDataTO
                myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)

                'AG 04/11/2013 - Task #1375 (in paused mode user has always the option for return to running normal mode
                If Not pendingExecutionsLeft AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                    pendingExecutionsLeft = True 'Set this flag to TRUE so user could ACCEPT message (continue in same status) or CANCEL message (and return to running normal mode)
                End If
                'AG 04/11/2013

                Dim conditionsOK As Boolean = False
                If executionsNumber > 0 Then
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        Dim wsStarted As Boolean = CType(myGlobal.SetDatos, Boolean)

                        If Not wsStarted AndAlso pendingExecutionsLeft Then
                            conditionsOK = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                        ElseIf pendingExecutionsLeft Then
                            conditionsOK = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                        Else 'All Executions pending are paused
                            conditionsOK = False
                        End If
                    End If
                End If

                Dim auxStr As String = "Failed, not running"
                If pausingAutomateProcessFlag Then
                    auxStr = "Skipped. User clicked PAUSE button"
                Else
                    If conditionsOK Then
                        auxStr = "OK. Go to Running!!!"
                    End If
                End If
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: Evaluate conditions before go Running. Result: " & auxStr, "IAx00MainMDI.FinishAutomaticWSWithLIS", EventLogEntryType.Information, False)

                If conditionsOK AndAlso Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then
                    SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessEnterRunning)
                    'Finally enter running
                    Cursor = Cursors.WaitCursor
                    EnableButtonAndMenus(False) 'Disable UI
                    SetActionButtonsEnableProperty(False) 'AG 09/07/2013
                    readBarcodeIfConfigured = False 'Enter running without barcode read neither warnings
                    InitializeAutoWSFlags() 'AG 30/07/2013 - Initialize HQ variables before go to Running
                    'StartEnterInRunningMode()
                    StartSession(True)
                Else
                    'Mark process as not started
                    SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                    InitializeAutoWSFlags() 'AG 30/07/2013 - Initialize HQ variables before enable buttons
                    'Reactivate the buttons once the process has been finished
                    EnableButtonAndMenus(True, True)
                End If

                'InitializeAutoWSFlags() 'AG 30/07/2013 - Here not. The HQ variables must be initialized before
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".FinishAutomaticWSCreationWithLISProcess ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FinishAutomaticWSCreationWithLISProcess ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' Assign value the automate process current state variable and also the flag that indicates if the process is pausing
    ''' </summary>
    ''' <param name="pValue"></param>
    ''' <remarks>
    ''' Created by: AG 10/07/2013
    ''' </remarks>
    Public Sub SetAutomateProcessStatusValue(ByVal pValue As LISautomateProcessSteps)
        automateProcessCurrentState = pValue

        If pValue = LISautomateProcessSteps.ExitAutomaticProcessPaused Then
            pausingAutomateProcessFlag = True
        ElseIf pValue = LISautomateProcessSteps.notStarted OrElse pValue = LISautomateProcessSteps.initialStatus Then
            pausingAutomateProcessFlag = False

            ''AG + XB 27/07/2013
            'If pValue = LISautomateProcessSteps.notStarted Then
            '    InitializeAutoWSFlags()
            'End If
            ''AG + XB 27/07/2013
            'If bsTSPauseSessionButton.Enabled Then

            '    Me.UIThread(Function() bsTSPauseSessionButton.Enabled = False) 'SGM 22/07/2013
            'End If
            'JV + AG revision 18/10/2013 task # 1341
            showPAUSEWSiconFlag = False
            If bsTSMultiFunctionSessionButton.Enabled Then
                Me.UIThread(Function() bsTSMultiFunctionSessionButton.Enabled = False)
            End If
            'JV + AG revision 18/10/2013 task # 1341
        End If
    End Sub

    ''' <summary>
    ''' Activate/Deactivate the Main MDI variable that indicates a HQ was sent by a final User
    ''' </summary>
    ''' <param name="pValue">True when the HQ was sent by a final User; False when the HQ was sent automatically</param>
    ''' <remarks>
    ''' Created by:  XB 24/07/2013
    ''' </remarks>
    Public Sub SetHQProcessByUserFlag(ByVal pValue As Boolean)
        HQProcessByUserFlag = pValue

        'AG + XB 27/07/2013 - When process finishes force enable user interface
        If (Not pValue) Then
            Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
            If (Not ActiveMdiChild Is Nothing) Then
                ActiveMdiChild.Enabled = True

            ElseIf (DisabledMDIChildAttribute.Count > 0) Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
                myCurrentMDIForm.Enabled = True
                If (DisabledMDIChildAttribute.Count = 1) Then
                    DisabledMDIChildAttribute.Clear()
                End If
            End If
        End If
        'AG + XB 27/07/2013
    End Sub

    ''' <summary>
    ''' Initialize Auto WS flags status
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 24/07/2013
    ''' Modified by: SA 21/10/2013 - BT #1349 ==> Added code to get value of setting LIS_ENABLE_COMM. Value of attribute autoWSCreationWithLISModeAttribute
    '''                              will depend also on value of this setting, instead of only on value of setting AUTO_WS_WITH_LIS_MODE
    ''' </remarks>
    Public Sub InitializeAutoWSFlags()
        Try
            Dim myResultData As New GlobalDataTO
            Dim myUserSettingDelegate As New UserSettingsDelegate

            'Deactivate the Main MDI variable that indicates when a HQ was sent by a final User
            SetHQProcessByUserFlag(False)

            'BT #1349 ==> Verify if LIS Communication is enabled. Get value of setting LIS_ENABLE_COMMS
            Dim lisCommunicationsON As Boolean = False
            myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
            If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                lisCommunicationsON = CType(myResultData.SetDatos, Boolean)
            End If

            'Verify if automatic WS creation with LIS is enabled. Get value of setting AUTO_WS_WITH_LIS_MODE
            Dim autoLisWsON As Boolean = GlobalConstants.AUTO_WS_WITH_LIS_MODE
            myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString())
            If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                autoLisWsON = CType(myResultData.SetDatos, Boolean)
            End If

            'BT #1349 ==> Set value of attribute autoWSCreationWithLISModeAttribute depending on following conditions:
            '** LIS Communications are ON AND ALSO (Automatic WS creation with LIS is ON OR ELSE the User has sent the HOST QUERY manually)
            autoWSCreationWithLISModeAttribute = (lisCommunicationsON AndAlso autoLisWsON) 'OrElse HQProcessByUserFlag) --> This condition is not needed due to this variable was set to FALSE (SetHQProcessByUserFlag)

            'Finally, inform the property in the AnalyzerManager Class
            MDIAnalyzerManager.autoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message, Name & ".InitializeHQProcessByUserFlag ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeHQProcessByUserFlag ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' Verify if LIS Buttons can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 01/08/2013
    ''' </remarks>
    Public Function DisableLISButtons() As Boolean
        Dim mydisableButtons As Boolean = False
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                If (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS") Or _
                   (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS") Or _
                   (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS") Or _
                   (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS") Or _
                   (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
                    mydisableButtons = True
                End If

                If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                    mydisableButtons = True
                End If

                If (WSStatusAttribute = "ABORTED") Then
                    mydisableButtons = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".DisableLISButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisableLISButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return mydisableButtons
    End Function


    ''' <summary>
    ''' Search how many time the application will be locked receiving Orders sent by LIS after an automatic HQ has been sent 
    ''' </summary>
    ''' <param name="pStatus">To enable/disable property LISWaitTimer</param>
    ''' <param name="pTotalSpecimen">Total number of Specimens for which a HQ should be sent to LIS. NOT USED!!</param>
    ''' <remarks>
    ''' Created by:  ?
    ''' Modified by: TR 16/07/2013 - Implemented the waiting time by the number of Specimens
    '''              AG 24/07/2013 - Disabled all code to calculate the waiting time according the number of Specimens
    ''' </remarks>
    Public Sub EnableLISWaitTimer(ByVal pStatus As Boolean, Optional pTotalSpecimen As Integer = 0)
        Try
            If (pStatus) Then
                Dim myResultData As GlobalDataTO
                Dim myUserSettingDelegate As New UserSettingsDelegate

                myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_LIS_WAIT_TIME.ToString())
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    LISWaitTimer.Interval = 1000 * CType(myResultData.SetDatos, Integer)
                Else
                    LISWaitTimer.Interval = CDbl(GlobalConstants.AUTO_LIS_WAIT_TIME * 1000)
                End If

                'AG 24/07/2013
                'Multiply by the amount of speciments
                'If pTotalSpecimen > 0 Then
                '    'Get package size
                '    myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_HOST_QUERY_PACKAGE.ToString())
                '    Dim PackageSize As Integer = 1
                '    If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                '        PackageSize = CType(myResultData.SetDatos, Integer)
                '    End If
                '    'Calculate the Total Speciment divided by the packageSize
                '    Dim CalcPackageAndSpecimen As Single = CSng(pTotalSpecimen / PackageSize)
                '    'get the integer part.
                '    Dim IntPart As Integer = CInt(CalcPackageAndSpecimen)
                '    'Get the decimal part of the number.
                '    Dim DecPart As Single = CalcPackageAndSpecimen - IntPart

                '    If DecPart > 0 Then
                '        'Add 1 to CalcPackageAndSpeciment
                '        CalcPackageAndSpecimen = IntPart + 1
                '    Else
                '        CalcPackageAndSpecimen = IntPart
                '    End If

                '    LISWaitTimer.Interval = LISWaitTimer.Interval * CalcPackageAndSpecimen
                'End If
                'AG 24/07/2013
            End If
            LISWaitTimer.Enabled = pStatus
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".EnableLISWaitTimer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EnableLISWaitTimer ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Manage all exception cases in process for auto create worksession with LIS and execute it
    ''' </summary>
    ''' <param name="pExceptionNumber"></param>
    ''' <param name="pOwnerWindow">Used when this method is called from a not MDIChild screen</param>
    ''' <param name="pPatientTubesNumber">Number of patient tubes read by barcode in the samples rotor</param>
    ''' <param name="pSkipSound">When FALSE the analyzer starts ringing before show the messagebox and stops ringing when user chooses one option
    '''                          The alarm sound is skipped for those messages shown inmediately after user action</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  AG 12/07/2013
    ''' Modified by: AG 22/07/2013 - Analyzer starts ringing everytime the process pauses and requires an user action. Stops ringing after the user chooses option
    ''' Modified by AG 04/11/2013 - Task #1375
    ''' </remarks>
    Public Function CheckForExceptionsInAutoCreateWSWithLISProcess(ByVal pExceptionNumber As Integer, Optional ByVal pOwnerWindow As IWin32Window = Nothing, _
                                                                   Optional ByVal pPatientTubesNumber As Integer = 0, Optional pSkipSound As Boolean = False) As DialogResult
        Dim myGlobal As New GlobalDataTO
        Dim toReturnValue As DialogResult = DialogResult.Yes 'Yes means no warning!!
        Try
            ' XB 29/07/2013 - Host Query displays warning messages only with Accept option
            Dim displayOnlyAcceptOption As Boolean = False
            displayOnlyAcceptOption = HQProcessByUserFlag
            ' XB 29/07/2013

            'Check if there are pending executions
            Dim myWSDelegate As New WorkSessionsDelegate
            Dim pendingExecutionsLeft As Boolean = False
            Dim executionsNumber As Integer = 0
            myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)

            'AG 04/11/2013 - Task #1375 (in paused mode user has always the option for return to running normal mode
            If Not pendingExecutionsLeft AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                pendingExecutionsLeft = True 'Set this flag to TRUE so user could ACCEPT message (continue in same status) or CANCEL message (and return to running normal mode)
            End If
            'AG 04/11/2013

            If (pOwnerWindow Is Nothing) Then
                If Me.MdiParent Is Nothing Then
                    pOwnerWindow = Me
                Else
                    pOwnerWindow = Me.MdiParent
                End If
            End If

            Dim addUserAnswerText As String = ""
            Select Case pExceptionNumber
                Case 1 'Check the LIS connection status just after START WS or CONTINE WS button click event
                    'Check conditions for HostQuery (if true continue, else show warning message)
                    Dim myESRulesDlg As New ESBusiness
                    Dim LISStatusOK As Boolean = myESRulesDlg.AllowLISAction(Nothing, LISActions.HostQuery_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage)

                    If Not LISStatusOK Then
                        'AG 22/07/2013 - Activate buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StartAnalyzerRinging()
                            System.Threading.Thread.Sleep(500)
                        End If
                        'AG 22/07/2013

                        If executionsNumber > 0 AndAlso pendingExecutionsLeft AndAlso Not displayOnlyAcceptOption Then
                            'Message with OKCancel type (defined messageType)
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_LIS_NOT_READY.ToString, "", pOwnerWindow)
                        Else
                            'Message with OK type
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_LIS_NOT_READY.ToString, "", pOwnerWindow, Nothing, "", "Warning")
                        End If

                        'AG 22/07/2013 - Close buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StopAnalyzerRinging()
                        End If
                        'AG 22/07/2013
                    End If

                    'Add Exception to log
                    If toReturnValue = DialogResult.Yes Then
                        addUserAnswerText = "AutoCreate WS with LIS: No exception (HostQuery available)"
                    Else
                        addUserAnswerText = "AutoCreate WS with LIS exception: HostQuery NOT available. "
                    End If

                Case 2 'Check the patient tubes read by the barcode reader (if any show warning message) 
                    If pPatientTubesNumber = 0 Then
                        'AG 22/07/2013 - Activate buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StartAnalyzerRinging()
                        End If
                        'AG 22/07/2013

                        If executionsNumber > 0 AndAlso pendingExecutionsLeft AndAlso Not displayOnlyAcceptOption Then
                            'Message with OKCancel type (defined messageType)
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_NO_TUBES_FOUND.ToString, "", pOwnerWindow)
                        Else
                            'Message with OK type
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_NO_TUBES_FOUND.ToString, "", pOwnerWindow, Nothing, "", "Warning")
                        End If

                        'AG 22/07/2013 - Close buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StopAnalyzerRinging()
                        End If
                        'AG 22/07/2013
                    End If

                    'Add Exception to log
                    If toReturnValue = DialogResult.Yes Then
                        addUserAnswerText = "AutoCreate WS with LIS: No exception (After read barcode: Sample tubes detected)"
                    Else
                        addUserAnswerText = "AutoCreate WS with LIS exception: After read barcode NO sample tubes detected (or skiped because they have duplicated barcode). "
                    End If

                Case 3 'Warnings in LIS configuration screen 
                    'Not business here. Business in the affected screen

                Case 4 'Warnings in LIS configuration screen 
                    'Not business here. Business in the affected screen

                Case 5 'No LIS workorders received (same message as in Case 1)
                    'Check conditions for OrdersDownload (if true continue, else show warning message)
                    Dim myESRulesDlg As New ESBusiness
                    Dim LISStatusOK As Boolean = myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage)

                    If Not LISStatusOK Then
                        'AG 22/07/2013 - Activate buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StartAnalyzerRinging()
                        End If
                        'AG 22/07/2013

                        If executionsNumber > 0 AndAlso pendingExecutionsLeft AndAlso Not displayOnlyAcceptOption Then
                            'Message with OKCancel type (defined messageType)
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_LIS_NOT_READY.ToString, "", pOwnerWindow)
                        Else
                            'Message with OK type
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_LIS_NOT_READY.ToString, "", pOwnerWindow, Nothing, "", "Warning")
                        End If

                        'AG 22/07/2013 - Close buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StopAnalyzerRinging()
                        End If
                        'AG 22/07/2013
                    End If

                    'Add Exception to log
                    If toReturnValue = DialogResult.Yes Then
                        addUserAnswerText = "AutoCreate WS with LIS: No exception (LIS workorders received)"
                    Else
                        addUserAnswerText = "AutoCreate WS with LIS exception: Any LIS workorder received. "
                    End If

                Case 6 'None of the received LIS workorders were valid - pAdditionalInformation means the result of order download process
                    'Check if there are any execution PENDING/LOCKED whose orderTest has been received from LIS
                    Dim myExdlg As New ExecutionsDelegate
                    myGlobal = myExdlg.ExistsLISWorkordersPendingToExecute(Nothing)
                    Dim SomeLISWorkOrdersFlag As Boolean = False 'Default value ... error
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        SomeLISWorkOrdersFlag = DirectCast(myGlobal.SetDatos, Boolean)
                    End If

                    If Not SomeLISWorkOrdersFlag Then
                        'AG 22/07/2013 - Activate buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StartAnalyzerRinging()
                        End If
                        'AG 22/07/2013

                        If executionsNumber > 0 AndAlso pendingExecutionsLeft AndAlso Not displayOnlyAcceptOption Then
                            'Message with OKCancel type (defined messageType)
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_ALL_ORDERS_WRONG.ToString, "", pOwnerWindow)
                        Else
                            'Message with OK type
                            toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_ALL_ORDERS_WRONG.ToString, "", pOwnerWindow, Nothing, "", "Warning")
                        End If

                        'AG 22/07/2013 - Close buzzer
                        If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                            MDIAnalyzerManager.StopAnalyzerRinging()
                        End If
                        'AG 22/07/2013
                    End If

                    'Add Exception to log
                    If toReturnValue = DialogResult.Yes Then
                        addUserAnswerText = "AutoCreate WS with LIS: No exception (After orders download at least 1 valid LIS workorder)"
                    Else
                        addUserAnswerText = "AutoCreate WS with LIS exception: After orders download any of the LIS workorders are valid. "
                    End If

                Case 7 'Some of the received LIS workorders were valid but not is positioned on the samples rotor
                    'AG 22/07/2013 - Activate buzzer
                    If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                        MDIAnalyzerManager.StartAnalyzerRinging()
                    End If
                    'AG 22/07/2013

                    If executionsNumber > 0 AndAlso pendingExecutionsLeft AndAlso Not displayOnlyAcceptOption Then
                        'Message with OKCancel type (defined messageType)
                        toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_NO_TUBES_MATCHED.ToString, "", pOwnerWindow)
                    Else
                        'Message with OK type
                        toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_NO_TUBES_MATCHED.ToString, "", pOwnerWindow, Nothing, "", "Warning")
                    End If

                    'AG 22/07/2013 - Close buzzer
                    If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                        MDIAnalyzerManager.StopAnalyzerRinging()
                    End If
                    'AG 22/07/2013

                    'Add Exception to log
                    If toReturnValue = DialogResult.Yes Then
                        addUserAnswerText = "AutoCreate WS with LIS: No exception (After orders download at least 1 valid LIS workorders is positioned)"
                    Else
                        addUserAnswerText = "AutoCreate WS with LIS exception: After orders download any of the valid LIS workorder are positioned. "
                    End If

                Case 8 'This case does not show message box. It checks if exists some execution PENDING:
                    '    a) Yes: Nothing added into WS but pending executions -> open monitor and go Running
                    '    b.1) No: Nothing added into WS, no executions pending but some ordertest pending -> open rotor positioning + accept automatically
                    '    b.2) No: Nothing added into WS and no pending ordertests -> Open monitor (WS) but do not go Running
                    If executionsNumber > 0 AndAlso pendingExecutionsLeft Then
                        'Open monitor (WS) and go Running -- Function returns: DialogResult.Yes
                        toReturnValue = DialogResult.Yes
                        addUserAnswerText = "AutoCreate WS with LIS: Nothing added after orders download but there are PENDING executions, open monitor (WS) and try go Running"
                    Else
                        Dim myOTDlg As New OrderTestsDelegate
                        myGlobal = myOTDlg.ReadByStatus(Nothing, "PENDING")
                        If DirectCast(myGlobal.SetDatos, OrderTestsDS).twksOrderTests.Rows.Count > 0 AndAlso Not displayOnlyAcceptOption Then
                            'If exists some OrderTest with status PENDING -> Open rotor positions and accept it -- Function returns: Cancel
                            toReturnValue = DialogResult.Cancel
                            addUserAnswerText = "AutoCreate WS with LIS: Nothing added after orders download and there are not PENDING executions but exists PENDING ordertests, open rotor positions and accept it"
                        Else
                            'Else: No pending ordertests in current worksession -> Open monitor (WS) but do not go Running -- Function returns: DialogResult.OK
                            toReturnValue = DialogResult.OK
                            addUserAnswerText = "AutoCreate WS with LIS: Nothing added after orders download and there are no PENDING/LOCKED executions neither PENDING ordertests, open monitor (WS) but do not go Running"
                        End If
                    End If

                Case 9 'AG 24/07/2013 - The barcode has not responded - the internal watchdog has ellapsed
                    'Activate buzzer
                    If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                        MDIAnalyzerManager.StartAnalyzerRinging()
                    End If
                    toReturnValue = ShowMessage(Me.Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.AUTOLIS_BARCODE_ERROR.ToString, "", pOwnerWindow)
                    addUserAnswerText = "Barcode does not responded and the internal Watchdog has ellapsed the programmed time"

                    'Close buzzer
                    If Not MDIAnalyzerManager Is Nothing AndAlso Not pSkipSound Then
                        MDIAnalyzerManager.StopAnalyzerRinging()
                    End If

                Case Else

            End Select

            'Add user answer to Log (if any) and save log into database
            '(exclude the exception number 8 because not requires any user action)
            If toReturnValue <> DialogResult.Yes AndAlso pExceptionNumber <> 8 AndAlso pExceptionNumber <> 9 Then
                If toReturnValue = DialogResult.OK Then
                    SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcesAndStop)
                    InitializeAutoWSFlags()
                    addUserAnswerText &= " User answers: STOP the process"
                Else
                    addUserAnswerText &= " User answers: Continue and go to Running"
                End If
            End If
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(addUserAnswerText, _
                       "IAx00MainMDI.CheckForExceptionsInAutoCreateWSWithLISProcess", EventLogEntryType.Information, False)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckForExceptionsInAutoCreateWSWithLISProcess", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return toReturnValue
    End Function

#End Region

#Region "Common Forms Returned Values"

    Private Sub LanguageConfig_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles myLanguageConfig.FormClosed
        Try
            EnableButtonAndMenus(False) 'RH 29/05/2012 Bugtracking 607
            Application.DoEvents()

            If myLanguageConfig.CurrentLanguage IsNot Nothing Then
                CurrentLanguage = myLanguageConfig.CurrentLanguage
                MultilanguageResourcesDelegate.SetCurrentLanguage(CurrentLanguageAttribute)

                ShowStatus(SaveStatusMessageID) 'RH 21/03/2012

                'Update the alarms globes to the new language
                InitializeAlertGlobesTexts()

            End If

            'RH 30/06/2011
            If ClosedByFormCloseButton Then 'It is been closed by Form Close button
                OpenMonitorForm()
                IMonitor.RefreshAlarmsGlobes(Nothing)
            Else
                ClosedByFormCloseButton = True
            End If

            Dim bf As BSBaseForm = CType(sender, BSBaseForm)
            ReleaseUnManageControls(bf.Controls)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LanguageConfig_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            EnableButtonAndMenus(True) 'RH 29/05/2012 Bugtracking 607

        End Try
    End Sub

    ''' <summary>
    ''' Opens the WS Monitor form after the common form is closed.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 17/12/2010
    ''' Modified by: RH 15/12/2011 Add myReport
    ''' Modified by: XBC 18/01/2012 Add mySettings
    ''' Modified by: XBC 08/02/2012 Add myISEUtilities
    ''' Modified by: XBC 14/02/2012 Add myConfigBarCode
    ''' Modified by: RH 28/03/2012 Add myAbout, myInstrumentInfo and myLogin
    ''' </remarks>
    Private Sub CommonForms_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles myConfigAnalyzers.FormClosed, myConfigUsers.FormClosed, myReport.FormClosed, _
            mySettings.FormClosed, myConfigBarCode.FormClosed, myISEUtilities.FormClosed, myAbout.FormClosed, _
            myInstrumentInfo.FormClosed, myLogin.FormClosed
        Try
            EnableButtonAndMenus(False) 'RH 29/05/2012 Bugtracking 607
            InitializeAutoWSFlags() 'AG 25/07/2013
            Application.DoEvents()

            HideBIOSYSTEMSOnlyMenuOptions() 'RH 16/04/2012
            MenuOptionsByUserLevel() 'TR 23/4/2012

            If ClosedByFormCloseButton Then 'It is been closed by Form Close button
                If Me.IsMdiContainer Then
                    OpenMonitorForm()
                    IMonitor.RefreshAlarmsGlobes(Nothing)
                End If
            Else
                ClosedByFormCloseButton = True
            End If

            Dim bf As BSBaseForm = CType(sender, BSBaseForm)
            ReleaseUnManageControls(bf.Controls)

            If TypeOf sender Is IISEUtilities Then
                MyClass.SetActionButtonsEnableProperty(True)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CommonForms_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            EnableButtonAndMenus(True) 'RH 29/05/2012 Bugtracking 607

        End Try
    End Sub

#End Region

#Region "LIS methods"
    ''' <summary>
    ''' Based on method InitializeAnalyzerAndWorkSession
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: XB 28/02/2013 - Added calls to function ReadByParameterName to get all parameters needed for ESWapper constructor
    '''              SG 11/07/2013 - Added code to get value of setting AUTO_WS_WITH_LIS_MODE to set value of attribute autoWSCreationWithLISModeAttribute
    ''' 　　　　　　　XB 23/07/2013 - Value of attribute autoWSCreationWithLISModeAttribute will be set to TRUE also when the User has clicked in Host Query 
    '''                              button in the Main MDI Button Bar 
    '''              SA 21/10/2013 - BT #1349 ==> Added code to get value of setting LIS_ENABLE_COMM. Value of attribute autoWSCreationWithLISModeAttribute
    '''                              will depend also on value of this setting, instead of only on value of setting AUTO_WS_WITH_LIS_MODE. Removed parameter 
    '''                              pStartingApplication due to it is not used
    ''' </remarks>
    Private Sub InitiateLISWrapper()
        Try
            'This flag indicates the LIS object is not ready until the MDI load process finishes
            ProcessingLISManagerObject = True

            'AG 22/04/0210 - Moved from BSBaseForm_Load
            Dim myResultData As New GlobalDataTO
            If (AppDomain.CurrentDomain.GetData("GlobalLISManager") Is Nothing) Then
                Dim myParametersDS As New ParametersDS
                Dim myParams As New SwParametersDelegate

                'Read these parameters from database and update attributes appNameForLISAttribute and channelIdForLISAttribute
                '** Read application name for LIS parameter
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.APP_NAME_FOR_LIS.ToString, Nothing)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    myParametersDS = DirectCast(myResultData.SetDatos, ParametersDS)
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then appNameForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
                End If
                '**Read channel ID for LIS parameter
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.CHANNELID_FOR_LIS.ToString, Nothing)
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    myParametersDS = DirectCast(myResultData.SetDatos, ParametersDS)
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then channelIdForLISAttribute = myParametersDS.tfmwSwParameters.Item(0).ValueText
                End If

                'Create a new instance of class ESWrapper
                MDILISManager = New ESWrapper(appNameForLISAttribute, channelIdForLISAttribute, AnalyzerModelAttribute, Me.MDIAnalyzerManager.ActiveAnalyzer)
                Application.DoEvents()
                AppDomain.CurrentDomain.SetData("GlobalLISManager", MDILISManager)
                Application.DoEvents()

                'BT #1349 ==> Verify if LIS Communication is enabled. Get value of setting LIS_ENABLE_COMMS
                Dim myUserSettingDelegate As New UserSettingsDelegate

                Dim lisCommunicationsON As Boolean = False
                myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    lisCommunicationsON = CType(myResultData.SetDatos, Boolean)
                End If

                'Verify if automatic WS creation with LIS is enabled. Get value of setting AUTO_WS_WITH_LIS_MODE
                Dim autoLisWsON As Boolean = GlobalConstants.AUTO_WS_WITH_LIS_MODE
                myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_WS_WITH_LIS_MODE.ToString())
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    autoLisWsON = CType(myResultData.SetDatos, Boolean)
                End If

                'BT #1349 ==> Set value of attribute autoWSCreationWithLISModeAttribute depending on following conditions:
                '** LIS Communications are ON AND ALSO (Automatic WS creation with LIS is ON OR ELSE the User has sent the HOST QUERY manually)
                autoWSCreationWithLISModeAttribute = lisCommunicationsON AndAlso (autoLisWsON OrElse HQProcessByUserFlag)

                'If the Analyzer is connected, inform the property also in the AnalyzerManager Class
                If (Not MDIAnalyzerManager Is Nothing) Then MDIAnalyzerManager.autoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute

                'Get the maximum time the application will be receiving Orders sent by LIS as answer to an automatic Host Query
                myResultData = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.AUTO_LIS_WAIT_TIME.ToString())
                If (Not myResultData.HasError AndAlso Not myResultData.SetDatos Is Nothing) Then
                    LISWaitTimer.Interval = 1000 * CType(myResultData.SetDatos, Integer)
                Else
                    LISWaitTimer.Interval = CDbl(GlobalConstants.AUTO_LIS_WAIT_TIME * 1000)
                End If
                AddHandler LISWaitTimer.Elapsed, AddressOf LISwaitTimer_Timer
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".InitiateLISWrapper ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitiateLISWrapper ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Public Sub InvokeCreateLISChannel()
        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim createChannelLISManagerThread As Thread
        createChannelLISManagerThread = New Thread(Sub() Me.SynchronousLISManagerCreateChannel())
        createChannelLISManagerThread.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousLISManagerCreateChannel()

    End Sub

    ''' <summary>
    ''' Implements the create channel + connect with ES library based on the SysteLab demo code
    ''' This is a synchronous process called from an auxiliary thread
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 25/02/2013
    ''' Modified by: XB 15/03/2013 - Change to public to be available from others forms
    '''              AG 27/03/2013 - If setting LIS comm disable do not connect!!
    ''' </remarks>
    Private Sub SynchronousLISManagerCreateChannel()
        Try
            ProcessingLISManagerObject = True ' Indicates the LIS object is not ready until the create channel + connect LIS object process finishes
            If Not MDILISManager Is Nothing Then
                Dim resultData As New GlobalDataTO

                Debug.Print("LIS CREATE CHANNEL ......................")

                resultData = MDILISManager.CreateChannel(Nothing)
                If Not resultData.HasError Then
                    'AG 27/03/2013
                    'resultData = MDILISManager.Connect()
                    'Dim liscommsEnable As Boolean = True
                    'Dim userSettings As New UserSettingsDelegate
                    'resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                    'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    '    liscommsEnable = CType(resultData.SetDatos, Boolean)
                    'End If
                    'If liscommsEnable Then
                    Dim myESBusin As New ESBusiness
                    Dim tryConnectFlag As Boolean = myESBusin.AllowLISAction(Nothing, LISActions.ConnectWitlhLIS, False, False, "", "")
                    If tryConnectFlag Then
                        resultData = MDILISManager.Connect()
                    End If
                    'AG 27/03/2013
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousLISManagerCreateChannel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousLISManagerCreateChannel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousLISManagerCreateChannel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013 
        End Try
        ProcessingLISManagerObject = False 'LIS channel created and connection tried
    End Sub

    Public Sub InvokeReleaseLIS(Optional ByVal pDisposeLISManager As Boolean = True)
        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim releaseLISManagerThread As Thread
        releaseLISManagerThread = New Thread(Sub() Me.SynchronousLISManagerRelease(pDisposeLISManager))
        releaseLISManagerThread.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousLISManagerRelease(pDisposeLISManager)

    End Sub

    ''' <summary>
    ''' Implements the release LIS object based on the SysteLab demo code
    ''' This is a synchronous process called from an auxiliary thread
    ''' </summary>
    ''' <remarks>
    ''' Created by  AG 25/02/2013
    ''' Modified by XB 15/03/2013 - Change to public to be available from others forms and agregate parameter pDisposeLISManager
    '''             XB 25/03/2013 - Add getting lisTimeOutSetting from Parameters table
    '''             XB 24/04/2013 - ProcessingLISManagerObject flag is no need on release channels
    '''             XB 15/05/2013 - call to Disconnect method is deleted. It was the cause of asynchronous malfunctions when release channels
    ''' </remarks>
    Private Sub SynchronousLISManagerRelease(Optional ByVal pDisposeLISManager As Boolean = True)
        Try
            'ProcessingLISManagerObject = True ' Indicates the LIS object is not ready until the release LIS object process finishes    ' XB 24/04/2013 - this flag is no need on release channels

            If Not MDILISManager Is Nothing Then
                Dim resultData As New GlobalDataTO
                'resultData = MDILISManager.Disconnect()    ' XB 15/05/2013
                Dim lisTimeOutSetting As Integer = 100

                'Read the LIS timeout from DB parameters table 
                Dim myParams As New SwParametersDelegate
                Dim myParametersDS As New ParametersDS
                resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_RELEASECHANNEL_TIMEOUT.ToString, Nothing)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myParametersDS = CType(resultData.SetDatos, ParametersDS)
                    If myParametersDS.tfmwSwParameters.Count > 0 Then
                        lisTimeOutSetting = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                    End If
                End If

                Debug.Print("LIS RELEASE ALL CHANNELS ......................")

                resultData = MDILISManager.ReleaseAllChannels(lisTimeOutSetting)
                If pDisposeLISManager Then
                    MDILISManager.Dispose()
                    MDILISManager = Nothing
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousLISManagerRelease ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousLISManagerRelease ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousLISManagerRelease ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        End Try

        'ProcessingLISManagerObject = False 'LIS object removed ' XB 24/04/2013
    End Sub

    Public Sub InvokeUploadResultsLIS(ByVal pHistoricalFlag As Boolean, Optional ByVal pCurrentResults As ResultsDS = Nothing, Optional ByVal pCurrentResultAlarms As ResultsDS = Nothing, Optional ByVal pHistoricalResults As HisWSResultsDS = Nothing)
        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim updateResLISManagerThread As Thread
        updateResLISManagerThread = New Thread(Sub() SynchronousLISManagerUploadResults(pHistoricalFlag, pCurrentResults, pCurrentResultAlarms, pHistoricalResults))
        updateResLISManagerThread.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousLISManagerUploadResults(pHistoricalFlag, pCurrentResults, pCurrentResultAlarms, pHistoricalResults)
    End Sub

    ''' <summary>
    ''' Manage the synchronous upload results to LIS
    ''' </summary>
    ''' <param name="pHistoricalFlag"></param>
    ''' <param name="pCurrentResults">Required when HistoricalFlag = False</param>
    ''' <param name="pCurrentResultAlarms">Required when HistoricalFlag = False</param>
    ''' <param name="pHistoricalResults">Required and Informed when HistoricalFlag = True</param>
    ''' <remarks>
    ''' Created by:  AG 19/03/2013
    ''' Modified by: AG 27/03/2013 - Do not upload results if LIS communications are disabled</remarks>
    Private Sub SynchronousLISManagerUploadResults(ByVal pHistoricalFlag As Boolean, Optional ByVal pCurrentResults As ResultsDS = Nothing, Optional ByVal pCurrentResultAlarms As ResultsDS = Nothing, Optional ByVal pHistoricalResults As HisWSResultsDS = Nothing)
        Try
            If Not ProcessingLISManagerObject Then
                Dim resultData As New GlobalDataTO

                If Not MDILISManager Is Nothing AndAlso Not ProcessingLISManagerUploadResults Then
                    ProcessingLISManagerUploadResults = True ' Indicates the LIS object is not ready while the results are uploading

                    'Dim liscommsEnable As Boolean = True
                    'Dim userSettings As New UserSettingsDelegate
                    'resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                    'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    '    liscommsEnable = CType(resultData.SetDatos, Boolean)
                    'End If
                    'If liscommsEnable Then
                    Dim myESRulesDlg As New ESBusiness
                    Dim runningFlag As Boolean = CBool(IIf(MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
                    Dim connectingFlag As Boolean = CBool(IIf(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))

                    'Set the enable value.
                    Dim uploadResultsEnabled As Boolean = myESRulesDlg.AllowLISAction(Nothing, LISActions.UploadResults, runningFlag, connectingFlag, MDILISManager.Status, MDILISManager.Storage)

                    If uploadResultsEnabled Then
                        'Move row from pendingUpload to InCourseUpload
                        SyncLock lockThis
                            For Each row As ExecutionsDS.twksWSExecutionsRow In pendingUploadToLisDS.twksWSExecutions
                                inCourseUploadToLisDS.twksWSExecutions.ImportRow(row)
                            Next
                            inCourseUploadToLisDS.twksWSExecutions.AcceptChanges()
                            pendingUploadToLisDS.Clear()
                        End SyncLock

                        'Get the test LIS mapping values
                        Dim testmappDelg As New AllTestByTypeDelegate
                        Dim testMappingDS As New AllTestsByTypeDS
                        If Not resultData.HasError Then
                            resultData = testmappDelg.ReadAll(Nothing)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                testMappingDS = CType(resultData.SetDatos, AllTestsByTypeDS)
                            End If
                        End If

                        'Get the confg LIS mapping values
                        Dim confmappdlg As New LISMappingsDelegate
                        Dim confMappingDS As New LISMappingsDS
                        If Not resultData.HasError Then
                            Dim MyGlobalBase As New GlobalBase
                            resultData = confmappdlg.ReadAll(Nothing, MyGlobalBase.GetSessionInfo().ApplicationLanguage)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                confMappingDS = CType(resultData.SetDatos, LISMappingsDS)
                            End If
                        End If

                        'Get current WS results and result alarms
                        If Not resultData.HasError AndAlso Not pHistoricalFlag AndAlso (pCurrentResults Is Nothing OrElse pCurrentResultAlarms Is Nothing) Then
                            'Get current WS results
                            Dim myResults As New ResultsDelegate
                            'AG 02/01/2014 - BT #1444 (automatic upload for manual orders of std tests do not upload specimenID)
                            'resultData = myResults.GetCompleteResults(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
                            resultData = myResults.GetCompleteResults(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, True)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                pCurrentResults = New ResultsDS
                                pCurrentResults = CType(resultData.SetDatos, ResultsDS)
                                resultData = myResults.GetResultAlarms(Nothing)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    pCurrentResultAlarms = New ResultsDS
                                    pCurrentResultAlarms = CType(resultData.SetDatos, ResultsDS)
                                End If
                            End If
                        End If

                        SyncLock lockThis
                            'Divide the inCourseUploadToLisDS into several xml messages following the ES rules
                            Dim myBusiness As New ESBusiness
                            resultData = myBusiness.DivideResultsToUploadIntoSeveralMessages(Nothing, inCourseUploadToLisDS)
                            inCourseUploadToLisDS.Clear()
                        End SyncLock

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            For Each item As ExecutionsDS In CType(resultData.SetDatos, List(Of ExecutionsDS))
                                resultData = MDILISManager.UploadOrdersResults(Nothing, item, pHistoricalFlag, testMappingDS, _
                                                        confMappingDS, pCurrentResults, pCurrentResultAlarms, pHistoricalResults)
                            Next
                        End If

                    Else 'Clear class DataSets
                        SyncLock lockThis
                            pendingUploadToLisDS.Clear()
                            inCourseUploadToLisDS.Clear()
                        End SyncLock
                    End If

                End If 'If Not MDILISManager Is Nothing Then
            End If 'If Not ProcessingLISManagerObject Then

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousLISManagerUploadResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousLISManagerUploadResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousLISManagerUploadResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        End Try
        ProcessingLISManagerUploadResults = False 'LIS object available
    End Sub

    ''' <summary>
    ''' Add results to upload into a queue of results to be updated
    ''' </summary>
    ''' <param name="pExecutionsDS"></param>
    ''' <remarks>
    ''' Created by: AG 19/03/2013
    ''' </remarks>
    Public Sub AddResultsIntoQueueToUpload(ByVal pExecutionsDS As ExecutionsDS)
        Try
            'Move row from parameter to pendingUpload
            SyncLock lockThis
                For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions
                    pendingUploadToLisDS.twksWSExecutions.ImportRow(row)
                Next
                pendingUploadToLisDS.twksWSExecutions.AcceptChanges()
            End SyncLock

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AddResultsIntoQueueToUpload ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddResultsIntoQueueToUpload ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Public Sub InvokeRejectAwosDelayedLIS(ByVal pAwosToReject As OrderTestsLISInfoDS)
        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim rejectAowosDelayedLISManagerThread As Thread
        rejectAowosDelayedLISManagerThread = New Thread(Sub() SynchronousLISManagerRejectAwosDelayed(pAwosToReject))
        rejectAowosDelayedLISManagerThread.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousLISManagerRejectAwosDelayed(pAwosToReject)
    End Sub

    ''' <summary></summary>
    ''' <param name="pAwosToReject"></param>
    ''' <remarks>
    ''' Created by:  AG 22/03/2013
    ''' Modified by: AG 27/03/2013 - Do not reject results if LIS communications are disabled</remarks>
    Private Sub SynchronousLISManagerRejectAwosDelayed(ByVal pAwosToReject As OrderTestsLISInfoDS)
        Try
            If Not ProcessingLISManagerObject Then
                Dim resultData As New GlobalDataTO

                If Not MDILISManager Is Nothing Then

                    Dim liscommsEnable As Boolean = True
                    Dim userSettings As New UserSettingsDelegate
                    resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        liscommsEnable = CType(resultData.SetDatos, Boolean)
                    End If

                    If liscommsEnable Then
                        'Divide the inCourseUploadToLisDS into several xml messages following the ES rules
                        Dim myBusiness As New ESBusiness
                        resultData = myBusiness.DivideAwosToRejectIntoSeveralMessages(Nothing, pAwosToReject)

                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            For Each item As OrderTestsLISInfoDS In CType(resultData.SetDatos, List(Of OrderTestsLISInfoDS))
                                resultData = MDILISManager.RejectAwosDelayed(Nothing, item)
                            Next
                        End If
                    End If

                End If 'If Not MDILISManager Is Nothing Then
            End If 'If Not ProcessingLISManagerObject Then
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousLISManagerRejectAwosDelayed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousLISManagerRejectAwosDelayed ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousLISManagerRejectAwosDelayed ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        End Try
    End Sub

    Public Sub InvokeProcessOrdersFromLIS()
        processingLISOrderDownload = True

        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim processingOrdersFromLIS As Thread
        processingOrdersFromLIS = New Thread(Sub() SynchronousProcessOrdersFromLIS())
        processingOrdersFromLIS.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousProcessOrdersFromLIS()

    End Sub

    ''' <summary>
    ''' Process all XML Messages received from LIS and:
    ''' - Add to the active WS all AWOS ID requested for adding 
    ''' - Remove from the active WS all AWOS ID requested for cancelling 
    ''' - Reject all AWOS ID that cannot be accepted due to lack of required data in the message, Mapping Errors, 
    '''   unknown Test/SampleType, duplicated Request, not allowed Reruns, ...
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: AG 22/03/2013 - Be carefull the parameter is passed by REF and this method has to fill it
    '''              AG 27/03/2013 - Do not process orders received from LIS when LIS comm disabled
    '''              AG 02/04/2013 - Remove ByRef parameter and use a variable private in MDI
    '''              SA 04/04/2013 - Removed DB Connection parameter when calling function ProcessXmlMessages
    '''              XB 23/04/2013 - Add the First step to load LIS Saved WS from a previous reset
    '''              SA 02/05/2013 - After download and process all pending LIS Orders, initialize the counter of pending LIS Orders to zero
    '''              AG 08/05/2013 - Add new parameter WorkSessionID to method ProcessXmlMessages
    '''              SA 05/09/2013 - When the auto creation function is in process, inform it when calling function LoadLISSavedWS
    '''              SA 02/01/2014 - BT #1433 ==> After calling function LoadLISSavedWS to process LIS Orders pending to process from a previous WorkSession, 
    '''                                           read from DB the new WorkSession Status, to avoid problems with the rest of the process (when the current Status is EMPTY, 
    '''                                           there are LIS Orders pending to process from a previous WorkSession, but there is not tube for any of them, the Status is 
    '''                                           updated to OPEN, but due to the MDI Attribute was not updated, once the processing of the new Orders sent by LIS finished, 
    '''                                           the MDI Attribute for the WS Status remains as EMPTY and the application works wrong) 
    ''' </remarks>
    Private Sub SynchronousProcessOrdersFromLIS()
        Dim resultData As New GlobalDataTO
        Try
            If (Not MDILISManager Is Nothing) Then
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim TotalStartTime As DateTime = Now
                Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'TR 05/04/2013 - Variable used to indicate which screen has to be opened 
                '0 = Monitor Screen, 1 = WS Sample Requests, 2 = WS Rotor Positions
                Dim myExistPositionedElement As Integer = 0

                'Verify if communication with the external LIS system is enabled
                Dim liscommsEnable As Boolean = True
                Dim userSettings As New UserSettingsDelegate

                resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    liscommsEnable = CType(resultData.SetDatos, Boolean)
                End If

                If (liscommsEnable) Then
                    'SA 05/09/2013 - To indicate LoadLISSavedWS the auto creation function is in process
                    Dim autoWSProcessInProcess As Boolean = (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso _
                                                            (automateProcessCurrentState <> LISautomateProcessSteps.notStarted)

                    'XB 23/04/2013 - First step: load LIS Saved WS from a previous reset
                    Dim auxDS As New OrderTestsLISInfoDS
                    Dim otDelegate As New OrderTestsDelegate
                    resultData = otDelegate.LoadLISSavedWS(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute, _
                                                           MDIAnalyzerManager.ISE_Manager.IsISEModuleReady, auxDS, autoWSProcessInProcess)

                    'AG 02/01/2014 - BT #1433 add more traces to orderdownload LIS workorders (v211 patch2)
                    Dim logTrace As String = String.Empty
                    logTrace = "After LoadLISSavedWS from a previous reset. Number of rejected order tests: " & auxDS.twksOrderTestsLISInfo.Rows.Count
                    CreateLogActivity(logTrace, "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                    'AG 02/01/2014 - BT #1433

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        'Get the returned value to indicate if at least one of the Patient or Control Samples saved from a previous Reset 
                        'is already positioned in Samples Rotor
                        myExistPositionedElement = CInt(resultData.SetDatos)

                        'SA 02/01/2014 - Read the new WorkSession Status to update value of the MDI Attribute and avoid errors in the rest of the process
                        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
                        resultData = myWSAnalyzersDelegate.ReadWSAnalyzers(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            If (DirectCast(resultData.SetDatos, WSAnalyzersDS).twksWSAnalyzers.Rows.Count > 0) Then
                                WSStatusAttribute = DirectCast(resultData.SetDatos, WSAnalyzersDS).twksWSAnalyzers.First.WSStatus
                            End If
                        End If

                        If (PendingOrders > 0) Then
                            'Second step: Get and process all XML Messages saved in table twksXMLMessages with Status PENDING
                            Dim xmlDelegate As New xmlMessagesDelegate

                            resultData = xmlDelegate.ProcessXmlMessages(channelIdForLISAttribute, AnalyzerModelAttribute, AnalyzerIDAttribute, WorkSessionIDAttribute)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                'Get the rejected AWOS of the last process
                                Dim rejectedAwosDS As OrderTestsLISInfoDS = DirectCast(resultData.SetDatos, OrderTestsLISInfoDS)

                                'AG 02/01/2014 - BT #1433 add more traces to orderdownload LIS workorders (v211 patch2)
                                logTrace = "PendingOrders > 0 and after ProcessXmlMessages. Number of rejected order tests: " & rejectedAwosDS.twksOrderTestsLISInfo.Rows.Count
                                CreateLogActivity(logTrace, "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                                'AG 02/01/2014 - BT #1433

                                'Third step: Load processed new orders received from LIS
                                resultData = otDelegate.LoadLISSavedWS(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute, _
                                                                       MDIAnalyzerManager.ISE_Manager.IsISEModuleReady, auxDS, autoWSProcessInProcess)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    'Get the returned value to indicate if at least one of the accepted Patient and/or Control Samples has 
                                    'a tube placed in Samples Rotor
                                    If (CInt(resultData.SetDatos) > myExistPositionedElement) Then myExistPositionedElement = CInt(resultData.SetDatos)

                                    'Get the rejected AWOS from the last process and add them to the previous rejected AWOS
                                    For Each row As OrderTestsLISInfoDS.twksOrderTestsLISInfoRow In auxDS.twksOrderTestsLISInfo
                                        rejectedAwosDS.twksOrderTestsLISInfo.ImportRow(row)
                                    Next
                                    rejectedAwosDS.twksOrderTestsLISInfo.AcceptChanges()

                                    'AG 02/01/2014 - BT #1433 add more traces to orderdownload LIS workorders (v211 patch2)
                                Else
                                    logTrace = "PendingOrders > 0 and after LoadLISSavedWS returns error or SetDatos = Nothing"
                                    CreateLogActivity(logTrace, "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                                    'AG 02/01/2014 - BT #1433
                                End If

                                'Finally inform LIS about the rejected awos
                                If (rejectedAwosDS.twksOrderTestsLISInfo.Rows.Count > 0) Then
                                    Dim myBusiness As New ESBusiness
                                    resultData = myBusiness.DivideAwosToRejectIntoSeveralMessages(Nothing, rejectedAwosDS)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        For Each item As OrderTestsLISInfoDS In CType(resultData.SetDatos, List(Of OrderTestsLISInfoDS))
                                            resultData = MDILISManager.RejectAwosDelayed(Nothing, item)
                                        Next
                                    End If
                                End If

                                'AG 02/01/2014 - BT #1433 add more traces to orderdownload LIS workorders (v211 patch2)
                            Else
                                logTrace = "PendingOrders > 0 and after ProcessXmlMessages returns error or SetDatos = Nothing"
                                CreateLogActivity(logTrace, "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                                'AG 02/01/2014 - BT #1433
                            End If
                        End If

                        'SA 02/05/2013 - Initialize the counter for LIS Orders pending to process...
                        PendingOrders = 0

                        'AG 02/01/2014 - BT #1433 add more traces to orderdownload LIS workorders (v211 patch2)
                    Else
                        logTrace = "After LoadLISSavedWS from a previous reset. Returns error or SetDatos = Nothing"
                        CreateLogActivity(logTrace, "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                        'AG 02/01/2014 - BT #1433

                    End If
                End If

                'Return the flag indicating if at least one of the accepted Order Test has a tube placed in Samples Rotor
                resultData.SetDatos = myExistPositionedElement

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                myLogAcciones.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "IAx00MainMDI.SynchronousProcessOrdersFromLIS", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousProcessOrdersFromLIS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'ShowMessage(Name & ".SynchronousProcessOrdersFromLIS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousProcessOrdersFromLIS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
        End Try

        SyncLock lockThis
            resultsOrderDownloadProcess = resultData 'Inform the process results
        End SyncLock
        processingLISOrderDownload = False
    End Sub

    Public Sub InvokeLISHostQuery(ByVal pSpecimenList As List(Of String))
        'Leave active only one of these options:
        'a) Asynchronous call to a synchronous process
        Dim processingHostQuery As Thread
        processingHostQuery = New Thread(Sub() SynchronousLISManagerHostQuery(pSpecimenList))
        processingHostQuery.Start()

        'b) Synchronous call to a synchronous process
        'SynchronousLISManagerHostQuery (pSpecimenList )

    End Sub

    ''' <summary></summary>
    ''' <param name="pSpecimenList"></param>
    ''' <remarks>
    ''' Created by:  AG 25/03/2013
    ''' Modified by: AG 27/03/2013 - Do not send any host query request if LIS comm disabled
    ''' </remarks>
    Private Sub SynchronousLISManagerHostQuery(ByVal pSpecimenList As List(Of String))
        Try
            If Not ProcessingLISManagerObject Then
                Dim resultData As New GlobalDataTO

                If Not MDILISManager Is Nothing Then
                    Dim liscommsEnable As Boolean = True
                    Dim userSettings As New UserSettingsDelegate
                    resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        liscommsEnable = CType(resultData.SetDatos, Boolean)
                    End If

                    If liscommsEnable Then
                        'Basic, no division
                        'resultData = MDILISManager.HostQuery(Nothing, pSpecimenList)

                        'With division
                        Dim myBusiness As New ESBusiness
                        resultData = myBusiness.DivideIntoSeveralHostQueryMessages(Nothing, pSpecimenList)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim hostQueryMessages As New List(Of List(Of String))
                            hostQueryMessages = DirectCast(resultData.SetDatos, List(Of List(Of String)))
                            For Each item As List(Of String) In hostQueryMessages
                                resultData = MDILISManager.HostQuery(Nothing, item)
                            Next
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousLISManagerHostQuery ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousLISManagerHostQuery ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousLISManagerHostQuery ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        End Try
    End Sub

    ''' <summary>
    ''' Call the method SynchronousDeleteAllMessage.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 22/04/2013
    ''' </remarks>
    Public Sub InvokeDeleteAllMessage()
        Dim processingDeleteAllMessage As Thread
        processingDeleteAllMessage = New Thread(Sub() SynchronousDeleteAllMessage())
        processingDeleteAllMessage.Start()
    End Sub

    ''' <summary>
    ''' Delete all XML Messages pending to be sent to LIS (Clean storage).
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 22/04/2013
    ''' </remarks>
    Private Sub SynchronousDeleteAllMessage()
        Try
            If (Not MDILISManager Is Nothing) Then
                MDILISManager.DeleteAllMessages(Nothing)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SynchronousDeleteAllMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".SynchronousDeleteAllMessage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            Me.UIThread(Function() ShowMessage(Name & ".SynchronousDeleteAllMessage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message))
            'DL 15/05/2013
        End Try
    End Sub

    ''' <summary>
    ''' LIS Timer elapsed
    ''' </summary>
    ''' <remarks>
    ''' AG 08/07/2013
    ''' </remarks>
    Private Sub LISwaitTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)

        'Disable timer
        LISWaitTimer.Enabled = False
        MDILISManager.ClearQueueOfSpecimenNotResponded() 'Clear queue

        'AG 23/07/2013 - Move the commented code to a new method. Executed in the presentation thread
        ''Close screen (use UIThread method to avoid cross-threading)
        ''Now evaluate if the a part from the ActiveMDIChild there is a popup screen active
        'Dim found As Boolean = False
        'Dim index As Integer
        'For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
        '    If NoMDIChildActiveFormsAttribute.Item(index).Name = HQBarcode.Name Then
        '        found = True
        '        Exit For
        '    End If
        'Next index
        'If found Then
        '    Dim CurrentNoMdiChild As HQBarcode
        '    CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), HQBarcode)
        '    Me.UIThread(Sub() CurrentNoMdiChild.CloseScreen())
        'End If
        UIThread(Sub() ProcessAutomaticOrdersDownload())
        'AG 23/07/2013
    End Sub


    ''' <summary>
    ''' Business moved from the LISwaitTimer_Timer event. Used for process orders download in automatic WS creation with LIS
    ''' </summary>
    ''' <remarks>AG 23/07/2013</remarks>
    Private Sub ProcessAutomaticOrdersDownload()
        Try
            'Disable timer
            LISWaitTimer.Enabled = False

            'Close screen (use UIThread method to avoid cross-threading)
            'Now evaluate if the a part from the ActiveMDIChild there is a popup screen active
            Dim found As Boolean = False
            Dim index As Integer
            For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                If NoMDIChildActiveFormsAttribute.Item(index).Name = HQBarcode.Name Then
                    found = True
                    Exit For
                End If
            Next index
            If found Then
                Dim CurrentNoMdiChild As HQBarcode
                CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), HQBarcode)
                CurrentNoMdiChild.CloseScreen()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ProcessAutomaticOrdersDownload ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProcessAutomaticOrdersDownload ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "LIS events (communications and user events)"

    ''' <summary>
    ''' New LIS notification event has been received and captured by MDI who needs to inform the presentation layer
    ''' Executes ManageNewLISNotification() method in the main thread
    ''' 
    ''' Events available only when LIS object is available (MDI is loaded)
    ''' </summary>
    ''' <remarks>
    ''' Created by: AG 25/02/2013
    ''' </remarks>
    Private Sub OnLISNotificationEvent(ByVal pChannelName As String, ByVal pPriority As Integer, ByVal pMessage As XmlDocument) Handles MDILISManager.OnLISNotification
        If Not ProcessingLISManagerObject Then
            Me.UIThread(Function() ManageNewLISNotification(pChannelName, pPriority, pMessage))
        End If
    End Sub

    ''' <summary>
    ''' New LIS message event has been received and captured by MDI who needs to inform the presentation layer
    ''' Executes ManageNewLISMessage() method in the main thread
    ''' 
    ''' Events available only when LIS object is available (MDI is loaded)
    ''' </summary>
    ''' <remarks>
    ''' Created by: AG 25/02/2013
    ''' </remarks>
    Private Sub OnLISMessageEvent(ByVal pChannelName As String, ByVal pPriority As Integer, ByVal pMessage As XmlDocument) Handles MDILISManager.OnLISMessage
        If Not ProcessingLISManagerObject Then
            Me.UIThread(Function() ManageNewLISMessage(pChannelName, pPriority, pMessage))
        End If
    End Sub

    ''' <summary>
    ''' Treats the new LIS notification: decode and perform the required business and screen refresh
    ''' </summary>
    ''' <param name="pChannelName"></param>
    ''' <param name="pPriority"></param>
    ''' <param name="pMessage"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Creation - AG 25/03/2013 - Based on communications events reception method ManageReceptionEvent
    ''' Modified AG 21/03/2013 - decode and apply business rules from ESBusiness class (MDI only refresh information)
    '''          SG 10/04/2013 - add refresh flag for refreshing data in current mdi child active screen
    '''          XB 24/04/2013 - Invoke Create Channel again is processed here when 'released' notification is received caused by settings changes from LIS Config screen
    '''          AG 07/05/2013 - Change ToUpper for ToUpperBS (ToUpperInvariant)
    '''          SG 15/05/2013 - for refreshing LIS buttons in case of Status or Storage notifications
    ''' </remarks>
    Private Function ManageNewLISNotification(ByVal pChannelName As String, ByVal pPriority As Integer, ByVal pMessage As XmlDocument) As Boolean
        Try
            'Pass xml information from LIS thread to main thread
            Dim myXMLmessage As New XmlDocument
            SyncLock lockThis
                myXMLmessage = pMessage 'From now use the variable myXMLMessage instead of use pMessage
            End SyncLock

            Dim refreshFlag As Boolean = False 'SGM 10/04/2012 - for refreshing data in current mdi child active screen
            Dim refreshHQScreenFlag As Boolean = False 'SGM 10/04/2012 - for refreshing data in current mdi child active screen
            Dim refreshLISButtonsFlag As Boolean = False 'SGM 15/05/2013 - for refreshing LIS buttons in case of Status or Storage notifications

            'Get the xml translated
            Dim resultData As New GlobalDataTO
            Dim myBusiness As New ESBusiness
            resultData = myBusiness.TreatXMLNotification(Nothing, channelIdForLISAttribute, AnalyzerModelAttribute, AnalyzerIDAttribute, pMessage)
            If Not resultData.HasError Then
                Dim notificationLis As New Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String))
                notificationLis = DirectCast(resultData.SetDatos, Dictionary(Of GlobalEnumerates.LISNotificationSensors, List(Of String)))

                '0- Read the required settings: LIS communication enable"
                Dim liscommsEnable As Boolean = True
                Dim userSettings As New UserSettingsDelegate
                resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    liscommsEnable = CType(resultData.SetDatos, Boolean)
                End If

                Dim notificationValues As New List(Of String)
                '1) CONTROL INFORMATION
                '======================
                'STATUS
                If notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.STATUS) AndAlso notificationLis(LISNotificationSensors.STATUS).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.STATUS) '1- Get Data
                    '2- STATUS information presentation
                    If liscommsEnable Then
                        MDILISManager.Status = notificationValues(0) 'Inform the status into ES wrapper object
                        SetLISLedStatusColor(notificationValues(0), "") 'Set the LIS status color

                        ' XB 24/04/2013 - Invoke Create Channel again is processed here when 'released' notification is received caused by settings changes from LIS Config screen
                        If ((notificationValues(0) = "" Or notificationValues(0).ToUpperBS() = GlobalEnumerates.LISStatus.released.ToString.ToUpperBS()) And (InvokeReleaseFromConfigSettings)) Then
                            InvokeReleaseFromConfigSettings = False
                            InvokeCreateLISChannel()
                        End If
                        ' XB 24/04/2013
                    Else
                        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : " & notificationValues(0))
                        BsLISLed.StateIndex = bsLed.LedColors.GRAY 'Gray
                    End If
                    refreshLISButtonsFlag = True 'SGM 15/05/2013


                    'STORAGE
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.STORAGE) AndAlso notificationLis(LISNotificationSensors.STORAGE).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.STORAGE) '1- Get Data
                    '2- STORAGE information presentation
                    If liscommsEnable Then
                        MDILISManager.Storage = notificationValues(0) 'Inform the storage value ES wrapper object
                        SetLISLedStatusColor("", notificationValues(0)) 'Set the LIS status color
                    Else
                        BsLISLed.StateIndex = bsLed.LedColors.GRAY 'Gray
                    End If
                    refreshLISButtonsFlag = True 'SGM 15/05/2013

                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : STORAGE")
                    ' TO TEST

                    'DELIVERED
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.DELIVERED) AndAlso notificationLis(LISNotificationSensors.DELIVERED).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.DELIVERED) '1- Get Data
                    '2- Treat the DELIVERED presentation
                    'Refresh the exportStatus from SENDING -> SENT (if current screen are current WS results, historical results or monitor)
                    refreshFlag = True 'SGM 10/04/2013

                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : DELIVERED")
                    ' TO TEST

                    'UNDELIVERED
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.UNDELIVERED) AndAlso notificationLis(LISNotificationSensors.UNDELIVERED).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.UNDELIVERED) '1- Get Data
                    '2- Treat the UNDELIVERED presentation
                    'Refresh the exportStatus from SENDING -> NOTSENT (if current screen are current WS results, historical results or monitor)
                    refreshFlag = True 'SGM 10/04/2013
                    refreshHQScreenFlag = True ' JCM 02/05/2013

                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : UNDELIVERED")
                    ' TO TEST

                    'UNRESPONDED
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.UNRESPONDED) AndAlso notificationLis(LISNotificationSensors.UNRESPONDED).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.UNRESPONDED) '1- Get Data
                    '2- Treat the UNRESPONDED presentation
                    'Refresh the exportStatus from SENDING -> NOTSENT (if current screen are current WS results, historical results or monitor)
                    refreshFlag = True 'SGM 10/04/2013
                    refreshHQScreenFlag = True ' JCM 02/05/2013

                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : UNRESPONDED")
                    ' TO TEST

                    'PENDINGMESSAGES
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.PENDINGMESSAGES) AndAlso notificationLis(LISNotificationSensors.PENDINGMESSAGES).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.PENDINGMESSAGES) '1- Get Data
                    '2- Treat the PENDINGMESSAGES presentation
                    '??? It will depend on the LIS utilities screen - Not defined yet
                    'TODO

                    'DELETED
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.DELETED) AndAlso notificationLis(LISNotificationSensors.DELETED).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.DELETED) '1- Get Data
                    '2- Treat the DELETED presentation
                    '??? It will depend on the LIS utilities screen - Not defined yet
                    'TODO


                    '2) QUERY RESPONSE
                    '=================
                    'INVALID
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.INVALID) AndAlso notificationLis(LISNotificationSensors.INVALID).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.INVALID) '1- Get Data
                    '2- Treat the INVALID presentation
                    'EnableButtonAndMenus(True) 'Activate UI 'AG 21/03/2013 - Not required, STL informs the 19/03/2013 that queryall / hostquery are asynchronous calls, not wait is required!!

                    'Show error message
                    ShowMessage("Error", resultData.ErrorMessage)

                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : INVALID")
                    ' TO TEST

                    'QUERYALL
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.QUERYALL) AndAlso notificationLis(LISNotificationSensors.QUERYALL).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.QUERYALL) '1- Get Data
                    '2- Treat the QUERYALL presentation
                    'EnableButtonAndMenus(True) 'Activate UI 'AG 21/03/2013 - Not required, STL informs the 19/03/2013 that queryall / hostquery are asynchronous calls, not wait is required!!
                    ' TO TEST
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS notification : QUERY ALL")
                    ' TO TEST

                    'HOSTQUERY
                ElseIf notificationLis.ContainsKey(GlobalEnumerates.LISNotificationSensors.HOSTQUERY) AndAlso notificationLis(LISNotificationSensors.HOSTQUERY).Count > 0 Then
                    notificationValues = notificationLis(LISNotificationSensors.HOSTQUERY) '1- Get Data
                    '2- Treat the HOSTQUERY presentation

                    refreshFlag = True 'JCM 29/04/2013
                    refreshHQScreenFlag = True ' JCM 02/05/2013

                End If


            Else
                'Error decoding xml notification
                ShowMessage(Me.Name & ".ManageNewLISNotification", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

            'SGM 10/04/2013 - the refresh method in current mdi child active screen
            If refreshFlag Then
                If Not ActiveMdiChild Is Nothing Then
                    If (TypeOf ActiveMdiChild Is IMonitor) Then
                        IMonitor.UpdateWSState(New UIRefreshDS)
                    ElseIf (TypeOf ActiveMdiChild Is IResults) Then
                        IResults.RefreshExportStatusChanged()
                    ElseIf (TypeOf ActiveMdiChild Is IHisResults) Then
                        IHisResults.RefreshScreen(Nothing, Nothing)
                    End If

                    ' JCM 02/05/2013
                    If refreshHQScreenFlag Then
                        'AG 25/04/2013 - Now evaluate if the a part from the ActiveMDIChild there is a popup screen active
                        Dim found As Boolean = False
                        Dim index As Integer
                        For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                            If NoMDIChildActiveFormsAttribute.Item(index).Name = HQBarcode.Name Then
                                found = True
                                Exit For
                            End If
                        Next index
                        If found Then
                            Dim CurrentNoMdiChild As HQBarcode
                            CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), HQBarcode)
                            CurrentNoMdiChild.LIMSImportButtonEnabled()
                            CurrentNoMdiChild.RefreshScreen(Nothing, Nothing)

                        End If
                        'AG 25/04/2013
                    End If  ' refreshHQScreenFlag 

                End If
            End If

            'SG 15/05/2013 - for refreshing LIS buttons in case of Status or Storage notifications
            If refreshLISButtonsFlag Then

                'MDI Buttons
                MyClass.ActivateLISActionButton()

                'LIS Utilities
                If Not ActiveMdiChild Is Nothing Then
                    If (TypeOf ActiveMdiChild Is ILISUtilities) Then
                        ILISUtilities.RefreshElementsEnabled()
                    End If
                End If

                'HQBarcode
                For index As Integer = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                    If NoMDIChildActiveFormsAttribute.Item(index).Name = HQBarcode.Name Then
                        Dim CurrentNoMdiChild As HQBarcode
                        CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), HQBarcode)
                        CurrentNoMdiChild.RefreshScreen(Nothing, Nothing)
                        Exit For
                    End If
                Next index

            End If
            'end SG 15/05/2013

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "ManageNewLISNotification", EventLogEntryType.Error, False)
        End Try

        Return True
    End Function


    ''' <summary>
    ''' Evaluates the status and storages values and set the new LIS status LED color
    ''' NOTE: This method does not implements Try ... Catch. The method who calls it has to do it
    ''' </summary>
    ''' <param name="pStatus"></param>
    ''' <param name="pStorageValue"></param>
    ''' <remarks>
    ''' Creation AG 28/02/2013
    ''' modified by TR: 01/03/2013 - Talk to AG because the validation is case sensitive. decide to set all to
    '''                              upper case validation.
    '''             XB: 19/04/2013 - Storage overloaded (100) is represented with Orange led instead of Red
    '''             XB: 24/04/2013 - Add 'released' case
    ''' AG 07/05/2013 - Change ToUpper for ToUpperBS (ToUpperInvariant)
    ''' </remarks>
    Private Sub SetLISLedStatusColor(Optional ByVal pStatus As String = "", Optional ByVal pStorageValue As String = "")

        If pStatus <> "" Then
            MDILISManager.Status = pStatus.ToUpperBS() ' TR 23/04/2013 Uldate the LISManager status.
            Select Case pStatus.ToUpperBS()
                'AG 18/03/2013 - STL confirms status connectionEnabled = connectionAccepted
                'Case GlobalEnumerates.LISStatus.noconnectionDisabled.ToString.ToUpperBS(), _
                '     GlobalEnumerates.LISStatus.connectionDisabled.ToString.ToUpperBS(), _
                '     GlobalEnumerates.LISStatus.connectionEnabled.ToString.ToUpperBS()
                Case GlobalEnumerates.LISStatus.noconnectionDisabled.ToString.ToUpperBS(), _
                     GlobalEnumerates.LISStatus.connectionDisabled.ToString.ToUpperBS()

                    BsLISLed.StateIndex = bsLed.LedColors.GRAY 'Gray

                Case GlobalEnumerates.LISStatus.noconnectionEnabled.ToString.ToUpperBS(), _
                    GlobalEnumerates.LISStatus.connectionRejected.ToString.ToUpperBS()

                    BsLISLed.StateIndex = bsLed.LedColors.RED  'Red

                    'Case GlobalEnumerates.LISStatus.connectionAccepted.ToString.ToUpperBS()
                Case GlobalEnumerates.LISStatus.connectionAccepted.ToString.ToUpperBS(), _
                    GlobalEnumerates.LISStatus.connectionEnabled.ToString.ToUpperBS()

                    BsLISLed.StateIndex = bsLed.LedColors.GREEN   'Green

                Case GlobalEnumerates.LISStatus.released.ToString.ToUpperBS()
                    BsLISLed.StateIndex = bsLed.LedColors.GRAY 'Gray

                Case Else 'Protection case for unkown status
                    BsLISLed.StateIndex = bsLed.LedColors.GRAY 'Gray

            End Select

            ' TO TEST
            Select Case pStatus.ToUpperBS()
                Case GlobalEnumerates.LISStatus.noconnectionEnabled.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : noconnectionEnabled")
                Case GlobalEnumerates.LISStatus.noconnectionDisabled.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : noconnectionDisabled")
                Case GlobalEnumerates.LISStatus.connectionEnabled.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : connectionEnabled")
                Case GlobalEnumerates.LISStatus.connectionAccepted.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : connectionAccepted")
                Case GlobalEnumerates.LISStatus.connectionRejected.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : connectionRejected")
                Case GlobalEnumerates.LISStatus.connectionDisabled.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : connectionDisabled")
                Case GlobalEnumerates.LISStatus.released.ToString.ToUpperBS()
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : released")
                Case Else
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - LIS STATUS : unknow connection status")
            End Select
            ' TO TEST

        ElseIf pStorageValue <> "" Then
            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") & " - STORAGE value : " & pStorageValue)
            Select Case pStorageValue
                Case "0"
                    'Do not use GREEN ... use the last LIS object status
                    'BsLISLed.StateIndex = bsLed.LedColors.GREEN  'Green
                    SetLISLedStatusColor(MDILISManager.Status, "")

                    'Case "100"
                    '    BsLISLed.StateIndex = bsLed.LedColors.RED  'Red

                Case Else '75, 80, 85,90, 95, 100 ... Warning!!!
                    BsLISLed.StateIndex = bsLed.LedColors.ORANGE  'Orange
            End Select

        End If
    End Sub


    ''' <summary>
    ''' Treats the new LIS message: decode and perform the required business and screen refresh
    ''' </summary>
    ''' <param name="pChannelName"></param>
    ''' <param name="pPriority"></param>
    ''' <param name="pMessage"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Creation - AG 25/02/2013 - Based on communications events reception method ManageReceptionEvent
    ''' Modified - XB 01/03/2013 - Add functionality to the activation rules for the button Orders Download
    ''' AG 22/07/2013 - During process of automatic query to LIS if the queue of specimen asked but not responded becomes empty then process orders download!!!
    ''' </remarks>
    Public Function ManageNewLISMessage(ByVal pChannelName As String, ByVal pPriority As Integer, ByVal pMessage As XmlDocument) As Boolean
        Dim resultData As New GlobalDataTO
        Try
            'Pass xml information from LIS thread to main thread
            Dim myXMLmessage As New XmlDocument
            SyncLock lockThis
                myXMLmessage = pMessage 'From now use the variable myXMLMessage instead of use pMessage
            End SyncLock

            'NOTE: The xml has been already saved inside the ESWrapper

            'Read settings from databasse
            Dim AcceptDownloadInRunning As Boolean = False
            Dim liscommsEnable As Boolean = True
            Dim userSettings As New UserSettingsDelegate
            resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                liscommsEnable = CType(resultData.SetDatos, Boolean)
            End If
            resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_DOWNLOAD_ONRUNNING.ToString)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                AcceptDownloadInRunning = CType(resultData.SetDatos, Boolean)
            End If

            'Evaluate the activation rules for the button Orders Download
            Dim ActivateButton As Boolean = False

            ' XB: Evaluate if this validation is required here or is redundant ????
            ' AG: YES, it is redundant because the xml already has been saved. At least there is the just received message pending to treat
            'Dim xmlMessageDgt As New xmlMessagesDelegate
            'resultData = xmlMessageDgt.ReadByStatus(Nothing, "PENDING")
            'If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
            '    Dim myXmlMessages As New List(Of XMLMessagesTO)
            '    myXmlMessages = DirectCast(resultData.SetDatos, List(Of XMLMessagesTO))
            '    If myXmlMessages.Count > 0 Then
            '        ActivateButton = True
            '    End If
            'End If

            If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                If liscommsEnable AndAlso AcceptDownloadInRunning Then
                    ActivateButton = True
                End If
            Else
                If liscommsEnable Then
                    ActivateButton = True
                End If
            End If

            If ActivateButton Then
                Me.bsTSOrdersDownloadButton.Enabled = True
            End If

            PendingOrders += 1


            'JC 29/05/2013 Ordres Download Count, return more Orders Pending than real, because Synapse Driver Generates fake Orders.
            '              For this reason has been decided disable the tooltip with Orders Pending Count. Now Orders Pending Count is disable
            bsTSOrdersDownloadButton.ToolTipText = NoPendingOrdersToolTip
            ' Before (when was shown the Orders Pending Count)
            'DL 29/04/2013 Add SavedOrdersfromsLIMS
            'bsTSOrdersDownloadButton.ToolTipText = PendingOrdersToolTip + " " + PendingOrders.ToString

            'AG 22/07/2013 - When LIS respond all the specimens asked by Sw ... the waiting time finishes and process the messages!!
            'XB 23/07/2013 - auto HQ
            'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState = LISautomateProcessSteps.subProcessAskBySpecimen  Then
            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.subProcessAskBySpecimen Then
                'XB 23/07/2013

                'Evaluate only when there are some specimen to receive (protection because when timer ellapses the queue is deleted!!)
                If MDILISManager.specimensWithNoResponse > 0 Then
                    resultData = MDILISManager.UpdateSpecimensNotResponded(pMessage)
                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        If DirectCast(resultData.SetDatos, List(Of String)).Count = 0 Then
                            UIThread(Sub() ProcessAutomaticOrdersDownload())
                        End If
                    End If
                End If
            End If
            'AG 22/07/2013

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "ManageNewLISMessage", EventLogEntryType.Error, False)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' LIS Query all button clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' AG 28/02/2013
    ''' </remarks>
    Private Sub bsTSQueryAllButton_Click(sender As Object, e As EventArgs) Handles bsTSQueryAllButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim TotalStartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            CreateLogActivity("QAll button in MDI horizontal bar", Me.Name & ".bsTSQueryAllButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013

            'Send QueryAll and deactivate UI
            If Not MDILISManager Is Nothing Then
                Dim resultData As New GlobalDataTO
                'EnableButtonAndMenus(False) 'Disable UI - Not required, STL informs the 19/03/2013 that queryall / hostquery are asynchronous calls, not wait is required!!
                resultData = MDILISManager.QueryAll(Nothing)

                If resultData.HasError Then
                    EnableButtonAndMenus(True) 'Enable UI
                    ShowMessage("Error", resultData.ErrorMessage)
                End If


            End If
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            Me.Name & "." & (New System.Diagnostics.StackTrace()).GetFrame(0).GetMethod().Name, EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "bsTSQueryAllButton_Click", EventLogEntryType.Error, False)
        End Try
    End Sub


    ''' <summary>
    ''' LIS Host query button clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by  AG 28/02/2013
    ''' Modified by XB 22/07/2013 - Add HQProcessByUserFlag condition for LIS auto HQ management
    ''' </remarks>
    Private Sub bsTSHostQueryButton_Click(sender As Object, e As EventArgs) Handles bsTSHostQueryButton.Click
        Try
            ''AG 28/02/2013 - To comment code - PROVISIONAL for testing
            'If Not MDILISManager Is Nothing Then
            '    Dim myUtils As New Utilities
            '    Dim resultData As New GlobalDataTO
            '    resultData = myUtils.GetNewGUID
            '    Dim specimenList As New List(Of String)
            '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '        For i As Integer = 1 To 14
            '            specimenList.Add("SPM" & i)
            '        Next
            '        SynchronousLISManagerHostQuery(specimenList)
            '    End If
            'End If
            ''END OF PROVISIONAL CODE!!!!

            'AG 02/01/2014 - BT#1433 - New log traces (v211 patch2)
            CreateLogActivity("HQ button in MDI horizontal bar", "IAx00MainMDI.bsTSHostQueryButton_Click", EventLogEntryType.Information, False)

            ' XB 22/07/2013 - Auto LIS HQ
            If Not LISWithFilesMode Then
                HQProcessByUserFlag = True
                Me.autoWSCreationWithLISMode = HQProcessByUserFlag
                MyClass.InitiateStartSession()
            Else

                'Final code
                'Open the Rotor positions and the LIS monitor screen for monitoring the last host query
                If Not MDILISManager Is Nothing Then
                    Dim openScreenFlag As Boolean = True
                    If Not ActiveMdiChild Is Nothing AndAlso (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                        'Do not open rotor positions screen is already open
                        openScreenFlag = False
                    End If

                    If openScreenFlag Then
                        'If current screen not is rotor position open the rotor positioning screen + popup HQ Screen
                        OpenRotorPositionsForm(Nothing, True)
                    Else
                        'Current screen is rotor positioning -> Show popup screen
                        Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                        If CurrentMdiChild.BarcodeWarningButton.Visible AndAlso CurrentMdiChild.BarcodeWarningButton.Enabled Then
                            CurrentMdiChild.BarcodeWarningButton.PerformClick()
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "bsTSHostQueryButton_Click", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' LIS Order download button clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 22/03/2013 - prepare code
    '''          AG 08/07/2013 - changes for automatic WS creation with LIS
    '''          AG 22/07/2013 - clear the queue of specimen asked but not responded (after process orders download)</remarks>
    Private Sub bsTSOrdersDownloadButton_Click(sender As Object, e As EventArgs) Handles bsTSOrdersDownloadButton.Click

        ' XB 24/07/2013 - Moved to OrdersDownload() sub due more stability
        'Try

        '    ' If there are too much Pending Orders and Analyser is running then request user to confirm if wish to proceed
        '    ' all other cases continue
        '    If Not GlobalConstants.AnalyzerIsRunningFlag OrElse _
        '       PendingOrders < MAX_PENDING_ORDERS OrElse _
        '       ShowMessage(Name, GlobalEnumerates.Messages.PENDING_ORDER_DOWNLOAD.ToString, , , New List(Of String)(New String() {MAX_PENDING_ORDERS.ToString()})) = Windows.Forms.DialogResult.Yes Then

        '        'Close current screen
        '        Dim closedFlag As Boolean = CloseActiveMdiChild(True) 'AG 10/07/2013 - optional parameter to TRUE
        '        If closedFlag Then
        '            Dim myLogAcciones As New ApplicationLogManager()

        '            'Order download and add to worksession
        '            '1- Disable UI
        '            Cursor = Cursors.WaitCursor
        '            EnableButtonAndMenus(False) 'Disable UI
        '            SetActionButtonsEnableProperty(False) 'AG 09/07/2013


        '            '2- Launch process with a new threading (like reset ws)
        '            Dim resultData As New GlobalDataTO

        '            'SGM 11/07/2013 + AG 11/07/2013 - get data from db
        '            Dim executeDownloadOrdersProcess As Boolean = True
        '            Dim maxIterations As Integer = GlobalConstants.AUTO_LIS_MAX_ITERA
        '            Dim auxInteger As Integer = 0
        '            Dim currentIt As Integer = 0
        '            Dim myESRulesDlg As New ESBusiness

        '            Dim myParams As SwParametersDelegate = New SwParametersDelegate
        '            resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.AUTO_LIS_MAX_ITERA.ToString, Nothing)
        '            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '                Dim myParametersDS As ParametersDS
        '                myParametersDS = CType(resultData.SetDatos, ParametersDS)
        '                If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
        '                    maxIterations = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
        '                End If
        '            End If
        '            'SGM 11/07/2013 + AG 11/07/2013

        '            ScreenWorkingProcess = True

        '            'AG 11/07/2013 - add while business
        '            ''3- Looping until process finishes
        '            'Application.DoEvents()
        '            'While processingLISOrderDownload 'ScreenWorkingProcess
        '            '    Me.InitializeMarqueeProgreesBar()
        '            '    Application.DoEvents()
        '            'End While
        '            'Application.DoEvents()
        '            'StopMarqueeProgressBar()
        '            '
        '            ''4- Enable UI
        '            'EnableButtonAndMenus(True) 'Enable UI
        '            '
        '            ''Results are available in variable returnValue (globalDataTO)
        '            'Dim myResults As New GlobalDataTO
        '            'SyncLock lockThis
        '            '    myResults = resultsOrderDownloadProcess
        '            'End SyncLock

        '            Dim myResults As New GlobalDataTO
        '            Dim cumulatedResults As Integer = 0
        '            While executeDownloadOrdersProcess
        '                InvokeProcessOrdersFromLIS()

        '                '3- Looping until process finishes
        '                Application.DoEvents()
        '                While processingLISOrderDownload 'ScreenWorkingProcess
        '                    Me.InitializeMarqueeProgreesBar()
        '                    Application.DoEvents()
        '                End While
        '                Application.DoEvents()

        '                'AG 11/07/2013
        '                'XB 23/07/2013 - auto HQ
        '                'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                    'XB 23/07/2013
        '                    'If new LIS workorders have been received while we were processing process 
        '                    currentIt += 1
        '                    If currentIt <= maxIterations Then
        '                        executeDownloadOrdersProcess = myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage, auxInteger)
        '                    Else
        '                        executeDownloadOrdersProcess = False
        '                    End If
        '                Else
        '                    executeDownloadOrdersProcess = False
        '                End If

        '                'Results are available in variable returnValue (globalDataTO)
        '                SyncLock lockThis
        '                    myResults = resultsOrderDownloadProcess
        '                    If cumulatedResults <> 2 Then
        '                        If (Not myResults.HasError) AndAlso (TypeOf (myResults.SetDatos) Is Integer) Then
        '                            If cumulatedResults < DirectCast(myResults.SetDatos, Integer) Then
        '                                cumulatedResults = DirectCast(myResults.SetDatos, Integer)
        '                            End If
        '                        End If
        '                    End If
        '                End SyncLock
        '                'executeDownloadOrdersProcess = False

        '                'Before exit the loop update myResults.SetDatos with the cumulated result value
        '                If Not executeDownloadOrdersProcess Then
        '                    myResults.SetDatos = cumulatedResults
        '                End If
        '                'AG 11/07/2013
        '            End While
        '            'AG 11/07/2013

        '            'AG 22/07/2013
        '            If Not MDILISManager Is Nothing Then
        '                MDILISManager.ClearQueueOfSpecimenNotResponded()
        '            End If
        '            'AG 22/07/2013
        '            StopMarqueeProgressBar()

        '            '4- Enable UI
        '            EnableButtonAndMenus(True) 'Enable UI

        '            '5- Apply refresh (open the proper screen) and show message with results (use variable myResults)
        '            'Conditions: ((0)Monitor, (1)Sample Request, (2)Rotor position).
        '            If (Not myResults.HasError) Then
        '                Select Case CInt(myResults.SetDatos)
        '                    Case 1
        '                        'New samples are added but no set to position then open the Sample Request Screen
        '                        'Inform screen properties
        '                        IWSSampleRequest.ActiveAnalyzer = AnalyzerIDAttribute
        '                        IWSSampleRequest.ActiveWorkSession = WorkSessionIDAttribute
        '                        IWSSampleRequest.ActiveWSStatus = WSStatusAttribute
        '                        OpenMDIChildForm(IWSSampleRequest)

        '                        'XB 23/07/2013 - auto HQ
        '                        'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                            'XB 23/07/2013 
        '                            myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download End process. Final screen SampleRequest", "IAx00MainMDI.bsTSOrdersDownloadButton_Click", EventLogEntryType.Information, False)
        '                            Dim autoProcessUserAnswer As DialogResult = DialogResult.Yes
        '                            autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(7)
        '                            If autoProcessUserAnswer = DialogResult.Yes Then
        '                                'Positive case. No exceptions
        '                            ElseIf autoProcessUserAnswer = DialogResult.OK Then 'User answers stops the automatic process
        '                                'Automatic process aborted
        '                                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
        '                                InitializeAutoWSFlags()
        '                                EnableButtonAndMenus(True, True) 'Enable buttons after update attribute!!
        '                            Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
        '                                FinishAutomaticWSWithLIS()
        '                            End If
        '                        End If
        '                        'AG 12/07/2013

        '                        Exit Select
        '                    Case 2
        '                        'If at least one of the order lis is set to position then open the Rotor Position Screen
        '                        If (IWSSampleRequest.Visible) Then
        '                            'It is not possible because we have close the current MDI so there is not any screen open but the mdi (empty, only the mdi frame)
        '                            'XB 23/07/2013 - auto HQ
        '                            'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then 'Inform the automatic process send to position + create executions
        '                            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then 'Inform the automatic process send to position + create executions
        '                                'XB 23/07/2013
        '                                IWSSampleRequest.OpenByAutomaticProcess = True
        '                                myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download End process. Final screen SampleRequest + auto send to positioning", "IAx00MainMDI.bsTSOrdersDownloadButton_Click", EventLogEntryType.Information, False)
        '                            End If
        '                            IWSSampleRequest.SaveWSWithPositioning()
        '                        Else
        '                            'AG 10/07/2013 - evaluate if open rotor positions in manual or automatic mode
        '                            'XB 23/07/2013 - auto HQ
        '                            'If Not autoWSCreationWithLISModeAttribute OrElse automateProcessCurrentState = LISautomateProcessSteps.notStarted Then 'Manual
        '                            If (Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag) OrElse automateProcessCurrentState = LISautomateProcessSteps.notStarted Then 'Manual
        '                                'XB 23/07/2013
        '                                OpenRotorPositionsForm(Nothing)
        '                            Else 'Automatic
        '                                myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download open Rotor positions screen", "IAx00MainMDI.bsTSOrdersDownloadButton_Click", EventLogEntryType.Information, False)
        '                                OpenRotorPositionsForm(Nothing, False, True) 'Inform new parameters for inform close automatically screen in order to generate executions
        '                            End If
        '                            'AG 10/07/2013
        '                        End If
        '                        Exit Select
        '                    Case Else
        '                        'Open Monitor screen.
        '                        'AG 18/07/2013 - After orderdownload the open screen is monitor --> Go to running if any pending execution
        '                        '                if all executions locked go to Rotor positions and press Accept automatically
        '                        'AG 12/07/2013
        '                        'OpenMonitorForm(Nothing)
        '                        'XB 23/07/2013 - auto HQ
        '                        'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
        '                            'XB 23/07/2013
        '                            myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download nothing added to Work Session", "IAx00MainMDI.bsTSOrdersDownloadButton_Click", EventLogEntryType.Information, False)
        '                            Dim autoProcessUserAnswer As DialogResult = DialogResult.OK
        '                            autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(8)
        '                            If autoProcessUserAnswer = DialogResult.Yes Then
        '                                'Exists some execution with status PENDING -> Open monitor and Running
        '                                OpenMonitorForm(Nothing, True)

        '                            ElseIf autoProcessUserAnswer = DialogResult.OK Then
        '                                'Exists no executions PENDING/LOCKED and no PENDING orderTest -> Open monitor but go Running is not possible
        '                                OpenMonitorForm(Nothing, True)

        '                            Else
        '                                'No PENDING executions but several LOCKED -> Open rotor positions + Accepts
        '                                OpenRotorPositionsForm(Nothing, False, True)
        '                            End If

        '                        Else 'No automatic WS creation with LIS mode
        '                            OpenMonitorForm(Nothing)
        '                        End If
        '                        'AG 12/07/2013                                'AG 10/07/2013
        '                        Exit Select
        '                End Select
        '            Else
        '                ShowMessage(Name & ".bsTSOrdersDownloadButton_Click ", myResults.ErrorCode, myResults.ErrorMessage, MsgParent)
        '                OpenMonitorForm(Nothing)
        '            End If

        '            'AG 02/05/2013 - These flags are reset in orders download process
        '            'PendingOrders = 0
        '            'SavedOrdesfromLIMS = 0 'The LIS saved WS are also processed!!, set to 0 too
        '        End If
        '    End If
        'Catch ex As Exception
        '    Dim myLogAcciones As New ApplicationLogManager()
        '    myLogAcciones.CreateLogActivity(ex.Message, "bsTSOrdersDownloadButton_Click", EventLogEntryType.Error, False)
        'End Try
        'ScreenWorkingProcess = False
        'Cursor = Cursors.Default

        'AG 02/01/2014 - BT#1433 - New log traces (v211 patch2)
        CreateLogActivity("OrderDownload (+) button in MDI horizontal bar", "IAx00MainMDI.bsTSOrdersDownloadButton_Click", EventLogEntryType.Information, False)

        OrdersDownload()
        ' XB 24/07/2013
    End Sub

    ''' <summary>
    ''' Create Sub because PerformClick is not stable
    ''' </summary>
    ''' <remarks>Created by XB 24/07/2013</remarks>
    Private Sub OrdersDownload()
        Try

            ' If there are too much Pending Orders and Analyser is running then request user to confirm if wish to proceed
            ' all other cases continue
            If Not GlobalConstants.AnalyzerIsRunningFlag OrElse _
               PendingOrders < MAX_PENDING_ORDERS OrElse _
               ShowMessage(Name, GlobalEnumerates.Messages.PENDING_ORDER_DOWNLOAD.ToString, , , New List(Of String)(New String() {MAX_PENDING_ORDERS.ToString()})) = Windows.Forms.DialogResult.Yes Then

                'Close current screen
                Dim closedFlag As Boolean = CloseActiveMdiChild(True) 'AG 10/07/2013 - optional parameter to TRUE
                If closedFlag Then
                    Dim myLogAcciones As New ApplicationLogManager()

                    'Order download and add to worksession
                    '1- Disable UI
                    Cursor = Cursors.WaitCursor
                    EnableButtonAndMenus(False) 'Disable UI
                    SetActionButtonsEnableProperty(False) 'AG 09/07/2013


                    '2- Launch process with a new threading (like reset ws)
                    Dim resultData As New GlobalDataTO

                    'SGM 11/07/2013 + AG 11/07/2013 - get data from db
                    Dim executeDownloadOrdersProcess As Boolean = True
                    Dim maxIterations As Integer = GlobalConstants.AUTO_LIS_MAX_ITERA
                    Dim auxInteger As Integer = 0
                    Dim currentIt As Integer = 0
                    Dim myESRulesDlg As New ESBusiness

                    Dim myParams As SwParametersDelegate = New SwParametersDelegate
                    resultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.AUTO_LIS_MAX_ITERA.ToString, Nothing)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        Dim myParametersDS As ParametersDS
                        myParametersDS = CType(resultData.SetDatos, ParametersDS)
                        If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
                            maxIterations = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                        End If
                    End If
                    'SGM 11/07/2013 + AG 11/07/2013

                    ScreenWorkingProcess = True

                    'AG 11/07/2013 - add while business
                    ''3- Looping until process finishes
                    'Application.DoEvents()
                    'While processingLISOrderDownload 'ScreenWorkingProcess
                    '    Me.InitializeMarqueeProgreesBar()
                    '    Application.DoEvents()
                    'End While
                    'Application.DoEvents()
                    'StopMarqueeProgressBar()
                    '
                    ''4- Enable UI
                    'EnableButtonAndMenus(True) 'Enable UI
                    '
                    ''Results are available in variable returnValue (globalDataTO)
                    'Dim myResults As New GlobalDataTO
                    'SyncLock lockThis
                    '    myResults = resultsOrderDownloadProcess
                    'End SyncLock

                    Dim myResults As New GlobalDataTO
                    Dim cumulatedResults As Integer = 0
                    While executeDownloadOrdersProcess
                        InvokeProcessOrdersFromLIS()

                        '3- Looping until process finishes
                        Application.DoEvents()
                        While processingLISOrderDownload 'ScreenWorkingProcess
                            Me.InitializeMarqueeProgreesBar()
                            Application.DoEvents()
                        End While
                        Application.DoEvents()

                        'AG 11/07/2013
                        'XB 23/07/2013 - auto HQ
                        'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                            'XB 23/07/2013
                            'If new LIS workorders have been received while we were processing process 
                            currentIt += 1
                            If currentIt <= maxIterations Then
                                executeDownloadOrdersProcess = myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage, auxInteger)
                            Else
                                executeDownloadOrdersProcess = False
                            End If
                        Else
                            executeDownloadOrdersProcess = False
                        End If

                        'Results are available in variable returnValue (globalDataTO)
                        SyncLock lockThis
                            myResults = resultsOrderDownloadProcess
                            If cumulatedResults <> 2 Then
                                If (Not myResults.HasError) AndAlso (TypeOf (myResults.SetDatos) Is Integer) Then
                                    If cumulatedResults < DirectCast(myResults.SetDatos, Integer) Then
                                        cumulatedResults = DirectCast(myResults.SetDatos, Integer)
                                    End If
                                End If
                            End If
                        End SyncLock
                        'executeDownloadOrdersProcess = False

                        'Before exit the loop update myResults.SetDatos with the cumulated result value
                        If Not executeDownloadOrdersProcess Then
                            myResults.SetDatos = cumulatedResults
                        End If
                        'AG 11/07/2013
                    End While
                    'AG 11/07/2013

                    'AG 22/07/2013
                    If Not MDILISManager Is Nothing Then
                        MDILISManager.ClearQueueOfSpecimenNotResponded()
                    End If
                    'AG 22/07/2013
                    StopMarqueeProgressBar()

                    '4- Enable UI
                    EnableButtonAndMenus(True) 'Enable UI

                    '5- Apply refresh (open the proper screen) and show message with results (use variable myResults)
                    'Conditions: ((0)Monitor, (1)Sample Request, (2)Rotor position).
                    If (Not myResults.HasError) Then
                        Select Case CInt(myResults.SetDatos)
                            Case 1
                                'New samples are added but no set to position then open the Sample Request Screen
                                'Inform screen properties
                                IWSSampleRequest.ActiveAnalyzer = AnalyzerIDAttribute
                                IWSSampleRequest.ActiveWorkSession = WorkSessionIDAttribute
                                IWSSampleRequest.ActiveWSStatus = WSStatusAttribute
                                OpenMDIChildForm(IWSSampleRequest)

                                'XB 23/07/2013 - auto HQ
                                'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                    'XB 23/07/2013 
                                    myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download End process. Final screen SampleRequest", "IAx00MainMDI.OrdersDownload", EventLogEntryType.Information, False)
                                    Dim autoProcessUserAnswer As DialogResult = DialogResult.Yes
                                    autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(7)
                                    If HQProcessByUserFlag Then SetHQProcessByUserFlag(False) 'AG 30/07/2013 - after call CheckForExceptionsInAutoCreateWSWithLISProcess
                                    If autoProcessUserAnswer = DialogResult.Yes Then
                                        'Positive case. No exceptions
                                    ElseIf autoProcessUserAnswer = DialogResult.OK Then 'User answers stops the automatic process
                                        'Automatic process aborted
                                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                        InitializeAutoWSFlags()
                                        EnableButtonAndMenus(True, True) 'Enable buttons after update attribute!!
                                    Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                                        FinishAutomaticWSWithLIS()
                                    End If
                                End If
                                'AG 12/07/2013

                                Exit Select
                            Case 2
                                'If at least one of the order lis is set to position then open the Rotor Position Screen
                                If (IWSSampleRequest.Visible) Then
                                    'It is not possible because we have close the current MDI so there is not any screen open but the mdi (empty, only the mdi frame)
                                    'XB 23/07/2013 - auto HQ
                                    'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then 'Inform the automatic process send to position + create executions
                                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then 'Inform the automatic process send to position + create executions
                                        'XB 23/07/2013
                                        If HQProcessByUserFlag Then SetHQProcessByUserFlag(False) 'AG 30/07/2013
                                        IWSSampleRequest.OpenByAutomaticProcess = True
                                        myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download End process. Final screen SampleRequest + auto send to positioning", "IAx00MainMDI.OrdersDownload", EventLogEntryType.Information, False)
                                    End If
                                    IWSSampleRequest.SaveWSWithPositioning()
                                Else
                                    'AG 10/07/2013 - evaluate if open rotor positions in manual or automatic mode
                                    'XB 23/07/2013 - auto HQ
                                    'If Not autoWSCreationWithLISModeAttribute OrElse automateProcessCurrentState = LISautomateProcessSteps.notStarted Then 'Manual
                                    If (Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag) OrElse automateProcessCurrentState = LISautomateProcessSteps.notStarted Then 'Manual
                                        'XB 23/07/2013
                                        OpenRotorPositionsForm(Nothing)
                                    Else 'Automatic
                                        myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download open Rotor positions screen", "IAx00MainMDI.OrdersDownload", EventLogEntryType.Information, False)
                                        OpenRotorPositionsForm(Nothing, False, True) 'Inform new parameters for inform close automatically screen in order to generate executions
                                    End If
                                    'AG 10/07/2013
                                End If
                                Exit Select
                            Case Else
                                'Open Monitor screen.
                                'AG 18/07/2013 - After orderdownload the open screen is monitor --> Go to running if any pending execution
                                '                if all executions locked go to Rotor positions and press Accept automatically
                                'AG 12/07/2013
                                'OpenMonitorForm(Nothing)
                                'XB 23/07/2013 - auto HQ
                                'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                    'XB 23/07/2013
                                    myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: After orders download nothing added to Work Session", "IAx00MainMDI.OrdersDownload", EventLogEntryType.Information, False)
                                    Dim autoProcessUserAnswer As DialogResult = DialogResult.OK
                                    autoProcessUserAnswer = CheckForExceptionsInAutoCreateWSWithLISProcess(8)
                                    If autoProcessUserAnswer = DialogResult.Yes Then
                                        If HQProcessByUserFlag Then InitializeAutoWSFlags() 'AG + SA 30/07/2013 - when process finishes
                                        'Exists some execution with status PENDING -> Open monitor and Running
                                        OpenMonitorForm(Nothing, True)

                                    ElseIf autoProcessUserAnswer = DialogResult.OK Then
                                        If HQProcessByUserFlag Then SetHQProcessByUserFlag(False) 'AG 30/07/2013 - when process finishes
                                        'Exists no executions PENDING/LOCKED and no PENDING orderTest -> Open monitor but go Running is not possible
                                        OpenMonitorForm(Nothing, True)

                                    Else
                                        'No PENDING executions but several LOCKED -> Open rotor positions + Accepts
                                        OpenRotorPositionsForm(Nothing, False, True)
                                    End If

                                Else 'No automatic WS creation with LIS mode
                                    OpenMonitorForm(Nothing)
                                End If
                                'AG 12/07/2013                                'AG 10/07/2013
                                Exit Select
                        End Select
                    Else
                        ShowMessage(Name & ".OrdersDownload ", myResults.ErrorCode, myResults.ErrorMessage, MsgParent)
                        OpenMonitorForm(Nothing)
                    End If

                    'AG 02/05/2013 - These flags are reset in orders download process
                    'PendingOrders = 0
                    'SavedOrdesfromLIMS = 0 'The LIS saved WS are also processed!!, set to 0 too
                End If
            End If
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "OrdersDownload", EventLogEntryType.Error, False)
        End Try
        ScreenWorkingProcess = False
        Cursor = Cursors.Default
    End Sub

    Private Sub LISUtilitiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LISUtilitiesToolStripMenuItem.Click
        'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
        'The form to be opened should be assigned its AcceptButton property to its default exit button
        OpenMDIChildForm(ILISUtilities)
    End Sub

#End Region

#Region "ForTESTING_REMOVE"
    Private Sub BorrameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BorrameToolStripMenuItem.Click
        Ax00MainForm.Show()
        'Using myfrm As New Ax00MainForm()
        '    myfrm.ShowDialog()
        'End Using
    End Sub

#End Region

#Region "Help Area"
    ''' <summary>
    ''' Get the help file name on the current application language.
    ''' </summary>
    ''' <remarks>CREATED BY: TR 03/11/2011</remarks>
    Public Sub SetHelpProvider()
        Try

            'A400HelpProvider.HelpNamespace = GetHelpFilePath(HELP_FILE_TYPE.MANUAL, CurrentLanguageAttribute)
            'A400HelpProvider.SetHelpNavigator(Me, HelpNavigator.TableOfContents)
            'A400HelpProvider.SetHelpNavigator(Me, HelpNavigator.AssociateIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetHelpProvider ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub QuickToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuickToolStripMenuItem.Click
        Try
            'Help.ShowHelp(Me, GetHelpFilePath(HELP_FILE_TYPE.QUICK_GUIDE, CurrentLanguageAttribute))
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".QuickToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    Private Sub ToolStripMenuItem10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem10.Click
        Try
            'Help.ShowHelp(Me, GetHelpFilePath(HELP_FILE_TYPE.MANUAL, CurrentLanguageAttribute))
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ToolStripMenuItem10_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

#End Region

End Class