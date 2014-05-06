Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSLabel
        Inherits System.Windows.Forms.Label



#Region "Attributes"
        Public TitleAttribute As Boolean
#End Region

#Region "Properties"

        Public Property Title() As Boolean
            Get
                Title = TitleAttribute
            End Get
            Set(ByVal Value As Boolean)
                TitleAttribute = Value
                ChangeTitleColor()
            End Set
        End Property



#End Region

#Region "Intialize"

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Font = New Font("Verdana", 8.25)
        End Sub

#End Region

#Region "Functions and Subs"
        Public Sub ChangeTitleColor()
            If (TitleAttribute) Then
                Me.BackColor = Drawing.Color.LightSteelBlue
                Me.Font = New Font("Verdana", 10)
                Me.ForeColor = Color.Black
            Else
                Me.BackColor = Drawing.Color.Transparent
                Me.Font = New Font("Verdana", 8.25)
                Me.ForeColor = Color.Black
            End If
        End Sub
#End Region

    End Class
End Namespace