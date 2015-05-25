Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DataAccess
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class Ax00Dispensing
        Implements IDispensing


        Public Function RequiredActionForDispensing(dispensing As IDispensing, scope As Integer, reagentNumber As Integer) As IContaminationsAction Implements IDispensing.RequiredActionForDispensing

            Select Case KindOfLiquid
                Case IDispensing.KindOfDispensedLiquid.Dummy

                    Return New ContaminationsAction With {.Action = IContaminationsAction.RequiredAction.NoAction}

                Case IDispensing.KindOfDispensedLiquid.Ise, IDispensing.KindOfDispensedLiquid.Reagent
                    Return ReagentRequiresWashingOrSkip(scope, dispensing, reagentNumber)

                Case IDispensing.KindOfDispensedLiquid.Washing
                    If reagentNumber = 1 Then
                        Dim contaAction = New ContaminationsAction
                        contaAction.Action = IContaminationsAction.RequiredAction.RemoveRequiredWashing
                        Dim a = Me.WashingID
                        Dim WashingSolutionID As String = "" 'TODO: Get washing solution String ID from WashingID
                        contaAction.InvolvedWash = New WashingDescription(-1, WashingSolutionID)
                        Return contaAction
                    Else
                        Return New ContaminationsAction With {.Action = IContaminationsAction.RequiredAction.NoAction}

                    End If
                Case Else
                    Return Nothing

            End Select

        End Function

        Private Function ReagentRequiresWashingOrSkip(scope As Integer, dispensing As IDispensing, reagentNumber As Integer) As IContaminationsAction

            'Scope indicates the distance in cycles with the reagent that is asking us if we contaminate it.
            'A negative Scope value indicates we're BEFORE the dispensing we're checking. (contamiantions can be solved with washing)
            'A possitive Scope value indicates we're AFTER the dispensing we're checking. (contaminations can be prevented with Skip)

            If scope = 0 Then   'A reagent can't contamine itself
                Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}

                'Miramos si el dispensing nos contaminará.
            ElseIf scope > 0 Then
                Dim result = dispensing.RequiredActionForDispensing(Me, -scope, reagentNumber)
                If result.Action = IContaminationsAction.RequiredAction.Wash Then
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
                ElseIf result.Action = IContaminationsAction.RequiredAction.Skip Then
                    'Back to future?? --> this is impossible
#If Config = "Debug" Then
                    Throw New Exception("Skip can't be required to solve a contamination of a BEFORE reagent. Happy debugging!")
#Else
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
#End If
                Else
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}
                End If

                'Miramos si contaminamos al dispensing:
            ElseIf Contamines Is Nothing OrElse Contamines.ContainsKey(dispensing.R1ReagentID) = False Then
                Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}

            Else

                'Contaminamos al dispnesing, así que miramos si es necesario o no lavar:
                Dim washing = Contamines(dispensing.R1ReagentID)

                'Si es TEST miramos persistencia para decidir si el washing es necesario
                If dispensing.DelayCyclesForDispensing = 0 AndAlso washing.RequiredWashing.WashingStrength < Math.Abs(scope) Then
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.NoAction}

                    'Si es PTEST, la persistencia nos da igual, ya que los dummis no limpian, por lo cual, lavamos siempre:
                Else
                    Dim newCleaning = New WashingDescription(washing.RequiredWashing.WashingStrength, washing.RequiredWashing.WashingSolutionID)
                    Return New ContaminationsAction() With {.Action = IContaminationsAction.RequiredAction.Wash, .InvolvedWash = newCleaning}

                End If
            End If
        End Function

        Public ReadOnly Property AnalysisMode As Integer Implements IDispensing.AnalysisMode
            Get
                Return _analysisMode
            End Get
        End Property

        Public ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IDispensing.Contamines
            Get
                Return _contamines
            End Get
        End Property

        Dim _r1ReagentId As Integer, _analysisMode As Integer, _contamines As Dictionary(Of Integer, IDispensingContaminationDescription)

        Public Property R1ReagentID As Integer Implements IDispensing.R1ReagentID
            Get
                Return _r1ReagentId
            End Get
            Set(value As Integer)
                If _r1ReagentId <> value Then
                    _r1ReagentId = value
                    _analysisMode = ContaminationsSpecification.GetAnalysisModeForReagent(_r1ReagentId)
                    FillContaminations()
                    'GET IF IT ISE
                    'GET IF IT IS PTEST
                End If
            End Set
        End Property

        Public ReadOnly Property ContaminationsSpecification As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsSpecification
            End Get
        End Property

        'Public Property ReagentNumber As Integer Implements IDispensing.ReagentNumber

        Dim _executionID As Integer
        Public Property ExecutionID As Integer Implements IDispensing.ExecutionID
            Get
                Return _executionID
            End Get
            Set(value As Integer)
                _executionID = value
                Dim aux = New DataAccess.vWSExecutionsDAO()
                If aux IsNot Nothing Then
                    Dim resultDS = aux.GetInfoExecutionByExecutionID(_executionID)
                    If resultDS IsNot Nothing AndAlso resultDS.vWSExecutionsSELECT.Any Then
                        Dim result = resultDS.vWSExecutionsSELECT(0)
                        R1ReagentID = result.ReagentID
                        OrderTestID = result.OrderTestID
                        SampleClass = result.SampleClass
                        Dim predilutionMode = If(result.IsPredilutionModeNull, "", result.PredilutionMode)
                        DelayCyclesForDispensing = If(predilutionMode = "INST", WSExecutionCreator.Instance.ContaminationsSpecification.AdditionalPredilutionSteps - 1, 0)
                    End If
                End If
            End Set
        End Property

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

        Public Property KindOfLiquid As IDispensing.KindOfDispensedLiquid Implements IDispensing.KindOfLiquid

        Public Property DelayCyclesForDispensing As Integer Implements IDispensing.DelayCyclesForDispensing

        Public Property OrderTestID As Integer Implements IDispensing.OrderTestID

        Public Property SampleClass As String Implements IDispensing.SampleClass

        Public Property TestID As Integer Implements IDispensing.TestID

        Public Sub FillDispense(analyzerContaminationsSpecification As IAnalyzerContaminationsSpecification, ByVal row As twksWSExecutionsRow) Implements IDispensing.FillDispense

            If Not row.IsReagentIDNull Then R1ReagentID = row.ReagentID
            If Not row.IsSampleClassNull Then SampleClass = row.SampleClass
            If Not row.IsOrderTestIDNull Then OrderTestID = row.OrderTestID
            If Not row.IsTestIDNull Then TestID = row.TestID

            Dim pTestMode = tparTestSamplesDAO.GetPredilutionModeForTest(R1ReagentID, row.SampleType)

            If String.CompareOrdinal(pTestMode, "INST") = 0 AndAlso String.CompareOrdinal(SampleClass, "PATIENT") = 0 Then
                DelayCyclesForDispensing = analyzerContaminationsSpecification.AdditionalPredilutionSteps - 1
                Debug.WriteLine("ExecutionID:" & ExecutionID & " SampleClass:" & SampleClass & " OrderTestID:" & OrderTestID & " R1Reagent:" & R1ReagentID & " is a predilution.")

            End If

            If row.IsExecutionTypeNull = False Then
                Select Case row.ExecutionType
                    Case "PREP_STD", "", Nothing
                        KindOfLiquid = IDispensing.KindOfDispensedLiquid.Reagent
                    Case "PREP_ISE"
                        KindOfLiquid = IDispensing.KindOfDispensedLiquid.Ise
                    Case Else
#If config = "Debug" Then
                        Throw New Exception("Found preparation with unknown execution type: """ & row.ExecutionType & """. Happy debugging!")
#End If
                End Select
            End If
        End Sub

        Dim _washingID As Integer = -1
        Public Property WashingID As Integer Implements IDispensing.WashingID
            Get
                Return _washingID
            End Get
            Set(value As Integer)
                _washingID = value
                'TODO: GET WASHING DATA FROM ID
                KindOfLiquid = IDispensing.KindOfDispensedLiquid.Washing
                Dim myDao = New vWSExecutionsDAO()
                Dim data = myDao.GetInfoExecutionByExecutionID(value)

            End Set
        End Property
    End Class

End Namespace