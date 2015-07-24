Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications
    Public MustInherit Class Ax00ContaminationsSpecification
        Implements IAnalyzerContaminationsSpecification



#Region "Public properties"
        Public Property AdditionalPredilutionSteps As Integer Implements IAnalyzerContaminationsSpecification.AdditionalPredilutionSteps

        Public Property ContaminationsContextRange As Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange

        Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep

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

#End Region

#Region "Constructor"
        Sub New(analyzer As IAnalyzerManager)

            AdditionalPredilutionSteps = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.PREDILUTION_CYCLES, analyzer.Model).SetDatos ' AnalyzerModel).SetDatos

            Dim contaminationPersitance = SwParametersDelegate.ReadIntValue(Nothing, GlobalEnumerates.SwParameters.CONTAMIN_REAGENT_PERSIS, Nothing).SetDatos

            ContaminationsContextRange = New Range(Of Integer)(-contaminationPersitance, AdditionalPredilutionSteps + contaminationPersitance - 1)

            DispensesPerStep = 2    'The analyzer has R1 and R2

        End Sub
#End Region

#Region "Abstract methods"

        Public MustOverride Function CreateDispensing() As IDispensing Implements IAnalyzerContaminationsSpecification.CreateDispensing

        Public MustOverride Function GetAnalysisModeForReagent(reagentID As Integer) As AnalysisMode Implements IAnalyzerContaminationsSpecification.GetAnalysisModeForReagent

#End Region

#Region "Runtime Analyzer interaction"

        Public Overridable Sub FillContextFromAnayzerData(instruction As String) Implements IAnalyzerContaminationsSpecification.FillContextFromAnayzerData
            Dim analyzerFrame = New LAx00Frame()
            Try
                analyzerFrame.ParseRawData(instruction)
                If analyzerFrame.KeysCollection.Contains("STATUS") AndAlso analyzerFrame("R") = "1" Then
                    Dim context = New Context.Context(Me)
                    context.FillContentsFromAnalyzer(analyzerFrame)
                    currentContext = context
                    Debug.WriteLine("Context filled in running! " & Mid(instruction, InStr(instruction, "R2B2:")))
                    OnContextRequestProcessed()
                End If
            Catch ex As Exception
                Debug.WriteLine("EXCEPTION FILLING CONTEXT " & ex.Message)
                GlobalBase.CreateLogActivity(ex)
            End Try

        End Sub

#End Region

#Region "Private members"
        Protected currentContext As Context.Context

        Protected Overridable Sub OnContextRequestProcessed()

        End Sub
#End Region

        Public MustOverride Function GetActionRequiredInRunning(dispensing As IDispensing) As IActionRequiredForDispensing Implements IAnalyzerContaminationsSpecification.GetActionRequiredInRunning

        Public Function ConvertRowToDispensing(row As Types.ExecutionsDS.twksWSExecutionsRow) As IDispensing Implements IAnalyzerContaminationsSpecification.ConvertRowToDispensing
            Dim disp = CreateDispensing()
            disp.R1ReagentID = row.ReagentID
            Return disp
        End Function
    End Class
End Namespace