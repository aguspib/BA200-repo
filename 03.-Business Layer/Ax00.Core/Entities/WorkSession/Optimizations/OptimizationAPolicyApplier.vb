Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Public Class OptimizationAPolicyApplier : Inherits OptimizationPolicyApplier

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
            'Sort the different ordertest inside pExecutions to minimize contaminations (move down the contaminated until becomes not contaminated)
            For i As Integer = 1 To sortedOTList.Count - 1
                'First contamination to analyze is between OrderTest(i-1) --> OrderTest(i)
                Dim auxIndex = i
                ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(auxIndex - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                MainContaminatorID = ReagentContaminatorID

                contaminatedOrderTest = sortedOTList(i)
                ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(auxIndex) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)

                'If no contamination between the consecutive tests look for high contamin: [If (i-2) contaminates (i)]
                'Only if ordertest(i-1) maxreplicates < hihgcontamination persistance
                If contaminations.Count = 0 Then
                    Dim maxReplicates As Integer = 1
                    maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                     Where a.OrderTestID = sortedOTList(auxIndex - 1) Select a.ReplicateNumber).Max
                    If maxReplicates < pHighContaminationPersistance Then
                        If i = 1 AndAlso Not pPreviousReagentID Is Nothing Then
                            contaminations = GetHardContaminationBetweenReagents(pPreviousReagentID(pPreviousReagentID.Count - 1), ReagentContaminatedID, pContaminationsDS)
                        ElseIf i > 1 Then
                            ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                    Where a.OrderTestID = sortedOTList(auxIndex - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                            contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                        End If
                    End If
                End If

                SetExpectedTypeReagent()

                'OrderTest(i-1) (the MainContaminatorID) contaminates OrderTest(i) ... so try move OrderTes(i) down until becomes no contaminated
                If contaminations.Count > 0 Then

                    'Move the contaminated reagent where not contaminated is (taking care about HIGH contaminations persistance inside the Element group OrderTests)
                    'Evaluate if it is a LOW contamination ... next process starts in index: i + 1
                    'or if it is a HIGH contamination ... next process starts in index: i + pHighContaminationPersistance
                    Dim offset As Integer = 1
                    Dim maxReplicates As Integer = 1
                    If pHighContaminationPersistance > 1 AndAlso Not contaminations(0).IsWashingSolutionR1Null Then

                        'AG 19/12/2011 - if orderTest(i+1) maxreplicates > persistance --> offset = 1 // else: --> offset = pHighContaminationPersistance
                        'offset = pHighContaminationPersistance
                        If i + offset <= sortedOTList.Count - 1 Then
                            maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                             Where a.OrderTestID = sortedOTList(auxIndex + offset) Select a.ReplicateNumber).Max
                            If maxReplicates < pHighContaminationPersistance Then
                                offset = pHighContaminationPersistance
                            End If
                        End If
                    End If

                    Execute_j_loop(pExecutions, auxIndex, (i + offset), sortedOTList.Count - 1)

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
            For j As Integer = lowerLimit To upperLimit
                Dim aux_j = j

                'Only have to look back a maximum of pHighContaminationPersistance steps, but maxReplicates have to be taken into account for every position
                Dim limit As Integer
                If (HighContaminationPersistence > indexI) Then
                    limit = indexI
                Else
                    limit = HighContaminationPersistence
                End If

                'Move the contaminated where it is not contaminated (taking care about HIGH contaminations persistance inside the Element group OrderTests)
                Execute_jj_loop(pExecutions, aux_j, aux_j, (aux_j - limit))

                If contaminations.Count = 0 AndAlso ReagentsAreCompatibleType() Then
                    'Move orderTest(i) (the contaminated one) after orderTest(j) (where orderTest(i) is not contaminated)

                    'New BAx00 (Ax5 do not implement this business
                    'Only when the OrderTest(i-1) (the MainContaminatorID) does not contaminates the current OrderTest(i+1) (the future OrderTest(i))
                    'Simplication: In this point do not take care about High contamination persistance
                    Dim newContaminatedID As Integer
                    If indexI < sortedOTList.Count - 1 Then
                        newContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                Where a.OrderTestID = sortedOTList(indexI + 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                        contaminations = GetContaminationBetweenReagents(MainContaminatorID, newContaminatedID, ContaminDS)
                        typeResult = GetTypeReagentInTest(dbConnection, newContaminatedID)
                    End If

                    'Before move OrderTest(i) (the Contaminated one, and future OrderTest(j+1)) also be carefull does not contaminates the current OrderTest(j+1) (the future OrderTest(j+2)
                    'Simplication: In this point do not take care about High contamination persistance
                    If contaminations.Count = 0 AndAlso ReagentsAreCompatibleType() Then
                        If aux_j < sortedOTList.Count - 1 Then
                            newContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                               Where a.OrderTestID = sortedOTList(aux_j + 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                            contaminations = GetContaminationBetweenReagents(ReagentContaminatedID, newContaminatedID, ContaminDS)
                        End If
                    End If


                    If contaminations.Count = 0 AndAlso ReagentsAreCompatibleType() Then
                        '(i < j)
                        If sortedOTList.Count - 1 > aux_j Then
                            sortedOTList.Insert(aux_j + 1, contaminatedOrderTest)
                        Else
                            sortedOTList.Add(contaminatedOrderTest)
                        End If
                        sortedOTList.Remove(contaminatedOrderTest) 'Remove the first ocurrence of contaminatedOrderTest
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
                If aux_jj >= 0 Then
                    ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                             Where a.OrderTestID = sortedOTList(aux_jj) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                    typeResult = GetTypeReagentInTest(dbConnection, ReagentContaminatorID)

                    If aux_jj = indexJ Then 'search for contamination (low or high level)
                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    Else 'search for contamination (only high level)
                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    End If

                    If contaminations.Count > 0 Then Exit For

                    If Not ReagentsAreCompatibleType() Then Exit For

                    'Evaluate only HIGH contamination persistance when OrderTest(jj) has MaxReplicates < pHighContaminationPersistance
                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                    If aux_jj = indexJ Then
                        Dim maxReplicates = (From b As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                         Where b.OrderTestID = sortedOTList(aux_jj) Select b.ReplicateNumber).Max
                        If maxReplicates >= HighContaminationPersistence Then
                            Exit For 'Do not evaluate high contamination persistance
                        End If
                    End If
                Else
                    Exit For
                End If

            Next
        End Sub
    End Class
End Namespace

