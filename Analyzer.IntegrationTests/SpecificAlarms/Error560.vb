Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports NUnit.Framework


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestFixture()> Public Class Error560Tests

        <Test()> Public Sub GetInstructionBuilt_TestTool_OK()
            Dim instruction = New List(Of InstructionParameterTO)

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;

            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 1, .ParameterValue = "A200"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "TYPE", .ParameterIndex = 2, .ParameterValue = "STATUS"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "S", .ParameterIndex = 3, .ParameterValue = "2"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 4, .ParameterValue = "46"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "T", .ParameterIndex = 5, .ParameterValue = "15"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "C", .ParameterIndex = 6, .ParameterValue = "20"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "W", .ParameterIndex = 7, .ParameterValue = "20"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "R", .ParameterIndex = 8, .ParameterValue = "0"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "E", .ParameterIndex = 9, .ParameterValue = "560"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "I", .ParameterIndex = 10, .ParameterValue = "0"})


            Dim auxIntruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            For Each o As InstructionParameterTO In instruction
                Dim item = auxIntruction.Find(Function(value As InstructionParameterTO)
                                                  Return value.ParameterIndex = o.ParameterIndex
                                              End Function)
                Assert.AreEqual(o.InstructionType, item.InstructionType)
                Assert.AreEqual(o.ParameterValue, item.ParameterValue)
            Next

            Assert.AreEqual(GetInstructionInString(auxIntruction), "A:A200;TYPE:STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;")
        End Sub

        <Test()> Public Sub ManageAnalyzerReceivedStatus_StatusErr560RetryWithReset_OK()

            Dim analyzerManager As IAnalyzerManager
            analyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)

            'Scenario
            analyzerManager.CommThreadsStarted = True
            GlobalConstants.REAL_DEVELOPMENT_MODE = 2
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "")
            analyzerManager.InstructionTypeSent = GlobalEnumerates.AppLayerEventList.STATE
            analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            'pre condition
            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'output and precondition for reset
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'Failed retry and Reset system
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'output
            Assert.IsFalse(analyzerManager.AnalyzerIsReady)
            Assert.IsFalse(analyzerManager.IsAlarmInfoRequested)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 2)

        End Sub

        <Test()> Public Sub ManageAnalyzerReceivedStatus_StatusErr560Retry_OK()

            Dim analyzerManager As IAnalyzerManager
            analyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)

            'Scenario
            analyzerManager.CommThreadsStarted = True
            GlobalConstants.REAL_DEVELOPMENT_MODE = 2
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess, "")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess, "")
            analyzerManager.InstructionTypeSent = GlobalEnumerates.AppLayerEventList.STATE
            analyzerManager.AnalyzerStatus() = GlobalEnumerates.AnalyzerManagerStatus.STANDBY

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            'precondition
            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'output
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'A200;STATUS;S:2;A:47;T:15;C:20;W:20;R:0;E:0;I:0;
            instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:47;T:15;C:20;W:20;R:0;E:0;I:0")

            'Finish Flight reads without problems
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            'output
            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

        End Sub
    End Class

End Namespace


