Namespace Biosystems.Ax00.Global.TO

    Public Class TestPositionTO
#Region "Attrubutes"
        Private TestIDAttribute As Integer
        Private TestPositionAttribute As Integer
#End Region
        
#Region "Properties"
        Public Property TestID() As Integer
            Get
                Return TestIDAttribute
            End Get
            Set(ByVal value As Integer)
                TestIDAttribute = value
            End Set
        End Property

        Public Property TestPosition() As Integer
            Get
                Return TestPositionAttribute
            End Get
            Set(ByVal value As Integer)
                TestPositionAttribute = value
            End Set
        End Property
#End Region
        
    End Class

End Namespace


