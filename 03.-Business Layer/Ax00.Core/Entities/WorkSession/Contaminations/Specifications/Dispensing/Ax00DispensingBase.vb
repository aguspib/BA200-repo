Imports System.Xml.Serialization
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Data
Imports Biosystems.Ax00.Data.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Types.ExecutionsDS

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
    <Serializable()>
    Public MustInherit Class Ax00DispensingBase
        Implements IDispensing

#Region "Unit Testing decoupling elements"

        'This will allow for data mocking in test situations, as we can inject another data source different from our DAL.
        'This is in fact a pointer that is redirected to a mocking factory when we're running a test.
        'On regular application usage, it points to the expected tparContaminationsDAO shared function.
        <XmlIgnore()>
        Protected GetAllContaminationsForAReagent As  _
            Func(Of Integer, TypedGlobalDataTo(Of EnumerableRowCollection(Of ContaminationsDS.tparContaminationsRow))) =
            AddressOf tparContaminationsDAO.GetAllContaminationsForAReagent

        'this is inferred dependency injection that by default points the the corresponding DAO object, but this is redirected on testing.
        <XmlIgnore()>
        Public WSExecutionsDAO As IvWSExecutionsDAO = New vWSExecutionsDAO()

#End Region

#Region "Public members"

        Public Overridable Function RequiredActionForDispensing(targetDispensing As IDispensing, stepIndex As Integer, dispensingNumber As Integer) As IContaminationsAction Implements IDispensing.RequiredActionForDispensing

            Select Case KindOfLiquid
                Case KindOfDispensedLiquid.Dummy

                    Return New RequiredAction With {.Action = IContaminationsAction.RequiredAction.GoAhead}

                Case KindOfDispensedLiquid.Ise, KindOfDispensedLiquid.Reagent
                    Return ReagentRequiresWashingOrSkip(stepIndex, targetDispensing, dispensingNumber)

                Case KindOfDispensedLiquid.Washing
                    If dispensingNumber = 1 Then
                        Dim contaAction = New RequiredAction
                        contaAction.Action = IContaminationsAction.RequiredAction.RemoveRequiredWashing
                        Dim a = WashingID

                        Dim WashingSolutionID As String = _washingDescription.WashingSolutionCode
                        contaAction.InvolvedWash = New WashingDescription(-1, WashingSolutionID)
                        Return contaAction
                    Else
                        Return New RequiredAction With {.Action = IContaminationsAction.RequiredAction.GoAhead}

                    End If
                Case Else
                    Return Nothing

            End Select

        End Function

        <XmlIgnore()>
        Public ReadOnly Property AnalysisMode As Integer Implements IDispensing.AnalysisMode
            Get
                Return _analysisMode
            End Get
        End Property

        <XmlIgnore()>
        Public ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription) Implements IDispensing.Contamines
            Get
                Return _contamines
            End Get
        End Property

        <XmlAttribute>
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


        <XmlIgnore()>
        Public ReadOnly Property ContaminationsSpecification As IAnalyzerContaminationsSpecification
            Get
                Return WSExecutionCreator.Instance.ContaminationsSpecification
            End Get
        End Property
        <XmlAttribute>
        Public Property ExecutionID As Integer Implements IDispensing.ExecutionID
            Get
                Return _executionID
            End Get
            Set(value As Integer)
                _executionID = value
                Dim aux = New vWSExecutionsDAO()
                If aux IsNot Nothing Then
                    Dim resultDS = aux.GetInfoExecutionByExecutionID(_executionID)
                    If resultDS IsNot Nothing AndAlso resultDS.vWSExecutionsSELECT.Any Then
                        Dim result = resultDS.vWSExecutionsSELECT(0)
                        R1ReagentID = result.ReagentID
                        SampleClass = result.SampleClass
                        Dim predilutionMode = If(result.IsPredilutionModeNull, "", result.PredilutionMode)
                        _delayCyclesForDispensing = If(predilutionMode = "INST", WSExecutionCreator.Instance.ContaminationsSpecification.AdditionalPredilutionSteps - 1, 0)
                    End If
                End If
            End Set
        End Property

        <XmlAttribute>
        Public Property KindOfLiquid As KindOfDispensedLiquid Implements IDispensing.KindOfLiquid

        <XmlIgnore()>
        Public ReadOnly Property DelayCyclesForDispensing As Integer Implements IDispensing.DelayCyclesForDispensing
            Get
                Return _delayCyclesForDispensing
            End Get
        End Property

        <XmlAttribute>
        Public Property SampleClass As String Implements IDispensing.SampleClass

        Public Overridable Sub FillDispense(analyzerContaminationsSpecification As IAnalyzerContaminationsSpecification, ByVal row As twksWSExecutionsRow) Implements IDispensing.FillDispense
            If row Is Nothing Then Return
            If Not row.IsReagentIDNull Then R1ReagentID = row.ReagentID
            If Not row.IsSampleClassNull Then SampleClass = row.SampleClass

            Dim pTestMode = tparTestSamplesDAO.GetPredilutionModeForTest(R1ReagentID, row.SampleType)

            If String.CompareOrdinal(pTestMode, "INST") = 0 AndAlso String.CompareOrdinal(SampleClass, "PATIENT") = 0 Then
                _delayCyclesForDispensing = analyzerContaminationsSpecification.AdditionalPredilutionSteps - 1
                'Debug.WriteLine("ExecutionID:" & ExecutionID & " SampleClass:" & SampleClass & " OrderTestID:" & OrderTestID & " R1Reagent:" & R1ReagentID & " is a predilution.")

            End If

            If row.IsExecutionTypeNull = False Then
                Select Case row.ExecutionType
                    Case "PREP_STD", "", Nothing
                        KindOfLiquid = KindOfDispensedLiquid.Reagent
                    Case "PREP_ISE"
                        KindOfLiquid = KindOfDispensedLiquid.Ise
                    Case Else
#If config = "Debug" Then
                        Throw New Exception("Found preparation with unknown execution type: """ & row.ExecutionType & """. Happy debugging!")
#End If
                End Select
            End If
        End Sub

        <XmlAttribute>
        Public Overridable Property WashingID As Integer Implements IDispensing.WashingID
            Get
                Return _washingID
            End Get
            Set(value As Integer)
                _washingID = value
                Try
                    If value >= 0 Then
                        KindOfLiquid = KindOfDispensedLiquid.Washing
                        'Dim myDao = WSExecutionsDAO
                        Dim WashingDS = WSExecutionsDAO.GetWashingSolution(_washingID, WSExecutionCreator.Instance.AnalyzerID, WSExecutionCreator.Instance.WorksesionID)
                        If WashingDS.WashingSolutionSELECT(0).IsSOLUTIONCODENull() OrElse WashingDS.WashingSolutionSELECT(0).SOLUTIONCODE = String.Empty Then
                            Me._washingDescription = New WashingDescription(1, Context.WashingDescription.RegularWaterWashingID)

                        Else
                            Me._washingDescription = New WashingDescription(2, WashingDS.WashingSolutionSELECT(0).SOLUTIONCODE)
                            If Me._washingDescription.WashingSolutionCode = Context.WashingDescription.RegularWaterWashingID Then
                                Me._washingDescription.WashingStrength = 1
                            End If
                        End If
                        Debug.WriteLine("Found washing of kind $$<<" & _washingDescription.WashingSolutionCode & ">>$$ ID= " & WashingID)
                    End If
                Catch _exception As Exception
                    GlobalBase.CreateLogActivity(_exception)
                End Try
            End Set
        End Property

        Protected _washingDescription As IWashingDescription
        <XmlIgnore>
        Public Overridable ReadOnly Property WashingDescription As IWashingDescription Implements IDispensing.WashingDescription
            Get
                Return _washingDescription
            End Get
        End Property

#End Region

#Region "Protected overridable members"
        Protected Overridable Function ReagentRequiresWashingOrSkip(scope As Integer, dispensing As IDispensing, reagentNumber As Integer) As IContaminationsAction

            'Scope indicates the distance in cycles with the reagent that is asking us if we contaminate it.
            'A negative Scope value indicates we're BEFORE the dispensing we're checking. (contamiantions can be solved with washing)
            'A possitive Scope value indicates we're AFTER the dispensing we're checking. (contaminations can be prevented with Skip)

            If scope = 0 Then   'A reagent can't contamine itself
                Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.GoAhead}

                'Miramos si el dispensing nos contaminará.
            ElseIf scope > 0 Then
                Dim result = dispensing.RequiredActionForDispensing(Me, -scope, reagentNumber)
                If result.Action = IContaminationsAction.RequiredAction.Wash Then
                    Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
                ElseIf result.Action = IContaminationsAction.RequiredAction.Skip Then
                    'Back to future?? --> this is impossible
#If Config = "Debug" Then
                    Throw New Exception("Skip can't be required to solve a contamination of a BEFORE reagent. Happy debugging!")
#Else
                    Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.Skip}
#End If
                Else
                    Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.GoAhead}
                End If

                'Miramos si contaminamos al dispensing:
            ElseIf Contamines Is Nothing OrElse Contamines.ContainsKey(dispensing.R1ReagentID) = False Then
                Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.GoAhead}

            Else

                'Contaminamos al dispnesing, así que miramos si es necesario o no lavar:
                Dim washing = Contamines(dispensing.R1ReagentID)

                'Si es TEST miramos persistencia para decidir si el washing es necesario
                If dispensing.DelayCyclesForDispensing = 0 AndAlso washing.RequiredWashing.WashingStrength < Math.Abs(scope) Then
                    Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.GoAhead}

                    'Si es PTEST, la persistencia nos da igual, ya que los dummis no limpian, por lo cual, lavamos siempre:
                Else
                    Dim newCleaning = New WashingDescription(washing.RequiredWashing.WashingStrength, washing.RequiredWashing.WashingSolutionCode)
                    Return New RequiredAction() With {.Action = IContaminationsAction.RequiredAction.Wash, .InvolvedWash = newCleaning}

                End If
            End If
        End Function

        Overrides Function ToString() As String
            Try
                Select Case KindOfLiquid
                    Case KindOfDispensedLiquid.Dummy
                        Return "Dummy " & Me.GetType.Name

                    Case KindOfDispensedLiquid.Reagent
                        Return "Reagent ID:" & R1ReagentID & " SC " & Me.SampleClass & " " & Me.GetType.Name

                    Case KindOfDispensedLiquid.Washing
                        Return "Washing ID:W" & WashingID & " " & Me._washingDescription.WashingSolutionCode & " Strength " & Me._washingDescription.WashingStrength & " " & Me.GetType.Name

                    Case Else
                        Return MyBase.ToString()

                End Select
            Catch
                Return MyBase.ToString
            End Try
        End Function
#End Region

#Region "attributes"
        Private _washingID As Integer = -1
        Private _executionID As Integer
        Private _r1ReagentId As Integer
        Private _analysisMode As Integer
        Private _contamines As Dictionary(Of Integer, IDispensingContaminationDescription)
        Private _delayCyclesForDispensing As Integer
#End Region

#Region "Private memebers"
        Private Sub FillContaminations()
            _contamines = New Dictionary(Of Integer, IDispensingContaminationDescription)()
            Dim contaminations = GetAllContaminationsForAReagent(R1ReagentID)
            For Each contamination In contaminations.SetDatos
                If contamination.ContaminationType <> "R1" Then Continue For

                Dim description = New DispensingContamination()
                description.ContaminedReagent = contamination.ReagentContaminatedID
                If contamination.IsWashingSolutionR1Null Then
                    description.RequiredWashing = New RegularWaterWashing
                Else
                    description.RequiredWashing = New WashingDescription(Math.Abs(ContaminationsSpecification.ContaminationsContextRange.Minimum), contamination.WashingSolutionR1)
                End If

                _contamines.Add(contamination.ReagentContaminatedID, description)
            Next
        End Sub
#End Region




    End Class

End Namespace