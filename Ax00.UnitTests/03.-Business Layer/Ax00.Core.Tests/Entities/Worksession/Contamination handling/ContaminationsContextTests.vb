Imports NUnit.Framework
Imports Biosystems.Ax00.CC
Imports Telerik.JustMock

Namespace Tests

    <TestFixture()> Public Class ContaminationsContextTests
        <Test()> Public Sub BasicConstructorTest()

            Const before = -2
            Const after = 2
            Const dispensesPerStep = 2  'Maximum 2 dispenses per Step or Cycle.

            Dim contaminations = Telerik.JustMock.Mock.Create(Of AnalyzerContaminationsSpecification)()
            Mock.Arrange(Function() contaminations.DispensesPerStep()).Returns(dispensesPerStep)
            Mock.Arrange(Function() contaminations.ContaminationsContextRange).Returns(New Range(Of Integer)(before, after))
            Mock.Arrange(Function() contaminations.DispensingFactory).Returns(AddressOf DispensingMockFactory)

            Dim cont As New ContaminationsContext(contaminations)
            Const testFrame = "BA350;ANSINF;R1B2:12;R2B2:9;R1B1:13;R2B1:24;R1A1:6;R2A1:9;R1A2:123;R2A2:8"
            cont.FillContentsFromAnalyzer(testFrame)
            Assert.AreEqual(cont.Steps(-2)(1).ReagentNumber, 12)
            Assert.AreEqual(cont.Steps(-2)(2).ReagentNumber, 9)

            'Obtener la washing solution necesaria si le doy la técnica 12
            cont.Steps(-2)(2).RequiredWashingSolution(12, -2)

        End Sub

        Private Function DispensingMockFactory() As ReagentDispensing
            Dim dispensing = Mock.Create(Of ReagentDispensing)()
            'dispensing.AnalysisMode = Biosystems.Ax00.Core.OptimizationPolicyApplier.AnalysisMode.MonoReactive
            Return dispensing
        End Function

    End Class



End Namespace


