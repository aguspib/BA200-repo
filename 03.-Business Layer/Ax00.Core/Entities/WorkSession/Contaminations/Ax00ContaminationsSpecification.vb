Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports System.Collections.Concurrent
Imports System.Data.Common
Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types
Public MustInherit Class Ax00ContaminationsSpecification
    Implements IAnalyzerContaminationsSpecification

    Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps
    Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

    Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

    Sub New(analyzer As IAnalyzerManager)

        AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, analyzer.Model).SetDatos ' AnalyzerModel).SetDatos

        Dim contaminationPersitance = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

        ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitance, AdditionalPredilutionSteps + contaminationPersitance - 1)

        DispensesPerStep = 2    'The analyzer has R1 and R2

    End Sub

    Public MustOverride Function CreateDispensing() As IDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing

    Public MustOverride Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode Implements IAnalyzerContaminationsSpecification.GetAnalysisModeForReagent

    Public MustOverride Function AreAnalysisModesCompatible(current As AnalysisMode, expected As AnalysisMode) As Boolean Implements IAnalyzerContaminationsSpecification.AreAnalysisModesCompatible

#Region "Private members"
    Protected currentContext As ContaminationsContext
#End Region

    Public Overridable Sub FillContextFromAnayzerData(instruction As IEnumerable(Of InstructionParameterTO)) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
        Dim analyzerFrame = New LAx00Frame(instruction)
        If analyzerFrame("R") = "1" Then
            Dim context = New ContaminationsContext(Me)
            context.FillContentsFromAnalyzer(analyzerFrame)
            currentContext = context
            Debug.WriteLine("Context filled in running! ")
        End If
    End Sub
    Public ReadOnly Property CurrentRunningContext As IContaminationsContext Implements IAnalyzerContaminationsSpecification.CurrentRunningContext
        Get
            Return currentContext
        End Get
    End Property

    Public ReadOnly Property HighContaminationPersistence As Integer Implements IAnalyzerContaminationsSpecification.HighContaminationPersistence
        Get
            <ThreadStatic> Static highContaminationPersitance As Integer = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos
            Return highContaminationPersitance
        End Get
    End Property

    Public Sub FillContextFromAnayzerData(instruction As String) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
        Dim analyzerFrame = New LAx00Frame()
        Try
            analyzerFrame.ParseRawData(instruction)
            If analyzerFrame.KeysCollection.Contains("STATUS") AndAlso analyzerFrame("R") = "1" Then
                Dim context = New ContaminationsContext(Me)
                context.FillContentsFromAnalyzer(analyzerFrame)
                currentContext = context
                Debug.WriteLine("Context filled in running! " & Mid(instruction, InStr(instruction, "R2B2:")))
            End If
        Catch ex As Exception
            Debug.WriteLine("EXCEPTION FILLING CONTEXT " & ex.Message)
            GlobalBase.CreateLogActivity(ex)
        End Try

    End Sub
End Class
