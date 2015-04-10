Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Telerik.JustMock
Imports Biosystems.Ax00.Global
Imports NUnit.Framework


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestFixture()> Public Class ProcessStatusReceivedTest

        ''' <summary>
        ''' Test End_run_end state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_EndRunEnd_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "10"

            Dim processStatusReceived = New ProcessStatusReceived(idAnalyzer, Nothing)
            processStatusReceived.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsFalse(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test SOUND_DONE state
        ''' </summary>
        <Test()>
        Public Sub EvaluateActionCodeValueTest_SoundDone_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            idAnalyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS"
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "60"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsFalse(idAnalyzer.Ringing)
        End Sub

        ''' <summary>
        ''' Test END_RUN_START state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_EndRunStart_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "9"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsTrue(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_START state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_AbortStart_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "15"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsTrue(idAnalyzer.AbortInstructionSent)
            Assert.IsTrue(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_END state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_AbortEnd_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "16"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsFalse(idAnalyzer.PauseInstructionSent)
            Assert.IsFalse(idAnalyzer.AbortInstructionSent)
            Assert.IsFalse(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_START state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_PauseStart_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            idAnalyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS"
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "96"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsTrue(idAnalyzer.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_END state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_PauseEnd_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "97"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsFalse(idAnalyzer.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_START state
        ''' </summary>
        <Test()> Public Sub EvaluateActionCodeValueTest_RecoverInstrumentStart_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "70"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsTrue(idAnalyzer.RecoverInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_END state
        ''' </summary>
        ''' <remarks>
        ''' WARNING!! This case has a possible access to database, the immutability of the scenario is not guaranteed
        ''' </remarks>
        <Test()> Public Sub EvaluateActionCodeValueTest_RecoverInstrumentEnd_OK()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()

            'scenario
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "71"

            Dim target = New ProcessStatusReceived(idAnalyzer, Nothing)
            target.EvaluateActionCodeValue(New GlobalEnumerates.AnalyzerManagerAx00Actions, instruction, New GlobalDataTO)

            'output
            Assert.IsFalse(idAnalyzer.RecoverInstructionSent)
        End Sub

    End Class

End Namespace


