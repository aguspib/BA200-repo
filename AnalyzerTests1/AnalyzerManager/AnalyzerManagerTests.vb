Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestClass()> Public Class AnalyzerManagerTests

        <TestMethod()> Public Sub STATUS_ERR560()

            Dim analyzerManager As IAnalyzerManager
            analyzerManager = New BA200AnalyzerEntity(String.Empty, String.Empty, Nothing)

            'Scenario
            analyzerManager.CommThreadsStarted = True
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess, "INPROCESS")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.WaitForAnalyzerReady, "INI")
            analyzerManager.SetSessionFlags(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "INI")

            Dim instruction = New List(Of InstructionParameterTO)

            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 1, .ParameterValue = "A200"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "TYPE", .ParameterIndex = 2, .ParameterValue = "STATE"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "S", .ParameterIndex = 3, .ParameterValue = "2"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = 4, .ParameterValue = "1"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "T", .ParameterIndex = 5, .ParameterValue = "15"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "C", .ParameterIndex = 6, .ParameterValue = "25"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "W", .ParameterIndex = 7, .ParameterValue = "20"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "R", .ParameterIndex = 8, .ParameterValue = "0"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "E", .ParameterIndex = 9, .ParameterValue = "560"})
            instruction.Add(New InstructionParameterTO With {.InstructionType = "I", .ParameterIndex = 10, .ParameterValue = "1"})

            analyzerManager.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED, False, instruction)

            Dim alarmResults = analyzerManager.Alarms
        End Sub
    End Class


End Namespace


