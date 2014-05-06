Option Explicit On
Option Strict On

Imports System.Windows.Forms

Public Class DataGridViewDisableTextBoxColumn
    Inherits DataGridViewTextBoxColumn

    Public Sub New()
        Me.CellTemplate = New DataGridViewDisableTextBoxCell()
    End Sub
End Class