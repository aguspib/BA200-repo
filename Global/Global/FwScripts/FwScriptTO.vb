Option Explicit On
Option Strict On



Public Class FwScriptTO

#Region "Public Properties"



    Public Property FwScriptID() As Integer
        Get
            Return FwScriptIDAttr
        End Get
        Set(ByVal value As Integer)
            FwScriptIDAttr = value
        End Set
    End Property

    Public Property ActionID() As String
        Get
            Return ActionIDAttr
        End Get
        Set(ByVal value As String)
            ActionIDAttr = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return DescriptionAttr
        End Get
        Set(ByVal value As String)
            DescriptionAttr = value
        End Set
    End Property

    Public Property Instructions() As List(Of InstructionTO)
        Get
            Return InstructionsAttr
        End Get
        Set(ByVal value As List(Of InstructionTO))
            InstructionsAttr = value
        End Set
    End Property

    Public Property Created() As DateTime
        Get
            Return CreatedAttr
        End Get
        Set(ByVal value As DateTime)
            CreatedAttr = value
        End Set
    End Property

    Public Property Modified() As DateTime
        Get
            Return ModifiedAttr
        End Get
        Set(ByVal value As DateTime)
            ModifiedAttr = value
        End Set
    End Property

    Public Property Author() As String
        Get
            Return AuthorAttr
        End Get
        Set(ByVal value As String)
            AuthorAttr = value
        End Set
    End Property

    Public Property SyntaxOK() As Boolean
        Get
            Return SyntaxOKAttr
        End Get
        Set(ByVal value As Boolean)
            SyntaxOKAttr = value
        End Set
    End Property

    Public Property TestedOK() As Boolean
        Get
            Return TestedOKAttr
        End Get
        Set(ByVal value As Boolean)
            TestedOKAttr = value
        End Set
    End Property

    Public ReadOnly Property Text() As String
        Get
            Return TextAttr
        End Get
    End Property

    

#End Region

#Region "Attributes"
    Private FwScriptIDAttr As Integer
    Private ActionIDAttr As String
    Private DescriptionAttr As String
    Private InstructionsAttr As List(Of InstructionTO)

    Private CreatedAttr As DateTime
    Private ModifiedAttr As DateTime
    Private AuthorAttr As String

    Private SyntaxOKAttr As Boolean
    Private TestedOKAttr As Boolean

    Private TextAttr As String

    Private TotalTimeAttr As Integer
#End Region

#Region "Constructor"
    Public Sub New()
        Me.Instructions = New List(Of InstructionTO)
        FwScriptIDAttr = 0
        ActionIDAttr = ""
        DescriptionAttr = ""

        CreatedAttr = CDate(Nothing)
        ModifiedAttr = CDate(Nothing)
        AuthorAttr = ""

        SyntaxOKAttr = False
        TestedOKAttr = False

        TextAttr = ""
    End Sub
#End Region


#Region "Private Methods"
    Private Sub BuildText()
        TextAttr = ""
        For Each I As InstructionTO In Me.Instructions
            TextAttr &= I.Text & vbCrLf
        Next
    End Sub
#End Region

#Region "Public Methods"
    Public Function Clone() As FwScriptTO
        Try
            Dim myNewFwScript As New FwScriptTO
            With Me
                myNewFwScript.ActionID = .ActionID
                myNewFwScript.Author = .Author
                myNewFwScript.Created = .Created
                myNewFwScript.Description = .Description
                myNewFwScript.FwScriptID = .FwScriptID
                For Each I As InstructionTO In .Instructions
                    myNewFwScript.Instructions.Add(I.Clone)
                Next
                myNewFwScript.Modified = .Modified
                myNewFwScript.SyntaxOK = .SyntaxOK
                myNewFwScript.TestedOK = .TestedOK
            End With
            Return myNewFwScript
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

End Class
