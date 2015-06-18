Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Worksession.Sorting
    Public Interface IOptimizationPolicyApplier
        Property calculateInRunning As Boolean

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
        Function ExecuteOptimization(ByVal pContaminationsDS As ContaminationsDS, _
                                            ByRef pExecutions As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                            ByVal pHighContaminationPersistance As Integer, _
                                            Optional ByVal pPreviousReagentID As List(Of Integer) = Nothing, _
                                            Optional ByVal pPreviousReagentIDMaxReplicates As List(Of Integer) = Nothing) As Integer
    End Interface
End Namespace