Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports NUnit.Framework
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Telerik.JustMock
Imports Biosystems.Ax00.Core.Services.Enums


Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class WarmUpServiceTests

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_DefaultStartupMode_StatusRunningOK()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()
            Dim sut = New WarmUpService(analyzerManager, Mock.Create(Of IAnalyzerManagerFlagsDelegate))

            'scenario
            analyzerManager.Connected = True
            analyzerManager.Model = "A200"
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.WUPprocess, "INPROCESS")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "INPROCESS")
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.StartInstrument, "")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.StartInstrument) = "")
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.Washing, "")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.Washing) = "")
            Mock.Arrange(Sub() analyzerManager.UpdateSessionFlags(Arg.IsAny(Of AnalyzerManagerFlagsDS), AnalyzerManagerFlags.Barcode, "")).DoInstead(Sub() analyzerManager.SessionFlag(AnalyzerManagerFlags.Barcode) = "")

            Dim result = sut.StartService()

            'output
            Assert.IsTrue(analyzerManager.ISEAnalyzer.IsAnalyzerWarmUp)
            Assert.AreEqual(sut.Status, ServiceStatusEnum.Running)
            Assert.AreEqual(sut.NextStep, WarmUpStepsEnum.StartInstrument)
            Assert.IsTrue(result)
        End Sub

        <Test()> Public Sub PauseServiceTest()
            Assert.Inconclusive("TODO")
        End Sub

        <Test()> Public Sub RestartServiceTest()
            Assert.Inconclusive("TODO")
        End Sub

        <Test()> Public Sub RecoverProcessTest()
            Assert.Inconclusive("TODO")
        End Sub



    End Class


End Namespace


