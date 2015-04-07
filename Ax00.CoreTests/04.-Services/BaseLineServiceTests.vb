Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports NUnit.Framework
Imports Telerik.JustMock


Namespace Biosystems.Ax00.Core.Services.Tests

    <TestFixture()> Public Class BaseLineServiceTests

        ''' <summary>
        ''' StartService: Normal execution test, check session flags and BL status
        ''' </summary> 
        <Test()> Public Sub StartService_TestStartup_OK()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = True
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY

            Dim baseLineService = New BaseLineService(analyzerManager)
            Dim output = baseLineService.StartService()

            'result
            Assert.IsTrue(output)

            Assert.AreEqual(baseLineService.Status, ServiceStatusEnum.Running)

            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.BaseLine), "")
            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill), "")
            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read), "")
            Assert.AreEqual(analyzerManager.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty), "")

        End Sub

        ''' <summary>
        ''' StartService: Normal execution test, check alarm list
        ''' </summary>
        <Test()> Public Sub StartService_DisableActiveAlarm_NotContainBaselineInitErrorAlarm()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = True
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY
            analyzerManager.Alarms.Add(Alarms.BASELINE_INIT_ERR)

            Dim baseLineService = New BaseLineService(analyzerManager)
            Dim output = baseLineService.StartService()

            'result
            Assert.IsTrue(output)

            Assert.IsFalse(analyzerManager.Alarms.Contains(Alarms.BASELINE_INIT_ERR))

        End Sub

        ''' <summary>
        ''' StartService: Analyzer Manager not connected
        ''' </summary>
        <Test()> Public Sub StartService_AnalyzerNotConnected_NotStartService()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = False
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY

            Dim baseLineService = New BaseLineService(analyzerManager)
            Dim output = baseLineService.StartService()

            'result
            Assert.IsFalse(output)

        End Sub

        ''' <summary>
        ''' StartService: Analyzer Manager in sleeping status mode
        ''' </summary>
        <Test()> Public Sub StartService_AnalyzerStatusIsSleeping_NotStartService()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = True
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING

            Dim baseLineService = New BaseLineService(analyzerManager)
            Dim output = baseLineService.StartService()

            'result
            Assert.IsFalse(output)

        End Sub

        ''' <summary>
        ''' Current Step: Verify the init value
        ''' </summary>
        <Test()> Public Sub CurrentStep_Startup_OK()
            Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            analyzerManager.Connected = True
            analyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY

            Dim baseLineService = New BaseLineService(analyzerManager)

            'result
            Assert.AreEqual(baseLineService.CurrentStep(), BaseLineStepsEnum.NotStarted)

        End Sub

        ''' <summary>
        ''' BAD TEST!
        ''' </summary>
        <Obsolete("Comment with MI")>
        <Test()> Public Sub RecoverProcessTest_ReSendWashing_OK()
            'Dim analyzerManager = Mock.Create(Of IAnalyzerManager)()

            'scenario
            'analyzerManager.SessionFlag(AnalyzerManagerFlags.Washing) = Biosystems.Ax00.Global.StepStringStatus.Initialized <-- SCOPE PROBLEM!!!
            'nextStep = BaseLineStepsEnum.CheckPreviousAlarms <-- INMUTABLE!! I CAN'T TEST THIS (Could be this code need more encapsulation and parameters to control?)

            'Dim baseLineService = New BaseLineService(analyzerManager)
            'baseLineService.RecoverProcess()

            'output
            '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty) 'Re-send Washing

        End Sub
    End Class

End Namespace


