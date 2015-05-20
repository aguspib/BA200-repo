Option Infer On

Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Types

Public Class ContaminationBetweenElementsSorter

    Public Property OriginalOrderChanged As Boolean = False
    Public Property AddContaminationBetweenGroups As Integer = 0
    Public Property pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)

#Region "Private attributes"
    Dim pContaminationsDS As ContaminationsDS
    Dim pPreviousReagentID As List(Of Integer)
    Dim pPreviousReplicatesNumber As List(Of Integer)
#End Region

    Sub New(ByVal pContaminationsDS As ContaminationsDS, ByVal pPreviousReagentID As List(Of Integer), ByVal pPreviousReplicatesNumber As List(Of Integer), ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow))
        Me.pContaminationsDS = pContaminationsDS
        Me.pPreviousReagentID = pPreviousReagentID
        Me.pPreviousReplicatesNumber = pPreviousReplicatesNumber
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

        'New "are there contaminations? detection"

        Dim context As New ContaminationsContext(ContaminationsSpecification)
        context.FillEmptyContext()

        'context.FillContextInStatic(pExecutions)
        For i As Integer = 1 To ContaminationsSpecification.HighContaminationPersistence
            Dim listIndex = pPreviousReagentID.Count - i
            If listIndex < 0 Then Continue For
            Dim disp As New Dispensing() With {.R1ReagentID = pPreviousReagentID(i)}
            'El elemento 1 es el Before 1, el 2 el Before 2, por eso se ponen en signo negativo en el contexto:
            context.Steps(-i)(1) = disp
        Next

        Dim result = context.ActionRequiredForAGivenDispensing(pExecutions(0))

        If result.Action = IContaminationsAction.RequiredAction.NoAction Then
            AddContaminationBetweenGroups = 0
            Return
        ElseIf result.Action = IContaminationsAction.RequiredAction.Wash Then
            AddContaminationBetweenGroups += result.InvolvedWashes.Count
            'Baacktracking to the rescue!!
        ElseIf result.Action = IContaminationsAction.RequiredAction.Skip Then
            AddContaminationBetweenGroups = 1 '??
            Return 'result.InvolvedWashes.Any
        End If

        '/New

        'Dim ReagentContaminatorID = pPreviousReagentID(pPreviousReagentID.Count - 1) 'Last Reagent in previous element group (reverse order)
        ''Dim ReagentContaminatedID As Integer = -1
        'Dim myExecLinqByOT As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)
        'Dim contaminations As EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow) = Nothing

        'If myDistinctOrderTestsList.Count > 1 Then
        '    Dim itera As Integer = 0
        '    Dim insertPosition As Integer = 0

        '    '1) Search test for FIRST POSITION: Evaluate contaminations between:
        '    'LastReagent(Last) -> Next Reagent(0)
        '    'LastReagent(Last-1) -> Next Reagent(0) (special)
        '    'If contamination ... search one test not contaminated to be place in FIRST position
        '    For Each myOrderTest As Integer In myDistinctOrderTestsList
        '        myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" Select a) '.ToList

        '        If myExecLinqByOT.Any Then
        '            Dim ReagentContaminatedID = myExecLinqByOT(0).ReagentID
        '            For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - ContaminationsSpecification.HighContaminationPersistence Step -1
        '                If jj >= 0 Then
        '                    ReagentContaminatorID = pPreviousReagentID(jj)

        '                    If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
        '                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
        '                    Else 'search for contamination (only high level)
        '                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
        '                    End If

        '                    If contaminations.Count > 0 Then Exit For

        '                    'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
        '                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
        '                    If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= ContaminationsSpecification.HighContaminationPersistence Then
        '                        Exit For 'Do not evaluate high contamination persistance
        '                    End If

        '                Else
        '                    Exit For
        '                End If
        '            Next

        '            If contaminations.Count = 0 Then
        '                AddContaminationBetweenGroups = 0
        '                If itera > 0 Then
        '                    'New FIRST position test executions
        '                    For Each currentExecution In myExecLinqByOT
        '                        pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
        '                        pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
        '                        insertPosition += 1
        '                    Next
        '                    OriginalOrderChanged = True
        '                End If
        '                Exit For 'For Each myOrderTest As ....

        '            Else
        '                AddContaminationBetweenGroups = 1
        '            End If
        '            'contaminations = Nothing

        '        Else
        '            AddContaminationBetweenGroups = 1
        '        End If
        '        itera += 1
        '    Next

        '    If OriginalOrderChanged Then
        '        '2) Search test for SECOND POSITION: Evaluate contaminations between:
        '        'Next Reagent(0) -> Next Reagent(1) 
        '        'LastReagent(Last) -> Next Reagent(1) (special)
        '        'If contamination ... search one test not contaminated to be place in SECOND position
        '        itera = 0
        '        Dim firstPositionContaminatorID As Integer = 0
        '        myDistinctOrderTestsList = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
        '                        Select a.OrderTestID Distinct).ToList 'New query over the new sort

        '        For Each myOrderTest As Integer In myDistinctOrderTestsList
        '            myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
        '                              Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" _
        '                              Select a)

        '            'Define the insert position for the SECOND test
        '            If myExecLinqByOT.Any Then
        '                If itera = 0 Then
        '                    insertPosition = myExecLinqByOT.Count
        '                    firstPositionContaminatorID = myExecLinqByOT(0).ReagentID

        '                    'Evaluate only HIGH contamination persistance when OrderTest in FIRST position has MaxReplicates < pHighContaminationPersistance
        '                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
        '                    If myExecLinqByOT.Count >= ContaminationsSpecification.HighContaminationPersistence Then
        '                        Exit For '(do not evaluate high contamination persistance)
        '                    End If
        '                End If

        '                If itera > 0 AndAlso myExecLinqByOT.Any Then
        '                    Dim ReagentContaminatedID = myExecLinqByOT(0).ReagentID

        '                    'search for contamination (low or high level) between FIRST position test (low or high level)
        '                    contaminations = GetContaminationBetweenReagents(firstPositionContaminatorID, ReagentContaminatedID, pContaminationsDS)

        '                    'If no contamination with the FIRST position test then evaluate the HIGH level contamination with the last reagent
        '                    If contaminations.Count = 0 Then
        '                        contaminations = GetHardContaminationBetweenReagents(pPreviousReagentID(pPreviousReagentID.Count - 1), ReagentContaminatedID, pContaminationsDS)
        '                    End If


        '                    If contaminations.Count = 0 Then
        '                        AddContaminationBetweenGroups = 0
        '                        If itera > 1 Then
        '                            'New SECOND position test executions
        '                            For Each currentExecution In myExecLinqByOT
        '                                pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
        '                                pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
        '                                insertPosition += 1
        '                            Next
        '                            OriginalOrderChanged = True
        '                        End If
        '                        Exit For 'For Each myOrderTest As ....

        '                    Else
        '                        AddContaminationBetweenGroups = 1
        '                    End If
        '                    'contaminations = Nothing
        '                End If
        '            End If
        '            itera += 1
        '        Next
        '    End If
        'ElseIf myDistinctOrderTestsList.Count = 1 Then 'If myOTListLinq.Count > 1 Then (only one test, no movement is possible)
        '    myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
        '      Where a.OrderTestID = myDistinctOrderTestsList(0) AndAlso a.ExecutionStatus = "PENDING" _
        '      Select a)

        '    If myExecLinqByOT.Any Then
        '        Dim ReagentContaminatedID = myExecLinqByOT(0).ReagentID

        '        For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - ContaminationsSpecification.HighContaminationPersistence Step -1
        '            If jj >= 0 Then
        '                ReagentContaminatorID = pPreviousReagentID(jj)

        '                If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
        '                    contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)

        '                Else 'search for contamination (only high level)
        '                    contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
        '                End If

        '                If contaminations.Count > 0 Then Exit For

        '                'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
        '                'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
        '                If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= ContaminationsSpecification.HighContaminationPersistence Then
        '                    Exit For 'Do not evaluate high contamination persistance
        '                End If

        '            Else
        '                Exit For
        '            End If
        '        Next
        '        'AG 20/12/2011 

        '        If contaminations.Count > 0 Then
        '            AddContaminationBetweenGroups = 1
        '        End If
        '    End If

        'End If

    End Sub
End Class
