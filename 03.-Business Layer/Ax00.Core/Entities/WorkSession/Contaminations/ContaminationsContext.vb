﻿Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ContaminationsContext

        Public ReadOnly Steps As RangedCollection(Of ContextStep)
        Public ReadOnly AnalyzerContaminationsDescriptor As IAnalyzerContaminationsSpecification

        Sub New(analyzerContaminationsDescriptor As IAnalyzerContaminationsSpecification)

            Me.AnalyzerContaminationsDescriptor = analyzerContaminationsDescriptor
            Dim range = analyzerContaminationsDescriptor.ContaminationsContextRange

            Steps = New RangedCollection(Of ContextStep)(range)

            Steps.AllowOutOfRange = False
            For i = range.Minimum To range.Maximum
                Steps.Append(New ContextStep(analyzerContaminationsDescriptor.DispensesPerStep)) 'Cantidad máxima de reactivos que se pueden dispensar por ciclo
            Next

        End Sub

        Function GetWashingRequiredForAGivenDispensing(dispensing As IReagentDispensing) As WashingDescription

            Dim result As IWashingDescription = New EmptyWashing, requiredWashingPower = 0

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                If Steps(curStep) Is Nothing Then Continue For
                For curDispensing = 1 To AnalyzerContaminationsDescriptor.DispensesPerStep

                    If Steps(curStep)(curDispensing) Is Nothing Then Continue For
                    Dim dispensingToAsk = Steps(curStep)(curDispensing)
                    Dim requiredWashing = dispensingToAsk.RequiredWashingSolution(dispensing, curStep)
                    requiredWashingPower += requiredWashing.WashingStrength
                    If result.WashingStrength < requiredWashing.WashingStrength Then result = requiredWashing

                Next
            Next

            If requiredWashingPower = 0 Then Return New EmptyWashing Else Return New WashingDescription(requiredWashingPower, result.WashingSolutionID)

        End Function

        Public Sub FillContentsFromAnalyzer(instructionParameters As IEnumerable(Of InstructionParameterTO))
            AnalyzerFrame = New LAx00Frame(instructionParameters)
            FillSteps()
        End Sub

        Public Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String)
            AnalyzerFrame = New LAx00Frame()
            AnalyzerFrame.ParseRawData(rawAnalyzerFrame)
            FillSteps()
        End Sub

        ' ReSharper disable once InconsistentNaming
        Public Shared Sub DebugContentsFromExecutionDS(executions As ExecutionsDS, currentIndex As Integer)

            Dim table = executions.twksWSExecutions, count As Integer = 0
            Debug.WriteLine("Listing executions:")
            For Each R In table
                If R.IsExecutionIDNull() Then
                    Debug.Write("Expected_ID = " & count)
                Else
                    Debug.Write("ExecutionID = " & R.ExecutionID)
                End If
                Debug.Write(" type= " & R.ExecutionType)
                If R.IsReagentIDNull Then
                    Debug.Write(" no reagent ")
                Else
                    Debug.Write(" reagent = " & R.ReagentID)
                End If
                Debug.Write(" OrderID = " & R.OrderID)
                Debug.Write(" OrderTestID = " & R.OrderTestID)
                Debug.Write(" PatientID = " & R.PatientID)
                If R.IsPreparationIDNull Then
                    Debug.Write(" PreparationID = <void> ")
                Else
                    Debug.Write(" PreparationID = " & R.PreparationID)
                End If
                Debug.Write(" Class = " & R.SampleClass)
                Debug.Write(" Type = " & R.SampleType)
                Debug.Write(" Name = " & R.TestName)
                Debug.Write(" TType = " & R.TestType)
                If R.IsWellUsedNull Then
                    Debug.Write(" Well = none")
                Else
                    Debug.Write(" Well = " & R.WellUsed)
                End If
                Debug.WriteLine("")
                count += 1
            Next
            Debug.WriteLine("DONE!")
        End Sub

        Sub FillContextInRunning(executions As ExecutionsDS)
            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To AnalyzerContaminationsDescriptor.DispensesPerStep

                    Dim dispensing = Steps(curStep)(curDispense)
                    Dim rows = executions.twksWSExecutions.Where(Function(element As twksWSExecutionsRow) element.ExecutionID = dispensing.ExecutionID)

                    If rows IsNot Nothing And rows.Any() Then
                        Dim row = rows.First()
                        dispensing.R1ReagentID = row.ReagentID
                        'FillDispenseContaminations(dispensing)

                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' fills context from minimum index to 0, using the ExecutionsDS.<Para>This method ONLY fills the first dispensing (R1). </Para>
        ''' </summary>
        ''' <param name="expectedExecutions"></param>
        ''' <remarks></remarks>
        Sub FillContextInStatic(expectedExecutions As ExecutionsDS)
            'We fill all Steps and ContexttStep collections with data:
            For j As Integer = Me.Steps.Range.Minimum To Me.Steps.Range.Maximum ' Each S In Steps
                If Steps(j) Is Nothing Then Steps(j) = New ContextStep(AnalyzerContaminationsDescriptor.DispensesPerStep)
                For i As Integer = 1 To AnalyzerContaminationsDescriptor.DispensesPerStep
                    If Steps(j)(i) Is Nothing Then Steps(j)(i) = (AnalyzerContaminationsDescriptor.CreateDispensing())
                Next
            Next
            'We update the already filled data:
            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                Const curDispense = 1 'We only fill R1 in static mode!
                Dim dispense = Steps(curStep)(curDispense)
                If dispense Is Nothing Then Continue For
                Dim table = expectedExecutions.twksWSExecutions
                Dim maxIndex = table.Count - 1
                Dim curIndex = maxIndex + curStep
                If curIndex <= maxIndex Then
                    Dim row = table(curIndex)
                    dispense.R1ReagentID = row.ReagentID
                    'row.PostDilutionType
                    'row.SampleClass
                    'row.SampleType

                    If row.IsExecutionTypeNull = False Then
                        Select Case row.ExecutionType
                            Case "PREP_STD", "", Nothing
                                dispense.IsISE = False
                            Case "PREP_ISE"
                                dispense.IsISE = True
                            Case Else
#If config = "debug" Then
                            Throw New Exception("Found preparation with unknown execution type: """ & row.executiontype & """. Happy debugging!")
#End If
                        End Select
                    End If
                End If
            Next
        End Sub

#Region "Private elements"
        Dim AnalyzerFrame As LAx00Frame

        Private Sub FillSteps()

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To AnalyzerContaminationsDescriptor.DispensesPerStep

                    If curStep < 0 Then   'Before step(s).
                        Steps(curStep)(curDispense) = AnalyzerContaminationsDescriptor.CreateDispensing()
                        Dim parameterName = String.Format("R{0}B{1}", curDispense, Math.Abs(curStep))
                        Steps(curStep)(curDispense).ExecutionID = CInt(AnalyzerFrame(parameterName))

                    ElseIf curStep = 0 Then   'Current step

                    ElseIf curStep > 0 Then   'After step(s)
                        Steps(curStep)(curDispense) = AnalyzerContaminationsDescriptor.CreateDispensing()
                        Dim parameterName = String.Format("R{0}A{1}", curDispense, curStep)
                        Steps(curStep)(curDispense).ExecutionID = CInt(AnalyzerFrame(parameterName))

                    End If
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace