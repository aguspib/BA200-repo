Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports NUnit.Framework
Imports Telerik.JustMock

Namespace Biosystems.Ax00.Core.Services.Tests
    <TestFixture()> Public Class BaseLineServiceTests

        ''' <summary>
        ''' 
        ''' </summary>
        <Test()> Public Sub StartService_StartRotorChangeServiceAndRaiseEventWithAnsInfIntruction_NextStepIsNewRotor()
            Dim iseAnalyzer = Mock.Create(Of IISEManager)()
            Dim analyzerManager As IAnalyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)
            Dim baseLine = New BaseLineService(analyzerManager)
            Dim warmUp = New WarmUpService(analyzerManager, baseLine)
            Dim rotorChange = New RotorChangeServices(analyzerManager, warmUp, baseLine)
            Dim testGlobal = New GlobalDataTO()

            'scenario
            analyzerManager.CommThreadsStarted = True
            GlobalConstants.REAL_DEVELOPMENT_MODE = 2
            analyzerManager.SetSessionFlags(AnalyzerManagerFlags.CONNECTprocess, "")
            analyzerManager.SetSessionFlags(AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
            analyzerManager.SetSessionFlags(AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")
            analyzerManager.ISEAnalyzer = iseAnalyzer
            analyzerManager.IseIsAlreadyStarted = False
            analyzerManager.Connected = True
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY
            analyzerManager.Model = "A200"
            testGlobal.HasError = False
            'A200;ANSINF;GC:1;PC:1;RC:1;SC:1;HS:1;WS:1;SW:1765;WW:2043;PT:37.5;FS:0;FT:0.0;HT:45.0;R1T:36.9;R2T:36.9;IS:1;HSV:336;WSV:311;
            Dim instruction = GetInstructionBuilt("A200", "ANSINF", "GC:1;PC:1;RC:1;SC:1;HS:1;WS:1;SW:1765;WW:2043;PT:37.5;FS:0;FT:0.0;HT:45.0;R1T:36.9;R2T:36.9;IS:0;HSV:336;WSV:311")

            'preconditions
            Dim step1 = baseLine.StartService()
            Assert.IsTrue(step1)
            Dim step2 = warmUp.StartService()
            Assert.IsTrue(step2)
            Dim step3 = rotorChange.StartService()
            Assert.IsTrue(step3)

            'act
            analyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.ANSINF_RECEIVED, False, instruction)

            'result --> Could be a no sense assert, verify preconditions to do the correct test 
            Assert.AreEqual(rotorChange.CurrentStepForTest, RotorChangeStepsEnum.NewRotor)
        End Sub
    End Class


End Namespace


