Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class OptimizationBacktrackingApplier : Inherits OptimizationPolicyApplier

        Private bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)

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
            Dim SolutionSet As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim Tests = OrderTests.ToList()

            Dim result = BacktrackingAlgorithm(0, Tests, SolutionSet)
            'currentContaminationNumber = New ExecutionsDelegate().GetContaminationNumber(ContDS, result, highContaminationPersistance)
            bestResult = result
        End Sub

        Private Function BacktrackingAlgorithm(ByVal Level As Integer, ByVal Tests As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef SolutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As List(Of ExecutionsDS.twksWSExecutionsRow)
            For Each elem In Tests
                If IsViable(SolutionSet, elem) Then
                    Dim auxTests = Tests.ToList()
                    auxTests.Remove(elem)
                    If Level = SolutionSet.Count Then
                        SolutionSet.Add(elem)
                    Else
                        SolutionSet.Item(Level) = elem
                    End If
                    If IsSolution(SolutionSet) Then
                        Return SolutionSet
                    End If
                    If auxTests.Count > 0 Then
                        BacktrackingAlgorithm(Level + 1, auxTests, SolutionSet)
                    End If
                    If IsSolution(SolutionSet) Then
                        Exit For
                    Else
                        SolutionSet.Remove(elem)
                    End If
                End If
            Next
            Return SolutionSet
        End Function

        Private Function IsViable(ByVal SolutionSet As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal elem As ExecutionsDS.twksWSExecutionsRow) As Boolean
            Dim aux = SolutionSet.ToList()
            aux.Add(elem)

            Return (New ExecutionsDelegate().GetContaminationNumber(ContaminDS, aux, HighContaminationPersistence) = 0)
        End Function

        Private Function IsSolution(ByVal SolutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (SolutionSet.Count = bestResult.Count AndAlso New ExecutionsDelegate().GetContaminationNumber(ContaminDS, SolutionSet, HighContaminationPersistence) = 0)
        End Function

    End Class
End Namespace