Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Optimizations

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class BA200ReagentDispensing
        Implements IReagentDispensing



        Public Function RequiredWashingSolution(Reagent As Integer, scope As Integer) As WashingDescription
            Throw New NotImplementedException("Not yet ready!")
        End Function

        Public Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode Implements IReagentDispensing.AnalysisMode

        Public Property Contamines As Dictionary(Of Integer, DispensingContaminationDescription) Implements IReagentDispensing.Contamines

        Public Property R1ReagentID As Integer Implements IReagentDispensing.R1ReagentID

        Public Property ReagentNumber As Integer Implements IReagentDispensing.ReagentNumber

        Public Function RequiredWashingSolution1(dispensing As IReagentDispensing, scope As Integer) As WashingDescription Implements IReagentDispensing.RequiredWashingSolution
            Throw (New NotImplementedException("Not yet ready!"))
        End Function

        Public Property ExecutionID As Integer Implements IReagentDispensing.ExecutionID
    End Class

End Namespace