Option Strict On
Option Explicit On
Namespace Biosystems.Ax00.Global.TO
    Public Class DeletedCalibratorTO

#Region "Attributes"
        Private CalibratorIDAttribute As Integer
        Private TestIDAttribute As Integer
        Private SampleTypeAttribute As String
#End Region
        
#Region "Properties"

        Public Property CalibratorID() As Integer
            Get
                Return CalibratorIDAttribute
            End Get
            Set(ByVal value As Integer)
                CalibratorIDAttribute = value
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

#Region "Contructor"
        Public Sub New()
            CalibratorIDAttribute = 0
            TestIDAttribute = 0
            SampleTypeAttribute = ""
        End Sub
#End Region

    End Class
End Namespace
