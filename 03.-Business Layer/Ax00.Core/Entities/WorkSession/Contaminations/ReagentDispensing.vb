Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ReagentDispensing
        Implements IReagentDispensing

        Public Function RequiredWashingSolution(dispensing As IReagentDispensing, scope As Integer) As IContaminationsAction Implements IReagentDispensing.RequiredWashingOrSkip

            If scope = 0 Then   'A reagent can't contamine itself
                Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}


            ElseIf scope > 0 Then
                Dim result = dispensing.RequiredWashingOrSkip(Me, -scope)
                If result.Action = IContaminationsAction.RequiredAction.Wash Then
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
                ElseIf result.Action = IContaminationsAction.RequiredAction.Skip Then
                    'Back to future?? --> this is impossible
#If Configuration = "Debug" Then
                    Throw New Exception("Skip can't be required to solve a contamination of a BEFORE reagent. Happy debugging!")
#Else
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
#End If
                Else
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}
                End If

            ElseIf Contamines Is Nothing OrElse Contamines.ContainsKey(dispensing.R1ReagentID) = False Then
                Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}

            Else

                'Contaminamos al dispnesing, así que miramos si es necesario o no lavar:
                Dim washing = Contamines(dispensing.R1ReagentID)

                'Si no es PTEST miramos persistencia para decidir si el washing es necesario
                If dispensing.DelayCyclesForDispensing = 0 AndAlso washing.RequiredWashing.WashingStrength < Math.Abs(scope) Then
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}

                    'Si es PTEST, la persistencia nos da igual, ya que los dummis no limpian, por lo cual, lavamos siempre:
                Else
                    Dim newCleaning = New WashingDescription(washing.RequiredWashing.WashingStrength, washing.RequiredWashing.WashingSolutionID)
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Wash, .InvolvedWash = newCleaning}

                End If
            End If

        End Function

        Public ReadOnly Property AnalysisMode As Integer Implements IReagentDispensing.AnalysisMode
            Get
                Return _analysisMode
            End Get
        End Property

        Public ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IReagentDispensing.Contamines
            Get
                Return _contamines
            End Get
        End Property

        Dim _r1ReagentID As Integer, _analysisMode As Integer, _contamines As Dictionary(Of Integer, IDispensingContaminationDescription)

        Public Property R1ReagentID As Integer Implements IReagentDispensing.R1ReagentID
            Get
                Return _r1ReagentID
            End Get
            Set(value As Integer)
                If _r1ReagentID <> value Then
                    _r1ReagentID = value
                    _analysisMode = ContaminationsSpecification.GetAnalysisModeForReagent(_r1ReagentID)
                    FillContaminations()
                End If
            End Set
        End Property

        Public ReadOnly Property ContaminationsSpecification As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsSpecification
            End Get
        End Property

        Public Property ReagentNumber As Integer Implements IReagentDispensing.ReagentNumber

        Public Property ExecutionID As Integer Implements IReagentDispensing.ExecutionID

        Private Sub FillContaminations()
            _contamines = New Dictionary(Of Integer, IDispensingContaminationDescription)()
            Dim contaminations = tparContaminationsDAO.GetAllContaminationsForAReagent(R1ReagentID)
            For Each contamination In contaminations.SetDatos
                If contamination.ContaminationType <> "R1" Then Continue For

                Dim description = New DispensingContaminationDescription()
                description.ContaminedReagent = contamination.ReagentContaminatedID
                If contamination.IsWashingSolutionR1Null Then
                    description.RequiredWashing = New RegularWaterWashing
                Else
                    description.RequiredWashing = New WashingDescription(Math.Abs(ContaminationsSpecification.ContaminationsContextRange.Minimum), contamination.WashingSolutionR1)
                End If

                _contamines.Add(contamination.ReagentContaminatedID, description)
            Next
        End Sub

        Public Property IsISE As Boolean Implements IReagentDispensing.IsISE

        Public Property DelayCyclesForDispensing As Integer Implements IReagentDispensing.DelayCyclesForDispensing

        Public Property OrderTestID As Integer Implements IReagentDispensing.OrderTestID

        Public Property SampleClass As String Implements IReagentDispensing.SampleClass

        Public Property TestID As Integer Implements IReagentDispensing.TestID

        Public Sub FillDispense(analyzerContaminationsSpecification As IAnalyzerContaminationsSpecification, ByVal row As twksWSExecutionsRow) Implements IReagentDispensing.FillDispense

            R1ReagentID = row.ReagentID
            SampleClass = row.SampleClass
            OrderTestID = row.OrderTestID
            TestID = row.TestID

            Dim pTestMode = tparTestSamplesDAO.GetPredilutionModeForTest(row.TestID, row.SampleType)

            If String.CompareOrdinal(pTestMode, "INST") = 0 AndAlso String.CompareOrdinal(SampleClass, "PATIENT") = 0 Then
                DelayCyclesForDispensing = analyzerContaminationsSpecification.AdditionalPredilutionSteps - 1
                Debug.WriteLine("ExecutionID:" & ExecutionID & " SampleClass:" & SampleClass & " OrderTestID:" & OrderTestID & " R1Reagent:" & R1ReagentID & " is a predilution.")

            End If

            If row.IsExecutionTypeNull = False Then
                Select Case row.ExecutionType
                    Case "PREP_STD", "", Nothing
                        IsISE = False
                    Case "PREP_ISE"
                        IsISE = True
                    Case Else
#If config = "Debug" Then
                        Throw New Exception("Found preparation with unknown execution type: """ & row.ExecutionType & """. Happy debugging!")
#End If
                End Select
            End If
        End Sub

    End Class

End Namespace