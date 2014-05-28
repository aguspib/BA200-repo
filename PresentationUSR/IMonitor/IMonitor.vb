Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.BL
Imports DevExpress.XtraGrid.Views.Grid 'DL 24/07/2012
Imports System.Timers 'AG 31/01/2014 - BT #1486


' Remarks
' -------
' Use the PageClient property to customize the appearance of the tab page's client region. It allows the background and foreground colors, 
' the gradient mode and a background image to be specified.
'
' Last Update 29/08/2011
' SamplesTab. Appearance. PageClient. Image = ..\Debug\Images\SampleRotor.png
' ReagentsTab. Appearance. PageClient. Image = ..\Debug\Images\ReagentRotor.png
'
Public Class IMonitor

#Region "Declaration"

    Private LanguageID As String
    Private mdiAnalyzerCopy As AnalyzerManager
    Private MainMDI As IAx00MainMDI
    Private myMultiLangResourcesDelegate As MultilanguageResourcesDelegate
    Private Shared IsFirstLoading = True

    'TR 14/11/2011 -Variable used to calculated the session time 
    Private WSStartDateTime As New DateTime
    Public remainingTime As Single
    'TR 14/11/2011 -END.

    Private ChangingTab As Boolean = False 'RH 09/02/2012
    Private testbackgroundMaintab As Boolean = True 'TO DEL by DL 26/03/2012
    Private autoWSCreationTimer As New Timer() 'AG 31/01/2014 - BT #1486 (v211 patch D + v300)
#End Region

#Region "Fields"

    Private AnalyzerIDField As String = String.Empty
    Private WorkSessionIDField As String = String.Empty
    Private WorkSessionStatusField As String = String.Empty

    'TR 30/11/2011 -Varible used to indicate if there was a session change
    Private WorkSessionChangedField As Boolean
    Private OpenByAutomaticProcessAttribute As Boolean = False 'AG 09/07/2013

    ' XB 17/07/2013 - Auto WS process
    Private AutoWSCreationWithLISModeAttribute As Boolean = False
#End Region

#Region "Properties"

    Public Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDField
        End Get
        Set(ByVal value As String)
            AnalyzerIDField = value
        End Set
    End Property

    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDField
        End Get
        Set(ByVal value As String)
            WorkSessionIDField = value
        End Set
    End Property

    Public Property CurrentWorkSessionStatus() As String
        Get
            Return WorkSessionIDField
        End Get
        Set(ByVal value As String)
            WorkSessionStatusField = value
        End Set
    End Property


    ''' <summary>
    ''' Varible used to indicate if there was a session change
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WorkSessionChange() As Boolean
        Get
            Return WorkSessionChangedField
        End Get
        Set(ByVal value As Boolean)
            WorkSessionChangedField = value
        End Set
    End Property

    'SGM 15/05/2012 for enabling shutdown button while 'Terminate' button is visible
    Public ReadOnly Property IsEndWarmUpButtonVisible() As Boolean
        Get
            Return bsEndWarmUp.Visible
        End Get
    End Property

    ''' <summary>
    ''' Indicates when the screen has been open by user (FALSE) or when by the automatic WS creation with LIS process (TRUE)
    ''' AG 09/07/2013
    ''' </summary>
    Public WriteOnly Property OpenByAutomaticProcess() As Boolean
        Set(ByVal value As Boolean)
            OpenByAutomaticProcessAttribute = value
        End Set
    End Property

    ' XB 17/07/2013 - Auto WS process
    Public WriteOnly Property AutoWSCreationWithLISMode() As Boolean
        Set(ByVal value As Boolean)
            AutoWSCreationWithLISModeAttribute = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Sub bsAlert_Move(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Not Me.MdiParent Is Nothing Then
            If Me.MdiParent.WindowState = FormWindowState.Normal Then
                UpdateAlertsPositions()
            End If
        End If
    End Sub

    Public Sub UpdateTimes(ByVal pRunningStatus As Boolean, ByVal pInitializing As Boolean, _
                           ByVal pChangesInWS As Boolean, Optional ByVal pPausedElements As Boolean = False)
        Try
            'Debug.Print("UpdateTimes")
            'Debug.Print("INICIALIZACION:" & pInitializing.ToString())
            'Debug.Print("running status:" & pRunningStatus.ToString())
            'Debug.Print("changes in WS :" & pChangesInWS.ToString())
            'Debug.Print("Paused elements :" & pPausedElements.ToString())

            If isClosingFlag Then Return 'AG 10/02/2014 - #1496 No refresh is screen is closing

            'Dim remainingTime As Single = 0
            Dim resultData As New GlobalDataTO
            Dim myWSDelegate As New WorkSessionsDelegate
            myWSDelegate.TotalSecElapsedTime = IAx00MainMDI.LocalTotalSecs2

            If (Not pRunningStatus) Then
                Dim initialHour As Date = "00:00:00"
                'TR 1/11/2011 -Validate the CLOSED Status.
                If (String.Equals(WorkSessionStatusField, "EMPTY") OrElse String.Equals(WorkSessionStatusField, "OPEN") OrElse _
                    String.Equals(WorkSessionStatusField, "CLOSED") OrElse String.Equals(WorkSessionStatusField, "ABORTED")) Then
                    'Initialize all time controls...
                    OverallTimeTextEdit.Text = initialHour.ToString("HH:mm:ss")
                    ElapsedTimeTextEdit.Text = initialHour.ToString("HH:mm:ss")
                    RemainingTimeTextEdit.Text = initialHour.ToString("HH:mm:ss")

                Else
                    'TR & DL 20/09/2012 -Validate if reset session is launched.
                    If Not IAx00MainMDI.isResetWorkSessionActive Then
                        'Calculate the total WS remaining time including the waiting cycles before Running
                        resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, True)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            remainingTime = DirectCast(resultData.SetDatos, Single)
                            'Set initial values
                            ElapsedTimeTextEdit.Text = initialHour.ToString("HH:mm:ss")
                            RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                            'OverallTimeTextEdit.Text = RemainingTimeTextEdit.Text
                            'TR 14/11/2011 -Set the value into local variable.
                            IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime)
                            OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")

                            If remainingTime > 0 Then
                                'Set tht maximum value to the progress bar.
                                TaskListProgressBar.Properties.Maximum = remainingTime 'IntialRemainingTime.TimeOfDay.TotalSeconds
                            End If
                            'Verify if the WorkSession has been already started
                            resultData = myWSDelegate.GetByWorkSession(Nothing, WorkSessionIDField)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                                If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                                    WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime
                                    ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                                    OverallTimeTextEdit.Text = ConvertSecondsInHHmmss(IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds() + _
                                                                                       IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds).ToString("HH:mm:ss")
                                    IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds() + _
                                                                                       IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds).ToString("HH:mm:ss")
                                End If
                            End If
                        End If
                    End If

                End If
            Else
                'TR 06/09/2012 -Validate if reset session is launched.
                If Not IAx00MainMDI.isResetWorkSessionActive Then
                    'Get value of SwParameter conatining the time in seconds for each Analyzer Cycle
                    Dim cycleMachineTime As Single = 0
                    Dim myParametersDS As New ParametersDS
                    Dim mySWParametersDelegate As New SwParametersDelegate

                    resultData = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, AnalyzerIDField, _
                                                                               GlobalEnumerates.SwParameters.CYCLE_MACHINE.ToString, True)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                        If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then cycleMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                    End If

                    If (Not resultData.HasError) Then

                        If (pInitializing) Then
                            'TR 30/11/2011 -Validate if there was a change during the initializing mode.
                            If (pChangesInWS) Then
                                If (pPausedElements) Then
                                    'Calculate the total WS remaining time excluding the waiting cycles before Running
                                    resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, True)
                                    remainingTime = DirectCast(resultData.SetDatos, Single)
                                    Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                    'Change the intial time to the new remaining time value and the initial time
                                    IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime)
                                    'Set the new maximum to the progress bar.
                                    TaskListProgressBar.Properties.Maximum = _
                                                                IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                                    'Set value of Remaining Time (as HH:mm:ss)
                                    RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                                    OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                                Else
                                    'Calculate the total WS remaining time including the waiting cycles before Running
                                    resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, True)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        remainingTime = DirectCast(resultData.SetDatos, Single)
                                        Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                        If remainingTime > prevRemainingTime Then
                                            'Change the intial time to the new remaining time value and the initial time (180)
                                            IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime)
                                            'Set the new maximum to the progress bar.
                                            TaskListProgressBar.Properties.Maximum = _
                                                IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                                        End If
                                        'Set value of Remaining Time (as HH:mm:ss)
                                        RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                                        OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                                        ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                                    End If
                                End If

                            Else

                                If RemainingTimeTextEdit.Text = String.Empty OrElse ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) = 0 Then
                                    resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, True)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        remainingTime = DirectCast(resultData.SetDatos, Single)
                                        Dim recalRemainingTime As Double = remainingTime + IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds()
                                        'TR 25/05/2012 
                                        If recalRemainingTime > IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds() Then
                                            'Validate before recalculation
                                            'If remainingTime > (recalRemainingTime - IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()) Then
                                            remainingTime = Math.Abs(remainingTime - (recalRemainingTime - _
                                                            IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()))
                                            Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                            'Add the Elapsed time to the remaining time And validate if is greater than previous time
                                            If (remainingTime + IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - cycleMachineTime) > prevRemainingTime Then
                                                'Change the intial time to the new remaining time value
                                                IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime + _
                                                                                    IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - _
                                                                                    cycleMachineTime)
                                            End If
                                            'End If
                                        End If

                                    End If
                                Else
                                    'TR 16/05/2012 Validate if timer is enable  then remove one machine cycle. 
                                    If IAx00MainMDI.ElapsedTimeTimer.Enabled Then
                                        remainingTime = ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) - cycleMachineTime
                                    End If
                                End If

                                RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                                OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                                ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                                TaskListProgressBar.Properties.Maximum = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                                TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum * -1)
                                TaskListProgressBar.Increment(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds())

                            End If

                        ElseIf (pChangesInWS) Then

                            If pPausedElements Then
                                'Calculate the total WS remaining time excluding the waiting cycles before Running
                                resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, False)
                                remainingTime = DirectCast(resultData.SetDatos, Single)
                                'Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                'Add the Elapsed time to the remaining time.
                                IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime).AddSeconds(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds)
                                'Set the new maximum to the progress bar.
                                TaskListProgressBar.Properties.Maximum = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                                TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum * -1)
                                TaskListProgressBar.Increment(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds())
                                'Set value of Remaining Time (as HH:mm:ss)
                                RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                                OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                            Else
                                'Calculate the total WS remaining time excluding the waiting cycles before Running
                                resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, False)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    remainingTime = DirectCast(resultData.SetDatos, Single)
                                    Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                    'TR 21/05/2012 Valida if time is disable, and de WS is not pauset by the user or by Alarm.
                                    If Not IAx00MainMDI.ElapsedTimeTimer.Enabled AndAlso Not IAx00MainMDI.UserPauseWS AndAlso _
                                       mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM) = 0 Then
                                        IAx00MainMDI.ElapsedTimeTimer.Start()
                                    End If
                                    'TR 21/05/2012 -END.
                                    'Add the Elapsed time to the remaining time And validate if is greater than previous time
                                    If (remainingTime + IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - cycleMachineTime) > prevRemainingTime Then
                                        'Change the intial time to the new remaining time value
                                        IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime + _
                                                                            IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - _
                                                                            cycleMachineTime)
                                    End If
                                    'Set the new maximum to the progress bar.
                                    TaskListProgressBar.Properties.Maximum = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                                    TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum * -1)
                                    TaskListProgressBar.Increment(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds())
                                    'Set value of Remaining Time (as HH:mm:ss)
                                    RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                                    OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                                    ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                                End If
                            End If
                        Else
                            'If RemainingTimeTextEdit.Text = String.Empty OrElse ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) = 0 Then
                            resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, False)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                remainingTime = DirectCast(resultData.SetDatos, Single)
                                Dim recalRemainingTime As Double = remainingTime + IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds()
                                'TR 25/05/2012 
                                If recalRemainingTime > IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds() Then
                                    'Validate before recalculation
                                    'If remainingTime > (recalRemainingTime - IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()) Then
                                    remainingTime = Math.Abs(remainingTime - (recalRemainingTime - _
                                                    IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()))
                                    'recalculamos el elapsed time.
                                    Dim prevRemainingTime As Single = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds
                                    'Add the Elapsed time to the remaining time And validate if is greater than previous time
                                    If (remainingTime + IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - cycleMachineTime) _
                                                                                                                > prevRemainingTime Then
                                        'Change the intial time to the new remaining time value
                                        IAx00MainMDI.InitialRemainingTime = ConvertSecondsInHHmmss(remainingTime + _
                                                                            IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds() - _
                                                                            cycleMachineTime)
                                    End If
                                End If
                            End If
                            'Else
                            '    'TR 16/05/2012 Validate if timer is enable 
                            '    If IAx00MainMDI.ElapsedTimeTimer.Enabled Then
                            '        remainingTime = ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) - cycleMachineTime
                            '    End If
                            'End If

                            RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                            OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                            ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                            TaskListProgressBar.Properties.Maximum = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                            TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum * -1)
                            TaskListProgressBar.Increment(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds())

                        End If
                        'If RemainingTimeTextEdit.Text = String.Empty OrElse ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) = 0 Then
                        '    resultData = myWSDelegate.CalculateTimeRemaining(Nothing, WorkSessionIDField, AnalyzerIDField, False, pIseInitializeTime)
                        '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        '        remainingTime = DirectCast(resultData.SetDatos, Single)
                        '    End If
                        'Else
                        '    'TR 16/05/2012 Validate if timer is enable 
                        '    If IAx00MainMDI.ElapsedTimeTimer.Enabled Then
                        '        remainingTime = ConvertHHmmssInSeconds(RemainingTimeTextEdit.Text) - cycleMachineTime
                        '    End If
                        'End If

                        'RemainingTimeTextEdit.Text = ConvertSecondsInHHmmss(remainingTime).ToString("HH:mm:ss")
                        'OverallTimeTextEdit.Text = IAx00MainMDI.InitialRemainingTime.ToString("HH:mm:ss")
                        'ElapsedTimeTextEdit.Text = IAx00MainMDI.LocalElapsedTime.ToString("HH:mm:ss")
                        'TaskListProgressBar.Properties.Maximum = IAx00MainMDI.InitialRemainingTime.TimeOfDay.TotalSeconds()
                        'TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum * -1)
                        'TaskListProgressBar.Increment(IAx00MainMDI.LocalElapsedTime.TimeOfDay.TotalSeconds())
                    End If
                End If
            End If


            'TR -11/11/2011 -Validate if there are remaining time left to stop Timer.
            If remainingTime = 0 Then
                IAx00MainMDI.ElapsedTimeTimer.Stop()
                'Complete the progress bar in case is no completed.
                TaskListProgressBar.Increment(TaskListProgressBar.Properties.Maximum)
            End If
            'End If

            If (resultData.HasError) Then
                'Error getting the WorkSession Start DateTime or calculating the WorkSession remaining time; shown it
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, MsgParent)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTimes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateTimes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

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

            resultData = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, AnalyzerIDField, _
                               GlobalEnumerates.SwParameters.WAITING_CYCLES_BEFORE_RUNNING.ToString, True)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myParametersDS = DirectCast(resultData.SetDatos, ParametersDS)
                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then waitingMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetWaitingCycle ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetWaitingCycle", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return waitingMachineTime
    End Function

    Private Function ConvertSecondsInHHmmss(ByVal pSeconds As Single) As DateTime
        Dim formattedRemTime As New DateTime
        Try
            If pSeconds >= 0 Then
                formattedRemTime = formattedRemTime.AddSeconds(pSeconds)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConvertSecondsInHHmmss ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConvertSecondsInHHmmss", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return formattedRemTime
    End Function

    Public Function ConvertHHmmssInSeconds(ByVal pFormattedDate As Date) As Single
        Dim timeInSeconds As Single = 0
        Try
            Dim remHours As Integer = pFormattedDate.ToString("HH:mm:ss").Substring(0, 2)
            Dim remMinutes As Integer = pFormattedDate.ToString("HH:mm:ss").Substring(3, 2)
            Dim remSeconds As Integer = pFormattedDate.ToString("HH:mm:ss").Substring(6, 2)

            timeInSeconds = (remHours * 3600) + (remMinutes * 60) + remSeconds
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ConvertHHmmssInSeconds ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ConvertHHmmssInSeconds", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return timeInSeconds
    End Function



    ''' <summary>
    ''' Refresh the common area in Monitor Screen
    ''' </summary>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by:  AG 16/06/2011
    ''' </remarks>
    Private Sub RefreshCommonArea(ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            'Dim StartTime As DateTime = Now
            If isClosingFlag Then Exit Sub ' XB 27/05/2014 - #1496 No refresh if screen is closing

            'LEDs AREA
            UpdateLeds()

            'Time information AREA
            Dim changesInWS As Boolean = False
            Dim runningStatus As Boolean = False
            Dim initializeRotor As Boolean = False

            If (Not mdiAnalyzerCopy Is Nothing) Then
                If (mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING) Then
                    runningStatus = True
                    'initializeRotor = (mdiAnalyzerCopy.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_START) AndAlso _
                    '                  (mdiAnalyzerCopy.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.RUNNING_END) AndAlso _
                    '                  (mdiAnalyzerCopy.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.START_INSTRUCTION_START AndAlso _
                    '                   mdiAnalyzerCopy.AnalyzerCurrentAction <> AnalyzerManagerAx00Actions.END_RUN_START)

                    'TR 18/05/2012 commente and take out the if 
                    'changesInWS = WorkSessionChange

                End If
            End If
            'TR validate iif is in initialization time 3:00 min.
            If IAx00MainMDI.LocalTotalSecs2() < 180 Then
                initializeRotor = True
            End If

            'TR Test on running time this is to avoid break points.
            'Debug.Print("Refresh area")
            'Debug.Print(mdiAnalyzerCopy.AnalyzerCurrentAction.ToString())
            'Debug.Print("INICIALIZACION:" & initializeRotor.ToString())
            'Debug.Print("running status:" & runningStatus.ToString())
            'Debug.Print("changes in WS :" & changesInWS.ToString())


            changesInWS = WorkSessionChange

            'DL 10/10/2012. B&T 838
            'UpdateTimes(runningStatus, initializeRotor, changesInWS) 
            'DL 10/10/2012. B&T 838

            'TR 30/11/2011 -Set the Change in WS Value to false 
            'changesInWS = False
            WorkSessionChange = False 'TR 14/12/2011 this is the variable

            'ContainerLevels information AREA
            If Not isClosingFlag Then
                UpdateContainerLevels()
            End If

            'Warming Up AREA
            If (Not mdiAnalyzerCopy Is Nothing AndAlso mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.STANDBY) Then
                'UpdateWarmUpProgressBar()
                If Not IAx00MainMDI.WarmUpFinished Then
                    UpdateWarmUpProgressBar()
                Else
                    If mdiAnalyzerCopy.ExistsALIGHT Then    ' XBC 05/07/2012
                        'TR 25/04/2012
                        TimeWarmUpProgressBar.Position = 100
                        bsEndWarmUp.Visible = False
                        'TR 25/04/2012-END
                    End If

                End If
            End If

            If (Not pRefreshDS Is Nothing) Then Application.DoEvents()

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'Console.Text &= String.Format("RefreshCommonArea: {0}{1}", ElapsedTime.ToStringWithDecimals(0), vbCrLf)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshCommonArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshCommonArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Refresh the content of Samples and Reagents Rotors
    ''' </summary>
    ''' <param name="pEventType" ></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by:  TR 02/05/2011 - 
    ''' Modified by: TR 01/09/2011 - Adapted for Barcode reception (using ROTORPOSITION_CHANGED)
    '''              AG 22/09/2011 - Use the same method but 2 event types (ROTORPOSITION_CHANGED or BARCODE_POSITION_READ) due to 
    '''                              they have different updating screen dataset business
    '''              AG 07/06/2012 - Avoid innecessary loops to execute faster
    '''              SA 21/11/2013 - BT #1359 => Added code to check if the status of the InProcessElement flag for positions in 
    '''                                          Reagents Rotor has to be changed and the corresponding position refreshed
    ''' </remarks>
    Private Sub RefreshRotors(ByVal pEventType As GlobalEnumerates.UI_RefreshEvents, ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            Dim tmpReagentsUpdatePosition As New WSRotorContentByPositionDS
            Dim tmpSamplesUpdatePosition As New WSRotorContentByPositionDS
            Dim RotorContentList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            Dim myInProcessCellsList As New List(Of RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow)

            Dim reagentsRotorFlags As Boolean = False 'AG 07/06/2012
            Dim samplesRotorFlags As Boolean = False 'AG 07/06/2012

            'Get all positions in Reagents Rotor that are currently In Process in the Analyzer
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim myInProcessCellsDS As New RotorPositionsInProcessDS
            Dim myWSRPosInProcess As New WSRotorPositionsInProcessDelegate

            myGlobalDataTO = myWSRPosInProcess.ReadAllReagents(Nothing, AnalyzerIDField)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myInProcessCellsDS = DirectCast(myGlobalDataTO.SetDatos, RotorPositionsInProcessDS)
            Else
                'Error getting the InProcess positions in Reagents Rotor...
                ShowMessage(Me.Name & ".RefreshRotors", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, MsgParent)
            End If

            'Get all positions that are currently marked as In Process in Reagents Rotor
            RotorContentList = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                               Where a.AnalyzerID = AnalyzerIDField _
                             AndAlso a.WorkSessionID = WorkSessionIDField _
                             AndAlso a.RotorType = "REAGENTS" _
                             AndAlso a.InProcessElement = True _
                              Select a).ToList

            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In RotorContentList
                'Check if this position continues InProcess
                myInProcessCellsList = (From a As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow In myInProcessCellsDS.twksWSRotorPositionsInProcess _
                                       Where a.AnalyzerID = row.AnalyzerID _
                                     AndAlso a.RotorType = row.RotorType _
                                     AndAlso a.CellNumber = row.CellNumber _
                                      Select a).ToList

                If (myInProcessCellsList.Count = 0) Then
                    'This position is not In Process; the corresponding flag is updated...
                    row.InProcessElement = False
                End If
            Next

            'For each new In Process position, mark it in the global DS of RotorContentByPositions
            For Each row As RotorPositionsInProcessDS.twksWSRotorPositionsInProcessRow In myInProcessCellsDS.twksWSRotorPositionsInProcess
                RotorContentList = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                   Where a.AnalyzerID = row.AnalyzerID _
                                 AndAlso a.RotorType = row.RotorType _
                                 AndAlso a.CellNumber = row.CellNumber _
                                  Select a).ToList
                If (RotorContentList.Count > 0) Then
                    RotorContentList.First.InProcessElement = True
                End If
            Next

            tmpReagentsUpdatePosition.twksWSRotorContentByPosition.Clear() 'AG 07/06/2012
            tmpSamplesUpdatePosition.twksWSRotorContentByPosition.Clear() 'AG 07/06/2012
            For Each RefreshRow As UIRefreshDS.RotorPositionsChangedRow In pRefreshDS.RotorPositionsChanged.Rows
                If (Not RefreshRow.IsAnalyzerIDNull AndAlso Not RefreshRow.IsWorkSessionIDNull AndAlso Not RefreshRow.IsRotorTypeNull) Then
                    RotorContentList = (From a In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                                       Where a.AnalyzerID = RefreshRow.AnalyzerID _
                                     AndAlso a.WorkSessionID = RefreshRow.WorkSessionID _
                                     AndAlso a.RotorType = RefreshRow.RotorType _
                                     AndAlso a.CellNumber = RefreshRow.CellNumber _
                                      Select a).ToList

                    If (RotorContentList.Count > 0) Then
                        RotorContentList.First().BeginEdit()
                        If Not RefreshRow.IsStatusNull Then
                            RotorContentList.First().Status = RefreshRow.Status
                            'Else - AG 22/09/2011 - Set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetStatusNull()
                        End If

                        If Not RefreshRow.IsElementStatusNull Then
                            RotorContentList.First().ElementStatus = RefreshRow.ElementStatus
                            'Else - AG 22/09/2011 - Set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetElementStatusNull()
                        End If

                        If Not RefreshRow.IsRealVolumeNull Then
                            RotorContentList.First().RealVolume = RefreshRow.RealVolume
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetRealVolumeNull()
                        End If

                        If Not RefreshRow.IsRemainingTestsNumberNull Then
                            RotorContentList.First().RemainingTestsNumber = RefreshRow.RemainingTestsNumber
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetRemainingTestsNumberNull()
                        End If

                        If Not RefreshRow.IsBarCodeInfoNull Then
                            RotorContentList.First().BarCodeInfo = RefreshRow.BarCodeInfo
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetBarCodeInfoNull()
                        End If

                        If Not RefreshRow.IsBarcodeStatusNull Then
                            RotorContentList.First().BarcodeStatus = RefreshRow.BarcodeStatus
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetBarCodeInfoNull()
                        End If

                        'New added Rows.
                        If Not RefreshRow.IsScannedPositionNull Then
                            RotorContentList.First().ScannedPosition = RefreshRow.ScannedPosition
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetScannedPositionNull()
                        End If

                        If Not RefreshRow.IsElementIDNull Then
                            RotorContentList.First().ElementID = RefreshRow.ElementID
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetElementIDNull()
                        End If

                        If Not RefreshRow.IsMultiTubeNumberNull Then
                            RotorContentList.First().MultiTubeNumber = RefreshRow.MultiTubeNumber
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetMultiTubeNumberNull()
                        End If

                        If Not RefreshRow.IsTubeTypeNull Then
                            RotorContentList.First().TubeType = RefreshRow.TubeType
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetTubeTypeNull()
                        End If

                        If Not RefreshRow.IsTubeContentNull Then
                            RotorContentList.First().TubeContent = RefreshRow.TubeContent
                            'Else - AG 22/09/2011 - set to null this field only in barcode mode
                        ElseIf pEventType = UI_RefreshEvents.BARCODE_POSITION_READ Then
                            RotorContentList.First().SetTubeContentNull()
                        End If

                        RotorContentList.First().EndEdit()
                        RotorContentList.First().AcceptChanges()

                        'Update the rotor position information on the screen 
                        If (RotorContentList.First().RotorType = "REAGENTS") Then
                            tmpReagentsUpdatePosition.twksWSRotorContentByPosition.ImportRow(RotorContentList.First())
                        ElseIf RotorContentList.First().RotorType = "SAMPLES" Then
                            tmpSamplesUpdatePosition.twksWSRotorContentByPosition.ImportRow(RotorContentList.First())
                        End If
                    End If
                End If
            Next

            'Get all positions that are currently marked as In Process in Reagents Rotor
            RotorContentList = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myRotorContentByPositionDSForm.twksWSRotorContentByPosition _
                               Where a.AnalyzerID = AnalyzerIDField _
                             AndAlso a.WorkSessionID = WorkSessionIDField _
                             AndAlso a.RotorType = "REAGENTS" _
                             AndAlso a.InProcessElement = True _
                              Select a).ToList

            Dim tmpInProcessList As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In RotorContentList
                'Verify if it is already included in the DS of positions to Refresh
                tmpInProcessList = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In tmpReagentsUpdatePosition.twksWSRotorContentByPosition _
                                   Where a.AnalyzerID = row.AnalyzerID _
                                  AndAlso a.RotorType = row.RotorType _
                                  AndAlso a.CellNumber = row.CellNumber _
                                   Select a).ToList

                If (tmpInProcessList.Count = 0) Then
                    tmpReagentsUpdatePosition.twksWSRotorContentByPosition.ImportRow(row)
                End If
            Next

            'AG 07/06/2012
            If (tmpReagentsUpdatePosition.twksWSRotorContentByPosition.Rows.Count > 0) Then
                UpdateRotorTreeViewArea(tmpReagentsUpdatePosition, "REAGENTS")
            End If

            If tmpSamplesUpdatePosition.twksWSRotorContentByPosition.Rows.Count > 0 Then
                UpdateRotorTreeViewArea(tmpSamplesUpdatePosition, "SAMPLES")
            End If
            'AG 07/06/2012

            'TR 25/09/2013 #memory
            RotorContentList = Nothing
            myInProcessCellsList = Nothing
            tmpInProcessList = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshRotors ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshRotors", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    Public Sub RefreshAlarmsGlobes(ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            'Forces all globe alarms to show up
            'Uncomment just for testing, and comment the after lines.
            ''BEGIN Simulation
            'Dim Alarms As New List(Of GlobalEnumerates.Alarms)

            'For Each alarm As GlobalEnumerates.Alarms In [Enum].GetValues(GetType(GlobalEnumerates.Alarms))
            '    Alarms.Add(alarm) 'All the alarms for simulation
            'Next

            ' ''Remove exclusive alarms (Warnings)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.CLOT_DETECTION_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.REACT_TEMP_WARN)

            ' ''New alarms
            ''Alarms.Remove(GlobalEnumerates.Alarms.MAIN_COVER_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R1_COLLISION_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R2_COLLISION_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.REACT_COVER_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.S_COLLISION_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.S_COVER_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R1_TEMP_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R2_TEMP_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.CLOT_DETECTION_WARN)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WS_TEMP_WARN)

            ''Or remove the others exclusive alarms (Errors)
            'Alarms.Remove(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.CLOT_DETECTION_ERR)
            'Alarms.Remove(GlobalEnumerates.Alarms.REACT_TEMP_ERR)

            ''New alarms
            ''Alarms.Remove(GlobalEnumerates.Alarms.MAIN_COVER_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R1_COLLISION_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.FRIDGE_COVER_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R2_COLLISION_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.REACT_COVER_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.S_COLLISION_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.S_COVER_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.CLOT_DETECTION_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR)
            ''Alarms.Remove(GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR)

            'UpdateGlobesInMainTab(Alarms)
            ''END Simulation

            'Update Main Area
            'Alarms information AREA
            'BEGIN Real code

            ''PENDING TO DECIDE
            ''SGM 01/10/2012 - In case of Closing NOT TO TREAT GLOBES
            'If MyBase.isClosingFlag Then
            '    CreateLogActivity("Globes while Is Closing", Me.Name & ".RefreshAlarmsGlobes ", EventLogEntryType.FailureAudit, False)
            '    Exit Sub
            'End If

            If isClosingFlag Then Return 'AG 10/02/2014 - #1496 No refresh is screen is closing

            If Not mdiAnalyzerCopy Is Nothing Then

                'AG 21/02/2012- do not use the conditional compilation (we always develop in debug mode .. use the REAL_DEVELOPMENT_MODE (global Setting)
                'when we want analyzer reportsats and simulate alarms we must change this settings to a value <> 0
                '#If DEBUG Then
                '#Else
                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then

                Else
                    'SGM 11/04/2012
                    If mdiAnalyzerCopy.Alarms.Contains(GlobalEnumerates.Alarms.ISE_OFF_ERR) Or mdiAnalyzerCopy.Alarms.Contains(GlobalEnumerates.Alarms.ISE_FAILED_ERR) Then
                        If mdiAnalyzerCopy.Alarms.Contains(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR) Then
                            mdiAnalyzerCopy.Alarms.Remove(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                        End If
                    End If

                End If

                UpdateGlobesInMainTab(mdiAnalyzerCopy.Alarms) 'Get the current alarms in analyzer
            End If
            'END Real code

            'Update Alarms grid Area
            UpdateAlarmsTab(pRefreshDS)

            Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshAlarmsGlobes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshAlarmsGlobes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pRefreshEventType"></param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Modified by AG 05/03/2014 - #1524, inform UpdateWSState when new results are calculated
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            RefreshDoneField = False 'RH 28/03/2012

            If isClosingFlag Then Return 'AG 03/04/2012

            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 04/07/2012 - time estimation
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS) OrElse _
                pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED) Then
                'Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation

                'AG 05/03/2014 - #1524
                'UpdateWSState(pRefreshDS)
                Dim newResultsFlag As Boolean = pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED)

                'AG 14/03/2014 - #1524
                Dim newRerunAdded As Boolean = False
                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) AndAlso mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_RERUN_ADDED) = 1 Then newRerunAdded = True
                'AG 14/03/2014 - #1524

                UpdateWSState(pRefreshDS, newResultsFlag, newRerunAdded)
                'AG 05/03/2014 - #1524

                RefreshDoneField = True 'RH 28/03/2012

                'Debug.Print("iMonitor.UpdateWSState: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
            End If

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.READINGS_RECEIVED) Then
                'Nothing
            End If

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                'Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation

                RefreshAlarmsGlobes(pRefreshDS)
                RefreshDoneField = True 'RH 28/03/2012

                'Debug.Print("iMonitor.RefreshAlarmsGlobes: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
            End If

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED) Then
                'Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation

                'Call the Method incharge to update Reactions Rotor
                RefreshReactionsRotor(pRefreshDS)
                RefreshDoneField = True 'RH 28/03/2012

                'Debug.Print("iMonitor.RefreshReactionsRotor: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation
            End If

            'If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) Then
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) OrElse _
                pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then

                Dim myLocalEventType As GlobalEnumerates.UI_RefreshEvents = GlobalEnumerates.UI_RefreshEvents.NONE
                If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) Then
                    'This event has to update only CellStatus, elementStatus and optionally TestLeft, RealVolume
                    myLocalEventType = GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED
                ElseIf pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then
                    myLocalEventType = GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ
                End If

                'Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation

                'Call the Method incharge to update Rotor (Sample/Reagent)
                RefreshRotors(myLocalEventType, pRefreshDS)
                RefreshDoneField = True 'RH 28/03/2012

                'Debug.Print("iMonitor.RefreshRotors: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

            End If

            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                'Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation
                RefreshCommonArea(pRefreshDS)
                RefreshDoneField = True 'RH 28/03/2012

                'Debug.Print("iMonitor.RefreshCommonArea: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

                'SGM 09/03/2012
                'ISE Monitor Data changed
                Dim sensorValue As Integer = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_MONITOR_DATA_CHANGED)
                If sensorValue = 1 Then
                    'StartTime = Now 'AG 05/06/2012 - time estimation

                    ScreenWorkingProcess = False
                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_MONITOR_DATA_CHANGED) = 0 'Once updated UI clear sensor
                    Me.BsIseMonitor.RefreshFieldsData(mdiAnalyzerCopy.ISE_Manager.MonitorDataTO)
                    BsISELongTermDeactivated.Visible = (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEModuleInstalled AndAlso MyClass.mdiAnalyzerCopy.ISE_Manager.IsLongTermDeactivation)
                    Me.ISETab.Refresh()

                    'Debug.Print("iMonitor.BsIseMonitor.RefreshFieldsData: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 05/06/2012 - time estimation

                End If
                'end SGM 09/03/2012

                sensorValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                If sensorValue = 1 Then
                    ScreenWorkingProcess = False
                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                    IAx00MainMDI.ISEProcedureFinished = True
                End If

            End If

            myLogAcciones.CreateLogActivity("Refresh monitor screen (complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "iMonitor.RefreshScreen", EventLogEntryType.Information, False) 'AG 04/07/2012

            ''AG 12/04/2012 - If WS aborted then show message in the app status bar
            'If WorkSessionStatusField = "ABORTED" Then
            '    bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
            'End If
            ''AG 12/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen ", EventLogEntryType.Error, _
                                                                    GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Set the worksession tab (used for the automatic WS creation with LIS)
    ''' </summary>
    ''' <remarks>AG 17/07/2013</remarks>
    Private Sub SetWorkSessionTab()
        If MonitorTabs Is Nothing Then Exit Sub ' XB 13/03/2014 - #1523 No refresh if screen is closing
        MonitorTabs.SelectedTabPage = Me.StatesTab
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Release elements not handle by the GC.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 04/08/2011
    ''' Modified by XB 17/01/2014 - Improve Dispose
    ''' AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
    ''' </remarks>
    Private Sub ReleaseElement()
        Try
            'isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called

            NoImage = Nothing
            SatImage = Nothing
            NoSatImage = Nothing
            FinishedImage = Nothing
            LockedImage = Nothing
            FinishedWithErrorsImage = Nothing
            PrintAvailableImage = Nothing
            HISSentImage = Nothing
            PausedImage = Nothing
            PlayImage = Nothing
            GraphImage = Nothing

            STDTestIcon = Nothing
            CALCTestIcon = Nothing
            ISETestIcon = Nothing
            OFFSTestIcon = Nothing

            BlankIcon = Nothing
            CalibratorIcon = Nothing
            ControlIcon = Nothing

            LegendSelectedImage.Image = Nothing
            LegendNotInUseImage.Image = Nothing
            LegendDepletedImage.Image = Nothing
            LegendPendingImage.Image = Nothing
            LegendInProgressImage.Image = Nothing
            LegendFinishedImage.Image = Nothing
            LegendPendingImage0.Image = Nothing
            LegendInProgressImage0.Image = Nothing
            LegendFinishedImage0.ImageLocation = Nothing
            LegendBarCodeErrorImage.Image = Nothing

            ERRORImage = Nothing
            WARNINGImage = Nothing
            SOLVEDimage = Nothing
            SOLVEDErrorImage = Nothing
            SOLVEDWarningImage = Nothing

            ' XB 17/01/2014
            For Each myControl As Control In Me.SamplesTab.Controls

                If (TypeOf myControl Is BSRImage) Then
                    Dim CurrentControl As BSRImage = CType(myControl, BSRImage)

                    CurrentControl.Image = Nothing
                    CurrentControl.BackgroundImage = Nothing

                    CurrentControl.Dispose()
                    CurrentControl = Nothing
                End If

            Next
            For Each myControl As Control In Me.ReagentsTab.Controls

                If (TypeOf myControl Is BSRImage) Then
                    Dim CurrentControl As BSRImage = CType(myControl, BSRImage)

                    CurrentControl.Image = Nothing
                    CurrentControl.BackgroundImage = Nothing

                    CurrentControl.Dispose()
                    CurrentControl = Nothing
                End If

            Next
            For Each myControl As Control In Me.ReactionsTab.Controls

                If (TypeOf myControl Is BSRImage) Then
                    Dim CurrentControl As BSRImage = CType(myControl, BSRImage)

                    CurrentControl.Image = Nothing
                    CurrentControl.BackgroundImage = Nothing

                    CurrentControl.Dispose()
                    CurrentControl = Nothing
                End If

            Next
            ' XB 17/01/2014

            'TR 29/09/2011 -Set controls to nothing.to reduce memory use
            If Not myPosControlList Is Nothing Then
                For Each myControl As BSRImage In myPosControlList
                    myControl.Image = Nothing
                    myControl.BackgroundImage = Nothing

                    myControl.Dispose() ' XB 17/01/2014
                    myControl = Nothing
                Next
                myPosControlList.Clear()
            End If
            If Not AllTubeSizeList Is Nothing Then AllTubeSizeList.Clear()

            'AG 03/03/2014 - #1524 - Clear list before set to Nothing
            AllTubeSizeList = Nothing
            myPosControlList.Clear()
            SampleIconList.Images.Clear()
            myExecutions.Clear()
            'AG 03/03/2014 - #1524

            'mdiAnalyzerCopy = Nothing 'not this variable
            'MainMDI = Nothing 'not this variable

            MainTab.Appearance.PageClient.Image = Nothing
            SamplesTab.Appearance.PageClient.Image = Nothing
            ReagentsTab.Appearance.PageClient.Image = Nothing
            ReactionsTab.Appearance.PageClient.Image = Nothing

            'TR 25/09/2013 -#New elements for memory release
            bsReactionsOpenGraph.Image = Nothing
            mySelectedElementInfo = Nothing

            myPosControlList = Nothing
            SampleIconList = Nothing
            myExecutions = Nothing

            PausePictureBox = Nothing
            PrintPictureBox = Nothing
            HISSentPictureBox = Nothing
            GraphPictureBox = Nothing

            HeaderFont = Nothing
            RowFont = Nothing
            CurrentRowColor = Nothing
            CurrentHeaderColor = Nothing

            ResultsReadyImage.Image = Nothing
            ResultsWarningImage.Image = Nothing
            ResultsAbsImage.Image = Nothing
            FinalReportImage.Image = Nothing
            ExportLISImage.Image = Nothing
            LockedTestImage.Image = Nothing
            PausedTestImage.Image = Nothing

            'AG 31/01/2014 - BT #1486 (v211 patch D + v300)
            autoWSCreationTimer.Enabled = False
            RemoveHandler autoWSCreationTimer.Elapsed, AddressOf autoWSCreationTimer_Timer
            autoWSCreationTimer = Nothing
            'AG 31/01/2014 - BT #1486

            '--- Detach variable defined using WithEvents ---
            MonitorTabs = Nothing
            MainTab = Nothing
            StatesTab = Nothing
            SamplesTab = Nothing
            ReagentsTab = Nothing
            ReactionsTab = Nothing
            ISETab = Nothing
            WasteLabel = Nothing
            WashingLabel = Nothing
            BsGroupBox9 = Nothing
            TotalTestsChart = Nothing
            bsTestStatusLabel = Nothing
            bsWSExecutionsDataGridView = Nothing
            ElapsedTimeLabel = Nothing
            TaskListProgressBar = Nothing
            PanelControl2 = Nothing
            BsGroupBox3 = Nothing
            bsTimeLabel = Nothing
            bsFridgeStatusLed = Nothing
            BsGroupBox10 = Nothing
            StateCfgLabel = Nothing
            bsConnectedLed = Nothing
            bsISEStatusLed = Nothing
            bsSamplesLegendGroupBox = Nothing
            bsSensorsLabel = Nothing
            bsReactionsTemperatureLed = Nothing
            bsFridgeTemperatureLed = Nothing
            bsSampleNumberTextBox = Nothing
            bsSampleContentTextBox = Nothing
            bsSampleDiskNameTextBox = Nothing
            bsSampleCellTextBox = Nothing
            bsSamplesNumberLabel = Nothing
            bsSamplesContentLabel = Nothing
            bsSamplesDiskNameLabel = Nothing
            bsSamplesCellLabel = Nothing
            bsSamplesBarcodeTextBox = Nothing
            bsDiluteStatusTextBox = Nothing
            bsSampleTypeTextBox = Nothing
            bsSampleIDTextBox = Nothing
            bsSamplesBarcodeLabel = Nothing
            bsDiluteStatusLabel = Nothing
            bsSampleTypeLabel = Nothing
            bsSampleIDLabel = Nothing
            bsSamplesMoveLastPositionButton = Nothing
            bsSamplesIncreaseButton = Nothing
            bsSamplesDecreaseButton = Nothing
            bsSamplesMoveFirstPositionButton = Nothing
            bsTubeSizeComboBox = Nothing
            bsTubeSizeLabel = Nothing
            bsReagentsPositionInfoGroupBox = Nothing
            bsReagentsCellTextBox = Nothing
            bsReagentsCellLabel = Nothing
            bsTeststLeftTextBox = Nothing
            bsCurrentVolTextBox = Nothing
            bsTestsLeftLabel = Nothing
            bsCurrentVolLabel = Nothing
            bsBottleSizeComboBox = Nothing
            bsBottleSizeLabel = Nothing
            bsExpirationDateTextBox = Nothing
            bsReagentsPositionInfoLabel = Nothing
            bsReagentsMoveLastPositionButton = Nothing
            bsReagentsIncreaseButton = Nothing
            bsReagentsDecreaseButton = Nothing
            bsReagentsMoveFirstPositionButton = Nothing
            bsReagentsBarCodeTextBox = Nothing
            bsTestNameTextBox = Nothing
            bsReagentsNumberTextBox = Nothing
            bsReagentNameTextBox = Nothing
            bsReagentsContentTextBox = Nothing
            bsReagentsDiskNameTextBox = Nothing
            bsExpirationDateLabel = Nothing
            bsReagentsBarCodeLabel = Nothing
            bsTestNameLabel = Nothing
            bsReagentsNumberLabel = Nothing
            bsReagentNameLabel = Nothing
            bsReagentsContentLabel = Nothing
            bsReagentsDiskNameLabel = Nothing
            PanelControl3 = Nothing
            LegendReagentsGroupBox = Nothing
            LegReagAdditionalSol = Nothing
            LegReagNoInUseLabel = Nothing
            LegReagentSelLabel = Nothing
            LegReagDepleteLabel = Nothing
            AdditionalSolPictureBox = Nothing
            NoInUsePictureBox = Nothing
            SelectedPictureBox = Nothing
            bsDepletedPictureBox = Nothing
            bsReagentsLegendLabel = Nothing
            PanelControl4 = Nothing
            bsReagentsStatusTextBox = Nothing
            bsReagentsStatusLabel = Nothing
            PanelControl6 = Nothing
            PanelControl7 = Nothing
            BsGroupBox11 = Nothing
            bsSampleStatusTextBox = Nothing
            bsSamplesStatusLabel = Nothing
            bsSamplesPositionInfoLabel = Nothing
            LegendDepletedImage = Nothing
            bsSamplesLegendLabel = Nothing
            LegendFinishedImage = Nothing
            LegendInProgressImage = Nothing
            LegendNotInUseImage = Nothing
            LegendBarCodeErrorImage = Nothing
            BarcodeErrorLabel = Nothing
            InProgressLabel = Nothing
            FinishedLabel = Nothing
            DepletedLabel = Nothing
            NotInUseLabel = Nothing
            SelectedLabel = Nothing
            PendingLabel = Nothing
            LegendPendingImage = Nothing
            RemainingTimeLabel = Nothing
            AccessRRTimeLabel = Nothing
            AccessRMTimeLabel = Nothing
            LegReagentLabel = Nothing
            LegReagLowVolLabel = Nothing
            LowVolPictureBox = Nothing
            ReagentPictureBox = Nothing
            LegendSelectedImage = Nothing
            TestRefresh = Nothing
            LegendSamplesGroupBox = Nothing
            OverallTimeTitleLabel = Nothing
            OverallTimeTextEdit = Nothing
            ElapsedTimeTextEdit = Nothing
            RemainingTimeTextEdit = Nothing
            AccessRRTimeTextEdit = Nothing
            AccessRMTimeTextEdit = Nothing
            PanelControl11 = Nothing
            PanelControl14 = Nothing
            PanelControl15 = Nothing
            LegendReactionsGroupBox = Nothing
            bsFinishLabel = Nothing
            bsR1SampleLabel = Nothing
            bsR1SampleR2 = Nothing
            bsReactionsLegendLabel = Nothing
            bsDilutionLabel = Nothing
            bsR1SamplePictureBox = Nothing
            bsFinishPictureBox = Nothing
            bsDilutionPictureBox = Nothing
            bsR1PictureBox = Nothing
            bsR1SampleR2PictureBox = Nothing
            bsWashingPictureBox = Nothing
            bsNotInUsePictureBox = Nothing
            bsR1Label = Nothing
            bsWashingLabel = Nothing
            bsNotInUseLabel = Nothing
            BsGroupBox13 = Nothing
            bsReacStatusTextBox = Nothing
            bsReacStatusLabel = Nothing
            bsReactionsMoveFirstPositionButton = Nothing
            bsReactionsPositionInfoLabel = Nothing
            bsWellNumberLabel = Nothing
            bsSampleClassLabel = Nothing
            bsReactionsDecreaseButton = Nothing
            bsCalibNumLabel = Nothing
            bsReactionsIncreaseButton = Nothing
            bsReactionsMoveLastPositionButton = Nothing
            bsWellNrTextBox = Nothing
            bsSampleClassTextBox = Nothing
            bsCalibNrTextBox = Nothing
            bsReacSampleIDLabel = Nothing
            bsDilutionTextBox = Nothing
            bsReacSampleTypeLabel = Nothing
            bsReacSampleTypeTextBox = Nothing
            bsReacDilutionLabel = Nothing
            bsPatientIDTextBox = Nothing
            bsRerunTextBox = Nothing
            bsReplicateTextBox = Nothing
            bsRerunLabel = Nothing
            bsReplicateLabel = Nothing
            bsOpticalLabel = Nothing
            bsContaminatedLabel = Nothing
            bsOpticalPictureBox = Nothing
            bsContaminatedPictureBox = Nothing
            BsGroupBox14 = Nothing
            bsTimeAvailableLabel = Nothing
            bsReacTestTextBox = Nothing
            bsReacTestNameLabel = Nothing
            bsReactionsOpenGraph = Nothing
            BsExecutionIDTextBox = Nothing
            bsOrderTestIDTextBox = Nothing
            BsTimer1 = Nothing
            PanelControl10 = Nothing
            LegendBarCodeErrorRGImage = Nothing
            BarcodeErrorRGLabel = Nothing
            LegendUnknownImage = Nothing
            UnknownLabel = Nothing
            bsWamUpGroupBox = Nothing
            bsEndWarmUp = Nothing
            TimeWarmUpProgressBar = Nothing
            bsTimeWUpLabel = Nothing
            PanelControl9 = Nothing
            LegendWSGroupBox = Nothing
            ResultsStatusLabel = Nothing
            PausedTestImage = Nothing
            PausedTestLabel = Nothing
            LockedTestImage = Nothing
            ResultsAbsImage = Nothing
            LockedTestLabel = Nothing
            ResultsAbsLabel = Nothing
            FinalReportLabel = Nothing
            ResultsWarningLabel = Nothing
            ResultsReadyLabel = Nothing
            TestStatusLabel = Nothing
            FinalReportImage = Nothing
            ResultsWarningImage = Nothing
            ResultsReadyImage = Nothing
            bsWorksessionLegendLabel = Nothing
            ExportLISLabel = Nothing
            ExportLISImage = Nothing
            PendingLabel0 = Nothing
            InProgressLabel0 = Nothing
            FinishedLabel0 = Nothing
            LegendPendingImage0 = Nothing
            LegendFinishedImage0 = Nothing
            LegendInProgressImage0 = Nothing
            SampleStatusLabel = Nothing
            AlarmsTab = Nothing
            BsGroupBox4 = Nothing
            bsTitleLabel = Nothing
            BsGroupBox1 = Nothing
            BsLabel1 = Nothing
            BsDataGridView1 = Nothing
            AlarmsXtraGrid = Nothing
            AlarmsXtraGridView = Nothing
            BsIseMonitor = Nothing
            BsISELongTermDeactivated = Nothing
            bsErrorProvider1 = Nothing
            CoverOffPicture = Nothing
            chart2 = Nothing
            CoverOnPicture = Nothing
            CoverReagentsPicture = Nothing
            CoverSamplesPicture = Nothing
            CoverReactionsPicture = Nothing
            ToolTipController1 = Nothing
            '------------------------------------------------

            'GC.Collect()

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElement", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Update the leds area using the current alarms and the current sensor values
    ''' </summary>
    ''' <remarks>
    ''' Created by:   AG 16/06/2011
    ''' Modified by: SGM 21/03/2012 - Changes in ISE Status LED: it will be Green only when setting IsISESwitchedON is True
    '''                             - Show the lock icon when the ISE Module is in status of long term deactivation; otherwise, hide it
    '''               SA 22/03/2012 - Changes in ISE Status LED: validation of IsISESwitchedON=True only once ISE_Manager has
    '''                               been created; otherwise, the LED is set to Gray color
    '''                             - Added Try/Catch to the function
    '''               SA 23/03/2012 - Changes in ISE Status LED: if IsISESwitchedON=False, the LED is set to RED color instead of GRAY
    '''               AG 29/03/2012
    ''' </remarks>
    Private Sub UpdateLeds()
        Try
            'Random for testing
            'ReactionsLed.StateIndex = rnd.Next Mod 4
            'FridgeTempLed.StateIndex = rnd.Next Mod 4
            'ISELed.StateIndex = rnd.Next Mod 4
            'A400Led.StateIndex = rnd.Next Mod 4
            'FridgeStateLed.StateIndex = rnd.Next Mod 4

            If (Not mdiAnalyzerCopy Is Nothing) Then
                Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = mdiAnalyzerCopy.AnalyzerStatus

                'Get the current alarms in analyzer
                Dim myAlarms As List(Of GlobalEnumerates.Alarms)
                myAlarms = mdiAnalyzerCopy.Alarms

                '********************
                '* PC connected LED *
                '********************
                Dim myConnectedValue As Single = 0
                myConnectedValue = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.CONNECTED)
                If (CBool(myConnectedValue)) Then
                    'The Analyzer is connected; the LED Color is set to GREEN
                    bsConnectedLed.StateIndex = bsLed.LedColors.GREEN

                    '...Now check the status of the other leds

                    '*********************
                    '* Fridge Status LED *
                    '*********************
                    'AG 29/03/2012 - Do not use only alarms (they appear only when Wup process is finished) use sensor + alarms
                    'If (myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR)) Then 'damaged
                    '    bsFridgeStatusLed.StateIndex = bsLed.LedColors.RED

                    'ElseIf (myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN)) Then 'off
                    '    bsFridgeStatusLed.StateIndex = bsLed.LedColors.GRAY

                    'ElseIf (myAx00Status = AnalyzerManagerStatus.SLEEPING) Then 'Unknown
                    '    bsFridgeStatusLed.StateIndex = bsLed.LedColors.GRAY

                    'Else 'on
                    '    bsFridgeStatusLed.StateIndex = bsLed.LedColors.GREEN
                    'End If
                    Dim fridgeOK As Boolean = False
                    If (myAx00Status = AnalyzerManagerStatus.SLEEPING) Then 'Sleeping, fridge status unknown (keep gray)
                        bsFridgeStatusLed.StateIndex = bsLed.LedColors.GRAY

                    ElseIf (CInt(mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FRIDGE_STATUS)) <= 0) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_STATUS_WARN) Then 'Fridge status unknown or OFF
                        bsFridgeStatusLed.StateIndex = bsLed.LedColors.GRAY

                    ElseIf (myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_STATUS_ERR)) Then 'damaged
                        bsFridgeStatusLed.StateIndex = bsLed.LedColors.RED

                    Else 'ON & OK
                        bsFridgeStatusLed.StateIndex = bsLed.LedColors.GREEN
                        fridgeOK = True
                    End If
                    'AG 29/03/2012

                    '********************
                    '*  ISE Status LED  *
                    '********************

                    'SGM 11/04/2012
                    'CASES:
                    'ISE Not Installed into BA400 or ISE Installed + BA400 not connected with Sw:	    GRAY	(IsISEModuleInstalled = False)
                    'ISE Installed + ISE Switch OFF: RED	    (IsISEModuleInstalled = True + IsISESwitchON =False)

                    'ISE Installed + ISE Switch ON + During ISE Initialization process:	GRAY	
                    '  (IsISEModuleInstalled = True + IsISESwitchON =True + IsISEInitiating = True)
                    'ISE Installed + ISE Switch ON + ISE Not Initialized  (Initialization process has finished with error or timeout): RED	
                    '  (IsISEModuleInstalled = True + IsISESwitchON = True + IsInitializedOk = False)
                    'ISE Installed + ISE Switch ON + ISE Initialized OK:    	        GREEN	
                    '  (IsISEModuleInstalled = True + IsISESwitchON = True + IsInitializedOk = True)
                    'ISE Installed + ISE Switch ON + ISE Initialized OK +ISE Not Ready (Reagents, Electrodes, etc): GREEN	
                    ' (IsISEModuleInstalled = True + IsISESwitchON = True + IsInitializedOk = True + IsISEModuleReady)
                    Dim adjustValue As String = ""
                    Dim iseInstalledFlag As Boolean = False

                    adjustValue = mdiAnalyzerCopy.ReadAdjustValue(Ax00Adjustsments.ISEINS) 'Verify if the Analyzer has ISE Module 
                    If (adjustValue <> "" AndAlso IsNumeric(adjustValue)) Then iseInstalledFlag = CType(adjustValue, Boolean)

                    If (Not iseInstalledFlag) Then
                        'If there is not an ISE Module in the Analyzer, color is GRAY
                        bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                    Else

                        If (myAx00Status = AnalyzerManagerStatus.SLEEPING) Then 'Sleeping, ise status unknown (keep gray)
                            bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                        ElseIf CInt(mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS)) < 0 Then 'ISE status unknown (=-1)
                            bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                        ElseIf CInt(mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS)) = 0 Then 'ISE Switch OFF
                            bsISEStatusLed.StateIndex = bsLed.LedColors.RED

                        ElseIf (myAlarms.Contains(GlobalEnumerates.Alarms.ISE_FAILED_ERR)) Then 'error after trying ise connection
                            bsISEStatusLed.StateIndex = bsLed.LedColors.RED

                        ElseIf MyClass.mdiAnalyzerCopy.ISE_Manager.IsLongTermDeactivation Then
                            bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                        ElseIf MyClass.mdiAnalyzerCopy.ISE_Manager.IsISESwitchON Then 'ISE Switch ON
                            If (Not MyClass.mdiAnalyzerCopy.ISE_Manager Is Nothing) Then
                                If MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEInitiating Then
                                    bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY
                                ElseIf (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEInitiatedOK = False) Then
                                    bsISEStatusLed.StateIndex = bsLed.LedColors.RED
                                ElseIf (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEInitiatedOK = True) Then
                                    bsISEStatusLed.StateIndex = bsLed.LedColors.GREEN
                                End If
                            Else
                                'If an instance of ISE Manager has not been created, then the Led color is GRAY
                                bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                            End If

                        End If
                        'AG 29/03/2012
                    End If


                    'SGM 11/04/2012
                    ''Verify if the Analyzer has ISE Module 
                    'Dim adjustValue As String = ""
                    'Dim iseInstalledFlag As Boolean = False

                    'adjustValue = mdiAnalyzerCopy.ReadAdjustValue(Ax00Adjustsments.ISEINS)
                    'If (adjustValue <> "" AndAlso IsNumeric(adjustValue)) Then iseInstalledFlag = CType(adjustValue, Boolean)

                    'If (Not iseInstalledFlag) Then
                    '    'If there is not an ISE Module in the Analyzer, color is GRAY
                    '    bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY
                    '    'BsISELongTermDeactivated.Visible = False
                    'Else

                    '    If (myAx00Status = AnalyzerManagerStatus.SLEEPING) Then 'Sleeping, ise status unknown (keep gray)
                    '        bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                    '    ElseIf CInt(mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS)) < 0 Then 'ISE status unknown
                    '        bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY

                    '    ElseIf CInt(mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_STATUS)) = 0 Then 'ISE status OFF
                    '        bsISEStatusLed.StateIndex = bsLed.LedColors.RED 'GRAY

                    '    ElseIf (myAlarms.Contains(GlobalEnumerates.Alarms.ISE_STATUS_ERR)) Then 'damaged
                    '        bsISEStatusLed.StateIndex = bsLed.LedColors.RED

                    '    Else 'ON
                    '        If (Not MyClass.mdiAnalyzerCopy.ISE_Manager Is Nothing) Then
                    '            If MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEInitializationDone Then
                    '                'For first connection
                    '                If (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISESwitchON) Then
                    '                    'ISE Module is Switched ON 
                    '                    bsISEStatusLed.StateIndex = bsLed.LedColors.GREEN
                    '                Else
                    '                    'ISE Module is Switched OFF 
                    '                    bsISEStatusLed.StateIndex = bsLed.LedColors.RED 'GRAY
                    '                End If
                    '            ElseIf Not MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEInitiating Then
                    '                'For reconnection
                    '                If (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISESwitchON) Then
                    '                    'ISE Module is Switched ON 
                    '                    bsISEStatusLed.StateIndex = bsLed.LedColors.GREEN
                    '                Else
                    '                    'ISE Module is Switched OFF 
                    '                    bsISEStatusLed.StateIndex = bsLed.LedColors.RED 'GRAY
                    '                End If
                    '            End If
                    '            'BsISELongTermDeactivated.Visible = MyClass.mdiAnalyzerCopy.ISE_Manager.IsLongTermDeactivation
                    '        Else
                    '            'If an instance of ISE Manager has not been created, then the Led color is GRAY
                    '            bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY
                    '            'BsISELongTermDeactivated.Visible = False
                    '        End If
                    '        'BsISELongTermDeactivated.Visible = MyClass.mdiAnalyzerCopy.ISE_Manager.IsLongTermDeactivation
                    '    End If
                    '    'AG 29/03/2012
                    'End If
                    'SGM 11/04/2012

                    'RH 04/05/2012 Reactions Rotor Temperature alarms simulation
                    'myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_WARN)
                    'myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR)
                    'myAlarms.Add(GlobalEnumerates.Alarms.REACT_TEMP_ERR)

                    'DL 21/06/2012. Begin
                    Dim reactioncoverFlag As Boolean
                    Dim reagentscoverFlag As Boolean

                    adjustValue = mdiAnalyzerCopy.ReadAdjustValue(Ax00Adjustsments.PHCOV)   'Reactions cover activate (1), deactivate (0)
                    If (Not String.Equals(adjustValue, String.Empty) AndAlso IsNumeric(adjustValue)) Then reactioncoverFlag = CType(adjustValue, Boolean)

                    adjustValue = mdiAnalyzerCopy.ReadAdjustValue(Ax00Adjustsments.RCOV)    'Reagents cover activate (1), deactivate (0)
                    If (Not String.Equals(adjustValue, String.Empty) AndAlso IsNumeric(adjustValue)) Then reagentscoverFlag = CType(adjustValue, Boolean)

                    'Reactions Rotor Temperature LED
                    'DL 31/07/2012. Begin
                    If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then
                        'If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS1_ERR) OrElse _
                        '  myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS2_ERR) Then
                        'DL 31/07/2012. End
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.RED

                    ElseIf myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_ERR) Then
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.RED

                    ElseIf Not reactioncoverFlag Then                                       'Reaction cover deactivate
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GRAY

                    ElseIf myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_WARN) Then  'Exists Temperature warning
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.ORANGE

                    ElseIf myAx00Status = AnalyzerManagerStatus.SLEEPING Then               'Unknown
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GRAY

                    Else 'ok
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GREEN
                    End If

                    'If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_ERR) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then
                    '    bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.RED
                    'ElseIf myAlarms.Contains(GlobalEnumerates.Alarms.REACT_TEMP_WARN) Then 'Exists Temperature warning
                    '    bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.ORANGE
                    'ElseIf myAx00Status = AnalyzerManagerStatus.SLEEPING Then 'Unknown
                    '    bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GRAY
                    'Else 'ok
                    '    bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GREEN
                    'End If
                    'DL 21/06/2012. End

                    'RH 04/05/2012 Fridge Temperature alarms simulation
                    'myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN)
                    'myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR)
                    'myAlarms.Add(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR)

                    'Fridge Temperature LED
                    'DL 31/07/2012.Begin
                    If myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) Then 'Exists Temperature error (only system damaged)
                        'If myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS1_ERR) OrElse _
                        '  myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_SYS2_ERR) Then 'Exists Temperature error (only system damaged)
                        'DL 31/07/2012. End
                        bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.RED

                    ElseIf Not reagentscoverFlag Then                                       'DL 21/06/2012 Reagent cover deactivate
                        bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GRAY         'DL 21/06/2012

                    ElseIf myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_WARN) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_TEMP_ERR) Then 'Exists Temperature warning or error but not damaged
                        bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.ORANGE

                    ElseIf myAx00Status = AnalyzerManagerStatus.SLEEPING Then 'Unknown
                        bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.GRAY

                    Else 'ok
                        If fridgeOK Then
                            bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.GREEN
                        Else
                            bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.GRAY
                        End If

                    End If

                    'RH 04/05/2012 Other Temperature alarms simulation for baloons validation
                    'myAlarms.Add(GlobalEnumerates.Alarms.R1_TEMP_WARN)
                    'myAlarms.Add(GlobalEnumerates.Alarms.R2_TEMP_WARN)
                    'myAlarms.Add(GlobalEnumerates.Alarms.WS_TEMP_WARN)
                    'myAlarms.Add(GlobalEnumerates.Alarms.R1_TEMP_SYSTEM_ERR)
                    'myAlarms.Add(GlobalEnumerates.Alarms.R2_TEMP_SYSTEM_ERR)
                    'myAlarms.Add(GlobalEnumerates.Alarms.WS_TEMP_SYSTEM_ERR)

                Else
                    'The Analyzer is not connected; all LEDs are set to GRAY
                    bsConnectedLed.StateIndex = bsLed.LedColors.GRAY
                    bsFridgeStatusLed.StateIndex = bsLed.LedColors.GRAY
                    bsISEStatusLed.StateIndex = bsLed.LedColors.GRAY
                    bsReactionsTemperatureLed.StateIndex = bsLed.LedColors.GRAY
                    bsFridgeTemperatureLed.StateIndex = bsLed.LedColors.GRAY
                End If

                'TR 25/09/2013 #memory 
                myAlarms = Nothing

            End If


        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateLeds", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Update the warmup common area in monitor screen
    ''' </summary>
    ''' <remarks>DL 12/09/2011</remarks>
    Private Sub UpdateWarmUpProgressBar()
        Dim myDate As String = ""

        Dim resultData As GlobalDataTO = Nothing
        Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate

        resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                   AnalyzerIDField, _
                                                                   GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue

            End If

            If myDate.Trim = "" Then
                bsEndWarmUp.Visible = False

                Dim sensorValue As Single = mdiAnalyzerCopy.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)
                If sensorValue = 0 Then
                    mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                    mdiAnalyzerCopy.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 - Ise alarms ready to be shown
                End If

            Else
                IAx00MainMDI.BsTimerWUp_Tick(Nothing, Nothing)

                Dim completeWupProcessFlag As Boolean = False 'AG 12/09/2011
                Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                                           AnalyzerIDField, _
                                                                           GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString())

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myAnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                    If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                        If String.Equals(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, "1") Then
                            bsEndWarmUp.Visible = True

                            mdiAnalyzerCopy.ISE_Manager.IsAnalyzerWarmUp = False 'SGM 13/04/2012 for enabling IE utilities

                            Dim sensorValue As Single = mdiAnalyzerCopy.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)
                            If sensorValue = 0 Then
                                mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                                mdiAnalyzerCopy.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 - Ise alarms ready to be shown
                            End If

                        Else
                            bsEndWarmUp.Visible = False
                            bsWamUpGroupBox.Visible = True 'DL 20/01/2012
                        End If

                    End If
                End If

            End If
        End If
    End Sub

    ''' <summary>
    '''  Gets all label texts for all Tabs and Monitor left column
    ''' </summary>
    ''' <remarks>Created by: RH 14/10/2011</remarks>
    Private Sub GetMonitorLabels()
        Try
            MainTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Main", LanguageID)
            StatesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Worksession", LanguageID)
            SamplesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", LanguageID)
            ReagentsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", LanguageID)
            ReactionsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reactions", LanguageID)
            ISETab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ISE_Module", LanguageID)
            AlarmsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Alarms", LanguageID)

            'DL 14/10/2011
            StateCfgLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_STATECFG", LanguageID)
            TestRefresh.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Refresh", LanguageID)
            bsConnectedLed.Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PCConnected", LanguageID)
            bsFridgeStatusLed.Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FRIDGE", LanguageID)
            bsISEStatusLed.Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "ISEMODULE_ALERT", LanguageID)
            bsSensorsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Temperatures", LanguageID)
            bsReactionsTemperatureLed.Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "REACTIONSROTOR_ALERT", LanguageID)
            bsFridgeTemperatureLed.Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FRIDGE", LanguageID)

            'DL 10/10/2012. B&T 838. BEGIN
            'bsTimeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_TIMEWS", LanguageID)
            'OverallTimeTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OverallTime", LanguageID)
            ''RH 02/05/2012
            'ElapsedTimeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_EllapsedTime", LanguageID)
            'RemainingTimeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RemainingTime", LanguageID)
            'bsTimeAvailableLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Time_Available_Rotors", LanguageID)
            'AccessRMTimeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleRotor", LanguageID)
            'AccessRRTimeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReagentRotor", LanguageID)
            'DL 10/10/2012. B&T 838. END

            bsTimeWUpLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Time_WarmUp", LanguageID)
            'DL 14/10/2011

            bsEndWarmUp.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_TERMINATE", LanguageID) 'TR 28/03/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetMonitorLabels ", EventLogEntryType.Error, _
                                                                          GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetMonitorLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' get status description on current language
    ''' </summary>
    ''' <param name="pItemID"></param>
    ''' <param name="pBarcodeStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStatusDescOnCurrentLanguage(ByVal pItemID As String, ByVal pBarcodeStatus As String) As String
        Dim myResult As String = ""
        Try
            Dim mySubTableID As GlobalEnumerates.PreloadedMasterDataEnum
            Dim myItemID As String = pItemID

            'Get the value for the subtable id 
            Select Case myRotorTypeForm
                Case "SAMPLES"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.SAMPLE_POS_STATUS
                    Exit Select
                Case "REAGENTS"
                    mySubTableID = GlobalEnumerates.PreloadedMasterDataEnum.REAGENT_POS_STATUS
                    Exit Select
            End Select

            'AG 06/10/2011 - add barcode status information (by now if error barcode show NO IN USE)
            If String.Equals(pBarcodeStatus, "ERROR") Then myItemID = "NO_INUSE"
            'AG 06/10/2011

            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            Dim myGlobalDataTO As New GlobalDataTO
            'Get the information related to the itemid 
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetSubTableItem(Nothing, mySubTableID, myItemID)
            If Not myGlobalDataTO.HasError Then
                Dim myPreloMasterDS As New PreloadedMasterDataDS
                myPreloMasterDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                If myPreloMasterDS.tfmwPreloadedMasterData.Count > 0 Then
                    myResult = myPreloMasterDS.tfmwPreloadedMasterData(0).FixedItemDesc
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetStatusLanguage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetStatusLanguage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Finish the auto create WS with LIS
    ''' </summary>
    ''' <remarks>
    ''' AG 31/01/2014 - BT #1486 (v211 patch D + v300)</remarks>
    ''' </remarks>
    Private Sub ExecuteAutoCreateWSLastStep()
        Try
            If isClosingFlag Then Exit Sub ' XB 13/03/2014 - #1496 No refresh if screen is closing

            Dim myEnableButtonsAlreadyLaunch As Boolean = False     ' XB 25/11/2013
            If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
                Me.SetWorkSessionTab()
                Dim myLogAcciones As New ApplicationLogManager()
                Dim autoProcessUserAnswer As DialogResult = DialogResult.Yes
                autoProcessUserAnswer = IAx00MainMDI.CheckForExceptionsInAutoCreateWSWithLISProcess(6)
                Dim resultFlagOK As Boolean = True
                If autoProcessUserAnswer = DialogResult.Yes Then
                    ''Positive case. No execptions
                    resultFlagOK = True
                ElseIf autoProcessUserAnswer = DialogResult.OK Then 'User answers stops the automatic process
                    'Automatic process aborted
                    resultFlagOK = False
                Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                    resultFlagOK = True
                    IAx00MainMDI.autoWSNotFinishedButGoRunning = True 'AG 01/04/2014 - #1565 inform that the process has not finished but user wants go to running
                End If

                ShownScreen() 'AG 22/04/2014 - #1598 before enable buttons be sure the shown screen atribute has been activated
                If resultFlagOK Then
                    myLogAcciones.CreateLogActivity("AutoCreate WS with LIS: Process near to finish successfully. Go to Running", "IMonitor.AutoCreateWSLastStep", EventLogEntryType.Information, False)
                    IAx00MainMDI.FinishAutomaticWSWithLIS()
                Else
                    IAx00MainMDI.EnableButtonAndMenus(True, True) 'Enable buttons before update attribute!! (required for PLAY/PAUSE button)
                    IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                    IAx00MainMDI.SetHQProcessByUserFlag(False)
                    myEnableButtonsAlreadyLaunch = True     ' XB 25/11/2013
                End If
                OpenByAutomaticProcessAttribute = False
            End If

            ' XB 25/11/2013 - Inform to MDI that this screen is shown - Task #1303
            'ShownScreen()'AG 22/04/2014 - #1598 flag is set to TRUE before last IF
            If Not myEnableButtonsAlreadyLaunch Then IAx00MainMDI.EnableButtonAndMenus(True, True)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))", Me.Name & ".ExecuteAutoCreateWSLastStep ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExecuteAutoCreateWSLastStep ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"

    Private Sub Monitor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***



        'Get the current Language from the current Application Session
        Dim currentLanguageGlobal As New GlobalBase
        LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

        'RH 18/10/2011 Initialize myMultiLangResourcesDelegate
        myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate()

        MainMDI = CType(Me.MdiParent, IAx00MainMDI)

        mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 16/06/2011 - Use the same AnalyzerManager as the MDI

        GetMonitorLabels() 'RH 14/10/2011

        InitializeMainTab()
        InitializeStatesTab()
        InitializeRotors()
        InitializeSamplesTab()
        InitializeReagentTab()
        InitializeReactionsTab()
        InitializeISETab()
        InitializeAlarmsTab()

        If Not Me.MdiParent Is Nothing Then
            'TR 03/08/2011 -Remove handler before adding.
            RemoveHandler MainMDI.Move, AddressOf bsAlert_Move
            AddHandler MainMDI.Move, AddressOf bsAlert_Move
        End If

        ResetBorder()
        Application.DoEvents()

        SetEnableMainTab(MainTabEnabled, True)

        UpdateAlertsPositions()
        RefreshCommonArea(Nothing)

        'RH 19/10/2011 Do not Refresh alarms twice. The first time the form is loaded, the MainMDI refreshes the alarms
        'If Not IsFirstLoading Then
        '    RefreshAlarmsGlobes(Nothing)
        'End If

        IsFirstLoading = False

        If MainMDI.WarmUpFinished Then
            TimeWarmUpProgressBar.Position = 100
            bsEndWarmUp.Visible = False 'TR 20/04/2012 -Set button not visible.
        End If

        Application.DoEvents()

        'AG 31/01/2014 - BT #1486 (v211 patch D + v300)
        autoWSCreationTimer.Enabled = False
        AddHandler autoWSCreationTimer.Elapsed, AddressOf autoWSCreationTimer_Timer
        'AG 31/01/2014 - BT #1486

        'DL 04/10/2011
        LegendWSGroupBox.Visible = True
        LegendSamplesGroupBox.Visible = True
        LegendReagentsGroupBox.Visible = True
        LegendReactionsGroupBox.Visible = True
        'DL 04/10/2011

        'AG 21/03/2012 - If WS aborted then show message in the app status bar
        If WorkSessionStatusField = "ABORTED" Then
            bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
        End If
        'AG 21/03/2012

        'for refreshing ISE deactivated Lock Icon SGM 07/06/2012
        BsISELongTermDeactivated.Visible = (MyClass.mdiAnalyzerCopy.ISE_Manager.IsISEModuleInstalled AndAlso MyClass.mdiAnalyzerCopy.ISE_Manager.IsLongTermDeactivation)

        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        myLogAcciones.CreateLogActivity("IMonitor LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                        "IMonitor.Monitor_Load", EventLogEntryType.Information, False)
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

    End Sub

    Private Sub MonitorTabs_SelectedPageChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MonitorTabs.SelectedPageChanged
        Application.DoEvents()
        UpdateRotorType()
        ChangingTab = False
        ShowActiveAlerts()

        If MonitorTabs.SelectedTabPage.Name = AlarmsTab.Name Then
            AlarmsXtraGrid.Focus()
        ElseIf MonitorTabs.SelectedTabPage.Name = ISETab.Name Then
            If mdiAnalyzerCopy.ISE_Manager IsNot Nothing Then
                If Not mdiAnalyzerCopy.Connected Then mdiAnalyzerCopy.ISE_Manager.RefreshMonitorDataTO() 'refresh in case of disconnected SGM 03/04/2012
                Me.BsIseMonitor.RefreshFieldsData(mdiAnalyzerCopy.ISE_Manager.MonitorDataTO)
            End If
        End If
    End Sub

    Private Sub MonitorTabs_SelectedPageChanging(ByVal sender As System.Object, ByVal e As DevExpress.XtraTab.TabPageChangingEventArgs) Handles MonitorTabs.SelectedPageChanging
        If e.PrevPage Is Nothing Then Return

        ChangingTab = True
        Application.DoEvents()
        HideActiveAlerts()
    End Sub

    Private Sub Monitor_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        If Not Me.MdiParent Is Nothing Then
            RemoveHandler MainMDI.Move, AddressOf bsAlert_Move
        End If

        'RH Make sure all Alert globes are closed when then form is closed
        For Each ChildForm As Form In MainMDI.OwnedForms
            If TypeOf ChildForm Is bsAlert Then
                ChildForm.Hide()
            End If
        Next

        CreateLogActivity("IMonitor CLOSED (Complete)", "IMonitor.Monitor_FormClosed", EventLogEntryType.Information, False) 'AG 28/05/2014 - New trace

        'Application.DoEvents() 'AG 21/02/2014 - #1516 Comment because sometimes cause internal exceptions
    End Sub

    Private Sub bsEndWarmUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEndWarmUp.Click
        IAx00MainMDI.FinishWarmUp(False)
    End Sub

    Private Sub Generic_MouseDown(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If TypeOf sender Is BSRImage Then
            Dim myPictureBox As BSRImage = CType(sender, BSRImage)

            'Validate that the tag property is not empty to get the information.
            If Not myPictureBox.Tag Is Nothing Then
                'get the selected ring and cell number.
                Dim myRingNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(0), Integer)
                Dim myCellNumber As Integer = CType(myPictureBox.Tag.ToString().Split(",")(1), Integer)
                mySelectedElementInfo = GetLocalPositionInfo(myRingNumber, myCellNumber, False)

                ShowPositionInfoArea(myRotorTypeForm, myRingNumber, myCellNumber)
                'MarkSelectedPosition(myRingNumber, myCellNumber, True)
            End If
        End If
    End Sub

    Private Sub bsReactionsOpenGraph_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsReactionsOpenGraph.Click
        'AG 07/12/2011 - commeted. Call the final graph abs(t) screen
        If bsReacTestTextBox.Text <> "" Then
            Using myForm As New IResultsAbsCurve

                myForm.AnalyzerID = ActiveAnalyzer
                myForm.WorkSessionID = ActiveWorkSession
                myForm.MultiItemNumber = 1          'CType(bsCalibNrTextBox.Text.ToString)
                myForm.ReRun = bsRerunTextBox.Text
                myForm.Replicate = CType(bsReplicateTextBox.Text.ToString, Integer)
                '
                myForm.TestName = bsReacTestTextBox.Text
                myForm.SampleID = bsPatientIDTextBox.Text
                myForm.SampleClass = bsSampleClassTextBox.Text
                '
                myForm.OrderTestID = bsOrderTestIDTextBox.Text
                myForm.SourceForm = GlobalEnumerates.ScreenCallsGraphical.WS_STATES
                myForm.ListExecutions = myExecutions

                IAx00MainMDI.AddNoMDIChildForm = myForm 'Inform the MDI the curve calib results is shown
                myForm.ShowDialog()
                IAx00MainMDI.RemoveNoMDIChildForm = myForm 'Inform the MDI the curve calib results is closed
            End Using

        End If

    End Sub

    'DL 10/10/2012. B&T 838
    '''' <summary>
    '''' Changes in Time Work Session have registered
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created by XBC 24/07/2012</remarks>
    'Private Sub OverallTimeTextEdit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles OverallTimeTextEdit.TextChanged
    '    Try
    '        If OverallTimeTextEdit.Text.Length > 0 Then
    '            If IsNumeric(remainingTime) Then
    '                If (Not MyClass.mdiAnalyzerCopy.ISE_Manager Is Nothing) Then
    '                    If Not (MyClass.mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS" Or _
    '                            MyClass.mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS") Then
    '                        mdiAnalyzerCopy.ISE_Manager.WorkSessionOverallTime = remainingTime
    '                    End If
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OverallTimeTextEdit_TextChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".OverallTimeTextEdit_TextChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub
    'DL 10/10/2012. B&T 838


    ''' <summary>
    ''' Customizing the Appearances of Rows. The GridView.RowStyle event is handled to customize the appearance of rows which have the value "False" in the AlarmStatus column. 
    ''' These rows are painted using gradient filling. The starting and ending gradient colors are specified via the AppearanceObject.BackColor and AppearanceObject.BackColor2 
    ''' properties of the event's RowStyleEventArgs.Appearance parameter. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by DL 24/07/2012</remarks>
    Private Sub AlarmsXtraGridView_RowStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs) Handles AlarmsXtraGridView.RowStyle
        Dim myAlarmsView As GridView = sender

        If (e.RowHandle >= 0) Then

            Dim myAlarmStatus As Boolean = CType(myAlarmsView.GetRowCellValue(e.RowHandle, myAlarmsView.Columns("AlarmStatus")), Boolean)

            If Not myAlarmStatus Then
                e.Appearance.BackColor = Color.White
                'e.Appearance.BackColor2 = Color.Gainsboro
                e.Appearance.ForeColor = Color.Gray
                'e.Appearance.Font = New Font(e.Appearance.Font, FontStyle.Italic)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Automate WS creation with LIs
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 09/07/2013</remarks>
    Private Sub IMonitor_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'AG 31/01/2014 - BT #1486 (v211 patch D + v300)
        If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then
            UIThread(Sub() IAx00MainMDI.SetActionButtonsEnableProperty(False))
            autoWSCreationTimer.Interval = 750 '1000 '2000
            autoWSCreationTimer.Enabled = True
        Else
            ShownScreen()
        End If

    End Sub


    '''' <summary>
    '''' This only works if the control is the size of the image, and has no border.
    '''' Ie the image is exactly what is in the control, with no resampling.
    '''' </summary>
    '''' <param name="pe"></param>
    '''' <returns></returns>
    'Private Function ExactClippingRegion(ByVal pe As PictureEdit) As Region
    '    Dim result As Region = Nothing
    '    Dim image As Image = pe.Image
    '    If image Is Nothing Then
    '        Return result
    '    End If
    '    Dim bitmap As Bitmap = DirectCast(image, Bitmap)
    '    Dim rects As New List(Of Rectangle)()
    '    For y As Integer = 0 To image.Height - 1
    '        For x As Integer = 0 To image.Width - 1
    '            If bitmap.GetPixel(x, y).A <> 0 Then
    '                ' Alpha = 0 => transparent
    '                Dim rect As New Rectangle(x, y, 1, 1)
    '                ' 1 pixel rectangle
    '                rects.Add(rect)
    '            End If
    '        Next
    '    Next
    '    If rects.Count > 0 Then
    '        result = New Region(rects(0))
    '        For Each rect As Rectangle In rects
    '            result.Union(rect)
    '        Next
    '    End If
    '    Return result
    'End Function


    'Private Sub BsButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton1.Click

    '    If testbackgroundMaintab Then
    '        MainTab.Appearance.PageClient.Image = Image.FromFile("C:\Users\David Luna\Documents\Ax00 v1\PresentationCOM\Images\Embedded\BackgroundMainMonitor-Start.png") 'GlobalBase.ImagesPath & "Embedded\BackgroundMainMonitor-Start.png")
    '        PanelControl12.Visible = False
    '        CoverReagentsPicture.Visible = False
    '        CoverReactionsPicture.Visible = False
    '        CoverSamplesPicture.Visible = False
    '        CoverOffPicture.Visible = False
    '        CoverOnPicture.Visible = False

    '        testbackgroundMaintab = False

    '    Else
    '        MainTab.Appearance.PageClient.Image = Image.FromFile("C:\Users\David Luna\Documents\Ax00 v1\PresentationCOM\Images\Embedded\BackgroundMainMonitor.png") 'GlobalBase.ImagesPath & "Embedded\BackgroundMainMonitor.png")
    '        PanelControl12.Visible = True
    '        CoverReagentsPicture.Visible = True
    '        CoverReactionsPicture.Visible = True
    '        CoverSamplesPicture.Visible = True
    '        CoverOnPicture.Visible = True

    '        testbackgroundMaintab = True

    '    End If
    'End Sub


    Public Sub New()
        ' XB 25/11/2013 - Inform to MDI that this screen is building - Task #1303
        LoadingScreen()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'DL 10/10/2012. B&T 838.BEGIN
        ''DL 11/05/2012
        'BsGroupBox14.Visible = False
        'bsWamUpGroupBox.Location = New System.Drawing.Point(3, 391)
        BsGroupBox3.Visible = False
        BsGroupBox14.Visible = False
        bsWamUpGroupBox.Location = BsGroupBox3.Location 'New System.Drawing.Point(3, 391)
        'DL 11/05/2012
        'DL 10/10/2012. B&T 838

    End Sub

    ''' <summary>
    ''' When the timer of the autoWS creation process triggers execute business
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 31/01/2014 - BT #1486 (v211 patch D + v300)</remarks>
    Private Sub autoWSCreationTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)

        'Disable timer
        autoWSCreationTimer.Enabled = False
        UIThread(Sub() ExecuteAutoCreateWSLastStep())

    End Sub


#End Region

End Class