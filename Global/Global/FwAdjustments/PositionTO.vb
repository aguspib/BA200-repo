Option Explicit On
Option Strict On


Public Class PositionTO

#Region "Attributes"
    Private IdAttr As String
    Private PolarAttr As String
    Private ZAttr As String
    Private RotorAttr As String
    Private canMovePolarAttr As Boolean
    Private canMoveZAttr As Boolean
    Private canMoveRotorAttr As Boolean
    Private canSavePolarAttr As Boolean
    Private canSaveZAttr As Boolean
    Private canSaveRotorAttr As Boolean
#End Region

#Region "Properties"
    Public Property Id() As String
        Get
            Return Me.IdAttr
        End Get
        Set(ByVal value As String)
            Me.IdAttr = value
        End Set
    End Property

    Public Property Polar() As String
        Get
            Return Me.PolarAttr
        End Get
        Set(ByVal value As String)
            Me.PolarAttr = value
        End Set
    End Property

    Public Property Z() As String
        Get
            Return Me.ZAttr
        End Get
        Set(ByVal value As String)
            Me.ZAttr = value
        End Set
    End Property

    Public Property Rotor() As String
        Get
            Return Me.RotorAttr
        End Get
        Set(ByVal value As String)
            Me.RotorAttr = value
        End Set
    End Property

    Public Property canMovePolar() As Boolean
        Get
            Return Me.canMovePolarAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canMovePolarAttr = value
        End Set
    End Property

    Public Property canMoveZ() As Boolean
        Get
            Return Me.canMoveZAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canMoveZAttr = value
        End Set
    End Property

    Public Property canMoveRotor() As Boolean
        Get
            Return Me.canMoveRotorAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canMoveRotorAttr = value
        End Set
    End Property

    Public Property canSavePolar() As Boolean
        Get
            Return Me.canSavePolarAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canSavePolarAttr = value
        End Set
    End Property

    Public Property canSaveZ() As Boolean
        Get
            Return Me.canSaveZAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canSaveZAttr = value
        End Set
    End Property

    Public Property canSaveRotor() As Boolean
        Get
            Return Me.canSaveRotorAttr
        End Get
        Set(ByVal value As Boolean)
            Me.canSaveRotorAttr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        Me.IdAttr = ""
        Me.PolarAttr = ""
        Me.ZAttr = ""
        Me.RotorAttr = ""
        Me.canMovePolarAttr = False
        Me.canMoveZAttr = False
        Me.canMoveRotorAttr = False
        Me.canSavePolarAttr = False
        Me.canSaveZAttr = False
        Me.canSaveRotorAttr = False
    End Sub
#End Region

End Class
