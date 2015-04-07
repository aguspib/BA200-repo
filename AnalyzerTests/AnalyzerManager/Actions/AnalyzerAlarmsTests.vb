Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports NUnit.Framework
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Entities

    <TestFixture()> Public Class AnalyzerAlarmsTests

        ''' <summary>
        ''' SimpleTranslateErrorCodeToAlarmId: With mocked data, found 560 error
        ''' </summary>
        <Test()> Public Sub SimpleTranslateErrorCodeToAlarmId_Translate560Error_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Mock.Arrange(Function() idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)).Returns(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR)

            Dim alarmResult = idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)
            Assert.AreEqual(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR, alarmResult)
        End Sub

        ''' <summary>
        ''' SimpleTranslateErrorCodeToAlarmId: With mocked data, NOT found 
        ''' </summary>
        <Test()> Public Sub SimpleTranslateErrorCodeToAlarmId_Translate560Error_NotFound()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Mock.Arrange(Function() idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 550)).Returns(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR)

            Dim alarmResult = idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)
            Assert.AreEqual(AlarmEnumerates.Alarms.NONE, alarmResult)
        End Sub
    End Class
End Namespace


