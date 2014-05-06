Imports System
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class bsAlertBox

        Private Const MinOpacity As Double = 0.6
        Private Const MaxOpacity As Double = 1.0

        <Browsable(True), Category("bsAlertBox")> _
        Public Property Caption() As String
            Get
                Return TitleLabel.Text
            End Get
            Set(ByVal value As String)
                TitleLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsAlertBox")> _
        Public Property Description() As String
            Get
                Return DescriptionLabel.Text
            End Get
            Set(ByVal value As String)
                DescriptionLabel.Text = value
                Panel2.Height = DescriptionLabel.Height
                Me.BsPanel1.Height = Panel1.Height + Panel2.Height
                TableLayoutPanel1.Height = Me.BsPanel1.Height + Panel3.Height + Panel4.Height
                Me.Height = TableLayoutPanel1.Height
            End Set
        End Property

        <Browsable(True), Category("bsAlertBox")> _
        Public Property LeftHanded() As Boolean
            Get
                Return RightPanel.Dock = Windows.Forms.DockStyle.Right
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    RightPanel.Dock = Windows.Forms.DockStyle.Right
                Else
                    RightPanel.Dock = Windows.Forms.DockStyle.Left
                End If
            End Set
        End Property


        <Browsable(True), Category("bsAlertBox")> _
        Public Property PointerLength() As Integer
            Get
                Return PointerPanel.Width
            End Get
            Set(ByVal value As Integer)
                PointerPanel.Width = value
                RightPanel.Width = PointerPanel.Width + 2
                Me.Width = LeftPanel.Width + RightPanel.Width
            End Set
        End Property

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
        End Sub

        Public Sub New(ByRef MyControlCollection As Windows.Forms.Control.ControlCollection, ByVal X As Integer, ByVal Y As Integer, ByVal AlertTitle As String, ByVal AlertText As String, ByVal LeftHanded As Boolean, ByVal PointerLength As Integer)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.SuspendLayout()
            Me.Visible = False
            Me.Caption = AlertTitle
            Me.Description = AlertText
            Me.Left = X
            Me.Top = Y
            Me.LeftHanded = LeftHanded
            Me.PointerLength = PointerLength
            MyControlCollection.Add(Me)
            Me.ResumeLayout(False)
        End Sub

        Private Sub bsAlertBox_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles DescriptionLabel.MouseLeave, TitleLabel.MouseLeave, Panel3.MouseLeave, Panel4.MouseLeave, BsPictureBox1.MouseLeave
            DescriptionLabel.ForeColor = Drawing.Color.Black
            'Me.Opacity = MinOpacity
        End Sub

        Private Sub bsAlertForm_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles DescriptionLabel.MouseEnter, TitleLabel.MouseEnter, Panel3.MouseEnter, Panel4.MouseEnter, BsPictureBox1.MouseEnter
            DescriptionLabel.ForeColor = Drawing.Color.Maroon
            Me.BringToFront()
            'Me.Opacity = MaxOpacity
        End Sub

        Private Sub bsAlertForm_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles DescriptionLabel.MouseUp, TitleLabel.MouseUp, Panel3.MouseUp, Panel4.MouseUp, BsPictureBox1.MouseUp
            MessageBox.Show("Mostrar mensaje ampliado de la alarma", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

    End Class

End Namespace
