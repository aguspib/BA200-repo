Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IContaminationsContext
        Function GetActionRequiredForAGivenDispensing(dispensing As IReagentDispensing) As ActionRequiredForDispensing

        ''' <summary>
        ''' fills context from minimum index to 0, using the ExecutionsDS.<Para>This method ONLY fills the first dispensing (R1). </Para>
        ''' </summary>
        ''' <param name="expectedExecutions"></param>
        ''' <remarks></remarks>
        Sub FillContextInStatic(expectedExecutions As ExecutionsDS)

        Sub FillContextInStatic(executionsList As List(Of ExecutionsDS.twksWSExecutionsRow))
    End Interface

    Public Class ActionRequiredForDispensing
        Public Property Action As IContaminationsAction.RequiredAction
        Public Property InvolvedWashes As New List(Of IWashingDescription)
    End Class
End Namespace
