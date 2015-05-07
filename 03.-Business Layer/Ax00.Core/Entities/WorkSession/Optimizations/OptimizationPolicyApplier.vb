Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Optimizations

    ''' <summary>
    ''' Class that implements the different optimization's policies that are currently available
    ''' </summary>
    ''' <remarks>
    ''' Created on 17/03/2015 by AJG
    ''' </remarks>
    Public Class OptimizationPolicyApplier
#Region "Properties"
        Protected Property ContaminationNumber As Integer
        Protected Property sortedOTList As List(Of Integer)
        Protected Property myOTListLinq As List(Of Integer)
        Protected Property ReagentContaminatorID As Integer
        Protected Property ReagentContaminatedID As Integer
        Protected Property contaminatedOrderTest As Integer
        Protected Property MainContaminatorID As Integer
        Protected Property contaminations As List(Of ContaminationsDS.tparContaminationsRow)
        Protected Property ContaminDS As ContaminationsDS
        Protected Property HighContaminationPersistence As Integer
        Protected Property contaminatorOrderTest As Integer
        Protected Property MainContaminatedID As Integer
        Protected Property dbConnection As SqlConnection
        Protected Property TypeContaminator As AnalysisMode
        Protected Property TypeContaminated As AnalysisMode
        Protected Property typeExpectedResult As AnalysisMode
        Protected Property typeResult As AnalysisMode
#End Region

#Region "Enums"


#End Region

#Region "Constructor"
        Public Sub New()
            ReagentContaminatorID = -1
            ReagentContaminatedID = -1
            contaminatedOrderTest = -1
            MainContaminatorID = -1
            contaminations = New List(Of ContaminationsDS.tparContaminationsRow)
            contaminatorOrderTest = -1
            MainContaminatedID = -1
            typeExpectedResult = AnalysisMode.MonoReactive
            typeResult = AnalysisMode.MonoReactive
        End Sub

        Public Sub New(ByVal pConn As SqlConnection) ', ByVal ActiveAnalyzer As String)
            Me.New()
            dbConnection = pConn

        End Sub

#End Region

#Region "Destructor"
        Protected Overrides Sub Finalize()
            dbConnection = Nothing
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' This code executes an optimization for solving contaminations between executions
        ''' </summary>
        ''' <param name="pContaminationsDS"></param>
        ''' <param name="pExecutions"></param>
        ''' <param name="pHighContaminationPersistance"></param>
        ''' <param name="pPreviousReagentID"></param>
        ''' <param name="pPreviousReagentIDMaxReplicates"></param>
        ''' <returns>the number of contaminations after optimization process</returns>
        ''' <remarks>
        ''' Created on 19/03/2015 by AJG
        ''' </remarks>
        Public Function ExecuteOptimization(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing) As Integer

            'This code is execute only when pPreviousReagentID is informed
            'Search a ReagentID (inside pExecutions) an OrderTest which ReagentID is not contamianted by pPreviousReagentID
            'and place his executions first
            Dim originalOrderChanged As Boolean = False
            Dim addContaminationBetweenGroups As Integer = 0
            If Not pPreviousReagentID Is Nothing Then
                MoveToAvoidContaminationBetweenElements(pContaminationsDS, pPreviousReagentID, pPreviousReagentIDMaxReplicates, pHighContaminationPersistance, pExecutions, originalOrderChanged, addContaminationBetweenGroups)
            End If

            Dim myExecutionDelegate As New ExecutionsDelegate()

            If pPreviousReagentID Is Nothing OrElse originalOrderChanged Then

                'Get all different ordertests in pExecutions list with executions pending
                myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                               Where a.ExecutionStatus = "PENDING" Select a.OrderTestID Distinct).ToList

                If myOTListLinq.Count > 1 Then
                    'Initialize the ordertest list to be sorted  ... use a temporal list
                    sortedOTList = myOTListLinq.ToList


                    Execute_i_loop(pContaminationsDS, pExecutions, pHighContaminationPersistance, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

                    'Now the sortedOTList contains the new orderTest order (but only these with executions pending), now we have to add the locked
                    AddLockedExecutions(pExecutions, sortedOTList)
                    ContaminationNumber = ExecutionsDelegate.GetContaminationNumber(pContaminationsDS, pExecutions, pHighContaminationPersistance) + addContaminationBetweenGroups

                ElseIf myOTListLinq.Count = 1 Then 'No movement is possible
                    ContaminationNumber = addContaminationBetweenGroups
                End If
                myOTListLinq = Nothing

            Else 'If pPreviousElementLastReagentID = -1 OrElse originalOrderChanged Then
                ContaminationNumber = ExecutionsDelegate.GetContaminationNumber(pContaminationsDS, pExecutions, pHighContaminationPersistance) + addContaminationBetweenGroups
            End If

            Return ContaminationNumber
        End Function

        Public ReadOnly Property ContaminationsSpecification() As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsSpecification
            End Get
        End Property

#End Region

#Region "Overridable methods"
        ''' <summary>
        ''' Virtual method that needs to be overriden on children classes
        ''' </summary>
        ''' <param name="pContaminationsDS"></param>
        ''' <param name="pExecutions"></param>
        ''' <param name="pHighContaminationPersistance"></param>
        ''' <param name="pPreviousReagentID"></param>
        ''' <param name="pPreviousReagentIDMaxReplicates"></param>
        ''' <remarks>
        ''' Created on 18/03/2015 by AJG
        ''' </remarks>
        Protected Overridable Sub Execute_i_loop(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            'this function must be overriden by children classes
            ContaminDS = pContaminationsDS
            HighContaminationPersistence = pHighContaminationPersistance
        End Sub

        ''' <summary>
        ''' Virtual method that needs to be overriden on children classes
        ''' </summary>
        ''' <param name="pExecutions"></param>
        ''' <param name="indexI"></param>
        ''' <param name="lowerLimit"></param>
        ''' <param name="upperLimit"></param>
        ''' <remarks>
        ''' Created on 18/03/2015 by AJG
        ''' </remarks>
        Protected Overridable Sub Execute_j_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal indexI As Integer, _
                                                  ByVal lowerLimit As Integer, _
                                                  ByVal upperLimit As Integer)
            'this function must be overriden by children classes
        End Sub

        ''' <summary>
        ''' Virtual method that needs to be overriden on children classes
        ''' </summary>
        ''' <param name="pExecutions"></param>
        ''' <param name="indexJ"></param>
        ''' <param name="leftLimit"></param>
        ''' <param name="rightLimit"></param>
        ''' <param name="upperTotalLimit"></param>
        ''' <remarks>
        ''' Created on 18/03/2015 by AJG
        ''' </remarks>
        Protected Overridable Sub Execute_jj_loop(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal indexJ As Integer, _
                                                  ByVal leftLimit As Integer, _
                                                  ByVal rightLimit As Integer, _
                                                  Optional ByVal upperTotalLimit As Integer = 0)
            'this function must be overriden by children classes
        End Sub
#End Region

#Region "Private methods"

        ''' <summary>
        ''' Sets the type of the reagent to be found, in order to eliminate the current contamination between contaminador and contaminated
        ''' </summary>
        ''' <remarks>
        ''' Created on 19/03/2015
        ''' </remarks>
        Protected Sub SetExpectedTypeReagent()
            TypeContaminator = ContaminationsSpecification.GetAnalysisModeForReagent(ReagentContaminatorID)
            TypeContaminated = ContaminationsSpecification.GetAnalysisModeForReagent(ReagentContaminatedID)

            typeExpectedResult = ContaminationsSpecification.RequiredAnalysisModeBetweenReactions(TypeContaminator, TypeContaminated)

            typeResult = AnalysisMode.MonoReactive

        End Sub

        ''' <summary>
        ''' Regarding contaminations, this method determines if the current candidate analysis mode (bi or monoreactive) is compatible with the expected reactive analysis mdoe (bi or monoreactive). 
        ''' <para>As instance, in some analyzers, a monoreactive reagent requires another monoreactive reagent when possible.</para>
        ''' </summary>
        ''' <returns>A boolean value that indicates if the current candidate complies the expected type of reagent</returns>
        ''' <remarks>
        ''' Created on 18/03/2015
        ''' </remarks>
        Protected Function ReagentAnalysisModesAreCompatible() As Boolean
            Return ContaminationsSpecification.AreAnalysisModesCompatible(typeResult, typeExpectedResult)
        End Function


        ''' <summary>
        ''' This is the last part of the algorithm for applying optimizations because of contaminations.This part is common to all of them
        ''' </summary>
        ''' <param name="pExecutions">List of executions that include the contaminations</param>
        ''' <param name="sortedOTList">List of sorted tests</param>
        ''' <remarks>
        ''' Created on 17/03/2015 by AJG
        ''' </remarks>
        Private Sub AddLockedExecutions(ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), ByVal sortedOTList As List(Of Integer))
            'Now the sortedOTList contains the new orderTest order (but only these with executions pending), now we have to add the locked
            myOTListLinq = (From a In pExecutions _
                            Where a.ExecutionStatus = "LOCKED" Select a.OrderTestID Distinct).ToList
            For Each lockedOrderTestitem As Integer In myOTListLinq
                If Not sortedOTList.Contains(lockedOrderTestitem) Then
                    sortedOTList.Add(lockedOrderTestitem)
                End If
            Next

            'Create a copy of pExecutions and then move executions from copy -> pExecutions
            Dim copypExecutions = pExecutions.ToList

            pExecutions.Clear()
            Dim myOTExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
            For Each orderTestitem As Integer In sortedOTList
                'Get executions of orderTestitem using the copypExecutions
                myOTExecutions = (From a As ExecutionsDS.twksWSExecutionsRow In copypExecutions _
                               Where a.OrderTestID = orderTestitem Select a).ToList

                pExecutions.AddRange(myOTExecutions)
            Next
        End Sub

        ''' <summary>
        ''' Get the contaminations that exist between contaminator and contaminated reagents
        ''' </summary>
        ''' <param name="Contaminator">Reagent that contaminates</param>
        ''' <param name="Contaminated">Reagent that is contaminated</param>
        ''' <param name="pContaminationsDS">List of contaminations for a given patient</param>
        ''' <returns>List of contaminations between Contaminator and Contaminated</returns>
        ''' <remarks>
        ''' Created by AJG in 17/03/2015
        ''' </remarks>
        Protected Function GetContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As List(Of ContaminationsDS.tparContaminationsRow)
            Dim result = (From wse In pContaminationsDS.tparContaminations _
                                             Where wse.ReagentContaminatorID = Contaminator _
                                             AndAlso wse.ReagentContaminatedID = Contaminated _
                                             Select wse).ToList()

            Return result
        End Function

        ''' <summary>
        ''' Get the hard contaminations that exist between contaminator and contaminated reagents.
        ''' Hard contaminations are those that requires a washing solution to applied.
        ''' </summary>
        ''' <param name="Contaminator">Reagent that contaminates</param>
        ''' <param name="Contaminated">Reagent that is contaminated</param>
        ''' <param name="pContaminationsDS">List of contaminations for a given patient</param>
        ''' <returns>List of hard contaminations between Contaminator and Contaminated</returns>
        ''' <remarks>
        ''' Created by AJG in 17/03/2015
        ''' </remarks>
        Protected Function GetHardContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As List(Of ContaminationsDS.tparContaminationsRow)
            Dim result = (From wse In pContaminationsDS.tparContaminations _
                                             Where wse.ReagentContaminatorID = Contaminator _
                                             AndAlso wse.ReagentContaminatedID = Contaminated _
                                             AndAlso Not wse.IsWashingSolutionR1Null _
                                             Select wse).ToList()

            Return result
        End Function

        ''' <summary>
        ''' To avoid contamination between previous element group in WS executions last reagent and the fist in the next element group in WS executions
        ''' Search an OrderTest in new element group not contaminated by last OrderTest in previous element group
        ''' Take into account LOW and HIGH contamination level
        ''' 
        ''' No Try - Catch implemented due the caller method implements it
        ''' </summary>
        ''' <param name="pContaminationsDS"></param>
        ''' <param name="pPreviousReagentID">(the nearest reagents use the higher indexs)</param> 
        ''' <param name="pPreviousReplicatesNumber">(the nearest reagents use the higher indexs)</param>
        ''' <param name="pHighContaminationPersistance" ></param>
        ''' <param name="pExecutions"></param>
        ''' <param name="originalorderchanged"></param>
        ''' <param name="addContaminationBetweenGroups"></param>
        ''' <remarks>AG 08/11/2011</remarks>
        Private Sub MoveToAvoidContaminationBetweenElements(ByVal pContaminationsDS As ContaminationsDS, ByVal pPreviousReagentID As List(Of Integer), ByVal pPreviousReplicatesNumber As List(Of Integer), _
                                                            ByVal pHighContaminationPersistance As Integer, ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), ByRef originalorderchanged As Boolean, ByRef addContaminationBetweenGroups As Integer)

            If pPreviousReagentID.Count > 0 Then
                Dim myOTListLinq As List(Of Integer)
                myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                               Select a.OrderTestID Distinct).ToList

                Dim ReagentContaminatorID = pPreviousReagentID(pPreviousReagentID.Count - 1) 'Last Reagent in previous element group (reverse order)
                Dim ReagentContaminatedID As Integer = -1
                Dim myExecLinqByOT As List(Of ExecutionsDS.twksWSExecutionsRow)
                Dim contaminations As List(Of ContaminationsDS.tparContaminationsRow) = Nothing

                If myOTListLinq.Count > 1 Then
                    Dim itera As Integer = 0
                    Dim insertPosition As Integer = 0

                    '1) Search test for FIRST POSITION: Evaluate contaminations between:
                    'LastReagent(Last) -> Next Reagent(0)
                    'LastReagent(Last-1) -> Next Reagent(0) (special)
                    'If contamination ... search one test not contaminated to be place in FIRST position
                    For Each myOrderTest As Integer In myOTListLinq
                        myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                          Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" _
                                          Select a).ToList

                        If myExecLinqByOT.Count > 0 Then
                            ReagentContaminatedID = myExecLinqByOT(0).ReagentID
                            For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - pHighContaminationPersistance Step -1
                                If jj >= 0 Then
                                    ReagentContaminatorID = pPreviousReagentID(jj)

                                    If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
                                        contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                                    Else 'search for contamination (only high level)
                                        contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                                    End If

                                    If contaminations.Count > 0 Then Exit For

                                    'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
                                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                    If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= pHighContaminationPersistance Then
                                        Exit For 'Do not evaluate high contamination persistance
                                    End If

                                Else
                                    Exit For
                                End If
                            Next

                            If contaminations.Count = 0 Then
                                addContaminationBetweenGroups = 0
                                If itera > 0 Then
                                    'New FIRST position test executions
                                    For Each currentExecution In myExecLinqByOT
                                        pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
                                        pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
                                        insertPosition += 1
                                    Next
                                    originalorderchanged = True
                                End If
                                Exit For 'For Each myOrderTest As ....

                            Else
                                addContaminationBetweenGroups = 1
                            End If
                            'contaminations = Nothing

                        Else
                            addContaminationBetweenGroups = 1
                        End If
                        itera += 1
                    Next

                    If originalorderchanged Then
                        '2) Search test for SECOND POSITION: Evaluate contaminations between:
                        'Next Reagent(0) -> Next Reagent(1) 
                        'LastReagent(Last) -> Next Reagent(1) (special)
                        'If contamination ... search one test not contaminated to be place in SECOND position
                        itera = 0
                        Dim firstPositionContaminatorID As Integer = 0
                        myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                        Select a.OrderTestID Distinct).ToList 'New query over the new sort

                        For Each myOrderTest As Integer In myOTListLinq
                            myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                                              Where a.OrderTestID = myOrderTest AndAlso a.ExecutionStatus = "PENDING" _
                                              Select a).ToList

                            'Define the insert position for the SECOND test
                            If myExecLinqByOT.Count > 0 Then
                                If itera = 0 Then
                                    insertPosition = myExecLinqByOT.Count
                                    firstPositionContaminatorID = myExecLinqByOT(0).ReagentID

                                    'Evaluate only HIGH contamination persistance when OrderTest in FIRST position has MaxReplicates < pHighContaminationPersistance
                                    'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                    If myExecLinqByOT.Count >= pHighContaminationPersistance Then
                                        Exit For '(do not evaluate high contamination persistance)
                                    End If
                                End If

                                If itera > 0 AndAlso myExecLinqByOT.Count > 0 Then
                                    ReagentContaminatedID = myExecLinqByOT(0).ReagentID

                                    'search for contamination (low or high level) between FIRST position test (low or high level)
                                    contaminations = GetContaminationBetweenReagents(firstPositionContaminatorID, ReagentContaminatedID, pContaminationsDS)

                                    'If no contamination with the FIRST position test then evaluate the HIGH level contamination with the last reagent
                                    If contaminations.Count = 0 Then
                                        contaminations = GetHardContaminationBetweenReagents(pPreviousReagentID(pPreviousReagentID.Count - 1), ReagentContaminatedID, pContaminationsDS)
                                    End If


                                    If contaminations.Count = 0 Then
                                        addContaminationBetweenGroups = 0
                                        If itera > 1 Then
                                            'New SECOND position test executions
                                            For Each currentExecution In myExecLinqByOT
                                                pExecutions.Remove(currentExecution) 'First remove from pExecutions (original position)
                                                pExecutions.Insert(insertPosition, currentExecution) 'Second add into pExecutions at the begining
                                                insertPosition += 1
                                            Next
                                            originalorderchanged = True
                                        End If
                                        Exit For 'For Each myOrderTest As ....

                                    Else
                                        addContaminationBetweenGroups = 1
                                    End If
                                    'contaminations = Nothing
                                End If
                            End If
                            itera += 1
                        Next
                    End If
                ElseIf myOTListLinq.Count = 1 Then 'If myOTListLinq.Count > 1 Then (only one test, no movement is possible)
                    myExecLinqByOT = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                      Where a.OrderTestID = myOTListLinq(0) AndAlso a.ExecutionStatus = "PENDING" _
                      Select a).ToList

                    If myExecLinqByOT.Count > 0 Then
                        ReagentContaminatedID = myExecLinqByOT(0).ReagentID

                        For jj = pPreviousReagentID.Count - 1 To pPreviousReagentID.Count - pHighContaminationPersistance Step -1
                            If jj >= 0 Then
                                ReagentContaminatorID = pPreviousReagentID(jj)

                                If jj = pPreviousReagentID.Count - 1 Then 'search for contamination (low or high level)
                                    contaminations = GetContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)

                                Else 'search for contamination (only high level)
                                    contaminations = GetHardContaminationBetweenReagents(ReagentContaminatorID, ReagentContaminatedID, pContaminationsDS)
                                End If

                                If contaminations.Count > 0 Then Exit For

                                'Evaluate only HIGH contamination persistance when OrderTest that uses reagent (pPreviousReagentID.Count - 1) has MaxReplicates < pHighContaminationPersistance
                                'If this condition is FALSE ... Exit For (do not evaluate high contamination persistance)
                                If jj = pPreviousReagentID.Count - 1 AndAlso pPreviousReplicatesNumber(pPreviousReplicatesNumber.Count - 1) >= pHighContaminationPersistance Then
                                    Exit For 'Do not evaluate high contamination persistance
                                End If

                            Else
                                Exit For
                            End If
                        Next
                        'AG 20/12/2011 

                        If contaminations.Count > 0 Then
                            addContaminationBetweenGroups = 1
                        End If
                    End If

                End If

                contaminations = Nothing
                myOTListLinq = Nothing
                myExecLinqByOT = Nothing
            End If

        End Sub

#End Region
    End Class
End Namespace

