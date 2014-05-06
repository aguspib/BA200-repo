Option Strict On
Option Explicit On
Namespace Biosystems.Ax00.Global.TO
    Public Class DeletedTestReagentsVolumeTO

#Region "Attributes"
        Private TestIDAttribute As Integer
        Private ReagentIDAttribute As Integer
        Private SampleTypeAttribute As String
        Private ReagentNumberAttribute As Integer
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

        Public Property ReagentID() As Integer
            Get
                Return ReagentIDAttribute
            End Get
            Set(ByVal value As Integer)
                ReagentIDAttribute = value
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

        Public Property ReagentNumber() As Integer
            Get
                Return ReagentNumberAttribute
            End Get
            Set(ByVal value As Integer)
                ReagentNumberAttribute = value
            End Set
        End Property

#End Region

#Region "Constructor"
        Public Sub New()
            'initialize all attributes.
            TestIDAttribute = 0
            ReagentIDAttribute = 0
            SampleTypeAttribute = ""
            ReagentIDAttribute = 0
        End Sub
#End Region

    End Class

End Namespace
