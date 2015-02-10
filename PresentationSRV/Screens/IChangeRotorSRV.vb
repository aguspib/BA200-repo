Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.App



Public Class UiChangeRotorSRV
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"

    'Private mdiAnalyzerCopy As AnalyzerManager
    'Private instructionALIGHTInProcess As Boolean = False
    Private MSG_StartInstrument As String 'RH 14/02/2012

    Private OKIconName As String
    Private WRONGIconName As String


    Private statusMDIChangedFlag As Boolean = False

    Private WithEvents myScreenDelegate As ChangeRotorDelegate

#End Region

#Region "Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/11/2012</remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'New rotor button
            auxIconName = GetIconName("ADJUSTMENT") 'CONTINUESEB")
            If (auxIconName <> "") Then
                bsNewRotorButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CHANGE ROTOR Button
            auxIconName = GetIconName("CHANGEROTORB")
            If (auxIconName <> "") Then
                bsWSUpDownButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'icons for result
            auxIconName = GetIconName("ACCEPTF")
            If (auxIconName <> "") Then
                OKIconName = iconPath & auxIconName
            End If

            auxIconName = GetIconName("CANCELF")
            If (auxIconName <> "") Then
                WRONGIconName = iconPath & auxIconName
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/11/2012</remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ChangeRotor", currentLanguage)
            bsChangeRotorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RISINGUP", currentLanguage)
            bsNewAdjLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NEW_ROTOR", currentLanguage) 'LBL_SRV_NEW_ROTOR

            'For Tooltips...
            ScreenTooltips.SetToolTip(bsWSUpDownButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RISINGUP", currentLanguage))
            ScreenTooltips.SetToolTip(bsNewRotorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_NEWADJUSTROTOR", currentLanguage))
            ScreenTooltips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            'RH 14/02/2012
            MSG_StartInstrument = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_StartInstrument", currentLanguage)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/11/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub InitializeScreen()
        Try

            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), _
                                    myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)
            'END DL 28/07/2011

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()
            Me.Opacity = 100

            'Screen delegate SGM 15/11/2012
            MyClass.myScreenDelegate = New ChangeRotorDelegate(MyBase.myServiceMDI.ActiveAnalyzer, myFwScriptDelegate)

            ' XBC 30/11/2011
            MyClass.myScreenDelegate.IsWashingStationUp = False

            MyBase.CurrentMode = ADJUSTMENT_MODES.LOADED
            MyClass.PrepareArea()


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, ".InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/11/2012</remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 13/11/2012</remarks>
    Private Sub ExitScreen()
        Try
            If bsExitButton.Enabled Then

                Me.Close()

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, ".ExitScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try

    End Sub

#End Region

#Region "Must Inherited"
    ''' <summary>
    ''' Prepare GUI for Error Mode
    ''' </summary>
    ''' <remarks>Created by XBC 12/04/2011</remarks>
    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            MyBase.ErrorMode()
            ' Configurate GUI Controls

            Me.bsWSUpDownButton.Enabled = False
            Me.bsNewRotorButton.Enabled = False
            Me.bsExitButton.Enabled = True

            ScreenWorkingProcess = False
            MyBase.ActivateMDIMenusButtons(True)

            Me.BsProgressBar.Value = Me.BsProgressBar.Maximum
            Me.BsNewRotorProcessTimer.Stop()
            Me.BsNewRotorProcessTimer.Enabled = False
            Me.bsStatusImage.Visible = True

            Cursor = Cursors.Default

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            PrepareErrorMode()

            If Me.BsProgressBar.Visible Then
                If AnalyzerController.Instance.Analyzer.Connected And pAlarmType <> ManagementAlarmTypes.UPDATE_FW And pAlarmType <> ManagementAlarmTypes.FATAL_ERROR Then '#REFACTORING
                    MyClass.PrepareLoadedMode()
                End If
                ScreenWorkingProcess = False
                MyBase.ActivateMDIMenusButtons(True)
                Me.bsStatusImage.Image = ImageUtilities.ImageFromFile(WRONGIconName)
                Me.bsStatusImage.Visible = True

                Me.BsNewRotorProcessTimer.Stop()
                Me.BsNewRotorProcessTimer.Enabled = False

                Me.BsProgressBar.Value = Me.BsProgressBar.Maximum

                Cursor = Cursors.Default
            End If

            'when stop action is finished, perform final operations after alarm received
            MyBase.myServiceMDI.ManageAlarmStep2(pAlarmType)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Refresh this specified screen with the information received from the Instrument
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>Created by XBC 05/12/2011</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        'Dim myGlobal As New GlobalDataTO
        Try
            myScreenDelegate.RefreshDelegate(pRefreshEventType, pRefreshDS)

            'if needed manage the event in the Base Form
            MyBase.OnReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            PrepareArea()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by SGM 15/11/2012</remarks>
    Private Sub SendWASH_STATION_CTRL()
        Dim myGlobal As New GlobalDataTO
        Try
            myGlobal = MyBase.Park
            If myGlobal.HasError Then
                PrepareErrorMode()
            Else
                PrepareArea()

                If Not myGlobal.HasError Then
                    If MyBase.SimulationMode Then
                        ' simulating
                        'MyBase.DisplaySimulationMessage("Doing Specified Test...")
                        Me.Cursor = Cursors.WaitCursor
                        System.Threading.Thread.Sleep(SimulationProcessTime)
                        MyBase.myServiceMDI.Focus()
                        Me.Cursor = Cursors.Default
                        MyBase.CurrentMode = ADJUSTMENT_MODES.PARKED
                        myScreenDelegate.IsWashingStationUp = Not myScreenDelegate.IsWashingStationUp
                        PrepareArea()
                    Else
                        ' Manage instruction for Washing Station UP/DOWN
                        If myScreenDelegate.IsWashingStationUp Then
                            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.DOWN)
                        Else
                            myScreenDelegate.SendWASH_STATION_CTRL(Ax00WashStationControlModes.UP)
                        End If
                    End If
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".SendWASH_STATION_CTRL", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".SendWASH_STATION_CTRL", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare Tab Area according with current operations
    ''' </summary>
    ''' <remarks>Created by: SGM 13/11/2012</remarks>
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


                Case ADJUSTMENT_MODES.PARKING
                    If Not myScreenDelegate.IsWashingStationUp Then
                        Me.PrepareWSMovingUpMode()
                    Else
                        Me.PrepareWSMovingDownMode()
                    End If

                Case ADJUSTMENT_MODES.PARKED
                    If myScreenDelegate.IsWashingStationUp Then
                        Me.PrepareWSUpMode()
                    Else
                        Me.PrepareWSDownMode()
                    End If

                Case ADJUSTMENT_MODES.NEW_ROTOR_START
                    Me.PrepareNewRotorProcessMode()

                Case ADJUSTMENT_MODES.NEW_ROTOR_END
                    Me.PrepareNewRotorFinishedMode()


                Case ADJUSTMENT_MODES.ERROR_MODE
                    PrepareErrorMode()
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareArea ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareLoadedMode()
        Try

            Me.bsWSUpDownButton.Enabled = True
            Me.bsNewRotorButton.Enabled = False
            Me.bsExitButton.Enabled = True


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareLoadedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareLoadedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareWSMovingUpMode()
        Try
            Me.bsStatusImage.Visible = False

            Me.Cursor = Cursors.WaitCursor
            MyClass.DisableAll()
            MyBase.ActivateMDIMenusButtons(False)

            Me.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWSMovingUpMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWSMovingUpMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareWSUpMode()
        Try

            Me.bsWSUpDownButton.Enabled = False
            Me.bsNewRotorButton.Enabled = True
            Me.bsExitButton.Enabled = True

            Cursor = Cursors.Default

            ScreenWorkingProcess = False
            MyBase.ActivateMDIMenusButtons(True)

            Me.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWSUpMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWSUpMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareWSMovingDownMode()
        Try

            Me.Cursor = Cursors.WaitCursor
            MyClass.DisableAll()
            MyBase.ActivateMDIMenusButtons(False)

            Me.Refresh()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWSMovingDownMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWSMovingDownMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareWSDownMode()
        Try

            ScreenWorkingProcess = True
            MyBase.ActivateMDIMenusButtons(False)
            MyClass.DisableAll()

            If MyBase.SimulationMode Then
                ' simulating
                MyBase.CurrentMode = ADJUSTMENT_MODES.NEW_ROTOR_START
                Me.Cursor = Cursors.WaitCursor
                System.Threading.Thread.Sleep(SimulationProcessTime)
                MyBase.myServiceMDI.Focus()
                Me.Cursor = Cursors.Default
                MyBase.CurrentMode = ADJUSTMENT_MODES.NEW_ROTOR_END
                PrepareArea()
            Else
                MyBase.CurrentMode = ADJUSTMENT_MODES.NEW_ROTOR_START

                MyClass.myScreenDelegate.SendNEW_ROTOR()

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareWSDownMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareWSDownMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareNewRotorProcessMode()
        Try

            ScreenWorkingProcess = True
            MyBase.ActivateMDIMenusButtons(False)
            MyClass.DisableAll()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareNewRotorProcessMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareNewRotorProcessMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrepareNewRotorFinishedMode()
        Try

            MyClass.PrepareLoadedMode()

            ScreenWorkingProcess = False
            MyBase.ActivateMDIMenusButtons(True)

            If AnalyzerController.Instance.Analyzer.ErrorCodes.Contains("550") Then 'Reactions rotor missing '#REFACTORING
                Me.bsStatusImage.Image = ImageUtilities.ImageFromFile(WRONGIconName)
            Else
                Me.bsStatusImage.Image = ImageUtilities.ImageFromFile(OKIconName)
            End If
            Me.BsProgressBar.Value = Me.BsProgressBar.Maximum
            Me.BsNewRotorProcessTimer.Stop()
            Me.BsNewRotorProcessTimer.Enabled = False
            Me.bsStatusImage.Visible = True

            Cursor = Cursors.Default


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".PrepareNewRotorFinishedMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareNewRotorFinishedMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DisableAll()
        Try
            Cursor = Cursors.WaitCursor
            Me.bsWSUpDownButton.Enabled = False
            Me.bsNewRotorButton.Enabled = False
            Me.bsExitButton.Enabled = False


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".DisableAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".DisableAll ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


#End Region

#Region "Screen Events"

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        ExitScreen()
    End Sub

    Private Sub IChangeRotorSRV_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If myScreenDelegate IsNot Nothing Then
            myScreenDelegate.Dispose()
            myScreenDelegate = Nothing
        End If
    End Sub

    Private Sub IChangeRotor_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            ExitScreen()
        End If
    End Sub

    Private Sub IChangeRotor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            InitializeScreen()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".IChangeRotor_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IChangeRotor_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub


    Private Sub bsWSUpDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsWSUpDownButton.Click
        Dim resultData As New GlobalDataTO
        Try
            If Not MyClass.myScreenDelegate.IsWashingStationUp Then

                Me.bsStatusImage.Visible = False
                Me.BsProgressBar.Visible = False
                Me.BsProgressBar.Value = 0

                If (AnalyzerController.IsAnalyzerInstantiated) Then '#REFACTORING

                    MyClass.DisableAll()

                    ScreenWorkingProcess = True
                    MyBase.ActivateMDIMenusButtons(False)

                    MyClass.SendWASH_STATION_CTRL()

                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".bsChangeRotortButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsChangeRotortButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub


    Private Sub bsNewRotorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewRotorButton.Click
        Dim resultData As New GlobalDataTO

        Try

            If MyClass.myScreenDelegate.IsWashingStationUp Then
                bsStatusImage.Visible = False
                Me.BsProgressBar.Value = 0

                If (AnalyzerController.IsAnalyzerInstantiated) Then '#REFACTORING

                    MyClass.DisableAll()

                    ScreenWorkingProcess = True
                    MyBase.ActivateMDIMenusButtons(False)

                    MyClass.SendWASH_STATION_CTRL()

                    Me.BsProgressBar.Maximum = 100
                    Me.BsProgressBar.Visible = True
                    Me.BsNewRotorProcessTimer.Interval = 1000
                    Me.BsNewRotorProcessTimer.Enabled = True
                    Me.BsNewRotorProcessTimer.Start()

                End If
            End If

            Exit Sub




        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".bsContinueButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsContinueButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
#End Region

#Region "Analyzer Events"

    ''' <summary>
    ''' manages the response of the Analyzer after sending a FwScript List
    ''' The response can be OK, NG, Timeout or Exception
    ''' </summary>
    ''' <param name="pResponse">response type</param>
    ''' <param name="pData">data received</param>
    ''' <remarks>Created by SG 15/11/2012</remarks>
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
                Case ADJUSTMENT_MODES.PARKED
                    If pResponse = RESPONSE_TYPES.OK Then

                        Application.DoEvents()

                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If


                Case ADJUSTMENT_MODES.NEW_ROTOR_END
                    If pResponse = RESPONSE_TYPES.OK Then

                        If Not myGlobal.HasError Then
                            PrepareArea()
                        Else
                            PrepareErrorMode()
                            Exit Function
                        End If
                    End If

                Case ADJUSTMENT_MODES.ERROR_MODE
                    MyBase.DisplayMessage(Messages.FWSCRIPT_DATA_ERROR.ToString)
                    PrepareErrorMode()

            End Select


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ScreenReceptionLastFwScriptEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".ScreenReceptionLastFwScriptEvent", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobal.ErrorMessage, Me)
        End Try

        Return True
    End Function


#End Region

    Private Sub BsNewRotorProcessTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles BsNewRotorProcessTimer.Tick
        Try
            If Me.BsProgressBar.Visible Then
                If Me.BsProgressBar.Value < Me.BsProgressBar.Maximum Then
                    Me.BsProgressBar.Value = Me.BsProgressBar.Value + 1
                End If
            Else
                Me.BsNewRotorProcessTimer.Stop()
                Me.BsNewRotorProcessTimer.Enabled = False
                Me.BsProgressBar.Value = 0
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".BsNewRotorProcessTimer_Tick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".BsNewRotorProcessTimer_Tick ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub
End Class
