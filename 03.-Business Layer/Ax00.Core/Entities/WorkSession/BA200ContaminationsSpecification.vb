Imports System.Collections.Concurrent
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class BA200ContaminationsSpecification
        Implements IAnalyzerContaminationsSpecification



        Sub New()

            'This is BA200 dependant:
            AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, "A200").SetDatos

            Dim contaminationPersitance = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

            ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitance, AdditionalPredilutionSteps + contaminationPersitance - 1)

            DispensesPerStep = 2    'The analyzer has R1 and R2
        End Sub

        Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

        Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

        Public Function CreateDispensing() As IReagentDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing
            Return New ReagentDispensing
        End Function

        Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps

        Public Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode Implements IAnalyzerContaminationsSpecification.GetAnalysisModeForReagent

            <ThreadStatic> Static testReagentsDataDS As TestReagentsDS

            Static catchedReagents As New ConcurrentDictionary(Of Integer, AnalysisMode)

            Dim byRefValue As AnalysisMode = AnalysisMode.MonoReactive 'Dummy value to be able to pass the var as reference to the TrygetValue
            If catchedReagents.TryGetValue(reagentID, byRefValue) = True Then Return byRefValue

            Try
                If testReagentsDataDS Is Nothing Then
                    testReagentsDataDS = GetAllReagents()
                End If

                Dim result = (From a In testReagentsDataDS.tparTestReagents
                              Where a.ReagentID = reagentID Select a.ReagentsNumber).First

                Dim mode = CType(result, AnalysisMode)
                catchedReagents.TryAdd(reagentID, mode)
                Return mode

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                Throw
            End Try


        End Function

        Private Function GetAllReagents() As TestReagentsDS

            Dim TestReagentsDAO As New tparTestReagentsDAO()
            Static testReagentsDataDS As TestReagentsDS
            Dim resultData As TypedGlobalDataTo(Of TestReagentsDS)

            If testReagentsDataDS Is Nothing Then
                resultData = TestReagentsDAO.GetAllReagents(Nothing)

                If Not resultData.HasError Then
                    testReagentsDataDS = resultData.SetDatos
                End If
            End If

            Return testReagentsDataDS
        End Function
    End Class
End Namespace
