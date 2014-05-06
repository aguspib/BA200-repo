Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' A complementary class for supporting bsAlert class
    ''' </summary>
    ''' <remarks>
    ''' Created by RH 10/03/2011
    ''' </remarks>
    Public Class bsLine

        Private LeftHandedField As Boolean = True
        Private UpDownField As Boolean = True
        Private ShowBeakField As Boolean = True

        Private BrownPen As Pen = New Pen(Color.Brown, 2)
        Private BlackPen As Pen = New Pen(Color.Black, 2)
        Private LineBrush As New SolidBrush(Color.FromArgb(&HFD, &HFD, &HE9))

        <Browsable(True), Category("bsLine")> _
        Public Property LeftHanded() As Boolean
            Get
                Return LeftHandedField
            End Get
            Set(ByVal value As Boolean)
                LeftHandedField = value
            End Set
        End Property

        <Browsable(True), Category("bsLine")> _
        Public Property UpDown() As Boolean
            Get
                Return UpDownField
            End Get
            Set(ByVal value As Boolean)
                UpDownField = value
            End Set
        End Property

        <Browsable(True), Category("bsLine")> _
        Public Property ShowBeak() As Boolean
            Get
                Return ShowBeakField
            End Get
            Set(ByVal value As Boolean)
                ShowBeakField = value
            End Set
        End Property

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
        End Sub

        Public Sub New(ByRef MyParentForm As Form, ByVal X As Integer, ByVal Y As Integer, ByVal LineWidth As Integer, ByVal LineHeight As Integer, ByVal LeftHanded As Boolean, ByVal UpDown As Boolean)

            Debug.Assert(Not MyParentForm Is Nothing, "MyParentForm Is Nothing. Pass a valid parent form.")

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.Visible = False
            Me.Left = X
            Me.Top = Y

            Me.Width = LineWidth
            Me.Height = Math.Max(Me.MinimumSize.Height, LineHeight)

            Me.Owner = MyParentForm

            Me.LeftHanded = LeftHanded
            Me.UpDown = UpDown

            If Not UpDown Then
                Me.Top += Me.MinimumSize.Height + 1
            End If

        End Sub

        Private Sub bsLine_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
            If UpDown Then
                If ShowBeak Then
                    If LeftHanded Then
                        Dim point1 As New Point(0, 1)
                        Dim point2 As New Point(Me.Width, Me.Height - Me.MinimumSize.Height + 1)
                        Dim point3 As New Point(0, Me.MinimumSize.Height - 1)
                        Dim point4 As New Point(point2.X - 1, point2.Y)
                        Dim curvePoints As Point() = {point1, point4, point3}

                        e.Graphics.DrawLine(BlackPen, point1, point2)
                        e.Graphics.DrawLine(BlackPen, point3, point4)
                        e.Graphics.FillPolygon(LineBrush, curvePoints)

                    Else
                        Dim point1 As New Point(0, Me.Height - Me.MinimumSize.Height + 1)
                        Dim point2 As New Point(Me.Width, 1)
                        Dim point3 As New Point(Me.Width, Me.MinimumSize.Height - 1)
                        Dim point4 As New Point(point1.X + 1, point1.Y)
                        Dim curvePoints As Point() = {point4, point2, point3}

                        e.Graphics.DrawLine(BlackPen, point1, point2)
                        e.Graphics.DrawLine(BlackPen, point4, point3)
                        e.Graphics.FillPolygon(LineBrush, curvePoints)
                    End If
                Else
                    If LeftHanded Then
                        e.Graphics.DrawLine(BrownPen, 0, 1, Me.Width, Me.Height - Me.MinimumSize.Height + 1)
                    Else
                        e.Graphics.DrawLine(BrownPen, 0, Me.Height - Me.MinimumSize.Height + 1, Me.Width, 1)
                    End If
                End If
            Else
                If ShowBeak Then
                    If LeftHanded Then
                        Dim point1 As New Point(0, Me.Height - Me.MinimumSize.Height + 1)
                        Dim point2 As New Point(Me.Width, 1)
                        Dim point3 As New Point(0, Me.Height - 1)
                        Dim point4 As New Point(point2.X - 1, point2.Y)
                        Dim curvePoints As Point() = {point1, point4, point3}

                        e.Graphics.DrawLine(BlackPen, point1, point2)
                        e.Graphics.DrawLine(BlackPen, point3, point4)
                        e.Graphics.FillPolygon(LineBrush, curvePoints)

                    Else
                        Dim point1 As New Point(0, 1)
                        Dim point2 As New Point(Me.Width, Me.Height - Me.MinimumSize.Height + 1)
                        Dim point3 As New Point(Me.Width, Me.Height - 1)
                        Dim point4 As New Point(point1.X + 1, point1.Y)
                        Dim curvePoints As Point() = {point4, point2, point3}

                        e.Graphics.DrawLine(BlackPen, point1, point2)
                        e.Graphics.DrawLine(BlackPen, point4, point3)
                        e.Graphics.FillPolygon(LineBrush, curvePoints)
                    End If
                Else
                    If LeftHanded Then
                        e.Graphics.DrawLine(BrownPen, 0, Me.Height - Me.MinimumSize.Height + 1, Me.Width, 1)
                    Else
                        e.Graphics.DrawLine(BrownPen, 0, 1, Me.Width, Me.Height - Me.MinimumSize.Height + 1)
                    End If
                End If
            End If
        End Sub

        Private Sub bsLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Click
            If (Not Me.Owner.Owner Is Nothing) AndAlso (Not Me.Owner.Owner.MdiParent Is Nothing) Then
                Me.Owner.Owner.MdiParent.Activate()
            End If
        End Sub

    End Class

End Namespace