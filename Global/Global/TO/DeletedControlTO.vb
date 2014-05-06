Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global.TO

    Public Class DeletedControlTO

#Region "Attributes"
        Private ControlIDAttribute As Integer
        Private ControlNameAttribute As String
        Private TestIDAttribute As Integer
        Private SampleTypeAttribute As String
#End Region

#Region "Properties"

        Public Property ControlID() As Integer
            Get
                Return ControlIDAttribute
            End Get
            Set(ByVal value As Integer)
                ControlIDAttribute = value
            End Set
        End Property

        Public Property ControlName() As String
            Get
                Return ControlNameAttribute
            End Get
            Set(ByVal value As String)
                ControlNameAttribute = value
            End Set
        End Property

        Public Property TestID() As Integer
            Get
                Return TestIDAttribute
            End Get
            Set(ByVal value As Integer)
                TestIDAttribute = value
            End Set
        End Property

        Public Property SampleType() As String
            Get
                Return SampleTypeAttribute
            End Get
            Set(ByVal value As String)
                SampleTypeAttribute = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()

            ControlIDAttribute = 0
            ControlNameAttribute = ""
            TestIDAttribute = 0
            SampleTypeAttribute = ""

        End Sub

#End Region

    End Class
End Namespace

