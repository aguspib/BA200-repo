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
        Private lastBireactiveID As New List(Of Integer)
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

            ContaminLimit = 0

            While (result.Count = 0 AndAlso ContaminLimit < currentContaminationNumber)
                _callStackNestingLevel = -1
                result = BacktrackingAlgorithm(Tests, solutionSet)
                If result.Count = 0 Then
                    ContaminLimit += 1
                End If
            End While

            If result.Count = 0 Then
                bestResult = OrderTests
            Else
                bestResult = result
            End If
        End Sub

        Public Overrides Function GetContaminationNumber(ByVal pContaminationsDS As ContaminationsDS, ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow)) As Integer
            Dim contaminaNumber As Integer = 0
            Dim auxContext = New ContaminationsContext(ContaminationsSpecification)
            auxContext.Steps.Clear()
            If Not calculateInRunning AndAlso Me.PreviousReagentID IsNot Nothing AndAlso PreviousReagentID.Any Then
                'Iterate throug last "persistence" elements of PreviousreagentID:
                For i As Integer = Math.Max(0, Me.PreviousReagentID.Count - ContaminationsSpecification.HighContaminationPersistence) To Me.PreviousReagentID.Count - 1
                    Dim curStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)
                    curStep(1) = ContaminationsSpecification.CreateDispensing()
                    curStep(1).R1ReagentID = PreviousReagentID(i)
                    auxContext.Steps.Append(curStep)
                Next
            ElseIf Not calculateInRunning Then
                For i = auxContext.Steps.Range.Minimum To -1
                    Dim curStep = New ContextStep(ContaminationsSpecification.DispensesPerStep)
                    curStep(1) = ContaminationsSpecification.CreateDispensing()
                    curStep(1).KindOfLiquid = IDispensing.KindOfDispensedLiquid.Dummy
                    auxContext.Steps.Append(curStep)
                    'dispense.f()
                Next
            Else
                'Get contents from current REAL context
            End If

            For i As Integer = 0 To orderTests.Count + auxContext.Steps.Range.Maximum '- 1
                Dim myStep As New ContextStep(ContaminationsSpecification.DispensesPerStep)
                Dim dispense = ContaminationsSpecification.CreateDispensing()
                If i < orderTests.Count Then
                    dispense.R1ReagentID = orderTests(i).ReagentID
                Else
                    dispense.KindOfLiquid = IDispensing.KindOfDispensedLiquid.Dummy
                End If
                myStep(1) = dispense
                auxContext.Steps.Append(myStep)
                If auxContext.Steps.IsIndexValid(0) AndAlso auxContext.Steps(0) IsNot Nothing AndAlso auxContext.Steps(0)(1) IsNot Nothing Then
                    Dim result = auxContext.ActionRequiredForDispensing(auxContext.Steps(0)(1))
                    Select Case result.Action
                        Case IContaminationsAction.RequiredAction.Wash
                            contaminaNumber += 1
                        Case IContaminationsAction.RequiredAction.NoAction, IContaminationsAction.RequiredAction.RemoveRequiredWashing, IContaminationsAction.RequiredAction.Skip
                            'Do nothing
                    End Select
                Else
                    Exit For
                End If
                auxContext.Steps.RemoveFirst()
            Next
            Return contaminaNumber
        End Function

        Private Function BacktrackingAlgorithm(ByVal Tests As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As List(Of ExecutionsDS.twksWSExecutionsRow)
            _callStackNestingLevel += 1
            Dim additionalNestingLevel As Integer = 0
            Static foundSolution As Boolean = False
            For Each elem In Tests
                If additionalNestingLevel > 0 Then
                    additionalNestingLevel -= 1
                    Continue For
                End If
                If IsViable(solutionSet, elem) Then
                    Dim auxTests = Tests.ToList()
                    auxTests.Remove(elem)
                    If _callStackNestingLevel = solutionSet.Count Then
                        solutionSet.Add(elem)

                    Else
                        solutionSet.Item(_callStackNestingLevel) = elem
                    End If

                    While auxTests.Any AndAlso
                        (auxTests(0).ReagentID = elem.ReagentID) AndAlso
                        (auxTests(0).SampleClass = elem.SampleClass) AndAlso
                        (auxTests(0).SampleType = elem.SampleType) AndAlso
                        (
                            Not (Not auxTests(0).IsPatientIDNull AndAlso Not elem.IsPatientIDNull) OrElse auxTests(0).PatientID = elem.PatientID
                        )

                        solutionSet.Add(auxTests(0))
                        auxTests.Remove(auxTests(0))
                        additionalNestingLevel += 1
                        _callStackNestingLevel += 1
                    End While




                    If IsSolution(solutionSet) Then
                        foundSolution = True
                        Return solutionSet
                    End If

                    If auxTests.Count > 0 Then
                        BacktrackingAlgorithm(auxTests, solutionSet)
                    End If

                    If foundSolution Then
                        Exit For
                    Else
                        solutionSet.Remove(elem)

                        solutionSet.RemoveRange(_callStackNestingLevel - additionalNestingLevel, additionalNestingLevel)
                        lastBireactiveID.Remove(elem.ReagentID)
                        _callStackNestingLevel -= additionalNestingLevel
                        'additionalNestingLevel = 0
                    End If
                End If
            Next
            _callStackNestingLevel -= 1 + additionalNestingLevel

            Return solutionSet
        End Function

        Private Function IsViable(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal elem As ExecutionsDS.twksWSExecutionsRow) As Boolean
            Dim aux = solutionSet.ToList()
            aux.Add(elem)

            Return (GetCurrentContaminationNumberInBacktracking(aux, ContaminationsSpecification.HighContaminationPersistence) <= ContaminLimit)
        End Function

        Private Function IsSolution(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (solutionSet.Count = bestResult.Count)
        End Function

        Private Function GetCurrentContaminationNumberInBacktracking(ByVal pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), Optional ByVal pHighContaminationPersistance As Integer = 0) As Integer
            'As a solution set, this doesn't have to contain any contamination. So, only it's needed to check contaminations between:
            ' - the last reagent and the newly inserted
            ' - the last pHighContaminationPersistence reagents and the newly inserted
            ' - if the newly is bi-reactive, check if there's contamination between it and the previous bi-reactive

            Dim context = New ContaminationsContext(ContaminationsSpecification)

            Dim newList = pExecutions.ToList    'Pending excutions list
            context.FillContextInStatic(newList)

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

            Dim currentReagent = pExecutions.Count - 1
            Dim contaminatorType As AnalysisMode

            If contam = 0 Then

                contaminatorType = ContaminationsSpecification.GetAnalysisModeForReagent(pExecutions(currentReagent).ReagentID)  'GetAnalysisModeInTest(dbConnection, pExecutions(currentReagent).ReagentID)
                If contaminatorType = AnalysisMode.BiReactive Then
                    If lastBireactiveID.Count > 0 Then
                        contam = GetContaminationBetweenReagents(lastBireactiveID.Item(lastBireactiveID.Count - 1), pExecutions(currentReagent).ReagentID, ContaminDS).Count
                    End If
                    If contam = 0 Then
                        lastBireactiveID.Add(pExecutions(currentReagent).ReagentID)
                    End If
                End If
            End If
            If contam = 0 AndAlso contaminatorType = AnalysisMode.BiReactive Then
                For j = lastBireactiveID.Count - 1 To lastBireactiveID.Count - (1 + pHighContaminationPersistance) Step -1
                    If (j >= 0) Then
                        contam = GetHardContaminationBetweenReagents(lastBireactiveID.Item(j), pExecutions(currentReagent).ReagentID, ContaminDS).Count
                        If contam > 0 Then
                            contam += 1
                        End If
                    End If
                Next
            End If

            Return contam


        End Function
    End Class
End Namespace