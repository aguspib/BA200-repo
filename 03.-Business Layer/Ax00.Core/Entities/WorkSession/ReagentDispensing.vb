Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Optimizations
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public MustInherit Class ReagentDispensing
        Implements IReagentDispensing

        Public Function RequiredWashingSolution(dispensing As IReagentDispensing, scope As Integer) As WashingDescription
            If scope = 0 Then   'A reagent can't contamine itself
                Return New EmptyWashing

            ElseIf scope > 0 Then   'A reagent can't contamine somethig that was already sent
                'Throw New Exception("A later dispensing can't contamine us")
                Return dispensing.RequiredWashingSolution(Me, -scope)

            ElseIf Me.Contamines Is Nothing OrElse Me.Contamines.ContainsKey(dispensing.R1ReagentID) = False Then
                Return New EmptyWashing
            Else

                Dim washing = Me.Contamines(dispensing.R1ReagentID)
                If washing.RequiredWashing.CleaningPower < Math.Abs(scope) Then
                    Return New EmptyWashing
                Else
                    'Dim newCleaning = New WashingDescription(washing.RequiredWashing.CleaningPower + scope, washing.RequiredWashing.WashingSolutionID)

                    Dim newCleaning = New WashingDescription(washing.RequiredWashing.CleaningPower, washing.RequiredWashing.WashingSolutionID)
                    Return newCleaning
                End If
            End If
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