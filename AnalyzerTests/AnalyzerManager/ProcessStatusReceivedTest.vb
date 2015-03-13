Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Telerik.JustMock
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestClass()> Public Class ProcessStatusReceivedTest

        ''' <summary>
        ''' Test End_run_end state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_END_RUN_END()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "10"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test SOUND_DONE state
        ''' </summary>
        ''' <remarks></remarks>        
        <TestMethod()>
        <Obsolete("Not working")>
        Public Sub EvaluateActionCodeValueTest_SOUND_DONE()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "60"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            idAnalyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS"
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(idAnalyzer.Ringing)
        End Sub

        ''' <summary>
        ''' Test END_RUN_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_END_RUN_START()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "9"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_ABORT_START()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "15"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(idAnalyzer.AbortInstructionSent)
            Assert.IsTrue(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_END state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_ABORT_END()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "16"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(idAnalyzer.PauseInstructionSent)
            Assert.IsFalse(idAnalyzer.AbortInstructionSent)
            Assert.IsFalse(idAnalyzer.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_PAUSE_START()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "96"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            idAnalyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS"
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(idAnalyzer.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_END state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_PAUSE_END()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "97"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(idAnalyzer.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_RECOVER_INSTRUMENT_START()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "70"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(idAnalyzer.RecoverInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_END state
        ''' </summary>
        ''' <remarks>
        ''' WARNING!! This case has a possible access to database, the immutability of the scenario is not guaranteed
        ''' </remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_RECOVER_INSTRUMENT_END()
            Dim idAnalyzer = Mock.Create(Of IAnalyzerManager)()
            Dim target = New ProcessStatusReceived(idAnalyzer)

            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "71"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(idAnalyzer.RecoverInstructionSent)
        End Sub

    End Class


End Namespace


