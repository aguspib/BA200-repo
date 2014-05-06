Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls

    'RH 2010
    Public Class bsAlert

        'Private Const MinOpacity As Double = 0.9
        'Private Const MaxOpacity As Double = 1.0
        Private Const LineTop As Integer = 10

        Private ActiveField As Boolean = True
        Private LeftHandedField As Boolean = True
        Private IsAlertWarningField As Boolean = True

        Private Line As bsLine

        Private X As Integer
        Private Y As Integer

        Private ParentFormLeft As Integer = 0
        Private ParentFormTop As Integer = 0

        Private OldText As String = String.Empty
        Private InnerDescription As String = String.Empty

        'Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByRef lParam As Object) As Integer
        'Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Object) As Integer

        'Constant values were found in the "windows.h" header file.
        'Private Const WM_ACTIVATE As Integer = &H6
        'Private Const WM_ACTIVATEAPP As Integer = &H1C

        'Changes the title bar color of the non-client window area to indicate active/inactive.
        'Private Const WM_NCACTIVATE As Integer = &H86

        '<System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name:="FullTrust")> _
        'Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        '    If m.Msg = WM_ACTIVATE Then
        '        If m.WParam.ToInt32() = 1 Then 'Activating
        '            If Not Me.Owner.MdiParent Is Nothing Then
        '                SendMessage(Me.Owner.MdiParent.Handle.ToInt32(), WM_NCACTIVATE, 1, IntPtr.Zero)
        '            End If
        '        End If
        '    End If

        '    MyBase.WndProc(m)
        'End Sub

        <Browsable(True), Category("bsAlert")> _
        Public Property Caption() As String
            Get
                Return TitleLabel.Text
            End Get
            Set(ByVal value As String)
                TitleLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsAlert")> _
        Public Property Description() As String
            Get
                Return DescriptionLabel.Text
            End Get
            Set(ByVal value As String)
                If DescriptionLabel.Text <> value Then
                    DescriptionLabel.Text = value
                    UpdateDescriptionHeigh()
                End If
            End Set
        End Property

        <Browsable(True), Category("bsAlert")> _
        Public Property LeftHanded() As Boolean
            Get
                Return LeftHandedField
            End Get
            Set(ByVal value As Boolean)
                LeftHandedField = value
                If Not Line Is Nothing Then
                    If LeftHandedField Then
                        Me.Left = X
                        Me.Line.Left = X + Me.Width - 1
                        Me.Line.LeftHanded = True
                    Else
                        Me.Line.Left = X
                        Me.Left = X + Me.Line.Width - 1
                        Me.Line.LeftHanded = False
                    End If
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

        <Browsable(True), Category("bsAlertForm")> _
        Public Overloads Property Visible() As Boolean
            Get
                Return MyBase.Visible
            End Get
            Set(ByVal value As Boolean)
                MyBase.Visible = value
                If Not Line Is Nothing Then
                    Line.Visible = value
                End If
            End Set
        End Property

        <Browsable(True), Category("bsAlertForm")> _
        Public Overloads Property IsAlertWarning() As Boolean
            Get
                Return IsAlertWarningField
            End Get
            Set(ByVal value As Boolean)
                IsAlertWarningField = value
                If IsAlertWarningField Then
                    bsAlertIconPictureBox.Image = Global.My.Resources.Resources.AlertWarning
                Else
                    bsAlertIconPictureBox.Image = Global.My.Resources.Resources.AlertError
                End If
            End Set
        End Property

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Debug.Assert(False, "MyParentForm Is Nothing. Call New(ByRef MyParentForm As Form, ByVal X As Integer, ByVal Y As Integer, ByVal LineWidth As Integer, ByVal LineHeight As Integer, ByVal AlertTitle As String, ByVal LeftHanded As Boolean).")
        End Sub

        Public Sub New(ByRef MyParentForm As Form, ByVal X As Integer, ByVal Y As Integer, ByVal LineWidth As Integer, ByVal LineHeight As Integer, ByVal AlertTitle As String, ByVal LeftHanded As Boolean)

            Debug.Assert(Not MyParentForm Is Nothing, "MyParentForm Is Nothing. Pass a valid parent form.")

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Caption = AlertTitle
            Me.Description = String.Empty
            Me.Active = False

            Me.Owner = MyParentForm
            Me.X = Me.Owner.Left + X
            Me.Y = Me.Owner.Top + Y

            ParentFormLeft = Me.Owner.Left
            ParentFormTop = Me.Owner.Top

            Me.Left = Me.X
            Me.Top = Me.Y

            If LineWidth > 0 Then
                If LineHeight >= 0 Then
                    Line = New bsLine(Me, 0, Me.Top + LineTop, LineWidth, LineHeight, LeftHanded, True)
                Else
                    Line = New bsLine(Me, 0, Me.Top + LineTop + LineHeight, LineWidth, -LineHeight, LeftHanded, False)
                End If
            End If

            Me.Visible = False
            Me.LeftHanded = LeftHanded

            'Me.Opacity = MinOpacity
            Me.Opacity = 0

            If Not Line Is Nothing Then
                Line.Opacity = Me.Opacity
            End If

        End Sub

        Public Sub UpdatePosition()
            If Not Me.Owner Is Nothing Then
                Dim DeltaX As Integer = ParentFormLeft - Me.Owner.Left
                Dim DeltaY As Integer = ParentFormTop - Me.Owner.Top

                ParentFormLeft = Me.Owner.Left
                ParentFormTop = Me.Owner.Top

                Me.X = Me.X - DeltaX
                Me.Y = Me.Y - DeltaY

                Me.Left = Me.Left - DeltaX
                Me.Top = Me.Top - DeltaY

                If Not Line Is Nothing Then
                    Line.Left = Line.Left - DeltaX
                    Line.Top = Line.Top - DeltaY
                End If
            End If
        End Sub

        Public Sub AppendDescription(ByVal value As String)
            If Not String.IsNullOrEmpty(value) Then
                If String.IsNullOrEmpty(InnerDescription) Then
                    InnerDescription = value
                Else
                    InnerDescription += String.Format("{0}{1}", vbCrLf, value)
                End If
            End If
        End Sub

        Public Sub RefreshDescription()
            If OldText <> InnerDescription Then
                OldText = InnerDescription
                DescriptionLabel.Text = InnerDescription
                If InnerDescription <> String.Empty Then
                    UpdateDescriptionHeigh()
                End If
            End If
        End Sub

        Public Sub ClearDescription()
            InnerDescription = String.Empty
        End Sub

        Public Overloads Sub Show()
            MyBase.Show()

            While Me.Opacity < 1
                Me.Opacity += 0.1

                If Not Line Is Nothing Then
                    Line.Opacity = Me.Opacity
                End If

                Application.DoEvents()
                System.Threading.Thread.Sleep(50)
            End While
        End Sub

        Public Overloads Sub Hide()
            While Me.Opacity > 0
                Me.Opacity -= 0.1

                If Not Line Is Nothing Then
                    Line.Opacity = Me.Opacity
                End If

                Application.DoEvents()
                System.Threading.Thread.Sleep(50)
            End While

            MyBase.Hide()
        End Sub

        Private Sub UpdateDescriptionHeigh()
            Dim SaveVisible As Boolean = Visible

            Hide()

            Panel2.Height = DescriptionLabel.Height
            AlertPanel.Height = Panel1.Height + Panel2.Height + Panel3.Height + Panel4.Height
            Me.BsPanel1.Height = Panel1.Height + Panel2.Height
            Me.Height = AlertPanel.Height

            If SaveVisible Then
                Show()
            End If
        End Sub

#Region "Old Code"
        'Private Sub bsAlertBox_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        'Handles DescriptionLabel.MouseLeave, TitleLabel.MouseLeave, Panel3.MouseLeave, Panel4.MouseLeave, BsPictureBox1.MouseLeave
        '    DescriptionLabel.ForeColor = Drawing.Color.Black

        '    'Me.Opacity = MinOpacity

        '    'If Not Line Is Nothing Then
        '    '    Line.Opacity = Me.Opacity
        '    'End If

        '    'If Not Me.Owner.MdiParent Is Nothing Then
        '    '    Me.Owner.MdiParent.Activate()
        '    'End If
        'End Sub

        'Private Sub bsAlertForm_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        'Handles DescriptionLabel.MouseEnter, TitleLabel.MouseEnter, Panel3.MouseEnter, Panel4.MouseEnter, BsPictureBox1.MouseEnter
        '    'DescriptionLabel.ForeColor = Drawing.Color.Maroon
        '    'Me.BringToFront()
        '    'Me.Opacity = MaxOpacity
        '    'If Not Line Is Nothing Then
        '    '    Line.BringToFront()
        '    '    Line.Opacity = Me.Opacity
        '    'End If
        'End Sub

        'Private Sub bsAlertForm_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        'Handles DescriptionLabel.MouseUp, TitleLabel.MouseUp, Panel3.MouseUp, Panel4.MouseUp, BsPictureBox1.MouseUp
        '    MessageBox.Show("Cuando el usuario hace click sobre el balón, mostrar mensaje ampliado de la alarma, " + _
        '                    "o imagen con zoom y texto explicativo, " + _
        '                    "o cualquier elemento visual que amplíe la información acerca de la alarma.", _
        '                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    If Not Me.Owner.MdiParent Is Nothing Then
        '        Me.Owner.MdiParent.Activate()
        '    End If
        'End Sub

#End Region

        'Private Sub bsAlertForm_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        'Handles DescriptionLabel.MouseUp, TitleLabel.MouseUp, Panel3.MouseUp, Panel4.MouseUp, bsAlertIconPictureBox.MouseUp
        '    MessageBox.Show("Cuando el usuario hace click sobre el balón, mostrar mensaje ampliado de la alarma, " + _
        '                    "o imagen con zoom y texto explicativo, " + _
        '                    "o cualquier elemento visual que amplíe la información acerca de la alarma.", _
        '                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    If Not Me.Owner Is Nothing Then
        '        Me.Owner.Activate()
        '    End If
        'End Sub

        Private Sub bsAlert_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
            If Not Line Is Nothing Then
                Line.Close()
            End If
        End Sub

        Private Sub bsAlert_VisibleChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.VisibleChanged
            If Not Line Is Nothing Then
                Line.Visible = Me.Visible
            End If
            Application.DoEvents()
        End Sub

    End Class

End Namespace