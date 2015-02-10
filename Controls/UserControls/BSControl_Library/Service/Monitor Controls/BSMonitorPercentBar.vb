Option Strict On
Option Explicit On

Imports System.Drawing

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Model


Namespace Biosystems.Ax00.Controls.UserControls


    ''' <summary>
    ''' User Control designed for displaying Percent Values Bar in the Monitor Panel
    ''' Inherits from BSMonitorBase User Control
    ''' </summary>
    ''' <remarks>
    ''' SGM 15/04/2011
    ''' </remarks>
    Public Class BSMonitorPercentBar
        Inherits BSMonitorControlBase

#Region "Constructor"

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            InstrumentationControl = IndicatorWidget1
            InstrumentationControl.HideFocusRectangle = True

            TextValueElementName = "LabelValue"

            InitializeColors()

            SetRealValue(RealValueAttr)
            SetTextValue(CInt(PercentValueAttr).ToString + "%")

        End Sub

#End Region

#Region "Perpeetum Items Identifiers"

        Protected Friend FrameElementName As String = "Frame"
        Protected Friend FrontBarElementName As String = "RangeFront"
        Protected Friend BackBarElementName As String = "RangeBack"
        Protected Friend LinearLevelElementName As String = "BarFront"
        Protected Friend BarFrameElementName As String = "BarFrame"

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Overloads Property CurrentStatus() As Status
            Get
                Return MyBase.CurrentStatus
            End Get
            Set(ByVal value As Status)
                If value <> CurrentStatusAttr Then
                    Select Case value
                        Case Status.DISABLED
                            SetTextForeColor(Color.DimGray)

                        Case Status._ON
                            TextForeColor = TextForeColorAttr

                        Case Status._OFF
                            SetTextForeColor(Color.Red)

                        Case Status.TIMEOUT
                            TextForeColor = TextForeColorAttr

                    End Select

                    MyBase.CurrentStatus = value
                    CurrentStatusAttr = value

                End If
            End Set
        End Property

        ''' <summary>
        ''' Visibility for text
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property TextVisible() As Boolean
            Get
                Return TextVisibleAttr
            End Get
            Set(ByVal value As Boolean)
                MyBase.SetElementVisible(MyBase.InstrumentationControl, MyBase.TextValueElementName, value)
                TextVisibleAttr = value
            End Set
        End Property


        ''' <summary>
        ''' text fore color
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property TextForeColor() As Color
            Get
                Return TextForeColorAttr
            End Get
            Set(ByVal value As Color)
                TextForeColorAttr = value
                MyClass.SetTextForeColor(TextForeColorAttr)
            End Set
        End Property

        ''' <summary>
        ''' text font size
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property TextFontSize() As Single
            Get
                Return TextFontSizeAttr
            End Get
            Set(ByVal value As Single)
                SetFontSize(value)
                TextFontSizeAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Real Value
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property RealValue() As Single
            Get
                Return RealValueAttr
            End Get
            Set(ByVal value As Single)
                RealValueAttr = value
                MyClass.SetRealValue(value)

                Dim myPercent As Single = CSng(100 * ((MyClass.MinLimit + value) / (MyClass.MaxLimit - MyClass.MinLimit)))
                MyClass.PercentValue = myPercent
            End Set
        End Property

        ''' <summary>
        ''' Back bar color
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property BackBarColor() As List(Of Color)
            Get
                If BackBarColorAttr Is Nothing Then
                    InitializeColors()
                End If
                Return BackBarColorAttr
            End Get
            Set(ByVal value As List(Of Color))
                BackBarColorAttr = value
                MyClass.SetBackBarColor(BackBarColorAttr)
            End Set
        End Property

        ''' <summary>
        ''' front bar color
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Public Property FrontBarColor() As List(Of Color)
            Get
                If FrontBarColorAttr Is Nothing Then
                    InitializeColors()
                End If
                Return FrontBarColorAttr
            End Get
            Set(ByVal value As List(Of Color))
                FrontBarColorAttr = value
                MyClass.SetFrontBarColor(FrontBarColorAttr)
            End Set
        End Property

#End Region

#Region "Private Properties"

        ''' <summary>
        ''' percent value
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Property PercentValue() As Double
            Get
                Return PercentValueAttr
            End Get
            Set(ByVal value As Double)
                If value >= MyBase.MinLimit And value <= MyBase.MaxLimit Then

                    PercentValueAttr = value

                    MyClass.SetTextValue(CInt(PercentValueAttr).ToString + "%")

                End If
            End Set
        End Property

#End Region

#Region "Attributes"
        Private CurrentStatusAttr As Status = Status.DISABLED
        Private BackBarColorAttr As New List(Of Color)
        Private FrontBarColorAttr As New List(Of Color)
        Private RealValueAttr As Single = 50
        Private PercentValueAttr As Double = 50
        Private TextVisibleAttr As Boolean = True
        Private TextFontSizeAttr As Single = 48
        Private TextValueAttr As String = "0%"
        Private TextForeColorAttr As Color = Color.SteelBlue

#End Region


#Region "Private Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetBackBarColor(ByVal pColors As List(Of Color))
            Try
                If pColors IsNot Nothing AndAlso pColors.Count = 3 Then
                    Dim myPortions As New List(Of Double)
                    myPortions.Add(0)
                    myPortions.Add(0.8)
                    myPortions.Add(1)
                    MyBase.SetElementMultiGradientColor(MyBase.InstrumentationControl, MyClass.BackBarElementName, pColors, myPortions, 90)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetFrontBarColor(ByVal pColors As List(Of Color))
            Try
                If pColors IsNot Nothing AndAlso pColors.Count = 3 Then
                    Dim myPortions As New List(Of Double)
                    myPortions.Add(0)
                    myPortions.Add(0.3)
                    myPortions.Add(1)
                    MyBase.SetElementMultiGradientColor(MyBase.InstrumentationControl, MyClass.FrontBarElementName, pColors, myPortions, 90)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetRealValue(ByVal pValue As Double)
            Try
                Dim myWidgetElement As Element = CType(GetElement(MyBase.InstrumentationControl, MyClass.FrontBarElementName), Element)

                If myWidgetElement IsNot Nothing Then
                    If TypeOf myWidgetElement Is Model.RangedLevel Then
                        Dim myRangedLevel As Model.RangedLevel = CType(myWidgetElement, Model.RangedLevel)
                        myRangedLevel.Value = pValue
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetTextValue(ByVal pText As String)
            Try
                Dim myLabel As Model.Label = CType(GetElement(Me.IndicatorWidget1, MyBase.TextValueElementName), Model.Label)

                If myLabel IsNot Nothing Then
                    myLabel.Text = pText
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetFontSize(ByVal pSize As Single)
            Try
                Dim myLabel As Model.Label = CType(GetElement(Me.IndicatorWidget1, MyBase.TextValueElementName), Model.Label)

                If myLabel IsNot Nothing Then
                    myLabel.Font.Size = pSize
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub SetTextForeColor(ByVal pColor As Color)
            Try
                Dim myLabel As Model.Label = CType(GetElement(Me.IndicatorWidget1, MyBase.TextValueElementName), Model.Label)

                If myLabel IsNot Nothing Then
                    myLabel.Fill = New PerpetuumSoft.Framework.Drawing.SolidFill(pColor)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub InitializeColors()
            Try

                'initialize colors
                MyClass.BackBarColorAttr = New List(Of Color)
                MyClass.FrontBarColorAttr = New List(Of Color)

                BackBarColorAttr.Add(Color.Gray)
                BackBarColorAttr.Add(Color.White)
                BackBarColorAttr.Add(Color.DarkGray)

                FrontBarColorAttr.Add(Color.LimeGreen)
                FrontBarColorAttr.Add(Color.Honeydew)
                FrontBarColorAttr.Add(Color.LightGreen)


            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Private Event Handlers"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub BSMonitorProgressText_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                Me.InstrumentPanel.Height = Me.Height - MyClass.TitleHeight
                SetFontSize(CSng((48 * Me.Height / 125) * 0.5))
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' SGM 15/04/2011
        ''' </remarks>
        Private Sub InstrumentPanel_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InstrumentPanel.Resize
            Try
                SetFontSize(CSng((48 * Me.Height / 125) * 0.5))
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region


    End Class

End Namespace
