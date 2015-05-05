Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Global

Public Class BA200ContaminationsSpecification
    Implements IAnalyzerContaminationsSpecification


    Sub New()

        'This is BA200 dependant:
        AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, "A200").SetDatos

        Dim contaminationPersitance = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

        ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitance, AdditionalPredilutionSteps + contaminationPersitance - 1)

    End Sub

    Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

    Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

    Public Function CreateDispensing() As IReagentDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing
        Return New BA200ReagentDispensing
    End Function

    Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps
End Class
