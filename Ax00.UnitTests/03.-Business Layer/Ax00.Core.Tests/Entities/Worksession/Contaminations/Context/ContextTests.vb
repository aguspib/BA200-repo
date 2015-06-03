Imports NUnit.Framework
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Telerik.JustMock
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Tests

    <TestFixture()> Public Class ContextTests

        ''' <summary>
        ''' This method tests the basic constext constructor
        ''' </summary>
        ''' <remarks></remarks>
        <Test()> Public Sub BasicConstructorTest()

            CreateSpecification()

            Dim cont As New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            Const testFrame = "BA350;ANSINF;R1B2:12;R2B2:9;R1B1:13;R2B1:24;R1A1:6;R2A1:9;R1A2:123;R2A2:8"
            cont.FillContentsFromAnalyzer(testFrame)
            Assert.AreEqual(cont.Steps(-2)(1).ExecutionID, 12)
            Assert.AreEqual(cont.Steps(-2)(2).ExecutionID, 9)

        End Sub


        ''' <summary>
        ''' This method tests the function to fill a context with empty data (dummys)
        ''' </summary>
        ''' <remarks></remarks>
        <Test()>
        Public Sub FillEmptyContextTest()
            CreateSpecification()
            Dim cont = New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            cont.FillEmptyContext()
            For i = cont.Steps.Range.Minimum To cont.Steps.Range.Maximum
                For j = 1 To WSExecutionCreator.Instance.ContaminationsSpecification.DispensesPerStep
                    If cont.Steps(i) Is Nothing Then
                        Assert.Fail("Unexpected empty step: " & i)
                    ElseIf cont.Steps(i)(j) Is Nothing Then
                        Assert.Fail("unexpected empty dispensing")
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' This method fills the context with latest elements of a list of ExecutionsDS.twksWSExecutionsRow
        ''' this method should fill all negative indexed steps and zero index step, as it's used to fill pre-existing contents into the context in Static mode.
        ''' </summary>
        ''' <remarks></remarks>
        <Test()>
        Public Sub FillContextInStaticTest_ListBasedOverload()

            Dim reagents As Integer() = {1, 2, 3, 4, 5}

            Dim cont As Context = GetNewContextFilledWithData(reagents)

            Assert.AreEqual(cont.Steps(-2)(1).R1ReagentID, 3)
            Assert.AreEqual(cont.Steps(-1)(1).R1ReagentID, 4)
            Assert.AreEqual(cont.Steps(-0)(1).R1ReagentID, 5)
            Assert.AreEqual(cont.Steps(1)(1).R1ReagentID, 0)
        End Sub

        ''' <summary>
        ''' This method tests the function to fill a context with latest elements of an integers list.
        ''' This elements should fill all negative indexes and zero index step. 
        ''' this method is used to fill a context with a previous group reagents list when solving contaminations between groups
        ''' </summary>
        ''' <remarks></remarks>
        <Test()>
        Public Sub FillContentsFromReagentsIDInStaticTest()
            CreateSpecification()
            Dim cont = New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            Dim lista = New List(Of Integer)
            For i = 100 To 1000
                lista.Add(i)
            Next
            cont.FillContentsFromReagentsIDInStatic(lista, 0)
            Assert.AreEqual(cont.Steps(-1)(1).R1ReagentID, 1000)
            Assert.AreEqual(cont.Steps(-2)(1).R1ReagentID, 999)

            cont = New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            cont.FillContentsFromReagentsIDInStatic(lista, 1)
            Assert.AreEqual(cont.Steps(-1)(1).R1ReagentID, 0)
            Assert.AreEqual(cont.Steps(-2)(1).R1ReagentID, 1000)

            cont = New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            cont.FillContentsFromReagentsIDInStatic(lista, 2)
            Assert.AreEqual(cont.Steps(-1)(1).R1ReagentID, 0)
            Assert.AreEqual(cont.Steps(-2)(1).R1ReagentID, 0)

        End Sub

        ''' <summary>
        ''' This method should return the action required when a new dispensing is going to be sent to the analyzer
        ''' This method follows all the contaminations defined in the private "SetContaminations" method of this test.
        ''' </summary>
        ''' <remarks></remarks>
        <Test()>
        Public Sub ActionRequiredForAGivenDispensing()
            Dim cont = GetNewContextFilledWithData({1, 2, 3, 4, 5})
            Dim result = cont.ActionRequiredForAGivenDispensing(GenerateTemporaryRow(2))
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.GoAhead)
            result = cont.ActionRequiredForAGivenDispensing(GenerateTemporaryRow(1))
            Assert.AreEqual(result.Action, IContaminationsAction.RequiredAction.Wash)
            Assert.AreEqual(result.InvolvedWashes(0).WashingSolutionCode, "DISW")

        End Sub

#Region "Private functions"
        ''' <summary>
        ''' This method generates a mocked version of the ContaminationsSpecification
        ''' In this test, we create an analyzer with a range of -2 to 2 contaminations viewport
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub CreateSpecification()
            Const dispensesPerStep = 2  'Maximum 2 dispenses per Step or Cycle.
            Const before = -2
            Const after = 2

            Dim contaminations = Telerik.JustMock.Mock.Create(Of IAnalyzerContaminationsSpecification)()
            Mock.Arrange(Function() contaminations.DispensesPerStep()).Returns(dispensesPerStep)
            Mock.Arrange(Function() contaminations.ContaminationsContextRange).Returns(New Range(Of Integer)(before, after))
            Mock.Arrange(Function() contaminations.CreateDispensing).Returns(AddressOf DispensingMockFactory)
            WSExecutionCreator.Instance.ContaminationsSpecification = contaminations

        End Sub

        ''' <summary>
        ''' This method is the mocked Dispensing factory for this test.
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Function DispensingMockFactory() As IDispensing
            Dim dispensing = Telerik.JustMock.Mock.Create(Of IDispensing)()

            Mock.Arrange(Sub() dispensing.FillDispense(Arg.AnyObject, Arg.AnyObject)).DoInstead(
                Sub(specification As IAnalyzerContaminationsSpecification, row As ExecutionsDS.twksWSExecutionsRow)
                    dispensing.R1ReagentID = row.ReagentID
                    SetContaminations(dispensing)
                End Sub)

            Mock.Arrange(Function() dispensing.RequiredActionForDispensing(Arg.AnyObject, Arg.AnyInt, Arg.AnyInt)).Returns(
                Function(targetDispensing As IDispensing, scope As Integer, reagentNumber As Integer) As IContaminationsAction
                    Dim action As IContaminationsAction = Mock.Create(Of IContaminationsAction)()
                    If (dispensing.Contamines Is Nothing OrElse Not dispensing.Contamines.Any) Then
                        action.Action = IContaminationsAction.RequiredAction.GoAhead
                    ElseIf dispensing.Contamines.ContainsKey(targetDispensing.R1ReagentID) Then
                        action.Action = IContaminationsAction.RequiredAction.Wash
                        action.InvolvedWash = dispensing.Contamines(targetDispensing.R1ReagentID).RequiredWashing
                    End If
                    Return action
                End Function)
            Return dispensing
        End Function

        ''' <summary>
        ''' This method gets a ReagentID and sets all test-case contaminations.
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Sub SetContaminations(dispensing As IDispensing)


            If dispensing.Contamines Is Nothing Then
                Assert.Fail("Dispensing can't have contaminations")
            End If

            Dim AppendContamination =
                Sub(contaminatedID As String, washingSolutionName As String, washingPersistence As Integer)
                    dispensing.Contamines.Add(contaminatedID, CreateDispensingContaminationDescription(contaminatedID, washingPersistence, washingSolutionName))
                End Sub

            'contaminations defined in this test:
            '1 --> 2 High persistence
            '2 --> 1 High persistence
            '3 does not contamine
            '4 --> 1 low persistence
            '5 --> 4 low persistence


            Select Case dispensing.R1ReagentID
                Case 1
                    AppendContamination(2, "Washing1", 2)
                Case 2
                    AppendContamination(1, "Washing1", 2)
                Case 3

                Case 4
                    AppendContamination(1, "DISW", 1)
                Case 5
                    AppendContamination(4, "DISW", 1)
            End Select
        End Sub

        ''' <summary>
        ''' This method generates a mocked version of a WashingSolution description.
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Function CreateWashingDescription(Strength As Integer, WSCode As String) As IWashingDescription
            Dim washingDesc = Mock.Create(Of IWashingDescription)()
            washingDesc.WashingStrength = Strength
            washingDesc.WashingSolutionCode = WSCode
            Return washingDesc
        End Function

        ''' <summary>
        ''' This method generates a mocked version of a DispensingContaminationDescription description.
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Function CreateDispensingContaminationDescription(contaminatedReagent As Integer, washingStrength As Integer, washingCode As String) As IDispensingContaminationDescription
            Dim dispensingContaminationDescription = Mock.Create(Of IDispensingContaminationDescription)()
            dispensingContaminationDescription.ContaminedReagent = contaminatedReagent
            dispensingContaminationDescription.RequiredWashing = CreateWashingDescription(washingStrength, washingCode)
            Return dispensingContaminationDescription
        End Function

        ''' <summary>
        ''' This method generates a new Context instance filled with test-case data.
        ''' </summary>
        ''' <remarks></remarks>
        Private Function GetNewContextFilledWithData(ByVal reagents As Integer()) As Context

            CreateSpecification()

            Dim List As New List(Of ExecutionsDS.twksWSExecutionsRow)

            For Each i As Integer In reagents
                List.Add(GenerateTemporaryRow(i))
            Next

            Dim cont As New Context(WSExecutionCreator.Instance.ContaminationsSpecification)
            cont.FillContextInStatic(List)
            Return cont
        End Function

        ''' <summary>
        ''' This method gets a ReagengID and returns a twksWSExecutionsRow with the same ReagentID
        ''' </summary>
        ''' <param name="reagentID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GenerateTemporaryRow(reagentID As Integer) As ExecutionsDS.twksWSExecutionsRow
            Static MyDataSet As New ExecutionsDS
            Dim row As ExecutionsDS.twksWSExecutionsRow = MyDataSet.twksWSExecutions.NewRow()
            row.ReagentID = reagentID
            Return row
        End Function

#End Region

    End Class



End Namespace


