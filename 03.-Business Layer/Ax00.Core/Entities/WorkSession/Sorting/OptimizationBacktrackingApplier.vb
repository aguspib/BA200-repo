Option Explicit On
Option Strict On
Option Infer On
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Friend Class OptimizationBacktrackingApplier : Inherits OptimizationPolicyApplier

        Private bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        'Private lastBireactiveID As New List(Of Integer)
        Private ContaminLimit As Integer

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal pConn As SqlConnection)
            MyBase.New(pConn)
        End Sub
        Dim PreviousReagentID As List(Of Integer)

        Protected Overrides Sub ExecuteOptimizationAlgorithm(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)

            PreviousReagentID = pPreviousReagentID
            MyBase.ExecuteOptimizationAlgorithm(pContaminationsDS, pExecutions, pHighContaminationPersistance, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

            bestResult = (From a In pExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList()

            BacktrackingOptimization(bestResult)

            sortedOTList = (From a In bestResult Select a.OrderTestID Distinct).ToList

        End Sub

 
        Private Const InitialCallStackDepth As Integer = -1
        Dim _callStackNestingLevel As Integer = InitialCallStackDepth

        Private Sub BacktrackingOptimization(ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            Dim solutionSet As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim Tests = OrderTests.ToList()
            Dim result As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentContaminationNumber = GetContaminationNumber(Nothing, OrderTests)
            'currentContaminationNumber = ExecutionsDelegate.GetContaminationNumber(ContaminDS, OrderTests, HighContaminationPersistence)

            If currentContaminationNumber > 0 Then
                ContaminLimit = 0

                While (result.Count = 0 AndAlso ContaminLimit < currentContaminationNumber)
                    _callStackNestingLevel = -1
                    foundSolution = False
                    result = BacktrackingAlgorithm(Tests, solutionSet)
                    If result.Count = 0 Then
                        ContaminLimit += 1
                    End If
                End While
            End If

            If result.Count = 0 Then
                bestResult = OrderTests
            Else
                bestResult = result
            End If
        End Sub

        Public Overrides Function GetContaminationNumber(ByVal pContaminationsDS As ContaminationsDS, ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow)) As Integer
            Return WSExecutionCreator.Instance.GetContaminationNumber(calculateInRunning, PreviousReagentID, orderTests)
        End Function

        Private foundSolution As Boolean = False

        Private Function BacktrackingAlgorithm(ByVal Tests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow), ByRef solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As List(Of ExecutionsDS.twksWSExecutionsRow)
            _callStackNestingLevel += 1

            Dim LastKnownNOTSolution As ExecutionsDS.twksWSExecutionsRow = Nothing

            For i As Integer = 0 To Tests.Count - 1
                Dim elem = Tests(i)

                If LastKnownNOTSolution IsNot Nothing AndAlso IsReplicate(LastKnownNOTSolution, elem) Then
                    Continue For
                End If

                If IsViable(solutionSet, elem) Then

                    Dim auxTests = Tests.ToList()
                    auxTests.Remove(elem)
                    solutionSet.Add(elem)

                    If IsSolution(solutionSet) Then
                        foundSolution = True
                        Return solutionSet

                    Else
                        Dim RecursiveSubSetInOrder = GetConsecutiveReplicatesList(Tests, i, elem, auxTests)
                        BacktrackingAlgorithm(RecursiveSubSetInOrder, solutionSet)
                    End If

                    If foundSolution Then
                        Exit For
                    Else
                        solutionSet.RemoveAt(solutionSet.Count - 1)
                        LastKnownNOTSolution = elem

                    End If

                Else

                    LastKnownNOTSolution = elem
                    Continue For
                End If
            Next
            _callStackNestingLevel -= 1

            Return solutionSet
        End Function

        Private Function GetConsecutiveReplicatesList(ByVal Tests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow), ByVal TestElementIndex As Integer,
                                                      ByVal elem As ExecutionsDS.twksWSExecutionsRow,
                                                      ByVal auxTests As List(Of ExecutionsDS.twksWSExecutionsRow)) As LinkedList(Of ExecutionsDS.twksWSExecutionsRow)

            Dim OrderedSubSet As New LinkedList(Of ExecutionsDS.twksWSExecutionsRow)

            Dim done = False
            Dim myIterator = TestElementIndex + 1

            '1.- Remove consecutive elements that are replicates
            While myIterator < Tests.Count And done = False
                If IsReplicate(Tests(myIterator), elem) Then
                    OrderedSubSet.AddLast(Tests(myIterator))
                    auxTests.Remove(Tests(myIterator))
                Else
                    done = True
                End If
                myIterator += 1
            End While

            '2.- Append pending elements
            For Each item In auxTests
                OrderedSubSet.AddLast(New LinkedListNode(Of ExecutionsDS.twksWSExecutionsRow)(item))
            Next

            Return OrderedSubSet
        End Function

        Private Function IsReplicate(execution1 As ExecutionsDS.twksWSExecutionsRow, execution2 As ExecutionsDS.twksWSExecutionsRow) As Boolean
            If execution1 Is Nothing OrElse execution2 Is Nothing Then Return False
            If (execution1.IsOrderTestIDNull <> execution2.IsOrderTestIDNull) Then Return False
            If execution1.IsOrderTestIDNull = False AndAlso execution1.OrderTestID <> execution2.OrderTestID Then Return False

            Return True


        End Function

        Private Function IsViable(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal elem As ExecutionsDS.twksWSExecutionsRow) As Boolean

            If solutionSet IsNot Nothing AndAlso solutionSet.Any AndAlso IsReplicate(solutionSet.Last(), elem) Then
                Return True

            Else
                Dim aux = solutionSet.ToList()
                aux.Add(elem)
                Return (GetCurrentContaminationNumberInBacktracking(aux, ContaminationsSpecification.HighContaminationPersistence) <= ContaminLimit)
            End If

        End Function

        Private Function IsSolution(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (solutionSet.Count >= bestResult.Count)
        End Function

        Private Function GetCurrentContaminationNumberInBacktracking(ByVal pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), Optional ByVal pHighContaminationPersistance As Integer = 0) As Integer
            'As a solution set, this doesn't have to contain any contamination. So, only it's needed to check contaminations between:
            ' - the last reagent and the newly inserted
            ' - the last pHighContaminationPersistence reagents and the newly inserted
            ' - if the newly is bi-reactive, check if there's contamination between it and the previous bi-reactive

            Dim context = New Context(ContaminationsSpecification)

            context.FillContextInStatic(pExecutions.ToList)

            If calculateInRunning Then
                Dim runningContext = ContaminationsSpecification.CurrentRunningContext
                Dim range = runningContext.Steps.Range
                For stepIndex = range.Minimum To range.Maximum
                    Dim newIndex = stepIndex - _callStackNestingLevel
                    If newIndex < range.Minimum Then Continue For

                    If runningContext.Steps(stepIndex) IsNot Nothing Then
                        For dispenseNum = 1 To ContaminationsSpecification.DispensesPerStep
                            If context.Steps(newIndex)(dispenseNum) IsNot Nothing AndAlso
                                runningContext.Steps(stepIndex)(dispenseNum) IsNot Nothing Then
                                'MERGE:
                                context.Steps(newIndex)(dispenseNum) = runningContext.Steps(stepIndex)(dispenseNum)
                            End If


                        Next
                    End If

                Next
            Else
                context.FillContentsFromReagentsIDInStatic(Me.PreviousReagentID, _callStackNestingLevel)
            End If


            Dim result = context.ActionRequiredForDispensing(context.Steps(0).Dispensing(1))    'Steps(0) is current step (not before, neither after)

            Dim washingsList = result.InvolvedWashes

            Dim contam = washingsList.Count

            Return contam


        End Function
    End Class
End Namespace