Option Explicit On
Option Strict On

Public Class InstructionParameterTO
    Public Property InstructionType As String

    Public Property ParameterIndex As Integer

    Public Property Parameter As String

    Public Property ParameterValue As String


    Public Sub New()
        InstructionType = ""
        Parameter = ""
        ParameterValue = ""
        ParameterIndex = 0
    End Sub
End Class
