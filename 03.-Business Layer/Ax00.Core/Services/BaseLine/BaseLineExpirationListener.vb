Imports System.ComponentModel
Imports System.Threading
Imports System.Timers
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces


Namespace Biosystems.Ax00.Core.Services.BaseLine
    Public Class BaseLineExpirationListener

#Region "Definitions"

        Private Shared _analyzer As IAnalyzerManager
        Private _analyzerAlarmsManager As IAnalyzerAlarms
        Private baseLineExpirationobj As BaseLineEntityExpiration
#End Region

        Public Sub New(analyzer As IAnalyzerManager)

            BaseLineExpirationListener._analyzer = analyzer
            _analyzerAlarmsManager = New AnalyzerAlarms(AnalyzerManager.GetCurrentAnalyzerManager())
            baseLineExpirationobj = New BaseLineEntityExpiration(_analyzer)

        End Sub

#Region "Events"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub startToListening()
            If _analyzer.Connected Then
                checkIsExpired()
            End If

            Dim T As New Threading.Thread(AddressOf Listening)
            T.IsBackground = True
            T.Priority = ThreadPriority.Lowest
            T.Start()
        End Sub

        ''' <summary>
        ''' Function that check every ten minuts if BaseLine is expired or not. and create or delete and Alarm to inform the user.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Listening()
            While True
                '10 minuts
                'Dim a = Task.Delay(10 * 60 * 1000)
                Dim a = Task.Delay(2 * 60 * 1000)
                a.Wait()

                checkIsExpired()

            End While
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub checkIsExpired()
            If baseLineExpirationobj.IsBlExpired Then
                _analyzerAlarmsManager.ActionAlarm(0, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            Else
                _analyzerAlarmsManager.ActionAlarm(1, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            End If
        End Sub

#End Region

#Region "Alarm Method"

        ' ''' <summary>
        ' ''' Method to create or delete an alarm dependiong if the baseline is expired or not.
        ' ''' </summary>
        ' ''' <param name="alarm"></param>
        ' ''' <remarks></remarks>
        'Public Sub ActionAlarm(ByVal typeAction As TypeActionAlarm, ByRef alarm As AlarmEnumerates.Alarms)
        '    Dim myGlobal As New GlobalDataTO
        '    Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
        '    Dim myAlarmStatusList As New List(Of Boolean)
        '    Dim status As Boolean
        '    Try


        '        myAlarmList.Add(alarm)

        '        Select Case typeAction
        '            Case TypeActionAlarm.Creation
        '                status = True
        '                myAlarmStatusList.Add(status)
        '                If Not _analyzerAlarmsManager.ExistsActiveAlarm(alarm.ToString()) Then myGlobal = _analyzerAlarmsManager.Manage(myAlarmList, myAlarmStatusList)
        '            Case TypeActionAlarm.Delete
        '                status = False
        '                myAlarmStatusList.Add(status)
        '                If _analyzerAlarmsManager.ExistsActiveAlarm(alarm.ToString()) Then myGlobal = _analyzerAlarmsManager.Manage(myAlarmList, myAlarmStatusList)
        '        End Select

        '        _analyzer.CreateAndThrowEventUiRefresh()
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

#End Region

    End Class
End Namespace