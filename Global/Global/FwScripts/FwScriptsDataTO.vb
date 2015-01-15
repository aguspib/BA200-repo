Option Explicit On
Option Strict On


''' <summary>
''' To be serialized as XML
''' </summary>
''' <remarks></remarks>
Public Class FwScriptsDataTO
    'all the data
    Public Property Version() As String
        Get
            Return VersionAttr
        End Get
        Set(ByVal value As String)
            VersionAttr = value
        End Set
    End Property

    Public Property Analyzers() As List(Of AnalyzerFwScriptsTO)
        Get
            Return AnalyzersAttr
        End Get
        Set(ByVal value As List(Of AnalyzerFwScriptsTO))
            AnalyzersAttr = value
        End Set
    End Property

    Public Property Screens() As List(Of ScreenTO)
        Get
            Return ScreensAttr
        End Get
        Set(ByVal value As List(Of ScreenTO))
            ScreensAttr = value
        End Set
    End Property

    Public Property FwScripts() As List(Of FwScriptTO)
        Get
            Return FwScriptsAttr
        End Get
        Set(ByVal value As List(Of FwScriptTO))
            FwScriptsAttr = value
        End Set
    End Property

    Public ReadOnly Property AllTestedOK() As Boolean
        Get
            Dim testedOK As Boolean = True
            For Each S As FwScriptTO In MyClass.FwScripts
                If Not S.TestedOK Then Return False
            Next
            Return testedOK
        End Get
    End Property

#Region "Attributes"
    Private VersionAttr As String
    Private AnalyzersAttr As List(Of AnalyzerFwScriptsTO)
    Private ScreensAttr As List(Of ScreenTO)
    Private FwScriptsAttr As List(Of FwScriptTO)
#End Region

#Region "Constructor"
    Public Sub New()
        Me.VersionAttr = ""
        Me.AnalyzersAttr = New List(Of AnalyzerFwScriptsTO)
        Me.ScreensAttr = New List(Of ScreenTO)
        Me.FwScriptsAttr = New List(Of FwScriptTO)
    End Sub
#End Region

#Region "Public Methods"
    Public Function Clone() As FwScriptsDataTO
        Try
            Dim myNewFwScriptsData As New FwScriptsDataTO
            With Me
                For Each A As AnalyzerFwScriptsTO In Me.Analyzers
                    myNewFwScriptsData.Analyzers.Add(A.Clone)
                Next
                For Each F As FwScriptTO In Me.FwScripts
                    myNewFwScriptsData.FwScripts.Add(F.Clone)
                Next
                For Each S As ScreenTO In Me.Screens
                    myNewFwScriptsData.Screens.Add(S.Clone)
                Next
                myNewFwScriptsData.Version = .Version
            End With
            Return myNewFwScriptsData
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

End Class
