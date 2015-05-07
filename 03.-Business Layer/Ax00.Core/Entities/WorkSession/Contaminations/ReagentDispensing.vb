Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
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

        Public ReadOnly Property AnalysisMode As Integer Implements IReagentDispensing.AnalysisMode
            Get
                Return _analysisMode
            End Get
        End Property

        Public ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IReagentDispensing.Contamines
            Get
                Return _contamines
            End Get
        End Property

        Dim _r1ReagentID As Integer, _analysisMode As Integer, _contamines As Dictionary(Of Integer, IDispensingContaminationDescription)


        Public Property R1ReagentID As Integer Implements IReagentDispensing.R1ReagentID
            Get
                Return _r1ReagentID
            End Get
            Set(value As Integer)
                If _r1ReagentID <> value Then
                    _r1ReagentID = value
                    _analysisMode = ContaminationsDescriptor.GetAnalysisModeForReagent(_r1ReagentID)
                    FillContaminations()
                End If
            End Set
        End Property

        Public ReadOnly Property ContaminationsDescriptor As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsDescriptor
            End Get
        End Property

        Public Property ReagentNumber As Integer Implements IReagentDispensing.ReagentNumber

        Public Property ExecutionID As Integer Implements IReagentDispensing.ExecutionID

        Private Sub FillContaminations()
            _contamines = New Dictionary(Of Integer, IDispensingContaminationDescription)()
            Dim contaminations = tparContaminationsDAO.GetAllContaminationsForAReagent(R1ReagentID)
            For Each contamination In contaminations.SetDatos
                If contamination.ContaminationType <> "R1" Then Continue For

                Dim description = New DispensingContaminationDescription()
                description.ContaminedReagent = contamination.ReagentContaminatedID
                If contamination.IsWashingSolutionR1Null Then
                    description.RequiredWashing = New RegularWaterWashing
                Else
                    description.RequiredWashing = New WashingDescription(Math.Abs(ContaminationsDescriptor.ContaminationsContextRange.Minimum), contamination.WashingSolutionR1)
                End If

                _contamines.Add(contamination.ReagentContaminatedID, description)
            Next
        End Sub

    End Class

End Namespace