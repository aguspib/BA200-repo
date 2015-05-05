Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ReagentDispensing
        Implements IReagentDispensing



        Public Function RequiredWashingSolution(dispensing As IReagentDispensing, scope As Integer) As IWashingDescription Implements IReagentDispensing.RequiredWashingSolution
            If scope = 0 Then   'A reagent can't contamine itself
                Return New EmptyWashing

            ElseIf scope > 0 Then   'A reagent can't contamine somethig that was already sent
                'Throw New Exception("A later dispensing can't contamine us")
                Return dispensing.RequiredWashingSolution(Me, -scope)

            ElseIf Me.Contamines Is Nothing OrElse Me.Contamines.ContainsKey(dispensing.R1ReagentID) = False Then
                Return New EmptyWashing
            Else

                Dim washing = Me.Contamines(dispensing.R1ReagentID)
                If washing.RequiredWashing.WashingStrength < Math.Abs(scope) Then
                    Return New EmptyWashing
                Else
                    'Dim newCleaning = New WashingDescription(washing.RequiredWashing.WashingStrength + scope, washing.RequiredWashing.WashingSolutionID)

                    Dim newCleaning = New WashingDescription(washing.RequiredWashing.WashingStrength, washing.RequiredWashing.WashingSolutionID)
                    Return newCleaning
                End If
            End If

        End Function

        Public Property AnalysisMode As Integer Implements IReagentDispensing.AnalysisMode

        Public Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IReagentDispensing.Contamines

        Public Property R1ReagentID As Integer Implements IReagentDispensing.R1ReagentID

        Public Property ReagentNumber As Integer Implements IReagentDispensing.ReagentNumber



        Public Property ExecutionID As Integer Implements IReagentDispensing.ExecutionID

        'Public Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IReagentDispensing.Contamines

    End Class

End Namespace