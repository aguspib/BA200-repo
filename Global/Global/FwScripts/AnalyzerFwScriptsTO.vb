Option Explicit On
Option Strict On



Public Class AnalyzerFwScriptsTO

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttr
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttr = value
        End Set
    End Property

    'all the Screen ID's related to the Analyzer
    Public Property ScreenIDs() As List(Of String)
        Get
            Return ScreenIDsAttr
        End Get
        Set(ByVal value As List(Of String))
            ScreenIDsAttr = value
        End Set
    End Property

#Region "Attributes"
    Private AnalyzerIDAttr As String

    'all the Screen ID's related to the Analyzer
    Private ScreenIDsAttr As List(Of String)
#End Region

#Region "Constructor"
    Public Sub New()
        AnalyzerIDAttr = ""
        Me.ScreenIDsAttr = New List(Of String)
    End Sub
#End Region

#Region "Public methods"
    Public Function Clone() As AnalyzerFwScriptsTO
        Try
            Dim myNewAnalyzerFwScripts As New AnalyzerFwScriptsTO
            With Me
                myNewAnalyzerFwScripts.AnalyzerID = .AnalyzerID
                myNewAnalyzerFwScripts.ScreenIDs = .ScreenIDs
            End With
            Return myNewAnalyzerFwScripts
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region
End Class
