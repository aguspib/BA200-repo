﻿Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities.WorkSession.Optimizations

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations

    ' ReSharper disable once UnusedMember.Global    'It's used at runtime by Recletion glue code!!
    ''' <summary>
    ''' Class that manages all the contamination related processes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ContaminationManager
        Public Property currentContaminationNumber As Integer
        Public Property bestContaminationNumber As Integer
        Public Property bestResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected Property currentResult As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected Property ContDS As ContaminationsDS
        Protected Property previousReagentID As List(Of Integer)
        Protected Property previousReagentIDMaxReplicates As List(Of Integer)
        Protected Property MakeCalculationsInRunning As Boolean

        Public Sub New(ByVal makeCalculationsInRunning As Boolean,
                       ByVal pCon As SqlConnection,
                       ByVal Analyzer As String,
                       ByVal currentCont As Integer,
                       ByVal pHighCont As Integer,
                       ByVal contaminsDS As ContaminationsDS,
                       ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow),
                       Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing,
                       Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing)
            Me.MakeCalculationsInRunning = makeCalculationsInRunning
            currentContaminationNumber = currentCont
            ContDS = contaminsDS
            previousReagentID = pPreviousReagentID
            previousReagentIDMaxReplicates = pPreviousReagentIDMaxReplicates
            bestContaminationNumber = Integer.MaxValue
            bestResult = OrderTests.ToList()
        End Sub

        ' ReSharper disable once UnusedMember.Global    'It's used by Reflection GLUE code!
        ''' <summary>
        ''' Method that apply the optimization algorithm defined on the system, in order to avoid as much number of contaminations as possible
        ''' </summary>
        ''' <param name="myOptimizer">OptimizationPolicyApplier wich defines the optimization algorithm</param>
        ''' <param name="OrderTests">OrderTests to be sorted</param>
        ''' <remarks></remarks>
        Public Sub ApplyOptimizations(ByVal myOptimizer As OptimizationPolicyApplier, ByVal OrderTests As List(Of ExecutionsDS.twksWSExecutionsRow))
            myOptimizer.calculateInRunning = MakeCalculationsInRunning
            Dim highContaminationPersistance = WSExecutionCreator.Instance.ContaminationsSpecification.HighContaminationPersistence
            If currentContaminationNumber > 0 Then
                currentResult = OrderTests.ToList()
                currentContaminationNumber = myOptimizer.ExecuteOptimization(ContDS, currentResult, highContaminationPersistance, previousReagentID, previousReagentIDMaxReplicates)
                If currentContaminationNumber < bestContaminationNumber Then
                    bestContaminationNumber = currentContaminationNumber
                    bestResult = currentResult
                End If
            End If
        End Sub
    End Class
End Namespace