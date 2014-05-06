Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class OpticCenterDataTO

#Region "Attributes"
        Private StepsAttr As Single
        Private CountsMainAttr As Single
        Private CountsRefAttr As Single
        ' XBC 21/12/2011 - Add Encoder functionality
        Private EncoderAttr As Single
#End Region

#Region "Properties"

        Public Property Steps() As Single
            Get
                Return Me.StepsAttr
            End Get
            Set(ByVal value As Single)
                Me.StepsAttr = value
            End Set
        End Property

        Public Property CountsMain() As Single
            Get
                Return Me.CountsMainAttr
            End Get
            Set(ByVal value As Single)
                Me.CountsMainAttr = value
            End Set
        End Property

        Public Property CountsRef() As Single
            Get
                Return Me.CountsRefAttr
            End Get
            Set(ByVal value As Single)
                Me.CountsRefAttr = value
            End Set
        End Property

        ' XBC 21/12/2011 - Add Encoder functionality
        Public Property Encoder() As Single
            Get
                Return Me.EncoderAttr
            End Get
            Set(ByVal value As Single)
                Me.EncoderAttr = value
            End Set
        End Property
#End Region

#Region "Constructor"

        Public Sub New()
            Me.StepsAttr = 0
            Me.CountsMainAttr = 0
            Me.CountsRefAttr = 0
            ' XBC 21/12/2011 - Add Encoder functionality
            Me.EncoderAttr = 0
        End Sub

#End Region

    End Class
End Namespace


