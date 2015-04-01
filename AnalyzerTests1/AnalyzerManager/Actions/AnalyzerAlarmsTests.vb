Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Entities

    <TestClass()> Public Class AnalyzerAlarmsTests

        <TestInitialize()> _
        Public Sub InitializeData()
            'alarmsDefintionTableDS.tfmwAlarms.AddtfmwAlarmsRow("GLF_BOARD_FBLD_ERR", "GLF_BOARD_FBLD_ERR", "", "GLF_BOARD_FBLD_ERR", "", "", "", "", "", False, False, "", False, 560)
        End Sub

        <TestMethod()> _
        Public Sub SimpleTranslateErrorCodeToAlarmIdTest()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Mock.Arrange(Function() idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)).Returns(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR)

            Dim alarmResult = idAnalyzer.SimpleTranslateErrorCodeToAlarmId(Nothing, 560)
            Assert.AreEqual(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR, alarmResult)
        End Sub
    End Class
End Namespace


