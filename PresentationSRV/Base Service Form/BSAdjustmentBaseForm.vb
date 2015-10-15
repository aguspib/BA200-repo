Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports System.IO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.DAL

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports DevExpress.XtraEditors
Imports Biosystems.Ax00.App


Public Class BSAdjustmentBaseForm
    Inherits BSBaseForm

#Region "Declarations"

    Protected Friend myServiceMDI As Ax00ServiceMainMDI

    'Protected Friend myAnalyzerManager As AnalyzerManager '#REFACTORING 
    Public WithEvents myFwScriptDelegate As SendFwScriptsDelegate 'delegate for sending/receiving to/from the Communications Layer
    Public WithEvents myBaseScreenDelegate As BaseFwScriptDelegate
    Protected Friend myAllAdjustmentsDS As SRVAdjustmentsDS 'SGM 27/01/11
    'Protected Friend SelectedAdjustmentsDS As SRVAdjustmentsDS 'SGM 22/09/2011
    Protected Friend myAdjustmentsDelegate As FwAdjustmentsDelegate 'SGM 27/01/11


    Protected Friend CurrentUserNumericalLevel As Integer = -1   'Initial value when NO userlevel exists

    'Monitor/Sensors 
    'Protected Friend mySensorsDelegate As SRVSensorsDelegate 'SGM 21/03/11

    'DS
    'Private myAllSensorsDS As New SRVSensorsDS 'SGM 21/03/11
    'Protected Friend ScreenSensorsDS As New SRVSensorsDS 'SGM 28/02/11
    'Protected Friend MonitorSensorsDS As New SRVSensorsDS 'SGM 28/02/11
    'Private RequestedSensorsDS As New SRVSensorsDS 'SGM 22/03/11

    'LIST
    'Private myAllSensors As New ServiceSensors 'SGM 24/03/11
    'Protected Friend WithEvents ScreenSensors As New ServiceSensors 'SGM 24/03/11
    'Protected Friend WithEvents MonitorSensors As New ServiceSensors 'SGM 24/03/11
    'Private RequestedSensors As New ServiceSensors 'SGM 24/03/11

    '??
    'Protected Friend WithEvents SensorsTimer As New Timer()


    'Private mySensorsFwScriptDelegate As SendFwScriptsDelegate
    'Private WithEvents mySensorsScreenDelegate As BaseFwScriptDelegate

    Protected Friend SimulationProcessTime As Integer = 500

    Private myCurrentControl As String  ' XBC 10/10/2011

    Private CloseRequestedByMDIAttr As Boolean = False 'SGM 28/03/2012 when user has accepted to close the application from MDI
    Private CloseWithShutDownRequestedByMDIAttr As Boolean = False
    Private CloseWithoutShutDownRequestedByMDIAttr As Boolean = False

    'SGM 04/06/2012
    Protected Friend ProgressBar As MarqueeProgressBarControl

#End Region

#Region "Constructor"

   

    Protected Friend Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If (AnalyzerController.IsAnalyzerInstantiated) Then '#REFACTORING
            MyClass.myFwScriptDelegate = New SendFwScriptsDelegate()

            'SGM 02/03/11
            MyClass.myBaseScreenDelegate = New BaseFwScriptDelegate()
            MyClass.myBaseScreenDelegate.myFwScriptDelegate = MyClass.myFwScriptDelegate

            'SGM 27/01/11 ADJUSTMENTS
            MyClass.RequestAdjustmentsMasterData()

            ''SGM 21/03/11 SENSORS
            ''MyClass.LoadSensorsMasterData()
            'MyClass.mySensorsFwScriptDelegate = New SendFwScriptsDelegate(myAnalyzerManager)
            'MyClass.mySensorsScreenDelegate = New BaseFwScriptDelegate()
            'MyClass.mySensorsScreenDelegate.myFwScriptDelegate = MyClass.mySensorsFwScriptDelegate

        End If
        If (AnalyzerController.IsAnalyzerInstantiated) Then
            MyClass.myFwScriptDelegate = New SendFwScriptsDelegate() '#REFACTORING
        End If

    End Sub
#End Region


#Region "Layout Structures"
    'structure data that defines the items involved in the navigation logic

    ''' <summary>
    ''' Information Panel layout
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure InfoLayout
        Public Container As Panel
        Public InfoTitle As Label
        Public InfoText As Label
        Public InfoXPS As BsXPSViewer
        Public InfoExpandButton As Panel

        ''QUITAR?
        'Public InfoVideo As AxWMPLib.AxWindowsMediaPlayer 'needed?
        'Public InfoPDF As WebBrowser  'needed?
        'Public InfoRTF As RichTextBox  'needed?

    End Structure

    ''' <summary>
    ''' Each Adjust Area Panel layout
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure SpecificAdjustAreaLayout
        Public Container As Control
        Public AdjustControls As List(Of BSAdjustControl)
    End Structure

    ''' <summary> Adjust Panel ( All Adjust Areas)</summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure AdjustmentAreaLayout
        Public Container As Panel
        Public AdjustAreas As List(Of SpecificAdjustAreaLayout)
    End Structure

    ''' <summary>Adjustment Panel (Info + Adjust)</summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure AllAdjustmentsLayout
        Public Container As Panel
        Public InfoPanel As InfoLayout
        Public AdjustPanel As AdjustmentAreaLayout
    End Structure

    ''' <summary>
    ''' Buttons Panel
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure ButtonsLayout
        Public Container As Panel
        Public AdjustButton As Button
        Public SaveButton As Button
        Public CancelButton As Button
        Public ExitButton As Button
        Public TestButton As Button
    End Structure

    ''' <summary>
    ''' Messages Panel
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure MessagesLayout
        Public Container As Panel
        Public Icon As PictureBox
        Public Label As Label
    End Structure

    ''' <summary>
    ''' Monitor Panel
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure MonitorLayout
        Public Monitor As BSMonitorPanel
    End Structure

    ''' <summary>
    ''' Global Screen Layout (Adjustment + Messages + Buttons)
    ''' </summary>
    ''' <remarks>Created by SG 04/01/11</remarks>
    Protected Friend Structure ScreenLayout
        Public AdjustmentPanel As AllAdjustmentsLayout
        Public MessagesPanel As MessagesLayout
        Public ButtonsPanel As ButtonsLayout
        Public MonitorPanel As MonitorLayout
        Public InfoPanel As InfoLayout
    End Structure
#End Region

#Region "Flags"
    'Protected Friend IsWaitingForResponse As Boolean 'PENDING QUITAR CUANDO SE IMPLEMENTE LO DE LOS SENSORES
    'Protected Friend SimulationMode As Boolean
    'Protected Friend AdjustmentsReaded As Boolean
    Protected Friend AdjustmentsMasterDataLoaded As Boolean = False 'SGM 27/01/11
    Protected Friend AdjustmentsObtainedFromAnalyzer As Boolean = False 'SGM 27/01/11

    Protected Friend AdjControlTabRequested As Boolean = False
    Protected Friend AdjControlBackTabRequested As Boolean = False
#End Region

#Region "Inheritable Properties"

    Protected Friend ReadOnly Property SimulationMode() As Boolean
        Get
            If GlobalConstants.REAL_DEVELOPMENT_MODE = 1 Then
                SimulationModeAttr = True ' Simulation mode
            ElseIf GlobalConstants.REAL_DEVELOPMENT_MODE = 2 Then
                SimulationModeAttr = False ' Developer mode
            Else
                SimulationModeAttr = False ' Real mode
            End If

            Return SimulationModeAttr
        End Get
    End Property


    Protected Friend Overridable Property CurrentMode() As ADJUSTMENT_MODES
        Get
            Return CurrentModeAttr
        End Get
        Set(ByVal value As ADJUSTMENT_MODES)
            If CurrentModeAttr <> value Then
                CurrentModeAttr = value
            End If
        End Set
    End Property
#End Region

#Region "Attributes"
    Private SimulationModeAttr As Boolean = False
    Private CurrentModeAttr As ADJUSTMENT_MODES
#End Region

#Region "Common Event Handlers"

    Protected Friend Sub MyBase_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles Me.FormClosed
        MyClass.myFwScriptDelegate = Nothing
    End Sub

    ''' <summary>
    ''' Common Load event handler
    ''' All the BSAdjustment Controls are collected and the events addressed
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Protected Friend Sub MyBase_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Try
            Cursor = Cursors.WaitCursor 'SGM 15/11/2012

            'assign MDI parent
            MyClass.myServiceMDI = CType(MyBase.MdiParent, Ax00ServiceMainMDI)
            'If MyClass.myServiceMDI IsNot Nothing Then
            '    MyClass.myAnalyzerManager = myServiceMDI.MDIAnalyzerManager
            'End If

            'MyClass.myFwScriptDelegate = New SendFwScriptsDelegate(myAnalyzerManager)

            ''SGM 02/03/11
            'MyClass.myBaseScreenDelegate = New BaseFwScriptDelegate()
            'MyClass.myBaseScreenDelegate.myFwScriptDelegate = MyClass.myFwScriptDelegate
            myServiceMDI.AnalyzerModel = AnalyzerController.Instance.Analyzer.Model
            myServiceMDI.ActiveAnalyzer = AnalyzerController.Instance.Analyzer.ActiveAnalyzer

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " MyBase_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Communication Events"
    'these methods are only invoked from the child screens on communication events

    Protected Friend Sub OnReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) 'Handles myScriptDelegate.LastScriptResponseEvent
        Try

            ''monitor modes
            'Select Case MyClass.CurrentSensorsMode
            '    Case SENSORS_MODES.MONITOR_OFF

            '    Case SENSORS_MODES.MONITOR_PAUSED

            '    Case SENSORS_MODES.MONITOR_REQUEST
            '        If pResponse = RESPONSE_TYPES.OK Then
            '            MyClass.CurrentSensorsMode = SENSORS_MODES.MONITOR_RECEIVED
            '            RaiseEvent SensorsDataReceived()
            '            Exit Sub

            '        ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
            '            MyClass.ErrorMode()
            '        End If

            '    Case SENSORS_MODES.MONITOR_RECEIVED


            'End Select

            'adjustment modes
            Me.RefreshModes(pResponse)


            MyClass.PrepareCommonAreas()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnManageReceptionFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    ' XBC 28/10/2011 - Is not used ???
    'Protected Friend Sub OnDataReceivedFromAnalyzer(ByVal ptreated As Boolean) 'Handles myScriptDelegate.DataReceivedEvent
    '    Try

    '        'every data from the analyzer
    '        'it can be manage as needed

    '        If ptreated Then
    '            Me.RefreshModes(RESPONSE_TYPES.OK)
    '        End If

    '    Catch ex As Exception
    '        'Dim myLogAcciones As New ApplicationLogManager()
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".OnDataReceivedFromAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '    End Try
    'End Sub

#End Region

#Region "Key Events Managing"

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by SG 03/01/11</remarks>
    'Private Sub MyBase_Keydown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
    '    Try
    '        If Not KeyPressed And Not FwActionRequested Then
    '            Select Case e.KeyCode
    '                Case Keys.Home, Keys.Escape, Keys.Enter
    '                    KeyPressed = True
    '            End Select

    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MyBase_Keydown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        Myclass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' THE TAB, UP,DOWN,LEFT AND RIGHT ARROWS CANNOT BE CATCHED BY KEYDOWN EVENTS
    '''' </summary>
    '''' <remarks>Created by SG 03/01/11</remarks>
    'Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean

    '    Dim handled As Boolean = False

    '    Try
    '        'If myFocusedAdjustControl IsNot Nothing And Not Me.KeyPreview Then
    '        '    myFocusedAdjustControl.Focus()
    '        'End If
    '        If keyData = 65552 Then 'SHIFT
    '            ShiftPressed = True
    '            handled = True
    '        End If

    '        If Not KeyPressed And Not FwActionRequested Then

    '            If ShiftPressed And keyData = 65545 Then 'TAB
    '                ShiftTabPressed = True
    '                ShiftPressed = False
    '                handled = True
    '            Else
    '                Select Case keyData
    '                    Case Keys.Back

    '                    Case Keys.Left, Keys.Right
    '                        KeyPressed = True

    '                    Case Keys.Up, Keys.Down
    '                        KeyPressed = True

    '                    Case Keys.Enter
    '                        If myFocusedAdjustControl IsNot Nothing Then
    '                            If myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.EnterRequest()
    '                            End If
    '                            handled = True
    '                        End If
    '                    Case Keys.Tab
    '                        If myFocusedAdjustControl IsNot Nothing Then
    '                            If myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.EscapeRequest()

    '                            End If
    '                        End If
    '                        handled = True
    '                        KeyPressed = True
    '                End Select
    '            End If
    '            'KeyPressed = True
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ProcessCmdKey ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        Myclass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try

    '    Return handled

    'End Function


    ''SET KEYPREVIEW to TRUE
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by SG 03/01/11</remarks>
    'Private Sub MyBase_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
    '    Try

    '        If KeyPressed Or ShiftTabPressed Then



    '            If myFocusedAdjustControl IsNot Nothing And myScreenLayout.ButtonsPanel.SaveButton IsNot Nothing Then
    '                Select Case e.KeyCode

    '                    Case Keys.Escape
    '                        myFocusedAdjustControl.EscapeRequest()

    '                    Case Keys.Home
    '                        If Not myFocusedAdjustControl.EditionMode Then
    '                            myFocusedAdjustControl.HomeRequest()
    '                        End If

    '                    Case Keys.Right
    '                        Dim myControl As BSAdjustControl
    '                        If myFocusedAdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.UpDown Then
    '                            myControl = myFocusedAdjustControl
    '                            FocusNextAdjustControl()
    '                            If myScreenLayout.ButtonsPanel.SaveButton.Focused Then
    '                                FocusNextAdjustControl()
    '                            End If
    '                        End If
    '                        If myControl Is Nothing Or myFocusedAdjustControl IsNot myControl Then
    '                            If Not myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.IncreaseRequest()
    '                                KeyPressed = False
    '                            End If
    '                        End If

    '                    Case Keys.Left
    '                        Dim myControl As BSAdjustControl
    '                        If myFocusedAdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.UpDown Then
    '                            myControl = myFocusedAdjustControl
    '                            FocusNextAdjustControl()
    '                            If myScreenLayout.ButtonsPanel.SaveButton.Focused Then
    '                                FocusNextAdjustControl()
    '                            End If
    '                        End If
    '                        If myControl Is Nothing Or myFocusedAdjustControl IsNot myControl Then
    '                            If Not myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.DecreaseRequest()
    '                                KeyPressed = False
    '                            End If
    '                        End If

    '                    Case Keys.Up
    '                        Dim myControl As BSAdjustControl
    '                        If myFocusedAdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.LeftRight Then
    '                            myControl = myFocusedAdjustControl
    '                            FocusNextAdjustControl()
    '                            If myScreenLayout.ButtonsPanel.SaveButton.Focused Then
    '                                FocusNextAdjustControl()
    '                            End If
    '                        End If
    '                        If myControl Is Nothing Or myFocusedAdjustControl IsNot myControl Then
    '                            If Not myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.IncreaseRequest()
    '                                KeyPressed = False
    '                            End If
    '                        End If

    '                    Case Keys.Down
    '                        Dim myControl As BSAdjustControl
    '                        If myFocusedAdjustControl.AdjustButtonMode = BSAdjustControl.AdjustButtonModes.LeftRight Then
    '                            myControl = myFocusedAdjustControl
    '                            FocusNextAdjustControl()
    '                            If myScreenLayout.ButtonsPanel.SaveButton.Focused Then
    '                                FocusNextAdjustControl()
    '                            End If
    '                        End If
    '                        If myControl Is Nothing Or myFocusedAdjustControl IsNot myControl Then
    '                            If Not myFocusedAdjustControl.EditionMode Then
    '                                myFocusedAdjustControl.DecreaseRequest()
    '                                KeyPressed = False
    '                            End If
    '                        End If

    '                    Case Keys.Enter
    '                        If myFocusedAdjustControl.EditionMode Then
    '                            myFocusedAdjustControl.EditionMode = True
    '                        End If

    '                    Case Keys.Tab
    '                        If Not FwActionRequested Then
    '                            If Not ShiftTabPressed Then
    '                                e.Handled = FocusNextAdjustControl()
    '                            Else
    '                                e.Handled = FocusPreviousAdjustControl()
    '                            End If

    '                        End If

    '                End Select
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".MyBase_KeyUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        Myclass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)

    '    Finally
    '        e.Handled = True
    '        KeyPressed = False
    '        ShiftTabPressed = False
    '    End Try
    'End Sub

#End Region

#Region "Inheritable Constants"

    Protected Friend Const SensorsSampleRate As Integer = 100

#End Region

#Region "Inheritable attributes"
    Protected Friend FwActionRequested As Boolean = False      'an action has been requested
    Protected Friend KeyPressed As Boolean = False           'a key has been pressed
    Protected Friend ShiftPressed As Boolean = False         'the shift key has been pressed
    Protected Friend ShiftTabPressed As Boolean = False         'the shift key has been pressed


    Protected Friend myAdjustControls As List(Of BSAdjustControl)   'adjust controls
    Protected Friend myFocusedAdjustControl As BSAdjustControl     'currently focused adjust control


    Protected Friend myScreenLayout As ScreenLayout         'screen layout structure

#End Region



#Region "Public Properties"

    ''' <summary>
    ''' Flag for performing the closing of the Screen when requested from MDI
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 28/03/2012</remarks>
    Public Property CloseRequestedByMDI() As Boolean
        Get
            Return CloseRequestedByMDIAttr
        End Get
        Set(ByVal value As Boolean)
            If CloseRequestedByMDIAttr <> value Then
                CloseRequestedByMDIAttr = value
                If value Then
                    Me.AcceptButton.PerformClick()
                End If
            End If
        End Set
    End Property



    Public Property CloseWithShutDownRequestedByMDI() As Boolean
        Get
            Return CloseWithShutDownRequestedByMDIAttr
        End Get
        Set(ByVal value As Boolean)
            CloseWithShutDownRequestedByMDIAttr = value
        End Set
    End Property

    Public Property CloseWithoutShutDownRequestedByMDI() As Boolean
        Get
            Return CloseWithoutShutDownRequestedByMDIAttr
        End Get
        Set(ByVal value As Boolean)
            CloseWithoutShutDownRequestedByMDIAttr = value
        End Set
    End Property

#End Region

#Region "Inheritable Methods"

    Protected Friend Sub PrepareCommonAreas()
        Try
            Application.DoEvents()
            'Select Case MyClass.currentMode
            '    Case ADJUSTMENT_MODES.LOADING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = False
            '        MyClass.BsTestButton.Enabled = False
            '        MyClass.BsCloseButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.LOADED
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = True
            '        MyClass.BsTestButton.Enabled = True
            '        MyClass.BsCloseButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = True
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.ADJUSTING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = False
            '        MyClass.BsTestButton.Enabled = False
            '        MyClass.BsCloseButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.ADJUSTED
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(1)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsSaveButton.Enabled = True
            '        MyClass.BsExitAdjustButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = True
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.TESTING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = False
            '        MyClass.BsTestButton.Enabled = False
            '        MyClass.BsCloseButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.TESTED
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(2)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsBeginTestButton.Enabled = True
            '        MyClass.BsExitTestButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = True
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.CLOSING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = False
            '        MyClass.BsTestButton.Enabled = False
            '        MyClass.BsCloseButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.SAVING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(1)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsSaveButton.Enabled = False
            '        MyClass.BsExitAdjustButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.SAVED
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = True
            '        MyClass.BsTestButton.Enabled = True
            '        MyClass.BsCloseButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = True
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.ADJUST_EXITING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(1)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsSaveButton.Enabled = False
            '        MyClass.BsExitAdjustButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.TEST_BEGINING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(2)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsBeginTestButton.Enabled = False
            '        MyClass.BsExitTestButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.TEST_ENDED
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(2)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsBeginTestButton.Enabled = True
            '        MyClass.BsExitTestButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = True
            '        MyClass.BsSensorsGrpbox.Enabled = True
            '    Case ADJUSTMENT_MODES.TEST_EXITING
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(2)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsBeginTestButton.Enabled = False
            '        MyClass.BsExitTestButton.Enabled = False
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = False
            '    Case ADJUSTMENT_MODES.ERROR_MODE
            '        Dim tp As TabPage = MyClass.BsButtonsAreaTabs.TabPages(0)
            '        MyClass.BsButtonsAreaTabs.SelectedTab = tp
            '        MyClass.BsAdjustButton.Enabled = False
            '        MyClass.BsTestButton.Enabled = False
            '        MyClass.BsCloseButton.Enabled = True
            '        MyClass.BsAdjustmentsAreaGrpbox.Enabled = False
            '        MyClass.BsTestAreaGrpbox.Enabled = False
            '        MyClass.BsSensorsGrpbox.Enabled = True
            'End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareCommonAreas ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Protected Friend Sub Initialize()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.LOADING ' FORM_LOADING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Initialize", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Initialize ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Sub ReadAdjustments()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READING
            MyClass.PrepareCommonAreas()


            ''request adjustments
            'MyClass.myServiceMDI.ReadFwAdjustments()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Sub ReadAbsorbance()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_SCANNING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ReadAbsorbance", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadAbsorbance ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Sub PrepareAbsorbance()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_PREPARING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".PrepareAbsorbance", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAbsorbance ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Function PrepareAdjust() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".PrepareAdjust", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareAdjust ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function Adjust() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Adjust", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Adjust ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function Test() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.TESTING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Test", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Test ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function CloseForm() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.CLOSING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".CloseForm", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

            'SGM 05/11/2012
            MyClass.myServiceMDI.Text = My.Application.Info.ProductName

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".CloseForm ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function Save() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.SAVING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Save", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Save ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    ' XB 15/10/2014 - BA-2004
    Protected Friend Function FineOpticalCentering() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_PERFORMING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Save", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FineOpticalCentering ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function Park() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.PARKING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".Park", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".Park ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function ExitAdjust() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_EXITING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ExitAdjust", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitAdjust ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function BeginTest() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.TESTING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".BeginTest", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BeginTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function StartTest() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".StartTest", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StartTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function BeginTestExit() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.TEST_EXITING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".BeginTestExit", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BeginTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Function ExitTest() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.TEST_EXITING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ExitTest", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExitTest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Sub ReadLeds()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.LEDS_READING
            MyClass.PrepareCommonAreas()

            ''request adjustments
            'myGlobal = RequestAdjustmentsFromAnalyzer()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ReadLeds", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadLeds ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Sub ReadStressMode()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.STRESS_READING
            MyClass.PrepareCommonAreas()

            ''request adjustments
            'myGlobal = RequestAdjustmentsFromAnalyzer()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ReadStressMode", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadStressMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Function ReadFw() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.FW_READING
            MyClass.PrepareCommonAreas()


            ''request adjustments
            'myGlobal = RequestAdjustmentsFromAnalyzer()

            'If myGlobal.HasError Then
            '    ' PDT !!!
            '    Myclass.ShowMessage(Me.Name & ".ReadAdjustments", "SYSTEM_ERROR", "SYSTEM ERROR")
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadFw ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Sub InitAnalyzer()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.STANDBY_DOING
            MyClass.PrepareCommonAreas()


            ''request adjustments
            'myGlobal = RequestAdjustmentsFromAnalyzer()

            'If myGlobal.HasError Then
            '    ' PDT !!!
            '    Myclass.ShowMessage(Me.Name & ".ReadAdjustments", "SYSTEM_ERROR", "SYSTEM ERROR")
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".InitAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Function ReadAnalyzerInfo() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.ANALYZER_INFO_READING
            MyClass.PrepareCommonAreas()

            If myGlobal.HasError Then
                ' PDT !!!
                MyClass.ShowMessage(Me.Name & ".ExitTest", "SYSTEM_ERROR", "SYSTEM ERROR")
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadAnalyzerInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return myGlobal
    End Function

    Protected Friend Sub ReadCycles()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.CYCLES_READING
            MyClass.PrepareCommonAreas()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ReadCycles ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Friend Sub WriteCycles()
        Dim myGlobal As New GlobalDataTO
        Try
            Me.Cursor = Cursors.WaitCursor
            MyClass.CurrentMode = ADJUSTMENT_MODES.CYCLES_WRITTING
            MyClass.PrepareCommonAreas()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".WriteCycles ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub


    Protected Friend Sub ErrorMode()
        MyClass.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
        MyClass.PrepareCommonAreas()

        'SGM 22/10/2012
        MyClass.DisplayMessage("")

        ''SGM 01/10/2012
        'MyClass.DisplayMessage(Messages.SRV_ERR_COMM.ToString)
        ''MyClass.DisplayMessage(Messages.SRV_DATA_RECEIVED_ERROR.ToString)

    End Sub

    ''' <summary>
    ''' Collects all the BSAdjustControls in the form in order to manage them
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Protected Friend Function GetAdjustAreas(ByVal pPanel As Panel) As List(Of SpecificAdjustAreaLayout)
        Try
            Dim myList As New List(Of SpecificAdjustAreaLayout)

            LookForAdjustmentAreas(pPanel, myList)

            Return myList

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Determines the AdjustControls and the SaveButton included in the Adjustment Area
    ''' </summary>
    ''' <param name="pAdjustmentArea"></param>
    ''' <remarks>SG 04/01/11</remarks>
    Protected Friend Sub SetAdjustmentItems(ByVal pAdjustmentArea As Control)
        Try

            'collect the existing BSAdjustControls in the adjustment
            myAdjustControls = GetAdjustControls(pAdjustmentArea)


            'assign event handlers
            For Each AC As BSAdjustControl In myAdjustControls

                AC.SimulationMode = MyClass.SimulationMode

                AddHandler AC.AbsoluteSetPointReleased, AddressOf BsAdjustControl_AbsoluteSetPointReleased
                AddHandler AC.RelativeSetPointReleased, AddressOf BsAdjustControl_RelativeSetPointReleased
                AddHandler AC.SetPointOutOfRange, AddressOf BsAdjustControl_SetPointOutOfRange
                AddHandler AC.HomeRequestReleased, AddressOf BsAdjustControl_HomeRequestReleased
                AddHandler AC.ValidationError, AddressOf BsAdjustControl_ValidationError
                AddHandler AC.FocusReceived, AddressOf BsAdjustControl_FocusReceived
                AddHandler AC.FocusLost, AddressOf BsAdjustControl_FocusLost
                AddHandler AC.EditionModeChanged, AddressOf BsAdjustControl_EditionModeChanged
                AddHandler AC.TabRequest, AddressOf BsAdjustControl_TabRequest
                AddHandler AC.BackTabRequest, AddressOf BsAdjustControl_BackTabRequest
                ' XBC 10/10/2011
                AddHandler AC.GotoNextAdjustControl, AddressOf BsAdjustControl_GotoNextAdjustControl
            Next

            'If myScreenLayout.ButtonsPanel.SaveButton IsNot Nothing Then
            '    AddHandler myScreenLayout.ButtonsPanel.SaveButton.KeyUp, AddressOf MyBase_KeyUp
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " SetAdjustmentItems ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub



    ''' <summary>
    ''' Displays a mesage in the message area
    ''' </summary>
    ''' <param name="pMessageID"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Protected Friend Function DisplayMessage(ByVal pMessageID As String, Optional ByVal p2ndMessageID As String = "") As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try

            Dim Messages As New MessageDelegate
            Dim myMessagesDS As New MessagesDS
            Dim myGlobalDataTO As New GlobalDataTO

            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'Get type and multilanguage text for the informed Message
            Dim msgText As String = ""
            If pMessageID.Length > 0 Then
                myGlobalDataTO = Messages.GetMessageDescription(Nothing, pMessageID)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    Dim Exists As Boolean = False
                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        msgText = myMessagesDS.tfmwMessages(0).MessageText
                        myScreenLayout.MessagesPanel.Icon.BackgroundImage = Nothing
                        'Show message with the proper icon according the Message Type
                        If (myMessagesDS.tfmwMessages(0).MessageType = "Error") Then
                            'Error Message 
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                            auxIconName = GetIconName("CANCELF")
                            Exists = File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Information") Then
                            'Information Message 
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                            auxIconName = GetIconName("INFO")
                            Exists = File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Warning") Then
                            'Warning
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                            auxIconName = GetIconName("STUS_WITHERRS") 'WARNING")dl 23/03/2012
                            Exists = File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "OK") Then
                            'Warning
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                            auxIconName = GetIconName("ACCEPTF")
                            Exists = File.Exists(iconPath & auxIconName)

                        ElseIf (myMessagesDS.tfmwMessages(0).MessageType = "Working") Then
                            'Warning
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                            auxIconName = GetIconName("GEAR")
                            Exists = File.Exists(iconPath & auxIconName)
                        End If

                    End If

                    'second line
                    If p2ndMessageID.Length > 0 Then
                        myGlobalDataTO = Messages.GetMessageDescription(Nothing, p2ndMessageID)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                            msgText = msgText & " | " & myMessagesDS.tfmwMessages(0).MessageText
                            myScreenLayout.MessagesPanel.Label.Text = msgText
                        End If
                    End If

                    If Exists Then
                        If File.Exists(iconPath & auxIconName) Then
                            ''Dim myUtil As New Utilities.
                            Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                            myGlobalDataTO = Utilities.ResizeImage(myImage, New Size(20, 20))
                            If Not myGlobalDataTO.HasError And myGlobalDataTO.SetDatos IsNot Nothing Then
                                myScreenLayout.MessagesPanel.Icon.BackgroundImage = CType(myGlobalDataTO.SetDatos, Image) 'ImageUtilities.ImageFromFile(iconPath & auxIconName)
                            Else
                                myScreenLayout.MessagesPanel.Icon.BackgroundImage = myImage
                            End If

                            myScreenLayout.MessagesPanel.Icon.BackgroundImageLayout = ImageLayout.Center

                        End If
                    End If
                Else
                    MessageBox.Show(Me, myGlobalDataTO.ErrorCode, "BSAdjustmentBaseForm.mybase.displaymessage", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                'clear
                myScreenLayout.MessagesPanel.Label.Text = ""
                myScreenLayout.MessagesPanel.Icon.BackgroundImage = Nothing
            End If

            Application.DoEvents()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSAdjustmentBaseForm.displaymessage", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Displays a label in the message area
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 05/07/2012</remarks>
    Protected Friend Function DisplayLabel(ByVal pMessageType As String, ByVal pLabelID As String, Optional ByVal p2ndLabelID As String = "") As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            Dim dbConnection As SqlConnection = Nothing
            Dim MLRD As New MultilanguageResourcesDelegate
            Dim myGlobal As New GlobalDataTO

            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            Dim myFinalMessage As String

            'obtain the connection
            myGlobal = DAOBase.GetOpenDBConnection(Nothing)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)

                If dbConnection IsNot Nothing Then

                    'obtain the current language
                    'Dim myLocalBase As New GlobalBase
                    Dim LanguageID As String = GlobalBase.GetSessionInfo.ApplicationLanguage

                    'Get type and multilanguage text for the informed Message
                    Dim msgText As String = ""
                    If pLabelID.Length > 0 Then

                        myFinalMessage = MLRD.GetResourceText(dbConnection, pLabelID, LanguageID)
                        'second line
                        If p2ndLabelID.Length > 0 Then
                            myFinalMessage &= vbCrLf & MLRD.GetResourceText(dbConnection, p2ndLabelID, LanguageID)
                        End If

                        Dim Exists As Boolean

                        Select Case pMessageType.ToLower
                            Case "error"
                                myScreenLayout.MessagesPanel.Label.Text = msgText
                                auxIconName = GetIconName("CANCELF")
                                Exists = File.Exists(iconPath & auxIconName)

                            Case "information"
                                myScreenLayout.MessagesPanel.Label.Text = msgText
                                auxIconName = GetIconName("INFO")
                                Exists = File.Exists(iconPath & auxIconName)

                            Case "warning"
                                myScreenLayout.MessagesPanel.Label.Text = msgText
                                auxIconName = GetIconName("STUS_WITHERRS") 'WARNING")dl 23/03/2012
                                Exists = File.Exists(iconPath & auxIconName)

                            Case "ok"
                                myScreenLayout.MessagesPanel.Label.Text = msgText
                                auxIconName = GetIconName("ACCEPTF")
                                Exists = File.Exists(iconPath & auxIconName)

                            Case "working"
                                myScreenLayout.MessagesPanel.Label.Text = msgText
                                auxIconName = GetIconName("GEAR")
                                Exists = File.Exists(iconPath & auxIconName)

                        End Select

                        If Exists Then
                            If File.Exists(iconPath & auxIconName) Then
                                ''Dim myUtil As New Utilities.
                                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                                myGlobal = Utilities.ResizeImage(myImage, New Size(20, 20))
                                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                                    myScreenLayout.MessagesPanel.Icon.BackgroundImage = CType(myGlobal.SetDatos, Image) 'ImageUtilities.ImageFromFile(iconPath & auxIconName)
                                Else
                                    myScreenLayout.MessagesPanel.Icon.BackgroundImage = myImage
                                End If

                                myScreenLayout.MessagesPanel.Icon.BackgroundImageLayout = ImageLayout.Center

                            End If
                        End If
                        myScreenLayout.MessagesPanel.Label.Text = myFinalMessage
                    Else
                        'clear
                        myScreenLayout.MessagesPanel.Label.Text = ""
                        myScreenLayout.MessagesPanel.Icon.BackgroundImage = Nothing
                    End If

                Else
                    MessageBox.Show(Me, myGlobal.ErrorCode, "BSAdjustmentBaseForm.mybase.DisplayLabel", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else

            End If

            Application.DoEvents()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSAdjustmentBaseForm.DisplayLabel", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    'SGM 08/04/11
    Protected Friend Function GetMessage(ByVal pMessageID As String) As String

        Dim msgText As String = ""

        Try
            Dim Messages As New MessageDelegate
            Dim myMessagesDS As New MessagesDS
            Dim myGlobalDataTO As New GlobalDataTO

            myGlobalDataTO = Messages.GetMessageDescription(Nothing, pMessageID)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                Dim Exists As Boolean = False
                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                    msgText = myMessagesDS.tfmwMessages(0).MessageText
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, "BSAdjustmentBaseForm.GetMessage", EventLogEntryType.Error, False)
        End Try

        Return msgText

    End Function

    ''' <summary>
    ''' Loads and resizes the button image
    ''' </summary>
    ''' <param name="pButton"></param>
    ''' <param name="pImageName"></param>
    ''' <param name="pWidth"></param>
    ''' <param name="pHeight"></param>
    ''' <remarks>Created: SGM 26/07/2011</remarks>
    Protected Friend Sub SetButtonImage(ByVal pButton As Button, ByVal pImageName As String, _
                              Optional ByVal pWidth As Integer = 28, _
                              Optional ByVal pHeight As Integer = 28, _
                              Optional ByVal pAlignment As ContentAlignment = ContentAlignment.MiddleCenter)

        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Dim myGlobal As New GlobalDataTO
        ''Dim myUtil As New Utilities.

        Try

            Dim myButtonImage As Image

            auxIconName = GetIconName(pImageName)
            If File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image
                myImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                myGlobal = Utilities.ResizeImage(myImage, New Size(pWidth, pHeight))
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myButtonImage = CType(myGlobal.SetDatos, Bitmap)
                Else
                    myButtonImage = CType(myImage, Bitmap)
                End If

                pButton.Image = myButtonImage
                pButton.ImageAlign = pAlignment

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SetButtonImage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SetButtonImage", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Protected Friend Function GetHWElementsName(ByVal pItemID As String, ByVal pLanguageID As String) As String

        Dim res As String = ""
        Dim myMLRD As New MultilanguageResourcesDelegate

        Try
            'only for cycles values
            If pItemID.EndsWith("_CYC") Then
                pItemID = pItemID.Substring(0, pItemID.Length - 4)
            End If

            res = myMLRD.GetResourceText(Nothing, "LBL_SRV_" & pItemID, pLanguageID)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetHWElementsName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage(Me.Name & ".GetHWElementsName", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return res

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>SGM 27/07/2011</remarks>
    Protected Friend Sub GetUserNumericalLevel()

        Dim myGlobal As New GlobalDataTO
        'Dim myGlobalbase As New GlobalBase
        Dim res As Integer = -1

        Try
            'Get the current user level
            Dim CurrentUserLevel As String = ""
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            Dim myUsersLevel As New UsersLevelDelegate
            If CurrentUserLevel <> "" Then  'When user level exists then find his numerical level
                myGlobal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
                If Not myGlobal.HasError Then
                    MyClass.CurrentUserNumericalLevel = CType(myGlobal.SetDatos, Integer)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetHWElementsName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage(Me.Name & ".GetHWElementsName", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try


    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by SGM 27/09/2011
    ''' </remarks>
    Protected Friend Sub ActivateMDIMenusButtons(ByVal pEnable As Boolean, _
                                                 Optional ByVal pDisableForced As Boolean = False, _
                                                 Optional ByVal pFwUpdated As Boolean = False)
        Try

            If Not MyClass.myServiceMDI Is Nothing Then
                MyClass.myServiceMDI.ActivateActionButtonBar(pEnable, pDisableForced, pFwUpdated)
                MyClass.myServiceMDI.ActivateMenus(pEnable, pDisableForced, pFwUpdated)
                MyClass.myServiceMDI.Refresh()

                If Not pEnable Or pDisableForced Then
                    Me.Cursor = Cursors.WaitCursor
                Else
                    Me.Cursor = Cursors.Default
                End If


            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ActivateMDIMenusButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage(Me.Name & ".ActivateMDIMenusButtons", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



#Region "MustOverride Methods"

    ''' <summary>
    ''' It manages the GUI behavior according to the Alarm received. 
    ''' That way each screen must include it and it will perform specific 
    ''' treatment due to each screen has a particular behavior.
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overridable Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        ' XBC 06/11/2012 - Fix it because generates Form GUI errors on design time
        'Public MustOverride Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        ' if want to force an override here, so if this gets called - blam!
        Throw New NotImplementedException("You must implement this!")
        ' XBC 06/11/2012
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' That way each screen must include it and it will perform specific treatment due 
    ''' to each screen has a particular behavior.
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overridable Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        ' XBC 06/11/2012 - Fix it because generates Form GUI errors on design time
        'Public MustOverride Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        ' if want to force an override here, so if this gets called - blam!
        Throw New NotImplementedException("You must implement this!")
        ' XBC 06/11/2012
    End Sub

#End Region

#Region "Fw Adjustments"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Protected Friend Function RequestAdjustmentsMasterData() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            myGlobal = AnalyzerController.Instance.Analyzer.ReadFwAdjustmentsDS
            If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                Dim resultDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                'if the global dataset is empty the force to load again from the db
                If resultDS Is Nothing OrElse resultDS.srv_tfmwAdjustments.Count = 0 Then
                    myGlobal = AnalyzerController.Instance.Analyzer.LoadFwAdjustmentsMasterData(MyClass.SimulationMode)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        resultDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                        'update DB
                        Dim myAdjustmentsDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                        Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                        myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, myAdjustmentsDS)

                        'update text file
                        MyClass.myAdjustmentsDelegate = New FwAdjustmentsDelegate(MyClass.myAllAdjustmentsDS)
                        myGlobal = MyClass.myAdjustmentsDelegate.ExportDSToFile(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)

                    End If
                End If

                MyClass.myAllAdjustmentsDS = resultDS
                MyClass.myAdjustmentsDelegate = New FwAdjustmentsDelegate(MyClass.myAllAdjustmentsDS)
            End If


        Catch ex As Exception
            'PENDING what to do in case of error
            myAllAdjustmentsDS = Nothing
            myAdjustmentsDelegate = Nothing
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " RequestAdjustmentsMasterData ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    ' TO DELETE ???
    'Protected Friend Function RequestAdjustmentsFromAnalyzer() As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try

    '        'PENDING 
    '        'REQUEST ADJUSTMENTS TO ANALYZER BY CORRESPONDING SCRIPT
    '        'A - SIMULATE 
    '        Dim myData As String = ""
    '        'Dim myGlobalbase As New GlobalBase
    '        Dim objReader As System.IO.StreamReader
    '        Dim path As String = Application.StartupPath & GlobalBase.FwAdjustmentsFile
    '        objReader = New System.IO.StreamReader(path)
    '        myData = objReader.ReadToEnd()
    '        objReader.Close()

    '        'B - simulate adjustments data received
    '        myGlobal = MyClass.myAdjustmentsDelegate.ConvertReceivedDataToDS(myData)
    '        If myGlobal.SetDatos IsNot Nothing And Not myGlobal.HasError Then
    '            MyClass.AdjustmentsObtainedFromAnalyzer = True
    '            MyClass.AdjustmentsReaded = True
    '        End If

    '    Catch ex As Exception
    '        AdjustmentsObtainedFromAnalyzer = False
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & " RequestAdjustmentsFromAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        Myclass.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return myGlobal
    'End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pUpdateTextFile"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Protected Friend Function SimulateRequestAdjustmentsFromAnalyzer(Optional ByVal pUpdateTextFile As Boolean = False) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try

            myGlobal = AnalyzerController.Instance.Analyzer.LoadFwAdjustmentsMasterData(MyClass.SimulationMode)

            If myGlobal.SetDatos IsNot Nothing And Not myGlobal.HasError Then

                Dim myAdjustmentsDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                If myAdjustmentsDS IsNot Nothing Then
                    If pUpdateTextFile Then
                        'update text file
                        MyClass.myAdjustmentsDelegate = New FwAdjustmentsDelegate(MyClass.myAllAdjustmentsDS)
                        myGlobal = MyClass.myAdjustmentsDelegate.ExportDSToFile(AnalyzerController.Instance.Analyzer.ActiveAnalyzer)
                    End If

                    'update DB
                    Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                    myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, myAdjustmentsDS)

                    MyClass.AdjustmentsObtainedFromAnalyzer = True
                    MyClass.myServiceMDI.AdjustmentsReaded = True 'SGM 14/09/2011
                    'MyClass.AdjustmentsReaded = True

                    myGlobal.SetDatos = myAdjustmentsDS

                End If

            End If

        Catch ex As Exception
            AdjustmentsObtainedFromAnalyzer = False
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " SimulateRequestAdjustmentsFromAnalyzer ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
        Return myGlobal
    End Function

    Protected Friend Function UpdateAdjustments(ByVal pSelectedAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO

        Try
            If pSelectedAdjustmentsDS IsNot Nothing Then

                MyClass.ActivateMDIMenusButtons(False) 'SGM 28/09/2011

                'update the dataset
                myGlobal = MyClass.myAdjustmentsDelegate.UpdateAdjustments(pSelectedAdjustmentsDS, MyClass.myServiceMDI.ActiveAnalyzer)

                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                    'update DDBB
                    Dim myAdjustmentsDelegate As New DBAdjustmentsDelegate
                    myGlobal = myAdjustmentsDelegate.UpdateAdjustmentsDB(Nothing, pSelectedAdjustmentsDS)

                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " UpdateAdjustments ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        MyClass.ActivateMDIMenusButtons(True) 'SGM 28/09/2011

        Return myGlobal

    End Function


#End Region



#End Region

#Region "Overriden Methods"

    'This does the same in the base class, except that in the base class it does not has the risk of infinite recursion. commented out:
    'Public Overrides Function ShowMessage(ByVal pWindowTitle As String, _
    '                                      ByVal pMessageID As String, _
    '                                      Optional ByVal pSystemMessageText As String = "", _
    '                                      Optional ByVal pOwnerWindow As IWin32Window = Nothing, _
    '                                      Optional ByVal pTextParameters As List(Of String) = Nothing, _
    '                                      Optional ByVal pAditionalText As String = "", _
    '                                      Optional ByVal pMessageType As String = "") As DialogResult

    '    Try
    '        Return MyBase.ShowMessage(My.Application.Info.ProductName, pMessageID, pSystemMessageText, pOwnerWindow)
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & " ShowMessage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyClass.ShowMessage("ShowMessage", Messages.SYSTEM_ERROR.ToString, ex.Message) '<-- Infinite recursion chance!!! 
    '        Return Nothing
    '    End Try
    'End Function

#End Region

#Region "Private methods"

    ''' <summary>
    ''' Collects all the BSAdjustControls in the form in order to manage them
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Function GetAdjustControls(ByVal pAdjustmentArea As Control) As List(Of BSAdjustControl)
        Try
            Dim myList As New List(Of BSAdjustControl)
            Dim mySortedList As New List(Of BSAdjustControl)

            myAdjustControls = New List(Of BSAdjustControl)
            LookForAdjustControls(pAdjustmentArea, myList)

            mySortedList = (From a In myList _
                          Select a Order By a.TabIndex).ToList()

            Return mySortedList

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Iterative method for collecting  adjustment TabPages or Panels in the form
    ''' </summary>
    ''' <param name="pList"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub LookForAdjustmentAreas(ByVal pPanel As Panel, ByRef pList As List(Of SpecificAdjustAreaLayout))
        Try

            For Each C As Control In pPanel.Controls
                If TypeOf C Is TabControl Then
                    Dim myTabControl As TabControl = CType(C, TabControl)
                    For Each P As TabPage In myTabControl.TabPages
                        Dim myAdjustArea As New SpecificAdjustAreaLayout
                        myAdjustArea.Container = P
                        pList.Add(myAdjustArea)
                    Next
                    Exit For
                ElseIf TypeOf C Is Panel Then
                    Dim myPanel As Control = CType(C, Control)
                    Dim myAdjustArea As New SpecificAdjustAreaLayout
                    myAdjustArea.Container = myPanel
                    pList.Add(myAdjustArea)
                End If
            Next

            If pList.Count = 0 Then
                Dim myAdjustArea As New SpecificAdjustAreaLayout
                myAdjustArea.Container = pPanel
                pList.Add(myAdjustArea)
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " LookForAdjustmentAreas ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Iterative method for collecting BSAdjustControls from the form
    ''' </summary>
    ''' <param name="pContainer"></param>
    ''' <param name="pList"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub LookForAdjustControls(ByVal pContainer As Control, ByRef pList As List(Of BSAdjustControl))
        Try
            For Each C As Control In pContainer.Controls
                If TypeOf C Is BSAdjustControl Then
                    If C.Visible Then
                        Dim myControl As BSAdjustControl = CType(C, BSAdjustControl)
                        If myControl IsNot Nothing Then
                            If Not myAdjustControls.Contains(myControl) Then
                                pList.Add(myControl)
                            End If
                        End If
                    End If
                Else
                    LookForAdjustControls(C, pList)
                End If
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " LookForAdjustControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
            Throw ex
        End Try
    End Sub



    '''' <summary>
    '''' gets information video about a specific adjustment
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>Created by SG 03/01/11</remarks>
    'Private Function GetInformationVideo(ByVal pPage As APPLICATION_PAGES) As GlobalDataTO
    '    Dim myGlobal As GlobalDataTO
    '    Try
    '        myGlobal = New GlobalDataTO
    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        myGlobal.ErrorMessage = ex.Message
    '        GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetInformationVideo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        MyClass.ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message)
    '    End Try
    '    Return myGlobal
    'End Function

    ''' <summary>
    ''' Refresh common modes status from any kind of reception event received (low and high instructions)
    ''' </summary>
    ''' <param name="pResponse"></param>
    ''' <remarks>
    ''' Created by XBC 04/05/2011 - Separation between FwScripts Low level Instructions and High level Instructions
    ''' Modified by XB 15/10/2014 - Use NROTOR when wash station is down - BA-2004
    '''                           - Use FCK command after save Optical Centering adjustment - BA-2004
    ''' </remarks>
    Private Sub RefreshModes(ByVal pResponse As RESPONSE_TYPES)
        Try
            'adjustment modes
            Select Case MyClass.CurrentMode

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.HOME_PREPARING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.HOME_FINISHED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.LOADING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LOADED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.LOADING ' PDT !!! - pending to implement the error answers !!!
                    End If


                Case ADJUSTMENT_MODES.ADJUST_PREPARING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARING
                    End If


                Case ADJUSTMENT_MODES.ABSORBANCE_SCANNING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_SCANNED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARING
                    End If


                Case ADJUSTMENT_MODES.ABSORBANCE_PREPARING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ABSORBANCE_PREPARED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_PREPARING
                    End If


                Case ADJUSTMENT_MODES.ADJUSTING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUSTING
                    End If
                    FwActionRequested = False


                Case ADJUSTMENT_MODES.ADJUST_EXITING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LOADED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.ADJUST_EXITING
                    End If


                Case ADJUSTMENT_MODES.TEST_PREPARING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TEST_PREPARED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If


                Case ADJUSTMENT_MODES.TESTING
                    If pResponse = RESPONSE_TYPES.OK Or _
                       pResponse = RESPONSE_TYPES.START Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TESTED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.STIRRER_TEST
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.STIRRER_TESTING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.STIRRER_TESTED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_PERFORMING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.FINE_OPTICAL_CENTERING_DONE
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    '**************TANKS****************************************************************
                Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST_OK
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.TANKS_EMPTY_LC_REQUEST_OK
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_EMPTY_LC_ENDED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If


                Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST_OK
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.TANKS_FILL_DW_REQUEST_OK
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_FILL_DW_ENDED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST_OK
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_REQUEST_OK
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TANKS_TRANSFER_DW_LC_ENDED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If
                    '********************************************************************************

                    '***********MOTORS PUMPS VALVES *******************************************

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_WASHING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_WASHING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    ' XB 15/10/2014 - BA-2004
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_NROTOR
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_NROTOR_PERFORMED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    'SGM 21/11/2011
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_DOWN
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_DOWN
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If
                    'SGM 21/11/2011

                    'SGM 21/11/2011
                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_UP
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_UP
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If
                    'SGM 21/11/2011

                Case ADJUSTMENT_MODES.MBEV_ALL_ARMS_TO_PARKING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_ARMS_IN_PARKING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.MBEV_ARM_TO_WASHING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ARM_IN_WASHING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.MBEV_ALL_SWITCHING_OFF
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_ALL_SWITCHED_OFF
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    'SGM 10/10/2012
                Case ADJUSTMENT_MODES.COLLISION_TEST_ENABLING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.COLLISION_TEST_ENABLED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    'SGM 10/10/2012
                Case ADJUSTMENT_MODES.COLLISION_TEST_DISABLING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LOADED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    '*****************************************************************************

                Case ADJUSTMENT_MODES.TEST_EXITING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.TEST_EXITING
                    End If


                Case ADJUSTMENT_MODES.SAVING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.SAVED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        'MyClass.CurrentMode = ADJUSTMENT_MODES.SAVING
                    End If

                Case ADJUSTMENT_MODES.PARKING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.PARKED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.CLOSING
                    If pResponse = RESPONSE_TYPES.OK Then
                        Me.Close()
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                        MyClass.CurrentMode = ADJUSTMENT_MODES.CLOSING
                    End If

                Case ADJUSTMENT_MODES.LEDS_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LEDS_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.STRESS_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.STRESS_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.FW_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.FW_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.ANALYZER_INFO_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.ANALYZER_INFO_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                    '****REACTIONS ROTOR LEVEL DETECTION*********************************************
                Case ADJUSTMENT_MODES.LD_WASH_STATION_TO_UP
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LD_WASH_STATION_IS_UP
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If
                Case ADJUSTMENT_MODES.LD_WASH_STATION_TO_NROTOR
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LD_WASH_STATION_NROTOR_DONE
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If
                Case ADJUSTMENT_MODES.LD_WASH_STATION_TO_DOWN
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LD_WASH_STATION_IS_DOWN
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If
                Case ADJUSTMENT_MODES.LD_REQUESTING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LD_REQUESTED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If
                Case ADJUSTMENT_MODES.LD_WASH_STATION_TO_UP_TO_FINISH
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.LOADED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If
                    '****CYCLES COUNTING*********************************************
                Case ADJUSTMENT_MODES.CYCLES_READING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.CYCLES_READED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.CYCLES_WRITTING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.CYCLES_WRITTEN
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                    'SGM 01/12/2011

                Case ADJUSTMENT_MODES.THERMO_ARM_TO_PARKING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_IN_PARKING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.THERMO_ARM_TO_WASHING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.THERMO_ARM_IN_WASHING
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                    'end SGM 01/1/2011

                    'SGM 15/10/2012
                Case ADJUSTMENT_MODES.SN_SAVING
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.SN_SAVED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()
                    End If

                Case ADJUSTMENT_MODES.LOADED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.ADJUST_PREPARED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.ADJUSTED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.TESTED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.SAVED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.FW_READED
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.STANDBY_DOING
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.STANDBY_DONE
                    ' Nothing to do by now

                Case ADJUSTMENT_MODES.ADJUSTMENTS_READED
                    ' Nothing to do by now

                    'SGM 15/11/2012
                Case ADJUSTMENT_MODES.NEW_ROTOR_START
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.NEW_ROTOR_END
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                Case ADJUSTMENT_MODES.MBEV_WASHING_STATION_TO_NROTOR
                    If pResponse = RESPONSE_TYPES.OK Then
                        MyClass.CurrentMode = ADJUSTMENT_MODES.MBEV_WASHING_STATION_IS_NROTOR_PERFORMED
                    ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                        MyClass.ErrorMode()

                    End If

                    '***ERROR************************************************************
                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyClass.ErrorMode()

                Case Else
                    GlobalBase.CreateLogActivity("Mode : " & MyClass.CurrentMode.ToString, Me.Name & ".RefreshModes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                    'MyClass.ErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " RefreshModes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

    End Sub

#End Region

#Region "Screen Events"

    'force the info XPS margins to 0
    Private Sub BSAdjustmentBaseForm_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
        Try
            If MyClass.myScreenLayout.InfoPanel.InfoXPS IsNot Nothing Then
                MyClass.myScreenLayout.InfoPanel.InfoXPS.HorizontalPageMargin = 0
                MyClass.myScreenLayout.InfoPanel.InfoXPS.VerticalPageMargin = 0
            End If

            Cursor = Cursors.Default 'SGM 15/11/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " MyBase_Paint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try
    End Sub

#End Region

#Region "Adjust Control Events"

    ''' <summary>
    ''' The adjust control returns a setpoint out of limits
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_SetPointOutOfRange(ByVal sender As Object)
        Try
            Dim myAdjustControl As BSAdjustControl = CType(sender, BSAdjustControl)
            If Not myAdjustControl Is Nothing Then
                myAdjustControl.EscapeRequest()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_SetPointOutOfRange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control returns a new setpoint
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="Value">setpoint value</param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_AbsoluteSetPointReleased(ByVal sender As Object, ByVal Value As Double)
        Try

            'Me.Enabled = False

            FwActionRequested = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_SetPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control returns a new setpoint
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="Value">setpoint value</param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_RelativeSetPointReleased(ByVal sender As Object, ByVal Value As Double)
        Try

            FwActionRequested = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_SetPointReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control returns a new home request
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_HomeRequestReleased(ByVal sender As Object)
        Try
            'Me.Enabled = False
            FwActionRequested = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_HomeRequestReleased ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control returns a validation error message
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="message"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_ValidationError(ByVal sender As Object, ByVal message As String)
        Try

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_ValidationError ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The edition mode has changed. That way the form can address the handling of the key events. 
    ''' If edition mode is activated the key events are catched by the control. Otherwise the form handles them.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="pEditionMode"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_EditionModeChanged(ByVal sender As Object, ByVal pEditionMode As Boolean)
        Try
            Dim myControl As BSAdjustControl = CType(sender, BSAdjustControl)

            Me.KeyPreview = Not pEditionMode

            'If pEditionMode Then
            '    myControl.GetFocus()
            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_EditionModeChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control has just received the focus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_FocusReceived(ByVal sender As Object)
        Try
            myFocusedAdjustControl = CType(sender, BSAdjustControl)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_FocusReceived ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control has just received the focus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_FocusLost(ByVal sender As Object)
        Try
            'myFocusedAdjustControl = Nothing

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_FocusLost ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control has just received the focus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_TabRequest(ByVal sender As Object)
        Try
            If MyClass.myAdjustControls IsNot Nothing Then
                MyClass.AdjControlTabRequested = True

                ' XBC 11/10/2011
                Dim myControl As Control
                myControl = DirectCast(sender, Control)
                MyClass.myCurrentControl = myControl.Name
                ' XBC 11/10/2011

                MyClass.FocusNextAdjustControl()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_TabRequest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' The adjust control has just received the focus
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Sub BsAdjustControl_BackTabRequest(ByVal sender As Object)
        Try
            If MyClass.myAdjustControls IsNot Nothing Then
                MyClass.AdjControlBackTabRequested = True
                MyClass.FocusPreviousAdjustControl()
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_BackTabRequest ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Move the focus to next adjust control enabled
    ''' </summary>
    ''' <remarks>Created by XBC 10/10/2011</remarks>
    Private Sub BsAdjustControl_GotoNextAdjustControl(ByVal pControl As String)
        Try
            If MyClass.myAdjustControls IsNot Nothing Then
                MyClass.myCurrentControl = pControl

                MyClass.FocusNextAdjustControl()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsAdjustControl_GotoNextAdjustControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

#End Region

#Region "Focus Management"


    ''' <summary>
    ''' set the focus to the next adjustment controls
    ''' </summary>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Function FocusNextAdjustControl() As Boolean

        Dim handled As Boolean = False

        Try
            Dim y As Integer

            Dim myCurrentControl As BSAdjustControl = Nothing
            Dim myNextControl As Control = Nothing

            If myFocusedAdjustControl IsNot Nothing Then
                For x As Integer = 0 To myAdjustControls.Count - 1 Step 1
                    myCurrentControl = myAdjustControls(x)
                    If myCurrentControl Is myFocusedAdjustControl Then
                        y = x
                        Exit For
                    End If
                Next
            End If

            If myNextControl Is Nothing Then
                Dim k As Integer = 0
                Do
                    If y < myAdjustControls.Count - 1 Then
                        myNextControl = myAdjustControls(y + 1)
                        y = y + 1
                    Else
                        'myFocusedAdjustControl = Nothing

                        myNextControl = myAdjustControls(0)
                    End If

                    k = k + 1
                    If k >= myAdjustControls.Count Then
                        handled = True 'no items found
                        Exit Do
                    End If
                Loop Until myNextControl.Enabled
            End If

            MyClass.AdjControlTabRequested = False

            ' XBC 10/10/2011
            If MyClass.myCurrentControl = myCurrentControl.Name Then
                myNextControl.Focus()
            Else
                handled = True
            End If
            ' XBC 10/10/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FocusNextAdjustControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return handled

    End Function

    ''' <summary>
    ''' Set the focus to the previous adjustment control
    ''' </summary>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Private Function FocusPreviousAdjustControl() As Boolean

        Dim handled As Boolean = False

        Try
            Dim y As Integer

            Dim myCurrentControl As BSAdjustControl
            Dim myNextControl As Control = Nothing
            If myFocusedAdjustControl IsNot Nothing Then
                For x As Integer = 0 To myAdjustControls.Count - 1 Step 1
                    myCurrentControl = myAdjustControls(x)
                    If myCurrentControl Is myFocusedAdjustControl Then
                        y = x
                        Exit For
                    End If
                Next
            End If

            If myNextControl Is Nothing Then
                Dim k As Integer = 0
                Do
                    If y >= 1 Then
                        myNextControl = myAdjustControls(y - 1)
                        y = y - 1
                    Else
                        'myFocusedAdjustControl = Nothing

                        myNextControl = myAdjustControls(myAdjustControls.Count - 1)

                    End If

                    k = k + 1
                    If k >= myAdjustControls.Count Then
                        handled = True 'no items found
                        Exit Do
                    End If
                Loop Until myNextControl.Enabled
            End If

            MyClass.AdjControlBackTabRequested = False

            myNextControl.Focus()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".FocusPreviousAdjustControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

        Return handled
    End Function

#End Region

#Region "Information"

    ''' <summary>
    ''' gets information about a specific adjustment
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 03/01/11</remarks>
    Protected Friend Function DisplayInformation(ByVal pPage As APPLICATION_PAGES, ByVal pInfoTextControl As BsXPSViewer) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            'Find document path
            Dim myDocumentPath As String = ""
            Dim myVideoPath As String = ""
            myGlobal = MyClass.myBaseScreenDelegate.GetInformationDocument(Nothing, MyClass.AnalyzerModel, pPage.ToString)
            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                Dim myDocs As SRVInfoDocumentsDS = CType(myGlobal.SetDatos, SRVInfoDocumentsDS)
                If myDocs IsNot Nothing Then
                    Dim myDocRow As SRVInfoDocumentsDS.srv_tfmwInfoDocumentsRow = CType(myDocs.srv_tfmwInfoDocuments.Rows(0), SRVInfoDocumentsDS.srv_tfmwInfoDocumentsRow)

                    'Dim myGlobalbase As New GlobalBase

                    If myDocRow.DocumentPath.Length > 0 Then
                        myDocumentPath = Application.StartupPath & GlobalBase.ServiceInfoDocsPath & myDocRow.DocumentPath

                        Dim isScrollable As Boolean = myDocRow.Expandable

                        'Document
                        If myDocumentPath.Length > 0 Then
                            If myDocumentPath.Length > 0 Then
                                'show document
                                If File.Exists(myDocumentPath) Then
                                    If pInfoTextControl IsNot Nothing Then
                                        With pInfoTextControl

                                            .IsScrollable = isScrollable

                                            .PopupMenuEnabled = False

                                            .PrintButtonVisible = False
                                            .WholePageButtonVisible = False
                                            .FitToWidthButtonVisible = False
                                            .FitToHeightButtonVisible = False
                                            .DecreaseZoomButtonVisible = False
                                            .IncreaseZoomButtonVisible = False
                                            .MenuBarVisible = False
                                            .SearchBarVisible = False

                                            'open document
                                            .Open(myDocumentPath)

                                            Application.DoEvents()

                                            'without margis
                                            .HorizontalPageMargin = 0
                                            .VerticalPageMargin = 0

                                            'readjust control size depending if the information is long or not
                                            If Not isScrollable Then
                                                .Left = .Left - 2
                                                .Top = .Top - 2
                                                .Width = .Width + 4
                                                .Height = .Height + 4
                                            Else
                                                .Left = .Left - 2
                                                .Top = .Top - 2
                                                .Width = .Width + 4
                                                .Height = .Height + 2
                                            End If
                                            'If pInfoExpandButton IsNot Nothing Then
                                            '    pInfoExpandButton.Visible = isExpandable
                                            'End If

                                            .RefreshPage()

                                        End With

                                    End If

                                End If
                            Else
                                myGlobal.HasError = True
                                myGlobal.ErrorCode = Messages.MASTER_DATA_MISSING.ToString


                                GlobalBase.CreateLogActivity(Messages.MASTER_DATA_MISSING.ToString, Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                                'Myclass.ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                            End If

                        End If

                    End If

                    'video NOT V1
                    'If myDocRow.VideoPath.Length > 0 Then
                    '    myVideoPath = Application.StartupPath & GlobalBase.ServiceInfoDocsPath & myDocRow.VideoPath
                    '    If File.Exists(myVideoPath) Then

                    '    Else
                    '        myGlobal.HasError = True
                    '        myGlobal.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString

                    '        GlobalBase.CreateLogActivity(GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString, Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                    '        MyClass.ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                    '    End If
                    'End If
                End If
            End If


        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetInformationText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Return myGlobal

    End Function


    Protected Friend Function ExpandInformation(ByVal pExpand As Boolean, ByRef pInfoPanel As Panel, ByVal pAdjPanel As Panel, ByRef pExpandButton As Panel) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try

            If pInfoPanel IsNot Nothing And pAdjPanel IsNot Nothing And pExpandButton IsNot Nothing Then
                pInfoPanel.Visible = False
                If pExpand Then
                    pInfoPanel.Width = pInfoPanel.Width + pAdjPanel.Width + 1
                Else
                    pInfoPanel.Width = pInfoPanel.Width - pAdjPanel.Width - 1
                End If

                For Each C As Control In pInfoPanel.Controls
                    If TypeOf C Is BsXPSViewer Then
                        Dim myXPSViewer As BsXPSViewer = CType(C, BsXPSViewer)
                        If myXPSViewer IsNot Nothing Then
                            myXPSViewer.FitToPageHeight()
                        End If
                    End If
                Next

                Dim auxIconName As String = ""
                Dim iconPath As String = MyBase.IconsPath

                'button Icon
                If pExpand Then
                    auxIconName = GetIconName("LEFT")
                Else
                    auxIconName = GetIconName("RIGHT")
                End If

                If File.Exists(iconPath & auxIconName) Then
                    pExpandButton.BackgroundImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                    pExpandButton.BackgroundImageLayout = ImageLayout.Stretch
                End If


            End If


            myGlobal.SetDatos = pExpand

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " ExpandInformation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyClass.ShowMessage("Error", Messages.SYSTEM_ERROR.ToString, ex.Message)
        End Try

        Application.DoEvents()
        If pInfoPanel IsNot Nothing Then pInfoPanel.Visible = True

        Return myGlobal

    End Function




#End Region




#Region "ProgressBar"
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
#End Region

    <Obsolete("To access the tool tips control for this form, use ScreenTooltips property instead.")> _
    Friend Overridable Function bsScreenTooltips() As BSToolTip
        Return ScreenTooltips()
    End Function

    Friend Overridable Function ScreenTooltips() As BSToolTip
        Return bsScreenToolTipsControl
    End Function

    'Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip

End Class

