Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO
    Public Class AllowedLIMSValuesTO

#Region "Attributes"
        Private SampleClassesAttribute As String
        Private SampleTypesAttribute As String
        Private TubeTypesAttribute As String
        Private TestTypesAttribute As String
        Private MinTestReplicatesAttribute As Integer
        Private MaxTestReplicatesAttribute As Integer
        Private MinCalibReplicatesAttribute As Integer
        Private MaxCalibReplicatesAttribute As Integer
        Private MinCtrlReplicatesAttribute As Integer
        Private MaxCtrlReplicatesAttribute As Integer
        Private MaxLenPatientIDAttribute As Integer
#End Region

#Region "Properties"
        Public Property SampleClasses() As String
            Get
                Return SampleClassesAttribute
            End Get
            Set(ByVal value As String)
                SampleClassesAttribute = value
            End Set
        End Property

        Public Property SampleTypes() As String
            Get
                Return SampleTypesAttribute
            End Get
            Set(ByVal value As String)
                SampleTypesAttribute = value
            End Set
        End Property

        Public Property TubeTypes() As String
            Get
                Return TubeTypesAttribute
            End Get
            Set(ByVal value As String)
                TubeTypesAttribute = value
            End Set
        End Property

        Public Property TestTypes() As String
            Get
                Return TestTypesAttribute
            End Get
            Set(ByVal value As String)
                TestTypesAttribute = value
            End Set
        End Property

        Public Property MinTestReplicates() As Integer
            Get
                Return MinTestReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MinTestReplicatesAttribute = value
            End Set
        End Property

        Public Property MaxTestReplicates() As Integer
            Get
                Return MaxTestReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MaxTestReplicatesAttribute = value
            End Set
        End Property

        Public Property MinCalibReplicates() As Integer
            Get
                Return MinCalibReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MinCalibReplicatesAttribute = value
            End Set
        End Property

        Public Property MaxCalibReplicates() As Integer
            Get
                Return MaxCalibReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MaxCalibReplicatesAttribute = value
            End Set
        End Property

        Public Property MinCtrlReplicates() As Integer
            Get
                Return MinCtrlReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MinCtrlReplicatesAttribute = value
            End Set
        End Property

        Public Property MaxCtrlReplicates() As Integer
            Get
                Return MaxCtrlReplicatesAttribute
            End Get
            Set(ByVal value As Integer)
                MaxCtrlReplicatesAttribute = value
            End Set
        End Property

        Public Property MaxLenPatientID() As Integer
            Get
                Return MaxLenPatientIDAttribute
            End Get
            Set(ByVal value As Integer)
                MaxLenPatientIDAttribute = value
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub Main()
            SampleClassesAttribute = ""
            SampleTypesAttribute = ""
            TubeTypesAttribute = ""
            TestTypesAttribute = ""
            MinTestReplicatesAttribute = 0
            MaxTestReplicatesAttribute = 0
            MinCalibReplicatesAttribute = 0
            MaxCalibReplicatesAttribute = 0
            MinCtrlReplicatesAttribute = 0
            MaxCtrlReplicatesAttribute = 0
            MaxLenPatientIDAttribute = 0
        End Sub
#End Region

    End Class
End Namespace
