Option Explicit On
Option Strict On

#Region "Libraries Used by this Class"
Imports System
#End Region

Namespace Biosystems.Ax00.Global.TO


    Public Class WSRequiredElementsTO


#Region "Attributes"
        Private FatherAttribute As String
        Private ElementCodeAttribute As String  'Integer    '// 19/11/09 v1.0.0 AG - Design change (Affect also the Constructor and the Get & Set properties)
        Private ElementTitleAttribute As String
        Private ElementIDAttribute As Integer
        Private ElementStatusAttribute As String
        Private ElementIconAttribute As String
        Private TubeContentAttribute As String
        Private AllowAttribute As Boolean
        Private ElementToolTipAttribute As String 'SGM 30/04/2013
#End Region

#Region "Properties"

        Public Property Father() As String
            Get
                Return FatherAttribute
            End Get

            Set(ByVal Value As String)
                FatherAttribute = Value
            End Set
        End Property

        Public Property ElementCode() As String
            Get
                Return ElementCodeAttribute
            End Get

            Set(ByVal Value As String)
                ElementCodeAttribute = Value
            End Set
        End Property

        Public Property ElementTitle() As String
            Get
                Return ElementTitleAttribute
            End Get

            Set(ByVal Value As String)
                ElementTitleAttribute = Value
            End Set
        End Property

        Public Property ElementID() As Integer
            Get
                Return ElementIDAttribute
            End Get

            Set(ByVal Value As Integer)
                ElementIDAttribute = Value
            End Set
        End Property

        Public Property ElementStatus() As String
            Get
                Return ElementStatusAttribute
            End Get

            Set(ByVal Value As String)
                ElementStatusAttribute = Value
            End Set
        End Property

        Public Property ElementIcon() As String
            Get
                Return ElementIconAttribute
            End Get

            Set(ByVal Value As String)
                ElementIconAttribute = Value
            End Set
        End Property

        Public Property TubeContent() As String
            Get
                Return TubeContentAttribute
            End Get

            Set(ByVal Value As String)
                TubeContentAttribute = Value
            End Set
        End Property

        Public Property Allow() As Boolean
            Get
                Return AllowAttribute
            End Get

            Set(ByVal Value As Boolean)
                AllowAttribute = Value
            End Set
        End Property

        'SGM 30/04/2013
        Public Property ElementToolTip() As String
            Get
                Return ElementToolTipAttribute
            End Get

            Set(ByVal Value As String)
                ElementToolTipAttribute = Value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()
            FatherAttribute = ""
            ElementCodeAttribute = ""
            ElementTitleAttribute = ""
            ElementIDAttribute = 0
            ElementStatusAttribute = ""
            ElementIconAttribute = ""
            TubeContentAttribute = ""
            AllowAttribute = False
        End Sub

#End Region

#Region "Methods"


#End Region


    End Class

End Namespace
