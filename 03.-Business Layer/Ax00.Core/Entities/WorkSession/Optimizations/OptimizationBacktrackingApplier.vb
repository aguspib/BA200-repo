Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Public Class OptimizationBacktrackingApplier : Inherits OptimizationPolicyApplier

        Private bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Private lastBireactiveID As New List(Of Integer)
        Private ContaminLimit As Integer

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal pConn As SqlConnection, ByVal ActiveAnalyzer As String)
            MyBase.New(pConn, ActiveAnalyzer)
        End Sub

        Protected Overrides Sub Execute_i_loop(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)

            MyBase.Execute_i_loop(pContaminationsDS, pExecutions, pHighContaminationPersistance, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

            bestResult = (From a In pExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList()

            BacktrackingOptimization(bestResult)

            sortedOTList = (From a In bestResult Select a.OrderTestID Distinct).ToList

        End Sub

        Protected Overrides Sub Execute_j_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal indexI As Integer, _
                                                  ByVal lowerLimit As Integer, _
                                                  ByVal upperLimit As Integer)

            MyBase.Execute_j_loop(pExecutions, indexI, lowerLimit, upperLimit)
        End Sub

        Protected Overrides Sub Execute_jj_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                             ByVal indexJ As Integer, _
                                             ByVal leftLimit As Integer, _
                                             ByVal rightLimit As Integer, _
                                             Optional ByVal upperTotalLimit As Integer = 0)

            MyBase.Execute_jj_loop(pExecutions, indexJ, leftLimit, rightLimit)


        End Sub

        Private Sub BacktrackingOptimization(ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            Dim solutionSet As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim Tests = OrderTests.ToList()
            Dim result As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim currentContaminationNumber = ExecutionsDelegate.GetContaminationNumber(ContaminDS, OrderTests, HighContaminationPersistence)

            ContaminLimit = 0

            While (result.Count = 0 AndAlso ContaminLimit < currentContaminationNumber)
                result = BacktrackingAlgorithm(0, Tests, solutionSet)
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

        Private Function BacktrackingAlgorithm(ByVal Level As Integer, ByVal Tests As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As List(Of ExecutionsDS.twksWSExecutionsRow)
            Static foundSolution As Boolean = False
            For Each elem In Tests
                If IsViable(solutionSet, elem) Then
                    Dim auxTests = Tests.ToList()
                    auxTests.Remove(elem)

                    If Level = solutionSet.Count Then
                        solutionSet.Add(elem)
                    Else
                        solutionSet.Item(Level) = elem
                    End If

                    If IsSolution(solutionSet) Then
                        foundSolution = True
                        Return solutionSet
                    End If

                    If auxTests.Count > 0 Then
                        BacktrackingAlgorithm(Level + 1, auxTests, solutionSet)
                    End If

                    If foundSolution Then
                        Exit For
                    Else
                        solutionSet.Remove(elem)
                        lastBireactiveID.Remove(elem.ReagentID)
                    End If
                End If
            Next
            Return solutionSet
        End Function

        Private Function IsViable(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal elem As ExecutionsDS.twksWSExecutionsRow) As Boolean
            Dim aux = solutionSet.ToList()
            aux.Add(elem)

            Return (GetContaminationNumberWithBireagents(aux, HighContaminationPersistence) <= ContaminLimit)
        End Function

        Private Function IsSolution(ByVal solutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (solutionSet.Count = bestResult.Count)
        End Function

        Private Function GetContaminationNumberWithBireagents(ByVal pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), Optional ByVal pHighContaminationPersistance As Integer = 0) As Integer
            'As a solution set, this doesn't have to contain any contamination. So, only it's needed to check contaminations between:
            ' - the last reagent and the newly inserted
            ' - the last pHighContaminationPersistence reagents and the newly inserted
            ' - if the newly is bi-reactive, check if there's contamination between it and the previous bi-reactive

            Dim contamNumber As Integer = 0
            Dim previousReagent = pExecutions.Count - 2
            Dim currentReagent = pExecutions.Count - 1
            Dim contaminatorType As AnalysisMode

            If previousReagent >= 0 AndAlso currentReagent >= 0 Then
                Dim contam = (From wse In ContaminDS.tparContaminations _
                                          Where wse.ReagentContaminatorID = pExecutions(previousReagent).ReagentID _
                                            AndAlso wse.ReagentContaminatedID = pExecutions(currentReagent).ReagentID _
                                            AndAlso pExecutions(currentReagent).ExecutionStatus = "PENDING" _
                                            AndAlso pExecutions(currentReagent).ExecutionType = "PREP_STD" _
                                            AndAlso pExecutions(previousReagent).ExecutionStatus = "PENDING" _
                                            AndAlso pExecutions(previousReagent).ExecutionType = "PREP_STD" _
                                          Select wse).ToList()

                If contam.Count = 0 Then
                    contaminatorType = GetTypeReagentInTest(dbConnection, pExecutions(currentReagent).ReagentID)
                    If contaminatorType = AnalysisMode.BiReactive Then
                        If lastBireactiveID.Count > 0 Then
                            contam = GetContaminationBetweenReagents(lastBireactiveID.Item(lastBireactiveID.Count - 1), pExecutions(currentReagent).ReagentID, ContaminDS)
                        End If
                        If contam.Count = 0 Then
                            lastBireactiveID.Add(pExecutions(currentReagent).ReagentID)
                        End If
                    End If
                End If

                If contam.Count > 0 Then
                    contamNumber += 1
                ElseIf pHighContaminationPersistance > 0 Then
                    For j = previousReagent To previousReagent - pHighContaminationPersistance Step -1
                        Dim auxj = j
                        If (j >= 0) Then
                            contam = (From wse In ContaminDS.tparContaminations _
                                                  Where wse.ReagentContaminatorID = pExecutions(auxj).ReagentID _
                                                  AndAlso wse.ReagentContaminatedID = pExecutions(currentReagent).ReagentID _
                                                  AndAlso Not wse.IsWashingSolutionR1Null _
                                                  AndAlso pExecutions(auxj).ExecutionStatus = "PENDING" _
                                                  AndAlso pExecutions(auxj).ExecutionType = "PREP_STD" _
                                                  AndAlso pExecutions(currentReagent).ExecutionStatus = "PENDING" _
                                                  AndAlso pExecutions(currentReagent).ExecutionType = "PREP_STD" _
                                                  Select wse).ToList()

                            If contam.Count > 0 Then
                                contamNumber += 1
                            End If
                        End If
                    Next
                    If contam.Count = 0 AndAlso contaminatorType = AnalysisMode.BiReactive Then
                        For j = lastBireactiveID.Count - 1 To lastBireactiveID.Count - (1 + pHighContaminationPersistance) Step -1
                            If (j >= 0) Then
                                contam = GetHardContaminationBetweenReagents(lastBireactiveID.Item(j), pExecutions(currentReagent).ReagentID, ContaminDS)
                                If contam.Count > 0 Then
                                    contamNumber += 1
                                End If
                            End If
                        Next
                    End If
                End If
            Else
                contaminatorType = GetTypeReagentInTest(dbConnection, pExecutions(currentReagent).ReagentID)
                If contaminatorType = AnalysisMode.BiReactive Then
                    lastBireactiveID.Add(pExecutions(currentReagent).ReagentID)
                End If
            End If

            Return contamNumber
        End Function
    End Class
End Namespace