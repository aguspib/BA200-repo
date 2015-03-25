Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestClass()> Public Class Error560Tests

        <TestMethod()> Public Sub TestUtils()
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

        <TestMethod()> Public Sub STATUS_ERR560_RetryKO_Reset()

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

            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'Failed retry and Reset system
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            Assert.IsFalse(analyzerManager.AnalyzerIsReady)
            Assert.IsFalse(analyzerManager.IsAlarmInfoRequested)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 2)
            Assert.AreEqual(analyzerManager.InfoActivated(), 0)

        End Sub

        <TestMethod()> Public Sub STATUS_ERR560_RetryOK()

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

            Assert.AreEqual(analyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "")

            'A200;STATUS;S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0;
            Dim instruction = GetInstructionBuilt("A200", "STATUS", "S:2;A:46;T:15;C:20;W:20;R:0;E:560;I:0")

            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

            'Not connected and Retry
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsTrue(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 1)

            'A200;ANSINF;GC:0;PC:0;RC:0;SC:0;HS:1;WS:1;SW:1469;WW:983;PT:37.5;FS:0;FT:0.0;HT:45.1;R1T:37.0;R2T:37.0;IS:0;HSV:454;WSV:554
            instruction = GetInstructionBuilt("A200", "ANSINF", "GC:0;PC:0;RC:0;SC:0;HS:1;WS:1;SW:1469;WW:983;PT:37.5;FS:0;FT:0.0;HT:45.1;R1T:37.0;R2T:37.0;IS:0;HSV:454;WSV:554")

            'Failed retry and Reset system
            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ANSINF_RECEIVED, False, instruction)

            Assert.IsFalse(analyzerManager.CanManageRetryAlarm)
            Assert.IsFalse(analyzerManager.CanSendingRepetitions)
            Assert.AreEqual(analyzerManager.NumSendingRepetitionsTimeout, 0)

        End Sub
    End Class


End Namespace


