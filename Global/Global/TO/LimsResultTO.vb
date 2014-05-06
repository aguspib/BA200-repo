Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class LimsResultTO

#Region "Attributes"
        Private OrderTestIDAttribute As Integer
        Private QUALITATIVEValueAttribute As String

        Private ControlNumberAttribute As Integer

        'DL 21/05/2012. begin
        Private SampleClassAttribute As String
        Private ExternalPatientIDAttribute As String
        Private SampleTypeAttribute As String
        Private TubeTypeAttribute As String
        Private TestNameAttribute As String
        Private TestTypeAttribute As String
        Private ABSValueAttribute As Decimal
        Private CONCValueAttribute As Single
        Private UnitsAttribute As String
        Private ExportDateAttribute As New DateTime
        Private ResultDateAttribute As New DateTime
        Private MultiPointNumberAttribute As Integer
        Private RerunNumberAttribute As Integer

        'DL 21/05/2012. end
#End Region

#Region "Properties"
        Public Property OrderTestID() As Integer
            Get
                Return OrderTestIDAttribute
            End Get
            Set(ByVal value As Integer)
                OrderTestIDAttribute = value
            End Set
        End Property

        Public Property MultiPointNumber() As Integer
            Get
                Return MultiPointNumberAttribute
            End Get
            Set(ByVal value As Integer)
                MultiPointNumberAttribute = value
            End Set
        End Property

        Public Property RerunNumber() As Integer
            Get
                Return RerunNumberAttribute
            End Get
            Set(ByVal value As Integer)
                RerunNumberAttribute = value
            End Set
        End Property

        Public Property ABSValue() As Decimal
            Get
                Return ABSValueAttribute
            End Get
            Set(ByVal value As Decimal)
                ABSValueAttribute = value
            End Set
        End Property

        Public Property CONCValue() As Single
            Get
                Return CONCValueAttribute
            End Get
            Set(ByVal value As Single)
                CONCValueAttribute = value
            End Set
        End Property

        Public Property QualitativeValue() As String
            Get
                Return QUALITATIVEValueAttribute
            End Get
            Set(ByVal value As String)
                QUALITATIVEValueAttribute = value
            End Set
        End Property

        Public Property ExportDate() As DateTime
            Get
                Return ExportDateAttribute
            End Get
            Set(ByVal value As DateTime)
                ExportDateAttribute = value
            End Set
        End Property

        Public Property SampleClass() As String
            Get
                Return SampleClassAttribute
            End Get
            Set(ByVal value As String)
                SampleClassAttribute = value
            End Set
        End Property

        Public Property TestType() As String
            Get
                Return TestTypeAttribute
            End Get
            Set(ByVal value As String)
                TestTypeAttribute = value
            End Set
        End Property

        Public Property ControlNumber() As Integer
            Get
                Return ControlNumberAttribute
            End Get
            Set(ByVal value As Integer)
                ControlNumberAttribute = value
            End Set
        End Property

        'DL 21/05/2012. Begin

        Public Property ExternalPatientID() As String
            Get
                Return ExternalPatientIDAttribute
            End Get
            Set(ByVal value As String)
                ExternalPatientIDAttribute = value
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

        Public Property TubeType() As String
            Get
                Return TubeTypeAttribute
            End Get
            Set(ByVal value As String)
                TubeTypeAttribute = value
            End Set
        End Property

        Public Property TestName() As String
            Get
                Return TestNameAttribute
            End Get
            Set(ByVal value As String)
                TestNameAttribute = value
            End Set
        End Property

        Public Property Units() As String
            Get
                Return UnitsAttribute
            End Get
            Set(ByVal value As String)
                UnitsAttribute = value
            End Set
        End Property

        Public Property ResultDate() As DateTime
            Get
                Return ResultDateAttribute
            End Get
            Set(ByVal value As DateTime)
                ResultDateAttribute = value
            End Set
        End Property

        'DL 21/05/2012. End
#End Region


    End Class

End Namespace
