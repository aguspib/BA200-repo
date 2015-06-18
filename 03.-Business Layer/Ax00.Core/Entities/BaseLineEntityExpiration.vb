Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities

Public Class BaseLineEntityExpiration
    Implements IBaseLineExpiration

    Public Property AnalyzerID As String Implements IBaseLineExpiration.AnalyzerID

    Public ReadOnly Property IsBlExpired As Boolean Implements IBaseLineExpiration.IsBlExpired
        Get

        End Get
    End Property

    
    'Public Sub CreationAlarm(ByRef alarm As AlarmEnumerates.Alarms) Implements IBaseLine.CreationAlarm
    '    Dim myGlobal As New GlobalDataTO
    '    Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
    '    Dim myAlarmStatusList As New List(Of Boolean)

    '    Try
    '        myAlarmList.Add(alarm)
    '        myAlarmStatusList.Add(True)

    '        If myAlarmList.Count > 0 Then

    '            Dim currentAlarms = New AnalyzerAlarms(Analyzer)
    '            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub








 
End Class