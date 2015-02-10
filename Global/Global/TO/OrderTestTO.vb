Namespace Biosystems.Ax00.Global.TO

    Public Class OrderTestTO

#Region "Attributes"
        Private TestIdAttribute As Integer
        Private TestTypeAttribute As String
        Private SampleTypeAttribute As SampleTypeTO
        Private TestPositionAttribute As Integer
        Private TestNameAttribute As String
        Private ShortNameAttribute As String
        Private TestLongNameAttribute As String
#End Region

#Region "Properties"
        Public Property TestId() As Integer
            Get
                Return TestIdAttribute
            End Get
            Set(ByVal value As Integer)
                TestIdAttribute = value
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

        Public Property SampleType() As SampleTypeTO
            Get
                Return SampleTypeAttribute
            End Get
            Set(ByVal value As SampleTypeTO)
                SampleTypeAttribute = value
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

        Public Property TestName() As String
            Get
                Return TestNameAttribute
            End Get
            Set(ByVal value As String)
                TestNameAttribute = value
            End Set
        End Property

        Public Property ShortName() As String
            Get
                Return ShortNameAttribute
            End Get
            Set(ByVal value As String)
                ShortNameAttribute = value
            End Set
        End Property

        ' XB 18/09/2014 - BA-1884
        Public Property TestLongName As String
            Get
                Return TestLongNameAttribute
            End Get
            Set(value As String)
                TestLongNameAttribute = value
            End Set
        End Property

#End Region

    End Class

End Namespace


