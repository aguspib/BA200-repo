Imports System.IO
Imports System.Xml.Serialization
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock
Imports Biosystems.Ax00.Data.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing.Tests


    Public Class ReagentDispensingTest
        Sub New()
            CreateSpecification()
        End Sub

        <Test()> Sub BasicConstructorTest()
            Dim dispensing = ContaminationsSpecification.CreateDispensing()
            dispensing.R1ReagentID = 1
        End Sub

        <Test()> Sub ReagentRequiresWashingOrSkipTest()

            '1 contamines  2 and requires "Wash1"
            '1 contamines 3 and requires  "Wash2"
            '2 contamines 1 and requires  "Wash3"
            '2 contamines 3 and requires  "Wash4"


            Dim dispensing = ContaminationsSpecification.CreateDispensing()
            dispensing.R1ReagentID = 1

            Dim dispensing2 = ContaminationsSpecification.CreateDispensing()
            dispensing2.R1ReagentID = 2

            Dim result = dispensing.RequiredActionForDispensing(dispensing2, -2, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Wash)

            result = dispensing2.RequiredActionForDispensing(dispensing, -2, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Wash)

            result = dispensing2.RequiredActionForDispensing(dispensing, -1, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Wash)

            result = dispensing2.RequiredActionForDispensing(dispensing, 1, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Skip)

            result = dispensing2.RequiredActionForDispensing(dispensing, 2, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Skip)

            Dim dispensing3 = ContaminationsSpecification.CreateDispensing()
            dispensing3.R1ReagentID = 123 ' Does not have any contamination

            result = dispensing3.RequiredActionForDispensing(dispensing, -2, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.GoAhead)

            result = dispensing3.RequiredActionForDispensing(dispensing, -1, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.GoAhead)

            result = dispensing3.RequiredActionForDispensing(dispensing, 1, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.GoAhead)

            result = dispensing3.RequiredActionForDispensing(dispensing, 2, 1)
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.GoAhead)

        End Sub

        <Test()> Sub WashingIDTest()
            Dim dispensing = ContaminationsSpecification.CreateDispensing()
            dispensing.WashingID = 5
            Assert.AreEqual(dispensing.WashingDescription.WashingSolutionCode, "WASH01")
            Assert.AreEqual(dispensing.WashingDescription.WashingStrength, 2)
        End Sub

        Private Shared Sub CreateSpecification()
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

            Dim analyzer = Mock.Create(Of IAnalyzerManager)()
            AnalyzerManager._currentAnalyzer = analyzer

        End Sub

        Private Shared Function DispensingFactory() As IDispensing
            Dim gDisp = New GenericDispensing()
            Return gDisp
        End Function

        Private Shared Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification
            Return WSExecutionCreator.Instance.ContaminationsSpecification
        End Function

    End Class

    Class GenericDispensing
        Inherits Ax00DispensingBase
        Sub New()
            GetAllContaminationsForAReagent = AddressOf MockedGetAllcontaminationsForAReagent

            WSExecutionsDAO = Mock.Create(Of IvWSExecutionsDAO)()

            'GetWashingSolution(ExecutionID As Integer, AnalyzerID As String, WorkSessionID As String) As vWSExecutionsDS
            Mock.Arrange(Function() WSExecutionsDAO.GetWashingSolution(Arg.AnyInt, Arg.AnyString, Arg.AnyString)).Returns(
                Function(executionID As Integer, analyzerID As String, workSessionID As String) As vWSExecutionsDS
                    Return GetWashingSolutionMock(executionID, analyzerID, workSessionID)
                End Function
                )
        End Sub

        Function GetWashingSolutionMock(executionID As Integer, analyzerID As String, workSessionID As String) As vWSExecutionsDS
            Return GenerateTemporaryRow(executionID, "WASH01")
        End Function

        Private Function GenerateTemporaryRow(elementID As Integer, WashingSolutionCode As String) As vWSExecutionsDS
            Dim MyDataSet As New vWSExecutionsDS
            Dim row As vWSExecutionsDS.WashingSolutionSELECTRow = MyDataSet.WashingSolutionSELECT.NewRow()
            row.ELEMENTID = elementID
            row.SOLUTIONCODE = WashingSolutionCode
            row.TUBECONTENT = "WASHSOL"
            MyDataSet.WashingSolutionSELECT.AddWashingSolutionSELECTRow(row)
            Return MyDataSet
        End Function
        Private Function MockedGetAllcontaminationsForAReagent(ReagentID As Integer) As TypedGlobalDataTo(Of EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow))
            Dim contaDS = New ContaminationsDS
            Dim addReagent = Sub(contaminator As Integer, contaminated As Integer, Washingsol As String)
                                 contaDS.tparContaminations.AddtparContaminationsRow("R1", contaminator, contaminated, 0, "R1", "Washing1", 1, "FakeTest", "Biosystems", Now, "Wather", "TestReagent", "AnotherTestReagent", contaminator, contaminated, False)
                             End Sub
            addReagent(1, 2, "Wash1")
            addReagent(1, 3, "Wash2")
            addReagent(2, 1, "Wash3")
            addReagent(2, 3, "Wash4")
            Dim resultData = (From conta As ContaminationsDS.tparContaminationsRow In contaDS.tparContaminations Where conta.ReagentContaminatorID = ReagentID)
            Dim result As New TypedGlobalDataTo(Of EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow))
            result.SetDatos = resultData
            Return result
        End Function


        <Test()>
        Sub BA200DispensingSerialization()
            Dim disp As New BA200Dispensing

            Dim serializer = New XmlSerializer(disp.GetType)
            Dim stream As New System.IO.MemoryStream
            serializer.Serialize(stream, disp)
            stream.Position = 0
            Dim reader As New StreamReader(stream)
            Dim contents = reader.ReadToEnd()
            MsgBox(contents)
        End Sub
    End Class


End Namespace
