Imports Biosystems.Ax00.Core.Interfaces
Imports NUnit.Framework
Imports Biosystems.Ax00.Core.Services
Imports Telerik.JustMock
Imports Biosystems.Ax00.Core.Services.Enums


Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class WarmUpServiceTests

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_DefaultStartupMode_StatusRunningOK()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = True
            analyzerManager.Model = "A200"

            Dim sut = New WarmUpService(analyzerManager)
            Dim result = sut.StartService()

            'output
            Assert.IsTrue(analyzerManager.ISEAnalyzer.IsAnalyzerWarmUp)
            Assert.AreEqual(sut.Status, ServiceStatusEnum.Running)
            Assert.AreEqual(sut.NextStep, WarmUpStepsEnum.StartInstrument)
            Assert.IsTrue(result)
        End Sub

        <Test()> Public Sub PauseServiceTest()
            Assert.Fail()
        End Sub

        <Test()> Public Sub RestartServiceTest()
            Assert.Fail()
        End Sub

        <Test()> Public Sub RecoverProcessTest()
            Assert.Fail()
        End Sub
    End Class


End Namespace


