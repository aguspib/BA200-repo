Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO
    Public Class LimsWorkSessionTO

#Region "Attributes"
        Private SampleClassAttribute As String
        Private SampleTypeAttribute As String
        Private PatientIDAttribute As String
        Private TestIDAttribute As String
        Private TubeTypeAttribute As String
        Private ReplicatesNumberAttribute As String
        Private LIMSOrderIDAttribute As String

        Private HasErrorAttribute As Boolean
        Private ErrorCodeAttribute As String
        Private ErrorMessageAttribute As String
#End Region

#Region "Properties"
        Public Property SampleClass() As String
            Get
                Return SampleClassAttribute
            End Get
            Set(ByVal value As String)
                SampleClassAttribute = value
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

        Public Property PatientID() As String
            Get
                Return PatientIDAttribute
            End Get
            Set(ByVal value As String)
                PatientIDAttribute = value
            End Set
        End Property

        Public Property TestID() As String
            Get
                Return TestIDAttribute
            End Get
            Set(ByVal value As String)
                TestIDAttribute = value
            End Set
        End Property

        Public Property TubeType() As String
            Get
                Return TubeTypeAttribute
            End Get
            Set(ByVal value As String)
                TubeTypeAttribute = value
            End Set
        End Property

        Public Property ReplicatesNumber() As String
            Get
                Return ReplicatesNumberAttribute
            End Get
            Set(ByVal value As String)
                ReplicatesNumberAttribute = value
            End Set
        End Property

        Public Property LIMSOrderID() As String
            Get
                Return LIMSOrderIDAttribute
            End Get
            Set(ByVal value As String)
                LIMSOrderIDAttribute = value
            End Set
        End Property

        Public Property HasError() As Boolean
            Get
                Return HasErrorAttribute
            End Get
            Set(ByVal value As Boolean)
                HasErrorAttribute = value
            End Set
        End Property

        Public Property ErrorCode() As String
            Get
                Return ErrorCodeAttribute
            End Get
            Set(ByVal value As String)
                ErrorCodeAttribute = value
            End Set
        End Property

        Public Property ErrorMessage() As String
            Get
                Return ErrorMessageAttribute
            End Get
            Set(ByVal value As String)
                ErrorMessageAttribute = value
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub Main()
            SampleClassAttribute = ""
            SampleTypeAttribute = ""
            PatientIDAttribute = ""
            TestIDAttribute = ""
            TubeTypeAttribute = ""
            ReplicatesNumberAttribute = ""
            LIMSOrderIDAttribute = ""

            HasErrorAttribute = False
            ErrorCodeAttribute = ""
            ErrorMessageAttribute = ""
        End Sub
#End Region
    End Class

End Namespace
