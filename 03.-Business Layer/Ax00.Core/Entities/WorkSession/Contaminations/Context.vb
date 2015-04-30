Imports System.Text
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Types

Public Class ContaminationsContext

    Public ReadOnly Steps As RangedCollection(Of ContextStep)
    Public ReadOnly AnalyzerContaminationsHandler As AnalyzerContaminationsSpecification

    Sub New(contaminationDesign As AnalyzerContaminationsSpecification)

        AnalyzerContaminationsHandler = contaminationDesign
        Dim range = contaminationDesign.ContaminationsContextRange

        Steps = New RangedCollection(Of ContextStep)(range)

        Steps.AllowOutOfRange = False
        For i = range.minimum To range.maximum
            Steps.Add(New ContextStep(contaminationDesign.DispensesPerStep)) 'Cantidad máxima de reactivos que se pueden dispensar por ciclo
        Next
    End Sub

    Public Sub FillContentsFromAnalyzer(instructionParameters As IEnumerable(Of InstructionParameterTO))
        parsedData = New AnalyzerParsedFrame(instructionParameters)
        FillSteps()
    End Sub

    Public Sub FillContentsFromAnalyzer(rawAnalyzerFrame As String)
        parsedData = New InstrumentFrameParser()
        parsedData.ParseRawData(rawAnalyzerFrame)
        FillSteps()
    End Sub

#Region "Private elements"
    Dim parsedData As InstrumentFrameParser

    Private Sub FillSteps()

        For curStep = Steps.Range.minimum To Steps.Range.maximum
            For curDispense = 1 To AnalyzerContaminationsHandler.DispensesPerStep

                If curStep < 0 Then   'Before step(s).
                    Steps(curStep)(curDispense) = AnalyzerContaminationsHandler.DispensingFactory()
                    Dim parameterName = "R" & curDispense & "B" & Math.Abs(curStep)     'In ex. R2B2, 
                    Steps(curStep)(curDispense).ReagentNumber = CInt(parsedData(parameterName))

                ElseIf curStep = 0 Then   'Current step

                ElseIf curStep > 0 Then   'After step(s)
                    Steps(curStep)(curDispense) = AnalyzerContaminationsHandler.DispensingFactory()
                    Dim parameterName = "R" & curDispense & "A" & curStep     'In ex. R1A1,
                    Steps(curStep)(curDispense).ReagentNumber = CInt(parsedData(parameterName))

                End If
            Next
        Next

    End Sub

#End Region

End Class

Public Class InstrumentFrameParser

#Region "Private attributes"
    Dim _index As Integer = 0
    Dim _frame As String
    Const ParserbufferSize = 1024
    Const SentenceSeparator = ";"c
    Const ValueOperator = ":"c
    <ThreadStatic> Shared buffer As New StringBuilder(ParserbufferSize)
    Protected Parameters As New Dictionary(Of String, String)
#End Region

#Region "Public members"
    Public Sub ParseRawData(Frame As String)

        _frame = Frame

        buffer.Clear()

        'Instruction = ParseSentence()

        Parameters = ParseParameters()

    End Sub

    Public ReadOnly Property KeysCollection() As IEnumerable(Of String)
        Get
            Return Parameters.Keys
        End Get
    End Property

    Public ReadOnly Property ValuesCollection() As IEnumerable(Of String)
        Get
            Return Parameters.Values
        End Get
    End Property

    Public Function ValueByIndex(index As Integer) As String
        Return Parameters.ElementAt(index).Value
    End Function

    Public Function KeyByIndex(index As Integer) As String
        Return Parameters.ElementAt(index).Key
    End Function

    Default Public Property Item(key As String) As String
        Get
            Return Parameters(key)
        End Get
        Set(value As String)
            Parameters(key) = value
        End Set
    End Property
#End Region

#Region "Private members"
    Private Function ParseParameters() As Dictionary(Of String, String)
        Dim dic As New Dictionary(Of String, String)
        While _index < _frame.Length
            'Dim sentence = ParseSentence()
            Dim KV = TokenizeSentence()
            dic.Add(KV.Key, KV.Value)
        End While
        Return dic

    End Function

    Private Function TokenizeSentence() As KeyValuePair(Of String, String)
        Dim paramName As String = String.Empty, paramValue As String = String.Empty

        'paramName = GetParamName()

        Dim done As Boolean = False
        While _index < _frame.Length And Not done
            Dim _char = _frame(_index)
            Select Case _char
                Case SentenceSeparator
                    paramName = buffer.ToString()
                    done = True
                Case ValueOperator
                    paramName = buffer.ToString()
                    buffer.Clear()
                    _index += 1
                    paramValue = GetParamValue()
                    done = True
                    _index -= 1
                Case Else
                    buffer.Append(_char)
            End Select
            _index += 1
        End While
        buffer.Clear()

        Return New KeyValuePair(Of String, String)(paramName, paramValue)

    End Function

    Private Function GetParamValue() As String
        Dim paramValue As String

        Dim done As Boolean = False
        While _index < _frame.Length And Not done
            Dim _char = _frame(_index)
            Select Case _char
                Case SentenceSeparator
                    done = True
                Case Else
                    buffer.Append(_char)
            End Select
            _index += 1
        End While
        paramValue = buffer.ToString()
        buffer.Clear()
        Return paramValue
    End Function

#End Region

End Class

Class AnalyzerParsedFrame
    Inherits InstrumentFrameParser
    Sub New(instructionParameters As IEnumerable(Of InstructionParameterTO))
        If instructionParameters IsNot Nothing Then
            For Each par In instructionParameters
                Parameters.Add(par.Parameter, par.ParameterValue)
            Next
        End If
    End Sub
End Class

Public Class ContextStep

    Public ReadOnly DispensingPerStep As Integer = 2

    Sub New(dispensingPerStep As Integer)
        Me.DispensingPerStep = dispensingPerStep
        ReDim _dispensings(dispensingPerStep)
    End Sub

    Default Public Property Dispensing(index As Integer) As ReagentDispensing
        Get
            Return _dispensings(index)
        End Get
        Set(value As ReagentDispensing)
            _dispensings(index) = value
        End Set
    End Property

    Private ReadOnly _dispensings() As ReagentDispensing  'R1 are dispenses(0), R2 are dispenses(1), etc.

End Class

Public Interface ReagentDispensing

    Property ReagentNumber As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!

    Property R1ReagentID As Integer

    Property TechniqueID As Integer  'ID of the associated technique

    Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode

    Property Contamines As Dictionary(Of Integer, ContaminationDescription)

    Function RequiredWashingSolution(TechniqueID As Integer, scope As Integer) As WashingDescription

End Interface

Public Class BA200ReagentDispensing
    Implements ReagentDispensing


    Public Function RequiredWashingSolution(TechniqueID As Integer, scope As Integer) As WashingDescription
        Throw New NotImplementedException("Not yet ready!")
    End Function

    Public Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode Implements ReagentDispensing.AnalysisMode

    Public Property Contamines As Dictionary(Of Integer, ContaminationDescription) Implements ReagentDispensing.Contamines

    Public Property R1ReagentID As Integer Implements ReagentDispensing.R1ReagentID

    Public Property ReagentNumber As Integer Implements ReagentDispensing.ReagentNumber

    Public Function RequiredWashingSolution1(TechniqueID As Integer, scope As Integer) As WashingDescription Implements ReagentDispensing.RequiredWashingSolution
        Throw (New NotImplementedException("Not yet ready!"))
    End Function

    Public Property TechniqueID As Integer Implements ReagentDispensing.TechniqueID
End Class

Public Class ContaminationDescription
    Public ContaminedTechnique As Integer
    Public RequiredWashing As WashingDescription
End Class

Public Class WashingDescription
    ''' <summary>
    ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
    ''' </summary>
    Public ReadOnly CleaningPower As Integer '= 0    '0 means no washing
    Public ReadOnly WashingSolutionID As Integer

    Protected Const NoWashingIDRequired = -1

    Sub New(cleaningPower As Integer, washingSolution As Integer)
        Me.CleaningPower = cleaningPower
        If Me.CleaningPower <> 0 Then
            WashingSolutionID = washingSolution
        ElseIf washingSolution <> -1 Then
            Throw New Exception("Data integrity. Washing solution of 0 power can't have a Washing solution ID")
        Else
            WashingSolutionID = NoWashingIDRequired
        End If
    End Sub

End Class

Class EmptyWashing
    Inherits WashingDescription
    Sub New()
        MyBase.New(0, NoWashingIDRequired)
    End Sub
End Class

Public Interface AnalyzerContaminationsSpecification
    ''' <summary>
    ''' This represents the amount of dispenses per step (that is, how many reagents are dispenses in a running cycle. In BA200 and BA400 that is always 2, R1 and R2)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DispensesPerStep As Integer

    ''' <summary>
    ''' This represents the range fo steps or cycles that have to be examined in order to calculate contaminations.
    ''' if 0 is current "cycle", this range usually goes from -2 to + persistence in the worst case.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ContaminationsContextRange As Range(Of Integer)

    ''' <summary>
    ''' This method provides Dispensing instances. Those instances will be responsible to return the contamination they generate when they're placed in the reactions rotor
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function DispensingFactory() As ReagentDispensing

End Interface