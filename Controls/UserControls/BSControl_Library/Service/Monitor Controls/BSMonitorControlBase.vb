Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Windows.Forms
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing

'RH 23/03/2012 Remove every Try/Catch
'It is a bad practice to catch an exception, do nothing with it and throw it again.
'Catch an exception only if you want to do something with it.
'On the other hand, do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
'This is the best way to preserve the exception call stack.
'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

Namespace Biosystems.Ax00.Controls.UserControls

    ''' <summary>
    ''' Base control for all the BSMonitor Controls (that involves Peerpetum)
    ''' </summary>
    ''' <remarks>Created by SGM 1/04/2011</remarks>
    Public MustInherit Class BSMonitorControlBase
        Inherits UserControl

#Region "Constructor"
        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            'MyClass.CurrentStatus = Status.DISABLED

        End Sub
#End Region

#Region "Inheritable Declarations"
        Protected Friend IsLoaded As Boolean = False
        'Protected Friend InstrumentPanel As Panel = Nothing
        Protected Friend InstrumentationControl As IndicatorWidget = Nothing
        Protected Friend TextValueElementName As String = "LabelValue"
        Protected Friend ScaleElementName As String = "Scale"
        Protected Friend BackImageElementName As String = "Image"


#End Region

#Region "Public Enumerates"
        Public Enum Status
            DISABLED
            _ON
            _OFF
            TIMEOUT
            EXCEPTION
        End Enum
#End Region

#Region "Public Properties"

        Public Property CurrentStatus() As Status
            Get
                Return CurrentStatusAttr
            End Get
            Set(ByVal value As Status)

                If value <> CurrentStatusAttr Then
                    Select Case value
                        Case Status.DISABLED
                            Me.TitleLabel.Enabled = True 'False

                        Case Status._ON
                            Me.TitleLabel.Enabled = True

                        Case Status._OFF
                            Me.TitleLabel.Enabled = True

                        Case Status.TIMEOUT
                            Me.TitleLabel.Enabled = True

                        Case Status.EXCEPTION
                            Me.TitleLabel.Enabled = True

                    End Select

                    CurrentStatusAttr = value

                    RaiseEvent StatusChanged(Me)
                End If
            End Set
        End Property


        'control min limit
        Public Property MinLimit() As Double
            Get
                Return MinLimitAttr
            End Get
            Set(ByVal value As Double)
                If value < MaxLimitAttr Then
                    MinLimitAttr = CInt(value)
                    SetScaleLimits()
                End If
            End Set
        End Property

        'control max limit
        Public Property MaxLimit() As Double
            Get
                Return MaxLimitAttr
            End Get
            Set(ByVal value As Double)
                If value > MinLimitAttr Then
                    MaxLimitAttr = CInt(value)
                    SetScaleLimits()
                End If
            End Set
        End Property

        Public Property TitleText() As String
            Get
                Return TitleTextAttr
            End Get
            Set(ByVal value As String)
                MyClass.TitleLabel.Text = value
                TitleTextAttr = value
            End Set
        End Property

        Public Property TitleAlignment() As ContentAlignment
            Get
                Return TitleAlignmentAttr
            End Get
            Set(ByVal value As ContentAlignment)
                MyClass.TitleLabel.TextAlign = value
                TitleAlignmentAttr = value
            End Set
        End Property


        Public Property TitleHeight() As Integer
            Get
                Return TitleHeightAttr
            End Get
            Set(ByVal value As Integer)
                If value >= 0 And value <= Me.Height Then
                    TitleHeightAttr = value
                    RefreshControl()
                End If
            End Set
        End Property

        Public Property TitleFont() As Font
            Get
                Return MyClass.TitleFontAttr
            End Get
            Set(ByVal value As Font)
                MyClass.TitleLabel.Font = value
                TitleFontAttr = value
            End Set
        End Property

        Public Property TitleForeColor() As Color
            Get
                Return TitleForeColorAttr
            End Get
            Set(ByVal value As Color)
                MyClass.TitleLabel.ForeColor = value
                TitleForeColorAttr = value
            End Set
        End Property

        Public Overloads WriteOnly Property Enabled() As Boolean
            Set(ByVal value As Boolean)
                For Each C As Control In MyClass.Controls
                    If TypeOf C Is System.Windows.Forms.Label Then
                        C.Enabled = True
                    Else
                        C.Enabled = value
                    End If
                Next

            End Set
        End Property

#End Region

#Region "Attributes"
        Private CurrentStatusAttr As Status = Status.DISABLED
        Private MinLimitAttr As Double = 0
        Private MaxLimitAttr As Double = 100
        Private TitleTextAttr As String = "Control Title"
        Private TitleAlignmentAttr As ContentAlignment = ContentAlignment.MiddleCenter
        Private TitleVisibleAttr As Boolean = True
        Private BarFrameVisibleAttr As Boolean = True
        Private TitleHeightAttr As Integer = 20
        Private TitleFontAttr As Font = Nothing
        Private TitleForeColorAttr As Color = Color.Black
#End Region

#Region "Public Events"
        Public Event StatusChanged(ByVal sender As System.Object)
#End Region

#Region "Inheritable Methods"


        Protected Friend MustOverride Sub RefreshControl()

        ''' <summary>
        ''' This function handles resizing of the control contents. Usually a InstrumentPanel on controls that implement it.
        ''' </summary>
        ''' <param name="contents">The control that should be addapted to fit inside the parent</param>
        ''' <remarks></remarks>
        Protected Friend Sub UpdateContentsSize(contents As Control)
            'Try
            If contents IsNot Nothing Then
                contents.Height = Me.ClientSize.Height - TitleHeight
            End If

            Me.TitleLabel.Height = Me.TitleHeight
            SetScaleLimits()

            '    'Catch ex As Exception
            '    '    Throw ex
            '    'End Try
        End Sub

        ''' <summary>
        ''' updates the scale limits
        ''' </summary>
        ''' <remarks>Created by SGM 28/04/2011</remarks>
        Protected Friend Sub SetScaleLimits()
            'Try
            If MyClass.IsLoaded Then
                Dim myScale As Model.Scale = CType(GetElement(MyClass.InstrumentationControl, MyClass.ScaleElementName), Model.Scale)

                If myScale IsNot Nothing Then
                    myScale.Minimum = MinLimit
                    myScale.Maximum = MaxLimit
                End If
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Private Methods"

#End Region

#Region "Perpetuum Instrumentation Controls Methods"

#Region "Generics"
        'converts from-to perpetuum unit
        Private Function Convert(ByVal pValue As Double, ByVal pFromUnits As Unit, ByVal pToUnits As Unit) As Object
            'Try
            Return Unit.Convert(pValue, pFromUnits, pToUnits)
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Function

        'get elements by its name
        Protected Friend Function GetElement(ByVal pWidget As IndicatorWidget, ByVal pItemName As String) As Element
            'Try
            If pWidget IsNot Nothing Then
                'Dim myWidgetElement As Element = CType(pWidget.Instrument.GetByName(pItemName), Element)
                Dim myWidgetElement As Element = pWidget.Instrument.GetByName(pItemName) 'RH 23/03/2012 Don't convert apples into apples
                Return myWidgetElement
            Else
                Return Nothing
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Function

        Protected Friend Sub SetElementSolidColor(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pColor As Color)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pElementName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                myWidgetElement.Fill = New SolidFill(pColor)
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

        Protected Friend Sub SetElementGradientColor(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pGradient As GradientColor, ByVal pAngle As Double)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pElementName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                Dim myGradientFill As New MultiGradientFill(pAngle)
                myGradientFill.Colors.Add(pGradient)
                myWidgetElement.Fill = myGradientFill
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

        Protected Friend Sub SetElementMultiGradientColor(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pColors As List(Of Color), ByVal pPortions As List(Of Double), ByVal pAngle As Double)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pElementName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                Dim myMultiGradientFill As New MultiGradientFill(pAngle)
                Dim i As Integer = 0
                For Each C As Color In pColors
                    myMultiGradientFill.Colors.Add(New GradientColor(C, pPortions(i)))
                    i += 1
                Next

                myWidgetElement.Fill = myMultiGradientFill
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

        Protected Friend Sub SetElementSphericalColor(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pColor1 As Color, ByVal pColor2 As Color, ByVal pAngle As Double)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pElementName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                Dim mySphericalFill As New SphericalFill(pColor1, pColor2, pAngle, 0.25)

                myWidgetElement.Fill = mySphericalFill
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

        Protected Friend Sub SetElementVisible(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pVisible As Boolean)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pElementName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                myWidgetElement.Visible = pVisible
            End If

            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Specific Digits"

        Private Sub WriteDigitsValue(ByVal pWidget As IndicatorWidget, ByVal pDigitName As String, ByVal pValue As Double)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pDigitName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pDigitName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                If TypeOf myWidgetElement Is Specialized.Digits Then
                    Dim myDigits As Specialized.Digits = CType(myWidgetElement, Specialized.Digits)
                    myDigits.Text = pValue.ToString
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Specific DigitalText"

        Private Sub WriteDigitalTextValue(ByVal pWidget As IndicatorWidget, ByVal pDigitalTextName As String, ByVal pValue As String)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pDigitalTextName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pDigitalTextName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                If TypeOf myWidgetElement Is Specialized.DigitalText Then
                    Dim myDigitalText As Specialized.DigitalText = CType(myWidgetElement, Specialized.DigitalText)
                    myDigitalText.Text = pValue
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Specific Label"

        Private Sub WriteLabelTextValue(ByVal pWidget As IndicatorWidget, ByVal pLabelName As String, ByVal pValue As String)
            'Try
            'Dim myWidgetElement As Element = CType(GetElement(pWidget, pLabelName), Element)
            Dim myWidgetElement As Element = GetElement(pWidget, pLabelName) 'RH 23/03/2012 Don't convert apples into apples

            If myWidgetElement IsNot Nothing Then
                If TypeOf myWidgetElement Is Model.Label Then
                    Dim myLabelText As Model.Label = CType(myWidgetElement, Model.Label)
                    myLabelText.Text = pValue
                End If
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Inheritable Event handlers"

        Protected Friend Sub BSMonitorControlBase_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'Try
            IsLoaded = True
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

        Protected Friend Sub BSMonitorControlBase_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            'Try
            RefreshControl()
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#End Region

    End Class

End Namespace