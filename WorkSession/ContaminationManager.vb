Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports System.Net.Configuration
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL

    Public Class ContaminationManager
        Public Property currentContaminationNumber As Integer
        Public Property bestContaminationNumber As Integer
        Public Property bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected Property currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected Property Conn As SqlConnection
        Protected Property ActiveAnalyzer As String
        Protected Property highContaminationPersistance As Integer
        Protected Property ContDS As ContaminationsDS
        Protected Property previousReagentID As List(Of Integer)
        Protected Property previousReagentIDMaxReplicates As List(Of Integer)

        Public Sub New(ByVal pCon As SqlConnection,
                       ByVal Analyzer As String,
                       ByVal currentCont As Integer,
                       ByVal pHighCont As Integer,
                       ByVal contaminsDS As ContaminationsDS,
                       ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                       Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                       Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            Conn = pCon
            ActiveAnalyzer = Analyzer
            currentContaminationNumber = currentCont
            highContaminationPersistance = pHighCont
            ContDS = contaminsDS
            previousReagentID = pPreviousReagentID
            previousReagentIDMaxReplicates = pPreviousReagentIDMaxReplicates
            bestContaminationNumber = Integer.MaxValue
            bestResult = OrderTests.ToList()
        End Sub

        Public Sub ApplyOptimizations(ByVal myOptimizer As OptimizationPolicyApplier, ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            If currentContaminationNumber > 0 Then
                currentResult = OrderTests.ToList()
                currentContaminationNumber = myOptimizer.ExecuteOptimization(ContDS, currentResult, highContaminationPersistance, previousReagentID, previousReagentIDMaxReplicates)
                If currentContaminationNumber < bestContaminationNumber Then
                    bestContaminationNumber = currentContaminationNumber
                    bestResult = currentResult
                End If
            End If
        End Sub

        Public Sub BacktrackingOptimization(ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            Dim SolutionSet As New List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim Tests = OrderTests.ToList()

            Dim result = BacktrackingAlgorithm(0, Tests, SolutionSet)
            currentContaminationNumber = New ExecutionsDelegate().GetContaminationNumber(ContDS, result, highContaminationPersistance)
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

            Return (New ExecutionsDelegate().GetContaminationNumber(ContDS, aux, highContaminationPersistance) = 0)
        End Function

        Private Function IsSolution(ByVal SolutionSet As List(Of ExecutionsDS.twksWSExecutionsRow)) As Boolean
            Return (SolutionSet.Count = bestResult.Count AndAlso New ExecutionsDelegate().GetContaminationNumber(ContDS, SolutionSet, highContaminationPersistance) = 0)
        End Function
    End Class
End Namespace