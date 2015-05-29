Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
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

        Public Overrides Function AreAnalysisModesCompatible(current As AnalysisMode, expected As AnalysisMode) As Boolean
            Return True
        End Function


    End Class
End Namespace
