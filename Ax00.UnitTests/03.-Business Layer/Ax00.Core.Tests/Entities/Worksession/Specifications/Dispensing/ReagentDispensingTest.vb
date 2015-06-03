Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing.Tests


    Public Class ReagentDispensingTest
        Sub New()
            CreateSpecification()
        End Sub

        <Test()> Sub BasicConstructor()
            Dim dispensing = ContaminationsSpecification.CreateDispensing()
            dispensing.R1ReagentID = 1
        End Sub

        Public Shared Sub CreateSpecification()
            Static done As Boolean = False
            If done Then Return Else done = True

            Const dispensesPerStep = 2  'Maximum 2 dispenses per Step or Cycle.
            Const before = -2
            Const after = 2

            Dim contaminations = Telerik.JustMock.Mock.Create(Of IAnalyzerContaminationsSpecification)()
            Mock.Arrange(Function() contaminations.DispensesPerStep()).Returns(dispensesPerStep)
            Mock.Arrange(Function() contaminations.ContaminationsContextRange).Returns(New Range(Of Integer)(before, after))
            Mock.Arrange(Function() contaminations.CreateDispensing).Returns(AddressOf DispensingFactory)
            WSExecutionCreator.Instance.ContaminationsSpecification = contaminations

        End Sub

        Private Shared Function DispensingFactory() As IDispensing
            Dim Gdisp = New GenericDispensing()
            Return Gdisp
        End Function

        Private Shared Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification
            Return WSExecutionCreator.Instance.ContaminationsSpecification
        End Function
    End Class



    Class GenericDispensing
        Inherits Ax00DispensingBase
        Sub New()
            getAllContaminationsForAReagent = AddressOf MockedGetAllcontaminationsForAReagent
        End Sub

        Private Function MockedGetAllcontaminationsForAReagent(ReagentID As Integer) As TypedGlobalDataTo(Of EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow))
            Dim contaDS = New ContaminationsDS
            Dim addReagent = Sub(contaminator As Integer, contaminated As Integer, Washingsol As String)
                                 contaDS.tparContaminations.AddtparContaminationsRow("R1", contaminator, contaminated, 0, "R1", "Washing1", 1, "FakeTest", "Biosystems", Now, "Wather", "TestReagent", "AnotherTestReagent", contaminator, contaminated, False)
                             End Sub
            addReagent(1, 2, "Wash1")
            addReagent(1, 3, "Wash1")
            addReagent(2, 1, "Wash1")
            addReagent(2, 3, "Wash1")
            Dim resultData = (From conta As ContaminationsDS.tparContaminationsRow In contaDS.tparContaminations Where conta.ReagentContaminatorID = ReagentID)
            Dim result As New TypedGlobalDataTo(Of EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow))
            result.SetDatos = resultData
            Return result
        End Function
    End Class


End Namespace
