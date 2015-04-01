Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports NUnit.Framework
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Entities

    <TestFixture()> Public Class AnalyzerAlarmsTests

        <Test()> _
        Public Sub SimpleTranslateErrorCodeToAlarmIdTest()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Mock.Arrange(Function() idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)).Returns(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR)

            Dim alarmResult = idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)
            Assert.AreEqual(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR, alarmResult)
        End Sub
    End Class
End Namespace


