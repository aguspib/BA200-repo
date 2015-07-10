Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services.BaseLine
Imports NUnit.Framework
Imports Telerik.JustMock
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Global
Imports Telerik.JustMock.Helpers

Namespace Biosystems.Ax00.Core.Services.BaseLine.Test
    <TestFixture()>
    Public Class BaseLineExpirationListenerTest
        <Test()>
        Sub StartListeningTest()
            CreateScenario()

            '1.- force I'm expired
            isBLExpired = True
            Listener.TimeDelay = 1000

            Listener.StartToListening()
            '2.- Check an alarm is requested

            '3.- check lasttypeaction
            NUnit.Framework.Assert.AreEqual(LastTypeActionAlarm, True)
            NUnit.Framework.Assert.AreEqual(lastAlarm, AlarmEnumerates.Alarms.BASELINE_EXPIRED)


            isBLExpired = False
            Dim a = Task.Delay((5 * 1000))
            a.Wait()

            'esperamos....
            NUnit.Framework.Assert.AreEqual(LastTypeActionAlarm, False)
            NUnit.Framework.Assert.AreEqual(lastAlarm, AlarmEnumerates.Alarms.BASELINE_EXPIRED)

            '4.- isBLExpired = true ... esperamos a que chequee otra vez
            NUnit.Framework.Assert.Pass("Everything passed ok!!")
        End Sub

        Dim Listener As BaseLineExpirationListener
        Dim isBLExpired As Boolean

        Public LastTypeActionAlarm As Boolean, lastAlarm As AlarmEnumerates.Alarms

        Sub CreateScenario()
            Dim analyzer = Mock.Create(Of IAnalyzerManager)()
            Mock.Arrange(Function() analyzer.Connected).Returns(True)
            Dim analyzerAlarms = Mock.Create(Of IAnalyzerAlarms)()

            Mock.Arrange(Sub() analyzerAlarms.ActionAlarm(Arg.AnyBool, Arg.IsAny(Of AlarmEnumerates.Alarms))).DoInstead(
                Sub(typeAction As Boolean, value2 As AlarmEnumerates.Alarms)
                    LastTypeActionAlarm = typeAction
                    lastAlarm = value2
                End Sub)

            Dim expirationEntity = Mock.Create(Of IBaseLineExpiration)()

            Mock.Arrange(Function() expirationEntity.IsBlExpired).Returns(Function() isBLExpired)

            Listener = New BaseLineExpirationListener(analyzer, expirationEntity, analyzerAlarms)
        End Sub
    End Class
End Namespace
