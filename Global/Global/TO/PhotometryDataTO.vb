Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types

<Serializable()> Public Class PhotometryDataTO
#Region "Attributes"
    ' Serial number
    Private AnalyzerIdAttr As String
    ' Position Led
    Private PositionLedAttr As List(Of Integer)
    ' Integration Times
    Private IntegrationTimesAttr As List(Of Single)
    ' Leds Values
    Private LEDsIntensitiesAttr As List(Of Single)
    ' Baseline
    Private DateBaselineTestAttr As DateTime
    Private CountsMainBaselineAttr As List(Of Single)
    Private CountsRefBaselineAttr As List(Of Single)
    ' Darkness Counts   
    Private DateDarknessTestAttr As DateTime
    Private CountsMainDarknessAttr As List(Of Single)
    Private CountsRefDarknessAttr As List(Of Single)
#End Region

#Region "Properties"
    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIdAttr
        End Get
        Set(ByVal value As String)
            AnalyzerIdAttr = value
        End Set
    End Property

    Public Property PositionLed() As List(Of Integer)
        Get
            Return PositionLedAttr
        End Get
        Set(ByVal value As List(Of Integer))
            PositionLedAttr = value
        End Set
    End Property

    Public Property IntegrationTimes() As List(Of Single)
        Get
            Return IntegrationTimesAttr
        End Get
        Set(ByVal value As List(Of Single))
            IntegrationTimesAttr = value
        End Set
    End Property

    Public Property LEDsIntensities() As List(Of Single)
        Get
            Return LEDsIntensitiesAttr
        End Get
        Set(ByVal value As List(Of Single))
            LEDsIntensitiesAttr = value
        End Set
    End Property

    Public Property DateBaselineTest() As DateTime
        Get
            Return DateBaselineTestAttr
        End Get
        Set(ByVal value As DateTime)
            DateBaselineTestAttr = value
        End Set
    End Property

    Public Property CountsMainBaseline() As List(Of Single)
        Get
            Return CountsMainBaselineAttr
        End Get
        Set(ByVal value As List(Of Single))
            CountsMainBaselineAttr = value
        End Set
    End Property

    Public Property CountsRefBaseline() As List(Of Single)
        Get
            Return CountsRefBaselineAttr
        End Get
        Set(ByVal value As List(Of Single))
            CountsRefBaselineAttr = value
        End Set
    End Property

    Public Property DateDarknessTest() As DateTime
        Get
            Return DateDarknessTestAttr
        End Get
        Set(ByVal value As DateTime)
            DateDarknessTestAttr = value
        End Set
    End Property

    Public Property CountsMainDarkness() As List(Of Single)
        Get
            Return CountsMainDarknessAttr
        End Get
        Set(ByVal value As List(Of Single))
            CountsMainDarknessAttr = value
        End Set
    End Property

    Public Property CountsRefDarkness() As List(Of Single)
        Get
            Return CountsRefDarknessAttr
        End Get
        Set(ByVal value As List(Of Single))
            CountsRefDarknessAttr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        PositionLedAttr = New List(Of Integer)
        IntegrationTimesAttr = New List(Of Single)
        LEDsIntensitiesAttr = New List(Of Single)
        CountsMainBaselineAttr = New List(Of Single)
        CountsRefBaselineAttr = New List(Of Single)
        CountsMainDarknessAttr = New List(Of Single)
        CountsRefDarknessAttr = New List(Of Single)
    End Sub
#End Region

End Class
