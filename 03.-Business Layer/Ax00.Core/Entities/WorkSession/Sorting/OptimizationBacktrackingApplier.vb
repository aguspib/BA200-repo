Option Explicit On
Option Strict On
Option Infer On
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    ''' <summary>
    ''' Class that implements the backtracking algorithm for sorting the executions
    ''' in a way that there aren't any contamination or, at least, a number of
    ''' contaminations below the initial number resulting from the original sorting.
    ''' As a derived class from OptimizationPolicyApplier, it inherits all the methods
    ''' defined on the father class.
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class OptimizationBacktrackingApplier : Inherits OptimizationPolicyApplier

        ''' <summary>
        ''' List that contains the list of execution with the best sort, with the
        ''' minimum number of contaminations, if any.
        ''' </summary>
        ''' <remarks></remarks>
        Private bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        'Private lastBireactiveID As New List(Of Integer)

        ''' <summary>
        ''' Containt the maximum number of possible contaminations allowed
        ''' for the best result list
        ''' </summary>
        ''' <remarks></remarks>
        Private ContaminLimit As Integer

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="pConn"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal pConn As SqlConnection)
            MyBase.New(pConn)
        End Sub

        ''' <summary>
        ''' List with the IDs of the previous reagents sent to the analyzer
        ''' </summary>
        ''' <remarks></remarks>
        Dim PreviousReagentID As List(Of Integer)

        ''' <summary>
        ''' Sets the environment for calling the backtracking algorithm, and calls it
        ''' </summary>
        ''' <param name="pContaminationsDS">List of all possible contaminations between two reagents</param>
        ''' <param name="pExecutions">List of executions to sort</param>
        ''' <param name="pHighContaminationPersistance">Limit of persistance for high contamination, in cycles.</param>
        ''' <param name="pPreviousReagentID">List of previous reagents sent to the analyzer</param>
        ''' <param name="pPreviousReagentIDMaxReplicates">List of number of maximum replicates for the previous reagents sent to the analyzer</param>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' Variable that controls the depth level of recursivity, inside the backtracking algorithm
        ''' </summary>
        ''' <remarks></remarks>
        Dim _callStackNestingLevel As Integer = InitialCallStackDepth

        ''' <summary>
        ''' Calls the backtracking algorithm for sorting the executions given as parameter
        ''' </summary>
        ''' <param name="OrderTests">List of executions to sort</param>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' Function with calculates the contamination number between all the reagents given in the execution list
        ''' </summary>
        ''' <param name="pContaminationsDS">List of all possible contaminations between two reagents</param>
        ''' <param name="orderTests">List of executions to calculate contaminations</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetContaminationNumber(ByVal pContaminationsDS As ContaminationsDS, ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow)) As Integer
            Return WSExecutionCreator.Instance.GetContaminationNumber(calculateInRunning, PreviousReagentID, orderTests)
        End Function

        ''' <summary>
        ''' Flag that indicates if a solution inside the backtracking algorithm is found.
        ''' </summary>
        ''' <remarks></remarks>
        Private foundSolution As Boolean = False

        ''' <summary>
        ''' Backtracking recursive algorithm 
        ''' </summary>
        ''' <param name="Tests"></param>
        ''' <param name="solutionSet"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' Returns all the replicates from the same test put together, and ordered in the linked list
        ''' </summary>
        ''' <param name="Tests">Reference list of executions</param>
        ''' <param name="TestElementIndex">position of the current execution</param>
        ''' <param name="elem">current execution to check if it has more replicates</param>
        ''' <param name="auxTests">List of executions rearrange</param>
        ''' <returns>The ordered list with the replicates of "elem" put together</returns>
        ''' <remarks></remarks>
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


        ''' <summary>
        ''' Returns a boolean value about if execution1 and execution2 are replicates for the same test
        ''' </summary>
        ''' <param name="execution1">first execution to test</param>
        ''' <param name="execution2">second execution to test</param>
        ''' <returns>boolean value about if execution1 and execution2 are replicates for the same test</returns>
        ''' <remarks></remarks>
        Private Function IsReplicate(execution1 As ExecutionsDS.twksWSExecutionsRow, execution2 As ExecutionsDS.twksWSExecutionsRow) As Boolean
            If execution1 Is Nothing OrElse execution2 Is Nothing Then Return False
            If (execution1.IsOrderTestIDNull <> execution2.IsOrderTestIDNull) Then Return False
            If execution1.IsOrderTestIDNull = False AndAlso execution1.OrderTestID <> execution2.OrderTestID Then Return False

            Return True


        End Function

        ''' <summary>
        ''' Function that returns a boolean value about if the current solutionSet + elem is a viable solution for the backtracking algorithm
        ''' </summary>
        ''' <param name="solutionSet">Current viable set of elements that are part of the solution</param>
        ''' <param name="elem">a new element to take into account to the solution set, if viable</param>
        ''' <returns>A boolean value about if the current solutionSet + elem is a viable solution for the backtracking algorithm</returns>
        ''' <remarks>
        ''' This function is the key for a good implementation for the backtracking algorithm, because it decides if a given set of elements
        ''' is a viable solution for the problem. This function needs to be as optimum as possible, because the efficiency for the algorithm
        ''' is based mainly onto this function.
        ''' </remarks>
        Private Function IsViable(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal elem As ExecutionsDS.twksWSExecutionsRow) As Boolean

            If solutionSet IsNot Nothing AndAlso solutionSet.Any AndAlso IsReplicate(solutionSet.Last(), elem) Then
                Return True

            Else
                Dim aux = solutionSet.ToList()
                aux.Add(elem)
                Return (GetCurrentContaminationNumberInBacktracking(aux, ContaminationsSpecification.HighContaminationPersistence) <= ContaminLimit)
            End If

        End Function

        ''' <summary>
        ''' Returns a boolean value about if the current solutionSet is the solution for the backtracking algorithm
        ''' </summary>
        ''' <param name="solutionSet">The set of elements that makes de solution</param>
        ''' <returns>A boolean value about if the current solutionSet is the solution for the backtracking algorithm</returns>
        ''' <remarks>
        ''' Because this function is called after checking if the solutionSet is viable, all of its elements are part of a viable solution.
        ''' Thus, the only thing to check here is if all the element from the initial sort are included into the solutionSet.
        ''' </remarks>
        Private Function IsSolution(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (solutionSet.Count = bestResult.Count)
        End Function

        ''' <summary>
        ''' Returns the number of existing contaminations in a list of executions, having in count the persistence for the contaminations (optional).
        ''' </summary>
        ''' <param name="pExecutions">List of executions to check their contaminations</param>
        ''' <param name="pHighContaminationPersistance">Persistence for the contamination, in cycles</param>
        ''' <returns>The number of existing contaminations in the list of executions, keeping in mind the persistence for the contaminations (optional)</returns>
        ''' <remarks></remarks>
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