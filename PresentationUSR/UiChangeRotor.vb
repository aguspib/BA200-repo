﻿Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.App.PresentationLayerListener
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global.GlobalEnumerates

Public Class UiChangeRotor
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"
    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING
    Private MSG_StartInstrument As String

    Private OKIconName As String
    Private WRONGIconName As String

    Private DefaultLightAdjustTime As Integer
    Private ExistBaseLineInitError As Boolean = False
    Private statusMDIChangedFlag As Boolean = False
    Private _processIsPaused As Boolean = False
    Private _recoverProcess As Boolean = False 'BA-2216

#End Region

#Region "Methods"

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/06/2011
    ''' Modified by: DL 28/02/2012 - Added code to get Icons for the final result of the Rotor change process: OK or ERROR
    '''              IT 12/01/2015 - BA-2143
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath

            'CHANGE ROTOR Button
            auxIconName = GetIconName("CHANGEROTORB")
            If (auxIconName <> String.Empty) Then bsChangeRotortButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'CONTINUE Button
            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> String.Empty) Then bsContinueButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> String.Empty) Then bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'Icons for the final result of the Rotor change process: OK or ERROR
            auxIconName = GetIconName("ACCEPTF")
            If (auxIconName <> String.Empty) Then OKIconName = iconPath & auxIconName

            auxIconName = GetIconName("CANCELF")
            If (auxIconName <> String.Empty) Then WRONGIconName = iconPath & auxIconName

            'BA-2143 - INI
            'Flight Read Button
            auxIconName = GetIconName("BTN_DBL_READ")
            If (auxIconName <> String.Empty) Then bsChangeRotorReadButton.Image = Image.FromFile(iconPath & auxIconName)
            'Flight Empty & Finalize Button
            auxIconName = GetIconName("BTN_DBL_EMPTY")
            If (auxIconName <> String.Empty) Then bsChangeRotorFinalizeButton.Image = Image.FromFile(iconPath & auxIconName)
            'BA-2143 - END

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/06/2011
    ''' Modified by: IT 12/01/2015 - BA-2143
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ChangeRotor", currentLanguage)
            bsChangeRotorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RISINGUP", currentLanguage)
            bsNewAdjLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ADJUSTROTOR", currentLanguage)
            bsRepeatReadLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DYNAMICBASELINE_READ", currentLanguage) 'BA-2143
            bsEmptyAndFinalizeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DYNAMICBASELINE_EMPTY", currentLanguage) 'BA-2143

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsChangeRotortButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RISINGUP", currentLanguage))
            bsScreenToolTips.SetToolTip(bsContinueButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_NEWADJUSTROTOR", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
            bsScreenToolTips.SetToolTip(bsChangeRotorReadButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_DBL_READ", currentLanguage)) 'BA-2143
            bsScreenToolTips.SetToolTip(bsChangeRotorFinalizeButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_DBL_EMPTY", currentLanguage)) 'BA-2143

            'RH 14/02/2012
            MSG_StartInstrument = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_StartInstrument", currentLanguage)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable or disable the buttons according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 07/09/2012 - This function is called when the form loads and every Refresh
    ''' Modified by: XB 06/11/2013 - BT #1150 / BT #1151 ==> Added additional protections against other performing operations (Shutting Down, Aborting WS) 
    '''              SA 22/04/2014 - BT #1595 ==> Added Connected=False to the list of conditions verified to disable screen buttons 
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 19/12/2014 - BA-2143
    '''              IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub EnableButtons()
        Try
            Dim disableButtons As Boolean = False
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                'XB 06/11/2013 - ABORTprocess and SDOWNprocess added to the list of conditions for disable buttons
                'BT #1595 - Added Connected=False to the list of conditions verified to disable screen buttons 
                If (AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.STANDBY) OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS") OrElse _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS") OrElse _
                   (Not AnalyzerController.Instance.Analyzer.Connected) OrElse (Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady) OrElse _
                   (AnalyzerController.Instance.Analyzer.AnalyzerIsFreeze) Then
                    disableButtons = True

                    'If the Analyzer is STARTING (WarmUp process is executing), a warnin message is shown
                    If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS") Then
                        'RH 14/02/2012 Show MDI message
                        bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                    End If
                End If
            Else
                disableButtons = True
            End If

            If (disableButtons) Then
                bsChangeRotortButton.Enabled = False
                bsContinueButton.Enabled = False
                bsChangeRotorFinalizeButton.Enabled = False 'BA-2143
                bsChangeRotorReadButton.Enabled = False 'BA-2143
            Else
                If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") AndAlso _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = String.Empty) AndAlso _
                   (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = String.Empty) Then

                    bsChangeRotortButton.Enabled = False
                    bsContinueButton.Enabled = True
                    bsCancelButton.Enabled = False

                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "CLOSED") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "END") AndAlso _
                       ((AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END") OrElse
                        (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED")) Then 'BA-2216

                    bsChangeRotortButton.Enabled = True
                    bsContinueButton.Enabled = False
                    bsCancelButton.Enabled = True

                    If (dxProgressBar.Visible) Then
                        bsStatusImage.Visible = True
                    Else
                        bsStatusImage.Visible = False
                    End If

                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "END") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "INI") Then

                    bsChangeRotortButton.Enabled = True
                    bsContinueButton.Enabled = False
                    bsCancelButton.Enabled = True

                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "INI") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = String.Empty) Then

                    bsChangeRotortButton.Enabled = False
                    bsContinueButton.Enabled = False
                    bsCancelButton.Enabled = False

                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = String.Empty) Then

                    bsChangeRotortButton.Enabled = True
                    bsContinueButton.Enabled = False
                    bsCancelButton.Enabled = True

                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") AndAlso _
                       (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") Then 'BA-2216
                    bsChangeRotorFinalizeButton.Enabled = True
                    bsChangeRotorReadButton.Enabled = True
                Else
                    bsChangeRotortButton.Enabled = UiAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.RAISE_WASH_STATION)
                End If

                ConfigureDynamicBaselineArea() 'BA-2143

            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".EnableButtons " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/03/2011
    ''' REMARK
    ''' All Tools screens (CHANGE ROTOR, CONDITIONING, ISE and futures) 
    ''' must have the same logic about Open and Close functionalities
    ''' 
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'DL 28/07/2011
            Dim mySize As Size = Me.Parent.Size
            Dim myLocation As Point = Me.Parent.Location
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), _
                                    myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()
            Me.Opacity = 100

            Dim resultData As GlobalDataTO
            Dim swParams As New SwParametersDelegate

            resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myParametersDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)

                Dim myList As List(Of ParametersDS.tfmwSwParametersRow) = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                                                                          Where a.ParameterName = GlobalEnumerates.SwParameters.LIGHT_ADJUST_TIME.ToString _
                                                                         Select a).ToList
                If (myList.Count > 0) Then
                    DefaultLightAdjustTime = CInt(myList(0).ValueNumeric)
                Else
                    DefaultLightAdjustTime = 160
                End If
            End If

            'JB 07/09/2012 - The enable/disable buttons logic is in the EnableButtons function
            EnableButtons()

            dxProgressBar.Position = 0
            dxProgressBar.Visible = False
            dxProgressBar.Properties.Minimum = 0
            dxProgressBar.Properties.Maximum = DefaultLightAdjustTime

            AnalyzerController.Instance.PauseWarmUpService() 'IT 26/03/2015 - BA-2406

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' ''' <summary>
    ' ''' Not allow move form and mantain the center location in center parent
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by: DL 27/07/2011
    ' ''' </remarks>
    'Protected Overrides Sub WndProc(ByRef m As Message)
    '    If (m.Msg = WM_WINDOWPOSCHANGING) Then
    '        Dim mySize As Size = Me.Parent.Size
    '        Dim myLocation As Point = Me.Parent.Location
    '        Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

    '        pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
    '        pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70
    '        Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
    '    End If
    '    MyBase.WndProc(m)
    'End Sub

    Private Sub ExitScreen()
        Try

            AnalyzerController.Instance.ChangeRotorCloseProcess() 'BA-2143

            If (bsCancelButton.Enabled) Then
                If (statusMDIChangedFlag) Then
                    UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
                Else
                    UiAx00MainMDI.ErrorStatusLabel.Text = String.Empty
                End If

                'Open the WS Monitor form and close this one
                'If (Not Me.Tag Is Nothing) Then
                'A PerformClick() method was executed
                'Me.Close()
                'Else
                'Normal button click
                'Open the WS Monitor form and close this one
                UiAx00MainMDI.OpenMonitorForm(Me)
                'End If
            End If

            AnalyzerController.Instance.ReStartWarmUpService() 'IT 26/03/2015 - BA-2406

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".ExitScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    '''  Created by: IT 12/01/2015
    ''' </remarks>
    Private Sub ConfigureDynamicBaselineArea()

        If (AnalyzerController.Instance.Analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then
            bsRepeatReadLabel.Show()
            bsEmptyAndFinalizeLabel.Show()
            bsChangeRotorFinalizeButton.Show()
            bsChangeRotorReadButton.Show()
            bsPoint3Label.Show()
            bsPoint4Label.Show()
        Else
            bsRepeatReadLabel.Hide()
            bsEmptyAndFinalizeLabel.Hide()
            bsChangeRotorFinalizeButton.Hide()
            bsChangeRotorReadButton.Hide()
            bsPoint3Label.Hide()
            bsPoint4Label.Hide()
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 13/01/2015 - BA-2143
    ''' Modified by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub StartProgressBar()
        'DL 28/02/2012. Begin
        Dim secondsInPause As Integer = 0

        _processIsPaused = IsProcessPaused()

        secondsInPause = dxProgressBar.Position
        If dxProgressBar.Visible = False Then dxProgressBar.Visible = True
        'dxProgressBar.Show()

        Dim Dt1 As DateTime = DateTime.Now
        Dim Dt2 As DateTime
        Dim Span As TimeSpan

        bsContinueButton.Enabled = False
        Dim elapsed = Now.AddSeconds(0.1)
        While ScreenWorkingProcess AndAlso (Not _processIsPaused)
            Dt2 = DateTime.Now
            Span = Dt2.Subtract(Dt1)

            If (Span.TotalSeconds + secondsInPause) > DefaultLightAdjustTime Then
                dxProgressBar.Position = DefaultLightAdjustTime
            Else
                dxProgressBar.Position = CInt(Span.TotalSeconds + secondsInPause)
            End If

            If dxProgressBar.Visible = False Then dxProgressBar.Show()
            If elapsed < Now Then
                elapsed = Now.AddSeconds(0.1)
                Application.DoEvents()
            End If

            If Not AnalyzerController.Instance.Analyzer.Connected Then ScreenWorkingProcess = False 'DL 26/09/2012
        End While

        If (Not _processIsPaused) AndAlso (Not ScreenWorkingProcess) Then
            dxProgressBar.Position = dxProgressBar.Properties.Maximum
            If dxProgressBar.Visible = False Then dxProgressBar.Show()
            Application.DoEvents()

            'dl 26/09/2012
            If Not AnalyzerController.Instance.Analyzer.Connected Then
                UiAx00MainMDI.EnableButtonAndMenus(False)
            Else
                UiAx00MainMDI.EnableButtonAndMenus(True)
                UiAx00MainMDI.SetActionButtonsEnableProperty(True)
            End If

            If Not ExistBaseLineInitError Then
                bsStatusImage.Visible = True
                bsStatusImage.Image = ImageUtilities.ImageFromFile(OKIconName)

            Else
                bsStatusImage.Visible = True
                bsStatusImage.Image = ImageUtilities.ImageFromFile(WRONGIconName)
            End If

            'DL 26/09/2012. begin
            If Not AnalyzerController.Instance.Analyzer.Connected Then
                dxProgressBar.Visible = False
                bsStatusImage.Visible = False
                'bsCancelButton.Enabled = True
            End If
            'DL 26/09/2012. end
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub ChangeRotorStart()
        Dim resultData As New GlobalDataTO
        Dim processStarted As Boolean = False

        bsStatusImage.Visible = False 'DL 22/02/2012
        dxProgressBar.Position = 0
        _processIsPaused = False

        If (AnalyzerController.IsAnalyzerInstantiated) Then

            bsChangeRotortButton.Enabled = False
            bsContinueButton.Enabled = False
            bsChangeRotorFinalizeButton.Enabled = False 'BA-2143
            bsChangeRotorReadButton.Enabled = False 'BA-2143

            If AnalyzerController.Instance.Analyzer.ExistBottleAlarms Then
                'Show message
                ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                bsChangeRotortButton.Enabled = True
            Else
                ScreenWorkingProcess = True
                UiAx00MainMDI.EnableButtonAndMenus(False) 'AG 18/10/2011
                UiAx00MainMDI.SetActionButtonsEnableProperty(False) 'AG 12/07/2011 - Disable all vertical action buttons bar
            End If

            Try

                If (Not _recoverProcess) Then
                    processStarted = AnalyzerController.Instance.ChangeRotorStartProcess()
                End If

                If (processStarted Or _recoverProcess) Then
                    'Disable buttons
                    'DL 28/02/2012
                    dxProgressBar.Visible = False
                    bsStatusImage.Visible = False
                    'DL 28/02/2012

                    bsChangeRotortButton.Enabled = False
                    bsCancelButton.Enabled = False
                End If

            Catch ex As Exception
                ScreenWorkingProcess = False
                ShowMessage("Warning", resultData.ErrorCode)

                'DL 28/02/2012
                dxProgressBar.Visible = True
                dxProgressBar.Position = dxProgressBar.Properties.Maximum

                bsStatusImage.Image = ImageUtilities.ImageFromFile(WRONGIconName)
                bsStatusImage.Visible = True

                UiAx00MainMDI.EnableButtonAndMenus(True)
                UiAx00MainMDI.SetActionButtonsEnableProperty(True) 'Enable vertical action buttons bar
                'DL 28/02/2012
            End Try

        End If

        UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY) 'DL 04/04/2012
        Cursor = Cursors.Default        'DL 04/04/2012
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub ChangeRotorContinue()
        Dim resultData As New GlobalDataTO
        Dim processStarted As Boolean = False

        If (AnalyzerController.IsAnalyzerInstantiated) AndAlso AnalyzerController.Instance.Analyzer.ExistBottleAlarms Then 'AG 25/05/2012
            'Show message
            'ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
            'AG 25/05/2012
        Else
            bsChangeRotortButton.Enabled = False
            ExistBaseLineInitError = False
        End If

        Try
            processStarted = AnalyzerController.Instance.ChangeRotorContinueProcess(_recoverProcess)

            If (processStarted) Then
                UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.LIGHT_ADJUSTMENT)
                statusMDIChangedFlag = True
                StartProgressBar() 'BA-2143 
            End If
        Catch ex As Exception
            ShowMessage("Warning", resultData.ErrorCode)

            'DL 28/02/2012. begin
            dxProgressBar.Visible = False
            bsStatusImage.Visible = False
            'DL 28/02/2012. end
        End Try

        UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY) 'DL 04/04/2012
        Cursor = Cursors.Default        'DL 04/04/2012
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Function IsProcessPaused() As Boolean

        If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
            If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) <> "CANCELED") OrElse
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) <> "CANCELED") OrElse
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) <> "CANCELED") OrElse
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) <> "CANCELED") OrElse
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) <> "CANCELED") Then
                Return False
            End If
        End If

        If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
            If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "") Then
                Return False
            End If
        End If

        If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "CLOSED") Then
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub RefreshProgressBar()

        If (statusMDIChangedFlag) Then
            If IsProcessPaused() Then
                _processIsPaused = True
            Else
                If (_processIsPaused) Then
                    StartProgressBar()
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 13/02/2015 - BA-2266
    ''' </remarks>
    Private Sub WashstationControlPerformed()
        ScreenWorkingProcess = True
        UiAx00MainMDI.EnableButtonAndMenus(False) 'AG 18/10/2011
        UiAx00MainMDI.SetActionButtonsEnableProperty(False) 'AG 18/10/2011
        ManageVisibilityChangeRotorButton() 'IT 17/02/2015 - BA-2266
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 13/02/2015 - BA-2266
    ''' </remarks>
    Private Sub NewRotorPerformed()

        'Create a new ReactionsRotor
        Dim myGlobal As New GlobalDataTO
        Dim myReactionsRotorDelegate As New ReactionsRotorDelegate
        myGlobal = myReactionsRotorDelegate.ChangeRotor(Nothing, UiAx00MainMDI.ActiveAnalyzer, UiAx00MainMDI.AnalyzerModel)

        'DL 29/02/2012 - evaluate if the adjust ligth has been successfully or not
        Dim myAlarms As List(Of AlarmEnumerates.Alarms) = AnalyzerController.Instance.Analyzer.Alarms
        If (Not myAlarms.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR)) Then
            ScreenWorkingProcess = False 'Process finished
            ExistBaseLineInitError = False
        Else
            ExistBaseLineInitError = True
            ScreenWorkingProcess = False
        End If
        myAlarms = Nothing

        bsChangeRotortButton.Enabled = True
        bsCancelButton.Enabled = True

        'AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once updated UI clear sensor
        _recoverProcess = False

        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, _
                                                  GlobalEnumerates.Ax00InfoInstructionModes.STR) 'Start ANSINF
        If (myGlobal.HasError) Then ShowMessage("Warning", myGlobal.ErrorCode)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: IT 17/02/2015 - BA-2266
    ''' </remarks>
    Private Sub ManageVisibilityChangeRotorButton()

        If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Or
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then

            If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "") Then
                Dim enabled = UiAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_REACTIONS_ROTOR)
                If (enabled <> bsContinueButton.Enabled) Then
                    bsContinueButton.Enabled = enabled
                End If
            End If
        End If

    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Activate or deactivate control depending the current change rotor process state
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDs"></param>
    ''' <remarks>
    ''' Created by:  AG 29/06/2011
    ''' Modified by: SA 16/04/2014 - BT #1595 ==> Call to function EnableButtons has been commented to avoid bad enabling of the screen buttons
    '''                                           during the light adjustment process 
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 19/12/2014 - BA-2143
    '''              IT 30/01/2015 - BA-2216
    '''              IT 13/02/2015 - BA-2266
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDs As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            RefreshDoneField = False 'RH 28/03/2012
            If (isClosingFlag) Then Return 'AG 03/08/2012

            'BT #1595 - This call is commented to avoid bad enabling of the screen buttons during the light adjustment process
            'EnableButtons()     'JBL 07/09/2012 Enable/disable buttons 

            If (Not pRefreshEventType Is Nothing AndAlso pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED)) Then
                'If WashStation CTRL has been performed ... activate the label + CONTINUE button
                Dim sensorValue As Single = 0

                '1st step finish (Wash station UP) >> Now enable control to perform the 2on steps in utility process
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED)
                If (sensorValue = 1) Then
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED) = 0 'Once updated UI clear sensor
                    WashstationControlPerformed()
                End If

                '2nd step is finish when valid ALIGHT o not more chances!!
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED)
                If (sensorValue = 1) Then
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once updated UI clear sensor
                    NewRotorPerformed()
                End If

                'AG 15/03/2012 - When FREEZE appears while UI is disabled because screen is working Sw must reactivate UI
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FREEZE)
                If (sensorValue = 1) Then
                    ScreenWorkingProcess = False 'Process finished
                    ExistBaseLineInitError = True 'Wrong result

                    bsChangeRotortButton.Enabled = False
                    bsContinueButton.Enabled = False
                    bsCancelButton.Enabled = True

                    UiAx00MainMDI.EnableButtonAndMenus(True)
                    UiAx00MainMDI.SetActionButtonsEnableProperty(True)
                    Cursor = Cursors.Default
                End If
                'AG 15/03/2012

                'IT 19/12/2014 - BA-2143 (INI)
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.DYNAMIC_BASELINE_ERROR)
                If (sensorValue = 1) Then
                    bsChangeRotorReadButton.Enabled = True
                    bsChangeRotorFinalizeButton.Enabled = True
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.DYNAMIC_BASELINE_ERROR) = 0 'Once updated UI clear sensor
                End If
                'IT 19/12/2014 - BA-2143 (END)

                'IT 30/01/2015 - BA-2216  (INI)
                sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED)
                If (sensorValue = 1) Then
                    RefreshProgressBar()
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED) = 0 'Once updated UI clear sensor
                End If
                'IT 30/01/2015 - BA-2216  (END)

                RefreshDoneField = True 'RH 28/03/2012
            End If

            'AG 12/03/2012 - Reactions rotor missing alarm
            If (Not pRefreshEventType Is Nothing AndAlso pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED)) Then
                Dim linQAlarm = (From a As Biosystems.Ax00.Types.UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                            Where a.AlarmID = AlarmEnumerates.Alarms.REACT_MISSING_ERR.ToString _
                          AndAlso a.AlarmStatus = True _
                           Select a)

                If (linQAlarm.Count > 0) Then
                    ScreenWorkingProcess = False 'Process finished
                    ExistBaseLineInitError = True 'Wrong result

                    bsChangeRotortButton.Enabled = True
                    bsCancelButton.Enabled = True
                    RefreshDoneField = True 'RH 28/03/2012
                End If

                'AG 02/04/2012 - Raise WashStation is active once the ISE initialization finished
                If (bsChangeRotortButton.Enabled AndAlso Not UiAx00MainMDI Is Nothing) Then
                    bsChangeRotortButton.Enabled = UiAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.RAISE_WASH_STATION)
                Else
                    bsChangeRotortButton.Enabled = False
                End If
                'AG 02/04/2012

                'IT 17/02/2015 - BA-2266 (INI)
                'AG 29/03/2012 - no change rotor is reactions rotor cover enabled and opened (the nothing condition is to avoid create a new MDI instance)
                'Not necessary because Fw peforms the action although the reaction cover enabled & open
                'If (bsContinueButton.Enabled AndAlso Not UiAx00MainMDI Is Nothing) Then
                '    'bsContinueButton.Enabled = True 'IAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_REACTIONS_ROTOR)
                '    bsContinueButton.Enabled = UiAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_REACTIONS_ROTOR)
                'Else
                '    bsContinueButton.Enabled = False
                'End If
                'AG 29/03/2012
                'IT 17/02/2015 - BA-2266 (END)

            End If
            'AG 12/03/2012

            If (AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED) = 1) Then bsChangeRotortButton.Enabled = False 'DL 25/09/2012

            ManageVisibilityChangeRotorButton() 'BA-2266

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".RefreshScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 30/01/2015 - BA-2216
    ''' Modified by: AG 04/02/2015 - BA-2246 define public
    '''              IT 13/02/2015 - BA-2266
    ''' </remarks>
    Public Sub RecoverProcess()

        If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Or
            (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then

            _recoverProcess = True
            ChangeRotorStart()
            WashstationControlPerformed()
            If (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) <> "") Then
                ChangeRotorContinue()
            End If

        End If

    End Sub

#End Region

#Region "Events"

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        ExitScreen()
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
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IChangeRotor_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IChangeRotor_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 19/12/2014 - BA-2143
    '''              IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub bsChangeRotortButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsChangeRotortButton.Click
        Try
            GlobalBase.CreateLogActivity("Btn ChangeRotor", Me.Name & ".bsChangeRotortButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            ChangeRotorStart()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsChangeRotortButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsChangeRotortButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY) 'DL 04/04/2012
            Cursor = Cursors.Default        'DL 04/04/2012
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 19/12/2014 - BA-2143
    '''              IT 30/01/2015 - BA-2216
    ''' </remarks>
    Private Sub bsContinueButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsContinueButton.Click
        Try
            GlobalBase.CreateLogActivity("Btn Continue", Me.Name & ".bsContinueButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            ChangeRotorContinue()
        Catch ex As Exception
            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY) 'DL 04/04/2012
            Cursor = Cursors.Default        'DL 04/04/2012

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsContinueButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsContinueButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub bsChangeRotorReadButton_Click(sender As Object, e As EventArgs) Handles bsChangeRotorReadButton.Click
        Try

            GlobalBase.CreateLogActivity("Btn Read", Me.Name & ".bsChangeRotorReadButton_Click", EventLogEntryType.Information, False)
            bsChangeRotorFinalizeButton.Enabled = False
            bsChangeRotorReadButton.Enabled = False
            bsChangeRotortButton.Enabled = False
            AnalyzerController.Instance.ChangeRotorRepeatDynamicBaseLineReadStep()

            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
            Cursor = Cursors.Default

        Catch ex As Exception
            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
            Cursor = Cursors.Default

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsChangeRotorReadButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsChangeRotorReadButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub bsChangeRotorFinalizeButton_Click(sender As Object, e As EventArgs) Handles bsChangeRotorFinalizeButton.Click
        Try

            GlobalBase.CreateLogActivity("Btn Finalize", Me.Name & ".bsChangeRotorFinalizeButton_Click", EventLogEntryType.Information, False)

            If (AnalyzerController.Instance.Analyzer.ExistBottleAlarms) Then
                ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
            Else
                bsChangeRotorFinalizeButton.Enabled = False
                bsChangeRotorReadButton.Enabled = False
                bsChangeRotortButton.Enabled = False
                AnalyzerController.Instance.ChangeRotorFinalizeProcess()
            End If

        Catch ex As Exception
            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
            Cursor = Cursors.Default

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsChangeRotorFinalizeButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsChangeRotorFinalizeButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
