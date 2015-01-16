Option Strict On
Option Explicit On

Imports System.Drawing

Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSMonitorReactionsRotor
        Inherits BSMonitorControlBase


#Region "Constructor"
        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            'MyBase.InstrumentPanel = MyClass.InstrumentPanel
            MyBase.InstrumentationControl = MyClass.IndicatorWidget1
            MyBase.InstrumentationControl.HideFocusRectangle = True

        End Sub
#End Region

        Private Structure WellStruct
            Public WellNumber As Integer
            Public WellSector As RingSector
        End Structure
        Private MeasurementSectors As New List(Of WellStruct)

        Protected Friend BaseCircleElementName As String = "BaseCircle"
        Protected Friend Circle4ElementName As String = "Circle4"

#Region "Public Properties"

        Public ReadOnly Property SelectedWell() As Integer
            Get
                Return SelectedWellAttr
            End Get
        End Property


        Public Property MeasurementWells() As List(Of Integer)
            Get
                Return MeasurementWellsAttr
            End Get
            Set(ByVal value As List(Of Integer))
                If value IsNot Nothing Then
                    If value.Count > 0 And value.Count <= 120 Then
                        Dim ContainsIt As Boolean = False
                        For Each M As Integer In value
                            If MeasurementWellsAttr.Contains(M) Then
                                ContainsIt = True
                            End If
                        Next
                        If Not ContainsIt Then
                            MyClass.SetMeasurementWells(value)
                            MeasurementWellsAttr = value
                        End If
                    End If
                End If

            End Set
        End Property

        Public Property SelectedWellColor() As Color
            Get
                Return SelectedWellColorAttr
            End Get
            Set(ByVal value As Color)
                SelectedWellColorAttr = value
            End Set
        End Property

        Public Property MeasurementWellsColor() As Color
            Get
                Return MeasurementWellsColorAttr
            End Get
            Set(ByVal value As Color)

                MeasurementWellsColorAttr = value
            End Set
        End Property
#End Region

#Region "Attributes"
        Private SelectedWellAttr As Integer = 0
        Private MeasurementWellsAttr As New List(Of Integer)
        Private SelectedWellColorAttr As Color = Color.Red
        Private MeasurementWellsColorAttr As Color = Color.Orange
#End Region

#Region "Private Methods"
        Private Sub SetWellColor(ByVal pWell As Integer, ByVal pColor As Color)
            Try
                If pColor <> Nothing Then

                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetMeasurementWells(ByVal pWells As List(Of Integer))
            Try
                Dim myBaseCircle As Circle = CType(GetElement(Me.IndicatorWidget1, MyClass.BaseCircleElementName), Circle)
                Dim myCircle4 As Circle = CType(GetElement(Me.IndicatorWidget1, MyClass.Circle4ElementName), Circle)

                If myBaseCircle IsNot Nothing And myCircle4 IsNot Nothing Then

                    MeasurementSectors.Clear()

                    For W As Integer = 1 To pWells.Count
                        Dim mySector As New RingSector
                        With mySector
                            .Active = False
                            .Center = myBaseCircle.Center
                            .InternalRadius = myCircle4.Radius / myBaseCircle.Radius
                            .Size = New Vector(2 * myBaseCircle.Radius, 2 * myBaseCircle.Radius)
                            .Angle = 90
                            .StartAngle = 3 * pWells(W)
                            .SweepAngle = 3
                            .Fill = New SolidFill(MyClass.MeasurementWellsColor)
                        End With

                        Dim myWell As WellStruct
                        myWell.WellNumber = W
                        myWell.WellSector = mySector
                        MeasurementSectors.Add(myWell)

                    Next
                    
                End If

                
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region

#Region "Private Event Handlers"

#End Region


    End Class
End Namespace