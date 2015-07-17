Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications

    Public Class BA400ContaminationsSpecification
        Inherits Ax00ContaminationsSpecification

        Implements IAnalyzerContaminationsSpecification

        Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)
        End Sub


        Public Overrides Function CreateDispensing() As IDispensing
            Return New BA400Dispensing
        End Function

        Public Overrides Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode
            Return AnalysisMode.MonoReactive
        End Function


        Public Overrides Function GetActionRequiredInRunning(dispensing As IDispensing) As IActionRequiredForDispensing
            Return currentContext.ActionRequiredForDispensing(dispensing)
        End Function

    End Class
End Namespace
