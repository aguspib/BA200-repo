Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports NUnit.Framework
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class RotorChangeServicesTests

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_NormalStartup_OK()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()
            Dim testGlobal = New GlobalDataTO()

            'scenario
            analyzerManager.Alarms.Clear()
            analyzerManager.Connected() = True
            testGlobal.HasError = False
            Mock.Arrange(Function() analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")).Returns(testGlobal)
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS")
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.NewRotor, "")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.NewRotor) = "")

            Dim rotorChange = New RotorChangeService(analyzerManager)
            Dim result = rotorChange.StartService()

            'output
            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess), "INPROCESS")
            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.NewRotor), "") 'StepStringStatus.Empty)
            Assert.IsTrue(result)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_NotConnected_ReturnFalse()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()
            Dim testGlobal = New GlobalDataTO()

            'scenario
            analyzerManager.Alarms.Clear()
            analyzerManager.Connected() = False
            testGlobal.HasError = True
            testGlobal.ErrorCode = "Not Connected"
            Mock.Arrange(Function() analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")).Returns(testGlobal)

            Dim rotorChange = New RotorChangeService(analyzerManager)
            Dim result = rotorChange.StartService()

            'output
            Assert.IsFalse(result)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_AnalyzerManagerNotWorks_ThrowException()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()
            Dim testGlobal = New GlobalDataTO()
            Try
                'scenario
                analyzerManager.Alarms.Clear()
                analyzerManager.Connected() = True
                testGlobal.HasError = True
                testGlobal.ErrorCode = "AnalyzerManagerError"
                Mock.Arrange(Function() analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")).Returns(testGlobal)

                Dim rotorChange = New RotorChangeService(analyzerManager)
                rotorChange.StartService()

                'Something wrong with the exception.. then
                Assert.Fail()

            Catch ex As Exception
                'output
                Assert.AreEqual(ex.Message, "AnalyzerManagerError")
            End Try
        End Sub

    End Class

End Namespace


