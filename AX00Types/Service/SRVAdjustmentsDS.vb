
Partial Public Class SRVAdjustmentsDS

    Private AnalyzerModelAttr As String = ""
    Private FirmwareVersionAttr As String = ""
    Private ReadedDatetimeAttr As DateTime = Nothing
    Private AnalyzerIDAttr As String = ""

    Public Property AnalyzerModel() As String
        Get
            Return AnalyzerModelAttr
        End Get
        Set(ByVal value As String)
            AnalyzerModelAttr = value
        End Set
    End Property

    Public Property FirmwareVersion() As String
        Get
            Return FirmwareVersionAttr
        End Get
        Set(ByVal value As String)
            FirmwareVersionAttr = value
        End Set
    End Property

    Public Property ReadedDatetime() As DateTime
        Get
            Return ReadedDatetimeAttr
        End Get
        Set(ByVal value As DateTime)
            ReadedDatetimeAttr = value
        End Set
    End Property

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttr
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttr = value
        End Set
    End Property

End Class
