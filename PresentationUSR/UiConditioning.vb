Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.App

'Imports Biosystems.Ax00.Types

Public Class UiConditioning
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"
    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING
    Private statusMDIChangedFlag As Boolean = False
    Protected IsConditioning As Boolean = False
    Private DefaultConditioningTime As Integer
    Private ExistConditioningError As Boolean
    Private MSG_StartInstrument As String
    Private MSG_ReactionRotorMissing As String
#End Region

#Region "Methods"

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 05/06/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'CONDITIONING Button
            auxIconName = GetIconName("CONDITIONING")
            If Not String.Equals(auxIconName, String.Empty) Then bsConditioningButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If Not String.Equals(auxIconName, String.Empty) Then bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            auxIconName = GetIconName("ACCEPTF")
            If Not String.Equals(auxIconName, String.Empty) Then bsStatusImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 05/06/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CONDITIONING_TITLE", currentLanguage)
            bsConditioningLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CONDITIONING", currentLanguage)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsConditioningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_CONDITIONING", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            MSG_StartInstrument = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_StartInstrument", currentLanguage)
            MSG_ReactionRotorMissing = myMultiLangResourcesDelegate.GetResourceText(Nothing, "REACT_MISSING", currentLanguage)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 05/06/2012
    ''' Modified by XBC - 11/07/2012
    ''' REMARK
    ''' All Tools screens (CHANGE ROTOR, CONDITIONING, ISE and futures) 
    ''' must have the same logic about Open and Close functionalities
    ''' Modified by: XB 06/11/2013 - Add protection against more performing operations (Shutting down, aborting WS) - BT #1150 + #1151
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)

            Dim swParams As New SwParametersDelegate
            Dim resultData As GlobalDataTO

            resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                Dim myParametersDS As ParametersDS

                myParametersDS = CType(resultData.SetDatos, ParametersDS)
                'Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)
                Dim myList As List(Of ParametersDS.tfmwSwParametersRow)

                myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                          Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.CONDITIONING_TIME.ToString) _
                          Select a).ToList

                DefaultConditioningTime = If(myList.Count > 0, CInt(myList(0).ValueNumeric), 90)

            End If

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()
            Me.Opacity = 100

            Dim disableButtons As Boolean = False

            'AG 22/11/2012 - v055 Button disabled when NO reactions rotor alarm!!! (not temperarture alarms)
            If (AnalyzerController.IsAnalyzerInstantiated) And _
               Not (AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_MISSING_ERR)) Then

                'If Not mdiAnalyzerCopy Is Nothing And _
                '   Not (AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_ERR) Or _
                '   AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS_ERR)) Then
                '    '    If Not mdiAnalyzerCopy Is Nothing And Not _
                '    '         (AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_ERR) Or _
                '    '         AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS1_ERR) Or _
                '    '        AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS2_ERR)) Then

                If AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    disableButtons = True

                    If String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") Then
                        'Show MDI message
                        bsScreenErrorProvider.ShowError(MSG_StartInstrument)
                    End If
                End If

            ElseIf (AnalyzerController.IsAnalyzerInstantiated) Then
                'AG 22/11/2012 - v055 Button disabled when NO reactions rotor alarm!!! (not temperarture alarms)
                If AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_MISSING_ERR) Then

                    ''DL 31/07/2012. Begin
                    'If (AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_ERR) Or _
                    '    AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS_ERR)) Then
                    '    '    If (AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_ERR) Or _
                    '    '    AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS1_ERR) Or _
                    '    '    AnalyzerController.Instance.Analyzer.Alarms.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS2_ERR)) Then
                    '    'DL 31/07/2012. End

                    disableButtons = True

                    'Show MDI message
                    bsScreenErrorProvider.ShowError(MSG_ReactionRotorMissing)
                End If

            Else
                disableButtons = True
            End If

            ' XBC 11/07/2012
            If String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") OrElse _
               String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") OrElse _
               String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "INPROCESS") OrElse _
               Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady OrElse _
               AnalyzerController.Instance.Analyzer.AnalyzerIsFreeze Then
                ' XB 06/11/2013 - ABORTprocess and SDOWNprocess added 
                disableButtons = True
            End If
            ' XBC 11/07/2012

            dxProgressBar.Position = 0
            dxProgressBar.Visible = False   'DL 18/07/2011
            dxProgressBar.Properties.Minimum = 0
            dxProgressBar.Properties.Maximum = DefaultConditioningTime

            bsConditioningButton.Enabled = If(disableButtons, False, True)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 05/06/2012
    ''' </remarks>
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

    Private Sub ExitScreen()
        Try
            If bsCancelButton.Enabled Then
                If statusMDIChangedFlag Then
                    UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
                End If

                'This is the standard way of closing MDI forms, not the previous one
                If Not Me.Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    UiAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".ExitScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Activate or deactivate control depending the current condtioning process state
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>DL 05/06/2012</remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                       ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            'RefreshDoneField = False
            If isClosingFlag Then Return 'AG 03/08/2012

            If IsConditioning Then
                If Not pRefreshEventType Is Nothing AndAlso pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                    If String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess), "CLOSED") Then '#REFACTORING

                        ScreenWorkingProcess = True
                        UiAx00MainMDI.EnableButtonAndMenus(True)
                        UiAx00MainMDI.SetActionButtonsEnableProperty(True)

                        'dxProgressBar.Position = 0
                        bsStatusImage.Visible = True
                        dxProgressBar.Position = dxProgressBar.Properties.Maximum '0
                        dxProgressBar.Show()
                        Application.DoEvents()
                        'dxProgressBar.Visible = False

                        bsConditioningButton.Enabled = True
                        bsCancelButton.Enabled = True
                    End If
                End If

                If Not pRefreshEventType Is Nothing AndAlso pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                    Dim linQAlarm As List(Of Biosystems.Ax00.Types.UIRefreshDS.ReceivedAlarmsRow)
                    linQAlarm = (From a As Biosystems.Ax00.Types.UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                                 Where String.Equals(a.AlarmID, AlarmEnumerates.Alarms.REACT_MISSING_ERR.ToString) _
                                 And a.AlarmStatus = True Select a).ToList

                    If linQAlarm.Count > 0 Then
                        ScreenWorkingProcess = False 'Process finished
                        ExistConditioningError = True 'Wrong result
                        bsCancelButton.Enabled = True
                        UiAx00MainMDI.EnableButtonAndMenus(True)
                        UiAx00MainMDI.SetActionButtonsEnableProperty(True)
                        Cursor = Cursors.Default
                        RefreshDoneField = True 'RH 28/03/2012
                    End If
                End If
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".RefreshScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        ExitScreen()
    End Sub

    Private Sub IConditioning_KeyDown(ByVal sender As Object, _
                                      ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If (e.KeyCode = Keys.Escape) Then ExitScreen()
    End Sub

    Private Sub IConditioning_Load(ByVal sender As Object, _
                                   ByVal e As System.EventArgs) Handles Me.Load
        Try
            InitializeScreen()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IConditioning_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IConditioning_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub bsConditioningButton_Click(ByVal sender As System.Object, _
                                           ByVal e As System.EventArgs) Handles bsConditioningButton.Click

        Dim resultData As New GlobalDataTO

        bsStatusImage.Visible = False
        Try
            GlobalBase.CreateLogActivity("Btn Conditioning", Me.Name & ".bsConditioningButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            If (AnalyzerController.IsAnalyzerInstantiated) AndAlso AnalyzerController.Instance.Analyzer.ExistBottleAlarms Then
                'Show message
                ShowMessage("Warning", GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                'Comment next 3 lines. They belongs to another utility screen!!
                'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED"
                'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "CANCELED"
                'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED"
                AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess) = "PAUSED"

            Else
                If (AnalyzerController.IsAnalyzerInstantiated) AndAlso AnalyzerController.Instance.Analyzer.Connected Then
                    'Turn off Sound alarm
                    AnalyzerController.Instance.Analyzer.StopAnalyzerRinging()
                    ExistConditioningError = False

                    IsConditioning = True

                    'disable buttons
                    ScreenWorkingProcess = True
                    UiAx00MainMDI.EnableButtonAndMenus(False)
                    UiAx00MainMDI.SetActionButtonsEnableProperty(False)
                    bsConditioningButton.Enabled = False
                    bsCancelButton.Enabled = False

                    resultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True)
                    AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONDITIONINGprocess) = "INPROCESS"

                    If Not resultData.HasError AndAlso AnalyzerController.Instance.Analyzer.Connected Then
                        UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.SRV_CONDITIONING)
                        statusMDIChangedFlag = True
                        'AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once instruction has been sent clear sensor

                        dxProgressBar.Position = 0
                        dxProgressBar.Visible = True
                        dxProgressBar.Show()

                        Dim Dt1 As DateTime = DateTime.Now
                        Dim Dt2 As DateTime
                        Dim Span As TimeSpan

                        While ScreenWorkingProcess
                            Dt2 = DateTime.Now
                            Span = Dt2.Subtract(Dt1)

                            dxProgressBar.Position = If(Span.TotalSeconds > DefaultConditioningTime, DefaultConditioningTime, CInt(Span.TotalSeconds))


                            'If dxProgressBar.Position < dxProgressBar.Properties.Maximum Then
                            dxProgressBar.Show()
                            'Else
                            'dxProgressBar.Visible = False
                            'End If

                            Application.DoEvents()
                        End While

                        'dxProgressBar.Visible = False
                        bsStatusImage.Visible = True
                        dxProgressBar.Position = 0 'dxProgressBar.Properties.Maximum
                        dxProgressBar.Show()
                        Application.DoEvents()

                        UiAx00MainMDI.EnableButtonAndMenus(True)
                        UiAx00MainMDI.SetActionButtonsEnableProperty(True)

                    ElseIf resultData.HasError Then
                        ShowMessage("Warning", resultData.ErrorCode)

                        dxProgressBar.Visible = False
                    End If
                End If

            End If

            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
            Cursor = Cursors.Default

        Catch ex As Exception
            UiAx00MainMDI.ShowStatus(GlobalEnumerates.Messages.STANDBY)
            Cursor = Cursors.Default

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsConditioningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsConditioningButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        UiAx00MainMDI.EnableButtonAndMenus(True)
        UiAx00MainMDI.SetActionButtonsEnableProperty(True)

    End Sub

#End Region

End Class
