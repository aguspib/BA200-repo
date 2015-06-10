Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.CC

Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IContaminationsContext
        Function ActionRequiredForDispensing(dispensing As IDispensing) As IActionRequiredForDispensing
        Function ActionRequiredForDispensing(Execution As ExecutionsDS.twksWSExecutionsRow) As IActionRequiredForDispensing
        Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String)

        ''' <summary>
        ''' fills context from minimum index to 0, using the ExecutionsDS.<Para>This method ONLY fills the first dispensing (R1). </Para>
        ''' </summary>
        ''' <param name="expectedExecutions"></param>
        ''' <remarks></remarks>
        Sub FillContextInStatic(expectedExecutions As ExecutionsDS)

        Sub FillContextInStatic(executionsList As List(Of ExecutionsDS.twksWSExecutionsRow))

        Property Steps As RangedCollection(Of IContextStep)
    End Interface

    Public Interface IActionRequiredForDispensing
        Property Action As IContaminationsAction.RequiredAction
        Property InvolvedWashes As List(Of IWashingDescription)
    End Interface
End Namespace
