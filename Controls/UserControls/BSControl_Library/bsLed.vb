
Imports System.Drawing
Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class bsLed

#Region "Public Enum"

        Public Enum LedColors
            GRAY
            GREEN
            ORANGE
            RED
        End Enum

#End Region

#Region "Declarations"

        'Private myBSMonitorLED As BSMonitorLED

#End Region

#Region "public Properties"

        <Browsable(True), Category("bsLed")> _
        Public Property Title() As String
            Get
                Return TextLabel.Text
            End Get
            Set(ByVal value As String)
                TextLabel.Text = value
            End Set
        End Property

        <Browsable(True), Category("bsLed")> _
        Public Property StateIndex() As Integer
            Get
                Return StateIndexField
            End Get
            Set(ByVal value As Integer)
                value = value Mod 4
                Dim myLedColor As LedColors = CType(value, LedColors)
                Dim myColors As New List(Of Color)


                Select Case myLedColor
                    Case LedColors.GRAY
                        myColors.Add(Color.DarkGray)
                        myColors.Add(Color.WhiteSmoke)

                    Case LedColors.GREEN
                        ' dl 11/05/2011
                        myColors.Add(System.Drawing.Color.FromArgb(255, 96, 233, 75))
                        myColors.Add(System.Drawing.Color.FromArgb(255, 157, 247, 146))

                    Case LedColors.ORANGE
                        ' dl 11/05/2011
                        myColors.Add(System.Drawing.Color.FromArgb(255, 255, 127, 39))
                        myColors.Add(System.Drawing.Color.FromArgb(255, 255, 201, 14))

                    Case LedColors.RED
                        ' dl 11/05/2011
                        myColors.Add(System.Drawing.Color.FromArgb(255, 237, 28, 36))
                        myColors.Add(System.Drawing.Color.FromArgb(255, 242, 91, 99))

                End Select

                BsMonitorLED.LightColor = myColors

                StateIndexField = value
            End Set
        End Property

        <Browsable(True), Category("bsLed")> _
        Public Property StateColor() As LedColors
            Get
                Return CType(StateIndexField, LedColors)
            End Get
            Set(ByVal value As LedColors)
                StateIndex = CType(value, Integer)
            End Set
        End Property

#End Region

#Region "Fields"

        Private StateIndexField As Integer = 0

#End Region

#Region "Constructor"

#End Region

#Region "Private Methods"

#End Region

#Region "Event Handlers"

        Private Sub bsLed_SizeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged
            BsMonitorLED.Left = Me.Width - BsMonitorLED.Width

        End Sub

#End Region

    End Class

End Namespace
