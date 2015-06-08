Imports System.Text

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations


    Public Class LAx00Frame

#Region "Private attributes"
        Dim _index As Integer = 0
        Dim _frame As String
        Const ParserbufferSize = 1024
        Const SentenceSeparator = ";"c
        Const ValueOperator = ":"c
        <ThreadStatic> Shared _buffer As StringBuilder
        Private Property buffer As StringBuilder
            Get
                If _buffer Is Nothing Then _buffer = New StringBuilder(ParserbufferSize)
                Return _buffer
            End Get
            Set(value As StringBuilder)
                _buffer = value
            End Set
        End Property
        Protected Parameters As New Dictionary(Of String, String)
#End Region

#Region "Public members"

        'Allow for empty constructor
        Sub New()
        End Sub

        Sub New(instructionParameters As IEnumerable(Of InstructionParameterTO))
            If instructionParameters IsNot Nothing Then
                For Each par In instructionParameters
                    Parameters.Add(par.Parameter, par.ParameterValue)
                Next
            End If
        End Sub

        Public Sub ParseRawData(Frame As String)

            _frame = Frame

            If buffer Is Nothing Then buffer = New StringBuilder
            buffer.Clear()

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

        Default Public Property Item(key As String, Optional defaultValue As String = "") As String
            Get
                If Parameters.ContainsKey(key) Then Return Parameters(key) Else Return defaultValue

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
                If dic.Keys.Contains(KV.Key) = False Then dic.Add(KV.Key, KV.Value)
            End While
            Return dic

        End Function

        Public Overrides Function ToString() As String
            Dim SB As New StringBuilder
            For Each element In Parameters.Keys
                SB.Append(element)
                If Parameters(element) IsNot Nothing AndAlso Parameters(element) <> String.Empty Then
                    SB.Append(":"c & Parameters(element))
                End If
                SB.Append(";"c)
            Next
            Return SB.ToString
        End Function

        Private Function TokenizeSentence() As KeyValuePair(Of String, String)
            Dim paramName As String = String.Empty, paramValue As String = String.Empty

            'paramName = GetParamName()

            Dim done As Boolean = False
            While _index < _frame.Length And Not done
                ' ReSharper disable once InconsistentNaming
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

End Namespace