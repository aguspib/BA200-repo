Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class bsAlertArrowForm

        Private Const MinOpacity As Double = 0.6
        Private Const MaxOpacity As Double = 1.0

        Private ActiveField As Boolean = True
        Private LeftHandedField As Boolean = True
        Private ArrowHeight As Integer = 0

        Private LinePen As Pen = New Pen(Color.Brown, 5)

        <Browsable(True), Category("bsAlertArrowForm")> _
        Public Property Caption() As String
            Get
                Return TitleLabel.Text
            End Get
            Set(ByVal value As String)
                TitleLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsAlertArrowForm")> _
        Public Property Description() As String
            Get
                Return DescriptionLabel.Text
            End Get
            Set(ByVal value As String)
                DescriptionLabel.Text = value
                Panel2.Height = DescriptionLabel.Height
                Me.BsPanel1.Height = Panel1.Height + Panel2.Height
                AlertPanel.Height = Me.BsPanel1.Height + Panel3.Height + Panel4.Height
            End Set
        End Property

        <Browsable(True), Category("bsAlertArrowForm")> _
        Public Property LeftHanded() As Boolean
            Get
                Return LeftHandedField
            End Get
            Set(ByVal value As Boolean)
                LeftHandedField = value
                If LeftHandedField Then
                    Me.AlertPanel.Left = 0
                Else
                    Me.AlertPanel.Left = Me.Width - Me.AlertPanel.Width
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

        Public Sub New(ByRef MyParentForm As Form, ByVal X As Integer, ByVal Y As Integer, ByVal ArrowWidth As Integer, ByVal ArrowHeight As Integer, ByVal AlertTitle As String, ByVal AlertText As String, ByVal LeftHanded As Boolean, ByVal PointerLength As Integer, ByVal Active As Boolean)

            Debug.Assert(Not MyParentForm Is Nothing, "MyParentForm Is Nothing. Pass a valid parent form.")

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
            Me.Caption = AlertTitle
            Me.Description = AlertText
            Me.Active = Active

            Me.Owner = MyParentForm
            Me.Left = Me.Owner.Left + X
            Me.Top = Me.Owner.Top + Y

            If Not MyParentForm.MdiParent Is Nothing Then
                Me.Left = Me.Left + MyParentForm.MdiParent.Left
                Me.Top = Me.Top + MyParentForm.MdiParent.Top
            End If

            Me.Width = Me.AlertPanel.Width + ArrowWidth

            If ArrowHeight > Me.AlertPanel.Height Then
                Me.Height = ArrowHeight
            Else
                Me.Height = Me.AlertPanel.Height
            End If

            Me.ArrowHeight = ArrowHeight

            Me.LeftHanded = LeftHanded

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

        Private Sub bsAlertArrowForm_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
            If LeftHandedField Then
                e.Graphics.DrawLine(LinePen, Me.AlertPanel.Width + 2, CInt(Me.AlertPanel.Height / 2), Me.Width, Me.ArrowHeight)
                'e.Graphics.DrawArc(LinePen, Me.AlertPanel.Width + 2, CInt(Me.AlertPanel.Height / 2), Me.Width, Me.ArrowHeight, 100, 90)
            Else
                e.Graphics.DrawLine(LinePen, Me.Width - Me.AlertPanel.Width - 2, CInt(Me.AlertPanel.Height / 2), 0, Me.ArrowHeight)
            End If
        End Sub
    End Class

End Namespace