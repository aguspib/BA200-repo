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
    Public MustInherit Class OptimizationPolicyApplier
#Region "Properties"
        Public Property calculateInRunning As Boolean
        Protected Property ContaminationNumber As Integer
        Protected Property sortedOTList As List(Of Integer)
        Protected Property myOTListLinq As List(Of Integer)
        Protected Property ContaminDS As ContaminationsDS
        Protected Property typeExpectedResult As AnalysisMode
        Protected Property typeResult As AnalysisMode
#End Region

#Region "Enums"


#End Region

#Region "Constructor"
        Public Sub New()
            typeExpectedResult = AnalysisMode.MonoReactive
            typeResult = AnalysisMode.MonoReactive
        End Sub

        Public Sub New(ByVal pConn As SqlConnection) ', ByVal ActiveAnalyzer As String)
            Me.New()
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
            Dim ContaminationBetweenGroups As Integer = 0
            If Not pPreviousReagentID Is Nothing Then

                'MIC
                Dim C = New ContaminationBetweenElementsSorter(pPreviousReagentID, pExecutions)
                C.MoveToAvoidContaminationBetweenElements()
                ContaminationBetweenGroups = C.AddContaminationBetweenGroups

                '/MIC

                'MoveToAvoidContaminationBetweenElements(pContaminationsDS, pPreviousReagentID, pPreviousReagentIDMaxReplicates, pHighContaminationPersistance, pExecutions, originalOrderChanged, addContaminationBetweenGroups)
            End If

            If pPreviousReagentID Is Nothing OrElse originalOrderChanged OrElse ContaminationBetweenGroups <> 0 Then

                'Get all different ordertests in pExecutions list with executions pending
                myOTListLinq = (From a As ExecutionsDS.twksWSExecutionsRow In pExecutions _
                               Where a.ExecutionStatus = "PENDING" Select a.OrderTestID Distinct).ToList

                If myOTListLinq.Count > 1 Then
                    'Initialize the ordertest list to be sorted  ... use a temporal list
                    sortedOTList = myOTListLinq.ToList


                    ExecuteOptimizationAlgorithm(pContaminationsDS, pExecutions, pHighContaminationPersistance, pPreviousReagentID, pPreviousReagentIDMaxReplicates)

                    'Now the sortedOTList contains the new orderTest order (but only these with executions pending), now we have to add the locked
                    AddLockedExecutions(pExecutions, sortedOTList)
                    ContaminationNumber = GetContaminationNumber(pContaminationsDS, pExecutions)
                    'ExecutionsDelegate.GetContaminationNumber(pContaminationsDS, pExecutions, pHighContaminationPersistance) + addContaminationBetweenGroups

                ElseIf myOTListLinq.Count = 1 Then 'No movement is possible
                    ContaminationNumber = ContaminationBetweenGroups
                End If
                myOTListLinq = Nothing

            Else 'If pPreviousElementLastReagentID = -1 OrElse originalOrderChanged Then
                ContaminationNumber = GetContaminationNumber(pContaminationsDS, pExecutions) 'ExecutionsDelegate.GetContaminationNumber(pContaminationsDS, pExecutions, pHighContaminationPersistance) + addContaminationBetweenGroups
            End If

            Return ContaminationNumber
        End Function

        Public ReadOnly Property ContaminationsSpecification() As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsSpecification
            End Get
        End Property

        Public MustOverride Function GetContaminationNumber(ByVal pContaminationsDS As ContaminationsDS, pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)) As Integer


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
        Protected Overridable Sub ExecuteOptimizationAlgorithm(ByVal pContaminationsDS As ContaminationsDS, _
                                                  ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                                  ByVal pHighContaminationPersistance As Integer, _
                                                  Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                                  Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            'this function must be overriden by children classes
            ContaminDS = pContaminationsDS
            'HighContaminationPersistence = pHighContaminationPersistance
        End Sub


#End Region

#Region "Private methods"

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
        Private Function InternalGetContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow)
            Dim result = (From wse In pContaminationsDS.tparContaminations _
                                             Where wse.ReagentContaminatorID = Contaminator _
                                             AndAlso wse.ReagentContaminatedID = Contaminated _
                                             Select wse)

            Return result
        End Function

        Protected Function AreThereContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As Boolean
            Return InternalGetContaminationBetweenReagents(Contaminator, Contaminated, pContaminationsDS).Any
        End Function

        Protected Function GetContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow)
            Dim result = InternalGetContaminationBetweenReagents(Contaminator, Contaminated, pContaminationsDS)
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
        Protected Function GetHardContaminationBetweenReagents(ByVal Contaminator As Integer, ByVal Contaminated As Integer, ByVal pContaminationsDS As ContaminationsDS) As EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow)
            Dim result = (From wse In pContaminationsDS.tparContaminations _
                                             Where wse.ReagentContaminatorID = Contaminator _
                                             AndAlso wse.ReagentContaminatedID = Contaminated _
                                             AndAlso Not wse.IsWashingSolutionR1Null _
                                             Select wse)

            Return result
        End Function

#End Region
    End Class
End Namespace

