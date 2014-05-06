Imports System
Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class bsAlertTip

        <Browsable(True), Category("bsAlertTip")> _
        Public Property Caption() As String
            Get
                Return TitleLabel.Text
            End Get
            Set(ByVal value As String)
                TitleLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsAlertTip")> _
        Public Property Description() As String
            Get
                Return DescriptionLabel.Text
            End Get
            Set(ByVal value As String)
                DescriptionLabel.Text = value
                'Panel2.Height = DescriptionLabel.Height + 4
                'Me.Height = Panel2.Height + Panel1.Height + 5
            End Set
        End Property

        '<Browsable(True), Category("bsAlertTip")> _
        'Public Property LeftHanded() As Boolean
        '    Get
        '        Return RightPanel.Dock = Windows.Forms.DockStyle.Right
        '    End Get
        '    Set(ByVal value As Boolean)
        '        If value Then
        '            RightPanel.Dock = Windows.Forms.DockStyle.Right
        '        Else
        '            RightPanel.Dock = Windows.Forms.DockStyle.Left
        '        End If
        '    End Set
        'End Property


        '<Browsable(True), Category("bsAlertTip")> _
        'Public Property PointerLength() As Integer
        '    Get
        '        Return PointerPanel.Width
        '    End Get
        '    Set(ByVal value As Integer)
        '        PointerPanel.Width = value
        '        RightPanel.Width = PointerPanel.Width + 2
        '        Me.Width = LeftPanel.Width + RightPanel.Width
        '    End Set
        'End Property

        Private Sub bsAlertBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TitleLabel.MouseUp, Panel2.MouseUp, Panel1.MouseUp, LeftPanel.MouseUp, DescriptionLabel.MouseUp
            Me.BringToFront()
        End Sub

        Private Sub bsAlertBox_MouseHover(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DescriptionLabel.MouseHover
            DescriptionLabel.ForeColor = Drawing.Color.Maroon
            'LeftPanel.BackColor = Drawing.Color.Gainsboro
        End Sub

        Private Sub bsAlertBox_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleLabel.MouseLeave, Panel2.MouseLeave, Panel1.MouseLeave, LeftPanel.MouseLeave, DescriptionLabel.MouseLeave

            DescriptionLabel.ForeColor = Drawing.Color.Black
            'LeftPanel.BackColor = Drawing.Color.Transparent
        End Sub
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
            'Me.LeftHanded = LeftHanded
            'Me.PointerLength = PointerLength
            MyControlCollection.Add(Me)
            Me.ResumeLayout(False)
        End Sub

    End Class

End Namespace
