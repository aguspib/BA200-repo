Option Strict On
Option Explicit On

Imports PerpetuumSoft.Instrumentation


Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSMonitorTankA
        Inherits BSMonitorTank

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            MyBase.InstrumentPanel = MyClass.myInstrumentPanel
            MyBase.InstrumentationControl = MyClass.IndicatorWidget1


            MyBase.TextValueElementName = "LabelValue"

            MyBase.TitleHeight = 0

            Me.Refresh()

        End Sub


        Protected Friend ScaleLabelsName As String = "ScaleLabels"
        Protected Friend TicksName As String = "Ticks"


        Public Property ScaleStep() As Integer
            Get
                Return ScaleStepAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then
                    SetScaleStep(value)
                    ScaleStepAttr = value
                End If
            End Set
        End Property

        Public Property ScaleDivisions() As Integer
            Get
                Return ScaleDivisionsAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then
                    SetScaleDivisions(value)
                    ScaleDivisionsAttr = value
                End If
            End Set
        End Property


        Public Property ScaleSubDivisions() As Integer
            Get
                Return ScaleSubDivisionsAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 Then
                    SetScaleSubDivisions(value)
                    ScaleSubDivisionsAttr = value
                End If
            End Set
        End Property


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

        Private ScaleStepAttr As Integer = 50
        Private ScaleDivisionsAttr As Integer = 4
        Private ScaleSubDivisionsAttr As Integer = 5
        Private BackImageAttr As System.Drawing.Bitmap = Nothing


        Private Sub SetScaleStep(ByVal pStep As Integer)
            Try
                Dim myScaleLabels As Model.ScaleLabels = CType(GetElement(MyBase.InstrumentationControl, MyClass.ScaleLabelsName), Model.ScaleLabels)

                If myScaleLabels IsNot Nothing Then
                    myScaleLabels.Step = pStep
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetScaleDivisions(ByVal pDivisions As Integer)
            Try
                Dim myTicks As Model.Ticks = CType(GetElement(MyBase.InstrumentationControl, MyClass.TicksName), Model.Ticks)
                If myTicks IsNot Nothing Then
                    myTicks.Divisions = pDivisions
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetScaleSubDivisions(ByVal pSubDivisions As Integer)
            Try
                Dim myTicks As Model.Ticks = CType(GetElement(MyBase.InstrumentationControl, MyClass.TicksName), Model.Ticks)
                If myTicks IsNot Nothing Then
                    myTicks.SubDivisions = pSubDivisions
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

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

        Private Sub SetTextItem(ByVal pText As String)
            Try
                Dim myLabel As Model.Label = CType(GetElement(MyBase.InstrumentationControl, MyClass.TextItemElementName), Model.Label)

                If myLabel IsNot Nothing Then
                    myLabel.Text = pText
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        'Private Sub SetScaleLabelsStep(ByVal pStep As Integer)
        '    Try

        '        Dim myScaleLabels As Model.ScaleLabels = CType(GetElement(MyBase.InstrumentationControl, MyClass.ScaleLabelsName), Model.ScaleLabels)
        '        If myScaleLabels IsNot Nothing Then

        '        End If
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub
        'Private Sub SetTicksStep(ByVal pStep As Integer)
        '    Try

        '        Dim myTicks As Model.Ticks = CType(GetElement(MyBase.InstrumentationControl, MyClass.TicksName), Model.Ticks)
        '        If myTicks IsNot Nothing Then

        '        End If
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub


        Private Sub SetFontSize(ByVal pSize As Single)
            Try

                Dim myTicks As Model.Ticks = CType(GetElement(MyBase.InstrumentationControl, MyClass.TicksName), Model.Ticks)
                If myTicks IsNot Nothing Then
                    Dim sign As Integer = 1
                    If myTicks.Length >= 0 Then
                        sign = 1
                    Else
                        sign = -1
                    End If
                    myTicks.Length = sign * pSize
                    myTicks.SubLength = sign * pSize / 2
                End If

                Dim myScaleLabels As Model.ScaleLabels = CType(GetElement(MyBase.InstrumentationControl, MyClass.ScaleLabelsName), Model.ScaleLabels)
                If myScaleLabels IsNot Nothing Then
                    myScaleLabels.Font.Size = 0.5 * pSize
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub BSMonitor_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                SetFontSize(CSng(12 * Me.Height / 180))
            Catch ex As Exception
                Throw ex
            End Try
        End Sub



    End Class

End Namespace