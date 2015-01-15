Option Explicit On
Option Strict On


Public Class ArmTO

#Region "Attributes"
    Private PositionArmAtrr As List(Of PositionTO)
#End Region

#Region "Properties"
    Public Property PositionArm() As List(Of PositionTO)
        Get
            Return Me.PositionArmAtrr
        End Get
        Set(ByVal value As List(Of PositionTO))
            Me.PositionArmAtrr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        Me.PositionArmAtrr = New List(Of PositionTO)
    End Sub
#End Region

End Class
