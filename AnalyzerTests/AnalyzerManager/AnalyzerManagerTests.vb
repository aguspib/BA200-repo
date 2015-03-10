Imports AnalyzerTests.Biosystems.Ax00.Core.Entities.Tests.Mock
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.Core.Entities.Tests

    <TestClass()> Public Class AnalyzerManagerTests

        ''' <summary>
        ''' Test End_run_end state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_END_RUN_END()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO            
            instruction.ParameterValue = "10"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(target.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test SOUND_DONE state
        ''' </summary>
        ''' <remarks></remarks>        
        <TestMethod()>
        <Obsolete("Not working")>
        Public Sub EvaluateActionCodeValueTest_SOUND_DONE()

            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "60"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS"
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(target.Ringing)
        End Sub

        ''' <summary>
        ''' Test END_RUN_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_END_RUN_START()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "9"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(target.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_ABORT_START()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "15"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(target.AbortInstructionSent)
            Assert.IsTrue(target.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test ABORT_END state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_ABORT_END()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "16"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(target.PauseInstructionSent)
            Assert.IsFalse(target.AbortInstructionSent)
            Assert.IsFalse(target.EndRunInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_PAUSE_START()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "96"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess) = "INPROCESS"
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(target.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test PAUSE_END state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_PAUSE_END()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "97"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(target.PauseInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_START state
        ''' </summary>
        ''' <remarks></remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_RECOVER_INSTRUMENT_START()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "70"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsTrue(target.RecoverInstructionSent)
        End Sub

        ''' <summary>
        ''' Test RECOVER_INSTRUMENT_END state
        ''' </summary>
        ''' <remarks>
        ''' WARNING!! This case has a possible access to database, the immutability of the scenario is not guaranteed
        ''' </remarks>
        <TestMethod()> Public Sub EvaluateActionCodeValueTest_RECOVER_INSTRUMENT_END()
            Dim blMock = New BaseLineEntityMock
            Dim target = New BA200AnalyzerMock("", "", blMock)
            Dim instruction As New InstructionParameterTO
            instruction.ParameterValue = "71"
            Dim myActionValue As GlobalEnumerates.AnalyzerManagerAx00Actions
            Dim myGlobal As New GlobalDataTO
            target.EvaluateActionCodeValue(myActionValue, instruction, myGlobal)
            Assert.IsFalse(target.RecoverInstructionSent)
        End Sub

    End Class


End Namespace


