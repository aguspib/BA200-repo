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

    Public Class BsScadaSyringeControl
        Inherits BsScadaControl

        Private OuterRectangleElementName As String = "OuterRectangle"
        Private InnerRectangleElementName As String = "InnerRectangle"
        Private PlungerElementName As String = "Plunger"
        Private CamberElementName As String = "Camber"
        Private NeedleElementName As String = "Needle"

        Public Sub New()

            Try
                ' This call is required by the Windows Form Designer.
                InitializeComponent()

                ' Add any initialization after the InitializeComponent() call.

            Catch ex As Exception
                Throw ex
            End Try

        End Sub


#Region "Public Events"
        
#End Region


#Region "Enumerates"

#End Region

#Region "Public Properties"



#End Region

#Region "Inherited Properties"

        Public Overloads Property ActivationState() As States
            Get
                Return MyBase.ActivationState
            End Get
            Set(ByVal value As States)
                MyBase.ActivationState = value
                MyClass.RefreshCamber()
            End Set
        End Property

        Public Overloads Property FluidColor() As Color
            Get
                Return MyBase.FluidColor
            End Get
            Set(ByVal value As Color)
                MyBase.FluidColor = value
                MyClass.RefreshCamber()
            End Set
        End Property

#End Region



#Region "Attributes"

#End Region

#Region "public Events"
        Public Sub RefreshSyringe()
            Try
                Me.Height = Me.Height - 1
                Me.Height = Me.Height + 1
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Private Methods"


        Private Sub RefreshCamber()
            Try
                Dim myInnerRec As Model.Rectangle = CType(GetElement(Me.ControlWidget, InnerRectangleElementName), Model.Rectangle)
                Dim myPlunger As Model.Rectangle = CType(GetElement(Me.ControlWidget, PlungerElementName), Model.Rectangle)
                Dim myCamber As Model.Rectangle = CType(GetElement(Me.ControlWidget, CamberElementName), Model.Rectangle)

                If myInnerRec IsNot Nothing And myPlunger IsNot Nothing And myCamber IsNot Nothing Then

                    myInnerRec.Recalculate()
                    myCamber.Visible = False
                    Select Case MyBase.ActivationState
                        Case States._OFF
                            myCamber.Center = New Vector(myInnerRec.Center.X, 1.4 * myInnerRec.Center.Y)
                            myCamber.Size = New Vector(myInnerRec.Size.X, 0.68 * myInnerRec.Size.Y)

                        Case States._ON
                            myCamber.Center = New Vector(myInnerRec.Center.X, 2.0 * myInnerRec.Center.Y)
                            myCamber.Size = New Vector(myInnerRec.Size.X, 0.15 * myInnerRec.Size.Y)

                        Case Else
                            myCamber.Center = New Vector(myInnerRec.Center.X, 1.4 * myInnerRec.Center.Y)
                            myCamber.Size = New Vector(myInnerRec.Size.X, 0.68 * myInnerRec.Size.Y)
                            MessageBox.Show("")
                    End Select

                    'MessageBox.Show(myCamber.Size.X.ToString("0") & ";" & myCamber.Size.Y.ToString("0"))
                End If

                SetElementSolidColor(Me.ControlWidget, CamberElementName, MyBase.FluidColor)
                SetElementSolidColor(Me.ControlWidget, NeedleElementName, MyBase.FluidColor)

                myCamber.Visible = True
                myCamber.Recalculate()
                myPlunger.Recalculate()
                Me.ControlWidget.Refresh()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub
#End Region

#Region "Private Event Handlers"

        Private Sub BsScadaSyringeControl_CursorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CursorChanged
            Try
                If Me.Parent IsNot Nothing Then
                    Me.Cursor = Me.Parent.Cursor
                ElseIf Me.ParentForm IsNot Nothing Then
                    Me.Cursor = Me.ParentForm.Cursor
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub BsScadaControl_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                MyBase.ActivatorVisible = False
                Me.Height = 160
                Me.Width = 40
                MyClass.RefreshCamber()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub ControlWidget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ControlWidget.Click
            Try
                MyBase.Selected = True
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region




    End Class
End Namespace