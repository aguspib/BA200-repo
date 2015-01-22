Option Explicit On
Option Strict On


''' <summary>
''' To be serialized as XML
''' </summary>
''' <remarks></remarks>
Public Class FwScriptsDataTO
    'all the data
    Public Property Version() As String = ""

    Public Property Analyzers() As New List(Of AnalyzerFwScriptsTO)

    Public Property Screens() As New List(Of ScreenTO)

    Public Property FwScripts() As new List(Of FwScriptTO)

    Public ReadOnly Property AllTestedOK() As Boolean
        Get
            Dim testedOK As Boolean = True
            For Each S As FwScriptTO In Me.FwScripts
                If Not S.TestedOK Then Return False
            Next
            Return testedOK
        End Get
    End Property

#Region "Attributes"

#End Region

#Region "Constructor"
    Public Sub New()

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
