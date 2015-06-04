Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities.Worksession.Sorting

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations

    ' ReSharper disable once UnusedMember.Global    'It's used at runtime by Recletion glue code!!
    ''' <summary>
    ''' Class that manages all the contamination related processes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ContaminationManager
        Implements IContaminationManager
        Public Property currentContaminationNumber As Integer Implements IContaminationManager.currentContaminationNumber
        Public Property bestContaminationNumber As Integer Implements IContaminationManager.bestContaminationNumber
        Public Property bestResult As List(Of ExecutionsDS.twksWSExecutionsRow) Implements IContaminationManager.bestResult
        Protected Property currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected Property ContDS As ContaminationsDS
        Protected Property previousReagentID As List(Of Integer)
        Protected Property previousReagentIDMaxReplicates As List(Of Integer)
        Protected Property MakeCalculationsInRunning As Boolean

        ''' <summary>
        ''' This is the class default constructor
        ''' </summary>
        ''' <param name="makeCalculationsInRunning"></param>
        ''' <param name="currentCont"></param>
        ''' <param name="contaminsDS"></param>
        ''' <param name="orderTests"></param>
        ''' <param name="pPreviousReagentID"></param>
        ''' <param name="pPreviousReagentIDMaxReplicates"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal makeCalculationsInRunning As Boolean,
                       ByVal currentCont As Integer,
                       ByVal contaminsDS As ContaminationsDS,
                       ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                       Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                       Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            Me.MakeCalculationsInRunning = makeCalculationsInRunning
            currentContaminationNumber = currentCont
            ContDS = contaminsDS
            previousReagentID = pPreviousReagentID
            previousReagentIDMaxReplicates = pPreviousReagentIDMaxReplicates
            bestContaminationNumber = Integer.MaxValue
            bestResult = orderTests.ToList()
        End Sub

        ''' <summary>
        ''' This is a static constructor that is injected to lower-level layers so they can create instances of the ContaminationsManager avoiding cyclic references.
        ''' </summary>
        ''' <param name="makeCalculationsInRunning"></param>
        ''' <param name="currentCont"></param>
        ''' <param name="contaminsDS"></param>
        ''' <param name="orderTests"></param>
        ''' <param name="pPreviousReagentID"></param>
        ''' <param name="pPreviousReagentIDMaxReplicates"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InjectableConstructor(ByVal makeCalculationsInRunning As Boolean,
                       ByVal currentCont As Integer,
                       ByVal contaminsDS As ContaminationsDS,
                       ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                       Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                       Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing) As IContaminationManager

            Dim aux = New ContaminationManager(makeCalculationsInRunning, currentCont, contaminsDS, orderTests, pPreviousReagentID, pPreviousReagentIDMaxReplicates)
            Return aux
        End Function

        ' ReSharper disable once UnusedMember.Global    'It's used by Reflection GLUE code!
        ''' <summary>
        ''' Method that apply the optimization algorithm defined on the system, in order to avoid as much number of contaminations as possible
        ''' </summary>
        ''' <param name="myOptimizer">OptimizationPolicyApplier wich defines the optimization algorithm</param>
        ''' <param name="orderTests">OrderTests to be sorted</param>
        ''' <remarks></remarks>
        Private Sub InternalApplyOptimizations(ByVal myOptimizer As IOptimizationPolicyApplier, ByVal orderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            myOptimizer.calculateInRunning = MakeCalculationsInRunning

            Dim highContaminationPersistance = WSExecutionCreator.Instance.ContaminationsSpecification.HighContaminationPersistence
            If currentContaminationNumber > 0 Then
                currentResult = orderTests.ToList()
                currentContaminationNumber = myOptimizer.ExecuteOptimization(ContDS, currentResult, highContaminationPersistance, previousReagentID, previousReagentIDMaxReplicates)
                If currentContaminationNumber < bestContaminationNumber Then
                    bestContaminationNumber = currentContaminationNumber
                    bestResult = currentResult
                End If
            End If
        End Sub

        Public Sub ApplyOptimizations(pCon As SqlConnection, orderTests As List(Of ExecutionsDS.twksWSExecutionsRow)) Implements IContaminationManager.ApplyOptimizations
            InternalApplyOptimizations(New OptimizationBacktrackingApplier(pCon), orderTests)
        End Sub
    End Class
End Namespace