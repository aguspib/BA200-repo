Option Explicit On
Option Strict On

Public Class InstructionParameterTO

    Private InstructionTypeAttribute As String
    Private ParameterIndexAttribute As Integer
    Private ParameterAttribute As String
    Private ParameterValueAttribute As String


    Public Property InstructionType() As String
        Get
            Return InstructionTypeAttribute
        End Get
        Set(ByVal value As String)
            InstructionTypeAttribute = value
        End Set
    End Property

    Public Property ParameterIndex() As Integer
        Get
            Return ParameterIndexAttribute
        End Get
        Set(ByVal value As Integer)
            ParameterIndexAttribute = value
        End Set
    End Property

    Public Property Parameter() As String
        Get
            Return ParameterAttribute
        End Get
        Set(ByVal value As String)
            ParameterAttribute = value
        End Set
    End Property

    Public Property ParameterValue() As String
        Get
            Return ParameterValueAttribute
        End Get
        Set(ByVal value As String)
            ParameterValueAttribute = value
        End Set
    End Property


    Public Sub New()
        InstructionTypeAttribute = ""
        ParameterAttribute = ""
        ParameterValueAttribute = ""
        ParameterIndexAttribute = 0
    End Sub
End Class
