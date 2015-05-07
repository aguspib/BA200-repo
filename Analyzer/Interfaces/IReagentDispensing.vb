Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

    Public Interface IReagentDispensing

        Property ReagentNumber As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!

        Property R1ReagentID As Integer

        Property ExecutionID As Integer

        ReadOnly Property AnalysisMode As Integer 'OptimizationPolicyApplier.AnalysisMode

        ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription)

        Function RequiredWashingSolution(Reagent As IReagentDispensing, scope As Integer) As IWashingDescription

    End Interface
End Namespace