Option Strict On
Option Explicit On

Imports System.Drawing

Imports PerpetuumSoft.Instrumentation.Windows.Forms
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing

Namespace Biosystems.Ax00.Controls.UserControls


    Public Class BsScadaControl


        Public Sub New()
            Try
                ' This call is required by the Windows Form Designer.
                InitializeComponent()

            Catch ex As Exception
                Throw ex
            End Try

        End Sub

#Region "Constants"
        Protected Friend InstrumentationControl As IndicatorWidget = Nothing

        Protected Friend CircleElementName As String = "Circle"
        Protected Friend LabelElementName As String = "Label"

        'Protected Friend OFF_Color As Color = Color.LightGray
        'Protected Friend ON_Color As Color = Color.LawnGreen

        Protected Friend ActuatorWidth As Integer = 25
#End Region

#Region "Public Enumerates"

        Public Enum Orientations
            _0
            _90
            _180
            _270
        End Enum

        Public Enum States
            _NONE
            _OFF
            _ON
        End Enum

#End Region

#Region "Declarations"
        Protected Friend IsLoaded As Boolean = False
#End Region

#Region "Public Events"
        Public Event ControlClicked(ByVal sender As Object)

#End Region

#Region "Inheritable Properties"

        Public Property Orientation() As BsScadaControl.Orientations
            Get
                Return MyClass.OrientationAttr
            End Get
            Set(ByVal value As BsScadaControl.Orientations)


                MyClass.OrientationAttr = value

                Me.Refresh()

            End Set
        End Property

        Public Property ActivationState() As States
            Get
                Return ActivationStateAttr
            End Get
            Set(ByVal value As States)
                If value <> ActivationStateAttr Then
                    ActivationStateAttr = value
                End If

            End Set
        End Property

        Public Property DefaultState() As States
            Get
                Return DefaultStateAttr
            End Get
            Set(ByVal value As States)
                If value <> DefaultStateAttr Then
                    DefaultStateAttr = value
                End If

            End Set
        End Property

        Public Property ActivatedColor() As Color
            Get
                Return ActivatedColorAttr
            End Get
            Set(ByVal value As Color)
                ActivatedColorAttr = value
            End Set
        End Property

        Public Property DeactivatedColor() As Color
            Get
                Return DeactivatedColorAttr
            End Get
            Set(ByVal value As Color)
                DeactivatedColorAttr = value
            End Set
        End Property

        Public Property FluidColor() As Color
            Get
                Return FluidColorAttr
            End Get
            Set(ByVal value As Color)
                FluidColorAttr = value
            End Set
        End Property

        Public Property ActivatorVisible() As Boolean
            Get
                Return ActivatorVisibleAttr
            End Get
            Set(ByVal value As Boolean)
                ActivatorVisibleAttr = value
            End Set
        End Property

        Public Property Identity() As String
            Get
                Return IdentityAttr
            End Get
            Set(ByVal value As String)
                IdentityAttr = value
            End Set
        End Property

        Public Property Group() As String
            Get
                Return GroupAttr
            End Get
            Set(ByVal value As String)
                GroupAttr = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return DescriptionAttr
            End Get
            Set(ByVal value As String)
                DescriptionAttr = value
            End Set
        End Property

        Public Property Selected() As Boolean
            Get
                Return SelectedAttr
            End Get
            Set(ByVal value As Boolean)
                'If value <> SelectedAttr Then
                SelectedAttr = value
                If value Then
                    RaiseEvent ControlClicked(Me)
                End If
                'End If
            End Set
        End Property

        Public Property IsAlarm() As Boolean
            Get
                Return IsAlarmAttr
            End Get
            Set(ByVal value As Boolean)
                IsAlarmAttr = value
                If value Then
                    Me.BackColor = Color.Orange
                Else
                    Me.BackColor = Me.BackgroundColor
                End If
                Me.Refresh()
            End Set
        End Property

        Public Property BackgroundColor() As Color
            Get
                Return BackgroundColorAttr
            End Get
            Set(ByVal value As Color)
                BackgroundColorAttr = value
            End Set
        End Property

        Public Property ToolTipText() As String
            Get
                Return ToolTipTextAttr
            End Get
            Set(ByVal value As String)
                ToolTipTextAttr = value
            End Set
        End Property
#End Region

#Region "Public methods"

#End Region

#Region "Inheritable methods"

        'get elements by its name
        Protected Friend Function GetElement(ByVal pWidget As IndicatorWidget, ByVal pItemName As String) As Element
            Try
                If pWidget IsNot Nothing Then
                    Dim myWidgetElement As Element = CType(pWidget.Instrument.GetByName(pItemName), Element)
                    Return myWidgetElement
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Function


        Protected Friend Sub SetElementSolidColor(ByVal pWidget As IndicatorWidget, ByVal pElementName As String, ByVal pColor As Color)
            Try
                Dim myWidgetElement As Element = CType(GetElement(pWidget, pElementName), Element)

                If myWidgetElement IsNot Nothing Then
                    myWidgetElement.Fill = New SolidFill(pColor)
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Protected Friend Sub UpdateFluidColor(ByRef E As BsScadaControl, ByVal pColor As Color)
            Try
                If TypeOf E Is BsScadaPipeControl Then
                    Dim myPipe As BsScadaPipeControl = CType(E, BsScadaPipeControl)
                    If myPipe IsNot Nothing Then
                        myPipe.FluidColor = pColor
                    End If
                ElseIf TypeOf E Is BsScadaValveControl Then
                    Dim myValve As BsScadaValveControl = CType(E, BsScadaValveControl)
                    If myValve IsNot Nothing Then
                        myValve.FluidColor = pColor
                    End If
                ElseIf TypeOf E Is BsScadaValve3Control Then
                    Dim myValve3 As BsScadaValve3Control = CType(E, BsScadaValve3Control)
                    If myValve3 IsNot Nothing Then
                        myValve3.FluidColor = pColor
                    End If
                ElseIf TypeOf E Is BsScadaPumpControl Then
                    Dim myPump As BsScadaPumpControl = CType(E, BsScadaPumpControl)
                    If myPump IsNot Nothing Then
                        myPump.FluidColor = pColor
                    End If
                ElseIf TypeOf E Is BsScadaSyringeControl Then
                    Dim mySyringe As BsScadaSyringeControl = CType(E, BsScadaSyringeControl)
                    If mySyringe IsNot Nothing Then
                        mySyringe.FluidColor = pColor
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


#End Region

#Region "Attributes"

        Private OrientationAttr As BsScadaControl.Orientations = BsScadaControl.Orientations._0
        Private ActivationStateAttr As States = States._OFF
        Private DefaultStateAttr As States = States._OFF
        Private ActivatedColorAttr As Color = Color.FromArgb(0, 102, 204)
        Private DeactivatedColorAttr As Color = Color.LightGray
        Private FluidColorAttr As Color = Color.Transparent
        Private ActivatorVisibleAttr As Boolean = True
        Private IdentityAttr As String = "ID"
        Private GroupAttr As String = "GROUP"
        Private DescriptionAttr As String = "Element Description"
        Private SelectedAttr As Boolean = False
        Private IsAlarmAttr As Boolean = False
        Private BackgroundColorAttr As Color = Color.Transparent
        Private ToolTipTextAttr As String = ""
#End Region

#Region "Private Event Handlers"

        Private Sub BsScadaControl_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                MyClass.IsLoaded = True
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub BsScadaControl_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.EnabledChanged
            If Not TypeOf sender Is BsScadaMotorControl Then
                If Me.Enabled Then
                    If Me.IsAlarm Then
                        Me.BackColor = Color.Orange
                    Else
                        Me.BackColor = Me.BackgroundColor
                    End If

                Else
                    'QUITAR
                    'Me.BackColor = Color.White
                End If
            End If
        End Sub

#End Region


    End Class
End Namespace