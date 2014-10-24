Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.App

Public Class IDemoMode
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As DemoModeDelegate

    ' Language
    Private currentLanguage As String

#End Region

#Region "Attributes"
    Private ActiveAnalyzerModelAttr As String
#End Region

#Region "Properties"
    Public Property ActiveAnalyzerModel() As String
        Get
            Return Me.ActiveAnalyzerModelAttr
        End Get
        Set(ByVal value As String)
            Me.ActiveAnalyzerModelAttr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout()
    End Sub
#End Region

#Region "Event Handlers"
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
    ''' <remarks>Created by XBC 12/04/11</remarks>
    Private Function ManageReceptionEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) As Boolean
        Dim myGlobal As New GlobalDataTO
        Try
            'manage special operations according to the screen characteristics

            ' XBC 05/05/2011 - timeout limit repetitions
            If pResponse = RESPONSE_TYPES.TIMEOUT Then
                Select Case pData.ToString
                    Case AnalyzerManagerSwActionList.TRYING_CONNECTION.ToString
                        MyBase.DisplayMessage(Messages.TRY_CONNECTION.ToString)

                    Case AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString
                        MyBase.CurrentMode = ADJUSTMENT_MODES.ERROR_MODE
                        MyBase.DisplayMessage(Messages.ERROR_COMM.ToString)
                        PrepareErrorMode()

                End Select
                Exit Function
            ElseIf pResponse = RESPONSE_TYPES.EXCEPTION Then
                PrepareErrorMode()
                Exit Function
            End If
            ' XBC 05/05/2011 - timeout limit repetitions

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(pResponse, pData)

            Select Case MyBase.CurrentMode
                Case ADJUSTMENT_MODES.TESTED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                        Me.RequestStressMode()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.STRESS_READED
                    If pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.TEST_EXITED
                    If pResponse = RESPONSE_TYPES.START Then
                        ' Nothing by now
                    ElseIf pResponse = RESPONSE_TYPES.OK Then
                        PrepareArea()
                    Else
                        PrepareErrorMode()
                        Exit Function
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try
        Return True
    End Function



#End Region

#Region "Private Methods"

    ''' <summary>
    ''' reset all homes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>SGM 10/03/11</remarks>
    Private Function InitializeHomes() As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = myScreenDelegate.ResetAllPreliminaryHomes(MyBase.myServiceMDI.ActiveAnalyzer)

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".InitializeHomes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".InitializeHomes", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobal
    End Function

    '''' <summary>
    '''' Generic function to send FwScripts to the Instrument through own delegates
    '''' </summary>
    '''' <param name="pMode"></param>
    '''' <remarks>Created by: XBC 12/04/2011</remarks>
    'Private Sub SendFwScript(ByVal pMode As ADJUSTMENT_MODES)
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = myScreenDelegate.SendFwScriptsQueueList(pMode)
    '        If Not myGlobal.HasError Then
    '            ' Send FwScripts
    '            myGlobal = myFwScriptDelegate.StartFwScriptQueue
    '        End If

    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '            If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
    '                myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
    '            End If
    '            CreateLogActivity(myGlobal.ErrorCode, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '            ShowMessage(Me.Name & ".SendFwScript ", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".SendFwScript ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".SendFwScript ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 12/04/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_DEMO", currentLanguage)
            Me.BsSubtitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SUBTITLE_DEMO", currentLanguage)

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 26/07/11
    ''' Modified by XBC 24/11/2011 - Unify buttons Start and Stop Demo
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...

            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then
                MyBase.bsScreenToolTips.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_TestStop", currentLanguage))
            Else
                MyBase.bsScreenToolTips.SetToolTip(BsTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "SRV_BTN_Test", currentLanguage))
            End If

            MyBase.bsScreenToolTips.SetToolTip(BsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 12/04/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        Try
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                BsTestButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'MyBase.SetButtonImage(BsExitButton, "CANCEL")
            'MyBase.SetButtonImage(BsTestButton, "ADJUSTMENT")


            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'BsExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

            '' XBC 24/11/2011 - Unify buttons Start and Stop Demo
            ' ''TEST Button
            ''auxIconName = GetIconName("ADJUSTMENT")
            ''If System.IO.File.Exists(iconPath & auxIconName) Then
            ''    BsTestButton_TODELETE.BackgroundImage = Image.FromFile(iconPath & auxIconName)
            ''    BsTestButton_TODELETE.BackgroundImageLayout = ImageLayout.Center
            ''End If

            ''ABORT TEST Button
            'auxIconName = GetIconName("ADJUSTMENT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    BsTestButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'BsTestButton.BackgroundImageLayout = ImageLayout.Center
            'End If
            '' XBC 24/11/2011 - Unify buttons Start and Stop Demo

            ''Info Button
            'auxIconName = GetIconName("RIGHT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    Me.BsInfoExpandButton.BackgroundImage = Image.FromFile(iconPath & auxIconName)
            '    Me.BsInfoExpandButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Request current Stress Mode
    ''' </summary>
    ''' <remarks>Created by XBC 14/05/2012</remarks>
    Private Sub RequestStressMode()
        Dim myGlobal As New GlobalDataTO
        Try
            MyBase.ReadStressMode()
            PrepareArea()

            ' Manage FwScripts must to be sent at load screen
            If MyBase.SimulationMode Then
                ' simulating...
                'MyBase.DisplaySimulationMessage("Request Status Stress Mode from Instrument...")
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                MyClass.CurrentMode = ADJUSTMENT_MODES.STRESS_READED
                PrepareArea()
            Else
                'SendFwScript(Me.CurrentMode)
                If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                    myGlobal = myScreenDelegate.SendSDPOLL()
                End If
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".RequestStressMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RequestStressMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ReadStressStatus()
        Try
            Select Case myScreenDelegate.StatusStressMode
                Case STRESS_STATUS.NOT_STARTED
                    ' Nothing by now
                Case STRESS_STATUS.UNFINISHED
                    ' Activate timer
                    Me.RequestStatusStressTimer.Interval = CInt(myScreenDelegate.TimerStatusModeStress) * 1000
                    Me.RequestStatusStressTimer.Enabled = True
                    MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)
                Case STRESS_STATUS.FINISHED_OK  ' , STRESS_STATUS.FINISHED_ERR ' XBC 25/10/2012
                    AnalyzerController.Instance.Analyzer.IsStressing = False '#REFACTORING
                    Me.RequestStatusStressTimer.Enabled = False
                    MyBase.myServiceMDI.SEND_INFO_START()
                    System.Threading.Thread.Sleep(500)
                    MyBase.DisplayMessage(Messages.SRV_COMPLETED.ToString)
                    Me.PrepareLoadedMode()

                    ' XBC 25/10/2012 - Difference Error/Alarms end
                Case STRESS_STATUS.FINISHED_ERR
                    AnalyzerController.Instance.Analyzer.IsStressing = False '#REFACTORING
                    Me.RequestStatusStressTimer.Enabled = False
                    MyBase.myServiceMDI.SEND_INFO_START()
                    System.Threading.Thread.Sleep(500)
                    MyBase.DisplayMessage(Messages.SRV_TESTEND_ERROR.ToString)
                    Me.PrepareLoadedMode()

            End Select

            Me.PrepareTestButton()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ReadStressStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadStressStatus ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: XBC 12/04/2011</remarks>
    Private Sub PrepareArea()
        Try
            Application.DoEvents()

            'Me.BsAdjustButton.Visible = False
            'Me.BsTestButton.Visible = False
            'Me.BsCancelButton.Visible = False

            ' Enabling/desabling form components to this child screen
            Select Case MyBase.CurrentMode

                Case ADJUSTMENT_MODES.LOADED
                    PrepareLoadedMode()
                    MyBase.DisplayMessage("")

                Case ADJUSTMENT_MODES.TESTING, _
                     ADJUSTMENT_MODES.TESTED
                    PrepareTestingMode()

                Case ADJUSTMENT_MODES.STRESS_READED
                    PrepareStressReadedMode()

                Case ADJUSTMENT_MODES.TEST_EXITING
                    PrepareTestExitingMode()

                Case ADJUSTMENT_MODES.TEST_EXITED
                    PrepareLoadedMode()
                    MyBase.DisplayMessage("")

                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Loadede Mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2011</remarks>
    Private Sub PrepareLoadedMode()
        Try
            ' Configurate GUI Controls
            Me.picAjax.Visible = False
            ' Buttons Area
            Me.BsTestButton.Enabled = True
            Me.BsExitButton.Enabled = True

            MyBase.ActivateMDIMenusButtons(True)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub PrepareStressReadedMode()
        Try
            Me.ReadStressStatus()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareStressReadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareStressReadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Testing Mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2011</remarks>
    Private Sub PrepareTestingMode()
        Try
            ' Configurate GUI Controls
            Me.picAjax.Visible = True
            ' Buttons Area
            Me.BsTestButton.Enabled = True
            Me.BsExitButton.Enabled = False

            'update MDI buttons and menus
            MyBase.ActivateMDIMenusButtons(False) 'SGM 27/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Test Exiting Mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2011</remarks>
    Private Sub PrepareTestExitingMode()
        Try
            ' Buttons Area
            Me.BsTestButton.Enabled = False
            Me.BsExitButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareTestExitingMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestExitingMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            ' Configurate GUI Controls
            Me.picAjax.Visible = False
            ' Buttons Area
            Me.BsTestButton.Enabled = False
            Me.BsExitButton.Enabled = True ' Just Exit button is enabled in error case
            '#REFACTORING
            If AnalyzerController.Instance.Analyzer.IsStressing Then
                AnalyzerController.Instance.Analyzer.IsStressing = False
                myScreenDelegate.InsertReport("UTIL", "DEMO")
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get Limits values from BD for Stress Tests
    ''' </summary>
    ''' <remarks>Created by XBC 22/03/2011</remarks>
    Private Function GetParameters() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            myGlobalDataTO = myScreenDelegate.GetParameters(Me.ActiveAnalyzerModel)
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GetParameters ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetParameters ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 12/04/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel

                .AdjustmentPanel.Container = Me.BsDemoAdjustPanel
                .InfoPanel.Container = Me.BsDemoModeInfoPanel
                .InfoPanel.InfoXPS = BsInfoXPSViewer

            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Must Inherited"
    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            PrepareErrorMode()

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' IDisposable functionality to force the releasing of the objects which wasn't totally closed by default
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by XBC 12/09/2011</remarks>
    Private Sub IDemoMode_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try

            If e.CloseReason = CloseReason.MdiFormClosing Then
                e.Cancel = True
            Else
                MyClass.myScreenDelegate.Dispose()
                MyClass.myScreenDelegate = Nothing

                'SGM 28/02/2012
                MyBase.ActivateMDIMenusButtons(Not MyBase.CloseRequestedByMDI)
                MyBase.myServiceMDI.IsFinalClosing = MyBase.CloseRequestedByMDI

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FormClosing ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DemoMode_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Try
            'Screen delegate SGM 20/01/2012
            MyClass.myScreenDelegate = New DemoModeDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels and get Icons for graphical Buttons
            GetScreenLabels()

            PrepareButtons()

            'SGM 12/11/2012 - Information
            Me.BsInfoXPSViewer.FitToPageHeight()
            MyBase.DisplayInformation(APPLICATION_PAGES.DEMO, Me.BsInfoXPSViewer)

            'Initialize homes SGM 20/09/2011
            MyClass.InitializeHomes()

            Me.RequestStatusStressTimer.Enabled = False

            Me.ActiveAnalyzerModel = Ax00ServiceMainMDI.ActiveAnalyzerModel
            ' Get common Parameters
            myGlobal = GetParameters()

            ' Check communications with Instrument
            If Not AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                PrepareErrorMode()
                MyBase.ActivateMDIMenusButtons(True)
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
                PrepareArea()
            End If

            If myGlobal.HasError Then
                PrepareErrorMode()
                ShowMessage(Me.Name & ".Load", myGlobal.ErrorCode, myGlobal.ErrorMessage, Me)
            End If

            ResetBorderSRV()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub IDemoMode_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            Me.BsInfoXPSViewer.Visible = True
            Me.BsInfoXPSViewer.RefreshPage()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Shown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ' XBC 24/11/2011 - Unify buttons Start and Stop Demo
    'Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton_TODELETE.Click
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        myGlobal = MyBase.Test
    '        If myGlobal.HasError Then
    '            PrepareErrorMode()
    '        Else
    '            PrepareArea()

    '            MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

    '            If Not myGlobal.HasError Then
    '                If MyBase.SimulationMode Then
    '                    ' simulating
    '                    Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Demo Mode Testing ..."
    '                    MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
    '                    PrepareTestingMode()
    '                Else
    '                    If Not myGlobal.HasError AndAlso Ax00ServiceMainMDI.MDIAnalyzerManager.Connected Then
    '                        myGlobal = myScreenDelegate.SendDEMO_TEST()
    '                    End If
    '                End If
    '            End If

    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".BsTestButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".BsTestButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Functionality to START/STOP Demo Mode
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Modified by XBC 24/11/2011 - Unify buttons Start and Stop Demo</remarks>
    Private Sub BsTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsTestButton.Click
        Dim myGlobal As New GlobalDataTO
        Try
            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then

                ' STOP DEMO 

                myGlobal = MyBase.ExitTest
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()

                    MyBase.DisplayMessage(Messages.SRV_TEST_ABORTING.ToString)

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Demo Mode Exiting ..."
                            myGlobal = myScreenDelegate.InsertReport("UTIL", "DEMO")
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TEST_EXITED
                            PrepareArea()
                        Else
                            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                myGlobal = myScreenDelegate.SendDEMO_STOP()
                            End If
                        End If
                    End If

                End If

            Else

                ' START DEMO 

                AnalyzerController.Instance.Analyzer.IsStressing = True '#REFACTORING

                myGlobal = MyBase.Test
                If myGlobal.HasError Then
                    PrepareErrorMode()
                Else
                    PrepareArea()

                    MyBase.DisplayMessage(Messages.SRV_TEST_IN_PROCESS.ToString)

                    MyBase.myServiceMDI.SEND_INFO_STOP()
                    System.Threading.Thread.Sleep(500)

                    If Not myGlobal.HasError Then
                        If MyBase.SimulationMode Then
                            ' simulating
                            Ax00ServiceMainMDI.ErrorStatusLabel.Text = "Demo Mode Testing ..."
                            MyBase.CurrentMode = ADJUSTMENT_MODES.TESTING
                            PrepareTestingMode()
                        Else
                            If Not myGlobal.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then '#REFACTORING
                                myGlobal = myScreenDelegate.SendDEMO_TEST()
                            End If
                        End If
                    End If

                End If
            End If

            PrepareTestButton()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsTestButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsTestButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change Icon button depending on screen status
    ''' </summary>
    ''' <remarks>Created by XBC 24/11/2011 - Unify buttons Start and Stop Test Stress</remarks>
    Private Sub PrepareTestButton()

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath

        Try

            If myScreenDelegate.StatusStressMode = STRESS_STATUS.UNFINISHED Then
                auxIconName = GetIconName("STOP")
            Else
                auxIconName = GetIconName("ADJUSTMENT")
            End If
            If (auxIconName <> "") Then
                BsTestButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Dim myNewImage As Image
            'If System.IO.File.Exists(iconPath & auxIconName) Then

            '    Dim myImage As Image
            '    myImage = Image.FromFile(iconPath & auxIconName)

            '    myGlobal = myUtil.ResizeImage(myImage, New Size(24, 24))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myNewImage = CType(myGlobal.SetDatos, Bitmap)
            '    Else
            '        myNewImage = CType(myImage, Bitmap)
            '    End If

            '    BsTestButton.Image = myNewImage
            '    'BsTestButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            GetScreenTooltip()

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareTestButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareTestButton ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

    Private Sub BsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsExitButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".BsExitButton.Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BsExitButton.Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub RequestStatusStressTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RequestStatusStressTimer.Tick
        Try
            Me.RequestStatusStressTimer.Enabled = False

            If MyBase.SimulationMode Then
                'MyBase.CurrentMode = ADJUSTMENT_MODES.STRESS_READED
                'myScreenDelegate.SimulateTestComplete()
                'PrepareArea()
            Else
                Me.RequestStressMode()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".RequestStatusStressTimer.Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RequestStatusStressTimer.Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/06/2011
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Me.BsExitButton.Enabled) Then
                '    Me.Close()
                'End If

                'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                BsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub BsButtons_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsTestButton.MouseHover
        Me.Cursor = Cursors.Hand
    End Sub

    Private Sub Buttons_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsTestButton.MouseLeave, BsExitButton.MouseLeave
        If AnalyzerController.Instance.Analyzer.IsStressing Then '#REFACTORING
            Me.Cursor = Cursors.WaitCursor
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub Buttons_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BsTestButton.MouseMove
        Me.Cursor = Cursors.Hand
    End Sub
#End Region

#Region "XPS Info Events"

    Private Sub BsXPSViewer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsInfoXPSViewer.Load
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
            MyBase.CreateLogActivity(ex.Message, Me.Name & ".BsXPSViewer_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsXPSViewer_Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

End Class