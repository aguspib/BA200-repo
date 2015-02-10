Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

'RH 23/03/2012 Remove every Try/Catch
'It is a bad practice to catch an exception, do nothing with it and throw it again.
'Catch an exception only if you want to do something with it.
'On the other hand, do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
'This is the best way to preserve the exception call stack.
'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSMonitorLED
        Inherits BSMonitorControlBase

        Protected Friend MainCircleElementName As String = "LightCircle"

#Region "Constructor"

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            'InstrumentPanel = InstrumentPanel
            InstrumentationControl = IndicatorWidget1
            InstrumentationControl.HideFocusRectangle = True

        End Sub

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 10/04/2011</remarks>
        Public Overloads Property CurrentStatus() As Status
            Get
                Return CurrentStatusAttr
            End Get
            Set(ByVal value As Status)

                If value <> CurrentStatusAttr Then
                    Select Case value
                        Case Status.DISABLED
                            Dim myColors As New List(Of Color)

                            myColors.Add(Color.Gray)
                            myColors.Add(Color.White)
                            LightColor = myColors

                        Case Status._ON
                            Dim myColors As New List(Of Color)
                            myColors.Add(Color.Green)
                            myColors.Add(Color.LightGreen)
                            LightColor = myColors

                        Case Status._OFF
                            Dim myColors As New List(Of Color)
                            myColors.Add(Color.Red)
                            myColors.Add(Color.Pink)
                            LightColor = myColors

                        Case Status.TIMEOUT
                            Dim myColors As New List(Of Color)
                            myColors.Add(Color.LightGray)
                            myColors.Add(Color.White)
                            LightColor = myColors

                        Case Status.EXCEPTION

                    End Select

                    'correct usage of MyBase here: (no shadowing) MIC
                    MyBase.CurrentStatus = value

                    CurrentStatusAttr = value

                End If
            End Set
        End Property

        ''' <summary>
        ''' sets directly the LED color
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 1/04/2011</remarks>
        Public Property LightColor() As List(Of Color)
            Get
                If LightColorAttr.Count = 0 Then
                    'LightColorAttr.Clear()
                    LightColorAttr.Add(Color.Gray)
                    LightColorAttr.Add(Color.White)
                End If
                Return LightColorAttr
            End Get
            Set(ByVal value As List(Of Color))
                If value.Count = 2 Then
                    LightColorAttr.Clear()
                    LightColorAttr.Add(value(0))
                    LightColorAttr.Add(value(1))
                    SetLightColor(LightColorAttr)
                End If
            End Set
        End Property

        ''' <summary>
        ''' The DesignMode property does not correctly tell you if
        ''' you are in design mode. IsDesignerHosted is a corrected
        ''' version of that property.
        ''' </summary>
        Public ReadOnly Property IsDesignerHosted() As Boolean
            Get
                Dim ctrl As Control = Me

                While ctrl IsNot Nothing
                    If (ctrl.Site IsNot Nothing) AndAlso ctrl.Site.DesignMode Then
                        Return True
                    End If
                    ctrl = ctrl.Parent
                End While
                Return False
            End Get
        End Property

#End Region

#Region "Attributes"

        Private CurrentStatusAttr As Status = Status.DISABLED
        Private LightColorAttr As List(Of Color) = New List(Of Color)

#End Region

#Region "Private Methods"

        Private Sub SetLightColor(ByVal pColors As List(Of Color))
            'Try
            If pColors.Count = 2 Then
                SetElementSphericalColor(InstrumentationControl, MainCircleElementName, pColors(0), pColors(1), 225)
            End If
            'Catch ex As Exception
            '    Throw ex
            'End Try
        End Sub

#End Region

#Region "Private Event Handlers"

#End Region

    End Class

End Namespace