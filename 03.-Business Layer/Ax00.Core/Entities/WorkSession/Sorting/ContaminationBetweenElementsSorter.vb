Option Infer On

Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Public Class ContaminationBetweenElementsSorter

    Public Property AddContaminationBetweenGroups As Integer = 0
    Public Property pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)

#Region "Private attributes"
    Dim pPreviousReagentID As List(Of Integer)
#End Region

    Sub New(ByVal pPreviousReagentID As List(Of Integer), ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow))
        Me.pPreviousReagentID = pPreviousReagentID
        Me.pExecutions = pExecutions
    End Sub

    Public ReadOnly Property ContaminationsSpecification As IAnalyzerContaminationsSpecification
        Get
            Return WSExecutionCreator.Instance.ContaminationsSpecification
        End Get
    End Property


    Public Sub MoveToAvoidContaminationBetweenElements()

        'Method constraints:
        If pPreviousReagentID.Any = False Then Return
        Dim myDistinctOrderTestsList = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions Select a.OrderTestID Distinct)
        If myDistinctOrderTestsList.Any = False Then Return

        'body:

        Dim context As New Context(ContaminationsSpecification)
        context.FillEmptyContextSteps()

        For i As Integer = 1 To ContaminationsSpecification.HighContaminationPersistence
            Dim listIndex = pPreviousReagentID.Count - i
            If listIndex < 0 Then Continue For
            Dim disp = ContaminationsSpecification.CreateDispensing()
            disp.R1ReagentID = pPreviousReagentID(listIndex)

            context.Steps(-i)(1) = disp
        Next

        Dim result = context.ActionRequiredForAGivenDispensing(pExecutions(0))

        If result.Action = IContaminationsAction.RequiredAction.GoAhead Then
            AddContaminationBetweenGroups = 0
            Return
        ElseIf result.Action = IContaminationsAction.RequiredAction.Wash Then
            AddContaminationBetweenGroups += result.InvolvedWashes.Count

        ElseIf result.Action = IContaminationsAction.RequiredAction.Skip Then
            AddContaminationBetweenGroups = 1 '??
            Return 'result.InvolvedWashes.Any
        End If

    End Sub
End Class
