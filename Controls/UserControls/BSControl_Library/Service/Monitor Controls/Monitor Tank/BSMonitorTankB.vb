Option Strict On
Option Explicit On

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Windows.Forms
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing


Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSMonitorTankB
        Inherits BSMonitorTank

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            MyBase.InstrumentPanel = MyClass.InstrumentPanel
            MyBase.InstrumentationControl = MyClass.IndicatorWidget1


            MyBase.TextValueElementName = "LabelValue"

            MyBase.TitleHeight = 0

            Me.Refresh()

        End Sub

        Public Property BackImage() As System.Drawing.Bitmap
            Get
                Return BackImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetBackImage(value)
                    BackImageAttr = value
                End If
            End Set
        End Property


        Private BackImageAttr As System.Drawing.Bitmap = Nothing

        Private Sub SetBackImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyBase.BackImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BSMonitor_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


    End Class

End Namespace