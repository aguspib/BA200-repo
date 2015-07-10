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

        'Default value = 10 minutes
        Public Property TimeDelay As Integer = 1000 * 60 * 2

        Public Sub New(analyzer As IAnalyzerManager, baseLineExpirationobj As IBaseLineExpiration, analyzerAlarmsManager As IAnalyzerAlarms)

            BaseLineExpirationListener._analyzer = analyzer
            _analyzerAlarmsManager = analyzerAlarmsManager
            Me.baseLineExpirationobj = baseLineExpirationobj

            'We create a thread when we Initialize the application.
            Dim T As New Threading.Thread(AddressOf Listening)
            T.IsBackground = True
            T.Priority = ThreadPriority.Lowest
            T.Start()

        End Sub

#Region "Events"

        ''' <summary>
        ''' Function only used when the application is connected with one analyzer.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub StartToListening()
            If _analyzer.Connected Then
                checkIsExpired()
            End If
        End Sub

        ''' <summary>
        ''' Function that check every ten minuts if BaseLine is expired or not. and create or delete and Alarm to inform the user.
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Listening()

            While True
                Dim a = Task.Delay(TimeDelay)
                a.Wait()
                'We only check if is expired if we have connection with one analyzer. 
                If _analyzer.Connected Then
                    CheckIsExpired()
                End If
            End While
        End Sub

        ''' <summary>
        ''' Function that call to public property from BaseLineEntityExpiration IsBlExpired to check the date 
        ''' and depends if return true or false do some action  
        ''' </summary>
        ''' <remarks> ActionAlarm
        '''          True to create the alarm.
        '''          False to delete the alarm. 
        ''' </remarks>
        Private Sub CheckIsExpired()
            If baseLineExpirationobj.IsBlExpired Then
                _analyzerAlarmsManager.ActionAlarm(True, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            Else
                _analyzerAlarmsManager.ActionAlarm(False, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
            End If
        End Sub

#End Region

    End Class
End Namespace