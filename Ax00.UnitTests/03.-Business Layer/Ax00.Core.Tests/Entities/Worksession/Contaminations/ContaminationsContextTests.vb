Imports NUnit.Framework
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Telerik.JustMock

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Tests

    <TestFixture()> Public Class ContaminationsContextTests
        <Test()> Public Sub BasicConstructorTest()

            Const before = -2
            Const after = 2
            Const dispensesPerStep = 2  'Maximum 2 dispenses per Step or Cycle.

            Dim contaminations = Telerik.JustMock.Mock.Create(Of IAnalyzerContaminationsSpecification)()
            Mock.Arrange(Function() contaminations.DispensesPerStep()).Returns(dispensesPerStep)
            Mock.Arrange(Function() contaminations.ContaminationsContextRange).Returns(New Range(Of Integer)(before, after))
            Mock.Arrange(Function() contaminations.CreateDispensing).Returns(AddressOf DispensingMockFactory)
            WSExecutionCreator.Instance.ContaminationsSpecification = contaminations

            Dim cont As New ContaminationsContext(contaminations)
            Const testFrame = "BA350;ANSINF;R1B2:12;R2B2:9;R1B1:13;R2B1:24;R1A1:6;R2A1:9;R1A2:123;R2A2:8"
            cont.FillContentsFromAnalyzer(testFrame)
            Assert.AreEqual(cont.Steps(-2)(1).ExecutionID, 12)
            Assert.AreEqual(cont.Steps(-2)(2).ExecutionID, 9)

            'Obtener la washing solution necesaria si le doy la técnica 12
            cont.Steps(-2)(2).RequiredWashingSolution(New ReagentDispensing With {.R1ReagentID = 12}, -2)

        End Sub

        Private Function DispensingMockFactory() As IReagentDispensing
            Dim dispensing = Mock.Create(Of IReagentDispensing)()
            Return dispensing
        End Function

    End Class



End Namespace


