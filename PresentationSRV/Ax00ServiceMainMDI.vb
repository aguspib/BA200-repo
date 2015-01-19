Option Strict On
Option Explicit On

'Imports System.Windows.Forms
Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.DAL
'Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.FwScriptsManagement
'Imports Biosystems.Ax00.InfoAnalyzer
'Imports System.IO
'Imports System.Threading
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Controls.UserControls

Public Class Ax00ServiceMainMDI
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    'Private AutoConnectProcess As Boolean = False
    Private AutoConnectFailsTitle As String = ""
    Private AutoConnectFailsErrorCode As String = ""

    Public WithEvents MDIAnalyzerManager As AnalyzerManager

    'Monitor Sensors
    'Private Const MONITOR_INTERVAL As Integer = 2000
    'Private FirstTime As Boolean = True
    'Private MonitorTimerCallBack As TimerCallback
    'Private WithEvents MonitorTimer As System.Threading.Timer
    Private IsRefreshing As Boolean = True
    Private MonitorSensorsActive As Boolean = False

    ' XBC 18/05/2011
    Private myAllAdjustmentsDS As SRVAdjustmentsDS

    ' XBC 27/05/2011
    Private myCurrentUserLevel As USER_LEVEL

    ' XBC 25/07/2011
    Private Const MAX_SIZE_LOG As Long = 5000000
    'SGM 21/09/2011
    Private myFwAdjustmentsDelegate As FwAdjustmentsDelegate

    'SGM 30/09/2011
    ' Waiting process functionality


    Private ReconnectRequested As Boolean
    'Private AdjustRequested As Boolean
    Private UserStandByRequested As Boolean
    Private UserSleepRequested As Boolean

    'DL 20/04/2012
    'Protected wfWaitScreen As Biosystems.Ax00.PresentationCOM.IAx00StartUp
    Protected wfWaitScreen As Biosystems.Ax00.PresentationCOM.WaitScreen
    'DL 20/04/2012


    'TR 20/10/2011 
    Private myDriveDetector As Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetector

    'SGM 15/11/2011
    Public IsAutoConnecting As Boolean = False

    ''SGM 23/11/2011
    'Public IsAutoInfoActivated As Boolean = False

    ' XBC 20/04/2012
    Public pScreenPendingToOpen As BSBaseForm

    ' XBC 27/04/2012
    Private CheckStress As Boolean
    Private CheckAnalyzerInfo As Boolean

    ' XBC 05/06/2012
    Public SkipQuestionExitProgram As Boolean = False

    Private ShutDownisPending As Boolean = False        ' XBC 26/06/2012

    ' XBC 18/09/2012 - add Update Fw functionality flag
    Public UpdateFirmwareProcessEnd As Boolean = False

    ' XBC 21/09/2012
    Public IsAnalyzerInfoScreenRunning As Boolean

    'SGM 19/10/2012
    Private RecoverCompleted As Boolean = False
    Private RecoverRequested As Boolean = False
    Private IsWaitingForRecoverAttr As Boolean = False
    Private ManageAlarmTypeAttr As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE
    Private mySpecialSimpleErrors As New List(Of String)

#End Region

#Region "Attributes"
    Private SimulationModeAttr As Boolean
    Private AnalyzerModelAttribute As String = ""
    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""    ' XB 12/11/2013
    Private AnalyzerIDAttribute As String = ""
    Private CurrentLanguageAttribute As String = ""
    Private CurrentUserLevelAttribute As Integer
    ' XBC 15/07/2011
    Private FwCompatibleAttr As Boolean

    'SGM 19/09/2011
    Private AdjustmentsReadedAttr As Boolean = False

    'SGM 23/09/2011
    Private IsFinalWashingNeededAttr As Boolean = False
    Private IsFinalWashed As Boolean = True
    Public IsFinalClosing As Boolean = False

    'SGM 26/09/2011 PENDING from DB
    Private ConnectedText As String = "CONNECTED"
    Private NotConnectedText As String = "NOT CONNECTED"

    'SGM 27/09/2011
    Private AutoConnectProcessAttr As Boolean = False

    'SGM 15/11/2011
    Private isWaitingForConnectedAttr As Boolean
    Private isWaitingForStandByAttr As Boolean
    Private isWaitingForSleepAttr As Boolean
    Private isWaitingForCloseAppAttr As Boolean
    Private isWaitingForAdjustAttr As Boolean

    'SGM 28/11/2011
    Private FwVersionAttribute As String = "0000000000"

    'SGM 02/02/2012
    Private FwScriptsLoadedOKAttr As Boolean = False
    Private FwAdjustmentsMasterDataLoadedOKAttr As Boolean = False


#End Region

#Region "Properties"

    Private myConsole As DebugTrace

    'SGM 25/11/2011
    Public Property SimulationMode() As Boolean
        Get
            Return SimulationModeAttr
        End Get
        Set(ByVal value As Boolean)
            If SimulationModeAttr <> value Then
                SimulationModeAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Public Property isWaitingForConnected() As Boolean
        Get
            Return isWaitingForConnectedAttr
        End Get
        Set(ByVal value As Boolean)
            If isWaitingForConnectedAttr <> value Then
                isWaitingForConnectedAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Public Property isWaitingForStandBy() As Boolean
        Get
            Return isWaitingForStandByAttr
        End Get
        Set(ByVal value As Boolean)
            If isWaitingForStandByAttr <> value Then
                isWaitingForStandByAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Public Property isWaitingForSleep() As Boolean
        Get
            Return isWaitingForSleepAttr
        End Get
        Set(ByVal value As Boolean)
            If isWaitingForSleepAttr <> value Then
                isWaitingForSleepAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Public Property isWaitingForCloseApp() As Boolean
        Get
            Return isWaitingForCloseAppAttr
        End Get
        Set(ByVal value As Boolean)
            If isWaitingForCloseAppAttr <> value Then
                isWaitingForCloseAppAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Public Property isWaitingForAdjust() As Boolean
        Get
            Return isWaitingForAdjustAttr
        End Get
        Set(ByVal value As Boolean)
            If isWaitingForAdjustAttr <> value Then
                isWaitingForAdjustAttr = value
            End If
        End Set
    End Property

    ' XBC 23/10/2012
    Public Property isWaitingForRecover() As Boolean
        Get
            Return IsWaitingForRecoverAttr
        End Get
        Set(ByVal value As Boolean)
            If IsWaitingForRecoverAttr <> value Then
                IsWaitingForRecoverAttr = value
            End If
        End Set
    End Property

    'SGM 15/11/2011
    Private ReadOnly Property IsWaitingForNewStatus() As Boolean
        Get
            Return (MyClass.isWaitingForConnected Or _
                    MyClass.isWaitingForStandBy Or _
                    MyClass.isWaitingForSleep Or _
                    MyClass.isWaitingForAdjust)
        End Get
    End Property

    Public Property ActiveAnalyzerModel() As String
        Get
            Return AnalyzerModelAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerModelAttribute = value
        End Set
    End Property

    Public Property ActiveAnalyzer() As String
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
            AnalyzerIDAttribute = value
        End Set
    End Property

    ' XB 12/11/2013
    Public Property ActiveStatus() As String
        Get
            Return WSStatusAttribute
        End Get
        Set(ByVal value As String)
            WSStatusAttribute = value
        End Set
    End Property

    'SGM 28/11/2011
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

    'SGM 30/05/2012
    Public ReadOnly Property ActiveSwVersion() As String
        Get
            Dim myFileVersion As FileVersionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            With myFileVersion
                Return (.FileMajorPart & "." & .FileMinorPart & "." & .FileBuildPart & "." & .FilePrivatePart)
            End With
        End Get
    End Property


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 30/05/2012</remarks>
    Public Function GetApplicationVersion() As String
        Dim myVersion As String = ""
        Try
            Dim myFileVersion As FileVersionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            With myFileVersion
                myVersion = .FileMajorPart & "." & .FileMinorPart & "." & .FileBuildPart & "." & .FilePrivatePart
            End With
        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "Utilities.GetApplicationVersion", EventLogEntryType.Error, False)
        End Try
        Return myVersion
    End Function

    Public WriteOnly Property CurrentLanguage() As String
        Set(ByVal value As String)
            If (String.Compare(CurrentLanguageAttribute, value, False) <> 0) Then
                CurrentLanguageAttribute = value
                GetScreenLabels()
            End If
        End Set
    End Property

    Public Property FwCompatible() As Boolean
        Get
            Return Me.FwCompatibleAttr
        End Get
        Set(ByVal value As Boolean)
            Me.FwCompatibleAttr = value
        End Set
    End Property

    Public Property AdjustmentsReaded() As Boolean
        Get
            Return AdjustmentsReadedAttr
        End Get
        Set(ByVal value As Boolean)

            Dim myGlobal As New GlobalDataTO

            If AdjustmentsReadedAttr <> value Then

                AdjustmentsReadedAttr = value



                'SGM 15/11/2011 made with SEND_START
                ''the INFO data refreshing mode can only be activated after Adjustments have been readed
                ''SGM 19/09/2011
                'If Me.MDIAnalyzerManager.Connected And value Then
                '    If Not Me.SimulationMode Then

                '        myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                '                                             True, _
                '                                             Nothing, _
                '                                             GlobalEnumerates.Ax00InfoInstructionModes.STR)


                '    Else
                '        Me.BsInfoTimer.Enabled = True
                '    End If

                'End If

                If value Then
                    MyClass.RecoverCompleted = False
                End If
            End If
        End Set
    End Property

    'SGM 23/09/2011
    Public Property IsFinalWashingNeeded() As Boolean
        Get
            Return IsFinalWashingNeededAttr
        End Get
        Set(ByVal value As Boolean)
            If Not IsFinalWashingNeededAttr And value Then
                Me.IsFinalWashed = False
            End If
            IsFinalWashingNeededAttr = value
        End Set
    End Property

    'SGM 29/09/2011
    Private Property AutoConnectProcess() As Boolean
        Get
            Return AutoConnectProcessAttr
        End Get
        Set(ByVal value As Boolean)
            If AutoConnectProcessAttr <> value Then
                AutoConnectProcessAttr = value
            End If
        End Set
    End Property

    'SGM 02/02/2012
    Private ReadOnly Property FwScriptsLoadedOK() As Boolean
        Get
            Return FwScriptsLoadedOKAttr
        End Get
    End Property

    'SGM 02/02/2012
    Private ReadOnly Property FwAdjustmentsMasterDataLoadedOK() As Boolean
        Get
            Return FwAdjustmentsMasterDataLoadedOKAttr
        End Get
    End Property

    'SGM 16/04/2012
    Public ReadOnly Property IsAnalyzerInitiated() As Boolean
        Get
            If MyClass.SimulationMode Or GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
                Return True
            End If

            Dim res As Boolean
            If MyClass.AdjustmentsReaded Then
                If MyClass.MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled Then
                    If Not MyClass.MDIAnalyzerManager.ISE_Manager.IsLongTermDeactivation Then
                        If MyClass.MDIAnalyzerManager.ISE_Manager.IsISESwitchON Then
                            'res = (Not MyClass.MDIAnalyzerManager.ISE_Manager.IsISEInitiating And MyClass.MDIAnalyzerManager.ISE_Manager.IsISEInitializationDone)

                            res = (MyClass.MDIAnalyzerManager.Connected And (MyClass.MDIAnalyzerManager.ISE_Manager.IsLongTermDeactivation Or MyClass.MDIAnalyzerManager.ISE_Manager.IsISEInitializationDone))
                        Else

                            res = (MyClass.MDIAnalyzerManager.Connected And Not MyClass.MDIAnalyzerManager.ISE_Manager.IsISEInitiating)
                            'If MyClass.MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Then
                            '    res = True
                            'End If
                        End If
                    Else
                        res = MyClass.MDIAnalyzerManager.Connected
                    End If
                Else
                    res = MyClass.MDIAnalyzerManager.Connected
                End If
            Else
                'If MyClass.MDIAnalyzerManager.IsInstructionAborted Then 'SGM 19/11/2012
                '    res = True
                If MyClass.RecoverCompleted Then
                    res = True
                ElseIf MyClass.MDIAnalyzerManager.Connected Then
                    res = False
                Else
                    res = True
                End If
            End If

            If Not res Then
                Me.Cursor = Cursors.WaitCursor
            End If

            Return res

        End Get
    End Property

    'SGM 08/11/2012
    Private Property ManageAlarmType() As GlobalEnumerates.ManagementAlarmTypes
        Get
            Return ManageAlarmTypeAttr
        End Get
        Set(ByVal value As GlobalEnumerates.ManagementAlarmTypes)
            If ManageAlarmTypeAttr <> value Then
                ManageAlarmTypeAttr = value
            End If
        End Set
    End Property

#End Region

#Region "Structures"

    'XBC 18/05/11
    Private Structure AdjustmentRowData
        Public CodeFw As String
        Public Value As String
        Public GroupID As String
        Public AxisID As String
        Public InFile As Boolean
        Public CanSave As Boolean


        Public Sub New(ByVal pCodeFw As String)
            CodeFw = pCodeFw
            Value = ""
            GroupID = ""
            InFile = False
        End Sub

        Public Sub Clear()
            CodeFw = ""
            Value = ""
            GroupID = ""
            InFile = False
        End Sub
    End Structure
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initializes a new instance of the Ax00ServiceMainMDI class
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 15/10/2010
    ''' </remarks>
    Public Sub New()
        Try
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            'TR 24/10/2011 
            myDriveDetector = New Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetector
            AddHandler myDriveDetector.DeviceRemoved, AddressOf OnDeviceRemoved

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " New ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'RH: At this point the Ax00ServiceMainMDI new object does not exist yet.
            'As the ShowMessage() method references Ax00ServiceMainMDI object as the parent window,
            'we can't call it here, so we call MessageBox.Show() instead.
            MessageBox.Show(GlobalEnumerates.Messages.SYSTEM_ERROR.ToString() & " - " & ex.Message, _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try

    End Sub
#End Region

#Region "Common Forms"

    Private WithEvents myLanguageConfig As IConfigLanguage
    Private WithEvents myConfigAnalyzers As IConfigGeneral
    'Private WithEvents myChangePassword As IPassword
    Private WithEvents myLogin As IAx00Login
    Private WithEvents myConfigUsers As IConfigUsers
    Private WithEvents mySettings As ISettings
    Private WithEvents myISEUtilities As IISEUtilities ' XBC 05/02/2012 ISE Utilities placed into PresetationCOM layer
    Private WithEvents myConfigBarCode As IBarCodesConfig ' XBC 14/02/2012 Barcodes Configuration placed into PresetationCOM layer
    Private WithEvents myAbout As IAboutBox 'RH 28/03/2012
    Private WithEvents myInstrumentInfo As IInstrumentInfo
    Private WithEvents myChangeRotor As IChangeRotorSRV
#End Region

#Region "Another Forms"
    Private WithEvents myErrCodesForm As IErrorCodes    ' XBC 07/11/2012
#End Region

#Region "Communications Events"

    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 27/05/2011
    ''' </remarks>
    Public Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) _
                                      Handles MDIAnalyzerManager.ReceptionEvent

        Me.UIThread(Function() ManageReceptionEvent(pInstructionReceived, pTreated, pRefreshEvent, pRefreshDS, pMainThread))

    End Sub

    ''' <summary>
    ''' Event to Show Reception Event information into presentation layer
    ''' Remarks : If is necessary to implement business logic it must be to implement into ScriptsManagement layer
    ''' </summary>
    ''' <param name="pInstructionReceived"></param>
    ''' <remarks>
    ''' Created by XBC 10/11/2010
    ''' Modified by XBC 17/05/2011 - Unified Reception event for sensors to allowed to refresh some screens at same time
    ''' Modified by: RH - 27/05/2011
    '''              XB - 13/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004
    ''' </remarks>
    Public Function ManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                  ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) As Boolean

        Dim myGlobal As New GlobalDataTO

        Try
            'SGM 19/09/2011
            'SyncLock ensures that multiple threads do not execute the statement block at the same time. 
            'SyncLock prevents each thread from entering the block until no other thread is executing it.
            'Protect pRefreshDS data from being updated by more than one thread simultaneously. 
            'If the statements that manipulate the data must go to completion without interruption, put them inside a SyncLock block.
            Dim lockThis As New Object()
            Dim copyRefreshDS As UIRefreshDS = Nothing
            Dim copyRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)

            SyncLock lockThis
                copyRefreshDS = CType(pRefreshDS.Copy(), UIRefreshDS)
                For Each item As GlobalEnumerates.UI_RefreshEvents In pRefreshEvent
                    copyRefreshEventList.Add(item)
                Next
                If pRefreshEvent.Count > 0 Then MDIAnalyzerManager.ReadyToClearUIRefreshDS(pMainThread) 'Inform the ui refresh dataset can be cleared so they are already copied
            End SyncLock
            'END SGM 19/09/2011



            If pTreated Then

                'block the data visualization
                Select Case MDIAnalyzerManager.InstructionTypeReceived
                    Case AnalyzerManagerSwActionList.ANSINF_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSBXX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFBX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFCP_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFDX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFGL_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFJE_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSFRX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSGLF_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSJEX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSSFX_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSCPU_RECEIVED, _
                    AnalyzerManagerSwActionList.ANSDXX_RECEIVED


                        'MDIAnalyzerManager.IsDisplayingServiceData = True 'to avoid updating data while displaying



                        If Not ActiveMdiChild Is Nothing Then

                            'SGM 18/09/2012 - Refresh ISE Utilities
                            If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If

                            'SGM 09/11/2012 - Refresh Analyzer Info
                            If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If
                        End If

                End Select


                'Refresh button bar the Ax00 and the connection Status  
                Select Case MDIAnalyzerManager.InstructionTypeReceived

                    Case AnalyzerManagerSwActionList.STATUS_RECEIVED
                        '
                        ' STATUS RECEIVED
                        ' 

                        'SGM 19/10/2012 - Alarms Management
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then

                            Dim sensorValue As Single = 0
                            sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE)
                            If sensorValue > 0 Then

                                'Dim ManageAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE
                                ManageAlarmType = CType(sensorValue, GlobalEnumerates.ManagementAlarmTypes)

                                'If Not MyClass.MDIAnalyzerManager.IsServiceAlarmInformed Then
                                'MyClass.MDIAnalyzerManager.IsServiceAlarmInformedAttr = True

                                'Manage
                                MyClass.ManageAlarmStep1(ManageAlarmType)
                                'End If

                                'Pending
                                'If MDIAnalyzerManager.ErrorCodesDisplay.Count > 0 Then
                                '    Me.bsTSWarningButton.Enabled = True
                                'Else
                                '    Me.bsTSWarningButton.Enabled = False
                                'End If

                                'exit from reception
                                Return True

                            End If
                        Else
                            ManageAlarmType = ManagementAlarmTypes.NONE
                        End If
                        'SGM 19/10/2012 - Alarms Management






                        'SGM 19/10/2012
                        'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                        '    ' Refresh Stress Mode screen
                        '    If Not ActiveMdiChild Is Nothing Then
                        '        If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                        '            Dim CurrentMdiChild As IStressModeTest = CType(ActiveMdiChild, IStressModeTest)
                        '            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        '        End If
                        '    End If
                        'End If

                        ' Refresh Analyzer Info screen
                        For Each oForm As Form In Me.MdiChildren
                            If oForm Is AnalyzerInfo Then
                                Dim CurrentMdiChild As AnalyzerInfo = CType(oForm, AnalyzerInfo)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.STANDBY_START
                                        CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.STANDBY_DOING
                                    Case AnalyzerManagerAx00Actions.STANDBY_END
                                        CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE

                                    Case AnalyzerManagerAx00Actions.UTIL_START
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select

                                'SGM 19/10/2012 - alarms already managed
                                'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                'End If

                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                Exit For
                            End If
                        Next



                        ' Refresh Instrument Update Utility screen
                        If Not ActiveMdiChild Is Nothing Then
                            If (TypeOf ActiveMdiChild Is IInstrumentUpdateUtil) Then
                                Dim CurrentMdiChild As IInstrumentUpdateUtil = CType(ActiveMdiChild, IInstrumentUpdateUtil)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.STANDBY_START
                                        CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.STANDBY_DOING
                                    Case AnalyzerManagerAx00Actions.STANDBY_END
                                        CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.STANDBY_DONE
                                End Select

                                'SGM 19/10/2012 - alarms already managed
                                'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                'End If

                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If
                        End If

                        ' Refresh Thermos Adjustment screen
                        If Not ActiveMdiChild Is Nothing Then
                            If (TypeOf ActiveMdiChild Is IThermosAdjustments) Then
                                Dim CurrentMdiChild As IThermosAdjustments = CType(ActiveMdiChild, IThermosAdjustments)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                                        ' Nothing by now
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END

                                        'SGM 19/10/2012 - alarms already managed
                                        'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                        '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                        'End If

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                        ' XB 13/10/2014 - BA-2004
                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select

                            End If
                        End If


                        ' XBC 05/12/2011
                        ' Refresh Positions Adjustment screen
                        If Not ActiveMdiChild Is Nothing Then

                            If (TypeOf ActiveMdiChild Is IPositionsAdjustments) Then
                                Dim CurrentMdiChild As IPositionsAdjustments = CType(ActiveMdiChild, IPositionsAdjustments)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                                        ' Nothing by now
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END

                                        'SGM 19/10/2012 - alarms already managed
                                        'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                        '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                        'End If

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                        ' XB 13/10/2014 - BA-2004
                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select
                            End If

                        End If

                        ' SGM 21/05/2012
                        ' Refresh Motors Test screen
                        If Not ActiveMdiChild Is Nothing Then

                            If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                                        ' Nothing by now
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END

                                        'SGM 19/10/2012 - alarms already managed
                                        'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                        '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                        'End If

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                    Case AnalyzerManagerAx00Actions.UTIL_START
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                        ' XB 13/10/2014 - BA-2004
                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select
                            End If

                        End If



                        ' XBC 20/02/2012 
                        ' Refresh Photometry Adjustment screen 
                        If Not ActiveMdiChild Is Nothing Then
                            If (TypeOf ActiveMdiChild Is IPhotometryAdjustments) Then
                                Dim CurrentMdiChild As IPhotometryAdjustments = CType(ActiveMdiChild, IPhotometryAdjustments)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                                        ' Nothing by now
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END

                                        'SGM 19/10/2012 - alarms already managed
                                        'If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                                        '    CurrentMdiChild.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                                        'End If

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                        ' XB 13/10/2014 - BA-2004
                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select

                            End If
                        End If

                        'SGM 15/11/2012
                        ' Refresh Change Rotor screen
                        If Not ActiveMdiChild Is Nothing Then

                            If (TypeOf ActiveMdiChild Is IChangeRotorSRV) Then
                                Dim CurrentMdiChild As IChangeRotorSRV = CType(ActiveMdiChild, IChangeRotorSRV)

                                Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_START
                                        ' Nothing by now
                                    Case AnalyzerManagerAx00Actions.WASHSTATION_CTRL_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_START
                                        'reset E:550 is informed flag
                                        MyClass.MDIAnalyzerManager.IsServiceRotorMissingInformed = False

                                    Case AnalyzerManagerAx00Actions.NEW_ROTOR_END

                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End Select
                            End If

                        End If

                        'SGM 18/09/2012 - Refresh ISE Utilities 
                        If Not ActiveMdiChild Is Nothing Then
                            If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If
                        End If

                        Select Case MDIAnalyzerManager.AnalyzerCurrentAction
                            Case AnalyzerManagerAx00Actions.CONNECTION_DONE
                                Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
                                Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString

                                If Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                                    ' XBC 03/10/2011
                                    If Me.isWaitingForConnected Then
                                        Me.isWaitingForConnected = False 'SGM 15/11/2011
                                        If Me.MDIAnalyzerManager.IsUserConnectRequested Then

                                            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                            Dim myText1 As String
                                            Dim myText2 As String

                                            ' TEMPORAL FINS QUE FUNCIONI POLLFW
                                            'myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SETTING_STANDBY", CurrentLanguageAttribute)
                                            'myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STANDBY_WAIT", CurrentLanguageAttribute)
                                            'Me.BsMonitor.DisableAllSensors()
                                            'Me.isWaitingForConnected = False
                                            'Me.UserStandByRequested = True
                                            'Me.WaitControl(myText1, myText2)
                                            'Me.MDIAnalyzerManager.ClearQueueToSend()
                                            'If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                            '    Me.SEND_STANDBY()
                                            'End If
                                            ' TEMPORAL FINS QUE FUNCIONI POLLFW



                                            myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FW_VERSION_READ", CurrentLanguageAttribute)
                                            myText2 = ""
                                            Me.BsMonitor.DisableAllSensors()
                                            Me.isWaitingForConnected = False
                                            Me.WaitControl(myText1, myText2)
                                            Me.MDIAnalyzerManager.ClearQueueToSend()
                                            If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                                Me.SEND_POLLFW(POLL_IDs.CPU)
                                            End If


                                            ' XBC 14/05/2012
                                        Else
                                            If Not Me.CheckAnalyzerInfo Then
                                                Me.CheckAnalyzerInfo = True
                                                MyClass.SEND_INFO_STOP()
                                                OpenAnalyzerInfoScreen()
                                            End If
                                            ' XBC 14/05/2012




                                        End If
                                    ElseIf Me.isWaitingForSleep Then
                                        Me.isWaitingForSleep = False
                                    End If
                                    ' XBC 03/10/2011



                                ElseIf Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then

                                    Me.isWaitingForConnected = False 'SGM 15/11/2011
                                    Me.isWaitingForStandBy = False 'SGM 15/11/2011
                                    If Not Me.AdjustmentsReaded And Not Me.isWaitingForAdjust Then
                                        ' XBC 27/10/2011 - can't send another READADJ if AnalyazerInfo screen is loading and sending another READADJ
                                        'Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                        If Me.MDIAnalyzerManager.IsUserConnectRequested Then

                                            '' XBC 01/05/2012
                                            ''Debug.Print(" - " & Date.Now.ToString & " - STANDBY ! ")
                                            ''Debug.Print(" - " & Date.Now.ToString & " - previous CHECK STRESS ")
                                            'If Not Me.CheckStress Then
                                            '    'Debug.Print(" - " & Date.Now.ToString & " - SEND SDPOLL ")
                                            '    Me.CheckStress = True
                                            '    System.Threading.Thread.Sleep(500)
                                            '    MyClass.SEND_SDPOLL()
                                            '    Exit Try

                                            'ElseIf Not Me.MDIAnalyzerManager.IsStressing Then

                                            '    If Not ActiveMdiChild Is Nothing Then
                                            '        If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                            '            Exit Try
                                            '        End If
                                            '    End If

                                            '    Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                            'End If
                                            ''Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                            '' XBC 01/05/2012


                                            Dim myText1 As String = ""
                                            Dim myText2 As String = ""
                                            Dim myMultiLangResourcesDelegate As MultilanguageResourcesDelegate = New MultilanguageResourcesDelegate
                                            myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FW_VERSION_READ", CurrentLanguageAttribute)
                                            myText2 = ""
                                            Me.BsMonitor.DisableAllSensors()
                                            Me.isWaitingForConnected = False
                                            Me.WaitControl(myText1, myText2)
                                            Me.MDIAnalyzerManager.ClearQueueToSend()
                                            If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                                Me.SEND_POLLFW(POLL_IDs.CPU)
                                            End If
                                            ' XBC 14/05/2012
                                        Else
                                            'Debug.Print(" - " & Date.Now.ToString & " - STANDBY (Else) ! ")
                                            'Debug.Print(" - " & Date.Now.ToString & " - previous CHECK STRESS ")

                                            If Not Me.CheckStress Then
                                                'Debug.Print(" - " & Date.Now.ToString & " - SEND SDPOLL ")
                                                Me.CheckStress = True
                                                MyClass.SEND_SDPOLL()
                                                Exit Try
                                            End If
                                            ' XBC 14/05/2012

                                        End If
                                        ' XBC 27/10/2011

                                    End If
                                    If Me.wfWaitScreen IsNot Nothing Then
                                        Me.wfWaitScreen.Close()
                                    End If

                                    Me.MDIAnalyzerManager.IsUserConnectRequested = False
                                    Me.ActivateActionButtonBar(True)
                                    Me.ActivateMenus(True)
                                    Me.bsAnalyzerStatus.Text = Me.MDIAnalyzerManager.AnalyzerStatus.ToString
                                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
                                    Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString
                                    Me.MDIAnalyzerManager.ClearQueueToSend()
                                    If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected AndAlso Me.AdjustmentsReaded Then
                                        Me.SEND_INFO_START()
                                    End If

                                    ' XBC 21/09/2012 - RUNNING case ! Nothing is allowed to do with Service Software
                                    ' XBC 21/09/2012 - PENDING TO SPEC !!!
                                ElseIf Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                                    If (MDIAnalyzerManager.StopComm()) Then
                                        Me.ActivateActionButtonBar(True)
                                        Me.ActivateMenus(True)
                                    End If
                                    Me.Cursor = Cursors.Default
                                    ShowMessage("Error", GlobalEnumerates.Messages.RUNNING.ToString)
                                    Exit Try
                                    ' XBC 21/09/2012 - PENDING TO SPEC !!!
                                End If

                            Case AnalyzerManagerAx00Actions.STANDBY_END
                                If Me.isWaitingForStandBy Then

                                    Me.isWaitingForStandBy = False

                                    'SGM 30/10/2012 - delay for connecting after FW UPDATE
                                    If Not MyClass.IsAutoConnecting Then
                                        System.Threading.Thread.Sleep(1000)
                                    End If

                                    If Me.UserStandByRequested And Not Me.AdjustmentsReaded And Not Me.isWaitingForAdjust And Not Me.IsAutoConnecting Then
                                        If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                            ' XBC 01/05/2012
                                            'Debug.Print(" - " & Date.Now.ToString & " - STANDBY_END ! ")
                                            'Debug.Print(" - " & Date.Now.ToString & " - previous CHECK STRESS ")
                                            If Not Me.CheckStress Then
                                                'Debug.Print(" - " & Date.Now.ToString & " - SEND SDPOLL ")
                                                Me.CheckStress = True
                                                System.Threading.Thread.Sleep(500)
                                                MyClass.SEND_SDPOLL()
                                                Exit Try

                                            ElseIf Not Me.MDIAnalyzerManager.IsStressing Then

                                                If Not ActiveMdiChild Is Nothing Then
                                                    If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                                        Exit Try
                                                    End If
                                                End If

                                                Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                                CreateLogActivity("Read Adjustments (3)", Me.Name & ".OnManageReceptionEvent ", EventLogEntryType.Information, False)

                                            End If
                                            'Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                            ' XBC 01/05/2012
                                        End If
                                    End If


                                    Me.MDIAnalyzerManager.IsUserConnectRequested = False
                                    Me.ActivateActionButtonBar(True)
                                    Me.ActivateMenus(True)
                                    Me.bsAnalyzerStatus.Text = Me.MDIAnalyzerManager.AnalyzerStatus.ToString
                                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
                                    Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString
                                    Me.MDIAnalyzerManager.ClearQueueToSend()

                                    Me.BsMonitor.EnableAllSensors() 'SGM 08/11/2012

                                    'SGM 16/11/2011
                                    'If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected AndAlso Me.AdjustmentsReaded Then
                                    '    Me.SEND_INFO_START()
                                    'End If
                                End If

                            Case AnalyzerManagerAx00Actions.SLEEP_END
                                If Me.isWaitingForSleep Then
                                    Me.MDIAnalyzerManager.IsShutDownRequested = False
                                    Me.isWaitingForSleep = False
                                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, False, Me.NotConnectedText)
                                    Me.bsAnalyzerStatus.Text = "NOT CONNECTED"
                                    Me.AdjustmentsReaded = False 'SGM 16/11/2011

                                    'SGM 21/11/2012 - reset flag
                                    MyClass.MDIAnalyzerManager.IsShutDownRequested = False

                                    ' XBC 04/10/2011
                                    If Me.isWaitingForCloseApp Then
                                        Me.isWaitingForCloseApp = True
                                        Me.CloseApplication()
                                    End If

                                End If

                            Case AnalyzerManagerAx00Actions.WASHING_RUN_END 'pending to spec if it is needed to wash before shutdown
                                ''if there was Washing needed
                                'If me.MDIAnalyzerManager.IsShutDownRequested Then
                                '    If me.IsFinalWashingNeeded Then
                                '        me.IsFinalWashed = True
                                '        me.ShutDownAnalyzer()
                                '    End If
                                'End If


                                ' XBC 23/10/2012 - RECOVER Treatment
                            Case AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END
                                RecoverRequested = False
                                isWaitingForRecover = False
                                RecoverCompleted = True

                                'SGM 08/11/2012
                                MyClass.MDIAnalyzerManager.IsAutoInfoActivated = False
                                Me.SEND_INFO_START()
                                System.Threading.Thread.Sleep(500)
                                Me.BsMonitor.EnableAllSensors()

                                Me.MDIAnalyzerManager.IsUserConnectRequested = False
                                Me.ActivateActionButtonBar(True)
                                Me.ActivateMenus(True)
                                Me.bsAnalyzerStatus.Text = Me.MDIAnalyzerManager.AnalyzerStatus.ToString
                                Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
                                Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString
                                Me.MDIAnalyzerManager.ClearQueueToSend()
                                'end SGM 08/11/2012



                        End Select


                        ''end SGM 27/09/2011 


                        ' XBC 14/02/2012 - CODEBR Configuration instruction + CONFIG general instruction 
                        If MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.CONFIG_DONE Then
                            If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected AndAlso Me.AdjustmentsReaded Then
                                If Not Me.MDIAnalyzerManager.IsStressing Then
                                    Me.SEND_INFO_START()
                                End If
                            End If


                            ' XBC 16/05/2012
                            ' Refresh Stress Mode screen
                            If Not ActiveMdiChild Is Nothing Then
                                If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                    Dim CurrentMdiChild As IStressModeTest = CType(ActiveMdiChild, IStressModeTest)
                                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                End If
                            End If
                            ' XBC 16/05/2012

                        End If
                        ' XBC 14/02/2012 - CODEBR Configuration instruction + CONFIG general instruction  



                    Case AnalyzerManagerSwActionList.ADJUSTMENTS_RECEIVED
                        ' 
                        ' ADJUSTMENTS RECEIVED 
                        ' 
                        Me.ReadAllFwAdjustments()
                        Me.AdjustmentsReaded = True
                        Me.isWaitingForAdjust = False 'SM 15/11/2011 

                        'SGM 16/11/2011 
                        Me.ActivateActionButtonBar(True)
                        Me.ActivateMenus(True)
                        Me.bsAnalyzerStatus.Text = Me.MDIAnalyzerManager.AnalyzerStatus.ToString
                        Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
                        Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString
                        Me.MDIAnalyzerManager.ClearQueueToSend()

                        ' XBC 14/02/2012 - CODEBR Configuration instruction
                        'If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected AndAlso Me.AdjustmentsReaded Then
                        'Me.SEND_CONFIG()
                        'Me.SEND_BARCODE_CONFIG()
                        'System.Threading.Thread.Sleep(100)
                        'Me.SEND_INFO_START() 'SGM 25/11/2011
                        'End If
                        ' XBC 14/02/2012 - CODEBR Configuration instruction

                        ' XBC 30/05/2012
                        MyClass.SetMonitorScreenLimits()
                        ' XBC 30/05/2012

                    Case AnalyzerManagerSwActionList.ANSUTIL_RECEIVED 'SGM 11/10/2012 
                        '
                        ' ANSUTIL RECEIVED
                        '
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then

                            If Not ActiveMdiChild Is Nothing Then

                                'SGM 25/10/2012 - reset Collision Detected Sensor
                                If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                    Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                End If
                                'end SGM 25/10/2012

                                If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                    Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)

                                End If


                            End If

                            'SGM 25/10/2012 - reset Collision Detected Sensor
                            Dim mySensorValuesChangedDT As New UIRefreshDS.SensorValueChangedDataTable
                            mySensorValuesChangedDT = pRefreshDS.SensorValueChanged
                            For Each S As UIRefreshDS.SensorValueChangedRow In mySensorValuesChangedDT.Rows
                                If S.SensorID = AnalyzerSensors.TESTING_NEEDLE_COLLIDED.ToString Then
                                    Dim sensorValue As Single = 0
                                    sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.TESTING_NEEDLE_COLLIDED)
                                    If sensorValue = 1 Then
                                        ScreenWorkingProcess = False
                                        Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.TESTING_NEEDLE_COLLIDED) = 0 'Once updated UI clear sensor
                                    End If
                                    Exit For
                                End If
                            Next
                            'end SGM 25/10/2012

                        End If


                    Case AnalyzerManagerSwActionList.ANSINF_RECEIVED
                        '
                        ' ANSINF RECEIVED
                        '

                        ' XBC 25/10/2012
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then

                            'Dim AnsInf_Alarms As Boolean = False
                            If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then

                                mySpecialSimpleErrors.Clear()
                                Dim lnqRes As List(Of UIRefreshDS.ReceivedAlarmsRow)
                                lnqRes = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                                          Where String.Equals(a.AlarmID, GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR.ToString) _
                                          Select a).ToList

                                If lnqRes.Count > 0 Then
                                    mySpecialSimpleErrors.Add(Messages.WATER_DEPOSIT_ERR.ToString)
                                End If

                                Dim lnqRes2 As List(Of UIRefreshDS.ReceivedAlarmsRow)
                                lnqRes2 = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                                          Where String.Equals(a.AlarmID, GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR.ToString) _
                                          Select a).ToList

                                If lnqRes2.Count > 0 Then
                                    mySpecialSimpleErrors.Add(Messages.WASTE_DEPOSIT_ERR.ToString)
                                End If

                                If mySpecialSimpleErrors.Count > 0 Then
                                    Dim sensorValue As Single = 0
                                    sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE)
                                    If sensorValue > 0 Then

                                        'Dim ManageAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE
                                        ManageAlarmType = CType(sensorValue, GlobalEnumerates.ManagementAlarmTypes)

                                        'Manage
                                        MyClass.ManageAlarmStep1(ManageAlarmType)

                                        'exit from reception
                                        Return True

                                    End If
                                End If

                            End If
                        End If
                        ' XBC 25/10/2012



                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then

                                ' Refresh Monitor 
                                RefreshMonitorPanel(copyRefreshDS.SensorValueChanged)


                                If Not ActiveMdiChild Is Nothing Then
                                    ' Refresh Tank Levels screen
                                    If (TypeOf ActiveMdiChild Is ITankLevelsAdjustments) Then
                                        Dim CurrentMdiChild As ITankLevelsAdjustments = CType(ActiveMdiChild, ITankLevelsAdjustments)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If

                                    '' Refresh Motors, Pumps Valves screen
                                    'If (TypeOf ActiveMdiChild Is MotorsPumpsValvesTest) Then
                                    '    Dim CurrentMdiChild As MotorsPumpsValvesTest = CType(ActiveMdiChild, MotorsPumpsValvesTest)
                                    '    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    'End If

                                    ' Refresh Thermos screen
                                    If (TypeOf ActiveMdiChild Is IThermosAdjustments) Then
                                        Dim CurrentMdiChild As IThermosAdjustments = CType(ActiveMdiChild, IThermosAdjustments)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If

                                    ' XBC 02/11/2011 
                                    ' Commented because activate-deactivate functionality is placed in a common region in SendFwScriptsDelegate
                                    '' Stop INFO Refreshing in Positions Adj Screen
                                    'If (TypeOf ActiveMdiChild Is IPositionsAdjustments) Then
                                    '    If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                    '        Me.SEND_INFO_STOP()
                                    '    End If
                                    'End If

                                    'SGM 17/04/2012
                                    ' Refresh Motors screen (for the moment no POLHW values can be obtained from Fw so some values will be obtained by means of ANSINF)
                                    If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                        Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If

                                    ''SGM 17/09/2012 -  Refresh Analyzer Info screen
                                    'If Not ActiveMdiChild Is Nothing Then
                                    '    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                    '        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                    '        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    '    End If
                                    'End If

                                End If

                            End If
                        End If

                    Case AnalyzerManagerSwActionList.ANSERR_RECEIVED
                        '
                        ' ANSERR RECEIVED
                        '
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then

                            'SGM 19/20/2012 Manage Alarms
                            Dim sensorValue As Single = 0
                            sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE)
                            If sensorValue > 0 Then

                                'Dim ManageAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE
                                ManageAlarmType = CType(sensorValue, GlobalEnumerates.ManagementAlarmTypes)

                                'Manage
                                MyClass.ManageAlarmStep1(ManageAlarmType)

                                'exit from reception
                                Return True

                            End If
                            'end SGM 19/10/2012


                            '' Refresh Stress Mode screen
                            'If Not ActiveMdiChild Is Nothing Then
                            '    If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                            '        Dim CurrentMdiChild As IStressModeTest = CType(ActiveMdiChild, IStressModeTest)
                            '        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            '    End If
                            'End If
                        Else
                            ManageAlarmType = ManagementAlarmTypes.NONE
                        End If

                    Case AnalyzerManagerSwActionList.ANSFCP_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFBX_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFDX_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFRX_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFGL_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFJE_RECEIVED, _
                         AnalyzerManagerSwActionList.ANSFSF_RECEIVED
                        '
                        ' any ANSF## RECEIVED
                        '

                        ' XBC 30/09/2011 - Add AnalyzerID received from the Instrument
                        If MDIAnalyzerManager.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSFCP_RECEIVED Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.FWCPUVALUE_CHANGED) Then

                                ' XBC 11/06/2012 - update information with the values read from the Instrument
                                ''MyClass.GetAnalyzerInfo(MDIAnalyzerManager.ActiveAnalyzer)
                                'AnalyzerModelAttribute = Me.MDIAnalyzerManager.GetModelValue()
                                'AnalyzerIDAttribute = Me.MDIAnalyzerManager.ActiveAnalyzer
                                '' XBC 06/06/2012

                                'MyClass.FwVersionAttribute = MDIAnalyzerManager.ActiveFwVersion

                                ' XBC 08/06/2012 - is no need here because is do it into AnalyzerManager layer
                                'validate versions compatibility
                                'MyClass.MDIAnalyzerManager.ValidateFwSwCompatibility(MyClass.ActiveSwVersion)
                                'If Not MDIAnalyzerManager.IsFwSwCompatible Then
                                '    MyClass.ActivateActionButtonBar(False, True)
                                '    MyClass.ActivateMenus(False, True)
                                'End If

                                If Not MDIAnalyzerManager.IsFwSwCompatible Then

                                    MyClass.IsAutoConnecting = False

                                    MyClass.IsAnalyzerInfoScreenRunning = False 'SGM 30/10/2012

                                    If Me.MdiChildren.Count = 0 Then

                                        'obtain needed fw version SGM 22/06/2012
                                        Dim mySwVersion As String
                                        ''Dim myUtil As New Utilities.
                                        myGlobal = Utilities.GetSoftwareVersion()
                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                            mySwVersion = myGlobal.SetDatos.ToString

                                            myGlobal = MyClass.MDIAnalyzerManager.GetFwVersionNeeded(mySwVersion)
                                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                                Dim myNeededFwVersion As String = CStr(myGlobal.SetDatos)

                                                Dim TextPar As New List(Of String)
                                                TextPar.Add(myNeededFwVersion)

                                                Dim myMsgList As New List(Of String)
                                                myMsgList.Add(Messages.FW_VERSION_NOT_VALID.ToString)
                                                If MyClass.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                                                    myMsgList.Add("")
                                                    myMsgList.Add(Messages.FW_UPDATE_NEEDED.ToString)
                                                End If
                                                ShowMultipleMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), myMsgList, Nothing, TextPar)

                                            End If

                                        End If


                                        'ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), Messages.FW_VERSION_NOT_VALID.ToString)
                                        'ShowMessage(GetMessageText(Messages.SRV_ADJUSTMENTS_TESTS.ToString), GetMessageText(Messages.FW_UPDATE_NEEDED.ToString))

                                        'PROTO5
                                        'All disabled except Instrument Update an exit
                                        MyClass.ActivateActionButtonBar(True)
                                        MyClass.ActivateMenus(True)

                                        'VERSION 1
                                        'Me.OpenInstrumentUpdateToolScreen(True)

                                    Else
                                        ' Refresh Analyzer Info screen
                                        For Each oForm As Form In Me.MdiChildren
                                            If oForm Is AnalyzerInfo Then
                                                Dim CurrentMdiChild As AnalyzerInfo = CType(oForm, AnalyzerInfo)
                                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    Me.bsAnalyzerStatus.Text = MyClass.MDIAnalyzerManager.AnalyzerStatus.ToString

                                    Exit Try

                                End If

                                ' Execute process to update the corresponding Work Session tables  
                                myGlobal = MDIAnalyzerManager.ProcessUpdateWSByAnalyzerID(Nothing)

                                If Not myGlobal.HasError Then
                                    MyClass.AnalyzerIDAttribute = MDIAnalyzerManager.ActiveAnalyzer
                                    MyClass.AnalyzerModelAttribute = MDIAnalyzerManager.GetModelValue(MyClass.AnalyzerIDAttribute)
                                    MyClass.FwVersionAttribute = MDIAnalyzerManager.ActiveFwVersion
                                    MyClass.UpdatePreliminaryHomesMasterData(Nothing, MyClass.AnalyzerIDAttribute)
                                Else
                                    ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
                                End If
                                ' XBC 11/06/2012


                                ' XBC 21/09/2012 - Always when ANSFCP_RECEIVED is received we're inside process connection and it must be continue
                                ' SGM 31/05/2012
                                If Not IsAnalyzerInfoScreenRunning Then
                                    'If (ActiveMdiChild Is Nothing) Then
                                    'If Me.MDIAnalyzerManager.IsUserConnectRequested Then

                                    ' XBC 21/09/2012

                                    If Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then

                                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                                        Dim myText1 As String
                                        Dim myText2 As String
                                        myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SETTING_STANDBY", CurrentLanguageAttribute)
                                        myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STANDBY_WAIT", CurrentLanguageAttribute)
                                        Me.BsMonitor.DisableAllSensors()
                                        Me.isWaitingForConnected = False
                                        Me.UserStandByRequested = True
                                        Me.WaitControl(myText1, myText2)
                                        Me.MDIAnalyzerManager.ClearQueueToSend()
                                        If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                                            Me.SEND_STANDBY()
                                            CreateLogActivity("Send Standby ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Error, False)
                                        End If

                                    ElseIf Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                                        If Not Me.CheckStress Then
                                            'Debug.Print(" - " & Date.Now.ToString & " - SEND SDPOLL ")
                                            Me.CheckStress = True
                                            System.Threading.Thread.Sleep(500)
                                            MyClass.SEND_SDPOLL()
                                            Exit Try
                                        ElseIf Not Me.MDIAnalyzerManager.IsStressing Then
                                            If Not ActiveMdiChild Is Nothing Then
                                                If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                                    Exit Try
                                                End If
                                            End If
                                            Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                            CreateLogActivity("Read Adjustments (1)", Me.Name & ".OnManageReceptionEvent ", EventLogEntryType.Information, False)
                                        End If
                                    End If

                                End If
                                ''SGM 31/05/2012


                            End If
                        End If


                        ' Refresh Analyzer Info screen
                        For Each oForm As Form In Me.MdiChildren
                            If oForm Is AnalyzerInfo Then
                                Dim CurrentMdiChild As AnalyzerInfo = CType(oForm, AnalyzerInfo)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                Exit For
                            End If
                        Next

                        'If Not ActiveMdiChild Is Nothing Then
                        '    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                        '        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                        '        CurrentMdiChild.RefreshScreen(pRefreshEvent, copyRefreshDS)
                        '    End If
                        'End If

                        ' Instrument Update Utility screen
                        If Not ActiveMdiChild Is Nothing Then
                            If (TypeOf ActiveMdiChild Is IInstrumentUpdateUtil) Then
                                Dim CurrentMdiChild As IInstrumentUpdateUtil = CType(ActiveMdiChild, IInstrumentUpdateUtil)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSCPU_RECEIVED
                        '
                        ' ANSCPU RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.CPUVALUE_CHANGED) Then

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If
                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSJEX_RECEIVED
                        '
                        ' ANSJEX_RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.MANIFOLDVALUE_CHANGED) Then

                                ' Refresh Motors Pumps screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                        Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Cycles Count screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                                        Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If

                    Case AnalyzerManagerSwActionList.ANSSFX_RECEIVED
                        '
                        ' ANSSFX_RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.FLUIDICSVALUE_CHANGED) Then

                                ' Refresh Motors Pumps screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                        Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Cycles Count screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                                        Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSGLF_RECEIVED
                        '
                        ' ANSGLF_RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED) Then

                                ' Refresh Motors Pumps screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is IPhotometryAdjustments) Or _
                                    (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then

                                        Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Cycles Count screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                                        Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSBXX_RECEIVED
                        '
                        ' ANSBXX RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ARMSVALUE_CHANGED) Then

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Cycles Count screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                                        Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSDXX_RECEIVED
                        '
                        ' ANSDXX RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.PROBESVALUE_CHANGED) Then

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Refresh Level Detection screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ILevelDetectionTest) Then
                                        Dim CurrentMdiChild As ILevelDetectionTest = CType(ActiveMdiChild, ILevelDetectionTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If


                    Case AnalyzerManagerSwActionList.ANSRXX_RECEIVED
                        '
                        ' ANSRXX RECEIVED
                        '

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORSVALUE_CHANGED) Then

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Cycles Count screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                                        Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                            End If
                        End If

                        'SGM 05/10/2012
                    Case AnalyzerManagerSwActionList.ANSUTIL_RECEIVED

                        If Not bsAnalyzerStatus Is Nothing Then
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then

                                ' Refresh Analyzer Info screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                        Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If

                                ' Refresh Motors screen
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                        Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                    End If
                                End If
                            End If
                        End If


                    Case AnalyzerManagerSwActionList.CYCLES_RECEIVED
                        ''
                        '' CYCLES_RECEIVED Pending to Spec RAUL
                        ''
                        'If Not bsAnalyzerStatus Is Nothing Then
                        '    If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.HWCYCLES_CHANGED) Then

                        '        ' Refresh Analyzer Info screen
                        '        If Not ActiveMdiChild Is Nothing Then
                        '            If (TypeOf ActiveMdiChild Is ICycleCountScreen) Then
                        '                Dim CurrentMdiChild As ICycleCountScreen = CType(ActiveMdiChild, ICycleCountScreen)
                        '                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        '            End If
                        '        End If
                        '    End If
                        'End If


                        ' XBC 16/12/2011
                    Case AnalyzerManagerSwActionList.ANSCBR_RECEIVED
                        '
                        ' ANSCBR_RECEIVED
                        '
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then

                            ' Refresh BarCode Info screen
                            If Not ActiveMdiChild Is Nothing Then
                                If (TypeOf ActiveMdiChild Is IBarCodeAdjustments) Then
                                    Dim CurrentMdiChild As IBarCodeAdjustments = CType(ActiveMdiChild, IBarCodeAdjustments)
                                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                End If
                            End If
                        End If


                        ' XBC 23/01/2012
                    Case AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED
                        '
                        ' ISE RECEIVED
                        '

                        ' Refresh BarCode Info screen
                        If Not ActiveMdiChild Is Nothing Then

                            If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If

                            If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If

                            If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                Dim CurrentMdiChild As IStressModeTest = CType(ActiveMdiChild, IStressModeTest)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            End If



                        Else

                            'if no screen is active
                            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                                Dim sensorValue As Single = 0

                                ' ''ISE switch on changed
                                'sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED)
                                'If sensorValue = 1 Then
                                '    ScreenWorkingProcess = False

                                '    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED) = 0 'Once updated UI clear sensor

                                '    If Me.MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                                '        Me.ActivateActionButtonBar(False)
                                '        Me.ActivateMenus(False)
                                '    End If

                                'End If

                                'ISE initiated
                                sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                                If sensorValue >= 1 Then
                                    ScreenWorkingProcess = False

                                    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                                    Me.ActivateActionButtonBar(True)
                                    Me.ActivateMenus(True)

                                End If

                                'ANSISE received SGM 12/06/2012
                                sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED)
                                If sensorValue = 1 Then
                                    ScreenWorkingProcess = False

                                    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED) = 0 'Once updated UI clear sensor

                                End If

                                'ISE procedure finished
                                sensorValue = Me.MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                                If sensorValue = 1 Then
                                    ScreenWorkingProcess = False

                                    Me.MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor

                                    If Me.MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                                        Me.ActivateActionButtonBar(False)
                                        Me.ActivateMenus(False)
                                    End If
                                End If


                            End If

                        End If
                        'End If


                        ' XBC 26/06/2012 - ISE final self-maintenance
                        If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then
                            If ShutDownisPending Then
                                ShutDownisPending = False
                                If MDIAnalyzerManager.ISE_Manager.LastProcedureResult = ISEManager.ISEProcedureResult.OK Then
                                    ' Continues with Shut Down
                                    Me.bsTSShutdownButton.Enabled = True
                                    Me.bsTSShutdownButton.PerformClick()
                                Else
                                    Me.UserSleepRequested = False
                                    If Me.wfWaitScreen IsNot Nothing Then
                                        Me.wfWaitScreen.Close()
                                    End If
                                    MyClass.isWaitingForSleep = False
                                    Me.ActivateActionButtonBar(True)
                                    Me.ActivateMenus(True)
                                    ShowMessage("Error", GlobalEnumerates.Messages.ISE_CLEAN_WRONG.ToString)
                                    Cursor = Cursors.Default
                                End If
                            End If
                        End If
                        ' XBC 26/06/2012


                        'SGM 29/05/2012
                    Case AnalyzerManagerSwActionList.ANSFWU_RECEIVED
                        '
                        ' ANSFWU RECEIVED
                        '
                        If Not ActiveMdiChild Is Nothing Then

                            If (TypeOf ActiveMdiChild Is IInstrumentUpdateUtil) Then
                                Dim CurrentMdiChild As IInstrumentUpdateUtil = CType(ActiveMdiChild, IInstrumentUpdateUtil)
                                CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                Return True 'not to refresh sensors

                            End If

                        Else

                            MDIAnalyzerManager.FWUpdateResponseData = Nothing

                            MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.FW_UPDATE_UTIL_RECEIVED) = 0 'Once updated UI clear sensor

                        End If


                        ' XBC 27/04/2012
                    Case AnalyzerManagerSwActionList.ANSSDM
                        '
                        ' STRESS MODE ANSWER
                        '
                        Dim openStressScreen As Boolean = False
                        myGlobal = MDIAnalyzerManager.ReadStressModeData
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            Dim MyResultsStressTest As StressDataTO
                            MyResultsStressTest = CType(myGlobal.SetDatos, StressDataTO)
                            If MyResultsStressTest.Status = STRESS_STATUS.UNFINISHED Then

                                If Not ActiveMdiChild Is Nothing Then
                                    If Not (TypeOf ActiveMdiChild Is IStressModeTest) And _
                                        Not (TypeOf ActiveMdiChild Is IDemoMode) Then
                                        'Dim CurrentMdiChild As IStressModeTest = CType(ActiveMdiChild, IStressModeTest)
                                        'CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                                        openStressScreen = True
                                    End If
                                Else
                                    openStressScreen = True
                                End If

                            End If
                        End If

                        If openStressScreen Then
                            ' Refresh Stress Mode screen
                            Me.IsAutoConnecting = False
                            Me.MDIAnalyzerManager.IsStressing = True
                            Me.CheckAnalyzerInfo = True
                            Me.StressTestToolStripMenuItem.PerformClick()
                            Exit Try
                        Else

                            If Not ActiveMdiChild Is Nothing Then
                                If (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                    Exit Try
                                End If
                            End If

                            If Not Me.MDIAnalyzerManager.IsStressing AndAlso Me.MDIAnalyzerManager.IsUserConnectRequested Then
                                Me.CheckAnalyzerInfo = True
                                Me.SEND_READ_ADJUSTMENTS(Ax00Adjustsments.ALL)
                                CreateLogActivity("Read Adjustments (2)", Me.Name & ".OnManageReceptionEvent ", EventLogEntryType.Information, False)
                                Exit Try
                            ElseIf Not Me.MDIAnalyzerManager.IsStressing AndAlso Not Me.CheckAnalyzerInfo Then
                                Me.CheckAnalyzerInfo = True
                                MyClass.SEND_INFO_STOP()
                                OpenAnalyzerInfoScreen()
                                Exit Try
                            End If
                        End If

                End Select


                ' XBC 16/11/2011 - Topmost functionality
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                    'AG 14/10/2011 - TimeOut Communication error message
                    Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)
                    lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                             Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString, False) = 0 _
                             Select a).ToList
                    If lnqRes.Count > 0 Then
                        If lnqRes(0).Value = 0 Then

                            ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens
                            'ShowMessage("Warning", GlobalEnumerates.Messages.ERROR_COMM.ToString)
                            If MyClass.UserSleepRequested Then
                                Me.UserSleepRequested = False
                                If MyClass.isWaitingForSleep Then
                                    'SGM 25/10/2012 - stop current operation when Connection loss
                                    MyClass.ManageConnectionLossAlarm()

                                    MyClass.ShowTimeoutMessage()
                                End If
                            Else
                                'SGM 25/10/2012 - stop current operation when Connection loss
                                MyClass.ManageConnectionLossAlarm()

                                MyClass.ShowTimeoutMessage()

                            End If

                            'SGM 21/11/2012 - reset flag
                            MyClass.MDIAnalyzerManager.IsShutDownRequested = False

                            ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens

                            'SGM 19/04/2012
                            MyClass.ActivateActionButtonBar(True)
                            MyClass.ActivateMenus(True)
                            'SGM 19/04/2012

                        End If
                    End If
                    'AG 14/10/2011 
                End If
                ' XBC 16/11/2011 - Topmost functionality 

            Else
                MDIAnalyzerManager.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.NO_ACTION

            End If


            ' XBC 07/11/2012
            If MDIAnalyzerManager.ErrorCodesDisplay.Count > 0 Then
                Me.bsTSWarningButton.Enabled = True
            Else
                Me.bsTSWarningButton.Enabled = False
            End If

        Catch ex As Exception

            'SGM 16/11/2011
            MyClass.isWaitingForConnected = False
            MyClass.isWaitingForStandBy = False
            MyClass.isWaitingForSleep = False
            MyClass.isWaitingForCloseApp = False

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnManageReceptionEvent", EventLogEntryType.Error, False)
        End Try

        'MDIAnalyzerManager.IsDisplayingServiceData = False 'allows to update again the UIRefreshDS

        Return True
    End Function


    ''' <summary>
    ''' Search in the DB if there is an Active WorkSession for the active Analyzer and if there is one, set
    ''' value of properties ActiveWorkSession and ActiveStatus
    ''' </summary>
    ''' <param name="pGetOnlyWSStateFlag" ></param>
    ''' <remarks>
    ''' Created by:  XB 12/11/2013 - copied from Sw User - BT #169 SERVICE
    ''' </remarks>
    Private Sub SetWSActiveDataFromDB(Optional ByVal pGetOnlyWSStateFlag As Boolean = False)
        Try
            If Not pGetOnlyWSStateFlag Then
                WorkSessionIDAttribute = ""
                WSStatusAttribute = ""
            End If

            Dim dataToReturn As GlobalDataTO
            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate

            dataToReturn = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(Nothing)

            If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
                Dim myWSAnalyzersDS As New WSAnalyzersDS
                myWSAnalyzersDS = DirectCast(dataToReturn.SetDatos, WSAnalyzersDS)

                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                    If Not pGetOnlyWSStateFlag And String.Equals(WorkSessionIDAttribute, String.Empty) Then
                        WorkSessionIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID
                        If Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                            MDIAnalyzerManager.ActiveWorkSession = WorkSessionIDAttribute
                        End If
                    End If

                    WSStatusAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus

                    If (String.Compare(AnalyzerIDAttribute, "", False) = 0) Then
                        AnalyzerIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID
                        AnalyzerModelAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerModel
                        If Not (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                            MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute
                        End If

                    End If
                End If

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
    ''' Clean application log
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 14/11/2013 - BT #171 SERVICE
    ''' </remarks>
    Private Sub CleanApplicationLog()
        Try
            'Dim myGlobalDataTO As New GlobalDataTO
            'Dim mySWParametersDS As New ParametersDS
            'Dim mySWParametersDelegate As New SwParametersDelegate
            'Dim myParameterValue As Single = 0
            'myGlobalDataTO = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, "", SwParameters.SRV_MONTHS_DELETE_LOG.ToString(), False)
            'If Not myGlobalDataTO.HasError Then
            '    mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
            '    If mySWParametersDS.tfmwSwParameters.Count > 0 Then
            '        myParameterValue = mySWParametersDS.tfmwSwParameters(0).ValueNumeric

            '        'Dim myLogAcciones As New ApplicationLogManager()
            '        myGlobalDataTO = myLogAcciones.DeleteByDate(Nothing, myParameterValue.ToString)

            '        If Not myGlobalDataTO.HasError Then
            '            Dim myAffectedRecords As Integer = myGlobalDataTO.AffectedRecords
            '            If myAffectedRecords > 0 Then
            '                'Dim myLogAcciones2 As New ApplicationLogManager()
            '                GlobalBase.CreateLogActivity("Trace Log Records deleted [" & myAffectedRecords.ToString & "]", Name & ".CleanApplicationLog ", EventLogEntryType.Information, False)
            '            End If
            '        End If
            '    End If
            'End If

            ' XB 04/12/2013 - Use the same functionality to Export Logs to XML that is used by User Sw - BT #171 SERVICE
            Dim myGlobalDataTO As New GlobalDataTO
            'Dim myLogAcciones As New ApplicationLogManager()
            Dim myLogMaxDays As Integer = 30
            Dim myParams As New SwParametersDelegate

            myGlobalDataTO = myParams.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_DAYS_IN_PREVIOUSLOG.ToString(), Nothing)
            If Not myGlobalDataTO.HasError Then myLogMaxDays = CInt(myGlobalDataTO.SetDatos)

            myGlobalDataTO = ApplicationLogManager.ExportLogToXml(WorkSessionIDAttribute, myLogMaxDays)
            'If expor to xml OK then delete all records on Application log Table
            If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = ApplicationLogManager.DeleteAll()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CleanApplicationLog ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CleanApplicationLog ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 17/11/2011 - Add more info to solve communications problems with adjustments screens
    ''' </remarks>
    Private Sub ShowTimeoutMessage()
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myTitle As String = ""
            myGlobal.ErrorCode = Messages.ERROR_COMM.ToString
            myTitle = "Warning"
            AutoConnectFailsErrorCode = myGlobal.ErrorCode
            AutoConnectFailsTitle = myTitle
            Me.AdjustmentsReaded = False
            Me.IsAutoConnecting = False ' SGM 18/11/2011
            If String.Compare(AutoConnectFailsTitle, "", False) <> 0 Then

                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim myAdtionalText As String = ""
                For Each oForm As Form In Me.MdiChildren
                    If oForm Is AnalyzerInfo Or _
                       oForm Is IPositionsAdjustments Or _
                       oForm Is IPhotometryAdjustments Or _
                       oForm Is ITankLevelsAdjustments Or _
                       oForm Is IMotorsPumpsValvesTest Or _
                       oForm Is IThermosAdjustments Or _
                       oForm Is IStressModeTest Or _
                       oForm Is IDemoMode Or _
                       oForm Is IInstrumentUpdateUtil Or _
                       oForm Is IChangeRotorSRV Or _
                       oForm Is FwScriptsEdition Then

                        myAdtionalText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_ERR_COMM2", CurrentLanguageAttribute)
                    End If
                Next

                myGlobal = Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, True, Me.NotConnectedText)

                Me.BsMonitor.DisableAllSensors()
                'Me.Cursor = Cursors.Default

                'SGM 16/11/2012
                If MyClass.MDIAnalyzerManager.ISE_Manager IsNot Nothing Then
                    MyClass.MDIAnalyzerManager.ISE_Manager.IsISESwitchON = False
                    MyClass.MDIAnalyzerManager.ISE_Manager.IsISECommsOk = False
                End If
                Me.UserSleepRequested = False
                If Me.wfWaitScreen IsNot Nothing Then
                    Me.wfWaitScreen.Close()
                End If

                MyClass.IsAnalyzerInfoScreenRunning = False
                MyClass.IsAutoConnecting = False

                MyClass.isWaitingForRecover = False
                MyClass.isWaitingForStandBy = False
                MyClass.isWaitingForAdjust = False
                MyClass.isWaitingForConnected = False
                MyClass.isWaitingForCloseApp = False
                MyClass.isWaitingForSleep = False

                'end SGM 16/11/2012

                'ASGM 08/11/2012
                Me.ActivateMenus(True)
                Me.ActivateActionButtonBar(True)

                Me.bsAnalyzerStatus.Text = "NOT CONNECTED"

                CreateLogActivity(AutoConnectFailsTitle & " - ErrorCode: " & AutoConnectFailsErrorCode, Me.Name & ".ShowTimeoutMessage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(AutoConnectFailsTitle, AutoConnectFailsErrorCode, myAdtionalText)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ShowTimeoutMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowTimeoutMessage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by XB 05/11/2014 - BA-1872
    ''' </remarks>
    Private Sub ShowISETimeoutMessage()
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myTitle As String = ""
            myGlobal.ErrorCode = Messages.ERROR_COMM.ToString
            myTitle = "Warning"

            If Me.wfWaitScreen IsNot Nothing Then
                Me.wfWaitScreen.Close()
            End If

            MyClass.IsAnalyzerInfoScreenRunning = False
            MyClass.IsAutoConnecting = False

            MyClass.isWaitingForRecover = False
            MyClass.isWaitingForStandBy = False
            MyClass.isWaitingForAdjust = False
            MyClass.isWaitingForConnected = False
            MyClass.isWaitingForCloseApp = False
            MyClass.isWaitingForSleep = False

            Me.ActivateMenus(True)
            Me.ActivateActionButtonBar(True)

            Dim myAdtionalText As String = ""
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            myAdtionalText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISE_TIMEOUT_ERR", CurrentLanguageAttribute)

            ShowMessage(myTitle, "ERROR_COMM", myAdtionalText)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ShowISETimeoutMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowISETimeoutMessage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pInstructionSent"></param>
    ''' <remarks>
    ''' Created by XB 05/11/2014 - BA-1872
    ''' </remarks>
    Private Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles MDIAnalyzerManager.SendEvent

        Me.UIThread(Function() ManageSentEvent(pInstructionSent))

    End Sub


    ''' <summary>
    ''' Special Event sent from Communications Layer
    ''' </summary>
    ''' <param name="pInstructionSent"></param>
    ''' <remarks>
    ''' Created by XB 05/11/2014 - code copied from the old OnManageSentEvent method and add new code for ISE timeouts - BA-1872
    ''' </remarks>
    Private Function ManageSentEvent(ByVal pInstructionSent As String) As Boolean
        Try
            ' XBC 18/11/2011
            If String.Compare(pInstructionSent, AnalyzerManagerSwActionList.NO_THREADS.ToString, False) = 0 Then

                'SGM 25/10/2012 - stop current operation when Connection loss
                MyClass.ManageConnectionLossAlarm()

                MyClass.ShowTimeoutMessage()

                Exit Function
            End If
            ' XBC 18/11/2011


            ' XB 05/11/2014 - BA-1872
            If String.Compare(pInstructionSent, AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString, False) = 0 Then

                If MDIAnalyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR) Then
                    ' Just for ISE timeout
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

                    MyClass.ShowISETimeoutMessage()

                End If

            End If
            ' XB 05/11/2014 - BA-1872

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & "ManageSentEvent", EventLogEntryType.Error, False)
        End Try
    End Function

    ' XB 05/11/2014 - Replace old OnManageSentEvent method with the new OnManageSentEvent and ManageSentEvent to fix the problems with crossed threads
    ' ''' <summary>
    ' ''' Event to Show Sent Event information into presentation layer
    ' ''' Remarks : If is necessary to implement business logic it must be to implement into ScriptsManagement layer
    ' ''' </summary>
    ' ''' <param name="pInstructionSent"></param>
    ' ''' <remarks>Created by XBC 10/11/2010</remarks>
    'Public Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles MDIAnalyzerManager.SendEvent
    '    Try
    '        ' XBC 16/11/2011 - Topmost functionality 
    '        '' XBC 05/05/2011 - timeout
    '        'If pInstructionSent = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString Then
    '        '    Dim myGlobal As New GlobalDataTO
    '        '    Dim myTitle As String = ""
    '        '    myGlobal.ErrorCode = Messages.ERROR_COMM.ToString
    '        '    myTitle = "Warning"
    '        '    AutoConnectFailsErrorCode = myGlobal.ErrorCode
    '        '    AutoConnectFailsTitle = myTitle
    '        '    Me.AdjustmentsReaded = False

    '        '    'Refresh CONNECTED Led in the Monitor Panel SGM 20/09/2011
    '        '    Dim myConnected As Integer = 0
    '        '    If Me.MDIAnalyzerManager.Connected Then
    '        '        myConnected = 1
    '        '    End If
    '        '    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, myConnected, False, Me.NotConnectedText)
    '        '    'END SGM 20/09/2011

    '        '    If AutoConnectFailsTitle <> "" Then

    '        '        ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens
    '        '        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
    '        '        Dim myAdtionalText As String = ""
    '        '        For Each oForm As Form In Me.MdiChildren
    '        '            If oForm Is AnalyzerInfo Or _
    '        '               oForm Is IPositionsAdjustments Or _
    '        '               oForm Is IPhotometryAdjustments Or _
    '        '               oForm Is ITankLevelsAdjustments Or _
    '        '               oForm Is IMotorsPumpsValvesTest Or _
    '        '               oForm Is IThermosAdjustments Or _
    '        '               oForm Is IStressModeTest Or _
    '        '               oForm Is IDemoMode Or _
    '        '               oForm Is IInstrumentUpdateUtil Or _
    '        '               oForm Is FwScriptsEdition Then

    '        '                myAdtionalText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_SRV_ERR_COMM2", CurrentLanguageAttribute)
    '        '            End If
    '        '        Next
    '        '        ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens

    '        '        CreateLogActivity(AutoConnectFailsTitle & " - ErrorCode: " & AutoConnectFailsErrorCode, Me.Name & ".MDIAnalyzerManager.SendEvent", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        '        ShowMessage(AutoConnectFailsTitle, AutoConnectFailsErrorCode, myAdtionalText)
    '        '    End If
    '        'End If
    '        '' XBC 05/05/2011 - timeout

    '        ''#If DEBUG Then
    '        ''            me.ErrorStatusLabel.Text = pInstructionSent
    '        ''#Else
    '        ''            ' Nothing
    '        ''#End If
    '        ' XBC 16/11/2011 - Topmost functionality 

    '        ' XBC 18/11/2011
    '        If String.Compare(pInstructionSent, AnalyzerManagerSwActionList.NO_THREADS.ToString, False) = 0 Then

    '            'SGM 25/10/2012 - stop current operation when Connection loss
    '            MyClass.ManageConnectionLossAlarm()

    '            MyClass.ShowTimeoutMessage()

    '            Exit Sub
    '        End If
    '        ' XBC 18/11/2011

    '        '            'SGM 25/11/2011
    '        '#If DEBUG Then
    '        '            If Not myConsole Is Nothing Then
    '        '                Me.WriteDebugConsoleTraceLine(pInstructionSent)
    '        '            End If
    '        '#End If
    '        '            'SGM 25/11/2011

    '        'If pInstructionSent = AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString Then
    '        '    'SGM 25/10/2012 - stop current operation when Connection loss
    '        '    MyClass.ManageConnectionLossAlarm()

    '        '    MyClass.ShowTimeoutMessage()
    '        'End If

    '    Catch ex As Exception
    '        'Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & "OnManageSentEvent", EventLogEntryType.Error, False)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Event incharge to show information if Ax00 Is disconected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>CREATE BY: 'TR 20/10/2011 </remarks>
    Private Sub OnDeviceRemoved(ByVal sender As Object, ByVal e As Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetectorEventArgs)
        Try
            'Get the open port.
            Dim myConnectedPort As String = ""
            Dim isConnected As Boolean = False
            If Not MDIAnalyzerManager Is Nothing Then
                isConnected = MDIAnalyzerManager.Connected
                myConnectedPort = MDIAnalyzerManager.PortName
                'Remove special character in case Port Number is Greather than 9 
                myConnectedPort = myConnectedPort.Replace("\\.\", String.Empty)
            End If

            If isConnected AndAlso Not myConnectedPort = String.Empty Then
                Dim MyPortList As New List(Of String)
                MyPortList = IO.Ports.SerialPort.GetPortNames().ToList
                'Search on myPortList if the current port is on the list.
                If MyPortList.Where(Function(a) String.Compare(a, myConnectedPort, False) = 0).Count = 0 Then
                    'AG 24/10/2011
                    ''Comment this line and inform the AnalyzerManager the USB port has been removed
                    'MessageBox.Show(e.Message)

                    'Inform the analyzer manager
                    Dim myGlobal As New GlobalDataTO

                    'action buttons
                    Me.bsTSConnectButton.Enabled = True
                    'Me.bsTSEmergencyButton.Enabled = False
                    'Me.bsTSStandByButton.enabled = False
                    Me.bsTSShutdownButton.Enabled = False

                    myGlobal = MDIAnalyzerManager.ProcessUSBCableDisconnection()
                    If Not myGlobal.HasError Then

                        'SGM 25/10/2012 - stop current operation when Connection loss
                        MyClass.ManageConnectionLossAlarm()

                        ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens
                        'ShowMessage("Warning", GlobalEnumerates.Messages.ERROR_COMM.ToString)
                        MyClass.ShowTimeoutMessage()
                        ' XBC 15/11/2011 - Add more info to solve communications problems with adjustments screens
                    Else
                        ShowMessage("Error", myGlobal.ErrorMessage)
                    End If

                    ' XBC 27/10/2011 
                    'SetActionButtonsEnableProperty(True)

                    If Not ActiveMdiChild Is Nothing Then
                        '- Configuration screen (close screen without save changes)
                        ' The 1st idea was update the ports combo but it was CANCELED due a system error was triggered and it was difficult to solve
                        If (TypeOf ActiveMdiChild Is IConfigGeneral) Then
                            Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                            Dim emptyRefreshDS As UIRefreshDS = Nothing
                            Dim emptyRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)
                            CurrentMdiChild.RefreshScreen(emptyRefreshEventList, emptyRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        End If
                    End If
                    'AG 24/10/2011

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".OnDeviceRemoved ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OnDeviceRemoved ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Send to the instrument the Connect instruction
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: AG 01/06/2010 (Tested ok)
    ''' </remarks>
    Private Sub bsTSConnectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSConnectButton.Click
        Try
            If Not MyClass.FwScriptsLoadedOK Then
                Me.bsTSConnectButton.Enabled = False
                Me.ControlBox = True
                Exit Sub
            End If

            'SGM 27/09/2011
            Me.isWaitingForConnected = True
            Me.ActivateActionButtonBar(False)
            Me.ActivateMenus(False)

            ' XBC 09/02/2012
            If Not ActiveMdiChild Is Nothing Then
                If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                    'Execute the closing code of the screen currently open
                    'Dim myScreenName As String = ActiveMdiChild.Name
                    ActiveMdiChild.AcceptButton.PerformClick()
                End If
            End If

            If ActiveMdiChild Is Nothing Then
                Me.Connect(True)
            End If

            'Me.Connect(True) 'true
            ' XBC 09/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsTSConnectButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTSConnectButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
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
    ''' PDT !!!
    Private Sub bsTSShutdownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSShutdownButton.Click
        Try
            Me.ShutDownAnalyzer()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsTSShutdownButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTSShutdownButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Recover button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 18/10/2012</remarks>
    Private Sub bsTSRecoverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSRecoverButton.Click
        Try
            If Not MDIAnalyzerManager Is Nothing Then

                ' XBC 07/11/2012
                Dim myGlobal As New GlobalDataTO
                myGlobal = MDIAnalyzerManager.RemoveErrorCodesToDisplay()
                If Not myGlobal.HasError Then
                    ' XBC 07/11/2012

                    Cursor = Cursors.WaitCursor
                    'MDIAnalyzerManager.StopAnalyzerRinging()

                    If Not Me.ActiveMdiChild Is Nothing Then
                        Me.ActiveMdiChild.Enabled = False
                    End If

                    Dim myText1 As String
                    Dim myText2 As String
                    Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                    myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "RECOVERING_INSTRUMENT", CurrentLanguageAttribute)
                    myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONNECT_WAIT", CurrentLanguageAttribute)
                    Me.RecoverRequested = True
                    Me.WaitControl(myText1, myText2)
                    Application.DoEvents()

                    RecoverInstrument()

                    ' XBC 07/11/2012
                Else
                    CreateLogActivity("Error when remove Error Codes List", Me.Name & ".bsTSRecoverButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                    ShowMessage(Me.Name & ".bsTSRecoverButton_Click ", Messages.SYSTEM_ERROR.ToString, "Error when remove Error Codes List", Me)
                End If
                ' XBC 07/11/2012

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSRecover_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSRecover_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


    ''' <summary>
    ''' Open Warning error codes screen modal
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 07/11/2012</remarks>
    Private Sub bsTSWarningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSWarningButton.Click
        Try
            myErrCodesForm = New IErrorCodes

            myErrCodesForm.ShowDialog()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsTSWarningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTSWarningButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Activation and deactivation event to be called from outher layer
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>Created by XBC 08/02/2012</remarks>
    Public Sub OnManageActivateScreenEvent(ByVal pEnable As Boolean, ByVal pMessageID As GlobalEnumerates.Messages) Handles myISEUtilities.ActivateScreenEvent
        Try
            MyClass.ActivateActionButtonBar(pEnable)
            MyClass.ActivateMenus(pEnable)

            'If Not pEnable Then
            '    Me.InitializeMarqueeProgreesBar()
            'Else
            '    Me.StopMarqueeProgressBar()
            'End If
            'Application.DoEvents()


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ActivateScreenEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ActivateScreenEvent ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


#End Region

#Region "Monitor Sensors"


#Region "Monitor Sensors Methods"

    ''' <summary>
    ''' Initializes the Monitor Panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SGM 14/04/2011
    ''' </remarks>
    Private Function InitializeMonitorPanel() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        'Dim MLRD As New MultilanguageResourcesDelegate
        Try
            With BsMonitor
                .RowCount = 2
                .ColumnCount = 6
            End With

            Dim myControl As BSMonitorControlBase
            Dim Min As Single
            Dim Max As Single

            'HIGH CONTAMINATION LEVEL
            Dim myHCPercentBar As New BSMonitorPercentBar
            'myGlobal = me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_CONTAMINATED_LIMIT, Min, Max)
            Min = 0
            Max = 100
            If Not myGlobal.HasError Then
                With myHCPercentBar
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_HIGH_CONCENTRATION", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    Dim myColors As New List(Of Color)
                    myColors.Add(Color.Yellow)
                    myColors.Add(Color.LightYellow)
                    myColors.Add(Color.Yellow)
                    .FrontBarColor = myColors
                    .RealValue = 0
                    .MinLimit = Min
                    .MaxLimit = Max
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myHCPercentBar, BSMonitorControlBase)
                BsMonitor.AddControl(myControl, AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE.ToString, 0, 0, True, Min, Max)
            End If

            'WASHING SOLUTION LEVEL
            Dim myWSPercentBar As New BSMonitorPercentBar
            'myGlobal = me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_WASH_SOLUTION_LIMIT, Min, Max)
            Min = 0
            Max = 100
            If Not myGlobal.HasError Then
                With myWSPercentBar
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_WashSolution", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    Dim myColors As New List(Of Color)
                    myColors.Add(Color.LimeGreen)
                    myColors.Add(Color.Honeydew)
                    myColors.Add(Color.LightGreen)
                    .FrontBarColor = myColors
                    .RealValue = 0
                    .MinLimit = Min
                    .MaxLimit = Max
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myWSPercentBar, BSMonitorControlBase)
                BsMonitor.AddControl(myControl, AnalyzerSensors.BOTTLE_WASHSOLUTION.ToString, 0, 1, True, Min, Max)
            End If

            'REACTIONS ROTOR TEMP
            Dim myReactionsTemp As New BSMonitorDigitLabel
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_REACTIONS_ROTOR_TEMP_LIMIT, Min, Max) 'SGM 21/02/2012
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_REACTIONS_LIMIT, Min, Max) ' XBC 30/05/2012

            If Not myGlobal.HasError Then
                With myReactionsTemp
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_REACTIONS_ROTOR_TEMP", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    .DigitSize = 18
                    .DigitValue = 0
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myReactionsTemp, BSMonitorControlBase)

                ' XBC 30/05/2012
                'BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_REACTIONS.ToString, 1, 0, True, Min, Max)
                BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_REACTIONS.ToString, 1, 0, True)
                ' XBC 30/05/2012
            End If

            'FRIDGE TEMP
            Dim myFridgeTemp As New BSMonitorDigitLabel
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_FRIDGE_TEMP_LIMIT, Min, Max) 'SGM 21/02/2012
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_FRIDGE_LIMIT, Min, Max) ' XBC 30/05/2012

            If Not myGlobal.HasError Then
                With myFridgeTemp
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_FRIDGE_TEMP", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    .DigitSize = 18
                    .DigitValue = 0
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myFridgeTemp, BSMonitorControlBase)

                ' XBC 30/05/2012
                'BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_FRIDGE.ToString, 2, 0, True, Min, Max)
                BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_FRIDGE.ToString, 2, 0, True)
                ' XBC 30/05/2012
            End If

            'REAGENTS 1 TEMP
            Dim myReagents1Temp As New BSMonitorDigitLabel

            ' XBC 04/01/2012 - Change sensor limits instead of target limits to display in monitor
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_R1_PROBE_TEMP_LIMIT, Min, Max)
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_THERMO_PROBE_R1_SETPOINT, Min, Max) 
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_ARMS_LIMIT, Min, Max) 'SGM 21/02/2012   ' XBC 30/05/2012
            ' XBC 04/01/2012 - Change sensor limits instead of target limits to display in monitor

            If Not myGlobal.HasError Then
                With myReagents1Temp
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_REAGENT1_TEMP", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    .DigitSize = 18
                    .DigitValue = 0
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myReagents1Temp, BSMonitorControlBase)

                ' XBC 30/05/2012
                'BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_R1.ToString, 3, 0, True, Min, Max)
                BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_R1.ToString, 3, 0, True)
                ' XBC 30/05/2012
            End If

            'REAGENTS 2 TEMP
            Dim myReagents2Temp As New BSMonitorDigitLabel

            ' XBC 04/01/2012 - Change sensor limits instead of target limits to display in monitor
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_R2_PROBE_TEMP_LIMIT, Min, Max)
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_THERMO_PROBE_R2_SETPOINT, Min, Max)
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_ARMS_LIMIT, Min, Max) 'SGM 21/02/2012   ' XBC 30/05/2012
            ' XBC 04/01/2012 - Change sensor limits instead of target limits to display in monitor

            If Not myGlobal.HasError Then
                With myReagents2Temp
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_REAGENT2_TEMP", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    .DigitSize = 18
                    .DigitValue = 0
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myReagents2Temp, BSMonitorControlBase)

                ' XBC 30/05/2012
                'BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_R2.ToString, 4, 0, True, Min, Max)
                BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_R2.ToString, 4, 0, True)
                ' XBC 30/05/2012
            End If

            'WASHING STATION HEATER TEMP
            Dim myWSHeaterTemp As New BSMonitorDigitLabel
            ' myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.SRV_WS_HEATER_TEMP_LIMIT, Min, Max) 'SGM 21/02/2012
            'myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_WASHSTATION_LIMIT, Min, Max)    ' XBC 30/05/2012

            If Not myGlobal.HasError Then
                With myWSHeaterTemp
                    '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_WS_HEATER_TEMP", CurrentLanguageAttribute)
                    'If .TitleText.Length = 0 Then .TitleText = " "
                    .DigitSize = 18
                    .DigitValue = 0
                    .CurrentStatus = BSMonitorControlBase.Status.DISABLED
                End With
                myControl = CType(myWSHeaterTemp, BSMonitorControlBase)

                ' XBC 30/05/2012
                'BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString, 5, 0, True, Min, Max)
                BsMonitor.AddControl(myControl, AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString, 5, 0, True)
                ' XBC 30/05/2012
            End If

            'GENERAL COVER
            Dim myGeneralCover As New BSMonitorLED
            With myGeneralCover
                '.TitleText = MLRD.GetResourceText(Nothing, "MAIN_COVER_WARN", CurrentLanguageAttribute)
                'If .TitleText.Length = 0 Then .TitleText = " "
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With
            myControl = CType(myGeneralCover, BSMonitorControlBase)
            BsMonitor.AddControl(myControl, AnalyzerSensors.COVER_GENERAL.ToString, 1, 1)

            'REAGENTS COVER
            Dim myReagentsCover As New BSMonitorLED
            With myReagentsCover
                '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_REAGENTS_COVER", CurrentLanguageAttribute)
                'If .TitleText.Length = 0 Then .TitleText = " "
                '.CurrentStatus = BSMonitorControlBase.Status._ON
            End With
            myControl = CType(myReagentsCover, BSMonitorControlBase)
            BsMonitor.AddControl(myControl, AnalyzerSensors.COVER_FRIDGE.ToString, 2, 1)

            'SAMPLES COVER
            Dim mySamplesCover As New BSMonitorLED
            With mySamplesCover
                '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_SAMPLE_COVER", CurrentLanguageAttribute)
                'If .TitleText.Length = 0 Then .TitleText = " "
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With
            myControl = CType(mySamplesCover, BSMonitorControlBase)
            BsMonitor.AddControl(myControl, AnalyzerSensors.COVER_SAMPLES.ToString, 3, 1)

            'REACTIONS COVER
            Dim myReactionsCover As New BSMonitorLED
            With myReactionsCover
                '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_REACTIONS_COVER", CurrentLanguageAttribute)
                'If .TitleText.Length = 0 Then .TitleText = " "
                .CurrentStatus = BSMonitorControlBase.Status.DISABLED
            End With
            myControl = CType(myReactionsCover, BSMonitorControlBase)
            BsMonitor.AddControl(myControl, AnalyzerSensors.COVER_REACTIONS.ToString, 4, 1)

            'CONNECTED
            Dim myConnected As New BSMonitorLED
            With myConnected
                '.TitleText = MLRD.GetResourceText(Nothing, "LBL_SRV_NOT_CONNECTED", CurrentLanguageAttribute)
                'If .TitleText.Length = 0 Then .TitleText = " "
                .CurrentStatus = BSMonitorControlBase.Status._OFF
                .Enabled = True
            End With
            myControl = CType(myConnected, BSMonitorControlBase)
            BsMonitor.AddControl(myControl, AnalyzerSensors.CONNECTED.ToString, 5, 1)

            Application.DoEvents()

            Me.BsMonitor.DisableAllSensors()

            Me.BsMonitor.Visible = True

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " InitializeMonitor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    Private Sub GetMonitorScreenLabels()
        Dim MLRD As New MultilanguageResourcesDelegate
        Try
            With Me.BsMonitor
                .SetSensorText(AnalyzerSensors.BOTTLE_WASHSOLUTION.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_WashSolution"))
                .SetSensorText(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_HIGH_CONCENTRATION"))
                .SetSensorText(AnalyzerSensors.TEMPERATURE_REACTIONS.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_REACTIONS_ROTOR_TEMP"))
                .SetSensorText(AnalyzerSensors.TEMPERATURE_FRIDGE.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_FRIDGE_TEMP"))
                .SetSensorText(AnalyzerSensors.TEMPERATURE_R1.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_REAGENT1_TEMP"))
                .SetSensorText(AnalyzerSensors.TEMPERATURE_R2.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_REAGENT2_TEMP"))
                .SetSensorText(AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_WS_HEATER_TEMP"))
                .SetSensorText(AnalyzerSensors.COVER_GENERAL.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_GeneralCover_Short"))
                .SetSensorText(AnalyzerSensors.COVER_FRIDGE.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_REAGENTS_COVER"))
                .SetSensorText(AnalyzerSensors.COVER_SAMPLES.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_SAMPLE_COVER"))
                .SetSensorText(AnalyzerSensors.COVER_REACTIONS.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_REACTIONS_COVER"))
                If MyClass.MDIAnalyzerManager.Connected Then
                    .SetSensorText(AnalyzerSensors.CONNECTED.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_CONNECTED"))
                Else
                    .SetSensorText(AnalyzerSensors.CONNECTED.ToString, MultilanguageResourcesDelegate.GetResourceText("LBL_SRV_NOT_CONNECTED"))
                End If
            End With

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetMonitorScreenLabels", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Limit ranges must be assigned after adjustments have been read
    ''' </summary>
    ''' <remarks>Created by XBC 30/05/2012</remarks>
    Private Sub SetMonitorScreenLimits()
        Dim myGlobal As New GlobalDataTO
        Dim myTarget As Single = 0
        Dim Min As Single
        Dim Max As Single
        Try
            With Me.BsMonitor
                myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_REACTIONS_LIMIT, Min, Max)
                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY, AXIS.TARGET)
                Min = Min + myTarget
                Max = Max + myTarget
                .SetSensorLimits(AnalyzerSensors.TEMPERATURE_REACTIONS.ToString, Min, Max)

                myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_FRIDGE_LIMIT, Min, Max)
                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_FRIDGE, AXIS.TARGET)
                Min = Min + myTarget
                Max = Max + myTarget
                .SetSensorLimits(AnalyzerSensors.TEMPERATURE_FRIDGE.ToString, Min, Max)

                myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_ARMS_LIMIT, Min, Max)
                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT1, AXIS.TARGET)
                Min = Min + myTarget
                Max = Max + myTarget
                .SetSensorLimits(AnalyzerSensors.TEMPERATURE_R1.ToString, Min, Max)

                myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_ARMS_LIMIT, Min, Max)
                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT2, AXIS.TARGET)
                Min = Min + myTarget
                Max = Max + myTarget
                .SetSensorLimits(AnalyzerSensors.TEMPERATURE_R2.ToString, Min, Max)

                myGlobal = Me.GetMonitorLimitValues(GlobalEnumerates.FieldLimitsEnum.THERMO_WASHSTATION_LIMIT, Min, Max)
                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_WS_HEATER, AXIS.TARGET)
                Min = Min + myTarget
                Max = Max + myTarget
                .SetSensorLimits(AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString, Min, Max)
            End With

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SetMonitorScreenLimits", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for every monitor sensor
    ''' </summary>
    ''' <remarks>Created by SGM 14/04/11</remarks>
    Private Function GetMonitorLimitValues(ByRef pLimitID As FieldLimitsEnum, ByRef pMin As Single, ByRef pMax As Single) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            pMin = 0
            pMax = 0

            ' Get Value limit ranges
            Dim myFieldLimitsDS As New FieldLimitsDS
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the specified limits
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitID)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
                If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                    pMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Single)
                    pMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Single)
                End If
            End If

        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".GetMonitorLimitValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetMonitorLimitValues ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    Private Sub BsInfoTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsInfoTimer.Tick

        Dim myGlobal As New GlobalDataTO
        Dim continousSimulation As Boolean

        Try
            continousSimulation = BsInfoTimer.Enabled
            BsInfoTimer.Enabled = False

            If Me.SimulationMode Then
                If Not ActiveMdiChild Is Nothing Then
                    If (TypeOf ActiveMdiChild Is ITankLevelsAdjustments) Then
                        Me.SimulateINFO_Off()
                        continousSimulation = False
                    End If
                End If



                myGlobal = Me.ANSINF_Generator()

                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then

                    RefreshMonitorPanel(MDIAnalyzerManager.SensorValueChanged)

                    Dim myRefreshEvent As New List(Of GlobalEnumerates.UI_RefreshEvents)
                    myRefreshEvent.Add(UI_RefreshEvents.SENSORVALUE_CHANGED)

                    Dim mySimRefreshDS As UIRefreshDS

                    mySimRefreshDS = CType(myGlobal.SetDatos, UIRefreshDS)

                    If mySimRefreshDS IsNot Nothing Then
                        If Not ActiveMdiChild Is Nothing Then

                            ' Refresh Tank Levels screen
                            If (TypeOf ActiveMdiChild Is ITankLevelsAdjustments) Then
                                Dim CurrentMdiChild As ITankLevelsAdjustments = CType(ActiveMdiChild, ITankLevelsAdjustments)
                                CurrentMdiChild.RefreshScreen(myRefreshEvent, mySimRefreshDS)
                            End If
                            ' Refresh Motors, Pumps Valves screen
                            If (TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest) Then
                                Dim CurrentMdiChild As IMotorsPumpsValvesTest = CType(ActiveMdiChild, IMotorsPumpsValvesTest)
                                CurrentMdiChild.RefreshScreen(myRefreshEvent, mySimRefreshDS)
                            End If
                            ' Refresh Thermos screen
                            If (TypeOf ActiveMdiChild Is IThermosAdjustments) Then
                                Dim CurrentMdiChild As IThermosAdjustments = CType(ActiveMdiChild, IThermosAdjustments)
                                CurrentMdiChild.RefreshScreen(myRefreshEvent, mySimRefreshDS)
                            End If
                        End If
                    End If
                End If

                If continousSimulation Then
                    BsInfoTimer.Enabled = True
                End If

            End If




        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & " BsInfoTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub SimulateINFO_On()
        Try
            'enable Request Timer
            BsInfoTimer.Enabled = True

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " SimulateINFO_On ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub SimulateINFO_Off()
        Try
            'disable Request Timer
            BsInfoTimer.Enabled = False

            If Me.SimulationMode Then
                Me.BsMonitor.DisableAllSensors()

                Dim myConnected As Integer = 0
                Dim myConnectedText As String = Me.NotConnectedText
                If Me.MDIAnalyzerManager.Connected Then
                    myConnected = 1
                    myConnectedText = Me.ConnectedText
                End If
                Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, myConnected, False, myConnectedText)
                Me.bsAnalyzerStatus.Text = myConnectedText.ToUpper
                'Me.BsMonitor.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " SimulateINFO_Off ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Monitor Sensors Events Handlers"

    ''' <summary>
    ''' Refresh Monitor Panel with the information received from the Instrument
    ''' </summary>
    ''' <remarks>Created by XBC 17/05/2011</remarks>
    Private Sub RefreshMonitorPanel(ByVal pSensorValueChanged As UIRefreshDS.SensorValueChangedDataTable)

        Dim myGlobal As New GlobalDataTO
        Dim mySensorValuesChangedDT As New UIRefreshDS.SensorValueChangedDataTable

        Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

        Dim min As Single
        Dim max As Single
        Dim myPercentValue As Single

        Dim myTarget As Single
        Dim mySetpoint As Single
        Dim myOffset As Single
        Dim myTempValue As Single

        ''Dim myUtil As New Utilities.
        Try
            'Me.BsMonitor.DisableAllSensors()

            If pSensorValueChanged IsNot Nothing Then
                IsRefreshing = True

                mySensorValuesChangedDT = pSensorValueChanged
                'mySensorValuesChangedDT = MDIAnalyzerManager.SensorValueChanged SGM 15/11/2011


                For Each S As UIRefreshDS.SensorValueChangedRow In mySensorValuesChangedDT.Rows

                    If Me.AdjustmentsReaded Then
                        If S.SensorID = AnalyzerSensors.BOTTLE_WASHSOLUTION.ToString Then
                            If Me.SimulationMode Then
                                min = 0
                                max = 4096
                            Else
                                min = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.EMPTY)
                                max = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.WASHING_SOLUTION, AXIS.FULL)
                            End If

                            myGlobal = Utilities.CalculatePercent(S.Value, min, max)
                            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                myPercentValue = CType(myGlobal.SetDatos, Single)

                                myPercentValue = CInt(myPercentValue)
                                If myPercentValue > 100 Then myPercentValue = 100
                                If myPercentValue < 0 Then myPercentValue = 0

                                myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myPercentValue, True)
                            End If

                        ElseIf S.SensorID = AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE.ToString Then
                            If Me.SimulationMode Then
                                min = 0
                                max = 4096
                            Else
                                min = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.EMPTY)
                                max = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION, AXIS.FULL)
                            End If

                            myGlobal = Utilities.CalculatePercent(S.Value, min, max)
                            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                myPercentValue = CType(myGlobal.SetDatos, Single)

                                myPercentValue = CInt(myPercentValue)
                                If myPercentValue > 100 Then myPercentValue = 100
                                If myPercentValue < 0 Then myPercentValue = 0

                                myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myPercentValue, True)
                            End If

                            'SGM 19/02/2012
                        ElseIf S.SensorID = AnalyzerSensors.TEMPERATURE_FRIDGE.ToString Then
                            If Me.SimulationMode Then
                                mySetpoint = 5
                                myOffset = 0
                            Else
                                mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_FRIDGE, AXIS.SETPOINT)
                                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_FRIDGE, AXIS.TARGET)
                            End If
                            myTempValue = Utilities.MakeSensorValueCorrection(S.Value, mySetpoint, myTarget)
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myTempValue, True)

                        ElseIf S.SensorID = AnalyzerSensors.TEMPERATURE_REACTIONS.ToString Then
                            If Me.SimulationMode Then
                                mySetpoint = 37
                                myOffset = 0
                            Else
                                ' XBC 07/05/2012
                                'mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.PHOTOMETRY, AXIS.SETPOINT)
                                'myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.PHOTOMETRY, AXIS.TARGET)
                                mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY, AXIS.SETPOINT)
                                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY, AXIS.TARGET)
                                ' XBC 07/05/2012
                            End If
                            myTempValue = Utilities.MakeSensorValueCorrection(S.Value, mySetpoint, myTarget)
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myTempValue, True)

                        ElseIf S.SensorID = AnalyzerSensors.TEMPERATURE_R1.ToString Then
                            If Me.SimulationMode Then
                                mySetpoint = 45
                                myOffset = 0
                            Else
                                mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT1, AXIS.SETPOINT)
                                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT1, AXIS.TARGET)
                            End If
                            myTempValue = Utilities.MakeSensorValueCorrection(S.Value, mySetpoint, myTarget)
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myTempValue, True)

                        ElseIf S.SensorID = AnalyzerSensors.TEMPERATURE_R2.ToString Then
                            If Me.SimulationMode Then
                                mySetpoint = 45
                                myOffset = 0
                            Else
                                mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT2, AXIS.SETPOINT)
                                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_REAGENT2, AXIS.TARGET)
                            End If
                            myTempValue = Utilities.MakeSensorValueCorrection(S.Value, mySetpoint, myTarget)
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myTempValue, True)

                        ElseIf String.Compare(S.SensorID, AnalyzerSensors.TEMPERATURE_WASHINGSTATION.ToString, False) = 0 Then
                            If Me.SimulationMode Then
                                mySetpoint = 45
                                myOffset = 0
                            Else
                                mySetpoint = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_WS_HEATER, AXIS.SETPOINT)
                                myTarget = MyClass.ReadSensorAdjustmentValue(ADJUSTMENT_GROUPS.THERMOS_WS_HEATER, AXIS.TARGET)
                            End If
                            myTempValue = Utilities.MakeSensorValueCorrection(S.Value, mySetpoint, myTarget)
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, myTempValue, True)

                            '    'end SGM 19/02/2012
                        Else
                            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, S.Value, True)
                        End If
                    ElseIf String.Compare(S.SensorID, AnalyzerSensors.CONNECTED.ToString, False) = 0 Then
                        Dim MLRD As New MultilanguageResourcesDelegate
                        Dim myConectedTitle As String = ""
                        If S.Value > 0 Then
                            myConectedTitle = Me.ConnectedText
                        Else
                            myConectedTitle = Me.NotConnectedText
                        End If
                        myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, S.Value, True, myConectedTitle)
                    End If

                Next

                Me.BsMonitor.EnableAllSensors()

                IsRefreshing = False

            End If
            'Me.BsMonitor.Enabled = True
        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshMonitorPanel", EventLogEntryType.Error, False)
        End Try
    End Sub

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="pSensorValuesChangedDT"></param>
    '''' <remarks>Created by SGM 15/04/2011</remarks>
    'Private Sub OnSensorValuesChanged(ByVal pSensorValuesChangedDT As UIRefreshDS.SensorValueChangedDataTable) Handles MDIAnalyzerManager.SensorValuesChangedEvent

    '    Dim myGlobal As New GlobalDataTO

    '    Try
    '        IsRefreshing = True
    '        For Each S As UIRefreshDS.SensorValueChangedRow In pSensorValuesChangedDT.Rows
    '            myGlobal = Me.BsMonitor.RefreshSensorValue(S.SensorID, S.Value)
    '        Next


    '        IsRefreshing = False

    '        Me.BsMonitor.Enabled = True

    '    Catch ex As Exception
    '        'Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, "SendFwScriptsDelegate.OnSensorValuesChanged", EventLogEntryType.Error, False)
    '    End Try
    'End Sub
    ' XBC 17/05/2011

#End Region


#End Region

#Region "Public Methods"

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send WASH instructions
    ''' </summary>
    ''' <remarks>Created by SGM 05/07/2011</remarks>
    Public Function SEND_WASH() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_WASH ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_WASH", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send STANDBY instructions
    ''' </summary>
    ''' <remarks>Created by SGM 05/07/2011</remarks>
    Public Function SEND_SLEEP() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                Me.isWaitingForSleep = True 'SGM 15/11/2011
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SLEEP, True)
                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            Me.isWaitingForSleep = False

            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_SLEEP ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_SLEEP", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send STANDBY instructions
    ''' </summary>
    ''' <remarks>Created by XBC 25/05/2011</remarks>
    Public Function SEND_STANDBY() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                Me.isWaitingForStandBy = True 'SGM 15/11/2011
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            Me.UserStandByRequested = False
            Me.isWaitingForStandBy = False

            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_STANDBY ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_STANDBY ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send RESET instructions
    ''' </summary>
    ''' <remarks>Created by SGM 01/07/2011</remarks>
    Public Function SEND_RESET() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.RESET_ANALYZER, True)
                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_RESET ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_RESET ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send READ ADJUSTMENTS instruction
    ''' </summary>
    ''' <remarks>Created by XBC 30/09/2011</remarks>
    Public Function SEND_READ_ADJUSTMENTS(ByVal pQueryMode As GlobalEnumerates.Ax00Adjustsments) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                MyClass.SEND_INFO_STOP()    ' XBC 17/11/2011
                MyClass.isWaitingForAdjust = True 'SGM 15/11/2011
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.READADJ, True, Nothing, pQueryMode)
                System.Threading.Thread.Sleep(100)
            End If

        Catch ex As Exception
            MyClass.isWaitingForAdjust = False

            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_READ_ADJUSTMENTS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_READ_ADJUSTMENTS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send ISECMD instruction
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 12/12/2011</remarks>
    ''' 
    Public Function SEND_ISE_COMMAND(ByVal pISECommand As ISECommandTO) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                MyClass.SEND_INFO_STOP()    ' XBC 17/11/2011

                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)

                myGlobal = MDIAnalyzerManager.ISE_Manager.SendISECommand
                'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, pISECommand)
                'System.Threading.Thread.Sleep(100)

                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_ISE_COMMAND ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_ISE_COMMAND ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send Start Info Refreshing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/09/2011</remarks>
    Public Function SEND_INFO_START(Optional ByVal pDontEnableMenusAndButtons As Boolean = False) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If MyClass.MDIAnalyzerManager.IsAutoInfoActivated Then Return myGlobal 'SGM 22/11/2011

            If Me.SimulationMode Then

                Me.SimulateINFO_On()

            Else
                If Me.MDIAnalyzerManager.Connected Then

                    Me.ActivateActionButtonBar(False)
                    Me.ActivateMenus(False)

                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                         True, _
                                                         Nothing, _
                                                         GlobalEnumerates.Ax00InfoInstructionModes.STR)

                    If Not myGlobal.HasError AndAlso Not pDontEnableMenusAndButtons Then
                        Me.ActivateActionButtonBar(True)
                        Me.ActivateMenus(True)
                    End If


                End If
            End If

            'SGM 22/11/2011
            If Not myGlobal.HasError Then
                MyClass.MDIAnalyzerManager.IsAutoInfoActivated = True
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_INFO_START( ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_INFO_START ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send Stop Info Refreshing Mode
    ''' </summary>
    ''' <remarks>Created by SGM 23/09/2011</remarks>
    Public Function SEND_INFO_STOP(Optional ByVal pDontEnableMenusAndButtons As Boolean = False) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If Not MyClass.MDIAnalyzerManager.IsAutoInfoActivated Then Return myGlobal 'SGM 22/11/2011

            If Me.SimulationMode Then

                Me.SimulateINFO_Off()

            Else
                If Me.MDIAnalyzerManager.Connected Then

                    Me.ActivateActionButtonBar(False)
                    Me.ActivateMenus(False)

                    myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                                                         True, _
                                                         Nothing, _
                                                         GlobalEnumerates.Ax00InfoInstructionModes.STP)

                    If Not myGlobal.HasError AndAlso Not pDontEnableMenusAndButtons Then
                        Me.ActivateActionButtonBar(True)
                        Me.ActivateMenus(True)
                    End If
                End If
            End If

            'SGM 22/11/2011
            If Not myGlobal.HasError Then
                MyClass.MDIAnalyzerManager.IsAutoInfoActivated = False
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_INFO_STOP ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_INFO_STOP ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' public procedure to be accesible from diferent parts of the application to send POLLFW CPU Refreshing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 03/10/2011</remarks>
    Public Function SEND_POLLFW(ByVal ID As POLL_IDs) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            If Not MDIAnalyzerManager Is Nothing Then

                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)

                If ID = POLL_IDs.CPU Then
                    myGlobal = MyClass.SEND_INFO_STOP
                    Application.DoEvents()
                End If

                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLFW, _
                                                     True, _
                                                     Nothing, _
                                                     ID)

                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_POLLFW( ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_POLLFW ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function





    ''' <summary>
    ''' Writes the Configuration obtained from DDBB to the Analyzer for Codebars
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 14/02/2012</remarks>
    Public Function SEND_BARCODE_CONFIG() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If Me.MDIAnalyzerManager.Connected Then

                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)

                Dim BarCodeDS As New AnalyzerManagerDS
                Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
                rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                With rowBarCode
                    .RotorType = "SAMPLES"
                    .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                    .Position = 0
                End With
                BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                BarCodeDS.AcceptChanges()
                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)

                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_BARCODE_CONFIG ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_BARCODE_CONFIG ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ''' <summary>
    ''' Send High Level Stress READ Instruction 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 27/04/2012</remarks>
    Public Function SEND_SDPOLL() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If Me.MDIAnalyzerManager.Connected Then

                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)

                myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.SDPOLL, True)

                If Not myGlobal.HasError Then
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message, Me.Name & ".SEND_SDPOLL ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SEND_SDPOLL ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function




    ''' <summary>
    ''' To be able to be opened from another screen, such as AnalyzerInfo (StartUp)
    ''' </summary>
    ''' <remarks>Created by XBC 15/07/2011</remarks>
    Public Sub OpenInstrumentUpdateToolScreen(Optional ByVal pForceToFirmwareUpdate As Boolean = False)
        Try
            ' ''DELETE
            'If Not Me.SimulationMode Then
            '    Dim MyFormP As New FormXavi
            '    MyFormP.Show()
            '    MyFormP.Location = New Point(1000, 100)
            '    Application.DoEvents()
            '    Me.Location = New Point(100, 100)
            'End If
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            Dim myInstrumUpdateForm As New IInstrumentUpdateUtil(pForceToFirmwareUpdate)
            OpenMDIChildForm(myInstrumUpdateForm)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".OpenInstrumentUpdateToolScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Activates or deactivates the menu bar
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' Created by SGM 27/09/2011
    ''' </remarks>
    Public Sub ActivateMenus(ByVal pEnable As Boolean, _
                             Optional ByVal pDisableForced As Boolean = False, _
                             Optional ByVal pFwUpdated As Boolean = False)
        Dim isconnected As Boolean
        Try
            'SGM 31/05/2012
            If Me.SimulationMode Then
                If String.Compare(Me.bsAnalyzerStatus.Text, "NOT CONNECTED", False) <> 0 Then
                    isconnected = True
                End If
            Else
                If Me.MDIAnalyzerManager IsNot Nothing Then
                    If Me.MDIAnalyzerManager.Connected Then
                        isconnected = True
                    End If
                End If
            End If
            'end SGM 31/05/2012

            'SGM 02/02/2012
            'if the FW Scripts and Adjustments masterdata were not correctly loaded set as disabled
            pEnable = pEnable And MyClass.FwScriptsLoadedOKAttr And MyClass.FwAdjustmentsMasterDataLoadedOKAttr
            'end SGM 02/02/2012
            If Not pFwUpdated Then
                If MDIAnalyzerManager.IsFwSwCompatible AndAlso Not MyClass.IsAnalyzerInitiated Then pDisableForced = True
            End If
            'If (isconnected And Not MDIAnalyzerManager.IsFwSwCompatible) Then pDisableForced = True 'SGM 30/05/2012


            If ManageAlarmType <> ManagementAlarmTypes.NONE Then pDisableForced = False ' XBC 24/10/2012

            If pDisableForced Then pEnable = False 'SGM 16/04/2012

            ' XBC 22/05/2012
            If Not ActiveMdiChild Is Nothing Then
                If (TypeOf ActiveMdiChild Is IPositionsAdjustments) Then
                    Dim CurrentMdiChild As IPositionsAdjustments = CType(ActiveMdiChild, IPositionsAdjustments)
                    If CurrentMdiChild.IsCenteringOptic Then
                        pEnable = False
                    End If
                End If
            End If
            ' XBC 22/05/2012

            If Not pEnable Then

                Me.ConfigurationToolStripMenuItem.Enabled = False
                Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                Me.UtilitiesToolStripMenuItem.Enabled = False
                Me.RegistryMaintenanceToolStripMenuItem.Enabled = False
                Me.ExitToolStripMenuItem.Enabled = False
                Me.HelpToolStripMenuItem.Enabled = False
                Me.ProductionToolStripMenuItem.Enabled = False



            ElseIf Not MyClass.IsWaitingForNewStatus And Not MyClass.IsAutoConnecting Then

                If Me.SimulationMode Then
                    Me.ConfigurationToolStripMenuItem.Enabled = pEnable
                    Me.AdjustmentsTestsToolStripMenuItem.Enabled = pEnable
                    Me.UtilitiesToolStripMenuItem.Enabled = pEnable
                    Me.RegistryMaintenanceToolStripMenuItem.Enabled = pEnable
                    Me.ExitToolStripMenuItem.Enabled = pEnable
                    Me.HelpToolStripMenuItem.Enabled = pEnable
                    Me.ProductionToolStripMenuItem.Enabled = pEnable

                Else
                    If Me.MDIAnalyzerManager IsNot Nothing Then

                        Dim isSleeping As Boolean = (Me.MDIAnalyzerManager.Connected AndAlso Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING)
                        Dim isStandby As Boolean = (Me.MDIAnalyzerManager.Connected AndAlso Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY)

                        'Enable only functionality not related to Firmware SGM 30/05/2012
                        ' If Not MDIAnalyzerManager.IsFwSwCompatible And MDIAnalyzerManager.IsFwReaded  ' XBC 18/09/2012 - add Update Fw functionality flag 
                        If Not MDIAnalyzerManager.IsFwSwCompatible And MDIAnalyzerManager.IsFwReaded _
                           Or Me.UpdateFirmwareProcessEnd Then

                            'FIRWARE PENDING TO UPDATE
                            Me.ConfigurationToolStripMenuItem.Enabled = True
                            Me.GeneralToolStripMenuItem.Enabled = False
                            Me.LanguageToolStripMenuItem.Enabled = True
                            Me.BarcodesConfigToolStripMenuItem.Enabled = False
                            Me.UserToolStripMenuItem.Enabled = True
                            Me.ChangeUserToolStripMenuItem.Enabled = True

                            Me.UtilitiesToolStripMenuItem.Enabled = (MyClass.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) ' True
                            Me.ConditioningToolStripMenuItem.Enabled = False
                            Me.DemoModeToolStripMenuItem.Enabled = False
                            Me.AnalyzerInfoToolStripMenuItem.Enabled = False
                            Me.InstrumentUpdateToolStripMenuItem.Enabled = (MyClass.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) ' True
                            Me.SendableScriptsToolStripMenuItem.Enabled = False
                            Me.SettingsToolStripMenuItem.Enabled = True
                            Me.ChangeRotorToolStripMenuItem.Enabled = False

                            Me.HelpToolStripMenuItem.Enabled = True

                            Me.ExitToolStripMenuItem.Enabled = True
                            Me.WithShutToolStripMenuItem.Enabled = False
                            Me.WithOutShutDownToolStripMenuItem.Enabled = True

                            Me.Cursor = Cursors.Default
                            Me.ControlBox = True

                            Exit Sub


                        Else


                            Me.WithShutToolStripMenuItem.Enabled = isStandby
                            Me.ChangeRotorToolStripMenuItem.Enabled = isStandby

                            ' XBC 09/02/2012
                            If isconnected Then
                                Me.ConfigurationToolStripMenuItem.Enabled = pEnable
                                Me.GeneralToolStripMenuItem.Enabled = pEnable
                                Me.BarcodesConfigToolStripMenuItem.Enabled = pEnable
                                Me.AdjustmentsTestsToolStripMenuItem.Enabled = pEnable
                                Me.UtilitiesToolStripMenuItem.Enabled = pEnable
                                Me.RegistryMaintenanceToolStripMenuItem.Enabled = pEnable
                                Me.ExitToolStripMenuItem.Enabled = pEnable
                                Me.HelpToolStripMenuItem.Enabled = pEnable
                                Me.ProductionToolStripMenuItem.Enabled = pEnable
                            Else
                                Me.ConfigurationToolStripMenuItem.Enabled = pEnable
                                Me.GeneralToolStripMenuItem.Enabled = False
                                Me.BarcodesConfigToolStripMenuItem.Enabled = False
                                Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                                Me.RegistryMaintenanceToolStripMenuItem.Enabled = pEnable
                                Me.ExitToolStripMenuItem.Enabled = pEnable
                                Me.HelpToolStripMenuItem.Enabled = pEnable
                                Me.ProductionToolStripMenuItem.Enabled = pEnable

                                Me.UtilitiesToolStripMenuItem.Enabled = False
                                'Me.ConditioningToolStripMenuItem.Enabled = isStandby
                                'Me.DemoModeToolStripMenuItem.Enabled = isStandby
                                'Me.AnalyzerInfoToolStripMenuItem.Enabled = isStandby
                                'Me.InstrumentUpdateToolStripMenuItem.Enabled = True
                                'Me.SendableScriptsToolStripMenuItem.Enabled = isStandby
                                'Me.SettingsToolStripMenuItem.Enabled = True
                                'Me.ChangeRotorToolStripMenuItem.Enabled = isStandby

                            End If
                            ' XBC 09/02/2012

                            'SGM 08/11/2012 - if Stop Alarm occured while initialization
                            If Not ActiveMdiChild Is Nothing Then
                                If (TypeOf ActiveMdiChild Is AnalyzerInfo) Then
                                    Dim CurrentMdiChild As AnalyzerInfo = CType(ActiveMdiChild, AnalyzerInfo)
                                    If CurrentMdiChild.IsStopAlarmWhileInitialization Then
                                        If Not RecoverRequested Then
                                            Me.ConfigurationToolStripMenuItem.Enabled = True

                                            Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                                            Me.UtilitiesToolStripMenuItem.Enabled = False

                                            Me.RegistryMaintenanceToolStripMenuItem.Enabled = True

                                            Me.ExitToolStripMenuItem.Enabled = True
                                            Me.WithShutToolStripMenuItem.Enabled = False
                                            Me.WithOutShutDownToolStripMenuItem.Enabled = True

                                            Me.HelpToolStripMenuItem.Enabled = True
                                        End If
                                    End If
                                End If
                            End If
                            'end SGM 08/11/2012

                        End If

                    Else
                        Me.ConfigurationToolStripMenuItem.Enabled = pEnable
                        Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                        Me.UtilitiesToolStripMenuItem.Enabled = False
                        Me.RegistryMaintenanceToolStripMenuItem.Enabled = False
                        Me.ExitToolStripMenuItem.Enabled = False
                        Me.HelpToolStripMenuItem.Enabled = False
                        Me.ProductionToolStripMenuItem.Enabled = False
                    End If
                End If

            End If

            If MyClass.IsWaitingForNewStatus Or MyClass.IsAutoConnecting Then
                Me.Cursor = Cursors.WaitCursor
            Else
                Me.Cursor = Cursors.Default
            End If

            ' XBC 08/02/2012
            Me.ControlBox = pEnable

            ' XBC 24/10/2012
            If ManageAlarmType <> ManagementAlarmTypes.NONE Then

                Select Case ManageAlarmType
                    Case ManagementAlarmTypes.UPDATE_FW
                        'FIRWARE PENDING TO UPDATE
                        Me.ConfigurationToolStripMenuItem.Enabled = True
                        Me.GeneralToolStripMenuItem.Enabled = False
                        Me.LanguageToolStripMenuItem.Enabled = True
                        Me.BarcodesConfigToolStripMenuItem.Enabled = False
                        Me.UserToolStripMenuItem.Enabled = True
                        Me.ChangeUserToolStripMenuItem.Enabled = True

                        Me.UtilitiesToolStripMenuItem.Enabled = (MyClass.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) ' True
                        Me.ConditioningToolStripMenuItem.Enabled = False
                        Me.DemoModeToolStripMenuItem.Enabled = False
                        Me.AnalyzerInfoToolStripMenuItem.Enabled = False
                        Me.InstrumentUpdateToolStripMenuItem.Enabled = (MyClass.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) ' True
                        Me.SendableScriptsToolStripMenuItem.Enabled = False
                        Me.SettingsToolStripMenuItem.Enabled = True
                        Me.ChangeRotorToolStripMenuItem.Enabled = False

                        Me.HelpToolStripMenuItem.Enabled = True

                        Me.ExitToolStripMenuItem.Enabled = True
                        Me.WithShutToolStripMenuItem.Enabled = False
                        Me.WithOutShutDownToolStripMenuItem.Enabled = True

                    Case ManagementAlarmTypes.FATAL_ERROR
                        Me.RegistryMaintenanceToolStripMenuItem.Enabled = False
                        Me.HelpToolStripMenuItem.Enabled = False
                        Me.ExitToolStripMenuItem.Enabled = False
                        Me.ConfigurationToolStripMenuItem.Enabled = False
                        Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                        Me.UtilitiesToolStripMenuItem.Enabled = False
                        Me.ProductionToolStripMenuItem.Enabled = False

                    Case ManagementAlarmTypes.RECOVER_ERROR
                        If Not RecoverRequested Then
                            Me.ConfigurationToolStripMenuItem.Enabled = True

                            ' XBC 07/11/2012 - Correction : Firmware is able to receive LOADADJ, CONFIG or BARCODE
                            'Me.GeneralToolStripMenuItem.Enabled = False
                            'Me.LanguageToolStripMenuItem.Enabled = True
                            'Me.BarcodesConfigToolStripMenuItem.Enabled = False
                            'Me.UserToolStripMenuItem.Enabled = True
                            'Me.ChangeUserToolStripMenuItem.Enabled = True
                            ' XBC 07/11/2012 

                            Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                            Me.UtilitiesToolStripMenuItem.Enabled = False

                            Me.RegistryMaintenanceToolStripMenuItem.Enabled = True

                            Me.ExitToolStripMenuItem.Enabled = True
                            Me.WithShutToolStripMenuItem.Enabled = False
                            Me.WithOutShutDownToolStripMenuItem.Enabled = True

                            Me.HelpToolStripMenuItem.Enabled = True
                        End If

                    Case ManagementAlarmTypes.SIMPLE_ERROR
                        Me.ConfigurationToolStripMenuItem.Enabled = pEnable
                        Me.AdjustmentsTestsToolStripMenuItem.Enabled = pEnable
                        Me.UtilitiesToolStripMenuItem.Enabled = pEnable
                        Me.RegistryMaintenanceToolStripMenuItem.Enabled = pEnable
                        Me.ExitToolStripMenuItem.Enabled = pEnable
                        Me.HelpToolStripMenuItem.Enabled = pEnable
                        Me.ProductionToolStripMenuItem.Enabled = pEnable

                        If Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                            ' XBC 07/11/2012 - Correction : Firmware is able to receive LOADADJ, CONFIG or BARCODE
                            'Me.GeneralToolStripMenuItem.Enabled = False
                            'Me.BarcodesConfigToolStripMenuItem.Enabled = False
                            'Me.UtilitiesToolStripMenuItem.Enabled = False
                            ' XBC 07/11/2012
                            Me.AdjustmentsTestsToolStripMenuItem.Enabled = False
                            Me.WithShutToolStripMenuItem.Enabled = False
                        End If

                End Select


            End If
            ' XBC 24/10/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Activates or not the vertical buttons (action buttons) depending the 
    ''' analyzer state (free or busy), the analyzer status, current alarms 
    ''' and so on
    ''' </summary>
    ''' <remarks>
    ''' Created by AG 01/06/2010 (Tested pending) (TODO case STANDBY)
    ''' Modified by SGM 27/09/2011
    ''' </remarks>
    Public Sub ActivateActionButtonBar(ByVal pEnabled As Boolean, _
                                       Optional ByVal pDisableForced As Boolean = False, _
                                       Optional ByVal pFwUpdated As Boolean = False)
        Dim isconnected As Boolean = False
        Try

            ' XBC 22/05/2012
            If Not ActiveMdiChild Is Nothing Then
                If (TypeOf ActiveMdiChild Is IPositionsAdjustments) Then
                    Dim CurrentMdiChild As IPositionsAdjustments = CType(ActiveMdiChild, IPositionsAdjustments)
                    If CurrentMdiChild.IsCenteringOptic Then
                        pEnabled = False
                    End If
                End If
            End If
            ' XBC 22/05/2012

            If Me.SimulationMode Then
                If String.Compare(Me.bsAnalyzerStatus.Text, "NOT CONNECTED", False) <> 0 Then
                    isconnected = True
                End If
            Else
                If Me.MDIAnalyzerManager IsNot Nothing Then
                    If Me.MDIAnalyzerManager.Connected Then
                        isconnected = True
                    End If
                End If
            End If


            'action buttons

            ' XBC 26/10/2012
            'Me.bsTSConnectButton.Enabled = Not isconnected
            If pEnabled Then
                Me.bsTSConnectButton.Enabled = Not isconnected
            End If

            'Me.bsTSEmergencyButton.Enabled = True 'PDT to spec
            'Me.bsTSStandByButton.enabled = False
            Me.bsTSShutdownButton.Enabled = False
            Me.bsTSRecoverButton.Enabled = False 'SGM 18/10/2012

            If Not pFwUpdated Then
                If Not MyClass.IsAnalyzerInitiated AndAlso MyClass.MDIAnalyzerManager.IsFwSwCompatible Then pDisableForced = True
            End If

            ' XBC 24/10/2012
            If ManageAlarmType <> ManagementAlarmTypes.NONE Then
                pDisableForced = False
                MyClass.IsAutoConnecting = False
            End If
            ' XBC 24/10/2012

            If pDisableForced Then pEnabled = False

            If pDisableForced Then Exit Sub 'SGM 16/04/2012

            ' XBC 09/02/2012
            Me.ControlBox = pEnabled

            If MyClass.IsAutoConnecting Then Exit Sub

            ' XBC 09/02/2012
            If Not pEnabled Then Exit Sub

            If isconnected And Not MyClass.IsWaitingForNewStatus Then

                Me.bsTSConnectButton.Enabled = False
                Me.bsTSRecoverButton.Enabled = False 'SGM 18/10/2012

                If MyClass.MDIAnalyzerManager.IsFwSwCompatible Then 'SGM 30/05/2012
                    Select Case Me.MDIAnalyzerManager.AnalyzerStatus
                        Case AnalyzerManagerStatus.SLEEPING
                            Me.bsTSConnectButton.Enabled = True
                            Me.bsTSShutdownButton.Enabled = False

                        Case AnalyzerManagerStatus.STANDBY
                            Me.bsTSShutdownButton.Enabled = True

                        Case AnalyzerManagerStatus.RUNNING
                            Me.bsTSShutdownButton.Enabled = False

                    End Select

                ElseIf MyClass.MDIAnalyzerManager.IsFwReaded Then
                    Me.bsTSConnectButton.Enabled = False
                    Me.bsTSShutdownButton.Enabled = False
                Else
                    ' XBC 25/10/2012
                    Me.bsTSConnectButton.Enabled = Not isconnected
                    'Me.bsTSShutdownButton.Enabled = False
                    Select Case Me.MDIAnalyzerManager.AnalyzerStatus
                        Case AnalyzerManagerStatus.STANDBY
                            Me.bsTSShutdownButton.Enabled = True
                        Case Else
                            Me.bsTSShutdownButton.Enabled = False
                    End Select
                    ' XBC 25/10/2012
                End If

            Else
                Me.bsTSConnectButton.Enabled = (Not MyClass.IsWaitingForNewStatus) ' (Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY And Not MyClass.IsWaitingForNewStatus)
                'Me.bsTSEmergencyButton.Enabled = False SGM 15/11/2011
                Me.bsTSShutdownButton.Enabled = False
                Me.bsTSRecoverButton.Enabled = False 'SGM 18/10/2012
            End If


            ' XBC 24/10/2012
            If ManageAlarmType <> ManagementAlarmTypes.NONE Then

                Select Case ManageAlarmType
                    Case ManagementAlarmTypes.UPDATE_FW
                        Me.bsTSConnectButton.Enabled = False
                        Me.bsTSShutdownButton.Enabled = False
                        Me.bsTSRecoverButton.Enabled = False

                    Case ManagementAlarmTypes.FATAL_ERROR
                        Me.bsTSConnectButton.Enabled = False
                        Me.bsTSShutdownButton.Enabled = False
                        Me.bsTSRecoverButton.Enabled = False

                    Case ManagementAlarmTypes.RECOVER_ERROR
                        If Not RecoverRequested Then
                            Me.bsTSConnectButton.Enabled = False
                            Me.bsTSShutdownButton.Enabled = False
                            Me.bsTSRecoverButton.Enabled = True
                        End If

                    Case ManagementAlarmTypes.SIMPLE_ERROR
                        Select Case Me.MDIAnalyzerManager.AnalyzerStatus
                            Case AnalyzerManagerStatus.SLEEPING
                                Me.bsTSConnectButton.Enabled = pEnabled
                                Me.bsTSShutdownButton.Enabled = False
                            Case AnalyzerManagerStatus.STANDBY
                                Me.bsTSConnectButton.Enabled = False
                                Me.bsTSShutdownButton.Enabled = pEnabled
                        End Select
                        Me.bsTSRecoverButton.Enabled = False

                End Select


            End If
            ' XBC 24/10/2012


        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message, Me.Name & ".ActivateActionButtonBar ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " ActivateActionButtonBar, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    'SGM 25/11/2011
    Public Sub WriteDebugConsoleTraceLine(ByVal pData As String)
        Try
#If DEBUG Then
            If myConsole IsNot Nothing Then
                myConsole.WriteLine(pData)
            End If
#End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".WriteDebugConsoleTraceLine ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " WriteDebugConsoleTraceLine, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub
#End Region

#Region "Private Methods"

#Region "Generic Adjusments DS Methods"
    ''' <summary>
    ''' Gets the value corresponding to informed Level (axis) from the selected adjustments dataset
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 18/05/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function ReadSensorAdjustmentValue(ByVal pAdjType As ADJUSTMENT_GROUPS, ByVal pSubType As AXIS) As Single
        Dim myGlobal As New GlobalDataTO
        Dim myResult As Single = -99999
        ''Dim myUtil As New Utilities.
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            Dim myAdjustmentRowData As New AdjustmentRowData("")

            Dim myAdj As String = pAdjType.ToString
            If myAdj = "_NONE" Then myAdj = ""

            Dim mySubType As String = pSubType.ToString
            If String.Compare(mySubType, "_NONE", False) = 0 Then mySubType = ""

            If Me.myAllAdjustmentsDS IsNot Nothing Then
                Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                    In Me.myAllAdjustmentsDS.srv_tfmwAdjustments _
                                    Where a.GroupID.Trim = myAdj.Trim _
                                    And a.AxisID.Trim = mySubType.Trim _
                                    Select a).ToList
                'Where a.GroupID.Trim.ToUpper = myAdj.Trim.ToUpper _
                'And a.AxisID.Trim.ToUpper = mySubType.Trim.ToUpper _

                If myAdjustmentRows.Count > 0 Then
                    myResult = Utilities.FormatToSingle(myAdjustmentRows.First.Value)
                    'Dim myValueStr As String = myAdjustmentRows.First.Value
                    'If IsNumeric(Trim(myValueStr.Replace(".", myDecimalSeparator))) Then
                    '    myResult = CSng(myValueStr.Replace(".", myDecimalSeparator))
                    'End If
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & ".ReadSensorAdjustmentValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadSensorAdjustmentValue ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return myResult

    End Function


    ''' <summary>
    ''' Gets adjustments from the AnalyzerManager
    ''' </summary>
    ''' <remarks>Created by SGM 21/09/2011</remarks>
    Public Sub ReadAllFwAdjustments()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MDIAnalyzerManager.ReadFwAdjustmentsDS()
            If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then

                Me.myAllAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                'SGM 28/11/2011 update fields AnalyzerID and FwVersion
                For Each A As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In Me.myAllAdjustmentsDS.srv_tfmwAdjustments.Rows
                    A.AnalyzerID = MyClass.AnalyzerIDAttribute
                    A.FwVersion = MyClass.FwVersionAttribute
                Next
                Me.myAllAdjustmentsDS.AcceptChanges()
                'SGM 28/11/2011

                'update database
                Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, Me.myAllAdjustmentsDS)

                ' XBC 04/10/2011 - Export to file is commented because this functionality is already executed in Comm layer (and this is the best option)
                ''update text file
                'Me.myFwAdjustmentsDelegate = New FwAdjustmentsDelegate(Me.myAllAdjustmentsDS)
                'myGlobal = Me.myFwAdjustmentsDelegate.ExportDSToFile("", Me.MDIAnalyzerManager.ActiveAnalyzer)

            End If

        Catch ex As Exception
            'Write Error in the Application Log and show the message
            CreateLogActivity(ex.Message, Me.Name & ".ReadAllFwAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadAllFwAdjustments ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub
#End Region


    ''' <summary>
    ''' Method incharge to load the buttons image
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 25/05/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            ' XBC 18/05/2012 - IS USED ???
            ''Dim iconPath As String = ConfigurationManager.AppSettings("IconsPath").ToString
            'Dim auxIconName As String = ""
            'Dim iconPath As String = MyBase.IconsPath

            'Analyzer Configuration Button
            'auxIconName = GetIconName("ANALYZER")
            'If (auxIconName <> "") Then bsTSAnalysersButton.Image = Image.FromFile(iconPath & auxIconName)

            'Alarms button
            'auxIconName = GetIconName("ALARM")
            'If (auxIconName <> "") Then bsTSAlarmsButton.Image = Image.FromFile(iconPath & auxIconName)
            ' XBC 18/05/2012 - IS USED ???

            ' XBC 23/02/2012 - IS USED ???
            ''Alarm Utilities button
            'auxIconName = GetIconName("UTILITIES")
            'If (auxIconName <> "") Then bsTSUtilitiesButton.Image = Image.FromFile(iconPath & auxIconName)
            ' XBC 23/02/2012 - IS USED ???

            'Emergency Stop button
            'auxIconName = GetIconName("EMERGENCY")
            'If (auxIconName <> "") Then bsTSEmergencyButton.Image = Image.FromFile(iconPath & auxIconName)

        Catch ex As Exception
            'Write Error in the Application Log and show the message
            CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Load the current language description and activate or deactivate the menu options
    ''' </summary>
    ''' <remarks>
    ''' Created by AG 17/06/2010
    ''' </remarks>
    ''' PDT !!!
    Private Sub PrepareMenuOptions()
        Try
            'Load the menu options Language description
            GetScreenLabels()

            'MasterDataDictionaryToolStripMenuItem.Enabled = False

            'Exit
            WithShutToolStripMenuItem.Enabled = True
            WithOutShutDownToolStripMenuItem.Enabled = True

            If GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
                Me.BorrameToolStripMenuItem.Visible = (GlobalConstants.REAL_DEVELOPMENT_MODE = 2)
                Me.MonitorToolStripMenuItem.Visible = (GlobalConstants.REAL_DEVELOPMENT_MODE = 2)
                WithOutShutDownToolStripMenuItem.Enabled = (GlobalConstants.REAL_DEVELOPMENT_MODE = 1)
            End If

        Catch ex As Exception
            'Write Error in the Application Log and show the message
            CreateLogActivity(ex.Message, Me.Name & ".PrepareMenuOptions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareMenuOptions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the menu options availability according to the current user level
    ''' </summary>
    ''' <remarks>Created by SGM 25/05/2012</remarks>
    Private Sub ManageMenuOptionsByUserLevel()

        Try
            Dim myCurrentUser As USER_LEVEL = CType(Me.CurrentUserLevelAttribute, USER_LEVEL)

            Me.SettingsToolStripMenuItem.Visible = (myCurrentUser >= USER_LEVEL.lBIOSYSTEMS)
            Me.SendableScriptsToolStripMenuItem.Visible = (myCurrentUser >= USER_LEVEL.lBIOSYSTEMS)
            Me.BorrameToolStripMenuItem.Visible = (myCurrentUser >= USER_LEVEL.lBIOSYSTEMS)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareMenuOptions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareMenuOptions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 01/12/2011</remarks>
    Private Sub PrepareXPSViewer()
        Try
            Dim myDummyInfoTextControl As New BsXPSViewer

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & " PrepareInformation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
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
    ''' <param name="pScreenToOpen">Screen selected to be opened</param>
    ''' <returns>False is the screen opening was cancelled; otherwise, it returns True</returns>
    ''' <remarks>
    ''' Created by:  SA 17/09/2010
    ''' Modified by: SA 20/09/2010 - After validation on the ActiveMDIChild, close all open MdiChildren (to avoid problems with
    '''                              "small" forms like Language Conf., Analyzer Conf., etc)
    ''' </remarks>
    Private Function OpenMDIChildForm(ByVal pScreenToOpen As BSBaseForm) As Boolean
        Dim screenOpened As Boolean = True

        Try
            ' XBC 20/04/2012
            Me.pScreenPendingToOpen = Nothing

            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor

            If (Not pScreenToOpen Is Nothing) Then
                If (Not ActiveMdiChild Is Nothing) Then

                    'PENDING
                    'If TypeOf ActiveMdiChild Is BSAdjustmentBaseForm Then
                    '    If Not (TypeOf ActiveMdiChild Is IStressModeTest) Then
                    '        Dim myBaseAdjForm As BSAdjustmentBaseForm = CType(ActiveMdiChild, BSAdjustmentBaseForm)
                    '        myBaseAdjForm.CloseRequestedByMDI = True
                    '        If TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest Then
                    '            Dim myMotorsForm As IMotorsPumpsValvesTest = CType(Me.ActiveMdiChild, IMotorsPumpsValvesTest)
                    '            If Not myMotorsForm.IsReadyToClose Then
                    '                Return False
                    '            End If
                    '        End If
                    '    End If
                    'Else

                    '    If (ActiveMdiChild.Name = pScreenToOpen.Name) Then
                    '        'If the screen to open is already opened, do nothing
                    '        screenOpened = False
                    '    Else
                    '        If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                    '            'Execute the closing code of the screen currently open
                    '            Dim myScreenName As String = ActiveMdiChild.Name
                    '            ActiveMdiChild.AcceptButton.PerformClick()

                    '            'The screen currently open could not be closed, the opening is cancelled
                    '            If (Not ActiveMdiChild Is Nothing AndAlso ActiveMdiChild.Name = myScreenName) Then
                    '                screenOpened = False
                    '            End If
                    '        End If
                    '    End If

                    'End If

                    If (ActiveMdiChild.Name = pScreenToOpen.Name) Then
                        'If the screen to open is already opened, do nothing
                        screenOpened = False
                    Else
                        If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                            'Execute the closing code of the screen currently open
                            Dim myScreenName As String = ActiveMdiChild.Name
                            ActiveMdiChild.AcceptButton.PerformClick()

                            'The screen currently open could not be closed, the opening is cancelled
                            If (Not ActiveMdiChild Is Nothing AndAlso String.Compare(ActiveMdiChild.Name, myScreenName, False) = 0) Then
                                screenOpened = False

                                'SGM 10/05/2012
                                If TypeOf ActiveMdiChild Is IISEUtilities Then
                                    Dim myISEUtilities As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                    myISEUtilities.myScreenPendingToOpenWhileISEUtilClosing = pScreenToOpen
                                Else
                                    ' XBC 20/04/2012
                                    Me.pScreenPendingToOpen = pScreenToOpen
                                End If
                                'end SGM 10/05/2012

                            End If
                        End If
                    End If
                End If

                If (screenOpened) Then
                    'If the ActiveMDI was closed, verify any other screen is open before opening the requested one
                    Dim i As Integer = 0
                    For i = Me.MdiChildren.Length - 1 To 0 Step -1
                        If (MdiChildren(i).Name = pScreenToOpen.Name) Then
                            screenOpened = False

                            '' dlm 30/09/2010
                            'Dim propiedadListView As System.Reflection.PropertyInfo

                            'propiedadListView = GetType(Form).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)
                            'propiedadListView.SetValue(MdiChildren(i), True, Nothing)
                            '' end dlm

                            MdiChildren(i).Show()
                        Else
                            MdiChildren(i).Close()
                        End If
                    Next
                End If

                If (screenOpened) Then
                    ' dlm 30/09/2010

                    '' end dlm

                    ''Everything is OK, open the selected screen
                    'pScreenToOpen.MdiParent = Me
                    'pScreenToOpen.AnalyzerModel = me.ActiveAnalyzerModel 'SGM 04/03/11

                    ''SGM 12/04/11
                    'pScreenToOpen.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                    'pScreenToOpen.Location = New Point(0, 0)
                    'pScreenToOpen.ControlBox = False
                    'pScreenToOpen.MinimizeBox = False
                    'pScreenToOpen.MaximizeBox = False
                    ''END SGM 12/04/11

                    'pScreenToOpen.Show()

                    'pScreenToOpen.Dock = DockStyle.Top 'SGM 12/04/11

                    'SGM 18/04/2012
                    MyClass.ActivateActionButtonBar(True, Not pScreenToOpen.Enabled)
                    MyClass.ActivateMenus(True, Not pScreenToOpen.Enabled)
                    'end SGM 18/04/2012

                    'Everything is OK, open the selected screen
                    pScreenToOpen.AnalyzerModel = Me.ActiveAnalyzerModel
                    pScreenToOpen.MdiParent = Me
                    pScreenToOpen.Show()

                End If
            Else
                'SGM 05/11/2012
                Me.Text = My.Application.Info.ProductName
            End If
        Catch ex As Exception
            screenOpened = False
            CreateLogActivity(ex.Message, Me.Name & ".OpenMDIChildForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OpenMDIChildForm ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try

        Return screenOpened
    End Function

    ''' <summary>
    ''' Closes the Application
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 22/06/2011</remarks>
    Private Function CloseApplication() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try


            'Close the current screen (if any)
            If (OpenMDIChildForm(Nothing)) Then

                'SGM 03/08/2011
                If Me.ActiveMdiChild IsNot Nothing Then
                    If Me.ActiveMdiChild.AcceptButton IsNot Nothing Then
                        'SGM 28/03/2012
                        If TypeOf ActiveMdiChild Is BSAdjustmentBaseForm Then
                            If Not (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                Dim myBaseAdjForm As BSAdjustmentBaseForm = CType(ActiveMdiChild, BSAdjustmentBaseForm)
                                myBaseAdjForm.CloseWithoutShutDownRequestedByMDI = True
                                myBaseAdjForm.CloseRequestedByMDI = True
                                If TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest Then
                                    Dim myMotorsForm As IMotorsPumpsValvesTest = CType(Me.ActiveMdiChild, IMotorsPumpsValvesTest)
                                    If Not myMotorsForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                                If TypeOf ActiveMdiChild Is IPositionsAdjustments Then
                                    Dim myPosForm As IPositionsAdjustments = CType(Me.ActiveMdiChild, IPositionsAdjustments)
                                    If Not myPosForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                                If TypeOf ActiveMdiChild Is ITankLevelsAdjustments Then
                                    Dim myTankForm As ITankLevelsAdjustments = CType(Me.ActiveMdiChild, ITankLevelsAdjustments)
                                    If Not myTankForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                                If TypeOf ActiveMdiChild Is IBarCodeAdjustments Then
                                    Dim myBarcodeForm As IBarCodeAdjustments = CType(Me.ActiveMdiChild, IBarCodeAdjustments)
                                    If Not myBarcodeForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                                'If TypeOf ActiveMdiChild Is IInstrumentUpdateUtil Then
                                '    Dim myInstrumForm As IInstrumentUpdateUtil = CType(Me.ActiveMdiChild, IInstrumentUpdateUtil)
                                '    If Not myInstrumForm.IsReadyToClose Then
                                '        Me.isWaitingForCloseApp = True
                                '        Return myGlobal
                                '    End If
                                'End If
                            End If
                        ElseIf TypeOf ActiveMdiChild Is IISEUtilities Then
                            Dim myISEUtilities As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)

                            'SGM 19/09/2012
                            If Not myISEUtilities.IsCompletelyClosed Then
                                myISEUtilities.myScreenPendingToOpenWhileISEUtilClosing = Nothing
                                myISEUtilities.IsMDICloseRequested = True
                                ActiveMdiChild.AcceptButton.PerformClick()
                                Me.isWaitingForCloseApp = True
                                Return myGlobal
                            Else
                                myISEUtilities.Dispose()
                                myISEUtilities = Nothing
                            End If

                        Else

                            ActiveMdiChild.AcceptButton.PerformClick()

                        End If
                    End If
                    'end SGM 28/03/2012
                End If

                If Not Me.SimulationMode Then
                    'the INFO data refreshing mode must be deactivated before closing the application
                    'SGM 19/09/2011
                    If Me.MDIAnalyzerManager IsNot Nothing Then
                        If Me.MDIAnalyzerManager.Connected Then

                            ' XBC 02/05/2012
                            If Not Me.MDIAnalyzerManager.IsStressing Then
                                myGlobal = MyClass.SEND_INFO_STOP()
                            End If

                            'myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
                            '                                     True, _
                            '                                     Nothing, _
                            '                                     GlobalEnumerates.Ax00InfoInstructionModes.STP)

                        End If
                    End If

                    'Close communication channel
                    If (Not MDIAnalyzerManager Is Nothing) Then

                        If (MDIAnalyzerManager.CommThreadsStarted) Then
                            'Dispose communications channel
                            MDIAnalyzerManager.Terminate()
                        End If
                    End If

                Else
                    Me.SimulateINFO_Off()
                End If



                'RH 03/11/2010 Wait until Ax00MDBackGround.RunWorkerAsync() is completed
                'DL 20/04/2012
                'Using wfPreload As New Biosystems.Ax00.PresentationCOM.IAx00StartUp(Me)
                Using wfPreload As New Biosystems.Ax00.PresentationCOM.WaitScreen(Me)
                    'dl 20/04/2012
                    wfPreload.Title = "Waiting ongoing processes completion..." 'AG - Not multilanguage text
                    wfPreload.WaitText = "Please wait..." 'AG - Not multilanguage text
                    wfPreload.Show()

                    While RunningAx00MDBackGround
                        Application.DoEvents()
                        System.Threading.Thread.Sleep(100)
                    End While

                    wfPreload.Close()
                End Using

                'RH 15/10/2010
                'Unregister CommAX00.exe COM server
                'System.Diagnostics.Process.Start("CommAX00.exe", "/unregserver")

                'RH 01/03/2012 Disable and release bsDateTimer
                'bsDateTimer.Enabled = False
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
                    If String.Compare(Application.OpenForms(Index).Name, Me.Name, False) = 0 Then
                        Index += 1
                    Else
                        Application.OpenForms(Index).Close()
                    End If
                End While

            Else
                myGlobal.SetDatos = True
            End If


            If Not myGlobal.HasError Then
                ' XBC 26/10/2011
                If ActiveMdiChild Is Nothing Then
                    Me.IsFinalClosing = True
                    'Me.Close()
                End If
            End If


            'SGM 28/03/2012
            If Me.IsFinalClosing Then
                Me.Close()
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & ".CloseApplication ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CloseApplication ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Return myGlobal

    End Function

    ''' <summary>
    ''' Shuts down the Analyzer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 22/09/2011</remarks>
    Private Function ShutDownAnalyzer() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myText1 As String
            Dim myText2 As String

            myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SHUTTING_DOWN", CurrentLanguageAttribute)
            myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SHUT_WAIT", CurrentLanguageAttribute)

            Me.BsMonitor.DisableAllSensors()
            Me.ActivateActionButtonBar(False)
            Me.ActivateMenus(False)

            ' XBC 09/02/2012
            If Not ActiveMdiChild Is Nothing Then
                If (Not ActiveMdiChild.AcceptButton Is Nothing) Then
                    'Execute the closing code of the screen currently open
                    'Dim myScreenName As String = ActiveMdiChild.Name

                    If Me.ActiveMdiChild.AcceptButton IsNot Nothing Then
                        'SGM 28/03/2012
                        If TypeOf ActiveMdiChild Is BSAdjustmentBaseForm Then
                            If Not (TypeOf ActiveMdiChild Is IStressModeTest) Then
                                Dim myBaseAdjForm As BSAdjustmentBaseForm = CType(ActiveMdiChild, BSAdjustmentBaseForm)
                                myBaseAdjForm.CloseWithShutDownRequestedByMDI = True
                                myBaseAdjForm.CloseRequestedByMDI = True
                                If TypeOf ActiveMdiChild Is IMotorsPumpsValvesTest Then
                                    Dim myMotorsForm As IMotorsPumpsValvesTest = CType(Me.ActiveMdiChild, IMotorsPumpsValvesTest)
                                    If Not myMotorsForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                                If TypeOf ActiveMdiChild Is IPositionsAdjustments Then
                                    Dim myPosForm As IPositionsAdjustments = CType(Me.ActiveMdiChild, IPositionsAdjustments)
                                    If Not myPosForm.IsReadyToClose Then
                                        Me.isWaitingForCloseApp = True
                                        Return myGlobal
                                    End If
                                End If
                            End If
                        Else

                            ActiveMdiChild.AcceptButton.PerformClick()

                        End If
                    End If

                    'ActiveMdiChild.AcceptButton.PerformClick()
                End If
            End If
            If Not ActiveMdiChild Is Nothing Then
                Exit Try
            End If
            ' XBC 09/02/2012

            If Me.SimulationMode Then

                If Me.IsFinalWashingNeeded Then 'PDT
                    '????
                End If

                Me.SimulateINFO_Off()


                Me.UserSleepRequested = True
                Me.WaitControl(myText1, myText2)

                System.Threading.Thread.Sleep(2000)

                Me.isWaitingForSleep = False

                Me.bsAnalyzerStatus.Text = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING.ToString

                Me.BsMonitor.DisableAllSensors()
                Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, False, Me.NotConnectedText)
                Me.bsAnalyzerStatus.Text = "NOT CONNECTED"

                Me.ActivateActionButtonBar(True)
                Me.ActivateMenus(True)

            Else
                If Not MDIAnalyzerManager Is Nothing Then

                    ' XBC 26/06/2012 - ISE final self-washing
                    If Not MDIAnalyzerManager.ISE_Manager Is Nothing Then

                        ' XBC 08/11/2012 - No send ISE Cleans when It is configured as Log Term deactivated
                        'If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled And MDIAnalyzerManager.ISE_Manager.IsISEInitiatedOK Then
                        If MDIAnalyzerManager.ISE_Manager.IsISEModuleInstalled AndAlso _
                           MDIAnalyzerManager.ISE_Manager.IsISEInitiatedOK AndAlso _
                           Not MDIAnalyzerManager.ISE_Manager.IsLongTermDeactivation Then
                            ' XBC 08/11/2012

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
                                                    Me.UserSleepRequested = True
                                                    Me.WaitControl(myText1, myText2)
                                                    ' waiting until ISE CLEAN operation is performed
                                                    ' then will be back to continue Shut Down operation
                                                    Exit Try
                                                End If
                                            End If

                                        End If
                                    End If
                                End If
                            End If

                            If myGlobal.HasError Then
                                ShowMessage("Error", GlobalEnumerates.Messages.ISE_CLEAN_WRONG.ToString)
                                Exit Try
                            End If
                        End If
                    End If
                    ' XBC 26/06/2012


                    Me.MDIAnalyzerManager.IsShutDownRequested = True


                    ' XBC 09/02/2012
                    ''Disable all buttons until Ax00 accept the new instruction
                    ''action buttons
                    'Me.bsTSConnectButton.Enabled = False
                    'Me.bsTSEmergencyButton.Enabled = False
                    ''Me.bsTSStandByButton.enabled = False
                    'Me.bsTSShutdownButton.Enabled = False
                    ' XBC 09/02/2012

                    If Me.IsFinalWashingNeeded Then 'PDT
                        'If Not me.IsFinalWashed Then
                        '    myGlobal = me.SEND_WASH
                        'End If
                    Else
                        If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                            myGlobal = Me.SEND_INFO_STOP
                        End If
                    End If

                    Application.DoEvents()



                    If Not myGlobal.HasError AndAlso Me.MDIAnalyzerManager.Connected Then
                        Me.UserSleepRequested = True
                        Me.WaitControl(myText1, myText2)
                        Me.MDIAnalyzerManager.ClearQueueToSend()

                        Me.bsTSAlarmsButton.Enabled = False 'SGM 21/11/2012

                        myGlobal = Me.SEND_SLEEP
                    End If

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        If Not MDIAnalyzerManager.Connected Then
                            Dim myAuxGlobal As New GlobalDataTO
                            myAuxGlobal.ErrorCode = "ERROR_COMM"
                            ShowMessage("Warning", myAuxGlobal.ErrorCode)
                        End If

                    ElseIf myGlobal.HasError Then
                        ShowMessage("Error", myGlobal.ErrorMessage)
                    End If

                End If
            End If

        Catch ex As Exception
            Me.UserSleepRequested = False
            Me.isWaitingForSleep = False
            Me.MDIAnalyzerManager.IsShutDownRequested = False
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            CreateLogActivity(ex.Message, Me.Name & ".ShutDownAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShutDownAnalyzer ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Return myGlobal

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
            myGlobalDataTO = myAnalyzerDelegate.CheckAnalyzer(Nothing)
            ' XBC 07/06/2012

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myAnalyzerData As New AnalyzersDS
                myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

                If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                    'Inform properties AnalyzerID and AnalyzerModel

                    AnalyzerModelAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel.ToString
                    AnalyzerIDAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerID.ToString
                    FwVersionAttribute = myAnalyzerData.tcfgAnalyzers(0).FirmwareVersion.ToString
                    Me.AnalyzerModel = AnalyzerModelAttribute

                    MyClass.UpdatePreliminaryHomesMasterData(Nothing, AnalyzerIDAttribute) 'SGM 24/01/2012
                End If
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetAnalyzerInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAnalyzerInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    '''' <summary>
    '''' Get ID and Model of the Analyzer currently active
    '''' </summary>
    '''' <remarks>
    '''' Created by XBC 30/09/2011 - overloaded method to obtain the information of the currently connected Instrument
    '''' Deleted by XBC : 06/06/2012 - Not used
    '''' </remarks>
    'Private Sub GetAnalyzerInfo(ByVal pAnalyzerID As String)

    '    Try
    '        Dim myGlobalDataTO As New GlobalDataTO
    '        Dim myAnalyzerDelegate As New AnalyzersDelegate

    '        myGlobalDataTO = myAnalyzerDelegate.GetAnalyzerByID(Nothing, pAnalyzerID)
    '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
    '            Dim myAnalyzerData As New AnalyzersDS
    '            myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

    '            If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
    '                'Inform properties AnalyzerID and AnalyzerModel
    '                AnalyzerModelAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel.ToString
    '                AnalyzerIDAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerID.ToString
    '                MyClass.UpdatePreliminaryHomesMasterData(Nothing, AnalyzerIDAttribute) 'SGM 24/01/2012
    '            End If
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".GetAnalyzerInfo2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".GetAnalyzerInfo2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub



    ''' <summary>
    ''' update Preliminary Homes table in case of new Analyzer detected
    ''' </summary>
    ''' <param name="pAnalyzerID"></param>
    ''' <remarks>Created by SGM 20/01/2012</remarks>
    Public Sub UpdatePreliminaryHomesMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String)

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myScreenDelegate As New BaseFwScriptDelegate(pAnalyzerID)
            myGlobal = myScreenDelegate.GetAllPreliminaryHomes(pDBConnection)
            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                Dim myHomesDS As SRVPreliminaryHomesDS = CType(myGlobal.SetDatos, SRVPreliminaryHomesDS)
                If myHomesDS IsNot Nothing AndAlso myHomesDS.srv_tadjPreliminaryHomes.Rows.Count = 0 Then
                    'if missing then create homes related to the new analyzer
                    myGlobal = myScreenDelegate.InsertNewAnalyzerPreliminaryHomes(pDBConnection)
                End If
            End If

            If myGlobal.HasError Then
                Throw New Exception(myGlobal.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UpdatePreliminaryHomesMasterData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdatePreliminaryHomesMasterData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    '''' <summary>
    '''' updates ISE Information table in case of new Analyzer detected
    '''' </summary>
    '''' <param name="pAnalyzerID"></param>
    '''' <remarks>Created by SGM 24/01/2012</remarks>
    'Private Sub UpdateISEInformationMasterData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String)

    '    Dim myGlobal As New GlobalDataTO

    '    Try
    '        Dim myISEInfoDelegate As New ISEInfoDelegate(pAnalyzerID)
    '        myGlobal = myISEInfoDelegate.ReadAllSettings(pDBConnection)
    '        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
    '            Dim myISEInfoDS As ISEInformationDS = CType(myGlobal.SetDatos, ISEInformationDS)
    '            If myISEInfoDS IsNot Nothing AndAlso myISEInfoDS.tinfoISE.Rows.Count = 0 Then
    '                'if missing then create ISe settings related to the new analyzer
    '                myGlobal = ISEInfoDelegate.InsertNewAnalyzerISESettings(pDBConnection, pAnalyzerID)
    '            End If
    '        End If

    '        If myGlobal.HasError Then
    '            Throw New Exception(myGlobal.ErrorMessage)
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".UpdatePreliminaryHomesMasterData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".UpdatePreliminaryHomesMasterData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Call the Connect process (move into a method due to we want to call it when the MDI screen is opened)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' Modified by: SGM 28/09/2011
    ''' </remarks>
    Private Sub Connect(Optional ByVal pRequestStandBy As Boolean = False)

        Dim myGlobal As New GlobalDataTO

        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myText1 As String
            Dim myText2 As String

            ' XBC 18/09/2012 - add Update Fw functionality flag
            Me.UpdateFirmwareProcessEnd = False

            If Me.SimulationMode Then

                If Not AutoConnectProcess Then
                    Me.Cursor = Cursors.WaitCursor
                    Me.ActivateActionButtonBar(False)
                    Me.ActivateMenus(False)
                End If

                If String.Compare(Me.bsAnalyzerStatus.Text, "NOT CONNECTED", False) = 0 Then
                    If Not AutoConnectProcess Then
                        myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TRY_CONNECT", CurrentLanguageAttribute)
                        myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONNECT_WAIT", CurrentLanguageAttribute)
                        Me.ReconnectRequested = True
                        Me.WaitControl(myText1, myText2)
                    End If

                    System.Threading.Thread.Sleep(2000)
                End If

                Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING

                Me.isWaitingForConnected = False

                Application.DoEvents()

                If Not AutoConnectProcess Then
                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, False, Me.ConnectedText)
                    Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString
                End If

                Me.MDIAnalyzerManager.IsFwSwCompatible = True 'Assume that the Firmware is Compatible with current software version SGM 31/05/2012

                If pRequestStandBy Then

                    myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SETTING_STANDBY", CurrentLanguageAttribute)
                    myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_STANDBY_WAIT", CurrentLanguageAttribute)
                    Me.UserStandByRequested = True
                    Me.WaitControl(myText1, myText2)

                    System.Threading.Thread.Sleep(2000)

                    Me.isWaitingForStandBy = False

                    Application.DoEvents()

                    Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY

                End If

                If Not AutoConnectProcess Then
                    Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString

                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                    Me.Cursor = Cursors.Default
                End If

                MyClass.IsAutoConnecting = False

            Else

                'AG 28/07/2010
                If Not AutoConnectProcess Then
                    Application.DoEvents()
                    Me.Cursor = Cursors.WaitCursor
                    Me.BsMonitor.DisableAllSensors()
                End If
                'END AG 28/07/2010

                If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                    Dim myAnalyzer As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

                    'Disable all buttons until Ax00 accept the new instruction
                    'AG 28/07/2010
                    'Me.SetActionButtonsEnableProperty(False)
                    If Not AutoConnectProcess Then
                        'action buttons
                        Me.bsTSConnectButton.Enabled = False
                        'Me.bsTSEmergencyButton.Enabled = False
                        'Me.bsTSStandByButton.enabled = False
                        Me.bsTSShutdownButton.Enabled = False
                    End If
                    'END AG 28/07/2010

                    If Not AutoConnectProcess Then

                        Me.BsMonitor.DisableAllSensors()
                        Me.ActivateActionButtonBar(False)
                        Me.ActivateMenus(False)

                        Me.MDIAnalyzerManager.IsUserConnectRequested = True

                        'SGM 29/09/2011
                        myText1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TRY_CONNECT", CurrentLanguageAttribute)
                        myText2 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONNECT_WAIT", CurrentLanguageAttribute)
                        Me.ReconnectRequested = True
                        Me.WaitControl(myText1, myText2)
                        Application.DoEvents()
                    End If

                    'Dim myGlobal As New GlobalDataTO
                    myGlobal = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT, True)

                    Dim myTitle As String = ""
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        If Not myAnalyzer.Connected Then
                            myGlobal.ErrorCode = "ERROR_COMM"
                            myTitle = "Warning"
                            'Me.ShowMessage("Warning", myAuxGlobal.ErrorCode)
                        End If

                    ElseIf myGlobal.HasError Then
                        'AG 28/07/2010
                        'ShowMessage("Error", myGlobal.ErrorMessage)
                        'Me.ShowMessage("Error", myGlobal.ErrorCode)
                        myTitle = "Error"
                    End If

                    If Not AutoConnectProcess Then
                        Me.Cursor = Cursors.Default
                        If Not myGlobal.ErrorCode Is Nothing Then
                            Me.isWaitingForConnected = False
                            Me.isWaitingForStandBy = False
                            Me.isWaitingForSleep = False
                            Me.isWaitingForSleep = False

                            Application.DoEvents()

                            ' XBC 25/10/2011 - message is shown in method OnManageSentEvent
                            'Me.ShowMessage(myTitle, myGlobal.ErrorCode)
                        End If

                    Else
                        AutoConnectFailsErrorCode = myGlobal.ErrorCode
                        AutoConnectFailsTitle = myTitle
                    End If

                    'AG 28/0/2010
                    'If Not myAnalyzer.Connected Or myGlobal.HasError Then Me.ActivateActionButtonBar()
                    If Not AutoConnectProcess Then
                        If Not myAnalyzer.Connected Or myGlobal.HasError Then
                            Me.ActivateActionButtonBar(True)
                            Me.BsMonitor.DisableAllSensors()
                            Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, False, Me.NotConnectedText)
                            Me.bsAnalyzerStatus.Text = "NOT CONNECTED"
                        End If

                    End If
                    'END AG 28/0/2010

                End If




            End If

        Catch ex As Exception
            Me.MDIAnalyzerManager.IsUserConnectRequested = False
            CreateLogActivity(ex.Message, Me.Name & ".Connect ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Connect ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

        Finally
            'RH 13/10/2010
            'Move the following lines here, because we want it to be executed anyway at the end of the method
            'wherever there is an exception or not
            'AG 28/07/2010 - Close AutoConnect Thread
            'Me.Cursor = Cursors.Default
            If Not AutoConnectProcess Then
                Me.Cursor = Cursors.Default
            End If
            'END AG 28/07/2010

        End Try

    End Sub

    '''' <summary>
    '''' Call the Connect process (move into a method due to we want to call it when the MDI screen is opened)
    '''' </summary>
    '''' <remarks>
    '''' Created by:  AG 28/07/2010
    '''' </remarks>
    'Private Sub Connect2()



    '    Try


    '        'AG 28/07/2010
    '        If Not AutoConnectProcess Then
    '            Application.DoEvents()
    '            Me.Cursor = Cursors.WaitCursor
    '        End If
    '        'END AG 28/07/2010

    '        If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '            Dim myAnalyzer As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)


    '            'Disable all buttons until Ax00 accept the new instruction
    '            'AG 28/07/2010
    '            'Me.SetActionButtonsEnableProperty(False)
    '            If Not AutoConnectProcess Then
    '                Me.SetActionButtonsEnableProperty(False)


    '            End If
    '            'END AG 28/07/2010

    '            Dim myGlobal As New GlobalDataTO
    '            myGlobal = myAnalyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONNECT, True)

    '            Dim myTitle As String = ""
    '            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
    '                If Not myAnalyzer.Connected Then
    '                    myGlobal.ErrorCode = "ERROR_COMM"
    '                    myTitle = "Warning"
    '                    'Me.ShowMessage("Warning", myAuxGlobal.ErrorCode)
    '                End If

    '            ElseIf myGlobal.HasError Then
    '                'AG 28/07/2010
    '                'ShowMessage("Error", myGlobal.ErrorMessage)
    '                'Me.ShowMessage("Error", myGlobal.ErrorCode)
    '                myTitle = "Error"


    '            End If



    '            If Not AutoConnectProcess Then
    '                Me.Cursor = Cursors.Default
    '                If Not myGlobal.ErrorCode Is Nothing Then Me.ShowMessage(myTitle, myGlobal.ErrorCode)
    '            Else
    '                AutoConnectFailsErrorCode = myGlobal.ErrorCode
    '                AutoConnectFailsTitle = myTitle


    '            End If

    '            'AG 28/0/2010
    '            'If Not myAnalyzer.Connected Or myGlobal.HasError Then Me.ActivateActionButtonBar()
    '            If Not AutoConnectProcess Then
    '                If Not myAnalyzer.Connected Or myGlobal.HasError Then
    '                    Me.ActivateActionButtonBar(True)
    '                    Me.BsMonitor.DisableAllSensors()
    '                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, False, Me.NotConnectedText)
    '                    Me.bsAnalyzerStatus.Text = "NOT CONNECTED"
    '                End If

    '            End If
    '            'END AG 28/0/2010

    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".Connect ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".Connect ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

    '    Finally
    '        'RH 13/10/2010
    '        'Move the following lines here, because we want it to be executed anyway at the end of the method
    '        'where ever there is an exception or not
    '        'AG 28/07/2010 - Close AutoConnect Thread
    '        'Me.Cursor = Cursors.Default
    '        If Not AutoConnectProcess Then
    '            Me.Cursor = Cursors.Default

    '        End If
    '        'END AG 28/07/2010

    '    End Try

    'End Sub

    ''' <summary>
    ''' Launches another thread in order to visualize the Application is waiting for some operation is finished
    ''' </summary>
    ''' <param name="pTitleText"></param>
    ''' <param name="pWaitText"></param>
    ''' <remarks>Created by SGM 29/09/2011</remarks>
    Private Sub WaitControl(ByVal pTitleText As String, ByVal pWaitText As String)
        Try

            Me.ActivateActionButtonBar(False)
            Me.ActivateMenus(False)
            Me.Cursor = Cursors.WaitCursor

            Application.DoEvents()

            If Me.bsWaitControl.IsBusy Then
                If Me.bsWaitControl.WorkerSupportsCancellation Then
                    Me.bsWaitControl.CancelAsync()
                End If
                While Me.bsWaitControl.CancellationPending

                End While
            End If


            Me.ActivateActionButtonBar(False)
            Me.ActivateMenus(False)
            Me.Cursor = Cursors.WaitCursor

            Me.bsWaitControl.RunWorkerAsync()

            'DL 20/04/2012
            'Me.wfWaitScreen = New IAx00StartUp(Nothing) With {.Title = pTitleText, .WaitText = pWaitText}
            Me.wfWaitScreen = New WaitScreen(Nothing) With {.Title = pTitleText, .WaitText = pWaitText}
            'DL 20/04/2012            
            Me.wfWaitScreen.Show()

            'SGM 08/11/2012
            If RecoverRequested Then
                Me.wfWaitScreen.Focus()
            End If

            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".WaitControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 29/09/2011</remarks>
    Private Sub PrepareWaitingMode()
        Try
            If Me.ReconnectRequested Then
                While Me.isWaitingForConnected
                    Application.DoEvents()
                End While

            ElseIf Me.UserStandByRequested Then
                While Me.isWaitingForStandBy
                    Application.DoEvents()
                End While

            ElseIf Me.UserSleepRequested Then
                While Me.isWaitingForSleep
                    Application.DoEvents()
                End While

                ' XBC 23/10/2012
            ElseIf Me.RecoverRequested Then
                While Me.isWaitingForRecover
                    Application.DoEvents()
                End While
            End If



        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareWaitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWaitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    ''' <summary>
    ''' Load all screen labels and tooltips in the current application language
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XBC 10/11/2010 - Check if rest of required Labels and ToolTips have been already 
    '''                               created in BD and complete this function 
    ''' </remarks>
    Private Sub GetScreenLabels() 'PDT create texts
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Configuration
            Me.ConfigurationToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Configuration", CurrentLanguageAttribute)
            Me.GeneralToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_General", CurrentLanguageAttribute)
            Me.LanguageToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_LANGUAGE", CurrentLanguageAttribute)
            Me.UserToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_USERS", CurrentLanguageAttribute)
            Me.BarcodesConfigToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", CurrentLanguageAttribute)
            Me.ChangeUserToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CHANGE_USER", CurrentLanguageAttribute)

            'Adjustments & Tests
            Me.AdjustmentsTestsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_AdjustmentsTests", CurrentLanguageAttribute)
            Me.PositionsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_Positions", CurrentLanguageAttribute)
            Me.PhotometryToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Photometry", CurrentLanguageAttribute)
            Me.TankLevelsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_TankLevels", CurrentLanguageAttribute)
            Me.MotorsTestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_MotorsPumpsValves", CurrentLanguageAttribute)
            Me.ThermosTestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_Thermos", CurrentLanguageAttribute)
            Me.LevelDetectionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_LevelDetection", CurrentLanguageAttribute)
            Me.BarCodeToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BarCode", CurrentLanguageAttribute) 'dl 30/11/2012 SRV_MENU_BarCode
            Me.ISEModuleToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_ISE", CurrentLanguageAttribute)
            Me.ClotDetectionToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_ClotDetection", CurrentLanguageAttribute)
            Me.StressTestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_Stress", CurrentLanguageAttribute)

            'Utilities
            Me.UtilitiesToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Utilities", CurrentLanguageAttribute)
            'Me.CommunicationsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AnalyzerConfiguration", CurrentLanguageAttribute)
            'Me.UsersManagementToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Users_Details", CurrentLanguageAttribute)
            'Me.LanguageToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LangConfig", CurrentLanguageAttribute)
            Me.ConditioningToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Conditioning", CurrentLanguageAttribute)
            Me.DemoModeToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_DemoMode", CurrentLanguageAttribute)
            Me.AnalyzerInfoToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_AnalyzerInfo", CurrentLanguageAttribute)
            Me.HardwareToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_HardwareTool", CurrentLanguageAttribute)
            Me.InstrumentUpdateToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_InstrumentUpdate", CurrentLanguageAttribute)
            Me.SendableScriptsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_FwScripts", CurrentLanguageAttribute)
            Me.SettingsToolStripMenuItem.Text = "AUX - " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SwParameter", CurrentLanguageAttribute)
            Me.ChangeRotorToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_ChangeReactionsRotor", CurrentLanguageAttribute) 'SGM 12/11/2012

            'Registry and maintenance
            Me.RegistryMaintenanceToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Results", CurrentLanguageAttribute) ' DL 30/11/2012  "SRV_MENU_RegistryMaintenance", CurrentLanguageAttribute)
            Me.HistoryToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_HistoryReports", CurrentLanguageAttribute)

            'AG 20/10/2014 BA-1939 -point 5 change text in MDI menu
            'Me.SATReportsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_SATReports", CurrentLanguageAttribute)
            Me.SATReportsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SATReport", CurrentLanguageAttribute)

            Me.CycleCountsToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_CycleCounts", CurrentLanguageAttribute)
            Me.PreventiveMaintenanceToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_PreventiveMaintenance", CurrentLanguageAttribute)

            'Exit
            Me.ExitToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Exit", CurrentLanguageAttribute)
            Me.WithShutToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WithShutDown", CurrentLanguageAttribute)
            Me.WithOutShutDownToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_WithOutShutDown", CurrentLanguageAttribute)

            'Help
            Me.HelpToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Help", CurrentLanguageAttribute)
            Me.OperationGuideToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_OperationGuide", CurrentLanguageAttribute)
            Me.MaintenancePlanToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_MaintenancePlan", CurrentLanguageAttribute)
            Me.TroubleshooterToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_Troubleshooter", CurrentLanguageAttribute)
            Me.AboutToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_About", CurrentLanguageAttribute)
            Me.UserManualToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_UserManual", CurrentLanguageAttribute) 'DL 27/06/2013

            'Production
            Me.AutoTestToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_AutoTest", CurrentLanguageAttribute)
            Me.AutoAdjustmentToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_AutoAdjustment", CurrentLanguageAttribute)
            Me.AdjustmentProcedureGuideToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_AdjustmentProcedureGuide", CurrentLanguageAttribute)
            Me.HistoryRecordGenerationToolStripMenuItem.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_MENU_HistoryRecord", CurrentLanguageAttribute)

            'Other screen labels
            bsAnalyzerStatus.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", CurrentLanguageAttribute)
            InstrumentToolStripLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Instrument", CurrentLanguageAttribute)

            ' Tooltips
            Me.bsTSConnectButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Connect", CurrentLanguageAttribute)
            Me.bsTSShutdownButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Shutdown", CurrentLanguageAttribute)
            'Me.bsTSEmergencyButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_EmergencyStop", CurrentLanguageAttribute)
            Me.bsTSRecoverButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RecoverAnalyzer", CurrentLanguageAttribute) 'SGM 27/10/2012
            Me.bsTSWarningButton.ToolTipText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Alarms", CurrentLanguageAttribute) 'XBC 07/11/2012

            'Connected
            Me.ConnectedText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CONNECTED", CurrentLanguageAttribute)
            Me.NotConnectedText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NOT_CONNECTED", CurrentLanguageAttribute)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Opens AnalyzerInfo screen
    ''' </summary>
    ''' <remarks>Created by </remarks>
    Private Sub OpenAnalyzerInfoScreen()
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()

            MyClass.ActivateActionButtonBar(False)
            MyClass.ActivateMenus(False)
            Me.Cursor = Cursors.WaitCursor

            OpenMDIChildForm(AnalyzerInfo)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".OpenAnalyzerInfoScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    '''' <summary>
    '''' SEND_INFO_STOP()
    '''' </summary>
    '''' <remarks>Created by SGM 19/09/2011</remarks>
    'Private Sub DeactivateANSINFRefreshingMode()
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        If Not Me.SimulationMode Then
    '            'the INFO data refreshing mode must be deactivated before reading adjustments
    '            'SGM 19/09/2011
    '            If Me.MDIAnalyzerManager IsNot Nothing Then
    '                If Me.MDIAnalyzerManager.Connected Then

    '                    myGlobal = Me.MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, _
    '                                                         True, _
    '                                                         Nothing, _
    '                                                         GlobalEnumerates.Ax00InfoInstructionModes.STP)

    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".DeactivateANSINFRefreshingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".DeactivateANSINFRefreshingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Maintenance of the file log activity in order to don't grow much
    '''' </summary>
    '''' <remarks>
    '''' Created by XBC 25/07/2011
    '''' Modified by SGM 29/07/2011 add file is corrupt case
    '''' </remarks>
    'Private Sub MaintenanceLog()
    '    Try
    '        Dim myGlobal As New GlobalDataTO
    '        Dim sourcePath As String = Application.StartupPath & GlobalBase.XmlLogFilePath
    '        Dim OldXmlFile As String = sourcePath & GlobalBase.XmlLogFile
    '        Dim arrayFileName() As String = Split(GlobalBase.XmlLogFile, ".xml")
    '        Dim NewXmlFile As String = sourcePath & arrayFileName(0) & " " & Format(Now, "yyyyMMdd hhmmss") & ".xml"

    '        If (System.IO.File.Exists(OldXmlFile)) Then

    '            Dim MyFile As New FileInfo(OldXmlFile)
    '            Dim FileSize As Long = MyFile.Length


    '            If FileSize > MAX_SIZE_LOG Then
    '                Rename(OldXmlFile, NewXmlFile)

    '                'Dim myUtil As New Utilities.
    '                If (System.IO.File.Exists(sourcePath & "Temp")) Then
    '                    myGlobal = Utilities.RemoveFolder(sourcePath & "Temp")
    '                End If

    '                myGlobal = Utilities.CreateFolder(sourcePath & "Temp")
    '                If (Not myGlobal.HasError) Then
    '                    myGlobal = Utilities.MoveFiles(sourcePath, sourcePath & "Temp\", "AX00Log*.xml")

    '                    Dim fileZip As String = "PreviousLog.zip"
    '                    If (Not myGlobal.HasError) Then
    '                        If (System.IO.File.Exists(sourcePath & fileZip)) Then
    '                            myGlobal = Utilities.ExtractFromZip(sourcePath & fileZip, sourcePath & "Temp\")

    '                            If (Not myGlobal.HasError) Then
    '                                Kill(sourcePath & fileZip)
    '                            End If
    '                        End If


    '                        myGlobal = Utilities.CompressToZip(sourcePath & "Temp\", sourcePath & fileZip)
    '                        If (Not myGlobal.HasError) Then
    '                            myGlobal = Utilities.RemoveFolder(sourcePath & "Temp")
    '                        End If
    '                    End If
    '                End If
    '            End If

    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".MaintenanceLog ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub


#End Region

#Region "Autoconnect process"
    ''' <summary>
    ''' Start auto connection when application (MDI) is shown
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 28/07/2010</remarks>
    Private Sub BsAutoConnect_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BsAutoConnect.DoWork
        Try

            AutoConnectProcess = True

            ''SGM 28/09/2011
            'If me.MDIAnalyzerManager IsNot Nothing AndAlso me.MDIAnalyzerManager.IsUserConnectRequested Then
            '    Me.SEND_STANDBY()
            'Else
            '    Me.Connect()
            'End If

            ''end SGM 28/09/2011
            Me.isWaitingForConnected = True 'SGM 15/11/2011
            Me.IsAutoConnecting = True 'SGM 15/11/2011

            Me.Connect()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsAutoConnect_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAutoConnect_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' End of auto connection (MDI shown) process
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 28/07/2010</remarks>
    Private Sub BsAutoConnect_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BsAutoConnect.RunWorkerCompleted
        Try
            'RH 14/10/2010
            'Long story short: if you dropped a BGW on a form then everything is taken care of automatically, you don't have to help.
            'If you didn't drop it on a form then it isn't an element in a components collection and nothing needs to be done.
            'You don't have to call Dispose().
            'http://stackoverflow.com/questions/2542326/proper-way-to-dispose-of-a-backgroundworker
            'Also: http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/2e691893-8e01-43b1-94e7-a34d78b4012e
            'BsAutoConnect.Dispose()

            AutoConnectProcess = False

            If String.Compare(AutoConnectFailsTitle, "", False) <> 0 Then
                CreateLogActivity(AutoConnectFailsTitle & " - ErrorCode: " & AutoConnectFailsErrorCode, Me.Name & ".BsAutoConnect_DoWork", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ' XBC 25/10/2011 - message is shown in method OnManageSentEvent
                'Me.ShowMessage(AutoConnectFailsTitle, AutoConnectFailsErrorCode)
                Me.isWaitingForConnected = False 'SGM 16/11/2011
            End If

            'Me.Enabled = True 'AG 03/08/2010

            'only buttons bar and menus SGM 28/09/2011
            'Me.ActivateActionButtonBar(True)
            'Me.ActivateMenus(True)

            'AG 02/11/2010
            'If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
            '    Dim myAnalyzer As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            '    If Not myAnalyzer.Connected Then Me.ActivateActionButtonBar()
            'End If
            If Not MDIAnalyzerManager Is Nothing Then
                If MDIAnalyzerManager.Connected Then

                    ' XBC 22/11/2011 - is need to be able test in DEVELOPMENT_MODE = 2
                    If GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
                        Dim MyFormP As New FormXavi
                        MyFormP.Show()
                        MyFormP.Location = New Point(1000, 100)
                        Application.DoEvents()
                        Me.Location = New Point(100, 100)
                    End If
                    ' XBC 22/11/2011

                    ' XBC 27/04/2012
                    'MyClass.SEND_INFO_STOP()
                    ''DeactivateANSINFRefreshingMode()
                    'OpenAnalyzerInfoScreen()
                    If MyClass.SimulationMode Then
                        MyClass.SEND_INFO_STOP()
                        OpenAnalyzerInfoScreen()
                    End If
                    Me.CheckStress = False
                    Me.CheckAnalyzerInfo = False
                    Me.MDIAnalyzerManager.IsStressing = False
                    ' XBC 27/04/2012

                    'SGM 28/09/2011
                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, False, Me.ConnectedText)
                    If Not MyClass.SimulationMode Then

                        Me.ActivateActionButtonBar(True)
                        Me.ActivateMenus(True)

                    End If
                    Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString

                    'End SGM 28/09/2011
                Else
                    Me.MDIAnalyzerManager.ClearQueueToSend()   ' XBC 24/05/2011

                    'SGM 28/09/2011
                    Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 0, False, Me.NotConnectedText)
                    Me.ActivateActionButtonBar(True)
                    Me.ActivateMenus(True)
                    Me.bsAnalyzerStatus.Text = "NOT CONNECTED"
                    'End SGM 28/09/2011
                End If

            Else
                Me.ActivateActionButtonBar(True)
                Me.ActivateMenus(True)
            End If

            'Me.isWaitingForConnected = False 'SGM 15/11/2011    ' XBC 14/05/2012

            'END AG 02/11/2010

            ' XBC 25/07/2011
            'Me.MaintenanceLog()


            'Me.BsMonitor.Enabled = True

            Dim myConnected As Integer = 0
            Dim myConnectedText As String = Me.NotConnectedText
            If MDIAnalyzerManager.Connected Then
                myConnected = 1
                myConnectedText = Me.ConnectedText
                If MyClass.SimulationMode Then
                    Me.ReadAllFwAdjustments()
                    Application.DoEvents()
                    Me.SEND_INFO_START()
                Else

                    ' XBC 14/05/2012
                    '' XBC 27/04/2012
                    '' Check is Instrument is on Stress Mode
                    'System.Threading.Thread.Sleep(500)
                    'If Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                    '    If Not Me.CheckAnalyzerInfo Then
                    '        Me.CheckAnalyzerInfo = True
                    '        MyClass.SEND_INFO_STOP()
                    '        ''DeactivateANSINFRefreshingMode()
                    '        OpenAnalyzerInfoScreen()
                    '    End If
                    'Else
                    '    If Not Me.CheckStress Then
                    '        Me.CheckStress = True
                    '        MyClass.SEND_SDPOLL()
                    '    End If
                    'End If
                    '' XBC 27/04/2012
                    ' XBC 14/05/2012

                End If
            End If
            Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, myConnected, False, myConnectedText)
            Me.bsAnalyzerStatus.Text = myConnectedText.ToUpper
            'If Me.SimulationMode Then
            '    Me.ActivateActionButtonBar(True)
            '    Me.ActivateMenus(True)
            'End If

            'SGM 01/08/2011
            'Dim myLogMngr As New ApplicationLogManager()
            'myLogMngr.MaintenanceLog()

            'Me.Cursor = Cursors.Default SGM 17/11/2011

            ' XBC 16/11/2011 - Topmost functionality
            'Me.Focus() 'AG 07/09/2010
            ' XBC 16/11/2011 - Topmost functionality



            '            'SGM 25/11/2011
            '#If DEBUG Then
            '            If myConsole Is Nothing Then
            '                myConsole = New DebugTrace
            '                myConsole.Location = New Point(0, 0)
            '                myConsole.Show()
            '            End If
            '#End If
            '            'SGM 25/11/2011

            ' XBC 09/02/2012
            'Me.ControlBox = True

        Catch ex As Exception
            Me.ReconnectRequested = False
            Me.isWaitingForConnected = False
            CreateLogActivity(ex.Message, Me.Name & ".BsAutoConnect_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsAutoConnect_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 29/09/2011</remarks>
    Private Sub bsWaitStandBy_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bsWaitControl.DoWork
        Try

            If Me.ReconnectRequested Then
                Me.isWaitingForConnected = True

            ElseIf Me.UserStandByRequested Then
                Me.isWaitingForStandBy = True

            ElseIf Me.UserSleepRequested Then
                Me.isWaitingForSleep = True

                ' XBC 23/10/2012
            ElseIf Me.RecoverRequested Then
                Me.isWaitingForRecover = True
            End If

            Me.PrepareWaitingMode()

            'If Not me.MDIAnalyzerManager.Connected Then
            '    me.isWaitingForConnected = True
            '    me.PrepareConnectingMode()
            'Else
            '    Select Case me.MDIAnalyzerManager.AnalyzerStatus
            '        Case AnalyzerManagerStatus.SLEEPING
            '            me.isWaitingForStandBy = True
            '            me.PrepareWaitingStandbyMode()

            '        Case AnalyzerManagerStatus.STANDBY
            '            me.isWaitingForSleep = True
            '            me.PrepareWaitingSleepMode()
            '    End Select

            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsWaitStandBy_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsWaitStandBy_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 29/09/2011</remarks>
    Private Sub bsWaitStandBy_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bsWaitControl.RunWorkerCompleted
        Try
            Me.wfWaitScreen.Close()

            Application.DoEvents()

            If Me.ReconnectRequested Then
                Me.isWaitingForConnected = False
                Me.ReconnectRequested = False
            ElseIf Me.UserStandByRequested Then
                Me.isWaitingForStandBy = False
                Me.UserStandByRequested = False
            ElseIf Me.UserSleepRequested Then
                Me.isWaitingForSleep = False
                'Me.UserSleepRequested = False

                ' XBC 23/10/2012
            ElseIf Me.RecoverRequested Then
                Me.isWaitingForRecover = False
                Me.RecoverRequested = False
            End If

            Me.Cursor = Cursors.Default
            Me.ActivateActionButtonBar(True)
            Me.ActivateMenus(True)


            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".bsWaitStandBy_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsWaitStandBy_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub


#Region "Help Management"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>CREATED BY: SGM 11/11/2011</remarks>
    Public Sub SetHelpProvider()
        Try

            'Me.A400ServiceHelpProvider.HelpNamespace = GetHelpFilePath(HELP_FILE_TYPE.MANUAL, CurrentLanguageAttribute)

            'Me.A400ServiceHelpProvider.SetHelpNavigator(Me, HelpNavigator.TableOfContents)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".SetHelpProvider ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub
#End Region
#End Region

#Region "Events "

    Private Sub Ax00ServiceMainMDI_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Try
            'SGM 07/11/2012 - log Application End
            Static isLogged As Boolean

            If Not isLogged Then
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - Application END", "Ax00ServiceMainMDI_FormClosed", EventLogEntryType.Information, False)
                isLogged = True
            End If

            'end SGM 07/11/2012
        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "Ax00ServiceMainMDI_FormClosed", EventLogEntryType.Error, False)
        End Try
    End Sub

    ' DELETED - XBC 02/05/2011 - Prevents the form window from flickering when it is showed.
    '''' <summary>
    '''' When there is not an opened MDIChild Form, open the MONITOR screen
    '''' </summary>
    '''' <remarks>
    '''' Created by:
    '''' Modified by: SA 17/09/2010 - Changes due to new function OpenMDIChildForm
    '''' </remarks>
    '''' ' PDT !!!
    'Private Sub Ax00ServiceMainMDI_MdiChildActivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MdiChildActivate
    '    Try
    '        If (MdiChildren.Length = 0) Then
    '            'If there is not an MDI Child opened, open Monitor Screen
    '            OpenMDIChildForm(WSMonitor)

    '        ElseIf (MdiChildren.Length = 1) Then
    '            If (MdiChildren(0).Name <> "WSMonitor") AndAlso (ActiveMdiChild Is Nothing) Then
    '                'If there is an MDI Child opened, and it is not the Monitor, close it
    '                OpenMDIChildForm(WSMonitor)
    '            Else
    '                'Monitor Screen is already open, just Refresh it
    '                MdiChildren(0).Refresh()
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".Ax00MainMDI_MdiChildActivate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".Ax00MainMDI_MdiChildActivate ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Call the Connect process first time MDI is shown (by now it doesn't work)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' </remarks>
    Private Sub Ax00ServiceMainMDI_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            'Option1: Slow UI refresh
            'Me.Connect()

            'Option2: Use new thread - Obsolete method
            'AutoConnectProcess = New Thread(AddressOf me.Connect)
            'AutoConnectProcess.Start()
            'AutoConnectProcess.Name = "Connection Process"

            'Option3: Use Background worker control
            'Me.Enabled = False 'AG 03/08/2010

            If MyClass.FwScriptsLoadedOK Then
                'only buttons bar and menus SGM 28/09/2011
                Me.isWaitingForConnected = True
                Me.ActivateActionButtonBar(False)
                Me.ActivateMenus(False)

                'Me.BsMonitor.Enabled = False
                Application.DoEvents()
                Me.Cursor = Cursors.WaitCursor

                BsAutoConnect.RunWorkerAsync()
            Else
                Me.ControlBox = True
                Me.bsTSConnectButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Ax00ServiceMainMDI_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Ax00ServiceMainMDI_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Screen load event
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/06/2010 - Inform GlobalAnalyzerManager properties from the global Attributes
    ''' Modifiedb by AG 23/09/2011 - change GlobalAnalyzerManager for MDIAnalyzerManager
    ''' </remarks>
    Private Sub Ax00ServiceMainMDI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Try
            Me.Text = My.Application.Info.ProductName 'SGM 22/02/2012

            Dim XMLScriptFileNamePath As String = Application.StartupPath & GlobalBase.XmlFwScripts
            Me.ErrorStatusLabel.Text = ""

            'Search the current user level
            Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes
            Dim CurrentUserLevel As String = ""
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            'AG 13/01/2010 - Once we have the current user level we need found his numerical level
            Dim myUsersLevel As New UsersLevelDelegate
            Me.CurrentUserLevelAttribute = -1   'Initial value when NO userlevel exists

            If String.Compare(CurrentUserLevel, "", False) <> 0 Then  'When user level exists then find his numerical level
                myGlobal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
                If Not myGlobal.HasError Then
                    Me.CurrentUserLevelAttribute = CType(myGlobal.SetDatos, Integer)
                    MyClass.ManageMenuOptionsByUserLevel() 'SGM 25/05/2012
                End If
            End If

            Me.SimulationMode = False
            If Me.CurrentUserLevelAttribute = USER_LEVEL.lBIOSYSTEMS Then

                'dialogResultToReturn = ShowMessage(My.Application.Info.ProductName, Messages.SRV_WORK_MODE_QUESTION.ToString)

                'Top Most Message Box SGM 08/05/2012
                Dim myTopMostForm As New Form
                myTopMostForm.TopMost = True
                Dim myMsg As String = GetMessageText(Messages.SRV_WORK_MODE_QUESTION.ToString)
                dialogResultToReturn = MessageBox.Show(myTopMostForm, myMsg, My.Application.Info.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                'end 08/05/2012

                If dialogResultToReturn = Windows.Forms.DialogResult.No Then
                    Me.SimulationMode = True
                    GlobalConstants.REAL_DEVELOPMENT_MODE = 1
                End If
            End If


            'AG - Basic method to solve exceptions when differents threads access to the same control
            'See: http://www.elguille.info/NET/vs2005/trucos/acceder_a_un_control_desde_otro_hilo.htm
            'CheckForIllegalCrossThreadCalls = False

            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor

            BsStatusStrip1.BackColor = Color.WhiteSmoke

            PrepareXPSViewer() ' in order to the Information not to delay too long when first loading SGM 01/12/2011

            ' XBC 18/05/2012 - IS USED ???
            'PrepareButtons()

            'Get the current application Language to set the correspondent attribute and prepare all menu options
            'Dim currentLanguageGlobal As New GlobalBase
            CurrentLanguageAttribute = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            PrepareMenuOptions()    'AG 17/06/2010

            ' XBC 30/09/2011 - This call function is temporal because the Analyzer's Info must be obtained from the Instrument (POLLFW)
            'Get values for the different Screen Properties
            GetAnalyzerInfo() 'TR 12/04/2010
            ' XBC 30/09/2011 - So this part of the functionality is pending to be anulled

            'AG 22/04/0210 - Moved from BSBaseServiceForm_Load
            If (AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                ' XBC 02/03/2011
                'GlobalAnalyzerManager = New AnalyzerManager()
                MDIAnalyzerManager = New AnalyzerManager(My.Application.Info.AssemblyName, Me.AnalyzerModel) 'GlobalAnalyzerManager = New AnalyzerManager(My.Application.Info.AssemblyName, me.AnalyzerModel)
                ' XBC 02/03/2011
                MDIAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute 'GlobalAnalyzerManager.ActiveAnalyzer = AnalyzerIDAttribute

                MDIAnalyzerManager.ActiveFwVersion = FwVersionAttribute    ' XBC 08/06/2012

                Dim blnStartComm As Boolean = False
                blnStartComm = MDIAnalyzerManager.Start(False)   'AG 21/04/2010 Start the CommAx00 process'blnStartComm = GlobalAnalyzerManager.Start(False)   'AG 21/04/2010 Start the CommAx00 process

                'RH 14/10/2010 Fixed in LinkLayer!
                ''AG 22/04/2010 BUG to solve: In Ax00 we need to call twice this method (in Ax5 and iPRO it works at the first time)
                'If (Not blnStartComm) Then
                '    blnStartComm = GlobalAnalyzerManager.Start(False)
                'End If

                AppDomain.CurrentDomain.SetData("GlobalAnalyzerManager", MDIAnalyzerManager) 'AppDomain.CurrentDomain.SetData("GlobalAnalyzerManager", GlobalAnalyzerManager)
                'AG 23/09/2011
                'MDIAnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 02/11/2010

                If Not bsAnalyzerStatus Is Nothing Then
                    bsAnalyzerStatus.DisplayStyle = ToolStripItemDisplayStyle.Text
                    bsAnalyzerStatus.Text = "NOT CONNECTED"
                End If
            End If


            'FW Scripts Data
            myGlobal = Me.MDIAnalyzerManager.LoadAppFwScriptsData()
            If myGlobal.HasError Or myGlobal Is Nothing Then
                ' Loading Data Scripts failed !!! PDT !!! localització text !!!
                MyBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Ax00ServiceMainMDI_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.SRV_FWSCRIPTS_NOT_LOADED.ToString)
                Me.ErrorStatusLabel.Text = GetMessageText(myGlobal.ErrorCode)
                'Application.Exit()

            Else
                'SGM 02/02/2012
                MyClass.FwScriptsLoadedOKAttr = True
            End If

            'FW Adjustments Master Data
            myGlobal = Me.MDIAnalyzerManager.LoadFwAdjustmentsMasterData(MyClass.SimulationMode)
            If myGlobal.HasError Or myGlobal.SetDatos Is Nothing Then
                ' Loading Adjustments failed 
                MyBase.CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".Ax00ServiceMainMDI_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                MyBase.ShowMessage(Me.Name, GlobalEnumerates.Messages.SRV_FWADJ_MASTER_NOK.ToString)
                Me.ErrorStatusLabel.Text = GetMessageText(myGlobal.ErrorCode)
                'Application.Exit()

            Else
                'SGM 02/02/2012
                MyClass.FwAdjustmentsMasterDataLoadedOKAttr = True

                'SGM 25/11/2011 Load simulated Adjustments and export to File
                If MyClass.SimulationMode Then
                    Dim mySimulatedAdjustmentsDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If mySimulatedAdjustmentsDS IsNot Nothing Then
                        MyClass.myFwAdjustmentsDelegate = New FwAdjustmentsDelegate(mySimulatedAdjustmentsDS)
                        myGlobal = MyClass.myFwAdjustmentsDelegate.ExportDSToFile(Me.MDIAnalyzerManager.ActiveAnalyzer)
                    End If
                End If

            End If


            'SGM 13/04/11 MONITOR
            MyClass.InitializeMonitorPanel()
            MyClass.GetMonitorScreenLabels()
            Me.isWaitingForCloseApp = False

            'Me.ActivateActionButtonBar(True) SGM 15/11/2011

            ' XBC 17/05/2011
            If Me.SimulationMode Then
                'Me.bsMonitorSensorsButton.Visible = True
                MDIAnalyzerManager.ReadRegistredPorts()
            Else
                Me.bsMonitorSensorsButton.Visible = False
            End If

            'RH 03/11/2010
            bsDateTimer_Tick(Nothing, Nothing)
            bsDateTimer.Enabled = True

            ' XB 12/11/2013
            SetWSActiveDataFromDB()

            '' XB 14/11/2013
            'CleanApplicationLog()  ' Commented by now because no works fine with date time formats - pending to develop a functionality like Reset User Sw

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            CreateLogActivity(ex.Message, Me.Name & ".Ax00ServiceMainMDI_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Ax00ServiceMainMDI_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BorrameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BorrameToolStripMenuItem.Click
        Dim myTestForm As New TestForm(Me) 'Dim myTestForm As New TestForm(MyBase.GlobalAnalyzerManager)
        myTestForm.Show()
    End Sub

    Private Sub bsDateTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateTimer.Tick
        Try
            Me.bsTSDateLabel.Text = String.Format("{0} {1}", Today.ToShortDateString(), Now.ToShortTimeString())
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".TestsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TestsToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' users screen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>SGM 04/04/11</remarks>
    Private Sub UsersManagementToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'myConfigUsers = New IConfigUsers(Me)
            myConfigUsers = New IConfigUsers()
            OpenMDIChildForm(myConfigUsers)
            'Cursor = Cursors.Default

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UsersManagementToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UsersManagementToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Application closing method
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: AG 22/04/2010 
    ''' Modified by AG 17/06/2010: Ask confirmation and close current screen properly (Using the code of the property AcceptButton)
    ''' Modified by SG 22/06/2011: Unify the Closing Operation in a unique Method
    ''' </remarks>
    Private Sub ApplicationClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            If Not Me.IsFinalClosing Then

                ' XBC 05/06/2012
                If SkipQuestionExitProgram Then
                    Me.CloseApplication()
                    Exit Try
                End If

                'SGM 19/09/2012 - special case for ISE utilities screen
                Dim myISEUtilities As IISEUtilities = Nothing
                If TypeOf ActiveMdiChild Is IISEUtilities Then
                    myISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                    If myISEUtilities.IsCompletelyClosed Then
                        Me.isWaitingForCloseApp = True
                    End If
                End If
                'end SGM 19/09/2012

                If Not Me.isWaitingForCloseApp Then
                    If (ShowMessage(My.Application.Info.ProductName, GlobalEnumerates.Messages.EXIT_PROGRAM.ToString) = Windows.Forms.DialogResult.Yes) Then
                        Me.CloseApplication()

                        'SGM 19/09/2012 - special case for ISE utilities screen
                        If myISEUtilities IsNot Nothing Then
                            e.Cancel = True
                        End If
                        'end SGM 19/09/2012
                    Else
                        e.Cancel = True
                    End If
                Else
                    If myISEUtilities IsNot Nothing Then
                        If myISEUtilities.IsCompletelyClosed Then
                            Me.isWaitingForCloseApp = False
                            Me.CloseApplication()
                        End If
                        Exit Try
                    End If

                    e.Cancel = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " ApplicationClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    '''' <summary>
    '''' Open screen of Analyzer Configuration when click in the correspondent menu option
    '''' </summary>
    '''' <remarks>
    '''' SGM 04/04/11
    '''' </remarks>
    'Private Sub AnalyzerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        Cursor = Cursors.WaitCursor
    '        Application.DoEvents()
    '        'myConfigAnalyzers = New IConfigGeneral(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
    '        myConfigAnalyzers = New IConfigGeneral() With {.ActiveAnalyzer = AnalyzerIDAttribute}
    '        OpenMDIChildForm(myConfigAnalyzers)
    '        'Cursor = Cursors.Default
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Name & ".AnalyzerToolStripMenuItem_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".AnalyzerToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    Finally
    '        Cursor = Cursors.Default
    '    End Try

    'End Sub


    ''' <summary>
    ''' Open screen of Analyzer Configuration when click in the correspondent graphical button
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 06/07/2010 - Inform property for the AnalyzerID before open the Form
    ''' </remarks>
    Private Sub bsTSAnalysersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTSAnalysersButton.Click
        Try
            Application.DoEvents()
            'myConfigAnalyzers = New IConfigGeneral(Me) 'SG 03/12/10
            myConfigAnalyzers = New IConfigGeneral() 'RH 12/04/2012
            myConfigAnalyzers.ActiveAnalyzer = AnalyzerIDAttribute
            OpenMDIChildForm(myConfigAnalyzers)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsTSAnalysersButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTSAnalysersButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Open screen to configure Scripts communication Instructions
    ''' </summary>
    ''' <remarks>
    ''' Modified by: XBC 09/11/2010 - Changes due to new function OpenMDIChildForm
    ''' </remarks>
    Private Sub HeadPToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendableScriptsToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor

            Me.SEND_INFO_STOP()

            OpenMDIChildForm(FwScriptsEdition)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".HeadPToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open screen to configure Positions Adjustments
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 11/11/2010
    ''' </remarks>
    Private Sub PositionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PositionsToolStripMenuItem.Click
        Try
            ''DELETE
            'If Not Me.SimulationMode Then
            '    Dim MyFormP As New FormXavi
            '    MyFormP.Show()
            '    MyFormP.Location = New Point(1000, 100)
            '    Application.DoEvents()
            '    Me.Location = New Point(100, 100)
            'End If
            Application.DoEvents()

            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button


            ' XBC 02/11/2011 
            ' Commented because activate-deactivate funtionality is placed in a common region in SendFwScriptsDelegate
            'Me.SEND_INFO_STOP()


            Application.DoEvents()
            'Me.Location = New Point(100, 100)
            Me.Cursor = Cursors.WaitCursor



            OpenMDIChildForm(IPositionsAdjustments)


            Me.Text = My.Application.Info.ProductName & " - " & PositionsToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PositionsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open screen to configure Photometry Adjustments
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 15/02/2011
    ''' </remarks>
    Private Sub PhotometryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PhotometryToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IPhotometryAdjustments)

            Me.Text = My.Application.Info.ProductName & " - " & PhotometryToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PhotometryToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open screen to configure Tank levels Adjustments
    ''' </summary>
    ''' <remarks>
    ''' Created by: SGM 22/02/2011
    ''' </remarks>
    Private Sub TankLevelsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TankLevelsToolStripMenuItem.Click
        Try

            ' ''DELETE
            'If Not Me.SimulationMode Then
            '    Dim MyFormP As New FormXavi
            '    MyFormP.Show()
            '    MyFormP.Location = New Point(1000, 100)
            '    Application.DoEvents()
            '    Me.Location = New Point(100, 100)
            'End If
            Application.DoEvents()

            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Me.Cursor = Cursors.WaitCursor

            OpenMDIChildForm(ITankLevelsAdjustments)

            ''DELETE
            'Me.StartPosition = FormStartPosition.Manual
            'Me.Location = New Point(100, 100)

            Me.Text = My.Application.Info.ProductName & " - " & TankLevelsToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".TankLevelsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub



    ''' <summary>
    ''' Open screen to test Stress Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 22/03/2011
    ''' </remarks>
    Private Sub StressTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StressTestToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IStressModeTest)

            Me.Text = My.Application.Info.ProductName & " - " & StressTestToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StressTestToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub DemoModeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DemoModeToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IDemoMode)

            Me.Text = My.Application.Info.ProductName & " - " & DemoModeToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DemoModeToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub MotorsTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MotorsTestToolStripMenuItem.Click
        Try
            ' ''DELETE
            'If Not Me.SimulationMode Then
            '    Dim MyFormP As New FormXavi
            '    MyFormP.Show()
            '    MyFormP.Location = New Point(1000, 100)
            '    Application.DoEvents()
            '    Me.Location = New Point(100, 100)
            'End If
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IMotorsPumpsValvesTest)

            Me.Text = My.Application.Info.ProductName & " - " & MotorsTestToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".MotorsTestToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub AnalyzerInfoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalyzerInfoToolStripMenuItem.Click
        Try
            Me.OpenAnalyzerInfoScreen()

            Me.Text = My.Application.Info.ProductName & " - " & AnalyzerInfoToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".AnalyzerInfoToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub bsMonitorSensorsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsMonitorSensorsButton.Click
        Try
            Dim continousSimulation As Boolean = True
            Application.DoEvents()
            If Not ActiveMdiChild Is Nothing Then
                If (TypeOf ActiveMdiChild Is ITankLevelsAdjustments) Then
                    continousSimulation = False
                End If
            End If

            If continousSimulation Then
                Me.MonitorSensorsActive = Not Me.MonitorSensorsActive

                If Me.MonitorSensorsActive Then
                    Me.SimulateINFO_On()
                Else
                    Me.SimulateINFO_Off()
                End If

            Else
                Me.BsInfoTimer_Tick(Nothing, Nothing)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsMonitorSensorsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub ShowMonitorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim myGlobal As New GlobalDataTO

            'DELETE
            Dim MyFormP As New TestSimulatorAX00
            MyFormP.Visible = False
            MyFormP.Show()
            MyFormP.Location = New Point(1100, 100)
            MyFormP.Visible = True

            Application.DoEvents()



        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub MonitorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MonitorToolStripMenuItem.Click
        Try
            Me.BsInfoTimer.Enabled = True
        Catch ex As Exception
            Throw ex
        End Try
    End Sub



    Private Sub ThermosTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThermosTestToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IThermosAdjustments)

            Me.Text = My.Application.Info.ProductName & " - " & ThermosTestToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".MotorsTestToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub CycleCountsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CycleCountsToolStripMenuItem.Click
        Try
            ' ''DELETE
            'If Not Me.SimulationMode Then
            '    Dim MyFormP As New FormXavi
            '    MyFormP.Show()
            '    MyFormP.Location = New Point(1000, 100)
            '    Application.DoEvents()
            '    Me.Location = New Point(100, 100)
            'End If
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(ICycleCountScreen)

            Me.Text = My.Application.Info.ProductName & " - " & CycleCountsToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CycleCountsToolStripMenuItem_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Emergency button 'PDT
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 22/06/2011</remarks>
    Private Sub bsTSEmergencyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Dim myGlobal As New GlobalDataTO

        Try
            'PDT
            MessageBox.Show("Pending to define")

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".bsTSEmergencyButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("bsTSEmergencyButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 'PDT
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 22/06/2011</remarks>
    Private Sub WithOutShutDownToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WithOutShutDownToolStripMenuItem.Click

        Dim myGlobal As New GlobalDataTO
        Try
            Application.DoEvents()

            If Not Me.isWaitingForCloseApp Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.EXIT_PROGRAM.ToString) = Windows.Forms.DialogResult.Yes) Then
                    myGlobal = Me.CloseApplication()
                End If
            Else
                myGlobal = Me.CloseApplication()
            End If
            'If (ShowMessage(Me.Name, GlobalEnumerates.Messages.EXIT_PROGRAM.ToString) = Windows.Forms.DialogResult.Yes) Then
            '    myGlobal = Me.CloseApplication()
            'End If


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".WithOutShutDownToolStripMenuItem_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("WithOutShutDownToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub InstrumentUpdateToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InstrumentUpdateToolStripMenuItem.Click
        Try
            If MyClass.MDIAnalyzerManager.Connected Then
                If MyClass.MDIAnalyzerManager.IsFwSwCompatible Then
                    Me.OpenInstrumentUpdateToolScreen((MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING))
                    Me.Text = My.Application.Info.ProductName & " - " & InstrumentUpdateToolStripMenuItem.Text
                Else
                    If MyClass.MDIAnalyzerManager.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                        MyBase.ShowMessage(Me.Text, Messages.SRV_UPDATEFW_MUST_SLEEP.ToString)
                    Else
                        Me.OpenInstrumentUpdateToolScreen(True)
                        Me.Text = My.Application.Info.ProductName & " - " & InstrumentUpdateToolStripMenuItem.Text
                    End If

                End If
            Else
                Me.OpenInstrumentUpdateToolScreen(False)
                Me.Text = My.Application.Info.ProductName & " - " & InstrumentUpdateToolStripMenuItem.Text
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".AdjustmentsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub HistoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HistoryToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IHistoricalReports)

            Me.Text = My.Application.Info.ProductName & " - " & HistoryToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".HistoryToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub



    Private Sub WithShutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WithShutToolStripMenuItem.Click
        Try
            Dim myGlobal As New GlobalDataTO

            If Not Me.isWaitingForCloseApp Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.EXIT_PROGRAM.ToString) = Windows.Forms.DialogResult.Yes) Then
                    Me.Cursor = Cursors.WaitCursor

                    Me.isWaitingForCloseApp = True
                    myGlobal = Me.ShutDownAnalyzer()

                    Application.DoEvents()

                End If
            Else
                myGlobal = Me.ShutDownAnalyzer()

                Application.DoEvents()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".WithShutToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Me.Cursor = Cursors.Default

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by SGM 11/11/2011
    ''' Modified by XBC 23/05/2012
    ''' </remarks>
    Private Sub OperationGuideToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OperationGuideToolStripMenuItem.Click


        ''Help.ShowHelp(Me, GetHelpFilePath(HELP_FILE_TYPE.MANUAL_SRV, CurrentLanguageAttribute))

        'Dim proc As New Process()

        'With proc.StartInfo
        '    .Arguments = System.AppDomain.CurrentDomain.BaseDirectory() & "\HelpFiles\MS-BA400.pdf"
        '    .UseShellExecute = True
        '    .WindowStyle = ProcessWindowStyle.Maximized
        '    .WorkingDirectory = "C:\Program Files\Adobe\Reader 8.0\Reader\" '<----- Set Acrobat Install Path
        '    .FileName = "AcroRd32.exe" '<----- Set Acrobat Exe Name
        'End With

        'proc.Start()
        'proc.Close()
        'proc.Dispose()

        Try
            ' Get the Help File Path and Name.
            Dim myHelpPath As String = ""
            Dim myHelpFilesSettingDelegate As New HelpFilesSettingDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHelpFilesSettingDS As New HelpFilesSettingDS
            myGlobalDataTO = myHelpFilesSettingDelegate.Read(Nothing, HELP_FILE_TYPE.MANUAL_SRV, CurrentLanguageAttribute)

            If Not myGlobalDataTO.HasError Then
                myHelpFilesSettingDS = DirectCast(myGlobalDataTO.SetDatos, HelpFilesSettingDS)
                If myHelpFilesSettingDS.tfmwHelpFilesSetting.Count = 1 Then
                    myHelpPath = System.AppDomain.CurrentDomain.BaseDirectory() & _
                                    myHelpFilesSettingDS.tfmwHelpFilesSetting(0).HelpFileName

                    System.Diagnostics.Process.Start(myHelpPath)

                Else
                    ShowMessage("Warning", GlobalEnumerates.Messages.MANUAL_MISSING.ToString)
                End If
            End If


        Catch ex As Exception
            'CreateLogActivity(ex.Message, Name & ".OperationGuideToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)

            If DirectCast(ex, System.ComponentModel.Win32Exception).ErrorCode = -2147467259 Then
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                CreateLogActivity(ex.Message, Name & ".OperationGuideToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage("OperationGuideToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_ERROR_READER", CurrentLanguageAttribute))
            Else
                CreateLogActivity(ex.Message, Name & ".OperationGuideToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage("OperationGuideToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            End If

        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 11/11/2011</remarks>
    Private Sub MaintenancePlanToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MaintenancePlanToolStripMenuItem.Click
        Try
            'Pending
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".MaintenancePlanToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 11/11/2011</remarks>
    Private Sub TroubleshooterToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TroubleshooterToolStripMenuItem.Click
        Try
            'Pending
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".TroubleshooterToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub LevelDetectionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LevelDetectionToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(ILevelDetectionTest)

            Me.Text = My.Application.Info.ProductName & " - " & LevelDetectionToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".LevelDetectionToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub BarCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarCodeToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            OpenMDIChildForm(IBarCodeAdjustments)

            Me.Text = My.Application.Info.ProductName & " - " & BarCodeToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BarCodeToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ISEModuleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ISEModuleToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button

            ' XBC 05/02/2012 ISE Utilities placed into PresetationCOM layer
            'Application.DoEvents()
            'Me.Cursor = Cursors.WaitCursor
            'OpenMDIChildForm(IIseAdjustments)

            'myISEUtilities = New IISEUtilities(Me, Me.SimulationMode)
            myISEUtilities = New IISEUtilities(Me, Me.SimulationMode)
            OpenMDIChildForm(myISEUtilities)
            'Cursor = Cursors.Default
            ' XBC 05/02/2012 ISE Utilities placed into PresetationCOM layer

            Me.Text = My.Application.Info.ProductName & " - " & ISEModuleToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ISEModuleToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'mySettings = New ISettings(Me)
            mySettings = New ISettings()
            OpenMDIChildForm(mySettings)
            'Cursor = Cursors.Default

            Me.Text = My.Application.Info.ProductName & " - " & SettingsToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SettingsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Utility for change Rotor
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 12/11/2012</remarks>
    Private Sub ChangeRotorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeRotorToolStripMenuItem.Click
        Try

            myChangeRotor = New IChangeRotorSRV()
            OpenMDIChildForm(myChangeRotor)

            Me.Text = My.Application.Info.ProductName & " - " & ChangeRotorToolStripMenuItem.Text

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ChangeRotorToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    Private Sub BarcodesConfigToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BarcodesConfigToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'myConfigBarCode = New IBarCodesConfig(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
            myConfigBarCode = New IBarCodesConfig() With {.ActiveAnalyzer = AnalyzerIDAttribute, .WorkSessionID = WorkSessionIDAttribute}
            OpenMDIChildForm(myConfigBarCode)
            'Cursor = Cursors.Default

            Me.Text = My.Application.Info.ProductName & " - " & BarcodesConfigToolStripMenuItem.Text 'SGM 22/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BarcodesConfigToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ISEMonitorTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ISEMonitorTestToolStripMenuItem.Click
        Cursor = Cursors.WaitCursor
        Application.DoEvents()
        TestISEMonitor.SimulationMode = MyClass.SimulationMode
        OpenMDIChildForm(TestISEMonitor)
        Cursor = Cursors.Default
    End Sub

    Private Sub GeneralToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GeneralToolStripMenuItem.Click
        Try
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'myConfigAnalyzers = New IConfigGeneral(Me) With {.ActiveAnalyzer = AnalyzerIDAttribute}
            'myConfigAnalyzers = New IConfigGeneral() With {.ActiveAnalyzer = AnalyzerIDAttribute}
            myConfigAnalyzers = New IConfigGeneral(MyClass.AdjustmentsReaded) With {.ActiveAnalyzer = AnalyzerIDAttribute} 'SGM 21/11/2012
            OpenMDIChildForm(myConfigAnalyzers)
            'Cursor = Cursors.Default
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GeneralToolStripMenuItem_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GeneralToolStripMenuItem_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub UserToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserToolStripMenuItem.Click
        Try
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'myConfigUsers = New IConfigUsers(Me)
            myConfigUsers = New IConfigUsers()
            OpenMDIChildForm(myConfigUsers)
            'Cursor = Cursors.Default

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UserToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UserToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub UtilitiesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UtilitiesToolStripMenuItem.Click
        Try


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UtilitiesToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UtilitiesToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>SGM 25/05/2012</remarks>
    Private Sub ChangeUserToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeUserToolStripMenuItem.Click
        Try
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            myLogin = New IAx00Login(True)
            OpenMDIChildForm(myLogin)
            'Cursor = Cursors.Default
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".UserToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UserToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub LanguageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LanguageToolStripMenuItem.Click
        Try
            Cursor = Cursors.WaitCursor
            Application.DoEvents()
            'myLanguageConfig = New IConfigLanguage(Me)
            myLanguageConfig = New IConfigLanguage()
            OpenMDIChildForm(myLanguageConfig)
            'Cursor = Cursors.Default

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".LanguageToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

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
            myAbout.IsUser = False

            OpenMDIChildForm(myAbout)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AboutToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    Private Sub AdjustmentsTestsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AdjustmentsTestsToolStripMenuItem.Click
        Try
            If MyClass.SimulationMode Then
                Me.ISEModuleToolStripMenuItem.Enabled = True
            Else
                If Me.MDIAnalyzerManager.ISE_Manager IsNot Nothing Then
                    If Me.MDIAnalyzerManager.Connected And Not Me.MDIAnalyzerManager.ISE_Manager.IsISEInitiating OrElse Not Me.MDIAnalyzerManager.Connected Then
                        Me.ISEModuleToolStripMenuItem.Enabled = True
                    Else
                        Me.ISEModuleToolStripMenuItem.Enabled = False
                    End If
                Else
                    Me.ISEModuleToolStripMenuItem.Enabled = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AdjustmentsTestsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AdjustmentsTestsToolStripMenuItem_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region


#Region "Common Forms Returned Values"
    Private Sub LanguageConfig_Closed() Handles myLanguageConfig.FormClosed
        Try
            If myLanguageConfig.CurrentLanguage IsNot Nothing Then
                Me.CurrentLanguage = myLanguageConfig.CurrentLanguage

                MultilanguageResourcesDelegate.SetCurrentLanguage(CurrentLanguageAttribute)

                MyClass.GetMonitorScreenLabels() 'Update Monitor texts

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".LanguageConfig_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Set label title app to original state
    ''' </summary>
    ''' <remarks>Created by XBC 20/04/2012</remarks>
    Private Sub CommonForms_Closed2() Handles myConfigBarCode.FormClosed
        Try
            If ActiveMdiChild Is Nothing Then
                Me.Text = My.Application.Info.ProductName
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CommonForms_Closed2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Login Screen closed - manage User change
    ''' </summary>
    ''' <remarks>Created by SGM 25/05/2012</remarks>
    Private Sub Login_Closed() Handles myLogin.FormClosed
        Try
            Dim myGlobal As New GlobalDataTO
            'Dim myGlobalbase As New GlobalBase
            Dim myUsersLevel As New UsersLevelDelegate
            MyClass.CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            myGlobal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
            If Not myGlobal.HasError Then
                Me.CurrentUserLevelAttribute = CType(myGlobal.SetDatos, Integer)
                MyClass.ManageMenuOptionsByUserLevel()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".LanguageConfig_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Opens the WS Monitor form after the common form is closed.
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 08/02/2012
    ''' </remarks>
    Private Sub CommonForms_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles myISEUtilities.FormClosed
        Try
            If ActiveMdiChild IsNot Nothing AndAlso ActiveMdiChild.Tag Is Nothing Then 'It is been closed by Form Close button
                Me.OnManageActivateScreenEvent(True, Messages.STANDBY)
            End If

            '' XBC 20/04/2012
            'If Not Me.pScreenPendingToOpen Is Nothing Then
            '    'Everything is OK, open the selected screen
            '    Me.pScreenPendingToOpen.AnalyzerModel = Me.ActiveAnalyzerModel
            '    Me.pScreenPendingToOpen.MdiParent = Me
            '    Me.pScreenPendingToOpen.Show()
            '    Me.pScreenPendingToOpen = Nothing
            'End If
            '' XBC 20/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".CommonForms_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' General Config Screen closed
    ''' </summary>
    ''' <remarks>Created by SGM 25/05/2012</remarks>
    Private Sub GeneralConfig_Closed() Handles myConfigAnalyzers.FormClosed
        Try
            MDIAnalyzerManager.IsConfigGeneralProcess = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GeneralConfig_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Actions to do when ErrCodesForm is closed
    ''' </summary>
    ''' <remarks>Created by XBC 07/11/2012</remarks>
    Private Sub ErrCodesForm_Closed() Handles myErrCodesForm.FormClosed
        Try
            If Not MDIAnalyzerManager Is Nothing Then
                If MDIAnalyzerManager.ErrorCodesDisplay.Count > 0 Then
                    Me.bsTSWarningButton.Enabled = True
                Else
                    Me.bsTSWarningButton.Enabled = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ErrCodesForm_Closed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub


#End Region

#Region "Simulate Methods"

    Private Function ANSINF_Generator() As GlobalDataTO
        Const min As Single = 0
        Const max As Single = 4096
        Dim myGlobal As New GlobalDataTO
        Dim AnalyzerSensorNumericalValues As New Dictionary(Of GlobalEnumerates.AnalyzerSensors, Single)
        Dim myPercentValue As Single
        ''Dim myUtil As New Utilities.

        Try
            Dim Rnd As New Random()

            '1  GC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_GENERAL) = CInt(Rnd.Next(0, 2)) ' Now.Second Mod 30)

            '2  PC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_REACTIONS) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 6)

            '3  RC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_FRIDGE) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 12)

            '4  SC
            AnalyzerSensorNumericalValues(AnalyzerSensors.COVER_SAMPLES) = CInt(Rnd.Next(0, 2)) 'Now.Second Mod 4)

            '5  HS
            AnalyzerSensorNumericalValues(AnalyzerSensors.WATER_DEPOSIT) = CInt(Rnd.Next(0, 4)) 'Now.Second Mod 5)

            '6  WS
            AnalyzerSensorNumericalValues(AnalyzerSensors.WASTE_DEPOSIT) = CInt(Rnd.Next(0, 4)) 'Now.Second Mod 5)

            '7 SW
            AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_WASHSOLUTION) = CSng(Rnd.Next(0, 4096)) '1000 * (Now.Second / 60))

            '8 WW
            AnalyzerSensorNumericalValues(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) = CSng(Rnd.Next(0, 4096)) '1000 * (1 - (Now.Second / 60)))

            '9  PT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_REACTIONS) = CSng(Rnd.Next(35, 40)) '37.2 - Rnd(Now.Second))

            '10 FS
            AnalyzerSensorNumericalValues(AnalyzerSensors.FRIDGE_STATUS) = CSng(Rnd.Next(0, 2)) '1)

            '11 FT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_FRIDGE) = CSng(Rnd.Next(0, 45)) '15 + Rnd(Now.Second))

            '12 HT
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_WASHINGSTATION) = CSng(Rnd.Next(35, 55)) '27.6 + Rnd(Now.Second))

            '13 R1T
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R1) = CSng(Rnd.Next(42, 58)) '34.6 + Rnd(Now.Second))

            '14 R2T
            AnalyzerSensorNumericalValues(AnalyzerSensors.TEMPERATURE_R2) = CSng(Rnd.Next(42, 58)) '35.7 - Rnd(Now.Second))

            '15 IS
            AnalyzerSensorNumericalValues(AnalyzerSensors.ISE_STATUS) = CSng(Rnd.Next(0, 2)) '1)


            Dim myRefreshDS As New UIRefreshDS

            For S As Integer = 1 To 15 Step 1
                Dim mySensorId As AnalyzerSensors
                mySensorId = CType(S, AnalyzerSensors)
                If mySensorId <> Nothing Then

                    If mySensorId = AnalyzerSensors.BOTTLE_WASHSOLUTION Or _
                       mySensorId = AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE Then
                        myGlobal = Utilities.CalculatePercent(AnalyzerSensorNumericalValues(mySensorId), min, max)
                        If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                            myPercentValue = CType(myGlobal.SetDatos, Single)

                            myPercentValue = CInt(myPercentValue)
                            If myPercentValue > 100 Then myPercentValue = 100
                            If myPercentValue < 0 Then myPercentValue = 0

                            Me.BsMonitor.RefreshSensorValue(mySensorId.ToString, myPercentValue, True)
                        End If

                    Else
                        Me.BsMonitor.RefreshSensorValue(mySensorId.ToString, AnalyzerSensorNumericalValues(mySensorId), True)
                    End If


                    Dim myRefreshRow As UIRefreshDS.SensorValueChangedRow = myRefreshDS.SensorValueChanged.NewSensorValueChangedRow
                    With myRefreshRow
                        .BeginEdit()
                        .SensorID = mySensorId.ToString
                        .Value = AnalyzerSensorNumericalValues(mySensorId)
                        .EndEdit()
                    End With
                    myRefreshDS.SensorValueChanged.AddSensorValueChangedRow(myRefreshRow)
                End If
            Next

            myRefreshDS.AcceptChanges()

            'Me.BsMonitor.Enabled = True
            Me.BsMonitor.RefreshSensorValue(AnalyzerSensors.CONNECTED.ToString, 1, True, Me.ConnectedText)
            Me.bsAnalyzerStatus.Text = MDIAnalyzerManager.AnalyzerStatus.ToString

            myGlobal.SetDatos = myRefreshDS

        Catch ex As Exception
            Throw ex
        End Try

        Return myGlobal

    End Function

#End Region


#Region "Alarms Management"

    ''' <summary>
    ''' Manages the treatment of the GUI when an Alarm happens
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>
    ''' Created by SGM 19/10/2012
    ''' Modified by XBC 24/10/2012 - Add StopCurrentOperation call when AnalyzerInfo screen is hide
    ''' </remarks>
    Private Sub ManageAlarmStep1(ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes)
        Try
            If pAlarmType = ManagementAlarmTypes.NONE Then Exit Sub

            If pAlarmType <> ManagementAlarmTypes.OMMIT_ERROR Then

                'Stop current operation ---------------------------------------------------------
                If ActiveMdiChild Is Nothing Then

                    If IsAnalyzerInfoScreenRunning Then
                        For Each oForm As Form In Me.MdiChildren
                            If oForm Is AnalyzerInfo Then
                                Dim CurrentMdiChild As AnalyzerInfo = CType(oForm, AnalyzerInfo)
                                CurrentMdiChild.StopCurrentOperation(pAlarmType)
                                Exit For
                            End If
                        Next
                    Else
                        MyClass.StopCurrentOperation(pAlarmType)
                    End If

                Else

                    If (TypeOf ActiveMdiChild Is BSAdjustmentBaseForm) Then 'Service screens
                        Dim CurrentMdiChild As BSAdjustmentBaseForm = CType(ActiveMdiChild, BSAdjustmentBaseForm)
                        CurrentMdiChild.StopCurrentOperation(pAlarmType)

                    ElseIf (TypeOf ActiveMdiChild Is IConfigGeneral) Then 'General screen
                        Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                        CurrentMdiChild.StopCurrentOperation(pAlarmType)

                    ElseIf (TypeOf ActiveMdiChild Is IBarCodesConfig) Then 'Barcode config screen
                        Dim CurrentMdiChild As IBarCodesConfig = CType(ActiveMdiChild, IBarCodesConfig)
                        CurrentMdiChild.StopCurrentOperation(pAlarmType)

                    ElseIf (TypeOf ActiveMdiChild Is IISEUtilities) Then 'ISE Utilities screen
                        Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        CurrentMdiChild.StopCurrentOperation(pAlarmType)

                    Else
                        'other screens
                        MyClass.StopCurrentOperation(pAlarmType)

                    End If
                End If

                'exit and wait until Stop Operation is finished

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ManageAlarmStep1", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ManageAlarmStep1", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the treatment of the GUI when an Alarm happens
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Sub ManageAlarmStep2(ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes)
        Try
            If pAlarmType = ManagementAlarmTypes.NONE Then Exit Sub

            If pAlarmType <> ManagementAlarmTypes.OMMIT_ERROR Then

                ' XBC 24/10/2012 TO DELETE !!!
                ''0 - Close AnalyzerInfo if it exists ----------------------------------------------------
                'If IsAnalyzerInfoScreenRunning Then
                '    For Each oForm As Form In Me.MdiChildren
                '        If oForm Is AnalyzerInfo Then
                '            Dim CurrentMdiChild As AnalyzerInfo = CType(oForm, AnalyzerInfo)
                '            CurrentMdiChild.Close()
                '            IsAnalyzerInfoScreenRunning = False
                '            Exit For
                '        End If
                '    Next
                'End If

                '1 - Display alarm message --------------------------------------------------------------
                MyClass.ShowAlarmOrSensorsWarningMessages(pAlarmType)

                '2 - final treatment acording to the alarm type ------------------------------------------
                MyClass.AlarmFinalTreatment(pAlarmType)

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ManageAlarmStep2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ManageAlarmStep2", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Shows the message corresponding to the occurred Alarm type
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Private Sub ShowAlarmOrSensorsWarningMessages(ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim myMessage As String = ""
            Dim myMessages As List(Of String) = Nothing

            Dim myErrorsString As String = MyClass.MDIAnalyzerManager.ErrorCodes

            Select Case pAlarmType
                Case ManagementAlarmTypes.UPDATE_FW : myMessage = Messages.FW_UPDATE.ToString
                Case ManagementAlarmTypes.FATAL_ERROR : myMessage = Messages.FREEZE_GENERIC_RESET.ToString
                Case ManagementAlarmTypes.RECOVER_ERROR
                    myMessages = New List(Of String)
                    myMessages.Add(Messages.FREEZE_GENERIC0.ToString)
                    myMessages.Add("")
                    myMessages.Add(Messages.FREEZE_GENERIC2.ToString)

                Case ManagementAlarmTypes.SIMPLE_ERROR
                    If myErrorsString.Contains("550") Then 'SGM 07/11/2012 Reactions Rotor missing
                        myMessage = Messages.ROTOR_MISSING_WARN.ToString
                        myErrorsString &= vbCrLf & vbCrLf & myMultiLangResourcesDelegate.GetResourceText(Nothing, "SOL_REACT_MISSING_ERR", MyClass.CurrentLanguageAttribute)
                        MyClass.MDIAnalyzerManager.IsServiceRotorMissingInformed = True
                    Else
                        If mySpecialSimpleErrors IsNot Nothing AndAlso mySpecialSimpleErrors.Count > 0 Then
                            myMessages = mySpecialSimpleErrors
                        Else
                            myMessage = Messages.FREEZE_GENERIC_AUTO.ToString
                        End If
                    End If


            End Select

            If myMessage.Length > 0 Or myMessages IsNot Nothing Then

                'Get current Alarms Codes


                If myMessages Is Nothing Then
                    MyBase.ShowMessage(My.Application.Info.ProductName, myMessage, "", Me, Nothing, myErrorsString)
                ElseIf myMessages.Count > 0 Then
                    MyBase.ShowMultipleMessage(My.Application.Info.ProductName, myMessages, Me, Nothing, myErrorsString)
                End If

                MyClass.MDIAnalyzerManager.IsServiceAlarmInformed = False

            End If



        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ShowAlarmOrSensorsWarningMessages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowAlarmOrSensorsWarningMessages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Performs the final actions according to the current Alarm type.
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 18/10/2012</remarks>
    Private Sub AlarmFinalTreatment(ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes)
        Try
            Me.isWaitingForConnected = False
            Me.ReconnectRequested = False
            Me.isWaitingForStandBy = False
            Me.UserStandByRequested = False
            Me.isWaitingForSleep = False
            Me.UserSleepRequested = False
            Me.isWaitingForRecover = False
            Me.RecoverRequested = False

            If pAlarmType = ManagementAlarmTypes.UPDATE_FW Then

                MyClass.ActivateMenus(False)
                MyClass.ActivateActionButtonBar(False)

                ''Show Fw Update
                'If ActiveMdiChild Is Nothing OrElse Not TypeOf (ActiveMdiChild) Is IInstrumentUpdateUtil Then
                '    Me.OpenInstrumentUpdateToolScreen(True)
                '    Me.Text = My.Application.Info.ProductName & " - " & InstrumentUpdateToolStripMenuItem.Text
                'End If

                MyClass.IsAutoConnecting = False
                MyClass.isWaitingForConnected = False
                MyClass.isWaitingForStandBy = False
                MyClass.isWaitingForSleep = False
                MyClass.isWaitingForAdjust = False
                MyClass.UpdateFirmwareProcessEnd = False
                MyClass.MDIAnalyzerManager.IsFwReaded = True
                MyClass.MDIAnalyzerManager.IsFwSwCompatible = False
                MyClass.ActivateMenus(True)
                MyClass.ActivateActionButtonBar(True)

                Me.BsMonitor.DisableAllSensors() 'SGM 07/11/2012

            ElseIf pAlarmType = ManagementAlarmTypes.FATAL_ERROR Then

                MyClass.ActivateMenus(False)
                MyClass.ActivateActionButtonBar(False)

                Me.BsMonitor.DisableAllSensors() 'SGM 07/11/2012

                'Close current screen 'MDI
                For Each ChildForm As BSBaseForm In Me.MdiChildren
                    Using ChildForm
                        ChildForm.Close()
                    End Using
                Next

            ElseIf pAlarmType = ManagementAlarmTypes.RECOVER_ERROR Then

                'Me.BsMonitor.DisableAllSensors() 'SGM 07/11/2012

                'Close current screen 'MDI
                For Each ChildForm As BSBaseForm In Me.MdiChildren
                    Using ChildForm
                        ChildForm.Close()
                    End Using
                Next

                'Enable Recover Button
                Me.bsTSRecoverButton.Enabled = True


            ElseIf pAlarmType = ManagementAlarmTypes.SIMPLE_ERROR Then

                MyClass.ActivateMenus(True)
                MyClass.ActivateActionButtonBar(True)

                ''SGM 05/11/2012
                'If MyClass.MDIAnalyzerManager.IsInfoRequestPending Then
                '    MyClass.SEND_INFO_START()
                'End If

            End If

            'In case of ShutDown close progress screen
            If Me.UserSleepRequested Or MyClass.isWaitingForSleep = False Then
                ShutDownisPending = False

                Me.UserSleepRequested = False
                If Me.wfWaitScreen IsNot Nothing Then
                    Me.wfWaitScreen.Close()
                End If
                MyClass.isWaitingForSleep = False
            End If

            Me.Cursor = Cursors.Default
            Me.ControlBox = True


        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".AlarmFinalTreatment", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AlarmFinalTreatment", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Sends the instruction for recovering the Analyzer
    ''' </summary>
    ''' <remarks>Created by SGM 18/10/2012</remarks>
    Private Sub RecoverInstrument()
        Try

            Dim myGlobal As New GlobalDataTO
            myGlobal = MDIAnalyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop SENSOR information instruction

            If Not myGlobal.HasError Then
                If MDIAnalyzerManager.Connected Then

                    Me.BsMonitor.DisableAllSensors() 'SGM 08/11/2012
                    Me.bsTSRecoverButton.Enabled = False

                    'Reset all analyzer flags
                    Dim myFlags As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlags.ResetFlags(Nothing, AnalyzerIDAttribute)
                    If Not myGlobal.HasError Then

                        'Clear all flags before recover
                        MDIAnalyzerManager.InitializeAnalyzerFlags(Nothing)
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS"
                        MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                        Me.bsAnalyzerStatus.Text = "RECOVER" 'TODO get message text
                        'ShowStatus(Messages.RECOVERING_INSTRUMENT)

                        myGlobal = MDIAnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.RECOVER, True)

                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            If Not MDIAnalyzerManager.Connected Then
                                MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = ""
                                Me.wfWaitScreen.Focus() 'SGM 08/11/2012
                            End If

                        ElseIf myGlobal.HasError Then
                            ShowMessage("Error", myGlobal.ErrorMessage)
                            Me.bsTSRecoverButton.Enabled = True
                            ScreenWorkingProcess = False
                            Me.RecoverRequested = False
                            Me.isWaitingForRecover = False
                            Me.bsAnalyzerStatus.Text = ""
                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".RecoverInstrument", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RecoverInstrument", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
            ScreenWorkingProcess = False
            Me.RecoverRequested = False
            Me.isWaitingForRecover = False
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Private Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'TODO

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Private Sub StopCurrentOperation(Optional ByVal pAlarmType As GlobalEnumerates.ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'SGM 22/10/2012
            MyClass.PrepareErrorMode(pAlarmType)


            MyClass.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Manages the operation stop of common screens
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>SGM 25/10/2012</remarks>
    Private Sub On_CommonScreenStopOperationFinished(ByVal pAlarmType As ManagementAlarmTypes) Handles myConfigAnalyzers.StopCurrentOperationFinished, _
                                                                                                myConfigBarCode.StopCurrentOperationFinished, _
                                                                                                myISEUtilities.StopCurrentOperationFinished
        Try

            MyClass.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".On_CommonScreenStopOperationFinished ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".On_CommonScreenStopOperationFinished ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub On_ConfigGeneralIsClosed(ByVal sender As Object) Handles myConfigAnalyzers.FormIsClosed
        Try
            Dim isSleeping As Boolean = (Me.MDIAnalyzerManager.Connected AndAlso Me.MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING)

            MyClass.ActivateMenus(True, False, isSleeping)
            MyClass.ActivateActionButtonBar(True, False, isSleeping)

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".On_ConfigGeneralIsClosed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".On_ConfigGeneralIsClosed ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' stop current operation when Connection loss
    ''' </summary>
    ''' <remarks>SGM 25/10/2012</remarks>
    Private Sub ManageConnectionLossAlarm()
        Try

            If ActiveMdiChild Is Nothing Then

                MyClass.StopCurrentOperation()

            Else
                If TypeOf (ActiveMdiChild) Is BSAdjustmentBaseForm Then
                    Dim CurrentMdiChild As BSAdjustmentBaseForm = CType(ActiveMdiChild, BSAdjustmentBaseForm)
                    CurrentMdiChild.StopCurrentOperation()

                ElseIf TypeOf (ActiveMdiChild) Is IConfigGeneral Then
                    Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                    CurrentMdiChild.StopCurrentOperation()

                ElseIf TypeOf (ActiveMdiChild) Is IBarCodesConfig Then
                    Dim CurrentMdiChild As IBarCodesConfig = CType(ActiveMdiChild, IBarCodesConfig)
                    CurrentMdiChild.StopCurrentOperation()

                ElseIf TypeOf (ActiveMdiChild) Is IISEUtilities Then
                    Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                    CurrentMdiChild.StopCurrentOperation()

                End If
            End If

            'end SGM 25/10/2012
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub
#End Region





    ''' <summary>
    ''' Open Acrobat In One Button Click Using VB.Net {No API Or Other Ocx}
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/11/2012 
    ''' </remarks>    
    Private Sub UserManualToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserManualToolStripMenuItem.Click
        Try
            ' Get the Help File Path and Name.
            Dim myHelpPath As String = ""
            Dim myHelpFilesSettingDelegate As New HelpFilesSettingDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHelpFilesSettingDS As New HelpFilesSettingDS
            myGlobalDataTO = myHelpFilesSettingDelegate.Read(Nothing, HELP_FILE_TYPE.MANUAL_SRV, CurrentLanguageAttribute)

            If Not myGlobalDataTO.HasError Then
                myHelpFilesSettingDS = DirectCast(myGlobalDataTO.SetDatos, HelpFilesSettingDS)
                If myHelpFilesSettingDS.tfmwHelpFilesSetting.Count = 1 Then
                    myHelpPath = System.AppDomain.CurrentDomain.BaseDirectory() & _
                                    myHelpFilesSettingDS.tfmwHelpFilesSetting(0).HelpFileName

                    System.Diagnostics.Process.Start(myHelpPath)

                Else
                    ShowMessage("Warning", GlobalEnumerates.Messages.MANUAL_MISSING.ToString)
                End If
            End If

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

    End Sub

    Private Sub SATReportsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SATReportsToolStripMenuItem.Click
        Try
            'This is the way a form should be called from the main window
            'Call the function OpenMDIChildForm and pass it as parameter the form name you want to open
            'The form to be opened should be assigned its AcceptButton property to its default exit button
            Application.DoEvents()
            Me.Cursor = Cursors.WaitCursor
            Dim mySATReport As ISATReportSRV = New ISATReportSRV '(MyClass.MDIAnalyzerManager) 'SGM 25/11/2011
            OpenMDIChildForm(mySATReport)

            Me.Text = My.Application.Info.ProductName & " - " & SATReportsToolStripMenuItem.Text 'SGM 22/02/2012


        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SATReportsToolStripMenuItem_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub
End Class