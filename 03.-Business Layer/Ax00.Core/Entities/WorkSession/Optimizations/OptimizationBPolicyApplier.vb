Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Public Class OptimizationBPolicyApplier : Inherits OptimizationPolicyApplier

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal pConn As SqlConnection) ', ByVal ActiveAnalyzer As String)
            MyBase.New(pConn) ', ActiveAnalyzer)
        End Sub

        Protected Overrides Sub Execute_i_loop(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            MyBase.Execute_i_loop(pContaminationsDS, pExecutions, pHighContaminationPersistance, pPreviousReagentID, pPreviousReagentIDMaxReplicates)
            'Sort the different ordertest inside pExecutions to minimize contaminations (move up the contaminated until becomes not contaminated)
            For i As Integer = 1 To sortedOTList.Count - 1
                ' ReSharper disable once InconsistentNaming
                Dim aux_i = i
                'First contamination to analyze is between OrderTest(i-1) --> OrderTest(i)
                ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(aux_i - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                MainContaminatorID = ReagentContaminatorID

                contaminatedOrderTest = sortedOTList(aux_i)
                ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(aux_i) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)

                'If no contamination between the consecutive tests look for high contamin [If (i-2) contaminates (i)]
                'Only if ordertest(i-1) maxreplicates < hihgcontamination persistance
                If contaminations.Count = 0 Then
                    Dim maxReplicates As Integer = 1
                    maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                     Where a.OrderTestID = sortedOTList(aux_i - 1) Select a.ReplicateNumber).Max
                    If maxReplicates < pHighContaminationPersistance Then
                        If aux_i = 1 AndAlso Not pPreviousReagentID Is Nothing Then
                            contaminations = GetHardContaminationBetweenReagents(pPreviousReagentID(pPreviousReagentID.Count - 1), ReagentContaminatedID, pContaminationsDS)
                        ElseIf aux_i > 1 Then
                            ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                    Where a.OrderTestID = sortedOTList(aux_i - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                            contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                        End If
                    End If
                End If

                'SetExpectedTypeReagent()

                'OrderTest(i-1) contaminates OrderTest(i) ... so try move OrderTes(i) up until becomes no contaminated
                If contaminations.Count > 0 Then
                    'Limit: when pPreviousReagentID <> Nothing the upper limit is 1, otherwise 0
                    Dim upperLimit As Integer = 0
                    If Not pPreviousReagentID Is Nothing Then upperLimit = 1

                    Execute_j_loop(pExecutions, aux_i, (aux_i - 2), upperLimit)
                Else
                    'OK, no contamination between OrderTest(i-1) and OrderTest(i), go to next iteration, analyze contamination between OrderTest(i) and OrderTest(i+1) ...
                End If

                contaminations = Nothing
            Next

        End Sub

        Protected Overrides Sub Execute_j_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal indexI As Integer, _
                                                  ByVal lowerLimit As Integer, _
                                                  ByVal upperLimit As Integer)
            MyBase.Execute_j_loop(pExecutions, indexI, lowerLimit, upperLimit)
            For j As Integer = lowerLimit To upperLimit Step -1
                Dim aux_j = j

                Execute_jj_loop(pExecutions, aux_j, aux_j, (aux_j - HighContaminationPersistence - 1), upperLimit)

                If contaminations.Count = 0 Then
                    'Move orderTest(i) (the contaminated one) after orderTest(j) (where orderTest(i) is not contaminated)

                    'New BAx00 (Ax5 do not implement this business
                    'Only when the OrderTest(i-1) (the MainContaminatorID, and future OrderTest(i)) does not contaminates the OrderTest(i+1)
                    'Simplication: In this point do not take care about High contamination persistance
                    Dim newContaminatedID As Integer
                    If indexI < sortedOTList.Count - 1 Then
                        newContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                Where a.OrderTestID = sortedOTList(indexI + 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                        contaminations = GetContaminationBetweenReagents(MainContaminatorID, newContaminatedID, ContaminDS)
                        'typeResult = GetTypeReagentInTest(dbConnection, newContaminatedID)
                    End If

                    'Before move OrderTest(i) (the Contaminated one, and future OrderTest(j+1)) also be carefull does not contaminates the current OrderTest(j+1) (the future OrderTest(j+2))
                    'Simplication: In this point do not take care about High contamination persistance
                    If contaminations.Count = 0 Then
                        If aux_j < sortedOTList.Count - 1 Then
                            newContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                               Where a.OrderTestID = sortedOTList(aux_j + 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                            contaminations = GetContaminationBetweenReagents(ReagentContaminatedID, newContaminatedID, ContaminDS)
                        End If
                    End If

                    If contaminations.Count = 0 Then
                        '(j < i)
                        sortedOTList.Remove(contaminatedOrderTest) 'Remove the first ocurrence of contaminatedOrderTest
                        sortedOTList.Insert(aux_j + 1, contaminatedOrderTest)
                        Exit For
                    End If
                End If
            Next
        End Sub

        Protected Overrides Sub Execute_jj_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal indexJ As Integer, _
                                                  ByVal leftLimit As Integer, _
                                                  ByVal rightLimit As Integer, _
                                                  Optional ByVal upperTotalLimit As Integer = 0)

            MyBase.Execute_jj_loop(pExecutions, indexJ, leftLimit, rightLimit)
            For jj = leftLimit To rightLimit Step -1
                Dim aux_jj = jj
                'Next contamination to analyze is between OrderTest(i-2) --> OrderTest(i) / OrderTest(i-3) --> OrderTest(i) / ... /
                'until an OrderTest that not contaminates OrderTest(i) is found
                If aux_jj >= upperTotalLimit Then
                    ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(aux_jj) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                    'typeResult = GetTypeReagentInTest(dbConnection, ReagentContaminatorID)

                    If aux_jj = indexJ Then 'search for contamination (low or high level)
                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    Else 'search for contamination (only high level)
                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    End If

                    If contaminations.Count > 0 Then Exit For

                    'If ReagentsAreCompatibleType() Then Exit For

                    'AG 19/12/2011 - Evaluate only HIGH contamination persistance when OrderTest(jj) has MaxReplicates < pHighContaminationPersistance
                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                    If aux_jj = indexJ Then
                        Dim maxReplicates As Integer = (From b As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                         Where b.OrderTestID = sortedOTList(aux_jj) Select b.ReplicateNumber).Max
                        If maxReplicates >= HighContaminationPersistence Then
                            Exit For 'Do not evaluate high contamination persistance
                        End If
                    End If
                    'AG 19/12/2011

                Else
                    Exit For
                End If
            Next
        End Sub
    End Class
End Namespace

