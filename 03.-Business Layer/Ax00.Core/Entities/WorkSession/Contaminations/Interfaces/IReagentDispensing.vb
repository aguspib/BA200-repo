Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Optimizations

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations

    Public Interface IReagentDispensing

        Property ReagentNumber As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!

        Property R1ReagentID As Integer

        Property TechniqueID As Integer  'ID of the associated technique

        Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode

        Property Contamines As Dictionary(Of Integer, DisposingContaminationDescription)

        Function RequiredWashingSolution(TechniqueID As Integer, scope As Integer) As WashingDescription

    End Interface
End Namespace