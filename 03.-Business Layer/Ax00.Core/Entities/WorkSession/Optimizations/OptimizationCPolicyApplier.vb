Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL
    Public Class OptimizationCPolicyApplier: Inherits OptimizationPolicyApplier

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

            'Limit: when pPreviousReagentID <> nothing the initial limit is 2, otherwise 1
            Dim initialLimit As Integer = 1
            If Not pPreviousReagentID Is Nothing Then initialLimit = 2

            'Sort the different ordertest inside pExecutions to minimize contaminations (move down the contaminator until it does not contaminates)
            For i As Integer = initialLimit To sortedOTList.Count - 1
                ' ReSharper disable once InconsistentNaming
                Dim aux_i = i
                'First contamination to analyze is between OrderTest(i-1) --> OrderTest(i)
                ReagentContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(aux_i - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                contaminatorOrderTest = sortedOTList(aux_i - 1)

                ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(aux_i) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                MainContaminatedID = ReagentContaminatedID

                contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                'If no contamination between the consecutive tests look for high contamin [If (i-2) contaminates (i)]
                'Only if ordertest(i-1) maxreplicates < hihgcontamination persistance
                If contaminations.Count = 0 Then
                    Dim maxReplicates As Integer = 1
                    maxReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                     Where a.OrderTestID = sortedOTList(aux_i - 1) Select a.ReplicateNumber).Max
                    If maxReplicates < pHighContaminationPersistance Then
                        If aux_i = 1 AndAlso Not pPreviousReagentID Is Nothing Then
                            'This code has no sense, in current policy we are trying to move the contaminator and it belongs to another SAMPLE (it can not be move NOW!!!)
                        ElseIf aux_i > 1 Then
                            Dim highContaminatorID As Integer = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                    Where a.OrderTestID = sortedOTList(aux_i - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                            contaminations = GetHardContaminationBetweenReagents(highContaminatorID, ReagentContaminatedID, pContaminationsDS)
                        End If
                    End If
                End If

                ' SetExpectedTypeReagent()

                'OrderTest(i-1) contaminates OrderTest(i) ... so try move OrderTes(i-1) down until it does not contaminates
                If contaminations.Count > 0 Then
                    Execute_j_loop(pExecutions, aux_i, (aux_i + 1), sortedOTList.Count - 1)

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
            For j = lowerLimit To upperLimit
                Dim auxJ = j
                'Move the contaminator where not contaminates (taking care about HIGH contaminations persistance inside the Element group OrderTests)
                Dim offset As Integer = 1
                If HighContaminationPersistence > 1 Then offset = HighContaminationPersistence

                Execute_jj_loop(pExecutions, auxJ, auxJ, (auxJ + HighContaminationPersistence - 1))
                'AG 25/11/2011

                If contaminations.Count = 0 Then
                    'Move orderTest(i-1) (the contaminator one) before orderTest(j) (where orderTest(i-1) does not contaminates)

                    'New BAx00 (Ax5 do not implement this business
                    'Only when OrderTest(i-2) does not contaminates the OrderTest(i) (the MainContaminated and future OrderTest(i-1))
                    'Simplication: In this point do not take care about High contamination persistance
                    Dim newContaminatorID As Integer
                    If indexI > 1 Then
                        newContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                Where a.OrderTestID = sortedOTList(indexI - 2) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                        contaminations = GetContaminationBetweenReagents(newContaminatorID, MainContaminatedID, ContaminDS)
                        'typeResult = GetTypeReagentInTest(dbConnection, newContaminatorID)
                    End If

                    'Before move OrderTest(i-1) (the contaminator one, and future OrderTest(j)) be carefull is not contaminated by current OrderTest(j-1)
                    'Simplication: In this point do not take care about High contamination persistance
                    If contaminations.Count = 0 Then
                        If auxJ > 0 Then
                            newContaminatorID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                                               Where a.OrderTestID = sortedOTList(auxJ - 1) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First
                            contaminations = GetContaminationBetweenReagents(newContaminatorID, ReagentContaminatorID, ContaminDS)
                        End If
                    End If

                    If contaminations.Count = 0 Then
                        '(i < j)
                        If sortedOTList.Count - 1 > auxJ - 1 Then
                            sortedOTList.Insert(auxJ, contaminatorOrderTest)
                        Else
                            sortedOTList.Add(contaminatorOrderTest)
                        End If

                        sortedOTList.Remove(contaminatorOrderTest) 'Remove the first ocurrence of contaminatedOrderTest
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
            'NOTE: index 'j' is equivalent to low persistance, so in next loop the limit is pHighContaminationPersistance - 1
            For jj = leftLimit To rightLimit
                Dim auxJj = jj
                'Next contamination to analyze is between OrderTest(i-1) --> OrderTest(i+1) / OrderTest(i-1) --> OrderTest(i+2) / ... /
                'until an OrderTest that not has contaminated by OrderTest(i-1) is found
                If auxJj <= sortedOTList.Count - 1 Then
                    ReagentContaminatedID = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                            Where a.OrderTestID = sortedOTList(auxJj) AndAlso a.ExecutionStatus = "PENDING" Select a.ReagentID).First

                    'typeResult = GetTypeReagentInTest(dbConnection, ReagentContaminatedID)

                    If auxJj = leftLimit Then 'search for contamination (low or high level)
                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    Else 'search for contamination (only high level)
                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, ContaminDS)
                    End If

                    If contaminations.Count > 0 Then Exit For

                    'If ReagentsAreCompatibleType() Then Exit For
                    
                    'AG 19/12/2011 - Evaluate only HIGH contamination persistance when OrderTest(jj) has MaxReplicates < pHighContaminationPersistance
                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                    If auxJj = leftLimit Then
                        Dim maxReplicates As Integer = (From b As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                         Where b.OrderTestID = sortedOTList(auxJj) Select b.ReplicateNumber).Max
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

