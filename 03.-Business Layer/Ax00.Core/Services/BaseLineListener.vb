Imports System.ComponentModel
Imports System.Timers
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces

Public Class BaseLineListener

#Region "Definitions"

    Private Shared _analyzer As IAnalyzerManager

    Private WithEvents _timerlistener As Timers.Timer
    Private WithEvents _myBkwBL_CMD As BackgroundWorker = Nothing

#End Region

    Public Sub New(analyzer As IAnalyzerManager)

        BaseLineListener._analyzer = analyzer

        _myBkwBL_CMD = New BackgroundWorker()
        _myBkwBL_CMD.WorkerSupportsCancellation = True
        _myBkwBL_CMD.WorkerReportsProgress = True
        _myBkwBL_CMD.RunWorkerAsync(1)

        _timerlistener = New Timer()
        AddHandler _timerlistener.Elapsed, AddressOf OnTimedEvent_listener


    End Sub

#Region "Background Worker and Timer events"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Bkw_ISE_CMD_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _myBkwBL_CMD.DoWork
        Try
            _timerlistener.Interval = 600000
            _timerlistener.Enabled = True

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Bkw_ISE_CMD_WorkCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _myBkwBL_CMD.RunWorkerCompleted
        'empty event. 
    End Sub

    ''' <summary>
    ''' Periodical event that check against the DB if baseline parameter is over time. 
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub OnTimedEvent_listener(source As Object, e As ElapsedEventArgs)
        Dim baseLineExpirationobj As New BaseLineEntityExpiration(_analyzer)

        If baseLineExpirationobj.IsBlExpired Then
            CreationAlarm(AlarmEnumerates.Alarms.BL_EXPIRED)
            _timerlistener.Enabled = False
            _myBkwBL_CMD.ReportProgress(100)
        Else
            DeleteAlarm(AlarmEnumerates.Alarms.BL_EXPIRED)
        End If
    End Sub

    ''' <summary>
    ''' Function to Update the progress of the ba.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub _myBkwBL_CMD_ProgressChanged(ByVal sender As System.Object, ByVal e As ProgressChangedEventArgs) Handles _myBkwBL_CMD.ProgressChanged
        If e.ProgressPercentage = 100 Then
            If _myBkwBL_CMD.IsBusy Then
                _myBkwBL_CMD.CancelAsync()
            End If
        End If
    End Sub

#End Region

#Region "Alarms Functions"
    ''' <summary>
    ''' Function to create a  new alarm when the baseline is expired.
    ''' </summary>
    ''' <param name="alarm"></param>
    ''' <remarks></remarks>
    Public Sub CreationAlarm(ByRef alarm As AlarmEnumerates.Alarms)
        Dim myGlobal As New GlobalDataTO
        Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
        Dim myAlarmStatusList As New List(Of Boolean)

        Try
            myAlarmList.Add(alarm)
            myAlarmStatusList.Add(True)

            If myAlarmList.Count > 0 Then

                Dim currentAlarms = New AnalyzerAlarms(AnalyzerManager.GetCurrentAnalyzerManager())
                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Function to delete the alarm of baseline is expired.
    ''' </summary>
    ''' <param name="alarm"></param>
    ''' <remarks></remarks>
    Public Sub DeleteAlarm(ByRef alarm As AlarmEnumerates.Alarms)
        Dim myGlobal As New GlobalDataTO
        Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
        Dim myAlarmStatusList As New List(Of Boolean)

        Try
            myAlarmList.Add(alarm)
            myAlarmStatusList.Add(False)

            If myAlarmList.Count > 0 Then

                Dim currentAlarms = New AnalyzerAlarms(AnalyzerManager.GetCurrentAnalyzerManager())
                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

End Class
