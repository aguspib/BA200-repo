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
        Private baseLineExpirationobj As IBaseLineExpiration
#End Region
        Public Property TimeDelay As Integer = 1000 * 60 * 2
        Public Sub New(analyzer As IAnalyzerManager, baseLineExpirationobj As IBaseLineExpiration, analyzerAlarmsManager As IAnalyzerAlarms)

            BaseLineExpirationListener._analyzer = analyzer
            _analyzerAlarmsManager = analyzerAlarmsManager
            Me.baseLineExpirationobj = baseLineExpirationobj

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
                Dim a = Task.Delay(TimeDelay)
                a.Wait()

                checkIsExpired()

            End While
        End Sub

        ''' <summary>
        ''' Function that call to public property from BaseLineEntityExpiration IsBlExpired to check the date 
        ''' and depends if return true or false do some action  
        ''' </summary>
        ''' <remarks> ActionAlarm
        '''          0 to create the alarm.
        '''          1 to delete the alarm. 
        ''' </remarks>
        Private Sub checkIsExpired()
            If baseLineExpirationobj.IsBlExpired Then
                _analyzerAlarmsManager.ActionAlarm(0, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            Else
                _analyzerAlarmsManager.ActionAlarm(1, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            End If
        End Sub

#End Region

    End Class
End Namespace