Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls




    Public Class BsScadaPipeControl
        Inherits BsScadaControl

        Public Sub New()
            Try
                ' This call is required by the Windows Form Designer.
                InitializeComponent()

                ' Add any initialization after the InitializeComponent() call.

            Catch ex As Exception
                Throw ex
            End Try

        End Sub

        Public Enum PipeTypes
            _0
            _1
            _2
            _3
            _4
            _5
            _6
            _7
            _8
        End Enum

        Public Overloads Property Orientation() As BsScadaControl.Orientations
            Get
                Return MyBase.Orientation
            End Get
            Set(ByVal value As BsScadaControl.Orientations)

                If MyBase.Orientation <> value Then

                    RefreshOrientation(value)

                    MyBase.Orientation = value

                End If
            End Set
        End Property

        Public Property PipeType() As PipeTypes
            Get
                Return TypeAttr
            End Get
            Set(ByVal value As PipeTypes)
                If value <> TypeAttr Then

                    RefreshType(value)

                    TypeAttr = value

                    Me.Refresh()

                End If

            End Set
        End Property

        Public Property End1Visible() As Boolean
            Get
                Return End1VisibleAttr
            End Get
            Set(ByVal value As Boolean)
                BsEnd1.Visible = value
                End1VisibleAttr = value
            End Set
        End Property

        Public Property End2Visible() As Boolean
            Get
                Return End2VisibleAttr
            End Get
            Set(ByVal value As Boolean)
                BsEnd2.Visible = value
                End2VisibleAttr = value
            End Set
        End Property

        Public Property OuterColor() As Color
            Get
                Return OuterColorAttr
            End Get
            Set(ByVal value As Color)
                BsOuter1.BackColor = value
                BsOuter2.BackColor = value
                BsEnd1.BackColor = value
                BsEnd2.BackColor = value
                OuterColorAttr = value
            End Set
        End Property

        Public Property InnerColor() As Color
            Get
                Return InnerColorAttr
            End Get
            Set(ByVal value As Color)
                Me.BackColor = value
                InnerColorAttr = value
            End Set
        End Property


        Public Property PipeWidth() As Integer
            Get
                Return PipeWidthAttr
            End Get
            Set(ByVal value As Integer)
                PipeWidthAttr = value

                RefreshType(MyClass.PipeType)

                Select Case MyClass.Orientation
                    Case BsScadaControl.Orientations._0, Orientations._180
                        Me.Height = value
                    Case BsScadaControl.Orientations._90, Orientations._270
                        Me.Width = value
                End Select
            End Set
        End Property
        Public Property OuterWidth() As Integer
            Get
                Return OuterWidthAttr
            End Get
            Set(ByVal value As Integer)
                OuterWidthAttr = value

                RefreshType(MyClass.PipeType)

                Select Case MyClass.Orientation
                    Case BsScadaControl.Orientations._0, Orientations._180
                        Me.Height = value
                    Case BsScadaControl.Orientations._90, Orientations._270
                        Me.Width = value
                End Select

                BsEnd1.Width = value
                BsEnd2.Width = value
            End Set
        End Property

        Public Overloads Property FluidColor() As Color
            Get
                Return MyBase.FluidColor
            End Get
            Set(ByVal value As Color)
                MyBase.FluidColor = value
                MyClass.InnerColor = value
            End Set
        End Property

        Private TypeAttr As PipeTypes = PipeTypes._0
        Private OuterColorAttr As Color = Color.DimGray
        Private InnerColorAttr As Color = Color.WhiteSmoke
        Private End1VisibleAttr As Boolean = False
        Private End2VisibleAttr As Boolean = False
        Private PipeWidthAttr As Integer = 5
        Private OuterWidthAttr As Integer = 1

        Private IsChangingOrientation As Boolean = False

        Private Sub RefreshOrientation(ByVal pOrientation As BsScadaControl.Orientations)
            Try
                Dim W As Integer = PipeWidth
                Dim v As Integer = OuterWidth

                IsChangingOrientation = True

                Select Case pOrientation
                    Case BsScadaControl.Orientations._0, Orientations._180
                        Dim a As Integer = Me.Width
                        Me.Width = Me.Height
                        Me.Height = a

                        Me.BsOuter1.Width = Me.BsOuter1.Height
                        Me.BsOuter1.Height = v

                        Me.BsOuter2.Width = Me.BsOuter2.Height
                        Me.BsOuter2.Height = v

                        Me.BsOuter1.Location = New Point(0, 0)
                        Me.BsOuter2.Location = New Point(0, W - v)

                        Me.BsEnd1.Width = v
                        Me.BsEnd2.Width = v

                        Me.BsEnd1.Height = W
                        Me.BsEnd2.Height = W

                        Me.BsEnd1.Location = New Point(0, 0)
                        Me.BsEnd2.Location = New Point(Me.Width - v, 0)

                    Case BsScadaControl.Orientations._90, Orientations._270
                        Dim b As Integer = Me.Height
                        Me.Height = Me.Width
                        Me.Width = b

                        Me.BsOuter1.Height = Me.BsOuter1.Width
                        Me.BsOuter1.Width = v

                        Me.BsOuter2.Height = Me.BsOuter2.Width
                        Me.BsOuter2.Width = v

                        Me.BsOuter1.Location = New Point(W - v, 0)
                        Me.BsOuter2.Location = New Point(0, 0)

                        Me.BsEnd1.Height = v
                        Me.BsEnd2.Height = v

                        Me.BsEnd1.Width = W
                        Me.BsEnd2.Width = W

                        Me.BsEnd1.Location = New Point(0, 0)
                        Me.BsEnd2.Location = New Point(0, Me.Height - v)

                End Select

                IsChangingOrientation = False

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub RefreshType(ByVal pType As PipeTypes)
            Try

                Dim W As Integer = PipeWidth
                Dim v As Integer = OuterWidth

                Select Case MyClass.Orientation
                    Case BsScadaControl.Orientations._0, Orientations._180

                        Me.BsOuter1.Height = v
                        Me.BsOuter2.Height = v

                        Select Case pType
                            Case PipeTypes._0
                                Me.BsOuter1.Width = Me.Width
                                Me.BsOuter2.Width = Me.Width
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                            Case PipeTypes._1
                                Me.BsOuter1.Width = Me.Width
                                Me.BsOuter2.Width = Me.Width - ((W - v) - v)
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point(((W - v) - v), W - v)

                            Case PipeTypes._2
                                Me.BsOuter1.Width = Me.Width - ((W - v) - v)
                                Me.BsOuter2.Width = Me.Width
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                            Case PipeTypes._3
                                Me.BsOuter1.Width = Me.Width
                                Me.BsOuter2.Width = Me.Width - (W - v)
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                            Case PipeTypes._4
                                Me.BsOuter1.Width = Me.Width - (W - v)
                                Me.BsOuter2.Width = Me.Width
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                            Case PipeTypes._5
                                Me.BsOuter1.Width = Me.Width
                                Me.BsOuter2.Width = Me.Width - (W - v) - (W - v)
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point((W - v), W - v)

                            Case PipeTypes._6
                                Me.BsOuter1.Width = Me.Width - (W - v)
                                Me.BsOuter2.Width = Me.Width - (W - v)
                                Me.BsOuter1.Location = New Point(0, 0)
                                Me.BsOuter2.Location = New Point((W - v), W - v)

                            Case PipeTypes._7
                                Me.BsOuter1.Width = Me.Width - (W - v)
                                Me.BsOuter2.Width = Me.Width - (W - v)
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                            Case PipeTypes._8
                                Me.BsOuter1.Width = Me.Width - (W - v) - (W - v)
                                Me.BsOuter2.Width = Me.Width
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, W - v)

                        End Select


                    Case BsScadaControl.Orientations._90, Orientations._270

                        Me.BsOuter1.Width = v
                        Me.BsOuter2.Width = v

                        Select Case pType
                            Case PipeTypes._0
                                Me.BsOuter1.Height = Me.Height
                                Me.BsOuter2.Height = Me.Height
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, 0)

                            Case PipeTypes._1
                                Me.BsOuter1.Height = Me.Height
                                Me.BsOuter2.Height = Me.Height - ((W - v) - v)
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, (W - v))

                            Case PipeTypes._2
                                Me.BsOuter1.Height = Me.Height - ((W - v) - v)
                                Me.BsOuter2.Height = Me.Height
                                Me.BsOuter1.Location = New Point((W - v), (W - v))
                                Me.BsOuter2.Location = New Point(0, 0)

                            Case PipeTypes._3
                                Me.BsOuter1.Height = Me.Height
                                Me.BsOuter2.Height = Me.Height - (W - v)
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, 0)

                            Case PipeTypes._4
                                Me.BsOuter1.Height = Me.Height - (W - v)
                                Me.BsOuter2.Height = Me.Height
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, 0)

                            Case PipeTypes._5
                                Me.BsOuter1.Height = Me.Height
                                Me.BsOuter2.Height = Me.Height - (W - v) - (W - v)
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, (W - v))

                            Case PipeTypes._6
                                Me.BsOuter1.Height = Me.Height - (W - v)
                                Me.BsOuter2.Height = Me.Height - (W - v)
                                Me.BsOuter1.Location = New Point((W - v), 0)
                                Me.BsOuter2.Location = New Point(0, (W - v))

                            Case PipeTypes._7
                                Me.BsOuter1.Height = Me.Height - (W - v)
                                Me.BsOuter2.Height = Me.Height - (W - v)
                                Me.BsOuter1.Location = New Point((W - v), (W - v))
                                Me.BsOuter2.Location = New Point(0, 0)

                            Case PipeTypes._8
                                Me.BsOuter1.Height = Me.Height - (W - v) - (W - v)
                                Me.BsOuter2.Height = Me.Height
                                Me.BsOuter1.Location = New Point((W - v), (W - v))
                                Me.BsOuter2.Location = New Point(0, 0)

                        End Select
                End Select

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BsScadaPipeControl_CursorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CursorChanged
            Try
                If Me.Parent IsNot Nothing Then
                    Me.Cursor = Me.Parent.Cursor
                ElseIf Me.ParentForm IsNot Nothing Then
                    Me.Cursor = Me.ParentForm.Cursor
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub BsPipeControl_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize

            Try
                If Not IsChangingOrientation Then

                    Select Case MyClass.Orientation
                        Case BsScadaControl.Orientations._0, Orientations._180
                            Me.Height = PipeWidth
                            RefreshType(MyClass.PipeType)
                            BsEnd1.Location = New Point(0, 0)
                            BsEnd2.Location = New Point(Me.Width - OuterWidth, 0)

                        Case BsScadaControl.Orientations._90, Orientations._270
                            Me.Width = PipeWidth
                            RefreshType(MyClass.PipeType)
                            BsEnd1.Location = New Point(0, 0)
                            BsEnd2.Location = New Point(0, Me.Height - OuterWidth)
                    End Select

                    Refresh()

                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Throw ex
            End Try


        End Sub

        Private Sub On_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
            Try
                Me.Cursor = Cursors.Hand
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

    End Class
End Namespace