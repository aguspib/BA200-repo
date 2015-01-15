Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' user control used for displaying values
    ''' </summary>
    ''' <remarks>Created by SGM 10/04/2011</remarks>
    Public Class BSMonitorDigitLabel
        Inherits BSMonitorControlBase

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            MyBase.InstrumentPanel = MyClass.InstrumentPanel

        End Sub


#Region "Public Properties"
        Public Overloads Property CurrentStatus() As Status
            Get
                Return MyBase.CurrentStatus
            End Get
            Set(ByVal value As Status)

                If value <> CurrentStatusAttr Then
                    Select Case value
                        Case Status.DISABLED

                            Select Case CurrentStatusAttr
                                Case Status._ON
                                    MyClass.DigitForeColor = Color.LightGreen
                                Case Status._OFF
                                    MyClass.DigitForeColor = Color.LightCoral
                                Case Else
                                    Me.DigitLabel.Enabled = False
                                    MyClass.DigitForeColor = Color.LightGray
                            End Select


                        Case Status._ON
                            Me.DigitLabel.Enabled = True
                            MyClass.DigitForeColor = Color.LimeGreen

                        Case Status._OFF
                            Me.DigitLabel.Enabled = True
                            MyClass.DigitForeColor = Color.Red

                        Case Status.TIMEOUT
                            Me.DigitLabel.Enabled = True
                            MyClass.DigitForeColor = Color.LightGray

                        Case Status.DISABLED
                            Me.DigitLabel.Enabled = False
                            MyClass.DigitForeColor = Color.LightGray

                    End Select

                    MyBase.CurrentStatus = value

                    CurrentStatusAttr = value

                End If
            End Set
        End Property

        Public Property DigitSize() As Single
            Get
                Return DigitSizeAttr
            End Get
            Set(ByVal value As Single)

                MyClass.SetDigitSize(value)

                DigitSizeAttr = value
            End Set
        End Property

        

        Public Property DigitUnitsFont() As Font
            Get
                Return DigitUnitsFontAttr
            End Get
            Set(ByVal value As Font)

                MyClass.SetDigitUnitsFont(value)

                DigitUnitsFontAttr = value
            End Set
        End Property

        Public Property DigitValue() As Double
            Get
                Return DigitValueAttr
            End Get
            Set(ByVal value As Double)

                MyClass.SetDigitText(value.ToString(DigitFormat))

                DigitValueAttr = value
            End Set
        End Property

        Public Property DigitFormat() As String
            Get
                Return DigitFormatAttr
            End Get
            Set(ByVal value As String)
                DigitFormatAttr = value
                MyClass.SetDigitText(DigitValue.ToString(value))
            End Set
        End Property

        Public Property DigitForeColor() As Color
            Get
                Return DigitForeColorAttr
            End Get
            Set(ByVal value As Color)
                DigitForeColorAttr = value
                MyClass.SetDigitForeColor(value)
            End Set
        End Property

        Public Property DigitUnitsForeColor() As Color
            Get
                Return DigitUnitsForeColorAttr
            End Get
            Set(ByVal value As Color)
                DigitUnitsForeColorAttr = value
                MyClass.SetDigitUnitsForeColor(value)
            End Set
        End Property

        Public Property DigitAlignment() As ContentAlignment
            Get
                Return DigitAlignmentAttr
            End Get
            Set(ByVal value As ContentAlignment)
                MyClass.DigitLabel.TextAlign = value
                DigitAlignmentAttr = value
            End Set
        End Property

        Public Property DigitBorderStyle() As BorderStyle
            Get
                Return DigitBorderStyleAttr
            End Get
            Set(ByVal value As BorderStyle)
                MyClass.DigitLabel.BorderStyle = value
                DigitBorderStyleAttr = value
            End Set
        End Property

        Public Property DigitUnits() As String
            Get
                Return DigitUnitsAttr
            End Get
            Set(ByVal value As String)
                DigitUnitsAttr = value
                Me.UnitsLabel.Text = value
                Me.UnitsLabel.Visible = (value.Length > 0)
                MyClass.SetDigitText(DigitValue.ToString(value))

            End Set
        End Property
#End Region

#Region "Attributes"
        Private CurrentStatusAttr As Status = Status.DISABLED
        Private DigitValueAttr As Double = 99.9
        Private DigitForeColorAttr As Color = Color.LimeGreen
        Private DigitUnitsForeColorAttr As Color = Color.LimeGreen
        Private DigitFormatAttr As String = "0.0"
        Private DigitUnitsAttr As String = ""
        Private DigitSizeAttr As Single = 18
        Private DigitUnitsFontAttr As Font = Me.Font
        Private DigitAlignmentAttr As ContentAlignment = ContentAlignment.MiddleCenter
        Private DigitBorderStyleAttr As BorderStyle = System.Windows.Forms.BorderStyle.None
#End Region


        Private Sub SetDigitSize(ByVal pSize As Single)
            Try
                Dim myFont As Font = MyClass.DigitLabel.Font
                Me.DigitLabel.Font = New Font(myFont.FontFamily, pSize, myFont.Style, myFont.Unit)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub SetDigitUnitsFont(ByVal pFont As Font)
            Try
                Me.UnitsLabel.Font = pFont
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetDigitText(ByVal pText As String)
            Try
                MyClass.DigitLabel.Text = pText.Replace(",", ".")

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetDigitForeColor(ByVal pColor As Color)
            Try
                Me.DigitLabel.ForeColor = pColor
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetDigitUnitsForeColor(ByVal pColor As Color)
            Try
                Me.UnitsLabel.ForeColor = pColor
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSMonitorDigitLabel_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                With Me.DigitLabel
                    .Top = CInt((Me.LabelPanel.Height - Me.DigitLabel.Height) / 2)
                    .Left = CInt((Me.LabelPanel.Width - Me.DigitLabel.Width) / 2)
                End With
                With Me.UnitsLabel
                    .Top = CInt((Me.DigitLabel.Top + Me.DigitLabel.Height - Me.UnitsLabel.Height))
                    .Left = CInt((Me.DigitLabel.Left + Me.DigitLabel.Width))
                End With
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        
    End Class

End Namespace