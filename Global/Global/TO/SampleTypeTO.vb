Namespace Biosystems.Ax00.Global.TO

    Public Class SampleTypeTO

#Region "Attributes"
        Private NameAttribute As String
        Private PositionAttribute As Integer

#End Region

#Region "Properties"

        Public Property Name() As String
            Get
                Return NameAttribute
            End Get
            Set(ByVal value As String)
                NameAttribute = value
            End Set
        End Property

        Public Property Position() As Integer
            Get
                Return PositionAttribute
            End Get
            Set(ByVal value As Integer)
                PositionAttribute = value
            End Set
        End Property

#End Region

    End Class

End Namespace


