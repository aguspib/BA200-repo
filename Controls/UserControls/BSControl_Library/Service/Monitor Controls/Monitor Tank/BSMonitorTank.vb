Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00
Imports Biosystems.Ax00.Controls
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Types

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Windows.Forms
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing

Namespace Biosystems.Ax00.Controls.UserControls


    Public Class BSMonitorTank
        Inherits BSMonitorControlBase

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

            MyBase.TextValueElementName = "LabelValue"

            MyBase.TitleHeight = 0

            Me.Refresh()

        End Sub

        Protected Friend TextItemElementName As String = "LabelItem"
        Protected Friend GuideElementName As String = "Guide"
        'Protected Friend ScaleLabelsName As String = "ScaleLabels"
        'Protected Friend TicksName As String = "Ticks"
        Protected Friend FrameElementName As String = "Frame"
        Protected Friend CurrentLevelElementName As String = "CurrentLevel"
        Protected Friend LowerLevelElementName As String = "LineLower"
        Protected Friend UpperLevelElementName As String = "LineUpper"


        'Public Property ScaleStep() As Integer
        '    Get
        '        Return ScaleStepAttr
        '    End Get
        '    Set(ByVal value As Integer)
        '        If value > 0 Then
        '            SetScaleStep(value)
        '            ScaleStepAttr = value
        '        End If
        '    End Set
        'End Property

        Public Property LevelValue() As Double
            Get
                Return LevelValueAttr

            End Get
            Set(ByVal value As Double)
                If value >= MyBase.MinLimit And value <= MyBase.MaxLimit Then
                    MyClass.SetLevelValue(value)
                    LevelValueAttr = value
                End If
            End Set
        End Property

        Public Property LowerLevelValue() As Double
            Get
                Return LowerLevelValueAttr

            End Get
            Set(ByVal value As Double)
                If value >= MyBase.MinLimit And value <= MyBase.MaxLimit Then
                    MyClass.SetLowerLevelValue(value)
                    LowerLevelValueAttr = value
                End If
            End Set
        End Property

        Public Property UpperLevelValue() As Double
            Get
                Return UpperLevelValueAttr

            End Get
            Set(ByVal value As Double)
                If value >= MyBase.MinLimit And value <= MyBase.MaxLimit Then
                    MyClass.SetUpperLevelValue(value)
                    UpperLevelValueAttr = value
                End If
            End Set
        End Property

        Public Property LowerLevelVisible() As Boolean
            Get
                Return LowerLevelVisibleAttr

            End Get
            Set(ByVal value As Boolean)
                MyClass.SetLowerLevelVisible(value)
                LowerLevelVisibleAttr = value
            End Set
        End Property

        Public Property UpperLevelVisible() As Boolean
            Get
                Return UpperLevelVisibleAttr

            End Get
            Set(ByVal value As Boolean)
                MyClass.SetUpperLevelVisible(value)
                UpperLevelVisibleAttr = value
            End Set
        End Property

        'IT CANNOT BE MODIFIED
        'Public Property UpperLevelColor() As Color
        '    Get
        '        Return UpperLevelColorAttr

        '    End Get
        '    Set(ByVal value As Color)
        '        UpperLevelColorAttr = value
        '        MyClass.SetUpperLevelColor(value)
        '    End Set
        'End Property

        'Public Property LowerLevelColor() As Color
        '    Get
        '        Return LowerLevelColorAttr

        '    End Get
        '    Set(ByVal value As Color)
        '        LowerLevelColorAttr = value
        '        MyClass.SetLowerLevelColor(value)
        '    End Set
        'End Property


        Private LowerLevelVisibleAttr As Boolean = False
        Private UpperLevelVisibleAttr As Boolean = False
        Private LowerLevelColorAttr As Color = Color.SteelBlue
        Private UpperLevelColorAttr As Color = Color.SteelBlue
        Private LevelValueAttr As Double = 57.32
        Private LowerLevelValueAttr As Double = 0
        Private UpperLevelValueAttr As Double = 100
        'Private ScaleStepAttr As Integer = 50

        'IT CANNOT BE MODIFIED
        'Private Sub SetUpperLevelColor(ByVal pColor As Color)
        '    Try

        '        Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.UpperLevelElementName), Model.Line)
        '        If myLine IsNot Nothing Then
        '            Dim myStroke As SimpleStroke = CType(myLine.Stroke, SimpleStroke)
        '            myStroke.Color = pColor
        '        End If
        '        Me.Refresh()
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

        'Private Sub SetLowerLevelColor(ByVal pColor As Color)
        '    Try
        '        Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.LowerLevelElementName), Model.Line)
        '        If myLine IsNot Nothing Then
        '            Dim myStroke As SimpleStroke = CType(myLine.Stroke, SimpleStroke)
        '            myStroke.Color = pColor
        '        End If
        '        Me.Refresh()
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

        Private Sub SetLevelValue(ByVal pValue As Double)
            Try
                Dim myLevel As Model.RangedLevel = CType(GetElement(MyBase.InstrumentationControl, MyClass.CurrentLevelElementName), Model.RangedLevel)
                If myLevel IsNot Nothing Then

                    myLevel.Value = pValue
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetLowerLevelValue(ByVal pValue As Double)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyBase.BackImageElementName), Model.Picture)
                Dim myFrame As Model.RoundedRectangle = CType(GetElement(MyBase.InstrumentationControl, MyClass.FrameElementName), Model.RoundedRectangle)
                Dim myGuide As Model.Guide = CType(GetElement(MyBase.InstrumentationControl, MyClass.GuideElementName), Model.Guide)
                Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.LowerLevelElementName), Model.Line)
                If myImage IsNot Nothing And myFrame IsNot Nothing And myGuide IsNot Nothing And myLine IsNot Nothing Then
                    Dim myDif As Double = MyBase.MaxLimit - MyBase.MinLimit
                    With myLine
                        .Visible = False
                        .StartPoint = New Vector(myImage.Center.X - 0.5 * myImage.Size.Width, myGuide.StartPoint.Y - (pValue / myDif) * (myGuide.StartPoint.Y - myGuide.EndPoint.Y))
                        .EndPoint = New Vector(myFrame.Size.Width, myGuide.StartPoint.Y - (pValue / myDif) * (myGuide.StartPoint.Y - myGuide.EndPoint.Y))
                        .Visible = True
                    End With
                End If

                Me.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Private Sub SetUpperLevelValue(ByVal pValue As Double)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyBase.BackImageElementName), Model.Picture)
                Dim myFrame As Model.RoundedRectangle = CType(GetElement(MyBase.InstrumentationControl, MyClass.FrameElementName), Model.RoundedRectangle)
                Dim myGuide As Model.Guide = CType(GetElement(MyBase.InstrumentationControl, MyClass.GuideElementName), Model.Guide)
                Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.UpperLevelElementName), Model.Line)
                If myImage IsNot Nothing And myFrame IsNot Nothing And myGuide IsNot Nothing And myLine IsNot Nothing Then
                    Dim myDif As Double = MyBase.MaxLimit - MyBase.MinLimit
                    With myLine
                        .Visible = False
                        .StartPoint = New Vector(myImage.Center.X - 0.5 * myImage.Size.Width, myGuide.StartPoint.Y - (pValue / myDif) * (myGuide.StartPoint.Y - myGuide.EndPoint.Y))
                        .EndPoint = New Vector(myFrame.Size.Width, myGuide.StartPoint.Y - (pValue / myDif) * (myGuide.StartPoint.Y - myGuide.EndPoint.Y))
                        .Visible = True
                    End With
                End If

                Me.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetLowerLevelVisible(ByVal pVisible As Boolean)
            Try
                Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.LowerLevelElementName), Model.Line)
                If myLine IsNot Nothing Then
                    With myLine
                        .Visible = pVisible
                    End With
                End If

                Me.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetUpperLevelVisible(ByVal pVisible As Boolean)
            Try
                Dim myLine As Model.Line = CType(GetElement(MyBase.InstrumentationControl, MyClass.UpperLevelElementName), Model.Line)
                If myLine IsNot Nothing Then
                    With myLine
                        .Visible = pVisible
                    End With
                End If

                Me.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub




        'Private Sub SetScaleStep(ByVal pStep As Integer)
        '    Try
        '        Dim myScaleLabels As Model.ScaleLabels = CType(GetElement(MyBase.InstrumentationControl, MyClass.ScaleLabelsName), Model.ScaleLabels)

        '        If myScaleLabels IsNot Nothing Then
        '            myScaleLabels.Step = pStep
        '        End If

        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub


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



        'Private Sub SetFontSize(ByVal pSize As Single)
        '    Try

        '        Dim myTicks As Model.Ticks = CType(GetElement(MyBase.InstrumentationControl, MyClass.TicksName), Model.Ticks)
        '        If myTicks IsNot Nothing Then
        '            myTicks.Length = pSize
        '            myTicks.SubLength = pSize / 2
        '        End If

        '        Dim myScaleLabels As Model.ScaleLabels = CType(GetElement(MyBase.InstrumentationControl, MyClass.ScaleLabelsName), Model.ScaleLabels)
        '        If myScaleLabels IsNot Nothing Then
        '            myScaleLabels.Font.Size = 0.5 * pSize
        '        End If

        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub


        'Private Sub BSMonitor_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        '    Try
        '        SetFontSize(CSng(12 * Me.Height / 180))
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub


    End Class

End Namespace