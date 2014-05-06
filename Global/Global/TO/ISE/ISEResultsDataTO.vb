Option Explicit On
Option Strict On

'testing
Namespace Biosystems.Ax00.Global

    Public Class ISEResultsDataTO


#Region "Attribute"
        Private ISEResultsDataAttr As List(Of ISEResultTO)

#End Region

#Region "Properties"
        Public Property ISEResultsData() As List(Of ISEResultTO)
            Get
                Return ISEResultsDataAttr
            End Get
            Set(ByVal value As List(Of ISEResultTO))
                ISEResultsDataAttr = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()
            ISEResultsData = New List(Of ISEResultTO)
        End Sub

#End Region

        Public Overrides Function ToString() As String
            Dim ret As String = ""
            For Each R As ISEResultTO In ISEResultsDataAttr
                ret &= R.ReceivedResults & vbCrLf
            Next
            Return ret
        End Function

    End Class
End Namespace