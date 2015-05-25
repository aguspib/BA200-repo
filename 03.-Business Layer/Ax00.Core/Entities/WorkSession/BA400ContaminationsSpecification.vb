Imports Biosystems.Ax00.BL
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

        Public Function CreateDispensing() As IDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing
            Return New Ax00Dispensing
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

        Dim _currentContext As ContaminationsContext

        Public Sub FillContextFromAnayzerData(instruction As IEnumerable(Of InstructionParameterTO)) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData

            Dim analyzerFrame = New LAx00Frame(instruction)
            If analyzerFrame("R") = "1" Then
                Dim context = New ContaminationsContext(Me)
                context.FillContentsFromAnalyzer(analyzerFrame)
                _currentContext = context
                Debug.WriteLine("Context filled in running! ")
            End If

        End Sub

        Public ReadOnly Property CurrentRunningContext As IContaminationsContext Implements IAnalyzerContaminationsSpecification.CurrentRunningContext
            Get
                Return _currentContext
            End Get
        End Property

        Public ReadOnly Property HighContaminationPersistence As Integer Implements IAnalyzerContaminationsSpecification.HighContaminationPersistence
            Get
                <ThreadStatic>
                Static highContaminationPersitance As Integer = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

                Return highContaminationPersitance
            End Get
        End Property


        Public Sub FillContextFromAnayzerData1(instruction As String) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
            Dim analyzerFrame = New LAx00Frame()
            Try
                analyzerFrame.ParseRawData(instruction)
                If analyzerFrame("R") = "1" Then
                    Dim context = New ContaminationsContext(Me)
                    context.FillContentsFromAnalyzer(analyzerFrame)
                    _currentContext = context
                    Debug.WriteLine("Context filled in running! ")
                End If
            Catch
            End Try

        End Sub
    End Class
End Namespace
