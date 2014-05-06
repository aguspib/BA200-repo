Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global.TO

    Public Class DeletedTestProgramingTO

#Region "Attributtes"
        Private TestIDAttribute As Integer
        Private SampleTypeAttribute As String
        Private TestNameAttribute As String 'TR 24/11/2010
        Private DeleteOnlyCalibrationResultsAttribute As Boolean
        Private DeleteBlankCalibResultsAttribute As Boolean
        Private TestVersionAttribute As Integer
        Private CurveResultsIDAttribute As Integer  'AG 22/03/2010

#End Region

#Region "Property"

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

        ''' <summary>
        ''' Get the test name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 24/11/2010
        ''' </remarks>
        Public Property TestName() As String
            Get
                Return TestNameAttribute
            End Get
            Set(ByVal value As String)
                TestNameAttribute = value
            End Set
        End Property

        Public Property DeleteOnlyCalibrationResult() As Boolean
            Get
                Return DeleteOnlyCalibrationResultsAttribute
            End Get
            Set(ByVal value As Boolean)
                DeleteOnlyCalibrationResultsAttribute = value
            End Set
        End Property

        Public Property DeleteBlankCalibResults() As Boolean
            Get
                Return DeleteBlankCalibResultsAttribute
            End Get
            Set(ByVal value As Boolean)
                DeleteBlankCalibResultsAttribute = value
            End Set
        End Property

        Public Property TestVersion() As Integer
            Get
                Return TestVersionAttribute
            End Get
            Set(ByVal value As Integer)
                TestVersionAttribute = value
            End Set
        End Property

        'AG 22/03/2010
        Public Property CurveResultsID() As Integer
            Get
                Return CurveResultsIDAttribute
            End Get
            Set(ByVal value As Integer)
                CurveResultsIDAttribute = value
            End Set
        End Property
        'END AG 22/03/2010

#End Region

#Region "Constructor"
        Public Sub New()
            TestIDAttribute = 0
            SampleTypeAttribute = ""
            DeleteOnlyCalibrationResultsAttribute = False
            DeleteBlankCalibResultsAttribute = False
            TestVersionAttribute = -1
            CurveResultsIDAttribute = -1  'AG 22/03/2010
        End Sub
#End Region

    End Class

End Namespace
