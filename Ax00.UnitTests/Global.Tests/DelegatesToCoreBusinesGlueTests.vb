Imports System.Reflection
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types
Imports NUnit.Framework
Imports Telerik.JustMock

Namespace Tests

    '<TestFixture()> Public Class DelegatesToCoreBusinesGlueTests


    '    <Test()> Public Sub CreateContaminationManagerTest()
    '        CreateScenario()
    '        Dim obj = New DelegatesToCoreBusinesGlue.ContaminationManagerWrapper(False, Nothing, "BA200", 2, 2, Nothing, New List(Of ExecutionsDS.twksWSExecutionsRow))
    '        Try
    '            obj.ApplyOptimizations(Nothing, "BA200", New List(Of ExecutionsDS.twksWSExecutionsRow)())
    '        Catch ex As TargetInvocationException
    '        Catch
    '            Throw
    '        End Try
    '    End Sub

    '    Sub CreateScenario()
    '        Const dispensesPerStep = 2  'Maximum 2 dispenses per Step or Cycle.
    '        Const before = -2
    '        Const after = 2

    '        Dim contaminations = Telerik.JustMock.Mock.Create(Of IAnalyzerContaminationsSpecification)()
    '        Mock.Arrange(Function() contaminations.DispensesPerStep()).Returns(dispensesPerStep)
    '        Mock.Arrange(Function() contaminations.ContaminationsContextRange).Returns(New Range(Of Integer)(before, after))
    '        Mock.Arrange(Function() contaminations.CreateDispensing).Returns(AddressOf DispensingMockFactory)
    '        WSExecutionCreator.Instance.ContaminationsSpecification = contaminations
    '    End Sub

    '    Private Shared Function DispensingMockFactory() As IDispensing
    '        Dim dispensing = Telerik.JustMock.Mock.Create(Of IDispensing)()

    '        Mock.Arrange(Sub() dispensing.FillDispense(Arg.AnyObject, Arg.AnyObject)).DoInstead(
    '            Sub(specification As IAnalyzerContaminationsSpecification, row As ExecutionsDS.twksWSExecutionsRow)
    '                dispensing.R1ReagentID = row.ReagentID
    '                SetContaminations(dispensing)
    '            End Sub)

    '        Mock.Arrange(Function() dispensing.RequiredActionForDispensing(Arg.AnyObject, Arg.AnyInt, Arg.AnyInt)).Returns(
    '            Function(targetDispensing As IDispensing, scope As Integer, reagentNumber As Integer) As IContaminationsAction
    '                Dim action As IContaminationsAction = Mock.Create(Of IContaminationsAction)()
    '                If (dispensing.Contamines Is Nothing OrElse Not dispensing.Contamines.Any) Then
    '                    action.Action = IContaminationsAction.RequiredAction.GoAhead
    '                ElseIf dispensing.Contamines.ContainsKey(targetDispensing.R1ReagentID) Then
    '                    action.Action = IContaminationsAction.RequiredAction.Wash
    '                    action.InvolvedWash = dispensing.Contamines(targetDispensing.R1ReagentID).RequiredWashing
    '                End If
    '                Return action
    '            End Function)
    '        Return dispensing
    '    End Function

    '    Private Shared Sub SetContaminations(dispensing As IDispensing)


    '        If dispensing.Contamines Is Nothing Then
    '            Assert.Fail("Dispensing can't have contaminations")
    '        End If

    '        Dim AppendContamination =
    '            Sub(contaminatedID As String, washingSolutionName As String, washingPersistence As Integer)
    '                dispensing.Contamines.Add(contaminatedID, CreateDispensingContaminationDescription(contaminatedID, washingPersistence, washingSolutionName))
    '            End Sub

    '        'contaminations defined in this test:
    '        '1 --> 2 High persistence
    '        '2 --> 1 High persistence
    '        '3 does not contamine
    '        '4 --> 1 low persistence
    '        '5 --> 4 low persistence


    '        Select Case dispensing.R1ReagentID
    '            Case 1
    '                AppendContamination(2, "Washing1", 2)
    '            Case 2
    '                AppendContamination(1, "Washing1", 2)
    '            Case 3

    '            Case 4
    '                AppendContamination(1, "DISW", 1)
    '            Case 5
    '                AppendContamination(4, "DISW", 1)
    '        End Select
    '    End Sub

    '    Private Shared Function CreateDispensingContaminationDescription(contaminatedReagent As Integer, washingStrength As Integer, washingCode As String) As IDispensingContaminationDescription
    '        Dim dispensingContaminationDescription = Mock.Create(Of IDispensingContaminationDescription)()
    '        dispensingContaminationDescription.ContaminedReagent = contaminatedReagent
    '        dispensingContaminationDescription.RequiredWashing = CreateWashingDescription(washingStrength, washingCode)
    '        Return dispensingContaminationDescription
    '    End Function

    '    Private Shared Function CreateWashingDescription(Strength As Integer, WSCode As String) As IWashingDescription
    '        Dim washingDesc = Mock.Create(Of IWashingDescription)()
    '        washingDesc.WashingStrength = Strength
    '        washingDesc.WashingSolutionCode = WSCode
    '        Return washingDesc
    '    End Function
    'End Class



End Namespace


