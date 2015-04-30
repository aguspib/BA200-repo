Imports Biosystems.Ax00.CC

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ContaminationsContext

        Public ReadOnly Steps As RangedCollection(Of ContextStep)
        Public ReadOnly AnalyzerContaminationsHandler As IAnalyzerContaminationsSpecification

        Sub New(contaminationDesign As IAnalyzerContaminationsSpecification)

            AnalyzerContaminationsHandler = contaminationDesign
            Dim range = contaminationDesign.ContaminationsContextRange

            Steps = New RangedCollection(Of ContextStep)(range)

            Steps.AllowOutOfRange = False
            For i = range.Minimum To range.Maximum
                Steps.Add(New ContextStep(contaminationDesign.DispensesPerStep)) 'Cantidad máxima de reactivos que se pueden dispensar por ciclo
            Next
        End Sub

        Public Sub FillContentsFromAnalyzer(instructionParameters As IEnumerable(Of InstructionParameterTO))
            AnalyzerFrame = New LAx00Frame(instructionParameters)
            FillSteps()
        End Sub

        Public Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String)
            AnalyzerFrame = New LAx00Frame()
            AnalyzerFrame.ParseRawData(rawAnalyzerFrame)
            FillSteps()
        End Sub

#Region "Private elements"
        Dim AnalyzerFrame As LAx00Frame

        Private Sub FillSteps()

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To AnalyzerContaminationsHandler.DispensesPerStep

                    If curStep < 0 Then   'Before step(s).
                        Steps(curStep)(curDispense) = AnalyzerContaminationsHandler.DispensingFactory()
                        Dim parameterName = String.Format("R{0}B{1}", curDispense, Math.Abs(curStep))
                        Steps(curStep)(curDispense).ReagentNumber = CInt(AnalyzerFrame(parameterName))

                    ElseIf curStep = 0 Then   'Current step

                    ElseIf curStep > 0 Then   'After step(s)
                        Steps(curStep)(curDispense) = AnalyzerContaminationsHandler.DispensingFactory()
                        Dim parameterName = String.Format("R{0}A{1}", curDispense, curStep)
                        Steps(curStep)(curDispense).ReagentNumber = CInt(AnalyzerFrame(parameterName))

                    End If
                Next
            Next
        End Sub
#End Region

    End Class

End Namespace