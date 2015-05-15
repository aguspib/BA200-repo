Imports System.Data.Common
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations


    Public Class ContaminationsContext
        Implements IContaminationsContext




        'Public Property Steps As RangedCollection(Of ContextStep) Implements IContaminationsContext.Steps
        Public ReadOnly ContaminationsSpecifications As IAnalyzerContaminationsSpecification

        Sub New(contaminationsSpecifications As IAnalyzerContaminationsSpecification)

            Me.ContaminationsSpecifications = contaminationsSpecifications
            Dim range = contaminationsSpecifications.ContaminationsContextRange

            Steps = New RangedCollection(Of IContextStep)(range)

            Steps.AllowOutOfRange = False
            For i = range.Minimum To range.Maximum
                Steps.Append(New ContextStep(contaminationsSpecifications.DispensesPerStep)) 'Cantidad máxima de reactivos que se pueden dispensar por ciclo
            Next

        End Sub

        Public Function ActionRequiredForDispensing(dispensing As IReagentDispensing) As ActionRequiredForDispensing Implements IContaminationsContext.ActionRequiredForDispensing

            Dim lookUpFilter As New HashSet(Of String)
            Dim results As New ActionRequiredForDispensing 'New List(Of IWashingDescription)

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                If Steps(curStep) Is Nothing Then Continue For
                For curDispensing = 1 To ContaminationsSpecifications.DispensesPerStep

                    If curDispensing > 1 Then Exit For
                    If Steps(curStep)(curDispensing) Is Nothing Then Continue For
                    Dim dispensingToAsk = Steps(curStep)(curDispensing)
                    Dim requiredWashing = dispensingToAsk.RequiredWashingOrSkip(dispensing, curStep)
                    If requiredWashing.Action = IContaminationsAction.RequiredAction.Wash Then
                        If requiredWashing.InvolvedWash.WashingStrength > 0 AndAlso lookUpFilter.Contains(requiredWashing.InvolvedWash.WashingSolutionID) = False Then
                            lookUpFilter.Add(requiredWashing.InvolvedWash.WashingSolutionID)
                            results.InvolvedWashes.Add(requiredWashing.InvolvedWash)
                        End If
                    ElseIf requiredWashing.Action = IContaminationsAction.RequiredAction.Skip Then
                        results.Action = IContaminationsAction.RequiredAction.Skip
                        Exit For
                    End If
                Next
            Next

            Return results

        End Function

        Public Sub FillContentsFromAnalyzer(instructionParameters As LAx00Frame)
            AnalyzerFrame = instructionParameters
            FillSteps()
        End Sub

        Public Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String) Implements IContaminationsContext.FillContentsFromAnalyzer
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

        Public Sub FillContextInRunning(executions As ExecutionsDS)
            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To ContaminationsSpecifications.DispensesPerStep

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
        Public Sub FillContextInStatic(expectedExecutions As ExecutionsDS) Implements IContaminationsContext.FillContextInStatic
            Dim Lista As New List(Of ExecutionsDS.twksWSExecutionsRow)
            For Each item In expectedExecutions.twksWSExecutions
                Lista.Add(item)
            Next
            FillContextInStatic(Lista)
        End Sub

        Public Sub FillContextInStatic(executionsList As List(Of ExecutionsDS.twksWSExecutionsRow)) Implements IContaminationsContext.FillContextInStatic
            'We fill all Steps and ContexttStep collections with data:
            Dim maxIndex = executionsList.Count - 1

            For j As Integer = Me.Steps.Range.Minimum To Me.Steps.Range.Maximum ' Each S In Steps
                If Steps(j) Is Nothing Then Steps(j) = New ContextStep(ContaminationsSpecifications.DispensesPerStep)
                For i As Integer = 1 To ContaminationsSpecifications.DispensesPerStep
                    If Steps(j)(i) Is Nothing Then Steps(j)(i) = (ContaminationsSpecifications.CreateDispensing())
                Next
            Next
            'We update the already filled data:
            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                Const curDispense = 1 'We only fill R1 in static mode!
                Dim dispense = Steps(curStep)(curDispense)
                If dispense Is Nothing Then Continue For

                'All "before" curStep indexes are negative, so it simplifies calculation. Add them to maxiumum and get proper "before" index.
                Dim curIndex = maxIndex + curStep
                If curIndex >= 0 AndAlso curIndex <= maxIndex Then
                    Dim row = executionsList(curIndex)
                    dispense.FillDispense(ContaminationsSpecifications, row)
                End If
            Next

        End Sub





#Region "Private elements"
        Dim AnalyzerFrame As LAx00Frame

        Private Sub FillSteps()

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To ContaminationsSpecifications.DispensesPerStep

                    If curStep < 0 Then   'Before step(s).
                        If Steps(curStep)(curDispense) Is Nothing Then Steps(curStep)(curDispense) = ContaminationsSpecifications.CreateDispensing()
                        Dim parameterName = String.Format("R{0}B{1}", curDispense, Math.Abs(curStep))
                        If AnalyzerFrame.KeysCollection.Contains(parameterName) Then
                            Steps(curStep)(curDispense).ExecutionID = CInt(AnalyzerFrame(parameterName))
                        End If
                    ElseIf curStep = 0 Then   'Current step

                    ElseIf curStep > 0 Then   'After step(s)
                        If Steps(curStep)(curDispense) Is Nothing Then Steps(curStep)(curDispense) = ContaminationsSpecifications.CreateDispensing()
                        Dim parameterName = String.Format("R{0}A{1}", curDispense, curStep)
                        If AnalyzerFrame.KeysCollection.Contains(parameterName) Then
                            Steps(curStep)(curDispense).ExecutionID = CInt(AnalyzerFrame(parameterName))
                        End If
                    End If
                Next
            Next
        End Sub

#End Region

        Public Function ActionRequiredForAGivenDispensing(ReagentID As ExecutionsDS.twksWSExecutionsRow) As ActionRequiredForDispensing Implements IContaminationsContext.ActionRequiredForDispensing
            Dim D As New ReagentDispensing()
            D.FillDispense(ContaminationsSpecifications, ReagentID)
            Return ActionRequiredForDispensing(D)

        End Function


        Public Property Steps As RangedCollection(Of IContextStep) Implements IContaminationsContext.Steps
    End Class

End Namespace