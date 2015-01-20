Option Explicit On
Option Strict On
Option Infer On


'Put here your business code for the tab MainTab inside Monitor Form

'Imports System.Text
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global
Imports DevExpress.XtraEditors.Controls
Imports System.IO

Partial Public Class IMonitor

    Dim rnd As Random = New Random(DateTime.Now.Second)

    Private Shared MainTabEnabled As Boolean = False

    Private Sub InitializeMainTab()
        Try
            'ISE Lock Icon SGM 21/03/2012
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = GetIconName("STUS_LOCKED")
            If System.IO.File.Exists(iconPath & auxIconName) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                If myImage IsNot Nothing Then
                    Me.BsISELongTermDeactivated.BackgroundImage = myImage
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeMainTab ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    'RH
    Public Sub ShowActiveAlerts(Optional ByVal CanHide As Boolean = True)
        If ChangingTab Then Return
        If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

        If (Not MainMDI Is Nothing) AndAlso (MainMDI.WindowState = FormWindowState.Normal) Then
            'AG 08/01/2014
            'If (Not MonitorTabs.SelectedTabPage Is Nothing) AndAlso (String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name)) Then
            If (Not MonitorTabs Is Nothing) AndAlso (Not MonitorTabs.SelectedTabPage Is Nothing) _
                AndAlso (String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name)) Then

                For Each ChildForm As Form In MainMDI.OwnedForms
                    If (TypeOf ChildForm Is bsAlert) Then
                        'RH 10/02/2012 Take into account that SelectedTabPage or WindowState can change in each cycle loop
                        If CType(ChildForm, bsAlert).Active AndAlso String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name) _
                                AndAlso MainMDI.WindowState = FormWindowState.Normal Then
                            'DL 20/09/2012
                            'If Not MonitorTabs Is Nothing AndAlso CType(ChildForm, bsAlert).Active AndAlso String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name) _
                            '   AndAlso MainMDI.WindowState = FormWindowState.Normal Then

                            CType(ChildForm, bsAlert).Show()

                            'RH 10/02/2012 Take into account that SelectedTabPage can change in each cycle loop
                            'DL 20/09/2012
                            If Not String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name) Then
                                'If Not MonitorTabs Is Nothing AndAlso Not String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name) Then
                                ChildForm.Hide()
                            End If
                        Else
                            CType(ChildForm, bsAlert).Hide()
                        End If

                        Application.DoEvents()
                    End If
                Next

                'AG 20/07/2012
                If Not mdiAnalyzerCopy Is Nothing AndAlso (Not mdiAnalyzerCopy.Connected OrElse _
                   (Not mdiAnalyzerCopy.Alarms Is Nothing AndAlso mdiAnalyzerCopy.Alarms.Contains(GlobalEnumerates.Alarms.COMMS_ERR))) Then
                    'Leave globe alarms empty 
                    Dim globeAlarmsList As New List(Of GlobalEnumerates.Alarms)
                    UpdateCoverImages(globeAlarmsList)
                Else
                    UpdateCoverImages(mdiAnalyzerCopy.Alarms)
                End If
                'AG 20/07/2012

                'DL 20/09/2012
                'If Not MdiParent Is Nothing Then

                Me.MdiParent.Activate()

                'RH 10/02/2012 Just in case SelectedTabPage or WindowState has changed while Alert globes were in showing process
                If MainMDI.WindowState = FormWindowState.Minimized OrElse Not String.Equals(MonitorTabs.SelectedTabPage.Name, MainTab.Name) Then
                    HideActiveAlerts()
                End If
            Else
                If CanHide Then HideActiveAlerts()
            End If
        Else
            If CanHide Then HideActiveAlerts()
        End If
    End Sub

    'RH
    Public Sub HideActiveAlerts()
        'If Not MonitorTabs.SelectedTabPage Is Nothing AndAlso MonitorTabs.SelectedTabPage.Name = MainTab.Name Then
        If Not MainMDI Is Nothing Then
            For Each ChildForm As Form In MainMDI.OwnedForms
                If TypeOf ChildForm Is bsAlert Then
                    ChildForm.Hide()
                End If
            Next

            Application.DoEvents()
        End If
    End Sub

    'RH
    Private Sub UpdateAlertsPositions()
        If Not MainMDI Is Nothing Then
            For Each ChildForm As Form In MainMDI.OwnedForms
                If (TypeOf ChildForm Is bsAlert) Then
                    CType(ChildForm, bsAlert).UpdatePosition()
                End If
            Next

            Application.DoEvents()
        End If
    End Sub

    'RH
    Public Sub UpdateGlobesInMainTab(ByVal pAlarms As List(Of GlobalEnumerates.Alarms))
        Try

            'AG 21/05/2012 - if not communications do not remove analyzermanager alarms, only do not show them
            Dim globeAlarmsList As New List(Of GlobalEnumerates.Alarms)
            If (Not pAlarms Is Nothing AndAlso pAlarms.Contains(GlobalEnumerates.Alarms.COMMS_ERR)) OrElse _
               (Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiAnalyzerCopy.Connected) Then
                'If no comms leave globe alarms empty 
            Else
                'Sleeping status DOES NOT SHOW globes
                For Each idAlarm As GlobalEnumerates.Alarms In pAlarms
                    globeAlarmsList.Add(idAlarm)
                Next
            End If
            'AG 21/05/2012

            'AG 20/07/2012 - move this code into ShowActiveAlerts method
            'UpdateCoverImages(globeAlarmsList) 'AG 23/05/2012 use local list, not the parameter list (pAlarms) 'RH 22/03/2012

            Dim appendFlag As Boolean = False

            While MainMDI.LoadingGlobes
                System.Threading.Thread.Sleep(1)
                Application.DoEvents()
            End While
            'TR 25/09/2013 -#memory declare outside the for.
            Dim AlertAlarms As List(Of GlobalEnumerates.Alarms) = Nothing
            For Each Alert As bsAlert In MainMDI.AlertList 'Loops through all bsAlert form dialogs
                'TR 25/09/2013 -#memory Set declaration Outside the for.
                'Dim AlertAlarms As List(Of GlobalEnumerates.Alarms) = CType(Alert.Tag, List(Of GlobalEnumerates.Alarms)) 'Gets associated alarms
                'TR 25/09/2013 -#memory Set value to nothing and set the new value.

                AlertAlarms = Nothing
                AlertAlarms = CType(Alert.Tag, List(Of GlobalEnumerates.Alarms)) 'Gets associated alarms

                Alert.ClearDescription()

                For Each Alarm As GlobalEnumerates.Alarms In AlertAlarms
                    If globeAlarmsList.Contains(Alarm) Then 'AG 23/05/2012 use local list, not the parameter list (pAlarms)
                        If Alarm = GlobalEnumerates.Alarms.BASELINE_INIT_ERR AndAlso Not mdiAnalyzerCopy Is Nothing Then
                            'AG 06/09/2012 - The property ShowBaseLineInitializationFailedMessage has to be used only in StandBy
                            '                in others status as Running the globe must appear
                            'If mdiAnalyzerCopy.ShowBaseLineInitializationFailedMessage Then
                            '    appendFlag = True 'All ALGIHT temptatives have failed show alert
                            'Else
                            '    appendFlag = False 'Temptative pending NO show alert
                            'End If
                            If mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                                If mdiAnalyzerCopy.ShowBaseLineInitializationFailedMessage Then
                                    appendFlag = True 'All ALGIHT temptatives have failed show alert
                                Else
                                    appendFlag = False 'Temptative pending NO show alert
                                End If
                            Else
                                appendFlag = True
                            End If
                            'AG 06/09/2012
                        Else
                            appendFlag = True
                        End If

                        If appendFlag Then
                            Alert.AppendDescription("● " + MainMDI.AlertText(Alarm))
                            If Alarm.ToString().Contains("ERR") Then Alert.IsAlertWarning = False Else Alert.IsAlertWarning = True
                        End If
                        'AG 24/02/2012
                    End If
                Next

                Alert.RefreshDescription()

                Alert.Active = (Alert.Description.Length > 0)
                Application.DoEvents()
            Next

            ShowActiveAlerts()

            'TR 25/09/2013 #memory set value to nothing to release memory
            AlertAlarms = Nothing


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateGlobesInMainTab ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateGlobesInMainTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    'RH 22/03/2012
    Private Sub UpdateCoverImages(ByVal pAlarms As List(Of GlobalEnumerates.Alarms))
        'If Not MainTabEnabled Then Return 'AG 18/05/2012

        CoverOffPicture.Visible = (pAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN) OrElse _
                                   pAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_ERR))

        CoverOnPicture.Visible = Not CoverOffPicture.Visible

        CoverReactionsPicture.Visible = Not (pAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN) OrElse _
                                      pAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_ERR))

        CoverSamplesPicture.Visible = Not (pAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_WARN) OrElse _
                                      pAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_ERR))

        CoverReagentsPicture.Visible = Not (pAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN) OrElse _
                                      pAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR))

        Application.DoEvents()

    End Sub

    ''' <summary>
    ''' Activate/deactivate main tab (start Instrument)
    ''' </summary>
    ''' <param name="pEnable">Indicates if main tab have to be activated or deactivated</param>
    ''' <remarks>
    ''' Created by:  DL 26/03/2012
    ''' Modified by RH 27/03/2012
    ''' </remarks>
    Public Sub SetEnableMainTab(ByVal pEnable As Boolean, Optional ByVal JustDoIt As Boolean = False)
        Try
            If JustDoIt OrElse (MainTabEnabled <> pEnable) Then
                MainTabEnabled = pEnable

                If Not pEnable Then
                    'DL 17/05/2012. BEGIN
                    'MainTab.Appearance.PageClient.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\BackgroundMainMonitor-Start.png")
                    'CoverReagentsPicture.Visible = False
                    'CoverReactionsPicture.Visible = False
                    'CoverSamplesPicture.Visible = False
                    'CoverOffPicture.Visible = False
                    'CoverOnPicture.Visible = False

                    MainTab.Appearance.PageClient.Image = Image.FromFile(IconsPath & "Embedded\BackgroundMainMonitor_DIS.png")
                    'MainTab.Appearance.PageClient.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\BackgroundMainMonitor_DIS.png")
                    CoverReagentsPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverReagents_DIS.png")
                    CoverReactionsPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverReactions_DIS.png")
                    CoverSamplesPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverSamples_DIS.png")
                    CoverOffPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverAnalyzerOff_DIS.png")
                    CoverOnPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverAnalyzerOn_DIS.png")

                    CoverReagentsPicture.Visible = True
                    CoverReactionsPicture.Visible = True
                    CoverSamplesPicture.Visible = True
                    CoverOffPicture.Visible = True
                    CoverOnPicture.Visible = True
                    'DL 17/05/2012. END

                Else
                    MainTab.Appearance.PageClient.Image = Image.FromFile(IconsPath & "Embedded\BackgroundMainMonitor.png")
                    'MainTab.Appearance.PageClient.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\BackgroundMainMonitor.png")

                    'DL 17/05/2012. BEGIN
                    CoverReagentsPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverReagents.png")
                    CoverReactionsPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverReactions.png")
                    CoverSamplesPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverSamples.png")
                    CoverOffPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverAnalyzerOff.png")
                    CoverOnPicture.Image = ImageUtilities.ImageFromFile(IconsPath & "Embedded\CoverAnalyzerOn.png")
                    'DL 17/05/2012. END

                    CoverReagentsPicture.Visible = True
                    CoverReactionsPicture.Visible = True
                    CoverSamplesPicture.Visible = True
                    CoverOffPicture.Visible = True
                    CoverOnPicture.Visible = True
                End If
                UpdateCoverImages(mdiAnalyzerCopy.Alarms) 'AG 18/05/2012
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetEnablemainTab", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetEnablemainTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Public Sub UpdateContainerLevels() 'Pass a list of levels
        'Simulation Code
        'chart2.Series("Series1").Points(1).YValues(0) = rnd.Next Mod 101
        'chart2.Series("Series1").Points(2).YValues(0) = rnd.Next Mod 101

        'Real Code
        'Dim myAlarms As New List(Of GlobalEnumerates.Alarms)

        If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

        Dim myAlarms As List(Of GlobalEnumerates.Alarms)
        myAlarms = mdiAnalyzerCopy.Alarms 'Get the current alarms in analyzer

        Dim sensorValue As Single = 0
        sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION)
        chart2.Series("Series1").Points(1).YValues(0) = sensorValue
        WashingLabel.Text = sensorValue.ToStringWithPercent(0) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena) 'RH 20/10/2011

        sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE)
        chart2.Series("Series1").Points(2).YValues(0) = sensorValue
        WasteLabel.Text = sensorValue.ToStringWithPercent(0) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena) 'RH 20/10/2011

        'TR 30/09/2011 -Format the values (ask AG the desired format)
        ''Dim myUtil As New Utilities.
        'WashingLabel.Text = Utilities.ToStringWithFormat(chart2.Series("Series1").Points(1).YValues(0), 1) & "%"
        'WasteLabel.Text = Utilities.ToStringWithFormat(chart2.Series("Series1").Points(2).YValues(0), 1) & "%"
        'TR 30/09/2011 -END.

        'RH 20/10/2011
        'Function Utilities.ToStringWithFormat() does not work properly and has some problems in its definition and use.
        'It does not remove comma zeroes (,0...) for every case. The problem it try to solve is not trivial.
        'I replaced it by the extension method ToStringWithPercent().
        'Just run the following lines in debug mode to compare both results. Try different decimal values.

        ''Dim myUtil As New Utilities.
        'Dim decimals As Integer = 1 'or 2, or 3...
        'Dim s As Single = 1.1234 'Or Double
        'Dim st As String = s.ToStringWithPercent(decimals)
        'st = Utilities.ToStringWithFormat(s, decimals)

        's = 1.01987
        'st = s.ToStringWithPercent(decimals)
        'st = Utilities.ToStringWithFormat(s, decimals)

        's = 1.001
        'st = s.ToStringWithPercent(decimals)
        'st = Utilities.ToStringWithFormat(s, decimals)

        's = 1.0001
        'st = s.ToStringWithPercent(decimals)
        'st = Utilities.ToStringWithFormat(s, decimals)

        chart2.ResetAutoValues()

        'TR 25/09/2013 #memory 
        myAlarms = Nothing

    End Sub

End Class