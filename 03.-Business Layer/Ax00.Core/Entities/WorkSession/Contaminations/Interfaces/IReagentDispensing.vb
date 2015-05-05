Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Optimizations

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations

    Public Interface IReagentDispensing

        Property ReagentNumber As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!

        Property R1ReagentID As Integer

        Property ExecutionID As Integer

        Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode

        Property Contamines As Dictionary(Of Integer, DispensingContaminationDescription)

        Function RequiredWashingSolution(Reagent As IReagentDispensing, scope As Integer) As WashingDescription

    End Interface
End Namespace