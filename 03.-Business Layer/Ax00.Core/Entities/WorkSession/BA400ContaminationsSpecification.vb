﻿Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class BA400ContaminationsSpecification
        Implements IAnalyzerContaminationsSpecification


        Sub New()

            'This is B4200 dependant:
            AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, AnalyzerModel).SetDatos

            Dim contaminationPersitence = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

            ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitence, +contaminationPersitence)

        End Sub

        Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

        Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

        Public Function CreateDispensing() As IReagentDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing
            Return New ReagentDispensing
        End Function

        Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps

        Public Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode Implements IAnalyzerContaminationsSpecification.GetAnalysisModeForReagent
            Return AnalysisMode.MonoReactive
        End Function

        Public Function AnalysisModesAreCompatible(current As AnalysisMode, expected As AnalysisMode) As Boolean Implements IAnalyzerContaminationsSpecification.AreAnalysisModesCompatible
            Return True
        End Function

        Public Function RequiredAnalysisModeBetweenReactions(contaminator As AnalysisMode, contamined As AnalysisMode) As AnalysisMode Implements IAnalyzerContaminationsSpecification.RequiredAnalysisModeBetweenReactions
            Return AnalysisMode.MonoReactive
        End Function

        Public ReadOnly Property AnalyzerModel As String Implements IAnalyzerContaminationsSpecification.AnalyzerModel
            Get
                Return Enums.AnalyzerModelEnum.A400.ToString
            End Get
        End Property
    End Class
End Namespace
