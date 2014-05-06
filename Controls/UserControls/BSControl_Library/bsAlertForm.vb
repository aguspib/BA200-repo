Imports System
Imports System.Windows.Forms
Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class bsAlertForm

        Private Const MinOpacity As Double = 0.6
        Private Const MaxOpacity As Double = 1.0

        Private ActiveField As Boolean = True
        Private LeftHandedField As Boolean = True
        'Private WithEvents CoverPanel As Panel

        <Browsable(True), Category("bsAlertForm")> _
        Public Property Caption() As String
            Get
                Return TitleLabel.Text
            End Get
            Set(ByVal value As String)
                TitleLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsAlertForm")> _
        Public Property Description() As String
            Get
                Return DescriptionLabel.Text
            End Get
            Set(ByVal value As String)
                DescriptionLabel.Text = value
                Panel2.Height = DescriptionLabel.Height
                Me.Height = Panel1.Height + Panel2.Height + Panel3.Height + Panel4.Height
                TableLayoutPanel1.Height = Me.Height
            End Set
        End Property

        <Browsable(True), Category("bsAlertForm")> _
        Public Property LeftHanded() As Boolean
            Get
                Return LeftHandedField
            End Get
            Set(ByVal value As Boolean)
                LeftHandedField = value
                If LeftHandedField Then
                    Me.Panel4.BackgroundImage = Global.My.Resources.Resources.lefthandballoon
                Else
                    Me.Panel4.BackgroundImage = Global.My.Resources.Resources.righthandballoon
                End If
            End Set
        End Property

        <Browsable(True), Category("bsAlertForm")> _
        Public Property Active() As Boolean
            Get
                Return ActiveField
            End Get
            Set(ByVal value As Boolean)
                ActiveField = value
            End Set
        End Property

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
        End Sub

        Public Sub New(ByRef MyParentForm As Form, ByVal X As Integer, ByVal Y As Integer, ByVal AlertTitle As String, ByVal AlertText As String, ByVal LeftHanded As Boolean, ByVal PointerLength As Integer, ByVal Active As Boolean)
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
            Me.Caption = AlertTitle
            Me.Description = AlertText
            Me.LeftHanded = LeftHanded
            Me.Active = Active

            If Not MyParentForm Is Nothing Then
                Me.Owner = MyParentForm
                Me.Left = Me.Owner.Left + X
                Me.Top = Me.Owner.Top + Y

                If Not MyParentForm.MdiParent Is Nothing Then
                    Me.Left = Me.Left + MyParentForm.MdiParent.Left
                    Me.Top = Me.Top + MyParentForm.MdiParent.Top
                End If
            End If

                'CoverPanel = New Panel()
                'Me.Controls.Add(CoverPanel)
                'CoverPanel.BackColor = Drawing.Color.Transparent
                'CoverPanel.Dock = DockStyle.Fill
                'CoverPanel.BringToFront()
        End Sub

        Private Sub bsAlertBox_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles DescriptionLabel.MouseLeave, TitleLabel.MouseLeave, Panel3.MouseLeave, Panel4.MouseLeave, BsPictureBox1.MouseLeave
            DescriptionLabel.ForeColor = Drawing.Color.Black
            Me.Opacity = MinOpacity
        End Sub

        Private Sub bsAlertForm_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles DescriptionLabel.MouseEnter, TitleLabel.MouseEnter, Panel3.MouseEnter, Panel4.MouseEnter, BsPictureBox1.MouseEnter
            DescriptionLabel.ForeColor = Drawing.Color.Maroon
            Me.BringToFront()
            Me.Opacity = MaxOpacity
        End Sub

        Private Sub bsAlertForm_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles DescriptionLabel.MouseUp, TitleLabel.MouseUp, Panel3.MouseUp, Panel4.MouseUp, BsPictureBox1.MouseUp
            MessageBox.Show("Mostrar mensaje ampliado de la alarma", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

    End Class

End Namespace