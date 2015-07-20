#If config = "Debug" Then
Imports System.Windows.Forms
#End If
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context


    Public Class Context
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

        Public Function ActionRequiredForDispensing(dispensing As IDispensing) As IActionRequiredForDispensing Implements IContaminationsContext.ActionRequiredForDispensing
#If config = "Debug" Then
            Try
                Dim anali As AnalyzerManager = TryCast(AnalyzerManager.GetCurrentAnalyzerManager(), AnalyzerManager)
                If anali.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    If dispensing.KindOfLiquid = KindOfDispensedLiquid.Reagent Then
                        Debug.WriteLine("  Asked context for reagent " & dispensing.R1ReagentID & " SC:" & dispensing.SampleClass)
                    ElseIf dispensing.KindOfLiquid = KindOfDispensedLiquid.Washing Then
                        Debug.WriteLine("  Asked context for washing " & dispensing.WashingID & " WS:" & dispensing.WashingDescription.WashingSolutionCode)
                    End If
                End If
            Catch : End Try

#End If

            Dim lookUpFilter As New HashSet(Of String)
            Dim results As New ActionRequiredForDispensing 'New List(Of IWashingDescription)

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                If Steps.IsIndexValid(curStep) = False OrElse Steps(curStep) Is Nothing Then Continue For

                For curDispensing = 1 To ContaminationsSpecifications.DispensesPerStep

                    If Steps(curStep) Is Nothing OrElse Steps(curStep)(curDispensing) Is Nothing Then Continue For
                    Dim dispensingToAsk = Steps(curStep)(curDispensing)
                    Dim responseFromDispense = dispensingToAsk.RequiredActionForDispensing(dispensing, curStep, curDispensing)
                    'If curStep = -1 AndAlso Steps(curStep)(1).R1ReagentID = 69 Then
                    '    Dim a = 1
                    'End If
                    Select Case responseFromDispense.Action

                        Case IContaminationsAction.RequiredAction.Wash
                            If responseFromDispense.InvolvedWash.WashingStrength > 0 AndAlso lookUpFilter.Contains(responseFromDispense.InvolvedWash.WashingSolutionCode) = False Then
                                lookUpFilter.Add(responseFromDispense.InvolvedWash.WashingSolutionCode)
                                results.InvolvedWashes.Add(responseFromDispense.InvolvedWash)
                                results.Action = IContaminationsAction.RequiredAction.Wash
                            End If

                        Case IContaminationsAction.RequiredAction.Skip
                            If results.Action <> IContaminationsAction.RequiredAction.Wash Then results.Action = IContaminationsAction.RequiredAction.Skip
                            'Exit For

                        Case IContaminationsAction.RequiredAction.RemoveRequiredWashing
                            RemoveWashingFromResult(responseFromDispense, lookUpFilter, results)
                    End Select

                Next
            Next

            Return results

        End Function

        Public Sub FillContentsFromAnalyzer(instructionParameters As LAx00Frame)
            AnalyzerFrame = instructionParameters
            FillSteps()
            Debug.WriteLine(Me)

        End Sub

        ''' <summary>
        ''' fills context from minimum index to 0, using the ExecutionsDS.<Para>This method ONLY fills the first dispensing (R1). </Para>
        ''' </summary>
        ''' <param name="expectedExecutions"></param>
        ''' <remarks></remarks>
        Public Sub FillContextInStatic(expectedExecutions As ExecutionsDS) Implements IContaminationsContext.FillContextInStatic
            Dim lista As List(Of twksWSExecutionsRow) = expectedExecutions.twksWSExecutions.ToList()
            FillContextInStatic(lista)
        End Sub

        Public Sub FillContextInStatic(executionsList As List(Of ExecutionsDS.twksWSExecutionsRow)) Implements IContaminationsContext.FillContextInStatic
            'We fill all Steps and ContexttStep collections with data:
            Dim maxIndex = executionsList.Count - 1

            FillEmptyContextSteps()

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


        Public Sub FillEmptyContextSteps()
            For j As Integer = Me.Steps.Range.Minimum To Me.Steps.Range.Maximum ' Each S In Steps
                If Steps(j) Is Nothing Then Steps(j) = New ContextStep(ContaminationsSpecifications.DispensesPerStep)
                For i As Integer = 1 To ContaminationsSpecifications.DispensesPerStep
                    If Steps(j)(i) Is Nothing Then Steps(j)(i) = (ContaminationsSpecifications.CreateDispensing())
                Next
            Next
        End Sub

        Public Sub FillContentsFromReagentsIDInStatic(reagentsIDList As List(Of Integer), timeOffset As Integer)
            FillEmptyContextSteps()
            If reagentsIDList Is Nothing Then Return
            'Dim maxIndex = reagentsIDList.Count - 1
            For i2 As Integer = Me.Steps.Range.Minimum To -1
                Dim auxIndex As Integer = i2
                Dim reagentsIndex = reagentsIDList.Count + auxIndex
                auxIndex -= timeOffset

                If auxIndex < Steps.Range.Minimum Then Continue For

                If reagentsIDList.Any AndAlso reagentsIDList.Count > reagentsIndex Then
                    If reagentsIDList.Count > reagentsIndex AndAlso reagentsIndex >= 0 Then
                        If Steps(auxIndex)(1) Is Nothing Then Steps(auxIndex)(1) = Me.ContaminationsSpecifications.CreateDispensing
                        Steps(auxIndex)(1).R1ReagentID = reagentsIDList(reagentsIndex)
                    End If
                End If
            Next

        End Sub


        Public Function ActionRequiredForDispensing(ReagentID As ExecutionsDS.twksWSExecutionsRow) As IActionRequiredForDispensing Implements IContaminationsContext.ActionRequiredForDispensing
            Dim dispense = ContaminationsSpecifications.CreateDispensing() 'As New Ax00Dispensing()
            dispense.FillDispense(ContaminationsSpecifications, ReagentID)
            Return ActionRequiredForDispensing(dispense)

        End Function

        Public Property Steps As RangedCollection(Of IContextStep) Implements IContaminationsContext.Steps

        Public Overrides Function ToString() As String
            Dim SB As New Text.StringBuilder(1024)
            For curDispense As Integer = 1 To ContaminationsSpecifications.DispensesPerStep
                SB.Append("|"c)
                For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                    If Steps(curStep) Is Nothing Then
                        SB.Append("  ?   ")
                    ElseIf Steps(curStep)(curDispense) Is Nothing Then
                        SB.Append("  ?   ")
                    Else
                        Dim dispense = Steps(curStep)(curDispense)
                        Select Case dispense.KindOfLiquid
                            Case KindOfDispensedLiquid.Washing
                                SB.Append("W" & Format(dispense.WashingID, "000"))
                            Case KindOfDispensedLiquid.Reagent
                                SB.Append(" " & Format(dispense.R1ReagentID, "000"))
                            Case KindOfDispensedLiquid.Dummy
                                SB.Append("Dumy")
                            Case Else
                                SB.Append(" ?? ")
                        End Select
                        SB.Append("/" & Format(dispense.ExecutionID, "00") & " "c)
                    End If
                    SB.Append("|"c)
                Next
                SB.Append("     " & vbCr)
            Next
            SB.Append(vbCr)
            Return SB.ToString


        End Function


#Region "Private elements"
        Dim AnalyzerFrame As LAx00Frame


        Public Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String) Implements IContaminationsContext.FillContentsFromAnalyzer
            AnalyzerFrame = New LAx00Frame()
            AnalyzerFrame.ParseRawData(rawAnalyzerFrame)
            FillSteps()
        End Sub

        Private Sub RemoveWashingFromResult(responseFromDispensing As IContaminationsAction, lookUpFilter As HashSet(Of String), results As ActionRequiredForDispensing)
            If responseFromDispensing.Action <> IContaminationsAction.RequiredAction.RemoveRequiredWashing Then Return

            ' ReSharper disable once InconsistentNaming
            Dim washingSolutionCode = responseFromDispensing.InvolvedWash.WashingSolutionCode
            If lookUpFilter.Contains(washingSolutionCode) Then
                Debug.WriteLine("Found and removed washing: <<" & washingSolutionCode & ">>")
                lookUpFilter.Remove(responseFromDispensing.InvolvedWash.WashingSolutionCode)
                Dim washingToRemove = results.InvolvedWashes.FirstOrDefault(Function(wash) wash.WashingSolutionCode = washingSolutionCode)
                If washingToRemove IsNot Nothing AndAlso washingToRemove.WashingSolutionCode <> String.Empty Then
                    results.InvolvedWashes.Remove(washingToRemove)
                End If
                'Else
                '    Debug.WriteLine("ERROR washing not found: <<" & washingSolutionCode & ">>")
                '    Debug.WriteLine("")
                '    Debug.WriteLine(Me)
                '    Debug.WriteLine("")
            End If
        End Sub

        Private Sub FillSteps()

            For curStep = Steps.Range.Minimum To Steps.Range.Maximum
                For curDispense = 1 To ContaminationsSpecifications.DispensesPerStep

                    If curStep < 0 Then   'Before step(s).
                        If Steps(curStep)(curDispense) Is Nothing Then Steps(curStep)(curDispense) = ContaminationsSpecifications.CreateDispensing()
                        Dim parameterName = String.Format("R{0}B{1}", curDispense, Math.Abs(curStep))
                        If AnalyzerFrame.KeysCollection.Contains(parameterName) Then
                            Dim targetStep = Steps(curStep)(curDispense)
                            SetStepValues(parameterName, targetStep, curDispense)
                        Else
                            Debug.WriteLine("Parameter " & parameterName & " not found in frame!")
                        End If
                    ElseIf curStep = 0 Then   'Current step

                    ElseIf curStep > 0 Then   'After step(s)
                        If Steps(curStep)(curDispense) Is Nothing Then Steps(curStep)(curDispense) = ContaminationsSpecifications.CreateDispensing()
                        Dim parameterName = String.Format("R{0}A{1}", curDispense, curStep)
                        If AnalyzerFrame.KeysCollection.Contains(parameterName) Then
                            Dim targetStep = Steps(curStep)(curDispense)
                            SetStepValues(parameterName, targetStep, curDispense)
                        Else
                            ' Debug.WriteLine("Parameter " & parameterName & " not found in frame!")

                        End If
                    End If
                Next
            Next
        End Sub

        Private Sub SetStepValues(parameterName As String, targetDispensing As IDispensing, curDispense As Integer)

            Dim value = AnalyzerFrame(parameterName)
            If String.Equals(value, "D", StringComparison.OrdinalIgnoreCase) Then
                targetDispensing.KindOfLiquid = KindOfDispensedLiquid.Dummy
            ElseIf value.StartsWith("W", StringComparison.OrdinalIgnoreCase) Then
                targetDispensing.WashingID = CInt(Mid(value, 2))
            Else
                targetDispensing.ExecutionID = CInt(value)
            End If
        End Sub

#End Region

    End Class

End Namespace