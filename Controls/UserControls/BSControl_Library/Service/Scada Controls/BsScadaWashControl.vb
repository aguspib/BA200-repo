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

    Public Class BsScadaWashControl
        Inherits BsScadaControl

        Private PictureElementName As String = "Picture"
        Private PictureSetElementName As String = "PictureSet"
        Private FluidRectangleElementName As String = "FluidRec"

        Public Sub New()
            Try
                ' This call is required by the Windows Form Designer.
                InitializeComponent()
                '
                ' Add any initialization after the InitializeComponent() call.
                'MyClass.SetActivationState(MyBase.ActivationState)

            Catch ex As Exception
                Throw ex
            End Try


        End Sub

#Region "Enumerates"

#End Region


#Region "Public Events"

#End Region

#Region "Public Properties"

#End Region

#Region "Inherited Properties"

#End Region



#Region "Attributes"
        Private ControlColorAttr As Color = Color.DimGray

#End Region


#Region "Private Methods"



      

#End Region

#Region "Private Event Handlers"

        Private Sub BsScadaWashControl_CursorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CursorChanged
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


        Private Sub BsWashControl_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                Me.ActuatorWidget.Height = 0
                MyBase.ActivatorVisible = False


            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub On_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ControlWidget.Click, ActuatorWidget.Click
            Try
                MyBase.Selected = True
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub On_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ControlWidget.MouseMove, ActuatorWidget.MouseMove
            Try
                Me.Cursor = Cursors.Hand
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region





        
    End Class
End Namespace
