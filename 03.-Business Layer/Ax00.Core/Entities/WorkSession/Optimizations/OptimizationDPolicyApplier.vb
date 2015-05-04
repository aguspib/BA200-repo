Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
    Public Class OptimizationDPolicyApplier : Inherits OptimizationPolicyApplier

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
            'Limit: when pPreviousReagentID <> -1 the initial limit is 2, otherwise 1
            Dim initialLimit As Integer = 1
            If Not pPreviousReagentID Is Nothing Then initialLimit = 2

            'Sort the different ordertest inside pExecutions to minimize contaminations (move up the contaminator until it does not contaminates)
            For i2 = initialLimit To sortedOTList.Count - 1
                Dim i = i2
                'First contamination to analyze is between OrderTest(i-1) --> OrderTest(i)
                ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(i - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                contaminatorOrderTest = sortedOTList(i - 1)
                ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(i) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                MainContaminatedID = ReagentContaminatedID

                contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                'If no contamination between the consecutive tests look for high contamin [If (i-2) contaminates (i)]
                'Only if ordertest(i-1) maxreplicates < hihgcontamination persistance
                If contaminations.Count = 0 Then
                    Dim maxReplicates As Integer = 1
                    maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                     Where a.OrderTestID = sortedOTList(i - 1) Select a.ReplicateNumber).Max
                    If maxReplicates < pHighContaminationPersistance Then
                        If i = 1 AndAlso Not pPreviousReagentID Is Nothing Then
                            'This code has no sense, in current policy we are trying to move the contaminator and it belongs to another SAMPLE (it can not be move NOW!!!)
                        ElseIf i > 1 Then
                            Dim highContaminatorID As Integer = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                    Where a.OrderTestID = sortedOTList(i - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                            contaminations = GetHardContaminationBetweenReagents(highContaminatorID, ReagentContaminatedID, pContaminationsDS)
                        End If
                    End If
                End If

                SetExpectedTypeReagent()

                'OrderTest(i-1) contaminates OrderTest(i) ... so try move OrderTes(i-1) up until it does not contaminates
                If contaminations.Count > 0 Then
                    'Limit: when pPreviousReagentID <> -1 the upper limit is 1, otherwise 0
                    Dim upperLimit As Integer = 0
                    If Not pPreviousReagentID Is Nothing Then upperLimit = 1


                    'AG 28/11/2011 Move the contaminator reagent where not contaminates (taking care about HIGH contaminations persistance inside the Element group OrderTests)
                    'Evaluate if it is a LOW contamination ... next process starts in index: i - 2
                    'or if it is a HIGH contamination ... next process starts in index: i -2 - pHighContaminationPersistance-1
                    Dim offset As Integer = 0
                    Dim maxReplicates As Integer = 1

                    If pHighContaminationPersistance > 1 AndAlso Not contaminations(0).IsWashingSolutionR1Null Then

                        'AG 19/12/2011 - if orderTest(i-2) maxreplicates > persistance --> offset = 0 // else: --> offset = pHighContaminationPersistance - 1
                        'offset = pHighContaminationPersistance - 1
                        If i - 2 >= upperLimit Then
                            maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                             Where a.OrderTestID = sortedOTList(i - 2) Select a.ReplicateNumber).Max
                            If maxReplicates < pHighContaminationPersistance Then
                                offset = pHighContaminationPersistance - 1
                            End If
                        End If
                        'AG 19/12/2011 

                    End If
                    'AG 25/11/2011


                    Execute_j_loop(pExecutions, i, (i - 2 - offset), upperLimit)
                    'AG 28/11/2011


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
            For j = lowerLimit To upperLimit Step -1
                Dim aux_j = j
                ''Next contamination to analyze is between OrderTest(i-1) --> OrderTest(i-2) / OrderTest(i-1) --> OrderTest(i-3) / ... /
                ''until an OrderTest that it is not contaminated bytOrderTest(i) is found

                Execute_jj_loop(pExecutions, aux_j, aux_j, (aux_j - HighContaminationPersistence - 1))

                If contaminations.Count = 0 AndAlso ReagentsAreCompatibleType() Then
                    'Move orderTest(i-1) (the contaminator one) before orderTest(j) (where orderTest(i-1) does not contaminates)

                    'New BAx00 (Ax5 do not implement this business
                    'Only when OrderTest(i-2) does not contaminates the OrderTest(i) (the MainContaminated)
                    'Simplication: In this point do not take care about High contamination persistance
                    Dim newContaminatorID As Integer
                    If indexI > 1 Then
                        newContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                Where a.OrderTestID = sortedOTList(indexI - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                        contaminations = GetContaminationBetweenReagents(newContaminatorID, MainContaminatedID, ContaminDS)
                        typeResult = GetTypeReagentInTest(dbConnection, newContaminatorID)
                    End If

                    'Before move OrderTest(i-1) (the contaminator one, and future OrderTest(j-1)) be carefull is not contaminated by OrderTest(j-1) (and future OrderTest(j-2))
                    'Simplication: In this point do not take care about High contamination persistance
                    If contaminations.Count = 0 AndAlso ReagentsAreCompatibleType() Then
                        If aux_j > 0 Then
                            newContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                               Where a.OrderTestID = sortedOTList(aux_j - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                            contaminations = GetContaminationBetweenReagents(newContaminatorID, ReagentContaminatorID, ContaminDS)
                        End If
                    End If

                    If contaminations.Count = 0 Then
                        '(j < i)
                        sortedOTList.Remove(contaminatorOrderTest) 'Remove the first ocurrence of contaminatedOrderTest
                        'AG 30/05/2012 - Fix system error (index out of bounds when j = 0)
                        'sortedOTList.Insert(j - 1, contaminatorOrderTest)
                        If aux_j > 0 Then
                            sortedOTList.Insert(aux_j - 1, contaminatorOrderTest)
                        Else
                            sortedOTList.Insert(0, contaminatorOrderTest)
                        End If
                        'AG 30/05/2012
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

            'Move the contaminator where not contaminates (taking care about HIGH contaminations persistance inside the Element group OrderTests)
            'NOTE: index 'j' is equivalent to low persistance, so in next loop the limit is pHighContaminationPersistance - 1
            For jj = leftLimit To rightLimit Step -1
                Dim auxJj = jj
                If auxJj >= 0 Then
                    'Next contamination to analyze is between OrderTest(i-1) --> OrderTest(i-2) / OrderTest(i-1) --> OrderTest(i-3) / ... /
                    'until an OrderTest that it is not contaminated bytOrderTest(i) is found
                    ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(auxJj) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                    typeResult = GetTypeReagentInTest(dbConnection, ReagentContaminatedID)

                    If auxJj = indexJ Then 'search for contamination (low or high level)
                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    Else 'search for contamination (only high level)
                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    End If

                    If contaminations.Count > 0 Then Exit For

                    If Not ReagentsAreCompatibleType() Then Exit For

                    'AG 19/12/2011 - Evaluate only HIGH contamination persistance when OrderTest(jj) has MaxReplicates < pHighContaminationPersistance
                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                    If auxJj = indexJ Then
                        Dim maxReplicates = (From b As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                         Where b.OrderTestID = sortedOTList(auxJj) Select b.ReplicateNumber).Max
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

