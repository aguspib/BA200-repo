Imports System.ComponentModel
Imports System.Timers
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Services.BaseLine
    Public Class BaseLineExpirationListener

#Region "Definitions"

        Private Shared _analyzer As IAnalyzerManager

#End Region

        Public Sub New(analyzer As IAnalyzerManager)

            BaseLineExpirationListener._analyzer = analyzer

            Dim T As New Threading.Thread(AddressOf Listening)
            T.IsBackground = True
            T.Start()

        End Sub


#Region "Events"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Listening()
            Dim baseLineExpirationobj As New BaseLineEntityExpiration(_analyzer)

            While True
                Dim a = Task.Delay(10 * 60 * 1000)
                a.Wait()

                If baseLineExpirationobj.IsBlExpired Then
                    CreationAlarm(AlarmEnumerates.Alarms.BL_EXPIRED)
                Else
                    DeleteAlarm(AlarmEnumerates.Alarms.BL_EXPIRED)
                End If

            End While

        End Sub

#End Region

#Region "Alarms Functions"
        ''' <summary>
        ''' Function to create a  new alarm when the baseline is expired.
        ''' </summary>
        ''' <param name="alarm"></param>
        ''' <remarks></remarks>
        Private Sub CreationAlarm(ByRef alarm As AlarmEnumerates.Alarms)
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
        Private Sub DeleteAlarm(ByRef alarm As AlarmEnumerates.Alarms)
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
End Namespace